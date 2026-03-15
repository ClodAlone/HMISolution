#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
#if DOCIO
using Syncfusion.CompoundFile.DocIO.Net;
#endif

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO
#else
namespace Syncfusion.CompoundFile.XlsIO
#endif
{
    /// <summary>
    /// This interface gives access to compound file functionality.
    /// </summary>
    public interface ICompoundFile : IDisposable
    {
        /// <summary>
        /// Returns root storage object for this file.
        /// </summary>
        ICompoundStorage RootStorage { get; }
#if DOCIO
        Directory Directory
        {
            get;
        }
#endif
        /// <summary>
        /// Flushes content into internal buffer.
        /// </summary>
        void Flush();
        /// <summary>
        /// Saves compound file into stream
        /// </summary>
        /// <param name="stream">Stream to save data into.</param>
        void Save(System.IO.Stream stream);
#if !(WINRT )
        /// <summary>
        /// Saves compound file into file.
        /// </summary>
        /// <param name="fileName">Name of the file to save into.</param>
        void Save(string fileName);
#endif
    }
}
