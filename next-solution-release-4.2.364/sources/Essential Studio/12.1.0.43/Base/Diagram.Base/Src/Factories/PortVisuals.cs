#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Contains PortVisual predefined types 
    /// </summary>
    public enum PortVisualType
    {
        /// <summary>
        /// Port will be drawn in circle format.
        /// </summary>
        CirclePort,

        /// <summary>
        /// Port will be drawn in X format.
        /// </summary>
        XPort,

        /// <summary>
        /// Port will be drawn in triangle format.
        /// </summary>
        Triangleport,

        /// <summary>
        /// Port will be drawn in square format.
        /// </summary>
        SquarePort,

        /// <summary>
        /// Port will be drawn in rhombus format.
        /// </summary>
        RhombPort,

        /// <summary>
        /// Port will be drawn in custom format.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Contains predefined sizes 
    /// </summary>
    public enum ConnectionPointSize
    {
        /// <summary>
        /// Connection point size will be in 12 * 12 size.
        /// </summary>
        Large, // 12 * 12

        /// <summary>
        /// Connection point size will be in 9 * 9 size.
        /// </summary>
        Medium, // 9 * 9

        /// <summary>
        /// Connection point size will be in 6 * 6 size.
        /// </summary>
        Small, // 6 * 6

        /// <summary>
        /// Connection point size will be custom size.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Contains static GraphicsPath objects that can be used for the
    /// visual representation of ports.
    /// </summary>
    public class PortVisuals
    {
        #region properties
        /// <summary>
        /// Gets the circle with a crosshair in the middle style port.
        /// </summary>
        /// <value>The circle port.</value>
        public static GraphicsPath CirclePort
        {
            get
            {
                if (PortVisuals.circlePort == null)
                {
                    PortVisuals.circlePort = new GraphicsPath();

                    float radius = 3.0f;
                    float diameter = radius * 2.0f;

                    RectangleF rcBounds = new RectangleF(-radius, -radius, diameter, diameter);

                    PortVisuals.circlePort.AddEllipse(rcBounds);
                    PortVisuals.circlePort.CloseFigure();

                    PortVisuals.circlePort.StartFigure();
                    PointF ptLeft = new PointF(-radius, 0);
                    PointF ptRight = new PointF(radius, 0);
                    PortVisuals.circlePort.AddLine(ptLeft, ptRight);
                    PortVisuals.circlePort.CloseFigure();

                    PortVisuals.circlePort.StartFigure();
                    PointF ptTop = new PointF(0, -radius);
                    PointF ptBottom = new PointF(0, radius);
                    PortVisuals.circlePort.AddLine(ptTop, ptBottom);
                    PortVisuals.circlePort.CloseFigure();
                }

                return PortVisuals.circlePort;
            }
        }

        /// <summary>
        /// Gets the X marks the spot style port.
        /// </summary>
        /// <value>The X port.</value>
        public static GraphicsPath XPort
        {
            get
            {
                if (PortVisuals.xPort == null)
                {
                    PortVisuals.xPort = new GraphicsPath();

                    float radius = 3.0f;
                    float diameter = radius * 2.0f;

                    RectangleF rcBounds = new RectangleF(-radius, -radius, diameter, diameter);

                    PortVisuals.xPort.StartFigure();
                    PointF ptUpperLeft = new PointF(rcBounds.X, rcBounds.Y);
                    PointF ptLowerRight = new PointF(rcBounds.X + rcBounds.Width, rcBounds.Y + rcBounds.Height);
                    PortVisuals.xPort.AddLine(ptUpperLeft, ptLowerRight);
                    PortVisuals.xPort.CloseFigure();

                    PortVisuals.xPort.StartFigure();
                    PointF ptUpperRight = new PointF(rcBounds.X + rcBounds.Width, rcBounds.Y);
                    PointF ptBottomLeft = new PointF(rcBounds.X, rcBounds.Y + rcBounds.Height);
                    PortVisuals.xPort.AddLine(ptUpperRight, ptBottomLeft);
                    PortVisuals.xPort.CloseFigure();
                }

                return PortVisuals.xPort;
            }
        }

        /// <summary>
        /// Gets the triangle form port.
        /// </summary>
        /// <value>The triangle port.</value>
        public static GraphicsPath TrianglePort
        {
            get
            {
                if (PortVisuals.portTriangle == null)
                {
                    PortVisuals.portTriangle = CreateTrianglePortShape();
                }

                return PortVisuals.portTriangle;
            }
        }

        /// <summary>
        /// Gets the square form port.
        /// </summary>
        /// <value>The square port.</value>
        public static GraphicsPath SquarePort
        {
            get
            {
                if (PortVisuals.portSquare == null)
                {
                    PortVisuals.portSquare = CreateSquarePortShape();
                }
                return PortVisuals.portSquare;
            }
        }

        /// <summary>
        /// Gets the rhomb form port.
        /// </summary>
        /// <value>The rhomb port.</value>
        public static GraphicsPath RhombPort
        {
            get
            {
                if (PortVisuals.portRhomb == null)
                {
                    PortVisuals.portRhomb = CreateRhombPortShape();
                }
                return PortVisuals.portRhomb;
            }
        }
        #endregion

        #region helper methods
        private static GraphicsPath CreateRhombPortShape()
        {
            GraphicsPath gfxPath = new GraphicsPath();

            float radius = 3.0f;
            PointF[] myArray = 
            {
                new PointF(0, -radius),
                new PointF(radius, 0),
                new PointF(0, radius),
                new PointF(-radius, 0)
            };

            gfxPath.StartFigure();
            gfxPath.AddPolygon(myArray);
            gfxPath.CloseFigure();

            return gfxPath;
        }
        private static GraphicsPath CreateSquarePortShape()
        {
            GraphicsPath gpSquareShapeToReturn = new GraphicsPath();

            float fRadius = 3.0f;

            gpSquareShapeToReturn.StartFigure();
            RectangleF rcBounds = new RectangleF(-fRadius, -fRadius, fRadius * 2, fRadius * 2);
            gpSquareShapeToReturn.AddRectangle(rcBounds);
            gpSquareShapeToReturn.CloseFigure();

            return gpSquareShapeToReturn;
        }

        private static GraphicsPath CreateTrianglePortShape()
        {
            GraphicsPath gpTriangleShapeToReturn = new GraphicsPath();

            float fRadius = 3.0f;

            PointF[] ptArray = { new PointF( 0, -fRadius ), new PointF( fRadius, fRadius ), new PointF( -fRadius, fRadius ) };

            gpTriangleShapeToReturn.StartFigure();
            gpTriangleShapeToReturn.AddPolygon(ptArray);
            gpTriangleShapeToReturn.CloseFigure();

            return gpTriangleShapeToReturn;
        }
        #endregion

        private static GraphicsPath portSquare = null;
        private static GraphicsPath portRhomb = null;
        private static GraphicsPath portTriangle = null;
        [ThreadStaticAttribute]
        private static GraphicsPath circlePort = null;
        [ThreadStaticAttribute]
        private static GraphicsPath xPort = null;

        /// <summary>
        /// Gets the graphics path from specified port style.
        /// </summary>
        /// <param name="type">The port style.</param>
        /// <param name="scale">The scale factor.</param>
        /// <returns>The graphics path.</returns>
        public static GraphicsPath GetGraphicsPath(PortVisualType type, float scale)
        {
            GraphicsPath gp = null;

            Matrix m = new Matrix(scale, 0, 0, scale, 0, 0);
            XPort.Transform(m);
            CirclePort.Transform(m);
            SquarePort.Transform(m);
            TrianglePort.Transform(m);

            switch (type)
            {
                case PortVisualType.CirclePort:
                    gp = PortVisuals.CirclePort;
                    break;
                case PortVisualType.SquarePort:
                    gp = PortVisuals.SquarePort;
                    break;
                case PortVisualType.Triangleport:
                    gp = PortVisuals.TrianglePort;
                    break;
                case PortVisualType.XPort:
                    gp = PortVisuals.XPort;
                    break;
            }
            gp.Transform(m);

            return gp;
        }
    }
}
