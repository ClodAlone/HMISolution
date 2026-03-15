using DevExpress.XtraReports.Native;
using System;
using System.Data;
using System.IO;
using System.Xml;

namespace ReportManager.ReportService
{
    public class CustomUntypedDataSetSerializer : IDataSerializer
    {
        public const string Name = "CustomUntypedDataSetSerializer";
        public bool CanSerialize(object data, object extensionProvider)
        {
            return (data is DataSet);
        }
        public string Serialize(object data, object extensionProvider)
        {
            if (data is DataSet)
            {
                DataSet ds = data as DataSet;
                using (var memoryStream = new MemoryStream())
                {
                    ds.WriteXml(memoryStream, XmlWriteMode.WriteSchema);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }

                //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //using (XmlWriter writer = XmlWriter.Create(sb))
                //{
                //    ds.WriteXml(writer, XmlWriteMode.WriteSchema);
                //    return sb.ToString();
                //}
            }
            return string.Empty;
        }
        public bool CanDeserialize(string value, string typeName, object extensionProvider)
        {
            return typeName == typeof(DataSet).FullName;
        }
        public object Deserialize(string value, string typeName, object extensionProvider)
        {
            DataSet ds = new DataSet();
            using (var memoryStream = new MemoryStream(Convert.FromBase64String(value)))
            {
                ds.ReadXml(memoryStream, XmlReadMode.ReadSchema);
                return ds;
            }

            //DataSet ds = new DataSet();
            //using (XmlReader reader = XmlReader.Create(new StringReader(value)))
            //{
            //    ds.ReadXml(reader, XmlReadMode.ReadSchema);
            //    return ds;
            //}
        }
    }
}
