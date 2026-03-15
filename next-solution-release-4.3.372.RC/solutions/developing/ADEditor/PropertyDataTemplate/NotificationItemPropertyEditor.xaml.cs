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
using DevExpress.Xpf.Core.Native;
using System.ComponentModel;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Import.Html;
using UFProjectManager.ComponentService;

namespace ADEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for NotificationItemNamePropertyEditor.xaml
    /// </summary>
    public partial class NotificationItemPropertyEditor : UserControl, INotifyPropertyChanged
    {
        ADModel.ADNotification adNotification = null;
        List<object> adNotifications;
        IUIMsgBoxAlertService uIInterface;
        private Dictionary<string, IDocument> _projectsDocument = new Dictionary<string, IDocument>();
        private Dictionary<string, string> _projectsTitleList = new Dictionary<string, string>();
        private string _defaultAppName;
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
                    InizializeProjectSelection();
                }
                else
                {
                    if (adNotification != null && adNotification.NotificationType == ADModel.NotificationTypes.Local)
                    {
                        OPCUAEntityReference value = xml.FromXml<OPCUAEntityReference>();
                        UpdateLabel(value);
                        InizializeProjectSelection(value.AppName);
                    }
                    else
                    {
                        UpdateLabel();
                        InizializeProjectSelection();
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

        OPCUAEntityReference original;
        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (adNotification != null)
            {
                if (adNotification.NotificationType == ADModel.NotificationTypes.Local)
                {
                    IDocument document = SelectedProjectDocument;

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

                    if (string.IsNullOrEmpty(uriLabel.Text))
                    {
                        uriButton.Tag = original = null;
                        return;
                    }

                    var editor = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    if (editor == null)
                        return;

                    var split = uriLabel.Text.Split(':');
                    var instance = split[0];
                    var tagName = split[0];
                    if (split.Length > 1)
                        tagName = split[1];
                    else
                        instance = null;

                    var xml = editor.GetTagEntityReference(document, tagName, instance);

                    if (string.IsNullOrWhiteSpace(xml))
                    {
                        var datasync = OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                        if (datasync != null)
                        {
                            var relPath = tagName.Replace('\\', '&').Replace('/', '&');
                            datasync.GetVariables();
                            var tagRef = datasync.GetReference(relPath);
                            if (!String.IsNullOrEmpty(tagRef.ReadablePath))
                                xml = tagRef.ToXml();
                        }
                    }
                    if (String.IsNullOrEmpty(xml))
                    {
                        if (original == null)
                        {
                            original = new OPCUAEntityReference(null);
                            original.HumanReadable = original.RelativePath = original.ReadablePath = uriLabel.Text;
                            uriButton.Tag = original;
                        }
                        else
                            original.HumanReadable = original.RelativePath = original.ReadablePath = uriLabel.Text;

                        original.ResolvedNodeId = null;

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
                        uriLabel.Text = item.StringRepresentationWithProject;
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
                return editor.GetFlatFullTagNameCollectionOrderByName(SelectedProjectDocument ?? doc, false, bForceRefresh);
            });
            task.ContinueWith(ret =>
            {
                uriLabel.ItemsSource = ret.Result;
                progressBar.Visibility = Visibility.Collapsed;
                uriLabel.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #region Initialize 

        private void InizializeProjectSelection(string entityAppName = null)
        {
            if (!OPCUAViewModelComponent.workspaceServiceAvailable)
                return;
            IDocument contextDoc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (contextDoc == null)
                return;

            var rootDocument = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(contextDoc, traverse: true);
            if (rootDocument == null)
                return;

            var ufEditorManager = rootDocument.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufEditorManager == null)
                return;

            var uFProjectManager = rootDocument.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            if (uFProjectManager == null)
                return;

            _projectsDocument = uFProjectManager.GetAllProjectsDocuments(rootDocument);
            _projectsTitleList = _projectsDocument.ToDictionary(x => x.Key, y => y.Value.Title);
            _defaultAppName = ufEditorManager.GetAplicationName(contextDoc);
            ProjectCmbBoxVisibility = _projectsTitleList.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

            SetSelectedProject(entityAppName ?? _defaultAppName);
        }

        private void SetSelectedProject(string appName)
        {
            SelectedAppName = appName;
            SelectedProjectDocument = _projectsDocument.FirstOrDefault(x => x.Key == SelectedAppName).Value;
            SelectedProjectName = SelectedProjectDocument?.Title;
            projectCmb.Text = SelectedProjectName;
        }

        #endregion

        #region Project Combo Box

        private IDocument _selectedProjectDocument;
        public IDocument SelectedProjectDocument
        {
            get { return _selectedProjectDocument; }
            set
            {
                if (_selectedProjectDocument == value) return;
                _selectedProjectDocument = value;
                OnPropertyChanged("SelectedProjectDocument");
            }
        }

        private string _selectedProjectName;
        public string SelectedProjectName
        {
            get { return _selectedProjectName; }
            set
            {
                if (_selectedProjectName == value) return;
                _selectedProjectName = value;
                OnPropertyChanged("SelectedProjectName");
            }
        }

        private string _selectedAppName;
        public string SelectedAppName
        {
            get { return _selectedAppName; }
            set
            {
                if (_selectedAppName == value) return;
                _selectedAppName = value;
                OnPropertyChanged("SelectedAppName");
            }
        }

        private Visibility _projectCmbBoxVisibility = Visibility.Collapsed;
        public Visibility ProjectCmbBoxVisibility
        {
            get { return _projectCmbBoxVisibility; }
            set
            {
                if (_projectCmbBoxVisibility == value) return;
                _projectCmbBoxVisibility = value;
                OnPropertyChanged("ProjectCmbBoxVisibility");
                OnPropertyChanged("SeparatorVisibility");
            }
        }

        public Visibility SeparatorVisibility
        {
            get
            {
                if (ProjectCmbBoxVisibility == Visibility.Visible)
                {
                    return Visibility.Hidden;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void projectCmb_GotFocus(object sender, RoutedEventArgs e)
        {
            projectCmb.Text = SelectedProjectName;
            projectCmb.SelectAll();
        }

        private void ProjectCmb_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillProjectComboBox();
        }

        bool bFilledProject;
        void FillProjectComboBox(bool bForceRefresh = false)
        {
            if (bFilledProject && !bForceRefresh)
                return;
            progressBar.Visibility = Visibility.Visible;
            bFilledProject = true;
            var task = Task.Factory.StartNew(() =>
            {
                return _projectsTitleList;
            });
            task.ContinueWith(ret =>
            {
                projectCmb.ItemsSource = ret.Result;
                projectCmb.DisplayMember = "Value";
                projectCmb.EditValue = "Key";
                progressBar.Visibility = Visibility.Collapsed;
                projectCmb.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void projectCmb_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            projectCmb.Text = SelectedProjectName;
            uriLabel.Text = String.Empty;
            bFilled = false;
            var combo = e.Source as ComboBoxEdit;
            if (combo?.EditValue != null)
            {
                var selectedPrj = (KeyValuePair<string, string>)combo.EditValue;
                SelectedProjectName = selectedPrj.Value;
                SelectedAppName = selectedPrj.Key;
                SelectedProjectDocument = _projectsDocument.FirstOrDefault(x => x.Key == SelectedAppName).Value;
                projectCmb.Text = selectedPrj.Value;
            }
        }

        #endregion
    }
}
