#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.OlapSilverlight.Engine;

namespace Syncfusion.Silverlight.Chart.Olap
{
    public class OlapLabelClickEvenArgs : EventArgs
    {
        private PivotCellDescriptor m_cellDescriptor;
        public PivotCellDescriptor CellDescriptor
        {
            get
            {
                return m_cellDescriptor;
            }
            set
            {
                m_cellDescriptor = value;
            }
        }
    }

    public class DataRefreshCompletedEventArgs : EventArgs
    {
        public DataRefreshCompletedEventArgs()
        {
        }
    }

    public class DataRefreshBeginEventArgs : EventArgs
    {
        public DataRefreshBeginEventArgs()
        {
        }

        public DataRefreshBeginEventArgs(RefreshType refreshType)
        {
            this.RefreshType = refreshType;
        }

        public DataRefreshBeginEventArgs(RefreshType refreshType, PivotCellDescriptor cellDescriptor)
        {
            this.RefreshType = refreshType;
            this.CellDescriptor = cellDescriptor;
        }


        public RefreshType RefreshType
        {
            get;
            internal set;
        }

        public PivotCellDescriptor CellDescriptor
        {
            get;
            internal set;
        }

        private bool m_handle;
        public bool Handled
        {
            get
            {
                return m_handle;
            }
            set
            {
                m_handle = value;
            }
        }
    }

    public enum RefreshType
    {
        DataBind,
        Drilldown
    }
}
