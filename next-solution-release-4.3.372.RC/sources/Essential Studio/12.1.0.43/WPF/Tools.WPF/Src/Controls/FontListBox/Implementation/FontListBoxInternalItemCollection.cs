// <copyright file="FontListBoxInternalItemCollection.cs" company="Syncfusion">
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
    /// Used for storing the <see cref=" Syncfusion.Windows.Tools.Controls.FontFamilyRecord"/> objects
    /// in <see cref="Syncfusion.Windows.Tools.Controls.FontListBox"/> and <see cref="Syncfusion.Windows.Tools.Controls.FontListComboBox"/>.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FontListBoxInternalItemCollection : ObservableCollection<FontListBoxInternalItem>
    {
    }
}