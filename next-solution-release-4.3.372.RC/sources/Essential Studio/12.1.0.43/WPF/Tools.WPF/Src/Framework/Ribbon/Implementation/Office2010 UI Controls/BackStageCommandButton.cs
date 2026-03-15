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
using System.Windows.Automation.Peers;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represent BackStageCommandButon class.
    /// </summary>
  public  class BackStageCommandButton:RibbonButton
    {
        /// <summary>
        /// Initializes the <see cref="BackStageCommandButton"/> class.
        /// </summary>
        static BackStageCommandButton()
        {
           
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(BackStageCommandButton), new FrameworkPropertyMetadata(typeof(BackStageCommandButton)));
          
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackStageCommandButton"/> class.
        /// </summary>
        public BackStageCommandButton()
        {
            this.MouseEnter += new System.Windows.Input.MouseEventHandler(BackStageCommandButton_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(BackStageCommandButton_MouseLeave);

            #if !SyncfusionFramework3_5
            this.TouchEnter += new EventHandler<System.Windows.Input.TouchEventArgs>(BackStageCommandButton_TouchEnter);
            this.TouchLeave += new EventHandler<System.Windows.Input.TouchEventArgs>(BackStageCommandButton_TouchLeave);
            #endif
        }

      #if !SyncfusionFramework3_5
        void BackStageCommandButton_TouchLeave(object sender, System.Windows.Input.TouchEventArgs e)
        {
            this.IsMouseOver = false;
        }

        void BackStageCommandButton_TouchEnter(object sender, System.Windows.Input.TouchEventArgs e)
        {
            this.IsMouseOver = true;
        }
    #endif

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
        public string  Header
        {
            get { return (string )GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(BackStageCommandButton), new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(BackStageCommandButton), new FrameworkPropertyMetadata(null));
      

        /// <summary>
        /// Gets a value indicating whether the mouse pointer is located over this element (including child elements in the visual tree).
        /// </summary>
        /// <value></value>
        /// <returns>true if mouse pointer is over the element or its child elements; otherwise, false. The default is false.</returns>
        //SU I78477
      //public bool IsMouseOver
        public new bool IsMouseOver
            //EU I78477
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        //SU I78477
      /*public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(BackStageCommandButton), new UIPropertyMetadata(false));*/
        public new static readonly DependencyProperty IsMouseOverProperty =
              DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(BackStageCommandButton), new UIPropertyMetadata(false));
      //EU I78477


        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            (e.Source as BackStageCommandButton).IsMouseOver = true;
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Source is BackStageCommandButton)
            {
                BackStageCommandButton commandButton = e.Source as BackStageCommandButton;
                commandButton.IsSelected = true;
            }
           
        }

        /// <summary>
        /// Called when an element loses keyboard focus.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.IInputElement.LostKeyboardFocus"/> event.</param>
        protected override void OnLostKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            this.IsMouseOver = false;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            GetSynchronizedCommand();
        }

        private void GetSynchronizedCommand()
        {
            string itemName = RibbonCommandManager.GetSynchronizedItem(this);
            Dictionary<string, FrameworkElement> syncItemColl = RibbonCommandManager.SynchronizedItemCollection;

            if (itemName != null && syncItemColl.ContainsKey(itemName))
            {
                FrameworkElement item = syncItemColl[itemName];
                if (this.Command == null)
                {
                    if (item is ButtonBase && (item as ButtonBase).Command != null)
                        this.Command = (item as ButtonBase).Command;
                    else if (item is SplitButton && (item as SplitButton).Command == null)
                        this.Command = (item as SplitButton).Command;
                    else if (item is SplitMenuButton && (item as SplitMenuButton).Command == null)
                        this.Command = (item as SplitMenuButton).Command;
                }
            }

            if (Command != null)
            {
                if (RibbonCommandManager.CommandDictionary.ContainsKey(Command))
                {
                    if (RibbonCommandManager.CommandDictionary[Command].SmallIcon != null)
                    {
                        Icon = RibbonCommandManager.CommandDictionary[Command].SmallIcon;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].Label != null)
                    {
                        Header = RibbonCommandManager.CommandDictionary[Command].Label;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].ToolTip != null)
                    {
                        ToolTip = RibbonCommandManager.CommandDictionary[Command].ToolTip;
                    }
                }
            }
        }

        #region Overrides

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BackStageCommandAutomationPeer(this);
        }

        #endregion
    }
    
}
