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

namespace Syncfusion.Windows.Tools.Controls
{
    public class DateTimeProperties
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
    }

    
    public enum DateTimeType
    {
        //f
        Fraction,
        //s
        Second,
        //m
        Minutes,
        //h
        Hour12,
        //H
        Hour24,
        //d
        Day,
        //ddd and dddd
        Dayname,//0 t0 30 || Mon || Monday
        //MM
        Month,
        //MMM or More
        monthname,
        year,
        //gg A.D or B.D
        period,
        //tt designator
        designator,
        //zz z zzzz
        TimeZone,
        others

        //timezoneoffset,// like +5.30 and -6.30
        //designator,//Am || PM
        //period,//period or era A.D and B.D
        //kind//utc || local


    }
}
