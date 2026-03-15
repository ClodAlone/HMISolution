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

#if (!SILVERLIGHT && !WP) || WINRT
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Layouting
{
  internal  class HtmlToDocLayoutInfo
    {
        # region Fields
        private bool m_bRemoveLineBreak = true;
        #endregion

        #region Properties
      /// <summary>
      /// Gets/Sets boolean value to indicate whether line break need to be removed if it is last item in a paragraph
      /// </summary>
        internal bool RemoveLineBreak
        {
            get
            {
                return m_bRemoveLineBreak;
            }
            set
            {
                m_bRemoveLineBreak = value;
            }
        }
         #endregion

    }
}
#endif
