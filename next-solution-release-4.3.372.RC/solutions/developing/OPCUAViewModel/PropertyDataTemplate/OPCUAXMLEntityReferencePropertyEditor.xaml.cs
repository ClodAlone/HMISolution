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
    /// Interaction logic for OPCUAEntityReferencePropertyEditor.xaml
    /// </summary>
    public partial class OPCUAXMLEntityReferencePropertyEditor : UserControl, INotifyPropertyChanged
    {
        bool bLoaded;
        private Dictionary<string, IDocument> _projectsDocument = new Dictionary<string, IDocument>();
        private Dictionary<string, string> _projectsTitleList = new Dictionary<string, string>();
        private string _defaultAppName;
        bool bEditing;
        String oldText;
        private IDocument _rootDocument;

        public OPCUAXMLEntityReferencePropertyEditor()
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

                    OPCUAXMLEntityReference entityRef = (OPCUAXMLEntityReference)(uriButton.Tag);
                    if (entityRef == null)
                    {
                        entityRef = new OPCUAXMLEntityReference();
                        entityRef.TagReference = new OPCUAEntityReference(null);
                    }

                    InizializeProjectSelection(entityRef.TagReference);
                }
            };
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            OPCUAXMLEntityReference tag = (OPCUAXMLEntityReference)(uriButton.Tag);
            if (tag == null)
                tag = new OPCUAXMLEntityReference();
            if (tag.TagReference == null)
                tag.TagReference = new OPCUAEntityReference(null);

            OPCUAEntityReference value = tag.TagReference;

            if (OPCUAViewModelComponent.workspaceService.ContextDocument != null &&
                OPCUAViewModelComponent.workspaceService.ContextDocument is IDocument)
            {
                var doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
                value.Editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                value.Document = OPCUAViewModelComponent.workspaceService.ContextDocument;
            }

            bEditing = false;
            if (value.Edit(sync: true))
            {
                if (value.HasValidValue)
                    Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference();
                            newValue.TagReference = new OPCUAEntityReference(value);
                            if (SelectedAppName != newValue.TagReference.AppName)
                            {
                                if(newValue.TagReference.EndpointUrl == null)
                                {
                                    SetDefaultSelectedProject();
                                }
                                else
                                {
                                    SelectedAppName = newValue.TagReference.AppName;
                                    SelectedProjectName = _projectsTitleList.FirstOrDefault(x => x.Key == newValue.TagReference.AppName).Value;
                                    SelectedProjectDocument = _projectsDocument.FirstOrDefault(x => x.Key == newValue.TagReference.AppName).Value;
                                    projectCmb.Text = SelectedProjectName;
                                }
                            }
                            uriButton.Tag = newValue;
                            uriLabel.Text = newValue.TagReference.StringRepresentationWithProject;
                        });
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = new OPCUAXMLEntityReference();
            //uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();
            uriLabel.Text = String.Empty;
            SetDefaultSelectedProject();
        }

        OPCUAXMLEntityReference original;
        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (uriLabel.Text == null || !bEditing || !OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                bEditing = false;
                if (uriButton.Tag as OPCUAXMLEntityReference != null && (uriButton.Tag as OPCUAXMLEntityReference).TagReference != null && (uriButton.Tag as OPCUAXMLEntityReference).TagReference.StringRepresentation == uriLabel.Text)
                {
                    uriLabel.Text = (uriButton.Tag as OPCUAXMLEntityReference).TagReference.StringRepresentationWithProject;
                    oldText = null;
                }
                return;
            }
            bEditing = false;
            var doc = SelectedProjectDocument;
            if (doc == null)
                return;

            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            var split = uriLabel.Text.Split(':');
            var instance = split[0]?.Replace('/', '\\');
            var tagName = split[0];
            if (split.Length > 1)
                tagName = split[1];
            else
                instance = null;

            if (original == null)
                original = uriButton.Tag as OPCUAXMLEntityReference;

            var xml = editor.GetTagEntityReference(doc, tagName, instance);

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
                original = new OPCUAXMLEntityReference();
                OPCUAEntityReference newTag = new OPCUAEntityReference(null);
                newTag.HumanReadable = newTag.RelativePath = newTag.ReadablePath = uriLabel.Text;
                newTag.ResolvedNodeId = null;
                original.TagReference = newTag;
                uriButton.Tag = original;
                /*
                if (!String.IsNullOrEmpty(oldText))
                {
                    uriLabel.Text = oldText;
                    oldText = null;
                }*/

                var oldColor = uriLabel.Foreground;
                uriLabel.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = oldColor;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = oldColor;
                /*
                if (!String.IsNullOrEmpty(oldText))
                {
                    uriLabel.Text = oldText;
                    oldText = null;
                }
                */
            }
            else
            {
                var tag = new OPCUAXMLEntityReference();
                tag.TagReference = xml.FromXml<OPCUAEntityReference>();
                uriButton.Tag = tag;
                uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();

                oldText = null;
                uriLabel.Text = tag.TagReference.StringRepresentationWithProject;

                if (original != null)
                {
                    if(original.TagReference != null)
                        original.TagReference.UpdateValue(tag.TagReference);
                    else
                        original.TagReference = new OPCUAEntityReference(tag.TagReference);
                }
            }
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (uriButton.Tag != null && uriButton.Tag is OPCUAXMLEntityReference && (uriButton.Tag as OPCUAXMLEntityReference).TagReference is OPCUAEntityReference)
            {
                var reference = uriButton.Tag as OPCUAXMLEntityReference;
                if (String.IsNullOrEmpty(oldText) &&
                   (uriLabel.Text.Contains(reference.TagReference.HumanReadable) ||
                    uriLabel.Text.Contains(reference.TagReference.HumanReadable.Replace('\\', '/').Replace('&', '/')) ||
                    uriLabel.Text.Contains(reference.TagReference.HumanReadable.Replace('/', '\\').Replace('&', '\\'))))
                {
                    oldText = uriLabel.Text;
                    var s = reference.TagReference.StringRepresentation;
                    if (!String.IsNullOrEmpty(s))
                        uriLabel.Text = s;
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
            if (entityRef == null || entityRef.EndpointUrl == null)
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
