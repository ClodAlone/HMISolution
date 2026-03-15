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
using System.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Diagnostics;
using Syncfusion.Reflection;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <exclude/>
	public class TabControlAdvDesigner : ParentControlDesigner, ITabControlAdvDesigner
	{
		static string VerbCaption = "Add Tab";
		private bool disableDrawGrid = false;
		private bool tabComponentSelected = false;
		private DesignerVerbCollection verbs;
		private DesignerVerb removeVerb;
		private ToolTip tooltip;

		// To prevent unserializable child control from getting serialized
		private TabControlCollectionSerializationProvider dummySerProvider;

		public TabControlAdvDesigner()
		{
//			this.disableDrawGrid = 0;
//			this.persistedSelectedIndex = 0;
			return ;
		}

		//SmartTags added for .NET Framework 2.0        
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
						new TabControlActionList(this.Component));
				}
				return actionLists;
			}
		}
#endif

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this.verbs == null)
				{
					this.removeVerb = new DesignerVerb("Remove Tab",new EventHandler(this.OnRemove));
					this.verbs = new DesignerVerbCollection();
					this.verbs.Add(new DesignerVerb("Add Tab",new EventHandler(this.OnAdd)));
					this.verbs.Add(this.removeVerb);
				}

				this.removeVerb.Enabled = (this.Control.Controls.Count > 0);
				return this.verbs;
			}
		}

		private void CheckVerbStatus(object sender, ComponentChangedEventArgs e)
		{
			if (this.removeVerb != null)
				this.removeVerb.Enabled = (this.Control.Controls.Count > 0);
		}

		public override /*ParentControlDesigner*/ bool CanParent(Control control)
	{
			return (control is TabPageAdv);
		}

	// This override allows you to prevent the user from dragging and dropping a toolbox item into the Control designer.
	protected override IComponent[] CreateToolCore(
		ToolboxItem tool,
		int x,
		int y,
		int width,
		int height,
		bool hasLocation,
		bool hasSize
		)
	{
		throw new NotSupportedException("Cannot drag and drop controls from the toolbox into this component, please use the " + VerbCaption + " verb in the context menu to add tab pages (TabPageAdv Controls) into this Control.");
	}

		public override /*ParentControlDesigner*/ void Initialize(IComponent component)
		{
			System.ComponentModel.Design.ISelectionService iSelectionService;
			System.ComponentModel.Design.IComponentChangeService iComponentChangeService;

			base.Initialize(component);
			iSelectionService = (System.ComponentModel.Design.ISelectionService)this.GetService(typeof(System.ComponentModel.Design.ISelectionService));
			if (iSelectionService != null)
				iSelectionService.SelectionChanged += new EventHandler(this.OnSelectionChanged);

			iComponentChangeService = (System.ComponentModel.Design.IComponentChangeService)this.GetService(typeof(System.ComponentModel.Design.IComponentChangeService));
			if (iComponentChangeService != null)
				iComponentChangeService.ComponentChanged += new ComponentChangedEventHandler(this.CheckVerbStatus);

			((TabControlAdv)component).SelectedIndexChanged += new EventHandler(this.OnTabSelectedIndexChanged);
			((TabControlAdv)component).GotFocus += new EventHandler(this.OnGotFocus);

			// Serialization listeners
			System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager =
				(System.ComponentModel.Design.Serialization.IDesignerSerializationManager)
				this.GetService(typeof(System.ComponentModel.Design.Serialization.IDesignerSerializationManager));

			if(manager != null)
			{
				dummySerProvider = new TabControlCollectionSerializationProvider(this.Control.Controls, this);
				manager.AddSerializationProvider(this.dummySerProvider);
//				manager.SerializationComplete += new EventHandler(this.SerializationCompleteHandler);
			}

			IDesignerHost iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			iDesignerHost.LoadComplete += new EventHandler(this.DesignerLoadCompleteHandler);

			tooltip = new ToolTip();
			tooltip.AutomaticDelay = 0;
			tooltip.InitialDelay = 0;
			tooltip.ReshowDelay = 0;
			tooltip.Active = true;
			tooltip.ShowAlways = true;
			tooltip.SetToolTip(this.Control, "Select " + VerbCaption + " from the Context Menu to add tab pages into this tab control.");
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				System.ComponentModel.Design.ISelectionService iSelectionService;
				System.ComponentModel.Design.IComponentChangeService iComponentChangeService;

				iSelectionService = (System.ComponentModel.Design.ISelectionService)this.GetService(typeof(System.ComponentModel.Design.ISelectionService));
				if (iSelectionService != null)
					iSelectionService.SelectionChanged -= new EventHandler(this.OnSelectionChanged);

				iComponentChangeService = (System.ComponentModel.Design.IComponentChangeService)this.GetService(typeof(System.ComponentModel.Design.IComponentChangeService));
				if (iComponentChangeService != null)
					iComponentChangeService.ComponentChanged -= new ComponentChangedEventHandler(this.CheckVerbStatus);

				if(this.Component != null && this.Component is TabControlAdv)
				{
					((TabControlAdv)this.Component).SelectedIndexChanged -= new EventHandler(this.OnTabSelectedIndexChanged);
					((TabControlAdv)this.Component).GotFocus -= new EventHandler(this.OnGotFocus);
				}

				IDesignerHost iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
				if(iDesignerHost != null)
					iDesignerHost.LoadComplete -= new EventHandler(this.DesignerLoadCompleteHandler);
			}

			base.Dispose(disposing);
		}

		protected override /*ParentControlDesigner*/ bool DrawGrid
		{
			get
			{
				if (!this.disableDrawGrid)
					return base.DrawGrid;
				else
					return false;
			} // end of method get_DrawGrid
		}

