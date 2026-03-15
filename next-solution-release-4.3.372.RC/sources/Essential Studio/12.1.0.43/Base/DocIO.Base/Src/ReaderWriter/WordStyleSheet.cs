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

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for WordStyleSheet.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordStyleSheet
    {
        #region Class constants
        internal const string DEF_FONT_NAME = "Times New Roman";

        /// <summary>
        /// 
        /// </summary>
        private const string DEF_NORMAL_STYLE = "Normal";

        private const string DEF_DPF_STYLE = "Default Paragraph Font";
        private const string DEF_LIST_FONT_NAME = "Wingdings";
        private const int DEF_STDCOUNT = 15;
        /// <summary>
        /// Returns true, if the fixed index 13 in stylesheet has style. (other than empty style)
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        internal bool IsFixedIndex13HasStyle = false;
        /// <summary>
        /// Returns true, if the fixed index 14 in stylesheet has style. (other than empty style)
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        internal bool IsFixedIndex14HasStyle = false;
        /// <summary>
        /// Represents the style name of the style present at the fixed index 13 in the stylesheet
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        internal string FixedIndex13StyleName = string.Empty;
        /// <summary>
        /// Represents the style name of the style present at the fixed index 14 in the stylesheet
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        internal string FixedIndex14StyleName = string.Empty;
        #endregion

        #region Class members
        /// <summary>
        /// List of font names
        /// </summary>
        private List<String> m_fontNameList = new List<String>();

        private Dictionary<String, Int32> m_fontNames = new Dictionary<String, Int32>();

        /// <summary>
        /// Default font names
        /// </summary>
        private string[] m_defFontNames = new string[] { "Times New Roman", "Symbol", "Arial", "Verdana", "Wingdings", "Courier New" };

        /// <summary>
        /// List of styles.
        /// </summary>
        private List<WordStyle> m_styleList = new List<WordStyle>();

        /// <summary>
        /// 
        /// </summary>
        private int m_defStyleIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string> m_fontSubstitutionTable;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, string> m_styleNames;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal WordStyleSheet()
        {
            WordStyle defStyle = new WordStyle(this, DEF_NORMAL_STYLE);
            defStyle.ID = 0;
            m_defStyleIndex = AddStyle(defStyle);

            UpdateFontNames(m_defFontNames);
            defStyle.CharacterProperties.FontAscii = 0;
            for (int i = 0; i < DEF_STDCOUNT - 1; i++)
            {
                AddEmptyStyle();
            }
        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal WordStyleSheet(bool createDefCharStyle)
        {
            WordStyle defStyle = new WordStyle(this, DEF_NORMAL_STYLE);
            defStyle.ID = 0;
            m_defStyleIndex = AddStyle(defStyle);

            UpdateFontNames(m_defFontNames);
            //defStyle.CharacterProperties.FontAscii = 0;

            if (createDefCharStyle)
            {
                for (int i = 1; i < 10; i++)
                {
                    AddEmptyStyle();
                }

                WordStyle defaultParaFont = new WordStyle(this, DEF_DPF_STYLE);
                defaultParaFont.ID = 65;
                defaultParaFont.IsCharacterStyle = true;
                defaultParaFont.BaseStyleIndex = 4095;
                defaultParaFont.HasUpe = true;
                AddStyle(defaultParaFont);

                for (int i = 11; i < DEF_STDCOUNT; i++)
                {
                    AddEmptyStyle();
                }
            }
            else
            {
                for (int i = 0; i < DEF_STDCOUNT - 1; i++)
                {
                    AddEmptyStyle();
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the stye names.
        /// </summary>
        /// <value>The stye names.</value>
        internal Dictionary<int, string> StyeNames
        {
            get
            {
                if (m_styleNames == null)
                    m_styleNames = new Dictionary<int, string>();
                return m_styleNames;
            }
        }
        /// <summary>
        /// Dictionary to hold the font substitution values
        /// </summary>
        internal Dictionary<string, string> FontSubstitutionTable
        {
            get
            {
                if (m_fontSubstitutionTable == null)
                {
                    m_fontSubstitutionTable = new Dictionary<string, string>();
                }
                return m_fontSubstitutionTable;
            }
            set
            {
                m_fontSubstitutionTable = value;
            }
        }

        /// <summary>
        /// Gets array of font names
        /// </summary>
        internal List<String> FontNamesList
        {
            get
            {
                return m_fontNameList;
            }
        }

        /// <summary>
        /// Gets default style index
        /// </summary>
        internal int DefaultStyleIndex
        {
            get
            {
                return m_defStyleIndex;
            }
        }

        /// <summary>
        /// Gets number of styles
        /// </summary>
        internal int StylesCount
        {
            get
            {
                return m_styleList.Count;
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Creates the style.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        internal WordStyle CreateStyle(string name)
        {
            return CreateStyle(name, false);
        }

        /// <summary>
        /// Create style with specified name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="characterStyle">if it is character style, set to <c>true</c>.</param>
        /// <returns></returns>
        internal WordStyle CreateStyle(string name, bool characterStyle)
        {
            WordStyle style = new WordStyle(this, name, characterStyle);
            AddStyle(style);
            return style;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal WordStyle CreateStyle(string name, int index)
        {
            if (index < 15)
                throw new ArgumentOutOfRangeException("index must be greater than 14");

            ValidateNameParameter(name, index);

            while (StylesCount < index)
            {
                AddEmptyStyle();
            }
            WordStyle style = new WordStyle(this, name);
            AddStyle(style);
            return style;
        }

        /// <summary>
        /// Adds style to stylesheet
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        internal int AddStyle(WordStyle style)
        {
            if (style == null)
                throw new ArgumentNullException("style");

#if DEBUG
//      if (StyleNameToIndex(style.Name) > 0)
//        throw new ArgumentException("style.Name");
#endif
            m_styleList.Add(style);
            return m_styleList.Count - 1;
        }

        /// <summary>
        /// Add empty style to stylesheet
        /// </summary>
        /// <returns></returns>
        internal int AddEmptyStyle()
        {
        	int index = m_styleList.Count;
            m_styleList.Add(WordStyle.Empty);
            return index;
        }

        /// <summary>
        /// Gets style index by specified style name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isCharacter">if it is character, set to <c>true</c>.</param>
        /// <returns></returns>
        internal int StyleNameToIndex(string name, bool isCharacter)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Style name can't be null or empty");

            for (int i = 0, len = m_styleList.Count; i < len; i++)
            {
                WordStyle style = (WordStyle)m_styleList[i];

                if (style.Name == name && style.IsCharacterStyle == isCharacter)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets style index by specified style name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        internal int StyleNameToIndex(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Style name can't be null or empty");

            for (int i = 0, len = m_styleList.Count; i < len; i++)
            {
                WordStyle style = (WordStyle)m_styleList[i];

                if (style.Name == name)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets font index by specified font name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal int FontNameToIndex(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            //if (name.Length == 0)
            //    throw new ArgumentException("name - string can not be empty");

            //      for (int i = 0, len = m_fontNameList.Count; i < len; i++)
            //      {
            //        if (m_fontNameList[i].ToString() == name)
            //          return i;
            //      }

            if (m_fontNames.ContainsKey(name))
            {
                return m_fontNames[name];
            }
            return -1;
        }

        /// <summary>
        /// Gets style by specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal WordStyle GetStyleByIndex(int index)
        {
            if (index < 0 || index > m_styleList.Count - 1)
            {
#if DEBUG
        //throw new ArgumentOutOfRangeException("index");
        Debug.WriteLine("Style index is out of range");        
#endif
                index = 0;
            }


            return m_styleList[index] as WordStyle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal WordStyle UpdateStyle(int index, string name)
        {
            if (index < 0 || index > StylesCount - 1)
                throw new ArgumentOutOfRangeException(
                  string.Format("Index should be between 0 and {0}", StylesCount - 1));

            ValidateNameParameter(name, index);

            WordStyle style = GetStyleByIndex(index);

            if (style == WordStyle.Empty)
            {
                m_styleList[index] = style = new WordStyle(this, name);
            }
            else
            {
                style.UpdateName(name);
            }

            return style;
        }

        /// <summary>
        /// Merge current stylesheet with specified one.
        /// </summary>
        /// <param name="styleSheet"></param>
        internal void MergeStyleSheets(WordStyleSheet styleSheet)
        {
            WordStyle importedStyle;
            WordStyle style;
            for (int i = 0; i < DEF_STDCOUNT; i++)
            {
                importedStyle = (WordStyle)(styleSheet.m_styleList[i]);
                if (importedStyle != WordStyle.Empty)
                {
                    string s = importedStyle.Name;
                    //int index = StyleNameToIndex(s);
                    style = UpdateStyle(i, s);

                    // NOTE: if method will be used need refactoring
                    CharacterPropertyException chpx = null; //importedStyle.CharacterProperties.CloneChpx();
                    if (chpx != null)
                    {
                        style.CharacterProperties = new CharacterProperties(chpx, this);
                        style.CharacterProperties.FontNameAscii = importedStyle.CharacterProperties.FontNameAscii;
                    }
                    ParagraphPropertyException papx = importedStyle.ParagraphProperties.ClonePapx();
                    if (papx != null)
                    {
                        style.ParagraphProperties = new ParagraphProperties(papx);
                    }
                }
            }
            for (int i = DEF_STDCOUNT; i < styleSheet.StylesCount; i++)
            {
                importedStyle = (WordStyle)styleSheet.m_styleList[i];
                if (importedStyle != WordStyle.Empty)
                {
                    string s = importedStyle.Name;
                    int index = StyleNameToIndex(s, importedStyle.IsCharacterStyle);
                    if (index != -1)
                    {
                        style = UpdateStyle(index, s);
                        // NOTE: if method will be used need refactoring
                        CharacterPropertyException chpx = null; //importedStyle.CharacterProperties.CloneChpx();
                        if (chpx != null)
                        {
                            style.CharacterProperties = new CharacterProperties(chpx, this);
                            style.CharacterProperties.FontNameAscii = importedStyle.CharacterProperties.FontNameAscii;
                        }
                        ParagraphPropertyException papx = importedStyle.ParagraphProperties.ClonePapx();
                        if (papx != null)
                        {
                            style.ParagraphProperties = new ParagraphProperties(papx);
                        }
                    }
                    else
                    {
                        AddStyle(importedStyle);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the style by index.
        /// </summary>
        /// <param name="index">The index.</param>
        internal void RemoveStyleByIndex(int index)
        {
            m_styleList.RemoveAt(index);
        }

        /// <summary>
        /// Inserts the style.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="style">The style.</param>
        internal void InsertStyle(int index, WordStyle style)
        {
            m_styleList.Insert(index, style);
        }
        #endregion

        #region Class internals
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ffnRecord"></param>
        internal void UpdateFontSubstitutionTable(FontFamilyNameRecord ffnRecord)
        {
            if (ffnRecord.AlternativeFontName != null && ffnRecord.AlternativeFontName != string.Empty)
            {
                if (!FontSubstitutionTable.ContainsKey(ffnRecord.FontName))
                {
                    m_fontSubstitutionTable.Add(ffnRecord.FontName, ffnRecord.AlternativeFontName);
                }
                else
                    FontSubstitutionTable[ffnRecord.FontName] = ffnRecord.AlternativeFontName;
            }
        }
        /// <summary>
        /// Updates the name of the font.
        /// </summary>
        /// <param name="name">The name.</param>
        internal void UpdateFontName(string name)
        {
            UpdateFontNames(new string[] { name });
        }

        /// <summary>
        /// Add specified font names to collection of font names  
        /// </summary>
        /// <param name="names"></param>
        internal void UpdateFontNames(string[] names)
        {
            m_fontNameList.AddRange(names);
            for (int i = 0; i < names.Length; i++)
            {
                try
                {
                    m_fontNames.Add(names[i], m_fontNames.Count);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Clear font name arraylist
        /// </summary>
        internal void ClearFontNames()
        {
            m_fontNameList.Clear();
            m_fontNames.Clear();
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="withoutIndex"></param>
        private void ValidateNameParameter(string name, int withoutIndex)
        {
            for (int i = 0; i < m_styleList.Count; i++)
            {
                WordStyle st = (WordStyle)m_styleList[i];
                //        if (st.Name == name && i != withoutIndex)
                //throw new ArgumentException("style name already exists");
                //          Debug.WriteLine("Style with the same name allready exists");
            }
        }
        #endregion
    }
}