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
using System.Windows;
using Syncfusion.Windows.Shared;
using System.Windows.Media;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent RibbonToggleButton class.
    /// </summary>
    public  class RibbonToggleButton:ButtonAdv,IRibbonControl
    {

        internal Ribbon parentRibbon;
        SystemGesture msystemGesture;

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonToggleButton"/> class.
        /// </summary>
        public RibbonToggleButton()
        {

        }

        /// <summary>
        /// Initializes the <see cref="RibbonToggleButton"/> class.
        /// </summary>
        static RibbonToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonToggleButton), new FrameworkPropertyMetadata(typeof(RibbonToggleButton)));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is normal state.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is normal state; otherwise, <c>false</c>.
        /// </value>
        public bool IsNormalState
        {
            get { return (bool)GetValue(IsNormalStateProperty); }
            set { SetValue(IsNormalStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsNormalState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNormalStateProperty =
            DependencyProperty.Register("IsNormalState", typeof(bool), typeof(RibbonToggleButton), new FrameworkPropertyMetadata (true));
        
        /// <summary>
        /// Handles the state of the ribbon.
        /// </summary>
        private void HandleRibbonState()
        {
            if (parentRibbon != null)
            {
                if (this.parentRibbon.RibbonState == RibbonState.Hide)
                    this.parentRibbon.RibbonState = RibbonState.Normal;

                else if (this.parentRibbon.RibbonState == RibbonState.Normal)
                    this.parentRibbon.RibbonState = RibbonState.Hide;

                else if (this.parentRibbon.RibbonState == RibbonState.Adorner)
                    this.parentRibbon.RibbonState = RibbonState.Normal;
                parentRibbon.isCheckedToggleButton = true;
                this.parentRibbon.HideKeyTips();
                parentRibbon.isCheckedToggleButton = false;
            }
            else
                this.IsChecked = !this.IsChecked;
        }
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnPreviewMouseLeftButtonDown(e);

                HandleRibbonState();
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnPreviewTouchDown(System.Windows.Input.TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch && msystemGesture == SystemGesture.Tap)
            {
                base.OnPreviewTouchDown(e);
                HandleRibbonState();
            }
        }

#endif

        protected override void OnStylusSystemGesture(System.Windows.Input.StylusSystemGestureEventArgs e)
        {
            msystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.ButtonBase.Click"/> event.
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();
            //this.IsNormalState = !this.IsNormalState;
            //HandleRibbonState();
        }

        protected override void OnMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
                FrameworkElement fe = VisualUtils.FindRootVisual(this) as FrameworkElement;
                while (ribbon == null && fe != null && fe.GetType() == VisualUtils.RootPopupType)
                {
                    Popup popup = fe.Parent as Popup;
                    fe = popup.TemplatedParent as FrameworkElement;
                    if (fe != null)
                    {
                        if (fe is Ribbon)
                            ribbon = fe as Ribbon;
                        else
                            ribbon = VisualUtils.FindAncestor(fe, typeof(Ribbon)) as Ribbon;
                        fe = VisualUtils.FindRootVisual(fe) as FrameworkElement;
                    }
                }


                RibbonToggleButton item = e.Source is RibbonToggleButton ? e.Source as RibbonToggleButton : this as RibbonToggleButton;
                QuickAccessToolBarPanel qatPanel = VisualUtils.FindAncestor(item as Visual, typeof(QuickAccessToolBarPanel)) as QuickAccessToolBarPanel;
                if (ribbon != null && this.ContextMenu == null && item != null)
                {
                    RibbonContextMenu.CreateContextMenu(item as FrameworkElement);
                    e.Handled = true;
                }
                base.OnMouseRightButtonUp(e);
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch && msystemGesture == SystemGesture.RightTap)
            {
                Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
                FrameworkElement fe = VisualUtils.FindRootVisual(this) as FrameworkElement;
                while (ribbon == null && fe != null && fe.GetType() == VisualUtils.RootPopupType)
                {
                    Popup popup = fe.Parent as Popup;
                    fe = popup.TemplatedParent as FrameworkElement;
                    if (fe != null)
                    {
                        if (fe is Ribbon)
                            ribbon = fe as Ribbon;
                        else
                            ribbon = VisualUtils.FindAncestor(fe, typeof(Ribbon)) as Ribbon;
                        fe = VisualUtils.FindRootVisual(fe) as FrameworkElement;
                    }
                }


                RibbonToggleButton item = e.Source is RibbonToggleButton ? e.Source as RibbonToggleButton : this as RibbonToggleButton;
                QuickAccessToolBarPanel qatPanel = VisualUtils.FindAncestor(item as Visual, typeof(QuickAccessToolBarPanel)) as QuickAccessToolBarPanel;
                if (ribbon != null && this.ContextMenu == null && item != null)
                {
                    RibbonContextMenu.CreateContextMenu(item as FrameworkElement);
                    e.Handled = true;
                }
                base.OnTouchUp(e);
            }
        }
#endif
    }
}
