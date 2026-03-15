#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents scrolling panel.
    /// </summary>
    public class ScrollingPanel : Panel
    {
        #region Class constants
        /// <summary>
        /// Default panel height.
        /// </summary>
        private const int DefaultHeight = 18;
        #endregion

        #region Private members
        /// <summary>
        /// Button used for scrolling to the previous tab.
        /// </summary>
        private ScrollingButton prevTab;

        /// <summary>
        /// Button used for scrolling to the next tab.
        /// </summary>
        private ScrollingButton nextTab;

        /// <summary>
        /// Button used for scrolling to the previous page.
        /// </summary>
        private ScrollingButton prevPage;

        /// <summary>
        /// Button used for scrolling to the next page.
        /// </summary>
        private ScrollingButton nextPage;

        /// <summary>
        /// Button used for scrolling to the first tab.
        /// </summary>
        private ScrollingButton firstTab;

        /// <summary>
        /// Button used for scrolling to the last tab.
        /// </summary>
        private ScrollingButton lastTab;

        /// <summary>
        /// Tablayoutpanel parent.
        /// </summary>
        private TabLayoutPanel parent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets tablayoutpanel parent.
        /// </summary>
        internal TabLayoutPanel LayoutPanel
        {
            get
            {
                return parent;
            }

            set
            {
                parent = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the ScrollingPanel class.
        /// </summary>
        public ScrollingPanel()
        {
            Margin = new Thickness(1, 0, 0, 0);
            this.Loaded += new RoutedEventHandler(ScrollingPanelLoaded);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Shows scroll buttons when it is needed.
        /// </summary>
        public void Show()
        {
            this.InitButtons();
            this.UpdateButtons();

            if (LayoutPanel != null)
            {
                if (LayoutPanel.TabScrollStyle == TabScrollStyle.Extended)
                {
                    if (LayoutPanel.TabControlParent.TabVisualStyle == TabVisualStyle.None)
                    {
                        prevPage.Visibility = Visibility.Visible;
                        nextPage.Visibility = Visibility.Visible;
                    }
                    firstTab.Visibility = Visibility.Visible;
                    lastTab.Visibility = Visibility.Visible;
                    if (LayoutPanel.TabControlParent.TabVisualStyle == TabVisualStyle.None)
                    {
                        prevPage.UpdatePaths();
                        nextPage.UpdatePaths();
                    }
                    firstTab.UpdatePaths();
                    lastTab.UpdatePaths();
                }
                else
                {
                    prevPage.Visibility = Visibility.Collapsed;
                    nextPage.Visibility = Visibility.Collapsed;
                    firstTab.Visibility = Visibility.Collapsed;
                    lastTab.Visibility = Visibility.Collapsed;
                }
            }

            prevTab.Visibility = Visibility.Visible;
            nextTab.Visibility = Visibility.Visible;

            prevTab.UpdatePaths();
            nextTab.UpdatePaths();

            this.InvalidateMeasure();
        }

        /// <summary>
        /// Hides scroll buttons when it is needed.
        /// </summary>
        public void Hide()
        {
            this.InitButtons();
            this.UpdateButtons();

            if (LayoutPanel != null && LayoutPanel.TabScrollStyle == TabScrollStyle.Extended)
            {
                prevPage.Visibility = Visibility.Collapsed;
                nextPage.Visibility = Visibility.Collapsed;
                firstTab.Visibility = Visibility.Collapsed;
                lastTab.Visibility = Visibility.Collapsed;
            }

            prevTab.Visibility = Visibility.Collapsed;
            nextTab.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Disables Next part of scrolling buttons.
        /// </summary>
        public void DisableNextPart()
        {
            this.InitButtons();

            if (LayoutPanel.TabControlParent.TabVisualStyle == TabVisualStyle.None)
            {

                if (this.LayoutPanel != null && this.LayoutPanel.IsRightToLeft)
                {
                    prevTab.IsEnabled = false;
                    firstTab.IsEnabled = false;
                    prevPage.IsEnabled = false;
                }
                else
                {
                    nextTab.IsEnabled = false;
                    lastTab.IsEnabled = false;
                    nextPage.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Enables Next part of scrolling buttons.
        /// </summary>
        public void EnableNextPart()
        {
            this.InitButtons();
            if (LayoutPanel.TabControlParent.TabVisualStyle == TabVisualStyle.None)
            {
                if (this.LayoutPanel != null && this.LayoutPanel.IsRightToLeft)
                {
                    prevTab.IsEnabled = true;
                    firstTab.IsEnabled = true;
                    prevPage.IsEnabled = true;
                }
                else
                {
                    nextTab.IsEnabled = true;
                    lastTab.IsEnabled = true;
                    nextPage.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Disables Prev part of scrolling buttons.
        /// </summary>
        public void DisablePrevPart()
        {
            this.InitButtons();

            if (LayoutPanel.TabControlParent.TabVisualStyle == TabVisualStyle.None)
            {

                if (this.LayoutPanel != null && this.LayoutPanel.IsRightToLeft)
                {
                    nextTab.IsEnabled = false;
                    lastTab.IsEnabled = false;
                    nextPage.IsEnabled = false;
                }
                else
                {
                    prevTab.IsEnabled = false;
                    firstTab.IsEnabled = false;
                    prevPage.IsEnabled = false;
                }
            }           
        }

        /// <summary>
        /// Enables Prev part of scrolling buttons.
        /// </summary>
        public void EnablePrevPart()
        {
            this.InitButtons();

            if (LayoutPanel.TabControlParent.TabVisualStyle == TabVisualStyle.None)
            {

                if (this.LayoutPanel != null && this.LayoutPanel.IsRightToLeft)
                {
                    nextTab.IsEnabled = true;
                    lastTab.IsEnabled = true;
                    nextPage.IsEnabled = true;
                }
                else
                {
                    prevTab.IsEnabled = true;
                    firstTab.IsEnabled = true;
                    prevPage.IsEnabled = true;
                }
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double totalWidth = 0;
            double maxHeight = 0;
            Size desiredSize = new Size();
            foreach (UIElement element in this.Children)
            {
                if (element.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                availableSize.Width = double.PositiveInfinity;
                element.Measure(availableSize);
                totalWidth += element.DesiredSize.Width;

                if (element.DesiredSize.Height > maxHeight)
                {
                    maxHeight = element.DesiredSize.Height;
                }
            }

            desiredSize.Width = totalWidth;
            desiredSize.Height = Math.Max(maxHeight, DefaultHeight);
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

            foreach (UIElement element in this.Children)
            {
                element.Arrange(new Rect(currentWidth, 0, element.DesiredSize.Width, finalSize.Height));
                currentWidth += element.DesiredSize.Width;
            }

            return finalSize;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Rotates scrolling buttons with specified angle.
        /// </summary>
        /// <param name="angle">Angle to rotate the buttons.</param>
        internal void RotateButtons(double angle)
        {
            this.InitButtons();
            prevTab.RotateButton(angle);
            nextTab.RotateButton(angle);          
            prevPage.RotateButton(angle);
            nextPage.RotateButton(angle);           
            firstTab.RotateButton(angle);
            lastTab.RotateButton(angle);
        }

        /// <summary>
        /// Invokes scrolling.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ProcessScrollingButtonClick(object sender, RoutedEventArgs e)
        {
            ScrollingButton button = (ScrollingButton)sender;
            if (this.LayoutPanel != null)
            {
                this.LayoutPanel.ProcessScrollInternal(button.ScrollDirection);
            }
        }

        /// <summary>
        /// Scroll buttons initialization.
        /// </summary>
        private void InitButtons()
        {
            prevTab = this.FindName("PrevTab") as ScrollingButton;
            prevTab.Click -= new RoutedEventHandler(ProcessScrollingButtonClick);
            prevTab.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
            nextTab = this.FindName("NextTab") as ScrollingButton;
            nextTab.Click -= new RoutedEventHandler(ProcessScrollingButtonClick);
            nextTab.Click += new RoutedEventHandler(ProcessScrollingButtonClick);          
            prevPage = this.FindName("PrevPage") as ScrollingButton;
            prevPage.Click -= new RoutedEventHandler(ProcessScrollingButtonClick);
            prevPage.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
            nextPage = this.FindName("NextPage") as ScrollingButton;
            nextPage.Click -= new RoutedEventHandler(ProcessScrollingButtonClick);
            nextPage.Click += new RoutedEventHandler(ProcessScrollingButtonClick);           
            firstTab = this.FindName("FirstTab") as ScrollingButton;
            firstTab.Click -= new RoutedEventHandler(ProcessScrollingButtonClick);
            firstTab.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
            lastTab = this.FindName("LastTab") as ScrollingButton;
            lastTab.Click -= new RoutedEventHandler(ProcessScrollingButtonClick);
            lastTab.Click += new RoutedEventHandler(ProcessScrollingButtonClick);
        }

        /// <summary>
        /// Updates scrolling buttons.
        /// </summary>
        private void UpdateButtons()
        {
            prevTab.UpdatePaths();
            nextTab.UpdatePaths();           
            prevPage.UpdatePaths();
            nextPage.UpdatePaths();            
            firstTab.UpdatePaths();
            lastTab.UpdatePaths();
        }

        /// <summary>
        /// Occurs when a System.Windows.FrameworkElement has completed layout passes,
        /// has rendered, and is ready for interaction.
        /// </summary>
        private void ScrollingPanelLoaded(object sender, RoutedEventArgs e)
        {
            this.InitButtons();
        }
        #endregion
    }
}