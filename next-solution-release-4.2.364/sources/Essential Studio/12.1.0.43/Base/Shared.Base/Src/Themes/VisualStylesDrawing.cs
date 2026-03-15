#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using System.ComponentModel;
using System.Reflection;
namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Implement this interface to support skins across the Grid. Exposes some themed drawing methods.
	/// </summary>
	public interface IVisualStylesDrawing
	{
		/// <summary>
		/// Draws the Header skins.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
		/// <param name="state">The current state of the header.</param>
		void DrawHeaderStyle(Graphics g, Rectangle rect, ThemedHeaderDrawing.HeaderState state);
        
		/// <summary>
		/// Returns the Header Border Colors.
		/// </summary>
		/// <param name="clrBottom">The bottom border color</param>
		/// <param name="clrRight">The right border color</param>
		/// <param name="clrInteriorFirst">The gradient start color for the header interior</param>
		/// <param name="clrInteriorLast">The gradient end color for the header interior</param>
        /// <returns></returns>
        bool GetHeaderBorderColors(out Color clrBottom, out Color clrRight, out Color clrInteriorFirst, out Color clrInteriorLast);

        /// <summary>
        /// Returns the SortIcon interior
        /// </summary>
        /// <param name="brush">The brush used to fill the sort icon</param>
        /// <param name="pen">The pen used to draw the sort icon</param>
        /// <returns></returns>
        void GetSortIconBrush(out Brush brush, out Pen pen);

		/// <summary>
		/// Returns the backcolor and header interior for GroupDropArea.
		/// </summary>
		/// <param name="backColor">The back color for GroupDropArea</param>
		/// <param name="headerBorderTop">The top border color for GroupDropArea header</param>
		/// <param name="headerBorderLeft">The left border color for GroupDropArea header</param>
		/// <returns></returns>
		bool GetGroupDropAreaColors(out Color backColor, out Color headerBorderTop, out Color headerBorderLeft);

		/// <summary>
		/// Draws the PushButton skins
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
		/// <param name="state">The current state of the button.</param>
		void DrawPushButtonStyle(Graphics g, Rectangle rect, ButtonState state);

		/// <summary>
		/// Draws the ComboBox skins
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
		/// <param name="state">The current state of the combo button.</param>
		void DrawComboBoxStyle(Graphics g, Rectangle rect, ThemedComboBoxDrawing.DropDownState state, Color clrBack);

		/// <summary>
		/// Draws the SpinButton skins
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
		/// <param name="btnId">An integer that represents the type of the button.</param>
		/// <param name="btnState">The current state of the spin button.</param>
		void DrawSpinButtonStyle(Graphics g, Rectangle rect, ButtonID btnId, ButtonState btnState, Color clrBack);

		/// <summary>
		/// Draws the CheckBox skins
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
		/// <param name="state">The current state of the checkbox</param>
		/// <param name="mixedState">Specifies whether the button is tri-stated.</param>
		void DrawCheckBoxStyle(Graphics g, Rectangle rect, ButtonState state, bool mixedState);

		/// <summary>
		/// Draws the RadioButton skins
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
		/// <param name="state">The current state of the radio button</param>
		void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state);

        /// <summary>
        /// Gets the current VisualStyles.
        /// </summary>
        GridVisualStyles VisualStyle { get; }

	}

    public interface IThemeStyle
    {
        
        /// <summary>
        ///  Returns the Header Border styles.
        /// </summary>
        /// <param name="clrHeaderBottom">The bottom border color of header.</param>
        /// <param name="borderWeight">The bottom border weight.</param>
        /// <returns>returns the weight and color of bottom border.</returns>
        bool GetHeaderBottomBorderStyle(out Color clrHeaderBottom,out GridBottomBorderWeight borderWeight);

        /// <summary>
        /// Header style color of Grid
        /// </summary>
        /// <param name="backColor">Header color</param>
        /// <param name="hoverColor">hover color</param>
        /// <param name="pressedColor">pressed color</param>
        /// <returns>true</returns>
        bool GetHeaderColors(out Color backColor, out Color hoverColor, out Color pressedColor);

        /// <summary>
        /// header style text color
        /// </summary>
        /// <param name="normalTextColor">Normal text color</param>
        /// <param name="hoverTextColor">hover text color</param>
        /// <param name="pressedTextColor">pressed text color</param>
        /// <returns>true</returns>
        bool GetHeaderTextColors(out Color normalTextColor, out Color hoverTextColor, out Color pressedTextColor);

        /// <summary>
        /// Gets the current VisualStyles.
        /// </summary>
        GridVisualStyles VisualStyle { get; }

    }
    # region GridMetroStyle
    /// <summary>
    /// Implements the Metro look and feel
    /// </summary>
    public class GridMetroStyle : Disposable, IVisualStylesDrawing, IThemeStyle
    {
        private GridVisualStyles visualStyle;
        private GridMetroColors gridMetroColors;
        private bool isLegacyStyle;
        static Syncfusion.Drawing.IconPaint iconPainter = null;
        private ThemedHeaderDrawing.HeaderState Headerstate = ThemedHeaderDrawing.HeaderState.Normal;
        private Color MetroColorSelection = Color.FromArgb(42, 191, 241);
        private Color MetroHoverColor = Color.FromArgb(94, 171, 222);
        private Color MetroPressedColor = Color.FromArgb(253, 143, 0);
        private Color MetroGroupBarColor = Color.FromArgb(94, 171, 222);
        private Color MetroHeaderBorderColor = Color.FromArgb(253, 143, 0);
        private Color MetroNormalSortIconColor = Color.FromArgb(94, 171, 222);
        private Color MetroHoverSortIconColor = Color.White;
        /// <summary>
        /// Creates a new instance of <see cref="GridMetroStyle"/> class.
        /// </summary>
        /// <param name="style">The current visual style.</param>
        public GridMetroStyle(GridVisualStyles style)
        {
            this.visualStyle = style;
            if (iconPainter == null)
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
        }
        public GridMetroStyle(GridVisualStyles style, bool legacyStyle)
        {
            this.visualStyle = style;
            this.isLegacyStyle = legacyStyle;
            if (iconPainter == null)
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
        }
        public GridMetroStyle(GridVisualStyles style, bool legacyStyle, Color metroColorSelection, Color metroHoverColor, Color metroPressedColor, Color metroGroupBarColor)
        {
            this.visualStyle = style;
            MetroColorSelection = SkinCollection.HeaderColor.NormalColor = metroColorSelection;
            MetroHoverColor = SkinCollection.HeaderColor.PressedColor = metroHoverColor;
            MetroPressedColor = SkinCollection.HeaderColor.HoverColor = metroPressedColor;
            MetroGroupBarColor = SkinCollection.GroupDropAreaColor.BackColor = metroGroupBarColor;
            this.isLegacyStyle = legacyStyle;
            if (iconPainter == null)
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
        }
        
        public GridMetroStyle(GridVisualStyles style, GridMetroColors metroColors)
        {
            gridMetroColors = metroColors;
            this.visualStyle = style;
            if (iconPainter == null)
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
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
            this.Headerstate = state;

            //Check for empty headers
            if (rect.Height == 0 && rect.Width == 0)
                return;

            //Check for the current state of the header and paints the foreground accordingly.

            if (state == ThemedHeaderDrawing.HeaderState.Normal)
            {
                SolidBrush br = new SolidBrush(SkinCollection.HeaderColor.NormalColor);// MetroColorSelection);
                g.FillRectangle(br, rect);
                br.Dispose();
            }
            else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
            {
                SolidBrush br = new SolidBrush(SkinCollection.HeaderColor.PressedColor);
                g.FillRectangle(br, rect);
                br.Dispose();
            }
            else if (state == ThemedHeaderDrawing.HeaderState.Hot)
            {
                SolidBrush br = new SolidBrush(SkinCollection.HeaderColor.HoverColor);
                g.FillRectangle(br, rect);
                br.Dispose();
            }
            else
            {
                SolidBrush br = new SolidBrush(MetroHoverColor);
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
        /// <returns></returns>
        public bool GetHeaderBorderColors(out Color clrBottom, out Color clrRight, out Color clrInteriorFirst, out Color clrInteriorLast)
        {
            clrBottom = Color.FromArgb(208, 208, 208);
            clrRight = Color.FromArgb(208, 208, 208);
            clrInteriorFirst = Color.FromArgb(208, 208, 208);
            clrInteriorLast = Color.FromArgb(208, 208, 208);
            return true;
        }

        /// <summary>
        ///  Returns the Header Border styles.
        /// </summary>
        /// <param name="clrHeaderBottom">The bottom border color of header.</param>
        /// <param name="borderWeight">The bottom border weight.</param>
        /// <returns>returns the weight and color of bottom border.</returns>
        public bool GetHeaderBottomBorderStyle(out Color clrBottomBorder, out GridBottomBorderWeight borderWeight)
        {
            clrBottomBorder = SkinCollection.HeaderBottomBorderColor;
            borderWeight = SkinCollection.HeaderBottomBorderWeight;
            return true;
        }
        /// <summary>
        /// Returns the SortIcon interior
        /// </summary>
        /// <param name="brush">The brush used to fill the sort icon</param>
        /// <param name="pen">The pen used to draw the sort icon</param>
        /// <returns></returns>
        public void GetSortIconBrush(out Brush brush, out Pen pen)
        {
            if (Headerstate == ThemedHeaderDrawing.HeaderState.Normal)
            {
                brush = new SolidBrush(SkinCollection.SortIconColor.NormalSortIconColor);
                pen = new Pen(SkinCollection.SortIconColor.NormalSortIconColor);
            }
            else
            {
                brush = new SolidBrush(SkinCollection.SortIconColor.HoverSortIconColor);
                pen = new Pen(SkinCollection.SortIconColor.HoverSortIconColor);
            }
        }

        /// <summary>
        /// Returns the backcolor and header interior for GroupDropArea.
        /// </summary>
        /// <param name="backColor">The back color for GroupDropArea</param>
        /// <param name="headerBorderTop">The top border color for GroupDropArea header</param>
        /// <param name="headerBorderLeft">The left border color for GroupDropArea header</param>
        /// <returns></returns>
        public bool GetGroupDropAreaColors(out Color backColor, out Color headerBorderTop, out Color headerBorderLeft)
        {
            backColor = SkinCollection.GroupDropAreaColor.BackColor;
            headerBorderTop = SkinCollection.GroupDropAreaColor.BorderTopColor;
            headerBorderLeft = SkinCollection.GroupDropAreaColor.BorderLeftColor;

            return true;
        }

        /// <summary>
        /// Header style color of Grid
        /// </summary>
        /// <param name="backColor">Header color</param>
        /// <param name="hoverColor">hover color</param>
        /// <param name="pressedColor">pressed color</param>
        /// <returns></returns>
        public bool GetHeaderColors(out Color backColor, out Color hoverColor, out Color pressedColor)
        {
            backColor = SkinCollection.HeaderColor.NormalColor;
            hoverColor = SkinCollection.HeaderColor.HoverColor;
            pressedColor = SkinCollection.HeaderColor.PressedColor;
            return true;
        }


        public bool GetHeaderTextColors(out Color normalTextColor, out Color hoverTextColor, out Color pressedColor)
        {
            normalTextColor = SkinCollection.HeaderTextColor.NormalTextColor;
            hoverTextColor = SkinCollection.HeaderTextColor.HoverTextColor;
            pressedColor = SkinCollection.HeaderTextColor.PressedTextColor;
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
                return;

            try
            {
                if (state == ButtonState.Flat)
                {
                    rect.Inflate(-1, -1);
                    SolidBrush br = new SolidBrush(gridMetroColors.PushButtonColor.NormalBackColor);
                    g.FillRectangle(br, rect);
                    Pen p = new Pen(MetroHoverColor);
                    g.DrawRectangle(p, rect);
                    br.Dispose();
                }
                else if (state == ButtonState.Normal)
                {
                    rect.Inflate(-1, -1);
                    Color normalState = ControlPaint.Dark(gridMetroColors.PushButtonColor.HoverBackColor);
                    SolidBrush br = new SolidBrush(gridMetroColors.PushButtonColor.HoverBackColor);
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                else if (state == ButtonState.Pushed)
                {
                    rect.Inflate(-1, -1);
                    Color pushState = MetroPressedColor;
                    SolidBrush br = new SolidBrush(gridMetroColors.PushButtonColor.PushedBackColor);
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
            }
            catch
            { }

        }

        /// <summary>
        /// Draws the ComboBox skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the combo button.</param>
        public void DrawComboBoxStyle(Graphics g, Rectangle rect, ThemedComboBoxDrawing.DropDownState state, Color clrBack)
        {
            Point ptOffset = Point.Empty;

            if (rect.Height == 0 || rect.Width == 0)
                return;

            if (state == ThemedComboBoxDrawing.DropDownState.Normal)
            {
                Brush brush = new SolidBrush(gridMetroColors.ComboboxColor.NormalBackColor);
                rect.Inflate(-1, 0);
                --rect.Height;
                g.FillRectangle(brush, rect);
                using (Pen pen = new Pen(gridMetroColors.ComboboxColor.NormalBorderColor))
                    g.DrawRectangle(pen, rect);
                brush.Dispose();
            }
            else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
            {
                Brush brush = new SolidBrush(gridMetroColors.ComboboxColor.HoverBackColor);
                rect.Inflate(-1, 0);
                --rect.Height;
                g.FillRectangle(brush, rect);
                using (Pen pen = new Pen(gridMetroColors.ComboboxColor.HoverBorderColor))
                    g.DrawRectangle(pen, rect);
                brush.Dispose();
            }
            else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
            {
                Brush brush = new SolidBrush(gridMetroColors.ComboboxColor.PressedBackColor );
                rect.Inflate(-1, 0);
                --rect.Height;
                g.FillRectangle(brush, rect);
                using (Pen pen = new Pen(gridMetroColors.ComboboxColor.PresedBorderColor))
                    g.DrawRectangle(pen, rect);
                brush.Dispose();
            }

            string bitmapName = "Down.png";

            iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.Red);
        }

        /// <summary>
        /// Draws the SpinButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="btnId">An integer that represents the type of the button.</param>
        /// <param name="btnState">The current state of the spin button.</param>
        public void DrawSpinButtonStyle(Graphics g, Rectangle rect, ButtonID btnId, ButtonState btnState, Color clrBack)
        {
            Point ptOffset = Point.Empty;
            if (btnId == ButtonID.Up)
                ptOffset = new Point(-1, -1);

            if (rect.Height == 0 || rect.Width == 0)
                return;

            if (btnState == ButtonState.Flat)
            {
                Brush brush = new SolidBrush(gridMetroColors.SpinButtonColor.NormalBackColor);
                g.FillRectangle(brush, rect);
                using (Pen pen = new Pen(gridMetroColors.SpinButtonColor.NormalBorderColor))
                    g.DrawRectangle(pen, rect);
                brush.Dispose();
            }
            else if (btnState == ButtonState.Normal)
            {
                Brush brush = new SolidBrush(gridMetroColors.SpinButtonColor.HoverBackColor);
                g.FillRectangle(brush, rect);
                using (Pen pen = new Pen(gridMetroColors.SpinButtonColor.HoverBorderColor))
                    g.DrawRectangle(pen, rect);
                brush.Dispose();
            }
            else if (btnState == ButtonState.Pushed)
            {
                if (btnId == ButtonID.Up)
                    ptOffset = new Point(0, 0);
                else
                    ptOffset = new Point(1, 1);

                Brush brush = new SolidBrush(gridMetroColors.SpinButtonColor.PressedBackColor);
                g.FillRectangle(brush, rect);
                using (Pen pen = new Pen(gridMetroColors.SpinButtonColor.PressedBackColor))
                    g.DrawRectangle(pen, rect);
                brush.Dispose();
            }

            string bitmapName;

            if (btnId == ButtonID.Down)
                bitmapName = "Down.png";
            else
                bitmapName = "Up.png";

            iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
        }

        private Pen MetroBorderPen(ButtonState buttonState)
        {
            Pen pen;
            if ((buttonState & ButtonState.Inactive) > 0)
                pen = new Pen(Off2007Colors.CheckBoxOuterBorderSilverInactive);
            else if ((buttonState & ButtonState.Flat) > 0)
                pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
            else if ((buttonState & ButtonState.Pushed) > 0)
                pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
            else
                pen = new Pen(Off2007Colors.CheckBoxOuterBorderSilverNormal);
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
                return;

            try
            {
                Pen borderPen = this.MetroBorderPen(state);
                borderPen.Color = SkinCollection.CheckBoxColor.BorderColor;
                if ((state & ButtonState.Inactive) > 0)
                    borderPen.Color = SystemColors.ControlDark;
                g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                Color penColor = SkinCollection.CheckBoxColor.BackColor;
                Color color = Color.White;

                Rectangle innerRect = rect;
                SolidBrush innerBrush = new SolidBrush(penColor);
                
                innerRect.Inflate(-1, -1);
                innerRect.Width--;
                innerRect.Height--;

                g.FillRectangle(innerBrush, innerRect);

                if ((state & ButtonState.Checked) > 0)
                {
                    Pen checkPen = new Pen(SkinCollection.CheckBoxColor.CheckColor, 2);
                    if ((state & ButtonState.Inactive) > 0)
                        checkPen = new Pen(SystemColors.ControlDarkDark, 2);

                    Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
													 new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
													 new Point( innerRect.Right - 1, innerRect.Top -1 )
												 };

                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawLines(checkPen, points);
                    g.SmoothingMode = prevMode;
                    checkPen.Dispose();
                }

                if (mixedState && ((state & ButtonState.Inactive) <= 0))
                {
                    SolidBrush br = new SolidBrush(Color.Black);
                    innerRect.Inflate(-2, -2);
                    g.FillRectangle(br, innerRect);
                    br.Dispose();
                }

                borderPen.Dispose();
                innerBrush.Dispose();
            }
            catch { }

        }

        /// <summary>
        /// Draws the RadioButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the radio button</param>
        public void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state)//, bool mixedState)
        {
            if (rect.Height == 0 || rect.Width == 0)
                return;

            try
            {
                SmoothingMode prevMode = g.SmoothingMode;

                g.SmoothingMode = SmoothingMode.AntiAlias;
                rect.Width -= 2;
                rect.Height -= 4;

                int outRectSize = rect.Height;
                if (rect.Height > 12)
                    outRectSize = 12;

                Pen borderPen = this.MetroBorderPen(state);
                borderPen.Color = gridMetroColors.RadiobuttonColor.RadioButtonColor;
                if ((state & ButtonState.Inactive) > 0)
                    borderPen.Color = SystemColors.ControlDark;
                g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);
                
                Color penColor = Color.White;
                Color color = Color.White;

                Rectangle innerRect = rect;
                LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                Pen pen = new Pen(innerBrush);
                LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                    Pen pen1 = new Pen(gridMetroColors.RadiobuttonColor.CheckedColor, 1.8f);
                    g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                    innerRect.Inflate(-1, -1);
                    Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                    SolidBrush br = new SolidBrush(gridMetroColors.RadiobuttonColor.CheckedColor);
                    if ((state & ButtonState.Inactive) > 0)
                        br.Color = SystemColors.ControlDarkDark;
                    g.FillEllipse(br, innMost);
                    br.Dispose();
                    pen1.Dispose();
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
            get { return this.visualStyle; }
        }


        public GridMetroColors SkinCollection
        {
            get 
            {
                if (gridMetroColors == null)
                    gridMetroColors = new GridMetroColors();
                return gridMetroColors;
            }
        }
        #endregion
    }

    # endregion

    #region SkinCollection

    /// <summary>
    /// Implement the SkinCollection for GridMetroColors
    /// </summary>
    public class GridMetroColors 
    {
        private Color headerThickBorderColor = Color.FromArgb(94, 171, 222);
        private GridBottomBorderWeight headerBottomBorderWeight = GridBottomBorderWeight.ExtraThick;
        
        internal GridHeaderText headerTextColor;
        internal GridHeader headerColor;
        internal GridSortIcon sortIconColor;
        internal GridGroupDropAreaColor groupDropAreaColor;
        internal GridCheckBox checkBoxColor;
        internal GridComboBox comboboxColor;
        internal GridPushButton pushButtonColor;
        internal GridRadioButton radiobuttonColor;
        internal GridSpinButton spinButtonColor;

        /// <summary>
        /// Collecction initialize
        /// </summary>
        public GridMetroColors()
        {
            //constructor
        }
        /// <summary>
        /// get / set Header color collection
        /// </summary>
        public GridHeader HeaderColor
        {
            get
            {
                if (headerColor == null)
                {
                    headerColor = new GridHeader();
                }

                return headerColor;
            }

            set
            {
                if (headerColor != value)
                {
                    headerColor = value;
                }
            }
        }
        /// <summary>
        /// get / set Header text color collection
        /// </summary>
        public GridHeaderText HeaderTextColor
        {
            get
            {
                if (headerTextColor == null)
                {
                    headerTextColor = new GridHeaderText();
                }

                return headerTextColor;
            }

            set
            {
                if (headerTextColor != value)
                {
                    headerTextColor = value;
                }
            }
        }

        /// <summary>
        /// get /set grop drop area color
        /// </summary>
        public GridGroupDropAreaColor GroupDropAreaColor
        {
            get
            {
                if (groupDropAreaColor == null)
                {
                    groupDropAreaColor = new GridGroupDropAreaColor();
                }

                return groupDropAreaColor;
            }

            set
            {
                if (groupDropAreaColor != value)
                {
                    groupDropAreaColor = value;
                }
            }
        }

        /// <summary>
        /// get / set sort icon color collection
        /// </summary>
        public GridSortIcon SortIconColor
        {
            get
            {
                if (sortIconColor == null)
                {
                    sortIconColor = new GridSortIcon();
                }

                return sortIconColor;
            }

            set
            {
                if (sortIconColor != value)
                {
                    sortIconColor = value;
                }
            }
        }
        /// <summary>
        /// get / set collection of check box color
        /// </summary>
        public GridCheckBox CheckBoxColor
        {
            get
            {
                if (checkBoxColor == null)
                {
                    checkBoxColor = new GridCheckBox();
                }

                return checkBoxColor;
            }

            set
            {
                if (checkBoxColor != value)
                {
                    checkBoxColor = value;
                }
            }
        }

        /// <summary>
        /// get/ set combo box color collection
        /// </summary>
        public GridComboBox ComboboxColor
        {
            get
            {
                if (comboboxColor == null)
                {
                    comboboxColor = new GridComboBox();
                }

                return comboboxColor;
            }

            set
            {
                if (comboboxColor != value)
                {
                    comboboxColor = value;
                }
            }
        }

        /// <summary>
        /// get / set spin button color collection
        /// </summary>
        public GridSpinButton SpinButtonColor
        {
            get
            {
                if (spinButtonColor == null)
                {
                    spinButtonColor = new GridSpinButton();
                }

                return spinButtonColor;
            }

            set
            {
                if (spinButtonColor != value)
                {
                    spinButtonColor = value;
                }
            }
        }

        /// <summary>
        /// get /set push button color collection
        /// </summary>
        public GridPushButton PushButtonColor
        {
            get
            {
                if (pushButtonColor == null)
                {
                    pushButtonColor = new GridPushButton();
                }
                return pushButtonColor;
            }
            set
            {
                pushButtonColor = value;
            }
        }

        /// <summary>
        /// get or set radio button color collection
        /// </summary>
        public GridRadioButton RadiobuttonColor
        {
            get
            {
                if (radiobuttonColor == null)
                {
                    radiobuttonColor = new GridRadioButton();
                }
                return radiobuttonColor;
            }
            set
            {
                radiobuttonColor = value;
            }
        }

        /// <summary>
        /// gets or sets header botom border color
        /// </summary>
        public Color HeaderBottomBorderColor
        {
            get
            {
                return headerThickBorderColor;
            }
            set
            {
                headerThickBorderColor = value;
            }
        }

        /// <summary>
        /// Gets/Sets the header botom border weight.
        /// </summary>
        public GridBottomBorderWeight HeaderBottomBorderWeight
        {
            get
            {
                return headerBottomBorderWeight;
            }
            set
            {
               headerBottomBorderWeight = value;
            }
        }
    }

    /// <summary>
    /// Specifies the weight of a header bottom border <see cref="GridBorder"/> class.
    /// None option is used to Show / hide the bottom border.
    /// </summary>
    public enum GridBottomBorderWeight
    {
        /// <summary>
        ///        A line with 0.25 point.
        /// </summary>
        ExtraThin = 1,

        /// <summary>
        ///        A line with 0.5 point.
        /// </summary>    
        Thin = 2,

        /// <summary>
        ///        A line with 1 point.
        /// </summary>
        Medium = 3,

        /// <summary>
        ///        A line with 1.5 points.
        /// </summary>
        Thick = 4,
             
        /// <summary>
        ///        A line with 2.0 points.
        /// </summary>
        ExtraThick = 5,
     
        /// <summary>
        ///        A line with 3.0 points.
        /// </summary>
        ExtraExtraThick = 6,

        /// <summary>
        /// This hide the header bottom border.
        /// </summary>
        None = 0
    }

    /// <summary>
    /// Collectionn of Header color
    /// </summary>
    public class GridHeader
    {
        private Color normalColor = Color.White;
        private Color hoverColor = Color.FromArgb(94, 171, 222);
        private Color pressedColor = Color.FromArgb(35, 130, 195);

        /// <summary>
        /// Get / set the Header Color in normal mode
        /// </summary>
        public Color NormalColor
        {
            get
            {
                return normalColor;
            }
            set
            {
                normalColor = value;
            }
        }
        /// <summary>
        /// Get / set the Header Color in mouse Hover
        /// </summary>
        public Color HoverColor
        {
            get
            {
                return hoverColor;
            }
            set
            {
                hoverColor = value;
            }
        }
        /// <summary>
        /// Get / set the header Color in mouse pressed
        /// </summary>
        public Color PressedColor
        {
            get
            {
                return pressedColor;
            }
            set
            {
                pressedColor = value;
            }
        }
    }

    /// <summary>
    /// Collection of Header text color
    /// </summary>
    public class GridHeaderText
    {
        private Color normalTextColor = Color.FromArgb(91, 91, 91);
        private Color hoverTextColor = Color.White;
        private Color pressedTextColor = Color.White;
        /// <summary>
        /// grt /set normal header text color
        /// </summary>
        public Color NormalTextColor
        {
            get
            {
                return normalTextColor;
            }
            set
            {
                normalTextColor = value;
            }
        }
        /// <summary>
        /// get / set hover text color of header
        /// </summary>
        public Color HoverTextColor
        {
            get
            {
                return hoverTextColor;
            }
            set
            {
                hoverTextColor = value;
            }
        }
        /// <summary>
        /// get /set header pressed color
        /// </summary>
        public Color PressedTextColor
        {
            get
            {
                return pressedTextColor;
            }
            set
            {
                pressedTextColor = value;
            }
        }
    }

    /// <summary>
    /// Collection of sort icon color
    /// </summary>
    public class GridSortIcon
    {
        private Color pressedTextColor = Color.FromArgb(35, 130, 195);
        private Color normalSortIconColor = Color.FromArgb(94, 171, 222);
        private Color hoverSortIconColor = Color.White;
        /// <summary>
        /// Get / set the sort icon Color in normal
        /// </summary>
        public Color NormalSortIconColor
        {
            get
            {
                return normalSortIconColor;
            }
            set
            {
                normalSortIconColor = value;
            }
        }
        /// <summary>
        /// Get / set the sort icon  Color in hover
        /// </summary>
        public Color HoverSortIconColor
        {
            get
            {
                return hoverSortIconColor;
            }
            set
            {
                hoverSortIconColor = value;
            }
        }
        
    }

    /// <summary>
    /// Collection of Check box Color
    /// </summary>
    public class GridCheckBox
    {
        private Color borderColor = Color.FromArgb(94, 171, 222);
        private Color penColor = Color.White;
        private Color checkColor = Color.FromArgb(35, 130, 195);
        /// <summary>
        /// Get / set the border color of checkbox
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }
        /// <summary>
        /// Get / set the back color of check box
        /// </summary>
        public Color BackColor
        {
            get
            {
                return penColor;
            }
            set
            {
                penColor = value;
            }
        }
        /// <summary>
        /// Get / set the check marker color
        /// </summary>
        public Color CheckColor
        {
            get
            {
                return checkColor;
            }
            set
            {
                checkColor = value;
            }
        }
    }

    /// <summary>
    /// Collection of GridCombobox color
    /// </summary>
    public class GridComboBox
    {
        private Color normalBackColor = Color.White;
        private Color normalBorderColor = Color.FromArgb(94, 171, 222);
        private Color hoverBackColor = Color.FromArgb(94, 171, 222);
        private Color hoverBorderColor = Color.DarkGray;
        private Color pressedBackColor = Color.FromArgb(253, 143, 0);
        private Color presedBorderColor = Color.FromArgb(94, 171, 222);
        /// <summary>
        /// Get / set the combobox 
        /// </summary>
        public Color NormalBackColor
        {
            get
            {
                return normalBackColor;
            }
            set
            {
                normalBackColor = value;
            }
        }
        /// <summary>
        /// Get / set the combobox NormalBorderColor
        /// </summary>
        public Color NormalBorderColor
        {
            get
            {
                return normalBorderColor;
            }
            set
            {
                normalBorderColor = value;
            }
        }
        /// <summary>
        /// Get / set the hoverBackColor
        /// </summary>
        public Color HoverBackColor
        {
            get
            {
                return hoverBackColor;
            }
            set
            {
                hoverBackColor = value;
            }
        }
        /// <summary>
        /// Get / set the HoverBorderColor
        /// </summary>
        public Color HoverBorderColor
        {
            get
            {
                return hoverBorderColor;
            }
            set
            {
                hoverBorderColor = value;
            }
        }
        /// <summary>
        /// Get / set the pressedBackColor
        /// </summary>
        public Color PressedBackColor
        {
            get
            {
                return pressedBackColor;
            }
            set
            {
                pressedBackColor = value;
            }
        }
        /// <summary>
        /// Get / set the presedBorderColor
        /// </summary>
        public Color PresedBorderColor
        {
            get
            {
                return presedBorderColor;
            }
            set
            {
                presedBorderColor = value;
            }
        }
    }

    /// <summary>
    /// Collection of Grid spin button colors
    /// </summary>
    public class GridSpinButton
    {
        private Color normalBackColor = Color.White;
        private Color normalBorderColor = Color.FromArgb(94, 171, 222);
        private Color hoverBackColor = Color.FromArgb(94, 171, 222);
        private Color hoverBorderColor = Color.DarkGray;
        private Color pressedBackColor = Color.FromArgb(253, 143, 0);
        private Color presedBorderColor = Color.FromArgb(94, 171, 222);
        /// <summary>
        /// Get / set spin button normal back color
        /// </summary>
        public Color NormalBackColor
        {
            get
            {
                return normalBackColor;
            }
            set
            {
                normalBackColor = value;
            }
        }
        /// <summary>
        /// Get / set spin button normal border color
        /// </summary>
        public Color NormalBorderColor
        {
            get
            {
                return normalBorderColor;
            }
            set
            {
                normalBorderColor = value;
            }
        }
        /// <summary>
        /// Get / set spin button hover back color
        /// </summary>
        public Color HoverBackColor
        {
            get
            {
                return hoverBackColor;
            }
            set
            {
                hoverBackColor = value;
            }
        }
        /// <summary>
        /// Get / set spin button hover border color
        /// </summary>
        public Color HoverBorderColor
        {
            get
            {
                return hoverBorderColor;
            }
            set
            {
                hoverBorderColor = value;
            }
        }
        /// <summary>
        /// Get / set spin button pressed back color
        /// </summary>
        public Color PressedBackColor
        {
            get
            {
                return pressedBackColor;
            }
            set
            {
                pressedBackColor = value;
            }
        }
        /// <summary>
        /// Get / set spin button pressed border color
        /// </summary>
        public Color PresedBorderColor
        {
            get
            {
                return presedBorderColor;
            }
            set
            {
                presedBorderColor = value;
            }
        }
    }

    /// <summary>
    /// Collection of grid push button colors
    /// </summary>
    public class GridPushButton
    {
        private Color normalBackColor = Color.White;
        private Color hoverBackColor = Color.FromArgb(94, 171, 222);
        private Color pushedBackColor = Color.FromArgb(253, 143, 0);
        /// <summary>
        /// Get / set normal push button color
        /// </summary>
        public Color NormalBackColor
        {
            get
            {
                return normalBackColor;
            }
            set
            {
                normalBackColor = value;
            }
        }
        /// <summary>
        /// Get / set bush button hover color
        /// </summary>
        public Color HoverBackColor
        {
            get
            {
                return hoverBackColor;
            }
            set
            {
                hoverBackColor = value;
            }
        }
        /// <summary>
        /// Get / set push button back color
        /// </summary>
        public Color PushedBackColor
        {
            get
            {
                return pushedBackColor;
            }
            set
            {
                pushedBackColor = value;
            }
        }
    }

    /// <summary>
    /// Collection of radio button color
    /// </summary>
    public class GridRadioButton
    {
        private Color radioButtonColor = Color.FromArgb(94, 171, 222);
        private Color checkedColor = Color.FromArgb(30, 75, 105);
        /// <summary>
        /// Get / set radio button color
        /// </summary>
        public Color RadioButtonColor
        {
            get
            {
                return radioButtonColor;
            }
            set
            {
                radioButtonColor = value;
            }
        }
        /// <summary>
        /// Get / set radio button checked color
        /// </summary>
        public Color CheckedColor
        {
            get
            {
                return checkedColor;
            }
            set
            {
                checkedColor = value;
            }
        }

    }
    /// <summary>
    /// collection of Group drop area color
    /// </summary>
    public class GridGroupDropAreaColor
    {
        private Color backColor = Color.FromArgb(35, 130, 195);
        private Color borderTopColor = Color.FromArgb(35, 130, 195);
        private Color borderLeftColor = Color.FromArgb(35, 130, 195);
        /// <summary>
        /// Get / set group srop area back color
        /// </summary>
        public Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
            }
        }
        /// <summary>
        /// Get / set grid gropu drop are border top color
        /// </summary>
        public Color BorderTopColor
        {
            get
            {
                return borderTopColor;
            }
            set
            {
                borderTopColor = value;
            }
        }
        /// <summary>
        /// Get / set group drop area border left color
        /// </summary>
        public Color BorderLeftColor
        {
            get
            {
                return borderLeftColor;
            }
            set
            {
                borderLeftColor = value;
            }
        }
    }

