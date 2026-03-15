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
using System.Text;

namespace Syncfusion.Windows.Forms.Chart.PostScript
{
    /// <summary>
    /// Represents the post script dictionary.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class PostScriptDictionary
    {
        #region Members
        private Hashtable entries;
        private const string name = "Dict";
        private int num = 0;
        private string m_tempString = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the internal table.
        /// </summary>
        /// <value>The internal table.</value>
        public Hashtable InternalTable
        {
            get
            {
                return entries;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get
            {
                return name + num.ToString();
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PostScriptDictionary"/> class.
        /// </summary>
        public PostScriptDictionary()
        {
            entries = new Hashtable(10);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Toes the post script string.
        /// </summary>
        /// <returns>Returns string.</returns>
        public string ToPostScriptString()
        {
            if (m_tempString == string.Empty)
            {
                StringBuilder sb = new StringBuilder(100);

                sb.Append("<<");

                int count = entries.Count;

                foreach (object key in entries.Keys)
                {
                    sb.Append(key.ToString());
                    sb.Append(" ");
                    sb.Append(entries[key].ToString());
                    sb.Append(" ");
                }

                sb.Append(">>");
                m_tempString = sb.ToString();
                return m_tempString;
            }
            else
                return m_tempString;
        }

        /// <summary>
        /// Sets the num.
        /// </summary>
        /// <param name="n">The n.</param>
        public void SetNum(int n)
        {
            num = n;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.ToPostScriptString();
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToPostScriptString().GetHashCode();
        }
        #endregion
    }
}
