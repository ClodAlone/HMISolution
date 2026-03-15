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
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;H1&gt; Tag.
    /// </summary>
    [ElementTag(TagName.H1)]
    public class H1ElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.H1;

        /// <summary>
        /// Default size of the element's font.
        /// </summary>
        private const int DEF_SIZE = 22;

        /// <summary>
        /// Indent from top and bottom for drawing.
        /// </summary>
        private const int DEF_INCREASOR = 10;

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
        /// Overridden. Own format for font. (Sets the size of the font if needed).
        /// </summary>
        protected internal override HTMLFormat OwnFormat
        {
            get
            {
                if (m_ownFormat == null)
                {
                    return SetOwnFormat();
                }
                return m_ownFormat;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the H1ElementImpl class
        /// </summary>
        static H1ElementImpl()
        {
            Type type = typeof(H1ElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the H1ElementImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        public H1ElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        /// <summary>
        /// Initializes a new instance of the H1ElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="tagName">Name of the tag.</param>
        protected H1ElementImpl(IHTMLElement parent, string tagName)
            : base(parent, tagName)
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
            this.CurrentPosition = parent.CurrentPosition;

            CalculateChildPositions(this.CurrentPosition, parent.Bounds);
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();
        }

        /// <summary>
        /// Overridden. Moves the current position of the parent element down after this element.
        /// </summary>
        /// <param name="currentPos">Current global position.</param>
        protected override void MoveFinalCurPos(ref Point currentPos)
        {
            base.MoveFinalCurPos(ref currentPos);

            int space = (int)this.Control.DefaultFormat.Font.GetHeight();

            BaseElement parent = (BaseElement)this.Parent;
            parent.MakeBottomIndent(space);
        }

        /// <summary>
        /// Overridden. Moves the current position of the parent element down before.
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
        #endregion

        #region Class utility methods
        /// <summary>
        /// Sets own format to the element.
        /// </summary>
        /// <returns>Resulting format.</returns>
        private HTMLFormat SetOwnFormat()
        {
            m_ownFormat = base.OwnFormat;

            m_ownFormat.Merge = MergeMask.FontSize;
            m_ownFormat.IsFontCreated = false;
            m_ownFormat.FontSize = GetFontSize();

            m_ownFormat.Merge |= MergeMask.FontWeight;
            m_ownFormat.FontWeight = FontStyle.Bold;

            return m_ownFormat;
        }

        /// <summary>
        /// Returns the default font size for element.
        /// </summary>
        /// <returns>Size of the font.</returns>
        protected virtual int GetFontSize()
        {
            return DEF_SIZE;
        }
        #endregion
    }

    /// <summary>
    /// Class that is responsible for &lt;H2&gt; Tag.
    /// </summary>
    [ElementTag(TagName.H2)]
    public class H2ElementImpl : H1ElementImpl
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.H2;

        /// <summary>
        /// Default size of the element's font.
        /// </summary>
        private const int DEF_SIZE = 17;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the H2ElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public H2ElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns the default font size for the element.
        /// </summary>
        /// <returns>Size of the font.</returns>
        protected override int GetFontSize()
        {
            return DEF_SIZE;
        }
        #endregion
    }

    /// <summary>
    /// Class that is responsible for &lt;H3&gt; Tag.
    /// </summary>
    [ElementTag(TagName.H3)]
    public class H3ElementImpl : H1ElementImpl
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.H3;

        /// <summary>
        /// Default size of the element's font.
        /// </summary>
        private const int DEF_SIZE = 13;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the H3ElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public H3ElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns the default font size for the element.
        /// </summary>
        /// <returns>Size of the font.</returns>
        protected override int GetFontSize()
        {
            return DEF_SIZE;
        }
        #endregion
    }

    /// <summary>
    /// Class that is responsible for &lt;H4&gt; Tag.
    /// </summary>
    [ElementTag(TagName.H4)]
    public class H4ElementImpl : H1ElementImpl
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.H4;

        /// <summary>
        /// Default size of the element's font.
        /// </summary>
        private const int DEF_SIZE = 11;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the H4ElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public H4ElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns the default font size for the element.
        /// </summary>
        /// <returns>Size of the font.</returns>
        protected override int GetFontSize()
        {
            return DEF_SIZE;
        }
        #endregion
    }

    /// <summary>
    /// Class that is responsible for &lt;H3&gt; Tag.
    /// </summary>
    [ElementTag(TagName.H5)]
    public class H5ElementImpl : H1ElementImpl
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.H5;

        /// <summary>
        /// Default size of the element's font.
        /// </summary>
        private const int DEF_SIZE = 9;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the H5ElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public H5ElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns the default font size for the element.
        /// </summary>
        /// <returns>Size of the font.</returns>
        protected override int GetFontSize()
        {
            return DEF_SIZE;
        }
        #endregion
    }

    /// <summary>
    /// Class that is responsible for &lt;H6&gt; Tag.
    /// </summary>
    [ElementTag(TagName.H6)]
    public class H6ElementImpl : H1ElementImpl
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.H6;

        /// <summary>
        /// Default size of the element's font.
        /// </summary>
        private const int DEF_SIZE = 7;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the H6ElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public H6ElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns the default font size for the element.
        /// </summary>
        /// <returns>Size of the font.</returns>
        protected override int GetFontSize()
        {
            return DEF_SIZE;
        }
        #endregion
    }
}