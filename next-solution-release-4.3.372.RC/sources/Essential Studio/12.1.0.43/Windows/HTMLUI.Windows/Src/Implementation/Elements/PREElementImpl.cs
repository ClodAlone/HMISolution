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
using System.Reflection;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;PRE&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Pre)]
    public class PREElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Pre;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        ///  Holds all events which this class supports.
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
                    m_ownFormat.Merge = MergeMask.FontFamily;
                    m_ownFormat.IsFontCreated = false;
                    m_ownFormat.FontFamilyName = FontFamily.GenericMonospace.Name;
                }

                return m_ownFormat;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the PREElementImpl class 
        /// </summary>
        static PREElementImpl()
        {
            Type type = typeof(PREElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the PREElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public PREElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns an instance of the event.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <returns>Event object.</returns>
        protected override IHTMLEvent CreateEventInternal(string name)
        {
            return new HashElementEvents(this, m_eventHash, name);
        }

        /// <summary>
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size instance</returns>
        protected override Size CalculateSizeInternal()
        {
            this.Type = ElementType.BlockNewLineResizableIndent;
            this.SkipWhiteSpaces = false;
            this.Size = DefaultCalculateSizeInternal();

            //// if( this.Control.IsLoading )
            {
                this.MinWidth = this.Width;
                this.MinHeight = this.Height;
            }

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
            DefaultCalculateFormatInternal();
            DeleteStartEndNewLines();
        }

        /// <summary>
        /// Overridden. Calculates the position of the text elements.
        /// </summary>
        /// <param name="node">Text object which contains text.</param>
        /// <param name="block">Current block object.</param>
        protected override void CalculateTextPos(Text node, Block block)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (block == null)
                throw new ArgumentNullException("block");

            if (node.Value.IndexOf(Environment.NewLine) >= 0)
            {
                BreakOnTwoLines(node, block);
                return;
            }

            base.CalculateTextPos(node, block);
        }

        /// <summary>
        /// Overridden. Calculates the size of text elements.
        /// </summary>
        /// <returns>Max size.</returns>
        protected override Size InFillTextSizeHash()
        {
            return base.InFillTextSizeHash();
        }

        /// <summary>
        /// Overridden. Moves the current position of the parent element down after this element.
        /// </summary>
        /// <param name="currentPos">Current position.</param>
        protected override void MoveFinalCurPos(ref Point currentPos)
        {
            base.MoveFinalCurPos(ref currentPos);
        }

        /// <summary>
        /// Overridden. Moves the current position of the parent element down before.
        /// </summary>
        /// <param name="currentPos">Current position.</param>
        protected override void MoveStartCurPos(ref Point currentPos)
        {
            base.MoveFinalCurPos(ref currentPos);
        }

        /// <summary>
        /// Overridden. Calculates the width of the element.
        /// </summary>
        /// <param name="str">String text of the element.</param>
        /// <param name="max">Max width.</param>
        /// <param name="format">Format object.</param>
        /// <returns>Max width of the text.</returns>
        protected override int GetRealMaxWidth(string str, int max, HTMLFormat format)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length == 0)
                throw new ArgumentException("str - string can not be empty");

            if (format == null)
                throw new ArgumentNullException("format");

            ArrayList words = InFillBlocks(str);
            string token = string.Empty;

            max = Math.Max(max, MeasureString(str, format.Font).Width);
            return max;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// If new line symbol exists in text, breaks apart text into two parts.
        /// </summary>
        /// <param name="node">Text node object.</param>
        /// <param name="block">Current block.</param>
        private void BreakOnTwoLines(Text node, Block block)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (block == null)
                throw new ArgumentNullException("block");

            int index = node.Value.IndexOf(Environment.NewLine);

            // Text has new line symbols.
            if (index > 0)
            {
                Text first = (Text)node.Clone();
                first.Value = node.Value.Substring(0, index);
                CalculateTextPos(first, block);

                CreateBlock(m_curBlock);
                Text second = (Text)node.Clone();
                second.Value = node.Value.Substring(index + Environment.NewLine.Length);
                CalculateTextPos(second, m_curBlock);
            }
            else if (index == 0) 
            {
                // String starts from new line symbols.
                int space = (int)this.Format.Font.GetHeight();
                MakeTopIndent(space);
                Text text = node.Clone();
                text.Value = node.Value.Substring(Environment.NewLine.Length);
                CalculateTextPos(text, m_curBlock);
            }
        }

        /// <summary>
        /// Removes the start and end new line symbols in the tag element.
        /// </summary>
        private void DeleteStartEndNewLines()
        {
            XmlNode first = this.Storage.FirstChild;
            XmlNode last = this.Storage.LastChild;

            // Check first new line just after begin tag.
            if (first is XmlText || first is XmlWhitespace)
            {
                if (first.Value.Length >= 2)
                {
                    if (first.Value == Environment.NewLine)
                    {
                        first.ParentNode.RemoveChild(first);
                    }
                    else if (first.Value.StartsWith(Environment.NewLine))
                    {
                        first.Value = first.Value.Remove(0, Environment.NewLine.Length);
                    }
                }
            }

            // Check last new line at the end of tag.
            if (last is XmlText || last is XmlWhitespace)
            {
                if (last.Value.Length >= 2)
                {
                    if (last.Value.Substring(last.Value.Length - Environment.NewLine.Length) == Environment.NewLine)
                    {
                        last.Value = last.Value.Substring(0, last.Value.Length - Environment.NewLine.Length);
                    }
                }
            }
        }
        #endregion
    }
}
