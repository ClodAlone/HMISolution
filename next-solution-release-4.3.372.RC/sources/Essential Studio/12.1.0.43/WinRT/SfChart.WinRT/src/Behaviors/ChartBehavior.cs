#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartBehavior is an abstract base class for behaviors which can be added to <see cref="SfChart"/> 
    /// </summary>
    /// <remarks>
    /// You can handle the <see cref="SfChart"/> events directly in Chart behavior,which will be helpful in designing the Chart application in MVVM pattern.
    /// You can add a custom behavior to the <see cref="SfChart"/> by inheriting a class from the <see cref="ChartBehavior"/>.
    /// You can also add ui elements to the Chart by making use of the top layer canvas returned from <see cref="ChartBehavior.AdorningCanvas"/> property,which can be used to place the ui
    /// elements at desired positions in <see cref="SfChart"/>.  
    /// </remarks>
    /// <seealso cref="ChartZoomPanBehavior"/>
    /// <seealso cref="ChartSelectionBehavior"/>
    /// <seealso cref="ChartTrackBallBehavior"/>
    /// <seealso cref="ChartCrossHairBehavior"/>
    public abstract class ChartBehavior : DependencyObject,ICloneable
    {
        #region fields

        private Canvas adorningCanvas, bottomAdorningCanvas;

        private SfChart chartArea;

        #endregion

        #region properties
        /// <summary>
        /// Gets the top layer Canvas.
        /// </summary>
        public Canvas AdorningCanvas
        {
            get
            {
                return adorningCanvas;
            }
            internal set
            {
                if (adorningCanvas != value)
                {
                    adorningCanvas = value;
                }
            }
        }

        /// <summary>
        /// Gets the bottom layer Canvas.
        /// </summary>
        public Canvas BottomAdorningCanvas
        {
            get
            {
                return bottomAdorningCanvas;
            }
            internal set
            {
                if (bottomAdorningCanvas != value)
                {
                    bottomAdorningCanvas = value;
                }
            }
        }

        /// <summary>
        /// Gets the owner Chart
        /// </summary>
        public SfChart ChartArea
        {
            get
            {
                return chartArea;
            }
            internal set
            {
                chartArea = value;
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// Constructor
        /// </summary>
        public ChartBehavior()
        {

        }

        #endregion

        #region methods

        /// <summary>
        /// Called when layout updated
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnLayoutUpdated()
        {

        }
        /// <summary>
        /// Method implementation for DetachElement
        /// </summary>
        /// <param name="element"></param>
        protected internal virtual void DetachElement(UIElement element)
        {
            if (this.AdorningCanvas.Children.Contains(element))
                this.AdorningCanvas.Children.Remove(element);
        }
        /// <summary>
        /// Method implementation for UpdateArea in Chart
        /// </summary>
        protected void UpdateArea()
        {
            if (ChartArea != null)
            {
                ChartArea.ScheduleUpdate();
            }
        }
        /// <summary>
        /// Return collection of double values from the given ChartSeries
        /// </summary>
        /// <param name="x"></param>
        /// <param name="series"></param>
        /// <returns></returns>
        protected IList<double> GetYValuesBasedOnIndex(double x, ChartSeriesBase series)
        {
            List<double> Values = new List<double>();
            if (x < series.DataCount)
            {
                for (int i = 0; i < series.ActualSeriesYValues.Count(); i++)
                {
                    Values.Add(series.ActualSeriesYValues[i][(int)x]);
                }
            }

            return Values;
        }

        internal void InternalAttachElements()
        {
            this.AttachElements();
        }

        /// <summary>
        /// Method implementation for AttachElements
        /// </summary>
        protected virtual void AttachElements()
        {

        }
        /// <summary>
        /// Method implementation for DetachElements
        /// </summary>
        internal protected virtual void DetachElements()
        {

        }
        /// <summary>
        /// Called when Size Changed
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnSizeChanged(SizeChangedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Drag action enter into the ChartArea
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnDragEnter(DragEventArgs e)
        {
        }
        /// <summary>
        /// Called when Drag action leave from the area
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnDragLeave(DragEventArgs e)
        {
        }
        /// <summary>
        /// Called when Drag action over in the Area
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnDragOver(DragEventArgs e)
        {
        }
        /// <summary>
        /// Called when drop the cursor in ChartArea
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnDrop(DragEventArgs e)
        {
        }
        /// <summary>
        /// Called when GotFocus in UIElement
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnGotFocus(RoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Lost the focus in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnLostFocus(RoutedEventArgs e)
        {
        }

#if WINDOWS_PHONE

#if !WPF
        /// <summary>
        /// Called when Double tapped in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnDoubleTap(GestureEventArgs e)
        {
            
        }
        /// <summary>
        /// Called when pointer Tap in chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnTap(GestureEventArgs e)
        {

        }
        /// <summary>
        /// Called when Hold the pointer in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnHold(GestureEventArgs e)
        {

        }

#endif
        /// <summary>
        /// Called when MouseWheel on Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnMouseWheel(MouseWheelEventArgs e)
        {

        }
        /// <summary>
        /// Called when MouseEnter in to Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnMouseEnter(MouseEventArgs e)
        {

        }
        /// <summary>
        /// Called when MouseLeave from Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnMouseLeave(MouseEventArgs e)
        {

        }
        /// <summary>
        /// Called when MouseMove in chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnMouseMove(MouseEventArgs e)
        {

        }
        /// <summary>
        /// Called when Pointer key up in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnKeyUp(KeyEventArgs e)
        {

        }
        /// <summary>
        /// Called when pointer key down in ChartArea
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnKeyDown(KeyEventArgs e)
        {

        }
        /// <summary>
        /// Called when MouseLeftButtonDown in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// Called when OnMouse
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// Called when ManipulationStarted
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnManipulationStarted(ManipulationStartedEventArgs e)
        {

        }
        /// <summary>
        /// Called when ManipulationCompleted
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {

        }
        /// <summary>
        /// Called when ManipulationDelta is changed
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            
        }
#else
        /// <summary>
        /// Method implementation for OnDoubleTapped
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Holding the Focus in UIElement
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnHolding(HoldingRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when KeyDown in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnKeyDown(KeyRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Key up in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnKeyUp(KeyRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Manipulation complete in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Manipulation delta is changed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Manipulation action Start in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Manipulation Started
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when manipulation starting
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Pointer cancelled in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnPointerCanceled(PointerRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Pointer Capturedlost in Chart.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when PointerEntered in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnPointerEntered(PointerRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when PointerExited in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnPointerExited(PointerRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Pointer moved in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnPointerMoved(PointerRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Pointer pressed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnPointerPressed(PointerRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Pointer Released in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnPointerReleased(PointerRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when PointerWheel Changed
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when RightTapped the Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnRightTapped(RightTappedRoutedEventArgs e)
        {
        }
        /// <summary>
        /// Called when Tapped the Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnTapped(TappedRoutedEventArgs e)
        {
        }

#endif
        protected virtual DependencyObject CloneBehavior(DependencyObject obj)
        {
            return obj;
        }

        public DependencyObject Clone()
        {
            return CloneBehavior(null);
        }

        #endregion


    }
}
