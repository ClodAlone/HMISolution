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
	internal sealed class ConnectionDialogResourceWrapper
	{
		#region Constants
		
		const string OLAPCLIENT_CONNECT_DLG_HEADER = "OlapClient_Connect_Dlg_Header";

		const string OLAPCLIENT_CONNECT_DLG_DATA_SOURCE_HEADER = "OlapClient_Connect_Dlg_Data_Source_Header";

		const string OLAPCLIENT_CONNECT_DLG_OFFLINE_CUBE_HEADER = "OlapClient_Connect_Dlg_Offline_Cube_Header";

		const string OLAPCLIENT_CONNECT_DLG_SERVER_HEADER = "OlapClient_Connect_Dlg_Server_Header";

		const string OLAPCLIENT_CONNECT_DLG_CONNECTION_STRING_HEADER = "OlapClient_Connect_Dlg_Connection_String_Header";

        const string OLAPCLIENT_CONNECT_DLG_PROVIDER_NAME_HEADER = "OlapClient_Connect_Dlg_Provider_Name_Header";

        const string OLAPCLIENT_CONNECT_DLG_PROVIDER_SSAS = "OlapClient_Connect_Dlg_Provider_Name_SSAS";

        const string OLAPCLIENT_CONNECT_DLG_PROVIDER_MONDRIAN = "OlapClient_Connect_Dlg_Provider_Name_Mondrian";

        const string OLAPCLIENT_CONNECT_DLG_PROVIDER_ACTIVEPIVOT = "OlapClient_Connect_Dlg_Provider_Name_ActivePivot";

        const string OLAPCLIENT_CONNECT_DLG_PROVIDER_SSAS_TOOLTIP = "OlapClient_Connect_Dlg_Provider_Name_SSAS_ToolTip";

        const string OLAPCLIENT_CONNECT_DLG_PROVIDER_MONDRIAN_TOOLTIP = "OlapClient_Connect_Dlg_Provider_Name_Mondrian_ToolTip";

        const string OLAPCLIENT_CONNECT_DLG_PROVIDER_ACTIVEPIVOT_TOOLTIP = "OlapClient_Connect_Dlg_Provider_Name_ActivePivot_ToolTip";

		const string OLAPCLIENT_CONNECT_DLG_SERVERNAME = "OlapClient_Connect_Dlg_ServerName";

		const string OLAPCLIENT_CONNECT_DLG_DATABASENAME = "OlapClient_Connect_Dlg_DatabaseName";

		const string OLAPCLIENT_CONNECT_DLG_CREDENTIAL = "OlapClient_Connect_Dlg_Credential";

		const string OLAPCLIENT_CONNECT_DLG_BROWSE = "OlapClient_Connect_Dlg_Browse";

		const string OLAPCLIENT_CONNECT_DLG_OK = "OlapClient_Connect_Dlg_Ok";

		const string OLAPCLIENT_CONNECT_DLG_CANCEL = "OlapClient_Connect_Dlg_Cancel";

        const string OLAPCLIENT_CONNECT_DLG_ENTERCREDENTIAL_HEADER = "OlapClient_Connect_Dlg_EnterCredential_Header";

        const string OLAPCLIENT_CONNECT_DLG_USER_NAME = "OlapClient_Connect_Dlg_User_Name";

        const string OLAPCLIENT_CONNECT_DLG_PASSWORD = "OlapClient_Connect_Dlg_Password";

        const string OLAPCLIENT_CONNECTOPTIONS_DLG_TITLE = "OlapClient_ConnectOptions_Dlg_Title";

		#endregion
		
		#region Members
		
		private string olapClientConnectDlgHeader;

		private string olapClientConnectDlgDataSourceHeader;

		private string olapClientConnectDlgOfflineCubeHeader;

		private string olapClientConnectDlgServerHeader;

		private string olapClientConnectDlgConnectionStringHeader;

        private string olapClientConnectDlgProviderNameHeader;

        private string olapClientConnectDlgProviderSSAS;

        private string olapClientConnectDlgProviderMondrian;

        private string olapClientConnectDlgProviderActivePivot;

        private string olapClientConnectDlgProviderSSASToolTip;

        private string olapClientConnectDlgProviderMondrianToolTip;

        private string olapClientConnectDlgProviderActivePivotToolTip;

		private string olapClientConnectDlgServerName;

		private string olapClientConnectDlgDatabaseName;

		private string olapClientConnectDlgCredential;

		private string olapClientConnectDlgBrowse;

		private string olapClientConnectDlgOk;

		private string olapClientConnectDlgCancel;

        private string olapClientConnectDlgEnterCredentialHeader;

        private string olapClientConnectDlgUserName;

        private string olapClientConnectDlgPassword;

        private string olapClientConnectOptionsDlgTitle;

		#endregion
	
		#region Constructor
		
		public ConnectionDialogResourceWrapper()
		{
			CultureInfo ci = CultureInfo.CurrentUICulture;
			
			olapClientConnectDlgHeader = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_HEADER);

			olapClientConnectDlgDataSourceHeader = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_DATA_SOURCE_HEADER);

			olapClientConnectDlgOfflineCubeHeader = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_OFFLINE_CUBE_HEADER);

			olapClientConnectDlgServerHeader = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_SERVER_HEADER);

			olapClientConnectDlgConnectionStringHeader = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_CONNECTION_STRING_HEADER);

            olapClientConnectDlgProviderNameHeader = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_PROVIDER_NAME_HEADER);

            olapClientConnectDlgProviderSSAS = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_PROVIDER_SSAS);

            olapClientConnectDlgProviderMondrian = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_PROVIDER_MONDRIAN);

            olapClientConnectDlgProviderActivePivot = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_PROVIDER_ACTIVEPIVOT);

            olapClientConnectDlgProviderSSASToolTip = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_PROVIDER_SSAS_TOOLTIP);

            olapClientConnectDlgProviderMondrianToolTip = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_PROVIDER_MONDRIAN_TOOLTIP);

            olapClientConnectDlgProviderActivePivotToolTip = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_PROVIDER_ACTIVEPIVOT_TOOLTIP);

			olapClientConnectDlgServerName = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_SERVERNAME);

			olapClientConnectDlgDatabaseName = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_DATABASENAME);

			olapClientConnectDlgCredential = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_CREDENTIAL);

			olapClientConnectDlgBrowse = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_BROWSE);

			olapClientConnectDlgOk = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_OK);

			olapClientConnectDlgCancel = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_CANCEL);

            olapClientConnectDlgEnterCredentialHeader = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_ENTERCREDENTIAL_HEADER);

            olapClientConnectDlgUserName = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_USER_NAME);

            olapClientConnectDlgPassword = SR.GetString(ci, OLAPCLIENT_CONNECT_DLG_PASSWORD);

            olapClientConnectOptionsDlgTitle = SR.GetString(ci, OLAPCLIENT_CONNECTOPTIONS_DLG_TITLE);
		} 
		
		#endregion
		
		#region Properties
		
		public string OlapClientConnectDlgHeader
        {
            get { return olapClientConnectDlgHeader; }
            set { olapClientConnectDlgHeader = value; }
        }

		public string OlapClientConnectDlgDataSourceHeader
        {
            get { return olapClientConnectDlgDataSourceHeader; }
            set { olapClientConnectDlgDataSourceHeader = value; }
        }

		public string OlapClientConnectDlgOfflineCubeHeader
        {
            get { return olapClientConnectDlgOfflineCubeHeader; }
            set { olapClientConnectDlgOfflineCubeHeader = value; }
        }

		public string OlapClientConnectDlgServerHeader
        {
            get { return olapClientConnectDlgServerHeader; }
            set { olapClientConnectDlgServerHeader = value; }
        }

		public string OlapClientConnectDlgConnectionStringHeader
        {
            get { return olapClientConnectDlgConnectionStringHeader; }
            set { olapClientConnectDlgConnectionStringHeader = value; }
        }

        public string OlapClientConnectDlgProviderNameHeader
        {
            get { return olapClientConnectDlgProviderNameHeader; }
            set { olapClientConnectDlgProviderNameHeader = value; }
        }

        public string OlapClientConnectDlgProviderSSAS
        {
            get { return olapClientConnectDlgProviderSSAS; }
            set { olapClientConnectDlgProviderSSAS = value; }
        }

        public string OlapClientConnectDlgProviderMondrian
        {
            get { return olapClientConnectDlgProviderMondrian; }
            set { olapClientConnectDlgProviderMondrian = value; }
        }

        public string OlapClientConnectDlgProviderActivePivot
        {
            get { return olapClientConnectDlgProviderActivePivot; }
            set { olapClientConnectDlgProviderActivePivot = value; }
        }

        public string OlapClientConnectDlgProviderSSASToolTip
        {
            get { return olapClientConnectDlgProviderSSASToolTip; }
            set { olapClientConnectDlgProviderSSASToolTip = value; }
        }

        public string OlapClientConnectDlgProviderMondrianToolTip
        {
            get { return olapClientConnectDlgProviderMondrianToolTip; }
            set { olapClientConnectDlgProviderMondrianToolTip = value; }
        }

        public string OlapClientConnectDlgProviderActivePivotToolTip
        {
            get { return olapClientConnectDlgProviderActivePivotToolTip; }
            set { olapClientConnectDlgProviderActivePivotToolTip = value; }
        }


		public string OlapClientConnectDlgServerName
        {
            get { return olapClientConnectDlgServerName; }
            set { olapClientConnectDlgServerName = value; }
        }

		public string OlapClientConnectDlgDatabaseName
        {
            get { return olapClientConnectDlgDatabaseName; }
            set { olapClientConnectDlgDatabaseName = value; }
        }

		public string OlapClientConnectDlgCredential
        {
            get { return olapClientConnectDlgCredential; }
            set { olapClientConnectDlgCredential = value; }
        }

		public string OlapClientConnectDlgBrowse
        {
            get { return olapClientConnectDlgBrowse; }
            set { olapClientConnectDlgBrowse = value; }
        }

		public string OlapClientConnectDlgOk
        {
            get { return olapClientConnectDlgOk; }
            set { olapClientConnectDlgOk = value; }
        }

		public string OlapClientConnectDlgCancel
        {
            get { return olapClientConnectDlgCancel; }
            set { olapClientConnectDlgCancel = value; }
        }

        public string OlapClientConnectDlgEnterCredentialHeader
        {
            get { return olapClientConnectDlgEnterCredentialHeader; }
            set { olapClientConnectDlgEnterCredentialHeader = value; }
        }

        public string OlapClientConnectDlgUserName
        {
            get { return olapClientConnectDlgUserName; }
            set { olapClientConnectDlgUserName = value; }
        }

        public string OlapClientConnectDlgPassword
        {
            get { return olapClientConnectDlgPassword; }
            set { olapClientConnectDlgPassword = value; }
        }

        public string OlapClientConnectOptionsDlgTitle
        {
            get { return olapClientConnectOptionsDlgTitle; }
            set { olapClientConnectOptionsDlgTitle = value; }
        }

		#endregion
	}
}
