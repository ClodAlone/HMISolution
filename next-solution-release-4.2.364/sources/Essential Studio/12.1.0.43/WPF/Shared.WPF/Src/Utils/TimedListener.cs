// <copyright file="TimedListener.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Trace listener that adds date and time to the written text.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class TimedListener : TextWriterTraceListener
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="TimedListener"/> class.
        /// </summary>
        /// <param name="stream">Stream to be used for output.</param>
        public TimedListener(Stream stream)
            : base(stream)
        {
            IndentSize = 4;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Writes message along with the date and time of the event to the output stream.
        /// </summary>
        /// <param name="message">Message to be written.</param>
        public override void WriteLine(string message)
        {
            string[] strs = message.Split('\n');
            string timeHeader = "[" + DateTime.Now.ToString("hh:mm:ss.ffff") + "] ";

            if (this.IndentLevel > 0)
            {
                timeHeader += new string(' ', IndentLevel * IndentSize);
            }

            string indent = new string(' ', timeHeader.Length);

            for (int i = 0; i < strs.Length; i++)
            {
                if (i == 0)
                {
                    message = timeHeader + strs[0];
                }
                else
                {
                    message = indent + strs[i];
                }

                this.NeedIndent = false;
                base.WriteLine(message);
            }
        }
        #endregion
    }
}
