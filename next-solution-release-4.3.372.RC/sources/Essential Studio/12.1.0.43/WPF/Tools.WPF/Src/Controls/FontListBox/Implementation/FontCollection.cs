// <copyright file="FontCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Collection of <see cref="System.Windows.Media.FontFamily"/> objects.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FontCollection : ObservableCollection<FontFamily>
    {
        #region Implementation

        /// <summary>
        /// Indicates whether the collection contains a specific <see cref="System.Windows.Media.FontFamily"/> entry.
        /// </summary>
        /// <param name="fontFamily">The <see cref="System.Windows.Media.FontFamily"/> entry to locate in the collection.</param>
        /// <returns>True if contains, otherwise false.</returns>
        public bool ContainsName(FontFamily fontFamily)
        {
            if (fontFamily == null)
            {
                return false;
            }

            XmlLanguage userLanguage = XmlLanguage.GetLanguage("en-US");

            bool result = false;

            foreach (FontFamily font in this)
            {
                try
                {
                    if (fontFamily.FamilyNames[userLanguage] == font.FamilyNames[userLanguage])
                    {
                        result = true;
                        break;
                    }
                }
                //SU I78477
                //catch (Exception e) { }
                catch (Exception) { }
                //EU I78477
            }

            return result;
        }

        /// <summary>
        /// Inserts an <see cref="System.Windows.Media.FontFamily"/> element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which element should be inserted.</param>
        /// <param name="item">The <see cref="System.Windows.Media.FontFamily"/> element to insert.</param>
        protected override void InsertItem(int index, FontFamily item)
        {
            if (!ContainsName(item) && item != null)
            {
                base.InsertItem(index, item);
            }
        }

        #endregion Implementation
    }
}