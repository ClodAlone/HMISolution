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
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Documentation;
using Syncfusion.Runtime.InteropServices;
using System.Drawing;


namespace Syncfusion.Windows.Forms.Tools
{
	public enum ThemedComboBoxStyles
	{
		Default,
		Office2007
	}

	[ToolboxItem( false )]
	[DocumentationExclude]
	public class ThemedComboBox:
		ComboBoxEx,
		ISupportOffice2007Theme,
        IVisualStyle 
	{
		#region Data

		/// <summary>
		/// Office2007 theme.
		/// </summary>
		private Office2007Theme m_office2007Theme = Office2007Theme.Blue;

		/// <summary>
		/// Control's visual style.
		/// </summary>
		private ThemedComboBoxStyles m_visualStyle = ThemedComboBoxStyles.Default;

		/// <summary>
		/// Color table.
		/// </summary>
		private Office12ColorTable m_colorTable = new Office12ColorTable( Office2007Theme.Blue );

		#endregion

		#region Properties

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string vStyle;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return vStyle;
            }
            set
            {
                vStyle = value;

                if (value == "Office2007Blue")
                {
                    VisualStyle = ThemedComboBoxStyles.Office2007;
                    Office2007ColorTheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    VisualStyle = ThemedComboBoxStyles.Office2007;
                    Office2007ColorTheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    VisualStyle = ThemedComboBoxStyles.Office2007;
                    Office2007ColorTheme = Office2007Theme.Black;
                }
                else if (value == "Managed")
                {
                    VisualStyle = ThemedComboBoxStyles.Office2007;
                    Office2007ColorTheme = Office2007Theme.Managed;
                }
                else if (value == "Default")
                    VisualStyle = ThemedComboBoxStyles.Default;
            }
        }
		/// <summary>
		/// Gets or sets Office2007 theme.
		/// </summary>
		[Description( "Office2007 theme." )]
        [Category( "Behavior" )]
		[DefaultValue( Office2007Theme.Blue )]
		public Office2007Theme Office2007ColorTheme
		{
			get
			{
				return m_office2007Theme;
			}
			set
			{
				if( m_office2007Theme != value )
				{
					m_office2007Theme = value;

					if( m_office2007Theme != Office2007Theme.Managed )
					{
						m_colorTable.UpdateColorScheme( m_office2007Theme );
					}
					else
					{
						m_colorTable = Office12ColorTable.ManagedColors;
					}

					Invalidate(true);
				}
			}
		}

		/// <summary>
		/// Gets or sets control's visual style.
		/// </summary>
        [Category( "Behavior" )]
		[Description( "Control's visual style." )]
		[DefaultValue( ThemedComboBoxStyles.Default )]
		public ThemedComboBoxStyles VisualStyle
		{
			get
			{
				return m_visualStyle;
			}
			set
			{
				if( m_visualStyle != value )
				{
					m_visualStyle = value;

					this.IsStyled = ( this.VisualStyle != ThemedComboBoxStyles.Default );
                    Invalidate(true);
				}
			}
		}

		protected override Office12ColorTable ColorTable
		{
			get
			{
				return m_colorTable;
			}
		}

		#endregion

		#region Override

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				m_colorTable = null;
			}

			base.Dispose( disposing );
		}
        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            base.ReleaseListBoxWindowHandle();
        }

        protected override void OnDropDown(EventArgs e)
        {
            base.CreateListBoxWindowHandle();
            base.OnDropDown(e);
        }

		#endregion

		#region ISupportOffice2007Theme Members

		void ISupportOffice2007Theme.EnableOffice2007Style()
		{
			this.VisualStyle = ThemedComboBoxStyles.Office2007;
		}

		#endregion
	}

	/// <summary>
	/// ComboBoxAutoComplete derives from <see cref="ComboBox"/> and embeds an 
	/// <see cref="AutoComplete"/> control to provide auto completion services
	/// for the ComboBox.
	/// </summary>
	[ToolboxItem(true),
	System.Drawing.ToolboxBitmap(typeof(ComboBoxAutoComplete), "ToolboxIcons.ComboBoxAutoComplete.bmp"),
	Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.ComboBoxAutoCompleteDesigner), typeof(System.ComponentModel.Design.IDesigner)),
	Description("Represents a combo box control with an embedded AutoComplete.")
	]
	public class ComboBoxAutoComplete:
		ThemedComboBox
	{
		private AutoComplete autoCompleteControl = new AutoComplete();
		private bool allowNewText = true;
		private bool refreshSelectionProperties = false;//Otherwise use the cached values
		private bool m_bReadOnly = false;
        private ComboBoxAutoCompleteNativeWindow m_nativeWindow = null;

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// Default font style of the control
        /// </summary>
        private static Font FONTSTYLE = default(Font);

        /// <summary>
        /// Font which stored after changed in design
        /// </summary>
        private static Font USERFONTSTYLE = default(Font);

		public ComboBoxAutoComplete()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( ComboBoxAutoComplete ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}

			this.autoCompleteControl.OverrideCombo = true;
			this.autoCompleteControl.ChangeDataManagerPosition = true;
			this.autoCompleteControl.DropDownClosed += new Syncfusion.Windows.Forms.PopupClosedEventHandler( this.HandleAutoCompleteDropDownClosed );
			this.autoCompleteControl.SetAutoComplete( this, Syncfusion.Windows.Forms.Tools.AutoCompleteModes.AutoAppend );
			this.autoCompleteControl.SetAutoCompleteComboUpDown( this, true );
			this.autoCompleteControl.ParentForm = this.Parent;

			base.DropDownWidth = this.Width;
            CTRLSIZE = this.Size;
            FONTSTYLE = this.Font;
            USERFONTSTYLE = FONTSTYLE;
		}

		~ComboBoxAutoComplete()
		{
			Dispose(false);
		}

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            if(this.AutoCompleteSource != AutoCompleteSource.CustomSource)
#endif
                m_nativeWindow = new ComboBoxAutoCompleteNativeWindow(this);
        }

        internal IntPtr TextBox
        {
            get 
            {
                NativeMethods.COMBOBOXINFO cbInfo = new NativeMethods.COMBOBOXINFO();
                cbInfo.cbSize = (uint)Marshal.SizeOf(cbInfo);

                if (NativeMethods.GetComboBoxInfo(this.Handle, ref cbInfo))
                {
                    return cbInfo.hwndItem;
                }

                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the control and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                if( this.autoCompleteControl != null )
                {
                    this.autoCompleteControl.SetAutoComplete(this, Syncfusion.Windows.Forms.Tools.AutoCompleteModes.Disabled);
                    this.autoCompleteControl.DropDownClosed -= new Syncfusion.Windows.Forms.PopupClosedEventHandler(this.HandleAutoCompleteDropDownClosed);
                    this.autoCompleteControl.Dispose();
                    this.autoCompleteControl = null;
                }
                m_nativeWindow = null;
			}
			base.Dispose( disposing );
		}
        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
		[DefaultValue(false)]
        [Category( "Behavior" )]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// Apply Scale for controls
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            if (FONTSTYLE == USERFONTSTYLE)
                this.Font = new Font(FONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor, this.Font.Style, this.Font.Unit, this.Font.GdiCharSet, this.Font.GdiVerticalFont);
            else
                this.Font = new Font(USERFONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor, this.Font.Style, this.Font.Unit, this.Font.GdiCharSet, this.Font.GdiVerticalFont);
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        /// OnFontChanged event
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (!isScaling)
            {
                if (USERFONTSTYLE != this.Font)
                    USERFONTSTYLE = this.Font;
            }
        }
        #endregion

        /// <summary>
        /// Returns the AutoComplete.
        /// </summary>
        [Category( "Behavior" )]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AutoComplete AutoCompleteControl
		{
			get
			{
				return this.autoCompleteControl;
			}
		}

		/// <summary>
		/// Indicates whether user is allowed to enter new text.
		/// </summary>
        [Category( "Behavior" )]
		[DefaultValue(true)]
        [Description("Indicates whether user is allowed to enter new text.")]
		public bool AllowNewText
		{
			get
			{
				return this.allowNewText;
			}

			set
			{
				this.allowNewText = value;
			}
		}
		/// <summary>
		/// Gets or sets the value indicating whether changes can be done.
		/// </summary>
        [Category( "Behavior" )]
		[DefaultValue( false )]
		[Description( "Gets or sets the value indicating whether changes can be done." )]
		public bool ReadOnly
		{
			get
			{
				return m_bReadOnly;
			}
			set
			{
				if( value != m_bReadOnly )
				{
					m_bReadOnly = value;

					NativeMethods.EnumChildWindowsCallBack callBack = new Syncfusion.Runtime.InteropServices.NativeMethods.EnumChildWindowsCallBack( this.EnumChildWindowsCallBack );
                    NativeMethods.EnumChildWindows( base.Handle, callBack, IntPtr.Zero );
				}
			}
		}

		private bool EnumChildWindowsCallBack( IntPtr hWnd, IntPtr lParam )
		{
			if( hWnd != IntPtr.Zero )
			{
				IntPtr readonlyValue = ( m_bReadOnly ) ? new IntPtr( 1 ) : IntPtr.Zero;
				NativeMethods.SendMessage( hWnd, NativeMethods.EM_SETREADONLY, readonlyValue, IntPtr.Zero );
				Invalidate();

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets or sets the ParentForm of the ComboBox.
		/// </summary>
		/// <remarks>
		/// This property needs to be set to be the form on which this ComboBox  control
		/// is placed. This is used by the <see cref="ComboBoxAutoComplete.AutoCompleteControl"/>
		/// control.
		/// </remarks>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Browsable(false)
		]
		public Control ParentForm
		{
			get
			{
				return this.autoCompleteControl.ParentForm;
			}

			set
			{
				this.autoCompleteControl.ParentForm = value;
			}
		}

		/// <summary>
		/// Override the OnDropDown method to show the AutoComplete drop down.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnDropDown(EventArgs e)
		{
			if( !ReadOnly )
			{
				if( !this.autoCompleteControl.IsDropDownShowing() )
				{
					this.autoCompleteControl.SetAutoComplete( this, Syncfusion.Windows.Forms.Tools.AutoCompleteModes.AutoSuggest );

					string currentText = this.Text;

					if( this.autoCompleteControl.GetMatchesCount( String.Empty ) > 0 )
					{
						this.autoCompleteControl.ProcessAutoComplete( String.Empty, currentText );
					}
					else
					{
						this.autoCompleteControl.SetAutoComplete( this, Syncfusion.Windows.Forms.Tools.AutoCompleteModes.AutoAppend );
					}
				}
				else
				{
					this.autoCompleteControl.CloseDropDown();
                    this.Focus();
                    MethodInvoker mi = new MethodInvoker(HideNativeDropDown);
                    this.BeginInvoke(mi);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string GetUnselectedInitialText()
		{
			string unselectedText = String.Empty;
			if(this.SelectionLength > 0)
				unselectedText = this.Text.Substring(0,this.SelectionStart);
			else
				unselectedText = this.Text;

			return unselectedText;

		}

		private void HandleAutoCompleteDropDownClosed(object sender, PopupClosedEventArgs args)
		{
			this.DroppedDown = false;
			this.autoCompleteControl.SetAutoComplete(this, this.AutoCompleteControl.GetAutoComplete());
		}

		/// <summary>
		/// Override WndProc.
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			bool handled = false;

			if( m.Msg == NativeMethods.WM_LBUTTONDOWN || m.Msg == NativeMethods.WM_LBUTTONDBLCLK )
			{
				handled = this.ReadOnly;
			}

			if( !handled )
			{
				if( m.Msg == (NativeMethods.WM_REFLECT + NativeMethods.WM_COMMAND) ) 
				{
					int wparam;
					wparam = ((int)(m.WParam)) >> 0x10;
					if(wparam != 7)
						base.WndProc(ref m);
					else
						this.OnDropDown(EventArgs.Empty);
				}
				else if ( m.Msg == NativeMethods.WM_LBUTTONDOWN || m.Msg == NativeMethods.WM_LBUTTONDBLCLK ) 
				{
                    if( this.DropDownWidth > this.Width )
                    {
                        this.DropDownWidth = this.Width;
                    }

                    if (this.DropDownStyle != ComboBoxStyle.DropDownList)
                    {
                        base.WndProc(ref m);
                        //if (this.autoCompleteControl.IsDropDownShowing() == false && this.DroppedDown)
						//{
							//this.DroppedDown = false;
							//this.Focus();
						//}
                    }
                    else
                    {
                        if (this.autoCompleteControl.IsDropDownShowing() == false)
                        {
                            this.OnDropDown(EventArgs.Empty);
                        }
                        else
                        {
                            this.autoCompleteControl.CloseDropDown();
                        }
                        if (!this.Focused)
                        {
                            this.Focus();
                        }
                    }
				}
                else if (m.Msg == 0x147)
                {
                    if (this.FindForm() != null)
                    {
                        if (!this.FindForm().Disposing)
                            base.WndProc(ref m);
                    }
                    else
                        base.WndProc(ref m);
                }
                else
                    base.WndProc(ref m);
			}
		}

        /// <override/>
        protected override void OnMouseUp( MouseEventArgs e )
        {
            base.OnMouseUp( e );
            if (this.autoCompleteControl.IsDropDownShowing())
                base.DroppedDown = false;
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.ParentChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			if(this.autoCompleteControl != null)
				this.autoCompleteControl.ParentForm = this.Parent;
		}

		private bool IsNavigationKey( Keys key )
		{
			// ignore modifiers
			if( ModifierKeys != Keys.None )
			{
				key = ( key & ~ModifierKeys );
			}

			bool bIsNavigationKey = ( ( key == Keys.Escape ) || ( key == Keys.Tab ) || ( key == Keys.Left ) || 
				( key == Keys.Right ) || ( key == Keys.Home ) || ( key == Keys.End ) || 
				( key >= Keys.F1 && key <= Keys.F24 ) );

			return bIsNavigationKey;
		}

		protected override bool ProcessCmdKey( ref Message m, Keys key )
		{
			int keyData = (int)m.WParam | ( int )ModifierKeys;

			bool processed = this.ReadOnly ? !IsNavigationKey( key ) : false;

			if( !processed ) 
			{
				processed = base.ProcessCmdKey( ref m, key );
			}

			return processed;
		}

		/// <summary>
		/// Preprocesses keyboard or input messages within the message loop before they are dispatched.
		/// </summary>
		/// <param name="msg">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the message to process. The possible values are WM_KEYDOWN, WM_SYSKEYDOWN, WM_CHAR, and WM_SYSCHAR.</param>
		/// <returns>
		/// true if the message was processed by the control; otherwise, false.
		/// </returns>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
		/// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// </PermissionSet>
		public override bool PreProcessMessage(ref System.Windows.Forms.Message msg)
		{
            if (msg.Msg == NativeMethods.WM_KEYDOWN)
            {
                Keys k = (Keys)msg.WParam.ToInt32();
                if (k >= Keys.F1 && k <= Keys.F12)
                    return (this.FindForm()).PreProcessMessage(ref msg);
                else
                    return base.PreProcessMessage(ref msg);
            }
            else
                return base.PreProcessMessage(ref msg);
		}
        
        private int savedAutoCompleteMatches = 0;

        internal bool PerformAllowNewText(Message m)
        {
            if (this.AllowNewText)
                return false;

            savedAutoCompleteMatches = 0;
            string strSet = "";
            
            if (m.Msg == NativeMethods.WM_CHAR)
            {
                char inputChar = (char)m.WParam;

                if (Char.IsLetterOrDigit(inputChar) || Char.IsSymbol(inputChar) ||
                    Char.IsSeparator(inputChar) || Char.IsPunctuation(inputChar))
                {
                    strSet = inputChar.ToString();
                }
            }
            else if (m.Msg == NativeMethods.WM_PASTE)
            {
                IDataObject iData = Clipboard.GetDataObject();

                if (iData.GetDataPresent(DataFormats.Text))
                    strSet = (String)iData.GetData(DataFormats.Text);
            }

            string strToSearch = this.Text.Substring(0, this.SelectionStart) + strSet +
                this.Text.Substring(this.SelectionStart + this.SelectionLength, this.Text.Length - (this.SelectionStart + this.SelectionLength));

            if (strToSearch != "" && !this.AllowNewText)
            {
                savedAutoCompleteMatches = this.AutoCompleteControl.GetMatchesCount(strToSearch);
                
                if (savedAutoCompleteMatches == 0)
                {
                    this.AutoCompleteControl.ProcessAutoComplete(strToSearch);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets or Sets Text according to the match. (overridden property)
        /// </summary>
		public override string Text
		{
			get
			{
				return base.Text;
			}

			set
			{
				int prevSelectedIndex = this.SelectedIndex;
				if(!AllowNewText)
				{
                    if (savedAutoCompleteMatches > 0)
                    {
                        base.Text = value;
                    }
                    else
                    {
                        savedAutoCompleteMatches = this.AutoCompleteControl.GetMatchesCount(value);
                        if (savedAutoCompleteMatches > 0)
                            base.Text = value;
                    }
				}
				else
				{					
					base.Text = value;
				}

				if( this.SelectedIndex !=prevSelectedIndex )
				{
					this.OnSelectedIndexChanged( EventArgs.Empty );
				}

				autoCompleteControl.PreChangeText = value;
			}
		}

		private bool updateComboSelectionProperties = true;

        /// <summary>
        /// Gets or sets value for updating AutoComplete with SelectedValue.
        /// </summary>
        [Category( "Behavior" )]
		[Description( "Gets or sets value for updating AutoComplete with SelectedValue." ), DefaultValue( true )]
		public bool UpdateComboSelectionProperties
		{
			get
			{
				return this.updateComboSelectionProperties;
			}

			set
			{
				this.updateComboSelectionProperties = value;
			}
		}
		/// <summary>
		/// Gets or sets the currently selected value in AutoCompleteControl when UpdateComboSelectionProperties is set .
		/// </summary>
		public new object SelectedValue
		{
			get
			{
                if (this.UpdateComboSelectionProperties)
				{
                    if (this.refreshSelectionProperties &&
                        (!this.AutoCompleteControl.ProcessingAutoComplete || savedAutoCompleteMatches == 0))
					{
						this.AutoCompleteControl.UpdateSelectedItem(this.Text);
						this.refreshSelectionProperties = false;
					}
					return this.AutoCompleteControl.SelectedValue;
				}
				else
					return base.SelectedValue;
			}
			set
			{
				base.SelectedValue = value;
			}
		}
		/// <summary>
		/// Gets or sets the currently selected item.
		/// </summary>
		public new object SelectedItem
		{
			get
			{
                if (this.UpdateComboSelectionProperties)
				{
					if(this.Text != String.Empty)
					{
                        if (this.refreshSelectionProperties &&
                            (!this.AutoCompleteControl.ProcessingAutoComplete || savedAutoCompleteMatches == 0))
						{
							this.AutoCompleteControl.UpdateSelectedItem(this.Text);
							this.refreshSelectionProperties = false;
						}
						return this.AutoCompleteControl.SelectedItem;
					}
					else
						return null;
				}
				else
					return base.SelectedItem;
			}

			set
			{
				base.SelectedItem = value;
			}
		}
		/// <summary>
		/// Gets or sets the  index of the currently selected item.
		/// </summary>
		public new int SelectedIndex
		{
			get
			{
				if(this.UpdateComboSelectionProperties)
				{
					if(this.refreshSelectionProperties && 
                        (!this.AutoCompleteControl.ProcessingAutoComplete || savedAutoCompleteMatches == 0))
					{
						this.AutoCompleteControl.UpdateSelectedItem(this.Text);
						this.refreshSelectionProperties = false;
					}
					return this.AutoCompleteControl.SelectedIndex;
				}
				else
					return base.SelectedIndex;
			}

			set
			{
				base.SelectedIndex = value;
			}
		}

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.TextChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
		protected override void OnTextChanged(EventArgs e)
		{
            if(!this.AutoCompleteControl.ProcessingAutoComplete)
			    refreshSelectionProperties = true;
            if (this.AutoCompleteMode == System.Windows.Forms.AutoCompleteMode.SuggestAppend)
            {
                if (this.autoCompleteControl.GetAutoComplete() != AutoCompleteModes.AutoSuggest)
                    this.autoCompleteControl.SetAutoComplete(this, Syncfusion.Windows.Forms.Tools.AutoCompleteModes.AutoSuggest);
            }
			base.OnTextChanged(e);
			autoCompleteControl.PreChangeText = this.Text;
		}

        /// <summary>
        /// This property is obsolete.
        /// </summary>
        [ Browsable( false ) ]
        [ EditorBrowsable( EditorBrowsableState.Never ) ]
        [ DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ) ]
        [ Description( "This property is obsolete." ) ]
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        public new int DropDownHeight
#else
		public int DropDownHeight
#endif
        {
            get 
            {
                return 0;
            }
        }

		private void HideNativeDropDown()
		{
			base.DropDownWidth = this.Width;
			base.DroppedDown = false;
		}

        /// <summary>
        /// This class used for additional handling messages, that are sent
        /// to ComboBoxAutoComplete.
        /// </summary>
        class ComboBoxAutoCompleteNativeWindow : NativeWindow
        {
            #region Class members
            /// <summary>
            /// ComboBoxAutoComplete control, listen to messages for.
            /// </summary>
            private ComboBoxAutoComplete m_cmb;

            #endregion

            #region Class Initialize/Finalize methods
            public ComboBoxAutoCompleteNativeWindow(ComboBoxAutoComplete value)
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_cmb = value;
                AssignHandle(m_cmb.TextBox);
            }
            #endregion

            #region Class overrides
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == NativeMethods.WM_CHAR ||
                    m.Msg == NativeMethods.WM_PASTE)
                {
                    if (m_cmb.PerformAllowNewText(m))
                        return;
                }

                base.WndProc(ref m);
            }
            #endregion
        }
	}
}

