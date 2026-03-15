#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Windows.Input;
using System.ComponentModel;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Windows.Input;

namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Windows.Input;
using Syncfusion.Licensing;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a control that allows users to select values by moving the pointer 
    /// in a radial motion.
    /// </summary>
#if !WINRT
    [ContentProperty("Content")]    
#else
     [ContentProperty(Name="Content")]    
#endif
    public class SfRadialSlider : RangeBase
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> class.
        /// </summary>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Navigation">Syncfusion.UI.Xaml.Controls.Navigation
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public SfRadialSlider()
        {
#if WPF
            if (EnvironmentTestNavigation.IsSecurityGranted)
            {
                EnvironmentTestNavigation.StartValidateLicense(typeof(SfRadialSlider));
            }
#endif

            DefaultStyleKey = typeof(SfRadialSlider);
            Loaded += RadialSlider_Loaded;
            Unloaded += RadialSlider_Unloaded;
            ValueChanged += RadialSlider_ValueChanged;
            SizeChanged += RadialSlider_SizeChanged;
            IsEnabledChanged += RadialSlider_IsEnabledChanged;
        }

        #endregion

        void RadialSlider_Loaded(object sender, RoutedEventArgs e)
        {
            if (TickFrequency > 0 && (_labelsList != null && _labelsList.Items.Count == 0) || (_ticksList != null && _ticksList.Items.Count == 0))
            {
                int range = (int)((Maximum - Minimum) / TickFrequency);
                for (int i = 0; i <= range; i++)
                {
                    string tickValue = ((i * TickFrequency)+Minimum).ToString();
                    RadialLabel rLabel = new RadialLabel() { Content = tickValue,ContentTemplate = LabelTemplate};
                    if (_labelsList != null)
                        _labelsList.Items.Add(rLabel);

                    RadialTick rTick = new RadialTick() { Content = tickValue,ContentTemplate = TickTemplate};
                    if (_ticksList != null)
                        _ticksList.Items.Add(rTick);
                }
                isLoaded = true;
            }
            UpdateOuterRimRadiusFactor();
            UpdateInnerRimRadiusFactor();
            UpdateLabelRadiusFactor();
            UpdateTicksRadiusFactor();
            UpdateInnerRimRadiusFactor();
            UpdatePointerRadiusFactor();
            UpdatePointerAngle();
            UpdateTicksList(false);
            if (_rootGrid != null)
            {
                _rootGrid.Width = ActualWidth;
                _rootGrid.Height = ActualHeight;
            }
            if (contentPresenter != null && _innerEllipse != null)
            {
                contentPresenter.MaxHeight = _innerEllipse.Height;
                contentPresenter.MaxWidth = _innerEllipse.Width;
            }

#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
#endif
        }

        void RadialSlider_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_ticksList != null)
                _ticksList.Items.Clear();
            if(_labelsList != null)
                _labelsList.Items.Clear();
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if(_labelsListRunTime != null)
                _labelsListRunTime.Items.Clear();
            if(_ticksListRunTime != null)
                _ticksListRunTime.Items.Clear();
#endif
            ValueChanged -= RadialSlider_ValueChanged;
            SizeChanged -= RadialSlider_SizeChanged;
            IsEnabledChanged -= RadialSlider_IsEnabledChanged;
            Unloaded -= RadialSlider_Unloaded;
        }
#if !WINRT
        void RadialSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
#else
        void RadialSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
