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

namespace Syncfusion.Windows.Forms.Design
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
	using System.Reflection;

	/// <summary>
	/// Generic <see cref="VisualStyle"/> enumeration value filter class.
	/// </summary>
	public class VisualStyleEnumFilter:
		EnumConverter
	{
		#region Data members

		/// <summary>
		/// Enumeration values to be skipped.
		/// </summary>
		protected VisualStyle[] m_avsValuesToSkip;

		#endregion Data members

		#region Construction

		protected VisualStyleEnumFilter( Type type )
			: base( type )
		{
		}

		public VisualStyleEnumFilter( Type type, VisualStyle[] valuesToSkip )
			: base( type )
		{
			m_avsValuesToSkip = valuesToSkip;
		}

		#endregion Construction

		#region Overrides
		
		public override StandardValuesCollection GetStandardValues( ITypeDescriptorContext context )
		{
			StandardValuesCollection stdValues = base.GetStandardValues( context );
			VisualStyle[] filteredValues = new VisualStyle[ stdValues.Count - m_avsValuesToSkip.Length ];

			for( int i = 0, j = 0; i < stdValues.Count; ++i )
			{
				VisualStyle vsStdVal = (VisualStyle)stdValues[i];

				if( Array.IndexOf( m_avsValuesToSkip, vsStdVal ) < 0 )	// Not found
				{
					filteredValues.SetValue( vsStdVal, j++ );
				}
			}

			return new StandardValuesCollection( filteredValues );
		}

		#endregion Overrides
	}

	/// <summary>
	/// Default <see cref="VisualStyle"/> enumeration value filter class.
	/// </summary>
	/// <remarks>Skips <see cref="VisualStyle.Office2007Outlook"/> value.</remarks>
	public class DefaultVisualStyleEnumFilter:
		VisualStyleEnumFilter
	{
		#region Data members

		/// <summary>
		/// Default values to be skipped.
		/// </summary>
		private static readonly VisualStyle[] r_avsValuesToSkip = new VisualStyle[]{ VisualStyle.Office2007Outlook };

		#endregion Data members

		#region Construction

		public DefaultVisualStyleEnumFilter( Type type )
			: base( type, (VisualStyle[])r_avsValuesToSkip.Clone() )
		{
		}

		public DefaultVisualStyleEnumFilter( Type type, VisualStyle[] valuesToSkip )
			: this( type )
		{
			if( valuesToSkip.Length > 0 )
			{
				VisualStyle[] avsValuesToSkip = new VisualStyle[ m_avsValuesToSkip.Length + valuesToSkip.Length ];
				
				m_avsValuesToSkip.CopyTo( avsValuesToSkip, 0 );
				valuesToSkip.CopyTo( avsValuesToSkip, m_avsValuesToSkip.Length );

				m_avsValuesToSkip = avsValuesToSkip;
			}
		}

		#endregion Construction
	}
	
	/// <summary>
	/// EnumFlagsEditor implements a UITypeEditor for modifying a enum value that
	/// has been marked with the FlagsAttribute.
	/// </summary>
	/// <remarks>The editor lets you check and uncheck
	/// individual flags in a dropdown CheckedListBox. A None button allows to reset all
	/// flags at once.
	/// </remarks>
	[Syncfusion.Documentation.DocumentationExclude()]
    public sealed class EnumFlagsEditor: UITypeEditor 
    {
        private IWindowsFormsEditorService edSvc = null;

		/// <override/>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)  
        {
             return UITypeEditorEditStyle.DropDown;
        }

		/// <override/>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)  
        {
            EnumFlagsUI ef;
            if (provider == null)
                return value;

            this.edSvc = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
            if (this.edSvc == null) 
                return value;
            
            ef = new EnumFlagsUI(this);
            ef.EditValue = value;
            this.edSvc.DropDownControl(ef);

			if (ef == null || ef.IsDisposed)
				return value;

			return Enum.ToObject(value.GetType(), ef.EditValue);
			
			// I have to explicitly call PropertyDescriptor.SetValue to 
			// change the value of the enum in the edited component itsself.
			
			//context.PropertyDescriptor.SetValue(context.Instance, ef.EditValue);
			
			// An alternative to calling SetValue is to derive a specialized 
			// class from EnumFlagsEditor for your enum and then cast to that
			// enum, for example
			// value = (MergeCellBehavior) ef.EditValue;
            
			//return ef.EditValue;
        }

		[Syncfusion.Documentation.DocumentationExclude()]
		private class EnumFlagsUI: Form
		{
			// Fields
			private EnumFlagsEditor mainEditor;
			private CheckedListBox listBox;
			private Button button; 
			private object editValue;
			private object originalValue;
			private object emptyValue;

			// ctor
			public EnumFlagsUI(EnumFlagsEditor editor)
			{
				this.mainEditor = editor;
				this.StartPosition = FormStartPosition.WindowsDefaultBounds;
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.FormBorderStyle = FormBorderStyle.None;
				this.TopLevel = false;
				this.ShowInTaskbar = false;
				this.TopMost = true;
				this.listBox = new CheckedListBox();
				this.listBox.Dock = DockStyle.Top;
				this.listBox.CheckOnClick = true;
				this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
				this.listBox.ItemCheck += new ItemCheckEventHandler(listBoxItemCheckEventHandler);
				this.button = new Button();
				this.button.Text = "Reset";
				this.button.Dock = DockStyle.Bottom;
				this.button.Height = this.listBox.ItemHeight;
				this.button.Visible = false;
				this.button.Click += new EventHandler(buttonOnClicked);
				this.Size = new Size(0, this.listBox.ItemHeight*7);
				this.listBox.Size = this.Size;
				this.Controls.Add(this.listBox);
				this.Controls.Add(this.button);
			}

			// Methods
			protected override bool ProcessDialogKey(Keys keyData)  
			{
				if (keyData == Keys.Enter) 
				{
					this.mainEditor.edSvc.CloseDropDown();
					return true;
				}

				if (keyData == Keys.Escape) 
				{
					this.editValue = this.originalValue;
					this.mainEditor.edSvc.CloseDropDown();
					return true;
				}

				return base.ProcessDialogKey(keyData);
			}

			protected void listBoxItemCheckEventHandler(object sender, ItemCheckEventArgs e)  
			{
				string s = this.listBox.Items[e.Index].ToString();
				if (e.NewValue == CheckState.Checked) 
					this.EnableOption(s);
				else
					this.DisableOption(s);
			}

			protected void buttonOnClicked(object sender, EventArgs e) 
			{
				int count = this.listBox.Items.Count;
				for (int i = 0; i < count; i++)
				{
					this.listBox.SetItemChecked(i, false);
				}
				ConvertFromInt(0);
				this.mainEditor.edSvc.CloseDropDown();
			}

			protected int GetOptionValue(string optionName)  
			{
				Type type = this.EditValue.GetType();
				return ((int) type.GetField(optionName).GetValue(this.EditValue));
			}

			protected void ConvertFromInt(int value)  
			{
				Type type = this.EditValue.GetType();
                try
                {
                    this.editValue = Enum.Parse(type, value.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                }
			}

			protected void DisableOption(string optionName)  
			{
				this.ConvertFromInt((((int) this.editValue) & (~this.GetOptionValue(optionName))));
			}

			protected void EnableOption(string optionName)  
			{
				this.ConvertFromInt((((int) this.editValue) | this.GetOptionValue(optionName)));
			}

			protected bool IsOptionEnabled(string optionName)  
			{
				int n = ((int) this.editValue);
				return ((n & this.GetOptionValue(optionName)) != 0);
			}



			// Properties
			public object EditValue
			{
				get 
				{
					return this.editValue;
				}
				set 
				{
					if (this.editValue != value) 
					{
						int btnHeight = 0;
						string btnText = "";
						object obj = value;
						this.editValue = value;
						this.originalValue = obj;
						this.button.Visible = false;
						this.listBox.Items.Clear();
						Type type = this.editValue.GetType();
                        for (int n = 0; n < type.GetFields().Length; n++)
						{
							FieldInfo fieldInfo = type.GetFields()[n];
                            if (fieldInfo.MemberType != MemberTypes.Field || (!fieldInfo.FieldType.Name.Equals(type.Name)))
								continue;

							object[] attributes = fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.BrowsableAttribute), false);
							if (attributes != null && attributes.Length > 0)
							{
								BrowsableAttribute browsable = (BrowsableAttribute) attributes[0];
								if (browsable != null && !browsable.Browsable)
									continue;
							}

							if (GetOptionValue(fieldInfo.Name) == 0)
							{
								Graphics g = this.CreateGraphics();
								btnText = fieldInfo.Name;
								Size t = g.MeasureString(btnText, this.Font).ToSize();
								btnHeight = t.Height+6;
								g.Dispose();
								emptyValue = fieldInfo.GetValue(this.editValue);
							}
							else if (this.IsOptionEnabled(fieldInfo.Name))
								this.listBox.Items.Add(fieldInfo.Name, CheckState.Checked);
							else
								this.listBox.Items.Add(fieldInfo.Name, CheckState.Unchecked);
						}

						this.listBox.Sorted = true;
                    
						int c = Math.Min(this.listBox.Items.Count, 15);
						this.Size = new Size(this.Size.Width, (this.listBox.ItemHeight*c)+btnHeight+4);
						if (btnHeight > 0)
						{
							this.listBox.Size = new Size(0, this.Size.Height-btnHeight);
							this.button.Size = new Size(0, btnHeight);
							this.button.Dock = DockStyle.Bottom;
							this.button.Text = btnText;
							this.button.Visible = true;
						}                        
					}
				}
			}
		}
    }
}
