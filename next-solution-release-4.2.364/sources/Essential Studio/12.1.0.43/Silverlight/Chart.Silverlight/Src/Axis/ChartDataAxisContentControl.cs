#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Data;

   
    /// <summary>
    /// To Display all ChartAxis Data in chart
    /// </summary>
    public class DataAxis : ContentControl,IDisposable
    {
        internal bool IsAxisHeader = false;
        internal bool IsHideLabel = false;
        internal double row = 0d;
        internal double transformx = 0d;
        internal double transformy = 0d;

        internal static readonly DependencyProperty LabelRotateTransformProperty =
DependencyProperty.Register("LabelRotateTransform", typeof(TransformGroup), typeof(DataAxis), new PropertyMetadata(new TransformGroup()));

        internal TransformGroup LabelRotateTransform
        {
            get { return (TransformGroup)GetValue(LabelRotateTransformProperty); }
            set { SetValue(LabelRotateTransformProperty, value); }
        }

        internal static readonly DependencyProperty LabelRotateAngleProperty =
  DependencyProperty.Register("LabelRotateAngle", typeof(double), typeof(DataAxis), new PropertyMetadata(0d, new PropertyChangedCallback(OnRotateLabelChanged)));

        internal double LabelRotateAngle
        {
            get { return (double)GetValue(LabelRotateAngleProperty); }
            set { SetValue(LabelRotateAngleProperty, value); }
        }

        private static void OnRotateLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataAxis axisLabel = d as DataAxis;

            if (axisLabel != null && axisLabel.RelatedAxis != null)
            {
                TransformGroup group = new TransformGroup();
                group.Children.Add(new RotateTransform() { Angle = (double)e.NewValue });
                //group.Children.Add(new TranslateTransform() { Y = axisLabel.LabelHeight / 2d , X = axisLabel.LabelWidth / 2d });

                axisLabel.LabelRotateTransform = group;
            }

        }

        /// <summary>
        /// Get or Set PointInfo property
        /// </summary>
        public ChartAxisPoints PointInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Called when instance created for DateAxis
        /// </summary>
        public DataAxis()
        {
            DefaultStyleKey = typeof(DataAxis);
            #region revamp
            this.SizeChanged += new SizeChangedEventHandler(DataAxis_SizeChanged);
            this.ActualValue = 0d;
            this.Row = 0d;
            this.PolyPoints = new PointCollection();
            #endregion
        }

        void DataAxis_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.RelatedAxis != null)
            {
                TranslateTransform transform = new TranslateTransform();
                if (this.RelatedAxis.Orientation == Orientation.Horizontal)
                {
                    transform.X = this.ActualWidth / 2d * -1;
                }
                else
                {
                    transform.Y = this.ActualHeight / 2d * -1;
                }
                this.RenderTransform = transform;
            }
        }
        /// <summary>
        ///  Identifies the RelatedAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty RelatedAxisProperty =
    DependencyProperty.Register("RelatedAxis", typeof(ChartAxis), typeof(DataAxis), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set relativeAxisProperty
        /// </summary>
        public ChartAxis RelatedAxis
        {
            get
            {
                return (ChartAxis)GetValue(RelatedAxisProperty);
            }

            set
            {
                SetValue(RelatedAxisProperty, value);
            }
        }

        /// <summary>
        /// Get or Set ActualValue property
        /// </summary>
        public double ActualValue
        {
            get;
            set;
        }
        /// <summary>
        /// Get or Set Row property 
        /// </summary>
        public double Row
        {
            get;
            set;
        }

        /// <summary>
        /// Identifies the PrimaryAxisStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty IntersectActionMarginProperty = DependencyProperty.Register("IntersectActionMargin", typeof(Thickness), typeof(DataAxis), new PropertyMetadata(new Thickness(0)));
        /// <summary>
        /// Get or Set IntersectActionMarginProperty 
        /// </summary>
        public Thickness IntersectActionMargin
        {
            get { return (Thickness)GetValue(IntersectActionMarginProperty); }
            set { SetValue(IntersectActionMarginProperty, value); }
        }

        /// <summary>
        ///  Identifies the LabelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty LablePositionProperty = DependencyProperty.Register("LabelPosition", typeof(Thickness), typeof(DataAxis), new PropertyMetadata(new Thickness(0)));
        /// <summary>
        /// Get or Set LablePositionProperty
        /// </summary>
        public Thickness LabelPosition
        {
            get { return (Thickness)GetValue(LablePositionProperty); }
            set { SetValue(LablePositionProperty, value); }
        }
        /// <summary>
        ///  Identifies the Radius dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(DataAxis), new PropertyMetadata(double.NaN));
       /// <summary>
       /// Get or Set Radius property
       /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        /// <summary>
        ///  Identifies the polyPoints dependency property.
        /// </summary>
        public static readonly DependencyProperty PolyPointsProperty = DependencyProperty.Register("PolyPoints", typeof(PointCollection), typeof(DataAxis), new PropertyMetadata(null));
       /// <summary>
       /// Get or Set PolyPointsProperty
       /// </summary>
        public PointCollection PolyPoints
        {
            get { return (PointCollection)GetValue(PolyPointsProperty); }
            set { SetValue(PolyPointsProperty, value); }
        }
        /// <summary>
        ///  Identifies the X1 dependency property.
        /// </summary>
        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(DataAxis), new PropertyMetadata(double.NaN));
        /// <summary>
        /// Get or Set X1 property
        /// </summary>
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }
        /// <summary>
        ///  Identifies the X2 dependency property.
        /// </summary>
        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(DataAxis), new PropertyMetadata(double.NaN));
      /// <summary>
      /// Get or Set X2 property
      /// </summary>
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }
        /// <summary>
        ///  Identifies the Y1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(DataAxis), new PropertyMetadata(double.NaN));
       /// <summary>
       /// Get or Set Y1 property
       /// </summary>
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }
        /// <summary>
        ///  Identifies the Y2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(DataAxis), new PropertyMetadata(double.NaN));
       /// <summary>
       /// Get or Set Y2 property
       /// </summary>
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        /// <summary>
        /// Identifies the Label dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(DataAxis), new PropertyMetadata(new object()));
        /// <summary>
        /// get or Set LabelProperty
        /// </summary>
        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        ///  Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DataAxis), new PropertyMetadata(Orientation.Horizontal));
        /// <summary>
        /// Get or Set OrientationProperty
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); GoToVisualState(); }
        }

        /// <summary>
        ///  Identifies the Opposedposition dependency property.
        /// </summary>
        public static readonly DependencyProperty OpposedPositionProperty = DependencyProperty.Register("OpposedPosition", typeof(bool), typeof(DataAxis), new PropertyMetadata(false));
        /// <summary>
        /// Get or Set OpposedPositionProperty
        /// </summary>
        public bool OpposedPosition
        {
            get { return (bool)GetValue(OpposedPositionProperty); }
            set { SetValue(OpposedPositionProperty, value); GoToVisualState(); }
        }

        /// <summary>
        /// Method implementation for SetBinding to Dependency properties
        /// </summary>
        public void SetBinding()
        {
            Binding OrientationBind = new Binding();
            OrientationBind.Source = this.RelatedAxis;
            OrientationBind.Path = new PropertyPath("Orientation");
            OrientationBind.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(this, DataAxis.OrientationProperty, OrientationBind);

            Binding OpposedBind = new Binding();
            OpposedBind.Source = this.RelatedAxis;
            OpposedBind.Path = new PropertyPath("OpposedPosition");
            OpposedBind.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(this, DataAxis.OpposedPositionProperty, OpposedBind);

            Binding RotateBind = new Binding();
            RotateBind.Source = this.RelatedAxis;
            RotateBind.Path = new PropertyPath("LabelRotateAngle");
            RotateBind.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(this, DataAxis.LabelRotateAngleProperty, RotateBind);
        }

        /// <summary>
        /// Get or Set Axismargin property
        /// </summary>
        public Thickness AxisMargin
        {
            get
            {
                return new Thickness(double.IsNaN(this.X1) ? 0 : this.X1, double.IsNaN(this.Y1) ? 0 : this.Y1, 0, 0);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            GoToVisualState();
            SetBinding();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Method implementation for set corresponding VisualState manager value based on axis orientation
        /// </summary>
        public void GoToVisualState()
        {
            if (this.RelatedAxis != null)
            {
                if (!(this.RelatedAxis.ChartAxesProvider is IChartCartesianAxes) && this.Orientation == Orientation.Horizontal)
                {
                    VisualStateManager.GoToState(this, "Non_Cartesian_Axis_State", true);
                }
                else if (this.RelatedAxis.Orientation == Orientation.Horizontal && this.RelatedAxis.OpposedPosition == false)
                {
                    VisualStateManager.GoToState(this, "Horizontal_NotOpposed_State", true);
                }
                else if (this.RelatedAxis.Orientation == Orientation.Horizontal && this.RelatedAxis.OpposedPosition == true)
                {
                    VisualStateManager.GoToState(this, "Horizontal_Opposed_State", true);
                }
                else if (this.RelatedAxis.Orientation == Orientation.Vertical && this.RelatedAxis.OpposedPosition == false)
                {
                    VisualStateManager.GoToState(this, "Vertical_NotOpposed_State", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Vertical_Opposed_State", true);
                }
            }

        }

        #region revamp

        #endregion

        /// <summary>
        /// Idenfities LabelHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHeightProperty =
            DependencyProperty.Register("LabelHeight", typeof(double), typeof(DataAxis), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the LabelHeight. This is dependency property.
        /// </summary>
        /// <value>The LabelWidth Value.</value>
        internal double LabelHeight
        {
            get
            {
                return (double)GetValue(LabelHeightProperty);
            }

            set
            {
                SetValue(LabelHeightProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(double), typeof(DataAxis), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the LabelWidth. This is dependency property.
        /// </summary>
        /// <value>The LabelWidth Value.</value>
        internal double LabelWidth
        {
            get
            {
                return (double)GetValue(LabelWidthProperty);
            }

            set
            {
                SetValue(LabelWidthProperty, value);
            }
        }

        /// <summary>
        /// Idenfities Top dependency property.
        /// </summary>
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(DataAxis), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the Top. This is dependency property.
        /// </summary>
        /// <value>The Top Value.</value>
        internal double Top
        {
            get
            {
                return (double)GetValue(TopProperty);
            }

            set
            {
                SetValue(TopProperty, value);
            }
        }

        /// <summary>
        /// Idenfities Left dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(DataAxis), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the Left. This is dependency property.
        /// </summary>
        /// <value>The Left Value.</value>
        protected internal double Left
        {
            get
            {
                return (double)GetValue(LeftProperty);
            }

            set
            {
                SetValue(LeftProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelCornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelCornerRadiusProperty =
            DependencyProperty.Register("LabelCornerRadius", typeof(CornerRadius), typeof(DataAxis), new PropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Gets or sets the LabelCornerRadius. This is dependency property.
        /// </summary>
        /// <value>The CornerRadius.</value>
        public CornerRadius LabelCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(LabelCornerRadiusProperty);
            }

            set
            {
                SetValue(LabelCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelBorderThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBorderThicknessProperty =
            DependencyProperty.Register("LabelBorderThickness", typeof(Thickness), typeof(DataAxis), new PropertyMetadata(new Thickness()));

        /// <summary>
        /// Gets or sets the LabelBorderThickness. This is dependency property.
        /// </summary>
        /// <value>The Thickness.</value>
        public Thickness LabelBorderThickness
        {
            get
            {
                return (Thickness)GetValue(LabelBorderThicknessProperty);
            }

            set
            {
                SetValue(LabelBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBorderBrushProperty =
            DependencyProperty.Register("LabelBorderBrush", typeof(Brush), typeof(DataAxis), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the LabelBorderBrush. This is dependency property.
        /// </summary>
        /// <value>The Brush.</value>
        public Brush LabelBorderBrush
        {
            get
            {
                return (Brush)GetValue(LabelBorderBrushProperty);
            }

            set
            {
                SetValue(LabelBorderBrushProperty, value);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.RelatedAxis = null;
        }

        #endregion
    }
}

