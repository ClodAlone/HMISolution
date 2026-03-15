#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

﻿using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Grid.Grouping;
using Syncfusion.ComponentModel;
using System.Text.RegularExpressions;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Initializes a new <see cref="GridHighlightSearchText"/>
    /// The highlight option works only if WrapText is set to false and AllowEnter is set to false.
    /// </summary>
    public class GridHighlightSearchText
    {

        private GridControlBase grid;
       
        /// <summary>
        /// Wires the grid and the text that needs to be highlighted.
        /// </summary>
        /// <param name="grid">GridControlBase.</param>
        /// <param name="text">The text is a string which searches the whole grid.</param>
        public void WireGrid(GridControlBase grid, string text)
        {
            UnWireGrid();
            this.grid = grid;
            this.HighlightText = text;
            grid.DrawCell += new GridDrawCellEventHandler(grid_DrawCell);

        }
        /// <summary>
        /// Wires the grid and its componenets
        /// </summary>
        public void WireGrid(GridControlBase grid)
        {
            UnWireGrid();
            this.grid = grid;
            grid.DrawCell += new GridDrawCellEventHandler(grid_DrawCell);
        }
        private string highlightText = string.Empty;

        /// <summary>
        /// Contains the text which needs to be highlighted
        /// </summary>
        public string HighlightText
        {
            get { return highlightText; }
            set { highlightText = value; }
        }

        
        private Color highlightColor = Color.Yellow;
       /// <summary>
        /// Gets or sets the color in which the text needs to be highlighted.
        /// </summary>
        public Color HighlightColor
        {
            get { return highlightColor; }
            set { highlightColor = value; }
        }

        private int padding = 7;
        // <summary>
        /// Triggers when the cell is been drawn.
        /// </summary>
        private void grid_DrawCell(object sender, GridDrawCellEventArgs e)
        {
            if (e.Style.Text != "")
            {
            }
            string text = HighlightText.ToLower();

            if (!string.IsNullOrEmpty(text) && e.Style.Text.ToLower().Contains(text) && !(e.Style.WrapText || e.Style.AllowEnter ))
            {
                Size textSize = TextRenderer.MeasureText(HighlightText, e.Style.GdipFont);

                int startWidth = 2;
                int position = e.Style.Text.ToLower().IndexOf(text);
                if (position > 0)
                {
                    string str = e.Style.Text.Substring(0, position);
                    
                    if (str.Length > 0 && str != text)
                    {
                            startWidth = TextRenderer.MeasureText(str, e.Style.GdipFont).Width - padding;//Convert.ToInt32(e.Style.GdipFont.Size);
                    }
                    
                    if (startWidth + textSize.Width - padding < e.Bounds.Width && e.Bounds.Height > padding)  //Convert.ToInt32(e.Style.GdipFont.Size))
                    {
                        Rectangle rect = new Rectangle(e.Bounds.X + startWidth, e.Bounds.Y + (e.Bounds.Height / 2) - (textSize.Height / 2), textSize.Width - padding, textSize.Height);
                        using (Brush br = new SolidBrush(HighlightColor))
                            e.Graphics.FillRectangle(br, rect);
                    }
                }
                else
                {
                    if (startWidth + textSize.Width - padding < e.Bounds.Width && e.Bounds.Height > padding)
                    {
                        Rectangle rect = new Rectangle(e.Bounds.X + startWidth, e.Bounds.Y + (e.Bounds.Height / 2) - (textSize.Height / 2), textSize.Width - padding, textSize.Height);
                        using (Brush br = new SolidBrush(HighlightColor))
                            e.Graphics.FillRectangle(br, rect);
                    }
                }
            }
        }

        /// <summary>
        /// Unwires the grid and its components.
        /// </summary>
        public void UnWireGrid()
        {
            this.HighlightText = string.Empty;
            this.HighlightColor = Color.Yellow;
            if (grid != null)
            {
                grid.DrawCell -= new GridDrawCellEventHandler(grid_DrawCell);
                grid = null;
            }
        }
    }
}
