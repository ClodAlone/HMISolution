using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Markup;
using System;
using Utilities;
using System.Collections.Generic;
using System.Linq;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;

namespace StatesChartControl
{
    [SvgValueConverter(typeof(ConvertPenItemList))]
    public class TagPenList : ObservableCollection<PenList>
    {
        #region Constructors
        public TagPenList() 
        { }

        public TagPenList(ObservableCollection<PenList> collection)
            : base(collection)
        { }

        public TagPenList(TagPenList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new PenList(item));
        }
        #endregion

        #region Custom Serialization
        public string ToXml()
        {
            using (var sw = new System.IO.StringWriter())
            {
                using (var writer = XmlWriter.Create(sw))
                {
                    XmlSerializer valueSerializer = new XmlSerializer(typeof(String));
                    writer.WriteStartElement("TagPenList");
                    string _value = XamlWriter.Save(this);
                    valueSerializer.Serialize(writer, _value);
                    writer.WriteEndElement();
                }
                return sw.ToString();
            }
        }
        static public TagPenList FromXml(string xml)
        {
            object value;
            using (var sw = new System.IO.StringReader(xml))
            {
                using (var reader = XmlReader.Create(sw))
                {
                    XmlSerializer valueSerializer = new XmlSerializer(typeof(String));
                    try
                    {
                        reader.ReadStartElement("TagPenList");
                        string svalue = (string)valueSerializer.Deserialize(reader);
                        value = XamlReader.Parse(svalue);
                        reader.ReadEndElement();
                    }
                    catch
                    {
                        return null;
                    }
                }
                return value as TagPenList;
            }
        }
        #endregion

        public List<object> ToDictionary(object document, IUFUAEditorManager uFUAEditorManager)
        {
            List<string> list = new List<string>();
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary(list, document, uFUAEditorManager)));
            return res;
        }
    }

    internal class ConvertPenItemList : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            IUFUAEditorManager uFUAEditorManager = (document as IDocument).GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            var entity = value as TagPenList;
            if (entity == null)
                return (new TagPenList()).ToDictionary(document, uFUAEditorManager);

            return entity.ToDictionary(document, uFUAEditorManager);
        }
        public override Type StorageType
        {
            get
            {
                return typeof(List<object>);
            }
        }
    }
}
