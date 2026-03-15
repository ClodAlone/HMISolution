using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PropertyControl.Localization
{
    public class PriorityResourceManager
    {
        #region Declarations
        readonly string baseName;
        readonly string rootNode;
        readonly string resourceFileName;
        List<string> listItems;

        internal static bool bDisableUILocalization;
        #endregion

        #region Constructors
        public PriorityResourceManager(string basename, string rootnode)
        {
            baseName = basename;
            rootNode = rootnode;

            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            resourceFileName = String.Format("{0}.{3}\\Cultures\\{1}\\{2}", 
                Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),
                System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                baseName,
                mainversion);

            if (!bDisableUILocalization && !System.IO.File.Exists(resourceFileName))
                resourceFileName = String.Format("{0}.{3}\\Cultures\\{1}\\{2}",
                Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),
                Properties.Settings.Default.DefaultFolderCultureName,
                baseName,
                mainversion);
        }
        #endregion

        #region Ovverides
        public int GetPriority(string name)
        {
            if (!string.IsNullOrEmpty(resourceFileName))
            {
                if (listItems == null)
                    listItems = LoadFromXml();
                return listItems.IndexOf(name.ToUpper());
            }

            return -1;
        }

        public void SetPriority(string name, int newValue)
        {
            if (listItems == null)
                listItems = LoadFromXml();
            bool bSave = listItems.Remove(name.ToUpper());
            if (newValue >= 0)
            {
                bSave = true;
                if (newValue < listItems.Count)
                    listItems.Insert(newValue, name.ToUpper());
                else
                    listItems.Add(name.ToUpper());
            }

            if (bSave)
                WriteToXml();
        }
        #endregion

        #region Methods
        internal List<string> LoadFromXml()
        {
            var items = new List<string>();
            if (File.Exists(resourceFileName))
            {
                try
                {
                    XDocument doc = new XDocument();
                    doc = XDocument.Parse(File.ReadAllText(resourceFileName));

                    var list = doc.Descendants(rootNode).ToList();
                    if (list.Count > 0)
                    {
                        var element = list[0];

                        foreach (var child in element.Descendants())
                        {
                            if (child.Name.Namespace == "" && !items.Contains(child.Value.ToUpper()))
                                items.Add(child.Value.ToUpper()); ;
                        }
                    }
                }
                catch
                { }
            }

            return items;
        }

        void WriteToXml()
        {
            if (listItems != null)
            {
                try
                {
                    string path = System.IO.Path.GetDirectoryName(resourceFileName);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    if (File.Exists(resourceFileName))
                    {
                        File.Delete(resourceFileName);
                    }

                    var settings = new System.Xml.XmlWriterSettings()
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (var writer = System.Xml.XmlWriter.Create(resourceFileName, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement(rootNode);

                        var textInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.InvariantCulture.Name, false).TextInfo;
                        foreach (var item in listItems)
                        {
                            writer.WriteStartElement("ID");
                            writer.WriteString(textInfo.ToTitleCase(item.ToLower()));
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }
                catch
                { }
            }
        }
        #endregion
    }
}
