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
#if !SILVERLIGHT && !WP
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    /// <summary>
    /// EventArgs for AddNewRowIntiating event.
    /// </summary>
    public class AddNewRowInitiatingEventArgs : GridEventArgs
    {
        #region Ctor

        public AddNewRowInitiatingEventArgs(object originalSource):base(originalSource)
        {

        }

        #endregion

        #region Args
        /// <summary>
        /// Get the new object created for AddNewRow
        /// </summary>
        public object NewObject { get; internal set; }

        #endregion
    }

    /// <summary>
    /// Delegate for AddNewRowInitiating event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void AddNewRowInitiatingEventHandler(object sender, AddNewRowInitiatingEventArgs args);
}
