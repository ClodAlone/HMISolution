#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.ComponentModel;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Creates a custom ComboBox class that does not display the drop down
	/// in any mode.
	/// </summary>
	[ ToolboxItem( false ) ]
	public class ComboBoxHideDropDown : ComboBox
	{
		/// <summary>
		/// Override WndProc.
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc( ref Message m )
		{
			if( ( m.Msg == 8465 ) ) //0x2111
			{
				int wparam = ( ( int )( m.WParam ) ) >> 16; //0x10
				
				if( wparam != 7 )
				{
					base.WndProc( ref m );
				}
				else
				{
					this.OnDropDown( EventArgs.Empty );
				}
			}
			else
			{
				base.WndProc( ref m );
			}
		}
	}

	/// <summary>
	/// Old code. Have to be removed in near future.
	/// </summary>
	[ 
	ToolboxItem( false ), 
	Obsolete( "Use the ComboBoxHideDropDown class instead." ) 
	]
	public class AutoCompleteCombo : ComboBoxHideDropDown
	{
	}
}