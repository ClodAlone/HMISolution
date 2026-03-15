// <copyright file="ContextTabGroup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class represents observable collection of RibbonTabs.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonTabCollection : ObservableCollection<RibbonTab>
    {
    }

    /// <summary>
    /// Represents a ContextTabGroup control.
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
    /// <example><code>public class ContextTabGroup : Control</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:ContextTabGroup Name="group" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// ContextTabGroup class describes the RibbonTab context group used for Contextual Tabs Ribbon feature.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a ContextTabGroup in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:ContextTabGroup>
    /// <ribbon:RibbonTab Caption="ContextTab"/>
    /// </ribbon:ContextTabGroup>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a ContextTabGroup in C#.
    /// <code>    
    /// ContextTabGroup contextTabGroup = new ContextTabGroup();
    /// contextTabGroup.Label = "New Context Group " + Ribbon.ContextTabGroups.Count;
    /// contextTabGroup.BackColor = Colors.Aqua;
    /// RibbonTab tab = new RibbonTab();
    /// tab.Caption = "New tab";
    /// contextTabGroup.RibbonTabs.Add(tab);
    /// Ribbon.ContextTabGroups.Add(contextTabGroup);
    /// contextTabGroup.IsGroupVisible = true;
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ContentProperty("RibbonTabs")]
    [DefaultProperty("RibbonTabs")]
    public class ContextTabGroup : Control
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="ContextTabGroup"/> class.
        /// </summary>
        static ContextTabGroup()
        {
            EnvironmentTest.ValidateLicense(typeof(ContextTabGroup));
            IsGroupVisibleChangedEvent = EventManager.RegisterRoutedEvent("IsGroupVisibleChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ContextTabGroup));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextTabGroup"/> class.
        /// </summary>
        public ContextTabGroup()
        {
            Focusable = false;
            RibbonTabs = new RibbonTabCollection();
            RibbonTabs.CollectionChanged += new NotifyCollectionChangedEventHandler(RibbonTabs_CollectionChanged);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Invoked when Initialized event is raised.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Binding binding = new Binding("IsGroupVisible");
            binding.Converter = new BooleanToVisibilityConverter();
            binding.Source = this;
            SetBinding(VisibilityProperty, binding);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// RibbonTabs collection changed event handler.
        /// </summary>
        /// <param name="sender">Changed collection.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void RibbonTabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    RibbonTab tab = e.NewItems[0] as RibbonTab;
                    Binding binding = new Binding("IsGroupVisible");
                    binding.Converter = new BooleanToVisibilityConverter();
                    binding.Source = this;
                    tab.SetBinding(UIElement.VisibilityProperty, binding);
                    tab.ContextTabGroup = this;

                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Calls OnIsGroupVisibleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsGroupVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContextTabGroup instance = (ContextTabGroup)d;
            instance.OnIsGroupVisibleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsGroupVisibleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsGroupVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(IsGroupVisibleChangedEvent, this);
            RaiseEvent(args);

            if (IsGroupVisibleChanged != null)
            {
                IsGroupVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Checks the first tab.
        /// </summary>
        private void CheckFirstTab()
        {
            this.RibbonTabs[0].IsChecked = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the collection of RibbonTabs.
        /// </summary>     
        /// <value>
        /// Type: <see cref="RibbonTabCollection"/>
        /// Collection of <see cref="RibbonTab"/>
        /// </value>
        /// <seealso cref="RibbonTabCollection"/>
        public RibbonTabCollection RibbonTabs
        {
            get
            {
                return (RibbonTabCollection)GetValue(RibbonTabsProperty);
            }

            set
            {
                SetValue(RibbonTabsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is group visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is group visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsGroupVisible
        {
            get
            {
                return (bool)GetValue(IsGroupVisibleProperty);
            }

            set
            {
                SetValue(IsGroupVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label of the context tab group.
        /// </summary>
        /// Type: <see cref="String"/>
        /// Text that labels the <see cref="ContextTabGroup"/>. The default is empty string.
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background color of the context tab group.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// Color of the context tab group background.
        /// </value>
        public Color BackColor
        {
            get
            {
                return (Color)GetValue(BackColorProperty);
            }

            set
            {
                SetValue(BackColorProperty, value);
            }
        }

        internal ContextAdorner ContextAdorner { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when IsGroupVisible property is changed.
        /// </summary>
        public event PropertyChangedCallback IsGroupVisibleChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Gets or sets whether context tab group is visible.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGroupVisibleProperty =
            DependencyProperty.Register("IsGroupVisible", typeof(bool), typeof(ContextTabGroup), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsGroupVisibleChanged)));

        /// <summary>
        /// Gets or sets collection of ribbon tabs.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty RibbonTabsProperty =
            DependencyProperty.Register("RibbonTabs", typeof(RibbonTabCollection), typeof(ContextTabGroup), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the label of the context tab group.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ContextTabGroup), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the background color of the context tab group.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register("BackColor", typeof(Color), typeof(ContextTabGroup), new UIPropertyMetadata(Colors.Transparent));
        #endregion

        #region Routed Events
        /// <summary>
        /// Event is raised when IsGroupVisible property changes.
        /// </summary>
        public static readonly RoutedEvent IsGroupVisibleChangedEvent;
        #endregion

        #region Public methods

        /// <summary>
        /// Activates this instance.
        /// </summary>
        /// <returns>Returns the active tab</returns>
        public bool Activate()
        {
            if (this.IsGroupVisible && this.RibbonTabs != null && this.RibbonTabs.Count > 0)
            {
                foreach (RibbonTab tab in this.RibbonTabs)
                {
                    if (tab.IsChecked)
                    {
                        return true;
                    }
                }

                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new ThreadStart(CheckFirstTab));

                return true;
            }

            return false;
        }
        #endregion
    }
}
