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
using System.Windows.Media;
using Syncfusion.Windows.GridCommon;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Controls.Primitives;

#if SILVERLIGHT
namespace Syncfusion.Silverlight.Controls.PivotGrid
#else 
namespace Syncfusion.Windows.Controls.PivotGrid
#endif
{
#if SILVERLIGHT
    /// <summary>
    /// Defines the direction of the drag indicator.
    /// </summary>
    public enum ArrowIndicatorDirection
    {
        Up,
        Down
    }
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class AnimatedGrid:ContentControl
    {
        public AnimatedGrid()
        {
            this.DefaultStyleKey = typeof(AnimatedGrid);
        }

        public static readonly DependencyProperty InnerBrushProperty = DependencyProperty.Register("InnerBrush", typeof(Brush), typeof(AnimatedGrid), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush InnerBrush
        {
            get
            {
                return (Brush)this.GetValue(AnimatedGrid.InnerBrushProperty);
            }

            set
            {
                this.SetValue(AnimatedGrid.InnerBrushProperty, value);
            }
        }

        private bool isTemplateApplied = false;
        private Border upBorder = null;
        private Border downBorder = null;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            if (this.IndicatorVisibility == Visibility.Visible)
            {
                SetVisualState();
            }

            this.upBorder = this.GetTemplateChild("PART_UpBorder") as Border;
            this.downBorder = this.GetTemplateChild("PART_DownBorder") as Border;
            SetBorderVisibility(this, this.ArrowIndicatorDirection);
        }

        private void SetVisualState()
        {
            if (this.ArrowIndicatorDirection == ArrowIndicatorDirection.Up)
            {
                VisualStateManager.GoToState(this, "UpIndicatorState", false);
            }
            else if (this.ArrowIndicatorDirection == ArrowIndicatorDirection.Down)
            {
                VisualStateManager.GoToState(this, "DownIndicatorState", false);
            }
        }

        public static readonly DependencyProperty IndicatorVisibilityProperty = DependencyProperty.Register(
            "IndicatorVisibility",
            typeof(Visibility),
            typeof(AnimatedGrid),
            new PropertyMetadata(Visibility.Collapsed, OnIndicatorVisibilityChanged));

        private static void OnIndicatorVisibilityChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as AnimatedGrid;
            if (!grid.isTemplateApplied)
            {
                return;
            }

            var visibility = (Visibility)args.NewValue;
            if (visibility == Visibility.Visible)
            {
                grid.SetVisualState();
            }
            else
            {
                VisualStateManager.GoToState(grid, "Normal", false);
            }
        }

        public Visibility IndicatorVisibility
        {
            get
            {
                return (Visibility)this.GetValue(AnimatedGrid.IndicatorVisibilityProperty);
            }

            set
            {
                this.SetValue(AnimatedGrid.IndicatorVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty ArrowIndicatorDirectionProperty = DependencyProperty.Register(
            "ArrowIndicatorDirection",
            typeof(ArrowIndicatorDirection),
            typeof(AnimatedGrid),
            new PropertyMetadata(OnArrowIndicatorDirectionChanged));

        private static void OnArrowIndicatorDirectionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var animatedGrid = dpo as AnimatedGrid;
            if (animatedGrid.upBorder == null || animatedGrid.downBorder == null || !animatedGrid.isTemplateApplied)
            {
                return;
            }

            var direction = (ArrowIndicatorDirection)args.NewValue;
            SetBorderVisibility(animatedGrid, direction);
        }

        private static void SetBorderVisibility(AnimatedGrid animatedGrid, ArrowIndicatorDirection direction)
        {
            if (direction == ArrowIndicatorDirection.Up)
            {
                animatedGrid.upBorder.Visibility = Visibility.Visible;
                animatedGrid.downBorder.Visibility = Visibility.Collapsed;
            }
            else if (direction == ArrowIndicatorDirection.Down)
            {
                animatedGrid.upBorder.Visibility = Visibility.Collapsed;
                animatedGrid.downBorder.Visibility = Visibility.Visible;
            }
        }

        public ArrowIndicatorDirection ArrowIndicatorDirection
        {
            get
            {
                return (ArrowIndicatorDirection)this.GetValue(AnimatedGrid.ArrowIndicatorDirectionProperty);
            }

            set
            {
                this.SetValue(AnimatedGrid.ArrowIndicatorDirectionProperty, value);
            }
        }
    }

    public class PopupPositionWindow
    {
        private Popup popupWindow;
       /// <summary>
        /// Initializes the <see cref="PopupPositionWindow"/> class.
        /// </summary>
        public PopupPositionWindow()
        {
            this.popupWindow = new Popup() { IsHitTestVisible = false };
        }

        public FrameworkElement Child
        {
            get
            {
                return (FrameworkElement)this.popupWindow.Child;
            }
            set
            {
                var element = value;
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }

                this.popupWindow.Child = element;
                this.popupWindow.Width = element.Width > 0 ? element.Width : 100;
                this.popupWindow.Height = element.Height > 0 ? element.Height : 100;
            }
        }

        /// <summary>
        /// Hides the popup.
        /// </summary>
        public void Hide()
        {
            this.popupWindow.IsOpen = false;
            this.popupWindow.Visibility = Visibility.Collapsed;
        }

        private void Show()
        {
            this.popupWindow.IsOpen = true;
            this.popupWindow.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Moves the popup to the specified point.
        /// </summary>
        /// <param name="p">The target point.</param>
        public void Move(Point p)
        {
            this.Show();
            this.popupWindow.HorizontalOffset = p.X;
            this.popupWindow.VerticalOffset = p.Y;
        }
    }

#else


    /// <summary>
    /// Defines the direction of the drag indicator.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Enables dragging in Up direction
        /// </summary>
        Up,
        /// <summary>
        /// Enables dragging in down direction
        /// </summary>
        Down,
        /// <summary>
        /// Enables dragging in cross direction
        /// </summary>
        Cross
    }
    
   /// <summary>
   ///  A Grid derived class that aggregates the functionality of a Animated grid object into a Grid
   /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class AnimatedGrid : System.Windows.Controls.Grid
    {
        /// <summary>
        /// Initializes the <see cref="AnimatedGrid"/> class.
        /// </summary>
        public AnimatedGrid()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.InnerBrush = GridUtil.GetXamlConvertedValue<Brush>("#FFFFFFFF");
                this.OuterBrush = GridUtil.GetXamlConvertedValue<Brush>("#FF000000");
            }
            this.Direction = Direction.Up;
        }

        private Brush outerBrush;
        /// <summary>
        /// Gets or sets the outer brush of the animated grid
        /// </summary>
        public Brush OuterBrush
        {
            get
            {
                return this.outerBrush;
            }

            set
            {
                if (this.outerBrush != value)
                {
                    this.outerBrush = value;
                    if (this.border != null)
                    {
                        // refresh
                        this.border.Background = this.GetBrush();
                    }
                }
            }
        }

        private Brush innerBrush;
        /// <summary>
        /// Gets or sets the inner brush of the Animated grid
        /// </summary>
        public Brush InnerBrush
        {
            get
            {
                return this.innerBrush;
            }

            set
            {
                if (this.innerBrush != value)
                {
                    this.innerBrush = value;
                    if (this.border != null)
                    {
                        // refresh
                        this.border.Background = this.GetBrush();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the direction of drag indicator
        /// </summary>
        public Direction Direction
        {
            get;
            set;
        }

        private Brush GetBrush()
        {
            Brush arrowBrush = null;
            if (this.Direction == Direction.Up)
            {
                //    <DrawingBrush x:Key="UpArrow">
                //    <DrawingBrush.Drawing>
                //        <DrawingGroup>
                //            <DrawingGroup.Children>
                //                <GeometryDrawing Brush="#FF000000" Geometry="F1 M 411.741,305.165L 407.204,300.027L 402.667,294.889L 398.129,300.027L 393.592,305.165L 398,305.165L 398,310.444L 406.667,310.444L 406.667,305.165L 411.741,305.165 Z "/>
                //                <GeometryDrawing Brush="#FFFFFFFF" Geometry="F1 M 405.332,309.112L 399.335,309.112L 399.335,305.164L 399.335,303.83L 398.001,303.83L 396.547,303.83L 399.126,300.909L 402.667,296.903L 406.204,300.909L 408.784,303.83L 406.665,303.83L 405.332,303.83L 405.332,305.164L 405.332,309.112 Z "/>
                //            </DrawingGroup.Children>
                //        </DrawingGroup>
                //    </DrawingBrush.Drawing>
                //</DrawingBrush>
                arrowBrush = new DrawingBrush()
                {
                    Drawing = new DrawingGroup()
                    {
                        Children = new DrawingCollection()  
                    {
                        new GeometryDrawing()
                        {
                            Brush = this.OuterBrush,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 411.741,305.165L 407.204,300.027L 402.667,294.889L 398.129,300.027L 393.592,305.165L 398,305.165L 398,310.444L 406.667,310.444L 406.667,305.165L 411.741,305.165 Z ")
                        },
                        new GeometryDrawing()
                        {
                            Brush = this.InnerBrush,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 405.332,309.112L 399.335,309.112L 399.335,305.164L 399.335,303.83L 398.001,303.83L 396.547,303.83L 399.126,300.909L 402.667,296.903L 406.204,300.909L 408.784,303.83L 406.665,303.83L 405.332,303.83L 405.332,305.164L 405.332,309.112 Z ")
                        }
                    }
                    }
                };
            }
            else if (this.Direction == Direction.Down)
            {
                //<DrawingBrush x:Key="Layer_1">
                //    <DrawingBrush.Drawing>
                //        <DrawingGroup>
                //            <DrawingGroup.Children>
                //                <GeometryDrawing Brush="#FF000000" Geometry="F1 M 411.741,300.168L 407.204,305.306L 402.667,310.444L 398.129,305.306L 393.592,300.168L 398,300.168L 398,294.889L 406.667,294.889L 406.667,300.168L 411.741,300.168 Z "/>
                //                <GeometryDrawing Brush="#FFFFFFFF" Geometry="F1 M 405.332,296.222L 399.335,296.222L 399.335,300.169L 399.335,301.503L 398.001,301.503L 396.547,301.503L 399.126,304.424L 402.667,308.43L 406.204,304.424L 408.784,301.503L 406.665,301.503L 405.332,301.503L 405.332,300.169L 405.332,296.222 Z "/>
                //            </DrawingGroup.Children>
                //        </DrawingGroup>
                //    </DrawingBrush.Drawing>
                //</DrawingBrush>
                arrowBrush = new DrawingBrush()
                {
                    Drawing = new DrawingGroup()
                    {
                        Children = new DrawingCollection()
                    {
                        new GeometryDrawing()
                        {
                            Brush = this.OuterBrush,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 411.741,300.168L 407.204,305.306L 402.667,310.444L 398.129,305.306L 393.592,300.168L 398,300.168L 398,294.889L 406.667,294.889L 406.667,300.168L 411.741,300.168 Z ")
                        },
                        new GeometryDrawing()
                        {
                            Brush = this.InnerBrush,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 405.332,296.222L 399.335,296.222L 399.335,300.169L 399.335,301.503L 398.001,301.503L 396.547,301.503L 399.126,304.424L 402.667,308.43L 406.204,304.424L 408.784,301.503L 406.665,301.503L 405.332,301.503L 405.332,300.169L 405.332,296.222 Z ")
                        }
                    }
                    }
                };
            }
            else if (this.Direction == Direction.Cross)
            {
                arrowBrush = new DrawingBrush()
                {
                    Drawing = new DrawingGroup()
                    {
                        Children = new DrawingCollection()
                    {
                        new GeometryDrawing()
                        {
                            Brush = this.OuterBrush,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("M1.5768458,0 L4.5866461,3.0098004 L7.5964422,3.5762787E-06 L9.1732903,1.5768521 L6.1634951,4.5866489 L9.1732941,7.5964479 L7.5964479,9.1732941 L4.5866489,6.1634951 L1.5768571,9.1732874 L8.3446503E-06,7.5964384 L3.0098002,4.5866461 L0,1.5768461 z") //M3.875,0 L5.125,0 5.125,3.875 9,3.875 9,5.125 5.125,5.125 5.125,9 3.875,9 3.875,5.125 0,5.125 0,3.875 3.875,3.875 3.875,0 z ")
                        }
                    }
                    }
                };
            }
           
            return arrowBrush;
        }

        private Border border = null;

        private Storyboard storyBoard = null;

        private void CreateStoryBoard()
        {
            var grid = new System.Windows.Controls.Grid()
            {
                Width = 12,
                Height = 11,
            };
            //<Border.RenderTransform>
            //       <TransformGroup>
            //           <ScaleTransform ScaleX="1" ScaleY="1"/>
            //           <SkewTransform AngleX="0" AngleY="0"/>
            //           <RotateTransform Angle="0"/>
            //           <TranslateTransform X="0" Y="0"/>
            //       </TransformGroup>
            //   </Border.RenderTransform>
            border = new Border()
            {
                RenderTransform = new TransformGroup()
                {
                    Children = new TransformCollection()
                    {
                        new ScaleTransform()
                        {
                            ScaleX = 1,
                            ScaleY = 1
                        },
                        new SkewTransform()
                        {
                            AngleX = 0,
                            AngleY = 0
                        },
                        new RotateTransform()
                        {
                            Angle = 0
                        },
                        new TranslateTransform()
                        {
                            X = 0,
                            Y = 0
                        }
                    }
                }
            };

            border.Background = this.GetBrush();
            grid.Children.Add(border);
            this.Children.Add(grid);
            //    <Storyboard x:Key="Storyboard1" RepeatBehavior="Forever" AutoReverse="True">
            //    <DoubleAnimationUsingKeyFrames BeginTime="00:00:00" Storyboard.TargetName="border" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)">
            //        <SplineDoubleKeyFrame KeyTime="00:00:02" Value="-10"/>
            //        <SplineDoubleKeyFrame KeyTime="00:00:04" Value="0"/>
            //    </DoubleAnimationUsingKeyFrames>
            //</Storyboard>
            DoubleAnimationUsingKeyFrames frame1 = null;
            if (this.Direction == Direction.Up)
            {
                frame1 = new DoubleAnimationUsingKeyFrames()
                {
                    BeginTime = GridUtil.GetXamlConvertedValue<TimeSpan>("00:00:00.1"),
                    KeyFrames = new DoubleKeyFrameCollection()
                    {
                        new SplineDoubleKeyFrame()
                        {
                            KeyTime = GridUtil.GetXamlConvertedValue<KeyTime>("00:00:00.5"),
                            Value = -5d
                        },
                        new SplineDoubleKeyFrame()
                        {
                            KeyTime = GridUtil.GetXamlConvertedValue<KeyTime>("00:00:01"),
                            Value = 0
                        }
                    }
                };
                Storyboard.SetTarget(frame1, border);
                Storyboard.SetTargetProperty(frame1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            }
            else if (this.Direction == Direction.Down)
            {
                //<Storyboard x:Key="Storyboard2" AutoReverse="True" RepeatBehavior="Forever">
                //    <DoubleAnimationUsingKeyFrames BeginTime="00:00:00" Storyboard.TargetName="border" Storyboard.TargetProperty="(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)">
                //        <SplineDoubleKeyFrame KeyTime="00:00:02" Value="38"/>
                //        <SplineDoubleKeyFrame KeyTime="00:00:04" Value="-36"/>
                //    </DoubleAnimationUsingKeyFrames>
                //</Storyboard>

                frame1 = new DoubleAnimationUsingKeyFrames()
                {
                    BeginTime = GridUtil.GetXamlConvertedValue<TimeSpan>("00:00:00.1"),
                    KeyFrames = new DoubleKeyFrameCollection()
                    {
                        new SplineDoubleKeyFrame()
                        {
                            KeyTime = GridUtil.GetXamlConvertedValue<KeyTime>("00:00:00.5"),
                            Value = 5d
                        },
                        new SplineDoubleKeyFrame()
                        {
                            KeyTime = GridUtil.GetXamlConvertedValue<KeyTime>("00:00:01"),
                            Value = -5d
                        }
                    }
                };
                Storyboard.SetTarget(frame1, border);
                Storyboard.SetTargetProperty(frame1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            }
            else if (this.Direction == Direction.Cross)
            {
                frame1 = new DoubleAnimationUsingKeyFrames() { BeginTime = GridUtil.GetXamlConvertedValue<TimeSpan>("00:00:00.1")};
                Storyboard.SetTarget(frame1, border);
                Storyboard.SetTargetProperty(frame1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            }

            storyBoard = new Storyboard()
            {
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true,
                Children = new TimelineCollection()
                {
                    frame1
                }
            };
        }
        /// <summary>
        /// Method to create the story board to view the Animated grid
        /// </summary>
        public void Begin()
        {
            if (this.storyBoard == null)
            {
                this.CreateStoryBoard();

            }

             this.storyBoard.Begin();
        }
        /// <summary>
        /// Method to stop the StoryBoard to hide the Animated Grid from display
        /// </summary>
        public void Stop()
        {
            if (this.storyBoard == null)
            {
                return;
            }

            this.storyBoard.Stop();
        }
    }
#endif
}