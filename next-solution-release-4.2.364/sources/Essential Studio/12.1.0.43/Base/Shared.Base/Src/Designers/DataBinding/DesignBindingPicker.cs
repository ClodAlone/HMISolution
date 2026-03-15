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
using System.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Design
{
	[
	ToolboxItem(false), 
	DesignTimeVisible(false)
	]
	class DesignBindingPicker : 
		TreeView
	{
        
		// Fields
		private IWindowsFormsEditorService edSvc;
		private bool multipleDataSources;
		private bool selectLists;
		private bool expansionSignClicked;
		private bool allowSelection;
		private DesignBinding selectedItem;
		private TreeNode selectedNode;
		private static readonly int BINDER_IMAGE;
		private static readonly int COLUMN_IMAGE;
		private static readonly int NONE_IMAGE;
		private const int MaximumDepth = 10 /*0x000A*/;
        
		// Constructors
		public DesignBindingPicker(ITypeDescriptorContext context, bool multipleDataSources, bool selectLists)
		{
			Image image0;
			ImageList imageList1;
			this.expansionSignClicked = false;
			this.allowSelection = false;
			this.selectedItem = null;
			this.selectedNode = null;
			this.multipleDataSources = multipleDataSources;
			this.selectLists = selectLists;
			image0 = ((Image)(new Bitmap(typeof(System.Windows.Forms.Design.ControlDesigner), "DataPickerImages.bmp")));
			imageList1 = ((ImageList)(new ImageList()));
			imageList1.TransparentColor = Color.Lime;
			imageList1.Images.AddStrip(image0);
			this.ImageList = imageList1;
		}
        
		static DesignBindingPicker()
		{
			DesignBindingPicker.BINDER_IMAGE = 0;
			DesignBindingPicker.COLUMN_IMAGE = 1;
			DesignBindingPicker.NONE_IMAGE = 2;
		}
        
        
		// Methods
        
		protected override /*TreeView*/ void OnAfterExpand(TreeViewEventArgs e)
		{
			base.OnAfterExpand(e);
			this.ExpansionSignClicked = false;
		}
        
        
		protected override /*TreeView*/ void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			this.ExpansionSignClicked = true;
			base.OnBeforeExpand(e);
		}
        
        
		protected override /*TreeView*/ void OnAfterCollapse(TreeViewEventArgs e)
		{
			base.OnAfterCollapse(e);
			this.ExpansionSignClicked = false;
		}
        
        
		protected override /*TreeView*/ void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			this.ExpansionSignClicked = true;
			base.OnBeforeCollapse(e);
		}
        
        
		protected override /*TreeView*/ void WndProc(ref Message m)
		{
			if (m.Msg == 513/*WM_LBUTTONDOWN*/) 
			{
				base.WndProc(ref m);
                if (!(this.allowSelection))
                    return;

				this.SetSelectedItem(this.GetNodeAtXAndY(((short)(((int)(m.LParam)))), ((int)(m.LParam)) >> 16/*0x10*/));
                if (this.selectedItem != null && !(this.ExpansionSignClicked))
					this.edSvc.CloseDropDown();
				this.ExpansionSignClicked = false;
			}
			base.WndProc(ref m);
		}
        
        
		protected override /*TreeView*/ void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.KeyData == Keys.Return) 
			{
				this.SetSelectedItem(this.SelectedNode);
				if (this.selectedItem != null)
					this.edSvc.CloseDropDown();
			}
		}
        
        
		protected override /*TreeView*/ bool IsInputKey(Keys key)
		{
			if (key == Keys.Return)
				return true;
			return base.IsInputKey(key);
		}
        
        
		public DesignBinding SelectedItem 
		{ 
			get
			{
				return this.selectedItem;
			}
		}
        
		private bool ExpansionSignClicked 
		{ 
			get
			{
				return this.expansionSignClicked;
			}
			set
			{
				this.expansionSignClicked = value;
			}
		}

		
		private TreeNode GetNodeAtXAndY(int x, int y) 
		{
			NativeMethods.TV_HITTESTINFO lparam;
			IntPtr handle;

			lparam = new NativeMethods.TV_HITTESTINFO();
			lparam.pt_x = x;
			lparam.pt_y = y;
			handle = NativeMethods.SendMessage(this.Handle, 4369/*0x1111*/, 0, lparam);

			if (handle == IntPtr.Zero)
				return null;

			if (lparam.flags == 4
				|| lparam.flags != 2)
				return this.GetNodeAt(x, y);

			return null;
		}
        
		public bool AllowSelection 
		{ 
			get
			{
				return this.allowSelection;
			}
			set
			{
				this.allowSelection = value;
			}
		}

		public void Start(ITypeDescriptorContext context, IWindowsFormsEditorService edSvc, object dataSource, DesignBinding selectedItem) 
		{
			this.edSvc = edSvc;
			this.selectedItem = selectedItem;
			this.ExpansionSignClicked = false;
			if (context == null || context.Container == null)
				return;
			this.FillDataSources(context, dataSource);
			this.Width = this.GetMaxItemWidth(this.Nodes) + SystemInformation.VerticalScrollBarWidth * 2;
		}
        
        
		public void End()
		{
			this.Nodes.Clear();
			this.edSvc = null;
			this.selectedItem = null;
			this.ExpansionSignClicked = false;
		}
        
		protected void FillDataSource(BindingContext bindingManager, object component) 
		{
			CurrencyManager currencyManager;
			PropertyDescriptorCollection pdc;
			TreeNodeCollection nodes;
			TreeNode selectedNode;
			int index;

			if (component as IListSource == null && component as IList == null)
				if (component as Array == null)
					return;

			currencyManager = (CurrencyManager) bindingManager[component];
			pdc = currencyManager.GetItemProperties();
			if (pdc.Count > 0) 
			{
				nodes = this.Nodes;
				if (this.multipleDataSources) 
				{
					selectedNode = new DataSourceNode((IComponent) component);
					this.Nodes.Add(selectedNode);
					if (this.selectedItem != null && this.selectedItem.Equals(component, ""))
						this.selectedNode = selectedNode;
					nodes = selectedNode.Nodes;
				}
				index = 0;
				while (index < pdc.Count) 
				{
					this.FillDataMembers(bindingManager, component, pdc[index].Name, pdc[index].Name, typeof(IList).IsAssignableFrom(pdc[index].PropertyType), nodes, 0);
					index++;
				}
			}
		}
		protected void FillDataSources(ITypeDescriptorContext context, object dataSource) 
		{
			BindingContext bindingManager;
			ComponentCollection components;
			IComponent current;
			TreeNode selectedNode;
			IEnumerator iEnum;
			IDisposable iDisposable;

			this.Nodes.Clear();
			bindingManager = new BindingContext();
			if (this.multipleDataSources) 
			{
				components = context.Container.Components;
				iEnum = components.GetEnumerator();
				try 
				{
					while (iEnum.MoveNext()) 
					{
						current = (IComponent) iEnum.Current;
						this.FillDataSource(bindingManager, current);
					}
				}
				finally 
				{
					iDisposable = iEnum as IDisposable;
					if (iDisposable != null)
						iDisposable.Dispose();
				}
			}
			this.FillDataSource(bindingManager, dataSource);
			selectedNode = new NoneNode();
			this.Nodes.Add(selectedNode);
			if (this.selectedNode == null)
				this.selectedNode = selectedNode;
			this.SelectedNode = this.selectedNode;
			this.selectedNode = null;
			this.selectedItem = null;
			this.allowSelection = true;
		}
         
		protected void FillDataMembers(BindingContext bindingManager, object dataSource, string dataMember, string propertyName, bool isList, TreeNodeCollection nodes, int depth) 
		{
			DataMemberNode selectedNode;
			CurrencyManager currencyManager;
			PropertyDescriptorCollection pdc;
			int index;
			ListBindableAttribute listBindableAttribute;

			if (depth > 10)
				return;
			if (!(isList) && this.selectLists)
				return;
			selectedNode = new DataMemberNode(dataMember, propertyName, isList);
			nodes.Add(selectedNode);
			if (this.selectedItem != null && this.selectedItem.Equals(dataSource, dataMember))
				this.selectedNode = selectedNode;
			if (isList) 
			{
				currencyManager = (CurrencyManager) bindingManager[dataSource, dataMember];
				pdc = currencyManager.GetItemProperties();
				index = 0;
				while (index < pdc.Count) 
				{
					listBindableAttribute = (ListBindableAttribute) pdc[index].Attributes[typeof(ListBindableAttribute)];
					if (listBindableAttribute == null || listBindableAttribute.ListBindable)
						this.FillDataMembers(bindingManager, dataSource, dataMember + "." + pdc[index].Name, pdc[index].Name, typeof(IList).IsAssignableFrom(pdc[index].PropertyType), selectedNode.Nodes, depth + 1);
					index++;
				}
			}
		}
		private int GetMaxItemWidth(TreeNodeCollection nodes) 
		{
			int val;
			TreeNode current;
			Rectangle bounds;
			int right;
			bool isExpanded;
			IEnumerator iEnumerator;
			IDisposable iDisposable;

			val = 0;
			iEnumerator = nodes.GetEnumerator();
			try 
			{
				while (iEnumerator.MoveNext()) 
				{
					current = (TreeNode) iEnumerator.Current;
					bounds = current.Bounds;
					right = bounds.Left + bounds.Width;
					isExpanded = current.IsExpanded;
					try 
					{
						val = Math.Max(right, val);
						current.Expand();
						val = Math.Max(val, this.GetMaxItemWidth(current.Nodes));
					}
					finally 
					{
						if (!isExpanded)
							current.Collapse();
					}
				}
			}
			finally 
			{
				iDisposable = iEnumerator as IDisposable;
				if (iDisposable != null)
					iDisposable.Dispose();
			}
			return val;
		}
        
		private void SetSelectedItem(TreeNode node) 
		{
			DataMemberNode current;

			this.selectedItem = null;
			if (node as DataMemberNode != null) 
			{
				current = (DataMemberNode) node;
				if (this.selectLists == current.IsList)
					this.selectedItem = new DesignBinding(current.DataSource, current.DataMember);
				return;
			}
			if ((node as NoneNode) != null)
				this.selectedItem = DesignBinding.Null;
		}
		// Nested Classes
		internal class DataSourceNode : TreeNode
		{
			// Fields
			private IComponent dataSource;
            
			// Constructors
			public DataSourceNode(IComponent dataSource)
				: base(dataSource.Site.Name, DesignBindingPicker.BINDER_IMAGE, DesignBindingPicker.BINDER_IMAGE)
			{
				this.dataSource = dataSource;
			}
            
			// Methods
			public IComponent DataSource 
			{ 
				get
				{
					return this.dataSource;
				}
			}
		}
		
		internal class DataMemberNode : TreeNode
		{
			// Fields
			private bool isList;
			private string dataMember;
            
			// Constructors
			public DataMemberNode(string dataMember, string dataField, bool isList)
				: base(dataField, DesignBindingPicker.COLUMN_IMAGE, DesignBindingPicker.COLUMN_IMAGE)
			{
				this.dataMember = dataMember;
				this.isList = isList;
			}
			// Methods
			public bool IsList 
			{ 
				get
				{
					return this.isList;
				}
			}
            
			public string DataMember 
			{ 
				get
				{
					return this.dataMember;
				}
			}
            
			public string DataField 
			{ 
				get
				{
					return this.Text;
				}
			}
            
			public IComponent DataSource 
			{ 
				get
				{
					TreeNode parent;

					parent = this;
					while (parent as DataMemberNode != null)
						parent = parent.Parent;
					if (parent as DataSourceNode != null)
						return ((DataSourceNode) parent).DataSource;
					return null;
				}
			}
		}
		
		internal class NoneNode : TreeNode
		{
            
			// Constructors
			public NoneNode()
				: base("Null", DesignBindingPicker.NONE_IMAGE, DesignBindingPicker.NONE_IMAGE)
			{
			}
            
		}
	}
}