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
using System.Windows.Forms;
using System.Xml;
using Syncfusion.HTMLUI.Base;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Message class used for sending to user the old and new values of
    /// property. It simply gives access to the new and old values and does
    /// not allow user to change them.
    /// </summary>
    [DebuggerStepThrough]
    public class ValueChangedEventArgs : EventArgs
    {
        #region Class constants
        /// <summary>
        /// Instance of the class which must be used as Empty analog for this class.
        /// </summary>
        private static readonly ValueChangedEventArgs _empty = new ValueChangedEventArgs();
        #endregion

        #region Class Static Properties
        /// <summary>
        /// Gets an instance of the class which is detected as an empty / NULL value.
        /// </summary>
        public static new ValueChangedEventArgs Empty
        {
            [DebuggerStepThrough]
            get
            {
                return _empty;
            }
        }
        #endregion

        #region ValueChangedEventArgs Class members
        /// <summary>
        /// Storage of old value.
        /// </summary>
        private object m_old;

        /// <summary>
        /// Storage of new value.
        /// </summary>
        private object m_new;
        #endregion

        #region ValueChangedEventArgs Class Properties
        /// <summary>
        /// Gets the newly set value.
        /// </summary>
        public object NewValue
        {
            [DebuggerStepThrough]
            get
            {
                return m_new;
            }
        }

        /// <summary>
        /// Gets the old value that was replaced by a new one.
        /// </summary>
        public object OldValue
        {
            [DebuggerStepThrough]
            get
            {
                return m_old;
            }
        }
        #endregion

        #region ValueChangedEventArgs Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the ValueChangedEventArgs class from being created
        /// </summary>
        private ValueChangedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ValueChangedEventArgs class
        /// </summary>
        /// <param name="old">Old value</param>
        /// <param name="newValue">new value</param>
        [DebuggerStepThrough]
        public ValueChangedEventArgs(object old, object newValue)
        {
            m_old = old;
            m_new = newValue;
        }
        #endregion
    }

    /// <summary>
    /// Message class which can be sent to the user before property
    /// value changes. On sending the class, the property must not be changed.
    /// </summary>
    [DebuggerStepThrough]
    public class BeforeValueChangedEventArgs : EventArgs
    {
        #region Class constants
        /// <summary>
        /// Empty instance of the class.
        /// </summary>
        private static readonly BeforeValueChangedEventArgs _empty = new BeforeValueChangedEventArgs();
        #endregion

        #region Class Static Properties
        /// <summary>
        /// Gets an empty instance of the class.
        /// </summary>
        public static new BeforeValueChangedEventArgs Empty
        {
            [DebuggerStepThrough]
            get
            {
                return _empty;
            }
        }
        #endregion

        #region BeforeValueChangedEventArgs Class members
        /// <summary>
        /// Name of property which will be changed soon.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Storage of new and old values.
        /// </summary>
        private ValueChangedEventArgs m_values;
        #endregion

        #region BeforeValueChangedEventArgs Class properties
        /// <summary>
        /// Gets the name of the property which will be changed.
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough]
            get
            {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the old and new value. Old value of property is still not replaced by
        /// message class sender.
        /// </summary>
        public ValueChangedEventArgs Values
        {
            [DebuggerStepThrough]
            get
            {
                return m_values;
            }
        }
        #endregion

        #region BeforeValueChangedEventArgs class initialize methods
        /// <summary>
        /// Prevents a default instance of the BeforeValueChangedEventArgs class from being created
        /// </summary>
        private BeforeValueChangedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BeforeValueChangedEventArgs class
        /// </summary>
        /// <param name="name">string value</param>
        /// <param name="args">ValueChangedEventArgs instance</param>
        [DebuggerStepThrough]
        public BeforeValueChangedEventArgs(string name, ValueChangedEventArgs args)
        {
            m_name = name;
            m_values = args;
        }
        #endregion
    }

    /// <summary>
    /// Class which is used for sending messages between the collection and the user.
    /// </summary>
    [DebuggerStepThrough]
    public class CollectionEventArgs : EventArgs
    {
        #region Class constants
        /// <summary>
        /// Empty instance of the class.
        /// </summary>
        private static readonly CollectionEventArgs _empty = new CollectionEventArgs();
        #endregion

        #region Class Static Properties
        /// <summary>
        /// Gets an empty instance of the class.
        /// </summary>
        public static new CollectionEventArgs Empty
        {
            [DebuggerStepThrough]
            get
            {
                return _empty;
            }
        }
        #endregion

        #region CollectionEventArgs Class members
        /// <summary>
        /// Storage of the Cancel property.
        /// </summary>
        private bool m_bCancel;

        /// <summary>
        /// Storage of the Index property.
        /// </summary>
        private int m_iIndex = -1;

        /// <summary>
        /// Storage of the Value property.
        /// </summary>
        private object m_value;

        /// <summary>
        /// Storage of the OldValue property.
        /// </summary>
        private object m_oldValue;
        #endregion

        #region CollectionEventArgs Class Properties
        /// <summary>
        /// Gets or sets a value indicating whether class will skip call to base Collection method; False indicates that the
        /// CollectionBase's class override method will be called.
        /// </summary>
        public bool Cancel
        {
            [DebuggerStepThrough]
            get
            {
                return m_bCancel;
            }
            [DebuggerStepThrough]
            set
            {
                m_bCancel = value;
            }
        }

        /// <summary>
        /// Gets the index of the item.
        /// </summary>
        public int Index
        {
            [DebuggerStepThrough]
            get
            {
                return m_iIndex;
            }
        }

        /// <summary>
        /// Gets the value of the item.
        /// </summary>
        public object Value
        {
            [DebuggerStepThrough]
            get
            {
                return m_value;
            }
        }

        /// <summary>
        /// Gets the old value of the property.
        /// </summary>
        public object OldValue
        {
            [DebuggerStepThrough]
            get
            {
                return m_oldValue;
            }
        }
        #endregion

        #region CollectionEventArgs Class constructors
        /// <summary>
        /// Prevents a default instance of the CollectionEventArgs class from being created
        /// </summary>
        private CollectionEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CollectionEventArgs class
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <param name="value">Value - reference to the collection item.</param>
        [DebuggerStepThrough]
        public CollectionEventArgs(int index, object value)
        {
            m_iIndex = index;
            m_value = value;
        }

        /// <summary>
        /// Initializes a new instance of the CollectionEventArgs class
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <param name="value">Value - reference to the collection item.</param>
        /// <param name="old">Old value of the item.</param>
        [DebuggerStepThrough]
        public CollectionEventArgs(int index, object value, object old)
        {
            m_iIndex = index;
            m_value = value;
            m_oldValue = old;
        }
        #endregion
    }

    /// <summary>
    /// Message / Data sender class. Class allows user to cancel action in which it was used
    /// as message sender class.
    /// </summary>
    [DebuggerStepThrough]
    public class DictionaryEventArgs : EventArgs
    {
        #region Class constants
        /// <summary>
        /// Empty instance of the class.
        /// </summary>
        private static readonly DictionaryEventArgs _empty = new DictionaryEventArgs();
        #endregion

        #region Class Static Properties
        /// <summary>
        /// Gets an empty instance of the class.
        /// </summary>
        public static new DictionaryEventArgs Empty
        {
            [DebuggerStepThrough]
            get
            {
                return _empty;
            }
        }
        #endregion

        #region DictionaryEventArgs Class members
        /// <summary>
        /// Indicates whether to cancel the current action.
        /// </summary>
        private bool m_bCancel;

        /// <summary>
        /// Key value of the dictionary item.
        /// </summary>
        private object m_key;

        /// <summary>
        /// Dictionary item.
        /// </summary>
        private object m_value;

        /// <summary>
        /// On Replace, get the dictionary item which will be replaced.
        /// </summary>
        private object m_oldValue;
        #endregion

        #region DictionaryEventArgs Class Properties
        /// <summary>
        /// Gets or sets a value indicating whether to cancel the current action.
        /// </summary>
        public bool Cancel
        {
            [DebuggerStepThrough]
            get
            {
                return m_bCancel;
            }
            [DebuggerStepThrough]
            set
            {
                m_bCancel = value;
            }
        }

        /// <summary>
        /// Gets the key value of the dictionary item.
        /// </summary>
        public object Key
        {
            [DebuggerStepThrough]
            get
            {
                return m_key;
            }
        }

        /// <summary>
        /// Gets the dictionary item.
        /// </summary>
        public object Value
        {
            [DebuggerStepThrough]
            get
            {
                return m_value;
            }
        }

        /// <summary>
        /// Gets the dictionary item which will be replaced.
        /// </summary>
        public object OldValue
        {
            [DebuggerStepThrough]
            get
            {
                return m_oldValue;
            }
        }
        #endregion

        #region DictionaryEventArgs Class constructors
        /// <summary>
        /// Prevents a default instance of the DictionaryEventArgs class from being created
        /// </summary>
        private DictionaryEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DictionaryEventArgs class
        /// </summary>
        /// <param name="key">Key of object.</param>
        /// <param name="value">Value of object.</param>
        public DictionaryEventArgs(object key, object value)
        {
            m_key = key;
            m_value = value;
        }

        /// <summary>
        /// Initializes a new instance of the DictionaryEventArgs class
        /// </summary>
        /// <param name="key">Key of object.</param>
        /// <param name="value">Value of object.</param>
        /// <param name="old">Old value of the object.</param>
        public DictionaryEventArgs(object key, object value, object old)
            : this(key, value)
        {
            m_oldValue = old;
        }
        #endregion
    }

    /// <summary>
    /// Event Arguments is used to delegate XML document with CSS data.
    /// </summary>
    [DebuggerStepThrough]
    public class ElementHasCssEventArgs : EventArgs
    {
        #region Class constants
        /// <summary>
        /// An instance of the class which must be used as an empty analog for this class.
        /// </summary>
        private static readonly ElementHasCssEventArgs _empty = new ElementHasCssEventArgs();
        #endregion

        #region Class Static Properties
        /// <summary>
        /// Gets an instance of class which is detected as empty / NULL value.
        /// </summary>
        public static new ElementHasCssEventArgs Empty
        {
            [DebuggerStepThrough]
            get
            {
                return _empty;
            }
        }
        #endregion

        #region Class members
        /// <summary>
        /// Writes XHTML document.
        /// </summary>
        private XmlDocument m_document;

        /// <summary>
        /// HTML Element which has CSS.
        /// </summary>
        private BaseElement m_element;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets the old value that is replaced by a new one.
        /// </summary>
        public XmlDocument XmlStorge
        {
            [DebuggerStepThrough]
            get
            {
                return m_document;
            }
        }

        /// <summary>
        /// Gets the HTML Element which contains CSS.
        /// </summary>
        public BaseElement Element
        {
            [DebuggerStepThrough]
            get
            {
                return m_element;
            }
        }
        #endregion

        #region  Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the ElementHasCssEventArgs class from being created
        /// </summary>
        private ElementHasCssEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ElementHasCssEventArgs class
        /// </summary>
        /// <param name="document">XmlDocument instance</param>
        /// <param name="element">BaseElement instance</param>
        [DebuggerStepThrough]
        public ElementHasCssEventArgs(XmlDocument document, BaseElement element)
        {
            m_document = document;
            m_element = element;
        }
        #endregion
    }

    /// <summary>
    /// Event Arguments for Tag elements. We can set flag delegate event to parent.
    /// </summary>
    public class BubblingEventArgs : EventArgs
    {
        #region Class members
        /// <summary>
        /// Indicates whether to delegate event to other elements.
        /// </summary>
        private bool m_bBubbling;

        /// <summary>
        /// Standard event arguments.
        /// </summary>
        private EventArgs m_inherit;

        /// <summary>
        /// Holds the first sender of the event.
        /// </summary>
        private object m_firstSender;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether to delegate event to other elements.
        /// </summary>
        public bool Bubbling
        {
            get
            {
                return m_bBubbling;
            }
            set
            {
                m_bBubbling = value;
            }
        }

        /// <summary>
        /// Gets the standard event arguments.
        /// </summary>
        public EventArgs EventArgs
        {
            get
            {
                return m_inherit;
            }
        }

        /// <summary>
        /// Gets the first sender of the event.
        /// </summary>
        public object RootSender
        {
            get
            {
                return m_firstSender;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the BubblingEventArgs class
        /// </summary>
        /// <param name="firstSender">Source of the event</param>
        /// <param name="inherit">Event arguments.</param>
        public BubblingEventArgs(object firstSender, EventArgs inherit)
        {
            if (firstSender == null)
                throw new ArgumentNullException("firstSender");

            m_firstSender = firstSender;
            m_inherit = inherit;
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for Tag elements. We can set flag delegate event to parent.
    /// </summary>
    public class NoImageEventArgs : EventArgs
    {
        #region Class members
        /// <summary>
        /// Stream for image.
        /// </summary>
        private Stream m_stream;

        /// <summary>
        /// Key for image cache.
        /// </summary>
        private string m_key;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the image stream.
        /// </summary>
        public Stream Stream
        {
            get
            {
                return m_stream;
            }
            set
            {
                if (m_stream != value)
                {
                    m_stream = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique key for image cache.
        /// If key is NULL, image will not be inserted into the cache.
        /// </summary>
        public string Key
        {
            get
            {
                return m_key;
            }
            set
            {
                if (m_key != value)
                {
                    m_key = value;
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the NoImageEventArgs class
        /// </summary>
        public NoImageEventArgs()
        {
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for Tag elements. We can set flag delegate event to parent.
    /// </summary>
    public class PreRenderDocumentArgs : EventArgs
    {
        #region Class members
        /// <summary>
        /// New document which was just created.
        /// </summary>
        private InputHTML m_document;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the image stream.
        /// </summary>
        public InputHTML Document
        {
            get
            {
                return m_document;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the PreRenderDocumentArgs class
        /// </summary>
        /// <param name="document">Document which is going to be rendered.</param>
        public PreRenderDocumentArgs(InputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            m_document = document;
        }
        #endregion
    }

    /// <summary>
    /// Event argument for user control tag elements.
    /// </summary>
    public class EnhanceEventsEventArgs : EventArgs
    {
        /// <summary>
        /// Event instance.
        /// </summary>
        private IHTMLEvent m_event;

        /// <summary>
        /// Name of the event.
        /// </summary>
        private string m_strName;

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string Name
        {
            get
            {
                return m_strName;
            }
        }

        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        public IHTMLEvent Event
        {
            get
            {
                return m_event;
            }
            set
            {
                m_event = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the EnhanceEventsEventArgs class
        /// </summary>
        /// <param name="name">Name of the event.</param>
        public EnhanceEventsEventArgs(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            m_strName = name;
        }
    }

    /// <summary>
    /// Arguments for event before the format of element is calculated.
    /// </summary>
    public class PreStyleCalculatedEventArgs : EventArgs
    {
        #region Class members
        /// <summary>
        /// Hashtable of styles for merging.
        /// </summary>
        private ArrayList m_styles;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the collection of IHTMLFormat objects from which the final style of element
        /// would be calculated. Significance of styles raises from the beginning to the end.
        /// </summary>
        public ArrayList Styles
        {
            get
            {
                return m_styles;
            }
        }

        /// <summary>
        /// Returns the IHTMLFormat object from collection by its index.
        /// </summary>
        /// <param name="index">index value</param>
        public IHTMLFormat this[int index]
        {
            get
            {
                if (index < 0 || index >= m_styles.Count)
                    throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0 and greater of collection size - 1.");

                return m_styles[index] as IHTMLFormat;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the PreStyleCalculatedEventArgs class from being created
        /// </summary>
        private PreStyleCalculatedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PreStyleCalculatedEventArgs class
        /// </summary>
        /// <param name="styles">Collection of IHTMLFormat objects.</param>
        public PreStyleCalculatedEventArgs(ArrayList styles)
            : this()
        {
            if (styles == null)
                throw new ArgumentNullException("styles");

            m_styles = styles;
        }
        #endregion
    }

    /// <summary>
    /// Class of arguments for raising OnPaint event of the HTML element.
    /// </summary>
    public class ElementPaintEventArgs : EventArgs
    {
        #region Class members
        /// <summary>
        /// Paint arguments for drawing.
        /// </summary>
        private PaintEventArgs m_args;

        /// <summary>
        /// Block for painting.
        /// </summary>
        private Block m_block;

        /// <summary>
        /// Indicates whether painting should be cancelled.
        /// </summary>
        private bool m_bCancel;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the block of element which is to be painted.
        /// </summary>
        public Block PaintingBlock
        {
            get
            {
                return m_block;
            }
        }

        /// <summary>
        /// Gets the graphics context of the element for drawing.
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                return m_args.Graphics;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether painting of the block should be cancelled.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return m_bCancel;
            }
            set
            {
                if (m_bCancel != value)
                {
                    m_bCancel = value;
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the ElementPaintEventArgs class from being created
        /// </summary>
        private ElementPaintEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ElementPaintEventArgs class
        /// </summary>
        /// <param name="e">Event arguments for drawing.</param>
        /// <param name="block">Block which is going to be drawn.</param>
        public ElementPaintEventArgs(PaintEventArgs e, Block block)
            : this()
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (block == null)
                throw new ArgumentNullException("block");

            m_args = e;
            m_block = block;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Invokes default drawing of the element by control.
        /// </summary>
        public void DrawDefault()
        {
            m_block.Owner.ProcessDrawBlock(m_block, m_args);
        }
        #endregion
    }

    /// <summary>
    /// Class corresponding to arguments in a document that renders error.
    /// </summary>
    public class LoadErrorEventArgs : EventArgs
    {
        #region Class members
        /// <summary>
        /// Document object.
        /// </summary>
        private IInputHTML m_document;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the document on which rendering error has occurred.
        /// </summary>
        /// <remarks>For more details, see RenderException property of document.</remarks>
        public IInputHTML Document
        {
            get
            {
                return m_document;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the LoadErrorEventArgs class from being created
        /// </summary>
        private LoadErrorEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LoadErrorEventArgs class
        /// </summary>
        /// <param name="document">Document on which rendering error has occurred.</param>
        public LoadErrorEventArgs(IInputHTML document)
            : this()
        {
            if (document == null)
                throw new ArgumentNullException("document");

            m_document = document;
        }
        #endregion
    }

    /// <summary>
    /// Class corresponding to 'Find' event in FindDialog class.
    /// </summary>
    public class FindTextEventArgs : EventArgs
    {
        #region Class members
        /// <summary>
        /// Text for searching.
        /// </summary>
        private string m_text;

        /// <summary>
        /// Start index for text searching.
        /// </summary>
        private int m_index;

        /// <summary>
        /// Indicates whether searching is forward.
        /// </summary>
        private bool m_bForward;

        /// <summary>
        /// Indicates whether search should match case.
        /// </summary>
        private bool m_bMatchCase;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the text for searching.
        /// </summary>
        public string Text
        {
            [DebuggerStepThrough()]
            get
            {
                return m_text;
            }
        }

        /// <summary>
        /// Gets or sets the index from which searching starts.
        /// </summary>
        public int StartIndex
        {
            [DebuggerStepThrough()]
            get
            {
                return m_index;
            }
            [DebuggerStepThrough()]
            set
            {
                if (m_index != value)
                {
                    m_index = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the order of searching is forward.
        /// </summary>
        public bool IsForward
        {
            [DebuggerStepThrough()]
            get
            {
                return m_bForward;
            }
        }

        /// <summary>
        /// Gets a value indicating whether search should match case.
        /// </summary>
        public bool MatchCase
        {
            [DebuggerStepThrough()]
            get
            {
                return m_bMatchCase;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the FindTextEventArgs class
        /// </summary>
        /// <param name="text">String text for searching.</param>
        /// <param name="startIndex">Index from which to start text searching.</param>
        /// <param name="isForward">Indicates whether order of searching is forward.</param>
        /// <param name="matchCase">Indicates whether search should match case.</param>
        public FindTextEventArgs(string text, int startIndex, bool isForward, bool matchCase)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            m_text = text;
            m_index = startIndex;
            m_bForward = isForward;
            m_bMatchCase = matchCase;
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for event raised by
    /// element.
    /// </summary>
    public class LinkForwardEventArgs : EventArgs
    {
        #region Class members
        /// <summary>
        /// Path to the resource.
        /// </summary>
        private string m_path;

        /// <summary>
        /// Indicates whether default processing should be cancelled.
        /// </summary>
        private bool m_bCancel;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the path to the resource that is to be loaded.
        /// </summary>
        public string Path
        {
            [DebuggerStepThrough()]
            get
            {
                return m_path;
            }
            [DebuggerStepThrough()]
            set
            {
                if (m_path != value)
                {
                    m_path = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether default processing of resource loading should
        /// be cancelled.
        /// </summary>
        public bool Cancel
        {
            [DebuggerStepThrough()]
            get
            {
                return m_bCancel;
            }
            [DebuggerStepThrough()]
            set
            {
                if (m_bCancel != value)
                {
                    m_bCancel = value;
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the LinkForwardEventArgs class
        /// </summary>
        /// <param name="path">Path to the resource.</param>
        public LinkForwardEventArgs(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            m_path = path;
        }
        #endregion
    }

    #region delegates declaration
    /// <summary>
    /// Delegate used before value is changed.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">BeforeValueChangedEventArgs instance</param>
    public delegate void BeforeValueChangeEventHandler(object sender, BeforeValueChangedEventArgs e);

    /// <summary>
    /// Delegate used after value is changed.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">ValueChangedEventArgs instance</param>
    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

    /// <summary>
    /// Delegate used by collection to send messages to user throwing events.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">CollectionEventArgs instance</param>
    public delegate void CollectionEventHandler(object sender, CollectionEventArgs e);

    /// <summary>
    /// Delegate which is used for throwing events.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">DictionaryEventArgs instance</param>
    public delegate void DictionaryEventHandler(object sender, DictionaryEventArgs e);

    /// <summary>
    /// Delegate which is used for throwing events.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">ElementHasCssEventArgs instance</param>
    public delegate void HasCssEventHandler(object sender, ElementHasCssEventArgs e);

    /// <summary>
    /// Delegate which is used for creating special events in user controls.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">EnhanceEventsEventArgs instance</param>
    public delegate void EnhanceEventsEventHandler(object sender, EnhanceEventsEventArgs e);

    /// <summary>
    /// Delegate which is used for defining image by user when image by src
    /// attribute could not be found.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">NoImageEventArgs instance</param>
    public delegate void NoImageEventHandler(object sender, NoImageEventArgs e);

    /// <summary>
    /// Delegate which is used for event raising when document has been created but
    /// its size and location of elements are not yet.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">PreRenderDocumentArgs instance</param>
    public delegate void PreRenderDocumentEventHandler(object sender, PreRenderDocumentArgs e);

    /// <summary>
    /// Delegate which is used for event raising just before formats merge for the
    /// element.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">PreStyleCalculatedEventArgs instance</param>
    public delegate void PreStyleCalculatedEventHandler(object sender, PreStyleCalculatedEventArgs e);

    /// <summary>
    /// Delegate which is used for invoking events before the HTML element's block painting.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">ElementPaintEventArgs instance</param>
    public delegate void ElementPaintEventHandler(object sender, ElementPaintEventArgs e);

    /// <summary>
    /// Delegate which is used for raising an event if exception occurs while rendering a document.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e"> LoadErrorEventArgs instance</param>
    public delegate void LoadErrorEventHandler(object sender, LoadErrorEventArgs e);

    /// <summary>
    /// Delegate which is used for raising 'the FindText' event by FindDialog.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">FindTextEventArgs instance</param>
    public delegate void FindTextEventHandler(object sender, FindTextEventArgs e);

    /// <summary>
    /// Delegate which is used for raising
    /// element.
    /// </summary>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">LinkForwardEventArgs instance</param>
    public delegate void LinkForwardEventHandler(object sender, LinkForwardEventArgs e);
    #endregion
}