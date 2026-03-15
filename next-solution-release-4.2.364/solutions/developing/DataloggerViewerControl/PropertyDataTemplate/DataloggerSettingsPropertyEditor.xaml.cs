using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DocumentManager.ComponentService;
using OPCUAViewModelService.ComponentService;
using ScreenSettings;
using UFInterfaces;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;

namespace DataloggerViewerControl.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for HistorianSettingsPropertyEditor.xaml
    /// </summary>
    public partial class DataloggerSettingsPropertyEditor : UserControl
    {
        IUFUAEditorManager UFUAEditor;
        IDocument Document;
        public DataloggerSettingsPropertyEditor()
        {
            InitializeComponent();
        }

        private void comboPrototype_DropDownOpened(object sender, EventArgs e)
        {
            if (comboPrototype.ItemsSource == null)
            {
                var list = new List<string>() { String.Empty };
                OPCUAViewModelComponent.QueryInterfaces();
                if (OPCUAViewModelComponent.workspaceServiceAvailable && OPCUAViewModelComponent.ufuaEditorServiceAvailable)
                {
                    OPCUAViewModelComponent.workspaceService.IsBusy = true;
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        OPCUAViewModelComponent.workspaceService.IsBusy = false;
                    });

                    var doc = OPCUAViewModelComponent.workspaceService.ContextDocument;
                    if (doc != null)
                    {
                        var list2 = OPCUAViewModelComponent.ufuaEditorService.GetDataLoggerSettingsNameList(doc);
                        if (list2 != null)
                            list.AddRange(list2);
                    }
                }

                comboPrototype.ItemsSource = list.OrderBy(x => x);
            }
        }
    }
}
