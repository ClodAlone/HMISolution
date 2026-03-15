using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ViewModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities.WPF;
using OPCUAViewModelService.ComponentService;
using System.Windows.Threading;
using Utilities;

namespace OPCUAViewModel.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ListOPCUAEntityReferenceEditor.xaml
    /// </summary>
    public partial class ListOPCUAEntityReferenceEditor : UserControl
    {
        #region Declaration
        readonly ObservableCollection<OPCUAEntityReference> ListUri;
        #endregion

        #region Constructors
        public ListOPCUAEntityReferenceEditor(List<OPCUAEntityReference> listUri)
        {
            InitializeComponent();

            ListUri = new ObservableCollection<OPCUAEntityReference>(listUri);
           
            layoutGrid.DataContext = ListUri;
        }
        #endregion

        #region Commands
        public static readonly RoutedCommand EditCommand = new RoutedCommand("EditCommand", typeof(ListOPCUAEntityReferenceEditor));
        public static readonly RoutedCommand RemoveCommand = new RoutedCommand("RemoveCommand", typeof(ListOPCUAEntityReferenceEditor));
        public static readonly RoutedCommand MoveUpCommand = new RoutedCommand("MoveUpCommand", typeof(ListOPCUAEntityReferenceEditor));
        public static readonly RoutedCommand MoveDownCommand = new RoutedCommand("MoveDownCommand", typeof(ListOPCUAEntityReferenceEditor));

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
                            OPCUAEntityReference value = new OPCUAEntityReference(null);
                            if (value.Edit())
                            {
                                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                {
                                    ListUri.Add(value);
                                });
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

            OPCUAEntityReference value = (OPCUAEntityReference)(button.Tag);
            if (value != null)
            {
                if (OPCUAViewModelService.ComponentService.OPCUAViewModelComponent.ufuaEditorServiceAvailable &&
                                                OPCUAViewModelService.ComponentService.OPCUAViewModelComponent.workspaceServiceAvailable)
                {
                    value.Editor = OPCUAViewModelService.ComponentService.OPCUAViewModelComponent.ufuaEditorService;
                    value.Document = OPCUAViewModelService.ComponentService.OPCUAViewModelComponent.workspaceService.ContextDocument;
                }

                if (value.Edit())
                {
                    Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            var index = ListUri.IndexOf(value);
                            if (index >= 0)
                            {
                                ListUri.Remove(value);
                                ListUri.Insert(index, value);
                            }
                        });
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

            OPCUAEntityReference value = (OPCUAEntityReference)(button.Tag);
            if (value != null)
                ListUri.Remove(value);
        }

        void OnCanExecuteMoveUpCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            OPCUAEntityReference value = (OPCUAEntityReference)(button.Tag);

            e.CanExecute = value != null && ListUri.IndexOf(value) > 0;
        }

        void OnMoveUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            OPCUAEntityReference value = (OPCUAEntityReference)(button.Tag);
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

            OPCUAEntityReference value = (OPCUAEntityReference)(button.Tag);

            e.CanExecute = value != null && ListUri.IndexOf(value) < ListUri.Count - 1;
        }

        void OnMoveDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            OPCUAEntityReference value = (OPCUAEntityReference)(button.Tag);
            if (value != null)
            {
                var index = ListUri.IndexOf(value);
                if (index < ListUri.Count - 1)
                    ListUri.Move(index, index + 1);
            }
        }
        
        #endregion

        #region Properties
        public List<OPCUAEntityReference> CurrentUri
        {
            get
            {
                //return new List<OPCUAEntityReference>(ListUri.ToList());
                return ListUri.ToList();
            }
        }
        #endregion

    }
}
