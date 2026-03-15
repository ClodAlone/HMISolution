//-------------------------------------------------------------------------------------------------
// <copyright file="VisualStylesDrawing.cs" company="Syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using System.Drawing.Drawing2D;   
    using System.Reflection;
    using Syncfusion.Windows.Forms.Grid.Grouping; 
    using Syncfusion.ComponentModel;
    using Syncfusion.Windows.Forms;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Drawing;

    /// <summary>
    /// Defines a list of in-built skins supported by grid.
    /// </summary>
    public class GridSkins
    {
        /// <summary>
        /// Intializes a new <see cref="GridSkins"/>
        /// </summary>
        public GridSkins()
            : base()
        {
        }

        /// <summary>
        /// Sets skin for grid.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <param name="skin">The skin to be applied.</param>
        public static void ApplySkin(GridModel gridModel, Skins skin)
        {
            gridModel.BeginUpdate();
            gridModel.Options.GridVisualStyles = GridVisualStyles.Custom;

            bool IsGroupingGrid = false;
            GridGroupingControl groupingGrid = null;

            if (gridModel is GridTableModel)
            {
                IsGroupingGrid = true;
                groupingGrid = ((GridTableModel)gridModel).GroupingControl;
            }

            switch (skin)
            {
                case Skins.Vista:
                    gridModel.Options.GridVisualStylesDrawing = new GridVisualStylesVista(GridVisualStyles.Custom);
                    if (IsGroupingGrid)
                    {
                        groupingGrid.GridGroupDropArea.Model.Options.GridVisualStyles = GridVisualStyles.Custom;
                        groupingGrid.GridGroupDropArea.Model.Options.GridVisualStylesDrawing = new GridVisualStylesVista(GridVisualStyles.Custom);

                        groupingGrid.TableOptions.GridVisualStyles = GridVisualStyles.Custom;
                        groupingGrid.TableOptions.GridVisualStylesDrawing = new GridVisualStylesVista(GridVisualStyles.Custom);
                    }

                    break;
            }

            gridModel.EndUpdate();
            gridModel.Refresh();
        }
    }

    #region Vista
    /// <summary>
    /// Implements the Vista look and feel.
    /// </summary>
    public class GridVisualStylesVista : Disposable, IVisualStylesDrawing
    {
        private GridVisualStyles visualStyle;
        static Syncfusion.Drawing.IconPaint iconPainter = null;

        /// <summary>
        /// Creates a new instance of <see cref="GridVisualStylesVista"/> class.
        /// </summary>
        /// <param name="style">The current visual style.</param>
        public GridVisualStylesVista(GridVisualStyles style)
        {
            this.visualStyle = style;
            Assembly assembly = Assembly.GetAssembly(typeof(Syncfusion.Windows.Forms.GridVisualStyles));

            if (iconPainter == null)
            {
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
            }
        }

        #region IVisualStylesDrawing Members

        /// <summary>
        /// Draws the header skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the header.</param>
        public void DrawHeaderStyle(Graphics g, Rectangle rect, ThemedHeaderDrawing.HeaderState state)
        {
            ColorBlend cb = new ColorBlend(22);
            cb.Positions = new float[] { 0.0F, 0.04F, 0.08F, 0.16F, 0.20F, 0.24F, 0.28F, 0.32F, 0.40F, 0.44F, 0.48F, 0.56F, 0.60F, 0.64F, 0.68F, 0.72F, 0.76F, 0.84F, 0.88F, 0.92F, 0.96F, 1.0F };

            //// Check for empty headers
            if (rect.Height == 0 && rect.Width == 0)
            {
                return;
            }

            //// Check for the current state of the header and paints the foreground accordingly.

            if (state == ThemedHeaderDrawing.HeaderState.Normal)
            {
                cb.Colors = VistaColors.Normal;
                LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(252, 252, 252), Color.FromArgb(233, 238, 239), LinearGradientMode.Vertical);
                br.InterpolationColors = cb;
                g.FillRectangle(br, rect);
                br.Dispose();
            }
            else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
            {
                cb.Colors = VistaColors.MouseDownColor;
                LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(140, 211, 247), Color.FromArgb(189, 231, 255), LinearGradientMode.Vertical);
                br.InterpolationColors = cb;
                g.FillRectangle(br, rect);
                br.Dispose();
            }
            else
            {
                cb.Colors = VistaColors.MouseOverColor;
                LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(190, 239, 255), LinearGradientMode.Vertical);
                br.InterpolationColors = cb;
                g.FillRectangle(br, rect);
                br.Dispose();
            }
        }

        /// <summary>
        /// Returns the Header Border Colors.
        /// </summary>
        /// <param name="clrBottom">The bottom border color</param>
        /// <param name="clrRight">The right border color</param>
        /// <param name="clrInteriorFirst">The gradient start color for the header interior</param>
        /// <param name="clrInteriorLast">The gradient end color for the header interior</param>
        /// <returns>boolean value</returns>
        public bool GetHeaderBorderColors(out Color clrBottom, out Color clrRight, out Color clrInteriorFirst, out Color clrInteriorLast)
        {
            clrBottom = Color.FromArgb(222, 219, 222);
            clrRight = Color.FromArgb(222, 219, 222);
            clrInteriorFirst = Color.FromArgb(252, 252, 252);
            clrInteriorLast = Color.FromArgb(233, 238, 239);
            return true;
        }

        /// <summary>
        /// Returns the SortIcon interior
        /// </summary>
        /// <param name="brush">The brush used to fill the sort icon</param>
        /// <param name="pen">The pen used to draw the sort icon</param>
         public void GetSortIconBrush(out Brush brush, out Pen pen)
        {
            brush = new SolidBrush(Color.FromArgb(145, 153, 164));
            pen = new Pen(Color.FromArgb(145, 153, 164));
        }

        /// <summary>
        /// Returns the backcolor and header interior for GroupDropArea.
        /// </summary>
        /// <param name="backColor">The back color for GroupDropArea</param>
        /// <param name="headerBorderTop">The top border color for GroupDropArea header</param>
        /// <param name="headerBorderLeft">The left border color for GroupDropArea header</param>
        /// <returns>boolean value</returns>
        public bool GetGroupDropAreaColors(out Color backColor, out Color headerBorderTop, out Color headerBorderLeft)
        {
            backColor = Color.FromArgb(82, 162, 173);
            headerBorderTop = Color.FromArgb(222, 219, 222);
            headerBorderLeft = Color.FromArgb(222, 219, 222);
            return true;
        }
       
        /// <summary>
        /// Draws the PushButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the button.</param>
        public void DrawPushButtonStyle(Graphics g, Rectangle rect, ButtonState state)
        {
            if (rect.Height == 0 || rect.Width == 0)
            {
                return;
            }

            try
            {
                if (state == ButtonState.Flat)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(
                        g, 
                        rect, 
                        VistaColors.Border, 
                        VistaColors.TopFirst, 
                        VistaColors.TopLast, 
                        VistaColors.BottomFirst, 
                        VistaColors.BottomLast, 
                        VistaColors.BottomLine);
                }
                else if (state == ButtonState.Normal)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(
                        g, 
                        rect, 
                        VistaColors.BorderOverColor, 
                        VistaColors.TopFirstOverColor, 
                        VistaColors.TopLastOverColor,
                        VistaColors.BottomFirstOverColor, 
                        VistaColors.BottomLastOverColor, 
                        VistaColors.BottomLineOverColor);
                }
                else if (state == ButtonState.Pushed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(
                        g, 
                        rect, 
                        VistaColors.BorderPressed, 
                        VistaColors.TopFirstPressed, 
                        VistaColors.TopLastPressed,
                        VistaColors.BottomFirstPressed, 
                        VistaColors.BottomLastPressed, 
                        VistaColors.BottomLinePressed);
                }
            }
            catch
            {                 
            }
        }

        /// <summary>
        /// Draws the ComboBox skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the combo button.</param>
        /// <param name="clrBack">Arrow color.</param>
        public void DrawComboBoxStyle(Graphics g, Rectangle rect, ThemedComboBoxDrawing.DropDownState state, Color clrBack)
        {
            Point ptOffset = Point.Empty;

            if (rect.Height == 0 || rect.Width == 0)
            {
                return;
            }

            if (state == ThemedComboBoxDrawing.DropDownState.Normal)
            {
                clrBack = Color.FromArgb(255, 255, 255);
                Brush brush = new SolidBrush(clrBack);
                rect.Inflate(-1, -1);
                g.FillRectangle(brush, rect);
                brush.Dispose();
            }
            else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
            {
                rect.Inflate(-1, -1);
                DrawingUtils.PaintButtonGradient(
                    g, 
                    rect, 
                    VistaColors.BorderOverColor, 
                    VistaColors.TopFirstOverColor, 
                    VistaColors.TopLastOverColor,
                    VistaColors.BottomFirstOverColor, 
                    VistaColors.BottomLastOverColor, 
                    VistaColors.BottomLineOverColor);
            }
            else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
            {
                rect.Inflate(-1, -1);
                DrawingUtils.PaintButtonGradient(
                    g, 
                    rect, 
                    VistaColors.BorderPressed, 
                    VistaColors.TopFirstPressed, 
                    VistaColors.TopLastPressed,
                    VistaColors.BottomFirstPressed, 
                    VistaColors.BottomLastPressed, 
                    VistaColors.BottomLinePressed);
            }

            string bitmapName = "Down.png";
            iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.Black);
        }

        /// <summary>
        /// Draws the SpinButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="btnId">An integer that represents the type of the button.</param>
        /// <param name="btnState">The current state of the spin button.</param>
        /// <param name="clrBack">Arrow color.</param>
        public void DrawSpinButtonStyle(Graphics g, Rectangle rect, ButtonID btnId, ButtonState btnState, Color clrBack)
        {
            Point ptOffset = Point.Empty;
            if (btnId == ButtonID.Up)
            {
                ptOffset = new Point(-1, -1);
            }

            if (rect.Height == 0 || rect.Width == 0)
            {
                return;
            }

            if (btnState == ButtonState.Flat)
            {
                Brush brush = new SolidBrush(clrBack);
                g.FillRectangle(brush, rect);
                brush.Dispose();
            }
            else if (btnState == ButtonState.Normal)
            {
                DrawingUtils.PaintButtonGradient(
                    g, 
                    rect, 
                    VistaColors.BorderOverColor, 
                    VistaColors.TopFirstOverColor, 
                    VistaColors.TopLastOverColor,
                    VistaColors.BottomFirstOverColor, 
                    VistaColors.BottomLastOverColor, 
                    VistaColors.BottomLineOverColor);
            }
            else if (btnState == ButtonState.Pushed)
            {
                if (btnId == ButtonID.Up)
                {
                    ptOffset = new Point(0, 0);
                }
                else
                {
                    ptOffset = new Point(1, 1);
                }

                DrawingUtils.PaintButtonGradient(
                    g, 
                    rect,
                    VistaColors.BorderPressed, 
                    VistaColors.TopFirstPressed, 
                    VistaColors.TopLastPressed,
                    VistaColors.BottomFirstPressed,
                    VistaColors.BottomLastPressed, 
                    VistaColors.BottomLinePressed);
            }

            string bitmapName;

            if (btnId == ButtonID.Down)
            {
                bitmapName = "Down.png";
            }
            else
            {
                bitmapName = "Up.png";
            }

            iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.FromArgb(95, 110, 165));
        }

        private Pen GetVistaBorderPen(ButtonState buttonState)
        {
            Pen pen;
            if ((buttonState & ButtonState.Pushed) > 0)
            {
                pen = new Pen(VistaColors.CheckBoxPressedBorder);
            }
            else if ((buttonState & ButtonState.Flat) > 0)
            {
                pen = new Pen(VistaColors.CheckBoxHOverBorder);
            }
            else
            {
                pen = new Pen(VistaColors.CheckBoxNormalBorder);
            }

            return pen;
        }

        /// <summary>
        /// Draws the CheckBox skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the checkbox</param>
        /// <param name="mixedState">Specifies whether the button is tri-stated.</param>
        public void DrawCheckBoxStyle(Graphics g, Rectangle rect, ButtonState state, bool mixedState)
        {
            if (rect.Height == 0 || rect.Width == 0)
            {
                return;
            }

            try
            {
                Pen borderPen = this.GetVistaBorderPen(state);
                g.DrawRectangle(borderPen, rect.X, rect.Y, (rect.Width - 1), (rect.Height - 1));

                Color penColor;
                Color color;

                if ((state & ButtonState.Pushed) > 0)
                {
                    penColor = VistaColors.CheckBoxPressedInnerBorder;
                    color = VistaColors.CheckBoxPressedFill;
                }
                else if ((state & ButtonState.Flat) > 0)
                {
                    penColor = VistaColors.CheckBoxInnerBorderOver;
                    color = VistaColors.CheckBoxFillOver;
                }
                else
                {
                    penColor = VistaColors.CheckBoxInnerBorderNormal;
                    color = VistaColors.CheckBoxFillNormal;
                }

                Rectangle innerRect = rect;
                LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.FromArgb(252, 252, 252), LinearGradientMode.ForwardDiagonal);
                Pen pen = new Pen(innerBrush);
                LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.FromArgb(252, 252, 252), LinearGradientMode.ForwardDiagonal);

                innerRect.Inflate(-2, -2);
                innerRect.Width--;
                innerRect.Height--;

                g.DrawRectangle(pen, innerRect);

                innerRect.Inflate(-1, -1);
                innerRect.Width++;
                innerRect.Height++;

                g.FillRectangle(brush, innerRect);

                if ((state & ButtonState.Checked) > 0)
                {
                    Pen checkPen = new Pen(VistaColors.CheckColor, 2);

                    Point[] points = new Point[] 
                    {
                                                     new Point(innerRect.X, (innerRect.Y + (innerRect.Height / 2))),
                                                     new Point((innerRect.X + (innerRect.Width / 2) - 1), (innerRect.Bottom - 2)),
                                                     new Point((innerRect.Right - 1), (innerRect.Top - 1))                                                
                    };

                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawLines(checkPen, points);
                    g.SmoothingMode = prevMode;
                    checkPen.Dispose();
                }

                if (mixedState)
                {
                    LinearGradientBrush br = new LinearGradientBrush(innerRect, Color.FromArgb(123, 203, 231), Color.FromArgb(33, 89, 140), LinearGradientMode.Vertical);
                    Pen pen1 = new Pen(Color.FromArgb(41, 97, 140));
                    g.DrawRectangle(pen1, (innerRect.X - 1), (innerRect.Y - 1), (innerRect.Width + 1), (innerRect.Height + 1));
                    g.FillRectangle(br, innerRect);
                    pen1.Dispose();
                    br.Dispose();
                    if ((state & ButtonState.Flat) > 0)
                    {
                        color = Color.FromArgb(38, 171, 217);
                        LinearGradientBrush bru = new LinearGradientBrush(innerRect, color, Color.FromArgb(252, 252, 252), LinearGradientMode.ForwardDiagonal);
                        g.FillRectangle(bru, innerRect);
                        bru.Dispose();
                    }
                }

                borderPen.Dispose();
                pen.Dispose();
                brush.Dispose();
                innerBrush.Dispose();
            }
           catch 
            {                 
            }
        }

        /// <summary>
        /// Draws the RadioButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the radio button</param>
        public void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state) ////, bool mixedState)
        {
            if (rect.Height == 0 || rect.Width == 0)
            {
                return;
            }

            try
            {
                SmoothingMode prevMode = g.SmoothingMode;

                g.SmoothingMode = SmoothingMode.AntiAlias;
                rect.Width -= 2;
                rect.Height -= 4;

                int outRectSize = rect.Height;
                if (rect.Height > 12)
                {
                    outRectSize = 12;
                }

                Pen borderPen = this.GetVistaBorderPen(state);
                g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);

                Color penColor;
                Color color;

                if ((state & ButtonState.Pushed) > 0)
                {
                    penColor = VistaColors.RadioButtonInnerBorderPressed;
                    color = VistaColors.RadioButtonPressedFill;
                }
                else if ((state & ButtonState.Flat) > 0)
                {
                    penColor = VistaColors.RadioButtonInnerBorderOver;
                    color = VistaColors.RadioButtonFillOver;
                }
                else
                {
                    penColor = VistaColors.RadioButtonInnerBorderNormal;
                    color = VistaColors.RadioButtonFillNormal;
                }

                Rectangle innerRect = rect;
                LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.FromArgb(252, 252, 252), LinearGradientMode.ForwardDiagonal);
                Pen pen = new Pen(innerBrush, 2f);
                LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.FromArgb(252, 252, 252), LinearGradientMode.ForwardDiagonal);

                innerRect.Inflate(-2, -2);
                innerRect.Width--;
                innerRect.Height--;

                g.DrawEllipse(pen, innerRect.X, innerRect.Y, outRectSize - 4, outRectSize - 4);

                innerRect.Inflate(-1, -1);
                innerRect.Width++;
                innerRect.Height++;

                g.FillEllipse(brush, innerRect.X, innerRect.Y, outRectSize - 5, outRectSize - 5);

                if ((state & ButtonState.Checked) > 0)
                {
                    innerRect.Width--;
                    innerRect.Height--;
                    Pen pen1 = new Pen(Color.FromArgb(24, 60, 90), 1.8f);
                    g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                    innerRect.Inflate(-1, -1);
                    Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                    LinearGradientBrush br = new LinearGradientBrush(innMost, Color.FromArgb(206, 239, 255), Color.FromArgb(8, 130, 198), LinearGradientMode.ForwardDiagonal);
                    g.FillEllipse(br, innMost);
                    pen1.Dispose();
                    br.Dispose();
                }

                g.SmoothingMode = prevMode;

                borderPen.Dispose();
                pen.Dispose();
                brush.Dispose();
                innerBrush.Dispose();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets the current VisualStyles.
        /// </summary>
        public GridVisualStyles VisualStyle
        {
            get 
            { 
                return this.visualStyle; 
            }
        }
        #endregion
    }

    #endregion

    #region VistaColors

    /// <summary>
    /// Represents the colors for Vista style.
    /// Provides static members to access the colors used by different grid elements.
    /// </summary>
    public class VistaColors
    {
        /// <summary>
        /// Initializes a new <see cref="VistaColors"/>
        /// </summary>
        public VistaColors()
            : base()
        {
        }

        // Normal Button colors
        /// <summary>
        /// Default color for TopFirst.
        /// </summary>
        public static Color TopFirst = Color.FromArgb(255, 247, 255);
        /// <summary>
        /// Default color for TopLast.
        /// </summary>
        public static Color TopLast = Color.FromArgb(247, 239, 247);
        /// <summary>
        /// Default color for BottomFirst.
        /// </summary>
        public static Color BottomFirst = Color.FromArgb(231, 227, 231);
        /// <summary>
        /// Default color for BottomLast.
        /// </summary>
        public static Color BottomLast = Color.FromArgb(214, 211, 214);
        /// <summary>
        /// Default color for BottomLine.
        /// </summary>
        public static Color BottomLine = Color.FromArgb(255, 247, 255);
        /// <summary>
        /// Default color for normal border.
        /// </summary>
        public static Color Border = Color.FromArgb(113, 115, 113);

        // MouseHOver Button colors
        /// <summary>
        /// Default color while mousehovering the TopFirst border.
        /// </summary>
        public static Color TopFirstOverColor = Color.FromArgb(239, 247, 255);
        /// <summary>
        /// Default color while mousehovering the TopLast border.
        /// </summary>
        public static Color TopLastOverColor = Color.FromArgb(222, 243, 255);
        /// <summary>
        /// Default color while mouse hovering the BottomFirst border.
        /// </summary>
        public static Color BottomFirstOverColor = Color.FromArgb(189, 231, 255);
        /// <summary>
        /// Default color while mouse hovering the BottomLast border.
        /// </summary>
        public static Color BottomLastOverColor = Color.FromArgb(165, 219, 247);
        /// <summary>
        /// Default color while mouse hovering the BottomLine border.
        /// </summary>
        public static Color BottomLineOverColor = Color.FromArgb(239, 247, 255);
        /// <summary>
        /// Default color while mouse hovering the border.
        /// </summary>
        public static Color BorderOverColor = Color.FromArgb(57, 125, 181);

        // MouseDown Button colors
        /// <summary>
        /// Default color while mouse button pressed in the TopFirst border.
        /// </summary>
        public static Color TopFirstPressed = Color.FromArgb(231, 243, 255);
        /// <summary>
        /// Default color while mouse button pressed in the TopLast border.
        /// </summary>
        public static Color TopLastPressed = Color.FromArgb(198, 231, 247);
        /// <summary>
        /// Default color while mouse button pressed in the BottomFirst border.
        /// </summary>
        public static Color BottomFirstPressed = Color.FromArgb(156, 211, 239);
        /// <summary>
        /// Default color while mouse button pressed in the BottomLast border.
        /// </summary>
        public static Color BottomLastPressed = Color.FromArgb(107, 178, 222);
        /// <summary>
        /// Default color while mouse button pressed in the BottomLine border.
        /// </summary>
        public static Color BottomLinePressed = Color.FromArgb(107, 178, 222);
        /// <summary>
        /// Default color while mouse button pressed in the border.
        /// </summary>
        public static Color BorderPressed = Color.FromArgb(41, 97, 140);

        // CheckBox Button colors
        /// <summary>
        /// Default color for inner border in filled checkbox.
        /// </summary>
        public static Color CheckBoxInnerBorderNormal = Color.FromArgb(181, 182, 198);
        /// <summary>
        /// Default color for the filled checkbox.
        /// </summary>
        public static Color CheckBoxFillNormal = Color.FromArgb(214, 211, 222);
        /// <summary>
        /// Default color while mouse hovering the inner border in filled checkbox.
        /// </summary>
        public static Color CheckBoxInnerBorderOver = Color.FromArgb(123, 199, 255);
        /// <summary>
        /// Default color while mouse hovering the filled checkbox.
        /// </summary>
        public static Color CheckBoxFillOver = Color.FromArgb(181, 223, 255);
        /// <summary>
        /// Default color while mouse button pressed in the filled checkbox' inner border.
        /// </summary>
        public static Color CheckBoxPressedInnerBorder = Color.FromArgb(90, 182, 247);
        /// <summary>
        /// Default color while mouse button pressed in the filled checkbox.
        /// </summary>
        public static Color CheckBoxPressedFill = Color.FromArgb(156, 215, 255);
        /// <summary>
        /// Default color for the check mark.
        /// </summary>
        public static Color CheckColor = Color.FromArgb(66, 89, 156);
        /// <summary>
        /// Default color while mouse hovering the filled checkbox' border.
        /// </summary>
        public static Color CheckBoxHOverBorder = Color.FromArgb(82, 134, 165);
        /// <summary>
        /// Default color for the border in checkbox.
        /// </summary>
        public static Color CheckBoxNormalBorder = Color.FromArgb(140, 142, 140);
        /// <summary>
        /// Default color while mouse button pressed in the checkbox' border.
        /// </summary>
        public static Color CheckBoxPressedBorder = Color.FromArgb(41, 97, 140);

        // Radio button colors
        /// <summary>
        /// Default color while mouse button pressed in the border of RadioButton.
        /// </summary>
        public static Color RadioButtonPressedBorder = Color.FromArgb(57, 105, 140);
        /// <summary>
        /// Default color while mouse button pressed in the filled RadioButton.
        /// </summary>
        public static Color RadioButtonPressedFill = Color.FromArgb(148, 211, 255);
        /// <summary>
        /// Default color while mouse hovering the inner border of RadioButton.
        /// </summary>
        public static Color RadioButtonInnerBorderOver = Color.FromArgb(132, 207, 255);
        /// <summary>
        /// Default color while mouse hovering the filled RadioButton.
        /// </summary>
        public static Color RadioButtonFillOver = Color.FromArgb(181, 227, 255);
        /// <summary>
        /// Default color for inner border in RadioButton.
        /// </summary>
        public static Color RadioButtonInnerBorderNormal = Color.FromArgb(189, 190, 198);
        /// <summary>
        /// Default color for filled RadioButton.
        /// </summary>
        public static Color RadioButtonFillNormal = Color.FromArgb(244, 244, 244);
        /// <summary>
        /// Default color for flat border in RadioButton.
        /// </summary>
        public static Color RadioButtonFlatBorder = Color.FromArgb(90, 142, 173);
        /// <summary>
        /// Default color for normal border in RadioButton.
        /// </summary>
        public static Color RadioButtonNormalBorder = Color.FromArgb(156, 150, 156);
        /// <summary>
        /// Default color while mouse button pressed in the inner border of RadioButton.
        /// </summary>
        public static Color RadioButtonInnerBorderPressed = Color.FromArgb(90, 182, 247);

        /// <summary>
        /// Gets an array of colors used for Vista style.
        /// </summary>
        public static Color[] Normal
        {
            get
            {
                return new Color[]
                {
                                     Color.FromArgb(254, 254, 254), Color.FromArgb(254, 254, 254), Color.FromArgb(254, 254, 254),
                                      Color.FromArgb(254, 254, 254), Color.FromArgb(254, 254, 254), Color.FromArgb(254, 254, 254),
                                      Color.FromArgb(254, 254, 254), Color.FromArgb(254, 254, 254), Color.FromArgb(254, 254, 254),
                                      Color.FromArgb(247, 248, 250), Color.FromArgb(247, 248, 250), Color.FromArgb(246, 247, 249),
                                      Color.FromArgb(246, 247, 249), Color.FromArgb(246, 247, 249), Color.FromArgb(245, 246, 248),
                                      Color.FromArgb(245, 246, 248), Color.FromArgb(244, 245, 247), Color.FromArgb(243, 244, 246),
                                      Color.FromArgb(243, 244, 246), Color.FromArgb(242, 243, 245), Color.FromArgb(241, 242, 244),
                                      Color.FromArgb(241, 242, 244)                                  
                };
            }
        }

        /// <summary>
        /// Gets an array of colors used to represent Hot State color.
        /// </summary>
        public static Color[] MouseOverColor
        {
            get
            {
                return new Color[]
                {
                                         Color.FromArgb(231, 247, 255), Color.FromArgb(231, 247, 255), Color.FromArgb(231, 247, 255),
                                         Color.FromArgb(231, 247, 255), Color.FromArgb(231, 247, 255), Color.FromArgb(231, 247, 255),
                                         Color.FromArgb(231, 247, 255), Color.FromArgb(231, 247, 255), Color.FromArgb(231, 247, 255),
                                         Color.FromArgb(189, 239, 255), Color.FromArgb(189, 239, 255), Color.FromArgb(189, 239, 255),
                                         Color.FromArgb(189, 235, 255), Color.FromArgb(189, 235, 255), Color.FromArgb(189, 231, 255),
                                         Color.FromArgb(189, 231, 255), Color.FromArgb(181, 231, 255), Color.FromArgb(181, 231, 255),
                                         Color.FromArgb(173, 227, 255), Color.FromArgb(173, 227, 255), Color.FromArgb(165, 219, 255),
                                         Color.FromArgb(165, 219, 255)                                      
                };
            }
        }

        /// <summary>
        /// Gets an array of colors used to represent Pressed State color.
        /// </summary>
        public static Color[] MouseDownColor
        {
            get
            {
                return new Color[]
                {
                                      Color.FromArgb(189, 231, 255), Color.FromArgb(189, 231, 255), Color.FromArgb(189, 231, 255),
                                     Color.FromArgb(189, 231, 255), Color.FromArgb(189, 231, 255), Color.FromArgb(189, 231, 255),
                                     Color.FromArgb(189, 231, 255), Color.FromArgb(189, 231, 255), Color.FromArgb(189, 231, 255),
                                     Color.FromArgb(140, 215, 247), Color.FromArgb(140, 215, 247), Color.FromArgb(140, 215, 247),
                                     Color.FromArgb(140, 215, 247), Color.FromArgb(140, 215, 247), Color.FromArgb(140, 215, 247),
                                      Color.FromArgb(140, 215, 247), Color.FromArgb(140, 211, 247), Color.FromArgb(140, 211, 247),
                                      Color.FromArgb(140, 211, 247), Color.FromArgb(140, 211, 247), Color.FromArgb(140, 211, 247),
                                      Color.FromArgb(140, 211, 247)                                 
                };
            }
        }       
    }
    #endregion

    #region Enum GridSkins

    /// <summary>
    /// Specifies the skin with which various components across the grid will appear and behave.
    /// </summary>
    public enum Skins
    {
        /// <summary>
        /// Applies vista theme.
        /// </summary>
        Vista
    }

    #endregion
}
