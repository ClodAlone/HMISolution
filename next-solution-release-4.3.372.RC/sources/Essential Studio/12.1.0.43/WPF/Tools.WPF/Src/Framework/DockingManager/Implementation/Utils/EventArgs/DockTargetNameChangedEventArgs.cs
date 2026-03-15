// <copyright file="DockTargetNameChangedEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class used to pass new and old names of the docking target of
    /// the dock able element.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DockTargetNameChangedEventArgs : EventArgs
    {
        #region Public members
        /// <summary>
        /// Specifies the old name of the docking target.
        /// </summary>        
        public readonly string OldName;
        
        /// <summary>
        /// Specifies the new name of the docking target.
        /// </summary>        
        public readonly string NewName;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DockTargetNameChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public DockTargetNameChangedEventArgs(string oldValue, string newValue)
        {
            OldName = oldValue;
            NewName = newValue;
        }
        #endregion
    }
}
