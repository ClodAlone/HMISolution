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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Security;

namespace Syncfusion.Windows.Controls.Map
{
    /// <summary>
    ///  Navigation control helps to Pan and Zoom the map
    /// </summary>
    public class NavigationControl : Control
    {
        #region Private Fields

        Button topButton;
        Button leftButton;
        Button rightButton;
        Button bottomButton;
        Button zoomInButton;
        Button zoomOutButton;
        Slider zoomSlider;
        MapControl mapControl;
        double tempZoomfactor;
        bool isload = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.NavigationControl">NavigationControl</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public NavigationControl()
        {
            this.DefaultStyleKey = typeof(NavigationControl);
        }


        #region Properties
        #region NavigationControlPosition

        /// <summary>
        /// Gets or sets Position of the NavigationControl.
        /// </summary>
        /// <value>
        /// NavigationControlPositions
        /// </value>
        public NavigationControlPositions NavigationControlPosition
        {
            get { return (NavigationControlPositions)GetValue(NavigationControlPositionProperty); }
            set { SetValue(NavigationControlPositionProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for NavigationControlPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NavigationControlPositionProperty =
            DependencyProperty.Register("NavigationControlPosition", typeof(NavigationControlPositions), typeof(NavigationControl), new PropertyMetadata(NavigationControlPositions.Left, new PropertyChangedCallback(OnNavigationControlPositionChanged)));

        private static void OnNavigationControlPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

            NavigationControl navigationControl = obj as NavigationControl;
            navigationControl.UpdateNavigationControlPosition();


        }

        #endregion

        #endregion

        #region Internal Properties

        internal double MinSliderValue
        {
            get { return (double)GetValue(MinSliderValueProperty); }
            set { SetValue(MinSliderValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinSliderValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinSliderValueProperty =
            DependencyProperty.Register("MinSliderValue", typeof(double), typeof(NavigationControl), new PropertyMetadata(0.0d));



        internal double MaxSliderValue
        {
            get { return (double)GetValue(MaxSliderValueProperty); }
            set { SetValue(MaxSliderValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSliderValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MaxSliderValueProperty =
            DependencyProperty.Register("MaxSliderValue", typeof(double), typeof(NavigationControl), new PropertyMetadata(0.0d));




#if SILVERLIGHT

        #region VisualStyle(Dependency Property)
        /// <summary>
        /// Gets or sets Visual Style for the Navigation Control.
        /// </summary>
        /// <value>
        /// VisualStyle
        /// </value>
        public VisualStyles VisualStyle
        {
            get { return (VisualStyles)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for VisualStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(VisualStyles), typeof(NavigationControl), new PropertyMetadata(VisualStyles.Default, new PropertyChangedCallback(OnVisualStyleChanged)));

        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationControl navigationControl = d as NavigationControl;
            navigationControl.ChangeSkins(e.NewValue.ToString());
        }

        #endregion


#endif


        #endregion

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>

#if !SyncfusionFramework4_0
        [SecuritySafeCritical]
#endif
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.topButton = this.GetTemplateChild("topButton") as Button;
            topButton.ClickMode = ClickMode.Press;
            this.leftButton = this.GetTemplateChild("leftButton") as Button;
            this.rightButton = this.GetTemplateChild("rightButton") as Button;
            this.bottomButton = this.GetTemplateChild("bottomButton") as Button;
            this.zoomInButton = this.GetTemplateChild("zoomInButton") as Button;
            this.zoomOutButton = this.GetTemplateChild("zoomOutButton") as Button;
            this.zoomSlider = this.GetTemplateChild("zoomSlider") as Slider;
            this.mapControl = MapControl.FindParent<MapControl>(this);
            this.zoomSlider.SetBinding(Slider.IsEnabledProperty, new Binding { Source = this.mapControl, Path = new PropertyPath("EnableZoom") });
            this.topButton.Click += new System.Windows.RoutedEventHandler(topButton_Click);
            this.leftButton.Click += new System.Windows.RoutedEventHandler(leftButton_Click);
            this.rightButton.Click += new System.Windows.RoutedEventHandler(rightButton_Click);
            this.bottomButton.Click += new System.Windows.RoutedEventHandler(bottomButton_Click);
            this.zoomInButton.Click += new System.Windows.RoutedEventHandler(zoomInButton_Click);
            this.zoomOutButton.Click += new System.Windows.RoutedEventHandler(zoomOutButton_Click);
            this.tempZoomfactor = mapControl.ZoomFactor;
            this.MaxSliderValue = mapControl.MaxZoom / this.tempZoomfactor;
            this.MinSliderValue = mapControl.MinZoom / this.tempZoomfactor;
            this.isload = true;
#if WPF
            if (Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(this) != null)
            {
                this.ChangeSkins(Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(this).ToString());
            }
#else
           
#endif

            if (mapControl != null)
            {
                zoomSlider.SetBinding(Slider.MaximumProperty, new Binding { Source = this, Path = new PropertyPath("MaxSliderValue") });
                zoomSlider.SetBinding(Slider.MinimumProperty, new Binding { Source = this, Path = new PropertyPath("MinSliderValue") });
                zoomSlider.SetBinding(Slider.ValueProperty, new Binding { Source = mapControl, Path = new PropertyPath("ZoomLevel"), Mode = BindingMode.TwoWay });
                this.SetBinding(NavigationControl.NavigationControlPositionProperty, new Binding { Source = mapControl, Path = new PropertyPath("NavigationControlPosition"), Mode = BindingMode.TwoWay });
#if SILVERLIGHT
                this.SetBinding(NavigationControl.VisualStyleProperty, new Binding { Source = mapControl, Path = new PropertyPath("VisualStyle"), Mode = BindingMode.TwoWay });
#endif
            }
        }




        void zoomOutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.mapControl.LayeredContent != null)
            {
                this.mapControl.ZoomOutCommand.Execute(null);
            }

        }

        void zoomInButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.mapControl.LayeredContent != null)
            {
                this.mapControl.ZoomInCommand.Execute(null);
            }
        }

        void bottomButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.mapControl.LayeredContent != null)
            {
                if (!(this.HorizontalAlignment == HorizontalAlignment.Left || this.HorizontalAlignment == HorizontalAlignment.Right))
                {
                    this.mapControl.PanCommand.Execute("left");
                }
                else
                {
                    this.mapControl.PanCommand.Execute("top");
                }
            }
        }

        void rightButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.mapControl.LayeredContent != null)
            {
                if (!(this.HorizontalAlignment == HorizontalAlignment.Left || this.HorizontalAlignment == HorizontalAlignment.Right))
                {
                    this.mapControl.PanCommand.Execute("bottom");
                }
                else
                {
                    this.mapControl.PanCommand.Execute("left");
                }
            }
        }

