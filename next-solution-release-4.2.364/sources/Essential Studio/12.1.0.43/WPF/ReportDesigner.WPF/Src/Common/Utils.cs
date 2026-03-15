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
using System.Windows;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Reports.Common
{
    internal class Util
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Util"/> class.
        /// </summary>
        public Util()
        {
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the parent item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child">The child.</param>
        /// <returns></returns>
        public static T GetParentItem<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject dependencyObject = VisualTreeHelper.GetParent(child);
            T parent = dependencyObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentItem<T>(dependencyObject);
            }
        }

        /// <summary>
        /// Returns the SQL query for top 1000 records.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The query for top 1000 records.</returns>
        public static string ReturnSQLTop(string query)
        {
            query = query.TrimStart().ToLower();

            if (query.Substring(0, 7).ToString() == "select ")
            {
                Regex regex = new Regex(@"(select)(\n|\s|\t|\r)+(top)");
                MatchCollection theCollection = regex.Matches(query);
                string matchedString = string.Empty;
                foreach (Match matched in theCollection)
                {
                    matchedString += matched.ToString();
                }

                /// top exist or not
                if (matchedString != string.Empty && query.IndexOf(matchedString) == 0)
                {
                    Regex regex1 = new Regex(@"(select)(\n|\s|\t|\r)+(top)([\n|\s|\t|\r]*)[(]?([\n|\s|\t|\r]*)(\d)+");
                    MatchCollection theCollection1 = regex1.Matches(query);
                    string matchedString1 = string.Empty;
                    foreach (Match matched1 in theCollection1)
                    {
                        matchedString1 += matched1.ToString();
                    }


                    string getTopNumber = string.Empty;
                    if (matchedString1 != string.Empty && query.IndexOf(matchedString1) == 0)
                    {
                        getTopNumber = query.Substring(0, matchedString1.Length).ToString().Replace("select", string.Empty).Replace("top", string.Empty).Replace("(", string.Empty).Trim().ToString();

                        /// if exist, check top more than 1000
                        if (Convert.ToDouble(getTopNumber) > 1000)
                        {
                            /// if more than 1000, convert it to 1000
                            string initialString = matchedString1;  
                            initialString = initialString.Replace(getTopNumber, "1000");
                            query = initialString + query.Substring(matchedString1.Length, query.Length - matchedString1.Length);
                        }
                    }
                }
                else
                {
                    /// if not insert top
                    query = "select top(1000) " + query.Substring(7, query.Length - 7);
                }
            }
            return query;
        }

        /// <summary>
        /// Checks the name with RE.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <returns>Whether input text is valid.</returns>
        public static bool CheckNameWithRE(string inputText)
        {           
            string patternRE = @"^[a-zA-Z][0-9|_|a-zA-Z]*$";//[_]*[0-9]*[a-zA-Z]*$";
            Match theMatch = Regex.Match(inputText, patternRE);

            return theMatch.Success;
        }

        /// <summary>
        /// Checks the name with previous collection.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="arrayOfName">Name of the array of.</param>
        /// <returns>Whether the input is correct.</returns>
        public static bool CheckNameWithPreviousCollection(string inputText, string[] arrayOfName)
        {
            bool correctName = true;
            if (arrayOfName != null && inputText!=null)
            {
                foreach (string strName in arrayOfName)
                {
                    if (strName.Equals(inputText))
                    {
                        correctName = false;
                        break;
                    }
                }
            }

            return correctName;
        }
        #endregion
    }
}
