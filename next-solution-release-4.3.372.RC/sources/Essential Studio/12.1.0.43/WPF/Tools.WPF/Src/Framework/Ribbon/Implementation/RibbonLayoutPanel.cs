// <copyright file="RibbonLayoutPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Syncfusion.Windows.Shared;
using System.Linq;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class of utility type, which is responsible for layout.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonLayoutPanel : Panel,IDisposable
    {
        #region Constants
        /// <summary>
        /// Represents the scroll tick
        /// </summary>
        private int c_ScrollTick = 30;

        /// <summary>
        /// Represents the scroll width
        /// </summary>
        private int c_ScrollWidth = 10;

        /// <summary>
        /// Represents the previous constraint
        /// </summary>
        private double m_previousConstraint = 0;
        #endregion

        #region Private members
        /// <summary>
        /// Number of remeasured elements.
        /// </summary>
        private int m_remeasuredElements = 0;

        /// <summary>
        /// Represents the Left Button
        /// </summary>
        private Button m_leftButton = null;

        /// <summary>
        /// Represents the Right Button
        /// </summary>
        private Button m_rightButton = null;

        /// <summary>
        /// Represents the Left Offset
        /// </summary>
        private double m_leftOffset = 0;

        /// <summary>
        /// Represents the needed space
        /// </summary>
        private double m_neededSpace = 0;

        /// <summary>
        /// Represents the available size
        /// </summary>
        private double m_availableSize = 0;

        /// <summary>
        /// Represents the scroll timer
        /// </summary>
        private DispatcherTimer m_scrollTimer = new DispatcherTimer();

        /// <summary>
        /// Represents the scroll
        /// </summary>
        private double m_needToScroll = 0;

        /// <summary>
        /// Represents the expand flag
        /// </summary>
        private bool isexpand = false;
        /// <summary>
        /// Represents the parent Ribbon.
        /// </summary>
        private Ribbon m_parentRibbon
        {
            get
            {
                return VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            }
        }

        /// <summary>
        /// Gets the m_parent window.
        /// </summary>
        /// <value>The m_parent window.</value>
        internal Window m_parentWindow
        {
            get
            {
                return VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            }
        }

        /// <summary>
        /// Mins the width of the gallery.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <returns></returns>
        private double MinGalleryWidth(RibbonGallery gallery)
        {
            if (gallery != null)
            {
                if (gallery.Items.Count > 0)
                {
                    FrameworkElement child = gallery.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
                    return child.ActualWidth * 3;
                }
                return 70;
            }
            return 0;
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonLayoutPanel"/> class.
        /// </summary>
        public RibbonLayoutPanel()
        {
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Inits the buttons.
        /// </summary>
        private void InitButtons()
        {
            Style style = GetCorrespondingScrollingButtonStyle();
            m_leftButton = new Button();
            m_rightButton = new Button();
            m_leftButton.Tag = "Left";
            m_rightButton.Tag = "Right";
            m_leftButton.Style = style;
            m_rightButton.Style = style;
            m_leftButton.Visibility = Visibility.Hidden;
            m_rightButton.Visibility = Visibility.Hidden;
            this.Unloaded += new RoutedEventHandler(RibbonLayoutPanel_Unloaded);
            m_leftButton.Click -= new RoutedEventHandler(LeftButton_Click);
            m_rightButton.Click -= new RoutedEventHandler(RightButton_Click);
            m_leftButton.Click += new RoutedEventHandler(LeftButton_Click);
            m_rightButton.Click += new RoutedEventHandler(RightButton_Click);
            AddLogicalChild(m_leftButton);
            AddVisualChild(m_leftButton);
            AddLogicalChild(m_rightButton);
            AddVisualChild(m_rightButton);
        }

        void RibbonLayoutPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose(); 
        }

        private Style GetCorrespondingScrollingButtonStyle()
        {
            ResourceDictionary dictionary = new ResourceDictionary();

            Style ScrollingButtonStyle = null;

            if (SkinStorage.GetVisualStyle(this) == "Blend")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2003")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "VS2010")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Metro")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Transparent")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2013")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2013Style.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else if (SkinStorage.GetVisualStyle(this) == "Windows8")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Windows8Style.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }
            else
            {
                dictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                ScrollingButtonStyle = dictionary["RibbonScrollingButton"] as Style;
            }

            return ScrollingButtonStyle;
        }

        /// <summary>
        /// Handles the Click event of the RightButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_neededSpace > m_availableSize)
            {
                m_needToScroll -= m_availableSize;
                m_neededSpace -= m_availableSize;
            }
            else
            {
                m_needToScroll -= m_neededSpace;
                m_neededSpace = 0;
            }

            m_scrollTimer.Stop();
            m_scrollTimer = new DispatcherTimer();
            m_scrollTimer.Tick -= new EventHandler(RightScrollTick);
            m_scrollTimer.Tick += new EventHandler(RightScrollTick);
            m_scrollTimer.Interval = TimeSpan.FromMilliseconds(c_ScrollTick);
            m_scrollTimer.Start();
        }

        /// <summary>
        /// Handles the Click event of the LeftButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_leftOffset <= -m_availableSize)
            {
                m_neededSpace += m_availableSize;
                m_needToScroll = m_availableSize;
            }
            else
            {
                m_neededSpace += Math.Abs(m_leftOffset);
                m_needToScroll = Math.Abs(m_leftOffset);
            }

            m_scrollTimer.Stop();
            m_scrollTimer = new DispatcherTimer();
            m_scrollTimer.Tick += new EventHandler(LeftScrollTick);
            m_scrollTimer.Interval = TimeSpan.FromMilliseconds(c_ScrollTick);
            m_scrollTimer.Start();
        }

        /// <summary>
        /// Lefts the scroll tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LeftScrollTick(object sender, EventArgs e)
        {
            if (m_needToScroll > 0 && m_needToScroll > c_ScrollWidth)
            {
                m_leftOffset += c_ScrollWidth;
                m_needToScroll -= c_ScrollWidth;

                InvalidateArrange();
            }
            else
            {
                m_leftOffset += m_needToScroll;
                m_needToScroll = 0;
                CheckLeftButtonVisibility();
                CheckRightButtonVisibility();
                InvalidateArrange();
                m_scrollTimer.Stop();
            }
        }

        /// <summary>
        /// Rights the scroll tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RightScrollTick(object sender, EventArgs e)
        {
            if (m_needToScroll < 0 && Math.Abs(m_needToScroll) > c_ScrollWidth)
            {
                m_leftOffset -= c_ScrollWidth;
                m_needToScroll += c_ScrollWidth;

                InvalidateArrange();
            }
            else
            {
                m_leftOffset -= Math.Abs(m_needToScroll);
                m_needToScroll = 0;
                CheckLeftButtonVisibility();
                CheckRightButtonVisibility();
                InvalidateArrange();
                m_scrollTimer.Stop();
            }
        }

        /// <summary>
        /// Checks the left button visibility.
        /// </summary>
        private void CheckLeftButtonVisibility()
        {
            if (m_leftButton != null)
            {
                m_leftButton.Visibility = (m_leftOffset == 0) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        /// <summary>
        /// Checks the right button visibility.
        /// </summary>
        private void CheckRightButtonVisibility()
        {
            if (m_rightButton != null)
            {
                m_rightButton.Visibility = m_neededSpace > 0 ? Visibility.Visible : Visibility.Hidden;
            }
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when the <see cref="T:System.Windows.Media.VisualCollection"/> of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The <see cref="T:System.Windows.Media.Visual"/> that was added to the collection.</param>
        /// <param name="visualRemoved">The <see cref="T:System.Windows.Media.Visual"/> that was removed from the collection.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            m_leftOffset = 0;
            m_neededSpace = 0;
            m_needToScroll = 0;
            CheckLeftButtonVisibility();
            CheckRightButtonVisibility();
        }

        /// <summary>
        /// Gets the number of child System.Windows.Media.Visual objects in this instance of System.Windows.Controls.Panel.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                int count = base.VisualChildrenCount;
                return (count >= 1) ? (count + 2) : count;
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Windows.Media.Visual"/> child of this <see cref="T:System.Windows.Controls.Panel"/> at the specified index position.
        /// </summary>
        /// <param name="index">The index position of the <see cref="T:System.Windows.Media.Visual"/> child.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.Visual"/> child of the parent <see cref="T:System.Windows.Controls.Panel"/> element.
        /// </returns>
        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            if (index >= VisualChildrenCount - 2)
            {
                if (index == VisualChildrenCount - 1)
                {
                    return m_leftButton;
                }
                else
                {
                    return m_rightButton;
                }
            }
            else
            {
                return base.GetVisualChild(index);
            }
        }

        /// <summary>
        /// Positions child elements.
        /// </summary>
        /// <param name="arrangeSize">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override System.Windows.Size ArrangeOverride(Size arrangeSize)
        {
            int count = Children.Count;
            Rect finalRect = new Rect(arrangeSize);
            double width = m_leftOffset;
            int i = 0;

            while (i < count)
            {
                UIElement element = GetVisualChild(i) as UIElement;
                if (element != null)
                {
                    finalRect.X += width;
                    width = element.DesiredSize.Width;
                    finalRect.Width = width;
                    finalRect.Height = Math.Max(arrangeSize.Height, element.DesiredSize.Height);
                    element.Arrange(finalRect);
                }

                i++;
            }

            finalRect.X = 0;
            finalRect.Width = m_leftButton.DesiredSize.Width;
            m_leftButton.Arrange(finalRect);

            finalRect.Width = m_rightButton.DesiredSize.Width;
            finalRect.X = m_availableSize - m_rightButton.DesiredSize.Width;
            m_rightButton.Arrange(finalRect);

            m_remeasuredElements = 0;
            return arrangeSize;
        }

        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size.
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
        protected override System.Windows.Size MeasureOverride(Size constraint)
        {
            Size size = new Size();
            //executed = false;
            if (m_leftButton == null)
            {
                InitButtons();
            }

            m_leftButton.Measure(constraint);
            m_rightButton.Measure(constraint);

            int count = MeasureAll(constraint, ref size);

            while (size.Width > constraint.Width && m_remeasuredElements < count)
            {
                m_remeasuredElements++;
                size.Width = 0;
                if (!double.IsInfinity(constraint.Width))
                {
                    if (m_parentRibbon != null && !Ribbon.GetIsAutoSizeFormEnabled(m_parentRibbon))
                    {
                        MeasureAll(constraint, ref size);
                    }
                }
            }

            m_availableSize = constraint.Width;
            return size;
        }

        /// <summary>
        /// Draws the content of a DrawingContext object during the
        /// render pass of a Panel element.
        /// </summary>
        /// <param name="dc">The DrawingContext object to draw.</param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbon != null)
            {
                ribbon.RenderKeyTips();
            }
        }
        bool ismax = false;
        /// <summary>
        /// Measures all.
        /// </summary>
        /// <param name="constraint">The constraint size.</param>
        /// <param name="size">The size value.</param>
        /// <returns>returns Measures</returns>
        private int MeasureAll(Size constraint, ref Size size)
        {
            Size availableSize = constraint;
            Size desiredSize = new Size();
            availableSize.Width = double.PositiveInfinity;
            int i = 0;
            int count = Children.Count;
            if (m_parentWindow != null)
            {
                if (Ribbon.GetIsAutoSizeFormEnabled(m_parentRibbon))
                {
                    if (m_parentWindow.WindowState == WindowState.Maximized)
                    {
                       // ResetToDefault();
                        ismax = true;
                    }
                    else
                    {
                        if (ismax)
                        {
                            //TryToCollapse(InternalChildren.Count - 1, InternalChildren[InternalChildren.Count - 1] as RibbonBar, constraint, constraint);
                            CheckForCollapse(constraint);
                            ismax = false;
                        }
                    }
                }
            }
            while (i < count)
            {
                RibbonBar element = GetVisualChild(i) as RibbonBar;

                if (element != null)
                {
                    double galleryItemsWidth = 0;
                    foreach (UIElement bar in InternalChildren)
                    {
                        if (bar is RibbonBar)
                        {
                            RibbonBar ribbonbar = (RibbonBar)bar;
                            if (!ribbonbar.IsRibbonGalleryPresent)
                            {
                                galleryItemsWidth += bar.DesiredSize.Width;
                            }
                        }
                    }

                    #region Ribbon gallery
                    if (element.IsRibbonGalleryPresent)
                    {
                        double value = constraint.Width - galleryItemsWidth;
                        RibbonGallery gallery = null;

                        if (value >= 300)
                        {
                            double otherItemsWidth = 0;
                            double maxGalleryWidth = 0;
                            if (element.ItemsSource == null)
                            {
                                foreach (ICollapsable barElementItems in element.Items)
                                {
                                    if ((barElementItems is RibbonGallery) && (barElementItems as RibbonGallery).VisualMode == RibbonGalleryVisualMode.InRibbon)
                                    {
                                        gallery = barElementItems as RibbonGallery;


                                    }
                                    else if ((barElementItems.SizeForm != SizeForm.Large && otherItemsWidth <= 0) || barElementItems.SizeForm == SizeForm.Large)
                                    {
                                        galleryItemsWidth += (barElementItems as FrameworkElement).DesiredSize.Width;
                                        otherItemsWidth += (barElementItems as FrameworkElement).DesiredSize.Width;
                                    }
                                }
                            }
                            if (gallery != null)
                            {
                                maxGalleryWidth = gallery.ItemWidth * gallery.Items.Count;
                                InRibbonItemsPresenter itemsPresenter = gallery.Template.FindName("PART_ScrollPresenter", gallery) as InRibbonItemsPresenter;
                                if (itemsPresenter != null && !double.IsInfinity(constraint.Width))
                                {
                                    WrapPanel child = itemsPresenter.GetWrapPabel();
                                    itemsPresenter.MaxWidth = maxGalleryWidth;

                                    if (maxGalleryWidth != 0)
                                    {
                                        itemsPresenter.Width = constraint.Width - galleryItemsWidth - 50;
                                        if (CheckforCollpase(element))
                                        {
                                            element.Width = itemsPresenter.Width + 30 + otherItemsWidth;
                                        }
                                    }
                                    if (child != null)
                                    {
                                        double width = 0;
                                        foreach (RibbonGalleryItem item in child.Children)
                                        {
                                            width += item.RenderSize.Width;

                                            if (width > itemsPresenter.Width)
                                            {
                                                itemsPresenter.Width = width - item.RenderSize.Width;
                                                if (CheckforCollpase(element))
                                                {
                                                    element.Width = itemsPresenter.Width + 30 + otherItemsWidth;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!isexpand)
                            {
                                element.Width = Math.Abs(value);
                                isexpand = true;
                            }
                            
                            if (element.PanelState == RibbonBarState.Collapsed)
                            {
                                if (isexpand)
                                {
                                    element.Width = double.NaN;
                                }
                            }

							var galleries = element.Items.OfType<RibbonGallery>().Where(g => g.VisualMode == RibbonGalleryVisualMode.InRibbon);

							foreach (RibbonGallery g in galleries)
							{
								double maxWidth = g.ItemWidth * g.Items.Count;
								InRibbonItemsPresenter itemsPresenter = g.Template.FindName("PART_ScrollPresenter", g) as InRibbonItemsPresenter;
								if (itemsPresenter != null)
								{
                                     double var = constraint.Width - galleryItemsWidth - 50;
									itemsPresenter.MaxWidth = maxWidth;
                                    if (maxWidth != 0)
                                    {
                                        double elementwidth = 0.0,elementHeight=0.0;
                                        foreach (FrameworkElement item in element.Items)
                                        {
                                            elementwidth += item.DesiredSize.Width;
                                            elementHeight = Math.Max(item.DesiredSize.Height, elementHeight);
                                        }
                                        if (elementwidth > SystemParameters.PrimaryScreenWidth - constraint.Width)
                                            itemsPresenter.Width = SystemParameters.PrimaryScreenWidth - constraint.Width;
                                        else
                                            itemsPresenter.Width = elementwidth;
                                    }
								}
                                double elementswidth=0.0;
                                foreach (FrameworkElement item in element.Items)
                                {
                                   
                                    if (item != g)
                                    {
                                        elementswidth += item.ActualWidth;
                                    }
                                }
                                if (itemsPresenter != null && itemsPresenter.Width + elementswidth > element.Width)
                                {
                                    double var=element.ActualWidth - (elementswidth + 30);
                                    if(var>0)
                                        itemsPresenter.Width =var ;
                                }
							}
                        }
                    }
                    #endregion

                    element.Measure(availableSize);
                    if (element.PanelState == RibbonBarState.Collapsed)
                    {
                        if (size.Width + element.m_DesiredSize.Width <= constraint.Width)
                        {
                            desiredSize = MeasureTwoRow(i, constraint);
                        }
                        else
                        {
                            desiredSize = element.DesiredSize;
                        }
                    }
                    else
                    {
                        desiredSize = element.DesiredSize;
                    }
                    size.Width += desiredSize.Width;
                    size.Height = Math.Max(size.Height, desiredSize.Height);

                    if (m_parentRibbon != null &&Ribbon.GetIsAutoSizeFormEnabled (m_parentRibbon ))
                    {
                        if (m_parentRibbon.isloaded)
                        {
                            if (m_previousConstraint != 0)
                            {
                                //Expanding the ribbon panel
                                if (m_previousConstraint < constraint.Width)
                                {
                                    ResetItems();
                                }
                                else
                                {
                                   
                                    //Collpasing the ribbon panel
                                    if (size.Width > constraint.Width)
                                    {
                                        ResizeChildControl(size, constraint);
                                    }
                                    else
                                    {
                                        if (i >= count - 1)
                                        {
                                            if (Math.Abs(m_leftOffset) + m_neededSpace + size.Width <= constraint.Width)
                                            {
                                                m_leftOffset = 0;
                                                CheckLeftButtonVisibility();

                                                var bars = from FrameworkElement bar in InternalChildren
                                                           where bar is RibbonBar
                                                           select bar;

                                                Size actualsize = new Size();

                                                foreach (RibbonBar ele in bars)
                                                {
                                                    actualsize.Width += ele.DesiredSize.Width;
                                                }
                                                if (actualsize.Width - constraint.Width > 0)
                                                    m_rightButton.Visibility = Visibility.Visible;
                                                else
                                                    m_rightButton.Visibility = Visibility.Collapsed;

                                                InvalidateArrange();
                                            }
                                            m_neededSpace = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (size.Width > constraint.Width)
                        {
                            if (constraint.Width > ActualWidth)
                                TryToCollapse(i, element, size, new Size(ActualWidth, ActualHeight));
                            else
                                TryToCollapse(i, element, size, constraint);
                        }
                        else
                        {
                            if (i >= count - 1)
                            {
                                
                                    m_leftOffset = 0;
                                    CheckLeftButtonVisibility();
                                    CheckRightButtonVisibility();
                                    InvalidateArrange();
                                

                                m_neededSpace = 0;
                                IsRightButtonVisible = false;
                            }
                        }
                    }
                    m_previousConstraint = constraint.Width;
                }
                else
                {
                    UIElement el = GetVisualChild(i) as UIElement;

                    if (el != null)
                    {
                        el.Measure(availableSize);
                        size.Width += el.DesiredSize.Width;
                    }
                }

                i++;
            }
            return count;
        }



        internal void ResetRemoveItems()
        {

            var query = from RibbonBar bar in InternalChildren
                        where bar.PanelState == RibbonBarState.Collapsed
                        select bar;
         
            if (query.Count() == 0)
            {

                foreach (RibbonBar item in InternalChildren)
                {
                    if (item.MeasureMode == MeasureMode.Compressed)
                    {
                        ResetElement(item);
                    }
                }

            }


        }

        internal void ResetItems()
        {
            var query = from RibbonBar bar in InternalChildren
                        where bar.PanelState == RibbonBarState.Collapsed
                        select bar;
        
            if (query.Count() == 0)
            {
                
                    foreach (RibbonBar item in InternalChildren)
                    {
                        if (item.MeasureMode == MeasureMode.Compressed)
                        {
                            if (ResetElement(item))
                            {
                                break;
                            }
                        }
                    }
                 
            }
        }

        internal void CheckForCollapse()
        {
            var bars = from FrameworkElement bar in InternalChildren
                       where bar is RibbonBar
                       select bar;

            Size actualsize = new Size();

            foreach (RibbonBar element in bars)
            {
                actualsize.Width += element.DesiredSize.Width;
            }

            int i = InternalChildren.Count;
            bool nxt = false;
            
            while (actualsize.Width > this.RenderSize.Width)
            {
                i--;
                RibbonBar bar = InternalChildren[i] as RibbonBar;
                if (bar != null)
                {
                    m_previousConstraint = 0;
                    if (nxt)
                        bar.CompressSmallItems();
                    else
                        bar.CompressLargeItems();
                    bar.UpdateLayout();
                }
                
                actualsize.Width = 0;
                
                foreach (RibbonBar element in bars)
                {
                    actualsize.Width += element.ActualWidth;
                }
                if (CheckIfAllCollapse())
                {
                    if (actualsize.Width - RenderSize.Width > 0)
                        m_rightButton.Visibility = Visibility.Visible;
                    else
                        m_rightButton.Visibility = Visibility.Collapsed;
                    break;
                }
                if (i == 0)
                {
                    i = InternalChildren.Count;
                    if (nxt)
                        break;
                    nxt = true;                    
                }
            }
            if (!ismax)
            {
                if (m_parentWindow != null)
                    m_parentWindow.Width -= 1;
            }
        }

        /// <summary>
        /// Checks for collapse.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        internal void CheckForCollapse(Size constraint)
        {
            var bars = from FrameworkElement bar in InternalChildren
                       where bar is RibbonBar
                       select bar;

            Size actualsize = new Size();

            foreach (RibbonBar element in bars)
            {
                actualsize.Width += element.DesiredSize.Width;
            }

            int i = InternalChildren.Count;
            bool nxt = false;

            while (actualsize.Width > constraint.Width)
            {
                i--;
                RibbonBar bar = InternalChildren[i] as RibbonBar;

                if (bar != null)
                {
                    m_previousConstraint = 0;
                    if (nxt)
                        bar.CompressSmallItems();
                    else
                        bar.CompressLargeItems();
                    bar.UpdateLayout();
                }

                actualsize.Width = 0;

                foreach (RibbonBar element in bars)
                {
                    actualsize.Width += element.ActualWidth;
                }
                if (CheckIfAllCollapse())
                {
                    if (actualsize.Width - RenderSize.Width > 0)
                        m_rightButton.Visibility = Visibility.Visible;
                    else
                        m_rightButton.Visibility = Visibility.Collapsed;
                    break;
                }
                if (i == 0)
                {
                    i = InternalChildren.Count;
                    if (nxt)
                        break;
                    nxt = true;
                }
            }
            if (!ismax)
            {
                if (m_parentWindow != null)
                    m_parentWindow.Width -= 1;
            }
        }


        /// <summary>
        /// Checks if all collapse.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfAllCollapse()
        {
            var bars = from FrameworkElement bar in InternalChildren
                       where bar is RibbonBar
                       select bar;

            foreach (RibbonBar element in bars)
            {
                if (element.PanelState != RibbonBarState.Collapsed)
                    return false;
            }

            return true;

        }

        /// <summary>
        /// Checkfors the collpase.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private bool CheckforCollpase(RibbonBar element)
        {
            var bars = from FrameworkElement bar in InternalChildren
                       where bar is RibbonBar
                       select bar;

            foreach (RibbonBar item in bars)
            {
                if (item.PanelState == RibbonBarState.Collapsed)
                {
                    item.PanelState = RibbonBarState.TwoRow;
                    return false;
                }

                //if (item.MeasureMode == MeasureMode.Compressed)
                //{
                //    ResetElement(item);
                //    return false;
                //}
            }
            return true;
        }

        /// <summary>
        /// Reset the Ribbon bar to default state. Used while resizing.
        /// </summary>
        /// <param name="element">The Ribbon bar</param>
        /// <returns>Returns true if reset</returns>
        private bool ResetElement(RibbonBar element)
        {
            bool issmall = false, isextra = false;
            for (int i = 0; i < element.Items.Count; i++)
            {
                if (((FrameworkElement)element.Items[i]).Tag != null)
                {
                    ICollapsable col = element.Items[i] as ICollapsable;
                    if (col.SizeForm == SizeForm.Small)
                    {
                        col.SizeForm = SizeForm.Large;
                        ((FrameworkElement)col).Tag = null;
                        issmall = true;

                        if (col is RibbonButton)
                        {
                            RibbonButton button = col as RibbonButton;
                            if (button.CollapseLabel != null)
                                button.Label = button.tempLabel;
                        }

                        if (col is DropDownButton)
                        {
                            DropDownButton button = col as DropDownButton;
                            if (button.CollapseLabel != null)
                                button.Label = button.tempLabel;
                        }


                        if (col is SplitButton)
                        {
                            SplitButton button = col as SplitButton;
                            if (button.CollapseLabel != null)
                                button.Label = button.tempLabel;
                        }

                    }
                }
            }
            if (issmall)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < element.Items.Count; i++)
                {
                    if (((FrameworkElement)element.Items[i]).Tag != null)
                    {
                        ICollapsable col = element.Items[i] as ICollapsable;
                        if (col.SizeForm == SizeForm.ExtraSmall)
                        {
                            col.SizeForm = SizeForm.Small;
                            ((FrameworkElement)col).Tag = null;
                            isextra = true;

                            if (col is RibbonButton)
                            {
                                RibbonButton button = col as RibbonButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.CollapseLabel;
                            }
                            
                            if (col is DropDownButton)
                            {
                                DropDownButton button = col as DropDownButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.CollapseLabel;
                            }

                            if (col is SplitButton)
                            {
                                SplitButton button = col as SplitButton;
                                if (button.CollapseLabel != null)
                                    button.Label = button.CollapseLabel;
                            }
                        }
                    }
                }
            }
            if (isextra)
            {
                element.MeasureMode = MeasureMode.Default;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets to default.
        /// </summary>
        private void ResetToDefault()
        {
            foreach (RibbonBar bar in InternalChildren)
            {
                if (bar.MeasureMode == MeasureMode.Compressed)
                {
                    bar.SetToDefault();
                }
            }
        }

        /// <summary>
        /// Resize the Ribbon controls
        /// </summary>
        /// <param name="i">Index</param>
        /// <param name="element">RibbonBar</param>
        /// <param name="size">Available Size</param>
        /// <param name="constraint">Constraint size</param>
        private void ResizeChildControl(Size size, Size constraint)
        {
            int k = InternalChildren.Count - 1;
            bool collapsed = false, issmall = false, islarge = false;
            RibbonBar element;
            Size actualsize = new Size();

            foreach (RibbonBar elemen in InternalChildren)
            {
                actualsize.Width += elemen.DesiredSize.Width;
            }
            while (k >= 0)
            {
                if (InternalChildren[k] is RibbonBar)
                {
                    element = (RibbonBar)InternalChildren[k];
                    if (element.PanelState != RibbonBarState.Collapsed)
                    {
                        if (element.CompressSmallItems())
                        {
                            issmall = true;
                            break;
                        }
                        else
                        {
                            k--;
                            continue;
                        }
                    }

                    k--;
                }
            }

            if (!issmall)
            {
                k = InternalChildren.Count - 1;
                while (k >= 0)
                {
                    if (InternalChildren[k] is RibbonBar)
                    {
                        element = (RibbonBar)InternalChildren[k];
                        if (element.PanelState != RibbonBarState.Collapsed)
                        {
                            if (element.CompressLargeItems())
                            {
                                islarge = true;
                                break;
                            }
                            else
                            {
                                k--;
                                continue;
                            }
                        }

                        k--;
                    }
                }
            }

            if (!islarge && !issmall)
            {
                k = InternalChildren.Count - 1;
                while (k >= 0)
                {
                    if (InternalChildren[k] is RibbonBar)
                    {
                        element = (RibbonBar)InternalChildren[k];
                        if (element.PanelState != RibbonBarState.Collapsed)
                        {                           
                            element.PanelState = RibbonBarState.Collapsed;
                            element.m_DesiredSize = element.DesiredSize;
                            element.Measure(size);
                            break;
                        }
                        else
                        {
                            k--;
                            continue;
                        }
                    }
                    k--;
                }

               
            }

            if (!collapsed)
            {
                k = InternalChildren.Count - 1;
                while (k >= 0)
                {
                    if (InternalChildren[k] is RibbonBar)
                    {
                        element = (RibbonBar)InternalChildren[k];
                        if (element.PanelState != RibbonBarState.Collapsed)
                        {
                            collapsed = true;

                            break;
                        }
                        k--;

                    }
                    k--;
                }
                if (!collapsed)
                {
                    m_neededSpace = size.Width - constraint.Width + m_leftOffset;
                    CheckRightButtonVisibility();
                }
            }
        }

        /// <summary>
        /// Tries to collapse.
        /// </summary>
        /// <param name="i">The i param value.</param>
        /// <param name="element">The element param value.</param>
        /// <param name="size">The size param value.</param>
        /// <param name="constraint">The constraint.</param>
        private void TryToCollapse(int i, RibbonBar element, Size size, Size constraint)
        {
            if (element.PanelState != RibbonBarState.Collapsed)
            {
                element.PanelState = RibbonBarState.Collapsed;
                element.m_DesiredSize = element.DesiredSize;
                element.Measure(constraint);
            }
            else
            {
                int k = i - 1;
                bool collapsed = false;

                while (k >= 0 && !collapsed)
                {
                    //element = (RibbonBar)InternalChildren[k];
                    if (InternalChildren[k] is RibbonBar)
                    {
                        element = (RibbonBar)InternalChildren[k];
                        if (element.PanelState != RibbonBarState.Collapsed)
                        {
                            element.PanelState = RibbonBarState.Collapsed;
                            element.m_DesiredSize = element.DesiredSize;
                            element.Measure(size);
                            collapsed = true;
                        }
                    }

                    k--;
                }

                if (!collapsed)
                {
                    m_neededSpace = size.Width - constraint.Width + m_leftOffset;
                    CheckRightButtonVisibility();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is right button visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is right button visible; otherwise, <c>false</c>.
        /// </value>
        internal bool IsRightButtonVisible
        {
            get
            {
                return m_rightButton.Visibility == Visibility.Visible;
            }

            set
            {
                m_rightButton.Visibility = (value && m_neededSpace > 0) ? Visibility.Visible
                    : Visibility.Hidden;
            }
        }

        /// <summary>
        /// Measures the two row.
        /// </summary>
        /// <param name="i">The i param value.</param>
        /// <param name="availableSize">Size of the available.</param>
        /// <returns>row measures</returns>
        private Size MeasureTwoRow(int i, Size availableSize)
        {
            Size desiredSize = new Size();
            double summaryWidth = 0;
            RibbonBar ribbonBar = GetVisualChild(i) as RibbonBar;
            for (int k = 0; k < InternalChildren.Count; k++)
            {
                if (k == i)
                {
                    summaryWidth += ribbonBar.m_DesiredSize.Width;
                }
                else
                {
                    summaryWidth += (InternalChildren[k] as UIElement).DesiredSize.Width;
                }
            }
            
            if (summaryWidth <= availableSize.Width)
            {
                ribbonBar.PanelState = RibbonBarState.TwoRow;
                desiredSize = ribbonBar.m_DesiredSize;
                ribbonBar.m_DesiredSize = new Size(0, 0);
                ribbonBar.Measure(availableSize);
            }
            else
            {
                desiredSize = ribbonBar.DesiredSize;
            }

            return desiredSize;
        }
        #endregion

        public void Dispose()
        {
            if (m_leftButton != null)
                m_leftButton.Click -= new RoutedEventHandler(LeftButton_Click);
            if (m_rightButton != null)
                m_rightButton.Click -= new RoutedEventHandler(RightButton_Click);
            if (m_scrollTimer != null)
                m_scrollTimer.Tick += new EventHandler(RightScrollTick);
        }
    }
}
