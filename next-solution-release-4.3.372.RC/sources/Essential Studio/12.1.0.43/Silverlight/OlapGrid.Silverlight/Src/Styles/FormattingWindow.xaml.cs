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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Silverlight.Grid.Olap;
using System.Collections;
using Syncfusion.Silverlight.Grid.Olap.Common;
using Syncfusion.Windows.Controls.Grid;

namespace Syncfusion.Silverlight.Grid.Olap
{
    /// <summary>
    /// Formatting window provides options to customize the apperance of grid
    /// </summary>
    public partial class FormattingWindow : ChildWindow
    {
        #region Constructor
        /// <summary>
        /// Constructor to initialize the window
        /// </summary>
        public FormattingWindow(OlapGrid olapGrid)
        {
            InitializeComponent();
            this.FlowDirection = olapGrid.FlowDirection;
            this.Title = Syncfusion.Silverlight.Grid.Olap.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapGrid_StyleDialog_Title");
            this.Grid = olapGrid;
            this.GridStyleInfo = olapGrid.GridStyleInfo;
            this.InitializeStyles();
        }
        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the grid style info.
        /// </summary>
        /// <value>The grid style info.</value>
        internal ExportingGridStyleInfo GridStyleInfo { get; set; }
        /// <summary>
        /// Gets or sets the OlapGrid.
        /// </summary>
        /// <value>The grid.</value>
        internal OlapGrid Grid { get; set; }

        #endregion


        /// <summary>
        /// Handles the Click event of the OK Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            GridStyleInfo.HeaderForeGroundColor = HeaderColumnForeColor.Color;
            GridStyleInfo.HeaderBackgroundColor = HeaderColumnBackColor.Color;
            GridStyleInfo.HeaderRowBackgroundColor = HeaderRowBackColor.Color;
            GridStyleInfo.HeaderRowForegroundColor = HeaderRowForeColor.Color;
            GridStyleInfo.HeaderFontSize = HeaderFontSize.Value;
            if (HeaderFontName.SelectedItem != null && HeaderFontName.SelectedIndex >= 0)
                GridStyleInfo.HeaderFontStyle = HeaderFontName.SelectedItem.ToString();
            GridStyleInfo.SummaryColumnForegroundColor = SummaryColumnForeColor.Color;
            GridStyleInfo.SummaryColumnBackgroundColor = SummaryColumnBackColor.Color;
            GridStyleInfo.SummaryRowBackgroundColor = SummaryRowBackColor.Color;
            GridStyleInfo.SummaryRowForegroundColor = SummaryRowForeColor.Color;
            GridStyleInfo.SummaryFontSize = this.SummaryFontSize.Value;
            if (SummaryFontName.SelectedItem != null && this.SummaryFontName.SelectedIndex >= 0)
                GridStyleInfo.SummaryFontName = GridStyleInfo.fontFamily[this.SummaryFontName.SelectedIndex];
            GridStyleInfo.CellFontColor = CellFontColor.Color;
            if (CellFontName.SelectedItem != null && this.CellFontName.SelectedIndex >= 0)
                GridStyleInfo.CellFontName = GridStyleInfo.fontFamily[this.CellFontName.SelectedIndex];
            GridStyleInfo.CellFontSize = this.CellFontSize.Value;
            GridStyleInfo.CellFontStyle = GridStyleInfo.fontStyle[this.CellFontStyle.SelectedIndex];


            GridStyleInfo.ValueTextColor = CellFontColor.Color;
            GridStyleInfo.GridlineColor = GridlineColor.Color;
            this.ApplyStylesToGrid();

            //Refresh the OlapGrid after new style information
            this.Grid.InternalGrid.Model.Refresh();
            this.DialogResult = true;

        }

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Bind fontstyle combo box controls  
        /// </summary>
        public void BindStyles()
        {
            ///Header Tab
            this.HeaderFontName.ItemsSource = GridStyleInfo.fontFamily;
            this.HeaderFontName.SelectedItem = GridStyleInfo.HeaderFontName;
            this.HeaderFontSize.Value = GridStyleInfo.HeaderFontSize;
            this.SummaryFontSize.Value = GridStyleInfo.SummaryFontSize;
            this.SummaryFontName.ItemsSource = GridStyleInfo.fontFamily;
            this.SummaryFontName.SelectedItem = GridStyleInfo.SummaryFontName;
            this.CellFontName.ItemsSource = GridStyleInfo.fontFamily;
            this.CellFontName.SelectedItem = GridStyleInfo.CellFontName;
            this.CellFontSize.Value = GridStyleInfo.CellFontSize;
            this.CellFontStyle.ItemsSource = GridStyleInfo.fontStyle;
        }

