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
            if(_backups.Count <= _backupCount)
                return;

            var missing = _backups.Where(backup => !backup.Value.Exists).Select(backup => backup.Key);

            foreach (var missingKey in missing)
            {
                _backups.Remove(missingKey);
            }

            if (_backups.Count <= _backupCount)
            {
                return;
            }

            _backups.Remove(_backups.Keys.Max());
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
            SevenZipCompressor compressor = new SevenZipCompressor();

            string archiveName = $"SaveBackup_{DateTime.Now:yyyyMMddHH}.7z";
            archiveName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, archiveName);
            compressor.CompressFiles(archiveName, files.Select(info => info.FullName).ToArray());

            PruneBackups();
        }
    }
}
