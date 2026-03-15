#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
using Windows.UI;
using System.Collections;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Collections;
#endif


namespace Syncfusion.UI.Xaml.Gauges
{
    /// <summary>
    ///  It contains collection of ranges, pointers, labels, and ticks that help to
    /// visualize the data.
    /// </summary>
    public class CircularScale : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Gauges.CircularScale"/> class.
        /// </summary>
        /// <remarks>
        /// New instance of the CircularRangeCollection, CircularPointerCollection,
        /// CircularScaleTickCollection, CircularScaleLabelCollection classes are
        /// initialized.
        /// </remarks>
        public CircularScale()
        {
            this.DefaultStyleKey = typeof(CircularScale);            
            this.Ranges = new CircularRangeCollection();
            this.Pointers = new CircularPointerCollection();
            Binding labelFontBinding = new Binding() { Path = new PropertyPath("FontSize"), Source = this, Mode = BindingMode.TwoWay };
            SetBinding(LabelFontSizeProperty, labelFontBinding);
            this.LabelFontSize = FontSize;
         #if WPF  
          if (this.Pointers.Count == 0)
            {
                this.Pointers.Add(new CircularPointer() {IsDefaultPointer= true});
            }
#endif
            this.Ticks = new CircularScaleTickCollection();
            this.SmallTicks = new CircularScaleTickCollection();
            this.Labels = new CircularScaleLabelCollection();
            this.Ranges.CollectionChanged += Ranges_CollectionChanged;
            Pointers.CollectionChanged += Pointers_CollectionChanged;
            this.SizeChanged += CircularScale_SizeChanged;
            PathSegmentCollection tempcoll = new PathSegmentCollection();
            ArcSegment arcseg = new ArcSegment() { RotationAngle = 0 };
            Binding sweepbinding = new Binding() { Path = new PropertyPath("SweepDirection"), Source = this, Mode = BindingMode.TwoWay };
            Binding islargearcbinding = new Binding() { Path = new PropertyPath("IsLargeArc"), Source = this, Mode = BindingMode.TwoWay };
            Binding halfsizebinding = new Binding() { Path = new PropertyPath("SemiSize"), Source = this, Mode = BindingMode.TwoWay };
            Binding rangeendptbinding = new Binding() { Path = new PropertyPath("EndPoint"), Source = this, Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(arcseg, ArcSegment.SweepDirectionProperty, sweepbinding);
            BindingOperations.SetBinding(arcseg, ArcSegment.IsLargeArcProperty, islargearcbinding);
            BindingOperations.SetBinding(arcseg, ArcSegment.SizeProperty, halfsizebinding);
            BindingOperations.SetBinding(arcseg, ArcSegment.PointProperty, rangeendptbinding);
            tempcoll.Add(arcseg);
            ScaleSeg = tempcoll;
        }

        #endregion

        #region Event Tagged Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            GaugeSize = availableSize;
            ResetScale();            
            return base.MeasureOverride(availableSize);
        }

