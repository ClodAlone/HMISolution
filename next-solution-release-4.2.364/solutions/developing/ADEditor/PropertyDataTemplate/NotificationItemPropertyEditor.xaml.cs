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
    public partial class NotificationItemPropertyEditor : UserControl
    {
        ADModel.ADNotification adNotification = null;
        List<object> adNotifications;
        IUIMsgBoxAlertService uIInterface;
        public NotificationItemPropertyEditor()
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

                string xml = null;
                if (adNotification != null)
                    xml = adNotification.NotificationItem;
                else if (adNotifications != null && adNotifications.Count > 0 && adNotifications[0] is ADModel.ADNotification)
                    xml = (adNotifications[0] as ADModel.ADNotification).NotificationItem;
                if (adNotification != null && adNotification.NotificationType == ADModel.NotificationTypes.Local)
                {
                    txturiLabel.Visibility = Visibility.Collapsed;
                    uriLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    uriLabel.Visibility = Visibility.Collapsed;
                    txturiLabel.Visibility = Visibility.Visible;
                }

                if (string.IsNullOrEmpty(xml))
                {
                    if (adNotification != null && adNotification.NotificationType == ADModel.NotificationTypes.Local)
                    {
                        UpdateLabel(null);
                    }
                    else
                    {
                        UpdateLabel();
                    }
                }
                else
                {
                    if (adNotification != null && adNotification.NotificationType == ADModel.NotificationTypes.Local)
                    {
                        OPCUAEntityReference value = xml.FromXml<OPCUAEntityReference>();
                        UpdateLabel(value);
                    }
                    else
                    {
                        UpdateLabel();
                    }
                }
            };
        }
        public void setNotification(ADModel.ADNotification notification)
        {
            adNotification = notification;
            adNotifications = null;
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
                if(adNotification != null && adNotification.NotificationType == ADModel.NotificationTypes.Local)
                {
                    uriButton.Tag = newValue.ToXml();
                    UpdateLabel(newValue);
                }
                else
                {
                    UpdateLabel();
                }
                
            }
        }
        private void UpdateLabel()
        {
            txturiLabel.Text = adNotification != null ? adNotification.AlarmName : String.Empty;
        }
        private void UpdateLabel(OPCUAEntityReference newValue)
        {
            try
            {
                OPCUAEntityReference uri = newValue as OPCUAEntityReference;
                if (uri.ReadablePath == null || uri.AppName == null || String.IsNullOrEmpty(uri.ReadablePath))
                    uriLabel.Text = uri.HumanReadable;
                else
                {
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    string oldChars = string.Format("{0}:", ns);
                    uriLabel.Text = uri.StringRepresentationWithProject;
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
                uriLabel.Text = String.Empty;
                txturiLabel.Text = String.Empty;
            }
        }


        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (adNotification != null)
            {
                if (adNotification.NotificationType == ADModel.NotificationTypes.Local)
                {
                    IDocument document = ADEditorManagerComponent.adeditorManagerComponent.Workspace.ContextDocument;

                    if (document != null)
                        document = document.Parent;

                    if (uriLabel.Text == null || !bEditing || document == null)
                    {
                        bEditing = false;
                        if (uriButton.Tag != null)
                        {
                            OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference() { TagReferenceXml = uriButton.Tag.ToString() };
                            var tagReference = newValue.TagReference as OPCUAEntityReference;
                            if (tagReference != null && tagReference.StringRepresentation == uriLabel.Text)
                            {
                                uriLabel.Text = tagReference.StringRepresentationWithProject;
                                oldText = null;
                            }
                        }
                        return;
                    }

                    bEditing = false;

                    var editor = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    if (editor == null)
                        return;

                    var split = uriLabel.Text.Split(':');
                    var instance = split[0];
                    var name = split[0];
                    if (split.Length > 1)
                        name = split[1];
                    else
                        instance = null;

                    var xml = editor.GetTagEntityReference(document, name, instance);
                    if (String.IsNullOrEmpty(xml))
                    {
                        var oldColor = uriLabel.Foreground;
                        uriLabel.Foreground = Brushes.Red;
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                        uriLabel.Foreground = oldColor;
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                        uriLabel.Foreground = Brushes.Red;
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                        uriLabel.Foreground = oldColor;

                        if (!String.IsNullOrEmpty(oldText))
                        {
                            uriLabel.Text = oldText;
                            oldText = null;
                        }
                    }
                    else
                    {
                        var item = xml.FromXml<OPCUAEntityReference>();
                        ADEditorDocument.UpdateNotificationItem(adNotification, item);
                        uriButton.Tag = xml;
                        uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();

                        oldText = null;
                        uriLabel.Text = item.StringRepresentation;
                    }
                }
            }
        }

        bool bEditing;
        String oldText;
        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (adNotification != null)
            {
                if (adNotification.NotificationType == ADModel.NotificationTypes.Local)
                {
                    bEditing = true;
                }
                else
                {
                    bEditing = false;
                }
            }
            FillComboBox(e.Key == Key.F5);
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (adNotification != null)
            {
                if (adNotification.NotificationType == ADModel.NotificationTypes.Local)
                {
                    uriLabel.IsReadOnly = false;
                    if (uriButton.Tag != null && uriButton.Tag is string)
                    {
                        if (String.IsNullOrEmpty(oldText) && !string.IsNullOrEmpty((string)uriButton.Tag))
                        {
                            OPCUAEntityReference tag = (uriButton.Tag as string).FromXml<OPCUAEntityReference>();
                            var reference = tag;
                            oldText = uriLabel.Text;
                            var s = reference.StringRepresentation;
                            if (!String.IsNullOrEmpty(s))
                                uriLabel.Text = s;
                        }
                    }

                    uriLabel.SelectAll();
                }
                else
                {
                    uriLabel.IsReadOnly = true;
                }
            }
            else
                uriLabel.IsReadOnly = true;
        }

        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox();
        }

        private void uriLabel_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            bEditing = true;
        }

        bool bFilled;
        void FillComboBox(bool bForceRefresh = false)
        {
            if ((bFilled && !bForceRefresh) || !OPCUAViewModelComponent.workspaceServiceAvailable)
                return;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            bFilled = true;
            progressBar.Visibility = Visibility.Visible;
            var task = Task.Factory.StartNew(() =>
            {
                return editor.GetFlatFullTagNameCollectionOrderByName(doc, false, bForceRefresh);
            });
            task.ContinueWith(ret =>
            {
                uriLabel.ItemsSource = ret.Result;
                progressBar.Visibility = Visibility.Collapsed;
                uriLabel.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

    }
}
