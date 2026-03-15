#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;
using Syncfusion.Windows.Tools.Controls.Resources;
using System.Windows.Automation.Peers;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent BackStageButton Class.
    /// </summary>
    public class BackStageButton : ButtonBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackStageButton"/> class.
        /// </summary>
        public BackStageButton()
        {

        }
        static ResourceWrapper wrapper = new ResourceWrapper();
        /// <summary>
        /// Initializes the <see cref="BackStageButton"/> class.
        /// </summary>
        static BackStageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackStageButton), new FrameworkPropertyMetadata(typeof(BackStageButton)));
        }

        /// <summary>
        /// Gets or sets the parent ribbon.
        /// </summary>
        /// <value>The parent ribbon.</value>
        public Ribbon parentRibbon
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Gets or Sets the Content of BackStage Button. It is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(BackStageButton), new UIPropertyMetadata(string.Empty));


        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Descriping Whether BackStage opened or not.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(BackStageButton), new FrameworkPropertyMetadata(false,new PropertyChangedCallback (OnIsOpenChanged)));


        protected static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackStageButton  source = (BackStageButton )d;
            if (source.IsOpen)
                source.parentRibbon.ShowBackStageInternal();
            else
                source.parentRibbon.HideBackStageInternal();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.ButtonBase.Click"/> routed event.
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();
            HandleClick();
        }

        internal void CheckClick()
        {
            this.OnClick();
        }

        private void HandleClick()
        {
            this.IsOpen = !this.IsOpen;
        }

        #region Overrides

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BackStageButtonAutoamtionPeer(this);
        }

        #endregion
    }
}
