using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SevenZip;

namespace ArkBackup
{
    /// <summary>
    /// Creates and prunes backups.
    /// </summary>
    internal class RollingBackups
    {
        /// <summary>
        /// A list of our current backups.  The key is the time stamp of creation
        /// </summary>
        private readonly Dictionary<long, FileInfo> _backups = new Dictionary<long, FileInfo>();

        /// <summary>
        /// The number of backups to keep.
        /// </summary>
        private readonly int _backupCount;

        /// <summary>
        /// Creates a new RollingBackups instance specifying the number of backup files to keep.
        /// </summary>
        /// <param name="backups">The number of backup files to keep.  The oldest are deleted.</param>
        public RollingBackups(int backups = 20)
        {
            _backupCount = backups;

            LoadCurrentBackups();
        }

        /// <summary>
        /// Prunes backups that are missing or 
        /// </summary>
        private void PruneBackups()
        {
            PruneMissing();

            if (_backups.Count <= _backupCount)
            {
                return;
            }

            IEnumerable<long> toRemove = _backups.Keys.OrderBy(key => key);
            toRemove = toRemove.Skip(_backupCount);

            foreach (var key in toRemove)
            {
                _backups[key].Delete();
                _backups.Remove(key);
            }            
        }

        /// <summary>
        /// Prunes all missing backups from our current list.
        /// </summary>
        private void PruneMissing()
        {
            var missing = _backups.Where(backup => !backup.Value.Exists).Select(backup => backup.Key);

            foreach (var missingKey in missing)
            {
                _backups.Remove(missingKey);
            }
        }

        /// <summary>
        /// Loads all current backup files into our list for management.
        /// </summary>
        private void LoadCurrentBackups()
        {
            var saveDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var currentBackups = saveDir.GetFiles("SaveBackup_*.7z");

            foreach (var backup in currentBackups)
            {
                long date = Convert.ToInt64(backup.Name.Split('.', '_')[1]);
                _backups.Add(date, backup);
            }
        }

        /// <summary>
        /// Creates a .7z file with all save <paramref name="files"/> compressed.
        /// </summary>
        /// <param name="files">Files to be included in the backup</param>
        public void CreateBackup(IEnumerable<FileInfo> files)
        {
            SevenZipCompressor.SetLibraryPath(Path.Combine(@"C:\Program Files\7-Zip", @"7z.dll"));
            SevenZipCompressor compressor = new SevenZipCompressor();

            var timestamp = TimeStamp();

            string archiveName = $"SaveBackup_{timestamp}.7z";
            archiveName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, archiveName);

            Console.WriteLine("Creating backup: {0}", archiveName);
            Console.WriteLine("Backing up files:");

            foreach (var fileInfo in files)
            {
                Console.WriteLine(fileInfo.FullName);
            }

            compressor.CompressFiles(archiveName, files.Select(info => info.FullName).ToArray());
            _backups.Add(timestamp, new FileInfo(archiveName));

            PruneBackups();

            Console.WriteLine();
        }

        /// <summary>
        /// Returns the year as a 64-bit integer in the format <c>yyyyMMddHHmm</c>
        /// </summary>
        /// <returns>The current time stamp, up to the minute.</returns>
        private static long TimeStamp()
        {
            long timestamp = (long) DateTime.Now.Year*100000000;
            timestamp += DateTime.Now.Month*1000000;
            timestamp += DateTime.Now.Day*10000;
            timestamp += DateTime.Now.Hour*100;
            timestamp += DateTime.Now.Minute;
            return timestamp;
        }
    }
}
