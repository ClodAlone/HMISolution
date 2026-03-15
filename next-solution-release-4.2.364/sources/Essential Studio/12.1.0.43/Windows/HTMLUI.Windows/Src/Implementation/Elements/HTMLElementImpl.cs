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
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;HTML&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Html)]
    public class HTMLElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Html;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        /// Holds all events.
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
        /// Initializes static members of the HTMLElementImpl class
        /// </summary>
        static HTMLElementImpl()
        {
            Type type = typeof(HTMLElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the HTMLElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public HTMLElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        /// <summary>
        /// Overridden. Disposes object.
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            if (this.Control != null)
            {
                HTMLFormat documentFormat = this.Control.DefaultFormat as HTMLFormat;

                if (documentFormat != null)
                {
                    documentFormat.OnChanged -= new BeforeValueChangeEventHandler(Attributes_Changed);
                }
            }
        }

        #endregion

        #region Class Overrides
        /// <summary>
        /// Returns an instance of the event.
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
            this.Size = Size.Empty;

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            HTMLFormat documentFormat = this.Control.DefaultFormat as HTMLFormat;

            documentFormat.OnChanged -= new BeforeValueChangeEventHandler(Attributes_Changed);
            documentFormat.OnChanged += new BeforeValueChangeEventHandler(Attributes_Changed);

            DefaultCalculateFormatInternal();
        }

        /// <summary>
        /// Overridden. Draws an element.
        /// </summary>
        /// <param name="e">PaintEventArgs instance</param>
        protected override void DrawElementInternal(PaintEventArgs e)
        {
            if (Control.DefaultFormat.BackgroundColor == this.Document.RenderRoot.Format.BackgroundColor)
            {
                Point startPoint = new Point(this.Document.Margins.Left, this.Document.Margins.Top);
                SolidBrush bgBrush = new SolidBrush(this.Control.DefaultFormat.BackgroundColor);

                Rectangle elRect = new Rectangle(startPoint, this.Document.AutoScrollMinSize);
                e.Graphics.FillRectangle(bgBrush, GlobalToClient(elRect));

                bgBrush.Dispose();
                bgBrush = null;
            }
        }
        #endregion

        #region Class Event Catchers
        /// <summary>
        /// Raises the Mouse Click event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal override void RaiseClickEvent(EventArgs args)
        {
        }

        /// <summary>
        /// Raises the Mouse Double Click event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal override void RaiseDoubleClickEvent(EventArgs args)
        {
        }

        /// <summary>
        /// Raises the Mouse Move Event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal override void RaiseMouseMoveEvent(EventArgs args)
        {
        }

        /// <summary>
        /// Raises the Mouse Enter event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal override void RaiseMouseEnterEvent(EventArgs args)
        {
        }

        /// <summary>
        /// Raises the Mouse Leave event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal override void RaiseMouseLeaveEvent(EventArgs args)
        {
        }

        /// <summary>
        /// Raises the Mouse Move Down event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal override void RaiseMouseDownEvent(EventArgs args)
        {
        }

        /// <summary>
        /// Raises the Key Down event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal override void RaiseKeyDownEvent(EventArgs args)
        {
        }

        /// <summary>
        /// Raises the Key Up event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal override void RaiseKeyUpEvent(EventArgs args)
        {
        }

        /// <summary>
        /// Raises the Key Press event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal override void RaiseKeyPressEvent(EventArgs args)
        {
        }

        #endregion
    }
}