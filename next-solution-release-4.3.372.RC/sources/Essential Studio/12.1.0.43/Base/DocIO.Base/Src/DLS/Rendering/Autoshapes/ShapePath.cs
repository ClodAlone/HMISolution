#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Syncfusion.DocIO.DLS.Rendering;
using Syncfusion.DocIO.DLS;

namespace Syncfusion.DocIO.Rendering
{
    internal class ShapePath
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private RectangleF m_rectBounds;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string> m_shapeGuide;
        #endregion
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        internal ShapePath(RectangleF bounds, Dictionary<string, string> shapeGuide)
        {
            m_rectBounds = bounds;
            m_shapeGuide = shapeGuide;
        }
        #endregion
        #region Implementation to get Shape Path
        #region Lines
        /// <summary>
        /// Get Curved Connector path
        /// </summary>
        /// <formula>
        /// formulaColl.Add("x2","*/ w adj1 100000");
        /// formulaColl.Add("x1","+/ l x2 2");
        /// formulaColl.Add("x3","+/ r x2 2");
        /// formulaColl.Add("y3","*/ h 3 4");
        /// </formula>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetCurvedConnectorPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.CurvedConnector);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + (m_rectBounds.Height / 4));
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Bottom);
            linePoints[6] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            path.AddBeziers(linePoints);
            return path;
        }
        /// <summary>
        /// Get Bent Connector path
        /// </summary>
        /// <formula>
        /// formulaColl.Add("x1","*/ w adj1 100000");
        /// </formula>>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetBentConnectorPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.ElbowConnector);
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            GraphicsPath path = new GraphicsPath();
            path.AddLines(linePoints);
            return path;
        }
        #endregion
        #region Rectangles
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetRoundedRectanglePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RoundedRectangle);
            GraphicsPath path = new GraphicsPath();
            float diameter = formulaValues["x1"] * 2.0F;
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, diameter, diameter, 180, 90);

            path.AddArc(m_rectBounds.Right - diameter, m_rectBounds.Y, diameter, diameter, 270, 90);

            path.AddArc(m_rectBounds.Right - diameter, m_rectBounds.Bottom - diameter, diameter, diameter, 0, 90);

            path.AddArc(m_rectBounds.X, m_rectBounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetSnipSingleCornerRectanglePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.SnipSingleCornerRectangle);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[5];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["dx1"]);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[4] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetSnipSameSideCornerRectanglePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.SnipSameSideCornerRectangle);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[8];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["tx1"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["tx2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["tx1"]);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["by1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["bx2"], m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["bx1"], m_rectBounds.Bottom);
            linePoints[6] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["by1"]);
            linePoints[7] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["tx1"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetSnipDiagonalCornerRectanglePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.SnipDiagonalCornerRectangle);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[8];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["lx1"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["rx2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["rx1"]);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["ly1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["lx2"], m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["rx1"], m_rectBounds.Bottom);
            linePoints[6] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["ry1"]);
            linePoints[7] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["lx1"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetSnipAndRoundSingleCornerRectanglePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.SnipAndRoundSingleCornerRectangle);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[6];
            float diameter = formulaValues["x1"] * 2;
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["dx2"]);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[4] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["x1"]);
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, diameter, diameter, 180, 90);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetRoundSingleCornerRectanglePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RoundSingleCornerRectangle);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[2];
            float diameter = formulaValues["dx1"] * 2;
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            path.AddLines(linePoints);
            path.AddArc(m_rectBounds.Right - diameter, m_rectBounds.Y, diameter, diameter, 270, 90);
            linePoints[0] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetRoundSameSideCornerRectanglePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RoundSameSideCornerRectangle);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[2];
            float diameter1 = formulaValues["tx1"] * 2;
            float diameter2 = formulaValues["bx1"] * 2;
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["tx1"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["tx2"], m_rectBounds.Y);
            path.AddLines(linePoints);
            path.AddArc(m_rectBounds.Right - diameter1, m_rectBounds.Y, diameter1, diameter1, 270, 90);
            if (diameter2 == 0)
            {
                linePoints[0] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
                linePoints[1] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
                path.AddLines(linePoints);
            }
            else
            {
                path.AddArc(m_rectBounds.Right - diameter2, m_rectBounds.Bottom - diameter2, diameter2, diameter2, 0, 90);

                path.AddArc(m_rectBounds.X, m_rectBounds.Bottom - diameter2, diameter2, diameter2, 90, 90);
            }
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, diameter1, diameter1, 180, 90);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetRoundDiagonalCornerRectanglePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RoundDiagonalCornerRectangle);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[2];
            float diameter1 = formulaValues["x1"] * 2;
            float diameter2 = formulaValues["a"] * 2;
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            path.AddLines(linePoints);
            if (diameter2 != 0)
                path.AddArc(m_rectBounds.Right - diameter2, m_rectBounds.Y, diameter2, diameter2, 270, 90);
            path.AddArc(m_rectBounds.Right - diameter1, m_rectBounds.Bottom - diameter1, diameter1, diameter1, 0, 90);
            if (diameter2 == 0)
            {
                linePoints[0] = new PointF(m_rectBounds.Right - formulaValues["x1"], m_rectBounds.Bottom);
                linePoints[1] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
                path.AddLines(linePoints);
            }
            else
                path.AddArc(m_rectBounds.X, m_rectBounds.Bottom - diameter2, diameter2, diameter2, 90, 90);
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, diameter1, diameter1, 180, 90);
            path.CloseFigure();
            return path;
        }
        #endregion
        #region Basic Shapes
        /// <summary>
        /// Get Triangle path
        /// </summary>
        /// <param name="shapeType"></param>
        /// <param name="m_rectBounds"></param>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        internal GraphicsPath GetTrianglePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.IsoscelesTriangle);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[3];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetParallelogramPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Parallelogram);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetTrapezoidPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Trapezoid);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetRegularPentagonPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RegularPentagon);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[5];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetHexagonPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Hexagon);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[6];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + m_rectBounds.Height / 2);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + +formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetHeptagonPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Heptagon);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + +formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetOctagonPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Octagon);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[8];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["x1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["x1"]);
            linePoints[4] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[7] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetDecagonPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Decagon);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[10];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + m_rectBounds.Height / 2);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + m_rectBounds.Height / 2);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetDodecagonPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Dodecagon);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[12];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[6] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y3"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Bottom);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[11] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y3"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetPiePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Pie);
            GraphicsPath path = new GraphicsPath();
            path.AddPie(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height, formulaValues["stAng"], (formulaValues["swAng"]));
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetChordPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Chord);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height, formulaValues["stAng"], (formulaValues["swAng"]));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetTearDropPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Teardrop);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height, 180, 90);
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[6] = new PointF(m_rectBounds.Right, m_rectBounds.Y + m_rectBounds.Height / 2);
            path.AddBeziers(linePoints);
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height, 0, 90);
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height, 90, 90);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFramePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Frame);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["x1"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetHalfFramePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.HalfFrame);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[6];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetL_ShapePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.L_Shape);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[6];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetDiagonalStripePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.DiagonalStripe);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.Left, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCrossPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Cross);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[12];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["x1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["x1"]);
            linePoints[6] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[11] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetPlaquePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Plaque);
            GraphicsPath path = new GraphicsPath();
            float diameter = formulaValues["x1"] * 2.0F;
            path.AddArc(m_rectBounds.X - formulaValues["x1"], m_rectBounds.Y - formulaValues["x1"], diameter, diameter, 90, -90);

            path.AddArc(m_rectBounds.Right - formulaValues["x1"], m_rectBounds.Y - formulaValues["x1"], diameter, diameter, 180, -90);

            path.AddArc(m_rectBounds.Right - formulaValues["x1"], m_rectBounds.Bottom - formulaValues["x1"], diameter, diameter, 270, -90);

            path.AddArc(m_rectBounds.X - formulaValues["x1"], m_rectBounds.Bottom - formulaValues["x1"], diameter, diameter, 0, -90);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCanPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Can);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, formulaValues["y1"] * 2, 0, 180);
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, formulaValues["y1"] * 2, 180, 180);
            path.AddArc(m_rectBounds.X, m_rectBounds.Y + formulaValues["y3"] - formulaValues["y1"], m_rectBounds.Width, formulaValues["y1"] * 2, 0, 180);
            path.AddLine(m_rectBounds.X, m_rectBounds.Y + formulaValues["y3"] - formulaValues["y1"], m_rectBounds.X, m_rectBounds.Y + formulaValues["y1"]);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCubePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Cube);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[6];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["y1"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y4"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[5];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Bottom);
            path.AddLines(linePoints);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetBevelPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Bevel);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.Left, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            path.AddLine(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["x1"]);
            path.CloseFigure();
            path.AddLine(m_rectBounds.X, m_rectBounds.Bottom, m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            path.CloseFigure();
            path.AddLine(m_rectBounds.Right, m_rectBounds.Y, m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["x1"]);
            path.CloseFigure();
            path.AddLine(m_rectBounds.Right, m_rectBounds.Bottom, m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetDonutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Donut);
            GraphicsPath path = new GraphicsPath();
            RectangleF bounds = new RectangleF(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height);
            path.AddArc(bounds, 180, 90);
            path.AddArc(bounds, 270, 90);
            path.AddArc(bounds, 0, 90);
            path.AddArc(bounds, 90, 90);
            path.CloseFigure();
            bounds = new RectangleF(m_rectBounds.X + formulaValues["dr"], m_rectBounds.Y + formulaValues["dr"], formulaValues["iwd2"] * 2, formulaValues["ihd2"] * 2);
            path.AddArc(bounds, 180, -90);
            path.AddArc(bounds, 90, -90);
            path.AddArc(bounds, 0, -90);
            path.AddArc(bounds, 270, -90);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetNoSymbolPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.NoSymbol);
            GraphicsPath path = new GraphicsPath();
            RectangleF bounds = new RectangleF(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height);
            path.AddArc(bounds, 180, 90);
            path.AddArc(bounds, 270, 90);
            path.AddArc(bounds, 0, 90);
            path.AddArc(bounds, 90, 90);
            path.CloseFigure();
            path.AddArc(m_rectBounds.X + formulaValues["dr"], m_rectBounds.Y + formulaValues["dr"], formulaValues["iwd2"] * 2, formulaValues["ihd2"] * 2, formulaValues["stAng1"], formulaValues["swAng"]);
            path.CloseFigure();
            path.AddArc(m_rectBounds.X + formulaValues["dr"], m_rectBounds.Y + formulaValues["dr"], formulaValues["iwd2"] * 2, formulaValues["ihd2"] * 2, formulaValues["stAng2"], formulaValues["swAng"]);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetBlockArcPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.BlockArc);
            GraphicsPath path = new GraphicsPath();
            RectangleF bounds = new RectangleF(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height);
            path.AddArc(bounds, formulaValues["stAng"] / 60000, formulaValues["swAng"] / 60000);
            path.AddArc(m_rectBounds.Right - formulaValues["x2"], m_rectBounds.Bottom - formulaValues["y2"], formulaValues["iwd2"] * 2, formulaValues["ihd2"] * 2, formulaValues["istAng"] / 60000, formulaValues["iswAng"] / 60000);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFoldedCornerPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.FoldedCorner);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[8];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Top + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Top + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[4] = new PointF(m_rectBounds.Left, m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.Left, m_rectBounds.Top);
            linePoints[6] = new PointF(m_rectBounds.Right, m_rectBounds.Top);
            linePoints[7] = new PointF(m_rectBounds.Right, m_rectBounds.Top + formulaValues["y2"]);
            path.AddLines(linePoints);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath[] GetSmileyFacePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.SmileyFace);
            GraphicsPath[] path = new GraphicsPath[2];
            for (int i = 0; i < path.Length; i++)
                path[i] = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = linePoints[0];
            linePoints[2] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y + formulaValues["y5"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y2"]);
            path[1].AddBeziers(linePoints);
            path[0].AddEllipse(m_rectBounds);
            path[0].AddEllipse(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"] - formulaValues["hR"], formulaValues["wR"] * 2, formulaValues["hR"] * 2);
            path[0].AddEllipse(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"] - formulaValues["hR"], formulaValues["wR"] * 2, formulaValues["hR"] * 2);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetHeartPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Heart);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y + m_rectBounds.Height / 4);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + m_rectBounds.Height / 4);
            linePoints[3] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Bottom);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + m_rectBounds.Height / 4);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[6] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y + m_rectBounds.Height / 4);
            path.AddBeziers(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLightningBoltPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LightningBolt);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[11];
            linePoints[0] = GetXYPosition(8472, 0, 21600);
            linePoints[1] = GetXYPosition(12860, 6080, 21600);
            linePoints[2] = GetXYPosition(11050, 6797, 21600);
            linePoints[3] = GetXYPosition(16577, 12007, 21600);
            linePoints[4] = GetXYPosition(14767, 12877, 21600);
            linePoints[5] = GetXYPosition(21600, 21600, 21600);
            linePoints[6] = GetXYPosition(10012, 14915, 21600);
            linePoints[7] = GetXYPosition(12222, 13987, 21600);
            linePoints[8] = GetXYPosition(5022, 9705, 21600);
            linePoints[9] = GetXYPosition(7602, 8382, 21600);
            linePoints[10] = GetXYPosition(0, 3890, 21600);
            path.AddLines(linePoints);
            path.CloseAllFigures();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetSunPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Sun);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[3];
            linePoints[0] = new PointF(m_rectBounds.Right, m_rectBounds.Y + m_rectBounds.Height / 2);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x15"], m_rectBounds.Y + formulaValues["y18"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x15"], m_rectBounds.Y + formulaValues["y14"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["ox1"], m_rectBounds.Y + formulaValues["oy1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x16"], m_rectBounds.Y + formulaValues["y13"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x17"], m_rectBounds.Y + formulaValues["y12"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints[0] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x18"], m_rectBounds.Y + formulaValues["y10"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x14"], m_rectBounds.Y + formulaValues["y10"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["ox2"], m_rectBounds.Y + formulaValues["oy1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x13"], m_rectBounds.Y + formulaValues["y12"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x12"], m_rectBounds.Y + formulaValues["y13"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + m_rectBounds.Height / 2);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y14"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y18"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["ox2"], m_rectBounds.Y + formulaValues["oy2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x12"], m_rectBounds.Y + formulaValues["y17"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x13"], m_rectBounds.Y + formulaValues["y16"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints[0] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x14"], m_rectBounds.Y + formulaValues["y15"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x18"], m_rectBounds.Y + formulaValues["y15"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["ox1"], m_rectBounds.Y + formulaValues["oy2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x17"], m_rectBounds.Y + formulaValues["y16"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x16"], m_rectBounds.Y + formulaValues["y17"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            path.AddEllipse(m_rectBounds.X + formulaValues["x19"], m_rectBounds.Y + m_rectBounds.Height / 2 - formulaValues["hR"], formulaValues["wR"] * 2, formulaValues["hR"] * 2);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetMoonPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Moon);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width * 2, m_rectBounds.Height, 90, 180);
            float startAng = formulaValues["stAng1"];
            if (startAng < 180)
                startAng += 180;
            path.AddArc(m_rectBounds.X + formulaValues["g0w"], m_rectBounds.Y + m_rectBounds.Height / 2 - formulaValues["dy1"], formulaValues["g18w"] * 2, formulaValues["dy1"] * 2, startAng, formulaValues["swAng1"] % 360);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCloudPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Cloud);
            GraphicsPath path = new GraphicsPath();
            PointF arcLocation = GetXYPosition(3900, 14370, 43200);
            arcLocation.X += formulaValues["g27"];
            arcLocation.Y -= formulaValues["g30"];
            SizeF arcSize1 = new SizeF((m_rectBounds.Width * 6753 / 43200) * 2, (m_rectBounds.Height * 9190 / 43200) * 2);
            SizeF arcSize2 = new SizeF((m_rectBounds.Width * 5333 / 43200) * 2, (m_rectBounds.Height * 7267 / 43200) * 2);
            SizeF arcSize3 = new SizeF((m_rectBounds.Width * 4365 / 43200) * 2, (m_rectBounds.Height * 5945 / 43200) * 2);
            SizeF arcSize4 = new SizeF((m_rectBounds.Width * 4857 / 43200) * 2, (m_rectBounds.Height * 6595 / 43200) * 2);
            SizeF arcSize5 = new SizeF((m_rectBounds.Width * 5333 / 43200) * 2, (m_rectBounds.Height * 7273 / 43200) * 2);
            SizeF arcSize6 = new SizeF((m_rectBounds.Width * 6775 / 43200) * 2, (m_rectBounds.Height * 9220 / 43200) * 2);
            SizeF arcSize7 = new SizeF((m_rectBounds.Width * 5785 / 43200) * 2, (m_rectBounds.Height * 7867 / 43200) * 2);
            SizeF arcSize8 = new SizeF((m_rectBounds.Width * 6752 / 43200) * 2, (m_rectBounds.Height * 9215 / 43200) * 2);
            SizeF arcSize9 = new SizeF((m_rectBounds.Width * 7720 / 43200) * 2, (m_rectBounds.Height * 10543 / 43200) * 2);
            SizeF arcSize10 = new SizeF((m_rectBounds.Width * 4360 / 43200) * 2, (m_rectBounds.Height * 5918 / 43200) * 2);
            SizeF arcSize11 = new SizeF((m_rectBounds.Width * 4345 / 43200) * 2, (m_rectBounds.Height * 5945 / 43200) * 2);

            path.AddArc(m_rectBounds.X + (4076 * m_rectBounds.Width / 43200), m_rectBounds.Y + (3912 * m_rectBounds.Height / 43200), arcSize1.Width, arcSize1.Height, -11429249 / 60000, 7426832 / 60000);

            path.AddArc(m_rectBounds.X + (13469 * m_rectBounds.Width / 43200), m_rectBounds.Y + (1304 * m_rectBounds.Height / 43200), arcSize2.Width, arcSize2.Height, -8646143 / 60000, 5396714 / 60000);

            path.AddArc(m_rectBounds.X + m_rectBounds.Width / 2 + (531 * m_rectBounds.Width / 43200), m_rectBounds.Y + 1, arcSize3.Width, arcSize3.Height, -8748475 / 60000, 5983381 / 60000);

            path.AddArc(arcLocation.X + m_rectBounds.Width / 2 + (3013 * m_rectBounds.Width / 43200), m_rectBounds.Y + 1, arcSize4.Width, arcSize4.Height, -7859164 / 60000, 7034504 / 60000);

            path.AddArc(m_rectBounds.Right - arcSize5.Width - (708 * m_rectBounds.Width / 43200), m_rectBounds.Y + arcSize4.Height / 2 - (1127 * m_rectBounds.Height / 43200), arcSize5.Width, arcSize5.Height, -4722533 / 60000, 6541615 / 60000);

            path.AddArc(m_rectBounds.Right - arcSize6.Width + (354 * m_rectBounds.Width / 43200), m_rectBounds.Y + m_rectBounds.Height / 2 - (9129 * m_rectBounds.Height / 43200), arcSize6.Width, arcSize6.Height, -2776035 / 60000, 7816140 / 60000);

            path.AddArc(m_rectBounds.X + m_rectBounds.Width / 2 + (4608 * m_rectBounds.Width / 43200), m_rectBounds.Y + m_rectBounds.Height / 2 + (869 * m_rectBounds.Height / 43200), arcSize7.Width, arcSize7.Height, 37501 / 60000, 6842000 / 60000);

            path.AddArc(m_rectBounds.X + m_rectBounds.Width / 2 - arcSize8.Width / 2 + (886 * m_rectBounds.Width / 43200), m_rectBounds.Bottom - arcSize8.Height, arcSize8.Width, arcSize8.Height, 1347096 / 60000, 6910353 / 60000);

            path.AddArc(m_rectBounds.X + (4962 * m_rectBounds.Width / 43200), m_rectBounds.Bottom - arcSize9.Height - (2173 * m_rectBounds.Height / 43200), arcSize9.Width, arcSize9.Height, 3974558 / 60000, 4542661 / 60000);

            path.AddArc(m_rectBounds.X + (1063 * m_rectBounds.Width / 43200), m_rectBounds.Y + m_rectBounds.Height / 2 + (2608 * m_rectBounds.Height / 43200), arcSize10.Width, arcSize10.Height, -16496525 / 60000, 8804134 / 60000);

            path.AddArc(m_rectBounds.X + 1, m_rectBounds.Y + m_rectBounds.Height / 2 - arcSize11.Height / 2 - (1304 * m_rectBounds.Height / 43200), arcSize11.Width, arcSize11.Height, -14809710 / 60000, 9151131 / 60000);
            path.CloseFigure();
            path.AddArc(m_rectBounds.X + m_rectBounds.Width / 2 + (2658 * m_rectBounds.Width / 43200), m_rectBounds.Y + m_rectBounds.Height / 2, (m_rectBounds.Width * 6753 / 43200) * 2, (m_rectBounds.Height * 9190 / 43200) * 2, -824660 / 60000 - 45, 891534 / 60000 + 45);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath[] GetArcPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Arc);
            GraphicsPath[] path = new GraphicsPath[2];
            for (int i = 0; i < path.Length; i++)
                path[i] = new GraphicsPath();
            path[0].AddArc(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height, formulaValues["stAng"] / 60000, formulaValues["swAng"] / 60000);
            path[1].AddPie(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height, formulaValues["stAng"] / 60000, formulaValues["swAng"] / 60000);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetDoubleBracketPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.DoubleBracket);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.X, m_rectBounds.Bottom - formulaValues["x1"] * 2, formulaValues["x1"] * 2, formulaValues["x1"] * 2, 90, 90);

            path.AddArc(m_rectBounds.X, m_rectBounds.Y, formulaValues["x1"] * 2, formulaValues["x1"] * 2, 180, 90);
            path.StartFigure();
            path.AddArc(m_rectBounds.X + formulaValues["x2"] - formulaValues["x1"], m_rectBounds.Y, formulaValues["x1"] * 2, formulaValues["x1"] * 2, 270, 90);

            path.AddArc(m_rectBounds.X + formulaValues["x2"] - formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"] - formulaValues["x1"], formulaValues["x1"] * 2, formulaValues["x1"] * 2, 0, 90);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetDoubleBracePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.DoubleBrace);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.X - formulaValues["x1"] + formulaValues["x2"], m_rectBounds.Bottom - formulaValues["x1"] * 2, formulaValues["x1"] * 2, formulaValues["x1"] * 2, 90, 90);

            path.AddArc(m_rectBounds.X - formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"] - formulaValues["x1"], formulaValues["x1"] * 2, formulaValues["x1"] * 2, 0, -90);
            path.AddArc(m_rectBounds.X - formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"] - formulaValues["x1"] * 3, formulaValues["x1"] * 2, formulaValues["x1"] * 2, 90, -90);

            path.AddArc(m_rectBounds.X - formulaValues["x1"] + formulaValues["x2"], m_rectBounds.Y, formulaValues["x1"] * 2, formulaValues["x1"] * 2, 180, 90);

            path.StartFigure();
            path.AddArc(m_rectBounds.X + formulaValues["x3"] - formulaValues["x1"], m_rectBounds.Top, formulaValues["x1"] * 2, formulaValues["x1"] * 2, 270, 90);

            path.AddArc(m_rectBounds.Right - formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"] - formulaValues["x1"], formulaValues["x1"] * 2, formulaValues["x1"] * 2, 180, -90);
            path.AddArc(m_rectBounds.Right - formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"] + formulaValues["x1"], formulaValues["x1"] * 2, formulaValues["x1"] * 2, 270, -90);

            path.AddArc(m_rectBounds.X + formulaValues["x3"] - formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"], formulaValues["x1"] * 2, formulaValues["x1"], 0, 90);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLeftBracketPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LeftBracket);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.Right - m_rectBounds.Width, m_rectBounds.Bottom - (formulaValues["y1"] * 2), m_rectBounds.Width * 2, formulaValues["y1"] * 2, 90, 90);
            path.AddArc(m_rectBounds.Right - m_rectBounds.Width, m_rectBounds.Y, m_rectBounds.Width * 2, formulaValues["y1"] * 2, 180, 90);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetRightBracketPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RightBracket);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.X - m_rectBounds.Width, m_rectBounds.Y, m_rectBounds.Width * 2, formulaValues["y1"] * 2, 270, 90);
            path.AddArc(m_rectBounds.X - m_rectBounds.Width, m_rectBounds.Y + formulaValues["y2"] - formulaValues["y1"], m_rectBounds.Width * 2, formulaValues["y1"] * 2, 0, 90);
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLeftBracePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LeftBrace);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.Right - m_rectBounds.Width / 2, m_rectBounds.Bottom - (formulaValues["y1"] * 2), m_rectBounds.Width, formulaValues["y1"] * 2, 90, 90);

            path.AddArc(m_rectBounds.X - m_rectBounds.Width / 2, m_rectBounds.Y + formulaValues["y4"] - formulaValues["y1"], m_rectBounds.Width, formulaValues["y1"] * 2, 0, -90);
            path.AddArc(m_rectBounds.X - m_rectBounds.Width / 2, m_rectBounds.Y + formulaValues["y4"] - formulaValues["y1"] * 3, m_rectBounds.Width, formulaValues["y1"] * 2, 90, -90);

            path.AddArc(m_rectBounds.Right - m_rectBounds.Width / 2, m_rectBounds.Y, m_rectBounds.Width, formulaValues["y1"] * 2, 180, 90);

            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetRightBracePath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RightBrace);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(m_rectBounds.X - m_rectBounds.Width / 2, m_rectBounds.Top, m_rectBounds.Width, formulaValues["y1"] * 2, 270, 90);
            PointF arcLocation = new PointF(m_rectBounds.X + m_rectBounds.Width, m_rectBounds.Y + formulaValues["y2"] - formulaValues["y1"]);
            path.AddArc(arcLocation.X - m_rectBounds.Width / 2, arcLocation.Y, m_rectBounds.Width, formulaValues["y1"] * 2, 180, -90);
            path.AddArc(arcLocation.X - m_rectBounds.Width / 2, arcLocation.Y + formulaValues["y1"] * 2, m_rectBounds.Width, formulaValues["y1"] * 2, 270, -90);
            path.AddArc(m_rectBounds.X - m_rectBounds.Width / 2, m_rectBounds.Y + formulaValues["y4"] - formulaValues["y1"], m_rectBounds.Width, formulaValues["y1"] * 2, 0, 90);
            return path;
        }
        #endregion
        #region Block Arrows
        /// <summary>
        /// Gets the right arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetRightArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RightArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[6] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the left arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLeftArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LeftArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        internal GraphicsPath GetUpArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.UpArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets down arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetDownArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.DownArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[6] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the left right arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLeftRightArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LeftRightArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[10];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Bottom);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the curved right arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCurvedRightArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.CurvedRightArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["hR"]);
            path.AddArc(linePoints[0].X, linePoints[0].Y - formulaValues["hR"], m_rectBounds.Width * 2, formulaValues["hR"] * 2, 180, formulaValues["mswAng"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            //path.AddLine(m_rectBounds.X + formulaValues["x1"], linePoints[0].Y + (formulaValues["hR"]), linePoints[1].X, linePoints[1].Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y6"]);
            path.AddLine(linePoints[1], linePoints[2]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y8"]);
            path.AddLine(linePoints[2], linePoints[3]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y7"]);
            //path.AddLine(linePoints[3], linePoints[4]);
            path.AddArc(linePoints[4].X - formulaValues["x1"], linePoints[4].Y - (formulaValues["hR"] * 2), m_rectBounds.Width * 2, formulaValues["hR"] * 2, formulaValues["stAng"], formulaValues["swAng"]);
            path.CloseFigure();
            linePoints[5] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["hR"]);
            //path.AddLine(linePoints[4].X - formulaValues["x1"], linePoints[4].Y - (formulaValues["hR"] * 2), linePoints[5].X, linePoints[5].Y);
            path.AddArc(linePoints[5].X, linePoints[5].Y - (formulaValues["hR"]), m_rectBounds.Width * 2, formulaValues["hR"] * 2, 180, 90);
            linePoints[6] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["th"]);
            path.AddLine(linePoints[5].X + (m_rectBounds.Width), linePoints[5].Y - (formulaValues["hR"]), linePoints[6].X, linePoints[6].Y);
            path.AddArc(linePoints[6].X - (m_rectBounds.Width), linePoints[6].Y, m_rectBounds.Width * 2, formulaValues["hR"] * 2, 270, formulaValues["swAng2"]);
            return path;
        }
        /// <summary>
        /// Gets the curved left arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCurvedLeftArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.CurvedLeftArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y3"]);
            path.AddArc(m_rectBounds.Right - (m_rectBounds.Width * 2), linePoints[0].Y - formulaValues["hR"], m_rectBounds.Width * 2, formulaValues["hR"] * 2, 0, -90);
            linePoints[1] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            path.AddArc(linePoints[1].X - m_rectBounds.Width, linePoints[1].Y, m_rectBounds.Width * 2, formulaValues["hR"] * 2, 270, 90);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y3"]);
            path.AddArc(linePoints[2].X - (m_rectBounds.Width * 2), linePoints[2].Y - formulaValues["hR"], m_rectBounds.Width * 2, formulaValues["hR"] * 2, 0, formulaValues["swAng"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y8"]);
            linePoints[4] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y6"]);
            path.AddLine(linePoints[3], linePoints[4]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[4], linePoints[5]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y5"]);
            path.AddLine(linePoints[5], linePoints[6]);
            path.AddArc(m_rectBounds.X - m_rectBounds.Width, linePoints[1].Y, m_rectBounds.Width * 2, formulaValues["hR"] * 2, formulaValues["swAng"], formulaValues["swAng2"]);
            return path;
        }
        /// <summary>
        /// Gets the curved up arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCurvedUpArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.CurvedUpArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            SizeF size = new SizeF(formulaValues["wR"], m_rectBounds.Height);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["ix"], m_rectBounds.Y + formulaValues["iy"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["wR"], m_rectBounds.Bottom);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["th"], m_rectBounds.Y);
            path.AddArc(linePoints[5].X - size.Width, linePoints[0].Y - size.Height - formulaValues["iy"], size.Width * 2, size.Height * 2, formulaValues["stAng2"], formulaValues["swAng2"]);
            path.AddLine(linePoints[1], linePoints[2]);
            path.AddLine(linePoints[2], linePoints[3]);
            path.AddLine(linePoints[3], linePoints[4]);
            path.AddArc(linePoints[6].X, m_rectBounds.Y - size.Height, size.Width * 2, size.Height * 2, formulaValues["stAng3"], formulaValues["swAng"]);
            path.AddArc(linePoints[5].X - size.Width, linePoints[5].Y - size.Height * 2, size.Width * 2, size.Height * 2, 90, 90);
            path.AddArc(linePoints[6].X, linePoints[6].Y - size.Height, size.Width * 2, size.Height * 2, 180, -90);
            return path;
        }
        /// <summary>
        /// Gets the curved down arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCurvedDownArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.CurvedDownArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[7];
            SizeF size = new SizeF(formulaValues["wR"], m_rectBounds.Height);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["ix"], m_rectBounds.Y + formulaValues["iy"]);
            linePoints[1] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddArc(linePoints[2].X - size.Width, m_rectBounds.Y, size.Width * 2, size.Height * 2, formulaValues["stAng2"], formulaValues["swAng2"]);
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, size.Width * 2, size.Height * 2, 180, 90);
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, size.Width * 2, size.Height * 2, 270, formulaValues["swAng"]);
            path.AddLine(linePoints[6], linePoints[5]);
            path.AddLine(linePoints[5], linePoints[4]);
            path.AddLine(linePoints[4], linePoints[3]);
            path.AddLine(linePoints[3], new PointF(linePoints[3].X - formulaValues["x5"] + formulaValues["x4"], linePoints[3].Y));
            path.AddArc(linePoints[2].X - size.Width, m_rectBounds.Y, size.Width * 2, size.Height * 2, formulaValues["stAng"], formulaValues["mswAng"]);
            PointF[] linePoint = new PointF[1];
            linePoint[0] = new PointF(m_rectBounds.X + size.Width, m_rectBounds.Y);
            path.AddLines(linePoint);
            return path;
        }
        /// <summary>
        /// Gets up down arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetUpDownArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.UpDownArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[10];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y3"]);
            linePoints[6] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Bottom);
            linePoints[7] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y3"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the quad arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetQuadArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.QuadArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[24];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[6] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[12] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[16] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[18] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Bottom);
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[20] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[21] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[22] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[23] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y5"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the left right up arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLeftRightUpArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LeftRightUpArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[17];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y4"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[6] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[12] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y4"]);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Bottom);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[16] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the bent arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetBentArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.BentArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[6];
            path.AddLine(m_rectBounds.X, m_rectBounds.Bottom, m_rectBounds.X, m_rectBounds.Y + formulaValues["y5"]);
            path.AddArc(m_rectBounds.X, m_rectBounds.Y + formulaValues["y5"] - formulaValues["bd"], formulaValues["bd"] * 2, formulaValues["bd"] * 2, 180, 90);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["dh2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["aw2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLines(linePoints);
            path.AddArc(linePoints[5].X - formulaValues["bd2"], linePoints[5].Y, formulaValues["bd2"] * 2, formulaValues["bd2"] * 2, 270, -90);
            path.AddLine(linePoints[5].X - formulaValues["bd2"], linePoints[5].Y + formulaValues["bd2"], m_rectBounds.X + formulaValues["th"], m_rectBounds.Bottom);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the U trun arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetUTrunArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.UTurnArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[11];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["bd"]);
            path.AddLine(linePoints[0], linePoints[1]);
            path.AddArc(linePoints[1].X, linePoints[1].Y - formulaValues["bd"], formulaValues["bd"] * 2, formulaValues["bd"] * 2, 180, 90);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y);
            path.AddLine(linePoints[1].X + formulaValues["bd"], linePoints[1].Y - formulaValues["bd"], linePoints[2].X, linePoints[2].Y);
            path.AddArc(linePoints[2].X - formulaValues["bd"], linePoints[2].Y, formulaValues["bd"] * 2, formulaValues["bd"] * 2, 270, 90);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[2].X + formulaValues["bd"], linePoints[2].Y + formulaValues["bd"], linePoints[3].X, linePoints[3].Y);
            linePoints[4] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[3], linePoints[4]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y5"]);
            path.AddLine(linePoints[4], linePoints[5]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[5], linePoints[6]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[6], linePoints[7]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["x3"]);
            path.AddLine(linePoints[7], linePoints[8]);
            path.AddArc(linePoints[8].X - (formulaValues["bd2"] * 2), linePoints[8].Y - formulaValues["bd2"], formulaValues["bd2"] * 2, formulaValues["bd2"] * 2, 0, -90);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["th"]);
            path.AddLine(linePoints[8].X - formulaValues["bd2"], linePoints[8].Y - formulaValues["bd2"], linePoints[9].X, linePoints[9].Y);
            path.AddArc(linePoints[9].X - formulaValues["bd2"], linePoints[9].Y, formulaValues["bd2"] * 2, formulaValues["bd2"] * 2, 270, -90);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["th"], m_rectBounds.Bottom);
            path.AddLine(linePoints[9].X - formulaValues["bd2"], linePoints[9].Y + formulaValues["bd2"], linePoints[10].X, linePoints[10].Y);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the left up arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLeftUpArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LeftUpArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[12];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y4"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y);
            linePoints[7] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["x1"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["x1"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the bent up arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetBentUpArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.BentUpArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[9];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Bottom);
            linePoints[8] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the striped right arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetStripedRightArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.StripedRightArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + (Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 32), m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + (Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 32), m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + (Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 16), m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + (Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 8), m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + (Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 8), m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + (Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 16), m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[7];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the notched right arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetNotchedRightArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.NotchedRightArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[8];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[6] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + (m_rectBounds.Height / 2));
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the pentagon path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetPentagonPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Pentagon);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[5];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[4] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the chevron path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetChevronPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Chevron);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[6];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[4] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + (m_rectBounds.Height / 2));
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the right arrow callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetRightArrowCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RightArrowCallout);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[11];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[10] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets down arrow callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetDownArrowCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.DownArrowCallout);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[11];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[6] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Bottom);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[10] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the left arrow callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLeftArrowCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LeftArrowCallout);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[11];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[6] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets up arrow callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetUpArrowCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.UpArrowCallout);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[11];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[8] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[9] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom); linePoints[10] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the left right arrow callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLeftRightArrowCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LeftRightArrowCallout);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[18];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[9] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[12] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Bottom);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[16] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the quad arrow callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetQuadArrowCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.QuadArrowCallout);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[32];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["ah"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["ah"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["ah"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["ah"]);
            linePoints[8] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["ah"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["ah"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[12] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[16] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[18] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[20] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints[21] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints[22] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y8"]);
            linePoints[23] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y8"]);
            linePoints[24] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Bottom);
            linePoints[25] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y8"]);
            linePoints[26] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y8"]);
            linePoints[27] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints[28] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints[29] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[30] = new PointF(m_rectBounds.X + formulaValues["ah"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[31] = new PointF(m_rectBounds.X + formulaValues["ah"], m_rectBounds.Y + formulaValues["y6"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the circular arrow path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCircularArrowPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.CircularArrow);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[5];
            SizeF size = new SizeF(formulaValues["rw1"], formulaValues["rh1"]);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["xE"], m_rectBounds.Y + formulaValues["yE"]);
            path.AddArc(linePoints[0].X - size.Width * 2, linePoints[0].Y - size.Height, size.Width * 2, size.Height * 2, formulaValues["stAng"], formulaValues["swAng"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["xGp"], m_rectBounds.Y + formulaValues["yGp"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["xA"], m_rectBounds.Y + formulaValues["yA"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["xBp"], m_rectBounds.Y + formulaValues["yBp"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["xC"], m_rectBounds.Y + formulaValues["yC"]);
            path.AddLine(new PointF(linePoints[1].X - (linePoints[4].X - linePoints[3].X), linePoints[1].Y), linePoints[1]);
            path.AddLine(linePoints[1], linePoints[2]);
            path.AddLine(linePoints[2], linePoints[3]);
            path.AddLine(linePoints[3], linePoints[4]);
            SizeF size1 = new SizeF(formulaValues["rw2"], formulaValues["rh2"]);
            path.AddArc(linePoints[0].X - size.Width - size1.Width, linePoints[0].Y - size1.Height, size1.Width * 2, size1.Height * 2, formulaValues["istAng"], formulaValues["iswAng"]);
            path.CloseFigure();
            return path;
        }
        #endregion
        #region Equation Shapes
        /// <summary>
        /// Gets the math plus path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetMathPlusPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.MathPlus);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[12];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the math minus path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetMathMinusPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.MathMinus);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the math multiply path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetMathMultiplyPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.MathMultiply);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[12];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["xA"], m_rectBounds.Y + formulaValues["yA"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["xB"], m_rectBounds.Y + formulaValues["yB"]);
            linePoints[2] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y + formulaValues["yC"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["xD"], m_rectBounds.Y + formulaValues["yB"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["xE"], m_rectBounds.Y + formulaValues["yA"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["xF"], m_rectBounds.Y + (m_rectBounds.Height / 2));
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["xE"], m_rectBounds.Y + formulaValues["yG"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["xD"], m_rectBounds.Y + formulaValues["yH"]);
            linePoints[8] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y + formulaValues["yI"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["xB"], m_rectBounds.Y + formulaValues["yH"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["xA"], m_rectBounds.Y + formulaValues["yG"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["xL"], m_rectBounds.Y + (m_rectBounds.Height / 2));
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the math division path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetMathDivisionPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.MathDivision);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            path.AddEllipse(m_rectBounds.X + (m_rectBounds.Width / 2) - formulaValues["rad"], m_rectBounds.Y + formulaValues["y1"], formulaValues["rad"] * 2, formulaValues["rad"] * 2);
            path.CloseFigure();
            path.AddEllipse(m_rectBounds.X + (m_rectBounds.Width / 2) - formulaValues["rad"], m_rectBounds.Y + formulaValues["y5"] - formulaValues["rad"] * 2, formulaValues["rad"] * 2, formulaValues["rad"] * 2);
            path.CloseFigure();
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the math equal path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetMathEqualPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.MathEqual);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the math not equal path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetMathNotEqualPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.MathNotEqual);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[20];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["lx"], m_rectBounds.Y + formulaValues["ly"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["rx"], m_rectBounds.Y + formulaValues["ry"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["rx6"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["rx5"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["rx4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["rx3"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[12] = new PointF(m_rectBounds.X + formulaValues["drx"], m_rectBounds.Y + formulaValues["dry"]);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["dlx"], m_rectBounds.Y + formulaValues["dly"]);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[16] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[18] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        #endregion
        #region FlowCharts
        /// <summary>
        /// Gets the flow chart alternate process path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartAlternateProcessPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.FlowChartAlternateProcess);
            GraphicsPath path = new GraphicsPath();
            float diameter = GetPresetOperandValue("ssd6") * 2.0F;
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, diameter, diameter, 180, 90);
            path.AddArc(m_rectBounds.Right - diameter, m_rectBounds.Y, diameter, diameter, 270, 90);
            path.AddArc(m_rectBounds.Right - diameter, m_rectBounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(m_rectBounds.X, m_rectBounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart predefined process path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartPredefinedProcessPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(m_rectBounds);
            path.AddLine(m_rectBounds.X + m_rectBounds.Width / 8, m_rectBounds.Y, m_rectBounds.X + m_rectBounds.Width / 8, m_rectBounds.Bottom);
            path.CloseFigure();
            path.AddLine(m_rectBounds.Right - m_rectBounds.Width / 8, m_rectBounds.Y, m_rectBounds.Right - m_rectBounds.Width / 8, m_rectBounds.Bottom);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart internal storage path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartInternalStoragePath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(m_rectBounds);
            path.AddLine(m_rectBounds.X + m_rectBounds.Width / 8, m_rectBounds.Y, m_rectBounds.X + m_rectBounds.Width / 8, m_rectBounds.Bottom);
            path.CloseFigure();
            path.AddLine(m_rectBounds.X, m_rectBounds.Y + m_rectBounds.Height / 8, m_rectBounds.Right, m_rectBounds.Top + m_rectBounds.Height / 8);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart document path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartDocumentPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Right, m_rectBounds.Y);
            path.AddLine(m_rectBounds.Right, m_rectBounds.Y, m_rectBounds.Right, m_rectBounds.Y + ((m_rectBounds.Height * 17322) / 21600));
            PointF point1 = GetXYPosition(21600, 17322, 21600);// new PointF(bounds.Right, bounds.Y + ((bounds.Height * 17322) / 21600));
            PointF point2 = GetXYPosition(10800, 17322, 21600);// new PointF(bounds.X + ((bounds.Width * 10200) / 21600), bounds.Y + ((bounds.Height * 17322) / 21600));
            PointF point3 = GetXYPosition(10800, 23922, 21600);// new PointF(bounds.X + ((bounds.Width * 10200) / 21600), bounds.Y + ((bounds.Height * 23922) / 21600));
            PointF point4 = GetXYPosition(0, 20172, 21600);// new PointF(bounds.X, bounds.Y + ((bounds.Height * 20172) / 21600));
            path.AddBezier(point1, point2, point3, point4);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart multi document path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartMultiDocumentPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 3675, 21600), GetXYPosition(18595, 3675, 21600));
            path.AddLine(GetXYPosition(18595, 3675, 21600), GetXYPosition(18595, 18022, 21600));
            path.AddBezier(GetXYPosition(18595, 18022, 21600), GetXYPosition(9298, 18022, 21600), GetXYPosition(9298, 23542, 21600), GetXYPosition(0, 20782, 21600));
            path.CloseFigure();
            path.AddLine(GetXYPosition(1532, 3675, 21600), GetXYPosition(1532, 1815, 21600));
            path.AddLine(GetXYPosition(1532, 1815, 21600), GetXYPosition(20000, 1815, 21600));
            path.AddLine(GetXYPosition(20000, 1815, 21600), GetXYPosition(20000, 16252, 21600));
            path.AddBezier(GetXYPosition(20000, 16252, 21600), GetXYPosition(19298, 16252, 21600), GetXYPosition(18595, 16352, 21600), GetXYPosition(18595, 16352, 21600));
            path.StartFigure();
            path.AddLine(GetXYPosition(2972, 1815, 21600), GetXYPosition(2972, 0, 21600));
            path.AddLine(GetXYPosition(2972, 0, 21600), GetXYPosition(21600, 0, 21600));
            path.AddLine(GetXYPosition(21600, 0, 21600), GetXYPosition(21600, 14392, 21600));
            path.AddBezier(GetXYPosition(21600, 14392, 21600), GetXYPosition(20800, 14392, 21600), GetXYPosition(20000, 14467, 21600), GetXYPosition(20000, 14467, 21600));
            return path;
        }
        /// <summary>
        /// Gets the flow chart terminator path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartTerminatorPath()
        {
            GraphicsPath path = new GraphicsPath();
            SizeF arcSize = new SizeF(((m_rectBounds.Width * 3475) / 21600) * 2, ((m_rectBounds.Height * 10800) / 21600) * 2);
            path.AddLine(GetXYPosition(3475, 0, 21600), GetXYPosition(18125, 0, 21600));
            path.StartFigure();
            PointF arcPoint = GetXYPosition(18125, 0, 21600);
            RectangleF bounds = new RectangleF(arcPoint.X - (arcSize.Width / 2), arcPoint.Y, arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 270, 180);
            path.AddLine(new PointF(bounds.X, bounds.Y + bounds.Height), GetXYPosition(3475, 21600, 21600));
            arcPoint = GetXYPosition(3475, 0, 21600);
            bounds = new RectangleF(arcPoint.X - (arcSize.Width / 2), arcPoint.Y, arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 90, 180);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart preparation path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartPreparationPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 5, 10), GetXYPosition(2, 0, 10));
            path.AddLine(GetXYPosition(2, 0, 10), GetXYPosition(8, 0, 10));
            path.AddLine(GetXYPosition(8, 0, 10), GetXYPosition(10, 5, 10));
            path.AddLine(GetXYPosition(10, 5, 10), GetXYPosition(8, 10, 10));
            path.AddLine(GetXYPosition(8, 10, 10), GetXYPosition(2, 10, 10));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart manual input path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartManualInputPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 1, 5), GetXYPosition(5, 0, 5));
            path.AddLine(GetXYPosition(5, 0, 5), GetXYPosition(5, 5, 5));
            path.AddLine(GetXYPosition(5, 5, 5), GetXYPosition(0, 5, 5));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart manual operation path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartManualOperationPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 0, 5), GetXYPosition(5, 0, 5));
            path.AddLine(GetXYPosition(5, 0, 5), GetXYPosition(4, 5, 5));
            path.AddLine(GetXYPosition(4, 5, 5), GetXYPosition(1, 5, 5));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart connector path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartConnectorPath()
        {
            GraphicsPath path = new GraphicsPath();
            RectangleF bounds = new RectangleF(new PointF(m_rectBounds.X, m_rectBounds.Y), new SizeF(m_rectBounds.Width, m_rectBounds.Height));
            path.AddArc(bounds, 180, 90);
            path.AddArc(bounds, 270, 90);
            path.AddArc(bounds, 0, 90);
            path.AddArc(bounds, 90, 90);
            return path;
        }
        /// <summary>
        /// Gets the flow chart off page connector path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartOffPageConnectorPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 0, 10), GetXYPosition(10, 0, 10));
            path.AddLine(GetXYPosition(10, 0, 10), GetXYPosition(10, 8, 10));
            path.AddLine(GetXYPosition(10, 8, 10), GetXYPosition(5, 10, 10));
            path.AddLine(GetXYPosition(5, 10, 10), GetXYPosition(0, 8, 10));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart card path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartCardPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 1, 5), GetXYPosition(1, 0, 5));
            path.AddLine(GetXYPosition(1, 0, 5), GetXYPosition(5, 0, 5));
            path.AddLine(GetXYPosition(5, 0, 5), GetXYPosition(5, 5, 5));
            path.AddLine(GetXYPosition(5, 5, 5), GetXYPosition(0, 5, 5));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart punched tape path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartPunchedTapePath()
        {
            GraphicsPath path = new GraphicsPath();
            RectangleF bounds = new RectangleF(GetXYPosition(0, 2, 20), new SizeF(((m_rectBounds.Width * 5) / 20) * 2, ((m_rectBounds.Height * 2) / 20) * 2));// GetXYPosition(5,2,20)));
            path.AddArc(bounds, 180, -180);
            PointF P = new PointF(bounds.X + bounds.Width, bounds.Y);
            bounds = new RectangleF(P, new SizeF(((m_rectBounds.Width * 5) / 20) * 2, ((m_rectBounds.Height * 2) / 20) * 2));// GetXYPosition(5,2,20)));
            path.AddArc(bounds, 180, 180);
            path.AddLine(new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height), GetXYPosition(20, 18, 20));
            bounds = new RectangleF(new PointF(GetXYPosition(20, 18, 20).X - bounds.Width, GetXYPosition(20, 18, 20).Y), new SizeF(((m_rectBounds.Width * 5) / 20) * 2, ((m_rectBounds.Height * 2) / 20) * 2));// GetXYPosition(5,2,20)));
            path.AddArc(bounds, 0, -180);
            P = new PointF(bounds.X - bounds.Width, bounds.Y);
            bounds = new RectangleF(P, new SizeF(((m_rectBounds.Width * 5) / 20) * 2, ((m_rectBounds.Height * 2) / 20) * 2));// GetXYPosition(5,2,20)));
            path.AddArc(bounds, 0, 180);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart summing junction path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartSummingJunctionPath()
        {
            GraphicsPath path = new GraphicsPath();
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.FlowChartSummingJunction);
            path.AddLine(new PointF(m_rectBounds.X + formulaValues["il"], m_rectBounds.Y + formulaValues["it"]), new PointF(m_rectBounds.X + formulaValues["ir"], m_rectBounds.Y + formulaValues["ib"]));
            path.StartFigure();
            path.AddLine(new PointF(m_rectBounds.X + formulaValues["ir"], m_rectBounds.Y + formulaValues["it"]), new PointF(m_rectBounds.X + formulaValues["il"], m_rectBounds.Y + formulaValues["ib"]));
            path.StartFigure();
            path.AddArc(m_rectBounds, 180, 90);
            path.AddArc(m_rectBounds, 270, 90);
            path.AddArc(m_rectBounds, 0, 90);
            path.AddArc(m_rectBounds, 90, 90);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart or path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartOrPath()
        {
            GraphicsPath path = new GraphicsPath();
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.FlowChartOr);
            path.AddLine(new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y), new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Bottom));
            path.StartFigure();
            path.AddLine(new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2)), new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2)));
            path.StartFigure();
            path.AddArc(m_rectBounds, 180, 90);
            path.AddArc(m_rectBounds, 270, 90);
            path.AddArc(m_rectBounds, 0, 90);
            path.AddArc(m_rectBounds, 90, 90);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart collate path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartCollatePath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 0, 2), GetXYPosition(2, 0, 2));
            path.AddLine(GetXYPosition(2, 0, 2), GetXYPosition(1, 1, 2));
            path.AddLine(GetXYPosition(1, 1, 2), GetXYPosition(2, 2, 2));
            path.AddLine(GetXYPosition(2, 2, 2), GetXYPosition(0, 2, 2));
            path.AddLine(GetXYPosition(0, 2, 2), GetXYPosition(1, 1, 2));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart sort path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartSortPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 1, 2), GetXYPosition(2, 1, 2));
            path.AddLine(GetXYPosition(2, 1, 2), GetXYPosition(0, 1, 2));
            path.AddLine(GetXYPosition(0, 1, 2), GetXYPosition(1, 0, 2));
            path.AddLine(GetXYPosition(1, 0, 2), GetXYPosition(2, 1, 2));
            path.AddLine(GetXYPosition(2, 1, 2), GetXYPosition(1, 2, 2));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart extract path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartExtractPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 2, 2), GetXYPosition(1, 0, 2));
            path.AddLine(GetXYPosition(1, 0, 2), GetXYPosition(2, 2, 2));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart merge path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartMergePath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 0, 2), GetXYPosition(2, 0, 2));
            path.AddLine(GetXYPosition(2, 0, 2), GetXYPosition(1, 2, 2));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart online storage path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartOnlineStoragePath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(1, 0, 6), GetXYPosition(6, 0, 6));
            SizeF arcSize = new SizeF((m_rectBounds.Width / 6) * 2, (m_rectBounds.Height / 2) * 2);
            PointF arcPoint = GetXYPosition(6, 0, 6);
            RectangleF bounds = new RectangleF(arcPoint.X - (arcSize.Width / 2), arcPoint.Y, arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 270, -180);
            path.AddLine(new PointF(arcPoint.X, arcPoint.Y + arcSize.Height), GetXYPosition(1, 6, 6));
            arcPoint = GetXYPosition(1, 0, 6);
            bounds = new RectangleF(arcPoint.X - (arcSize.Width / 2), arcPoint.Y, arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 90, 180);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart delay path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartDelayPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(new PointF(m_rectBounds.X, m_rectBounds.Y), new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y));
            SizeF arcSize = new SizeF(m_rectBounds.Width, m_rectBounds.Height);
            PointF arcPoint = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2), m_rectBounds.Y);
            RectangleF bounds = new RectangleF(arcPoint.X - (arcSize.Width / 2), arcPoint.Y, arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 270, 180);
            path.AddLine(new PointF(arcPoint.X, arcPoint.Y + arcSize.Height), new PointF(m_rectBounds.X, m_rectBounds.Bottom));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart sequential access storage path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartSequentialAccessStoragePath()
        {
            GraphicsPath path = new GraphicsPath();
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.FlowChartSequentialAccessStorage);
            SizeF arcSize = new SizeF(m_rectBounds.Width, m_rectBounds.Height);
            RectangleF bounds = new RectangleF(m_rectBounds.X, m_rectBounds.Y, arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 90, 90);
            path.AddArc(bounds, 180, 90);
            path.AddArc(bounds, 270, 90);
            path.AddArc(bounds, 0, formulaValues["ang1"]);
            path.AddLine(new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["ib"]), new PointF(m_rectBounds.Right, m_rectBounds.Bottom));
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart magnetic disk path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartMagneticDiskPath()
        {
            GraphicsPath path = new GraphicsPath();
            SizeF arcSize = new SizeF(m_rectBounds.Width, m_rectBounds.Height / 3);
            PointF arcPoint = GetXYPosition(6, 1, 6);
            RectangleF bounds = new RectangleF(arcPoint.X - arcSize.Width, arcPoint.Y - (arcSize.Height / 2), arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 0, 180);
            path.StartFigure();
            arcPoint = GetXYPosition(0, 1, 6);
            bounds = new RectangleF(arcPoint.X, arcPoint.Y - (arcSize.Height / 2), arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 180, 180);
            path.AddLine(new PointF(bounds.X + bounds.Width, bounds.Y + arcSize.Height), GetXYPosition(6, 5, 6));
            arcPoint = GetXYPosition(6, 5, 6);
            bounds = new RectangleF(arcPoint.X - arcSize.Width, arcPoint.Y - (arcSize.Height / 2), arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 0, 180);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the flow chart direct access storage path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartDirectAccessStoragePath()
        {
            GraphicsPath path = new GraphicsPath();
            SizeF arcSize = new SizeF(m_rectBounds.Width / 3, m_rectBounds.Height);
            PointF arcPoint = GetXYPosition(5, 6, 6);
            RectangleF bounds = new RectangleF(arcPoint.X - (arcSize.Width / 2), arcPoint.Y - arcSize.Height, arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 90, 180);
            path.StartFigure();
            path.AddLine(GetXYPosition(1, 0, 6), GetXYPosition(5, 0, 6));
            arcPoint = GetXYPosition(5, 0, 6);
            bounds = new RectangleF(arcPoint.X - (arcSize.Width / 2), arcPoint.Y, arcSize.Width, arcSize.Height);
            path.AddArc(bounds, 270, 180);
            path.AddLine(new PointF(bounds.X, bounds.Y + arcSize.Height), GetXYPosition(1, 6, 6));
            arcPoint = GetXYPosition(1, 6, 6);
            bounds = new RectangleF(arcPoint.X - (arcSize.Width / 2), arcPoint.Y - arcSize.Height, arcSize.Width, arcSize.Height);
            path.StartFigure();
            path.AddArc(bounds, 90, 180);
            return path;
        }
        /// <summary>
        /// Gets the flow chart display path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetFlowChartDisplayPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(GetXYPosition(0, 3, 6), GetXYPosition(1, 0, 6));
            path.AddLine(GetXYPosition(1, 0, 6), GetXYPosition(5, 0, 6));
            path.AddArc(GetXYPosition(5, 0, 6).X - (m_rectBounds.Width / 6), GetXYPosition(5, 0, 6).Y, m_rectBounds.Width / 3, m_rectBounds.Height, 270, 180);
            path.AddLine(new PointF(GetXYPosition(5, 0, 6).X - (m_rectBounds.Width / 6), GetXYPosition(5, 0, 6).Y + m_rectBounds.Height), GetXYPosition(1, 6, 6));
            path.CloseFigure();
            return path;
        }
        #endregion
        #region Stars and Banners
        internal GraphicsPath GetExplosion1()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Explosion1);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[24];
            linePoints[0] = GetXYPosition(10800, 5800, 21600);
            linePoints[1] = GetXYPosition(14522, 0, 21600);
            linePoints[2] = GetXYPosition(14155, 5325, 21600);
            linePoints[3] = GetXYPosition(18380, 4457, 21600);
            linePoints[4] = GetXYPosition(16702, 7315, 21600);
            linePoints[5] = GetXYPosition(21097, 8137, 21600);
            linePoints[6] = GetXYPosition(17607, 10475, 21600);
            linePoints[7] = GetXYPosition(21600, 13290, 21600);
            linePoints[8] = GetXYPosition(16837, 12942, 21600);
            linePoints[9] = GetXYPosition(18145, 18095, 21600);
            linePoints[10] = GetXYPosition(14020, 14457, 21600);
            linePoints[11] = GetXYPosition(13247, 19737, 21600);
            linePoints[12] = GetXYPosition(10532, 14935, 21600);
            linePoints[13] = GetXYPosition(8485, 21600, 21600);
            linePoints[14] = GetXYPosition(7715, 15627, 21600);
            linePoints[15] = GetXYPosition(4762, 17617, 21600);
            linePoints[16] = GetXYPosition(5667, 13937, 21600);
            linePoints[17] = GetXYPosition(135, 14587, 21600);
            linePoints[18] = GetXYPosition(3722, 11775, 21600);
            linePoints[19] = GetXYPosition(0, 8615, 21600);
            linePoints[20] = GetXYPosition(4627, 7617, 21600);
            linePoints[21] = GetXYPosition(370, 2295, 21600);
            linePoints[22] = GetXYPosition(7312, 6320, 21600);
            linePoints[23] = GetXYPosition(8352, 2295, 21600);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        internal GraphicsPath GetExplosion2()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Explosion2);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[28];
            linePoints[0] = GetXYPosition(11462, 4342, 21600);
            linePoints[1] = GetXYPosition(14790, 0, 21600);
            linePoints[2] = GetXYPosition(14525, 5777, 21600);
            linePoints[3] = GetXYPosition(18007, 3172, 21600);
            linePoints[4] = GetXYPosition(16380, 6532, 21600);
            linePoints[5] = GetXYPosition(21600, 6645, 21600);
            linePoints[6] = GetXYPosition(16985, 9402, 21600);
            linePoints[7] = GetXYPosition(18270, 11290, 21600);
            linePoints[8] = GetXYPosition(16380, 12310, 21600);
            linePoints[9] = GetXYPosition(18877, 15632, 21600);
            linePoints[10] = GetXYPosition(14640, 14350, 21600);
            linePoints[11] = GetXYPosition(14942, 17370, 21600);
            linePoints[12] = GetXYPosition(12180, 15935, 21600);
            linePoints[13] = GetXYPosition(11612, 18842, 21600);
            linePoints[14] = GetXYPosition(9872, 17370, 21600);
            linePoints[15] = GetXYPosition(8700, 19712, 21600);
            linePoints[16] = GetXYPosition(7527, 18125, 21600);
            linePoints[17] = GetXYPosition(4917, 21600, 21600);
            linePoints[18] = GetXYPosition(4805, 18240, 21600);

            linePoints[19] = GetXYPosition(1285, 17825, 21600);
            linePoints[20] = GetXYPosition(3330, 15370, 21600);
            linePoints[21] = GetXYPosition(0, 12877, 21600);
            linePoints[22] = GetXYPosition(3935, 11592, 21600);
            linePoints[23] = GetXYPosition(1172, 8270, 21600);
            linePoints[24] = GetXYPosition(5372, 7817, 21600);
            linePoints[25] = GetXYPosition(4502, 3625, 21600);
            linePoints[26] = GetXYPosition(8550, 6382, 21600);
            linePoints[27] = GetXYPosition(9722, 1887, 21600);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        internal GraphicsPath GetStar4Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star4Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[8];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[2] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[4] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[6] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Bottom);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy2"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        internal GraphicsPath GetStar5Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star5Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[10];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[2] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[7] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy2"]);
            path.AddLines(linePoints);
            path.CloseFigure();

            return path;
        }
        internal GraphicsPath GetStar6Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star6Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[12];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + (m_rectBounds.Height / 4f));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[2] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + (m_rectBounds.Height / 4f));
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[8] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Bottom);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + (m_rectBounds.Height / 2f));
            path.AddLines(linePoints);
            path.CloseFigure();

            return path;
        }
        internal GraphicsPath GetStar7Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star7Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[14];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[4] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[11] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y + formulaValues["sy4"]);

            linePoints[12] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy3"]);
            //path.AddLine(linePoints[0], linePoints[1]);
            //path.AddLine(linePoints[0], linePoints[2]);
            //path.AddLine(linePoints[0], linePoints[3]);
            //path.AddLine(linePoints[3], linePoints[4]);
            //path.AddLine(linePoints[4], linePoints[5]);
            //path.AddLine(linePoints[5], linePoints[6]);
            //path.AddLine(linePoints[6], linePoints[7]);
            //path.AddLine(linePoints[8], linePoints[9]);
            //path.AddLine(linePoints[9], linePoints[0]);
            path.AddLines(linePoints);
            path.CloseFigure();

            return path;
        }
        internal GraphicsPath GetStar8Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star8Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[16];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[4] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[8] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[12] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Bottom);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy3"]);
            path.AddLines(linePoints);
            path.CloseFigure();

            return path;
        }
        internal GraphicsPath GetStar10Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star10Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[20];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[4] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y2"]);

            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[12] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[14] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Bottom);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[16] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[18] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + (m_rectBounds.Height / 2f));
            path.AddLines(linePoints);
            path.CloseFigure();

            return path;
        }
        internal GraphicsPath GetStar12Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star12Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[24];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + (m_rectBounds.Height / 4f));
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[4] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 4f), m_rectBounds.Y + formulaValues["y1"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[6] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + (m_rectBounds.Height / 4f));
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[12] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy5"]);
            linePoints[16] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy6"]);
            linePoints[18] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Bottom);
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy6"]);
            linePoints[20] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 4f), m_rectBounds.Y + formulaValues["y4"]);
            linePoints[21] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy5"]);
            linePoints[22] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[23] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy4"]);
            path.AddLines(linePoints);
            path.CloseFigure();

            return path;
        }
        internal GraphicsPath GetStar16Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star16Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[32];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[8] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[12] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["sx7"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["sx8"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[16] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["sx8"], m_rectBounds.Y + formulaValues["sy5"]);
            linePoints[18] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["sx7"], m_rectBounds.Y + formulaValues["sy6"]);
            linePoints[20] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[21] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + formulaValues["sy7"]);
            linePoints[22] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[23] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy8"]);
            linePoints[24] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Bottom);
            linePoints[25] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy8"]);
            linePoints[26] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[27] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy7"]);
            linePoints[28] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[29] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy6"]);
            linePoints[30] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[31] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy5"]);
            path.AddLines(linePoints);
            path.CloseFigure();

            return path;
        }
        internal GraphicsPath GetStar24Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star24Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[48];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy6"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy5"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);


            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + formulaValues["sy1"]);

            linePoints[12] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["sx7"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["sx8"], m_rectBounds.Y + formulaValues["sy2"]);

            linePoints[16] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["sx9"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[18] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["sx10"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[20] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[21] = new PointF(m_rectBounds.X + formulaValues["sx11"], m_rectBounds.Y + formulaValues["sy5"]);
            linePoints[22] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[23] = new PointF(m_rectBounds.X + formulaValues["sx12"], m_rectBounds.Y + formulaValues["sy6"]);


            linePoints[24] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2f));

            linePoints[25] = new PointF(m_rectBounds.X + formulaValues["sx12"], m_rectBounds.Y + formulaValues["sy7"]);
            linePoints[26] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[27] = new PointF(m_rectBounds.X + formulaValues["sx11"], m_rectBounds.Y + formulaValues["sy8"]);
            linePoints[28] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints[29] = new PointF(m_rectBounds.X + formulaValues["sx10"], m_rectBounds.Y + formulaValues["sy9"]);
            linePoints[30] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y8"]);
            linePoints[31] = new PointF(m_rectBounds.X + formulaValues["sx9"], m_rectBounds.Y + formulaValues["sy10"]);

            linePoints[32] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y9"]);
            linePoints[33] = new PointF(m_rectBounds.X + formulaValues["sx8"], m_rectBounds.Y + formulaValues["sy11"]);
            linePoints[34] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y10"]);

            linePoints[35] = new PointF(m_rectBounds.X + formulaValues["sx7"], m_rectBounds.Y + formulaValues["sy12"]);
            linePoints[36] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Bottom);

            linePoints[37] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + formulaValues["sy12"]);
            linePoints[38] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y10"]);
            linePoints[39] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy11"]);
            linePoints[40] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y9"]);
            linePoints[41] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy10"]);
            linePoints[42] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y8"]);
            linePoints[43] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy9"]);
            linePoints[44] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints[45] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy8"]);
            linePoints[46] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[47] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy7"]);

            //path.AddLine(linePoints[0], linePoints[1]);
            //path.AddLine(linePoints[0], linePoints[2]);
            //path.AddLine(linePoints[0], linePoints[3]);
            ////path.AddLine(linePoints[3], linePoints[4]);
            //path.AddLine(linePoints[4], linePoints[5]);
            //path.AddLine(linePoints[5], linePoints[6]);
            //path.AddLine(linePoints[6], linePoints[7]);
            //path.AddLine(linePoints[8], linePoints[9]);
            //path.AddLine(linePoints[9], linePoints[0]);
            path.AddLines(linePoints);
            path.CloseFigure();

            return path;
        }
        internal GraphicsPath GetStar32Point()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Star32Point);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[64];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy8"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy7"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy6"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy5"]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[12] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["sx7"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["sx8"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[16] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Y);
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["sx9"], m_rectBounds.Y + formulaValues["sy1"]);
            linePoints[18] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["sx10"], m_rectBounds.Y + formulaValues["sy2"]);
            linePoints[20] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[21] = new PointF(m_rectBounds.X + formulaValues["sx11"], m_rectBounds.Y + formulaValues["sy3"]);
            linePoints[22] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[23] = new PointF(m_rectBounds.X + formulaValues["sx12"], m_rectBounds.Y + formulaValues["sy4"]);
            linePoints[24] = new PointF(m_rectBounds.X + formulaValues["x11"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[25] = new PointF(m_rectBounds.X + formulaValues["sx13"], m_rectBounds.Y + formulaValues["sy5"]);
            linePoints[26] = new PointF(m_rectBounds.X + formulaValues["x12"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[27] = new PointF(m_rectBounds.X + formulaValues["sx14"], m_rectBounds.Y + formulaValues["sy6"]);
            linePoints[28] = new PointF(m_rectBounds.X + formulaValues["x13"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[29] = new PointF(m_rectBounds.X + formulaValues["sx15"], m_rectBounds.Y + formulaValues["sy7"]);
            linePoints[30] = new PointF(m_rectBounds.X + formulaValues["x14"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints[31] = new PointF(m_rectBounds.X + formulaValues["sx16"], m_rectBounds.Y + formulaValues["sy8"]);
            linePoints[32] = new PointF(m_rectBounds.Right, m_rectBounds.Y + (m_rectBounds.Height / 2f));
            linePoints[33] = new PointF(m_rectBounds.X + formulaValues["sx16"], m_rectBounds.Y + formulaValues["sy9"]);
            linePoints[34] = new PointF(m_rectBounds.X + formulaValues["x14"], m_rectBounds.Y + formulaValues["y8"]);
            linePoints[35] = new PointF(m_rectBounds.X + formulaValues["sx15"], m_rectBounds.Y + formulaValues["sy10"]);
            linePoints[36] = new PointF(m_rectBounds.X + formulaValues["x13"], m_rectBounds.Y + formulaValues["y9"]);
            linePoints[37] = new PointF(m_rectBounds.X + formulaValues["sx14"], m_rectBounds.Y + formulaValues["sy11"]);
            linePoints[38] = new PointF(m_rectBounds.X + formulaValues["x12"], m_rectBounds.Y + formulaValues["y10"]);
            linePoints[39] = new PointF(m_rectBounds.X + formulaValues["sx13"], m_rectBounds.Y + formulaValues["sy12"]);
            linePoints[40] = new PointF(m_rectBounds.X + formulaValues["x11"], m_rectBounds.Y + formulaValues["y11"]);
            linePoints[41] = new PointF(m_rectBounds.X + formulaValues["sx12"], m_rectBounds.Y + formulaValues["sy13"]);
            linePoints[42] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y12"]);
            linePoints[43] = new PointF(m_rectBounds.X + formulaValues["sx11"], m_rectBounds.Y + formulaValues["sy14"]);
            linePoints[44] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y13"]);
            linePoints[45] = new PointF(m_rectBounds.X + formulaValues["sx10"], m_rectBounds.Y + formulaValues["sy15"]);
            linePoints[46] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y14"]);
            linePoints[47] = new PointF(m_rectBounds.X + formulaValues["sx9"], m_rectBounds.Y + formulaValues["sy16"]);
            linePoints[48] = new PointF(m_rectBounds.X + (m_rectBounds.Width / 2f), m_rectBounds.Bottom);
            linePoints[49] = new PointF(m_rectBounds.X + formulaValues["sx8"], m_rectBounds.Y + formulaValues["sy16"]);
            linePoints[50] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y14"]);
            linePoints[51] = new PointF(m_rectBounds.X + formulaValues["sx7"], m_rectBounds.Y + formulaValues["sy15"]);
            linePoints[52] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y13"]);
            linePoints[53] = new PointF(m_rectBounds.X + formulaValues["sx6"], m_rectBounds.Y + formulaValues["sy14"]);
            linePoints[54] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y12"]);
            linePoints[55] = new PointF(m_rectBounds.X + formulaValues["sx5"], m_rectBounds.Y + formulaValues["sy13"]);
            linePoints[56] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y11"]);
            linePoints[57] = new PointF(m_rectBounds.X + formulaValues["sx4"], m_rectBounds.Y + formulaValues["sy12"]);
            linePoints[58] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y10"]);
            linePoints[59] = new PointF(m_rectBounds.X + formulaValues["sx3"], m_rectBounds.Y + formulaValues["sy11"]);
            linePoints[60] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y9"]);
            linePoints[61] = new PointF(m_rectBounds.X + formulaValues["sx2"], m_rectBounds.Y + formulaValues["sy10"]);
            linePoints[62] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y8"]);
            linePoints[63] = new PointF(m_rectBounds.X + formulaValues["sx1"], m_rectBounds.Y + formulaValues["sy9"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        internal GraphicsPath GetUpRibbon()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.UpRibbon);
            GraphicsPath path = new GraphicsPath();
            //path[0] = new GraphicsPath();
            //path[1] = new GraphicsPath();
            PointF[] linePoints = new PointF[25];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X + m_rectBounds.Width / 8, m_rectBounds.Y + formulaValues["y3"]);
            path.AddLine(linePoints[0], linePoints[1]);
            linePoints[2] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[1], linePoints[2]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[2], linePoints[3]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["hR"]);
            path.AddLine(linePoints[3], linePoints[4]);
            path.AddArc(linePoints[4].X, linePoints[4].Y - formulaValues["hR"], (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 180, 90);
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y);
            path.AddLine(new PointF(linePoints[4].X + (m_rectBounds.Width / 32), linePoints[4].Y - formulaValues["hR"]), new PointF(linePoints[5].X - (m_rectBounds.Width / 32), linePoints[5].Y));
            path.AddArc(linePoints[5].X - (m_rectBounds.Width / 16), linePoints[5].Y, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 270, 90);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(new PointF(linePoints[5].X, linePoints[5].Y + formulaValues["hR"]), linePoints[6]);
            linePoints[7] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[6], linePoints[7]);
            linePoints[8] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[7], linePoints[8]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLine(linePoints[8], linePoints[9]);
            linePoints[10] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            path.AddLine(linePoints[9], linePoints[10]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Bottom);
            path.AddLine(linePoints[10], new PointF(linePoints[11].X + (m_rectBounds.Width / 32), linePoints[11].Y));
            path.AddArc(linePoints[11].X, linePoints[11].Y - formulaValues["hR"] * 2, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 90, 180);
            linePoints[12] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddLine(new PointF(linePoints[11].X + (m_rectBounds.Width / 32), linePoints[11].Y - formulaValues["hR"] * 2), new PointF(linePoints[12].X - (m_rectBounds.Width / 32), linePoints[12].Y));
            path.AddArc(linePoints[12].X - ((m_rectBounds.Width / 32) * 2), linePoints[12].Y - formulaValues["hR"] * 2, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 90, -180);
            linePoints[13] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLine(new PointF(linePoints[12].X - ((m_rectBounds.Width / 32) * 2), linePoints[12].Y - formulaValues["hR"] * 2), new PointF(linePoints[13].X + (m_rectBounds.Width / 32), linePoints[13].Y));
            path.AddArc(linePoints[13].X, linePoints[13].Y, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 270, -180);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddLine(new PointF(linePoints[13].X + (m_rectBounds.Width / 32), linePoints[13].Y + formulaValues["hR"] * 2), new PointF(linePoints[14].X - (m_rectBounds.Width / 32), linePoints[14].Y));
            path.AddArc(linePoints[14].X - (m_rectBounds.Width / 32) * 2, linePoints[14].Y, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 270, 180);
            path.CloseFigure();
            path.StartFigure();
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[16] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y6"]);
            path.AddLine(linePoints[15], linePoints[16]);
            path.StartFigure();
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[18] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLine(linePoints[17], linePoints[18]);
            path.StartFigure();
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints[20] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[19], linePoints[20]);
            path.StartFigure();
            linePoints[21] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[22] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y7"]);
            path.AddLine(linePoints[21], linePoints[22]);
            return path;
        }
        internal GraphicsPath GetDownRibbon()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.DownRibbon);
            GraphicsPath path = new GraphicsPath();

            PointF[] linePoints = new PointF[23];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y);
            path.AddLine(linePoints[0], new PointF(linePoints[1].X - (m_rectBounds.Width / 32), linePoints[1].Y));
            path.AddArc(linePoints[1].X - (m_rectBounds.Width / 32) * 2, linePoints[1].Y, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 270, 180);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddLine(new PointF(linePoints[1].X - (m_rectBounds.Width / 32), linePoints[1].Y + formulaValues["hR"] * 2), new PointF(linePoints[2].X + (m_rectBounds.Width / 32), linePoints[2].Y));
            path.AddArc(linePoints[2].X, linePoints[2].Y, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 270, -180);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLine(new PointF(linePoints[2].X + (m_rectBounds.Width / 32), linePoints[2].Y + formulaValues["hR"] * 2), new PointF(linePoints[3].X - (m_rectBounds.Width / 32), linePoints[3].Y));
            path.AddArc(linePoints[3].X - (m_rectBounds.Width / 32) * 2, linePoints[3].Y - formulaValues["hR"] * 2, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 90, -180);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddLine(new PointF(linePoints[3].X - (m_rectBounds.Width / 32), linePoints[3].Y - formulaValues["hR"] * 2), new PointF(linePoints[4].X + (m_rectBounds.Width / 32), linePoints[4].Y));
            path.AddArc(linePoints[4].X, linePoints[4].Y - formulaValues["hR"] * 2, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 90, 180);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            path.AddLine(new PointF(linePoints[4].X + (m_rectBounds.Width / 32), linePoints[4].Y - formulaValues["hR"] * 2), linePoints[5]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLine(linePoints[5], linePoints[6]);
            linePoints[7] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[6], linePoints[7]);
            linePoints[8] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[7], linePoints[8]);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y5"]);
            path.AddLine(linePoints[8], linePoints[9]);
            path.AddArc(linePoints[9].X - (m_rectBounds.Width / 32) * 2, linePoints[9].Y - formulaValues["hR"], (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 0, 90);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Bottom);
            path.AddLine(new PointF(linePoints[9].X - (m_rectBounds.Width / 32), linePoints[9].Y + formulaValues["hR"]), new PointF(linePoints[10].X + (m_rectBounds.Width / 32), linePoints[10].Y));
            path.AddArc(linePoints[10].X, linePoints[10].Y - formulaValues["hR"] * 2, (m_rectBounds.Width / 32) * 2, formulaValues["hR"] * 2, 90, 90);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(new PointF(linePoints[10].X, linePoints[10].Y - formulaValues["hR"]), linePoints[11]);
            linePoints[12] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[11], linePoints[12]);
            linePoints[13] = new PointF(m_rectBounds.X + m_rectBounds.Width / 8, m_rectBounds.Y + formulaValues["y3"]);
            path.AddLine(linePoints[12], linePoints[13]);
            path.CloseFigure();
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["hR"]);
            linePoints[15] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLine(linePoints[14], linePoints[15]);
            path.StartFigure();
            linePoints[16] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[17] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["hR"]);
            path.AddLine(linePoints[16], linePoints[17]);
            path.StartFigure();
            linePoints[18] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[19] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y6"]);
            path.AddLine(linePoints[18], linePoints[19]);
            path.StartFigure();
            linePoints[21] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[22] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[21], linePoints[22]);
            return path;
        }
        internal GraphicsPath GetCurvedUpRibbon()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.CurvedUpRibbon);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[5];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X + m_rectBounds.Width / 8, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["q1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["cx4"], m_rectBounds.Y + formulaValues["cy4"]);
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y5"]);
            PointF[] linePoints5 = new PointF[2];
            linePoints5[0] = linePoints[0];
            linePoints5[1] = linePoints[1];
            path.AddLines(linePoints5);
            path.AddBezier(linePoints[2], linePoints[2], linePoints[3], linePoints[4]);
            PointF[] linePoints1 = new PointF[3];
            linePoints1[0] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints1[1] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y + formulaValues["cy6"]);
            linePoints1[2] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y6"]);
            path.AddBezier(linePoints1[0], linePoints1[0], linePoints1[1], linePoints1[2]);
            PointF[] linePoints2 = new PointF[3];
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["cx5"], m_rectBounds.Y + formulaValues["cy4"]);
            linePoints2[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["q1"]);
            path.AddBezier(linePoints2[0], linePoints2[0], linePoints2[1], linePoints2[2]);
            PointF[] linePoints3 = new PointF[4];
            linePoints3[0] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints3[1] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints3[2] = new PointF(m_rectBounds.X + formulaValues["cx2"], m_rectBounds.Y + formulaValues["cy1"]);
            linePoints3[3] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            PointF[] linePoints8 = new PointF[1];
            linePoints8[0] = linePoints3[0];
            path.AddLines(linePoints8);
            path.AddBezier(linePoints3[1], linePoints3[1], linePoints3[2], linePoints3[3]);
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints2[1] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y + formulaValues["cy3"]);
            linePoints2[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddBezier(linePoints2[0], linePoints2[0], linePoints2[1], linePoints2[2]);
            PointF[] linePoints9 = new PointF[3];
            linePoints9[0] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints9[1] = new PointF(m_rectBounds.X + formulaValues["cx1"], m_rectBounds.Y + formulaValues["cy1"]);
            linePoints9[2] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddBezier(linePoints9[0], linePoints9[0], linePoints9[1], linePoints9[2]);
            path.CloseFigure();
            PointF[] linePoints4 = new PointF[2];
            linePoints4[0] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints4[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y5"]);
            path.AddLines(linePoints4);
            path.CloseFigure();
            linePoints4[0] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints4[1] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLines(linePoints4);
            path.CloseFigure();
            linePoints4[0] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints4[1] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddLines(linePoints4);
            path.CloseFigure();
            linePoints4[0] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints4[1] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y7"]);
            path.AddLines(linePoints4);
            path.CloseFigure();
            return path;
        }
        internal GraphicsPath GetCurvedDownRibbon()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.CurvedDownRibbon);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[3];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["cx1"], m_rectBounds.Y + formulaValues["cy1"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddBezier(linePoints[0], linePoints[0], linePoints[1], linePoints[2]);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[1] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y + formulaValues["cy3"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddBezier(linePoints[0], linePoints[0], linePoints[1], linePoints[2]);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["cx2"], m_rectBounds.Y + formulaValues["cy1"]);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            path.AddBezier(linePoints[0], linePoints[0], linePoints[1], linePoints[2]);
            PointF[] linePoints1 = new PointF[4];
            linePoints1[0] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints1[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["rh"]);
            linePoints1[2] = new PointF(m_rectBounds.X + formulaValues["cx5"], m_rectBounds.Y + formulaValues["cy4"]);
            linePoints1[3] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y5"]);
            PointF[] linePoints3 = new PointF[1];
            linePoints3[0] = linePoints1[0];
            path.AddLines(linePoints3);
            path.AddBezier(linePoints1[1], linePoints1[1], linePoints1[2], linePoints1[3]);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[1] = new PointF(m_rectBounds.X + m_rectBounds.Width / 2, m_rectBounds.Y + formulaValues["cy6"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y6"]);
            path.AddBezier(linePoints[0], linePoints[0], linePoints[1], linePoints[2]);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["cx4"], m_rectBounds.Y + formulaValues["cy4"]);
            linePoints[2] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["rh"]);
            path.AddBezier(linePoints[0], linePoints[0], linePoints[1], linePoints[2]);
            linePoints[0] = new PointF(m_rectBounds.X + m_rectBounds.Width / 8, m_rectBounds.Y + formulaValues["y2"]);
            linePoints3[0] = linePoints[0];
            path.AddLines(linePoints3);
            path.CloseFigure();
            PointF[] linePoints2 = new PointF[2];
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLines(linePoints2);
            path.CloseFigure();
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y5"]);
            path.AddLines(linePoints2);
            path.CloseFigure();
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y7"]);
            path.AddLines(linePoints2);
            path.CloseFigure();
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y7"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddLines(linePoints2);
            path.CloseFigure();
            return path;
        }
        internal GraphicsPath GetVerticalScroll()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.VerticalScroll);
            GraphicsPath path = new GraphicsPath();

            PointF[] linePoints = new PointF[2];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["ch"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["ch"], m_rectBounds.Y);
            PointF[] linePoints1 = new PointF[1];
            linePoints1[0] = linePoints[0];
            path.AddLines(linePoints1);
            path.AddArc(linePoints[1].X, linePoints[1].Y, formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 180, 90);
            PointF[] linePoints2 = new PointF[1];
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y);
            path.AddArc(linePoints2[0].X - formulaValues["ch2"], linePoints2[0].Y, formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 270, 180);
            PointF[] linePoints3 = new PointF[2];
            linePoints3[0] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["ch"]);
            linePoints3[1] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y4"]);
            PointF[] temp = new PointF[1];
            temp[0] = linePoints3[0];
            path.AddLines(temp);
            path.AddArc(linePoints3[1].X - formulaValues["ch2"] * 2, linePoints3[1].Y - formulaValues["ch2"], formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 0, 90);
            PointF[] linePoints4 = new PointF[1];
            linePoints4[0] = new PointF(m_rectBounds.X + formulaValues["ch2"], m_rectBounds.Bottom);
            path.AddArc(linePoints4[0].X - formulaValues["ch2"], linePoints4[0].Y - formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 90, 180);
            path.CloseFigure();
            PointF[] linePoints5 = new PointF[1];
            linePoints5[0] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y);
            path.StartFigure();
            path.AddArc(linePoints5[0].X - formulaValues["ch2"], linePoints5[0].Y, formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 270, 180);
            path.StartFigure();
            path.AddArc(linePoints5[0].X - formulaValues["ch2"] / 2, linePoints5[0].Y + formulaValues["ch2"], formulaValues["ch4"] * 2, formulaValues["ch4"] * 2, 90, 180);
            PointF[] linePoints6 = new PointF[1];
            linePoints6[0] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["ch2"]);
            path.AddLines(linePoints6);
            //path.CloseFigure();
            PointF[] linePoints7 = new PointF[2];
            path.StartFigure();
            linePoints7[0] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["ch"]);
            linePoints7[1] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["ch"]);
            path.AddLines(linePoints7);
            path.CloseFigure();
            PointF[] linePoints8 = new PointF[1];
            linePoints8[0] = new PointF(m_rectBounds.X + formulaValues["ch2"], m_rectBounds.Y + formulaValues["y3"]);
            path.StartFigure();
            path.AddArc(linePoints8[0].X - formulaValues["ch2"] / 2, linePoints8[0].Y, formulaValues["ch4"] * 2, formulaValues["ch4"] * 2, 270, 180);
            PointF[] linePoints9 = new PointF[1];
            linePoints9[0] = new PointF(m_rectBounds.X + formulaValues["ch"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLines(linePoints9);
            // path.CloseFigure();
            PointF[] linePoints10 = new PointF[1];
            linePoints10[0] = new PointF(m_rectBounds.X + formulaValues["ch2"], m_rectBounds.Bottom);
            path.StartFigure();
            path.AddArc((linePoints10[0].X - formulaValues["ch2"]), linePoints10[0].Y - formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 90, -90);
            PointF[] linePoints11 = new PointF[1];
            linePoints11[0] = new PointF(m_rectBounds.X + formulaValues["ch"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLines(linePoints11);
            return path;
        }
        internal GraphicsPath[] GetHorizontalScroll()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.HorizontalScroll);
            GraphicsPath[] path = new GraphicsPath[7];
            path[0] = new GraphicsPath();
            path[1] = new GraphicsPath();
            path[2] = new GraphicsPath();
            path[3] = new GraphicsPath();
            path[4] = new GraphicsPath();
            path[5] = new GraphicsPath();
            path[6] = new GraphicsPath();
            PointF[] linePoints1 = new PointF[1];
            linePoints1[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y3"]);
            path[0].AddArc(linePoints1[0].X, linePoints1[0].Y - formulaValues["ch2"], formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 180, 90);
            PointF[] linePoints2 = new PointF[2];
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["ch"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["ch2"]);
            PointF[] temp = new PointF[1];
            temp[0] = linePoints2[0];
            path[0].AddLines(temp);
            path[0].AddArc(linePoints2[1].X, linePoints2[1].Y - formulaValues["ch2"], formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 180, 180);
           
            linePoints1[0] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y5"]);
            path[0].AddArc(linePoints1[0].X - formulaValues["ch2"] * 2f, linePoints1[0].Y - formulaValues["ch2"], formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 0, 90);

            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["ch"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["ch"], m_rectBounds.Y + formulaValues["y7"]);

            temp[0] = linePoints2[0];
            path[0].AddLines(temp);

            path[0].AddArc(linePoints2[0].X - formulaValues["ch2"] * 2, linePoints2[0].Y, formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 0, 180);
            path[0].CloseFigure();

            //Outline part finish.
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x3"] + formulaValues["ch2"], m_rectBounds.Y + formulaValues["ch"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["ch"]);
            PointF[] temp1 = new PointF[2];
            temp1[0] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["ch"]);
            temp1[1] = linePoints2[0];
            path[1].AddLines(temp1);
            path[2].AddArc(linePoints2[1].X - formulaValues["ch2"], linePoints2[1].Y - formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 90, -90);
            path[1].CloseFigure();
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["ch"] - formulaValues["ch2"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["ch2"]);
            temp1[0] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["ch"]);
            temp1[1] = linePoints2[0];
            path[3].AddLines(temp1);
            path[3].AddArc(linePoints2[1].X - formulaValues["ch2"], linePoints2[1].Y - formulaValues["ch2"] / 2, formulaValues["ch4"] * 2, formulaValues["ch4"] * 2, 0, 180);
            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["ch2"] * 2, m_rectBounds.Y - formulaValues["ch2"] + formulaValues["y4"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["ch2"], m_rectBounds.Y + formulaValues["y3"]);
            temp1[0] = new PointF(m_rectBounds.X + formulaValues["ch"], m_rectBounds.Y + formulaValues["y6"]);
            temp1[1] = linePoints2[0];
            path[4].AddLines(temp1);
            path[5].AddArc(linePoints2[1].X, linePoints2[1].Y - formulaValues["ch2"] / 2, formulaValues["ch4"] * 2, formulaValues["ch4"] * 2, 180, 180);

            path[5].AddArc(linePoints2[1].X - formulaValues["ch2"], linePoints2[1].Y - formulaValues["ch2"], formulaValues["ch2"] * 2, formulaValues["ch2"] * 2, 0, 180);

            linePoints2[0] = new PointF(m_rectBounds.X + formulaValues["ch"] - formulaValues["ch2"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints2[1] = new PointF(m_rectBounds.X + formulaValues["ch"] - formulaValues["ch2"], m_rectBounds.Y + formulaValues["y3"] + formulaValues["ch2"]);
            path[6].AddLines(linePoints2);
            return path;
        }
        internal GraphicsPath GetWave()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.Wave);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddBezier(linePoints[0], linePoints[1], linePoints[2], linePoints[3]);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddBezier(linePoints[0], linePoints[1], linePoints[2], linePoints[3]);
            path.CloseFigure();
            return path;
        }
        internal GraphicsPath GetDoubleWave()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.DoubleWave);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x5"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddBezier(linePoints[0], linePoints[1], linePoints[2], linePoints[3]);
            PointF[] linePoints1 = new PointF[4];
            linePoints1[1] = new PointF(m_rectBounds.X + formulaValues["x6"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints1[2] = new PointF(m_rectBounds.X + formulaValues["x7"], m_rectBounds.Y + formulaValues["y3"]);
            linePoints1[3] = new PointF(m_rectBounds.X + formulaValues["x8"], m_rectBounds.Y + formulaValues["y1"]);
            path.AddBezier(linePoints[3], linePoints1[1], linePoints1[2], linePoints1[3]);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x15"], m_rectBounds.Y + formulaValues["y4"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x14"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x13"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x12"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddBezier(linePoints[0], linePoints[1], linePoints[2], linePoints[3]);
            linePoints1[1] = new PointF(m_rectBounds.X + formulaValues["x11"], m_rectBounds.Y + formulaValues["y6"]);
            linePoints1[2] = new PointF(m_rectBounds.X + formulaValues["x10"], m_rectBounds.Y + formulaValues["y5"]);
            linePoints1[3] = new PointF(m_rectBounds.X + formulaValues["x9"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddBezier(linePoints[3], linePoints1[1], linePoints1[2], linePoints1[3]);
            path.CloseFigure();
            return path;
        }
        #endregion
        #region Callouts
        /// <summary>
        /// Gets the rectangular callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetRectangularCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RectangularCallout);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[16];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["xt"], m_rectBounds.Y + formulaValues["yt"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[4] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[5] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[6] = new PointF(m_rectBounds.X + formulaValues["xr"], m_rectBounds.Y + formulaValues["yr"]);
            linePoints[7] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[8] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[9] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[10] = new PointF(m_rectBounds.X + formulaValues["xb"], m_rectBounds.Y + formulaValues["yb"]);
            linePoints[11] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[12] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            linePoints[13] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[14] = new PointF(m_rectBounds.X + formulaValues["xl"], m_rectBounds.Y + formulaValues["yl"]);
            linePoints[15] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y1"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the rounded rectangular callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetRoundedRectangularCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.RoundedRectangularCallout);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, formulaValues["u1"] * 2, formulaValues["u1"] * 2, 180, 90);
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["xt"], m_rectBounds.Y + formulaValues["yt"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["u2"], m_rectBounds.Y);
            path.AddLines(linePoints);
            path.AddArc(linePoints[3].X - formulaValues["u1"], linePoints[3].Y, formulaValues["u1"] * 2, formulaValues["u1"] * 2, 270, 90);
            linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["xr"], m_rectBounds.Y + formulaValues["yr"]);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[3] = new PointF(m_rectBounds.Right, m_rectBounds.Y + formulaValues["v2"]);
            path.AddLines(linePoints);
            path.AddArc(linePoints[3].X - formulaValues["u1"] * 2, linePoints[3].Y - formulaValues["u1"], formulaValues["u1"] * 2, formulaValues["u1"] * 2, 0, 90);
            linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Bottom);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["xb"], m_rectBounds.Y + formulaValues["yb"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["u1"], m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.AddArc(linePoints[3].X - formulaValues["u1"], linePoints[3].Y - formulaValues["u1"] * 2, formulaValues["u1"] * 2, formulaValues["u1"] * 2, 90, 90);
            linePoints = new PointF[3];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y2"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["xl"], m_rectBounds.Y + formulaValues["yl"]);
            linePoints[2] = new PointF(m_rectBounds.X, m_rectBounds.Y + formulaValues["y1"]);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the oval callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetOvalCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.OvalCallout);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[1];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["xPos"], m_rectBounds.Y + formulaValues["yPos"]);
            float startAng = formulaValues["stAng1"];
            float sweepAngle = formulaValues["swAng"];
            if ((startAng < 180 && linePoints[0].X < m_rectBounds.X + m_rectBounds.Width / 2)
                || (startAng < 0 && linePoints[0].Y > m_rectBounds.Y))
                startAng += 180;
            if (sweepAngle < 180)
                sweepAngle += 180;
            path.AddArc(m_rectBounds.X, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height, startAng, sweepAngle);
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the cloud callout path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetCloudCalloutPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.CloudCallout);
            GraphicsPath path = new GraphicsPath();
            PointF arcLocation = GetXYPosition(3900, 14370, 43200);
            arcLocation.X += formulaValues["g27"];
            arcLocation.Y -= formulaValues["g30"];
            SizeF arcSize1 = new SizeF((m_rectBounds.Width * 6753 / 43200) * 2, (m_rectBounds.Height * 9190 / 43200) * 2);
            SizeF arcSize2 = new SizeF((m_rectBounds.Width * 5333 / 43200) * 2, (m_rectBounds.Height * 7267 / 43200) * 2);
            SizeF arcSize3 = new SizeF((m_rectBounds.Width * 4365 / 43200) * 2, (m_rectBounds.Height * 5945 / 43200) * 2);
            SizeF arcSize4 = new SizeF((m_rectBounds.Width * 4857 / 43200) * 2, (m_rectBounds.Height * 6595 / 43200) * 2);
            SizeF arcSize5 = new SizeF((m_rectBounds.Width * 5333 / 43200) * 2, (m_rectBounds.Height * 7273 / 43200) * 2);
            SizeF arcSize6 = new SizeF((m_rectBounds.Width * 6775 / 43200) * 2, (m_rectBounds.Height * 9220 / 43200) * 2);
            SizeF arcSize7 = new SizeF((m_rectBounds.Width * 5785 / 43200) * 2, (m_rectBounds.Height * 7867 / 43200) * 2);
            SizeF arcSize8 = new SizeF((m_rectBounds.Width * 6752 / 43200) * 2, (m_rectBounds.Height * 9215 / 43200) * 2);
            SizeF arcSize9 = new SizeF((m_rectBounds.Width * 7720 / 43200) * 2, (m_rectBounds.Height * 10543 / 43200) * 2);
            SizeF arcSize10 = new SizeF((m_rectBounds.Width * 4360 / 43200) * 2, (m_rectBounds.Height * 5918 / 43200) * 2);
            SizeF arcSize11 = new SizeF((m_rectBounds.Width * 4345 / 43200) * 2, (m_rectBounds.Height * 5945 / 43200) * 2);

            path.AddArc(m_rectBounds.X + (4076 * m_rectBounds.Width / 43200), m_rectBounds.Y + (3912 * m_rectBounds.Height / 43200), arcSize1.Width, arcSize1.Height, -11429249 / 60000, 7426832 / 60000);

            path.AddArc(m_rectBounds.X + (13469 * m_rectBounds.Width / 43200), m_rectBounds.Y + (1304 * m_rectBounds.Height / 43200), arcSize2.Width, arcSize2.Height, -8646143 / 60000, 5396714 / 60000);

            path.AddArc(m_rectBounds.X + m_rectBounds.Width / 2 + (531 * m_rectBounds.Width / 43200), m_rectBounds.Y + 1, arcSize3.Width, arcSize3.Height, -8748475 / 60000, 5983381 / 60000);

            path.AddArc(arcLocation.X + m_rectBounds.Width / 2 + (3013 * m_rectBounds.Width / 43200), m_rectBounds.Y + 1, arcSize4.Width, arcSize4.Height, -7859164 / 60000, 7034504 / 60000);

            path.AddArc(m_rectBounds.Right - arcSize5.Width - (708 * m_rectBounds.Width / 43200), m_rectBounds.Y + arcSize4.Height / 2 - (1127 * m_rectBounds.Height / 43200), arcSize5.Width, arcSize5.Height, -4722533 / 60000, 6541615 / 60000);

            path.AddArc(m_rectBounds.Right - arcSize6.Width + (354 * m_rectBounds.Width / 43200), m_rectBounds.Y + m_rectBounds.Height / 2 - (9129 * m_rectBounds.Height / 43200), arcSize6.Width, arcSize6.Height, -2776035 / 60000, 7816140 / 60000);

            path.AddArc(m_rectBounds.X + m_rectBounds.Width / 2 + (4608 * m_rectBounds.Width / 43200), m_rectBounds.Y + m_rectBounds.Height / 2 + (869 * m_rectBounds.Height / 43200), arcSize7.Width, arcSize7.Height, 37501 / 60000, 6842000 / 60000);

            path.AddArc(m_rectBounds.X + m_rectBounds.Width / 2 - arcSize8.Width / 2 + (886 * m_rectBounds.Width / 43200), m_rectBounds.Bottom - arcSize8.Height, arcSize8.Width, arcSize8.Height, 1347096 / 60000, 6910353 / 60000);

            path.AddArc(m_rectBounds.X + (4962 * m_rectBounds.Width / 43200), m_rectBounds.Bottom - arcSize9.Height - (2173 * m_rectBounds.Height / 43200), arcSize9.Width, arcSize9.Height, 3974558 / 60000, 4542661 / 60000);

            path.AddArc(m_rectBounds.X + (1063 * m_rectBounds.Width / 43200), m_rectBounds.Y + m_rectBounds.Height / 2 + (2608 * m_rectBounds.Height / 43200), arcSize10.Width, arcSize10.Height, -16496525 / 60000, 8804134 / 60000);

            path.AddArc(m_rectBounds.X + 1, m_rectBounds.Y + m_rectBounds.Height / 2 - arcSize11.Height / 2 - (1304 * m_rectBounds.Height / 43200), arcSize11.Width, arcSize11.Height, -14809710 / 60000, 9151131 / 60000);
            path.CloseFigure();
            path.AddArc(m_rectBounds.X + m_rectBounds.Width / 2 + (2658 * m_rectBounds.Width / 43200), m_rectBounds.Y + m_rectBounds.Height / 2, (m_rectBounds.Width * 6753 / 43200) * 2, (m_rectBounds.Height * 9190 / 43200) * 2, -824660 / 60000 - 45, 891534 / 60000 + 45);
            return path;
            return path;
        }
        /// <summary>
        /// Gets the line callout1 path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout1Path()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout1);
            GraphicsPath path = new GraphicsPath();
            float xdiff = (formulaValues["x1"] < formulaValues["x2"]) ? ((formulaValues["x1"] < 0) ? formulaValues["x1"] : 0) : ((formulaValues["x2"] < 0) ? formulaValues["x2"] : 0);
            RectangleF newBounds = m_rectBounds;
            if (xdiff < 0)
                newBounds = new RectangleF(m_rectBounds.X - xdiff, m_rectBounds.Y, m_rectBounds.Width, m_rectBounds.Height);

            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(newBounds.X, newBounds.Y);
            linePoints[1] = new PointF(newBounds.Right, newBounds.Y);
            linePoints[2] = new PointF(newBounds.Right, newBounds.Bottom);
            linePoints[3] = new PointF(newBounds.X, newBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[2];
            linePoints[0] = new PointF(newBounds.X + formulaValues["x1"], newBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(newBounds.X + formulaValues["x2"], newBounds.Y + formulaValues["y2"]);
            path.AddLine(linePoints[0], linePoints[1]);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the line callout2 path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout2Path()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout2);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            path.StartFigure();
            linePoints = new PointF[3];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLine(linePoints[0], linePoints[1]);
            path.StartFigure();
            path.AddLine(linePoints[1], linePoints[2]);
            return path;
        }
        /// <summary>
        /// Gets the line callout3 path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout3Path()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout3);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            path.StartFigure();
            path.AddLine(linePoints[0], linePoints[1]);
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            path.StartFigure();
            path.AddLine(linePoints[1], linePoints[2]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y4"]);
            path.StartFigure();
            path.AddLine(linePoints[2], linePoints[3]);
            return path;
        }
        /// <summary>
        /// Gets the line callout1 accent bar path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout1AccentBarPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout1AccentBar);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            path.AddLine(linePoints[0], linePoints[1]);
            path.StartFigure();
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLine(linePoints[2], linePoints[3]);
            return path;
        }
        /// <summary>
        /// Gets the line callout2 accent bar path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout2AccentBarPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout2AccentBar);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[5];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            path.AddLine(linePoints[0], linePoints[1]);
            path.StartFigure();
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLine(linePoints[2], linePoints[3]);
            path.StartFigure();
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLine(linePoints[3], linePoints[4]);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the line callout3 accent bar path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout3AccentBarPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout3AccentBar);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[6];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Bottom);
            path.AddLine(linePoints[0], linePoints[1]);
            path.StartFigure();
            linePoints[2] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[3] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLine(linePoints[2], linePoints[3]);
            path.StartFigure();
            linePoints[4] = new PointF(m_rectBounds.X + formulaValues["x3"], m_rectBounds.Y + formulaValues["y3"]);
            path.AddLine(linePoints[3], linePoints[4]);
            path.StartFigure();
            linePoints[5] = new PointF(m_rectBounds.X + formulaValues["x4"], m_rectBounds.Y + formulaValues["y4"]);
            path.AddLine(linePoints[4], linePoints[5]);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the line callout1 no border path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout1NoBorderPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout1NoBorder);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[4];
            linePoints[0] = new PointF(m_rectBounds.X, m_rectBounds.Y);
            linePoints[1] = new PointF(m_rectBounds.Right, m_rectBounds.Y);
            linePoints[2] = new PointF(m_rectBounds.Right, m_rectBounds.Bottom);
            linePoints[3] = new PointF(m_rectBounds.X, m_rectBounds.Bottom);
            path.AddLines(linePoints);
            path.CloseFigure();
            linePoints = new PointF[2];
            linePoints[0] = new PointF(m_rectBounds.X + formulaValues["x1"], m_rectBounds.Y + formulaValues["y1"]);
            linePoints[1] = new PointF(m_rectBounds.X + formulaValues["x2"], m_rectBounds.Y + formulaValues["y2"]);
            path.AddLines(linePoints);
            return path;
        }
        /// <summary>
        /// Gets the line callout2 no border path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout2NoBorderPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout2NoBorder);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[2];
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the line callout3 no border path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout3NoBorderPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout3NoBorder);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[2];
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the line callout1 border and accent bar path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout1BorderAndAccentBarPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout1BorderAndAccentBar);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[2];
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the line callout2 border and accent bar path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout2BorderAndAccentBarPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout2BorderAndAccentBar);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[2];
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets the line callout3 border and accent bar path.
        /// </summary>
        /// <returns></returns>
        internal GraphicsPath GetLineCallout3BorderAndAccentBarPath()
        {
            Dictionary<string, float> formulaValues = ParseShapeFormula(AutoShapeType.LineCallout3BorderAndAccentBar);
            GraphicsPath path = new GraphicsPath();
            PointF[] linePoints = new PointF[2];
            path.AddLines(linePoints);
            path.CloseFigure();
            return path;
        }
        #endregion
        #endregion
        #region Helper Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private float GetDegreeValue(float value)
        {
            return value / 60000;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDifference"></param>
        /// <param name="yDifference"></param>
        /// <param name="positionRatio"></param>
        /// <returns></returns>
        private PointF GetXYPosition(float xDifference, float yDifference, float positionRatio)
        {
            float x = m_rectBounds.X + ((m_rectBounds.Width * xDifference) / positionRatio);
            float y = m_rectBounds.Y + ((m_rectBounds.Height * yDifference) / positionRatio);
            return new PointF(x, y);
        }
        /// <summary>
        /// Get Path adjust value
        /// </summary>
        /// <param name="m_shapeGuide"></param>
        /// <returns></returns>
        private Dictionary<string, float> GetPathAdjustValue(AutoShapeType shapeType)
        {

            Dictionary<string, float> pathAdjValue = GetFormulaValues(AutoShapeType.Unknown, m_shapeGuide, true);
            List<string> Keys = new List<string>(pathAdjValue.Keys);
            if (shapeType == AutoShapeType.CircularArrow)
            {
                foreach (string key in Keys)
                {
                    if (key != "adj1" && key != "adj5")
                    {
                        pathAdjValue[key] = pathAdjValue[key] / 60000;
                    }
                }
            }
            return pathAdjValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="adjValue"></param>
        /// <param name="formulaColl"></param>
        /// <returns></returns>
        internal Dictionary<string, float> ParseShapeFormula(AutoShapeType shapeType)
        {
            return GetFormulaValues(shapeType, GetShapeFormula(shapeType), false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formulaColl"></param>
        /// <returns></returns>
        private Dictionary<string, float> GetFormulaValues(AutoShapeType shapeType, Dictionary<string, string> formulaColl, bool isAdjValue)
        {
            Dictionary<string, float> formulaValues = new Dictionary<string, float>();
            foreach (KeyValuePair<string, string> formula in formulaColl)
            {
                string[] splitFormula = formula.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (splitFormula.Length > 1)
                {
                    float[] operandValues = GetOperandValues(shapeType, ref formulaValues, splitFormula, isAdjValue);
                    float resultValue = GetResultValue(splitFormula[0], operandValues);
                    formulaValues.Add(formula.Key, resultValue);
                }
            }
            return formulaValues;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_rectBounds"></param>
        /// <param name="adjValues"></param>
        /// <param name="formulaValues"></param>
        /// <param name="splitFormula"></param>
        /// <returns></returns>
        private float[] GetOperandValues(AutoShapeType shapeType, ref Dictionary<string, float> formulaValues, string[] splitFormula, bool isAdjValue)
        {
            string[] presetOperands = new string[] { "3cd4", "3cd8", "5cd8", "7cd8", "b", "cd2", "cd4", "cd8", "h", "hc", "hd2", "hd3", "hd4", "hd5", "hd6", "hd8", "l", "ls", "r", "ss", "ssd2", "ssd4", "ssd6", "ssd8", "t", "vc", "w", "wd2", "wd3", "wd4", "wd5", "wd6", "wd8", "wd10" };
            Dictionary<string, float> adjValues = m_shapeGuide.Count > 0 && !isAdjValue ? GetPathAdjustValue(shapeType) : GetDefaultPathAdjValues(shapeType);
            float[] operandValues = new float[splitFormula.Length - 1];
            int j = 0;
            for (int i = 1; i < splitFormula.Length; i++)
            {
                if (!float.TryParse(splitFormula[i], out operandValues[j]))
                {
                    if (Array.IndexOf(presetOperands, splitFormula[i]) > -1)
                        operandValues[j] = GetPresetOperandValue(splitFormula[i]);
                    else if (!isAdjValue && adjValues.ContainsKey(splitFormula[i]))
                        operandValues[j] = adjValues[splitFormula[i]];
                    else if (formulaValues.ContainsKey(splitFormula[i]))
                        operandValues[j] = formulaValues[splitFormula[i]];
                }
                j++;
            }
            return operandValues;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private float GetPresetOperandValue(string operand)
        {
            switch (operand)
            {
                case "3cd4":
                    return 270;
                case "3cd8":
                    return 135;
                case "5cd8":
                    return 225;
                case "7cd8":
                    return 315;
                case "b":
                    return m_rectBounds.Height;
                case "cd2":
                    return 180;
                case "cd4":
                    return 90;
                case "cd8":
                    return 45;
                case "h":
                    return m_rectBounds.Height;
                case "hc":
                    return m_rectBounds.Width / 2;
                case "hd2":
                    return m_rectBounds.Height / 2;
                case "hd3":
                    return m_rectBounds.Height / 3;
                case "hd4":
                    return m_rectBounds.Height / 4;
                case "hd5":
                    return m_rectBounds.Height / 5;
                case "hd6":
                    return m_rectBounds.Height / 6;
                case "hd8":
                    return m_rectBounds.Height / 8;
                case "l":
                    return 0;
                case "ls":
                    return Math.Max(m_rectBounds.Width, m_rectBounds.Height);
                case "r":
                    return m_rectBounds.Width;
                case "ss":
                    return Math.Min(m_rectBounds.Width, m_rectBounds.Height);
                case "ssd2":
                    return Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 2;
                case "ssd4":
                    return Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 4;
                case "ssd6":
                    return Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 6;
                case "ssd8":
                    return Math.Min(m_rectBounds.Width, m_rectBounds.Height) / 8;
                case "t":
                    return 0;
                case "vc":
                    return m_rectBounds.Height / 2;
                case "w":
                    return m_rectBounds.Width;
                case "wd2":
                    return m_rectBounds.Width / 2;
                case "wd3":
                    return m_rectBounds.Width / 3;
                case "wd4":
                    return m_rectBounds.Width / 4;
                case "wd5":
                    return m_rectBounds.Width / 5;
                case "wd6":
                    return m_rectBounds.Width / 6;
                case "wd8":
                    return m_rectBounds.Width / 8;
                case "wd10":
                    return m_rectBounds.Width / 10;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="operandValues"></param>
        /// <returns></returns>
        private float GetResultValue(string formula, float[] operandValues)
        {
            char[] operators = new char[] { '*', '/', '+', '-' };
            float resultValue = operandValues[0];
            if (formula.Length > 1 && Array.IndexOf(operators, formula[0]) > -1)
            {
                int k = 0;
                for (int i = 0; i < formula.Length; i++)
                {
                    k++;
                    switch (formula[i])
                    {
                        case '*':
                            resultValue *= operandValues[k];
                            break;
                        case '/':
                            resultValue /= operandValues[k];
                            break;
                        case '+':
                            resultValue += operandValues[k];
                            break;
                        case '-':
                            resultValue -= operandValues[k];
                            break;
                    }
                }
            }
            else
            {
                switch (formula)
                {
                    case "?:":
                        resultValue = operandValues[0] > 0 ? operandValues[1] : operandValues[2];
                        break;
                    case "abs":
                        resultValue = Math.Abs(operandValues[0]);
                        break;
                    case "at2":
                        resultValue = (float)(Math.Atan(operandValues[1] / operandValues[0]));
                        break;
                    case "cat2":
                        float degree = (float)(Math.Atan(operandValues[2] / operandValues[1]));
                        resultValue = (float)(operandValues[0] * Math.Cos(degree));
                        break;
                    case "cos":
                        double dd = Math.Cos(operandValues[1] * Math.PI / 180);
                        resultValue = (float)(operandValues[0] * dd);
                        break;
                    case "max":
                        resultValue = (float)Math.Max(operandValues[0], operandValues[1]);
                        break;
                    case "min":
                        resultValue = (float)Math.Min(operandValues[0], operandValues[1]);
                        break;
                    case "mod":
                        resultValue = (float)Math.Sqrt((Math.Pow(operandValues[0], 2) + Math.Pow(operandValues[1], 2) + Math.Pow(operandValues[2], 2)));
                        break;
                    case "pin":
                        if (operandValues[1] < operandValues[0])
                            resultValue = operandValues[0];
                        else if (operandValues[1] > operandValues[2])
                            resultValue = operandValues[2];
                        else
                            resultValue = operandValues[1];
                        break;
                    case "sat2":
                        degree = (float)(Math.Atan(operandValues[2] / operandValues[1]));
                        resultValue = (float)(operandValues[0] * Math.Sin(degree));
                        break;
                    case "sin":
                        dd = Math.Sin(operandValues[1] * Math.PI / 180);
                        resultValue = (float)(operandValues[0] * dd);
                        break;
                    case "sqrt":
                        resultValue = (float)Math.Sqrt(operandValues[0]);
                        break;
                    case "tan":
                        dd = Math.Tan(operandValues[1] * Math.PI / 180);
                        resultValue = (float)(operandValues[0] * dd);
                        break;
                    case "val":
                        resultValue = operandValues[0];
                        break;

                }
            }
            return resultValue;
        }
        #endregion
        #region Shape Formulas and Adjust Values
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetShapeFormula(AutoShapeType shapeType)
        {
            Dictionary<string, string> formulaColl = new Dictionary<string, string>();
            switch (shapeType)
            {
                #region Lines
                case AutoShapeType.CurvedConnector:
                    formulaColl.Add("x2", "*/ w adj1 100000");
                    formulaColl.Add("x1", "+/ l x2 2");
                    formulaColl.Add("x3", "+/ r x2 2");
                    formulaColl.Add("y3", "*/ h 3 4");
                    break;
                case AutoShapeType.ElbowConnector:
                    formulaColl.Add("x1", "*/ w adj1 100000");
                    break;
                #endregion
                #region Rectangles
                case AutoShapeType.RoundedRectangle:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("x1", "*/ ss a 100000");
                    formulaColl.Add("x2", "+- r 0 x1");
                    formulaColl.Add("y2", "+- b 0 x1");
                    formulaColl.Add("il", "*/ x1 29289 100000");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 il");
                    break;
                case AutoShapeType.SnipSingleCornerRectangle:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dx1", "*/ ss a 100000");
                    formulaColl.Add("x1", "+- r 0 dx1");
                    formulaColl.Add("it", "*/ dx1 1 2");
                    formulaColl.Add("ir", "+/ x1 r 2");
                    break;
                case AutoShapeType.SnipSameSideCornerRectangle:
                    formulaColl.Add("a1", "pin 0 adj1 50000");
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("tx1", "*/ ss a1 100000");
                    formulaColl.Add("tx2", "+- r 0 tx1");
                    formulaColl.Add("bx1", "*/ ss a2 100000");
                    formulaColl.Add("bx2", "+- r 0 bx1");
                    formulaColl.Add("by1", "+- b 0 bx1");
                    formulaColl.Add("d", "+- tx1 0 bx1");
                    formulaColl.Add("dx", "?: d tx1 bx1");
                    formulaColl.Add("il", "*/ dx 1 2");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("it", "*/ tx1 1 2");
                    formulaColl.Add("ib", "+/ by1 b 2");
                    break;
                case AutoShapeType.SnipDiagonalCornerRectangle:
                    formulaColl.Add("a1", "pin 0 adj1 50000");
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("lx1", "*/ ss a1 100000");
                    formulaColl.Add("lx2", "+- r 0 lx1");
                    formulaColl.Add("ly1", "+- b 0 lx1");
                    formulaColl.Add("rx1", "*/ ss a2 100000");
                    formulaColl.Add("rx2", "+- r 0 rx1");
                    formulaColl.Add("ry1", "+- b 0 rx1");
                    formulaColl.Add("d", "+- lx1 0 rx1");
                    formulaColl.Add("dx", "?: d lx1 rx1");
                    formulaColl.Add("il", "*/ dx 1 2");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 il");
                    break;
                case AutoShapeType.SnipAndRoundSingleCornerRectangle:
                    formulaColl.Add("a1", "pin 0 adj1 50000");
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("x1", "*/ ss a1 100000");
                    formulaColl.Add("dx2", "*/ ss a2 100000");
                    formulaColl.Add("x2", "+- r 0 dx2");
                    formulaColl.Add("il", "*/ x1 29289 100000");
                    formulaColl.Add("ir", "+/ x2 r 2");
                    break;
                case AutoShapeType.RoundSingleCornerRectangle:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dx1", "*/ ss a 100000");
                    formulaColl.Add("x1", "+- r 0 dx1");
                    formulaColl.Add("idx", "*/ dx1 29289 100000");
                    formulaColl.Add("ir", "+- r 0 idx");
                    break;
                case AutoShapeType.RoundSameSideCornerRectangle:
                    formulaColl.Add("a1", "pin 0 adj1 50000");
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("tx1", "*/ ss a1 100000");
                    formulaColl.Add("tx2", "+- r 0 tx1");
                    formulaColl.Add("bx1", "*/ ss a2 100000");
                    formulaColl.Add("bx2", "+- r 0 bx1");
                    formulaColl.Add("by1", "+- b 0 bx1");
                    formulaColl.Add("d", "+- tx1 0 bx1");
                    formulaColl.Add("tdx", "*/ tx1 29289 100000");
                    formulaColl.Add("bdx", "*/ bx1 29289 100000");
                    formulaColl.Add("il", "?: d tdx bdx");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 bdx");
                    break;
                case AutoShapeType.RoundDiagonalCornerRectangle:
                    formulaColl.Add("a1", "pin 0 adj1 50000");
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("x1", "*/ ss a1 100000");
                    formulaColl.Add("y1", "+- b 0 x1");
                    formulaColl.Add("a", "*/ ss a2 100000");
                    formulaColl.Add("x2", "+- r 0 a");
                    formulaColl.Add("y2", "+- b 0 a");
                    formulaColl.Add("dx1", "*/ x1 29289 100000");
                    formulaColl.Add("dx2", "*/ a 29289 100000");
                    formulaColl.Add("d", "+- dx1 0 dx2");
                    formulaColl.Add("dx", "?: d dx1 dx2");
                    formulaColl.Add("ir", "+- r 0 dx");
                    formulaColl.Add("ib", "+- b 0 dx");
                    break;
                #endregion
                #region BasicShapes
                case AutoShapeType.Oval:
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.RightTriangle:
                    formulaColl.Add("it", "*/ h 7 12");
                    formulaColl.Add("ir", "*/ w 7 12");
                    formulaColl.Add("ib", "*/ h 11 12");
                    break;
                case AutoShapeType.Diamond:
                    formulaColl.Add("ir", "*/ w 3 4");
                    formulaColl.Add("ib", "*/ h 3 4");
                    break;
                case AutoShapeType.IsoscelesTriangle:
                    formulaColl.Add("a", "pin 0 adj 100000");
                    formulaColl.Add("x1", "*/ w a 200000");
                    formulaColl.Add("x2", "*/ w a 100000");
                    formulaColl.Add("x3", "+- x1 wd2 0");
                    break;
                case AutoShapeType.Parallelogram:
                    formulaColl.Add("maxAdj", "*/ 100000 w ss");
                    formulaColl.Add("a", "pin 0 adj maxAdj");
                    formulaColl.Add("x1", "*/ ss a 200000");
                    formulaColl.Add("x2", "*/ ss a 100000");
                    formulaColl.Add("x6", "+- r 0 x1");
                    formulaColl.Add("x5", "+- r 0 x2");
                    formulaColl.Add("x3", "*/ x5 1 2");
                    formulaColl.Add("x4", "+- r 0 x3");
                    formulaColl.Add("il1", "*/ wd2 a maxAdj");
                    formulaColl.Add("q1", "*/ 5 a maxAdj");
                    formulaColl.Add("q2", "+/ 1 q1 12");
                    formulaColl.Add("il", "*/ q2 w 1");
                    formulaColl.Add("it", "*/ q2 h 1");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 it");
                    formulaColl.Add("q3", "*/ h hc x2");
                    formulaColl.Add("y1", "pin 0 q3 h");
                    formulaColl.Add("y2", "+- b 0 y1");
                    break;
                case AutoShapeType.Trapezoid:
                    formulaColl.Add("maxAdj", "*/ 50000 w ss");
                    formulaColl.Add("a", "pin 0 adj maxAdj");
                    formulaColl.Add("x1", "*/ ss a 200000");
                    formulaColl.Add("x2", "*/ ss a 100000");
                    formulaColl.Add("x3", "+- r 0 x2");
                    formulaColl.Add("x4", "+- r 0 x1");
                    formulaColl.Add("il", "*/ wd3 a maxAdj");
                    formulaColl.Add("it", "*/ hd3 a maxAdj");
                    formulaColl.Add("ir", "+- r 0 il");
                    break;
                case AutoShapeType.RegularPentagon:
                    formulaColl.Add("swd2", "*/ wd2 hf 100000");
                    formulaColl.Add("shd2", "*/ hd2 vf 100000");
                    formulaColl.Add("svc", "*/ vc  vf 100000");
                    formulaColl.Add("dx1", "cos swd2 18");
                    formulaColl.Add("dx2", "cos swd2 306");
                    formulaColl.Add("dy1", "sin shd2 18");
                    formulaColl.Add("dy2", "sin shd2 306");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- svc 0 dy1");
                    formulaColl.Add("y2", "+- svc 0 dy2");
                    formulaColl.Add("it", "*/ y1 dx2 dx1");
                    break;
                case AutoShapeType.Hexagon:
                    formulaColl.Add("maxAdj", "*/ 50000 w ss");
                    formulaColl.Add("a", "pin 0 adj maxAdj");
                    formulaColl.Add("shd2", "*/ hd2 vf 100000");
                    formulaColl.Add("x1", "*/ ss a 100000");
                    formulaColl.Add("x2", "+- r 0 x1");
                    formulaColl.Add("dy1", "sin shd2 60");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc dy1 0");
                    formulaColl.Add("q1", "*/ maxAdj -1 2");
                    formulaColl.Add("q2", "+- a q1 0");
                    formulaColl.Add("q3", "?: q2 4 2");
                    formulaColl.Add("q4", "?: q2 3 2");
                    formulaColl.Add("q5", "?: q2 q1 0");
                    formulaColl.Add("q6", "+/ a q5 q1");
                    formulaColl.Add("q7", "*/ q6 q4 -1");
                    formulaColl.Add("q8", "+- q3 q7 0");
                    formulaColl.Add("il", "*/ w q8 24");
                    formulaColl.Add("it", "*/ h q8 24");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 it");
                    break;
                case AutoShapeType.Heptagon:
                    formulaColl.Add("swd2", "*/ wd2 hf 100000");
                    formulaColl.Add("shd2", "*/ hd2 vf 100000");
                    formulaColl.Add("svc", "*/ vc  vf 100000");
                    formulaColl.Add("dx1", "*/ swd2 97493 100000");
                    formulaColl.Add("dx2", "*/ swd2 78183 100000");
                    formulaColl.Add("dx3", "*/ swd2 43388 100000");
                    formulaColl.Add("dy1", "*/ shd2 62349 100000");
                    formulaColl.Add("dy2", "*/ shd2 22252 100000");
                    formulaColl.Add("dy3", "*/ shd2 90097 100000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc 0 dx3");
                    formulaColl.Add("x4", "+- hc dx3 0");
                    formulaColl.Add("x5", "+- hc dx2 0");
                    formulaColl.Add("x6", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- svc 0 dy1");
                    formulaColl.Add("y2", "+- svc dy2 0");
                    formulaColl.Add("y3", "+- svc dy3 0");
                    formulaColl.Add("ib", "+- b 0 y1");
                    break;
                case AutoShapeType.Octagon:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("x1", "*/ ss a 100000");
                    formulaColl.Add("x2", "+- r 0 x1");
                    formulaColl.Add("y2", "+- b 0 x1");
                    formulaColl.Add("il", "*/ x1 1 2");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 il");
                    break;
                case AutoShapeType.Decagon:
                    formulaColl.Add("shd2", "*/ hd2 vf 100000");
                    formulaColl.Add("dx1", "cos wd2 36");
                    formulaColl.Add("dx2", "cos wd2 72");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("dy1", "sin shd2 72");
                    formulaColl.Add("dy2", "sin shd2 36");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc dy2 0");
                    formulaColl.Add("y4", "+- vc dy1 0");
                    break;
                case AutoShapeType.Dodecagon:
                    formulaColl.Add("x1", "*/ w 2894 21600");
                    formulaColl.Add("x2", "*/ w 7906 21600");
                    formulaColl.Add("x3", "*/ w 13694 21600");
                    formulaColl.Add("x4", "*/ w 18706 21600");
                    formulaColl.Add("y1", "*/ h 2894 21600");
                    formulaColl.Add("y2", "*/ h 7906 21600");
                    formulaColl.Add("y3", "*/ h 13694 21600");
                    formulaColl.Add("y4", "*/ h 18706 21600");
                    break;
                case AutoShapeType.Pie:
                    formulaColl.Add("stAng", "pin 0 adj1 360");
                    formulaColl.Add("enAng", "pin 0 adj2 360");
                    formulaColl.Add("sw1", "+- enAng 0 stAng");
                    formulaColl.Add("sw2", "+- sw1 360 0");
                    formulaColl.Add("swAng", "?: sw1 sw1 sw2");
                    formulaColl.Add("wt1", "sin wd2 stAng");
                    formulaColl.Add("ht1", "cos hd2 stAng");
                    formulaColl.Add("dx1", "cat2 wd2 ht1 wt1");
                    formulaColl.Add("dy1", "sat2 hd2 ht1 wt1");
                    formulaColl.Add("x1", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc dy1 0");
                    formulaColl.Add("wt2", "sin wd2 enAng");
                    formulaColl.Add("ht2", "cos hd2 enAng");
                    formulaColl.Add("dx2", "cat2 wd2 ht2 wt2");
                    formulaColl.Add("dy2", "sat2 hd2 ht2 wt2");
                    formulaColl.Add("x2", "+- hc dx2 0");
                    formulaColl.Add("y2", "+- vc dy2 0");
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.Chord:
                    formulaColl.Add("stAng", "pin 0 adj1 360");
                    formulaColl.Add("enAng", "pin 0 adj2 360");
                    formulaColl.Add("sw1", "+- enAng 0 stAng");
                    formulaColl.Add("sw2", "+- sw1 360 0");
                    formulaColl.Add("swAng", "?: sw1 sw1 sw2");
                    formulaColl.Add("wt1", "sin wd2 stAng");
                    formulaColl.Add("ht1", "cos hd2 stAng");
                    formulaColl.Add("dx1", "cat2 wd2 ht1 wt1");
                    formulaColl.Add("dy1", "sat2 hd2 ht1 wt1");
                    formulaColl.Add("wt2", "sin wd2 enAng");
                    formulaColl.Add("ht2", "cos hd2 enAng");
                    formulaColl.Add("dx2", "cat2 wd2 ht2 wt2");
                    formulaColl.Add("dy2", "sat2 hd2 ht2 wt2");
                    formulaColl.Add("x1", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc dy1 0");
                    formulaColl.Add("x2", "+- hc dx2 0");
                    formulaColl.Add("y2", "+- vc dy2 0");
                    formulaColl.Add("x3", "+/ x1 x2 2");
                    formulaColl.Add("y3", "+/ y1 y2 2");
                    formulaColl.Add("midAng0", "*/ swAng 1 2");
                    formulaColl.Add("midAng", "+- stAng midAng0 cd2");
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.Teardrop:
                    formulaColl.Add("a", "pin 0 adj 200000");
                    formulaColl.Add("r2", "sqrt 2");
                    formulaColl.Add("tw", "*/ wd2 r2 1");
                    formulaColl.Add("th", "*/ hd2 r2 1");
                    formulaColl.Add("sw", "*/ tw a 100000");
                    formulaColl.Add("sh", "*/ th a 100000");
                    formulaColl.Add("dx1", "cos sw 45");
                    formulaColl.Add("dy1", "sin sh 45");
                    formulaColl.Add("x1", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("x2", "+/ hc x1 2");
                    formulaColl.Add("y2", "+/ vc y1 2");
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.Frame:
                    formulaColl.Add("a1", "pin 0 adj1 50000");
                    formulaColl.Add("x1", "*/ ss a1 100000");
                    formulaColl.Add("x4", "+- r 0 x1");
                    formulaColl.Add("y4", "+- b 0 x1");
                    break;
                case AutoShapeType.HalfFrame:
                    formulaColl.Add("maxAdj2", "*/ 100000 w ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("x1", "*/ ss a2 100000");
                    formulaColl.Add("g1", "*/ h x1 w");
                    formulaColl.Add("g2", "+- h 0 g1");
                    formulaColl.Add("maxAdj1", "*/ 100000 g2 ss");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("y1", "*/ ss a1 100000");
                    formulaColl.Add("dx2", "*/ y1 w h");
                    formulaColl.Add("x2", "+- r 0 dx2");
                    formulaColl.Add("dy2", "*/ x1 h w");
                    formulaColl.Add("y2", "+- b 0 dy2");
                    formulaColl.Add("cx1", "*/ x1 1 2");
                    formulaColl.Add("cy1", "+/ y2 b 2");
                    formulaColl.Add("cx2", "+/ x2 r 2");
                    formulaColl.Add("cy2", "*/ y1 1 2");
                    break;
                case AutoShapeType.L_Shape:
                    formulaColl.Add("maxAdj1", "*/ 100000 h ss");
                    formulaColl.Add("maxAdj2", "*/ 100000 w ss");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("x1", "*/ ss a2 100000");
                    formulaColl.Add("dy1", "*/ ss a1 100000");
                    formulaColl.Add("y1", "+- b 0 dy1");
                    formulaColl.Add("cx1", "*/ x1 1 2");
                    formulaColl.Add("cy1", "+/ y1 b 2");
                    formulaColl.Add("d", "+- w 0 h");
                    formulaColl.Add("it", "?: d y1 t");
                    formulaColl.Add("ir", "?: d r x1");
                    break;
                case AutoShapeType.DiagonalStripe:
                    formulaColl.Add("a", "pin 0 adj 100000");
                    formulaColl.Add("x2", "*/ w a 100000");
                    formulaColl.Add("x1", "*/ x2 1 2");
                    formulaColl.Add("x3", "+/ x2 r 2");
                    formulaColl.Add("y2", "*/ h a 100000");
                    formulaColl.Add("y1", "*/ y2 1 2");
                    formulaColl.Add("y3", "+/ y2 b 2");
                    break;
                case AutoShapeType.Cross:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("x1", "*/ ss a 100000");
                    formulaColl.Add("x2", "+- r 0 x1");
                    formulaColl.Add("y2", "+- b 0 x1");
                    formulaColl.Add("d", "+- w 0 h");
                    formulaColl.Add("il", "?: d l x1");
                    formulaColl.Add("ir", "?: d r x2");
                    formulaColl.Add("it", "?: d x1 t");
                    formulaColl.Add("ib", "?: d y2 b");
                    break;
                case AutoShapeType.Plaque:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("x1", "*/ ss a 100000");
                    formulaColl.Add("x2", "+- r 0 x1");
                    formulaColl.Add("y2", "+- b 0 x1");
                    formulaColl.Add("il", "*/ x1 70711 100000");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 il");
                    break;
                case AutoShapeType.Can:
                    formulaColl.Add("maxAdj", "*/ 50000 h ss");
                    formulaColl.Add("a", "pin 0 adj maxAdj");
                    formulaColl.Add("y1", "*/ ss a 200000");
                    formulaColl.Add("y2", "+- y1 y1 0");
                    formulaColl.Add("y3", "+- b 0 y1");
                    break;
                case AutoShapeType.Cube:
                    formulaColl.Add("a", "pin 0 adj 100000");
                    formulaColl.Add("y1", "*/ ss a 100000");
                    formulaColl.Add("y4", "+- b 0 y1");
                    formulaColl.Add("y2", "*/ y4 1 2");
                    formulaColl.Add("y3", "+/ y1 b 2");
                    formulaColl.Add("x4", "+- r 0 y1");
                    formulaColl.Add("x2", "*/ x4 1 2");
                    formulaColl.Add("x3", "+/ y1 r 2");
                    break;
                case AutoShapeType.Bevel:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("x1", "*/ ss a 100000");
                    formulaColl.Add("x2", "+- r 0 x1");
                    formulaColl.Add("y2", "+- b 0 x1");
                    break;
                case AutoShapeType.Donut:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dr", "*/ ss a 100000");
                    formulaColl.Add("iwd2", "+- wd2 0 dr");
                    formulaColl.Add("ihd2", "+- hd2 0 dr");
                    formulaColl.Add("idx", "cos wd2 2700000");
                    formulaColl.Add("idy", "sin hd2 2700000");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.NoSymbol:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dr", "*/ ss a 100000");
                    formulaColl.Add("iwd2", "+- wd2 0 dr");
                    formulaColl.Add("ihd2", "+- hd2 0 dr");
                    formulaColl.Add("ang3", "at2 w h");
                    formulaColl.Add("ang", "*/ ang3 180 " + Math.PI.ToString());
                    formulaColl.Add("ct", "cos ihd2 ang");
                    formulaColl.Add("st", "sin iwd2 ang");
                    formulaColl.Add("m", "mod ct st 0");
                    formulaColl.Add("n", "*/ iwd2 ihd2 m");
                    formulaColl.Add("drd2", "*/ dr 1 2");
                    formulaColl.Add("dang3", "at2 n drd2");
                    formulaColl.Add("dang", "*/ dang3 180 " + Math.PI.ToString());
                    formulaColl.Add("2dang", "*/ dang 2 1");
                    formulaColl.Add("swAng", "+- -180 2dang 0");
                    formulaColl.Add("t4", "at2 w h");
                    formulaColl.Add("t3", "*/ t4 180 " + Math.PI.ToString());
                    formulaColl.Add("stAng1", "+- t3 0 dang");
                    formulaColl.Add("stAng2", "+- stAng1 0 cd2");
                    formulaColl.Add("ct1", "cos ihd2 stAng1");
                    formulaColl.Add("st1", "sin iwd2 stAng1");
                    formulaColl.Add("m1", "mod ct1 st1 0");
                    formulaColl.Add("n1", "*/ iwd2 ihd2 m1");
                    formulaColl.Add("dx1", "cos n1 stAng1");
                    formulaColl.Add("dy1", "sin n1 stAng1");
                    formulaColl.Add("x1", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc dy1 0");
                    formulaColl.Add("x2", "+- hc 0 dx1");
                    formulaColl.Add("y2", "+- vc 0 dy1");
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.BlockArc:
                    formulaColl.Add("stAng", "pin 0 adj1 21599999");
                    formulaColl.Add("istAng", "pin 0 adj2 21599999");
                    formulaColl.Add("a3", "pin 0 adj3 50000");
                    formulaColl.Add("sw11", "+- istAng 0 stAng");
                    formulaColl.Add("sw12", "+- sw11 21600000 0");
                    formulaColl.Add("swAng", "?: sw11 sw11 sw12");
                    formulaColl.Add("iswAng", "+- 0 0 swAng");
                    formulaColl.Add("wt1", "sin wd2 stAng");
                    formulaColl.Add("ht1", "cos hd2 stAng");
                    formulaColl.Add("wt3", "sin wd2 istAng");
                    formulaColl.Add("ht3", "cos hd2 istAng");
                    formulaColl.Add("dx1", "cat2 wd2 ht1 wt1");
                    formulaColl.Add("dy1", "sat2 hd2 ht1 wt1");
                    formulaColl.Add("dx3", "cat2 wd2 ht3 wt3");
                    formulaColl.Add("dy3", "sat2 hd2 ht3 wt3");
                    formulaColl.Add("x1", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc dy1 0");
                    formulaColl.Add("x3", "+- hc dx3 0");
                    formulaColl.Add("y3", "+- vc dy3 0");
                    formulaColl.Add("dr", "*/ ss a3 100000");
                    formulaColl.Add("iwd2", "+- wd2 0 dr");
                    formulaColl.Add("ihd2", "+- hd2 0 dr");
                    formulaColl.Add("wt2", "sin iwd2 istAng");
                    formulaColl.Add("ht2", "cos ihd2 istAng");
                    formulaColl.Add("wt4", "sin iwd2 stAng");
                    formulaColl.Add("ht4", "cos ihd2 stAng");
                    formulaColl.Add("dx2", "cat2 iwd2 ht2 wt2");
                    formulaColl.Add("dy2", "sat2 ihd2 ht2 wt2");
                    formulaColl.Add("dx4", "cat2 iwd2 ht4 wt4");
                    formulaColl.Add("dy4", "sat2 ihd2 ht4 wt4");
                    formulaColl.Add("x2", "+- hc dx2 0");
                    formulaColl.Add("y2", "+- vc dy2 0");
                    formulaColl.Add("x4", "+- hc dx4 0");
                    formulaColl.Add("y4", "+- vc dy4 0");
                    formulaColl.Add("sw0", "+- 21600000 0 stAng");
                    formulaColl.Add("da1", "+- swAng 0 sw0");
                    formulaColl.Add("g1", "max x1 x2");
                    formulaColl.Add("g2", "max x3 x4");
                    formulaColl.Add("g3", "max g1 g2");
                    formulaColl.Add("ir", "?: da1 r g3");
                    formulaColl.Add("sw1", "+- cd4 0 stAng");
                    formulaColl.Add("sw2", "+- 27000000 0 stAng");
                    formulaColl.Add("sw3", "?: sw1 sw1 sw2");
                    formulaColl.Add("da2", "+- swAng 0 sw3");
                    formulaColl.Add("g5", "max y1 y2");
                    formulaColl.Add("g6", "max y3 y4");
                    formulaColl.Add("g7", "max g5 g6");
                    formulaColl.Add("ib", "?: da2 b g7");
                    formulaColl.Add("sw4", "+- cd2 0 stAng");
                    formulaColl.Add("sw5", "+- 32400000 0 stAng");
                    formulaColl.Add("sw6", "?: sw4 sw4 sw5");
                    formulaColl.Add("da3", "+- swAng 0 sw6");
                    formulaColl.Add("g9", "min x1 x2");
                    formulaColl.Add("g10", "min x3 x4");
                    formulaColl.Add("g11", "min g9 g10");
                    formulaColl.Add("il", "?: da3 l g11");
                    formulaColl.Add("sw7", "+- 3cd4 0 stAng");
                    formulaColl.Add("sw8", "+- 37800000 0 stAng");
                    formulaColl.Add("sw9", "?: sw7 sw7 sw8");
                    formulaColl.Add("da4", "+- swAng 0 sw9");
                    formulaColl.Add("g13", "min y1 y2");
                    formulaColl.Add("g14", "min y3 y4");
                    formulaColl.Add("g15", "min g13 g14");
                    formulaColl.Add("it", "?: da4 t g15");
                    formulaColl.Add("x5", "+/ x1 x4 2");
                    formulaColl.Add("y5", "+/ y1 y4 2");
                    formulaColl.Add("x6", "+/ x3 x2 2");
                    formulaColl.Add("y6", "+/ y3 y2 2");
                    formulaColl.Add("cang1", "+- stAng 0 cd4");
                    formulaColl.Add("cang2", "+- istAng cd4 0");
                    formulaColl.Add("cang3", "+/ cang1 cang2 2");
                    break;
                case AutoShapeType.FoldedCorner:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dy2", "*/ ss a 100000");
                    formulaColl.Add("dy1", "*/ dy2 1 5");
                    formulaColl.Add("x1", "+- r 0 dy2");
                    formulaColl.Add("x2", "+- x1 dy1 0");
                    formulaColl.Add("y2", "+- b 0 dy2");
                    formulaColl.Add("y1", "+- y2 dy1 0");
                    break;
                case AutoShapeType.SmileyFace:
                    formulaColl.Add("a", "pin -4653 adj 4653");
                    formulaColl.Add("x1", "*/ w 4969 21699");
                    formulaColl.Add("x2", "*/ w 6215 21600");
                    formulaColl.Add("x3", "*/ w 13135 21600");
                    formulaColl.Add("x4", "*/ w 16640 21600");
                    formulaColl.Add("y1", "*/ h 7570 21600");
                    formulaColl.Add("y3", "*/ h 16515 21600");
                    formulaColl.Add("dy2", "*/ h a 100000");
                    formulaColl.Add("y2", "+- y3 0 dy2");
                    formulaColl.Add("y4", "+- y3 dy2 0");
                    formulaColl.Add("dy3", "*/ h a 50000");
                    formulaColl.Add("y5", "+- y4 dy3 0");
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    formulaColl.Add("wR", "*/ w 1125 21600");
                    formulaColl.Add("hR", "*/ h 1125 21600");
                    break;
                case AutoShapeType.Heart:
                    formulaColl.Add("dx1", "*/ w 49 48");
                    formulaColl.Add("dx2", "*/ w 10 48");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- t 0 hd3");
                    formulaColl.Add("il", "*/ w 1 6");
                    formulaColl.Add("ir", "*/ w 5 6");
                    formulaColl.Add("ib", "*/ h 2 3");
                    break;
                case AutoShapeType.LightningBolt:
                    formulaColl.Add("x1", "*/ w 5022 21600");
                    formulaColl.Add("x3", "*/ w 8472 21600");
                    formulaColl.Add("x4", "*/ w 8757 21600");
                    formulaColl.Add("x5", "*/ w 10012 21600");
                    formulaColl.Add("x8", "*/ w 12860 21600");
                    formulaColl.Add("x9", "*/ w 13917 21600");
                    formulaColl.Add("x11", "*/ w 16577 21600");
                    formulaColl.Add("y1", "*/ h 3890 21600");
                    formulaColl.Add("y2", "*/ h 6080 21600");
                    formulaColl.Add("y4", "*/ h 7437 21600");
                    formulaColl.Add("y6", "*/ h 9705 21600");
                    formulaColl.Add("y7", "*/ h 12007 21600");
                    formulaColl.Add("y10", "*/ h 14277 21600");
                    formulaColl.Add("y11", "*/ h 14915 21600");
                    break;
                case AutoShapeType.Sun:
                    formulaColl.Add("a", "pin 12500 adj 46875");
                    formulaColl.Add("g0", "+- 50000 0 a");
                    formulaColl.Add("g1", "*/ g0 30274 32768");
                    formulaColl.Add("g2", "*/ g0 12540 32768");
                    formulaColl.Add("g3", "+- g1 50000 0");
                    formulaColl.Add("g4", "+- g2 50000 0");
                    formulaColl.Add("g5", "+- 50000 0 g1");
                    formulaColl.Add("g6", "+- 50000 0 g2");
                    formulaColl.Add("g7", "*/ g0 23170 32768");
                    formulaColl.Add("g8", "+- 50000 g7 0");
                    formulaColl.Add("g9", "+- 50000 0 g7");
                    formulaColl.Add("g10", "*/ g5 3 4");
                    formulaColl.Add("g11", "*/ g6 3 4");
                    formulaColl.Add("g12", "+- g10 3662 0");
                    formulaColl.Add("g13", "+- g11 3662 0");
                    formulaColl.Add("g14", "+- g11 12500 0");
                    formulaColl.Add("g15", "+- 100000 0 g10");
                    formulaColl.Add("g16", "+- 100000 0 g12");
                    formulaColl.Add("g17", "+- 100000 0 g13");
                    formulaColl.Add("g18", "+- 100000 0 g14");
                    formulaColl.Add("ox1", "*/ w 18436 21600");
                    formulaColl.Add("oy1", "*/ h 3163 21600");
                    formulaColl.Add("ox2", "*/ w 3163 21600");
                    formulaColl.Add("oy2", "*/ h 18436 21600");
                    formulaColl.Add("x8", "*/ w g8 100000");
                    formulaColl.Add("x9", "*/ w g9 100000");
                    formulaColl.Add("x10", "*/ w g10 100000");
                    formulaColl.Add("x12", "*/ w g12 100000");
                    formulaColl.Add("x13", "*/ w g13 100000");
                    formulaColl.Add("x14", "*/ w g14 100000");
                    formulaColl.Add("x15", "*/ w g15 100000");
                    formulaColl.Add("x16", "*/ w g16 100000");
                    formulaColl.Add("x17", "*/ w g17 100000");
                    formulaColl.Add("x18", "*/ w g18 100000");
                    formulaColl.Add("x19", "*/ w a 100000");
                    formulaColl.Add("wR", "*/ w g0 100000");
                    formulaColl.Add("hR", "*/ h g0 100000");
                    formulaColl.Add("y8", "*/ h g8 100000");
                    formulaColl.Add("y9", "*/ h g9 100000");
                    formulaColl.Add("y10", "*/ h g10 100000");
                    formulaColl.Add("y12", "*/ h g12 100000");
                    formulaColl.Add("y13", "*/ h g13 100000");
                    formulaColl.Add("y14", "*/ h g14 100000");
                    formulaColl.Add("y15", "*/ h g15 100000");
                    formulaColl.Add("y16", "*/ h g16 100000");
                    formulaColl.Add("y17", "*/ h g17 100000");
                    formulaColl.Add("y18", "*/ h g18 100000");
                    break;
                case AutoShapeType.Moon:
                    formulaColl.Add("a", "pin 0 adj 87500");
                    formulaColl.Add("g0", "*/ ss a 100000");
                    formulaColl.Add("g0w", "*/ g0 w ss");
                    formulaColl.Add("g1", "+- ss 0 g0");
                    formulaColl.Add("g2", "*/ g0 g0 g1");
                    formulaColl.Add("g3", "*/ ss ss g1");
                    formulaColl.Add("g4", "*/ g3 2 1");
                    formulaColl.Add("g5", "+- g4 0 g2");
                    formulaColl.Add("g6", "+- g5 0 g0");
                    formulaColl.Add("g6w", "*/ g6 w ss");
                    formulaColl.Add("g7", "*/ g5 1 2");
                    formulaColl.Add("g8", "+- g7 0 g0");
                    formulaColl.Add("dy1", "*/ g8 hd2 ss");
                    formulaColl.Add("g10h", "+- vc 0 dy1");
                    formulaColl.Add("g11h", "+- vc dy1 0");
                    formulaColl.Add("g12", "*/ g0 9598 32768");
                    formulaColl.Add("g12w", "*/ g12 w ss");
                    formulaColl.Add("g13", "+- ss 0 g12");
                    formulaColl.Add("q1", "*/ ss ss 1");
                    formulaColl.Add("q2", "*/ g13 g13 1");
                    formulaColl.Add("q3", "+- q1 0 q2");
                    formulaColl.Add("q4", "sqrt q3");
                    formulaColl.Add("dy4", "*/ q4 hd2 ss");
                    formulaColl.Add("g15h", "+- vc 0 dy4");
                    formulaColl.Add("g16h", "+- vc dy4 0");
                    formulaColl.Add("g17w", "+- g6w 0 g0w");
                    formulaColl.Add("g18w", "*/ g17w 1 2");
                    formulaColl.Add("dx2p", "+- g0w g18w w");
                    formulaColl.Add("dx2", "*/ dx2p -1 1");
                    formulaColl.Add("dy2", "*/ hd2 -1 1");
                    formulaColl.Add("stAng", "at2 dx2 dy2");
                    formulaColl.Add("stAng1", "*/ stAng 180 " + Math.PI.ToString());
                    formulaColl.Add("enAngp", "at2 dx2 hd2");
                    formulaColl.Add("enAngp1", "*/ enAngp 180 " + Math.PI.ToString());
                    formulaColl.Add("enAng1", "+- enAngp1 0 360");
                    formulaColl.Add("swAng1", "+- enAng1 0 stAng1");
                    break;
                case AutoShapeType.Cloud:
                    formulaColl.Add("il", "*/ w 2977 21600");
                    formulaColl.Add("it", "*/ h 3262 21600");
                    formulaColl.Add("ir", "*/ w 17087 21600");
                    formulaColl.Add("ib", "*/ h 17337 21600");
                    formulaColl.Add("g27", "*/ w 67 21600");
                    formulaColl.Add("g28", "*/ h 21577 21600");
                    formulaColl.Add("g29", "*/ w 21582 21600");
                    formulaColl.Add("g30", "*/ h 1235 21600");
                    break;
                case AutoShapeType.Arc:
                    formulaColl.Add("stAng", "pin 0 adj1 21599999");
                    formulaColl.Add("enAng", "pin 0 adj2 21599999");
                    formulaColl.Add("sw11", "+- enAng 0 stAng");
                    formulaColl.Add("sw12", "+- sw11 21600000 0");
                    formulaColl.Add("swAng", "?: sw11 sw11 sw12");
                    formulaColl.Add("wt1", "sin wd2 stAng");
                    formulaColl.Add("ht1", "cos hd2 stAng");
                    formulaColl.Add("dx1", "cat2 wd2 ht1 wt1");
                    formulaColl.Add("dy1", "sat2 hd2 ht1 wt1");
                    formulaColl.Add("wt2", "sin wd2 enAng");
                    formulaColl.Add("ht2", "cos hd2 enAng");
                    formulaColl.Add("dx2", "cat2 wd2 ht2 wt2");
                    formulaColl.Add("dy2", "sat2 hd2 ht2 wt2");
                    formulaColl.Add("x1", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc dy1 0");
                    formulaColl.Add("x2", "+- hc dx2 0");
                    formulaColl.Add("y2", "+- vc dy2 0");
                    formulaColl.Add("sw0", "+- 21600000 0 stAng");
                    formulaColl.Add("da1", "+- swAng 0 sw0");
                    formulaColl.Add("g1", "max x1 x2");
                    formulaColl.Add("ir", "?: da1 r g1");
                    formulaColl.Add("sw1", "+- cd4 0 stAng");
                    formulaColl.Add("sw2", "+- 27000000 0 stAng");
                    formulaColl.Add("sw3", "?: sw1 sw1 sw2");
                    formulaColl.Add("da2", "+- swAng 0 sw3");
                    formulaColl.Add("g5", "max y1 y2");
                    formulaColl.Add("ib", "?: da2 b g5");
                    formulaColl.Add("sw4", "+- cd2 0 stAng");
                    formulaColl.Add("sw5", "+- 32400000 0 stAng");
                    formulaColl.Add("sw6", "?: sw4 sw4 sw5");
                    formulaColl.Add("da3", "+- swAng 0 sw6");
                    formulaColl.Add("g9", "min x1 x2");
                    formulaColl.Add("il", "?: da3 l g9");
                    formulaColl.Add("sw7", "+- 3cd4 0 stAng");
                    formulaColl.Add("sw8", "+- 37800000 0 stAng");
                    formulaColl.Add("sw9", "?: sw7 sw7 sw8");
                    formulaColl.Add("da4", "+- swAng 0 sw9");
                    formulaColl.Add("g13", "min y1 y2");
                    formulaColl.Add("it", "?: da4 t g13");
                    formulaColl.Add("cang1", "+- stAng 0 cd4");
                    formulaColl.Add("cang2", "+- enAng cd4 0");
                    formulaColl.Add("cang3", "+/ cang1 cang2 2");
                    break;
                case AutoShapeType.DoubleBracket:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("x1", "*/ ss a 100000");
                    formulaColl.Add("x2", "+- r 0 x1");
                    formulaColl.Add("y2", "+- b 0 x1");
                    formulaColl.Add("il", "*/ x1 29289 100000");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 il");
                    break;
                case AutoShapeType.DoubleBrace:
                    formulaColl.Add("a", "pin 0 adj 25000");
                    formulaColl.Add("x1", "*/ ss a 100000");
                    formulaColl.Add("x2", "*/ ss a 50000");
                    formulaColl.Add("x3", "+- r 0 x2");
                    formulaColl.Add("x4", "+- r 0 x1");
                    formulaColl.Add("y2", "+- vc 0 x1");
                    formulaColl.Add("y3", "+- vc x1 0");
                    formulaColl.Add("y4", "+- b 0 x1");
                    formulaColl.Add("it", "*/ x1 29289 100000");
                    formulaColl.Add("il", "+- x1 it 0");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 it");
                    break;
                case AutoShapeType.LeftBracket:
                    formulaColl.Add("maxAdj", "*/ 50000 h ss");
                    formulaColl.Add("a", "pin 0 adj maxAdj");
                    formulaColl.Add("y1", "*/ ss a 100000");
                    formulaColl.Add("y2", "+- b 0 y1");
                    formulaColl.Add("dx1", "cos w 2700000");
                    formulaColl.Add("dy1", "sin y1 2700000");
                    formulaColl.Add("il", "+- r 0 dx1");
                    formulaColl.Add("it", "+- y1 0 dy1");
                    formulaColl.Add("ib", "+- b dy1 y1");
                    break;
                case AutoShapeType.RightBracket:
                    formulaColl.Add("maxAdj", "*/ 50000 h ss");
                    formulaColl.Add("a", "pin 0 adj maxAdj");
                    formulaColl.Add("y1", "*/ ss a 100000");
                    formulaColl.Add("y2", "+- b 0 y1");
                    formulaColl.Add("dx1", "cos w 2700000");
                    formulaColl.Add("dy1", "sin y1 2700000");
                    formulaColl.Add("ir", "+- l dx1 0");
                    formulaColl.Add("it", "+- y1 0 dy1");
                    formulaColl.Add("ib", "+- b dy1 y1");
                    break;
                case AutoShapeType.LeftBrace:
                    formulaColl.Add("a2", "pin 0 adj2 100000");
                    formulaColl.Add("q1", "+- 100000 0 a2");
                    formulaColl.Add("q2", "min q1 a2");
                    formulaColl.Add("q3", "*/ q2 1 2");
                    formulaColl.Add("maxAdj1", "*/ q3 h ss");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("y1", "*/ ss a1 100000");
                    formulaColl.Add("y3", "*/ h a2 100000");
                    formulaColl.Add("y4", "+- y3 y1 0");
                    formulaColl.Add("dx1", "cos wd2 2700000");
                    formulaColl.Add("dy1", "sin y1 2700000");
                    formulaColl.Add("il", "+- r 0 dx1");
                    formulaColl.Add("it", "+- y1 0 dy1");
                    formulaColl.Add("ib", "+- b dy1 y1");
                    break;
                case AutoShapeType.RightBrace:
                    formulaColl.Add("a2", "pin 0 adj2 100000");
                    formulaColl.Add("q1", "+- 100000 0 a2");
                    formulaColl.Add("q2", "min q1 a2");
                    formulaColl.Add("q3", "*/ q2 1 2");
                    formulaColl.Add("maxAdj1", "*/ q3 h ss");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("y1", "*/ ss a1 100000");
                    formulaColl.Add("y3", "*/ h a2 100000");
                    formulaColl.Add("y2", "+- y3 0 y1");
                    formulaColl.Add("y4", "+- b 0 y1");
                    formulaColl.Add("dx1", "cos wd2 2700000");
                    formulaColl.Add("dy1", "sin y1 2700000");
                    formulaColl.Add("ir", "+- l dx1 0");
                    formulaColl.Add("it", "+- y1 0 dy1");
                    formulaColl.Add("ib", "+- b dy1 y1");
                    break;
                #endregion
                #region Block Arrows
                case AutoShapeType.RightArrow:
                    formulaColl.Add("maxAdj2", "*/ 100000 w ss");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("dx1", "*/ ss a2 100000");
                    formulaColl.Add("x1", "+- r 0 dx1");
                    formulaColl.Add("dy1", "*/ h a1 200000");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc dy1 0");
                    formulaColl.Add("dx2", "*/ y1 dx1 hd2");
                    formulaColl.Add("x2", "+- x1 dx2 0");
                    break;
                case AutoShapeType.LeftArrow:
                    formulaColl.Add("maxAdj2", "*/ 100000 w ss");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("dx2", "*/ ss a2 100000");
                    formulaColl.Add("x2", "+- l dx2 0");
                    formulaColl.Add("dy1", "*/ h a1 200000");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc dy1 0");
                    formulaColl.Add("dx1", "*/ y1 dx2 hd2");
                    formulaColl.Add("x1", "+- x2  0 dx1");
                    break;
                case AutoShapeType.UpArrow:
                    formulaColl.Add("maxAdj2", "*/ 100000 h ss");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("dy2", "*/ ss a2 100000");
                    formulaColl.Add("y2", "+- t dy2 0");
                    formulaColl.Add("dx1", "*/ w a1 200000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc dx1 0");
                    formulaColl.Add("dy1", "*/ x1 dy2 wd2");
                    formulaColl.Add("y1", "+- y2  0 dy1");
                    break;
                case AutoShapeType.DownArrow:
                    formulaColl.Add("maxAdj2", "*/ 100000 h ss");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("dy1", "*/ ss a2 100000");
                    formulaColl.Add("y1", "+- b 0 dy1");
                    formulaColl.Add("dx1", "*/ w a1 200000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc dx1 0");
                    formulaColl.Add("dy2", "*/ x1 dy1 wd2");
                    formulaColl.Add("y2", "+- y1 dy2 0");
                    break;
                case AutoShapeType.LeftRightArrow:
                    formulaColl.Add("maxAdj2", "*/ 50000 w ss");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("x2", "*/ ss a2 100000");
                    formulaColl.Add("x3", "+- r 0 x2");
                    formulaColl.Add("dy", "*/ h a1 200000");
                    formulaColl.Add("y1", "+- vc 0 dy");
                    formulaColl.Add("y2", "+- vc dy 0");
                    formulaColl.Add("dx1", "*/ y1 x2 hd2");
                    formulaColl.Add("x1", "+- x2 0 dx1");
                    formulaColl.Add("x4", "+- x3 dx1 0");
                    break;
                case AutoShapeType.UpDownArrow:
                    formulaColl.Add("maxAdj2", "*/ 50000 h ss");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("y2", "*/ ss a2 100000");
                    formulaColl.Add("y3", "+- b 0 y2");
                    formulaColl.Add("dx1", "*/ w a1 200000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc dx1 0");
                    formulaColl.Add("dy1", "*/ x1 y2 wd2");
                    formulaColl.Add("y1", "+- y2 0 dy1");
                    formulaColl.Add("y4", "+- y3 dy1 0");
                    break;
                case AutoShapeType.QuadArrow:
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("q1", "+- 100000 0 maxAdj1");
                    formulaColl.Add("maxAdj3", "*/ q1 1 2");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("x1", "*/ ss a3 100000");
                    formulaColl.Add("dx2", "*/ ss a2 100000");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x5", "+- hc dx2 0");
                    formulaColl.Add("dx3", "*/ ss a1 200000");
                    formulaColl.Add("x3", "+- hc 0 dx3");
                    formulaColl.Add("x4", "+- hc dx3 0");
                    formulaColl.Add("x6", "+- r 0 x1");
                    formulaColl.Add("y2", "+- vc 0 dx2");
                    formulaColl.Add("y5", "+- vc dx2 0");
                    formulaColl.Add("y3", "+- vc 0 dx3");
                    formulaColl.Add("y4", "+- vc dx3 0");
                    formulaColl.Add("y6", "+- b 0 x1");
                    formulaColl.Add("il", "*/ dx3 x1 dx2");
                    formulaColl.Add("ir", "+- r 0 il");
                    break;
                case AutoShapeType.LeftRightUpArrow:
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("q1", "+- 100000 0 maxAdj1");
                    formulaColl.Add("maxAdj3", "*/ q1 1 2");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("x1", "*/ ss a3 100000");
                    formulaColl.Add("dx2", "*/ ss a2 100000");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x5", "+- hc dx2 0");
                    formulaColl.Add("dx3", "*/ ss a1 200000");
                    formulaColl.Add("x3", "+- hc 0 dx3");
                    formulaColl.Add("x4", "+- hc dx3 0");
                    formulaColl.Add("x6", "+- r 0 x1");
                    formulaColl.Add("dy2", "*/ ss a2 50000");
                    formulaColl.Add("y2", "+- b 0 dy2");
                    formulaColl.Add("y4", "+- b 0 dx2");
                    formulaColl.Add("y3", "+- y4 0 dx3");
                    formulaColl.Add("y5", "+- y4 dx3 0");
                    formulaColl.Add("il", "*/ dx3 x1 dx2");
                    formulaColl.Add("ir", "+- r 0 il");
                    break;
                case AutoShapeType.BentArrow:
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("a3", "pin 0 adj3 50000");
                    formulaColl.Add("th", "*/ ss a1 100000");
                    formulaColl.Add("aw2", "*/ ss a2 100000");
                    formulaColl.Add("th2", "*/ th 1 2");
                    formulaColl.Add("dh2", "+- aw2 0 th2");
                    formulaColl.Add("ah", "*/ ss a3 100000");
                    formulaColl.Add("bw", "+- r 0 ah");
                    formulaColl.Add("bh", "+- b 0 dh2");
                    formulaColl.Add("bs", "min bw bh");
                    formulaColl.Add("maxAdj4", "*/ 100000 bs ss");
                    formulaColl.Add("a4", "pin 0 adj4 maxAdj4");
                    formulaColl.Add("bd", "*/ ss a4 100000");
                    formulaColl.Add("bd3", "+- bd 0 th");
                    formulaColl.Add("bd2", "max bd3 0");
                    formulaColl.Add("x3", "+- th bd2 0");
                    formulaColl.Add("x4", "+- r 0 ah");
                    formulaColl.Add("y3", "+- dh2 th 0");
                    formulaColl.Add("y4", "+- y3 dh2 0");
                    formulaColl.Add("y5", "+- dh2 bd 0");
                    formulaColl.Add("y6", "+- y3 bd2 0");
                    break;
                case AutoShapeType.UTurnArrow:
                    formulaColl.Add("a2", "pin 0 adj2 25000");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("q2", "*/ a1 ss h");
                    formulaColl.Add("q3", "+- 100000 0 q2");
                    formulaColl.Add("maxAdj3", "*/ q3 h ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("q1", "+- a3 a1 0");
                    formulaColl.Add("minAdj5", "*/ q1 ss h");
                    formulaColl.Add("a5", "pin minAdj5 adj5 100000");
                    formulaColl.Add("th", "*/ ss a1 100000");
                    formulaColl.Add("aw2", "*/ ss a2 100000");
                    formulaColl.Add("th2", "*/ th 1 2");
                    formulaColl.Add("dh2", "+- aw2 0 th2");
                    formulaColl.Add("y5", "*/ h a5 100000");
                    formulaColl.Add("ah", "*/ ss a3 100000");
                    formulaColl.Add("y4", "+- y5 0 ah");
                    formulaColl.Add("x9", "+- r 0 dh2");
                    formulaColl.Add("bw", "*/ x9 1 2");
                    formulaColl.Add("bs", "min bw y4");
                    formulaColl.Add("maxAdj4", "*/ bs 100000 ss");
                    formulaColl.Add("a4", "pin 0 adj4 maxAdj4");
                    formulaColl.Add("bd", "*/ ss a4 100000");
                    formulaColl.Add("bd3", "+- bd 0 th");
                    formulaColl.Add("bd2", "max bd3 0");
                    formulaColl.Add("x3", "+- th bd2 0");
                    formulaColl.Add("x8", "+- r 0 aw2");
                    formulaColl.Add("x6", "+- x8 0 aw2");
                    formulaColl.Add("x7", "+- x6 dh2 0");
                    formulaColl.Add("x4", "+- x9 0 bd");
                    formulaColl.Add("x5", "+- x7 0 bd2");
                    formulaColl.Add("cx", "+/ th x7 2");
                    break;
                case AutoShapeType.LeftUpArrow:
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("maxAdj3", "+- 100000 0 maxAdj1");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("x1", "*/ ss a3 100000");
                    formulaColl.Add("dx2", "*/ ss a2 50000");
                    formulaColl.Add("x2", "+- r 0 dx2");
                    formulaColl.Add("y2", "+- b 0 dx2");
                    formulaColl.Add("dx4", "*/ ss a2 100000");
                    formulaColl.Add("x4", "+- r 0 dx4");
                    formulaColl.Add("y4", "+- b 0 dx4");
                    formulaColl.Add("dx3", "*/ ss a1 200000");
                    formulaColl.Add("x3", "+- x4 0 dx3");
                    formulaColl.Add("x5", "+- x4 dx3 0");
                    formulaColl.Add("y3", "+- y4 0 dx3");
                    formulaColl.Add("y5", "+- y4 dx3 0");
                    formulaColl.Add("il", "*/ dx3 x1 dx4");
                    formulaColl.Add("cx1", "+/ x1 x5 2");
                    formulaColl.Add("cy1", "+/ x1 y5 2");
                    break;
                case AutoShapeType.BentUpArrow:
                    formulaColl.Add("a1", "pin 0 adj1 50000");
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("a3", "pin 0 adj3 50000");
                    formulaColl.Add("y1", "*/ ss a3 100000");
                    formulaColl.Add("dx1", "*/ ss a2 50000");
                    formulaColl.Add("x1", "+- r 0 dx1");
                    formulaColl.Add("dx3", "*/ ss a2 100000");
                    formulaColl.Add("x3", "+- r 0 dx3");
                    formulaColl.Add("dx2", "*/ ss a1 200000");
                    formulaColl.Add("x2", "+- x3 0 dx2");
                    formulaColl.Add("x4", "+- x3 dx2 0");
                    formulaColl.Add("dy2", "*/ ss a1 100000");
                    formulaColl.Add("y2", "+- b 0 dy2");
                    formulaColl.Add("x0", "*/ x4 1 2");
                    formulaColl.Add("y3", "+/ y2 b 2");
                    formulaColl.Add("y15", "+/ y1 b 2");
                    break;
                case AutoShapeType.CurvedRightArrow:
                    formulaColl.Add("maxAdj2", "*/ 50000 h ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("a1", "pin 0 adj1 a2");
                    formulaColl.Add("th", "*/ ss a1 100000");
                    formulaColl.Add("aw", "*/ ss a2 100000");
                    formulaColl.Add("q1", "+/ th aw 4");
                    formulaColl.Add("hR", "+- hd2 0 q1");
                    formulaColl.Add("q7", "*/ hR 2 1");
                    formulaColl.Add("q8", "*/ q7 q7 1");
                    formulaColl.Add("q9", "*/ th th 1");
                    formulaColl.Add("q10", "+- q8 0 q9");
                    formulaColl.Add("q11", "sqrt q10");
                    formulaColl.Add("idx", "*/ q11 w q7");
                    formulaColl.Add("maxAdj3", "*/ 100000 idx ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("ah", "*/ ss a3 100000");
                    formulaColl.Add("y3", "+- hR th 0");
                    formulaColl.Add("q2", "*/ w w 1");
                    formulaColl.Add("q3", "*/ ah ah 1");
                    formulaColl.Add("q4", "+- q2 0 q3");
                    formulaColl.Add("q5", "sqrt q4");
                    formulaColl.Add("dy", "*/ q5 hR w");
                    formulaColl.Add("y5", "+- hR dy 0");
                    formulaColl.Add("y7", "+- y3 dy 0");
                    formulaColl.Add("q6", "+- aw 0 th");
                    formulaColl.Add("dh", "*/ q6 1 2");
                    formulaColl.Add("y4", "+- y5 0 dh");
                    formulaColl.Add("y8", "+- y7 dh 0");
                    formulaColl.Add("aw2", "*/ aw 1 2");
                    formulaColl.Add("y6", "+- b 0 aw2");
                    formulaColl.Add("x1", "+- r 0 ah");
                    formulaColl.Add("swAng0", "at2 ah dy");
                    formulaColl.Add("swAng", "*/ swAng0 180 " + Math.PI.ToString());
                    formulaColl.Add("stAng", "+- cd2 0 swAng");
                    formulaColl.Add("mswAng", "+- 0 0 swAng");
                    formulaColl.Add("ix", "+- r 0 idx");
                    formulaColl.Add("iy", "+/ hR y3 2");
                    formulaColl.Add("q12", "*/ th 1 2");
                    formulaColl.Add("dang0", "at2 idx q12");
                    formulaColl.Add("dang2", "*/ dang0 180 " + Math.PI.ToString());
                    formulaColl.Add("swAng2", "+- dang2 0 cd4");
                    formulaColl.Add("swAng3", "+- cd4 dang2 0");
                    formulaColl.Add("stAng3", "+- cd2 0 dang2");
                    break;
                case AutoShapeType.CurvedLeftArrow:
                    formulaColl.Add("maxAdj2", "*/ 50000 h ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("a1", "pin 0 adj1 a2");
                    formulaColl.Add("th", "*/ ss a1 100000");
                    formulaColl.Add("aw", "*/ ss a2 100000");
                    formulaColl.Add("q1", "+/ th aw 4");
                    formulaColl.Add("hR", "+- hd2 0 q1");
                    formulaColl.Add("q7", "*/ hR 2 1");
                    formulaColl.Add("q8", "*/ q7 q7 1");
                    formulaColl.Add("q9", "*/ th th 1");
                    formulaColl.Add("q10", "+- q8 0 q9");
                    formulaColl.Add("q11", "sqrt q10");
                    formulaColl.Add("idx", "*/ q11 w q7");
                    formulaColl.Add("maxAdj3", "*/ 100000 idx ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("ah", "*/ ss a3 100000");
                    formulaColl.Add("y3", "+- hR th 0");
                    formulaColl.Add("q2", "*/ w w 1");
                    formulaColl.Add("q3", "*/ ah ah 1");
                    formulaColl.Add("q4", "+- q2 0 q3");
                    formulaColl.Add("q5", "sqrt q4");
                    formulaColl.Add("dy", "*/ q5 hR w");
                    formulaColl.Add("y5", "+- hR dy 0");
                    formulaColl.Add("y7", "+- y3 dy 0");
                    formulaColl.Add("q6", "+- aw 0 th");
                    formulaColl.Add("dh", "*/ q6 1 2");
                    formulaColl.Add("y4", "+- y5 0 dh");
                    formulaColl.Add("y8", "+- y7 dh 0");
                    formulaColl.Add("aw2", "*/ aw 1 2");
                    formulaColl.Add("y6", "+- b 0 aw2");
                    formulaColl.Add("x1", "+- l ah 0");
                    formulaColl.Add("swAng1", "at2 ah dy");
                    formulaColl.Add("swAng", "*/ swAng1 180 " + Math.PI.ToString());
                    formulaColl.Add("mswAng", "+- 0 0 swAng");
                    formulaColl.Add("ix", "+- l idx 0");
                    formulaColl.Add("iy", "+/ hR y3 2");
                    formulaColl.Add("q12", "*/ th 1 2");
                    formulaColl.Add("dang3", "at2 idx q12");
                    formulaColl.Add("dang2", "*/ dang3 180 " + Math.PI.ToString());
                    formulaColl.Add("swAng2", "+- dang2 0 swAng");
                    formulaColl.Add("swAng3", "+- swAng dang2 0");
                    formulaColl.Add("stAng3", "+- 0 0 dang2");
                    break;
                case AutoShapeType.CurvedUpArrow:
                    formulaColl.Add("maxAdj2", "*/ 50000 w ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("th", "*/ ss a1 100000");
                    formulaColl.Add("aw", "*/ ss a2 100000");
                    formulaColl.Add("q1", "+/ th aw 4");
                    formulaColl.Add("wR", "+- wd2 0 q1");
                    formulaColl.Add("q7", "*/ wR 2 1");
                    formulaColl.Add("q8", "*/ q7 q7 1");
                    formulaColl.Add("q9", "*/ th th 1");
                    formulaColl.Add("q10", "+- q8 0 q9");
                    formulaColl.Add("q11", "sqrt q10");
                    formulaColl.Add("idy", "*/ q11 h q7");
                    formulaColl.Add("maxAdj3", "*/ 100000 idy ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("ah", "*/ ss adj3 100000");
                    formulaColl.Add("x3", "+- wR th 0");
                    formulaColl.Add("q2", "*/ h h 1");
                    formulaColl.Add("q3", "*/ ah ah 1");
                    formulaColl.Add("q4", "+- q2 0 q3");
                    formulaColl.Add("q5", "sqrt q4");
                    formulaColl.Add("dx", "*/ q5 wR h");
                    formulaColl.Add("x5", "+- wR dx 0");
                    formulaColl.Add("x7", "+- x3 dx 0");
                    formulaColl.Add("q6", "+- aw 0 th");
                    formulaColl.Add("dh", "*/ q6 1 2");
                    formulaColl.Add("x4", "+- x5 0 dh");
                    formulaColl.Add("x8", "+- x7 dh 0");
                    formulaColl.Add("aw2", "*/ aw 1 2");
                    formulaColl.Add("x6", "+- r 0 aw2");
                    formulaColl.Add("y1", "+- t ah 0");
                    formulaColl.Add("swAng0", "at2 ah dx");
                    formulaColl.Add("swAng", "*/ swAng0 180 " + Math.PI.ToString());
                    formulaColl.Add("mswAng", "+- 0 0 swAng");
                    formulaColl.Add("iy", "+- t idy 0");
                    formulaColl.Add("ix", "+/ wR x3 2");
                    formulaColl.Add("q12", "*/ th 1 2");
                    formulaColl.Add("dang0", "at2 idy q12");
                    formulaColl.Add("dang2", "*/ dang0 180 " + Math.PI.ToString());
                    formulaColl.Add("swAng2", "+- dang2 0 swAng");
                    formulaColl.Add("mswAng2", "+- 0 0 swAng2");
                    formulaColl.Add("stAng3", "+- cd4 0 swAng");
                    formulaColl.Add("swAng3", "+- swAng dang2 0");
                    formulaColl.Add("stAng2", "+- cd4 0 dang2");
                    break;
                case AutoShapeType.CurvedDownArrow:
                    formulaColl.Add("maxAdj2", "*/ 50000 w ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("th", "*/ ss a1 100000");
                    formulaColl.Add("aw", "*/ ss a2 100000");
                    formulaColl.Add("q1", "+/ th aw 4");
                    formulaColl.Add("wR", "+- wd2 0 q1");
                    formulaColl.Add("q7", "*/ wR 2 1");
                    formulaColl.Add("q8", "*/ q7 q7 1");
                    formulaColl.Add("q9", "*/ th th 1");
                    formulaColl.Add("q10", "+- q8 0 q9");
                    formulaColl.Add("q11", "sqrt q10");
                    formulaColl.Add("idy", "*/ q11 h q7");
                    formulaColl.Add("maxAdj3", "*/ 100000 idy ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("ah", "*/ ss adj3 100000");
                    formulaColl.Add("x3", "+- wR th 0");
                    formulaColl.Add("q2", "*/ h h 1");
                    formulaColl.Add("q3", "*/ ah ah 1");
                    formulaColl.Add("q4", "+- q2 0 q3");
                    formulaColl.Add("q5", "sqrt q4");
                    formulaColl.Add("dx", "*/ q5 wR h");
                    formulaColl.Add("x5", "+- wR dx 0");
                    formulaColl.Add("x7", "+- x3 dx 0");
                    formulaColl.Add("q6", "+- aw 0 th");
                    formulaColl.Add("dh", "*/ q6 1 2");
                    formulaColl.Add("x4", "+- x5 0 dh");
                    formulaColl.Add("x8", "+- x7 dh 0");
                    formulaColl.Add("aw2", "*/ aw 1 2");
                    formulaColl.Add("x6", "+- r 0 aw2");
                    formulaColl.Add("y1", "+- b 0 ah");
                    formulaColl.Add("swAng0", "at2 ah dx");
                    formulaColl.Add("swAng", "*/ swAng0 180 " + Math.PI.ToString());
                    formulaColl.Add("mswAng", "+- 0 0 swAng");
                    formulaColl.Add("iy", "+- b 0 idy");
                    formulaColl.Add("ix", "+/ wR x3 2");
                    formulaColl.Add("q12", "*/ th 1 2");
                    formulaColl.Add("dang0", "at2 idy q12");
                    formulaColl.Add("dang2", "*/ dang0 180 " + Math.PI.ToString());
                    formulaColl.Add("stAng", "+- 3cd4 swAng 0");
                    formulaColl.Add("stAng2", "+- 3cd4 0 dang2");
                    formulaColl.Add("swAng2", "+- dang2 0 cd4");
                    formulaColl.Add("swAng3", "+- cd4 dang2 0");
                    break;
                case AutoShapeType.StripedRightArrow:
                    formulaColl.Add("maxAdj2", "*/ 84375 w ss");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("x4", "*/ ss 5 32");
                    formulaColl.Add("dx5", "*/ ss a2 100000");
                    formulaColl.Add("x5", "+- r 0 dx5");
                    formulaColl.Add("dy1", "*/ h a1 200000");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc dy1 0");
                    formulaColl.Add("dx6", "*/ dy1 dx5 hd2");
                    formulaColl.Add("x6", "+- r 0 dx6");
                    break;
                case AutoShapeType.NotchedRightArrow:
                    formulaColl.Add("maxAdj2", "*/ 100000 w ss");
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("dx2", "*/ ss a2 100000");
                    formulaColl.Add("x2", "+- r 0 dx2");
                    formulaColl.Add("dy1", "*/ h a1 200000");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc dy1 0");
                    formulaColl.Add("x1", "*/ dy1 dx2 hd2");
                    formulaColl.Add("x3", "+- r 0 x1");
                    break;
                case AutoShapeType.Pentagon:
                    formulaColl.Add("maxAdj", "*/ 100000 w ss");
                    formulaColl.Add("a", "pin 0 adj maxAdj");
                    formulaColl.Add("dx1", "*/ ss a 100000");
                    formulaColl.Add("x1", "+- r 0 dx1");
                    formulaColl.Add("ir", "+/ x1 r 2");
                    formulaColl.Add("x2", "*/ x1 1 2");
                    break;
                case AutoShapeType.Chevron:
                    formulaColl.Add("maxAdj", "*/ 100000 w ss");
                    formulaColl.Add("a", "pin 0 adj maxAdj");
                    formulaColl.Add("x1", "*/ ss a 100000");
                    formulaColl.Add("x2", "+- r 0 x1");
                    formulaColl.Add("x3", "*/ x2 1 2");
                    formulaColl.Add("dx", "+- x2 0 x1");
                    formulaColl.Add("il", "?: dx x1 l");
                    formulaColl.Add("ir", "?: dx x2 r");
                    break;
                case AutoShapeType.LeftArrowCallout:
                    formulaColl.Add("maxAdj2", "*/ 50000 h ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("maxAdj3", "*/ 100000 w ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("q2", "*/ a3 ss w");
                    formulaColl.Add("maxAdj4", "+- 100000 0 q2");
                    formulaColl.Add("a4", "pin 0 adj4 maxAdj4");
                    formulaColl.Add("dy1", "*/ ss a2 100000");
                    formulaColl.Add("dy2", "*/ ss a1 200000");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc dy2 0");
                    formulaColl.Add("y4", "+- vc dy1 0");
                    formulaColl.Add("x1", "*/ ss a3 100000");
                    formulaColl.Add("dx2", "*/ w a4 100000");
                    formulaColl.Add("x2", "+- r 0 dx2");
                    formulaColl.Add("x3", "+/ x2 r 2");
                    break;
                case AutoShapeType.RightArrowCallout:
                    formulaColl.Add("maxAdj2", "*/ 50000 h ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("maxAdj3", "*/ 100000 w ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("q2", "*/ a3 ss w");
                    formulaColl.Add("maxAdj4", "+- 100000 0 q2");
                    formulaColl.Add("a4", "pin 0 adj4 maxAdj4");
                    formulaColl.Add("dy1", "*/ ss a2 100000");
                    formulaColl.Add("dy2", "*/ ss a1 200000");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc dy2 0");
                    formulaColl.Add("y4", "+- vc dy1 0");
                    formulaColl.Add("dx3", "*/ ss a3 100000");
                    formulaColl.Add("x3", "+- r 0 dx3");
                    formulaColl.Add("x2", "*/ w a4 100000");
                    formulaColl.Add("x1", "*/ x2 1 2");
                    break;
                case AutoShapeType.DownArrowCallout:
                    formulaColl.Add("maxAdj2", "*/ 50000 w ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("maxAdj3", "*/ 100000 h ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("q2", "*/ a3 ss h");
                    formulaColl.Add("maxAdj4", "+- 100000 0 q2");
                    formulaColl.Add("a4", "pin 0 adj4 maxAdj4");
                    formulaColl.Add("dx1", "*/ ss a2 100000");
                    formulaColl.Add("dx2", "*/ ss a1 200000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("dy3", "*/ ss a3 100000");
                    formulaColl.Add("y3", "+- b 0 dy3");
                    formulaColl.Add("y2", "*/ h a4 100000");
                    formulaColl.Add("y1", "*/ y2 1 2");
                    break;
                case AutoShapeType.UpArrowCallout:
                    formulaColl.Add("maxAdj2", "*/ 50000 w ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("maxAdj3", "*/ 100000 h ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("q2", "*/ a3 ss h");
                    formulaColl.Add("maxAdj4", "+- 100000 0 q2");
                    formulaColl.Add("a4", "pin 0 adj4 maxAdj4");
                    formulaColl.Add("dx1", "*/ ss a2 100000");
                    formulaColl.Add("dx2", "*/ ss a1 200000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("y1", "*/ ss a3 100000");
                    formulaColl.Add("dy2", "*/ h a4 100000");
                    formulaColl.Add("y2", "+- b 0 dy2");
                    formulaColl.Add("y3", "+/ y2 b 2");
                    break;
                case AutoShapeType.LeftRightArrowCallout:
                    formulaColl.Add("maxAdj2", "*/ 50000 h ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("maxAdj3", "*/ 50000 w ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("q2", "*/ a3 ss wd2");
                    formulaColl.Add("maxAdj4", "+- 100000 0 q2");
                    formulaColl.Add("a4", "pin 0 adj4 maxAdj4");
                    formulaColl.Add("dy1", "*/ ss a2 100000");
                    formulaColl.Add("dy2", "*/ ss a1 200000");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc dy2 0");
                    formulaColl.Add("y4", "+- vc dy1 0");
                    formulaColl.Add("x1", "*/ ss a3 100000");
                    formulaColl.Add("x4", "+- r 0 x1");
                    formulaColl.Add("dx2", "*/ w a4 200000");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    break;
                case AutoShapeType.UpDownArrowCallout:
                    formulaColl.Add("maxAdj2", "*/ 50000 w ss");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("maxAdj3", "*/ 50000 h ss");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("q2", "*/ a3 ss hd2");
                    formulaColl.Add("maxAdj4", "+- 100000 0 q2");
                    formulaColl.Add("a4", "pin 0 adj4 maxAdj4");
                    formulaColl.Add("dx1", "*/ ss a2 100000");
                    formulaColl.Add("dx2", "*/ ss a1 200000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("y1", "*/ ss a3 100000");
                    formulaColl.Add("y4", "+- b 0 y1");
                    formulaColl.Add("dy2", "*/ h a4 200000");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc dy2 0");
                    break;
                case AutoShapeType.QuadArrowCallout:
                    formulaColl.Add("a2", "pin 0 adj2 50000");
                    formulaColl.Add("maxAdj1", "*/ a2 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("maxAdj3", "+- 50000 0 a2");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("q2", "*/ a3 2 1");
                    formulaColl.Add("maxAdj4", "+- 100000 0 q2");
                    formulaColl.Add("a4", "pin a1 adj4 maxAdj4");
                    formulaColl.Add("dx2", "*/ ss a2 100000");
                    formulaColl.Add("dx3", "*/ ss a1 200000");
                    formulaColl.Add("ah", "*/ ss a3 100000");
                    formulaColl.Add("dx1", "*/ w a4 200000");
                    formulaColl.Add("dy1", "*/ h a4 200000");
                    formulaColl.Add("x8", "+- r 0 ah");
                    formulaColl.Add("x2", "+- hc 0 dx1");
                    formulaColl.Add("x7", "+- hc dx1 0");
                    formulaColl.Add("x3", "+- hc 0 dx2");
                    formulaColl.Add("x6", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc 0 dx3");
                    formulaColl.Add("x5", "+- hc dx3 0");
                    formulaColl.Add("y8", "+- b 0 ah");
                    formulaColl.Add("y2", "+- vc 0 dy1");
                    formulaColl.Add("y7", "+- vc dy1 0");
                    formulaColl.Add("y3", "+- vc 0 dx2");
                    formulaColl.Add("y6", "+- vc dx2 0");
                    formulaColl.Add("y4", "+- vc 0 dx3");
                    formulaColl.Add("y5", "+- vc dx3 0");
                    break;
                case AutoShapeType.CircularArrow:
                    formulaColl.Add("a5", "pin 0 adj5 25000");
                    formulaColl.Add("maxAdj1", "*/ a5 2 1");
                    formulaColl.Add("a1", "pin 0 adj1 maxAdj1");
                    formulaColl.Add("enAng", "pin 1 adj3 360");
                    formulaColl.Add("stAng", "pin 0 adj4 360");
                    formulaColl.Add("th", "*/ ss a1 100000");
                    formulaColl.Add("thh", "*/ ss a5 100000");
                    formulaColl.Add("th2", "*/ th 1 2");
                    formulaColl.Add("rw1", "+- wd2 th2 thh");
                    formulaColl.Add("rh1", "+- hd2 th2 thh");
                    formulaColl.Add("rw2", "+- rw1 0 th");
                    formulaColl.Add("rh2", "+- rh1 0 th");
                    formulaColl.Add("rw3", "+- rw2 th2 0");
                    formulaColl.Add("rh3", "+- rh2 th2 0");
                    formulaColl.Add("wtH", "sin rw3 enAng");
                    formulaColl.Add("htH", "cos rh3 enAng");
                    formulaColl.Add("dxH", "cat2 rw3 htH wtH");
                    formulaColl.Add("dyH", "sat2 rh3 htH wtH");
                    formulaColl.Add("xH", "+- hc dxH 0");
                    formulaColl.Add("yH", "+- vc dyH 0");
                    formulaColl.Add("rI", "min rw2 rh2");
                    formulaColl.Add("u1", "*/ dxH dxH 1");
                    formulaColl.Add("u2", "*/ dyH dyH 1");
                    formulaColl.Add("u3", "*/ rI rI 1");
                    formulaColl.Add("u4", "+- u1 0 u3");
                    formulaColl.Add("u5", "+- u2 0 u3");
                    formulaColl.Add("u6", "*/ u4 u5 u1");
                    formulaColl.Add("u7", "*/ u6 1 u2");
                    formulaColl.Add("u8", "+- 1 0 u7");
                    formulaColl.Add("u9", "sqrt u8");
                    formulaColl.Add("u10", "*/ u4 1 dxH");
                    formulaColl.Add("u11", "*/ u10 1 dyH");
                    formulaColl.Add("u12", "+/ 1 u9 u11");
                    formulaColl.Add("u0", "at2 1 u12");
                    formulaColl.Add("u13", "*/ u0 180 " + Math.PI.ToString());
                    formulaColl.Add("u14", "+- u13 360 0");
                    formulaColl.Add("u15", "?: u13 u13 u14");
                    formulaColl.Add("u16", "+- u15 0 enAng");
                    formulaColl.Add("u17", "+- u16 360 0");
                    formulaColl.Add("u18", "?: u16 u16 u17");
                    formulaColl.Add("u19", "+- u18 0 cd2");
                    formulaColl.Add("u20", "+- u18 0 360");
                    formulaColl.Add("u21", "?: u19 u20 u18");
                    formulaColl.Add("maxAng", "abs u21");
                    formulaColl.Add("aAng", "pin 0 adj2 maxAng");
                    formulaColl.Add("ptAng", "+- enAng aAng 0");
                    formulaColl.Add("wtA", "sin rw3 ptAng");
                    formulaColl.Add("htA", "cos rh3 ptAng");
                    formulaColl.Add("dxA", "cat2 rw3 htA wtA");
                    formulaColl.Add("dyA", "sat2 rh3 htA wtA");
                    formulaColl.Add("xA", "+- hc dxA 0");
                    formulaColl.Add("yA", "+- vc dyA 0");
                    formulaColl.Add("wtE", "sin rw1 stAng");
                    formulaColl.Add("htE", "cos rh1 stAng");
                    formulaColl.Add("dxE", "cat2 rw1 htE wtE");
                    formulaColl.Add("dyE", "sat2 rh1 htE wtE");
                    formulaColl.Add("xE", "+- hc dxE 0");
                    formulaColl.Add("yE", "+- vc dyE 0");
                    formulaColl.Add("dxG", "cos thh ptAng");
                    formulaColl.Add("dyG", "sin thh ptAng");
                    formulaColl.Add("xG", "+- xH dxG 0");
                    formulaColl.Add("yG", "+- yH dyG 0");
                    formulaColl.Add("dxB", "cos thh ptAng");
                    formulaColl.Add("dyB", "sin thh ptAng");
                    formulaColl.Add("xB", "+- xH 0 dxB 0");
                    formulaColl.Add("yB", "+- yH 0 dyB 0");
                    formulaColl.Add("sx1", "+- xB 0 hc");
                    formulaColl.Add("sy1", "+- yB 0 vc");
                    formulaColl.Add("sx2", "+- xG 0 hc");
                    formulaColl.Add("sy2", "+- yG 0 vc");
                    formulaColl.Add("rO", "min rw1 rh1");
                    formulaColl.Add("x1O", "*/ sx1 rO rw1");
                    formulaColl.Add("y1O", "*/ sy1 rO rh1");
                    formulaColl.Add("x2O", "*/ sx2 rO rw1");
                    formulaColl.Add("y2O", "*/ sy2 rO rh1");
                    formulaColl.Add("dxO", "+- x2O 0 x1O");
                    formulaColl.Add("dyO", "+- y2O 0 y1O");
                    formulaColl.Add("dO", "mod dxO dyO 0");
                    formulaColl.Add("q1", "*/ x1O y2O 1");
                    formulaColl.Add("q2", "*/ x2O y1O 1");
                    formulaColl.Add("DO", "+- q1 0 q2");
                    formulaColl.Add("q3", "*/ rO rO 1");
                    formulaColl.Add("q4", "*/ dO dO 1");
                    formulaColl.Add("q5", "*/ q3 q4 1");
                    formulaColl.Add("q6", "*/ DO DO 1");
                    formulaColl.Add("q7", "+- q5 0 q6");
                    formulaColl.Add("q8", "max q7 0");
                    formulaColl.Add("sdelO", "sqrt q8");
                    formulaColl.Add("ndyO", "*/ dyO -1 1");
                    formulaColl.Add("sdyO", "?: ndyO -1 1");
                    formulaColl.Add("q9", "*/ sdyO dxO 1");
                    formulaColl.Add("q10", "*/ q9 sdelO 1");
                    formulaColl.Add("q11", "*/ DO dyO 1");
                    formulaColl.Add("dxF1", "+/ q11 q10 q4");
                    formulaColl.Add("q12", "+- q11 0 q10");
                    formulaColl.Add("dxF2", "*/ q12 1 q4");
                    formulaColl.Add("adyO", "abs dyO");
                    formulaColl.Add("q13", "*/ adyO sdelO 1");
                    formulaColl.Add("q14", "*/ DO dxO -1");
                    formulaColl.Add("dyF1", "+/ q14 q13 q4");
                    formulaColl.Add("q15", "+- q14 0 q13");
                    formulaColl.Add("dyF2", "*/ q15 1 q4");
                    formulaColl.Add("q16", "+- x2O 0 dxF1");
                    formulaColl.Add("q17", "+- x2O 0 dxF2");
                    formulaColl.Add("q18", "+- y2O 0 dyF1");
                    formulaColl.Add("q19", "+- y2O 0 dyF2");
                    formulaColl.Add("q20", "mod q16 q18 0");
                    formulaColl.Add("q21", "mod q17 q19 0");
                    formulaColl.Add("q22", "+- q21 0 q20");
                    formulaColl.Add("dxF", "?: q22 dxF1 dxF2");
                    formulaColl.Add("dyF", "?: q22 dyF1 dyF2");
                    formulaColl.Add("sdxF", "*/ dxF rw1 rO");
                    formulaColl.Add("sdyF", "*/ dyF rh1 rO");
                    formulaColl.Add("xF", "+- hc sdxF 0");
                    formulaColl.Add("yF", "+- vc sdyF 0");
                    formulaColl.Add("x1I", "*/ sx1 rI rw2");
                    formulaColl.Add("y1I", "*/ sy1 rI rh2");
                    formulaColl.Add("x2I", "*/ sx2 rI rw2");
                    formulaColl.Add("y2I", "*/ sy2 rI rh2");
                    formulaColl.Add("dxI1", "+- x2I 0 x1I");
                    formulaColl.Add("dyI1", "+- y2I 0 y1I");
                    formulaColl.Add("dI", "mod dxI1 dyI1 0");
                    formulaColl.Add("v1", "*/ x1I y2I 1");
                    formulaColl.Add("v2", "*/ x2I y1I 1");
                    formulaColl.Add("DI", "+- v1 0 v2");
                    formulaColl.Add("v3", "*/ rI rI 1");
                    formulaColl.Add("v4", "*/ dI dI 1");
                    formulaColl.Add("v5", "*/ v3 v4 1");
                    formulaColl.Add("v6", "*/ DI DI 1");
                    formulaColl.Add("v7", "+- v5 0 v6");
                    formulaColl.Add("v8", "max v7 0");
                    formulaColl.Add("sdelI", "sqrt v8");
                    formulaColl.Add("v9", "*/ sdyO dxI1 1");
                    formulaColl.Add("v10", "*/ v9 sdelI 1");
                    formulaColl.Add("v11", "*/ DI dyI1 1");
                    formulaColl.Add("dxC1", "+/ v11 v10 v4");
                    formulaColl.Add("v12", "+- v11 0 v10");
                    formulaColl.Add("dxC2", "*/ v12 1 v4");
                    formulaColl.Add("adyI", "abs dyI1");
                    formulaColl.Add("v13", "*/ adyI sdelI 1");
                    formulaColl.Add("v14", "*/ DI dxI1 -1");
                    formulaColl.Add("dyC1", "+/ v14 v13 v4");
                    formulaColl.Add("v15", "+- v14 0 v13");
                    formulaColl.Add("dyC2", "*/ v15 1 v4");
                    formulaColl.Add("v16", "+- x1I 0 dxC1");
                    formulaColl.Add("v17", "+- x1I 0 dxC2");
                    formulaColl.Add("v18", "+- y1I 0 dyC1");
                    formulaColl.Add("v19", "+- y1I 0 dyC2");
                    formulaColl.Add("v20", "mod v16 v18 0");
                    formulaColl.Add("v21", "mod v17 v19 0");
                    formulaColl.Add("v22", "+- v21 0 v20");
                    formulaColl.Add("dxC", "?: v22 dxC1 dxC2");
                    formulaColl.Add("dyC", "?: v22 dyC1 dyC2");
                    formulaColl.Add("sdxC", "*/ dxC rw2 rI");
                    formulaColl.Add("sdyC", "*/ dyC rh2 rI");
                    formulaColl.Add("xC", "+- hc sdxC 0");
                    formulaColl.Add("yC", "+- vc sdyC 0");
                    formulaColl.Add("ist00", "at2 sdxC sdyC");
                    formulaColl.Add("ist0", "*/ ist00 180 " + Math.PI.ToString());
                    formulaColl.Add("ist1", "+- ist0 360 0");
                    formulaColl.Add("istAng", "?: ist0 ist0 ist1");
                    formulaColl.Add("isw1", "+- stAng 0 istAng");
                    formulaColl.Add("isw2", "+- isw1 0 360");
                    formulaColl.Add("iswAng", "?: isw1 isw2 isw1");
                    formulaColl.Add("p1", "+- xF 0 xC");
                    formulaColl.Add("p2", "+- yF 0 yC");
                    formulaColl.Add("p3", "mod p1 p2 0");
                    formulaColl.Add("p4", "*/ p3 1 2");
                    formulaColl.Add("p5", "+- p4 0 thh");
                    formulaColl.Add("xGp", "?: p5 xF xG");
                    formulaColl.Add("yGp", "?: p5 yF yG");
                    formulaColl.Add("xBp", "?: p5 xC xB");
                    formulaColl.Add("yBp", "?: p5 yC yB");
                    formulaColl.Add("en00", "at2 sdxF sdyF");
                    formulaColl.Add("en0", "*/ en00 180 " + Math.PI.ToString());
                    formulaColl.Add("en1", "+- en0 360 0");
                    formulaColl.Add("en2", "?: en0 en0 en1");
                    formulaColl.Add("sw0", "+- en2 0 stAng");
                    formulaColl.Add("sw1", "+- sw0 360 0");
                    formulaColl.Add("swAng", "?: sw0 sw0 sw1");
                    formulaColl.Add("wtI", "sin rw3 stAng");
                    formulaColl.Add("htI", "cos rh3 stAng");
                    formulaColl.Add("dxI", "cat2 rw3 htI wtI");
                    formulaColl.Add("dyI", "sat2 rh3 htI wtI");
                    formulaColl.Add("xI", "+- hc dxI 0");
                    formulaColl.Add("yI", "+- vc dyI 0");
                    formulaColl.Add("aI", "+- stAng 0 cd4");
                    formulaColl.Add("aA", "+- ptAng cd4 0");
                    formulaColl.Add("aB", "+- ptAng cd2 0");
                    formulaColl.Add("idx", "cos rw1 45");
                    formulaColl.Add("idy", "sin rh1 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                #endregion
                #region Equation Shapes
                case AutoShapeType.MathPlus:
                    formulaColl.Add("a1", "pin 0 adj1 73490");
                    formulaColl.Add("dx1", "*/ w 73490 200000");
                    formulaColl.Add("dy1", "*/ h 73490 200000");
                    formulaColl.Add("dx2", "*/ ss a1 200000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc 0 dx2");
                    formulaColl.Add("y3", "+- vc dx2 0");
                    formulaColl.Add("y4", "+- vc dy1 0");
                    break;
                case AutoShapeType.MathMinus:
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("dy1", "*/ h a1 200000");
                    formulaColl.Add("dx1", "*/ w 73490 200000");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc dy1 0");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc dx1 0");
                    break;
                case AutoShapeType.MathMultiply:
                    formulaColl.Add("a1", "pin 0 adj1 51965");
                    formulaColl.Add("th", "*/ ss a1 100000");
                    formulaColl.Add("a0", "at2 w h");
                    formulaColl.Add("a", "*/ a0 180 " + Math.PI.ToString());
                    formulaColl.Add("sa", "sin 1 a");
                    formulaColl.Add("ca", "cos 1 a");
                    formulaColl.Add("ta", "tan 1 a");
                    formulaColl.Add("dl", "mod w h 0");
                    formulaColl.Add("rw", "*/ dl 51965 100000");
                    formulaColl.Add("lM", "+- dl 0 rw");
                    formulaColl.Add("xM", "*/ ca lM 2");
                    formulaColl.Add("yM", "*/ sa lM 2");
                    formulaColl.Add("dxAM", "*/ sa th 2");
                    formulaColl.Add("dyAM", "*/ ca th 2");
                    formulaColl.Add("xA", "+- xM 0 dxAM");
                    formulaColl.Add("yA", "+- yM dyAM 0");
                    formulaColl.Add("xB", "+- xM dxAM 0");
                    formulaColl.Add("yB", "+- yM 0 dyAM");
                    formulaColl.Add("xBC", "+- hc 0 xB");
                    formulaColl.Add("yBC", "*/ xBC ta 1");
                    formulaColl.Add("yC", "+- yBC yB 0");
                    formulaColl.Add("xD", "+- r 0 xB");
                    formulaColl.Add("xE", "+- r 0 xA");
                    formulaColl.Add("yFE", "+- vc 0 yA");
                    formulaColl.Add("xFE", "*/ yFE 1 ta");
                    formulaColl.Add("xF", "+- xE 0 xFE");
                    formulaColl.Add("xL", "+- xA xFE 0");
                    formulaColl.Add("yG", "+- b 0 yA");
                    formulaColl.Add("yH", "+- b 0 yB");
                    formulaColl.Add("yI", "+- b 0 yC");
                    formulaColl.Add("xC2", "+- r 0 xM");
                    formulaColl.Add("yC3", "+- b 0 yM");
                    break;
                case AutoShapeType.MathDivision:
                    formulaColl.Add("a1", "pin 1000 adj1 36745");
                    formulaColl.Add("ma1", "+- 0 0 a1");
                    formulaColl.Add("ma3h", "+/ 73490 ma1 4");
                    formulaColl.Add("ma3w", "*/ 36745 w h");
                    formulaColl.Add("maxAdj3", "min ma3h ma3w");
                    formulaColl.Add("a3", "pin 1000 adj3 maxAdj3");
                    formulaColl.Add("m4a3", "*/ -4 a3 1");
                    formulaColl.Add("maxAdj2", "+- 73490 m4a3 a1");
                    formulaColl.Add("a2", "pin 0 adj2 maxAdj2");
                    formulaColl.Add("dy1", "*/ h a1 200000");
                    formulaColl.Add("yg", "*/ h a2 100000");
                    formulaColl.Add("rad", "*/ h a3 100000");
                    formulaColl.Add("dx1", "*/ w 73490 200000");
                    formulaColl.Add("y3", "+- vc 0 dy1");
                    formulaColl.Add("y4", "+- vc dy1 0");
                    formulaColl.Add("a", "+- yg rad 0");
                    formulaColl.Add("y2", "+- y3 0 a");
                    formulaColl.Add("y1", "+- y2 0 rad");
                    formulaColl.Add("y5", "+- b 0 y1");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x3", "+- hc dx1 0");
                    formulaColl.Add("x2", "+- hc 0 rad");
                    break;
                case AutoShapeType.MathEqual:
                    formulaColl.Add("a1", "pin 0 adj1 36745");
                    formulaColl.Add("2a1", "*/ a1 2 1");
                    formulaColl.Add("mAdj2", "+- 100000 0 2a1");
                    formulaColl.Add("a2", "pin 0 adj2 mAdj2");
                    formulaColl.Add("dy1", "*/ h a1 100000");
                    formulaColl.Add("dy2", "*/ h a2 200000");
                    formulaColl.Add("dx1", "*/ w 73490 200000");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc dy2 0");
                    formulaColl.Add("y1", "+- y2 0 dy1");
                    formulaColl.Add("y4", "+- y3 dy1 0");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc dx1 0");
                    formulaColl.Add("yC1", "+/ y1 y2 2");
                    formulaColl.Add("yC2", "+/ y3 y4 2");
                    break;
                case AutoShapeType.MathNotEqual:
                    formulaColl.Add("a1", "pin 0 adj1 50000");
                    formulaColl.Add("crAng", "pin 4200000 adj2 6600000");
                    formulaColl.Add("2a1", "*/ a1 2 1");
                    formulaColl.Add("maxAdj3", "+- 100000 0 2a1");
                    formulaColl.Add("a3", "pin 0 adj3 maxAdj3");
                    formulaColl.Add("dy1", "*/ h a1 100000");
                    formulaColl.Add("dy2", "*/ h a3 200000");
                    formulaColl.Add("dx1", "*/ w 73490 200000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x8", "+- hc dx1 0");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc dy2 0");
                    formulaColl.Add("y1", "+- y2 0 dy1");
                    formulaColl.Add("y4", "+- y3 dy1 0");
                    formulaColl.Add("cadj2", "+- crAng 0 cd4");
                    formulaColl.Add("xadj2", "tan hd2 cadj2");
                    formulaColl.Add("len", "mod xadj2 hd2 0");
                    formulaColl.Add("bhw", "*/ len dy1 hd2");
                    formulaColl.Add("bhw2", "*/ bhw 1 2");
                    formulaColl.Add("x7", "+- hc xadj2 bhw2");
                    formulaColl.Add("dx67", "*/ xadj2 y1 hd2");
                    formulaColl.Add("x6", "+- x7 0 dx67");
                    formulaColl.Add("dx57", "*/ xadj2 y2 hd2");
                    formulaColl.Add("x5", "+- x7 0 dx57");
                    formulaColl.Add("dx47", "*/ xadj2 y3 hd2");
                    formulaColl.Add("x4", "+- x7 0 dx47");
                    formulaColl.Add("dx37", "*/ xadj2 y4 hd2");
                    formulaColl.Add("x3", "+- x7 0 dx37");
                    formulaColl.Add("dx27", "*/ xadj2 2 1");
                    formulaColl.Add("x2", "+- x7 0 dx27");
                    formulaColl.Add("rx7", "+- x7 bhw 0");
                    formulaColl.Add("rx6", "+- x6 bhw 0");
                    formulaColl.Add("rx5", "+- x5 bhw 0");
                    formulaColl.Add("rx4", "+- x4 bhw 0");
                    formulaColl.Add("rx3", "+- x3 bhw 0");
                    formulaColl.Add("rx2", "+- x2 bhw 0");
                    formulaColl.Add("dx7", "*/ dy1 hd2 len");
                    formulaColl.Add("rxt", "+- x7 dx7 0");
                    formulaColl.Add("lxt", "+- rx7 0 dx7");
                    formulaColl.Add("rx", "?: cadj2 rxt rx7");
                    formulaColl.Add("lx", "?: cadj2 x7 lxt");
                    formulaColl.Add("dy3", "*/ dy1 xadj2 len");
                    formulaColl.Add("dy4", "+- 0 0 dy3");
                    formulaColl.Add("ry", "?: cadj2 dy3 t");
                    formulaColl.Add("ly", "?: cadj2 t dy4");
                    formulaColl.Add("dlx", "+- w 0 rx");
                    formulaColl.Add("drx", "+- w 0 lx");
                    formulaColl.Add("dly", "+- h 0 ry");
                    formulaColl.Add("dry", "+- h 0 ly");
                    formulaColl.Add("xC1", "+/ rx lx 2");
                    formulaColl.Add("xC2", "+/ drx dlx 2");
                    formulaColl.Add("yC1", "+/ ry ly 2");
                    formulaColl.Add("yC2", "+/ y1 y2 2");
                    formulaColl.Add("yC3", "+/ y3 y4 2");
                    formulaColl.Add("yC4", "+/ dry dly 2");
                    break;
                #endregion
                #region Flowchart
                case AutoShapeType.FlowChartProcess:
                    break;
                case AutoShapeType.FlowChartAlternateProcess:
                    formulaColl.Add("x2", "+- r 0 ssd6");
                    formulaColl.Add("y2", "+- b 0 ssd6");
                    formulaColl.Add("il", "*/ ssd6 29289 100000");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 il");
                    break;
                case AutoShapeType.FlowChartDecision:
                    formulaColl.Add("ir", "*/ w 3 4");
                    formulaColl.Add("ib", "*/ h 3 4");
                    break;
                case AutoShapeType.FlowChartData:
                    formulaColl.Add("x3", "*/ w 2 5");
                    formulaColl.Add("x4", "*/ w 3 5");
                    formulaColl.Add("x5", "*/ w 4 5");
                    formulaColl.Add("x6", "*/ w 9 10");
                    break;
                case AutoShapeType.FlowChartPredefinedProcess:
                    formulaColl.Add("x2", "*/ w 7 8");
                    break;
                case AutoShapeType.FlowChartInternalStorage:
                    break;
                case AutoShapeType.FlowChartDocument:
                    formulaColl.Add("y1", "*/ h 17322 21600");
                    formulaColl.Add("y2", "*/ h 20172 21600");
                    break;
                case AutoShapeType.FlowChartMultiDocument:
                    formulaColl.Add("y2", "*/ h 3675 21600");
                    formulaColl.Add("y8", "*/ h 20782 21600");
                    formulaColl.Add("x3", "*/ w 9298 21600");
                    formulaColl.Add("x4", "*/ w 12286 21600");
                    formulaColl.Add("x5", "*/ w 18595 21600");
                    break;
                case AutoShapeType.FlowChartTerminator:
                    formulaColl.Add("il", "*/ w 1018 21600");
                    formulaColl.Add("ir", "*/ w 20582 21600");
                    formulaColl.Add("it", "*/ h 3163 21600");
                    formulaColl.Add("ib", "*/ h 18437 21600");
                    break;
                case AutoShapeType.FlowChartPreparation:
                    formulaColl.Add("x2", "*/ w 4 5");
                    break;
                case AutoShapeType.FlowChartManualInput:
                    break;
                case AutoShapeType.FlowChartManualOperation:
                    formulaColl.Add("x3", "*/ w 4 5");
                    formulaColl.Add("x4", "*/ w 9 10");
                    break;
                case AutoShapeType.FlowChartConnector:
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.FlowChartOffPageConnector:
                    formulaColl.Add("y1", "*/ h 4 5");
                    break;
                case AutoShapeType.FlowChartCard:
                    break;
                case AutoShapeType.FlowChartPunchedTape:
                    formulaColl.Add("y2", "*/ h 9 10");
                    formulaColl.Add("ib", "*/ h 4 5");
                    break;
                case AutoShapeType.FlowChartSummingJunction:
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.FlowChartOr:
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.FlowChartCollate:
                    formulaColl.Add("ir", "*/ w 3 4");
                    formulaColl.Add("ib", "*/ h 3 4");
                    break;
                case AutoShapeType.FlowChartSort:
                    formulaColl.Add("ir", "*/ w 3 4");
                    formulaColl.Add("ib", "*/ h 3 4");
                    break;
                case AutoShapeType.FlowChartExtract:
                    formulaColl.Add("x2", "*/ w 3 4");
                    break;
                case AutoShapeType.FlowChartMerge:
                    formulaColl.Add("x2", "*/ w 3 4");
                    break;
                case AutoShapeType.FlowChartStoredData:
                    formulaColl.Add("x2", "*/ w 5 6");
                    break;
                case AutoShapeType.FlowChartDelay:
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.FlowChartSequentialAccessStorage:
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    formulaColl.Add("ang", "at2 w h");
                    formulaColl.Add("ang1", "*/ ang 180 " + Math.PI.ToString());
                    break;
                case AutoShapeType.FlowChartMagneticDisk:
                    formulaColl.Add("y3", "*/ h 5 6");
                    break;
                case AutoShapeType.FlowChartDirectAccessStorage:
                    formulaColl.Add("x2", "*/ w 2 3");
                    break;
                case AutoShapeType.FlowChartDisplay:
                    formulaColl.Add("x2", "*/ w 5 6");
                    break;
                #endregion
                #region Starts and Banners
                case AutoShapeType.Explosion1:
                    formulaColl.Add("x5", "*/ w 4627 21600");
                    formulaColl.Add("x12", "*/ w 8485 21600");
                    formulaColl.Add("x21", "*/ w 16702 21600");
                    formulaColl.Add("x24", "*/ w 14522 21600");
                    formulaColl.Add("y3", "*/ h 6320 21600");
                    formulaColl.Add("y6", "*/ h 8615 21600");
                    formulaColl.Add("y9", "*/ h 13937 21600");
                    formulaColl.Add("y18", "*/ h 13290 21600");
                    break;
                case AutoShapeType.Explosion2:
                    formulaColl.Add("x2", "*/ w 9722 21600");
                    formulaColl.Add("x5", "*/ w 5372 21600");
                    formulaColl.Add("x16", "*/ w 11612 21600");
                    formulaColl.Add("x19", "*/ w 14640 21600");
                    formulaColl.Add("y2", "*/ h 1887 21600");
                    formulaColl.Add("y3", "*/ h 6382 21600");
                    formulaColl.Add("y8", "*/ h 12877 21600");
                    formulaColl.Add("y14", "*/ h 19712 21600");
                    formulaColl.Add("y16", "*/ h 18842 21600");
                    formulaColl.Add("y17", "*/ h 15935 21600");
                    formulaColl.Add("y24", "*/ h 6645 21600");
                    break;
                case AutoShapeType.Star4Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("iwd2", "*/ wd2 a 50000");
                    formulaColl.Add("ihd2", "*/ hd2 a 50000");
                    formulaColl.Add("sdx", "cos iwd2 45");
                    formulaColl.Add("sdy", "sin ihd2 45");
                    formulaColl.Add("sx1", "+- hc 0 sdx");
                    formulaColl.Add("sx2", "+- hc sdx 0");
                    formulaColl.Add("sy1", "+- vc 0 sdy");
                    formulaColl.Add("sy2", "+- vc sdy 0");
                    formulaColl.Add("yAdj", "+- vc 0 ihd2");
                    break;
                case AutoShapeType.Star5Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("swd2", "*/ wd2 hf 100000");
                    formulaColl.Add("shd2", "*/ hd2 vf 100000");
                    formulaColl.Add("svc", "*/ vc  vf 100000");
                    formulaColl.Add("dx1", "cos swd2 18");
                    formulaColl.Add("dx2", "cos swd2 306");
                    formulaColl.Add("dy1", "sin shd2 18");
                    formulaColl.Add("dy2", "sin shd2 306");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- svc 0 dy1");
                    formulaColl.Add("y2", "+- svc 0 dy2");
                    formulaColl.Add("iwd2", "*/ swd2 a 50000");
                    formulaColl.Add("ihd2", "*/ shd2 a 50000");
                    formulaColl.Add("sdx1", "cos iwd2 342");
                    formulaColl.Add("sdx2", "cos iwd2 54");
                    formulaColl.Add("sdy1", "sin ihd2 54");
                    formulaColl.Add("sdy2", "sin ihd2 342");
                    formulaColl.Add("sx1", "+- hc 0 sdx1");
                    formulaColl.Add("sx2", "+- hc 0 sdx2");
                    formulaColl.Add("sx3", "+- hc sdx2 0");
                    formulaColl.Add("sx4", "+- hc sdx1 0");
                    formulaColl.Add("sy1", "+- svc 0 sdy1");
                    formulaColl.Add("sy2", "+- svc 0 sdy2");
                    formulaColl.Add("sy3", "+- svc ihd2 0");
                    formulaColl.Add("yAdj", "+- svc 0 ihd2");
                    break;
                case AutoShapeType.Star6Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("swd2", "*/ wd2 hf 100000");
                    formulaColl.Add("dx1", "cos swd2 30");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc dx1 0");
                    formulaColl.Add("y2", "+- vc hd4 0");
                    formulaColl.Add("iwd2", "*/ swd2 a 50000");
                    formulaColl.Add("ihd2", "*/ hd2 a 50000");
                    formulaColl.Add("sdx2", "*/ iwd2 1 2");
                    formulaColl.Add("sx1", "+- hc 0 iwd2");
                    formulaColl.Add("sx2", "+- hc 0 sdx2");
                    formulaColl.Add("sx3", "+- hc sdx2 0");
                    formulaColl.Add("sx4", "+- hc iwd2 0");
                    formulaColl.Add("sdy1", "sin ihd2 60");
                    formulaColl.Add("sy1", "+- vc 0 sdy1");
                    formulaColl.Add("sy2", "+- vc sdy1 0");
                    formulaColl.Add("yAdj", "+- vc 0 ihd2");
                    break;
                case AutoShapeType.Star7Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("swd2", "*/ wd2 hf 100000");
                    formulaColl.Add("shd2", "*/ hd2 vf 100000");
                    formulaColl.Add("svc", "*/ vc  vf 100000");
                    formulaColl.Add("dx1", "*/ swd2 97493 100000");
                    formulaColl.Add("dx2", "*/ swd2 78183 100000");
                    formulaColl.Add("dx3", "*/ swd2 43388 100000");
                    formulaColl.Add("dy1", "*/ shd2 62349 100000");
                    formulaColl.Add("dy2", "*/ shd2 22252 100000");
                    formulaColl.Add("dy3", "*/ shd2 90097 100000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc 0 dx3");
                    formulaColl.Add("x4", "+- hc dx3 0");
                    formulaColl.Add("x5", "+- hc dx2 0");
                    formulaColl.Add("x6", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- svc 0 dy1");
                    formulaColl.Add("y2", "+- svc dy2 0");
                    formulaColl.Add("y3", "+- svc dy3 0");
                    formulaColl.Add("iwd2", "*/ swd2 a 50000");
                    formulaColl.Add("ihd2", "*/ shd2 a 50000");
                    formulaColl.Add("sdx1", "*/ iwd2 97493 100000");
                    formulaColl.Add("sdx2", "*/ iwd2 78183 100000");
                    formulaColl.Add("sdx3", "*/ iwd2 43388 100000");
                    formulaColl.Add("sx1", "+- hc 0 sdx1");
                    formulaColl.Add("sx2", "+- hc 0 sdx2");
                    formulaColl.Add("sx3", "+- hc 0 sdx3");
                    formulaColl.Add("sx4", "+- hc sdx3 0");
                    formulaColl.Add("sx5", "+- hc sdx2 0");
                    formulaColl.Add("sx6", "+- hc sdx1 0");
                    formulaColl.Add("sdy1", "*/ ihd2 90097 100000");
                    formulaColl.Add("sdy2", "*/ ihd2 22252 100000");
                    formulaColl.Add("sdy3", "*/ ihd2 62349 100000");
                    formulaColl.Add("sy1", "+- svc 0 sdy1");
                    formulaColl.Add("sy2", "+- svc 0 sdy2");
                    formulaColl.Add("sy3", "+- svc sdy3 0");
                    formulaColl.Add("sy4", "+- svc ihd2 0");
                    formulaColl.Add("yAdj", "+- svc 0 ihd2");
                    break;
                case AutoShapeType.Star8Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dx1", "cos wd2 45");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc dx1 0");
                    formulaColl.Add("dy1", "sin hd2 45");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc dy1 0");
                    formulaColl.Add("iwd2", "*/ wd2 a 50000");
                    formulaColl.Add("ihd2", "*/ hd2 a 50000");
                    formulaColl.Add("sdx1", "*/ iwd2 92388 100000");
                    formulaColl.Add("sdx2", "*/ iwd2 38268 100000");
                    formulaColl.Add("sdy1", "*/ ihd2 92388 100000");
                    formulaColl.Add("sdy2", "*/ ihd2 38268 100000");
                    formulaColl.Add("sx1", "+- hc 0 sdx1");
                    formulaColl.Add("sx2", "+- hc 0 sdx2");
                    formulaColl.Add("sx3", "+- hc sdx2 0");
                    formulaColl.Add("sx4", "+- hc sdx1 0");
                    formulaColl.Add("sy1", "+- vc 0 sdy1");
                    formulaColl.Add("sy2", "+- vc 0 sdy2");
                    formulaColl.Add("sy3", "+- vc sdy2 0");
                    formulaColl.Add("sy4", "+- vc sdy1 0");
                    formulaColl.Add("yAdj", "+- vc 0 ihd2");
                    break;
                case AutoShapeType.Star10Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("swd2", "*/ wd2 hf 100000");
                    formulaColl.Add("dx1", "*/ swd2 95106 100000");
                    formulaColl.Add("dx2", "*/ swd2 58779 100000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc dx2 0");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("dy1", "*/ hd2 80902 100000");
                    formulaColl.Add("dy2", "*/ hd2 30902 100000");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc dy2 0");
                    formulaColl.Add("y4", "+- vc dy1 0");
                    formulaColl.Add("iwd2", "*/ swd2 a 50000");
                    formulaColl.Add("ihd2", "*/ hd2 a 50000");
                    formulaColl.Add("sdx1", "*/ iwd2 80902 100000");
                    formulaColl.Add("sdx2", "*/ iwd2 30902 100000");
                    formulaColl.Add("sdy1", "*/ ihd2 95106 100000");
                    formulaColl.Add("sdy2", "*/ ihd2 58779 100000");
                    formulaColl.Add("sx1", "+- hc 0 iwd2");
                    formulaColl.Add("sx2", "+- hc 0 sdx1");
                    formulaColl.Add("sx3", "+- hc 0 sdx2");
                    formulaColl.Add("sx4", "+- hc sdx2 0");
                    formulaColl.Add("sx5", "+- hc sdx1 0");
                    formulaColl.Add("sx6", "+- hc iwd2 0");
                    formulaColl.Add("sy1", "+- vc 0 sdy1");
                    formulaColl.Add("sy2", "+- vc 0 sdy2");
                    formulaColl.Add("sy3", "+- vc sdy2 0");
                    formulaColl.Add("sy4", "+- vc sdy1 0");
                    formulaColl.Add("yAdj", "+- vc 0 ihd2");
                    break;
                case AutoShapeType.Star12Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dx1", "cos wd2 30");
                    formulaColl.Add("dy1", "sin hd2 60");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x3", "*/ w 3 4");
                    formulaColl.Add("x4", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y3", "*/ h 3 4");
                    formulaColl.Add("y4", "+- vc dy1 0");
                    formulaColl.Add("iwd2", "*/ wd2 a 50000");
                    formulaColl.Add("ihd2", "*/ hd2 a 50000");
                    formulaColl.Add("sdx1", "cos iwd2 15");
                    formulaColl.Add("sdx2", "cos iwd2 45");
                    formulaColl.Add("sdx3", "cos iwd2 75");
                    formulaColl.Add("sdy1", "sin ihd2 75");
                    formulaColl.Add("sdy2", "sin ihd2 45");
                    formulaColl.Add("sdy3", "sin ihd2 15");
                    formulaColl.Add("sx1", "+- hc 0 sdx1");
                    formulaColl.Add("sx2", "+- hc 0 sdx2");
                    formulaColl.Add("sx3", "+- hc 0 sdx3");
                    formulaColl.Add("sx4", "+- hc sdx3 0");
                    formulaColl.Add("sx5", "+- hc sdx2 0");
                    formulaColl.Add("sx6", "+- hc sdx1 0");
                    formulaColl.Add("sy1", "+- vc 0 sdy1");
                    formulaColl.Add("sy2", "+- vc 0 sdy2");
                    formulaColl.Add("sy3", "+- vc 0 sdy3");
                    formulaColl.Add("sy4", "+- vc sdy3 0");
                    formulaColl.Add("sy5", "+- vc sdy2 0");
                    formulaColl.Add("sy6", "+- vc sdy1 0");
                    formulaColl.Add("yAdj", "+- vc 0 ihd2");
                    break;
                case AutoShapeType.Star16Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dx1", "*/ wd2 92388 100000");
                    formulaColl.Add("dx2", "*/ wd2 70711 100000");
                    formulaColl.Add("dx3", "*/ wd2 38268 100000");
                    formulaColl.Add("dy1", "*/ hd2 92388 100000");
                    formulaColl.Add("dy2", "*/ hd2 70711 100000");
                    formulaColl.Add("dy3", "*/ hd2 38268 100000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc 0 dx3");
                    formulaColl.Add("x4", "+- hc dx3 0");
                    formulaColl.Add("x5", "+- hc dx2 0");
                    formulaColl.Add("x6", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc 0 dy3");
                    formulaColl.Add("y4", "+- vc dy3 0");
                    formulaColl.Add("y5", "+- vc dy2 0");
                    formulaColl.Add("y6", "+- vc dy1 0");
                    formulaColl.Add("iwd2", "*/ wd2 a 50000");
                    formulaColl.Add("ihd2", "*/ hd2 a 50000");
                    formulaColl.Add("sdx1", "*/ iwd2 98079 100000");
                    formulaColl.Add("sdx2", "*/ iwd2 83147 100000");
                    formulaColl.Add("sdx3", "*/ iwd2 55557 100000");
                    formulaColl.Add("sdx4", "*/ iwd2 19509 100000");
                    formulaColl.Add("sdy1", "*/ ihd2 98079 100000");
                    formulaColl.Add("sdy2", "*/ ihd2 83147 100000");
                    formulaColl.Add("sdy3", "*/ ihd2 55557 100000");
                    formulaColl.Add("sdy4", "*/ ihd2 19509 100000");
                    formulaColl.Add("sx1", "+- hc 0 sdx1");
                    formulaColl.Add("sx2", "+- hc 0 sdx2");
                    formulaColl.Add("sx3", "+- hc 0 sdx3");
                    formulaColl.Add("sx4", "+- hc 0 sdx4");
                    formulaColl.Add("sx5", "+- hc sdx4 0");
                    formulaColl.Add("sx6", "+- hc sdx3 0");
                    formulaColl.Add("sx7", "+- hc sdx2 0");
                    formulaColl.Add("sx8", "+- hc sdx1 0");
                    formulaColl.Add("sy1", "+- vc 0 sdy1");
                    formulaColl.Add("sy2", "+- vc 0 sdy2");
                    formulaColl.Add("sy3", "+- vc 0 sdy3");
                    formulaColl.Add("sy4", "+- vc 0 sdy4");
                    formulaColl.Add("sy5", "+- vc sdy4 0");
                    formulaColl.Add("sy6", "+- vc sdy3 0");
                    formulaColl.Add("sy7", "+- vc sdy2 0");
                    formulaColl.Add("sy8", "+- vc sdy1 0");
                    formulaColl.Add("idx", "cos iwd2 45");
                    formulaColl.Add("idy", "sin ihd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("ib", "+- vc idy 0");
                    formulaColl.Add("yAdj", "+- vc 0 ihd2");
                    break;
                case AutoShapeType.Star24Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dx1", "cos wd2 15");
                    formulaColl.Add("dx2", "cos wd2 30");
                    formulaColl.Add("dx3", "cos wd2 45");
                    formulaColl.Add("dx4", "val wd4");
                    formulaColl.Add("dx5", "cos wd2 75");
                    formulaColl.Add("dy1", "sin hd2 75");
                    formulaColl.Add("dy2", "sin hd2 60");
                    formulaColl.Add("dy3", "sin hd2 45");
                    formulaColl.Add("dy4", "val hd4");
                    formulaColl.Add("dy5", "sin hd2 15");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc 0 dx3");
                    formulaColl.Add("x4", "+- hc 0 dx4");
                    formulaColl.Add("x5", "+- hc 0 dx5");
                    formulaColl.Add("x6", "+- hc dx5 0");
                    formulaColl.Add("x7", "+- hc dx4 0");
                    formulaColl.Add("x8", "+- hc dx3 0");
                    formulaColl.Add("x9", "+- hc dx2 0");
                    formulaColl.Add("x10", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc 0 dy3");
                    formulaColl.Add("y4", "+- vc 0 dy4");
                    formulaColl.Add("y5", "+- vc 0 dy5");
                    formulaColl.Add("y6", "+- vc dy5 0");
                    formulaColl.Add("y7", "+- vc dy4 0");
                    formulaColl.Add("y8", "+- vc dy3 0");
                    formulaColl.Add("y9", "+- vc dy2 0");
                    formulaColl.Add("y10", "+- vc dy1 0");
                    formulaColl.Add("iwd2", "*/ wd2 a 50000");
                    formulaColl.Add("ihd2", "*/ hd2 a 50000");
                    formulaColl.Add("sdx1", "*/ iwd2 99144 100000");
                    formulaColl.Add("sdx2", "*/ iwd2 92388 100000");
                    formulaColl.Add("sdx3", "*/ iwd2 79335 100000");
                    formulaColl.Add("sdx4", "*/ iwd2 60876 100000");
                    formulaColl.Add("sdx5", "*/ iwd2 38268 100000");
                    formulaColl.Add("sdx6", "*/ iwd2 13053 100000");
                    formulaColl.Add("sdy1", "*/ ihd2 99144 100000");
                    formulaColl.Add("sdy2", "*/ ihd2 92388 100000");
                    formulaColl.Add("sdy3", "*/ ihd2 79335 100000");
                    formulaColl.Add("sdy4", "*/ ihd2 60876 100000");
                    formulaColl.Add("sdy5", "*/ ihd2 38268 100000");
                    formulaColl.Add("sdy6", "*/ ihd2 13053 100000");
                    formulaColl.Add("sx1", "+- hc 0 sdx1");
                    formulaColl.Add("sx2", "+- hc 0 sdx2");
                    formulaColl.Add("sx3", "+- hc 0 sdx3");
                    formulaColl.Add("sx4", "+- hc 0 sdx4");
                    formulaColl.Add("sx5", "+- hc 0 sdx5");
                    formulaColl.Add("sx6", "+- hc 0 sdx6");
                    formulaColl.Add("sx7", "+- hc sdx6 0");
                    formulaColl.Add("sx8", "+- hc sdx5 0");
                    formulaColl.Add("sx9", "+- hc sdx4 0");
                    formulaColl.Add("sx10", "+- hc sdx3 0");
                    formulaColl.Add("sx11", "+- hc sdx2 0");
                    formulaColl.Add("sx12", "+- hc sdx1 0");
                    formulaColl.Add("sy1", "+- vc 0 sdy1");
                    formulaColl.Add("sy2", "+- vc 0 sdy2");
                    formulaColl.Add("sy3", "+- vc 0 sdy3");
                    formulaColl.Add("sy4", "+- vc 0 sdy4");
                    formulaColl.Add("sy5", "+- vc 0 sdy5");
                    formulaColl.Add("sy6", "+- vc 0 sdy6");
                    formulaColl.Add("sy7", "+- vc sdy6 0");
                    formulaColl.Add("sy8", "+- vc sdy5 0");
                    formulaColl.Add("sy9", "+- vc sdy4 0");
                    formulaColl.Add("sy10", "+- vc sdy3 0");
                    formulaColl.Add("sy11", "+- vc sdy2 0");
                    formulaColl.Add("sy12", "+- vc sdy1 0");
                    formulaColl.Add("idx", "cos iwd2 45");
                    formulaColl.Add("idy", "sin ihd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("ib", "+- vc idy 0");
                    formulaColl.Add("yAdj", "+- vc 0 ihd2");
                    break;
                case AutoShapeType.Star32Point:
                    formulaColl.Add("a", "pin 0 adj 50000");
                    formulaColl.Add("dx1", "*/ wd2 98079 100000");
                    formulaColl.Add("dx2", "*/ wd2 92388 100000");
                    formulaColl.Add("dx3", "*/ wd2 83147 100000");
                    formulaColl.Add("dx4", "cos wd2 45");
                    formulaColl.Add("dx5", "*/ wd2 55557 100000");
                    formulaColl.Add("dx6", "*/ wd2 38268 100000");
                    formulaColl.Add("dx7", "*/ wd2 19509 100000");
                    formulaColl.Add("dy1", "*/ hd2 98079 100000");
                    formulaColl.Add("dy2", "*/ hd2 92388 100000");
                    formulaColl.Add("dy3", "*/ hd2 83147 100000");
                    formulaColl.Add("dy4", "sin hd2 45");
                    formulaColl.Add("dy5", "*/ hd2 55557 100000");
                    formulaColl.Add("dy6", "*/ hd2 38268 100000");
                    formulaColl.Add("dy7", "*/ hd2 19509 100000");
                    formulaColl.Add("x1", "+- hc 0 dx1");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- hc 0 dx3");
                    formulaColl.Add("x4", "+- hc 0 dx4");
                    formulaColl.Add("x5", "+- hc 0 dx5");
                    formulaColl.Add("x6", "+- hc 0 dx6");
                    formulaColl.Add("x7", "+- hc 0 dx7");
                    formulaColl.Add("x8", "+- hc dx7 0");
                    formulaColl.Add("x9", "+- hc dx6 0");
                    formulaColl.Add("x10", "+- hc dx5 0");
                    formulaColl.Add("x11", "+- hc dx4 0");
                    formulaColl.Add("x12", "+- hc dx3 0");
                    formulaColl.Add("x13", "+- hc dx2 0");
                    formulaColl.Add("x14", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc 0 dy1");
                    formulaColl.Add("y2", "+- vc 0 dy2");
                    formulaColl.Add("y3", "+- vc 0 dy3");
                    formulaColl.Add("y4", "+- vc 0 dy4");
                    formulaColl.Add("y5", "+- vc 0 dy5");
                    formulaColl.Add("y6", "+- vc 0 dy6");
                    formulaColl.Add("y7", "+- vc 0 dy7");
                    formulaColl.Add("y8", "+- vc dy7 0");
                    formulaColl.Add("y9", "+- vc dy6 0");
                    formulaColl.Add("y10", "+- vc dy5 0");
                    formulaColl.Add("y11", "+- vc dy4 0");
                    formulaColl.Add("y12", "+- vc dy3 0");
                    formulaColl.Add("y13", "+- vc dy2 0");
                    formulaColl.Add("y14", "+- vc dy1 0");
                    formulaColl.Add("iwd2", "*/ wd2 a 50000");
                    formulaColl.Add("ihd2", "*/ hd2 a 50000");
                    formulaColl.Add("sdx1", "*/ iwd2 99518 100000");
                    formulaColl.Add("sdx2", "*/ iwd2 95694 100000");
                    formulaColl.Add("sdx3", "*/ iwd2 88192 100000");
                    formulaColl.Add("sdx4", "*/ iwd2 77301 100000");
                    formulaColl.Add("sdx5", "*/ iwd2 63439 100000");
                    formulaColl.Add("sdx6", "*/ iwd2 47140 100000");
                    formulaColl.Add("sdx7", "*/ iwd2 29028 100000");
                    formulaColl.Add("sdx8", "*/ iwd2 9802 100000");
                    formulaColl.Add("sdy1", "*/ ihd2 99518 100000");
                    formulaColl.Add("sdy2", "*/ ihd2 95694 100000");
                    formulaColl.Add("sdy3", "*/ ihd2 88192 100000");
                    formulaColl.Add("sdy4", "*/ ihd2 77301 100000");
                    formulaColl.Add("sdy5", "*/ ihd2 63439 100000");
                    formulaColl.Add("sdy6", "*/ ihd2 47140 100000");
                    formulaColl.Add("sdy7", "*/ ihd2 29028 100000");
                    formulaColl.Add("sdy8", "*/ ihd2 9802 100000");
                    formulaColl.Add("sx1", "+- hc 0 sdx1");
                    formulaColl.Add("sx2", "+- hc 0 sdx2");
                    formulaColl.Add("sx3", "+- hc 0 sdx3");
                    formulaColl.Add("sx4", "+- hc 0 sdx4");
                    formulaColl.Add("sx5", "+- hc 0 sdx5");
                    formulaColl.Add("sx6", "+- hc 0 sdx6");
                    formulaColl.Add("sx7", "+- hc 0 sdx7");
                    formulaColl.Add("sx8", "+- hc 0 sdx8");
                    formulaColl.Add("sx9", "+- hc sdx8 0");
                    formulaColl.Add("sx10", "+- hc sdx7 0");
                    formulaColl.Add("sx11", "+- hc sdx6 0");
                    formulaColl.Add("sx12", "+- hc sdx5 0");
                    formulaColl.Add("sx13", "+- hc sdx4 0");
                    formulaColl.Add("sx14", "+- hc sdx3 0");
                    formulaColl.Add("sx15", "+- hc sdx2 0");
                    formulaColl.Add("sx16", "+- hc sdx1 0");
                    formulaColl.Add("sy1", "+- vc 0 sdy1");
                    formulaColl.Add("sy2", "+- vc 0 sdy2");
                    formulaColl.Add("sy3", "+- vc 0 sdy3");
                    formulaColl.Add("sy4", "+- vc 0 sdy4");
                    formulaColl.Add("sy5", "+- vc 0 sdy5");
                    formulaColl.Add("sy6", "+- vc 0 sdy6");
                    formulaColl.Add("sy7", "+- vc 0 sdy7");
                    formulaColl.Add("sy8", "+- vc 0 sdy8");
                    formulaColl.Add("sy9", "+- vc sdy8 0");
                    formulaColl.Add("sy10", "+- vc sdy7 0");
                    formulaColl.Add("sy11", "+- vc sdy6 0");
                    formulaColl.Add("sy12", "+- vc sdy5 0");
                    formulaColl.Add("sy13", "+- vc sdy4 0");
                    formulaColl.Add("sy14", "+- vc sdy3 0");
                    formulaColl.Add("sy15", "+- vc sdy2 0");
                    formulaColl.Add("sy16", "+- vc sdy1 0");
                    formulaColl.Add("idx", "cos iwd2 45");
                    formulaColl.Add("idy", "sin ihd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("ib", "+- vc idy 0");
                    formulaColl.Add("yAdj", "+- vc 0 ihd2");
                    break;
                case AutoShapeType.UpRibbon:
                    formulaColl.Add("a1", "pin 0 adj1 33333");
                    formulaColl.Add("a2", "pin 25000 adj2 75000");
                    formulaColl.Add("x10", "+- r 0 wd8");
                    formulaColl.Add("dx2", "*/ w a2 200000");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x9", "+- hc dx2 0");
                    formulaColl.Add("x3", "+- x2 wd32 0");
                    formulaColl.Add("x8", "+- x9 0 wd32");
                    formulaColl.Add("x5", "+- x2 wd8 0");
                    formulaColl.Add("x6", "+- x9 0 wd8");
                    formulaColl.Add("x4", "+- x5 0 wd32");
                    formulaColl.Add("x7", "+- x6 wd32 0");
                    formulaColl.Add("dy1", "*/ h a1 200000");
                    formulaColl.Add("y1", "+- b 0 dy1");
                    formulaColl.Add("dy2", "*/ h a1 100000");
                    formulaColl.Add("y2", "+- b 0 dy2");
                    formulaColl.Add("y4", "+- t dy2 0");
                    formulaColl.Add("y3", "+/ y4 b 2");
                    formulaColl.Add("hR", "*/ h a1 400000");
                    formulaColl.Add("y6", "+- b 0 hR");
                    formulaColl.Add("y7", "+- y1 0 hR");
                    break;
                case AutoShapeType.DownRibbon:
                    formulaColl.Add("a1", "pin 0 adj1 33333");
                    formulaColl.Add("a2", "pin 25000 adj2 75000");
                    formulaColl.Add("x10", "+- r 0 wd8");
                    formulaColl.Add("dx2", "*/ w a2 200000");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x9", "+- hc dx2 0");
                    formulaColl.Add("x3", "+- x2 wd32 0");
                    formulaColl.Add("x8", "+- x9 0 wd32");
                    formulaColl.Add("x5", "+- x2 wd8 0");
                    formulaColl.Add("x6", "+- x9 0 wd8");
                    formulaColl.Add("x4", "+- x5 0 wd32");
                    formulaColl.Add("x7", "+- x6 wd32 0");
                    formulaColl.Add("y1", "*/ h a1 200000");
                    formulaColl.Add("y2", "*/ h a1 100000");
                    formulaColl.Add("y4", "+- b 0 y2");
                    formulaColl.Add("y3", "*/ y4 1 2");
                    formulaColl.Add("hR", "*/ h a1 400000");
                    formulaColl.Add("y5", "+- b 0 hR");
                    formulaColl.Add("y6", "+- y2 0 hR");
                    break;
                case AutoShapeType.CurvedUpRibbon:
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 25000 adj2 75000");
                    formulaColl.Add("q10", "+- 100000 0 a1");
                    formulaColl.Add("q11", "*/ q10 1 2");
                    formulaColl.Add("q12", "+- a1 0 q11");
                    formulaColl.Add("minAdj3", "max 0 q12");
                    formulaColl.Add("a3", "pin minAdj3 adj3 a1");
                    formulaColl.Add("dx2", "*/ w a2 200000");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- x2 wd8 0");
                    formulaColl.Add("x4", "+- r 0 x3");
                    formulaColl.Add("x5", "+- r 0 x2");
                    formulaColl.Add("x6", "+- r 0 wd8");
                    formulaColl.Add("dy1", "*/ h a3 100000");
                    formulaColl.Add("f1", "*/ 4 dy1 w");
                    formulaColl.Add("q111", "*/ x3 x3 w");
                    formulaColl.Add("q2", "+- x3 0 q111");
                    formulaColl.Add("u1", "*/ f1 q2 1");
                    formulaColl.Add("y1", "+- b 0 u1");
                    formulaColl.Add("cx1", "*/ x3 1 2");
                    formulaColl.Add("cu1", "*/ f1 cx1 1");
                    formulaColl.Add("cy1", "+- b 0 cu1");
                    formulaColl.Add("cx2", "+- r 0 cx1");
                    formulaColl.Add("q1", "*/ h a1 100000");
                    formulaColl.Add("dy3", "+- q1 0 dy1");
                    formulaColl.Add("q3", "*/ x2 x2 w");
                    formulaColl.Add("q4", "+- x2 0 q3");
                    formulaColl.Add("q5", "*/ f1 q4 1");
                    formulaColl.Add("u3", "+- q5 dy3 0");
                    formulaColl.Add("y3", "+- b 0 u3");
                    formulaColl.Add("q6", "+- dy1 dy3 u3");
                    formulaColl.Add("q7", "+- q6 dy1 0");
                    formulaColl.Add("cu3", "+- q7 dy3 0");
                    formulaColl.Add("cy3", "+- b 0 cu3");
                    formulaColl.Add("rh", "+- b 0 q1");
                    formulaColl.Add("q8", "*/ dy1 14 16");
                    formulaColl.Add("u2", "+/ q8 rh 2");
                    formulaColl.Add("y2", "+- b 0 u2");
                    formulaColl.Add("u5", "+- q5 rh 0");
                    formulaColl.Add("y5", "+- b 0 u5");
                    formulaColl.Add("u6", "+- u3 rh 0");
                    formulaColl.Add("y6", "+- b 0 u6");
                    formulaColl.Add("cx4", "*/ x2 1 2");
                    formulaColl.Add("q9", "*/ f1 cx4 1");
                    formulaColl.Add("cu4", "+- q9 rh 0");
                    formulaColl.Add("cy4", "+- b 0 cu4");
                    formulaColl.Add("cx5", "+- r 0 cx4");
                    formulaColl.Add("cu6", "+- cu3 rh 0");
                    formulaColl.Add("cy6", "+- b 0 cu6");
                    formulaColl.Add("u7", "+- u1 dy3 0");
                    formulaColl.Add("y7", "+- b 0 u7");
                    formulaColl.Add("cu7", "+- q1 q1 u7");
                    formulaColl.Add("cy7", "+- b 0 cu7");
                    break;
                case AutoShapeType.CurvedDownRibbon:
                    formulaColl.Add("a1", "pin 0 adj1 100000");
                    formulaColl.Add("a2", "pin 25000 adj2 75000");
                    formulaColl.Add("q10", "+- 100000 0 a1");
                    formulaColl.Add("q11", "*/ q10 1 2");
                    formulaColl.Add("q12", "+- a1 0 q11");
                    formulaColl.Add("minAdj3", "max 0 q12");
                    formulaColl.Add("a3", "pin minAdj3 adj3 a1");
                    formulaColl.Add("dx2", "*/ w a2 200000");
                    formulaColl.Add("x2", "+- hc 0 dx2");
                    formulaColl.Add("x3", "+- x2 wd8 0");
                    formulaColl.Add("x4", "+- r 0 x3");
                    formulaColl.Add("x5", "+- r 0 x2");
                    formulaColl.Add("x6", "+- r 0 wd8");
                    formulaColl.Add("dy1", "*/ h a3 100000");
                    formulaColl.Add("f1", "*/ 4 dy1 w");
                    formulaColl.Add("q111", "*/ x3 x3 w");
                    formulaColl.Add("q2", "+- x3 0 q111");
                    formulaColl.Add("y1", "*/ f1 q2 1");
                    formulaColl.Add("cx1", "*/ x3 1 2");
                    formulaColl.Add("cy1", "*/ f1 cx1 1");
                    formulaColl.Add("cx2", "+- r 0 cx1");
                    formulaColl.Add("q1", "*/ h a1 100000");
                    formulaColl.Add("dy3", "+- q1 0 dy1");
                    formulaColl.Add("q3", "*/ x2 x2 w");
                    formulaColl.Add("q4", "+- x2 0 q3");
                    formulaColl.Add("q5", "*/ f1 q4 1");
                    formulaColl.Add("y3", "+- q5 dy3 0");
                    formulaColl.Add("q6", "+- dy1 dy3 y3");
                    formulaColl.Add("q7", "+- q6 dy1 0");
                    formulaColl.Add("cy3", "+- q7 dy3 0");
                    formulaColl.Add("rh", "+- b 0 q1");
                    formulaColl.Add("q8", "*/ dy1 14 16");
                    formulaColl.Add("y2", "+/ q8 rh 2");
                    formulaColl.Add("y5", "+- q5 rh 0");
                    formulaColl.Add("y6", "+- y3 rh 0");
                    formulaColl.Add("cx4", "*/ x2 1 2");
                    formulaColl.Add("q9", "*/ f1 cx4 1");
                    formulaColl.Add("cy4", "+- q9 rh 0");
                    formulaColl.Add("cx5", "+- r 0 cx4");
                    formulaColl.Add("cy6", "+- cy3 rh 0");
                    formulaColl.Add("y7", "+- y1 dy3 0");
                    formulaColl.Add("cy7", "+- q1 q1 y7");
                    formulaColl.Add("y8", "+- b 0 dy1");
                    break;
                case AutoShapeType.VerticalScroll:
                    formulaColl.Add("a", "pin 0 adj 25000");
                    formulaColl.Add("ch", "*/ ss a 100000");
                    formulaColl.Add("ch2", "*/ ch 1 2");
                    formulaColl.Add("ch4", "*/ ch 1 4");
                    formulaColl.Add("x3", "+- ch ch2 0");
                    formulaColl.Add("x4", "+- ch ch 0");
                    formulaColl.Add("x6", "+- r 0 ch");
                    formulaColl.Add("x7", "+- r 0 ch2");
                    formulaColl.Add("x5", "+- x6 0 ch2");
                    formulaColl.Add("y3", "+- b 0 ch");
                    formulaColl.Add("y4", "+- b 0 ch2");
                    break;
                case AutoShapeType.HorizontalScroll:
                    formulaColl.Add("a", "pin 0 adj 25000");
                    formulaColl.Add("ch", "*/ ss a 100000");
                    formulaColl.Add("ch2", "*/ ch 1 2");
                    formulaColl.Add("ch4", "*/ ch 1 4");
                    formulaColl.Add("y3", "+- ch ch2 0");
                    formulaColl.Add("y4", "+- ch ch 0");
                    formulaColl.Add("y6", "+- b 0 ch");
                    formulaColl.Add("y7", "+- b 0 ch2");
                    formulaColl.Add("y5", "+- y6 0 ch2");
                    formulaColl.Add("x3", "+- r 0 ch");
                    formulaColl.Add("x4", "+- r 0 ch2");
                    break;
                case AutoShapeType.Wave:
                    formulaColl.Add("a1", "pin 0 adj1 20000");
                    formulaColl.Add("a2", "pin -10000 adj2 10000");
                    formulaColl.Add("y1", "*/ h a1 100000");
                    formulaColl.Add("dy2", "*/ y1 10 3");
                    formulaColl.Add("y2", "+- y1 0 dy2");
                    formulaColl.Add("y3", "+- y1 dy2 0");
                    formulaColl.Add("y4", "+- b 0 y1");
                    formulaColl.Add("y5", "+- y4 0 dy2");
                    formulaColl.Add("y6", "+- y4 dy2 0");
                    formulaColl.Add("dx1", "*/ w a2 100000");
                    formulaColl.Add("of2", "*/ w a2 50000");
                    formulaColl.Add("x1", "abs dx1");
                    formulaColl.Add("dx2", "?: of2 0 of2");
                    formulaColl.Add("x2", "+- l 0 dx2");
                    formulaColl.Add("dx5", "?: of2 of2 0");
                    formulaColl.Add("x5", "+- r 0 dx5");
                    formulaColl.Add("dx3", "+/ dx2 x5 3");
                    formulaColl.Add("x3", "+- x2 dx3 0");
                    formulaColl.Add("x4", "+/ x3 x5 2");
                    formulaColl.Add("x6", "+- l dx5 0");
                    formulaColl.Add("x10", "+- r dx2 0");
                    formulaColl.Add("x7", "+- x6 dx3 0");
                    formulaColl.Add("x8", "+/ x7 x10 2");
                    formulaColl.Add("x9", "+- r 0 x1");
                    formulaColl.Add("xAdj", "+- hc dx1 0");
                    formulaColl.Add("xAdj2", "+- hc 0 dx1");
                    formulaColl.Add("il", "max x2 x6");
                    formulaColl.Add("ir", "min x5 x10");
                    formulaColl.Add("it", "*/ h a1 50000");
                    formulaColl.Add("ib", "+- b 0 it");
                    break;
                case AutoShapeType.DoubleWave:
                    formulaColl.Add("a1", "pin 0 adj1 12500");
                    formulaColl.Add("a2", "pin -10000 adj2 10000");
                    formulaColl.Add("y1", "*/ h a1 100000");
                    formulaColl.Add("dy2", "*/ y1 10 3");
                    formulaColl.Add("y2", "+- y1 0 dy2");
                    formulaColl.Add("y3", "+- y1 dy2 0");
                    formulaColl.Add("y4", "+- b 0 y1");
                    formulaColl.Add("y5", "+- y4 0 dy2");
                    formulaColl.Add("y6", "+- y4 dy2 0");
                    formulaColl.Add("dx1", "*/ w a2 100000");
                    formulaColl.Add("of2", "*/ w a2 50000");
                    formulaColl.Add("x1", "abs dx1");
                    formulaColl.Add("dx2", "?: of2 0 of2");
                    formulaColl.Add("x2", "+- l 0 dx2");
                    formulaColl.Add("dx8", "?: of2 of2 0");
                    formulaColl.Add("x8", "+- r 0 dx8");
                    formulaColl.Add("dx3", "+/ dx2 x8 6");
                    formulaColl.Add("x3", "+- x2 dx3 0");
                    formulaColl.Add("dx4", "+/ dx2 x8 3");
                    formulaColl.Add("x4", "+- x2 dx4 0");
                    formulaColl.Add("x5", "+/ x2 x8 2");
                    formulaColl.Add("x6", "+- x5 dx3 0");
                    formulaColl.Add("x7", "+/ x6 x8 2");
                    formulaColl.Add("x9", "+- l dx8 0");
                    formulaColl.Add("x15", "+- r dx2 0");
                    formulaColl.Add("x10", "+- x9 dx3 0");
                    formulaColl.Add("x11", "+- x9 dx4 0");
                    formulaColl.Add("x12", "+/ x9 x15 2");
                    formulaColl.Add("x13", "+- x12 dx3 0");
                    formulaColl.Add("x14", "+/ x13 x15 2");
                    formulaColl.Add("x16", "+- r 0 x1");
                    formulaColl.Add("xAdj", "+- hc dx1 0");
                    formulaColl.Add("il", "max x2 x9");
                    formulaColl.Add("ir", "min x8 x15");
                    formulaColl.Add("it", "*/ h a1 50000");
                    formulaColl.Add("ib", "+- b 0 it");
                    break;
                #endregion
                #region Callouts
                case AutoShapeType.RectangularCallout:
                    formulaColl.Add("dxPos", "*/ w adj1 100000");
                    formulaColl.Add("dyPos", "*/ h adj2 100000");
                    formulaColl.Add("xPos", "+- hc dxPos 0");
                    formulaColl.Add("yPos", "+- vc dyPos 0");
                    formulaColl.Add("dx", "+- xPos 0 hc");
                    formulaColl.Add("dy", "+- yPos 0 vc");
                    formulaColl.Add("dq", "*/ dxPos h w");
                    formulaColl.Add("ady", "abs dyPos");
                    formulaColl.Add("adq", "abs dq");
                    formulaColl.Add("dz", "+- ady 0 adq");
                    formulaColl.Add("xg1", "?: dxPos 7 2");
                    formulaColl.Add("xg2", "?: dxPos 10 5");
                    formulaColl.Add("x1", "*/ w xg1 12");
                    formulaColl.Add("x2", "*/ w xg2 12");
                    formulaColl.Add("yg1", "?: dyPos 7 2");
                    formulaColl.Add("yg2", "?: dyPos 10 5");
                    formulaColl.Add("y1", "*/ h yg1 12");
                    formulaColl.Add("y2", "*/ h yg2 12");
                    formulaColl.Add("t1", "?: dxPos l xPos");
                    formulaColl.Add("xl", "?: dz l t1");
                    formulaColl.Add("t2", "?: dyPos x1 xPos");
                    formulaColl.Add("xt", "?: dz t2 x1");
                    formulaColl.Add("t3", "?: dxPos xPos r");
                    formulaColl.Add("xr", "?: dz r t3");
                    formulaColl.Add("t4", "?: dyPos xPos x1");
                    formulaColl.Add("xb", "?: dz t4 x1");
                    formulaColl.Add("t5", "?: dxPos y1 yPos");
                    formulaColl.Add("yl", "?: dz y1 t5");
                    formulaColl.Add("t6", "?: dyPos t yPos");
                    formulaColl.Add("yt", "?: dz t6 t");
                    formulaColl.Add("t7", "?: dxPos yPos y1");
                    formulaColl.Add("yr", "?: dz y1 t7");
                    formulaColl.Add("t8", "?: dyPos yPos b");
                    formulaColl.Add("yb", "?: dz t8 b");
                    break;
                case AutoShapeType.RoundedRectangularCallout:
                    formulaColl.Add("dxPos", "*/ w adj1 100000");
                    formulaColl.Add("dyPos", "*/ h adj2 100000");
                    formulaColl.Add("xPos", "+- hc dxPos 0");
                    formulaColl.Add("yPos", "+- vc dyPos 0");
                    formulaColl.Add("dq", "*/ dxPos h w");
                    formulaColl.Add("ady", "abs dyPos");
                    formulaColl.Add("adq", "abs dq");
                    formulaColl.Add("dz", "+- ady 0 adq");
                    formulaColl.Add("xg1", "?: dxPos 7 2");
                    formulaColl.Add("xg2", "?: dxPos 10 5");
                    formulaColl.Add("x1", "*/ w xg1 12");
                    formulaColl.Add("x2", "*/ w xg2 12");
                    formulaColl.Add("yg1", "?: dyPos 7 2");
                    formulaColl.Add("yg2", "?: dyPos 10 5");
                    formulaColl.Add("y1", "*/ h yg1 12");
                    formulaColl.Add("y2", "*/ h yg2 12");
                    formulaColl.Add("t1", "?: dxPos l xPos");
                    formulaColl.Add("xl", "?: dz l t1");
                    formulaColl.Add("t2", "?: dyPos x1 xPos");
                    formulaColl.Add("xt", "?: dz t2 x1");
                    formulaColl.Add("t3", "?: dxPos xPos r");
                    formulaColl.Add("xr", "?: dz r t3");
                    formulaColl.Add("t4", "?: dyPos xPos x1");
                    formulaColl.Add("xb", "?: dz t4 x1");
                    formulaColl.Add("t5", "?: dxPos y1 yPos");
                    formulaColl.Add("yl", "?: dz y1 t5");
                    formulaColl.Add("t6", "?: dyPos t yPos");
                    formulaColl.Add("yt", "?: dz t6 t");
                    formulaColl.Add("t7", "?: dxPos yPos y1");
                    formulaColl.Add("yr", "?: dz y1 t7");
                    formulaColl.Add("t8", "?: dyPos yPos b");
                    formulaColl.Add("yb", "?: dz t8 b");
                    formulaColl.Add("u1", "*/ ss adj3 100000");
                    formulaColl.Add("u2", "+- r 0 u1");
                    formulaColl.Add("v2", "+- b 0 u1");
                    formulaColl.Add("il", "*/ u1 29289 100000");
                    formulaColl.Add("ir", "+- r 0 il");
                    formulaColl.Add("ib", "+- b 0 il");
                    break;
                case AutoShapeType.OvalCallout:
                    formulaColl.Add("dxPos", "*/ w adj1 100000");
                    formulaColl.Add("dyPos", "*/ h adj2 100000");
                    formulaColl.Add("xPos", "+- hc dxPos 0");
                    formulaColl.Add("yPos", "+- vc dyPos 0");
                    formulaColl.Add("sdx", "*/ dxPos h 1");
                    formulaColl.Add("sdy", "*/ dyPos w 1");
                    formulaColl.Add("pang1", "at2 sdx sdy");
                    formulaColl.Add("pang", "*/ pang1 180 " + Math.PI.ToString());
                    formulaColl.Add("stAng", "+- pang 11 0");
                    formulaColl.Add("enAng", "+- pang 0 11");
                    formulaColl.Add("dx1", "cos wd2 stAng");
                    formulaColl.Add("dy1", "sin hd2 stAng");
                    formulaColl.Add("x1", "+- hc dx1 0");
                    formulaColl.Add("y1", "+- vc dy1 0");
                    formulaColl.Add("dx2", "cos wd2 enAng");
                    formulaColl.Add("dy2", "sin hd2 enAng");
                    formulaColl.Add("x2", "+- hc dx2 0");
                    formulaColl.Add("y2", "+- vc dy2 0");
                    formulaColl.Add("stAng2", "at2 dx1 dy1");
                    formulaColl.Add("stAng1", "*/ stAng2 180 " + Math.PI.ToString());
                    formulaColl.Add("enAng2", "at2 dx2 dy2");
                    formulaColl.Add("enAng1", "*/ enAng2 180 " + Math.PI.ToString());
                    formulaColl.Add("swAng1", "+- enAng1 0 stAng1");
                    formulaColl.Add("swAng2", "+- swAng1 360 0");
                    formulaColl.Add("swAng", "?: swAng1 swAng1 swAng2");
                    formulaColl.Add("idx", "cos wd2 45");
                    formulaColl.Add("idy", "sin hd2 45");
                    formulaColl.Add("il", "+- hc 0 idx");
                    formulaColl.Add("ir", "+- hc idx 0");
                    formulaColl.Add("it", "+- vc 0 idy");
                    formulaColl.Add("ib", "+- vc idy 0");
                    break;
                case AutoShapeType.CloudCallout:
                    formulaColl.Add("dxPos", "*/ w adj1 100000");
                    formulaColl.Add("dyPos", "*/ h adj2 100000");
                    formulaColl.Add("xPos", "+- hc dxPos 0");
                    formulaColl.Add("yPos", "+- vc dyPos 0");
                    formulaColl.Add("ht", "cat2 hd2 dxPos dyPos");
                    formulaColl.Add("wt", "sat2 wd2 dxPos dyPos");
                    formulaColl.Add("g2", "cat2 wd2 ht wt");
                    formulaColl.Add("g3", "sat2 hd2 ht wt");
                    formulaColl.Add("g4", "+- hc g2 0");
                    formulaColl.Add("g5", "+- vc g3 0");
                    formulaColl.Add("g6", "+- g4 0 xPos");
                    formulaColl.Add("g7", "+- g5 0 yPos");
                    formulaColl.Add("g8", "mod g6 g7 0");
                    formulaColl.Add("g9", "*/ ss 6600 21600");
                    formulaColl.Add("g10", "+- g8 0 g9");
                    formulaColl.Add("g11", "*/ g10 1 3");
                    formulaColl.Add("g12", "*/ ss 1800 21600");
                    formulaColl.Add("g13", "+- g11 g12 0");
                    formulaColl.Add("g14", "*/ g13 g6 g8");
                    formulaColl.Add("g15", "*/ g13 g7 g8");
                    formulaColl.Add("g16", "+- g14 xPos 0");
                    formulaColl.Add("g17", "+- g15 yPos 0");
                    formulaColl.Add("g18", "*/ ss 4800 21600");
                    formulaColl.Add("g19", "*/ g11 2 1");
                    formulaColl.Add("g20", "+- g18 g19 0");
                    formulaColl.Add("g21", "*/ g20 g6 g8");
                    formulaColl.Add("g22", "*/ g20 g7 g8");
                    formulaColl.Add("g23", "+- g21 xPos 0");
                    formulaColl.Add("g24", "+- g22 yPos 0");
                    formulaColl.Add("g25", "*/ ss 1200 21600");
                    formulaColl.Add("g26", "*/ ss 600 21600");
                    formulaColl.Add("x23", "+- xPos g26 0");
                    formulaColl.Add("x24", "+- g16 g25 0");
                    formulaColl.Add("x25", "+- g23 g12 0");
                    formulaColl.Add("il", "*/ w 2977 21600");
                    formulaColl.Add("it", "*/ h 3262 21600");
                    formulaColl.Add("ir", "*/ w 17087 21600");
                    formulaColl.Add("ib", "*/ h 17337 21600");
                    formulaColl.Add("g27", "*/ w 67 21600");
                    formulaColl.Add("g28", "*/ h 21577 21600");
                    formulaColl.Add("g29", "*/ w 21582 21600");
                    formulaColl.Add("g30", "*/ h 1235 21600");
                    formulaColl.Add("pang", "at2 dxPos dyPos");
                    break;
                case AutoShapeType.LineCallout1:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    break;
                case AutoShapeType.LineCallout2:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    formulaColl.Add("y3", "*/ h adj5 100000");
                    formulaColl.Add("x3", "*/ w adj6 100000");
                    break;
                case AutoShapeType.LineCallout3:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    formulaColl.Add("y3", "*/ h adj5 100000");
                    formulaColl.Add("x3", "*/ w adj6 100000");
                    formulaColl.Add("y4", "*/ h adj7 100000");
                    formulaColl.Add("x4", "*/ w adj8 100000");
                    break;
                case AutoShapeType.LineCallout1AccentBar:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    break;
                case AutoShapeType.LineCallout2AccentBar:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    formulaColl.Add("y3", "*/ h adj5 100000");
                    formulaColl.Add("x3", "*/ w adj6 100000");
                    break;
                case AutoShapeType.LineCallout3AccentBar:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    formulaColl.Add("y3", "*/ h adj5 100000");
                    formulaColl.Add("x3", "*/ w adj6 100000");
                    formulaColl.Add("y4", "*/ h adj7 100000");
                    formulaColl.Add("x4", "*/ w adj8 100000");
                    break;
                case AutoShapeType.LineCallout1NoBorder:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    break;
                case AutoShapeType.LineCallout2NoBorder:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    formulaColl.Add("y3", "*/ h adj5 100000");
                    formulaColl.Add("x3", "*/ w adj6 100000");
                    break;
                case AutoShapeType.LineCallout3NoBorder:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    formulaColl.Add("y3", "*/ h adj5 100000");
                    formulaColl.Add("x3", "*/ w adj6 100000");
                    formulaColl.Add("y4", "*/ h adj7 100000");
                    formulaColl.Add("x4", "*/ w adj8 100000");
                    break;
                case AutoShapeType.LineCallout1BorderAndAccentBar:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    break;
                case AutoShapeType.LineCallout2BorderAndAccentBar:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    formulaColl.Add("y3", "*/ h adj5 100000");
                    formulaColl.Add("x3", "*/ w adj6 100000");
                    break;
                case AutoShapeType.LineCallout3BorderAndAccentBar:
                    formulaColl.Add("y1", "*/ h adj1 100000");
                    formulaColl.Add("x1", "*/ w adj2 100000");
                    formulaColl.Add("y2", "*/ h adj3 100000");
                    formulaColl.Add("x2", "*/ w adj4 100000");
                    formulaColl.Add("y3", "*/ h adj5 100000");
                    formulaColl.Add("x3", "*/ w adj6 100000");
                    formulaColl.Add("y4", "*/ h adj7 100000");
                    formulaColl.Add("x4", "*/ w adj8 100000");
                    break;
                #endregion
            }
            return formulaColl;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        private Dictionary<string, float> GetDefaultPathAdjValues(AutoShapeType shapeType)
        {
            Dictionary<string, float> pathAdjustValue = new Dictionary<string, float>();
            switch (shapeType)
            {
                case AutoShapeType.ElbowConnector:
                case AutoShapeType.CurvedConnector:
                    pathAdjustValue.Add("adj1", 50000);
                    break;
                case AutoShapeType.RoundedRectangle:
                case AutoShapeType.SnipSingleCornerRectangle:
                case AutoShapeType.RoundSingleCornerRectangle:
                case AutoShapeType.FoldedCorner:
                case AutoShapeType.Plaque:
                case AutoShapeType.DoubleBracket:
                    pathAdjustValue.Add("adj", 16667);
                    break;
                case AutoShapeType.IsoscelesTriangle:
                case AutoShapeType.DiagonalStripe:
                case AutoShapeType.Moon:
                case AutoShapeType.Chevron:
                case AutoShapeType.Pentagon:
                    pathAdjustValue.Add("adj", 50000);
                    break;
                case AutoShapeType.SnipSameSideCornerRectangle:
                case AutoShapeType.RoundSameSideCornerRectangle:
                case AutoShapeType.RoundDiagonalCornerRectangle:
                    pathAdjustValue.Add("adj1", 16667);
                    pathAdjustValue.Add("adj2", 0);
                    break;
                case AutoShapeType.SnipDiagonalCornerRectangle:
                    pathAdjustValue.Add("adj2", 16667);
                    pathAdjustValue.Add("adj1", 0);
                    break;
                case AutoShapeType.SnipAndRoundSingleCornerRectangle:
                    pathAdjustValue.Add("adj1", 16667);
                    pathAdjustValue.Add("adj2", 16667);
                    break;
                case AutoShapeType.Parallelogram:
                case AutoShapeType.Trapezoid:
                    pathAdjustValue.Add("adj", 25000);
                    break;
                case AutoShapeType.RegularPentagon:
                    pathAdjustValue.Add("hf", 105146);
                    pathAdjustValue.Add("vf", 110557);
                    break;
                case AutoShapeType.Hexagon:
                    pathAdjustValue.Add("adj", 25000);
                    pathAdjustValue.Add("vf", 115470);
                    break;
                case AutoShapeType.Heptagon:
                    pathAdjustValue.Add("hf", 102572);
                    pathAdjustValue.Add("vf", 105210);
                    break;
                case AutoShapeType.Octagon:
                    pathAdjustValue.Add("adj", 29289);
                    break;
                case AutoShapeType.Decagon:
                    pathAdjustValue.Add("vf", 105146);
                    break;
                case AutoShapeType.Pie:
                    pathAdjustValue.Add("adj1", 0);
                    pathAdjustValue.Add("adj2", 270);
                    break;
                case AutoShapeType.Chord:
                    pathAdjustValue.Add("adj1", 45);
                    pathAdjustValue.Add("adj2", 270);
                    break;
                case AutoShapeType.Teardrop:
                    pathAdjustValue.Add("adj", 100000);
                    break;
                case AutoShapeType.Frame:
                    pathAdjustValue.Add("adj1", 12500);
                    break;
                case AutoShapeType.HalfFrame:
                    pathAdjustValue.Add("adj1", 33333);
                    pathAdjustValue.Add("adj2", 33333);
                    break;
                case AutoShapeType.L_Shape:
                case AutoShapeType.RightArrow:
                case AutoShapeType.LeftArrow:
                case AutoShapeType.DownArrow:
                case AutoShapeType.LeftRightArrow:
                case AutoShapeType.UpDownArrow:
                case AutoShapeType.StripedRightArrow:
                case AutoShapeType.NotchedRightArrow:
                case AutoShapeType.UpArrow:
                    pathAdjustValue.Add("adj1", 50000);
                    pathAdjustValue.Add("adj2", 50000);
                    break;
                case AutoShapeType.Cross:
                case AutoShapeType.Can:
                case AutoShapeType.Cube:
                case AutoShapeType.Donut:
                case AutoShapeType.Sun:
                    pathAdjustValue.Add("adj", 25000);
                    break;
                case AutoShapeType.Bevel:
                case AutoShapeType.Star4Point:
                case AutoShapeType.VerticalScroll:
                case AutoShapeType.HorizontalScroll:
                    pathAdjustValue.Add("adj", 12500);
                    break;
                case AutoShapeType.NoSymbol:
                    pathAdjustValue.Add("adj", 18750);
                    break;
                case AutoShapeType.BlockArc:
                    pathAdjustValue.Add("adj1", 10800000);
                    pathAdjustValue.Add("adj2", 0);
                    pathAdjustValue.Add("adj3", 25000);
                    break;
                case AutoShapeType.SmileyFace:
                    pathAdjustValue.Add("adj", 4653);
                    break;
                case AutoShapeType.Arc:
                    pathAdjustValue.Add("adj1", 16200000);
                    pathAdjustValue.Add("adj2", 0);
                    break;
                case AutoShapeType.DoubleBrace:
                case AutoShapeType.LeftBracket:
                case AutoShapeType.RightBracket:
                    pathAdjustValue.Add("adj", 8333);
                    break;
                case AutoShapeType.LeftBrace:
                case AutoShapeType.RightBrace:
                    pathAdjustValue.Add("adj1", 8333);
                    pathAdjustValue.Add("adj2", 50000);
                    break;
                case AutoShapeType.QuadArrow:
                case AutoShapeType.LeftRightUpArrow:
                    pathAdjustValue.Add("adj1", 22500);
                    pathAdjustValue.Add("adj2", 22500);
                    pathAdjustValue.Add("adj3", 22500);
                    break;
                case AutoShapeType.BentArrow:
                    pathAdjustValue.Add("adj1", 25000);
                    pathAdjustValue.Add("adj2", 25000);
                    pathAdjustValue.Add("adj3", 25000);
                    pathAdjustValue.Add("adj4", 43750);
                    break;
                case AutoShapeType.UTurnArrow:
                    pathAdjustValue.Add("adj1", 25000);
                    pathAdjustValue.Add("adj2", 25000);
                    pathAdjustValue.Add("adj3", 25000);
                    pathAdjustValue.Add("adj4", 43750);
                    pathAdjustValue.Add("adj5", 75000);
                    break;
                case AutoShapeType.LeftUpArrow:
                case AutoShapeType.BentUpArrow:
                    pathAdjustValue.Add("adj1", 25000);
                    pathAdjustValue.Add("adj2", 25000);
                    pathAdjustValue.Add("adj3", 25000);
                    break;
                case AutoShapeType.CurvedRightArrow:
                case AutoShapeType.CurvedLeftArrow:
                case AutoShapeType.CurvedUpArrow:
                case AutoShapeType.CurvedDownArrow:
                    pathAdjustValue.Add("adj1", 25000);
                    pathAdjustValue.Add("adj2", 50000);
                    pathAdjustValue.Add("adj3", 25000);
                    break;
                case AutoShapeType.RightArrowCallout:
                case AutoShapeType.LeftArrowCallout:
                case AutoShapeType.UpArrowCallout:
                case AutoShapeType.DownArrowCallout:
                    pathAdjustValue.Add("adj1", 25000);
                    pathAdjustValue.Add("adj2", 25000);
                    pathAdjustValue.Add("adj3", 25000);
                    pathAdjustValue.Add("adj4", 64977);
                    break;
                case AutoShapeType.LeftRightArrowCallout:
                case AutoShapeType.UpDownArrowCallout:
                    pathAdjustValue.Add("adj1", 25000);
                    pathAdjustValue.Add("adj2", 25000);
                    pathAdjustValue.Add("adj3", 25000);
                    pathAdjustValue.Add("adj4", 48123);
                    break;
                case AutoShapeType.QuadArrowCallout:
                    pathAdjustValue.Add("adj1", 18515);
                    pathAdjustValue.Add("adj2", 18515);
                    pathAdjustValue.Add("adj3", 18515);
                    pathAdjustValue.Add("adj4", 48123);
                    break;
                case AutoShapeType.CircularArrow:
                    pathAdjustValue.Add("adj1", 12500);
                    pathAdjustValue.Add("adj2", 19);
                    pathAdjustValue.Add("adj3", 341);
                    pathAdjustValue.Add("adj4", 180);
                    pathAdjustValue.Add("adj5", 12500);
                    break;
                case AutoShapeType.MathPlus:
                case AutoShapeType.MathMinus:
                case AutoShapeType.MathMultiply:
                    pathAdjustValue.Add("adj1", 23520);
                    break;
                case AutoShapeType.MathDivision:
                    pathAdjustValue.Add("adj1", 23520);
                    pathAdjustValue.Add("adj2", 5880);
                    pathAdjustValue.Add("adj3", 11760);
                    break;
                case AutoShapeType.MathEqual:
                    pathAdjustValue.Add("adj1", 23520);
                    pathAdjustValue.Add("adj2", 11760);
                    break;
                case AutoShapeType.MathNotEqual:
                    pathAdjustValue.Add("adj1", 23520);
                    pathAdjustValue.Add("adj2", 6600000);
                    pathAdjustValue.Add("adj3", 11760);
                    break;
                case AutoShapeType.Star5Point:
                    pathAdjustValue.Add("adj", 19098);
                    pathAdjustValue.Add("hf", 105146);
                    pathAdjustValue.Add("vf", 110557);
                    break;
                case AutoShapeType.Star6Point:
                    pathAdjustValue.Add("adj", 28868);
                    pathAdjustValue.Add("hf", 115470);
                    break;
                case AutoShapeType.Star7Point:
                    pathAdjustValue.Add("adj", 34601);
                    pathAdjustValue.Add("hf", 102572);
                    pathAdjustValue.Add("vf", 105210);
                    break;
                case AutoShapeType.Star8Point:
                case AutoShapeType.Star12Point:
                case AutoShapeType.Star16Point:
                case AutoShapeType.Star24Point:
                case AutoShapeType.Star32Point:
                    pathAdjustValue.Add("adj", 37500);
                    break;
                case AutoShapeType.Star10Point:
                    pathAdjustValue.Add("adj", 42533);
                    pathAdjustValue.Add("hf", 105146);
                    break;
                case AutoShapeType.UpRibbon:
                case AutoShapeType.DownRibbon:
                    pathAdjustValue.Add("adj1", 16667);
                    pathAdjustValue.Add("adj2", 50000);
                    break;
                case AutoShapeType.CurvedUpRibbon:
                case AutoShapeType.CurvedDownRibbon:
                    pathAdjustValue.Add("adj1", 25000);
                    pathAdjustValue.Add("adj2", 50000);
                    pathAdjustValue.Add("adj3", 12500);
                    break;
                case AutoShapeType.Wave:
                    pathAdjustValue.Add("adj1", 12500);
                    pathAdjustValue.Add("adj2", 0);
                    break;
                case AutoShapeType.DoubleWave:
                    pathAdjustValue.Add("adj1", 6250);
                    pathAdjustValue.Add("adj2", 0);
                    break;
                case AutoShapeType.RectangularCallout:
                case AutoShapeType.OvalCallout:
                case AutoShapeType.CloudCallout:
                    pathAdjustValue.Add("adj1", -20833);
                    pathAdjustValue.Add("adj2", 62500);
                    break;
                case AutoShapeType.RoundedRectangularCallout:
                    pathAdjustValue.Add("adj1", -20833);
                    pathAdjustValue.Add("adj2", 62500);
                    pathAdjustValue.Add("adj3", 16667);
                    break;
                case AutoShapeType.LineCallout1:
                case AutoShapeType.LineCallout1AccentBar:
                case AutoShapeType.LineCallout1NoBorder:
                case AutoShapeType.LineCallout1BorderAndAccentBar:
                    pathAdjustValue.Add("adj1", 18750);
                    pathAdjustValue.Add("adj2", -8333);
                    pathAdjustValue.Add("adj3", 112500);
                    pathAdjustValue.Add("adj4", -38333);
                    break;
                case AutoShapeType.LineCallout2:
                case AutoShapeType.LineCallout2AccentBar:
                case AutoShapeType.LineCallout2NoBorder:
                case AutoShapeType.LineCallout2BorderAndAccentBar:
                    pathAdjustValue.Add("adj1", 18750);
                    pathAdjustValue.Add("adj2", -8333);
                    pathAdjustValue.Add("adj3", 18750);
                    pathAdjustValue.Add("adj4", -16667);
                    pathAdjustValue.Add("adj5", 112500);
                    pathAdjustValue.Add("adj6", -46667);
                    break;
                case AutoShapeType.LineCallout3:
                case AutoShapeType.LineCallout3AccentBar:
                case AutoShapeType.LineCallout3NoBorder:
                case AutoShapeType.LineCallout3BorderAndAccentBar:
                    pathAdjustValue.Add("adj1", 18750);
                    pathAdjustValue.Add("adj2", -8333);
                    pathAdjustValue.Add("adj3", 18750);
                    pathAdjustValue.Add("adj4", -16667);
                    pathAdjustValue.Add("adj5", 100000);
                    pathAdjustValue.Add("adj6", -16667);
                    pathAdjustValue.Add("adj7", 112963);
                    pathAdjustValue.Add("adj8", -8333);
                    break;
            }
            return pathAdjustValue;
        }
        #endregion
    }
}
