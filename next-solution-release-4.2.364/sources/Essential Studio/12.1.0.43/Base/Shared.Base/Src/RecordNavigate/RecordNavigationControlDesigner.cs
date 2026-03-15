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

using System.Diagnostics;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Reflection;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// A designer for the <see cref="RecordNavigationControl"/>.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	internal class RecordNavigationControlDesigner : ParentControlDesigner
	{
		// Constructors
		/// <summary>
		/// Initializes a new <see cref="RecordNavigationControlDesigner"/>.
		/// </summary>
		public RecordNavigationControlDesigner()  
		{ 
		}
 
		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				RecordNavigationControl tc = this.Component as RecordNavigationControl;
				if (tc != null)
				{
					tc.Click -= new EventHandler(ControlClick);
				}
			}
			base.Dispose(disposing);
		}

		/// <override/>
		public override void Initialize(IComponent component)  
		{ 
			base.Initialize(component);
                
			RecordNavigationControl tc = component as RecordNavigationControl;
			if (tc != null)
			{
				tc.Click += new EventHandler(ControlClick);
			}
		} 

		void ControlClick(object sender, EventArgs e)
		{
			ISelectionService ss = (ISelectionService) this.GetService(typeof(ISelectionService));
			if (ss != null)
				ss.SetSelectedComponents(new object[] { sender } );
		}
	}
}
