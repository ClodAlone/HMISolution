// <copyright file="SplitButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
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
using System.Windows.Automation.Peers;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Controls represents Ribbon split style dropdown button.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SplitButton : DropDownButton, ICommandSource
    {
        #region Fields
        /// <summary>
        /// Represents the button
        /// </summary>
        private FrameworkElement m_button;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="SplitButton"/> class.
        /// </summary>
        static SplitButton()
        {
            EnvironmentTest.ValidateLicense(typeof(SplitButton));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
            ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitButton"/> class.
        /// </summary>
        public SplitButton()
        {
            
        }
        #endregion

        #region DP getters & setters
        /// <summary>
        /// Gets or sets the command to invoke when this <see cref="SplitButton"/> is pressed. 
        /// </summary>
        /// <remarks>
        /// Implements <see cref="ICommandSource.Command"/>.
        /// This property is used to associate a command with a particular button instance.
        /// In Extensible Application Markup Language (XAML), this property is most often set 
        /// to be a static command value from one of the existing command libraries, such as ApplicationCommands or NavigationCommands. For details, see ICommand.
        /// </remarks>
        /// <value>
        /// Type: <see cref="System.Windows.Input.ICommand"/>
        /// A command to invoke when this button is pressed. The default value is null reference (Nothing in Visual Basic).
        /// </value>
        /// <seealso cref="ICommand"/>
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
        /// Gets or sets the element on which to raise the specified command.
        /// </summary>
        /// <remarks>
        /// The CommandTarget property cannot be used to define a UIElement. 
        /// The CommandTarget property provides a reference to an element that is already 
        /// defined somewhere in your application.
        /// </remarks>
        /// <value>
        /// Type: <see cref="System.Windows.IInputElement"/>.
        /// Element on which to raise a command.
        /// </value>
        /// <seealso cref="ICommand"/>
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
        /// Gets or sets the parameter to pass to the Command property.
        /// </summary>
        /// <remarks>
        /// Most existing commands from the command libraries do not use a command parameter. 
        /// Of the commands that do use a command parameter, most of these take a parameter with 
        /// some primitive type value, such as an integer or a string. However, it is possible 
        /// to create a custom command where that command expects a non primitive type as its 
        /// command parameter. For a custom command case like this, setting CommandParameter in 
        /// code might require a new or existing object instance. Setting CommandParameter in markup 
        /// might require property element syntax, where the object element filling the property 
        /// element syntax is a new element of the type expected by that command. Alternatively, 
        /// setting in markup might require a reference through a markup extension to an existing object
        /// </remarks>
        /// <value>
        /// Type: <see cref="System.Object"/>
        /// Parameter to pass to the Command property.
        /// </value>
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
        /// Gets or sets the HitTest area size.
        /// </summary>
        /// <value>
        /// Type: <see cref="HitTestArea"/>
        /// ImageOnly specifies that only image is hit testable for command action, LabelAndImage specifies that image and text is hit testable for command action.  Default is LabelAndImage.
        /// </value>
        /// <seealso cref="HitTestArea"/>
        public HitTestArea HitTestArea
        {
            get
            {
                return (HitTestArea)GetValue(HitTestAreaProperty);
            }

            set
            {
                SetValue(HitTestAreaProperty, value);
            }
        }
        #endregion

        #region Dependency properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines Command associated with control.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButton), new PropertyMetadata((ICommand)null, new PropertyChangedCallback(OnCommandChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines CommandTarget object. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(SplitButton), new PropertyMetadata((IInputElement)null));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines Command parameter. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButton), new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the HitTest area size. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HitTestAreaProperty =
            DependencyProperty.Register("HitTestArea", typeof(HitTestArea), typeof(SplitButton), new FrameworkPropertyMetadata(HitTestArea.LabelAndImage, new PropertyChangedCallback(OnHitTestAreaChanged)));

        #endregion

        #region Implementation
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnCommandChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton instance = (SplitButton)d;
            instance.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        protected virtual void OnClick()
        {
           
            RoutedEventArgs e = new RoutedEventArgs(ClickEvent, this);
            RaiseEvent(e);

            if (Command != null)
            {
                RoutedCommand command = Command as RoutedCommand;
                if (command != null)
                {
                    IInputElement target = CommandTarget != null ? CommandTarget : this as IInputElement;
                    if (command.CanExecute(CommandParameter, target))
                    {
                        command.Execute(CommandParameter, target);
                    }
                }
                else
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        Command.Execute(CommandParameter);
                    }
                }
            }
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
                if (command != null)
                {
                    IInputElement target = CommandTarget != null ? CommandTarget : this as IInputElement;
                    if (command.CanExecute(CommandParameter, target))
                    {

                        this.IsEnabled = true;
                    }
                    else
                    {

                        this.IsEnabled = false;
                    }
                }
                else
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
            }
        }

        
        
        /// <summary>
        /// Keeps a copy of the handler so it doesn't get garbage
        /// collected 
        /// </summary>
        private EventHandler canExecuteChangedHandler;

        /// <summary>
        /// Calls OnHitTestAreaChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHitTestAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton instance = (SplitButton)d;
            instance.OnHitTestAreaChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HitTestAreaChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHitTestAreaChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HitTestAreaChanged != null)
            {
                HitTestAreaChanged(this, e);
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsDropDownOpen = false;

            SetIsPressed(true);

            e.Handled = true;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsPressed && IsDropDownOpen == false)
            {
                OnClick();
            }

            SetIsPressed(false);

            e.Handled = true;
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when HitTestArea property is changed.
        /// </summary>
        public event PropertyChangedCallback HitTestAreaChanged;

        /// <summary>
        /// Routed Click event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent;

        /// <summary>
        /// Adds or removes handler for click event.
        /// </summary>
        [Category("Behavior")]
        public event RoutedEventHandler Click
        {
            add
            {
                base.AddHandler(ClickEvent, value);
            }

            remove
            {
                base.RemoveHandler(ClickEvent, value);
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

            m_button = GetTemplateChild("PART_Button") as FrameworkElement;

            FrameworkElement m_toggleButton = GetTemplateChild("PART_ToggleButton") as FrameworkElement;
            if ((m_toggleButton.ToolTip!=null) && (m_toggleButton.ToolTip as ToolTip).Placement == PlacementMode.Mouse)
            {
                (m_toggleButton.ToolTip as ToolTip).Placement = ToolTipService.GetPlacement(this);    
            }

            if (m_button != null)
            {
                m_button.MouseLeftButtonDown += new MouseButtonEventHandler(Button_MouseLeftButtonDown);
                m_button.MouseLeftButtonUp += new MouseButtonEventHandler(Button_MouseLeftButtonUp);
            }

            if (Command != null)
            {
                if (RibbonCommandManager.CommandDictionary.ContainsKey(Command))
                {
                    if (RibbonCommandManager.CommandDictionary[Command].SmallIcon != null)
                    {
                        SmallIcon = RibbonCommandManager.CommandDictionary[Command].SmallIcon;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].Label != null)
                    {
                        Label = RibbonCommandManager.CommandDictionary[Command].Label;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].ToolTip != null)
                    {
                        ToolTip = RibbonCommandManager.CommandDictionary[Command].ToolTip;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/>�attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            SetIsPressed(false);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (this.SizeForm != Tools.SizeForm.ExtraSmall)
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

                SplitButton item = e.Source is SplitButton ? e.Source as SplitButton : this as SplitButton;
                QuickAccessToolBarPanel qatPanel = VisualUtils.FindAncestor(item as Visual, typeof(QuickAccessToolBarPanel)) as QuickAccessToolBarPanel;
                if (ribbon != null && this.ContextMenu == null && qatPanel == null && item != null)
                {
                    RibbonContextMenu.CreateContextMenu(item as FrameworkElement);
                    e.Handled = true;
                }
                if (qatPanel != null)
                    base.OnMouseRightButtonUp(e);
            }
            else
                base.OnMouseRightButtonUp(e);
        }
        #endregion

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SplitButtonAutomationPeer(this);
        }
    }

    public class SplitButtonAutomationPeer : ItemsControlAutomationPeer
    {
        public SplitButtonAutomationPeer(SplitButton owner)
            : base(owner)
        {

        }
        protected override string GetClassNameCore()
        {
            return "SplitButton";
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new SplitButtonDataAutomationPeer(item, this);
        }
    }

    public class SplitButtonDataAutomationPeer : ItemAutomationPeer
    {
        public SplitButtonDataAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer)
            : base(item, itemsControlAutomationPeer)
        {

        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return "SplitButton";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }
       
    }
}
