// <copyright file="DocumentContainer_Commands.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DocumentContainer class.
    /// </summary>

    public partial class DocumentContainer
    {
        #region Commands

        /// <summary>
        /// Command used to hide active document.
        /// </summary>
        public static readonly RoutedCommand HideDocumentCommand = new RoutedCommand("HideDocumentCommand", typeof(DocumentContainer));
        
        /// <summary>
        /// Command used to minimize active MDI document.
        /// </summary>
        public static readonly RoutedCommand MinimizeDocumentCommand = new RoutedCommand("MinimizeDocumentCommand", typeof(DocumentContainer));
        
        /// <summary>
        /// Command used to maximize active MDI document.
        /// </summary>
        public static readonly RoutedCommand MaximizeDocumentCommand = new RoutedCommand("MaximizeDocumentCommand", typeof(DocumentContainer));
        
        /// <summary>
        /// Command used to normalize active MDI document.
        /// </summary>
        public static readonly RoutedCommand RestoreDocumentCommand = new RoutedCommand("RestoreDocumentCommand", typeof(DocumentContainer));
        
        /// <summary>
        /// Command used to hide all documents.
        /// </summary>
        public static readonly RoutedCommand HideAllDocumentsCommand = new RoutedCommand("HideAllDocumentsCommand", typeof(DocumentContainer));
        
        /// <summary>
        /// Command used to minimize all MDI documents.
        /// </summary>
        public static readonly RoutedCommand MinimizeAllDocumentsCommand = new RoutedCommand("MinimizeAllDocumentsCommand", typeof(DocumentContainer));
        
        /// <summary>
        /// Command used to maximize all MDI documents.
        /// </summary>
        public static readonly RoutedCommand MaximizeAllDocumentsCommand = new RoutedCommand("MaximizeAllDocumentsCommand", typeof(DocumentContainer));
        
        /// <summary>
        /// Command used to normalize all MDI documents.
        /// </summary>
        public static readonly RoutedCommand RestoreAllDocumentsCommand = new RoutedCommand("RestoreAllDocumentsCommand", typeof(DocumentContainer));
        
        /// <summary>
        /// Command used to initiate moving the MDI document.
        /// </summary>
        public static readonly RoutedCommand BeginDocumentMovingCommand = new RoutedCommand("BeginDocumentMovingCommand", typeof(DocumentContainer));
        
        /// <summary>
        /// Command used to initiate resizing the document.
        /// </summary>
        public static readonly RoutedCommand BeginDocumentResizingCommand = new RoutedCommand("BeginDocumentResizingCommand", typeof(DocumentContainer));

        #endregion
    }
}
