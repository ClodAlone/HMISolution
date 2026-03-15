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
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Syncfusion.HTMLUI.Base;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;LINK&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Link)]
    public class LinkElementImpl
      : BaseElement, IElementHasCss
    {
        #region Class constants

        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Link;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        /// Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;

        /// <summary>
        /// Holds the type of reaction that occur when attributes change.
        /// </summary>
        private static ReactionCollection m_reactionType;
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
        /// Overridden. Returns an hashtable that contains names of attributes as keys and types of reaction that
        /// occur on change in attributes as values.
        /// </summary>
        internal override ReactionCollection Reaction
        {
            get
            {
                return m_reactionType;
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
                    //// throw new ArgumentException( "Visibility of this element must be False.", "IsVisible" );

                    if (!this.Attributes.Contains(DEF_RUNTIME_VISIBLE))
                    {
                        this.Attributes.Add(DEF_RUNTIME_VISIBLE);
                        ((HTMLAttributeImpl)this.Attributes[DEF_RUNTIME_VISIBLE]).Value = false.ToString();
                    }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Redefines some attribute reaction of control.
        /// </summary>
        private static void ReDefineReactions()
        {
            m_reactionType = (ReactionCollection)m_reaction.Clone();

            string typeName = typeof(HTMLAttributesCollection).ToString();

            m_reactionType.Add(typeName, AttributeName.Href, ReactType.ReFormatCrtDocAndReCalcDoc);

            m_reactionType.Add(typeName, AttributeName.Rel, ReactType.ReFormatCrtDocAndReCalcDoc);

            m_reactionType.Add(typeName, AttributeName.Type, ReactType.ReFormatCrtDocAndReCalcDoc);
        }

        /// <summary>
        /// Initializes static members of the LinkElementImpl class 
        /// </summary>
        static LinkElementImpl()
        {
            Type type = typeof(LinkElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);

            ReDefineReactions();
        }

        /// <summary>
        /// Initializes a new instance of the LinkElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public LinkElementImpl(IHTMLElement parent)
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
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size instance</returns>
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
        /// Overriden. Returns the CSS data of this element in stream.
        /// </summary>
        /// <returns>Token stream by link URL.</returns>
        public TokenStream GetCssStream()
        {
            if (!this.Storage.HasAttribute(AttributeName.Rel) ||
                          !this.Storage.HasAttribute(AttributeName.Href) ||
                          !this.Storage.HasAttribute(AttributeName.Type))
            {
                return null;
            }

            string RelAttr = this.Storage.GetAttribute(AttributeName.Rel).ToLower();
            string HrefAttr = this.Storage.GetAttribute(AttributeName.Href);
            string TypeAttr = this.Storage.GetAttribute(AttributeName.Type).ToLower();

            if (RelAttr != "stylesheet" ||
                TypeAttr != "text/css")
            {
                return null;
            }

            TokenStream ts = null;

            string fullPath;
            ResourceType type = ResourceType.Unknown;
            DataSource dataSource = this.Document.DataSource;

            if (dataSource.ResourceExists(HrefAttr, out fullPath, out type))
            {
                Stream stream = dataSource.GetResource(fullPath, type);

                if (stream != null)
                {
                    ts = new TokenStream(stream);
                }
            }

            return ts;
        }
        #endregion
    }
}
