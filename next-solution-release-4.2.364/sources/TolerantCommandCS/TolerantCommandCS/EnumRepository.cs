using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace Bornander.UI.Commands.Test
{
    public class EnumRepository<T> : IWarningRepository<T> 
    {
        private IList<T> ignored = new List<T>();
        private readonly string filename;

        public EnumRepository()
        {
            string name = String.Format("{0}.ignored.xml", Assembly.GetExecutingAssembly().FullName.Split(','));
            filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), name);

            Load();
        }

        public EnumRepository(string filename)
        {
            this.filename = filename;

            Load();
        }

        private void Load()
        {
            if (File.Exists(filename))
            {
                XPathDocument document = new XPathDocument(filename);
                XPathNavigator navigator = document.CreateNavigator();

                foreach (XPathNavigator node in navigator.Select("IgnoredWarnings/Warning"))
                {
                    ignored.Add((T)Enum.Parse(typeof(T), node.Value));
                }
            }
        }

        private void Save()
        {
            File.WriteAllText(filename, ToString());
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("<IgnoredWarnings>");
            foreach (T warning in ignored)
            {
                builder.AppendFormat("\t<Warning>{0}</Warning>\n", warning);
            }
            builder.AppendLine("</IgnoredWarnings>");

            return builder.ToString();
        }

        public IEnumerable<T> Ignored
        {
            get { return ignored; }
        }

        public void Ignore(T warning)
        {
            if (!ignored.Contains(warning))
                ignored.Add(warning);

            Save();
        }

        public void Acknowledge(T warning)
        {
            if (ignored.Contains(warning))
                ignored.Remove(warning);

            Save();
        }
    }
}