#endregion

    # region Office2010
    	/// <summary>
	/// Implements the Office 2010 look and feel
	/// </summary>
    public class GridVisualStylesOffice2010 : Disposable, IVisualStylesDrawing,IThemeStyle
    {
        private GridVisualStyles visualStyle;
        private bool isLegacyStyle;
        static Syncfusion.Drawing.IconPaint iconPainter = null;
        private ThemedHeaderDrawing.HeaderState Headerstate = ThemedHeaderDrawing.HeaderState.Normal;

        /// <summary>
        /// Creates a new instance of <see cref="GridVisualStylesOffice2010"/> class.
        /// </summary>
        /// <param name="style">The current visual style.</param>
        public GridVisualStylesOffice2010(GridVisualStyles style)
        {
            this.visualStyle = style;
            if (iconPainter == null)
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
        }
        public GridVisualStylesOffice2010(GridVisualStyles style, bool legacyStyle)
        {
            this.visualStyle = style;
            this.isLegacyStyle = legacyStyle;

            if (iconPainter == null)
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
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
            if (this.isLegacyStyle)
            {
                #region With Legacy Styles
                ColorBlend cb = new ColorBlend(12);
                cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };
                this.Headerstate = state;

                //Check for empty headers
                if (rect.Height == 0 && rect.Width == 0)
                    return;

                //Check for the current state of the header and paints the foreground accordingly.

                if (state == ThemedHeaderDrawing.HeaderState.Normal)
                {
                    LinearGradientBrush br;
                    if (this.visualStyle == GridVisualStyles.Office2010Blue)
                    {
                        br = new LinearGradientBrush(rect, Color.FromArgb(241, 245, 249), Color.FromArgb(218, 231, 245), LinearGradientMode.Vertical);
                    }
                    else if (this.visualStyle == GridVisualStyles.Office2010Black)
                    {
                        br = new LinearGradientBrush(rect, Color.FromArgb(106, 106, 106), Color.FromArgb(89, 89, 89), LinearGradientMode.Vertical);
                    }
                    else
                    {
                        br = new LinearGradientBrush(rect, Color.FromArgb(223, 227, 232), Color.FromArgb(183, 188, 193), LinearGradientMode.Vertical);
                    }
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
                {
                    cb.Colors = Off2010Colors.MouseDownColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(247, 150, 15), Color.FromArgb(255, 222, 79), LinearGradientMode.Vertical);
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                else
                {
                    // cb.Colors = Off2010Colors.MouseHOverColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 223, 107), Color.FromArgb(255, 254, 235), LinearGradientMode.Vertical);
                    //  br.InterpolationColors = cb;
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                #endregion
            }
            else
            {
                #region WithoutLegacyStyles
                ColorBlend cb = new ColorBlend(12);
                cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };
                this.Headerstate = state;

                //Check for empty headers
                if (rect.Height == 0 && rect.Width == 0)
                    return;

                //Check for the current state of the header and paints the foreground accordingly.

                if (state == ThemedHeaderDrawing.HeaderState.Normal)
                {
                    LinearGradientBrush br;
                    if (this.visualStyle == GridVisualStyles.Office2010Blue)
                    {
                        br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                        g.FillRectangle(br, rect);
                        cb.Colors = new Color[] { Color.FromArgb(233, 240, 250), Color.FromArgb(229, 238, 250), Color.FromArgb(225, 236, 250),
                        Color.FromArgb(221, 234, 251), Color.FromArgb(216, 232, 251), Color.FromArgb(211, 230, 251),
                        Color.FromArgb(206, 228, 252), Color.FromArgb(201, 226, 252), Color.FromArgb(196, 224, 252), 
                        Color.FromArgb(191, 222, 253), Color.FromArgb(190, 220, 253), Color.FromArgb(188, 216, 253)};
                    }
                    else if (this.visualStyle == GridVisualStyles.Office2010Black)
                    {
                        br = new LinearGradientBrush(rect, Color.FromArgb(150, 150, 150), Color.FromArgb(150, 150, 150), LinearGradientMode.Vertical);
                        g.FillRectangle(br, rect);
                        cb.Colors = new Color[] { Color.FromArgb(126, 126, 126), Color.FromArgb(119, 119, 119), Color.FromArgb(112, 112, 112),
                        Color.FromArgb(105, 105, 105), Color.FromArgb(98, 98, 98), Color.FromArgb(91, 91, 91),
                        Color.FromArgb(84, 84, 84), Color.FromArgb(77, 77, 77), Color.FromArgb(70, 70, 70), 
                        Color.FromArgb(63, 63, 63), Color.FromArgb(56, 56, 56), Color.FromArgb(49, 49, 49)};
                    }
                    else
                    {
                        br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                        g.FillRectangle(br, rect);
                        cb.Colors = new Color[] { Color.FromArgb(248, 248, 248), Color.FromArgb(243, 243, 243), Color.FromArgb(241, 241, 241),
                        Color.FromArgb(239, 239, 239), Color.FromArgb(237, 237, 237), Color.FromArgb(235, 235, 235),
                        Color.FromArgb(233, 233, 233), Color.FromArgb(231, 231, 231), Color.FromArgb(229, 229, 229), 
                        Color.FromArgb(227, 227, 227), Color.FromArgb(225, 225, 225), Color.FromArgb(223, 223, 223)};
                    }
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
                {
                    cb.Colors = Off2010Colors.MouseDownColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(255, 244, 200), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114),
                        Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114),
                        Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), 
                        Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(255, 244, 200), Color.FromArgb(255, 244, 205), Color.FromArgb(255, 237, 159),
                        Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159),
                        Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), 
                        Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 244, 200)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                #endregion
            }
        }

        /// <summary>
        /// Returns the Header Border Colors.
        /// </summary>
        /// <param name="clrBottom">The bottom border color</param>
        /// <param name="clrRight">The right border color</param>
        /// <param name="clrInteriorFirst">The gradient start color for the header interior</param>
        /// <param name="clrInteriorLast">The gradient end color for the header interior</param>
        /// <returns></returns>
        public bool GetHeaderBorderColors(out Color clrBottom, out Color clrRight, out Color clrInteriorFirst, out Color clrInteriorLast)
        {
            if (this.isLegacyStyle)
            {
                if (this.visualStyle == GridVisualStyles.Office2010Blue)
                {
                    clrBottom = Color.FromArgb(160, 176, 199);
                    clrRight = Color.FromArgb(160, 176, 199);
                    clrInteriorFirst = Color.FromArgb(199, 222, 255);
                    clrInteriorLast = Color.FromArgb(249, 252, 255);
                }
                else if (this.visualStyle == GridVisualStyles.Office2010Black)
                {
                    clrBottom = Color.FromArgb(44, 44, 44);
                    clrRight = Color.FromArgb(44, 44, 44);
                    clrInteriorFirst = Color.FromArgb(106, 106, 106);
                    clrInteriorLast = Color.FromArgb(200, 200, 200);
                }
                else
                {
                    clrBottom = Color.FromArgb(147, 154, 163);
                    clrRight = Color.FromArgb(147, 154, 163);
                    clrInteriorFirst = Color.FromArgb(147, 154, 163);
                    clrInteriorLast = Color.FromArgb(249, 252, 255);
                }
                return true;
            }
            else
            {
                if (this.visualStyle == GridVisualStyles.Office2010Blue)
                {

                    clrBottom = Color.FromArgb(174, 185, 200);
                    clrRight = Color.FromArgb(174, 185, 200);// new code
                    clrInteriorFirst = Color.FromArgb(241, 252, 255);
                    clrInteriorLast = Color.FromArgb(241, 252, 255);// new code

                }
                else if (this.visualStyle == GridVisualStyles.Office2010Black)
                {
                    clrBottom = Color.FromArgb(44, 44, 44);
                    clrRight = Color.FromArgb(44, 44, 44);
                    clrInteriorFirst = Color.FromArgb(106, 106, 106);
                    clrInteriorLast = Color.FromArgb(200, 200, 200);
                }
                else
                {

                    clrBottom = Color.FromArgb(182, 182, 182);
                    clrRight = Color.FromArgb(182, 182, 182);
                    clrInteriorFirst = Color.FromArgb(223, 223, 223);
                    clrInteriorLast = Color.FromArgb(248, 248, 248);
                }
                return true;
            }
        }

        /// <summary>
        ///  Returns the Header Border styles.
        /// </summary>
        /// <param name="clrHeaderBottom">The bottom border color of header.</param>
        /// <param name="borderWeight">The bottom border weight.</param>
        /// <returns>returns the weight and color of bottom border.</returns>
        public bool GetHeaderBottomBorderStyle(out Color clrHeaderBottomBorder, out GridBottomBorderWeight borderWeight)
        {
            clrHeaderBottomBorder = Color.Empty;
            borderWeight = GridBottomBorderWeight.ExtraThin;
            return true;
        }

        /// <summary>
        /// Returns the SortIcon interior
        /// </summary>
        /// <param name="brush">The brush used to fill the sort icon</param>
        /// <param name="pen">The pen used to draw the sort icon</param>
        /// <returns></returns>
        public void GetSortIconBrush(out Brush brush, out Pen pen)
        {
            if (this.isLegacyStyle)
            {
                if (this.visualStyle == GridVisualStyles.Office2010Blue)
                {
                    brush = new SolidBrush(Color.FromArgb(101, 147, 207));
                    pen = new Pen(Color.FromArgb(101, 147, 207));
                }
                else if (this.visualStyle == GridVisualStyles.Office2010Black)
                {
                    if (Headerstate == ThemedHeaderDrawing.HeaderState.Normal)
                    {
                        brush = new SolidBrush(Color.White);
                        pen = new Pen(Color.White);
                    }
                    else
                    {
                        brush = new SolidBrush(SystemColors.WindowText);
                        pen = new Pen(SystemColors.WindowText);
                    }
                }
                else if (this.visualStyle == GridVisualStyles.Office2010Silver)
                {
                    brush = new SolidBrush(Color.FromArgb(71, 52, 142));
                    pen = new Pen(Color.FromArgb(71, 52, 142));
                }
                else
                {
                    brush = new SolidBrush(Color.FromArgb(165, 172, 181));
                    pen = new Pen(Color.FromArgb(165, 172, 181));
                }
            }
            else
            {
                if (this.visualStyle == GridVisualStyles.Office2010Blue)
                {
                    brush = new SolidBrush(Color.FromArgb(53, 92, 175));
                    pen = new Pen(Color.FromArgb(53, 92, 175));
                }
                else if (this.visualStyle == GridVisualStyles.Office2010Black)
                {
                    if (Headerstate == ThemedHeaderDrawing.HeaderState.Normal)
                    {
                        brush = new SolidBrush(Color.White);
                        pen = new Pen(Color.White);
                    }
                    else
                    {
                        brush = new SolidBrush(SystemColors.WindowText);
                        pen = new Pen(SystemColors.WindowText);
                    }
                }
                else
                {
                    brush = new SolidBrush(Color.FromArgb(51, 51, 51));
                    pen = new Pen(Color.FromArgb(51, 51, 51));
                }
            }
        }

        /// <summary>
        /// Returns the backcolor and header interior for GroupDropArea.
        /// </summary>
        /// <param name="backColor">The back color for GroupDropArea</param>
        /// <param name="headerBorderTop">The top border color for GroupDropArea header</param>
        /// <param name="headerBorderLeft">The left border color for GroupDropArea header</param>
        /// <returns></returns>
        public bool GetGroupDropAreaColors(out Color backColor, out Color headerBorderTop, out Color headerBorderLeft)
        {
            if (this.isLegacyStyle)
            {
                if (this.visualStyle == GridVisualStyles.Office2010Blue)
                {
                    backColor = Color.FromArgb(227, 239, 255);
                    headerBorderTop = Color.FromArgb(101, 147, 207);
                    headerBorderLeft = Color.FromArgb(101, 147, 207);
                }
                else if (this.visualStyle == GridVisualStyles.Office2010Black)
                {
                    backColor = Color.FromArgb(100, 100, 110);
                    headerBorderTop = Color.FromArgb(182, 182, 182);
                    headerBorderLeft = Color.FromArgb(182, 182, 182);
                }
                else
                {
                    backColor = Color.FromArgb(223, 227, 222);
                    headerBorderTop = Color.FromArgb(144, 145, 146);
                    headerBorderLeft = Color.FromArgb(144, 145, 146);
                }
                return true;
            }
            else
            {
                if (this.visualStyle == GridVisualStyles.Office2010Blue)
                {
                    backColor = Color.FromArgb(201, 226, 252);
                    headerBorderTop = Color.FromArgb(101, 147, 207);
                    headerBorderLeft = Color.FromArgb(101, 147, 207);
                }
                else if (this.visualStyle == GridVisualStyles.Office2010Black)
                {
                    backColor = Color.FromArgb(93, 93, 93);
                    headerBorderTop = Color.FromArgb(182, 182, 182);
                    headerBorderLeft = Color.FromArgb(182, 182, 182);
                }
                else
                {
                    backColor = Color.FromArgb(233, 233, 233);
                    headerBorderTop = Color.FromArgb(144, 145, 146);
                    headerBorderLeft = Color.FromArgb(144, 145, 146);
                }
                return true;
            }
        }

        /// <summary>
        /// Header style color of Grid
        /// </summary>
        /// <param name="backColor">Header color</param>
        /// <param name="hoverColor">hover color</param>
        /// <param name="pressedColor">pressed color</param>
        /// <returns></returns>
        public bool GetHeaderColors(out Color backColor, out Color hoverColor, out Color pressedColor)
        {
            backColor = Color.Empty;
            hoverColor = Color.Empty;
            pressedColor = Color.Empty;
            return true;
        }

        public bool GetHeaderTextColors(out Color normalTextColor, out Color hoverTextColor,out Color pressedTextColor)
        {
            normalTextColor = Color.White;
            hoverTextColor = SystemColors.WindowText;
            pressedTextColor = SystemColors.WindowText;
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
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    if (state == ButtonState.Flat)
                    {
                        rect.Inflate(-1, -1);
                        if (this.visualStyle == GridVisualStyles.Office2010Blue)
                        {
                            DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderBlue, Off2010Colors.TopFirstBlue, Off2010Colors.TopLastBlue,
                                Off2010Colors.BottomFirstBlue, Off2010Colors.BottomLastBlue, Off2010Colors.BottomLineBlue);
                        }
                        else if (this.visualStyle == GridVisualStyles.Office2010Black)
                        {
                            DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderBlack, Off2010Colors.TopFirstBlack, Off2010Colors.TopLastBlack,
                                Off2010Colors.BottomFirstBlack, Off2010Colors.BottomLastBlack, Off2010Colors.BottomLineBlack);
                        }
                        else
                        {
                            DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderSilver, Off2010Colors.TopFirstSilver, Off2010Colors.TopLastSilver,
                                Off2010Colors.BottomFirstSilver, Off2010Colors.BottomLastSilver, Off2010Colors.BottomLineSilver);
                        }
                    }
                    else if (state == ButtonState.Normal)
                    {
                        rect.Inflate(-1, -1);
                        DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderHover, Off2010Colors.TopFirstHoverColor, Off2010Colors.TopLastHoverColor,
                            Off2010Colors.BottomFirstHoverColor, Off2010Colors.BottomLastHoverColor, Off2010Colors.BottomLineHoverColor);
                    }
                    else if (state == ButtonState.Pushed)
                    {
                        rect.Inflate(-1, -1);
                        DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderClicked, Off2010Colors.TopFirstClickedColor, Off2010Colors.TopLastClickedColor,
                            Off2010Colors.BottomFirstClickedColor, Off2010Colors.BottomLastClickedColor, Off2010Colors.BottomLineClickedColor);
                    }
                }
                catch
                { }
            }
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    if (state == ButtonState.Flat)
                    {
                        rect.Inflate(-1, -1);
                        if (this.visualStyle == GridVisualStyles.Office2010Blue)
                        {

                            DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderBlue, Color.FromArgb(232, 241, 252), Color.FromArgb(233, 241, 252),
                               Color.FromArgb(218, 231, 247), Color.FromArgb(229, 238, 251), Off2010Colors.BottomLineBlue);//new code
                        }
                        else if (this.visualStyle == GridVisualStyles.Office2010Black)
                        {

                            DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderBlack, Color.FromArgb(201, 201, 201), Color.FromArgb(201, 201, 201),
                              Color.FromArgb(181, 181, 181), Color.FromArgb(181, 181, 181), Off2010Colors.BottomLineBlack);//new code
                        }
                        else
                        {
                            DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderSilver, Off2010Colors.TopFirstSilver, Off2010Colors.TopLastSilver,
                                Off2010Colors.BottomFirstSilver, Off2010Colors.BottomLastSilver, Off2010Colors.BottomLineSilver);
                        }
                    }
                    else if (state == ButtonState.Normal)
                    {
                        if (this.visualStyle == GridVisualStyles.Office2010Blue)
                        {
                            rect.Inflate(-1, -1);

                            DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                           Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                        }

                        else if (this.visualStyle == GridVisualStyles.Office2010Black)
                        {
                            rect.Inflate(-1, -1);
                            DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);//new code
                        }
                        else
                        {
                            rect.Inflate(-1, -1);
                            DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                        }
                    }
                    else
                    {
                        rect.Inflate(-1, -1);
                        DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderClicked, Off2010Colors.TopFirstClickedColor, Off2010Colors.TopLastClickedColor,
                            Off2010Colors.BottomFirstClickedColor, Off2010Colors.BottomLastClickedColor, Off2010Colors.BottomLineClickedColor);

                    }
                }
                catch
                { }
            }
        }

        /// <summary>
        /// Draws the ComboBox skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the combo button.</param>
        public void DrawComboBoxStyle(Graphics g, Rectangle rect, ThemedComboBoxDrawing.DropDownState state, Color clrBack)
        {
            if (this.isLegacyStyle)
            {
                Point ptOffset = Point.Empty;

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ThemedComboBoxDrawing.DropDownState.Normal)
                {
                    rect.Inflate(-1, -1);
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();

                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderHover, Off2010Colors.TopFirstHoverColor, Off2010Colors.TopLastHoverColor,
                        Off2010Colors.BottomFirstHoverColor, Off2010Colors.BottomLastHoverColor, Off2010Colors.BottomLineHoverColor);
                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderClicked, Off2010Colors.TopFirstClickedColor, Off2010Colors.TopLastClickedColor,
                        Off2010Colors.BottomFirstClickedColor, Off2010Colors.BottomLastClickedColor, Off2010Colors.BottomLineClickedColor);
                }

                string bitmapName = "Down.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
            else
            {
                Point ptOffset = Point.Empty;

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ThemedComboBoxDrawing.DropDownState.Normal)
                {
                    rect.Inflate(-1, -1);
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();

                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderHover, Off2010Colors.TopFirstHoverColor, Off2010Colors.TopLastHoverColor,
                        Off2010Colors.BottomFirstHoverColor, Off2010Colors.BottomLastHoverColor, Off2010Colors.BottomLineHoverColor);
                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderClicked, Off2010Colors.TopFirstClickedColor, Off2010Colors.TopLastClickedColor,
                        Off2010Colors.BottomFirstClickedColor, Off2010Colors.BottomLastClickedColor, Off2010Colors.BottomLineClickedColor);
                }

                string bitmapName = "Down.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
        }

        /// <summary>
        /// Draws the SpinButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="btnId">An integer that represents the type of the button.</param>
        /// <param name="btnState">The current state of the spin button.</param>
        public void DrawSpinButtonStyle(Graphics g, Rectangle rect, ButtonID btnId, ButtonState btnState, Color clrBack)
        {
            if (this.isLegacyStyle)
            {
                Point ptOffset = Point.Empty;
                if (btnId == ButtonID.Up)
                    ptOffset = new Point(-1, -1);

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (btnState == ButtonState.Flat)
                {
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else if (btnState == ButtonState.Normal)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderHover, Off2010Colors.TopFirstHoverColor, Off2010Colors.TopLastHoverColor,
                        Off2010Colors.BottomFirstHoverColor, Off2010Colors.BottomLastHoverColor, Off2010Colors.BottomLineHoverColor);
                }
                else if (btnState == ButtonState.Pushed)
                {
                    if (btnId == ButtonID.Up)
                        ptOffset = new Point(0, 0);
                    else
                        ptOffset = new Point(1, 1);

                    DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderClicked, Off2010Colors.TopFirstClickedColor, Off2010Colors.TopLastClickedColor,
                        Off2010Colors.BottomFirstClickedColor, Off2010Colors.BottomLastClickedColor, Off2010Colors.BottomLineClickedColor);
                }

                string bitmapName;

                if (btnId == ButtonID.Down)
                    bitmapName = "Down.png";
                else
                    bitmapName = "Up.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
            else
            {
                Point ptOffset = Point.Empty;
                if (btnId == ButtonID.Up)
                    ptOffset = new Point(-1, -1);

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (btnState == ButtonState.Flat)
                {
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else if (btnState == ButtonState.Normal)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderHover, Off2010Colors.TopFirstHoverColor, Off2010Colors.TopLastHoverColor,
                        Off2010Colors.BottomFirstHoverColor, Off2010Colors.BottomLastHoverColor, Off2010Colors.BottomLineHoverColor);
                }
                else if (btnState == ButtonState.Pushed)
                {
                    if (btnId == ButtonID.Up)
                        ptOffset = new Point(0, 0);
                    else
                        ptOffset = new Point(1, 1);

                    DrawingUtils.PaintButtonGradient(g, rect, Off2010Colors.BorderClicked, Off2010Colors.TopFirstClickedColor, Off2010Colors.TopLastClickedColor,
                        Off2010Colors.BottomFirstClickedColor, Off2010Colors.BottomLastClickedColor, Off2010Colors.BottomLineClickedColor);
                }

                string bitmapName;

                if (btnId == ButtonID.Down)
                    bitmapName = "Down.png";
                else
                    bitmapName = "Up.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
        }

        private Pen GetOffice2010BorderPen(ButtonState buttonState)
        {
            Pen pen;
            switch (visualStyle)
            {
                case GridVisualStyles.Office2010Black:
                    if ((buttonState & ButtonState.Inactive) > 0)
                        pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlackInactive);
                    else if ((buttonState & ButtonState.Flat) > 0)
                        pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                    else if ((buttonState & ButtonState.Pushed) > 0)
                        pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                    else
                        pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlackNormal);
                    break;
                case GridVisualStyles.Office2010Blue:
                    if ((buttonState & ButtonState.Inactive) > 0)
                        pen = new Pen(Off2010Colors.CheckBoxOuterBorderBlueInactive);
                    else if ((buttonState & ButtonState.Flat) > 0)
                        pen = new Pen(Off2010Colors.CheckBoxOuterBorderColorHot);
                    else if ((buttonState & ButtonState.Pushed) > 0)
                        pen = new Pen(Off2010Colors.CheckBoxOuterBorderColorHot);
                    else
                        pen = new Pen(Off2010Colors.CheckBoxOuterBorderBlueNormal);
                    break;
                default:
                    if ((buttonState & ButtonState.Inactive) > 0)
                        pen = new Pen(Off2007Colors.CheckBoxOuterBorderSilverInactive);
                    else if ((buttonState & ButtonState.Flat) > 0)
                        pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                    else if ((buttonState & ButtonState.Pushed) > 0)
                        pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                    else
                        pen = new Pen(Off2007Colors.CheckBoxOuterBorderSilverNormal);
                    break;
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
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    Pen borderPen = this.GetOffice2010BorderPen(state);
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Inactive) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxInnerBorderInactive;
                        color = Off2010Colors.CheckBoxFillInactive;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxInnerBorderColorHot;
                        color = Off2010Colors.CheckBoxFillColorHot;
                    }
                    else if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxPushedBorderColor;
                        color = Off2010Colors.CheckBoxPushedFillColor;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen checkPen = new Pen(Off2010Colors.checkColor, 2);

                        Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
													 new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
													 new Point( innerRect.Right - 1, innerRect.Top -1 )
												 };

                        SmoothingMode prevMode = g.SmoothingMode;

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(checkPen, points);
                        g.SmoothingMode = prevMode;
                        checkPen.Dispose();
                    }

                    if (mixedState && ((state & ButtonState.Inactive) <= 0))
                    {
                        LinearGradientBrush br = new LinearGradientBrush(innerRect, Color.FromArgb(123, 203, 231), Color.FromArgb(33, 89, 140), LinearGradientMode.Vertical);
                        Pen pen1 = new Pen(Color.FromArgb(41, 97, 140));
                        g.DrawRectangle(pen1, innerRect.X - 1, innerRect.Y - 1, innerRect.Width + 1, innerRect.Height + 1);

                        g.FillRectangle(br, innerRect);
                        pen1.Dispose();
                        br.Dispose();
                    }

                    borderPen.Dispose();
                    pen.Dispose();
                    brush.Dispose();
                    innerBrush.Dispose();
                }
                catch { }
            }
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    Pen borderPen = this.GetOffice2010BorderPen(state);
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Inactive) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxInnerBorderInactive;
                        color = Off2010Colors.CheckBoxFillInactive;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxInnerBorderColorHot;
                        color = Off2010Colors.CheckBoxFillColorHot;
                    }
                    else if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxPushedBorderColor;
                        color = Off2010Colors.CheckBoxPushedFillColor;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen checkPen = new Pen(Off2010Colors.checkColor, 2);

                        Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
													 new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
													 new Point( innerRect.Right - 1, innerRect.Top -1 )
												 };

                        SmoothingMode prevMode = g.SmoothingMode;

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(checkPen, points);
                        g.SmoothingMode = prevMode;
                        checkPen.Dispose();
                    }

                    if (mixedState && ((state & ButtonState.Inactive) <= 0))
                    {
                        LinearGradientBrush br = new LinearGradientBrush(innerRect, Color.FromArgb(123, 203, 231), Color.FromArgb(33, 89, 140), LinearGradientMode.Vertical);
                        Pen pen1 = new Pen(Color.FromArgb(41, 97, 140));
                        g.DrawRectangle(pen1, innerRect.X - 1, innerRect.Y - 1, innerRect.Width + 1, innerRect.Height + 1);

                        g.FillRectangle(br, innerRect);
                        pen1.Dispose();
                        br.Dispose();
                    }

                    borderPen.Dispose();
                    pen.Dispose();
                    brush.Dispose();
                    innerBrush.Dispose();
                }
                catch { }
            }
        }

        /// <summary>
        /// Draws the RadioButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the radio button</param>
        public void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state)//, bool mixedState)
        {
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    rect.Width -= 2;
                    rect.Height -= 4;

                    int outRectSize = rect.Height;
                    if (rect.Height > 12)
                        outRectSize = 12;

                    Pen borderPen = this.GetOffice2010BorderPen(state);
                    g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxPushedBorderColor;
                        color = Off2010Colors.CheckBoxPushedFillColor;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxInnerBorderColorHot;
                        color = Off2010Colors.CheckBoxFillColorHot;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen pen1 = new Pen(Color.FromArgb(30, 75, 105), 1.8f);
                        g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                        innerRect.Inflate(-1, -1);
                        Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                        LinearGradientBrush br = new LinearGradientBrush(innMost, Color.FromArgb(198, 235, 254), Color.FromArgb(8, 130, 198), LinearGradientMode.ForwardDiagonal);
                        g.FillEllipse(br, innMost);
                        br.Dispose();
                        pen1.Dispose();
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
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    rect.Width -= 2;
                    rect.Height -= 4;

                    int outRectSize = rect.Height;
                    if (rect.Height > 12)
                        outRectSize = 12;

                    Pen borderPen = this.GetOffice2010BorderPen(state);
                    g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxPushedBorderColor;
                        color = Off2010Colors.CheckBoxPushedFillColor;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2010Colors.CheckBoxInnerBorderColorHot;
                        color = Off2010Colors.CheckBoxFillColorHot;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen pen1 = new Pen(Color.FromArgb(30, 75, 105), 1.8f);
                        g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                        innerRect.Inflate(-1, -1);
                        Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                        LinearGradientBrush br = new LinearGradientBrush(innMost, Color.FromArgb(198, 235, 254), Color.FromArgb(8, 130, 198), LinearGradientMode.ForwardDiagonal);
                        g.FillEllipse(br, innMost);
                        br.Dispose();
                        pen1.Dispose();
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
        }


        /// <summary>
        /// Gets the current VisualStyles.
        /// </summary>
        public GridVisualStyles VisualStyle
        {
            get { return this.visualStyle; }
        }

        #endregion
    }

    # endregion

    # region Office2007Blue
    /// <summary>
	/// Implements the Office 2007 Blue look and feel.
	/// </summary>
	public class GridVisualStylesOffice2007Blue : Disposable, IVisualStylesDrawing
	{
		private GridVisualStyles visualStyle;
        private bool isLegacyStyle;
		static Syncfusion.Drawing.IconPaint iconPainter = null;

		/// <summary>
		/// Creates a new instance of <see cref="GridVisualStylesOffice2007Blue"/> class.
		/// </summary>
		/// <param name="style">The current visual style.</param>
		public GridVisualStylesOffice2007Blue(GridVisualStyles style)
		{
			this.visualStyle = style;

			if (iconPainter == null)
				iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
		}
        public GridVisualStylesOffice2007Blue(GridVisualStyles style, bool legacyStyle)
        {
            this.visualStyle = style;
            this.isLegacyStyle = legacyStyle;

            if (iconPainter == null)
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
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
            if (this.isLegacyStyle)
            {
                ColorBlend cb = new ColorBlend(12);
                cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };

                //Check for empty headers
                if (rect.Height == 0 && rect.Width == 0)
                    return;

                //Check for the current state of the header and paints the foreground accordingly.

                if (state == ThemedHeaderDrawing.HeaderState.Normal)
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(249, 252, 255), Color.FromArgb(197, 222, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
                {
                    cb.Colors = Off2007Colors.MouseDownColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.Orange, Color.Firebrick, LinearGradientMode.Vertical);
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                else
                {
                    cb.Colors = Off2007Colors.MouseHOverColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 254, 228), Color.FromArgb(255, 230, 159), LinearGradientMode.Vertical);
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
            }
            else
            {
                ColorBlend cb = new ColorBlend(12);
                cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };

                //Check for empty headers
                if (rect.Height == 0 && rect.Width == 0)
                    return;

                //Check for the current state of the header and paints the foreground accordingly.

                if (state == ThemedHeaderDrawing.HeaderState.Normal)
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(230, 240, 250), Color.FromArgb(232, 242, 252), Color.FromArgb(228, 238, 252),
                    Color.FromArgb(225, 237, 252), Color.FromArgb(222, 235, 251), Color.FromArgb(219, 233, 251),
                    Color.FromArgb(216, 231, 251), Color.FromArgb(212, 229, 251), Color.FromArgb(210, 227, 250), 
                    Color.FromArgb(210, 227, 249), Color.FromArgb(209, 226, 247), Color.FromArgb(208, 225, 245)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
                {
                    cb.Colors = Off2007Colors.MouseDownColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(255, 244, 200), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114),
                    Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114),
                    Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), 
                    Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else
                {
                    cb.Colors = Off2007Colors.MouseHOverColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(255, 244, 200), Color.FromArgb(255, 244, 205), Color.FromArgb(255, 237, 159),
                    Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159),
                    Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), 
                    Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 244, 200)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
            }
		}

        /// <summary>
        /// Returns the Header Border Colors.
        /// </summary>
        /// <param name="clrBottom">The bottom border color</param>
        /// <param name="clrRight">The right border color</param>
        /// <param name="clrInteriorFirst">The gradient start color for the header interior</param>
        /// <param name="clrInteriorLast">The gradient end color for the header interior</param>
        /// <returns></returns>
		public bool GetHeaderBorderColors(out Color clrBottom, out Color clrRight, out Color clrInteriorFirst, out Color clrInteriorLast)
		{
            if (this.isLegacyStyle)
            {
                clrBottom = Color.FromArgb(101, 147, 207);
                clrRight = Color.FromArgb(101, 147, 207);
                clrInteriorFirst = Color.FromArgb(199, 222, 255);
                clrInteriorLast = Color.FromArgb(249, 252, 255);
                return true;
            }
            else
            {
                clrBottom = Color.FromArgb(132, 157, 189);
                clrRight = Color.FromArgb(132, 157, 189);
                clrInteriorFirst = Color.FromArgb(199, 222, 255);
                clrInteriorLast = Color.FromArgb(249, 252, 255);
                return true;
            }
		}

        /// <summary>
        /// Returns the SortIcon interior
        /// </summary>
        /// <param name="brush">The brush used to fill the sort icon</param>
        /// <param name="pen">The pen used to draw the sort icon</param>
        /// <returns></returns>
		public void GetSortIconBrush(out Brush brush, out Pen pen)
		{
            if (this.isLegacyStyle)
            {
                brush = new SolidBrush(Color.FromArgb(101, 147, 207));
                pen = new Pen(Color.FromArgb(101, 147, 207));
            }
            else
            {
                brush = new SolidBrush(Color.FromArgb(21, 66, 139));
                pen = new Pen(Color.FromArgb(21, 66, 139));
            }
		}

        /// <summary>
        /// Returns the backcolor and header interior for GroupDropArea.
        /// </summary>
        /// <param name="backColor">The back color for GroupDropArea</param>
        /// <param name="headerBorderTop">The top border color for GroupDropArea header</param>
        /// <param name="headerBorderLeft">The left border color for GroupDropArea header</param>
        /// <returns></returns>
		public bool GetGroupDropAreaColors(out Color backColor, out Color headerBorderTop, out Color headerBorderLeft)
		{
            if (this.isLegacyStyle)
            {
                backColor = Color.FromArgb(227, 239, 255);
                headerBorderTop = Color.FromArgb(101, 147, 207);
                headerBorderLeft = Color.FromArgb(101, 147, 207);
                return true;
            }
            else
            {
                backColor = Color.FromArgb(222, 235, 251);
                headerBorderTop = Color.FromArgb(101, 147, 207);
                headerBorderLeft = Color.FromArgb(101, 147, 207);
                return true;
            }
		}

        /// <summary>
        /// Draws the PushButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the button.</param>
		public void DrawPushButtonStyle(Graphics g, Rectangle rect, ButtonState state)
		{
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    if (state == ButtonState.Flat)
                    {
                        rect.Inflate(-1, -1);
                        DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderBlue, Off2007Colors.TopFirstBlue, Off2007Colors.TopLastBlue,
                            Off2007Colors.BottomFirstBlue, Off2007Colors.BottomLastBlue, Off2007Colors.BottomLineBlue);
                    }
                    else if (state == ButtonState.Normal)
                    {
                        rect.Inflate(-1, -1);
                        DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                            Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                    }
                    else if (state == ButtonState.Pushed)
                    {
                        rect.Inflate(-1, -1);
                        DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                            Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                    }
                }
                catch
                { }
            }
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    if (state == ButtonState.Flat)
                    {
                        rect.Inflate(-1, -1);
                        DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderBlue, Off2007Colors.TopFirstBlue, Off2007Colors.TopLastBlue,
                            Off2007Colors.BottomFirstBlue, Off2007Colors.BottomLastBlue, Off2007Colors.BottomLineBlue);
                    }
                    else if (state == ButtonState.Normal)
                    {
                        rect.Inflate(-1, -1);
                        DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                            Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                    }
                    else if (state == ButtonState.Pushed)
                    {
                        rect.Inflate(-1, -1);
                        DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                            Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                    }
                }
                catch
                { }
            }

		}

        /// <summary>
        /// Draws the ComboBox skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the combo button.</param>
		public void DrawComboBoxStyle(Graphics g, Rectangle rect, ThemedComboBoxDrawing.DropDownState state, Color clrBack)
		{
            if (this.isLegacyStyle)
            {
                Point ptOffset = Point.Empty;

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ThemedComboBoxDrawing.DropDownState.Normal)
                {
                    rect.Inflate(-1, -1);
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();

                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName = "Down.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
            else
            {
                Point ptOffset = Point.Empty;

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ThemedComboBoxDrawing.DropDownState.Normal)
                {
                    rect.Inflate(-1, -1);
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();

                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName = "Down.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
		}

        /// <summary>
        /// Draws the SpinButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="btnId">An integer that represents the type of the button.</param>
        /// <param name="btnState">The current state of the spin button.</param>
		public void DrawSpinButtonStyle(Graphics g, Rectangle rect, ButtonID btnId, ButtonState btnState, Color clrBack)
		{
            if (this.isLegacyStyle)
            {
                Point ptOffset = Point.Empty;
                if (btnId == ButtonID.Up)
                    ptOffset = new Point(-1, -1);

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (btnState == ButtonState.Flat)
                {
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else if (btnState == ButtonState.Normal)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (btnState == ButtonState.Pushed)
                {
                    if (btnId == ButtonID.Up)
                        ptOffset = new Point(0, 0);
                    else
                        ptOffset = new Point(1, 1);

                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName;

                if (btnId == ButtonID.Down)
                    bitmapName = "Down.png";
                else
                    bitmapName = "Up.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
            else
            {
                Point ptOffset = Point.Empty;
                if (btnId == ButtonID.Up)
                    ptOffset = new Point(-1, -1);

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (btnState == ButtonState.Flat)
                {
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else if (btnState == ButtonState.Normal)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (btnState == ButtonState.Pushed)
                {
                    if (btnId == ButtonID.Up)
                        ptOffset = new Point(0, 0);
                    else
                        ptOffset = new Point(1, 1);

                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName;

                if (btnId == ButtonID.Down)
                    bitmapName = "Down.png";
                else
                    bitmapName = "Up.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
		}

		private Pen GetOffice2007BorderPen(ButtonState buttonState)
		{
			Pen pen;
            if (this.isLegacyStyle)
            {
                if ((buttonState & ButtonState.Inactive) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlueInactive);
                else if ((buttonState & ButtonState.Flat) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else if ((buttonState & ButtonState.Pushed) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlueNormal);
            }
            else
            {
                if ((buttonState & ButtonState.Inactive) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlueInactive);
                else if ((buttonState & ButtonState.Flat) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else if ((buttonState & ButtonState.Pushed) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlueNormal);
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
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Inactive) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderInactive;
                        color = Off2007Colors.CheckBoxFillInactive;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen checkPen = new Pen(Off2007Colors.checkColor, 2);

                        Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
													 new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
													 new Point( innerRect.Right - 1, innerRect.Top -1 )
												 };

                        SmoothingMode prevMode = g.SmoothingMode;

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(checkPen, points);
                        g.SmoothingMode = prevMode;
                        checkPen.Dispose();
                    }

                    if (mixedState && ((state & ButtonState.Inactive) <= 0))
                    {
                        LinearGradientBrush br = new LinearGradientBrush(innerRect, Color.FromArgb(123, 203, 231), Color.FromArgb(33, 89, 140), LinearGradientMode.Vertical);
                        Pen pen1 = new Pen(Color.FromArgb(41, 97, 140));
                        g.DrawRectangle(pen1, innerRect.X - 1, innerRect.Y - 1, innerRect.Width + 1, innerRect.Height + 1);

                        g.FillRectangle(br, innerRect);
                        pen1.Dispose();
                        br.Dispose();
                    }

                    borderPen.Dispose();
                    pen.Dispose();
                    brush.Dispose();
                    innerBrush.Dispose();
                }
                catch { }
            }
            else
            {

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Inactive) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderInactive;
                        color = Off2007Colors.CheckBoxFillInactive;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen checkPen = new Pen(Off2007Colors.checkColor, 2);

                        Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
													 new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
													 new Point( innerRect.Right - 1, innerRect.Top -1 )
												 };

                        SmoothingMode prevMode = g.SmoothingMode;

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(checkPen, points);
                        g.SmoothingMode = prevMode;
                        checkPen.Dispose();
                    }

                    if (mixedState && ((state & ButtonState.Inactive) <= 0))
                    {
                        LinearGradientBrush br = new LinearGradientBrush(innerRect, Color.FromArgb(123, 203, 231), Color.FromArgb(33, 89, 140), LinearGradientMode.Vertical);
                        Pen pen1 = new Pen(Color.FromArgb(41, 97, 140));
                        g.DrawRectangle(pen1, innerRect.X - 1, innerRect.Y - 1, innerRect.Width + 1, innerRect.Height + 1);

                        g.FillRectangle(br, innerRect);
                        pen1.Dispose();
                        br.Dispose();
                    }

                    borderPen.Dispose();
                    pen.Dispose();
                    brush.Dispose();
                    innerBrush.Dispose();
                }
                catch { }
            }
	
		}
		
		/// <summary>
        /// Draws the RadioButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the radio button</param>
		public void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state)//, bool mixedState)
		{
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    rect.Width -= 2;
                    rect.Height -= 4;

                    int outRectSize = rect.Height;
                    if (rect.Height > 12)
                        outRectSize = 12;

                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen pen1 = new Pen(Color.FromArgb(30, 75, 105), 1.8f);
                        g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                        innerRect.Inflate(-1, -1);
                        Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                        LinearGradientBrush br = new LinearGradientBrush(innMost, Color.FromArgb(198, 235, 254), Color.FromArgb(8, 130, 198), LinearGradientMode.ForwardDiagonal);
                        g.FillEllipse(br, innMost);
                        br.Dispose();
                        pen1.Dispose();
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
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    rect.Width -= 2;
                    rect.Height -= 4;

                    int outRectSize = rect.Height;
                    if (rect.Height > 12)
                        outRectSize = 12;

                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen pen1 = new Pen(Color.FromArgb(30, 75, 105), 1.8f);
                        g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                        innerRect.Inflate(-1, -1);
                        Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                        LinearGradientBrush br = new LinearGradientBrush(innMost, Color.FromArgb(198, 235, 254), Color.FromArgb(8, 130, 198), LinearGradientMode.ForwardDiagonal);
                        g.FillEllipse(br, innMost);
                        br.Dispose();
                        pen1.Dispose();
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
		}


        /// <summary>
        /// Gets the current VisualStyles.
        /// </summary>
        public GridVisualStyles VisualStyle
        {
            get { return this.visualStyle; }
        }

		#endregion
    }

	#endregion 

	# region Office2007Black
	/// <summary>
	/// Implements the Office 2007 Black look and feel
	/// </summary>
	public class GridVisualStylesOffice2007Black : Disposable, IVisualStylesDrawing
	{
		private GridVisualStyles visualStyle;
        private bool isLegacyStyle;
		static Syncfusion.Drawing.IconPaint iconPainter = null;

        /// <summary>
		/// Creates a new instance of <see cref="GridVisualStylesOffice2007Black"/> class.
		/// </summary>
		/// <param name="style">The current visual style.</param>
		public GridVisualStylesOffice2007Black(GridVisualStyles style)
		{
			this.visualStyle = style;

			if (iconPainter == null)
				iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
		}

        public GridVisualStylesOffice2007Black(GridVisualStyles style, bool legacyStyle)
        {
            this.visualStyle = style;
            this.isLegacyStyle = legacyStyle;

            if (iconPainter == null)
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
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
            if (this.isLegacyStyle)
            {
                ColorBlend cb = new ColorBlend(12);
                cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };

                if (rect.Height == 0 && rect.Width == 0)
                    return;

                if (state == ThemedHeaderDrawing.HeaderState.Normal)
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(248, 248, 248), Color.FromArgb(223, 223, 223), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
                {
                    cb.Colors = Off2007Colors.MouseDownColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.Orange, Color.Firebrick, LinearGradientMode.Vertical);
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                else
                {
                    cb.Colors = Off2007Colors.MouseHOverColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 254, 228), Color.FromArgb(255, 230, 159), LinearGradientMode.Vertical);
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
            }
            else
            {
                ColorBlend cb = new ColorBlend(12);
                cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };

                if (rect.Height == 0 && rect.Width == 0)
                    return;

                if (state == ThemedHeaderDrawing.HeaderState.Normal)
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(233, 233, 233), Color.FromArgb(233, 233, 233), Color.FromArgb(232, 232, 232),
                    Color.FromArgb(231, 231, 231), Color.FromArgb(230, 230, 230), Color.FromArgb(229, 229, 229),
                    Color.FromArgb(228, 228, 228), Color.FromArgb(227, 227, 227), Color.FromArgb(226, 226, 226), 
                    Color.FromArgb(225, 225, 225), Color.FromArgb(224, 224, 224), Color.FromArgb(223, 223, 223)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
                {
                    cb.Colors = Off2007Colors.MouseDownColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(255, 244, 200), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114),
                    Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114),
                    Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), 
                    Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else
                {
                    cb.Colors = Off2007Colors.MouseHOverColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(255, 244, 200), Color.FromArgb(255, 244, 205), Color.FromArgb(255, 237, 159),
                    Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159),
                    Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), 
                    Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 244, 200)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
            }
		}

        /// <summary>
        /// Returns the Header Border Colors.
        /// </summary>
        /// <param name="clrBottom">The bottom border color</param>
        /// <param name="clrRight">The right border color</param>
        /// <param name="clrInteriorFirst">The gradient start color for the header interior</param>
        /// <param name="clrInteriorLast">The gradient end color for the header interior</param>
        /// <returns></returns>
        public bool GetHeaderBorderColors(out Color clrBottom, out Color clrRight, out Color clrInteriorFirst, out Color clrInteriorLast)
        {
            if (this.isLegacyStyle)
            {
                clrBottom = Color.FromArgb(182, 182, 182);
                clrRight = Color.FromArgb(182, 182, 182);
                clrInteriorFirst = Color.FromArgb(223, 223, 223);
                clrInteriorLast = Color.FromArgb(248, 248, 248);
                return true;
            }
            else
            {
                clrBottom = Color.FromArgb(59, 59, 59);
                clrRight = Color.FromArgb(59, 59, 59);
                clrInteriorFirst = Color.FromArgb(223, 223, 223);
                clrInteriorLast = Color.FromArgb(248, 248, 248);
                return true;
            }
		}

        /// <summary>
        /// Returns the SortIcon interior
        /// </summary>
        /// <param name="brush">The brush used to fill the sort icon</param>
        /// <param name="pen">The pen used to draw the sort icon</param>
        /// <returns></returns>
		public void GetSortIconBrush(out Brush brush, out Pen pen)
		{
            if (this.isLegacyStyle)
            {
                brush = new SolidBrush(Color.FromArgb(182, 182, 182));
                pen = new Pen(Color.FromArgb(182, 182, 182));
            }
            else
            {
                brush = new SolidBrush(Color.FromArgb(145, 153, 164));
                pen = new Pen(Color.FromArgb(145, 153, 164));
            }
		}

        /// <summary>
        /// Returns the backcolor and header interior for GroupDropArea.
        /// </summary>
        /// <param name="backColor">The back color for GroupDropArea</param>
        /// <param name="headerBorderTop">The top border color for GroupDropArea header</param>
        /// <param name="headerBorderLeft">The left border color for GroupDropArea header</param>
        /// <returns></returns>
		public bool GetGroupDropAreaColors(out Color backColor, out Color headerBorderTop, out Color headerBorderLeft)
		{
            if (this.isLegacyStyle)
            {
                backColor = Color.FromArgb(240, 241, 242);
                headerBorderTop = Color.FromArgb(182, 182, 182);
                headerBorderLeft = Color.FromArgb(182, 182, 182);
                return true;
            }
            else
            {
                backColor = Color.FromArgb(227, 227, 227);
                headerBorderTop = Color.FromArgb(59, 59, 59);
                headerBorderLeft = Color.FromArgb(59, 59, 59);
                return true;
            }
		}

        /// <summary>
        /// Draws the PushButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the button.</param>
		public void DrawPushButtonStyle(Graphics g, Rectangle rect, ButtonState state)
		{
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ButtonState.Flat)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderBlack, Off2007Colors.TopFirstBlack, Off2007Colors.TopLastBlack,
                        Off2007Colors.BottomFirstBlack, Off2007Colors.BottomLastBlack, Off2007Colors.BottomLineBlack);
                }
                else if (state == ButtonState.Normal)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ButtonState.Pushed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }
            }
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ButtonState.Flat)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderBlack, Off2007Colors.TopFirstBlack, Off2007Colors.TopLastBlack,
                        Off2007Colors.BottomFirstBlack, Off2007Colors.BottomLastBlack, Off2007Colors.BottomLineBlack);
                }
                else if (state == ButtonState.Normal)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ButtonState.Pushed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }
            }
		}

        /// <summary>
        /// Draws the ComboBox skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the combo button.</param>
		public void DrawComboBoxStyle(Graphics g, Rectangle rect, ThemedComboBoxDrawing.DropDownState state, Color clrBack)
		{
            if (this.isLegacyStyle)
            {
                Point ptOffset = Point.Empty;

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ThemedComboBoxDrawing.DropDownState.Normal)
                {
                    rect.Inflate(-1, -1);
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();

                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName = "Down.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
            else
            {
                Point ptOffset = Point.Empty;

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ThemedComboBoxDrawing.DropDownState.Normal)
                {
                    rect.Inflate(-1, -1);
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();

                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName = "Down.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
		}

        /// <summary>
        /// Draws the SpinButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="btnId">An integer that represents the type of the button.</param>
        /// <param name="btnState">The current state of the spin button.</param>
		public void DrawSpinButtonStyle(Graphics g, Rectangle rect, ButtonID btnId, ButtonState btnState, Color clrBack)
		{
            if (this.isLegacyStyle)
            {
                Point ptOffset = Point.Empty;
                if (btnId == ButtonID.Up)
                    ptOffset = new Point(-1, -1);

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (btnState == ButtonState.Flat)
                {
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else if (btnState == ButtonState.Normal)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (btnState == ButtonState.Pushed)
                {
                    if (btnId == ButtonID.Up)
                        ptOffset = new Point(0, 0);
                    else
                        ptOffset = new Point(1, 1);

                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName;

                if (btnId == ButtonID.Down)
                    bitmapName = "Down.png";
                else
                    bitmapName = "Up.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
            else
            {
                Point ptOffset = Point.Empty;
                if (btnId == ButtonID.Up)
                    ptOffset = new Point(-1, -1);

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (btnState == ButtonState.Flat)
                {
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else if (btnState == ButtonState.Normal)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (btnState == ButtonState.Pushed)
                {
                    if (btnId == ButtonID.Up)
                        ptOffset = new Point(0, 0);
                    else
                        ptOffset = new Point(1, 1);

                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName;

                if (btnId == ButtonID.Down)
                    bitmapName = "Down.png";
                else
                    bitmapName = "Up.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
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
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Inactive) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderInactive;
                        color = Off2007Colors.CheckBoxFillInactive;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen checkPen = new Pen(Off2007Colors.checkColor, 2);

                        Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
													 new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
													 new Point( innerRect.Right - 1, innerRect.Top -1 )
												 };

                        SmoothingMode prevMode = g.SmoothingMode;

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(checkPen, points);
                        g.SmoothingMode = prevMode;
                        checkPen.Dispose();
                    }

                    if (mixedState && ((state & ButtonState.Inactive) <= 0))
                    {
                        LinearGradientBrush br = new LinearGradientBrush(innerRect, Color.FromArgb(123, 203, 231), Color.FromArgb(33, 89, 140), LinearGradientMode.Vertical);
                        Pen pen1 = new Pen(Color.FromArgb(41, 97, 140));
                        g.DrawRectangle(pen1, innerRect.X - 1, innerRect.Y - 1, innerRect.Width + 1, innerRect.Height + 1);

                        g.FillRectangle(br, innerRect);
                        pen1.Dispose();
                        br.Dispose();
                    }
                    borderPen.Dispose();
                    pen.Dispose();
                    brush.Dispose();
                    innerBrush.Dispose();
                }
                catch { }
            }
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Inactive) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderInactive;
                        color = Off2007Colors.CheckBoxFillInactive;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen checkPen = new Pen(Off2007Colors.checkColor, 2);

                        Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
													 new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
													 new Point( innerRect.Right - 1, innerRect.Top -1 )
												 };

                        SmoothingMode prevMode = g.SmoothingMode;

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(checkPen, points);
                        g.SmoothingMode = prevMode;
                        checkPen.Dispose();
                    }

                    if (mixedState && ((state & ButtonState.Inactive) <= 0))
                    {
                        LinearGradientBrush br = new LinearGradientBrush(innerRect, Color.FromArgb(123, 203, 231), Color.FromArgb(33, 89, 140), LinearGradientMode.Vertical);
                        Pen pen1 = new Pen(Color.FromArgb(41, 97, 140));
                        g.DrawRectangle(pen1, innerRect.X - 1, innerRect.Y - 1, innerRect.Width + 1, innerRect.Height + 1);

                        g.FillRectangle(br, innerRect);
                        pen1.Dispose();
                        br.Dispose();
                    }
                    borderPen.Dispose();
                    pen.Dispose();
                    brush.Dispose();
                    innerBrush.Dispose();
                }
                catch { }
            }

		}

        /// <summary>
        /// Draws the RadioButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the radio button</param>
		public void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state)//, bool mixedState)
		{
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;


                try
                {
                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    rect.Width -= 2;
                    rect.Height -= 4;

                    int outRectSize = rect.Height;
                    if (rect.Height > 12)
                        outRectSize = 12;

                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen pen1 = new Pen(Color.FromArgb(30, 75, 105), 1.8f);
                        g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                        innerRect.Inflate(-1, -1);
                        Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                        LinearGradientBrush br = new LinearGradientBrush(innMost, Color.FromArgb(198, 235, 254), Color.FromArgb(8, 130, 198), LinearGradientMode.ForwardDiagonal);
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
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;


                try
                {
                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    rect.Width -= 2;
                    rect.Height -= 4;

                    int outRectSize = rect.Height;
                    if (rect.Height > 12)
                        outRectSize = 12;

                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen pen1 = new Pen(Color.FromArgb(30, 75, 105), 1.8f);
                        g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                        innerRect.Inflate(-1, -1);
                        Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                        LinearGradientBrush br = new LinearGradientBrush(innMost, Color.FromArgb(198, 235, 254), Color.FromArgb(8, 130, 198), LinearGradientMode.ForwardDiagonal);
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

		}

		private Pen GetOffice2007BorderPen(ButtonState buttonState)
		{
			Pen pen;
            if (this.isLegacyStyle)
            {
                if ((buttonState & ButtonState.Inactive) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlackInactive);
                else if ((buttonState & ButtonState.Flat) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else if ((buttonState & ButtonState.Pushed) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlackNormal);
            }
            else
            {
                if ((buttonState & ButtonState.Inactive) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlackInactive);
                else if ((buttonState & ButtonState.Flat) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else if ((buttonState & ButtonState.Pushed) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderBlackNormal);
            }
			return pen;
		}

        /// <summary>
        /// Gets the current visual style.
        /// </summary>
		public GridVisualStyles VisualStyle
		{
			get { return this.visualStyle; }
		}
		#endregion
	}

	#endregion 

	# region Office2007Silver
	/// <summary>
	/// Implements the Office 2007 Silver look and feel
	/// </summary>
	public class GridVisualStylesOffice2007Silver : Disposable, IVisualStylesDrawing
	{
		private GridVisualStyles visualStyle;
        private bool isLegacyStyle;
		static Syncfusion.Drawing.IconPaint iconPainter = null;

        /// <summary>
		/// Creates a new instance of <see cref="GridVisualStylesOffice2007Blue"/> class.
		/// </summary>
		/// <param name="style">The current visual style.</param>
       	public GridVisualStylesOffice2007Silver(GridVisualStyles style)
		{
			this.visualStyle = style;

			if (iconPainter == null)
				iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
		}

        public GridVisualStylesOffice2007Silver(GridVisualStyles style, bool legacyStyle)
        {
            this.visualStyle = style;
            this.isLegacyStyle = legacyStyle;

            if (iconPainter == null)
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
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
            if (this.isLegacyStyle)
            {
                ColorBlend cb = new ColorBlend(12);
                cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };

                if (rect.Height == 0 && rect.Width == 0)
                    return;

                if (state == ThemedHeaderDrawing.HeaderState.Normal)
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(241, 243, 243), Color.FromArgb(200, 201, 202), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
                {
                    cb.Colors = Off2007Colors.MouseDownColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.Orange, Color.Firebrick, LinearGradientMode.Vertical);
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
                else
                {
                    cb.Colors = Off2007Colors.MouseHOverColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 254, 228), Color.FromArgb(255, 230, 159), LinearGradientMode.Vertical);
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
            }
            else
            {
                ColorBlend cb = new ColorBlend(12);
                cb.Positions = new float[] { 0.0F, 0.10F, 0.15F, 0.20F, 0.25F, 0.45F, 0.54F, 0.65F, 0.75F, 0.8F, 0.9F, 1.0F };

                if (rect.Height == 0 && rect.Width == 0)
                    return;

                if (state == ThemedHeaderDrawing.HeaderState.Normal)
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(245, 247, 249), Color.FromArgb(245, 247, 249), Color.FromArgb(244, 246, 249),
                    Color.FromArgb(244, 246, 249), Color.FromArgb(243, 245, 249), Color.FromArgb(242, 245, 249),
                    Color.FromArgb(242, 244, 248), Color.FromArgb(241, 243, 248), Color.FromArgb(240, 243, 248), 
                    Color.FromArgb(240, 243, 248), Color.FromArgb(240, 243, 247), Color.FromArgb(239, 242, 246)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
                {
                    cb.Colors = Off2007Colors.MouseDownColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(255, 244, 200), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114),
                    Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114),
                    Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), 
                    Color.FromArgb(247,206,114), Color.FromArgb(247,206,114), Color.FromArgb(247,206,114)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
                else
                {
                    cb.Colors = Off2007Colors.MouseHOverColor;
                    LinearGradientBrush br = new LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    cb.Colors = new Color[] { Color.FromArgb(255, 244, 200), Color.FromArgb(255, 244, 205), Color.FromArgb(255, 237, 159),
                    Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159),
                    Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), 
                    Color.FromArgb(255, 237, 159), Color.FromArgb(255, 237, 159), Color.FromArgb(255, 244, 200)};
                    br.InterpolationColors = cb;
                    g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
                    br.Dispose();
                }
            }
		}

        /// <summary>
        /// Returns the Header Border Colors.
        /// </summary>
        /// <param name="clrBottom">The bottom border color</param>
        /// <param name="clrRight">The right border color</param>
        /// <param name="clrInteriorFirst">The gradient start color for the header interior</param>
        /// <param name="clrInteriorLast">The gradient end color for the header interior</param>
        /// <returns></returns>
		public bool GetHeaderBorderColors(out Color clrBottom, out Color clrRight, out Color clrInteriorFirst, out Color clrInteriorLast)
		{
            if (this.isLegacyStyle)
            {
                clrBottom = Color.FromArgb(144, 145, 146);
                clrRight = Color.FromArgb(144, 145, 146);
                clrInteriorFirst = Color.FromArgb(200, 201, 202);
                clrInteriorLast = Color.FromArgb(241, 243, 243);
            }
            else
            {
                clrBottom = Color.FromArgb(144, 145, 146);
                clrRight = Color.FromArgb(144, 145, 146);
                clrInteriorFirst = Color.FromArgb(200, 201, 202);
                clrInteriorLast = Color.FromArgb(241, 243, 243);
            }
			return true;
		}

        /// <summary>
        /// Returns the SortIcon interior
        /// </summary>
        /// <param name="brush">The brush used to fill the sort icon</param>
        /// <param name="pen">The pen used to draw the sort icon</param>
        /// <returns></returns>
		public void GetSortIconBrush(out Brush brush, out Pen pen)
		{
            if (this.isLegacyStyle)
            {
                brush = new SolidBrush(Color.FromArgb(110, 109, 143));
                pen = new Pen(Color.FromArgb(110, 109, 143));
            }
            else
            {
                brush = new SolidBrush(Color.FromArgb(110, 109, 143));
                pen = new Pen(Color.FromArgb(110, 109, 143));
            }
		}

        /// <summary>
        ///  Returns the backcolor and header interior for GroupDropArea.
        /// </summary>
        /// <param name="backColor">The back color for GroupDropArea</param>
        /// <param name="headerBorderTop">The top border color for GroupDropArea header</param>
        /// <param name="headerBorderLeft">The left border color for GroupDropArea header</param>
        /// <returns></returns>
		public bool GetGroupDropAreaColors(out Color backColor, out Color headerBorderTop, out Color headerBorderLeft)
		{
            if (this.isLegacyStyle)
            {
                backColor = Color.FromArgb(240, 241, 242);
                headerBorderTop = Color.FromArgb(144, 145, 146);
                headerBorderLeft = Color.FromArgb(144, 145, 146);
            }
            else
            {
                backColor = Color.FromArgb(240, 243, 248);
                headerBorderTop = Color.FromArgb(165, 172, 181);
                headerBorderLeft = Color.FromArgb(165, 172, 181);
            }
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
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ButtonState.Flat)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderSilver, Off2007Colors.TopFirstSilver, Off2007Colors.TopLastSilver,
                        Off2007Colors.BottomFirstSilver, Off2007Colors.BottomLastSilver, Off2007Colors.BottomLineSilver);
                }
                else if (state == ButtonState.Normal)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ButtonState.Pushed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }
            }
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ButtonState.Flat)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderSilver, Off2007Colors.TopFirstSilver, Off2007Colors.TopLastSilver,
                        Off2007Colors.BottomFirstSilver, Off2007Colors.BottomLastSilver, Off2007Colors.BottomLineSilver);
                }
                else if (state == ButtonState.Normal)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ButtonState.Pushed)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }
            }
		}

        /// <summary>
        /// Draws the ComboBox skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the combo button.</param>
		public void DrawComboBoxStyle(Graphics g, Rectangle rect, ThemedComboBoxDrawing.DropDownState state, Color clrBack)
		{
            if (this.isLegacyStyle)
            {
                Point ptOffset = Point.Empty;

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ThemedComboBoxDrawing.DropDownState.Normal)
                {
                    rect.Inflate(-1, -1);
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();

                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName = "Down.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
            else
            {
                Point ptOffset = Point.Empty;

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (state == ThemedComboBoxDrawing.DropDownState.Normal)
                {
                    rect.Inflate(-1, -1);
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();

                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName = "Down.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
		}

        /// <summary>
        /// Draws the SpinButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="btnId">An integer that represents the type of the button.</param>
        /// <param name="btnState">The current state of the spin button.</param>
		public void DrawSpinButtonStyle(Graphics g, Rectangle rect, ButtonID btnId, ButtonState btnState, Color clrBack)
		{
            if (this.isLegacyStyle)
            {
                Point ptOffset = Point.Empty;
                if (btnId == ButtonID.Up)
                    ptOffset = new Point(-1, -1);

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (btnState == ButtonState.Flat)
                {
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else if (btnState == ButtonState.Normal)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (btnState == ButtonState.Pushed)
                {
                    if (btnId == ButtonID.Up)
                        ptOffset = new Point(0, 0);
                    else
                        ptOffset = new Point(1, 1);

                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName;

                if (btnId == ButtonID.Down)
                    bitmapName = "Down.png";
                else
                    bitmapName = "Up.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
            else
            {
                Point ptOffset = Point.Empty;
                if (btnId == ButtonID.Up)
                    ptOffset = new Point(-1, -1);

                if (rect.Height == 0 || rect.Width == 0)
                    return;

                if (btnState == ButtonState.Flat)
                {
                    Brush brush = new SolidBrush(clrBack);
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else if (btnState == ButtonState.Normal)
                {
                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderHover, Off2007Colors.TopFirstHoverColor, Off2007Colors.TopLastHoverColor,
                        Off2007Colors.BottomFirstHoverColor, Off2007Colors.BottomLastHoverColor, Off2007Colors.BottomLineHoverColor);
                }
                else if (btnState == ButtonState.Pushed)
                {
                    if (btnId == ButtonID.Up)
                        ptOffset = new Point(0, 0);
                    else
                        ptOffset = new Point(1, 1);

                    DrawingUtils.PaintButtonGradient(g, rect, Off2007Colors.BorderClicked, Off2007Colors.TopFirstClickedColor, Off2007Colors.TopLastClickedColor,
                        Off2007Colors.BottomFirstClickedColor, Off2007Colors.BottomLastClickedColor, Off2007Colors.BottomLineClickedColor);
                }

                string bitmapName;

                if (btnId == ButtonID.Down)
                    bitmapName = "Down.png";
                else
                    bitmapName = "Up.png";

                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.RoyalBlue);
            }
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
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Inactive) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderInactive;
                        color = Off2007Colors.CheckBoxFillInactive;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen checkPen = new Pen(Off2007Colors.checkColor, 2);

                        Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
													 new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
													 new Point( innerRect.Right - 1, innerRect.Top -1 )
												 };

                        SmoothingMode prevMode = g.SmoothingMode;

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(checkPen, points);
                        g.SmoothingMode = prevMode;
                        checkPen.Dispose();
                    }

                    if (mixedState && ((state & ButtonState.Inactive) <= 0))
                    {
                        LinearGradientBrush br = new LinearGradientBrush(innerRect, Color.FromArgb(123, 203, 231), Color.FromArgb(33, 89, 140), LinearGradientMode.Vertical);
                        Pen pen1 = new Pen(Color.FromArgb(41, 97, 140));
                        g.DrawRectangle(pen1, innerRect.X - 1, innerRect.Y - 1, innerRect.Width + 1, innerRect.Height + 1);
                        g.FillRectangle(br, innerRect);
                        pen1.Dispose();
                        br.Dispose();
                    }
                    borderPen.Dispose();
                    pen.Dispose();
                    brush.Dispose();
                    innerBrush.Dispose();
                }
                catch { }
            }
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Inactive) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderInactive;
                        color = Off2007Colors.CheckBoxFillInactive;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen checkPen = new Pen(Off2007Colors.checkColor, 2);

                        Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
													 new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
													 new Point( innerRect.Right - 1, innerRect.Top -1 )
												 };

                        SmoothingMode prevMode = g.SmoothingMode;

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(checkPen, points);
                        g.SmoothingMode = prevMode;
                        checkPen.Dispose();
                    }

                    if (mixedState && ((state & ButtonState.Inactive) <= 0))
                    {
                        LinearGradientBrush br = new LinearGradientBrush(innerRect, Color.FromArgb(123, 203, 231), Color.FromArgb(33, 89, 140), LinearGradientMode.Vertical);
                        Pen pen1 = new Pen(Color.FromArgb(41, 97, 140));
                        g.DrawRectangle(pen1, innerRect.X - 1, innerRect.Y - 1, innerRect.Width + 1, innerRect.Height + 1);
                        g.FillRectangle(br, innerRect);
                        pen1.Dispose();
                        br.Dispose();
                    }
                    borderPen.Dispose();
                    pen.Dispose();
                    brush.Dispose();
                    innerBrush.Dispose();
                }
                catch { }
            }
		}

        /// <summary>
        /// Draws the RadioButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the radio button</param>
		public void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state)//, bool mixedState)
		{
            if (this.isLegacyStyle)
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    rect.Width -= 2;
                    rect.Height -= 4;

                    int outRectSize = rect.Height;
                    if (rect.Height > 12)
                        outRectSize = 12;

                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen pen1 = new Pen(Color.FromArgb(30, 75, 105), 1.8f);
                        g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                        innerRect.Inflate(-1, -1);
                        Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                        LinearGradientBrush br = new LinearGradientBrush(innMost, Color.FromArgb(198, 235, 254), Color.FromArgb(8, 130, 198), LinearGradientMode.ForwardDiagonal);
                        g.FillEllipse(br, innMost);
                        br.Dispose();
                        pen1.Dispose();
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
            else
            {
                if (rect.Height == 0 || rect.Width == 0)
                    return;

                try
                {
                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    rect.Width -= 2;
                    rect.Height -= 4;

                    int outRectSize = rect.Height;
                    if (rect.Height > 12)
                        outRectSize = 12;

                    Pen borderPen = this.GetOffice2007BorderPen(state);
                    g.DrawEllipse(borderPen, rect.X, rect.Y, outRectSize, outRectSize);

                    Color penColor;
                    Color color;

                    if ((state & ButtonState.Pushed) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxPushedBorderColor;
                        color = Off2007Colors.CheckBoxPushedFillColor;
                    }
                    else if ((state & ButtonState.Flat) > 0)
                    {
                        penColor = Off2007Colors.CheckBoxInnerBorderColorHot;
                        color = Off2007Colors.CheckBoxFillColorHot;
                    }
                    else
                    {
                        penColor = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                        color = Office2007Colors.Default.DataTimePickerCheckBoxInnerRectFillNormalColor;
                    }

                    Rectangle innerRect = rect;
                    LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal);
                    Pen pen = new Pen(innerBrush);
                    LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

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
                        Pen pen1 = new Pen(Color.FromArgb(30, 75, 105), 1.8f);
                        g.DrawEllipse(pen1, innerRect.X, innerRect.Y, outRectSize - 6, outRectSize - 6);
                        innerRect.Inflate(-1, -1);
                        Rectangle innMost = new Rectangle(innerRect.X, innerRect.Y, outRectSize - 8, outRectSize - 8);
                        LinearGradientBrush br = new LinearGradientBrush(innMost, Color.FromArgb(198, 235, 254), Color.FromArgb(8, 130, 198), LinearGradientMode.ForwardDiagonal);
                        g.FillEllipse(br, innMost);
                        br.Dispose();
                        pen1.Dispose();
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

		}
		private Pen GetOffice2007BorderPen(ButtonState buttonState)
		{
			Pen pen;
            if (this.isLegacyStyle)
            {
                if ((buttonState & ButtonState.Inactive) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderSilverInactive);
                else if ((buttonState & ButtonState.Flat) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else if ((buttonState & ButtonState.Pushed) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderSilverNormal);
            }
            else
            {
                if ((buttonState & ButtonState.Inactive) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderSilverInactive);
                else if ((buttonState & ButtonState.Flat) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else if ((buttonState & ButtonState.Pushed) > 0)
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderColorHot);
                else
                    pen = new Pen(Off2007Colors.CheckBoxOuterBorderSilverNormal);
            }
			return pen;
		}
		/// <summary>
		/// Gets the current visual style.
		/// </summary>
        public GridVisualStyles VisualStyle
		{
			get { return this.visualStyle; }
		}
		#endregion
	}

	#endregion 

	# region Office2003
	/// <summary>
	/// Implements the Office 2003 look and feel
	/// </summary>
	public class GridVisualStylesOffice2003 : Disposable, IVisualStylesDrawing
	{
		private GridVisualStyles visualStyle;
		static Syncfusion.Drawing.IconPaint iconPainter = null;
		private ThemedCheckBoxDrawing themedCheckDrawing = null;
		private ThemedRadioButtonDrawing themedRadioDrawing = null;

        /// <summary>
		/// Creates a new instance of <see cref="GridVisualStylesOffice2007Blue"/> class.
		/// </summary>
		/// <param name="style">The current visual style.</param>
		public GridVisualStylesOffice2003(GridVisualStyles style)
		{
			this.visualStyle = style;

			if (iconPainter == null)
				iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
		}

        /// <summary>
        /// Dispose the resources being used.
        /// </summary>
        /// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (themedCheckDrawing != null)
			{
				themedCheckDrawing.Dispose();
				themedCheckDrawing = null;
			}
			if (themedRadioDrawing != null)
			{
				themedRadioDrawing.Dispose();
				themedRadioDrawing = null;
			}
			base.Dispose (disposing);
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
			Office2003Colors.UpdateMenuColors();

			if (state == ThemedHeaderDrawing.HeaderState.Normal)
			{
				LinearGradientBrush br = new LinearGradientBrush(rect, Office2003Colors.DockBarColorLight, Office2003Colors.GroupBarHeaderColorLight, LinearGradientMode.Vertical);
				g.FillRectangle(br, rect);
				br.Dispose();
			}
			else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
			{
				SolidBrush br = new SolidBrush(Office2003Colors.PressedSelColor);
				g.FillRectangle(br, rect);
				br.Dispose();
			}
			else
			{
				SolidBrush br = new SolidBrush(Office2003Colors.SelColor);
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
        /// <returns></returns>
        public bool GetHeaderBorderColors(out Color clrBottom, out Color clrRight, out Color clrInteriorFirst, out Color clrInteriorLast)
        {
            clrBottom = Color.FromArgb(122, 121, 153);
            clrRight = Color.FromArgb(122, 121, 153);
            clrInteriorFirst = Office2003Colors.GroupBarHeaderColorLight;
            clrInteriorLast = Office2003Colors.DockBarColorLight;
            return true;
        }

		/// <summary>
        /// Returns the SortIcon interior
        /// </summary>
        /// <param name="brush">The brush used to fill the sort icon</param>
        /// <param name="pen">The pen used to draw the sort icon</param>
        /// <returns></returns>
        public void GetSortIconBrush(out Brush brush, out Pen pen)
        {
            brush = new SolidBrush(Color.FromArgb(160, Color.Black));
            pen = new Pen(Color.FromArgb(190, Color.Black));
        }

		/// <summary>
        /// Returns the backcolor and header interior for GroupDropArea.
        /// </summary>
        /// <param name="backColor">The back color for GroupDropArea</param>
        /// <param name="headerBorderTop">The top border color for GroupDropArea header</param>
        /// <param name="headerBorderLeft">The left border color for GroupDropArea header</param>
        /// <returns></returns>
		public bool GetGroupDropAreaColors(out Color backColor, out Color headerBorderTop, out Color headerBorderLeft)
		{
			backColor = Office2003Colors.GroupBarHeaderColorLight;
			headerBorderTop = Color.FromArgb(122, 121, 153);
			headerBorderLeft = Color.FromArgb(122, 121, 153);
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
			Office2003Colors.UpdateMenuColors();
			if (state == ButtonState.Flat)
			{
				LinearGradientBrush br = new LinearGradientBrush(rect, Office2003Colors.DockBarColorLight, Office2003Colors.GroupBarHeaderColorLight, LinearGradientMode.Vertical);
				rect.Inflate(-1,-1);
				g.FillRectangle(br, rect);
				ControlPaint.DrawBorder(g, rect, SystemColors.Highlight, ButtonBorderStyle.Solid);
				br.Dispose();
			}
			else if (state == ButtonState.Normal)
			{
				SolidBrush br = new SolidBrush(Office2003Colors.SelColor);
				rect.Inflate(-1,-1);
				g.FillRectangle(br, rect);
				ControlPaint.DrawBorder(g, rect, Office2003Colors.SelBorderColor, ButtonBorderStyle.Solid);
				br.Dispose();
			}
			else if (state == ButtonState.Pushed)
			{
				SolidBrush br = new SolidBrush(Office2003Colors.PressedSelColor);
				rect.Inflate(-1,-1);
				g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width-1, rect.Height-1));
				ControlPaint.DrawBorder(g, rect, Office2003Colors.SelBorderColor, ButtonBorderStyle.Solid);
				br.Dispose();
			}
		}

		/// <summary>
        /// Draws the ComboBox skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the combo button.</param>
		public void DrawComboBoxStyle(Graphics g, Rectangle rect, ThemedComboBoxDrawing.DropDownState state, Color clrBack)
		{
			Office2003Colors.UpdateMenuColors();

			Point ptOffset = Point.Empty;

			if (state == ThemedComboBoxDrawing.DropDownState.Normal)
			{
				LinearGradientBrush br = new LinearGradientBrush(rect, Office2003Colors.DockBarColorLight, Office2003Colors.GroupBarHeaderColorLight, LinearGradientMode.Vertical);
				rect.Inflate(-1,-1);
				g.FillRectangle(br, rect);
				ControlPaint.DrawBorder(g, rect, SystemColors.Highlight, ButtonBorderStyle.Solid);
				br.Dispose();
			}
			else if (state == ThemedComboBoxDrawing.DropDownState.Hot)
			{
				SolidBrush br = new SolidBrush(Office2003Colors.SelColor);
				rect.Inflate(-1,-1);
				g.FillRectangle(br, rect);
				ControlPaint.DrawBorder(g, rect, Office2003Colors.SelBorderColor, ButtonBorderStyle.Solid);
				br.Dispose();
			}
			else if (state == ThemedComboBoxDrawing.DropDownState.Pressed)
			{
				SolidBrush br = new SolidBrush(Office2003Colors.PressedSelColor);
				rect.Inflate(-1,-1);
				g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width-1, rect.Height-1));
				ControlPaint.DrawBorder(g, rect, Office2003Colors.SelBorderColor, ButtonBorderStyle.Solid);
				ptOffset = new Point(1, 1);
				br.Dispose();
			}


			string bitmapName = "downFill.png";
			iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.Black);
		}

		/// <summary>
        /// Draws the SpinButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="btnId">An integer that represents the type of the button.</param>
        /// <param name="btnState">The current state of the spin button.</param>
		public void DrawSpinButtonStyle(Graphics g, Rectangle rect, ButtonID btnId, ButtonState btnState, Color clrBack)
		{
			Office2003Colors.UpdateMenuColors();
			Point ptOffset = Point.Empty;
			if(btnId == ButtonID.Up)
				ptOffset = new Point(-1, -1);
			
			if (btnState == ButtonState.Flat)
			{
				LinearGradientBrush br;
				if (btnId == ButtonID.Down)
					br = new LinearGradientBrush(rect, Office2003Colors.DockBarColorLight, Office2003Colors.GroupBarHeaderColorLight, LinearGradientMode.Vertical);
				else
					br = new LinearGradientBrush(rect, Office2003Colors.GroupBarHeaderColorLight, Office2003Colors.DockBarColorLight, LinearGradientMode.Vertical);
				g.FillRectangle(br, rect);
				ControlPaint.DrawBorder(g, rect, SystemColors.Highlight, ButtonBorderStyle.Solid);
				br.Dispose();
			}
			else if (btnState == ButtonState.Normal)
			{
				SolidBrush br = new SolidBrush(Office2003Colors.SelColor);
				g.FillRectangle(br, rect);
				ControlPaint.DrawBorder(g, rect, Office2003Colors.SelBorderColor, ButtonBorderStyle.Solid);
				br.Dispose();
			}
			else if (btnState == ButtonState.Pushed)
			{
				if(btnId == ButtonID.Up)
					ptOffset = new Point(0, 0);
				else
					ptOffset = new Point(1, 1);
				SolidBrush br = new SolidBrush(Office2003Colors.PressedSelColor);
				g.FillRectangle(br, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width-1, rect.Height-1));
				ControlPaint.DrawBorder(g, rect, Office2003Colors.SelBorderColor, ButtonBorderStyle.Solid);
				br.Dispose();
			}

			string bitmapName;

			if (btnId == ButtonID.Down)
				bitmapName = "downFill.png";
			else
				bitmapName = "upFill.png";

			iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, Color.Black);

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
                return;

            Color color;
            bool shouldDrawInnerBorder = false;
            if ((state & ButtonState.Pushed) > 0)
            {
                color = Off2003Colors.CheckBoxPushedFillColor;
            }
            else if ((state & ButtonState.Flat) > 0)
            {
                shouldDrawInnerBorder = true;
                color = Off2003Colors.CheckBoxFillColorHot;
            }
            else if ((state & ButtonState.Inactive) > 0)
            {
                color = Color.Transparent;
            }
            else
            {
                color = Off2003Colors.CheckBoxFillNormal;
            }

            Rectangle innerRect = rect;
            using (LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, 45))
            {
                innerRect.Inflate(-1, -1);
                g.FillRectangle(brush, innerRect);
            }

            Rectangle clipRect = innerRect;
            clipRect.Inflate(-2, -2);
            if ((state & ButtonState.Checked) > 0)
            {

                Point[] points = new Point[] {
													 new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 - 2),
													 new Point( innerRect.X + innerRect.Width / 2 -1 , innerRect.Bottom - 4 ),
													 new Point( innerRect.Right - 1, innerRect.Top + 1)
												 };
                Pen checkPen;
                if ((state & ButtonState.Inactive) > 0)
                    checkPen = new Pen(Off2003Colors.CheckColorInactive, 2);
                else
                    checkPen = new Pen(Off2003Colors.CheckColor, 2);
                SmoothingMode prevMode = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.SetClip(clipRect);
                g.DrawLines(checkPen, points);
                g.SmoothingMode = prevMode;
                g.ResetClip();
            }

            if (mixedState && ((state & ButtonState.Inactive) <= 0))
            {
                using (Brush br = new SolidBrush(Off2003Colors.CheckColor))
                {
                    g.FillRectangle(br, clipRect);
                }
            }

            if (shouldDrawInnerBorder)
            {
                innerRect.Width--;
                innerRect.Height--;

                using (LinearGradientBrush innerBrush = new LinearGradientBrush(innerRect, Off2003Colors.CheckBoxInnerBorderColorHot, Color.White, 225))
                {
                    using (Pen pen = new Pen(innerBrush, 2.5f))
                    {
                        g.DrawRectangle(pen, innerRect);
                    }
                }

                innerRect.Width++;
                innerRect.Height++;

            }

            Pen borderPen;
            if ((state & ButtonState.Inactive) > 0)
                borderPen = new Pen(Off2003Colors.CheckBoxOuterBorderColorInactive);
            else
                borderPen = new Pen(Off2003Colors.CheckBoxOuterBorderColor);
            g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
        
        //public void DrawCheckBoxStyle(Graphics g, Rectangle rect, ButtonState state, bool mixedState)
        //{
        //    if (this.themedCheckDrawing == null)
        //        this.themedCheckDrawing = new ThemedCheckBoxDrawing();
        //    this.themedCheckDrawing.DrawCheckBox(g, rect, state, mixedState);
        //}

		/// <summary>
		/// Draws the RadioButton skins
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
		/// <param name="state">The current state of the radio button</param>
        public void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state)//, bool mixedState)
        {
            if (rect.Height == 0 || rect.Width == 0)
                return;

            SmoothingMode prevMode = g.SmoothingMode;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            rect.Offset(1, 0);
            rect.Width -= 3;
            rect.Height -= 2;

            Color penColor = Off2003Colors.CheckBoxOuterBorderColor;
            Color color = Off2003Colors.CheckBoxFillNormal;
            bool shouldDrawInnerBorder = false;

            if ((state & ButtonState.Pushed) > 0)
            {
                color = Off2003Colors.CheckBoxPushedFillColor;
            }
            else if ((state & ButtonState.Flat) > 0)
            {
                //hovering notification was ignored....
                //check GridCellButton class Draw method and GridRadioButtonCell class DrawButton method
                //shouldDrawInnerBorder = true;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(rect, color, Color.White, 45))
            {

                g.FillEllipse(brush, rect);
            }

            if (shouldDrawInnerBorder)
            {
                //TODO: Draw RadioButtonHotInnerBorder
            }

            using (Pen borderPen = new Pen(Off2003Colors.CheckBoxOuterBorderColor))
            {
                g.DrawEllipse(borderPen, rect);
            }
            if ((state & ButtonState.Checked) > 0)
            {
                RectangleF rectf = rect;
                rectf.Offset(3.3f, 3.3f);
                rectf.Width = 5.0f;
                rectf.Height = 5.0f;
                using (LinearGradientBrush br = new LinearGradientBrush(rectf, Color.Green, Color.LightGreen, 225))
                {
                    g.FillEllipse(br, rectf);
                }
            }
            g.SmoothingMode = prevMode;
        }

        //public void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state)// bool mixedState)
        //{
        //    if (this.themedRadioDrawing == null)
        //        this.themedRadioDrawing = new ThemedRadioButtonDrawing();
        //    this.themedRadioDrawing.DrawRadio(g, rect, state, false);//mixedState);

        //}

		/// <summary>
		/// Gets the current VisualStyle.
		/// </summary>
        public GridVisualStyles VisualStyle
        {
            get { return GridVisualStyles.Office2003; }
        }
        #endregion
    }

