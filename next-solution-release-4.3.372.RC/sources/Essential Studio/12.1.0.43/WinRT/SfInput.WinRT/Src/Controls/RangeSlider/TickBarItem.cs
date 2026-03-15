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

#if WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT || WINDOWS_PHONE_7 || WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Syncfusion.Tools.Controls.Input
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using System.Collections.ObjectModel;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a set of items for the <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TickBar"/> control.
    /// </summary>
    public class TickBarItem : Control
    {
        /// <summary>
        /// Creates an instance for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TickBarItem"/> class.
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls.Input"/> namespace.
        /// </summary>
        public TickBarItem()
        {
            DefaultStyleKey = typeof(TickBarItem);
#if WINRT
            this.Loaded += TickBarItem_Loaded;
#endif
        }

        void TickBarItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (tickBar != null)
            {
                UpdateLabelPosition();
            }
        }

        private void UpdateLabelPosition()
        {
            if (tickBar.Orientation == Orientation.Horizontal)
            {
                if ((tickBar.ValuePlacement == ValuePlacement.BottomRight || tickBar.LabelPlacement == LabelPlacement.BottomRight) && tickBar.Name == "BottomTickBar")
                {
                    if (PART_Text != null)
                    {
                        TransformGroup transform;
                        transform = PART_Text.RenderTransform as TransformGroup;
                        if (transform != null)
                        {
                            for (int i = 0; i < transform.Children.Count; i++)
                            {
                                if (transform.Children[i] is TranslateTransform)
                                {
                                    if (tickBar.LabelOrientation == Orientation.Horizontal)
                                    {
#if WINRT
                                        if(tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.BottomRight)
                                            (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X + (PART_TextLeft.ActualWidth);
                                        else
                                        (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X - (PART_Text.ActualWidth / 8);
#else
                                        (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X - (PART_Text.ActualWidth / 4);
#endif
                                    }
                                    else
                                    {
#if WINDOWS_PHONE_7 || WINDOWS_PHONE
                                        double x = (transform.Children[i] as TranslateTransform).X;
                                        (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).Y;
                                        (transform.Children[i] as TranslateTransform).Y = x;
                                            (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X - (PART_Text.ActualWidth);
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y - (PART_Text.ActualHeight / 3) ;
#elif SILVERLIGHT
                                        double x = (transform.Children[i] as TranslateTransform).X;
                                        (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).Y;
                                        (transform.Children[i] as TranslateTransform).Y = x;
                                            (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X - (PART_Text.ActualWidth);
                                        if(tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.BottomRight)
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y - (PART_Text.ActualHeight / 4) ;
                                       else
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y - (PART_Text.ActualHeight / 16);
#else
                                        if ((tickBar.ShowValueLabels && tickBar.ShowCustomLabels) || tickBar.ShowCustomLabels)
                                            (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X - (PART_Text.ActualHeight / 4);
                                        (transform.Children[i] as TranslateTransform).Y = PART_Text.ActualWidth;
#endif
                                    }
                                    PART_Text.RenderTransform = transform;
                                }
                            }
                        }
                    }
                }
                if ((tickBar.ValuePlacement == ValuePlacement.TopLeft || tickBar.LabelPlacement == LabelPlacement.TopLeft) && tickBar.Name == "TopTickBar")
                {
                    if (PART_TextTop != null)
                    {
                        TransformGroup transform;
                        transform = PART_TextTop.RenderTransform as TransformGroup;
                        if (transform != null)
                        {
                            for (int i = 0; i < transform.Children.Count; i++)
                            {
                                if (transform.Children[i] is TranslateTransform)
                                {
                                    if (tickBar.LabelOrientation == Orientation.Horizontal)
                                    {
#if WINRT
                                                  if(tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.BottomRight)
                                            (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X + (PART_TextLeft.ActualWidth);
                                        else
                                        (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X - (PART_Text.ActualWidth / 8);
#else
                                        (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X - (PART_TextTop.ActualWidth / 4);
#endif

                                    }
                                    else
                                    {
#if WINDOWS_PHONE_7 || WINDOWS_PHONE
                                        double x = (transform.Children[i] as TranslateTransform).X;
                                        (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).Y;
                                        (transform.Children[i] as TranslateTransform).Y = x - (PART_TextLeft.ActualHeight /2);
#elif SILVERLIGHT
                                        double x = (transform.Children[i] as TranslateTransform).X;
                                        (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).Y;
                                        (transform.Children[i] as TranslateTransform).Y = x;
#else
                                        (transform.Children[i] as TranslateTransform).X = (transform.Children[i] as TranslateTransform).X - (PART_TextTop.ActualHeight / 4);
                                        if (tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.TopLeft)
                                            (transform.Children[i] as TranslateTransform).Y = ActualHeight - 30;
#endif
                                    }
                                    PART_TextTop.RenderTransform = transform;
                                }
                            }
                        }
                    }
                }
            }

            else
            {
                if ((tickBar.ValuePlacement == ValuePlacement.BottomRight || tickBar.LabelPlacement == LabelPlacement.BottomRight) && tickBar.Name == "RightTickBar")
                {
                    if (PART_TextRight != null)
                    {
                        TransformGroup transform;
                        transform = PART_TextRight.RenderTransform as TransformGroup;
                        if (transform != null)
                        {
                            for (int i = 0; i < transform.Children.Count; i++)
                            {
                                if (transform.Children[i] is TranslateTransform)
                                {
                                    if (tickBar.LabelOrientation == Orientation.Horizontal)
                                    {
#if WINDOWS_PHONE_7 || WINDOWS_PHONE
                                        (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y - (PART_TextRight.ActualHeight / 3);
#else
                                        if (tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.BottomRight)
                                        {
#if WINRT
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y + (PART_TextRight.ActualHeight / 4);
#endif
                                        }
                                        else
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y - (PART_TextRight.ActualHeight / 4);
#endif
                                    }
                                    else
                                    {
#if WINDOWS_PHONE_7 || WINDOWS_PHONE
                                        double x = (transform.Children[i] as TranslateTransform).X;
                                               if (tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.BottomRight)
                                                   (transform.Children[i] as TranslateTransform).X = -(transform.Children[i] as TranslateTransform).Y - PART_TextRight.ActualWidth+ PART_TextRight.ActualWidth/4;
                                               else
                                                   (transform.Children[i] as TranslateTransform).X = -(transform.Children[i] as TranslateTransform).Y + PART_TextRight.ActualWidth/4;
                                               (transform.Children[i] as TranslateTransform).Y = x;
#elif  SILVERLIGHT
                                        
                                        double x = (transform.Children[i] as TranslateTransform).X;
                                               if (tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.BottomRight)
                                                   (transform.Children[i] as TranslateTransform).X = -(transform.Children[i] as TranslateTransform).Y - PART_TextRight.ActualWidth;
                                               else
                                                   (transform.Children[i] as TranslateTransform).X = -(transform.Children[i] as TranslateTransform).Y;
                                            (transform.Children[i] as TranslateTransform).Y = x;
#else
                                        if (tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.BottomRight)
                                        {
#if WINRT
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y + (PART_TextRight.ActualHeight / 2) + PART_TextRight.ActualHeight;
#else
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y + (PART_TextRight.ActualWidth / 2) + (PART_TextRight.ActualHeight / 2);
#endif
                                        }
#endif
                                    }
                                    PART_TextRight.RenderTransform = transform;
                                }
                            }
                        }
                    }
                }
                if ((tickBar.ValuePlacement == ValuePlacement.TopLeft || tickBar.LabelPlacement == LabelPlacement.TopLeft) && tickBar.Name == "LeftTickBar")
                {
                    if (PART_TextLeft != null)
                    {
                        TransformGroup transform;
                        transform = PART_TextLeft.RenderTransform as TransformGroup;
                        if (transform != null)
                        {
                            for (int i = 0; i < transform.Children.Count; i++)
                            {
                                if (transform.Children[i] is TranslateTransform)
                                {
                                    if (tickBar.LabelOrientation == Orientation.Horizontal)
                                    {
                                        if (tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.TopLeft)
                                        {
#if WINRT
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y + (PART_TextLeft.ActualHeight / 4);
#elif WINDOWS_PHONE_7 || WINDOWS_PHONE
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y - (PART_TextLeft.ActualHeight /4);
#endif
                                        }
                                        else
#if WINDOWS_PHONE_7 || WINDOWS_PHONE
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y - (PART_TextLeft.ActualHeight / 3);
#else
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y - (PART_TextLeft.ActualHeight / 4);
#endif
                                    }
                                    else
                                    {
#if SILVERLIGHT || WINDOWS_PHONE_7 || WINDOWS_PHONE
                                        if (tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.TopLeft)
                                            (transform.Children[i] as TranslateTransform).X = -(transform.Children[i] as TranslateTransform).Y - PART_TextLeft.ActualWidth + (PART_TextLeft.ActualWidth / 4);
                                        else
                                            (transform.Children[i] as TranslateTransform).X = -(transform.Children[i] as TranslateTransform).Y + (PART_TextLeft.ActualWidth/4);
                                            (transform.Children[i] as TranslateTransform).Y = tickBar.ActualWidth - 40;

#else
                                        if (tickBar.ShowCustomLabels && tickBar.LabelPlacement == LabelPlacement.TopLeft)
                                        {
#if WINRT
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y + (PART_TextLeft.ActualHeight / 2) + PART_TextLeft.ActualHeight;
#else
                                            (transform.Children[i] as TranslateTransform).Y = (transform.Children[i] as TranslateTransform).Y + (PART_TextLeft.ActualWidth / 2) + (PART_TextLeft.ActualHeight / 2);
#endif
                                            (transform.Children[i] as TranslateTransform).X = ActualWidth - 30;
                                        }
#endif
                                    }
                                    PART_TextLeft.RenderTransform = transform;
                                }
                            }
                        }
                    }
                }
            }
        }


        internal TextBlock PART_Text;

        internal TextBlock PART_TextLeft;

        internal TextBlock PART_TextRight;

        internal TextBlock PART_TextTop;

        internal Line PART_Line;

        internal Line PART_LineLeft;

        internal Line PART_LineRight;

        internal Line PART_LineTop;

        internal Grid TopPanel;

        internal Grid BottomPanel;

        internal Grid LeftPanel;

        internal Grid RightPanel;

        internal TickBar tickBar;
        
        /// <summary>
        /// Gets or sets a value for the tick
        /// </summary>
        public String Tick
        {
            get { return (String)GetValue(TickProperty); }
            set { SetValue(TickProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Tick.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickProperty =
            DependencyProperty.Register("Tick", typeof(String), typeof(TickBarItem), new PropertyMetadata(String.Empty));

        
        /// <summary>
        /// Gets or sets the value for the opacity of the line
        /// </summary>
        /// <value>
        /// Te default value is 1.0
        /// </value>
        public double LineOpacity
        {
            get { return (double)GetValue(LineOpacityProperty); }
            set { SetValue(LineOpacityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LineOpacity.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LineOpacityProperty =
            DependencyProperty.Register("LineOpacity", typeof(double), typeof(TickBarItem), new PropertyMetadata(1.0)); 

        /// <summary>
        /// Gets or sets the value for the StartPointX
        /// </summary>
        public Double StartPointX
        {
            get { return (Double)GetValue(StartPointXProperty); }
            set { SetValue(StartPointXProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StartPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartPointXProperty =
            DependencyProperty.Register("StartPointX", typeof(Double), typeof(TickBarItem), new PropertyMetadata(0d));




        /// <summary>
        /// Gets or sets the value for the StartPointY
        /// </summary>
        public Double StartPointY
        {
            get { return (Double)GetValue(StartPointYProperty); }
            set { SetValue(StartPointYProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StartPointY.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartPointYProperty =
            DependencyProperty.Register("StartPointY", typeof(Double), typeof(TickBarItem), new PropertyMetadata(0d));



        /// <summary>
        /// Gets or sets the value for the EndPointX
        /// </summary>
        public Double EndPointX
        {
            get { return (Double)GetValue(EndPointXProperty); }
            set { SetValue(EndPointXProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EndPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndPointXProperty =
            DependencyProperty.Register("EndPointX", typeof(Double), typeof(TickBarItem), new PropertyMetadata(0d));




        /// <summary>
        /// Gets or sets the value for the EndPoinY
        /// </summary>
        public Double EndPointY
        {
            get { return (Double)GetValue(EndPointYProperty); }
            set { SetValue(EndPointYProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EndPointY.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndPointYProperty =
            DependencyProperty.Register("EndPointY", typeof(Double), typeof(TickBarItem), new PropertyMetadata(0d));



        /// <summary>
        /// Gets or sets the Brush value for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TickBarItem"/> control.
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(TickBarItem), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the value for the ticks.
        /// </summary>
        public int Ticks
        {
            get { return (int)GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Ticks.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(int), typeof(TickBarItem), new PropertyMetadata(0));
        
        /// <summary>
        /// Initializes all the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TickBarItem"/> control.
        /// </summary>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#elif WINRT
        protected override void OnApplyTemplate()
#endif
        {
#if !WINRT && !SILVERLIGHT
            this.Loaded += TickBarItem_Loaded;
#endif
            PART_Text = GetTemplateChild("PART_Text") as TextBlock;
            PART_TextLeft = GetTemplateChild("PART_TextLeft") as TextBlock;
            PART_TextRight = GetTemplateChild("PART_TextRight") as TextBlock;
            PART_TextTop = GetTemplateChild("PART_TextTop") as TextBlock;
            TopPanel = GetTemplateChild("TopPanel") as Grid;
            BottomPanel = GetTemplateChild("BottomPanel") as Grid;
            RightPanel = GetTemplateChild("RightPanel") as Grid;
            LeftPanel = GetTemplateChild("LeftPanel") as Grid;
            PART_Line = GetTemplateChild("PART_Line") as Line;
            PART_LineLeft = GetTemplateChild("PART_LineLeft") as Line;
            PART_LineRight = GetTemplateChild("PART_LineRight") as Line;
            PART_LineTop = GetTemplateChild("PART_LineTop") as Line;

            if (tickBar != null)
            {
                UpdateTickVisibility();

                if (tickBar.TickPlacement != TickPlacement.Outside && tickBar.TickPlacement != TickPlacement.Inline
                    && tickBar.TickPlacement != TickPlacement.None)
                {
                    TopPanel.Visibility = Visibility.Collapsed;
                    BottomPanel.Visibility = Visibility.Collapsed;
                    LeftPanel.Visibility = Visibility.Collapsed;
                    RightPanel.Visibility = Visibility.Collapsed;
                }
                TransformGroup transformGroup = new TransformGroup();
                RotateTransform rotate = new RotateTransform();

                if (tickBar.LabelOrientation == Orientation.Horizontal)
                {
                    rotate.Angle = 0;
                }
                else
                {
                    rotate.Angle = -90;
                }         
                if (tickBar.ShowCustomLabels && (((tickBar.Name == "TopTickBar" || tickBar.Name == "LeftTickBar") && tickBar.LabelPlacement == LabelPlacement.TopLeft) ||
                    ((tickBar.Name == "BottomTickBar" || tickBar.Name == "RightTickBar") && tickBar.LabelPlacement == LabelPlacement.BottomRight)))
                {
                    if (tickBar.LabelPlacement == LabelPlacement.TopLeft)
                    {
                        PART_TextLeft.Visibility = Visibility.Visible;
                        PART_TextTop.Visibility = Visibility.Visible;
                    }
                    else if (tickBar.LabelPlacement == LabelPlacement.BottomRight)
                    {
                        PART_Text.Visibility = Visibility.Visible;
                        PART_TextRight.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        PART_Text.Visibility = Visibility.Collapsed;
                        PART_TextLeft.Visibility = Visibility.Collapsed;
                        PART_TextRight.Visibility = Visibility.Collapsed;
                        PART_TextTop.Visibility = Visibility.Collapsed;
                    }

                    if (tickBar.Orientation == Orientation.Horizontal)
                    {

                        TranslateTransform transform = new TranslateTransform();
                        transform.X = StartPointX - 5;
                        transform.Y = StartPointY;
                        if (tickBar.LabelPlacement == LabelPlacement.TopLeft)
                            TopPanel.Visibility = Visibility.Visible;
                        else
                            BottomPanel.Visibility = Visibility.Visible;

#if WINRT || WPF
                        transformGroup.Children.Add(rotate);
                        transformGroup.Children.Add(transform);
#endif
                        if (tickBar.LabelPlacement == LabelPlacement.BottomRight)
                        {
#if !WINRT && !WPF
                            transformGroup = PART_Text.RenderTransform as TransformGroup;
                            for (int i = 0; i < transformGroup.Children.Count; i++)
                            {
                                if (transformGroup.Children[i] is TranslateTransform)
                                {
                                    (transformGroup.Children[i] as TranslateTransform).X = StartPointX -5;
                                    (transformGroup.Children[i] as TranslateTransform).Y = StartPointY;
                                }
                                else
                                    transformGroup.Children[i] = rotate;
                            }
#endif
                            PART_Text.RenderTransform = transformGroup;
                        }
                        else
                        {
#if !WINRT && !WPF
                            transformGroup = PART_TextTop.RenderTransform as TransformGroup;
                            for (int i = 0; i < transformGroup.Children.Count; i++)
                            {
                                if (transformGroup.Children[i] is TranslateTransform)
                                {
                                    (transformGroup.Children[i] as TranslateTransform).X = StartPointX - 5;
                                    (transformGroup.Children[i] as TranslateTransform).Y = StartPointY;
                                }
                                else
                                    transformGroup.Children[i] = rotate;
                            }
#endif
                            PART_TextTop.RenderTransform = transformGroup;
                        }

                    }
                    else 
                    {

                        if (tickBar.LabelPlacement == LabelPlacement.TopLeft)
                            LeftPanel.Visibility = Visibility.Visible;
                        else
                            RightPanel.Visibility = Visibility.Visible;

                        TranslateTransform transform = new TranslateTransform();
                        transform.X = StartPointX;
                        transform.Y = StartPointY-10;

#if WINRT || WPF
                        transformGroup.Children.Add(rotate);
                        transformGroup.Children.Add(transform);
#endif
                        if (tickBar.LabelPlacement == LabelPlacement.BottomRight)
                        {
#if !WINRT && !WPF
                            transformGroup = PART_TextRight.RenderTransform as TransformGroup;
                            for (int i = 0; i < transformGroup.Children.Count; i++)
                            {
                                if (transformGroup.Children[i] is TranslateTransform)
                                {
                                    (transformGroup.Children[i] as TranslateTransform).X = StartPointX;
                                    (transformGroup.Children[i] as TranslateTransform).Y = StartPointY - 10;
                                }
                                else
                                    transformGroup.Children[i] = rotate;
                            }
#endif
                            PART_TextRight.RenderTransform = transformGroup;
                        }
                        else
                        {
#if !WINRT && !WPF
                            transformGroup = PART_TextLeft.RenderTransform as TransformGroup;
                            for (int i = 0; i < transformGroup.Children.Count; i++)
                            {
                                if (transformGroup.Children[i] is TranslateTransform)
                                {
                                    (transformGroup.Children[i] as TranslateTransform).X = StartPointX;
                                    (transformGroup.Children[i] as TranslateTransform).Y = StartPointY - 10;
                                }
                                else
                                    transformGroup.Children[i] = rotate;
                            }
#endif
                            PART_TextLeft.RenderTransform = transformGroup;
                        }

                    }
                }
                else
                {
                    if (tickBar.ShowValueLabels && (((tickBar.Name == "TopTickBar" || tickBar.Name == "LeftTickBar") && tickBar.ValuePlacement == ValuePlacement.TopLeft) ||
                    ((tickBar.Name == "BottomTickBar" || tickBar.Name == "RightTickBar") && tickBar.ValuePlacement == ValuePlacement.BottomRight)))
                    {
                        PART_Text.Visibility = Visibility.Visible;
                        PART_TextLeft.Visibility = Visibility.Visible;
                        PART_TextRight.Visibility = Visibility.Visible;
                        PART_TextTop.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        PART_Text.Visibility = Visibility.Collapsed;
                        PART_TextLeft.Visibility = Visibility.Collapsed;
                        PART_TextRight.Visibility = Visibility.Collapsed;
                        PART_TextTop.Visibility = Visibility.Collapsed;
                    }
                    
                    if (tickBar.Orientation == Orientation.Horizontal)
                    {
                        if (tickBar.TickPlacement == TickPlacement.TopLeft && (tickBar.Name =="TopTickBar"))
                            TopPanel.Visibility = Visibility.Visible;
                        else if(tickBar.TickPlacement != TickPlacement.None && (tickBar.Name =="BottomTickBar")) 
                            BottomPanel.Visibility = Visibility.Visible;
                        if (tickBar.TickPlacement == TickPlacement.Outside)
                        {
                            if (tickBar.Name == "BottomTickBar")
                                BottomPanel.Visibility = Visibility.Visible;
                            else
                                TopPanel.Visibility = Visibility.Visible;
                        }
                        if (tickBar.TickPlacement == TickPlacement.Inline && tickBar.Name == "HorizontalInlineTickBar")
                        {
                            BottomPanel.Visibility = Visibility.Visible;
                            Grid.SetRowSpan(PART_Line, 2);
                        }
                        else
                        {
                            Grid.SetRowSpan(PART_Line,1);
                        }

                        if (tickBar.ValuePlacement == ValuePlacement.TopLeft && tickBar.Name == "TopTickBar")
                            TopPanel.Visibility = Visibility.Visible;
                        else if(tickBar.Name == "BottomTickBar")
                            BottomPanel.Visibility = Visibility.Visible;

                        TranslateTransform transform = new TranslateTransform();
                        transform.X = StartPointX - 7;
                        transform.Y = StartPointY;


#if WINRT || WPF
                        transformGroup.Children.Add(rotate);
                        transformGroup.Children.Add(transform);
#endif
                        if (tickBar.ValuePlacement == ValuePlacement.BottomRight)
                        {
#if !WINRT && !WPF
                            transformGroup = PART_Text.RenderTransform as TransformGroup;
                            for (int i = 0; i < transformGroup.Children.Count; i++)
                            {
                                if (transformGroup.Children[i] is TranslateTransform)
                                {
                                    (transformGroup.Children[i] as TranslateTransform).X = StartPointX - 7;
                                    (transformGroup.Children[i] as TranslateTransform).Y = StartPointY;
                                }
                                else
                                    transformGroup.Children[i] = rotate;
                            }
#endif
                            PART_Text.RenderTransform = transformGroup;
                        }
                        else
                        {
#if !WINRT && !WPF
                            transformGroup = PART_TextTop.RenderTransform as TransformGroup;
                            for (int i = 0; i < transformGroup.Children.Count; i++)
                            {
                                if (transformGroup.Children[i] is TranslateTransform)
                                {
                                    (transformGroup.Children[i] as TranslateTransform).X = StartPointX - 7;
                                    (transformGroup.Children[i] as TranslateTransform).Y = StartPointY;
                                }
                                else
                                    transformGroup.Children[i] = rotate;
                            }
#endif
                            PART_TextTop.RenderTransform = transformGroup;
                            PART_TextTop.RenderTransformOrigin = new Point(0.5, 0.5);
                        }

                    }
                    else
                    {

                        if (tickBar.TickPlacement == TickPlacement.TopLeft && (tickBar.Name == "LeftTickBar"))
                            LeftPanel.Visibility = Visibility.Visible;
                        else if (tickBar.TickPlacement != TickPlacement.None && (tickBar.Name == "RightTickBar"))
                            RightPanel.Visibility = Visibility.Visible;
                        if (tickBar.TickPlacement == TickPlacement.Outside)
                        {
                            if (tickBar.Name == "LeftTickBar")
                                LeftPanel.Visibility = Visibility.Visible;
                            else
                                RightPanel.Visibility = Visibility.Visible;
                        }
                        if (tickBar.TickPlacement == TickPlacement.Inline && tickBar.Name == "VerticalInlineTickBar")
                        {
                            RightPanel.Visibility = Visibility.Visible;
                            Grid.SetRowSpan(PART_LineRight, 2);
                        }
                        else
                        {
                            Grid.SetRowSpan(PART_LineRight, 1);
                        }

                        if (tickBar.ValuePlacement == ValuePlacement.TopLeft && tickBar.Name == "LeftTickBar")
                            LeftPanel.Visibility = Visibility.Visible;
                        else if(tickBar.Name == "RightTickBar")
                            RightPanel.Visibility = Visibility.Visible;

                        TranslateTransform transform = new TranslateTransform();
                        transform.X = StartPointX;
                        transform.Y = StartPointY - 5;


#if WINRT || WPF
                        transformGroup.Children.Add(rotate);
                        transformGroup.Children.Add(transform);
#endif
                        if (tickBar.ValuePlacement == ValuePlacement.BottomRight)
                        {
#if !WINRT && !WPF
                            transformGroup = PART_TextRight.RenderTransform as TransformGroup;
                            for (int i = 0; i < transformGroup.Children.Count; i++)
                            {
                                if (transformGroup.Children[i] is TranslateTransform)
                                {
                                    (transformGroup.Children[i] as TranslateTransform).X = StartPointX;
                                    (transformGroup.Children[i] as TranslateTransform).Y = StartPointY-5;
                                }
                                else
                                    transformGroup.Children[i] = rotate;
                            }
#endif
                            PART_TextRight.RenderTransform = transformGroup;
                            PART_TextRight.RenderTransformOrigin = new Point(0.5, 0.5);
                        }
                        else
                        {
#if !WINRT && !WPF
                            transformGroup = PART_TextLeft.RenderTransform as TransformGroup;
                            for (int i = 0; i < transformGroup.Children.Count; i++)
                            {
                                if (transformGroup.Children[i] is TranslateTransform)
                                {
                                    (transformGroup.Children[i] as TranslateTransform).X = StartPointX;
                                    (transformGroup.Children[i] as TranslateTransform).Y = StartPointY - 5;
                                }
                                else
                                    transformGroup.Children[i] = rotate;
                            }
#endif
                            PART_TextLeft.RenderTransform = transformGroup;
                            PART_TextLeft.RenderTransformOrigin = new Point(0.5, 0.5);
                        }

                    }
                }
                UpdateLabelPosition();
            }
            base.OnApplyTemplate();
        }

        private void UpdateTickVisibility()
        {
            if (PART_Line != null && PART_LineLeft != null && PART_LineRight != null && PART_LineTop != null)
            {
                PART_Line.Visibility = Visibility.Collapsed;
                PART_LineLeft.Visibility = Visibility.Collapsed;
                PART_LineRight.Visibility = Visibility.Collapsed;
                PART_LineTop.Visibility = Visibility.Collapsed;
                if (tickBar != null)
                {
                    if (tickBar.Orientation == Orientation.Horizontal)
                    {
                        if (tickBar.TickPlacement == TickPlacement.BottomRight ||
                            (tickBar.TickPlacement == TickPlacement.Inline && tickBar.Name == "HorizontalInlineTickBar"))
                        {
                            PART_Line.Visibility = Visibility.Visible;
                        }
                        else if (tickBar.TickPlacement == TickPlacement.TopLeft)
                        {
                            PART_LineTop.Visibility = Visibility.Visible;
                        }
                        else if (tickBar.TickPlacement == TickPlacement.Outside)
                        {
                            PART_Line.Visibility = Visibility.Visible;
                            PART_LineTop.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        if (tickBar.TickPlacement == TickPlacement.BottomRight ||
                            (tickBar.TickPlacement == TickPlacement.Inline && tickBar.Name == "VerticalInlineTickBar"))
                        {
                            PART_LineRight.Visibility = Visibility.Visible;
                        }
                        else if (tickBar.TickPlacement == TickPlacement.TopLeft)
                        {
                            PART_LineLeft.Visibility = Visibility.Visible;
                        }
                        else if (tickBar.TickPlacement == TickPlacement.Outside)
                        {
                            PART_LineRight.Visibility = Visibility.Visible;
                            PART_LineLeft.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }
    }
}
