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
#if WINRT
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Gauges
{
    public enum RangePosition
    {

        Inside,


        SetAsGaugeRim,


        Outside
    }

    public enum RangePointerPosition
    {

        Inside,

        Outside,
    }

    internal enum TickType
    {
        Major,

        Minor,
    }
    public enum LabelPosition
    {

        Inside,

        Outside,
    }

    public enum TickPosition
    {
        Inside,

        Outside,

        Cross
    }

    public enum PointerType
    {
        NeedlePointer,

        RangePointer,

        SymbolPointer

    }

    public enum NumericScaleType
    {
        Auto,

        Thousands,

        Millions,

        Billions,

        Trillions,

        Quadrillions,

        Quintillions

    }

    public enum Symbol
    {
        Rectangle,

        RoundedRectangle,

        Ellipse,

        Triangle,

        InvertedTriangle,

        Arrow,

        InvertedArrow,

        Hexagon,

        Pentagon,

        Cross,

        Diamond,

        Custom

    }

}
