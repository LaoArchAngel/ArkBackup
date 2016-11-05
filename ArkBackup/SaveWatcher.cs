using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArkBackup
{
    class SaveWatcher
    {
        /// <summary>
        /// Watcher checking when the the save changes.
        /// </summary>
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        /// <summary>
        /// Number of backups to keep.
        /// </summary>
        private readonly int _backups = 20;

        public SaveWatcher()
        {
            DirectoryInfo saveDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            IEnumerable<FileInfo> saveFiles = saveDir.GetFiles("*.ark", SearchOption.TopDirectoryOnly);
            FileInfo save = saveFiles.OrderBy(saveFile => saveFile.Length).First();

            _watcher.Filter = save.Name;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Changed += SaveChanged;
        }

        public SaveWatcher(int backups) : this()
        {
            _backups = backups;
        }

        private void SaveChanged(object source, FileSystemEventArgs args)
        {
            
        }


    }
}
