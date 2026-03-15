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
    internal sealed class CalcMemberEditorResourceWrapper
    {
        #region Constants
        
        const string OLAPTOOLS_CALCMEMBEREDITOR_TITLE = "OlapTools_CalcMemberEditor_Title";

        const string OLAPTOOLS_EDITOR_OK = "OlapTools_Editor_Ok";

        const string OLAPTOOLS_EDITOR_CANCEL = "OlapTools_Editor_Cancel";

        const string OLAPTOOLS_CALCMEMBEREDITOR_CAPTION = "OlapTools_CalcMemberEditor_Caption";

        const string CUBEDIMENSIONBROWSER_RIGHTCLICK_REMOVE = "CubeDimensionBrowser_RightClick_Remove";

        const string CUBEDIMENSIONBROWSER_RIGHTCLICK_EDIT = "CubeDimensionBrowser_RightClick_Edit";

        const string OLAPTOOLS_CALCMEMBEREDITOR_EXPRESSION = "OlapTools_CalcMemberEditor_Expression";

        const string OLAPTOOLS_CALCMEMBEREDITOR_TYPE = "OlapTools_CalcMemberEditor_Type";

        const string OLAPTOOLS_CALCMEMBEREDITOR_FORMAT = "OlapTools_CalcMemberEditor_Format";        
        
        #endregion
        
        #region Members
        
        private string olapToolsCalcMemberEditorTitle;

        private string olapToolsEditorOk;

        private string olapToolsEditorCancel;

        private string _calcMemberEditorCaptionText;

        private string _virtualKpiRemove;

        private string _virtualKpiEdit;

        private string _calcMemberEditorExpression;

        private string _calcMemberEditorMemberType;
        
        private string _calcMemberEditorFormatText;
        
        #endregion
    
        #region Constructor

        public CalcMemberEditorResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            olapToolsCalcMemberEditorTitle = SR.GetString(ci, OLAPTOOLS_CALCMEMBEREDITOR_TITLE);

            olapToolsEditorOk = SR.GetString(ci, OLAPTOOLS_EDITOR_OK);

            olapToolsEditorCancel = SR.GetString(ci, OLAPTOOLS_EDITOR_CANCEL);

            _calcMemberEditorCaptionText = SR.GetString(ci, OLAPTOOLS_CALCMEMBEREDITOR_CAPTION);

            _virtualKpiRemove = SR.GetString(ci, CUBEDIMENSIONBROWSER_RIGHTCLICK_REMOVE);

            _virtualKpiEdit = SR.GetString(ci, CUBEDIMENSIONBROWSER_RIGHTCLICK_EDIT);

            _calcMemberEditorExpression = SR.GetString(ci, OLAPTOOLS_CALCMEMBEREDITOR_EXPRESSION);

            _calcMemberEditorMemberType = SR.GetString(ci, OLAPTOOLS_CALCMEMBEREDITOR_TYPE);

            _calcMemberEditorFormatText = SR.GetString(ci, OLAPTOOLS_CALCMEMBEREDITOR_FORMAT);
        } 
        
        #endregion
        
        #region Properties
        
        public string OlapToolsCalcMemberEditorTitle
        {
            get { return olapToolsCalcMemberEditorTitle; }
            set { olapToolsCalcMemberEditorTitle = value; }
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

        public string CalcMemberEditorCaptionText
        {
            get { return _calcMemberEditorCaptionText; }
            set { _calcMemberEditorCaptionText = value; }
        }

        public string VirtualKpiRemove
        {
            get { return _virtualKpiRemove; }
            set { _virtualKpiRemove = value; }
        }

        public string CalcMemberEditorExpression
        {
            get { return _calcMemberEditorExpression; }
            set { _calcMemberEditorExpression = value; }
        }

        public string CalcMemberEditorType
        {
            get { return _calcMemberEditorMemberType; }
            set { _calcMemberEditorMemberType = value; }
        }

        public string CalcMemberEditorFormatText
        {
            get { return _calcMemberEditorFormatText; }
            set { _calcMemberEditorFormatText = value; }
        }

        #endregion
    }
}
