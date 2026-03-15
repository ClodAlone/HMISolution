#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WPF

namespace Syncfusion.Windows.Shared
#endif

#if SILVERLIGHT
namespace Syncfusion.Windows.Tools.Controls
#endif
{
    internal class DateTimeProperties
    {
        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        /// <value>The start position.</value>
        public int StartPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lenghth.
        /// </summary>
        /// <value>The lenghth.</value>
        public int Lenghth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the is read only.
        /// </summary>
        /// <value>The is read only.</value>
        public bool? IsReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public DateTimeType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        public string Pattern
        {
            get;
            set;
        }

        private int _KeyPressCount = 0;

        /// <summary>
        /// Gets or sets the key press count.
        /// </summary>
        /// <value>The key press count.</value>
        public int KeyPressCount
        {
            get { return _KeyPressCount; }
            set { _KeyPressCount = value; }
        }

        /// <summary>
        /// Gets or sets the name of the month.
        /// </summary>
        /// <value>The name of the month.</value>
        public string MonthName
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This enum classifies DropDownViews Type.
    /// </summary>
    public enum DropDownViews
    {
        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is Classic view.
        /// </summary>
        Classic,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is Combined View.
        /// </summary>
        Combined,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is Calendar View.
        /// </summary>
        Calendar
    }

    /// <summary>
    /// This enum classifies Default DateParts type.
    /// </summary>
    public enum DateParts
    {
        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is year.
        /// </summary>
        Year = 0,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is month.
        /// </summary>
        Month,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is day.
        /// </summary>
        Day,

        /// <summary>
        /// Specifies values representing that the object holding this
        /// value is None.
        /// </summary>
        None
    }

    /// <summary>
    ///
    /// </summary>
    public enum DateTimeType
    {
        /// <summary>
        /// f
        /// </summary>
        Fraction,

        /// <summary>
        /// s
        /// </summary>
        Second,

        /// <summary>
        /// m
        /// </summary>
        Minutes,

        /// <summary>
        /// h
        /// </summary>
        Hour12,

        /// <summary>
        /// H
        /// </summary>
        Hour24,

        //d
        /// <summary>
        ///
        /// </summary>
        Day,

        //ddd and dddd
        /// <summary>
        ///
        /// </summary>
        Dayname,//0 t0 30 || Mon || Monday

        /// <summary>
        /// MM
        /// </summary>
        Month,

        /// <summary>
        ///  MMM or More
        /// </summary>
        monthname,

        /// <summary>
        ///
        /// </summary>
        year,

        /// <summary>
        /// gg A.D or B.D
        /// </summary>
        period,

        /// <summary>
        /// tt designator
        /// </summary>
        designator,

        /// <summary>
        /// zz z zzzz
        /// </summary>
        TimeZone,

        /// <summary>
        ///
        /// </summary>
        others

        //timezoneoffset,// like +5.30 and -6.30
        //designator,//Am || PM
        //period,//period or era A.D and B.D
        //kind//utc || local
    }
}