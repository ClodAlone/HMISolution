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
    /// Class that is responsible for &lt;FONT&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Font)]
    public class FONTElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Font;

        /// <summary>
        /// Default size of the element's font.
        /// </summary>
        private const int DEF_SIZE = 3;

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

        /// <summary>
        /// Overridden. Own format for font. (Sets the size of the font if needed).
        /// </summary>
        protected internal override HTMLFormat OwnFormat
        {
            get
            {
                if (!this.Attributes.Contains(AttributeName.Size)) return base.OwnFormat;

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
        /// Initializes static members of the FONTElementImpl class 
        /// </summary>
        static FONTElementImpl()
        {
            Type type = typeof(FONTElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the FONTElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public FONTElementImpl(IHTMLElement parent)
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
        /// Overridden. Creates the format from attributes.
        /// </summary>
        /// <returns>Format created from attributes.</returns>
        protected override HTMLFormat GetFormatFromAttributes()
        {
            HTMLFormat attrFormat = base.GetFormatFromAttributes();

            // Gets font family from attributes.
            if (this.Attributes.Contains(AttributeName.Face))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.Face];

                if (attr != null)
                {
                    this.Control.FormatManager.SetFontFamily(attrFormat, attr.Value);
                    attrFormat.Merge |= MergeMask.FontFamily;
                }
            }

            return attrFormat;
        }

        #endregion

        #region Class utility methods
        /// <summary>
        /// Sets own format to the element. Sets the size of font if needed.
        /// </summary>
        /// <returns>Resulting format.</returns>
        private HTMLFormat SetOwnFormat()
        {
            // Set font size if we have size attribute in element.
            IHTMLAttribute attr = this.Attributes[AttributeName.Size];
            if (attr != null)
            {
                double dSize;
                if (Double.TryParse(attr.Value, NumberStyles.Integer, null, out dSize))
                {
                    HTMLFormat result = base.OwnFormat;
                    int size = (int)dSize;

                    size = (size < 1) ? 1 : size;
                    size = (size > 7) ? 7 : size;

                    float oldSize = (int)this.Control.DefaultFormat.Font.Size;
                    float newSize = oldSize + (size - DEF_SIZE);

                    result.Merge = MergeMask.FontSize;

                    result.IsFontCreated = false;
                    result.FontSize = newSize;
                    m_ownFormat = result;

                    return m_ownFormat;
                }

                return base.OwnFormat;
            }

            return base.OwnFormat;
        }
        #endregion
    }
}