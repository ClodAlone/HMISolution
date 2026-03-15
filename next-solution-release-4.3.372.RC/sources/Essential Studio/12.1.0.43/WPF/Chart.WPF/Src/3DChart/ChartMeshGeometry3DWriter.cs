// <copyright file="ChartMeshGeometry3DWriter.cs" company="Syncfusion">
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
    /// Represents ChartMeshGeometry3DWriter 
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class ChartMeshGeometry3DWriter
    {
        #region Members
        /// <summary>
        /// Initializes m_geometry
        /// </summary>
        private MeshGeometry3D m_geometry = null;
        #endregion      

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ChartMeshGeometry3DWriter class
        /// </summary>
        public ChartMeshGeometry3DWriter()
        {
           this.m_geometry = new MeshGeometry3D();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Geometry
        /// </summary>
        public MeshGeometry3D Geometry
        {
            get
            {
                return this.m_geometry;
            }
        }
        #endregion

        #region Public merhods

        #region Points
        /// <summary>
        /// The WritePoint method
        /// </summary>
        /// <param name="point">The point value</param>
        /// <param name="normal">The normal normal</param>
        /// <param name="uv">The uv point</param>
        /// <returns>The result</returns>
        public int WritePoint(Point3D point, Vector3D normal, Point uv)
        {
            int result = this.m_geometry.Positions.Count;

           this.m_geometry.Positions.Add(point);
           this.m_geometry.Normals.Add(normal);
           this.m_geometry.TextureCoordinates.Add(uv);

            return result;
        }

        /// <summary>
        /// The write point method
        /// </summary>
        /// <param name="index">The index value</param>
        public void WritePoint(int index)
        {
           this.m_geometry.TriangleIndices.Add(index);
        }

        /// <summary>
        /// The WriteSolidPoint method
        /// </summary>
        /// <param name="point">The point value</param>
        /// <param name="normal">The normal normal</param>
        /// <param name="uv">The uv point</param>
        public void WriteSolidPoint(Point3D point, Vector3D normal, Point uv)
        {
           this.m_geometry.TriangleIndices.Add(this.m_geometry.Positions.Count);
           this.m_geometry.Positions.Add(point);
           this.m_geometry.Normals.Add(normal);
           this.m_geometry.TextureCoordinates.Add(uv);
        }

        /// <summary>
        /// The Write Smooth Point method
        /// </summary>
        /// <param name="point">The Point value</param>
        /// <param name="normal">The normal value</param>
        /// <param name="uv">The uv point </param>
        public void WriteSmoothPoint(Point3D point, Vector3D normal, Point uv)
        {
            int index = -1;

            for (int i = this.m_geometry.Positions.Count - 1; i > -1; i--)
            {
                if (this.m_geometry.Positions[i] == point)
                {
                    index = i;
                    break;
                }
            }

            if (index > -1)
            {
               this.m_geometry.TriangleIndices.Add(index);
            }
            else
            {
               this.m_geometry.TriangleIndices.Add(this.m_geometry.Positions.Count);
               this.m_geometry.Positions.Add(point);
               this.m_geometry.Normals.Add(normal);
               this.m_geometry.TextureCoordinates.Add(uv);
            }
        }
        #endregion

        #region Triangles
        /// <summary>
        /// The Write Solid Triangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="n1">The n1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="n2">The n2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="n3">The n3 value</param>
        /// <param name="uv3">The uv3 value</param>
        public void WriteSolidTriangle(Point3D pt1, Vector3D n1, Point uv1, Point3D pt2, Vector3D n2, Point uv2, Point3D pt3, Vector3D n3, Point uv3)
        {
           this.WriteSolidPoint(pt1, n1, uv1);
           this.WriteSolidPoint(pt2, n2, uv2);
           this.WriteSolidPoint(pt3, n3, uv3);
        }

        /// <summary>
        ///  The Write Solid Triangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="uv3">The uv3 value</param>
        /// <param name="normal">The normal value</param>
        public void WriteSolidTriangle(Point3D pt1, Point uv1, Point3D pt2, Point uv2, Point3D pt3, Point uv3, Vector3D normal)
        {
           this.WriteSolidPoint(pt1, normal, uv1);
           this.WriteSolidPoint(pt2, normal, uv2);
           this.WriteSolidPoint(pt3, normal, uv3);
        }

        /// <summary>
        /// The WriteSolidTriangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="uv3">The uv3 value</param>
        public void WriteSolidTriangle(Point3D pt1, Point uv1, Point3D pt2, Point uv2, Point3D pt3, Point uv3)
        {
            Vector3D normal = Vector3D.CrossProduct(Point3D.Subtract(pt1, pt2), Point3D.Subtract(pt1, pt3));
           this.WriteSolidPoint(pt1, normal, uv1);
           this.WriteSolidPoint(pt2, normal, uv2);
           this.WriteSolidPoint(pt3, normal, uv3);
        }

        /// <summary>
        /// The WriteSmoothTriangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="n1">The n1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="n2">The n2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="n3">The n3 value</param>
        /// <param name="uv3">The uv3 value</param>
        public void WriteSmoothTriangle(Point3D pt1, Vector3D n1, Point uv1, Point3D pt2, Vector3D n2, Point uv2, Point3D pt3, Vector3D n3, Point uv3)
        {
           this.WriteSolidPoint(pt1, n1, uv1);
           this.WriteSolidPoint(pt2, n2, uv2);
           this.WriteSolidPoint(pt3, n3, uv3);
        }

        /// <summary>
        /// The WriteSmoothTriangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="uv3">The uv3 value</param>
        /// <param name="normal">The normal value</param>
        public void WriteSmoothTriangle(Point3D pt1, Point uv1, Point3D pt2, Point uv2, Point3D pt3, Point uv3, Vector3D normal)
        {
           this.WriteSolidPoint(pt1, normal, uv1);
           this.WriteSolidPoint(pt2, normal, uv2);
           this.WriteSolidPoint(pt3, normal, uv3);
        }

        /// <summary>
        /// The WriteSmoothTriangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="uv3">The uv3 value</param>
        public void WriteSmoothTriangle(Point3D pt1, Point uv1, Point3D pt2, Point uv2, Point3D pt3, Point uv3)
        {
            Vector3D normal = Vector3D.CrossProduct(Point3D.Subtract(pt1, pt2), Point3D.Subtract(pt1, pt3));
           this.WriteSolidPoint(pt1, normal, uv1);
           this.WriteSolidPoint(pt2, normal, uv2);
           this.WriteSolidPoint(pt3, normal, uv3);
        }
        #endregion

        #region Quadrangles
        /// <summary>
        /// The WriteSolidQuadrangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="n1">The n1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="n2">The n2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="n3">The n3 value</param>
        /// <param name="uv3">The uv3 value</param>
        /// <param name="pt4">The pt4 value</param>
        /// <param name="n4">The n4 value</param>
        /// <param name="uv4">The uv4 value</param>
        public void WriteSolidQuadrangle(Point3D pt1, Vector3D n1, Point uv1, Point3D pt2, Vector3D n2, Point uv2, Point3D pt3, Vector3D n3, Point uv3, Point3D pt4, Vector3D n4, Point uv4)
        {
           this.WriteSolidPoint(pt1, n1, uv1);
           this.WriteSolidPoint(pt2, n2, uv2);
           this.WriteSolidPoint(pt3, n3, uv3);
           this.WriteSolidPoint(pt1, n1, uv1);
           this.WriteSolidPoint(pt3, n3, uv3);
           this.WriteSolidPoint(pt4, n4, uv4);
        }

        /// <summary>
        /// The WriteSolidQuadrangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="uv3">The uv3 value</param>
        /// <param name="pt4">The pt4 value</param>
        /// <param name="uv4">The uv4 value</param>
        /// <param name="normal">The normal value</param>
        public void WriteSolidQuadrangle(Point3D pt1, Point uv1, Point3D pt2, Point uv2, Point3D pt3, Point uv3, Point3D pt4, Point uv4, Vector3D normal)
        {
           this.WriteSolidPoint(pt1, normal, uv1);
           this.WriteSolidPoint(pt2, normal, uv2);
           this.WriteSolidPoint(pt3, normal, uv3);
           this.WriteSolidPoint(pt1, normal, uv1);
           this.WriteSolidPoint(pt3, normal, uv3);
           this.WriteSolidPoint(pt4, normal, uv4);
        }

        /// <summary>
        /// The WriteSolidQuadrangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="uv3">The uv3 value</param>
        /// <param name="pt4">The pt4 value</param>
        /// <param name="uv4">The uv4 value</param>
        public void WriteSolidQuadrangle(Point3D pt1, Point uv1, Point3D pt2, Point uv2, Point3D pt3, Point uv3, Point3D pt4, Point uv4)
        {
            Vector3D normal = Vector3D.CrossProduct(Point3D.Subtract(pt1, pt2), Point3D.Subtract(pt1, pt3));
           this.WriteSolidPoint(pt1, normal, uv1);
           this.WriteSolidPoint(pt2, normal, uv2);
           this.WriteSolidPoint(pt3, normal, uv3);
           this.WriteSolidPoint(pt1, normal, uv1);
           this.WriteSolidPoint(pt3, normal, uv3);
           this.WriteSolidPoint(pt4, normal, uv4);
        }

        /// <summary>
        /// The WriteSmoothQuadrangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="n1">The n1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="n2">The n2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="n3">The n3 value</param>
        /// <param name="uv3">The uv3 value</param>
        /// <param name="pt4">The pt4 value</param>
        /// <param name="n4">The n4 value</param>
        /// <param name="uv4">The uv4 value</param>       
        public void WriteSmoothQuadrangle(Point3D pt1, Vector3D n1, Point uv1, Point3D pt2, Vector3D n2, Point uv2, Point3D pt3, Vector3D n3, Point uv3, Point3D pt4, Vector3D n4, Point uv4)
        {
           this.WriteSmoothPoint(pt1, n1, uv1);
           this.WriteSmoothPoint(pt2, n2, uv2);
           this.WriteSmoothPoint(pt3, n3, uv3);
           this.WriteSmoothPoint(pt1, n1, uv1);
           this.WriteSmoothPoint(pt3, n3, uv3);
           this.WriteSmoothPoint(pt4, n4, uv4);
        }

        /// <summary>
        /// WriteSmoothQuadrangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="uv3">The uv3 value</param>
        /// <param name="pt4">The pt4 value</param>
        /// <param name="uv4">The uv4 value</param>
        /// <param name="normal">The normal value</param>
        public void WriteSmoothQuadrangle(Point3D pt1, Point uv1, Point3D pt2, Point uv2, Point3D pt3, Point uv3, Point3D pt4, Point uv4, Vector3D normal)
        {
           this.WriteSmoothPoint(pt1, normal, uv1);
           this.WriteSmoothPoint(pt2, normal, uv2);
           this.WriteSmoothPoint(pt3, normal, uv3);
           this.WriteSmoothPoint(pt1, normal, uv1);
           this.WriteSmoothPoint(pt3, normal, uv3);
           this.WriteSmoothPoint(pt4, normal, uv4);
        }

        /// <summary>
        /// WriteSmoothQuadrangle method
        /// </summary>
        /// <param name="pt1">The pt1 value</param>
        /// <param name="uv1">The uv1 value</param>
        /// <param name="pt2">The pt2 value</param>
        /// <param name="uv2">The uv2 value</param>
        /// <param name="pt3">The pt3 value</param>
        /// <param name="uv3">The uv3 value</param>
        /// <param name="pt4">The pt4 value</param>
        /// <param name="uv4">The uv4 value</param>
        public void WriteSmoothQuadrangle(Point3D pt1, Point uv1, Point3D pt2, Point uv2, Point3D pt3, Point uv3, Point3D pt4, Point uv4)
        {
            Vector3D normal = Vector3D.CrossProduct(Point3D.Subtract(pt3, pt1), Point3D.Subtract(pt3, pt2));

            normal.Normalize();

           this.WriteSmoothPoint(pt1, normal, uv1);
           this.WriteSmoothPoint(pt2, normal, uv2);
           this.WriteSmoothPoint(pt3, normal, uv3);
           this.WriteSmoothPoint(pt1, normal, uv1);
           this.WriteSmoothPoint(pt3, normal, uv3);
           this.WriteSmoothPoint(pt4, normal, uv4);
        }
        #endregion

        #endregion
    }
}
