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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using Syncfusion.Runtime.Serialization;

namespace Syncfusion.Text
{
    [
    ToolboxItem(true),
    Description("Provides spell checking fucntionality."),
    ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.SpellChecker.bmp")
    ]
    public class SpellChecker : Component
    {
        private enum ECaseUpperWord
        {
            AllUpper,
            AllLower,
            FirstUpper
        }

        #region  private Fields

        private static String DEF_CUSTOM_DIC_PATH = Application.CommonAppDataPath + Path.DirectorySeparatorChar + "Custom_Dictionay.dic";

        private static String DEF_DIC_NAME = "Syncfusion_en_us.dic";

        private bool CustomDictionayCacheDirty = false;

        private SpellCheckerDialog spellCheckerDialog;

        private OptionsDialog optionsDialog;

        private CustomDictionaryEditor customDictionaryEditor;

        private ISpellEditor editor = null;

        private static Hashtable htdictPathVsEntriesList = new Hashtable();

        private static Hashtable htdictPathVsSoundXList = new Hashtable();

        /// <summary>
        /// Concatenates the suggestion and wrong words, and returns it to the js file. 
        /// </summary>
        /// <remarks>String returned to js file that has both the suggestion & wrong words.</remarks>
        StringBuilder error_string;
        /// <summary>
        /// Specifies the path of the dictionary. 
        /// </summary>
        private string dictionaryPath = DEF_DIC_NAME;
        /// <summary>
        /// Specifies the path of the custom dictionary. 
        /// </summary>
        private string customDictionaryPath = DEF_CUSTOM_DIC_PATH;
        /// <summary>
        /// ArrayList to hold the wrong words.
        /// </summary>
        ArrayList misspelledWords = new ArrayList();     
        [Syncfusion.Documentation.DocumentationExclude()]
        ArrayList repeatwordsArray = new ArrayList();
        /// <summary>
        /// Specifies whether to ignore internet address during spell check.
        /// </summary>
        private bool excludeInternetAddresses;
        /// <summary>
        /// Specifies to ignore email address during spell check.
        /// </summary>
        private bool excludeEmailAddress;
        /// <summary>
        /// Specifies to ignore html tags during spell check.
        /// </summary>
        private bool excludeHtmlTags;
        /// <summary>
        /// Specifies to ignore file names during spell check.
        /// </summary>
        private bool excludeFileNames;
        /// <summary>
        /// Specifies to ignore words in mixed case during spell check.
        /// </summary>
        private bool excludeWordsInMixedCase;
        /// <summary>
        /// Specifies to ignore words in upper case during spell check.
        /// </summary>
        private bool excludeWordsInUpperCase;
        /// <summary>
        /// Specifies to ignore words with number during spell check.
        /// </summary>
        private bool excludeWordsWithNumbers;
        /// <summary>
        /// Specifies to ignore words repeated words during spell check.
        /// </summary>
        private bool excludeRepeatedWords ;
        /// <summary>
        /// Allows to customize the number of suggestions to be displayed in the suggestion list.
        /// </summary>
        private int maxSuggestions = 20;
        /// <summary>
        /// Specifies to ignore special symbols during spell check.
        /// </summary>
        private bool excludeSpecialSymbols = false;
        /// <summary>
        /// Stream that contains the dictionary.
        /// </summary>
        private Stream dictionaryStream = null;
        /// <summary>
        /// SpellCheckerDialog.
        /// </summary>

        #endregion

        #region Properties
        internal ISpellEditor Editor
        {
            get { return editor; }
            set { editor = value; }
        }

        internal CustomDictionaryEditor CustomDictionaryEditor
        {
            get { return customDictionaryEditor; }
            set { customDictionaryEditor = value; }
        }

        internal SpellCheckerDialog SpellCheckerDialog
        {
            get { return spellCheckerDialog; }
            set { spellCheckerDialog = value; }
        }