        /// <summary>
        /// Initial selected items
        /// </summary>
        public void InitializeStyles()
        {

            this.BindStyles();
            this.HeaderColumnBackColor.Color = GridStyleInfo.HeaderBackgroundColor;
            this.HeaderColumnForeColor.Color = GridStyleInfo.HeaderForeGroundColor;
            this.HeaderFontName.SelectedItem = GridStyleInfo.HeaderFontStyle;
            this.HeaderRowBackColor.Color = GridStyleInfo.HeaderRowBackgroundColor;
            this.HeaderRowForeColor.Color = GridStyleInfo.HeaderRowForegroundColor;
            this.HeaderFontName.SelectedItem = GridStyleInfo.HeaderFontName;
            this.HeaderFontSize.Value = GridStyleInfo.HeaderFontSize;

            this.SummaryColumnBackColor.Color = GridStyleInfo.SummaryColumnBackgroundColor;
            this.SummaryColumnForeColor.Color = GridStyleInfo.SummaryColumnForegroundColor;
            this.SummaryRowBackColor.Color = GridStyleInfo.SummaryRowBackgroundColor;
            this.SummaryRowForeColor.Color = GridStyleInfo.SummaryRowForegroundColor;

            this.SummaryFontSize.Value = GridStyleInfo.SummaryFontSize;
            this.SummaryFontName.SelectedItem = GridStyleInfo.SummaryFontName;

            this.CellFontName.SelectedItem = GridStyleInfo.CellFontName;
            this.CellFontStyle.SelectedItem = GridStyleInfo.CellFontStyle;
            this.CellFontSize.Value = GridStyleInfo.CellFontSize;
            this.CellFontColor.Color = GridStyleInfo.CellFontColor;
            this.GridlineColor.Color = GridStyleInfo.GridlineColor;

        }

        /// <summary>
        /// Applies the styles to grid.
        /// </summary>
        public void ApplyStylesToGrid()
        {
            this.SetStyles();

            if (HeaderColumnBackColor.SelectedItem != null)
            {
                this.Grid.ColumnHeaderStyle.Background = Convert(this.HeaderColumnBackColor.Color);
            }
            if (HeaderColumnForeColor.SelectedItem != null)
            {
                this.Grid.ColumnHeaderStyle.Foreground = Convert(this.HeaderColumnForeColor.Color);
            }
            if (HeaderRowBackColor.SelectedItem != null)
            {
                this.Grid.RowHeaderStyle.Background = Convert(this.HeaderRowBackColor.Color);
            }
            if (HeaderRowForeColor.SelectedItem != null)
            {
                this.Grid.RowHeaderStyle.Foreground = Convert(this.HeaderRowForeColor.Color);
            }


            this.Grid.RowHeaderStyle.FontSize = this.Grid.ColumnHeaderStyle.FontSize = (int)this.HeaderFontSize.Value;
            if (this.HeaderFontName.SelectedItem != null && this.HeaderFontName.SelectedIndex >= 0)
            {
                this.Grid.RowHeaderStyle.FontFamily = this.Grid.ColumnHeaderStyle.FontFamily = new FontFamily(GridStyleInfo.fontFamily[this.HeaderFontName.SelectedIndex]);
            }
            if (this.CellFontName.SelectedItem != null)
            {
                this.Grid.SummaryColumnStyle.Background = Convert(this.SummaryColumnBackColor.Color);
            }
            if (SummaryColumnForeColor.SelectedItem != null)
            {
                this.Grid.SummaryColumnStyle.Foreground = Convert(this.SummaryColumnForeColor.Color);
            }
            if (SummaryRowBackColor.SelectedItem != null)
            {
                this.Grid.SummaryRowStyle.Background = Convert(this.SummaryRowBackColor.Color);
            }
            if (this.HeaderFontName.SelectedItem != null)
            {
                this.Grid.SummaryRowStyle.Foreground = Convert(this.SummaryRowForeColor.Color);
            }
            this.Grid.SummaryColumnStyle.FontSize = this.Grid.SummaryRowStyle.FontSize = (int)this.SummaryFontSize.Value;
            if (this.SummaryFontName.SelectedItem != null && this.SummaryFontName.SelectedIndex >= 0)
            {
                this.Grid.SummaryColumnStyle.FontFamily = this.Grid.SummaryRowStyle.FontFamily = new FontFamily(GridStyleInfo.fontFamily[this.SummaryFontName.SelectedIndex]);
            }

            if (CellFontColor.SelectedItem != null)
            {
                this.Grid.ValueCellStyle.Foreground = Convert(this.CellFontColor.Color);
            }

            this.Grid.ValueCellStyle.FontSize = (int)this.CellFontSize.Value;
            if (CellFontName.SelectedItem != null && this.CellFontName.SelectedIndex >= 0)
            {
                this.Grid.ValueCellStyle.FontFamily = new FontFamily(GridStyleInfo.fontFamily[this.CellFontName.SelectedIndex]);
            }
            if (this.GridlineColor.SelectedItem != null)
                this.Grid.GridLineStroke = Convert(this.GridlineColor.Color);

            this.Grid.GridStyleInfo = this.GridStyleInfo;
        }

