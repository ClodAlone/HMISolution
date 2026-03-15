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
using Utilities;
using Utilities.WPF;

namespace CommonControls.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for AuditTraceNamePropertyEditor.xaml
    /// </summary>
    public partial class AuditTraceNamePropertyEditor : UserControl
    {
        bool bFilled;

        public AuditTraceNamePropertyEditor()
        {
            InitializeComponent();
        }

        private void comboPrototype_DropDownOpened(object sender, EventArgs e)
        {
            FillNames();
        }

        void FillNames()
        {
            if (progressBar.Visibility == Visibility.Visible || bFilled)
                return;

            OPCUAViewModelComponent.QueryInterfaces();
            if (OPCUAViewModelComponent.workspaceServiceAvailable && OPCUAViewModelComponent.ufuaEditorServiceAvailable)
            {
                var doc = OPCUAViewModelComponent.workspaceService.ContextDocument;
                if (doc != null)
                {
                    progressBar.Visibility = Visibility.Visible;
                    var selected = comboPrototype.SelectedValue;
                    var task = Task.Factory.StartNew(() =>
                    {
                        var list = new List<string>() { String.Empty };
                        var list2 = OPCUAViewModelComponent.ufuaEditorService.GetAuditTraceTagNameList(doc);
                        if (list2 != null)
                            list.AddRange(list2);
                        return list.OrderBy(x => x);
                    });
                    task.ContinueWith(ret =>
                    {
                        bFilled = true;
                        progressBar.Visibility = Visibility.Collapsed;
                        comboPrototype.ItemsSource = ret.Result;
                        if (selected != null)
                            comboPrototype.SelectedValue = selected;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }
}
