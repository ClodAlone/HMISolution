// <copyright file="TreeObjectCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System.Collections.ObjectModel;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is observable collection of elements type of
    /// object
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeObjectCollection : ObservableCollection<object>
    {
    }
}