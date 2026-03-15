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
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;FORM&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Form)]
    public class FORMElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Form;

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
        /// Returns the array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the FORMElementImpl class
        /// </summary>
        static FORMElementImpl()
        {
            Type type = typeof(FORMElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the FORMElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public FORMElementImpl(IHTMLElement parent)
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
    }
}
