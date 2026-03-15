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
    internal sealed class MeasureEditorResourceWrapper
    {
        #region Constants
        
        const string OLAPTOOLS_MEASUREEDITOR_TITLE = "OlapTools_MeasureEditor_Title";

        const string OLAPTOOLS_MEASUREEDITOR_REMOVE_CONTEXTMENU = "OlapTools_MeasureEditor_Remove_ContextMenu";

        const string OLAPTOOLS_EDITOR_OK = "OlapTools_Editor_Ok";

        const string OLAPTOOLS_EDITOR_CANCEL = "OlapTools_Editor_Cancel";

        const string OLAPTOOLS_MEASUREEDITOR_CHECKALL_UNCHECKALL = "OlapTools_MeasureEditor_CheckAll_UnCheckAll";

        const string OLAPTOOLS_EDITOR_DELETE = "OlapTools_Editor_Delete";

        const string OLAPTOOLS_EDITOR_MOVEUP = "OlapTools_Editor_MoveUp";

        const string OLAPTOOLS_EDITOR_MOVEDOWN = "OlapTools_Editor_MoveDown";

        #endregion
        
        #region Members
        
        private string olapToolsMeasureEditorTitle;

        private string olapToolsMeasureEditorRemoveContextMenu;

        private string olapToolsEditorOk;

        private string olapToolsEditorCancel;

        private string olapMeasureEditorCheckOrUncheckAll;

        private string olapToolsEditorDelete;

        private string olapToolsEditorMoveUp;

        private string olapToolsEditorMoveDown;
        
        #endregion
    
        #region Constructor

        public MeasureEditorResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;
            
            olapToolsMeasureEditorTitle = SR.GetString(ci, OLAPTOOLS_MEASUREEDITOR_TITLE);

            olapToolsMeasureEditorRemoveContextMenu = SR.GetString(ci, OLAPTOOLS_MEASUREEDITOR_REMOVE_CONTEXTMENU);

            olapToolsEditorOk = SR.GetString(ci, OLAPTOOLS_EDITOR_OK);

            olapToolsEditorCancel = SR.GetString(ci, OLAPTOOLS_EDITOR_CANCEL);

            olapMeasureEditorCheckOrUncheckAll = SR.GetString(ci, OLAPTOOLS_MEASUREEDITOR_CHECKALL_UNCHECKALL);

            olapToolsEditorDelete = SR.GetString(ci, OLAPTOOLS_EDITOR_DELETE);

            olapToolsEditorMoveDown = SR.GetString(ci, OLAPTOOLS_EDITOR_MOVEDOWN);

            olapToolsEditorMoveUp = SR.GetString(ci, OLAPTOOLS_EDITOR_MOVEUP);
        } 
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets the text for the measure editor title.
        /// </summary>
        /// <value>The olap tools measure editor title.</value>
        public string OlapToolsMeasureEditorTitle
        {
            get { return olapToolsMeasureEditorTitle; }
            set { olapToolsMeasureEditorTitle = value; }
        }

        /// <summary>
        /// Gets or sets the text for the measure editor remove context menu option.
        /// </summary>
        /// <value>The olap tools measure editor remove context menu.</value>
        public string OlapToolsMeasureEditorRemoveContextMenu
        {
            get { return olapToolsMeasureEditorRemoveContextMenu; }
            set { olapToolsMeasureEditorRemoveContextMenu = value; }
        }

        /// <summary>
        /// Gets or sets the text for the tools editor ok option.
        /// </summary>
        /// <value>The olap tools editor ok.</value>
        public string OlapToolsEditorOk
        {
            get { return olapToolsEditorOk; }
            set { olapToolsEditorOk = value; }
        }

        /// <summary>
        /// Gets or sets text for the tools editor cancel option.
        /// </summary>
        /// <value>The olap tools editor cancel.</value>
        public string OlapToolsEditorCancel
        {
            get { return olapToolsEditorCancel; }
            set { olapToolsEditorCancel = value; }
        }

        /// <summary>
        /// Gets or sets text for the measure editor check or uncheck all option.
        /// </summary>
        /// <value>The olap measure editor check or uncheck all.</value>
        public string OlapMeasureEditorCheckOrUncheckAll
        {
            get { return olapMeasureEditorCheckOrUncheckAll; }
            set { olapMeasureEditorCheckOrUncheckAll = value; }
        }

        /// <summary>
        /// Gets or sets the text for tools editor delete option.
        /// </summary>
        /// <value>The olap tools editor delete.</value>
        public string OlapToolsEditorDelete
        {
            get { return olapToolsEditorDelete; }
            set { olapToolsEditorDelete = value; }
        }

        /// <summary>
        /// Gets or sets the text for tools editor move up option.
        /// </summary>
        /// <value>The olap tools editor move up.</value>
        public string OlapToolsEditorMoveUp
        {
            get { return olapToolsEditorMoveUp; }
            set { olapToolsEditorMoveUp = value; }
        }

        /// <summary>
        /// Gets or sets the text for tools editor move down option.
        /// </summary>
        /// <value>The olap tools editor move down.</value>
        public string OlapToolsEditorMoveDown
        {
            get { return olapToolsEditorMoveDown; }
            set { olapToolsEditorMoveDown = value; }
        }

        #endregion
    }
}
