#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WinRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.UI.Xaml.ScrollAxis;
using System.ComponentModel;

#endif
namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using MouseButtonEventArgs = PointerRoutedEventArgs;
#endif
    
    [ClassReference(IsReviewed = false)]
    public class GridCell : ContentControl, IDisposable
    {
        #region Fields

        internal string eventErrorMessage;
        internal string attributeErrorMessage;
        internal string bindingErrorMessage;
#if WPF
        private bool isPreviewMouseDown;
#endif

        #endregion

        #region Properties

        public DataColumnBase ColumnBase { get; internal set; }

        public bool HasError
        {
            get
            {
                return !string.IsNullOrEmpty(eventErrorMessage) || !string.IsNullOrEmpty(attributeErrorMessage) || !string.IsNullOrEmpty(bindingErrorMessage);
            }
        }

        #endregion

        #region Dependency Region

        /// <summary>
        /// Get or sets the CurrentCellBorder visiblity.
        /// Which is bind to  CurrentCell Boreder visiblity property.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Visibility CurrentCellBorderVisibility
        {
            get { return (Visibility)GetValue(CurrentCellBorderVisibilityProperty); }
            set { SetValue(CurrentCellBorderVisibilityProperty, value); }
        }


        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(GridCell), new PropertyMetadata(string.Empty));

        

        /// <summary>
        /// Dependency registration for CurrentCellBorderVisiblity.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty CurrentCellBorderVisibilityProperty =
            DependencyProperty.Register("CurrentCellBorderVisibility", typeof(Visibility), typeof(GridCell), new PropertyMetadata(Visibility.Collapsed));

        public bool IsLastCell
        {
            get { return (bool)GetValue(IsLastCellProperty); }
            set { SetValue(IsLastCellProperty, value); }
        }

        public static readonly DependencyProperty IsLastCellProperty =
            DependencyProperty.Register("IsLastCell", typeof(bool), typeof(GridCell), new PropertyMetadata(false, OnIsLastCellChanged));

        /// <summary>
        /// Gets or sets a thickness for CurrentCell border
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Thickness CurrentCellBorderThickness
        {
            get { return (Thickness)GetValue(CurrentCellBorderThicknessProperty); }
            set { SetValue(CurrentCellBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Dependency registration for CurrentCellBorderThickness.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty CurrentCellBorderThicknessProperty =
            DependencyProperty.Register("CurrentCellBorderThickness", typeof(Thickness), typeof(GridCell), new PropertyMetadata(new Thickness(2)));

        /// <summary>
        /// Gets or sets Brush for CurrnetCell border.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Brush CurrentCellBorderBrush
        {
            get { return (Brush)GetValue(CurrentCellBorderBrushProperty); }
            set { SetValue(CurrentCellBorderBrushProperty, value); }
        }

        /// <summary>
        /// Dependency registration for CurrnetCell border brush
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty CurrentCellBorderBrushProperty =
            DependencyProperty.Register("CurrentCellBorderBrush", typeof(Brush), typeof(GridCell), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region Dependency Call Back

        private static void OnIsLastCellChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var cell = obj as GridCell;
            if (cell != null) cell.ApplyVisualState((bool)args.NewValue);
        }

        #endregion

        #region Ctor

        public GridCell()
        {
            this.DefaultStyleKey = typeof(GridCell);
#if WPF
            this.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnErrorHandled), true);
#elif SILVERLIGHT || WP
            this.BindingValidationError += GridCell_BindingValidationError;
#endif
        }

#if SILVERLIGHT || WP
        void GridCell_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                this.bindingErrorMessage = e.Error.ErrorContent.ToString();
            else
                this.bindingErrorMessage = string.Empty;
                
            this.ApplyValidationVisualState();
        }
#elif WPF
        private void OnErrorHandled(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                this.bindingErrorMessage = e.Error.ErrorContent.ToString();
            else if (this.Content is DependencyObject && !GetHasError(this.Content as DependencyObject))
                this.bindingErrorMessage = string.Empty;
            if (e.Error.RuleInError != null && e.Error.RuleInError.ValidationStep == ValidationStep.ConvertedProposedValue)
                this.bindingErrorMessage = string.Empty;
            this.ApplyValidationVisualState();
        }

        private bool GetHasError(DependencyObject obj)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var element = VisualTreeHelper.GetChild(obj, i) as DependencyObject;
                if (element != null)
                {
                    var haserror = Validation.GetHasError(element as DependencyObject);
                    if (haserror)
                        return true;
                    else
                        return GetHasError(element);
                }
            }
            return false;
        }
