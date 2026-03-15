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

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileSplitPaneBaseItem
    {
        #region Properties
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public MvcTemplate<MobileSplitPaneBaseItem> LeftPane { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public MvcTemplate<MobileSplitPaneBaseItem> RightPane { get; set; }
        #endregion

         #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabBaseItem"/> class.
        /// </summary>
        public MobileSplitPaneBaseItem()
        {
            this.LeftPane = new MvcTemplate<MobileSplitPaneBaseItem>();
            this.RightPane = new MvcTemplate<MobileSplitPaneBaseItem>();
        }
        #endregion
    }
}
