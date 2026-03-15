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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <exclude/>
	public class GroupViewItemConverter : TypeConverter
	{
		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
		{
			if(sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if( (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				&& (value is GroupViewItem) )
			{
				GroupViewItem item = (GroupViewItem)value;

				if (!item.Visible)
				{
					System.Reflection.ConstructorInfo ci = typeof(GroupViewItem).GetConstructor(new Type[] {typeof(string), typeof(int), typeof(bool), typeof(Object), typeof(string), typeof(bool)});
					return new InstanceDescriptor(ci, new object[] { item.Text, item.ImageIndex, item.Enabled, item.Tag, item.ToolTipText, false });
				}
				else if ( item.ToolTipText != null && item.ToolTipText.Length > 0 )
				{
					System.Reflection.ConstructorInfo ci = typeof(GroupViewItem).GetConstructor(new Type[] {typeof(string),typeof(int),typeof(bool),typeof(Object),typeof(string)});
					return new InstanceDescriptor(ci, new object[]{item.Text,item.ImageIndex,item.Enabled,item.Tag,item.ToolTipText});
				}
				else if((item.Tag != null) && ((item.Tag as String) != String.Empty) && (item.Enabled == false))
				{
					System.Reflection.ConstructorInfo ci = typeof(GroupViewItem).GetConstructor(new Type[] {typeof(string),typeof(int),typeof(bool),typeof(Object)});
					return new InstanceDescriptor(ci, new object[]{item.Text,item.ImageIndex,item.Enabled,item.Tag});
				}
				else if((item.Tag != null) && ((item.Tag as String) != String.Empty))
				{
					System.Reflection.ConstructorInfo ci = typeof(GroupViewItem).GetConstructor(new Type[] {typeof(string),typeof(int),typeof(Object)});
					return new InstanceDescriptor(ci, new object[] { item.Text, item.ImageIndex, item.Tag });
				}
				else if(item.Enabled == false)
				{
					System.Reflection.ConstructorInfo ci = typeof(GroupViewItem).GetConstructor(new Type[] {typeof(string),typeof(int),typeof(bool)});
					return new InstanceDescriptor(ci, new object[] { item.Text, item.ImageIndex, item.Enabled });
				}
				else 
				{
					System.Reflection.ConstructorInfo ci = typeof(GroupViewItem).GetConstructor(new Type[] {typeof(string),typeof(int)});
					return new InstanceDescriptor(ci, new object[] { item.Text, item.ImageIndex });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
		{
			if(destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;

			return base.CanConvertTo(context, destinationType);
		}
	}

	public class GroupViewDesigner : ControlDesigner
	{
		public GroupViewDesigner()
		{
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			iccs.ComponentRemoved += new ComponentEventHandler(this.IComponentChangeService_ComponentRemoved);
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		DesignerActionListCollection actionLists;

		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if (null == actionLists)
				{
					actionLists = new DesignerActionListCollection();
					actionLists.Add(
						new GroupViewActionList(this.Component));
				}
				return actionLists;
			}
		}
#endif

		public void IComponentChangeService_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			if(e.Component.Equals(this.Component))
			{
				GroupView ctrl = this.Component as GroupView;
				if(ctrl.GroupViewItems.Count > 0)
				{
					GroupViewItem[] items = new GroupViewItem[ctrl.GroupViewItems.Count];
					ctrl.GroupViewItems.CopyTo(items, 0);
					foreach(GroupViewItem item in items)
					{
						ctrl.GroupViewItems.Remove(item);
						IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
					}
					ctrl.GroupViewItems.Clear();
				}
			}
		}

		protected override void OnMouseDragBegin(int x, int y)
		{
			base.OnMouseDragBegin(x,y);
			GroupView ctrl = this.Component as GroupView;
			(ctrl as IGroupViewDesignerInvoke).HandleMouseDown(MouseButtons.Left, ctrl.PointToClient(new Point(x,y)));
		}

		protected override void OnMouseDragEnd(bool cancel)
		{
			base.OnMouseDragEnd(cancel);
			GroupView ctrl = this.Component as GroupView;
			(ctrl as IGroupViewDesignerInvoke).HandleMouseUp(MouseButtons.Left, ctrl.PointToClient(Cursor.Position));
		}

		protected override void Dispose(bool bdispose)
		{
			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(iccs != null)
				iccs.ComponentRemoved -= new ComponentEventHandler(this.IComponentChangeService_ComponentRemoved);

			base.Dispose(bdispose);
		}
	}

    public class GroupBarDesigner : ParentControlDesigner
    {
		protected DesignerVerb dvAddGroup = null;
		protected DesignerVerb dvRemoveGroup = null;
		protected DesignerVerbCollection dvcVerbs = null;
		protected bool bLoadComplete = false;

		protected override bool DrawGrid
	    {
		    get { return false; }
	    }

		public override DesignerVerbCollection Verbs
		{
			get { return this.dvcVerbs; }
		}


#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		DesignerActionListCollection actionLists;

		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if (null == actionLists)
				{
					actionLists = new DesignerActionListCollection();
					actionLists.Add(
						new GroupBarActionList(this.Component));
				}
				return actionLists;
			}
		}
#endif

		public GroupBarDesigner()
		{
			this.dvAddGroup = new DesignerVerb("Add Group", new EventHandler(OnVerbAddGroup));
			this.dvRemoveGroup = new DesignerVerb("Remove Group", new EventHandler(OnVerbRemoveGroup));
			DesignerVerb[] dvarray = new DesignerVerb[] { this.dvAddGroup, this.dvRemoveGroup };
			this.dvcVerbs = new DesignerVerbCollection(dvarray);
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			idh.LoadComplete += new EventHandler(this.IDesignerHost_LoadComplete);
			IComponentChangeService iccs = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			iccs.ComponentRemoved += new ComponentEventHandler(this.IComponentChangeService_ComponentRemoved);
			iccs.ComponentChanged += new ComponentChangedEventHandler(this.IComponentChangeService_ComponentChanged);
			ISelectionService iss = (ISelectionService)this.GetService(typeof(ISelectionService));
			iss.SelectionChanged += new EventHandler(this.ISelectionService_SelectionChanged);
		}

		protected override void PostFilterProperties(IDictionary properties)
		{
			if(this.bLoadComplete == true)
			{
				GroupBar groupBarCtrl = this.Component as GroupBar;
				if((groupBarCtrl != null) && (groupBarCtrl.StackedMode == false))
				{
					if(properties.Contains("HeaderHeight"))
						properties.Remove("HeaderHeight");

					if(properties.Contains("HeaderFont"))
						properties.Remove("HeaderFont");

					if(properties.Contains("HeaderForeColor"))
						properties.Remove("HeaderForeColor");

					if(properties.Contains("HeaderBackColor"))
						properties.Remove("HeaderBackColor");

					if(properties.Contains("NavigationPaneButtonWidth"))
						properties.Remove("NavigationPaneButtonWidth");

					if(properties.Contains("NavigationPaneHeight"))
						properties.Remove("NavigationPaneHeight");
				}
			}
			base.PostFilterProperties(properties);
		}

		protected override void OnMouseDragBegin(int x, int y)
		{
			base.OnMouseDragBegin(x,y);
			GroupBar ctrl = this.Component as GroupBar;
			(ctrl as IIntegratedScrollContainer).HandleScrollButtonDown(ctrl.PointToClient(new Point(x,y)));
		}

		protected override void OnMouseDragEnd(bool cancel)
		{
			base.OnMouseDragEnd(cancel);
			GroupBar ctrl = this.Component as GroupBar;
			(ctrl as IIntegratedScrollContainer).HandleScrollButtonUp(ctrl.PointToClient(Cursor.Position));
		}

		protected override void OnDragDrop(DragEventArgs de)
		{
			base.OnDragDrop(de);

			GroupBar gbar = this.Control as GroupBar;
			if((gbar.SelectedItem != -1) && (gbar.SelectedItem < gbar.GroupBarItems.Count))
			{
				gbar.SelectedItem = gbar.SelectedItem; // Reset selection;

				ISelectionService selsvc = (ISelectionService)this.GetService(typeof(ISelectionService));
				Object[] objarray = new Object[1];
				objarray[0] = gbar;
				selsvc.SetSelectedComponents(objarray);
				objarray[0] = gbar.GroupBarItems[gbar.SelectedItem].Client;
				selsvc.SetSelectedComponents(objarray);
			}
		}

		protected override void WndProc(ref Message m)
		{
			bool bselected = false;
			if(m.Msg == 0x0201 /*WM_LBUTTONDOWN*/)
			{
				// Is the GroupBar selected?
				if(((ISelectionService)this.GetService(typeof(ISelectionService))).PrimarySelection is GroupBar)
					bselected = true;
			}

			base.WndProc(ref m);

			if(m.Msg == 0x0201 /*WM_LBUTTONDOWN*/ && bselected)
			{
				GroupBar grpbar = this.Control as GroupBar;
				int nindex = (grpbar as IGroupBarDesignerInvoke).GetGroupAtLocation(  new Point((Int32)m.LParam) );
				if((nindex >= 0) && (nindex != grpbar.SelectedItem))
				{
					int prevselection = grpbar.SelectedItem;
					grpbar.SelectedItem = nindex;
					this.RaiseComponentChanged(TypeDescriptor.GetProperties(grpbar)["SelectedIndex"], prevselection, nindex);
				}
			}
		}

		protected void IDesignerHost_LoadComplete(object sender, EventArgs e)
		{
			this.bLoadComplete = true;
			GroupBar ctrl = this.Component as GroupBar;
			if(ctrl.GroupBarItems.Count == 0)
				this.dvRemoveGroup.Enabled = false;
			TypeDescriptor.Refresh(ctrl);
		}

		protected void IComponentChangeService_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			if((e.Component is Control) == false)
				return;

			GroupBar ctrl = this.Component as GroupBar;
			if(e.Component.Equals(ctrl))
			{
				if(ctrl.GroupBarItems.Count > 0)
				{
					GroupBarItem[] items = new GroupBarItem[ctrl.GroupBarItems.Count];
					ctrl.GroupBarItems.CopyTo(items, 0);
					ctrl.GroupBarItems.Clear();
					IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
					foreach(GroupBarItem item in items)
						idh.DestroyComponent(item);
				}
			}
			else
			{
				GroupBar.GroupBarItemCollection cllnold = ctrl.GroupBarItems;
				foreach(GroupBarItem item in cllnold)
				{
					if((item.Client != null) && (item.Client.Equals(e.Component)))
					{
						item.Client = null;
						break;
					}
				}
				// Raise the component changed event, so that the collection can be repersisted.
				this.RaiseComponentChanged(TypeDescriptor.GetProperties(ctrl)["GroupBarItems"], cllnold, ctrl.GroupBarItems);
			}
		}

		protected void IComponentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			if(e.Component != this.Component)
				return;

			DesignerVerbCollection dvc = this.Verbs;
			if(dvc != null && dvc.Count > 0)
			{
				if(((GroupBar)this.Control).GroupBarItems.Count > 0)
					this.dvRemoveGroup.Enabled = true;
				else
					this.dvRemoveGroup.Enabled = false;
			}
		}

		protected void ISelectionService_SelectionChanged(object sender, EventArgs e)
		{
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if((iss != null) && (iss.PrimarySelection == this.Component))
			{
				GroupBar gbarctrl = this.Component as GroupBar;
				// Check to make sure that the GroupBar's StackedMode related properties are not in display if
				// it StackedMode property is set to false.
				if(gbarctrl.StackedMode == false)
				{
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(gbarctrl);
					foreach(PropertyDescriptor property in properties)
					{
						if(property.Category == "Stacked Mode")
						{
							TypeDescriptor.Refresh(gbarctrl);
							break;
						}
					}
				}
			}
		}

		public void OnVerbAddGroup(object sender, EventArgs e)
		{
			GroupBar ctrl = this.Component as GroupBar;
			IDesignerHost idh = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			GroupBarItem item = (GroupBarItem)idh.CreateComponent(typeof(GroupBarItem));
			ctrl.GroupBarItems.Add(item);
			this.RaiseComponentChanged(TypeDescriptor.GetProperties(ctrl)["GroupBarItems"], null, ctrl.GroupBarItems);
		}

		public void OnVerbRemoveGroup(object sender, EventArgs e)
		{
			GroupBar ctrl = this.Component as GroupBar;
			if(ctrl.GroupBarItems.Count > 0 && ctrl.SelectedItem >= 0)
			{
				IDesignerHost idh = (IDesignerHost)this.GetService(typeof(IDesignerHost));
				GroupBarItem item = ctrl.GroupBarItems[ctrl.SelectedItem];
				if(item.Client != null)
				{
					Control client = item.Client;
					item.Client = null;
					client.Hide();
					idh.DestroyComponent(client);
				}
				ctrl.GroupBarItems.Remove(item);

				idh.DestroyComponent(item);
				this.RaiseComponentChanged(TypeDescriptor.GetProperties(ctrl)["BarObjects"], null, ctrl.GroupBarItems);
			}
		}

		protected override void Dispose(bool bdispose)
		{
			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(idh != null)
				idh.LoadComplete -= new EventHandler(this.IDesignerHost_LoadComplete);
			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(iccs != null)
			{
				iccs.ComponentChanged -= new ComponentChangedEventHandler(this.IComponentChangeService_ComponentChanged);
				iccs.ComponentRemoved -= new ComponentEventHandler(this.IComponentChangeService_ComponentRemoved);
			}
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if(iss != null)
				iss.SelectionChanged -= new EventHandler(this.ISelectionService_SelectionChanged);

			base.Dispose(bdispose);
		}
    }

	public class BorderColorsConverter : TypeConverter
	{
		public BorderColorsConverter()
		{
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc =
				TypeDescriptor.GetProperties(typeof(Syncfusion.Windows.Forms.Tools.BorderColors), attributes);
			string[] names = new string[4];
			names[0] = "Left";
			names[1] = "Top";
			names[2] = "Right";
			names[3] = "Bottom";
			return pdc.Sort(names);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return new Syncfusion.Windows.Forms.Tools.BorderColors( ((Color)(propertyValues["Left"])), ((Color)(propertyValues["Top"])),
				((Color)(propertyValues["Right"])), ((Color)(propertyValues["Bottom"])) );
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType == null)
				throw new ArgumentNullException("destinationType");

			if((destinationType == typeof(string)) && (value is Syncfusion.Windows.Forms.Tools.BorderColors))
			{
				if(culture == null)
					culture = CultureInfo.CurrentCulture;
				TypeConverter typeConverter2 = TypeDescriptor.GetConverter(typeof(Color));
				string[] strcolors = new string[4];
				Syncfusion.Windows.Forms.Tools.BorderColors bc = (Syncfusion.Windows.Forms.Tools.BorderColors)(value);
				strcolors[0] = typeConverter2.ConvertToString(context, culture, bc.Left);
				strcolors[1] = typeConverter2.ConvertToString(context, culture, bc.Top);
				strcolors[2] = typeConverter2.ConvertToString(context, culture, bc.Right);
				strcolors[3] = typeConverter2.ConvertToString(context, culture, bc.Bottom);
				return string.Join(string.Concat(culture.TextInfo.ListSeparator, " "), strcolors);
			}

			if( (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				&& (value is Syncfusion.Windows.Forms.Tools.BorderColors) )
			{
				Type[] types = new Type[4];
				types[0] = typeof(Color);
				types[1] = typeof(Color);
				types[2] = typeof(Color);
				types[3] = typeof(Color);
				ConstructorInfo cinfo = typeof(Syncfusion.Windows.Forms.Tools.BorderColors).GetConstructor(types);
				if (cinfo != null)
				{
					object[] objcolors = new object[4];
					Syncfusion.Windows.Forms.Tools.BorderColors bc = ((Syncfusion.Windows.Forms.Tools.BorderColors)(value));
					objcolors[0] = bc.Left;
					objcolors[1] = bc.Top;
					objcolors[2] = bc.Right;
					objcolors[3] = bc.Bottom;
					return new InstanceDescriptor(cinfo, ((ICollection)(objcolors)));
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if(value is System.String)
			{
				string strobject = ((string)value).Trim();
				if(strobject.Length == 0)
					return null;
				if(culture == null)
					culture = CultureInfo.CurrentCulture;
				string[] strcolors = strobject.Split(culture.TextInfo.ListSeparator[0]);
				Color[] colors = new Color[(uint)(strcolors.Length)];
				TypeConverter typeConverter4 = TypeDescriptor.GetConverter(typeof(Color));
				for(int i=0; i<colors.Length; i++)
					colors[i] = (Color)(typeConverter4.ConvertFromString(context, culture, strcolors[i]));
				if (colors.Length != 4)
					throw new ArgumentException("TextParseFailedFormat");
				return new Syncfusion.Windows.Forms.Tools.BorderColors(colors[0], colors[1], colors[2], colors[3]);
			}
			return base.ConvertFrom(context, culture, value);
		}


		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if(destinationType != typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return base.CanConvertTo(context, destinationType);
			return true;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType != typeof(string))
				return base.CanConvertFrom(context, sourceType);
			return true;
		}
	}
}


