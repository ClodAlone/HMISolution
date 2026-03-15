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
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;TEXTAREA&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Textarea)]
    public class TEXTAREAElementImpl
      : UserControlHolderBase
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Textarea;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;

        /// <summary>
        /// Control class instance.
        /// </summary>
        private IControlImpl m_userControlImpl;
        #endregion

        #region Class members
        /// <summary>
        /// Array of merged events.
        /// </summary>
        private ArrayList m_mergeEvents = new ArrayList();

        /// <summary>
        /// Holds the type of reactions that occur when attributes change.
        /// </summary>
        private static ReactionCollection m_reactionType;
        #endregion

        #region Class Properties
        /// <summary>
        /// Overridden. Returns an array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                if (m_mergeEvents.Count == 0)
                    return DEF_SUPP_EVENTS;

                return (string[])m_mergeEvents.ToArray(typeof(string));
            }
        }

        /// <summary>
        /// Overridden. Gets or sets the user control instance.
        /// </summary>
        public override IControlImpl UserControl
        {
            get
            {
                return m_userControlImpl;
            }
        }

        /// <summary>
        /// Overridden. Returns an hashtable that contains names of attributes as keys and type of reaction that
        /// occur when attricutes change as values.
        /// </summary>
        internal override ReactionCollection Reaction
        {
            get
            {
                return m_reactionType;
            }
        }

        #endregion

        #region Class Events
        /// <summary>
        /// Delegate. Raised when there is need to create a new special event.
        /// </summary>
        internal event EnhanceEventsEventHandler OnCreateEvent;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Redefines an attribute reaction of control.
        /// </summary>
        private static void ReDefineReactions()
        {
            m_reactionType = (ReactionCollection)m_reaction.Clone();

            string typeName = typeof(HTMLAttributesCollection).ToString();

            m_reactionType.Add(typeName, AttributeName.Rows, ReactType.ReFormatMergElmAndReCalcDoc);

            m_reactionType.Add(typeName, AttributeName.Cols, ReactType.ReFormatMergElmAndReCalcDoc);
        }

        /// <summary>
        /// Initializes static members of the TEXTAREAElementImpl class 
        /// </summary>
        static TEXTAREAElementImpl()
        {
            Type type = typeof(TEXTAREAElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);

            // Define new reactions on this elements.
            ReDefineReactions();
        }

        /// <summary>
        /// Initializes a new instance of the TEXTAREAElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public TEXTAREAElementImpl(IHTMLElement parent)
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
            EnhanceEventsEventArgs args = new EnhanceEventsEventArgs(name);

            if (OnCreateEvent != null)
            {
                OnCreateEvent(this, args);
            }

            return (args.Event != null) ? args.Event :
              new HashElementEvents(this, m_eventHash, name);
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
        /// Overridden. Calculates the format to the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();

            this.Control.Recalculate = false;

            if (this.UserControl == null)
            {
                m_userControlImpl = UserControlFactory.CreateUserControl(this);
            }
            else
            {
                this.UserControl.ConfigureControl();
            }

            this.Control.Recalculate = true;
        }

        /// <summary>
        /// Overridden. Draws an element on the control.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void DrawElementInternal(PaintEventArgs e)
        {
            return;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Merges standard and special events.
        /// </summary>
        /// <param name="events">Array of events.</param>
        public void MergeSupportedEvents(string[] events)
        {
            if (events == null)
                throw new ArgumentNullException("events");

            Hashtable hash = new Hashtable();

            string str = string.Empty;

            for (int i = 0, len = DEF_SUPP_EVENTS.Length; i < len; i++)
            {
                str = DEF_SUPP_EVENTS[i];
                hash[str] = 1;
            }

            for (int i = 0, len = events.Length; i < len; i++)
            {
                str = events[i];
                hash[str] = 1;
            }

            for (int i = 0, len = m_mergeEvents.Count; i < len; i++)
            {
                str = (string)m_mergeEvents[i];
                hash[str] = 1;
            }

            m_mergeEvents.Clear();
            m_mergeEvents.AddRange(hash.Keys);
        }
        #endregion
    }
}