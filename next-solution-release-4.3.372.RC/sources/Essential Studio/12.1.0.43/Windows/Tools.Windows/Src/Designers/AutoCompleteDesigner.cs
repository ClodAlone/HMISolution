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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
//using Syncfusion.Win32;
using Syncfusion.Windows.Forms.Tools;


#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Text;
using System.Reflection;
using Syncfusion.Windows.Forms.Design;
#endif

namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// This is a designer for the AutoComplete control.
	/// </summary>
	public class AutoCompleteDesigner : ComponentDesigner 
	{
		/// <summary>
		/// Designer verb for adding a button.
		/// </summary>
		protected DesignerVerb dvRefreshColumns = null;

		/// <summary>
		/// The collection of designer verbs.
		/// </summary>
		protected DesignerVerbCollection dvcVerbs = null;

		/// <summary>
		/// Initializes a new instance of the ButtonEditDesigner class.
		/// </summary>
		public AutoCompleteDesigner()
		{
			this.dvRefreshColumns = new DesignerVerb("Refresh Columns", new EventHandler(this.HandleRefreshColumns));
			this.dvRefreshColumns.Enabled = false;
			DesignerVerb[] dvarray = new DesignerVerb[] { dvRefreshColumns }; 
			this.dvcVerbs = new DesignerVerbCollection(dvarray);
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
						new AutoCompleteActionList(this.Component));
				}
				return actionLists;
			}
		}

		public override void InitializeExistingComponent( IDictionary defaultValues )
		{
			base.InitializeExistingComponent( defaultValues );

			InitializeExistingComponent();
		}

#endif

		/// <summary>
		/// Returns the designer verbs collection.
		/// </summary>
		public override DesignerVerbCollection Verbs 
		{
			get
			{
				if(((AutoComplete)(this.Component)).TableData.Columns.Count > 0)
					this.dvRefreshColumns.Enabled = true;
				else
					this.dvRefreshColumns.Enabled = false;
				return this.dvcVerbs;	
			}
		}

		/// <summary>
		/// Handles the AddButton. 
		/// </summary>
		/// <param name="sender">The designer.</param>
		/// <param name="e">The event data.</param>
		public void HandleRefreshColumns(object sender, EventArgs e)
		{
			if(this.Component != null)
			{
				AutoComplete autoComplete = this.Component as AutoComplete;
				autoComplete.RefreshColumns();
			}
		}


		/// <summary>
		/// Overrides initialize.  Here we add an event handler to the selection service.
		/// Notice that we are very careful not to assume that the selection service is
		/// available.  It is entirely optional that a service is available and you should
		/// always degrade gracefully if a service could not be found.
		/// </summary>
		/// <param name="component">The AutoComplete control that is being designed.</param>
		public override void Initialize(IComponent component) 
		{
			base.Initialize(component);

			IDesignerHost idhost = component.Site.Container as IDesignerHost;
			Form mainfrm = idhost.RootComponent as Form;

			AutoComplete ac = component as AutoComplete;
			ac.ParentForm = mainfrm;

			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if (ss != null) 
			{
				ss.SelectionChanged += new EventHandler(HandleSelectionChanged);
			}
		}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		/// <summary>
		/// 
		/// </summary>
		public override void InitializeNonDefault()
		{
			base.InitializeNonDefault();
			InitializeExistingComponent();
		}
