using System;

namespace ArkBackup
{
    class ArkBackup
    {
        static void Main()
        {
            var watcher = new SaveWatcher();

            Console.ReadLine();

            watcher.Dispose();
        }
    }
}
