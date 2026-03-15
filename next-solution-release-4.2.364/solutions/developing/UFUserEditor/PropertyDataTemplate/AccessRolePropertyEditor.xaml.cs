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
using UFUserEditor.ComponentService;

namespace UFUserEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for AccessRolePropertyEditor.xaml
    /// </summary>
    public partial class AccessRolePropertyEditor : UserControl
    {
        bool bLoaded;
        bool bFilled;

        public AccessRolePropertyEditor()
        {
            InitializeComponent();

            Loaded += (s, e) => 
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                FillRoleNames();
            };
        }

        private void comboRole_DropDownOpened(object sender, EventArgs e)
        {
            FillRoleNames();
        }

        void FillRoleNames(bool bForceRefresh = false)
        {
            if (progressBar.Visibility == Visibility.Visible || (bFilled && !bForceRefresh))
                return;

            if (UFUserEditorManagerComponent.userEditorManagerComponent.Workspace != null &&
                UFUserEditorManagerComponent.userEditorManagerComponent.Workspace.ContextDocument != null)
            {
                IDocument parent = UFUserEditorManagerComponent.userEditorManagerComponent.Workspace.ContextDocument;
                if (parent != null)
                {
                    progressBar.Visibility = Visibility.Visible;
                    var selected = comboRole.SelectedValue;
                    var task = Task.Factory.StartNew(() =>
                    {
                        var ret = new List<string>() { string.Empty };
                        var roles = UFUserEditorManagerComponent.userEditorManagerComponent.GetListRoleNames(parent, bForceRefresh, false);
                        if (roles != null)
                            ret.AddRange(roles);
                        return ret;
                    });
                    task.ContinueWith(ret =>
                    {
                        bFilled = true;
                        progressBar.Visibility = Visibility.Collapsed;
                        comboRole.ItemsSource = ret.Result;
                        if (selected != null)
                            comboRole.SelectedValue = selected;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void comboRole_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            bFilled = false;
            FillRoleNames(e.Key == Key.F5);
        }
    }
}
