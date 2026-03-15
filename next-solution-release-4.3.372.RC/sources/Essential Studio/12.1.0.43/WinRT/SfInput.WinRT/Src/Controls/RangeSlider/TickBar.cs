// <copyright file="TickBar.cs" company="Syncfusion">
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

#if !(WPFSILVERLIGHT||WINDOWS_PHONE_7)
using Windows.UI;
#endif
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
using System.Collections.ObjectModel;

namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
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
    /// Represents a TickBar Control
    /// </summary>
    [ClassReference(IsReviewed = false,ShouldInclude=false)]
    public class TickBar : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TickBar"/> class.
        /// </summary>
        public TickBar()
        {
            DefaultStyleKey = typeof(TickBar);
            this.Loaded += TickBar_Loaded;
            this.SizeChanged += TickBar_SizeChanged;
        }

        void TickBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateLayout();
            CreateTicks();
        }

        #endregion

        #region Variables

        private Grid PART_Layout;        

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the maximum value that is possible for a tick mark.
        /// </summary>
        /// <value>The maximum.</value>
        [ClassReference(IsReviewed = false)]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(TickBar), new PropertyMetadata(0.0d,new PropertyChangedCallback(OnMaximumChanged)));

        /// <summary>
        /// Gets or sets the minimum value that is possible for a tick mark. 
        /// </summary>
        /// <value>The minimum.</value>
        [ClassReference(IsReviewed = false)]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
           DependencyProperty.Register("Minimum", typeof(double), typeof(TickBar), new PropertyMetadata(0.0d,new PropertyChangedCallback(OnMinimumChanged)));

        /// <summary>
        /// Gets or sets the interval between tick marks. 
        /// </summary>
        /// <value>The tick frequency.</value>
        [ClassReference(IsReviewed = false)]
        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickFrequencyProperty =
           DependencyProperty.Register("TickFrequency", typeof(double), typeof(TickBar), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnTickFrequencyChanged)));

        /// <summary>
        /// Gets or sets the Brush that is used to draw the tick marks. 
        /// </summary>
        /// <value>The fill.</value>
        [ClassReference(IsReviewed = false)]
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FillProperty =
           DependencyProperty.Register("Fill", typeof(Brush), typeof(TickBar), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Gets or sets the positions of the tick marks. 
        /// </summary>
        /// <value>The ticks.</value>
        [ClassReference(IsReviewed = false)]   
        public DoubleCollection Ticks
        {
            get { return (DoubleCollection)GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(TickBar), new PropertyMetadata(null, new PropertyChangedCallback(OnTicksCollectionChanged)));

        /// <summary>
        /// Gets or sets the tick bar orientation.
        /// </summary>
        /// <value>The tick bar orientation.</value>
        [ClassReference(IsReviewed = false)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(TickBar), new PropertyMetadata(Orientation.Horizontal,new PropertyChangedCallback(OnOrientationChanged)));


        /// <summary>
        /// Gets or sets the tickplacement for the ticks <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TickBar"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TickPlacement"/>
        /// </summary>
        public TickPlacement TickPlacement
        {
            get { return (TickPlacement)GetValue(TickPlacementProperty); }
            set { SetValue(TickPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickPlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(TickBar), new PropertyMetadata(TickPlacement.Inline,OnTickPlacementChanged));



        /// <summary>
        /// Returns a value when the direction is reversed
        /// </summary>
        /// <remarks> The default value is false </remarks>
        /// <value>
        /// <c>true</c> if instance is created; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirectionReversed
        {
            get { return (bool)GetValue(IsDirectionReversedProperty); }
            set { SetValue(IsDirectionReversedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDirectionReversed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDirectionReversedProperty =
            DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(TickBar), new PropertyMetadata(false,OnIsDirectionReversed));



        /// <summary>
        /// Returns a value to show Value labels
        /// </summary>
        /// <remarks> The default value is false </remarks>
        /// <value>
        /// <c>true</c> if instance is created; otherwise, <c>false</c>.
        /// </value>
        public bool ShowValueLabels
        {
            get { return (bool)GetValue(ShowValueLabelsProperty); }
            set { SetValue(ShowValueLabelsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowValueLabels.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowValueLabelsProperty =
            DependencyProperty.Register("ShowValueLabels", typeof(bool), typeof(TickBar), new PropertyMetadata(false, OnShowValueLabelsChanged));


        /// <summary>
        /// Returns a value to show custom labels
        /// </summary>
        /// <remarks> The default value is false </remarks>
        /// <value>
        /// <c>true</c> if instance is created; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCustomLabels
        {
            get { return (bool)GetValue(ShowCustomLabelsProperty); }
            set { SetValue(ShowCustomLabelsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowCustomLabels.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowCustomLabelsProperty =
            DependencyProperty.Register("ShowCustomLabels", typeof(bool), typeof(TickBar), new PropertyMetadata(false, OnShowCustomLabelsChanged));



        /// <summary>
        /// Returns a value on setting the label orientation.
        /// <see cref="T:Windows.UI.Xaml.Controls.Orientation"/>
        /// </summary>
        public Orientation LabelOrientation
        {
            get { return (Orientation)GetValue(LabelOrientationProperty); }
            set { SetValue(LabelOrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelOrientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelOrientationProperty =
            DependencyProperty.Register("LabelOrientation", typeof(Orientation), typeof(TickBar), new PropertyMetadata(Orientation.Horizontal,OnLabelOrientationChanged));      

        

        /// <summary>
        /// Returns the position of the placement of the labels
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.LabelPlacement"/>
        /// </summary>
        public LabelPlacement LabelPlacement
        {
            get { return (LabelPlacement)GetValue(LabelPlacementProperty); }
            set { SetValue(LabelPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelPlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelPlacementProperty =
            DependencyProperty.Register("LabelPlacement", typeof(LabelPlacement), typeof(TickBar), new PropertyMetadata(LabelPlacement.TopLeft,OnLabelPlacementChanged));


        /// <summary>
        /// Returns the position of the placement of the values
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.ValuePlacement"/>
        /// </summary>
        public ValuePlacement ValuePlacement
        {
            get { return (ValuePlacement)GetValue(ValuePlacementProperty); }
            set { SetValue(ValuePlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ValuePlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValuePlacementProperty =
            DependencyProperty.Register("ValuePlacement", typeof(ValuePlacement), typeof(TickBar), new PropertyMetadata(ValuePlacement.BottomRight,OnValuePlacementChanged));   

        
        /// <summary>
        /// Returns a list of custom labels
        /// </summary>
        public ObservableCollection<Items> CustomLabels
        {
            get { return (ObservableCollection<Items>)GetValue(CustomLabelsProperty); }
            set { SetValue(CustomLabelsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CustomLabels.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CustomLabelsProperty =
            DependencyProperty.Register("CustomLabels", typeof(ObservableCollection<Items>), typeof(TickBar), new PropertyMetadata(null,OnCustomLabelsChanged));      


        #endregion

        #region Helper Methods

        void TickBar_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();
            CreateTicks();
        }

        internal double GetTransform(double newValue)
        {
            double factor = (newValue - Minimum) / (Maximum - Minimum);
            double transform = 0d;
            if (factor >= 0d)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (IsDirectionReversed)
                        transform = (1 - factor) * 2 + (ActualWidth / (Maximum - Minimum)) * (newValue - Minimum);
                    else
                    transform = (ActualWidth / (Maximum - Minimum)) * (newValue - Minimum) - (factor * 2);
                }
                else
                {
                    if (IsDirectionReversed)
                        transform = (1 - factor) * 2 + (ActualHeight / (Maximum - Minimum)) * (newValue - Minimum);
                    else
                    transform = (ActualHeight / (Maximum - Minimum)) * (newValue - Minimum) - (factor * 2);
                }
            }
            return transform;
        }
        internal void CreateTickBar(int tickFrequency)
        {
            if (PART_Layout != null && tickFrequency > 0)
            {
                double x1 = 0;

                PART_Layout.Children.Clear();

                int numberoftikcs = tickFrequency;
                x1 = UpdateTickLabel(x1, numberoftikcs);
            }
            else
            {
                if (PART_Layout != null)
                PART_Layout.Children.Clear();
            }
        }

        private double UpdateTickLabel(double x1, int numberoftikcs)
        {
            double X1 = 0;
            for (int i = 0; i <= numberoftikcs; i++)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (!IsDirectionReversed)
                        x1 =((i) * ((ActualWidth - 1) / numberoftikcs));
                    else
                        x1 = ActualWidth - (((i) * ((ActualWidth - 1) / numberoftikcs)));
                    TickBarItem panel = new TickBarItem();
                    panel.Ticks = numberoftikcs + 1;
                    panel.StartPointX = x1;
                 
                        panel.StartPointY = 0;
                    panel.EndPointX = x1;
                    panel.EndPointY = 5;
                    if (TickPlacement == TickPlacement.Inline)
                        panel.EndPointY = 10;
                    panel.Fill = Fill;
                    UpdateTickValue(i, panel);
                    PART_Layout.Children.Add(panel);

                    panel.tickBar = this;
                }
                else
                {
                    if (!IsDirectionReversed)
                        x1 = ((i) * ((ActualHeight-1) / numberoftikcs));
                    else
                        x1 = ActualHeight - (((i) * ((ActualHeight-1) / numberoftikcs)));
                    TickBarItem panel = new TickBarItem();
                    panel.Ticks = numberoftikcs + 1;
                    panel.StartPointX = 0;
                    panel.StartPointY = x1;
                    panel.EndPointX = 5;
                    if (TickPlacement == TickPlacement.Inline)
                        panel.EndPointX = 10;
                    panel.EndPointY = x1;
                    panel.Fill = Fill;
                    UpdateTickValue(i, panel);
                    PART_Layout.Children.Add(panel);

                    panel.tickBar = this;
                }
            }
            if (ShowCustomLabels && (((this.Name == "TopTickBar" || this.Name == "LeftTickBar") && LabelPlacement == LabelPlacement.TopLeft)|| ((this.Name == "BottomTickBar" || this.Name == "RightTickBar") && LabelPlacement == LabelPlacement.BottomRight)))
            {
                foreach (Items item in CustomLabels)
                {
                    if (item != null && item.value >= Minimum && item.value <= Maximum)
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            if (!IsDirectionReversed)
                                X1 = (GetTransform(item.value));
                            else
                                X1 = ActualWidth - GetTransform(item.value);
                            TickBarItem panel = new TickBarItem();
                            panel.StartPointX = X1;
                            panel.StartPointY = 0;
                            panel.EndPointX = X1;
                            panel.EndPointY = 5;
                            panel.Tick = item.label;
                            panel.LineOpacity = 0;
                            panel.Fill = Fill;
                            PART_Layout.Children.Add(panel);

                            panel.tickBar = this;
                        }
                        else
                        {
                            if (IsDirectionReversed)
                                X1 = (GetTransform(item.value));
                            else
                                X1 = ActualHeight - GetTransform(item.value);
                            TickBarItem panel = new TickBarItem();
                            panel.StartPointX = 0;
                            panel.StartPointY = X1;
                            panel.EndPointX = 5;
                            panel.EndPointY = X1;
                            panel.Tick = item.label;
                            panel.Fill = Fill;
                            panel.LineOpacity = 0;
                            PART_Layout.Children.Add(panel);
                            panel.tickBar = this;
                        }
                    }
                }
            }
            return x1;
        }

        private void UpdateTickValue(int i, TickBarItem panel)
        {
            double tickValue;
            if (((this.Name == "TopTickBar" || this.Name == "LeftTickBar") && ValuePlacement == ValuePlacement.TopLeft) ||
     ((this.Name == "BottomTickBar" || this.Name == "RightTickBar") && ValuePlacement == ValuePlacement.BottomRight))
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (i == 0)
                        tickValue = Minimum;
                    else
                        tickValue = (Minimum + (i * TickFrequency));
                }
                else
                {
                    if (i == 0)
                        tickValue = Maximum;
                    else
                        tickValue = (Maximum - (i * TickFrequency));
                }

                if (ShowCustomLabels)
                {
                    foreach (Items item in CustomLabels)
                    {
                        if (item != null && item.value == tickValue)
                        {
                            if ((LabelPlacement == LabelPlacement.TopLeft && ValuePlacement == ValuePlacement.TopLeft) ||
                                (LabelPlacement == LabelPlacement.BottomRight && ValuePlacement == ValuePlacement.BottomRight))
                                panel.Tick = String.Empty;
                            else
                                panel.Tick = tickValue.ToString();
                            return;
                        }
                        
                    }
                    if (ShowValueLabels)
                    {
                        foreach (Items item in CustomLabels)
                        {
                            if(item != null && tickValue !=item.value)
                            panel.Tick = tickValue.ToString();
                        }
                    }
                    else
                        panel.Tick = String.Empty;
                }
                else if (ShowValueLabels)
                    panel.Tick = tickValue.ToString();
                else
                    panel.Tick = String.Empty;
            }
            else
                panel.Tick = String.Empty;
        }
        /// <summary>
        /// Create the specified number of ticks
        /// </summary>
        public void CreateTicks()
        {
            int numberofticks = (int)((Maximum - Minimum) / TickFrequency);
            if (TickFrequency != 0)
                CreateTickBar(numberofticks);
        }

        private Line Drawline(Point startPoint, Point endPoint)
        {
            Line ln = new Line();
            ln.Stroke = Fill;
#if WPFSILVERLIGHT
            ln.StrokeThickness = 2.2;
#else
            ln.StrokeThickness = 1;
#endif
            ln.X1 = startPoint.X;
            ln.Y1 = startPoint.Y;
            ln.X2 = endPoint.X;
            ln.Y2 = endPoint.Y;
            return ln;
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes all the child elements of the control
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.TickBar"/>
        /// </summary>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#elif WINRT
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            PART_Layout = GetTemplateChild("PART_Layout") as Grid;          
        }

        #endregion

        #region Callback Methods

        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TickBar control = sender as TickBar;
            control.CreateTicks();
        }
        private static void OnMinimumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();  
        }
        private static void OnMaximumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();  
        }
        private static void OnTickFrequencyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();  
        }


        private static void OnIsDirectionReversed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();
        }
        private static void OnShowCustomLabelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();
        }

        private static void OnShowValueLabelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();
        }

        private static void OnLabelPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();
        }


        private static void OnValuePlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();
        }

        private static void OnLabelOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();
        }

        private static void OnCustomLabelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();
        }
        private static void OnTickPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as TickBar).UpdateTicks();
        }
        private void UpdateTicks()
        {
            int numberofticks = (int)((Maximum - Minimum) / TickFrequency);
            CreateTickBar(numberofticks);
            
        }
        private static void OnTicksCollectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TickBar control = sender as TickBar;

            if (control != null)
            {
                if (control.Ticks != null)
                {
            control.CreateTickBar(control.Ticks.Count);
                }
            }
        }

        #endregion        
        
    }
}
