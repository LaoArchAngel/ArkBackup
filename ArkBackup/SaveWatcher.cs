using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ArkBackup
{
    /// <summary>
    ///     Watches the save directory for changes to the effective save.
    /// </summary>
    internal class SaveWatcher
    {
        /// <summary>
        ///     <c>Directory</c> we're watching.
        /// </summary>
        private static readonly DirectoryInfo SaveDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        ///     Backup manager used to create backups of our watched saves.
        /// </summary>
        private readonly RollingBackups _mgr = new RollingBackups();

        /// <summary>
        ///     Watcher checking when the the save changes.
        /// </summary>
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        /// <summary>
        ///     Initializes the <see cref="FileSystemWatcher" /> . Determines save
        ///     by looking for shortest filename with a .ark extension.
        /// </summary>
        public SaveWatcher()
        {
            IEnumerable<FileInfo> saveFiles = SaveDir.GetFiles("*.ark", SearchOption.TopDirectoryOnly);
            var save = saveFiles.OrderBy(saveFile => saveFile.Length).First();

            _watcher.Filter = save.Name;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Changed += SaveChanged;
        }

        /// <summary>
        ///     Handles <see cref="System.IO.FileSystemWatcher.Changed" /> event.
        /// </summary>
        /// <param name="source">Likely the watcher itself.</param>
        /// <param name="args">Arguments given by the watcher.</param>
        private void SaveChanged(object source, FileSystemEventArgs args)
        {
            Thread.Sleep(TimeSpan.FromMinutes(1));

            var toBackup = new List<FileInfo> {new FileInfo(args.FullPath)};
            toBackup.AddRange(SaveDir.GetFiles(@"*.arkprofile"));

            _mgr.CreateBackup(toBackup);
        }
    }
}