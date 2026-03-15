// <copyright file="ThemeFontFamilyCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a dynamic data collection of <see cref="Syncfusion.Windows.Tools.Controls.ThemeFontFamily"/> objects.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ThemeFontFamilyCollection : ObservableCollection<ThemeFontFamily>
    {
        /// <summary>
        /// Determines whether [contains] [the specified font family].
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified font family]; otherwise, <c>false</c>.
        /// </returns>
        new public bool Contains(ThemeFontFamily fontFamily)
        {
            if (fontFamily == null)
            {
                return false;
            }

            bool result = false;

            foreach (ThemeFontFamily font in this)
            {
                if (fontFamily.Purpose == font.Purpose)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object of <see cref="Syncfusion.Windows.Tools.Controls.ThemeFontFamily"/> type to insert.</param>
        protected override void InsertItem(int index, ThemeFontFamily item)
        {
            if (!Contains(item))
            {
                base.InsertItem(index, item);
            }
        }
    }
}