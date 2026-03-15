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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Implements a drop-down button control for selecting colors.
	/// </summary>
	/// <remarks>
	/// The ColorPickerButton class is a button-type control that pops-up an instance of the Syncfusion 
	/// <see cref="ColorUIControl"/> when clicked. This class derives from the Windows Forms Button class and 
	/// hence can be used in place of a regular button control. The ColorPickerButton's ColorUIControl component 
	/// can be accessed through the <see cref="ColorPickerButton.ColorUI"/> property.
	/// <seealso cref="ColorUIControl"/>
	/// </remarks>
	[System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.ColorPickerButton), "ToolboxIcons.ColorPickerButton.bmp")]
	[Description("Represents a drop-down button control for selecting colors.")]
	public class ColorPickerButton: ButtonAdv
	{
		private PopupControlContainer dropdownContainer = null;
		private ColorUIControl colorUI;
		private bool bSelectedAsBackcolor = false;
		private bool bSelectedAsText = false;
		
		// Drop-down control attributes.
		private static Size szDefault = new Size(208, 230);

		/// <summary>
		/// The ColorSelected event occurs when a color is selected from the drop-down <see cref="ColorUIControl"/>.
		/// </summary>
		[
		Description("The ColorSelected event occurs when a color is selected from the drop-down ColorUIControl." ),
		]
		public event System.EventHandler ColorSelected;

		/// <summary>
		/// Returns a reference to the drop-down ColorUIControl.
		/// </summary>
		/// <value>A reference to the <see cref="ColorUIControl"/> instance.</value>
		[
		Description("Returns a reference to the drop-down ColorUIControl."),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public ColorUIControl ColorUI
		{
			get { return this.colorUI; }
		}

		/// <summary>
		/// Gets or sets the size of the drop-down <see cref="ColorUIControl"/>.
		/// </summary>
		/// <value>A Size value.</value>
		[
		Description("Gets or sets the size of the drop-down ColorUIControl."),
		Category("Layout")
		]
		public Size ColorUISize
		{
			get { return this.dropdownContainer.Size; }
			set 
			{ 
				if(value == Size.Empty)
					value = ColorPickerButton.szDefault;
				this.dropdownContainer.Size = value;
			}
		}

		/// <summary>
		/// Indicates whether the drop-down <see cref="ColorUIControl"/> is visible.
		/// </summary>
		/// <value>True if the drop-down is visible; False otherwise. By default, it is False.</value>
		[
		Description("Specifies whether the drop-down ColorUIControl is visible."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
		public virtual bool DroppedDown
		{
			get
			{
				return this.DropDownContainer != null && this.DropDownContainer.IsShowing();
			}
			set
			{
				if(this.DroppedDown == value || this.DropDownContainer == null)
					return;

				if (this.dropdownContainer != null) 
				{
					if(value)
						this.dropdownContainer.ShowPopup(Point.Empty);
					else
						this.dropdownContainer.HidePopup(PopupCloseType.Canceled);
				}
			}
		}
        [DefaultValue(false)]
        public override bool EnableTouchMode
        {
            get
            {
                return base.EnableTouchMode;
            }
            set
            {
                base.EnableTouchMode = value;
                this.ColorUI.EnableTouchMode = value;
            }
        }
       
        
		/// <summary>
		/// Indicates whether the <see cref="ColorPickerButton.SelectedColor"/> is set as the button backcolor.
		/// </summary>
		/// <value>True if the SelectedColor is used; False otherwise. By default, it is False.</value>
		[
		Category("Appearance"),
		Description("Specifies whether the SelectedColor is set as the button backcolor."),
		DefaultValue(false)
		]
		public bool SelectedAsBackcolor
		{
			get { return this.bSelectedAsBackcolor; }
			set 
			{
				this.bSelectedAsBackcolor = value; 
				if(value == false)
					this.BackColor = SystemColors.Control;
				else 
					this.BackColor = this.SelectedColor;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="ColorPickerButton.SelectedColor"/> is set as the button text value.
		/// </summary>
		/// <value>True if the SelectedColor is used; false otherwise. By default, it is False.</value> 
		[
		Category("Appearance"),
		Description("Specifies whether the button text will reflect the name of the SelectedColor."),
		DefaultValue(false)
		]
		public bool SelectedAsText
		{
			get { return this.bSelectedAsText; }
			set 
			{ 
				this.bSelectedAsText = value; 
				if(value == false)
					this.Text = this.Name;
				else
					this.Text = this.SelectedColor.Name;
			}
		}
		/// <summary>
		/// Gets or sets the current selected color.
		/// </summary>
		/// <value>A color value.</value>
		[
		Category("Color Selection"),
		Description("Specifies the current selected color.")
		]
		public Color SelectedColor
		{
			get	{ return this.colorUI.SelectedColor; }
			set	
			{
				this.colorUI.SelectedColor = value;
				if(this.bSelectedAsBackcolor == true)
					this.BackColor = value;
				if(this.bSelectedAsText == true)
					this.Text = value.Name;
			}
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedColor()
		{
			if(this.colorUI.SelectedColor == Color.Empty)
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Resets the <see cref="ColorPickerButton.SelectedColor"/> property to its default value.
		/// </summary>
		public virtual void ResetSelectedColor()
		{
			this.colorUI.ResetSelectedColor();
		}

		/// <summary>
		/// Gets or sets the color tabpages to be displayed by the drop-down <see cref="ColorUIControl"/>.
		/// </summary>
		/// <value>A <see cref="ColorUIGroups"/> value. The default is ColorUIGroups.All.</value>
		[
		Category("Color Selection"),
		Description("Specifies the color tabpages to be displayed by the drop-down ColorUIControl."),
		DefaultValue(ColorUIGroups.All)
		]
		public ColorUIGroups ColorGroups
		{
			get { return this.colorUI.ColorGroups; }

			set	{ this.colorUI.ColorGroups = value; }			
		}

		/// <summary>
		/// Gets or sets the tab associated with this colorgroup as the selected tab in the drop-down <see cref="ColorUIControl"/>.
		/// </summary>
		/// <value>A <see cref="ColorUISelectedGroup"/> value.</value>
		[
		Category("Color Selection"),
		Description("Makes the tab associated with this colorgroup the selected tab in the drop-down ColorUIControl."),
        DefaultValue(typeof(ColorUISelectedGroup), "None")
		]
		public ColorUISelectedGroup SelectedColorGroup
		{
			get { return this.colorUI.SelectedColorGroup; }
			set	{ this.colorUI.SelectedColorGroup = value; }
		}

		/// <summary>
		/// Gets or sets the text displayed on the drop-down <see cref="ColorUIControl"/>'s custom colors tab.
		/// </summary>
		/// <value>A String value.</value>
		[
		Category("Appearance"),
		Description("The text displayed on the drop-down ColorUIControl's custom colors tab.")
		]
		public String CustomTabName
		{
			get { return this.colorUI.CustomTabName; }
			set { this.colorUI.CustomTabName = value; }
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeCustomTabName()
		{
			if(this.colorUI.CustomTabName.Equals(SR.GetString(SR.ColorEditorPaletteTab,this)))
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Resets the <see cref="ColorPickerButton.CustomTabName"/> property to its default value.
		/// </summary>
		public virtual void ResetCustomTabName()
		{
			this.colorUI.ResetCustomTabName();
		}

		/// <summary>
		/// Gets or sets the text displayed on the drop-down <see cref="ColorUIControl"/>'s standard colors tab.
		/// </summary>
		/// <value>A String value.</value>
		[
		Category("Appearance"),
		Description("The text displayed on the drop-down ColorUIControl's standard colors tab.")
		]
		public String StandardTabName
		{
			get { return this.colorUI.StandardTabName; }
			set	{ this.colorUI.StandardTabName = value;	}
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeStandardTabName()
		{
			if(this.colorUI.StandardTabName.Equals(SR.GetString(SR.ColorEditorStandardTab,this)))
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Resets the <see cref="ColorPickerButton.StandardTabName"/> property to its default value.
		/// </summary>
		public virtual void ResetStandardTabName()
		{
			this.colorUI.ResetStandardTabName();
		}

		/// <summary>
		/// Gets or sets the text displayed on the drop-down <see cref="ColorUIControl"/>'s system colors tab.
		/// </summary>
		/// <value>A String value.</value>
		[
		Category("Appearance"),
		Description("The text displayed on the drop-down ColorUIControl's system colors tab.")
		]
		public String SystemTabName
		{
			get { return this.colorUI.SystemTabName; }
			set	{ this.colorUI.SystemTabName = value; }				
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSystemTabName()
		{
			if(this.colorUI.SystemTabName.Equals(SR.GetString(SR.ColorEditorSystemTab,this)))
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Resets the <see cref="ColorPickerButton.SystemTabName"/> property to its default value. 
		/// </summary>
		public virtual void ResetSystemTabName()
		{
			this.colorUI.ResetSystemTabName();
		}	

		/// <summary>
		/// Returns a reference to the <see cref="PopupControlContainer"/> that will contain the <see cref="ColorUIControl"/>.
		/// </summary>
		/// <value>A reference to the <see cref="PopupControlContainer"/> instance.</value>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public PopupControlContainer DropDownContainer
		{
			get
			{
				EnsureDropDownContainer();
				return this.dropdownContainer;
			}
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ColorPickerButton"/> class.
		/// </summary>
		public ColorPickerButton()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			   new Syncfusion.Core.Licensing.LicensedComponent(typeof(ColorPickerButton));
			}
			finally
			{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			this.EnsureDropDownContainer();
		}		

		/// <summary>
		/// Overridden. See <see cref="M:System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.dropdownContainer != null)
				{
					this.dropdownContainer.BeforePopup -= new CancelEventHandler(this.DropdownContainerBeforePopup);
					this.dropdownContainer.Popup -= new EventHandler(this.DropdownContainerPopup);
					this.dropdownContainer.CloseUp -= new PopupClosedEventHandler(this.DropdownContainerCloseUp);

					this.dropdownContainer.Dispose();
					this.dropdownContainer = null;
				}
			}
			base.Dispose(disposing);
		}
		/// <summary>
		/// Sets the font to ColorUI control
		/// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            colorUI.Font = this.Font;
        }
		/// <summary>
		/// Creates a default instance of a <see cref="PopupControlContainer"/>.
		/// </summary>
		/// <returns>The <see cref="PopupControlContainer"/> instance.</returns>
		protected virtual PopupControlContainer CreateDropdownContainer()
		{
			PopupControlContainer container = new PopupControlContainer();
			container.FakeFocus = false;
			return container;
		}

		private void EnsureDropDownContainer()
		{
			if (this.dropdownContainer == null)
			{
				this.dropdownContainer = CreateDropdownContainer();
				this.dropdownContainer.Size = ColorPickerButton.szDefault;
				this.dropdownContainer.ParentControl = this;
                    
				this.dropdownContainer.BeforePopup += new CancelEventHandler(this.DropdownContainerBeforePopup);
				this.dropdownContainer.Popup += new EventHandler(this.DropdownContainerPopup);
				this.dropdownContainer.CloseUp += new PopupClosedEventHandler(this.DropdownContainerCloseUp);
                    
				InitializeDropdownContainer();
			}
		}

		/// <summary>
		/// Initializes the drop-down container. Adds a <see cref="ColorUIControl"/>.
		/// </summary>
		protected virtual void InitializeDropdownContainer()
		{
			if (this.dropdownContainer != null)
			{
				colorUI = new ColorUIControl();
				colorUI.ColorSelected += new EventHandler(this.OnCUIColorSelected);
				colorUI.Dock = DockStyle.Fill;
				colorUI.Visible = true;
                colorUI.Font = this.Font;
				this.dropdownContainer.Controls.Add(colorUI);
			}
		}

		void OnCUIColorSelected(object sender, EventArgs e)
		{
			this.SelectedColor = this.colorUI.SelectedColor;
			DroppedDown = false;
			if(this.ColorSelected != null)
			{
				this.ColorSelected(this, EventArgs.Empty);
			}
		}

		// Called after the drop-down has been made visible. Sets the focus on the ColorUIControl.
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DropdownContainerPopup(object sender, EventArgs e)
		{
			colorUI.Focus();
		}


		// Called before the drop-down has been made visible. 		
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DropdownContainerBeforePopup(object sender, CancelEventArgs e)
		{
			if(this.dropdownContainer.PopupHost != null)
				this.dropdownContainer.PopupHost.FormBorderStyle = FormBorderStyle.None;
			
			this.colorUI.Start(this.ColorUI.SelectedColor);
		}


		/// Called when the drop-down has been closed.		
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DropdownContainerCloseUp(object sender, PopupClosedEventArgs e)  
		{
			if (e.PopupCloseType == PopupCloseType.Done)
			{
				this.SelectedColor = this.colorUI.SelectedColor;
				DroppedDown = false;
			}
			this.Focus();
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnKeyDown(KeyEventArgs e)  
		{
			bool bAlt = (Control.ModifierKeys & Keys.Alt) != Keys.None;

			if (e.Handled)
				return;

			switch (e.KeyCode)
			{
				case Keys.Down:
					if (bAlt)
					{
						e.Handled = true;
						DroppedDown = !DroppedDown;
						return;
					}
					break;
				case Keys.F4:
					if (Control.ModifierKeys == Keys.None)
					{
						e.Handled = true;
						DroppedDown = !DroppedDown;
					}
					break;
			}

			base.OnKeyDown(e);
		}				

		/// <summary>
		/// Overridden. See <see cref="M:System.Windows.Forms.Control.OnClick"/>.
		/// </summary>
		protected override void OnClick(EventArgs e)
		{
			DroppedDown = !DroppedDown;
			base.OnClick(e);
		}
	}
}