        void leftButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.mapControl.LayeredContent != null)
            {
                if (!(this.HorizontalAlignment == HorizontalAlignment.Left || this.HorizontalAlignment == HorizontalAlignment.Right))
                {
                    this.mapControl.PanCommand.Execute("top");
                }
                else
                {
                    this.mapControl.PanCommand.Execute("right");
                }
            }
        }

        void topButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.mapControl.LayeredContent != null)
            {
                if (!(this.HorizontalAlignment == HorizontalAlignment.Left || this.HorizontalAlignment == HorizontalAlignment.Right))
                {
                    this.mapControl.PanCommand.Execute("right");
                }
                else
                {
                    this.mapControl.PanCommand.Execute("bottom");
                }
            }
        }

        #region Helper Methods


        private void UpdateNavigationControlPosition()
        {
            TransformGroup tgroup = new TransformGroup();
            TranslateTransform transTransform = new TranslateTransform();
            RotateTransform rotateTransfrom = new RotateTransform { CenterX=this.ActualWidth /2 , CenterY=this.ActualHeight/2 };
            tgroup.Children.Add(transTransform);
            tgroup.Children.Add(rotateTransfrom);
            switch (this.NavigationControlPosition)
            {
                case NavigationControlPositions.Left:
                    rotateTransfrom.Angle = 0;
                    this.VerticalAlignment = VerticalAlignment.Center;
                    this.HorizontalAlignment = HorizontalAlignment.Left;
                    this.RenderTransform = tgroup;
                    break;

                case NavigationControlPositions.Right:
                    rotateTransfrom.Angle = 0;
                    transTransform.X = 120;
                    this.VerticalAlignment = VerticalAlignment.Center;
                    this.HorizontalAlignment = HorizontalAlignment.Right;
                    this.RenderTransform = tgroup;
                    break;

                case NavigationControlPositions.Top:
                    rotateTransfrom.Angle = 270;      
                    transTransform.X = 120;
                    this.VerticalAlignment = VerticalAlignment.Top;
                    this.HorizontalAlignment = HorizontalAlignment.Center;
                    this.RenderTransform = tgroup;
                    break;

                case NavigationControlPositions.Bottom:
                    rotateTransfrom.Angle = 270;
                    transTransform.X = 10;
                    this.VerticalAlignment = VerticalAlignment.Bottom;
                    this.HorizontalAlignment = HorizontalAlignment.Center;
                    this.RenderTransform = tgroup;
                    break;
            }
        }

