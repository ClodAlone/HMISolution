#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    ///  DateTimePickerAdv CalendarStore class.
    /// </summary>
    internal class DateTimePickerAdvCalendarStore
    {
        private Font font;
        private Color foreColor;
        private Color monthBackground;
        private Size size;
        private Color titleBackColor;
        private Color titleForeColor;
        private Color trailingForeColor;
        private CultureInfo culture;
        private DateTime minDate;
        private DateTime maxDate;
        private bool themedEnabledGrid = false;
        private bool themedEnabledScrollButtons = false;
        private Size popupSize;
        private ImageList imageList;
        private bool noneEnabled = true;
        private bool sizeToFit = true;
        private bool enableNullDate = true;

        public bool EnableNullDate
        {
            get 
            { 
                return enableNullDate; 
            }
            set
            { 
                enableNullDate = value; 
            }
        }

        public bool SizeToFit
        {
            get { return sizeToFit; }
            set { sizeToFit = value; }
        }

        public bool NoneEnabled
        {
            get { return noneEnabled; }
            set { noneEnabled = value; }
        }

        public ImageList ImageList
        {
            get { return imageList; }
            set { imageList = value; }
        }

        public Size PopupSize
        {
            get { return this.popupSize; }
            set { this.popupSize = value; }
        }

        public bool ThemedEnabledGrid
        {
            get { return this.themedEnabledGrid; }
            set { this.themedEnabledGrid = value; }
        }
        public bool ThemedEnabledScrollButtons
        {
            get { return this.themedEnabledScrollButtons; }
            set { this.themedEnabledScrollButtons = value; }
        }

        public DateTime MaxDate
        {
            get { return maxDate; }
            set { maxDate = value; }
        }

        public DateTime MinDate
        {
            get { return minDate; }
            set { minDate = value; }
        }
        public CultureInfo Culture
        {
            get { return culture; }
            set { culture = value; }
        }

        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }
        public Color MonthBackground
        {
            get { return monthBackground; }
            set { monthBackground = value; }
        }
        public Size Size
        {
            get { return size; }
            set { size = value; }
        }
        public Color TitleBackColor
        {
            get { return titleBackColor; }
            set { titleBackColor = value; }
        }
        public Color TitleForeColor
        {
            get { return titleForeColor; }
            set { titleForeColor = value; }
        }
        public Color TrailingForeColor
        {
            get { return trailingForeColor; }
            set { trailingForeColor = value; }
        }
        public DateTimePickerAdvCalendarStore()
        {
        }
    }
}