#endregion 

    #region SystemTheme
	/// <summary>
	/// Implements the SystemTheme for grid components.
	/// </summary>
	public class GridVisualStylesSystemTheme : Disposable, IVisualStylesDrawing
	{
		private GridVisualStyles visualStyle;
		static Syncfusion.Drawing.IconPaint iconPainter = null;
		private ThemedHeaderDrawing themeHeaderDrawing = null;
		private ThemedPushButtonDrawing themedButtonDrawing = null;
		private ThemedComboBoxDrawing themedComboDrawing = null;
		private ThemedSpinButtonDrawing themedSpinDrawing = null;
		private ThemedCheckBoxDrawing themedCheckDrawing = null;
		private ThemedRadioButtonDrawing themedRadioDrawing = null;

		/// <summary>
		/// Disposes all resources being used.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (themeHeaderDrawing != null)
			{
				themeHeaderDrawing.Dispose();
				themeHeaderDrawing = null;
			}
			if (themedButtonDrawing != null)
			{
				themedButtonDrawing.Dispose();
				themedButtonDrawing = null;
			}
			if (themedComboDrawing != null)
			{
				themedComboDrawing.Dispose();
				themedComboDrawing = null;
			}
			if (themedSpinDrawing != null)
			{
				themedSpinDrawing.Dispose();
				themedSpinDrawing = null;
			}
			if (themedCheckDrawing != null)
			{
				themedCheckDrawing.Dispose();
				themedCheckDrawing = null;
			}
			if (themedRadioDrawing != null)
			{
				themedRadioDrawing.Dispose();
				themedRadioDrawing = null;
			}
			base.Dispose (disposing);
		}

        /// <summary>
		/// Creates a new instance of <see cref="GridVisualStylesOffice2007Blue"/> class.
		/// </summary>
		/// <param name="style">The current visual style.</param>
		public GridVisualStylesSystemTheme(GridVisualStyles style)
		{
			this.visualStyle = style;

			if (iconPainter == null)
				iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Images.", AssemblyInfo.Assembly);
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
			if (this.themeHeaderDrawing == null)
				this.themeHeaderDrawing = new ThemedHeaderDrawing();
			this.themeHeaderDrawing.DrawHeader(g, rect, state);
		}

        /// <summary>
        /// Returns the Header Border Colors.
        /// </summary>
        /// <param name="clrBottom">The bottom border color</param>
        /// <param name="clrRight">The right border color</param>
        /// <param name="clrInteriorFirst">The gradient start color for the header interior</param>
        /// <param name="clrInteriorLast">The gradient end color for the header interior</param>
        /// <returns></returns>
        public bool GetHeaderBorderColors(out Color clrBottom, out Color clrRight, out Color clrInteriorFirst, out Color clrInteriorLast)
        {
            clrBottom = Color.Empty;
            clrRight = Color.Empty;
            clrInteriorFirst = Color.Empty;
            clrInteriorLast = Color.Empty;
            return false;
        }

		/// <summary>
        /// Returns the SortIcon interior
        /// </summary>
        /// <param name="brush">The brush used to fill the sort icon</param>
        /// <param name="pen">The pen used to draw the sort icon</param>
        /// <returns></returns>
        public void GetSortIconBrush(out Brush brush, out Pen pen)
        {
            brush = new SolidBrush(SystemColors.ControlDarkDark);
            pen = new Pen(SystemColors.WindowFrame);
        }

		/// <summary>
		/// Returns the backcolor and header interior for GroupDropArea.
		/// </summary>
		/// <param name="backColor">The back color for GroupDropArea</param>
		/// <param name="headerBorderTop">The top border color for GroupDropArea header</param>
		/// <param name="headerBorderLeft">The left border color for GroupDropArea header</param>
		/// <returns></returns>
		public bool GetGroupDropAreaColors(out Color backColor, out Color headerBorderTop, out Color headerBorderLeft)
		{
			backColor = SystemColors.ControlDark;
			headerBorderTop = Color.Empty;
			headerBorderLeft = Color.Empty;
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
			if (this.themedButtonDrawing == null)
				this.themedButtonDrawing = new ThemedPushButtonDrawing();
			this.themedButtonDrawing.DrawPushButton(g, rect, state);
		}

		/// <summary>
        /// Draws the ComboBox skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the combo button.</param>
		public void DrawComboBoxStyle(Graphics g, Rectangle rect, ThemedComboBoxDrawing.DropDownState state, Color clrBack)
		{
			if(themedComboDrawing == null)
				this.themedComboDrawing = new ThemedComboBoxDrawing("COMBOBOX");
			this.themedComboDrawing.DrawDropDownButton(g, state, rect);
		}

		/// <summary>
        /// Draws the SpinButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="btnId">An integer that represents the type of the button.</param>
        /// <param name="btnState">The current state of the spin button.</param>
		public void DrawSpinButtonStyle(Graphics g, Rectangle rect, ButtonID btnId, ButtonState btnState, Color clrBack)
		{
			if(this.themedSpinDrawing == null)
				this.themedSpinDrawing = new ThemedSpinButtonDrawing();
			this.themedSpinDrawing.DrawScrollButton(g, rect, btnId, ScrollButtonAppearance.Vertical, btnState);
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
			if(this.themedCheckDrawing == null)
				this.themedCheckDrawing = new ThemedCheckBoxDrawing();
			this.themedCheckDrawing.DrawCheckBox(g, rect, state, mixedState);
		}

		/// <summary>
        /// Draws the RadioButton skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the radio button</param>
		public void DrawRadioStyle(Graphics g, Rectangle rect, ButtonState state)//, bool mixedState)
		{
			if(this.themedRadioDrawing == null)
				this.themedRadioDrawing = new ThemedRadioButtonDrawing();
			this.themedRadioDrawing.DrawRadio(g, rect, state, false);//mixedState);
		}

		/// <summary>
		/// Gets the current VisualStyle.
		/// </summary>
        public GridVisualStyles VisualStyle
        {
            get { return GridVisualStyles.SystemTheme; }
        }
        #endregion

    }
    #endregion

	#region Enum GridVisualStyles

	/// <summary>
	/// Specifies the VisualStyle with which various components across the grid will appear and behave.
	/// </summary>
	public enum GridVisualStyles
	{
		SystemTheme,
		Office2003,
		Office2007Blue,
        Office2007Black,
        Office2007Silver,
        Office2010Blue,
        Office2010Black,
        Office2010Silver,
        Metro,
        Custom
	};
    
    #endregion

    #region Office2003Colors
    /// <summary>
    /// Represents the colors for Office2003 style. 
    /// Provides static members to access the colors used by different grid elements.
    /// </summary>
    public class Off2003Colors
    {
       
        public static Color CheckBoxOuterBorderColor = Color.FromArgb(28, 81, 128);
        public static Color CheckBoxOuterBorderColorInactive = Color.FromArgb(206, 203, 189);
        public static Color CheckBoxFillNormal = Color.FromArgb(220, 220, 215);

        public static Color CheckBoxInnerBorderColorHot = Color.FromArgb(248, 179, 48);
        public static Color CheckBoxFillColorHot = Color.FromArgb(231, 231, 227);

        public static Color CheckBoxPushedFillColor = Color.FromArgb(176, 176, 167);

        public static Color CheckColor = Color.FromArgb(33, 161, 33);
        public static Color CheckColorInactive = Color.FromArgb(206, 203, 189);
    }

    #endregion

	#region Office2007Colors

	/// <summary>
	/// Represents the colors for Office2007 style. 
	/// Provides static members to access the colors used by different grid elements.
	/// </summary>
	public class Off2007Colors
	{
		/// <summary>
		/// Gets an array of colors used for Office2007Blue style.
		/// </summary>
		public static Color[] NormalBlue
		{
			get
			{
				return new Color[]{
									  Color.FromArgb(240,247,255),Color.FromArgb(233,243,255), Color.FromArgb(226,239,255),
									  Color.FromArgb(213,230,254),Color.FromArgb(213,230,254), Color.FromArgb(206,226,254),
									  Color.FromArgb(178,208,252),Color.FromArgb(182,211,252),Color.FromArgb(186,214,252),
									  Color.FromArgb(191,216,253),Color.FromArgb(195,219,253),Color.FromArgb(199,222,253)
								  };
			}
		}

		/// <summary>
		/// Gets an array of colors used to represent Hot State color. 
		/// </summary>
		public static Color[] MouseHOverColor
		{
			get
			{
				return new Color[]{
									  Color.FromArgb(255,254,228), Color.FromArgb(255,250,216), Color.FromArgb(255,246,204),
									  Color.FromArgb(255,241,192), Color.FromArgb(255,237,180), Color.FromArgb(255,233,168),
									  Color.FromArgb(255,215,103), Color.FromArgb(255,218,114), Color.FromArgb(255,221,125),
									  Color.FromArgb(255,224,137), Color.FromArgb(255,227,148), Color.FromArgb(255,230,159)
									  
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
				return new Color[]{
									  Color.FromArgb(255,189,105), Color.FromArgb(255,186,97), Color.FromArgb(255,183,90),
									  Color.FromArgb(255,179,82), Color.FromArgb(255,176,75), Color.FromArgb(255,173,67),
									  Color.FromArgb(251,140,60), Color.FromArgb(252,149,69), Color.FromArgb(253,159,77),
									  Color.FromArgb(253,168,86), Color.FromArgb(254,178,94), Color.FromArgb(255,187,103)
									  
								  };
			}
		}

		//Colors for Blue Theme
		public static Color TopFirstBlue = Color.FromArgb(240,247,255);
		public static Color TopLastBlue = Color.FromArgb(206,226,254);
		public static Color BottomFirstBlue = Color.FromArgb(178,208,252);
		public static Color BottomLastBlue = Color.FromArgb(199,222,253);
        public static Color BottomLineBlue = Color.FromArgb(227, 237, 251);
        public static Color BorderBlue = Color.FromArgb(154, 184, 223);

		//Colors for Black Theme
        public static Color TopFirstBlack = Color.FromArgb(237, 241, 253);
        public static Color TopLastBlack = Color.FromArgb(220, 222, 236);
        public static Color BottomFirstBlack = Color.FromArgb(198, 199, 214);
        public static Color BottomLastBlack = Color.FromArgb(217, 220, 233);
		public static Color BottomLineBlack = Color.FromArgb(239, 242, 246);
        public static Color BorderBlack = Color.FromArgb(107, 113, 115);

        //Colors for Silver Theme
		public static Color TopFirstSilver = Color.FromArgb(253, 249, 253);
        public static Color TopLastSilver = Color.FromArgb(232, 230, 236);
        public static Color BottomFirstSilver = Color.FromArgb(206, 207, 214);
        public static Color BottomLastSilver = Color.FromArgb(229, 228, 233);
		public static Color BottomLineSilver = Color.FromArgb(239, 242, 246);
        public static Color BorderSilver = Color.FromArgb(74, 81, 90);

		//Colors for Hot State
        public static Color TopFirstHoverColor = Color.FromArgb( 254, 252, 226 );
        public static Color TopLastHoverColor = Color.FromArgb(254, 232, 150);
        public static Color BottomFirstHoverColor = Color.FromArgb(254, 217, 117);
        public static Color BottomLastHoverColor = Color.FromArgb(254, 231, 165);
        public static Color BottomLineHoverColor = Color.FromArgb( 254, 252, 243 );
        public static Color BorderHover = Color.FromArgb(219, 206, 153);

		//Colors for Pressed State
        public static Color TopFirstClickedColor = Color.FromArgb(234, 224, 191);
        public static Color TopLastClickedColor = Color.FromArgb(239, 189, 119);
        public static Color BottomFirstClickedColor = Color.FromArgb(255, 168, 56);
        public static Color BottomLastClickedColor = Color.FromArgb(255, 212, 86);
        public static Color BottomLineClickedColor = Color.FromArgb(255, 230, 148);
        public static Color BorderClicked = Color.FromArgb(172, 160, 111);

		public static Color CheckBoxOuterBorderBlueNormal = Color.FromArgb(171, 193, 222);
		public static Color CheckBoxOuterBorderBlackNormal = Color.FromArgb(132, 132, 132);
		public static Color CheckBoxOuterBorderSilverNormal = Color.FromArgb(155, 157, 160);
        public static Color CheckBoxOuterBorderBlueInactive = Color.FromArgb(173, 178, 181);
        public static Color CheckBoxOuterBorderBlackInactive = Color.FromArgb(173, 178, 181);
        public static Color CheckBoxOuterBorderSilverInactive = Color.FromArgb(214, 219, 222);

		public static Color CheckBoxInnerBorderNormal = Color.FromArgb(162, 172, 185);
        public static Color CheckBoxInnerBorderInactive = Color.FromArgb(207, 199, 207);
        public static Color CheckBoxFillNormal = Color.FromArgb(202, 207, 213);
        public static Color CheckBoxFillInactive = Color.FromArgb(239, 243, 247);

		public static Color CheckBoxOuterBorderColorHot = Color.FromArgb(85, 119, 163);
		public static Color CheckBoxInnerBorderColorHot = Color.FromArgb(250, 213, 122);
		public static Color CheckBoxFillColorHot = Color.FromArgb(252, 231, 175);

		public static Color CheckBoxPushedBorderColor = Color.FromArgb(242, 137, 28);
		public static Color CheckBoxPushedFillColor = Color.FromArgb(255, 208, 103);

		public static Color checkColor = Color.FromArgb( 68, 101, 145);
	}
    #endregion

    # region Office2010Colors
    /// <summary>
    /// Represents the colors for Office2010style. 
    /// Provides static members to access the colors used by different grid elements.
    /// </summary>
    public class Off2010Colors
    {
        /// <summary>
        /// Gets an array of colors used to represent Hot State color. 
        /// </summary>
        public static Color[] MouseHOverColor
        {
            get
            {
                return new Color[]{
									  Color.FromArgb(255,223,107), Color.FromArgb(255,219,95), Color.FromArgb(255,215,83),
									  Color.FromArgb(255,210,79), Color.FromArgb(255,206,67), Color.FromArgb(255,204,55),
									  Color.FromArgb(255,186,105), Color.FromArgb(255,189,116), Color.FromArgb(255,192,127),
									  Color.FromArgb(255,195,139), Color.FromArgb(255,198,140), Color.FromArgb(255,201,150)
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
                return new Color[]{
									  Color.FromArgb(255,223,107), Color.FromArgb(255,219,95), Color.FromArgb(255,215,83),
									  Color.FromArgb(255,210,79), Color.FromArgb(255,206,67), Color.FromArgb(255,204,55),
									  Color.FromArgb(255,186,5), Color.FromArgb(255,189,16), Color.FromArgb(255,192,27),
									  Color.FromArgb(255,195,39), Color.FromArgb(255,198,40), Color.FromArgb(255,201,50)
								  };
            }
        }
   
        public static Color TopFirstBlue = Color.FromArgb(240, 247, 255);
        public static Color TopLastBlue = Color.FromArgb(218, 231, 245);
        public static Color BottomFirstBlue = Color.FromArgb(218, 231, 245);
        public static Color BottomLastBlue = Color.FromArgb(199, 222, 253);
        public static Color BottomLineBlue = Color.FromArgb(227, 237, 251);
        public static Color BorderBlue = Color.FromArgb(160, 176, 199);

        //Colors for Black Theme
        public static Color TopFirstBlack = Color.FromArgb(221, 224, 227);
        public static Color TopLastBlack = Color.FromArgb(221, 224, 227);
        public static Color BottomFirstBlack = Color.FromArgb(199, 203, 209);
        public static Color BottomLastBlack = Color.FromArgb(199, 203, 209);
        public static Color BottomLineBlack = Color.FromArgb(199, 203, 209);
        public static Color BorderBlack = Color.FromArgb(44, 44, 44);

        //Colors for Silver Theme
        public static Color TopFirstSilver = Color.FromArgb(253, 249, 253);
        public static Color TopLastSilver = Color.FromArgb(232, 230, 236);
        public static Color BottomFirstSilver = Color.FromArgb(206, 207, 214);
        public static Color BottomLastSilver = Color.FromArgb(229, 228, 233);
        public static Color BottomLineSilver = Color.FromArgb(239, 242, 246);
        public static Color BorderSilver = Color.FromArgb(74, 81, 90);

        //Colors for Hot State
        public static Color TopFirstHoverColor = Color.FromArgb(255, 251, 222);
        public static Color TopLastHoverColor = Color.FromArgb(255, 243, 137);
        public static Color BottomFirstHoverColor = Color.FromArgb(255, 223, 107);
        public static Color BottomLastHoverColor = Color.FromArgb(255, 223, 107);
        public static Color BottomLineHoverColor = Color.FromArgb(255, 223, 107);
        public static Color BorderHover = Color.FromArgb(219, 206, 153);

        //Colors for Pressed State
        public static Color TopFirstClickedColor = Color.FromArgb(255, 252, 224);
        public static Color TopLastClickedColor = Color.FromArgb(255, 203, 54);
        public static Color BottomFirstClickedColor = Color.FromArgb(255, 186, 6);
        public static Color BottomLastClickedColor = Color.FromArgb(255, 200, 45);
        public static Color BottomLineClickedColor = Color.FromArgb(255, 200, 45);
        public static Color BorderClicked = Color.FromArgb(194, 138, 48);

        public static Color CheckBoxOuterBorderBlueNormal = Color.FromArgb(171, 193, 222);
        public static Color CheckBoxOuterBorderBlackNormal = Color.FromArgb(132, 132, 132);
        public static Color CheckBoxOuterBorderSilverNormal = Color.FromArgb(155, 157, 160);
        public static Color CheckBoxOuterBorderBlueInactive = Color.FromArgb(173, 178, 181);
        public static Color CheckBoxOuterBorderBlackInactive = Color.FromArgb(173, 178, 181);
        public static Color CheckBoxOuterBorderSilverInactive = Color.FromArgb(214, 219, 222);

        public static Color CheckBoxInnerBorderNormal = Color.FromArgb(162, 172, 185);
        public static Color CheckBoxInnerBorderInactive = Color.FromArgb(207, 199, 207);
        public static Color CheckBoxFillNormal = Color.FromArgb(202, 207, 213);
        public static Color CheckBoxFillInactive = Color.FromArgb(239, 243, 247);

        public static Color CheckBoxOuterBorderColorHot = Color.FromArgb(85, 119, 163);
        public static Color CheckBoxInnerBorderColorHot = Color.FromArgb(250, 213, 122);
        public static Color CheckBoxFillColorHot = Color.FromArgb(252, 231, 175);

        public static Color CheckBoxPushedBorderColor = Color.FromArgb(242, 137, 28);
        public static Color CheckBoxPushedFillColor = Color.FromArgb(255, 208, 103);

        public static Color checkColor = Color.FromArgb(68, 101, 145);
    }

    # endregion

    #region Enum ColorStyles

    /// <summary>
    /// Specifies the VisualStyle with which various components across the grid will appear and behave.
    /// </summary>
    public enum ColorStyles
    {
        SystemTheme,
        Office2003,
        Office2007Blue,
        Office2007Black,
        Office2007Silver,
        Office2010Blue,
        Office2010Black,
        Office2010Silver
    };

    #endregion
}
