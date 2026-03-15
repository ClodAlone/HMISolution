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
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a range of the DateTime
    /// </summary>
    public class DateRange
    {
        private DateTime startDate;

        /// <summary>
        /// Represents a start date of date range
        /// </summary>
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        private DateTime endDate;

        /// <summary>
        /// Represents a start date of date range
        /// </summary>
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }
        
        /// <summary>
        /// Initializes a new date range
        /// </summary>
        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate < endDate)
            {
                this.StartDate = startDate;
                this.EndDate = endDate;
            }
            else
            {
                this.StartDate = endDate;
                this.EndDate = startDate;
            }
        }

        /// <summary>
        /// Initializes a new date range
        /// </summary>
        public DateRange(DateTime startDate)
        {
            this.StartDate = startDate;
            this.EndDate = startDate;
        }
    }
}
