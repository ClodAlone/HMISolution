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

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// SvgElement class.
    /// </summary>
    public class SvgElement : SuperElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public Length Width
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_WIDTH, Length.Empty);
            }
            set
            {
                m_attributes[SVG.ATTR_WIDTH] = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public Length Height
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_HEIGHT, Length.Empty);
            }
            set
            {
                m_attributes[SVG.ATTR_HEIGHT] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length X
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_X, Length.Empty);
            }
            set
            {
                m_attributes[SVG.ATTR_X] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length Y
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_Y, Length.Empty);
            }
            set
            {
                m_attributes[SVG.ATTR_Y] = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgElement"/> class.
        /// </summary>
        public SvgElement()
        {
            m_name = SVG.NAME_SVG;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="g">The graphics.</param>
        public override void Draw(Graphics g)
        {
            Region tmp = g.Clip;
            SetClip(g);

            base.Draw(g);

            g.Clip = tmp;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Sets the clip.
        /// </summary>
        /// <param name="g">The graphics.</param>
        private void SetClip(Graphics g)
        {
            RectangleF bounds = g.ClipBounds;

            if (!X.IsEmpty)
            {
                bounds = new RectangleF(new PointF(X.Value, bounds.Y), bounds.Size);
            }

            if (!Y.IsEmpty)
            {
                bounds = new RectangleF(new PointF(bounds.X, Y.Value), bounds.Size);
            }

            if (!Width.IsEmpty)
            {
                bounds = new RectangleF(bounds.Location, new SizeF(Width.Value, bounds.Height));
            }

            if (!Height.IsEmpty)
            {
                bounds = new RectangleF(bounds.Location, new SizeF(bounds.Width, Height.Value));
            }

            g.Clip = new Region(bounds);
        }
        #endregion
    }
}
