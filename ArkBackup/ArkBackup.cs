using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ArkBackup.Config;

namespace ArkBackup
{
    internal class ArkBackup
    {
        private static void Main()
        {
            AbConfigSection section = ConfigurationManager.GetSection("ArkBackupConfig") as AbConfigSection;
            var watchers = new Stack<SaveWatcher>();

            foreach (Save abConfigSection in section.Saves.OfType<Save>())
            {
                watchers.Push(
                    new SaveWatcher(
                        abConfigSection.Map, abConfigSection.Path, abConfigSection.Delay,
                        new RollingBackups(abConfigSection.Path, abConfigSection.Name?.Trim() ?? abConfigSection.Map, abConfigSection.Saves)));
            }

            Console.ReadLine();

            foreach (SaveWatcher saveWatcher in watchers)
            {
                saveWatcher.Dispose();
            }
        }
    }
}