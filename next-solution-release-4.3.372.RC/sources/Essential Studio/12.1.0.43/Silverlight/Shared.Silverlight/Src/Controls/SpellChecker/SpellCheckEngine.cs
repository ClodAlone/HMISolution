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
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Resources;
using System.Windows.Input;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// LanguageType
    /// </summary>
    public enum LanguageType
    {
        /// <exclude/>
        ENGLISH = 0,
        /// <exclude/>
        DUTCH,
        /// <exclude/>
        FRENCH,
        /// <exclude/>
        GERMAN,
        /// <exclude/>
        ITALIAN,
        /// <exclude/>
        PORTUGUESE,
        /// <exclude/>
        SPANISH
    }

    internal interface ICheckerEngine
    {
        // Methods
        bool AddWord(string word);
        void ChangeBadWord(string newWord);
        void Check(string text);
        List<object> FindSuggestions();
        int GetConsiderationRange();
        bool GetIncludeUserDictionaryInSuggestions();
        void IgnoreAll(string word);
        bool LookUp(string word);
        BadWord NextBadWord();
        void SetConsiderationRange(int range);
        void SetDictFileStream(Stream dictFileStream, string sKey);
        void SetIncludeUserDictionaryInSuggestions(bool includeUserDictionaryInSuggestions);
        void SetPosition(int pos);
        void SetSeparateHyphenWords(bool separate);
        void SetSuggestionsMethod(int method);
        void SetUserDictionary(UserDictionary userDictionary);
        void SetUserDictionary(string userDictionary);

        // Properties
        bool AllowAnyCase { get; set; }
        bool ExcludeWordsInMixedCase { get; set; }
        bool ExcludeWordsInUpperCase { get; set; }
        bool CheckCompoundWords { get; set; }
        BadWord CurrentBadWord { get; }
        string DictionaryPath { get; set; }
        bool FindCapitalizedSuggestions { get; set; }
        List<object> IgnoreList { get; set; }
        bool ExcludeInternetAddresses { get; set; }
        bool ExcludeEmailAddress { get; set; }
        bool ExcludeFileNames { get; set; }
        bool ExcludeWordsWithNumbers { get; set; }
        bool ExcludeHtmlTags { get; set; }
        LanguageType LanguageParser { get; set; }
        bool LookIntoHyphenatedText { get; set; }
        bool SuggestSplitWords { get; set; }
        AdvancedTextBoundary TextBoundary { get; set; }
        UserDictionary UserDictionary { get; }
        bool V2Parser { get; set; }
        bool WarnDuplicates { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SpellChecker :
         DependencyObject, ICheckerEngine
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler SpellCheckCompleted;

        // Constants
        private static int[] DecimalCodes = new int[] { 
            160, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 170, 0xab, 0xac, 0xad, 0xae, 0xaf, 
            0xb0, 0xb1, 0xb2, 0xb3, 180, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 190, 0xbf, 
            0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 200, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf, 
            0xd0, 0xd1, 210, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 220, 0xdd, 0xde, 0xdf, 
            0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 230, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef, 
            240, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 250, 0xfb, 0xfc, 0xfd, 0xfe, 0xff, 
            0x22, 0x26, 60, 0x3e, 0x152, 0x153, 0x160, 0x161, 0x178, 710, 0x2dc, 0x2002, 0x2003, 0x2009, 0x200c, 0x200d, 
            0x200e, 0x200f, 0x2013, 0x2014, 0x2018, 0x2019, 0x201a, 0x201c, 0x201d, 0x201e, 0x2020, 0x2021, 0x2030, 0x2039, 0x203a, 0x20ac, 
            0x192, 0x391, 0x392, 0x393, 0x394, 0x395, 0x396, 0x397, 920, 0x399, 0x39a, 0x39b, 0x39c, 0x39d, 0x39e, 0x39f, 
            0x3a0, 0x3a1, 0x3a3, 0x3a4, 0x3a5, 0x3a6, 0x3a7, 0x3a8, 0x3a9, 0x3b1, 0x3b2, 0x3b3, 0x3b4, 0x3b5, 950, 0x3b7, 
            0x3b8, 0x3b9, 0x3ba, 0x3bb, 0x3bc, 0x3bd, 0x3be, 0x3bf, 960, 0x3c1, 0x3c2, 0x3c3, 0x3c4, 0x3c5, 0x3c6, 0x3c7, 
            0x3c8, 0x3c9, 0x3d1, 0x3d2, 0x3d6, 0x2022, 0x2026, 0x2032, 0x2033, 0x203e, 0x2044, 0x2118, 0x2111, 0x211c, 0x2122, 0x2135, 
            0x2190, 0x2191, 0x2192, 0x2193, 0x2194, 0x21b5, 0x21d0, 0x21d1, 0x21d2, 0x21d3, 0x21d4, 0x2200, 0x2202, 0x2203, 0x2205, 0x2207, 
            0x2208, 0x2209, 0x220b, 0x220f, 0x2211, 0x2212, 0x2217, 0x221a, 0x221d, 0x221e, 0x2220, 0x2227, 0x2228, 0x2229, 0x222a, 0x222b, 
            0x2234, 0x223c, 0x2245, 0x2248, 0x2260, 0x2261, 0x2264, 0x2265, 0x2282, 0x2283, 0x2284, 0x2286, 0x2287, 0x2295, 0x2297, 0x22a5, 
            0x22c5, 0x2308, 0x2309, 0x230a, 0x230b, 0x2329, 0x232a, 0x25ca, 0x2660, 0x2663, 0x2665, 0x2666
         };

        private static string[] entities = new string[] { 
            "nbsp", "iexcl", "cent", "pound", "curren", "yen", "brvbar", "sect", "uml", "copy", "ordf", "laquo", "not", "shy", "reg", "macr", 
            "deg", "plusmn", "sup2", "sup3", "acute", "micro", "para", "middot", "cedil", "sup1", "ordm", "raquo", "frac14", "frac12", "frac34", "iquest", 
            "Agrave", "Aacute", "Acirc", "Atilde", "Auml", "Aring", "AElig", "Ccedil", "Egrave", "Eacute", "Ecirc", "Euml", "Igrave", "Iacute", "Icirc", "Iuml", 
            "ETH", "Ntilde", "Ograve", "Oacute", "Ocirc", "Otilde", "Ouml", "times", "Oslash", "Ugrave", "Uacute", "Ucirc", "Uuml", "Yacute", "THORN", "szlig", 
            "agrave", "aacute", "acirc", "atilde", "auml", "aring", "aelig", "ccedil", "egrave", "eacute", "ecirc", "euml", "igrave", "iacute", "icirc", "iuml", 
            "eth", "ntilde", "ograve", "oacute", "ocirc", "otilde", "ouml", "divide", "oslash", "ugrave", "uacute", "ucirc", "uuml", "yacute", "thorn", "yuml", 
            "quot", "amp", "lt", "gt", "OElig", "oelig", "Scaron", "scaron", "Yuml", "circ", "tilde", "ensp", "emsp", "thinsp", "zwnj", "zwj", 
            "lrm", "rlm", "ndash", "mdash", "lsquo", "rsquo", "sbquo", "ldquo", "rdquo", "bdquo", "dagger", "Dagger", "permil", "lsaquo", "rsaquo", "euro", 
            "fnof", "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa", "Lambda", "Mu", "Nu", "Xi", "Omicron", 
            "Pi", "Rho", "Sigma", "Tau", "Upsilon", "Phi", "Chi", "Psi", "Omega", "alpha", "beta", "gamma", "delta", "epsilon", "zeta", "eta", 
            "theta", "iota", "kappa", "lambda", "mu", "nu", "xi", "omicron", "pi", "rho", "sigmaf", "sigma", "tau", "upsilon", "phi", "chi", 
            "psi", "omega", "thetasym", "upsih", "piv", "bull", "hellip", "prime", "Prime", "oline", "frasl", "weierp", "image", "real", "trade", "alefsym", 
            "larr", "uarr", "rarr", "darr", "harr", "crarr", "lArr", "uArr", "rArr", "dArr", "hArr", "forall", "part", "exist", "empty", "nabla", 
            "isin", "notin", "ni", "prod", "sum", "minus", "lowast", "radic", "prop", "infin", "ang", "and", "or", "cap", "cup", "int", 
            "there4", "sim", "cong", "asymp", "ne", "equiv", "le", "ge", "sub", "sup", "nsub", "sube", "supe", "oplus", "otimes", "perp", 
            "sdot", "lceil", "rceil", "lfloor", "rfloor", "lang", "rang", "loz", "spades", "clubs", "hearts", "diams"
         };

        /// <summary>
        /// 
        /// </summary>
        public static int PHONETIC_SUGGESTIONS = 1;

        /// <summary>
        /// 
        /// </summary>
        public static int HASHING_SUGGESTIONS = 2;

        private static int OPTIMIZE_FOR_SPEED = 1;
        private static int OPTIMIZE_FOR_MEMORY = 2;

        private string c_sEmailEx = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$";
        private string c_sURLEx = @"(mailto\:|(news|(ht|f)tp(s?))\://)((\S+)|(\S+)( #([^#]+)#)?)";
        private string c_sFilePathEx = @"[a-zA-Z0-9_$\-\.\\]*\\[a-zA-Z0-9_$\-\.\\]+";

        // Fields
        private UserDictionary m_UserDictionary;
        private int agreement;
        private List<object> m_arrBadWordList;
        private bool m_bCheckCompoundWords = false;
        private int m_nChosenCharPos;
        private CompareL m_compare;
        private BadWord m_CurrentBadWord = null;

        private DictionaryFile m_DictFile = null;
        private string m_sDictFilePath = null;
        private Stream m_DictFileStream = null;
        private bool m_bDictionaryIsLoaded = false;
        private char[] m_arrDictionaryWordA;
        private int m_nDistance;

        /// <summary>
        /// 
        /// </summary>
        protected string[] m_arrDontSuggest = new string[] { 
                "arse", "arsehole", "arseholes", "arses", "ass-head", "asshole", "assholes", "ass's", "bastard", "bitch", "bollocks", "bullshit", "bullshited", "bullshiting", "bullshits", "bullshitted", 
                "clit", "clits", "cock", "cocksucker", "cock-sucker", "cocksuckers", "cock-suckers", "cunt", "cunts", "dick", "dickhead", "dick's", "dildo", "dildo's", "dildos", "fart", 
                "farted", "farting", "farts", "fart's", "fuck", "fuck-all", "fuck-all's", "fucked", "fucked", "fucked-up", "fucked-up's", "fucker", "fuckers", "fucker's", "fucking", "fucks", 
                "fuck's", "fuckup", "motherfucker", "mother-fucker", "motherfuckers", "mother-fuckers", "nigger", "niggers", "nigger's", "orgies", "orgy", "orgy's", "penis", "penis's", "piss", "pissed", 
                "pisses", "pissing", "shit", "shithead", "shitheads", "shithole", "shits", "shit's", "shitted", "shitting", "shitty", "slut", "sluts", "slut's", "spic", "spics", 
                "spic's", "twat", "twatface", "twats", "twatsucker", "vagina", "vagina's", "wank", "wanked", "wanking", "wanks", "whore", "whored", "whoredom", "whores", "whore's"
             };

        private static Dictionary<string, int> m_htEntityDecimalCodes = null;

        internal bool m_bFindCapitalizedSuggestions = false;
        //private bool m_bAllowAnyCase = false;
        //private bool m_bExcludeWordsInMixedCase = false;
        //private bool m_bExcludeWordsInUpperCase = false;
        private bool m_bIgnoreRtfCodes = false;
        //private bool m_sExcludeInternetAddresses = false;
        //private bool m_bExcludeEmailAddress = false;
        //private bool m_bExcludeFileNames = false;
        //private bool m_bExcludeWordsWithNumbers = true;
        private bool m_bIncludeUserDictionaryInSuggestions = false;

        private List<object> m_arrIgnoreList;
        internal int m_nIgnorePatternEndPtr = 0;
        internal Regex[] m_arrIgnorePatternRegexs = new Regex[20];
        internal string[] m_arrIgnorePatternStrings = new string[20];

        private InvariantComparer m_InvariantComparer = new InvariantComparer();
        private bool m_bIsTaken;
        private LanguageType m_LanguageParser = LanguageType.ENGLISH;
        private string m_sLoadedQryWrd = "";
        private bool m_bLookIntoHyphenatedText = true;
        private List<object> m_arrMainDicListPhonetic;
        private List<object> m_arrMainDictionary = new List<object>();
        private int maxScore;
        private int m_nNumWords = 0;
        private int m_nOptimization = OPTIMIZE_FOR_SPEED;
        internal bool m_bOverrideEval = false;
        private int p;
        private string m_sPreviousWord = "";
        private float m_fQryLen;
        private int m_nQryLenI;
        private char[] m_arrQueryWordA;
        private int m_nRadius = 100;
        private List<object> m_arrReverseSortedDictionary = null;
        private bool m_bShareDictionary = false;
        private static List<object> m_arrSharedMainDictionary = null;
        private static List<object> m_arrsharedReverseDictionary = null;
        private int m_nSmallestDistance;
        private int m_nSuggestionsMethod = HASHING_SUGGESTIONS;
        private int m_nComplementSuggestionsMethod = -1;
        private bool m_bSuggestSplitWords = true;
        private int m_nTail;
        private int[] m_arrTaken = new int[15];
        private int m_nTally;
        /// <summary>
        /// 
        /// </summary>
        protected StringBuilder m_sText;
        private List<object> m_arrUserDicList = new List<object>();
        private bool m_bWarnDuplicates = true;

        /// <summary>
        /// 
        /// </summary>
        protected int m_nWordEnd;
        private AdvancedTextBoundary m_WordIterator = new AdvancedTextBoundary();
        /// <summary>
        /// 
        /// </summary>
        protected int m_nWordStart;

        /// <summary>
        /// 
        /// </summary>
        public SpellChecker()
        {          
            Init();

#if WPF
//            FileStream fileStream = new FileStream(@"..\Debug\Dictionary\en_us.dic", FileMode.Open, FileAccess.Read);
//            SetDictFileStream(fileStream, @"..\Debug\Dictionary\en_us.dic");
            Stream stream = Application.GetResourceStream(new Uri("/Syncfusion.Shared.WPF;component//Controls/SpellChecker/Dictionary/en_us.dic", UriKind.RelativeOrAbsolute)).Stream;
            SetDictFileStream(stream, @"/Syncfusion.Shared.WPF;component//Controls/SpellChecker/Dictionary/en_us.dic");
#endif

#if SILVERLIGHT
            Stream stream = Application.GetResourceStream(new Uri("/Syncfusion.Shared.Silverlight;component/Controls/SpellChecker/Dictionary/en_us.dic", UriKind.RelativeOrAbsolute)).Stream;
            SetDictFileStream(stream, @"/Syncfusion.Shared.Silverlight;component/Controls/SpellChecker/Dictionary/en_us.dic");
#endif
            SetupCheckerEngine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public virtual void AddIgnorePattern(string p)
        {
            m_arrBadWordList.Clear();
            m_arrIgnorePatternStrings[m_nIgnorePatternEndPtr] = p;
            m_arrIgnorePatternRegexs[m_nIgnorePatternEndPtr] = new Regex(p, RegexOptions.None);
            if (m_nIgnorePatternEndPtr < 0x13)
            {
                m_nIgnorePatternEndPtr++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public virtual bool AddWord(string word)
        {
            if (word == null)
            {
                throw new ArgumentNullException("Null String passed to AddWord() - ensure word parameter is not null.");
            }
            IgnoreAll(word);
            m_arrUserDicList.Add(word);
            if (!((this.UserDictionary != null) && this.UserDictionary.IsValid()))
            {
                return false;
            }
            if (this.LookUpUserDictionary(word))
            {
                return true;
            }
            return this.UserDictionary.AddWord(word);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newWord"></param>
        public virtual void ChangeBadWord(string newWord)
        {
            if (newWord == null)
            {
                throw new ArgumentNullException("Null String passed to ChangeBadWord() - ensure newWord parameter is not null");
            }
            if ((this.CurrentBadWord.StartPosition == -1) || (this.CurrentBadWord.EndPosition == -1))
            {
                throw new Exception("No word currently selected, use NextBadWord() first.");
            }
            m_sText.Remove(this.CurrentBadWord.StartPosition, this.CurrentBadWord.EndPosition - this.CurrentBadWord.StartPosition);
            m_sText.Insert(this.CurrentBadWord.StartPosition, newWord);
            m_WordIterator.SetText(m_sText.ToString());
            m_nWordEnd = this.CurrentBadWord.StartPosition + newWord.Length;
            m_sPreviousWord = newWord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public virtual void Check(string text)
        {
            if (text == null)
            {
                throw new Exception("Null String passed to check() - ensure text parameter is not null.");
            }
            this.Check(text, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startPosition"></param>
        public virtual void Check(string text, int startPosition)
        {
            if (text == null)
            {
                throw new ArgumentNullException("Null String passed to check() - ensure text parameter is not null.");
            }
            this.LoadLexicon();
            m_sPreviousWord = "";
            if (startPosition < 0)
            {
                startPosition = 0;
            }
            m_nWordStart = startPosition;
            m_nWordEnd = m_nWordStart;
            m_sText = new StringBuilder(text);
            m_WordIterator.SetText(text);
            this.PreCheck();
        }

        private void ClearReverseList()
        {
            lock (this)
            {
                m_arrReverseSortedDictionary = null;
                m_bDictionaryIsLoaded = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string convertHtmlEntities(string text)
        {
            return convertHtmlEntities(text, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="convertAMP"></param>
        /// <returns></returns>
        public static string convertHtmlEntities(string text, bool convertAMP)
        {
            if (m_htEntityDecimalCodes == null)
            {
                m_htEntityDecimalCodes = new Dictionary<string, int>();
                for (int i = 0; i < entities.Length; i++)
                {
                    m_htEntityDecimalCodes.Add(entities[i], DecimalCodes[i]);
                }
                DecimalCodes = null;
                entities = null;
            }
            int startIndex = -1;
            int index = -1;
            while (((startIndex + 1) < text.Length) && ((startIndex = text.IndexOf("&", (int)(startIndex + 1))) > -1))
            {
                index = text.IndexOf(";", startIndex);
                if (((index > -1) && (index < (startIndex + 10))) && (index > startIndex))
                {
                    object obj2;
                    if ((text[startIndex + 1] == '#') && (index > (startIndex + 2)))
                    {
                        if (((startIndex + 2) < text.Length) && (text[startIndex + 2] == 'x'))
                        {
                            obj2 = int.Parse(text.Substring(startIndex + 3, (index - (startIndex + 2)) - 1), NumberStyles.AllowHexSpecifier);
                        }
                        else
                        {
                            obj2 = int.Parse(text.Substring(startIndex + 2, (index - (startIndex + 1)) - 1));
                        }
                    }
                    else
                    {
                        obj2 = m_htEntityDecimalCodes[text.Substring(startIndex + 1, index - (startIndex + 1))];
                    }
                    if ((convertAMP || (obj2 == null)) || (((int)obj2) != 0x26))
                    {
                        if (obj2 != null)
                        {
                            text = text.Substring(0, startIndex) + ((char)((int)obj2)) + text.Substring(index + 1);
                        }
                        else
                        {
                            text = text.Substring(0, startIndex) + text.Substring(index + 1);
                        }
                    }
                }
            }
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            m_arrMainDictionary = null;
            m_arrMainDicListPhonetic = null;
            m_arrUserDicList = null;
            m_arrReverseSortedDictionary = null;
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~SpellChecker()
        {
            m_arrMainDictionary = null;
            m_arrMainDicListPhonetic = null;
            m_arrUserDicList = null;
            m_arrReverseSortedDictionary = null;
            this.Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editor"></param>
        public void SpellCheckForEditor(ISpellEditor editor)
        {
            if (!String.IsNullOrEmpty(editor.Text))
            {
                SpellCheckDialog dialog = new SpellCheckDialog(this, editor);
                dialog.KeyDown += new System.Windows.Input.KeyEventHandler(dialog_KeyDown);
                dialog.SpellCheckCompleted += new EventHandler(dialog_SpellCheckCompleted);
#if WPF
                if (editor.ControlToCheck != null)
                {
                    Window window = VisualUtils.FindAncestor(editor.ControlToCheck, typeof(Window)) as Window;
                    if (window != null)
                        dialog.Owner = window;
                }
#endif
                if (dialog.Suggestions != null)
                     dialog.ShowDialog();
                 else
                     dialog.OnSpellCheckCompleted();
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the dialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void dialog_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (sender is SpellCheckDialog)
                {
                    (sender as SpellCheckDialog).Close();
                }
            }
        }

        /// <summary>
        /// Handles the SpellCheckCompleted event of the dialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void dialog_SpellCheckCompleted(object sender, EventArgs e)
        {
            (sender as SpellCheckDialog).SpellCheckCompleted -= new EventHandler(dialog_SpellCheckCompleted);
            if (SpellCheckCompleted != null)
            {
                SpellCheckCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public Dictionary<WordOccurrence, ObservableCollection<string>> SpellCheck(string sKey)
        {
            Dictionary<WordOccurrence, ObservableCollection<string>> suggestionsAndWordRange;
            Check(sKey);
            suggestionsAndWordRange = CommonSpellCheck(sKey, 7);           
            return suggestionsAndWordRange;
        }

        private Dictionary<WordOccurrence, ObservableCollection<string>> CommonSpellCheck(string eargs, int nMaxSuggestions)
        {
            BadWord word;          
            Dictionary<WordOccurrence, ObservableCollection<string>> suggestionsAndWordRange = new Dictionary<WordOccurrence, ObservableCollection<string>>();
            while ((word = NextBadWord()) != null)
            {
                List<object> suggest_list = FindSuggestions();
                ObservableCollection<string> list = new ObservableCollection<string>();
                if ((0 < suggest_list.Count) && (nMaxSuggestions < suggest_list.Count))
                {
                    suggest_list = suggest_list.GetRange(0, nMaxSuggestions);
                }
                foreach (object obj in suggest_list)
                {
                    list.Add(obj.ToString());
                }
                suggestionsAndWordRange[word] = list;
            }

            return suggestionsAndWordRange;
        }

        private string ConvertArrayToClientString(IList list)
        {

            StringBuilder sbRes = new StringBuilder();
            for (int i = 1; i < list.Count; i++)
            {
                object obj = list[i];
                string sText = (null != obj) ? obj.ToString() : "No Suggesstions";
                if (sText != "")
                    sbRes.Append(sText + "\n");
            }
            return sbRes.ToString();
        }

        internal void SetupCheckerEngine()
        {
            //Resetting the settings of spell checker engine
            AllowAnyCase = false;
            ExcludeEmailAddress = true;
            ExcludeFileNames = true;
            ExcludeWordsInMixedCase = true;
            ExcludeInternetAddresses = true;
            ExcludeWordsWithNumbers = true;
            ExcludeWordsInUpperCase = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="anagrams"></param>
        public virtual void FindAnagrams(string word, IList anagrams)
        {
            int[] numArray = new int[word.Length];
            Permuter permuter = new Permuter(word.Length);
            StringBuilder builder = new StringBuilder(word, word.Length);
            for (int i = 0; i < permuter.GetNumberOfPermutations(); i++)
            {
                for (int j = 0; j < permuter.GetPermutation(i).Length; j++)
                {
                    builder[j] = word[permuter.GetPermutation(i)[j] - 1];
                }
                if (this.LookUp(builder.ToString()) && !anagrams.Contains(builder.ToString()))
                {
                    bool flag = false;
                    string str = builder.ToString().ToLower();
                    for (int k = 0; k < m_arrDontSuggest.Length; k++)
                    {
                        if (str.ToString().Equals(m_arrDontSuggest[k]))
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        anagrams.Add(builder.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="subwords"></param>
        /// <returns></returns>
        public virtual bool FindCompoundWords(string text, IList subwords)
        {
            int num = 2;
            if (this.LanguageParser == LanguageType.DUTCH)
            {
                num = 3;
            }
            for (int i = text.Length; i > num; i--)
            {
                if (this.LookUpMainDictionary(text.Substring(0, i)) || this.LookUpUserDictionary(text.Substring(0, i)))
                {
                    string query = text.Substring(i);
                    if ((this.LanguageParser == LanguageType.GERMAN) && (query.Length > 0))
                    {
                        query = char.ToUpper(query[0]) + query.Substring(1);
                    }
                    if (((text.Length - i) >= num) && (this.LookUpMainDictionary(query) || this.LookUpUserDictionary(query)))
                    {
                        if (subwords != null)
                        {
                            subwords.Add(query);
                            subwords.Add(text.Substring(0, i));
                        }
                        return true;
                    }
                    if (this.FindCompoundWords(query, subwords))
                    {
                        if (subwords != null)
                        {
                            subwords.Add(text.Substring(0, i));
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual List<object> FindSuggestions()
        {
            lock (this)
            {
                if ((this.CurrentBadWord.StartPosition == -1) || (this.CurrentBadWord.EndPosition == -1))
                {
                    throw new Exception("No word currently selected, use NextBadWord() first.");
                }

                List<object> arrSuggections = null;

                if (((m_nComplementSuggestionsMethod == HASHING_SUGGESTIONS) ||
                    (m_nComplementSuggestionsMethod == PHONETIC_SUGGESTIONS)) &&
                    (m_nComplementSuggestionsMethod != m_nSuggestionsMethod))
                {
                    int nMainSuggetionsMethod = m_nSuggestionsMethod;
                    int nComplementSuggestionsMethod = m_nComplementSuggestionsMethod;
                    arrSuggections = this.FindSuggestions(m_sText.ToString(this.CurrentBadWord.StartPosition, this.CurrentBadWord.EndPosition - this.CurrentBadWord.StartPosition));

                    this.SetSuggestionsMethod(m_nComplementSuggestionsMethod);
                    List<object> arrComplementSuggections = this.FindSuggestions(m_sText.ToString(this.CurrentBadWord.StartPosition, this.CurrentBadWord.EndPosition - this.CurrentBadWord.StartPosition));
                    this.SetSuggestionsMethod(nMainSuggetionsMethod, nComplementSuggestionsMethod);

                    int nComplementSuggetionsCount = arrComplementSuggections.Count;
                    for (int i = 0; i < arrComplementSuggections.Count; i++)
                    {
                        object oSuggection = arrComplementSuggections[i];
                        if (!arrSuggections.Contains(oSuggection))
                        {
                            arrSuggections.Add(oSuggection);
                        }
                    }
                }
                else
                {
                    arrSuggections = this.FindSuggestions(m_sText.ToString(this.CurrentBadWord.StartPosition, this.CurrentBadWord.EndPosition - this.CurrentBadWord.StartPosition));
                }

                return arrSuggections;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public virtual List<object> FindSuggestions(string word)
        {
            return this.FindSuggestions(word, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="searchLowerCase"></param>
        /// <returns></returns>
        protected virtual List<object> FindSuggestions(string word, bool searchLowerCase)
        {
            short tolerance = 700;
            if (word.Length < 4)
            {
                tolerance = 600;
                m_nRadius *= 2;
            }
            if (m_nSuggestionsMethod == PHONETIC_SUGGESTIONS)
            {
                tolerance = 600;
            }

            lock (this)
            {
                if (word == null)
                {
                    throw new ArgumentNullException("Null String passed to findSuggestions() - ensure word parameter is not null.");
                }
                this.LoadLexicon();
                List<object> anagrams = new List<object>();
                if (!word.Equals(""))
                {
                    string str;
                    int num5;
                    List<object> list2;
                    StringBuilder builder;
                    int num6;
                    string str3;
                    int num10;
                    if (this.ExcludeHtmlTags)
                    {
                        int num2;
                        int num3;
                        while (((num2 = word.IndexOf('<')) > -1) && ((num3 = word.IndexOf('>', num2)) > -1))
                        {
                            word = word.Substring(0, num2) + word.Substring(num3 + 1);
                        }
                    }
                    bool flag = false;
                    bool flag2 = true;
                    bool flag3 = true;
                    bool flag4 = false;
                    flag4 = char.ToUpper(word[0]) == char.ToLower(word[0]);
                    if (char.IsLower(word[0]))
                    {
                        flag2 = false;
                    }
                    bool flag5 = char.IsLetter(word[0]);
                    if (((word.Length > 1) && flag2) && flag5)
                    {
                        for (int i = 0; (i < word.Length) && flag3; i++)
                        {
                            flag3 = flag3 && char.IsUpper(word, i);
                        }
                    }
                    if (((flag2 && searchLowerCase) && flag5) && !flag4)
                    {
                        anagrams = this.FindSuggestions(word.ToLower());
                        for (num5 = 0; num5 < anagrams.Count; num5++)
                        {
                            if (!flag3)
                            {
                                anagrams[num5] = char.ToUpper(((string)anagrams[num5])[0]) + ((string)anagrams[num5]).Substring(1);
                            }
                            else
                            {
                                anagrams[num5] = ((string)anagrams[num5]).ToUpper();
                            }
                        }
                        if (this.SuggestSplitWords)
                        {
                            list2 = new List<object>();
                            builder = new StringBuilder("");
                            if (this.FindCompoundWords(word, list2))
                            {
                                for (num6 = list2.Count - 1; num6 >= 0; num6--)
                                {
                                    builder.Append((string)list2[num6]).Append(" ");
                                }
                                if (builder.Length > 1)
                                {
                                    builder.Remove(builder.Length - 1, 1);
                                    if (!anagrams.Contains(builder.ToString()))
                                    {
                                        anagrams.Add(builder.ToString());
                                    }
                                }
                            }
                        }
                    }
                    else if (!(((flag2 || !m_bFindCapitalizedSuggestions) || !flag5) || flag4))
                    {
                        anagrams = this.FindSuggestions(char.ToUpper(word[0]) + word.Substring(1), false);
                        anagrams.AddRange(this.FindSuggestions(word.ToUpper(), false));
                    }
                    if (m_nSuggestionsMethod == HASHING_SUGGESTIONS)
                    {
                        List<object> mainDictionary = null;
                        if (m_nOptimization == OPTIMIZE_FOR_MEMORY)
                        {
                            mainDictionary = m_arrMainDictionary;
                        }
                        else
                        {
                            if (m_arrReverseSortedDictionary == null)
                            {
                                this.LoadReverseList();
                            }
                            int radius = m_arrMainDictionary.BinarySearch(word, m_InvariantComparer);
                            if (radius < 0)
                            {
                                radius *= -1;
                            }
                            int num8 = m_arrReverseSortedDictionary.BinarySearch(word, new ReverseSorter());
                            if (num8 < 0)
                            {
                                num8 *= -1;
                            }
                            if (radius > (m_nNumWords - m_nRadius))
                            {
                                radius = m_nNumWords - m_nRadius;
                            }
                            if (radius < m_nRadius)
                            {
                                radius = m_nRadius;
                            }
                            int count = m_nRadius * 2;
                            if (count > m_arrMainDictionary.Count)
                            {
                                count = m_arrMainDictionary.Count - (radius - m_nRadius);
                            }
                            mainDictionary = new List<object>(m_arrMainDictionary.GetRange(radius - m_nRadius, count));
                            if (num8 < m_nRadius)
                            {
                                num8 = m_nRadius;
                            }
                            else if (num8 > (m_arrReverseSortedDictionary.Count - m_nRadius))
                            {
                                num8 = m_arrReverseSortedDictionary.Count - m_nRadius;
                            }
                            if (count > m_arrReverseSortedDictionary.Count)
                            {
                                count = m_arrReverseSortedDictionary.Count - (num8 - m_nRadius);
                            }
                            mainDictionary.AddRange(m_arrReverseSortedDictionary.GetRange(num8 - m_nRadius, count));
                        }
                        num5 = 0;
                        while (num5 < mainDictionary.Count)
                        {
                            if (m_nOptimization == OPTIMIZE_FOR_MEMORY)
                            {
                                str = (string)m_arrMainDictionary[num5];
                            }
                            else
                            {
                                str = (string)mainDictionary[num5];
                            }
                            if (this.IsSuggestion(str, word, tolerance))
                            {
                                if (flag2)
                                {
                                    str = this.MakeCap(str);
                                }
                                str3 = str.ToLower();
                                num10 = 0;
                                while (num10 < m_arrDontSuggest.Length)
                                {
                                    if (str3.Equals(m_arrDontSuggest[num10]))
                                    {
                                        flag = true;
                                    }
                                    num10++;
                                }
                                if (!(flag || anagrams.Contains(str)))
                                {
                                    anagrams.Add(str);
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            num5++;
                        }
                        if (m_bIncludeUserDictionaryInSuggestions)
                        {
                            for (num5 = 0; num5 < m_arrUserDicList.Count; num5++)
                            {
                                str = (string)m_arrUserDicList[num5];
                                if (this.IsSuggestion(str, word, tolerance))
                                {
                                    if (flag2)
                                    {
                                        str = this.MakeCap(str);
                                    }
                                    str3 = str.ToLower();
                                    num10 = 0;
                                    while (num10 < m_arrDontSuggest.Length)
                                    {
                                        if (str3.Equals(m_arrDontSuggest[num10]))
                                        {
                                            flag = true;
                                        }
                                        num10++;
                                    }
                                    if (!(flag || anagrams.Contains(str)))
                                    {
                                        anagrams.Add(str);
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string str4 = PhoneticsProcessor.MetaPhone(word);
                        if (m_nOptimization == OPTIMIZE_FOR_MEMORY)
                        {
                            for (num5 = 0; num5 < m_nNumWords; num5++)
                            {
                                str = (string)m_arrMainDictionary[num5];
                                if (PhoneticsProcessor.MetaPhone(str).Equals(str4))
                                {
                                    if (flag2)
                                    {
                                        str = this.MakeCap(str);
                                    }
                                    str3 = str.ToLower();
                                    num10 = 0;
                                    while (num10 < m_arrDontSuggest.Length)
                                    {
                                        if (str3.Equals(m_arrDontSuggest[num10]))
                                        {
                                            flag = true;
                                        }
                                        num10++;
                                    }
                                    if (!flag && !anagrams.Contains(str))
                                    {
                                        anagrams.Add(str);
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            num5 = 0;
                            while (num5 < m_nNumWords)
                            {
                                if (m_arrMainDicListPhonetic == null)
                                {
                                    this.GeneratePhoneticList();
                                }
                                string str2 = (string)m_arrMainDicListPhonetic[num5];
                                if (str2.Equals(str4))
                                {
                                    str = (string)m_arrMainDictionary[num5];
                                    if (flag2)
                                    {
                                        str = this.MakeCap(str);
                                    }
                                    str3 = str.ToLower();
                                    num10 = 0;
                                    while (num10 < m_arrDontSuggest.Length)
                                    {
                                        if (str3.Equals(m_arrDontSuggest[num10]))
                                        {
                                            flag = true;
                                        }
                                        num10++;
                                    }
                                    if (!flag && !anagrams.Contains(str) && this.IsSuggestion(str, word, tolerance))
                                    {
                                        anagrams.Add(str);
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                                num5++;
                            }
                        }
                        if (m_bIncludeUserDictionaryInSuggestions)
                        {
                            for (num5 = 0; num5 < m_arrUserDicList.Count; num5++)
                            {
                                str = (string)m_arrUserDicList[num5];
                                if (PhoneticsProcessor.MetaPhone(str).Equals(str4))
                                {
                                    if (flag2)
                                    {
                                        str = this.MakeCap(str);
                                    }
                                    str3 = str.ToLower();
                                    for (num10 = 0; num10 < m_arrDontSuggest.Length; num10++)
                                    {
                                        if (str3.Equals(m_arrDontSuggest[num10]))
                                        {
                                            flag = true;
                                        }
                                    }
                                    if (!flag && !anagrams.Contains(str))
                                    {
                                        anagrams.Add(str);
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                            }
                        }
                    }
                    if (this.SuggestSplitWords)
                    {
                        list2 = new List<object>();
                        builder = new StringBuilder("");
                        if (this.FindCompoundWords(word, list2) && !flag2)
                        {
                            for (num6 = list2.Count - 1; num6 >= 0; num6--)
                            {
                                builder.Append((string)list2[num6]).Append(" ");
                            }
                            if (builder.Length > 1)
                            {
                                builder.Remove(builder.Length - 1, 1);
                                if (!anagrams.Contains(builder.ToString()))
                                    anagrams.Add(builder.ToString());
                            }
                        }
                        if (m_WordIterator.V2Parser && !flag2)
                        {
                            for (num5 = 0; num5 < SimpleTextBoundary.noBreakChars.Length; num5++)
                            {
                                int num11;
                                if (((num11 = word.IndexOf(SimpleTextBoundary.noBreakChars[num5])) > -1) && (this.LookUpMainDictionary(word.Substring(0, num11)) && this.LookUpMainDictionary(word.Substring(num11 + 1))))
                                {
                                    if (!anagrams.Contains(word.Substring(0, num11 + 1) + " " + word.Substring(num11 + 1)))
                                        anagrams.Add(word.Substring(0, num11 + 1) + " " + word.Substring(num11 + 1));
                                }
                            }
                        }
                    }
                    if (word.Length <= 5)
                    {
                        this.FindAnagrams(word, anagrams);
                    }
                    if (word.Length < 4)
                    {
                        m_nRadius /= 2;
                    }
                    m_compare.with(word);
                    anagrams.Sort(m_compare);
                }
                return anagrams;
            }
        }

        private List<object> FindSuggestions(string word, bool searchLowerCase, bool returnStrings)
        {
            short num = 700;
            if (word.Length < 4)
            {
                num = 600;
            }
            int score = -1;
            lock (this)
            {
                if (word == null)
                {
                    throw new ArgumentNullException("Null String passed to findSuggestions() - ensure word parameter is not null.");
                }
                this.LoadLexicon();
                List<object> list = new List<object>();
                if (!word.Equals(""))
                {
                    string str;
                    int num6;
                    List<object> list2;
                    StringBuilder builder;
                    int num7;
                    string str3;
                    int num11;
                    if (this.ExcludeHtmlTags)
                    {
                        int num3;
                        int num4;
                        while (((num3 = word.IndexOf('<')) > -1) && ((num4 = word.IndexOf('>', num3)) > -1))
                        {
                            word = word.Substring(0, num3) + word.Substring(num4 + 1);
                        }
                    }
                    bool flag = false;
                    bool flag2 = true;
                    bool flag3 = true;
                    if (char.IsLower(word[0]))
                    {
                        flag2 = false;
                    }
                    bool flag4 = char.IsLetter(word[0]);
                    if (((word.Length > 1) && flag2) && flag4)
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            flag3 = flag3 && char.IsUpper(word, i);
                        }
                    }
                    if ((flag2 && searchLowerCase) && flag4)
                    {
                        list = this.FindSuggestions(word.ToLower(), true, false);
                        for (num6 = 0; num6 < list.Count; num6++)
                        {
                            if (!flag3)
                            {
                                ((Suggestion)list[num6]).Word = char.ToUpper(((Suggestion)list[num6]).Word[0]) + ((Suggestion)list[num6]).Word.Substring(1);
                            }
                            else
                            {
                                ((Suggestion)list[num6]).Word = ((Suggestion)list[num6]).Word.ToUpper();
                            }
                        }
                        if (this.SuggestSplitWords)
                        {
                            list2 = new List<object>();
                            builder = new StringBuilder("");
                            if (this.FindCompoundWords(word, list2))
                            {
                                for (num7 = list2.Count - 1; num7 >= 0; num7--)
                                {
                                    builder.Append((string)list2[num7]).Append(" ");
                                }
                                if (builder.Length > 1)
                                {
                                    builder.Remove(builder.Length - 1, 1);
                                    if (!Suggestion.Contains(list, builder.ToString()))
                                    {
                                        list.Add(new Suggestion(builder.ToString(), 0x3e7));
                                    }
                                }
                            }
                        }
                    }
                    else if ((!flag2 && m_bFindCapitalizedSuggestions) && flag4)
                    {
                        list = this.FindSuggestions(char.ToUpper(word[0]) + word.Substring(1), false, false);
                        list.AddRange(this.FindSuggestions(word.ToUpper(), false, false));
                    }
                    if (m_nSuggestionsMethod == HASHING_SUGGESTIONS)
                    {
                        List<object> mainDictionary = null;
                        if (m_nOptimization == OPTIMIZE_FOR_MEMORY)
                        {
                            mainDictionary = m_arrMainDictionary;
                        }
                        else
                        {
                            if (m_arrReverseSortedDictionary == null)
                            {
                                this.LoadReverseList();
                            }
                            int radius = m_arrMainDictionary.BinarySearch(word, m_InvariantComparer);
                            if (radius < 0)
                            {
                                radius *= -1;
                            }
                            int num9 = m_arrReverseSortedDictionary.BinarySearch(word, new ReverseSorter());
                            if (num9 < 0)
                            {
                                num9 *= -1;
                            }
                            if (radius > (m_nNumWords - m_nRadius))
                            {
                                radius = m_nNumWords - m_nRadius;
                            }
                            if (radius < m_nRadius)
                            {
                                radius = m_nRadius;
                            }
                            int count = m_nRadius * 2;
                            if (count > m_arrMainDictionary.Count)
                            {
                                count = m_arrMainDictionary.Count - (radius - m_nRadius);
                            }
                            mainDictionary = new List<object>(m_arrMainDictionary.GetRange(radius - m_nRadius, count));
                            if (num9 < m_nRadius)
                            {
                                num9 = m_nRadius;
                            }
                            else if (num9 > (m_arrReverseSortedDictionary.Count - m_nRadius))
                            {
                                num9 = m_arrReverseSortedDictionary.Count - m_nRadius;
                            }
                            if (count > m_arrReverseSortedDictionary.Count)
                            {
                                count = m_arrReverseSortedDictionary.Count - (num9 - m_nRadius);
                            }
                            mainDictionary.AddRange(m_arrReverseSortedDictionary.GetRange(num9 - m_nRadius, count));
                        }
                        num6 = 0;
                        while (num6 < mainDictionary.Count)
                        {
                            if (m_nOptimization == OPTIMIZE_FOR_MEMORY)
                            {
                                str = (string)m_arrMainDictionary[num6];
                            }
                            else
                            {
                                str = (string)mainDictionary[num6];
                            }
                            score = this.suggestionScore2(str, word);
                            if (score >= num)
                            {
                                if (flag2)
                                {
                                    str = this.MakeCap(str);
                                }
                                str3 = str.ToLower();
                                num11 = 0;
                                while (num11 < m_arrDontSuggest.Length)
                                {
                                    if (str3.Equals(m_arrDontSuggest[num11]))
                                    {
                                        flag = true;
                                    }
                                    num11++;
                                }
                                if (!(flag || Suggestion.Contains(list, str)))
                                {
                                    list.Add(new Suggestion(str, score));
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            num6++;
                        }
                        if (m_bIncludeUserDictionaryInSuggestions)
                        {
                            for (num6 = 0; num6 < m_arrUserDicList.Count; num6++)
                            {
                                str = (string)m_arrUserDicList[num6];
                                score = this.suggestionScore2(str, word);
                                if (score >= num)
                                {
                                    if (flag2)
                                    {
                                        str = this.MakeCap(str);
                                    }
                                    str3 = str.ToLower();
                                    num11 = 0;
                                    while (num11 < m_arrDontSuggest.Length)
                                    {
                                        if (str3.Equals(m_arrDontSuggest[num11]))
                                        {
                                            flag = true;
                                        }
                                        num11++;
                                    }
                                    if (!flag)
                                    {
                                        list.Add(new Suggestion(str, score));
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string str4 = PhoneticsProcessor.MetaPhone(word);
                        if (m_nOptimization == OPTIMIZE_FOR_MEMORY)
                        {
                            for (num6 = 0; num6 < m_nNumWords; num6++)
                            {
                                str = (string)m_arrMainDictionary[num6];
                                if (PhoneticsProcessor.MetaPhone(str).Equals(str4))
                                {
                                    if (flag2)
                                    {
                                        str = this.MakeCap(str);
                                    }
                                    str3 = str.ToLower();
                                    num11 = 0;
                                    while (num11 < m_arrDontSuggest.Length)
                                    {
                                        if (str3.Equals(m_arrDontSuggest[num11]))
                                        {
                                            flag = true;
                                        }
                                        num11++;
                                    }
                                    if (!flag)
                                    {
                                        score = this.suggestionScore2(str, word);
                                        list.Add(new Suggestion(str, score));
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            num6 = 0;
                            while (num6 < m_nNumWords)
                            {
                                if (m_arrMainDicListPhonetic == null)
                                {
                                    this.GeneratePhoneticList();
                                }
                                string str2 = (string)m_arrMainDicListPhonetic[num6];
                                if (str2.Equals(str4))
                                {
                                    str = (string)m_arrMainDictionary[num6];
                                    if (flag2)
                                    {
                                        str = this.MakeCap(str);
                                    }
                                    str3 = str.ToLower();
                                    num11 = 0;
                                    while (num11 < m_arrDontSuggest.Length)
                                    {
                                        if (str3.Equals(m_arrDontSuggest[num11]))
                                        {
                                            flag = true;
                                        }
                                        num11++;
                                    }
                                    if (!flag)
                                    {
                                        score = this.suggestionScore2(str, word);
                                        list.Add(new Suggestion(str, score));
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                                num6++;
                            }
                        }
                        if (m_bIncludeUserDictionaryInSuggestions)
                        {
                            for (num6 = 0; num6 < m_arrUserDicList.Count; num6++)
                            {
                                str = (string)m_arrUserDicList[num6];
                                if (PhoneticsProcessor.MetaPhone(str).Equals(str4))
                                {
                                    if (flag2)
                                    {
                                        str = this.MakeCap(str);
                                    }
                                    str3 = str.ToLower();
                                    for (num11 = 0; num11 < m_arrDontSuggest.Length; num11++)
                                    {
                                        if (str3.Equals(m_arrDontSuggest[num11]))
                                        {
                                            flag = true;
                                        }
                                    }
                                    if (!flag)
                                    {
                                        score = this.suggestionScore2(str, word);
                                        list.Add(new Suggestion(str, score));
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                            }
                        }
                    }
                    if (this.SuggestSplitWords)
                    {
                        list2 = new List<object>();
                        builder = new StringBuilder("");
                        if (this.FindCompoundWords(word, list2) && !flag2)
                        {
                            for (num7 = list2.Count - 1; num7 >= 0; num7--)
                            {
                                builder.Append((string)list2[num7]).Append(" ");
                            }
                            if (builder.Length > 1)
                            {
                                builder.Remove(builder.Length - 1, 1);
                                list.Add(new Suggestion(builder.ToString(), 0x3e7));
                            }
                        }
                        if (m_WordIterator.V2Parser && !flag2)
                        {
                            for (num6 = 0; num6 < SimpleTextBoundary.noBreakChars.Length; num6++)
                            {
                                int num12;
                                if (((num12 = word.IndexOf(SimpleTextBoundary.noBreakChars[num6])) > -1) && (this.LookUpMainDictionary(word.Substring(0, num12)) && this.LookUpMainDictionary(word.Substring(num12 + 1))))
                                {
                                    list.Add(new Suggestion(word.Substring(0, num12 + 1) + " " + word.Substring(num12 + 1), 0x3e6));
                                }
                            }
                        }
                    }
                    if (word.Length <= 5)
                    {
                        List<object> anagrams = new List<object>();
                        this.FindAnagrams(word, anagrams);
                        foreach (string str5 in anagrams)
                        {
                            score = this.suggestionScore2(str5, word);
                            list.Add(new Suggestion(str5, score));
                        }
                    }
                    m_compare.with(word);
                    list.Sort(m_compare);
                    if (returnStrings)
                    {
                        for (num6 = 0; num6 < list.Count; num6++)
                        {
                            list[num6] = ((Suggestion)list[num6]).Word;
                        }
                    }
                }
                return list;
            }
        }

        internal bool Flagged(string word)
        {
            return m_arrBadWordList.Contains(word);
        }

        private void GeneratePhoneticList()
        {
            m_arrMainDicListPhonetic = new List<object>();
            for (int i = 0; i < m_nNumWords; i++)
            {
                m_arrMainDicListPhonetic.Add(PhoneticsProcessor.MetaPhone((string)m_arrMainDictionary[i]));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string GetAmendedText()
        {
            if (m_sText != null)
            {
                return m_sText.ToString();
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetConsiderationRange()
        {
            return m_nRadius;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetIncludeUserDictionaryInSuggestions()
        {
            return m_bIncludeUserDictionaryInSuggestions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal virtual string GetNextWord()
        {
            m_nWordStart = m_nWordEnd;
            if (m_nWordEnd < m_WordIterator.Last())
            {
                m_nWordEnd = m_WordIterator.Following(m_nWordEnd);
                if ((((m_nWordEnd - m_nWordStart) <= 0) || !char.IsLetterOrDigit(m_sText[m_nWordStart])) || ((m_bIgnoreRtfCodes && (m_nWordStart > 0)) && (m_sText[m_nWordStart - 1] == '\\')))
                {
                    return this.GetNextWord();
                }
                return m_sText.ToString(m_nWordStart, m_nWordEnd - m_nWordStart);
            }
            return null;
        }

        internal string GetPreviousWord()
        {
            m_nWordEnd = m_nWordStart;
            if (m_nWordStart > 0)
            {
                m_nWordStart = m_WordIterator.Preceding(m_nWordStart);
                if (m_nWordStart < 0)
                {
                    return null;
                }
                if ((((m_nWordEnd - m_nWordStart) <= 0) || !char.IsLetterOrDigit(m_sText[m_nWordStart])) || ((m_bIgnoreRtfCodes && (m_nWordStart > 0)) && (m_sText.ToString()[m_nWordStart - 1] == '\\')))
                {
                    return this.GetPreviousWord();
                }
                return m_sText.ToString(m_nWordStart, m_nWordEnd - m_nWordStart);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetSeparateHyphenWords()
        {
            return m_WordIterator.SeparateHyphenWords;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int GetSuggestionsMethod()
        {
            return m_nSuggestionsMethod;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        public virtual void IgnoreAll(string word)
        {
            if (word == null)
            {
                throw new ArgumentNullException("Null String passed to ignoreAll() - ensure word parameter is not null.");
            }
            m_arrIgnoreList.Add(word);
        }

        private void Init()
        {
            m_compare = new CompareL(this);
            m_arrBadWordList = new List<object>();
            m_arrIgnoreList = new List<object>();
        }

        private bool isAllCaps(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsLower(s[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool isMixedCase(string s)
        {
            bool flag = false;
            bool flag2 = false;
            int num = char.IsUpper(s[0]) ? 1 : 0;
            for (int i = num; i < s.Length; i++)
            {
                if (char.IsLower(s[i]))
                {
                    flag2 = true;
                    if (flag)
                    {
                        return true;
                    }
                }
                else if (char.IsUpper(s[i]))
                {
                    flag = true;
                    if (flag2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsSuggestion(string dictionaryWord, string queryWord, short tolerance)
        {
            return (this.suggestionScore2(dictionaryWord, queryWord) >= tolerance);
        }

        /// <exclude/>
        internal static void EnsureLoadDictionary()
        {
            SpellChecker spellcheckEngine = new SpellChecker();
                spellcheckEngine.DictionaryPath = @"..\..\Dictionary\en_us.dic";
                spellcheckEngine.LoadLexicon();
                spellcheckEngine.LoadReverseList();
        }


        private void LoadLexicon()
        {
            lock (this)
            {
                if (!m_bDictionaryIsLoaded)
                {
                    m_arrMainDictionary.Clear();
                    m_arrBadWordList.Clear();
                    if (m_bShareDictionary && (m_arrSharedMainDictionary == null))
                    {
                        m_arrSharedMainDictionary = new List<object>();
                        m_arrSharedMainDictionary = m_DictFile.ReadWordListStream(m_arrSharedMainDictionary, DictionaryFile.WordList, "UTF8");
                        m_nNumWords = m_arrSharedMainDictionary.Count;
                    }
                    else if (!m_bShareDictionary)
                    {
                        m_arrMainDictionary = m_DictFile.ReadWordListStream(m_arrMainDictionary, DictionaryFile.WordList, "UTF8");
                        m_nNumWords = m_arrMainDictionary.Count;
                    }
                    else
                    {
                        m_nNumWords = m_arrSharedMainDictionary.Count;
                    }
                    m_bDictionaryIsLoaded = true;
                    if (m_bShareDictionary)
                    {
                        m_arrMainDictionary = m_arrSharedMainDictionary;
                    }
                }
            }
        }

        private void LoadReverseList()
        {
            lock (this)
            {
                if (m_arrReverseSortedDictionary == null)
                {
                    if (!m_bShareDictionary)
                    {
                        m_arrReverseSortedDictionary = new List<object>();
                    }
                    int count = 0;
                    if (m_bShareDictionary && (m_arrsharedReverseDictionary == null))
                    {
                        m_arrsharedReverseDictionary = new List<object>();
                        m_arrsharedReverseDictionary = m_DictFile.ReadWordListStream(m_arrsharedReverseDictionary, DictionaryFile.ReverseList, "UTF8");
                        count = m_arrsharedReverseDictionary.Count;
                    }
                    else if (!m_bShareDictionary)
                    {
                        m_arrReverseSortedDictionary = m_DictFile.ReadWordListStream(m_arrReverseSortedDictionary, DictionaryFile.ReverseList, "UTF8");
                        count = m_arrReverseSortedDictionary.Count;
                    }
                    else
                    {
                        count = m_arrsharedReverseDictionary.Count;
                    }
                    if (m_nNumWords != count)
                    {
                        throw new Exception(string.Concat(new object[] { "rlist.Length (", count, ") != wlist.Length (", m_nNumWords, ")" }));
                    }
                    if (m_bShareDictionary)
                    {
                        m_arrReverseSortedDictionary = m_arrsharedReverseDictionary;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual bool LookUp(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("Null String passed to LookUp() - ensure query parameter is not null.");
            }
            lock (this)
            {
                return (this.LookUpMainDictionary(query) || (this.LookUpUserDictionary(query) || (CheckCompoundWords && this.FindCompoundWords(query, null))));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual bool LookUpMainDictionary(string query)
        {
            this.LoadLexicon();
            query = query.Replace("'", "\'");
            query = query.Replace("" + '\x001f', "");
            if (query.Length <= 1)
            {
                return true;
            }
            if (this.ExcludeHtmlTags)
            {
                int num;
                int num2;
                while (((num = query.IndexOf('<')) > -1) && ((num2 = query.IndexOf('>', num)) > -1))
                {
                    query = query.Substring(0, num) + query.Substring(num2 + 1);
                }
            }
            if (AllowAnyCase)
            {
                query = char.ToUpper(query[0]) + query.Substring(1).ToLower();
            }
            if (this.isAllCaps(query))
            {
                if (ExcludeWordsInUpperCase)
                {
                    return true;
                }
                if (m_arrMainDictionary.BinarySearch(query, m_InvariantComparer) >= 0)
                {
                    return true;
                }
                query = query[0] + query.Substring(1).ToLower();
            }
            if (m_arrMainDictionary.BinarySearch(query, m_InvariantComparer) >= 0)
            {
                return true;
            }
            if (!(this.isMixedCase(query) && !ExcludeWordsInMixedCase) && (m_arrMainDictionary.BinarySearch(query.ToLower(), m_InvariantComparer) >= 0))
            {
                return true;
            }

            if (m_WordIterator.V2Parser)
            {
                for (int i = 0; i < 3; i++)
                {
                    int num3;
                    if ((((num3 = query.IndexOf(@"/\-"[i])) > -1) && ((query[num3] != '-') || m_bLookIntoHyphenatedText)) && (this.LookUpMainDictionary(query.Substring(0, num3)) && this.LookUpMainDictionary(query.Substring(num3 + 1))))
                    {
                        return true;
                    }
                }
                for (int j = 0; j < m_nIgnorePatternEndPtr; j++)
                {
                    if ((query.Length < 100) && m_arrIgnorePatternRegexs[j].IsMatch(query))
                    {
                        return true;
                    }
                }
            }
            else
            {
                int num6;
                if (m_bLookIntoHyphenatedText && ((num6 = query.IndexOf('-')) > -1))
                {
                    return (this.LookUpMainDictionary(query.Substring(0, num6)) && this.LookUpMainDictionary(query.Substring(num6 + 1)));
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual bool LookUpUserDictionary(string query)
        {
            if (query != null)
            {
                if (query.Length <= 1)
                {
                    return true;
                }
                if (this.ExcludeHtmlTags)
                {
                    int num;
                    int num2;
                    while (((num = query.IndexOf('<')) > -1) && ((num2 = query.IndexOf('>', num)) > -1))
                    {
                        query = query.Substring(0, num) + query.Substring(num2 + 1);
                    }
                }
                if ((this.UserDictionary != null) && this.UserDictionary.IsValid())
                {
                    for (int i = 0; i < m_arrUserDicList.Count; i++)
                    {
                        if (((string)m_arrUserDicList[i]).Equals(query) || (!query.Equals(query.ToLower()) && ((string)m_arrUserDicList[i]).Equals(query.ToLower())))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private string MakeCap(string word)
        {
            if (word.Length > 1)
            {
                word = word.ToUpper()[0] + word.Substring(1);
                return word;
            }
            word = word.ToUpper();
            return word;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual BadWord NextBadWord()
        {
            bool flag = false;
            int count = 0;
            string nextWord = this.GetNextWord();
            int caretStart = -1;
            int caretEnd = -1;
            m_CurrentBadWord = null;
            if (this.WarnDuplicates && ((m_sPreviousWord == null) || (m_sPreviousWord == "")))
            {
                int wordStart = m_nWordStart;
                int wordEnd = m_nWordEnd;
                m_sPreviousWord = this.GetPreviousWord();
                m_nWordStart = wordStart;
                m_nWordEnd = wordEnd;
            }
            if (nextWord == null)
            {
                return null;
            }
            if (!(((!this.WarnDuplicates || (m_sPreviousWord == null)) || !m_sPreviousWord.ToLower().Equals(nextWord.ToLower())) || this.WordIsDigits(m_sPreviousWord)))
            {
                count = (m_nWordStart <= 4) ? m_nWordStart : 4;
                flag = ((m_sText[m_nWordStart - 1] == ' ') && (m_sText.ToString().LastIndexOf('.', m_nWordStart, count) == -1)) && (m_sText.ToString().LastIndexOf(',', m_nWordStart, count) == -1);
            }
        Label_01F7: ;
            if ((!this.Flagged(nextWord) || m_arrIgnoreList.Contains(nextWord)) && !flag)
            {
                m_sPreviousWord = nextWord;
                nextWord = this.GetNextWord();
                if (nextWord == null)
                {
                    return null;
                }
                if (!((!this.WarnDuplicates || !m_sPreviousWord.ToLower().Equals(nextWord.ToLower())) || this.WordIsDigits(m_sPreviousWord)))
                {
                    count = (m_nWordStart <= 4) ? m_nWordStart : 4;
                    flag = ((m_sText[m_nWordStart - 1] == ' ') && (m_sText.ToString().LastIndexOf('.', m_nWordStart, count) == -1)) && (m_sText.ToString().LastIndexOf(',', m_nWordStart, count) == -1);
                }
                goto Label_01F7;
            }
            m_sPreviousWord = nextWord;
            caretStart = m_nWordStart;
            caretEnd = m_nWordEnd;
            if (this.ExcludeHtmlTags)
            {
                int num6;
                int num7;
                while (((num6 = nextWord.IndexOf('<')) > -1) && ((num7 = nextWord.IndexOf('>', num6)) > -1))
                {
                    nextWord = nextWord.Substring(0, num6) + nextWord.Substring(num7 + 1);
                }
            }
            if (!flag)
            {
                m_CurrentBadWord = new BadWord(nextWord, caretStart, caretEnd);
            }
            else
            {
                m_CurrentBadWord = new BadWord(nextWord, caretStart, caretEnd, BadWord.REASON_DUPLICATE);
            }
            return m_CurrentBadWord;
        }

        private void PreCheck()
        {
            string current;
            m_arrBadWordList.Clear();
            while ((current = this.GetNextWord()) != null)
            {
                if ((!m_arrBadWordList.Contains(current) && !this.LookUpMainDictionary(current)) && ((!this.ExcludeWordsWithNumbers || !this.WordHasDigits(current)) && !this.WordIsDigits(current)))
                {
                    m_arrBadWordList.Add(current);
                }
            }
            this.Reset();
            if (m_arrBadWordList.Count > 0)
            {
                IEnumerator enumerator = m_arrBadWordList.ToArray().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (string)enumerator.Current;
                    if (this.LookUpUserDictionary(current))
                    {
                        m_arrBadWordList.Remove(current);
                    }
                    else if (CheckCompoundWords && this.FindCompoundWords(current, null))
                    {
                        m_arrBadWordList.Remove(current);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public virtual void RemoveIgnorePattern(string p)
        {
            m_arrBadWordList.Clear();
            List<object> list = new List<object>(m_arrIgnorePatternStrings);
            List<object> list2 = new List<object>(m_arrIgnorePatternRegexs);
            int index = list.IndexOf(p);
            if (index >= 0)
            {
                list.RemoveAt(index);
                list2.RemoveAt(index);
                list.CopyTo(m_arrIgnorePatternStrings);
                list2.CopyTo(m_arrIgnorePatternRegexs);
                if (m_nIgnorePatternEndPtr > 0)
                {
                    m_nIgnorePatternEndPtr--;
                }
            }
        }

        private void Reset()
        {
            this.SetPosition(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        public void SetConsiderationRange(int w)
        {
            if (w > 1)
            {
                m_nRadius = w;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictFileStream"></param>
        /// <param name="sKey"></param>
        public virtual void SetDictFileStream(Stream dictFileStream, string sKey)
        {
            SetDictFileStream(dictFileStream, null, sKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictFileStream"></param>
        /// <param name="dictReverseFileStream"></param>
        /// <param name="sKey"></param>
        public virtual void SetDictFileStream(Stream dictFileStream, Stream dictReverseFileStream, string sKey)
        {
            lock (this)
            {
                if (m_bDictionaryIsLoaded)
                {
                    throw new InvalidOperationException("SetDictFileStream called after dictionary has been loaded.");
                }
                m_DictFileStream = dictFileStream;
                if (dictFileStream != null)
                {
                    m_DictFile = new DictionaryFile(dictFileStream, dictReverseFileStream, sKey);
                }
                else
                {
                    m_DictFile = null;
                }
                m_arrReverseSortedDictionary = null;
                m_bDictionaryIsLoaded = false;
                this.LoadLexicon();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void SetIncludeUserDictionaryInSuggestions(bool v)
        {
            m_bIncludeUserDictionaryInSuggestions = v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        public virtual void SetPosition(int pos)
        {
            if (pos < 0)
            {
                pos = 0;
            }
            if (pos > m_sText.Length)
            {
                pos = m_sText.Length;
            }
            m_nWordEnd = pos;
            m_sPreviousWord = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        public void SetSeparateHyphenWords(bool f)
        {
            m_WordIterator.SeparateHyphenWords = f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="complementMethod"></param>
        public void SetSuggestionsMethod(int method, int complementMethod)
        {
            SetSuggestionsMethod(method);
            if ((method != complementMethod) &&
                ((complementMethod == PHONETIC_SUGGESTIONS) || (complementMethod == HASHING_SUGGESTIONS)))
            {
                m_nComplementSuggestionsMethod = complementMethod;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public void SetSuggestionsMethod(int method)
        {
            if ((method == PHONETIC_SUGGESTIONS) || (method == HASHING_SUGGESTIONS))
            {
                m_nSuggestionsMethod = method;
                m_nComplementSuggestionsMethod = -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDictionary"></param>
        public void SetUserDictionary(UserDictionary userDictionary)
        {
            if (userDictionary == null)
            {
                throw new ArgumentNullException("Null UserDictionary object passed to SetUserDictionary().");
            }
            this.UserDictionary = userDictionary;
            this.UserDictionary.ReadAll(m_arrUserDicList);
        }

        /// <summary>
        /// Loads the user dictionary from IsolatedStorage
        /// </summary>
        /// <param name="userDictionaryFile"></param>
        public void SetUserDictionary(string userDictionaryFile)
        {
            if ((userDictionaryFile == null) || userDictionaryFile.Equals(""))
            {
                this.UserDictionary = null;
            }
            else
            {
                this.UserDictionary = new UserDictionary(userDictionaryFile);
                this.UserDictionary.ReadAll(m_arrUserDicList);
            }
        }

        private int suggestionScore2(string dictionaryWord, string queryWord)
        {
            if (!((m_sLoadedQryWrd != null) && m_sLoadedQryWrd.Equals(queryWord)))
            {
                m_sLoadedQryWrd = queryWord;
                m_nQryLenI = queryWord.Length;
                m_fQryLen = m_nQryLenI;
                m_arrQueryWordA = queryWord.ToCharArray();
            }
            this.maxScore = dictionaryWord.Length;
            m_nTail = this.maxScore;
            if (this.maxScore >= m_nQryLenI)
            {
                if ((this.maxScore - m_nQryLenI) > (m_fQryLen * 0.5))
                {
                    return 0;
                }
            }
            else if ((m_nQryLenI - this.maxScore) > (m_fQryLen * 0.5))
            {
                return 0;
            }
            m_nTally = 0;
            for (int i = 0; i < m_nQryLenI; i++)
            {
                if (dictionaryWord.IndexOf(m_arrQueryWordA[i]) > -1)
                {
                    m_nTally++;
                }
            }

            double d = (m_nSuggestionsMethod == PHONETIC_SUGGESTIONS) ? 0.4 : 0.38;
            if (m_nTally < (m_nQryLenI - (m_nQryLenI * d)))
            {
                return 0;
            }
            return this.SuggestionScore2b(dictionaryWord, queryWord);
        }

        internal int SuggestionScore2b(string dictionaryWord, string queryWord)
        {
            m_arrDictionaryWordA = dictionaryWord.ToCharArray();
            if (!((m_sLoadedQryWrd != null) && m_sLoadedQryWrd.Equals(queryWord)))
            {
                m_sLoadedQryWrd = queryWord;
                m_nQryLenI = (short)queryWord.Length;
                m_fQryLen = m_nQryLenI;
                m_arrQueryWordA = queryWord.ToCharArray();
            }
            this.maxScore = dictionaryWord.Length;
            m_nTail = this.maxScore;
            this.agreement = 0;
            m_arrTaken = new int[this.maxScore];
            this.p = 0;
            for (int i = 0; i < this.maxScore; i++)
            {
                m_nSmallestDistance = m_nTail;
                for (int j = 0; (j < m_nQryLenI) && (m_nSmallestDistance != 0); j++)
                {
                    m_bIsTaken = false;
                    if ((m_arrDictionaryWordA[i] == m_arrQueryWordA[j]) || ((m_bFindCapitalizedSuggestions && (char.IsLower(m_arrDictionaryWordA[i]) == char.IsLower(m_arrQueryWordA[j]))) && (string.Compare(dictionaryWord, i, queryWord, j, 1,StringComparison.InvariantCultureIgnoreCase) == 0)))
                    {
                        for (int k = 0; k < this.p; k++)
                        {
                            if (m_arrTaken[k] == j)
                            {
                                m_bIsTaken = true;
                            }
                        }
                        if (!m_bIsTaken)
                        {
                            if (j > i)
                            {
                                m_nDistance = j - i;
                            }
                            else
                            {
                                m_nDistance = i - j;
                            }
                            if (m_nDistance < m_nSmallestDistance)
                            {
                                m_nSmallestDistance = m_nDistance;
                                m_nChosenCharPos = j;
                            }
                        }
                    }
                }
                m_nSmallestDistance *= m_nSmallestDistance;
                if (m_nSmallestDistance > m_nTail)
                {
                    m_nSmallestDistance = m_nTail;
                }
                if (m_nSmallestDistance != m_nTail)
                {
                    m_arrTaken[this.p] = m_nChosenCharPos;
                    if (this.p < 15)
                    {
                        this.p++;
                    }
                    this.agreement += (0x3e8 * (m_nTail - m_nSmallestDistance)) / m_nTail;
                }
            }
            return (((this.agreement / m_nQryLenI) + (this.agreement / this.maxScore)) / 2);
        }

        private bool WordHasDigits(string w)
        {
            for (int i = 0; i < w.Length; i++)
            {
                if (char.IsDigit(w[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool WordIsDigits(string w)
        {
            for (int i = 0; i < w.Length; i++)
            {
                if (!(char.IsDigit(w[i]) || (w[i] == '.')))
                {
                    return false;
                }
            }
            return true;
        }

        // Properties
        internal int _WordEnd
        {
            get
            {
                return m_nWordEnd;
            }
        }

        internal int _WordStart
        {
            get
            {
                return m_nWordStart;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowAnyCase
        {
            get
            {
                return (bool)GetValue(AllowAnyCaseProperty);
            }
            set
            {
                SetValue(AllowAnyCaseProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AllowAnyCaseProperty = DependencyProperty.Register("AllowAnyCase", typeof(bool), typeof(SpellChecker), new PropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public bool ExcludeWordsInMixedCase
        {
            get
            {
                return (bool)GetValue(ExcludeWordsInMixedCaseProperty);
            }
            set
            {
                SetValue(ExcludeWordsInMixedCaseProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ExcludeWordsInMixedCaseProperty = DependencyProperty.Register("ExcludeWordsInMixedCase", typeof(bool), typeof(SpellChecker), new PropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public bool ExcludeWordsInUpperCase
        {
            get
            {
                return (bool)GetValue(ExcludeWordsInUpperCaseProperty);
            }
            set
            {
                SetValue(ExcludeWordsInUpperCaseProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ExcludeWordsInUpperCaseProperty = DependencyProperty.Register("ExcludeWordsInUpperCase", typeof(bool), typeof(SpellChecker), new PropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public bool CheckCompoundWords
        {
            get
            {
                return m_bCheckCompoundWords;
            }
            set
            {
                m_bCheckCompoundWords = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BadWord CurrentBadWord
        {
            get
            {
                return m_CurrentBadWord;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DictionaryPath
        {
            get
            {
                return m_sDictFilePath;
            }
            set
            {
                if (m_sDictFilePath != value)
                {
                    lock (this)
                    {
                        m_sDictFilePath = value;
                        if ((m_sDictFilePath != null) && (m_sDictFilePath != ""))
                        {
                            m_DictFile = new DictionaryFile(m_sDictFilePath);
                        }
                        else
                        {
                            m_DictFile = null;
                        }
                        m_arrReverseSortedDictionary = null;
                        m_arrMainDicListPhonetic = null;
                        m_bDictionaryIsLoaded = false;
                        this.LoadLexicon();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FindCapitalizedSuggestions
        {
            get
            {
                return m_bFindCapitalizedSuggestions;
            }
            set
            {
                m_bFindCapitalizedSuggestions = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<object> IgnoreList
        {
            get
            {
                return m_arrIgnoreList;
            }
            set
            {
                m_arrIgnoreList = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreRtfCodes
        {
            get
            {
                return m_bIgnoreRtfCodes;
            }
            set
            {
                m_bIgnoreRtfCodes = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExcludeInternetAddresses
        {
            get
            {
                return (bool)GetValue(ExcludeInternetAddressesProperty);
            }
            set
            {
                SetValue(ExcludeInternetAddressesProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ExcludeInternetAddressesProperty = DependencyProperty.Register("ExcludeInternetAddresses", typeof(bool), typeof(SpellChecker), new PropertyMetadata(false, new PropertyChangedCallback(OnExcludeInternetAddressesChanged)));

        private static void OnExcludeInternetAddressesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpellChecker spellCheker = (SpellChecker)obj;
            spellCheker.OnExcludeInternetAddressesChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnExcludeInternetAddressesChanged(DependencyPropertyChangedEventArgs args)
        {
            if (!(!(bool)args.NewValue || (bool)args.OldValue))
            {
                AddIgnorePattern(c_sURLEx);
            }
            else if (!((bool)args.NewValue || !(bool)args.OldValue))
            {
                RemoveIgnorePattern(c_sURLEx);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExcludeEmailAddress
        {
            get
            {
                return (bool)GetValue(ExcludeEmailAddressProperty);
            }
            set
            {
                SetValue(ExcludeEmailAddressProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ExcludeEmailAddressProperty = DependencyProperty.Register("ExcludeEmailAddress", typeof(bool), typeof(SpellChecker), new PropertyMetadata(false, new PropertyChangedCallback(OnExcludeEmailAddressChanged)));

        private static void OnExcludeEmailAddressChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpellChecker spellCheker = (SpellChecker)obj;
            spellCheker.OnExcludeEmailAddressChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnExcludeEmailAddressChanged(DependencyPropertyChangedEventArgs args)
        {
            if (!(!(bool)args.NewValue || (bool)args.OldValue))
            {
                AddIgnorePattern(c_sEmailEx);
            }
            else if (!((bool)args.NewValue || !(bool)args.OldValue))
            {
                RemoveIgnorePattern(c_sEmailEx);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExcludeFileNames
        {
            get
            {
                return (bool)GetValue(ExcludeFileNamesProperty);
            }
            set
            {
                SetValue(ExcludeFileNamesProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ExcludeFileNamesProperty = DependencyProperty.Register("ExcludeFileNames", typeof(bool), typeof(SpellChecker), new PropertyMetadata(false, new PropertyChangedCallback(OnExcludeFileNamesChanged)));

        private static void OnExcludeFileNamesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpellChecker spellCheker = (SpellChecker)obj;
            spellCheker.OnExcludeFileNamesChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnExcludeFileNamesChanged(DependencyPropertyChangedEventArgs args)
        {
            if (!(!(bool)args.NewValue || (bool)args.OldValue))
            {
                AddIgnorePattern(c_sFilePathEx);
            }
            else if (!((bool)args.NewValue || !(bool)args.OldValue))
            {
                RemoveIgnorePattern(c_sFilePathEx);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExcludeWordsWithNumbers
        {
            get
            {
                return (bool)GetValue(ExcludeWordsWithNumbersProperty);
            }
            set
            {
                SetValue(ExcludeWordsWithNumbersProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ExcludeWordsWithNumbersProperty = DependencyProperty.Register("ExcludeWordsWithNumbers", typeof(bool), typeof(SpellChecker), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public bool ExcludeHtmlTags
        {
            get
            {
                return m_WordIterator.IgnoreXML;
            }
            set
            {
                m_WordIterator.IgnoreXML = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LanguageType LanguageParser
        {
            get
            {
                return m_LanguageParser;
            }
            set
            {
                if (value == LanguageType.ENGLISH)
                {
                    m_WordIterator.LanguageParsing = SimpleTextBoundary.ENGLISH;
                }
                if (value == LanguageType.FRENCH)
                {
                    m_WordIterator.LanguageParsing = SimpleTextBoundary.FRENCH;
                }
                if (value == LanguageType.DUTCH)
                {
                    m_WordIterator.LanguageParsing = SimpleTextBoundary.FRENCH;
                }
                if (value == LanguageType.GERMAN)
                {
                    m_WordIterator.LanguageParsing = SimpleTextBoundary.GERMAN;
                }
                m_LanguageParser = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool LookIntoHyphenatedText
        {
            get
            {
                return m_bLookIntoHyphenatedText;
            }
            set
            {
                m_bLookIntoHyphenatedText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShareDictionary
        {
            get
            {
                return m_bShareDictionary;
            }
            set
            {
                m_bShareDictionary = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SuggestSplitWords
        {
            get
            {
                return m_bSuggestSplitWords;
            }
            set
            {
                m_bSuggestSplitWords = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AdvancedTextBoundary TextBoundary
        {
            get
            {
                return m_WordIterator;
            }
            set
            {
                m_WordIterator = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UserDictionary UserDictionary
        {
            get
            {
                return m_UserDictionary;
            }
            set
            {
                m_UserDictionary = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool V2Parser
        {
            get
            {
                return m_WordIterator.V2Parser;
            }
            set
            {
                m_WordIterator.V2Parser = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool WarnDuplicates
        {
            get
            {
                return m_bWarnDuplicates;
            }
            set
            {
                m_bWarnDuplicates = value;
            }
        }


    }
}
