// <copyright file="RibbonGalleryItem.cs" company="Syncfusion">
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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a RibbonGalleryItem control.
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
    /// <example><code>public class RibbonGalleryItem : ButtonBase</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:RibbonGalleryItem Name="iteml" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// RibbonGalleryItem class represents a control that is used to wrap any content inside the Ribbon gallery control.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a RibbonGalleryItem in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:RibbonGalleryItem>
    ///   <Image Source="SampleImages/Apex.png"/>
    /// </ribbon:RibbonGalleryItem>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a ButtonPanel in C#.
    /// <code>
    /// TextBlock text = new TextBlock();
    /// text.Text = "Item";
    /// RibbonGalleryItem item = new RibbonGalleryItem();
    /// item.Content = text;
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonGalleryItem : ButtonBase
    {
        #region Properties

        SystemGesture m_systemGesture;

        /// <summary>
        /// Gets or sets a value indicating whether [check on click].
        /// </summary>
        /// <value><c>true</c> if [check on click]; otherwise, <c>false</c>.</value>
        public bool CheckOnClick
        {
            get
            {
                return (bool)GetValue(CheckOnClickProperty);
            }

            set
            {
                SetValue(CheckOnClickProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsChecked
        {
            get
            {
                return (bool)GetValue(IsCheckedProperty);
            }

            set
            {
                SetValue(IsCheckedProperty, value);
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonGalleryItem"/> class.
        /// </summary>
        static RibbonGalleryItem()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonGalleryItem));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGalleryItem), new FrameworkPropertyMetadata(typeof(RibbonGalleryItem)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonGalleryItem"/> class.
        /// </summary>
        public RibbonGalleryItem()
        {
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when CheckOnClick property is changed.
        /// </summary>
        public event PropertyChangedCallback CheckOnClickChanged;

        /// <summary>
        /// Event that is raised when IsChecked property is changed.
        /// </summary>
        public event PropertyChangedCallback IsCheckedChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines whether gallery item can be checked on mouse Click.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckOnClickProperty =
            DependencyProperty.Register("CheckOnClick", typeof(bool), typeof(RibbonGalleryItem), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCheckOnClickChanged)));

        /// <summary>
        /// Defines whether gallery item is checked.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(RibbonGalleryItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCheckedChanged)));
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnCheckOnClickChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCheckOnClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGalleryItem instance = (RibbonGalleryItem)d;
            instance.OnCheckOnClickChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CheckOnClickChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCheckOnClickChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CheckOnClickChanged != null)
            {
                CheckOnClickChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsCheckedChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGalleryItem instance = (RibbonGalleryItem)d;
            instance.OnIsCheckedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsCheckedChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsCheckedChanged != null)
            {
                IsCheckedChanged(this, e);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Creates AutomationPeer for ribbon button.
        /// </summary>
        /// <returns>
        /// An appropriate RibbonButtonAutomationPeer for this control as
        /// part of the WPF infrastructure.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RibbonGalleryItemAutomationPeer(this);
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event that occurs when the left mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnMouseLeftButtonUp(e);
                e.Handled = false;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnMouseRightButtonUp(e);
                RibbonContextMenu.CreateContextMenu(this);
                e.Handled = true;
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch)
            {
                if (m_systemGesture == SystemGesture.Tap)
                {
                    e.Handled = false;
                }
                if (m_systemGesture == SystemGesture.RightTap)
                {
                    RibbonContextMenu.CreateContextMenu(this);
                    e.Handled = true;
                }
                base.OnTouchUp(e);
            }
        }
        #endif


        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_systemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        #endregion
    }

    #region UI Automation support
    /// <summary>
    /// Class that provides UI Automation support.
    /// </summary>
    public class RibbonGalleryItemAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonGalleryItemAutomationPeer"/> class.
        /// </summary>
        /// <param name="control">The control.</param>
        public RibbonGalleryItemAutomationPeer(RibbonGalleryItem control)
            : base(control)
        {
        }

        /// <summary>
        /// Gets the name of the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer"/>. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="F:System.String.Empty"/> string.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return "RibbonGalleryItem";
        }

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetLocalizedControlType"/>.
        /// </summary>
        /// <returns>
        /// The string that contains the type of control.
        /// </returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return "button";
        }

        /// <summary>
        /// Gets the control type for the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer"/>. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom"/> enumeration value.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// <summary>
        /// Gets the control pattern for the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface"> A value from the enumeration.</param>
        /// <returns>Returns null</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets ribbon button object.
        /// </summary>
        private RibbonGalleryItem MyOwner
        {
            get
            {
                return (RibbonGalleryItem)base.Owner;
            }
        }

        #region IInvokeProvider Members
        /// <summary>
        /// Invokes RibbonButton Click event.
        /// </summary>
        /// <exception cref="T:System.Windows.Automation.ElementNotEnabledException">
        /// If the control is not enabled.
        /// </exception>
        public void Invoke()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
            MyOwner.RaiseEvent(newEventArgs);
            if (MyOwner.Command != null)
            {
                MyOwner.Command.Execute(MyOwner.CommandParameter);
            }
        }
        #endregion
    }
    #endregion
}
