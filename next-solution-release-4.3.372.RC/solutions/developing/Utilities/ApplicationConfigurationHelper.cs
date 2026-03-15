using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Utilities
{
    public class ApplicationConfigurationHelper
    {
        public static Opc.Ua.ApplicationConfiguration LoadConfiguration(String filePath)
        {
            Opc.Ua.ApplicationConfiguration applicationConf = null;

            try
            {
                var file = new System.IO.FileInfo(filePath);
                using (var reader = new XmlTextReader(file.Open(FileMode.Open, FileAccess.Read)))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Opc.Ua.ApplicationConfiguration));
                    applicationConf = serializer.ReadObject(reader, false) as Opc.Ua.ApplicationConfiguration;
                }
            }
            catch (Exception e)
            {
               Opc.Ua.Utils.Trace(e, "Could not load configuration file. {0}", filePath);
            }

            return applicationConf;
        }
    }
}
