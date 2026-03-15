#region Copyright Syncfusion Inc. 2001 - 2014
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

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Animation Class.
    /// </summary>
   public class Animation
    { 
        #region Private memebers
        /// <summary>
        /// Stores the width key frame.
        /// </summary>
        private SplineDoubleKeyFrame sizeAnimationWidthKeyFrame;

        /// <summary>
        /// Stores the height key frame.
        /// </summary>
        private SplineDoubleKeyFrame sizeAnimationHeightKeyFrame;

        /// <summary>
        /// Stores the posisition X key frame.
        /// </summary>
        private SplineDoubleKeyFrame positionAnimationXKeyFrame;

        /// <summary>
        /// Stores the position Y keyframe.
        /// </summary>
        private SplineDoubleKeyFrame positionAnimationYKeyFrame;

        /// <summary>
        /// Stores the size animation.
        /// </summary>
        private Storyboard sizeAnimation;

        /// <summary>
        /// Stores the position animation.
        /// </summary>
        private Storyboard positionAnimation;

        /// <summary>
        /// Stores the opacity animation.
        /// </summary>
        private Storyboard opactityAnimation;

        /// <summary>
        /// Stores a flag indicating if the opacity is animating.
        /// </summary>
        private bool opacityAnimating;

        /// <summary>
        /// Stores a flag indicating if the size is animating.
        /// </summary>        
        private bool sizeAnimating;

        /// <summary>
        /// Stores a flag storing if the position is animating.
        /// </summary>
        private bool positionAnimating;

        /// <summary>
        /// Stores the size animation timespan.
        /// </summary>
        private TimeSpan sizeAnimationTimespan = new TimeSpan(0, 0, 0, 0, 500);

        /// <summary>
        /// Stores the position animation time span.
        /// </summary>
        private TimeSpan positionAnimationTimespan = new TimeSpan(0, 0, 0, 0, 500);

        /// <summary>
        /// Stores the Animation Window.
        /// </summary>
        private Window _window;
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="window">The window.</param>
        public Animation(Window window)
        {
            _window = window;
            this.sizeAnimation = new Storyboard();
            int timeSpan = 500;
            if (window.DockingManager != null)
            {
                timeSpan = window.DockingManager.AutoHideAnimationSpeed;
            }
            DoubleAnimationUsingKeyFrames widthAnimation = new DoubleAnimationUsingKeyFrames();
            //// Required for WPF, but causes flickering in Silverlight
            //// widthAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(widthAnimation, _window);
            Storyboard.SetTargetProperty(widthAnimation, new System.Windows.PropertyPath("(FrameworkElement.Width)"));
            this.sizeAnimationWidthKeyFrame = new SplineDoubleKeyFrame();
            this.sizeAnimationWidthKeyFrame.KeySpline = new KeySpline()
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            this.sizeAnimationWidthKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeSpan));
            this.sizeAnimationWidthKeyFrame.Value = 0;
            widthAnimation.KeyFrames.Add(this.sizeAnimationWidthKeyFrame);
            DoubleAnimationUsingKeyFrames heightAnimation = new DoubleAnimationUsingKeyFrames();
            // Required for WPF, but causes flickering in Silverlight
            // heightAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(heightAnimation, _window);
            Storyboard.SetTargetProperty(heightAnimation, new System.Windows.PropertyPath("(FrameworkElement.Height)"));
            this.sizeAnimationHeightKeyFrame = new SplineDoubleKeyFrame();
            this.sizeAnimationHeightKeyFrame.KeySpline = new KeySpline()
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            this.sizeAnimationHeightKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeSpan));
            this.sizeAnimationHeightKeyFrame.Value = 0;
            heightAnimation.KeyFrames.Add(this.sizeAnimationHeightKeyFrame);
            this.sizeAnimation.Children.Add(widthAnimation);
            this.sizeAnimation.Children.Add(heightAnimation);
           //// this.sizeAnimation.Completed += new EventHandler(this.SizeAnimation_Completed);
            this.positionAnimation = new Storyboard();
            DoubleAnimationUsingKeyFrames positionXAnimation = new DoubleAnimationUsingKeyFrames();
            //// Required for WPF, but causes flickering in Silverlight
            //// positionXAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(positionXAnimation, _window);
            Storyboard.SetTargetProperty(positionXAnimation, new System.Windows.PropertyPath("(Canvas.Left)"));
            this.positionAnimationXKeyFrame = new SplineDoubleKeyFrame();
            this.positionAnimationXKeyFrame.KeySpline = new KeySpline()
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            this.positionAnimationXKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeSpan));
            this.positionAnimationXKeyFrame.Value = 0;
            positionXAnimation.KeyFrames.Add(this.positionAnimationXKeyFrame);
            DoubleAnimationUsingKeyFrames positionYAnimation = new DoubleAnimationUsingKeyFrames();
            //// Required for WPF, but causes flickering in Silverlight
            //// positionYAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(positionYAnimation, _window);
            Storyboard.SetTargetProperty(positionYAnimation, new System.Windows.PropertyPath("(Canvas.Top)"));
            this.positionAnimationYKeyFrame = new SplineDoubleKeyFrame();
            this.positionAnimationYKeyFrame.KeySpline = new KeySpline()
            {
                ControlPoint1 = new Point(0.528, 0),
                ControlPoint2 = new Point(0.142, 0.847)
            };
            this.positionAnimationYKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(timeSpan));
            this.positionAnimationYKeyFrame.Value = 0;
            positionYAnimation.KeyFrames.Add(this.positionAnimationYKeyFrame);
            this.positionAnimation.Children.Add(positionXAnimation);
            this.positionAnimation.Children.Add(positionYAnimation);
            this.positionAnimation.Completed += new EventHandler(this.PositionAnimation_Completed);
            this.sizeAnimation.Completed += new EventHandler(SizeAnimation_Completed);
        }

        /// <summary>
        /// Stores the opacity animation time span.
        /// </summary>
        DoubleAnimation opacitydblAnimation;


        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="type">The type.</param>
        public Animation(Window window, string type)
        {
            _window = window;
            this.opactityAnimation = new Storyboard();
            #region FutureUse
            ////DoubleAnimationUsingKeyFrames opacitydblAnimation = new DoubleAnimationUsingKeyFrames();
            ////Storyboard.SetTargetProperty(opacitydblAnimation, new PropertyPath("(FrameworkElement.Opacity)"));
            ////Storyboard.SetTarget(opacitydblAnimation, _window);
            ////this.opacityAnimationKeyFrame = new SplineDoubleKeyFrame();
            ////this.opacityAnimationKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000));
            ////this.opacityAnimationKeyFrame.Value = 1;
            ////opacitydblAnimation.KeyFrames.Add(this.opacityAnimationKeyFrame); 
            #endregion
            this.opacitydblAnimation = new DoubleAnimation();
            int timeSpan = 500;
            if (window.DockingManager != null)
            {
                timeSpan = window.DockingManager.AutoHideAnimationSpeed;
            }
            Duration duration = new Duration(TimeSpan.FromSeconds(Convert.ToInt32(timeSpan)));

            opactityAnimation.Duration = duration;
            this.opacitydblAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(Convert.ToInt32(timeSpan)));            
            _window.Opacity = 1;
            Storyboard.SetTargetProperty(this.opacitydblAnimation, new PropertyPath("(FrameworkElement.Opacity)"));
            Storyboard.SetTarget(this.opacitydblAnimation, _window);
            this.opactityAnimation.Children.Add(this.opacitydblAnimation);
            //this.opactityAnimation.Completed += new EventHandler(opactityAnimation_Completed);
            this.opacitydblAnimation.Completed += new EventHandler(opacitydblAnimation_Completed);
        }

        /// <summary>
        /// Handles the Completed event of the opacitydblAnimation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void opacitydblAnimation_Completed(object sender, EventArgs e)
        { 
        }

        /// <summary>
        /// Handles the Completed event of the opactityAnimation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void opactityAnimation_Completed(object sender, EventArgs e)
        { 
        }


        /// <summary>
        /// Animates the opacity.
        /// </summary>
        protected internal void AnimateOpacity()
        {
            if (this.opacityAnimating)
            {
                this.opactityAnimation.Pause();
            }

            ////this.Width = this.ActualWidth;
            ////this.Height = this.ActualHeight;
            this.opacityAnimating = true;
            this.opacitydblAnimation.To = 1;
            this.opacitydblAnimation.From = 0.0001;
            this.opactityAnimation.Begin();
        }

        #region Public members
        /// <summary>
        /// Gets or sets the size animation duration.
        /// </summary>
        /// <value>The duration of the size animation.</value>
        protected internal TimeSpan SizeAnimationDuration
        {
            get
            {
                return this.sizeAnimationTimespan;
            }

            set
            {
                this.sizeAnimationTimespan = value;
                if (this.sizeAnimationWidthKeyFrame != null)
                {
                    this.sizeAnimationWidthKeyFrame.KeyTime = KeyTime.FromTimeSpan(this.sizeAnimationTimespan);
                }

                if (this.sizeAnimationHeightKeyFrame != null)
                {
                    this.sizeAnimationHeightKeyFrame.KeyTime = KeyTime.FromTimeSpan(this.sizeAnimationTimespan);
                }
            }
        }

        /// <summary>
        /// Gets or sets the position animation duration.
        /// </summary>
        /// <value>The duration of the position animation.</value>
        protected internal TimeSpan PositionAnimationDuration
        {
            get
            {
                return this.positionAnimationTimespan;
            }

            set
            {
                this.positionAnimationTimespan = value;
                if (this.positionAnimationXKeyFrame != null)
                {
                    this.positionAnimationXKeyFrame.KeyTime = KeyTime.FromTimeSpan(this.positionAnimationTimespan);
                }

                if (this.positionAnimationYKeyFrame != null)
                {
                    this.positionAnimationYKeyFrame.KeyTime = KeyTime.FromTimeSpan(this.positionAnimationTimespan);
                }
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Animates the size of the XY.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void AnimateXYSize(double width, double height)
        {
        }

        /// <summary>
        /// Animates the size of the control.
        /// </summary>
        /// <param name="width">The target width</param>
        /// <param name="height">The target height</param>
        protected internal void AnimateSize(double width, double height)
        {
            if (this.sizeAnimating)
            {
                this.sizeAnimation.Pause();
            }
            try
            {
                this.sizeAnimating = true;
                this.sizeAnimationWidthKeyFrame.Value = width;
                this.sizeAnimationHeightKeyFrame.Value = height;
                this.sizeAnimation.Begin();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Stops all animation.
        /// </summary>
        protected internal void StopAllAnimation()
        {
            _window.FloatWidth = _window.Width;
            _window.FloatHeight = _window.Height;
            this.sizeAnimation.Stop();
            this.positionAnimation.Stop();
            _window.Width = _window.FloatWidth;
            _window.Height = _window.FloatHeight;
        }

        /// <summary>
        /// Animates the Canvas.Left and Canvas.Top of the control.
        /// </summary>
        /// <param name="x">New X position</param>
        /// <param name="y">New Y position</param>
        public void AnimatePosition(double x, double y)
        {
            if (this.positionAnimating)
            {
                this.positionAnimation.Pause();
            }

                this.positionAnimating = true;
                this.positionAnimationXKeyFrame.Value = x;
                this.positionAnimationYKeyFrame.Value = y;
                this.positionAnimation.Begin();
        }
        #endregion

        /// <summary>
        /// Stores the position.
        /// </summary>
        /// <param name="sender">The position animation.</param>
        /// <param name="e">Event args.</param>
        private void PositionAnimation_Completed(object sender, EventArgs e)
        {              
        }

        /// <summary>
        /// Stores the values once the animation has completed.
        /// </summary>
        /// <param name="sender">The animated content control.</param>
        /// <param name="e">The event args.</param>
        private void SizeAnimation_Completed(object sender, EventArgs e)
        {
            ////_window.Width = this.sizeAnimationWidthKeyFrame.Value;
            ////_window.Height = this.sizeAnimationHeightKeyFrame.Value;
            if (_window.WindowDockPin == DockPin.UnPinned)
            {
                #region oldContent
                //if (_window.DockManager.Parent.GetType() != typeof(WindowContainer))
                //{
                //    _window.Visibility = Visibility.Collapsed;
                //    _window.ChangeState(DockState.AutoHidden);
                //    if (!_window.DockingManager.Children.Contains(_window))
                //    {
                //        if (_window.Parent == null)
                //        {
                //            _window.DockingManager.Children.Add(_window);
                //        }
                //    }

                //    _window.DockingManager.GenerateSideGrid(_window);
                //    _window.WindowDockPin = DockPin.Pinned;
                //    _window.Visibility = Visibility.Collapsed;
                //}
                //else
                //{
                //    if ((_window.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count == 1)
                //    {
                //        UIElement el = (_window.DockManager.Children[0] as DockingGrid).gridDocking.Children[0] as UIElement;
                //        (_window.DockManager.Parent as WindowContainer)._window.Visibility = Visibility.Collapsed;
                //        if ((_window.DockManager.Parent as WindowContainer)._window.Parent is Grid)
                //        {
                //            (_window.DockManager.Parent as WindowContainer)._window.ChangeState(DockState.Hidden);
                //        }                           

                //            if ((_window.DockManager.Parent as WindowContainer)._window.Parent == null)
                //            {
                //                //Window windowcontain = (_window.DockManager.Parent as WindowContainer)._window;
                //                //if ((windowcontain.DockManager.Children[0] as DockingGrid).gridDocking.Children.Count == 0)
                //                //{
                //                    _window.DockingManager.Children.Add((_window.DockManager.Parent as WindowContainer)._window);
                //               // }
                //            }
                //            else
                //            {

                //                //_window.GetFloatParentWindowContainer((_window.DockManager.Parent as WindowContainer)._window);
                //                _window.AutoHideParentWindow(_window);
                //                ((_window.DockManager.Parent as WindowContainer)._window.Parent as Grid).Children.Remove((_window.DockManager.Parent as WindowContainer)._window);
                //                _window.DockingManager.Children.Add((_window.DockManager.Parent as WindowContainer)._window);
                //            }
                //            ////_window.Visibility = Visibility.Collapsed;
                //            ////_window.ChangeState(DockState.Hidden);
                //            _window.DockingManager.GenerateSideGrid(_window);
                //            _window.WindowDockPin = DockPin.Pinned;
                //            _window.DockState = DockState.AutoHidden;
                //            _window.AutoHideParentWindow(_window);
                //    }
                //    else
                //    {
                //        _window.Visibility = Visibility.Collapsed;
                //        _window.ChangeState(DockState.AutoHidden);
                //        if (!_window.DockingManager.Children.Contains(_window))
                //        {
                //            if (_window.Parent == null)
                //            {
                //                _window.DockingManager.Children.Add(_window);
                //            }
                //        }

                //        _window.DockingManager.GenerateSideGrid(_window);
                //        _window.WindowDockPin = DockPin.Pinned;
                //        _window.Visibility = Visibility.Collapsed;
                //    }
                //}  
                #endregion                
                _window.SetPinDockWindow();
                _window.DockingManager.UpdateSidePanelLayout();
                //if (_window.DockingManager.DockFill && counter == 1)
                //{
                //    (_window.DockManager.Children[0] as DockingGrid).gridDocking.Children.Clear();
                //}
                _window.WindowDockPin = DockPin.Pinned;
                if (_window.DockingManager != null)
                {
                    _window.DockingManager.InvokeEventForPinned(_window.WindowChildElement);
                }
                            }
            else if (_window.WindowDockPin == DockPin.Pinned)
            {
                _window.Visibility = Visibility.Visible;
                _window.Width = this.sizeAnimationWidthKeyFrame.Value;
                _window.Height = this.sizeAnimationHeightKeyFrame.Value;
                _window.WindowDockPin = DockPin.UnPinned;
            }
            else if (_window.WindowDockPin == DockPin.None)
            {
                _window.Visibility = Visibility.Collapsed;               
                _window.WindowDockPin = DockPin.Pinned;    
            }
        }
    }
}
