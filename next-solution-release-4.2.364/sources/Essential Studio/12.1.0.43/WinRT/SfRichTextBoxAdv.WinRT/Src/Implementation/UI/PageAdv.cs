#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;
#if WPF
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Devices.Input;
using Windows.UI.Core;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class PageAdv : Canvas
    {
        #region Fields
        private Border CurrentPagePopup;
        private TextBlock CurrentPageBlock;
        private DispatcherTimer popupTimer = null;
        private SectionAdv section = null;
        internal HeaderFooterWidget HeaderWidget = null;
        internal HeaderFooterWidget FooterWidget = null;
        internal ObservableCollection<BodyWidget> BodyWidgets;
        /// <summary>
        /// Gets or Sets the foreground container
        /// </summary>
        internal Canvas ForegroundContainer;
        /// <summary>
        /// Gets or Sets the decoration container
        /// </summary>
        internal Canvas DecorationContainer;
        /// <summary>
        /// Gets or Sets the viewer
        /// </summary>
        internal LayoutViewer Viewer;
        private Rect boundingRectangle;
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets or sets the section.
        /// </summary>
        /// <value>
        /// The section.
        /// </value>
        internal SectionAdv Section
        {
            get
            {
                return section;
            }
            set
            {
                section = value;
            }
        }
        /// <summary>
        /// Gets or sets the bounding rectangle.
        /// </summary>
        /// <value>
        /// The bounding rectangle.
        /// </value>
        internal Rect BoundingRectangle
        {
            get
            {
                return boundingRectangle;
            }
            set
            {
                boundingRectangle = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PageAdv" /> class.
        /// </summary>
        internal PageAdv()
        {
#if WPF
            //Handled specifically to avoid focus, on Tab navigation. Sets PageAdv as not focusable control.
            this.Focusable = false;
#endif
            IsHitTestVisible = false;
            this.ForegroundContainer = new Canvas();
            this.DecorationContainer = new Canvas();
            Children.Add(ForegroundContainer);
            Children.Add(DecorationContainer);
            Canvas.SetZIndex(ForegroundContainer, 2);
            Canvas.SetZIndex(DecorationContainer, 1);
            popupTimer = new DispatcherTimer();
            popupTimer.Tick += popupTimer_Tick;
            popupTimer.Interval = new TimeSpan(0, 0, 2);
            CurrentPageBlock = new TextBlock();
            CurrentPageBlock.Foreground = new SolidColorBrush(Colors.White);
            CurrentPageBlock.FontSize = 25;
            CurrentPagePopup = new Border();
            CurrentPagePopup.Background = new SolidColorBrush(Colors.Gray);
            CurrentPagePopup.Padding = new Thickness(10, 15, 10, 10);
            CurrentPagePopup.Child = CurrentPageBlock;
            CurrentPagePopup.Visibility = Visibility.Collapsed;
            Children.Add(CurrentPagePopup);
            BodyWidgets = new ObservableCollection<BodyWidget>();
            BodyWidgets.CollectionChanged += BodyWidgets_CollectionChanged;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Handles the CollectionChanged event of the BodyWidgets control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        void BodyWidgets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (BodyWidget widget in e.NewItems)
                {
                    if (widget != null)
                        widget.Page = this;
                }
            }
        }
        /// <summary>
        /// Popups the timer_ tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void popupTimer_Tick(object sender, object e)
        {
            CurrentPagePopup.Visibility = Visibility.Collapsed;
            (sender as DispatcherTimer).Stop();
        }
        /// <summary>
        /// Shows the page number.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        internal void ShowPageNumber(string pageNumber)
        {
            CurrentPageBlock.Text = pageNumber;
            CurrentPagePopup.Visibility = Visibility.Visible;
            popupTimer.Start();
        }
        /// <summary>
        /// Hides the page number.
        /// </summary>
        internal void HidePageNumber()
        {
            if (CurrentPagePopup != null)
            {
                CurrentPagePopup.Visibility = Visibility.Collapsed;
                popupTimer.Stop();
            }
        }
        /// <summary>
        /// Renders the widgets.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void RenderWidgets(FlowLayoutViewer viewer)
        {
            for (int i = 0; i < BodyWidgets.Count; i++)
            {
                BodyWidgets[i].Render(viewer);
            }
            viewer.UpdateCaret(this, false);
        }
        /// <summary>
        /// Renders the widgets.
        /// </summary>
        internal void RenderWidgets()
        {
            if (HeaderWidget != null)
                HeaderWidget.Render(this);
            if (FooterWidget != null)
                FooterWidget.Render(this);
            for (int i = 0; i < BodyWidgets.Count; i++)
            {
                BodyWidgets[i].Render(this);
            }
            if (Viewer != null)
                Viewer.UpdateCaret(this, false);
        }
        /// <summary>
        /// Removes the widgets.
        /// </summary>
        internal void RemoveWidgets()
        {
            if (Width != BoundingRectangle.Width)
            {
                //Sets the page to normal (100%) scaling factor.
                Width = BoundingRectangle.Width;
                Height = BoundingRectangle.Height;
            }
            HidePageNumber();
            if (HeaderWidget != null)
                HeaderWidget.RemoveWidget();
            if (FooterWidget != null)
                FooterWidget.RemoveWidget();
            for (int i = 0; i < BodyWidgets.Count; i++)
            {
                BodyWidgets[i].RemoveWidget();
            }
            if (ForegroundContainer != null)
            {
                ForegroundContainer.ClearValue(Canvas.RenderTransformProperty);
                ForegroundContainer.Children.Clear();
            }
            if (DecorationContainer != null)
            {
                DecorationContainer.ClearValue(Canvas.RenderTransformProperty);
                DecorationContainer.Children.Clear();
            }
        }
        /// <summary>
        /// Sets the background.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        internal void SetBackground(SfRichTextBoxAdv richTextBoxAdv)
        {
            if (richTextBoxAdv != null && richTextBoxAdv.LayoutType == LayoutType.Block)
                ClearValue(BackgroundProperty);
            else
            {
                Color background = Colors.White;
                if (richTextBoxAdv != null && richTextBoxAdv.Document != null)
                    background = richTextBoxAdv.Document.Background;
                Background = new SolidColorBrush(background);
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            HeaderWidget = null;
            FooterWidget = null;
            if (Viewer != null)
            {
                if (Viewer.Pages != null)
                    Viewer.RemovePage(this);
                Viewer = null;
            }
            section = null;
            RemoveWidgets();
            ForegroundContainer = null;
            DecorationContainer = null;
            Children.Clear();
            BodyWidgets.CollectionChanged -= BodyWidgets_CollectionChanged;
            BodyWidgets.Clear();
            popupTimer.Stop();
            popupTimer.Tick -= popupTimer_Tick;
            CurrentPagePopup = null;
            CurrentPageBlock = null;
        }
        #endregion
    }
}
