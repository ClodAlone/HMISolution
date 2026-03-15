#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Forms
{

	/// <summary>
	///    <para>Represents a standard Windows vertical scroll bar.</para>
	/// </summary>
	/// <remarks>
	///    <para>Most controls that need scroll bars already provide them
	///          and do not require this control. This is true of a multi-line
    ///          <see cref="System.Windows.Forms.TextBox"/> control, a <see cref="System.Windows.Forms.ListBox"/>
    ///          and a <see cref="System.Windows.Forms.ComboBox"/> , for example.</para>
	///    <para>You can use this control to implement scrolling in
	///          containers that do not provide their own scroll bars, such as 
    ///          a <see cref="System.Windows.Forms.PictureBox"/> or for user input
	///          of numeric data. The numeric data may be displayed in a control or utilized in
	///          code. The <see cref="FlatScrollBar.Minimum"/> and <see cref="FlatScrollBar.Maximum"/>
	///          properties determine the range of values the user can select. The <see cref="FlatScrollBar.LargeChange"/> property
	///          determines the effect of clicking within the scroll bar but outside the scroll
	///          box. The <see cref="FlatScrollBar.SmallChange"/> property
	///          determines the effect of clicking the scroll arrows at each end of the control.</para>
	/// </remarks>
	/// <seealso cref="FlatScrollBar"/>
	/// <seealso cref="FlatHScrollBar"/>
	[
	ToolboxItem(false),
	Syncfusion.Documentation.DocumentationExclude()
	]
	public class FlatVScrollBar : FlatScrollBar 
	{

		// Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
		public FlatVScrollBar()  
		{
			this.Size = new Size(SystemInformation.VerticalScrollBarWidth+1,80);
			this.scrollBarType = FlatScrollBarType.Vertical;
		}

		/// <internalonly/>
		/// <summary>
		///    <para>
		///       Returns the parameters needed to create the handler. Inheriting classes
		///       can override this to provide extra functionality. They should not,
		///       however, forget to call base.getCreateParams() first to get the structure
		///       filled up with the basic info.
		///    </para>
		/// </summary>
		/// <seealso cref="System.Windows.Forms.CreateParams"/>
		/// <keyword term=""/>
		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
			get 
			{
				System.Windows.Forms.CreateParams cp;
				cp = base.CreateParams;
				cp.ClassName = null;
				cp.Style = (int) (cp.Style | (int)NativeMethods.WS_VSCROLL);
				return cp;
			}
		}

		/// <override/>
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)  
		{
			int sbWidth = SystemInformation.VerticalScrollBarWidth;
			if (!(XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed) && width <= sbWidth)
				base.SetBoundsCore(x-1, y, sbWidth+1, height, specified);
			else
				base.SetBoundsCore(x, y, width, height, specified);
		}

		/// <internalonly/>
		/// <summary>
		/// </summary>
		[Browsable(false)]
		public override RightToLeft RightToLeft
		{
			get 
			{
				return RightToLeft.No;
			}
			set 
			{
			}
		}
	}

}

