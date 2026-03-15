#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid.Cells;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Grid.Utility;
using System;
using System.ComponentModel;
using Syncfusion.Data.Extensions;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#else
using System.Windows.Input;
using System.Windows;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using KeyEventArgs = KeyRoutedEventArgs;
    using MouseButtonEventArgs = PointerRoutedEventArgs;
    using DoubleTappedEventArgs = Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs;
    using TappedEventArgs = Windows.UI.Xaml.Input.TappedRoutedEventArgs;
    using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
    using Windows.UI.Xaml.Data;
#elif WPF
    using DoubleTappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
    using TappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
    using Syncfusion.Data;
#elif SILVERLIGHT
    using DoubleTappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
    using TappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#elif WP
    using DoubleTappedEventArgs = System.Windows.Input.GestureEventArgs;
    using TappedEventArgs = System.Windows.Input.GestureEventArgs;
#endif
    [ClassReference(IsReviewed = false)]
    public abstract class DataColumnBase : IColumnElement, IDisposable 
    {
        #region Fields

        IGridCellRenderer renderer;
        int rowIndex = -1;
        int columnIndex = -1;
        Visibility columnVisibility = Visibility.Visible;
        bool isEditing;
        GridColumn gridColumn = null;
        IndentColumnType indentColumnType;
        bool isSelectedColumn = false;

        #endregion

        #region Internal Property

        internal FrameworkElement ColumnElement;
        internal IGridSelectionController SelectionController;
        internal bool IsIndentColumn = false;
        internal int RowSpan = 0;
        internal int ColumnSpan = 0;
        internal int Level = 1;
        internal bool IsEnsured;
        internal bool isSuspendUpdateStyle;
#if !WP
        internal bool IsExpanderColumn;
#endif

        internal Visibility ColumnVisibility
        {
            get
            {
                return columnVisibility;
            }
            set
            {
                columnVisibility = value;
                OnColumnVisibilityChanged();
            }
        }

        #endregion

        #region Public property

        /// <summary>
        /// Gets or sets a value indicating whether the current cell selection is visible or not..
        /// </summary>
        /// <value><see langword="true"/> if this instance ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool IsSelectedColumn
        {
            get
            {
                return isSelectedColumn;
            }
            set
            {
                isSelectedColumn = value;
                OnPropertyChanged("IsSelectedColumn");
            }
        }

        public bool IsEditing
        {
            get
            {
                return isEditing;
            }
            internal set
            {
                isEditing = value;
            }
        }

        public int ColumnIndex
        {
            get
            {
                return columnIndex;
            }
            internal set
            {
                columnIndex = value;
            }
        }

        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
            internal set
            {
                rowIndex = value;
                OnRowIndexChanged();
            }
        }

        public GridColumn GridColumn
        {
            get
            {
                return gridColumn;
            }
            internal set
            {
                gridColumn = value;
            }
        }

        public IndentColumnType IndentColumnType
        {
            get
            {
                return indentColumnType;
            }
            set
            {
                indentColumnType = value;
                OnPropertyChanged("IndentColumnType");
            }
        }

        #endregion

        #region Property Changed

        private void OnColumnVisibilityChanged()
        {
            this.ColumnElement.Visibility = this.columnVisibility;
        }

        private void OnRowIndexChanged()
        {
            if (this.RowIndex > 0 && !isSuspendUpdateStyle)
                this.UpdateCellStyle();
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Prepares the Column element based on the Visible column 
        /// </summary>
        /// <remarks></remarks>
        internal void InitializeColumnElement(object dataContext, bool isInEdit)
        {
            this.ColumnElement = OnInitializeColumnElement(dataContext, isInEdit);
        }

        /// <summary>
        /// Raise the PointerPressed method in the selection controller
        /// </summary>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.Input.PointerRoutedEventArgs">PointerRoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void RaisePointerPressed(MouseButtonEventArgs args)
        {
            this.SelectionController.HandlePointerOperations(new GridPointerEventArgs(PointerOperation.Pressed,args), new RowColumnIndex(this.RowIndex, this.ColumnIndex));
        }

        /// <summary>
        /// Raise the pointer released method in selection controller.
        /// </summary>
        /// <param name="args">An <see cref="T:Windows.UI.Xaml.Input.PointerRoutedEventArgs">PointerRoutedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        internal void RaisePointerReleased(MouseButtonEventArgs args)
        {
            this.SelectionController.HandlePointerOperations(new GridPointerEventArgs(PointerOperation.Released, args), new RowColumnIndex(this.RowIndex, this.ColumnIndex));
        }

#if !WinRT && !WP
        internal void RaisePointerWheel()
        {
            var columnindex = this.IsExpanderColumn ? this.SelectionController.CurrentCellManager.CurrentCellIndex.ColumnIndex : this.ColumnIndex;
            this.SelectionController.HandlePointerOperations(new GridPointerEventArgs(PointerOperation.Wheel, null), new RowColumnIndex(this.RowIndex, columnindex));
        }
#endif

        /// <summary>
        /// Raise OnTapped Method in Selection Controller
        /// </summary>
        /// <param name="e">TappedRoutedEventArgs</param>
        internal void OnTapped(TappedEventArgs e)
        {
#if !WP
            var columnindex = this.IsExpanderColumn ? this.SelectionController.CurrentCellManager.CurrentCellIndex.ColumnIndex : this.ColumnIndex;
#else
            var columnindex = this.ColumnIndex;
#endif
            this.SelectionController.HandlePointerOperations(new GridPointerEventArgs(PointerOperation.Tapped, e), new RowColumnIndex(this.RowIndex, this.ColumnIndex));
        }

        /// <summary>
        /// Raise OnDoubleTapped Method in Selection Controller
        /// </summary>
        /// <param name="e">DoubleTappedRoutedEventArgs</param>
        internal void OnDoubleTapped(DoubleTappedEventArgs e)
        {
#if !WP
            var columnindex = this.IsExpanderColumn ? this.SelectionController.CurrentCellManager.CurrentCellIndex.ColumnIndex : this.ColumnIndex;
#else
            var columnindex = this.ColumnIndex;
#endif
            this.SelectionController.HandlePointerOperations(new GridPointerEventArgs(PointerOperation.DoubleTapped, e), new RowColumnIndex(this.RowIndex, this.ColumnIndex));
        }

        protected void SetBindings(UIElement columnElement)
        {
            if (columnElement is GridCell)
            {
                var cell = columnElement as GridCell;
                var bind = new Binding
                {
                    Path = new PropertyPath("IsSelectedColumn"),
                    Source = this,
                    Mode = BindingMode.TwoWay,
                    Converter = new BoolToVisiblityConverter()
                };
                cell.SetBinding(GridCell.CurrentCellBorderVisibilityProperty, bind);
                cell.ColumnBase = this;
            }

            if (columnElement is GridIndentCell)
            {
                var element = columnElement as GridIndentCell;
                var bind = new Binding
                {
                    Path = new PropertyPath("IndentColumnType"),
                    Source = this,
                    Mode = BindingMode.TwoWay,
                };
                element.SetBinding(GridIndentCell.ColumnTypeProperty, bind);
            }


#if WPF

            if (this.Renderer != null && this.Renderer.DataGrid != null)
            {
                var dataGrid = this.Renderer.DataGrid;
                if (columnElement is GridCaptionSummaryCell)
                {
                    var element = columnElement as GridCaptionSummaryCell;
                    var bind = new Binding
                    {
                        Path = new PropertyPath("GroupCaptionContextMenu"),
                        Source = dataGrid,
                        Mode = BindingMode.TwoWay,
                    };
                    element.SetBinding(GridCaptionSummaryCell.ContextMenuProperty, bind);

                }
                else if (columnElement is GridGroupSummaryCell)
                {
                    var element = columnElement as GridGroupSummaryCell;
                    var bind = new Binding
                    {
                        Path = new PropertyPath("GroupSummaryContextMenu"),
                        Source = dataGrid,
                        Mode = BindingMode.TwoWay,
                    };
                    element.SetBinding(GridGroupSummaryCell.ContextMenuProperty, bind);

                }
                else if (columnElement is GridTableSummaryCell)
                {
                    var element = columnElement as GridTableSummaryCell;
                    var bind = new Binding
                    {
                        Path = new PropertyPath("TableSummaryContextMenu"),
                        Source = dataGrid,
                        Mode = BindingMode.TwoWay,
                    };
                    element.SetBinding(GridTableSummaryCell.ContextMenuProperty, bind);
                }
                else if (columnElement is GridHeaderCellControl)
                {
                    var element = columnElement as GridHeaderCellControl;
                    var bind = new Binding
                    {
                        Path = new PropertyPath("HeaderContextMenu"),
                        Source = dataGrid,
                        Mode = BindingMode.TwoWay,
                    };
                    element.SetBinding(GridHeaderCellControl.ContextMenuProperty, bind);
                }
                else if (columnElement is GridRowHeaderCell)
                {
                    var dataContext = (columnElement as GridRowHeaderCell).DataContext;
                    var element = columnElement as GridCell;
                    if (dataContext == null)
                        return;
                    if (dataContext is Group)
                    {
                        var bind = new Binding
                        {
                            Path = new PropertyPath("GroupCaptionContextMenu"),
                            Source = dataGrid,
                            Mode = BindingMode.TwoWay,
                        };
                        element.SetBinding(GridRowHeaderCell.ContextMenuProperty, bind);
                    }
                    else if (dataContext is SummaryRecordEntry)
                    {
                        var bind = new Binding
                        {
                            Path = new PropertyPath("GroupSummaryContextMenu"),
                            Source = dataGrid,
                            Mode = BindingMode.TwoWay,
                        };
                        element.SetBinding(GridRowHeaderCell.ContextMenuProperty, bind);
                    }
                    else
                    {
                        var bind = new Binding
                        {
                            Path = new PropertyPath("RecordContextMenu"),
                            Source = dataGrid,
                            Mode = BindingMode.TwoWay,
                        };
                        element.SetBinding(GridRowHeaderCell.ContextMenuProperty, bind);
                    }
                }
                else if (columnElement is GridCell)
                {
                    var element = columnElement as GridCell;
                    var bind = new Binding
                    {
                        Path = new PropertyPath("RecordContextMenu"),
                        Source = dataGrid,
                        Mode = BindingMode.TwoWay,
                    };
                    element.SetBinding(GridCell.ContextMenuProperty, bind);
                }
            }
#endif
        }

        #endregion

        #region abstract methods

        /// <summary>
        /// Prepares the Column element based on the Visible column 
        /// </summary>
        /// <remarks></remarks>
        protected abstract FrameworkElement OnInitializeColumnElement(object dataContext, bool IsInEdit);

        /// <summary>
        /// When we scroll the Grid vertically row's will be recycled. 
        /// While recycling we need to update the style info of all the cell's in old row.
        /// This property change call back will update the style info of all the cell element when the row index changed.
        /// </summary>
        /// <remarks></remarks>
        public abstract void UpdateCellStyle();

        /// <summary>
        /// Method which is update the binding and style information of the 
        /// cell when we recycle the cell for scrolling.
        /// </summary>
        /// <remarks></remarks>
        public abstract void UpdateBinding(object dataContext, bool updateCellStyle = true);

        #endregion

        #region IElement

        FrameworkElement IElement.Element
        {
            get { return this.ColumnElement; }
        }

        int IElement.Index
        {
            get { return this.ColumnIndex; }
        }

        
        public IGridCellRenderer Renderer
        {
            get { return renderer; }
            internal set { renderer = value; }
        }

        int IColumnElement.RowSpan
        {
            get
            {
                return this.RowSpan;
            }
        }

        int IColumnElement.ColumnSpan
        {
            get
            {
                return this.ColumnSpan;
            }
        }

        int IColumnElement.RowIndex
        {
            get
            {
                return this.RowIndex;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            IElement thisdc = this;
            var dc = obj as IElement;
            if (dc != null)
            {
                if (thisdc.Index > dc.Index)
                    return 1;
                else if (thisdc.Index < dc.Index)
                    return -1;
                else
                    return 0;
            }
            return 0;
        }
        #endregion


        #endregion

        public virtual void Dispose()
        {
            if (this.renderer != null)
            {
                this.renderer.UnloadUIElements(new RowColumnIndex(this.RowIndex, this.ColumnIndex), this.ColumnElement);
                this.renderer = null;
            }

            if (this.ColumnElement is IDisposable)
                (this.ColumnElement as IDisposable).Dispose();
            
            this.ColumnElement = null;
            this.SelectionController = null;
            this.gridColumn = null;
        }
    }
}
