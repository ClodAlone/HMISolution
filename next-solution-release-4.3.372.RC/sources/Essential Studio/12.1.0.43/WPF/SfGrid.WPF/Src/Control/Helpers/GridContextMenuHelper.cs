#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.UI.Xaml.Grid
{
    public class GridContextMenuInfo : INotifyPropertyChanged
    {
        SfDataGrid _DataGrid;
        public SfDataGrid DataGrid
        {
            get { return _DataGrid; }
            internal set
            {
                _DataGrid = value;
                this.OnPropertyChanged("DataGrid");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class GridColumnContextMenuInfo : GridContextMenuInfo
    {
        GridColumn _Column;
        public GridColumn Column 
        {
            get { return _Column; }
            internal set
            {
                _Column = value;
                this.OnPropertyChanged("Column");
            }
        }
                
    }

    public class GridRecordContextMenuInfo : GridContextMenuInfo
    {
        object _Record;
        public object Record
        {
            get { return _Record; }
            internal set
            {
                _Record = value;
                this.OnPropertyChanged("Record");
            }
        }
    }

    public class GridGroupDropAreaContextMenuInfo : GridContextMenuInfo
    {

    }

    public delegate void GridContextMenuOpeningEventHandler(object sender, GridContextMenuEventArgs e);

    public class GridContextMenuEventArgs : EventArgs
    {

        public GridContextMenuEventArgs(ContextMenu _contextMenu, object _contextMenuInfo, RowColumnIndex _rowColumnIndex, ContextMenuType _contextMenuType)
        {
            ContextMenu = _contextMenu;
            ContextMenuInfo = _contextMenuInfo;
            RowColumnIndex = _rowColumnIndex;
            ContextMenuType = _contextMenuType;
        }

        public bool Handled { get; set; }

        private ContextMenu contextMenu;
        public ContextMenu ContextMenu
        {
            get { return contextMenu; }
            set { contextMenu = value; }
        }

        private object contextMenuInfo;
        public object ContextMenuInfo
        {
            get { return contextMenuInfo; }
            set { contextMenuInfo = value; }
        }

        private RowColumnIndex rowColumnIndex;
        public RowColumnIndex RowColumnIndex
        {
            get
            {
                if (rowColumnIndex == null)
                    return RowColumnIndex.Empty;

                return rowColumnIndex;
            }
            set
            {
                rowColumnIndex = value;
            }
        }

        private ContextMenuType contextMenuType;
        public ContextMenuType ContextMenuType
        {
            get { return contextMenuType; }
            set { contextMenuType = value; }
        }

    }


}
