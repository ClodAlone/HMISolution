// <copyright file="ItemsPresenterExt.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class overrides metadata from <see cref="ItemsPresenter"/>.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ItemsPresenterExt : ItemsPresenter
    {
        #region Initailization

        /// <summary>
        /// Initializes static members of the <see cref="ItemsPresenterExt"/> class.
        /// </summary>
        static ItemsPresenterExt()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemsPresenterExt), new FrameworkPropertyMetadata(typeof(ItemsPresenterExt)));
        }

        #endregion Initailization
    }
}