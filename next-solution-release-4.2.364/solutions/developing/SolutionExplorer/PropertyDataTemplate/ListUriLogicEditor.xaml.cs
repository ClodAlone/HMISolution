using DocumentManager.ComponentService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;
using UFProjectManager.ComponentService;
using UFProjectManager.Document;
using Utilities;
using Utilities.WPF;
using ViewModelLib;

namespace UFProjectManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ListUriLogicEditor.xaml
    /// </summary>
    public partial class ListUriLogicEditor : UserControl
    {
        #region Declaration
        readonly String Filter;
        readonly ObservableCollection<StartupLogic> ListUri;
        #endregion

        #region Constructors
        public ListUriLogicEditor(IList<StartupLogic> listUri, String filter)
        {
            InitializeComponent();

            ListUri = new ObservableCollection<StartupLogic>(listUri);
            Filter = filter;
            layoutGrid.DataContext = ListUri;
        }
        #endregion

        #region Commands
        public static readonly RoutedCommand EditCommand = new RoutedCommand();
        public static readonly RoutedCommand RemoveCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveUpCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveDownCommand = new RoutedCommand();

        RelayCommand addNew;
        public ICommand AddNew
        {
            get
            {
                if (addNew == null)
                {
                    addNew = new RelayCommand(
                        param =>
                        {
                            var doc = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument;
                            if (doc == null)
                                return;

                            var control = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControl(doc, Filter);
                            var Dialog = new GeneralDialogContent(control)
                            {
                                Owner = this.FindParent<Window>(),
                                HelpLink = "SelectLogic"
                            };
                            Dialog.DialogKeepContent = true;
                            if (Dialog.ShowDialog() == true)
                            {
                                var uri = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControlUri(control);
                                var urirelative = doc.MakeRelativeUri(uri);
                                if (urirelative != null)
                                    ListUri.Add(new StartupLogic() { Uri = urirelative });
                            }
                        });
                }
                return addNew;
            }
        }

        RelayCommand removeAll;
        public ICommand RemoveAll
        {
            get
            {
                if (removeAll == null)
                {
                    removeAll = new RelayCommand(
                        param => 
                        { 
                            ListUri.Clear(); 
                        },
                        param => ListUri != null && ListUri.Count > 0
                        );
                }
                return removeAll;
            }
        }

        RelayCommand serviceManager;
        public ICommand ServiceManager
        {
            get
            {
                if (serviceManager == null)
                {
                    serviceManager = new RelayCommand(
                        param =>
                        {
                            var doc = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument as UFProjectDocument;
                            if (doc == null)
                                return;

                            var dependencies = new List<String>();
                            if (UFProjectManagerComponent.projectManagerComponent.UFUAEditor != null)
                                dependencies.Add(UFProjectManagerComponent.projectManagerComponent.UFUAEditor.GetServiceName(doc));
                            LogicServiceCMS.LogicServiceCSMHelpers.OpenServiceManager(doc.Title, dependencies.ToArray(), doc.ProjectPath);
                        },
                        param => true
                        );
                }
                return serviceManager;
            }
        }
        #endregion

        #region Methods
        void OnCanExecuteEditCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void OnEditCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (StartupLogic)(button.Tag);
            if (value != null)
            {
                var doc = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument;
                if (doc == null)
                    return;

                var control = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControl(doc, Filter);
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "SelectLogic"
                };
                if (Dialog.ShowDialog() == true)
                {
                    var uri = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControlUri(control);
                    var urirelative = doc.MakeRelativeUri(uri);
                    var index = ListUri.IndexOf(value);
                    ListUri.Insert(index, new StartupLogic() { Uri = urirelative });
                    ListUri.Remove(value);
                }
            }
        }

        void OnCanExecuteRemoveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void OnRemoveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (StartupLogic)(button.Tag);
            if (value != null)
                ListUri.Remove(value);
        }

        void OnCanExecuteMoveUpCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (StartupLogic)(button.Tag);

            e.CanExecute = value != null && ListUri.IndexOf(value) > 0;
        }

        void OnMoveUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (StartupLogic)(button.Tag);
            if (value != null)
            {
                var index = ListUri.IndexOf(value);
                if (index > 0)
                    ListUri.Move(index, index - 1);
            }
        }

        void OnCanExecuteMoveDownCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Button button = (Button)e.OriginalSource;

            var value = (StartupLogic)(button.Tag);

            e.CanExecute = value != null && ListUri.IndexOf(value) < ListUri.Count - 1;
        }

        void OnMoveDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (StartupLogic)(button.Tag);
            if (value != null)
            {
                var index = ListUri.IndexOf(value);
                if (index < ListUri.Count - 1)
                    ListUri.Move(index, index + 1);
            }
        }
        #endregion

        #region Properties
        public StartupLogic[] CurrentUri
        {
            get
            {
                return ListUri.ToArray();
            }
        }
        #endregion

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo == null)
                return;
            combo.ItemsSource = Enum.GetValues(typeof(ExecutionMode)).Cast<ExecutionMode>();
        }
    }
}
