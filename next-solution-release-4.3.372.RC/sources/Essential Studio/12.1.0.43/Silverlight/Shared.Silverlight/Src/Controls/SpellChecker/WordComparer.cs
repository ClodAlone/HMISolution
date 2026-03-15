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
using System.Globalization;

namespace Syncfusion.Windows.Shared
{
    class WordComparer
    {
    }
    internal class InvariantComparer :
            IComparer<object>
    {
        // Fields
        private CompareInfo m_compareInfo = CultureInfo.InvariantCulture.CompareInfo;

        // Methods
        internal InvariantComparer()
        {
        }

        public virtual int Compare(object a, object b)
        {
            string str = a as string;
            string str2 = b as string;
            if ((str != null) && (str2 != null))
            {
                return m_compareInfo.Compare(str, str2);
            }

            return a == null ? -1 : 1;
        }

        public virtual int Compare(string x, string y)
        {
            if ((x != null) && (y != null))
            {
                return m_compareInfo.Compare(x, y);
            }

            return x.CompareTo(y);
        }
    }

    /// <exclude/>
    internal class ReverseSorter :
            InvariantComparer
    {
        // Methods
        public override int Compare(object a, object b)
        {
            return base.Compare(Reverse((string)a), Reverse((string)b));
        }

        public static string Reverse(string s)
        {
            char[] chArray = new char[s.Length];
            for (int i = s.Length; i > 0; i--)
            {
                chArray[s.Length - i] = s[i - 1];
            }
            return new string(chArray);
        }
    }

    /// <exclude/>
    internal class CompareL :
        IComparer<object>
    {
        // Fields
        private SpellChecker m_SpellChecker;
        private Dictionary<object, int> scores;
        private string topWord;

        // Methods
        public CompareL(SpellChecker spellchecker)
        {
            m_SpellChecker = spellchecker;
        }

        public int Compare(object a, object b)
        {
            if ( !scores.ContainsKey(b) || scores[b] == 0)
            {
                scores[b] = m_SpellChecker.SuggestionScore2b((string)b, topWord);
            }
            if (!scores.ContainsKey(a) || scores[a] == 0)
            {
                scores[a] = m_SpellChecker.SuggestionScore2b((string)a, topWord);
            }
            return (((int)scores[b]) - ((int)scores[a]));
        }

        public void with(string w)
        {
            topWord = w;
            scores = new Dictionary<object, int>();
        }
    }
}
