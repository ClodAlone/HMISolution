// <copyright file="RibbonTab.cs" company="Syncfusion">
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
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents Ribbon TabItem control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonTab : ItemsControl
    {
        #region Private Members
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Tab button.
        /// </summary>
        internal TabButton m_tabButton;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Ribbon Tab margin.
        /// </summary>
        internal double M_margin;

        /// <summary>
        /// Ribbon Context Tab group
        /// </summary>
        private ContextTabGroup m_contextTabGroup;

        /// <summary>
        /// Ribbon Context Adorner
        /// </summary>
        private ContextAdorner m_contextAdorner;

        SystemGesture msystemGesture;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text that headers the <see cref="RibbonTab"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that headers the <see cref="RibbonTab"/>. The default is empty string.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonTab tab;
        /// button.Caption = "Insert tab";                
        /// </code>
        /// </example>
        /// <seealso cref="RibbonTab"/>
        /// <seealso cref="string"/>
        public string Caption
        {
            get
            {
                return (string)GetValue(CaptionProperty);
            }

            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tab is checked.
        /// </summary>
        /// <value>
        /// True if tab is checked; otherwise, false.
        /// </value>
        /// <seealso cref="RibbonTab"/>
        public bool IsChecked
        {
            get
            {              
                return (bool)GetValue(IsCheckedProperty);
            }

            set
            {
                SetValue(IsCheckedProperty, value);
                try
                {
                    m_tabButton.IsChecked = value;
                    if (value && m_tabButton.m_ribbonParent != null && (m_tabButton.m_ribbonParent.SelectedTabItem == null || (m_tabButton.m_ribbonParent.ItemsSource != null)))
                        m_tabButton.m_ribbonParent.SelectedTabItem = this;
                }
                catch
                { }
            }
        }

        /// <summary>
        /// Gets or sets color of the context tab color when <see cref="RibbonTab"/> is in <see cref="ContextTabGroup"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// Color value that will fill the <see cref="RibbonTab"/>
        /// </value>
        public Color ContextColor
        {
            get
            {
                return (Color)GetValue(ContextColorProperty);
            }

            set
            {
                SetValue(ContextColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the context tab group.
        /// </summary>
        /// <value>The context tab group.</value>
        protected internal ContextTabGroup ContextTabGroup
        {
            get
            {
                return m_contextTabGroup;
            }

            set
            {
                m_contextTabGroup = value;

                if (m_contextTabGroup != null)
                {
                    Binding binding = new Binding("BackColor");
                    binding.Source = m_contextTabGroup;
                    binding.Mode = BindingMode.TwoWay;
                    SetBinding(ContextColorProperty, binding);
                    HasContextTabGroup = true;
                }
                else
                {
                    HasContextTabGroup = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the context adorner.
        /// </summary>
        /// <value>The context adorner.</value>
        protected internal ContextAdorner ContextAdorner
        {
            get
            {
                return m_contextAdorner;
            }

            set
            {
                m_contextAdorner = value;

                if (m_contextAdorner != null)
                {
                    m_contextAdorner.MouseLeftButtonDown -= ContextAdorner_MouseLeftButtonDown;
                    m_contextAdorner.MouseLeftButtonDown += ContextAdorner_MouseLeftButtonDown;
                     #if !SyncfusionFramework3_5
                    m_contextAdorner.TouchDown -= new EventHandler<TouchEventArgs>(ContextAdorner_TouchDown);
                    m_contextAdorner.TouchDown += new EventHandler<TouchEventArgs>(ContextAdorner_TouchDown);
#endif

                    if (m_contextTabGroup.Style != null)
                    {
                        m_contextAdorner.TemplatedInnerControl.Style = m_contextTabGroup.Style;
                    }

                    if (m_contextTabGroup.Template != null)
                    {
                        m_contextAdorner.TemplatedInnerControl.Template = m_contextTabGroup.Template;
                    }

                    Binding binding = new Binding("IsVisible");
                    binding.Source = this;
                    binding.Converter = new BooleanToVisibilityConverter();

                    m_contextAdorner.SetBinding(VisibilityProperty, binding);
                }
            }
        }       

        /// <summary>
        /// Gets or sets a value indicating whether this instance has context tab group.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has context tab group; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool HasContextTabGroup
        {
            get
            {
                return (bool)GetValue(HasContextTabGroupProperty);
            }

            internal set
            {
                SetValue(HasContextTabGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the arrange.
        /// </summary>
        /// <value>The width of the arrange.</value>
        internal double ArrangeWidth
        {
            get
            {
                return m_tabButton.M_arrangeWidth;
            }

            set
            {
                if (value < MinWidth && value != 0)
                {
                    m_tabButton.M_arrangeWidth = MinWidth;
                }
                else
                {
                    m_tabButton.M_arrangeWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the tab button template.
        /// </summary>
        /// <value>The tab button template.</value>
        public ControlTemplate TabButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(TabButtonTemplateProperty);
            }

            set
            {
                SetValue(TabButtonTemplateProperty, value);
            }
        }

        /// <summary>        
        /// Gets or sets the style to be used by the tab button control.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Windows.Style"/>
        /// The applied, non default style for the tab button, if present. Otherwise, null reference (Nothing in Visual Basic). The default is null reference (Nothing in Visual Basic).
        /// </value>
        public Style TabButtonStyle
        {
            get
            {
                return (Style)GetValue(TabButtonStyleProperty);
            }

            set
            {
                SetValue(TabButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the data template used to display the content of <see cref="Syncfusion.Windows.Tools.Controls.RibbonTab"></see>. This is a dependency property./>
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }

            set
            {
                SetValue(ContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a template selector which enables an application-writer to provide custom template selection logic. This is a dependency property.
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
            }

            set
            {
                SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        public bool IsCancelRibbonState
        {
            get { return (bool)GetValue(IsCancelRibbonStateProperty); }
            set { SetValue(IsCancelRibbonStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTabClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCancelRibbonStateProperty =
            DependencyProperty.Register("IsCancelRibbonState", typeof(bool), typeof(RibbonTab), new PropertyMetadata(false));

        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies RibbonTab caption.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionProperty =
                DependencyProperty.Register("Caption", typeof(string), typeof(RibbonTab), new FrameworkPropertyMetadata(" ", new PropertyChangedCallback(OnCaptionChanged)));

        /// <summary>
        /// Identifies whether RibbonTab belongs to ContextTabGroup.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HasContextTabGroupProperty =
            DependencyProperty.Register("HasContextTabGroup", typeof(bool), typeof(RibbonTab), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies color of the context tab color when <see cref="RibbonTab"/> is in <see cref="ContextTabGroup"/>.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ContextColorProperty =
            DependencyProperty.Register("ContextColor", typeof(Color), typeof(RibbonTab), new FrameworkPropertyMetadata(Colors.Transparent, new PropertyChangedCallback(OnContextColorChanged)));

        /// <summary>
        /// Identifies a tab button control template.
        /// </summary>
        public static readonly DependencyProperty TabButtonTemplateProperty =
            DependencyProperty.Register("TabButtonTemplate", typeof(ControlTemplate), typeof(RibbonTab), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabButtonTemplateChanged)));
                   

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(RibbonTab), new UIPropertyMetadata(false));
               

        /// <summary>
        /// Identifies the style to be used by the tab button control.
        /// </summary>
        public static readonly DependencyProperty TabButtonStyleProperty =
            DependencyProperty.Register("TabButtonStyle", typeof(Style), typeof(RibbonTab), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabButtonStyleChanged)));

        /// <summary>
        /// Identifies the contenttemplate used by the RibbonTab Control.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(RibbonTab), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the template selector used by the Ribbon Tab content.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(RibbonTab), new FrameworkPropertyMetadata(null));
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonTab"/> class.
        /// </summary>
        static RibbonTab()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonTab));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTab), new FrameworkPropertyMetadata(typeof(RibbonTab)));
            VisibilityProperty.OverrideMetadata(typeof(RibbonTab), new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnVisibilityChanged)));
            VisibilityChangedEvent = EventManager.RegisterRoutedEvent("VisibilityChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RibbonTab));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonTab"/> class.
        /// </summary>
        public RibbonTab()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && Application.Current.MainWindow != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }

            this.FocusVisualStyle = null;
            Initialize();
            MinWidth = 23;
            Background = Brushes.Transparent;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            m_tabButton = new TabButton();
            BindingUtils.SetBinding(m_tabButton, this, TabButton.CaptionProperty, RibbonTab.CaptionProperty, BindingMode.TwoWay);
            BindingUtils.SetBinding(m_tabButton, this, TabButton.IsCheckedProperty, RibbonTab.IsCheckedProperty, BindingMode.TwoWay);
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Caption property is changed.
        /// </summary>
        public event PropertyChangedCallback CaptionChanged;

        /// <summary>
        /// Occurs when Visibility property is changed. 
        /// </summary>
        public event PropertyChangedCallback VisibilityChanged;

        /// <summary>
        /// Event that is raised when ContextColor property is changed.
        /// </summary>
        public event PropertyChangedCallback ContextColorChanged;

        /// <summary>
        /// Event that is raised when TabButtonTemplate property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback TabButtonTemplateChanged;

        /// <summary>
        /// Event that is raised when TabButtonStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback TabButtonStyleChanged;
        #endregion

        #region Static Methods
        /// <summary>
        /// Calls OnCaptionChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTab instance = (RibbonTab)d;
            instance.OnCaptionChanged(e);
        }

        /// <summary>
        /// Called when [visibility changed].
        /// </summary>
        /// <param name="d">The d param value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTab instance = (RibbonTab)d;
            instance.OnVisibilityChanged(e);
        }
        #endregion

        #region Override methods

        /// <summary>
        /// Updates property value cache and raises CaptionChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnCaptionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_tabButton != null)
            {
                #if SyncfusionFramework3_5
                m_tabButton.Caption = (string)e.NewValue;
                #else
                Binding binding = BindingOperations.GetBinding(m_tabButton, TabButton.CaptionProperty);
                if (binding != null && (binding.Mode == BindingMode.TwoWay))
                {
                    m_tabButton.Caption = (string)e.NewValue;
                }
                else
                    m_tabButton.SetCurrentValue(RibbonTab.CaptionProperty, (string)e.NewValue);
                
                #endif
            }
            Window m_ribbonWindow = (Window)VisualUtils.FindAncestor(this, typeof(Window));
            if (m_ribbonWindow != null && m_ribbonWindow.ActualWidth> 0)
            {            
                this.Measure(new Size(m_ribbonWindow.ActualWidth, 23));
            }           

            if (CaptionChanged != null)
            {
                CaptionChanged(this, e);
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />,
        /// and returns a child at the specified index from a collection
        /// of child elements.
        /// </summary>
        /// <param name="index">The zero\-based index of the requested
        /// child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if
        /// the provided index is out of range, an exception is raised.
        /// </returns>
        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 0:
                    return m_tabButton;
                default:
                    throw new Exception("Index for Visual Child is wrong.");
            }
        }

        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size for a ribbon tab.
        /// </summary>
        /// <param name="constraint">The available size that this
        /// element can give to the child.
        /// Infinity can be specified as a
        /// value to indicate that the element
        /// will size to whatever content is
        /// available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of children's sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            try
            {
                m_tabButton.InvalidateMeasure();
                m_tabButton.Measure(constraint);
                if (ArrangeWidth == 0)
                {
                    return new Size(m_tabButton.DesiredSize.Width, 23);
                }
            }
            catch { }

            return new Size(ArrangeWidth, 23);
        }

        /// <summary>
        /// Arranges child elements and determines a size for a ribbon
        /// tab.
        /// </summary>
        /// <param name="arrangeBounds">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            m_tabButton.M_arrangeWidth = arrangeBounds.Width;
            Size size = base.ArrangeOverride(new Size(arrangeBounds.Width, arrangeBounds.Height));
            M_margin = m_tabButton.M_margin;

            
            return size;
        }

        

        /// <summary>
        /// Invoked when the parent of this element in the visual tree is changed. Overrides <see cref="M:System.Windows.UIElement.OnVisualParentChanged(System.Windows.DependencyObject)"/>.
        /// </summary>
        /// <param name="oldParent">The old parent element. May be null to indicate that the element did not have a visual parent previously.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            
            if (m_tabButton != null && m_tabButton.InternalVisualParent == null)
            {
                if (this.VisualParent is TabPanel)
                {
                    m_tabButton.m_ribbonParent = (this.VisualParent as TabPanel).m_ribbon;
                    AddVisualChild(m_tabButton);
                }
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var panel = new RibbonLayoutPanel();
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (Parent != null)
                {
                    panel = VisualUtils.FindDescendant(Parent as Visual, typeof(RibbonLayoutPanel)) as RibbonLayoutPanel;
                    if (panel != null)
                        panel.ResetRemoveItems();
                }
            }

            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Calls OnTabButtonStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTabButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTab instance = (RibbonTab)d;
            instance.OnTabButtonStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabButtonStyleChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnTabButtonStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            m_tabButton.Style = (Style)e.NewValue;

            if (TabButtonStyleChanged != null)
            {
                TabButtonStyleChanged(this, e);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the ContextAdorner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ContextAdorner_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                Ribbon m_ribbon = (Ribbon)VisualUtils.FindAncestor(this, typeof(Ribbon));
                if (m_ribbon != null)
                {
                    if (m_ribbon.RibbonState == RibbonState.Hide)
                    {
                        m_ribbon.RibbonState = RibbonState.Normal;
                    }
                }
                else
                {
                    throw new NullReferenceException("Ribbon could not be found");
                }

                if (sender is ContextAdorner)
                {
                    if ((sender as ContextAdorner).ContextTabGroup != null)
                    {
                        var ribbonTabs = (sender as ContextAdorner).ContextTabGroup.RibbonTabs;

                        foreach (var item in ribbonTabs)
                        {
                            if (item.Visibility == Visibility.Visible)
                            {
                                item.IsChecked = true;
                                break;
                            }
                        }
                    }
                }

                //m_tabButton.IsChecked = true;
            }
        }

         #if !SyncfusionFramework3_5
        private void ContextAdorner_TouchDown(object sender, TouchEventArgs e)
        {
            Ribbon ribbonTouch = (Ribbon)VisualUtils.FindAncestor(this, typeof(Ribbon));
            if (ribbonTouch != null && ribbonTouch.EnableTouch && msystemGesture == SystemGesture.Tap)
            {
                Ribbon m_ribbon = (Ribbon)VisualUtils.FindAncestor(this, typeof(Ribbon));
                if (m_ribbon != null)
                {
                    if (m_ribbon.RibbonState == RibbonState.Hide)
                    {
                        m_ribbon.RibbonState = RibbonState.Normal;
                    }
                }
                else
                {
                    throw new NullReferenceException("Ribbon could not be found");
                }

                m_tabButton.IsChecked = true;
            }
        }
#endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            msystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// Raises the <see cref="E:VisibilityChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(VisibilityChangedEvent, this);
            RaiseEvent(args);

            if (VisibilityChanged != null)
            {
                VisibilityChanged(this, e);
            }

            Ribbon m_ribbon = (Ribbon)VisualUtils.FindAncestor(this, typeof(Ribbon));
            bool isContextTabGroup=false;
            if (m_ribbon != null)
            {
                foreach (var item in m_ribbon.ContextTabGroups)
                {
                    if (item is ContextTabGroup)
                    {
                        if (item.RibbonTabs != null && item.RibbonTabs.Contains(this))
                            isContextTabGroup = true;
                    }
                }
            }

            //if (e.Property == RibbonTab.VisibilityProperty)
            //{
                //if ((Visibility)e.NewValue == Visibility.Visible && !this.IsChecked && !isContextTabGroup)
                    //this.IsChecked = true;
            //}
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnContextColorChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnContextColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTab instance = (RibbonTab)d;
            instance.OnContextColorChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises ContextColorChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnContextColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ContextColorChanged != null)
            {
                ContextColorChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnTabButtonTemplateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTabButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTab instance = (RibbonTab)d;
            instance.OnTabButtonTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// TabButtonTemplateChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTabButtonTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            m_tabButton.Template = (ControlTemplate)e.NewValue;

            if (TabButtonTemplateChanged != null)
            {
                TabButtonTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Adds the childern.
        /// </summary>
        public void AddChildren()
        {
            m_tabButton.AddCanvas();
        }

        /// <summary>
        /// Sets the border.
        /// </summary>
        /// <param name="setdefault">if set to <c>true</c> [setdefault].</param>
        internal void SetBorder(bool setdefault)
        {
            RibbonBar bar;
            for (int i = 0; i < Items.Count; i++)
            {
                bar = Items[i] as RibbonBar;
                if (bar != null)
                {
                    if (!setdefault)
                    {
                        GradientStopCollection coll = new GradientStopCollection();
                        GradientStop gr = new GradientStop((Color)ColorConverter.ConvertFromString("#C7C7C7"), 1.0);
                        GradientStop gr1 = new GradientStop(ContextColor, 0.0);
                        coll.Add(gr);
                        coll.Add(gr1);
                        LinearGradientBrush brush = new LinearGradientBrush(coll, new Point(0.5, 1), new Point(0.5, 0));
                        if (bar.m_innerBorder != null)
                        {
                            bar.m_innerBorder.BorderBrush = brush;
                        }
                    }
                    else
                    {
                        if (bar.m_innerBorder != null)
                        {
                            bar.m_innerBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
                        }
                    }
                }
            }
        }

        #endregion

        #region Routed Events
        /// <summary>
        /// Event that is raised when Visibility property is changed.
        /// </summary>
        protected internal static readonly RoutedEvent VisibilityChangedEvent;
        #endregion

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RibbonTabAutomationPeer(this);
        }
    }
}
