#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class represents root for auto-complete, where the
    /// source is Custom.
    /// </summary>
    internal class CustomRoot : object, IAutocompleteRoot
    {
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Only one current level for this source.
        /// </summary>
        private CustomLevel currentRootLevel = null;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.CustomRoot">CustomRoot</see>
        /// class.
        /// </summary>
        /// <remarks>
        /// Constructor of the CustomRoot, which will initialize all events and properties.
        /// </remarks>
        /// <param name="inputList">Collection of items for current level.</param>
        internal CustomRoot(List<string> inputList)
        {
            if (inputList != null)
            {
                inputList.Sort();
                this.currentRootLevel = new CustomLevel(inputList);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method searches items which meet the requirements of
        /// appropriate conditions.
        /// </summary>
        /// <param name="level">Current level.</param>
        /// <param name="filterString">Filter string.</param>
        /// <returns>
        /// Collection which include items of appropriate conditions.
        /// </returns>
        public AutocompleteItemCollection CreateFilteredGhost(IAutocompleteLevel level, string filterString)
        {
            AutocompleteItemCollection collection = new AutocompleteItemCollection();
            AutocompleteItemCollection inputCollection = level.Items;
            int filterLenght = filterString.Length;

            for (int i = 0, cnt = inputCollection.Count; i < cnt; ++i)
            {
                string inputText = inputCollection[i].Text;

                if (filterLenght <= inputText.Length
                    && inputText.StartsWith(filterString, StringComparison.OrdinalIgnoreCase))
                {
                    collection.Add(inputCollection[i]);
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
            return this.currentRootLevel;
        }
    }
}