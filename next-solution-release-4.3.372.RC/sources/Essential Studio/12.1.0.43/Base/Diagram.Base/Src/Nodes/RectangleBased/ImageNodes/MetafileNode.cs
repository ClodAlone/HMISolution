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
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Stores and renders an enhanced metafile image.
    /// </summary>
    [Serializable]
    public class MetafileNode
        : Node
    {
        #region Class members
        private Metafile m_metafile;
        private Byte[] m_metabytes = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileNode"/> class.
        /// </summary>
        /// <param name="stream">IO stream containing the Metafile.</param>
        public MetafileNode(Stream stream)
            : base()
        {
            Metafile metafile = new Metafile(stream);
            InitializeMetafileNode(metafile, new RectangleF(0, 0, metafile.Width, metafile.Height), MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileNode"/> class.
        /// </summary>
        /// <param name="filename">Name of file to load Metafile from.</param>
        public MetafileNode(string filename)
            : base()
        {
            Metafile metafile = new Metafile(filename);
            InitializeMetafileNode(metafile, new RectangleF(0, 0, metafile.Width, metafile.Height), MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileNode"/> class.
        /// </summary>
        /// <param name="srcMetafile">Metafile for the node.</param>
        public MetafileNode(Metafile srcMetafile)
            : base()
        {
            Metafile metafile = (Metafile)srcMetafile.Clone();
            InitializeMetafileNode(metafile, new RectangleF(0, 0, metafile.Width, metafile.Height), MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileNode"/> class.
        /// </summary>
        /// <param name="src">Metafile for the node.</param>
        /// <param name="rcBounds">The node bounds.</param>
        /// <param name="measureUnits">Specifies rcBounds measure units.</param>
        public MetafileNode(Metafile src, RectangleF rcBounds, MeasureUnits measureUnits)
            : base()
        {
            InitializeMetafileNode(src, rcBounds, measureUnits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileNode"/> class.
        /// </summary>
        /// <param name="src">Metafile for the node.</param>
        /// <param name="rcBounds">The node bounds.</param>
        public MetafileNode(Metafile src, RectangleF rcBounds)
            : base()
        {
            InitializeMetafileNode(src, rcBounds, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileNode"/> class.
        /// </summary>
        /// <param name="src">Object to copy.</param>
        public MetafileNode(MetafileNode src)
            : base(src)
        {
            m_metafile = (Metafile)src.m_metafile.Clone();
            m_metabytes = (byte[])src.m_metabytes.Clone();

            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileNode"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected MetafileNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Byte[] bytes = (Byte[])info.GetValue("metafile", typeof(Byte[]));

            // init metafile
            MemoryStream ms = new MemoryStream(bytes);
            m_metafile = new Metafile(ms);
            m_metabytes = bytes;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the <see cref="System.Drawing.Imaging.Metafile"/>.
        /// </summary>
        /// <value>The image.</value>
        public Metafile Image
        {
            get { return m_metafile; }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Renders shapes visual representation on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void Render(Graphics gfx)
        {
            // 1 - call base method impementation
            base.Render(gfx);

            // 2 - Draw interior
            DrawInterior(gfx);

            // 3 - Draw border
            DrawBorder(gfx);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public override object Clone()
        {
            return new MetafileNode(this);
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to
        /// serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("metafile", m_metabytes);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Draws the image border.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        private void DrawBorder(Graphics gfx)
        {
            if (this.LineStyle.LineWidth > 0)
            {
                using (Pen pen = this.LineStyle.CreatePen())
                    gfx.DrawPath(pen, this.GraphicsPath);
            }
        }

        /// <summary>
        /// Draws node's interior on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        private void DrawInterior(Graphics gfx)
        {
            if (MetaRecordParser.SupportFormat(m_metafile))
            {
                // use cloned metafile to draw on graphics.
                using (Metafile metafile = (Metafile)m_metafile.Clone())
                using (EmfRenderer renderer = new EmfRenderer(gfx, this.BoundingRect.Size))
                using (MetaRecordParser metaParser = new MetaRecordParser(renderer, metafile))
                {
                    metaParser.Enumerate();
                    gfx.DrawImage(metaParser.MetaFile, 0, 0, this.Size.Width, this.Size.Height);
                }
            }
            else
            {
                gfx.DrawImage(m_metafile, 0, 0, this.Size.Width, this.Size.Height);
            }
        }

        /// <summary>
        /// Initializes the metafile node from metafile image and bounds.
        /// </summary>
        /// <param name="metafile">The metafile image.</param>
        /// <param name="rectBounds">The bounds rectangle.</param>
        /// <param name="measureUnits">The bounds rectangle measure units.</param>
        private void InitializeMetafileNode(Metafile metafile, RectangleF rectBounds, MeasureUnits measureUnits)
        {
            if (metafile == null)
                throw new ArgumentNullException("metafile");

            m_metabytes = GetAsByteArray(metafile);
            m_metafile = metafile;

            rectBounds = MeasureUnitsConverter.ToPixels(rectBounds, measureUnits);
            //// calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);
            //// assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounds.Location.X + szPinOffsetUnitIndependent.Width,
                rectBounds.Location.Y + szPinOffsetUnitIndependent.Height);
            //// assign node size value
            SizeF szSizeUnitIndependent = rectBounds.Size;
            //// Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Serialize metafile image to byte array.
        /// </summary>
        /// <param name="meta">The metafile image.</param>
        /// <returns>Metafile byte array.</returns>
        private static byte[] GetAsByteArray(Metafile meta)
        {
            byte[] byts;
            Bitmap img = new Bitmap(meta);
            MemoryStream fs = new MemoryStream();
            try
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    IntPtr hDC = g.GetHdc();
                    using (Metafile m = new Metafile(fs, hDC))
                    {
                        using (Graphics gMeta = Graphics.FromImage(m))
                        {
                            gMeta.DrawImage(meta, new Point(0, 0));
                        }
                    }
                    g.ReleaseHdc(hDC);
                }

                fs.Seek(0, 0);
                byts = new byte[fs.Length];
                fs.Read(byts, 0, (int)fs.Length);
            }
            finally
            {
                img.Dispose();
                fs.Close();
            }

            return byts;
        }
        #endregion
    }
}
