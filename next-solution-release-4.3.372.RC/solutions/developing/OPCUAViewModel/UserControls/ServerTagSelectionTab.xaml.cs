using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DocumentManager.ComponentService;
using DocumentManager.ComponentService.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UFInterfaces.Editors;
using UFUAEditor.ComponentService;
using Utilities.WPF;

namespace OPCUAViewModel.UserControls
{
    /// <summary>
    /// Interaction logic for ServerTagSelectionTab.xaml
    /// </summary>
    public partial class ServerTagSelectionTab: UserControl, IDisposable
    {
        #region Constructor
        
        public ServerTagSelectionTab(ServerTagSelectionModel model)
        {
            InitializeComponent();
            DataContext = model;
        }

        #endregion

        #region Events

        private void cBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataContext = this.DataContext as ServerTagSelectionModel;
            var projectSelected = e.Source as ComboBox;

            if (dataContext != null && projectSelected?.SelectedValue != null)
            {
                string appName = (string)projectSelected.SelectedValue;
                UpdateContentTab(dataContext, appName);
            }
        }

        private void cBox_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            var dataContext = this.DataContext as ServerTagSelectionModel;
            var projectSelected = e.Source as ComboBoxEdit;

            if (dataContext != null && projectSelected?.EditValue != null)
            {
                string appName = (string)projectSelected.EditValue;
                UpdateContentTab(dataContext, appName);
            }
        }

        #endregion

        #region Public Method

        public void BringIntoView(IDocument doc, OPCUAEntityReference entityReference)
        {
            if(DataContext != null)
            {
                (DataContext as ServerTagSelectionModel).StartingProject = entityReference.AppName;
                cBox.EditValue = entityReference.AppName;
            }

            projectTagMap.TryGetValue(doc, out UserControl tag);

            if (tag != null)
            {
                ISelectEntityReference control = tag as ISelectEntityReference;

                if(control?.SelectedReference != null)
                {
                    control.SelectedReference = entityReference;
                }

                if (control?.SelectedReferences != null)
                {
                    control.SelectedReferences.Clear();
                    control.SelectedReferences.Add(entityReference);
                }

                control.BringIntoView(entityReference);
                contentTab.Content = null;
                contentTab.Content = control;
            }
        }

        public void ChangeTagViewByAppName(ServerTagSelectionModel model, string appName)
        {
            UpdateContentTab(model, appName);
        }

        #endregion

        #region Private Method

        private Dictionary<IDocument, UserControl> projectTagMap = new Dictionary<IDocument, UserControl>();

        private void UpdateContentTab(ServerTagSelectionModel dataContext, string appName)
        {
            var docSelected = dataContext.DocumentProjectsList.FirstOrDefault(x => x.Key == appName);
            IDocument doc = docSelected.Value ?? dataContext.Entity?.Document;

            if (doc != null)
            {
                var userControlTagList = GetUserControlTag(dataContext, doc);
                if (userControlTagList == null)
                    return;
                ISelectEntityReference control = userControlTagList as ISelectEntityReference;

                if (control != null)
                    control.BringIntoView(dataContext.Entity);

                userControlTagList.ClearValue(FrameworkElement.WidthProperty);
                userControlTagList.ClearValue(FrameworkElement.HeightProperty);
                contentTab.Content = userControlTagList;
            }
        }

        private UserControl GetUserControlTag(ServerTagSelectionModel dataContext, IDocument doc)
        {
            if (!projectTagMap.ContainsKey(doc))
            {
                var userControlTagList = dataContext.EditorManager.GetAddressSpaceControl(doc, bNewControl: true);

                if (userControlTagList == null)
                    return null;
                userControlTagList.DataContext = dataContext.Entity;
                projectTagMap.Add(doc, userControlTagList);
                return userControlTagList;
            }
            else
            {
                return projectTagMap[doc];
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            foreach (var p in projectTagMap)
            {
                p.Value.Dispose();
            }
            projectTagMap?.Clear();
        }

        #endregion


    }

    /// <summary>
    /// Tag Selection Model Class
    /// </summary>
    public class ServerTagSelectionModel: INotifyPropertyChanged
    {
        public IUFUAEditorManager EditorManager { get; set; }
        public Dictionary<string, IDocument> DocumentProjectsList { get; set; }
        public OPCUAEntityReference Entity { get; set; }
        public string AppNameParent { get; set; }

        public ServerTagSelectionModel(
            OPCUAEntityReference entity,
            IUFUAEditorManager editorManager)
        {
            Entity = entity;
            EditorManager = editorManager;

            var documentParent = DocumentHelper.GetRootParent(entity.Document, traverse: true);
            AppNameParent = EditorManager.GetAplicationName(documentParent);
            DocumentProjectsList = GetProjectsDocuments(documentParent, EditorManager, AppNameParent);

            _projectsList = new ObservableCollection<ProjectName>();
            foreach(var doc in DocumentProjectsList)
            {
                _projectsList.Add(new ProjectName(doc.Key, doc.Value.Title));
            }
            StartingProject = string.IsNullOrEmpty(entity.AppName) ? AppNameParent : entity.AppName;
        }

        private Dictionary<string, IDocument> GetProjectsDocuments(
            IDocument document,
            IUFUAEditorManager editor,
            string appName)
        {
            var projectsList = new Dictionary<string, IDocument>();
            projectsList.Add(appName, document);

            var children = document.Childs ?? document.Parent?.Childs ?? new List<IDocument>();
            foreach (var childDoc in children)
            {
                var childEditor = childDoc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (childEditor == null)
                    continue;
                var childAppName = childEditor.GetAplicationName(childDoc);
                var childDictionary = GetProjectsDocuments(childDoc, childEditor, childAppName);
                projectsList.AddRange(childDictionary);
            }

            return projectsList;
        }

        private readonly ObservableCollection<ProjectName> _projectsList;
        public ObservableCollection<ProjectName> ProjectsList
        {
            get { return _projectsList; }
        }

        private string _startingProject;
        public string StartingProject
        {
            get { return _startingProject; }
            set
            {
                if (_startingProject == value) return;
                _startingProject = value;
                OnPropertyChanged("ProjectId");
            }
        }

        private bool _isCmbBoxVisible;
        public bool IsCmbBoxVisible
        {
            get { return _isCmbBoxVisible; }
            set
            {
                if (_isCmbBoxVisible == value) return;
                _isCmbBoxVisible = value;
                OnPropertyChanged("IsCmbBoxVisible");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProjectName
    {
        public ProjectName(string appName, string projectName)
        {
            Id = appName;
            DisplayName = projectName;
        }
    
        public string Id { get; set; }
        public string DisplayName { get; set; }
    }
}
