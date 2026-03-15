// <copyright file="SplitterPage.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Present page in splitter items.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Skin.Default,
Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
 Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
 Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
Type = typeof(SplitterPage), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/MetroStyle.xaml")]
    public class SplitterPage : HeaderedContentControl
    {
        #region Private members
        /// <summary>
        /// Presents PAge storage
        /// </summary>
        private SplitterPagesCollection m_pageStorage = null;

        /// <summary>
        /// Presents the Temp storage for headers
        /// </summary>
        internal string m_Header = null;
        #endregion

        #region Properties
        /// <summary>
        /// Sets the page storage.
        /// </summary>
        /// <value>The page storage.</value>
        internal SplitterPagesCollection PageStorage
        {
            set
            {
                m_pageStorage = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        [Category("Appearance"), Bindable(true)]
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected page.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected page; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelectedPage
        {
            get
            {
                return (bool)GetValue(IsSelectedPageProperty);
            }

            set
            {
                SetValue(IsSelectedPageProperty, value);
            }
        }
        /// <summary>
        ///  Gets or sets a Image(leftImage) for TabSplitterPage
        /// </summary>
        public ImageSource Image
        {
            get
            {
                return (ImageSource)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="SplitterPage"/> class.
        /// </summary>
        static SplitterPage()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterPage), new FrameworkPropertyMetadata(typeof(SplitterPage)));
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the TabStripPlacement property.  This dependency property
        /// indicates ....
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <returns> Dock TabStripPlacementProperty</returns>
        public static Dock GetTabStripPlacement(DependencyObject d)
        {
            return (Dock)d.GetValue(TabStripPlacementProperty);
        }

        /// <summary>
        /// Provides a secure method for setting the TabStripPlacement property.
        /// This dependency property indicates ....
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="value">The value.</param>
        internal static void SetTabStripPlacement(DependencyObject d, Dock value)
        {
            d.SetValue(TabStripPlacementPropertyKey, value);
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Raises the Selected event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(true, e);
        }
        
        /// <summary>
        /// Raises the Unselected event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(false, e);
        }
        
        /// <summary>
        /// Handles the is selected changed.
        /// </summary>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleIsSelectedChanged(bool newValue, RoutedEventArgs e)
        {
            RaiseEvent(e);
        }
        protected override void OnContentChanged(object oldContent, object newContent)
        {                    
            base.OnContentChanged(oldContent, newContent);
           TabSplitterItem tab = null;
           if (m_pageStorage != null)
           {
               tab = this.m_pageStorage.CollectionOwner;
           }
          if (tab!=null && tab.TabSplitterParent!= null)
          {
              tab.TabSplitterParent.UpdateOnSelection();
          }
        }
        
        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitterPage container = (SplitterPage)d;
            bool newValue = (bool)e.NewValue;

            if (newValue)
            {
                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }
        }

        /// <summary>
        /// Called when [is selected page changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SplitterPage container = (SplitterPage)sender;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            SelectThisPage();
        }
        
        /// <summary>
        /// Selects current page.
        /// </summary>
        internal void SelectThisPage()
        {
            if (m_pageStorage != null)
            {
                m_pageStorage.SelectedItem = this;
                m_pageStorage.CollectionOwner.SelectedPage = this;
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Header != null)
            {
                m_Header = Header.ToString();
            }
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (m_pageStorage != null)
            {
                foreach (SplitterPage element in m_pageStorage)
                {
                    TabSplitter splitter = VisualUtils.FindAncestor(element, typeof(TabSplitter)) as TabSplitter;
                    if (splitter != null)
                    {
                        FrameworkElement content = element.Content as FrameworkElement;
                        if (content != null)
                        {
                            BindingUtils.SetBinding(content, splitter, FrameworkElement.FlowDirectionProperty, TabSplitter.FlowDirectionProperty, BindingMode.OneWay);
                        }
                    }
                }
            }
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Gets collection of the SplitterPages.
        /// </summary>
        /// <returns>SplitterPages Collection</returns>
        internal SplitterPagesCollection GetPageStorage()
        {
            return m_pageStorage;
        }
        
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            m_pageStorage.CollectionOwner.CollapseTabSplitterBottomPanel();
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// TabStripPlacement Read-Only Dependency Property
        /// </summary>
        internal static readonly DependencyPropertyKey TabStripPlacementPropertyKey = DependencyProperty.RegisterAttachedReadOnly("TabStripPlacement", typeof(Dock), typeof(SplitterPage), new FrameworkPropertyMetadata(Dock.Top, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Represents the TabStripPlacement Dependency Property
        /// </summary>
        public static readonly DependencyProperty TabStripPlacementProperty = TabStripPlacementPropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the IsSelected Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(SplitterPage), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(SplitterPage.OnIsSelectedChanged)));
        
        /// <summary>
        /// Identifies IsSelected dependency property of the SplitterPage.
        /// </summary>
        public static readonly DependencyProperty IsSelectedPageProperty = DependencyProperty.Register("IsSelectedPage", typeof(bool), typeof(SplitterPage), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedPageChanged)));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(SplitterPage), new FrameworkPropertyMetadata(null));
        #endregion
    }
}
