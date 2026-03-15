#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
#else
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents Graphics3D.
    /// </summary>
    public class Graphics3D
    {
        #region Members

        private ChartTransform.ChartTransform3D transform;
        private readonly BspTreeBuilder treeBuilder = new BspTreeBuilder();
        private BspNode tree;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>The transform.</value>
        public ChartTransform.ChartTransform3D Transform
        {
            get
            {
                return transform;
            }

            set
            {
                if (transform != value)
                {
                    transform = value;
                }
            }
        }
        #endregion

        #region Public methods

        public int GetVisualCount()
        {
            return treeBuilder.Count();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphics3D"/> class.
        /// </summary>
        /// <summary>
        /// Adds the polygon to the drawing.
        /// </summary>
        /// <param name="polygon">The <see cref="Polygon3D"/>.</param>
        /// <returns></returns>
        public int AddVisual(Polygon3D polygon)
        {
            if ((polygon == null) || (polygon.Test()))
            {
                return -1;
            }
            polygon.Graphics3D = this;
            return treeBuilder.Add(polygon);
        }

        /// <summary>
        /// Removes the specified polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        public void Remove(Polygon3D polygon)
        {
            treeBuilder.Remove(polygon);
        }

        /// <summary>
        /// clear the polygon from visual tree.
        /// </summary>
        public void ClearVisual()
        {
            treeBuilder.Clear();
        }

        public List<Polygon3D> GetVisual()
        {
            return treeBuilder.Polygons;
        }

        /// <summary>
        /// Computes the BSP tree.
        /// </summary>
        public void PrepareView()
        {
            tree = treeBuilder.Build();
        }

        /// <summary>
        /// Computes the BSP tree.
        /// </summary>
        public void PrepareView(double perspectiveAngle,double depth, double rotation, double tilt, Size size)
        {
            if (transform == null)
                transform = new ChartTransform.ChartTransform3D(size);
            else
                transform.mViewport = size;
            transform.Rotation = rotation;
            transform.Tilt = tilt;
            transform.Depth = depth;
            transform.PerspectiveAngle = perspectiveAngle;
            transform.Transform();
            tree = treeBuilder.Build();
        }

        /// <summary>
        /// Draws the paths to the panel/>.
        /// </summary>
        public void View(Panel panel)
        {
            if (panel == null) return;
            panel.Children.Clear();
            var eye = new Vector3D(0, 0, short.MaxValue);
            DrawBspNode3D(tree, eye, panel);
        }

        /// <summary>
        /// Draws the polygons to the <see cref="System.Drawing.Graphics"/>.
        /// </summary>
        public void View(Panel panel, double rotation, double tilt, Size size, double perspectiveAngle, double depth)
        {
            if (panel == null) return;
            panel.Children.Clear();
            if (transform == null)
                transform = new ChartTransform.ChartTransform3D(size);
            else
                transform.mViewport = size;
            transform.Rotation = rotation;
            transform.Tilt = tilt;
            transform.Depth = depth;
            transform.PerspectiveAngle = perspectiveAngle;
            transform.Transform();
            var eye = new Vector3D(0, 0, short.MaxValue);
            DrawBspNode3D(tree, eye, panel);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Draws the BSP node in 3D.
        /// </summary>
        /// <param name="tr">The tree.</param>
        /// <param name="eye">The eye position.</param>
        /// <param name="panel"></param>
        private void DrawBspNode3D(BspNode tr, Vector3D eye, Panel panel)
        {
            if (tr == null || transform == null) return;
            while (true)
            {
                var r = tr.Plane.GetNormal(transform.Result) & eye;
                if (r > tr.Plane.D)
                {
                    if (tr.Front != null)
                    {
                        DrawBspNode3D(tr.Front, eye, panel);
                    }

                    tr.Plane.Draw(panel);

                    if (tr.Back != null)
                    {
                        tr = tr.Back;
                        continue;
                    }
                }
                else
                {
                    if (tr.Back != null)
                    {
                        DrawBspNode3D(tr.Back, eye, panel);
                    }

                    tr.Plane.Draw(panel);

                    if (tr.Front != null)
                    {
                        tr = tr.Front;
                        continue;
                    }
                }
                break;
            }
        }

        #endregion
    }
}
