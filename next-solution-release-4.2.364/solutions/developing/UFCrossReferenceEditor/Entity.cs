using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFCrossReferenceEditor
{
    internal class Entity
    {
        public string Name { get; set; }
        public string Appname { get; set; }
        public string EndpointUrl { get; set; }
        public CrossReferenceType Crtype { get; set; }
        public string RelativePath { get; set; }
        public string ReferencedNodeID { get; set; }
        public string RelativePathNoProject { get; set; }
        public string Description { get; set; }
        public string Settings { get; set; }
        public string ContainerDoc { get; set; }
        public string EType { get; set; }
    }
}
