#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;

namespace Syncfusion.Windows.Controls.Grid
{
    public interface IGridCurrentCellRenderer
    {
        bool OnActivating(int rowIndex, int colIndex);
        void OnActivated();
        void OnInitialize(int rowIndex, int colIndex);
        bool OnDeactivating();
        void OnDeactived(int rowIndex, int colIndex);

        void OnBeginEdit();
        void OnEndEdit();

        void OnRejectChanges();
        bool OnValidate();
        void OnValidated();
        bool OnSaveChanges();
        bool OnStartEditing();
        void OnEditingComplete();

        bool OnDeleting();

        void OnCloseDropDown();
        void OnShowDropDown();

        //void OnPrepareRenderCell();//GridPrepareViewStyleInfoEventArgs e)
        //void OnButtonClicked(int rowIndex, int colIndex, int button);
        //void OnCancelMode(int rowIndex, int colIndex);
        //void OnClick(int rowIndex, int colIndex);//, MouseEventArgs e)
        //void OnControlDoubleClick();
        //void OnDoubleClick(int rowIndex, int colIndex);//, MouseEventArgs e)
        //void OnGridGotFocus(EventArgs e);
        //void OnHasFocusControlChanged();
        //Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds);
        //bool OnScrollInView();
        //void OnSetControlText(string text);
    }


}
