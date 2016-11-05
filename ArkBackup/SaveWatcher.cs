using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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

        /// <summary>
        /// <c>Directory</c> we're watching.
        /// </summary>
        private static readonly DirectoryInfo SaveDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        private readonly BackupManager _mgr = new BackupManager();

        public SaveWatcher()
        {
            IEnumerable<FileInfo> saveFiles = SaveDir.GetFiles("*.ark", SearchOption.TopDirectoryOnly);
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
            Thread.Sleep(TimeSpan.FromMinutes(1));

            List<FileInfo> toBackup = new List<FileInfo> {new FileInfo(args.FullPath)};
            toBackup.AddRange(SaveDir.GetFiles(@"*.arkprofile"));

            _mgr.CreateBackup(toBackup);
        }


    }
}
