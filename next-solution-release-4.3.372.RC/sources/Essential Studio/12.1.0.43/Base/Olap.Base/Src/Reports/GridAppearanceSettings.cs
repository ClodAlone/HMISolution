#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if SILVERLIGHT
namespace Syncfusion.OlapSilverlight.Reports
{
    public class GridAppearanceSettings
    {
#else
using Syncfusion.Olap.Common;
namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// This will hold the Grid Appearance settings for serialization.
    /// </summary>
    [Serializable]
    public class GridAppearanceSettings : ICloneable<GridAppearanceSettings>
    {
#endif
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridAppearanceSettings"/> class.
        /// </summary>
        public GridAppearanceSettings()
        {
            this.FreezeHeader = true;
            this.ResizeColumnsToFit = true;
            this.ResizeRowsToFit = false;
            this.ShowValueCellTooltip = true;
            this.ShowHyperlink = false;
        }
        #endregion

        #region Grid Toolbar properties
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public string Background { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [allow selection].
        /// </summary>
        /// <value><c>true</c> if [allow selection]; otherwise, <c>false</c>.</value>
        public bool AllowSelection { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [freeze header].
        /// </summary>
        /// <value><c>true</c> if [freeze header]; otherwise, <c>false</c>.</value>
        public bool FreezeHeader { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show value cell tooltip].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show value cell tooltip]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowValueCellTooltip { get; set; }
#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets a value indicating whether [show header cell tooltip].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show header cell tooltip]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowHeaderCellTooltip { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show member properties tool tip].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show member properties tool tip]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowMemberPropertiesToolTip { get; set; } 
#endif
        /// <summary>
        /// Gets or sets the grid layout.
        /// </summary>
        /// <value>The grid layout.</value>
        public string GridLayout { get; set; }
        /// <summary>
        /// Gets or sets the grid styles.
        /// </summary>
        /// <value>The grid styles.</value>
        public GridStyles GridStyles { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [allow resize rows].
        /// </summary>
        /// <value><c>true</c> if [allow resize rows]; otherwise, <c>false</c>.</value>
        public bool AllowResizeRows { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [allow resize columns].
        /// </summary>
        /// <value><c>true</c> if [allow resize columns]; otherwise, <c>false</c>.</value>
        public bool AllowResizeColumns { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [resize rows to fit].
        /// </summary>
        /// <value><c>true</c> if [resize rows to fit]; otherwise, <c>false</c>.</value>
        public bool ResizeRowsToFit { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [resize columns to fit].
        /// </summary>
        /// <value><c>true</c> if [resize columns to fit]; otherwise, <c>false</c>.</value>
        public bool ResizeColumnsToFit { get; set; }
        /// <summary>
        /// Gets or sets the value cell horizontal alignment.
        /// </summary>
        /// <value>The value cell horizontal alignment.</value>
        public string ValueCellHorizontalAlignment { get; set; }
        /// <summary>
        /// Gets or sets the Hyperlink should be Enabled in OlapGrid (Applicable only for Asp.Net OlapClient).
        /// </summary>
        /// <value><c>true</c> if EnableHyperLink is true; otherwise, <c>false</c>.</value>
        public bool ShowHyperlink { get; set; }
        #endregion

#if !SILVERLIGHT
        #region IClonnable members
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public GridAppearanceSettings Clone()
        {
            GridAppearanceSettings gridSettings = new GridAppearanceSettings();
            gridSettings.AllowSelection = this.AllowSelection;
            gridSettings.FreezeHeader = this.FreezeHeader;
            gridSettings.ShowValueCellTooltip = this.ShowValueCellTooltip;
            gridSettings.GridLayout = this.GridLayout;
            gridSettings.Background = this.Background;
            gridSettings.GridStyles = this.GridStyles;
            gridSettings.AllowResizeRows = this.AllowResizeRows;
            gridSettings.AllowResizeColumns = this.AllowResizeColumns;
            gridSettings.ResizeColumnsToFit = this.ResizeColumnsToFit;
            gridSettings.ResizeRowsToFit = this.ResizeRowsToFit;
            gridSettings.ValueCellHorizontalAlignment = this.ValueCellHorizontalAlignment;
            gridSettings.ShowHyperlink = this.ShowHyperlink;
            return gridSettings;
        }        
        #endregion
#endif
    }


#if SILVERLIGHT
    public class GridStyles 
#else
    /// <summary>
    /// This class will holds the Grid Styles for its appearance.
    /// </summary>
    [Serializable]
    public class GridStyles
#endif
    {
        #region Grid Header/Value Cell Appearance

        /// <summary>
        /// Gets or sets the row header foreground.
        /// </summary>
        /// <value>The row header foreground.</value>
        public string RowHeaderForeground { get; set; }
        /// <summary>
        /// Gets or sets the row header background.
        /// </summary>
        /// <value>The row header background.</value>
        public string RowHeaderBackground { get; set; }
        /// <summary>
        /// Gets or sets the column header foreground.
        /// </summary>
        /// <value>The column header foreground.</value>
        public string ColumnHeaderForeground { get; set; }
        /// <summary>
        /// Gets or sets the column header background.
        /// </summary>
        /// <value>The column header background.</value>
        public string ColumnHeaderBackground { get; set; }
        /// <summary>
        /// Gets or sets the header cell font family.
        /// </summary>
        /// <value>The header cell font family.</value>
        public string HeaderCellFontFamily { get; set; }
        /// <summary>
        /// Gets or sets the size of the header cell font.
        /// </summary>
        /// <value>The size of the header cell font.</value>
        public double HeaderCellFontSize { get; set; }

        /// <summary>
        /// Gets or sets the row summary foreground.
        /// </summary>
        /// <value>The row summary foreground.</value>
        public string RowSummaryForeground { get; set; }
        /// <summary>
        /// Gets or sets the row summary background.
        /// </summary>
        /// <value>The row summary background.</value>
        public string RowSummaryBackground { get; set; }
        /// <summary>
        /// Gets or sets the column summary foreground.
        /// </summary>
        /// <value>The column summary foreground.</value>
        public string ColumnSummaryForeground { get; set; }
        /// <summary>
        /// Gets or sets the column summary background.
        /// </summary>
        /// <value>The column summary background.</value>
        public string ColumnSummaryBackground { get; set; }
        /// <summary>
        /// Gets or sets the summary cell font family.
        /// </summary>
        /// <value>The summary cell font family.</value>
        public string SummaryCellFontFamily { get; set; }
        /// <summary>
        /// Gets or sets the size of the summary cell font.
        /// </summary>
        /// <value>The size of the summary cell font.</value>
        public double SummaryCellFontSize { get; set; }

        /// <summary>
        /// Gets or sets the value cell background.
        /// </summary>
        /// <value>The value cell background.</value>
        public string ValueCellBackground { get; set; }
        /// <summary>
        /// Gets or sets the value cell foreground.
        /// </summary>
        /// <value>The value cell foreground.</value>
        public string ValueCellForeground { get; set; }
        /// <summary>
        /// Gets or sets the value cell font family.
        /// </summary>
        /// <value>The value cell font family.</value>
        public string ValueCellFontFamily { get; set; }
        /// <summary>
        /// Gets or sets the size of the value cell font.
        /// </summary>
        /// <value>The size of the value cell font.</value>
        public double ValueCellFontSize { get; set; }
        /// <summary>
        /// Gets or sets the value cell font style.
        /// </summary>
        /// <value>The value cell font style.</value>
        public string ValueCellFontStyle { get; set; }

        /// <summary>
        /// Gets or sets the grid line stroke.
        /// </summary>
        /// <value>The grid line stroke.</value>
        public string GridLineStroke { get; set; }
        /// <summary>
        /// Gets or sets the grid line thickness.
        /// </summary>
        /// <value>The grid line thickness.</value>
        public double GridLineThickness { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets a value indicating whether [apply column header style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply column header style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyColumnHeaderStyle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [apply row header style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply row header style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyRowHeaderStyle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [apply summary column style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply summary column style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplySummaryColumnStyle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [apply summary row style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply summary row style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplySummaryRowStyle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [apply header font style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply header font style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyHeaderFontStyle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [apply summary header font style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply summary header font style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplySummaryHeaderFontStyle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [apply value cell style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply value cell style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyValueCellStyle { get; set; }
#endif

        #endregion
    }
}
