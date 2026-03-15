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
using WPFUtilities.Converters;

namespace UFProjectManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ListUriLogicEditor.xaml
    /// </summary>
    public partial class ListScreenEditor : UserControl
    {
        #region Declaration
        readonly String Filter;
        readonly ObservableCollection<AutoloadScreen> ListUri;
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(ListScreenEditor), new UIPropertyMetadata(null));
        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }

        #endregion
        #endregion

        #region Constructors
        public ListScreenEditor(IList<AutoloadScreen> listUri, String filter)
        {
            InitializeComponent();
            UriConverter uriConverter = TryFindResource("UriConverter") as UriConverter;
            if (uriConverter != null)
            {
                uriConverter.document = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument;
                uriConverter.getRenamed = true;
            }
            Document = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument;
            ListUri = new ObservableCollection<AutoloadScreen>(listUri);
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
                            if (Document == null)
                                return;

                            var control = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControl(Document, Filter);
                            var Dialog = new GeneralDialogContent(control)
                            {
                                Owner = this.FindParent<Window>(),
                                HelpLink = "SelectScreen"
                            };
                            Dialog.DialogKeepContent = true;
                            if (Dialog.ShowDialog() == true)
                            {
                                var uri = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControlUri(control);
                                var _list = (from a in ListUri select a.Uri.OriginalString).ToList();
                                var urirelative = Document.MakeRelativeUri(uri);
                                if (urirelative != null && !_list.Contains(urirelative.OriginalString))
                                    ListUri.Add(new AutoloadScreen() { Uri = urirelative });
                            }
                        },
                        param => Document != null);
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
        #endregion

        #region Methods
        void OnCanExecuteEditCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null;
        }

        void OnEditCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (AutoloadScreen)(button.Tag);
            if (value != null && Document != null)
            {
                var control = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControl(Document, Filter);
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "SelectLogic"
                };
                if (Dialog.ShowDialog() == true)
                {
                    var uri = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControlUri(control);
                    var urirelative = Document.MakeRelativeUri(uri);
                    var index = ListUri.IndexOf(value);
                    ListUri.Insert(index, new AutoloadScreen() { Uri = urirelative });
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

            var value = (AutoloadScreen)(button.Tag);
            if (value != null)
                ListUri.Remove(value);
        }

        void OnCanExecuteMoveUpCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (AutoloadScreen)(button.Tag);

            e.CanExecute = value != null && ListUri.IndexOf(value) > 0;
        }

        void OnMoveUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (AutoloadScreen)(button.Tag);
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

            var value = (AutoloadScreen)(button.Tag);

            e.CanExecute = value != null && ListUri.IndexOf(value) < ListUri.Count - 1;
        }

        void OnMoveDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            var value = (AutoloadScreen)(button.Tag);
            if (value != null)
            {
                var index = ListUri.IndexOf(value);
                if (index < ListUri.Count - 1)
                    ListUri.Move(index, index + 1);
            }
        }
        #endregion

        #region Properties
        public AutoloadScreen[] CurrentUri
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
