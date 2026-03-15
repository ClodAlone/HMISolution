#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.ExcelToPdfConverter
{
    public delegate void CurrentProgressChangedEventHandler(object sender, CurrentProgressChangedEventArgs args);
    public delegate void SheetBeforeDrawnEventHandler(object sender, SheetBeforeDrawnEventArgs args);
    public delegate void SheetAfterDrawnEventHandler(object sender, SheetAfterDrawnEventArgs args);

    public class CurrentProgressChangedEventArgs : EventArgs
    {
        private float m_eventsCurrentProgress;
        private int m_noOfSheets;
        private int m_activeSheetIndex;

        /// <summary>
        /// Gets or sets the current progress changed.
        /// </summary>
        /// <value>The current progress changed.</value>
        public float CurrentProgressChanged
        {
            get { return m_eventsCurrentProgress; }
            set { m_eventsCurrentProgress = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentProgressChangedEventArgs"/> class.
        /// </summary>
        /// <param name="noOfSheets">The no of sheets.</param>
        /// <param name="activeSheetIndex">Index of the active sheet.</param>
        /// <param name="source">The source.</param>
        public CurrentProgressChangedEventArgs(int noOfSheets, int activeSheetIndex, object source)
        {
            this.m_noOfSheets = noOfSheets;
            this.m_activeSheetIndex = activeSheetIndex;
            this.m_eventsCurrentProgress = (100 / noOfSheets) * (activeSheetIndex + 1);
        }
    }

    public class SheetBeforeDrawnEventArgs : EventArgs
    {
        private int m_currentSheet = -1;
        private bool m_skip = false;

        /// <summary>
        /// Gets or sets the current sheet.
        /// </summary>
        /// <value>The current sheet.</value>
        public int CurrentSheet
        {
            get
            {
                return m_currentSheet;
            }
            set
            {
                m_currentSheet = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SheetBeforeDrawnEventArgs"/> is skip.
        /// </summary>
        /// <value><c>true</c> if skip; otherwise, <c>false</c>.</value>
        public bool Skip
        {
            get
            {
                return m_skip;
            }
            set
            {
                m_skip = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SheetBeforeDrawnEventArgs"/> class.
        /// </summary>
        /// <param name="currentSheet">The current sheet.</param>
        /// <param name="source">The source.</param>
        public SheetBeforeDrawnEventArgs(int currentSheet, object source)
        {
            if (!m_skip)
                this.m_currentSheet = currentSheet+1;
        }
    }

    public class SheetAfterDrawnEventArgs : EventArgs
    {
        private int m_afterSheet = -1;
        /// <summary>
        /// Gets the after sheet.
        /// </summary>
        /// <value>The after sheet.</value>
        public int AfterSheet
        {
            get
            {
                return m_afterSheet;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SheetAfterDrawnEventArgs"/> class.
        /// </summary>
        /// <param name="afterSheet">The after sheet.</param>
        /// <param name="source">The source.</param>
        public SheetAfterDrawnEventArgs(int afterSheet, object source)
        {
            m_afterSheet = afterSheet+1;
        }
    }

}
