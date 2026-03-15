using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace PropertyControl.Localization
{
    public class PropertyResourceManager
    {
        #region Declarations
        readonly string baseName;
        readonly string resourceFileName;
        Dictionary<string, string> mapItems;

        internal static bool bDisableUILocalization;
        #endregion

        #region Constructors
        public PropertyResourceManager(string basename)
        {
            baseName = basename;

            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            string filename = String.Format("{0}.{3}\\Cultures\\{1}\\{2}.resources.xml", 
                Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),
                System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                baseName,
                mainversion);

            if (!bDisableUILocalization && !System.IO.File.Exists(filename))
                filename = String.Format("{0}.{3}\\Cultures\\{1}\\{2}.resources.xml",
                Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),
                Properties.Settings.Default.DefaultFolderCultureName,
                baseName,
                mainversion);

            if (System.IO.File.Exists(filename))
                resourceFileName = filename;
        }
        #endregion

        #region Ovverides
        public string GetString(string name)
        {
            if (!string.IsNullOrEmpty(resourceFileName))
            {
                if (mapItems == null)
                {
                    mapItems = LoadFromXml();
                }

                if (mapItems.ContainsKey(name))
                    return mapItems[name];
            }

            return null;
        }
        #endregion

        #region Methods
        internal Dictionary<string, string> LoadFromXml()
        {
            var items = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(resourceFileName))
            {
                try
                {
                    var keyexpandolist = Utilities.XmlHelper.GetExpandoAttributeFromXml(File.ReadAllText(resourceFileName), "resources", true);
                    if (keyexpandolist.Count() != 0)
                    {
                        keyexpandolist.ToList().ForEach(e =>
                        {
                            var regkeydictionary = e as IDictionary<string, object>;
                            regkeydictionary.ToList().ForEach(r =>
                            {
                                try
                                {
                                    items.Add(r.Key.ToString(), r.Value.ToString());
                                }
                                catch
                                {
                                }
                            });
                        });
                    }
                }
                catch
                { }
            }

            return items;
        }

        internal void WriteToXml(Dictionary<string, string> items)
        {
            if (String.IsNullOrEmpty(baseName))
                return;

            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            string filename = String.Format("{0}.{3}\\Cultures\\{1}\\{2}.resources.xml",
                Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),
                System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                baseName,
                mainversion);

            try
            {
                string path = System.IO.Path.GetDirectoryName(filename);
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                var settings = new System.Xml.XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                };

                using (var writer = System.Xml.XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("resources");

                    foreach (var key in items.Keys)
                    {
                        writer.WriteStartElement("ID");
                        writer.WriteAttributeString("text", key);
                        writer.WriteString(items[key]);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch
            { }
        }
        #endregion
    }
}
