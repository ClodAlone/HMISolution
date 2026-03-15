#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A symbol palette is a collection of related SymbolModel objects used
    /// to add symbols to a diagram.
    /// </summary>
    /// <remarks>
    /// This class is a special type of model object that contains only
    /// SymbolModel objects. SymbolPalettes are serializable and can be
    /// saved to a file and reloaded. This class is used in conjunction
    /// with the PaletteGroupBar control, which displays the contents
    /// a SymbolPalette and allows the user to drag-and-drop symbols
    /// onto a diagram.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Model"/>
    /// </remarks>
    [Serializable]
    public class SymbolPalette
        : ISerializable,
          ICloneable
    {
        #region Class constants
        /// <summary>
        /// Default small image size.
        /// </summary>
        private readonly Size c_SMALL_IMAGE_SIZE = new Size(16, 16);

        /// <summary>
        /// Default large image size.
        /// </summary>
        private readonly Size c_LARGE_IMAGE_SIZE = new Size(32, 32);
        #endregion

        #region Class members
        /// <summary>
        /// Symbol palette rendering style.
        /// </summary>
        private RenderingStyle m_styleRendering;

        /// <summary>
        /// Store palette name.
        /// </summary>
        private string m_strPaletteName = string.Empty;

        /// <summary>
        /// Store palette nodes.
        /// </summary>
        private NodeCollection m_paletteNodes;

        /// <summary>
        /// Store small images of the palette.
        /// </summary>
        private ImageList m_smallImageList;

        /// <summary>
        /// Store array of bool values that indicates user definite small images.
        /// </summary>
        private ArrayList m_userSmallImages;

        /// <summary>
        /// Store large images of the palette.
        /// </summary>
        private ImageList m_largeImageList;

        /// <summary>
        /// Store array of bool values that indicates user definite large images.
        /// </summary>
        private ArrayList m_userLargeImages;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolPalette"/> class.
        /// </summary>
        public SymbolPalette()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolPalette"/> class.
        /// </summary>
        /// <param name="strPaletteName">Palette name.</param>
        public SymbolPalette(string strPaletteName)
        {
            m_strPaletteName = strPaletteName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolPalette"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public SymbolPalette(SymbolPalette src)
        {
            m_strPaletteName = (string)src.m_strPaletteName.Clone();
            m_paletteNodes = (NodeCollection)src.m_paletteNodes.Clone();
            m_styleRendering = (RenderingStyle)src.m_styleRendering.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolPalette"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected SymbolPalette(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "name":
                        m_strPaletteName = (string)entry.Value;
                        break;
                    case "children":
                        m_paletteNodes = (NodeCollection)entry.Value;
                        break;
                    case "renderingStyle":
                        m_styleRendering = (RenderingStyle)entry.Value;
                        break;
                    case "smallIconsAdv":
                        this.SmallImageList.ImageStream = (ImageListStreamer)entry.Value;
                        break;
                    case "smallIcons":
                        ArrayList images = (ArrayList)entry.Value;
                        if (images != null)                        
                            foreach (Image image in images)
                                this.SmallImageList.Images.Add(image);
                        break;
                    case "userSmallIcons":
                        m_userSmallImages = (ArrayList)entry.Value;
                        break;
                    case "largeIconsAdv":
                        this.LargeImageList.ImageStream = (ImageListStreamer)entry.Value;
                        break;
                    case "largeIcons":
                        images = (ArrayList)entry.Value;
                        if (images != null)
                            foreach (Image image in images)
                                this.LargeImageList.Images.Add(image);
                        break;
                    case "userLargeIcons":
                        m_userLargeImages = (ArrayList)entry.Value;
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the SymbolPalette rendering style.
        /// </summary>
        /// <remarks>
        /// The rendering style is used to configure graphics.
        /// </remarks>
        /// <value>The rendering style.</value>
        public RenderingStyle RenderingStyle
        {
            get
            {
                if (m_styleRendering == null)
                {
                    m_styleRendering = new RenderingStyle();
                    m_styleRendering.SmoothingMode = SmoothingMode.HighQuality;
                }

                return m_styleRendering;
            }
        }

        /// <summary>
        /// Gets or sets large <see cref="System.Windows.Forms.ImageList"/>.
        /// </summary>
        public ImageList LargeImageList
        {
            get
            {
                if (m_largeImageList == null)
                {
                    m_largeImageList = new ImageList();
                    m_largeImageList.ImageSize = c_LARGE_IMAGE_SIZE;
                    m_largeImageList.ColorDepth = ColorDepth.Depth32Bit;
                    m_userLargeImages = new ArrayList();
                }

                return m_largeImageList;
            }
            set
            {
                m_largeImageList = value;
            }
        }

        /// <summary>
        /// Gets or sets small <see cref="System.Windows.Forms.ImageList"/> of the palette.
        /// </summary>
        public ImageList SmallImageList
        {
            get
            {
                if (m_smallImageList == null)
                {
                    m_smallImageList = new ImageList();
                    m_smallImageList.ImageSize = c_SMALL_IMAGE_SIZE;
                    m_smallImageList.ColorDepth = ColorDepth.Depth32Bit;
                    m_userSmallImages = new ArrayList();
                }

                return m_smallImageList;
            }
            set
            {
                m_smallImageList = value;
            }
        }

        /// <summary>
        /// Gets or sets name of the palette.
        /// </summary>
        public string Name
        {
            get { return m_strPaletteName; }
            set { m_strPaletteName = value; }
        }

        /// <summary>
        /// Gets the copy of containing node collection.
        /// </summary>
        /// <value>The copy of containing node collection..</value>
        public NodeCollection Nodes
        {
            get
            {
                NodeCollection nodes = new NodeCollection();

                foreach (Node node in this.PaletteNodes)
                {
                    nodes.Add(node);
                }

                return nodes;
            }
        }

        /// <summary>
        /// Gets palette nodes collection.
        /// </summary>
        protected NodeCollection PaletteNodes
        {
            get
            {
                if (m_paletteNodes == null)
                {
                    m_paletteNodes = new NodeCollection();
                }

                return m_paletteNodes;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Occurs when collection of the nodes was changed.
        /// </summary>
        public event CollectionEEventHandler PaletteChildrenChanged;
        #endregion

        #region Class public methods
        /// <summary>
        /// Updates icons to display on <see cref="Syncfusion.Windows.Forms.Tools.GroupView"/>.
        /// </summary>
        public void UpdateIcons()
        {
            RecreateIcons();
        }

        /// <summary>
        /// Force recreation icons to display on <see cref="Syncfusion.Windows.Forms.Tools.GroupView"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RecreateIcons()
        {
            foreach (Node node in PaletteNodes)
            {               
                SetIcons(node, PaletteNodes.IndexOf(node));                
            }
        }

        /// <summary>
        /// Add node to the palette nodes and updates its icon to display on <see cref="Syncfusion.Windows.Forms.Tools.GroupView"/>.
        /// </summary>
        /// <param name="nodeNewPaletteNode">Node to add.</param>
        /// <returns>Index of added node in the palette nodes collection.</returns>
        public int AppendChild(Node nodeNewPaletteNode)
        {
            int nInsertIndex = this.PaletteNodes.Add(nodeNewPaletteNode);

            if (nInsertIndex >= 0)
            {
                CollectionExEventArgs evtArgs =
                    new CollectionExEventArgs(this, CollectionExChangeType.Insert, nodeNewPaletteNode, nInsertIndex);

                SetIcons(nodeNewPaletteNode);

                OnPaletteChildrenComplete(evtArgs);
            }
            return nInsertIndex;
        }

        /// <summary>
        /// Update icon associated with given Node.
        /// </summary>
        /// <param name="node">Node to update icon.</param>
        public void InsertImage(Node node)
        {
            SetIcons(node, PaletteNodes.IndexOf(node));
        }

        /// <summary>
        /// Remove node and its icons.
        /// </summary>
        /// <param name="nodePaletteChild">Node to remove.</param>
        public void RemoveChild(Node nodePaletteChild)
        {
            int nIdx = this.PaletteNodes.IndexOf(nodePaletteChild);

            if (nIdx >= 0)
            {
                RemoveIcons(nIdx);

                this.PaletteNodes.RemoveAt(nIdx);

                if (nIdx >= 0)
                {
                    CollectionExEventArgs evtArgs =
                        new CollectionExEventArgs(this, CollectionExChangeType.Remove, nodePaletteChild, nIdx);

                    OnPaletteChildrenComplete(evtArgs);
                }
            }
        }

        /// <summary>
        /// Inserts node at specified location.
        /// </summary>
        /// <param name="nodeToInsert">Node to insert.</param>
        /// <param name="nIndex">Insert index.</param>
        public void InsertChild(Node nodeToInsert, int nIndex)
        {
            if (nIndex >= 0 && this.PaletteNodes.Count > 0)
            {
                this.PaletteNodes.Insert(nIndex, nodeToInsert);
                SetIcons(nodeToInsert);
                UpdateIcons();

                if (nIndex >= 0)
                {
                    CollectionExEventArgs evtArgs =
                        new CollectionExEventArgs(this, CollectionExChangeType.Insert, nodeToInsert, nIndex);

                    OnPaletteChildrenComplete(evtArgs);
                }
            }
            else
                AppendChild(nodeToInsert);
        }

        /// <summary>
        /// Updates icons by ones index.
        /// </summary>
        /// <param name="index">Index of icons in the ImageList</param>
        public void UpdateImage(int index)
        {
            if (index < 0 || index > m_smallImageList.Images.Count)
            {
                throw new IndexOutOfRangeException("index");
            }

            if (!(bool)m_userSmallImages[index])
            {
                SetSmallIcon(PaletteNodes[index], index);
            }

            if (!(bool)m_userLargeImages[index])
            {
                SetLargeIcon(PaletteNodes[index], index);
            }
        }

        /// <summary>
        /// Sets user definite small image to the node by its index.
        /// </summary>
        /// <param name="index">Index of the node to set icon</param>
        /// <param name="image">Icon to set.</param>
        public void SetUserSmallImage(int index, Image image)
        {
            if (index < 0 || index > SmallImageList.Images.Count)
                throw new IndexOutOfRangeException("image index");

            if (image == null)
            {
                m_userSmallImages[index] = false;

                // Set default image
                SetSmallIcon(PaletteNodes[index], index);
            }
            else
            {
                SmallImageList.Images[index] = ScaleImage(image, SmallImageList.ImageSize);
                m_userSmallImages[index] = true;
            }
        }

        /// <summary>
        /// Sets user definite large image to the node by its index.
        /// </summary>
        /// <param name="index">Index of the node to set icon</param>
        /// <param name="image">Icon to set.</param>
        public void SetUserLargeImage(int index, Image image)
        {
            if (index < 0 || index > LargeImageList.Images.Count)
            {
                throw new IndexOutOfRangeException("image index");
            }

            if (image == null)
            {
                m_userLargeImages[index] = false;

                // Set default images
                SetLargeIcon(PaletteNodes[index], index);
            }
            else
            {
                LargeImageList.Images[index] = ScaleImage(image, LargeImageList.ImageSize);
                m_userLargeImages[index] = true;
            }
        }

        /// <summary>
        /// Gets value indicates that small icon for given node is user definite.
        /// </summary>
        /// <param name="index">Index of the Node to check icon.</param>
        /// <returns>TRUE if icon is user definite, otherwise FALSE.</returns>
        public bool IsUserDefinitedSmallIcon(int index)
        {
            if (index < 0 || index >= m_userSmallImages.Count)
            {
                return false;
            }

            return (bool)m_userSmallImages[index];
        }

        /// <summary>
        /// Gets value indicates that large icon for given node is user definite.
        /// </summary>
        /// <param name="index">Index of the Node to check icon.</param>
        /// <returns>TRUE if icon is user definite, otherwise FALSE.</returns>
        public bool IsUserDefinitedLargeIcon(int index)
        {
            if (index < 0 || index >= m_userLargeImages.Count)
            {
                return false;
            }

            return (bool)m_userLargeImages[index];
        }

        /// <summary>
        /// Update node in collection to given node by its index.
        /// </summary>
        /// <param name="node">Node to insert.</param>
        /// <param name="index">Index of old node.</param>
        public void UpdateNode(Node node, int index)
        {
            if (node == null)
            {
                throw new NullReferenceException("node");
            }

            if (index < 0 || index > PaletteNodes.Count)
            {
                throw new IndexOutOfRangeException("index");
            }

            PaletteNodes.RemoveAt(index);
            PaletteNodes.Insert(index, node);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Raises the <see cref="E:PaletteChildrenComplete"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPaletteChildrenComplete(CollectionExEventArgs evtArgs)
        {
            if (PaletteChildrenChanged != null)
            {
                PaletteChildrenChanged(this, evtArgs);
            }
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", m_strPaletteName);
            info.AddValue("children", m_paletteNodes);

            ArrayList smallImages = new ArrayList();
            foreach (Image img in SmallImageList.Images)
            {
                smallImages.Add(img);
            }

            info.AddValue("smallIcons", smallImages);
            info.AddValue("userSmallIcons", m_userSmallImages);
            info.AddValue("smallIconsAdv", m_smallImageList.ImageStream);

            ArrayList largeImages = new ArrayList();
            foreach (Image img in LargeImageList.Images)
            {
                largeImages.Add(img);
            }

            info.AddValue("largeIcons", largeImages);
            info.AddValue("userLargeIcons", m_userLargeImages);
            info.AddValue("largeIconsAdv", m_largeImageList.ImageStream);
            info.AddValue("renderingStyle", m_styleRendering);
        }

        /// <summary>
        /// Removes icons for given node by its index. Used only if node is removeng from palette nodes collection.
        /// </summary>
        /// <param name="index">index of the node to remove icons.</param>
        private void RemoveIcons(int index)
        {
            if (index < 0 || SmallImageList.Images.Count != LargeImageList.Images.Count)
                return;

            m_userSmallImages.RemoveAt(index);
            SmallImageList.Images.RemoveAt(index);
            m_userLargeImages.RemoveAt(index);
            LargeImageList.Images.RemoveAt(index);
        }

        /// <summary>
        /// Updates icons for given node.
        /// </summary>
        /// <param name="node">Node to update icons</param>
        private void SetIcons(Node node)
        {
            bool bDrawPorts = node.DrawPorts;
            node.DrawPorts = false;

            // make small icon
            Image img = MakeNodeSnapshot(node, c_SMALL_IMAGE_SIZE);
            if (img != null)
            {
                SmallImageList.Images.Add(img);
                m_userSmallImages.Add(false);
            }

            // make large icon
            img = MakeNodeSnapshot(node, c_LARGE_IMAGE_SIZE);
            if (img != null)
            {
                LargeImageList.Images.Add(img);
                m_userLargeImages.Add(false);
            }

            // restore node.DrawPorts value
            node.DrawPorts = bDrawPorts;
        }

        /// <summary>
        /// Update large icon for given node to default.
        /// </summary>
        /// <param name="node">Node to update icon.</param>
        /// <param name="index">Index of the node in palette nodes collection.</param>
        private void SetLargeIcon(Node node, int index)
        {
            if (index < 0 || index > m_largeImageList.Images.Count)
            {
                throw new IndexOutOfRangeException("insert index");
            }

            bool bDrawPorts = node.DrawPorts;
            node.DrawPorts = false;

            // make large icon
            Image img = MakeNodeSnapshot(node, c_LARGE_IMAGE_SIZE);
            if (img != null)
            {
                LargeImageList.Images[index] = img;
            }

            // restore node.DrawPorts value
            node.DrawPorts = bDrawPorts;
        }

        /// <summary>
        /// Update small icon for given node to default.
        /// </summary>
        /// <param name="node">Node to update small icon.</param>
        /// <param name="index">index of the node in palette nodes collection.</param>
        private void SetSmallIcon(Node node, int index)
        {
            if (index < 0 || index > m_smallImageList.Images.Count)
                throw new IndexOutOfRangeException("insert index");

            bool bDrawPorts = node.DrawPorts;
            node.DrawPorts = false;

            // make large icon
            Image img = MakeNodeSnapshot(node, c_SMALL_IMAGE_SIZE);
            if (img != null)
            {
                SmallImageList.Images[index] = img;
            }

            // restore node.DrawPorts value
            node.DrawPorts = bDrawPorts;
        }

        /// <summary>
        /// Updates icons for given node.
        /// </summary>
        /// <param name="node">Node to update icons.</param>
        /// <param name="insertIndex">Node's insert index.</param>
        private void SetIcons(Node node, int insertIndex)
        {
            if (LargeImageList.Images.Count > 0 && SmallImageList.Images.Count > 0)
            {
                if (insertIndex < 0 || insertIndex > m_smallImageList.Images.Count)
                    throw new IndexOutOfRangeException("insert index");

                //Image img = null;
                bool bDrawPorts = node.DrawPorts;
                node.DrawPorts = false;
                Image img = null;
                // make small icon
                if (m_userSmallImages[PaletteNodes.IndexOf(node)].ToString() != "True")
                {
                    img = MakeNodeSnapshot(node, c_SMALL_IMAGE_SIZE);
                    if (img != null)
                    {
                        SmallImageList.Images[insertIndex] = img;
                    }
                }

                if (m_userLargeImages[PaletteNodes.IndexOf(node)].ToString() != "True")
                {
                    // make large icon
                    img = MakeNodeSnapshot(node, c_LARGE_IMAGE_SIZE);
                    if (img != null)
                    {
                        LargeImageList.Images[insertIndex] = img;
                    }
                }

                // restore node.DrawPorts value
                node.DrawPorts = bDrawPorts;
            }
        }

        /// <summary>
        /// Makes snapshot for given node.
        /// </summary>
        /// <param name="node">Node to make snapshot.</param>
        /// <param name="szImg">Size of returned snapshot.</param>
        /// <returns>Snapshot for given node.</returns>
        protected Image MakeNodeSnapshot(Node node, Size szImg)
        {
            Image imageToReturn = new Bitmap(szImg.Width, szImg.Height);
            using (Graphics gfxBmp = Graphics.FromImage(imageToReturn))
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();

                IntPtr hdc = IntPtr.Zero;
                Image meta = null;
                if (CheckMetafileNode(node))
                {
                    int width = 1;
                    if (width < node.BoundingRectangle.Width)
                        width = (int)Math.Ceiling(node.BoundingRectangle.Width);
                    int height = 1;
                    if (height < node.BoundingRectangle.Height)
                        height = (int)Math.Ceiling(node.BoundingRectangle.Height);
                    meta = new System.Drawing.Bitmap(width, height);
                }
                else
                {
                    // create metafile to return
                    hdc = gfxBmp.GetHdc();
                    meta = new System.Drawing.Imaging.Metafile(stream, hdc);
                }

                RectangleF rectNodeBounds = new RectangleF();
                float fScaleFactor = 1;
                if (node.Size != SizeF.Empty)
                {
                    // create graphics to draw on
                    using (Graphics gfxToDrawOn = Graphics.FromImage(meta))
                    {
                        this.RenderingStyle.ApplySettings(gfxToDrawOn);
                        rectNodeBounds = ((IUnitIndependent)node).GetBoundingRectangle(MeasureUnits.Pixel, false);
                        if (node is LineBase)
                        {
                            LineBase line = node as LineBase;
                            rectNodeBounds.Height = rectNodeBounds.Height < 1 ? Math.Max(line.HeadDecorator.Size.Height, line.TailDecorator.Size.Height) : rectNodeBounds.Height;
                            rectNodeBounds.Width = rectNodeBounds.Width < 1 ? Math.Max(line.HeadDecorator.Size.Width, line.TailDecorator.Size.Width) : rectNodeBounds.Width;
                        }
            
                        // Fixed: D9982
                        if (Math.Round(rectNodeBounds.Height, 2) == 0)
                            rectNodeBounds.Height = rectNodeBounds.Width;
                        if (Math.Round(rectNodeBounds.Width, 2) == 0)
                            rectNodeBounds.Width = rectNodeBounds.Height;

                        // include shadow
                        if (node.ShadowStyle.Visible)
                        {
                            ShadowStyle styleShadow = node.ShadowStyle;

                            if (styleShadow.OffsetX < 0)
                            {
                                rectNodeBounds.X += styleShadow.OffsetX;
                            }

                            rectNodeBounds.Width += Math.Abs(styleShadow.OffsetX);

                            if (styleShadow.OffsetY < 0)
                            {
                                rectNodeBounds.Y -= styleShadow.OffsetY;
                            }

                            rectNodeBounds.Height += Math.Abs(styleShadow.OffsetY);
                        }

                        SizeF szNodeSize = rectNodeBounds.Size;

                        float fScaleFactorX = (szNodeSize.Width > 0) ? szImg.Width / szNodeSize.Width : 1;
                        float fScaleFactorY = (szNodeSize.Height > 0) ? szImg.Height / szNodeSize.Height : 1;

                        // calculate new magnification factor
                        fScaleFactor = Math.Min(fScaleFactorX, fScaleFactorY);

                        // set new origin offset and magnification
                        gfxToDrawOn.TranslateTransform(-rectNodeBounds.X, -rectNodeBounds.Y, MatrixOrder.Append);

                        gfxToDrawOn.PageScale = fScaleFactor + (float)0.5;
                        gfxToDrawOn.PageUnit = GraphicsUnit.Pixel;

                        node.Draw(gfxToDrawOn);
                    }
                }
                if (hdc != IntPtr.Zero)
                    gfxBmp.ReleaseHdc(hdc);

                float fThumbWidth = (node is LineBase && node.BoundingRectangle.Width <= szImg.Width) ? rectNodeBounds.Width : rectNodeBounds.Width * fScaleFactor;
                float fThumbHeight = (node is LineBase && node.BoundingRectangle.Height <= szImg.Height) ? rectNodeBounds.Height : rectNodeBounds.Height * fScaleFactor;
                float fThumbXPos = szImg.Width / 2 - fThumbWidth / 2;
                float fThumbYPos = szImg.Height / 2 - fThumbHeight / 2;

                if (node.Size != SizeF.Empty)
                {
                    gfxBmp.DrawImage(meta, new System.Drawing.Rectangle((int)Math.Round(fThumbXPos), (int)Math.Round(fThumbYPos), (int)Math.Round(fThumbWidth), (int)Math.Round(fThumbHeight)));
                }
            }

            return imageToReturn;
        }

        /// <summary>
        /// Check for MetaFileNode existence.
        /// </summary>
        /// <param name="node">Node to check.</param>        
        /// <returns>true if node is MetaFileNode</returns>
        private bool CheckMetafileNode(Node node)
        {
            if (node is Group)
            {
                Group group = node as Group;
                for (int i = 0; i < group.ChildCount; i++)
                {
                    Node child = group.GetChild(i);
                    if (child is MetafileNode)
                        return true;
                    else if (child is Group)
                    {
                        return CheckMetafileNode((Group)child);
                    }
                }
            }
            else if (node is MetafileNode)
                return true;
            return false;
        }

        /// <summary>
        /// Scale image to new size.
        /// </summary>
        /// <param name="imgScale">Image to resize.</param>
        /// <param name="szNewSize">New image size.</param>
        /// <returns>The scaled image.</returns>
        private Image ScaleImage(Image imgScale, Size szNewSize)
        {
            Image imgToReturn = imgScale;

            // if current size different to new
            if (imgScale.Size != szNewSize)
            {
                imgToReturn = new Bitmap(imgScale, szNewSize);
            }

            return imgToReturn;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new SymbolPalette(this);
        }
        #endregion

        #region ISerializable Members
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }
        #endregion

        #region Load Palette From File
        /// <summary>
        /// Loads the symbol palette from the given path.
        /// </summary>
        /// <param name="filepath">Path of the file.</param>
        /// <returns>Symbol palette.</returns>
        public SymbolPalette FromFile(string filepath)
        {
            SymbolPalette curSymbolPalette = null;
            FileStream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            if (iStream != null)
            {
                IFormatter formatter = new BinaryFormatter();
				formatter.Binder = new OldToNewDeserializationBinder();
                try
                {
                    AppDomain.CurrentDomain.AssemblyResolve +=
                        new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
                    curSymbolPalette = (SymbolPalette)formatter.Deserialize(iStream);
                }
                catch (SerializationException)
                {
                    try
                    {
                        formatter = new SoapFormatter();
						formatter.Binder = new OldToNewDeserializationBinder();
                        iStream.Position = 0;
                        curSymbolPalette = (SymbolPalette)formatter.Deserialize(iStream);
                    }
                    catch (Exception se)
                    {
                        // To get the version from the edp file
                        string strRegex = @"version\w*=\w*\d+\.\d+\.\d+(\.\d+)+\w*,";
                        Regex regex = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        StreamReader sr = File.OpenText(filepath);
                        string strFileContent = sr.ReadToEnd();
                        Match version = regex.Match(strFileContent);
                        string strVersion = version.Value;

                        if (string.IsNullOrEmpty(strVersion))
                        {
                            throw new System.Exception("Incompatible with older version.\nPlease use the converter utility to make it compatible with newer version", se);
                        }
                        else
                        {
                            throw new System.Exception("Unable to parse the palette file " + strVersion, se);
                        }
                    }
                }
                finally
                {
                    iStream.Close();
                    AppDomain.CurrentDomain.AssemblyResolve -=
                       new ResolveEventHandler(DiagramBaseAssembly.AssemblyResolver);
                }
            }
            return curSymbolPalette;
        }
        #endregion
    }

    /// <summary>
    /// Class for DragHelper
    /// </summary>
    public class DiagramDragHelper : DragHelper
    {
        #region Members
        private DragAction m_dragAction;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramDragHelper"/> class.
        /// </summary>
        public DiagramDragHelper()
            : base()
        {

        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the value indicating the current drag opeartion.
        /// </summary>
        public DragAction DragAction
        {
            get { return m_dragAction; }
            set
            {
                if (value != m_dragAction)
                    m_dragAction = value;
            }
        }
        #endregion
    }
}
