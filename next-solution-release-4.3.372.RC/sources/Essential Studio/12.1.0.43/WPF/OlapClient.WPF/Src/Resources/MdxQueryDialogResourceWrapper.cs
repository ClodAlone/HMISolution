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

namespace Syncfusion.Windows.Client.Olap.Resources
{
    public class MdxQueryDialogResourceWrapper
    {
        #region Constants

        const string OLAPCLIENT_MDXQUERY_DLG_TITLE = "OlapClient_MdxQuery_Dlg_Title";
        const string OLAPCLIENT_MDXQUERY_DLG_OK = "OlapClient_MdxQuery_Dlg_Ok";

        #endregion

        #region Members

        private string olapClientMdxQueryDlgTitle;
        private string olapClientMdxQueryDlgOk;

        #endregion

        #region Constructor

        public MdxQueryDialogResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;
            olapClientMdxQueryDlgTitle = SR.GetString(ci, OLAPCLIENT_MDXQUERY_DLG_TITLE);
            olapClientMdxQueryDlgOk = SR.GetString(ci, OLAPCLIENT_MDXQUERY_DLG_OK);
        }
        #endregion

        #region Properties

        public string OlapClientMdxDlgQueryTitle
        {
            get { return olapClientMdxQueryDlgTitle; }
            set { olapClientMdxQueryDlgTitle = value; }
        }

        public string OlapClientMdxQueryDlgOk
        {
            get { return olapClientMdxQueryDlgOk; }
            set { olapClientMdxQueryDlgOk = value; }
        }
        #endregion
    }
}