        /// <summary>
        /// Set styles for Grid
        /// </summary>
        private void SetStyles()
        {
            if (this.Grid.ColumnHeaderStyle == null)
            {
                this.Grid.ColumnHeaderStyle = new OlapGridCellStyle();
            }
            if (this.Grid.RowHeaderStyle == null)
            {
                this.Grid.RowHeaderStyle = new OlapGridCellStyle();
            }
            if (this.Grid.SummaryColumnStyle == null)
            {
                this.Grid.SummaryColumnStyle = new OlapGridCellStyle();
            }
            if (this.Grid.SummaryRowStyle == null)
            {
                this.Grid.SummaryRowStyle = new OlapGridCellStyle();
            }
            if (this.Grid.ValueCellStyle == null)
            {
                this.Grid.ValueCellStyle = new OlapGridCellStyle();
            }
        }

        /// <summary>
        /// Default style for exporting files
        /// </summary>
        public void DefaultStyles()
        {
            GridStyleInfo = new ExportingGridStyleInfo();
            GridStyleInfo.LoadStyles();
            Color initial = new Color();
            initial.A = 255;
            initial.R = 173;
            initial.G = 216;
            initial.B = 230;
            GridStyleInfo.HeaderFontName = "Calibri";
            GridStyleInfo.HeaderBackgroundColor = initial;
            GridStyleInfo.HeaderRowBackgroundColor = initial;
            GridStyleInfo.HeaderRowForegroundColor = Colors.Black;
            GridStyleInfo.HeaderForeGroundColor = Colors.Black;
            GridStyleInfo.SummaryColumnBackgroundColor = initial;
            GridStyleInfo.SummaryColumnForegroundColor = Colors.Black;
            GridStyleInfo.SummaryRowBackgroundColor = initial;
            GridStyleInfo.SummaryRowForegroundColor = Colors.Black;
            GridStyleInfo.HeaderFontSize = 12f;
            GridStyleInfo.HeaderFontStyle = "Bold";
            GridStyleInfo.SummaryFontName = "Calibri";
            GridStyleInfo.SummaryFontSize = 12;
            GridStyleInfo.ValueTextColor = Colors.Black;
            GridStyleInfo.CellFontColor = Colors.Black;
            GridStyleInfo.CellFontName = "Calibri";
            GridStyleInfo.CellFontSize = 12;
            GridStyleInfo.CellFontStyle = "Bold";
            GridStyleInfo.GridBackColor = Colors.White;
            GridStyleInfo.GridlineColor = Colors.Black;
            GridStyleInfo.GridlineThickness = 0.5;


        }


        /// <summary>
        /// Converts the specified System.Windows.Media.Color to SolidColorBrush.
        /// </summary>
        /// <param name="value">System.Windows.Media.Color.</param>
        /// <returns></returns>
        public System.Windows.Media.SolidColorBrush Convert(System.Windows.Media.Color value)
        {
            string val = value.ToString();
            val = val.Replace("#", "");
            byte a = System.Convert.ToByte("ff", 16);
            byte pos = 0;
            if (val.Length == 8)
            {
                a = System.Convert.ToByte(val.Substring(pos, 2), 16);
                pos = 2;
            }
            byte r = System.Convert.ToByte(val.Substring(pos, 2), 16);
            pos += 2;
            byte g = System.Convert.ToByte(val.Substring(pos, 2), 16);
            pos += 2; byte b = System.Convert.ToByte(val.Substring(pos, 2), 16);
            System.Windows.Media.Color col = System.Windows.Media.Color.FromArgb(a, r, g, b);
            return new SolidColorBrush(col);
        }
    }
}

