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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.HTMLUI.Base;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;A&gt; Tag.
    /// </summary>
    [ElementTag(TagName.A)]
    public class AElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.A;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        /// Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class members

        /// <summary>
        /// Type of destination document.
        /// </summary>
        private ResourceType m_destinationType;

        /// <summary>
        /// Format indicating :hover pseudo-class.
        /// </summary>
        private IHTMLFormat m_hoverFormat;

        /// <summary>
        /// Format indicating :visited pseudo-class.
        /// </summary>
        private IHTMLFormat m_visitedFormat;

        /// <summary>
        /// Format storing previous main format when mouse enters and leaves.
        /// </summary>
        private IHTMLFormat m_tempFormat;
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
        /// Gets the format object containing :hover pseudo-class style.
        /// </summary>
        [Browsable(false)]
        public IHTMLFormat HoverFormat
        {
            get
            {
                return m_hoverFormat;
            }
        }

        /// <summary>
        /// Gets the format object containing :visited pseudo-class style.
        /// </summary>
        [Browsable(false)]
        public IHTMLFormat VisitedFormat
        {
            get
            {
                return m_visitedFormat;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the link is visited.
        /// </summary>
        [Browsable(false)]
        public bool IsVisited
        {
            get
            {
                string path = GetPath();
                bool result = false;

                if (path != null)
                {
                    result = this.Control.History.Contains(path);
                }

                return result;
            }
        }

        /// <summary>
        /// Overridden. Gets or sets the format which is special for tag A (hyperlink).
        /// </summary>
         protected internal override HTMLFormat OwnFormat
        {
            get
            {
                if (m_ownFormat == null)
                {
                    m_ownFormat = base.OwnFormat;

                    if (this.IsLink)
                    {
                        m_ownFormat.Merge |= MergeMask.Cursor;
                        m_ownFormat.Cursor = Cursors.Hand;

                        m_ownFormat.Merge |= MergeMask.TextDecoration;
                        m_ownFormat.TextDecoration = FontStyle.Underline;

                        m_ownFormat.Merge |= MergeMask.ForeColor;
                        m_ownFormat.ForeColor = Color.Blue;
                    }
                }

                return m_ownFormat;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the link has href attribute.
        /// </summary>
        private bool IsLink
        {
            get
            {
                IHTMLAttribute hrefAttr = this.Attributes[AttributeName.Href];
                return hrefAttr != null;
            }
        }
        #endregion

        #region Class events

        /// <summary>
        /// Raised before forwarding on another resource by defined URI.
        /// </summary>
        public event LinkForwardEventHandler Forward;
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes static members of the AElementImpl class 
        /// </summary>
        static AElementImpl()
        {
            Type type = typeof(AElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the AElementImpl class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        public AElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
            m_destinationType = ResourceType.Unknown;
        }

        /// <summary>
        /// Overridden. Disposes resources.
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            if (m_hoverFormat != null)
            {
                (m_hoverFormat as HTMLFormat).Dispose();
                m_hoverFormat = null;
            }

            if (m_visitedFormat != null)
            {
                (m_visitedFormat as HTMLFormat).Dispose();
                m_visitedFormat = null;
            }

            if (this.Control != null)
            {
                this.Forward -= new LinkForwardEventHandler(this.Control.OnLinkClicked);
            }

            m_tempFormat = null;
        }
        #endregion

        #region Class Public Methods

        /// <summary>
        /// Excludes element from visited links list.
        /// </summary>
        public void ResetVisited()
        {
            this.Control.History.ExcludeFromVisited(this);
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
        /// Calculates the format of the element from an array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();

            CalculateOtherFormats();

            // Define if link is visited or not.
            string url = GetPath();

            if (url != null && url.Length > 0 && this.Control.History.Contains(url))
            {
                SetFormat(this.VisitedFormat as HTMLFormat);
            }
        }

        /// <summary>
        /// Overridden. Initializes element's properties.
        /// </summary>
        protected internal override void InitializeElement()
        {
            base.InitializeElement();

            if (!this.Attributes.Contains(AttributeName.TabIndex) && this.IsLink)
            {
                this.TabIndex = 0;
            }

            this.Forward += new LinkForwardEventHandler(this.Control.OnLinkClicked);
        }

        /// <summary>
        /// Raises the <see cref="Forward"/> event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnForward(LinkForwardEventArgs args)
        {
            RaiseForward(args);
        }
        #endregion

        #region Class event raisers

        /// <summary>
        /// Raises <see cref="Forward"/> event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected void RaiseForward(LinkForwardEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            if (Forward != null)
            {
                Forward(this, args);
            }
        }
        #endregion

        #region Class Event Catchers

        /// <summary>
        /// Overridden. Implement on click functionality.
        /// </summary>
        /// <param name="args">Event arguments.</param>
       protected internal override void RaiseClickEvent(EventArgs args)
        {
            base.RaiseClickEvent(args);

            if (this.Control.ClickedButton == MouseButtons.Left && this.IsLink)
            {
                string url = GetPath();
                string fullPath;

                // Raise Forward event.
                LinkForwardEventArgs fArgs = new LinkForwardEventArgs(url);
                OnForward(fArgs);

                url = fArgs.Path;

                if (!fArgs.Cancel)
                {
                    //// Try to load resource if is not canceled.
                    if (url.Length > 1 && url[0] == Utilities.DEF_FRAGMENT_PREFIX)
                    {
                        //// if link refers to bookmark in the same document.
                        this.Control.JumpToFragment(url, this.Document);
                    }                   
                    else if (IsGoodPath(url, out fullPath))
                    {
                        //// try to load another document.
                        ForwardResource(fullPath);
                    }
                }
            }
        }

        /// <summary>
        /// Overridden. Raised when mouse enters the element.
        /// </summary>
        /// <param name="args">Event data.</param>
        protected internal override void RaiseMouseEnterEvent(EventArgs args)
        {
            base.RaiseMouseEnterEvent(args);

            if (this.Format != this.HoverFormat && this.IsLink)
            {
                m_tempFormat = this.Format;

                SetFormat(this.HoverFormat as HTMLFormat);

                this.Control.Invalidate();
            }
        }

        /// <summary>
        /// Overridden. Raised when mouse leaves the element.
        /// </summary>
        /// <param name="args">Event data.</param>
        protected internal override void RaiseMouseLeaveEvent(EventArgs args)
        {
            base.RaiseMouseLeaveEvent(args);

            if (this.Format != m_tempFormat && this.IsLink)
            {
                if (m_tempFormat != null)
                {
                    SetFormat(m_tempFormat as HTMLFormat);
                    m_tempFormat = null;
                }

                this.Control.Invalidate();
            }
        }
        #endregion

        #region Class Utility methods

        /// <summary>
        /// Returns a string which represents the path (value of href attribute) if it exists; NULL otherwise.
        /// </summary>
        /// <returns>Link path.</returns>
        protected internal string GetPath()
        {
            if (!this.Attributes.Contains(AttributeName.Href)) return null;

            IHTMLAttribute attr = this.Attributes[AttributeName.Href];
            string result = string.Empty;

            if (attr.Value != null && attr.Value.Length > 0)
            {
                result = attr.Value;
            }

            return result;
        }

        /// <summary>
        /// Indicates whether the URL is valid.
        /// </summary>
        /// <param name="path">Link path.</param>
        /// <param name="fullPath">If resource found - holds full path to it.</param>
        /// <returns>True if resource found.</returns>
        private bool IsGoodPath(string path, out string fullPath)
        {
            bool bExists = false;
            fullPath = null;

            if (path != null && path.Length > 0)
            {
                IResourceProvider dataProvider = this.Document.DataSource.DefaultDataProvider;
                bExists = dataProvider.ResourceExists(path, out fullPath, out m_destinationType);
            }

            return bExists;
        }

        /// <summary>
        /// Reloads new document in the control.
        /// </summary>
        /// <param name="path">Path of the link.</param>
        private void ForwardResource(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("path - string can not be empty");

            if (m_destinationType == ResourceType.LocalResource)
            {
                this.Control.LoadHTML(path);
            }
            else if (m_destinationType == ResourceType.RemoteResource)
            {
                Uri uri = new Uri(path);
                this.Control.LoadHTML(uri);
            }
        }

        /// <summary>
        /// Calculcates additional formats for the element such as: hover, visited.
        /// </summary>
        private void CalculateOtherFormats()
        {
            Hashtable formatsHash = this.Document.GetCSSFormatsToElementHash();

            ////Calculate format :hover.
            string hoverKey = this.UniqueID + FormatManager.DEF_PSEUDO_HOVER;
            ArrayList formats = GetFormatsArray(formatsHash, hoverKey);
            HTMLFormat formatEx = this.Format as HTMLFormat;

            if (formats.Count > 0)
            {
                HTMLFormat hoverFormat = HTMLFormat.MergeFormats(formats, formatEx, formatEx);
                hoverFormat.Type = FormatType.Merged;
                hoverFormat.Name = hoverKey + "_" + this.Name;
                hoverFormat.IsMerged = true;
                hoverFormat.QuietMode = false;
                m_hoverFormat = hoverFormat;
            }
            else
            {
                m_hoverFormat = this.Format;
            }

            ////Calculate format :visited.
            string visitedKey = this.UniqueID + FormatManager.DEF_PSEUDO_VISITED;
            formats = GetFormatsArray(formatsHash, visitedKey);

            if (formats.Count > 0)
            {
                HTMLFormat visitedFormat = HTMLFormat.MergeFormats(formats, formatEx, formatEx);
                visitedFormat.Type = FormatType.Merged;
                visitedFormat.Name = visitedKey + "_" + this.Name;
                visitedFormat.IsMerged = true;
                visitedFormat.QuietMode = false;
                m_visitedFormat = visitedFormat;
            }
            else
            {
                m_visitedFormat = this.Format;
            }
        }

        /// <summary>
        /// Searchs and returns an array of formats for the element with the specified key.
        /// </summary>
        /// <param name="formatsHash">Dictionary of all formats.</param>
        /// <param name="key">Key of the array in the dictionary.</param>
        /// <returns>Array of formats for the element with the specified key.</returns>
        private ArrayList GetFormatsArray(IDictionary formatsHash, string key)
        {
            if (formatsHash == null)
                throw new ArgumentNullException("formatsHash");

            if (key == null)
                throw new ArgumentNullException("key");

            object result = formatsHash[key];

            if (result == null)
            {
                result = new ArrayList();
                formatsHash[key] = result;
            }

            return result as ArrayList;
        }
        #endregion
    }
}