using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFInterfaces.Editors
{
    public class SvgModel
    {
        public DocumentManager.ComponentService.IDocument Parent { get; private set; }
        public CancellationToken QuitEvent { get; private set; }
        public IEnumerable<string> ResourceList { get; private set; }
        public string ProjectFolder { get; private set; }
        public bool IsInError { get; set; }
        public SvgModel(DocumentManager.ComponentService.IDocument p,
            CancellationToken q, IEnumerable<string> r, string f)
        {
            Parent = p;
            QuitEvent = q;
            ResourceList = r;
            ProjectFolder = f;
        }
        public string ExportedProjectFilePath { get; set; }
    }
}
