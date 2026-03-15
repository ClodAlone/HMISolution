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
using System.Windows;

#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media; 
#else
using System.Windows.Media; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface IPort : IDiagramElement
    {
        object Shape { get; set; }
        Style ShapeStyle { get; set; }
        PortConstraints Constraints { get; set; }
    }

    public interface INodePort : IPort
    {
        double NodeOffsetX { get; set; }
        double NodeOffsetY { get; set; }
        object Node { get; set; }
        UnitMode UnitMode { get; set; }
        OrthogonalDirection GetDirection();
    }

    public interface INodePortInfo
    {
        double OffsetX { get; set; }
        double OffsetY { get; set; }
    }

    internal interface IInternalNodePort : 
        INodePort,
        INodePortInfo,
        IInternalDiagramElement,
        IWrapper
    {
        NodePort View { get; }
        IInternalNode KnownNode { get; set; }
        Point UpdatePosition();
        event Action PositionChanged;
    }

    public enum UnitMode
    {
        Fraction,
        Absolute
    }
}


