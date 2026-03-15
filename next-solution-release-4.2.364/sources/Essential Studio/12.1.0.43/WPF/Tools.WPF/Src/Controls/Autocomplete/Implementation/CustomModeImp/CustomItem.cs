// <copyright file="CustomItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>
using System;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class represents items for auto-complete, where the source is Custom.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class CustomItem : Object, IAutocompleteItem
    {
        #region Private members

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains item's text.
        /// </summary>
        private readonly string m_Text = String.Empty;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItem"/> class.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        internal CustomItem(string inputString)
        {
            m_Text = inputString;
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets item text. This property contains item's text.
        /// </summary>
        /// <value></value>
        /// <property name="flag" value="Finished"/>
        public string Text
        {
            get
            {
                return m_Text;
            }
        }

        #endregion Properties
    }
}