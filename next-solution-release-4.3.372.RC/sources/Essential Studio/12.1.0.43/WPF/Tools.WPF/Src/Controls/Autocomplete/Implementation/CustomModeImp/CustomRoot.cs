// <copyright file="CustomRoot.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class represents root for auto-complete, where the
    /// source is Custom.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class CustomRoot : Object, IAutocompleteRoot
    {
        #region Private members

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Only one current level for this source.
        /// </summary>
        private readonly CustomLevel m_CurrentRootLevel = null;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRoot"/> class.
        /// </summary>
        /// <param name="inputList">The input list.</param>
        internal CustomRoot(List<object> inputList, bool EnableSorting)
        {
            if (inputList != null)
            {
                if (EnableSorting)
                    inputList.Sort();
                m_CurrentRootLevel = new CustomLevel(inputList);
            }
        }

        #endregion Initialization

        #region Public method

        /// <summary>
        /// Creates the filtered ghost.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="filterString">The filter string.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public AutocompleteItemCollection CreateFilteredGhost(IAutocompleteLevel level, string filterString, StringMode mode, int index)
        {
            AutocompleteItemCollection collection = new AutocompleteItemCollection();
            AutocompleteItemCollection inputCollection = level.Items;

            int filterLenght = filterString.Length;

            for (int i = 0, cnt = inputCollection.Count; i < cnt; ++i)
            {
                string inputText = inputCollection[i].Text;

                if (mode == StringMode.IndexBased)
                {
                    if (index <= (inputText.Length - 1) && index >= 0)
                    {
                        if (filterLenght <= inputText.Length
                            && CompareFromIndex(index, inputText, filterString))
                        {
                            collection.Add(inputCollection[i]);
                        }
                    }
                    else
                    {
                        throw new IndexOutOfRangeException("Filter Index Out of Range");
                    }
                }
                else if (mode == StringMode.AnyChar)
                {
                    if (filterLenght <= inputText.Length
                        && inputText.ToLower().Contains(filterString.ToLower()))
                    {
                        collection.Add(inputCollection[i]);
                    }
                }
                else if (mode == StringMode.StartChar)
                {
                    if (filterLenght <= inputText.Length
                        && inputText.StartsWith(filterString, StringComparison.OrdinalIgnoreCase))
                    {
                        collection.Add(inputCollection[i]);
                    }
                }
            }

            return collection;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets root for this case.
        /// </summary>
        /// <param name="rootText">Current root's text.</param>
        /// <returns>
        /// Only one current level for this source.
        /// </returns>
        public IAutocompleteLevel GetRoot(string rootText)
        {
            return m_CurrentRootLevel;
        }

        /// <summary>
        /// Compares from index.
        /// </summary>
        /// <param name="Start">The start.</param>
        /// <param name="input">The input.</param>
        /// <param name="test">The test.</param>
        /// <returns></returns>
        private bool CompareFromIndex(int Start, string input, string test)
        {
            string temp;
            temp = input.Substring(Start);
            return temp.StartsWith(test, StringComparison.OrdinalIgnoreCase);
        }
    }

        #endregion Public method
}