#endif
        #endregion

        #region Overrides
        
#if !WinRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.ApplyValidationVisualState();

#if WPF            
            this.ContextMenuOpening += OnContextMenuOpening;   
#endif
        }
        internal void ApplyValidationVisualState()
        {
            if (this.HasError)
            {
                ErrorMessage = string.Empty;
                if (!string.IsNullOrEmpty(eventErrorMessage))
                    ErrorMessage = eventErrorMessage;
                if (!string.IsNullOrEmpty(attributeErrorMessage))
                    ErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage + string.Format("\n") + attributeErrorMessage : attributeErrorMessage;
                if (!string.IsNullOrEmpty(bindingErrorMessage))
                    ErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage + string.Format("\n") + bindingErrorMessage : bindingErrorMessage;

                VisualStateManager.GoToState(this, "HasError", true);
            }
            else
            {
                ErrorMessage = string.Empty;
                VisualStateManager.GoToState(this, "NoError", true);
            }
        }
        
#if WinRT
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic);
            if (ColumnBase != null)
                this.ColumnBase.RaisePointerPressed(e);
            e.Handled = true;
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (ColumnBase != null)
                this.ColumnBase.RaisePointerReleased(e);
            base.OnPointerReleased(e);
        }
        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            if (ColumnBase != null)
                this.ColumnBase.OnDoubleTapped(e);
            base.OnDoubleTapped(e);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (ColumnBase != null)
                this.ColumnBase.OnTapped(e);
            base.OnTapped(e);
        }

