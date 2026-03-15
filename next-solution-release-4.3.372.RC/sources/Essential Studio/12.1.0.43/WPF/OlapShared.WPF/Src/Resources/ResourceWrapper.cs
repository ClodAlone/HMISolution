#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Syncfusion.Windows.Shared.Olap.Resources
{
    internal sealed class ResourceWrapper
    {
        #region Constants

        const string OLAPTOOLS_FILTERSORTING_DLG_HEADER = "OlapTools_FilterSorting_Dlg_Header";

        const string OLAPTOOLS_FILTERSORTING_DLG_FILTER_TAB = "OlapTools_FilterSorting_Dlg_Filter_Tab";

        const string OLAPTOOLS_FILTERSORTING_DLG_SORTING_TAB = "OlapTools_FilterSorting_Dlg_Sorting_Tab";

        const string OLAPTOOLS_FILTERSORTING_DLG_EMPTYRESULT_HEADER = "OlapTools_FilterSorting_Dlg_EmptyResult_Header";

        const string OLAPTOOLS_FILTERSORTING_DLG_FILTER1_HEADER = "OlapTools_FilterSorting_Dlg_Filter1_Header";

        const string OLAPTOOLS_FILTERSORTING_DLG_FILTER2_HEADER = "OlapTools_FilterSorting_Dlg_Filter2_Header";

        const string OLAPTOOLS_FILTERSORTING_DLG_SORT_HEADER = "OlapTools_FilterSorting_Dlg_Sort_Header";

        const string OLAPTOOLS_FILTERSORTING_DLG_UPDATE = "OlapTools_FilterSorting_Dlg_Update";

        const string OLAPTOOLS_FILTERSORTING_DLG_CANCEL = "OlapTools_FilterSorting_Dlg_Cancel";

        const string OLAPTOOLS_FILTERSORTING_DLG_FILTEREMPTYCOLUMNS = "OlapTools_FilterSorting_Dlg_FilterEmptyColumns";

        const string OLAPTOOLS_FILTERSORTING_FILTERCONDITION = "OlapTools_FilterSorting_FilterCondition";

        const string OLAPTOOLS_FILTERSORTING_DLG_FILTERON = "OlapTools_FilterSorting_Dlg_FilterOn";

        const string OLAPTOOLS_FILTERSORTING_DLG_FILTERONVALUE = "OlapTools_FilterSorting_Dlg_FilterOnValue";

        const string OLAPTOOLS_FILTERSORTING_DLG_SORTINGON = "OlapTools_FilterSorting_Dlg_SortingOn";

        const string OLAPTOOLS_FILTERSORTING_DLG_SORTASC = "OlapTools_FilterSorting_Dlg_SortAsc";

        const string OLAPTOOLS_FILTERSORTING_DLG_SORTDESC = "OlapTools_FilterSorting_Dlg_SortDesc";

        const string OLAPTOOLS_FILTERSORTING_DLG_SORT_PRESERVER_HIERARCHY = "OlapTools_FilterSorting_Dlg_Sort_Preserver_Hierarchy";
        
        const string OLAPTOOLS_FILTERSORTING_DLG_FILTEREMPTYROWS = "OlapTools_FilterSorting_Dlg_FilterEmptyRows";

        const string OLAPTOOLS_FILTERSORTING_DLG_ONCOLUMN = "OlapTools_FilterSorting_Dlg_OnColumn";

        const string OLAPTOOLS_FILTERSORTING_DLG_ONROW = "OlapTools_FilterSorting_Dlg_OnRow";

        #endregion

        #region Members

        private string olapToolsFilterSortingDlgHeader;

        private string olapToolsFilterSortingDlgFilterTab;

        private string olapToolsFilterSortingDlgSortingTab;

        private string olapToolsFilterSortingDlgEmptyResultHeader;

        private string olapToolsFilterSortingDlgFilter1Header;

        private string olapToolsFilterSortingDlgFilter2Header;

        private string olapToolsFilterSortingDlgSortHeader;

        private string olapToolsFilterSortingDlgUpdate;

        private string olapToolsFilterSortingDlgCancel;

        private string olapToolsFilterSortingDlgFilterEmptyColumns;

        private string olapToolsFilterSortingFilterCondition;

        private string olapToolsFilterSortingDlgFilterOn;

        private string olapToolsFilterSortingDlgFilterOnValue;

        private string olapToolsFilterSortingDlgSortingOn;

        private string olapToolsFilterSortingDlgSortAsc;

        private string olapToolsFilterSortingDlgSortDesc;

        private string olapToolsFilterSortingDlgSortPreserverHierarchy;

        private string olapToolsFilterSortingDlgFilterEmptyRows;

        private string olapToolsFilterSortingDlgOnColumn;

        private string olapToolsFilterSortingDlgOnRow;

        #endregion

        #region Constructor

        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            olapToolsFilterSortingDlgHeader = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_HEADER);

            olapToolsFilterSortingDlgFilterTab = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_FILTER_TAB);

            olapToolsFilterSortingDlgSortingTab = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_SORTING_TAB);

            olapToolsFilterSortingDlgEmptyResultHeader = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_EMPTYRESULT_HEADER);

            olapToolsFilterSortingDlgFilter1Header = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_FILTER1_HEADER);

            olapToolsFilterSortingDlgFilter2Header = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_FILTER2_HEADER);

            olapToolsFilterSortingDlgSortHeader = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_SORT_HEADER);

            olapToolsFilterSortingDlgUpdate = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_UPDATE);

            olapToolsFilterSortingDlgCancel = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_CANCEL);

            olapToolsFilterSortingDlgFilterEmptyColumns = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_FILTEREMPTYCOLUMNS);

            olapToolsFilterSortingFilterCondition = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_FILTERCONDITION);

            olapToolsFilterSortingDlgFilterOn = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_FILTERON);

            olapToolsFilterSortingDlgFilterOnValue = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_FILTERONVALUE);

            olapToolsFilterSortingDlgSortingOn = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_SORTINGON);

            olapToolsFilterSortingDlgSortAsc = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_SORTASC);

            olapToolsFilterSortingDlgSortDesc = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_SORTDESC);

            olapToolsFilterSortingDlgSortPreserverHierarchy = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_SORT_PRESERVER_HIERARCHY);

            olapToolsFilterSortingDlgFilterEmptyRows = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_FILTEREMPTYROWS);

            olapToolsFilterSortingDlgOnColumn = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_ONCOLUMN);

            olapToolsFilterSortingDlgOnRow = SR.GetString(ci, OLAPTOOLS_FILTERSORTING_DLG_ONROW);
        }

        #endregion

        #region Properties

        public string OlapToolsFilterSortingDlgHeader
        {
            get { return olapToolsFilterSortingDlgHeader; }
            set { olapToolsFilterSortingDlgHeader = value; }
        }

        public string OlapToolsFilterSortingDlgFilterTab
        {
            get { return olapToolsFilterSortingDlgFilterTab; }
            set { olapToolsFilterSortingDlgFilterTab = value; }
        }

        public string OlapToolsFilterSortingDlgSortingTab
        {
            get { return olapToolsFilterSortingDlgSortingTab; }
            set { olapToolsFilterSortingDlgSortingTab = value; }
        }

        public string OlapToolsFilterSortingDlgEmptyResultHeader
        {
            get { return olapToolsFilterSortingDlgEmptyResultHeader; }
            set { olapToolsFilterSortingDlgEmptyResultHeader = value; }
        }

        public string OlapToolsFilterSortingDlgFilter1Header
        {
            get { return olapToolsFilterSortingDlgFilter1Header; }
            set { olapToolsFilterSortingDlgFilter1Header = value; }
        }

        public string OlapToolsFilterSortingDlgFilter2Header
        {
            get { return olapToolsFilterSortingDlgFilter2Header; }
            set { olapToolsFilterSortingDlgFilter2Header = value; }
        }

        public string OlapToolsFilterSortingDlgSortHeader
        {
            get { return olapToolsFilterSortingDlgSortHeader; }
            set { olapToolsFilterSortingDlgSortHeader = value; }
        }

        public string OlapToolsFilterSortingDlgUpdate
        {
            get { return olapToolsFilterSortingDlgUpdate; }
            set { olapToolsFilterSortingDlgUpdate = value; }
        }

        public string OlapToolsFilterSortingDlgCancel
        {
            get { return olapToolsFilterSortingDlgCancel; }
            set { olapToolsFilterSortingDlgCancel = value; }
        }

        public string OlapToolsFilterSortingDlgFilterEmptyColumns
        {
            get { return olapToolsFilterSortingDlgFilterEmptyColumns; }
            set { olapToolsFilterSortingDlgFilterEmptyColumns = value; }
        }

        public string OlapToolsFilterSortingFilterCondition
        {
            get { return olapToolsFilterSortingFilterCondition; }
            set { olapToolsFilterSortingFilterCondition = value; }
        }

        public string OlapToolsFilterSortingDlgFilterOn
        {
            get { return olapToolsFilterSortingDlgFilterOn; }
            set { olapToolsFilterSortingDlgFilterOn = value; }
        }

        public string OlapToolsFilterSortingDlgFilterOnValue
        {
            get { return olapToolsFilterSortingDlgFilterOnValue; }
            set { olapToolsFilterSortingDlgFilterOnValue = value; }
        }

        public string OlapToolsFilterSortingDlgSortingOn
        {
            get { return olapToolsFilterSortingDlgSortingOn; }
            set { olapToolsFilterSortingDlgSortingOn = value; }
        }

        public string OlapToolsFilterSortingDlgSortAsc
        {
            get { return olapToolsFilterSortingDlgSortAsc; }
            set { olapToolsFilterSortingDlgSortAsc = value; }
        }

        public string OlapToolsFilterSortingDlgSortDesc
        {
            get { return olapToolsFilterSortingDlgSortDesc; }
            set { olapToolsFilterSortingDlgSortDesc = value; }
        }

        public string OlapToolsFilterSortingDlgSortPreserverHierarchy
        {
            get { return olapToolsFilterSortingDlgSortPreserverHierarchy; }
            set { olapToolsFilterSortingDlgSortPreserverHierarchy = value; }
        }

        public string OlapToolsFilterSortingDlgFilterEmptyRows
        {
            get { return olapToolsFilterSortingDlgFilterEmptyRows; }
            set { olapToolsFilterSortingDlgFilterEmptyRows = value; }
        }

        public string OlapToolsFilterSortingDlgOnRow
        {
            get { return olapToolsFilterSortingDlgOnRow; }
            set { olapToolsFilterSortingDlgOnRow = value; }
        }

        public string OlapToolsFilterSortingDlgOnColumn
        {
            get { return olapToolsFilterSortingDlgOnColumn; }
            set { olapToolsFilterSortingDlgOnColumn = value; }
        }

        #endregion
    }
}
