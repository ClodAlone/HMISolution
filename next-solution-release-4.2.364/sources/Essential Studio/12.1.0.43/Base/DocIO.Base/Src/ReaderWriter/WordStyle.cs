#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.DLS;

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for WordStyle.
    /// </summary>
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordStyle
    {
        #region Class members
        /// <summary>
        /// Style name
        /// </summary>
        private string m_strName;

        private readonly WordStyleSheet m_styleSheet;
        private int m_baseStyleIndex = 4095;
        private int m_nextStyleIndex = 4095;
        private int m_linkStyleIndex = 0;
        /// <summary>
        /// Style id
        /// </summary>
        private int m_id = -1;

        /// <summary>
        /// 
        /// </summary>
        internal static readonly WordStyle Empty = new WordStyle();

        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsCharacter = false;
        private CharacterProperties m_charProps = null;
        private ParagraphProperties m_paragProps = null;
        private bool m_hasUpe;
        private bool m_isPrimary;
        private bool m_isSemiHidden;
        private bool m_unhideWhenUsed;
        private WordStyleType m_typeCode;
        private byte[] m_tapx = null;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal WordStyle()
        {
        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        /// <param name="styleSheet"></param>
        /// <param name="name"></param>
        internal WordStyle(WordStyleSheet styleSheet, string name)
        {
            m_strName = name;
            m_styleSheet = styleSheet;
            m_charProps = new CharacterProperties(m_styleSheet);
            m_paragProps = new ParagraphProperties();
        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        /// <param name="styleSheet"></param>
        /// <param name="name"></param>
        /// <param name="isCharacterStyle"></param>
        internal WordStyle(WordStyleSheet styleSheet, string name, bool isCharacterStyle)
        {
            m_strName = name;
            m_styleSheet = styleSheet;
            m_bIsCharacter = isCharacterStyle;
            m_charProps = new CharacterProperties(m_styleSheet);

            if (!m_bIsCharacter)
            {
                m_paragProps = new ParagraphProperties();
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the table style representation
        /// </summary>
        internal byte[] TableStyleData
        {
            get
            {
                return m_tapx;
            }
            set
            {
                m_tapx = value;
            }
        }
        /// <summary>
        /// Gets or sets the type code of the style.
        /// </summary>
        internal WordStyleType TypeCode
        {
            get
            {
                return m_typeCode;
            }
            set
            {
                m_typeCode = value;
            }
        }
        /// <summary>
        /// Gets/sets style id
        /// </summary>
        internal int BaseStyleIndex
        {
            get
            {
                return m_baseStyleIndex;
            }

            set
            {
                m_baseStyleIndex = value;
            }
        }

        /// <summary>
        /// Gets/sets style id
        /// </summary>
        internal int ID
        {
            get
            {
                return m_id;
            }

            set
            {
                m_id = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsCharacterStyle
        {
            get
            {
                return m_bIsCharacter;
            }

            set
            {
                m_bIsCharacter = value;
            }
        }

        /// <summary>
        /// Gets style name
        /// </summary>
        internal string Name
        {
            get
            {
                return m_strName;
            }

            set
            {
                m_strName = value;
            }
        }

        /// <summary>
        /// Gets character properties
        /// </summary>
        internal CharacterProperties CharacterProperties
        {
            get
            {
                return m_charProps;
            }

            set
            {
                m_charProps = value;
            }
        }

        /// <summary>
        /// Gets paragraph properties
        /// </summary>
        internal ParagraphProperties ParagraphProperties
        {
            get
            {
                return m_paragProps;
            }

            set
            {
                m_paragProps = value;
            }
        }

        /// <summary>
        /// Indicates whether UPEs have been generated.
        /// </summary>
        internal bool HasUpe
        {
            get
            {
                return m_hasUpe;
            }

            set
            {
                m_hasUpe = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the next style.
        /// </summary>
        /// <value>The index of the next style.</value>
        internal int NextStyleIndex
        {
            get
            {
                return m_nextStyleIndex;
            }

            set
            {
                m_nextStyleIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the link style.
        /// </summary>
        /// <value>The index of the link style.</value>
        internal int LinkStyleIndex
        {
            get
            {
                return m_linkStyleIndex;
            }

            set
            {
                m_linkStyleIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is primary; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPrimary
        {
            get
            {
                return m_isPrimary;
            }

            set
            {
                m_isPrimary = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is semi hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is semi hidden; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSemiHidden
        {
            get
            {
                return m_isSemiHidden;
            }

            set
            {
                m_isSemiHidden = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether unhide when used.
        /// </summary>
        /// <value><c>true</c> if unhide when used; otherwise, <c>false</c>.</value>
        internal bool UnhideWhenUsed
        {
            get
            {
                return m_unhideWhenUsed;
            }

            set
            {
                m_unhideWhenUsed = value;
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Update character properties
        /// </summary>
        internal void UpdateCharactersProperties(CharacterProperties charProps)
        {
            m_charProps = charProps;
        }

        /// <summary>
        /// Update paragraph properties
        /// </summary>
        /// <param name="paraProps"></param>
        internal void UpdateParagraphsProperties(ParagraphProperties paraProps)
        {
            m_paragProps = paraProps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        internal void UpdateName(string name)
        {
            m_strName = name;
        }
        #endregion
    }
}