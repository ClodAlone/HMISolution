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
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;B&gt; Tag.
    /// </summary>
    [ElementTag(TagName.B)]
    public class BElementImpl : BaseElement
    {
        #region Class constants

        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.B;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        /// Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;

        #endregion

        #region Class Properties

        /// <summary>
        /// Overridden. Returns an array of the supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }

        /// <summary>
        /// Overridden. Returns specific element's settings for drawing.
        /// </summary>
        protected internal override HTMLFormat OwnFormat
        {
            get
            {
                if (m_ownFormat == null)
                {
                    m_ownFormat = base.OwnFormat;

                    m_ownFormat.Merge = MergeMask.FontWeight;
                    m_ownFormat.FontWeight = FontStyle.Bold;
                }

                return m_ownFormat;
            }
        }

        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes static members of the BElementImpl class 
        /// </summary>
        static BElementImpl()
        {
            Type type = typeof(BElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the BElementImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        public BElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BElementImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        /// <param name="tagName">Name of the tag.</param>
        protected BElementImpl(IHTMLElement parent, string tagName)
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
        /// Overridden. Calculates the format of the element from an array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();
        }
        #endregion
        }
}