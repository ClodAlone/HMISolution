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
using System.Drawing;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Collections;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class for containing elements used for drawing (Text elements or BaseElements).
    /// One object represents one line.
    /// </summary>
    public sealed class Block : IEnumerable
    {
        #region Class members
        /// <summary>
        /// Width of the block.
        /// </summary>
        private int m_width;

        /// <summary>
        /// Height of the block.
        /// </summary>
        private int m_height;

        /// <summary>
        /// Container of elements as keys and rectangles with coordinates as values.
        /// </summary>
        private Hashtable m_container;

        /// <summary>
        /// Reference to the element which contains this block.
        /// </summary>
        private BaseElement m_owner;

        /// <summary>
        /// Contains all keys (Needed for running through keys in order).
        /// </summary>
        private ArrayList m_keys;

        /// <summary>
        /// Block left position.
        /// </summary>
        private int m_posX;

        /// <summary>
        /// Block top position.
        /// </summary>
        private int m_posY;

        /// <summary>
        /// Indicates whether block was horizontally aligned.
        /// </summary>
        private bool m_hShifted;

        /// <summary>
        /// Indicates whether block was vertically aligned.
        /// </summary>
        private bool m_vShifted;

        /// <summary>
        /// Indicates whether to draw borders, backgrounds, etc. for this block.
        /// </summary>
        private bool m_drawProperties;

        /// <summary>
        /// Indicates whether block X coordinate is hard defined.
        /// </summary>
        private bool m_isFixedX;

        /// <summary>
        /// Indicates whether block Y coordinate is hard defined.
        /// </summary>
        private bool m_isFixedY;

        /// <summary>
        /// Indicates whether current block is main for its owner.
        /// </summary>
        private bool m_isMain;

        /// <summary>
        /// Final width of the block. Must be defined after block is calculated.
        /// </summary>
        private int m_finalWidth;

        /// <summary>
        /// Final height of the block. Must be defined after block is calculated.
        /// </summary>
        private int m_finalHeight;

        /// <summary>
        /// Rectangle of the block.
        /// </summary>
        private Rectangle m_rect;

        /// <summary>
        /// Indicates the quiet mode for events.
        /// </summary>
        private bool m_bQuietMode;

        /// <summary>
        /// Indicates whether block was drawn at first.
        /// </summary>
        private bool m_bWasDrawn;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets the bounds of the block.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return this.Rectangle;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the block is main for its owner element which has a block structure.
        /// </summary>
        /// <remarks>Only elements with block structure have main block containing whole element.
        /// Inline elements do not have main blocks.</remarks>
        public bool IsMain
        {
            get
            {
                return this.Owner.IsBlock ? m_isMain : false;
            }
            set
            {
                if (m_isMain != value)
                {
                    m_isMain = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the block.
        /// </summary>
        internal int Width
        {
            get
            {
                //// if( m_width != -1 ) return m_width;

                //// Calculate width of all elements.
                int width = 0;
                object element = null;

                for (int i = 0, len = m_keys.Count; i < len; i++)
                {
                    element = m_keys[i];

                    if (element is Block)
                    {
                        if (this.IsMain)
                            width = Math.Max(width, (element as Block).FinalWidth);
                        else
                            width += (element as Block).FinalWidth;
                    }
                    else
                    {
                        width += this[element].Width;
                    }
                }

                return width;
            }
            set
            {
                if (m_width != value)
                {
                    m_width = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the final width of the block (depends of style width of the element).
        /// </summary>
        internal int FinalWidth
        {
            get
            {
                if (m_finalWidth != -1) return m_finalWidth;

                if (m_width != -1)
                {
                    return Math.Max(this.Width, m_width);
                }

                return this.Width;
            }
            set
            {
                if (m_finalWidth != value && value > 0)
                {
                    m_finalWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the final height of the block (depends of style width of the element).
        /// </summary>
        internal int FinalHeight
        {
            get
            {
                if (m_finalHeight != -1) return m_finalHeight;

                if (m_height != -1)
                {
                    return Math.Max(this.Height, m_height);
                }

                return this.Height;
            }
            set
            {
                if (m_finalHeight != value)
                {
                    m_finalHeight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the block.
        /// </summary>
        internal int Height
        {
            get
            {
                //// if( m_height != -1 ) return m_height;

                int height = 0;
                object element = null;

                for (int i = 0, len = m_keys.Count; i < len; i++)
                {
                    element = m_keys[i];
                    if (element is Block)
                    {
                        if (this.IsMain)
                        {
                            height += (element as Block).FinalHeight;
                        }
                        else
                        {
                            height = Math.Max(height, (element as Block).FinalHeight);
                        }
                    }
                    else
                    {
                        height = Math.Max(height, this[element].Height);
                    }
                }

                return height;
            }
            set
            {
                if (m_height != value)
                {
                    m_height = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the hash that contains elements for drawing.
        /// </summary>
        internal Hashtable Containter
        {
            get
            {
                if (m_container == null) m_container = new Hashtable();

                return m_container;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Container");

                if (m_container != value)
                {
                    m_container = value;
                }
            }
        }

        /// <summary>
        /// Gets the first element in the block.
        /// </summary>
        internal object First
        {
            get
            {
                return (m_keys.Count == 0) ? null : m_keys[0];
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the location block.
        /// </summary>
        internal int X
        {
            get
            {
                if (m_isFixedX || m_keys.Count == 0) return m_posX;

                return this[this.First].X;
            }
            set
            {
                if (m_posX != value)
                {
                    m_posX = value;
                }

                m_isFixedX = true;

                OnLocationChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of location block.
        /// </summary>
        internal int Y
        {
            get
            {
                if (m_isFixedY || m_keys.Count == 0) return m_posY;

                return this[this.First].Y;
            }
            set
            {
                if (m_posY != value)
                {
                    m_posY = value;
                }

                m_isFixedY = true;

                OnLocationChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the tag element which contains this block.
        /// </summary>
        internal BaseElement Owner
        {
            get
            {
                return m_owner;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the fixed left position is defined.
        /// </summary>
        internal bool IsFixedLeftPos
        {
            get
            {
                return m_isFixedX;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the fixed top position is defined.
        /// </summary>
        internal bool IsFixedTopPos
        {
            get
            {
                return m_posY != -1;
            }
        }

        /// <summary>
        /// Gets or sets the values to the container.
        /// (Text or blocks as keys and rectangle as values).
        /// </summary>
        /// <param name="key">Any object representing key value</param>
        internal Rectangle this[object key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                object rc = m_container[key];
                return rc == null ? Rectangle.Empty : (Rectangle)rc;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                if (!m_keys.Contains(key))
                {
                    m_keys.Add(key);
                }

                m_container[key] = value;
            }
        }

        /// <summary>
        /// Returns the object in the block with the specified index.
        /// </summary>
        /// <param name="index">Interger index value</param>
        internal object this[int index]
        {
            get
            {
                if (index < 0 || index >= m_keys.Count) return null;

                return m_keys[index];
            }
        }

        /// <summary>
        /// Gets a value indicating whether the block is empty.
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                return m_keys.Count == 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the block was horizontally shifted (aligned).
        /// </summary>
        internal bool IsHShifted
        {
            get
            {
                return m_hShifted;
            }
            set
            {
                if (m_hShifted != value)
                {
                    m_hShifted = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the block was vertically shifted (aligned).
        /// </summary>
        internal bool IsVShifted
        {
            get
            {
                return m_vShifted;
            }
            set
            {
                if (m_vShifted != value)
                {
                    m_vShifted = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the bounds of rectangle for the block.
        /// </summary>
        internal Rectangle Rectangle
        {
            get
            {
                if (m_rect.IsEmpty)
                {
                    m_rect = new Rectangle(this.X, this.Y, this.FinalWidth, this.FinalHeight);
                }

                return m_rect;
            }
            set
            {
                m_rect = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we can draw borders, backgrounds, etc. for this block.
        /// </summary>
        internal bool DrawProperties
        {
            get
            {
                return m_drawProperties;
            }
            set
            {
                if (m_drawProperties != value)
                {
                    m_drawProperties = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of elements in the block.
        /// </summary>
        internal int Count
        {
            get
            {
                return m_container.Count;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the quiet mode for the block object.
        /// </summary>
        internal bool QuietMode
        {
            get
            {
                return m_bQuietMode;
            }
            set
            {
                if (m_bQuietMode != value)
                {
                    m_bQuietMode = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the block was drawn.
        /// </summary>
        internal bool WasDrawn
        {
            get
            {
                return m_bWasDrawn;
            }
            set
            {
                if (m_bWasDrawn != value)
                {
                    m_bWasDrawn = true;
                }
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Event. Raised when location of block is changed.
        /// </summary>
        internal event EventHandler LocationChanged;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the Block class from being created
        /// </summary>
        private Block()
        {
            m_width = -1;
            m_height = -1;
            m_finalWidth = -1;
            m_finalHeight = -1;

            m_container = new Hashtable();
            m_keys = new ArrayList();

            m_posX = -1;
            m_posY = -1;

            m_drawProperties = true;
            //// m_bQuietMode      = True;
        }

        /// <summary>
        /// Initializes a new instance of the Block class
        /// </summary>
        /// <param name="owner">Owner of the block.</param>
        internal Block(BaseElement owner)
            : this()
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            m_owner = owner;
            m_posX = m_owner.CurrentPosition.X;
            m_posY = m_owner.CurrentPosition.Y;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Adds an element to the block.
        /// </summary>
        /// <param name="key">Key for adding to the block.</param>
        /// <param name="value">Value for adding to the block.</param>
        internal void Add(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (value == null)
                throw new ArgumentNullException("value");

            if (!(key is Text) && !(key is Block) && !(key is IHTMLElement))
                throw new ArgumentException("Key has invalid type!");

            if (!(value is Rectangle))
                throw new ArgumentException("Value has invalid type!");

            // add element and it's rectangle to inner container
            this[key] = (Rectangle)value;
        }

        /// <summary>
        /// Inserts element to the specified position.
        /// </summary>
        /// <param name="index">Index where the insert is to occur.</param>
        /// <param name="key">Key for adding to the block.</param>
        /// <param name="value">Value for adding to the block.</param>
        internal void Insert(int index, object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (value == null)
                throw new ArgumentNullException("value");

            if (!(key is Text) && !(key is Block) && !(key is IHTMLElement))
                throw new ArgumentException("Key has invalid type!");

            if (!(value is Rectangle))
                throw new ArgumentException("Value has invalid type!");

            if (index < 0 || index > m_keys.Count)
                throw new ArgumentOutOfRangeException("index", "index is out of range to insert");

            m_keys.Insert(index, key);
            m_container[key] = value;
        }

        /// <summary>
        /// Clears all content of the block.
        /// </summary>
        internal void Clear()
        {
            m_keys.Clear();
            m_container.Clear();
        }

        /// <summary>
        /// Removes element from the block.
        /// </summary>
        /// <param name="key">Key by which element will be removed from the collection.</param>
        internal void Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            this.Containter.Remove(key);
        }

        /// <summary>
        /// Indicates whether the key exists in the hashtable.
        /// </summary>
        /// <param name="key">Key by which element will be searched in the hashtable.</param>
        /// <returns>True if element exists; False otherwise.</returns>
        internal bool Contains(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return this.Containter.ContainsKey(key);
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raises LocationChanged event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        private void RaiseLocationChanged(EventArgs args)
        {
            if (LocationChanged != null && !this.QuietMode)
            {
                LocationChanged(this, args);
            }
        }

        /// <summary>
        /// Raises LocationChanged event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        private void OnLocationChanged(EventArgs args)
        {
            RaiseLocationChanged(args);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Clears event handlers for this block.
        /// </summary>
        internal void ClearEvents()
        {
            this.LocationChanged = null;
        }

        /// <summary>
        /// Raises LocationChanged event.
        /// </summary>
        internal void RaiseEvents()
        {
            if (!this.WasDrawn)
            {
                this.QuietMode = false;
                OnLocationChanged(EventArgs.Empty);
                this.QuietMode = true;
                this.WasDrawn = true;
            }
        }

        /// <summary>
        /// Resets and renews cached Rectangle value.
        /// </summary>
        internal void ResetRectCache()
        {
            m_rect = Rectangle.Empty;
            m_rect = this.Rectangle;
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns the enumerator for this object.
        /// </summary>
        /// <returns>Enumerator object.</returns>
        public IEnumerator GetEnumerator()
        {
            return m_keys.GetEnumerator();
        }
        #endregion

    }
}