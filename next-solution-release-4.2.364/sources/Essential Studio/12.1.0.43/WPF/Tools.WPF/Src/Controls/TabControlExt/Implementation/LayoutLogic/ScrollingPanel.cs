// <copyright file="ScrollingPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class Represents the scrolling panel
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

     public class ScrollingPanel : Panel
    {
        #region Private members
        /// <summary>
        /// Button for Previous tab.
        /// </summary>
        private Button m_prevTab;

        /// <summary>
        /// Button for Next tab.
        /// </summary>
        private Button m_nextTab;

        /// <summary>
        /// Button for Previous page.
        /// </summary>
        private Button m_prevPage;

        /// <summary>
        /// Button for Next page.
        /// </summary>
        private Button m_nextPage;

        /// <summary>
        /// Button for First tab.
        /// </summary>
        private Button m_firstTab;

        /// <summary>
        /// Button for Last tab.
        /// </summary>
        private Button m_lastTab;

        /// <summary>
        /// tab layout parent panel
        /// </summary>
        public TabLayoutPanel parentPanel;

        /// <summary>
        /// checks whether it is showing
        /// </summary>
        internal bool Showing = false;

        TabControlExt tabcontrol = null;
        #endregion

        #region Private properties
        /// <summary>
        /// Gets the layout panel.
        /// </summary>
        /// <value>The layout panel.</value>
        private TabLayoutPanel LayoutPanel
        {
            get
            {
                if (Parent is TabLayoutPanel)
                {
                    return Parent as TabLayoutPanel;
                }
                return null;
                //throw new ArgumentException("Parent should be of TabLayoutPanel only");
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="ScrollingPanel"/> class.
        /// </summary>
        static ScrollingPanel()
        {
            EnvironmentTest.ValidateLicense(typeof(ScrollingPanel));
            FlowDirectionProperty.OverrideMetadata(typeof(ScrollingPanel), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFlowDirectionChanged), new CoerceValueCallback(OnCoerceFlowDirection)));
        }

        public ScrollingPanel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollingPanel"/> class.
        /// </summary>
        public ScrollingPanel(TabLayoutPanel tabpanel)
        {
            AllowDrop = false;
            this.parentPanel = tabpanel;
            InitButtons();
            Margin = new Thickness(1, 0, 0, 0);
            if (tabpanel.m_ParentTabControl.TabVisualStyle != TabVisualStyle.None)
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                VerticalAlignment = System.Windows.VerticalAlignment.Center;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Shows scroll buttons when it is needed.
        /// </summary>
        public void Show()
        {
            if (LayoutPanel.TabScrollStyle == TabScrollStyle.Extended)
            {
                if (LayoutPanel.m_ParentTabControl.TabVisualStyle == TabVisualStyle.None)
                {
                    m_prevPage.Visibility = Visibility.Visible;
                    m_nextPage.Visibility = Visibility.Visible;
                }
                m_firstTab.Visibility = Visibility.Visible;
                m_lastTab.Visibility = Visibility.Visible;
            }
            else
            {
                if (LayoutPanel.m_ParentTabControl.TabVisualStyle == TabVisualStyle.None)
                {
                    m_prevPage.Visibility = Visibility.Collapsed;
                    m_nextPage.Visibility = Visibility.Collapsed;
                }
                m_firstTab.Visibility = Visibility.Collapsed;
                m_lastTab.Visibility = Visibility.Collapsed;
            }

            m_prevTab.Visibility = Visibility.Visible;
            m_nextTab.Visibility = Visibility.Visible;
            Showing = true;
        }

      
        /// <summary>
        /// Hides scroll buttons when it is needed.
        /// </summary>
        public void Hide()
        {
            if (LayoutPanel.TabScrollStyle == TabScrollStyle.Extended)
            {
                m_prevPage.Visibility = Visibility.Collapsed;
                m_nextPage.Visibility = Visibility.Collapsed;
                m_firstTab.Visibility = Visibility.Collapsed;
                m_lastTab.Visibility = Visibility.Collapsed;
            }

            m_prevTab.Visibility = Visibility.Collapsed;
            m_nextTab.Visibility = Visibility.Collapsed;
            Showing = false;
        }

        /// <summary>
        /// Disables Next part of scrolling buttons.
        /// </summary>
        public void DisableNextPart()
        {
            if (LayoutPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                m_nextTab.IsEnabled = false;
                m_lastTab.IsEnabled = false;            
                m_nextPage.IsEnabled = false;
            }
        }

        /// <summary>
        /// Enables Next part of scrolling buttons.
        /// </summary>
        public void EnableNextPart()
        {
            if (LayoutPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                m_nextTab.IsEnabled = true;
                m_lastTab.IsEnabled = true;           
                m_nextPage.IsEnabled = true;
            }
        }

        /// <summary>
        /// Disables Prev part of scrolling buttons.
        /// </summary>
        public void DisablePrevPart()
        {
            if (LayoutPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                m_prevTab.IsEnabled = false;
                m_firstTab.IsEnabled = false;           
                m_prevPage.IsEnabled = false;
            }
        }

        /// <summary>
        /// Enables Prev part of scrolling buttons.
        /// </summary>
        public void EnablePrevPart()
        {
            if (LayoutPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                m_prevTab.IsEnabled = true;
                m_firstTab.IsEnabled = true;           
                m_prevPage.IsEnabled = true;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Invoked when the parent of this element in the visual tree is changed. Overrides <see cref="M:System.Windows.UIElement.OnVisualParentChanged(System.Windows.DependencyObject)"/>.
        /// </summary>
        /// <param name="oldParent">The old parent element. May be null to indicate that the element did not have a visual parent previously.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            Binding visualStyleBinding = new Binding();
            Binding visualStyleListBinding = new Binding();
            visualStyleBinding.Source = LayoutPanel;
            visualStyleListBinding.Source = LayoutPanel;
            visualStyleBinding.Mode = BindingMode.OneWay;
            visualStyleListBinding.Mode = BindingMode.OneWay;
            visualStyleBinding.Path = new PropertyPath(SkinStorage.VisualStyleProperty);
            SetBinding(SkinStorage.VisualStyleProperty, visualStyleBinding);
            
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(System.Windows.Size availableSize)
        {
            double totalWidth = 0;
            double maxHeight = 0;
            Size desiredSize = new Size();
            tabcontrol = tabcontrol==null?VisualUtils.FindAncestor(this, typeof(TabControlExt)) as TabControlExt:tabcontrol;
         
            foreach (UIElement element in InternalChildren)
            {
                if (element.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                element.Measure(availableSize);
                totalWidth += element.DesiredSize.Width;

                if (element.DesiredSize.Height > maxHeight)
                {
                    maxHeight = element.DesiredSize.Height;
                }
            }

            desiredSize.Width = totalWidth;
            desiredSize.Height = Math.Max(maxHeight, availableSize.Height);
            return desiredSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double currentWidth = 0;

            foreach (UIElement element in InternalChildren)
            {
                element.Arrange(new Rect(currentWidth, 0, element.DesiredSize.Width, finalSize.Height));
                currentWidth += element.DesiredSize.Width;
            }

            return finalSize;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Invokes scrolling.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessScrollingButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ScrollDirection scrollDirection = (ScrollDirection)Enum.Parse(typeof(ScrollDirection), button.Tag.ToString());            
            LayoutPanel.ProcessScrollInternal(scrollDirection);
        }

        /// <summary>
        /// Scroll buttons initialization.
        /// </summary>
        private void InitButtons()
        {
            ResourceDictionary dictionary = new ResourceDictionary
                                                {
                                                    Source = new Uri(
                                                        "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/CommonResources/TabScrollingButton.xaml",
                                                        UriKind.RelativeOrAbsolute)
                                                };
            if (parentPanel != null)
            {
                tabcontrol = parentPanel.GetTabControl();
            }          
          
           string skin="Default";
            Style style=null;
            style = (Style)dictionary["TabScrollingButton"];
           if (tabcontrol != null)
           {
               skin = SkinStorage.GetVisualStyle(tabcontrol);
           }
           if (skin == "Office2007Blue")
           {
               style = (Style)dictionary["Office2007BlueTabScrollingButton"];
           }
           if (skin == "Office2007Black")
           {
               style = (Style)dictionary["Office2007BlackTabScrollingButton"];
           }
           if (skin == "Office2007Silver")
           {
               style = (Style)dictionary["Office2007SilverTabScrollingButton"];
           }
           if (skin == "Office2003")
           {
               style = (Style)dictionary["Office2003TabScrollingButton"];
           }
           if (skin == "ShinyRed")
           {
               style = (Style)dictionary["ShinyRedTabScrollingButton"];
           }
           if (skin == "ShinyBlue")
           {
               style = (Style)dictionary["ShinyBlueTabScrollingButton"];
           }
           if (skin == "Office2010Blue")
           {
               style = (Style)dictionary["Office2010BlueTabScrollingButton"];

               if (parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.ExcelBlue)
               {
                   style = (Style)dictionary["ExcelBlueTabScrollingButton"];
                   this.Background = new SolidColorBrush(Color.FromArgb(255, 199, 217, 237));
               }
           }
           if (skin == "Office2010Black")
           {
               style = (Style)dictionary["Office2010BlackTabScrollingButton"];

               if (parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.ExcelBlack)
               {
                   style = (Style)dictionary["ExcelBlackTabScrollingButton"];
                   this.Background = new SolidColorBrush(Color.FromArgb(255, 106, 106, 106));
               }
           }
           if (skin == "Office2010Silver")
           {
               style = (Style)dictionary["Office2010SilverTabScrollingButton"];

               if (parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.ExcelSilver)
               {
                   style = (Style)dictionary["ExcelSilverTabScrollingButton"];
                   this.Background = new SolidColorBrush(Color.FromArgb(255, 220, 226, 232));
               }
           }
           if (skin == "SyncOrange")
           {
               style = (Style)dictionary["OrangeTabScrollingButton"];
           }
           else if (skin == "Blend")
           {
               style = (Style)dictionary["BlendTabScrollingButton"];
           }
           else if (skin == "Metro")
           {
               style = (Style)dictionary["MetroTabScrollingButton"];
           }
           else if(skin=="Default")
           {
               style = (Style)dictionary["TabScrollingButton"];
               if (parentPanel.GetTabControl() != null)
               {
                   if (parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.ExcelBlue)
                   {
                       style = (Style) dictionary["ExcelBlueTabScrollingButton"];
                       this.Background = new SolidColorBrush(Color.FromArgb(255, 199, 217, 237));
                   }
                   else if (parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.ExcelBlack)
                   {
                       style = (Style) dictionary["ExcelBlackTabScrollingButton"];
                       this.Background = new SolidColorBrush(Color.FromArgb(255, 106, 106, 106));
                   }
                   else if (parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.ExcelSilver)
                   {
                       style = (Style) dictionary["ExcelSilverTabScrollingButton"];
                       this.Background = new SolidColorBrush(Color.FromArgb(255, 220, 226, 232));
                   }
               }
           }
           else if (skin == "VS2010")
           {
               style = (Style)dictionary["VS2010TabScrollingButton"];
           }
          
           if (tabcontrol !=null &&tabcontrol.ScrollingButtonStyle != null)
               style = tabcontrol.ScrollingButtonStyle;
            m_prevTab = new Button();
            m_nextTab = new Button();
            if (parentPanel.GetTabControl()!=null && parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                m_prevPage = new Button();
                m_nextPage = new Button();
            }
            m_firstTab = new Button();
            m_lastTab = new Button();
            m_prevTab.Tag = "PrevTab";
            m_nextTab.Tag = "NextTab";
            if (parentPanel.GetTabControl()!=null && parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                m_prevPage.Tag = "PrevPage";
                m_nextPage.Tag = "NextPage";
            }
            m_firstTab.Tag = "FirstTab";
            m_lastTab.Tag = "LastTab";
            m_prevTab.Style = style;
            m_nextTab.Style = style;
            if (parentPanel.GetTabControl()!=null && parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                m_prevPage.Style = style;
                m_nextPage.Style = style;
            }
            m_firstTab.Style = style;
            m_lastTab.Style = style;            
            InternalChildren.Add(m_firstTab);
            if (parentPanel.GetTabControl()!=null && parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                InternalChildren.Add(m_prevPage);
                InternalChildren.Add(m_nextPage);
            }
            InternalChildren.Add(m_prevTab);
            InternalChildren.Add(m_nextTab);          
            InternalChildren.Add(m_lastTab);
            m_nextTab.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
            m_prevTab.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
            if (parentPanel.GetTabControl()!=null && parentPanel.GetTabControl().TabVisualStyle == TabVisualStyle.None)
            {
                m_nextPage.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
                m_prevPage.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
            }
            m_firstTab.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
            m_lastTab.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
        }

        /// <summary>
        /// Calls OnFlowDirectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFlowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Called when [coerce flow direction].
        /// </summary>
        /// <param name="d">Sender object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>Object value</returns>
        private static object OnCoerceFlowDirection(DependencyObject d, object baseValue)
        {
            ScrollingPanel instance = (ScrollingPanel)d;
            FlowDirection newValue = (FlowDirection)baseValue;
            if (instance.LayoutPanel.m_ParentTabControl != null)
            {
                Dock tabStripPlacement = instance.LayoutPanel.m_ParentTabControl.TabStripPlacement;

                if (tabStripPlacement == Dock.Bottom || tabStripPlacement == Dock.Left)
                {
                    return FlowDirection.RightToLeft;
                }
            }

            return newValue;
        }
        #endregion
    }
}