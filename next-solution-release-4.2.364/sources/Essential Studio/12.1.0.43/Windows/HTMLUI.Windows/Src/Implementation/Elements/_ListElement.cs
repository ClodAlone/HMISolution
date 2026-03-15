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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Base class for list element holders like UL, OL elements.
    /// </summary>
    public abstract class ListElement : BaseElement
    {
        #region Class constants

        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Ul;

        /// <summary>
        /// Square marker for item drawing.
        /// </summary>
        private const string DEF_MARKER_SQUARE = "\u25a0 ";

        /// <summary>
        /// Circle marker for item drawing.
        /// </summary>
        private const string DEF_MARKER_CIRCLE = "\u25cb ";

        /// <summary>
        /// Disc marker for item drawing.
        /// </summary>
        private const string DEF_MARKER_DISC = "\u25cf ";

        /// <summary>
        /// Name of the parameter in xslt template.
        /// </summary>
        private const string DEF_XSLT_PARAM_NAME = "format";

        /// <summary>
        /// Name of the bullet font.
        /// </summary>
        private const string DEF_BULLET_FONT_NAME = "Times New Roman";

        /// <summary>
        /// Amount of space added to the width of the marker for each item.
        /// </summary>
        private const int DEF_MARKER_INDENT = 10;

        #endregion

        #region Class static members
        /// <summary>
        /// Holds the type of reaction on attribute changing.
        /// </summary>
        private static ReactionCollection m_reactionType;

        /// <summary>
        /// Supported events.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected static string[] DEF_SUPP_EVENTS;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        protected static Hashtable m_eventHash;

        /// <summary>
        /// Format for unordered marker drawing.
        /// </summary>
        private static StringFormat m_unorderedFormat;

        /// <summary>
        /// Format for ordered marker drawing.
        /// </summary>
        private static StringFormat m_orderedFormat;

        #endregion

        #region Class members
        /// <summary>
        /// Font in unordered marker item style.
        /// </summary>
        private Font m_bulletFont;

        /// <summary>
        /// Indent from the left of the element. List has space from the left.
        /// </summary>
        internal int m_indent = 10;

        #endregion

        #region Class Properties

        /// <summary>
        /// Returns an array of the supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }

        /// <summary>
        /// Gets the default item style in the list.
        /// </summary>
        internal virtual ListItemType DefaultStyle
        {
            get
            {
                return ListItemType._1;
            }
        }

        /// <summary>
        /// Returns the hashtable of names of attributes as keys and type of reaction
        /// changes as value.
        /// </summary>
        internal override ReactionCollection Reaction
        {
            get
            {
                return m_reactionType;
            }
        }

        /// <summary>
        /// Gets the font for markers drawing when item has bullet style.
        /// </summary>
        public Font BulletFont
        {
            get
            {
                if (m_bulletFont == null)
                {
                    m_bulletFont = new Font(DEF_BULLET_FONT_NAME, this.Control.DefaultFormat.Font.Size);
                }

                return m_bulletFont;
            }
        }

        /// <summary>
        /// Gets or sets the indent from the left of the element. List has space from the left.
        /// </summary>
        public int Indent
        {
            get
            {
                return m_indent;
            }
            set
            {
                if (m_indent != value)
                {
                    m_indent = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element has ordered style.
        /// </summary>
        public bool StyleOrdered
        {
            get
            {
                bool result = true;
                ListItemType type = GetListStyle();

                if (type == ListItemType.circle ||
                  type == ListItemType.disc ||
                  type == ListItemType.square)
                {
                    result = false;
                }

                return result;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Redefines some attribute reaction of the control.
        /// </summary>
        private static void ReDefineReactions()
        {
            m_reactionType = (ReactionCollection)m_reaction.Clone();

            string typeName = typeof(HTMLAttributesCollection).ToString();

            m_reactionType.Add(typeName, AttributeName.Type, ReactType.ReCalculatingDocument);
        }

        /// <summary>
        /// Initializes static members of the ListElement class 
        /// </summary>
        static ListElement()
        {
            // Add some element to reactions colection.
            ReDefineReactions();

            m_unorderedFormat = new StringFormat();

            m_unorderedFormat.Alignment = StringAlignment.Far;
            m_unorderedFormat.LineAlignment = StringAlignment.Center;
            m_unorderedFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            m_orderedFormat = _stringFormat.Clone() as StringFormat;
            m_orderedFormat.Alignment = StringAlignment.Far;
        }

        /// <summary>
        /// Initializes a new instance of the ListElement class
        /// </summary>
        /// <param name="parent">Parent element of this object.</param>
        /// <param name="name">Name of the element.</param>
        public ListElement(IHTMLElement parent, string name)
            : base(parent, name)
        {
        }

        /// <summary>
        /// Overridden. Disposes element.
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            if (this.Control != null && this.Control.DefaultFormat != null)
            {
                HTMLFormat format = this.Control.DefaultFormat as HTMLFormat;

                if (format != null)
                {
                    format.FontChanged -= new ValueChangedEventHandler(DefFormatFontChanged);
                }
            }

            if (m_bulletFont != null)
            {
                m_bulletFont.Dispose();
                m_bulletFont = null;
            }
        }
        #endregion

        #region Class Public Methods

        /// <summary>
        /// Returns the marker style of the item.
        /// </summary>
        /// <param name="item">Item of the list.</param>
        /// <returns>Marker style of the item.</returns>

        public ListItemType GetItemStyle(LIElementImpl item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            ListItemType type = GetListStyle();
            IHTMLAttribute attr = item.Attributes[AttributeName.Type];

            if (attr != null && attr.Value.Length > 0)
            {
                type = AttributeParser.GetListItemType(attr.Value);
            }

            return type;
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
            CalculateIndent();

            this.Type = ElementType.BlockNewLineResizableIndent;
            this.Size = DefaultCalculateSizeInternal();

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            BaseElement parent = (BaseElement)this.Parent;
            m_curPos.X = parent.CurrentPosition.X + this.Indent;
            m_curPos.Y = parent.CurrentPosition.Y;

            Rectangle bounds = new Rectangle(parent.Bounds.X + this.Indent, parent.Bounds.Y, parent.Bounds.Width, parent.Bounds.Height);

            CalculateChildPositions(this.CurrentPosition, parent.Bounds);
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        /// <param name="curPosition">Point instance</param>
        /// <param name="bounds">Rectangle instance</param>
        /// <returns>BlockCollection object</returns>
        protected override BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            // Reduce width of the element's bounds.
            curPosition.X += this.Indent;
            this.CurrentPosition = curPosition;

            bounds.X += this.Indent;
            bounds.Width -= this.Indent;
            this.Bounds = bounds;

            Rectangle rect = this.MainBlock.Rectangle;
            rect.X += this.Indent;
            rect.Width -= this.Indent;
            this.MainBlock.Rectangle = rect;

            this.Width -= this.Indent;
            this.MainBlock.FinalWidth -= this.Indent;

            BlocksCollection blocks = base.CalculateChildPositions(this.CurrentPosition, bounds);

            // After inner content calculating expand bounds of the list element.
            int offset = this.Indent - IndentSpace.Left;
            this.MainBlock.X = this.MainBlock.Rectangle.X - offset;
            this.MainBlock.FinalWidth += Indent;

            return blocks;
        }

        /// <summary>
        /// Overridden. Calculates the format of element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();
        }

        /// <summary>
        /// Overridden. Moves current position of the parent element down after this element.
        /// </summary>
        /// <param name="currentPos">Current global position.</param>
        protected override void MoveFinalCurPos(ref Point currentPos)
        {
            base.MoveFinalCurPos(ref currentPos);

            int space = (int)this.Control.DefaultFormat.Font.GetHeight();

            BaseElement parent = (BaseElement)this.Parent;
            if (!(parent is ListElement))
            {
                parent.MakeBottomIndent(space);
            }
        }

        /// <summary>
        /// Overridden. Moves current position of the parent element down before.
        /// </summary>
        /// <param name="currentPos">Current global position.</param>
        protected override void MoveStartCurPos(ref Point currentPos)
        {
            base.MoveFinalCurPos(ref currentPos);

            int space = (int)this.Control.DefaultFormat.Font.GetHeight();

            BaseElement parent = (BaseElement)this.Parent;
            if ((parent.Type & ElementType.BlockNewLineIndent) == 0)
            {
                parent.MakeTopIndent(space);
            }
        }

        /// <summary>
        /// Overridden. Draws an element on the control.
        /// </summary>
        /// <param name="block">Current block.</param>
        /// <param name="e">Paint arguments.</param>
        protected override void DrawBlock(Block block, PaintEventArgs e)
        {
            base.DrawBlock(block, e);

            // If main block is painting - paint markers to all item elements.
            if (block.IsMain)
            {
                IHTMLElement[] items = this.Children.GetElementsByName(TagName.Li);

                for (int i = 0, len = items.Length; i < len; i++)
                {
                    LIElementImpl item = items[i] as LIElementImpl;
                    Block main = item.MainBlock;

                    if (main != null && main.First != null)
                    {
                        Rectangle rect = GlobalToClient(GetItemRectangle(main));

                        using (SolidBrush brush = new SolidBrush(Color.Black))
                        {
                            // Correct rectangle.
                            rect.Width = this.Indent;
                            rect.X -= this.Indent;

                            Font font = GetMarkerFont(item);
                            StringFormat strFormat = item.StyleOrdered ? m_orderedFormat : m_unorderedFormat;

                            e.Graphics.DrawString(item.Marker, font, brush, rect, strFormat);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Overridden. Initializes an element.
        /// </summary>
        protected internal override void InitializeElement()
        {
            base.InitializeElement();

            // We must to watch default font changing, because unordered markers use it's size.
            HTMLFormat format = this.Control.DefaultFormat as HTMLFormat;
            format.FontChanged += new ValueChangedEventHandler(DefFormatFontChanged);
        }
        #endregion

        #region Class event handlers

        /// <summary>
        /// Raised when default font is changed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DefFormatFontChanged(object sender, ValueChangedEventArgs e)
        {
            lock (m_bulletFont)
            {
                if (m_bulletFont != null)
                {
                    m_bulletFont.Dispose();
                    m_bulletFont = new Font(DEF_BULLET_FONT_NAME, this.Control.DefaultFormat.Font.Size);
                }
            }
        }
        #endregion

        #region Class utility methods

        /// <summary>
        /// Returs rectangle of the block, where data are located.
        /// </summary>
        /// <param name="parent">Parent main block.</param>
        /// <returns>Rectangle of the block, where data are located.</returns>
        private Rectangle GetItemRectangle(Block parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (!parent.IsMain)
                throw new ArgumentException("Block is not main");

            Rectangle result = Rectangle.Empty;

            for (int i = 0, len = parent.Count; i < len; i++)
            {
                Block obj = parent[i] as Block;

                if (obj != null && obj.Count > 0)
                {
                    result = parent[obj];
                    break;
                }
            }

            return result;
        }
        #endregion

        #region Class item markers methods

        /// <summary>
        /// Returns the string marker of item in the list.
        /// </summary>
        /// <param name="item">Item element in the list.</param>
        /// <returns>String marker of the item in the list.</returns>
        internal string GetMarker(LIElementImpl item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            string marker = string.Empty;
            ListItemType itemType = GetItemStyle(item);

            if (item.StyleOrdered)
            {
                int itemNumber = GetItemNumber(item);
                marker = GetOrderedMarker(itemType, itemNumber);
            }
            else
            {
                marker = GetUnorderedMarker(itemType);
            }

            return marker;
        }

        /// <summary>
        /// Returns the marker in the specified type.
        /// </summary>
        /// <param name="type">Type of numbering.</param>
       /// <returns>Marker by its type.</returns>
        private string GetUnorderedMarker(ListItemType type)
        {
            string marker = string.Empty;

            switch (type)
            {
                case ListItemType.circle: marker = DEF_MARKER_CIRCLE;
                    break;
                case ListItemType.disc: marker = DEF_MARKER_DISC;
                    break;
                case ListItemType.square: marker = DEF_MARKER_SQUARE;
                    break;
            }

            marker += " ";

            return marker;
        }

        /// <summary>
        /// Returns marker of numbered item.
        /// </summary>
        /// <param name="type">Type of item.</param>
        /// <param name="itemNumber">Item number.</param>
        /// <returns>Marker of numbered item.</returns>
        private string GetOrderedMarker(ListItemType type, int itemNumber)
        {
            string marker = itemNumber.ToString();

            if (itemNumber > 0)
            {
                switch (type)
                {
                    case ListItemType.A:
                        marker = Utilities.ArabicToLetter(itemNumber);
                        break;
                    case ListItemType.a:
                        marker = Utilities.ArabicToLetter(itemNumber);
                        marker = marker.ToLower();
                        break;
                    case ListItemType.I:
                        marker = Utilities.ArabicToRoman(itemNumber);
                        break;
                    case ListItemType.i:
                        marker = Utilities.ArabicToRoman(itemNumber);
                        marker = marker.ToLower();
                        break;
                }
            }

            marker += ". ";

            return marker;
        }

        /// <summary>
        /// Returns the style of the items numbering in the list.
        /// </summary>
        /// <returns>Style of items numbering in the list.</returns>
        private ListItemType GetListStyle()
        {
            ListItemType type = this.DefaultStyle;

            IHTMLAttribute attr = this.Attributes[AttributeName.Type];

            if (attr != null && attr.Value.Length > 0)
            {
                type = AttributeParser.GetListItemType(attr.Value);
            }

            return type;
        }

        /// <summary>
        /// Calculates the offset from the left of the list.
        /// </summary>
        private void CalculateIndent()
        {
            IHTMLElement[] items = this.Children.GetElementsByName(TagName.Li);
            for (int i = 0, len = items.Length; i < len; i++)
            {
                LIElementImpl item = items[i] as LIElementImpl;

                Size size = GetMarkerSize(item);
                m_indent = Math.Max(this.Indent, size.Width);
            }
        }

        /// <summary>
        /// Overloaded. Calculates the size of the marker.
        /// </summary>
        /// <param name="marker">string value</param>
        /// <param name="font">Font object</param>
        /// <returns>Size of the marker.</returns>
        private Size GetMarkerSize(string marker, Font font)
        {
            if (marker == null)
                throw new ArgumentNullException("marker");
            if (font == null)
                throw new ArgumentNullException("font");

            Size size = this.MeasureString(marker, font);
            size.Width += DEF_MARKER_INDENT;

            return size;
        }

        /// <summary>
        /// Calculates the size of the marker for the specified item.
        /// </summary>
        /// <param name="item">Item element.</param>
        /// <returns>Size of the marker for item.</returns>
        private Size GetMarkerSize(LIElementImpl item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            string marker = GetMarker(item);
            Font font = GetMarkerFont(item);

            return GetMarkerSize(marker, font);
        }

        /// <summary>
        /// Returns the font object by which marker will be calculated.
        /// </summary>
        /// <param name="item">Item element.</param>
        /// <returns>Font of the marker.</returns>
        private Font GetMarkerFont(LIElementImpl item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Font font = (Font)item.Format.Font;

            if (!item.StyleOrdered)
            {
                font = this.BulletFont;
            }

            return font;
        }

        /// <summary>
        /// Returns the index of the item in the items holder.
        /// </summary>
        /// <param name="item">Item in the list.</param>
        /// <returns>Index of item in the list if found; -1 otherwise.</returns>
        private int IndexOf(LIElementImpl item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            IHTMLElement[] children = this.Children.GetElementsByName(item.Name);
            int index = Array.IndexOf(children, item);

            return ++index;
        }

        /// <summary>
        /// Calculates number of item.
        /// </summary>
        /// <param name="item">Item of the list.</param>
        /// <returns>Number of the item.</returns>
        private int GetItemNumber(LIElementImpl item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            int number = -1;

            if (item.StyleOrdered)
            {
                IHTMLAttribute attr = item.Attributes[AttributeName.Value];
                double num = 0;
                if (attr != null &&
                  double.TryParse(attr.Value, NumberStyles.Integer, null, out num) &&
                  num > 0)
                {
                    number = (int)num;
                }
                else
                {
                    number = IndexOf(item);
                }
            }

            return number;
        }
        #endregion
    }
}
