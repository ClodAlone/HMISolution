#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;HR&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Hr)]
    public class HRElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Hr;

        /// <summary>
        /// Height of the element by default.
        /// </summary>
        private const int DEF_HEIGHT = 2;

        /// <summary>
        /// Space between the element end block which it contains.
        /// </summary>
        private const int DEF_INCREASOR = 5;

        /// <summary>
        /// Indent from left and right for drawing.
        /// </summary>
        private const int DEF_INDENT = 5;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        ///  Holds all events.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class Properties
        /// <summary>
        /// Overridden. Returns the array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }

        /// <summary>
        /// Overridden. Gets or sets the format which defines special visualization for this element.
        /// </summary>
        protected internal override HTMLFormat OwnFormat
        {
            get
            {
                if (m_ownFormat == null)
                {
                    m_ownFormat = base.OwnFormat;
                    m_ownFormat.Merge = MergeMask.BgColor;
                    m_ownFormat.BackgroundColor = Color.DarkGray;
                }

                return m_ownFormat;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the HRElementImpl class 
        /// </summary>
        static HRElementImpl()
        {
            Type type = typeof(HRElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the HRElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public HRElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns an instance of the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Event object.</returns>
        protected override IHTMLEvent CreateEventInternal(string name)
        {
            return new HashElementEvents(this, m_eventHash, name);
        }

        /// <summary>
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size object</returns>
        protected override Size CalculateSizeInternal()
        {
            this.Size = CalculateOwnSize();

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            BaseElement parent = (BaseElement)this.Parent;
            this.CurrentPosition = parent.CurrentPosition;

            CalculateChildPositions(this.CurrentPosition, parent.Bounds);
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            this.Type = ElementType.BlockNewLineResizable;

            DefaultCalculateFormatInternal();

            if (this.Parent != null)
                this.Width = this.Parent.Size.Width;

            this.Height = DEF_HEIGHT;
        }

        /// <summary>
        /// Overridden. Calculates its position and size.
        /// </summary>
        /// <param name="curPosition">Global current position.</param>
        /// <param name="bounds">Bounds for this element.</param>
        /// <returns>Array of blocks for this element.</returns>
        protected override BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            m_blocks.Clear();
            m_curBlock = null;

            this.QuietMode = true;
            this.CurrentPosition = curPosition;
            this.Bounds = bounds;
            this.Width = GetWidth();

            if (!this.IsVisible)
            {
                this.QuietMode = false;

                return new BlocksCollection();
            }

            m_curPos = ShiftStartPos(m_curPos);

            // Add line block.
            CreateBlock(null);
            this.Location = this.CurrentPosition;
            Rectangle rect = new Rectangle(this.CurrentPosition, this.Size);
            AddElement(m_curBlock, this, rect);
            m_curBlock.DrawProperties = false;

            if (this.MainBlock != null)
            {
                this.MainBlock.DrawProperties = false;
            }

            this.QuietMode = false;
            OnLocationCalculated(EventArgs.Empty);

            return m_blocks;
        }

        /// <summary>
        /// Overridden. Draws an element on the control.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void DrawElementInternal(PaintEventArgs e)
        {
            GDIUtils gdi = new GDIUtils();
            Rectangle elRect = new Rectangle(this.Location, this.Size);

            // Draw rectangle.
            if (!this.Attributes.Contains(AttributeName.Noshade) &&
                !this.Attributes.Contains(AttributeName.Color))
            {
                gdi.Draw3DBox(e.Graphics, GlobalToClient(elRect), Canvas3DStyle.RaisedSimple);
            }
            else 
            {
                // Fill rectangle.
                Color color = GetColor();
                SolidBrush brush = new SolidBrush(color);
                e.Graphics.FillRectangle(brush, GlobalToClient(elRect));

                brush.Dispose();
                brush = null;
            }

            this.MainBlock.DrawProperties = true;
            DrawBorders(this.MainBlock, e.Graphics);
            this.MainBlock.DrawProperties = false;
        }

        /// <summary>
        /// Overridden. Moves the current position of the parent element down after this element.
        /// </summary>
        /// <param name="currentPos">Current global position.</param>
        protected override void MoveFinalCurPos(ref Point currentPos)
        {
            base.MoveFinalCurPos(ref currentPos);

            BaseElement parent = (BaseElement)this.Parent;
            parent.MakeBottomIndent(DEF_INCREASOR);
        }

        /// <summary>
        /// Overridden. Moves the current position of the parent element down before.
        /// </summary>
        /// <param name="currentPos">>Current global position.</param>
        protected override void MoveStartCurPos(ref Point currentPos)
        {
            base.MoveFinalCurPos(ref currentPos);

            BaseElement parent = (BaseElement)this.Parent;
            parent.MakeTopIndent(DEF_INCREASOR);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Calculates the size of the current element by its size attribute or style attribute.
        /// </summary>
        /// <returns>Size object</returns>
        private Size CalculateOwnSize()
        {
            Size result = new Size(0, DEF_HEIGHT);

            // Detect if attribute size exists in element.
            if (this.Attributes.Contains(AttributeName.Size))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.Size];
                AttributeToken type = AttributeParser.DetectType(attr.Value);
                if (type == AttributeToken.Integer)
                {
                    if (attr != null)
                    {
                        result.Height = AttributeParser.GetInteger(attr.Value);
                    }
                }
            }

            // Detect if element's format has width and height not default properties.
            // We must override previous value.
            if (IsStyleHeight)
            {
                result.Height = this.Format.Height;
                this.Type = ElementType.BlockNewLineFixedSize;
            }
            if (IsStyleWidth)
            {
                result.Width = this.Format.Width;
                this.Type = ElementType.BlockNewLineFixedSize;
            }

            return result;
        }

        /// <summary>
        /// Shifts the current position to a new line.
        /// </summary>
        private void ShiftToNewLine()
        {
            // Postpone to new line.
            m_curPos.Y += GetIncreaseValue();
            m_curPos.X = this.Bounds.X;
        }

        /// <summary>
        /// Adds empty spaces between lines and other elements.
        /// </summary>
        /// <param name="holder">Block Instance</param>
        /// <param name="location">Point instance</param>
        private void AddEmptySpace(Block holder, Point location)
        {
            Rectangle rect = new Rectangle(location.X, location.Y, this.Width, DEF_INCREASOR);
            AddElement(holder, this, rect);
        }

        /// <summary>
        /// Returns the color of the element.
        /// </summary>
        /// <returns>Color of the element.</returns>
        private Color GetColor()
        {
            if (this.Format.ForeColor != this.Control.DefaultFormat.ForeColor)
            {
                return this.Format.ForeColor;
            }

            if (this.Attributes.Contains(AttributeName.Color))
            {
                HTMLAttributeImpl attr = (HTMLAttributeImpl)this.Attributes[AttributeName.Color];
                string colorVal = attr.Value;

                AttributeToken type = AttributeParser.DetectType(colorVal);
                if (
                  type == AttributeToken.Color_Hex ||
                  type == AttributeToken.Color_rgb ||
                  type == AttributeToken.Color_Word
                  )
                {
                    return AttributeParser.GetColor(colorVal);
                }
            }

            return this.Format.BackgroundColor;
        }

        /// <summary>
        /// Returns the width of the element.
        /// </summary>
        /// <returns>Width of the element.</returns>
        private int GetWidth()
        {
            if (!((HTMLFormat)this.Format).IsWidthDefault) return this.Format.Width;

            return GetWidthFromParent();
        }
        #endregion
    }
}