#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// Delegate for SelectionPointerChangedEvent
    /// </summary>
    /// <param name="sender">Gets the sender object from the reporting source</param>
    /// <param name="args">Gets the EditIntellisenseArgs object from the reporting source</param>
    public delegate void IntellisenseBoxEventHandler(object sender, EditIntellisenseArgs args);

#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class EditIntellisenseArgs : EventArgs
    {
        /// <summary>
        ///
        /// </summary>
        public int LineIndex { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public int CursorIndex { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public IIntellisenseItem SelectedItem { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public ScopeDefinition CurrentScope { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public string TextInput { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public List<Uri> Assemblies { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<IIntellisenseItem> ItemsSource { get; set; }
    }
}