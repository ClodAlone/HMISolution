using DocumentManager.ComponentService;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Controls;
#endif

namespace OPCUAViewModel
{
    public interface DataSinkInterface
    {
        void Start();
        void Stop();
        string DataSynkName { get; }
        string HumanReadableName { get; }
        string TypeScheme { get; }
        bool IsProjectTypeAware(String projectType);
        void UpdateVariable(String name, DataValue value);
        MonitoredItemViewModel GetVariable(String name, IDocument parent = null);
        List<MonitoredItemViewModel> GetRunningVariables();
        List<String> GetVariables(IDocument parent = null);
#if !NET_STANDARD
        bool CheckVariable(String name);
#endif
        OPCUAEntityReference GetReference(String name);
#if !WINDOWS_UWP && !NET_STANDARD
        UserControl Editor(bool bPopup = true);
#endif
        void SetDocumentParent(IDocument parent);
        void DisposingDocumentParent(IDocument parent);
#if !WINDOWS_UWP && !NET_STANDARD
        bool CheckSource(IDocument parent, object source);
        void Copy(Uri uri, string newPath, bool bCopy, IDocument parent);
        bool Save(IDocument parent, bool encryptFile = false);
        bool NeedsSave(IDocument parent);
        bool RemoveVariable(String name);
#endif
    }
}
