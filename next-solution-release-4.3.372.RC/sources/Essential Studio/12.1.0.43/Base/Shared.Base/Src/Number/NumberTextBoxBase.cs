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

namespace Syncfusion.Windows.Forms.Tools
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Globalization;
	using System.Windows.Forms;

	using Syncfusion.ComponentModel;
	using Syncfusion.Diagnostics;
	using Syncfusion.Runtime.InteropServices;
	using Syncfusion.Windows.Forms.Localization;

	/// <summary>
	/// Extends the <see cref="TextBox"/> class to handle numeric input
	/// and validation. 
	/// </summary>
	/// <remarks>
	/// The NumberTextBoxBase class is not to be used directly. Defines the base class for 
	/// controls that provide specific formatting and validation for different data types.
	/// </remarks>
	[
	ToolboxItem( false )
	]
	public class NumberTextBoxBase: TextBoxExt

	{
		#region FIELDS

		/// <summary>
		/// The NumberFormatInfo object that will specify the
		/// localized attributes for displaying the currency
		/// value.
		/// </summary>
		private NumberFormatInfo numberFormatInfoObject = null;

		/// <summary>
		/// Forecolor to be used when the value is negative.
		/// </summary>
		private Color negativeColor;

        /// <summary>
        /// Gets or sets a value indicating whether to overrite the immediate text.
        /// </summary>
        bool overWriteText = false;

        /// <summary>
        /// Specifies when the MinMax Validation Need to be performed.
        /// </summary>
        private MinMaxValidation m_MinMaxValidation = MinMaxValidation.OnKeyPress;
        /// <summary>
        /// Specifies the Action To be Performed when Validation Fails
        /// </summary>
        private OnValidationFailed m_OnValidationFailed = OnValidationFailed.SetMinOrMax;
		/// <summary>
		/// Forecolor to be used when the value is positive.
		/// </summary>
		private Color positiveColor;

		/// <summary>
		/// Forecolor to be used when the value is zero.
		/// </summary>
		private Color zeroColor;

		/// <summary>
		/// Indicates whether the current value is negative.
		/// </summary>
		private bool isNegative;

		/// <summary>		
		/// The clip mode specifies the kind of data to be returned
		/// when copied to the clip board. 
		/// </summary>
		private CurrencyClipModes clipMode;

		/// <summary>
		/// The context menu for the text box.
		/// </summary>
		private ContextMenu editMenu;

		/// <summary>
		/// Menu items.
		/// </summary>
		private MenuItem miUndo, miCut, miCopy, miPaste, miDelete, miSelectAll;

		/// <summary>
		/// String for holding undo buffer.
		/// </summary>
		private string	undoBufferText;

		/// <summary>
		/// String for holding redo buffer.
		/// </summary>
		private string	redoBufferText;

		/// <summary>
        /// Internal variable to suppress TextChanged event.
		/// </summary>
		protected bool ignoreTextChange=false;

		/// <summary>
		/// The currently selected culture.
		/// </summary>
		private CultureInfo selectedCulture;

		/// <summary>
		/// Indicates whether the special culture values need to be applied.
		/// </summary>
		private SpecialCultureValues specialCultureValue = SpecialCultureValues.CurrentCulture;

		/// <summary>
		/// Array of cultures that require RightToLeft by default.
		/// </summary>
		private ArrayList rightToLeftCultures = new ArrayList();

		/// <summary>
		/// The UseUserOverride value to be passed in when creating CultureInfo objects.
		/// </summary>
		private bool useUserOverride = true;

		/// <summary>
		/// The NULL string value.
		/// </summary>
		private string nullString;

		/// <summary>
		/// Indicates whether the control is in NULL state.
		/// </summary>
		private bool nullState = false;

		/// <summary>
		/// The IFormatProvider for formatting NULL entries.
		/// </summary>
		private NumberFormatInfo nullFormat = null;

		/// <summary>
		/// 
		/// </summary>
		private bool returnBaseText = false;

		/// <summary>
		/// Indicates whether the NULL String should be used.
		/// </summary>
		private bool useNullString = false;

		/// <summary>
		/// Indicates whether an operation is to be rolled back because of an error.
		/// </summary>
		protected bool rollBackOperation = false;

		/// <summary>
		/// Support for culture initialization.
		/// </summary>
		private CultureInfo initCulture = new CultureInfo( CultureInfo.CurrentCulture.LCID, false );

		/// <summary>
		/// Support for NumberFormatInfo initialization.
		/// </summary>
		protected NumberFormatInfo initNumberFormatInfoObject = null;

		/// <summary>
		/// Indicates whether to allow the KeyPress event to be raised but no other action to be taken
		/// by the base class.
		/// </summary>
		protected bool supressKeyPress = false;

		/// <summary>
		/// Indicates whether to allow the KeyDown event to be raised but no other action to be taken
		/// by the base class.
		/// </summary>
		private bool supressKeyDown = false;

		/// <summary>
		/// Indicates whether the current value can be held in numeric form or has
		/// been 'dirtied' by user input.
		/// </summary>
		private bool preserveData = false;

		/// <summary>
		/// Used when the NegativeSign is keyed in when the value is zero.
		/// </summary>
		private bool zeroNegative = false;

		/// <summary>
		/// Indicates whether the control is to listen to the <see cref="ThreadCulture.CultureChanged"/>
		/// event and refresh the culture. (Only if the SpecialCultureValue is set to CurrentCulture.)
		/// </summary>
		private bool currentCultureRefresh = false;

		/// <summary>
		/// 
		/// </summary>
		private object defaultValue = 0;

        private bool allowNull = false;

		/// <summary>
		/// This will be set to True when the negative key is input with the entire contents of the
		/// textbox selected.
		/// </summary>
		private bool negativeInputPending = false;

		/// <summary>
		/// Custom backcolor that will be used when the control is Read-only.
		/// </summary>
		private Color readOnlyBackColor = SystemColors.Control;
		/// <summary>
		/// Gets or Sets the BackGroundColor of the control.
		/// </summary>
		private Color backGroundColor = System.Drawing.SystemColors.Window;

		/// <summary>
		/// Event raised when NULL State is to be set based on a value.
		/// </summary>
		[Description( "Event raised when NULL State is to be set based on a value." )]
		public event SetNullEventHandler SetNull;

		/// <summary>
		/// Event raised when a Key is to be validated. This validation is performed before any of the
		/// NumberTextBox's own validation of the input character.
		/// </summary>
		[Description( "Event raised when a Key is to be validated. This validation is performed before any of the NumberTextBox's own validation of the input character." )]
		public event KeyValidateEventHandler KeyValidate;

		#endregion

		#region INITIALIZATION

		/// <summary>
		/// Overloaded. Creates an object of type NumberTextBox. 
		/// </summary>
		/// <remarks>
		/// The NumberTextBox object will be initialized with the default values
		/// for the display and data properties. You need to set any specific 
		/// values.
		/// </remarks>
		public NumberTextBoxBase()
		{
			this.rightToLeftCultures.Add( "ar" );
			this.rightToLeftCultures.Add( "fa" );
			this.rightToLeftCultures.Add( "he" );
			this.rightToLeftCultures.Add( "ur" );
			this.rightToLeftCultures.Add( "syr" );
			this.rightToLeftCultures.Add( "div" );

			this.isNegative = false;
			this.negativeColor = Color.Red;
			this.positiveColor = Control.DefaultForeColor;
			this.zeroColor = Control.DefaultForeColor;
            this.nullString = String.Empty;
			this.clipMode = CurrencyClipModes.IncludeFormatting;
			this.CreateAndInitNumberFormatInfo();

			InitializeComponent();
			InitializeContextMenu();
            WireEvents();
		}

        private void WireEvents()
        {
            ThreadCulture.CultureChanged += new CultureChangedEventHandler(this.HandleCultureChanged);
        }
        private void UnWireEvents()
        {
            ThreadCulture.CultureChanged -= new CultureChangedEventHandler(this.HandleCultureChanged);
        }
        

        /// <summary>
        /// Implementation of the <see cref="ISupportInitialize"/> interface.
        /// </summary>
       
        public override void EndInit()
        {
            base.EndInit();
            InitializeNumberTextBox();
        }

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected void InitializeNumberTextBox()
		{
			if( SpecialCultureValue != SpecialCultureValues.CurrentCulture )
			{
				Culture = initCulture;
			}
			else
			{
				if( UseUserOverride == true )
				{
					if( CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator != CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator &&
                        CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator &&
                        CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator != CultureInfo.CurrentCulture.NumberFormat.PercentGroupSeparator )
					{
						string negativeSign = NegativeSign;
						Culture = new CultureInfo( CultureInfo.CurrentCulture.LCID, UseUserOverride );
						NumberFormatInfoObject.NegativeSign = negativeSign;
					}
					else
						Console.WriteLine( "Unable to use UseUserOverride as true because of incompatible values." );
				}
				else
					this.ApplyRightToLeft();
			}
		}

		private void HandleCultureChanged( object sender, CultureChangedEventArgs e )
		{
			if( e.NewLCID != e.CurrentLCID && ( this.SpecialCultureValue == SpecialCultureValues.CurrentCulture ) && ( this.CurrentCultureRefresh == true ) )
			{
				this.Culture = new CultureInfo( e.NewLCID, this.UseUserOverride );
			}

		}

		/// <summary>
		/// Initialize the context menu.
		/// </summary>
		private void InitializeContextMenu()
		{
			// Context menu.
			editMenu = new ContextMenu();

			// Undo menu item.

			miUndo = new MenuItem( Syncfusion.Windows.Forms.Localization.SR.GetString(SR.Undo, this));
			miUndo.Click += new EventHandler( HandleMenuUndoClick );
			miUndo.Shortcut = Shortcut.CtrlZ;
			editMenu.MenuItems.Add( miUndo );
			editMenu.MenuItems.Add( "-" );

			// Cut menu item.

			miCut = new MenuItem( SR.GetString( Syncfusion.Windows.Forms.Localization.SR.Cut, this));
			miCut.Click += new EventHandler( HandleMenuCutClick );
			miCut.Shortcut = Shortcut.CtrlX;
			editMenu.MenuItems.Add( miCut );

			// Copy menu item.

			miCopy = new MenuItem( SR.GetString( Syncfusion.Windows.Forms.Localization.SR.Copy, this));
			miCopy.Click += new EventHandler( HandleMenuCopyClick );
			miCopy.Shortcut = Shortcut.CtrlC;
			editMenu.MenuItems.Add( miCopy );

			// Paste menu item.

			miPaste = new MenuItem( SR.GetString( Syncfusion.Windows.Forms.Localization.SR.Paste, this));
			miPaste.Click += new EventHandler( HandleMenuPasteClick );
			miPaste.Shortcut = Shortcut.CtrlV;
			editMenu.MenuItems.Add( miPaste );

			// Delete menu item.
			miDelete = new MenuItem( SR.GetString( Syncfusion.Windows.Forms.Localization.SR.Delete, this));
			miDelete.Click += new EventHandler( HandleMenuDeleteClick );
			miDelete.Shortcut = Shortcut.Del;
			editMenu.MenuItems.Add( miDelete );
			editMenu.MenuItems.Add( "-" );


			// Select All menu item.
			miSelectAll = new MenuItem( SR.GetString( Syncfusion.Windows.Forms.Localization.SR.SelectAll, this));
			miSelectAll.Click += new EventHandler( HandleMenuSelectAllClick );
			miSelectAll.Shortcut = Shortcut.CtrlA;
			editMenu.MenuItems.Add( miSelectAll );

			this.ContextMenu = editMenu;
			this.ContextMenu.Popup += new EventHandler( this.HandleContextMenuPopup );
		}

		/// <summary>
		/// 
		/// </summary>
		private void InitializeComponent()
		{
			this.Multiline = false;
		}

		#endregion

		#region VALIDATION

		private bool enforceMinMaxDuringValidating = false;

		/// <summary>
		/// If the Min Max values are not met, the Validating event will be handled and cancelled if this property is set to true.
		/// </summary>
        [Browsable(false), DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("This property will be removed, use OnValidationFailed.KeepFocus Property instead.")]
		[Description( "If the Min Max values are not met, the Validating event will be handled and cancelled if this property is set to true." )]
		public bool EnforceMinMaxDuringValidating
		{
			get
			{
				return this.enforceMinMaxDuringValidating;
			}

			set
			{
				this.enforceMinMaxDuringValidating = value;
			}
		}

		/// <summary>
		/// Occurs when the input text is invalid for the current state of the control.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This event can be handled and you can do your processing based
		/// on the information provided. The ValidationErrorEventArgs object
		/// will provide the invalid text that was input and also the position
		/// within that text where the error occurred.
		/// </para>
		/// <para>
		/// The ValidationError event is raised when:
		/// 1. The <see cref="System.Windows.Forms.Control.Validating"/> event is raised (if there is invalid input).
		/// 2. Invalid key characters are input.
		/// 3. Invalid values are set through the Text property.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code lang="C#">
		///		Console.WriteLine("ValidationError in currencyTextBox1 InvalidText" + e.InvalidText);
		///		Console.WriteLine("ValidationError in currencyTextBox1 StartPosition" + e.StartPosition );</code>
		///		<coderef file="c:\syncfusion\essential suite\tools\samples\quick start\currencydemo\VB\MainForm.vb" name="Currency ValidationError" lang="VB"><code lang="VB">
		///     Console.WriteLine(("ValidationError in currencyTextBox1 InvalidText" + e.InvalidText))
		///     Console.WriteLine(("ValidationError in currencyTextBox1 StartPosition" + e.StartPosition))</code></coderef>
		/// </example>
		[Category( "Validation" )]
		[Description( "Occurs when the input text is invalid for the current state of the control." )]
		public event ValidationErrorEventHandler ValidationError;

		/// <summary>
		/// This method raises the ValidationError event.
		/// </summary>
		/// <param name="invalidText">The text that was input.</param>
		/// <param name="startPosition">The start position of the error.</param>
		/// <remarks>
		/// See the <see cref="OnValidationError"/> method for more information.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RaiseValidationError( string invalidText, int startPosition )
		{
			this.RaiseValidationError( invalidText, startPosition, String.Empty );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RaiseValidationError( string invalidText, int startPosition, string errorMessage )
		{
			ValidationErrorArgs arg = new ValidationErrorArgs( invalidText, startPosition, errorMessage );
			this.OnValidationError( arg );
		}


		/// <summary>
		/// Invokes the ValidationError event.
		/// <param name="args">A ValidationErrorEventArgs that contains the event data.</param>
		/// </summary>
		/// <remarks>
		/// The OnValidationError method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// 
		/// <note type="note">Inheritors: When overriding OnValidationError in a derived
		/// class, be sure to call the base class's OnValidationError method so that
		/// registered delegates receive the event.</note>
		/// </remarks>
		protected virtual void OnValidationError( ValidationErrorArgs args )
		{
			if( this.ValidationError != null )
				this.ValidationError( this, args );
		}

		/// <summary>
		/// Overrides <see cref="System.Windows.Forms.Control.OnValidating"/> method.
		/// </summary>
		/// <param name="args">The event data.</param>
		/// <remarks>
		/// Raises the <see cref="ValidationError"/> event when there is unacceptable
		/// text in the control.
		/// </remarks>
		protected override void OnValidating( System.ComponentModel.CancelEventArgs args )
		{
			base.OnValidating( args );
			bool validateSuccess = this.Validate( true );
			if( validateSuccess == false && this.OnValidationFailed==OnValidationFailed.KeepFocus)
			{
				args.Cancel = true;
			}
		}

		/// <summary>
		/// Validates the control.
		/// </summary>
		/// <param name="bRaiseValidationError">Indicates whether the validation error is to be raised.</param>
		virtual public bool Validate( bool bRaiseValidationError )
		{
			bool validateSuccess = true;
			string formatText = this.FormattedText;
			if (formatText == this.NullString && this.AllowNull)
			{
				return true;
			}
			if( this.BindableValue == null || Convert.IsDBNull( this.BindableValue ) == true )
			{
				validateSuccess = false;
			}
			else if( formatText != null && formatText != String.Empty )
			{
				string currentValue = this.GetNumberValue( this.FormattedText, 0 );
				validateSuccess = this.CheckForMinMax( currentValue, true );
			}
			else
			{
				validateSuccess = false;
			}

			if( validateSuccess == false )
			{
				if( bRaiseValidationError )
					this.RaiseValidationError( this.FormattedText, 0, "Current value does not meet MinValue and MaxValue requirements." );
			}

			return validateSuccess;
		}

		#endregion

		#region CULTURE

		/// <summary>
		/// Gets or sets the culture that is to be used for formatting the numeric display.
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Localizable( false ),
		RefreshProperties( RefreshProperties.Repaint ),
		Editor( typeof( Syncfusion.Windows.Forms.Tools.Design.CurrencyCultureEditor ), typeof( System.Drawing.Design.UITypeEditor ) ),
		Description( "Gets or sets the culture that is to be used for formatting the numeric display." )
		]
		public CultureInfo Culture
		{
			get
			{
				if( this.SpecialCultureValue == SpecialCultureValues.CurrentCulture )
					return CultureInfo.CurrentCulture;
				else if( this.SpecialCultureValue == SpecialCultureValues.UICulture )
					return CultureInfo.CurrentUICulture;
				else if( this.SpecialCultureValue == SpecialCultureValues.InstalledCulture )
					return CultureInfo.InstalledUICulture;

				if( this.selectedCulture == null )
					return CultureInfo.CurrentCulture;

				return this.selectedCulture;
			}

			set
			{
				CultureInfo prevSelectedCulture = this.selectedCulture;

				if( this.SpecialCultureValue == SpecialCultureValues.None )
					this.selectedCulture = value;
				else if( this.SpecialCultureValue == SpecialCultureValues.CurrentCulture )
					this.selectedCulture = CultureInfo.CurrentCulture;
				else if( this.SpecialCultureValue == SpecialCultureValues.UICulture )
					this.selectedCulture = CultureInfo.CurrentUICulture;
				else if( this.SpecialCultureValue == SpecialCultureValues.InstalledCulture )
					this.selectedCulture = CultureInfo.InstalledUICulture;

				if( this.Initializing == false )
				{
					// Change the NumberFormat also.
					this.NumberFormatInfoObject = this.Culture.NumberFormat;

					if( prevSelectedCulture != this.selectedCulture )
					{
						this.ApplyRightToLeft();
					}
				}
				else
					this.initCulture = value;
			}
		}

		/// <summary>
		/// Refreshes and reapplies the culture specific settings. 
		/// </summary>
		/// <remarks>
		/// Call this function when there has been a change in the CurrentCulture of the
		/// application.
		/// </remarks>
		public void RefreshCulture()
		{
			if( this.SpecialCultureValue == SpecialCultureValues.CurrentCulture )
				this.Culture = CultureInfo.CurrentCulture;
			else if( this.SpecialCultureValue == SpecialCultureValues.UICulture )
				this.Culture = CultureInfo.CurrentUICulture;
			else if( this.SpecialCultureValue == SpecialCultureValues.InstalledCulture )
				this.Culture = CultureInfo.InstalledUICulture;

			this.NumberFormatInfoObject = this.Culture.NumberFormat;
			this.ApplyRightToLeft();
		}

		private bool ShouldSerializeCulture()
		{
			if( this.SpecialCultureValue == SpecialCultureValues.None )
				return true;
			else
				return false;
		}

		private void ResetCulture()
		{
			this.SpecialCultureValue = SpecialCultureValues.None;
			this.Culture = CultureInfo.CurrentCulture;
		}


		/// <summary>
		/// Gets or sets the mode for the cultures.
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance" ),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Gets or sets the mode for the cultures." )
		]
		public SpecialCultureValues SpecialCultureValue
		{
			get
			{
				return this.specialCultureValue;
			}

			set
			{
				this.specialCultureValue = value;
			}
		}

		private bool ShouldSerializeSpecialCultureValue()
		{
			if( this.SpecialCultureValue != SpecialCultureValues.CurrentCulture )
				return true;
			else
				return false;
		}

		private void ResetSpecialCultureValue()
		{
			this.SpecialCultureValue = SpecialCultureValues.CurrentCulture;
		}

		/// <summary>
		/// Indicates whether the Culture property is to be refreshed when the culture changes.
		/// </summary>
		[
		Browsable( true ),
		Category( "Behavior" ),
		RefreshProperties( RefreshProperties.Repaint ),
		DefaultValue( false ),
		Description( "Indicates whether the Culture property is to be refreshed when the culture changes." )
		]
		public bool CurrentCultureRefresh
		{
			get
			{
				return this.currentCultureRefresh;
			}

			set
			{
				this.currentCultureRefresh = value;
			}
		}

		/// <summary>
		/// The UseUserOverride parameter for CultureInfo.
		/// </summary>
		/// <remarks>
		/// The NumberTextBoxBase control has several properties that expose culture-specific
		/// information. These properties use a <see cref="NumberFormatInfo"/> object for
		/// handling the culture specific information. This property is used in the creation
		/// of the NumberFormatInfo object. <seealso cref="CultureInfo.UseUserOverride"/>
		/// </remarks>
		[
		Browsable( true ),
		Category( "Culture" ),
		Description( "Specifies if the NumberFormatInfo used for formatting will use the User Overrides for the culture." ),
		DefaultValue( true )
		]
		public bool UseUserOverride
		{
			get
			{
				return this.useUserOverride;
			}

			set
			{
				this.useUserOverride = value;
			}
		}

		/// <summary>
		/// Indicates whether the NULLString property will be used.
		/// </summary>
		[
		Browsable( false ),
		Category( "Behavior" ),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Description( "Specifies if the NULLString will be used when the value is NULL." ),
		DefaultValue( false ),
		Obsolete("This property will not be used in future.Use AllowNull instead")
		]
		public bool UseNullString
		{
			get
			{
				return this.useNullString;
			}

			set
			{
				this.useNullString = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected void ApplyRightToLeft()
		{
			if( this.rightToLeftCultures.Contains( this.Culture.Parent.Name ) && this.RightToLeft == RightToLeft.No )
				this.RightToLeft = RightToLeft.Yes;
			else if( this.RightToLeft == RightToLeft.Yes )
				this.RightToLeft = RightToLeft.No;
		}

		#endregion

		#region FORMATTING

		/// <summary>
		///  Gets or sets a value indicating whether control's elements are aligned to
		///     support locales using right-to-left fonts.
		/// </summary>
		public new RightToLeft RightToLeft
		{
			get
			{
				return base.RightToLeft;
			}

			set
			{
				if( value == RightToLeft.Yes && this.rightToLeftCultures.Contains( this.Culture.Parent.Name ) )
				{
					base.RightToLeft = RightToLeft.Yes;
				}
				else
				{
					base.RightToLeft = RightToLeft.No;
				}

			}
		}

		private bool ShouldSerializeRightToLeft()
		{
			if( this.RightToLeft == RightToLeft.No )
				return false;
			else
				return true;
		}

		/// <summary>
		/// Resets the <see cref="P:System.Windows.Forms.Control.RightToLeft"/> property to its default value.
		/// </summary>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
		/// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// </PermissionSet>
		public override void ResetRightToLeft()
		{
			this.RightToLeft = RightToLeft.No;
		}

		private bool ShouldSerializeForeColor()
		{
			if( this.ForeColor == SystemColors.ControlText )
				return false;
			else
				return true;
		}

		/// <summary>
		/// Resets the <see cref="P:System.Windows.Forms.Control.ForeColor"/> property to its default value.
		/// </summary>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// </PermissionSet>
		public override void ResetForeColor()
		{
			this.ForeColor = SystemColors.ControlText;
		}


		/// <summary>
		/// Gets or sets the back color. (overridden property)
		/// </summary>
		[Browsable(false),EditorBrowsable(EditorBrowsableState.Never),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BackColor
		{
			get
			{
				return base.BackColor;
			}

			set
			{
				base.BackColor = value;

				// for Backward compatibility
				if (!this.ReadOnly)
					this.BackGroundColor = this.BackColor;
			}
		}

		/// <summary>
		/// Gets or sets the NumberFormatInfo object that will be used for formatting the
		/// number value.
		/// </summary>
		/// <remarks>
		/// This property will not be exposed to the developer. This is only meant
		/// to be an accessor for use within the control. The developer will be able
		/// to access the properties of the NumberFormatInfo through the individual
		/// properties exposed.
		/// </remarks>
		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		Category( "Appearance" ),
		Description( "The NumberFormatInfo object that will be used for formatting the number value." ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public NumberFormatInfo NumberFormatInfoObject
		{
			get
			{
				if( this.numberFormatInfoObject == null )
					CreateAndInitNumberFormatInfo();

				return this.numberFormatInfoObject;
			}

			set
			{
				NumberFormatInfo localObject = new NumberFormatInfo();

				if( value != null )
					localObject = value;
				else
					localObject = this.Culture.NumberFormat;

				if( this.Initializing == false )
				{
					string currentText = GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();

					if( this.numberFormatInfoObject == null )
						this.numberFormatInfoObject = new NumberFormatInfo();

					this.numberFormatInfoObject.CurrencyDecimalDigits = localObject.CurrencyDecimalDigits;

					this.numberFormatInfoObject.CurrencyDecimalSeparator = localObject.CurrencyDecimalSeparator;
					this.numberFormatInfoObject.NumberDecimalSeparator = localObject.NumberDecimalSeparator;
					this.numberFormatInfoObject.PercentDecimalSeparator = localObject.PercentDecimalSeparator;

					this.numberFormatInfoObject.CurrencyGroupSeparator = localObject.CurrencyGroupSeparator;
					this.numberFormatInfoObject.NumberGroupSeparator = localObject.NumberGroupSeparator;
					this.numberFormatInfoObject.PercentGroupSeparator = localObject.PercentGroupSeparator;

					this.numberFormatInfoObject.CurrencyGroupSizes = localObject.CurrencyGroupSizes;
					this.numberFormatInfoObject.CurrencyNegativePattern = localObject.CurrencyNegativePattern;
					this.numberFormatInfoObject.CurrencyPositivePattern = localObject.CurrencyPositivePattern;
					this.numberFormatInfoObject.CurrencySymbol = localObject.CurrencySymbol;

					this.numberFormatInfoObject.NegativeSign = localObject.NegativeSign;

					this.FormatChanged( currentText, prevFormat );
				}
				else
					this.initNumberFormatInfoObject = localObject;
			}
		}

		private void CreateAndInitNumberFormatInfo()
		{
			this.numberFormatInfoObject = new NumberFormatInfo();

			this.numberFormatInfoObject.CurrencyDecimalDigits = this.Culture.NumberFormat.CurrencyDecimalDigits;

			this.numberFormatInfoObject.CurrencyDecimalSeparator = this.Culture.NumberFormat.CurrencyDecimalSeparator;
			this.numberFormatInfoObject.NumberDecimalSeparator = this.Culture.NumberFormat.NumberDecimalSeparator;
			this.numberFormatInfoObject.PercentDecimalSeparator = this.Culture.NumberFormat.PercentDecimalSeparator;

			this.numberFormatInfoObject.CurrencyGroupSeparator = this.Culture.NumberFormat.CurrencyGroupSeparator;
			this.numberFormatInfoObject.NumberGroupSeparator = this.Culture.NumberFormat.NumberGroupSeparator;
			this.numberFormatInfoObject.PercentGroupSeparator = this.Culture.NumberFormat.PercentGroupSeparator;

			this.numberFormatInfoObject.CurrencyGroupSizes = this.Culture.NumberFormat.CurrencyGroupSizes;
			this.numberFormatInfoObject.CurrencyNegativePattern = this.Culture.NumberFormat.CurrencyNegativePattern;
			this.numberFormatInfoObject.CurrencyPositivePattern = this.Culture.NumberFormat.CurrencyPositivePattern;
			this.numberFormatInfoObject.CurrencySymbol = this.Culture.NumberFormat.CurrencySymbol;

			this.numberFormatInfoObject.PercentGroupSizes = this.Culture.NumberFormat.PercentGroupSizes;
			this.numberFormatInfoObject.PercentNegativePattern = this.Culture.NumberFormat.PercentNegativePattern;
			this.numberFormatInfoObject.PercentPositivePattern = this.Culture.NumberFormat.PercentPositivePattern;
			this.numberFormatInfoObject.PercentSymbol = this.Culture.NumberFormat.PercentSymbol;

			this.numberFormatInfoObject.NumberNegativePattern = this.Culture.NumberFormat.NumberNegativePattern;
		}


		/// <summary>
		/// Gets or sets the sign that is to be used to indicate a negative value.
		/// </summary>
		/// <remarks>
		/// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
		/// and can be changed based on your requirements or based on the locale.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Localizable( false ),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Gets or sets the sign that is to be used to indicate a negative value." )
		]
		public string NegativeSign
		{
			get
			{
				return this.numberFormatInfoObject.NegativeSign;
			}

			set
			{
				if( this.Initializing == false )
				{
					string currentText = GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.numberFormatInfoObject.NegativeSign = value;
					this.FormatChanged( currentText, prevFormat );
				}
				else
					this.numberFormatInfoObject.NegativeSign = value;
			}
		}

		/// <summary>
		/// Indicates whether the NegativeSign should not be serialized if the value is the same as the one for the 
		/// current culture.
		/// </summary>
		/// <returns>True if the property should be serialized; otherwise False.</returns>
		protected bool ShouldSerializeNegativeSign()
		{
			return this.Culture.NumberFormat.NegativeSign != this.NegativeSign;
		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetNegativeSign()
		{
			this.NegativeSign = this.Culture.NumberFormat.NegativeSign;
		}


		/// <summary>
		/// Returns a copy of the current NumberFormatInfo.
		/// </summary>
		/// <returns></returns>
		protected NumberFormatInfo GetCopyOfCurrentNumberFormatInfo()
		{
			NumberFormatInfo info = new NumberFormatInfo();
			if( this.NumberFormatInfoObject != null )
			{
				info.CurrencyDecimalDigits = this.NumberFormatInfoObject.CurrencyDecimalDigits;

				info.CurrencyDecimalSeparator = this.NumberFormatInfoObject.CurrencyDecimalSeparator;
				info.NumberDecimalSeparator = this.NumberFormatInfoObject.NumberDecimalSeparator;
				info.PercentDecimalSeparator = this.NumberFormatInfoObject.PercentDecimalSeparator;

				info.CurrencyGroupSeparator = this.NumberFormatInfoObject.CurrencyGroupSeparator;
				info.NumberGroupSeparator = this.NumberFormatInfoObject.NumberGroupSeparator;
				info.PercentGroupSeparator = this.NumberFormatInfoObject.PercentGroupSeparator;

				info.CurrencyGroupSizes = this.NumberFormatInfoObject.CurrencyGroupSizes;
				info.CurrencyNegativePattern = this.NumberFormatInfoObject.CurrencyNegativePattern;
				info.CurrencyPositivePattern = this.NumberFormatInfoObject.CurrencyPositivePattern;
				info.CurrencySymbol = this.NumberFormatInfoObject.CurrencySymbol;

				info.PercentGroupSizes = this.NumberFormatInfoObject.PercentGroupSizes;
				info.PercentNegativePattern = this.NumberFormatInfoObject.PercentNegativePattern;
				info.PercentPositivePattern = this.NumberFormatInfoObject.PercentPositivePattern;
				info.PercentSymbol = this.NumberFormatInfoObject.PercentSymbol;

			}

			return info;
		}

		/// <summary>
		/// Gets or sets the forecolor when the current value is negative.
		/// </summary>
		/// <remarks>
		/// You can customize the look and provide feedback to the user by defining
		/// a different color for the negative numbers.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( "Gets or sets the forecolor when the current value is negative." )
		]
		public Color NegativeColor
		{
			get
			{
				return this.negativeColor;
			}

			set
			{
				this.negativeColor = value;
				if( this.Initializing == false )
					this.SetControlColor();
			}
		}

		/// <summary>
		/// Indicates whether NegativeColor property value should be serialized.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public bool ShouldSerializeNegativeColor()
		{
			if( this.NegativeColor == Color.Red )
				return false;
			else
				return true;
		}

		/// <summary>
		/// Resets NegativeColor property to default value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public void ResetNegativeColor()
		{
			this.NegativeColor = Color.Red;
		}


		/// <summary>
		/// Gets or sets the forecolor when the current value is zero.
		/// </summary>
		/// <remarks>
		/// You can customize the look and provide feedback to the user by defining
		/// a different color for displaying zero.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( "Gets or sets the forecolor when the current value is zero." )
		]
		public Color ZeroColor
		{
			get
			{
				return this.zeroColor;
			}

			set
			{
				this.zeroColor = value;
				this.SetControlColor();
			}
		}

		/// <summary>
		/// Indicates whether ZeroColor property value should be serialized.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public bool ShouldSerializeZeroColor()
		{
			if( this.ZeroColor == Control.DefaultForeColor )
				return false;
			else
				return true;
		}

		/// <summary>
		/// Resets ZeroColor property to default value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public void ResetZeroColor()
		{
			this.ZeroColor = Control.DefaultForeColor;
		}

		/// <summary>
		/// Gets or sets the forecolor when the current value is positive.
		/// </summary>
		/// <remarks>
		/// You can customize the look and provide feedback to the user by defining
		/// a different color for the positive numbers.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( "Gets or sets the forecolor when the current value is positive." )
		]
		public Color PositiveColor
		{
			get
			{
				return this.positiveColor;
			}

			set
			{
				this.positiveColor = value;
				if( this.Initializing == false )
					this.SetControlColor();
			}
		}

		/// <summary>
		/// Indicates whether PositiveColor property value should be serialized.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public bool ShouldSerializePositiveColor()
		{
			if( this.PositiveColor == Control.DefaultForeColor )
				return false;
			else
				return true;
		}

		/// <summary>
		/// Resets ResetPositiveColor property to default value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public void ResetPositiveColor()
		{
			this.PositiveColor = Control.DefaultForeColor;
		}

		/// <summary>
		/// Returns the NumberFormatInfo object for the NULL display.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public NumberFormatInfo NullFormat
		{
			get
			{
				if( this.nullFormat == null )
				{
					this.nullFormat = new NumberFormatInfo();
					this.nullFormat.CurrencyDecimalDigits = 0;
					this.nullFormat.CurrencySymbol = String.Empty;
				}
				return this.nullFormat;

			}
		}

		/// <summary>
		/// Gets or sets the NULL string to be displayed.
		/// </summary>
		[
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( "Specify the string to be displayed when the DecimalValue is 0." ),
		RefreshProperties( RefreshProperties.Repaint ),
		]
		public string NullString
		{
			get
			{
				return this.nullString;
			}

			set
			{
				this.nullString = value;

				OnNullStringChanged(this.IsNull);
			}
		}

        /// <summary>
        /// Gets or sets the NULL string to be displayed.
        /// </summary>
        [
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Specify whether the TextBox can be empty "),
        DefaultValue(false)
        ]
        public virtual bool AllowNull
        {
            get
            {
                return this.allowNull;
            }

            set
            {
                if (this.allowNull != value)
                {
                    this.allowNull = value;
                    OnAllowNullChanged();
                }
            }
        }

		/// <summary>
		/// Occurs when the <see cref="BindableValue"/> property is changed.
		/// </summary>
		[
		Category( @"Property Changed" ),
		Description( @"Occurs when the BindableValue property is changed." )
		]
		public event EventHandler BindableValueChanged;

		/// <summary>
		/// Raises the <see cref="BindableValueChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnBindableValueChanged( EventArgs e )
		{
			if( BindableValueChanged != null )
				BindableValueChanged( this, e );
		}

        /// <summary>
        /// Occurs when the <see cref="BindableValue"/> property is changed.
        /// </summary>
        [
        Category(@"Validation"),
        Description(@"Occurs when control is validated.")
        ]
        public event ControlValidatedEventHandler ControlValidated;

        protected virtual void OnControlValidated(string newValue, string oldValue)
        {
            ControlValidatedEventArgs ctrlValidatedEventArgs = new ControlValidatedEventArgs(newValue, oldValue);
            if (this.ControlValidated != null)
                this.ControlValidated(this, ctrlValidatedEventArgs);
        }

		/// <summary>
		/// Wrapper property around the selected value. Use this property if you
		/// want to be able to set the value of the control to NULL.
		/// </summary>
		[Description( "Wrapper property that indicates the value. Can be set to NULL." )]
		[Category( "Behavior" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		[Bindable( true )]
		public object BindableValue
		{
			get
			{
				if( this.IsNull == true && this.Text == this.NullString )
					return null;
				else
					return this.GetValue();
			}
			set
			{
				bool valueAssigned =  false;

                string msg = string.Empty;
				if( value == System.DBNull.Value || value == null )
				{
                    if (!this.AllowNull)
                    {
                        msg = "Null value can't be set when AllowNull is false";
                    }
					else if(!this.RaiseSetNull( value ) )
					{
						this.SetNullNumberValue();
						SetTextBoxText( this.NullString );
						valueAssigned = true;
					}
				}


				if( valueAssigned == false )
				{
					bool assignable = IsAssignable( value );

					if( assignable == false )
						throw new ArgumentException(msg);
					else
					{
						this.SetValue( value );
					}
				}

				OnBindableValueChanged( EventArgs.Empty );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool IsAssignable( object val )
		{
			return true;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void SetValue( object val )
		{

		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual object GetValue()
		{
			return null;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetDefaultValue( object defaultValue )
		{
			this.defaultValue = defaultValue;
		}

		/// <summary>
		/// Gets or set the default value. 
		/// </summary>
		[Description( "Can be set to NULL." )]
		[Category( "Behavior" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}

			set
			{
				bool valueAssigned =  false;

				if( value == System.DBNull.Value || value == null )
				{
					this.defaultValue = value;
					valueAssigned = true;
				}

				if( valueAssigned == false )
				{
					bool assignable = IsAssignable( value );

					if( assignable == false )
						throw new ArgumentException();
					else
						this.defaultValue = value;
				}
			}
		}
       
        /// <summary>
        /// Gets or sets a value indicating whether to overrite the immediate text when the text cannot be inserted.
        /// Effective when MinMaxValidation is set to OnKeyPress.
        /// </summary>
        [
        Description("Gets or sets a value indicating whether to overrite the immediate text."),
        DefaultValue(typeof(bool), "false")
        ]
        public bool OverWriteText
        {
            get { return overWriteText; }
            set { overWriteText = value; }
        }

        /// <summary>
        /// Specifies when the MinMax Validation Need to be performed.
        /// </summary>
        [
        Description("Specifies when the MinMax Validation Need to be performed."),
        DefaultValue(typeof(MinMaxValidation), "OnKeyPress")
        ]
        public MinMaxValidation MinMaxValidation
        {
            get
            {
                return this.m_MinMaxValidation;
            }
            set
            {
                if (this.m_MinMaxValidation != value)
                    this.m_MinMaxValidation = value;
            }
        }
        /// <summary>
        /// Overrides the MaxLength property. This has no effect on this EditControl as it does not honor MinValue and MaxValue properties.
        /// </summary>
        /// <remarks>
        /// The <see cref="PercentTextBox"/> control does not honor the 
        /// MaxLength property. Set the <see cref="MaxValue"/> and <see cref="MinValue"/>
        /// properties.
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new int MaxLength
        {
            get
            {
                return base.MaxLength;
            }

            set
            {
                //base.MaxLength = value;
            }
        }
        /// <summary>
        /// Specifies the action to be performed when validation fails, Effective only if MinMaxValidation is set to OnLostFocus
        /// </summary>
        [
        Description("Specifies the action to be performed when validation fails, Effective only if MinMaxValidation is set to OnLostFocus"),
        DefaultValue(typeof(OnValidationFailed), "SetMinOrMax")
        ]
        public OnValidationFailed OnValidationFailed
        {
            get
            {
                return this.m_OnValidationFailed;
            }
            set
            {
                if (this.m_OnValidationFailed != value)
                    this.m_OnValidationFailed = value;
            }
        }

        /// <summary>
		/// Indicates whether the field is Null(NullString) or Not.
		/// </summary>
        /// <remarks>when this is True and <see cref="AllowNull"/> is True the field will be assigned with <see cref="NullString"/> </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool IsNull
        {
            get
            {
                return (base.Text==this.NullString || base.Text==String.Empty || base.Text==null); 
            }
        }

		/// <summary>
		/// Indicates the NULLState of the control.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Obsolete("This property will not be used in the future versions. Use IsNull instead to check if the value is null.")
		]
		public bool NullState
		{
			get
			{
				return this.nullState;
			}
			set
			{
				if( nullState != value )
				{
					nullState = value;
				}
			}
		}

		/// <summary>
		/// Sets the control's forecolor depending on whether the
		/// current value is negative.
		/// </summary>
		/// <remarks>
		/// See the <see cref="NumberTextBoxBase.NegativeColor"/> and <see cref="NumberTextBoxBase.PositiveColor"/>
		/// properties.
		/// </remarks>
		public void SetControlColor()
		{
			if( this.IsNegative )
			{
				if( this.ForeColor != this.NegativeColor )
					this.ForeColor = this.NegativeColor;
			}
			else
			{
				if (this.CheckIsZero())
				{
					if( this.GetZeroNegative() == true && this.ForeColor != this.NegativeColor )
						this.ForeColor = this.NegativeColor;
					else if( this.ForeColor != this.ZeroColor )
						this.ForeColor = this.ZeroColor;
				}
				else if( this.ForeColor != this.PositiveColor )
					this.ForeColor = this.PositiveColor;
			}
		}

		/// <summary>
		/// Takes the incoming text, formats it based on the
		/// rules and settings prevailing currently and sets
		/// the display accordingly.
		/// Returns the length of the new text after setting the
		/// display.
		/// </summary>
		/// <param name="rawValue"></param>
		/// <returns></returns>
		protected string ApplyFormattingAndSetText( string rawValue )
		{
			bool textChangeState = this.ignoreTextChange;
			this.IsNegative  = CheckIfNegative( rawValue );
			this.undoBufferText = this.GetTextBoxText();
		    string formattedText = this.ApplyFormatting( rawValue );
            //if (formattedText.Length > this.MaxLength)
            //{
            //    formattedText = RemoveFormatting(formattedText);
            //}

			// Added 06/25/02 to prevent DataBinding bug.
			//this.ignoreTextChange = true;
			//this.SetTextBoxText( rawValue );

			// 10/07/02 To force the control to refresh itself 
			// when the same value is applied. This is necessary when
			// the display paramters have changed (like +ve and -ve).
			//this.SetTextBoxText("");	

			this.ignoreTextChange = textChangeState;
			ReturnBaseText = true;
			this.SetTextBoxText( formattedText );
			ReturnBaseText = false;
			this.SetControlColor();
			return formattedText;
		}

		/// <summary>
		/// Takes the incoming text, formats it based on the
		/// rules and settings prevailing currently and sets
		/// the display accordingly. 
		/// Returns the length of the new text after setting the
		/// display.
		/// </summary>
		/// <param name="rawValue"></param>
		/// <returns></returns>
		protected string SetModifiedText( string formattedText, string rawValue )
		{
			bool textChangeState = this.ignoreTextChange;
			this.IsNegative  = CheckIfNegative( rawValue );
			this.undoBufferText = this.GetTextBoxText();

			// Added 06/25/02 to prevent DataBinding bug.
			//this.ignoreTextChange = true;
			//this.SetTextBoxText( rawValue );

			// 10/07/02 To force the control to refresh itself 
			// when the same value is applied. This is necessary when
			// the display paramters have changed (like +ve and -ve).
			//this.SetTextBoxText("");	

			this.ignoreTextChange = textChangeState;
			ReturnBaseText = true;
            this.SetTextBoxText(formattedText);
			ReturnBaseText = false;
			this.SetControlColor();
			return formattedText;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetTextBoxText( string text )
		{
			base.Text = text;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected string GetTextBoxText()
		{
			return base.Text;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int GetTextBoxTextLength()
		{
			return base.Text.Length;
		}

		/// <summary>
		/// Formats the given text according to the current setting.
		/// </summary>
		/// <param name="rawValue"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string ApplyFormatting( string rawValue )
		{
			return rawValue;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string RemoveFormatting( string formattedText )
		{
			return this.RemoveFormatting( formattedText, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string RemoveFormatting( string formattedText, bool padIfEmpty )
		{
			return this.RemoveFormatting( formattedText, this.NumberFormatInfoObject, padIfEmpty );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string RemoveFormatting( string formattedText, NumberFormatInfo info )
		{
			return this.RemoveFormatting( formattedText, info, true );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="formattedText"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string RemoveFormatting( string formattedText, NumberFormatInfo info, bool padIfEmpty )
		{
			int		decimalSeparatorPosition = 0;
			string	numberPartString = String.Empty;
			string	decimalPartString = String.Empty;
			string rawText = String.Empty;

			decimalSeparatorPosition = this.GetDecimalSeparatorPosition( formattedText, info );

			formattedText = formattedText.Trim();

			// What is text to be inserted has - as the first character.
			if( formattedText != String.Empty )
			{
				bool returnBaseText = this.ReturnBaseText;
				this.ReturnBaseText = true;
				int selectionStart = this.SelectionStart;
				int selectionLength = this.SelectionLength;
				this.ReturnBaseText = returnBaseText;

				if( selectionStart == 0 && selectionLength == this.GetTextBoxTextLength() )
					if( this.ParseForNegativeFormat( formattedText ) )
						rawText = this.NegativeSign;
			}

			if( decimalSeparatorPosition < 0 )
				decimalSeparatorPosition = formattedText.Length;

			for( int i=0; i<decimalSeparatorPosition; i++ )
			{
				if( Char.IsDigit( formattedText[i] ) )
					numberPartString += formattedText[i];
			}

			if( decimalSeparatorPosition >= 0 && decimalSeparatorPosition < formattedText.Length )
			{
				for( int j=decimalSeparatorPosition; j<formattedText.Length; j++ )
				{
					if( Char.IsDigit( formattedText[j] ) )
						decimalPartString += formattedText[j];
				}
				decimalPartString = decimalPartString.Trim();
				if( decimalPartString == String.Empty )
					decimalPartString = "0";
				rawText = rawText + numberPartString + this.GetDecimalSeparator( info ) + decimalPartString;
			}
			else
			{
				if( ( numberPartString == null || numberPartString == String.Empty ) && padIfEmpty == true )
					numberPartString = "0";
				rawText = rawText + numberPartString;
			}

			return rawText;
		}
        /// <summary>
        /// Gets or Sets the BackGroundColor of the control.
        /// </summary>
        [Category("Appearance"), Description("Gets or Sets the BackColor of the control.")]
        public Color BackGroundColor
        {
            get 
            {
                return backGroundColor; 
            }
            set
            {
                backGroundColor = value;

                if (!this.ReadOnly)
                    base.BackColor = value;
            }
        }
        /// <summary>
        /// Resets the ControlBackColor property to its default value.
        /// </summary>
		public void ResetControlBackColor()
		{
			this.BackGroundColor = SystemColors.Window;
		}
		private bool ShouldSerializeControlBackColor()
		{
			if (this.BackGroundColor == SystemColors.Window)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Color to be used for the Backcolor when the control is Read-only.
		/// </summary>
		[Category( "Appearance" )]
		[Description( @"Color to be used for the Backcolor when the control is Read-only." )]
		public Color ReadOnlyBackColor
		{
			get
			{
				return this.readOnlyBackColor;
			}

			set
			{
				if( this.readOnlyBackColor != value )
				{
					this.readOnlyBackColor = value;
					if( this.ReadOnly )
					{
						base.BackColor = this.ReadOnlyBackColor;
					}
				}
			}
		}

		/// <summary>
		/// Resets ReadOnlyBackColor property to default value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public virtual void ResetReadOnlyBackColor()
		{
			this.readOnlyBackColor = SystemColors.Control;
		}
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            if (this.ReadOnly)
            {
                this.BackColor = this.ReadOnlyBackColor;
            }
            else
            {
                this.BackColor = this.BackGroundColor;
            }
        }
		private bool ShouldSerializeReadOnlyBackColor()
		{
			return this.readOnlyBackColor != SystemColors.Control;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnReadOnlyChanged( EventArgs e )
		{
			base.OnReadOnlyChanged(e);

			if( this.ReadOnly )
			{
				this.BackColor = this.ReadOnlyBackColor;
			}
			else
			{
				this.BackColor = this.BackGroundColor;
			}			
		}

		#endregion

		#region DATA

		/// <summary>
		/// Determines whether to include or exclude the literal characters in the input mask when doing a copy command.
		/// </summary>
		/// <remarks>
		/// This property is used when copying to the clipboard and also the
		/// <see cref="Text"/> property.
		/// <para>
		/// When databinding the Text property, it is advisable to have the ClipMode
		/// set to <see cref="CurrencyClipModes.ExcludeFormatting"/> in cases where
		/// the data source does not accept the formatted text.
		/// </para>
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		DefaultValue( CurrencyClipModes.IncludeFormatting ),
		Description( "Determines whether to include or exclude the literal characters in the input mask when doing a copy command." )
		]
		public CurrencyClipModes ClipMode
		{
			get
			{
				return this.clipMode;
			}
			set
			{
				this.clipMode = value;
			}
		}

		/// <summary>
		/// Returns the clipped text without the formatting.
		/// </summary>
		/// <remarks>
		/// For example, if the text in the CurrencyTextBox is $45,000.00, the
		/// ClipText property will give 45000.00.
		/// </remarks>
		[
		Browsable( false ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		virtual public string ClipText
		{
			get
			{
				return this.GetClipText();
			}

			set
			{
				this.SetTextProperty( value );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void SetTextProperty( string newText )
		{
			bool valueAssigned = false;

			if( newText == null || newText == String.Empty )
			{
				if(this.AllowNull && !RaiseSetNull( newText ) )
				{
					this.SetNullNumberValue();

					SetTextBoxText( this.NullString );

					valueAssigned = true;
				}
				else
					//This will ensure that the newText always empty. Leaving it null causes issue with String formatting.
					newText = string.Empty; 
			}

			if( valueAssigned == false )
			{
				this.SelectAll();
				string currentText = this.GetTextBoxText();
				int selectionStart = this.SelectionStart;
				int selectionLength = this.SelectionLength;
				if( selectionStart > currentText.Length || selectionLength > currentText.Length )
				{
					this.SetTextBoxText( this.FormattedText );
					currentText = this.GetTextBoxText();
				}
				this.InsertString( currentText, selectionStart, selectionLength, newText, true );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void SetNullNumberValue()
		{

		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual string GetClipText()
		{
			return GetClipText( true );
		}

		/// <summary>
		/// Gets the clip text. 
		/// </summary>
		/// <param name="padIfEmpty"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public string GetClipText( bool padIfEmpty )
		{
			string clippedText = this.RemoveFormatting( this.GetTextBoxText(), padIfEmpty );
			if( this.isNegative )
			{
				if( clippedText != null && clippedText.Length > 0 && this.NegativeSign.Length > 0 )
				{
					if( clippedText[0] != this.NegativeSign[0] )
						clippedText = this.NegativeSign + clippedText;
				}
			}
			return clippedText;
		}

		/// <summary>
		/// Occurs when the <see cref="Syncfusion.Windows.Forms.Tools.CurrencyTextBox.DecimalValue"/> property is changed.
		/// </summary>
		[Category( "PropertyChanged" )]
		[Description( "Occurs when the ClipText property is changed." )]
		public event EventHandler ClipTextChanged;

		/// <summary>
		/// Raises the <see cref="NumberTextBoxBase.ClipTextChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnClipTextChanged( EventArgs e )
		{
			if( ClipTextChanged != null )
				ClipTextChanged( this, e );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ReturnBaseText
		{
			get
			{
				return this.returnBaseText;
			}

			set
			{
				this.returnBaseText = value;
			}
		}


		/// <summary>
		/// Returns the formatted text with the formatting.
		/// </summary>
		/// <remarks>
		/// For example, if the Text in the CurrencyTextBox is $45,000.00, the
		/// FormattedText property will give $45,000.00.
		/// </remarks>
		[
		Browsable( false ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		virtual public string FormattedText
		{
			get
			{
				return this.GetTextBoxText();
			}

			set
			{
				this.Text = value;
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Syncfusion.Windows.Forms.Tools.CurrencyTextBox.DecimalValue"/> property is changed.
		/// </summary>
		[Category( "PropertyChanged" )]
		[Description( "Occurs when the FormattedText property is changed." )]
		public event EventHandler FormattedTextChanged;

		/// <summary>
		/// Raises the <see cref="FormattedTextChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnFormattedTextChanged( EventArgs e )
		{
			if( FormattedTextChanged != null )
				FormattedTextChanged( this, e );
		}

		/// <summary>
		/// Indicates whether the Undo operation is possible at this time.
		/// </summary>
		/// <remarks>
		/// The NumberTextBox maintains its own undo mechanism and hence
		/// needs to implement this method to be compatible with the Undo
		/// mechanism.
		/// </remarks>
		public new bool CanUndo
		{
			get
			{
				return this.undoBufferText != this.GetTextBoxText() && !this.ReadOnly;
			}
		}


		#endregion

		#region OPERATIONS
		/// <summary>
		/// Handles the Context popup.
		/// </summary>
		/// <param name="sender">The context menu.</param>
		/// <param name="ea"></param>
		private void HandleContextMenuPopup( object sender, EventArgs ea )
		{
			this.Focus();

			miUndo.Enabled = this.CanUndo;

			bool bEnabled = ( this.SelectionLength > 0 );

			if( !this.Enabled || this.ReadOnly )
			{
				miCut.Enabled = false;
				miPaste.Enabled = false;
				miDelete.Enabled = false;
				miCopy.Enabled = bEnabled;
			}
			else
			{
				miCut.Enabled = bEnabled;
				miCopy.Enabled = bEnabled;
				miDelete.Enabled = ( this.Text.Length > 0 );

				DataObject dobject = (DataObject)Clipboard.GetDataObject();
				miPaste.Enabled = dobject.GetDataPresent( typeof( string ) );
			}

			miSelectAll.Enabled = ( this.SelectionLength != this.Text.Length );
		}

		/// <summary>
		/// Handles the Undo menu click.
		/// </summary>
		/// <param name="sender">The undo menu.</param>
		/// <param name="ea">The event data.</param>
		private void HandleMenuUndoClick( object sender, EventArgs ea )
		{
			this.Undo();
			this.ClearUndo();
		}

		/// <summary>
		/// Handles the Cut menu.
		/// </summary>
		/// <param name="sender">Cut menu.</param>
		/// <param name="ea">The event data.</param>
		private void HandleMenuCutClick( object sender, EventArgs ea )
		{
			this.Cut();
		}

		/// <summary>
		/// Handles the Copy menu.
		/// </summary>
		/// <param name="sender">Copy menu.</param>
		/// <param name="ea">The event data.</param>
		private void HandleMenuCopyClick( object sender, EventArgs ea )
		{
			this.Copy();
		}

		/// <summary>
		/// Handles the Delete menu.
		/// </summary>
		/// <param name="sender">Delete menu.</param>
		/// <param name="ea">The event data.</param>
		private void HandleMenuDeleteClick( object sender, EventArgs ea )
		{
			this.Delete();
		}

		/// <summary>
		/// Handles the Select All menu.
		/// </summary>
		/// <param name="sender">Select All menu.</param>
		/// <param name="ea">The event data.</param>
		private void HandleMenuSelectAllClick( object sender, EventArgs ea )
		{
			this.SelectAll();
		}

		/// <summary>
		/// Handles the Paste menu.
		/// </summary>
		/// <param name="sender">Paste menu.</param>
		/// <param name="ea">The event data.</param>
		private void HandleMenuPasteClick( object sender, EventArgs ea )
		{
			this.Paste();
		}

		/// <summary>
		/// This method will Undo the previous operation.
		/// </summary>
		/// <remarks>
		/// This method is invoked when the Undo context menu item is
		/// clicked or the shortcut (CTRL+Z) is clicked. You can override
		/// this method to provide your own implementation for Undo.
		/// </remarks>
		public virtual new void Undo()
		{
			if( this.CanUndo == false )
				return;

			this.redoBufferText = this.GetTextBoxText();
			this.Text = this.undoBufferText;
			this.SelectionStart = this.GetFirstDataPos();
		}

		/// <summary>
		/// Selects all text in the text box.
		/// </summary>
		/// <remarks>
		/// This method is called when the menu item "Select All" or its
		/// shortcut (CTRL+A) is used. This method overrides the base 
		/// implementation in <see cref="System.Windows.Forms.TextBoxBase.SelectAll"/>.
		/// Override this method to provide your own implementation for 
		/// SelectAll.
		/// </remarks>
		public virtual new void SelectAll()
		{
			base.SelectAll();
		}


		/// <summary>
		/// Pastes the data in the clipboard into the NumberTextBox control.
		/// </summary>
		/// <remarks>
		/// The data will be formatted before being pasted into the text box.
		/// </remarks>
		public virtual new void Paste()
		{
			if( this.ReadOnly == false )
			{
				try
				{
					string clipBoardData = String.Empty;

					// Create a new instance of the DataObject interface.
					IDataObject data = Clipboard.GetDataObject();
					// If the data is text, then set the text.
					if( data.GetDataPresent( DataFormats.Text ) )
					{
						clipBoardData = data.GetData( DataFormats.Text ).ToString();

						if( clipBoardData != null )
						{
							bool b = this.InsertString( this.GetTextBoxText(), this.SelectionStart, this.SelectionLength, clipBoardData, true );
							if( b )
								this.SetPreserveData( false );
						}
					}
				}
				catch( Exception ex )
				{
					TraceUtil.TraceExceptionCatched( ex );
					ExceptionManager.RaiseExceptionCatched( this, ex );
				}
			}
		}


		/// <summary>
		/// Copies the content of the NumberTextBox to the clipboard.
		/// The ClipMode property dictates what gets copied.
		/// </summary>
		/// <remarks>
		/// If the text of the control is $56,000.12, this is the content
		/// that will be copied to the clipboard based on whether the ClipMode
		/// is set to <see cref="CurrencyClipModes.IncludeFormatting"/> or 
		/// <see cref="CurrencyClipModes.ExcludeFormatting"/>.
		/// <para>
		/// IncludeFormatting - $56,000.12
		/// ExcludeFormatting - 56000.12
		/// </para>
		/// </remarks>
		public virtual new void Copy()
		{
			if( this.ClipMode == CurrencyClipModes.IncludeFormatting )
			{
				base.Copy();
				return;
			}

			string clipText = this.ClipText;
			Clipboard.SetDataObject( clipText );
		}


		/// <summary>
		/// Cuts the selected data to the clipboard.
		/// </summary>
		/// <remarks>
		/// The selected text in the CurrencyTextBox will be deleted and the content
		/// will be copied to the clipboard.
		/// </remarks>
		public virtual new void Cut()
		{
			if( this.ReadOnly == false )
			{
				this.Copy();
				bool handled = this.HandleDeleteKey();
				if( handled )
					this.SetPreserveData( false );
			}
			return;
		}

		/// <summary>
		/// Deletes the current selection of the text box.
		/// </summary>
		/// <remarks>
		/// This method is invoked by the ContextMenu "Delete" menu item of the
		/// text box through the "Del" shortcut or by selecting the menu item.
		/// If you want to override the default behavior of this method, derive
		/// and override this method to provide your own implementation.
		/// </remarks>
		public virtual void Delete()
		{
			bool handled = this.HandleDeleteKey();
			if( handled )
				this.SetPreserveData( false );
		}

		#endregion

		#region INTERNAL OPERATIONS

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.editMenu = null;
				this.numberFormatInfoObject = null;
				this.nullFormat = null;
                UnWireEvents();
			}
			base.Dispose( disposing );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void FormatChanged( string currentText, NumberFormatInfo previousFormat )
		{
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string ToggleNegative( string currentText )
		{
			return currentText;
		}

		[Browsable( false )]
		[EditorBrowsable( EditorBrowsableState.Never )]
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool IsNegative
		{
			get
			{
				return this.isNegative;
			}

			set
			{
				this.isNegative = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool GetZeroNegative()
		{
			return this.zeroNegative;
		}

        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool CheckIsZero()
        {
            return false;
        }
        
        [Syncfusion.Documentation.DocumentationExclude()]
		protected void SetNegativeInputPending( bool negativeInputPending )
		{
			this.negativeInputPending = negativeInputPending;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool GetNegativeInputPending()
		{
			return this.negativeInputPending;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetZeroNegative( bool zeroNegative )
		{
			this.zeroNegative = zeroNegative;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string GetNumberValue( string formattedText, int startPosition )
		{
			return formattedText;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual int GetStartPositionJustNumbers( string currentText, int startPosition )
		{
			int startPositionJustNumbers = 0;

			// We keep a count of the number of valid digits before the
			// decimal character before the text is inserted - this will
			// be used when we want to position the cursor after the 
			// text insertion. 
			// For example, if the current content of the text box is 
			// $123,456.78 and the start position is at the third position,
			// (before the character 2), the count of numbers before insert
			// will be one.
			for( int i = 0; i < startPosition; i++ )
			{
				if( Char.IsDigit( currentText[i] ) )
					startPositionJustNumbers++;
			}

			return startPositionJustNumbers;
		}

		/// <summary>
		/// The NumberTextBox accepts numeric digits only. This method checks
		/// for numeric input.
		/// </summary>
		/// <param name="inputCharacter">The character to be checked.</param>
		/// <returns>True if the character is valid; false otherwise.</returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool IsValidCharacter( char inputCharacter )
		{
			bool returnValue = false;
			if( ( inputCharacter >= '0' && inputCharacter <= '9' ) )
				returnValue = true;
			else
				returnValue = false;

			return returnValue;
		}

		/// <summary>
		/// Overloaded. Override this method to provide an implementation for parsing
		/// a negative value.
		/// </summary>
		/// <param name="currentText">The text to be parsed.</param>
		/// <returns>True if the value is negative; false otherwise.</returns>
		protected virtual bool ParseForNegativeFormat( string currentText )
		{
			return ParseForNegativeFormat( currentText, this.NumberFormatInfoObject );
		}

		/// <summary>
		/// Override this method to provide an implementation for parsing
		/// a negative value.
		/// </summary>
		/// <param name="currentText">The text to be parsed.</param>
		/// <returns>True if the value is negative; false otherwise.</returns>
		protected virtual bool ParseForNegativeFormat( string currentText, NumberFormatInfo info )
		{
			return false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected bool CheckIfNegative( string rawValue )
		{
			return false;
		}

		/// <summary>
		/// Sets the start position to value passed and sets the
		/// SelectionLength to be zero.
		/// </summary>
		/// <param name="selectionStart">The start of the selection.</param>
		public void SetEmptySelection( int selectionStart )
		{
            if(selectionStart != -1)
			    this.SelectionStart = selectionStart;
			this.SelectionLength = 0;

			/*int num1 = (start + length);
			if (base.IsHandleCreated)
			{
				base.SendMessage(177, start, num1);
				return;
 
			}
			this.selectionStart = start;
			this.selectionLength = length;*/
		}
		/// <summary>
		/// Returns the first data position which is the index within the current
		/// string content of the edit control at which data can be 
		/// inserted.
		/// </summary>
		/// <returns>The index of the first possible data position.</returns>
		protected int GetFirstDataPos()
		{
			string formattedText = this.GetTextBoxText();
			if( formattedText == null || formattedText.Length == 0 )
				return 0;

			int i = 0;
			while( i < formattedText.Length && !Char.IsDigit( formattedText[i] ) )
				i++;

			return i;
		}

		/// <summary>
		/// Returns the next valid position for receiving data input.
		/// </summary>
		/// <param name="startPosition">The position to start seeking from.</param>
		/// <param name="inDecimalPosition">Indicates whether the start position is after the decimal separator.</param>
		/// <param name="back">Indicates whether the backspace key has been pressed.</param>
		/// <returns>The index in the string that is valid for input.</returns>
		protected int GetNextDataPos( int startPosition, bool inDecimalPosition, bool back, bool delete )
		{
			string formattedText = this.GetTextBoxText();
			if( formattedText==null || formattedText.Length==0 )
				return 0;

			int i = Math.Max( startPosition, 0 );

			//int decimalSeparatorPosition = formattedText.IndexOf(this.NumberFormatInfoObject.CurrencyDecimalSeparator);
			int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( formattedText );
			if( startPosition == decimalSeparatorPosition )
			{
				if( inDecimalPosition )
				{
					if( back )
						return startPosition;
				}
				else
					return startPosition;
			}

			while( i < formattedText.Length && !Char.IsDigit( formattedText[i] ) )
				i++;

			if( inDecimalPosition == true && back == false && delete == false )
				i++;

			return Math.Min( formattedText.Length, i );
		}

		/// <summary>
		/// Overloaded. Returns the position of the decimal separator taking into account
		/// the CurrencySymbol.
		/// </summary>
		/// <param name="currentText">The text in which to look for the decimal separator.</param>
		/// <returns>The decimal separator's position.</returns>
		virtual protected int GetDecimalSeparatorPosition( string currentText )
		{
			return this.GetDecimalSeparatorPosition( currentText, this.NumberFormatInfoObject );
		}

		/// <summary>
		/// Returns the position of the decimal separator taking into account
		/// the CurrencySymbol.
		/// </summary>
		/// <param name="currentText">The text in which to look for the decimal separator.</param>
		/// <returns>The decimal separator's position.</returns>
		virtual protected int GetDecimalSeparatorPosition( string currentText, NumberFormatInfo info )
		{
			int decimalPosition = 0;
			decimalPosition = currentText.IndexOf( this.GetDecimalSeparator( info ) );
			return decimalPosition;
		}

		/// <summary>
		/// Returns the previous valid data position for text input.
		/// </summary>
		/// <param name="startPosition">The position to seek from.</param>
		/// <returns></returns>
		public int GetPrevDataPos( int startPosition )
		{
			string formattedText = this.GetTextBoxText();

			if( formattedText ==null ||  formattedText.Length==0 )
				return 0;

			if( startPosition >= formattedText.Length )
				return formattedText.Length;

			int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( formattedText );

			int i = Math.Min( startPosition, formattedText.Length-1 );

			// Return the position just before the first digit 
			// from the end of the number.
			while( i>0 && !Char.IsDigit( formattedText[i] ) )
				i--;

			if( i == decimalSeparatorPosition )
				i--;

			return Math.Max( 0, i );
		}


		/// <summary>
		/// Overloaded. Override this to return DecimalSeparator in NumberTextBox.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string GetDecimalSeparator()
		{
			return this.GetDecimalSeparator( this.NumberFormatInfoObject );
		}

		/// <summary>
		/// Override this to return DecimalSeparator in NumberTextBox.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string GetDecimalSeparator( NumberFormatInfo info )
		{
			return info.NumberDecimalSeparator;
		}

		/// <summary>
		/// Overloaded. Override this to return GroupSeparator in NumberTextBox.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string GetGroupSeparator()
		{
			return this.GetGroupSeparator( this.NumberFormatInfoObject );
		}

		/// <summary>
		/// Override this to return GroupSeparator in NumberTextBox.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected string GetGroupSeparator( NumberFormatInfo info )
		{
			return info.NumberGroupSeparator;
		}


		/// <summary>
		/// Deletes the currently selected text.
		/// </summary>
		/// <returns>The content of the text box after deletion.</returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected string DeleteSelectedText( string formattedText, int startPosition, int endPosition )
		{
			if( endPosition > startPosition )
			{
				int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( formattedText );

				if( endPosition > decimalSeparatorPosition && decimalSeparatorPosition >= startPosition )
				{
					if( startPosition == 0 && formattedText.Length == endPosition )
						formattedText = "";
					else
						formattedText = formattedText.Substring( 0, startPosition ) + this.GetDecimalSeparator() + formattedText.Substring( endPosition );
				}
				else
				{
					formattedText = formattedText.Substring( 0, startPosition ) + formattedText.Substring( endPosition );
				}
			}
			return formattedText;
		}

		/// <summary>
		/// Deletes the currently selected text.
		/// </summary>
		/// <returns>The content of the text box after deletion.</returns>
		protected string DeleteSelectedText()
		{
			return this.DeleteSelectedText( this.GetTextBoxText(), this.SelectionStart, this.SelectionStart+this.SelectionLength );
		}

		/// <summary>
		/// Returns the first data position that can take valid input.
		/// </summary>
		/// <param name="startPosition">The start position to seek from.</param>
		/// <returns>The index of the first valid position.</returns>
		private int GetFirstFillPos( int startPosition )
		{
			int n = Math.Max( GetFirstDataPos(), GetPrevDataPos( startPosition ) );
			return n;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected int GetDecimalDigits()
		{
			return this.NumberFormatInfoObject.NumberDecimalDigits;
		}


		/// <summary>
		/// Checks if the text can be inserted subject to the constraints.
		/// </summary>
		/// <param name="currentText">The current content of the text box.</param>
		/// <param name="inputText">The text to be inserted.</param>
		/// <returns>The length of the acceptable string to be inserted.</returns>
		protected virtual int CanInsert( string currentText, string inputText, int initialZeroCount )
		{
			return inputText.Length;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newText"></param>
		/// <returns></returns>
		protected virtual bool CheckForMinMax( string newText )
		{
			return this.CheckForMinMax( newText, false );
		}

		/// <summary>
		/// Indicates whether the given value meets the minimum and maximum value considerations.
		/// </summary>
		/// <param name="newText"></param>
		/// <param name="ignoreLength"></param>
		/// <returns></returns>
		protected virtual bool CheckForMinMax( string newText, bool ignoreLength )
		{
            return true;
		}
        /// <summary>
        /// Checks whether the NullSring Value is with in the Min Max Values
        /// </summary>
        /// <param name="nullString"></param>
        /// <returns>True if it is in range else false</returns>
        /// <Note>override this method to check the nullString is in range by parsing it to respective Type </Note>
        /// <example>
        /// Double doubleVal;
        /// bool isNumber = Double.TryParse(currentTextValue, out doubleVal);
        /// if (isNumber)
        /// {
        ///     return CheckForMinMax(doubleVal.ToString(),true);
        /// }
        /// else
        ///     return true;
        /// </example>
        protected virtual bool CheckNullStringIsInRange(string nullString)
        {
            return true;
        }
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int GetNumberPartLength( string numberText )
		{
			int numberPartLength = 0;
			int decimalSeparator = numberText.IndexOf( this.GetDecimalSeparator() );
			if( decimalSeparator != -1 )
				numberPartLength = decimalSeparator;
			else
				numberPartLength = numberText.Length;

			return numberPartLength;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected bool InsertString( string currentText, int startPosition, int selectionLength, string textToBeInserted )
		{
			return this.InsertString( currentText, startPosition, selectionLength, textToBeInserted, false );
		}

		/// <summary>
		/// Inserts a string into the textbox at the current position.
		/// The string data will be parsed for valid numeric data and 
		/// only the valid characters will be accepted.
		/// </summary>
		/// <param name="textToBeInserted">The string to be inserted.</param>
		/// <param name="pasteOperation">Paste and Text properties will be treated differently for validation.</param>
		/// <returns>True if the operation succeeds.</returns>
		/// <remarks>
		/// This method attempts to insert the text passed in as the parameter into
		/// the NumberTextBox subject to the constraints imposed by the other
		/// attributes of the NumberTextBox such as the maximum length of a 
		/// the string etc.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected bool InsertString( string currentText, int startPosition, int selectionLength, string textToBeInserted, bool pasteOperation )
		{
			NumberModifyState state = this.PrepareInsertString( currentText, startPosition, selectionLength, textToBeInserted, pasteOperation );
			return this.CompleteInsertString( state );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual NumberModifyState PrepareInsertString( string currentText, int startPosition, int selectionLength, string textToBeInserted, bool pasteOperation )
		{
			NumberModifyState state = new NumberModifyState();
			string newText			= String.Empty;
			string modifiedRawText	= String.Empty;
			string inputRawText		= String.Empty;
			int initialZeroCount	= 0;

			state.moveBack			= false;
			state.initialNegativeState = this.IsNegative;

			if(!this.NegativeSign.Equals(string.Empty) &&  textToBeInserted.IndexOf( this.NegativeSign ) == 0 )
			{
				state.isNegative = true;
				this.IsNegative = true;
			}
			else if( startPosition == 0 && selectionLength == currentText.Length )
			{
				state.isNegative = false;
				this.IsNegative = false;
			}

			// Remove the formatting and keep just the numbers.
			inputRawText = this.RemoveFormatting( textToBeInserted );

			state.startPositionJustNumbers = this.GetStartPositionJustNumbers( currentText, startPosition );

			initialZeroCount = this.GetInitialZeroCount( currentText, startPosition );
			state.inDecimalPosition = this.IsInDecimalPosition( currentText, startPosition );

			// If any text is selected, first delete it and get the text.
			currentText = DeleteSelectedText( currentText, startPosition, startPosition+selectionLength/*this.SelectionLength*/);
            
			// Check if a character can be inserted in this position.
			// We check after deleting any selected text - so this should
			// never be false if a character or more was deleted.
			int acceptableLength = this.CanInsert( currentText, inputRawText, initialZeroCount );

			// Check for minimum and maximum values.
			string checkText = currentText.Substring( 0, startPosition )+ inputRawText + currentText.Substring( startPosition );

			bool bMinMaxCheck = true;
			bool useOverWrittenText = false;
			bool needReCheck = false;

			do
			{
				if (OverWriteText && needReCheck && (startPosition + 1) <= currentText.Length)
				{
					useOverWrittenText = true;
					checkText = currentText.Substring(0, startPosition) + inputRawText + currentText.Substring(startPosition + 1);
				}

				// Apply ZeroNegative and NegativeInputPending before checking for Min and Max.
				if (this.GetZeroNegative() == true || this.GetNegativeInputPending())
				{
					checkText = "-" + checkText;
					this.IsNegative = true;
					bMinMaxCheck = this.CheckForMinMax(checkText, true);
				}
				else
					bMinMaxCheck = this.CheckForMinMax(checkText, pasteOperation);

				if (OverWriteText && !needReCheck && !bMinMaxCheck && MinMaxValidation == MinMaxValidation.OnKeyPress)
					needReCheck = true;
				else
					needReCheck = false;
			}
			while (needReCheck);

			// Check if we can proceed further or if we need to rollback.
			if( this.rollBackOperation == true )
			{
				this.rollBackOperation = false;
				state.success = false;
				this.IsNegative = state.initialNegativeState;
			}
			else
			{
				if( this.GetNegativeInputPending() )
				{
					this.IsNegative = true;
					this.SetNegativeInputPending( false );
				}

				if( this.GetZeroNegative() == true&& Int32.Parse( inputRawText ) != 0 )
				{
					this.IsNegative = true;
					this.SetZeroNegative( false );
				}

				if( bMinMaxCheck == false )
				{
					if( this.CausesValidation == true )
						this.RaiseValidationError( checkText, startPosition, "Cannot change value as minimum / maximum conditions will not be met." );

					state.success = false;
					return state;
				}

				if( acceptableLength == 0 )
				{
					if( this.CausesValidation == true )
					{
						string invalidText = currentText.Substring( 0, startPosition )+ inputRawText + currentText.Substring( startPosition );
						this.RaiseValidationError( invalidText, startPosition );
					}
					state.success = false;
					return state;
				}
				else if( acceptableLength < GetNumberLengthWithoutMinus( inputRawText ) )
				{
					if( this.CausesValidation == true )
					{
						string invalidText = currentText.Substring( 0, startPosition )+ inputRawText + currentText.Substring( startPosition );
						this.RaiseValidationError( invalidText, startPosition );
					}

					// Truncate the raw text if its too long.
					inputRawText = inputRawText.Substring( 0, acceptableLength );
				}

				// The modified text.
				if (useOverWrittenText && bMinMaxCheck)
					modifiedRawText = ConcatModifiedText( currentText.Substring( 0, startPosition ), inputRawText, currentText.Substring( startPosition + 1 ) );
				else
					modifiedRawText = ConcatModifiedText(currentText.Substring(0, startPosition), inputRawText, currentText.Substring(startPosition));
				modifiedRawText = this.RemoveFormatting( modifiedRawText );

				if( state.inDecimalPosition )
					modifiedRawText = this.CheckDecimalPartLength( modifiedRawText );

				if( this.IsValidNumberValue( modifiedRawText ) )
				{
                   
					state.changedValue = this.GetNumberValue( modifiedRawText, state.startPositionJustNumbers );
                    state.changedString = ApplyFormatting(state.changedValue);
               		if( state.inDecimalPosition == false )
						state.startPositionJustNumbers = state.startPositionJustNumbers + inputRawText.Length - initialZeroCount;
				}
			}

			return state;
		}

		/// <summary>
		/// Concat modified text.
		/// </summary>
		/// <param name="startCurrentText"></param>
		/// <param name="inputRawText"></param>
		/// <param name="endCurrentText"></param>
		/// <returns></returns>
		protected virtual string ConcatModifiedText( string startCurrentText, string inputRawText, string endCurrentText )
		{
			return ( startCurrentText + inputRawText + endCurrentText );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool CompleteInsertString( NumberModifyState state )
		{
			if( state.success )
			{
				//if(this.GetZeroNegative() == true)
				//{
				//    this.IsNegative = true;
				//    this.SetZeroNegative(false);
				//}

				if( this.IsNull)
				{
					state.changedString = ApplyFormatting( state.changedValue );
				}

				this.SetModifiedText( state.changedString, state.changedValue );
				this.PositionCursorAfterEdit( state.startPositionJustNumbers, state.inDecimalPosition, state.moveBack, false );
			}
			else
				this.IsNegative = state.initialNegativeState;

			return state.success;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		private int GetNumberLengthWithoutMinus( string incomingText )
		{
			if( incomingText.IndexOf( '-' ) != -1 )
				return incomingText.Length -1;
			else
				return incomingText.Length;
		}

		/// <summary>
		/// Overrides OnTextChanged.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnTextChanged( EventArgs e )
		{
			UpdateNullState();

			if( ignoreTextChange == false )
			{
				base.OnTextChanged( e );
				this.OnClipTextChanged( e );
				this.OnFormattedTextChanged( e );
			}
			this.SetNegativeInputPending( false );
		}

		/// <summary>
		/// Checks the length of the decimal part of the text box content.
		/// </summary>
		/// <param name="rawText">The input raw text string.</param>
		/// <returns>The checked string.</returns>
		protected string CheckDecimalPartLength( string rawText )
		{
			int decimalPosition = 0;
			string decimalString = String.Empty;
			string checkedString = rawText;

			decimalPosition = this.GetDecimalSeparatorPosition( rawText );
			if( decimalPosition > 0 )
			{
				decimalString = rawText.Substring( decimalPosition+1, rawText.Length - decimalPosition - this.GetDecimalSeparator().Length );
				if( decimalString.Length > this.GetDecimalDigits() )
				{
					int diff = decimalString.Length - this.GetDecimalDigits();
					checkedString = rawText.Remove( rawText.Length -diff, diff );
				}
			}

			return checkedString;
		}

		/// <summary>
		/// Returns the count of zeros at the beginning of the string. This is 
		/// needed as we will have to compensate for this once a valid
		/// number appears after it for repositioning the cursor.
		/// </summary>
		/// <param name="currentText"></param>
		/// <param name="startPosition"></param>
		/// <returns></returns>
		virtual protected int GetInitialZeroCount( string currentText, int startPosition )
		{
			bool firstDigitHit = false;
			int initialZeroCount = 0;

			for( int k = 0; k < currentText.Length; k++ )
			{
				if( Char.IsDigit( currentText[k] ) || firstDigitHit == true )
				{
					if( currentText[k] == '0' )
					{
						initialZeroCount++;
						firstDigitHit = true;
					}
					else
						break;
				}
			}

			return initialZeroCount;
		}

		/// <summary>
		/// Indicates whether the start position is after the decimal separator.
		/// </summary>
		/// <param name="currentText">The current text.</param>
		/// <param name="startPosition">The start position to seek for.</param>
		/// <returns></returns>
		virtual protected bool IsInDecimalPosition( string currentText, int startPosition )
		{
			bool isInDecimalPosition = false;
			int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( currentText );

			if( decimalSeparatorPosition != -1 && startPosition > decimalSeparatorPosition )
			{
				isInDecimalPosition = true;
			}

			return isInDecimalPosition;
		}


		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool IsValidNumberValue( string doubleString )
		{
			return true;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSetNull( SetNullEventArgs e )
		{
			try
			{
				if( this.SetNull != null )
					this.SetNull( this, e );
			}
			catch
			{
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnKeyValidate( KeyValidateEventArgs e )
		{
			try
			{
				if( this.KeyValidate != null )
					this.KeyValidate( this, e );
			}
			catch
			{
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool RaiseSetNull( object nullValue )
		{
			SetNullEventArgs args = new SetNullEventArgs( nullValue );
			this.OnSetNull( args );
			return args.Cancel;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool RaiseKeyValidate( char key, string changedString, string changedValueString )
		{
			KeyValidateEventArgs args = new KeyValidateEventArgs( key, changedString, changedValueString );
			this.OnKeyValidate( args );
			return !args.Cancel;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal virtual void OnNullStringChanged(bool isNull)
		{
			if(isNull && this.AllowNull)
			{
				this.SetNullNumberValue();
				SetTextBoxText( this.NullString );
			}
		}
        [Syncfusion.Documentation.DocumentationExclude()]
        internal virtual void OnAllowNullChanged()
        {
            if (this.IsNull)
            {
                if (this.AllowNull)
                {
                    this.SetNullNumberValue();
                    SetTextBoxText(this.NullString);
                }
                else
					//Setting the DefaultValue Can break the Min Max Value Override OnAllowNullChanged() accordingly
					this.ApplyFormattingAndSetText(this.DefaultValue.ToString());
            }
        }
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		private void UpdateNullState()
		{
			//this.NullState = this.UseNullString && IsNullOrEmpty( this.Text );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private bool IsNullOrEmpty( string text )
		{
			return ( text == null ) || ( text.Length == 0 ) || text.Equals( this.NullString );
		}
		#endregion

		#region KEY HANDLING

		/// <summary>
		/// This method overrides the <see cref="System.Windows.Forms.Control.ProcessKeyMessage"/> method
		/// and handles the key messages that are of interest to the NumberTextBox.
		/// </summary>
		/// <param name="m">The message that is to handled.</param>
		/// <returns>True if the key message is handled; false otherwise.</returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override bool ProcessKeyMessage( ref Message m )
		{
			if (this.ReadOnly)
				return base.ProcessKeyMessage( ref m );

			NumberModifyState state = null;

			int keyData = (int)m.WParam | (int)ModifierKeys;
			bool handled = true;

            /*if (this.MaxLength > 0 && this.SelectionLength == 0  && GetNumberLengthWithoutMinus(this.Text) >= this.MaxLength && keyData != 8 && keyData != 45 && !((NegativeSign.Length == 1 && (char)keyData == NegativeSign[0]) ||
                           (NegativeSign.Length != 1 && (char)keyData == '-')))
            {
                return base.ProcessKeyMessage(ref m);
            }*/


			if( m.Msg == 0x102 ) //winw.WM_CHAR
			{
				supressKeyPress = true;

				// Check for menu shortcuts.
				if( m.WParam == (IntPtr)0x3 )// CtrlC ^C
				{
					this.Copy();
					return base.ProcessKeyMessage( ref m );
				}
				else if( m.WParam == (IntPtr)0x16 )// CtrlV ^V
				{
					this.Paste();
					return base.ProcessKeyMessage( ref m );
				}
				else if( m.WParam == (IntPtr)0x1A )// CtrlZ ^Z
				{
					this.Undo();
					return base.ProcessKeyMessage( ref m );
				}
				else if( m.WParam == (IntPtr)0x18 )// CtrlX ^X
				{
					this.Cut();
					return base.ProcessKeyMessage( ref m );
				}
				else if( m.WParam == (IntPtr)0x1 )// CtrlA ^A
				{
					if( m.LParam == (IntPtr)0x1e0001 )
						this.SelectAll();
					else
						this.Paste();
					return base.ProcessKeyMessage( ref m );
				}

				if( ( this.GetDecimalSeparator().Length > 0 ) && (char)keyData == this.GetDecimalSeparator()[0] )
				{
					handled = this.HandleDecimalKey();
					if( !handled )
						this.supressKeyPress = false;
				}
				else if( ( NegativeSign.Length == 1 && (char)keyData == NegativeSign[0] ) ||
                          ( NegativeSign.Length != 1 && (char)keyData == '-' ) )
				{
					state = this.HandleSubtractKey();
					if( RaiseKeyValidate( (char)keyData, state.changedString, state.changedValue ) == false )
						state.success = false;
					handled = this.CompleteSubtractKey( state );
					if( state.success == false )
						return base.ProcessKeyMessage( ref m );
				}
				else if( IsValidCharacter( (char)keyData ) )
				{

					handled = this.HandleCharacterKey( (char)keyData );
					if( handled == false )
						return base.ProcessKeyMessage( ref m );
				}
				else if( keyData == 8 )
				{
					handled = this.HandleBackspaceKeyChar();
					if( !handled )
						this.supressKeyPress = false;
				}
				if( ( this.GetGroupSeparator().Length > 0 ) && (char)keyData== this.GetGroupSeparator()[0] )
				{
					handled = this.HandleDecimalKey();
					if( !handled )
						this.supressKeyPress = false;
				}

				if( this.CausesValidation == true && handled==false )
				{
					try
					{
						string invalidText = this.GetTextBoxText().Substring( 0, this.SelectionStart )+ (char)keyData + this.GetTextBoxText().Substring( this.SelectionStart );
						if( keyData != 8 )//Backspace
							this.RaiseValidationError( invalidText, this.SelectionStart, String.Format( "Invalid key entered {0}", keyData ) );
					}
					catch
					{
					}
				}

				if( handled == true )
				{
					m.Result = (IntPtr)3;
					this.SetPreserveData( false );
				}

				return base.ProcessKeyMessage( ref m );
			}
			else if( m.Msg == 0x100 ) //winw.WM_KEYDOWN
			{
				int modifier = (int)ModifierKeys;
				if( modifier != (int)Keys.Shift )
				{
					switch( keyData & (int)Keys.KeyCode )
					{
						case (int)Keys.Back:
						handled = this.HandleBackspaceKey();
						if( handled == true )
						{
							this.supressKeyDown = true;
							this.SetPreserveData( false );
						}
						return base.ProcessKeyMessage( ref m );

						case (int)Keys.Delete:
						handled = this.HandleDeleteKey();
						if( handled == true )
						{
							this.supressKeyDown = true;
							this.SetPreserveData( false );
						}
						return base.ProcessKeyMessage( ref m );

						default:
						break;
					}
				}
			}
			return base.ProcessKeyMessage( ref m );
		}

		/// <summary>
        /// Overrides to suppress KeyPress. Only the KeyPress event is raised - no other
		/// processing is done.
		/// </summary>
		/// <param name="m">The message.</param>
		/// <returns>True if the message is a KeyPress; otherwise the base class handles this.</returns>
		protected override bool ProcessKeyEventArgs( ref Message m )
		{
			if( this.supressKeyPress == true )
			{
				int keyData = (int)m.WParam | (int)ModifierKeys;
				this.supressKeyPress = false;
				this.OnKeyPress( new KeyPressEventArgs( (char)keyData ) );
				m.Result = (IntPtr)3;
				return true;
			}
			else if( this.supressKeyDown == true )
			{
				int keyData = (int)m.WParam | (int)ModifierKeys;
				this.supressKeyDown = false;
				this.OnKeyDown( new KeyEventArgs( (Keys)keyData ) );
				m.Result = (IntPtr)3;
				return true;
			}
			else
				return base.ProcessKeyEventArgs( ref m );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected bool HandleBackspaceKeyChar()
		{
			return true;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetPreserveData( bool preserveData )
		{
			this.preserveData = preserveData;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool GetPreserveData()
		{
			return this.preserveData;
		}

		/// <summary>
		/// Handles the backspace key.
		/// </summary>
		/// <returns>True if the key was accepted and the action performed; false otherwise.</returns>
		/// <remarks>
		/// The backspace key results in one character being removed in front of the
		/// current selection if the selection is empty or deletion of the selection if the
		/// selection is not empty.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected bool HandleBackspaceKey()
		{
			if( this.ReadOnly == true )
				return false;

			if (this.SelectionStart == 0 && this.SelectionLength == 0)
				return true;
			int startPosition = this.SelectionStart;
			string currentText = this.GetTextBoxText();
			string modifiedRawText = String.Empty;
			int startPositionJustNumbers = 0;
			bool deleteInDecimalPosition = false;
			int realSelectionLength = 0;

			startPositionJustNumbers = this.GetStartPositionJustNumbers( currentText, startPosition );

			// Find if the cursor is after the decimal separator.
			int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( currentText );
			deleteInDecimalPosition = this.IsInDecimalPosition( currentText, startPosition );
			if( startPosition == decimalSeparatorPosition+this.GetDecimalSeparator().Length )
				deleteInDecimalPosition = false;

			// Change the text and put it in the control.
			if( this.SelectionLength == 0 )
			{
				realSelectionLength = 0;
				int oldStartPosition = startPosition;
				startPosition = GetPrevDataPos( startPosition-1 );
				this.SelectionStart = startPosition;
				this.SelectionLength = oldStartPosition - startPosition;
			}
			else
				realSelectionLength = this.SelectionLength;

			if( startPosition > 0 && realSelectionLength == 0 )
			{
				startPositionJustNumbers--;
			}

			currentText = DeleteSelectedText();

			if( CheckForMinMax( currentText ) )
			{
				// Check for NULL state.
				if( IsNullOrEmpty( currentText ) )
				{
					if( this.AllowNull && !RaiseSetNull( currentText ) )
					{
						this.SetNullNumberValue();
						this.SetTextBoxText( this.NullString );
					}
					else
					{
						string text = this.DefaultValue.ToString();
						if( this.IsValidNumberValue( text ) )
						{
							this.ApplyFormattingAndSetText( this.GetNumberValue( text, startPosition ) );
							this.PositionCursorAfterEdit( startPositionJustNumbers, deleteInDecimalPosition, true, false );
						}
					}
				}
                else if (this.Text != this.NullString && this.IsValidNumberValue(RemoveFormatting(currentText)))
				{
					string text = RemoveFormatting( currentText.Substring( 0, startPosition ) + currentText.Substring( startPosition ) );
					if( this.IsValidNumberValue( text ) )
					{
						this.ApplyFormattingAndSetText( this.GetNumberValue( text, startPosition ) );
						this.PositionCursorAfterEdit( startPositionJustNumbers, deleteInDecimalPosition, true, false );
					}
				}
			}
            this.SetControlColor();
			return true;
		}

		/// <summary>
		/// Invoked when a character key is pressed.
		/// </summary>
		/// <param name="charToBeInserted">The character to be inserted.</param>
		/// <returns>True if the insert was successful; false otherwise.</returns>
		/// <remarks>
		/// Character entry is accepted if the character is a valid digit. This
		/// method invokes the InsertString method to insert the character into
		/// the CurrencyTextBox.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected bool HandleCharacterKey( char charToBeInserted )
		{
			NumberModifyState state = this.PrepareInsertString( this.GetTextBoxText(), this.SelectionStart, this.SelectionLength, Convert.ToString( charToBeInserted ), false );
			if( RaiseKeyValidate( charToBeInserted, state.changedString, state.changedValue ) == false )
				state.success = false;

			if( m_bDecimalMode )
			{
				if( charToBeInserted == '0' )
				{
					if( this.NumberFormatInfoObject.CurrencyDecimalDigits - 1 > m_iDecimalDigitsPosition )
					{
						m_iDecimalDigitsPosition++;
					}
				}
				else
				{
					string changedValue = "0" + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
					for( int i = 0; i < m_iDecimalDigitsPosition; i++ )
					{
						changedValue += "0";
					}
					changedValue += charToBeInserted.ToString();
					state.startPositionJustNumbers += m_iDecimalDigitsPosition;

					if( this.isNegative )
					{
						changedValue = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NegativeSign + changedValue;
					}

					state.changedValue = changedValue;
					state.inDecimalPosition = true;
                    if (CheckForMinMax(state.changedValue))
                        state.success = true;
                    else
                        state.success = false;
                    
					m_iDecimalDigitsPosition = 0;
					m_bDecimalMode = false;
				}
			}

			this.CompleteInsertString( state );

			if( state.handled == false )
				this.supressKeyPress = false;
			return state.success;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool IsDataPosition( string formattedText, int position )
		{
			if( position >= formattedText.Length )
				return false;

			return Char.IsDigit( formattedText[position] );
		}

		/// <summary>
		/// When the start position is in the number part and there is only a zero, the cursor should be moved.
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="inDecimalPosition"></param>
		/// <returns></returns>
		protected virtual int CursorShouldBeMoved( int startPosition, bool inDecimalPosition )
		{
			return GetNextDataPos( startPosition+1, inDecimalPosition, false, false );
		}

		/// <summary>
		/// Invoked when the Delete key is pressed.
		/// </summary>
		/// <returns>True if the key is handle; false otherwise.</returns>
		/// <remarks>
		/// This method performs the action of removing the selected text when the 
		/// delete key was pressed and also positioning the cursor appropriately.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected bool HandleDeleteKey()
		{
			if( this.ReadOnly == true )
				return false;

			int startPosition = this.SelectionStart;
			string currentText = this.GetTextBoxText();
			int startPositionJustNumbers = 0;
			bool inDecimalPosition = false;

			startPositionJustNumbers = this.GetStartPositionJustNumbers( currentText, startPosition );

			inDecimalPosition = this.IsInDecimalPosition( currentText, startPosition );

			// If there is no selection, fake a selection from a starting point to another valid
			// position and then delete the fake selected text.
			if( this.SelectionLength == 0 )
			{
				// Special case - when the start position is in the number part and there is only 
				// a 0, the cursor should be moved to the next position.
				int prevDataPos = this.GetPrevDataPos( Math.Max( 0, startPosition-1 ) );
				if( inDecimalPosition == false && prevDataPos == this.GetFirstDataPos() )
				{
					if( currentText != String.Empty && currentText[prevDataPos] == '0' )
						startPosition = CursorShouldBeMoved( startPosition, inDecimalPosition );
				}

				// Special case - position is right before decimal separator - make sure first
				// character in decimal part is deleted.
				int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( currentText );
				if( startPosition == decimalSeparatorPosition )
				{
					while( startPosition < currentText.Length && !this.IsDataPosition( currentText, startPosition ) )
						startPosition++;
					inDecimalPosition = this.IsInDecimalPosition( currentText, startPosition );
				}

				// If the current position is not at a data position, move to the next data position.
				if( !IsDataPosition( currentText, startPosition ) )
					startPosition = GetNextDataPos( startPosition, inDecimalPosition, false, false );

				int newEndPosition = 0;
				if( inDecimalPosition )
					newEndPosition = GetNextDataPos( startPosition, inDecimalPosition, false, false );
				else
					newEndPosition = GetNextDataPos( startPosition+1, inDecimalPosition, false, false );
				this.SelectionStart = startPosition;
				this.SelectionLength = newEndPosition - startPosition;
			}

			currentText = DeleteSelectedText();

			if( CheckForMinMax( currentText ) )
			{
				// Check for NULL state.
				if( IsNullOrEmpty( currentText ) )
				{
					if( this.AllowNull && !RaiseSetNull( currentText ) )
					{
						this.SetNullNumberValue();
						this.SetTextBoxText( this.NullString );
					}
					else
					{
						string text = this.DefaultValue.ToString();
						if( this.IsValidNumberValue( text ) )
						{
							this.ApplyFormattingAndSetText( this.GetNumberValue( text, startPosition ) );
							this.PositionCursorAfterEdit( startPositionJustNumbers, inDecimalPosition, false, true );
						}
					}
				}
                else if (this.Text != this.NullString && this.IsValidNumberValue(RemoveFormatting(currentText)))
				{
					string text = RemoveFormatting( currentText.Substring( 0, startPosition ) + currentText.Substring( startPosition ) );
					if( this.IsValidNumberValue( text ) )
					{
						this.ApplyFormattingAndSetText( this.GetNumberValue( text, startPosition ) );
						this.PositionCursorAfterEdit( startPositionJustNumbers, inDecimalPosition, false, true );
					}
				}
			}
			this.SetControlColor();
			return true;
		}

		private bool negativeInputPendingOnSelectAll = false;

		/// <summary>
		/// This property defines the behavior when the contents of the TextBox is fully selected and the negative 
		/// key is pressed by the user. 
		/// If the value is set to True - The current value is not changed at all. The next key stroke is taken to be a 
		/// new value and the entire contents of the TextBox is replaced by the negative value of the key stroke character
		/// entered. Example: If the current value of the TextBox is 1.00 and all the text is selected and the user presses
		/// the -ve key followed by the key 5 - the value is -5.00
		/// If the value is set to False - The current value is changed to the negative value immediately. Example: If the current value of the TextBox is 1.00 and all the text is selected and the user presses
		/// the -ve key the value is -1.00
		/// </summary>
		[
        Description( "This property defines the behavior when the contents of the TextBox is fully selected and the negative key is pressed by the user." ),
        DefaultValue(false)
        ]
		public bool NegativeInputPendingOnSelectAll
		{
			get
			{
				return this.negativeInputPendingOnSelectAll;
			}

			set
			{
				this.negativeInputPendingOnSelectAll = value;
			}
		}

		/// <summary>
		/// Invoked when the negative key is pressed.
		/// </summary>
		/// <returns>True if the key is handled; false otherwise.</returns>
		/// <remarks>
		/// The defined behavior for this key is to toggle the sign (negativity)
		/// of the content of the NumberTextBox.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected NumberModifyState HandleSubtractKey()
		{
			NumberModifyState state = new NumberModifyState();
			bool allSelected = false;// Note 12/15/04 - this doesn't work - needs to be fixed

			// If the entire contents of the text box is selected, we set it to 
			// Negative number input pending mode.
			if( this.SelectionStart == 0 && this.SelectionLength == this.TextLength && this.NegativeInputPendingOnSelectAll )
			{
				this.SetNegativeInputPending( true );// This is used in HandleCharacterkey - Set back to False in TextChanged - should be set to False when any text is changed.
				state.handled = false;
				return state;
			}
			else
			{
				allSelected = true;
			}

			if( this.ReadOnly == true )
			{
				state.handled = false;
				return state;
			}

			state.initialNegativeState = this.IsNegative;

			int startPosition = this.SelectionStart;
			int startPositionJustNumbers = 0;
			string numberValue = String.Empty;
			string currentText = this.GetTextBoxText();
			bool inDecimalPosition = false;

			inDecimalPosition = this.IsInDecimalPosition( currentText, startPosition );
			startPositionJustNumbers = this.GetStartPositionJustNumbers( currentText, startPosition );

			this.isNegative = !this.isNegative;
			numberValue = this.GetNumberValue( this.GetTextBoxText(), 0 );
			if( this.IsNegative && ( this.CheckIfNegative( numberValue ) == false ) )
				numberValue = this.ToggleNegative( numberValue );

			state.initialString = currentText;

			if( this.CheckForMinMax( numberValue, true ) == true )
			{
				string formattedText = this.ApplyFormatting( numberValue );
				state.changedString = formattedText;
				state.changedValue = numberValue;
				state.success = true;
			}
			else
			{
				state.success = false;
				state.changedString = numberValue;
				state.changedValue = numberValue;
				state.message = "Cannot change value as minimum / maximum conditions will not be met.";
			}

			if( allSelected )
				this.SelectAll();
			return state;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected bool CompleteSubtractKey( NumberModifyState state )
		{
			if( state.handled == false )
				return false;

			if( state.success )
			{
				this.SetModifiedText( state.changedString, state.changedValue );
				this.PositionCursorAfterEdit( state.startPositionJustNumbers, state.inDecimalPosition, state.moveBack, false );
			}
			else
			{
				//Undo the changes.
				//this.isNegative = !this.isNegative;
				this.IsNegative = state.initialNegativeState;
				this.SetZeroNegative( false );
				this.RaiseValidationError( state.changedValue.ToString(), 0, state.message );
			}

			return true;
		}

		/// <summary>
		/// Decimal typing mode. Use if NullString is true;
		/// </summary>
		protected bool m_bDecimalMode = false;
		/// <summary>
		/// Decimal digits position. Use for typing in decimal mode.
		/// </summary>
		private int m_iDecimalDigitsPosition = 0;

		/// <summary>
		/// Invoked when the decimal key is pressed.
		/// </summary>
		/// <returns>True if the key is handled; false otherwise.</returns>
		/// <remarks>
		/// The defined behavior for this key is to jump to the position immediately
		/// after the decimal position.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		virtual protected bool HandleDecimalKey()
		{
			int cursorPosition = this.SelectionStart;
			string currentText = this.GetTextBoxText();

			int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( currentText );
			if( decimalSeparatorPosition > 0 )
			{
				decimalSeparatorPosition++;
				this.SetEmptySelection( decimalSeparatorPosition );

				m_bDecimalMode = false;
			}
			else if( this.IsNull && this.NumberFormatInfoObject.CurrencyDecimalDigits > 0 )
			{
				m_bDecimalMode = true;
			}

			return true;
		}

		#endregion

		#region CURSOR

		/// <summary>
		/// Invoked for positioning the cursor at the right position after 
		/// something has changed.
		/// </summary>
		/// <param name="startPositionJustNumbers">The number of valid numbers before the point.</param>
		/// <param name="inDecimalPosition">Indicates whether the point comes after the decimal separator.</param>
		/// <param name="back">Indicates whether the direction is backwards.</param>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void PositionCursorAfterEdit( int startPositionJustNumbers, bool inDecimalPosition, bool back, bool delete )
		{
			string currentText = this.GetTextBoxText();
			int cursorPosition = 0;
			bool positionReached = false;

			for( int index = 0; index < currentText.Length; index++ )
			{
				if( Char.IsDigit( currentText[index] ) )
				{
					cursorPosition++;
					if( startPositionJustNumbers == 0 || cursorPosition >= startPositionJustNumbers )
					{
						if( delete && inDecimalPosition == false )
						{
							if( startPositionJustNumbers > 0 )
							{
								cursorPosition = index + 1;
							}
							else
							{
								cursorPosition = index;
							}
						}
						else
						{
							cursorPosition = index+1;
							if( back && inDecimalPosition )//Additional check so that backspace does not result in crossing over to number area?
							{
								int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( currentText );
								if( cursorPosition <= decimalSeparatorPosition )
									cursorPosition = decimalSeparatorPosition + this.GetDecimalSeparator().Length;
							}
						}
						positionReached = true;
						break;
					}
				}
			}

			if( !positionReached )
				cursorPosition = currentText.Length;

			if( cursorPosition < 0 )
				cursorPosition = this.GetDecimalSeparatorPosition( currentText );
			else if( cursorPosition > currentText.Length )
				cursorPosition = currentText.Length;

			cursorPosition = this.GetNextDataPos( cursorPosition, inDecimalPosition, back, delete );

			//if(back)
			//{
			//	if(cursorPosition == this.GetDecimalSeparatorPosition(currentText))
			//		cursorPosition = this.GetNextDataPos(cursorPosition+1, inDecimalPosition, back,delete);
			//}

			this.SetEmptySelection( cursorPosition );
		}

		#endregion

		#region MOUSE

		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			if( m_bNeedAllSelectOnMouseDown )
			{
				m_bNeedAllSelectOnMouseDown = false;
				this.SelectAll();
			}
		}

		/// <summary>
		/// Need all select OnMouseDown.
		/// </summary>
		private bool m_bNeedAllSelectOnMouseDown = false;

		#endregion

		/// <override/>
		protected override /*TextBox*/ void WndProc( ref Message m )
		{
			bool processed = false;

			if( m.Msg == NativeMethods.WM_CONTEXTMENU )
			{
				if( this.ContextMenu == null )
				{
					m.Result = IntPtr.Zero;
					processed = true;
				}
			}
			else if( m.Msg == NativeMethods.WM_MOUSEACTIVATE )
			{
				if( this.SelectAllOnFocus && !this.Focused )
				{
					m_bNeedAllSelectOnMouseDown = true;
				}
			}
			else if( m.Msg == NativeMethods.WM_CUT )
			{
				this.Cut();
				processed = true;
			}
			else if( m.Msg == NativeMethods.WM_PASTE )
			{
				this.Paste();
				processed = true;
			}

			if( !processed )
			{
				base.WndProc( ref m );
			}
		}

		private bool selectAllOnFocus = true;

		/// <summary>
		/// Specifies if the text should be selected when the control gets the focus.
		/// </summary>
		[DefaultValue( true )]
		[Description( @"Specifies if the text should be selected when the control gets the focus." )]
		public bool SelectAllOnFocus
		{
			get
			{
				return selectAllOnFocus;
			}

			set
			{
				selectAllOnFocus = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnEnter( EventArgs args )
		{
			if( SelectAllOnFocus )
				this.SelectAll();
			base.OnEnter( args );
		}

		/// <summary>
		/// Gets or sets SelectedText. (overridden property)
		/// </summary>
		public override string SelectedText
		{
			get
			{
				return base.SelectedText;
			}

			set
			{
				if( this.SelectionLength > 0 )
				{
					base.SelectedText = value;
					this.Text = this.GetTextBoxText();
				}
				else
				{
					this.Text = value;
					this.SelectAll();
				}
			}
		}

		/// <summary>
		/// Please use NegativeColor, ZeroColor and PositiveColor properties instead of ForeColor property.
		/// </summary>
		[Browsable( false )]
		[EditorBrowsable( EditorBrowsableState.Never )]
		public new Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}
	}

	/// <summary>
	/// The delegate for handling the SetNULL event.
	/// </summary>
	/// <remarks>
	/// Refer to the <see cref="NumberTextBoxBase.SetNull"/> event for more information.
	/// </remarks>
	public delegate void SetNullEventHandler( object sender, SetNullEventArgs e );

	/// <summary>
	/// Provides data about a <see cref="NumberTextBoxBase.SetNull"/> and 
	/// events of a <see cref="CurrencyTextBox"/>, <see cref="DoubleTextBox"/>,
	/// <see cref="IntegerTextBox"/>, or <see cref="PercentTextBox"/>.
	/// </summary>
	public class SetNullEventArgs: SyncfusionCancelEventArgs
	{
		/// <summary>
		/// Initializes a new <see cref="SetNullEventArgs"/> with event data.
		/// </summary>
		/// <param name="nullValue">The NULL value.</param>
		public SetNullEventArgs( object nullValue )
		{
			this.nullValue = nullValue;
		}

		/// <summary>
		/// Returns the NULL value.
		/// </summary>
		[TraceProperty( true )]
		public object NullValue
		{
			get
			{
				return nullValue;
			}
		}

		private object nullValue;
	}

	/// <summary>
	/// The delegate for handling the KeyValidate event.
	/// </summary>
	/// <remarks>
	/// Refer to the <see cref="NumberTextBoxBase.KeyValidate"/> event for more information.
	/// </remarks>
	public delegate void KeyValidateEventHandler( object sender, KeyValidateEventArgs e );

	/// <summary>
	/// Cancellable event for <see cref="NumberTextBoxBase.KeyValidate"/>
	/// of <see cref="CurrencyTextBox"/>, <see cref="DoubleTextBox"/>,
	/// <see cref="IntegerTextBox"/>, or <see cref="PercentTextBox"/>.
	/// </summary>
	public class KeyValidateEventArgs: SyncfusionCancelEventArgs
	{
		public KeyValidateEventArgs( char key, string changedString, string changedValueString )
		{
			this.key = key;
			this.changedString = changedString;
			this.changedValueString = changedValueString;
		}

		/// <summary>
		/// Returns the character key that was input by the user.
		/// </summary>
		[TraceProperty( true )]
		public char Key
		{
			get
			{
				return key;
			}
		}

		/// <summary>
		/// Returns the changed string that will be set as the text of the text box if this event is not cancelled.
		/// </summary>
		[TraceProperty( true )]
		public string ChangedString
		{
			get
			{
				return changedString;
			}
		}

		/// <summary>
		/// Returns the unformatted changed string that will be set as the text of the text box if this event is not cancelled.
		/// </summary>
		[TraceProperty( true )]
		public string ChangedValueString
		{
			get
			{
				return changedValueString;
			}
		}


		private char key;
		private string changedString;
		private string changedValueString;
	}

    /// <summary>
    /// provides new value and old value after the validation.
    /// </summary>
    public delegate void ControlValidatedEventHandler(object sender, ControlValidatedEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class ControlValidatedEventArgs : EventArgs
    {
        private string newValue = string.Empty;
        private string oldValue = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlValidatedEventArgs"/> class.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        public ControlValidatedEventArgs(string newValue, string oldValue)
        {
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public string NewValue
        {
            get
            {
                return newValue;
            }
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public string OldValue
        {
            get
            {
                return oldValue;
            }
        }
    }

	/// <summary>
	/// Used to pass state between PrepareXXX and CompleteXXX methods during key processing. This allows for
	/// raising the KeyValidate event to cancel any key.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class NumberModifyState
	{

		public string initialString;
		public string changedString;
		public string changedValue;

		// Cursor Position
		public int startPositionJustNumbers;
		public bool inDecimalPosition;
		public bool moveBack;

		// Success
		public bool success = true;

		// Handled
		public bool handled = true;

		// In case of failure
		public string message;

		// Negative
		public bool isNegative;
		public bool initialNegativeState = false;
	}

    #region ENUM

    public enum MinMaxValidation
    {
        OnKeyPress,
        OnLostFocus
    }
    public enum OnValidationFailed
    {
        /// <summary>
        /// Keeps the focus on the control if the validation fails
        /// </summary>
        KeepFocus,
        /// <summary>
        /// Sets NullString to the the control if the validation fails
        /// </summary>
        SetNullString,
        /// <summary>
        /// Sets MinValue if Value is less than MinValue or MaxValue if greater thas MaxValue if the Validation fails
        /// </summary>
        SetMinOrMax
    }

    #endregion
}

