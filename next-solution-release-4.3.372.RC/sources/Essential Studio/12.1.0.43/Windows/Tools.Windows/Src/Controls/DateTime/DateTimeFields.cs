#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.Tools
{
    public sealed class FieldDefinition
    {
        private FieldType type;
        private int format;
        private object data;

        #region construction
        public FieldDefinition(FieldType type, DayFormat format)
            : this(type, (int)format, null)
        { 
        }

        public FieldDefinition(FieldType type, HourFormat format)
            : this(type, (int)format, null)
        { 
        }

        public FieldDefinition(FieldType type, MinuteFormat format)
            : this(type, (int)format, null)
        { 
        }

        public FieldDefinition(FieldType type, MonthFormat format)
            : this(type, (int)format, null)
        { 
        }

        public FieldDefinition(FieldType type, SecondsFormat format)
            : this(type, (int)format, null)
        { 
        }

        public FieldDefinition(FieldType type, AMPMFormat format)
            : this(type, (int)format, null)
        { 
        }

        public FieldDefinition(FieldType type, YearFormat format)
            : this(type, (int)format, null)
        { 
        }

        public FieldDefinition(FieldType type, DateTimeStringFormat format, string data)
            : this(type, (int)format, data)
        { 
        }

        private FieldDefinition(FieldType type, int format, object data)
        {
            this.type = type;
            this.format = format;
            this.data = data;
        }
        #endregion

        #region properties
        public FieldType Type
        {
            get
            {
                return this.type;
            }
        }

        public DayFormat DayFormat
        {
            get
            {
                return (DayFormat)this.format;
            }
        }

        public HourFormat HourFormat
        {
            get
            {
                return (HourFormat)this.format;
            }
        }

        /*Added*/
        public MonthFormat MonthFormat
        {
            get
            {
                return (MonthFormat)this.format;
            }
        }

        public MinuteFormat MinuteFormat
        {
            get
            {
                return (MinuteFormat)this.format;
            }
        }

        public SecondsFormat SecondsFormat
        {
            get
            {
                return (SecondsFormat)this.format;
            }
        }

        public AMPMFormat AMPMFormat
        {
            get
            {
                return (AMPMFormat)this.format;
            }
        }

        public YearFormat YearFormat
        {
            get
            {
                return (YearFormat)this.format;
            }
        }

        public DateTimeStringFormat StringFormat
        {
            get
            {
                return (DateTimeStringFormat)this.format;
            }
        }

        public string StringData
        {
            get
            {
                return (string)this.data;
            }
        }
        #endregion

        public override string ToString()
        {
            string fieldTypeDesc = Enum.GetName(typeof(FieldType), this.type);
            string formatDesc = null;
            string value = "NA";

            System.Type formatEnumType = null;
            switch (type)
            {
                case FieldType.Day:
                    formatEnumType = typeof(DayFormat);
                    break;
                case FieldType.Hour:
                    formatEnumType = typeof(HourFormat);
                    break;
                case FieldType.Minute:
                    formatEnumType = typeof(MinuteFormat);
                    break;
                case FieldType.Month:
                    formatEnumType = typeof(MonthFormat);
                    break;
                case FieldType.Seconds:
                    formatEnumType = typeof(SecondsFormat);
                    break;
                case FieldType.AMPM:
                    formatEnumType = typeof(AMPMFormat);
                    break;
                case FieldType.Year:
                    formatEnumType = typeof(YearFormat);
                    break;
                case FieldType.String:
                    formatEnumType = typeof(DateTimeStringFormat);
                    value = this.StringData;
                    break;
            }

            formatDesc = System.Enum.GetName(formatEnumType, this.format);
            string retval = string.Format("FieldType: {0}, Format: {1}, Value: '{2}'", fieldTypeDesc, formatDesc, value);
            return retval;
        }
    }

    public enum FieldType
    {
        /// <summary>
        /// Represents Field Type Day
        /// </summary>
        Day,

        /// <summary>
        ///  Represents Field Type hour
        /// </summary>
        Hour,

        /// <summary>
        ///  Represents Field Type minute
        /// </summary>
        Minute,

        /// <summary>
        ///  Represents Field Type month
        /// </summary>
        Month,

        /// <summary>
        ///  Represents Field Type seconds
        /// </summary>
        Seconds,

        /// <summary>
        ///  Represents Field Type AMPM
        /// </summary>
        AMPM,

        /// <summary>
        ///  Represents Field Type Year
        /// </summary>
        Year,

        /// <summary>
        ///  Represents Field Type string
        /// </summary>
        String,
    }

    public enum DayFormat
    {
        /// <summary>
        /// d The one or two-digit day. 
        /// </summary>
        OneOrTwoDigit = 1,

        /// <summary>
        /// dd The two-digit day. Single digit day values are preceded by a zero. 
        /// </summary>
        TwoDigit = 2, 

        /// <summary>
        /// ddd The three-character day-of-week abbreviation. 
        /// </summary>
        AbbreviatedName = 3,

        /// <summary>
        /// The full day-of-week name. 
        /// </summary>
        FullName = 4,
    }

    public enum HourFormat
    {
        /// <summary>
        /// h The one or two-digit hour in 12-hour format. 
        /// </summary>
        OneOrTwoDigit12 = 1,

        /// <summary>
        /// hh The two-digit hour in 12-hour format. Single digit values are preceded by a zero. 
        /// </summary>
        TwoDigit12 = 2,

        /// <summary>
        /// H The one or two-digit hour in 24-hour format. 
        /// </summary>
        OneOrTwoDigit24 = 11,

        /// <summary>
        /// HH The two-digit hour in 24-hour format. Single digit values are preceded by a zero. 
        /// </summary>
        TwoDigit24 = 12,
    }

    public enum MinuteFormat
    {
        /// <summary>
        /// m The one or two-digit minute.
        /// </summary>
        OneOrTwoDigit = 1,

        /// <summary>
        /// mm The two-digit minute. Single digit values are preceded by a zero.
        /// </summary>
        TwoDigit = 2,
    }

    public enum MonthFormat
    {
        /// <summary>
        /// M The one or two-digit month number.
        /// </summary>
        OneOrTwoDigit = 1,

        /// <summary>
        /// MM The two-digit month number. Single digit values are preceded by a zero.
        /// </summary>
        TwoDigit = 2,

        /// <summary>
        /// MMM The three-character month abbreviation. 
        /// </summary>
        AbbreviatedName = 3,

        /// <summary>
        /// MMMM The full month name. 
        /// </summary>
        FullName = 4,
    }

    public enum SecondsFormat
    {
        /// <summary>
        /// s The one or two-digit seconds. 
        /// </summary>
        OneOrTwoDigit = 1,

        /// <summary>
        /// ss The two-digit seconds. Single digit values are preceded by a zero. 
        /// </summary>
        TwoDigit = 2,
    }

    public enum AMPMFormat
    {
        /// <summary>
        /// Represents AMPMFormat.t The one-letter AM/PM abbreviation ("AM" is displayed as "A").
        /// </summary>
        OneLetter = 1,

        /// <summary>
        /// tt The two-letter AM/PM abbreviation ("AM" is displayed as "AM"). 
        /// </summary>
        TwoLetter = 2,
    }

    public enum YearFormat
    {
        /// <summary>
        /// Represent the year format y The one-digit year (2001 is displayed as "1"). 
        /// </summary>
        OneDigit = 1,

        /// <summary>
        ///  Represent the year format yy The last two digits of the year (2001 is displayed as "01"). 
        /// </summary>
        TwoDigit = 2,

        /// <summary>
        /// Represent the year format yyyy 
        /// </summary>
        Full = 4,
    }
    public enum DateTimeStringFormat
    {
        /// <summary>
        /// Represents DatetimeString format default style
        /// </summary>
        Default
    }
}