#elif WPF


        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.ContextMenu == null)
                return;

            var dataGrid = ColumnBase.Renderer.DataGrid;
            var dataContext = this.ContextMenu.DataContext;
            if (dataContext != null && (dataContext is GridRecordContextMenuInfo))
                (dataContext as GridRecordContextMenuInfo).Record = this.DataContext;
            else
                dataContext = new GridRecordContextMenuInfo() { Record = this.DataContext, DataGrid = dataGrid };

            this.ContextMenu.DataContext = dataContext;
            ContextMenuType menuType = ContextMenuType.RecordCell;
            if (sender is GridCaptionSummaryCell)
                menuType = ContextMenuType.GroupCaption;
            else if (sender is GridGroupSummaryCell)
                menuType = ContextMenuType.GroupSummary;
            else if (sender is GridTableSummaryCell)
                menuType = ContextMenuType.TableSummary;
            else if (!(sender is GridIndentCell) && !(sender is GridRowHeaderIndentCell))
                menuType = ContextMenuType.RecordCell;

            var rowColIndex = new RowColumnIndex(ColumnBase.RowIndex, dataGrid.ResolveToScrollColumnIndex(ColumnBase.ColumnIndex));
            var args = new GridContextMenuEventArgs(ContextMenu, dataContext, rowColIndex, menuType);

            e.Handled =  dataGrid.RaiseGridContextMenuEvent(args);
        }
       
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            isPreviewMouseDown = true;
            ProcessMouseDown(e);
            base.OnPreviewMouseDown(e);
        }
        /// <summary>
        /// When click the pop up and select the row, the OnPreviewMouseDown() method is not hit and the row is not selected because the Scrollviewer handled the OnPreviewMouseDown method. So here we written the OnMouseDown() method to overcome this issue.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!isPreviewMouseDown)
                ProcessMouseDown(e);
            isPreviewMouseDown = false;
            base.OnMouseDown(e);
        }

        private void ProcessMouseDown(MouseButtonEventArgs e)
        {
            if (VisualContainer.GetWantsMouseInput(e.OriginalSource as DependencyObject, this) == false)
            {
                if (!this.IsKeyboardFocusWithin)
                {
                    this.Focus();
                    e.Handled = !ValidationHelper.IsCurrentCellValidated;
                }
            }
            if (ColumnBase != null)
                ColumnBase.RaisePointerPressed(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (ColumnBase != null)
                ColumnBase.RaisePointerReleased(e);
            base.OnPreviewMouseUp(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (ColumnBase != null)
                this.ColumnBase.OnTapped(e);
            base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (ColumnBase != null)
                ColumnBase.OnDoubleTapped(e);
            base.OnMouseDoubleClick(e);
        }
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.Focus();
            if (ColumnBase != null)
                this.ColumnBase.RaisePointerPressed(e);
            e.Handled = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (ColumnBase != null)
            {
                this.ColumnBase.RaisePointerReleased(e);
#if WP
                if (MouseButtonHelper.IsDoubleClick(this, e))
                    this.ColumnBase.OnDoubleTapped(null);
                else
                    this.ColumnBase.OnTapped(null);
#else
                if (MouseButtonHelper.IsDoubleClick(this, e))
                    this.ColumnBase.OnDoubleTapped(e);
                else
                    this.ColumnBase.OnTapped(e);
#endif
            }
            base.OnMouseLeftButtonDown(e);
        }
  
#endif

#if !WinRT && !WP
#if WPF
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
#elif SILVERLIGHT
        protected override void OnMouseWheel(MouseWheelEventArgs e)
#endif
        {
            if (ColumnBase != null)
                ColumnBase.RaisePointerWheel();
#if WPF
            base.OnPreviewMouseWheel(e);
#else
            base.OnMouseWheel(e);
#endif
        }
#endif

        #endregion

        #region Internal Method

        internal void SetError(string errorMessage, bool isAttributeError)
        {
            if (isAttributeError)
                attributeErrorMessage = errorMessage;
            else
                eventErrorMessage = errorMessage;

            ApplyValidationVisualState();
        }

        internal void RemoveError(bool isAttributeError)
        {
            if (isAttributeError)
                attributeErrorMessage = string.Empty;
            else
                eventErrorMessage = string.Empty;
            ApplyValidationVisualState();
        }

        internal void RemoveError()
        {
            attributeErrorMessage = string.Empty;
            bindingErrorMessage = string.Empty;
            ApplyValidationVisualState();
        }

        internal void RemoveAll()
        {
            attributeErrorMessage = string.Empty;
            bindingErrorMessage = string.Empty;
            eventErrorMessage = string.Empty;
            ApplyValidationVisualState();
        }

        internal void ApplyVisualState(bool value)
        {
            VisualStateManager.GoToState(this, value ? "LastCell" : "NormalCell", false);
        }

        #endregion

        #region Dispose

        public virtual void Dispose()
        {
            this.ColumnBase = null;
            ClearValue(IsLastCellProperty);
#if WPF
            this.ContextMenuOpening -= OnContextMenuOpening;
#endif
        }

        #endregion
        
    }

    [ClassReference(IsReviewed = false)]
    public class GridGroupSummaryCell : GridCell
    {
       
        #region Ctor

        public GridGroupSummaryCell()
        {
            this.DefaultStyleKey = typeof(GridGroupSummaryCell);
            this.IsTabStop = false;
        }

        #endregion

        #region Overrides

#if WinRT 
        protected override void OnApplyTemplate() 
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.ApplyVisualState(this.IsLastCell);
        }

#if SILVERLIGHT || WP
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ColumnBase != null)
                this.ColumnBase.RaisePointerPressed(e);
        }
#elif WinRT
        protected override void OnPointerPressed(MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(" GridGroupSummaryCell - On Pointer Pressed");
            if (ColumnBase != null)
                this.ColumnBase.RaisePointerPressed(e);
        }
#endif
        

        #endregion

    }


    [ClassReference(IsReviewed = false)]
    public class GridCaptionSummaryCell : GridCell
    {
        #region Ctor

        public GridCaptionSummaryCell()
        {
            this.DefaultStyleKey = typeof(GridCaptionSummaryCell);
            this.IsTabStop = false;
        }

        #endregion

        #region Overrides

#if WinRT 
        protected override void OnApplyTemplate() 
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.ApplyVisualState(this.IsLastCell);
        }

#if SILVERLIGHT || WP
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ColumnBase != null)
                this.ColumnBase.RaisePointerPressed(e);

        }
#elif WinRT
        protected override void OnPointerPressed(MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(" GridCaptionSummaryCell - On Pointer Pressed");
            if (ColumnBase != null)
                this.ColumnBase.RaisePointerPressed(e);
        }
