#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;TH&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Th)]
    public class THElementImpl : TDElementImpl
    {
        #region Class constants
        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Th;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the THElementImpl class
        /// </summary>
        /// <param name="parent">IHTMLElement instance</param>
        public THElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Sets own format (inherits background color and border from table and tr parent elements).
        /// </summary>
        protected internal override void SetOwnFormat()
        {
            base.SetOwnFormat();

            m_ownFormat.Merge |= MergeMask.FontWeight;
            m_ownFormat.FontWeight = FontStyle.Bold;

            m_ownFormat.HorizontalAlign = StringAlignment.Center;
            m_ownFormat.Merge |= MergeMask.HAlignment;

            m_ownFormat.VerticalAlign = StringAlignment.Center;
            m_ownFormat.Merge |= MergeMask.VAlignmnet;
        }
        #endregion
    }
}