#if WPF
        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific
        /// dependency property that changed is reported in the arguments parameter.
        /// Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well
        /// as old and new values.</param>
#if !SyncfusionFramework4_0
        [SecuritySafeCritical]
#endif
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == Syncfusion.Windows.Shared.SkinStorage.VisualStyleProperty)
            {
                switch (e.NewValue.ToString())
                {
                    case "Office2007Blue":
                        this.ChangeSkins("Office2007Blue");
                        break;
                    case "Office2007Silver":
                        this.ChangeSkins("Office2007Silver");
                        break;
                    case "Office2007Black":
                        this.ChangeSkins("Office2007Black");
                        break;
                    case "Office2010Blue":
                        this.ChangeSkins("Office2010Blue");
                        break;
                    case "Office2010Silver":
                        this.ChangeSkins("Office2010Silver");
                        break;
                    case "Office2010Black":
                        this.ChangeSkins("Office2010Black");
                        break;
                    case "VS2010":
                        this.ChangeSkins("VS2010");
                        break;
                    case "Blend":
                        this.ChangeSkins("Blend");
                        break;
                    default:
                        this.ChangeSkins("Default");
                        break;

                }
            }
        }
#endif
        internal void ChangeSkins(string style)
        {
            if (this.isload)
            {
                ResourceDictionary resourceDic = new ResourceDictionary();
#if WPF
                if (style == "Default")
                {
                    resourceDic.Source = new Uri("/Syncfusion.Maps.Wpf;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                }
                else
                {
                    resourceDic.Source = new Uri("/Syncfusion.Maps.Wpf;component/Themes/" + style + ".xaml", UriKind.RelativeOrAbsolute);
                }
#else
            if (style == "Default")
            {
                resourceDic.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {
                resourceDic.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/" + style + ".xaml", UriKind.RelativeOrAbsolute);
            }
#endif
                foreach (object obj in resourceDic.Keys)
                {
                    if (obj.ToString() == "SliderStyle1")
                    {
                        this.zoomSlider.Style = resourceDic["SliderStyle1"] as Style;
                    }
                    if (obj.ToString() == "ButtonStyle2")
                    {
                        this.leftButton.Style = resourceDic["ButtonStyle2"] as Style;
                        this.rightButton.Style = resourceDic["ButtonStyle2"] as Style;
                        this.topButton.Style = resourceDic["ButtonStyle2"] as Style;
                        this.bottomButton.Style = resourceDic["ButtonStyle2"] as Style;
                    }
                    if (obj.ToString() == "ButtonStyle4")
                    {
                        this.zoomInButton.Style = resourceDic["ButtonStyle4"] as Style;
                    }
                    if (obj.ToString() == "ButtonStyle5")
                    {
                        this.zoomOutButton.Style = resourceDic["ButtonStyle5"] as Style;
                    }
                }

            }

        #endregion


        }
    }
}
