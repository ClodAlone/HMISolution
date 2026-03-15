using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DocumentManager.ComponentService;
using OPCUAViewModelService.ComponentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using UFProjectManager.ComponentService;
using UFUAEditor.ComponentService;
using Utilities;
using WPFUtilities;

namespace OPCUAViewModel.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for OPCUAEntityReferencePropertyEditorFromXML.xaml
    /// </summary>
    public partial class OPCUAEntityReferencePropertyEditorFromXML : UserControl, INotifyPropertyChanged
    {
        #region Dependency Properties

        #region AllowDataSync
        public static readonly DependencyProperty AllowDataSyncProperty = DependencyProperty.Register("AllowDataSync", typeof(bool), typeof(OPCUAEntityReferencePropertyEditorFromXML), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowDataSyncChanged), new CoerceValueCallback(OnCoerceAllowDataSync)));

        private static object OnCoerceAllowDataSync(DependencyObject o, object value)
        {
            OPCUAEntityReferencePropertyEditorFromXML control = o as OPCUAEntityReferencePropertyEditorFromXML;
            if (control != null)
                return control.OnCoerceAllowDataSync((bool)value);
            else
                return value;
        }

        private static void OnAllowDataSyncChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OPCUAEntityReferencePropertyEditorFromXML control = o as OPCUAEntityReferencePropertyEditorFromXML;
            if (control != null)
                control.OnAllowDataSyncChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowDataSync(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowDataSyncChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowDataSync
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowDataSyncProperty);
            }
            set
            {
                SetValue(AllowDataSyncProperty, value);
            }
        }
        #endregion

        #region AllowRemoteServer
        public static readonly DependencyProperty AllowRemoteServerProperty = DependencyProperty.Register("AllowRemoteServer", typeof(bool), typeof(OPCUAEntityReferencePropertyEditorFromXML), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowRemoteServerChanged), new CoerceValueCallback(OnCoerceAllowRemoteServer)));

        private static object OnCoerceAllowRemoteServer(DependencyObject o, object value)
        {
            OPCUAEntityReferencePropertyEditorFromXML control = o as OPCUAEntityReferencePropertyEditorFromXML;
            if (control != null)
                return control.OnCoerceAllowRemoteServer((bool)value);
            else
                return value;
        }

        private static void OnAllowRemoteServerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OPCUAEntityReferencePropertyEditorFromXML control = o as OPCUAEntityReferencePropertyEditorFromXML;
            if (control != null)
                control.OnAllowRemoteServerChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowRemoteServer(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowRemoteServerChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowRemoteServer
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowRemoteServerProperty);
            }
            set
            {
                SetValue(AllowRemoteServerProperty, value);
            }
        }
        #endregion

        #endregion
        bool bLoaded;
        private Dictionary<string, IDocument> _projectsDocument = new Dictionary<string, IDocument>();
        private Dictionary<string, string> _projectsTitleList = new Dictionary<string, string>();
        private string _defaultAppName;
        bool bEditing;
        String oldText;
        private IDocument _rootDocument;

        public OPCUAEntityReferencePropertyEditorFromXML()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
                    if (!string.IsNullOrEmpty(currentStyle))
                        ThemeHelper.SetTheme(uriLabel, currentStyle);

                    string xml = uriButton.Tag as string;
                    OPCUAEntityReference value = null;
                    if (!string.IsNullOrEmpty(xml))
                        value = xml.FromXml<OPCUAEntityReference>();

                    if (value == null)
                        value = new OPCUAEntityReference(null);

                    InizializeProjectSelection(value);
                }
            };
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {

            string xml = uriButton.Tag as string;
            OPCUAEntityReference value = null;
            if (!string.IsNullOrEmpty(xml))
                value = xml.FromXml<OPCUAEntityReference>();

            if (value == null)
                value = new OPCUAEntityReference(null);

            if (OPCUAViewModelComponent.workspaceService.ContextDocument != null &&
                OPCUAViewModelComponent.workspaceService.ContextDocument is IDocument)
            {
                var doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
                value.Editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                value.Document = OPCUAViewModelComponent.workspaceService.ContextDocument;
            }

            bEditing = false;
            if (value.Edit(sync: true, localserver: !AllowRemoteServer, noDataSinks: !AllowDataSync))
            {
                if (value.HasValidValue)
                    Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        OPCUAEntityReference newValue = new OPCUAEntityReference(value);
                        if (SelectedAppName != newValue.AppName)
                        {
                            if (newValue.EndpointUrl == null)
                            {
                                SetDefaultSelectedProject();
                            }
                            else
                            {
                                SelectedAppName = newValue.AppName;
                                SelectedProjectName = _projectsTitleList.FirstOrDefault(x => x.Key == newValue.AppName).Value;
                                SelectedProjectDocument = _projectsDocument.FirstOrDefault(x => x.Key == newValue.AppName).Value;
                                projectCmb.Text = SelectedProjectName;
                            }
                        }
                        uriButton.Tag = newValue.ToXml();
                        uriLabel.Text = newValue.StringRepresentationWithProject;
                    });
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = null;
            uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();
            uriLabel.Text = String.Empty;
            SetDefaultSelectedProject();
        }

        void Empty_uriLabel()
        {
            if (!String.IsNullOrEmpty(oldText))
            {
                uriLabel.Text = oldText;
                oldText = null;
            }
        }

        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (uriLabel.Text == null || !bEditing || !OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                bEditing = false;
                Empty_uriLabel();
                return;
            }
            bEditing = false;

            var split = uriLabel.Text.Split(':');
            var instance = split[0];
            var tagName = split[0];
            if (split.Length > 1)
                tagName = split[1];
            else
                instance = null;

            var doc = SelectedProjectDocument;
            if (doc == null)
                return;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            var xml = editor.GetTagEntityReference(doc, tagName, instance);

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
                var tag = xml.FromXml<OPCUAEntityReference>();
                uriButton.Tag = xml;
                uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();
                oldText = null;
                uriLabel.Text = tag.StringRepresentationWithProject;
            }
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (uriButton.Tag != null && uriButton.Tag is string)
            {
                if (String.IsNullOrEmpty(oldText) && !string.IsNullOrEmpty((string) uriButton.Tag))
                {
                    try
                    {
                        OPCUAEntityReference tag = (uriButton.Tag as string).FromXml<OPCUAEntityReference>();
                        var reference = tag;

                        if (String.IsNullOrEmpty(oldText) &&
                           (uriLabel.Text.Contains(reference.HumanReadable) ||
                            uriLabel.Text.Contains(reference.HumanReadable.Replace("\\", "/")) ||
                            uriLabel.Text.Contains(reference.HumanReadable.Replace('/', '\\').Replace('&', '\\'))))
                        {
                            oldText = uriLabel.Text;
                            var s = reference.StringRepresentation;
                            if (!String.IsNullOrEmpty(s))
                                uriLabel.Text = s;
                        }
                    }
                    catch (Exception)
                    {
                       }
                }
            }

            uriLabel.SelectAll();
        }


        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            bEditing = true;

            FillComboBox(e.Key == Key.F5);
        }

        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox();
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

        private void uriLabel_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            bEditing = true;
        }

        #region Initialize 

        private void InizializeProjectSelection(OPCUAEntityReference entityRef)
        {
            if (!OPCUAViewModelComponent.workspaceServiceAvailable)
                return;
            IDocument contextDoc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (contextDoc == null)
                return;

            _rootDocument = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(contextDoc, traverse: true);
            if (_rootDocument == null)
                return;

            var ufEditorManager = _rootDocument.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufEditorManager == null)
                return;

            var uFProjectManager = _rootDocument.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            if (uFProjectManager == null)
                return;

            _projectsDocument = uFProjectManager.GetAllProjectsDocuments(_rootDocument);
            _projectsTitleList = _projectsDocument.ToDictionary(x => x.Key, y => y.Value.Title);
            _defaultAppName = ufEditorManager.GetAplicationName(contextDoc);
            ProjectCmbBoxVisibility = _projectsTitleList.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

            SetSelectedProject(entityRef);
        }

        private void SetSelectedProject(OPCUAEntityReference entityRef)
        {
            if (entityRef.EndpointUrl == null)
            {
                SetDefaultSelectedProject();
            }
            else
            {
                SelectedAppName = entityRef.AppName;
                SelectedProjectDocument = _projectsDocument.FirstOrDefault(x => x.Key == SelectedAppName).Value;
                SelectedProjectName = SelectedProjectDocument?.Title;
                projectCmb.Text = SelectedProjectName;
            }
        }

        private void SetDefaultSelectedProject()
        {
            SelectedAppName = _defaultAppName;
            SelectedProjectDocument = _rootDocument;
            SelectedProjectName = _rootDocument?.Title;
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
