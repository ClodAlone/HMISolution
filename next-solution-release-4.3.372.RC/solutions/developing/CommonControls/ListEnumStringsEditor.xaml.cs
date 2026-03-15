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
using Utilities;
using Utilities.WPF;
using ViewModelLib;
using UFInterfaces;
using StringManager.ComponentService;
using DocumentManager.ComponentService;

namespace CommonControls
{
    /// <summary>
    /// Interaction logic for ListEnumStringsEditor.xaml
    /// </summary>
    public partial class ListEnumStringsEditor : UserControl
    {
        #region Declaration
        readonly ObservableCollection<StringWrapperObject> ListEnum = new ObservableCollection<StringWrapperObject>();
        readonly IWorkspace workspace;
        #endregion

        #region Constructors
        public ListEnumStringsEditor(IList<String> listEnum, IWorkspace workspace)
            : this(listEnum)
        {
            this.workspace = workspace;
        }

        public ListEnumStringsEditor(IList<String> listEnum)
        {
            InitializeComponent();

            foreach (var item in listEnum)
                ListEnum.Add(new StringWrapperObject(item));
            layoutGrid.DataContext = ListEnum;
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
                            ListEnum.Add(new StringWrapperObject(Properties.Resources.NewEnumString));
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
                            ListEnum.Clear(); 
                        },
                        param => ListEnum != null && ListEnum.Count > 0
                        );
                }
                return removeAll;
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

            StringWrapperObject value = (StringWrapperObject)(button.Tag);
            if (value != null)
            {
                UserControl newEnumControl = null;
                if (workspace != null)
                {
                    var doc = workspace.ContextDocument as IDocument;
                    if (doc == null)
                        return;

                    IStringEditorManager stringEditorManager = doc.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                    if (stringEditorManager != null)
                    {
                        newEnumControl = stringEditorManager.GetStringEditor(doc);
                        newEnumControl.DataContext = value;
                    }
                }

                if (newEnumControl == null)
                {
                    newEnumControl = new NewEnumStringControl();
                    (newEnumControl as NewEnumStringControl).enumStringName.Text = value.Value;
                }
                
                var newFolderDialog = new GeneralDialogContent(newEnumControl)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.SelectStringEditor,
                    HelpLink = "ListEnumStringsEditor"
                };
                
                if (newFolderDialog.ShowDialog() != true)
                    return;

                string text = null;
                if (newEnumControl is NewEnumStringControl)
                {
                    text = (newEnumControl as NewEnumStringControl).enumStringName.Text;
                }
                else if (newEnumControl.DataContext != null)
                {
                    text = (newEnumControl.DataContext as String);
                }

                if (!String.IsNullOrEmpty(text))
                {
                    var index = ListEnum.IndexOf(value);
                    ListEnum.Insert(index, new StringWrapperObject(text));
                    ListEnum.Remove(value);
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

            StringWrapperObject value = (StringWrapperObject)(button.Tag);
            if (value != null)
            {
                ListEnum.Remove(value);
                ForceIndexRecalculation();
            }
        }

        void ForceIndexRecalculation()
        {
            layoutGrid.DataContext = null;
            layoutGrid.DataContext = ListEnum;
        }

        void OnCanExecuteMoveUpCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            StringWrapperObject value = (StringWrapperObject)(button.Tag);
            e.CanExecute = value != null && ListEnum.IndexOf(value) > 0;
        }

        void OnMoveUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            StringWrapperObject value = (StringWrapperObject)(button.Tag);
            if (value != null)
            {
                var index = ListEnum.IndexOf(value);
                if (index > 0)
                    ListEnum.Move(index, index - 1);
            }
            ForceIndexRecalculation();
        }

        void OnCanExecuteMoveDownCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Button button = (Button)e.OriginalSource;

            StringWrapperObject value = (StringWrapperObject)(button.Tag);
            e.CanExecute = value != null && ListEnum.IndexOf(value) < ListEnum.Count - 1;
        }

        void OnMoveDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            StringWrapperObject value = (StringWrapperObject)(button.Tag);
            if (value != null)
            {
                var index = ListEnum.IndexOf(value);
                if (index < ListEnum.Count - 1)
                    ListEnum.Move(index, index + 1);
            }
            ForceIndexRecalculation();
        }
        #endregion

        #region Properties
        public String[] CurrentEnums
        {
            get
            {
                var ret = new String[ListEnum.Count];
                for (int ii = 0; ii < ret.Length; ii++)
                    ret[ii] = ListEnum[ii].Value;

                return ret;
            }
        }
        #endregion
    }
}
