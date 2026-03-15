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
	internal sealed class ReportNameResourceWrapper
	{
		#region Constants
		
		const string OLAPCLIENT_REPORTNAME_DLG_REPORTNAME = "OlapClient_ReportName_Dlg_ReportName";

        const string OLAPCLIENT_CONNECT_DLG_OK = "OlapClient_Connect_Dlg_Ok";

		const string OLAPCLIENT_CONNECT_DLG_CANCEL = "OlapClient_Connect_Dlg_Cancel";
		
		#endregion
		
		#region Members
		
		private string olapClientReportNameDlgReportName;

		private string olapClientReportNameDlgOk;

		private string olapClientReportNameDlgCancel;
		
		#endregion
	
		#region Constructor
		
		public ReportNameResourceWrapper()
		{
			CultureInfo ci = CultureInfo.CurrentUICulture;
			
			olapClientReportNameDlgReportName = SR.GetString(ci, OLAPCLIENT_REPORTNAME_DLG_REPORTNAME);

            olapClientReportNameDlgOk = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_OK);

			olapClientReportNameDlgCancel = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_CANCEL);
		} 
		
		#endregion
		
		#region Properties
		
		public string OlapClientReportNameDlgReportName
        {
            get { return olapClientReportNameDlgReportName; }
            set { olapClientReportNameDlgReportName = value; }
        }

		public string OlapClientReportNameDlgOk
        {
            get { return olapClientReportNameDlgOk; }
            set { olapClientReportNameDlgOk = value; }
        }

		public string OlapClientReportNameDlgCancel
        {
            get { return olapClientReportNameDlgCancel; }
            set { olapClientReportNameDlgCancel = value; }
        }


		
		#endregion
	}
}
