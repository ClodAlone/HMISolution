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
namespace MSZService
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
        public MSZRequestArgs(MSZRequest r)
        {
            _MSZRequest = r;
        }

        private readonly MSZRequest _MSZRequest;
        public MSZRequest MSZRequest
        {
            get { return _MSZRequest; }
        }
    }
}
