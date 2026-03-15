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
#if !WINDOWS_PHONE
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Defines memebers and methods to handle DateTime type range in <see cref="ChartAxis"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public struct DateTimeRange
    {
        #region Members
        /// <summary>
        /// Initilaizes m_start
        /// </summary>
        private DateTime m_start;

        /// <summary>
        /// Initilaizes m_end
        /// </summary>
        private DateTime m_end;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRange"/> struct.
        /// </summary>
        /// <param name="rangeStart">The range start.</param>
        /// <param name="rangeEnd">The range end.</param>
        public DateTimeRange(DateTime rangeStart, DateTime rangeEnd)
        {
            m_start = rangeStart;
            m_end = rangeEnd;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        [ClassReference(IsReviewed = false)]
        public bool IsEmpty
        {
            get
            {
                return m_end <= m_start;
            }
        }

        /// <summary>
        /// Gets the start.
        /// </summary>
        /// <value>The start.</value>
        [ClassReference(IsReviewed = false)]
        public DateTime Start
        {
            get
            {
                return m_start;
            }
        }

        /// <summary>
        /// Gets the end.
        /// </summary>
        /// <value>The end value.</value>
        [ClassReference(IsReviewed = false)]
        public DateTime End
        {
            get
            {
                return m_end;
            }
        }
        #endregion
    }
}
