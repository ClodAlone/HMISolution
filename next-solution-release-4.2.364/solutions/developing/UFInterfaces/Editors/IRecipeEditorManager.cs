using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using DocumentManager.ComponentService;
using UFInterfaces.AuditTrace;

namespace UFRecipeEditor.ComponentService
{
    public interface IRecipeEditorManager : IUFInterfaceBase
    {
#if !NET_STANDARD
        object AddRecipe(Uri relative, IDocument parent);
        object AddGroup(string name, IDocument parent);
        void SaveToFile(IDocument parent);
#endif
        String GetRecipeUAServerEntityReference(IDocument parent);
        String GetRecipeUAServerEntityReference(IDocument parent, String relativePath, String nodeID);
        IAuditTrace GetAuditTraceInterface(Uri uri, IDocument parent);
        bool IsReady(Uri uri, int commandType, IDocument parent);
    }
}
