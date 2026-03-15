#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Provies a <see cref="AllowKeepAliveOnlyCurrentCell"/> property that gets or sets 
    /// whether the renderer will create the UIElement only when BeginEdit is called for
    /// the current cell and the UIElement should be discarded once EndEdit or CancelEdit
    /// is called. Setting this option is only valid when SupportsRenderOptimization is enabled.
    /// </summary>
    public interface IAllowKeepAliveOnlyCurrentCell
    {
        /// <summary>
        /// Gets or sets whether the renderer will create the UIElement only when
        /// BeginEdit is called for the current cell and the UIElement should be
        /// discarded once EndEdit or CancelEdit is called. Setting this option
        /// is only valid when SupportsRenderOptimization is enabled.
        /// </summary>
        bool AllowKeepAliveOnlyCurrentCell
        {
            get;
            set;
        }
    }

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
    /// code for the GridVirtualizingCellRendererBase base class between grid,
    /// tree and common assemblies and keep tree/grid control specific code
    /// out of the base class. It is currently not possible with C# to the base class as 
    /// template type parameter. This is the reason for this copy/paste approach for the 
    /// codebase for the base class of this class.
    /// </summary>
    /// <typeparam name="T">The type of the UIElement that should be placed inside cells</typeparam>
    public abstract class GridVirtualizingCellRenderer<T> : GridVirtualizingCellRendererBase<T>, IAllowKeepAliveOnlyCurrentCell
        where T : FrameworkElement, new()   // FrameworkElement required for Unloaded event.
    {
        bool allowKeepAliveOnlyCurrentCell = false;
        int textBoxSelectionStart = -1;
        RoutedEventHandler textBox_SelectionChangedHandler;
        //Thickness smallestMargins = new Thickness();

        //public Thickness SmallestMargins
        //{
        //    get { return smallestMargins; }
        //    set { smallestMargins = value; }
        //}

        /// <summary>
        /// Initializes a new <see cref="GridVirtualizingCellRenderer"/>.
        /// </summary>
        public GridVirtualizingCellRenderer()
        {
            EditorType = typeof(T);
        }

        /// <summary>
        /// Gets or sets whether the renderer will create the UIElement only when
        /// BeginEdit is called for the current cell and the UIElement should be
        /// discarded once EndEdit or CancelEdit is called. Setting this option
        /// is only valid when SupportsRenderOptimization is enabled.
        /// </summary>
        public bool AllowKeepAliveOnlyCurrentCell
        {
            get { return allowKeepAliveOnlyCurrentCell; }
            set { allowKeepAliveOnlyCurrentCell = value; }
        }

        protected override void OnWireUIElement(T uiElement)
        {
            uiElement.AddHandler(TextBox.SelectionChangedEvent, textBox_SelectionChangedHandler = new RoutedEventHandler(textBox_SelectionChanged));
            base.OnWireUIElement(uiElement);
        }

        /// <summary>
        /// Gets or sets the UI element for the current cell.
        /// </summary>
        public new T CurrentCellUIElement
        {
            get { return (T)base.CurrentCellUIElement; }
            private set { base.CurrentCellUIElement = value; }
        }

        protected override void OnUnwireUIElement(T uiElement)
        {
            uiElement.RemoveHandler(TextBox.SelectionChangedEvent, textBox_SelectionChangedHandler);
            base.OnUnwireUIElement(uiElement);
        }

        void textBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (IsInArrange)
                return;

            TextBox textBox = e.OriginalSource as TextBox;
            if (textBox != null && textBox.SelectionLength == 0)
                textBoxSelectionStart = textBox.SelectionStart;
        }

        protected override void OnCancelMouseCapture(UIElement element)
        {
            // TODO: Add support for other stock controls and allow support for other controls to participate in cancel/recapture mechanism.
            if (element is TextBox)
            {
                TextBox textBox = (TextBox)element;
                textBox.ReleaseMouseCapture();
                textBox.SelectAll();
            }
            else
            {
                IInputElement ie = element as IInputElement;
                if (ie != null)
                {
                    ie.ReleaseMouseCapture();
                }
            }
        }

        protected override void OnRecaptureMouse(UIElement element)
        {
            if (element is TextBox)
            {
                TextBox textBox = (TextBox)element;
                if (textBoxSelectionStart != -1)
                    textBox.SelectionStart = textBoxSelectionStart;
                textBox.CaptureMouse();
                textBox.Focus();
            }
            else
            {
                IInputElement ie = element as IInputElement;
                if (ie != null)
                {
                    ie.CaptureMouse();
                }
            }
        }

        protected override object GetControlValueFromEditor()
        {
            if (InInitializeContent)
                return CurrentStyle.CellValue;

            if (IsControlTextShown)
            {
                string text = GetControlTextFromEditor();
                ApplyControlText(CurrentStyleCopy, text);
                return CurrentStyleCopy.CellValue;
            }

            if (CurrentCellUIElement == null)
                return CurrentStyle.CellValue;

            return GetControlValueFromEditorCore((T)CurrentCellUIElement);
        }


        protected virtual object GetControlValueFromEditorCore(T uiElement)
        {
            //throw new NotImplementedException("GetControlValueFromEditor(uiElement) is not implemented in derived renderer when IsDisplayingCellValueAsText is false.");
            return CurrentStyle.CellValue;
        }

        /// <summary>
        /// Returns the ControlText from the current cell's editor control.
        /// </summary>
        /// <returns>ControlText of the current cell editor.</returns>
        public sealed override string GetControlTextFromEditor()
        {
            if (CurrentCellUIElement != null)
                return GetControlTextFromEditorCore(CurrentCellUIElement);
            else
            {
                try
                {
                    return GetControlTextCore(CurrentStyle, CurrentStyle.CellValue);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        protected virtual string GetControlTextFromEditorCore(T uiElement)
        {
            return uiElement.ToString();
        }


        /// <summary>
        /// Refreshes the content of current cell.
        /// </summary>
        public override void RefreshContent()
        {
            if (!this.HasCurrentCellState)
                return;

            SetCurrentCellState(GridControl, CellRowColumnIndex, ActivateOptions);
            if (CurrentCellUIElement != null)
            {
                T uiElement = (T)CurrentCellUIElement;
                ArrangeCellArgs aca = VirtualizingCellsControl.GetArrangeCellArgs(uiElement);
                InitializeContent(uiElement, CurrentStyle);
                ArrangeUIElement(aca, uiElement, CurrentStyle);
            }
            else
                GridControl.InvalidateCell(CellRowColumnIndex);
        }

        /// <summary>
        /// Returns true if current cell holds an UI element.
        /// </summary>
        /// <returns>True if current cell has an UI element; false otherwise.</returns>
        public bool EnsureCurrentCellUIElement()
        {
            if (CurrentCellUIElement != null)
                return false;

            GridControl.DelayedCreateCellUIElements(CellRowColumnIndex);
            CellUIElements visuals = GridControl.GetCellUIElements(CellRowColumnIndex);
            if (visuals != null)
            {
                CurrentCellUIElement = GetUIElement(visuals);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Initializes the content of the cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>       
        /// <param name="uiElement">The current cell ui element.</param>
        /// <param name="style">The cell style info.</param>
        public override void CreateRendererElement(T uiElement, GridRenderStyleInfo style)
        {
            this.InitUIElementProperties(uiElement, style);
        }

        private void InitUIElementProperties(T uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            }
            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            //margins.Left = Math.Max(0, margins.Left - 2);
            //margins.Right = Math.Max(0, margins.Right - 2);


            GridFontInfo font = style.ReadOnlyFont;
            uiElement.SetValue(TextBox.FontFamilyProperty, font.FontFamily);
            uiElement.SetValue(TextBox.FontSizeProperty, font.FontSize);
            uiElement.SetValue(TextBox.FontStretchProperty, font.FontStretch);
            uiElement.SetValue(TextBox.FontWeightProperty, font.FontWeight);
            uiElement.SetValue(TextBox.FontStyleProperty, font.FontStyle);
            uiElement.SetValue(TextBox.TextDecorationsProperty, font.TextDecorations);
            uiElement.SetValue(TextBox.TextAlignmentProperty, HorizontalAlignmentToTextAlignment(style.HorizontalAlignment));
           
                uiElement.SetValue(TextBox.HorizontalContentAlignmentProperty, style.HorizontalAlignment);
                uiElement.SetValue(TextBox.HorizontalAlignmentProperty, style.HorizontalAlignment);
                uiElement.SetValue(TextBox.VerticalAlignmentProperty, style.VerticalAlignment);
                uiElement.SetValue(TextBox.VerticalContentAlignmentProperty, style.VerticalAlignment);
                //uiElement.SetValue(TextBox.UndoLimitProperty, 0);
                //uiElement.ClearValue(TextBox.UndoLimitProperty);
            
            if (font.Orientation != 0)
                uiElement.RenderTransform = new RotateTransform(font.Orientation);

           if (!(this is GridCellDataBoundTemplateRenderer))
            {
                uiElement.SetValue(TextBox.PaddingProperty, margins);                 
                uiElement.SetValue(TextBox.TextWrappingProperty, style.TextWrapping);
                uiElement.SetValue(TextBox.CharacterCasingProperty, style.CharacterCasing);
                uiElement.SetValue(TextBox.AutoWordSelectionProperty, style.AutoWordSelection);
                uiElement.SetValue(TextBox.AcceptsReturnProperty, style.AcceptsReturn);

            }

            uiElement.SetValue(TextBox.ForegroundProperty, style.Foreground);
            
#if SyncfusionFramework4_0
            uiElement.SetValue(TextBox.SelectionBrushProperty, style.SelectionBrush);
            uiElement.SetValue(TextBox.SelectionOpacityProperty, style.SelectionOpacity);
            uiElement.SetValue(TextBox.CaretBrushProperty, style.CaretBrush);
#endif
            if (style.ReadOnlyIsThemed)
            {
                var gridVisualStyle = GridVisualStyleHelper.GetVisualStyleOfGrid(uiElement);
                if (gridVisualStyle != string.Empty)
                {
                    SkinStorage.SetVisualStyle(uiElement, gridVisualStyle);
                }
                else
                {
                    SkinStorage.SetVisualStyle(uiElement, "Default");
                }
            }

            //if (style.HasToolTip)
            //{
            //    var tooltip = (ToolTip)style.ToolTip;
            //    if (style.HasTooltipTemplateKey)
            //    {
            //        var dt = (DataTemplate)style.GridControl.TryFindResource(style.TooltipTemplateKey);
            //        if (dt != null)
            //        {
            //            tooltip.Content = style;
            //            tooltip.ContentTemplate = dt;
            //        }
            //    }
            //    uiElement.SetValue(ToolTip.ToolTipProperty, style.ToolTip);
            //}
            //else
            //    if (style.HasShowTooltip && style.ShowTooltip)
            //    {
                    //Tooltip is intialized in TooltipService.cs file.


                    //var tooltip = new ToolTip();
                    //if (style.HasTooltipTemplateKey)
                    //{
                    //    tooltip.Background = Brushes.Transparent;
                    //    tooltip.BorderBrush = Brushes.Transparent;
                    //    var dt = (DataTemplate)style.GridControl.TryFindResource(style.TooltipTemplateKey);
                    //    if (dt != null)
                    //    {
                    //        tooltip.Content = style;
                    //        tooltip.ContentTemplate = dt;
                    //    }
                    //}
                    //else
                    //{
                    //    tooltip.Content = style.CellValue;
                    //}
                    //uiElement.SetValue(ToolTip.ToolTipProperty, tooltip);
                //}
                //else
                //{
                //    uiElement.SetValue(ToolTip.ToolTipProperty, null);
                //}
        }

        /// <summary>
        /// Initializes the content of the cell when in edit mode
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>       
        /// <param name="uiElement">The current cell ui element.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(T uiElement, GridRenderStyleInfo style)
        {
            this.InitUIElementProperties(uiElement, style);
        }
        // Converts a HorizontalAlignment enum to a TextAlignment enum.
        internal TextAlignment HorizontalAlignmentToTextAlignment(HorizontalAlignment horizontalAlignment)
        {
            TextAlignment textAlignment;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                default:
                    textAlignment = TextAlignment.Left;
                    break;

                case HorizontalAlignment.Right:
                    textAlignment = TextAlignment.Right;
                    break;

                case HorizontalAlignment.Center:
                    textAlignment = TextAlignment.Center;
                    break;

                case HorizontalAlignment.Stretch:
                    textAlignment = TextAlignment.Justify;
                    break;
            }

            return textAlignment;
        }

        /// <summary>
        /// Triggers GridPreviewMouseMove event.
        /// </summary>
        /// <param name="rci">The cell row column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> object.</param>
        public override void RaiseGridPreviewMouseMove(RowColumnIndex rci, System.Windows.Input.MouseEventArgs e)
        {
            if (!AllowKeepAliveOnlyCurrentCell)
            {
                if ((GridControl.Model.Options.ActivateCurrentCellBehavior & (GridCellActivateAction.ClickOnCell | GridCellActivateAction.SetCurrent)) != 0
                    && GridUtil.GetMouseButton(e) == null)
                {
                    GridControl.DelayedCreateCellUIElements(rci);
                }
            }

            base.RaiseGridPreviewMouseMove(rci, e);
        }

        protected void RollbackTextChange(TextBoxBase textBox)
        {
            textBox.Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(OnRollbackTextChange), textBox);
        }

        private object OnRollbackTextChange(object o)
        {
            TextBox tb = o as TextBox;
            if (tb != null)
                tb.Undo();

            return null;
        }

        /// <summary>
        /// Empties recyclebin.
        /// </summary>
        public override void EmptyRecycleBin()
        {
            if (recycleBin != null)
                recycleBin.Clear();
        }
    }
}