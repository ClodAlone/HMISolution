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
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent BackStageCommandButon class.
    /// </summary>
    public class BackStageCommandButton : ButtonBase,IBackStageColor
    {
        /// <summary>
        /// Initializes the <see cref="BackStageCommandButton"/> class.
        /// </summary>
        static BackStageCommandButton()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackStageCommandButton"/> class.
        /// </summary>
        public BackStageCommandButton()
        {
            this.DefaultStyleKey = typeof(BackStageCommandButton);
            this.MouseEnter += new System.Windows.Input.MouseEventHandler(BackStageCommandButton_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(BackStageCommandButton_MouseLeave);
        }

        /// <summary>
        /// Handles the MouseLeave event of the BackStageCommandButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void BackStageCommandButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.IsMouseOver = false;
        }

        /// <summary>
        /// Handles the MouseEnter event of the BackStageCommandButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void BackStageCommandButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.IsMouseOver = true;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets parent Back Stage Element.
        /// </summary>
        internal Backstage BackStageParent
        {
            get
            {
                return (ItemsControl.ItemsControlFromItemContainer(this) as Backstage);
            }
        }


        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(BackStageCommandButton), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(BackStageCommandButton), new PropertyMetadata(null));


        /// <summary>
        /// Gets a value indicating whether the mouse pointer is located over this element (including child elements in the visual tree).
        /// </summary>
        /// <value></value>
        /// <returns>true if mouse pointer is over the element or its child elements; otherwise, false. The default is false.</returns>
        public new bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        /// </summary>
        public new static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(BackStageCommandButton), new PropertyMetadata(false, new PropertyChangedCallback(OnIsMouseOverChanged)));


        /// <summary>
        /// 
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
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(BackStageCommandButton), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));



        /// <summary>
        /// Called when [is mouse over changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsMouseOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackStageCommandButton source = (BackStageCommandButton)d;
            if (source.IsMouseOver)
                source.HideAllMouseOverElements(source);
            source.UpdateVisualState(source);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState(this);
        }


        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
        /// <param name="button">The button.</param>
        private void UpdateVisualState(BackStageCommandButton button)
        {
            if (button.IsMouseOver && button.IsEnabled)
                VisualStateManager.GoToState(button, "IsMouseOverEnabled", true);
            else
                VisualStateManager.GoToState(button, "Normal", true);

            if (!button.IsEnabled)
                VisualStateManager.GoToState(button, "Disabled", true);
        }

        /// <summary>
        /// Hides all mouse over elements.
        /// </summary>
        /// <param name="currentButton">The current button.</param>
        private void HideAllMouseOverElements(BackStageCommandButton currentButton)
        {
            foreach (var item in this.BackStageParent.Items)
                if (item is BackStageCommandButton)
                {
                    BackStageCommandButton button = (BackStageCommandButton)item;
                    if (item != currentButton)
                        button.IsMouseOver = false;
                }
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
