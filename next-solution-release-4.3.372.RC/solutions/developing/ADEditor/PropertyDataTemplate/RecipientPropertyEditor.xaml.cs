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
using ADEditor.ComponentService;
using ADEditor.Controls;
using ADEditor.Document;
using DevExpress.Xpf.Grid;
using Opc.Ua;
using Utilities;
using Utilities.WPF;
using WPFUtilities;

namespace ADEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for RecipientPropertyEditor.xaml
    /// </summary>
    public partial class RecipientPropertyEditor : UserControl
    {
        ADModel.ADNotification adNotification;
        List<object> adNotifications;
        ADEditorDocument doc;
        public RecipientPropertyEditor()
        {
            InitializeComponent();
            try
            {
                if (ADEditorManagerComponent.adeditorManagerComponent.Workspace != null &&
                                ADEditorManagerComponent.adeditorManagerComponent.PropertyControl != null)
                {
                    doc = ADEditorManagerComponent.adeditorManagerComponent.Workspace.ContextDocument as ADEditorDocument;
                    adNotification = ADEditorManagerComponent.adeditorManagerComponent.PropertyControl.SelectObject as ADModel.ADNotification;
                    adNotifications = ADEditorManagerComponent.adeditorManagerComponent.PropertyControl.SelectObjects as List<object>;
                }
            }
            catch (Exception)
            {
            }
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            if (doc != null)
            {
                var listroles = doc.GetRoles();

                if (listroles == null || listroles.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.NoUsers);
                    return;
                }

                var seluser = new SelectUser() { DataContext = listroles };

                seluser.treeListControl.SelectionMode = MultiSelectMode.MultipleRow;
                GeneralDialogContent Dialog = new GeneralDialogContent(seluser)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "UserSelector"
                };
                if (Dialog.ShowDialog() == true)
                {
                    if (adNotification != null)
                    {
                        adNotification.Recipient = string.Empty;
                        adNotification.RecipientID = string.Empty;
                        adNotification.MultiRecipientID = string.Empty;
                    }
                    StringBuilder strRec = new StringBuilder();
                    foreach (TreeListNode tvm in seluser.treeListControl.GetSelectedNodes())
                    {
                        if (tvm != null)
                        {
                            UFUserModel.UFUser us = null;
                            UFUserModel.UFRole el = tvm.Tag as UFUserModel.UFRole;
                            if (el == null)
                                us = tvm.Tag as UFUserModel.UFUser;

                            if (el != null || us != null)
                            {
                                strRec.Append(el != null ? el.Name : string.Format(@"\{0}",
                                    us.Name));
                                strRec.Append(ADModel.ADNotification.delimiter);
                            }
                        }
                    }
                    if (adNotification != null)
                    {
                        adNotification.Recipient = strRec.ToString();

                    }
                    else if (adNotifications != null)
                    {
                        adNotifications.ForEach(n =>
                        {
                            ADModel.ADNotification adnotification = n as ADModel.ADNotification;
                            adnotification.Recipient = strRec.ToString();
                        });
                    }
                    text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                }
            }

        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            if (adNotification != null)
            {
                adNotification.Recipient = String.Empty;
                adNotification.RecipientID = String.Empty;
                adNotification.MultiRecipientID = String.Empty;
            }
            else if (adNotifications != null)
            {
                adNotifications.ForEach(n =>
                {
                    ADModel.ADNotification adnotification = n as ADModel.ADNotification;
                    adnotification.Recipient = String.Empty;
                    adnotification.RecipientID = String.Empty;
                    adnotification.MultiRecipientID = String.Empty;
                });
            }

            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
