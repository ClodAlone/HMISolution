// <copyright file="_delegates.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Contains event data associated with a Initialized event. 
    /// </summary>
    public class FolderBrowserInitializedEventArgs : EventArgs
    {
        /// <summary>
        /// FolderBrowser handle
        /// </summary>
        public readonly IntPtr Hwnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowserInitializedEventArgs"/> class.
        /// </summary>
        /// <param name="hwnd">The HWND IntPtr.</param>
        public FolderBrowserInitializedEventArgs(IntPtr hwnd)
        {
            this.Hwnd = hwnd;
        }
    }
    
    /// <summary>
    /// Contains event data associated with a IUnknown event. 
    /// </summary>
    public class FolderBrowserIUnknownEventArgs : EventArgs
    {
        /// <summary>
        /// FolderBrowser handle.
        /// </summary>
        public readonly IntPtr Hwnd;
        
        /// <summary>
        /// Reference to IUnknown interface
        /// </summary>
        public readonly IntPtr Iunknown;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowserIUnknownEventArgs"/> class.
        /// </summary>
        /// <param name="hwnd">The HWND IntPtr.</param>
        /// <param name="iunknown">The Iunknown.</param>
        public FolderBrowserIUnknownEventArgs(IntPtr hwnd, IntPtr iunknown)
        {
            this.Hwnd = hwnd;
            this.Iunknown = iunknown;
        }
    }
    
    /// <summary>
    /// Contains event data associated with a SelectedChanged event. 
    /// </summary>
    public class FolderBrowserSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// FolderBrowser handle.
        /// </summary>
        public readonly IntPtr Hwnd;
        
        /// <summary>
        /// Selected folder path
        /// </summary>
        public readonly string SelectedFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowserSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="hwnd">The HWND IntPtr.</param>
        /// <param name="selectedFolderPath">The selected folder path.</param>
        public FolderBrowserSelectionChangedEventArgs(IntPtr hwnd, string selectedFolderPath)
        {
            this.Hwnd = hwnd;
            this.SelectedFolderPath = selectedFolderPath;
        }
    }
    
    /// <summary>
    /// Contains event data associated with a ValidateFailed event. 
    /// </summary>
    public class FolderBrowserValidateFailedEventArgs : EventArgs
    {
        /// <summary>
        /// FolderBrowser handle.
        /// </summary>
        public readonly IntPtr Hwnd;
        
        /// <summary>
        /// Invalid path.
        /// </summary>
        public readonly string InvalidPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowserValidateFailedEventArgs"/> class.
        /// </summary>
        /// <param name="hwnd">The HWND IntPtr.</param>
        /// <param name="invalidPath">The invalid path.</param>
        public FolderBrowserValidateFailedEventArgs(IntPtr hwnd, string invalidPath)
        {
            this.Hwnd = hwnd;
            this.InvalidPath = invalidPath;
        }
    }

    /// <summary>
    /// Represents the delegate for handlers that receive Initialized event. 
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">InitializedEventArgs event arguments that contain information about the Initialized event.</param>
    public delegate void FolderBrowserInitializedEventHandler(Syncfusion.Windows.Tools.Controls.FolderBrowser sender, FolderBrowserInitializedEventArgs e);
    
    /// <summary>
    /// Represents the delegate for handlers that receive IUnknown event. 
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">IUnknownEventArgs event arguments that contain information about the IUnknown  event.</param>
    public delegate void FolderBrowserIUnknownEventHandler(Syncfusion.Windows.Tools.Controls.FolderBrowser sender, FolderBrowserIUnknownEventArgs e);
    
    /// <summary>
    /// Represents the delegate for handlers that receive SelectedChanged event. 
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">SelectedChangedEventArgs event arguments that contain information about the SelectedChanged event.</param>
    public delegate void FolderBrowserSelectionChangedEventHandler(Syncfusion.Windows.Tools.Controls.FolderBrowser sender, FolderBrowserSelectionChangedEventArgs e);
    
    /// <summary>
    /// Represents the delegate for handlers that receive ValifdationFailed event. 
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">ValidationFailedEventArgs event arguments that contain information about the ValidationFailed event.</param>
    /// <returns>int value type.</returns>
    public delegate int FolderBrowserValidateFailedEventHandler(Syncfusion.Windows.Tools.Controls.FolderBrowser sender, FolderBrowserValidateFailedEventArgs e);
}
