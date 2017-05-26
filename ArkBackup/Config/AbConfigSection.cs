using System.Configuration;
using System.Runtime.InteropServices.ComTypes;

namespace ArkBackup.Config
{
    public class AbConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("map", DefaultValue = "TheIsland", IsRequired = false)]
        public string Map
        {
            get => (string) this["map"];
            set => this["map"] = value;
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get => (string) this["path"];
            set => this["path"] = value;
        }

        [ConfigurationProperty("saves", IsRequired = false, DefaultValue = 10)]
        public int Saves
        {
            get => (int) this["saves"];
            set => this["saves"] = value;
        }

        [ConfigurationProperty("delay", IsRequired = false, DefaultValue = 10)]
        public int Delay
        {
            get => (int) this["delay"];
            set => this["delay"] = value;
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get => (string) this["name"];
            set => this["name"] = value;
        }
    }
}