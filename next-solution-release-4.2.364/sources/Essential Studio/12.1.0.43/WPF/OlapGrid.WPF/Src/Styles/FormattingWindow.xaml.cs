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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.Reflection;
using Syncfusion.Windows.Controls.Cells;
using System.ComponentModel;
using Syncfusion.Windows.Grid.Olap.Common;

namespace Syncfusion.Windows.Grid.Olap
{

    /// <summary>
    /// Formatting window is used to change the apperance of grid
    /// </summary>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public partial class FormattingWindow : ChromelessWindow
    {
        #region Constructor
        /// <summary>
        /// Constructor to initialize the window
        /// </summary>
        public FormattingWindow(OlapGrid olapGrid)
        {
            InitializeComponent();
            this.grdColumnHeader.IsEnabled = false;
            this.grdRowHeader.IsEnabled = false;
            this.grdHeaderFont.IsEnabled = false;
            this.grdColumnSummary.IsEnabled = false;
            this.grdRowSummary.IsEnabled = false;
            this.grdSummaryFont.IsEnabled = false;
            this.grdCellStyles.IsEnabled = false;
            this.grdCommonStyles.IsEnabled = false;
            ProcessCheckBox(olapGrid.GridStyleInfo);

            this.SummaryFontSize.ItemsSource = GridStyleInfo.fontSize;
            this.CellFontSize.ItemsSource = GridStyleInfo.fontSize;
            this.CellFontStyle.ItemsSource = GridStyleInfo.fontStyle;
            this.HeaderFontSize.ItemsSource = GridStyleInfo.fontSize;
            this.Grid = olapGrid;

            this.Grid.GridStyleInfo = olapGrid.GridStyleInfo;
            this.GridStyleInfo = olapGrid.GridStyleInfo;
            this.InitializeStyles();
            this.FlowDirection = olapGrid.FlowDirection;
            this.Title = Syncfusion.Windows.Grid.Olap.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapGrid_Dialog_Title");
            
        }

        private void ProcessCheckBox(ExportingGridStyleInfo exportingGridStyleInfo)
        {
            if (exportingGridStyleInfo.ApplyColumnHeaderStyle)
            {
                this.chkColumnHeader.IsChecked = true;
            }
            else
                this.chkColumnHeader.IsChecked = false;

            if (exportingGridStyleInfo.ApplyHeaderFontStyle)
            {
                //this.grdHeaderFont.IsEnabled = true;
                this.chkHeaderFont.IsChecked = true;
                //this.HeaderFontSize.IsEnabled = true;
                //this.HeaderFontName.IsEnabled = true;
            }
            else
                this.chkHeaderFont.IsChecked = false;

            if (exportingGridStyleInfo.ApplyRowHeaderStyle)
            {
                this.chkRowHeader.IsChecked = true;
            }
            else
                this.chkRowHeader.IsChecked = false;

            if (exportingGridStyleInfo.ApplySummaryColumnStyle)
            {
                this.chkColumnSummary.IsChecked = true;
            }
            else
                this.chkColumnSummary.IsChecked = false;

            if (exportingGridStyleInfo.ApplySummaryHeaderFontStyle)
            {
                this.chkSummaryFont.IsChecked = true;
            }
            else
                this.chkSummaryFont.IsChecked = false;

            if (exportingGridStyleInfo.ApplySummaryRowStyle)
            {
                this.chkRowSummary.IsChecked = true;
            }
            else
                this.chkRowSummary.IsChecked = false;

            if (exportingGridStyleInfo.ApplyValueCellStyle)
            {
                this.chkCellStyles.IsChecked = true;
            }
            else
                this.chkCellStyles.IsChecked = false;

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        internal OlapGrid Grid
        {
            get;
            set;
        }
     
        /// <summary>
        /// Create instance for ExportingGridStyleInfo
        /// </summary>
        public ExportingGridStyleInfo GridStyleInfo =new ExportingGridStyleInfo();

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the default styles.
        /// </summary>
        public void LoadDefaultStyles()
        {
            GridStyleInfo.HeaderBackgroundColor = "#6698FF";
            GridStyleInfo.HeaderForeGroundColor = Colors.White.ToString();
            GridStyleInfo.HeaderRowBackgroundColor = "#6698FF";
            GridStyleInfo.HeaderRowForegroundColor = Colors.White.ToString();
            GridStyleInfo.SummaryColumnBackgroundColor = "#82CAFF";
            GridStyleInfo.SummaryColumnForegroundColor = Colors.White.ToString();
            GridStyleInfo.SummaryRowBackgroundColor = "#82CAFF";
            GridStyleInfo.SummaryRowForegroundColor = Colors.White.ToString();
            GridStyleInfo.CellFontColor = Colors.Black.ToString();
            GridStyleInfo.HeaderFontName = "Arial";
            GridStyleInfo.HeaderFontSize = 12f;
            GridStyleInfo.SummaryFontName = "Arial";
            GridStyleInfo.SummaryFontSize = 12f;
            GridStyleInfo.SummaryFontSize = 12;
            GridStyleInfo.CellFontName = "Arial";
            GridStyleInfo.CellFontSize = 12f;
            GridStyleInfo.CellFontStyle = "Bold";
            GridStyleInfo.GridBackGround = "#F5F5F5";
            GridStyleInfo.GridBorderColor = "#2554C7";
            GridStyleInfo.GridThickness = 0.5f;
        }

        /// <summary>
        /// Apply Style information to OlapGrid
        /// </summary>
        /// <param name="olapgrid1">OlapGrid</param>
        public void ApplyStylesToGrid(OlapGrid olapgrid1)
        {
            if (GridStyleInfo.ApplyColumnHeaderStyle)
            {
                Brush HeaderColumnBackColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderBackgroundColor));
                Brush HeaderColumnForeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderForeGroundColor));
                olapgrid1.ColumnHeaderStyle.Background = HeaderColumnBackColor;
                olapgrid1.ColumnHeaderStyle.Foreground = HeaderColumnForeColor;
            }

            if (GridStyleInfo.ApplyRowHeaderStyle)
            {
                Brush HeaderRowBackColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderRowBackgroundColor));
                Brush HeaderRowForeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderRowForegroundColor));
                olapgrid1.RowHeaderStyle.Background = HeaderRowBackColor;
                olapgrid1.RowHeaderStyle.Foreground = HeaderRowForeColor;
            }

            if (GridStyleInfo.ApplySummaryColumnStyle)
            {
                Brush SummaryColumnBackColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryColumnBackgroundColor));
                Brush SummaryColumnForeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryColumnForegroundColor));
                olapgrid1.SummaryColumnStyle.Background = SummaryColumnBackColor;
                olapgrid1.SummaryColumnStyle.Foreground = SummaryColumnForeColor;
            }

            if (GridStyleInfo.ApplySummaryRowStyle)
            {
                Brush SummaryRowBackColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryRowBackgroundColor));
                Brush SummaryRowForeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryRowForegroundColor));
                olapgrid1.SummaryRowStyle.Background = SummaryRowBackColor;
                olapgrid1.SummaryRowStyle.Foreground = SummaryRowForeColor;
            }

           // Brush GridBackGroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.GridBackGround));
           // Brush GridBorderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.GridBorderColor));

            if (GridStyleInfo.ApplyHeaderFontStyle)
            {
                olapgrid1.ColumnHeaderStyle.FontSize = (int)GridStyleInfo.HeaderFontSize;
                olapgrid1.RowHeaderStyle.FontSize = (int)GridStyleInfo.HeaderFontSize;
                olapgrid1.ColumnHeaderStyle.FontFamily = new FontFamily(GridStyleInfo.HeaderFontName);
                olapgrid1.RowHeaderStyle.FontFamily = new FontFamily(GridStyleInfo.HeaderFontName);
            }

            if (GridStyleInfo.ApplySummaryHeaderFontStyle)
            {
                olapgrid1.SummaryRowStyle.FontFamily = new FontFamily(GridStyleInfo.SummaryFontName);
                olapgrid1.SummaryRowStyle.FontSize = (int)GridStyleInfo.SummaryFontSize;
                olapgrid1.SummaryColumnStyle.FontFamily = new FontFamily(GridStyleInfo.SummaryFontName);
                olapgrid1.SummaryColumnStyle.FontSize = (int)GridStyleInfo.SummaryFontSize;
            }

            if (GridStyleInfo.ApplyValueCellStyle)
            {
                Brush CellFont = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.CellFontColor));
                olapgrid1.ValueCellStyle = new OlapGridCellStyle();
                olapgrid1.ValueCellStyle.Foreground = CellFont;
                olapgrid1.ValueCellStyle.FontFamily = new FontFamily(GridStyleInfo.CellFontName);
                olapgrid1.ValueCellStyle.FontSize = (int)GridStyleInfo.CellFontSize;
            }
            //olapgrid1.Background = GridBackGroundColor;
            //olapgrid1.Borders = new Pen(GridBorderColor, GridStyleInfo.GridThickness);
            //olapgrid1.GridLineColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridStyleInfo.GridBorderColor));
        }

        /// <summary>
        /// Initializes the styles.
        /// </summary>
        public void InitializeStyles()
        {          
            this.HeaderColumnBackColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderBackgroundColor.ToString());
            this.HeaderColumnForeColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderForeGroundColor.ToString());
            this.HeaderRowBackColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderRowBackgroundColor.ToString());
            this.HeaderRowForeColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.HeaderRowForegroundColor.ToString());
            this.SummaryColumnBackColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryColumnBackgroundColor.ToString());
            this.SummaryColumnForeColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryColumnForegroundColor.ToString());
            this.SummaryRowBackColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryRowBackgroundColor.ToString());
            this.SummaryRowForeColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.SummaryRowForegroundColor.ToString());
            this.SummaryFontName.SelectedFontFamily =new FontFamily(GridStyleInfo.SummaryFontName);
            this.SummaryFontSize.SelectedItem = GridStyleInfo.SummaryFontSize;
            this.HeaderFontSize.SelectedItem = (float)GridStyleInfo.HeaderFontSize;
            this.HeaderFontName.SelectedFontFamily =new FontFamily(GridStyleInfo.HeaderFontName);
            this.CellFontName.SelectedFontFamily=new FontFamily(GridStyleInfo.CellFontName);
            this.CellFontSize.SelectedItem = GridStyleInfo.CellFontSize;
            this.CellFontStyle.SelectedItem = GridStyleInfo.CellFontStyle;
            this.CellFontColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.CellFontColor);
            this.GridBackGroundColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.GridBackGround);
            //this.GridlinesColor.Color = (Color)ColorConverter.ConvertFromString(GridStyleInfo.GridBorderColor);
            //this.GridlineThickness.Value = GridStyleInfo.GridThickness;
        }
        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the OK Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.chkColumnHeader.IsChecked == true)
            {
                GridStyleInfo.HeaderBackgroundColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (HeaderColumnBackColor.Color.R), (HeaderColumnBackColor.Color.G), (HeaderColumnBackColor.Color.B), (HeaderColumnBackColor.Color.A));//HeaderColumnBackColor.Color;
                GridStyleInfo.HeaderForeGroundColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (HeaderColumnForeColor.Color.R), (HeaderColumnForeColor.Color.G), (HeaderColumnForeColor.Color.B), (HeaderColumnForeColor.Color.A));
                GridStyleInfo.ApplyColumnHeaderStyle = true;                
            }

            if (this.chkRowHeader.IsChecked == true)
            {
                GridStyleInfo.HeaderRowBackgroundColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (HeaderRowBackColor.Color.R), (HeaderRowBackColor.Color.G), (HeaderRowBackColor.Color.B), (HeaderRowBackColor.Color.A));
                GridStyleInfo.HeaderRowForegroundColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (HeaderRowForeColor.Color.R), (HeaderRowForeColor.Color.G), (HeaderRowForeColor.Color.B), (HeaderRowForeColor.Color.A));
                GridStyleInfo.ApplyRowHeaderStyle = true;
            }

            if (this.chkHeaderFont.IsChecked == true)
            {
                if (this.HeaderFontSize.SelectedIndex >= 0)
                {
                    GridStyleInfo.HeaderFontSize = GridStyleInfo.fontSize[this.HeaderFontSize.SelectedIndex];
                }
                GridStyleInfo.HeaderFontName = HeaderFontName.SelectedFontFamily.ToString();
                GridStyleInfo.ApplyHeaderFontStyle = true;
            }

            if (this.chkColumnSummary.IsChecked == true)
            {
                GridStyleInfo.SummaryColumnBackgroundColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (SummaryColumnBackColor.Color.R), (SummaryColumnBackColor.Color.G), (SummaryColumnBackColor.Color.B), (SummaryColumnBackColor.Color.A));
                GridStyleInfo.SummaryColumnForegroundColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (SummaryColumnForeColor.Color.R), (SummaryColumnForeColor.Color.G), (SummaryColumnForeColor.Color.B), (SummaryColumnForeColor.Color.A));
                GridStyleInfo.ApplySummaryColumnStyle = true;
            }

            if (chkRowSummary.IsChecked == true)
            {
                GridStyleInfo.SummaryRowBackgroundColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (SummaryRowBackColor.Color.R), (SummaryRowBackColor.Color.G), (SummaryRowBackColor.Color.B), (SummaryRowBackColor.Color.A));
                GridStyleInfo.SummaryRowForegroundColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (SummaryRowForeColor.Color.R), (SummaryRowForeColor.Color.G), (SummaryRowForeColor.Color.B), (SummaryRowForeColor.Color.A));
                GridStyleInfo.ApplySummaryRowStyle = true;
            }

            if (chkSummaryFont.IsChecked == true)
            {
                if (this.SummaryFontSize.SelectedIndex >= 0)
                {
                    GridStyleInfo.SummaryFontSize = GridStyleInfo.fontSize[this.SummaryFontSize.SelectedIndex];
                }
                GridStyleInfo.SummaryFontName = SummaryFontName.SelectedFontFamily.ToString();
                GridStyleInfo.ApplySummaryHeaderFontStyle = true;
            }

            if (chkCellStyles.IsChecked == true)
            {
                GridStyleInfo.CellFontColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (CellFontColor.Color.R), (CellFontColor.Color.G), (CellFontColor.Color.B), (CellFontColor.Color.A));
                GridStyleInfo.CellFontName = CellFontName.SelectedFontFamily.ToString();
                if (this.CellFontSize.SelectedIndex >= 0)
                {
                    GridStyleInfo.CellFontSize = GridStyleInfo.fontSize[this.CellFontSize.SelectedIndex];
                }
                if (this.CellFontStyle.SelectedIndex >= 0)
                {
                    GridStyleInfo.CellFontStyle = GridStyleInfo.fontStyle[this.CellFontStyle.SelectedIndex];
                }
                GridStyleInfo.ApplyValueCellStyle = true;
            }

            //GridStyleInfo.GridBackGround = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (GridBackGroundColor.Color.R), (GridBackGroundColor.Color.G), (GridBackGroundColor.Color.B), (GridBackGroundColor.Color.A));
            //GridStyleInfo.GridBorderColor = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", (GridlinesColor.Color.R), (GridlinesColor.Color.G), (GridlinesColor.Color.B), (GridlinesColor.Color.A));
            //GridStyleInfo.GridThickness = GridlineThickness.Value;
            //if (ApplyStyle)
            {
                this.ApplyStylesToGrid(this.Grid);
            }
            this.Grid.GridStyleInfo = this.GridStyleInfo;
            this.Close();
            this.Grid.DataBind();
        }

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Checked event of the chkColumnHeader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkColumnHeader_Checked(object sender, RoutedEventArgs e)
        {
            this.grdColumnHeader.IsEnabled = true;
            //this.grdRowHeader.IsEnabled = false;
            //this.grdHeaderFont.IsEnabled = false;
        }

        /// <summary>
        /// Handles the Unchecked event of the chkColumnHeader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkColumnHeader_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdColumnHeader.IsEnabled = false;
            //this.grdRowHeader.IsEnabled = true;
            //this.grdHeaderFont.IsEnabled = true;
            this.GridStyleInfo.ApplyColumnHeaderStyle = false;
        }

        /// <summary>
        /// Handles the Checked event of the chkRowHeader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkRowHeader_Checked(object sender, RoutedEventArgs e)
        {
            this.grdRowHeader.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Unchecked event of the chkRowHeader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkRowHeader_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdRowHeader.IsEnabled = false;
            this.GridStyleInfo.ApplyRowHeaderStyle = false;
        }

        /// <summary>
        /// Handles the Checked event of the chkHeaderFont control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkHeaderFont_Checked(object sender, RoutedEventArgs e)
        {
            this.grdHeaderFont.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Unchecked event of the chkHeaderFont control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkHeaderFont_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdHeaderFont.IsEnabled = false;
            this.GridStyleInfo.ApplyHeaderFontStyle = false;
        }

        /// <summary>
        /// Handles the Checked event of the chkColumnSummary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkColumnSummary_Checked(object sender, RoutedEventArgs e)
        {
            this.grdColumnSummary.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Unchecked event of the chkColumnSummary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkColumnSummary_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdColumnSummary.IsEnabled = false;
            this.GridStyleInfo.ApplySummaryColumnStyle = false;
        }

        /// <summary>
        /// Handles the Checked event of the chkRowSummary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkRowSummary_Checked(object sender, RoutedEventArgs e)
        {
            this.grdRowSummary.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Unchecked event of the chkRowSummary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkRowSummary_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdRowSummary.IsEnabled = false;
            this.GridStyleInfo.ApplySummaryRowStyle = false;
        }

        /// <summary>
        /// Handles the Checked event of the chkSummaryFont control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkSummaryFont_Checked(object sender, RoutedEventArgs e)
        {
            this.grdSummaryFont.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Unchecked event of the chkSummaryFont control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkSummaryFont_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdSummaryFont.IsEnabled = false;
            this.GridStyleInfo.ApplySummaryHeaderFontStyle = false;
        }

        /// <summary>
        /// Handles the Checked event of the chkCellStyles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCellStyles_Checked(object sender, RoutedEventArgs e)
        {
            this.grdCellStyles.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Unchecked event of the chkCellStyles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCellStyles_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdCellStyles.IsEnabled = false;
            this.GridStyleInfo.ApplyValueCellStyle = false;
        }

        /// <summary>
        /// Handles the Checked event of the chkCommonStyles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCommonStyles_Checked(object sender, RoutedEventArgs e)
        {
            this.grdCommonStyles.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Unchecked event of the chkCommonStyles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCommonStyles_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdCommonStyles.IsEnabled = false;
        }

        #endregion
    }
}
