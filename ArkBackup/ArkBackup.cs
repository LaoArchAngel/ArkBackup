using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ArkBackup.Config;

namespace ArkBackup
{
    class ArkBackup
    {
        static void Main()
        {
            var configGroup = (ConfigurationSectionGroup) ConfigurationManager.GetSection("ArkBackupGroup");
            IEnumerable<AbConfigSection> configs = configGroup.Sections.OfType<AbConfigSection>();
            var watchers = new Stack<SaveWatcher>();

            foreach (var abConfigSection in configs)
            {
                watchers.Push(new SaveWatcher(abConfigSection.Name, abConfigSection.Path, abConfigSection.Delay,
                    new RollingBackups(abConfigSection.Path, abConfigSection.Saves)));
            }

            Console.ReadLine();

            foreach (var saveWatcher in watchers)
            {
                saveWatcher.Dispose();
            }
        }
    }
}
