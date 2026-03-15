#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Class  represents the Delegates for FileUpload control
    /// </summary>
      public class FileUploadControlDelegates
    {
       /// <summary>                              
       /// Represents FileCountExceeded Delegate
       /// </summary>
       /// <param name="sender">FileUpload Control</param>
       /// <param name="args">Event Argument</param>
        public delegate void FileCountExceededEventHandler(object sender, FilesEventArgs args);

        /// <summary>                              
        /// Represents FilesSelected Delegate
        /// </summary>
        /// <param name="sender">FileUpload Control</param>
        /// <param name="args">Event Argument</param>
        public delegate void FilesSelectedEventHandler(object sender, FilesEventArgs args);

        /// <summary>                              
        /// Represents FilesTooLarge Delegate
        /// </summary>
        /// <param name="sender">FileUpload Control</param>
        /// <param name="args">Event Argument</param>
        public delegate void FilesTooLargeEventHandler(object sender, FilesEventArgs args);

        /// <summary>                              
        /// Represents TotalUploadSizeExceeded Delegate
        /// </summary>
        /// <param name="sender">FileUpload Control</param>
        /// <param name="args">Event Argument</param>
        public delegate void TotalUploadSizeExceededEventHandler(object sender, FilesEventArgs args);

        /// <summary>                              
        /// Represents FileExists Delegate
        /// </summary>
        /// <param name="sender">FileUpload Control</param>
        /// <param name="args">Event Argument</param>
        public delegate void FileExistsEventHandler(object sender, FileEventArgs args);

        /// <summary>                              
        /// Represents FileUploadStarting Delegate
        /// </summary>
        /// <param name="sender">FileUpload Control</param>
        /// <param name="args">Event Argument</param>
        public delegate void FileUploadStartingEventHandler(object sender, FileEventArgs args);

        /// <summary>                              
        /// Represents UploadPaused Delegate
        /// </summary>
        /// <param name="sender">FileUpload Control</param>
        /// <param name="args">Event Argument</param>
        public delegate void UploadPausedEventHandler(object sender, FileEventArgs args);

        /// <summary>                              
        /// Represents UploadResumed Delegate
        /// </summary>
        /// <param name="sender">FileUpload Control</param>
        /// <param name="args">Event Argument</param>
        public delegate void UploadResumedEventHandler(object sender, FileEventArgs args);

        /// <summary>                              
        /// Represents UploadFinished Delegate
        /// </summary>
        /// <param name="sender">FileUpload Control</param>
        /// <param name="args">Event Argument</param>
        public delegate void UploadFinishedEventHandler(object sender, FileEventArgs args);
    }

    /// <summary>
    /// Class that represents FilesEventArgs
    /// </summary>
     public class FilesEventArgs
    {
        /// <summary>
        /// Gets or Sets the FilesList Property
        /// </summary>
        public List<FileInfo> FilesList = new List<FileInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesEventArgs"/> class
        /// </summary>
        public FilesEventArgs()
        {
        }

       /// <summary>
        /// Initializes a new instance of the <see cref="FilesEventArgs"/> class
       /// </summary>
       /// <param name="filescount">The Files Count</param>
       /// <param name="fileslist">The List of Files</param>
        public FilesEventArgs(int filescount, List<FileInfo> fileslist)
        {
            this.FilesCount = filescount;
            this.FilesList = fileslist;
        }

        /// <summary>
        /// Gets or sets the FilesCount Property
        /// </summary>
        public int FilesCount { get; set; }
    }

    /// <summary>
    /// Class that represents FileEventArgs
    /// </summary>
     public class FileEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventArgs"/> class
       /// </summary>
       /// <param name="file">The UploadFileInfo</param>
        public FileEventArgs(UploadFileInfo file)
        {
           this.File = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventArgs"/> class
        /// </summary>
        public FileEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets the File Property
        /// </summary>
        public UploadFileInfo File { get; set; }
    }
}