#endif

		private void InitializeExistingComponent()
		{
			AutoComplete ac = this.Component as AutoComplete;
			if(ac.ParentForm == null)
			{
				IDesignerHost idhost = this.Component.Site.Container as IDesignerHost;
				Form mainfrm = idhost.RootComponent as Form;
				ac.ParentForm = mainfrm;
			}
		}

		/// <summary>
		/// Overrides Dispose.  Here we remove our handler for the selection changed
		/// event.  With designers, it is critical that they clean up any event they
		/// have attached.  Otherwise, during the course of an editing session many
		/// designers may get created and never destroyed.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			ISelectionService iSelectionService = (ISelectionService)this.GetService(typeof(ISelectionService)); 
			if (iSelectionService != null) 
				iSelectionService.SelectionChanged -= new EventHandler(this.HandleSelectionChanged); 
			base.Dispose(disposing);
		}

		/// <summary>
		/// Handles the SelectionChanged event of the Selection service.
		/// </summary>
		/// <param name="sender">The selection service.</param>
		/// <param name="e">EventArgs with the event data.</param>
		private void HandleSelectionChanged(object sender, EventArgs e) 
		{
			ISelectionService ss = (ISelectionService)sender;
			UpdateAutoCompleteSelection(ss);
		}

		/// <summary>
		/// Updates the current selection.
		/// </summary>
		/// <param name="ss">The ISelectionService object.</param>
		private void UpdateAutoCompleteSelection(ISelectionService ss)
		{
			Control c = ss.PrimarySelection as Control;
			Syncfusion.Windows.Forms.Tools.AutoComplete autoComplete = (AutoComplete)this.Component;
			if (c != null) 
				autoComplete.ActiveFocusControl = c;
			else
			{
				if (autoComplete.ActiveFocusControl != null)
					autoComplete.ActiveFocusControl = null;
			}
		}

		/// <summary>
		/// Adjusts the set of properties the component exposes through a TypeDescriptor.
		/// </summary>
		/// <param name="properties">An IDictionary that contains the properties for the class of the component. </param>
		protected override void PreFilterProperties(IDictionary properties) 
		{
			base.PreFilterProperties(properties);

			String[] strcolln = new String[28];
			strcolln[0] = "RightToLeft";
			strcolln[1] = "ContextMenu";
			strcolln[2] = "ImeMode";
			strcolln[3] = "TabStop";			
			strcolln[4] = "Dock";
			strcolln[5] = "DockPadding";
			strcolln[6] = "Anchor";
			strcolln[7] = "AutoScroll";
			strcolln[8] = "CausesValidation";
			strcolln[9] = "AllowDrop";
			strcolln[10] = "BackgroundImage";
			strcolln[11] = "Cursor";
			strcolln[12] = "DialogResult";
			strcolln[13] = "Location";
			strcolln[14] = "Size";
			strcolln[15] = "AccessibleDescription";
			strcolln[16] = "AccessibleName";
			strcolln[17] = "AccessibleRole";
			strcolln[18] = "Locked";
			strcolln[19] = "Font";
			strcolln[20] = "Enabled";
			strcolln[21] = "BackColor";
			strcolln[22] = "ForeColor";
			strcolln[23] = "AutoScrollMargin";
			strcolln[24] = "AutoScrollMinSize";
			strcolln[25] = "TabIndex";
			strcolln[26] = "Visible";
			strcolln[27] = "DataBindings";

			RemovePropertyBrowsable(this.Component, strcolln, properties);	
		}

		/// <summary>
		/// Removes a set of properties.
		/// </summary>
		/// <param name="control">The control to which the changes apply.</param>
		/// <param name="strcolln">The array of property names to exclude.</param>
		/// <param name="properties">Contains the properties for the class of the component.</param>
		static private void RemovePropertyBrowsable(IComponent control, String[] strcolln, IDictionary properties)
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




	public class ComboBoxAutoCompleteDesigner : ControlDesigner 
	{
		/// <summary>
		/// Designer verb for adding a button.
		/// </summary>
		protected DesignerVerb dvRefreshColumns = null;

		/// <summary>
		/// The collection of designer verbs.
		/// </summary>
		protected DesignerVerbCollection dvcVerbs = null;

		/// <summary>
		/// Initializes a new instance of the ButtonEditDesigner class.
		/// </summary>
		public ComboBoxAutoCompleteDesigner()
		{
			this.dvRefreshColumns = new DesignerVerb("Refresh Columns", new EventHandler(this.HandleRefreshColumns));
			this.dvRefreshColumns.Enabled = false;
			DesignerVerb[] dvarray = new DesignerVerb[] { dvRefreshColumns }; 
			this.dvcVerbs = new DesignerVerbCollection(dvarray);
		} 

		/// <summary>
		/// Returns the designer verbs collection.
		/// </summary>
		public override DesignerVerbCollection Verbs 
		{
			get
			{
				if(((ComboBoxAutoComplete)(this.Control)).AutoCompleteControl.TableData.Columns.Count > 0)
					this.dvRefreshColumns.Enabled = true;
				else
					this.dvRefreshColumns.Enabled = false;
				return this.dvcVerbs;	
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		System.ComponentModel.Design.DesignerActionListCollection actionLists;

		public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
		{
			get
			{
				if (null == actionLists)
				{
					actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
					actionLists.Add(
						new ComboBoxAutoCompleteActionList(this.Component));
				}
				return actionLists;
			}
		}

#endif

		/// <summary>
		/// Handles the AddButton. 
		/// </summary>
		/// <param name="sender">The designer.</param>
		/// <param name="e">The event data.</param>
		public void HandleRefreshColumns(object sender, EventArgs e)
		{
			if(this.Control != null)
			{
				ComboBoxAutoComplete comboBoxAutoComplete = this.Control as ComboBoxAutoComplete;
				comboBoxAutoComplete.AutoCompleteControl.RefreshColumns();
			}
		}


		/// <summary>
		/// Overrides initialize.  Here we add an event handler to the selection service.
		/// Notice that we are very careful not to assume that the selection service is
		/// available.  It is entirely optional that a service is available and you should
		/// always degrade gracefully if a service could not be found.
		/// </summary>
		/// <param name="control">The AutoComplete control that is being designed.</param>
		public override void Initialize(IComponent control) 
		{
			base.Initialize(control);

			IDesignerHost idhost = control.Site.Container as IDesignerHost;
			Form mainfrm = idhost.RootComponent as Form;

			ComboBoxAutoComplete ac = control as ComboBoxAutoComplete;
			ac.ParentForm = mainfrm;

			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if (ss != null) 
			{
				ss.SelectionChanged += new EventHandler(HandleSelectionChanged);
			}
		}

		private void InitializeExistingComponent()
		{
			ComboBoxAutoComplete ac = this.Control as ComboBoxAutoComplete;
			if( ac.ParentForm == null )
			{
				IDesignerHost idhost = this.Component.Site.Container as IDesignerHost;
				Form mainfrm = idhost.RootComponent as Form;
				ac.AutoCompleteControl.ParentForm = mainfrm;
			}
		}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// 
		/// </summary>
		public override void InitializeNonDefault()
		{
			base.InitializeNonDefault();
			InitializeExistingComponent();
		}
#else
		public override void InitializeExistingComponent( IDictionary defaultValues )
		{
			base.InitializeExistingComponent( defaultValues );
		}
#endif
		/// <summary>
		/// Overrides Dispose.  Here we remove our handler for the selection changed
		/// event.  With designers, it is critical that they clean up any events they
		/// have attached.  Otherwise, during the course of an editing session many
		/// designers may get created and never destroyed.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			ISelectionService iSelectionService = (ISelectionService)this.GetService(typeof(ISelectionService)); 
			if (iSelectionService != null) 
				iSelectionService.SelectionChanged -= new EventHandler(this.HandleSelectionChanged); 
			base.Dispose(disposing);
		}

		/// <summary>
		/// Handles the SelectionChanged event of the Selection service.
		/// </summary>
		/// <param name="sender">The selection service.</param>
		/// <param name="e">EventArgs with the event data.</param>
		private void HandleSelectionChanged(object sender, EventArgs e) 
		{
			ISelectionService ss = (ISelectionService)sender;
			UpdateAutoCompleteSelection(ss);
		}

		/// <summary>
		/// Updates the current selection.
		/// </summary>
		/// <param name="ss">The ISelectionService object.</param>
		private void UpdateAutoCompleteSelection(ISelectionService ss)
		{
			Control c = ss.PrimarySelection as Control;
			Syncfusion.Windows.Forms.Tools.ComboBoxAutoComplete comboBoxAutoComplete = (ComboBoxAutoComplete)this.Control;
			if (c != null) 
				comboBoxAutoComplete.AutoCompleteControl.ActiveFocusControl = c;
			else
			{
				if (comboBoxAutoComplete.AutoCompleteControl.ActiveFocusControl != null)
					comboBoxAutoComplete.AutoCompleteControl.ActiveFocusControl = null;
			}
		}

		/// <summary>
		/// Adjusts the set of properties the component exposes through a TypeDescriptor.
		/// </summary>
		/// <param name="properties">An IDictionary that contains the properties for the class of the component. </param>
		protected override void PreFilterProperties(IDictionary properties) 
		{
			base.PreFilterProperties(properties);

			String[] strcolln = new String[9];
			strcolln[0] = "Items";
			strcolln[1] = "DataSource";
			strcolln[2] = "DisplayMember";
			strcolln[3] = "DropDownWidth";
			strcolln[4] = "IntegralHeight";
			strcolln[5] = "ItemHeight";
			strcolln[6] = "MaxDropDownItems";
			strcolln[7] = "Sorted";
			strcolln[8] = "ValueMember";
			RemovePropertyBrowsable(this.Component, strcolln, properties);	
		}


		/// <summary>
		/// Removes a set of properties.
		/// </summary>
		/// <param name="control">The control to which the changes apply.</param>
		/// <param name="strcolln">The array of property names to exclude.</param>
		/// <param name="properties">Contains the properties for the class of the component.</param>
		static private void RemovePropertyBrowsable(IComponent control, String[] strcolln, IDictionary properties)
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

	#region ActionList class
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	public class ComboBoxAutoCompleteActionList : SyncActionListBase<ComboBoxAutoComplete>
	{
		public ComboBoxAutoCompleteActionList (IComponent component)
			: base(component)
		{

		}

		protected override void InitializeActionList ( )
		{
			this.AddDesignerActionHeaderItem("Essential Tools - ComboBoxAutoComplete");
			this.AddDesignerActionPropertyItem("Name", "Name", "Appearance_", "Specifies the name of the control.");

			this.AddDesignerActionHeaderItem("Appearance");
			this.AddDesignerActionPropertyItem("FlatStyle", "FlatStyle", "Appearance", "Determines the display style of the control.");

			this.AddDesignerActionHeaderItem("Behavior");
			this.AddDesignerActionPropertyItem("DropDownHeight", "DropDown Height", "Behavior", "Specifies the height of the dropdown.");


			this.AddDesignerActionPropertyItem("AutoCompleteMode", "AutoCompleteMode", "Behavior", "Indicates the text completion behavior of ComboBox.");
			this.AddDesignerActionPropertyItem("AutoCompleteSource", "AutoCompleteSource", "Behavior", "Specifies the source of complete strings which is used for automatic completion.");
			this.AddDesignerActionPropertyItem("AllowNewText", "Allow NewText", "Behavior", "Specifies whether new text can be accepted to the autoComplete or not.");
			this.AddDesignerActionPropertyItem("ReadOnly", "ReadOnly", "Behavior", "Specifies whether the control should be made read-only or not.");
		}

		public string Name
		{
			get
			{
				string name = String.Empty;
				if (this.Control != null)
				{
					ComboBoxAutoComplete autoComplete = this.Control as ComboBoxAutoComplete;
					name = autoComplete.Name;
				}
				return name;
			}
			set
			{
				SetValue("Name", value);
			}
		}

		public FlatStyle FlatStyle
		{
			get
			{
				FlatStyle flatStyle = FlatStyle.System;
				if (this.Control != null)
				{
					ComboBoxAutoComplete autoComplete = this.Control as ComboBoxAutoComplete;
					flatStyle = autoComplete.FlatStyle;
				}
				return flatStyle;
			}
			set
			{
				SetValue("FlatStyle", value);
			}
		}

		public int DropDownHeight
		{
			get
			{
				int dropDownHeight = 106;
				if (this.Control != null)
				{
					ComboBoxAutoComplete autoComplete = this.Control as ComboBoxAutoComplete;
					dropDownHeight = autoComplete.DropDownHeight;
				}
				return dropDownHeight;
			}
			set
			{
				SetValue("DropDownHeight", value);
			}
		}

		public AutoCompleteMode AutoCompleteMode
		{
			get
			{
				AutoCompleteMode autoCompleteMode = AutoCompleteMode.None;
				if (this.Control != null)
				{
					ComboBoxAutoComplete autoComplete = this.Control as ComboBoxAutoComplete;
					autoCompleteMode = autoComplete.AutoCompleteMode;
				}
				return autoCompleteMode;
			}
			set
			{
				SetValue("AutoCompleteMode", value);
			}
		}

		public AutoCompleteSource AutoCompleteSource
		{
			get
			{
				AutoCompleteSource autoCompleteSource = AutoCompleteSource.None;
				if (this.Control != null)
				{
					ComboBoxAutoComplete autoComplete = this.Control as ComboBoxAutoComplete;
					autoCompleteSource = autoComplete.AutoCompleteSource;
				}
				return autoCompleteSource;
			}
			set
			{
				SetValue("AutoCompleteSource", value);
			}
		}

		public bool AllowNewText
		{
			get
			{
				bool allowNewText = true;
				if (this.Control != null)
				{
					ComboBoxAutoComplete autoComplete = this.Control as ComboBoxAutoComplete;
					allowNewText = autoComplete.AllowNewText;
				}
				return allowNewText;
			}
			set
			{
				SetValue("AllowNewText", value);
			}
		}

		public bool ReadOnly
		{
			get
			{
				bool readOnly = false;
				if (this.Control != null)
				{
					ComboBoxAutoComplete autoComplete = this.Control as ComboBoxAutoComplete;
					readOnly = autoComplete.ReadOnly;
				}
				return readOnly;
			}
			set
			{
				SetValue("ReadOnly", value);
			}
		}
	}
#endif
	#endregion
}