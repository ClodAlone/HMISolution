// <copyright file="SuggestionBox.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !Silverlight4
using System.Threading.Tasks;
#endif

#if WPF

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
namespace Syncfusion.Windows.Controls.Input

#elif WINDOWS_PHONE || WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
namespace Syncfusion.Tools.Controls.Input
#elif WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    ///  Represent a popup that displayed when the matching <see
    /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Suggestions"/> found.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class SuggestionBox : ListBox
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SuggestionBox"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SuggestionBox()
        {
#if WPFSILVERLIGHT
            this.MouseLeftButtonUp += SuggestionBox_MouseLeftButtonUp;
#else            
            this.SelectionChanged += SuggestionBox_SelectionChanged;
#endif
        }

       
#if WPFSILVERLIGHT
        void SuggestionBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (autoComplete != null && autoComplete.IsSuggestionOpen)
                autoComplete.IsSuggestionOpen = false;
        }
        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter)
            {
                if (autoComplete != null && autoComplete.IsSuggestionOpen)
                    autoComplete.IsSuggestionOpen = false;
            }
        }

#endif

        #endregion

        #region Variables

        internal SfTextBoxExt autoComplete;

        internal ScrollViewer scrollViewer=null;

        internal bool isScrollViewer=false;

        #endregion

        #region Helper Methods

#if ! WPFSILVERLIGHT
        private ScrollViewer FindScrollViewer(UIElement rootElement)
        {
            ScrollViewer result = null;
            if (!isScrollViewer)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(rootElement); i++)
                {
                    UIElement child = VisualTreeHelper.GetChild(rootElement, i) as UIElement;
                    if (child is ScrollViewer)
                    {
                        result = child as ScrollViewer;
                        isScrollViewer = true;
                        break;
                    }
                    else if (child != null)
                    {
                        if (!isScrollViewer)
                            result = FindScrollViewer(child as UIElement);
                    }
                }
            }
            return result;
        }


        void SuggestionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = null;
            if (SelectedItem != null)
            {
                if (SelectedItem is ListBoxItem)
                    item = SelectedItem as ListBoxItem;
                else
                    item = this.ItemContainerGenerator.ContainerFromItem(SelectedItem) as ListBoxItem;
            }
            if (item != null)
            {
                GeneralTransform transform = item.TransformToVisual(this);
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                Point relativePoint = transform.Transform(new Point(0, 0));
#else
                Point relativePoint = transform.TransformPoint(new Point(0, 0));
#endif
                Point popUpPosition = new Point(0, 0);

                if (autoComplete != null && item != null) 
                {
                    if (autoComplete.DataContext == null)
                    {
                        autoComplete.SelectedItem = item;
                    }
                    else
                    {
                        autoComplete.SelectedItem = item.DataContext;
                    }
                    GeneralTransform popUptransform = autoComplete.PART_SuggestionBox.TransformToVisual(this);
#if WPFSILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    popUpPosition = popUptransform.Transform(new Point(0, 0));
#else
                    popUpPosition = popUptransform.TransformPoint(new Point(0, 0));
#endif
                    if ((relativePoint.Y >= popUpPosition.Y && relativePoint.Y+item.ActualHeight > ActualHeight) || relativePoint.Y <= popUpPosition.Y)
                    {
                        if (scrollViewer == null)
                        {
                            scrollViewer = FindScrollViewer(this);
                        }
                        if (scrollViewer != null)
                        {
                            if (relativePoint.Y <= popUpPosition.Y)
                            {
                                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 1);
                            }
                            else
                            {
                                if (relativePoint.Y+item.ActualHeight <= scrollViewer.ScrollableHeight)
                                {
                                    double diff = 0.0;
                                    if(relativePoint.Y > this.ActualHeight)
                                    diff = Math.Abs(relativePoint.Y - this.ActualHeight);
                                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 1 + diff);
                                }
                                else
                                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 1);
                            }
                        }
                        isScrollViewer = false;
                    }
                }
            }

        }
#endif
        #endregion
    }
}