        internal OptionsDialog OptionsDialog
        {
            get { return optionsDialog; }
            set { optionsDialog = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying whether to ignore repeated words during spell check.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying whether to ignore repeated words during spell check."),
        Browsable(true),DefaultValueAttribute(false)]
        public bool ExcludeRepeatedWords
        {
            get { return excludeRepeatedWords; }
            set { excludeRepeatedWords = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying whether to ignore words in mixed case during spell check.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying whether to ignore words in mixed case during spell check."),
        Browsable(true), DefaultValueAttribute(false)]
        public bool ExcludeWordsInMixedCase
        {
            get { return excludeWordsInMixedCase; }
            set { excludeWordsInMixedCase = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying whether to ignore words in upper case during spell check.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying whether to ignore words in upper case during spell check."),
        Browsable(true), DefaultValueAttribute(false)]
        public bool ExcludeWordsInUpperCase
        {
            get { return excludeWordsInUpperCase; }
            set { excludeWordsInUpperCase = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying whether to ignore words with number during spell check.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying whether to ignore words with number during spell check."),
        Browsable(true), DefaultValueAttribute(false)]
        public bool ExcludeWordsWithNumbers
        {
            get { return excludeWordsWithNumbers; }
            set { excludeWordsWithNumbers = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying the number of suggestions to be displayed in the suggestion list.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying the number of suggestions to be displayed in the suggestion list."),
        Browsable(true), DefaultValueAttribute(20)]
        public int MaxSuggestions
        {
            get { return maxSuggestions; }
            set { maxSuggestions = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying whether to ignore special symbols during spell check.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying whether to ignore special symbols during spell check."),
        Browsable(true), DefaultValueAttribute(false)]
        public bool ExcludeSpecialSymbols
        {
            get { return excludeSpecialSymbols; }
            set { excludeSpecialSymbols = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying whether to ignore file names during spell check.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying whether to ignore file names during spell check."),
        Browsable(true), DefaultValueAttribute(false)]
        public bool ExcludeFileNames
        {
            get { return excludeFileNames; }
            set { excludeFileNames = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying whether to ignore internet address during spell check.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying whether to ignore internet address during spell check."),
        Browsable(true), DefaultValueAttribute(false)]
        public bool ExcludeInternetAddresses
        {
            get { return excludeInternetAddresses; }
            set { excludeInternetAddresses = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying the path of the dictionary. 
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying the path of the dictionary."), Browsable(true)]
        public string DictionaryPath
        {
            get { return dictionaryPath; }
            set { dictionaryPath = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying the path of the custom dictionary. 
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying the path of the custom dictionary."), Browsable(true)]
        public string CustomDictionaryPath
        {
            get 
            { 
                return customDictionaryPath; 
            }
            set 
            { 
                customDictionaryPath = value;

                PersistCustomDictionaryPath(customDictionaryPath);
            }
        }

        /// <summary>
        /// Gets or Sets a value specifying whether to ignore email address during spell check.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying whether to ignore email address during spell check."),
        Browsable(true), DefaultValueAttribute(false)]
        public bool ExcludeEmailAddress
        {
            get { return excludeEmailAddress; }
            set { excludeEmailAddress = value; }
        }

        /// <summary>
        /// Gets or Sets a value specifying whether to ignore html tags during spell check.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets a value specifying whether to ignore html tags during spell check."),
        Browsable(true), DefaultValueAttribute(false)]
        public bool ExcludeHtmlTags
        {
            get { return excludeHtmlTags; }
            set { excludeHtmlTags = value; }
        }

        /// <summary>
        /// Holds the list of misspelled words.
        /// </summary>
        [Description("Holds the list of misspelled words."),Browsable(false)]
        public ArrayList MisspelledWords
        {
            get 
            {
                return this.misspelledWords;
            }
        }

		/// <summary>
		/// Gets or sets the stream that contains the dictionary.
		/// </summary>
		[Description("Gets or sets the stream that contains the dictionary."),Browsable(false)]
		public Stream DictionaryStream
		{
			get
			{ 
				return dictionaryStream;
			}
			set 
			{
				dictionaryStream = value;
			}
		}
        #endregion

        #region Constructor
        public SpellChecker()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SpellChecker));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.DictionaryStream = (typeof(SpellChecker).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Text.Dictionary.en_us.dic"));
            this.CustomDictionaryPath = LoadPersistedPath();
        }

        public SpellChecker(IContainer container):this()
        {
            container.Add(this);
        }
        #endregion

        #region ProcessAfterCallBack
        /// <summary>
        /// Performs the spell check operation.
        /// </summary>
        public string SpellCheck(string text)
        {
            string comma = ",";
            string delim = "*";
            if (this.DictionaryPath == DEF_DIC_NAME)
                EnsureDictionaryList();
            else
                EnsureCustomDictionayList(this.DictionaryPath);
            int rind = 0;                                                    
			error_string = new StringBuilder();
            misspelledWords.Clear();
            SplitWords(text);
            for (int i = 0; i < misspelledWords.Count; i++)
            {
                if (i != (int)repeatwordsArray[rind])
                {
					error_string.Append( (string)misspelledWords[i] );
					error_string.Append(delim);
                    ArrayList suggest_list = Suggest((string)misspelledWords[i]);
                    if (suggest_list.Count > 0)
                    {
						string maxSuggest = MaxSuggest(suggest_list, (string)misspelledWords[i]);
						error_string = new StringBuilder();
						error_string.Append(maxSuggest);
                    }
					error_string.Append(comma);
                }
                if (i == (int)repeatwordsArray[rind])
                {
					error_string.Append((string)misspelledWords[i]);
					error_string.Append("#");
					error_string.Append(delim);
					error_string.Append(comma);
                    rind = rind + 1;
                }

            }
            return error_string.ToString();
        }
        #endregion
        
        #region Converting Dictionary To ArrayList

        /// <summary>
        /// Converts the dictionary entries to array list.
        /// </summary>
        private void EnsureDictionaryList()
        {
            string line;
            string dict_path = this.DictionaryPath;
            if (htdictPathVsEntriesList[dict_path] == null)
            {
				StreamReader srReader = null;
				if ( null == this.DictionaryStream )
				{
                    try
                    {
                        srReader = File.OpenText(dict_path);
                    }
                    catch (Exception ex)
                    {
                        RaiseError(ex);
                        srReader = new StreamReader(this.DictionaryStream);
                    }
                }
                else
                {
                    srReader = new StreamReader(this.DictionaryStream);
                }

                ArrayList dictEntries = new ArrayList();
                ArrayList soundXArray = new ArrayList();
                while ((line = srReader.ReadLine()) != null)
                {
                    dictEntries.Add(line.ToLower());                      //Adds the lines from the dictinary to the array list.              
                    soundXArray.Add(Soundex.ToSoundexCode(line));         //Gets the soundex code of the line and adds it to the array list.
                }
                htdictPathVsEntriesList[dict_path] = dictEntries;
                htdictPathVsSoundXList[dict_path] = soundXArray;
            }
         }

        /// <summary>
        /// Converts the dictionary entries to array list.
        /// </summary>
        private void EnsureCustomDictionayList(string dictionaryPath)
        {
            if (htdictPathVsEntriesList[dictionaryPath] == null
                || CustomDictionayCacheDirty)
            {
                StreamReader srReader = null;
                string line;
                try
                {
                    srReader = new StreamReader(File.Open(dictionaryPath, FileMode.OpenOrCreate));
                }
                catch (Exception ex)
                {
                    RaiseError(ex);
                }
                if (srReader != null)
                {
                    ArrayList dictEntries = new ArrayList();
                    while ((line = srReader.ReadLine()) != null)
                    {
                        dictEntries.Add(line.ToLower());
                    }
                    htdictPathVsEntriesList[dictionaryPath] = dictEntries;
                    CustomDictionayCacheDirty = false;

                    srReader.BaseStream.Flush();
                    srReader.Close();
                }
            }
        }

        /// <summary>
        /// Gets the items from the dictionary list.
        /// </summary>
        private ArrayList GetDictList()
        {
            this.EnsureDictionaryList();
            return htdictPathVsEntriesList[this.DictionaryPath] as ArrayList;
        }

        /// <summary>
        /// Gets the items from the dictionary list.
        /// </summary>
        internal ArrayList GetDictList(string dictionaryPath)
        {
            this.EnsureCustomDictionayList(dictionaryPath);
            return htdictPathVsEntriesList[dictionaryPath] as ArrayList;
        }
        
        /// <summary>
        /// Gets the Soundex code.
        /// </summary>
        private ArrayList GetSoundXList()
        {
            this.EnsureDictionaryList();
            return htdictPathVsSoundXList[this.DictionaryPath] as ArrayList;
        }

        #endregion

        #region Suggestion list Generation
        /// <summary>
        /// Generates the word list by deleting a single alphabet and interchanging the adjacent alphabets every time 
        /// </summary>
        public ArrayList Suggest(string source)
        {
            string wrongword = source.ToLower();                                              //Variable which is used to store the wrong word.

            ArrayList suggest_array = new ArrayList();
            source = source.ToLower();
            string temp;

            // Produces the word list, by deleting a single alphabet from the word, each time.

            for (int i = 0; i < source.Length; i++)
            {
                temp = source.Remove(i, 1);
                suggest_array.Add(temp);
            }

            //Produces the word list, by replacing the adjacent alphabets in the word, each time.

            int l = 0;
            int m = 1;
            for (int i = 0; i < source.Length - 1; i++)
            {
                temp = " ";
                temp = source.Substring(0, l) + source[m].ToString();
                temp = temp + source[l].ToString();
                temp = temp + source.Substring(m + 1);
                suggest_array.Add(temp);
                l = l + 1;
                m = m + 1;
            }
            return this.ValidateSuggestions(suggest_array, wrongword);
        }
        #endregion

        #region Splitting up Sentence into Words

        /// <summary>
        /// Splits the retrieved sentence into words.
        /// </summary>
        private void SplitWords(string sentence)
        {
            ArrayList checkList = new ArrayList();                           //ArrayList contains the words to be checked with the dictionary.    
            ArrayList tempList = new ArrayList();                            //Temporary Arraylist. 
            int ind;                                                         //Index of the word.
            int wordLength;                                                  //Length of the word.
            Regex r1 = new Regex("\\b[a-zA-Z0-9_'\x00c0-\x00ff]+\\b", RegexOptions.Compiled);                //Regular expression used to split the sentence.
            Regex r2 = new Regex(@"[a-zA-Z0-9_\-\.]+@[a-zA-Z0-9_\-\.]+\.[a-zA-Z]+", RegexOptions.Compiled);  //Regular expression used to ignore email address.
            Regex r3 = new Regex("(([a-zA-Z]+:)|www\\.)[^\"'>\\)\\]\\s]*", RegexOptions.Compiled);           //Regular expression used to ignore internet address.
            Regex r4 = new Regex("[A-Z]", RegexOptions.Compiled);                                            //Regular expression used to ignore words in mixed case.
            Regex r5 = new Regex("\\b[A-Z]+\\b", RegexOptions.Compiled);                                     //Regular expression used to ignore words in upper case.
            Regex r6 = new Regex(@"\d", RegexOptions.Compiled);                                              //Regular expression used to ignore words with numbers.
            Regex r7 = new Regex(@"[a-zA-Z0-9_$\-\.\\]*\\[a-zA-Z0-9_$\-\.\\]+", RegexOptions.Compiled);      //Regular expression used to ignore File names.
            Regex r8 = new Regex(@"</[c-g\d]+>|</[i-o\d]+>|</[a\d]+>|</[q-z\d]+>|<[cg]+[^>]*>|<[i-o]+[^>]*>|<[q-z]+[^>]*>|<[a]+[^>]*>|<(\[^\]*\|'[^']*'|[^'\>])*>", RegexOptions.IgnoreCase & RegexOptions.Compiled); //Regex Expression Used To Ignore Html Tags.
            Regex r9 = new Regex( @"&#?[a-zA-Z0-9]+;", RegexOptions.Compiled ); //Regular expression used to ignore special symbols.


            //Eliminate the special symbols from the retrieved sentence.

            if (this.ExcludeSpecialSymbols)
            {
                MatchCollection iginetmatches = r9.Matches(sentence);
                foreach (Match iginmatch in iginetmatches)
                {
                    ind = sentence.IndexOf(iginmatch.ToString());
                    wordLength = iginmatch.ToString().Length;    
                    sentence = sentence.Remove(ind,wordLength);
                }
            }

            //Eliminate the internet address from the retrieved sentence.

            if( this.ExcludeInternetAddresses )
            {
                MatchCollection iginetmatches = r3.Matches( sentence );
                foreach( Match iginmatch in iginetmatches )
                {
                    ind = sentence.IndexOf( iginmatch.ToString() );
                    wordLength = iginmatch.ToString().Length;
                    sentence = sentence.Remove( ind, wordLength );
                }

            }

            //Eliminate the email address from the retrieved sentence.

            if (this.ExcludeEmailAddress)
            {
                MatchCollection ieadd = r2.Matches(sentence);
                foreach (Match iea in ieadd)
                {
                    ind = sentence.IndexOf(iea.ToString());
                    wordLength = iea.ToString().Length;
                    sentence = sentence.Remove(ind, wordLength);
                }
            }

            //Eliminate the html tags from the retrieved sentence.
            if (this.ExcludeHtmlTags)
            {
                MatchCollection ihtml = r8.Matches(sentence);
                foreach (Match ihtm in ihtml)
                {
                    ind = sentence.IndexOf(ihtm.ToString());
                    wordLength = ihtm.ToString().Length;
                    sentence = sentence.Remove(ind,wordLength);
                }
            }

            //Eliminate the file names from the retrieved sentence.
            if (this.ExcludeFileNames)
            {
                MatchCollection ifilename = r7.Matches(sentence);
                foreach (Match ifile in ifilename)
                {
                    ind = sentence.IndexOf(ifile.ToString());
                    wordLength = ifile.ToString().Length;
                    sentence = sentence.Remove(ind,wordLength);
                }
            }
            
            //Splits the retrieved sentence into words.
            MatchCollection matches = r1.Matches(sentence);
            foreach (Match match in matches)
            {
                checkList.Add(match.ToString());
                tempList.Add(match.ToString());
            }


            for (int i = 0; i < checkList.Count; i++)
            {
               
                string cs = tempList[i].ToString();
                //Eliminate the words in mixed case.
                if (this.ExcludeWordsInMixedCase)
                {
                    Match mmc = r4.Match(cs);
                    if (mmc.Success)
                    {
                        checkList.Remove(cs);
                        continue;
                       
                    }
                }
                //Eliminate the words in upper case.
                if (this.ExcludeWordsInUpperCase)
                {
                    Match muc = r5.Match(cs);
                    if (muc.Success)
                    {
                        checkList.Remove(cs);
                        continue;

                    }
                }
                //Eliminate the words with numbers.
                if (this.ExcludeWordsWithNumbers)
                {
                    Match mwn = r6.Match(cs);
                    if (mwn.Success)
                    {
                        checkList.Remove(cs);
                        continue;

                    }
                }
            }
            this.CheckForMisspelledWords(checkList);
        }     
                
        #endregion

        #region IsInDictionary
        /// <summary>
        /// Checks whether the word is present in the dictionary.
        /// </summary>
        private bool IsInDictionary(string word)
        {
            string check_word_lower = word.ToLower();
            ArrayList dictEntries = this.GetDictList();

            // Variable Which is Used To Indicate Whether The Given Word Is Present In The Dictionary Or Not
            return (dictEntries.Contains(check_word_lower));
        }
        #endregion

        #region CheckForMisspelledWords

        /// <summary>
        /// Checks the dictinary, to identify the misspelled words.
        /// </summary>
		/// <param name="source_arlist">ArrayList</param>
        /// <returns>wrongWords
        /// </returns>
        private ArrayList CheckForMisspelledWords(ArrayList source_arlist)
        {
            int repeatwordscount_index = 0;              //Count variable counts the total number of repeated values.
            bool intial_flag = true;
            string check_word = " ";
            bool present;
            bool flagRpw = false;                       // This variable indicate that a repeated word is added to the dictionary.

            if (intial_flag == true)
            {
                for (int i = 0; i < repeatwordsArray.Count; i++)
                {
                    repeatwordsArray[i] = -1;
                }
				if (repeatwordsArray.Count <= 0)
				{
					repeatwordsArray.Add(-1);
				}
                intial_flag = false;
            }



            for (int i = 0; i < source_arlist.Count; i++)
            {
                check_word = source_arlist[i].ToString();
                present = IsInDictionary(check_word);

                //Checks for repeated words.

                if (this.ExcludeRepeatedWords  == false)
                {
                    if (i > 0)
                    {
                        if ((string.Compare(check_word, (string)source_arlist[i - 1]) == 0) && (present == true))
                        {
                            flagRpw = true;
                            present = false;
                        }
                    }
                }

                if (present == false)
                {
                    EnsureCustomDictionayList(this.CustomDictionaryPath);
                    ArrayList customDict = htdictPathVsEntriesList[this.CustomDictionaryPath] as ArrayList;

                    if (customDict != null)
                        present = customDict.Contains(check_word.ToLower());
                 }

                //  Adds a word to the wrongword list, if it is not present in the dictionary.

                if (present == false)
                {
                    if (flagRpw)
                    {
						if (repeatwordsArray.Count < repeatwordscount_index)
                        repeatwordsArray[repeatwordscount_index] = misspelledWords.Count;
						else
							repeatwordsArray.Add(misspelledWords.Count);
                        repeatwordscount_index = repeatwordscount_index + 1;
                        flagRpw = false;
                    }
                    misspelledWords.Add(check_word);
                }
            }

            return (misspelledWords);
        }

        #endregion

        #region ValidateSuggestions

        /// <summary>
        /// Checks the dictionary to generate the suggestion list.
        /// </summary>
		/// <param name="source_arlist">ArrayList</param>
        /// <param name="wrongword">String</param>
        /// <returns>suggest_list
        /// </returns>
        private ArrayList ValidateSuggestions(ArrayList source_arlist, string wrongword)
        {
            string check_word = " ";
            bool present = false;

            ArrayList suggest_list = new ArrayList();

            for (int i = 0; i < source_arlist.Count; i++)
            {
                check_word = (string)source_arlist[i];
                present = IsInDictionary(check_word);

                //Checks whether the suggestion is present in the dictionary.

                if (present == true)
                {
                    if (!suggest_list.Contains(check_word))
                    {
                        suggest_list.Add(check_word);
                    }
                }
            }

            SuggestionSound(suggest_list, wrongword);

            return (suggest_list);
        }

        #endregion

        #region Suggestion Generation Using Soundex Algorigthm

        /// <summary>
        /// Gets the suggestion list generated using the Soundex Alogrithm.
        /// </summary>
        /// <param name="suggest_list">ArrayList</param>
        /// <param name="wrongword">String</param>
        private void SuggestionSound(ArrayList suggest_list, string wrongword)
        {
          
            string inp_sound = Soundex.ToSoundexCode(wrongword);
            ArrayList dictEntries = this.GetDictList();

            //Gets the next alphabet and calculates the it's index.

            string source_dict_char = wrongword[0].ToString().ToLower();
            int source_index = dictEntries.IndexOf(source_dict_char);
            int desig_index = 0;
            if (source_dict_char == "z")
            {
                desig_index = dictEntries.Count;
            }
            else
            {
                string next_dict_char = Soundex.NextLetter(source_dict_char);
                desig_index = dictEntries.IndexOf(next_dict_char);
            }

            //Implementation Of the Soundex algortithm.

            int len1 = wrongword.Length;
            ArrayList sound_array = this.GetSoundXList();
            for (int k = 0; k < desig_index; k++)
            {
                if (suggest_list.Count >= MaxSuggestions)
                    break;

                if (sound_array != null && sound_array[k].ToString() == inp_sound)
                {
                    int enumLen = dictEntries[k].ToString().Length;
                    int res = Soundex.EditDistanceCompute(wrongword, dictEntries[k].ToString());
                    if ((res < 4) && (enumLen == len1) | (enumLen == len1 + 1) | (enumLen == len1 - 1) | (enumLen == len1 - 2) | (enumLen == len1 + 2))
                    {

                        if (!suggest_list.Contains(dictEntries[k].ToString()))
                        {
                            suggest_list.Add(dictEntries[k].ToString());
                        }
                    }
                }
            }
        }

        #endregion

        #region Maxmium Suggestion List Genreation

        private int CalculateStartAndEndWord(string word, string WrongWord)
        {
            int result = 0;
            for (int i = WrongWord.Length; i >= 0; i--)
            {
                for (int i1 = 0; i1 <= WrongWord.Length - i; i1++)
                {
                    int length = 0;
                    int ind = 0;
                    if ((ind = word.IndexOf(WrongWord.Substring(i1, i))) >= 0)
                    {

                        for (int iend = WrongWord.Length - i1 - i; iend >= 0; iend--)
                        {
                            string s = WrongWord.Substring(WrongWord.Length - iend);

                            if ((ind + i <= word.IndexOf(s, ind + i)) &&
                                (s.Length > length) )
                            {
                                length = s.Length;
                            }
                        }

                        length += i;
                        if (result < length)
                            result = length;
                        
                    }
                }
            }

            return result;
        } 

        private Hashtable SelectWordFromList(ArrayList suggest_list, string wrongWord)
        {
            Hashtable htWord = new Hashtable();

            for (int i = 0; i < suggest_list.Count; i++)
            {
                string s = string.Empty;
                int key = CalculateStartAndEndWord(s = (string)suggest_list[i], wrongWord.ToLower());
                if (null == htWord[key])
                {
                    htWord.Add(key, new ArrayList());
                }
                ((ArrayList)(htWord[key])).Add(s);

            }
            
            return htWord;
        }

        private ECaseUpperWord CaseFromWord(string wrongWord)
        {
            ECaseUpperWord result = ECaseUpperWord.AllLower;
            if (!char.IsLower(wrongWord[0]))
            {
                result = ECaseUpperWord.FirstUpper;
                if (wrongWord.ToUpper() == wrongWord)
                {
                    result = ECaseUpperWord.AllUpper;
                }
            }

            return result;
        }
        /// <summary>
        /// To customize the number of suggestions to be displayed.
        /// </summary>
        /// <param name="suggest_list">ArrayList</param>
        /// <returns>errror_string
        /// </returns>
        private string MaxSuggest(ArrayList suggest_list, string wrongWord)
        {
            Hashtable htWord = SelectWordFromList(suggest_list, wrongWord);
            ECaseUpperWord caseFromWord = CaseFromWord(wrongWord);

            int totalSuggestion = (this.MaxSuggestions <= suggest_list.Count) ? (this.MaxSuggestions) : (suggest_list.Count);

            for (int k = wrongWord.Length; (k >= 0 )&&(totalSuggestion > 0) ; k--)
            {
                if (htWord[k] != null)
                {
                    ArrayList arWords = (ArrayList)htWord[k];
                    for (int i = 0; (i < arWords.Count)&&(totalSuggestion > 0); i++)
                    {
                        error_string.Append("\\");
                        string sWord = (string)arWords[i];
                        switch (caseFromWord)
                        {
                            case ECaseUpperWord.AllLower:
                            {
                                error_string.Append(sWord);
                                break;
                            }
                            case ECaseUpperWord.FirstUpper:
                            {
                                error_string.Append(sWord[0].ToString().ToUpper() +
                                    sWord.Substring(1));
                                break;
                            }
                            case ECaseUpperWord.AllUpper:
                            {
                                error_string.Append(sWord.ToUpper());
                                break;
                            }
                              
                        }
                        totalSuggestion--;
                    }
                }
            }
            
            
            return error_string.ToString();
        }
        #endregion

        #region Delegates and Events

        /// <summary>
        /// Raised when there is an exception in the <see cref="SpellChecker"/>.
        /// </summary>
        public event ExceptionHanlder Error;

        /// <summary>
        /// Raises the <see cref="Error"/> events.
        /// </summary>
        /// <param name="ex"></param>
        private void RaiseError(Exception ex)
        {
            if (this.Error != null && this.CanRaiseEvents)
                Error(this, ex);
        }

        public delegate void ExceptionHanlder(Object sender, Exception ex);

        #endregion

        #region SpellCheck
        /// <summary>
        /// Checks the text of the <see cref="IEditor"/> for misspellings with <see cref="SpellCheckerDialog"/>.
        /// </summary>
        /// <param name="editor">IEditor whose text is to be checked for misspellings.</param>
        public void SpellCheck(ISpellEditor editor)
        {
            this.Editor = editor;
            InitDialogs();

            Form owner = this.Editor.Control.FindForm();
            this.SpellCheckerDialog.Editor = editor;
            this.SpellCheckerDialog.ShowSpellCheckerDialog(owner);
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Shows the <see cref="OptionsDialog"/>.
        /// </summary>
        /// <param name="owner">Form that owns this dialog.</param>
        internal void ShowOptionsDialog(Form owner)
        {
            this.OptionsDialog.ShowOptionsDialog(this, owner);
        }

        /// <summary>
        /// Shows the <see cref="CustomDictionaryEditor"/>.
        /// </summary>
        /// <param name="owner">Form that owns this dialog.</param>
        internal void ShowCustomDictionaryEditor(Form owner)
        {
            this.CustomDictionaryEditor.ShowOptionsDialog(this, owner);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private void InitDialogs()
        {
            if (this.SpellCheckerDialog == null)
                this.SpellCheckerDialog = new SpellCheckerDialog(this, this.Editor);
            if (this.OptionsDialog == null)
                this.OptionsDialog = new OptionsDialog();
            if (this.CustomDictionaryEditor == null)
                this.CustomDictionaryEditor = new CustomDictionaryEditor();
        }

        /// <summary>
        /// Deletes the contents of the dictionary file by overwritting.
        /// </summary>
        /// <param name="custDictPath">File path whose content to be deleted.</param>
        internal void DeleteAllFromDictionary(string custDictPath)
        {
            try
            {
                FileStream stream = File.Create(custDictPath);
                stream.Close();
                CustomDictionayCacheDirty = true;
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        /// <summary>
        /// Deletes the specified word from the customdictionary.
        /// </summary>
        /// <param name="custDictPath">Specifies the path of the dictionay file.</param>
        /// <param name="wordToDelete">Specifies the word to be deleted.</param>
        internal void DeleteFromDictionary(string custDictPath, string wordToDelete)
        {
            String[] wordList = File.ReadAllLines(custDictPath);

            while (wordList.GetEnumerator().MoveNext())
            {
                if (wordList.GetEnumerator().Current.ToString() == wordToDelete)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Writes the specified word to the dictionary.
        /// </summary>
        /// <param name="custDictPath">Specifies the path of the dictionay file.</param>
        /// <param name="word">Specifies the word to be added to the dictionary.</param>
        public void WriteToDictionary(string custDictPath, string word)
        {
            StreamWriter writter = null;
            try
            {
                writter = File.AppendText(custDictPath);
                writter.WriteLine(word);
                CustomDictionayCacheDirty = true;
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
            finally
            {
                if (writter != null)
                {
                    writter.Flush();
                    writter.Close();
                }
            }
        }

        /// <summary>
        /// Overwrites the specified wordlist to the dictionary.
        /// </summary>
        /// <param name="custDictPath">Specifies the path of the dictionay file.</param>
        /// <param name="wordList">List of words to overrite the dictionary.</param>
        internal void WriteToDictionary(string custDictPath, ArrayList wordList)
        {
            try
            {
                string[] newWordList = new string[wordList.Count];
                wordList.CopyTo(newWordList, 0);
                File.WriteAllLines(custDictPath, newWordList);
                CustomDictionayCacheDirty = true;
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        private void PersistCustomDictionaryPath(string customDictionaryPath)
        {
            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey("Software\\Syncfusion\\CustomDictionaryPath");
            AppStateSerializer serializer = new AppStateSerializer(SerializeMode.WindowsRegistry, regKey);
            serializer.SerializeObject("CustomDictionaryPath", customDictionaryPath);
            serializer.PersistNow();
        }

        private string LoadPersistedPath()
        {
            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey("Software\\Syncfusion\\CustomDictionaryPath");
            AppStateSerializer serializer = new AppStateSerializer(SerializeMode.WindowsRegistry, regKey);
            string custDictPath = serializer.DeserializeObject("CustomDictionaryPath") as string;

            if (custDictPath != null && custDictPath != string.Empty)
                return custDictPath;
            else
                return customDictionaryPath;
        }

        #endregion

    }

    #region ISpellEditor interface

    /// <summary>
    /// Defines a <see cref="SpellChecker"/> interface that provides methods to interact with the editor.
    /// </summary>
    public interface ISpellEditor
    {
        /// <summary>
        /// Gets or sets the <see cref="Control"/> whose <see cref="Control.Text"/> is to be spell checked.
        /// </summary>
        [Description("Gets or sets the Control whose Text is to be spell checked.")]
        System.Windows.Forms.Control Control { get; set; }

        /// <summary>
        /// Gets or sets the current misspelled word.
        /// </summary>
        [Description("Gets or sets the current misspelled word.")]
        String CurrentWord { get; set; }

        /// <summary>
        /// Selects the word specified by the index.
        /// </summary>
        /// <param name="startIndex">Zero based index of the word on the Text.</param>
        /// <param name="length">length of the word to be selected.</param>
        void SelectText(int startIndex, int length);

        /// <summary>
        /// Gets or sets the Text to be spell checked by the <see cref="SpellChecker"/>
        /// </summary>
        [Description("Gets or sets the Text to be spell checked by the SpellChecker")]
        string Text { get; set; }
    }
    
    #endregion

    #region TextEditor

    /// <summary>
    /// Implements <see cref="ISpellEditor"/> interface to interact with instance of <see cref="TextBoxBase"/>.
    /// </summary>
    public class SpellEditorWrapper : ISpellEditor
    {
        #region Thread Safe calls
        
        delegate void SetTextDelegate(string text);
        delegate void SetSelectionDelegate(int selectionStart,int length);
        
        private void SetText(string text)
        {
            if (this.Control.InvokeRequired)
            {
                SetTextDelegate delegete = new SetTextDelegate(SetText);
                control.Invoke(delegete, new object[] { text });
            }
            else
            {
                this.Control.Text = text;
            }
        }

        private void SetSelectedText(string text)
        {
            if (this.Control.InvokeRequired)
            {
                SetTextDelegate delegete = new SetTextDelegate(SetSelectedText);
                control.Invoke(delegete, new object[] { text });
            }
            else
            {
                (this.Control as TextBoxBase).SelectedText = text;
            }
        }

        private void SetSelection(int selectionStart, int selectionLength)
        {
            if (this.Control.InvokeRequired)
            {
                SetSelectionDelegate delegete = new SetSelectionDelegate(SetSelection);
                control.Invoke(delegete, new object[] { selectionStart, selectionLength });
            }
            else
            {
                (this.Control as TextBoxBase).SelectionStart = selectionStart;
                (this.Control as TextBoxBase).SelectionLength = selectionLength;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="SpellEditorWrapper"/> class that hosts the <see cref="TextBoxBase"/> control.
        /// </summary>
        /// <param name="control"></param>
        public SpellEditorWrapper(System.Windows.Forms.TextBoxBase control)
        {
            this.Control = control;
        }
        
        #endregion

        #region ISpellChecker Members

        /// <summary>
        /// Gets or sets the <see cref="Control"/> whose <see cref="Control.Text"/> is to be spell checked.
        /// </summary>
        [Description("Gets or sets the Control whose Text is to be spell checked.")]
        public System.Windows.Forms.Control Control
        {
            get
            {
                return this.control;
            }
            set
            {
                if(value is TextBoxBase)
                    this.control = value as TextBoxBase;
            }
        }

        /// <summary>
        /// Gets or sets the current misspelled word.
        /// </summary>
        [Description("Gets or sets the current misspelled word.")]
        public string CurrentWord
        {
            get
            {
                return (this.Control as TextBoxBase).SelectedText;
            }
            set
            {
                this.SetSelectedText(value);
            }
        }

        /// <summary>
        /// Selects the word specified by the index.
        /// </summary>
        /// <param name="startIndex">Zero based index of the word on the Text.</param>
        /// <param name="length">length of the word to be selected.</param>
        public void SelectText(int selectionStart, int selectionLength)
        {
            this.SetSelection(selectionStart, selectionLength);
        }

        /// <summary>
        /// Gets or sets the Text to be spell checked by the <see cref="SpellChecker"/>
        /// </summary>
        [Description("Gets or sets the Text to be spell checked by the SpellChecker")]
        public string Text
        {
            get
            {
                return this.Control.Text;
            }
            set
            {
                this.SetText(value);
            }
        }

        #endregion

        #region Private Fields
        private TextBoxBase control = null;
        private String currentWord = string.Empty;
        #endregion
    }

    #endregion
}
#endif