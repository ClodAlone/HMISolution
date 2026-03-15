#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WinRT
using Syncfusion.UI.Xaml.ScrollAxis;
using Windows.UI.Xaml;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.ScrollAxis;
using System.Windows.Data;
using System.Windows.Input;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
    using EventArgs = PointerRoutedEventArgs;
#endif
    /// <summary>
    /// GridVirtualizingCellRenderer is an abstract base class for cell renderers
    /// that need live UIElement visuals displayed in a cell. You can derive from
    /// this class and provide the type of the UIElement you want to show inside cells
    /// as type paramater. The class provides strong typed virtual methods for 
    /// initializing content of the cell and arranging the cell visuals. See 
    /// <see cref="GridVirtualizingCellRendererBase{T}"/> for more details.
    /// <para/>
    /// The idea behind this class is to provide a place where we can 
    /// add general code that should be shared for all cell renderers in the tree derived
    /// from GridVirtualizingCellRendererBase. While this class does at
    /// the moment not add meaningfull functionality to GridVirtualizingCellRendererBase
    /// we created this extra layer of inheritance to make it easy to share 
    /// code for the GridVirtualizingCellRendererBase base class between grid and
    /// common assemblies and keep grid control specific code
    /// out of the base class. It is currently not possible with C# to the base class as 
    /// template type parameter.
    /// </summary>
    /// <typeparam name="T">The type of the UIElement that should be placed inside cells</typeparam>
    [ClassReference(IsReviewed = false)]
    public abstract class GridVirtualizingCellRenderer<D, E> : GridVirtualizingCellRendererBase<D,E>
        where D : FrameworkElement, new()
        where E : FrameworkElement, new()
    {
        #region Property
        public Type EditorType { get; set; }
        #endregion

        #region Ctor

        protected GridVirtualizingCellRenderer()
        {
            this.EditorType = typeof(E);
        }

        #endregion

        #region Private Methods
        internal bool CheckControlKeyPressed()
        {
#if WinRT
            return (Window.Current.CoreWindow.GetAsyncKeyState(Key.Control).HasFlag(CoreVirtualKeyStates.Down));
#elif WPF
            return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
#else
            return (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control));
#endif
        }

        internal bool CheckAltKeyPressed()
        {
#if WinRT
            return (Window.Current.CoreWindow.GetAsyncKeyState(Key.Menu).HasFlag(CoreVirtualKeyStates.Down));
#elif WPF
            return (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
#else
            return (((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt));
#endif
        }

#if WinRT
        internal bool CheckCapsKeyPressed()
        {
            return (Window.Current.CoreWindow.GetAsyncKeyState(Key.CapitalLock).HasFlag(CoreVirtualKeyStates.Down));
        }
#endif
        internal bool CheckShiftKeyPressed()
        {
#if WinRT
            return (Window.Current.CoreWindow.GetAsyncKeyState(Key.Shift).HasFlag(CoreVirtualKeyStates.Down));
#elif WPF
            return (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
#else
            return (((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift));
#endif
        }
        #endregion

        #region Protected Methods
#if !WP
        /// <summary>
        /// Raises the current cell validating event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="column">The column.</param>
        /// <param name="changedNewValue">The changed new value.</param>
        /// <returns></returns>
        protected bool RaiseCurrentCellValidatingEvent(object oldValue, object newValue, GridColumn column, out object changedNewValue, object dataGrid)
        {
            var e = new CurrentCellValidatingEventArgs(dataGrid)
            {
                OldValue = oldValue,
                NewValue = newValue,
                Column = column
            };
            bool isSuspendValidating = DataGrid.RaiseCurrentCellValidatingEvent(e);
            changedNewValue = e.NewValue;
            return isSuspendValidating;
        }

        /// <summary>
        /// Raises the current cell validated event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="column">The column.</param>
        protected void RaiseCurrentCellValidatedEvent(object oldValue, object newValue, GridColumn column, object dataGrid)
        {
            var e = new CurrentCellValidatedEventArgs(dataGrid)
            {
                OldValue = oldValue,
                NewValue = newValue,
                Column = column
            };
            DataGrid.RaiseCurrentCellValidatedEvent(e);
        }
#endif
        /// <summary>
        /// Texts the alignment to horizontal alignment.
        /// </summary>
        /// <param name="textAlignment">The text alignment.</param>
        /// <returns></returns>
        protected HorizontalAlignment TextAlignmentToHorizontalAlignment(TextAlignment textAlignment)
        {
            HorizontalAlignment horizontalAlignment;

            switch (textAlignment)
            {
                case TextAlignment.Right:
                    horizontalAlignment = HorizontalAlignment.Right;
                    break;

                case TextAlignment.Center:
                    horizontalAlignment = HorizontalAlignment.Center;
                    break;

                case TextAlignment.Justify:
                    horizontalAlignment = HorizontalAlignment.Stretch;
                    break;
                default:
                    horizontalAlignment = HorizontalAlignment.Left;
                    break;
            }
            return horizontalAlignment;
        }
        #endregion

        #region Virtual Methods
#if !WP
        protected virtual void CurrentRendererValueChanged()
        {
            DataGrid.RaiseCurrentCellValueChangedEvent(new CurrentCellValueChangedEventArgs { RowColumnIndex = CurrentCellIndex });
#if !WinRT && !Silverlight4
            var column = (DataGrid.Columns[DataGrid.ResolveToGridVisibleColumnIndex(CurrentCellIndex.ColumnIndex)]);
            if (column.UpdateTrigger != UpdateSourceTrigger.PropertyChanged || BindingExpression == null) 
                return;
            object oldValue = DataGrid.View.GetPropertyAccessProvider()
                             .GetValue(BindingExpression.DataItem, BindingExpression.ParentBinding.Path.Path)
                             .ToString();
            BindingExpression.UpdateSource();
            var Text = DataGrid.View.GetPropertyAccessProvider()
                       .GetValue(BindingExpression.DataItem, BindingExpression.ParentBinding.Path.Path)
                       .ToString();
            object newValue;
            string errorMessage;
            if (!DataGrid.Validations.RaiseCurrentCellValidatingEvent(oldValue, Text, column, out newValue, CurrentCellIndex, CurrentCellElement, out errorMessage, BindingExpression.DataItem)) 
                return;
            if (!ReferenceEquals(newValue, Text))
                SetControlValue(newValue);

            if (DataGrid.GridValidationMode != GridValidationMode.None)
                DataGrid.Validations.ValidateColumn(BindingExpression.DataItem, BindingExpression.ParentBinding.Path.Path, (GridCell)CurrentCellElement, CurrentCellIndex);

            DataGrid.Validations.RaiseCurrentCellValidatedEvent(oldValue, Text, column, errorMessage, BindingExpression.DataItem);
#endif
        }
#endif
        #endregion

        #region Override Methods
        protected override bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Up:
                case Key.Down:
                case Key.F2:
                case Key.Escape:
                case Key.PageDown:
                case Key.PageUp:
              
                case Key.Delete:
                case Key.Left:
                case Key.Right:
                case Key.Home:
                case Key.End:
                {
                    e.Handled = true;
                    return true;
                }
                case Key.Tab:
                return true;
                case Key.C:
                case Key.V:
                case Key.X:
                case Key.A:
                    return CheckControlKeyPressed() && !IsInEditing;
            }
            return false;
        }

        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, D uiElement, GridColumn column, object dataContext)
        {
            uiElement.SetBinding(TextBlock.TextProperty, column.DisplayBinding);
            var textAlignmentBind = new Binding { Path = new PropertyPath("TextAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBlock.TextAlignmentProperty, textAlignmentBind);
            var textTrimmingBind = new Binding { Path = new PropertyPath("TextTrimming"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBlock.TextTrimmingProperty, textTrimmingBind);
#if WPF
            var textDecorations = new Binding { Path = new PropertyPath("TextDecorations"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBlock.TextDecorationsProperty, textDecorations);
            uiElement.VerticalAlignment = VerticalAlignment.Center;
#else
            uiElement.VerticalAlignment = VerticalAlignment.Stretch;
#endif
        }

        public override void OnUpdateDisplayBinding(RowColumnIndex cellRowcolumnIndex, D uiElement, GridColumn column, object dataContext)
        {
            OnInitializeDisplayElement(cellRowcolumnIndex, uiElement, column, dataContext);
        }

        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, E uiElement, GridColumn column, object dataContext)
        {
            var textPadding = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(Control.PaddingProperty, textPadding);
            var textAlignBind = new Binding { Path = new PropertyPath("TextAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBox.TextAlignmentProperty, textAlignBind);
            var textWrappingBinding = new Binding { Path = new PropertyPath("TextWrapping"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBox.TextWrappingProperty, textWrappingBinding); 
            uiElement.VerticalAlignment = VerticalAlignment.Stretch;
        }

        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, E element, GridColumn column, object dataContext)
        {
            OnInitializeEditElement(cellRowcolumnIndex, element, column, dataContext);
        }
        #endregion
    }
}
