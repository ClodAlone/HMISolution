// <copyright file="SplitMenuButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents SplitMenuButton control.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class SplitMenuButton : MenuButtonBase</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:SplitMenuButton Name="button" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// SplitMenuButton class represents a control that can display child UIElements in a dropdown popup and can be used in ApplicationMenu control.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a SplitMenuButton in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:SplitMenuButton Label="Print" Icon="SampleImages/Print32.png">
    /// <ribbon:RibbonButton SizeForm = "Small" Label="Item1" SmallIcon="SampleImages/Document32.png"/>
    /// <ribbon:RibbonButton SizeForm = "Small" Label="Item2" SmallIcon="SampleImages/Document32.png"/>
    /// </ribbon:SplitMenuButton>]]></code>
    /// <para/>This example shows how to create a SplitMenuButton in C#.
    /// <code>
    /// SplitMenuButton button = new SplitMenuButton();
    /// RibbonMenuItem item = new RibbonMenuItem();
    /// button.Items.Add(item);
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SplitMenuButton : MenuButtonBase, ICommandSource
    {
        #region Private members
        /// <summary>
        /// Represents the clickable area border
        /// </summary>
        private Border m_clickableArea;

        SystemGesture msystemGesture;

        #endregion

        #region Dependency properties
        /// <summary>
        /// Gets whether button is pressed.  This is a dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey IsPressedPropertyKey = DependencyProperty.RegisterReadOnly("IsPressed", typeof(bool), typeof(SplitMenuButton), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Defines whether button is pressed.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

     
        /// <summary>
        ///  Defines the iconsize that appears in SimpleMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(Size), typeof(SplitMenuButton), new FrameworkPropertyMetadata(new Size(20d, 20d), new PropertyChangedCallback(OnIconSizeChanged)));

        // Using a DependencyProperty as the backing store for SmallIconProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallIconProperty = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(SplitMenuButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSmallIconChanged)));
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether button is pressed.
        /// </summary>
        protected bool IsPressed
        {
            get
            {
                return (bool)GetValue(IsPressedProperty);
            }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether this instance can command execute.
        ///// </summary>
        //public bool CanCommandExecute
        //{
        //    get
        //    {
        //        return (bool)GetValue(CanCommandExecuteProperty);
        //    }

        //    set
        //    {
        //        SetValue(CanCommandExecuteProperty, value);
        //    }
        //}

        /// <summary>
        /// Gets or sets the size of the icon.
        /// </summary>
        /// <value>The size of the icon.</value>
        public Size IconSize
        {
            get
            {
                return (Size)GetValue(IconSizeProperty);
            }

            set
            {
                SetValue(IconSizeProperty, value);
            }
        }


        public  new ImageSource SmallIcon
        {
            get
            {
                return (ImageSource)GetValue(SmallIconProperty);
            }
            set
            {
                SetValue(SmallIconProperty, value);
            }
        }


        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when Iconsize property is changed.
        /// </summary>
        public event PropertyChangedCallback IconSizeChanged;

        public event PropertyChangedCallback SmallIconChanged;


        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="SplitMenuButton"/> class.
        /// </summary>
        static SplitMenuButton()
        {
            EnvironmentTest.ValidateLicense(typeof(SplitMenuButton));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitMenuButton), new FrameworkPropertyMetadata(typeof(SplitMenuButton)));
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Sets the is pressed.
        /// </summary>
        /// <param name="pressed">if set to <c>true</c> [pressed].</param>
        private void SetIsPressed(bool pressed)
        {
            if (pressed)
            {
                base.SetValue(IsPressedPropertyKey, true);
            }
            else
            {
                base.ClearValue(IsPressedPropertyKey);
            }
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        private void OnClick()
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }

        /// <summary>
        /// Called when [icon size changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitMenuButton instance = (SplitMenuButton)d;
            instance.OnIconSizeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IconSizeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIconSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IconSizeChanged != null)
            {
                IconSizeChanged(this, e);
            }
        }
        
        /// <summary>
        /// Calls OnLabelChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSmallIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitMenuButton instance = (SplitMenuButton)d;
            instance.OnSmallIconChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises LabelChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnSmallIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SmallIconChanged != null)
            {
                SmallIconChanged(this, e);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_clickableArea = (Border)GetTemplateChild("PART_OutterBorder");
            if (m_clickableArea == null)
            {
                throw new NullReferenceException("Template key part could not be found");
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/>�routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || ( ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                e.Handled = true;
                base.Focus();
                if (m_clickableArea.IsMouseOver)
                {
                    if (e.ButtonState == MouseButtonState.Pressed)
                    {
                        m_clickableArea.CaptureMouse();
                        if (m_clickableArea.IsMouseCaptured)
                        {
                            if (e.ButtonState == MouseButtonState.Pressed)
                            {
                                if (!this.IsPressed)
                                {
                                    this.SetIsPressed(true);
                                }
                            }
                            else
                            {
                                m_clickableArea.ReleaseMouseCapture();
                            }
                        }
                    }
                }

                base.OnMouseLeftButtonDown(e);
            }
        }

        #if !SyncfusionFramework3_5

        protected override void OnTouchDown(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch!=null && ribbonTouch.EnableTouch && msystemGesture == SystemGesture.Tap)
            {
                e.Handled = true;
                base.Focus();
                if (m_clickableArea.IsMouseOver)
                {
                    //if (e.ButtonState == MouseButtonState.Pressed)
                    {
                        m_clickableArea.CaptureMouse();
                        if (m_clickableArea.IsMouseCaptured)
                        {
                           // if (e.ButtonState == MouseButtonState.Pressed)
                            {
                                if (!this.IsPressed)
                                {
                                    this.SetIsPressed(true);
                                }
                            }
                            if (m_clickableArea.IsMouseCaptured)
                            {
                                m_clickableArea.ReleaseMouseCapture();
                            }
                        }
                    }
                }
                base.OnTouchDown(e);
            }
        }

