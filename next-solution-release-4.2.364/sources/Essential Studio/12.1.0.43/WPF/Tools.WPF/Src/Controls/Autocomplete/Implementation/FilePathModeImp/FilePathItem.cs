// <copyright file="FilePathItem.cs" company="Syncfusion">
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
    /// This class represent items for auto-complete, where the
    /// source is FilePath.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class FilePathItem : Object, IAutocompleteItem
    {
        #region Private members

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains item's text.
        /// </summary>
        private readonly string m_Text;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathItem"/> class.
        /// </summary>
        /// <param name="text">The text of the FilePathItem.</param>
        internal FilePathItem(string text)
        {
            m_Text = text;
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