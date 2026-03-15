#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for inserting bitmaps into a diagram.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.BitmapNode"/>
    /// </remarks>
    public class BitmapTool
        : RectangleToolBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public BitmapTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("BitmapTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            if (this.InAction)
            {
                // draw selection rectangle
                using (Pen pen = new Pen(Color.Black, 0f))
                {
                    pen.DashStyle = DashStyle.Dash;

                    // save graphics state
                    GraphicsState save = gfx.Save();
                    
                    // reset graphics transforms
                    gfx.PageScale = 1f;
                    gfx.Transform = new Matrix();
                    
                    // draw selection frame
                    gfx.DrawRectangle(pen, this.WorkRect);
                    
                    // restore graphics state
                    gfx.Restore(save);
                }
            }
        }

        /// <summary>
        /// Creates the node from given rectangle base.
        /// </summary>
        /// <param name="rectBounding">The bounding rectangle.</param>
        /// <returns>The tool.</returns>
        protected override Node CreateNode(RectangleF rectBounding)
        {
            Node nodeImage = null;

            // 1 - choose image
            OpenFileDialog dlgImage = new OpenFileDialog();
            dlgImage.Filter = "Windows Bitmaps (*.bmp)|*.bmp|JPEG files (*.jpg)|*.jpg|Graphics Interchange Format files (*.gif)|*.gif|Portable Network Graphics files (*.png)|*.png|Enhanced Metafiles (*.emf)|*.emf|All files (*.*)|*.*";
            dlgImage.DefaultExt = "*.bmp;*.jpg;*.gif;*.png;*.emf";
            dlgImage.Title = "Select an image file";

            if (dlgImage.ShowDialog(this.Controller.ParentControl) == DialogResult.OK)
            {
                // 2 - load chosen image
                Image imgToInsert = LoadImageFromFile(dlgImage.FileName);

                // 3 - create node on image type
                if (imgToInsert != null)
                {
                    // Create node.
                    nodeImage = CreateNode(imgToInsert, rectBounding);
                }
            }

            return nodeImage;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Creates the node from image.
        /// </summary>
        /// <param name="imgToInsert">The image to insert.</param>
        /// <param name="rectNodeBounds">The node bounds.</param>
        /// <returns>The node.</returns>
        public virtual Node CreateNode(Image imgToInsert, RectangleF rectNodeBounds)
        {
            Node nodeToReturn = null;

            if (imgToInsert is Bitmap)
            {
                nodeToReturn = new BitmapNode(imgToInsert as Bitmap, rectNodeBounds);
            }
            else if (imgToInsert is Metafile)
            {
                nodeToReturn = new MetafileNode(imgToInsert as Metafile, rectNodeBounds);
            }

            return nodeToReturn;
        }
        #endregion

        #region Class hepler methods
        private Image LoadImageFromFile(string strImagePath)
        {
            Image imgToReturn = null;

            if (File.Exists(strImagePath))
                imgToReturn = Image.FromFile(strImagePath);

            return imgToReturn;
        }
        #endregion
    }
}