#endif
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (m_clickableArea.IsMouseCaptured && (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed))
            {
                Point position = Mouse.PrimaryDevice.GetPosition(this);
                if (((position.X >= 0) && (position.X <= m_clickableArea.ActualWidth)) && ((position.Y >= 0) && (position.Y <= m_clickableArea.ActualHeight)))
                {
                    if (!this.IsPressed)
                    {
                        this.SetIsPressed(true);
                    }
                }
                else if (this.IsPressed)
                {
                    this.SetIsPressed(false);
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
             var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                e.Handled = true;

                if (m_clickableArea.IsMouseCaptured)
                {
                    m_clickableArea.ReleaseMouseCapture();
                }

                if (IsPressed)
                {
                    OnClick();
                }

                base.OnMouseLeftButtonUp(e);
            }
        }

#if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch!=null && ribbonTouch.EnableTouch && msystemGesture == SystemGesture.Tap)
            {
                e.Handled = true;

                if (m_clickableArea.IsMouseCaptured)
                {
                    m_clickableArea.ReleaseMouseCapture();
                }

                if (IsPressed)
                {
                    OnClick();
                }
                base.OnTouchUp(e);
            }
        }

#endif


        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            //e.Handled = true;
            if (e.Key == Key.Right)
            {
                IsMenuOpen = true;
                if (this.Items.Count != 0)
                {
                    ApplicationMenuGroup appGroup = Items[0] as ApplicationMenuGroup;
                    if (null != appGroup)
                    {
                        if (appGroup.Items.Count > 0)
                        {
                            int i = 0;
                            while (i < appGroup.Items.Count)
                            {
                                FrameworkElement firstItem = appGroup.Items[i] as FrameworkElement;
                                if (null != firstItem)
                                {
                                    if (!(firstItem is Separator))
                                    {
                                        firstItem.Focus();
                                        break;
                                    }
                                }
                                i++;
                            }
                        }
                        HandleKeyDownEvents(appGroup);
                    }
                }
            }
        }


        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            msystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// Handles the key down events.
        /// </summary>
        /// <param name="appGroup">The app group.</param>
        private void HandleKeyDownEvents(ApplicationMenuGroup appGroup)
        {
            foreach (FrameworkElement item in appGroup.Items)
            {
                item.Tag = appGroup;
                item.KeyDown += new KeyEventHandler(item_KeyDown);
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the item control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void item_KeyDown(object sender, KeyEventArgs e)
        {
            //e.Handled = true;
            if (e.Key == Key.Left)
            {
                IsMenuOpen = false;
                this.Focus();
            }
            else if (e.Key == Key.Down)
            {
                ApplicationMenuGroup appGroup = ((FrameworkElement)sender).Tag as ApplicationMenuGroup;
                if (appGroup != null)
                {
                    int index = appGroup.Items.IndexOf((FrameworkElement)sender);
                    int i = index + 1;
                    if (i == appGroup.Items.Count)
                    {
                        MoveToNextMenuGroup(appGroup);
                    }
                    else
                    {
                        bool isfocusmoved = false;
                        while (i < appGroup.Items.Count)
                        {
                            FrameworkElement nextItem = appGroup.Items[i] as FrameworkElement;
                            if (!(nextItem is Separator))
                            {
                                nextItem.Focus();
                                isfocusmoved = true;
                                break;
                            }
                            i++;
                        }
                        if (!isfocusmoved)
                        {
                            MoveToNextMenuGroup(appGroup);
                        }
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                ApplicationMenuGroup appGroup = ((FrameworkElement)sender).Tag as ApplicationMenuGroup;
                if (appGroup != null)
                {
                    int index = appGroup.Items.IndexOf((FrameworkElement)sender);
                    int i = index - 1;
                    if (index == -1)
                    {
                        MoveToPrevMenuGroup(appGroup);
                    }
                    else
                    {
                        bool isfocusmoved = false;
                        while (i >= 0)
                        {
                            FrameworkElement prevItem = appGroup.Items[i] as FrameworkElement;
                            if (!(prevItem is Separator))
                            {
                                prevItem.Focus();
                                isfocusmoved = true;
                                break;
                            }
                            i--;
                        }
                        if (!isfocusmoved)
                        {
                            MoveToPrevMenuGroup(appGroup);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves to next menu group.
        /// </summary>
        /// <param name="appGroup">The app group.</param>
        private void MoveToNextMenuGroup(ApplicationMenuGroup appGroup)
        {
            int app_index = Items.IndexOf(appGroup);
            if (app_index + 1 < Items.Count)
            {
                appGroup = Items[app_index + 1] as ApplicationMenuGroup;
                if (null != appGroup)
                {
                    if (appGroup.Items.Count > 0)
                    {
                        int k = 0;
                        while (k < appGroup.Items.Count)
                        {
                            FrameworkElement firstItem = appGroup.Items[k] as FrameworkElement;
                            if (null != firstItem)
                            {
                                if (!(firstItem is Separator))
                                {
                                    firstItem.Focus();
                                    break;
                                }
                            }
                            k++;
                        }
                    }
                    HandleKeyDownEvents(appGroup);
                }
            }
        }

        /// <summary>
        /// Moves to prev menu group.
        /// </summary>
        /// <param name="appGroup">The app group.</param>
        private void MoveToPrevMenuGroup(ApplicationMenuGroup appGroup)
        {
            int app_index = Items.IndexOf(appGroup);
            if (app_index - 1 >= 0)
            {
                appGroup = Items[app_index - 1] as ApplicationMenuGroup;
                if (null != appGroup)
                {
                    if (appGroup.Items.Count > 0)
                    {
                        int k = appGroup.Items.Count - 1;
                        while (k >= 0)
                        {
                            FrameworkElement lastItem = appGroup.Items[k] as FrameworkElement;
                            if (null != lastItem)
                            {
                                if (!(lastItem is Separator))
                                {
                                    lastItem.Focus();
                                    break;
                                }
                            }
                            k--;
                        }
                    }
                    HandleKeyDownEvents(appGroup);
                }
            }
        }
        /// <summary>
        /// Creates AutomationPeer for ribbon button.
        /// </summary>
        /// <returns>
        /// An appropriate SplitMenuButtonAutomationPeer for this control as
        /// part of the WPF infrastructure.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SplitMenuButtonAutomationPeer(this);
        }
        #endregion

        #region ICommand Interface Memembers

        /// <summary>
        /// make Command a dependency property so it can be DataBound
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitMenuButton), new PropertyMetadata((ICommand)null, new PropertyChangedCallback(CommandChanged)));

        /// <summary>
        /// Gets or sets the command that will be executed when the command source is invoked.
        /// </summary>
        /// <value></value>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// make CommandTarget a dependency property so it can be DataBound
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(SplitMenuButton), new PropertyMetadata((IInputElement)null));

        /// <summary>
        /// Gets or sets the command target
        /// </summary>
        /// <value></value>
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)GetValue(CommandTargetProperty);
            }

            set
            {
                SetValue(CommandTargetProperty, value);
            }
        }

        /// <summary>
        /// make CommandParameter a dependency property so it can be DataBound
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitMenuButton), new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the user defined data value that can be passed to the command when it is executed.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The command specific data.
        /// </returns>
        public object CommandParameter
        {
            get
            {
                return (object)GetValue(CommandParameterProperty);
            }

            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Commands the changed.
        /// </summary>
        /// <param name="d">The split menu button.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitMenuButton instance = (SplitMenuButton)d;
            instance.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        /// <summary>
        /// Hooks the up command.
        /// </summary>
        /// <param name="oldCommand">The old command.</param>
        /// <param name="newCommand">The new command.</param>
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }

            AddCommand(oldCommand, newCommand);
        }

        /// <summary>
        /// Removes the command.
        /// </summary>
        /// <param name="oldCommand">The old command.</param>
        /// <param name="newCommand">The new command.</param>
        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="oldCommand">The old command.</param>
        /// <param name="newCommand">The new command.</param>
        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = new EventHandler(CanExecuteChanged);
            canExecuteChangedHandler = handler;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += canExecuteChangedHandler;
            }
        }

        /// <summary>
        /// Determines whether this instance [can execute changed] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CanExecuteChanged(object sender, EventArgs e)
        {
            if (this.Command != null)
            {
                RoutedCommand command = this.Command as RoutedCommand;
            }
        }

        /// <summary>
        /// keep a copy of the handler so it doesn't get garbage collected
        /// </summary>
        private  EventHandler canExecuteChangedHandler;
        #endregion
    }  
}
