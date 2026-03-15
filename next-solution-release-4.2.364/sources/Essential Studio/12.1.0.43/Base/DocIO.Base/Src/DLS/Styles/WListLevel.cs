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

#region File using directives
using System;
using System.Collections;
using System.Text;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WListLevel.
    /// </summary>
    public class WListLevel : XDLSSerializableBase
    {
        #region Class constants
        /// <summary>
        /// Limit number of converting arabic to \"A\" format.
        /// </summary>
        private const float DEF_AR_TO_LETTER_LIMIT = 26.0f;
        /// <summary>
        /// Index of A char in the ASCII table.
        /// </summary>
        private const int DEF_A_ASCII_INDEX = (int)('A' - 1);
        /// <summary>
        /// 
        /// </summary>
        private readonly string[] DEF_NUMBER_WORDS = new string[]
      {
        "one", // 1
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine",
        "ten", // 10
        "eleven", 
        "twelve", 
        "thirteen", 
        "fourteen", 
        "fifteen", 
        "sixteen", 
        "seventeen", 
        "eighteen", 
        "nineteen"  // 19
      };
        /// <summary>
        /// 
        /// </summary>
        private readonly string[] DEF_TENS_WORDS = new string[]
      {
        "ten", // 1
        "twenty",
        "thirty",
        "forty",
        "fifty",
        "sixty",
        "seventy",
        "eighty",
        "ninety", // 19
    };
        /// <summary>
        /// List numbering strings 
        /// </summary>
        internal const string Level1Str = "\u0000";
        internal const string Level2Str = "\u0001";
        internal const string Level3Str = "\u0002";
        internal const string Level4Str = "\u0003";
        internal const string Level5Str = "\u0004";
        internal const string Level6Str = "\u0005";
        internal const string Level7Str = "\u0006";
        internal const string Level8Str = "\u0007";
        internal const string Level9Str = "\u0008";
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private WCharacterFormat m_chFormat;
        /// <summary>
        /// 
        /// </summary>
        private WParagraphFormat m_prFormat;
        /// <summary>
        /// 
        /// </summary>
        private string m_numberPrefix;
        /// <summary>
        /// 
        /// </summary>
        private string m_numberSufix;
        /// <summary>
        /// 
        /// </summary>
        private string m_layoutNumSuf = ".";
        /// <summary>
        /// 
        /// </summary>
        private string m_layoutNumPref = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private string m_bulletChar;
        /// <summary>
        /// 
        /// </summary>
        private bool m_noRestart;
        /// <summary>
        /// 
        /// </summary>
        private int m_startAt = 0;
        /// <summary>
        /// 
        /// </summary>
        private ListNumberAlignment m_alignment;
        /// <summary>
        /// 
        /// </summary>
        private ListPatternType m_patternType = ListPatternType.Arabic;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isLegal;
        /// <summary>
        /// 
        /// </summary>
        private FollowCharacterType m_followChar;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bUsePrevLevelPattern;
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_charOffset = new byte[9];
        /// <summary>
        /// 
        /// </summary>
        private bool m_legacy;
        /// <summary>
        /// 
        /// </summary>
        private int m_legacySpace;
        /// <summary>
        /// 
        /// </summary>
        private int m_legacyIndent;
        /// <summary>
        /// 
        /// </summary>
        private string m_pStyle;
        /// <summary>
        /// 
        /// </summary>
        private bool m_noLvlText;
        /// <summary>
        /// 
        /// </summary>
        private WPicture m_picBullet;
        /// <summary>
        /// 
        /// </summary>
        private short m_picButtetId;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsEmptyPicture;
        #endregion

        #region Class properties
        /// <summary>
        /// Get/set alignment (left, right, or centered) of the paragraph number. 
        /// </summary>
        public ListNumberAlignment NumberAlignment
        {
            get
            {
                return m_alignment;
            }
            set
            {
                m_alignment = value;
            }
        }
        /// <summary>
        /// Get/set start at value.
        /// </summary>
        public int StartAt
        {
            get
            {
                return m_startAt;
            }
            set
            {
                m_startAt = value;
            }
        }
        /// <summary>
        /// Get/set spacing after list level's number or bullet
        /// ( tab position if follow character is tab ).
        /// </summary>
        public float TabSpaceAfter
        {
            get
            {
                if (m_prFormat.Tabs.Count > 0)
                {
                    return m_prFormat.Tabs[0].Position;
                }

                return 0; //Document.LastSection.PageSetup.DefaultTabWidth;
            }
            set
            {
                m_prFormat.Tabs.AddTab(value);
            }
        }
        /// <summary>
        /// Gets / sets left listlevel indent
        /// </summary>
        public float TextPosition
        {
            get
            {
                return m_prFormat.LeftIndent;
            }
            set
            {
                m_prFormat.LeftIndent = value;
            }
        }
        /// <summary>
        /// Gets / set prefix pattern for numbered level.
        /// </summary>
        public string NumberPrefix
        {
            get
            {
                return m_numberPrefix;
            }
            set
            {
                m_numberPrefix = value;
            }
        }
        /// <summary>
        /// Gets / sets suffix pattern for numbered level.
        /// </summary>
        public string NumberSufix
        {
            get
            {
                return m_numberSufix;
            }
            set
            {
                m_numberSufix = value;
            }
        }
        /// <summary>
        /// Get/set bullet pattern
        /// </summary>
        public string BulletCharacter
        {
            get
            {
                return m_bulletChar;
            }
            set
            {
                m_bulletChar = value;
            }
        }
        /// <summary>
        /// Gets / sets list numbering type.
        /// </summary>
        public ListPatternType PatternType
        {
            get
            {
                return m_patternType;
            }
            set
            {
                m_patternType = value;
            }
        }
        /// <summary>
        /// True if the level's number sequence is not restarted by higher
        /// (more significant) levels in the list.
        /// </summary>
        public bool NoRestartByHigher
        {
            get
            {
                return m_noRestart;
            }
            set
            {
                m_noRestart = value;
            }
        }
        /// <summary>
        /// Gets / sets character formats of list symbol.
        /// </summary>
        public WCharacterFormat CharacterFormat
        {
            get
            {
                return m_chFormat;
            }
        }
        /// <summary>
        /// Gets / sets paragraph format of list level.
        /// </summary>
        public WParagraphFormat ParagraphFormat
        {
            get
            {
                return m_prFormat;
            }
        }
        /// <summary>
        /// Gets the owner list style.
        /// </summary>
        /// <value>The owner list style.</value>
        protected ListStyle OwnerListStyle
        {
            get
            {
                return OwnerBase as ListStyle;
            }
        }
        /// <summary>
        /// Gets previous list.
        /// </summary>
        protected WListLevel PreviousLevel
        {
            get
            {
                ListStyle ls = OwnerListStyle;

                if (ls != null)
                {
                    int index = ls.Levels.IndexOf(this);

                    if (index > 0)
                    {
                        return ls.Levels[index - 1];
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Get/set the type of character following the number text for the paragraph.
        /// </summary>
        public FollowCharacterType FollowCharacter
        {
            get
            {
                return m_followChar;
            }
            set
            {
                m_followChar = value;
            }
        }
        /// <summary>
        /// Get/set ArabaicNumberFormat property
        /// ( true if the level turns all inherited numbers to arabic,
        ///  false if it preserves their number format code ). 
        /// </summary>
        public bool IsLegalStyleNumbering
        {
            get
            {
                return m_isLegal;
            }
            set
            {
                m_isLegal = value;
            }
        }
        /// <summary>
        /// Get/set number/bullet position for current listlevel. 
        /// </summary>
        public float NumberPosition
        {
            get
            {
                return m_prFormat.FirstLineIndent;
            }
            set
            {
                m_prFormat.FirstLineIndent = value;
            }
        }
        /// <summary>
        /// When true, number generated will include previous
        /// levels (used for legal numbering).
        /// </summary>
        public bool UsePrevLevelPattern
        {
            get
            {
                return m_bUsePrevLevelPattern;
            }
            set
            {
                m_bUsePrevLevelPattern = value;
            }
        }
        /// <summary>
        /// Get/set Word6 compatibility mode.
        /// </summary>
        internal bool Word6Legacy
        {
            get
            {
                return m_legacy;
            }
            set
            {
                m_legacy = value;
            }
        }
        /// <summary>
        /// Get/set level space value for Word6 compatibility mode.
        /// </summary>
        internal int LegacySpace
        {
            get
            {
                return m_legacySpace;
            }
            set
            {
                m_legacySpace = value;
            }
        }
        /// <summary>
        /// Get/set level indent value for Word6 compatibility mode.
        /// </summary>
        internal int LegacyIndent
        {
            get
            {
                return m_legacyIndent;
            }
            set
            {
                m_legacyIndent = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the paragraph style.
        /// </summary>
        /// <value>The name of the paragraph style.</value>
        internal string ParaStyleName
        {
            get
            {
                return m_pStyle;
            }
            set
            {
                m_pStyle = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether level has level text in list level formatting string.
        /// </summary>
        /// <value>if it has no level text, set to <c>true</c>.</value>
        internal bool NoLevelText
        {
            get
            {
                return m_noLvlText;
            }
            set
            {
                m_noLvlText = value;
            }
        }
        /// <summary>
        /// Gets the number of the level.
        /// </summary>
        /// <value>The index of the level.</value>
        internal int LevelNumber
        {
            get
            {
                if (OwnerListStyle == null)
                {
                    if (OwnerBase is OverrideLevelFormat)
                    {
                        OverrideLevelFormat lvlFormat = OwnerBase as OverrideLevelFormat;
                        return (lvlFormat.OwnerBase as ListOverrideStyle).OverrideLevels.GetLevelNumber(lvlFormat);
                    }
                    return -1;
                }
                return OwnerListStyle.Levels.IndexOf(this);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has pic bullet.
        /// </summary>
        internal WPicture PicBullet
        {
            get
            {
                return m_picBullet;
            }
            set
            {
                m_picBullet = value;
                m_picBullet.SetOwner(this);
            }
        }
        /// <summary>
        /// Gets or sets the pic bullet id.
        /// </summary>
        /// <value>The pic bullet id.</value>
        internal short PicBulletId
        {
            get
            {
                return m_picButtetId;
            }
            set
            {
                m_picButtetId = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int PicIndex
        {
            get
            {
                return CharacterFormat.ListPictureIndex;
            }
        }
        internal bool IsEmptyPicture
        {
            get
            {
                return m_bIsEmptyPicture;
            }
            set
            {
                m_bIsEmptyPicture = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listStyle"></param>
        public WListLevel(ListStyle listStyle)
            : this(listStyle.Document)
        {
            SetOwner(listStyle);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        internal WListLevel(WordDocument doc)
            : base(doc, null)
        {
            m_chFormat = m_doc.CreateCharacterFormatImpl();
            m_chFormat.SetOwner(this);
            m_prFormat = m_doc.CreateParagraphFormatImpl();
            m_prFormat.SetOwner(this);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Create level layout data
        /// </summary>
        /// <param name="numStr"></param>
        /// <param name="characterOffsets"></param>
        /// <param name="levelNumber"></param>
        public void CreateLayoutData(string numStr, byte[] characterOffsets, int levelNumber)
        {
            int start = 0;
            int length = 0;
            char[] splitter = new char[2] { '\\', Convert.ToChar(levelNumber) };
            string[] patternArray = numStr.Split(splitter);
            int numOffset = patternArray[0].Length + 1;
            for (int i = 0; i < 9; i++)
            {
                if ((int)characterOffsets[i] == numOffset)
                {
                    // Gets prefix
                    if (i == 0)
                        m_layoutNumPref = numStr.Substring(0, numOffset - 1);
                    else
                    {
                        start = (int)characterOffsets[i - 1];
                        length = (numOffset - 1) - (int)characterOffsets[i - 1];
                        m_layoutNumPref = numStr.Substring(start, length);
                    }

                    // Gets suffix
                    if ((i == 8) || ((int)characterOffsets[i + 1] == 0))
                    {
                        m_layoutNumSuf = patternArray[1];
                    }
                    else
                    {
                        length = (int)characterOffsets[i + 1] - (numOffset + 1);
                        start = numOffset + 1;
                        m_layoutNumSuf = numStr.Substring(start, length);
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// Gets list symbol for specified item index
        /// </summary>
        /// <param name="listItemIndex"></param>
        /// <returns></returns>
        public string GetListItemText(int listItemIndex, ListType listType)
        {
            string listItemText = string.Empty;
            if (listType == ListType.Bulleted && PatternType != ListPatternType.Bullet)
            {
                listType = ListType.Numbered;
            }
            switch (listType)
            {
                case ListType.Numbered:
                    if (m_numberPrefix != null && m_numberSufix != null) //Check whether the list text is empty or not
                        listItemText = GetNumberedItemText(listItemIndex);
                    break;
                case ListType.Bulleted:
                    listItemText = m_bulletChar;
                    break;
                case ListType.NoList:
                default:
                    listItemText = "";
                    break;
            }

            return listItemText;
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public WListLevel Clone()
        {
            return (WListLevel)CloneImpl();
        }
        /// <summary>
        /// Create default bullet level.
        /// </summary>
        /// <param name="dxLeft"></param>
        /// <param name="str"></param>
        /// <param name="listStyle"></param>
        /// <returns></returns>
        internal static WListLevel CreateDefBulletLvl(float dxLeft, string str, ListStyle listStyle)
        {
            WListLevel lvl = listStyle.Document.CreateListLevelImpl(listStyle);
            lvl.m_startAt = 1;
            lvl.m_patternType = ListPatternType.Bullet;
            //lvl.m_bUsePrevLevelPattern = true;      

            // we switch font depending on bullet style
            string fontName = "Times New Roman";
            switch (str)
            {
                case ListStyle.DEF_BULLLET_FIRST:
                    fontName = "Symbol";
                    break;

                case ListStyle.DEF_BULLLET_SECOND:
                    fontName = "Courier New";
                    break;

                case ListStyle.DEF_BULLLET_THIRD:
                    fontName = "Wingdings";
                    break;
            }

            lvl.m_chFormat.FontName = fontName;
            lvl.m_prFormat.LeftIndent = dxLeft;
            lvl.m_bulletChar = str;

            return lvl;
        }
        /// <summary>
        /// Create default numbered level.
        /// </summary>
        internal static WListLevel CreateDefNumberLvl(int dxLeft, int levelNumber,
          ListPatternType patType, ListNumberAlignment align, ListStyle listStyle)
        {
            WListLevel lvl = listStyle.Document.CreateListLevelImpl(listStyle);
            lvl.m_startAt = 1;
            lvl.m_patternType = patType;
            lvl.m_alignment = align;
            lvl.NumberPrefix = string.Empty;
            lvl.NumberSufix = ".";
            //lvl.m_bUsePrevLevelPattern = true;

            lvl.m_prFormat.LeftIndent = dxLeft;
            lvl.m_chFormat.FontName = "Times New Roman";
            return lvl;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WListLevel cloneLevel = (WListLevel)base.CloneImpl();

            cloneLevel.m_chFormat = new WCharacterFormat(Document);
            cloneLevel.m_chFormat.SetOwner(cloneLevel);
            cloneLevel.m_prFormat = new WParagraphFormat(Document);
            cloneLevel.m_prFormat.SetOwner(cloneLevel);
            cloneLevel.m_prFormat.ImportContainer(ParagraphFormat);
            cloneLevel.m_chFormat.ImportContainer(CharacterFormat);
            if (PicBullet != null)
                cloneLevel.PicBullet = m_picBullet.Clone() as WPicture;


            cloneLevel.m_charOffset = new byte[m_charOffset.Length];
            m_charOffset.CopyTo(cloneLevel.m_charOffset, 0);

            return cloneLevel;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listItemIndex"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private string GetNumberedItemText(int listItemIndex)
        {
            char[] trimChars = new char[] { '.' };
            switch (m_patternType)
            {
                case ListPatternType.UpRoman:
                    return m_numberPrefix + GetAsRoman(listItemIndex + 1).ToUpper() + m_numberSufix;
                case ListPatternType.LowRoman:
                    return m_layoutNumPref + GetAsRoman(listItemIndex + 1).ToLower() + m_layoutNumSuf;
                case ListPatternType.UpLetter:
                    return m_layoutNumPref + GetAsLetter(listItemIndex + 1).ToUpper() + m_layoutNumSuf;
                case ListPatternType.LowLetter:
                    return m_numberPrefix + GetAsLetter(listItemIndex + 1).ToLower() + m_numberSufix;
                case ListPatternType.Ordinal:
                    return m_numberPrefix + this.Document.GetOrdinal(listItemIndex + 1,this.CharacterFormat) + m_numberSufix;
                case ListPatternType.Arabic:
                    return m_numberPrefix + (listItemIndex + 1).ToString() + m_numberSufix;
                case ListPatternType.LeadingZero:
                    if(listItemIndex < 9)
                        return m_numberPrefix + "0" + (listItemIndex + 1).ToString() + m_numberSufix;
                    else
                        return m_numberPrefix + (listItemIndex + 1).ToString() + m_numberSufix;
                case ListPatternType.None:
                    return "";
                default:
                    return m_layoutNumPref + (listItemIndex + 1).ToString() + m_layoutNumSuf;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private string GetAsRoman(int number)
        {
            StringBuilder retval = new StringBuilder();

            retval.Append(GenerateNumber(ref number, 1000, "M"));
            retval.Append(GenerateNumber(ref number, 900, "CM"));
            retval.Append(GenerateNumber(ref number, 500, "D"));
            retval.Append(GenerateNumber(ref number, 400, "CD"));
            retval.Append(GenerateNumber(ref number, 100, "C"));
            retval.Append(GenerateNumber(ref number, 90, "XC"));
            retval.Append(GenerateNumber(ref number, 50, "L"));
            retval.Append(GenerateNumber(ref number, 40, "XL"));
            retval.Append(GenerateNumber(ref number, 10, "X"));
            retval.Append(GenerateNumber(ref number, 9, "IX"));
            retval.Append(GenerateNumber(ref number, 5, "V"));
            retval.Append(GenerateNumber(ref number, 4, "IV"));
            retval.Append(GenerateNumber(ref number, 1, "I"));

            return retval.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private string GetAsLetter(int number)
        {
            Stack<int> stack = ConvertToLetter(number);
            StringBuilder result = new StringBuilder();

            while (stack.Count > 0)
            {
                int num = stack.Pop();
                AppendChar(result, num);
            }

            return result.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="isOrdinal"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private string GetAsWord(int number, bool isOrdinal)
        {
            string retValue = "";
            if (isOrdinal)
            {
                throw new NotImplementedException("style list not implemented now");
            }
            else
            {

                if (number > 99)
                    throw new ArgumentOutOfRangeException("Cannot support number greater than 99");

                if (number < 20)
                {
                    retValue = DEF_NUMBER_WORDS[number];
                }
                else
                {
                    int tens = (int)Math.Floor((double)number / 10);
                    retValue = DEF_TENS_WORDS[tens] + "-" + DEF_NUMBER_WORDS[number - tens * 10];
                }
            }

            return retValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="magnitude"></param>
        /// <param name="letter"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private string GenerateNumber(ref int value, int magnitude, string letter)
        {
            StringBuilder numberstring = new StringBuilder();

            while (value >= magnitude)
            {
                value -= magnitude;
                numberstring.Append(letter);
            }

            return numberstring.ToString();
        }
        /// <summary>
        /// Utility metnod. Helps to convert arabic number to \"A\" format.
        /// </summary>
        /// <param name="arabic">Arabic number.</param>
        /// <returns>Sequence of number.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private static Stack<int> ConvertToLetter(float arabic)
        {
            if (arabic < 0)
#if SILVERLIGHT || WP
        throw new ArgumentOutOfRangeException( "arabic", "Value can not be less 0" );
#else
                throw new ArgumentOutOfRangeException("arabic", arabic, "Value can not be less 0");
#endif



            Stack<int> stack = new Stack<int>();


            while (((int)arabic) > DEF_AR_TO_LETTER_LIMIT)
            {
                float remainder = arabic % DEF_AR_TO_LETTER_LIMIT;

                if (remainder == 0.0f)
                {
                    arabic = arabic / DEF_AR_TO_LETTER_LIMIT - 1f;
                    remainder = DEF_AR_TO_LETTER_LIMIT;
                }
                else
                {
                    arabic /= DEF_AR_TO_LETTER_LIMIT;
                }

                stack.Push((int)remainder);
            }

            if (arabic > 0f)
            {
                stack.Push((int)arabic);
            }

            return stack;
        }
        /// <summary>
        /// Adds letter instead of number.
        /// </summary>
        /// <param name="builder">String builder object.</param>
        /// <param name="number">Number to be converted to letter.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        private static void AppendChar(StringBuilder builder, int number)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (number <= 0 || number > 26)
                throw new ArgumentOutOfRangeException("number", "Value can not be less 0 and greater 26");

            char letter = (char)(DEF_A_ASCII_INDEX + number);
            builder.Append(letter);
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            m_charOffset = null;

            if (m_chFormat != null)
            {
                m_chFormat.Close();
                m_chFormat = null;
            }

            if (m_prFormat != null)
            {
                m_prFormat.Close();
                m_chFormat = null;
            }
        }
        #endregion
//#if !SILVERLIGHT
        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.ListLevelIndentAttr))
            {
                TextPosition = reader.ReadFloat(XDLSConstants.ListLevelIndentAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelPrefPatternAttr))
            {
                NumberPrefix = reader.ReadString(XDLSConstants.ListLevelPrefPatternAttr);
            }
            else
            {
                NumberPrefix = null;
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelSufPatternAttr))
            {
                NumberSufix = reader.ReadString(XDLSConstants.ListLevelSufPatternAttr);
            }
            else
            {
                NumberSufix = null;
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelBulletPatternAttr))
            {
                BulletCharacter = reader.ReadString(XDLSConstants.ListLevelBulletPatternAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelPatternTypeAttr))
            {
                PatternType = (ListPatternType)reader.ReadEnum(XDLSConstants.ListLevelPatternTypeAttr, typeof(ListPatternType));
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelPrevPatternAttr))
            {
                UsePrevLevelPattern = reader.ReadBoolean(XDLSConstants.ListLevelPrevPatternAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelStartAtAttr))
            {
                StartAt = reader.ReadInt(XDLSConstants.ListLevelStartAtAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelNumberAlignAttr))
            {
                NumberAlignment = (ListNumberAlignment)reader.ReadEnum(XDLSConstants.ListLevelNumberAlignAttr,
                  typeof(ListNumberAlignment));
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelFollowCharacterAttr))
            {
                FollowCharacter = (FollowCharacterType)reader.ReadEnum(XDLSConstants.ListLevelFollowCharacterAttr, typeof(FollowCharacterType));
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelIsLegalAttr))
            {
                IsLegalStyleNumbering = reader.ReadBoolean(XDLSConstants.ListLevelIsLegalAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelNoRestartNum))
            {
                NoRestartByHigher = reader.ReadBoolean(XDLSConstants.ListLevelNoRestartNum);
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelLegacyAttr))
            {
                m_legacy = reader.ReadBoolean(XDLSConstants.ListLevelLegacyAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelLegacyIndentAttr))
            {
                m_legacyIndent = reader.ReadInt(XDLSConstants.ListLevelLegacyIndentAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListLevelLegacySpaceAttr))
            {
                m_legacySpace = reader.ReadInt(XDLSConstants.ListLevelLegacySpaceAttr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(XDLSConstants.ListLevelIndentAttr, TextPosition);
            writer.WriteValue(XDLSConstants.ListLevelPrefPatternAttr, NumberPrefix);
            writer.WriteValue(XDLSConstants.ListLevelSufPatternAttr, NumberSufix);
            writer.WriteValue(XDLSConstants.ListLevelBulletPatternAttr, BulletCharacter);
            writer.WriteValue(XDLSConstants.ListLevelPatternTypeAttr, PatternType);
            writer.WriteValue(XDLSConstants.ListLevelPrevPatternAttr, UsePrevLevelPattern);
            writer.WriteValue(XDLSConstants.ListLevelStartAtAttr, StartAt);
            ListStyle ls = OwnerListStyle;
            if (ls != null && ls.ListType == ListType.Numbered)
            {
                writer.WriteValue(XDLSConstants.ListLevelNumberAlignAttr, NumberAlignment);
            }
            writer.WriteValue(XDLSConstants.ListLevelIsLegalAttr, IsLegalStyleNumbering);
            writer.WriteValue(XDLSConstants.ListLevelFollowCharacterAttr, FollowCharacter);
            writer.WriteValue(XDLSConstants.ListLevelNoRestartNum, NoRestartByHigher);
            if (m_legacy)
            {
                writer.WriteValue(XDLSConstants.ListLevelLegacyAttr, m_legacy);
                writer.WriteValue(XDLSConstants.ListLevelLegacyIndentAttr, m_legacyIndent);
                writer.WriteValue(XDLSConstants.ListLevelLegacySpaceAttr, m_legacySpace);
            }
        }
        /// <summary>
        /// Serialize paragraph and character properties.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();
            XDLSHolder.AddElement(XDLSConstants.ParagraphFormatTag, m_prFormat);
            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, m_chFormat);
        }
        #endregion
//#endif
    }
}
