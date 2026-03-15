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
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;INPUT&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Input)]
    public class INPUTElementImpl
      : UserControlHolderBase
    {
        #region Class constants

        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Input;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
         #endregion

        #region Class members
        /// <summary>
        /// Control class instance.
        /// </summary>
        private IControlImpl m_userControlImpl;

        /// <summary>
        /// Array of merged events.
        /// </summary>
        private ArrayList m_mergeEvents = new ArrayList();

        /// <summary>
        /// Holds reaction that occur when attributes change.
        /// </summary>
        private static ReactionCollection m_reactionType;
        #endregion

        #region Class Properties
        /// <summary>
        /// Returns the array of supported events.
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
        /// Gets or sets the user control instance.
        /// </summary>
        public override IControlImpl UserControl
        {
            get
            {
                return m_userControlImpl;
            }
        }

        /// <summary>
        /// Returns an hashtable that contain names of attributes as keys and type of reaction that occur
        /// when attributes change as values.
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
        /// Raised when we need a special event which is not created yet.
        /// </summary>
        internal event EnhanceEventsEventHandler OnCreateEvent;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Redefines some attribute reaction of control.
        /// </summary>
        private static void ReDefineReactions()
        {
            m_reactionType = (ReactionCollection)m_reaction.Clone();

            string typeName = typeof(HTMLAttributesCollection).ToString();

            m_reactionType.Add(typeName, AttributeName.MaxLength, ReactType.ReFormatMergingElement);

            m_reactionType.Add(typeName, AttributeName.Checked, ReactType.ReFormatMergingElement);

            m_reactionType.Add(typeName, AttributeName.Value, ReactType.ReFormatMergElmAndReCalcDoc);

            m_reactionType.Add(typeName, AttributeName.Multiple, ReactType.ReFormatMergingElement);
        }

        /// <summary>
        /// Initializes static members of the INPUTElementImpl class 
        /// </summary>
        static INPUTElementImpl()
        {
            Type type = typeof(INPUTElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);

            // Define new reactions on this elements.
            ReDefineReactions();
        }

        /// <summary>
        /// Initializes a new instance of the INPUTElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public INPUTElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Merges standard and special events.
        /// </summary>
        /// <param name="events">Array of the events.</param>
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

            foreach (string strg in m_mergeEvents)
            {
                hash[strg] = 1;
            }

            m_mergeEvents.Clear();
            m_mergeEvents.AddRange(hash.Keys);
        }

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
            //// this.UserControl.CustomControl.Location = this.CurrentPosition;

            CalculateChildPositions(this.CurrentPosition, parent.Bounds);
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
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

            this.IsVisible = this.UserControl.CustomControl.Visible;
            this.Control.Recalculate = true;
        }

        /// <summary>
        /// Overridden. Detects the type of control and creates the corresponding control.
        /// </summary>
        /// <param name="control">Control object.</param>
        /// <param name="element">XML element of object.</param>
        public override void InfillFromXMLElement(HTMLUIControl control, XmlElement element)
        {
            base.InfillFromXMLElement(control, element);
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
        /// Overridden. Clones object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public override object Clone()
        {
            INPUTElementImpl twin = base.Clone() as INPUTElementImpl;
            twin.m_mergeEvents = new ArrayList();

            return twin;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Returns the background color for the control element from parents.
        /// </summary>
        /// <returns>Color instance</returns>
        protected internal Color GetBgColorForControl()
        {
            Color result = this.Format.BackgroundColor;
            Color empty = Color.Empty;
            IHTMLElement parent = this;

            while (true)
            {
                if (result != empty || parent == null ||
                  parent == this.Document.RenderRoot) break;

                parent = parent.Parent;

                if (parent != null)
                {
                    result = parent.Format.BackgroundColor;
                }
            }

            if (result == empty)
            {
                result = this.Control.DefaultFormat.BackgroundColor;
            }

            return result;
        }
        #endregion
    }
}