        private void CircularScale_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GaugeSize = e.NewSize;
            ResetScale();
        }

        internal void ResetScale()
        {
            SetRimBindings();
            SetRangeBindings();
            SetPointerBindings();
            CreateTicks();
            CreateLabels();
            CalculateMargins();
        }

        #endregion

        #region Internal Methods

        private double ValueToAngle(double Value)
        {
            double angle;
            angle = StartAngle + ((SweepAngle / Math.Abs(EndValue - StartValue)) * Value);
            return angle;
        }

        internal double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        internal void CalculateMargins()
        {
            double rangeStrokeThickness=0;
            double rangePointerThickness =0;
            double symbolPointerDiameter = 0;
            double tickLength = TickLength > SmallTickLength ? TickLength : SmallTickLength;
            double crossTick = tickLength > RimStrokeThickness ? tickLength : RimStrokeThickness;
            if (this.Ranges.Count > 0)
            {
                foreach (CircularRange range in this.Ranges)
                {
                    rangeStrokeThickness = rangeStrokeThickness < range.StrokeThickness ? range.StrokeThickness : rangeStrokeThickness;
                }
            }
            if (this.Pointers.Count > 0)
            {
                symbolPointerDiameter = this.Pointers[0].SymbolPointerHeight;
                foreach (CircularPointer pointer in this.Pointers)
                {
                    rangePointerThickness = rangePointerThickness < pointer.RangePointerStrokeThickness ? pointer.RangePointerStrokeThickness : rangePointerThickness;
                    if (pointer.PointerType == PointerType.SymbolPointer)
                    {
                        symbolPointerDiameter = symbolPointerDiameter < pointer.SymbolPointerHeight ? symbolPointerDiameter : pointer.SymbolPointerHeight;
                    }
                }
            }
            #region SettingMargin
            switch (RangePosition)
            {
                case Gauges.RangePosition.SetAsGaugeRim:
                    {
                        switch (TickPosition)
                        {
                            case Gauges.TickPosition.Inside:                                
                                {
                                    switch (LabelPosition)
                                    {
                                        case Gauges.LabelPosition.Inside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.ScaleMargin = new Thickness(RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(RimStrokeThickness);
                                                            this.LabelsMargin = new Thickness(RimStrokeThickness + tickLength + 10);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangePointerThickness / 2);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + RimStrokeThickness);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + RimStrokeThickness + tickLength);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                        
                                                }
                                                break;
                                            }
                                        case Gauges.LabelPosition.Outside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.LabelsMargin = new Thickness(10);
                                                            this.ScaleMargin = new Thickness(ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(RimStrokeThickness + ScaleMaxWidth);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangePointerThickness / 2);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + RimStrokeThickness + ScaleMaxWidth);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case Gauges.TickPosition.Cross:
                                {
                                    switch (LabelPosition)
                                    {
                                        case Gauges.LabelPosition.Inside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.TicksMargin = new Thickness(0);
                                                            this.ScaleMargin = new Thickness(crossTick / 2);
                                                            this.RangesMargin = new Thickness(crossTick / 2);
                                                            this.LabelsMargin = new Thickness(crossTick + 10);
                                                            this.RangePointerMargin = new Thickness(crossTick + 10 + ScaleMaxWidth + rangePointerThickness / 2);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + crossTick / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + crossTick / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + crossTick + 10);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                        case Gauges.LabelPosition.Outside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.LabelsMargin = new Thickness(10);
                                                            this.TicksMargin = new Thickness(ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(crossTick / 2 + ScaleMaxWidth);
                                                            this.RangesMargin = new Thickness(crossTick / 2 + ScaleMaxWidth);
                                                            this.RangePointerMargin = new Thickness(crossTick + 10 + ScaleMaxWidth + rangePointerThickness / 2);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + crossTick / 2 + ScaleMaxWidth);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + crossTick / 2 + ScaleMaxWidth);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                    }

                                    break;
                                }
                            case Gauges.TickPosition.Outside:
                                {
                                    switch (LabelPosition)
                                    {
                                        case Gauges.LabelPosition.Inside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.TicksMargin = new Thickness(0);
                                                            this.ScaleMargin = new Thickness(tickLength + RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(tickLength + RimStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(RimStrokeThickness + tickLength + 10);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangePointerThickness / 2);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + tickLength + RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + tickLength + RimStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + RimStrokeThickness + tickLength);
                                                            foreach (CircularRange range in Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                        case Gauges.LabelPosition.Outside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.LabelsMargin = new Thickness(10);
                                                            this.TicksMargin = new Thickness(ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(ScaleMaxWidth + tickLength + RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(ScaleMaxWidth + tickLength + RimStrokeThickness / 2);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = RimStrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + ScaleMaxWidth + tickLength + RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + ScaleMaxWidth + tickLength + RimStrokeThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = this.RimStrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                    }

                                    break;
                                }                            
                        }
                        break;
                    }
                case Gauges.RangePosition.Inside:
                    {
                        switch (TickPosition)
                        {
                            case Gauges.TickPosition.Inside:
                                {
                                    switch (LabelPosition)
                                    {
                                        case Gauges.LabelPosition.Inside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.ScaleMargin = new Thickness(RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(RimStrokeThickness);
                                                            this.LabelsMargin = new Thickness(RimStrokeThickness + tickLength + 10);
                                                            this.RangesMargin = new Thickness(RimStrokeThickness + tickLength + ScaleMaxWidth + rangeStrokeThickness / 2);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + RimStrokeThickness);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + RimStrokeThickness + tickLength);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + RimStrokeThickness + tickLength + ScaleMaxWidth + rangeStrokeThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                        case Gauges.LabelPosition.Outside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.LabelsMargin = new Thickness(10);
                                                            this.ScaleMargin = new Thickness(ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(ScaleMaxWidth + RimStrokeThickness);
                                                            this.RangesMargin = new Thickness(ScaleMaxWidth + RimStrokeThickness + tickLength + rangeStrokeThickness / 2);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + ScaleMaxWidth + RimStrokeThickness);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + ScaleMaxWidth + RimStrokeThickness + tickLength + rangeStrokeThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case Gauges.TickPosition.Cross:
                                {
                                    switch (LabelPosition)
                                    {
                                        case Gauges.LabelPosition.Inside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.TicksMargin = new Thickness(0);
                                                            this.ScaleMargin = new Thickness(crossTick / 2);
                                                            this.LabelsMargin = new Thickness(crossTick + 10);
                                                            this.RangesMargin = new Thickness(crossTick + ScaleMaxWidth + rangeStrokeThickness / 2);
                                                            this.RangePointerMargin = new Thickness(crossTick + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + crossTick / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + crossTick + 10);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + crossTick + ScaleMaxWidth + rangeStrokeThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                        case Gauges.LabelPosition.Outside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.LabelsMargin = new Thickness(10);
                                                            this.TicksMargin = new Thickness(ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(ScaleMaxWidth + crossTick / 2);
                                                            this.RangesMargin = new Thickness(ScaleMaxWidth + crossTick + rangeStrokeThickness / 2);
                                                            this.RangePointerMargin = new Thickness(crossTick + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + ScaleMaxWidth + crossTick / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + ScaleMaxWidth + crossTick + rangeStrokeThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                    }

                                    break;
                                }
                            case Gauges.TickPosition.Outside:
                                {
                                    switch (LabelPosition)
                                    {
                                        case Gauges.LabelPosition.Inside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.TicksMargin = new Thickness(0);
                                                            this.ScaleMargin = new Thickness(tickLength + RimStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(tickLength + RimStrokeThickness + 10);
                                                            this.RangesMargin = new Thickness(RimStrokeThickness + tickLength + ScaleMaxWidth + rangeStrokeThickness / 2);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + tickLength + RimStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + tickLength + RimStrokeThickness);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + RimStrokeThickness + tickLength + ScaleMaxWidth + rangeStrokeThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                        case Gauges.LabelPosition.Outside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.LabelsMargin = new Thickness(10);
                                                            this.TicksMargin = new Thickness(ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(tickLength + ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(RimStrokeThickness + tickLength + ScaleMaxWidth + rangeStrokeThickness / 2);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + tickLength + ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + RimStrokeThickness + tickLength + ScaleMaxWidth + rangeStrokeThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                    }

                                    break;
                                }
                        }
                        break;
                    }
                case Gauges.RangePosition.Outside:
                    {
                        switch (TickPosition)
                        {
                            case Gauges.TickPosition.Inside:
                                {
                                    switch (LabelPosition)
                                    {
                                        case Gauges.LabelPosition.Inside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.RangesMargin = new Thickness(rangeStrokeThickness / 2);
                                                            this.ScaleMargin = new Thickness(rangeStrokeThickness + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangeStrokeThickness + RimStrokeThickness);
                                                            this.LabelsMargin = new Thickness(tickLength + 10 + rangeStrokeThickness + RimStrokeThickness);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + rangeStrokeThickness / 2);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + RimStrokeThickness);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + tickLength + rangeStrokeThickness + RimStrokeThickness);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                        case Gauges.LabelPosition.Outside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.RangesMargin = new Thickness(rangeStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangeStrokeThickness + 10);
                                                            this.ScaleMargin = new Thickness(rangeStrokeThickness + ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangeStrokeThickness + ScaleMaxWidth + RimStrokeThickness);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + tickLength + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + rangeStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + rangeStrokeThickness);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + ScaleMaxWidth + RimStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + ScaleMaxWidth + RimStrokeThickness);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case Gauges.TickPosition.Cross:
                                {
                                    switch (LabelPosition)
                                    {
                                        case Gauges.LabelPosition.Inside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.RangesMargin = new Thickness(rangeStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangeStrokeThickness);
                                                            this.ScaleMargin = new Thickness(rangeStrokeThickness + crossTick / 2);
                                                            this.LabelsMargin = new Thickness(rangeStrokeThickness + crossTick + 10);
                                                            this.RangePointerMargin = new Thickness(crossTick + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + rangeStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + rangeStrokeThickness);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + crossTick / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + rangeStrokeThickness + crossTick);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                        case Gauges.LabelPosition.Outside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.RangesMargin = new Thickness(rangeStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangeStrokeThickness + 10);
                                                            this.TicksMargin = new Thickness(rangeStrokeThickness + ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(rangeStrokeThickness + ScaleMaxWidth + crossTick / 2);
                                                            this.RangePointerMargin = new Thickness(crossTick + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + rangeStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + rangeStrokeThickness);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + ScaleMaxWidth + crossTick / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                    }

                                    break;
                                }
                            case Gauges.TickPosition.Outside:
                                {
                                    switch (LabelPosition)
                                    {
                                        case Gauges.LabelPosition.Inside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.RangesMargin = new Thickness(rangeStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangeStrokeThickness);
                                                            this.ScaleMargin = new Thickness(rangeStrokeThickness + tickLength + RimStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(tickLength + 10 + rangeStrokeThickness + RimStrokeThickness);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + crossTick + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + rangeStrokeThickness / 2);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + rangeStrokeThickness);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + tickLength + RimStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + tickLength + rangeStrokeThickness + RimStrokeThickness);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                        case Gauges.LabelPosition.Outside:
                                            {
                                                switch (RangePointerPosition)
                                                {
                                                    case Gauges.RangePointerPosition.Inside:
                                                        {
                                                            this.RangesMargin = new Thickness(rangeStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangeStrokeThickness + 10);
                                                            this.TicksMargin = new Thickness(rangeStrokeThickness + ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(rangeStrokeThickness + TickLength + RimStrokeThickness / 2 + ScaleMaxWidth);
                                                            this.RangePointerMargin = new Thickness(RimStrokeThickness + crossTick + 10 + ScaleMaxWidth + rangeStrokeThickness + rangePointerThickness / 2);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }
                                                    case Gauges.RangePointerPosition.Outside:
                                                        {
                                                            this.RangePointerMargin = new Thickness(rangePointerThickness / 2);
                                                            this.RangesMargin = new Thickness(rangePointerThickness + rangeStrokeThickness / 2);
                                                            this.LabelsMargin = new Thickness(rangePointerThickness + 10 + rangeStrokeThickness);
                                                            this.TicksMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + ScaleMaxWidth);
                                                            this.ScaleMargin = new Thickness(rangePointerThickness + rangeStrokeThickness + TickLength + RimStrokeThickness / 2 + ScaleMaxWidth);
                                                            foreach (CircularRange range in this.Ranges)
                                                            {
                                                                range.RangeSize = range.StrokeThickness;
                                                            }
                                                            break;
                                                        }

                                                }
                                                break;
                                            }
                                    }

                                    break;
                                }
                        }
                        break;
                    }
            }
            #endregion
            if (Pointers.Count > 0)
                this.SymbolPointerMargin = new Thickness(TicksMargin.Top );
           
            SetRimBindings();
            SetRangeBindings();
            SetPointerBindings();
            CreateTicks();
            CreateLabels();

        }

        internal void SetRangeBindings()
        {
            double startVal = StartValue < EndValue ? StartValue : EndValue;
            double endVal = EndValue > StartValue ? EndValue : StartValue;
            foreach (CircularRange range in Ranges)
            {
                double width = GaugeSize.Width  - (RangesMargin.Left + RangesMargin.Right );
                double height = GaugeSize.Height - (RangesMargin.Top + RangesMargin.Bottom );
                width = width < 0 ? 0 : width;
                height = height < 0 ? 0 : height;
                range.ParentScale = this;
                range.ParentStartAngle = StartAngle;
                range.ParentSweepAngle = SweepAngle;
                range.ParentStartValue = startVal;
                range.ParentEndValue = endVal;
                range.SweepDirection = SweepDirection;
                range.ParentSize = new Size(width,height);
            }
        }

        internal void SetRimBindings()
        {
            double radX = (GaugeSize.Width - (ScaleMargin.Left + ScaleMargin.Right )) / 2;
            double radY = (GaugeSize.Height - (ScaleMargin.Top + ScaleMargin.Bottom )) / 2;
            radX = radX < 0 ? 0 : radX;
            radY = radY < 0 ? 0 : radY;
            SemiSize = new Size(radX, radY);
            double endAngle;
            if (this.SweepDirection == SweepDirection.Clockwise)
            {
                if (SweepAngle == 360)
                {
                    endAngle = (StartAngle + 359.99) % 360;
                }
                else
                    endAngle = (StartAngle + SweepAngle) % 360;
            }
            else
            {
                if (SweepAngle == 360)
                {
                    endAngle = (StartAngle - 359.99) % 360;
                }
                else
                    endAngle = (StartAngle - SweepAngle) % 360;
            }
            StartPoint = new Point(radX + radX * Math.Cos(DegToRad(StartAngle)), radX + radX * Math.Sin(DegToRad(StartAngle)));
            EndPoint = new Point(radY + radY * Math.Cos(DegToRad(endAngle)), radY + radY * Math.Sin(DegToRad(endAngle)));
           
            IsLargeArc = SweepAngle > 180;
        }
        
        internal void SetPointerBindings()
        {
            double startVal = StartValue < EndValue ? StartValue : EndValue;
            double endVal = EndValue > StartValue ? EndValue : StartValue;
            foreach (CircularPointer pointer in Pointers)
            {
                double width = (GaugeSize.Width  - (RangePointerMargin.Left + RangePointerMargin.Right));
                double height = (GaugeSize.Height  - (RangePointerMargin.Top + RangePointerMargin.Bottom));
                width = width < 0 ? 0 : width;
                height = height < 0 ? 0 : height;
                pointer.ParentStartAngle = StartAngle;
                pointer.ParentSweepAngle = SweepAngle;
                pointer.ParentStartValue = startVal;
                pointer.ParentEndValue = endVal;
                pointer.SweepDirection = SweepDirection;
                pointer.AvailSize = new Size(width, height);
                pointer.ParentSize = new Size(GaugeSize.Width, GaugeSize.Height);
                pointer.RangePointerMargin = RangePointerMargin;
                pointer.SymbolPointerMargin= SymbolPointerMargin;
            }
        }
        
        internal void CreateTicks()
        {
            if (StartValue != EndValue || Interval > (Math.Abs(StartValue - EndValue)))
            {
                Ticks.Clear();
                if (Interval <= 0 || double.IsNaN(Interval))// || Interval > (Math.Abs(StartValue - EndValue)))// || (ValueDiff != (Math.Abs(StartValue - EndValue)) && ((Interval <= 0 || double.IsNaN(Interval) || Interval > (Math.Abs(StartValue - EndValue))))))
                {
                    Interval = (Math.Abs(StartValue - EndValue) / 10);
                    ValueDiff = (Math.Abs(StartValue - EndValue));
                }
                MinorTicksPerInterval = MinorTicksPerInterval < 0 ? 1 : MinorTicksPerInterval;
                int noofmajorticks = (int)(Math.Abs(StartValue - EndValue) / Interval) + 1;
                int partialminorticks = (int)((Math.Abs(StartValue - EndValue) - ((noofmajorticks-1 ) * Interval)) / (Interval / (MinorTicksPerInterval + 1)));
                int noofticks = noofmajorticks + MinorTicksPerInterval * (noofmajorticks - 1) + partialminorticks;
                
                double MinorInterval = Interval / (MinorTicksPerInterval + 1);
                double count = StartValue;
                
                for (int i = 0; i < noofticks; i++)
                {
                    CircularRange correspRange = null;
                    if (Ranges.Count > 0 && BindRangeStrokeToTicks)
                    {
                        foreach (CircularRange range in Ranges)
                        {
                            if (count <= range.EndValue && count >= range.StartValue)
                            {
                                correspRange = range;
                                break;
                            }
                        }
                    }
                    
                    if ((i) % (MinorTicksPerInterval + 1) == 0)
                    {
                        Ticks.Add(new CircularScaleTick()
                        {
                            Length = TickLength,
                            TickStrokeThickness = TickStrokeThickness,
                            TickStroke = correspRange != null ? correspRange.Stroke : TickStroke,
                            Ticktype= TickType.Major
                        });
                    }
                    else
                    {
                        Ticks.Add(new CircularScaleTick()
                        {
                            Length = SmallTickLength,
                            TickStrokeThickness = SmallTickStrokeThickness,
                            TickStroke = correspRange != null ? correspRange.Stroke : SmallTickStroke,
                            Ticktype= TickType.Minor
                        });

                    }
                    
                    count += MinorInterval;
                }

                CircularTicksAngularSpace = (SweepAngle / Math.Abs(EndValue - StartValue)) * MinorInterval;
            }
        }

        private object CalculateSmartAxisLabels(NumericScaleType ScaleType, double doublevalue)
        {
            double content = doublevalue;
            double value = doublevalue;
            int? fractionalDigit = NoOfFractionalDigit;
 #if !SILVERLIGHT    && !WINDOWSPHONE_7 
            if (fractionalDigit != null)
            {
switch (ScaleType)
{
                case NumericScaleType.Auto:
                    if (doublevalue > 999 && doublevalue <= 999999)
                    {
                        value = Math.Round(doublevalue / 1000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " K";
                    }
                    else if (doublevalue > 999999 && doublevalue <= 999999999)
                    {
                        value = Math.Round(doublevalue / 1000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " M";
                    }
                    else if (doublevalue > 999999999 && doublevalue <= 999999999999)
                    {
                        value = Math.Round(doublevalue / 1000000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " B";
                    }
                    else if (doublevalue > 999999999999 && doublevalue <= 999999999999999)
                    {
                        value = Math.Round(doublevalue / 1000000000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " T";
                    }
                    else if (doublevalue > 999999999999999 && doublevalue <= 99999999999999999)
                    {
                        value = Math.Round(doublevalue / 1000000000000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " Qd";
                    }
                    else if (doublevalue > 999999999999999 )
                    {
                        value = Math.Round(doublevalue / 1000000000000000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " Qt";
                    }
                    else
                        value = Math.Round(doublevalue, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value ;
                        
                case NumericScaleType.Thousands:
                    {
                        value = Math.Round(doublevalue / 1000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " K";
                    }
                case NumericScaleType.Millions:
                    {
                        value = Math.Round(doublevalue / 1000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " M";
                    }
                case NumericScaleType.Billions:
                    {
                        value = Math.Round(doublevalue / 1000000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " B";
                    }
                case NumericScaleType.Trillions:
                    {
                        value = Math.Round(doublevalue / 1000000000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " T";
                    }
                case NumericScaleType.Quadrillions:
                    {
                        value = Math.Round(doublevalue / 1000000000000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " Qd";
                    }
                case NumericScaleType.Quintillions:
                    {
                        value = Math.Round(doublevalue / 1000000000000000000, (int)fractionalDigit, MidpointRounding.AwayFromZero);
                        return value + " Qt";
                    }
            }
}
  #endif         
            return content;
        }


        private object CalculateFractionalLabels(double doublevalue)
        {
            double content = doublevalue;
            double value = doublevalue;
            int? fractionalDigit = NoOfFractionalDigit;
#if !SILVERLIGHT && !WINDOWSPHONE_7
            if (fractionalDigit != null)
            {
               value = Math.Round(doublevalue, (int)fractionalDigit, MidpointRounding.AwayFromZero);
               return value;
            }
#endif
            return content;
        }

        
        internal void CreateLabels()
        {
            Labels.Clear();
			string labVal = "";
            string labContent = null;
            double startVal = StartValue < EndValue ? StartValue : EndValue;
            double endVal = EndValue > StartValue ? EndValue : StartValue;
            for (double i = startVal; i <= endVal; i =Math.Round(i+ Interval,5))
            {
                CircularRange correspRange = null;

                if (Ranges.Count > 0 && BindRangeStrokeToLabels)
                {
                    foreach (CircularRange range in Ranges)
                    {
                        if (i <= range.EndValue && i >= range.StartValue)
                        {
                            correspRange = range;
                            break;
                        }
                    }
                }
                
                if (EnableSmartLabels)
                {
                    labVal = CalculateSmartAxisLabels(NumericScaleType, i).ToString();
                }
                else
                {
                   labVal = CalculateFractionalLabels(i).ToString();
                }
                labContent = LabelPrefix + " " + labVal + " "+LabelPostfix;
                var circularLabel = new CircularScaleLabel
                {
                    Content = labContent,
                    Foreground = correspRange != null ? correspRange.Stroke : LabelStroke,
                    LabelFontSize = LabelFontSize
                };
                var labelStrokeBinding = new Binding();
                if (correspRange != null)
                {
                    labelStrokeBinding.Source = correspRange;
                    labelStrokeBinding.Path = new PropertyPath("Stroke");
                }
                else
                {
                    labelStrokeBinding.Source = this;
                    labelStrokeBinding.Path = new PropertyPath("LabelStroke");
                }
                BindingOperations.SetBinding(circularLabel, CircularScaleLabel.ForegroundProperty, labelStrokeBinding);
                Labels.Add(circularLabel);
            }
            CircularLabelsAngularSpace = (SweepAngle / Math.Abs(endVal - startVal)) * Interval;

        }
        
        #endregion

        #region Collection Property Changed

        void Ranges_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {           
            if (this.Istemplateapplied)
            {
                SetRangeBindings();
                CreateTicks();
                CreateLabels();
                CalculateMargins();
            }
        }

        void Pointers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
         #if WPF
           Pointers  = (sender as CircularPointerCollection);
            
            foreach (CircularPointer item in (IEnumerable)Pointers.ToList())
            {
                if (item.IsDefaultPointer == true)
                {
                    Pointers.Remove(item);
                }
            }
#endif            
            if (this.Istemplateapplied)
            {
                SetPointerBindings();
                CalculateMargins();
            }
        }

        #endregion

        #region Properties



        public PathSegmentCollection ScaleSeg
        {
            get { return (PathSegmentCollection)GetValue(ScaleSegProperty); }
            set { SetValue(ScaleSegProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleSeg.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleSegProperty =
            DependencyProperty.Register("ScaleSeg", typeof(PathSegmentCollection), typeof(CircularScale), new PropertyMetadata(new PathSegmentCollection()));

        

        internal double ScaleMaxWidth
        {
            get { return (double)GetValue(ScaleMaxWidthProperty); }
            set { SetValue(ScaleMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleMaxWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ScaleMaxWidthProperty =
            DependencyProperty.Register("ScaleMaxWidth", typeof(double), typeof(CircularScale), new PropertyMetadata(0d));


        /// <summary>
        /// Gets or sets the numeric scale type of the scale.
        /// </summary>
        /// <remarks>
        /// Default value of this property is Auto. This property remains active only when EnableSmartLabels property is set to true.
        /// </remarks>
        /// <value>
        /// NumericScaleType
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.EnableSmartLabels = true;
        ///             scale.NoOfFractionalDigit = 1;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        public NumericScaleType NumericScaleType
        {
            get { return (NumericScaleType)GetValue(NumericScaleTypeProperty); }
            set { SetValue(NumericScaleTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumericScaleType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumericScaleTypeProperty =
            DependencyProperty.Register("NumericScaleType", typeof(NumericScaleType), typeof(CircularScale), new PropertyMetadata(NumericScaleType.Auto, OnLabelStrokeChanged));



        /// <summary>
        /// Gets or sets a value indicating whether to display smart labels or not .
        /// </summary>
        /// <remarks>
        /// This property helps to set the Numeric scale type to the labels displayed in
        /// scale
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.EnableSmartLabels = true;
        ///             scale.NoOfFractionalDigit = 1;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        public bool EnableSmartLabels
        {
            get { return (bool)GetValue(EnableSmartLabelsProperty); }
            set { SetValue(EnableSmartLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableSmartLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableSmartLabelsProperty =
            DependencyProperty.Register("EnableSmartLabels", typeof(bool), typeof(CircularScale), new PropertyMetadata(false, OnLabelStrokeChanged));



        /// <summary>
        /// Gets or sets the number of fractional digit has to be displayed in the Scale
        /// labels..
        /// </summary>
        /// <remarks>
        /// Default value of this property is 1. If the values have large no of fractional
        /// values then the fractional value count is reduced to one.
        /// </remarks>
        /// <value>
        /// int
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.EnableSmartLabels = true;
        ///             scale.NoOfFractionalDigit = 1;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        public int NoOfFractionalDigit
        {
            get { return (int)GetValue(NoOfFractionalDigitProperty); }
            set { SetValue(NoOfFractionalDigitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoOfFractionalDigit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoOfFractionalDigitProperty =
            DependencyProperty.Register("NoOfFractionalDigit", typeof(int), typeof(CircularScale), new PropertyMetadata(0, OnLabelStrokeChanged));

        
        
        internal Thickness ScaleMargin
        {
            get { return (Thickness)GetValue(ScaleMarginProperty); }
            set { SetValue(ScaleMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeScaleMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ScaleMarginProperty =
            DependencyProperty.Register("ScaleMargin", typeof(Thickness), typeof(CircularScale), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Gets or sets the brush that describes the RimStroke value of the CircularScale.
        /// </summary>
        /// <remarks>
        /// RimStroke property will remains effective until the value of the
        /// RangePostion property is not set to 'SetAsScaleRim'.
        /// </remarks>
        /// <value>
        /// Brush
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.RimStroke = new SolidColorBrush(Colors.Red);
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush RimStroke
        {
            get { return (Brush)GetValue(RimStrokeProperty); }
            set { SetValue(RimStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RimStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RimStrokeProperty =
            DependencyProperty.Register("RimStroke", typeof(Brush), typeof(CircularScale), new PropertyMetadata(new SolidColorBrush(Colors.White)));


        /// <summary>
        /// Gets or sets the thickness of the RimStroke.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularScale scale = new CircularScale();
        ///            scale.RimStrokeThickness =3;
        ///            gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double RimStrokeThickness
        {
            get { return (double)GetValue(RimStrokeThicknessProperty); }
            set { SetValue(RimStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RimStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RimStrokeThicknessProperty =
            DependencyProperty.Register("RimStrokeThickness", typeof(double), typeof(CircularScale), new PropertyMetadata(10d));

        /// <summary>
        /// Gets or sets LabelPosition that decides the position of Labels in the CircularScale.
        /// </summary>
        /// <remarks>
        /// The Labels in the scale can be placed inside the scale or outside the scale by
        /// choosing the options avail in the LabelPosition property. Options are
        /// 1. Inside (Default) : Places the Labels inside the scale.
        /// 2. Outside          : Places the Labels outside the scale.
        /// </remarks>
        /// <value>
        /// LabelPosition
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.LabelPosition = LabelPosition.Outside;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public LabelPosition LabelPosition
        {
            get { return (LabelPosition)GetValue(LabelPositionProperty); }
            set { SetValue(LabelPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register("LabelPosition", typeof(LabelPosition), typeof(CircularScale), new PropertyMetadata(LabelPosition.Inside, OnRangePositionChanged));

        /// <summary>
        /// Gets or sets TickPosition that decides the position of ticks in the CircularScale.
        /// </summary>
        /// <remarks>
        /// The Ticks in the scale can be placed inside the scale, outside or on top of the scale by
        /// choosing the options avail in the TickPosition property. Options are
        /// 1. Inside (Default) : Places the Ticks inside the scale.
        /// 2. Outside          : Places the Ticks outside the scale.
        /// 3. Cross            : Places the Ticks on top of the scale.
        /// </remarks>
        /// <value>
        /// TickPosition
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.TickPosition = TickPosition.Outside;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public TickPosition TickPosition
        {
            get { return (TickPosition)GetValue(TickPositionProperty); }
            set { SetValue(TickPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickPositionProperty =
            DependencyProperty.Register("TickPosition", typeof(TickPosition), typeof(CircularScale), new PropertyMetadata(TickPosition.Inside, OnRangePositionChanged));


        /// <summary>
        /// Gets or sets RangePointerPosition that decides the position of RangePointer in the CircularScale.
        /// </summary>
        /// <remarks>
        /// The RangePointer in the scale can be placed inside the scale or outside the scale by
        /// choosing the options avail in the RangePointerPosition property. Options are
        /// 1. Inside (Default) : Places the RangePointer inside the scale.
        /// 2. Outside          : Places the RangePointer outside the scale.
        /// </remarks>
        /// <value>
        /// RangePointerPosition
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.RangePointerPosition = RangePointerPosition.Outside;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>    
        [ClassReference(IsReviewed = false)]
        public RangePointerPosition RangePointerPosition
        {
            get { return (RangePointerPosition)GetValue(RangePointerPositionProperty); }
            set { SetValue(RangePointerPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePointerPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePointerPositionProperty =
            DependencyProperty.Register("RangePointerPosition", typeof(RangePointerPosition), typeof(CircularScale), new PropertyMetadata(RangePointerPosition.Inside, OnRangePositionChanged));


        /// <summary>
        /// Gets or sets RangePosition that decides the position of ranges in the CircularScale.
        /// </summary>
        /// <remarks>
        /// The Ranges in the scale can be placed inside the scale or outside the scale by
        /// choosing the options avail in the RangePosition property. Options are
        /// 1. Inside (Default) : Places the Ranges inside the scale.
        /// 2. Outside          : Places the Ranges outside the scale.
        /// 3. SetAsScaleRim    : Placed the Ranges on the Scale Rim.
        /// </remarks>
        /// <value>
        /// RangePosition
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.RangePosition = RangePosition.Outside;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>    
        [ClassReference(IsReviewed = false)]
        public RangePosition RangePosition
        {
            get { return (RangePosition)GetValue(RangePositionProperty); }
            set { SetValue(RangePositionProperty, value); }
        }
        // Using a DependencyProperty as the backing store for RangePosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePositionProperty =
            DependencyProperty.Register("RangePosition", typeof(RangePosition), typeof(CircularScale), new PropertyMetadata(RangePosition.SetAsGaugeRim, OnRangePositionChanged));


        internal Thickness TicksMargin
        {
            get { return (Thickness)GetValue(TicksMarginProperty); }
            set { SetValue(TicksMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TicksMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TicksMarginProperty =
            DependencyProperty.Register("TicksMargin", typeof(Thickness), typeof(CircularScale), new PropertyMetadata(new Thickness(0)));


        internal Thickness LabelsMargin
        {
            get { return (Thickness)GetValue(LabelsMarginProperty); }
            set { SetValue(LabelsMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelsMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LabelsMarginProperty =
            DependencyProperty.Register("LabelsMargin", typeof(Thickness), typeof(CircularScale), new PropertyMetadata(new Thickness(0)));



        internal Thickness SymbolPointerMargin
        {
            get { return (Thickness)GetValue(SymbolPointerMarginProperty); }
            set { SetValue(SymbolPointerMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolPointerMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SymbolPointerMarginProperty =
            DependencyProperty.Register("SymbolPointerMargin", typeof(Thickness), typeof(CircularScale), new PropertyMetadata(new Thickness(0)));

        

        internal Thickness RangePointerMargin
        {
            get { return (Thickness)GetValue(RangePointerMarginProperty); }
            set { SetValue(RangePointerMarginProperty, value); }    
        }

        // Using a DependencyProperty as the backing store for RangePointerMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangePointerMarginProperty =
            DependencyProperty.Register("RangePointerMargin", typeof(Thickness), typeof(CircularScale), new PropertyMetadata(new Thickness(0)));

        

        internal Thickness RangesMargin
        {
            get { return (Thickness)GetValue(RangesMarginProperty); }
            set { SetValue(RangesMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangesMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangesMarginProperty =
            DependencyProperty.Register("RangesMargin", typeof(Thickness), typeof(CircularScale), new PropertyMetadata(new Thickness(0)));


        internal CircularScaleTickCollection Ticks
        {
            get { return (CircularScaleTickCollection)GetValue(TicksProperty); }
            private set { SetValue(TicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ticks.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(CircularScaleTickCollection), typeof(CircularScale), new PropertyMetadata(null));



        internal ObservableCollection<CircularScaleTick> SmallTicks
        {
            get { return (ObservableCollection<CircularScaleTick>)GetValue(SmallTicksProperty); }
            private set { SetValue(SmallTicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallTicks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallTicksProperty =
            DependencyProperty.Register("SmallTicks", typeof(ObservableCollection<CircularScaleTick>), typeof(CircularScale), new PropertyMetadata(null));

        
        internal CircularScaleLabelCollection Labels
        {
            get { return (CircularScaleLabelCollection)GetValue(LabelsProperty); }
            private set { SetValue(LabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Labels.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(CircularScaleLabelCollection), typeof(CircularScale), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the collection of Ranges to the CircularScale.
        /// </summary>
        /// <remarks>
        /// A range is a visual element which begins and ends at specified values within a scale. 
        /// These start and end values are set by the StartValue and EndValue properties of Range. 
        /// Range’s UI is customized by the Stroke and StrokeThickness Properties.
        /// </remarks>
        /// <value>
        /// CircularRangeCollection
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularRange range1 = new CircularRange();
        ///            range1.StartValue = 0;
        ///            range1.EndValue = 40;
        ///            range1.Stroke = new SolidColorBrush(Colors.Green);
        ///            range1.StrokeThickness = 10;
        ///            CircularRange range2 = new CircularRange();
        ///            range2.StartValue = 40;
        ///            range2.EndValue = 60;
        ///            range2.Stroke = new SolidColorBrush(Colors.Yellow);
        ///            range2.StrokeThickness = 10;
        ///            CircularRange range3 = new CircularRange();
        ///            range3.StartValue = 60;
        ///            range3.EndValue = 100;
        ///            range3.Stroke = new SolidColorBrush(Colors.Red);
        ///            range3.StrokeThickness = 10;
        ///            gauge.MainScale.Ranges.Add(range1);
        ///            gauge.MainScale.Ranges.Add(range2);
        ///            gauge.MainScale.Ranges.Add(range3);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public CircularRangeCollection Ranges
        {
            get { return (CircularRangeCollection)GetValue(RangesProperty); }
            set { SetValue(RangesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ranges.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangesProperty =
            DependencyProperty.Register("Ranges", typeof(CircularRangeCollection), typeof(CircularScale), new PropertyMetadata(null, new PropertyChangedCallback(OnRangesChanged)));

        private static void OnRangesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            CircularScale scale = (CircularScale)d;
            if(scale.Istemplateapplied)
            scale.SetRangeBindings();
        }


        /// <summary>
        /// Gets or sets collection of Pointers to the CircularScale.
        /// </summary>
        /// <remarks>
        /// User can add multiple pointers to the gauge to point at multiple values on the
        /// same scale. This can be useful for showing a low and a high value at the same
        /// time. Value of the pointer is set by the Value property.
        /// </remarks>
        /// <value>
        /// CircularPointerCollection
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer needlePointer = new CircularPointer();
        ///            needlePointer.Value = 80;
        ///            needlePointer.PointerType = PointerType.NeedlePointer;
        ///            needlePointer.EnableAnimation = true;
        ///            needlePointer.NeedleLengthFactor = 0.5;
        ///            needlePointer.NeedlePointerStroke = new SolidColorBrush(Colors.White);
        ///            needlePointer.NeedlePointerStrokeThickness = 2;
        ///            needlePointer.PointerCapStroke = new SolidColorBrush(Colors.White);
        ///            needlePointer.PointerCapDiameter = 20;
        ///            gauge.MainScale.Pointers.Add(needlePointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public CircularPointerCollection Pointers
        {
            get { return (CircularPointerCollection)GetValue(PointersProperty); }
            set { SetValue(PointersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Pointers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointersProperty =
            DependencyProperty.Register("Pointers", typeof(CircularPointerCollection), typeof(CircularScale), new PropertyMetadata(null, OnPointersChanged));

        private static void OnPointersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularScale)
            {
                CircularScale circularScale = obj as CircularScale;
                if(circularScale.Istemplateapplied)
                circularScale.SetPointerBindings();
            }
        }


        /// <summary>
        /// Gets or sets SweepDirection that decides the rendering direction of the elements in CircularScale.
        /// </summary>
        /// <remarks>
        /// SweepDirection property decides in which direction labels and ticks  have to be
        /// rendered in the CircularScale.
        /// </remarks>
        /// <value>
        /// SweepDirection
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.StartAngle = 180;
        ///             scale.SweepAngle = 180;
        ///             scale.SweepDirection = SweepDirection.Clockwise;
        ///             scale.StartValue = 0;
        ///             scale.EndValue = 100;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public SweepDirection SweepDirection
        {
            get { return (SweepDirection)GetValue(SweepDirectionProperty); }
            set { SetValue(SweepDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SweepDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(CircularScale), new PropertyMetadata(SweepDirection.Clockwise, OnSweepDirectionChanged));

        private static void OnSweepDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularScale)
            {
                CircularScale circularScale = obj as CircularScale;
                if (circularScale.Istemplateapplied)
                {
                    circularScale.SetRimBindings();
                    circularScale.SetPointerBindings();
                    circularScale.SetRangeBindings();
                    circularScale.CreateLabels();
                    circularScale.CreateTicks();
                    circularScale.CalculateMargins();
                }
                if (circularScale.cir_panel != null && circularScale.Tick_panel != null)
                {
                    circularScale.cir_panel.InvalidateMeasure();
                    circularScale.Tick_panel.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the SweepAngle of the CircularScale that decides the shape of the SfCircularGauge
        /// </summary>
        /// <remarks>
        /// By setting the StartAngle, SweepDirection and SweepAngle we can shape the Circular Gauge into 
        /// Full Circular Gauge, Half Circular Gauge, and Quarter Circular Gauge.
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.StartAngle = 180;
        ///             scale.SweepAngle = 180;
        ///             scale.SweepDirection = SweepDirection.Clockwise;
        ///             scale.StartValue = 0;
        ///             scale.EndValue = 100;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double SweepAngle
        {
            get { return (double)GetValue(SweepAngleProperty); }
            set { SetValue(SweepAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SweepAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SweepAngleProperty =
            DependencyProperty.Register("SweepAngle", typeof(double), typeof(CircularScale), new PropertyMetadata(270d, OnSweepAngleChanged));

        private static void OnSweepAngleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularScale)
            {
                CircularScale circularScale = obj as CircularScale;
                //circularScale.EndAngle = circularScale.StartAngle + circularScale.SweepAngle;
                if (circularScale.Istemplateapplied)
                {
                    circularScale.SweepAngle = circularScale.SweepAngle < 0 ? (circularScale.SweepAngle * (-1)) : circularScale.SweepAngle;
                    circularScale.SweepAngle = circularScale.SweepAngle > 360 ? circularScale.SweepAngle % 360 : circularScale.SweepAngle;
                    circularScale.SetRimBindings();
                    circularScale.SetPointerBindings();
                    circularScale.SetRangeBindings();
                    circularScale.CreateLabels();
                    circularScale.CreateTicks();
                    circularScale.CalculateMargins();
                }
                if (circularScale.cir_panel != null && circularScale.Tick_panel != null)
                {
                    circularScale.cir_panel.InvalidateMeasure();
                    circularScale.Tick_panel.InvalidateMeasure();
                }
            }
        }


        /// <summary>
        /// Gets or sets the StartAngle of the CircularScale that decides the shape of the SfCircularGauge.
        /// </summary>
        /// <remarks>
        /// By setting the StartAngle, SweepDirection and SweepAngle we can shape the Circular Gauge into 
        /// Full Circular Gauge, Half Circular Gauge, and Quarter Circular Gauge.
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.StartAngle = 180;
        ///             scale.SweepAngle = 180;
        ///             scale.SweepDirection = SweepDirection.Clockwise;
        ///             scale.StartValue = 0;
        ///             scale.EndValue = 100;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(CircularScale), new PropertyMetadata(135d, OnStartAngleChanged));

        

        private static void OnStartAngleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularScale)
            {
                CircularScale circularScale = obj as CircularScale;
                //circularScale.EndAngle = circularScale.StartAngle + circularScale.SweepAngle;
                if (circularScale.Istemplateapplied)
                {
                    circularScale.SetRimBindings();
                    circularScale.SetPointerBindings();
                    circularScale.SetRangeBindings();
                    circularScale.CreateLabels();
                    circularScale.CreateTicks();
                    circularScale.CalculateMargins();
                }
                if (circularScale.cir_panel != null && circularScale.Tick_panel != null)
                {
                    circularScale.cir_panel.InvalidateMeasure();
                    circularScale.Tick_panel.InvalidateMeasure();
                }
            }
        }

        


        internal Size SemiSize
        {
            get { return (Size)GetValue(SemiSizeProperty); }
            set { SetValue(SemiSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SemiSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SemiSizeProperty =
            DependencyProperty.Register("SemiSize", typeof(Size), typeof(CircularScale), new PropertyMetadata(new Size(0,0)));

        


        //public double EndAngle
        //{
        //    get { return (double)GetValue(EndAngleProperty); }
        //    set { SetValue(EndAngleProperty, value); }
        //}



        //// Using a DependencyProperty as the backing store for EndAngle.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty EndAngleProperty =
        //    DependencyProperty.Register("EndAngle", typeof(double), typeof(CircularScale), new PropertyMetadata(300d, OnEndAngleChanged));

        
        //private static void OnEndAngleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    if (obj is CircularScale)
        //    {
        //        CircularScale circularScale = obj as CircularScale;
        //        circularScale.IsLargeArc = Math.Abs(circularScale.StartAngle - circularScale.EndAngle) > 180d;
        //        circularScale.SetRangeBindings();
        //        circularScale.SetPointerBindings();
        //        circularScale.SetArcProperties();
        //    }
        //}


        /// <summary>
        /// Gets EndPoint of the CircularScale.
        /// </summary>
        /// <remarks>
        /// EndPoint is a read only property used to get the value of the EndPoint.
        /// </remarks>
        /// <value>
        /// Point
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            internal set { SetValue(EndPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(CircularScale), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets StartPoint of the CircularScale.
        /// </summary>
        /// <remarks>
        /// StartPoint is a read only property used to get the value of the StartPoint.
        /// </remarks>
        /// <value>
        /// Point
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            internal set { SetValue(StartPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(CircularScale), new PropertyMetadata(new Point()));



        internal bool IsLargeArc
        {
            get { return (bool)GetValue(IsLargeArcProperty); }
            set { SetValue(IsLargeArcProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLargeArc.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(CircularScale), new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets a value indicating whether the Range Stroke applied to Labels
        /// Stroke
        /// </summary>
        /// <remarks>
        /// <para>The Scale should have atleast one range to make the property
        /// effective.</para>
        /// <para>Value as<b>True: </b>Range Stroke is applied to the labels which are in
        /// the appropriate range.</para>
        /// <para>Value as<b>False: </b>Default or user defined Stroke is applied to the
        /// labels.</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.BindRangeStrokeToLabels = true;
        ///             scale.BindRangeStrokeToTicks = true;
        ///             CircularRange range1 = new CircularRange();
        ///             range1.StartValue = 0;
        ///             range1.EndValue = 40;
        ///             range1.Stroke = new SolidColorBrush(Colors.Green);
        ///             range1.StrokeThickness = 10;
        ///             CircularRange range2 = new CircularRange();
        ///             range2.StartValue = 40;
        ///             range2.EndValue = 60;
        ///             range2.Stroke = new SolidColorBrush(Colors.Yellow);
        ///             range2.StrokeThickness = 10;
        ///             CircularRange range3 = new CircularRange();
        ///             range3.StartValue = 60;
        ///             range3.EndValue = 100;
        ///             range3.Stroke = new SolidColorBrush(Colors.Red);
        ///             range3.StrokeThickness = 10;
        ///             scale.Ranges.Add(range1);
        ///             scale.Ranges.Add(range2);
        ///             scale.Ranges.Add(range3);
        ///             gauge.MainScale = scale;
        /// 
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public bool BindRangeStrokeToLabels
        {
            get { return (bool)GetValue(BindRangeStrokeToLabelsProperty); }
            set { SetValue(BindRangeStrokeToLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindRangeStrokeToLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindRangeStrokeToLabelsProperty =
            DependencyProperty.Register("BindRangeStrokeToLabels", typeof(bool), typeof(CircularScale), new PropertyMetadata(false, new PropertyChangedCallback(OnBindRangeStrokeToLabelsChanged)));

        private static void OnBindRangeStrokeToLabelsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is CircularScale)
            {
                CircularScale circularScale = obj as CircularScale;
                if (circularScale != null && circularScale.Istemplateapplied)
                    circularScale.CreateLabels();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Range Stroke applied to Ticks
        /// Stroke
        /// </summary>
        /// <remarks>
        /// <para>The Scale should have atleast one range to make the property
        /// effective.</para>
        /// <para>Value as<b>True: </b>Range Stroke is applied to the Ticks which are in
        /// the appropriate range.</para>
        /// <para>Value as<b>False: </b>Default or user defined Stroke is applied to the
        /// Ticks.</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.BindRangeStrokeToLabels = true;
        ///             scale.BindRangeStrokeToTicks = true;
        ///             CircularRange range1 = new CircularRange();
        ///             range1.StartValue = 0;
        ///             range1.EndValue = 40;
        ///             range1.Stroke = new SolidColorBrush(Colors.Green);
        ///             range1.StrokeThickness = 10;
        ///             CircularRange range2 = new CircularRange();
        ///             range2.StartValue = 40;
        ///             range2.EndValue = 60;
        ///             range2.Stroke = new SolidColorBrush(Colors.Yellow);
        ///             range2.StrokeThickness = 10;
        ///             CircularRange range3 = new CircularRange();
        ///             range3.StartValue = 60;
        ///             range3.EndValue = 100;
        ///             range3.Stroke = new SolidColorBrush(Colors.Red);
        ///             range3.StrokeThickness = 10;
        ///             scale.Ranges.Add(range1);
        ///             scale.Ranges.Add(range2);
        ///             scale.Ranges.Add(range3);
        ///             gauge.MainScale = scale;
        /// 
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public bool BindRangeStrokeToTicks
        {
            get { return (bool)GetValue(BindRangeStrokeToTicksProperty); }
            set { SetValue(BindRangeStrokeToTicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindRangeStrokeToTicks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindRangeStrokeToTicksProperty =
            DependencyProperty.Register("BindRangeStrokeToTicks", typeof(bool), typeof(CircularScale), new PropertyMetadata(false, new PropertyChangedCallback(OnBindRangeStrokeToTicksChanged)));

        private static void OnBindRangeStrokeToTicksChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is CircularScale)
            {
                CircularScale circularScale = obj as CircularScale;
                if (circularScale != null && circularScale.Istemplateapplied)
                    circularScale.CreateTicks();
            }
        }

        /// <summary>
        /// Gets or sets value that decides the length of the ticks in the CircularScale.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularScale scale = new CircularScale();
        ///            scale.TickLength = 10;
        ///            gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double TickLength
        {
            get { return (double)GetValue(TickLengthProperty); }
            set { SetValue(TickLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register("TickLength", typeof(double), typeof(CircularScale), new PropertyMetadata(10d, OnTicksChanged));



        /// <summary>
        /// Gets or sets value that decides the length of the small ticks in the CircularScale.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularScale scale = new CircularScale();
        ///            scale.SmallTickLength = 10;
        ///            gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double SmallTickLength
        {
            get { return (double)GetValue(SmallTickLengthProperty); }
            set { SetValue(SmallTickLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallTickLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallTickLengthProperty =
            DependencyProperty.Register("SmallTickLength", typeof(double), typeof(CircularScale), new PropertyMetadata(5d, OnTicksChanged));



        /// <summary>
        /// Gets or sets the brush that describes the TickStroke of the CircularScale.
        /// </summary>
        /// <remarks>
        /// TickStroke property will remains effective until the value of the
        /// BindRangeStrokeToTicks property is false.
        /// </remarks>
        /// <value>
        /// Brush
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.TickStroke = new SolidColorBrush(Colors.Red);
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush TickStroke
        {
            get { return (Brush)GetValue(TickStrokeProperty); }
            set { SetValue(TickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickStrokeProperty =
            DependencyProperty.Register("TickStroke", typeof(Brush), typeof(CircularScale), new PropertyMetadata(new SolidColorBrush(Colors.White), OnTicksChanged));

        private static void OnTicksChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularScale)
            {
                CircularScale scale = obj as CircularScale;
                if(scale.Istemplateapplied)
                scale.CreateTicks();
            }
        }

        /// <summary>
        /// Gets or sets the brush that describes the Stroke value of the Labels.
        /// </summary>
        /// <remarks>
        /// LabelStroke property will remains effective until the value of the
        /// BindRangeStrokeToLabels property is false.
        /// </remarks>
        /// <value>
        /// Brush
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.LabelStroke = new SolidColorBrush(Colors.Red);
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush LabelStroke
        {
            get { return (Brush)GetValue(LabelStrokeProperty); }
            set { SetValue(LabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelStrokeProperty =
            DependencyProperty.Register("LabelStroke", typeof(Brush), typeof(CircularScale), new PropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnLabelStrokeChanged)));

        private static void OnLabelStrokeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var circularScale = obj as CircularScale;
            if (circularScale != null && circularScale.Istemplateapplied)
            {
                circularScale.CreateLabels();
            }
        }

        /// <summary>
        /// Gets or sets the Boolean property that allow user to decided whether the gauge label can resize automatically.
        /// </summary>
        /// <remarks>
        /// LabelAutoSizeChange property will remains effective until the user set it to false.
        /// </remarks>
        /// <value>
        /// bool
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.LabelAutoSizeChange = true;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        public bool LabelAutoSizeChange
        {
            get { return (bool)GetValue(LabelAutoSizeChangeProperty); }
            set { SetValue(LabelAutoSizeChangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelAutoSizeChange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelAutoSizeChangeProperty =
            DependencyProperty.Register("LabelAutoSizeChange", typeof(bool), typeof(CircularScale), new PropertyMetadata(false));

        

        internal double LabelFontSize
        {
            get { return (double)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(CircularScale), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the brush that describes the SmallTickStroke of the CircularScale.
        /// </summary>
        /// <remarks>
        /// SmallTickStroke property will remains effective until the value of the
        /// BindRangeStrokeToTicks property is false.
        /// </remarks>
        /// <value>
        /// Brush
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.SmallTickStroke = new SolidColorBrush(Colors.Red);
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush SmallTickStroke
        {
            get { return (Brush)GetValue(SmallTickStrokeProperty); }
            set { SetValue(SmallTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallTickStrokeProperty =
            DependencyProperty.Register("SmallTickStroke", typeof(Brush), typeof(CircularScale), new PropertyMetadata(new SolidColorBrush(Colors.White), OnTicksChanged));


        /// <summary>
        /// Gets or sets the thickness of the TickStroke.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularScale scale = new CircularScale();
        ///            scale.TickStrokeThickness =3;
        ///            gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double TickStrokeThickness
        {
            get { return (double)GetValue(TickStrokeThicknessProperty); }
            set { SetValue(TickStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickStrokeThicknessProperty =
            DependencyProperty.Register("TickStrokeThickness", typeof(double), typeof(CircularScale), new PropertyMetadata(1d, OnTicksChanged));


        /// <summary>
        /// Gets or sets the thickness of the  SmallTickStroke.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularScale scale = new CircularScale();
        ///            scale.SmallTickStrokeThickness =3;
        ///            gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double SmallTickStrokeThickness
        {
            get { return (double)GetValue(SmallTickStrokeThicknessProperty); }
            set { SetValue(SmallTickStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallTickStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallTickStrokeThicknessProperty =
            DependencyProperty.Register("SmallTickStrokeThickness", typeof(double), typeof(CircularScale), new PropertyMetadata(1d, OnTicksChanged));


        /// <summary>
        /// Gets or sets the StartValue of the CircularScale that decides the overall range of the scale.
        /// </summary>
        /// <remarks>
        /// StartValue and EndValue properties will decides the overall range of the
        /// Circular Rim
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.StartAngle = 180;
        ///             scale.SweepAngle = 180;
        ///             scale.SweepDirection = SweepDirection.Clockwise;
        ///             scale.StartValue = 0;
        ///             scale.EndValue = 100;
        ///             scale.RimStroke = new SolidColorBrush(Colors.Black);
        ///             scale.RimStrokeThickness =3;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double StartValue
        {
            get { return (double)GetValue(StartValueProperty); }
            set { SetValue(StartValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(CircularScale), new PropertyMetadata(0d, OnRangeValueChanged));

        private static void OnRangeValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CircularScale circularScale = obj as CircularScale;
            circularScale.StartValue = double.IsNaN(circularScale.StartValue) ? 0 : circularScale.StartValue;
            circularScale.EndValue = double.IsNaN(circularScale.EndValue) ? 0 : circularScale.EndValue;
            if (circularScale != null && circularScale.Istemplateapplied)
            {
                circularScale.SetRimBindings();
                circularScale.SetPointerBindings();
                circularScale.SetRangeBindings(); 
                circularScale.CreateTicks();
                circularScale.CreateLabels();
                circularScale.CalculateMargins();
            }
        }

        private static void OnIntervalValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CircularScale circularScale = obj as CircularScale;
            circularScale.StartValue = double.IsNaN(circularScale.StartValue) ? 0 : circularScale.StartValue;
            circularScale.EndValue = double.IsNaN(circularScale.EndValue) ? 0 : circularScale.EndValue;
            if (circularScale != null )
            {
                circularScale.SetRimBindings();
                circularScale.SetPointerBindings();
                circularScale.SetRangeBindings();
                circularScale.CreateTicks();
                circularScale.CreateLabels();
                circularScale.CalculateMargins();
            }
        }

        /// <summary>
        /// Gets or sets the LabelPostfix value that can attached at the end of the labels.
        /// </summary>
        /// <remarks>
        /// Used to add some extentions to the labels like % symbol can be added to labels
        /// while displaying the percentage values in SfCircularGauge.
        /// </remarks>
        /// <value>
        /// string
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.LabelPostfix="%";
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public string LabelPostfix
        {
            get { return (string)GetValue(LabelPostfixProperty); }
            set { SetValue(LabelPostfixProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Postfix.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelPostfixProperty =
            DependencyProperty.Register("LabelPostfix", typeof(string), typeof(CircularScale), new PropertyMetadata(null, OnLabelStrokeChanged));

        /// <summary>
        /// Gets or sets the LabelPrefix value that can be added before the labels.
        /// </summary>
        /// <remarks>
        /// Used to add some prefix to the labels like $ symbol can be added to labels
        /// while displaying the dollar values in SfCircularGauge.
        /// </remarks>
        /// <value>
        /// string
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.LabelPrefix="$";
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public string LabelPrefix
        {
            get { return (string)GetValue(LabelPrefixProperty); }
            set { SetValue(LabelPrefixProperty, value); }
        }

		// Using a DependencyProperty as the backing store for Postfix.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelPrefixProperty =
            DependencyProperty.Register("LabelPrefix", typeof(string), typeof(CircularScale), new PropertyMetadata(null, OnLabelStrokeChanged));


        /// <summary>
        /// Gets or sets the EndValue of the CircularScale that decides the overall range of the scale.
        /// </summary>
        /// <remarks>
        /// StartValue and EndValue properties will decides the overall range of the
        /// Circular Rim
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.StartAngle = 180;
        ///             scale.SweepAngle = 180;
        ///             scale.SweepDirection = SweepDirection.Clockwise;
        ///             scale.StartValue = 0;
        ///             scale.EndValue = 100;
        ///             scale.RimStroke = new SolidColorBrush(Colors.Black);
        ///             scale.RimStrokeThickness =3;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double EndValue
        {
            get { return (double)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(CircularScale), new PropertyMetadata(100d, OnRangeValueChanged));




        /// <summary>
        /// Gets or sets the Interval value of the CircularScale that used to calculate the Tick count.
        /// </summary>
        /// <remarks>
        /// Interval property is used to calculate the Tick count for a scale based on
        /// StartValue and EndValue.
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.Interval = 20;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
               set { SetValue(IntervalProperty, Math.Round( value,5)); }
        }

        // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(CircularScale), new PropertyMetadata(double.NaN, OnRangeValueChanged));

        private double ValueDiff
        {
            get { return (double)GetValue(ValueDiffProperty); }
            set { SetValue(ValueDiffProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueDiff.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueDiffProperty =
            DependencyProperty.Register("ValueDiff", typeof(double), typeof(CircularScale), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the value that used to calculate the MinorTicks count in the scale.
        /// </summary>
        /// <remarks>
        /// MinorTicksPerInterval property is used to calculate the MinorTicks count for 
        /// a scale based on StartValue and EndValue.
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale scale = new CircularScale();
        ///             scale.MinorTicksPerInterval = 2;
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public int MinorTicksPerInterval
        {
            get { return (int)GetValue(MinorTicksPerIntervalProperty); }
            set { SetValue(MinorTicksPerIntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTicksPerInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTicksPerIntervalProperty =
            DependencyProperty.Register("MinorTicksPerInterval", typeof(int), typeof(CircularScale), new PropertyMetadata(1, OnRangeValueChanged));



        //public double Value
        //{
        //    get { return (double)GetValue(ValueProperty); }
        //    set { SetValue(ValueProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ValueProperty =
        //    DependencyProperty.Register("Value", typeof(double), typeof(CircularScale), new PropertyMetadata(0d));

        //public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    if (obj is CircularScale)
        //    {
        //        CircularScale circularScale = obj as CircularScale;
        //        circularScale.ValueAngle = circularScale.ValueToAngle(circularScale.Value);
        //    }
        //}



        //public double ValueAngle
        //{
        //    get { return (double)GetValue(ValueAngleProperty); }
        //    set { SetValue(ValueAngleProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ValueAngle.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ValueAngleProperty =
        //    DependencyProperty.Register("ValueAngle", typeof(double), typeof(CircularScale), new PropertyMetadata(0d));

        
        internal double CircularLabelsAngularSpace
        {
            get { return (double)GetValue(CircularLabelsAngularSpaceProperty); }
            set { SetValue(CircularLabelsAngularSpaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircularLabelsSweepAngle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CircularLabelsAngularSpaceProperty =
            DependencyProperty.Register("CircularLabelsAngularSpace", typeof(double), typeof(CircularScale), new PropertyMetadata(0d));


        internal double CircularTicksAngularSpace
        {
            get { return (double)GetValue(CircularTicksAngularSpaceProperty); }
            set { SetValue(CircularTicksAngularSpaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircularLabelsSweepAngle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CircularTicksAngularSpaceProperty =
            DependencyProperty.Register("CircularTicksAngularSpace", typeof(double), typeof(CircularScale), new PropertyMetadata(0d));


        private static void OnRangePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            CircularScale scale = (CircularScale)d;
            if(scale.Istemplateapplied)
            scale.CalculateMargins();
        }

        private Size GaugeSize;
        internal bool Istemplateapplied;
        internal CircularPanel cir_panel = null;
        internal TickLinesPanel Tick_panel = null;
        #endregion

#if WINRT
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Istemplateapplied = true;
            cir_panel = GetTemplateChild("LabelsPanel") as CircularPanel;
            Tick_panel = GetTemplateChild("Tickpanel") as TickLinesPanel;
        }       
#else
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Istemplateapplied = true;
        }
#endif


        public void Dispose()
        {
            this.Ranges.CollectionChanged -= Ranges_CollectionChanged;
            Pointers.CollectionChanged -= Pointers_CollectionChanged;
            this.Labels.Clear();
            this.Ticks.Clear();
        }

    }
}
