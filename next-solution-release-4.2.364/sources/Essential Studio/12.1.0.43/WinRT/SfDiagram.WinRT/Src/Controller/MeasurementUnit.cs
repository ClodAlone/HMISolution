#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#endif
using System.Reflection;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Panels;

namespace Syncfusion.UI.Xaml.Diagram
{
    public abstract class MeasurementUnit
    {
        //private UnitToUnit<TUnit> _unitToUnit;

        public abstract event UnitToUnitEventHandler<double> UnitChangedEvent;

        public abstract double ToPixel(double unit);
        //public abstract Double ToPixel(double unit);
        internal Point ToPixel(Point unit)
        {
            return new Point(ToPixel(unit.X), ToPixel(unit.Y));
        }
        internal Size ToPixel(Size unit)
        {
            return new Size(ToPixel(unit.Width), ToPixel(unit.Height));
        }
        internal Rect ToPixel(Rect unit)
        {
            return new Rect(ToPixel(new Point(unit.X, unit.Y)), ToPixel(new Size(unit.Width, unit.Height)));
        }
        internal Thickness ToPixel(Thickness unit)
        {
            return new Thickness(ToPixel(unit.Left), ToPixel(unit.Top), ToPixel(unit.Right), ToPixel(unit.Bottom));
        }
        public abstract double ToUnit(double pixel);
        //public abstract TUnit ToUnit(Point pixel);
        //public abstract TUnit ToUnit(Size pixel);
        //public abstract Double ToUnit(double pixel);

        internal Point ToUnit(Point unit)
        {
            return new Point(ToUnit(unit.X), ToUnit(unit.Y));
        }
        internal Size ToUnit(Size unit)
        {
            return new Size(ToUnit(unit.Width), ToUnit(unit.Height));
        }
        internal Rect ToUnit(Rect unit)
        {
            return new Rect(ToUnit(new Point(unit.X, unit.Y)), ToUnit(new Size(unit.Width, unit.Height)));
        }
        internal Thickness ToUnit(Thickness unit)
        {
            return new Thickness(ToUnit(unit.Left), ToUnit(unit.Top), ToUnit(unit.Right), ToUnit(unit.Bottom));
        }

        //public UnitToUnit<TUnit> UnitToUnit
        //{
        //    get { return _unitToUnit; }
        //    set
        //    {
        //        _unitToUnit = value;
        //        UnitToUnitConverted.Invoke(value);
        //    }
        //}

        //internal abstract event Action<UnitToUnit<TUnit>> UnitToUnitConverted<TUnit>;

    }

    public abstract class MeasurementUnit<TUnit>
    {
        //private UnitToUnit<TUnit> _unitToUnit;

        public abstract event UnitToUnitEventArgs<TUnit> UnitChangedEvent;

        public abstract double ToPixel(TUnit unit);

        public abstract TUnit ToUnit(double pixel);
    }

    public delegate void UnitToUnitEventHandler<TUnit>(object sender, UnitToUnitEventArgs<TUnit> current);
    public delegate TUnit UnitToUnitEventArgs<TUnit>(TUnit current);

    public class LengthUnit : MeasurementUnit
    {
        public LengthUnit()
        {
            Unit = LengthUnits.Pixels;
        }

        private LengthUnits _unit;
        private LengthUnits _source;
        private LengthUnits _target;
        public LengthUnits Unit
        {
            get { return _unit; }
            set
            {
                if (_unit != value)
                {
                    _source = _unit;
                    _unit = value;
                    _target = _unit;
                    //UnitToUnit = new UnitToUnit<double>(UnitToUnitConverter);
                    if (UnitChangedEvent != null)
                    {
                        UnitChangedEvent.Invoke(this, UnitToUnitConverter);
                    }
                }
            }
        }

        public const double DPI = 96d;
        public const double Inch = DPI;
        public const double Foot = Inch * 12;
        public const double Yard = Foot * 3;
        public const double Miles = Yard * 1760;
        public const double Millimeters = Inch / 25.4;
        public const double Centimeters = Millimeters * 10;
        public const double Meters = Centimeters * 100;
        public const double Kilometers = Meters * 100;

        public override double ToPixel(double unit)
        {
            return ToPixel(unit, Unit);
        }

        private static double ToPixel(double unit, LengthUnits type)
        {
            switch (type)
            {
                case LengthUnits.Inches:
                    return unit * Inch;
                case LengthUnits.Feets:
                    return unit * Foot;
                case LengthUnits.Yards:
                    return unit * Yard;
                case LengthUnits.Miles:
                    return unit * Miles;
                case LengthUnits.Millimeters:
                    return unit * Millimeters;
                case LengthUnits.Centimeters:
                    return unit * Centimeters;
                case LengthUnits.Meters:
                    return unit * Meters;
                case LengthUnits.Kilometers:
                    return unit * Kilometers;
                default:
                    return unit;
            }
        }

        public override double ToUnit(double pixel)
        {
            return ToUnit(pixel, Unit);
        }

        private static double ToUnit(double pixel, LengthUnits type)
        {
            switch (type)
            {
                case LengthUnits.Inches:
                    return pixel / Inch;
                case LengthUnits.Feets:
                    return pixel / Foot;
                case LengthUnits.Yards:
                    return pixel / Yard;
                case LengthUnits.Miles:
                    return pixel / Miles;
                case LengthUnits.Millimeters:
                    return pixel / Millimeters;
                case LengthUnits.Centimeters:
                    return pixel / Centimeters;
                case LengthUnits.Meters:
                    return pixel / Meters;
                case LengthUnits.Kilometers:
                    return pixel / Kilometers;
                default:
                    return pixel;
            }
        }

        private double UnitToUnitConverter(double current)
        {
            return ToUnit(ToPixel(current, _source), _target);
        }

        public double Convert(double value, LengthUnits from, LengthUnits to)
        {
            return ToUnit(ToPixel(value, from), to);
        }

        public override event UnitToUnitEventHandler<double> UnitChangedEvent;
    }

    public enum LengthUnits
    {
        Pixels,
        Inches,
        Feets,
        Yards,
        Miles,
        Millimeters,
        Centimeters,
        Meters,
        Kilometers
    }
}
