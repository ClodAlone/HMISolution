using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DevExpress.Xpo.DB;
using Opc.Ua;
using System.Runtime.Serialization.Formatters.Binary;
using Utilities;
namespace OPCUAViewModel
{
    [CollectionDataContract(Name = "OPCUADataReferenceList", ItemName = "OPCUADataReference", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class OPCUAEntityReferenceList : List<OPCUAEntityReference>
    {
        #region Constructors
        public OPCUAEntityReferenceList(List<OPCUAEntityReference> collection)
            : base(collection)
        {
        }

        public OPCUAEntityReferenceList()
            : base()
        {
        }
        #endregion
    }
}
