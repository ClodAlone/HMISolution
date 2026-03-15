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

namespace Syncfusion.Windows.Shared
{
   
    internal class Suggestion
    {
        // Fields
        private int m_nScore;
        private string m_sWord;

        // Properties
        public virtual int Score
        {
            get
            {
                return m_nScore;
            }
            set
            {
                m_nScore = value;
            }
        }

        public virtual string Word
        {
            get
            {
                return m_sWord;
            }
            set
            {
                m_sWord = value;
            }
        }

        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Suggestion"/> class.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="score">The score.</param>
        public Suggestion(string word, int score)
        {
            m_sWord = word;
            m_nScore = score;
        }

        /// <summary>
        /// Determines whether [contains] [the specified list].
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="suggestion">The suggestion.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified list]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(List<object> list, Suggestion suggestion)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (((Suggestion)list[i]).Word == suggestion.Word)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether [contains] [the specified list].
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="suggestion">The suggestion.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified list]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(List<object> list, string suggestion)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (((Suggestion)list[i]).Word == suggestion)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
