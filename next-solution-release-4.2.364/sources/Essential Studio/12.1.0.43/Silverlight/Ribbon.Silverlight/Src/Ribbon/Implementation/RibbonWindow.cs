#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents RibbonWindpow class.
    /// </summary>
    public class RibbonWindow : WindowControl,IDisposable
    {

        internal Ribbon ribbon = null;

        /// <summary>
        /// 
        /// </summary>
        public RibbonWindow()
        {
            DefaultStyleKey = typeof(RibbonWindow);
            this.Title = "";
            this.Icon = new BitmapImage();
        }


        /// <summary>
        /// Gets or sets a value indicating whether [save original state].
        /// </summary>
        /// <value><c>true</c> if [save original state]; otherwise, <c>false</c>.</value>
        public bool AutoPersist
        {
            get { return (bool)GetValue(AutoPersistProperty); }
            set { SetValue(AutoPersistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveOriginalState.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Indicates whether to save state persisted on loading.
        /// </summary>
        public static readonly DependencyProperty AutoPersistProperty =
            DependencyProperty.Register("AutoPersist", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public Brush BackStageColor
        {
            get { return (Brush)GetValue(BackStageColorProperty); }
            set { SetValue(BackStageColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageColor.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BackStageColorProperty =
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(RibbonWindow), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            if (ribbon != null)
                ribbon.Dispose();
        }
    }
}
