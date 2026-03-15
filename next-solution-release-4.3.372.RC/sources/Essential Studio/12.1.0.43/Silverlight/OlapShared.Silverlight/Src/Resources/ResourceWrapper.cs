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
using System.Globalization;

namespace Syncfusion.OlapShared.Silverlight.Resources
{
    /// <summary>
    /// ResourceWrapper is used to apply the static resource to Silverlight-Components
    /// </summary>
    public sealed class ResourceWrapper
    {
        #region Constants
        const string COLUMNPAGER = "OlapPager_ColumnPager";
        const string ROWPAGER = "OlapPager_RowPager";
        const string COLUMN_PAGESETTINGS = "OlapPager_ColumnPageSettings";
        const string ROW_PAGESETTINGS = "OlapPager_RowPageSettings";
        const string CURRENT_PAGE = "OlapPager_CurrentPage";
        const string PAGE_SIZE = "OlapPager_PageSize";
        const string BUTTON_CANCEL = "OlapPager_Button_Cancel";
        const string BUTTON_OK = "OlapPager_Button_OK";
        const string PAGESETTINGS = "OlapPager_PageSettings";
        #endregion

        #region Constructor

        public ResourceWrapper()
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            columnPager = SR.GetString(culture, COLUMNPAGER);
            rowPager = SR.GetString(culture, ROWPAGER);
            columnPageSettings = SR.GetString(culture, COLUMN_PAGESETTINGS);
            rowPageSettings = SR.GetString(culture, ROW_PAGESETTINGS);
            currentPage = SR.GetString(culture, CURRENT_PAGE);
            pageSize = SR.GetString(culture, PAGE_SIZE);
            button_Cancel = SR.GetString(culture, BUTTON_CANCEL);
            button_OK = SR.GetString(culture, BUTTON_OK);
            pageSettings = SR.GetString(culture, PAGESETTINGS);
        }

        #endregion

        #region Private Variables

        private string columnPager;
        private string rowPager;
        private string columnPageSettings;
        private string rowPageSettings;
        private string pageSize;
        private string currentPage;
        private string  button_OK;
        private string button_Cancel;
        private string pageSettings;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or Set the ColumnPager for localization use.
        /// </summary>
        public string ColumnPager
        {
            get { return columnPager; }
            set { columnPager = value; }
        }

        /// <summary>
        /// Gets or Set the RowPager for localization use.
        /// </summary>
        public string RowPager
        {
            get { return rowPager; }
            set { rowPager = value; }
        }

        /// <summary>
        /// Gets or Set the ColumnPageSettings for localization use.
        /// </summary>
        public string ColumnPageSettings
        {
            get { return columnPageSettings; }
            set { columnPageSettings = value; }
        }

        /// <summary>
        /// Gets or Set the RowPageSettings for localization use.
        /// </summary>
        public string RowPageSettings
        {
            get { return rowPageSettings; }
            set { rowPageSettings = value; }
        }

        /// <summary>
        /// Gets or Set the PageSize for localization use.
        /// </summary>
        public string PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        /// <summary>
        /// Gets or Set the CurrentPage for localization use.
        /// </summary>
        public string CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
        }

        /// <summary>
        /// Gets or Set the Button_OK for localization use.
        /// </summary>
        public string Button_OK
        {
            get { return button_OK; }
            set { button_OK = value; }
        }

        /// <summary>
        /// Gets or Set the Button_Cancel for localization use.
        /// </summary>
        public string Button_Cancel
        {
            get { return button_Cancel; }
            set { button_Cancel = value; }
        }

        /// <summary>
        /// Gets or Set the PageSettings for localization use.
        /// </summary>
        public string PageSettings
        {
            get { return pageSettings; }
            set { pageSettings = value; }
        }

        #endregion
    }
}
