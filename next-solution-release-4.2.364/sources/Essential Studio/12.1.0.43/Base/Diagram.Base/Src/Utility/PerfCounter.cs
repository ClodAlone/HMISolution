#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class containing Perf Counter.
    /// </summary>
    public sealed class PerfCounter
        : IDisposable
    {
        #region Class members
        string m_strMessage;
        long m_start;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PerfCounter"/> class.
        /// </summary>
        /// <param name="strMessage">The STR message.</param>
        public PerfCounter(string strMessage)
        {
            m_strMessage = strMessage;
            m_start = 0;

            QueryPerformanceCounter(ref m_start);
        }
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            long finish = 0;
            QueryPerformanceCounter(ref finish);

            System.Diagnostics.Trace.WriteLine(new TimeSpan(finish - m_start), m_strMessage);
        }

        #endregion

        #region Class utility methods
        [DllImport("Kernel32.dll")]
        static extern bool QueryPerformanceCounter(ref long performanceCount);
        #endregion
    }
}
