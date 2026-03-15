// <copyright file="FilePathRoot.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class represent root for auto-complete, where the source
    /// is FilePath.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class FilePathRoot : IAutocompleteRoot
    {
        #region Constants

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant presents LAN start text.
        /// </summary>
        private const string LANBegin = @"\\";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant presents supplement to letter for presenting
        /// logic disk.
        /// </summary>
        private const string StrSufics = @":\";

        #endregion Constants

        #region Private member

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains root's items.
        /// </summary>
        private readonly AutocompleteItemCollection m_Items = new AutocompleteItemCollection();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains root current level.
        /// </summary>
        private IAutocompleteLevel m_currentRootLevel = null;

        #endregion Private member

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathRoot"/> class.
        /// </summary>
        internal FilePathRoot()
        {
            String[] drives = Environment.GetLogicalDrives();

            for (int i = 0, cnt = drives.Length; i < cnt; ++i)
            {
                string driver = drives[i];
                m_Items.Add(new FilePathLevel(driver[0].ToString(), StrSufics));
            }

            m_Items.Add(new FilePathLevel(String.Empty, LANBegin));
        }

        #endregion Initialization

        #region Public methods

        /// <summary>
        /// This method searches items which meet the requirements of the
        /// appropriate conditions.
        /// </summary>
        /// <param name="level">Current level.</param>
        /// <param name="filterString">Filter string.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        /// Collection which includes items of appropriate conditions.
        /// </returns>
        /// <property name="flag" value="Finished"/>
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
        /// <param name="rootText">Current root text.</param>
        /// <returns>
        /// Root for this root text.
        /// </returns>
        public IAutocompleteLevel GetRoot(string rootText)
        {
            int rootTextLenght = rootText.Length;

            if (null != m_currentRootLevel)
            {
                string fullPath = m_currentRootLevel.GetFullPath();

                if (fullPath.Length >= rootTextLenght
                    || !rootText.StartsWith(fullPath, StringComparison.OrdinalIgnoreCase))
                {
                    m_currentRootLevel = GetDesireLevel(rootText, rootTextLenght);
                }
            }
            else
            {
                m_currentRootLevel = GetDesireLevel(rootText, rootTextLenght);
            }

            return m_currentRootLevel;
        }

        #endregion Public methods

        #region Implementation

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method searches current level.
        /// </summary>
        /// <param name="rootText">Root for this root text.</param>
        /// <param name="rootTextLenght">Length of the root text.</param>
        /// <returns>
        /// Current level.
        /// </returns>
        private IAutocompleteLevel GetDesireLevel(string rootText, int rootTextLenght)
        {
            for (int i = 0, cnt = m_Items.Count; i < cnt; ++i)
            {
                IAutocompleteLevel itemLevel = m_Items[i] as IAutocompleteLevel;

                if (null != itemLevel)
                {
                    string itemLevelText = itemLevel.GetFullPath();

                    if (itemLevelText.Length <= rootTextLenght
                        && rootText.StartsWith(itemLevelText, StringComparison.OrdinalIgnoreCase))
                    {
                        return itemLevel;
                    }
                }
            }

            return null;
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

        #endregion Implementation
    }
}