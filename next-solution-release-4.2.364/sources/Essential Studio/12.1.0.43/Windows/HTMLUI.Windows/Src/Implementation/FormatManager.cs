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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.HTMLUI.Base;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which is responsible for creating formats.
    /// </summary>
    public class FormatManager
      : IDisposable
    {
        #region Class constants
        /// <summary>
        /// Suffix name of the CSS in style attribute in HTML elements.
        /// </summary>
        public const string DEF_STYLE_ATTR_FORMAT = "_style";

        /// <summary>
        /// Default format name.
        /// </summary>
        private const string DEF_FORMAT_NAME = "default";

        /// <summary>
        /// Prefix of format's name by element id.
        /// </summary>
        private const string DEF_ID_PREFIX = "#";

        /// <summary>
        /// Name of pseudo-class :link.
        /// </summary>
        internal const string DEF_PSEUDO_LINK = ":link";

        /// <summary>
        /// Name of pseudo-class :hover.
        /// </summary>
        internal const string DEF_PSEUDO_HOVER = ":hover";

        /// <summary>
        /// Name of pseudo-class :visited.
        /// </summary>
        internal const string DEF_PSEUDO_VISITED = ":visited";
        #endregion

        #region Class members
        /// <summary>
        /// Collection of formats.
        /// </summary>
        private HTMLFormatsCollection m_formatsCollection;

        /// <summary>
        /// Holds the CSS in XML document.
        /// </summary>
        private XmlDocument m_document;

        /// <summary>
        /// Instance of the control.
        /// </summary>
        private HTMLUIControl m_control;

        /// <summary>
        /// Holds arrays of formats by their name.
        /// </summary>
        private IDictionary m_formatsByName;

        /// <summary>
        /// Index for each format.
        /// </summary>
        private long m_formatIndex;

        /// <summary>
        /// Indicates whether we were disposed once.
        /// </summary>
        private bool m_bDisposed;

        /// <summary>
        /// Default format.
        /// </summary>
        private HTMLFormat m_defaultFormat;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the collection of formats.
        /// </summary>
        public HTMLFormatsCollection Formats
        {
            get
            {
                return m_formatsCollection;
            }
        }

        /// <summary>
        /// Gets the XML storage of the CSS elements.
        /// </summary>
        public XmlDocument Document
        {
            get
            {
                return m_document;
            }
        }

        /// <summary>
        /// Gets an instance of the control.
        /// </summary>
        public HTMLUIControl Control
        {
            get
            {
                return m_control;
            }
        }

        /// <summary>
        /// Gets the default format of the control.
        /// </summary>
        public IHTMLFormat DefaultFormat
        {
            get
            {
                return m_defaultFormat;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Prevents a default instance of the FormatManager class from being created
        /// </summary>
        private FormatManager()
        {
            m_defaultFormat = new HTMLFormat(DEF_FORMAT_NAME, 0);
            m_formatsCollection = new HTMLFormatsCollection();
            m_formatsByName = CollectionsUtil.CreateCaseInsensitiveHashtable();
            m_formatIndex = 1;
            m_document = null;

            m_formatsCollection.Add(m_defaultFormat);
            m_defaultFormat.IsMerged = true;
            m_defaultFormat.Merge = MergeMask.All;
        }

        /// <summary>
        /// Initializes a new instance of the FormatManager class
        /// </summary>
        /// <param name="control">HTMLUI control</param>
        public FormatManager(HTMLUIControl control)
            : this()
        {
            if (control == null)
                throw new ArgumentNullException("control");

            m_control = control;

            m_defaultFormat.CursorChanged += new ValueChangedEventHandler(CursorChanged);
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Overloaded. Builds formats from file with specified FileName.
        /// </summary>
        /// <param name="fileName">Path to the file with CSS.</param>
        public void LoadCss(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            if (fileName.Length == 0)
                throw new ArgumentException("fileName - string can not be empty...");

            m_document = HTMLUIControl.CSSParser.Parse(fileName);
            BuildFormatsCollection(m_document, m_formatsCollection);
        }

        /// <summary>
        /// Builds formats from specified Stream.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        public void LoadCss(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (stream.Length == 0)
                throw new ArgumentException("stream - Stream can not be empty...");

            TokenStream ts = new TokenStream(stream);
            m_document = HTMLUIControl.CSSParser.Parse(ts);
            BuildFormatsCollection(m_document, m_formatsCollection);
        }

        /// <summary>
        /// Overloaded. Builds formats from file with specified FileName and appends XML nodes to the existing XML document.
        /// </summary>
        /// <param name="fileName">Path to the file with CSS.</param>
        public void AppendCss(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            if (fileName.Length == 0)
                throw new ArgumentException("fileName - string can not be empty...");

            ArrayList formats = new ArrayList();
            XmlDocument document = HTMLUIControl.CSSParser.Parse(fileName);
            BuildFormatsCollection(document, formats);
            MergeFormatsCollections(formats);
        }

        /// <summary>
        /// Builds formats from specified Stream and merges XML documents.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        public void AppendCss(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (stream.Length == 0)
                throw new ArgumentException("stream - Stream can not be empty...");

            TokenStream ts = new TokenStream(stream);

            ArrayList formats = new ArrayList();
            XmlDocument document = HTMLUIControl.CSSParser.Parse(ts);
            BuildFormatsCollection(document, formats);
            MergeFormatsCollections(formats);
        }

        /// <summary>
        /// Removes format from the collection.
        /// </summary>
        /// <param name="format">Format which should be removed from the collection.</param>
        internal void Remove(IHTMLFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            ArrayList formats = m_formatsByName[format.Name] as ArrayList;
            if (formats != null)
            {
                formats.Remove(format);
                if (formats.Count == 0)
                {
                    m_formatsByName.Remove(format.Name);
                }
            }

            Formats.Remove(format);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Invoked when HasCSS event is raised by the HTMLUIParser.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        internal void Element_HasCss(object sender, ElementHasCssEventArgs args)
        {
            Debug.WriteLine("formatManager is going to build Formats ");

            m_document = args.XmlStorge;
            ArrayList formats = new ArrayList();
            BuildFormatsCollection(m_document, formats);

            IHTMLFormat[] arrFormats = (IHTMLFormat[])formats.ToArray(typeof(IHTMLFormat));
            m_formatsCollection.AddRange(arrFormats);

            IElementHasCss hasCss = args.Element as IElementHasCss;
            if (hasCss != null)
            {
                hasCss.CreatedFormats = arrFormats;
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Infills the specified hash by creating formats for the defined element.
        /// </summary>
        /// <param name="hash">Dictionary for format's infilling.</param>
        /// <param name="element">Owner of formats.</param>
        /// <param name="bCalcChildren">Indicates whether to infill formats of children.</param>
        internal void InFillFormatsHash(Hashtable hash, IHTMLElement element, bool bCalcChildren)
        {
            if (hash == null)
                throw new ArgumentNullException("hash");

            if (element == null)
                throw new ArgumentNullException("element");

            if (element is BaseElement == false)
                throw new ArgumentException("element must inherit BaseElement class", "element");

            Queue queue = new Queue();
            queue.Enqueue(element);

            while (queue.Count > 0)
            {
                BaseElement tagElement = queue.Dequeue() as BaseElement;

                if (tagElement == null)
                    throw new ArgumentNullException("tagElement", "each element must inherit BaseElement class");

                // Don't attach formats to tags from HEAD tag.
                if ( /*tagElement.Parent != null &&
          !Utilities.StrEquals( tagElement.Parent.Name, "head" ) &&*/
                  !Utilities.StrEquals(tagElement.Name, "head")
                  )
                {
                    // Add general formats.
                    ArrayList array = new ArrayList();
                    AddToCollection(array, tagElement, string.Empty);

                    // Add format from tag "style" attribute.
                    AttachStyleFormat(tagElement, array);

                    // Store formats for future use.
                    hash[tagElement.UniqueID] = array;

                    // Add additional formats to link elements.
                    AttachLinkFormats(hash, tagElement);

                    tagElement.CalculateFormat();
                }

                if (!bCalcChildren) return;

                // Add for all children.
                if (tagElement.HasChildren)
                {
                    IHTMLElement child = null;

                    for (int i = 0, len = tagElement.Children.Count; i < len; i++)
                    {
                        child = tagElement.Children[i];
                        queue.Enqueue(child);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the format manager with the specified default format.
        /// </summary>
        /// <param name="defaultFormat">New default format object for the format manager.</param>
        internal void SetDefaultFormat(IHTMLFormat defaultFormat)
        {
            if (defaultFormat == null)
                throw new ArgumentNullException("defaultFormat");

            if (m_defaultFormat != null)
            {
                m_defaultFormat.CursorChanged -= new ValueChangedEventHandler(CursorChanged);
            }

            m_formatsCollection.Remove(m_defaultFormat);
            m_defaultFormat = defaultFormat as HTMLFormat;
            m_formatsCollection.Add(m_defaultFormat);
            m_defaultFormat.IsMerged = true;
            m_defaultFormat.CursorChanged += new ValueChangedEventHandler(CursorChanged);
        }

        /// <summary>
        /// Merges formats.
        /// </summary>
        /// <param name="formats">Collection of formats for merging.</param>
        protected void MergeFormatsCollections(IList formats)
        {
            if (formats == null)
                throw new ArgumentNullException("formats");

            IEnumerator enm = formats.GetEnumerator();
            enm.Reset();

            while (enm.MoveNext())
            {
                HTMLFormat format = (HTMLFormat)enm.Current;

                if (m_formatsCollection.Contains(format.Name))
                {
                    format.Storage.ParentNode.RemoveChild(format.Storage);
                    m_formatsCollection.Remove(format);
                }

                XmlNode newElement = m_document.ImportNode(((HTMLFormat)enm.Current).Storage, true);
                m_document.DocumentElement.AppendChild(newElement);
            }
        }

        /// <summary>
        /// Builds collection of formats.
        /// </summary>
        /// <param name="document">XmlDocument instance</param>
        /// <param name="formats">IList instance</param>
        private void BuildFormatsCollection(XmlDocument document, IList formats)
        {
            XmlElement root = document.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;
            XmlNode node = null;

            for (int i = 0, len = nodeList.Count; i < len; i++)
            {
                node = nodeList[i];
                if (node is XmlElement)
                {
                    HTMLFormat format = BuildFormat((XmlElement)node);
                    formats.Add(format);
                }
            }
        }

        /// <summary>
        /// Builds new format from the specified XML data.
        /// </summary>
        /// <param name="element">XML data.</param>
        /// <returns>HTMLFormat instance</returns>
        private HTMLFormat BuildFormat(XmlElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (!element.HasAttribute("name"))
                throw new ParseException("Invalid CSS document");

            string name = element.GetAttribute("name");
            FormatType type = GetFormatType(name);
            name = DeletePreffix(name);

            HTMLFormat format = new HTMLFormat(name, m_formatIndex);
            format.IsMerged = false;
            format.QuietMode = true;
            format.Storage = element;
            format.Type = type;

            m_formatIndex++;

            InFillFormatsByName(format);

            XmlNodeList nodeList = element.ChildNodes;
            XmlNode node = null;

            for (int i = 0, len = nodeList.Count; i < len; i++)
            {
                node = nodeList[i];
                if (node is XmlElement)
                {
                    SetProperty(format, (XmlElement)node);
                }
            }
            return format;
        }

        /// <summary>
        /// Sets the specified properties for the specified format.
        /// </summary>
        /// <param name="format">Format class for which properties must be set.</param>
        /// <param name="element">Element which contains format property value.</param>
        private void SetProperty(HTMLFormat format, XmlElement element)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            if (element == null)
                throw new ArgumentNullException("element");

            if (!element.HasAttribute("name")) return;

            string name = element.GetAttribute("name");

            string value = element.InnerText;
            if (value == null || value.Length == 0) return;

            // Check for all known attributes.
            switch (name.ToLower(CultureInfo.InvariantCulture))
            {
                /* colors */
                case "color": 
                    SetForeColor(format, value);
                    break;
                case "background-color": 
                    SetBackgroundColor(format, value);
                    break;
                /* text alignment */
                case "text-align": 
                    SetHorizontalAlign(format, value); 
                    break;
                case "vertical-align": 
                    SetVerticalAlign(format, value); 
                    break;
                /* cursor */
                case "cursor": 
                    SetCursor(format, value); 
                    break;
                /* padding */
                case "padding": 
                    SetPadding(format, value);
                    break;
                case "padding-left": 
                    SetPadding(format, value, 1); 
                    break;
                case "padding-top": 
                    SetPadding(format, value, 2);
                    break;
                case "padding-right": 
                    SetPadding(format, value, 3);
                    break;
                case "padding-bottom": 
                    SetPadding(format, value, 4); 
                    break;
                /* borders */
                case "border": 
                    SetBorder(format, value, "all");
                    break;
                case "border-left":
                    SetBorder(format, value, "left");
                    break;
                case "border-top": 
                    SetBorder(format, value, "top");
                    break;
                case "border-right": 
                    SetBorder(format, value, "right"); 
                    break;
                case "border-bottom":
                    SetBorder(format, value, "bottom");
                    break;
                /* borders styles */
                case "border-style":
                    SetBorderStyle(format, value, "all");
                    break;
                case "border-left-style":
                    SetBorderStyle(format, value, "left");
                    break;
                case "border-top-style":
                    SetBorderStyle(format, value, "top"); 
                    break;
                case "border-right-style":
                    SetBorderStyle(format, value, "right");
                    break;
                case "border-bottom-style": 
                    SetBorderStyle(format, value, "bottom"); 
                    break;
                /* borders width */
                case "border-width": 
                    SetBorderWidth(format, value, "all"); 
                    break;
                case "border-left-width": 
                    SetBorderWidth(format, value, "left");
                    break;
                case "border-top-width": 
                    SetBorderWidth(format, value, "top");
                    break;
                case "border-right-width":
                    SetBorderWidth(format, value, "right");
                    break;
                case "border-bottom-width":
                    SetBorderWidth(format, value, "bottom"); 
                    break;
                /* borders colors */
                case "border-color":
                    SetBorderColor(format, value, "all"); 
                    break;
                case "border-left-color": 
                    SetBorderColor(format, value, "left");
                    break;
                case "border-top-color": 
                    SetBorderColor(format, value, "top");
                    break;
                case "border-right-color": 
                    SetBorderColor(format, value, "right");
                    break;
                case "border-bottom-color": 
                    SetBorderColor(format, value, "bottom");
                    break;
                /* font settings */
                case "font-family": 
                    SetFontFamily(format, value); 
                    break;
                case "font-style":
                    SetFontStyle(format, value);
                    break;
                case "font-weight":
                    SetFontWeight(format, value);
                    break;
                case "font-size":
                    SetFontSize(format, value);
                    break;
                case "text-decoration":
                    SetTextDecoration(format, value); 
                    break;
                /* Size */
                case "width": 
                    SetWidth(format, value);
                    break;
                case "height": 
                    SetHeight(format, value); 
                    break;
                /*Background image*/
                case "background-image": 
                    SetBackgroundImage(format, value);
                    break;
                case "background-repeat": 
                    SetBackgroundRepeat(format, value); 
                    break;
                case "display": 
                    SetDisplay(format, value); 
                    break;
            }
        }

        /// <summary>
        /// Builds format from the specified string.
        /// </summary>
        /// <param name="css">CSS data in string.</param>
        /// <returns>Format if OK; otherwise NULL.</returns>
        private HTMLFormat FromString(string css)
        {
            if (css == null)
                throw new ArgumentNullException("css");

            if (css.Length == 0)
                throw new ArgumentException("String line is empty.");

            css = DEF_STYLE_ATTR_FORMAT + " { " + css + " }";

            HTMLFormat format = null;

            XmlDocument xmlDoc = HTMLUIControl.CSSParser.ParseString(css);
            XmlNode node = xmlDoc.DocumentElement.FirstChild;
            if (!(node is XmlElement)) return format;

            format = BuildFormat((XmlElement)node);

            return format;
        }

        /// <summary>
        /// Infills hash with an array of formats by their names (CSS names cought be complex value).
        /// </summary>
        /// <param name="format">Format object.</param>
        private void InFillFormatsByName(HTMLFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            string[] names = format.Name.Split(',');
            string formatName = string.Empty;
            string name = string.Empty;

            for (int i = 0, len = names.Length; i < len; i++)
            {
                name = names[i];
                formatName = name.Trim();

                if (!m_formatsByName.Contains(formatName))
                {
                    m_formatsByName[formatName] = new ArrayList();
                }

                (m_formatsByName[formatName] as ArrayList).Add(format);
            }
        }

        /// <summary>
        /// Adds formats to the specified array.
        /// </summary>
        /// <param name="array">Reference to the array to which the formats must be added.</param>
        /// <param name="name">Name by which formats will be found. Cannot be empty.</param>
        /// <param name="type">FormatType instance</param>
        private void AddFormatsByName(ArrayList array, string name, FormatType type)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            name = DeletePreffix(name);
            ArrayList arrayOfNames = (ArrayList)m_formatsByName[name];

            if (arrayOfNames != null)
            {
                HTMLFormat format = null;

                for (int i = 0, len = arrayOfNames.Count; i < len; i++)
                {
                    format = arrayOfNames[i] as HTMLFormat;

                    if (format.Type == type)
                    {
                        array.Add(format);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the point character if the name of the format begins with it.
        /// </summary>
        /// <param name="name">Name to correct. Name cannot be empty.</param>
        /// <returns>New corrected name.</returns>
        private string DeletePreffix(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            if (name[0] == '.' || name[0] == '#')
                return name.Substring(1);

            return name;
        }

        /// <summary>
        /// Returns the type of format by format name.
        /// </summary>
        /// <param name="name">Name used to detect the CSS format type.</param>
        /// <returns>FormatType instance</returns>
        private FormatType GetFormatType(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            char ch = name[0];

            if (ch == '.')     // format assigned to element by it "class" attribute
                return FormatType.Class;
            else if (ch == '#') // format assinged to element by it "id" attribute
                return FormatType.Id;
            else                        // all other CSS formats
                return FormatType.Name;
        }

        /// <summary>
        /// Adds body format if needed (if current element is BODY element).
        /// </summary>
        /// <param name="element">Reference to the element.</param>
        /// <param name="array">Array assigned to the element CSS formats.</param>
        private void AttachBodyFormat(BaseElement element, ArrayList array)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (array == null)
                throw new ArgumentNullException("array");

            if (!Utilities.StrEquals(element.Name, "body"))
            {
                AddFormatsByName(array, "body", FormatType.Name);
            }
        }

        /// <summary>
        /// Adds format by class name.
        /// </summary>
        /// <param name="tagElement">Reference to the element.</param>
        /// <param name="array">Array assigned to the element CSS formats.</param>
        /// <param name="sufix">Suffix for style's name.</param>
        private void AttachClassFormat(BaseElement tagElement, ArrayList array, string sufix)
        {
            if (tagElement == null)
                throw new ArgumentNullException("tagElement");

            if (array == null)
                throw new ArgumentNullException("array");

            if (sufix == null)
                throw new ArgumentNullException("sufix");

            if (tagElement.Attributes.Contains(AttributeName.Class))
            {
                IHTMLAttribute attribute = (HTMLAttributeImpl)tagElement.Attributes[AttributeName.Class];

                // Add formats by tag name.
                AddFormatsByName(array, "." + attribute.Value + sufix, FormatType.Class);

                // Add format by tag name and specified class.
                AddFormatsByName(array, tagElement.Name + "." + attribute.Value + sufix, FormatType.Class);
            }
        }

        /// <summary>
        /// Adds format by ID of the element (&lt;td id="someId" &gt; #someId { ... })
        /// </summary>
        /// <param name="element">Reference to the element.</param>
        /// <param name="array">Array assigned to the element CSS formats.</param>
        /// <param name="sufix">Suffix for the style's name.</param>
        private void AttachIDFormat(BaseElement element, ArrayList array, string sufix)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (array == null)
                throw new ArgumentNullException("array");

            if (sufix == null)
                throw new ArgumentNullException("sufix");

            if (element.Attributes.Contains("id"))
            {
                IHTMLAttribute attribute = (HTMLAttributeImpl)element.Attributes["id"];

                // Add format by tag "id" attribute value.
                AddFormatsByName(array, DEF_ID_PREFIX + attribute.Value + sufix, FormatType.Id);
            }
        }

        /// <summary>
        /// Adds format from style attribute if it exists.
        /// </summary>
        /// <param name="element">Reference to the element.</param>
        /// <param name="array">Array assigned to the element CSS formats.</param>
        private void AttachStyleFormat(BaseElement element, ArrayList array)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (array == null)
                throw new ArgumentNullException("array");

            // Create dynamic format from tag "style" attribute value.
            if (element.Attributes.Contains("style"))
            {
                IHTMLAttribute attribute = (HTMLAttributeImpl)element.Attributes["style"];
                HTMLFormat format = FromString(attribute.Value);
                if (format == null) return;

                format.Name = element.UniqueID + format.Name;

                array.Add(format);

                // NOTE: Remove format from the collection if such exists.
                if (m_formatsCollection.Contains(format.Name))
                {
                    m_formatsCollection.Remove(format.Name);
                }

                m_formatsCollection.Add(format);
            }
        }

        /// <summary>
        /// Searches for additional formats for link elements and stores it to the specified hash.
        /// </summary>
        /// <param name="hash">Hash for storing formats.</param>
        /// <param name="element">Element for formats calculation.</param>
        private void AttachLinkFormats(Hashtable hash, BaseElement element)
        {
            if (hash == null)
                throw new ArgumentNullException("hash");

            if (element == null)
                throw new ArgumentNullException("element");

            AElementImpl linkElm = element as AElementImpl;

            if (linkElm != null)
            {
                // add to general collection :link styles also.
                ArrayList arrFormats = hash[element.UniqueID] as ArrayList;

                if (arrFormats != null)
                {
                    AddToCollection(arrFormats, element, DEF_PSEUDO_LINK);
                }

                //// Create collection of :hover styles.
                ArrayList hoverList = new ArrayList();
                AddToCollection(hoverList, element, DEF_PSEUDO_HOVER);
                hash[element.UniqueID + DEF_PSEUDO_HOVER] = hoverList;

                //// Create collection of :visited styles.
                ArrayList visitedList = new ArrayList();
                AddToCollection(visitedList, element, DEF_PSEUDO_VISITED);
                hash[element.UniqueID + DEF_PSEUDO_VISITED] = visitedList;
            }
        }

        /// <summary>
        /// Searches different types of styles for element and adds it to the specified array.
        /// </summary>
        /// <param name="formatsList">Formats storage.</param>
        /// <param name="element">Parent format's element.</param>
        /// <param name="sufix">String suffix for the format names.</param>
        private void AddToCollection(ArrayList formatsList, BaseElement element, string sufix)
        {
            if (formatsList == null)
                throw new ArgumentNullException("formatsList");

            if (element == null)
                throw new ArgumentNullException("element");

            if (sufix == null)
                throw new ArgumentNullException("sufix");

            // Add format by tag name.
            AddFormatsByName(formatsList, element.Name + sufix, FormatType.Name);

            // Add formats by tag "class" attribute value.
            AttachClassFormat(element, formatsList, sufix);

            // Add format by tag "id" attribute value.
            AttachIDFormat(element, formatsList, sufix);
        }
        #endregion

        #region Property Set methods
        /// <summary>
        /// Sets the forecolor for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of the format.</param>
        internal void SetForeColor(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);

            if (type != AttributeToken.Color_Hex &&
                type != AttributeToken.Color_rgb &&
                type != AttributeToken.Color_Word
              ) return;

            format.ForeColor = AttributeParser.GetColor(value);
            format.Merge |= MergeMask.ForeColor;
        }

        /// <summary>
        /// Sets the background color for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of the color.</param>
        internal void SetBackgroundColor(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);

            if (type != AttributeToken.Color_Hex &&
                type != AttributeToken.Color_rgb &&
                type != AttributeToken.Color_Word
              ) return;

            format.BackgroundColor = AttributeParser.GetColor(value);
            format.Merge |= MergeMask.BgColor;
        }

        /// <summary>
        /// Sets the HorizontalAlign for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of the align.</param>
        internal void SetHorizontalAlign(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.String) return;

            switch (value.ToLower())
            {
                case "left":
                case "justify": goto default;
                case "middle":
                case "center": 
                format.HorizontalAlign = StringAlignment.Center; 
                break;
                case "right": 
                    format.HorizontalAlign = StringAlignment.Far;
                    break;
                default: 
                    format.HorizontalAlign = StringAlignment.Near; 
                    break;
            }

            format.Merge |= MergeMask.HAlignment;
        }

        /// <summary>
        /// Sets the VerticalAlign for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of the align.</param>
        internal void SetVerticalAlign(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.String) return;

            switch (value.ToLower())
            {
                case "baseline":
                case "middle": goto default;
                case "super":
                case "top":
                case "text-top":
                format.VerticalAlign = StringAlignment.Near;
                break;
                case "bottom":
                case "text-bottom":
                format.VerticalAlign = StringAlignment.Far;
                break;
                default: 
                    format.VerticalAlign = StringAlignment.Center; 
                    break;
            }

            format.Merge |= MergeMask.VAlignmnet;
        }

        /// <summary>
        /// Sets the cursor for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of the cursor.</param>
        internal void SetCursor(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.String) return;

            switch (value.ToLower())
            {
                case "auto":
                    format.Cursor = Cursor.Current;
                    break;
                case "crosshair": 
                    format.Cursor = Cursors.Cross;
                    break;
                case "default":
                    format.Cursor = Cursors.Default;
                    break;
                case "pointer": 
                    format.Cursor = Cursors.Hand; 
                    break;
                case "move": 
                    format.Cursor = Cursors.SizeAll; 
                    break;
                case "e-resize":
                case "w-resize":
                format.Cursor = Cursors.SizeWE;
                break;
                case "ne-resize":
                case "sw-resize": 
                format.Cursor = Cursors.SizeNESW;
                break;
                case "nw-resize ":
                case "se-resize": 
                format.Cursor = Cursors.SizeNWSE; 
                break;
                case "n-resize ":
                case "s-resize ": 
                format.Cursor = Cursors.SizeNS; 
                break;
                case "text": 
                    format.Cursor = Cursors.IBeam; 
                    break;
                case "wait":
                    format.Cursor = Cursors.WaitCursor; 
                    break;
                case "help": 
                    format.Cursor = Cursors.Help;
                    break;
                default:
                    format.Cursor = Cursors.Default; 
                    break;
            }

            format.Merge |= MergeMask.Cursor;
        }

        /// <summary>
        /// Overloaded. Sets the padding for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of the padding.</param>
        internal void SetPadding(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.Rectangle &&
                type != AttributeToken.Triangle &&
                type != AttributeToken.Point &&
                type != AttributeToken.Integer
              ) return;

            Rectangle padding;
            switch (type)
            {
                case AttributeToken.Rectangle:
                    padding = AttributeParser.GetRectangle(value);
                    format.Merge |= MergeMask.PaddingAll;
                    break;

                case AttributeToken.Triangle:
                    Rectangle rect = AttributeParser.GetTriangle(value);
                    padding = new Rectangle(rect.Top, rect.Left, rect.Top, rect.Width);
                    format.Merge |= MergeMask.PaddingAll;
                    break;

                case AttributeToken.Point:
                    Point p = AttributeParser.GetPoint(value);
                    padding = new Rectangle(p.Y, p.X, p.Y, p.X);
                    format.Merge |= MergeMask.PaddingAll;
                    break;

                case AttributeToken.Integer:
                    int padValue = AttributeParser.GetInteger(value);
                    padding = new Rectangle(padValue, padValue, padValue, padValue);
                    format.Merge |= MergeMask.PaddingAll;
                    break;

                default:
                    padding = Rectangle.Empty;
                    break;
            }

            format.Padding = padding;
        }

        /// <summary>
        /// Sets the padding for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of padding.</param>
        /// <param name="order">Side of padding.</param>
        internal void SetPadding(HTMLFormat format, string value, int order)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            if (order < 0 || order > 4)
                throw new ArgumentOutOfRangeException("order", "Value can not be less than 0 and greater than 4.");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.Integer) return;

            Rectangle padding;
            Rectangle frmPad = format.Padding;
            int val = AttributeParser.GetInteger(value);

            switch (order)
            {
                case 1:
                    padding = new Rectangle(val, frmPad.Top, frmPad.Width, frmPad.Height);
                    format.Merge |= MergeMask.PaddingLeft;
                    break;

                case 2:
                    padding = new Rectangle(frmPad.Left, val, frmPad.Width, frmPad.Height);
                    format.Merge |= MergeMask.PaddingTop;
                    break;

                case 3:
                    padding = new Rectangle(frmPad.Left, frmPad.Top, val, frmPad.Height);
                    format.Merge |= MergeMask.PaddingRight;
                    break;

                case 4:
                    padding = new Rectangle(frmPad.Left, frmPad.Top, frmPad.Width, val);
                    format.Merge |= MergeMask.PaddingBottom;
                    break;

                default:
                    padding = Rectangle.Empty;
                    break;
            }

            format.Padding = padding;
        }

        /// <summary>
        /// Sets the border for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of border.</param>
        /// <param name="toWhat">Target side of border.</param>
        internal void SetBorder(HTMLFormat format, string value, string toWhat)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.Complex) return;

            string[] values = AttributeParser.GetTokensValues(value, ' ');

            if (values.Length != 3)
                return;

            Border border = new Border();
            int[] was_ident = { 1, 1, 1 };
            for (int i = 0; i < 3; i++)
            {
                AttributeToken tokenType = AttributeParser.DetectType(values[i]);
                switch (tokenType)
                {
                    case AttributeToken.BorderStyle:
                        if (was_ident[0] != 0)
                        {
                            border.Style = AttributeParser.GetBorderStyle(values[i]);
                            was_ident[0] = 0;
                            format.Merge |= MergeMask.BorderStyle;
                            break;
                        }
                        else return;

                    case AttributeToken.Integer:
                        if (was_ident[1] != 0)
                        {
                            border.Width = AttributeParser.GetInteger(values[i]);
                            was_ident[1] = 0;
                            format.Merge |= MergeMask.BorderWidth;
                            break;
                        }
                        else return;

                    case AttributeToken.Color_rgb:
                    case AttributeToken.Color_Hex:
                    case AttributeToken.Color_Word:

                        if (was_ident[2] != 0)
                        {
                            border.Color = AttributeParser.GetColor(values[i]);
                            was_ident[2] = 0;
                            format.Merge |= MergeMask.BorderColor;
                            break;
                        }
                        else return;
                }
            }

            switch (toWhat)
            {
                case "all": border.CopyTo(format.Left);
                    border.CopyTo(format.Top);
                    border.CopyTo(format.Right);
                    border.CopyTo(format.Bottom);
                    format.Merge |= MergeMask.BorderAll;
                    break;
                case "left": border.CopyTo(format.Left);
                    format.Merge |= MergeMask.BorderLeft;
                    break;
                case "top": border.CopyTo(format.Top);
                    format.Merge |= MergeMask.BorderTop;
                    break;
                case "right": border.CopyTo(format.Right);
                    format.Merge |= MergeMask.BorderRight;
                    break;
                case "bottom": border.CopyTo(format.Bottom);
                    format.Merge |= MergeMask.BorderBottom;
                    break;
            }
        }

        /// <summary>
        /// Sets the border style for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of the Border style.</param>
        /// <param name="toWhat">Target side of border style.</param>
        internal void SetBorderStyle(HTMLFormat format, string value, string toWhat)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.BorderStyle) return;

            BordersStyle style = AttributeParser.GetBorderStyle(value);
            format.Merge |= MergeMask.BorderStyle;

            switch (toWhat)
            {
                case "all": format.Left.Style = style;
                    format.Top.Style = style;
                    format.Right.Style = style;
                    format.Bottom.Style = style;
                    format.Merge |= MergeMask.BorderAll;
                    break;
                case "left": format.Left.Style = style;
                    format.Merge |= MergeMask.BorderLeft;
                    break;
                case "top": format.Top.Style = style;
                    format.Merge |= MergeMask.BorderTop;
                    break;
                case "right": format.Right.Style = style;
                    format.Merge |= MergeMask.BorderRight;
                    break;
                case "bottom": format.Bottom.Style = style;
                    format.Merge |= MergeMask.BorderBottom;
                    break;
            }
        }

        /// <summary>
        /// Sets the border width for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of the border width.</param>
        /// <param name="toWhat">Target side of border width.</param>
        internal void SetBorderWidth(HTMLFormat format, string value, string toWhat)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.Integer) return;

            int width = AttributeParser.GetInteger(value);
            format.Merge |= MergeMask.BorderWidth;

            switch (toWhat)
            {
                case "all": format.Left.Width = width;
                    format.Top.Width = width;
                    format.Right.Width = width;
                    format.Bottom.Width = width;
                    format.Merge |= MergeMask.BorderAll;
                    break;
                case "left": format.Left.Width = width;
                    format.Merge |= MergeMask.BorderLeft;
                    break;
                case "top": format.Top.Width = width;
                    format.Merge |= MergeMask.BorderTop;
                    break;
                case "right": format.Right.Width = width;
                    format.Merge |= MergeMask.BorderRight;
                    break;
                case "bottom": format.Bottom.Width = width;
                    format.Merge |= MergeMask.BorderBottom;
                    break;
            }
        }

        /// <summary>
        /// Sets the border color for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of border color.</param>
        /// <param name="toWhat">Target side.</param>
        internal void SetBorderColor(HTMLFormat format, string value, string toWhat)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.Color_Hex &&
              type != AttributeToken.Color_rgb &&
              type != AttributeToken.Color_Word
              ) return;

            Color color = AttributeParser.GetColor(value);
            format.Merge |= MergeMask.BorderColor;

            switch (toWhat)
            {
                case "all": format.Left.Color = color;
                    format.Top.Color = color;
                    format.Right.Color = color;
                    format.Bottom.Color = color;
                    format.Merge |= MergeMask.BorderAll;
                    break;
                case "left": format.Left.Color = color;
                    format.Merge |= MergeMask.BorderLeft;
                    break;
                case "top": format.Top.Color = color;
                    format.Merge |= MergeMask.BorderTop;
                    break;
                case "right": format.Right.Color = color;
                    format.Merge |= MergeMask.BorderRight;
                    break;
                case "bottom": format.Bottom.Color = color;
                    format.Merge |= MergeMask.BorderBottom;
                    break;
            }
        }

        /// <summary>
        /// Sets the font family for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String value of font family.</param>
        internal void SetFontFamily(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.FontFamily) return;

            Font font = format.Font;

            format.FontFamilyName = AttributeParser.GetFontFamily(value);
            format.Merge |= MergeMask.FontFamily;
        }

        /// <summary>
        /// Sets the font style for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of font style.</param>
        internal void SetFontStyle(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.FontStyle) return;

            format.FontStyle = AttributeParser.GetFontStyle(value);
            format.Merge |= MergeMask.FontStyle;
        }

        /// <summary>
        /// Sets the font weight for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of font weight.</param>
        internal void SetFontWeight(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.FontWeight && type != AttributeToken.FontStyle) return;

            format.FontWeight = AttributeParser.GetFontWeight(value);
            format.Merge |= MergeMask.FontWeight;
        }

        /// <summary>
        /// Adds font size to format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of font size.</param>
        internal void SetFontSize(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (
              type != AttributeToken.FontSize &&
              type != AttributeToken.Integer &&
              type != AttributeToken.Float &&
              type != AttributeToken.Percent
              ) return;

            GraphicsUnit unit;
            format.FontSize = AttributeParser.GetFontFromSize(value, format.Font, out unit);

            format.Unit = unit;
            format.Merge |= MergeMask.FontSize;
        }

        /// <summary>
        /// Sets the text decoration for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of text decoration.</param>
        internal void SetTextDecoration(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.TextDecoration && !Utilities.StrEquals(value, "none")) return;

            format.TextDecoration = AttributeParser.GetTextDecoration(value);
            format.Merge |= MergeMask.TextDecoration;
        }

        /// <summary>
        /// Sets the width for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of width.</param>
        internal void SetWidth(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.Percent &&
                type != AttributeToken.Integer
              ) return;

            string suffix;
            int width = 0;
            if (type == AttributeToken.Percent)
            {
                format.Width = AttributeParser.GetInteger(value);
                format.WidthType = SizeTypeEx.Percent;
            }
            else if (type == AttributeToken.Integer)
            {
                width = AttributeParser.GetInteger(value, out suffix);
                if (Utilities.StrEquals(suffix, "px") ||
                    suffix.Length == 0)
                {
                    format.Width = width;
                    format.WidthType = SizeTypeEx.Number;
                }
            }
            format.Merge |= MergeMask.Width;
        }

        /// <summary>
        /// Sets the height for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of height.</param>
        internal void SetHeight(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);
            if (type != AttributeToken.Percent &&
                type != AttributeToken.Integer
              ) return;

            string suffix;
            int height = 0;
            if (type == AttributeToken.Percent)
            {
                format.Height = AttributeParser.GetInteger(value, out suffix);
                format.HeightType = SizeTypeEx.Percent;
            }
            else if (type == AttributeToken.Integer)
            {
                height = AttributeParser.GetInteger(value, out suffix);
                if (Utilities.StrEquals(suffix, "px") || suffix.Length == 0)
                {
                    format.Height = height;
                    format.HeightType = SizeTypeEx.Number;
                }
            }
            format.Merge |= MergeMask.Height;
        }

        /// <summary>
        /// Sets the background image for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of background image.</param>
        internal void SetBackgroundImage(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);

            if (type != AttributeToken.Uri &&
                type != AttributeToken.String
              ) return;

            string path = AttributeParser.GetUri(value);
            string fullPath;
            if (!this.Control.ThreadDocument.ImageCache.Insert(path, out fullPath)) return;

            format.BackgroundImage = this.Control.ThreadDocument.ImageCache[fullPath];
            format.Merge |= MergeMask.BgImage;
        }

        /// <summary>
        /// Sets the background repeat property for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">String representation of background repeat.</param>
        internal void SetBackgroundRepeat(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            AttributeToken type = AttributeParser.DetectType(value);

            if (type != AttributeToken.String) return;

            switch (value)
            {
                case "repeat":
                    format.BackgroundImageRepeat = RepeatStyle.Repeat;
                    break;
                case "repeat-x":
                    format.BackgroundImageRepeat = RepeatStyle.RepeatX;
                    break;
                case "repeat-y":
                    format.BackgroundImageRepeat = RepeatStyle.RepeatY;
                    break;
                case "no-repeat":
                    format.BackgroundImageRepeat = RepeatStyle.NoRepeat;
                    break;
            }
            format.Merge |= MergeMask.BgRepeat;
        }

        /// <summary>
        /// Sets the value of 'display' CSS attribute for the format.
        /// </summary>
        /// <param name="format">Format object.</param>
        /// <param name="value">Value of 'display' attribute.</param>
        internal void SetDisplay(HTMLFormat format, string value)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            if (value == null)
                throw new ArgumentNullException("value");

            value = value.Trim();

            if (Utilities.StrEquals(value, "none"))
            {
                format.DisplayNone = true;
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Clears all resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_bDisposed)
            {
                if (m_document != null)
                {
                    m_document.RemoveAll();
                    m_document = null;
                }
                if (m_formatsCollection != null)
                {
                    m_formatsCollection.Dispose();
                    m_formatsCollection = null;
                }
                if (m_formatsByName != null)
                {
                    m_formatsByName.Clear();
                    m_formatsByName = null;
                }
                if (m_defaultFormat != null)
                {
                    m_defaultFormat.CursorChanged -= new ValueChangedEventHandler(CursorChanged);
                }

                m_formatIndex = 0;

                // Set flag that was disposed.
                m_bDisposed = true;
            }
        }

        #endregion

        #region Class event handlers
        /// <summary>
        /// Handles cursor changinf of default format.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void CursorChanged(object sender, ValueChangedEventArgs e)
        {
            Cursor cursor = e.NewValue as Cursor;

            if (cursor != null)
            {
                m_control.Cursor = cursor;
            }
        }
        #endregion
    }
}