// <copyright file="ThemeFontFamily.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a family of related theme fonts.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ThemeFontFamily : FontFamily
    {
        #region Private fields

        /// <summary>
        /// Stores the data for m_purpose variable
        /// </summary>
        private string m_purpose;

        #endregion Private fields

        #region	Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeFontFamily"/> class.
        /// </summary>
        /// <param name="fontFamily"> <see cref="System.Windows.Media.FontFamily"/> of the ThemeFontFamily.</param>
        /// <param name="purpose"> Purpose of the ThemeFontFamily.</param>
        public ThemeFontFamily(string fontFamily, string purpose)
            : base(fontFamily)
        {
            m_purpose = purpose;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the purpose of the ThemeFontFamily.
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

        #endregion

        #region Overrides

        /// <summary>
        /// Checks whether the current theme font family object and the
        /// specified font family object are the same.
        /// </summary>
        /// <param name="o">The <see cref="Syncfusion.Windows.Tools.Controls.ThemeFontFamily"/> object to compare.</param>
        /// <returns>
        /// True if o is equal to the current <see cref="Syncfusion.Windows.Tools.Controls.ThemeFontFamily"/> object;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object o)
        {
            if (!(o is ThemeFontFamily))
            {
                return false;
            }

            ThemeFontFamily second = (ThemeFontFamily)o;

            return this.Source == second.Source && this.Purpose == second.Purpose;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}