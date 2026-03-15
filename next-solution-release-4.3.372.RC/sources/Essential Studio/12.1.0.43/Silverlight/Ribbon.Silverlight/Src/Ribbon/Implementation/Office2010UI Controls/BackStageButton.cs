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
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent BackStageButton Class.
    /// </summary>
    public class BackStageButton : ButtonBase,IBackStageColor
    {

       internal static ResourceWrapper Wrapper = new ResourceWrapper();

        /// <summary>
        /// Initializes a new instance of the <see cref="BackStageButton"/> class.
        /// </summary>
        public BackStageButton()
        {
            this.DefaultStyleKey = typeof(BackStageButton);
        }

        /// <summary>
        /// Initializes the <see cref="BackStageButton"/> class.
        /// </summary>
        static BackStageButton()
        {

        }

        /// <summary>
        /// Gets or sets the parent ribbon.
        /// </summary>
        /// <value>The parent ribbon.</value>
        internal Ribbon parentRibbon
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
            DependencyProperty.Register("Header", typeof(object), typeof(BackStageButton), new PropertyMetadata(Wrapper.BackStageButtonHeader));


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
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(BackStageButton), new PropertyMetadata(false, new PropertyChangedCallback(OnIsOpenChanged)));



        /// <summary>
        /// Gets a value indicating whether the mouse pointer is located over this button control.
        /// </summary>
        /// <value></value>
        /// <returns>true to indicate the mouse pointer is over the button control, otherwise false. The default is false.</returns>
        public new bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        /// </summary>
        public new static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(BackStageButton), new PropertyMetadata(false));


        /// <summary>
        /// Called when [is open changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackStageButton source = (BackStageButton)d;
            if (source.parentRibbon != null)
            {
                if (source.IsOpen)
                {
                    if (source.parentRibbon.BackStage != null)
                    {
                        source.parentRibbon.ShowBackStage();
                        source.parentRibbon.BackStage.Focus();
                    }
                }
                else
                    source.parentRibbon.HideBackStage();
            }
        }


        /// <summary>
        /// It is used to set the BackStageColor from the Ribbon.
        /// </summary>
        public Brush BackStageColor
        {
            get { return (Brush)GetValue(BackStageColorProperty); }
            set { SetValue(BackStageColorProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for BackStageColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackStageColorProperty =
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(BackStageButton),new PropertyMetadata(new SolidColorBrush(Colors.Blue)));



        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseEnter"/> event that occurs when the mouse enters this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="e"/> is null.</exception>
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsMouseOver = true;
            UpdateVisualStates();
        }


        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeave"/> routed event that occurs when the mouse leaves an element.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.MouseLeave"/> event.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="e"/> is null.</exception>
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseOver = false;
            UpdateVisualStates();
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

        /// <summary>
        /// Handles the click.
        /// </summary>
        private void HandleClick()
        {
            this.IsOpen = !this.IsOpen;
            UpdateVisualStates();
        }

        /// <summary>
        /// Updates the visual states.
        /// </summary>
        private void UpdateVisualStates()
        {
            if (this.IsMouseOver)
                VisualStateManager.GoToState(this, "MouseOver", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);

            if (!this.IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
                    
            if (this.IsOpen)
                VisualStateManager.GoToState(this, "IsOpen", true);

            if (this.IsMouseOver && this.IsOpen)
                VisualStateManager.GoToState(this, "IsOpenMouseOver", true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void OnBackStageColorChanged(Brush color)
        {
            BackStageColor = color;
        }
    }
}