#endif
        #endregion

    }

    [ClassReference(IsReviewed = false)]
    public class GridIndentCell : GridCell
    {
        #region Dependency Region

        public IndentColumnType ColumnType
        {
            get { return (IndentColumnType)GetValue(ColumnTypeProperty); }
            set { SetValue(ColumnTypeProperty, value); }
        }

        public static readonly DependencyProperty ColumnTypeProperty =
            DependencyProperty.Register("ColumnType", typeof(IndentColumnType), typeof(GridIndentCell), new PropertyMetadata(IndentColumnType.InDataRow, OnColumnTypeChanged));

        #endregion

        #region Dependency CallBack

        private static void OnColumnTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var indentCell = obj as GridIndentCell;
            if (indentCell != null) indentCell.ApplyIndentVisualState((IndentColumnType)args.NewValue);
        }

        #endregion

        #region Ctor

        public GridIndentCell()
        {
            this.DefaultStyleKey = typeof(GridIndentCell);
            this.IsTabStop = false;
        }

        #endregion

        #region Overrides

#if WinRT
        protected override void OnPointerReleased(MouseButtonEventArgs e)
        {
            if (CheckToHandleCell(e))
                e.Handled = true;
            else
                base.OnPointerReleased(e);
        }

        protected override void OnPointerPressed(MouseButtonEventArgs e)
        {
            if (CheckToHandleCell(e))
                e.Handled = true;
            else if (ColumnBase != null)
                this.ColumnBase.RaisePointerPressed(e);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            e.Handled = true;
        }
#elif WPF
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (CheckToHandleCell(e))
                e.Handled = true;
            else
                base.OnPreviewMouseUp(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (CheckToHandleCell(e))
                e.Handled = true;
            else
                base.OnPreviewMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CheckToHandleCell(e))
                e.Handled = true;
            else if (ColumnBase != null)
                this.ColumnBase.RaisePointerPressed(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (CheckToHandleCell(e))
                e.Handled = true;
            else
                base.OnMouseLeftButtonUp(e);
        }
#endif

        

#if WinRT
        protected override void OnApplyTemplate() 
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.ApplyIndentVisualState(this.ColumnType);
        }

        #endregion

        #region Private Methods

        internal void ApplyIndentVisualState(IndentColumnType columnType)
        {
            switch (columnType)
            {
                case IndentColumnType.AfterExpander:
                    VisualStateManager.GoToState(this, "After_Expander", true);
                    break;
                case IndentColumnType.BeforeExpander:
                    VisualStateManager.GoToState(this, "Before_Expander", true);
                    break;
                case IndentColumnType.InExpanderCollapsed:
                    VisualStateManager.GoToState(this, "Expander_Collapsed", true);
                    break;
                case IndentColumnType.InExpanderExpanded:
                    VisualStateManager.GoToState(this, "Expander_Expanded", true);
                    break;
                case IndentColumnType.InLastGroupRow:
                    VisualStateManager.GoToState(this, "Last_GroupRow", true);
                    break;
                case IndentColumnType.InSummaryRow:
                    VisualStateManager.GoToState(this, "SummaryRow", true);
                    break;
                case IndentColumnType.InTableSummaryRow:
                    VisualStateManager.GoToState(this, "TableSummaryRow", true);
                    break;
                case IndentColumnType.InDataRow:
                    VisualStateManager.GoToState(this, "DataRow", true);
                    break;
            }
        }

        private bool CheckToHandleCell(MouseButtonEventArgs e)
        {
            return this.ColumnType == IndentColumnType.BeforeExpander || this.ColumnType == IndentColumnType.InSummaryRow || this.ColumnType == IndentColumnType.InTableSummaryRow;
        }

        #endregion
    }

    [ClassReference(IsReviewed = false)]
    public class GridHeaderIndentCell : GridIndentCell
    {
        #region Ctor

        public GridHeaderIndentCell()
        {
            this.DefaultStyleKey = typeof(GridHeaderIndentCell);
            this.IsTabStop = false;
        }

        #endregion
    }

    [ClassReference(IsReviewed = false)]
    public class GridTableSummaryCell : GridCell
    {
        #region Ctor

        public GridTableSummaryCell()
        {
            this.DefaultStyleKey = typeof(GridTableSummaryCell);
            this.IsTabStop = false;
        }

        #endregion

        #region Overrides

#if WinRT 
        protected override void OnApplyTemplate() 
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.ApplyVisualState(this.IsLastCell);
        }
#if WinRT
        protected override void OnPointerPressed(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnPointerReleased(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }
#elif WPF

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Released)
                e.Handled = true;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed)
                e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Released)
                e.Handled = true;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
#endif

        #endregion

    }
}
