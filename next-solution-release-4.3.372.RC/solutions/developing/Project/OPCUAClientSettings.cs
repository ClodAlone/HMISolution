using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Opc.Ua;
using System.IO;

namespace UFProject
{
    [DataContract(Name = "OPCUAClientSettings", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    class OPCUAClientSettings
    {
        #region Declarations
        public static readonly String defExt = "opcuasettings";
        #endregion

        #region Persistance

        [DataMember]
        EndpointDescriptionCollection endpointDescription;

        [DataMember]
        bool UseSecurity;
                         
        #endregion

        #region Methods
        public static OPCUAClientSettings Open(String p)
        {
            String path = Path.ChangeExtension(p, defExt);

            if (String.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                DataContractSerializer formatter = new DataContractSerializer(typeof(OPCUAClientSettings));
                OPCUAClientSettings ret = formatter.ReadObject(fileStream) as OPCUAClientSettings;
                return ret;
            }
        }
        #endregion
    }
}
