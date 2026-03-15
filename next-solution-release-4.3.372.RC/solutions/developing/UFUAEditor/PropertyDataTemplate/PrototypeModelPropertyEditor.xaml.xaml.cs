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
using UFUAEditor.ComponentService;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;

namespace UFUAEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for PrototypeModelPropertyEditor.xaml
    /// </summary>
    public partial class PrototypeModelPropertyEditor : UserControl
    {
        public PrototypeModelPropertyEditor()
        {
            InitializeComponent();
        }

        private void comboPrototype_DropDownOpened(object sender, EventArgs e)
        {
            if (comboPrototype.ItemsSource == null)
            {
                var list = new List<String>();// { String.Empty };

                if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace != null)
                {
                    UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.IsBusy = true;
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.IsBusy = false;
                    });

                    var doc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetContextServerDocument();
                    if (doc != null)
                        list.AddRange(doc.GetPrototypesNames());
                }

                comboPrototype.ItemsSource = list;
            }
        }
    }
}
