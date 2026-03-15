#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;

namespace Syncfusion.Windows.Tools.Controls
{
    public class FileSavingEventArgs : RoutedEventArgs
    {
        public FileSavingEventArgs()
        {

        }

        /// <summary>
        /// Gets or sets the doucment stream.
        /// </summary>
        /// <value>
        /// The doucment stream.
        /// </value>
        public Stream DoucmentStream
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the document extension.
        /// </summary>
        /// <value>
        /// The document extension.
        /// </value>
        public string FormatType
        {
            get;
            internal set;
        }

#if !WPF
        public bool Handled
        {
            get;
            set;
        }
#endif
    }

    public class SaveFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFailedEventArgs" /> class.
        /// </summary>
        public SaveFailedEventArgs()
        {
        }
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get;
            set;
        }
    }
}
