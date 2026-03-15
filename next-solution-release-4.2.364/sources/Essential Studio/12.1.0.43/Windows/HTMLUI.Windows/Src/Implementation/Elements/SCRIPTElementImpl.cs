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
using Syncfusion.HTMLUI.Base;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;SCRIPT&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Script)]
    public class SCRIPTElementImpl : BaseElement
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Script;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        /// Hold all events.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class Properties
        /// <summary>
        /// Returns an array of supported events (here is NULL).
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return new string[] { };
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

        /// <summary>
        /// Gets the path to the script file.
        /// </summary>
        internal string Path
        {
            get
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.Src];

                if (attr == null) return null;

                return attr.Value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the SCRIPTElementImpl class 
        /// </summary>
        static SCRIPTElementImpl()
        {
            Type type = typeof(SCRIPTElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the SCRIPTElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public SCRIPTElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        #endregion

        #region Class Public Methods
        /// <summary>
        /// Overloaded. Returns the script code of the element.
        /// </summary>
        /// <returns>Script code.</returns>
        public string GetScriptCode()
        {
            bool fromFile;
            return GetScriptCode(out fromFile);
        }

        /// <summary>
        /// Returns the script code of the element.
        /// </summary>
        /// <param name="isFromFile">Returns value which indicates if code has been extracted from external file.</param>
        /// <returns>Script code.</returns>
        public string GetScriptCode(out bool isFromFile)
        {
            string path = this.Path;

            if (path == null)
            {
                isFromFile = false;

                return this.InnerHTML;
            }

            string result;
            string fullPath;
            ResourceType type = ResourceType.Unknown;
            DataSource dataSource = this.Document.DataSource;

            if (dataSource.ResourceExists(path, out fullPath, out type))
            {
                Stream stream = dataSource.GetResource(fullPath, type);

                if (stream != null)
                {
                    // NOTE: Which encoding is used?
                    using (TextReader tr = new StreamReader(stream))
                    {
                        result = tr.ReadToEnd();
                    }
                    isFromFile = true;
                    stream.Close();
                }
            }

            isFromFile = false;

            return this.InnerHTML;
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns an instance of the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Event name.</returns>
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
            this.Size = Size.Empty;

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            // do not calculate any positions.
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            this.IsVisible = false;
        }
        #endregion
    }
}
