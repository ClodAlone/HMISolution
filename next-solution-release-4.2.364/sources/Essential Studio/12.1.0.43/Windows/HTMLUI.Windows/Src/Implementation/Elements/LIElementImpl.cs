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
using System.Windows.Forms;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;LI&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Li)]
    public class LIElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Li;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;

        /// <summary>
        /// Holds type of reaction on attribute changing.
        /// </summary>
        private static ReactionCollection m_reactionType;
        #endregion

        #region Class members
        /// <summary>
        /// Marker of the item in the list. 
        /// </summary>
        private string m_marker;
        #endregion

        #region Class Properties
        /// <summary>
        /// Overridden. Returns an array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }

        /// <summary>
        /// Overridden. Returns an hashtable that contains the names of attributes as keys and type of reaction of its
        /// changing as value.
        /// </summary>
        internal override ReactionCollection Reaction
        {
            get
            {
                return m_reactionType;
            }
        }

        /// <summary>
        /// Gets the string marker for the item.
        /// </summary>
        internal string Marker
        {
            get
            {
                if (m_marker == null || m_marker.Length == 0)
                {
                    m_marker = GetMarker();
                }

                return m_marker;
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
                ListItemType type = ListItemType._1;
                ListElement parent = GetListParent();

                if (parent != null)
                {
                    type = parent.GetItemStyle(this);

                    if (type == ListItemType.circle ||
                      type == ListItemType.disc ||
                      type == ListItemType.square)
                    {
                        result = false;
                    }
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
        /// Initializes static members of the LIElementImpl class 
        /// </summary>
        static LIElementImpl()
        {
            ReDefineReactions();

            Type type = typeof(LIElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the LIElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public LIElementImpl(IHTMLElement parent)
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
        /// <returns>Size instance</returns>
        protected override Size CalculateSizeInternal()
        {
            this.Type = ElementType.BlockNewLineResizable;
            this.Size = DefaultCalculateSizeInternal();

            // Add to width of the item.
            ListElement parent = GetListParent();

            if (parent != null)
            {
                this.Width += parent.Indent;
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
        }

        /// <summary>
        /// Overridden. Implements special features for this element.
        /// </summary>
        /// <param name="curPosition">Global current position.</param>
        /// <param name="bounds">Bounds for this element.</param>
        /// <returns>Array of blocks.</returns>
        protected override BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            BlocksCollection blocks = base.CalculateChildPositions(curPosition, bounds);

            // Reset marker.
            m_marker = null;

            return blocks;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Returns the marker for drawing.
        /// </summary>
        /// <returns>Marker for indicating the list.</returns>
        private string GetMarker()
        {
            ListElement parentEx = GetListParent();

            string marker = string.Empty;

            if (parentEx != null)
            {
                marker = parentEx.GetMarker(this);
            }

            return marker;
        }

        /// <summary>
        /// Returns the List parent for the node if found; Null otherwise.
        /// </summary>
        /// <returns>List parent for the node if found; Null otherwise.</returns>
        private ListElement GetListParent()
        {
            return this.Parent as ListElement;
        }
        #endregion
    }
}
