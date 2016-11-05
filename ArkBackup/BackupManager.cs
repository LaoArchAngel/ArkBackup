using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArkBackup
{
    class BackupManager
    {
        private readonly Dictionary<int, FileInfo> _backups = new Dictionary<int, FileInfo>();
        private readonly int _numBackups;

        public BackupManager(int backups = 20)
        {
            _numBackups = 20;

            LoadCurrentBackups();
        }

        /// <summary>
        /// Prunes backups that are missing or 
        /// </summary>
        private void PruneBackups()
        {
            if(_backups.Count <= _numBackups)
                return;

            var missing = _backups.Where(backup => !backup.Value.Exists).Select(backup => backup.Key);

            foreach (var missingKey in missing)
            {
                _backups.Remove(missingKey);
            }

            if (_backups.Count <= _numBackups)
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
                int date = Convert.ToInt32(backup.Name.Split('.', '_')[1]);
                _backups.Add(date, backup);
            }
        }

        public void CreateBackup(IEnumerable<FileInfo> files)
        {
            
        }
    }
}
