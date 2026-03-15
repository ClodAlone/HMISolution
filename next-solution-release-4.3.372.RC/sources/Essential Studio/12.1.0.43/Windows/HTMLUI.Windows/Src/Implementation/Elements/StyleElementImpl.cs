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
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Syncfusion.HTMLUI.Base;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;STYLE&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Style)]
    public class StyleElementImpl : BaseElement, IElementHasCss
    {
        #region Class constants

        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Style;

        /// <summary>
        /// Pattern for comments.
        /// </summary>
        private const string DEF_COMMENTS = "<!--|-->";

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        /// Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class Static members
        /// <summary>
        /// For deleting possible commas after values.
        /// </summary>
        private static Regex m_regDelComments = new Regex(DEF_COMMENTS, DEF_REGEX_OPTIONS);
        #endregion

        #region Class members
        /// <summary>
        /// The array of formats created from this element.
        /// </summary>
        private IHTMLFormat[] m_createdFormats;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets or sets the array of formats created from this element.
        /// </summary>
        IHTMLFormat[] IElementHasCss.CreatedFormats
        {
            get
            {
                return m_createdFormats;
            }
            set
            {
                m_createdFormats = value;
            }
        }

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
        /// Gets or sets a value indicating whether the element is visible or not
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
                    //// throw new ArgumentException( "Visibility of this element must be False.", "IsVisible" );

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
        /// Initializes static members of the StyleElementImpl class 
        /// </summary>
        static StyleElementImpl()
        {
            Type type = typeof(StyleElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the StyleElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public StyleElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        /// <summary>
        /// Disposes the element.
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            m_createdFormats = null;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Returns the stream of inner data of the tag element. Data represents CSS.
        /// </summary>
        /// <returns>Stream data by the URL.</returns>
        public TokenStream GetCssStream()
        {
            TokenStream ts = null;

            try
            {
                string cssData = m_regDelComments.Replace(this.Storage.InnerText, string.Empty);
                ts = TokenStream.FromString(cssData);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Can't load CSS Document! Details: " +
                  e.Message + Environment.NewLine +
                  e.StackTrace);

                throw;
            }

            return ts;
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
            return null;
        }

        /// <summary>
        /// Overidden. Returns the CSS data of this element in stream.
        /// </summary>
        /// <returns>Size of the element.</returns>
        protected override Size CalculateSizeInternal()
        {
            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            // This method is not called from anywhere. Element is not displayable.
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            //// DefaultCalculateFormatInternal();

            this.IsVisible = false;
        }

        /// <summary>
        /// Overriden. Reparses content when inner html was changed.
        /// </summary>
        /// <param name="htmlToParse">New inner html of the element.</param>
        protected override void ReparseInnerHTML(string htmlToParse)
        {
            // NOTE: Here we don't have to use the default logic since inside of this element
            // we should get CSS style.
            // Set the inner html data.
            if (!this.IsCDATA)
            {
                m_storage.InnerXml = htmlToParse;
            }
            else
            {
                XmlCDataSection sect = m_storage.FirstChild as XmlCDataSection;

                sect.Data = htmlToParse;
            }

            // Recreate formats.
            ReFormatCrtDocAndReCalcDoc(this);
        }
        #endregion
    }
}
