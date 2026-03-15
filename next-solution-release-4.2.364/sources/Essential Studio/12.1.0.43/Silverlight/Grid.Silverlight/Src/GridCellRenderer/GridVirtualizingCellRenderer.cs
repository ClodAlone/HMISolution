#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;

#if !WinRT
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
namespace Syncfusion.Windows.Controls.Grid

#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.GridCommon;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    /// <summary>
    /// Provies a <see cref="AllowKeepAliveOnlyCurrentCell"/> property that gets or sets 
    /// whether the renderer will create the UIElement only when BeginEdit is called for
    /// the current cell and the UIElement should be discarded once EndEdit or CancelEdit
    /// is called. Setting this option is only valid when SupportsRenderOptimization is enabled.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
    /// 
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
            //uiElement.AddHandler(TextBox.SelectionChangedEvent, textBox_SelectionChangedHandler = new RoutedEventHandler(textBox_SelectionChanged));
            base.OnWireUIElement(uiElement);
        }

        public new T CurrentCellUIElement
        {
            get { return (T)base.CurrentCellUIElement; }
            private set { base.CurrentCellUIElement = value; }
        }

        protected override void OnUnwireUIElement(T uiElement)
        {
            //uiElement.RemoveHandler(TextBox.SelectionChangedEvent, textBox_SelectionChangedHandler);
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
#if !WinRT
                textBox.ReleaseMouseCapture();
#else
                textBox.ReleasePointerCaptures();
#endif
                textBox.SelectAll();
            }
        }

        protected override void OnRecaptureMouse(UIElement element)
        {
            if (element is TextBox)
            {
                TextBox textBox = (TextBox)element;
                if (textBoxSelectionStart != -1)
                    textBox.SelectionStart = textBoxSelectionStart;
#if !WinRT
                textBox.CaptureMouse();
                textBox.Focus();
#else
                textBox.CapturePointer(null);
                textBox.Focus(FocusState.Programmatic);
#endif
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
            return CurrentStyle.CellValue;
        }

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



        public override void RefreshContent()
        {
            if (!this.HasCurrentCellState)
                return;

            SetCurrentCellState(GridControl, CellRowColumnIndex, ActivateOptions);
            if (CurrentCellUIElement != null)
                InitializeContent((T)CurrentCellUIElement, CurrentStyle);
            else
                GridControl.InvalidateCell(CellRowColumnIndex);
        }

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

        public override void OnInitializeContent(T uiElement, GridRenderStyleInfo style)
        {
            /*Thickness margins = style.TextMargins.ToThickness();

            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            //margins.Left = Math.Max(0, margins.Left - 2);
            //margins.Right = Math.Max(0, margins.Right - 2);

                GridFontInfo font = style.ReadOnlyFont;

            TextBlock tb = uiElement as TextBlock;
            if (false && tb != null)
            {
                tb.FontFamily = font.FontFamily;
                tb.FontSize= font.FontSize;
                tb.FontStretch = font.FontStretch;
                tb.FontWeight = font.FontWeight;
                tb.FontStyle = font.FontStyle;
                tb.Foreground = style.Foreground;
                tb.HorizontalAlignment = style.HorizontalAlignment;
                tb.Margin = style.TextMargins.ToThickness();
                tb.Padding = style.BorderMargins.ToThickness();
                tb.TextDecorations = font.TextDecorations;
                tb.TextWrapping = style.TextWrapping;
                tb.VerticalAlignment = style.VerticalAlignment;
                }
            else
            {
                uiElement.SetValue(TextBlock.PaddingProperty, margins);

                uiElement.SetValue(TextBlock.FontFamilyProperty, font.FontFamily);
                uiElement.SetValue(TextBlock.FontSizeProperty, font.FontSize);
                uiElement.SetValue(TextBlock.FontStretchProperty, font.FontStretch);
                uiElement.SetValue(TextBlock.FontWeightProperty, font.FontWeight);
                uiElement.SetValue(TextBlock.FontStyleProperty, font.FontStyle);
                uiElement.SetValue(TextBlock.TextDecorationsProperty, font.TextDecorations);
                //if (font.Orientation != 0)
                //    uiElement.RenderTransform = new RotateTransform(font.Orientation);

                uiElement.SetValue(TextBlock.TextAlignmentProperty, HorizontalAlignmentToTextAlignment(style.HorizontalAlignment));
                //uiElement.SetValue(TextBox.HorizontalContentAlignmentProperty, style.HorizontalAlignment);
                //uiElement.SetValue(TextBox.HorizontalAlignmentProperty, style.HorizontalAlignment);
                //uiElement.SetValue(TextBox.VerticalAlignmentProperty, style.VerticalAlignment);
                //uiElement.SetValue(TextBox.VerticalContentAlignmentProperty, style.VerticalAlignment);
                uiElement.SetValue(TextBlock.ForegroundProperty, style.Foreground);
                uiElement.SetValue(TextBlock.TextWrappingProperty, style.TextWrapping);
                //uiElement.SetValue(TextBox.CharacterCasingProperty, style.CharacterCasing);
                //uiElement.SetValue(TextBox.AutoWordSelectionProperty, style.AutoWordSelection);
                //uiElement.SetValue(TextBox.AcceptsReturnProperty, style.AcceptsReturn);

                //uiElement.SetValue(ToolTip.ToolTipProperty, style.ToolTip);
            }*/
        }

        // Converts a HorizontalAlignment enum to a TextAlignment enum.
        internal override TextAlignment HorizontalAlignmentToTextAlignment(HorizontalAlignment horizontalAlignment)
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

                case HorizontalAlignment.Stretch:
                case HorizontalAlignment.Center:
                    textAlignment = TextAlignment.Center;
                    break;

            }

            return textAlignment;
        }
    }
}