#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Implements the data model for a button cell.
    /// </summary>
    public class GridCellButtonModel : GridCellModel<GridCellButtonRenderer>
    {
        /// <summary>
        /// Returns the formatted text for the specified value.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds the cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            return style.Description;
        }
    }

    /// <summary>
    /// Implements the renderer part of a button cell.
    /// </summary>
    public class GridCellButtonRenderer : GridVirtualizingCellRenderer<Button>
    {
        private ButtonPaint buttonPaint;

        private bool firstEditor = true;

        /// <summary>
        /// Initializes the button cell renderer.
        /// </summary>
        public GridCellButtonRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowTransparentBackground = false;
            this.AllowRecycle = true;
            this.IsControlTextShown = true;
            this.IsModifiable = false;
            this.IsFocusable = true;
            this.buttonPaint = new ButtonPaint();
        }

        /// <summary>
        /// Initializes the content of the button cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(Button button, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(button, style);
            ////NOTE: Changing values here might cause the control to animate (e.g. setting CheckBox.IsChecked)
            Thickness margins = new Thickness(0);
            if (style.FlowDirection == FlowDirection.RightToLeft)
                {
                button.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = button.Width;
                double offsetY = 0;
                button.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
                }
            else
                {
                button.LayoutTransform = MatrixTransform.Identity;
                }
            button.Padding = margins;
            button = this.buttonPaint.GetButtonVisualStyle(button, style);
            ////There is no way offered by built-in WPF controls to turn off this animation.
            ////button.SetValue(Control.BackgroundProperty, DependencyProperty.UnsetValue);
            if (!string.IsNullOrEmpty(style.Description))
                button.Content = style.Description;
            else
                button.Content = style.CellValue;

            button.ClickMode = ClickMode.Release;
            if (this.firstEditor)
            {
                this.GridControl.InvalidateArrange();
                this.firstEditor = false;
            }
        }

        public override void CreateRendererElement(Button button, GridRenderStyleInfo style)
        {
            //////NOTE: Changing values here might cause the control to animate (e.g. setting CheckBox.IsChecked)
            Thickness margins = new Thickness(0);

            button.Padding = margins;
            ////There is no way offered by built-in WPF controls to turn off this animation.
            ////button.SetValue(Control.BackgroundProperty, DependencyProperty.UnsetValue);

            if (!string.IsNullOrEmpty(style.Description))
                button.Content = style.Description;
            else
                button.Content = style.CellValue;

            button.ClickMode = ClickMode.Release;
            if (this.firstEditor)
            {
                this.GridControl.InvalidateArrange();
                this.firstEditor = false;
            }
            base.CreateRendererElement(button, style);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, Button uiElement, GridRenderStyleInfo style)
        {
            this.SetBounds(uiElement, aca.OriginalCellRect, aca.ForceMeasure, false);
        }

        protected override string GetControlTextFromEditorCore(Button button)
        {
            return button.Content != null ? button.Content.ToString() : string.Empty;
        }

        protected override void OnActivated()
        {
            GridControl.PreviewKeyDown += new KeyEventHandler(this.GridControl_PreviewKeyDown);
            base.OnActivated();
        }

        protected override void OnDeactivated()
        {
            GridControl.PreviewKeyDown -= new KeyEventHandler(this.GridControl_PreviewKeyDown);
            base.OnDeactivated();
        }

        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
            {
                return;
            }
            string text;
            // Will only get hit if SupportsRenderOptimization is true ...
            Rect textRectangle = rca.OriginalCellRect;
            if(! string.IsNullOrEmpty( style.Description))
                 text=style.Description;
            else
                text =Convert.ToString(style.CellValue);

            // Draw the formatted text string to the DrawingContext of the control.
            this.buttonPaint.DrawButton(dc, textRectangle, text, style);

            base.OnRender(dc, rca, style);
        }

        /// <summary>
        /// Unwire previously wired events from button. 
        /// </summary>
        /// <param name="button">The cell button.</param>
        protected override void OnUnwireUIElement(Button button)
        {
            button.Click -= new RoutedEventHandler(this.Button_Click);
        }

        /// <summary>
        /// Wire events from button.
        /// </summary>
        /// <param name="button">The cell button.</param>
        protected override void OnWireUIElement(Button button)
        {
            button.Click += new RoutedEventHandler(this.Button_Click);
        }
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // This false value is returned to handle the PreviewKeyDown event
                return false;
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            RowColumnIndex cp = VirtualizingCellsControl.GetCellRowColumnIndex(button);
            ////MessageBox.Show(String.Format("You clicked on button at row {0} and column {1}", cp.RowIndex, cp.ColumnIndex));
            GridControl.RaiseGridCellButtonClick(cp.RowIndex, cp.ColumnIndex);
        }

        private void GridControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (this.CurrentCellUIElement != null)
                {
                    this.CurrentCellUIElement.Focus();
                }
                else
                {
                    this.CurrentCell.ScrollInView();
                    this.CurrentCell.BeginEdit();
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (this.CurrentCellUIElement != null)
                {
                    this.CurrentCellUIElement.Focus();
                }
                else
                {
                    this.CurrentCell.ScrollInView();
                    this.CurrentCell.BeginEdit();
                }
            }

        }
    }

    /// <summary>
    /// Paints a button control in the grid cells.
    /// </summary>
    public class ButtonPaint
    {
        private Dictionary<Size, VisualBrush> brushes = new Dictionary<Size, VisualBrush>();

        private Thickness margin = new Thickness(0);

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ButtonPaint()
        {
        }

        /// <summary>
        /// Gets or sets the button margins.
        /// </summary>
        public Thickness Margin
        {
            get { return this.margin; }
            set { this.margin = value; }
        }

        /// <summary>
        /// Draws the button in the cell rectangle.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rc">Cell rectangle.</param>
        /// <param name="text">Text to be drawn over the button.</param>
        /// <param name="style">Cell style information.</param>
        /// <returns>Cell margins.</returns>
        public Thickness DrawButton(DrawingContext dc, Rect rc, string text, GridStyleInfo style)
        {
            VisualBrush vb = this.GetVisualBrush(rc.Size, style);
            dc.DrawRectangle(vb, null, rc);
            this.margin = style.TextMargins.ToThickness();
          
          
            if (style.HorizontalAlignment == HorizontalAlignment.Center)
            {
                if (style.VerticalAlignment == VerticalAlignment.Top)
                    margin.Top += 1;
                else if (style.VerticalAlignment == VerticalAlignment.Bottom)
                    margin.Bottom += 1;
                else
                    margin.Top += 2;
            }
            if (style.HorizontalAlignment == HorizontalAlignment.Left|| style.HorizontalAlignment==HorizontalAlignment.Stretch)
            {
                margin.Left+=2;
                if(style.VerticalAlignment==VerticalAlignment.Top)
                margin.Top += 3;
                else if(style.VerticalAlignment==VerticalAlignment.Bottom)
                 margin.Bottom+=1;
                else
                    margin.Top += 2;
            }
            if (style.HorizontalAlignment == HorizontalAlignment.Right)
            {
                margin.Right += 2;
                if (style.VerticalAlignment == VerticalAlignment.Top)
                    margin.Top += 3;

                else if (style.VerticalAlignment == VerticalAlignment.Bottom)
                    margin.Bottom += 1;
                else
                    margin.Top += 2;
            }
            rc = this.SubtractBorderMargins(rc, this.margin);
            
            GridTextBoxPaint.DrawText(dc, rc, text, style);
            return this.margin;
        }


        /// <summary>
        /// Removes border margins to get cells client area.
        /// </summary>
        /// <param name="cellRect">Cell rectangle.</param>
        /// <param name="mi">The border margins.</param>
        /// <returns>Returns the cells client area.</returns>
        public Rect SubtractBorderMargins(Rect cellRect, Thickness mi)
        {
            if (cellRect.IsEmpty || cellRect.Width <= mi.Left + mi.Right || cellRect.Height <= mi.Top + mi.Bottom)
            {
                return new Rect(0, 0, 0, 0);
            }

            cellRect.Height -= mi.Bottom + mi.Top;
            cellRect.Y += mi.Top;
            cellRect.Width -= mi.Right + mi.Left;
            cellRect.X += mi.Left;

            return cellRect;
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
        }

        private VisualBrush GetVisualBrush(Size size, GridStyleInfo style)
        {
            if (this.brushes.ContainsKey(size))
            {
                return this.brushes[size];
            }

            bool wasAnimated = GridUtil.IsAnimated;
            try
            {
                GridUtil.IsAnimated = false;

                VisualBrush visualBrush;
                Button b = new Button();
                b.BeginInit();
                b.Content = null;
                b.ClickMode = ClickMode.Release;
                b.Width = size.Width;
                b.Height = size.Height;
                b.EndInit();
                b.Measure(size);
                b.Arrange(new Rect(size));

                ContentPresenter cp = FindVisualChild<ContentPresenter>(b);

                PropertyInfo pi = cp.GetType().GetProperty("PreviousArrangeRect", BindingFlags.Instance | BindingFlags.NonPublic);
                Rect r = (Rect)pi.GetValue(cp, null);

                this.margin = new Thickness(r.Left, r.Top, size.Width - r.Width - r.Left, size.Height - r.Height - r.Top);
                b = GetButtonVisualStyle(b, style);
                visualBrush = new VisualBrush();
                visualBrush.Visual = b;
                this.brushes[size] = visualBrush;
                return visualBrush;
            }
            finally
            {
                GridUtil.IsAnimated = wasAnimated;
            }
        }

        /// <summary>
        /// Returns the button VisualStyle.
        /// </summary>
        public Button GetButtonVisualStyle(Button button, GridStyleInfo style)
        {
            if (style.GridModel is GridDataTableModel)
            {
                var gridTableModel = style.GridModel as GridDataTableModel;
                if (style.IsThemed)
                    gridTableModel.GetVisualStyleDictionary(button);
            }
            return button;
        }
    }
}
