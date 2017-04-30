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
            var configGroup = (ConfigurationSectionGroup) ConfigurationManager.GetSection("ArkBackupGroup");
            IEnumerable<AbConfigSection> configs = configGroup.Sections.OfType<AbConfigSection>();
            var watchers = new Stack<SaveWatcher>();

            foreach (AbConfigSection abConfigSection in configs)
            {
                watchers.Push(
                    new SaveWatcher(
                        abConfigSection.Name, abConfigSection.Path, abConfigSection.Delay,
                        new RollingBackups(abConfigSection.Path, abConfigSection.Saves)));
            }

            Console.ReadLine();

            foreach (SaveWatcher saveWatcher in watchers)
            {
                saveWatcher.Dispose();
            }
        }
    }
}