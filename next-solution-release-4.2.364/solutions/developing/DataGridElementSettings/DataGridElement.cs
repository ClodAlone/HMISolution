using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridElementSettings
{
    public class DataGridRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("DataGrids", IsDefaultCollection = true)]
        public DataGridElementCollection DataGrids
        {
            get { return (DataGridElementCollection)this["DataGrids"]; }
            set { this["DataGrids"] = value; }
        }
    }

    [ConfigurationCollection(typeof(DataGridElement))]
    public class DataGridElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DataGridElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DataGridElement)element).Name;
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public void Add(DataGridElement element)
        {
            base.BaseAdd(element);
        }
    }

    public class DataGridElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }

        [ConfigurationProperty("query", IsRequired = true)]
        public String Query
        {
            get { return (string)this["query"]; }
            set { this["query"] = value; }
        }

        [ConfigurationProperty("users", IsRequired = false)]
        public String Users
        {
            get { return (string)this["users"]; }
            set { this["users"] = value; }
        }

        [ConfigurationProperty("roles", IsRequired = false)]
        public String Roles
        {
            get { return (string)this["roles"]; }
            set { this["roles"] = value; }
        }
    }
}
