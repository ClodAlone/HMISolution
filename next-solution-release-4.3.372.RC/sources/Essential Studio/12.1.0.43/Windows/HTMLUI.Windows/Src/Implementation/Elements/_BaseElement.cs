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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Syncfusion.Documentation;
using Syncfusion.HTMLUI.Base;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Scripting;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility.Selection;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Base class for all HTML elements. All HTML tag elements must inherit
    /// this class.
    /// </summary>
    [AttributeHolder(typeof(HTMLAttributesCollection))]
    public abstract class BaseElement
      : IHTMLElement, ICloneable, IDisposable
    {
        #region Class constants

        /// <summary>
        /// Default case insensitive comparer for internal use.
        /// </summary>
        private static readonly IComparer DEF_COMPARER = new CaseInsensitiveComparer();

        /// <summary>
        /// Pattern for dividing string by words with regular expressions.
        /// </summary>
        private const string DEF_REG_WORDS = @"^[ ]{1}|[\S]+[\s]*"; ////@"[\S]+[\s]*";

        /// <summary>
        /// Pattern for numbers.
        /// </summary>
        private const string DEF_REG_NUMBERS = @"[0-9]+";

        /// <summary>
        /// Options for regular expressions.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected const RegexOptions DEF_REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        /// <summary>
        /// Attribute name for location run-time property.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal const string DEF_RUNTIME_LOCATION = "xLocation";

        /// <summary>
        /// Attribute name for size run-time property.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal const string DEF_RUNTIME_SIZE = "xSize";

        /// <summary>
        /// Attribute name for visible run-time property.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal const string DEF_RUNTIME_VISIBLE = "xVisible";

        /// <summary>
        /// List of default run-time attributes similar to all HTML elements.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal readonly string[] DEF_RUNTIME = new string[]
  {
    DEF_RUNTIME_LOCATION,
    DEF_RUNTIME_SIZE,
    DEF_RUNTIME_VISIBLE
  };

        /// <summary>
        /// Index of default selected symbol in the element.
        /// </summary>
        private const int DEF_SELECT_INDEX = -1;

        /// <summary>
        /// Value  of tab stop in the text.
        /// </summary>
        private const float DEF_TAB_STOP_VALUE = 64.0f;

        #endregion

        #region Class static members

        /// <summary>
        /// String format for text measuring and output.
        /// </summary>
        internal static readonly StringFormat _stringFormat;

        /// <summary>
        /// RegEx object for dividing string by words.
        /// </summary>
        private static Regex _regWords = new Regex(DEF_REG_WORDS, DEF_REGEX_OPTIONS);

        /// <summary>
        /// RegEx object for retrieving numbers from string.
        /// </summary>
        private static Regex _regNumbers = new Regex(DEF_REG_NUMBERS, DEF_REGEX_OPTIONS);

        /// <summary>
        /// Graphics object.
        /// </summary>
        private static Graphics _graphics = Graphics.FromImage(new Bitmap(1, 1));

        /// <summary>
        /// Gets an object used for measurement of text elements.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static Graphics Graphics
        {
            get
            {
                return _graphics;
            }
        }

        /// <summary>
        /// Holds reaction on attributes changing.
        /// </summary>
        internal static ReactionCollection m_reaction;
        #endregion

        #region Class members

        /// <summary>
        /// Utility array which holds sorted list of supported events.
        /// </summary>
        private ArrayList m_arrSupEvents;

        /// <summary>
        /// Sorted list of run-time attributes supported by element.
        /// </summary>       
        private ArrayList m_arrRuntimeAttr;

        /// <summary>
        /// Parent of the current element.
        /// </summary>       
        private IHTMLElement m_parent;

        /// <summary>
        /// Format of the element.
        /// </summary>
        private HTMLFormat m_format;

        /// <summary>
        /// AccessKey storage.
        /// </summary>
        private Keys m_key;

        /// <summary>
        /// Collection of children. Collection created on demand.
        /// </summary>
        private HTMLElementsCollection m_children;

        /// <summary>
        /// Collection of events. Collection created on demand.
        /// </summary>
        private HTMLEventsCollection m_events;

        /// <summary>
        /// Collection of attributes.
        /// </summary>
        private HTMLAttributesCollection m_attributes;

        /// <summary>
        /// Unique identifier of the element in the document.
        /// </summary>
        private string m_unique;

        /// <summary>
        /// Name of the element.
        /// </summary>
        private string m_strName;

        /// <summary>
        /// Member holds all information about current element in XML format
        /// compatible with XHTML.
        /// </summary>
        internal XmlElement m_storage;

        /// <summary>
        /// Indicates whether element resizable.
        /// </summary>
        private bool m_isResizable;

        /// <summary>
        /// Format for special visibility of the element by default.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected HTMLFormat m_ownFormat;

        /// <summary>
        /// Holds max size of the element.
        /// </summary>
        private Size m_size;

        /// <summary>
        /// Hash which contains size of each XML text in the element.
        /// </summary>
        private Hashtable m_textSizeHash;

        /// <summary>
        /// Array of the blocks which represent a single line.
        /// </summary>
        protected BlocksCollection m_blocks;

        /// <summary>
        /// Instance of the control.
        /// </summary>
        private HTMLUIControl m_control;

        /// <summary>
        /// Represents location of the element.
        /// </summary>
        private Point m_location;

        /// <summary>
        /// Represents line in the element. Needed for inserting children.
        /// </summary>
        internal Block m_curBlock;

        /// <summary>
        /// Indicates where current position is.
        /// </summary>
        protected Point m_curPos;

        /// <summary>
        /// Bounds for this element.
        /// </summary>
        protected Rectangle m_bounds;

        /// <summary>
        /// Indicates whether element has rectangle structure or not.
        /// </summary>
        protected bool m_isBlockStructure;

        /// <summary>
        /// Indicates whether element must be postponed in new line (div, ...).
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool m_isPostponed;

        /// <summary>
        /// Cursor before mouse enters on element.
        /// </summary>
        protected Cursor m_oldCursor;

        /// <summary>
        /// Type of the element (block, inline, etc).
        /// </summary>
        private ElementType m_elementType;

        /// <summary>
        /// Indicates whether we must skip whitespaces in the element.
        /// </summary>
        private bool m_skipWhitespaces;

        /// <summary>
        /// Instance of the main block of the element if it has block structure.
        /// </summary>
        private Block m_mainBlock;

        /// <summary>
        /// Minimum width of the element.
        /// </summary>
        private int m_minWidth;

        /// <summary>
        /// Minimum height of the element.
        /// </summary>
        private int m_minHeight;

        /// <summary>
        /// Indicates whether element is inside table.
        /// </summary>
        private bool m_bInTable;

        /// <summary>
        /// Indicates whether we invoked the Dispose method.
        /// </summary>
        private bool m_bDisposed;

        /// <summary>
        /// Indicates whether the element is visible.
        /// </summary>
        private bool m_bIsVisible;

        /// <summary>
        /// Indents space around the element.
        /// </summary>
        private Space m_indentSpace;

        /// <summary>
        /// Document holder instance.
        /// </summary>
        private InputHTML m_document;

        /// <summary>
        /// Width of the longest word in the element.
        /// </summary>
        private int m_textMinWidth;

        /// <summary>
        /// Indicates whether to skip raised event.
        /// </summary>
        private bool m_bSkipEvents;

        /// <summary>
        /// Indicates whether to skip the width of this element when getting width from parent.
        /// </summary>
        private bool m_bSkipWidth;

        /// <summary>
        /// Indicates whether element processes default focusing algorithm.
        /// </summary>
        private bool m_bDefaultFocusing = true;

        /// <summary>
        /// Indicating whether whitespace after element is prohibited.
        /// </summary>
        private bool m_bSpaceProhibited = false;

        #endregion

        #region Class properties

        /// <summary>
        /// Gets or sets the parent of the current HTML element.
        /// </summary>
        [Browsable(false)]
        public IHTMLElement Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                if (value != m_parent)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_parent, value);
                    m_parent = value;
                    OnParentChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Fast access key to the element.
        /// </summary>
        [Browsable(false)]
        public Keys AccessKey
        {
            get
            {
                return m_key;
            }
            set
            {
                if (value != m_key)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_key, value);
                    m_key = value;
                    OnAccessKeyChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets the collection of attributes in the current element.
        /// </summary>
        [ReadOnly(true), Browsable(true), Description("Gets collection of attributes in current element."), Editor(typeof(AttributesCollectionEditor), typeof(UITypeEditor))]
        public IHTMLAttributesCollection Attributes
        {
            get
            {
                return m_attributes;
            }
        }

        /// <summary>
        /// Gets the collection of the element's children.
        /// </summary>
        //// [ ScriptBrowsable( PropertyType.Collection, typeof( IHTMLElement ) ) ]
        [Browsable(true), Description("Gets collection of element children."), Editor(typeof(ElementsCollectionEditor), typeof(UITypeEditor))]
        public IHTMLElementsCollection Children
        {
            get
            {
                if (m_children == null)
                    m_children = new HTMLElementsCollection(this);

                return m_children;
            }
        }

        /// <summary>
        /// Gets the collection of events supported by the element.
        /// </summary>
        [Browsable(false)]
        public IHTMLEventsCollection Events
        {
            get
            {
                if (m_events == null)
                    m_events = new HTMLEventsCollection(this);

                return m_events;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the HTML element has children.
        /// </summary>
        /// <remarks>
        /// The check does not create internal children collection thus optimizing memory
        /// usage.
        /// </remarks>
        [Browsable(false)]
        public bool HasChildren
        {
            get
            {
                return m_children != null && m_children.Count > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the HTML element has events.
        /// </summary>
        [Browsable(false)]
        public bool HasEvents
        {
            get
            {
                return m_events != null && m_events.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets the format of the current element needed for rendering.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true), Description("Get format of current element needed for rendering.")]
        public IHTMLFormat Format
        {
            get
            {
                if (m_format == null)
                    return m_control.DefaultFormat;

                return m_format;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Format");

                if (m_format != value)
                {
                    if (m_format != null)
                    {
                        m_format.OnChanged -= new BeforeValueChangeEventHandler(Attributes_Changed);
                    }

                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_format, value);
                    m_format = (HTMLFormat)value;
                    m_format.OnChanged += new BeforeValueChangeEventHandler(Attributes_Changed);

                    if (!this.QuietMode)
                    {
                        OnFormatChanged(args);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for the HTML element in the HTML elements objects tree. This
        /// id is specified by the user.
        /// </summary>
        [Browsable(true), Description("Unique identifier of HTML element in HTML elements objects tree." +
           "This element is assigned from id attribute of a tag.")]
        public string ID
        {
            get
            {
                if (!m_attributes.Contains("id"))
                {
                    return string.Empty;
                }

                return m_attributes["id"].Value;
            }
            set
            {
                if (!m_attributes.Contains("id"))
                {
                    m_attributes.Add("id", string.Empty);
                }

                if (m_attributes["id"].Value != value)
                {
                    ((HTMLAttributeImpl)m_attributes["id"]).Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique id of the element. Control guarantees that this id is
        /// always unique for the document.
        /// </summary>
        [ReadOnly(true), Description("This is unique id of element. Control guarantees that this id" + "is always unique for document.")]
        public string UniqueID
        {
            get
            {
                return m_unique;
            }
            set
            {
                if (value != m_unique)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_unique, value);
                    m_unique = value;
                    OnUniqueIDChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets or sets in text, the inner part of the element. Inner part includes the children of
        /// the current element. On change of inner part, HTML parser must change if there is need for
        /// tree of children. On inner HTML property change, it will lose all attached
        /// events. After change, you must attach events again.
        /// </summary>
        [Browsable(true), Description("Gets or sets in text inner part of element.")]
        public string InnerHTML
        {
            get
            {
                return CalculateInnerHTML();
            }
            set
            {
                ValueChangedEventArgs args = new ValueChangedEventArgs(this.InnerHTML, value);
                OnInnerHTMLChanged(args);
            }
        }

        /// <summary>
        /// Gets the HTML text including inner and current element text; also the current element is
        /// added into the output.
        /// </summary>
        [ReadOnly(true), Description("HTML text including inner and current element text, also" + " current element is added into output.")]
        public string OuterHTML
        {
            get
            {
                return ElementOpenPart + InnerHTML + ElementClosePart;
            }
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        [Browsable(true), Description("Gets name of the tag.")]
        public string Name
        {
            get
            {
                return m_strName;
            }
        }

        /// <summary>
        /// Gets or sets the value of the element with the specified attribute name.
        /// </summary>
        /// <param name="attributeName">String Attribute</param>
        [Browsable(false)]
        public string this[string attributeName]
        {
            get
            {
                return m_attributes[attributeName].Value;
            }
            set
            {
                ((HTMLAttributeImpl)m_attributes[attributeName]).Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the tab index for the HTML element.
        /// </summary>
        /// <remarks>
        /// Objects with a positive TabIndex are selected in increasing order and in source order to resolve duplicates.
        /// Objects with an TabIndex of zero are selected in source order. 
        /// Objects with a negative TabIndex are omitted from the tabbing order.
        /// </remarks>
        [Browsable(true), Description("Gets or sets tab index for HTML element.")]
        public int TabIndex
        {
            get
            {
                if (m_attributes.Contains(AttributeName.TabIndex))
                {
                    int value = Utilities.ConvertToInteger(m_attributes[AttributeName.TabIndex].Value);
                    return (value == int.MinValue) ? -1 : value;
                }

                return -1;
            }
            set
            {
                if (!m_attributes.Contains(AttributeName.TabIndex))
                {
                    m_attributes.Add(AttributeName.TabIndex);
                }

                ((HTMLAttributeImpl)m_attributes[AttributeName.TabIndex]).Value =
                  value.ToString(CultureInfo.InvariantCulture);

                RaiseTabIndexChangedEvent(EventArgs.Empty);
                SyncTabStopCollection(value);
            }
        }

        /// <summary>
        /// Gets or sets the real location of elements after rendering in control client coordinates.
        /// </summary>
        [ReadOnly(true), Description("Real location of element after rendering in control client coordinates.")]
        public Point Location
        {
            get
            {
                if (!m_attributes.Contains(DEF_RUNTIME_LOCATION))
                {
                    m_attributes.Add(DEF_RUNTIME_LOCATION);
                    ((HTMLAttributeImpl)m_attributes[DEF_RUNTIME_LOCATION]).Value = m_location.ToString();
                }
                return m_location;
            }
            set
            {
                if (!m_attributes.Contains(DEF_RUNTIME_LOCATION))
                {
                    m_attributes.Add(DEF_RUNTIME_LOCATION);
                }

                if (m_location != value)
                {
                    m_location = value;
                    ((HTMLAttributeImpl)m_attributes[DEF_RUNTIME_LOCATION]).Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the element location.
        /// </summary>
        [ReadOnly(true), Description("Gets or sets X coordinate of element location.")]
        public int X
        {
            get
            {
                return this.Location.X;
            }
            set
            {
                m_location = this.Location;
                m_location.X = value;
                this.Location = m_location;
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the element location.
        /// </summary>
        [ReadOnly(true), Description("Gets or sets Y coordinate of element location.")]
        public int Y
        {
            get
            {
                return this.Location.Y;
            }
            set
            {
                m_location = this.Location;
                m_location.Y = value;
                this.Location = m_location;
            }
        }

        /// <summary>
        /// Gets or sets the real size of the element after rendering.
        /// </summary>
        [ReadOnly(true), Description("Gets or sets real size of element after rendering.")]
        public Size Size
        {
            get
            {
                if (!m_attributes.Contains(DEF_RUNTIME_SIZE))
                {
                    m_attributes.Add(DEF_RUNTIME_SIZE);
                    ((HTMLAttributeImpl)m_attributes[DEF_RUNTIME_SIZE]).Value = m_size.ToString();
                }

                return m_size;
            }
            set
            {
                if (!m_attributes.Contains(DEF_RUNTIME_SIZE))
                {
                    m_attributes.Add(DEF_RUNTIME_SIZE);
                }

                if (m_size != value)
                {
                    m_size = value;
                    ((HTMLAttributeImpl)m_attributes[DEF_RUNTIME_SIZE]).Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the element.
        /// </summary>
        [ReadOnly(true), Description("Gets or sets width of the element.")]
        public int Width
        {
            get
            {
                return this.Size.Width;
            }
            set
            {
                if (m_size.Width != value)
                {
                    m_size = this.Size;
                    m_size.Width = value;
                    this.Size = m_size;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the element.
        /// </summary>
        [ReadOnly(true), Description("Gets or sets height of the element.")]
        public int Height
        {
            get
            {
                return this.Size.Height;
            }
            set
            {
                if (m_size.Height != value)
                {
                    m_size = this.Size;
                    m_size.Height = value;
                    this.Size = m_size;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current element is visible to the user.
        /// </summary>
        [ReactionType(DEF_RUNTIME_VISIBLE, ReactType.ReCalculatingDocument), Browsable(true), Description("Indicate if current element is visible to user or not.")]
        public virtual bool IsVisible
        {
            get
            {
                if (!m_attributes.Contains(DEF_RUNTIME_VISIBLE))
                {
                    m_attributes.Add(DEF_RUNTIME_VISIBLE);
                    ((HTMLAttributeImpl)m_attributes[DEF_RUNTIME_VISIBLE]).Value = m_bIsVisible.ToString();
                }

                return m_bIsVisible;
            }
            set
            {
                if (!m_attributes.Contains(DEF_RUNTIME_VISIBLE))
                {
                    m_attributes.Add(DEF_RUNTIME_VISIBLE);
                }
                if (m_bIsVisible != value)
                {
                    m_bIsVisible = value;
                    ((HTMLAttributeImpl)m_attributes[DEF_RUNTIME_VISIBLE]).Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element is resizable.
        /// </summary>
        [Browsable(false)]
        public bool IsResizable
        {
            get
            {
                return ((HTMLFormat)this.Format).IsWidthDefault & m_isResizable;
            }
            set
            {
                if (m_isResizable != value)
                {
                    m_isResizable = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether format is in quiet mode, skip event.
        /// </summary>
        [Browsable(false)]
        public bool QuietMode
        {
            get
            {
                return m_bSkipEvents;
            }
            set
            {
                if (value != m_bSkipEvents)
                {
                    m_bSkipEvents = value;
                    OnQuietModeChanged();
                }
            }
        }

        /// <summary>
        /// Gets an instance of the control.
        /// </summary>
        [Browsable(false)]
        public HTMLUIControl Control
        {
            get
            {
                return m_control;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element has input focus.
        /// </summary>
        [Browsable(false)]
        public bool Focused
        {
            get
            {
                return this.Document.FocusManager.FocusedElement == this;
            }
        }

        /// <summary>
        /// Gets the text inside the element.
        /// </summary>
        [Browsable(false)]
        public string Text
        {
            get
            {
                return CalculateInnerText();
            }
        }

        /// <summary>
        /// Gets the selected text inside the element.
        /// </summary>
        [Browsable(false)]
        public string SelectedText
        {
            get
            {
                return CalculateSelectedText();
            }
        }

        /// <summary>
        /// Gets the XML element that represents the current element.
        /// </summary>
        protected internal XmlElement Storage
        {
            get
            {
                return m_storage;
            }
        }

        /// <summary>
        /// Gets the format for special visibility of the element by default.
        /// </summary>
        protected internal virtual HTMLFormat OwnFormat
        {
            get
            {
                if (m_ownFormat == null)
                {
                    m_ownFormat = ((HTMLFormat)this.Control.DefaultFormat).Clone();
                    m_ownFormat.IsMerged = true;
                }
                m_ownFormat.Merge = MergeMask.None;

                return m_ownFormat;
            }
        }

        /// <summary>
        /// Gets the storage of the block's current element.
        /// </summary>
        internal BlocksCollection Blocks
        {
            get
            {
                if (m_blocks == null)
                    m_blocks = new BlocksCollection();

                return m_blocks;
            }
        }

        /// <summary>
        /// Gets or sets the current position inside the element.
        /// </summary>
        internal Point CurrentPosition
        {
            get
            {
                return m_curPos;
            }
            set
            {
                if (!m_curPos.Equals(value))
                {
                    m_curPos = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the bounds for the element.
        /// </summary>
        internal Rectangle Bounds
        {
            get
            {
                return m_bounds;
            }
            set
            {
                if (m_bounds != value)
                {
                    m_bounds = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element has rectangular form (for instance: div) or fixed width element.
        /// </summary>
        internal bool IsBlock
        {
            get
            {
                return (this.Type & ElementType.Block) > 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element must be postponed to a new line.
        /// </summary>
        protected bool IsPostponed
        {
            get
            {
                return m_isPostponed;
            }
            set
            {
                if (m_isPostponed != value)
                {
                    m_isPostponed = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether whitespace after the element must be prohibited.
        /// </summary>
        protected internal bool SpaceProhibited
        {
            get
            {
                return m_bSpaceProhibited;
            }
            set
            {
                if (m_bSpaceProhibited != value)
                {
                    m_bSpaceProhibited = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the element structure (Inline, block, etc).
        /// </summary>
        internal ElementType Type
        {
            get
            {
                return m_elementType;
            }
            set
            {
                m_elementType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to skip all whitespaces in the element.
        /// </summary>
        protected bool SkipWhiteSpaces
        {
            get
            {
                return m_skipWhitespaces;
            }
            set
            {
                if (m_skipWhitespaces != value)
                {
                    m_skipWhitespaces = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the main block for the element if it has block type.
        /// </summary>
        internal Block MainBlock
        {
            get
            {
                if (!this.IsBlock) return null;

                return m_mainBlock;
            }
            set
            {
                if (m_mainBlock != value)
                {
                    m_mainBlock = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current block of the element.
        /// </summary>
        internal Block CurrentBlock
        {
            get
            {
                return m_curBlock;
            }
            set
            {
                if (m_curBlock != value)
                {
                    m_curBlock = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum width of the element.
        /// </summary>
        internal int MinWidth
        {
            get
            {
                return m_minWidth;
            }
            set
            {
                if (m_minWidth != value)
                {
                    m_minWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum height of the element.
        /// </summary>
        internal int MinHeight
        {
            get
            {
                return m_minHeight;
            }
            set
            {
                if (m_minHeight != value)
                {
                    m_minHeight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element is inside the table.
        /// </summary>
        internal bool InsideTable
        {
            get
            {
                return m_bInTable;
            }
            set
            {
                if (m_bInTable != value)
                {
                    m_bInTable = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element is disposed.
        /// </summary>
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return m_bDisposed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element has width attribute.
        /// </summary>
        /// <returns>True if width attribute is in the element.</returns>
        protected bool IsAttributeWidth
        {
            get
            {
                return this.Storage.Attributes[AttributeName.Width] != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element has height attribute.
        /// </summary>
        /// <returns>True if element has height attribute.</returns>
        protected bool IsAttributeHeight
        {
            get
            {
                return this.Storage.Attributes[AttributeName.Height] != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element has CSS width attribute.
        /// </summary>
        /// <returns>True if element has style attribute.</returns>
        protected internal bool IsStyleWidth
        {
            get
            {
                return !((HTMLFormat)this.Format).IsWidthDefault;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element has CSS height attribute.
        /// </summary>
        /// <returns>True if element has height in style attribute.</returns>
        protected internal bool IsStyleHeight
        {
            get
            {
                return !((HTMLFormat)this.Format).IsHeightDefault;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to skip the width of this element when getting width from parent.
        /// </summary>
        protected bool SkipWidth
        {
            get
            {
                return m_bSkipWidth;
            }
            set
            {
                if (m_bSkipWidth != value)
                {
                    m_bSkipWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets the indent values around the element.
        /// </summary>
        protected internal Space IndentSpace
        {
            get
            {
                return m_indentSpace;
            }
        }

        /// <summary>
        /// Gets or sets the document holder for this tag element.
        /// </summary>
        protected internal InputHTML Document
        {
            get
            {
                return m_document;
            }
            set
            {
                if (m_document != value)
                {
                    if (value == null)
                        throw new ArgumentNullException("Document");

                    m_document = value;
                }
            }
        }

        /// <summary>
        /// Gets the hashtable of names of attributes as keys and type of reaction on its
        /// changing as values.
        /// </summary>
        internal virtual ReactionCollection Reaction
        {
            get
            {
                return m_reaction;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element is CDATA element.
        /// </summary>
        protected internal bool IsCDATA
        {
            get
            {
                return this.Storage.FirstChild is XmlCDataSection;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element processes focus in the default way.
        /// </summary>
        protected internal bool FocusDefault
        {
            get
            {
                return m_bDefaultFocusing;
            }
            set
            {
                if (m_bDefaultFocusing != value)
                {
                    m_bDefaultFocusing = value;
                }
            }
        }
        #endregion

        #region Class events

        /// <summary>
        /// Event which is raised when run-time attribute has been changed.
        /// </summary>
        public event EventHandler RuntimeAttributeChanged;

        /// <summary>
        /// Utility event. Raised when parent property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler ParentChanged;

        /// <summary>
        /// Utility event. Raised when AccessKey property change.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler AccessKeyChanged;

        /// <summary>
        /// Utility event. Raised when UniqueID property changes.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler UniqueIDChanged;

        /// <summary>
        /// Utility event. Raised when InnerHTML property changes.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler InnerHTMLChanged;

        /// <summary>
        /// Utility event. Raised when Format property changes.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler FormatChanged;

        /// <summary>
        /// Event. Raised before the CSS style of the element (Format) is merged.
        /// </summary>
        public event PreStyleCalculatedEventHandler BeforeStyleCalculated;

        /// <summary>
        /// Event. Raised before the element or its part is painted.
        /// </summary>
        public event ElementPaintEventHandler Paint;

        /// <summary>
        /// Event. Raised when the size of the element is calculated.
        /// </summary>
        public event EventHandler SizeCalculated;

        /// <summary>
        /// Event. Raised when the location of the element is calculated.
        /// </summary>
        public event EventHandler LocationCalculated;

        /// <summary>
        /// Event. Raised when the mouse is clicked.
        /// </summary>
        [ElementEvent("RaiseClickEvent")]
        public event EventHandler Click;

        /// <summary>
        /// Event. Raises when the mouse double clicked.
        /// </summary>
        [ElementEvent("RaiseDoubleClickEvent")]
        public event EventHandler DoubleClick;

        /// <summary>
        /// Event. Raised when the mouse moves.
        /// </summary>
        [ElementEvent("RaiseMouseMoveEvent")]
        public event EventHandler MouseMove;

        /// <summary>
        /// Event. Raised when the mouse enters.
        /// </summary>
        [ElementEvent("RaiseMouseEnterEvent")]
        public event EventHandler MouseEnter;

        /// <summary>
        /// Event. Raised when the mouse leaves.
        /// </summary>
        [ElementEvent("RaiseMouseLeaveEvent")]
        public event EventHandler MouseLeave;

        /// <summary>
        /// Event. Raised when the mouse button is pressed down.
        /// </summary>
        [ElementEvent("RaiseMouseDownEvent")]
        public event EventHandler MouseDown;

        /// <summary>
        /// Event. Raised when Key Down.
        /// </summary>
        [ElementEvent("RaiseKeyDownEvent")]
        public event EventHandler KeyDown;

        /// <summary>
        /// Event. Raised when Key Up.
        /// </summary>
        [ElementEvent("RaiseKeyUpEvent")]
        public event EventHandler KeyUp;

        /// <summary>
        /// Event. Raised when key is pressed.
        /// </summary>
        [ElementEvent("RaiseKeyPressEvent")]
        public event EventHandler KeyPress;

        /// <summary>
        /// Event. Raised when the element gets focus.
        /// </summary>
        [ElementEvent("RaiseGotFocusEvent")]
        public event EventHandler GotFocus;

        /// <summary>
        /// Event. Raises when the element has lost focus.
        /// </summary>
        [ElementEvent("RaiseLeaveEvent")]
        public event EventHandler Leave;

        /// <summary>
        /// Event. Raised when the TabIndex property value has been changed.
        /// </summary>
        [ElementEvent("RaiseTabIndexChangedEvent")]
        public event EventHandler TabIndexChanged;

        /// <summary>
        /// Delegate. Raised when quiet mode property is changed.
        /// </summary>
        public event EventHandler QuietModeChanged;

        /// <summary>
        /// Event. Raised before the element is disposed.
        /// </summary>
        protected internal EventHandler BeforeDisposing;
        #endregion

        #region Class Static Methods

        /// <summary>
        /// Method according to metadata information with specified user type and an hash table of
        /// name-to-EventInfo data will return an array of supported event names.
        /// </summary>
        /// <param name="type">Type of information which must be used for infill.</param>
        /// <param name="events">Hashtable with name-to-EventInfo data.</param>
        /// <returns>String array of supported events.</returns>
        internal static string[] FillEventsInfo(Type type, out Hashtable events)
        {
            EventInfo[] typeEvents = type.GetEvents();
            ArrayList eventNames = new ArrayList();
            events = new Hashtable();

            // Fill hash by events.
            for (int i = 0; i < typeEvents.Length; i++)
            {
                EventInfo evnt = typeEvents[i] as EventInfo;

                ElementEventAttribute[] attr = (ElementEventAttribute[])evnt.GetCustomAttributes(
                  typeof(ElementEventAttribute), false);

                if (attr != null && attr.Length > 0)
                {
                    eventNames.Add(evnt.Name);

                    // Set additional run-time information.
                    attr[0].Event = evnt;
                    attr[0].RaiserMethod = type.GetMethod(attr[0].RaiserName, BindingFlags.Instance | BindingFlags.NonPublic);

                    events[evnt.Name] = attr[0];
                }
            }

            // Fill list of supported events.
            return (string[])eventNames.ToArray(typeof(string));
        }

        /// <summary>
        /// Infills hash by attribute names and their reaction to changing types.
        /// </summary>
        /// <returns>Collection of reactions to attributes changing.</returns>
        internal static ReactionCollection FillAttributesInfo()
        {
            ReactionCollection collection = new ReactionCollection();

            FillAttributes(collection, typeof(AttributeName));
            FillAttributes(collection, typeof(HTMLFormat));
            FillAttributes(collection, typeof(BaseElement));

            return collection;
        }

        /// <summary>
        /// Infills simple attributes of the element.
        /// </summary>
        /// <param name="collection">Collection which holds information.</param>
        /// <param name="type">Type of attributes holder.</param>
        internal static void FillAttributes(ReactionCollection collection, Type type)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (type == null)
                throw new ArgumentNullException("type");

            AttributeHolderAttribute[] attrHolder = (AttributeHolderAttribute[])type.GetCustomAttributes(
              typeof(AttributeHolderAttribute), false);

            if (attrHolder != null && attrHolder.Length > 0)
            {
                // Name of attribute holder type.
                string holderName = attrHolder[0].TypeName;

                // Get all fields of type.
                FieldInfo[] finfo = type.GetFields();

                for (int i = 0, len = finfo.Length; i < len; i++)
                {
                    FieldInfo field = finfo[i] as FieldInfo;

                    ReactionTypeAttribute[] attr = (ReactionTypeAttribute[])field.GetCustomAttributes(
                      typeof(ReactionTypeAttribute), false);

                    if (attr != null && attr.Length > 0)
                    {
                        collection.Add(holderName, attr[0].Name, attr[0].Reaction);
                    }
                }

                // Get all properties of type.
                PropertyInfo[] pinfo = type.GetProperties();

                for (int i = 0, len = pinfo.Length; i < len; i++)
                {
                    PropertyInfo field = pinfo[i] as PropertyInfo;

                    ReactionTypeAttribute[] attr = (ReactionTypeAttribute[])field.GetCustomAttributes(
                      typeof(ReactionTypeAttribute), false);

                    if (attr != null && attr.Length > 0)
                    {
                        collection.Add(holderName, attr[0].Name, attr[0].Reaction);
                    }
                }
            }
        }

        /// <summary>
        /// Assigns the specified graphics object for document elements text calculation.
        /// </summary>
        /// <param name="g">New graphics context.</param>
        /// <returns>Old graphics object used by elements.</returns>
        internal static Graphics SelectGraphics(Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            Graphics old = Graphics;
            _graphics = g;

            return old;
        }
        #endregion

        #region Class initialize/finalize methods

        /// <summary>
        /// Initializes static members of the BaseElement class
        /// </summary>
        static BaseElement()
        {
            m_reaction = FillAttributesInfo();

            _stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
            _stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces |
              StringFormatFlags.NoClip;
            _stringFormat.Trimming = StringTrimming.None;
            _stringFormat.SetTabStops(0.0f, new float[] { DEF_TAB_STOP_VALUE });
        }

        /// <summary>
        /// Prevents a default instance of the BaseElement class from being created.
        /// </summary>
        private BaseElement()
        {
            InitializeCollections();
        }

        /// <summary>
        /// Initializes a new instance of the BaseElement class
        /// </summary>
        /// <param name="parent">Parent of the element.</param>
        /// <param name="name">Name of the element tag.</param>
        protected BaseElement(IHTMLElement parent, string name)
            : this()
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name can not be empty");

            m_parent = parent;
            m_strName = name;
        }

        /// <summary>
        /// Initializes a new instance of the BaseElement class
        /// </summary>
        /// <param name="parent">Parent element for element.</param>
        /// <param name="name">Name of the tag.</param>
        /// <param name="document">Document instance.</param>
        protected BaseElement(IHTMLElement parent, string name, InputHTML document)
            : this(parent, name)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            m_document = document;
        }

        /// <summary>
        /// Initializes a new instance of the BaseElement class
        /// </summary>
        /// <param name="control">Control object.</param>
        protected BaseElement(HTMLUIControl control)
            : this()
        {
            if (control == null)
                throw new ArgumentNullException("control");

            m_control = control;
        }

        /// <summary>
        /// Finalizes an instance of the BaseElement class
        /// </summary>
        ~BaseElement()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes all resources and invokes this method in all child elements.
        /// </summary>
        public void Dispose()
        {
            if (!m_bDisposed)
            {
                OnBeforeDisposing(EventArgs.Empty);

                OnDispose();
                m_bDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Method must be overridden by inheritors if it has some additional
        /// resources to dispose.
        /// </summary>
        protected virtual void OnDispose()
        {
            if (this.Control != null && this.Document != null)
            {
                this.Document.RemoveElement(this);
            }

            //// Invoke dispose method in all child elements before clearing this collection.
            if (m_children != null)
            {
                DisposeChildren();
                m_children.Clear(false);
                m_children = null;
            }

            if (m_storage != null)
            {
                //// Remove XML data from the parent.
                BaseElement parentEx = m_parent as BaseElement;

                if (parentEx != null && parentEx.Storage != null &&
                 parentEx.Storage == m_storage.ParentNode)
                {
                    parentEx.Storage.RemoveChild(m_storage);
                }

                m_storage.RemoveAll();
                m_storage = null;
            }

            if (m_arrRuntimeAttr != null)
            {
                m_arrRuntimeAttr.Clear();
                m_arrRuntimeAttr = null;
            }

            if (m_format != null && !m_format.IsDisposed)
            {
                m_format.OnChanged -= new BeforeValueChangeEventHandler(Attributes_Changed);

                if (m_format.DisposeWithElement)
                {
                    m_format.Dispose();
                }

                m_format = null;
            }

            if (m_events != null)
            {
                m_events.Clear();
                m_events = null;
            }

            //// Remove element from tab collection.
            if (m_document != null)
            {
                SyncTabStopCollection(-1);
            }

            if (m_attributes != null)
            {
                m_attributes.Changed -= new BeforeValueChangeEventHandler(Attributes_Changed);
                m_attributes.Clear();
                m_attributes = null;
            }

            if (m_ownFormat != null)
            {
                m_ownFormat.Dispose();
                m_ownFormat = null;
            }

            if (m_textSizeHash != null)
            {
                m_textSizeHash.Clear();
                m_textSizeHash = null;
            }

            if (m_blocks != null)
            {
                m_blocks.Clear();
                m_blocks = null;
            }

            m_oldCursor = null;
            m_mainBlock = null;
            m_curBlock = null;
        }

        /// <summary>
        /// Disposes all child elements.
        /// </summary>
        private void DisposeChildren()
        {
            if (m_children == null || m_children.Count == 0) return;

            BaseElement child = null;
            object el = null;

            for (int i = 0, len = m_children.Count; i < len; i++)
            {
                el = m_children[i];
                if (el is BaseElement)
                {
                    child = el as BaseElement;
                    child.Dispose();
                }
            }
        }

        /// <summary>
        /// Initializes all collections and variables.
        /// </summary>
        protected void InitializeCollections()
        {
            m_arrSupEvents = new ArrayList(this.SupportedEvents);
            m_arrSupEvents.Sort(DEF_COMPARER);

            m_arrRuntimeAttr = new ArrayList(this.RuntimeAttributes);
            m_arrRuntimeAttr.Sort(DEF_COMPARER);

            m_children = new HTMLElementsCollection(this);
            m_events = new HTMLEventsCollection(this);

            m_attributes = new HTMLAttributesCollection(this);
            m_attributes.Changed += new BeforeValueChangeEventHandler(Attributes_Changed);

            m_textSizeHash = new Hashtable();
            m_blocks = new BlocksCollection();

            m_isResizable = true;
            m_isBlockStructure = false;
            m_isPostponed = false;
            m_skipWhitespaces = true;
            m_size = Size.Empty;
            m_location = Point.Empty;
            m_elementType = ElementType.InLine;
            m_bIsVisible = true;
            m_bDisposed = false;
            m_indentSpace = new Space(this);
            m_textMinWidth = 0;
        }
        #endregion

        #region Class Public Methods

        /// <summary>
        /// Disables event raising by element.
        /// </summary>
        public void BeginUpdate()
        {
            this.QuietMode = true;
        }

        /// <summary>
        /// Enables event raising by element.
        /// </summary>
        public void EndUpdate()
        {
            this.QuietMode = false;
        }

        /// <summary>
        /// Sets input focus to the element.
        /// </summary>
        /// <returns>True if the input focus request was successful; false otherwise.</returns>
        public bool Focus()
        {
            bool result;

            if (this.TabIndex < 0)
            {
                result = false;
            }
            else
            {
                result = FocusElementInternal();
            }

            if (result)
            {
                result = this.Document.FocusManager.SetFocus(this);
            }

            return result;
        }

        /// <summary>
        /// Applies the specified format to the element.
        /// </summary>
        /// <param name="format">Format to be attached to the element.</param>
        public void ApplyFormat(IHTMLFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            HTMLFormat formatEx = format as HTMLFormat;

            formatEx.DisposeWithElement = false;

            this.Format = format;
        }
        #endregion

        #region Class event raisers

        /// <summary>
        /// Raises the RuntimeAttributeChanged event.
        /// </summary>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="args">Event arguments.</param>
        protected void RaiseRuntimeAttributeChanged(string name, ValueChangedEventArgs args)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            if (RuntimeAttributeChanged != null && !this.QuietMode)
            {
                BeforeValueChangedEventArgs arg1 = new BeforeValueChangedEventArgs(
                  name, args);

                RuntimeAttributeChanged(this, arg1);
            }
        }

        /// <summary>
        /// Raises the ParentChanged event on parent property value change.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseParentChanged(ValueChangedEventArgs args)
        {
            if (ParentChanged != null && !this.QuietMode)
            {
                ParentChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the AccesssKeysChange event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseAccessKeyChanged(ValueChangedEventArgs args)
        {
            if (AccessKeyChanged != null && !this.QuietMode)
            {
                AccessKeyChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the UniqueIDChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseUniqueIDChanged(ValueChangedEventArgs args)
        {
            if (UniqueIDChanged != null && !this.QuietMode)
            {
                UniqueIDChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the InnerHTMLChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseInnerHTMLChanged(ValueChangedEventArgs args)
        {
            if (InnerHTMLChanged != null && !this.QuietMode)
            {
                InnerHTMLChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the FormatChanged event.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseFormatChanged(ValueChangedEventArgs args)
        {
            if (FormatChanged != null && !this.QuietMode)
            {
                FormatChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the BeforeStyleCalculated event.
        /// </summary>
        /// <param name="args">PreStyleCalculatedEventArgs instance</param>
        protected void RaiseBeforeStyleCalculated(PreStyleCalculatedEventArgs args)
        {
            if (BeforeStyleCalculated != null && !this.QuietMode)
            {
                BeforeStyleCalculated(this, args);
            }
        }

        /// <summary>
        /// Raises the SizeCalculated event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected void RaiseSizeCalculated(EventArgs args)
        {
            if (SizeCalculated != null && !this.QuietMode)
            {
                SizeCalculated(this, args);
            }
        }

        /// <summary>
        /// Raises the LocationCalculated event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected void RaiseLocationCalculated(EventArgs args)
        {
            if (LocationCalculated != null && !this.QuietMode)
            {
                LocationCalculated(this, args);
            }
        }

        /// <summary>
        /// Raises the OnPaint event.
        /// </summary>
        /// <param name="args">Paint arguments.</param>
        /// <returns>True if someone is subscribed on this event; otherwise False.</returns>
        protected bool RaiseOnPaint(ElementPaintEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            if (Paint != null && !this.QuietMode)
            {
                Paint(this, args);

                return Paint.GetInvocationList().Length > 0;
            }

            return false;
        }

        /// <summary>
        /// Raises events on inherited elements.
        /// </summary>
        /// <param name="evnt">Event delegate.</param>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        /// <param name="bBubble">Indicates whether to bubble event to the top.</param>
        internal virtual void RaiseBubblingEvent(Delegate evnt, string name, EventArgs args, bool bBubble)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            if (this.QuietMode) return;

            BubblingEventArgs arg;

            if ((args as BubblingEventArgs) == null)
            {
                arg = new BubblingEventArgs(this, args);
                arg.Bubbling = bBubble;
            }
            else
            {
                arg = args as BubblingEventArgs;
            }

            if (evnt != null)
            {
                Delegate[] delegates = evnt.GetInvocationList();
                object[] param = new object[] { this, arg };

                for (int i = 0; i < delegates.Length; i++)
                {
                    delegates[i].Method.Invoke(delegates[i].Target, param);

                    bBubble = (!arg.Bubbling) ? false : bBubble;
                }
            }

            if (Parent != null && bBubble)
            {
                BaseElement parent = GetParentForEvent(name);
                if (parent != null)
                {
                    ((HashElementEvents)parent.Events[name]).RaiseEvent(arg);
                }
            }
        }

        /// <summary>
        /// Raises events on inherited elements.
        /// </summary>
        /// <param name="evnt">Event delegate.</param>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        internal virtual void RaiseBubblingEvent(Delegate evnt, string name, EventArgs args)
        {
            RaiseBubblingEvent(evnt, name, args, true);
        }

        /// <summary>
        /// Raises the Mouse Click event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseClickEvent(EventArgs args)
        {
            RaiseBubblingEvent(Click, EventName.Click, args);
        }

        /// <summary>
        /// Raises the Mouse Double Click event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseDoubleClickEvent(EventArgs args)
        {
            RaiseBubblingEvent(DoubleClick, EventName.DoubleClick, args);
        }

        /// <summary>
        /// Raises the Mouse Move event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseMouseMoveEvent(EventArgs args)
        {
            RaiseBubblingEvent(MouseMove, EventName.MouseMove, args);
        }

        /// <summary>
        /// Raises the Mouse Enter event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseMouseEnterEvent(EventArgs args)
        {
            RaiseBubblingEvent(MouseEnter, EventName.MouseEnter, args);

            m_oldCursor = this.Control.Cursor;
            this.Control.Cursor = this.Format.Cursor;

            ShowToolTip();
        }

        /// <summary>
        /// Raises the Mouse Leave event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseMouseLeaveEvent(EventArgs args)
        {
            RaiseBubblingEvent(MouseLeave, EventName.MouseLeave, args);

            this.Control.Cursor = m_oldCursor;
            HideToolTip();
        }

        /// <summary>
        /// Raises the Mouse Move Down event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseMouseDownEvent(EventArgs args)
        {
            RaiseBubblingEvent(MouseDown, EventName.MouseDown, args);
        }

        /// <summary>
        /// Raises the Key Down event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseKeyDownEvent(EventArgs args)
        {
            RaiseBubblingEvent(KeyDown, EventName.KeyDown, args);
        }

        /// <summary>
        /// Raises the Key Up event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseKeyUpEvent(EventArgs args)
        {
            RaiseBubblingEvent(KeyUp, EventName.KeyUp, args);
        }

        /// <summary>
        /// Raises the Key Press event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseKeyPressEvent(EventArgs args)
        {
            RaiseBubblingEvent(KeyPress, EventName.KeyPress, args);
        }

        /// <summary>
        /// Raises the GotFocus event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseGotFocusEvent(EventArgs args)
        {
            RaiseBubblingEvent(GotFocus, EventName.GotFocus, args, false);
        }

        /// <summary>
        /// Raises the Leave event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseLeaveEvent(EventArgs args)
        {
            RaiseBubblingEvent(Leave, EventName.Leave, args, false);
        }

        /// <summary>
        /// Raises the TabIndexChanged event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected internal virtual void RaiseTabIndexChangedEvent(EventArgs args)
        {
            RaiseBubblingEvent(TabIndexChanged, EventName.TabIndexChanged, args, false);
        }

        /// <summary>
        /// Raises the Quite Mode Changed event when mode changes.
        /// </summary>
        protected void RaiseQuietModeChangedEvent()
        {
            if (QuietModeChanged != null)
            {
                QuietModeChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the BeforeDisposing event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void RaiseBeforeDisposing(EventArgs args)
        {
            if (BeforeDisposing != null && !this.QuietMode)
            {
                BeforeDisposing(this, args);
            }
        }

        /// <summary>
        /// Raises the Leave Focus event.
        /// </summary>
        protected internal virtual void LeaveFocus()
        {
            RaiseLeaveEvent(EventArgs.Empty);

            if (this.FocusDefault)
            {
                this.Document.FocusManager.ClearPrevFocusRect();
            }
        }
        #endregion

        #region Class abstract part

        /// <summary>
        /// Gets the list of events supported by the element.
        /// </summary>
        [Browsable(false)]
        public abstract string[] SupportedEvents { get; }

        /// <summary>
        /// Returns an instance of the event class with the specified name. Name is previously checked
        /// if it is supported or not.
        /// </summary>
        /// <param name="name">Name of the event. Case insensitive.</param>
        /// <returns>Instance of the event class.</returns>
        protected abstract IHTMLEvent CreateEventInternal(string name);

        /// <summary>
        /// Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size object</returns>
        protected abstract Size CalculateSizeInternal();

        /// <summary>
        /// Calculates the format of the element from an array of possible formats.
        /// </summary>
        protected abstract void CalculateFormatInternal();

        /// <summary>
        /// Calculates the element's position for rendering.
        /// </summary>
        protected abstract void CalculatePositionInternal();
        #endregion

        #region Class overrides

        /// <summary>
        /// Gets a list of attributes which is known to the element as
        /// run-time properties.
        /// </summary>
        [Browsable(false)]
        public virtual string[] RuntimeAttributes
        {
            get
            {
                return DEF_RUNTIME;
            }
        }

        /// <summary>
        /// Creates an instance of the event class which knows how to attach
        /// user delegates to the element internal event.
        /// </summary>
        /// <param name="name">Name of the event. Case insensitive.</param>
        /// <returns>NULL if event is not supported; instance of the event class otherwise.</returns>
        public IHTMLEvent CreateEvent(string name)
        {
            if (!IsEventSupported(name)) return null;

            return CreateEventInternal(name);
        }

        /// <summary>
        /// Raises RuntimeAttributeChanged event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnRuntimeAttributeChanged(string name, ValueChangedEventArgs args)
        {
            RaiseRuntimeAttributeChanged(name, args);
        }

        /// <summary>
        /// Called by parent property set part. This is best place for
        /// any logic which must control parent property changes (for inheritors).
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnParentChanged(ValueChangedEventArgs args)
        {
            RaiseParentChanged(args);
        }

        /// <summary>
        /// Called when AccessKey property changes. This is the best
        /// place for custom logic.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnAccessKeyChanged(ValueChangedEventArgs args)
        {
            RaiseAccessKeyChanged(args);
        }

        /// <summary>
        /// Called when UniqueID property is changed. This is the best place for
        /// custom logic.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnUniqueIDChanged(ValueChangedEventArgs args)
        {
            RaiseUniqueIDChanged(args);
        }

        /// <summary>
        /// When InnerHTML property changes, this method will be called in the
        /// ReparseInnerHTML class method, which makes all others work for you.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnInnerHTMLChanged(ValueChangedEventArgs args)
        {
            ReparseInnerHTML(args.NewValue as string);
            RaiseInnerHTMLChanged(args);
        }

        /// <summary>
        /// Raises the FormatChanged event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnFormatChanged(ValueChangedEventArgs args)
        {
            RaiseFormatChanged(args);

            OnFormatChanged(this, args.OldValue as HTMLFormat, args.NewValue as HTMLFormat);
        }

        /// <summary>
        /// Raises the BeforeStyleCalculated event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnBeforeStyleCalculated(PreStyleCalculatedEventArgs args)
        {
            RaiseBeforeStyleCalculated(args);
        }

        /// <summary>
        /// Raises the SizeCalculated event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnSizeCalculated(EventArgs args)
        {
            RaiseSizeCalculated(args);
        }

        /// <summary>
        /// Raises the LocationCalculated event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnLocationCalculated(EventArgs args)
        {
            RaiseLocationCalculated(args);
        }

        /// <summary>
        /// Raises the Paint event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        /// <returns>True if someone is subscribed on this event; false otherwise.</returns>
        protected virtual bool OnPaint(ElementPaintEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            return RaiseOnPaint(args);
        }

        /// <summary>
        /// Raised when QuietMode property is changed.
        /// </summary>
        protected virtual void OnQuietModeChanged()
        {
            RaiseQuietModeChangedEvent();
        }

        /// <summary>
        /// Raises the BeforeDisposing event.
        /// </summary>
        /// <param name="args">Event data.</param>
        protected virtual void OnBeforeDisposing(EventArgs args)
        {
            RaiseBeforeDisposing(args);
        }

        /// <summary>
        /// Builds the string which represents InnerHTML of the current element.
        /// </summary>
        /// <returns>Inner XML content.</returns>
        protected virtual string CalculateInnerHTML()
        {
            if (m_storage == null)
                throw new ArgumentException("Class not properly initialized. Must be called method InfillFromXMLElement.");

            return m_storage.InnerXml;
        }

        /// <summary>
        /// When InnerHTML changes, this method code reparses the InnerHTML string and
        /// builds a new sub-elements tree.
        /// </summary>
        /// <param name="htmlToParse">Here we put HTML which must be first converted to XHTML and
        /// then set as InnerXML of the current storage.</param>
        protected virtual void ReparseInnerHTML(string htmlToParse)
        {
            if (m_children != null) m_children.Clear();

            if (htmlToParse != null)
            {
                XmlElement elm = HTMLUIControl.HTMLParser.ParseString(htmlToParse);

                if (!this.IsCDATA)
                {
                    m_storage.InnerXml = elm.InnerXml;
                }
                else
                {
                    XmlCDataSection sect = m_storage.FirstChild as XmlCDataSection;

                    sect.Data = elm.InnerXml;
                }

                ReConvertInnerHTML(m_storage, this.Parent);

                if (!this.QuietMode) ReCalculateDocument();
            }
        }

        /// <summary>
        /// Reconverts all children to tree of objects.
        /// </summary>
        /// <param name="xmlCurrent">Current XML element.</param>
        /// <param name="elementParent">Parent HTML element object.</param>
        /// <returns>Created HTML element object.</returns>
        protected virtual IHTMLElement ReConvertInnerHTML(XmlElement xmlCurrent, IHTMLElement elementParent)
        {
            if (xmlCurrent == null)
                throw new ArgumentNullException("xmlCurrent");

            BaseElement currentTagElement;
            if (elementParent == this.Parent)
            {
                currentTagElement = this;
            }
            else
            {
                return elementParent.Control.ConvertDocument(xmlCurrent, elementParent as BaseElement, null);
            }

            if (xmlCurrent.HasChildNodes)
            {
                XmlNodeList childNodeList = xmlCurrent.ChildNodes;

                for (int i = 0; i < childNodeList.Count; i++)
                {
                    XmlNode childNode = childNodeList[i];

                    if (childNode is System.Xml.XmlElement)
                    {
                        XmlElement childXmlElement = (XmlElement)childNode;

                        BaseElement childElm = currentTagElement.Control.ConvertDocument(
                          childXmlElement, currentTagElement, null);

                        if (childElm != null && childElm.Document != null)
                        {
                            // set quite mode for disabling all reaction on format changing.
                            bool quiteMode = childElm.Document.QuietMode;
                            childElm.Document.QuietMode = true;

                            childElm.Document.RecreateFormatElement(childElm);

                            childElm.Document.Reaction = ReactLevel.None;
                            childElm.Document.QuietMode = quiteMode;
                        }
                    }
                }
            }

            return currentTagElement;
        }

        /// <summary>
        /// Gets the declaration of the element.
        /// </summary>
        protected virtual string ElementOpenPart
        {
            get
            {
                return "<" + m_strName + " " + ElementAttributesPart + ">";
            }
        }

        /// <summary>
        /// Gets the declaration of attributes.
        /// </summary>
        protected virtual string ElementAttributesPart
        {
            get
            {
                string[] attr = new string[m_attributes.Count];

                for (int i = 0; i < m_attributes.Count; i++)
                {
                    IHTMLAttribute attrib = m_attributes[i];
                    attr[i] = ((HTMLAttributeImpl)attrib).ToString();
                }

                return string.Join(" ", attr);
            }
        }

        /// <summary>
        /// Gets the close part of the HTML element tag.
        /// </summary>
        protected virtual string ElementClosePart
        {
            get
            {
                return "</" + m_strName + ">";
            }
        }

        /// <summary>
        /// Converts from XML element to our own elements.
        /// </summary>
        /// <param name="control">HTMLUI control</param>
        /// <param name="element">XmlElement instance</param>
        public virtual void InfillFromXMLElement(HTMLUIControl control, XmlElement element)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (element == null)
                throw new ArgumentNullException("element");

            this.QuietMode = true;

            m_control = control;
            m_storage = element;

            if (m_storage.HasAttributes)
            {
                XmlAttributeCollection attrList = m_storage.Attributes;

                for (int i = 0; i < attrList.Count; i++)
                {
                    XmlNode node = attrList[i];

                    if (node is XmlAttribute)
                    {
                        XmlAttribute attr = node as XmlAttribute;
                        this.Attributes.Add(attr.Name, attr.Value);
                    }
                }
            }

            // Define run-time attributes.
            this.IsVisible = this.IsVisible;
            this.Size = this.Size;
            this.Location = this.Location;

            this.QuietMode = false;
        }

        /// <summary>
        /// Overloaded. Calculates the size of the rectangle which is needed to output the string text.
        /// </summary>
        /// <param name="str">String for measuring.</param>
        /// <param name="format">Format object.</param>
        /// <param name="maxLength">Max length for the string.</param>
        /// <returns>Size for this text.</returns>
        protected Size MeasureString(string str, HTMLFormat format, int maxLength)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (format == null)
                throw new ArgumentNullException("format");

            if (str.Length == 0) return Size.Empty;

            maxLength = GetRealMaxWidth(str, maxLength, format);
            /*SizeF size = BaseElement.Graphics.MeasureString(
               str,
               format.Font,
               maxLength,
               _stringFormat );*/

            ////*
            CharacterRange[] characterRanges =
      {
        new CharacterRange( 0, str.Length )
      };
            Region[] stringRegions;

            _stringFormat.SetMeasurableCharacterRanges(characterRanges);
            stringRegions = BaseElement.Graphics.MeasureCharacterRanges(str, format.Font, new Rectangle(0, 0, maxLength, int.MaxValue), _stringFormat);

            RectangleF rect = stringRegions[0].GetBounds(BaseElement.Graphics);
            if (!this.Document.IsPrinting)
            {
                rect.Width += 2 * rect.X;
                rect.Height += 2 * rect.Y;
            }
            return Size.Ceiling(rect.Size);
            //// return Size.Ceiling( size );
        }

        /// <summary>
        /// Calculates the size of rectangle which is needed to output the string text.
        /// </summary>
        /// <param name="str">String to be measured.</param>
        /// <param name="font">Font object.</param>
        /// <returns>Size for this text.</returns>
        protected Size MeasureString(string str, Font font)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (font == null)
                throw new ArgumentNullException("font");

            if (str.Length == 0) return Size.Empty;

            PointF point = PointF.Empty;
            //// SizeF size = BaseElement.Graphics.MeasureString( str, font, point, _stringFormat );

            //// *
            CharacterRange[] characterRanges =
      {
        new CharacterRange( 0, str.Length )
      };
            Region[] stringRegions = null;

            _stringFormat.SetMeasurableCharacterRanges(characterRanges);
            stringRegions = BaseElement.Graphics.MeasureCharacterRanges(str, font, new Rectangle(0, 0, int.MaxValue, int.MaxValue), _stringFormat);

            RectangleF rect = stringRegions[0].GetBounds(BaseElement.Graphics);
            if (!this.Document.IsPrinting)
            {
                rect.Width += 2 * rect.X;
                rect.Height += 2 * rect.Y;
            }
            return Size.Ceiling(rect.Size);
            //// return Size.Ceiling( size );
        }

        /// <summary>
        /// Calculates the size of rectangle which is needed to output the string text.
        /// </summary>
        /// <param name="str">String to be measured.</param>
        /// <param name="font">Font object.</param>
        /// <param name="maxLength">Max length of the text.</param>
        /// <returns>Size for this text.</returns>
        protected Size MeasureString(string str, Font font, int maxLength)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (font == null)
                throw new ArgumentNullException("font");

            if (str.Length == 0) return Size.Empty;

            PointF point = PointF.Empty;
            ////SizeF size = BaseElement.Graphics.MeasureString( str, font, maxLength, _stringFormat );

            ////*
            CharacterRange[] characterRanges =
      {
        new CharacterRange( 0, str.Length )
      };
            Region[] stringRegions;

            _stringFormat.SetMeasurableCharacterRanges(characterRanges);

            stringRegions = BaseElement.Graphics.MeasureCharacterRanges(str, font, new Rectangle(0, 0, maxLength, int.MaxValue), _stringFormat);

            RectangleF rect = stringRegions[0].GetBounds(BaseElement.Graphics);
            if (!this.Document.IsPrinting)
            {
                rect.Width += 2 * rect.X;
                rect.Height += 2 * rect.Y;
            }

            return Size.Ceiling(rect.Size);
            ////*/

            //// return Size.Ceiling( size );
        }

        /// <summary>
        /// Returns the real max size of the string.
        /// </summary>
        /// <param name="str">String text.</param>
        /// <param name="max">Max width.</param>
        /// <param name="format">Format object.</param>
        /// <returns>Real max width for the text.</returns>
        protected virtual int GetRealMaxWidth(string str, int max, HTMLFormat format)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length == 0)
                throw new ArgumentException("str - string can not be empty");

            if (format == null)
                throw new ArgumentNullException("format");

            // if we already know maximum word length in the element.
            if (m_textMinWidth > 0)
            {
                this.MinWidth = Math.Max(this.MinWidth, m_textMinWidth);
                return Math.Max(max, m_textMinWidth);
            }

            ArrayList words = InFillBlocks(str);
            string token = string.Empty;

            for (int i = 0; i < words.Count; i++)
            {
                token = (string)words[i];
                Size size = MeasureString(token, format.Font, max);
                int wordLength = size.Width;

                m_textMinWidth = Math.Max(m_textMinWidth, wordLength);
                max = Math.Max(max, wordLength);
            }

            this.MinWidth = Math.Max(this.MinWidth, m_textMinWidth);

            return max;
        }

        /// <summary>
        /// Changes its parent current position.Needed if element makes space before itself.
        /// </summary>
        /// <param name="currentPos">Global current position.</param>
        protected virtual void MoveStartCurPos(ref Point currentPos)
        {
            // Do nothing here, used only by inheritors.
        }

        /// <summary>
        /// Changes its parent current position. Needed if element makes space after itself.
        /// </summary>
        /// <param name="currentPos">Global current position.</param>
        protected virtual void MoveFinalCurPos(ref Point currentPos)
        {
            // Do nothing here, used only by inheritors.
        }

        /// <summary>
        /// Virtual method for initialization of the element. It is invoked after element creating.
        /// By overriding this method, element can initialize its special properties.
        /// </summary>
        protected internal virtual void InitializeElement()
        {
            SyncTabStopCollection(this.TabIndex);
        }

        /// <summary>
        /// Focuses the element.
        /// </summary>
        /// <returns>True if the input focus request was successful; false otherwise.</returns>
        protected virtual bool FocusElementInternal()
        {
            RaiseGotFocusEvent(EventArgs.Empty);

            if (this.FocusDefault)
            {
                this.Control.ScrollToElement(this);
                this.Document.FocusManager.DrawFocusRect(this);
                this.Control.Focus();
            }

            return true;
        }
        #endregion

        #region Class utility methods

        /// <summary>
        /// Returns the parent element which supports the specified event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>Parent element for the event.</returns>
        protected BaseElement GetParentForEvent(string eventName)
        {
            if (eventName == null)
                throw new ArgumentNullException("eventName");

            if (eventName.Length == 0)
                throw new ArgumentException("eventName - string can not be empty");

            if (this.Parent == null) return null;

            BaseElement parent = (BaseElement)this.Parent;
            while (true)
            {
                if (parent == null) return null;
                if (parent.IsEventSupported(eventName)) return parent;

                parent = (BaseElement)parent.Parent;
            }
        }

        /// <summary>
        /// Indicates whether the specified name belongs to the list of supported events.
        /// </summary>
        /// <param name="name">Name which must be checked. Case insensitive.</param>
        /// <returns>True if event is supported; false otherwise.</returns>
        public bool IsEventSupported(string name)
        {
            return m_arrSupEvents.BinarySearch(name, DEF_COMPARER) >= 0;
        }

        /// <summary>
        /// Indicates whether the specified name of the attribute belongs to list of run-time attributes.
        /// </summary>
        /// <param name="name">Name which must be checked. Case insensitive.</param>
        /// <returns>True if attribute with specified name is a run-time attribute;
        /// false otherwise.</returns>
        public bool IsAttributeRuntime(string name)
        {
            return m_arrRuntimeAttr.BinarySearch(name, DEF_COMPARER) >= 0;
        }

        /// <summary>
        /// Returns the element level in the current document.
        /// </summary>
        /// <param name="element">Element whose level is needed.</param>
        /// <returns>Level of element in the document tree.</returns>
        public static int GetElementLevel(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            int result = 0;

            if (element.Parent != null)
                result += GetElementLevel(element.Parent);

            return result;
        }

        /// <summary>
        /// Calculates the format of elements from an array of possible formats.
        /// </summary>
        public void CalculateFormat()
        {
            if (m_format != null)
            {
                m_format.Dispose();
                m_format = null;
            }

            CalculateFormatInternal();
        }

        /// <summary>
        /// Calculates the position of the element.
        /// </summary>
        public void CalculatePosition()
        {
            CalculatePositionInternal();
        }

        /// <summary>
        /// Returns an array of formats by its Unique ID.
        /// </summary>
        /// <param name="formatsHash">Hash of formats.</param>
        /// <returns>Array of formats by UniqueID.</returns>
        protected ArrayList GetArray(Hashtable formatsHash)
        {
            if (formatsHash == null)
                throw new ArgumentNullException("formatsHash");

            object result = formatsHash[this.UniqueID];

            if (result == null)
            {
                result = new ArrayList();
                formatsHash[this.UniqueID] = result;
            }

            result = (result as ArrayList).Clone();

            return result as ArrayList;
        }

        /// <summary>
        /// Inserts own format into the format array.
        /// </summary>
        /// <param name="formats">Array of formats.</param>
        /// <param name="formatsHash">Hashtable of formats.</param>
        protected void SetOwnFormat(ArrayList formats, Hashtable formatsHash)
        {
            // Add format from attributes of the element.
            formats.Insert(0, this.OwnFormat);
            formats.Insert(1, GetFormatFromAttributes());
        }

        /// <summary>
        /// Calculates the format of the element from array of possible formats.
        /// </summary>
        protected void DefaultCalculateFormatInternal()
        {
            HTMLFormat defaultFormat = GetInheritedFormat();
            Hashtable formatsHash = this.Document.GetCSSFormatsToElementHash();
            ArrayList formats = GetArray(formatsHash);

            // Insert into an array of possible formats own special format.
            SetOwnFormat(formats, formatsHash);

            SetFirstFormat(formats);

            // Raise event for user changing collection.
            PreStyleCalculatedEventArgs args = new PreStyleCalculatedEventArgs(formats);
            OnBeforeStyleCalculated(args);

            // Final format for element.
            HTMLFormat finalFormat = HTMLFormat.MergeFormats(formats, defaultFormat, (HTMLFormat)Control.DefaultFormat);
            finalFormat.Type = FormatType.Merged;
            finalFormat.Name = this.UniqueID + "_" + this.Name;
            finalFormat.IsMerged = true;

            //// Remove format which is created from tag attributes. It is first.
            if (formats.Count > 0)
            {
                formats.RemoveAt(0);
            }

            formats.Remove(this.OwnFormat);

            finalFormat.QuietMode = false;
            SetFormat(finalFormat);

            this.IsVisible = !finalFormat.DisplayNone;
        }

        /// <summary>
        /// Inserts the first format in the collection.
        /// </summary>
        /// <param name="formats">Array of formats.</param>
        protected virtual void SetFirstFormat(ArrayList formats)
        {
        }

        /// <summary>
        /// Shows ToolTip on the element.
        /// </summary>
        protected virtual void ShowToolTip()
        {
            if (!this.Attributes.Contains(AttributeName.Title)) return;

            IHTMLAttribute attr = this.Attributes[AttributeName.Title];
            if (attr == null || attr.Value.Length == 0) return;

            this.Control.ToolTip.SetToolTip(this.Control, attr.Value);
            this.Control.ToolTip.Active = true;
        }

        /// <summary>
        /// Hides ToolTip on the element.
        /// </summary>
        protected void HideToolTip()
        {
            this.Control.ToolTip.Active = false;
            this.Control.ToolTip.RemoveAll();
        }

        /// <summary>
        /// Returns the width of the first nearest parent which has a block structure (IsBlock == True).
        /// </summary>
        /// <returns>Width according to parent width.</returns>
        protected virtual int GetWidthFromParent()
        {
            int result = 0;
            if (this.Parent != null && this != this.Document.RenderRoot)
            {
                BaseElement parent = (BaseElement)this.Parent;
                while (true)
                {
                    if (parent == null)
                    {
                        result = this.Document.ClientSize.Width;
                        break;
                    }

                    if (parent.IsBlock && parent.Width > 0 && !parent.SkipWidth)
                    {
                        result = parent.Width;
                        break;
                    }

                    parent = (BaseElement)parent.Parent;
                }
            }
            else
            {
                result = this.Document.ClientSize.Width; ////this.Width;
            }

            return Math.Max(result, this.MinWidth);
        }

        /// <summary>
        /// Returns the height of the first nearest parent which has a block structure (IsBlock == True).
        /// </summary>
        /// <returns>Height of the element according to height of parent.</returns>
        protected virtual int GetHeightFromParent()
        {
            int result = 0;
            if (this.Parent != null && this != this.Document.RenderRoot)
            {
                BaseElement parent = (BaseElement)this.Parent;
                while (true)
                {
                    if (parent == null)
                    {
                        result = this.Document.ClientSize.Height;
                        break;
                    }

                    if (parent.IsBlock && parent.IsStyleHeight && parent.Height > 0)
                    {
                        result = parent.Height;
                        break;
                    }

                    parent = (BaseElement)parent.Parent;
                }
            }
            else
            {
                result = this.Height;
            }

            return Math.Max(result, this.MinHeight);
        }

        /// <summary>
        /// Returns the inner width of the first nearest parent which has a block structure (IsBlock == True).
        /// </summary>
        /// <returns>Width according to parent width.</returns>
        protected virtual int GetInnerWidthFromParent()
        {
            int result = 0;
            if (this.Parent != null && this != this.Document.RenderRoot)
            {
                BaseElement parent = (BaseElement)this.Parent;
                while (true)
                {
                    if (parent == null)
                    {
                        result = this.Document.ClientSize.Width;
                        break;
                    }

                    if (parent.IsBlock && parent.Width > 0 && !parent.SkipWidth)
                    {
                        result = parent.ReduceWidth(parent.Width);
                        break;
                    }

                    parent = (BaseElement)parent.Parent;
                }
            }
            else
            {
                result = this.Document.ClientSize.Width; ////this.Width;
            }

            return Math.Max(result, this.MinWidth);
        }

        /// <summary>
        /// Returns the height of the first nearest parent which has a block structure (IsBlock == True).
        /// </summary>
        /// <returns>Height of the element according to height of parent.</returns>
        protected virtual int GetInnerHeightFromParent()
        {
            int result = 0;
            if (this.Parent != null && this != this.Document.RenderRoot)
            {
                BaseElement parent = (BaseElement)this.Parent;
                while (true)
                {
                    if (parent == null)
                    {
                        result = this.Document.ClientSize.Height;
                        break;
                    }

                    if (parent.IsBlock && parent.IsStyleHeight && parent.Height > 0)
                    {
                        result = parent.ReduceHeight(parent.Height);
                        break;
                    }

                    parent = (BaseElement)parent.Parent;
                }
            }
            else
            {
                result = this.Height;
            }

            return Math.Max(result, this.MinHeight);
        }

        /// <summary>
        /// Makes indent above the element.
        /// </summary>
        /// <param name="height">Height of block height.</param>
        protected internal virtual void MakeTopIndent(int height)
        {
            if (height < 0)
                throw new ArgumentOutOfRangeException("height", "Height must be greater than 0.");

            this.CurrentBlock.Height = height;
            this.CurrentBlock.Width = 0;
            this.CurrentBlock.DrawProperties = false;
            CreateBlock(this.CurrentBlock);
        }

        /// <summary>
        /// Makes indent below the element.
        /// </summary>
        /// <param name="height">Height value.</param>
        protected internal virtual void MakeBottomIndent(int height)
        {
            if (height < 0)
                throw new ArgumentOutOfRangeException("height", "Height must be greater than 0.");

            CreateBlock(this.CurrentBlock);
            this.CurrentBlock.Height = height;
            this.CurrentBlock.Width = 0;
            this.CurrentBlock.DrawProperties = false;
        }

        /// <summary>
        /// If element has attributes, attaches it to the formats.
        /// </summary>
        /// <returns>HTMLUIFormat object</returns>
        protected virtual HTMLFormat GetFormatFromAttributes()
        {
            HTMLFormat format = GetInheritedFormat();
            format.Merge = MergeMask.None;

            // 1. Attaches borders from attribute.
            if (this.Attributes.Contains(AttributeName.Border))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.Border];
                if (attr != null)
                {
                    format.Merge |= MergeMask.BorderWidth;
                    if (format.Left.Width == 0)
                    {
                        this.Control.FormatManager.SetBorderWidth(format, attr.Value, "left");
                        format.Merge |= MergeMask.BorderLeft;
                    }
                    if (format.Top.Width == 0)
                    {
                        this.Control.FormatManager.SetBorderWidth(format, attr.Value, "top");
                        format.Merge |= MergeMask.BorderTop;
                    }
                    if (format.Right.Width == 0)
                    {
                        this.Control.FormatManager.SetBorderWidth(format, attr.Value, "right");
                        format.Merge |= MergeMask.BorderRight;
                    }
                    if (format.Bottom.Width == 0)
                    {
                        this.Control.FormatManager.SetBorderWidth(format, attr.Value, "bottom");
                        format.Merge |= MergeMask.BorderBottom;
                    }
                }
            }

            // 2. Attaches size from attribute's width and height of the element.
            if (IsAttributeWidth)
            {
                SizeTypeEx type;
                int width = GetWidthValue(out type);

                if (width != -1)
                {
                    //                     To reduce the border width of the table(SD2832)
                    format.Width = width - format.Left.Width - format.Right.Width;
                    format.WidthType = type;
                    format.Merge |= MergeMask.Width;
                }
            }

            if (IsAttributeHeight)
            {
                SizeTypeEx type;
                int height = GetHeightValue(out type);
                if (height != -1)
                {
                    //                       To reduce the border height of the table(SD2832)
                    format.Height = height - format.Top.Width - format.Bottom.Width;
                    format.HeightType = type;
                    format.Merge |= MergeMask.Height;
                }
            }

            // 3. Attaches background color from attributes.
            if (this.Attributes.Contains(AttributeName.BgColor))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.BgColor];
                if (attr != null)
                {
                    this.Control.FormatManager.SetBackgroundColor(format, attr.Value);
                    format.Merge |= MergeMask.BgColor;
                }
            }

            // 4. Attaches color from attributes.
            if (this.Attributes.Contains(AttributeName.Color))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.Color];
                if (attr != null)
                {
                    this.Control.FormatManager.SetForeColor(format, attr.Value);
                    format.Merge |= MergeMask.ForeColor;
                }
            }

            
            // 5. Attaches horizontal align from attributes.
            if (this.Attributes.Contains(AttributeName.Align))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.Align];
                if (attr != null)
                {
                    this.Control.FormatManager.SetHorizontalAlign(format, attr.Value);
                    format.Merge |= MergeMask.HAlignment;
                }
            }

            // 6. Attaches vertical align from attributes.
            if (this.Attributes.Contains(AttributeName.VAlign))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.VAlign];
                if (attr != null)
                {
                    this.Control.FormatManager.SetVerticalAlign(format, attr.Value);
                    format.Merge |= MergeMask.VAlignmnet;
                }
            }

            // 7. Attaches background image from attributes.
            if (this.Attributes.Contains(AttributeName.Background))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.Background];
                if (attr != null)
                {
                    this.Control.FormatManager.SetBackgroundImage(format, attr.Value);
                    format.Merge |= MergeMask.BgImage;
                }
            }

            // 8. Attaches bordercolor from attribute.
            if (this.Attributes.Contains(AttributeName.BorderColor))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.BorderColor];
                if (attr != null)
                {
                    this.Control.FormatManager.SetBorderColor(format, attr.Value, "all");
                    format.Merge |= MergeMask.BorderColor;
                    format.Merge |= MergeMask.BorderAll;
                }
            }

            return format;
        }

        /// <summary>
        /// Adds / removes element from TabStop collection depending on
        /// TabStop value assigned.
        /// </summary>
        /// <param name="tabIndex">TabIndex of element.</param>
        protected virtual void SyncTabStopCollection(int tabIndex)
        {
            if (tabIndex < 0)
            {
                this.Document.FocusManager.Remove(this);
            }
            else
            {
                this.Document.FocusManager.Add(this);
            }
        }

        /// <summary>
        /// Sets the format to the element.
        /// </summary>
        /// <param name="format">HTMLUIFormat instance</param>
        protected void SetFormat(HTMLFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            bool quietMode = this.QuietMode;
            this.QuietMode = true;
            this.Format = format;
            this.QuietMode = quietMode;
        }

        /// <summary>
        /// Searches the block parent element for the current element.
        /// </summary>
        /// <returns>Block parent for element if it exists; Null otherwise.</returns>
        protected internal BaseElement GetBlockParent()
        {
            BaseElement blockParent = null;
            BaseElement curElement = this;
            BaseElement curParent = null;

            if (curElement.Parent != null && curElement != this.Document.RenderRoot)
            {
                while (true)
                {
                    curParent = curElement.Parent as BaseElement;

                    if (curParent == null) break;

                    if (curParent.IsBlock)
                    {
                        blockParent = curParent;
                        break;
                    }

                    curElement = curParent;
                }
            }

            return blockParent;
        }

        /// <summary>
        /// Returns cloned format with inherited options.
        /// </summary>
        /// <returns>Cloned format with inherited options.</returns>
        protected HTMLFormat GetInheritedFormat()
        {
            HTMLFormat defaultFormat = null;

            if (this.Parent == null || Equals(this.Document.RenderRoot))
            {
                defaultFormat = ((HTMLFormat)Control.DefaultFormat).GetInheritedFormat();
            }
            else
            {
                defaultFormat = ((HTMLFormat)this.Parent.Format).GetInheritedFormat();

                // Just hot fix for the table.
                if (this.Parent.Name == TagName.Table)
                {
                    defaultFormat.Merge &= ~MergeMask.HAlignment;
                    defaultFormat.HorizontalAlign = StringAlignment.Near;
                }
            }

            return defaultFormat;
        }
        #endregion
        #region Size Methods

        /// <summary>
        /// Calculates the size of the element for rendering.
        /// </summary>
        public void CalculateSize()
        {
            this.Size = Size.Empty;
            this.MinWidth = 0;
            this.MinHeight = 0;
            m_textMinWidth = 0;

            if (!this.IsVisible || (this.Parent != null && !this.Parent.IsVisible)) return;

            CalculateSizeInternal();

            // Raise event which indicates the size of the element calculated.
            OnSizeCalculated(EventArgs.Empty);
        }

        /// <summary>
        /// Calculates the size of the element without border and spacing values.
        /// </summary>
        /// <returns>Size of the element.</returns>
        protected virtual Size DefGetSize()
        {
            Size textSize = this.InFillTextSizeHash();

            // Set size corresponding to attribute's size.
            textSize = GetFinalSizeInternal(textSize);

            if (IsStyleWidth && GetWidthType() == SizeTypeEx.Number)
            {
                this.Width = textSize.Width;
            }

            ArrayList widthSizes = new ArrayList();
            ArrayList heightSizes = new ArrayList();

            widthSizes.Add(textSize.Width);
            heightSizes.Add(textSize.Height);

            if (this.HasChildren)
            {
                for (int index = 0; index < Children.Count; index++)
                {
                    BaseElement child = Children[index] as BaseElement;
                    child.CalculateSize();

                    widthSizes.Add(child.Size.Width);
                    heightSizes.Add(child.Size.Height);

                    this.MinWidth = Math.Max(this.MinWidth, child.MinWidth);
                    this.MinHeight = Math.Max(this.MinHeight, child.MinHeight);
                }
            }

            widthSizes.Sort();
            heightSizes.Sort();

            textSize.Width = (int)widthSizes[widthSizes.Count - 1];
            textSize.Height = (int)heightSizes[heightSizes.Count - 1];

            if (IsStyleHeight && GetHeightType() == SizeTypeEx.Number)
            {
                this.Height = Math.Max(textSize.Height, this.Format.Height);
            }

            textSize.Width = Math.Max(textSize.Width, this.MinWidth);
            return textSize;
        }

        /// <summary>
        /// Calculates the size of the element without border and spacing values.
        /// Element is inside table.
        /// </summary>
        /// <returns>Size of element in the table.</returns>
        protected virtual Size DefGetSizeInTable()
        {
            // Set size corresponding to attribute's size.
            Size textSize = GetFinalSizeInternal(Size.Empty);

            if (IsStyleWidth && GetWidthType() == SizeTypeEx.Number)
            {
                this.Width = textSize.Width;
            }
            ArrayList widthSizes = new ArrayList();
            ArrayList heightSizes = new ArrayList();

            widthSizes.Add(textSize.Width);
            heightSizes.Add(textSize.Height);

            int index = 0;
            bool bSumSize = true;
            int width = 0;
            int height = 0;
            int sumWidth = 0;
            int sumHeight = 0;

            ////int maxLength = ReduceWidth( GetMaxWidth() );
            int maxLength = ReduceWidth(GetMaxWidthForTextSizing());
            XmlNodeList nodeList = this.Storage.ChildNodes;

            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode node = nodeList[i] as XmlNode;

                if (node is XmlText || node is XmlWhitespace)
                {
                    textSize = InFillTextSizeHash(node, maxLength);
                    bSumSize = true;
                    width = textSize.Width;
                    height = textSize.Height;
                }
                else if (node is XmlElement && index < this.Children.Count)
                {
                    BaseElement child = this.Children[index] as BaseElement;
                    child.InsideTable = true;
                    child.CalculateSize();
                    width = child.Width;
                    height = child.Height;
                    this.MinWidth = Math.Max(this.MinWidth, child.MinWidth);
                    this.MinHeight = Math.Max(this.MinHeight, child.MinHeight);
                    index++;

                    if ((child.Type & ElementType.NewLine) > 0)
                    {
                        bSumSize = false;
                        if (sumWidth != 0 || sumHeight != 0)
                        {
                            widthSizes.Add(sumWidth);
                            heightSizes.Add(sumHeight);
                        }

                        sumWidth = width;
                        sumHeight = height;
                    }
                    else
                    {
                        bSumSize = true;
                    }
                }

                if (bSumSize)
                {
                    sumWidth += width;
                    sumHeight = Math.Max(sumHeight, height);
                }
                else
                {
                    widthSizes.Add(sumWidth);
                    heightSizes.Add(sumHeight);

                    sumWidth = 0;
                    sumHeight = 0;
                }
            }

            width = 0;
            height = 0;

            if (sumWidth != 0 || sumHeight != 0)
            {
                widthSizes.Add(sumWidth);
                heightSizes.Add(sumHeight);
            }

            widthSizes.Sort();
            heightSizes.Sort();

            textSize.Width = (int)widthSizes[widthSizes.Count - 1];
            textSize.Height = (int)heightSizes[heightSizes.Count - 1];

            if (IsStyleHeight && GetHeightType() == SizeTypeEx.Number)
            {
                this.Height = Math.Max(textSize.Height, this.Format.Height);
            }

            textSize.Width = Math.Max(textSize.Width, this.MinWidth);

            return textSize;
        }

        /// <summary>
        /// Calculates the size of the element with borders, spaces, etc.
        /// </summary>
        /// <returns>Size of element with borders, etc.</returns>
        protected Size DefGetSizeWithAttributes()
        {
            Size result = Size.Empty;
            if (!this.InsideTable)
            {
                result = DefGetSize();
            }
            else
            {
                result = DefGetSizeInTable();
            }

            bool expandedHeight = false;

            ////if( this.IsBlock )
            {
                if (!IsStyleWidth || GetWidthType() != SizeTypeEx.Number)
                {
                    result.Width = ExpandWidth(result.Width);
                }

                if (!IsStyleHeight || GetHeightType() != SizeTypeEx.Number)
                {
                    result.Height = ExpandHeight(result.Height);
                    expandedHeight = true;
                }
            }

            this.MinWidth = ExpandWidth(this.MinWidth);
            this.MinHeight = Math.Max(this.MinHeight, result.Height);

            if (!expandedHeight) this.MinHeight = ExpandHeight(this.MinHeight);

            result.Width = Math.Max(result.Width, this.MinWidth);
            result.Height = Math.Max(result.Height, this.MinHeight);

            SetElementStyle();

            return result;
        }

        /// <summary>
        /// Calculates the max size of the element by default (with borders, paddings, etc.).
        /// </summary>
        /// <returns>Default max size.</returns>
        protected Size DefaultCalculateSizeInternal()
        {
            return DefGetSizeWithAttributes();
        }

        /// <summary>
        /// Overloaded. Infills hash with each text block of the element as key
        /// and the size of the block as value.
        /// </summary>
        /// <returns>Size of the max text in the element.</returns>
        protected virtual Size InFillTextSizeHash()
        {
            XmlNodeList nodeList = this.Storage.ChildNodes;
            int maxWidth = 0;
            int maxHeight = 0;
            Size textSize = Size.Empty;

            ////int maxRegionWidth = ReduceWidth( GetMaxWidth() );
            int maxRegionWidth = ReduceWidth(GetMaxWidthForTextSizing());

            XmlNode node = null;
            for (int i = 0, len = nodeList.Count; i < len; i++)
            {
                node = nodeList[i];
                if (node is XmlText || node is XmlWhitespace)
                {
                    textSize = MeasureString(node.Value, (HTMLFormat)this.Format, maxRegionWidth);

                    m_textSizeHash[node] = textSize;
                    maxWidth = Math.Max(maxWidth, (int)textSize.Width);
                    maxHeight = Math.Max(maxHeight, (int)textSize.Height);
                }
            }

            //// Size result = new Size( ExpandWidth( maxWidth ), ExpandHeight( maxHeight ) );
            Size result = new Size(maxWidth, maxHeight);

            return result;
        }

        /// <summary>
        /// Infills hash with each text block of the element as key
        /// and the size of the block as value. Maximum length is defined.
        /// </summary>
        /// <param name="text">XmlNode instance</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Size of the max text in the element.</returns>
        protected virtual Size InFillTextSizeHash(XmlNode text, int maxLength)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (!((text is XmlText) || (text is XmlWhitespace)))
                throw new ArgumentException("Text has bad type.");

            Size textSize = Size.Empty;

            textSize = MeasureString(text.Value, (HTMLFormat)this.Format, maxLength);

            //// textSize.Width = ExpandWidth( textSize.Width );
            //// textSize.Height = ExpandHeight( textSize.Height );

            return textSize;
        }

        /// <summary>
        /// Returns the value of the width attribute of the element.
        /// </summary>
        /// <param name="widthType">Width value</param>
        /// <returns>Width of the element if successful; -1 otherwise.</returns>
        internal int GetWidthValue(out SizeTypeEx widthType)
        {
            if (!IsAttributeWidth)
            {
                widthType = SizeTypeEx.Unknown;
                return -1;
            }
            XmlAttribute attr = this.Storage.Attributes[AttributeName.Width];
            AttributeToken type = AttributeParser.DetectType(attr.Value);

            if (type == AttributeToken.Percent)
            {
                widthType = SizeTypeEx.Percent;
                return AttributeParser.GetInteger(attr.Value);
            }
            else if (type == AttributeToken.Integer)
            {
                widthType = SizeTypeEx.Number;
                return AttributeParser.GetInteger(attr.Value);
            }

            widthType = SizeTypeEx.Unknown;
            return -1;
        }

        /// <summary>
        /// Returns the value of the height attribute of the element.
        /// </summary>
        /// <param name="heightType">Height value</param>
        /// <returns>Height of the element if successful; -1 otherwise.</returns>
        internal int GetHeightValue(out SizeTypeEx heightType)
        {
            if (!IsAttributeHeight)
            {
                heightType = SizeTypeEx.Unknown;
                return -1;
            }

            XmlAttribute attr = this.Storage.Attributes[AttributeName.Height];
            AttributeToken type = AttributeParser.DetectType(attr.Value);

            if (type == AttributeToken.Percent)
            {
                heightType = SizeTypeEx.Percent;
                return AttributeParser.GetInteger(attr.Value);
            }
            else if (type == AttributeToken.Integer)
            {
                heightType = SizeTypeEx.Number;
                return AttributeParser.GetInteger(attr.Value);
            }

            heightType = SizeTypeEx.Unknown;
            return -1;
        }

        /// <summary>
        /// Compares max size of the child elements with own attributes
        /// in style, attribute width and height parameters with width and height attributes
        /// of the element.
        /// </summary>
        /// <param name="testSize">Size for checking and comparing.</param>
        /// <returns>Final size after comparing.</returns>
        protected Size GetFinalSizeInternal(Size testSize)
        {
            // Element has attributes in style attribute.
            if (IsStyleWidth && GetWidthType() == SizeTypeEx.Number)
            {
                testSize.Width = ReduceWidth(this.Format.Width);

                // Set minimum width of the element.
                this.MinWidth = Math.Max(this.MinWidth, testSize.Width);
            }

            if (IsStyleHeight && GetHeightType() == SizeTypeEx.Number)
            {
                int attrHeight = ReduceHeight(this.Format.Height);
                testSize.Height = Math.Max(attrHeight, testSize.Height);

                // Set minimum height of the element.
                this.MinHeight = Math.Max(this.MinHeight, testSize.Height);
            }

            return testSize;
        }

        /// <summary>
        /// Sets the element's style to FixedSize if width or height attributes have been defined.
        /// </summary>
        protected void SetElementStyle()
        {
            HTMLFormat format = (HTMLFormat)this.Format;

            if (!format.IsWidthDefault && format.WidthType == SizeTypeEx.Number)
            {
                if ((this.Type & ElementType.BlockNewLineIndent) > 0)
                {
                    this.Type = ElementType.BlockNewLineFixedSizeIndent;
                }
                else if ((this.Type & ElementType.BlockNewLine) > 0)
                {
                    this.Type = ElementType.BlockNewLineFixedSize;
                }
                else
                {
                    this.Type = ElementType.BlockFixedSize;
                }
            }
            else if (!format.IsWidthDefault && format.WidthType == SizeTypeEx.Percent)
            {
                if ((this.Type & ElementType.BlockNewLineIndent) > 0)
                {
                    this.Type = ElementType.BlockNewLineResizableIndent;
                }
                else if ((this.Type & ElementType.BlockNewLine) > 0)
                {
                    this.Type = ElementType.BlockNewLineResizable;
                }
                else
                {
                    this.Type = ElementType.BlockResizable;
                }
            }
        }

        /// <summary>
        /// Returns the width of the area in which an element can be drawn.
        /// </summary>
        /// <returns>Width of the area in which an element can be drawn.</returns>
        protected int GetMaxWidth()
        {
            int maxRegionWidth = GetWidthFromParent();

            if (this.Parent != null && this.Parent.Size.Width != 0)
            {
                maxRegionWidth = this.Parent.Size.Width;
            }

            // If we have to attribute width in style.
            if (IsStyleWidth && GetWidthType() == SizeTypeEx.Number)
            {
                maxRegionWidth = this.Format.Width;
            }

            return maxRegionWidth;
        }

        /// <summary>
        /// Returns the type of the width CSS attribute.
        /// </summary>
        /// <returns>Type of the width CSS attribute.</returns>
        internal SizeTypeEx GetWidthType()
        {
            HTMLFormat format = (HTMLFormat)this.Format;

            return format.WidthType;
        }

        /// <summary>
        /// Returns the type of the height CSS attribute.
        /// </summary>
        /// <returns>Type of the height CSS attribute.</returns>
        internal SizeTypeEx GetHeightType()
        {
            HTMLFormat format = (HTMLFormat)this.Format;

            return format.HeightType;
        }

        /// <summary>
        /// Returns the width of the element if its size depends on parent's size.
        /// </summary>
        /// <returns>Width of the element if its size depends on parent's size.</returns>
        internal int GetResizableWidth()
        {
            int result = GetWidthFromParent();

            HTMLFormat format = (HTMLFormat)this.Format;
            if (IsStyleWidth && format.WidthType == SizeTypeEx.Percent)
            {
                result = result * format.Width / 100;
            }

            return Math.Max(this.MinWidth, result);
        }

        /// <summary>
        /// Returns the height of the element if its size depends on parent's size.
        /// </summary>
        /// <returns>Height of the element if its size depends on parent's size.</returns>
        internal int GetResizableHeight()
        {
            int result = GetHeightFromParent();

            HTMLFormat format = (HTMLFormat)this.Format;
            if (IsStyleHeight && format.HeightType == SizeTypeEx.Percent)
            {
                result = result * format.Height / 100;
                return result;
            }

            return Math.Max(this.MinHeight, this.Height);
        }

        /// <summary>
        /// Returns the inner width of the element if its size depends on parent's size.
        /// </summary>
        /// <returns>Inner width of the element if its size depends on parent's size.</returns>
        internal int GetResizableInnerWidth()
        {
            int result = GetInnerWidthFromParent();

            HTMLFormat format = (HTMLFormat)this.Format;
            if (IsStyleWidth && format.WidthType == SizeTypeEx.Percent)
            {
                result = result * format.Width / 100;
            }

            return Math.Max(this.MinWidth, result);
        }

        /// <summary>
        /// Returns the inner height of the element if its size depends on parent's size.
        /// </summary>
        /// <returns>Inner height of the element if its size depends on parent's size.</returns>
        internal int GetResizableInnerHeight()
        {
            int result = GetInnerHeightFromParent();

            HTMLFormat format = (HTMLFormat)this.Format;
            if (IsStyleHeight && format.HeightType == SizeTypeEx.Percent)
            {
                result = result * format.Height / 100;
                return result;
            }

            return Math.Max(this.MinHeight, this.Height);
        }

        /// <summary>
        /// Returns the height of the block element.
        /// </summary>
        /// <returns>Height of the block element.</returns>
        internal int GetBlockElementHeight()
        {
            HTMLFormat format = (HTMLFormat)this.Format;

            SizeTypeEx type;
            int value = GetHeightValue(out type);

            if (type == SizeTypeEx.Percent || format.WidthType == SizeTypeEx.Percent)
            {
                return GetResizableHeight();
            }

            return this.Height;
        }

        /// <summary>
        /// Returns the maximum width for the element when the size of text is being calculated
        /// inside the element. Used during sizing only.
        /// </summary>
        /// <returns>Maximum width for the element when the size of text is being calculated
        /// inside the element. Used during sizing only.</returns>
        protected int GetMaxWidthForTextSizing()
        {
            int maxWidth = 0;

            if (NoWrapEnabled())
            {
                maxWidth = Int32.MaxValue;
            }
            else
            {
                maxWidth = GetMaxWidth();
            }

            return maxWidth;
        }
        #endregion

        #region Position Methods

        /// <summary>
        /// Calculates the positions of each child of the element.
        /// </summary>
        /// <param name="curPosition">Position of the cursor</param>
        /// <param name="bounds">Rectangle bounds</param>
        /// <returns>BlockCollection instance</returns>
        protected virtual BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            PreparePositioning(curPosition, bounds);

            // If element is invisible, skip position calculating method.
            if (!this.IsVisible) return new BlocksCollection();

            int elementCount = 0;
            string textValue = string.Empty;

            // Throw all elements inside current element and calculate
            // their position.
            XmlNode node = null;
            XmlNodeList nodeList = this.Storage.ChildNodes;

            for (int i = 0, len = nodeList.Count; i < len; i++)
            {
                node = nodeList[i];
                bool lastWhiteSpaceInBlock = (i == len - 1) && IsBlock && (node is XmlWhitespace);

                if (IsValidText(node) && !lastWhiteSpaceInBlock)
                {
                    //// simple text in the element
                    //// Get all text to some element even it is contained in several nodes.
                    if ((i < len - 1) && IsValidText(nodeList[i + 1]))
                    {
                        textValue += node.Value;
                    }
                    else
                    {
                        textValue += node.Value;
                        Text text = new Text(textValue);
                        textValue = string.Empty;

                        CalculateTextPos(text, m_curBlock);
                    }
                }
                else if (node is XmlElement)
                {
                    
                    //// Tag element in the element
                    CalculateElementPos(elementCount, m_curBlock);
                    elementCount++;
                }
            }

            ResizeElement();
            SetLastCurrentPosition();

            //// NOTE: Raise event location changed.
            //// If element overrides this method, it must raise event itself.

            if (!this.IsBlock)
            {
                OnLocationCalculated(EventArgs.Empty);
            }

            return m_blocks;
        }

        /// <summary>
        /// Calculates the position of the text element.
        /// </summary>
        /// <param name="node">Text element node.</param>
        /// <param name="block">Current block.</param>
        protected virtual void CalculateTextPos(Text node, Block block)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (block == null)
                throw new ArgumentNullException("block");

            if (!NeedCalculate(node)) return;

            Size initSize = MeasureString(node.Value, this.Format.Font);

            // Check if text is inside bounds of element or not.
            if (IsGoodWidth(block, m_curPos.X, initSize.Width) || NoWrapEnabled())
            {
                InsertText(node, m_curBlock, initSize);
            }
            else
            {
                BreakText(node, block);
            }
        }

        /// <summary>
        /// Calculates the position of the HTML element.
        /// </summary>
        /// <param name="count">Index of the child element.</param>
        /// <param name="block">Current block.</param>
        protected void CalculateElementPos(int count, Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            BaseElement element = (BaseElement)this.Children[count];

            CalculateChildPosFromType(element);
        }

        /// <summary>
        /// Detects the type of the child element and invokes corresponding method to calculate position.
        /// </summary>
        /// <param name="child">Child element.</param>
        protected void CalculateChildPosFromType(BaseElement child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

            if (!child.IsBlock)
            {
                //// Element is inline.
                CalculateInlineChildPos(child);
            }
            else
            {
                //// Element has block structure.
                CalculateBlockChildPos(child);
            }
        }

        /// <summary>
        /// Calculates the position for the inline child element.
        /// </summary>
        /// <param name="element">Child element.</param>
        protected void CalculateInlineChildPos(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            Size initSize = element.Size;
            Rectangle elementRect = this.Bounds;

            if (!this.IsBlock)
            {
                elementRect = ReduceBounds(elementRect);
            }

            BlocksCollection childBlocks = element.CalculateChildPositions(m_curPos, elementRect);

            Block childBlock = null;
            object item = null;

            for (int i = 0, len = childBlocks.Count; i < len; i++)
            {
                item = childBlocks[i];
                if (item is Block == false) continue;

                childBlock = item as Block;

                childBlock.ResetRectCache();
                Rectangle childRect = childBlock.Rectangle;

                //// Check where child block is.
                //// Child block will be inserted in current block.
                if (ElementInSameBlock(childRect))
                {
                    AddElement(m_curBlock, childBlock, childRect);
                }
                else
                {
                    //// We must create new block and insert child block in it.
                    if (!this.IsBlock)
                    {
                        m_curBlock.FinalWidth += this.IndentSpace.Left + this.IndentSpace.Right;
                        m_curBlock.FinalHeight += this.IndentSpace.Top + this.IndentSpace.Bottom;

                        m_curBlock.X -= this.IndentSpace.Left;
                        m_curBlock.Y -= this.IndentSpace.Top;
                    }

                    Block newBlock = CreateBlock(childBlock);
                    AddElement(newBlock, childBlock, childRect);
                }
            }

            if (element.IsVisible) this.CurrentPosition = element.CurrentPosition;
        }

        /// <summary>
        /// Detects the type of the block element and invokes the corresponding method.
        /// </summary>
        /// <param name="element">Child element.</param>
        protected void CalculateBlockChildPos(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (!element.IsBlock) CalculateInlineChildPos(element);

            switch (element.Type)
            {
                case ElementType.BlockFixedSize:
                    CalculateBlockFixedSizePos(element);
                    break;

                case ElementType.BlockNewLineFixedSizeIndent:
                case ElementType.BlockNewLineFixedSize:
                    CalculateBlockNewLineFixedSizePos(element);
                    break;

                case ElementType.BlockNewLineResizableIndent:
                case ElementType.BlockNewLineResizable:
                    CalculateBlockNewLineResizablePos(element);
                    break;

                case ElementType.BlockSimple:
                    CalculateBlockSimplePos(element);
                    break;

                case ElementType.BlockNewLineSimple:
                    CalculateBlockNewLineSimplePos(element);
                    break;

                case ElementType.BlockResizable:
                    CalculateBlockResizablePos(element);
                    break;

                default:
                    throw new ArgumentException("Element has unknown structure type.");
            }

            element.SetAttributes();
            element.OnLocationCalculated(EventArgs.Empty);
        }

        /// <summary>
        /// Calculates the position for the child element with type "BlockFixedSize".
        /// </summary>
        /// <param name="element">Child element.</param>
        protected void CalculateBlockFixedSizePos(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (!IsGoodWidth(m_curBlock, m_curPos.X, element.Width))
            {
                CreateBlock(m_curBlock);
            }

            element.MoveStartCurPos(ref m_curPos);
            element.CurrentPosition = this.CurrentPosition;
            //// Main child block:
            Block mainChildBlock = new Block(element);
            mainChildBlock.IsMain = true;
            mainChildBlock.Width = element.Width;
            mainChildBlock.Height = element.GetBlockElementHeight();
            element.MainBlock = mainChildBlock;

            BlocksCollection blocks = element.CalculateChildPositions(this.CurrentPosition, element.ReduceBounds(mainChildBlock.Rectangle));

            AddBlocksToMain(element, mainChildBlock, blocks);

            AddElement(m_curBlock, mainChildBlock, mainChildBlock.Rectangle);

            element.MoveFinalCurPos(ref m_curPos);
        }

        /// <summary>
        /// Calculates the position for the child element with type "BlockNewLineFixedSize".
        /// </summary>
        /// <param name="element">Child element.</param>
        protected void CalculateBlockNewLineFixedSizePos(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            CreateBlock(m_curBlock);

            CalculateBlockFixedSizePos(element);
            CreateBlock(m_curBlock);
        }

        /// <summary>
        /// Calculates the position for the child element with type "BlockSimplePos".
        /// </summary>
        /// <param name="element">Child element.</param>
        protected void CalculateBlockSimplePos(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.MoveStartCurPos(ref m_curPos);

            element.CurrentPosition = this.CurrentPosition;
            Block mainChildBlock = new Block(element);

            mainChildBlock.IsMain = true;
            element.MainBlock = mainChildBlock;

            int oldXPosition = this.CurrentPosition.X;
            Rectangle bounds = new Rectangle(this.CurrentPosition, element.Size);

            BlocksCollection blocks = element.CalculateChildPositions(this.CurrentPosition, element.ReduceBounds(bounds));

            AddBlocksToMain(element, mainChildBlock, blocks);
            AddElement(m_curBlock, mainChildBlock, mainChildBlock.Rectangle);
            element.MoveFinalCurPos(ref m_curPos);
            m_curPos.X = oldXPosition + element.Width;
        }

        /// <summary>
        /// Calculates the position for the child element with type "BlockNewLineSimple".
        /// </summary>
        /// <param name="element">Child element.</param>
        protected void CalculateBlockNewLineSimplePos(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            CreateBlock(m_curBlock);
            CalculateBlockSimplePos(element);
            CreateBlock(m_curBlock);
        }

        /// <summary>
        /// Calculates the position for the child element with type "BlockNewLineResizable".
        /// </summary>
        /// <param name="element">Chile element.</param>
        protected void CalculateBlockNewLineResizablePos(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            CreateBlock(m_curBlock);

            CalculateBlockResizablePos(element);
            CreateBlock(m_curBlock);
        }

        /// <summary>
        /// Calculates the position for the child element with type "BlockResizable".
        /// </summary>
        /// <param name="element">Child element.</param>
        protected void CalculateBlockResizablePos(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.MoveStartCurPos(ref m_curPos);

            element.CurrentPosition = this.CurrentPosition;
            //// Main child block:
            Block mainChildBlock = new Block(element);
            mainChildBlock.IsMain = true;

            //// NOTE: this line of code was changed several times.
            //// One of other cases was: this.Bounds.Width, but for elements,
            //// which have not nowrap attribute.
            mainChildBlock.Width = element.GetResizableInnerWidth();

            element.Width = mainChildBlock.FinalWidth;
            mainChildBlock.Height = element.GetResizableInnerHeight();
            element.Height = mainChildBlock.FinalHeight;
            element.MainBlock = mainChildBlock;
            Rectangle bounds = element.ReduceBounds(mainChildBlock.Rectangle);

            BlocksCollection blocks = element.CalculateChildPositions(this.CurrentPosition, bounds);

            AddBlocksToMain(element, mainChildBlock, blocks);

            mainChildBlock.ResetRectCache();

            AddElement(m_curBlock, mainChildBlock, mainChildBlock.Rectangle);
            element.MoveFinalCurPos(ref m_curPos);
        }

        /// <summary>
        /// Adds each block from an array of blocks to the main block.
        /// </summary>
        /// <param name="element">Current element object.</param>
        /// <param name="mainBlock">Main block container.</param>
        /// <param name="blocks">Blocks for inserting into the main block.</param>
        protected void AddBlocksToMain(BaseElement element, Block mainBlock, BlocksCollection blocks)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (mainBlock == null)
                throw new ArgumentNullException("mainBlock");
            if (blocks == null)
                throw new ArgumentNullException("blocks");

            //// object item = null;
            Block child = null;
            for (int i = 0, len = blocks.Count; i < len; i++)
            {
                child = blocks[i];

                child.ResetRectCache();
                mainBlock.Add(child, child.Rectangle);
            }

            mainBlock.QuietMode = true;

            if (element.IsVisible)
            {
                mainBlock.X -= element.IndentSpace.Left;
                mainBlock.Y -= element.IndentSpace.Top;

                element.Width = Math.Max(element.Width, mainBlock.FinalWidth);
                element.Height = Math.Max(element.Height, mainBlock.FinalHeight);

                mainBlock.FinalWidth = element.Width;
                mainBlock.FinalHeight = element.Height;
            }
            else
            {
                //// Element is invisible and set size to zero.
                element.Width = 0;
                element.Height = 0;
                mainBlock.Width = 0;
                mainBlock.Height = 0;
                mainBlock.FinalWidth = 0;
                mainBlock.FinalHeight = 0;
            }

            mainBlock.QuietMode = false;
        }

        /// <summary>
        /// Indicates whether the width of the child is larger than the element width.
        /// </summary>
        /// <param name="block">Block in which we want to insert the element.</param>
        /// <param name="curX">Current X coordinate.</param>
        /// <param name="elementWidth">Width of the element.</param>
        /// <returns>True if free space is suitable for the block.</returns>
        protected bool IsGoodWidth(Block block, int curX, int elementWidth)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            // Free width in the block.
            int freeWidth = GetFreeWidthInLine(block, curX);

            if (freeWidth < 0) return false;

            return freeWidth >= elementWidth;
        }

        /// <summary>
        /// Breaks the string text into two parts. First is in the same line, another into a new line.
        /// </summary>
        /// <param name="node">Text which must be broken and inserted in two lines.</param>
        /// <param name="block">First block (for first part of text).</param>
        protected void BreakText(Text node, Block block)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (block == null)
                throw new ArgumentNullException("block");

            // Break text in to two parts.
            ArrayList tokens;
            string[] parts = SplitText(node.Value, GetFreeWidthInLine(block, m_curPos.X), out tokens);

            if (parts.Length != 2)
                throw new ArgumentException("There are some invalid count of text parts.");

            // If whole text is in bounds.
            if (parts[1].Length == 0)
            {
                Size initSize = MeasureString(node.Value, this.Format.Font);
                InsertText(node, block, initSize);

                return;
            }
            else if (parts[0].Length == 0)
            {
                // If we must postpone whole text on new line.
                // If we already created the block and it is empty, it is not needed for us.
                if (m_blocks.Count == 1 && block.IsEmpty)
                {
                    m_blocks.Clear();
                }
            }
            else
            {
                // Part of the text is in the same line and part must be postponed on new line.
                // Calculate first part of the text element.
                Text first = (Text)node.Clone();
                first.Value = parts[0];
                Size initSize = MeasureString(first.Value, this.Format.Font);
                InsertText(first, block, initSize);
            }

            Block newBlock = CreateBlock(block);

            // Get an array of lines where the length is less than the width.
            int width = GetFreeWidthInLine(newBlock, m_curPos.X);
            ArrayList sizes;
            ArrayList lines = SplitTextToLines(tokens, width, out sizes);
            Size size;

            // Infill lines.
            for (int i = 0, len = lines.Count; i < len; i++)
            {
                newBlock = (i == 0) ? newBlock : newBlock = CreateBlock(newBlock);
                node = (Text)node.Clone();
                node.Value = (string)lines[i];
                size = (Size)sizes[i];

                InsertText(node, newBlock, size);
            }
        }

        /// <summary>
        /// Inserts the text node into the block element.
        /// </summary>
        /// <param name="node">Current text node.</param>
        /// <param name="block">Current block.</param>
        /// <param name="size">Size of the text.</param>
        protected void InsertText(Text node, Block block, Size size)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (block == null)
                throw new ArgumentNullException("block");

            if (!NeedCalculate(node)) return;

            // Add element to the block.
            Rectangle bound = new Rectangle(m_curPos, size);
            AddElement(block, node, bound);
            node.Parent = block;
        }

        /// <summary>
        /// Breaks the text into two parts by word and length (if all text can't be in bounds element).
        /// </summary>
        /// <param name="str">String text.</param>
        /// <param name="length">Length which remains in first line</param>
        /// <param name="tokens">Array of the remaining words.</param>
        /// <returns>Array of two parts of the string.</returns>
        protected string[] SplitText(string str, int length, out ArrayList tokens)
        {
            string token = string.Empty;
            string[] result = new string[2];
            result[0] = string.Empty;
            result[1] = str;
            tokens = InFillBlocks(str);
            int i = 0;
            HTMLFormat format = (HTMLFormat)this.Format;

            ArrayList removeTokens = new ArrayList();

            do
            {
                if (i == tokens.Count) break;

                token = (string)tokens[i];
                int tokenLength = MeasureString(result[0] + token, format.Font).Width;
                if (tokenLength > length) break;

                result[0] += token;
                result[1] = result[1].Remove(0, token.Length);
                i++;
            }
            while (true);

            // Remove the token words from first line.
            tokens.RemoveRange(0, i);

            return result;
        }

        /// <summary>
        /// Splits the string represented by an array into lines of text whose length is less
        /// than defined.
        /// </summary>
        /// <param name="tokens">Array of words.</param>
        /// <param name="length">Max width of the line.</param>
        /// <param name="sizes">Array of sizes of those lines.</param>
        /// <returns>Array of lines.</returns>
        protected ArrayList SplitTextToLines(ArrayList tokens, int length, out ArrayList sizes)
        {
            if (tokens == null)
                throw new ArgumentNullException("tokens");

            ArrayList result = new ArrayList();
            sizes = new ArrayList();

            if (tokens.Count == 0) return result;

            HTMLFormat format = (HTMLFormat)this.Format;
            string line = (string)tokens[0];
            string empty = string.Empty;
            string token = empty;
            Size curSize;
            Size lineSize = Size.Empty;
            int i = 0;

            do
            {
                curSize = MeasureString(line + token, format.Font);

                // Line is full.
                if (curSize.Width > length)
                {
                    // The first token is larger than the width.
                    if (token.Length == 0)
                    {
                        int oldLength = tokens.Count;
                        SplitToken(line, tokens, i, length);

                        if (oldLength < tokens.Count)
                        {
                            line = (string)tokens[i];
                            token = empty;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (line.Length > 0)
                    {
                        result.Add(line);
                        sizes.Add(lineSize);
                    }
                    line = token;
                    token = empty;
                }
                else
                {
                    lineSize = curSize;
                    line += token;
                    i++;

                    if (i == tokens.Count)
                    {
                        result.Add(line);
                        sizes.Add(lineSize);
                        break;
                    }

                    token = (string)tokens[i];
                }
            }
            while (true);

            return result;
        }

        /// <summary>
        /// Adds element to the block and moves if needed all previous elements in the block.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <param name="key">Element for adding to the block.</param>
        /// <param name="value">Rectangle which key reserves.</param>
        protected void AddElement(Block block, object key, Rectangle value)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (key == null)
                throw new ArgumentNullException("key");

            m_curPos.X += value.Width;

            // Add element to line.
            block.Add(key, value);
        }

        /// <summary>
        /// Returns the amount of free space in current line.
        /// Free space depends on current X pos and horizontal alignment.
        /// </summary>
        /// <param name="block">Block instance</param>
        /// <param name="curX">Current X coordinate on the line.</param>
        /// <returns>Width in the current line.</returns>
        protected int GetFreeWidthInLine(Block block, int curX)
        {
            return GetRemainderWidth(block, curX);
        }

        /// <summary>
        /// Returns the remainder which is in bounds in the current line when horizontal align is left.
        /// </summary>
        /// <param name="block">Current block.</param>
        /// <param name="curWidth">Current width of the block.</param>
        /// <returns>Remaining width in the block.</returns>
        protected int GetRemainderWidth(Block block, int curWidth)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            int freeInElement = this.Bounds.X + this.Bounds.Width - curWidth;
            int left = this.Bounds.X;

            // Reduce width of inline element and shift start position for block.
            if (!this.IsBlock)
            {
                freeInElement -= this.IndentSpace.Right;
                left += this.IndentSpace.Left;
            }

            if (curWidth == left)
            {
                return (int)Math.Max(freeInElement, ReduceWidth(this.MinWidth));
            }

            return freeInElement;
        }

        /// <summary>
        /// Calculates the width from the current position to the control depending on alignment.
        /// </summary>
        /// <param name="block">Current block.</param>
        /// <param name="curWidth">Current width of the block.</param>
        /// <returns>Width of current point to border of the block.</returns>
        protected int GetWidthToBorder(Block block, int curWidth)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            return GetWidthFromParent() - block.Width;
        }

        /// <summary>
        /// Creates a new block, resizes the element and changes the start position of the new block.
        /// </summary>
        /// <param name="oldBlock">Current active block.</param>
        /// <returns>Block which is created.</returns>
        protected internal virtual Block CreateBlock(Block oldBlock)
        {
            // Create block and add it to storage of the blocks.
            if (oldBlock != null)
            {
                //// NOTE: Disabled for testing.
                //// ResizeElement();

                //// Fix block's size.
                oldBlock.FinalWidth = oldBlock.FinalWidth;
                oldBlock.FinalHeight = oldBlock.FinalHeight;

                //// random 12.04.2004
                if (!this.IsBlock && oldBlock.Owner == this)
                {
                    oldBlock.FinalWidth += this.IndentSpace.Left + this.IndentSpace.Right;
                    oldBlock.FinalHeight += this.IndentSpace.Top + this.IndentSpace.Bottom;

                    oldBlock.X -= this.IndentSpace.Left;
                    oldBlock.Y -= this.IndentSpace.Top;
                }

                // Change start position.
                if (oldBlock.Owner == this)
                {
                    ChangeStartPosition(oldBlock);
                }
                else
                {
                    ChangeStartPosition(oldBlock.Rectangle);
                }

                // Shift start position of inline element.
                if (!this.IsBlock)
                {
                    m_curPos.X += this.IndentSpace.Left;
                }
            }

            Block block = new Block(this);

            m_blocks.Add(block);
            m_curBlock = block;

            return m_curBlock;
        }

        /// <summary>
        /// Resizes the element if inside blocks are bigger than the element.
        /// </summary>
        protected void ResizeElement()
        {
            if ((this.Type & ElementType.BlockFixed) > 0 &&
              !NoWrapEnabled())
            {
                return;
            }

            int elmWidth = this.Width;
            int blocksHeight = 0;

            for (int i = 0; i < m_blocks.Count; i++)
            {
                Block b = m_blocks[i] as Block;
                elmWidth = Math.Max(elmWidth, b.FinalWidth);

                blocksHeight += b.FinalHeight;
            }

            blocksHeight = ExpandHeight(blocksHeight);

            if (this.Width != elmWidth)
            {
                this.Width = elmWidth;
            }

            if (this.Height != blocksHeight)
            {
                this.Height = Math.Max(blocksHeight, this.Height);
            }

            // NOTE: If nowrap attribute is enabled - min size is equals to content size.
            if (NoWrapEnabled())
            {
                this.MinWidth = this.Width;
                this.MinHeight = this.Height;
            }
        }

        /// <summary>
        /// Overloaded. Defines the start position for the block.
        /// </summary>
        /// <param name="block">Block which was filled (may be NULL if there are no filled blocks).</param>
        protected void ChangeStartPosition(Block block)
        {
            // Change coordinates (at least one block already exists and is filled by elements).
            if (block != null)
            {
                m_curPos.X = this.Bounds.X;
                m_curPos.Y += GetIncreaseValue();
            }
        }

        /// <summary>
        /// Defines the start position for the block if we want to add a new child TAG element.
        /// </summary>
        /// <param name="childRect">Rectangle for changing position.</param>
        protected void ChangeStartPosition(Rectangle childRect)
        {
            // Change coordinates (at least one block already exists and is filled by elements).
            m_curPos.X = childRect.X;
            m_curPos.Y = childRect.Y;
        }

        /// <summary>
        /// Returns the Y value by which we must increase the current Y coordinate.
        /// </summary>
        /// <returns>Y increases value for new line.</returns>
        internal int GetIncreaseValue()
        {
            bool emptyElement = m_blocks.Count <= 1;
            if (emptyElement)
            {
                //// Nothing was added in this element.
                BaseElement curElement = this;
                int increase = 0;

                if (m_curBlock != null && m_blocks.Count == 1)
                {
                    increase = m_curBlock.FinalHeight;
                }
                if (this.IsBlock) return increase;

                while (true)
                {
                    if (curElement.IsBlock) return increase;

                    BaseElement parent = (BaseElement)curElement.Parent;

                    if (parent == null) return increase;

                    increase = Math.Max(increase, parent.GetIncreaseValue());
                    curElement = parent;
                }
            }
            else
            {
                //// We already added something in this element.
                return m_curBlock.FinalHeight;
            }
        }

        /// <summary>
        /// Divides the string into words and returns an array of words.
        /// </summary>
        /// <param name="str">Regex pattern.</param>
        /// <returns>Collection of results.</returns>
        protected ArrayList InFillBlocks(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length == 0)
                throw new ArgumentException("String can't be empty.");

            MatchCollection matches = _regWords.Matches(str);
            int len = matches.Count;
            Match[] _tmp = new Match[len];
            matches.CopyTo(_tmp, 0);

            ArrayList tokens = new ArrayList(len);
            Match m = null;

            for (int i = 0; i < len; i++)
            {
                m = _tmp[i]; //// as Match;
                string val = m.Groups[0].Value;

                if (val.Length > 0)
                {
                    tokens.Add(val);
                }
            }

            return tokens;
        }

        /// <summary>
        /// Indicates whether the child element is located in the current block of the current element.
        /// </summary>
        /// <param name="childRect">Rectangle of the child block.</param>
        /// <returns>True if child element is located in the current block of the current element; false otherwise.</returns>
        protected bool ElementInSameBlock(Rectangle childRect)
        {
            return m_curPos.Equals(childRect.Location);
        }

        /// <summary>
        /// Sets the current position of the element after its positioning.
        /// Needed for its parent element.
        /// </summary>
        protected void SetLastCurrentPosition()
        {
            if (!this.IsBlock && m_curBlock != null && !m_curBlock.IsEmpty)
            {
                //// Fix block position.
                m_curBlock.X -= this.IndentSpace.Left;
                m_curBlock.Y -= this.IndentSpace.Top;

                m_curBlock.FinalWidth += this.IndentSpace.Left + this.IndentSpace.Right;
                m_curBlock.FinalHeight += this.IndentSpace.Top + this.IndentSpace.Bottom;
            }

            if (m_blocks.Count > 0 && !m_curBlock.IsEmpty)
            {
                Rectangle rect = m_curBlock.Rectangle;
                m_curPos.X = rect.X + rect.Width;
                m_curPos.Y = rect.Y;

                if (!this.IsBlock)
                {
                    rect = m_blocks[0].Rectangle;
                    this.Location = rect.Location;
                }
            }
            //// Check if some whitespace just after this element is allowed or not.
            ProhibitSpaceAfter();
        }

        /// <summary>
        /// Sets the location of the element.
        /// </summary>
        private void SetAttributes()
        {
            if (this.IsBlock && this.MainBlock != null)
            {
                // Fix position of the block.
                this.MainBlock.QuietMode = true;
                this.MainBlock.X = this.MainBlock.X;
                this.MainBlock.Y = this.MainBlock.Y;
                this.MainBlock.QuietMode = false;

                this.X = this.MainBlock.X;
                this.Y = this.MainBlock.Y;
            }
        }

        /// <summary>
        /// Reduces the rectangle by space of the element.
        /// </summary>
        /// <param name="rect">Rectangle for bounds reducing.</param>
        /// <returns>Rectangle with new attributes.</returns>
        protected internal virtual Rectangle ReduceBounds(Rectangle rect)
        {
            rect.X += this.IndentSpace.Left;
            rect.Y += this.IndentSpace.Top;

            rect.Width -= this.IndentSpace.Left + this.IndentSpace.Right;
            rect.Height -= this.IndentSpace.Top + this.IndentSpace.Bottom;

            return rect;
        }

        /// <summary>
        /// Shifts the location by element's space to the right / bottom.
        /// </summary>
        /// <param name="location">Point structure object.</param>
        /// <returns>Shifted location.</returns>
        protected internal virtual Point ReduceLocation(Point location)
        {
            location.X += this.IndentSpace.Left;
            location.Y += this.IndentSpace.Top;

            return location;
        }

        /// <summary>
        /// Reduces the width by space of the element.
        /// </summary>
        /// <param name="width">Width of the block.</param>
        /// <returns>New width value.</returns>
        protected internal virtual int ReduceWidth(int width)
        {
            width -= this.IndentSpace.Left + this.IndentSpace.Right;

            return width;
        }

        /// <summary>
        /// Reduces the height by space of the element.
        /// </summary>
        /// <param name="height">Width of the block.</param>
        /// <returns>Height after reducing.</returns>
        protected internal virtual int ReduceHeight(int height)
        {
            height -= this.IndentSpace.Top + this.IndentSpace.Bottom;

            return height;
        }

        /// <summary>
        /// Expands the rectangle by space of the element.
        /// </summary>
        /// <param name="rect">Rectangle for expanding.</param>
        /// <returns>Expanded rectangle.</returns>
        protected internal virtual Rectangle ExpandBounds(Rectangle rect)
        {
            rect.X -= this.IndentSpace.Left;
            rect.Y -= this.IndentSpace.Top;

            rect.Width += this.IndentSpace.Left + this.IndentSpace.Right;
            rect.Height += this.IndentSpace.Top + this.IndentSpace.Bottom;

            return rect;
        }

        /// <summary>
        /// Shifts the location by element's space to the left / top.
        /// </summary>
        /// <param name="location">Point structure object.</param>
        /// <returns>Shifted location.</returns>
        protected internal virtual Point ExpandLocation(Point location)
        {
            location.X -= this.IndentSpace.Left;
            location.Y -= this.IndentSpace.Top;

            return location;
        }

        /// <summary>
        /// Expands the width by space of the element.
        /// </summary>
        /// <param name="width">Width of the block.</param>
        /// <returns>New width value.</returns>
        protected internal virtual int ExpandWidth(int width)
        {
            width += this.IndentSpace.Left + this.IndentSpace.Right;

            return width;
        }

        /// <summary>
        /// Reduces the height by space of the element.
        /// </summary>
        /// <param name="height">Width of the block.</param>
        /// <returns>Height after expanding.</returns>
        protected internal virtual int ExpandHeight(int height)
        {
            height += this.IndentSpace.Top + this.IndentSpace.Bottom;

            return height;
        }

        /// <summary>
        /// Shifts the start position of the element.
        /// </summary>
        /// <param name="position">Start position.</param>
        /// <returns>Point instance</returns>
        protected internal virtual Point ShiftStartPos(Point position)
        {
            position.X += this.IndentSpace.Left;
            position.Y += this.IndentSpace.Top;

            return position;
        }

        /// <summary>
        /// Sets the value indicating whether trailing whitespace must be allowed at the beginning of the element.
        /// </summary>
        protected virtual void ProhibitSpaceBefore()
        {
            this.SpaceProhibited = (this.Type & ElementType.Block) > 0;
        }

        /// <summary>
        /// Sets the value indicating whether trailing whitespace must be allowed after element.
        /// </summary>
        protected virtual void ProhibitSpaceAfter()
        {
            if (this.Parent != null)
            {
                BaseElement parent = this.Parent as BaseElement;
                parent.SpaceProhibited = (this.Type & ElementType.NewLine) > 0;
            }
        }

        /// <summary>
        /// Indicates whether the node is text and has some non-empty value.
        /// </summary>
        /// <param name="node">Node object.</param>
        /// <returns>True if node is text and has some non-empty value; False otherwise.</returns>
        private bool IsValidText(XmlNode node)
        {
            XmlText text = node as XmlText;
            XmlWhitespace whitespace = node as XmlWhitespace;

            return (text != null || whitespace != null) && node.Value.Length > 0;
        }

        /// <summary>
        /// Indicates whether the text object needs to be calculated and positioned.
        /// </summary>
        /// <param name="text">Text object.</param>
        /// <returns>True if text object needs to be calculated and positioned; False otherwise.</returns>
        private bool NeedCalculate(Text text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            bool result = text.Value.Length > 0;

            if (AttributeParser.IsWhitespace(text.Value) &&
              this.SkipWhiteSpaces && SpaceProhibited)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Prepares the element to position it's content.
        /// </summary>
        /// <param name="currentPosition">Current start position for this element.</param>
        /// <param name="bounds">Bound for this element.</param>
        protected virtual void PreparePositioning(Point currentPosition, Rectangle bounds)
        {
            m_curPos = currentPosition;
            m_bounds = bounds;

            //// If nowrap is enabled our bound by width is unbounded.
            if (NoWrapEnabled())
            {
                m_bounds.Width = Int32.MaxValue;
            }

            //// Clear all blocks if we are redrawing element.
            m_blocks.Clear();
            m_curBlock = null;

            //// If element is invisible, skip position calculating method.
            if (this.IsVisible)
            {
                //// Shift start position by left / top space of element.
                m_curPos = ShiftStartPos(m_curPos);

                CreateBlock(null);
            }

            ProhibitSpaceBefore();
        }

        /// <summary>
        /// Indicates whether the nowrap attribute is enabled.
        /// </summary>
        /// <returns>True if the attribute is set; False otherwise.</returns>
        protected bool NoWrapEnabled()
        {
            // NOTE: nowrap is enbaled when NOWRAP attribute is present and width 
            // is not set.
            bool numberWidthSet = IsStyleWidth && GetWidthType() == SizeTypeEx.Number;
            bool hasNoWrap = this.Attributes.Contains(AttributeName.NoWrap) && !numberWidthSet;

            if (!hasNoWrap && Parent != null)
            {
                hasNoWrap = (Parent as BaseElement).NoWrapEnabled();
            }

            return hasNoWrap;
        }

        /// <summary>
        /// Splits one token if it's longer than length into smaller pieces.
        /// </summary>
        /// <param name="line">Token string.</param>
        /// <param name="tokens">Array of tokens.</param>
        /// <param name="i">Index of the token in the array.</param>
        /// <param name="length">Max length of the line.</param>
        private void SplitToken(string line, ArrayList tokens, int i, int length)
        {
            if (line == null)
                throw new ArgumentNullException("line");
            if (tokens == null)
                throw new ArgumentNullException("tokens");
            if (i < 0 || i >= tokens.Count)
                throw new ArgumentOutOfRangeException("i", i, "Value can not be less 0 and greater tokens.Count");

            int tokenLength = 1;
            string word = string.Empty;

            while (true)
            {
                if (tokenLength > line.Length) break;

                string token = line.Substring(0, tokenLength);
                Size tokenSize = MeasureString(token, Format.Font);

                //// token is larger than width and it's not the first symbol.
                if (tokenSize.Width > length && tokenLength > 1)
                {
                    //// Split token on two parts and add them to the general tokens collection.
                    string secondWord = line.Substring(word.Length);

                    if (word.Length > 0)
                    {
                        tokens.RemoveAt(i);
                        tokens.Insert(i, word);
                    }
                    if (secondWord.Length > 0)
                    {
                        tokens.Insert(i + 1, secondWord);
                    }
                    break;
                }
                else if (tokenSize.Width > length && tokenLength == 1)
                {
                    //// The first symbol is larger than the width.
                    break;
                }

                tokenLength++;
                word = token;
            }
        }
        #endregion

        #region Draw Methods

        /// <summary>
        /// Draws an element.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        public void DrawElement(PaintEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            DrawElementInternal(e);
        }

        /// <summary>
        /// Checks whether the element must be redrawn. If element must be drawn, the method draws it.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void DrawElementInternal(PaintEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            Rectangle elRect = new Rectangle(this.Location, this.Size);
            Rectangle clipRect = ClientToGlobal(e.ClipRectangle);
            elRect = ExpandBounds(elRect);

            // Paint element if element is in the visible area or is painted at first.
            if (elRect.IntersectsWith(clipRect) || !this.Document.WasPainted)
            {
                Draw(e);
            }
        }

        /// <summary>
        /// Draws an element.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void Draw(PaintEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            Graphics g = e.Graphics;

            // Draw main block. (Element is block).
            if (this.MainBlock != null)
            {
                DrawBlock(this.MainBlock, e);
            }
            else
            {
                //// Element is inline.
                for (int i = 0, len = this.Blocks.Count; i < len; i++)
                {
                    DrawBlock(this.Blocks[i] as Block, e);
                }
            }
        }

        /// <summary>
        /// Raises the event before block painting and if there are no event handlers,
        /// raises the default painting.
        /// </summary>
        /// <param name="block">Block for drawing.</param>
        /// <param name="e">Paint arguments.</param>
        protected virtual void DrawBlock(Block block, PaintEventArgs e)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (e == null)
                throw new ArgumentNullException("e");

            ElementPaintEventArgs args = new ElementPaintEventArgs(e, block);

            // Raise Paint event.
            OnPaint(args);

            // Checks if painting should be cancelled or not.
            if (!args.Cancel)
            {
                ProcessDrawBlock(block, e);
            }
        }

        /// <summary>
        /// Draws data in block which represents a single line.
        /// </summary>
        /// <param name="block">Current block.</param>
        /// <param name="e">Paint arguments.</param>
        protected internal virtual void ProcessDrawBlock(Block block, PaintEventArgs e)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (e == null)
                throw new ArgumentNullException("e");

            // Realign elements.
            Rectangle clipRect = ClientToGlobal(e.ClipRectangle);
            ProcessAlignment(block);

            // Add block to searcher of elements.
            if (!block.IsVShifted)
            {
                this.Document.Searcher.AddBlock(block);
            }

            // Indicates not to shift blocks because they are already shifted.
            block.IsHShifted = true;
            block.IsVShifted = true;

            if (!MustDrawBlock(block, clipRect)) return;

            // Draw background.
            DrawBgColor(block, e.Graphics);
            DrawBgImage(block, e.Graphics);

            // Paint content of the block.
            DrawChildsOf(block, e);

            // Draw border around block.
            DrawBorders(block, e.Graphics);
        }

        /// <summary>
        /// Draws text in the element.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="rect">Rectangle for output.</param>
        /// <param name="text">Text object.</param>
        /// <param name="parent">Parent block for the text.</param>
        protected virtual void DrawText(Graphics g, Rectangle rect, Text text, Block parent)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (parent == null)
                throw new ArgumentNullException("parent");

            HTMLFormat format = this.Format as HTMLFormat;

            using (SolidBrush brush = new SolidBrush(format.ForeColor))
            {
                DrawText(g, brush, format.Font, text.Value, rect);
            }

            // Prepare text for selection.
            CalculateSelectionText(text, g, rect, format.Font);

            // If printing - insert the data.
            if (this.Document.IsPrinting && this.Document.TextRegionManager != null)
            {
                Rectangle globalRect = ClientToGlobal(rect);
                TextRegion region = new TextRegion(globalRect.Y, globalRect.Height);
                this.Document.TextRegionManager.Add(region);
            }

            DrawSelectedText(g, rect, text, parent);
        }

        /// <summary>
        /// Draws all childs elements of the current block.
        /// </summary>
        /// <param name="block">Current block for drawing.</param>
        /// <param name="e">Paint event arguments.</param>
        private void DrawChildsOf(Block block, PaintEventArgs e)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (e == null)
                throw new ArgumentNullException("e");

            object element = null;

            for (int i = 0, len = block.Count; i < len; i++)
            {
                element = block[i];

                //// Draw block.
                if (element is Block)
                {
                    Block childBlock = element as Block;
                    BaseElement childElement = childBlock.Owner;

                    childElement.DrawBlock(childBlock, e);
                }
                else if (element is Text)
                {
                    //// draw text
                    Rectangle elRect = GlobalToClient(block[element]);

                    DrawText(e.Graphics, elRect, element as Text, block);
                }
                else if (element is BaseElement)
                {
                    (element as BaseElement).DrawElement(e);
                }
            }
        }

        /// <summary>
        /// Draw text with specified settings.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="brush">Brush for text drawing.</param>
        /// <param name="font">Font object.</param>
        /// <param name="text">Text data.</param>
        /// <param name="bound">Bound for the text.</param>
        private void DrawText(Graphics g, Brush brush, Font font, string text, RectangleF bound)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            if (brush == null)
                throw new ArgumentNullException("brush");

            if (text == null)
                throw new ArgumentNullException("text");

            g.DrawString(text, font, brush, bound.X, bound.Y, _stringFormat);
        }

        /// <summary>
        /// Draws selected text in the block.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="rect">Bound for the text.</param>
        /// <param name="text">Text data.</param>
        /// <param name="parent">Parent block for the text.</param>
        private void DrawSelectedText(Graphics g, Rectangle rect, Text text, Block parent)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            if (text == null)
                throw new ArgumentNullException("text");

            if (parent == null)
                throw new ArgumentNullException("parent");

            RectangleF clipRect;
            clipRect = GetSelectedRect(parent, text);

            clipRect = GlobalToClient(clipRect);

            // Block contains selected text and part of text is painting which is selected.
            if (!clipRect.IsEmpty)
            {
                GraphicsState oldState = g.Save();
                g.SetClip(clipRect);
                DrawSelectionRectangle(g, clipRect);
                DrawText(g, SystemBrushes.HighlightText, this.Format.Font, text.Value, rect);

                RegisterSelectedElement(this);

                g.Restore(oldState);
            }
        }

        /// <summary>
        /// Draws selection rectangle.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="rect">Selection rectangle.</param>
        private void DrawSelectionRectangle(Graphics g, RectangleF rect)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            g.FillRectangle(SystemBrushes.Highlight, rect);
        }

        /// <summary>
        /// Overloaded. Translates logical rectangle to client rectangle.
        /// </summary>
        /// <param name="rect">Rectangle for converting attributes.</param>
        /// <returns>Changed rectangle.</returns>
        protected internal Rectangle ClientToGlobal(Rectangle rect)
        {
            rect.X -= this.Document.AutoScrollPosition.X;
            rect.Y -= this.Document.AutoScrollPosition.Y;

            return rect;
        }

        /// <summary>
        /// Overloaded. Translates client rectangle to global rectangle.
        /// </summary>
        /// <param name="rect">Rectangle for converting attributes.</param>
        /// <returns>Changed rectangle.</returns>
        protected internal Rectangle GlobalToClient(Rectangle rect)
        {
            rect.X += this.Document.AutoScrollPosition.X;
            rect.Y += this.Document.AutoScrollPosition.Y;

            return rect;
        }

        /// <summary>
        /// Translates logical rectangle to client rectangle.
        /// </summary>
        /// <param name="rect">Rectangle for converting attributes.</param>
        /// <returns>Changed rectangle.</returns>
        protected internal RectangleF ClientToGlobal(RectangleF rect)
        {
            rect.X -= this.Document.AutoScrollPosition.X;
            rect.Y -= this.Document.AutoScrollPosition.Y;

            return rect;
        }

        /// <summary>
        /// Translates client rectangle to global rectangle.
        /// </summary>
        /// <param name="rect">Rectangle for converting attributes.</param>
        /// <returns>Changed rectangle.</returns>
        protected internal RectangleF GlobalToClient(RectangleF rect)
        {
            rect.X += this.Document.AutoScrollPosition.X;
            rect.Y += this.Document.AutoScrollPosition.Y;

            return rect;
        }

        /// <summary>
        /// Draws background.
        /// </summary>
        /// <param name="block">Current block.</param>
        /// <param name="g">Graphics context.</param>
        private void DrawBgColor(Block block, Graphics g)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (g == null)
                throw new ArgumentNullException("g");

            if (!block.DrawProperties) return;

            if (this.IsBlock && !block.IsMain) return;

#if DEBUG
            try
            {
#endif
            if (this.Format.BackgroundColor != this.Document.RenderRoot.Format.BackgroundColor)
            {
                SolidBrush bgBrush = new SolidBrush(this.Format.BackgroundColor);

                Rectangle bgRect = block.Rectangle;
                //// bgRect.Width  -= this.Format.Left.Width - this.Format.Right.Width;
                //// bgRect.Height -= this.Format.Top.Width  - this.Format.Bottom.Width;

                g.FillRectangle(bgBrush, GlobalToClient(bgRect));

                bgBrush.Dispose();
                bgBrush = null;
            }
#if DEBUG
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");
                throw;
            }
#endif
        }

        /// <summary>
        /// Draw borders for the element.
        /// </summary>
        /// <param name="block">Block Instance</param>
        /// <param name="g">Graphics context.</param>
        protected void DrawBorders(Block block, Graphics g)
        {
            if (block == null)
                throw new ArgumentNullException("block");
            if (g == null)
                throw new ArgumentNullException("g");

            if (!block.DrawProperties) return;

            if (this.IsBlock && !block.IsMain) return;

            if (Format.Top.Width <= 0 &&
              Format.Left.Width <= 0 &&
              Format.Right.Width <= 0 &&
              Format.Bottom.Width <= 0) return;

            //// Start point to border line.
            Point startPoint = Point.Empty;

            //// End pont to border line.
            Point endPoint = Point.Empty;

            //// Rectangle of the block.
            Rectangle blockPos = block.Rectangle;

            if (blockPos.Width <= 0 || blockPos.Height <= 0) return;
            blockPos = GlobalToClient(blockPos);

            //// Defines values of top and bottom shift values by which we must
            //// expand side of borders position.
            int leftShift = 0;
            int topShift = 0;
            int rightShift = 0;
            int botShift = 0;
            int leftRightBotShift = 0;

            if (block.Owner is TDElementImpl == false)
            {
                float shift = (float)this.Format.Left.Width / 2f;
                leftShift = (shift >= 1) ? (int)shift : 0;

                shift = (float)this.Format.Top.Width / 2f;
                topShift = (shift >= 1) ? (int)shift : 0;

                shift = (float)this.Format.Right.Width / 2f;
                rightShift = (shift >= 1) ? (int)Math.Ceiling(shift) : 1;

                shift = (float)this.Format.Bottom.Width / 2f;
                botShift = (shift >= 1) ? (int)Math.Ceiling(shift) : 1;

                // NOTE: Workaround due to strange painting of the 1 px lines.
                leftRightBotShift = (this.Format.Bottom.Width == 1) ? 1 : 0;
            }

            //// Top border:
            if (this.Format.Top.Width > 0)
            {
                int brdWidth = this.Format.Top.Width;
                startPoint = new Point(blockPos.X, blockPos.Y + topShift);
                endPoint = new Point(startPoint.X + blockPos.Width - rightShift, startPoint.Y);

                DrawBorder(g, (Border)this.Format.Top, startPoint, endPoint);
            }

            //// Bottom border:
            if (this.Format.Bottom.Width > 0)
            {
                int brdWidth = this.Format.Bottom.Width;
                startPoint = new Point(blockPos.X, blockPos.Y + blockPos.Height - botShift);
                endPoint = new Point(startPoint.X + blockPos.Width - rightShift, startPoint.Y);

                DrawBorder(g, (Border)this.Format.Bottom, startPoint, endPoint);
            }

            //// Left border:
            if (this.Format.Left.Width > 0)
            {
                int brdWidth = this.Format.Left.Width;
                startPoint = new Point(blockPos.X + leftShift, blockPos.Y);
                endPoint = new Point(startPoint.X, startPoint.Y + blockPos.Height - leftRightBotShift);

                DrawBorder(g, (Border)this.Format.Left, startPoint, endPoint);
            }

            //// Right border:
            if (this.Format.Right.Width > 0)
            {
                int brdWidth = this.Format.Right.Width;
                startPoint = new Point(blockPos.X + blockPos.Width - rightShift, blockPos.Y);
                endPoint = new Point(startPoint.X, startPoint.Y + blockPos.Height - leftRightBotShift);

                DrawBorder(g, (Border)this.Format.Right, startPoint, endPoint);
            }
        }

        /// <summary>
        /// Draws border for the element.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="border">Border instance.</param>
        /// <param name="startP">Start point.</param>
        /// <param name="endP">End point.</param>
        private void DrawBorder(Graphics g, Border border, Point startP, Point endP)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            if (border == null)
                throw new ArgumentNullException("border");

            // TODO: Implement different styles of the border.
            using (Pen pen = new Pen(border.Color, border.Width))
            {
                g.DrawLine(pen, startP, endP);
            }
        }

        /// <summary>
        /// Indicates whether the block is inside ClipRectangle which must be repainted.
        /// </summary>
        /// <param name="block">Current block object.</param>
        /// <param name="clipRect">Clip rectangle.</param>
        /// <returns>True if block must be drawn.</returns>
        private bool MustDrawBlock(Block block, Rectangle clipRect)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            Rectangle blockRect = block.Rectangle;

            // NOTE: this is workaround for correct drawing of borders.
            int add = 1;
            blockRect.Width += add;
            blockRect.Height += add;

            // Paint block when it is inside clip bounds or is painted at first and is visible.
            return (clipRect.IntersectsWith(blockRect) || !this.Document.WasPainted
              ) && this.IsVisible;
        }

        /// <summary>
        /// Changes positions of the elements in the block depending on the alignment.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <returns>Block instance</returns>
        private Block ProcessAlignment(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            Block result = block;

            if (!block.IsHShifted || !block.IsVShifted)
            {
                //// Reset cached value.
                block.Rectangle = Rectangle.Empty;

                result = ProcessHAlignment(block);
                result = ProcessVAlignment(result);

                //// Save new value to cache.
                block.Rectangle = block.Rectangle;
            }

            return result;
        }

        /// <summary>
        /// Shifts all elements depending on horizontal alignment.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <returns>Block instance</returns>
        private Block ProcessHAlignment(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            StringAlignment ownerHAlign = block.Owner.Format.HorizontalAlign;

            if (ownerHAlign == StringAlignment.Near)
            {
                return block;
            }

            if (block.IsHShifted) return block;

            Block result = null;

            if (block.IsMain)
            {
                result = MoveElementsInMainBlock(block);
            }
            else
            {
                result = MoveElementsInInlineBlock(block);
            }

            return result;
        }

        /// <summary>
        /// Moves all elements in simple inline block.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <returns>Block after alignment.</returns>
        private Block MoveElementsInInlineBlock(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            StringAlignment ownerHAlign = block.Owner.Format.HorizontalAlign;
            if (ownerHAlign == StringAlignment.Near) return block;

            if (block.IsHShifted) return block;

            int shiftVal = GetFreeWidth(block);

            if (ownerHAlign == StringAlignment.Center)
            {
                shiftVal /= 2;
            }

            return MoveElementsHorizontal(block, shiftVal);
        }

        /// <summary>
        /// Moves all elements inside the main block.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <returns>Block after alignment.</returns>
        private Block MoveElementsInMainBlock(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            StringAlignment ownerHAlign = block.Owner.Format.HorizontalAlign;
            if (ownerHAlign == StringAlignment.Near) return block;

            if (block.IsHShifted || !block.IsMain) return block;

            object item = null;
            Block child = null;

            for (int i = 0, len = block.Count; i < len; i++)
            {
                item = block[i];
                if (item is Block == false) continue;

                child = item as Block;
                int shiftVal = GetFreeWidthInMainBlock(block, child);

                if (ownerHAlign == StringAlignment.Center)
                {
                    shiftVal /= 2;
                }

                if (shiftVal > 0)
                {
                    child.X += shiftVal;
                    child.Owner.MoveElementsHorizontal(child, shiftVal);

                    Rectangle childRect = child.Rectangle;
                    block.Add(child, childRect);
                    child.IsHShifted = true;
                }
            }

            return block;
        }

        /// <summary>
        /// Move all elements in block by the specified shiftValue in right.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <param name="shiftValue">Value on which block must be shifted.</param>
        /// <returns>Block after alignment.</returns>
        internal Block MoveElementsHorizontal(Block block, int shiftValue)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (shiftValue <= 0) return block;

            if (block == null)
                throw new ArgumentNullException("block");

            if (block.IsHShifted) return block;

            object element = null;

            for (int i = 0, len = block.Count; i < len; i++)
            {
                element = block[i];

                if (element is Block)
                {
                    Block childBlock = element as Block;

                    childBlock.X += shiftValue;
                    childBlock = childBlock.Owner.MoveElementsHorizontal(childBlock, shiftValue);

                    // Reset / save cached value.
                    childBlock.ResetRectCache();

                    Rectangle childRect = childBlock.Rectangle;
                    block.Add(childBlock, childRect);
                }
                else if (element is Text)
                {
                    // Reset symbols calculating in the text, because all text is moved.
                    (element as Text).IsCalculated = false;

                    Rectangle textRect = block[element];
                    textRect.X += shiftValue;
                    block.Add(element, textRect);
                }
                else if (element is IHTMLElement)
                {
                    Rectangle elementRect = block[element];
                    elementRect.X += shiftValue;
                    ((BaseElement)element).X += shiftValue;

                    block.Add(element, elementRect);
                }
            }

            return block;
        }

        /// <summary>
        /// Shifts all elements in the bloack depending on vertical alignment.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <returns>Block after aligning.</returns>
        private Block ProcessVAlignment(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            StringAlignment ownerVAlign = block.Owner.Format.VerticalAlign;

            if (ownerVAlign == StringAlignment.Near)
            {
                return block;
            }
            if (block.IsVShifted) return block;

            Block result = null;
            switch (ownerVAlign)
            {
                case StringAlignment.Center: result = MoveToVCenter(block);
                    break;
                case StringAlignment.Far: result = MoveToVBottom(block);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns the value for shifting elements in the block.
        /// </summary>
        /// <param name="block">Block instance</param>
        /// <returns>Free width value.</returns>
        private int GetFreeWidth(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (block.Owner == this.Document.RenderRoot)
            {
                return block.Owner.Width - block.FinalWidth;
            }

            return block.FinalWidth - block.Width;
        }

        /// <summary>
        /// Returns the free width between the main block and child block.
        /// </summary>
        /// <param name="mainBlock">Main block.</param>
        /// <param name="childBlock">Current child block.</param>
        /// <returns>Free space in main block.</returns>
        private int GetFreeWidthInMainBlock(Block mainBlock, Block childBlock)
        {
            if (mainBlock == null)
                throw new ArgumentNullException("mainBlock");

            if (childBlock == null)
                throw new ArgumentNullException("childBlock");

            return mainBlock.Owner.ReduceWidth(mainBlock.FinalWidth) - childBlock.FinalWidth;
        }

        /// <summary>
        /// Shifts all elements in block corresponding to the middle vertical position.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <returns>Block after aligning.</returns>
        private Block MoveToVCenter(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (block.IsVShifted) return block;

            int blockHeight = block.Owner.ReduceHeight(block.FinalHeight);
            int shiftVal = 0;

            object element = null;

            for (int i = 0, len = block.Count; i < len; i++)
            {
                element = block[i];

                if (element is Block)
                {
                    Block childBlock = element as Block;
                    Rectangle blockRect = block[element];

                    if (!block.IsMain)
                    {
                        shiftVal = (int)((blockHeight - blockRect.Height) / 2);
                    }
                    else
                    {
                        shiftVal = (int)((block.Owner.ReduceHeight(block.FinalHeight) - block.Height) / 2);
                    }

                    if (shiftVal > 0)
                    {
                        childBlock.Y += shiftVal;
                        childBlock = childBlock.Owner.ShiftVertical(childBlock, shiftVal);
                    }

                    childBlock.ResetRectCache();

                    block.Add(childBlock, childBlock.Rectangle);
                }
                else if (element is Text)
                {
                    // Reset symbols calculating in the text, because all text is moved.
                    (element as Text).IsCalculated = false;

                    Rectangle textRect = block[element];
                    shiftVal = (int)((blockHeight - textRect.Height) / 2);
                    if (shiftVal > 0)
                    {
                        textRect.Y += shiftVal;
                        block.Add(element, textRect);
                    }
                }
                else if (element is IHTMLElement)
                {
                    Rectangle elementRect = block[element];
                    shiftVal = (int)((blockHeight - elementRect.Height) / 2);

                    if (shiftVal > 0)
                    {
                        elementRect.Y += shiftVal;
                        ((BaseElement)element).Y += shiftVal;
                        block.Add(element, elementRect);
                    }
                }
            }
            return block;
        }

        /// <summary>
        /// Shifts all elements inside the block by the defined value.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <param name="shiftVal">Value by which all elements in block must be shifted down.</param>
        /// <returns>Block after aligning.</returns>
        private Block ShiftVertical(Block block, int shiftVal)
        {
            if (shiftVal <= 0) return block;

            if (block == null)
                throw new ArgumentNullException("block");

            if (block.IsVShifted) return block;

            int blockHeight = block.FinalHeight;

            object element = null;

            for (int i = 0, len = block.Count; i < len; i++)
            {
                element = block[i];

                if (element is Block)
                {
                    Block childBlock = element as Block;
                    childBlock.Y += shiftVal;

                    childBlock = childBlock.Owner.ShiftVertical(childBlock, shiftVal);

                    childBlock.ResetRectCache();
                    Rectangle childRect = childBlock.Rectangle;

                    block.Add(childBlock, childRect);
                }
                else if (element is Text)
                {
                    // Reset symbols calculating in the text, because all text is moved.
                    (element as Text).IsCalculated = false;

                    Rectangle textRect = block[element];
                    textRect.Y += shiftVal;
                    block.Add(element, textRect);
                }
                else if (element is IHTMLElement)
                {
                    Rectangle elementRect = block[element];
                    elementRect.Y += shiftVal;
                    ((BaseElement)element).Y += shiftVal;

                    block.Add(element, elementRect);
                }
            }

            return block;
        }

        /// <summary>
        /// Shifts all elements in block corresponding to bottom vertical position.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <returns>Block after aligning.</returns>
        private Block MoveToVBottom(Block block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (block.IsVShifted) return block;

            //// if( block.Owner.IsBlock && !block.IsMain ) return block;

            int blockHeight = block.Owner.ReduceHeight(block.FinalHeight);
            int shiftVal = 0;

            object element = null;

            for (int i = 0, len = block.Count; i < len; i++)
            {
                element = block[i];

                if (element is Block)
                {
                    Block childBlock = element as Block;
                    Rectangle blockRect = block[element];

                    if (!block.IsMain)
                    {
                        shiftVal = blockHeight - blockRect.Height;
                    }
                    else
                    {
                        shiftVal = block.Owner.ReduceHeight(block.FinalHeight) - block.Height;
                    }

                    if (shiftVal > 0)
                    {
                        childBlock.Y += shiftVal;
                        childBlock = childBlock.Owner.ShiftVertical(childBlock, shiftVal);
                    }

                    childBlock.ResetRectCache();

                    Rectangle childRect = childBlock.Rectangle;
                    block.Add(childBlock, childRect);
                }
                else if (element is Text)
                {
                    Rectangle textRect = block[element];
                    shiftVal = blockHeight - textRect.Height;

                    if (shiftVal > 0)
                    {
                        // Reset symbols calculating in the text, because all text is moved.
                        (element as Text).IsCalculated = false;

                        textRect.Y += shiftVal;
                        block.Add(element, textRect);
                    }
                }
                else if (element is IHTMLElement)
                {
                    Rectangle elementRect = block[element];
                    shiftVal = blockHeight - elementRect.Height;

                    if (shiftVal > 0)
                    {
                        elementRect.Y += shiftVal;
                        ((BaseElement)element).Y += shiftVal;
                        block.Add(element, elementRect);
                    }
                }
            }

            return block;
        }

        /// <summary>
        /// Draws background image for the element.
        /// </summary>
        /// <param name="block">Block for its content alignment.</param>
        /// <param name="g">Graphic context.</param>
        private void DrawBgImage(Block block, Graphics g)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            if (g == null)
                throw new ArgumentNullException("g");

            HTMLFormat format = this.Format as HTMLFormat;

            if (format.BackgroundImage == null) return;
            if (!block.DrawProperties) return;
            if (this.IsBlock && !block.IsMain) return;

            Rectangle destRect = block.Rectangle;
            destRect.X += format.Left.Width;
            destRect.Y += format.Top.Width;
            destRect.Width -= format.Left.Width + format.Right.Width;
            destRect.Height -= format.Top.Width + format.Bottom.Width;

            Rectangle srcRect = new Rectangle(0, 0, format.BackgroundImage.Width, format.BackgroundImage.Height);

            switch (format.BackgroundImageRepeat)
            {
                case RepeatStyle.RepeatX:
                    destRect.Height = (int)Math.Min(destRect.Height, srcRect.Height);
                    break;
                case RepeatStyle.RepeatY:
                    destRect.Width = (int)Math.Min(destRect.Width, srcRect.Width);
                    break;
                case RepeatStyle.NoRepeat:
                    destRect.Width = (int)Math.Min(destRect.Width, srcRect.Width);
                    destRect.Height = (int)Math.Min(destRect.Height, srcRect.Height);
                    break;
            }

            srcRect.Width = destRect.Width;
            srcRect.Height = destRect.Height;

            DrawImg(g, destRect, srcRect);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="g">Graphic context.</param>
        /// <param name="destRect">Destination rectangle for output.</param>
        /// <param name="srcRect">Rectangle of image to draw.</param>
        private void DrawImg(Graphics g, Rectangle destRect, Rectangle srcRect)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            HTMLFormat format = this.Format as HTMLFormat;

            if (format.BackgroundImage == null) return;

            destRect = GlobalToClient(destRect);

            using (ImageAttributes attr = new ImageAttributes())
            {
                attr.SetWrapMode(WrapMode.Tile);

                // Location of three edges of rectangle for drawing.
                Point p1 = destRect.Location;
                Point p2 = new Point(p1.X + destRect.Width, p1.Y);
                Point p3 = new Point(p1.X, p1.Y + destRect.Height);
                Point[] destPoints = { p1, p2, p3 };

                g.DrawImage(format.BackgroundImage, destPoints, srcRect, GraphicsUnit.Pixel, attr);
            }
        }
        #endregion

        #region Selection methods

        /// <summary>
        /// Processes the selection algorithm.
        /// </summary>
        protected internal void ProcessSelection()
        {
            // NOTE: improve following code. Only needed area must be repainted.
            this.Control.Invalidate();
        }

        /// <summary>
        /// Calculates the rectangle which suits the selected text in the block.
        /// </summary>
        /// <param name="block">Block containing selected text.</param>
        /// <param name="text">Current text object.</param>
        /// <returns>Rectangle which suits the selected text in the block.</returns>
        internal RectangleF GetSelectedRect(Block block, Text text)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            RectangleF commonBound = RectangleF.Empty;

            if (this.Control.SelectionManager.HasSelectedText)
            {
                Rectangle textSelRect = new Rectangle(text.StartNumber, 0, text.Value.Length, 1);

                int elmStartIndex = this.Control.SelectionManager.Region.FirstIndex;
                int elmLastIndex = this.Control.SelectionManager.Region.LastIndex;
                Rectangle docSelRect = new Rectangle(elmStartIndex, 0, elmLastIndex - elmStartIndex + 1, 1);

                textSelRect.Intersect(docSelRect);

                if (textSelRect.Width > 0)
                {
                    int startSymbol = textSelRect.X;
                    int endSymbol = textSelRect.Right - 1;

                    LetterPair smbInfo;

                    // Retrieve first symbol's bound.
                    smbInfo = text.Symbols[startSymbol];

                    if (smbInfo != null)
                    {
                        commonBound = smbInfo.Bounds;
                    }

                    // Retieve last symbol's bound.
                    smbInfo = text.Symbols[endSymbol];

                    if (smbInfo != null)
                    {
                        commonBound.Width = smbInfo.Bounds.Right - commonBound.Left;
                    }
                }
            }

            return commonBound;
        }

        /// <summary>
        /// Calculates all data regarding the selection of text in the document.
        /// </summary>
        /// <param name="text">Text object.</param>
        /// <param name="g">Graphics context.</param>
        /// <param name="clientRect">Rectangle of the text in the document.</param>
        /// <param name="textFont">Font of the text.</param>
        private void CalculateSelectionText(Text text, Graphics g, Rectangle clientRect, Font textFont)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (g == null)
                throw new ArgumentNullException("g");

            if (textFont == null)
                throw new ArgumentNullException("textFont");

            if (!text.IsCalculated)
            {
                Rectangle globalRect = ClientToGlobal(clientRect);

                this.Control.SelectionManager.CalculateText(text, globalRect, g, textFont);
            }
        }

        /// <summary>
        /// Adds the element to the collection of selected elements.
        /// </summary>
        /// <param name="element">Element is selected.</param>
        private void RegisterSelectedElement(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            SelectionManager manager = this.Control.SelectionManager;

            if (!manager.SelectedElements.Contains(element))
            {
                manager.SelectedElements.Add(element);
            }
        }

        /// <summary>
        /// Calculates the start / end indices of the text inside the element.
        /// </summary>
        /// <returns>Point instance</returns>
        private Point CalculateInnerTextIndexes()
        {
            SelectionRegion indexHolder = this.Control.SelectionManager.Region;
            Point indexes = Point.Empty;

            BaseElement elm = null;
            Queue elements = new Queue();
            elements.Enqueue(this);

            while (elements.Count > 0)
            {
                elm = elements.Dequeue() as BaseElement;
                Point elmIndexes = indexHolder.IndexOf(elm);

                // First time just assign values, not compare.
                if (indexes.IsEmpty)
                {
                    indexes = elmIndexes;
                }
                else
                {
                    indexes.X = Math.Min(indexes.X, elmIndexes.X);
                    indexes.Y = Math.Max(indexes.Y, elmIndexes.Y);
                }

                if (elm.Children.Count > 0)
                {
                    for (int i = 0, len = elm.Children.Count; i < len; i++)
                    {
                        elements.Enqueue(elm.Children[i]);
                    }
                }
            }

            return indexes;
        }

        /// <summary>
        /// Calculates the text inside the element.
        /// </summary>
        /// <returns>Inner text for the element.</returns>
        private string CalculateInnerText()
        {
            string text = string.Empty;
            Point indexes = CalculateInnerTextIndexes();

            if (!indexes.IsEmpty)
            {
                string documentText = this.Control.SelectionManager.DocumentText;

                int length = indexes.Y - indexes.X + 1;
                text = documentText.Substring(indexes.X, length);
            }

            return text;
        }

        /// <summary>
        /// Calculates the selected text for the element.
        /// </summary>
        /// <returns>Selected text for the element.</returns>
        private string CalculateSelectedText()
        {
            string selectedText = string.Empty;

            SelectionRegion indexHolder = this.Control.SelectionManager.Region;
            Point textIndexes = CalculateInnerTextIndexes();
            Rectangle elmBounds = new Rectangle(textIndexes.X, 0, textIndexes.Y - textIndexes.X + 1, 1);

            int selTextStart = indexHolder.FirstIndex;
            int selTextLast = indexHolder.LastIndex;
            Rectangle selBounds = new Rectangle(selTextStart, 0, selTextLast - selTextStart + 1, 1);

            elmBounds.Intersect(selBounds);

            if (!elmBounds.IsEmpty)
            {
                string documentText = this.Control.SelectionManager.DocumentText;
                selectedText = documentText.Substring(elmBounds.X, elmBounds.Width);
            }

            return selectedText;
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clone class members.
        /// </summary>
        /// <returns>Returns a new instance of the class with the same parameters set.</returns>
        public virtual object Clone()
        {
            if (IsDisposed)
                throw new ArgumentException("Can not clone Disposed object.");

            // All collections are created in the InitializeCollections() method.
            BaseElement elm = (BaseElement)this.MemberwiseClone();

            elm.m_format = null;
            elm.m_unique = null;

            ClearEvents(elm);
            elm.InitializeCollections();

            return elm;
        }

        /// <summary>
        /// Clears all events.
        /// </summary>
        /// <param name="element">New cloned object.</param>
        private void ClearEvents(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.RuntimeAttributeChanged != null)
            {
                element.RuntimeAttributeChanged = null;
            }

            if (element.ParentChanged != null)
            {
                element.ParentChanged = null;
            }

            if (element.AccessKeyChanged != null)
            {
                element.AccessKeyChanged = null;
            }

            if (element.UniqueIDChanged != null)
            {
                element.UniqueIDChanged = null;
            }

            if (element.InnerHTMLChanged != null)
            {
                element.InnerHTMLChanged = null;
            }

            if (element.BeforeStyleCalculated != null)
            {
                element.BeforeStyleCalculated = null;
            }

            if (element.Paint != null)
            {
                element.Paint = null;
            }

            if (element.SizeCalculated != null)
            {
                element.SizeCalculated = null;
            }

            if (element.Click != null)
            {
                element.Click = null;
            }

            if (element.DoubleClick != null)
            {
                element.DoubleClick = null;
            }

            if (element.MouseMove != null)
            {
                element.MouseMove = null;
            }

            if (element.MouseEnter != null)
            {
                element.MouseEnter = null;
            }

            if (element.MouseLeave != null)
            {
                element.MouseLeave = null;
            }

            if (element.MouseDown != null)
            {
                element.MouseDown = null;
            }

            if (element.KeyDown != null)
            {
                element.KeyDown = null;
            }

            if (element.KeyUp != null)
            {
                element.KeyUp = null;
            }

            if (element.KeyPress != null)
            {
                element.KeyPress = null;
            }

            if (element.QuietModeChanged != null)
            {
                element.QuietModeChanged = null;
            }
        }
        #endregion

        #region Catcher of Attributes value change

        /// <summary>
        /// Runs when attributes have been changed. In some cases, control must react on this.
        /// </summary>
        /// <param name="sender">Sender of changed event.</param>
        /// <param name="e">Additional parameters.</param>
        protected virtual void Attributes_Changed(object sender, BeforeValueChangedEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            if (e == null)
                throw new ArgumentNullException("e");

            if (this.Control.IsLoading) return;

            string typeName = sender.GetType().ToString();

            // Exit if unknown property has been changed.
            if (!this.Reaction.ContainsAttribute(typeName, e.Name)) return;

            // Values are identical.
            if (e.Values.NewValue.Equals(e.Values.OldValue)) return;

            ReactType reaction = this.Reaction.GetReaction(typeName, e.Name);
            IHTMLElement element = this;

            if (element == null) return;

            // React corresponding to reacting type.
            switch (reaction)
            {
                case ReactType.None:
                    return;

                case ReactType.ReCalculatingDocument:
                    ReCalculateDocument();
                    break;

                case ReactType.ReFormatMergingElement:
                    ReFormatMergeElement(element);
                    break;

                case ReactType.ReFormatMergElmAndReCalcDoc:
                    ReFormatMergElmAndReCalcDoc(element);
                    break;

                case ReactType.ReFormatCrtElmAndReCalcDoc:
                    ReFormatCrtElmAndReCalcDoc(element);
                    break;

                case ReactType.ReFormatCrtDocAndReCalcDoc:
                    ReFormatCrtDocAndReCalcDoc(element);
                    break;

                case ReactType.RePaintDocument:
                    RePaintDocument();
                    break;

                case ReactType.RePaintElement:
                    RePaintElement(element);
                    break;

                case ReactType.RePositionDocument:
                    RePositionDocument();
                    break;

                case ReactType.RePositionElement:
                    RePositionElement(element);
                    break;

                case ReactType.ReChildrenFormatMerge:
                    ChildrenFormatMergeAndRepaintDoc(element);
                    break;

                case ReactType.ReChildrenFormatMergeAndRecalcDoc:
                    ChildrenFormatMergeAndRecalcDoc(element);
                    break;

                case ReactType.ReFormatMergElmAndChildrenAndReCalcDoc:
                    ReFormatMergElmAndChildrenAndReCalcDoc(element);
                    break;
            }
        }

        /// <summary>
        /// Recalculates the document and repaints the control.
        /// </summary>
        protected internal virtual void ReCalculateDocument()
        {
            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.ReCalculateDoc;

                return;
            }

            this.Control.RecalculateDocument();
            RePaintDocument();
        }

        /// <summary>
        /// Re-merges the format for the element.
        /// </summary>
        /// <param name="element">Element for remerging formats.</param>
        protected internal virtual void ReFormatMergeElement(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.ReMergeFormats;

                return;
            }

            element.CalculateFormat();
            RePaintDocument();
        }

        /// <summary>
        /// Recalculates the format for the element and recalculates the current document.
        /// </summary>
        /// <param name="element">Element for which attribute has been changed.</param>
        protected internal virtual void ReFormatMergElmAndReCalcDoc(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.ReMergeFormats;
                this.Document.Reaction |= ReactLevel.ReCalculateDoc;

                return;
            }

            element.CalculateFormat();
            ReCalculateDocument();
        }

        /// <summary>
        /// Creates the format for the element and recalculates the current document.
        /// </summary>
        /// <param name="element">Element for reaction performing.</param>
        protected internal virtual void ReFormatCrtElmAndReCalcDoc(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.ReFormatsCreate;
                this.Document.Reaction |= ReactLevel.ReCalculateDoc;

                return;
            }

            this.Document.RecreateFormatElement(element);
            ReCalculateDocument();
        }

        /// <summary>
        /// Recreates the formats for the document and recalculates the current document.
        /// </summary>
        /// <param name="element">Element where event has been raised.</param>
        protected internal virtual void ReFormatCrtDocAndReCalcDoc(IHTMLElement element)
        {
            // CSS has been changed.
            IElementHasCss cssElm = element as IElementHasCss;
            if (cssElm != null)
            {
                IHTMLFormat[] formats = cssElm.CreatedFormats;
                InputHTML document = (element as BaseElement).Document;
                if (formats != null)
                {
                    for (int i = 0, len = formats.Length; i < len; i++)
                    {
                        IHTMLFormat format = formats[i];
                        document.Formats.Remove(format);
                    }
                }

                TokenStream stream = cssElm.GetCssStream();

                // If element can resolve link to CSS file then parse it.
                if (stream != null)
                {
                    XmlDocument css = HTMLUIControl.CSSParser.Parse(stream);
                    ElementHasCssEventArgs args = new ElementHasCssEventArgs(css, element as BaseElement);
                    document.Formats.Element_HasCss(this, args);
                    stream.Close();
                }
            }

            ReFormatCrtElmAndReCalcDoc(this.Document.Root);
        }

        /// <summary>
        /// Repaints the document.
        /// </summary>
        protected internal virtual void RePaintDocument()
        {
            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.RePaintdoc;

                return;
            }

            this.Control.Invalidate();
        }

        /// <summary>
        /// Repaints the element.
        /// </summary>
        /// <param name="element">Element for reaction performing.</param>
        protected internal virtual void RePaintElement(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (!this.Control.IsHandleCreated) return;

            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.RePaintdoc;

                return;
            }

            using (Graphics g = this.Control.CreateGraphics())
            using (PaintEventArgs args = new PaintEventArgs(g, this.Control.ClientRectangle))
            {
                element.DrawElement(args);
            }
        }

        /// <summary>
        /// Recalculates the position of the document.
        /// </summary>
        protected internal virtual void RePositionDocument()
        {
            if (this.Document.RenderRoot == null) return;

            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.ReCalculateDoc;

                return;
            }

            this.Document.RenderRoot.CalculatePosition();
            RePaintDocument();
        }

        /// <summary>
        /// Recalculates the position of the element.
        /// </summary>
        /// <param name="element">Element for reaction performing.</param>
        protected internal virtual void RePositionElement(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.ReCalculateDoc;

                return;
            }

            // TODO: Implement good position recalculating.
            // If element has block type, code below is not valid:
            element.CalculatePosition();
        }

        /// <summary>
        /// Re-merges the formats for all children of the element and repaints the document.
        /// </summary>
        /// <param name="element">Element for reaction performing.</param>
        protected internal virtual void ChildrenFormatMergeAndRepaintDoc(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            Queue queue = new Queue();
            queue.Enqueue(element);
            IHTMLElement tagElement = null;
            IHTMLElement child = null;

            while (queue.Count > 0)
            {
                tagElement = queue.Dequeue() as IHTMLElement;

                // All children of element must calculate format.
                if (tagElement != element)
                {
                    tagElement.CalculateFormat();
                }

                if (tagElement.Children.Count > 0)
                {
                    for (int i = 0, len = tagElement.Children.Count; i < len; i++)
                    {
                        child = tagElement.Children[i];
                        queue.Enqueue(child);
                    }
                }
            }

            RePaintDocument();
        }

        /// <summary>
        /// Re-merges the formats for all the children of the element.
        /// </summary>
        /// <param name="element">Element for reaction performing.</param>
        /// <param name="bMergeOfElement">If True - merges format of element.</param>
        protected internal virtual void ChildrenFormatMerge(IHTMLElement element, bool bMergeOfElement)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            Queue queue = new Queue();
            queue.Enqueue(element);
            IHTMLElement tagElement = null;
            IHTMLElement child = null;

            while (queue.Count > 0)
            {
                tagElement = queue.Dequeue() as IHTMLElement;

                // All children of element must calculate format.
                if (tagElement != element || bMergeOfElement)
                {
                    tagElement.CalculateFormat();
                }

                if (tagElement.Children.Count > 0)
                {
                    for (int i = 0, len = tagElement.Children.Count; i < len; i++)
                    {
                        child = tagElement.Children[i];
                        queue.Enqueue(child);
                    }
                }
            }
        }

        /// <summary>
        /// Re-merges all the children formats and recalculates the document.
        /// </summary>
        /// <param name="element">Element for reaction performing.</param>
        protected internal virtual void ChildrenFormatMergeAndRecalcDoc(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            ChildrenFormatMerge(element, false);

            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.ReCalculateDoc;

                return;
            }

            ReCalculateDocument();
        }

        /// <summary>
        /// Re-merges the format of element and its children and recalculates the document.
        /// </summary>
        /// <param name="element">Element for reaction performing.</param>
        protected internal virtual void ReFormatMergElmAndChildrenAndReCalcDoc(IHTMLElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            ChildrenFormatMerge(element, true);

            if (this.Document.QuietMode)
            {
                this.Document.Reaction |= ReactLevel.ReCalculateDoc;

                return;
            }

            ReCalculateDocument();
        }

        /// <summary>
        /// Raised when format of the element has been changed.
        /// </summary>
        /// <param name="element">Parent element of formats.</param>
        /// <param name="oldFormat">Old format.</param>
        /// <param name="newFormat">New format.</param>
        protected virtual void OnFormatChanged(IHTMLElement element, HTMLFormat oldFormat, HTMLFormat newFormat)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (oldFormat == null)
                throw new ArgumentNullException("oldFormat");

            if (newFormat == null)
                throw new ArgumentNullException("newFormat");

            bool bNeedRecalculate = false;

            bNeedRecalculate = oldFormat.Font.Size != newFormat.Font.Size;

            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.Font.FontFamily != newFormat.Font.FontFamily;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.Width != newFormat.Width;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.Height != newFormat.Height;

            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.WidthType != newFormat.WidthType;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.HeightType != newFormat.HeightType;

            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.HorizontalAlign != newFormat.HorizontalAlign;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.VerticalAlign != newFormat.VerticalAlign;

            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.Left.Width != newFormat.Left.Width;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.Top.Width != newFormat.Top.Width;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.Right.Width != newFormat.Right.Width;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.Bottom.Width != newFormat.Bottom.Width;

            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.PaddingLeft != newFormat.PaddingLeft;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.PaddingTop != newFormat.PaddingTop;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.PaddingRight != newFormat.PaddingRight;
            if (!bNeedRecalculate) bNeedRecalculate =
                                     oldFormat.PaddingBottom != newFormat.PaddingBottom;

            if (bNeedRecalculate)
            {
                ChildrenFormatMergeAndRecalcDoc(element);
            }
            else
            {
                ChildrenFormatMerge(element, false);
                RePaintElement(element);
            }
        }
        #endregion
    }
}