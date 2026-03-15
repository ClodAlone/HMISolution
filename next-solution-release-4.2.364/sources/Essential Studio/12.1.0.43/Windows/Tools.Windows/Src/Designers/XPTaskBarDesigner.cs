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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Globalization;
using System.Drawing.Design;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    ///<exclude/>
	public class XPTaskBarDesigner :
		ScrollableControlDesigner
	{
		static string VerbCaption = "Add Task Box";
		private DesignerVerbCollection verbs;
		private ToolTip tooltip;
		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this.verbs == null)
				{
					this.verbs = new DesignerVerbCollection();
					this.verbs.Add(new DesignerVerb(VerbCaption ,new EventHandler(this.OnAdd)));
				}

				return this.verbs;
			}
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
						new XPTaskBarActionList(this.Component));
				}
				return actionLists;
			}
		}

#endif

		public override /*ControlDesigner*/ void Initialize(IComponent component)
		{
			base.Initialize(component);

			tooltip = new ToolTip();
			tooltip.AutomaticDelay = 0;
			tooltip.InitialDelay = 0;
			tooltip.ReshowDelay = 0;
			tooltip.Active = true;
			tooltip.ShowAlways = true;
			tooltip.SetToolTip(this.Control, "Select " + VerbCaption + " from the Context Menu to add a new menu Category");
		}

		protected virtual Control GetSelectedControl()
		{
			ISelectionService service = (ISelectionService)base.GetService(typeof(ISelectionService));
			if(service != null)
				return (service.PrimarySelection as Control);

			return null;
		}

		protected override /*ControlDesigner*/ void OnDragOver(DragEventArgs de)
		{
			base.OnDragOver(de);

			XPTaskBarBox source = GetSelectedControl() as XPTaskBarBox;
			if(source != null)
			{
				Rectangle sourceBounds = source.Bounds;
				Point dragPoint = this.Control.PointToClient(new Point(de.X, de.Y));
				// There is no change in visual feedback though!
				if(sourceBounds.Contains(dragPoint))
					de.Effect = DragDropEffects.None;
				else
					de.Effect = DragDropEffects.Move;
			}
		}

		protected override /*ControlDesigner*/ void OnDragDrop(DragEventArgs de)
		{
			base.OnDragDrop(de);
			XPTaskBarBox source = GetSelectedControl() as XPTaskBarBox;
			// Move the control to the new position
			if(source != null)
			{
				XPTaskBarBox dest = GetTaskMenuBoxUnder(this.Control.PointToClient(new Point(de.X, de.Y)), source);
				if(dest != null)
				{
					Control.ControlCollection controlCollection = this.Control.Controls;
					controlCollection.SetChildIndex(source,
						controlCollection.GetChildIndex(dest));
//					source.PerformLayout();
				}
			}
		}

		protected virtual XPTaskBarBox GetTaskMenuBoxUnder(Point pt, XPTaskBarBox ignoreControl)
		{
			foreach(XPTaskBarBox taskMenuBox in this.Control.Controls)
			{
				if(pt.Y < taskMenuBox.Bounds.Bottom
					&& taskMenuBox != ignoreControl)
					return taskMenuBox;
			}
			return null;
		}

