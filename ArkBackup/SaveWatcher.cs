using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ArkBackup
{
    /// <summary>
    ///     Watches the save directory for changes to the effective save.
    /// </summary>
    internal class SaveWatcher : IDisposable
    {
        /// <summary>
        ///     <c>Directory</c> we're watching.
        /// </summary>
        private static DirectoryInfo _saveDir;

        private readonly int _backupDelay;

        /// <summary>
        ///     Backup manager used to create backups of our watched saves.
        /// </summary>
        private readonly RollingBackups _mgr;

        /// <summary>
        ///     Watcher checking when the the save changes.
        /// </summary>
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        private readonly FileInfo _saveFile;

        /// <summary>
        ///     Initializes the <see cref="FileSystemWatcher" /> . Determines save
        ///     by looking for shortest filename with a .ark extension.
        /// </summary>
        public SaveWatcher(string saveName, string path, int backupDelay, RollingBackups backup)
        {
            _saveDir = new DirectoryInfo(path);
            _backupDelay = backupDelay;
            _mgr = backup;
            _saveFile = new FileInfo(Path.Combine(path, saveName + ".ark"));

            Console.WriteLine("Watching File: {0}", saveName);

            _watcher.Path = _saveDir.FullName;
            _watcher.Filter = $@"{saveName}.tmp";
            _watcher.Changed += SaveChanged;
            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        ///     Implement the <see cref="IDisposable" /> pattern.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Handles <see cref="FileSystemWatcher.Changed" /> event.
        /// </summary>
        /// <param name="source">Likely the watcher itself.</param>
        /// <param name="args">Arguments given by the watcher.</param>
        private void SaveChanged(object source, FileSystemEventArgs args)
        {
            Console.WriteLine("Change detected: {0} :: {1}.  Sleeping...", args.FullPath, args.ChangeType);
            Thread.Sleep(TimeSpan.FromSeconds(_backupDelay));

            var toBackup = new List<FileInfo> {_saveFile};
            toBackup.AddRange(_saveDir.GetFiles(@"*.arkprofile"));
            toBackup.AddRange(_saveDir.GetFiles(@"*.arktribe"));

            var toPrune =
                _saveDir.GetFiles(_saveFile.Name.Substring(0, _saveFile.Name.Length - 4) + "_??.??.????_??.??.??.ark",
                    SearchOption.TopDirectoryOnly);

            foreach (var saveFile in toPrune)
            {
                saveFile.Delete();
            }

            _mgr.CreateBackup(toBackup);
        }

        /// <summary>
        ///     Dispose of our watcher.
        /// </summary>
        /// <param name="disposing"><c>true</c> if we're calling from <see cref="IDisposable.Dispose" />, otherwise false.</param>
        // ReSharper disable once FlagArgument
        private void Dispose(bool disposing)
        {
            if (disposing)
                _watcher.Dispose();
        }

        /// <summary>
        ///     Safely dispose of objects.
        /// </summary>
        ~SaveWatcher()
        {
            Dispose(false);
        }
    }
}