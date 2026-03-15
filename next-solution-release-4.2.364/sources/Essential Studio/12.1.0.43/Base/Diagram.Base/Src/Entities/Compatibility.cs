#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Text;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Connection port.
    /// </summary>
    [Obsolete("Port class is obsolete.Use ConnectionPoint class instead")]
    public class Port : ConnectionPoint
    {
    }

    /// <summary>
    /// End point decorator.
    /// </summary>
    [Obsolete("EndPointDecorator class is obsolete.Use HeadDecorator or TailDecorator class instead")]
    public class EndPointDecorator
    {
    }

    /// <summary>
    /// Link to connect the nodes.
    /// </summary>
    [Obsolete("Link class is obsolete.Use LineConnector instead")]
    public class Link : LineConnector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        public Link(PointF ptStart, PointF ptEnd)
            : base(ptStart, ptEnd)
        {
        }
    }

    /// <summary>
    /// Line connector to connect the node orthogonally.
    /// </summary>
    [Obsolete("OrthogonalLine class is obsolete.Use OrthogonalConnector class instead")]
    public class OrthogonalLine : OrthogonalConnector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalLine"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="measureUnits">Real world units of measurement.</param>
        public OrthogonalLine(PointF ptStart, PointF ptEnd, MeasureUnits measureUnits)
            : base(ptStart, ptEnd, measureUnits)
        {
        }
    }

    /// <summary>
    /// Node that is rendered in poly line.
    /// </summary>
    [Obsolete("Polyline class is obsolete.Use PolylineNode class instead")]
    public class Polyline : PolylineNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Polyline"/> class.
        /// </summary>
        /// <param name="pts">The points collection.</param>
        public Polyline(PointF[] pts)
            : base(pts)
        {
        }
    }

    /// <summary>
    /// Arc node.
    /// </summary>
    [Obsolete("Arc class is obsolete.Use SplineNode class instead")]
    public class Arc : SplineNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Arc"/> class.
        /// </summary>
        /// <param name="pts">The points collection.</param>
        public Arc(PointF[] pts)
            : base(pts)
        {
        }
    }

    /// <summary>
    /// Shape class
    /// </summary>
    [Obsolete("Shape class is obsolete.Use Node class instead")]
    public class Shape : Node
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    /// <summary>
    /// Filled shape.
    /// </summary>
    [Obsolete("FilledShape class is obsolete.Use FilledPath class instead")]
    public class FilledShape : FilledPath
    {
    }

    /// <summary>
    /// Symbol class.
    /// </summary>
    [Obsolete("Symbol class is obsolete.Use Node class instead")]
    public class Symbol : Node
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
