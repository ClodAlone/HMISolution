//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using MSZServiceCMS;
namespace MSZWebService
{
    /// <summary>
    /// Event args for the AddingNewItem event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class MSZRequestArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of MSZEventArgs.
        /// </summary>
        public MSZRequestArgs()
        {
        }

        /// <summary>
        /// Constructs a new instance of MSZEventArgs.
        /// </summary>
        public MSZRequestArgs(MSZWRequest r)
        {
            _MSZWRequest = r;
        }

        private readonly MSZWRequest _MSZWRequest;
        public MSZWRequest MSZWRequest
        {
            get { return _MSZWRequest; }
        }
    }
}
