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

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Represents a class that will used to provide infromation about a Schedule row.
    /// </summary>
    public class GanttScheduleRowInfo : ICloneable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttScheduleRowInfo"/> class.
        /// </summary>
        public GanttScheduleRowInfo()
        {
            PixelsPerUnit = 1;
            CellsPerUnit = 1;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            VerticalAlignment = System.Windows.VerticalAlignment.Center;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the timeunit.
        /// </summary>
        /// <value>
        /// The timeunit.
        /// </value>
        public TimeUnit TimeUnit { get; set; }

        /// <summary>
        /// Sets the pixels per unit.
        /// </summary>
        /// <value>
        /// The pixels per unit.
        /// </value>
        public Double PixelsPerUnit { get; set;}

        /// <summary>
        /// Sets the cells per unit.
        /// </summary>
        /// <value>
        /// The cells per unit.
        /// </value>
        public double CellsPerUnit { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>
        /// The horizontal alignment.
        /// </value>
        public HorizontalAlignment HorizontalAlignment { get; set;}

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>
        /// The vertical alignment.
        /// </value>
        public VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the cell text format.
        /// </summary>
        /// <value>The cell text format.</value>
        public string CellTextFormat { get; set; }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new GanttScheduleRowInfo
            {
                CellsPerUnit = this.CellsPerUnit,
                PixelsPerUnit = this.PixelsPerUnit,
                CellTextFormat = this.CellTextFormat,
                TimeUnit = this.TimeUnit,
                HorizontalAlignment = this.HorizontalAlignment,
                VerticalAlignment = this.VerticalAlignment,
            };
        }

        #endregion
    }
}
