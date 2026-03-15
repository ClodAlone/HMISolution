#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Syncfusion.Silverlight.Grid.Olap.Common
{
    /// <summary>
    /// ExportingGridStyleInfo contains a list of appearance properties which is used while exporting the 
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
            Color initial = new Color();
            initial.A = 255;
            initial.R = 173;
            initial.G = 216;
            initial.B = 230;
            this.HeaderFontName = "Calibri";
            SolidColorBrush defaultColor = Convert("#6698FF");
            this.HeaderBackgroundColor = Color.FromArgb(defaultColor.Color.A, defaultColor.Color.R, defaultColor.Color.G, defaultColor.Color.B);// initial;// new Color(initial);
            this.HeaderRowBackgroundColor = Color.FromArgb(defaultColor.Color.A, defaultColor.Color.R, defaultColor.Color.G, defaultColor.Color.B); //  initial;// new SolidColorBrush(initial);
            this.HeaderRowForegroundColor = Colors.Black;// new SolidColorBrush(Colors.Black);
            this.HeaderForeGroundColor = Colors.Black;// new SolidColorBrush(Colors.Black);
            defaultColor = Convert("#82CAFF");
            this.SummaryColumnBackgroundColor = Color.FromArgb(defaultColor.Color.A, defaultColor.Color.R, defaultColor.Color.G, defaultColor.Color.B);// initial;// new SolidColorBrush(initial);
            this.SummaryColumnForegroundColor = Colors.Black;// new SolidColorBrush(Colors.Black);
            this.SummaryRowBackgroundColor = Color.FromArgb(defaultColor.Color.A, defaultColor.Color.R, defaultColor.Color.G, defaultColor.Color.B);// initial;// new SolidColorBrush(initial);
            this.SummaryRowForegroundColor = Colors.Black;// new SolidColorBrush(Colors.Black);
            this.HeaderFontSize = 12f;
            this.HeaderFontStyle = "Bold";
            this.SummaryFontName = "Calibri";
            this.SummaryFontSize = 12;
            this.ValueTextColor = Colors.Black;// new SolidColorBrush(Colors.Black);
            this.CellFontColor = Colors.Black;
            this.CellFontName = "Calibri";
            this.CellFontSize = 12;
            this.CellFontStyle = "Bold";
            this.GridBackColor = Colors.White;
            this.GridlineColor = Colors.Black;
            this.GridlineThickness = 0.5;
        }
        #endregion

        #region Public Members

        /// <summary>
        /// List of font family names.
        /// </summary>
        public List<string> fontFamily;


        /// <summary>
        /// List of font size values.
        /// </summary>
        public List<string> fontStyle;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the color of the header background.
        /// </summary>
        /// <value>The color of the header background.</value>
        public Color HeaderBackgroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the header fore ground.
        /// </summary>
        /// <value>The color of the header fore ground.</value>
        public Color HeaderForeGroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the header row background.
        /// </summary>
        /// <value>The color of the header row background.</value>
        public Color HeaderRowBackgroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the header row foreground.
        /// </summary>
        /// <value>The color of the header row foreground.</value>
        public Color HeaderRowForegroundColor
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
        /// Gets or sets the header font style.
        /// </summary>
        /// <value>The header font style.</value>
        public String HeaderFontStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the summary column background.
        /// </summary>
        /// <value>The color of the summary column background.</value>
        public Color SummaryColumnBackgroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the summary column foreground.
        /// </summary>
        /// <value>The color of the summary column foreground.</value>
        public Color SummaryColumnForegroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the summary row background.
        /// </summary>
        /// <value>The color of the summary row background.</value>
        public Color SummaryRowBackgroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the summary row foreground.
        /// </summary>
        /// <value>The color of the summary row foreground.</value>
        public Color SummaryRowForegroundColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the value text.
        /// </summary>
        /// <value>The color of the value text.</value>
        public Color ValueTextColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the gridline.
        /// </summary>
        /// <value>The color of the gridline.</value>
        public Color GridlineColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the grid back.
        /// </summary>
        /// <value>The color of the grid back.</value>
        public Color GridBackColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the gridline thickness.
        /// </summary>
        /// <value>The gridline thickness.</value>
        public double GridlineThickness
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
        public double SummaryFontSize
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the cell font.
        /// </summary>
        /// <value>The color of the cell font.</value>
        public Color CellFontColor
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
        /// Gets or sets the summary font style.
        /// </summary>
        /// <value>The summary font style.</value>
        public string SummaryFontStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the size of the cell font.
        /// </summary>
        /// <value>The size of the cell font.</value>
        public double CellFontSize
        {
            get;
            set;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the styles.
        /// </summary>
        public void LoadStyles()
        {
            fontStyle = new List<String>();
            fontStyle.Add("Bold");
            fontStyle.Add("Normal");
            fontFamily = new List<string>();

            fontFamily.Add("Courier");
            fontFamily.Add("Helvetica");
            //fontFamily.Add("Symbol");
            fontFamily.Add("TimesRoman");
            fontFamily.Add("ZapfDingbats");
        }

        /// <summary>
        /// Converts the specified System.Windows.Media.Color to SolidColorBrush.
        /// </summary>
        /// <param name="value">System.Windows.Media.Color.</param>
        /// <returns></returns>
        private System.Windows.Media.SolidColorBrush Convert(System.Windows.Media.Color value)//, Type targetType, object parameter)//, System.Globalization.CultureInfo culture)
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

        /// <summary>
        /// Converts the specified System.Windows.Media.Color to SolidColorBrush.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private System.Windows.Media.SolidColorBrush Convert(string value)
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

        #endregion
    }
}
