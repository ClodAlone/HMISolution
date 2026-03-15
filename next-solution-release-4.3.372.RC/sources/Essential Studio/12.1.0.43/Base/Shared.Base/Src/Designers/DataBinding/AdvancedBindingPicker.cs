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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Design;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Design
{
	class AdvancedBindingPicker : 
		Form
	{
        
		// Fields
		private ITypeDescriptorContext context;
		private AdvancedBindingObject value;
		private Container components;
		private Label label1;
		private PropertyGrid propertyGrid1;
		private CheckBox checkBox1;
		private Button button1;
		private static readonly string HELP_KEYWORD;
        
		// Constructors
        
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="T:System.Windows.Forms.Design.AdvancedBindingPicker" /> class.</para>
		/// </summary>
		/// <param name="context">A type descriptor context that can provide context information.</param>
		public AdvancedBindingPicker(ITypeDescriptorContext context)
		{
			this.context = context;
			this.ShowInTaskbar = false;
			this.InitializeComponent();
		}
        
		static AdvancedBindingPicker()
		{
			AdvancedBindingPicker.HELP_KEYWORD = "VS.PropertyBrowser.DataBindings.Advanced";
		}
        
        
		// Events
        
		// Methods
        
		public void End()
		{
			this.propertyGrid1.SelectedObjects = null;
			
		}
        
        
		public AdvancedBindingObject Value 
		{ 
			get
			{
				return this.value;
			}
			set
			{
				//object[] args0;
				this.value = value;
				// Syncfusion Change:
				//args0 = new object[1];
				// if ((value.Bindings.Control.Site != null)) goto IL_002f;
				// goto IL_0044;
				// // 'goto IL_002f' Conditional Stack: ""
				// IL_002f: ;
				//IL_0044: ;
				//args0[0] = ((value.Bindings.Control.Site != null) ? value : ((AdvancedBindingObject)(""))).Bindings.Control.Site.Name;
				//this.Text = SR.GetString("DataGridAdvancedBindingString", args0);
				this.Text = "DataGridAdvancedBindingString";
				value.ShowAll = (this.checkBox1.CheckState == CheckState.Checked);
				this.propertyGrid1.SelectedObject = value;
			}
		}
        
		private void CheckBox1_CheckedChanged(object sender, EventArgs e)
		{
			this.Value = this.value;
		}
        
        
		private void OnHelpRequested(object sender, HelpEventArgs e)
		{
			IServiceProvider serviceProvider;
			IHelpService helpService;

			serviceProvider = this.context;
			if (serviceProvider != null) 
			{
				helpService = (IHelpService) serviceProvider.GetService(typeof(IHelpService));
				if (helpService != null)
					helpService.ShowHelpFromKeyword(AdvancedBindingPicker.HELP_KEYWORD);
			}
		}
        
        
		private void InitializeComponent()
		{
			ResourceManager resourceManager0;
			Control[] controls1;
			resourceManager0 = new ResourceManager(typeof(Syncfusion.Windows.Forms.Design.AdvancedBindingPicker));
			this.components = new Container();
			this.checkBox1 = ((CheckBox)(new CheckBox()));
			this.label1 = ((Label)(new Label()));
			this.button1 = ((Button)(new Button()));
			this.propertyGrid1 = ((PropertyGrid)(new PropertyGrid()));
			this.checkBox1.CheckState = CheckState.Checked;
			this.checkBox1.CheckedChanged += new EventHandler(this.CheckBox1_CheckedChanged);
			this.checkBox1.AccessibleDescription = ((string)(resourceManager0.GetObject("checkBox1.AccessibleDescription")));
			this.checkBox1.AccessibleName = ((string)(resourceManager0.GetObject("checkBox1.AccessibleName")));
			this.checkBox1.Anchor = ((AnchorStyles)(resourceManager0.GetObject("checkBox1.Anchor")));
			this.checkBox1.CheckAlign = ((ContentAlignment)(resourceManager0.GetObject("checkBox1.CheckAlign")));
			this.checkBox1.Cursor = ((Cursor)(resourceManager0.GetObject("checkBox1.Cursor")));
			this.checkBox1.FlatStyle = ((FlatStyle)(resourceManager0.GetObject("checkBox1.FlatStyle")));
			this.checkBox1.ImeMode = ((ImeMode)(resourceManager0.GetObject("checkBox1.ImeMode")));
			this.checkBox1.Location = ((Point)(resourceManager0.GetObject("checkBox1.Location")));
			this.checkBox1.Size = ((Size)(resourceManager0.GetObject("checkBox1.Size")));
			this.checkBox1.TabIndex = ((int)(resourceManager0.GetObject("checkBox1.TabIndex")));
			//this.checkBox1.Text = SR.GetString("DataGridShowAllString");
			this.checkBox1.Text = "DataGridShowAllString";
			this.checkBox1.TextAlign = ((ContentAlignment)(resourceManager0.GetObject("checkBox1.TextAlign")));
			this.label1.AccessibleDescription = ((string)(resourceManager0.GetObject("label1.AccessibleDescription")));
			this.label1.AccessibleName = ((string)(resourceManager0.GetObject("label1.AccessibleName")));
			this.label1.Anchor = ((AnchorStyles)(resourceManager0.GetObject("label1.Anchor")));
			this.label1.Cursor = ((Cursor)(resourceManager0.GetObject("label1.Cursor")));
			this.label1.ImeMode = ((ImeMode)(resourceManager0.GetObject("label1.ImeMode")));
			this.label1.Location = ((Point)(resourceManager0.GetObject("label1.Location")));
			this.label1.Size = ((Size)(resourceManager0.GetObject("label1.Size")));
			this.label1.Text = resourceManager0.GetString("label1.Text");
			this.label1.TextAlign = ((ContentAlignment)(resourceManager0.GetObject("label1.TextAlign")));
			this.button1.DialogResult = DialogResult.OK;
			this.button1.AccessibleDescription = ((string)(resourceManager0.GetObject("button1.AccessibleDescription")));
			this.button1.AccessibleName = ((string)(resourceManager0.GetObject("button1.AccessibleName")));
			this.button1.Anchor = ((AnchorStyles)(resourceManager0.GetObject("button1.Anchor")));
			this.button1.Cursor = ((Cursor)(resourceManager0.GetObject("button1.Cursor")));
			this.button1.FlatStyle = ((FlatStyle)(resourceManager0.GetObject("button1.FlatStyle")));
			this.button1.ImeMode = ((ImeMode)(resourceManager0.GetObject("button1.ImeMode")));
			this.button1.Location = ((Point)(resourceManager0.GetObject("button1.Location")));
			this.button1.Size = ((Size)(resourceManager0.GetObject("button1.Size")));
			this.button1.TabIndex = ((int)(resourceManager0.GetObject("button1.TabIndex")));
			this.button1.Text = resourceManager0.GetString("button1.Text");
			this.button1.TextAlign = ((ContentAlignment)(resourceManager0.GetObject("button1.TextAlign")));
			this.propertyGrid1.PropertySort = PropertySort.Alphabetical;
			this.propertyGrid1.CommandsVisibleIfAvailable = false;
			this.propertyGrid1.HelpVisible = false;
			this.propertyGrid1.ToolbarVisible = false;
			this.propertyGrid1.AccessibleDescription = ((string)(resourceManager0.GetObject("propertyGrid1.AccessibleDescription")));
			this.propertyGrid1.AccessibleName = ((string)(resourceManager0.GetObject("propertyGrid1.AccessibleName")));
			this.propertyGrid1.Anchor = ((AnchorStyles)(resourceManager0.GetObject("propertyGrid1.Anchor")));
			this.propertyGrid1.Cursor = ((Cursor)(resourceManager0.GetObject("propertyGrid1.Cursor")));
			this.propertyGrid1.ImeMode = ((ImeMode)(resourceManager0.GetObject("propertyGrid1.ImeMode")));
			this.propertyGrid1.Location = ((Point)(resourceManager0.GetObject("propertyGrid1.Location")));
			this.propertyGrid1.Size = ((Size)(resourceManager0.GetObject("propertyGrid1.Size")));
			this.propertyGrid1.TabIndex = ((int)(resourceManager0.GetObject("propertyGrid1.TabIndex")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.AcceptButton = this.button1;
			this.CancelButton = this.button1;
			this.HelpRequested += new HelpEventHandler(this.OnHelpRequested);
			this.AccessibleDescription = ((string)(resourceManager0.GetObject("$this.AccessibleDescription")));
			this.AccessibleName = ((string)(resourceManager0.GetObject("$this.AccessibleName")));
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			this.AutoScaleBaseSize = ((Size)(resourceManager0.GetObject("$this.AutoScaleBaseSize")));
#endif
			this.ClientSize = ((Size)(resourceManager0.GetObject("$this.ClientSize")));
			this.ControlBox = false;
			this.Cursor = ((Cursor)(resourceManager0.GetObject("$this.Cursor")));
			this.Icon = ((Icon)(resourceManager0.GetObject("$this.Icon")));
			this.ImeMode = ((ImeMode)(resourceManager0.GetObject("$this.ImeMode")));
			this.MinimumSize = ((Size)(resourceManager0.GetObject("$this.MinimumSize")));
			this.StartPosition = ((FormStartPosition)(resourceManager0.GetObject("$this.StartPosition")));
			this.Text = resourceManager0.GetString("$this.Text");
			controls1 = new Control[4];
			controls1[0] = this.label1;
			controls1[1] = this.propertyGrid1;
			controls1[2] = this.checkBox1;
			controls1[3] = this.button1;
			this.Controls.AddRange(controls1);
		}
        
	}
}

