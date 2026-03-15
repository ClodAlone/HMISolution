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
using Opc.Ua;
using OPCUAViewModel;
using Utilities;
using Utilities.WPF;
using System.Windows.Threading;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using OPCUAViewModelService.ComponentService;

namespace ADEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for NotificationItemNamePropertyEditor.xaml
    /// </summary>
    public partial class TextBoxNotificationItemPropertyEditor : UserControl
    {

        ADModel.ADNotification adNotification = null;
        List<object> adNotifications;
        IUIMsgBoxAlertService uIInterface;
        public TextBoxNotificationItemPropertyEditor()
        {

            InitializeComponent();
            
            Loaded += (o, e) =>
            {
 
                uIInterface = ADEditorManagerComponent.adeditorManagerComponent.UIInterface;
                if (adNotification == null && ADEditorManagerComponent.adeditorManagerComponent.Workspace != null)
                {
                    adNotification = ADEditorManagerComponent.adeditorManagerComponent.Workspace.ContextObject as ADModel.ADNotification;
                    adNotifications = ADEditorManagerComponent.adeditorManagerComponent.Workspace.ContextObjects as List<object>;
                }

                UpdateLabel();
            };
        }
        public void setNotification(ADModel.ADNotification notification)
        {
            adNotification = notification;
            adNotifications = null;
            UpdateLabel();
       }
        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            OPCUAEntityReference newValue = null;
            if (adNotification != null)
            {
                if (adNotification.NotificationType == ADModel.NotificationTypes.Local)
                {
                    string xml = uriButton.Tag as string;
                    OPCUAEntityReference value = null;
                    if (!string.IsNullOrEmpty(xml))
                        value = xml.FromXml<OPCUAEntityReference>();

                    if (value == null)
                        value = new OPCUAEntityReference(null);

                    newValue = ADEditorDocument.BrowseLocalItem(adNotification, value);
                }
                else
                {
                    var doc = ADEditorManagerComponent.adeditorManagerComponent.Workspace.ContextDocument as ADEditorDocument;
                    if (doc != null)
                        newValue = ADEditorDocument.BrowseServerAlarm(doc, adNotification);
                }
            }
            else if (adNotifications != null)
            {
                if (adNotifications.Any(ad => (from x in adNotifications where (ad as ADModel.ADNotification).NotificationType == (x as ADModel.ADNotification).NotificationType select x).Count() == adNotifications.Count()))
                {
                    if ((adNotifications[0] as ADModel.ADNotification).NotificationType == ADModel.NotificationTypes.Local)
                    {
                        string xml = uriButton.Tag as string;
                        OPCUAEntityReference value = null;
                        if (!string.IsNullOrEmpty(xml))
                            value = xml.FromXml<OPCUAEntityReference>();

                        if (value == null)
                            value = new OPCUAEntityReference(null);

                        newValue = ADEditorDocument.BrowseLocalItem((adNotifications[0] as ADModel.ADNotification), value);
                    }
                    else
                    {
                        var doc = ADEditorManagerComponent.adeditorManagerComponent.Workspace.ContextDocument as ADEditorDocument;
                        if (doc != null)
                            newValue = ADEditorDocument.BrowseServerAlarm(doc, (adNotifications[0] as ADModel.ADNotification));
                    }
                }
                else
                {
                    if (uIInterface != null)
                        uIInterface.ShowWarning(Properties.Resources.NotificationItemEditNotAllowed);
                }
            }
            if (newValue != null)
            {
                uriButton.Tag = newValue.ToXml();
                UpdateLabel(newValue);
            }
        }
        private void UpdateLabel()
        {
            string xml = null;
            if (adNotification != null)
                xml = adNotification.NotificationItem;
            else if (adNotifications != null && adNotifications.Count > 0 && adNotifications[0] is ADModel.ADNotification)
                xml = (adNotifications[0] as ADModel.ADNotification).NotificationItem;

            if (string.IsNullOrEmpty(xml))
                UpdateLabel(null);
            else
            {
                OPCUAEntityReference value = xml.FromXml<OPCUAEntityReference>();
                UpdateLabel(value);
            }
        }
        private void UpdateLabel(OPCUAEntityReference item)
        {
            try
            {
                if (item == null)
                {
                    txturiLabel.Text = string.Empty;
                }
                    
                else
                {
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    string oldChars = string.Format("{0}:", ns);
                    string readablePath = item.ReadablePath;

                    if(adNotification != null && adNotification.NotificationType == ADModel.NotificationTypes.Server)
                        readablePath = item.EndpointUrl;
                    else if (adNotifications != null && (adNotifications[0] as ADModel.ADNotification).NotificationType == ADModel.NotificationTypes.Server)
                        readablePath = item.EndpointUrl;

                    txturiLabel.Text = string.Format("{0} ({1})", readablePath.Replace(oldChars, ""), item.AppName);
                }
            }
            catch (Exception)
            {
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            if (adNotification != null || adNotifications != null)
            {
                uriButton.Tag = null;
                uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();
                txturiLabel.Text = String.Empty;
            }
        }

    }
}
