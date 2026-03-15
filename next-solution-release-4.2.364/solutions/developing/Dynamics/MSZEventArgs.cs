//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
namespace MSZ
{
    /// <summary>
    /// Event args for the AddingNewItem event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class MSZEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of MSZEventArgs.
        /// </summary>
        public MSZEventArgs()
        {
            _MSZStateChanged = false;
            _MSZState = true;
            _MSZSleepTime = 0;
        }

        /// <summary>
        /// Constructs a new instance of MSZEventArgs.
        /// </summary>
        public MSZEventArgs(bool s, double t, bool c)
        {
            _MSZState = s;
            _MSZSleepTime = t;
            _MSZStateChanged = c;
        }

        private readonly bool _MSZStateChanged;
        public bool MSZStateChanged
        {
            get { return _MSZStateChanged; }
        }

        private readonly bool _MSZState;
        public bool MSZState
        {
            get { return _MSZState; }
        }
        private readonly double _MSZSleepTime;
        public double MSZSleepTime
        {
            get { return _MSZSleepTime; }
        }
    }
}
