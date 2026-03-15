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
using CommandManager.ComponentService;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.RangeControl;
using DocumentManager.ComponentService;
using OPCUAViewModelService.ComponentService;
using UFInterfaces;
using UFUAEditor.ComponentService;
using UFUAEditor.Controls;
using UFUAEditor.Document;
using UFUAModel;
using Utilities;
using Utilities.WPF;

namespace UFUAEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for EngineeringUnitPropertyEditor.xaml
    /// </summary>
    public partial class EngineeringUnitPropertyEditor : UserControl
    {
        public EngineeringUnitPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
                        
            var doc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.ContextDocument;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            string itemToSelect = button != null && button.Tag != null ? button.Tag.ToString() : null;

            var euControl = editor.GetEngineeringUnitsControl(doc, true, itemToSelect);

            if (euControl != null)
            {
                GeneralDialogContent gdc = new GeneralDialogContent(euControl)
                {
                    Title = Properties.Resources.SelectEUTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EngineeringUnitPropertyEditor"
                };
                    
                if (gdc.ShowDialog() == true)
                {
                    var selected = (euControl as EngineeringUnitPrototypeList).GetSelectedItem();
                    if (selected != null)
                    {
                        button.Tag = selected.Name;
                        uriLabel.GetBindingExpression(ComboBoxEdit.TextProperty).UpdateTarget();
                    }
                }
            }            
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            button.Tag = null;
            uriLabel.Text = String.Empty;
        }

        private void initializeItemsSource()
        {
            if (uriLabel.ItemsSource == null)
            {
                var list = new List<string>();

                if (UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace != null)
                {
                    UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.IsBusy = true;
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.IsBusy = false;
                    });

                    var doc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetContextServerDocument();
                    if (doc != null)
                        list.AddRange(doc.GetEngineeringUnits().Select(x => x.Name));
                }

                uriLabel.ItemsSource = list;
            }
        }

        private void comboBox_PopupOpening(object sender, OpenPopupEventArgs e)
        {
            using (var v = new WaitCursor())
            {
                initializeItemsSource();
            }
        }
    }
}
