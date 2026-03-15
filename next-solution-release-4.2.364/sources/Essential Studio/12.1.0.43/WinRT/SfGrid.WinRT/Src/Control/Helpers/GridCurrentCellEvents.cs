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

namespace Syncfusion.UI.Xaml.Grid
{
    #region Event Arguments and Handlers

    #region CurrentCellActivatingEventHandler

    public delegate void CurrentCellActivatingEventHandler(object sender, CurrentCellActivatingEventArgs args);

    public class CurrentCellActivatingEventArgs : GridCancelEventArgs
    {
        public CurrentCellActivatingEventArgs(object originalSource)
            : base(originalSource)
        {

        }

        public RowColumnIndex CurrentRowColumnIndex
        {
            get;
            internal set;
        }

        public RowColumnIndex PreviousRowColumnIndex
        {
            get;
            internal set;
        }
        public ActivationTrigger ActivationTrigger
        {
            get;
            internal set;
        }
    }
    #endregion

    #region CurrentCellActivatedEventHandler
    public delegate void CurrentCellActivatedEventHandler(object sender, CurrentCellActivatedEventArgs args);

    public class CurrentCellActivatedEventArgs : GridEventArgs
    {
        public CurrentCellActivatedEventArgs(object originalSource)
            : base(originalSource)
        {
            
        }

        public RowColumnIndex CurrentRowColumnIndex
        {
            get;
            internal set;
        }
        public RowColumnIndex PreviousRowColumnIndex
        {
            get;
            internal set;
        }
        public ActivationTrigger ActivationTrigger
        {
            get;
            internal set;
        }
    }

    #endregion

    #region CurrentCellBeginEditEventHandler

    public delegate void CurrentCellBeginEditEventHandler(object sender, CurrentCellBeginEditEventArgs args);

    public class CurrentCellBeginEditEventArgs : GridCancelEventArgs
    {
        public CurrentCellBeginEditEventArgs(object originalSource)
            : base(originalSource)
        {
            
        }

        public RowColumnIndex RowColumnIndex
        {
            get;
            internal set;
        }

        public GridColumn Column
        {
            get;
            internal set;
        }
    }
    #endregion

    #region CurrentCellEndEditEventHandler

    public delegate void CurrentCellEndEditEventHandler(object sender, CurrentCellEndEditEventArgs args);

    public class CurrentCellEndEditEventArgs : GridEventArgs
    {
        public CurrentCellEndEditEventArgs(object originalSource)
            : base(originalSource)
        {
            
        }

        public RowColumnIndex RowColumnIndex
        {
            get;
            internal set;
        }
    }

    #endregion

    #region CurrentCellValidatingEventHandler

    public delegate void CurrentCellValidatingEventHandler(object sender, CurrentCellValidatingEventArgs args);


    public class CurrentCellValidatingEventArgs : GridEventArgs
    {
        public CurrentCellValidatingEventArgs(object originalSource)
            : base(originalSource)
        {
            
        }

        public GridColumn Column
        {
            get;
            internal set;
        }
        public object OldValue
        {
            get;
            internal set;
        }
        public object NewValue
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public bool IsValid
        {
            get;
            set;
        }

        public object RowData
        {
            get;
            internal set;
        }
    }

    #endregion

    #region CurrentCellValidatedEventHandler

    public delegate void CurrentCellValidatedEventHandler(object sender, CurrentCellValidatedEventArgs args);

    public class CurrentCellValidatedEventArgs : GridEventArgs
    {
        public CurrentCellValidatedEventArgs(object originalSource)
            : base(originalSource)
        {
            
        }

        public GridColumn Column
        {
            get;
            internal set;
        }
        public object OldValue
        {
            get;
            internal set;
        }
        public object NewValue
        {
            get;
            internal set;
        }

        public string ErrorMessage
        {
            get;
            internal set;
        }

        public object RowData
        {
            get;
            internal set;
        }
    }


    #region CurrentCellSelectionChangedEventHandler

    public delegate void CurrentCellDropDownSelectionChangedEventHandler(object sender, CurrentCellDropDownSelectionChangedEventArgs args);

    public class CurrentCellDropDownSelectionChangedEventArgs : EventArgs
    {
        public object SelectedItem { get; internal set; }
        public int SelectedIndex { get; internal set; }
        public RowColumnIndex RowColumnIndex { get; internal set; }
    }


    #endregion


    #region CurrentCellValueChangedEventHandler

    public delegate void CurrentCellValueChangedEventHandler(object sender, CurrentCellValueChangedEventArgs args);

    public class CurrentCellValueChangedEventArgs:EventArgs
    {
        public RowColumnIndex RowColumnIndex
        {
            get; 
            internal set;
        }
    }

    #endregion

    #region CurrentCellRequestNavigateEventHandler

    public delegate void CurrentCellRequestNavigateEventHandler(object sender, CurrentCellRequestNavigateEventArgs args);

    public class CurrentCellRequestNavigateEventArgs : EventArgs
    {
        public string NavigateText { get; internal set; }
        public object RowData { get; internal set; }
        public bool Handled { get; internal set; }
        public RowColumnIndex RowColumnIndex { get; internal set; }
    }

    #endregion

    #region RowValidatingEventHandler

    public delegate void RowValidatingEventHandler(object sender, RowValidatingEventArgs args);

    public class RowValidatingEventArgs : EventArgs
    {
        public RowValidatingEventArgs(object _rowData, int _rowIndex, Dictionary<string, string> errorMessages)
        {
            RowData = _rowData;
            RowIndex = _rowIndex;
            ErrorMessages = errorMessages;
        }

        bool isValid = true;
        public bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }

        object rowData;
        public object RowData
        {
            get { return rowData; }
            internal set { rowData = value; }
        }

        int rowIndex;
        public int RowIndex
        {
            get { return rowIndex; }
            internal set { rowIndex = value; }
        }

        /// <summary>
        /// Gets or sets the Error messanges for validation.
        /// </summary>
        /// <value>Column's Mapping as Key and Error message as Value</value>
        /// <remarks></remarks>
        public Dictionary<string, string> ErrorMessages
        {
            get;
            internal set;
        }
    }

    #endregion


    #region RowValidatedEventHandler

    public delegate void RowValidatedEventHandler(object sender, RowValidatedEventArgs args);

    public class RowValidatedEventArgs : EventArgs
    {
        public RowValidatedEventArgs(object _rowData, int _rowIndex, Dictionary<string, string> errorMessages)
        {
            RowData = _rowData;
            RowIndex = _rowIndex;
            ErrorMessages = errorMessages;
        }

        
        object rowData;
        public object RowData
        {
            get { return rowData; }
            internal set { rowData = value; }
        }

        int rowIndex;
        public int RowIndex
        {
            get { return rowIndex; }
            internal set { rowIndex = value; }
        }

        /// <summary>
        /// Gets or sets the Error messanges for validation.
        /// </summary>
        /// <value>Column's Mapping as Key and Error message as Value</value>
        /// <remarks></remarks>
        public Dictionary<string, string> ErrorMessages
        {
            get;
            internal set;
        }
    }

    #endregion

    #endregion

    #endregion

}