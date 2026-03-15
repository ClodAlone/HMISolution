#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

using System;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Edit.Dialogs.Options
{
    /// <summary>
    /// Interface for controls that manage EditControl options.
    /// </summary>
    public interface IOptionsControl
    {
        #region Interface Methods
        /// <summary>
        /// Initializes options control with data from EditControl.
        /// </summary>
        /// <param name="control">EditControl used for initializing.</param>
        void Init(EditControl control);

        /// <summary>
        /// Applies set options to given EditControl.
        /// </summary>
        /// <param name="control">EditControl to apply options to.</param>
        /// <returns>Null if everything is OK; control to transfer focus to if error occured.</returns>
        Control Apply(EditControl control);
        #endregion
    }
}