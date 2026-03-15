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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Text;
using System.Reflection;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;
using System.Globalization;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
	public class XPTaskBarActionList : SyncActionListBase<XPTaskBar>
	{
		public XPTaskBarActionList (IComponent component)
			: base(component)
		{
		}

		protected override void InitializeActionList()
		{
			this.AddDesignerActionHeaderItem("Essential Tools - XPTaskBar");
			this.AddDesignerActionPropertyItem("Name", "Name", "Misc", "Specifies the name of the control.");
			this.AddDesignerActionMethodItem("AddTaskBox", "Add TaskBarBox", "Misc", "Adds new XPTaskBarBoxes.");

			// Appearance
			this.AddDesignerActionHeaderItem("Appearance");
			this.AddDesignerActionPropertyItem("BackColor", "BackColor", "Appearance", "Specifies the BackColor of XPTaskBar.");
			this.AddDesignerActionPropertyItem("HeaderImageList", "Header ImageList", "Appearance", "Specifies the imageList used for Header.");
			this.AddDesignerActionPropertyItem("ThemesEnabled", "Themes Enabled", "Appearance", "Specifies whether themes should be enabled.");
			this.AddDesignerActionPropertyItem("VerticalLayout", "Vertical Layout", "Appearance", "Specifies the layout of XPTaskBarBoxes.");
			this.AddDesignerActionPropertyItem("AutoPersistStates", "AutoPersistStates", "Appearance", "Specifies whether XPTaskBarBoxes state should be persisted.");
			
			//Layout
			this.AddDesignerActionHeaderItem("Layout");
			this.AddDesignerActionPropertyItem("Dock", "Dock", "Layout", "Defines which borders of the control are to be bound to the container.");
			this.AddDesignerActionPropertyItem("Padding", "Padding", "Layout", "Specifies the spacing between the borders and XPTaskBarBoxes.");
			this.AddDesignerActionPropertyItem("VerticalPadding", "Vertical Padding", "Layout", "Specfies the spacing between XPTaskBarBoxes.");
			this.AddDesignerActionPropertyItem("AutoScroll", "Auto Scroll", "Layout", "Specfies whether AutoScroll should be enabled.");
		}

		public string Name
		{
			get
			{
				string name = string.Empty;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					name = Control.Name;
				}
				return name;
			}
			set
			{
				SetValue("Name", value);
			}
		}

		#region Layout
		public DockStyle Dock
		{
			get
			{
				DockStyle dock = DockStyle.None;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					dock = control.Dock;
				}
				return dock;
			}
			set
			{
				SetValue("Dock", value);
			}
		}

		public Padding Padding
		{
			get
			{
				Padding padding = Padding.Empty;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					padding = control.Padding;
				}
				return padding;
			}
			set
			{
				SetValue("Padding", value);
			}
		}

		public int VerticalPadding
		{
			get
			{
				int verticalPadding = 0;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					verticalPadding = control.VerticalPadding;
				}
				return verticalPadding;
			}
			set
			{
				SetValue("VerticalPadding", value);
			}
		}

		public bool AutoScroll
		{
			get
			{
				bool scroll = false;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					scroll = control.AutoScroll;
				}
				return scroll;
			}
			set
			{
				SetValue("AutoScroll", value);
			}
		}

		#endregion

		#region Appearance
		public Color BackColor
		{
			get
			{
				Color color = Color.Empty;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					color = control.BackColor;
				}
				return color;
			}
			set
			{
				SetValue("BackColor", value);
			}
		}

		public ImageList HeaderImageList
		{
			get
			{
				ImageList imageList = null;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					imageList = control.HeaderImageList;
				}
				return imageList;
			}
			set
			{
				SetValue("HeaderImageList", value);
			}
		}

		public bool ThemesEnabled
		{
			get
			{
				bool themes = false;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					themes = control.ThemesEnabled;
				}
				return themes;
			}
			set
			{
				SetValue("ThemesEnabled", value);
			}
		}

		public bool VerticalLayout
		{
			get
			{
				bool layout = true;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					layout = control.VerticalLayout;
				}
				return layout;
			}
			set
			{
				SetValue("VerticalLayout", value);
			}
		}

		public bool AutoPersistStates
		{
			get
			{
				bool persist = true;
				if (this.Control != null)
				{
					XPTaskBar control = this.Control as XPTaskBar;
					persist = control.AutoPersistStates;
				}
				return persist;
			}
			set
			{
				SetValue("AutoPersistStates", value);
			}
		} 
		#endregion

		#region Verb
		public void AddTaskBox ( )
		{
			XPTaskBar xpTaskBar;
			System.ComponentModel.MemberDescriptor memberDescriptor;
			System.ComponentModel.Design.IDesignerHost iDesignerHost;
			System.ComponentModel.Design.DesignerTransaction designerTransaction;
			System.ComponentModel.Design.CheckoutException checkoutException;
			XPTaskBarBox taskMenuBox;
			string compName;
			System.ComponentModel.PropertyDescriptor propertyDescriptor;

			xpTaskBar = (XPTaskBar)this.Component;
			memberDescriptor = (MemberDescriptor)TypeDescriptor.GetProperties((object)this.Component)[(string)@"Controls"];
			iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			if (iDesignerHost != null)
			{
				designerTransaction = null;
				// Raise the RaiseComponentChanging and RaiseComponentChanged events
				// Also call CreateComponent and parent the tabpage to the tabcontrol
				try
				{
					try
					{
						designerTransaction = iDesignerHost.CreateTransaction(String.Concat((string)@"Add task menu category to ", this.Component.Site.Name));
						//	this.RaiseComponentChanging(memberDescriptor);
					}
					catch (System.ComponentModel.Design.CheckoutException exception)
					{
						checkoutException = exception;
						if (checkoutException == CheckoutException.Canceled)
							throw checkoutException;
					}
					taskMenuBox = (XPTaskBarBox)iDesignerHost.CreateComponent(this.GetChildType());
					compName = null;
					propertyDescriptor = TypeDescriptor.GetProperties((object)taskMenuBox)[(string)@"Name"];
					if (propertyDescriptor != null)
					{
						if (propertyDescriptor.PropertyType == typeof(System.String))
							compName = (string)(System.String)propertyDescriptor.GetValue((object)taskMenuBox);
					}
					if ((compName != null))
						taskMenuBox.Text = compName;

					xpTaskBar.Controls.Add((Control)taskMenuBox);
					//	this.RaiseComponentChanged(memberDescriptor,null,null);

                    if( !xpTaskBar.ThemesEnabled && xpTaskBar.Style == XPTaskBarStyle.Office2007 )
                    {
                        taskMenuBox.ResetPADY();
                    }
				}
				finally
				{
					if (designerTransaction != null)
						designerTransaction.Commit();
				}
			}
		}
		private Type GetChildType ( )
		{
			Type childType = typeof(XPTaskBarBox);
			object[] atts = this.Component.GetType().GetCustomAttributes(typeof(Syncfusion.Windows.Forms.Design.DefaultChildTypeAttribute), true);
			if (atts.Length > 0)
			{
				DefaultChildTypeAttribute attribute = atts[0] as DefaultChildTypeAttribute;

				if (!typeof(XPTaskBarBox).IsAssignableFrom(attribute.ChildType))
					MessageBox.Show("Specified custom type " + attribute.ChildType.ToString() + " is not derived from XPTaskBarBox. Creating a XPTaskBarBox instance.", "XPTaskBar designer warning:");
				else
					childType = attribute.ChildType;
			}
			return childType;
		} 
		#endregion
	}
}
#endif
