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
using System.Collections;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	class TabPageAdvDesigner : 
		ScrollableControlDesigner
	{
        
		public TabPageAdvDesigner()
		{
		}
        
		public override /*ControlDesigner*/ SelectionRules SelectionRules 
		{ 
			get
			{
				System.Windows.Forms.Design.SelectionRules selectionRules;
				System.Windows.Forms.Control control;
				selectionRules = base.SelectionRules;
				control = this.Control;
				if (control.Parent is TabControlAdv)
					selectionRules = (SelectionRules)(selectionRules & ~(SelectionRules.AllSizeable));

				return selectionRules;
			}
		}
        
		// Methods
        
		public override /*ControlDesigner*/ bool CanBeParentedTo(IDesigner parentDesigner)
		{
			return (parentDesigner is TabControlAdvDesigner);
		}

		protected override /*ParentControlDesigner*/ void OnPaintAdornments(PaintEventArgs pe)
		{
			System.Windows.Forms.Panel panel;
			panel = (System.Windows.Forms.Panel)this.Component;
			if (panel.BorderStyle == BorderStyle.None)
				DrawingUtils.DrawDesignTimeBorder(pe.Graphics, panel);
			
            base.OnPaintAdornments(pe);
		}
	}
}
