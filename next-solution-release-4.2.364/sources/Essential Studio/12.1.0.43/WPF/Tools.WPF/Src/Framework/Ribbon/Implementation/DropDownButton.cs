// <copyright file="DropDownButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DropDownButton control.
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
    /// <example><code>public class DropDownButton : RibbonItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:DropDownButton Name="button" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// DropDownButton class represents a control that can display child UIElements in a dropdown popup.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a DropDownButton in XAML.
    /// <code>
    /// <![CDATA[<ribbon:DropDownButton SmallIcon="SampleImages/ChangeCase16.png">
    /// <ribbon:RibbonMenuItem Header="Sentence case"/>
    /// <ribbon:RibbonMenuItem Header="lowercase"/>
    /// <ribbon:RibbonMenuItem Header="UPPERCASE"/>
    /// <ribbon:RibbonMenuItem Header="Capitalize Each Word"/>
    /// <ribbon:RibbonMenuItem Header="tOOGLE cASE"/>
    /// </ribbon:DropDownButton>]]></code>
    /// <para/>This example shows how to create a DropDownButton in C#.
    /// <code>
    /// DropDownButton button = new DropDownButton();
    /// RibbonMenuItem item = new RibbonMenuItem();
    /// button.Items.Add(item);
    /// stackPanel.Children.Add(button);
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
   Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
 Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
 Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2013,
Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2013Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Windows8,
Type = typeof(DropDownButton), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Windows8Style.xaml")]
   

    public class DropDownButton : RibbonItemsControl
    {
        /// <summary>
        /// Menu Popup.
        /// </summary>
        private Popup m_popup;

        SystemGesture m_systemGesture;

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="DropDownButton"/> class.
        /// </summary>
        static DropDownButton()
        {
            EnvironmentTest.ValidateLicense(typeof(DropDownButton));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));

            FrameworkPropertyMetadata sourceData = new FrameworkPropertyMetadata();
            PropertyMetadata savedSourceData = ItemsSourceProperty.GetMetadata(typeof(DropDownButton));

            sourceData.DefaultValue = savedSourceData.DefaultValue;
            sourceData.PropertyChangedCallback = savedSourceData.PropertyChangedCallback;
            sourceData.CoerceValueCallback = new CoerceValueCallback(CoerceItemsSourceChanged);

            //ItemsSourceProperty.OverrideMetadata(typeof(DropDownButton), sourceData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownButton"/> class.
        /// </summary>
        public DropDownButton()
        {
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
                if (this.Items.Count > 0 && this.Items[0] != null)
                {
                    e.Handled = true;
                    RibbonMenuItem item = this.Items[0] as RibbonMenuItem;
                    Keyboard.Focus(item);
                }
            base.OnKeyDown(e);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this button is pressed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsPressed
        {
            get
            {
                return (bool)GetValue(IsPressedProperty);
            }
        }

        /// <summary>
        /// Gets or sets path which is contained in toggle button.
        /// </summary>
        /// <value>
        /// Type: <see cref="Geometry"/>
        /// Geometry used in bottom part of the DropDownButton.
        /// </value>
        public Geometry PathGeometry
        {
            get
            {
                return (Geometry)GetValue(PathGeometryProperty);
            }

            set
            {
                SetValue(PathGeometryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets tooltip for toggle button which is available at bottom part of large size form.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Object to display as a tooltip in bottom part of the DropDownButton.
        /// </value>
        public object ToggleButtonToolTip
        {
            get
            {
                return (object)GetValue(ToggleButtonToolTipProperty);
            }

            set
            {
                SetValue(ToggleButtonToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can add child items in QAT using right click. If true, user can not add its child as QAT item. This is a dependency property.
        /// </summary>
        /// <value><c>true</c> if this instance is group; otherwise, <c>false</c>.</value>
        public bool IsGroup
        {
            get
            {
                return (bool)GetValue(IsGroupProperty);
            }

            set
            {
                SetValue(IsGroupProperty, value);
            }
        }
        #endregion

        #region Events
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsPressed property is changed. 
        /// </summary>
        public event PropertyChangedCallback IsPressedChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when PathGeometry property is changed. 
        /// </summary>
        public event PropertyChangedCallback PathGeometryChanged;
        #endregion

        #region Dependency properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Key through which IsPressed property can be changed.  This is a dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey IsPressedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsPressed", typeof(bool), typeof(DropDownButton), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsPressedChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Identifies when button is pressed.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines path which is contained in toggle button.  This is a dependency property. 
        /// </summary>
        public static readonly DependencyProperty PathGeometryProperty =
            DependencyProperty.Register("PathGeometry", typeof(Geometry), typeof(DropDownButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPathGeometryChanged)));

        /// <summary>
        /// Defines tooltip for toggle button which is contained in large size form dropdown .  This is a dependency property. 
        /// </summary>
        public static readonly DependencyProperty ToggleButtonToolTipProperty =
            DependencyProperty.Register("ToggleButtonToolTip", typeof(object), typeof(DropDownButton), new FrameworkPropertyMetadata(null));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines IsGrouped dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsGroupProperty =
            DependencyProperty.Register("IsGroup", typeof(bool), typeof(DropDownButton), new FrameworkPropertyMetadata(false));
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnPathGeometryChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnPathGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton instance = (DropDownButton)d;
            instance.OnPathGeometryChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises PathGeometryChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnPathGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PathGeometryChanged != null)
            {
                PathGeometryChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsPressedChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnIsPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton instance = (DropDownButton)d;
            instance.OnIsPressedChanged(e);
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
                Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
                QuickAccessToolBarPanel qatPanel = VisualUtils.FindAncestor(e.Source as Visual, typeof(QuickAccessToolBarPanel)) as QuickAccessToolBarPanel;
                if (ribbon != null && this.ContextMenu == null && qatPanel == null)
                {
                    //RibbonContextMenu.CreateContextMenu(this);
                    if (this.m_popup != null)
                        this.m_popup.IsOpen = true;
                    e.Handled = true;
                }
            }
           
        }

          #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbon != null && ribbon.EnableTouch && m_systemGesture==SystemGesture.RightTap)
            {
                base.OnTouchUp(e);

                QuickAccessToolBarPanel qatPanel = VisualUtils.FindAncestor(e.Source as Visual, typeof(QuickAccessToolBarPanel)) as QuickAccessToolBarPanel;
                if (ribbon != null && this.ContextMenu == null && qatPanel == null)
                {
                    //RibbonContextMenu.CreateContextMenu(this);
                    if (this.m_popup != null)
                        this.m_popup.IsOpen = true;
                    e.Handled = true;
                }
            }
        }
#endif


        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_systemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsPressedChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsPressedChanged != null)
            {
                IsPressedChanged(this, e);
            }
        }

        /// <summary>
        /// Sets IsPressed dependency property.
        /// </summary>
        /// <param name="pressed">Value which should be set. </param>
        protected void SetIsPressed(bool pressed)
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
        /// Gets the app menu pop up.
        /// </summary>
        /// <value>The app menu pop up.</value>
        internal Popup MenuPopUp
        {
            get
            {
                return m_popup;
            }
        }

        /// <summary>
        /// Coerces the items source changed.
        /// </summary>
        /// <param name="d">The d value</param>
        /// <param name="baseObject">The base object.</param>
        /// <returns>base object</returns>
        private static object CoerceItemsSourceChanged(DependencyObject d, object baseObject)
        {
            ItemsControl control = (ItemsControl)d;
            IEnumerable newValue = (IEnumerable)baseObject;

            if (newValue != null)
            {
                ObjectToRibbonDropDownItemConverter converter = new ObjectToRibbonDropDownItemConverter();
                return converter.Convert(newValue, typeof(IEnumerable), null, System.Globalization.CultureInfo.CurrentCulture);
            }

            return baseObject;
        }

        #endregion

        #region Override methods

        /// <summary>
        /// Represents the suspended Items
        /// </summary>
        //SU I78477
        //private bool m_bItemsChangedSuspended = false;
        //EU I78477

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            //if (!m_bItemsChangedSuspended && e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            //{
            //    m_bItemsChangedSuspended = true;

            //    IList en = (IList)CoerceItemsSourceChanged(this, e.NewItems);
            //    for (int i = 0; i < en.Count; i++)
            //    {
            //        ((IList)ItemsSource).RemoveAt(e.NewStartingIndex);
            //        ((IList)ItemsSource).Add(en[i]);
            //    }

            //    m_bItemsChangedSuspended = false;
            //}
        }
     
        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if(!DesignerProperties.GetIsInDesignMode(this))
            {
                m_popup = (Popup)GetTemplateChild("PART_Popup");
            }
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrameworkElement;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonMenuItem();
            //return new FrameworkElement();
        }
        #endregion
    }
}
