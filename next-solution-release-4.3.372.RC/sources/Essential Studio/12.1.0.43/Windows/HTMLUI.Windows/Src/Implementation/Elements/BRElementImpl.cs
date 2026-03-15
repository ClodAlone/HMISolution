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
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;BR&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Br)]
    public class BRElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Br;

        /// <summary>
        /// Default height by which we make new line.
        /// </summary>
        private const int DEF_HEIGHT = 14;

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
        /// Overridden. Indicates the visibility of the control.
        /// </summary>
        public override bool IsVisible
        {
            get
            {
                if (!this.Attributes.Contains(DEF_RUNTIME_VISIBLE))
                {
                    this.Attributes.Add(DEF_RUNTIME_VISIBLE);
                    ((HTMLAttributeImpl)this.Attributes[DEF_RUNTIME_VISIBLE]).Value = false.ToString();
                }

                return false;
            }
            set
            {
                if (value != true)
                {
                    ////throw new ArgumentException( "Visibility of this element must be False.", "IsVisible" );

                    if (!this.Attributes.Contains(DEF_RUNTIME_VISIBLE))
                    {
                        this.Attributes.Add(DEF_RUNTIME_VISIBLE);
                        ((HTMLAttributeImpl)this.Attributes[DEF_RUNTIME_VISIBLE]).Value = false.ToString();
                    }
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the BRElementImpl class 
        /// </summary>
        static BRElementImpl()
        {
            Type type = typeof(BRElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the BRElementImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        public BRElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
            if (parent != null)
                this.Width = ((BaseElement)this.Parent).Width;
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
        /// <returns>Size of the element.</returns>
        protected override Size CalculateSizeInternal()
        {
            this.Size = new Size(0, GetHeight());

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
        /// Overridden. Calculates the format of the element from an array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            this.Type = ElementType.InLineNewLine;
            this.IsVisible = false;
        }

        /// <summary>
        /// Overridden. Calculates its position and size.
        /// </summary>
        /// <param name="curPosition">Current global position.</param>
        /// <param name="bounds">Bounds for the element.</param>
        /// <returns>Array of blocks.</returns>
        protected override BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            this.QuietMode = true;
            m_blocks.Clear();
            m_curBlock = null;

            //// If before line break was something in the same line, define element size to
            //// zero and postpone location to the next line, otherwise - if in current line no anything,
            //// leave location in the same line, but define height of the element not zero, but height
            //// of postponed line.

            this.CurrentPosition = curPosition;
            this.Location = this.CurrentPosition;
            this.Bounds = bounds;

            //// Makes new line.
            int oldLineHeight = GetIncreaseValue();
            int newLineHeight = (oldLineHeight == 0) ? DEF_HEIGHT : oldLineHeight;
            m_curPos.Y += newLineHeight;
            m_curPos.X = this.Bounds.X;

            if (oldLineHeight > 0)
            {
                this.Location = this.CurrentPosition;
            }

            //// Creates new empty block.
            CreateBlock(null);
            this.Width = 0;
            this.Height = GetHeight();
            Rectangle rect = new Rectangle(this.Location, this.Size);
            AddElement(m_curBlock, this, rect);
            m_curBlock.DrawProperties = false;

            ProhibitSpaceAfter();
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
            return;
        }

        /// <summary>
        /// Overridden. Sets value indicating whether trailing whitespace is allowed after element.
        /// </summary>
        protected override void ProhibitSpaceAfter()
        {
            if (this.Parent != null)
            {
                (this.Parent as BaseElement).SpaceProhibited = true;
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Returns height of the element.
        /// </summary>
        /// <returns>height of the element.</returns>
        private int GetHeight()
        {
            int height = DEF_HEIGHT;
            BaseElement parent = this.Parent as BaseElement;

            if (parent != null)
            {
                height = parent.Format.Font.Height;
            }

            return height;
        }
        #endregion
    }
}