//		Don't need this since the TabControlAdv doesn't return HTTRANSPARENT for any area.
//		protected override /*ParentControlDesigner*/ void WndProc(ref Message m)
//		{
//			int msg;
//			msg = m.Msg;
//			if (msg == 0x84/*WM_NCHITTEST*/)
//			{
//				base.WndProc(ref m);
//				if ((int)m.Result == -1/*HTTRANSPARENT*/)
//					m.Result = (IntPtr)1/*HTCLIENT*/;
//
//				return;
//			}
//
//			base.WndProc(ref m);
//		}

		protected override /*ParentControlDesigner*/ void OnPaintAdornments(PaintEventArgs pe)
		{
			try
			{
				this.disableDrawGrid = true;
				base.OnPaintAdornments(pe);
			}
			finally
			{
				this.disableDrawGrid = false;
			}
		} // end of method OnPaintAdornments

		protected override /*ControlDesigner*/ bool GetHitTest(Point point)
		{
			TabControlAdv tabControl;
			tabControl = (TabControlAdv)this.Control;
			if (tabControl.TabPages.Count > 1)
			{
				point = tabControl.PointToClient(point);
				return this.tabComponentSelected &&
					((tabControl.Renderer.HitTestTabs(point, false) != -1) || tabControl.GetHitTestScroll( point ) );
			}
			else
				return false;
		}

        protected override void OnMouseLeave()
        {
            ( this.Control as TabControlAdv ).TabPrimitivesHost.HandledMouseLeave();
            base.OnMouseLeave();
        }

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			System.ComponentModel.Design.ISelectionService iSelectionService;
			System.Collections.ICollection selectedComponents;
			TabControlAdv tabControl;
			TabPageAdv tabPage;

			iSelectionService = (System.ComponentModel.Design.ISelectionService)this.GetService(typeof(System.ComponentModel.Design.ISelectionService));
			this.tabComponentSelected = false;
			if (iSelectionService != null)
			{
				selectedComponents = iSelectionService.GetSelectedComponents();
				tabControl = (TabControlAdv)this.Component;
				foreach(object selectedComponent in selectedComponents)
				{
					if(selectedComponent == tabControl)
						this.tabComponentSelected = true;

					tabPage = TabControlAdvDesigner.GetTabPageOfComponent(selectedComponent);
					if (tabPage != null)
					{
						if (tabPage.Parent == (Control)tabControl)
						{
							this.tabComponentSelected = true;
							tabControl.SelectedTab = tabPage;
						}
					}
				}
			}
		}

		private void OnTabSelectedIndexChanged(object sender, EventArgs e)
		{
			System.ComponentModel.Design.ISelectionService iSelectionService;
			System.Collections.ICollection selectedComponents;
			TabControlAdv tabControl;
			bool changeSelection;
			TabPageAdv tabPage;
			object[] newSelection;

			tabControl = (TabControlAdv)this.Component;

			iSelectionService = (System.ComponentModel.Design.ISelectionService)this.GetService(typeof(System.ComponentModel.Design.ISelectionService));
			if (iSelectionService != null)
			{
				selectedComponents = iSelectionService.GetSelectedComponents();
				changeSelection = true;

				foreach(object selectedComponent in selectedComponents)
				{
					tabPage = TabControlAdvDesigner.GetTabPageOfComponent(selectedComponent);
					if(tabPage != null &&
						tabPage.Parent == (Control)tabControl &&
						tabPage == tabControl.SelectedTab)
						changeSelection = false;
				}
				if (changeSelection)
				{
					newSelection = (object[])new System.Object[1];
					newSelection[0] = this.Component;
					iSelectionService.SetSelectedComponents((ICollection)newSelection);
				}
			}

			//this.SetDirty();

			// Also ensure that the order of the tabpages and the order of the Controls list are in sync.
			foreach(TabPageAdv TabPageAdv in tabControl.TabPages)
			{
				TabPageAdv.SendToBack();
			}
		}

		private void OnGotFocus(object sender, EventArgs e)
		{
			System.Windows.Forms.Design.EventHandlerService iEventHandlerService;
			System.Windows.Forms.Control control;
			iEventHandlerService = (System.Windows.Forms.Design.EventHandlerService)this.GetService(typeof(System.Windows.Forms.Design.EventHandlerService));
			if (iEventHandlerService != null)
			{
				control = iEventHandlerService.FocusWindow;
				if (control != null)
					control.Focus();
			}
		}

		internal static TabPageAdv GetTabPageOfComponent(object selectedComponent)
		{
			System.Windows.Forms.Control selectedControl;
			if (!(selectedComponent is System.Windows.Forms.Control))
				return null;

			selectedControl = (System.Windows.Forms.Control)selectedComponent;

			while(selectedControl != null)
			{
				if(selectedControl is TabPageAdv)
					return (TabPageAdv)selectedControl;

				selectedControl = selectedControl.Parent;
			}

			return null;
		}

		private void OnRemove(object sender, EventArgs eevent)
		{
			TabControlAdv tabControl;
			System.ComponentModel.MemberDescriptor memberDescriptor;
			TabPageAdv tabPage;
			System.ComponentModel.Design.IDesignerHost iDesignerHost;
			System.ComponentModel.Design.DesignerTransaction designerTransaction;
			System.ComponentModel.Design.CheckoutException checkoutException;
			tabControl = (TabControlAdv)this.Component;
			if (tabControl == null || tabControl.TabPages.Count == 0)
				return ;

			memberDescriptor = (MemberDescriptor)TypeDescriptor.GetProperties((object)this.Component)[(string)@"Controls"];
			tabPage = tabControl.SelectedTab;
			iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			if (iDesignerHost != null)
			{
				designerTransaction = null;
				try
				{
					try
					{
						designerTransaction = iDesignerHost.CreateTransaction(String.Concat((string)@"Remove ",tabPage.Site.Name,(string)@" from ",this.Component.Site.Name));
						this.RaiseComponentChanging(memberDescriptor);
					}
					catch(System.ComponentModel.Design.CheckoutException exception)
					{
						checkoutException = exception;
						if (checkoutException != CheckoutException.Canceled)
							throw checkoutException;
					}
					iDesignerHost.DestroyComponent((IComponent)tabPage);
					this.RaiseComponentChanged(memberDescriptor,null,null);
				}
				finally
				{
					if (designerTransaction != null)
						designerTransaction.Commit();
				}
			}
		}

		//IDesignerSerializationManager serManager = null;

		private TypeLoader GetExisitingTypeLoader()
		{
			// Parse through all the components in the component collection of the designer
/*			IDesignerHost iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			ComponentCollection componentCollection = iDesignerHost.Container.Components;
*/
			IDesignerHost iDesignerHost = ( System.ComponentModel.Design.IDesignerHost )this.GetService( typeof( System.ComponentModel.Design.IDesignerHost ) );
			ComponentCollection componentCollection = iDesignerHost.Container.Components;
			
			foreach( object obj in componentCollection )
			{
				TypeLoader objTypeLoader = obj as TypeLoader;

				if( null == objTypeLoader ) continue;

				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties( obj )[ ( string )@"Name" ];

				if( propertyDescriptor == null || propertyDescriptor.PropertyType != typeof( System.String ) ) 
					continue;

				if( "designTimeTabTypeLoader" 
					== (string)propertyDescriptor.GetValue( obj ) )
				{
					return objTypeLoader;
				}

			}

			return null;
			/*
			if(serManager == null)
				serManager =(IDesignerSerializationManager)
					this.GetService(typeof(IDesignerSerializationManager));

			object tab = serManager.GetInstance("designTimeTabTypeLoader");

			if(tab != null && !(tab is TypeLoader))
				MessageBox.Show("Error: A variable designTimeTabTypeLoader of a type different from TypeLoader is found. Rename the existing designTimeTabTypeLoader variable.");

			if(tab != null)
				return tab as TypeLoader;
			else
				return null;*/
		}
		private void SetupTypeLoaderComponent(TypeLoader typeLoader)
		{
			if(typeLoader == null)
				return;

			// Get a reference to the TypesToLoadList property,
			// so that you can call InitInvokeMemberSettings on it.
			// Calling this member will inturn call InvokeMember (with the provided arguments)
			// on the types in the list (there by loading the types) and also on any types
			// that gets added later into the list.
			// Need to do this everytime the designer gets loaded because the "InvokeMember"
			// settings are not persisted in code.
			TypesToLoadList typesList = null;

			PropertyDescriptor typeListProperty = TypeDescriptor.GetProperties((object)typeLoader)[(string)@"TypesToLoadList"];
			if (typeListProperty != null)
			{
				if (typeListProperty.PropertyType == typeof(TypesToLoadList))
					typesList = typeListProperty.GetValue((object)typeLoader) as TypesToLoadList;
			}

			if(typesList != null)
			{
				Type typesListType = typeof(TypesToLoadList);

				//	System.Reflection.MethodInfo memberInfo = typesListType.GetMethod("InitInvokeMemberSettings",
				//		System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod);

				// These are the arguments for "InitInvokeMemberSettings" which in turn
				// gets sent to InvokeMember
				Object[] args = new Object[]{"TabStyleName",
												System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty,
												null,
												null,
												null,
												null,
												null};

				try
				{
					typesListType.InvokeMember("InitInvokeMemberSettings", System.Reflection.BindingFlags.Default
						| System.Reflection.BindingFlags.InvokeMethod, null, typesList, args);
				}
				catch(Exception e){MessageBox.Show(e.Message);}
			}
		}

		void ITabControlAdvDesigner.InitTypeLoaderComponent()
		{
			TypeLoader typeLoader = GetExisitingTypeLoader();
			// Create a TypeLoader instance and insert into designer if not already there.
			if(typeLoader == null)
			{
				IDesignerHost iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
				// Create the typeLoader component...
				typeLoader = (TypeLoader)iDesignerHost.CreateComponent(typeof(TypeLoader));
				// ... and name it "designTimeTabTypeLoader
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties((object)typeLoader)[(string)@"Name"];
				if (propertyDescriptor != null)
				{
					if (propertyDescriptor.PropertyType == typeof(System.String))
						propertyDescriptor.SetValue((object)typeLoader, "designTimeTabTypeLoader");
				}

				iDesignerHost.Container.Add(typeLoader);

				// Notify Component Added
				IComponentChangeService componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
				if(componentChangeService != null)
					componentChangeService.OnComponentChanged(typeLoader, null, null, null);
			}
			this.SetupTypeLoaderComponent(typeLoader);
		}

		private void OnAdd(object sender, EventArgs eevent)
		{
			TabControlAdv tabControl;
			System.ComponentModel.MemberDescriptor memberDescriptor;
			System.ComponentModel.Design.IDesignerHost iDesignerHost;
			System.ComponentModel.Design.DesignerTransaction designerTransaction;
			System.ComponentModel.Design.CheckoutException checkoutException;
			TabPageAdv tabPage;
			string tabName;
			System.ComponentModel.PropertyDescriptor propertyDescriptor;

			tabControl = (TabControlAdv)this.Component;
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
						designerTransaction = iDesignerHost.CreateTransaction(String.Concat((string)@"Add tab to ",this.Component.Site.Name));
						this.RaiseComponentChanging(memberDescriptor);
					}
					catch(System.ComponentModel.Design.CheckoutException exception)
					{
						checkoutException = exception;
						if (checkoutException == CheckoutException.Canceled)
							throw checkoutException;
					}
					tabPage = (TabPageAdv)iDesignerHost.CreateComponent(this.GetChildType());
					tabName = null;
					propertyDescriptor = TypeDescriptor.GetProperties((object)tabPage)[(string)@"Name"];
					if (propertyDescriptor != null)
					{
						if (propertyDescriptor.PropertyType == typeof(System.String))
							tabName = (string)(System.String)propertyDescriptor.GetValue((object)tabPage);
					}
					if ((tabName!=null))
						tabPage.Text = tabName;

					tabControl.Controls.Add((Control)tabPage);
					this.RaiseComponentChanged(memberDescriptor,null,null);
				}
				finally
				{
					if (designerTransaction != null)
						designerTransaction.Commit();
				}
			}
		}
		private Type GetChildType()
		{
			Type childType = typeof(TabPageAdv);
			object[] atts = this.Component.GetType().GetCustomAttributes(typeof(DefaultChildTypeAttribute), true);
			if(atts.Length > 0)
			{
				DefaultChildTypeAttribute attribute = atts[0] as DefaultChildTypeAttribute;

				if(!typeof(TabPageAdv).IsAssignableFrom(attribute.ChildType))
					MessageBox.Show("Specified custom type " + attribute.ChildType.ToString() + " is not derived from TabPageAdv. Creating a TabPageAdv instance.", "TabControlAdv designer warning:");
				else
					childType = attribute.ChildType;
			}
			return childType;
		}

		public void RemoveUnserializableChildControls()
		{
			TabControlAdv tabControl = this.Control as TabControlAdv;
			if(tabControl != null)
			{
				ArrayList childControls = new ArrayList();
				for(int i = tabControl.Controls.Count - 1; i >= 0; i--)
				{
					Control childControl = tabControl.Controls[i];
					if(childControl != null && !(childControl is TabPageAdv))
					{
						childControls.Add(childControl);
						tabControl.Controls.Remove(childControl);
					}
				}
				if(childControls.Count > 0)
					tabControl.ChildControlsRemovedByDesigner(childControls);
			}
		}
/*		public void ResetUnserializableChildControls()
		{
			TabControlAdv tabControl = this.Control as TabControlAdv;
			if(tabControl != null)
				foreach(Control childControl in tabControl.Controls)
					tabControl.Controls.Add((Control)childControl);
		}
		*/
		public void DesignerLoadCompleteHandler(object sender, EventArgs e)
		{
//			this.ResetUnserializableChildControls();
//			InitTypeLoaderComponent();
			this.SetupTypeLoaderComponent(this.GetExisitingTypeLoader());
		}
		protected virtual void SetDirty()
		{
			System.ComponentModel.MemberDescriptor memberDescriptor;
			System.ComponentModel.Design.IDesignerHost iDesignerHost;

			memberDescriptor = (MemberDescriptor)TypeDescriptor.GetProperties((object)this.Component)[(string)"TabPages"];
			iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			if (iDesignerHost != null)
				this.RaiseComponentChanged(memberDescriptor,null,null);
		}
	}
}