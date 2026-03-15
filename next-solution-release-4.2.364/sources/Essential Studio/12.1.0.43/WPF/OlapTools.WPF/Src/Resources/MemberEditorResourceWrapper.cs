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

namespace Syncfusion.Windows.Tools.Olap.Resources
{
    internal sealed class MemberEditorResourceWrapper
    {
        #region Constants
        
        const string OLAPTOOLS_MEMBEREDITOR_TITLE = "OlapTools_MemberEditor_Title";

        const string OLAPTOOLS_MEMBEREDITOR_CHECKALL_UNCHECKALL = "OlapTools_MemberEditor_CheckAll_UnCheckAll";

        const string OLAPTOOLS_EDITOR_OK = "OlapTools_Editor_Ok";

        const string OLAPTOOLS_EDITOR_CANCEL = "OlapTools_Editor_Cancel";
        
        #endregion
        
        #region Members
        
        private string olapToolsMemberEditorTitle;

        private string olapToolsMemberEditorCheckAllUnCheckAll;

        private string olapToolsEditorOk;

        private string olapToolsEditorCancel;
        
        #endregion
    
        #region Constructor

        public MemberEditorResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;
            
            olapToolsMemberEditorTitle = SR.GetString(ci, OLAPTOOLS_MEMBEREDITOR_TITLE);

            olapToolsMemberEditorCheckAllUnCheckAll = SR.GetString(ci, OLAPTOOLS_MEMBEREDITOR_CHECKALL_UNCHECKALL);

            olapToolsEditorOk = SR.GetString(ci, OLAPTOOLS_EDITOR_OK);

            olapToolsEditorCancel = SR.GetString(ci, OLAPTOOLS_EDITOR_CANCEL);
        } 
        
        #endregion
        
        #region Properties
        
        public string OlapToolsMemberEditorTitle
        {
            get { return olapToolsMemberEditorTitle; }
            set { olapToolsMemberEditorTitle = value; }
        }

        public string OlapToolsMemberEditorCheckAllUnCheckAll
        {
            get { return olapToolsMemberEditorCheckAllUnCheckAll; }
            set { olapToolsMemberEditorCheckAllUnCheckAll = value; }
        }

        public string OlapToolsEditorOk
        {
            get { return olapToolsEditorOk; }
            set { olapToolsEditorOk = value; }
        }

        public string OlapToolsEditorCancel
        {
            get { return olapToolsEditorCancel; }
            set { olapToolsEditorCancel = value; }
        }
        
        #endregion
    }
}
