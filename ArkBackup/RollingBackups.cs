using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SevenZip;

namespace ArkBackup
{
    /// <summary>
    ///     Creates and prunes backups.
    /// </summary>
    internal class RollingBackups
    {
        /// <summary>
        ///     The number of backups to keep.
        /// </summary>
        private readonly int _backupCount;

        /// <summary>
        ///     A list of our current backups. The key is the time stamp of creation
        /// </summary>
        private readonly Dictionary<long, FileInfo> _backups = new Dictionary<long, FileInfo>();

        /// <summary>
        ///     Creates a new RollingBackups instance specifying the number of
        ///     backup files to keep.
        /// </summary>
        /// <param name="backups">
        ///     The number of backup files to keep. The oldest are deleted.
        /// </param>
        public RollingBackups(int backups = 20)
        {
            _backupCount = backups;

            LoadCurrentBackups();
            PruneBackups();
        }

        /// <summary>
        ///     Prunes backups that are missing or
        /// </summary>
        private void PruneBackups()
        {
            PruneMissing();

            if (_backups.Count <= _backupCount)
                return;

            IEnumerable<long> toRemove = _backups.Keys.OrderByDescending(key => key);
            toRemove = toRemove.Skip(_backupCount);

            foreach (var key in toRemove)
            {
                _backups[key].Delete();
                _backups.Remove(key);
            }
        }

        /// <summary>
        ///     Prunes all missing backups from our current list.
        /// </summary>
        private void PruneMissing()
        {
            var missing = _backups.Where(backup => !backup.Value.Exists).Select(backup => backup.Key);

            foreach (var missingKey in missing)
                _backups.Remove(missingKey);
        }

        /// <summary>
        ///     Loads all current backup files into our list for management.
        /// </summary>
        private void LoadCurrentBackups()
        {
            var saveDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var currentBackups = saveDir.GetFiles("SaveBackup_*.7z");

            foreach (var backup in currentBackups)
            {
                var date = Convert.ToInt64(backup.Name.Split('.', '_')[1]);
                _backups.Add(date, backup);
            }
        }

        /// <summary>
        ///     <para>
        ///         Creates a .7z file with all save <paramref name="files" />
        ///     </para>
        ///     <para>compressed.</para>
        /// </summary>
        /// <param name="files">Files to be included in the backup</param>
        public void CreateBackup(IEnumerable<FileInfo> files)
        {
            SevenZipBase.SetLibraryPath(Path.Combine(@"C:\Program Files\7-Zip", @"7z.dll"));
            var compressor = new SevenZipCompressor();

            var timestamp = TimeStamp();
            string archiveName = $"SaveBackup_{timestamp}.7z";
            var fileList = files.Select(info => info.FullName).ToArray();

            archiveName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, archiveName);

            LogBackup(archiveName, fileList);

            compressor.CompressFiles(archiveName, fileList);
            _backups.Add(timestamp, new FileInfo(archiveName));

            PruneBackups();

            Console.WriteLine();
        }

        /// <summary>
        ///     Writes to current log output the name of the backup and included
        ///     files.
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="fileList"></param>
        private static void LogBackup(string archiveName, string[] fileList)
        {
            Console.WriteLine("Creating backup: {0}", archiveName);
            Console.WriteLine("Backing up files:");

            foreach (var file in fileList)
                Console.WriteLine(file);
        }

        /// <summary>
        ///     Returns the year as a 64-bit integer in the format
        ///     <c>yyyyMMddHHmm</c>
        /// </summary>
        /// <returns>
        ///     The current time stamp, up to the minute.
        /// </returns>
        private static long TimeStamp()
        {
            var timestamp = (long) DateTime.Now.Year*100000000;
            timestamp += DateTime.Now.Month*1000000;
            timestamp += DateTime.Now.Day*10000;
            timestamp += DateTime.Now.Hour*100;
            timestamp += DateTime.Now.Minute;
            return timestamp;
        }
    }
}