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
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Timers;


namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// The designer for the SplashControl class. 
	/// </summary>
	public class SplashControlDesigner : ComponentDesigner
	{

		/// <summary>
		/// Designer verb for previewing the Splash Form.
		/// </summary>
		protected DesignerVerb dvDisplaySplash = null;

		/// <summary>
		/// Designer verb for cancelling the Splash Form being
		/// previewed.
		/// </summary>
		protected DesignerVerb dvCancelSplash = null;

		/// <summary>
		/// The designer verbs collection
		/// </summary>
		protected DesignerVerbCollection dvcVerbs = null;

		/// <summary>
		/// For the preview splash verb.
		/// </summary>
		private Point customPanelLocation = Point.Empty;

		/// <summary>
		/// Indicates whether a custom splash panel is being used.
		/// </summary>
		private bool customSplashUsed = false;

		/// <summary>
		/// Creates a new object of type SplashControlDesigner
		/// </summary>
		public SplashControlDesigner()
		{
			this.dvDisplaySplash = new DesignerVerb("Preview Splash", new EventHandler(HandlePreviewSplashEvent));
			this.dvCancelSplash = new DesignerVerb("Cancel Splash", new EventHandler(HandleCancelSplashEvent));
			this.dvCancelSplash.Enabled = false;
			DesignerVerb[] dvarray = new DesignerVerb[] { this.dvDisplaySplash, this.dvCancelSplash}; 
			this.dvcVerbs = new DesignerVerbCollection(dvarray);
		}

		/// <summary>
		/// Returns the designer verbs collection.
		/// </summary>
		public override DesignerVerbCollection Verbs 
		{
			get 
			{
				return this.dvcVerbs;	
			}
		}

		/// <summary>
		/// Overrides initialize.  Here we add an event handler to the selection service.
		/// Notice that we are very careful not to assume that the selection service is
		/// available.  It is entirely optional that a service is available and you should
		/// always degrade gracefully if a service could not be found.
		/// </summary>
		public override void Initialize(IComponent component) 
		{
			base.Initialize(component);
			SplashControl splashControl = this.Component as SplashControl;
			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			Form hostform = idh.RootComponent as Form;
			if(hostform == null)
				throw( new ApplicationException("A SplashControl host must be a top-level form."));
			splashControl.HostForm = hostform;

			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(iccs != null)
				iccs.ComponentChanged += new ComponentChangedEventHandler(this.IComponentChangeService_ComponentChanged);

			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if(iss != null)
				iss.SelectionChanged += new EventHandler(this.ISelectionService_SelectionChanged);
		}

		protected override void Dispose(bool bdisposing)
		{
			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(iccs != null)
				iccs.ComponentChanged -= new ComponentChangedEventHandler(this.IComponentChangeService_ComponentChanged);

			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if(iss != null)
				iss.SelectionChanged -= new EventHandler(this.ISelectionService_SelectionChanged);

			base.Dispose(bdisposing);
		}
	

		private void IComponentChangeService_ComponentChanged(Object sender, ComponentChangedEventArgs e)
		{
			SplashControl control = this.Component as SplashControl;
			if(e.Component == this.Component)
			{
				if(control.UseCustomSplashPanel == true)
					this.dvDisplaySplash.Enabled = false;
				else
					this.dvDisplaySplash.Enabled = true;
			}
		}

		private void ISelectionService_SelectionChanged(Object sender, EventArgs e)
		{
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if(iss != null)
			{
				SplashControl control = this.Component as SplashControl;
				if(control.UseCustomSplashPanel == true)
					this.dvDisplaySplash.Enabled = false;
				else
					this.dvDisplaySplash.Enabled = true;
			}
		}		

		/// <summary>
		/// Implementation for the verb PreviewSplash.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The event data.</param>
		private void HandlePreviewSplashEvent(object sender, EventArgs e) 
		{
			SplashControl control = this.Component as SplashControl;
			control.SplashClosed += new EventHandler(this.HandleSplashClosedEvent);
			if(control.UseCustomSplashPanel == true && control.CustomSplashPanel != null)
			{
				this.customPanelLocation = control.CustomSplashPanel.Location;
				this.customSplashUsed = true;
			}
			control.ShowSplash(false);
			this.dvDisplaySplash.Enabled = false;
			this.dvCancelSplash.Enabled = true;
		}	


		/// <summary>
		/// Handler for the CancelSplash verb.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event data.</param>
		private void HandleCancelSplashEvent(object sender, EventArgs e) 
		{
			if(this.dvDisplaySplash.Enabled == false)
			{
				SplashControl control = this.Component as SplashControl;
				control.HideSplash();
			}
			this.dvCancelSplash.Enabled = false;
		}	

		/// <summary>
		/// Handler for the SplashClosed event.
		/// </summary>
		/// <param name="sender">The splash form</param>
		/// <param name="e">Event data.</param>
		private void HandleSplashClosedEvent(object sender, EventArgs e)
		{
			if(this.customSplashUsed == true)
			{
				this.customSplashUsed = false;
				SplashControl control = this.Component as SplashControl;
				if(control.HostForm != null)
				{
					if(control.HostForm.Controls.Contains(control.CustomSplashPanel) == false)
					{
						control.HostForm.Controls.Add(control.CustomSplashPanel);
						control.CustomSplashPanel.Location = this.customPanelLocation;
					}
				}

			}
			// Enable the verb
			this.dvCancelSplash.Enabled = false;
			this.dvDisplaySplash.Enabled = true;
		}
	
		/// <summary>
		/// Overrides PreFilterProperties and removes the properties 
		/// visible in the designer for the SplashControl.
		/// </summary>
		/// <param name="properties"></param>
		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);
		
			String[] strcolln = new String[25];
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
			strcolln[12] = "DynamicProperties";
			strcolln[13] = "DataBindings";
			strcolln[14] = "Size";
			strcolln[15] = "Location";
			strcolln[16] = "AccessibleName";
			strcolln[17] = "AccessibleDescription";
			strcolln[18] = "AccessibleRole";
			strcolln[19] = "TabIndex";
			strcolln[20] = "Text";
			strcolln[21] = "Visible";
			strcolln[22] = "Font";
			strcolln[23] = "ForeColor";
			strcolln[24] = "BackColor";
			RemovePropertyBrowsable(this.Component, strcolln, properties);			
		}

		/// <summary>
		/// Helper function for removing a list of properties.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="strcolln"></param>
		/// <param name="properties"></param>
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

}