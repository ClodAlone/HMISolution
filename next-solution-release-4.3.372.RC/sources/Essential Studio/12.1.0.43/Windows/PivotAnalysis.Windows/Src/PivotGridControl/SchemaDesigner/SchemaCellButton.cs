#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    public class SchemaCellButton : GridCellButton
    {
        private GridCellRendererBase owner;
        public SchemaCellButton(GridCellRendererBase owner):base(owner)
        {
            this.owner = owner;
        }
        
        public virtual void DrawButtonStyle(Graphics g, Rectangle rect, ButtonState state, GridStyleInfo style, bool isWhite)
        {
            string value = style.Description;
            if (rect.Height == 0 || rect.Width == 0)
                return;
            try
            {
                IconPaint iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Resources.", AssemblyInfo.Assembly);
                string bitmapName = null;

                if (state == ButtonState.Flat)
                    if (isWhite)
                    {
                        bitmapName = value == "-" ? "WhiteD.png" : value == "@" ? "sch_filter_white.png" : "clear_white.png";
                        if (value == "@" && FilterDropDown.FilterDimensions != null && FilterDropDown.FilterDimensions.Contains(style.Text))
                            bitmapName = "filtered_white.png";
                    }
                    else
                    {
                        bitmapName = value == "-" ? "NormalD.png" : value == "@" ? "sch_filter.png" : "clear.png";
                        if (value == "@" && FilterDropDown.FilterDimensions != null && FilterDropDown.FilterDimensions.Contains(style.Text))
                            bitmapName = "filtered_gray.png";
                    }
                else if (state == ButtonState.Pushed)
                {
                    bitmapName = value == "-" ? "ClickedD.png" : value == "@" ? "sch_filter.png" : "clear.png";
                    if (value == "@" && FilterDropDown.FilterDimensions != null && FilterDropDown.FilterDimensions.Contains(style.Text))
                        bitmapName = "filtered_gray.png";
                }
                else
                {
                    bitmapName = value == "-" ? "WhiteD.png" : value == "@" ? "sch_filterhover.png" : "clear_white.png";
                    if (value == "@" && FilterDropDown.FilterDimensions != null && FilterDropDown.FilterDimensions.Contains(style.Text))
                        bitmapName = "filtered_gray.png";
                }
               
                iconPainter.PaintIcon(g, rect, Point.Empty, bitmapName, Color.Black);
            }
            catch
            { }
        }
        public override void DrawButton(Graphics g, Rectangle rect, ButtonState buttonState, GridStyleInfo style)
        {

            if ((!Grid.PrintingMode && style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled) || ((style.Themed && this.Grid.ThemesEnabled && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)))))
            {
                if (!style.Enabled && (buttonState & ButtonState.Inactive) != 0)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Color.FromArgb(156, 164, 173), Color.FromArgb(242, 242, 242),
                        Color.FromArgb(227, 227, 227), Color.FromArgb(211, 211, 211), Color.FromArgb(215, 215, 215), Color.FromArgb(215, 215, 215));
                }
                else
                {
                        DrawButtonStyle(g, rect, buttonState, style, false);
                    
                }
            }
            else
            {
                base.DrawButton(g, rect, buttonState, style);
            }
           
        }
    }
}
