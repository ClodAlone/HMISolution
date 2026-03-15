#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Controls;
    using System;
    using Syncfusion.Windows.Shared;
    using System.Diagnostics;
    using System.Windows.Data;
    using System.Windows.Shapes;

    public class GridDataExpandCollapseVisualCellRenderer : GridCellRendererBase
    {
        public GridDataExpandCollapseVisualCellRenderer()
        {
        }

        private Thickness expandCollapseMargin = new Thickness(4);

        /// <summary>
        /// Gets or sets the uniform margin in pixels that appears around the Expand/Collapse glyph.
        /// </summary>
        /// <remarks>
        /// You may need to change this value from its default of 4 to something smaller if you change the 
        /// default row height to smaller values.
        /// </remarks>
        public Thickness ExpandCollapseMargin
        {
            get { return expandCollapseMargin; }
            set { expandCollapseMargin = value; }
        }

        private VisualBrush visualBrush = null;
        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            var cellValue = (bool)style.CellValue;
            var backgroundEl = this.GetChildContent(cellValue);

            Thickness tempMargin = ExpandCollapseMargin;
            if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
            {
                ApplyBrush(style, cellValue);
                if (this.currentVisualStyle != null)
                {
                    if ((tempMargin.ToString() == "4,4,4,4") && !(this.currentVisualStyle.Value == VisualStyle.Office2007Blue || this.currentVisualStyle.Value == VisualStyle.Office2007Black || this.currentVisualStyle.Value == VisualStyle.Office2007Silver ||
                        this.currentVisualStyle.Value == VisualStyle.GlassyGreen))
                    {
                        tempMargin = new Thickness(4,5,4,5); //Margin is changed from 7 to 4,5,4,5 due to Expander Button Alignment
                    }
                }
            }

            Thickness margins = style.TextMargins.ToThickness();
            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
            margins.Left = tempMargin.Left;// Math.Max(margins.Left, 2);
            margins.Right = tempMargin.Right;// Math.Max(margins.Right, 2);
            margins.Top = tempMargin.Top;
            margins.Bottom = tempMargin.Bottom;
            textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
            //var textRectangle = rca.CellRect;
            if (!(style.CellValue is bool))
            {
                return;
            }

            if (this.TableModel.TableProperties.IsLegacyStyleEnabled)
            {
                if (this.currentVisualStyle != null)
                {
                    if (this.currentVisualStyle.Value == VisualStyle.Office14Blue ||
                        this.currentVisualStyle.Value == VisualStyle.Office14Black ||
                        this.currentVisualStyle.Value == VisualStyle.Office14Silver)
                    {
                        textRectangle.Width = textRectangle.Height = 10;
                    }
                    else
                    {
                        textRectangle.Width = textRectangle.Height = 15;
                    }
                }
            }
            else
            {
                var grid = this.GridControl.FindParentElementOfType<GridDataControl>();
                bool hasPlusPath = grid != null && grid.StyleManager != null && grid.StyleManager.ExpanderAppearence != null && grid.StyleManager.ExpanderAppearence.PlusPath != null;
                bool hasMinusPath = grid != null && grid.StyleManager != null && grid.StyleManager.ExpanderAppearence != null && grid.StyleManager.ExpanderAppearence.MinusPath != null;
                if(hasMinusPath || hasMinusPath)
                {
                    if (hasPlusPath && !cellValue)
                    {
                        textRectangle.Width = grid.StyleManager.ExpanderAppearence.PlusPath.ActualWidth > 0 ? grid.StyleManager.ExpanderAppearence.PlusPath.ActualWidth : 15;
                        textRectangle.Height = grid.StyleManager.ExpanderAppearence.PlusPath.ActualHeight > 0 ? grid.StyleManager.ExpanderAppearence.PlusPath.ActualHeight : 15;
                    }

                    if (hasMinusPath && cellValue)
                    {
                        textRectangle.Width = grid.StyleManager.ExpanderAppearence.MinusPath.ActualWidth > 0 ? grid.StyleManager.ExpanderAppearence.MinusPath.ActualWidth : 15;
                        textRectangle.Height = grid.StyleManager.ExpanderAppearence.MinusPath.ActualHeight > 0 ? grid.StyleManager.ExpanderAppearence.MinusPath.ActualHeight : 15;
                    }
                }
                else if (this.currentVisualStyle != null)
                {
                    if (this.currentVisualStyle.Value == VisualStyle.Office2007Blue ||
                        this.currentVisualStyle.Value == VisualStyle.Office2007Black ||
                        this.currentVisualStyle.Value == VisualStyle.Office2007Silver ||
                        this.currentVisualStyle.Value == VisualStyle.GlassyGreen)
                    {

                        textRectangle.Width = textRectangle.Height = 13;
                    }
                    else
                    {
                        //Measurements are changed due to Expander Button Alignment.
                        if (!cellValue)
                        {
                            textRectangle.Width = 11;
                            textRectangle.Height = 11;
                        }
                        else
                        {
                            textRectangle.Width = 9;
                            textRectangle.Height = 10;
                        }

                    }
                }
            }
            if (this.IsChanged)
            {
                var border = this.GetContent(textRectangle, backgroundEl);
                this.visualBrush = new VisualBrush(border);
                if (this.GridControl is GridCellNestedGridEditor)
                {
                    var nestedGrid = this.GridControl as GridCellNestedGridEditor;
                    GridDataControlBaseImpl parentGrid = nestedGrid.ParentGrid as GridDataControlBaseImpl;
                    if (parentGrid != null && parentGrid.StyleManager != null && parentGrid.StyleManager.ExpanderAppearence != null)
                    {
                        if (!cellValue)
                        {
                            if (parentGrid.StyleManager.ExpanderAppearence.PlusPath != null)
                                this.visualBrush = new VisualBrush(parentGrid.StyleManager.ExpanderAppearence.PlusPath);
                        }
                        else if (parentGrid.StyleManager.ExpanderAppearence.MinusPath != null)
                        {
                            this.visualBrush = new VisualBrush(parentGrid.StyleManager.ExpanderAppearence.MinusPath);
                        }
                    }
                }

                if (this.GridControl is GridDataControlBaseImpl && (this.GridControl as GridDataControlBaseImpl).StyleManager != null
                   && (this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence != null
                   && (this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.PlusPath != null)
                {
                    if (!cellValue)
                        this.visualBrush = new VisualBrush((this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.PlusPath);
                    else if ((this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.MinusPath != null)
                        this.visualBrush = new VisualBrush((this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.MinusPath);
                }
            }

            textRectangle.Y += (rca.CellRect.Height / 2) - textRectangle.Height;

            dc.DrawRectangle(this.visualBrush, null, textRectangle);
        }

        #region Theme Brushes

        private Grid currentBrush = GridDataResourceWrapper.PlusGrid;

        private VisualStyle? currentVisualStyle = null;

        private bool? currentValue = null;

        private bool IsChanged
        {
            get;
            set;
        }

        private Grid GetChildContent(bool opened)
        {
            bool isChanged = false;
            this.UpdateCurrentVisualStyle(out isChanged);
            isChanged = true;
            if (!this.currentValue.HasValue)
            {
                this.currentValue = opened;
                isChanged = true;
            }
            else
            {
                if (this.currentValue.Value != opened)
                {
                    this.currentValue = opened;
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                if (this.TableModel.TableProperties.IsLegacyStyleEnabled)
                {
                    if (this.currentVisualStyle.Value == VisualStyle.Office14Blue ||
                        this.currentVisualStyle.Value == VisualStyle.Office14Black ||
                        this.currentVisualStyle.Value == VisualStyle.Office14Silver)
                    {
                        if (!opened)
                        {
                            this.currentBrush = GridDataResourceWrapper.LegacyPlusOff14;
                        }
                        else
                        {
                            this.currentBrush = GridDataResourceWrapper.LegacyMinusOff14;

                            var borderBackgroundPath = this.currentBrush.Children[1] as Path;
                            borderBackgroundPath.Fill = this.TableModel.GetPlusMinusButtonBackgroundBrush();
                            borderBackgroundPath.Stroke = this.TableModel.GetPlusMinusButtonBorderBrush();
                        }


                    }
                    else
                    {
                        if (!opened)
                        {
                            this.currentBrush = GridDataResourceWrapper.LegacyPlusGrid;
                        }
                        else
                        {
                            this.currentBrush = GridDataResourceWrapper.LegacyMinusGrid;
                        }

                        var borderPath = this.currentBrush.Children[0] as Path;
                        borderPath.Fill = this.TableModel.GetPlusMinusButtonBorderBrush();

                        var borderBackgroundPath = this.currentBrush.Children[1] as Path;
                        borderBackgroundPath.Fill = this.TableModel.GetPlusMinusButtonBackgroundBrush();

                        var foregroundPath = this.currentBrush.Children[3] as Path;
                        foregroundPath.Fill = this.TableModel.GetPlusMinusButtonForeground();
                        //border.DataContext = this.TableModel.GridVisualStyle.PlusMinusButtonBackground;
                        //border.BorderBrush = this.TableModel.GridVisualStyle.PlusMinusButtonBorderBrush;
                        //border.Tag = this.TableModel.GridVisualStyle.PlusMinusButtonForeground;
                    }
                }
                else
                {

                    if (this.currentVisualStyle.Value == VisualStyle.GlassyGreen ||
                        this.currentVisualStyle.Value == VisualStyle.Office2007Blue ||
                        this.currentVisualStyle.Value == VisualStyle.Office2007Black ||
                        this.currentVisualStyle.Value == VisualStyle.Office2007Silver)
                    {

                        if (!opened)
                        {
                            this.currentBrush = GridDataResourceWrapper.PlusGrid;
                            (this.currentBrush.Children[0] as Path).Fill = this.TableModel.GetPlusMinusButtonBackgroundBrush();
                            (this.currentBrush.Children[0] as Path).Stroke = this.TableModel.GetPlusMinusButtonBorderBrush();
                        }
                        else
                        {
                            this.currentBrush = GridDataResourceWrapper.MinusGrid;
                            (this.currentBrush.Children[0] as Path).Fill = this.TableModel.GetPlusMinusButtonBackgroundBrush();
                            (this.currentBrush.Children[0] as Path).Stroke = this.TableModel.GetPlusMinusButtonBorderBrush();
                        }
                    }
                    else
                    {
                        if (!opened)
                        {
                            this.currentBrush = GridDataResourceWrapper.PlusOff14;
                            (this.currentBrush.Children[0] as Path).Fill = this.TableModel.GetPlusMinusButtonBackgroundBrush();
                            (this.currentBrush.Children[0] as Path).Stroke = this.TableModel.GetPlusMinusButtonBorderBrush();
                        }
                        else
                        {
                            this.currentBrush = GridDataResourceWrapper.MinusOff14;
                            (this.currentBrush.Children[0] as Path).Fill = this.TableModel.GetPlusMinusButtonBackgroundBrush();
                            (this.currentBrush.Children[0] as Path).Stroke = this.TableModel.GetPlusMinusButtonBorderBrush();
                        }
                    }
                }
            }

            this.IsChanged = isChanged;
            return this.currentBrush;
        }

        private void ApplyBrush(GridRenderStyleInfo style, bool opened)
        {
            Brush background = this.TableModel.GetPlusMinusButtonBackgroundBrush();
            Brush foreground = this.TableModel.GetPlusMinusButtonForeground();
            Brush borderBrush = this.TableModel.GetPlusMinusButtonBorderBrush();
            if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
            {
                if (opened)
                {                    
                        background = this.TableModel.GetPlusMinusExpandedButtonBackground();
                        foreground = this.TableModel.GetPlusMinusExpandedButtonForeground();
                        borderBrush = this.TableModel.GetPlusMinusExpandedButtonBorderBrush();                    
                }

                /* Since the properties are changed as Obsolete below codes are commented.
                if (this.GridControl.CurrentCell.RowIndex == style.RowIndex && style.ModelStyle.CellIdentity is GridDataTableStyleInfoIdentity
                    && (style.ModelStyle.CellIdentity as GridDataTableStyleInfoIdentity).TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell)
                {
                    
                    if (!(this.currentVisualStyle.Value == VisualStyle.GlassyGreen ||
                           this.currentVisualStyle.Value == VisualStyle.Office2007Blue ||
                           this.currentVisualStyle.Value == VisualStyle.Office2007Black ||
                           this.currentVisualStyle.Value == VisualStyle.Office2007Silver))
                    {
                        background = this.TableModel.GetPlusMinusCaptionSelectedButtonBackground();
                        foreground = this.TableModel.GetPlusMinusCaptionSelectedButtonForeground();
                        borderBrush = this.TableModel.GetPlusMinusCaptionSelectedButtonBorderBrush();
                    }
                }
                 */

                if (this.GridControl.rowColumnUnderMouse.RowIndex == style.RowIndex && this.GridControl.rowColumnUnderMouse.ColumnIndex == style.ColumnIndex
                    && this.GridControl.CurrentCell.RowIndex != style.RowIndex)
                {
                    background = this.TableModel.GetPlusMinusHoverButtonBackground();
                    foreground = this.TableModel.GetPlusMinusHoverButtonForeground();
                    borderBrush = this.TableModel.GetPlusMinusHoverButtonBorderBrush();
                }
            }

            if (this.currentVisualStyle.Value == VisualStyle.GlassyGreen ||
                   this.currentVisualStyle.Value == VisualStyle.Office2007Blue ||
                   this.currentVisualStyle.Value == VisualStyle.Office2007Black ||
                   this.currentVisualStyle.Value == VisualStyle.Office2007Silver)
            {

                var rectanglePath = this.currentBrush.Children[0] as Path;
                var plusminusPath = this.currentBrush.Children[1] as Path;

                rectanglePath.Fill = background;
                rectanglePath.Stroke = borderBrush;
                plusminusPath.Fill = foreground;

            }
            else
            {
                if (!opened)
                {
                    var borderBackgroundPath = this.currentBrush.Children[0] as Path;
                    borderBackgroundPath.Fill = background;
                    borderBackgroundPath.Stroke = borderBrush;
                }
                else
                {
                    var borderBackgroundPath = this.currentBrush.Children[0] as Path;
                    borderBackgroundPath.Fill = background;
                    borderBackgroundPath.Stroke = borderBrush;
                }
            }
        }

        private void UpdateCurrentVisualStyle(out bool isChanged)
        {
            if (!this.currentVisualStyle.HasValue)
            {
                this.currentVisualStyle = this.TableModel.TableProperties.VisualStyle;
                isChanged = true;
                return;
            }
            else
            {
                if (this.currentVisualStyle.Value != this.TableModel.TableProperties.VisualStyle)
                {
                    this.currentVisualStyle = this.TableModel.TableProperties.VisualStyle;
                    isChanged = true;
                    return;
                }
            }

            isChanged = false;
        }

        #endregion

        public GridDataTableModel TableModel
        {
            get
            {
                var gridDataTableModel = this.GridControl.Model as GridDataTableModel;
                return gridDataTableModel;
            }
        }

        private Border GetContent(Rect rect, Grid child)
        {
            var border = new Border();
            border.MaxWidth = border.Width = border.MinWidth = rect.Width;
            border.MaxHeight = border.Height = border.MinHeight = rect.Height;
            border.Measure(new Size(rect.Width, rect.Height));
            VisualContainer.SetWantsMouseInput(border, false);
            //var innerBorder = new Border();
            //innerBorder.Background = brush;//this.GetBrush(opened);
            ////innerBorder.Margin = new Thickness(5, 2, 5, 0);
            //border.Child = innerBorder;
            //border.Background = brush;
            border.Child = child;
            border.BorderThickness = new Thickness(0);
            border.HorizontalAlignment = HorizontalAlignment.Center;
            border.VerticalAlignment = VerticalAlignment.Center;
            border.Measure(new Size(rect.Width, rect.Height));
            border.Arrange(rect);

            if (rect.Width > 0 && rect.Height > 0)
            {
                child.Measure(new Size(rect.Width, rect.Height - 5));
            }

            return border;
        }
    }

    [ValueConversion(typeof(double), typeof(int))]
    internal class GridDataObjectToBrushConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Brush)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /*    public class GridDataExpandCollapseContentCellRenderer : GridVirtualizingCellRenderer<Border>
        {
            public GridDataExpandCollapseContentCellRenderer()
            {
                this.IsFocusable = true;
                this.AllowRecycle = false;
            }

            public override void OnInitializeContent(Border uiElement, GridRenderStyleInfo style)
            {
                base.OnInitializeContent(uiElement, style);
                if (!(style.CellValue is bool))
                {
                    return;
                }
                VisualContainer.SetWantsMouseInput(uiElement, false);
                bool opened = (bool)style.CellValue;
                var innerBorder = new Border();
                if (!opened)
                {
                    innerBorder.Style = GridDataResourceWrapper.BorderPlusStyle;
                }
                else
                {
                    innerBorder.Style = GridDataResourceWrapper.BorderMinusStyle;
                }
                uiElement.Child = innerBorder;
                uiElement.Padding = new Thickness(4, 3, 4, 3);
                uiElement.BorderThickness = new Thickness(0);
            }

            protected override void OnElementMeasured(UIElement el, Size size)
            {
                var border = el as Border;
                var style = GridControlBase.GetRenderStyleInfo(border);
                if (style.ModelStyle is GridDataStyleInfo)
                {
                    var grid = border.FindParentElementOfType<GridDataControl>();
                    if (grid != null)
                    {
                        var visualStyle = SkinStorage.GetVisualStyle(grid);
                        SkinStorage.SetVisualStyle(border, visualStyle);
                    }
                }
            }
        }*/

    /// <summary>
    /// Implements the Expand / Collapse path logic for the Grid Data Control.
    /// </summary>
    public class GridDataCellExpandRenderer : GridCellRendererBase
    {
        public GridDataCellExpandRenderer()
        {
        }

        /// <summary>
        /// Draws the Expand / Collapse glyph in the DrawingContext.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="rca">The rca.</param>
        /// <param name="style">The style.</param>
        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            base.OnRender(dc, rca, style);
            if (!(style.CellValue is bool))
            {
                return;
            }

            bool opened = (bool)style.CellValue;
            double xoffSet = rca.CellRect.Width / 4;
            int yoffSet = 3;
            int w = 5;
            int h = 5;
            Point pt0 = new Point(rca.CellRect.Left + xoffSet, rca.CellRect.Top + yoffSet);
            PathGeometry pg = new PathGeometry();
            PathFigure pfRight = new PathFigure();
            pfRight.StartPoint = new Point(pt0.X, pt0.Y);
            pfRight.IsClosed = false;
            pfRight.IsFilled = false;
            PolyLineSegment pls = new PolyLineSegment(
                new Point[] 
                { 
                    new Point(pt0.X + 2 * w, pt0.Y), 
                    new Point(pt0.X + 2 * w, pt0.Y + 2 * h), 
                    new Point(pt0.X, pt0.Y + 2 * h), 
                    new Point(pt0.X, pt0.Y) 
                },
                true);
            pfRight.Segments.Add(pls);

            pls = new PolyLineSegment(
                new Point[] 
                { 
                    new Point(pt0.X, pt0.Y), 
                    new Point(pt0.X + .5 * w, pt0.Y + h) 
                },
                false);
            pfRight.Segments.Add(pls);

            pls = new PolyLineSegment(
                new Point[] 
                { 
                    new Point(pt0.X + .5 * w, pt0.Y + h), 
                    new Point(pt0.X + 1.5 * w, pt0.Y + h) 
                },
                true);
            pfRight.Segments.Add(pls);

            if (!opened)
            {
                pls = new PolyLineSegment(
                    new Point[] 
                    { 
                        new Point(pt0.X + 1.5 * w, pt0.Y + h), 
                        new Point(pt0.X + 1 * w, pt0.Y + .5 * h) 
                    },
                    false);
                pfRight.Segments.Add(pls);

                pls = new PolyLineSegment(
                    new Point[] 
                    { 
                        new Point(pt0.X + 1 * w, pt0.Y + .5 * h), 
                        new Point(pt0.X + 1 * w, pt0.Y + 1.5 * h) 
                    },
                    true);
                pfRight.Segments.Add(pls);
            }

            pg.Figures.Add(pfRight);
            dc.DrawGeometry(null, new Pen(Brushes.Black, 0.45d), pg);
        }
    }
}
