// <copyright file="MiniToolbar.cs" company="Syncfusion">
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a Ribbon MiniToolbar control.
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
    /// <example><code>public class MiniToolbar : Popup</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:MiniToolbar Name="toolbar" />]]></code></example>
    /// </list>
    /// <para/>
    /// </example>
    /// </list>
    /// <para/>
    /// <remarks>
    /// MiniToolbar class represents Ribbon MiniToolbar control for displaying UIElements in separate popup.
    /// </remarks>
    /// <para/>
    /// <example>
    /// <para/>This example shows how to create a MiniToolbar in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:MiniToolbar x:Key="MiniToolbar">
    ///     <ribbon:RibbonComboBox Width="100" IsEditable="True" >
    ///         <ComboBoxItem>Arial</ComboBoxItem>
    ///         <ComboBoxItem>Tahoma</ComboBoxItem>
    ///         <ComboBoxItem>Verdana</ComboBoxItem>
    ///     </ribbon:RibbonComboBox>
    ///     <ribbon:RibbonButton SmallIcon="/SampleImages/TextHighlight.png"/>
    /// </ribbon:MiniToolbar>
    /// ]]>
    /// </code>
    /// <para/>
    /// <para/>This example shows how to create a MiniToolbar in C#.
    /// <code>    
    /// MiniToolbar toolBar = new MiniToolbar();
    /// toolBar.PlacementTarget = Editor;
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ContentProperty("Items")]
    [DefaultProperty("Items")]
    public class MiniToolbar : Popup
    {
        #region Fields
        /// <summary>
        ///  Represents the opacity shown value
        /// </summary>
        private bool m_isOpacityShown = false;

        /// <summary>
        /// Represents the distance
        /// </summary>
        private double m_distance = 0;

        /// <summary>
        /// Represents the Hide value
        /// </summary>
        private bool m_canHide = true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of MiniToolbar child elements.
        /// </summary>
        /// <value>
        /// Type: <see cref="ItemCollection"/>
        /// Collection of child elements.
        /// </value>
        public ItemCollection Items
        {
            get
            {
                return (this.Child as ItemsControl).Items;
            }
        }

        /// <summary>
        /// Gets or sets a control template.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Windows.Controls.ControlTemplate"/>
        /// The template that defines the appearance of the MiniToolbar.
        /// </value>
        public ControlTemplate Template
        {
            get
            {
                return (ControlTemplate)GetValue(TemplateProperty);
            }

            set
            {
                SetValue(TemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can hide.
        /// </summary>
        /// <value><c>true</c> if this instance can hide; otherwise, <c>false</c>.</value>
        protected internal bool CanHide
        {
            get
            {
                return m_canHide;
            }

            set
            {
                m_canHide = value;
            }
        }

        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies the Template for the control. This is a dependency property. 
        /// </summary>        
        public static readonly DependencyProperty TemplateProperty = DependencyProperty.Register("Template", typeof(ControlTemplate), typeof(MiniToolbar));

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="MiniToolbar"/> class.
        /// </summary>
        static MiniToolbar()
        {
            EnvironmentTest.ValidateLicense(typeof(MiniToolbar));
            IsOpenProperty.OverrideMetadata(typeof(MiniToolbar), new FrameworkPropertyMetadata(false, null, new CoerceValueCallback(CoerceIsOpenCallback)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniToolbar"/> class.
        /// </summary>
        public MiniToolbar()
        {
            this.Placement = PlacementMode.MousePoint;
            this.VerticalOffset = -66;
            AutoTemplatedItemsControl child = new AutoTemplatedItemsControl(this.GetType());
            this.Child = child;
            StaysOpen = false;
            AllowsTransparency = true;
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseMoveEvent, new MouseEventHandler(OnMove));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// On the move.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMove(object sender, MouseEventArgs e)
        {
            if (m_isOpacityShown && CanHide)
            {
                Control ctrl = this.Child as Control;
                WindowInterop.POINT mousePt;
                WindowInterop.GetCursorPos(out mousePt);
                Point upperLeft = this.Child.PointToScreen(new Point(0, 0));
                Point bottomRight = this.Child.PointToScreen(new Point(ctrl.ActualWidth, ctrl.ActualHeight));
                OnMouseMove(mousePt.Location, new Rect(upperLeft, bottomRight));
            }
        }

        /// <summary>
        /// On the mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            IsOpen = false;
        }

        /// <summary>
        /// Coerces the is open callback.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>return result.</returns>
        private static object CoerceIsOpenCallback(DependencyObject d, object baseValue)
        {
            bool result = (bool)baseValue;
            if (!result)
            {
                (d as MiniToolbar).m_isOpacityShown = false;
                (d as MiniToolbar).CanHide = true; 
            }

            return result;
        }

        /// <summary>
        /// This method is invoked whenever IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            object o = VisualUtils.FindAncestor(this.Child, typeof(Ribbon));
            base.OnInitialized(e);
        }

        /// <summary>
        /// Responds to the condition in which the value of the IsOpen property changes from false to true. 
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnOpened(EventArgs e)
        {
            if (Template != null)
            {
                BindingUtils.SetBinding(Child, this, ItemsControl.TemplateProperty, MiniToolbar.TemplateProperty);
            }

            object o = VisualUtils.FindAncestor(this.Child, typeof(Ribbon));
            base.OnOpened(e);
        }

        /// <summary>
        /// Called when [mouse move].
        /// </summary>
        /// <param name="pt">The pt value.</param>
        /// <param name="rc">The rc value.</param>
        private void OnMouseMove(Point pt, Rect rc)
        {
		if(!this.IsMouseOver)
		{
            double dx = 0;

            if (pt.X < rc.X)
            {
                dx = rc.X - pt.X;
            }
            else if (pt.X > rc.Right)
            {
                dx = pt.X - rc.Right;
            }

            double dy = 0;

            if (pt.Y < rc.Y)
            {
                dy = rc.Y - pt.Y;
            }
            else if (pt.Y > rc.Bottom)
            {
                dy = pt.Y - rc.Bottom;
            }

            double distance = Math.Max(dx, dy);
            if (m_distance >= 0 && distance > m_distance)
            {
                double range = distance - m_distance;

                if (range < 90 * 2)
                {
                    this.Child.Opacity = 1.0f - range / 90;
                }
                else
                {
                    Hide();
                }
            }
            else
            {
                m_distance = distance;
            }
		}
        }

        /// <summary>
        /// Handles the MouseRightButtonUp event of the element control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Element_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
            {
                IsOpen = true;
                m_isOpacityShown = false;
                e.Handled = false;
            }
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.LostMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">TheMouseEventArgs that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(e.OriginalSource as UIElement);
            if (peer != null && peer is IExpandCollapseProvider)
            {
                m_canHide = true;
            }

            base.OnLostMouseCapture(e);
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.GotMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            CanHide = true;

            AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(e.OriginalSource as UIElement);
            if (peer != null && peer is IExpandCollapseProvider)
            {
                m_canHide = false;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Hides the MiniToolbar popup.
        /// </summary>
        public void Hide()
        {
            IsOpen = false;
            m_isOpacityShown = false;
            CanHide = true;
        }

        /// <summary>
        /// Shows the MiniToolbar popup.
        /// </summary>
        public void Show()
        {
            IsOpen = true;
            m_isOpacityShown = true;
        }

        /// <summary>
        /// Associates MiniToolbar with UIElement for the right click purpose.
        /// </summary>
        /// <param name="element">UIElement which is associated with MiniToolbar.</param>
        public void AssociateWith(UIElement element)
        {
            PlacementTarget = element;
            element.MouseRightButtonUp += new MouseButtonEventHandler(Element_MouseRightButtonUp);
        }

        #endregion
    }

    /// <summary>
    /// Class used to create items control that uses some other type as it's default style key. 
    /// Useful when element can not have template itself and it's internal classes should not be visible to user.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class AutoTemplatedItemsControl : ItemsControl
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTemplatedItemsControl"/> class.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        public AutoTemplatedItemsControl(Type keyType)
        {
            if (keyType == null)
            {
                throw new ArgumentNullException("keyType");
            }

            this.SetValue(DefaultStyleKeyProperty, keyType);
        }
        #endregion
    }
}