#endif
        {
            if (!_isPointerPressed)
            {
                UpdatePointerAngle();
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                if (_ticksListRunTime != null && _labelsListRunTime != null)
                {
                    _ticksListRunTime.Items.Clear();
                    _ticksListRunTime.UpdateLayout();
                    _labelsListRunTime.Items.Clear();
                    _labelsListRunTime.UpdateLayout();
                }
#endif
                UpdateTicksList(true);
            }
        }

         void RadialSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateOuterRimRadiusFactor();
            UpdateInnerRimRadiusFactor();
            UpdateLabelRadiusFactor();
            UpdateTicksRadiusFactor();
            UpdateInnerRimRadiusFactor();
            UpdatePointerRadiusFactor();
            UpdatePointerAngle();
            if (_rootGrid != null)
            {
                _rootGrid.Width = ActualWidth;
                _rootGrid.Height = ActualHeight;
            }
        }

         void RadialSlider_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
         {
             if (!(bool)e.NewValue)
                 VisualStateManager.GoToState(this, "Disabled", true);
             else
                 VisualStateManager.GoToState(this, "Normal", true);
         }

         internal void UpdateTicksList(bool layoutupdate)
         {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
             if (Value % TickFrequency != 0d)
             {
                 if (_ticksListRunTime != null && _labelsListRunTime != null && _radialPointer != null)
                 {
                     double angle = 0d;
                     if (_radialPointer.RenderTransform is RotateTransform)
                         angle = (_radialPointer.RenderTransform as RotateTransform).Angle;

                     _ticksListRunTime.Items.Add(new RadialTick() { Angle = angle, Content = Value, ContentTemplate = TickTemplate });
#if SILVERLIGHT
                    _labelsListRunTime.Items.Add((new RadialLabel() { Angle = angle, Content = Value.ToString(), ContentTemplate = LabelTemplate }));
#else
                     _labelsListRunTime.Items.Add((new RadialLabel() { Angle = angle, Content = Value, ContentTemplate = LabelTemplate }));
#endif
                     if (layoutupdate)
                     {
                         _ticksListRunTime.UpdateLayout();
                         _labelsListRunTime.UpdateLayout();
                     }

                 }
             }
#endif
         }

        #region Dependency Properties

#if WINDOWS_PHONE || WINDOWS_PHONE_7
         /// <summary>
         /// Gets or sets the starting angle for the <see 
         /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
         /// </summary>
         /// <value>
         /// The default value is 0.
         /// </value>
#else        
        /// <summary>
        /// Gets or sets the starting angle for the <see cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 0.
        /// </value>
#endif
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(0d,OnStartAngleChanged));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the ending angle for the <see cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 360.
        /// </value>
#else
        /// <summary>
        /// Gets or sets the ending angle for the <see cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 360.
        /// </value>
#endif
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(360d,OnEndAngleChanged));

         /// <summary>
        /// Using a DependencyProperty as the backing store for LabelVisibilityProperty.  This enables animation, styling, binding, etc...
         /// </summary>
        public static readonly DependencyProperty LabelVisibilityProperty =
            DependencyProperty.Register("LabelVisibility", typeof (Visibility), typeof (SfRadialSlider), new PropertyMetadata(Visibility.Visible));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the label visibility property for the
        /// <see cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is <seealso cref="P:Visibility.Visible"/>
        /// </value>
#else
        /// <summary>
        /// Gets or sets the label visibility property for the
        /// <see cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is <seealso cref="P:Visibility.Visible"/>
        /// </value>
