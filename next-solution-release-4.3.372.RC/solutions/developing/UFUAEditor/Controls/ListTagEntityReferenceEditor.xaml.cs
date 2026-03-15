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
using UFUAEditor.ComponentService;
using UFUAEditor.Controls;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;
using ViewModelLib;
using UFUAEditor.Extensions;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for ListTagEntityReferenceEditor.xaml
    /// </summary>
    public partial class ListTagEntityReferenceEditor : UserControl
    {
        #region Declaration
        readonly ObservableCollection<UFUAModel.TagEntityReference> ListTags = new ObservableCollection<UFUAModel.TagEntityReference>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize a new instance of tags list editor.
        /// </summary>
        /// <param name="tags">
        /// Starting tags list (null value for empty list).
        /// </param>
        public ListTagEntityReferenceEditor(UFUAModel.TagEntityReference[] tags)
        {
            InitializeComponent();

            if (tags != null)
            {
                foreach (var tag in tags)
                    ListTags.Add(tag);
            }

            layoutGrid.DataContext = ListTags;
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
                            ListTags.Add(UFUAModel.TagEntityReference.Empty);
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
                            ListTags.Clear(); 
                        },
                        param => ListTags != null && ListTags.Count > 0
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

            UFUAModel.TagEntityReference value = UFUAModel.TagEntityReference.Empty;
            if (button.Tag is UFUAModel.TagEntityReference)
                value = (UFUAModel.TagEntityReference)button.Tag;

            var tag = value.Edit(this.FindParent<Window>())?.First();
            if (tag != null)
            {
                var index = ListTags.IndexOf(value);
                ListTags.Insert(index, tag.TagReference as UFUAModel.TagEntityReference);
                ListTags.Remove(value);
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

            UFUAModel.TagEntityReference value = (UFUAModel.TagEntityReference)(button.Tag);
            if (value != null)
                ListTags.Remove(value);
        }

        void OnCanExecuteMoveUpCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            UFUAModel.TagEntityReference value = (UFUAModel.TagEntityReference)(button.Tag);
            e.CanExecute = value != null && ListTags.IndexOf(value) > 0;
        }

        void OnMoveUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            UFUAModel.TagEntityReference value = (UFUAModel.TagEntityReference)(button.Tag);
            if (value != null)
            {
                var index = ListTags.IndexOf(value);
                if (index > 0)
                    ListTags.Move(index, index - 1);
            }
        }

        void OnCanExecuteMoveDownCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Button button = (Button)e.OriginalSource;

            UFUAModel.TagEntityReference value = (UFUAModel.TagEntityReference)(button.Tag);
            e.CanExecute = value != null && ListTags.IndexOf(value) < ListTags.Count - 1;
        }

        void OnMoveDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            UFUAModel.TagEntityReference value = (UFUAModel.TagEntityReference)(button.Tag);
            if (value != null)
            {
                var index = ListTags.IndexOf(value);
                if (index < ListTags.Count - 1)
                    ListTags.Move(index, index + 1);
            }
        }
        #endregion

        #region Properties
        public UFUAModel.TagEntityReference[] CurrentTags
        {
            get
            {
                return ListTags.ToArray();
            }
        }
        #endregion
    }
}
