// <copyright file="FontFamilyRecord.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Specifies the types used for grouping the font records in <see cref="Syncfusion.Windows.Tools.Controls.FontListBox"/>
    /// and <see cref="Syncfusion.Windows.Tools.Controls.FontListComboBox"/>
    /// </summary>

    public enum FontFamilyRecordType
    {
        /// <summary>
        /// Common font family records.
        /// </summary>
        Common,

        /// <summary>
        /// Recently used font family records.
        /// </summary>
        RecentlyUsed,

        /// <summary>
        /// Theme font family records.
        /// </summary>
        Theme
    }

    /// <summary>
    /// FontFamilyRecord observable collection.
    /// </summary>
    public class FontFamilyRecordCollection : ObservableCollection<FontFamilyRecord>
    {
    }

    /// <summary>
    /// Contains the additional information about FontFamily. Used for displaying fonts
    /// in <see cref="Syncfusion.Windows.Tools.Controls.FontListBox"/> and <see cref="Syncfusion.Windows.Tools.Controls.FontListComboBox"/>
    /// </summary>
    public class FontFamilyRecord
    {
        #region Private fields

        /// <summary>
        /// Represents the type of record.
        /// </summary>
        private FontFamilyRecordType m_type;

        /// <summary>
        /// Cache of <see cref="System.Windows.Media.FontFamily"/> property.
        /// </summary>
        private FontFamily m_family;

        /// <summary>
        /// Name of the font.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Purpose of the font.
        /// </summary>
        private string m_purpose;

        #endregion Private fields

        #region Properties

        /// <summary>
        /// Gets or sets <see cref=" Syncfusion.Windows.Tools.Controls.FontFamilyRecordType"/> of the record.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontFamilyRecordType"/>
        /// </value>
        /// <seealso cref="FontFamilyRecordType"/>
        public FontFamilyRecordType Type
        {
            get
            {
                return m_type;
            }

            set
            {
                m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="System.Windows.Media.FontFamily"/> of the record.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontFamily"/>
        /// </value>
        /// <seealso cref="FontFamily"/>
        public FontFamily Family
        {
            get
            {
                return m_family;
            }

            set
            {
                m_family = value;
            }
        }

        /// <summary>
        /// Gets the name of the record.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// </value>
        /// <seealso cref="string"/>
        public string Name
        {
            get
            {
                return m_name;
            }

            internal set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the purpose of the record.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// </value>
        /// <seealso cref="string"/>
        public string Purpose
        {
            get
            {
                return m_purpose;
            }

            set
            {
                m_purpose = value;
            }
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Serves as a hash function for a particular type, suitable for
        /// use in hashing algorithms and data structures like a hash
        /// table.
        /// </summary>
        /// <returns>
        /// A hash code for the current Object.
        /// </returns>
        public override int GetHashCode()
        {
            return m_family.GetHashCode();
        }

        /// <summary>
        /// Checks whether objects are equal.
        /// </summary>
        /// <param name="obj">Current object for comparison.</param>
        /// <returns>
        /// True if objects are equal, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is FontFamilyRecord))
            {
                return false;
            }

            FontFamilyRecord record = (FontFamilyRecord)obj;

            bool result = this == record;

            return result;
        }

        /// <summary>
        /// Operator which checks whether two records are equal.
        /// </summary>
        /// <param name="rec1">First record to compare.</param>
        /// <param name="rec2">Second record to compare.</param>
        /// <returns>
        /// True if two records are equal, otherwise false.
        /// </returns>
        static public bool operator ==(FontFamilyRecord rec1, FontFamilyRecord rec2)
        {
            // if null and null
            if (!(rec1 is FontFamilyRecord) && !(rec2 is FontFamilyRecord))
            {
                return true;
            }

            if (!(rec1 is FontFamilyRecord) || !(rec2 is FontFamilyRecord))
            {
                return false;
            }

            return (rec1.m_name == rec2.m_name) && (rec1.m_type == rec2.m_type) && (rec1.m_purpose == rec2.m_purpose);
        }

        /// <summary>
        /// Operator which checks whether two records are different.
        /// </summary>
        /// <param name="rec1">First record to compare.</param>
        /// <param name="rec2">Second record to compare.</param>
        /// <returns>
        /// True if two records are not equal, otherwise false.
        /// </returns>
        static public bool operator !=(FontFamilyRecord rec1, FontFamilyRecord rec2)
        {
            // if null and null
            if (!(rec1 is FontFamilyRecord) && !(rec2 is FontFamilyRecord))
            {
                return false;
            }

            if (!(rec1 is FontFamilyRecord) || !(rec2 is FontFamilyRecord))
            {
                return true;
            }

            return (rec1.m_name != rec2.m_name) || (rec1.m_type != rec2.m_type) || (rec1.m_purpose != rec2.m_purpose);
        }

        #endregion Implementation
    }
}