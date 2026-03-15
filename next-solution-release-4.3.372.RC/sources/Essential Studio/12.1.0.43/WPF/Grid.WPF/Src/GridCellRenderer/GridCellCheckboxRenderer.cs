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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Styles;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// This enumeration indicates whether the checkbox checker should be placed either before the content or after the content.
    /// </summary>
    public enum CheckerPlacement
    {
        /// <summary>
        /// Checkbox checker appears before the cell content.
        /// </summary>
        BeforeContent,
        /// <summary>
        /// Checkbox checker appears after the cell content.
        /// </summary>
        AfterContent
    }

    /// <summary>
    /// Implements the data model part of a checkbox cell.
    /// </summary>
    public class GridCellCheckboxModel : GridCellModel<GridCellCheckboxRenderer>
    {
        protected override Size OnQueryPrefferedClientSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            return new Size(13, 13);
        }
    }

    /// <summary>
    /// Implements the renderer part of a checkbox cell.
    /// </summary>
    public class GridCellCheckboxRenderer : GridVirtualizingCellRenderer<CheckBox>
    {
        private static readonly bool?[] isCheckedValues = new bool?[] { null, true, false };
        private Dictionary<int, Queue<CheckBox>> checkboxes = new Dictionary<int, Queue<CheckBox>>();
        private CheckBoxPaint checkBoxPaint;
        private int queueLoad = 20;

        /// <summary>
        /// Initializes a new <see cref="GridCellCheckboxRenderer"/> object for the given cell.
        /// </summary>
        public GridCellCheckboxRenderer()
        {
            this.AllowKeepAliveOnlyCurrentCell = false;
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            this.IsModifiable = true;
            this.IsFocusable = true;

            CheckBox cb = new CheckBox();
            this.checkBoxPaint = new CheckBoxPaint(CheckerPlacement.BeforeContent, false, new Thickness(0), new Typeface(cb.FontFamily.Source), cb.FontSize, Brushes.Black);
            this.checkBoxPaint.Trimming = TextTrimming.None;
            this.checkBoxPaint.HorizontalAlignment = TextAlignment.Center;
            this.checkBoxPaint.VerticalAlignment = VerticalAlignment.Center;
            this.checkBoxPaint.WrapText = true;

            // Checkbox does not offer any way to turn off the animation. We
            // have to workaround this, otherwise animation occurs while
            // scrolling through grid.
            this.PreventAnimationWorkaround();
        }

        /// <summary>
        /// Gets or sets a value that indicates the number of elements in the prevent animation queue.
        /// </summary>
        /// <remarks>Checkbox does not offer any way to turn off the animation. We
        /// have to workaround this, otherwise animation occurs while
        /// scrolling through grid.</remarks>
        public int PreventAnimationQueueCount
        {
            get
            {
                return this.queueLoad;
            }

            set
            {
                this.queueLoad = value;
                this.PreventAnimationWorkaround();
            }
        }

        /// <summary>
        /// Initializes the content of the checkbox cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="checkBox">The check box.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(CheckBox checkBox, GridRenderStyleInfo style)
        {
            // Note: Either check IsInArrage in checkBox_Checked event handler
            // or unwire events to avoid events from being raised when setting up checkbox here.
            // Not sure which is better, it really depends on the actual control. I leave
            // it up to user to unwire/wire or check the IsInArrange property as shown
            // in checkBox_Checked below.
            //base.OnInitializeContent(checkBox, style);//This code was commented out. Because Check box foreground wrongly applied. Actually no need to set CheckBox Foreground Property.
            //Necessary property for CheckBox Set here.

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
          if (style.IsThemed)
              this.checkBoxPaint.GetCheckBoxVisualStyle(checkBox, style);
          checkBox.Padding = margins;
          checkBox.FontFamily = style.Font.FontFamily;
          checkBox.FontSize = style.Font.FontSize;
          checkBox.FontStretch = style.Font.FontStretch;
          checkBox.FontWeight = style.Font.FontWeight;
          checkBox.FontStyle = style.Font.FontStyle;
          checkBox.HorizontalAlignment = style.HorizontalAlignment;
          checkBox.VerticalAlignment = style.VerticalAlignment;
          if (style.ReadOnlyIsThemed)
          {
              var gridVisualStyle = GridVisualStyleHelper.GetVisualStyleOfGrid(checkBox);
              if (gridVisualStyle != string.Empty)
              {
                  SkinStorage.SetVisualStyle(checkBox, gridVisualStyle);
              }
              else
              {
                  SkinStorage.SetVisualStyle(checkBox, "Default");
              }
          }
            
            this.OnUnwireUIElement(checkBox);

            checkBox.BeginInit();
            
            // NOTE: Changing values here might cause the control to animate (e.g. setting CheckBox.IsChecked)
            // There is no way offered by built-in WPF controls to turn off this animation.
            //checkBox.Content = null;           
            // TODO: Setting checkBox.IsChecked will cause animation of the checkbox.
            // Therefore when you scroll through the grid this checkboxes will initially
            // all appear unchecked and only when you stop scrolling they will slowly 
            // animate into the checked state behavior.

            checkBox.IsThreeState = style.IsThreeState;
            checkBox.IsEnabled = style.Enabled;
            bool? value;

            value = (bool?)ValueConvert.ChangeType(ControlValue, typeof(bool?), style.GetCulture(true));
            if (!style.IsThreeState && (value == null || value.ToString() == ""))
            {
                value = false;
            }
            checkBox.IsChecked = value;

            checkBox.EndInit();
            
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                checkBox.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = checkBox.Width;
                double offsetY = 0;
                checkBox.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            }
            else
            {
                checkBox.LayoutTransform = MatrixTransform.Identity;
            }
            this.OnWireUIElement(checkBox);
        }

        /// <summary>
        /// Raises the grid preview mouse move.
        /// </summary>
        /// <param name="rci">The rci.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        public override void RaiseGridPreviewMouseMove(RowColumnIndex rci, MouseEventArgs e)
        {
            if (!AllowKeepAliveOnlyCurrentCell)
            {
                if (GridUtil.GetMouseButton(e) == null)
                {
                    GridControl.DelayedCreateCellUIElements(rci);
                }
            }

            base.RaiseGridPreviewMouseMove(rci, e);
        }
        public override void CreateRendererElement(CheckBox checkBox, GridRenderStyleInfo style)
        {     
            this.OnUnwireUIElement(checkBox);
            if (style.IsThemed)
                this.checkBoxPaint.GetCheckBoxVisualStyle(checkBox, style);
            checkBox.BeginInit();
            // NOTE: Changing values here might cause the control to animate (e.g. setting CheckBox.IsChecked)
            // There is no way offered by built-in WPF controls to turn off this animation.
            //checkBox.Content = null;            
            // TODO: Setting checkBox.IsChecked will cause animation of the checkbox.
            // Therefore when you scroll through the grid this checkboxes will initially
            // all appear unchecked and only when you stop scrolling they will slowly 
            // animate into the checked state behavior.
            checkBox.IsThreeState = style.IsThreeState;
            checkBox.IsEnabled = style.Enabled;
            object value = GetControlText(style);
            bool isBool;
            if ((!style.IsThreeState && (value == null || value.ToString() == "")) || ((value != null && value.ToString() != string.Empty)) && style.IsThreeState && !(bool.TryParse(value.ToString(), out isBool)))
            {
                value = false;
            }
            checkBox.IsChecked = (bool?)ValueConvert.ChangeType(value, typeof(bool?), style.GetCulture(true));
            checkBox.EndInit();            
            this.OnWireUIElement(checkBox);
            // base.CreateRendererElement(checkBox, style); //This code was commented out. Because Check box foreground wrongly applied. Actually no need to set CheckBox Foreground Property.
            //Necessary property for CheckBox Set here.
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

            checkBox.Padding = margins;
            checkBox.FontFamily = style.Font.FontFamily;
            checkBox.FontSize = style.Font.FontSize;
            checkBox.FontStretch = style.Font.FontStretch;
            checkBox.FontWeight = style.Font.FontWeight;
            checkBox.FontStyle = style.Font.FontStyle;
            checkBox.HorizontalAlignment = style.HorizontalAlignment;
            checkBox.VerticalAlignment = style.VerticalAlignment;

            if (style.ReadOnlyIsThemed)
            {
                var gridVisualStyle = GridVisualStyleHelper.GetVisualStyleOfGrid(checkBox);
                if (gridVisualStyle != string.Empty)
                {
                    SkinStorage.SetVisualStyle(checkBox, gridVisualStyle);
                }
                else
                {
                    SkinStorage.SetVisualStyle(checkBox, "Default");
                }
            }
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, CheckBox checkBox, GridRenderStyleInfo style)
        {
            Rect rc = this.checkBoxPaint.DetermineCheckerBounds(aca.CellRect, string.Empty);

            if (style.HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
                rc.X = aca.CellRect.X + 1;
            if (style.HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
                rc.X = aca.CellRect.X + aca.CellRect.Width - 1 -checkBox.ActualWidth;
            if (style.VerticalAlignment == System.Windows.VerticalAlignment.Top)
                rc.Y = aca.CellRect.Y + 1;
            if (style.VerticalAlignment == System.Windows.VerticalAlignment.Bottom)
                rc.Y = aca.CellRect.Y + aca.CellRect.Height - 1 - checkBox.ActualHeight;

            SetBounds(checkBox, rc, aca.ForceMeasure, false);
        }

        protected override CheckBox CreateUIElement(ArrangeCellArgs aca, GridRenderStyleInfo style)
        {
            object value = GetControlText(style);
            bool isBool;
            if ((!style.IsThreeState && (value == null || value.ToString() == "")) || ((value != null && value.ToString() != string.Empty)) && style.IsThreeState && !(bool.TryParse(value.ToString(), out isBool)))
                value = false;

            var bolval = (bool?)ValueConvert.ChangeType(value, typeof(bool?), style.GetCulture(true));
            int checkedState = Array.IndexOf(isCheckedValues, bolval);
            var queue = this.checkboxes[checkedState];
            this.CreateAndEnqueueCheckBox(queue, checkedState);
            var checkbox = queue.Dequeue();
            checkbox.IsThreeState = style.IsThreeState;
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                checkbox.FlowDirection = style.FlowDirection;
                checkbox.LayoutTransform = new ScaleTransform(-1, 1);
            }
            return checkbox;
        }

        protected override object GetControlValueFromEditorCore(CheckBox uiElement)
        {
            return uiElement.IsChecked;
        }

        protected override void OnActivated()
        {
            GridControl.PreviewKeyDown += new KeyEventHandler(this.GridControl_PreviewKeyDown);
            base.OnActivated();
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        public override void RaiseGridCellClick(int rowIndex, int colIndex, Scroll.MouseControllerEventArgs e)
        {
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
               && !CurrentCell.IsEditing)
               //&& GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
            {
                CurrentCell.BeginEdit(true);

            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
            {
                return true;
            }
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (this.CurrentStyle != null)
                        {
                            var renderer = this.CurrentCell.Renderer;
                            if (renderer != null)
                            {
                                GridDataStyleInfo sif = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                                if (sif != null && sif.CellIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell)
                                {
                                    e.Handled = true;
                                }
                                else if (sif != null && sif.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell && !CurrentCell.IsEditing)
                                {
                                    e.Handled = true;
                                }
                            }
                        }
                        if(this.CurrentCell.IsEditing)
                            CurrentCell.EndEdit();
                        CurrentCell.MoveRight();
                        
                        return true;
                        // break; Unreachable code
                    }
                    
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        protected virtual void OnClickedCheckBox(CheckBox checkBox)
        {
            if (IsCurrentCell(checkBox))
            {
                CurrentCell.BeginEdit(true);
                if (CurrentCellUIElement != null)
                    CurrentCellUIElement.Focus();
                object value = GetControlValueFromEditor();
                
                if (!SetControlValue(value))
                {
                    RefreshContent(); // reverses change.
                }
            }
        }

        //protected override void OnEditingComplete()
        //{
        //    this.CurrentCell.EndEdit();
        //    base.OnEditingComplete();
        //}

        //public override void MouseDown(FrameworkElement owner, Scroll.MouseControllerEventArgs e)
        //{
        //    if (this.GridControl.Model.Options.ListBoxSelectionMode == GridSelectionMode.MultiSimple)
        //        {
        //        RowColumnIndex rci = this.GridControl.PointToCellRowColumnIndex(e.Location);
        //        GridRangeInfo newRange = GridRangeInfo.Cells(rci.RowIndex, 0, rci.RowIndex, this.GridControl.Model.ColumnCount - 1);
        //        GridDataControl gdc = this.GridControl.FindParentElementOfType<GridDataControl>();
        //        if (this.GridControl.Model.SelectedRanges.Contains(GridRangeInfo.Table()))
        //            {
        //            this.GridControl.Model.SelectedRanges.Remove(GridRangeInfo.Table());
        //            }
        //        if (this.GridControl.Model.SelectedRanges.Contains(newRange))
        //            {
                    
        //            this.GridControl.Model.SelectedRanges.Remove(newRange);
        //            if (gdc != null && gdc.SelectedItem!=null)
        //                {
        //                gdc.SelectedItems.Remove(gdc.SelectedItem);
        //                }
        //            }
        //        else
        //            {
        //            this.GridControl.Model.SelectedRanges.Add(newRange);
        //            if (gdc != null && gdc.SelectedItem != null)
        //                {
        //                gdc.SelectedItems.Add(gdc.SelectedItem);
        //                }
        //            }
        //        this.CurrentCell.ConfirmChanges();
        //        this.GridControl.InvalidateCell(newRange);
        //        }
        //    base.MouseDown(owner, e);
        //    }

 
        protected override void OnDeactivated()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
            GridControl.PreviewKeyDown -= new KeyEventHandler(this.GridControl_PreviewKeyDown);
            base.OnDeactivated();
        }

        protected override string GetControlTextFromEditorCore(CheckBox uiElement)
        {
            if (HasCurrentCellState && this.CurrentCellUIElement != null && this.CurrentCellUIElement != null)
            {
                return this.CurrentCellUIElement.IsChecked.ToString();
            }
            return base.GetControlTextFromEditorCore(uiElement);
        }

        protected override object GetControlValueFromEditor()
        {
            if (HasCurrentCellState && this.CurrentCellUIElement != null && this.CurrentCellUIElement != null)
            {
                return this.CurrentCellUIElement.IsChecked;
            }
            return base.GetControlValueFromEditor();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ControlValue = GetControlValueFromEditor();
        }

        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
                return;
            // Will only get hit if SupportsRenderOptimization is true ...
            Rect textRectangle = rca.CellRect;
            string text = "Checkbox";
            // Draw the formatted text string to the DrawingContext of the control.
            bool value;
            bool.TryParse(((style.CellValue) != null ? style.CellValue.ToString() : null), out value);
            var val = (style.CellValue != null && !String.IsNullOrEmpty(style.CellValue.ToString())) ? value : (bool?)null;
            if (!style.IsThreeState && val == null)
                this.checkBoxPaint.DrawCheckBox(dc, textRectangle, text, false, style, AllowRecycle);
            else
                this.checkBoxPaint.DrawCheckBox(dc, textRectangle, text, val, style, AllowRecycle);
            base.OnRender(dc, rca, style);
        }

        protected override void OnRenderForPrinting(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo cellInfo)
        {
            var style = cellInfo;
            // Will only get hit if SupportsRenderOptimization is true ...
            Rect textRectangle = rca.CellRect;
            string text = "Checkbox";
            // Draw the formatted text string to the DrawingContext of the control.
            bool value;
            bool.TryParse(((style.CellValue) != null ? style.CellValue.ToString() : null), out value);
            var val = (style.CellValue != null && !String.IsNullOrEmpty(style.CellValue.ToString())) ? value : (bool?)null;
            if (!style.IsThreeState && val == null)
                this.checkBoxPaint.DrawCheckBox(dc, textRectangle, text, false, style, AllowRecycle);
            else
                this.checkBoxPaint.DrawCheckBox(dc, textRectangle, text, val, style, AllowRecycle);
            base.OnRender(dc, rca, style);
        }

        /// <summary>
        /// Unwire previously wired events from checkBox. 
        /// </summary>
        /// <param name="checkBox"></param>
        protected override void OnUnwireUIElement(CheckBox checkBox)
        {
            checkBox.Indeterminate -= new RoutedEventHandler(this.CheckBox_Indeterminate);
            checkBox.Checked -= new RoutedEventHandler(this.CheckBox_Checked);
            checkBox.Unchecked -= new RoutedEventHandler(this.CheckBox_Unchecked);
        }

        /// <summary>
        /// Wire events from checkBox
        /// </summary>
        /// <param name="checkBox"></param>  
        protected override void OnWireUIElement(CheckBox checkBox)
        {
            checkBox.Checked += new RoutedEventHandler(this.CheckBox_Checked);
            checkBox.Unchecked += new RoutedEventHandler(this.CheckBox_Unchecked);
            checkBox.Indeterminate += new RoutedEventHandler(this.CheckBox_Indeterminate);
        }

        private void PreventAnimationWorkaround()
        {
            AllowRecycle = false;
            for (int checkedState = 0; checkedState < 3; checkedState++)
            {
                Queue<CheckBox> queue = new Queue<CheckBox>();
                this.checkboxes[checkedState] = queue;

                while (queue.Count < this.queueLoad)
                {
                    this.CreateAndEnqueueCheckBox(queue, checkedState);
                }
            }
            AllowRecycle = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (!this.IsInArrange)
            {
                this.OnClickedCheckBox(checkBox);
            }
        }

        private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (!this.IsInArrange)
            {
                this.OnClickedCheckBox(checkBox);
            }
        }

        private void GridControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!this.CurrentCell.IsEditing)
                {
                    CurrentCell.BeginEdit(true);
                    CurrentCell.ScrollInView();
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (!this.IsInArrange)
            {
                this.OnClickedCheckBox(checkBox);
            }
        }

        private void CreateAndEnqueueCheckBox(Queue<CheckBox> queue, int checkedState)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.BeginInit();
            checkBox.IsChecked = isCheckedValues[checkedState];
            Rect rc = new Rect(new Point(0, 0), this.checkBoxPaint.CheckBoxSize);
            checkBox.Arrange(rc);
            checkBox.EndInit();
            queue.Enqueue(checkBox);
        }
    }

    /// <summary>
    /// Draws checkbox control control in the grid cells.
    /// </summary>
    public class CheckBoxPaint : TextBoxPaint
    {
        private static bool?[] isCheckedValues = new bool?[] { null, true, false };
        private Dictionary<int, VisualBrush> brushes = new Dictionary<int, VisualBrush>();
        private Size checkBoxSize = new Size(15, 15);
        
        // TODO: add support for checkbox content/description.
        private CheckerPlacement checkerPlacement;
        private bool isRound;
        private Thickness margins;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="checkerPlacement"><see cref="CheckerPlacement"/> object that indicates whether the checkbox should appear before or after the cell text.</param>
        /// <param name="isRound">Not implemented.</param>
        /// <param name="margins">Border margins.</param>
        /// <param name="typeface">The <see cref="Typeface"/> object.</param>
        /// <param name="emSize">Font size.</param>
        /// <param name="foreGround">Foreground brush.</param>
        public CheckBoxPaint(CheckerPlacement checkerPlacement, bool isRound, Thickness margins, Typeface typeface, double emSize, Brush foreGround)
            : base(typeface, emSize, foreGround)
        {
            this.checkerPlacement = checkerPlacement;
            this.isRound = isRound;
            this.margins = margins;
        }

        /// <summary>
        /// Gets or sets the size of the checkbox control. 
        /// </summary>
        public Size CheckBoxSize
        {
            get
            {
                return this.checkBoxSize;
            }

            set
            {
                this.checkBoxSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CheckerPlacement"/> for the cell.
        /// </summary>
        public CheckerPlacement CheckerPlacement
        {
            get
            {
                return this.checkerPlacement;
            }

            set
            {
                this.checkerPlacement = value;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public bool IsRound
        {
            get
            {
                return this.isRound;
            }

            set
            {
                this.isRound = value;
            }
        }

        /// <summary>
        /// Border margins.
        /// </summary>
        public Thickness Margins
        {
            get
            {
                return this.margins;
            }

            set
            {
                this.margins = value;
            }
        }

        /// <summary>
        /// Returns the checkbox bounds.
        /// </summary>
        /// <param name="clientRect">The client rectangle.</param>
        /// <param name="text">The cell text.</param>
        /// <returns></returns>
        public Rect DetermineCheckerBounds(Rect clientRect, string text)
        {
            return CenterInRect(clientRect, this.checkBoxSize);
        }

        public void DrawCheckBox(DrawingContext dc, Rect clientRect, string text, bool? isChecked,GridRenderStyleInfo style)
        {
            this.DrawCheckBox(dc, clientRect, text, isChecked, style, true);
        }
        
        /// <summary>
        /// Draws the checkbox.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="clientRect">The cell client rectangle.</param>
        /// <param name="text">The cell text.</param>
        /// <param name="isChecked">If true, the checkbox will be drawn as checked; if false it will be drawn as unchecked.</param>
        public void DrawCheckBox(DrawingContext dc, Rect clientRect, string text, bool? isChecked, GridRenderStyleInfo style, bool AllowRecycle)
        {
            Rect rc = this.DetermineCheckerBounds(clientRect, text);
            VisualBrush vb = this.GetVisualBrush(isChecked, style, AllowRecycle);
            if (style.HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
                rc.X = clientRect.X + 1;
            if (style.HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
                rc.X = clientRect.X + clientRect.Width - rc.Width - 1;
            if (style.VerticalAlignment == System.Windows.VerticalAlignment.Top)
                rc.Y = clientRect.Y + 1;
            if (style.VerticalAlignment == System.Windows.VerticalAlignment.Bottom)
                rc.Y = clientRect.Y + clientRect.Height - rc.Height - 1;

            dc.DrawRectangle(vb, null, rc);
        }

        private VisualBrush GetVisualBrush(bool? isChecked, GridStyleInfo style)
        {
            return this.GetVisualBrush(isChecked, style, true);
        }

        private VisualBrush GetVisualBrush(bool? isChecked, GridStyleInfo style, bool AllowRecycle)
        {
            int checkedState = Array.IndexOf(isCheckedValues, isChecked);
            if (this.brushes.ContainsKey(checkedState) && AllowRecycle)
            {
                return this.brushes[checkedState];
            }

            bool wasAnimated = GridUtil.IsAnimated;
            try
            {
                GridUtil.IsAnimated = false;

                VisualBrush visualBrush;
                CheckBox b = new CheckBox();
                b.BeginInit();
                b.Content = null;
                b.Width = this.checkBoxSize.Width;
                b.Height = this.checkBoxSize.Height;
                b.IsChecked = isChecked;
                b.IsEnabled = style.Enabled;
                b.Background = Brushes.Transparent;
                if (style.FlowDirection == System.Windows.FlowDirection.RightToLeft)
                {
                    b.FlowDirection = style.FlowDirection;
                    b.LayoutTransform = new ScaleTransform(-1, 1);
                }
                b.EndInit();
                b.Measure(this.checkBoxSize);
                b.Arrange(new Rect(this.checkBoxSize));
                if (style.IsThemed)
                    GetCheckBoxVisualStyle(b, style);
                visualBrush = new VisualBrush();
                visualBrush.Visual = b;
                this.brushes[checkedState] = visualBrush;
                return visualBrush;
            }
            finally
            {
                GridUtil.IsAnimated = wasAnimated;
            }
        }
        /// <summary>
        /// Returns the checkbox VisualStyle.
        /// </summary>
        public CheckBox GetCheckBoxVisualStyle(CheckBox checkbox, GridStyleInfo style)
        {
            var gridTableModel = style.GridModel as GridDataTableModel;
            var treeModel = style.GridModel as GridTreeModel;
            if (gridTableModel != null && gridTableModel.TableProperties.EnableVisualStyleForEditors && !gridTableModel.TableProperties.IsLegacyStyleEnabled)
            {
                gridTableModel.GetVisualStyleDictionary(checkbox);
            }

            if (treeModel != null)
            {
                treeModel.GetVisualStyleDictionary(checkbox);
            }
            return checkbox;
        }
    }
}
