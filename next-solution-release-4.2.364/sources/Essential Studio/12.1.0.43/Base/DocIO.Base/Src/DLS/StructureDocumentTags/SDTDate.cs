
#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives

#endregion

namespace Syncfusion.DocIO.DLS
{
    internal class SDTDate
    {
        # region Fields
        private string m_fullDate;
        private CalendarType m_calendarType;
        private string m_dateFormat;
        private string m_LID;
        # endregion

        #region Properties

        internal string DateFormat
        {
            get
            {
                return m_dateFormat;
            }
            set
            {
                m_dateFormat = value;
            }
        }
        internal  CalendarType CalendarType
        {
            get
            {
                return m_calendarType;
            }
            set
            {
                m_calendarType = value;
            }
        }      
        internal string LID
        {
            get
            {
                return m_LID ;
            }
            set
            {
                m_LID =value ;
            }
        }
        internal string FullDate
        {
            get
            {
                return m_fullDate;
            }
            set
            {
                m_fullDate = value;
            }
        }
        # endregion
        #region Constructor
        internal SDTDate()
        {
            //Empty Constructor
        }
        #endregion
    }
}
