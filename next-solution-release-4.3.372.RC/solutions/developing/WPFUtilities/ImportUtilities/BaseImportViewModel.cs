using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using UFInterfaces;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Collections;
using System.Linq;

namespace Utilities
{
    public abstract class BaseImportViewModel<T> : ViewModelBase, IDisposable
    {
        public List<string> Delimiters
        {
            get { return new List<string>() { ",", ".", ";", "|" }; }
        }
        #region Properties
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                SetProperty(ref _filePath, value, nameof(FilePath));
                LoadDataCommand.RaiseCanExecuteChanged();
            }
        }

        private string _delimiter;
        public string Delimiter
        {
            get { return _delimiter; }
            set
            {
                SetProperty(ref _delimiter, value, nameof(Delimiter));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(ref _errorMessage, value, nameof(ErrorMessage)); }
        }

        private bool _loadingDataError;
        public bool LoadingDataError
        {
            get { return _loadingDataError; }
            set { SetProperty(ref _loadingDataError, value, nameof(LoadingDataError)); }
        }


        private List<T> _selectedItems;
        public List<T> SelectedItems
        {
            get { return _selectedItems; }
            set { SetProperty(ref _selectedItems, value, nameof(SelectedItems)); }
        }

        private List<T> _itemsSource;
        public List<T> ItemsSource
        {
            get { return _itemsSource; }
            set { SetProperty(ref _itemsSource, value, nameof(ItemsSource)); }
        }

        private bool _isSelectingAll;
        private bool _isUnselectingAll;
        private T _lastSelectedItem;
        private IList _visibleItems;

        #endregion

        #region Constructor
        public BaseImportViewModel()
        {
            ItemsSource = new List<T>();
            SelectedItems = new List<T>();
            Delimiter = ",";

            #region Commands
            LoadDataCommand = new DelegateCommand(() =>
            {
                LoadData();
            }, () => canExecuteLoadDataCommand());

            ChooseFileCommand = new DelegateCommand(() =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "CSV files (*.csv)|*.csv";

                if (dialog.ShowDialog() == true)
                {
                    FilePath = dialog.FileName;
                }
            });

            UnselectAllCommand = new DelegateCommand(() =>
            {
                _isUnselectingAll = true;
                UnselectAll();
                Refresh();
                _isUnselectingAll = false;
            }, () => canExecuteUnselectAllCommand());

            SelectAllCommand = new DelegateCommand(() =>
            {
                _isSelectingAll = true;
                SelectAll();
                Refresh();
                _isSelectingAll = false;
            }, () => canExecuteSelectAllCommand());

            SelectionChangedCommand = new DelegateCommand<object>(a =>
            {
                try
                {
                    if (_isSelectingAll || _isUnselectingAll)
                        return;

                    var selEventArg = a as GridSelectionChangedEventArgs;
                    if (selEventArg != null)
                    {
                        T _currentItem = (T)selEventArg.Source.FocusedRowData.Row;

                        if (_currentItem != null)
                            selectUnselectItem(_currentItem, _lastSelectedItem);
                        Refresh();
                        _lastSelectedItem = _currentItem;
                    }
                }
                catch { }
            });

            DelimitersSelectionChangedCommand = new DelegateCommand<object>(a =>
            {
                var eventArg = a as SelectionChangedEventArgs;
                if (eventArg != null)
                {
                    Delimiter = eventArg.AddedItems[0].ToString();

                    RaisePropertiesChanged(nameof(Delimiter));
                }

            }, a => true);

            FilterChangedCommand = new DelegateCommand<object>(a =>
            {
                var filterChangedArg = (a as GridEventArgs);
                if (filterChangedArg != null)
                {
                    var items = filterChangedArg.Source.VisibleItems;

                    ItemsSource.ForEach((item) => (item as IImportItem).SetIsVisible(false));
                    ItemsSource.ForEach((item) => (item as IImportItem).SetIsSelected(false));

                    foreach (var item in items)
                    {
                        if (item is IImportItem)
                        {
                            setItemVisibility(item as IImportItem, true);
                        }
                    }
                }
                Refresh();
            });
            #endregion
        }

        #endregion

        #region Private Methods       

        private void selectUnselectItem(T selectedItem, T lastSelectedItem)
        {
            var _visibleItems = GetVisibleItems();
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (selectedItem is IImportItem)
                {
                    var isSelected = (selectedItem as IImportItem).GetIsSelected();
                    (selectedItem as IImportItem).SetIsSelected(!isSelected);
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                var selectedItemIndex = _visibleItems.IndexOf(selectedItem);
                var lastSelectedItemIndex = _visibleItems.IndexOf(lastSelectedItem);

                var startIndex = Math.Min(selectedItemIndex, lastSelectedItemIndex);
                var endIndex = Math.Max(selectedItemIndex, lastSelectedItemIndex);

                if (lastSelectedItemIndex < selectedItemIndex)
                {
                    var test = _visibleItems.GetRange(startIndex + 1, endIndex - startIndex);
                    foreach (var i in test)
                    {
                        var item = (ItemsSource.FirstOrDefault(x => (x as IImportItem) == (i as IImportItem)) as IImportItem);
                        var isSelected = item.GetIsSelected();
                        item.SetIsSelected(!isSelected);
                    }
                }
                else
                {
                    var test = _visibleItems.GetRange(startIndex, endIndex - startIndex + 1);
                    foreach (var i in test)
                    {
                        var item = (ItemsSource.FirstOrDefault(x => (x as IImportItem) == (i as IImportItem)) as IImportItem);
                        var isSelected = item.GetIsSelected();
                        item.SetIsSelected(!isSelected);
                    }
                }
            }
            else
            {
                SelectedItems.Clear();
                UnselectAll();
                var isSelected = (selectedItem as IImportItem).GetIsSelected();
                (selectedItem as IImportItem).SetIsSelected(!isSelected);
            }
        }

        private void setItemVisibility(IImportItem selectedItem, bool isVisible)
        {
            selectedItem.SetIsVisible(isVisible);
        }

        private void Refresh()
        {
            SelectedItems = GetSelectedItems();
            GetVisibleItems();
            RaisePropertyChanged(nameof(SelectedItems));
            SelectAllCommand.RaiseCanExecuteChanged();
            UnselectAllCommand.RaiseCanExecuteChanged();
        }

        private bool canExecuteLoadDataCommand()
        {
            return !String.IsNullOrEmpty(FilePath) && !String.IsNullOrEmpty(Delimiter);
        }
        private bool canExecuteUnselectAllCommand()
        {
            return SelectedItems != null &&
                   SelectedItems.Count > 0;
        }
        private bool canExecuteSelectAllCommand()
        {
            return ItemsSource != null &&
                   SelectedItems != null &&
                   !(GetSelectedItems().Count == ItemsSource.Count);
        }
        #endregion

        #region Public Members
        public abstract List<T> GetSelectedItems();
        public abstract List<T> GetVisibleItems();
        public abstract void UnselectAll();
        public abstract void SelectAll();
        public abstract void LoadData();

        public DelegateCommand<object> SelectionChangedCommand { get; set; }
        public DelegateCommand<object> DelimitersSelectionChangedCommand { get; set; }
        public DelegateCommand LoadDataCommand { get; set; }
        public DelegateCommand UnselectAllCommand { get; set; }
        public DelegateCommand SelectAllCommand { get; set; }
        public DelegateCommand ChooseFileCommand { get; set; }
        public DelegateCommand<object> FilterChangedCommand { get; set; }
        #endregion

        public void Dispose()
        {
            ItemsSource.Clear();
            ItemsSource = null;
            SelectedItems.Clear();
            SelectedItems = null;
        }
    }
}
