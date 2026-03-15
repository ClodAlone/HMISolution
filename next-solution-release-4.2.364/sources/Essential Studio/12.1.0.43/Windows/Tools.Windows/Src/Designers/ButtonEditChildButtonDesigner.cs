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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// Extends the design time capabilities of <see cref="ButtonEditChildButton"/>.
	/// </summary>
	public class ButtonEditChildButtonDesigner : ControlDesigner
	{
		/// <summary>
		/// Initializes a new instance of the ButtonEditChildButtonDesigner class.
		/// </summary>
		public ButtonEditChildButtonDesigner()
		{

		}

		public override SelectionRules SelectionRules
		{
			get
			{
				return SelectionRules.None;
			}
		}
        
		/// <summary>
		/// Adjusts the set of properties the component exposes through a TypeDescriptor.
		/// </summary>
		/// <param name="properties">An IDictionary that contains the properties for the class of the component.</param>
		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);
		
			String[] strcolln = new String[18];
			strcolln[0] = "RightToLeft";
			strcolln[1] = "ContextMenu";
			strcolln[2] = "ImeMode";
			strcolln[3] = "Dock";
			strcolln[4] = "DockPadding";
			strcolln[5] = "Anchor";
			strcolln[6] = "AutoScroll";
			strcolln[7] = "CausesValidation";
			strcolln[8] = "AllowDrop";
			strcolln[9] = "BackgroundImage";
			strcolln[10] = "Cursor";
			strcolln[11] = "DialogResult";
			strcolln[12] = "Location";
			strcolln[13] = "Size";
			strcolln[14] = "AccessibleDescription";
			strcolln[15] = "AccessibleName";
			strcolln[16] = "AccessibleRole";
			strcolln[17] = "DataBindings";

			RemovePropertyBrowsable(this.Control, strcolln, properties);			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <param name="strcolln"></param>
		/// <param name="properties"></param>
		static private void RemovePropertyBrowsable(Control control, String[] strcolln, IDictionary properties)
		{
			foreach(String property in strcolln)
			{		
				PropertyDescriptor prop = (PropertyDescriptor)properties[property];			
				if( (prop != null) && (prop.IsBrowsable == true) )
				{
					AttributeCollection mac = prop.Attributes;      				
					bool bnondef = false;
					foreach(Attribute mematt in mac)
					{						
						// Is Browsable a default attribute? If so, break.
						if(mematt as BrowsableAttribute != null)							
						{
							bnondef = true;
							break;
						}							
					}					
					int ncount = (bnondef == true) ? mac.Count : mac.Count + 1;
					Attribute[] arrmematt = new Attribute[ncount];				
					mac.CopyTo(arrmematt, 0);
					if(bnondef == true)
						arrmematt[Array.IndexOf(arrmematt, BrowsableAttribute.Yes)] = BrowsableAttribute.No;
					else				
						arrmematt[ncount-1] = BrowsableAttribute.No;				
					properties[property] = TypeDescriptor.CreateProperty(control.GetType(), prop, arrmematt);
				}
			}
		}
    }
}

