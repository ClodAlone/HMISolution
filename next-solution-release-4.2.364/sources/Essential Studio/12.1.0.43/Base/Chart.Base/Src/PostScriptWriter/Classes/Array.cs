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
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart.PostScript
{
    /// <summary>
    /// Represents the post script array.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class PostScriptArray
    {
        #region Members
        private ArrayList list;
        private string name = "Array";
        private int m_id = 0;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return name + m_id.ToString();
            }
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>The list.</value>
        public ArrayList List
        {
            get
            {
                return list;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PostScriptArray"/> class.
        /// </summary>
        public PostScriptArray()
        {
            list = new ArrayList(10);
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Toes the post script string.
        /// </summary>
        /// <returns>Returns string.</returns>
        public string ToPostScriptString()
        {
            StringBuilder sb = new StringBuilder(100);

            sb.Append("[");

            foreach (object ob in list)
            {
                sb.Append(ob.ToString());
                sb.Append(" ");
            }

            sb.Append("]");

            return sb.ToString();
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
        #endregion
    }
}
