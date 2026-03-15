#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Globalization;

namespace Syncfusion.Silverlight.Chart.Olap.Resources
{
    /// <summary>
    /// ResourceWrapper is used to apply the static resource to Silverlight-Components
    /// </summary>
    public sealed class ResourceWrapper
    {
        #region Constant Members
        
        const string MEASURE_VALUE = "OlapChart_Tooltip_Measure";
        const string ROW_VALUE = "OlapChart_Tooltip_Row";
        const string COLUMN_VALUE = "OlapChart_Tooltip_Column";
        const string VALUECELL_VALUE = "OlapChart_Tooltip_Value"; 
        const string LOADING_INDICATOR_TEXT = "OlapChart_LoadingIndicator_Text";

        #endregion

        #region Members

        private string measureValue;
        private string rowValue;
        private string columnValue;
        private string valueCellValue;
        private string loadingIndicatorText;

        #endregion

        #region Constructor
        
        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            measureValue = SR.GetString(ci, MEASURE_VALUE);
            rowValue = SR.GetString(ci, ROW_VALUE);
            columnValue = SR.GetString(ci, COLUMN_VALUE);
            valueCellValue = SR.GetString(ci, VALUECELL_VALUE);
            loadingIndicatorText = SR.GetString(ci, LOADING_INDICATOR_TEXT);
        } 

        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the MeasureValue for localization use.
        /// </summary>
        public string MeasureValue
        {
            get { return measureValue; }
            set { measureValue = value; }
        }

        /// <summary>
        /// Gets or Sets the RowValue for localization use.
        /// </summary>
        public string RowValue
        {
            get { return rowValue; }
            set { rowValue = value; }
        }

        /// <summary>
        /// Gets or Sets the ColumnValue for localization use.
        /// </summary>
        public string ColumnValue
        {
            get { return columnValue; }
            set { columnValue = value; }
        }

        /// <summary>
        /// Gets or Sets the ValueCellValue for localization use.
        /// </summary>
        public string ValueCellValue
        {
            get { return valueCellValue; }
            set { valueCellValue = value; }
        }

        /// <summary>
        /// Gets or Sets the LoadingIndicatorText for localization use.
        /// </summary>
        public string LoadingIndicatorText
        {
            get { return loadingIndicatorText; }
            set { loadingIndicatorText = value; }
        }

        #endregion
    }
}
