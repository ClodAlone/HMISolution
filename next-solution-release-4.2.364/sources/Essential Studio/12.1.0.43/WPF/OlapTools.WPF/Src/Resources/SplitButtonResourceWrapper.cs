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
	internal sealed class SplitButtonResourceWrapper
	{
		#region Constants
		
		const string OLAPTOOLS_EDITOR_SPLITBUTTON_MOVEUP_TOOLTIP = "OlapTools_Editor_SplitButton_MoveUp_ToolTip";

		const string OLAPTOOLS_EDITOR_SPLITBUTTON_MOVEDOWN_TOOLTIP = "OlapTools_Editor_SplitButton_MoveDown_ToolTip";

		const string OLAPTOOLS_EDITOR_SPLITBUTTON_DELETE_TOOLTIP = "OlapTools_Editor_SplitButton_Delete_ToolTip";


		
		#endregion
		
		#region Members
		
		private string olapToolsEditorSplitButtonMoveUpToolTip;

		private string olapToolsEditorSplitButtonMoveDownToolTip;

		private string olapToolsEditorSplitButtonDeleteToolTip;


		
		#endregion
	
		#region Constructor
		
		public SplitButtonResourceWrapper()
		{
			CultureInfo ci = CultureInfo.CurrentUICulture;
			
			olapToolsEditorSplitButtonMoveUpToolTip = SR.GetString(ci, OLAPTOOLS_EDITOR_SPLITBUTTON_MOVEUP_TOOLTIP);

			olapToolsEditorSplitButtonMoveDownToolTip = SR.GetString(ci, OLAPTOOLS_EDITOR_SPLITBUTTON_MOVEDOWN_TOOLTIP);

			olapToolsEditorSplitButtonDeleteToolTip = SR.GetString(ci, OLAPTOOLS_EDITOR_SPLITBUTTON_DELETE_TOOLTIP);


		} 
		
		#endregion
		
		#region Properties
		
		public string OlapToolsEditorSplitButtonMoveUpToolTip
        {
            get { return olapToolsEditorSplitButtonMoveUpToolTip; }
            set { olapToolsEditorSplitButtonMoveUpToolTip = value; }
        }

		public string OlapToolsEditorSplitButtonMoveDownToolTip
        {
            get { return olapToolsEditorSplitButtonMoveDownToolTip; }
            set { olapToolsEditorSplitButtonMoveDownToolTip = value; }
        }

		public string OlapToolsEditorSplitButtonDeleteToolTip
        {
            get { return olapToolsEditorSplitButtonDeleteToolTip; }
            set { olapToolsEditorSplitButtonDeleteToolTip = value; }
        }


		
		#endregion
	}
}
