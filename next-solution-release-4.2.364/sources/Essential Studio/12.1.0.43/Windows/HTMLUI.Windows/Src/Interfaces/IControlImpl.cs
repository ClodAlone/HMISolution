#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Interface declaring the main functionality for custom control wrappers.
    /// </summary>
    public interface IControlImpl : ICustomControlBase
    {
        /// <summary>
        /// Gets Parent INPUT tag element of the class.
        /// </summary>
        new IUserControlHolder Parent 
        { 
            get; 
        }

        /// <summary>
        /// Configures user control corresponding to the attributes.
        /// </summary>
        void ConfigureControl();
    }
}
