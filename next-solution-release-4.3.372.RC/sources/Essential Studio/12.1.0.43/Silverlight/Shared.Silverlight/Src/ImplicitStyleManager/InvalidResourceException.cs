#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "Syncfusion.Windows.Controls.Theming.InvalidResourceException", Justification = "Exception used to indicate a specific cause of failure.")]

namespace Syncfusion.Windows.Controls.Theming
{
    /// <summary>
    /// Exception thrown when resource is not found.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    internal class InvalidResourceException : Exception
    {
        /// <summary>
        /// Gets the uri of the resource that could not be found.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Initializes a new instance of the InvalidResourceException class.
        /// </summary>
        public InvalidResourceException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidResourceException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidResourceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidResourceException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidResourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidResourceException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="uri">The uri of the resource.</param>
        public InvalidResourceException(string message, Uri uri)
            : base(message)
        {
            Uri = uri;
        }

        /// <summary>
        /// Initializes a new instance of the InvalidResourceException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="uri">The uri of the resource.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidResourceException(string message, Uri uri, Exception innerException)
            : base(message, innerException)
        {
            Uri = uri;
        }
    }
}