#endif
        public Visibility LabelVisibility
        {
            get { return (Visibility) GetValue(LabelVisibilityProperty); }
            set { SetValue(LabelVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickVisibilityProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickVisibilityProperty =
            DependencyProperty.Register("TickVisibility", typeof (Visibility), typeof (SfRadialSlider), new PropertyMetadata(Visibility.Visible));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the tick visibility property for the
        /// <see cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is <seealso cref="P:Visibility.Visible"/>
        /// </value>
#else
        /// <summary>
        /// Gets or sets the tick visibility property for the
        /// <see cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is <seealso cref="P:Visibility.Visible"/>
        /// </value>
#endif
        public Visibility TickVisibility
        {
            get { return (Visibility) GetValue(TickVisibilityProperty); }
            set { SetValue(TickVisibilityProperty, value); }
        }

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the frequency of ticks for the
        /// <see cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 1/>
        /// </value>
#else
        /// <summary>
        /// Gets or sets the frequency of ticks for the
        /// <see cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 1/>
        /// </value>
#endif
        public int TickFrequency
        {
            get { return (int)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(int), typeof(SfRadialSlider), new PropertyMetadata(1,OnTickFrequencyChanged));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the outer rim radius for the
        /// <see cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 0.08/>
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnOuterRimRadiusFactorChanged"/>
#else
        /// <summary>
        /// Gets or sets the outer rim radius for the
        /// <see cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 0.08/>
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnOuterRimRadiusFactorChanged"/>
#endif
        public double OuterRimRadiusFactor
        {
            get { return (double)GetValue(OuterRimRadiusFactorProperty); }
            set { SetValue(OuterRimRadiusFactorProperty, value); }
        }
         
        /// <summary>
        /// Using a DependencyProperty as the backing store for OuterRimRadiusFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OuterRimRadiusFactorProperty =
            DependencyProperty.Register("OuterRimRadiusFactor", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(0.70d,OnOuterRimRadiusFactorChanged));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the inner rim radius for the
        /// <see cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 0.02/>
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnInnerRimRadiusFactorChanged"/>
#else
        /// <summary>
        /// Gets or sets the inner rim radius for the
        /// <see cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 0.02/>
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnInnerRimRadiusFactorChanged"/>
#endif
        public double InnerRimRadiusFactor
        {
            get { return (double)GetValue(InnerRimRadiusFactorProperty); }
            set { SetValue(InnerRimRadiusFactorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InnerRimRadiusFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InnerRimRadiusFactorProperty =
            DependencyProperty.Register("InnerRimRadiusFactor", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(0.2d,OnInnerRimRadiusFactorChanged));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the rim radius for the label in
        /// <see cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 1/>
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnLabelRadiusFactorChanged"/>
#else
        /// <summary>
        /// Gets or sets the rim radius for the label in
        /// <see cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 1/>
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnLabelRadiusFactorChanged"/>
#endif
        public double LabelRadiusFactor
        {
            get { return (double)GetValue(LabelRadiusFactorProperty); }
            set { SetValue(LabelRadiusFactorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelsRadiusFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelRadiusFactorProperty =
            DependencyProperty.Register("LabelRadiusFactor", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(0.87d,OnLabelRadiusFactorChanged));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the rim radius for the tick in
        /// <see cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 0.82/>
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnTicksRadiusFactorChanged"/>
#else
        /// <summary>
        /// Gets or sets the rim radius for the tick in
        /// <see cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 0.82/>
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnTicksRadiusFactorChanged"/>
#endif
        public double TickRadiusFactor
        {
            get { return (double)GetValue(TickRadiusFactorProperty); }
            set { SetValue(TickRadiusFactorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TicksRadiusFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickRadiusFactorProperty =
            DependencyProperty.Register("TickRadiusFactor", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(0.72d,OnTicksRadiusFactorChanged));
        
#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the it=ntermediate value for <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#else
        /// <summary>
        /// Gets or sets the it=ntermediate value for <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#endif
        public double IntermediateValue
        {
            get { return (double)GetValue(IntermediateValueProperty); }
            set { SetValue(IntermediateValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IntermediateValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntermediateValueProperty =
            DependencyProperty.Register("IntermediateValue", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(0d));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the content for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#else       
        /// <summary>
        /// Gets or sets the content for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#endif
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(SfRadialSlider), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for ContentTemplateProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof (DataTemplate), typeof (SfRadialSlider), new PropertyMetadata(null));
        
#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the template of the content for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#else
        /// <summary>
        /// Gets or sets the template of the content for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#endif
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate) GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the template of the tick for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#else
        /// <summary>
        /// Gets or sets the template of the tick for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#endif
        public DataTemplate TickTemplate
        {
            get { return (DataTemplate)GetValue(TickTemplateProperty); }
            set { SetValue(TickTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickTemplateProperty =
            DependencyProperty.Register("TickTemplate", typeof(DataTemplate), typeof(SfRadialSlider), new PropertyMetadata(null));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the template of the label for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#else
        /// <summary>
        /// Gets or sets the template of the label for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#endif
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(SfRadialSlider), new PropertyMetadata(null));


#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the stroke of the inner rim for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#else
        /// <summary>
        /// Gets or sets the stroke of the inner rim for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#endif
        public Brush InnerRimStroke
        {
            get { return (Brush)GetValue(InnerRimStrokeProperty); }
            set { SetValue(InnerRimStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InnerRimFill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InnerRimStrokeProperty =
            DependencyProperty.Register("InnerRimStroke", typeof(Brush), typeof(SfRadialSlider), new PropertyMetadata(null));
        
#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the stroke of the outer rim for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#else
        /// <summary>
        /// Gets or sets the stroke of the outer rim for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
#endif
        public Brush OuterRimStroke
        {
            get { return (Brush)GetValue(OuterRimStrokeProperty); }
            set { SetValue(OuterRimStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for OuterRimFill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OuterRimStrokeProperty =
            DependencyProperty.Register("OuterRimStroke", typeof(Brush), typeof(SfRadialSlider), new PropertyMetadata(null));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the strokethickness of the outer rim for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 2.0
        /// </value>
#else
        /// <summary>
        /// Gets or sets the strokethickness of the outer rim for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 2.0
        /// </value>
#endif
        public double OuterRimStrokeThickness
        {
            get { return (double)GetValue(OuterRimStrokeThicknessProperty); }
            set { SetValue(OuterRimStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for OuterRimStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OuterRimStrokeThicknessProperty =
            DependencyProperty.Register("OuterRimStrokeThickness", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(2.0d));
        
#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the strokethickness of the inner rim for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 2.0
        /// </value>
#else
        /// <summary>
        /// Gets or sets the strokethickness of the inner rim for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is 2.0
        /// </value>
#endif
        public double InnerRimStrokeThickness
        {
            get { return (double)GetValue(InnerRimStrokeThicknessProperty); }
            set { SetValue(InnerRimStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InnerRimStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InnerRimStrokeThicknessProperty =
            DependencyProperty.Register("InnerRimStrokeThickness", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(2.0d));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the brush for the inner rim for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is null
        /// </value>
#else
        /// <summary>
        /// Gets or sets the brush for the inner rim for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>
        /// The default value is null
        /// </value>
#endif
        public Brush InnerRimFill
        {
            get { return (Brush)GetValue(InnerRimFillProperty); }
            set { SetValue(InnerRimFillProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InnerRimFill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InnerRimFillProperty =
            DependencyProperty.Register("InnerRimFill", typeof(Brush), typeof(SfRadialSlider), new PropertyMetadata(null));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the radius of the pointer for  <see 
        /// cref="Syncfusion.WP.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>The default value is 0.75d</value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnPointerRadiusFacotrChanged"/>
#else
        /// <summary>
        /// Gets or sets the radius of the pointer for  <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
        /// </summary>
        /// <value>The default value is 0.75d</value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Navigation.SfRadialSlider.OnPointerRadiusFacotrChanged"/>
#endif
        public double PointerRadiusFactor
        {
            get { return (double)GetValue(PointerRadiusFactorProperty); }
            set { SetValue(PointerRadiusFactorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PointerRadiusFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PointerRadiusFactorProperty =
            DependencyProperty.Register("PointerRadiusFactor", typeof(double), typeof(SfRadialSlider), new PropertyMetadata(0.75d, OnPointerRadiusFacotrChanged));
        
        /// <summary>
        /// Gets or sets the pointer style
        /// </summary>
        public Style PointerStyle
        {
            get { return (Style)GetValue(PointerStyleProperty); }
            set { SetValue(PointerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RadialPointerStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PointerStyleProperty =
            DependencyProperty.Register("PointerStyle", typeof(Style), typeof(SfRadialSlider), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the preview pointer style
        /// </summary>
        public Style PreviewPointerStyle
        {
            get { return (Style)GetValue(PreviewPointerStyleProperty); }
            set { SetValue(PreviewPointerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RadialPreviewPointerStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PreviewPointerStyleProperty =
            DependencyProperty.Register("PreviewPointerStyle", typeof(Style), typeof(SfRadialSlider), new PropertyMetadata(null));

        #endregion

        #region Callback Methods

        private static void OnPointerRadiusFacotrChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialSlider rSlider = sender as SfRadialSlider;
            if (rSlider != null)
                rSlider.UpdatePointerRadiusFactor();            
        }

        private static void OnInnerRimRadiusFactorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialSlider rSlider = sender as SfRadialSlider;
            if (rSlider != null)
                rSlider.UpdateInnerRimRadiusFactor();
        }

        private static void OnOuterRimRadiusFactorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialSlider rSlider = sender as SfRadialSlider;
            if (rSlider != null)
                rSlider.UpdateOuterRimRadiusFactor();           
        }

        private static void OnLabelRadiusFactorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialSlider rSlider = sender as SfRadialSlider;
            if (rSlider != null)
                rSlider.UpdateLabelRadiusFactor();
        }
        
        private static void OnTicksRadiusFactorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialSlider rSlider = sender as SfRadialSlider;
            if (rSlider != null)
                rSlider.UpdateTicksRadiusFactor();
        }

        private static void OnTickFrequencyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialSlider rSlider = sender as SfRadialSlider;
            if (rSlider != null && rSlider.isLoaded)
            {
                rSlider.Update();
            }
        }

        private static void OnStartAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialSlider rSlider = sender as SfRadialSlider;
            if (rSlider != null)
            {
                if (rSlider.StartAngle < 0)
                    rSlider.StartAngle += 360;
                if (rSlider.StartAngle > 360)
                    rSlider.StartAngle -= 360;

                rSlider.UpdateAngle();
            }
        }

        private static void OnEndAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialSlider rSlider = sender as SfRadialSlider;
            if (rSlider != null)
            {
                if (rSlider.EndAngle < 0)
                    rSlider.EndAngle += 360;
                if (rSlider.EndAngle > 360)
                    rSlider.EndAngle -= 360;

                rSlider.UpdateAngle();
            }
        }

         /// <summary>
         /// Updates the SfRadialSlider layout when the Maximum value is changed
         /// </summary>
         /// <param name="oldMaximum"></param>
         /// <param name="newMaximum"></param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            Update();
        }

         /// <summary>
        /// Updates the SfRadialSlider layout when the Minimum value is changed
         /// </summary>
         /// <param name="oldMinimum"></param>
         /// <param name="newMinimum"></param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            Update();
        }

        #endregion

        #region Helper Methods

        private void UpdateAngle()
        {
            if (TickFrequency > 0 && (_labelsList != null && _labelsList.Items.Count >= 0) || (_ticksList != null && _ticksList.Items.Count >= 0))
            {
                if (_labelsList.Items.Count > 0)
                    _labelsList.Items.Clear();
                if (_ticksList.Items.Count > 0)
                    _ticksList.Items.Clear();
                int range = (int)((Maximum - Minimum) / TickFrequency);
                for (int i = 0; i <= range; i++)
                {
                    string tickValue = ((i * TickFrequency) + Minimum).ToString();
                    RadialLabel rLabel = new RadialLabel() { Content = tickValue, ContentTemplate = LabelTemplate };
                    if (_labelsList != null)
                        _labelsList.Items.Add(rLabel);

                    RadialTick rTick = new RadialTick() { Content = tickValue, ContentTemplate = TickTemplate };
                    if (_ticksList != null)
                        _ticksList.Items.Add(rTick);
                }
            }
            UpdatePointerAngle();
        }

        private void Update()
        {
            if (isLoaded)
            {
                ValidateTickFrequency();
            }

            if (TickFrequency > 0 && (_labelsList != null && _labelsList.Items.Count >= 0) || (_ticksList != null && _ticksList.Items.Count >= 0))
            {
                if (_labelsList.Items.Count > 0)
                    _labelsList.Items.Clear();
                if (_ticksList.Items.Count > 0)
                    _ticksList.Items.Clear();
                int range = (int)((Maximum - Minimum) / TickFrequency);
                for (int i = 0; i <= range; i++)
                {
                    string tickValue = ((i * TickFrequency) + Minimum).ToString();
                    RadialLabel rLabel = new RadialLabel() { Content = tickValue, ContentTemplate = LabelTemplate };
                    if (_labelsList != null)
                        _labelsList.Items.Add(rLabel);

                    RadialTick rTick = new RadialTick() { Content = tickValue, ContentTemplate = TickTemplate };
                    if (_ticksList != null)
                        _ticksList.Items.Add(rTick);
                }
            }
            UpdatePointerAngle();
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (_ticksListRunTime != null && _labelsListRunTime != null)
            {
                _ticksListRunTime.Items.Clear();
                _ticksListRunTime.UpdateLayout();
                _labelsListRunTime.Items.Clear();
                _labelsListRunTime.UpdateLayout();
            }
#endif
            UpdateTicksList(true);
        }

        private void UpdateInnerRimRadiusFactor()
        {
            if (_innerEllipse != null && InnerRimRadiusFactor >= 0)
            {
				
                    _innerEllipse.Width = ActualWidth * InnerRimRadiusFactor;
                    _innerEllipse.Height = ActualWidth * InnerRimRadiusFactor;                 
               
               
            }
        }

        private void UpdateOuterRimRadiusFactor()
        {
            if (_outerEllipse != null && OuterRimRadiusFactor >= 0)
            {
                
                    _outerEllipse.Width = ActualWidth * OuterRimRadiusFactor;
                    _outerEllipse.Height = ActualHeight * OuterRimRadiusFactor;                 
                              
                                   
            }
        }

        private void UpdateTicksRadiusFactor()
        {
            if (TickRadiusFactor < 0)
                TickRadiusFactor = 0;
            if (_ticksList != null)
            {
                _ticksList.Width = ActualWidth * TickRadiusFactor;
                _ticksList.Height = ActualHeight * TickRadiusFactor;
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (_ticksListRunTime != null)
            {
                _ticksListRunTime.Width = ActualWidth * TickRadiusFactor;
                _ticksListRunTime.Height = ActualHeight * TickRadiusFactor;
            }
#endif
        }

        private void UpdateLabelRadiusFactor()
        {
            if (LabelRadiusFactor < 0)
                LabelRadiusFactor = 0;
            if (_labelsList != null)
            {
                _labelsList.Width = ActualWidth * LabelRadiusFactor;
                _labelsList.Height = ActualHeight * LabelRadiusFactor;
            }
            if (_labelsListRunTime != null)
            {
                _labelsListRunTime.Width = ActualWidth * LabelRadiusFactor;
                _labelsListRunTime.Height = ActualHeight * LabelRadiusFactor;
            }
        }

        private void UpdatePointerRadiusFactor()
        {
            if (PointerRadiusFactor < 0)
                PointerRadiusFactor = 0;
            if (_radialPointer != null)
            {
                    _radialPointer.Width = (ActualWidth / 2 * PointerRadiusFactor);
                    double rightMargin = (ActualWidth / 2 - (ActualWidth / 2 * PointerRadiusFactor));
                    _radialPointer.Margin = new Thickness(0, 0, rightMargin, 0);
                    _radialPointer.HorizontalAlignment = HorizontalAlignment.Right;
                    _radialPointer.RenderTransformOrigin = new Point(0, 0.5);
            }

            if (_radialPreviewPointer != null)
            {
                    _radialPreviewPointer.Width = (ActualWidth / 2 * PointerRadiusFactor);
                    double rightMargin = (ActualWidth / 2 - (ActualWidth / 2 * PointerRadiusFactor));
                    _radialPreviewPointer.Margin = new Thickness(0, 0, rightMargin, 0);
                    _radialPreviewPointer.HorizontalAlignment = HorizontalAlignment.Right;
                    _radialPreviewPointer.RenderTransformOrigin = new Point(0, 0.5);
            }
        }

        private void UpdatePointerAngle()
        {
            int trackUnitsCount = 0;
            if (_ticksList != null && _ticksList.Items.Count > 0)
                trackUnitsCount = _ticksList.Items.Count;
            else if (_labelsList != null && _labelsList.Items.Count > 0)
                trackUnitsCount = _labelsList.Items.Count;

            if (_radialPointer != null && _ticksList != null && trackUnitsCount > 0)
            {             
                RotateTransform transform = new RotateTransform();               
               // transform.Angle = (Value -Minimum) * ((360.0 / trackUnitsCount) / TickFrequency);
                double factor = (Math.Abs(StartAngle - EndAngle)/trackUnitsCount)/TickFrequency;

                if (Minimum == Maximum)
                {
                    StartAngle = 0; 
                    EndAngle = 360;
                }

                if (!((StartAngle == 0d && EndAngle == 360d) || (StartAngle == 360d && EndAngle == 0d)))
                    factor = (Math.Abs(StartAngle - EndAngle)/(Math.Abs(Minimum - Maximum)/TickFrequency))/TickFrequency;
                double angle = 0d;
                if (StartAngle < EndAngle)
                    angle = (Value-Minimum) * factor + StartAngle;
                else
                    angle = Math.Abs((Value-Minimum) * factor - StartAngle);
              
                transform.Angle = angle;
                _radialPointer.RenderTransform = transform;
                if (_ticksListRunTime != null && _labelsListRunTime != null)
                {
                    _ticksListRunTime.Items.Clear();
                    _labelsListRunTime.Items.Clear();
                    _ticksListRunTime.Items.Add(new RadialTick() { Angle = angle, Content = Value, ContentTemplate = TickTemplate });
#if SILVERLIGHT
                    _labelsListRunTime.Items.Add((new RadialLabel() { Angle = angle, Content = Value.ToString(), ContentTemplate = LabelTemplate }));
#else
                    _labelsListRunTime.Items.Add((new RadialLabel() { Angle = angle, Content = Value, ContentTemplate = LabelTemplate }));
#endif
                }
            }
        }

#if !WINRT
        private double GetValueFromMousePoint(MouseEventArgs e, out double angle)
#else
        private double GetValueFromMousePoint(PointerRoutedEventArgs e, out double angle)
#endif
        {
#if !WINRT
            Point currentLocation = e.GetPosition(this);
#else
            Point currentLocation = e.GetCurrentPoint(this).Position;
#endif
            Point knobCenter = new Point(ActualHeight / 2, ActualWidth / 2);
            double radians = Math.Atan((currentLocation.Y - knobCenter.Y) /
                                       (currentLocation.X - knobCenter.X));

            angle = Convert.ToInt16((radians * 180 / Math.PI));

            if (currentLocation.X - knobCenter.X < 0)
                angle += 180;

            if (angle < 0)
                angle += 359;
            int count = 1;

            if (_ticksList != null)
                count = _ticksList.Items.Count;
   			double factor = (Math.Abs(StartAngle - EndAngle) / count) / TickFrequency;
            if (!((StartAngle == 0d && EndAngle == 360d) || (StartAngle == 360d && EndAngle == 0d)))
                factor = (Math.Abs(StartAngle - EndAngle) / (Math.Abs(Minimum - Maximum) / TickFrequency)) / TickFrequency;
            double value = (angle / factor)+Minimum;


            if (StartAngle > EndAngle)
            {
             if (angle > StartAngle || angle < EndAngle)
             {
                 double sDiff = 360 % Math.Abs(angle - StartAngle);
                 double eDiff = 360 % Math.Abs(angle - EndAngle);
                 if (sDiff < eDiff)
                     angle = StartAngle;
                 else
                 {
                     angle = EndAngle;
                 }
             }
            }
            else
            {
                if ((angle < StartAngle || angle > EndAngle) && EndAngle <= 360)
                {
                    double sDiff = Math.Abs(angle - StartAngle);
                    double eDiff = Math.Abs(angle - EndAngle);
                    if (sDiff < eDiff)
                        angle = StartAngle;
                    else
                    {
                        angle = EndAngle;
                    }
                }
            }



          //  factor = Math.Abs(StartAngle - EndAngle)/Math.Abs(Maximum - Minimum);
            if(StartAngle < EndAngle)
               value =  ((angle - StartAngle) / factor)+Minimum;
            else
            {
                value = ((StartAngle - angle)/factor)+Minimum;
            }
            if (EndAngle > 360 && value.ToString().Contains("-"))
                value = Maximum + value;
            return value <= Maximum ? value : Minimum;

        }

#if !WINRT
        private void UpdatePointer(MouseEventArgs e)
#else
        private void UpdatePointer(PointerRoutedEventArgs e)
#endif
        {
            double angle;
            double value;

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (_ticksListRunTime != null && _labelsListRunTime != null)
            {
                _ticksListRunTime.Items.Clear();
                _ticksListRunTime.UpdateLayout();
                _labelsListRunTime.Items.Clear();
                _labelsListRunTime.UpdateLayout();
            }
#endif
            value = GetValueFromMousePoint(e, out angle);
            IntermediateValue = value;
            if (_radialPointer != null)
            {
                RotateTransform transform = new RotateTransform();
                transform.Angle = angle;
                _radialPointer.RenderTransform = transform;
            }
            if (SmallChange > 0d && value != Minimum)
            {
                double tempValue = 0;
                if (value > Value)
                    tempValue = value - Value;
                else
                    tempValue = value - (Value % SmallChange);
                double diff = tempValue % SmallChange;
                if (diff > SmallChange / 2.0)
                    Value = (value - diff) + SmallChange;
                else
                {
                    Value = value - diff;
                }
            }
            else
                Value = value;
        }

#if !WINRT
        private void UpdateMouseOverPointer(MouseEventArgs e)
#else
        private void UpdateMouseOverPointer(PointerRoutedEventArgs e)
#endif
        {
            double angle;
            double value = 0d;
            value = GetValueFromMousePoint(e, out angle);

            if (_radialPreviewPointer != null)
            {
                RotateTransform transform = new RotateTransform();
                transform.Angle = angle;
                _radialPreviewPointer.RenderTransform = transform;
                //  radialPreviewPointer.Visibility = Math.Abs(value - Value) <= TickFrequency ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
            }
            IntermediateValue = value;
        }

        #endregion

        #region Local Variables
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        /// <summary>
        /// Gets an instance for the <see 
        /// cref="T:Syncfusion.WP.Controls.Navigation.RadialPointer"/> control.
        /// </summary>
        [Browsable(false)]
        public RadialPointer _radialPointer;
#else
        private RadialPointer _radialPointer;
#endif
        private RadialPreviewPointer _radialPreviewPointer;
        private Ellipse _outerEllipse;
        private Ellipse _innerEllipse;
        private RadialList _ticksList;
        private RadialList _labelsList;
        private RadialList _ticksListRunTime;
        private RadialList _labelsListRunTime;
        private bool _isPointerPressed;
        private Grid _rootGrid;
        private Border contentPresenter;
        private bool isLoaded = false;

        #endregion

        #region Override Methods

         /// <summary>
         /// Sets the values for the declared variables when initiated.
         /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            _rootGrid = GetTemplateChild("PART_Root") as Grid;
            _radialPointer = GetTemplateChild("PART_Pointer") as RadialPointer;
            _radialPreviewPointer = GetTemplateChild("PART_PreviewPointer") as RadialPreviewPointer;
            _outerEllipse = GetTemplateChild("PART_OuterEllipse") as Ellipse;
            _innerEllipse = GetTemplateChild("PART_InnerEllipse") as Ellipse;
            _ticksList = GetTemplateChild("PART_Ticks") as RadialList;
            _labelsList = GetTemplateChild("PART_Labels") as RadialList;
            _ticksListRunTime = GetTemplateChild("PART_TicksRunTime") as RadialList;
            _labelsListRunTime = GetTemplateChild("PART_LabelsRunTime") as RadialList;
            contentPresenter = GetTemplateChild("PART_ContentBorder") as Border;

            ValidateTickFrequency();

            base.OnApplyTemplate();
#if SILVERLIGHT
            DependencyObject parent = FindAncestor(this);
            if (parent == null)
            {
                Loaded += RadialSlider_Loaded;
                Unloaded += RadialSlider_Unloaded;
            }
#endif
        }

#if SILVERLIGHT
        private DependencyObject FindAncestor(DependencyObject element)
        {
            if (element != null)
            {
                var _element = VisualTreeHelper.GetParent(element);
                if (!(_element is SfRadialMenu))
                {
                    return FindAncestor(_element);
                }
                else
                {
                    return _element;
                }
            }
            else
                return null;
        }
#endif

        internal void ValidateTickFrequency()
        {
            if (this.TickFrequency > this.Maximum)
                this.TickFrequency = Convert.ToInt32(this.Maximum);
            if ((((this.Maximum - this.Minimum) / this.TickFrequency) % 1) != 0)
            {
                for (int i = 0; (((this.Maximum - this.Minimum) / this.TickFrequency) % 1) != 0; i++)
                    this.TickFrequency -= 1;
            }
        }

        /// <summary>
        /// Sets the focus when the pointer is pressed
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {

            _isPointerPressed = true;
#if !WINRT
            CaptureMouse();
#else
            CapturePointer(e.Pointer);
#endif

            UpdatePointer(e);

#if !WINRT
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif
        }

         /// <summary>
        /// Sets the focus on pointer movement
         /// </summary>
         /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseMove(MouseEventArgs e)
#else
        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
#if WINRT
            var point = e.GetCurrentPoint(this);
            if (point.Position.X >= 0 && point.Position.Y >= 0 && point.Position.X <= this.ActualWidth && point.Position.Y <= this.ActualHeight)
#else
            var point = e.GetPosition(this);
            if(point.X >=0 && point.Y >=0 && point.X <= this.ActualWidth && point.Y <= this.ActualHeight)
#endif
            {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                if (_isPointerPressed)
                {
#endif
                    UpdatePointer(e);
                    if (_radialPreviewPointer != null)
                        _radialPreviewPointer.Visibility = Visibility.Collapsed;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                }
                else
                {
                    if (_radialPreviewPointer != null)
                        _radialPreviewPointer.Visibility = Visibility.Visible;
                    UpdateMouseOverPointer(e);
                }
#endif
            }
#if !WINRT
            base.OnMouseMove(e);
#else
            base.OnPointerMoved(e);
#endif

        }

         /// <summary>
         /// Sets the focus on pointer release
         /// </summary>
         /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            _isPointerPressed = false;
#if !WINRT
            ReleaseMouseCapture();
#else
            ReleasePointerCaptures();
#endif
            UpdatePointerAngle();

            UpdateTicksList(true);
#if !WINRT
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
        }

         /// <summary>
         /// Sets the visibility as visible to the control when pointer has been entered
         /// </summary>
         /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (_radialPreviewPointer != null)
                _radialPreviewPointer.Visibility = Visibility.Visible;
#if !WINRT
            base.OnMouseEnter(e);
#else
            base.OnPointerEntered(e);
#endif
        }

         /// <summary>
        /// Sets the visibility as collapsed to the control when pointer has been exited
         /// </summary>
         /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (_radialPreviewPointer != null)
                _radialPreviewPointer.Visibility = Visibility.Collapsed;
#if !WINRT
            base.OnMouseLeave(e);
#else
            base.OnPointerExited(e);
#endif
        }

        #endregion

    }
}
