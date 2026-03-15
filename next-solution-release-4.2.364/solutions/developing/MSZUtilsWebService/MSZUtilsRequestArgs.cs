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
namespace MSZUtilsWebService
{
    /// <summary>
    /// Event args for the AddingNewItem event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class MSZUWResponseArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of MSZEventArgs.
        /// </summary>
        public MSZUWResponseArgs()
        {
        }

        /// <summary>
        /// Constructs a new instance of MSZEventArgs.
        /// </summary>
        public MSZUWResponseArgs(MSZUWResponse r, MSZUWRequest c)
        {
            _MSZUWResponse = r;
            _ClientRequest = c;
        }

        private readonly MSZUWResponse _MSZUWResponse;
        public MSZUWResponse MSZUtilsWRequest
        {
            get { return _MSZUWResponse; }
        }
        private readonly MSZUWRequest _ClientRequest;
        public MSZUWRequest ClientRequest
        {
            get { return _ClientRequest; }
        }
    }
}
