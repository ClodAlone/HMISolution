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
    using System.Diagnostics;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    public class GridDataExpandCollapseContentCellRenderer : GridVirtualizingCellRenderer<GridDataExpandCellControl>
    {
        public GridDataExpandCollapseContentCellRenderer()
        {
            this.previousValue = null;
        }

        private bool? previousValue;
        protected override void ArrangeUIElement(ArrangeCellArgs aca, GridDataExpandCellControl image, GridRenderStyleInfo style)
        {
            base.ArrangeUIElement(aca, image, style);
            VisualContainer.SetWantsMouseInput(image, true);

            var cellValue = (bool)style.CellValue;
            bool isChanged = false;
            this.UpdateCurrentVisualStyle(out isChanged);
            if (this.previousValue != cellValue || isChanged)
            {
                this.previousValue = cellValue;
            }

            bool? value = style.CellValue as bool?;
            Brush background = this.TableModel.GetPlusMinusButtonBackgroundBrush();
            Brush foreground = this.TableModel.GetPlusMinusButtonForeground();
            Brush borderBrush = this.TableModel.GetPlusMinusButtonBorderBrush();
            if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
            {
                if (cellValue)
                {
                    background = this.TableModel.GetPlusMinusExpandedButtonBackground();
                    foreground = this.TableModel.GetPlusMinusExpandedButtonForeground();
                    borderBrush = this.TableModel.GetPlusMinusExpandedButtonBorderBrush();
                }
                /*Since the properties are changed as Obsolete below codes are commented.
                if (this.GridControl.CurrentCell.RowIndex == style.RowIndex && style.ModelStyle.CellIdentity is GridDataTableStyleInfoIdentity
                    && (style.ModelStyle.CellIdentity as GridDataTableStyleInfoIdentity).TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell)
                {
                    background = this.TableModel.GetPlusMinusCaptionSelectedButtonBackground();
                    foreground = this.TableModel.GetPlusMinusCaptionSelectedButtonForeground();
                    borderBrush = this.TableModel.GetPlusMinusCaptionSelectedButtonBorderBrush();
                }*/
            }

            if (this.GridControl is GridDataControlBaseImpl && (this.GridControl as GridDataControlBaseImpl).StyleManager != null
                   && (this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence != null)
            {
                if ((this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.PlusPath != null)
                {
                    var pathHeight = (this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.PlusPath.ActualHeight;
                    var pathWidth = (this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.PlusPath.ActualWidth;
                    WriteableBitmap bmp1 = new WriteableBitmap((this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.PlusPath, new TranslateTransform() { X = 0, Y = 0 });
                    Image img1 = new Image();
                    img1.Source = bmp1;
                    img1.Width = pathWidth > 0 ? pathWidth : 17;
                    img1.Height = pathHeight > 0 ? pathHeight : 17;
                    if (image.plusPath.Children.Count == 0)
                        image.plusPath.Children.Add(img1);
                }
                if ((this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.MinusPath != null)
                {
                    var pathHeight = (this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.MinusPath.ActualHeight;
                    var pathWidth = (this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.MinusPath.ActualWidth;
                    WriteableBitmap bmp = new WriteableBitmap((this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.MinusPath, new TranslateTransform() { X = 0, Y = 0 });
                    Image img = new Image();
                    img.Source = bmp;
                    img.Width = pathWidth > 0 ? pathWidth : 17;
                    img.Height = pathHeight > 0 ? pathHeight : 17;
                    if (image.minusPath.Children.Count == 0)
                        image.minusPath.Children.Add(img);
                }

                if (!cellValue)
                    VisualStateManager.GoToState(image, "VisualState4", true);
                else if ((this.GridControl as GridDataControlBaseImpl).StyleManager.ExpanderAppearence.MinusPath != null)
                    VisualStateManager.GoToState(image, "VisualState5", true);
            }
            else
            {

                if (this.TableModel.TableProperties.IsLegacyStyleEnabled)
                {
                    if (value == true)
                        VisualStateManager.GoToState(image, "VisualState1", true);
                    else
                        VisualStateManager.GoToState(image, "VisualState", true);

                    image.PlusMinusBackground = foreground;
                    image.PlusMinusBorderBrush = borderBrush;
                    image.RectangleBackground = background;

                    if (image.rectangle != null)
                    {
                        image.rectangle.RadiusX = 3;
                        image.rectangle.RadiusY = 3;
                    }
                }
                else
                {
                    if (image.rectangle != null)
                    {
                        image.rectangle.RadiusX = 0;
                        image.rectangle.RadiusY = 0;
                    }
                    if (this.currentVisualStyle.Value == VisualStyle.Office2007Blue || this.currentVisualStyle.Value == VisualStyle.Office2007Black || this.currentVisualStyle.Value == VisualStyle.Office2007Silver || this.currentVisualStyle.Value == VisualStyle.GlassyGreen)
                    {
                        if (value == true)
                            VisualStateManager.GoToState(image, "VisualState1", true);
                        else
                            VisualStateManager.GoToState(image, "VisualState", true);

                        image.PlusMinusBackground = this.TableModel.GetPlusMinusButtonForeground();
                        image.PlusMinusBorderBrush = this.TableModel.GetPlusMinusButtonBorderBrush();
                        image.RectangleBackground = this.TableModel.GetPlusMinusButtonBackgroundBrush();
                    }
                    else
                    {
                        if (value == true)
                            VisualStateManager.GoToState(image, "VisualState3", true);
                        else
                            VisualStateManager.GoToState(image, "VisualState2", true);
                        
                        image.PlusMinusBackground = background;
                        image.PlusMinusBorderBrush = borderBrush;
                        //image.RectangleBackground = this.TableModel.GridVisualStyle.PlusMinusButtonBackground;
                    }
                }
            }
        }

        public override void OnInitializeContent(GridDataExpandCellControl uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
        }


        public GridDataTableModel TableModel
        {
            get
            {
                var gridDataTableModel = this.GridControl.Model as GridDataTableModel;
                return gridDataTableModel;
            }
        }

        private VisualStyle? currentVisualStyle = null;
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
    }
}
