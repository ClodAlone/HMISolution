#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Events
{
    /// <summary>
    /// Used in split container events to pass info about changed values to 
    /// event handlers.
    /// </summary>
    public class SplitterMoveEventArgs : EventArgs
    {
        #region Class members

        private PointF m_oldSplitPosition = PointF.Empty;

        private PointF m_newSplitPosition = PointF.Empty;
        #endregion

        #region Class properties

        public PointF OldSplitPosition
        {
            get
            {
                return m_oldSplitPosition;
            }
        }

        public PointF NewSplitPosition
        {
            get
            {
                return m_newSplitPosition;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
   
        public SplitterMoveEventArgs(PointF oldSplitPosition, PointF newSplitPosition)
        {
            m_oldSplitPosition = oldSplitPosition;
            m_newSplitPosition = newSplitPosition;
        }
        #endregion
    }

    #region Event Delegates
    /// <summary>
    /// Used for Splitter moving events.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e"> EventArgs that contains the event data.</param>
    public delegate void SplitterMoveEventHandler(object sender, SplitterMoveEventArgs e);
    #endregion
}