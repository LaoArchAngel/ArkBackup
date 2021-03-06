﻿using System;
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
        private readonly string _savePath;
        private readonly string _name;

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
        /// <param name="savePath">Path to place the backups</param>
        /// <param name="name">Name of the backups</param>
        /// <param name="backups">
        ///     The number of backup files to keep. The oldest are deleted.
        /// </param>
        public RollingBackups(string savePath, string name, int backups = 20)
        {
            _savePath = savePath;
            _name = name;
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

            var timestamp = TimeStamp();
            string archiveName = $"SaveBackup_{_name}_{timestamp}.7z";
            archiveName = Path.Combine(_savePath, archiveName);

            var fileList = files.Select(info => info.FullName).ToArray();

            LogBackup(archiveName, fileList);
            Compress(archiveName, fileList);
            _backups.Add(timestamp, new FileInfo(archiveName));

            PruneBackups();
            Console.WriteLine();
        }

        private static void Compress(string archiveName, string[] fileList)
        {
            SevenZipBase.SetLibraryPath(@".\7z.dll");
            var compressor = new SevenZipCompressor();

            compressor.CompressFiles(archiveName, fileList);
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