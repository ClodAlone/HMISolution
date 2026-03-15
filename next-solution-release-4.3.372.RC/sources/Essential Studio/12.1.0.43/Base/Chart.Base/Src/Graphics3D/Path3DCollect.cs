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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents polygones polygon.
    /// </summary>
    public sealed class Path3DCollect : Polygon
    {
        #region Members
        private ArrayList m_list = new ArrayList();
        private bool m_needRefresh = true;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the points of polygon.
        /// </summary>
        /// <value>The points.</value>
        public override Vector3D[] Points
        {
            get
            {
                if (m_needRefresh)
                {
                    RefreshPoints();
                    m_needRefresh = false;
                }

                return m_points;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Path3DCollect"/> class.
        /// </summary>
        /// <param name="paths">The array of polygons.</param>
        public Path3DCollect(Polygon[] paths)
            : base(paths[0].Normal, paths[0].D)
        {
            m_list.AddRange(paths);
            isClipPolygon = false;

            for (int i = 3; (Test()) && (i < Points.Length); i++)
                CalcNormal(Points[i], Points[0], Points[i / 2]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Path3DCollect"/> class.
        /// </summary>
        /// <param name="paths">The polygon.</param>
        public Path3DCollect(Polygon paths)
            : base(paths.Normal, paths.D)
        {
            m_list.Add(paths);
            isClipPolygon = false;

            for (int i = 3; (i < Points.Length) && (Test()); i++)
                CalcNormal(Points[i], Points[0], Points[i / 2]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Path3DCollect"/> class.
        /// </summary>
        /// <param name="p3dc">The <see cref="Path3DCollect"/>.</param>
        public Path3DCollect(Path3DCollect p3dc)
            : base(p3dc.m_normal, p3dc.D)
        {
            for (int i = 0; i < p3dc.m_list.Count; i++)
            {
                if (p3dc.m_list[i] != null)
                {
                    m_list.Add(((Polygon)p3dc.m_list[i]).Clone());
                }
            }

            isClipPolygon = false;

            for (int i = 3; (i < Points.Length) && (Test()); i++)
                CalcNormal(Points[i], Points[0], Points[i / 2]);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified polygon to the group.
        /// </summary>
        /// <param name="polygon">The <see cref="Polygon"/>.</param>
        /// <returns>Returns the index of the added polygon.</returns>
        public int Add(Polygon polygon)
        {
            if (polygon != null)
            {
                m_needRefresh = true;
            }

            return (polygon != null) ? m_list.Add(polygon) : -1;
        }

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D"/>.
        /// </summary>
        /// <param name="g3d">The g3d.</param>
        /// <returns>Returns ChartRegion.</returns>
        public override ChartRegion Draw(Graphics3D g3d)
        {
            Region rgn = null;

            //for (int i = 0, l = m_list.Count; i < l; i++)
            for (int i = 0, l = m_list.Count; (i < l) && (m_list[i] != null); i++)
            {
                Polygon pl = m_list[i] as Polygon;
                ChartRegion r = pl.Draw(g3d);

                if (r != null)
                {
                    if (rgn == null)
                    {
                        rgn = r.Region;
                    }
                    else
                    {
                        rgn.Union(r.Region);
                    }
                }
            }

            ChartRegion res = null;

            if (m_dataRegion != null)
            {
                res = m_dataRegion.GetChartRegion(rgn);
            }

            return res;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the polygon.</returns>
        public override Polygon Clone()
        {
            return new Path3DCollect(this);
        }

        /// <summary>
        /// Transforms by the specified <see cref="Matrix3D"/>.
        /// </summary>
        /// <param name="matrix3D">The <see cref="Matrix3D"/>.</param>
        public override void Transform(Matrix3D matrix3D)
        {
            foreach (Polygon polygon in m_list)
            {
                polygon.Transform(matrix3D);
            }

            RefreshPoints();
            CalcNormal();
        }

        /// <summary>
        /// Refreshes the points.
        /// </summary>
        public void RefreshPoints()
        {
            ArrayList ar = new ArrayList();

            for (int i = 0, c = m_list.Count; i < c; i++)
            {
                if (m_list[i] != null)
                {
                    ar.AddRange((m_list[i] as Polygon).Points);
                }
            }

            m_points = (Vector3D[])ar.ToArray(typeof(Vector3D));
        }
        #endregion
    }
}
