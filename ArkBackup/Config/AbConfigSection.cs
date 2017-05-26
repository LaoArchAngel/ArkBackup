using System.Configuration;

namespace ArkBackup.Config
{
    public class AbConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Saves", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(SavesCollection), AddItemName = "add")]
        public SavesCollection Saves => (SavesCollection) base["Saves"];
    }

    public class SavesCollection : ConfigurationElementCollection
    {
        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new Save();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            Save save = (Save) element;
            return save.Path + (save.Name ?? save.Map);
        }
    }

    public class Save : ConfigurationElement
    {
        [ConfigurationProperty("map", DefaultValue = "TheIsland", IsRequired = false)]
        public string Map
        {
            get => (string)this["map"];
            set => this["map"] = value;
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get => (string)this["path"];
            set => this["path"] = value;
        }

        [ConfigurationProperty("saves", IsRequired = false, DefaultValue = 10)]
        public int Saves
        {
            get => (int)this["saves"];
            set => this["saves"] = value;
        }

        [ConfigurationProperty("delay", IsRequired = false, DefaultValue = 10)]
        public int Delay
        {
            get => (int)this["delay"];
            set => this["delay"] = value;
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }
    }
}