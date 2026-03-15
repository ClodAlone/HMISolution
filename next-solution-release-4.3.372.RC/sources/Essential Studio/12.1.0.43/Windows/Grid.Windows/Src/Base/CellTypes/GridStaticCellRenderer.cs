//-------------------------------------------------------------------------------------------------
// <copyright file="GridStaticCellRenderer.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the renderer part of a static cell. A static cell renderer is also a base class for many
    /// other cell types and provides the inactive cell rendering for cell types that support editing (such as
    /// a text box or combo box).
    /// </summary>
    /// <remarks>
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridStaticCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// The following table lists some characteristics about the Static cell type.
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>Static</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridStaticCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridStaticCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>NA</description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Click Only</description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Support</term>
    ///         <description>Both</description>
    ///     </item>
    ///     <item>
    ///         <term>Base Type</term>
    ///         <description><see cref="GridCellRendererBase"/></description>
    ///     </item>
    /// </list>
    /// <para/>
    /// <para/>
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class.
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BackgroundImage"/> (<see cref="System.Drawing.Image"/>)</term>
    ///         <description>Gets / sets the image that the cell displays as background. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BackgroundImageMode"/> (<see cref="GridBackgroundImageMode"/>)</term>
    ///         <description>Indicates how the background image is displayed. (Default: GridBackgroundImageMode)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BaseStyle"/> (<see cref="System.String"/>)</term>
    ///         <description>The base style for this style instance with default values for properties that are not initialized for this style object. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Borders"/> (<see cref="GridBordersInfo"/>)</term>
    ///         <description>Top, left, bottom, and right border settings. (Default: GridBordersInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellAppearance"/> (<see cref="GridCellAppearance"/>)</term>
    ///         <description>Specifies if cell edges shall be drawn raised, sunken, or flat (default). (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>Static. (Default: TextBox)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValue"/> (<see cref="System.Object"/>)</term>
    ///         <description>This property holds the cell value. Although the cell value is typically a string, it can also be any other primitive type such as int, byte, enum, or any custom type that is derived from <see cref="System.Object"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValueType"/> (<see cref="System.Type"/>)</term>
    ///         <description>Specifies the preferred <see cref="System.Type"/> for cell values. When you assign a value to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information. (Default: null)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CultureInfo"/> (<see cref="System.Globalization.CultureInfo"/>)</term>
    ///         <description>The culture information holds rules for parsing and formatting the cells value. (Default: null)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if the cell should be skipped when moving the current cell. (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.FloatCell"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if text can float into the boundaries of a neighboring cell. (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.FloodCell"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if this cell can be flooded by a previous cell. (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Format"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the format mask for formatting the cell value. You can specify numeric format strings,
    /// date format strings, or enumeration format strings as discussed in the section "Format Specifiers and Format Providers" of the .NET Framework Developers Guide (see ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconformatspecifiersformatproviders.htm) (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HorizontalAlignment"/> (<see cref="GridHorizontalAlignment"/>)</term>
    ///         <description>Specifies horizontal alignment of text in the cell. (Default: GridHorizontalAlignment.Left)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HotkeyPrefix"/> (<see cref="System.Drawing.Text.HotkeyPrefix"/>)</term>
    ///         <description>Specifies how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand). When you enable hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not be displayed. (Default: HotkeyPrefix.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageIndex"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Specifies an index for an image in the <see cref="GridStyleInfo.ImageList"/> of a <see cref="GridStyleInfo"/>
    /// instance. (Default: -1)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageList"/> (<see cref="System.Windows.Forms.ImageList"/>)</term>
    ///         <description>The <see cref="GridStyleInfo.ImageList"/> that holds a collection of images. Cells can choose images with the <see cref="GridStyleInfo.ImageIndex"/> property in a <see cref="GridStyleInfo"/>
    /// instance. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MergeCell"/> (<see cref="GridMergeCellDirection"/>)</term>
    ///         <description>Specifies merge behavior for an individual cell when merging cells feature has been enabled in a <see cref="GridModel"/> with  <see cref="GridModelOptions.MergeCellsMode"/>. (Default: GridMergeCellDirection.None)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Text"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the value as a string. If a <see cref="GridStyleInfo.CellValueType"/>
    /// is specified, the text will be parsed and converted to the type specified with
    /// <see cref="GridStyleInfo.CellValueType"/> using any <see cref="GridStyleInfo.CultureInfo"/>
    /// information.
    ///  (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextColor"/> (<see cref="System.Drawing.Color"/>)</term>
    ///         <description>Lets you specify the color for drawing the cell text. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>Holds text margins in pixels. When drawing a cell, this specifies the empty area between the
    /// text rectangle and the client rectangle of the cell without borders and cell buttons. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValidateValue"/> (<see cref="GridCellValidateValueInfo"/>)</term>
    ///         <description>Holds validation rules for the cell values that are being checked before any user changes are committed to the grid cell's style object. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalAlignment"/> (<see cref="GridVerticalAlignment"/>)</term>
    ///         <description>Specifies vertical alignment of text in the cell. (Default: GridVerticalAlignment.Top)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: true)</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public class GridStaticCellRenderer : GridCellRendererBase
    {
        /// <summary>
        /// Initializes a new GridStaticCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridStaticCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.SupportsEditing = GetType() != typeof(GridStaticCellRenderer);
        }

        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            bool drawDisabled = false;
            string displayText = String.Empty;

            if (clientRectangle.IsEmpty)
            {
                return;
            }

            int imageIndex = style.ImageIndex;
            if (imageIndex != -1)
            {
                ImageList imageList = style.ImageList;
                if (imageList != null && imageIndex < imageList.Images.Count)
                {
                    // TODO: Draw in client or cell boundaries.
                    // Using cellbounds core has advantage that image won't shift around when
                    // we draw a border.
                    Rectangle imageRectangle = this.GetCellBoundsCoreInt(rowIndex, colIndex, true);
                    Rectangle clipRectangle = this.GetCellBoundsCoreInt(rowIndex, colIndex, false);
                    if (!g.ClipBounds.IsEmpty)
                    {
                        clipRectangle.Intersect(Rectangle.Ceiling(g.ClipBounds));
                    }

                    if (clipRectangle.Contains(imageRectangle))
                    {
                        DrawImage(g, imageList, imageIndex, imageRectangle, Grid.IsRightToLeft());
                    }
                    else
                    {
                        if (clipRectangle.IntersectsWith(imageRectangle))
                        {
                            DrawImage(g, imageList, imageIndex, imageRectangle, clipRectangle, Grid.IsRightToLeft());
                        }
                    }
                }
            }

            Rectangle textRectangle = this.RemoveMargins(clientRectangle, style);
            if (textRectangle.IsEmpty)
            {
                return;
            }

            bool handled = false;

            if (style.ImageFromByteArray && style.CellValueType == typeof(byte[]))
            {
                try
                {
                    // Image cells.
                    object cellValue = style.CellValue;
                    Image image = ImageUtil.ConvertToImage(cellValue);
                    if (image != null)
                    {
                        Rectangle clipRectangle = this.GetCellBoundsCoreInt(rowIndex, colIndex, false);
                        bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
                        GridImageUtil.DrawImage(image, clipRectangle, g, clientRectangle, style, isTextRightToLeft);
                        handled = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Image conversion from ByteArray failed. Set GridStyleInfo.ImageFromByteArray = false; to prevent this exception.");
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }

            if (!handled)
            {
                try
                {
                    if (this.ShouldDrawEditing(rowIndex, colIndex))
                    {
                        displayText = this.ControlText;
                    }
                    else
                    {
                        displayText = Model.GetFormattedOrActiveTextAt(rowIndex, colIndex, style);
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    displayText = style.Text;
                    ////style.ToolTip = ex.Message;
                    drawDisabled = true;
                }

                if (style.HasError)
                {
                    displayText = style.Error;
                    drawDisabled = true;
                }

                if (displayText.Length > 0)
                {
                    GridDrawCellDisplayTextEventArgs e = new GridDrawCellDisplayTextEventArgs(g, displayText, textRectangle, style);
                    Grid.RaiseDrawCellDisplayText(e);
                    if (!e.Cancel)
                    {
                        textRectangle = e.TextRectangle;
                        displayText = e.DisplayText;
                        Font font = style.GdipFont;
                        Color textColor = Grid.PrintingMode && Grid.Model.Properties.BlackWhite ? Color.Black : style.TextColor;
                        bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
                        DrawText(g, displayText, font, textRectangle, style, textColor, drawDisabled, isTextRightToLeft);
                    }
                }
            }
        }
        /// <summary>
        /// Allows single cell copy for static cells..
        /// </summary>
        /// <returns></returns>
        public override bool CanCopy()
        {
            return true;
        }
        /// <summary>
        /// Allows single cell cut for static cells..
        /// </summary>
        /// <returns></returns>
        public override bool CanCut()
        {
            return true;
        }
        /// <summary>
        /// Removes TextMargins from the specified client bounds.
        /// </summary>
        /// <param name="clientRectangle">The Rectangle</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>
        /// A <see cref="System.Drawing.Rectangle"/> with the bounds.
        /// </returns>
        /// <override/>
        protected override Rectangle RemoveMargins(Rectangle clientRectangle, GridStyleInfo style)
        {
            GridMargins margins = style.ReadOnlyTextMargins.ToMargins();
            if (Grid.IsRightToLeft())
            {
                margins = margins.SwapRightToLeft();
            }

            Rectangle textRectangle = GridMargins.RemoveMargins(clientRectangle, margins);

            int imageIndex = style.ImageIndex;
            ImageList imageList = style.ImageList;
            if (imageIndex != -1 && imageList != null && imageIndex < imageList.Images.Count)
            {
                if (Grid.IsRightToLeft())
                {
                    textRectangle.Width -= imageList.ImageSize.Width + 2;
                }
                else
                {
                    GridUtil.OffsetLeft(ref textRectangle, imageList.ImageSize.Width + 2);
                }
            }

            if (textRectangle.Width <= 0 || textRectangle.Height <= 0)
            {
                return Rectangle.Empty;
            }

            return textRectangle;
        }

        /// <overload>
        /// Draws an image at a given position in an ImageList onto the <see cref="Graphics"/> canvas at a specified <see cref="Rectangle"/>.
        /// </overload>
        /// <summary>
        /// Draws an image at a given position in an ImageList onto the <see cref="Graphics"/> canvas at a specified <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="imageList">The image list.</param>      
        /// <param name="imageIndex">The image index in the image list.</param>
        /// <param name="bounds">The target rectangle where the image should be drawn.</param>
        /// <param name="isRightToLeft">Indicates if grid is in RightToLeft mode.</param>
        /// <returns>The bounds of the rectangle. Might differ if it was vertically centered.</returns>
        public static Rectangle DrawImage(Graphics g, ImageList imageList, int imageIndex, Rectangle bounds, bool isRightToLeft)
        {
            return DrawImage(g, imageList, imageIndex, bounds, Rectangle.Empty, isRightToLeft);
        }

        /// <summary>
        /// Draws an image at a given position in an ImageList onto the <see cref="Graphics"/> canvas at a specified <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="imageList">The image list.</param>       
        /// <param name="imageIndex">The image index in the image list.</param>
        /// <param name="bounds">The target rectangle where the image should be drawn.</param>
        /// <param name="clipBounds">The target rectangle where the image should be clipped.</param>
        /// <param name="isRightToLeft">Indicates if grid is in RightToLeft mode.</param>
        /// <returns>The bounds of the rectangle. Might differ if it was vertically centered.</returns>
        public static Rectangle DrawImage(Graphics g, ImageList imageList, int imageIndex, Rectangle bounds, Rectangle clipBounds, bool isRightToLeft)
        {
            Region originalClip = null;

            Size size = imageList.ImageSize;
            int dy = (bounds.Height - size.Height) / 2;
            try
            {
                if (dy <= 0)
                {
                    dy = 0;
                }

                Rectangle r;
                if (isRightToLeft)
                {
                    r = new Rectangle(Math.Max(bounds.Left, bounds.Right - size.Width), bounds.Y + dy, size.Width, Math.Min(bounds.Height, size.Height));
                }
                else
                {
                    r = new Rectangle(bounds.X, bounds.Y + dy, Math.Min(bounds.Width, size.Width), Math.Min(bounds.Height, size.Height));
                }

                if (clipBounds.IsEmpty || clipBounds.IntersectsWith(r))
                {
                    if (dy <= 0)
                    {
                        dy = 0;
                        originalClip = g.Clip;
                        ////                        if (!clipBounds.IsEmpty)
                        ////                            g.IntersectClip(clipBounds);
                        ////                        g.IntersectClip(bounds);
                    }
                    else if (!clipBounds.IsEmpty)
                    {
                        originalClip = g.Clip;
                        ////g.IntersectClip(clipBounds);
                    }

                    //// TODO: Is there a better way to find out if we are printing?
                    //// || originalClip != null) 
                    if (g.DpiY > 96 || GridControlBase.UseImageListDrawing) 
                    {
                        Rectangle rectangle = r;
                        rectangle.Offset(-rectangle.X, -rectangle.Y);
                        Bitmap bm = new Bitmap(bounds.Width, bounds.Height);
                        Graphics bmg = Graphics.FromImage(bm);
                        imageList.Draw(bmg, 0, 0, imageIndex);
                        bmg.Dispose();
                        g.DrawImageUnscaled(bm, r);

                        /* Possible fix for incident 13765 - horizontal pixel scrolling, problem was somewhere else
                         * but I leave code here just in case I need to revisit the problem again.
                         *
                         if (bounds.X < clipBounds.X)
                        {
                            deltax = clipBounds.X - bounds.X;
                        }
                        g.IntersectClip(clipBounds);

                        Rectangle rectangle = r;
                        rectangle.Offset(-rectangle.X, -rectangle.Y);
                        Bitmap bm = new Bitmap(bounds.Width-deltax, bounds.Height);
                        Graphics bmg = Graphics.FromImage(bm);
                        imageList.Draw(bmg, -deltax, 0, imageIndex);
                        bmg.Dispose();
                        rectangle = r;
                        rectangle.Offset(deltax, 0);
                        g.DrawImageUnscaled(bm, r);
                        */
                    }
                    else if (originalClip != null && !clipBounds.IsEmpty)
                    {
                        // This method will use ImageList_DrawEx to draw the image (to use the transparency info in the embedded images).
                        // We use the PInvoke rather than ImageList.Draw because, the Draw method
                        // uses the PaintEventArgs.ClipRectangle rather than g.ClipBounds (both
                        // can be different) and here we force the ClipBounds on the DC.
                        DrawingUtils.DrawImageViaImageList(g, imageList, imageIndex, r);
                    }
                    else
                    {
                        DrawingUtils.DrawImageViaImageList(g, imageList, imageIndex, r);
                        ////imageList.Draw(g, r.X, r.Y, r.Width, r.Height, imageIndex);
                    }
                }

                if (isRightToLeft)
                {
                    bounds.Width -= size.Width + 2;
                }
                else
                {
                    GridUtil.OffsetLeft(ref bounds, size.Width + 2);
                }
            }
            finally
            {
                ////                if (originalClip != null)
                ////                    g.Clip = originalClip;
            }

            return bounds;
        }

        /// <summary>
        /// Draws an image at a given position in an ImageList onto the <see cref="Graphics"/> canvas at a specified <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="imageList">The image list.</param>
        /// <param name="imageIndex">The image index in the image list.</param>
        /// <param name="bounds">The target rectangle where the image should be drawn.</param>
        /// <returns>The bounds of the rectangle. Might differ if it was vertically centered.</returns>
        [Obsolete("It is recommended to specify isRightToLeft parameter, default for isRightToLeft is False.")]
        public static Rectangle DrawImage(Graphics g, ImageList imageList, int imageIndex, Rectangle bounds)
        {
            return DrawImage(g, imageList, imageIndex, bounds, false);
        }

        static Rectangle GetImageRect(ImageList imageList, Rectangle bounds, bool isRightToLeft)
        {
            Rectangle r = Rectangle.Empty;
            if (imageList != null)
            {
                Size size = imageList.ImageSize;
                int dy = (bounds.Height - size.Height) / 2;
                if (dy <= 0)
                {
                    dy = 0;
                }

                if (isRightToLeft)
                {
                    r = new Rectangle(Math.Max(bounds.Left, bounds.Right - size.Width), bounds.Y + dy, size.Width, Math.Min(bounds.Height, size.Height));
                }
                else
                {
                    r = new Rectangle(bounds.X, bounds.Y + dy, Math.Min(bounds.Width, size.Width), Math.Min(bounds.Height, size.Height));
                }
            }

            return r;
        }

        /// <overload>
        /// Draws a text at a given position onto the <see cref="Graphics"/> canvas at a specified <see cref="Rectangle"/>.
        /// </overload>
        /// <summary>
        /// Draws a text at a given position onto the <see cref="Graphics"/> canvas at a specified <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="g">Points to the device context.</param>       
        /// <param name="displayText">The display text.</param>
        /// <param name="font">The font value.</param>
        /// <param name="textRectangle">The text rectangle.</param>
        /// <param name="style">Cell information with alignment, trimming, and HotkeyPrefix information.</param>
        /// <param name="textColor">The text color.</param>
        /// <param name="isRightToLeft">Indicates if grid is in RightToLeft mode.</param>
        public static void DrawText(Graphics g, string displayText, Font font, Rectangle textRectangle, GridStyleInfo style, Color textColor, bool isRightToLeft)
        {
            DrawText(g, displayText, font, textRectangle, style, textColor, false, isRightToLeft);
        }

        /// <summary>
        /// Draws a text at a given position onto the <see cref="Graphics"/> canvas at a specified <see cref="Rectangle"/> and
        /// draws text optionally disabled.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="font">The font value.</param>
        /// <param name="textRectangle">The text rectangle.</param>      
        /// <param name="style">Cell information with alignment, trimming, and HotkeyPrefix information.</param>
        /// <param name="textColor">The text color.</param>
        /// <param name="drawDisabled">True if text should be drawn disabled.</param>
        /// <param name="isRightToLeft">Indicates if grid is in RightToLeft mode.</param>
        public static void DrawText(Graphics g, string displayText, Font font, Rectangle textRectangle, GridStyleInfo style, Color textColor, bool drawDisabled, bool isRightToLeft)
        {
            Brush br = new SolidBrush(textColor);
            StringFormat format = new StringFormat();
            format.LineAlignment = GridUtil.ConvertToStringAlignment(style.VerticalAlignment);
            format.Alignment = GridUtil.ConvertToStringAlignment(style.HorizontalAlignment);
            if (isRightToLeft)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }
            int newlocation = 0;
            if (font.Size < 8.5F && font.Size > 6F)
            {
                newlocation = 8 - (int)font.Size;
            }
            textRectangle = new Rectangle(textRectangle.X, textRectangle.Y - newlocation, textRectangle.Width, textRectangle.Height + newlocation);
            format.HotkeyPrefix = style.HotkeyPrefix;
            format.Trimming = style.Trimming;
            format.SetTabStops(0f, new float[] { 50 });

            // Note: Trimming will only work if style.WrapText = True, FloatCell = false.
            if (!style.WrapText)
            {
                format.FormatFlags |= StringFormatFlags.NoWrap;
            }

            char pwc = style.PasswordChar;
            if (pwc != ' ')
            {
                displayText = new string(pwc, displayText.Length);
            }

            int orientation = style.ReadOnlyFont.Orientation;

            if (orientation != 0)
            {
                float angle = (float)orientation;

                // Let GDI+ do text rotation.
                if (angle == 90 || angle == 180 || angle == 270 || angle == 360)
                {
                    format = GridUtil.ConvertToRotateStringAlignment(format,style.HorizontalAlignment, style.VerticalAlignment);
                    RotatePaint.DrawRotatedString(g, displayText, font, br, textRectangle, format, angle);
                }
                else
                {
                    DrawRotatedString(g, displayText, font, br, textRectangle, format, angle,style);
                }
            }
            else
            {
                if (format.Alignment == StringAlignment.Far && GridControlBase.UseGdiPlusRightAlignedTextWorkaround)
                {
                    if (g.TextRenderingHint == System.Drawing.Text.TextRenderingHint.SystemDefault)
                    {
                        int stringWidth = MeasureDisplayStringWidth(g, displayText, font);
                        ////if (stringWidth < textRectangle.Width)
                        {
                            stringWidth = Math.Min(stringWidth, textRectangle.Width);
                            textRectangle = new Rectangle(textRectangle.Right - stringWidth, textRectangle.Y, stringWidth, textRectangle.Height);
                            format.Alignment = StringAlignment.Near;
                            format.Trimming = StringTrimming.None;
                            format.FormatFlags = StringFormatFlags.NoWrap;
                        }
                    }
                }

                // SD 2846: Condition added to avoid GDI+ error when text length exceeds 65536 characters
                if (displayText.Length <= UInt16.MaxValue)
                {
                    if (!drawDisabled)
                    {
                        g.DrawString(displayText, font, br, textRectangle, format);
                    }
                    else
                    {
                        ControlPaint.DrawStringDisabled(g, displayText, font, textColor, textRectangle, format);
                    }
                }
            }

            br.Dispose();
            format.Dispose();
        }
        /// <summary>
        /// Measures the Height and width of the text.
        /// </summary>
        /// <param name="text">The Input Text</param>
        /// <param name="font">Font style of the Text</param>
        /// <param name="format">Alignemnt Settings of the text</param>
        /// <param name="rotation">The Angle of Rotation of the Text</param>
        /// <param name="dpiY">Vertical Resolution of the Text</param>
        /// <returns></returns>
        public static RectangleF GetRotatedTextBound(string text, Font font, StringFormat format, float rotation, float dpiY)
        {
            GraphicsPath gp = new GraphicsPath();

            float emSize = dpiY * font.Size / 72; // Default final font size is considerd as 72.

            gp.AddString(text, font.FontFamily, (int)font.Style, emSize, new PointF(0, 0), format);

            Matrix mat = new Matrix();
            mat.Rotate(rotation, MatrixOrder.Append);

            gp.Transform(mat);

            return gp.GetBounds();
        }


        /// <summary>
        /// Draws the text at the specified angle.
        /// </summary>
        /// <param name="g">Graphical Parameter</param>
        /// <param name="text">THe text to be rotated</param>
        /// <param name="font">The font Style of the text</param>
        /// <param name="br">The brush used to draw the text</param>
        /// <param name="rect">The text rectangle</param>
        /// <param name="format">the format that holds different alignement settings</param>
        /// <param name="angle">The angle at which the text is rotated</param>

        public static void DrawRotatedString(Graphics g, string text, Font font, Brush br, RectangleF rect, StringFormat format, float angle, GridStyleInfo style)
        {
            float normAngle = angle;
            while (normAngle < 0)
                normAngle += 360.0f;

            while (normAngle >= 360.0)
                normAngle -= 360.0f;

            while (normAngle > 90 && normAngle <= 360)
                normAngle = normAngle - 360;

            Region oldClip = g.Clip;
            g.IntersectClip(rect);

            RectangleF txBounds = GetRotatedTextBound(text, font, format, normAngle, g.DpiY);

            SizeF size = g.MeasureString(text, font);
            PointF origin = new PointF();
            RectangleF rcOutside = new RectangleF();

            if (format.Alignment == StringAlignment.Near)
            {
                switch (format.LineAlignment)
                {
                    case StringAlignment.Near:
                        if (normAngle > 0)
                        {
                            origin = new PointF(rect.X, rect.Y + txBounds.Height);
                            if (style.WrapRotatedText)
                            {
                                if (normAngle >= 70)
                                {
                                    rcOutside = new RectangleF(100, 0, rect.Height, rect.Height);
                                }

                                else if (normAngle >= 50)
                                {
                                    rcOutside = new RectangleF(90, -50, rect.Height, rect.Height);
                                }
                                else
                                {
                                    rcOutside = new RectangleF(40, -80, rect.Height, rect.Height);
                                }
                            }
                        }
                        else
                        {
                            origin = new PointF(rect.X, rect.Y);
                        }
                        break;

                    case StringAlignment.Center:
                        if (normAngle > 0)
                        {
                            origin = new PointF(rect.X, (rect.Y + rect.Height / 2) + txBounds.Height / 2);
                            if (style.WrapRotatedText)
                                rcOutside = new RectangleF(0, 0, rect.Height, rect.Height);
                        }
                        else
                        {
                            origin = new PointF(rect.X, (rect.Y + rect.Height / 2) - txBounds.Height / 2);
                        }
                        break;
                    case StringAlignment.Far:
                        if (normAngle > 0)
                        {
                            if (normAngle <= 40)
                            {
                                origin = new PointF(rect.X, rect.Y + rect.Height - 10);
                                if (style.WrapRotatedText)
                                    rcOutside = new RectangleF(10, -15, rect.Height, rect.Height);
                            }
                            else
                            {
                                origin = new PointF(rect.X, rect.Y + rect.Height - 5);

                                if (style.WrapRotatedText)
                                {
                                    if (normAngle >= 70)
                                    {
                                        rcOutside = new RectangleF(0, 0, rect.Height, rect.Height);
                                    }

                                    else if (normAngle < 50)
                                    {
                                        rcOutside = new RectangleF(10, -15, rect.Height, rect.Height);
                                    }
                                    else
                                    {
                                        rcOutside = new RectangleF(15, -15, rect.Height, rect.Height);
                                    }
                                }
                            }
                        }
                        else
                        {
                            origin = new PointF(rect.X, rect.Y + rect.Height - txBounds.Height - 4);
                        }
                        break;
                }
            }

            if (format.Alignment == StringAlignment.Center)
            {
                switch (format.LineAlignment)
                {
                    case StringAlignment.Near:
                        if (normAngle > 0)
                        {
                            origin = new PointF(rect.X + (rect.Width / 2) - txBounds.Width / 2, rect.Y + txBounds.Height);
                            if (style.WrapRotatedText)
                            {
                                if (normAngle >= 70)
                                {
                                    rcOutside = new RectangleF(100, 0, rect.Height, rect.Height);
                                }

                                else if (normAngle >= 50)
                                {
                                    rcOutside = new RectangleF(90, -50, rect.Height, rect.Height);
                                }
                                else
                                {
                                    rcOutside = new RectangleF(40, -40, rect.Height, rect.Height);
                                }
                            }
                        }
                        else
                        {
                            if (normAngle <= -80)
                                origin = new PointF((rect.X + rect.Width / 2) - 5, rect.Y);
                            else
                                origin = new PointF((rect.X + rect.Width / 2) - (txBounds.Width / 2) - 5, rect.Y);
                        }
                        break;

                    case StringAlignment.Center:
                        if (normAngle > 0)
                        {
                            origin = new PointF(((rect.X + rect.Width / 2) - txBounds.Width / 2) - 5, ((rect.Y + rect.Height / 2) + txBounds.Height / 2) - 5);
                            if (style.WrapRotatedText)
                                rcOutside = new RectangleF(0, 0, rect.Height, rect.Height);
                        }
                        else
                            if (normAngle <= -80)
                                origin = new PointF((rect.X + rect.Width / 2) - 5, (rect.Y + rect.Height / 2) - txBounds.Height / 2);
                            else
                                origin = new PointF(((rect.X + rect.Width / 2) - txBounds.Width / 2) - 5, (rect.Y + rect.Height / 2) - (txBounds.Height / 2) - 5);
                        break;

                    case StringAlignment.Far:
                        if (normAngle > 0)
                        {
                            if (normAngle <= 40)
                            {
                                if (style.WrapRotatedText)
                                    rcOutside = new RectangleF(40, 20, rect.Height, rect.Height);
                                origin = new PointF((rect.X + rect.Width / 2) - txBounds.Width / 2, rect.Y + rect.Height - 10);
                            }
                            else
                            {
                                if (style.WrapRotatedText)
                                {
                                    if (normAngle >= 70)
                                    {
                                        rcOutside = new RectangleF(0, 0, rect.Height, rect.Height);
                                    }
                                    else if (normAngle >= 40)
                                        rcOutside = new RectangleF(20, 20, rect.Height, rect.Height);
                                    else
                                        rcOutside = new RectangleF(20, 20, rect.Height, rect.Height);
                                }
                                origin = new PointF((rect.X + rect.Width / 2) - txBounds.Width / 2, rect.Y + rect.Height - 5);
                            }
                        }
                        else
                        {
                            if (normAngle <= -80)
                                origin = new PointF((rect.X + rect.Width / 2) - 5, rect.Y + rect.Height - txBounds.Height);
                            else
                                origin = new PointF((rect.X + rect.Width / 2) - (txBounds.Width / 2) - 5, rect.Y + rect.Height - txBounds.Height - 4);
                        }
                        break;
                }
            }


            if (format.Alignment == StringAlignment.Far)
            {
                switch (format.LineAlignment)
                {
                    case StringAlignment.Near:
                        if (normAngle > 0)
                        {
                            origin = new PointF((rect.X + rect.Width) - txBounds.Width - 7, rect.Y + txBounds.Height);
                            if (style.WrapRotatedText)
                                rcOutside = new RectangleF(100, -20, rect.Height, rect.Height);
                        }
                        else
                        {
                            origin = new PointF((rect.X + rect.Width) - txBounds.Width - 12, rect.Y);
                        }
                        break;

                    case StringAlignment.Center:
                        if (normAngle > 0)
                        {
                            origin = new PointF((rect.X + rect.Width) - txBounds.Width - 7, rect.Y + (rect.Height / 2) + (txBounds.Height / 2));
                            if (style.WrapRotatedText)
                                rcOutside = new RectangleF(10, -20, rect.Height, rect.Height);
                        }
                        else
                        {
                            if (normAngle <= -80)
                                origin = new PointF((rect.X + rect.Width) - (txBounds.Width / 2) - 10, (rect.Y + rect.Height / 2) - txBounds.Height / 2);
                            else
                                origin = new PointF((rect.X + rect.Width) - txBounds.Width - 12, (rect.Y + rect.Height / 2) - (txBounds.Height / 2) - 5);
                        }
                        break;

                    case StringAlignment.Far:
                        if (normAngle > 0)
                        {
                            if (normAngle <= 40)
                            {
                                origin = new PointF((rect.X + rect.Width) - txBounds.Width - 5, rect.Y + rect.Height - 10);
                                if (style.WrapRotatedText)
                                    rcOutside = new RectangleF(20, 15, rect.Height, rect.Height);
                            }
                            else
                            {
                                origin = new PointF((rect.X + rect.Width) - txBounds.Width - 7, rect.Y + rect.Height - 7);
                                if (style.WrapRotatedText)
                                {
                                    if (normAngle >= 70)
                                        rcOutside = new RectangleF(0, -20, rect.Height, rect.Height);
                                    else
                                        rcOutside = new RectangleF(20, 20, rect.Height, rect.Height);
                                }
                            }
                        }
                        else
                            if (normAngle <= -80)
                                origin = new PointF((rect.X + rect.Width) - (txBounds.Width / 2) - 10, rect.Y + rect.Height - txBounds.Height);
                            else
                                origin = new PointF((rect.X + rect.Width) - txBounds.Width - 12, rect.Y + rect.Height - txBounds.Height - 5);
                        break;
                }
            }

            if (normAngle > 0)
                g.TranslateTransform(origin.X, origin.Y);
            else
                g.TranslateTransform(origin.X + 15, origin.Y);
            g.RotateTransform(-normAngle);
            if (style.WrapRotatedText)
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Near;
                rcOutside.Offset(0, 0);
                g.DrawString(text, font, br, rcOutside, format);
            }
            else
            {
                g.DrawString(text, font, br, PointF.Empty);
            }

            g.ResetTransform();
            g.Clip = oldClip;
        }

        /// <summary>
        /// Draws a text at a given position onto the <see cref="Graphics"/> canvas at a specified <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="font">The font value.</param>
        /// <param name="textRectangle">The text rectangle.</param>
        /// <param name="style">Cell information with alignment, trimming, and HotkeyPrefix information.</param>
        /// <param name="textColor">The text color.</param>
        [Obsolete("It is recommended to specify isRightToLeft parameter, default for isRightToLeft is False.")]
        public static void DrawText(Graphics g, string displayText, Font font, Rectangle textRectangle, GridStyleInfo style, Color textColor)
        {
            DrawText(g, displayText, font, textRectangle, style, textColor, false, false);
        }

        /// <summary>
        /// UseGdiPlusRightAlignedTextWorkaround implements a work-around for a GDI+ known issue with
        /// right-aligned text and DrawString.
        /// </summary>
        /// <remarks>
        /// When you have cells with right-aligned text, some words are not aligned at the right border as
        /// you would expect. This is because of GDI+ designed behavior, and the degree of the problem varies
        /// from font to font. You can see this behavior for example with regular System.Windows.Forms.Label
        /// controls. Since Essential Grid relies on GDI+, it exhibits the same behavior. Besides using a
        /// Monospaced Font, some work around include using either antialiased string drawing or
        /// explicitly measuring the string width and not relying on DrawString to draw the text right-aligned.
        /// <para/>
        /// When you enable UseGdiPlusRightAlignedTextWorkaround, the static cell renderer will
        /// use the measure string width workaround. However, this will slow down drawing of right-aligned text.
        /// </remarks>
        /// <returns>returns Measure Display String Width</returns>
        static internal int MeasureDisplayStringWidth(Graphics graphics, string text, Font font)
        {
            RectangleF rect = RectangleF.Empty;

            using (StringFormat format = new System.Drawing.StringFormat())
            {
                rect = new System.Drawing.RectangleF(0, 0, 1000, 1000);
                CharacterRange[] ranges = { new System.Drawing.CharacterRange(0, text.Length) };
                Region[] regions = new System.Drawing.Region[1];
                format.SetMeasurableCharacterRanges(ranges);
                regions = graphics.MeasureCharacterRanges(text, font, rect, format);
                rect = regions[0].GetBounds(graphics);
            }

            return (int)(rect.Right + 1.0f);
        }

        /// <summary>
        /// Checks if the specified point is over an image (see <see cref="GridStyleInfo.ImageIndex"/>) .
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="pt">The point to be tested.</param>
        /// <returns>True if inside image; False otherwise.</returns>
        /// <override/>
        protected override bool IsPointOverImage(int rowIndex, int colIndex, Point pt)
        {
            GridStyleInfo style = Grid.GetViewStyleInfo(rowIndex, colIndex);
            ImageList imageList = style.ImageList;
            if (imageList != null)
            {
                int index = style.ImageIndex;
                if (index >= 0 && index < imageList.Images.Count)
                {
                    GridCellLayout cellLayout = PerformLayout(rowIndex, colIndex);
                    Rectangle r = GetImageRect(imageList, cellLayout.InnerRectangle, Grid.IsRightToLeft());
                    return r.Contains(pt);
                }
            }

            return false;
        }
    }
}
