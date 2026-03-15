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
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;BODY&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Body)]
    public class BODYElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Body;

        /// <summary>
        /// Default value of the left margin attribute.
        /// </summary>
        private const int DEF_LEFT_MARGIN = 10;

        /// <summary>
        /// Default value of the top margin attribute.
        /// </summary>
        private const int DEF_TOP_MARGIN = 15;

        /// <summary>
        /// Default value of the right margin attribute.
        /// </summary>
        private const int DEF_RIGHT_MARGIN = 10;

        /// <summary>
        /// Default value of the bottom margin attribute.
        /// </summary>
        private const int DEF_BOTTOM_MARGIN = 15;

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
        /// Overridden. Returns the HTML format for the element.
        /// </summary>
        protected internal override HTMLFormat OwnFormat
        {
            get
            {
                return base.OwnFormat;
                /*
                if( m_ownFormat == null )
                {
                  m_ownFormat = base.OwnFormat;

                  m_ownFormat.Merge = MergeMask.BgColor;
                  //m_ownFormat.BackgroundColor = Color.White;
                }

                return m_ownFormat;*/
            }
        }

        /// <summary>
        /// Gets or sets the left margin attribute for the element.
        /// </summary>
        protected internal int LeftMargin
        {
            get
            {
                return GetMargin(AttributeName.LeftMargin);
            }
            set
            {
                SetMargin(AttributeName.LeftMargin, value);
            }
        }

        /// <summary>
        /// Gets or sets the top margin attribute for the element.
        /// </summary>
        protected internal int TopMargin
        {
            get
            {
                return GetMargin(AttributeName.TopMargin);
            }
            set
            {
                SetMargin(AttributeName.TopMargin, value);
            }
        }

        /// <summary>
        /// Gets or sets the right margin attribute for the element.
        /// </summary>
        protected internal int RightMargin
        {
            get
            {
                return GetMargin(AttributeName.RightMargin);
            }
            set
            {
                SetMargin(AttributeName.RightMargin, value);
            }
        }

        /// <summary>
        /// Gets or sets the bottom margin attribute for the element.
        /// </summary>
        protected internal int BottomMargin
        {
            get
            {
                return GetMargin(AttributeName.BottomMargin);
            }
            set
            {
                SetMargin(AttributeName.BottomMargin, value);
            }
        }
        #endregion

        #region Class Events
        /// <summary>
        /// Event raised when document is loading.
        /// </summary>
        [ElementEvent("RaiseOnLoadEvent")]
        public event EventHandler OnLoad;

        /// <summary>
        /// Event raised when document is unloading.
        /// </summary>
        [ElementEvent("RaiseOnUnLoadEvent")]
        public event EventHandler OnUnLoad;

        /// <summary>
        /// Event raised when document is scrolling.
        /// </summary>
        [ElementEvent("RaiseOnScrollEvent")]
        public event EventHandler OnScroll;

        /// <summary>
        /// Event raised when document is resizing.
        /// </summary>
        [ElementEvent("RaiseOnResizeEvent")]
        public event EventHandler OnResize;

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the BODYElementImpl class 
        /// </summary>
        static BODYElementImpl()
        {
            Type type = typeof(BODYElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the BODYElementImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        public BODYElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
            this.Type = ElementType.BlockResizable;
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
            this.Document.ClearRectSearcher();
            this.Type = ElementType.BlockResizable;

            this.Size = DefaultCalculateSizeInternal();

            this.Width = Math.Max(this.Width, this.Document.ClientSize.Width);
            this.Height = Math.Max(this.Height, this.Document.ClientSize.Height);
            ResizeControl();

            return this.Size;
        }

        /// <summary>
        ///  Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            Point location = new Point(this.Document.Margins.Left, this.Document.Margins.Top);

            this.QuietMode = true;
            this.Location = location;
            this.CurrentPosition = location;
            this.Bounds = new Rectangle(
              this.Location, this.Document.AutoScrollMinSize);

            Block mainChildBlock = new Block(this);
            mainChildBlock.IsMain = true;
            this.MainBlock = mainChildBlock;

            BlocksCollection blocks = this.CalculateChildPositions(this.CurrentPosition, ReduceBounds(this.Bounds));

            mainChildBlock.Width = this.Width;
            mainChildBlock.Height = this.Height;
            AddBlocksToMain(this, mainChildBlock, blocks);

            ResizeControl();
            this.QuietMode = false;
            OnLocationCalculated(EventArgs.Empty);
        }

        /// <summary>
        /// Overridden. Draws an element.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void DrawElementInternal(PaintEventArgs e)
        {
            if (this.Format.BackgroundColor != this.Control.DefaultFormat.BackgroundColor)
            {
                SolidBrush bgBrush = new SolidBrush(this.Format.BackgroundColor);
                Rectangle elRect = new Rectangle(this.Location, this.Document.AutoScrollMinSize);
                e.Graphics.FillRectangle(bgBrush, GlobalToClient(elRect));
                bgBrush.Dispose();
                bgBrush = null;
            }

            base.DrawElementInternal(e);
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from an array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();
        }

        /// <summary>
        /// Overridden. Sets the first format with white background color.
        /// </summary>
        /// <param name="formats">Array of formats.</param>
        protected override void SetFirstFormat(ArrayList formats)
        {
            if (formats == null)
                throw new ArgumentNullException("formats");

            HTMLFormat firstFormat = (this.Control.DefaultFormat as HTMLFormat).Clone();
            firstFormat.BackgroundColor = Color.Transparent; ////SystemColors.Window;
            firstFormat.Merge = MergeMask.BgColor;
            formats.Insert(0, firstFormat);
        }

        #endregion

        #region Class Event Catchers
        /// <summary>
        /// Raises the load event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseOnLoadEvent(EventArgs args)
        {
            if (OnLoad != null)
            {
                OnLoad(this, args);
            }
        }

        /// <summary>
        /// Raises the unload event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseOnUnLoadEvent(EventArgs args)
        {
            if (OnUnLoad != null)
            {
                OnUnLoad(this, args);
            }
        }

        /// <summary>
        /// Raises the scroll event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseOnScrollEvent(EventArgs args)
        {
            if (OnScroll != null)
            {
                OnScroll(this, args);
            }
        }

        /// <summary>
        /// Raises the resize event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseOnResizeEvent(EventArgs args)
        {
            if (OnResize != null)
            {
                OnResize(this, args);
            }
        }

        #endregion

        #region Class utility methods
        /// <summary>
        /// Resizes control if needed (Add scroll).
        /// </summary>
        private void ResizeControl()
        {
            if (this.Control.AutoScroll)
            {
                this.Control.Recalculate = false;
                Size scrollSize = new Size(this.Width, this.Height);
                scrollSize.Width = Math.Max(scrollSize.Width, this.Document.AutoScrollMinSize.Width);
                scrollSize.Height = Math.Max(scrollSize.Height, this.Document.AutoScrollMinSize.Height);

                this.Document.AutoScrollMinSize = scrollSize;
                this.Control.Recalculate = true;
            }
        }

        /// <summary>
        /// Returns the value of the margin with the specified name of the attribute.
        /// </summary>
        /// <param name="attrName">Name of the attribute.</param>
        /// <returns>Integer margin value</returns>
        private int GetMargin(string attrName)
        {
            if (attrName == null)
                throw new ArgumentNullException("attrName");
            if (attrName.Length == 0)
                throw new ArgumentException("attrName - string can not be empty");

            int marginVal = GetDefaultMargin(attrName);

            IHTMLAttribute attr = this.Attributes[attrName];

            if (attr != null)
            {
                string attrVal = attr.Value;

                double result;

                if (double.TryParse(attrVal, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                {
                    marginVal = (int)result;
                }
            }

            return marginVal;
        }

        /// <summary>
        /// Sets a new value to the margin attribute of the element.
        /// </summary>
        /// <param name="attrName">Name of the attribute.</param>
        /// <param name="marginVal">Value of the margin.</param>
        private void SetMargin(string attrName, int marginVal)
        {
            if (attrName == null)
                throw new ArgumentNullException("attrName");
            if (attrName.Length == 0)
                throw new ArgumentException("attrName - string can not be empty");

            int oldVal = GetMargin(attrName);

            if (oldVal != marginVal)
            {
                string strVal = marginVal.ToString(CultureInfo.InvariantCulture);
                IHTMLAttribute attr = this.Attributes[attrName];

                if (attr == null)
                {
                    attr = new HTMLAttributeImpl(this, attrName, strVal);
                    this.Attributes.Add(attr);
                }
                else
                {
                    attr.Value = strVal;
                }
            }
        }

        /// <summary>
        /// Returns the default value of margin's attribute of the element.
        /// </summary>
        /// <param name="attrName">Name of the attribute.</param>
        /// <returns>Default value of margin's attribute of the element.</returns>
        private int GetDefaultMargin(string attrName)
        {
            if (attrName == null)
                throw new ArgumentNullException("attrName");
            if (attrName.Length == 0)
                throw new ArgumentException("attrName - string can not be empty");

            int marginVal = 0;

            switch (attrName)
            {
                case AttributeName.LeftMargin:
                    if (this.Control.NeedDefaultMargin)
                        marginVal = DEF_LEFT_MARGIN;
                    else
                        marginVal = DEF_LEFT_MARGIN - 10;
                    break;
                case AttributeName.TopMargin:
                    if (this.Control.NeedDefaultMargin)
                        marginVal = DEF_TOP_MARGIN;
                    else
                        marginVal = DEF_TOP_MARGIN - 15;
                    break;
                case AttributeName.RightMargin:
                    if (this.Control.NeedDefaultMargin)
                        marginVal = DEF_RIGHT_MARGIN;
                    else
                        marginVal = DEF_RIGHT_MARGIN - 10;
                    break;
                case AttributeName.BottomMargin:
                    if (this.Control.NeedDefaultMargin)
                        marginVal = DEF_BOTTOM_MARGIN;
                    else
                        marginVal = DEF_BOTTOM_MARGIN - 15;
                    break;
            }

            return marginVal;
        }
        #endregion
    }
}