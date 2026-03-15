#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public sealed class RequestNavigateEventArgs : EventArgs
    {
        #region Fields
        Hyperlink hyperlink;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the hyperlink.
        /// </summary>
        /// <value>
        /// The hyperlink.
        /// </value>
        public Hyperlink Hyperlink
        {
            get
            {
                return hyperlink;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestNavigateEventArgs"/> class.
        /// </summary>
        public RequestNavigateEventArgs()
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestNavigateEventArgs"/> class.
        /// </summary>
        /// <param name="hyperlink">The hyperlink.</param>
        internal RequestNavigateEventArgs(Hyperlink hyperlink)
        {
            this.hyperlink = hyperlink;
        }
        #endregion
    }
    public sealed class Hyperlink
    {
        #region Fields
        string navigationLink;
        string bookmark;
        string targetFrame;
        HyperlinkType linkType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the navigation link.
        /// </summary>
        /// <value>
        /// The navigation link.
        /// </value>
        public string NavigationLink
        {
            get
            {
                return navigationLink;
            }
        }
        /// <summary>
        /// Gets the type of the link.
        /// </summary>
        /// <value>
        /// The type of the link.
        /// </value>
        public HyperlinkType LinkType
        {
            get
            {
                return linkType;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Hyperlink"/> class.
        /// </summary>
        /// <param name="fieldBegin">The field begin.</param>
        public Hyperlink(FieldBeginAdv fieldBegin)
        {
            string fieldCode = fieldBegin.GetFieldCode().ToLowerInvariant();
            fieldCode = fieldCode.Substring(fieldCode.IndexOf("hyperlink") + 9);
            ParseFieldValue(fieldCode.Trim(' '));
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Parses the field value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="switchLevel">The switch level.</param>
        /// <param name="endChar">The end char.</param>
        private void ParseFieldValue(ref string value, int switchLevel, string endChar)
        {
            value = value.Substring(1);
            int endIndex = value.IndexOf(endChar);
            if (endIndex == -1)
                endIndex = value.Length;
            if (switchLevel == 2)
                bookmark = value.Substring(0, endIndex);
            else if (switchLevel == 1)
            {
                if (targetFrame == null)
                    targetFrame = value.Substring(0, endIndex);
            }
            else
            {
                if (navigationLink == null)
                    navigationLink = value.Substring(0, endIndex);
            }
            value = value.Substring(endIndex + 1).Trim(' ');
        }
        /// <summary>
        /// Parses the field value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void ParseFieldValue(string value)
        {
            byte switchLevel = 0;
            bool skipBookmark = false;
            bool skipTargetFrame = false;
            while (value.Length > 0)
            {
                string currentChar = value.Substring(0, 1);
                switch (currentChar)
                {
                    case "\"":
                        ParseFieldValue(ref value, switchLevel, "\"");
                        switchLevel = 0;
                        break;
                    case "\\l":
                    case "\\t":
                        value = value.Substring(2);
                        if (switchLevel == 1)
                            skipBookmark = true;
                        else if (switchLevel == 2)
                            skipTargetFrame = true;
                        if (currentChar == "\\l")
                            switchLevel = (byte)(skipBookmark ? 0 : 1);
                        else
                            switchLevel = (byte)(skipTargetFrame ? 0 : 2);
                        break;
                    default:
                        ParseFieldValue(ref value, switchLevel, " ");
                        switchLevel = 0;
                        break;
                }
                value = value.Trim(' ');
            }
            navigationLink = navigationLink.Trim(' ');
            if (navigationLink.StartsWith("http://") || navigationLink.StartsWith("https://"))
                linkType = HyperlinkType.Webpage;
            else if (navigationLink.StartsWith("mailto:"))
                linkType = HyperlinkType.Email;
            else if (string.IsNullOrEmpty(navigationLink))
                linkType = HyperlinkType.Bookmark;
            else
            {
                if (navigationLink.StartsWith("www."))
                {
                    navigationLink = "http://" + navigationLink;
                    linkType = HyperlinkType.Webpage;
                }
                else if (navigationLink.Contains("@"))
                {
                    navigationLink = "mailto:" + navigationLink;
                    linkType = HyperlinkType.Email;
                }
                else
                    linkType = HyperlinkType.File;
            }
        }
        #endregion
    }

    public enum HyperlinkType
    {
        File,
        Webpage,
        Email,
        Bookmark
    }
}
