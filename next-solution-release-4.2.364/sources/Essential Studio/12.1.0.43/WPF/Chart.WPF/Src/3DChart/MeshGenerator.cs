// <copyright file="MeshGenerator.cs" company="Syncfusion">
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
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Windows.Controls;
    
    /// <summary>
    /// Represents MeshGenerator
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal sealed class MeshGenerator
    {
        #region Contants
        /// <summary>
        /// Initializes PI_IN_RADIAL
        /// </summary>
        private const double PI_IN_RADIAL = 180;

        /// <summary>
        /// Initializes DOUBLE_PI_IN_RADIAL
        /// </summary>
        private const double DOUBLE_PI_IN_RADIAL = 360;

        /// <summary>
        /// Initializes DOUBLE_PI
        /// </summary>
        private const double DOUBLE_PI = 2 * Math.PI;

        /// <summary>
        /// Initializes HALF_PI
        /// </summary>
        private const double HALF_PI = 0.5 * Math.PI;

        /// <summary>
        /// Initializes RADIAL_TO_PI
        /// </summary>
        private const double RADIAL_TO_PI = Math.PI / PI_IN_RADIAL;

        /// <summary>
        /// Initializes PI_TO_RADIAL
        /// </summary>
        private const double PI_TO_RADIAL = PI_IN_RADIAL / Math.PI;

        /// <summary>
        /// Initializes BOX_POINT_INDEXES
        /// </summary>
        private static int[] box_POINT_INDEXES = new int[] { 0, 2, 1, 0, 3, 2, 0, 5, 4, 0, 1, 5, 1, 6, 5, 1, 2, 6, 2, 7, 6, 2, 3, 7, 3, 4, 7, 3, 0, 4, 4, 5, 6, 4, 7, 6 };

        /// <summary>
        /// Initializes PYRAMID_POINT_INDEXES
        /// </summary>
        private static int[] pyramid_POINT_INDEXES = new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 1, 1, 4, 3, 1, 3, 2 };
        #endregion

        #region Public methods
        /// <summary>
        /// Cones the specified radius.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="sect">The sect value.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D Cone(double radius, double height, int sect)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double angl = 0;
            double coef = DOUBLE_PI / sect;
            double top = 0.5 * height;
            double bottom = -0.5 * height;

            res.Positions.Add(new Point3D(0, top, 0));
            res.Positions.Add(new Point3D(0, bottom, 0));

            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, -1, 0));

            for (int i = 0; i < sect; i++)
            {
                double sin = Math.Sin(angl);
                double cos = Math.Cos(angl);

                res.Positions.Add(new Point3D(radius * cos, bottom, radius * sin));
                res.Normals.Add(new Vector3D(cos, bottom, sin));

                int prev = i == 0 ? sect - 1 : i - 1;

                res.TriangleIndices.Add(0);
                res.TriangleIndices.Add(2 + i);
                res.TriangleIndices.Add(2 + prev);

                res.TriangleIndices.Add(1);
                res.TriangleIndices.Add(2 + prev);
                res.TriangleIndices.Add(2 + i);

                angl += coef;
            }

            return res;
        }

        /// <summary>
        /// Cones the specified top radius.
        /// </summary>
        /// <param name="topRadius">The top radius.</param>
        /// <param name="bottomRadius">The bottom radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="sect">The sect value.</param>
        /// <param name="IsExploded">Exploded boolean value</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D Cone(double topRadius, double bottomRadius, double height, int sect,bool IsExploded)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            double angl = 0;
            double coef = DOUBLE_PI / sect;
            double top = 0.5 * height;
            double bottom = -0.5 * height; 

            double explodedDistance = IsExploded ? 0.15 : 0;

            res.Positions.Add(new Point3D(0+explodedDistance, top, 0));
            res.Positions.Add(new Point3D(0+explodedDistance, bottom, 0));

            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, -1, 0));

            //res.TextureCoordinates.Add(new Point(0, 0));
            //res.TextureCoordinates.Add(new Point(0, 1));
            //res.TextureCoordinates.Add(new Point(1, 0));
            //res.TextureCoordinates.Add(new Point(1, 1));

           
            for (int i = 0; i < sect; i++)
            {
                double sin = Math.Sin(angl);
                double cos = Math.Cos(angl);
                res.Positions.Add(new Point3D(topRadius * cos + explodedDistance, top , topRadius * sin));
                res.Positions.Add(new Point3D(bottomRadius * cos + explodedDistance, bottom , bottomRadius * sin));

                res.Normals.Add(new Vector3D(cos, top, sin));
                res.Normals.Add(new Vector3D(cos, bottom, sin));

                int prev = i == 0 ? sect - 1 : i - 1;

                res.TriangleIndices.Add(0);
                res.TriangleIndices.Add(2 + i * 2);
                res.TriangleIndices.Add(2 + prev * 2);

                res.TriangleIndices.Add(1);
                res.TriangleIndices.Add(3 + prev * 2);
                res.TriangleIndices.Add(3 + i * 2);

                res.TriangleIndices.Add(2 + i * 2);
                res.TriangleIndices.Add(3 + prev * 2);
                res.TriangleIndices.Add(2 + prev * 2);

                res.TriangleIndices.Add(2 + i * 2);
                res.TriangleIndices.Add(3 + i * 2);
                res.TriangleIndices.Add(3 + prev * 2);

                angl += coef;
            }
            return res;
        }

        /// <summary>
        /// Cylinders the specified radius.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="sect">The sect value.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D Cylinder(double radius, double height, int sect)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double angl = 0;
            double coef = DOUBLE_PI / sect;
            double top = 0.5 * height;
            double bottom = -0.5 * height;

            res.Positions.Add(new Point3D(0, top, 0));
            res.Positions.Add(new Point3D(0, bottom, 0));

            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, -1, 0));

            for (int i = 0; i < sect; i++)
            {
                double sin = Math.Sin(angl);
                double cos = Math.Cos(angl);

                Point3D pt1 = new Point3D(radius * cos, top, radius * sin);
                Point3D pt2 = new Point3D(radius * cos, bottom, radius * sin);

                res.Positions.Add(pt1);
                res.Positions.Add(pt2);
                res.Positions.Add(pt1);
                res.Positions.Add(pt2);

                res.Normals.Add(new Vector3D(0, 1, 0));
                res.Normals.Add(new Vector3D(0, -1, 0));
                res.Normals.Add(new Vector3D(cos, top, sin));
                res.Normals.Add(new Vector3D(cos, bottom, sin));

                int prev = i == 0 ? sect - 1 : i - 1;

                res.TriangleIndices.Add(0);
                res.TriangleIndices.Add(2 + i * 4);
                res.TriangleIndices.Add(2 + prev * 4);

                res.TriangleIndices.Add(1);
                res.TriangleIndices.Add(3 + prev * 4);
                res.TriangleIndices.Add(3 + i * 4);

                res.TriangleIndices.Add(2 + i * 4);
                res.TriangleIndices.Add(3 + prev * 4);
                res.TriangleIndices.Add(2 + prev * 4);

                res.TriangleIndices.Add(2 + i * 4);
                res.TriangleIndices.Add(3 + i * 4);
                res.TriangleIndices.Add(3 + prev * 4);

                angl += coef;
            }

            return res;
        }

        /// <summary>
        /// Spheres the specified radius.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="sect">The sect value.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D Sphere(double radius, int sect)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double angl = 0;
            double coef = DOUBLE_PI / sect;

            res.Positions.Add(new Point3D(0, radius, 0));
            res.Positions.Add(new Point3D(0, -radius, 0));

            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, -1, 0));

            for (int i = 0; i < sect; i++)
            {
                double angl2 = 0;
                double sin = Math.Sin(angl);
                double cos = Math.Cos(angl);

                for (int j = 0; j < sect; j++)
                {
                    if (i == 0)
                    {
                        int curr = i * sect + j + 2;
                        int prev = j > 0 ? curr - 1 : i * sect + sect + 1;

                        res.TriangleIndices.Add(0);
                        res.TriangleIndices.Add(curr);
                        res.TriangleIndices.Add(prev);
                    }
                    else if (i == sect)
                    {
                        int curr = i * sect + j + 2;
                        int prev = j > 0 ? curr - 1 : i * sect + sect + 1;

                        res.TriangleIndices.Add(1);
                        res.TriangleIndices.Add(curr);
                        res.TriangleIndices.Add(prev);
                    }
                    else
                    {
                        double sin2 = Math.Sin(angl2);
                        double cos2 = Math.Cos(angl2);

                        res.Positions.Add(new Point3D(radius * sin * cos2, radius * cos, radius * sin * sin2));
                        res.Normals.Add(new Vector3D(radius * sin * cos2, radius * cos, radius * sin * sin2));
                        res.TextureCoordinates.Add(new Point(cos, sin2));

                        int cc = i * sect + j + 2;
                        int pc = (i - 1) * sect + j + 2;
                        int cp = j > 0 ? cc - 1 : i * sect + sect + 1;
                        int pp = j > 0 ? (i - 1) * sect + j + 1 : (i - 1) * sect + sect + 1;

                        res.TriangleIndices.Add(cc);
                        res.TriangleIndices.Add(pc);
                        res.TriangleIndices.Add(cp);

                        res.TriangleIndices.Add(pp);
                        res.TriangleIndices.Add(cp);
                        res.TriangleIndices.Add(pc);

                        angl2 += coef;
                    }
                }

                angl += coef;
            }

            return res;
        }

        /// <summary>
        /// Cubes the specified side.
        /// </summary>
        /// <param name="side">The side value.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D Cube(double side)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double side2 = side / 2;

            Point3D p1 = new Point3D(-side2, -side2, -side2);
            Point3D p2 = new Point3D(side2, -side2, -side2);
            Point3D p3 = new Point3D(side2, side2, -side2);
            Point3D p4 = new Point3D(-side2, side2, -side2);
            Point3D p5 = new Point3D(-side2, -side2, side2);
            Point3D p6 = new Point3D(side2, -side2, side2);
            Point3D p7 = new Point3D(side2, side2, side2);
            Point3D p8 = new Point3D(-side2, side2, side2);

            AddQuad(res, p1, p4, p3, p2, new Vector3D(0, 0, -1));
            AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, 1));
            AddQuad(res, p8, p7, p3, p4, new Vector3D(0, -1, 0));
            AddQuad(res, p1, p2, p6, p5, new Vector3D(0, 1, 0));
            AddQuad(res, p1, p5, p8, p4, new Vector3D(-1, 0, 0));
            AddQuad(res, p6, p2, p3, p7, new Vector3D(1, 0, 0));

            /*      res.Positions.Add(new Point3D(-side2, side2, side2));
                        res.Positions.Add(new Point3D(side2, side2, side2));
                        res.Positions.Add(new Point3D(side2, -side2, side2));
                        res.Positions.Add(new Point3D(-side2, -side2, side2));

                        res.Positions.Add(new Point3D(-side2, side2, -side2));
                        res.Positions.Add(new Point3D(side2, side2, -side2));
                        res.Positions.Add(new Point3D(side2, -side2, -side2));
                        res.Positions.Add(new Point3D(-side2, -side2, -side2));

                        for (int i = 0; i < BOX_POINT_INDEXES.Length; i++)
                        {
                            res.TriangleIndices.Add(BOX_POINT_INDEXES[i]);
                        }*/

            return res;
        }

        /// <summary>
        /// Parallelotopes the specified side A.
        /// </summary>
        /// <param name="sideA">The side A.</param>
        /// <param name="sideB">The side B.</param>
        /// <param name="sideC">The side C.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D Parallelotope(double sideA, double sideB, double sideC)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double sideA2 = sideA / 2;
            double sideB2 = sideB / 2;
            double sideC2 = sideC / 2;

            Point3D p1 = new Point3D(-sideA2, -sideB2, -sideC2);
            Point3D p2 = new Point3D(sideA2, -sideB2, -sideC2);
            Point3D p3 = new Point3D(sideA2, sideB2, -sideC2);
            Point3D p4 = new Point3D(-sideA2, sideB2, -sideC2);
            Point3D p5 = new Point3D(-sideA2, -sideB2, sideC2);
            Point3D p6 = new Point3D(sideA2, -sideB2, sideC2);
            Point3D p7 = new Point3D(sideA2, sideB2, sideC2);
            Point3D p8 = new Point3D(-sideA2, sideB2, sideC2);

            AddTexturedQuad(res, p2, p1, p4, p3, new Vector3D(0, 0, -1));
            AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, 1));
            AddQuad(res, p8, p7, p3, p4, new Vector3D(0, -1, 0));
            AddQuad(res, p1, p2, p6, p5, new Vector3D(0, 1, 0));
            AddQuad(res, p1, p5, p8, p4, new Vector3D(-1, 0, 0));
            AddQuad(res, p6, p2, p3, p7, new Vector3D(1, 0, 0));

            return res;
        }

        /// <summary>
        /// Areas the segment
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height1">The height1.</param>
        /// <param name="height2">The height2.</param>
        /// <param name="depth">The depth.</param>
        ///  <param name="index">The index.</param>
        ///  <param name="deep">The deep.</param>
        ///  <param name="clustered">The clustered.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D AreaSegment(double width, double height1, double height2, double depth, double index, double deep, bool clustered)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            Point3D p1, p2, p3, p4, p5, p6, p7, p8;
            double sideA2 = width / 2;
            double sideD2 = depth / 2;
            if (!clustered)
            {
                double sideC21 = 0d;
                if (index == 0)
                    sideC21 = (deep / (2 * (index + 1))) + (0.1) * ((deep / 0.1) - 1);
                else
                    sideC21 = ((deep / 2) - (0.05 * 6 * index)) + (0.1) * ((deep / 0.1) - 1);
                double sideC22 = (sideC21 - 0.04);

                p1 = new Point3D(-sideA2, 0, sideC21);
                p2 = new Point3D(-sideA2, height1, sideC21);
                p3 = new Point3D(sideA2, height2, sideC21);
                p4 = new Point3D(sideA2, 0, sideC21);

                p5 = new Point3D(-sideA2, 0, sideC22);
                p6 = new Point3D(-sideA2, height1, sideC22);
                p7 = new Point3D(sideA2, height2, sideC22);
                p8 = new Point3D(sideA2, 0, sideC22);

            }
            else
            {

                p1 = new Point3D(-sideA2, 0, sideD2);
                p2 = new Point3D(-sideA2, height1, sideD2);
                p3 = new Point3D(sideA2, height2, sideD2);
                p4 = new Point3D(sideA2, 0, sideD2);

                p5 = new Point3D(-sideA2, 0, -sideD2);
                p6 = new Point3D(-sideA2, height1, -sideD2);
                p7 = new Point3D(sideA2, height2, -sideD2);
                p8 = new Point3D(sideA2, 0, -sideD2);
            }
            AddTexturedQuad(res, p4, p3, p2, p1, new Vector3D(0, 0, -1)); // Front
            AddTexturedQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, 1)); // Back
            AddTexturedQuad(res, p8, p7, p3, p4, new Vector3D(0, -1, 0)); // Top
            AddTexturedQuad(res, p1, p2, p6, p5, new Vector3D(0, 1, 0)); // Bottom
            AddTexturedQuad(res, p1, p5, p8, p4, new Vector3D(-1, 0, 0)); // Left
            AddTexturedQuad(res, p6, p2, p3, p7, new Vector3D(1, 0, 0)); // Right

            return res;
        }
        /// <summary>
        /// Areas the segment.
        /// </summary>
        /// <param name="points">The IChartDataPoint.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="transformer">The IChartTransformer.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D AreaSegmentFull(IChartDataPoint[] points, double depth, IChartTransformer transformer)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            double side = depth / 2;
            int koff = points.Length / 2 - 1;

            Point[] points1 = new Point[points.Length / 2];
            Point[] points2 = new Point[points.Length / 2];

            if (points.Length > 0)
            {
                for (int i = 0; i < points.Length / 2; i++)
                {
                    points1[i] = transformer.TransformToVisible(points[i].X, points[i].Y);
                    points1[i].Y = 1 - points1[i].Y;
                    points2[i] = transformer.TransformToVisible(points[points.Length - 1 - i].X, points[points.Length - 1 - i].Y);
                    points2[i].Y = 1 - points2[i].Y;
                }
            }

            if (points.Length > 0)
            {
                for (int i = 1; i < points1.Length; i++)
                {
                    Point3D p1 = new Point3D(points1[i - 1].X, points1[i - 1].Y, side);
                    Point3D p2 = new Point3D(points1[i].X, points1[i].Y, side);
                    Point3D p3 = new Point3D(points2[i].X, points2[i].Y, side);
                    Point3D p4 = new Point3D(points2[i - 1].X, points2[i - 1].Y, side);
                    Point3D p5 = new Point3D(points1[i - 1].X, points1[i - 1].Y, -side);
                    Point3D p6 = new Point3D(points1[i].X, points1[i].Y, -side);
                    Point3D p7 = new Point3D(points2[i].X, points2[i].Y, -side);
                    Point3D p8 = new Point3D(points2[i - 1].X, points2[i - 1].Y, -side);

                    AddTexturedQuad(res, p4, p1, p2, p3, new Vector3D(0, 0, -1));
                    AddTexturedQuad(res, p8, p5, p6, p7, new Vector3D(0, 0, 1));
                    AddTexturedQuad(res, p4, p8, p5, p1, new Vector3D(-1, 0, 0));
                    AddTexturedQuad(res, p1, p5, p6, p2, new Vector3D(0, -1, 0));
                    AddTexturedQuad(res, p2, p6, p7, p3, new Vector3D(1, 0, 0));
                    AddTexturedQuad(res, p3, p7, p8, p4, new Vector3D(0, 1, 0));
                }
            }

            return res;
        }
        /// <summary>
        /// Columns the parallelotope.
        /// </summary>
        /// <param name="sideA">The side A.</param>
        /// <param name="sideB">The side B.</param>
        /// <param name="sideC">The side C.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D ColumnParallelotope(double sideA, double sideB, double sideC)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double sideA2 = sideA/2;
            double sideB2 = sideB/2;
            double sideC2 = sideC/2;

            Point3D p1 = new Point3D(-sideA2, -sideB2, -sideC2);
            Point3D p2 = new Point3D(sideA2, -sideB2, -sideC2);
            Point3D p3 = new Point3D(sideA2, sideB2, -sideC2);
            Point3D p4 = new Point3D(-sideA2, sideB2, -sideC2);
            Point3D p5 = new Point3D(-sideA2, -sideB2, sideC2);
            Point3D p6 = new Point3D(sideA2, -sideB2, sideC2);
            Point3D p7 = new Point3D(sideA2, sideB2, sideC2);
            Point3D p8 = new Point3D(-sideA2, sideB2, sideC2);

            AddTexturedQuad(res, p3, p2, p1, p4, new Vector3D(0, 0, -1));
            AddTexturedQuad(res, p8, p5, p6, p7, new Vector3D(0, 0, 1));
            AddTexturedQuad(res, p4, p1, p5, p8, new Vector3D(-1, 0, 0));
            AddTexturedQuad(res, p7, p6, p2, p3, new Vector3D(1, 0, 0));
            AddTexturedQuad(res, p4, p8, p7, p3, new Vector3D(0, -1, 0));
            AddTexturedQuad(res, p5, p1, p2, p6, new Vector3D(0, 1, 0));

            
            return res;
        }

        internal static MeshGeometry3D ColumnParallelotope(double sideA, double sideB, double sideC, double index)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double sideA2 = sideA / 2;
            double sideB2 = sideB / 2;
            double sideC21 = 0d;
            if (index == 0)
                sideC21 = (sideC / (2 * (index + 1))) + (0.1)*((sideC/0.1)-1);
            else
                sideC21 = ((sideC / 2) - (0.05 * 6 * index)) + (0.1) * ((sideC / 0.1) - 1); 
            double sideC22 = (sideC21 - 0.1);
            Point3D p1, p2, p3, p4, p5, p6, p7, p8;
          
                p1 = new Point3D(-sideA2, -sideB2, sideC22);
                p2 = new Point3D(sideA2, -sideB2, sideC22);
                p3 = new Point3D(sideA2, sideB2, sideC22);
                p4 = new Point3D(-sideA2, sideB2, sideC22);
                p5 = new Point3D(-sideA2, -sideB2, sideC21);
                p6 = new Point3D(sideA2, -sideB2, sideC21);
                p7 = new Point3D(sideA2, sideB2, sideC21);
                p8 = new Point3D(-sideA2, sideB2, sideC21);
           
            AddTexturedQuad(res, p3, p2, p1, p4, new Vector3D(0, 0, -1));
            AddTexturedQuad(res, p8, p5, p6, p7, new Vector3D(0, 0, 1));
            AddTexturedQuad(res, p4, p1, p5, p8, new Vector3D(-1, 0, 0));
            AddTexturedQuad(res, p7, p6, p2, p3, new Vector3D(1, 0, 0));
            AddTexturedQuad(res, p4, p8, p7, p3, new Vector3D(0, -1, 0));
            AddTexturedQuad(res, p5, p1, p2, p6, new Vector3D(0, 1, 0));


            return res;
        }
        /// <summary>
        /// Pyramids the specified side.
        /// </summary>
        /// <param name="side">The side value.</param>
        /// <param name="height">The height.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D Pyramid(double side, double height)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double side2 = side / 2;
            double height2 = height / 2;

            Point3D p1 = new Point3D(-side2, -height2, -side2);
            Point3D p2 = new Point3D(side2, -height2, -side2);
            Point3D p3 = new Point3D(side2, -height2, side2);
            Point3D p4 = new Point3D(-side2, -height2, side2);
            Point3D p5 = new Point3D(0, height2, 0);

            AddQuad(res, p1, p2, p3, p4, new Vector3D(0, -1, 0));
            AddTri(res, p2, p1, p5, Vector3D.CrossProduct((Vector3D)p5 - (Vector3D)p1, (Vector3D)p5 - (Vector3D)p2));
            AddTri(res, p3, p2, p5, Vector3D.CrossProduct((Vector3D)p5 - (Vector3D)p2, (Vector3D)p5 - (Vector3D)p3));
            AddTri(res, p4, p3, p5, Vector3D.CrossProduct((Vector3D)p5 - (Vector3D)p3, (Vector3D)p5 - (Vector3D)p4));
            AddTri(res, p1, p4, p5, Vector3D.CrossProduct((Vector3D)p5 - (Vector3D)p4, (Vector3D)p5 - (Vector3D)p1));

            return res;
        }

        /// <summary>
        /// Pyramids the specified bottom.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        /// <param name="top">The top value.</param>
        /// <param name="height">The height.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D Pyramid(double bottom, double top, double height)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double bottom2 = bottom / 2;
            double top2 = top / 2;

            res.Positions.Add(new Point3D(-top2, height, top2));
            res.Positions.Add(new Point3D(top2, height, top2));
            res.Positions.Add(new Point3D(bottom2, 0, bottom2));
            res.Positions.Add(new Point3D(-bottom2, 0, bottom2));

            res.Positions.Add(new Point3D(-top2, height, -top2));
            res.Positions.Add(new Point3D(top2, height, -top2));
            res.Positions.Add(new Point3D(bottom2, 0, -bottom2));
            res.Positions.Add(new Point3D(-bottom2, 0, -bottom2));

            for (int i = 0; i < box_POINT_INDEXES.Length; i++)
            {
                res.TriangleIndices.Add(box_POINT_INDEXES[i]);
            }

            return res;
        }

        /// <summary>
        /// Planes the Y.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D PlaneX(double width, double height)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double w2 = 0.5 * width;
            double h2 = 0.5 * height;

            res.Positions.Add(new Point3D(0, -w2, -h2));
            res.Positions.Add(new Point3D(0, w2, -h2));
            res.Positions.Add(new Point3D(0, w2, h2));
            res.Positions.Add(new Point3D(0, -w2, h2));

            res.TextureCoordinates.Add(new Point(1, 1));
            res.TextureCoordinates.Add(new Point(1, 0));
            res.TextureCoordinates.Add(new Point(0, 0));
            res.TextureCoordinates.Add(new Point(0, 1));

            res.TriangleIndices.Add(0);
            res.TriangleIndices.Add(1);
            res.TriangleIndices.Add(2);
            res.TriangleIndices.Add(0);
            res.TriangleIndices.Add(2);
            res.TriangleIndices.Add(3);

            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, 1, 0));

            return res;
        }

        /// <summary>
        /// Pies the segment.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="endAngle">The end angle.</param>
        ///  <param name="IsExploded">The IsExploded.</param>
        ///  <param name="isExplodedVisible">The isExplodedVisible.</param>
        /// <returns>The MeshGeometry3D res</returns>
       
        internal static MeshGeometry3D PieSegment(Point center, double radius, double startAngle, double endAngle,bool IsExploded,bool isExplodedVisible)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            Point startPoint;
            Point endPoint;
            double deltaAngle = endAngle - startAngle;
            double minSect = Math.PI * 2 / 5000;
            Point center1 = new Point();
            center1 = center;
           //double explodedDistance = IsExploded ? 0.15 : 0;

            if (deltaAngle >= minSect)
            {
                int sectCount = (int)(deltaAngle / minSect);
                minSect = deltaAngle / sectCount;
                if (IsExploded && isExplodedVisible)
                {
                    if (sectCount % 2 == 0)
                    {
                        double explodedStartAng = startAngle + (sectCount / 2) * minSect;
                        double explodedCenterX = 0.10 * Math.Cos((endAngle + explodedStartAng) / 2 - deltaAngle / 4) - 0 * Math.Sin((endAngle + explodedStartAng) / 2);
                        double explodedCenterY = 0.10 * Math.Sin((endAngle + explodedStartAng) / 2 - deltaAngle / 4) + 0 * Math.Cos((endAngle + explodedStartAng) / 2);
                        center1 = new Point(explodedCenterX, explodedCenterY);
                    }
                    else
                    {
                        double explodedStartAng1 = startAngle + ((sectCount - 1) / 2) * minSect;
                        double explodedStartAng2 = startAngle + ((sectCount + 1) / 2) * minSect;
                        double explodedStartAng = (explodedStartAng1 + explodedStartAng2) / 2;
                        double explodedCenterX = 0.10 * Math.Cos((endAngle + explodedStartAng) / 2 - deltaAngle / 4) - 0 * Math.Sin((endAngle + explodedStartAng) / 2);
                        double explodedCenterY = 0.10 * Math.Sin((endAngle + explodedStartAng) / 2 - deltaAngle / 4) + 0 * Math.Cos((endAngle + explodedStartAng) / 2);

                        center1 = new Point(explodedCenterX, explodedCenterY);
                    }
                }
                for (int i = 0; i <= sectCount && startAngle <= endAngle; i++)
                {
                    if (IsExploded && isExplodedVisible)
                    {
                        double stop1x = 0.15 * Math.Cos((endAngle + startAngle) / 2 - deltaAngle / 4) - 0 * Math.Sin((endAngle + startAngle) / 2);
                        double stop1y = 0.15 * Math.Sin((endAngle + startAngle) / 2 - deltaAngle / 4) + 0 * Math.Cos((endAngle + startAngle) / 2);
                       Point  newcenter = new Point(stop1x, stop1y);
                        startPoint = newcenter + (radius) * new Vector(Math.Cos(startAngle), Math.Sin(startAngle));
                        endPoint = newcenter + (radius) * new Vector(Math.Cos(endAngle), Math.Sin(endAngle));
                    }
                    else
                    {
                        startPoint = center + radius * new Vector(Math.Cos(startAngle), Math.Sin(startAngle));
                        endPoint = center + radius * new Vector(Math.Cos(startAngle + minSect), Math.Sin(startAngle + minSect));
                    }
                    Point3D p1 = new Point3D(center1.X, center1.Y, 0d);
                    Point3D p2 = new Point3D(startPoint.X , startPoint.Y, 0d);
                    Point3D p3 = new Point3D(endPoint.X , endPoint.Y, 0d);
                    Point3D p4 = new Point3D(center1.X, center1.Y, 0.2d);
                    Point3D p5 = new Point3D(startPoint.X , startPoint.Y, 0.2d);
                    Point3D p6 = new Point3D(endPoint.X , endPoint.Y, 0.2d);

                    AddTri(res, p3, p2, p1, new Vector3D(0, 0, -1));
                    AddQuad(res, p2, p3, p6, p5, new Vector3D(Math.Cos(startAngle + minSect / 2), Math.Sin(startAngle + minSect / 2), 0));
                    AddTri(res, p4, p5, p6, new Vector3D(0, 0, 1));

                    AddQuad(res, p3, p1, p4, p6, new Vector3D(0, 0, 1));
                    AddQuad(res, p5, p4, p1, p2, new Vector3D(0, 0, 1));

                    startAngle += minSect;
                }
            }

            return res;
        }

        /// <summary>
        /// Doughnuts the segment.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius1">The radius1.</param>
        /// <param name="radius2">The radius2.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="endAngle">The end angle.</param>
        /// <param name="IsExploded"></param>
        /// <param name="explodedRadius"></param>
        /// <param name="isExplodedVisible"></param>
        /// <returns>The MeshGeometry3D res</returns>
        /// <remarks></remarks>
        internal static MeshGeometry3D DoughnutSegment(Point center, double radius1, double radius2, double startAngle, double endAngle,bool IsExploded,double explodedRadius,bool isExplodedVisible)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            //if (radius1 != radius2)
            //{
                Point startPointTop;
                Point endPointTop;
                Point startPointBottom;
                Point endPointBottom;
                double radiusTop = (radius1 > radius2) ? radius1 : radius2;
                double radiusBottom = (radius1 < radius2) ? radius1 : radius2;
                double explodedDistance = IsExploded ? 0.15 : 0;
                explodedRadius = explodedRadius / 100;

                double deltaAngle = endAngle - startAngle;
                double minSect = Math.PI * 2 / 500;

                double startAngle1 = startAngle;
                double endAngle1 = endAngle;
                if (deltaAngle >= minSect)
                {
                    int sectCount = (int)(deltaAngle / minSect);
                    minSect = deltaAngle / sectCount;
            
                    for (int i = 0; i <= sectCount && startAngle <= endAngle; i++)
                    {
                        if (IsExploded && isExplodedVisible)
                        {
                            double stop1x = 0.15 * Math.Cos((endAngle + startAngle) / 2 - deltaAngle / 4) - 0 * Math.Sin((endAngle + startAngle) / 2);
                            double stop1y = 0.15 * Math.Sin((endAngle + startAngle) / 2 - deltaAngle / 4) + 0 * Math.Cos((endAngle + startAngle) / 2);
                            center = new Point(stop1x, stop1y);

                            double delta = Math.Sqrt(stop1x*stop1x + stop1y*stop1y);
                            startPointTop = center + (radiusTop-0.15/2) * new Vector(Math.Cos(startAngle), Math.Sin(startAngle));
                            endPointTop = center + (radiusTop-0.15/2) * new Vector(Math.Cos(endAngle), Math.Sin(endAngle));

                            startPointBottom = center + (radiusBottom-0.15/2) * new Vector(Math.Cos(startAngle), Math.Sin(startAngle));
                            endPointBottom = center + (radiusBottom-0.15/2) * new Vector(Math.Cos(endAngle), Math.Sin(endAngle));
                        }
                        else
                        {
                            startPointTop = center + radiusTop * new Vector(Math.Cos(startAngle), Math.Sin(startAngle));
                            endPointTop = center + radiusTop * new Vector(Math.Cos(startAngle + minSect), Math.Sin(startAngle + minSect));

                            startPointBottom = center + radiusBottom * new Vector(Math.Cos(startAngle), Math.Sin(startAngle));
                            endPointBottom = center + radiusBottom * new Vector(Math.Cos(startAngle + minSect), Math.Sin(startAngle + minSect));
                        }

                        Point3D p1 = new Point3D(startPointTop.X, startPointTop.Y, 0d);
                        Point3D p2 = new Point3D(endPointTop.X, endPointTop.Y, 0d);
                        Point3D p3 = new Point3D(endPointBottom.X, endPointBottom.Y, 0d);
                        Point3D p4 = new Point3D(startPointBottom.X, startPointBottom.Y, 0d);
                        Point3D p5 = new Point3D(startPointTop.X, startPointTop.Y, 0.2d);
                        Point3D p6 = new Point3D(endPointTop.X, endPointTop.Y, 0.2d);
                        Point3D p7 = new Point3D(endPointBottom.X, endPointBottom.Y, 0.2d);
                        Point3D p8 = new Point3D(startPointBottom.X, startPointBottom.Y, 0.2d);


                        AddQuad(res, p4, p3, p2, p1, new Vector3D(0, 0, -1));
                        AddQuad(res, p8, p7, p3, p4, new Vector3D(Math.Cos(startAngle + minSect / 2), Math.Sin(startAngle + minSect / 2), -1));
                        AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, 1));
                        AddQuad(res, p1, p2, p6, p5, new Vector3D(Math.Cos(startAngle + minSect / 2), Math.Sin(startAngle + minSect / 2), 0));

                        AddQuad(res, p5, p1, p4, p8, new Vector3D(0, 0, 1));
                        AddQuad(res, p8, p4, p1, p5, new Vector3D(0, 0, 1));

                        startAngle += minSect;
                    }
                //}
            }

            return res;
        }

        /// <summary>
        /// Planes the Y.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D PlaneY(double width, double height)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double w2 = 0.5 * width;
            double h2 = 0.5 * height;

            res.Positions.Add(new Point3D(-w2, 0, -h2));
            res.Positions.Add(new Point3D(w2, 0, -h2));
            res.Positions.Add(new Point3D(w2, 0, h2));
            res.Positions.Add(new Point3D(-w2, 0, h2));

            res.TextureCoordinates.Add(new Point(0, 1));
            res.TextureCoordinates.Add(new Point(1, 1));
            res.TextureCoordinates.Add(new Point(1, 0));
            res.TextureCoordinates.Add(new Point(0, 0));

            res.TriangleIndices.Add(0);
            res.TriangleIndices.Add(1);
            res.TriangleIndices.Add(2);
            res.TriangleIndices.Add(0);
            res.TriangleIndices.Add(2);
            res.TriangleIndices.Add(3);

            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, 1, 0));

            return res;
        }

        /// <summary>
        /// Planes the Z.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D PlaneZ(double width, double height)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            
            double w2 = 0.5 * width;
            double h2 = 0.5 * height;

            res.Positions.Add(new Point3D(-w2, -h2, 0));
            res.Positions.Add(new Point3D(w2, -h2, 0));
            res.Positions.Add(new Point3D(w2, h2, 0));
            res.Positions.Add(new Point3D(-w2, h2, 0));

            res.TextureCoordinates.Add(new Point(0, 1));
            res.TextureCoordinates.Add(new Point(1, 1));
            res.TextureCoordinates.Add(new Point(1, 0));
            res.TextureCoordinates.Add(new Point(0, 0));

            res.TriangleIndices.Add(0);
            res.TriangleIndices.Add(1);
            res.TriangleIndices.Add(2);
            res.TriangleIndices.Add(0);
            res.TriangleIndices.Add(2);
            res.TriangleIndices.Add(3);

            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, 1, 0));

            return res;
        }

        /// <summary>
        /// Sectors the specified radius.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="start">The start.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="sect">The sect value.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D Sector(double radius, double height, double start, double angle, int sect)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            double angl = start;
            double coef = DOUBLE_PI / sect;
            double top = 0.5 * height;
            double bottom = -0.5 * height;
            int count = (int)(angle / coef);

            res.Positions.Add(new Point3D(0, top, 0));
            res.Positions.Add(new Point3D(0, bottom, 0));

            res.Normals.Add(new Vector3D(0, 1, 0));
            res.Normals.Add(new Vector3D(0, -1, 0));

            for (int i = 0, c = count < 2 ? 2 : count; i < c; i++)
            {
                double sin = Math.Sin(angl);
                double cos = Math.Cos(angl);

                Point3D pt1 = new Point3D(radius * cos, top, radius * sin);
                Point3D pt2 = new Point3D(radius * cos, bottom, radius * sin);

                res.Positions.Add(pt1);
                res.Positions.Add(pt2);
                res.Positions.Add(pt1);
                res.Positions.Add(pt2);

                res.Normals.Add(new Vector3D(0, 1, 0));
                res.Normals.Add(new Vector3D(0, -1, 0));
                res.Normals.Add(new Vector3D(cos, top, sin));
                res.Normals.Add(new Vector3D(cos, bottom, sin));

                int prev = i == 0 ? c - 1 : i - 1;

                res.TriangleIndices.Add(0);
                res.TriangleIndices.Add(2 + i * 4);
                res.TriangleIndices.Add(2 + prev * 4);

                res.TriangleIndices.Add(1);
                res.TriangleIndices.Add(3 + prev * 4);
                res.TriangleIndices.Add(3 + i * 4);

                res.TriangleIndices.Add(2 + i * 4);
                res.TriangleIndices.Add(3 + prev * 4);
                res.TriangleIndices.Add(2 + prev * 4);

                res.TriangleIndices.Add(2 + i * 4);
                res.TriangleIndices.Add(3 + i * 4);
                res.TriangleIndices.Add(3 + prev * 4);

                angl += coef;
            }

            return res;
        }

        /// <summary>
        /// Lines the bar.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="point3">The point3.</param>
        /// <param name="point4">The point4.</param>
        /// <param name="index"></param>
        /// <param name="isClustered"></param>
        /// <param name="deep"></param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D LineBar(Vector3D point1, Vector3D point2, Vector3D point3, Vector3D point4, int index, bool isClustered, double deep)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            Point3D p1, p2, p3, p4, p5, p6, p7, p8 = new Point3D();
            double depth = -0.05;
            if (!isClustered)
            {
                double sideC21 = 0d;
                if (index == 0)
                    sideC21 = (deep / (2 * (index + 1))) + (0.1) * ((deep / 0.1) - 1);
                else
                    sideC21 = ((deep / 2) - (0.05 * 6 * index)) + (0.1) * ((deep / 0.1) - 1);
                double sideC22 = (sideC21 - 0.04);
                p1 = new Point3D(point1.X, point1.Y, sideC21);
                p2 = new Point3D(point2.X, point2.Y, sideC21);
                p3 = new Point3D(point3.X, point3.Y, sideC21);
                p4 = new Point3D(point4.X, point4.Y, sideC21);
                p5 = new Point3D(point1.X, point1.Y, sideC22);
                p6 = new Point3D(point2.X, point2.Y, sideC22);
                p7 = new Point3D(point3.X, point3.Y, sideC22);
                p8 = new Point3D(point4.X, point4.Y, sideC22);
            }
            else
            {
                p1 = new Point3D(point1.X, point1.Y, point1.Z);
                p2 = new Point3D(point2.X, point2.Y, point2.Z);
                p3 = new Point3D(point3.X, point3.Y, point3.Z);
                p4 = new Point3D(point4.X, point4.Y, point4.Z);
                p5 = new Point3D(point1.X, point1.Y, depth + point1.Z);
                p6 = new Point3D(point2.X, point2.Y, depth + point2.Z);
                p7 = new Point3D(point3.X, point3.Y, depth + point3.Z);
                p8 = new Point3D(point4.X, point4.Y, depth + point4.Z);
            }
            AddQuad(res, p2, p1, p4, p3, new Vector3D(0, 0, -1));
            AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, 1));
            AddQuad(res, p8, p7, p3, p4, new Vector3D(0, -1, 0));
            AddQuad(res, p1, p2, p6, p5, new Vector3D(0, 1, 0));
            AddQuad(res, p1, p5, p8, p4, new Vector3D(-1, 0, 0));
            AddQuad(res, p6, p2, p3, p7, new Vector3D(1, 0, 0));

            return res;
        }

        /// <summary>
        /// Steps the line.
        /// </summary>
        /// <param name="pointStart">The point start.</param>
        /// <param name="pointEnd">The point end.</param>
        /// <param name="pointStep">The point step.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D StepLine(Point3D pointStart, Point3D pointEnd, Point3D pointStep)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            double thickness = 0.06;
            double shift = 0.01d;
            double shiftX = 0.01d;

            Point3D p1;
            Point3D p2;
            Point3D p3;
            Point3D p4;
            Point3D p5;
            Point3D p6;
            Point3D p7;
            Point3D p8;

            if (pointStart.Y > pointStep.Y)
            {
                p1 = new Point3D(pointStep.X - shiftX, pointStep.Y - shift, pointStep.Z);
                p2 = new Point3D(pointStart.X - shiftX, pointStart.Y, pointStart.Z);
                p3 = new Point3D(pointStart.X + shiftX, pointStart.Y, pointStart.Z);
                p4 = new Point3D(pointStep.X + shiftX, pointStep.Y - shift, pointStep.Z);
                p5 = new Point3D(pointStep.X - shiftX, pointStep.Y - shift, -thickness + pointStep.Z);
                p6 = new Point3D(pointStart.X - shiftX, pointStart.Y, -thickness + pointStart.Z);
                p7 = new Point3D(pointStart.X + shiftX, pointStart.Y, -thickness + pointStart.Z);
                p8 = new Point3D(pointStep.X + shiftX, pointStep.Y - shift, -thickness + pointStep.Z);
            }
            else
            {
                p1 = new Point3D(pointStart.X - shiftX, pointStart.Y, pointStart.Z);
                p2 = new Point3D(pointStep.X - shiftX, pointStep.Y + shift, pointStep.Z);
                p3 = new Point3D(pointStep.X + shiftX, pointStep.Y + shift, pointStep.Z);
                p4 = new Point3D(pointStart.X + shiftX, pointStart.Y, pointStart.Z);
                p5 = new Point3D(pointStart.X - shiftX, pointStart.Y, -thickness + pointStart.Z);
                p6 = new Point3D(pointStep.X - shiftX, pointStep.Y + shift, -thickness + pointStep.Z);
                p7 = new Point3D(pointStep.X + shiftX, pointStep.Y + shift, -thickness + pointStep.Z);
                p8 = new Point3D(pointStart.X + shiftX, pointStart.Y, -thickness + pointStart.Z);
            }

            AddQuad(res, p2, p1, p4, p3, new Vector3D(0, 0, 1));
            AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, -1));
            AddQuad(res, p8, p7, p3, p4, new Vector3D(1, 0, 0));
            AddQuad(res, p1, p2, p6, p5, new Vector3D(-1, 0, 0));
            AddQuad(res, p1, p5, p8, p4, new Vector3D(0, -1, 0));
            AddQuad(res, p6, p2, p3, p7, new Vector3D(0, 1, 0));

            double shiftY = 0.01d;
            shift = 0.01d;

            p1 = new Point3D(pointEnd.X + shift, pointEnd.Y + shiftY, pointEnd.Z);
            p2 = new Point3D(pointEnd.X + shift, pointEnd.Y - shiftY, pointEnd.Z);
            p3 = new Point3D(pointStep.X + shift, pointStep.Y - shiftY, pointStep.Z);
            p4 = new Point3D(pointStep.X + shift, pointStep.Y + shiftY, pointStep.Z);
            p5 = new Point3D(pointEnd.X + shift, pointEnd.Y + shiftY, -thickness + pointEnd.Z);
            p6 = new Point3D(pointEnd.X + shift, pointEnd.Y - shiftY, -thickness + pointEnd.Z);
            p7 = new Point3D(pointStep.X + shift, pointStep.Y - shiftY, -thickness + pointStep.Z);
            p8 = new Point3D(pointStep.X + shift, pointStep.Y + shiftY, -thickness + pointStep.Z);

            AddQuad(res, p2, p1, p4, p3, new Vector3D(0, 0, 1));
            AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, -1));
            AddQuad(res, p8, p7, p3, p4, new Vector3D(-1, 0, 0));
            AddQuad(res, p1, p2, p6, p5, new Vector3D(1, 0, 0));
            AddQuad(res, p1, p5, p8, p4, new Vector3D(0, 1, 0));
            AddQuad(res, p6, p2, p3, p7, new Vector3D(0, -1, 0));

            return res;
        }

        /// <summary>
        /// Fasts the line bar.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="point3">The point3.</param>
        /// <param name="point4">The point4.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D FastLineBar(Vector3D point1, Vector3D point2, Vector3D point3, Vector3D point4)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            double depth = -0.5;
            Point3D p1 = new Point3D(point1.X, point1.Y, point1.Z);
            Point3D p2 = new Point3D(point2.X, point2.Y, point2.Z);
            Point3D p3 = new Point3D(point3.X, point3.Y, point3.Z);
            Point3D p4 = new Point3D(point4.X, point4.Y, point4.Z);
            Point3D p5 = new Point3D(point1.X, point1.Y, depth + point1.Z);
            Point3D p6 = new Point3D(point2.X, point2.Y, depth + point2.Z);
            Point3D p7 = new Point3D(point3.X, point3.Y, depth + point3.Z);
            Point3D p8 = new Point3D(point4.X, point4.Y, depth + point4.Z);

            AddQuad(res, p2, p1, p4, p3, new Vector3D(0, 0, -1));
            AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, 1));
            ////AddQuad(res, p8, p7, p3, p4, new Vector3D(0, -1, 0));
            ////AddQuad(res, p1, p2, p6, p5, new Vector3D(0, 1, 0));
            AddQuad(res, p1, p5, p8, p4, new Vector3D(-1, 0, 0));
            AddQuad(res, p6, p2, p3, p7, new Vector3D(1, 0, 0));

            return res;
        }

        /// <summary>
        /// Splines the area.
        /// </summary>
        /// <param name="splinePoints">The spline points.</param>
        /// <param name="transformer">The IChartTransformer.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D SplineArea(IChartDataPoint[] splinePoints, IChartTransformer transformer)
        {
            MeshGeometry3D res = new MeshGeometry3D();

            Point point = transformer.TransformToVisible(splinePoints[0].X, splinePoints[0].Y);
            point.Y = 1 - point.Y;
            AddQuad(res, new Point3D(point.X, 0, 0), new Point3D(point.X, point.Y, 0), new Point3D(point.X, point.Y, -0.02), new Point3D(point.X, 0, -0.02), new Vector3D(-1, 0, 0));

            for (int i = 0, len = splinePoints.Length - 1; i < len; i += 3)
            {
                Point point1 = transformer.TransformToVisible(splinePoints[i].X, splinePoints[i].Y);
                point1.Y = 1 - point1.Y;
                Point point2 = transformer.TransformToVisible(splinePoints[i + 1].X, splinePoints[i + 1].Y);
                point2.Y = 1 - point2.Y;
                Point point3 = transformer.TransformToVisible(splinePoints[i + 2].X, splinePoints[i + 2].Y);
                point3.Y = 1 - point3.Y;
                Point point4 = transformer.TransformToVisible(splinePoints[i + 3].X, splinePoints[i + 3].Y);
                point4.Y = 1 - point4.Y;
                Point[] points = InterpolateBezier(point1, point2, point3, point4, 10);

                for (int j = 0; j < points.Length - 1; j++)
                {
                    Point3D p1 = new Point3D(points[j].X, 0, 0);
                    Point3D p2 = new Point3D(points[j].X, points[j].Y, 0);
                    Point3D p3 = new Point3D(points[j + 1].X, points[j + 1].Y, 0);
                    Point3D p4 = new Point3D(points[j + 1].X, 0, 0);
                    Point3D p5 = new Point3D(points[j].X, 0, -0.02);
                    Point3D p6 = new Point3D(points[j].X, points[j].Y, -0.02);
                    Point3D p7 = new Point3D(points[j + 1].X, points[j + 1].Y, -0.02);
                    Point3D p8 = new Point3D(points[j + 1].X, 0, -0.02);

                    AddQuad(res, p2, p1, p4, p3, new Vector3D(0, 0, -1));
                    AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, 1));
                    AddQuad(res, p1, p5, p8, p4, new Vector3D(-1, 0, 0));
                    AddQuad(res, p6, p2, p3, p7, new Vector3D(1, 0, 0));
                }
            }

            int lastPoint = splinePoints.Length - 1;
            point = transformer.TransformToVisible(splinePoints[lastPoint].X, splinePoints[lastPoint].Y);
            point.Y = 1 - point.Y;

            AddQuad(res, new Point3D(point.X, 0, -0.02), new Point3D(point.X, point.Y, -0.02), new Point3D(point.X, point.Y, 0), new Point3D(point.X, 0, 0), new Vector3D(1, 0, 0));
            return res;
        }

        /// <summary>
        /// Draws splines segment.
        /// </summary>
        /// <param name="point1">The poin1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="point3">The point3.</param>
        /// <param name="point4">The point4.</param>
        /// <returns>The MeshGeometry3D res</returns>
        internal static MeshGeometry3D SplineSegment(Point3D point1, Point3D point2, Point3D point3, Point3D point4)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            Point3D[] points = InterpolateBezier(point1, point2, point3, point4, 20);
            Point3D pointShift1;
            Point3D pointShift2;
            double depth = 0.05d;
            double y;
            double yEx = points[0].Y - (0.005d + 0.005d * (Math.Abs(points[1].Y - points[0].Y) / Math.Abs(points[1].X - points[0].X)));

            for (int j = 0; j < points.Length - 1; j++)
            {
                y = points[j + 1].Y - (0.005d + 0.005d * (Math.Abs(points[j + 1].Y - points[j].Y) / Math.Abs(points[j + 1].X - points[j].X)));

                pointShift1 = new Point3D(points[j].X, yEx, points[j].Z);
                pointShift2 = new Point3D(points[j + 1].X, y, points[j + 1].Z);
                Point3D p1 = new Point3D(pointShift1.X, pointShift1.Y, pointShift1.Z);
                Point3D p2 = new Point3D(points[j].X, points[j].Y, points[j].Z);
                Point3D p3 = new Point3D(points[j + 1].X, points[j + 1].Y, points[j].Z);
                Point3D p4 = new Point3D(pointShift2.X, pointShift2.Y, pointShift2.Z);
                Point3D p5 = new Point3D(pointShift1.X, pointShift1.Y, pointShift1.Z + depth);
                Point3D p6 = new Point3D(points[j].X, points[j].Y, points[j].Z + depth);
                Point3D p7 = new Point3D(points[j + 1].X, points[j + 1].Y, points[j + 1].Z + depth);
                Point3D p8 = new Point3D(pointShift2.X, pointShift2.Y, pointShift2.Z + depth);
                AddQuad(res, p2, p1, p4, p3, new Vector3D(0, 0, -1));
                AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, 1));
                AddQuad(res, p1, p5, p8, p4, new Vector3D(-1, 0, 0));
                AddQuad(res, p6, p2, p3, p7, new Vector3D(1, 0, 0));

                yEx = y;
            }

            return res;
        }

        internal static MeshGeometry3D SurfaceAreaSegment(Point3D point1, Point3D point2, Point3D point3, Point3D point4, Point3D point5, Point3D point6, Point3D point7, Point3D point8, double deep)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            Point3D[] points1 = InterpolateBezier(point1, point2, point3, point4, 20);
            Point3D[] points2 = InterpolateBezier(point5, point6, point7, point8, 20);
            Point3D pointShift1;
            Point3D pointShift2;
            Point3D pointShift3;
            Point3D pointShift4;
            double depth = -deep;
            double y1,y2;
            double yEx1 = points1[0].Y - (0.005d + 0.005d * (Math.Abs(points1[1].Y - points1[0].Y) / Math.Abs(points1[1].X - points1[0].X)));
            double yEx2 = points2[0].Y - (0.005d + 0.005d * (Math.Abs(points2[1].Y - points2[0].Y) / Math.Abs(points2[1].X - points2[0].X)));

            for (int j = 0; j < points1.Length - 1 && j < points2.Length -1; j++)
            {
                y1 = points1[j + 1].Y - (0.005d + 0.005d * (Math.Abs(points1[j + 1].Y - points1[j].Y) / Math.Abs(points1[j + 1].X - points1[j].X)));
                y2 = points2[j + 1].Y - (0.005d + 0.005d * (Math.Abs(points2[j + 1].Y - points2[j].Y) / Math.Abs(points2[j + 1].X - points2[j].X)));

                pointShift1 = new Point3D(points1[j].X, yEx1, points1[j].Z);
                pointShift2 = new Point3D(points1[j + 1].X, y1, points1[j + 1].Z);

                pointShift3 = new Point3D(points2[j].X, yEx2, points2[j].Z);
                pointShift4 = new Point3D(points2[j + 1].X, y2, points2[j + 1].Z);

                Point3D p1 = new Point3D(pointShift1.X, pointShift1.Y, pointShift1.Z);
                Point3D p2 = new Point3D(points1[j].X, points1[j].Y, points1[j].Z);
                Point3D p3 = new Point3D(points1[j + 1].X, points1[j + 1].Y, points1[j].Z);
                Point3D p4 = new Point3D(pointShift2.X, pointShift2.Y, pointShift2.Z);

                Point3D p5 = new Point3D(pointShift3.X, pointShift3.Y, pointShift3.Z);
                Point3D p6 = new Point3D(points2[j].X, points2[j].Y, pointShift1.Z);
                Point3D p7 = new Point3D(points2[j + 1].X, points2[j + 1].Y, points2[j+1].Z);
                Point3D p8 = new Point3D(pointShift4.X, pointShift4.Y, pointShift3.Z);

                AddQuad(res, p2, p1, p4, p3, new Vector3D(0, 0, -1));
                AddQuad(res, p5, p6, p7, p8, new Vector3D(0, 0, 1));
                AddQuad(res, p1, p5, p8, p4, new Vector3D(-1, 0, 0));
                AddQuad(res, p6, p2, p3, p7, new Vector3D(1, 0, 0));

                yEx1 = y1;
                yEx2 = y2;
            }

            return res;
        }

        internal static MeshGeometry3D SurfaceSegment(Point3D point1, Point3D point2, Point3D point3, Point3D point4)
        {
            MeshGeometry3D res = new MeshGeometry3D();
            double depth = 0.07d;
            Point3D p1 = new Point3D(point1.X, point1.Y, point1.Z);
            Point3D p2 = new Point3D(point2.X, point2.Y, point2.Z);
            Point3D p3 = new Point3D(point3.X, point3.Y, depth + point3.Z);
            Point3D p4 = new Point3D(point4.X, point4.Y, depth + point4.Z);

            AddTexturedQuad(res, p1, p2, p3, p4, new Vector3D(0, -1, 0));
            AddTexturedQuad(res, p4, p3, p2, p1, new Vector3D(0, 0, -1));
            return res;
        }
        /// <summary>
        /// Glyphes the specified glyph.
        /// </summary>
        /// <param name="glyph">The glyph.</param>
        /// <returns>Returns null</returns>
        internal static MeshGeometry3D Glyph(GlyphTypeface glyph)
        {
            return null;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Interpolates the bezier.
        /// </summary>
        /// <param name="p1">The p1 value.</param>
        /// <param name="p2">The p2 value.</param>
        /// <param name="p3">The p3 value.</param>
        /// <param name="p4">The p4 value.</param>
        /// <param name="count">The count.</param>
        /// <returns>The points</returns>
        internal static Point3D[] InterpolateBezier(Point3D p1, Point3D p2, Point3D p3, Point3D p4, int count)
        {
            Point3D[] pts = new Point3D[count];

            double cx = 3 * (p2.X - p1.X);

            double cy = 3 * (p2.Y - p1.Y);

            double cz = 3 * (p2.Z - p1.Z);

            double bx = 3 * (p3.X - p2.X) - cx;

            double by = 3 * (p3.Y - p2.Y) - cy;

            double bz = 3 * (p3.Z - p2.Z) - cz;

            double ax = p4.X - p1.X - bx - cx;

            double ay = p4.Y - p1.Y - by - cy;

            double az = p4.Z - p1.Z - bz - cz;

            for (int i = 0; i < count; i++)
            {
                double f = (double)i / (count - 1);

                double x = ax * f * f * f + bx * f * f + cx * f + p1.X;

                double y = ay * f * f * f + by * f * f + cy * f + p1.Y;

                double z = az * f * f * f + bz * f * f + cz * f + p1.Z;

                pts[i] = new Point3D(x, y, z);
            }

            return pts;
        }

        /// <summary>
        /// Interpolates the bezier.
        /// </summary>
        /// <param name="p1">The p1 value.</param>
        /// <param name="p2">The p2 value.</param>
        /// <param name="p3">The p3 value.</param>
        /// <param name="p4">The p4 value.</param>
        /// <param name="count">The count.</param>
        /// <returns>The points</returns>
        internal static Point[] InterpolateBezier(Point p1, Point p2, Point p3, Point p4, int count)
        {
            Point[] pts = new Point[count];

            double cx = 3 * (p2.X - p1.X);

            double cy = 3 * (p2.Y - p1.Y);

            double bx = 3 * (p3.X - p2.X) - cx;

            double by = 3 * (p3.Y - p2.Y) - cy;

            double ax = p4.X - p1.X - bx - cx;

            double ay = p4.Y - p1.Y - by - cy;

            for (int i = 0; i < count; i++)
            {
                double f = (double)i / (count - 1);

                double x = ax * f * f * f + bx * f * f + cx * f + p1.X;

                double y = ay * f * f * f + by * f * f + cy * f + p1.Y;

                pts[i] = new Point(x, y);
            }

            return pts;
        }

        /// <summary>
        /// Adds the quad.
        /// </summary>
        /// <param name="geomerty">The geomerty.</param>
        /// <param name="p1">The p1 value.</param>
        /// <param name="p2">The p2 value.</param>
        /// <param name="p3">The p3 value.</param>
        /// <param name="p4">The p4 value.</param>
        /// <param name="normal">The normal.</param>
        private static void AddQuad(MeshGeometry3D geomerty, Point3D p1, Point3D p2, Point3D p3, Point3D p4, Vector3D normal)
        {
            int lastIndex = geomerty.Positions.Count;

            geomerty.Positions.Add(p1);
            geomerty.Positions.Add(p2);
            geomerty.Positions.Add(p3);
            geomerty.Positions.Add(p4);

            geomerty.Normals.Add(normal);
            geomerty.Normals.Add(normal);
            geomerty.Normals.Add(normal);
            geomerty.Normals.Add(normal);

            geomerty.TriangleIndices.Add(lastIndex);
            geomerty.TriangleIndices.Add(lastIndex + 1);
            geomerty.TriangleIndices.Add(lastIndex + 2);
            geomerty.TriangleIndices.Add(lastIndex);
            geomerty.TriangleIndices.Add(lastIndex + 2);
            geomerty.TriangleIndices.Add(lastIndex + 3);
        }

        /// <summary>
        /// Adds the textured quad.
        /// </summary>
        /// <param name="geomerty">The geomerty.</param>
        /// <param name="p1">The p1 value.</param>
        /// <param name="p2">The p2 value.</param>
        /// <param name="p3">The p3 value.</param>
        /// <param name="p4">The p4 value.</param>
        /// <param name="normal">The normal.</param>
        /// <seealso cref="MeshGenerator"/>
        private static void AddTexturedQuad(MeshGeometry3D geomerty, Point3D p1, Point3D p2, Point3D p3, Point3D p4, Vector3D normal)
        {
            int lastIndex = geomerty.Positions.Count;

            geomerty.Positions.Add(p1);
            geomerty.Positions.Add(p2);
            geomerty.Positions.Add(p3);
            geomerty.Positions.Add(p4);

            //geomerty.TextureCoordinates.Add(new Point(1, 0));
            //geomerty.TextureCoordinates.Add(new Point(0, 0));
            //geomerty.TextureCoordinates.Add(new Point(0, 1));
            //geomerty.TextureCoordinates.Add(new Point(1, 1));

            geomerty.TextureCoordinates.Add(new Point(0, 0));
            geomerty.TextureCoordinates.Add(new Point(0, 1));
            geomerty.TextureCoordinates.Add(new Point(1, 1));
            geomerty.TextureCoordinates.Add(new Point(1, 0));

            //geomerty.Normals.Add(normal);
            //geomerty.Normals.Add(normal);
            //geomerty.Normals.Add(normal);
            //geomerty.Normals.Add(normal);

            //geomerty.Normals.Add(new Vector3D(0, 0, -1));
            //geomerty.Normals.Add(new Vector3D(0, 0, -1));
            //geomerty.Normals.Add(new Vector3D(0, 0, -1));
   
            geomerty.TriangleIndices.Add(lastIndex);
            geomerty.TriangleIndices.Add(lastIndex + 1);
            geomerty.TriangleIndices.Add(lastIndex + 2);
            geomerty.TriangleIndices.Add(lastIndex);
            geomerty.TriangleIndices.Add(lastIndex + 2);
            geomerty.TriangleIndices.Add(lastIndex + 3);  
        }
        /// <summary>
        /// Adds the tri.
        /// </summary>
        /// <param name="geomerty">The geomerty.</param>
        /// <param name="p1">The p1 value.</param>
        /// <param name="p2">The p2 value.</param>
        /// <param name="p3">The p3 value.</param>
        /// <param name="normal">The normal.</param>
        /// <seealso cref="MeshGenerator"/>
        private static void AddTri(MeshGeometry3D geomerty, Point3D p1, Point3D p2, Point3D p3, Vector3D normal)
        {
            int lastIndex = geomerty.Positions.Count;

            geomerty.Positions.Add(p1);
            geomerty.Positions.Add(p2);
            geomerty.Positions.Add(p3);

            geomerty.Normals.Add(normal);
            geomerty.Normals.Add(normal);
            geomerty.Normals.Add(normal);

            geomerty.TriangleIndices.Add(lastIndex);
            geomerty.TriangleIndices.Add(lastIndex + 1);
            geomerty.TriangleIndices.Add(lastIndex + 2);
        }

        /// <summary>
        /// The ShiftPoint method
        /// </summary>
        /// <param name="poin1">The point1 value</param>
        /// <param name="poin2">The point2 value</param>
        /// <param name="poin3">The point3 value</param>
        /// <returns>The point value</returns>
        private static Point ShiftPoint(Point poin1, Point poin2, Point poin3)
        {
            double b = ((double)Math.Abs(poin3.X - poin1.X) * poin2.X) / ((double)Math.Abs(poin3.Y - poin1.Y) * poin2.Y);
            double k = (poin2.Y - b) / poin2.Y;

            double x = 0.05 * Math.Cos(Math.Atan(k)) + poin2.X;
            double y = 0.05 * Math.Cos(Math.Atan(k)) + poin2.Y;

            return new Point(x, y);
        }
        #endregion

    }
}