//		protected override bool GetHitTest(Point point)
//		{
//			return false;
//		}

		public override /*ParentControlDesigner*/ bool CanParent(Control control)
		{
			return (control is XPTaskBarBox);
		}

		public override bool CanParent(ControlDesigner controlDesigner)
		{
			return (controlDesigner is XPTaskBarBoxDesigner);
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
			throw new NotSupportedException("Cannot drag and drop controls from the toolbox into this component, please use the " + VerbCaption + " verb to add XPTaskBarBox Controls into this Control.");
		}
		private void OnAdd(object sender, EventArgs eevent)
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
						designerTransaction = iDesignerHost.CreateTransaction(String.Concat((string)@"Add task menu category to ",this.Component.Site.Name));
						this.RaiseComponentChanging(memberDescriptor);
					}
					catch(System.ComponentModel.Design.CheckoutException exception)
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
					if ((compName!=null))
						taskMenuBox.Text = compName;

					xpTaskBar.Controls.Add((Control)taskMenuBox);
					this.RaiseComponentChanged(memberDescriptor,null,null);
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
		private Type GetChildType()
		{
			Type childType = typeof(XPTaskBarBox);
			object[] atts = this.Component.GetType().GetCustomAttributes(typeof(Syncfusion.Windows.Forms.Design.DefaultChildTypeAttribute), true);
			if(atts.Length > 0)
			{
				DefaultChildTypeAttribute attribute = atts[0] as DefaultChildTypeAttribute;

				if(!typeof(XPTaskBarBox).IsAssignableFrom(attribute.ChildType))
					MessageBox.Show("Specified custom type " + attribute.ChildType.ToString() + " is not derived from XPTaskBarBox. Creating a XPTaskBarBox instance.", "XPTaskBar designer warning:");
				else
					childType = attribute.ChildType;
			}
			return childType;
		}
		protected override /*ParentControlDesigner*/ void OnPaintAdornments(PaintEventArgs pe)
		{
			Syncfusion.Windows.Forms.Tools.XPTaskBar taskBar;
			taskBar = this.Component as Syncfusion.Windows.Forms.Tools.XPTaskBar;
			if (taskBar != null && taskBar.BorderStyle == BorderStyle.None)
				DrawingUtils.DrawDesignTimeBorder(pe.Graphics, taskBar);

			base.OnPaintAdornments(pe);
		}
	}
    /// <exclude/>
	class XPTaskBarBoxDesigner :
		ParentControlDesigner
	{
		#region Class constants
		private const string DEF_ITEMS_PROPERTY_NAME = "Name";
		private const string DEF_ADD_ITEM_VERB_TEXT = "Add Item";
		#endregion

		#region Class members
		XPTaskBarBox control;
		private DesignerVerbCollection m_verbs = null;
		#endregion

		#region Verbs processing
		public override DesignerVerbCollection Verbs
		{
			get
			{
				if( m_verbs == null )
				{
					InitVerbs();
				}

				return m_verbs;
			}
		}

		private void InitVerbs()
		{
			m_verbs = new DesignerVerbCollection();

			DesignerVerb addItemVerb = new DesignerVerb( DEF_ADD_ITEM_VERB_TEXT,
				new EventHandler( this.OnAddItem ) );

			m_verbs.Add( addItemVerb );
		}

		private void OnAddItem( object sender, EventArgs e )
		{
			XPTaskBarBox taskBarBox = this.Component as XPTaskBarBox;
			if( taskBarBox != null )
			{
				IDesignerHost host = this.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				if( host != null )
				{
					DesignerTransaction transaction = host.CreateTransaction();

					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties( taskBarBox );
					PropertyDescriptor descriptor = properties[ DEF_ITEMS_PROPERTY_NAME ];

					this.RaiseComponentChanging( descriptor );

					XPTaskBarItem newItem = new XPTaskBarItem();
					taskBarBox.Items.Add( newItem );

					this.RaiseComponentChanged( descriptor, null, null );

					transaction.Commit();

				}
			}
		}

		#endregion

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
						new XPTaskBarBoxActionList(this.Component));
				}
				return actionLists;
			}
		}

#endif		

		public override /*ControlDesigner*/ void Initialize( IComponent component )
		{
			base.Initialize( component );
			this.control = component as XPTaskBarBox;
			if(this.control != null)
			{
				this.control.SizeChanged += new EventHandler(this.SourceControl_SizeChanged);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(this.control != null)
				{
					this.control.SizeChanged -= new EventHandler(this.SourceControl_SizeChanged);
				}
			}
			base.Dispose(disposing);
		}
		private void SourceControl_SizeChanged(object sender, EventArgs e)
		{
			Type type = typeof(System.Windows.Forms.Design.AnchorEditor);
			type = type.Assembly.GetType("System.Windows.Forms.Design.ISelectionUIService");
			if(type != null)
			{
				object selectionUIService = this.GetService(type);
				DesignTimeUtils.SyncSelection(selectionUIService);
			}
		}
		public override /*ParentControlDesigner*/ bool CanParent(Control control)
		{
			XPTaskBarBox box = this.Component as XPTaskBarBox;
			// My Children can only be of type Panel and there can be only 1 child.
			return (control is Panel) && box.Controls.Count == 0;
		}

		public override /*ControlDesigner*/ SelectionRules SelectionRules
		{
			get
			{
				System.Windows.Forms.Design.SelectionRules selectionRules;
				System.Windows.Forms.Control control;
				selectionRules = base.SelectionRules;
				control = this.Control;
				if (control.Parent is XPTaskBar)
					selectionRules = (SelectionRules)(selectionRules & ~(SelectionRules.AllSizeable));

				return selectionRules;
			}
		}

		public override /*ControlDesigner*/ bool CanBeParentedTo(IDesigner parentDesigner)
		{
			return (parentDesigner is XPTaskBarDesigner);
		}

		// If returning false, doesn't allow the user to click and drop Panel from toolbox.
//		protected override bool EnableDragRect {get{return false;}
//		}

		protected override /*ParentControlDesigner*/ void OnPaintAdornments(PaintEventArgs pe)
		{

			Syncfusion.Windows.Forms.Tools.XPTaskBarBox taskBarBox;
			taskBarBox = (Syncfusion.Windows.Forms.Tools.XPTaskBarBox)this.Component;
			if (taskBarBox.HeaderBackColor == taskBarBox.BackColor
					|| taskBarBox.ItemBackColor == taskBarBox.BackColor)
				DrawingUtils.DrawDesignTimeBorder(pe.Graphics, taskBarBox);

			base.OnPaintAdornments(pe);
		}
	}
}
