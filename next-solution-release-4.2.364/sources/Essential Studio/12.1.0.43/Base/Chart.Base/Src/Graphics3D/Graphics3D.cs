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
using System.Collections;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    #region Class Transform3D
    /// <summary>
    /// Contains the view and projection transformation of <see cref="Graphics3D"/>
    /// </summary>
    public class Transform3D
    {
        #region Members
        private bool m_needUpdate = true;

        private Matrix3D m_centeredMatrix = Matrix3D.Identity;
        private Matrix3D m_viewMatrix = Matrix3D.Identity;
        private Matrix3D m_projectionMatrix = Matrix3D.Identity;
        private Matrix3D m_resultMatrix = Matrix3D.Identity;
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public Matrix3D Centered
        {
            get
            {
                return m_centeredMatrix;
            }
            set
            {
                if (m_centeredMatrix != value)
                {
                    m_centeredMatrix = value;
                    m_needUpdate = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        /// <value>The view.</value>
        public Matrix3D View
        {
            get
            {
                return m_viewMatrix; 
            }

            set
            {
                if (m_viewMatrix != value)
                {
                    m_viewMatrix = value;
                    m_needUpdate = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>The projection.</value>
        public Matrix3D Projection
        {
            get
            {
                return m_projectionMatrix; 
            }

            set
            {
                if (m_projectionMatrix != value)
                {
                    m_projectionMatrix = value;
                    m_needUpdate = true;
                }
            }
        }

        /// <summary>
        /// Gets the result matrix.
        /// </summary>
        /// <value>The result.</value>
        public Matrix3D Result
        {
            get
            {
                if (m_needUpdate)
                {
                    m_resultMatrix = Matrix3D.GetInvertal(m_centeredMatrix)
                      * m_projectionMatrix
                                  * m_viewMatrix * m_centeredMatrix;
                    m_needUpdate = false;
                }

                return m_resultMatrix;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the center of world.
        /// </summary>
        /// <param name="center">The center.</param>
        public void SetCenter(Vector3D center)
        {
            m_centeredMatrix = Matrix3D.Transform(-center.X, -center.Y, -center.Z);
            m_needUpdate = true;
        }

        /// <summary>
        /// Sets the perspective.
        /// </summary>
        /// <param name="distance">The distance to the "eye".</param>
        public void SetPerspective(double distance)
        {
            m_projectionMatrix = Matrix3D.GetIdentity();

            m_projectionMatrix[0, 0] = distance;
            m_projectionMatrix[1, 1] = distance;
            m_projectionMatrix[2, 3] = 1;
            m_projectionMatrix[3, 3] = distance;

            m_needUpdate = true;
        }

        /// <summary>
        /// Sets the view matrix by the position and direction of eye.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="up">The up.</param>
        public void SetLookAt(Vector3D pos, Vector3D dir, Vector3D up)
        {
            m_viewMatrix = Matrix3D.GetIdentity();

            Vector3D zAxis = !dir;
            zAxis.Normalize();

            Vector3D xAxis = up * zAxis;
            xAxis.Normalize();

            Vector3D yAxis = zAxis * xAxis;

            double offsetX = -(xAxis & pos);
            double offsetY = -(yAxis & pos);
            double offsetZ = -(zAxis & pos);

            m_viewMatrix[0, 0] = xAxis.X;
            m_viewMatrix[0, 1] = yAxis.X;
            m_viewMatrix[0, 2] = zAxis.X;

            m_viewMatrix[1, 0] = xAxis.Y;
            m_viewMatrix[1, 1] = yAxis.Y;
            m_viewMatrix[1, 2] = zAxis.Y;

            m_viewMatrix[2, 0] = xAxis.Z;
            m_viewMatrix[2, 1] = yAxis.Z;
            m_viewMatrix[2, 2] = zAxis.Z;

            m_viewMatrix[3, 0] = offsetX;
            m_viewMatrix[3, 1] = offsetY;
            m_viewMatrix[3, 2] = offsetZ;

            m_needUpdate = true;
        }

        /// <summary>
        /// Transform <see cref="Vector3D"/> to the screen.
        /// </summary>
        /// <param name="vector3d">The vector3d.</param>
        /// <returns></returns>
        public PointF ToScreen(Vector3D vector3d)
        {
            vector3d = Result * vector3d;
            return new PointF((float)vector3d.m_x, (float)vector3d.m_y);
        }

        /// <summary>
        /// Returns the intercept point of mouse ray with the specified plane.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="plane">The plane.</param>
        /// <returns></returns>
        public Vector3D ToPlane(PointF point, Plane3D plane)
        {
            Vector3D vec1 = new Vector3D(point.X, point.Y, 0);
            Vector3D vec2 = vec1 + new Vector3D(0, 0, 0.1);

            vec1 = m_centeredMatrix * vec1;
            vec2 = m_centeredMatrix * vec2;

            vec1 = Matrix3D.GetInvertal(m_projectionMatrix) * vec1;
            vec2 = Matrix3D.GetInvertal(m_projectionMatrix) * vec2;

            vec1 = plane.GetPoint(vec1, vec2 - vec1);

            vec1 = Matrix3D.GetInvertal(m_viewMatrix) * vec1;
            vec1 = Matrix3D.GetInvertal(m_centeredMatrix) * vec1;

            return vec1;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// Provide the methods for drawing in 3D mode. 
    /// </summary>
    public class Graphics3D
    {
        #region Members
        private bool m_light = false;
        private float m_lightCoef = 16;
        private Vector3D m_lightPosition = new Vector3D(0, 0, 1);

        private Transform3D m_transform = null;

        private BspTreeBuilder m_treeBuilder = new BspTreeBuilder();
        private BspNode m_tree = null;

        private IList m_regions;
        private Graphics m_graph;

        private static StringFormat m_defSFormat = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the root node.
        /// </summary>
        /// <value>The root node.</value>
        internal BspNode RootNode
        {
            get
            {
                return m_tree;
            }

            set
            {
                m_tree = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Chart.Polygon"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Polygon this[int index]
        {
            get
            {
                return m_treeBuilder[index];
            }
        }

        /// <summary>
        /// Gets the count of input polygons.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_treeBuilder.Count;
            }
        }

        /// <summary>
        /// Gets or sets the light position.
        /// </summary>
        /// <value>The light position.</value>
        public Vector3D LightPosition
        {
            get
            {
                return m_lightPosition;
            }

            set
            {
                m_lightPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the light coefficient.
        /// </summary>
        /// <value>The light coefficient.</value>
        public float LightCoeficient
        {
            get
            {
                return m_lightCoef;
            }

            set
            {
                m_lightCoef = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Graphics3D"/> is light.
        /// </summary>
        /// <value><c>true</c> if light; otherwise, <c>false</c>.</value>
        public bool Light
        {
            get
            {
                return m_light;
            }

            set
            {
                m_light = value;
            }
        }

        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public Graphics Graphics
        {
            get
            {
                return m_graph;
            }
        }

        /// <summary>
        /// Gets the count output polygons.
        /// </summary>
        /// <value>The count polygons.</value>
        public int CountPolygons
        {
            get
            {
                return m_treeBuilder.GetNodeCount(m_tree);
            }
        }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        /// <value>The regions.</value>
        public IList Regions
        {
            get
            {
                return m_regions;
            }

            set
            {
                m_regions = value;
            }
        }

        /// <summary>
        /// Gets the default string format.
        /// </summary>
        /// <value>The default string format.</value>
        public static StringFormat DefaultStrinfFormat
        {
            get
            {
                if (m_defSFormat == null)
                {
                    m_defSFormat = new StringFormat();
                    m_defSFormat.Alignment = StringAlignment.Near;
                }

                return m_defSFormat;
            }
        }

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>The transform.</value>
        public Transform3D Transform
        {
            get
            {
                if (m_transform == null)
                {
                    m_transform = new Transform3D();
                }

                return m_transform;
            }

            set
            {
                if (m_transform != value)
                {
                    m_transform = value;
                }
            }
        }
        #endregion

        #region Constrcutor
        /// <summary>
        /// Initializes a new instance of the <see cref="Graphics3D"/> class.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> to the drawing.</param>
        public Graphics3D(Graphics g)
        {
            m_graph = g;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the polygon to the drawing.
        /// </summary>
        /// <param name="polygon">The <see cref="Polygon"/>.</param>
        /// <returns></returns>
        public int AddPolygon(Polygon polygon)
        {
            if ((polygon == null) || (polygon.Test()))
            {
                return -1;
            }

            return m_treeBuilder.Add(polygon);
        }

        /// <summary>
        /// Computes the BSP tree.
        /// </summary>
        public void PrepairView()
        {
            m_tree = m_treeBuilder.Build();
        }

        /// <summary>
        /// Draws the polygons to the <see cref="Graphics3D.Graphics"/>.
        /// </summary>
        public void View3D()
        {
            Vector3D eye = new Vector3D(0, 0, short.MaxValue);
            Matrix3D mtx = Matrix3D.GetInvertal(m_transform.Centered) * m_transform.View * m_transform.Centered;

            //eye = mtx * eye;

            //BspNode temp_tree = TreeClone( m_tree );
            //MatrixTransform( temp_tree );
            DrawBspNode3D(m_tree, eye);
        }

        /// <summary>
        /// Saves the <see cref="Graphics3D"/> options.
        /// </summary>
        /// <returns></returns>
        public Graphics3DState SaveState()
        {
            Graphics3DState res = new Graphics3DState();

            res.Light = m_light;
            res.LightCoeficient = m_lightCoef;
            res.LightPosition = m_lightPosition;

            return res;
        }

        /// <summary>
        /// Loads the <see cref="Graphics3D"/> options.
        /// </summary>
        /// <param name="state">The state.</param>
        public void LoadState(Graphics3DState state)
        {
            m_light = state.Light;
            m_lightCoef = state.LightCoeficient;
            m_lightPosition = state.LightPosition;
        }

        #region Create objects
        /// <summary>
        /// Creates the box.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="p">The pen.</param>
        /// <param name="b">The brush.</param>
        /// <returns></returns>
        public Polygon[] CreateBox(Vector3D v1, Vector3D v2, Pen p, Brush b)
        {
            Polygon[] res = new Polygon[0];
            Vector3D sz = v1 - v2;

            if (sz.X != 0 && sz.Y != 0 && sz.Z != 0)
            {
                Vector3D[] p1 = new Vector3D[]{ new Vector3D( v1.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) };

                Vector3D[] p2 = new Vector3D[]{ new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v2.Z ) };

                Vector3D[] p3 = new Vector3D[]{ new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v1.X, v1.Y, v1.Z ) };

                Vector3D[] p4 = new Vector3D[]{ new Vector3D( v1.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) };

                Vector3D[] p5 = new Vector3D[]{ new Vector3D( v1.X, v1.Y, v1.Z ),
                                        new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) };

                Vector3D[] p6 = new Vector3D[]{ new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ) };

                Polygon pl1 = new Polygon(p1, b, null);
                Polygon pl2 = new Polygon(p2, b, null);
                Polygon pl3 = new Polygon(p3, b, p);
                Polygon pl4 = new Polygon(p4, b, p);
                Polygon pl5 = new Polygon(p5, b, p);
                Polygon pl6 = new Polygon(p6, b, p);

                this.AddPolygon(pl1);
                this.AddPolygon(pl2);
                this.AddPolygon(pl3);
                this.AddPolygon(pl4);
                this.AddPolygon(pl5);
                this.AddPolygon(pl6);

                res = new Polygon[] { pl1, pl2, pl3, pl4, pl5, pl6 };
            }

            return res;
        }

        /// <summary>
        /// Creates the box.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="p">The p.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public Polygon[] CreateBox(Vector3D v1, Vector3D v2, Pen p, BrushInfo b)
        {
            Polygon[] res = new Polygon[0];
            Vector3D sz = v1 - v2;

            //if( sz.X != 0 && sz.Y != 0 && sz.Z != 0 )
            {
                Vector3D[] p1 = new Vector3D[] { new Vector3D( v1.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) };

                Vector3D[] p2 = new Vector3D[] { new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v2.Z ) };

                Vector3D[] p3 = new Vector3D[] { new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v1.X, v1.Y, v1.Z ) };

                Vector3D[] p4 = new Vector3D[] { new Vector3D( v1.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) };

                Vector3D[] p5 = new Vector3D[] { new Vector3D( v1.X, v1.Y, v1.Z ),
                                        new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) };

                Vector3D[] p6 = new Vector3D[] { new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ) };

                Polygon pl1 = new Polygon(p1, b, null);
                Polygon pl2 = new Polygon(p2, b, null);
                Polygon pl3 = new Polygon(p3, b, p);
                Polygon pl4 = new Polygon(p4, b, p);
                Polygon pl5 = new Polygon(p5, b, p);
                Polygon pl6 = new Polygon(p6, b, p);

                this.AddPolygon(pl1);
                this.AddPolygon(pl2);
                this.AddPolygon(pl3);
                this.AddPolygon(pl4);
                this.AddPolygon(pl5);
                this.AddPolygon(pl6);

                res = new Polygon[] { pl1, pl2, pl3, pl4, pl5, pl6 };
            }

            return res;
        }

        /// <summary>
        /// Creates the vertical box.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="p">The p.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public Polygon[] CreateBoxV(Vector3D v1, Vector3D v2, Pen p, BrushInfo b)
        {
            Polygon[] res = new Polygon[0];
            Vector3D sz = v1 - v2;

            //if( sz.X != 0 && sz.Y != 0 && sz.Z != 0 )
            {
                Vector3D[] p1 = new Vector3D[] { new Vector3D( v1.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) };

                Vector3D[] p2 = new Vector3D[] { new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v2.Z ) };

                Vector3D[] p3 = new Vector3D[] { new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v1.X, v1.Y, v1.Z ) };

                Vector3D[] p4 = new Vector3D[] { new Vector3D( v1.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) };

                Vector3D[] p5 = new Vector3D[] { new Vector3D( v1.X, v1.Y, v1.Z ),
                                        new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) };

                Vector3D[] p6 = new Vector3D[] { new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ) };

                Polygon pl1 = new Polygon(p1, b, p);
                Polygon pl2 = new Polygon(p2, b, p);
                Polygon pl3 = new Polygon(p3, b, p);
                Polygon pl4 = new Polygon(p4, b, p);
                Polygon pl5 = new Polygon(p5, b, null);
                Polygon pl6 = new Polygon(p6, b, null);

                this.AddPolygon(pl6);
                this.AddPolygon(pl5);
                this.AddPolygon(pl1);
                this.AddPolygon(pl2);
                this.AddPolygon(pl3);
                this.AddPolygon(pl4);

                res = new Polygon[] { pl1, pl2, pl3, pl4, pl5, pl6 };
            }

            return res;
        }

        /// <summary>
        /// Creates the ellipse.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="sz">The sz.</param>
        /// <param name="dsc">The DSC.</param>
        /// <param name="p">The p.</param>
        /// <param name="br">The br.</param>
        /// <returns></returns>
        public Polygon[] CreateEllipse(Vector3D v1, SizeF sz, int dsc, Pen p, BrushInfo br)
        {
            Vector3D[] vs = new Vector3D[dsc];
            double k = 2 * Math.PI / dsc;

            float cx = (float)(v1.X + sz.Width / 2f);
            float cy = (float)(v1.Y + sz.Height / 2f);

            for (int i = 0; i < dsc; i++)
            {
                float x = (float)(cx + sz.Width / 2 * Math.Cos(i * k));
                float y = (float)(cy + sz.Height / 2 * Math.Sin(i * k));

                vs[i] = new Vector3D(x, y, v1.Z);
            }

            Polygon polygon = new Polygon(vs, br, p);
            AddPolygon(polygon);

            return new Polygon[] { polygon };
        }

        /// <summary>
        /// Creates the rectangle.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="sz">The sz.</param>
        /// <param name="p">The p.</param>
        /// <param name="br">The br.</param>
        /// <returns></returns>
        public Polygon[] CreateRectangle(Vector3D v1, SizeF sz, Pen p, BrushInfo br)
        {
            Vector3D[] vs = new Vector3D[4];

            vs[0] = new Vector3D(v1.X, v1.Y, v1.Z);
            vs[1] = new Vector3D(v1.X + sz.Width, v1.Y, v1.Z);
            vs[2] = new Vector3D(v1.X + sz.Width, v1.Y + sz.Height, v1.Z);
            vs[3] = new Vector3D(v1.X, v1.Y + sz.Height, v1.Z);

            Polygon polygon = new Polygon(vs, br, p);
            AddPolygon(polygon);

            return new Polygon[] { polygon };
        }

        /// <summary>
        /// Creates the rectangle.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="sz">The sz.</param>
        /// <param name="p">The p.</param>
        /// <param name="br">The br.</param>
        /// <param name="IsPNF">The PNF.</param>
        /// <returns></returns>
        public Polygon[] CreateRectangle(Vector3D v1, SizeF sz, Pen p, BrushInfo br, bool IsPNF)
        {
            Vector3D[] vs = new Vector3D[4];

            vs[0] = new Vector3D(v1.X, v1.Y, v1.Z);
            vs[1] = new Vector3D(v1.X + sz.Width, v1.Y, v1.Z);
            vs[2] = new Vector3D(v1.X + sz.Width, v1.Y + sz.Height, v1.Z);
            vs[3] = new Vector3D(v1.X, v1.Y + sz.Height, v1.Z);

            Polygon polygon = new Polygon(vs, br, p, IsPNF);
            AddPolygon(polygon);

            return new Polygon[] { polygon };
        }

        /// <summary>
        /// Creates the sphere.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="r">The r.</param>
        /// <param name="dsc">The DSC.</param>
        /// <param name="p">The p.</param>
        /// <param name="br">The br.</param>
        /// <returns></returns>
        public Polygon[] CreateShpeare(Vector3D v1, Vector3D r, int dsc, Pen p, BrushInfo br)
        {
            ArrayList res = new ArrayList();
            double coef = 2 * Math.PI / dsc;

            for (int i = 0; i < dsc - 1; i++)
            {
                double z1 = r.Z * Math.Cos(i * coef);
                double zx1 = r.X * Math.Sin(i * coef);
                double zy1 = r.Y * Math.Sin(i * coef);
                double z2 = r.Z * Math.Cos((i + 1) * coef);
                double zx2 = r.X * Math.Sin((i + 1) * coef);
                double zy2 = r.Y * Math.Sin((i + 1) * coef);

                for (int j = 0; j < dsc - 1; j++)
                {
                    double x11 = zx1 * Math.Cos(j * coef);
                    double y11 = zy1 * Math.Sin(j * coef);
                    double x21 = zx2 * Math.Cos(j * coef);
                    double y21 = zy2 * Math.Sin(j * coef);
                    double x12 = zx1 * Math.Cos((j + 1) * coef);
                    double y12 = zy1 * Math.Sin((j + 1) * coef);
                    double x22 = zx2 * Math.Cos((j + 1) * coef);
                    double y22 = zy2 * Math.Sin((j + 1) * coef);

                    Vector3D[] vs = new Vector3D[]
            {
              new Vector3D( x11, y11, z1 ) + v1,
              new Vector3D( x21, y21, z2 ) + v1,
              new Vector3D( x22, y22, z2 ) + v1,
              new Vector3D( x12, y12, z1 ) + v1
            };

                    Polygon polygon = new Polygon(vs, br);
                    res.Add(polygon);
                    AddPolygon(polygon);
                }
            }

            return (Polygon[])res.ToArray(typeof(Polygon));
        }

        /// <summary>
        /// Creates the vertical cylinder.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="dsc">The DSC.</param>
        /// <param name="p">The p.</param>
        /// <param name="br">The br.</param>
        /// <returns></returns>
        public Polygon[] CreateCylinderV(Vector3D v1, Vector3D v2, int dsc, Pen p, BrushInfo br)
        {
            ArrayList res = new ArrayList();

            Vector3D[] vst = new Vector3D[dsc];
            Vector3D[] vsb = new Vector3D[dsc];
            double k = 2 * Math.PI / dsc;

            Vector3D sz = v2 - v1;

            float cx = (float)(v1.X + sz.X / 2f);
            float cz = (float)(v1.Z + sz.Z / 2f);

            for (int i = 0; i < dsc; i++)
            {
                float x = (float)(cx + sz.X / 2 * Math.Cos(i * k));
                float z = (float)(cz + sz.Z / 2 * Math.Sin(i * k));
                float x2 = (float)(cx + sz.X / 2 * Math.Cos((i + 1) * k));
                float z2 = (float)(cz + sz.Z / 2 * Math.Sin((i + 1) * k));

                Polygon polygon = new Polygon(new Vector3D[]{
                                                   new Vector3D( x, v1.Y, z ),
                                                   new Vector3D( x, v2.Y, z ),
                                                   new Vector3D( x2, v2.Y, z2 ),
                                                   new Vector3D( x2, v1.Y, z2 )}, br);
                vst[i] = new Vector3D(x, v1.Y, z);
                vsb[i] = new Vector3D(x, v2.Y, z);

                res.Add(polygon);
                AddPolygon(polygon);
            }

            Polygon plgt = new Polygon(vst, br, p);
            Polygon plgb = new Polygon(vsb, br, p);
            AddPolygon(plgt);
            AddPolygon(plgb);
            res.Add(plgt);
            res.Add(plgb);

            return (Polygon[])res.ToArray(typeof(Polygon));
        }

        /// <summary>
        /// Creates the horizontal cylinder.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="dsc">The DSC.</param>
        /// <param name="p">The p.</param>
        /// <param name="br">The br.</param>
        /// <returns></returns>
        public Polygon[] CreateCylinderH(Vector3D v1, Vector3D v2, int dsc, Pen p, BrushInfo br)
        {
            ArrayList res = new ArrayList();

            Vector3D[] vst = new Vector3D[dsc];
            Vector3D[] vsb = new Vector3D[dsc];
            double k = 2 * Math.PI / dsc;

            Vector3D sz = v2 - v1;

            float cy = (float)(v1.Y + sz.Y / 2f);
            float cz = (float)(v1.Z + sz.Z / 2f);

            for (int i = 0; i < dsc; i++)
            {
                float y = (float)(cy + sz.Y / 2 * Math.Cos(i * k));
                float z = (float)(cz + sz.Z / 2 * Math.Sin(i * k));
                float y2 = (float)(cy + sz.Y / 2 * Math.Cos((i + 1) * k));
                float z2 = (float)(cz + sz.Z / 2 * Math.Sin((i + 1) * k));

                Polygon polygon = new Polygon(new Vector3D[]{
                                                   new Vector3D( v1.X, y, z ),
                                                   new Vector3D( v2.X, y, z ),
                                                   new Vector3D( v2.X, y2, z2 ),
                                                   new Vector3D( v1.X, y2, z2 )}, br);
                vst[i] = new Vector3D(v1.X, y, z);
                vsb[i] = new Vector3D(v2.X, y, z);

                res.Add(polygon);
                AddPolygon(polygon);
            }

            Polygon plgt = new Polygon(vst, br, p);
            Polygon plgb = new Polygon(vsb, br, p);
            AddPolygon(plgt);
            AddPolygon(plgb);
            res.Add(plgt);
            res.Add(plgb);

            return (Polygon[])res.ToArray(typeof(Polygon));
        }
        #endregion

        #endregion

        #region Helper methods
        /// <summary>
        /// Draws the BSP node in 3D.
        /// </summary>
        /// <param name="tr">The tree.</param>
        /// <param name="eye">The eye position.</param>
        private void DrawBspNode3D(BspNode tr, Vector3D eye)
        {
            double r = tr.Plane.GetNormal(m_transform.Result) & eye;

            if (r > tr.Plane.D)
            {
                if (tr.Front != null)
                {
                    this.DrawBspNode3D(tr.Front, eye);
                }

                ChartRegion res = tr.Plane.Draw(this);

                if (res != null)
                    m_regions.Add(res);

                if (tr.Back != null)
                {
                    this.DrawBspNode3D(tr.Back, eye);
                }
            }
            else
            {
                if (tr.Back != null)
                {
                    this.DrawBspNode3D(tr.Back, eye);
                }

                ChartRegion res = tr.Plane.Draw(this);

                if (res != null)
                    m_regions.Add(res);

                if (tr.Front != null)
                {
                    this.DrawBspNode3D(tr.Front, eye);
                }
            }
        }
        #endregion
    }
}
