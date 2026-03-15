// <copyright file="RegistryItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class represents items for auto-complete, where the
    /// source is Registry.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class RegistryItem : Object, IAutocompleteItem
    {
        #region Private member

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains item's text.
        /// </summary>
        private readonly string m_Text = String.Empty;

        #endregion Private member

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryItem"/> class.
        /// </summary>
        /// <param name="text">The Item's text.</param>
        internal RegistryItem(string text)
        {
            m_Text = text;
        }

        #endregion Initialization

        #region Properties

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the value of the m_Text member.
        /// </summary>
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