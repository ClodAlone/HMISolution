using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using System.Windows;
using System.Windows.Media.Imaging;
using DocumentManager.ComponentService;

namespace Toolbox.ComponentService
{
    public class ToolBoxData
    {
        public String Hash { get; set; }
        public BitmapImage image { get; set; }
        public String Title { get; set; }
    }

    public class PromptForControlEventArgs : EventArgs
    {
        public FrameworkElement toolboxControl;
        public String TypeLabel;
    }

    public interface IToolbox : IUFInterfaceBase
    {
        event EventHandler<PromptForControlEventArgs> PromptForControl;

        ToolBoxData ActiveToolCode { get; set; }

        String GetCodeFromHash(String hash);
        String GetCodeFromHash(String hash, IDocumentManager manager, out String fullPath);
        String GetCurrentDropSettings(String hash);
        String GetCurrentSourceSymbolProvider(String hash);
        String GetCurrentSourceSymbolPath(String hash);

        bool IsToolboxDragging(Object obj);

        IEnumerable<String> GetListCategories(String Type, String fileType, bool deepsearch);
        IEnumerable<String> GetListCategories(String Type, String fileType);
        IEnumerable<String> GetQuickListCategories(String Type, String fileType);
        
        IEnumerable<ToolBoxData> GetListTools(String Type, String fileType, String Category);
    }
}
