// <copyright file="HubTileBase.cs" company="Syncfusion">
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
#if !(SILVERLIGHT||WPF||WINDOWS_PHONE_7)
using Windows.UI.Input;
#endif
#if WINDOWS_PHONE || WINDOWS_PHONE_7
using Syncfusion.WP.Primitives;
using Syncfusion.WP.Utils;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;


namespace Syncfusion.WP.Controls.Notification
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Tools.Primitives;
using System.Windows.Media.Animation;
namespace Syncfusion.Tools.Controls.Notification
#else
#if WPF
using Syncfusion.Windows.Primitives;
using Syncfusion.Windows.Utils;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
namespace Syncfusion.Windows.Controls.Notification
#else
using Syncfusion.UI.Xaml.Primitives;
using Syncfusion.UI.Xaml.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation;
using System.Windows.Input;
namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
#endif
#endif
{
    /// <summary>
    /// Serves as a base for the <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/> class.
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
#if WPF
    [CLSCompliant(false)]
#endif
    public class HubTileBase : HeaderedContentControl
    {
        #region Variables

        private Storyboard storyboard;
        /// <summary>
        /// Gets or internal set the Storyboard.
        /// </summary>
        /// <value>The storyboard.</value>
        public Storyboard Storyboard
        {
            get { return storyboard; }
            internal set { storyboard = value;  }
        }

        private PointerDirection pointerdirection;

        private bool pointerpressed;

        private bool pointerover;

        /// <summary>
        /// Invokes the method for routed events
        /// </summary>
        public event RoutedEventHandler Click;

        private bool _canExecute = true;
        #endregion

        #region Dependency Properties


        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(HubTileBase), new PropertyMetadata(String.Empty));



        /// <summary>
        /// Gets or sets a value indicating whether this instance is frozen.
        /// </summary>
        /// <value><c>true</c> if this instance is frozen; otherwise, <c>false</c>.</value>
        public bool IsFrozen
        {
            get { return (bool)GetValue(IsFrozenProperty); }
            set { SetValue(IsFrozenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsFrozen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsFrozenProperty =
            DependencyProperty.Register("IsFrozen", typeof(bool), typeof(HubTileBase), new PropertyMetadata(false, new PropertyChangedCallback(OnIsFrozenChanged)));



        /// <summary>
        /// Gets or sets the title style.
        /// </summary>
        /// <value>The title style.</value>
        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TitleStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty =
            DependencyProperty.Register("TitleStyle", typeof(Style), typeof(HubTileBase), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets a value indicating whether to override default states of the tile.
        /// </summary>
        /// <value><c>true</c> if override default states; otherwise, <c>false</c>.</value>
        public bool OverrideDefaultStates
        {
            get { return (bool)GetValue(OverrideDefaultStatesProperty); }
            set { SetValue(OverrideDefaultStatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for OverrideDefaultStates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OverrideDefaultStatesProperty =
            DependencyProperty.Register("OverrideDefaultStates", typeof(bool), typeof(HubTileBase), new PropertyMetadata(false));



        /// <summary>
        /// Gets or sets the accent brush.
        /// </summary>
        /// <value>The accent brush.</value>
        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(HubTileBase), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the title to show in the tile.
        /// </summary>
        /// <value>The title.</value>
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(HubTileBase), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the image to display in the tile.
        /// </summary>
        /// <value>The image source.</value>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(HubTileBase), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the rotation depth.
        /// </summary>
        /// <value>The rotation depth.</value>
        public double RotationDepth
        {
            get { return (double)GetValue(RotationDepthProperty); }
            set { SetValue(RotationDepthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RotationDepth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RotationDepthProperty =
            DependencyProperty.Register("RotationDepth", typeof(double), typeof(HubTileBase), new PropertyMetadata(20.0d));



        /// <summary>
        /// Gets or sets the scale depth.
        /// </summary>
        /// <value>The scale depth.</value>
        public double ScaleDepth
        {
            get { return (double)GetValue(ScaleDepthProperty); }
            set { SetValue(ScaleDepthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScaleDepth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScaleDepthProperty =
            DependencyProperty.Register("ScaleDepth", typeof(double), typeof(HubTileBase), new PropertyMetadata(0.9d));




        /// <summary>
        /// Gets or sets the duration of the tile press.
        /// </summary>
        /// <value>The duration of the tile press.</value>
        public TimeSpan TilePressDuration
        {
            get { return (TimeSpan)GetValue(TilePressDurationProperty); }
            set { SetValue(TilePressDurationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TilePressDurationProperty =
            DependencyProperty.Register("TilePressDuration", typeof(TimeSpan), typeof(HubTileBase), new PropertyMetadata(TimeSpan.FromSeconds(0.1)));



        /// <summary>
        /// Gets or Sets the command to be executed when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/> is pressed.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        /// </summary>
        /// <value>The default value is null</value>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(HubTileBase), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the value for the parameter of the command.
        /// </summary>
        /// <value>The default value is null</value>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(HubTileBase), new PropertyMetadata(null));


        #endregion

        #region Helper Methods

        public void ExecutePointerReleased()
        {
            if (pointerpressed)
            {
                if (OverrideDefaultStates)
                {
                    if (pointerover)
                    {
                        VisualStateManager.GoToState(this, "PointerOver", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "Normal", true);
                    }
                }
                else
                {
#if !WPF
                    if (storyboard != null)
                    {
                        if (pointerdirection != PointerDirection.Center)
                        {
                            DoubleAnimationUsingKeyFrames animation = storyboard.Children[0] as DoubleAnimationUsingKeyFrames;
                            if (animation != null)
                            {
                                EasingDoubleKeyFrame frame1 = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                                EasingDoubleKeyFrame frame2 = animation.KeyFrames[1] as EasingDoubleKeyFrame;

                                frame1.Value = frame2.Value;
                                frame2.Value = 0;
                            }
                        }
                        else
                        {
                            DoubleAnimationUsingKeyFrames xanimation = storyboard.Children[0] as DoubleAnimationUsingKeyFrames;
                            DoubleAnimationUsingKeyFrames yanimation = storyboard.Children[1] as DoubleAnimationUsingKeyFrames;

                            if (xanimation != null)
                            {
                                EasingDoubleKeyFrame frame1 = xanimation.KeyFrames[0] as EasingDoubleKeyFrame;
                                EasingDoubleKeyFrame frame2 = xanimation.KeyFrames[1] as EasingDoubleKeyFrame;

                                frame1.Value = frame2.Value;
                                frame2.Value = 1;
                            }

                            if (yanimation != null)
                            {
                                EasingDoubleKeyFrame frame1 = yanimation.KeyFrames[0] as EasingDoubleKeyFrame;
                                EasingDoubleKeyFrame frame2 = yanimation.KeyFrames[1] as EasingDoubleKeyFrame;

                                frame1.Value = frame2.Value;
                                frame2.Value = 1;
                            }
                        }
                        storyboard.Begin();
                    }
#else
                    this.RenderTransformOrigin = new Point(0.5, 0.5);
                    this.RenderTransform = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };
#endif
                }
            }
            pointerpressed = false;
        }

        private Timeline BuildScaleXAnimation()
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();

            Storyboard.SetTarget(animation, this);
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
#else
#if WPF
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
#else
            Storyboard.SetTargetProperty(animation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
#endif
#endif
            EasingDoubleKeyFrame keyframe1 = new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0), Value = 1 };
            EasingDoubleKeyFrame keyframe2 = new EasingDoubleKeyFrame() { KeyTime = TilePressDuration, Value = (ScaleDepth>this.ActualWidth/4)?ActualWidth/4:ScaleDepth};

            animation.KeyFrames.Add(keyframe1);
            animation.KeyFrames.Add(keyframe2);
#if WPF
            TranslateTransform transform = new TranslateTransform();
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = transform;
#else
            CompositeTransform transform = new CompositeTransform();
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = transform;
#endif
            return animation;
        }

        private Timeline BuildScaleYAnimation()
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();

            Storyboard.SetTarget(animation, this);
#if WPF
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#elif !WINRT
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
#else
            Storyboard.SetTargetProperty(animation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
#endif
            EasingDoubleKeyFrame keyframe1 = new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0), Value = 1 };
            EasingDoubleKeyFrame keyframe2 = new EasingDoubleKeyFrame() { KeyTime = TilePressDuration, Value = (ScaleDepth>this.ActualWidth/4)?ActualWidth/4:ScaleDepth };

            animation.KeyFrames.Add(keyframe1);
            animation.KeyFrames.Add(keyframe2);
#if WPF
            TranslateTransform transform = new TranslateTransform();
            this.RenderTransform = transform;
            this.RenderTransformOrigin = new Point(0.5, 0.5);
#else
            CompositeTransform transform = new CompositeTransform();
            this.RenderTransform = transform;
            this.RenderTransformOrigin = new Point(0.5, 0.5);
#endif
            return animation;
        }

        private Timeline BuildAnimation(PointerDirection direction)
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(animation, this);

            if (direction != PointerDirection.Center)
            {
#if !WPF
                PlaneProjection projection = new PlaneProjection() { CenterOfRotationZ = 0 };
#endif
                EasingDoubleKeyFrame keyframe1 = new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0), Value = 0 };
                EasingDoubleKeyFrame keyframe2 = new EasingDoubleKeyFrame() { KeyTime = TilePressDuration };

                animation.KeyFrames.Add(keyframe1);
                animation.KeyFrames.Add(keyframe2);

                //Setting the value
                if (direction == PointerDirection.Left || direction == PointerDirection.Bottom)
                {
                    keyframe2.Value =(RotationDepth>this.ActualWidth/2)?ActualWidth/2:RotationDepth;
                }
                else if (direction == PointerDirection.Top || direction == PointerDirection.Right)
                {
                    keyframe2.Value = -((RotationDepth>this.ActualWidth/2)?ActualWidth/2:RotationDepth);
                }

                //Setting the target property
                if (direction == PointerDirection.Top || direction == PointerDirection.Bottom)
                {
#if WPF
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
#elif !WINRT
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationX)"));
#else
                    Storyboard.SetTargetProperty(animation, "(UIElement.Projection).(PlaneProjection.RotationX)");
#endif
                }
                else if (direction == PointerDirection.Left || direction == PointerDirection.Right)
                {
#if WPF
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#elif !WINRT
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
#else
                    Storyboard.SetTargetProperty(animation, "(UIElement.Projection).(PlaneProjection.RotationY)");
#endif
                }
#if !WPF
                //Setting the origin
                if (direction == PointerDirection.Bottom)//(0.5, 0)
                {
                    projection.CenterOfRotationX = 0.5;
                    projection.CenterOfRotationY = 0;
                }
                else if (direction == PointerDirection.Top)//(0.5, 1)
                {
                    projection.CenterOfRotationX = 0.5;
                    projection.CenterOfRotationY = 1;
                }
                else if (direction == PointerDirection.Left)//(1, 0.5)
                {
                    projection.CenterOfRotationX = 1;
                    projection.CenterOfRotationY = 0.5;
                }
                else if (direction == PointerDirection.Right)//(0, 0.5)
                {
                    projection.CenterOfRotationX = 0;
                    projection.CenterOfRotationY = 0.5;
                }
                this.Projection = projection;
#endif
            }
            return animation;
        }

        #endregion

        #region Override Methods
        
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        /// <summary>
        /// Invoked when the manipulation has started.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationDelta(System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            _canExecute = false;
            base.OnManipulationDelta(e);
        }
#endif


#if !WPF
        /// <summary>
        /// Invoked when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.HubTile"/> is pressed.
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
#else
        protected override void OnTapped(Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
#endif
        {
            ExecutePointerReleased();
#if !WINRT
            base.OnTap(e);
#else
            base.OnTapped(e);
#endif
        }
#endif
        /// <summary>
        /// Invoked when the focus is lost
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnLostMouseCapture(System.Windows.Input.MouseEventArgs e)
#else
            protected override void OnPointerCaptureLost(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            ExecutePointerReleased();
#if !WINRT
            base.OnLostMouseCapture(e);
#else
            base.OnPointerCaptureLost(e);
#endif
        }

        /// <summary>
        /// Invoked when the Pointer is released
        /// </summary>
        /// <param name="e"></param>
#if !WINRT

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (pointerpressed)
            {
                if (_canExecute)
                {
                    if (Click != null)
                    {
                        Click(this, e);
                    }
                    if (Command != null && Command.CanExecute(CommandParameter))
                    {
                        Command.Execute(CommandParameter);
                    }
                    ExecutePointerReleased();
                }
                _canExecute = true;
            }
#if !WINRT
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
        }
        /// <summary>
        /// Invoked when the Pointer is pressed
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            pointerpressed = true;
            if (OverrideDefaultStates)
            {
                VisualStateManager.GoToState(this, "PointerPressed", true);
            }
            else
            {
#if !WINRT
                Point currentpoint = e.GetPosition(this);
#else
                PointerPoint currentpoint = e.GetCurrentPoint(this);
#endif

                double width = this.ActualWidth / 3;
                double height = this.ActualHeight / 3;

                //Divide the element into 9 pieces
                Dictionary<PointerDirection, Rect> pieces = new Dictionary<PointerDirection, Rect>();

                pieces.Add(PointerDirection.TopLeft, new Rect(0, 0, width, height));
                pieces.Add(PointerDirection.Top, new Rect(width, 0, width, height));
                pieces.Add(PointerDirection.TopRight, new Rect(width * 2, 0, width, height));

                pieces.Add(PointerDirection.Left, new Rect(0, height, width, height));
                pieces.Add(PointerDirection.Center, new Rect(width, height, width, height));
                pieces.Add(PointerDirection.Right, new Rect(width * 2, height, width, height));

                pieces.Add(PointerDirection.BottomLeft, new Rect(0, height * 2, width, height));
                pieces.Add(PointerDirection.Bottom, new Rect(width, height * 2, width, height));
                pieces.Add(PointerDirection.BottomRight, new Rect(width * 2, height * 2, width, height));

#if !WINRT
                pointerdirection = (from PointerDirection _direction in pieces.Keys
                                    where pieces[_direction].Contains(currentpoint)
                                    select _direction).FirstOrDefault();
#else
                pointerdirection = (from PointerDirection _direction in pieces.Keys
                                    where pieces[_direction].Contains(currentpoint.Position)
                                    select _direction).FirstOrDefault();
#endif

                storyboard = new Storyboard();
#if WPF
                this.RenderTransformOrigin = new Point(0.5, 0.5);
                this.RenderTransform = new ScaleTransform() { ScaleX = 0.9, ScaleY = 0.9 };
#else
                if ((int)pointerdirection <= 4)
                {
                    if (pointerdirection == PointerDirection.Center)
                    {
                        Timeline xtimeline = BuildScaleXAnimation();
                        Timeline ytimeline = BuildScaleYAnimation();
                        storyboard.Children.Add(xtimeline);
                        storyboard.Children.Add(ytimeline);
                        storyboard.Begin();
                    }
                    else
                    {
                        Timeline timeline = BuildAnimation(pointerdirection);
                        storyboard.Children.Add(timeline);
                        storyboard.Begin();
                    }
                }
                else
                {
                    if (pointerdirection == PointerDirection.TopLeft)
                    {
#if !WINRT
                        if (currentpoint.X > currentpoint.Y)
#else
                        if (currentpoint.Position.X > currentpoint.Position.Y)
#endif
                        {
                            Timeline timeline = BuildAnimation(PointerDirection.Top);
                            storyboard.Children.Add(timeline);
                            storyboard.Begin();
                        }
                        else
                        {
                            Timeline timeline = BuildAnimation(PointerDirection.Left);
                            storyboard.Children.Add(timeline);
                            storyboard.Begin();
                        }
                    }
                    else if (pointerdirection == PointerDirection.TopRight)
                    {
#if !WINRT
                        if (currentpoint.Y > height - (currentpoint.X - (width * 2)))
#else
                        if (currentpoint.Position.Y > height - (currentpoint.Position.X - (width * 2)))
#endif
                        {
                            Timeline timeline = BuildAnimation(PointerDirection.Right);
                            storyboard.Children.Add(timeline);
                            storyboard.Begin();
                        }
                        else
                        {
                            Timeline timeline = BuildAnimation(PointerDirection.Top);
                            storyboard.Children.Add(timeline);
                            storyboard.Begin();
                        }
                    }
                    else if (pointerdirection == PointerDirection.BottomLeft)
                    {
#if !WINRT
                        if ((currentpoint.Y - (height * 2)) > height - currentpoint.X)
#else
                        if ((currentpoint.Position.Y - (height * 2)) > height - currentpoint.Position.X)
#endif
                        {
                            Timeline timeline = BuildAnimation(PointerDirection.Bottom);
                            storyboard.Children.Add(timeline);
                            storyboard.Begin();
                        }
                        else
                        {
                            Timeline timeline = BuildAnimation(PointerDirection.Left);
                            storyboard.Children.Add(timeline);
                            storyboard.Begin();
                        }
                    }
                    else
                    {
#if !WINRT
                        if (currentpoint.X > currentpoint.Y)
#else
                        if (currentpoint.Position.X > currentpoint.Position.Y)
#endif
                        {
                            Timeline timeline = BuildAnimation(PointerDirection.Right);
                            storyboard.Children.Add(timeline);
                            storyboard.Begin();
                        }
                        else
                        {
                            Timeline timeline = BuildAnimation(PointerDirection.Bottom);
                            storyboard.Children.Add(timeline);
                            storyboard.Begin();
                        }
                    }
                }
#endif
            }
#if !WINRT
            base.OnMouseLeftButtonDown(e);
#if !WPF
            base.CaptureMouse();
#endif
#else
            
            base.OnPointerPressed(e);
#endif
        }
        
        /// <summary>
        /// Invoked when the Pointer entered
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            pointerover = true;
            VisualStateManager.GoToState(this, "PointerOver", true);

#if !WINRT
            base.OnMouseEnter(e);
#else
            base.OnPointerEntered(e);
#endif
        }

        /// <summary>
        /// Invoked when the Pointer exited
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (pointerpressed)
            {
                ExecutePointerReleased();
            }
            pointerover = false;
            VisualStateManager.GoToState(this, "Normal", true);
#if !WINRT
            base.OnMouseLeave(e);
            base.ReleaseMouseCapture();
#else
           
            base.OnPointerExited(e);
#endif
        }

        #endregion

        #region Virtual methods

        /// <summary>
        /// Invoked when the IsFrozen property is changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIsFrozenChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region Callback

        private static void OnIsFrozenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            HubTileBase control = sender as HubTileBase;
            if (control != null)
            {
                control.OnIsFrozenChanged(e);
            }
        }

        #endregion
    }

}
