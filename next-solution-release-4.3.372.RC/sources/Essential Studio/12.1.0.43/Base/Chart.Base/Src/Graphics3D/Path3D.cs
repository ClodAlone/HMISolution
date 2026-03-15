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
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class PathGroup3D : Polygon
	{
		#region Internal class
		/// <summary>
		/// 
		/// </summary>
		class PathItem
		{
			public Pen Pen;
			public Brush Brush;
			public BrushInfo BrushInfo;

			public int Index;
			public int Length;

			public byte[] Types; 
		}
		#endregion

		#region Members
		private List<PathItem> m_items = new List<PathItem>();
		#endregion

		#region Constructor
		/// <summary>
		/// 
		/// </summary>      
        ///<param name="z"></param> 
		public PathGroup3D(double z)
			: base(new Vector3D(0, 0, 1), z)
		{ 
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Adds the path.
		/// </summary>
		/// <param name="gp">The gp.</param>
		/// <param name="brush">The brush.</param>
        /// <param name="brushInfo">The BrushInfo</param>
		/// <param name="pen">The pen.</param>
		public void AddPath(GraphicsPath gp, Brush brush, BrushInfo brushInfo, Pen pen)
		{
			PathData pathData = gp.PathData;
			PathItem item = new PathItem();

			item.Brush = brush;
			item.BrushInfo = brushInfo;
			item.Pen = pen;
			item.Index = m_points == null ? 0 : m_points.Length;
			item.Types = pathData.Types;
			item.Length = item.Types.Length;

			List<Vector3D> list = m_points == null ? 
				new List<Vector3D>() : new List<Vector3D>(m_points);

			foreach (PointF point in pathData.Points)
			{
				list.Add(new Vector3D(point.X, point.Y, m_d));
			}

			m_items.Add(item);
			m_points = list.ToArray();
		}
		#endregion

		#region implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="transform"></param>
		/// <returns></returns>
		internal override Vector3D GetNormal(Matrix3D transform)
		{
			Vector3D norm = transform & m_normal;
			norm.Normalize();

			return norm;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g3d"></param>
		/// <returns></returns>
		public override ChartRegion Draw(Graphics3D g3d)
		{
			PointF[] points = new PointF[m_points.Length];

			for (int i = 0; i < m_points.Length; i++)
			{
				points[i] = g3d.Transform.ToScreen(m_points[i]);
			}

			int coef = 0;

			if (g3d.Light)
			{
				Vector3D ln = !g3d.LightPosition;
				ln.Normalize();
				coef = (int)(g3d.LightCoeficient * (m_normal & ln));
			}

			Region reg = m_dataRegion == null ? null : new Region(Rectangle.Empty);

			foreach (PathItem item in m_items)
			{
				PointF[] subPoints = new PointF[item.Length];
				Array.Copy(points, item.Index, subPoints, 0, item.Length);
				GraphicsPath gp = new GraphicsPath(subPoints, item.Types);

				if (item.BrushInfo != null)
				{
					FillPolygon(g3d.Graphics, item.BrushInfo, gp, coef);
				}

				if (item.Brush != null)
				{
					FillPolygon(g3d.Graphics, item.Brush, gp, coef);
				}

				if (item.Pen != null)
				{
					g3d.Graphics.DrawPath(item.Pen, gp);
				}

				if (reg != null)
				{
					reg.Union(gp);
				}
			}

			return reg == null ? null : m_dataRegion.GetChartRegion(reg); 
		}
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Polygon Clone()
    {
			PathGroup3D pathGroup = new PathGroup3D(m_d);

			pathGroup.m_items = m_items;

			return pathGroup;
    }
		#endregion
	}

	/// <summary>
	/// Represents the <see cref="GraphicsPath"/> in the 3D.
	/// </summary>
	public sealed class Path3D : Polygon
	{
    #region Members
    private byte[] m_types; 
    #endregion

    #region Properties
		/// <summary>
		/// Gets the types.
		/// </summary>
		/// <value>The types.</value>
    public byte[] Types
    {
      get
      {
        return m_types;
      }
    }
    #endregion

    #region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="Path3D"/> class.
		/// </summary>
		/// <param name="plane">The plane.</param>
		private Path3D(Plane3D plane)
			: base( plane.Normal, plane.D )
		{ 
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Path3D"/> class.
		/// </summary>
		/// <param name="vs">The vs.</param>
    private Path3D( Vector3D[] vs ):base( vs )
    {
    }
		/// <summary>
		/// Initializes a new instance of the <see cref="Path3D"/> class.
		/// </summary>
		/// <param name="vs">The vs.</param>
		/// <param name="types">The types.</param>
		/// <param name="br">The br.</param>
		/// <param name="pen">The pen.</param>
    public Path3D( Vector3D[] vs, byte[] types, BrushInfo br, Pen pen ):this( vs )
    {
      m_types = types;
      m_brInfo = br;
      m_pen = pen;
    }
		/// <summary>
		/// Initializes a new instance of the <see cref="Path3D"/> class.
		/// </summary>
		/// <param name="p3d">The P3D.</param>
    public Path3D( Path3D p3d ) : base ( p3d.Normal, p3d.D )
    {
			m_points = p3d.m_points.Clone() as Vector3D[];
      m_types = (byte[])p3d.Types.Clone();
      m_brInfo = p3d.BrushInfo;
      m_brush = p3d.Brush;
      m_pen = p3d.Pen;
      m_FigurePen=p3d.m_FigurePen;
    }
    #endregion

    #region Public methods
		/// <summary>
		/// Creates <see cref="Path3D"/> from the graphics path.
		/// </summary>
		/// <param name="gp">The gp.</param>
		/// <param name="z">The z.</param>
		/// <param name="br">The br.</param>
		/// <param name="pen">The pen.</param>
		/// <returns></returns>
    public static Path3D FromGraphicsPath( GraphicsPath gp, double z, Brush br, Pen pen )
    {
      Path3D res = null;

      if( gp.PointCount > 0 )
      {
        PathData pd = gp.PathData;
        Vector3D[] vs = new Vector3D[ pd.Points.Length ];

        for( int i = 0; i < pd.Points.Length; i++ )
        {
          vs[ i ] = new Vector3D( pd.Points[i].X, pd.Points[i].Y, z );
        }

        res = new Path3D( vs );

        res.m_types = pd.Types;
        res.m_pen = pen;
        res.m_brush = br;
      }

      return res;
    }
		/// <summary>
		/// Creates <see cref="Path3D"/> from the graphics path.
		/// </summary>
		/// <param name="gp">The gp.</param>
		/// <param name="z">The z.</param>
		/// <param name="br">The br.</param>
		/// <param name="pen">The pen.</param>
		/// <returns></returns>
    public static Path3D FromGraphicsPath( GraphicsPath gp, double z, BrushInfo br, Pen pen )
    {
      Path3D res = null;

      if( gp.PointCount > 0 )
      {
        PathData pd = gp.PathData;
        Vector3D[] vs = new Vector3D[ pd.Points.Length ];

        for( int i = 0; i < pd.Points.Length; i++ )
        {
          vs[ i ] = new Vector3D( pd.Points[i].X, pd.Points[i].Y, z );
        }

        res = new Path3D( vs );

        res.m_types = pd.Types;
        res.m_pen = pen;
        res.m_brInfo = br;
      }

      return res;
    }
		/// <summary>
		/// Creates <see cref="Path3D"/> from the graphics path.
		/// </summary>
		/// <param name="gp">The gp.</param>
		/// <param name="z">The z.</param>
		/// <param name="br">The br.</param>
		/// <returns></returns>
    public static Path3D FromGraphicsPath( GraphicsPath gp, double z, Brush br )
    {
      Path3D res = null;

      if( gp.PointCount > 0 )
      {
        PathData pd = gp.PathData;
        Vector3D[] vs = new Vector3D[ pd.Points.Length ];

        for( int i = 0; i < pd.Points.Length; i++ )
        {
          vs[ i ] = new Vector3D( pd.Points[i].X, pd.Points[i].Y, z );
        }

        res = new Path3D( vs );

        res.m_types = pd.Types;
        res.m_brush = br;
      }

      return res;
    }
		/// <summary>
		/// Creates <see cref="Path3D"/> from the graphics path.
		/// </summary>
		/// <param name="gp">The gp.</param>
		/// <param name="z">The z.</param>
		/// <param name="pen">The pen.</param>
		/// <returns></returns>
    public static Path3D FromGraphicsPath( GraphicsPath gp, double z, Pen pen )
    {
      Path3D res = null;

      if( gp.PointCount > 0 )
      {
        PathData pd = gp.PathData;
        Vector3D[] vs = new Vector3D[ pd.Points.Length ];

        for( int i = 0; i < pd.Points.Length; i++ )
        {
          vs[ i ] = new Vector3D( pd.Points[i].X, pd.Points[i].Y, z );
        }

        res = new Path3D( vs );

        res.m_types = pd.Types;
        res.m_pen = pen;
      }

      return res;
    }
		/// <summary>
		/// Creates <see cref="Path3D"/> from the graphics path.
		/// </summary>
		/// <param name="gp">The gp.</param>
		/// <param name="z">The z.</param>
		/// <param name="br">The br.</param>
		/// <returns></returns>
    public static Path3D FromGraphicsPath( GraphicsPath gp, double z, BrushInfo br )
    {
      Path3D res = null;

      if( gp.PointCount > 0 )
      {
        PathData pd = gp.PathData;
        Vector3D[] vs = new Vector3D[ pd.Points.Length ];

        for( int i = 0; i < pd.Points.Length; i++ )
        {
          vs[ i ] = new Vector3D( pd.Points[i].X, pd.Points[i].Y, z );
        }

        res = new Path3D( vs );

        res.m_types = pd.Types;
        res.m_brInfo = br;
      }

      return res;
    }
		/// <summary>
		/// Creates <see cref="Path3D"/> from the graphics path.
		/// </summary>
		/// <param name="gp">The gp.</param>
		/// <param name="plane">The plane.</param>
		/// <param name="z">The z.</param>
		/// <param name="br">The br.</param>
		/// <param name="pen">The pen.</param>
		/// <returns></returns>
		public static Path3D FromGraphicsPath(GraphicsPath gp, Plane3D plane, double z, Brush br, Pen pen)
		{
			Path3D res = null;

			if (gp.PointCount > 0)
			{
				PathData pd = gp.PathData;
				Vector3D[] vs = new Vector3D[pd.Points.Length];

				for (int i = 0; i < pd.Points.Length; i++)
				{
					vs[i] = new Vector3D(pd.Points[i].X, pd.Points[i].Y, z);
				}

				res = new Path3D(plane);

				res.m_points = vs;
				res.m_types = pd.Types;
				res.m_pen = pen;
				res.m_brush = br;
			}

			return res;
		}
		/// <summary>
		/// 
		/// </summary>
    /// <param name="pts"></param>
    /// <returns></returns>
    public GraphicsPath GetPath( PointF[] pts )
    {    
      return new GraphicsPath( pts, m_types );
    }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g3d"></param>
		/// <returns></returns>
		public override ChartRegion Draw(Graphics3D g3d)
		{
			PointF[] points = new PointF[m_points.Length];

			for (int i = 0; i < m_points.Length; i++)
			{
				points[i] = g3d.Transform.ToScreen(m_points[i]);
			}

			GraphicsPath gp = new GraphicsPath(points, m_types);
			int coef = 0;

			if (g3d.Light)
			{
				Vector3D ln = !g3d.LightPosition;
				ln.Normalize();
				coef = (int)(g3d.LightCoeficient * (m_normal & ln));
			}

			if (m_brInfo != null)
			{
				FillPolygon(g3d.Graphics, m_brInfo, gp, coef);
			}

			if (m_brush != null)
			{
				FillPolygon(g3d.Graphics, m_brush, gp, coef);
			}

			if (m_pen != null)
			{
				g3d.Graphics.DrawPath(m_pen, gp);
			}

			ChartRegion res = null;

			if (m_dataRegion != null)
			{
				res = m_dataRegion.GetChartRegion(new Region(gp));
			}

			return res; 
		}
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Polygon Clone()
    {
      return new Path3D( this );
    }
    #endregion
  }
}
