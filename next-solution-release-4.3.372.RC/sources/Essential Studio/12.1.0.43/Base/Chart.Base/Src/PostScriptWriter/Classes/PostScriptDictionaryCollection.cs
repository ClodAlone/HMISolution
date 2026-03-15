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
using System.Collections;

namespace Syncfusion.Windows.Forms.Chart.PostScript
{
    /// <summary>
    /// Collection of <see cref="PostScriptDictionary"/>.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class PostScriptDictionaryCollection : CollectionBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PostScriptDictionaryCollection"/> class.
        /// </summary>
        public PostScriptDictionaryCollection()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the specified dictionary.
        /// </summary>
        /// <param name="dict">The <see cref="PostScriptDictionary"/>.</param>
        /// <returns>Returns PostScriptDictionary object.</returns>
        public PostScriptDictionary Add(PostScriptDictionary dict)
        {
            int index = this.ContainsSimilar(dict);
            if (index == -1)
            {
                this.List.Add(dict);
                dict.SetNum(List.Count - 1);
                return dict;
            }
            else
            {
                return (PostScriptDictionary)List[index];
            }
        }

        /// <summary>
        /// Inserts the specified dictionary.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="dict">The <see cref="PostScriptDictionary"/>.</param>
        public void Insert(int index, PostScriptDictionary dict)
        {
            this.List.Insert(index, dict);
            dict.SetNum(index);
        }

        /// <summary>
        /// Removes the specified dictionary.
        /// </summary>
        /// <param name="dict">The <see cref="PostScriptDictionary"/>.</param>
        public void Remove(PostScriptDictionary dict)
        {
            this.List.Remove(dict);
        }

        /// <summary>
        /// Returns index of the specified dictionary.
        /// </summary>
        /// <param name="dict">The <see cref="PostScriptDictionary"/>.</param>
        /// <returns> The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(PostScriptDictionary dict)
        {
            return this.List.IndexOf(dict);
        }

        /// <summary>
        /// Determines whether collection contains the specified dictionary.
        /// </summary>
        /// <param name="dict">The <see cref="PostScriptDictionary"/>.</param>
        /// <returns>
        /// 	<c>true</c> if collection contains the specified dictionary; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(PostScriptDictionary dict)
        {
            return this.List.Contains(dict);
        }

        /// <summary>
        /// Determines whether colection contains similar dictionary.
        /// </summary>
        /// <param name="dict">The <see cref="PostScriptDictionary"/>.</param>
        /// <returns>True if it contains similar object else false.</returns>
        public int ContainsSimilar(PostScriptDictionary dict)
        {
            int containsIndex = -1;
            int hash = dict.GetHashCode();
            for (int i = 0; i < List.Count; i++)
            {
                PostScriptDictionary d = (PostScriptDictionary)List[i];
                if (d.GetHashCode() == hash)
                {
                    containsIndex = i;
                    break;
                }
            }
            return containsIndex;
        }
        #endregion
    }
}
