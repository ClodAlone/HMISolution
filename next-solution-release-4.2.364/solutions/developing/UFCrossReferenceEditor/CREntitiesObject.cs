using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFCrossReferenceEditor
{
    internal class CREntitiesObject
    {
        public IDocumentManager DocumentManager { get; set; }
        public IDocument Parent { get; set; }
        public CancellationToken Cancellation { get; set; }
        public Dictionary<string, object> ScriptMap { get; set; }
        public Dictionary<string, int> CRItemsPerDocManagerMap { get; set; }
        public List<UFInterfaces.Editors.CrossReferenceResultModel> CRResultList { get; set; }
        public bool ErrorResult { get; set; }
        public Dictionary<string, string> RenamedMap { get; set; }
    }
}
