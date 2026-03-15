#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Xml;

using Syncfusion.Windows.Forms.HTMLUI.Implementation.Collections;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class which represents string in the block.
    /// </summary>
    public sealed class Text : ICloneable
    {
        #region Class members
        /// <summary>
        /// Text in the block.
        /// </summary>
        private string m_node;

        /// <summary>
        /// Indicates whether the content of this object is calculated for selecting.
        /// </summary>
        private bool m_bCalculated;

        /// <summary>
        /// Indicates the number from which the symbol of the element in this object starts.
        /// </summary>
        private int m_startNumber;

        /// <summary>
        /// Collection of symbols in the block.
        /// </summary>
        private SymbolCollection m_symbols;

        /// <summary>
        /// Parent block containing this text object.
        /// </summary>
        private Block m_parent;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the string text in the block.
        /// </summary>
        internal string Value
        {
            get
            {
                if (m_node == null)
                    return string.Empty;

                return m_node;
            }
            set
            {
                if (m_node != value)
                {
                    m_node = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the content of this object is calculated for selecting.
        /// </summary>
        internal bool IsCalculated
        {
            get
            {
                return m_bCalculated;
            }
            set
            {
                if (m_bCalculated != value)
                {
                    m_bCalculated = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the number from which the symbol of the element in this object starts.
        /// </summary>
        internal int StartNumber
        {
            get
            {
                return m_startNumber;
            }
            set
            {
                if (m_startNumber != value)
                {
                    m_startNumber = value;
                }
            }
        }

        /// <summary>
        /// Gets the collection of symbols in the text object.
        /// </summary>
        internal SymbolCollection Symbols
        {
            get
            {
                if (m_symbols == null)
                {
                    m_symbols = new SymbolCollection();
                }

                return m_symbols;
            }
        }

        /// <summary>
        /// Gets or sets the parent block containing this text object.
        /// </summary>
        internal Block Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                if (m_parent != value)
                {
                    m_parent = value;
                }
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Prevents a default instance of the Text class from being created
        /// </summary>
        private Text()
        {
            m_node = null;
        }

        /// <summary>
        /// Initializes a new instance of the Text class
        /// </summary>
        /// <param name="value">String text data.</param>
        internal Text(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            m_node = value;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        object ICloneable.Clone()
        {
            Text text = (Text)this.MemberwiseClone();
            text.IsCalculated = false;

            return text;
        }

        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public Text Clone()
        {
            return (Text)((ICloneable)this).Clone();
        }
        #endregion
    }
}
