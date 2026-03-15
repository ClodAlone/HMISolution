using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFInterfaces.Editors
{
    public class CrossReferenceModel
    {
        public DocumentManager.ComponentService.IDocument Parent { get; private set; }
        public CancellationToken QuitEvent { get; private set; }
        public Dictionary<string, object> ScriptTagMap { get; private set; }
        public Dictionary<string, string> RenamedMap { get; private set; }
        public IEnumerable<string> ResourceList { get; private set; }
        public CRManagement CRManagement { get; private set; }
        public String Settings { get; set; }
        public List<String> ErrorMessages { get; set; }
        object lockObject;
        public CrossReferenceModel(DocumentManager.ComponentService.IDocument p,
            CancellationToken q, Dictionary<string, object> s, IEnumerable<string> r, CRManagement cRManagement, Dictionary<string, string> rn = null)
        {
            if (rn == null)
                rn = new Dictionary<string, string>();
            Parent = p;
            QuitEvent = q;
            ScriptTagMap = s;
            ResourceList = r;
            ErrorMessages = new List<string>();
            RenamedMap = rn;
            lockObject = new object();
            CRManagement = cRManagement;
        }
        public void AddMessages(List<string> list)
        {
            lock(lockObject)
                ErrorMessages.AddRange(list);
        }
    }
    public class CRManagement
    {
        public List<IDocumentManager> ManagerList { get; set; }
        public List<CrossReferenceType> CrossReferenceTypeList { get; set; }
        public CRManagement()
        {
            ManagerList = new List<IDocumentManager>();
            CrossReferenceTypeList = new List<CrossReferenceType>();
        }
    }
    public class CrossReferenceResultModel
    {
        public string TypeDefinition { get; set; }
        public bool HasPrototypeModel { get; set; }
        public string RelativePath { get; set; }
        public string Name { get; set; }
        public string AppName { get; set; }
        public string EndpointUrl { get; set; }
        public string Description { get; set; }
        public string Settings { get; set; }
        public string ContainerDoc { get; set; }
        public string ReferencedNodeId { get; set; }
        public string IconType { get; set; }
        public DocumentManager.ComponentService.CrossReferenceType CReferenceType { get; set; }
    }
}
