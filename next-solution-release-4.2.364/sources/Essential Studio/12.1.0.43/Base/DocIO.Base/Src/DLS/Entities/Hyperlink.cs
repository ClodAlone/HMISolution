#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#region File using directives
using System;

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for Hyperlink.
    /// </summary>
    public class Hyperlink
    {
        #region Fields
        /// <summary>
        /// Hyperlink field
        /// </summary>
        private WField m_hyperlink;
        /// <summary>
        /// Type of the hyperlink
        /// </summary>
        private HyperlinkType m_type;
        /// <summary>
        /// File path.
        /// </summary>
        private string m_filePath;
        /// <summary>
        /// Url link.
        /// </summary>
        private string m_uriPath;
        /// <summary>
        /// Bookmark.
        /// </summary>
        private string m_bookmark;
        /// <summary>
        /// Text which will be displayed on the place of hyperlink
        /// </summary>
        private string m_textToDisplay;
        /// <summary>
        /// Hyperlink image
        /// </summary>
        private WPicture m_picToDisplay;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets / sets file path.
        /// </summary>
        public string FilePath
        {
            get
            {
                return m_filePath.Replace("\"", string.Empty);
            }
            set
            {
                SetFilePath(value);
            }
        }
        /// <summary>
        /// Gets / sets url link. 
        /// </summary>
        public string Uri
        {
            get
            {
                return m_uriPath.Replace("\"", string.Empty);
            }
            set
            {
                SetUri(value);
                m_uriPath = value;
            }
        }
        /// <summary>
        /// Gets / sets bookmark.
        /// </summary>
        public string BookmarkName
        {
            get
            {
                if (m_bookmark != null)
                {
                    return m_bookmark.Replace("\"", string.Empty);
                }
                else
                    return null;
            }
            set
            {
                SetBookmarkName(value);
                m_bookmark = value;
            }
        }
        /// <summary>
        /// Gets / sets a HyperlinkType object that indicates the link type. 
        /// </summary>
        public HyperlinkType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
                UpdateType();
            }
        }
        /// <summary>
        /// Gets or sets the text which will be displayed on the place of hyperlink.
        /// </summary>
        /// <value>The text to display.</value>
        public string TextToDisplay
        {
            get
            {
                return m_textToDisplay;
            }
            set
            {
                m_textToDisplay = value;
                SetTextToDisplay();
            }
        }
        /// <summary>
        /// Gets or sets the image which will be displayed on the place of hyperlink.
        /// </summary>
        /// <value>The image.</value>
        public WPicture PictureToDisplay
        {
            get
            {
                return m_picToDisplay;
            }
            set
            {
                m_picToDisplay = value;
                SetImageToDisplay();
            }
        }
        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>The field.</value>
        internal WField Field
        {
            get
            {
                return m_hyperlink;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Hyperlink"/> class.
        /// </summary>
        /// <param name="hyperlink">The hyperlink.</param>
        public Hyperlink(WField hyperlink)
        {
            CheckHyperlink(hyperlink);
            m_hyperlink = hyperlink;
            Parse();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Performes hyperlink field validation.
        /// </summary>
        /// <param name="field">The field.</param>
        private void CheckHyperlink(WField field)
        {
            if (field == null)
                throw new ArgumentException("Argument is not a field", "hyperlink");

            if (field.FieldType != FieldType.FieldHyperlink)
                throw new ArgumentException("Argument is not a hyperlink", "hyperlink");
        }
        /// <summary>
        /// Parses the hyperlink
        /// </summary>
        private void Parse()
        {
            string value = m_hyperlink.FieldValue;

            if (value == null || value == string.Empty)
                return;

            if (value.StartsWith("\"http"))
            {
                m_type = HyperlinkType.WebLink;
                m_uriPath = value;
            }
            else if (value.StartsWith("\"mailto"))
            {
                m_type = HyperlinkType.EMailLink;
                m_uriPath = value;
            }
            else if (m_hyperlink.IsLocal || m_hyperlink.FormattingString.IndexOf("l") != -1)
            {
                m_type = HyperlinkType.Bookmark;
                if (m_hyperlink.LocalReference != null && m_hyperlink.LocalReference != string.Empty)
                {
                    m_bookmark = m_hyperlink.LocalReference;
                    m_filePath = value;
                }
                else
                {
                    m_bookmark = value;
                }
            }
            else
            {
                m_type = HyperlinkType.FileLink;
                m_filePath = value;
            }

            UpdateTextToDisplay();
        }
        /// <summary>
        /// Updates the text to dispay.
        /// </summary>
        private void UpdateTextToDisplay()
        {
            if (m_hyperlink.OwnerParagraph == null)
                return;

            WParagraph ownerPara = m_hyperlink.OwnerParagraph;
            int itemIndex = FindHyperlinkText(ref ownerPara);

            if (itemIndex == -1)
                return;

            ParagraphItem item = ownerPara.Items[itemIndex];

            while (!(item is WFieldMark))
            {
                item = ownerPara.Items[itemIndex];
                if (item is WTextRange && !(item is WField))
                {
                    m_textToDisplay += (item as WTextRange).Text;
                }
                else if (item is WPicture)
                {
                    m_picToDisplay = item as WPicture;
                }
                itemIndex += 1;

                if (ownerPara.Items.Count <= itemIndex)
                {
                    if (ownerPara.NextSibling != null && ownerPara.NextSibling is WParagraph)
                    {
                        ownerPara = ownerPara.NextSibling as WParagraph;
                        while (ownerPara.NextSibling != null && ownerPara.NextSibling is WParagraph
                            && ownerPara.Items.Count == 0)
                        {
                            ownerPara = ownerPara.NextSibling as WParagraph;
                        }
                        if (ownerPara.LastItem is WField)
                            itemIndex = 1;
                        else
                            itemIndex = 0;
                    }
                    else
                    {
                        break;
                    }
                }

                item = ownerPara.Items[itemIndex];
            }
        }
        /// <summary>
        /// Sets the text to display.
        /// </summary>
        private void SetTextToDisplay()
        {
            if (m_hyperlink.OwnerParagraph == null)
                return;

            WParagraph ownerPara = m_hyperlink.OwnerParagraph;
            int textIndex = FindHyperlinkText(ref ownerPara);
            RemoveHLItems(ownerPara, textIndex);

            if (ownerPara.Items[textIndex] is WTextRange)
            {
                (ownerPara.Items[textIndex] as WTextRange).Text = m_textToDisplay;
            }
            else if (ownerPara.Items[textIndex] is WPicture)
            {
                WTextRange text = new WTextRange(ownerPara.Document);
                text.Text = m_textToDisplay;
                ownerPara.Items.RemoveAt(textIndex);
                ownerPara.Items.Insert(textIndex, text);
            }
        }
        /// <summary>
        /// Sets the image to display.
        /// </summary>
        private void SetImageToDisplay()
        {
            if (m_hyperlink.OwnerParagraph == null)
                return;

            WParagraph ownerPara = m_hyperlink.OwnerParagraph;
            int textIndex = FindHyperlinkText(ref ownerPara);
            RemoveHLItems(ownerPara, textIndex);

            ownerPara.Items.RemoveAt(textIndex);
            ownerPara.Items.Insert(textIndex, m_picToDisplay);
        }
        /// <summary>
        /// Removes the hyperlink items.
        /// </summary>
        /// <param name="ownerPara">The owner paragraph.</param>
        /// <param name="itemIndex">Index of the item.</param>
        private void RemoveHLItems(WParagraph ownerPara, int itemIndex)
        {
            if (!(ownerPara.Items[itemIndex + 1] is WFieldMark))
            {
                while (!(ownerPara.Items[itemIndex + 1] is WFieldMark))
                {
                    ownerPara.Items.RemoveAt(itemIndex + 1);
                }
            }
        }
        /// <summary>
        /// Finds the first item of hyperlink text range.
        /// </summary>
        /// <returns></returns>
        private int FindHyperlinkText(ref WParagraph ownerPara)
        {
            // After field item goes feild separator and then field text.
            Entity ent  = m_hyperlink as Entity;
            int i=0;
            while (ent.NextSibling != null)
            {
                //Returns if no text is preserved for the hyperlink. (Between field separator and end mark).
                if ((ent is WFieldMark)
                    && (ent as WFieldMark).Type == FieldMarkType.FieldSeparator
                    && (ent.NextSibling is WFieldMark)
                    && (ent.NextSibling as WFieldMark).Type == FieldMarkType.FieldEnd)
                    return -1;
                if (ent.NextSibling is InlineShapeObject || ent.NextSibling is WFieldMark)
                    i++;
                else if (ent.NextSibling is WTextRange || ent.NextSibling is WPicture)
                {
                    i++;
                    break;
                }
                if (i > 3)
                {
                    i = -1;
                    break;
                }

                ent = ent.NextSibling as Entity;
            }
            int textIndex = m_hyperlink.GetIndexInOwnerCollection() + i;
            if (ownerPara.Items.Count <= textIndex)
            {
                if (ownerPara.NextSibling != null && ownerPara is WParagraph)
                {
                    if (ownerPara.LastItem is WField)
                        textIndex = 1;
                    else
                        textIndex = 0;
                    ownerPara = ownerPara.NextSibling as WParagraph;
                }
                else
                {
                    return -1;
                }
            }
            ParagraphItem item = ownerPara.Items[textIndex];

            if (item is WTextRange || item is WPicture)
            {
                return textIndex;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// Updates the hyperlink type.
        /// </summary>
        private void UpdateType()
        {
            if (m_type == HyperlinkType.Bookmark)
            {
                m_hyperlink.IsLocal = true;
            }
            else
            {
                m_hyperlink.LocalReference = null;
                m_hyperlink.m_formattingString = string.Empty;
            }

        }
        /// <summary>
        /// Sets the URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        private void SetUri(string uri)
        {
            if (m_type != HyperlinkType.WebLink && m_type != HyperlinkType.EMailLink)
            {
                throw new ArgumentException("Uri can be set only for \"WebLink\" or \"EMailLink\" types of hyperlink");
            }

            uri = CheckUri(uri);
            uri = CheckValue(uri);
            if (m_hyperlink.FieldCode != string.Empty && m_hyperlink.FieldCode.Contains(m_hyperlink.m_fieldValue))
                m_hyperlink.FieldCode = m_hyperlink.FieldCode.Replace(m_hyperlink.m_fieldValue, uri);
            m_hyperlink.m_fieldValue = uri;
        }
        /// <summary>
        /// Sets the name of the bookmark.
        /// </summary>
        /// <param name="name">The name.</param>
        private void SetBookmarkName(string name)
        {
            if (m_type != HyperlinkType.Bookmark)
            {
                throw new ArgumentException("Bookmark name can be set only for \"Bookmark\" type of hyperlink");
            }
            name = CheckValue(name);
            if (m_hyperlink.FieldCode != string.Empty && m_hyperlink.FieldCode.Contains(m_hyperlink.m_fieldValue))
                m_hyperlink.FieldCode = m_hyperlink.FieldCode.Replace(m_hyperlink.m_fieldValue, name);
            m_hyperlink.m_fieldValue = name;
        }
        /// <summary>
        /// Sets the file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void SetFilePath(string filePath)
        {
            if (m_type != HyperlinkType.FileLink && m_type != HyperlinkType.Bookmark)
            {
                throw new ArgumentException("File path can be set only for \"FileLink\" or \"Bookmark\" type of hyperlink");
            }

            filePath = CheckPath(filePath);
            filePath = CheckValue(filePath);
            if (m_hyperlink.FieldCode != string.Empty &&  m_hyperlink.FieldCode.Contains(m_hyperlink.m_fieldValue))
                m_hyperlink.FieldCode = m_hyperlink.FieldCode.Replace(m_hyperlink.m_fieldValue, filePath);
            m_hyperlink.m_fieldValue = filePath;
            if (m_type == HyperlinkType.Bookmark)
            {
                if (m_bookmark == null || m_bookmark == string.Empty)
                    throw new ArgumentException("Bookmark name can't be null or empty. Bookmark name must be set before file path.");

                m_hyperlink.LocalReference = CheckValue(m_bookmark);
            }
            m_filePath = filePath;
        }
        /// <summary>
        /// Checks the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string CheckValue(string value)
        {
            if (!value.StartsWith("\""))
            {
                value = "\"" + value;
            }

            if (!value.EndsWith("\""))
            {
                value = value + "\"";
            }

            return value;
        }
        /// <summary>
        /// Checks the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string CheckPath(string path)
        {
            if (!path.StartsWith(@"file:///"))
                path = @"file:///" + path;
            char[] separator = new char[1] { '\\' };
            string[] pathParts = path.Split(separator);
            path = string.Empty;

            for (int i = 0, cnt = pathParts.Length; i < cnt; i++)
            {
                if (pathParts[i] != string.Empty)
                {
                    path += pathParts[i];
                    if (i < cnt - 1)
                    {
                        path += @"\\";
                    }
                }
            }

            return path;
        }
        /// <summary>
        /// Checks the URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        private string CheckUri(string uri)
        {
            uri = uri.Replace("\"", string.Empty);

            if (m_type == HyperlinkType.WebLink)
            {
                if (!uri.Contains("http://") && !uri.Contains("https://"))
                {
                    if(uri.ToLower().StartsWith("www."))
                    {                        
                        uri = "http://" + uri;
                    }
                    else if (uri.Contains("@"))
                    {
                        uri = "mailto:" + uri;
                    }
                }
            }
            else
            {
                uri = "mailto:" + uri;
            }
            return uri;
        }
        #endregion
    }
}
