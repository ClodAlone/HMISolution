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
using System.IO;
using System.Collections;
using System.Windows;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class SpellCheckControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iStr"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ObservableCollection<string> Main1(string iStr, string str)
        {
            
            ICheckerEngine sc = new SpellChecker();
            SpellChecker checker = sc as SpellChecker;
            #region Load Dictionary

            FileStream fileStream = new FileStream( @"Controls\SpellChecker\Dictionary\en_us.dic", FileMode.Open, FileAccess.Read);
            checker.SetDictFileStream(fileStream, @"Controls\SpellChecker\Dictionary\en_us.dic");
            #endregion
            #region Express SpellCheckEngine settings
            //while (true)
            //{
                bool spellSetting = true;
                SetupCheckerEngine(sc);
                //string[] inputStr = Console.ReadLine().Split(' ');
                //foreach (string iStr in inputStr)
                //{
                    switch (iStr)
                    {
                        case @"/case":// Setting True will allow checking in any case
                            sc.AllowAnyCase = true;
                            break;
                        case @"/inUcase": // Setting false will include words in upper case for spell check
                            sc.ExcludeWordsInUpperCase = false;
                            break;
                        case @"/inemail": //Setting false will include email address for spell check
                            sc.ExcludeEmailAddress = false;
                            break;
                        case @"/infilen": //Setting false will include file names for spell check
                            sc.ExcludeFileNames = false;
                            break;
                        case @"/inMixedCase": //Setting false will include spell key to check in mixed case .
                            sc.ExcludeWordsInMixedCase = false;
                            break;
                        case @"/iniaddr": //Setting false will include interner address for spell check
                            sc.ExcludeInternetAddresses = false;
                            break;
                        case @"/lookup": //To Check if the spell key is present in main dictionary. Returns Boolean.
                            //bool bPresent = sc.LookUp(inputStr[inputStr.Length - 1]);
                            //Console.WriteLine(bPresent ? "Available\n" : "Unavailable in the main dictionary.\n");
                            spellSetting = false;
                            break;
                        case @"/?": // Applicaiton level help which will clear the screen and display help switches.
                            //SetUpHelp();
                            spellSetting = false;
                            break;
                        case @"/i": //To add a new word to the local cache dictionary
                            sc.AddWord(str);
                            //Console.WriteLine("Word Successfully added.");
                            spellSetting = false;
                            break;
                        default:
                            break;
                    }
                //}
                if (spellSetting)
                {
                    return SpellCheck(sc, str);
                }

                return null;

            //}
            #endregion
        }
        
        #region SpellChecker and populate the Suggesstions
        private static ObservableCollection<string> SpellCheck(ICheckerEngine sc, string sKey)
        {
            ObservableCollection<string> _list = new ObservableCollection<string>();
            sc.Check(sKey);
            string sErrorString = CommonSpellCheck(sKey, sc, 7);
            if (sErrorString.Equals(""))
            {
                bool present = sc.LookUp(sKey);
                if(present)
                {
                //Console.WriteLine("No Spelling Errors.");
                   // MessageBox.Show("No Spelling Errors");
                    
                }
                else
                {
                    //Console.WriteLine("Suggesstions not available.");
                    //MessageBox.Show("Suggestions not available");
                    _list.Add("Suggestions not available");
                }
            }
            else
            {
                //Console.WriteLine("Possible Suggessions:");
                //Console.WriteLine("---------------------");
                //Console.WriteLine(sErrorString);
               // MessageBox.Show(sErrorString);
                string[] arr = sErrorString.Split(new String[]{"\n"}, StringSplitOptions.None);
                foreach (var a in arr)
                {
                    if (a != String.Empty)
                    {
                        _list.Add(a);
                    }
                }
            }
            return _list;
        }
        #endregion
        #region Reset Spell Check Engine settings
        internal static void SetupCheckerEngine(ICheckerEngine sc)
        {
            //Resetting the settings of spell checker engine
            sc.AllowAnyCase = false;
            sc.ExcludeEmailAddress = true;
            sc.ExcludeFileNames = true;
            sc.ExcludeWordsInMixedCase = true;
            sc.ExcludeInternetAddresses = true;
            sc.ExcludeWordsWithNumbers = true;
            sc.ExcludeWordsInUpperCase = true;
        }
        #endregion
        #region Generate Suggestions list
        private static string CommonSpellCheck(string eargs, ICheckerEngine iCheckerEngine, int nMaxSuggestions)
        {
            BadWord word;

            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();

            while ((word = iCheckerEngine.NextBadWord()) != null)
            {
                List<object> suggest_list = iCheckerEngine.FindSuggestions();
                list.Add(word.Word);
                if ((0 < suggest_list.Count) && (nMaxSuggestions < suggest_list.Count))
                {
                    suggest_list =suggest_list.GetRange(0, nMaxSuggestions);
                }
                list.Add(ConvertArrayToClientString(suggest_list));
            }
            string error_string = ConvertArrayToClientString(list);
            return error_string;
        }
        #endregion
        #region Get the suggestions in string format
        private static string ConvertArrayToClientString(IList list)
        {

            StringBuilder sbRes = new StringBuilder();
            for (int i = 1; i < list.Count; i++)
            {
                object obj = list[i];
                string sText = (null != obj) ? obj.ToString() : "No Suggesstions";
                if(sText!="")
                sbRes.Append(sText + "\n");
            }
            return sbRes.ToString();
        }
        #endregion
    }

}
