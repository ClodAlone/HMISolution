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

namespace Syncfusion.Windows.Shared
{
    
    /// <summary>
    /// 
    /// </summary>
    public class WordOccurrence
    {
        // Fields
        private int m_nCaretEnd = -1;
        private int m_nCaretStart = -1;
        private string m_sWord = "";

        // Properties
        /// <summary>
        /// 
        /// </summary>
        public int EndPosition
        {
            get
            {
                return m_nCaretEnd;
            }
            set
            {
                m_nCaretEnd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int StartPosition
        {
            get
            {
                return m_nCaretStart;
            }
            set
            {
                m_nCaretStart = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Word
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
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="caretStart"></param>
        /// <param name="caretEnd"></param>
        public WordOccurrence(string word, int caretStart, int caretEnd)
        {
            m_sWord = word;
            m_nCaretStart = caretStart;
            m_nCaretEnd = caretEnd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetEndPosition()
        {
            return m_nCaretEnd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetStartPosition()
        {
            return m_nCaretStart;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetWord()
        {
            return m_sWord;
        }
    }

    /// <exclude/>
    public class BadWord :
        WordOccurrence
    {
        // Fields
        private int m_nReason;
        /// <summary>
        /// 
        /// </summary>
        public static int REASON_DUPLICATE = 1;
        /// <summary>
        /// 
        /// </summary>
        public static int REASON_SPELLING = 2;

        // Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="caretStart"></param>
        /// <param name="caretEnd"></param>
        public BadWord(string word, int caretStart, int caretEnd)
            : base(word, caretStart, caretEnd)
        {
            m_nReason = REASON_SPELLING;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="caretStart"></param>
        /// <param name="caretEnd"></param>
        /// <param name="reason"></param>
        public BadWord(string word, int caretStart, int caretEnd, int reason)
            : base(word, caretStart, caretEnd)
        {
            m_nReason = reason;
        }

        // Properties
        /// <summary>
        /// 
        /// </summary>
        public int Reason
        {
            get
            {
                return m_nReason;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetReason()
        {
            return m_nReason;
        }
    }
}
