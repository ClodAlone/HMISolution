#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace Syncfusion.Windows.Grid.Olap.Common
{
    /// <summary>
    /// ExportingGridStyleInfo contains a list of apperance properties which is used while exporting the 
    /// pivot data to excel,word,pdf
    /// </summary>
    public class ExportingGridStyleInfo
    {
        #region Constructor

        /// <summary>
        /// Sets default value for the properties
        /// </summary>
        public ExportingGridStyleInfo()
        {
            this.LoadStyles();
            this.HeaderBackgroundColor = "#6698FF";
            this.HeaderForeGroundColor = Colors.Black.ToString();
            this.HeaderRowBackgroundColor = "#6698FF";
            this.HeaderRowForegroundColor = Colors.Black.ToString();
            this.SummaryColumnBackgroundColor = "#82CAFF";
            this.SummaryColumnForegroundColor = Colors.Black.ToString();
            this.SummaryRowBackgroundColor = "#82CAFF";
            this.SummaryRowForegroundColor = Colors.Black.ToString();
            this.CellFontColor = Colors.Black.ToString();
            this.HeaderFontName = "Arial";
            this.HeaderFontSize = 10;
            this.SummaryFontName = "Arial";
            this.SummaryFontSize = 10;
            this.SummaryFontSize = 10;
            this.CellFontName = "Arial";
            this.CellFontSize = 10;
            this.CellFontStyle = "Bold";
            this.GridBackGround = "#F5F5F5";
            this.GridBorderColor = "#2554C7";
            this.GridThickness = 0.5f;
        }

        #endregion

        #region Internal Members

        internal List<String> colorList;       
        internal Hashtable hstColor = new Hashtable();

        #endregion

        #region Public Members

        /// <summary>
        /// List of font size values.
        /// </summary>
        public List<float> fontSize;

        /// <summary>
        /// List of font style names.
        /// </summary>
        public List<String> fontStyle;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the color of the header background.
        /// </summary>
        /// <value>The color of the header background.</value>
        public String HeaderBackgroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the header fore ground.
        /// </summary>
        /// <value>The color of the header fore ground.</value>
        public String HeaderForeGroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the header row background.
        /// </summary>
        /// <value>The color of the header row background.</value>
        public String HeaderRowBackgroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the header row foreground.
        /// </summary>
        /// <value>The color of the header row foreground.</value>
        public String HeaderRowForegroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name of the header font.
        /// </summary>
        /// <value>The name of the header font.</value>
        public String HeaderFontName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the size of the header font.
        /// </summary>
        /// <value>The size of the header font.</value>
        public double HeaderFontSize
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the header font syle.
        /// </summary>
        /// <value>The header font syle.</value>
        public String HeaderFontStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the summary column background.
        /// </summary>
        /// <value>The color of the summary column background.</value>
        public String SummaryColumnBackgroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the summary column foreground.
        /// </summary>
        /// <value>The color of the summary column foreground.</value>
        public String SummaryColumnForegroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the summary row background.
        /// </summary>
        /// <value>The color of the summary row background.</value>
        public String SummaryRowBackgroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the summary row foreground.
        /// </summary>
        /// <value>The color of the summary row foreground.</value>
        public String SummaryRowForegroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the value text.
        /// </summary>
        /// <value>The color of the value text.</value>
        public String ValueTextColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name of the summary font.
        /// </summary>
        /// <value>The name of the summary font.</value>
        public String SummaryFontName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the size of the summary font.
        /// </summary>
        /// <value>The size of the summary font.</value>
        public float SummaryFontSize
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the cell font.
        /// </summary>
        /// <value>The color of the cell font.</value>
        public String CellFontColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name of the cell font.
        /// </summary>
        /// <value>The name of the cell font.</value>
        public String CellFontName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cell font style.
        /// </summary>
        /// <value>The cell font style.</value>
        public String CellFontStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the size of the cell font.
        /// </summary>
        /// <value>The size of the cell font.</value>
        public float CellFontSize
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the grid back ground.
        /// </summary>
        /// <value>The grid back ground.</value>
        public String GridBackGround
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the grid border.
        /// </summary>
        /// <value>The color of the grid border.</value>
        public String GridBorderColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the grid thickness.
        /// </summary>
        /// <value>The grid thickness.</value>
        public Double GridThickness
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [apply column header style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply column header style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyColumnHeaderStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [apply row header style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply row header style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyRowHeaderStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [apply header font style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply header font style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyHeaderFontStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [apply summary column style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply summary column style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplySummaryColumnStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [apply summary row style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply summary row style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplySummaryRowStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [apply summary header font style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply summary header font style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplySummaryHeaderFontStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [apply value cell style].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [apply value cell style]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyValueCellStyle
        {
            get;
            set;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Loads the style information from Metadata
        /// </summary>
        public void LoadStyles()
        {
            colorList = new List<String>();
            Type type = typeof(System.Windows.Media.Colors);
            PropertyInfo[] properties = type.GetProperties();
            foreach (var item1 in properties)
            {
                object color = type.InvokeMember(item1.Name, BindingFlags.GetProperty, null, null, null);
                colorList.Add(item1.Name);
                if (!hstColor.Contains(color))
                    hstColor[color.ToString()] = item1.Name;
            }
            fontStyle = new List<String>();
            fontStyle.Add("Bold");
            fontStyle.Add("Normal");
            fontSize = new List<float>();
            for (int i = 10; i < 16; i++)
            {
                fontSize.Add(i);
            }
        }
        #endregion
    }
}
