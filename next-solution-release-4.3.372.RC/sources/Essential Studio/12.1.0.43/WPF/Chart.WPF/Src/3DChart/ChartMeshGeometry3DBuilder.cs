// <copyright file="ChartMeshGeometry3DBuilder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represent ChartMeshGeometry3DAxis Enum
    /// </summary>
    public enum ChartMeshGeometry3DAxis
    {
        /// <summary> 
        /// The X value
        /// </summary>
        X,

        /// <summary>
        /// The Y value
        /// </summary>
        Y,

        /// <summary>
        /// The Z value 
        /// </summary>
        Z
    }

    /// <summary>
    /// Represents ChartMeshGeometry3DBuilder class
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class ChartMeshGeometry3DBuilder
    {
        #region Constants
        /// <summary>
        /// The Half value
        /// </summary>
        private const double HALF = 0.5;

        /// <summary>
        /// The TWO_PI value
        /// </summary>
        private const double TWO_PI = 2 * Math.PI;

        /// <summary>
        /// The R_TO_D value
        /// </summary>
        private const double R_TO_D = 180d / Math.PI;

        /// <summary>
        /// The D_TO_R value
        /// </summary>
        private const double D_TO_R = Math.PI / 180d;
        #endregion

        #region Public methods
        /// <summary>
        /// MeshGeometry3D Build Plane
        /// </summary>
        /// <param name="size">The size value</param>
        /// /// <param name="axis">The axis value</param>
        /// <returns>Returns the writer Geometry value</returns>
        public static MeshGeometry3D BuildPlane(Size size, ChartMeshGeometry3DAxis axis)
        {
            ChartMeshGeometry3DWriter writer = new ChartMeshGeometry3DWriter();
            if (axis == ChartMeshGeometry3DAxis.X)
            {
                Point3D pt1 = new Point3D(0, -HALF * size.Width, -HALF * size.Height);
                Point3D pt2 = new Point3D(0, HALF * size.Width, -HALF * size.Height);
                Point3D pt3 = new Point3D(0, HALF * size.Width, HALF * size.Height);
                Point3D pt4 = new Point3D(0, -HALF * size.Width, HALF * size.Height);

                writer.WriteSolidQuadrangle(pt1, new Point(0, 0), pt2, new Point(1, 0), pt3, new Point(1, 1), pt4, new Point(0, 1));
            }
            else if (axis == ChartMeshGeometry3DAxis.Y)
            {
                Point3D pt1 = new Point3D(-HALF * size.Width, 0, -HALF * size.Height);
                Point3D pt2 = new Point3D(HALF * size.Width, 0, -HALF * size.Height);
                Point3D pt3 = new Point3D(HALF * size.Width, 0, HALF * size.Height);
                Point3D pt4 = new Point3D(-HALF * size.Width, 0, HALF * size.Height);

                writer.WriteSolidQuadrangle(pt1, new Point(0, 0), pt2, new Point(1, 0), pt3, new Point(1, 1), pt4, new Point(0, 1));
            }
            else if (axis == ChartMeshGeometry3DAxis.Z)
            {
                Point3D pt1 = new Point3D(-HALF * size.Width, -HALF * size.Height, 0);
                Point3D pt2 = new Point3D(HALF * size.Width, -HALF * size.Height, 0);
                Point3D pt3 = new Point3D(HALF * size.Width, HALF * size.Height, 0);
                Point3D pt4 = new Point3D(-HALF * size.Width, HALF * size.Height, 0);

                writer.WriteSolidQuadrangle(pt1, new Point(0, 0), pt2, new Point(1, 0), pt3, new Point(1, 1), pt4, new Point(0, 1));
            }

            return writer.Geometry;
        }

        /// <summary>
        /// The BuildBox method
        /// </summary>
        /// <param name="size">The size value</param>
        /// <returns>Returns writer.Geometry</returns>
        public static MeshGeometry3D BuildBox(Size3D size)
        {
            Point3D[] points = Generate(GetHalfSize(size));
            ChartMeshGeometry3DWriter writer = new ChartMeshGeometry3DWriter();

            writer.WriteSolidQuadrangle(points[3], new Point(0, 0), points[2], new Point(1, 0), points[1], new Point(1, 1), points[0], new Point(0, 1));

            writer.WriteSolidQuadrangle(points[1], new Point(0, 0), points[5], new Point(1, 0), points[4], new Point(1, 1), points[0], new Point(0, 1));

            writer.WriteSolidQuadrangle(points[2], new Point(0, 0), points[6], new Point(1, 0), points[5], new Point(1, 1), points[1], new Point(0, 1));

            writer.WriteSolidQuadrangle(points[3], new Point(0, 0), points[7], new Point(1, 0), points[6], new Point(1, 1), points[2], new Point(0, 1));

            writer.WriteSolidQuadrangle(points[0], new Point(0, 0), points[4], new Point(1, 0), points[7], new Point(1, 1), points[3], new Point(0, 1));

            writer.WriteSolidQuadrangle(points[5], new Point(0, 0), points[6], new Point(1, 0), points[7], new Point(1, 1), points[4], new Point(0, 1));

            return writer.Geometry;
        }

        /// <summary>
        /// BuildRounedBox mwthod
        /// </summary>
        /// <param name="size">The size value</param>
        /// <param name="radius">The radius value</param>
        /// <returns>Returns writer.Geometry</returns>
        public static MeshGeometry3D BuildRounedBox(Size3D size, Vector3D radius)
        {
            Size3D halfSize = GetHalfSize(size);
            Point3D[] pointsX = Generate(SubtractMin(halfSize, new Size3D(0, radius.Y, radius.Z)));
            Point3D[] pointsY = Generate(SubtractMin(halfSize, new Size3D(radius.X, 0, radius.Z)));
            Point3D[] pointsZ = Generate(SubtractMin(halfSize, new Size3D(radius.X, radius.Y, 0)));
            ChartMeshGeometry3DWriter writer = new ChartMeshGeometry3DWriter();

            writer.WriteSmoothQuadrangle(pointsZ[3], new Point(0, 0), pointsZ[2], new Point(1, 0), pointsZ[1], new Point(1, 1), pointsZ[0], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsZ[5], new Point(0, 0), pointsZ[6], new Point(1, 0), pointsZ[7], new Point(1, 1), pointsZ[4], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsY[1], new Point(0, 0), pointsY[5], new Point(1, 0), pointsY[4], new Point(1, 1), pointsY[0], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsY[3], new Point(0, 0), pointsY[7], new Point(1, 0), pointsY[6], new Point(1, 1), pointsY[2], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsX[2], new Point(0, 0), pointsX[6], new Point(1, 0), pointsX[5], new Point(1, 1), pointsX[1], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsX[0], new Point(0, 0), pointsX[4], new Point(1, 0), pointsX[7], new Point(1, 1), pointsX[3], new Point(0, 1));

            writer.WriteSmoothTriangle(pointsZ[7], new Point(), pointsY[7], new Point(), pointsX[7], new Point());
            writer.WriteSmoothTriangle(pointsX[6], new Point(), pointsY[6], new Point(), pointsZ[6], new Point());
            writer.WriteSmoothTriangle(pointsZ[5], new Point(), pointsY[5], new Point(), pointsX[5], new Point());
            writer.WriteSmoothTriangle(pointsX[4], new Point(), pointsY[4], new Point(), pointsZ[4], new Point());
            writer.WriteSmoothTriangle(pointsX[3], new Point(), pointsY[3], new Point(), pointsZ[3], new Point());
            writer.WriteSmoothTriangle(pointsZ[2], new Point(), pointsY[2], new Point(), pointsX[2], new Point());
            writer.WriteSmoothTriangle(pointsX[1], new Point(), pointsY[1], new Point(), pointsZ[1], new Point());
            writer.WriteSmoothTriangle(pointsZ[0], new Point(), pointsY[0], new Point(), pointsX[0], new Point());

            writer.WriteSmoothQuadrangle(pointsZ[7], new Point(0, 0), pointsZ[6], new Point(1, 0), pointsY[6], new Point(1, 1), pointsY[7], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsX[6], new Point(0, 0), pointsX[2], new Point(1, 0), pointsY[2], new Point(1, 1), pointsY[6], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsZ[2], new Point(0, 0), pointsZ[3], new Point(1, 0), pointsY[3], new Point(1, 1), pointsY[2], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsX[3], new Point(0, 0), pointsX[7], new Point(1, 0), pointsY[7], new Point(1, 1), pointsY[3], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsZ[5], new Point(0, 0), pointsZ[4], new Point(1, 0), pointsY[4], new Point(1, 1), pointsY[5], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsX[4], new Point(0, 0), pointsX[0], new Point(1, 0), pointsY[0], new Point(1, 1), pointsY[4], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsZ[0], new Point(0, 0), pointsZ[1], new Point(1, 0), pointsY[1], new Point(1, 1), pointsY[0], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsX[1], new Point(0, 0), pointsX[5], new Point(1, 0), pointsY[5], new Point(1, 1), pointsY[1], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsZ[3], new Point(0, 0), pointsZ[0], new Point(1, 0), pointsX[0], new Point(1, 1), pointsX[3], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsZ[1], new Point(0, 0), pointsZ[2], new Point(1, 0), pointsX[2], new Point(1, 1), pointsX[1], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsZ[6], new Point(0, 0), pointsZ[5], new Point(1, 0), pointsX[5], new Point(1, 1), pointsX[6], new Point(0, 1));

            writer.WriteSmoothQuadrangle(pointsZ[4], new Point(0, 0), pointsZ[7], new Point(1, 0), pointsX[7], new Point(1, 1), pointsX[4], new Point(0, 1));

            return writer.Geometry;
        }

        /// <summary>
        /// Build Shpere method
        /// </summary>
        /// <param name="size">The size value</param>
        /// <param name="desc">The desc value</param>
        /// <returns>Returns writer.Geometry</returns>
        public static MeshGeometry3D BuildShpere(Size3D size, int desc)
        {
            Size3D halfSize = GetHalfSize(size);
            ChartMeshGeometry3DWriter writer = new ChartMeshGeometry3DWriter();
            double coef = TWO_PI / desc;

            for (int i = 0; i < desc; i++)
            {
                double sin11 = Math.Sin(i * coef);
                double cos11 = Math.Cos(i * coef);
                double sin12 = Math.Sin(i * coef + coef);
                double cos12 = Math.Cos(i * coef + coef);

                for (int j = 0; j < desc; j++)
                {
                    double sin21 = Math.Sin(j * coef);
                    double cos21 = Math.Cos(j * coef);
                    double sin22 = Math.Sin(j * coef + coef);
                    double cos22 = Math.Cos(j * coef + coef);

                    Point3D pt1 = new Point3D(halfSize.X * sin11 * cos21, halfSize.Y * cos11, halfSize.Z * sin11 * sin21);
                    Point3D pt2 = new Point3D(halfSize.X * sin12 * cos21, halfSize.Y * cos12, halfSize.Z * sin12 * sin21);
                    Point3D pt3 = new Point3D(halfSize.X * sin12 * cos22, halfSize.Y * cos12, halfSize.Z * sin12 * sin22);
                    Point3D pt4 = new Point3D(halfSize.X * sin11 * cos22, halfSize.Y * cos11, halfSize.Z * sin11 * sin22);

                    Vector3D n1 = new Vector3D(sin11 * cos21, cos11, sin11 * sin21);
                    Vector3D n2 = new Vector3D(sin12 * cos21, cos12, sin12 * sin21);
                    Vector3D n3 = new Vector3D(sin12 * cos22, cos12, sin12 * sin22);
                    Vector3D n4 = new Vector3D(sin11 * cos22, cos11, sin11 * sin22);

                    writer.WriteSmoothQuadrangle(pt1, n1, new Point(), pt2, n2, new Point(), pt3, n3, new Point(), pt4, n4, new Point());
                }
            }

            return writer.Geometry;
        }

        /// <summary>
        /// The Build Cylinder method
        /// </summary>
        /// <param name="size">The size value</param>
        /// <param name="desc">The desc value</param>
        /// <param name="axis">The axis value</param>
        /// <returns>Returns writer.Geometry</returns>
        public static MeshGeometry3D BuildCylinder(Size3D size, int desc, ChartMeshGeometry3DAxis axis)
        {
            Size3D halfSize = GetHalfSize(size);
            ChartMeshGeometry3DWriter writer = new ChartMeshGeometry3DWriter();
            double coef = TWO_PI / desc;

            Point3D upPoint = new Point3D(
                axis == ChartMeshGeometry3DAxis.X ? -halfSize.X : 0,
                axis == ChartMeshGeometry3DAxis.Y ? -halfSize.Y : 0,
                axis == ChartMeshGeometry3DAxis.Z ? -halfSize.Z : 0);
            Point3D downPoint = new Point3D(
                axis == ChartMeshGeometry3DAxis.X ? halfSize.X : 0,
                axis == ChartMeshGeometry3DAxis.Y ? halfSize.Y : 0,
                axis == ChartMeshGeometry3DAxis.Z ? halfSize.Z : 0);

            for (int i = 0; i < desc; i++)
            {
                double sin1 = Math.Sin(i * coef);
                double cos1 = Math.Cos(i * coef);
                double sin2 = Math.Sin(i * coef + coef);
                double cos2 = Math.Cos(i * coef + coef);

                if (axis == ChartMeshGeometry3DAxis.X)
                {
                    Point3D pt1 = new Point3D(-halfSize.X, halfSize.Y * sin2, halfSize.Z * cos2);
                    Point3D pt2 = new Point3D(-halfSize.X, halfSize.Y * sin1, halfSize.Z * cos1);
                    Point3D pt3 = new Point3D(halfSize.X, halfSize.Y * sin1, halfSize.Z * cos1);
                    Point3D pt4 = new Point3D(halfSize.X, halfSize.Y * sin2, halfSize.Z * cos2);

                    writer.WriteSmoothTriangle(upPoint, new Point(), pt2, new Point(), pt1, new Point());
                    writer.WriteSmoothTriangle(downPoint, new Point(), pt4, new Point(), pt3, new Point());
                    writer.WriteSolidQuadrangle(pt1, new Point(), pt2, new Point(), pt3, new Point(), pt4, new Point());
                }
                else if (axis == ChartMeshGeometry3DAxis.Y)
                {
                    Point3D pt1 = new Point3D(halfSize.X * sin2, -halfSize.Y, halfSize.Z * cos2);
                    Point3D pt2 = new Point3D(halfSize.X * sin1, -halfSize.Y, halfSize.Z * cos1);
                    Point3D pt3 = new Point3D(halfSize.X * sin1, halfSize.Y, halfSize.Z * cos1);
                    Point3D pt4 = new Point3D(halfSize.X * sin2, halfSize.Y, halfSize.Z * cos2);

                    writer.WriteSmoothTriangle(upPoint, new Point(), pt2, new Point(), pt1, new Point());
                    writer.WriteSmoothTriangle(downPoint, new Point(), pt4, new Point(), pt3, new Point());
                    writer.WriteSolidQuadrangle(pt1, new Point(), pt2, new Point(), pt3, new Point(), pt4, new Point());
                }
                else if (axis == ChartMeshGeometry3DAxis.Z)
                {
                    Point3D pt1 = new Point3D(halfSize.X * cos2, halfSize.Y * sin2, -halfSize.Z);
                    Point3D pt2 = new Point3D(halfSize.X * cos1, halfSize.Y * sin1, -halfSize.Z);
                    Point3D pt3 = new Point3D(halfSize.X * cos1, halfSize.Y * sin1, halfSize.Z);
                    Point3D pt4 = new Point3D(halfSize.X * cos2, halfSize.Y * sin2, halfSize.Z);

                    writer.WriteSmoothTriangle(upPoint, new Point(), pt2, new Point(), pt1, new Point());
                    writer.WriteSmoothTriangle(downPoint, new Point(), pt4, new Point(), pt3, new Point());
                    writer.WriteSolidQuadrangle(pt1, new Point(), pt2, new Point(), pt3, new Point(), pt4, new Point());
                }
            }

            return writer.Geometry;
        }

        /// <summary>
        /// BuildCylindricPolyline method
        /// </summary>
        /// <param name="points">The points</param>
        /// <param name="axis">The axis value</param>
        /// <param name="desc">The desc value</param>
        /// <param name="useSolid">The use solid</param>
        /// <param name="IsExploded"></param>
        /// <returns>Returns writer.Geometry</returns>
        public static MeshGeometry3D BuildCylindricPolyline(Point[] points, ChartMeshGeometry3DAxis axis, int desc, bool useSolid,bool IsExploded)
        {
            ChartMeshGeometry3DWriter writer = new ChartMeshGeometry3DWriter();
            double stepAngle = TWO_PI / desc;
            double explodedDistance = IsExploded ? 0.15 : 0;
            for (int i = 0; i < desc; i++)
            {
                Vector fVector = new Vector(Math.Cos(i * stepAngle), Math.Sin(i * stepAngle));
                Vector nVector = new Vector(Math.Cos((i + 1) * stepAngle), Math.Sin((i + 1) * stepAngle));

                for (int j = 0; j < points.Length - 1; j++)
                {
                    if (axis == ChartMeshGeometry3DAxis.X)
                    {
                        Point3D pt1 = new Point3D(points[j].X, points[j].Y * fVector.X, points[j].Y * fVector.Y);
                        Point3D pt2 = new Point3D(points[j].X, points[j].Y * nVector.X, points[j].Y * nVector.Y);
                        Point3D pt3 = new Point3D(points[j + 1].X, points[j + 1].Y * fVector.X, points[j + 1].Y * fVector.Y);
                        Point3D pt4 = new Point3D(points[j + 1].X, points[j + 1].Y * nVector.X, points[j + 1].Y * nVector.Y);

                        if (useSolid)
                        {
                            writer.WriteSolidQuadrangle(pt4, new Point(), pt3, new Point(), pt2, new Point(), pt1, new Point());
                        }
                        else
                        {
                            writer.WriteSmoothQuadrangle(pt4, new Point(), pt3, new Point(), pt2, new Point(), pt1, new Point());
                        }
                    }
                    else if (axis == ChartMeshGeometry3DAxis.Y)
                    {
                        Point3D pt1 = new Point3D(points[j].Y * fVector.X + explodedDistance, points[j].X, points[j].Y * fVector.Y);
                        Point3D pt2 = new Point3D(points[j].Y * nVector.X + explodedDistance, points[j].X, points[j].Y * nVector.Y);
                        Point3D pt3 = new Point3D(points[j + 1].Y * nVector.X + explodedDistance, points[j + 1].X, points[j + 1].Y * nVector.Y);
                        Point3D pt4 = new Point3D(points[j + 1].Y * fVector.X + explodedDistance, points[j + 1].X, points[j + 1].Y * fVector.Y);

                        if (useSolid)
                        {
                            if (points[j].X == points[j + 1].X)
                            {
                                writer.WriteSolidQuadrangle(pt4, new Point(), pt3, new Point(), pt2, new Point(), pt1, new Point(), new Vector3D(0, 1, 0));
                            }
                            else
                            {
                                writer.WriteSolidQuadrangle(pt4, new Point(), pt3, new Point(), pt2, new Point(), pt1, new Point());
                            }
                        }
                        else
                        {
                            writer.WriteSmoothQuadrangle(pt4, new Point(), pt3, new Point(), pt2, new Point(), pt1, new Point());
                        }
                    }
                    else if (axis == ChartMeshGeometry3DAxis.Z)
                    {
                        Point3D pt1 = new Point3D(points[j].Y * fVector.X, points[j].Y * fVector.Y, points[j].X);
                        Point3D pt2 = new Point3D(points[j].Y * nVector.X, points[j].Y * nVector.Y, points[j].X);
                        Point3D pt3 = new Point3D(points[j + 1].Y * nVector.X, points[j + 1].Y * nVector.Y *3, points[j + 1].X);
                        Point3D pt4 = new Point3D(points[j + 1].Y * fVector.X, points[j + 1].Y * fVector.Y *3, points[j + 1].X);

                        if (useSolid)
                        {
                            writer.WriteSolidQuadrangle(pt4, new Point(), pt3, new Point(), pt2, new Point(), pt1, new Point());
                        }
                        else
                        {
                            writer.WriteSmoothQuadrangle(pt4, new Point(), pt3, new Point(), pt2, new Point(), pt1, new Point());
                        }
                    }
                }
            }

            return writer.Geometry;
        }
        #endregion

        #region Helper methdos

        /// <summary>
        /// Gets the size of the half.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <returns>Returns the Half size</returns>
        private static Size3D GetHalfSize(Size3D size)
        {
            return new Size3D(HALF * size.X, HALF * size.Y, HALF * size.Z);
        }

        /// <summary>
        /// Generate method to return Point3D
        /// </summary>
        /// <param name="size">The size value</param>
        /// <returns>Returns Point3D</returns>
        private static Point3D[] Generate(Size3D size)
        {
            return new Point3D[] 
            { 
                new Point3D(-size.X, -size.Y, -size.Z),
        new Point3D(size.X, -size.Y, -size.Z),
        new Point3D(size.X, size.Y, -size.Z),
        new Point3D(-size.X, size.Y, -size.Z),
        new Point3D(-size.X, -size.Y, size.Z),
        new Point3D(size.X, -size.Y, size.Z),
        new Point3D(size.X, size.Y, size.Z),
        new Point3D(-size.X, size.Y, size.Z) 
            };
        }

        /// <summary>
        /// SubtractMin method
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="sz">The size value</param>
        /// <returns>Returns Size3D value</returns>
        /// <seealso cref="ChartMeshGeometry3DBuilder"/>
        private static Size3D SubtractMin(Size3D source, Size3D sz)
        {
            return new Size3D(
                source.X > sz.X ? source.X - sz.X : 0,
                source.Y > sz.Y ? source.Y - sz.Y : 0,
                source.Z > sz.Z ? source.Z - sz.Z : 0);
        }
        #endregion
    }
}
