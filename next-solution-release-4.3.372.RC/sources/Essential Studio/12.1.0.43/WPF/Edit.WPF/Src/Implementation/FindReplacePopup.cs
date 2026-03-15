#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class FindReplacePopup : NonStickingPopup
    {
        /// <summary>
        ///
        /// </summary>
        static FindReplacePopup()
        {
            HorizontalOffsetProperty.OverrideMetadata(typeof(FindReplacePopup), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(onHorizontalOffsetChanged)));
            VerticalOffsetProperty.OverrideMetadata(typeof(FindReplacePopup), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(onHorizontalOffsetChanged)));
            PlacementRectangleProperty.OverrideMetadata(typeof(FindReplacePopup), new FrameworkPropertyMetadata(new Rect(0, 0, 10, 10), new PropertyChangedCallback(onHorizontalOffsetChanged)));
        }

        /// <summary>
        ///
        /// </summary>
        public FindReplacePopup()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void onHorizontalOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class IntellisensePopup : NonStickingPopup
    {
        /// <summary>
        ///
        /// </summary>
        public IntellisensePopup()
        {
        }
    }
}