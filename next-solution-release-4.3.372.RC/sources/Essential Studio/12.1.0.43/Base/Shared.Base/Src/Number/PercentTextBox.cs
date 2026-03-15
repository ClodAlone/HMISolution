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

#region file using directives
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Syncfusion.Core.Licensing;
using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Extends the <see cref="System.Windows.Forms.TextBox"/> class to handle percentage input
	/// and validation.
	/// </summary>
	/// <remarks>
	/// The PercentTextBox is derived from the textbox and provides all the functionality
	/// of a textbox and adds additional functionality of its own.
	/// <para>
	/// Collecting percentage input in a consistent format requires validation 
	/// that needs to be built into the application when using the Windows Forms textbox control.
	/// The PercentTextBox includes all this logic into its methods and properties
	/// and makes it easy for the developer and the end user to collect and enter percentage  data.
	/// </para><para>
	/// The PercentTextBox is also closely tied to the globalization settings of the
	/// operating system for pecentage-related properties. Please refer to the 
	/// <see cref="System.Globalization.NumberFormatInfo"/>
	/// class for a detailed explanation of globalization and percent-related attributes.
	/// </para><para>
	/// The PercentTextBox has full support for the Windows Forms designer and you can
	/// just drag-and-drop and set properties on the control just as you would with the
	/// Windows Forms textbox.
	/// </para><para>
	/// The PercentTextBox also raises a <see cref="NumberTextBoxBase.ValidationError"/> event when
	/// inappropriate data is entered into the control.
	/// </para><para>
	/// All clipboard functions such as copy, paste, and cut are also supported with
	/// special accomodations for percent-related issues.
	/// </para></remarks>
	[
	ToolboxItem( true ),
	ToolboxBitmap( typeof( PopupControlContainer ), "ToolboxIcons.PercentTextBox.bmp" ),
	Description("Represents a TextBox to handle percentage input and validation.")
	]
	public class PercentTextBox : NumberTextBoxBase
	{
		#region FIELDS
		/// <summary></summary>
		private Container components = null;

		/// <summary>
		/// Specifies the control is in edit mode.
		/// </summary>
		private PercentTextBoxMode internalMode = PercentTextBoxMode.PercentMode;

		/// <summary>
		/// The minimum value.
		/// </summary>
		private double minValue = -100.0;

		/// <summary>
		/// The maximum value.
		/// </summary>
		private double maxValue = 100.0;

		/// <summary>
		/// The internal value string.
		/// </summary>
		private string internalValue = String.Empty;

		/// <summary></summary>
		private bool returnActualText = false;

		/// <summary>
		/// Indicates whether the mode has to be automatically switched 
		/// when the control receives focus.
		/// </summary>
		private bool switchModeOnFocus = false;

		/// <summary>
		/// The initial double value set in InitializeComponent.
		/// </summary>
		private double initDoubleValue = 0.0;

		/// <summary>
		/// The double value when the control gets the focus. Used when validating.
		/// </summary>
		private double enterDoubleValue = 0.0;

		/// <summary>
		/// The double value that is set through the DoubleValue property.
		/// </summary>
		private double preservedDoubleValue = 0.0;

		/// <summary>
		/// For initialization of culture-related values. Needed when SpecialCultureValue is
		/// not the default value. Since the base class will reset the culture property during
		/// ISupportInitialize.EndInit, we will have to hold these values and set them on the
		/// correct cultureinfo / numberformatinfo.
		/// </summary>
		private int initPercentDecimalDigits = -1;

		/// <summary></summary>
		private string initPercentDecimalSeparator = null;

        /// <summary>
        /// 
        /// </summary>
        private string oldDoubleValue = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string newDoubleValue = string.Empty;

		/// <summary></summary>
		private string initPercentGroupSeparator = null;

		/// <summary></summary>
		private int[ ] initPercentGroupSizes = null;

		/// <summary></summary>
		private int initPercentNegativePattern = -1;

		/// <summary></summary>
		private int initPercentPositivePattern = -1;

		/// <summary></summary>
		private string initPercentSymbol = null;

		/// <summary>
		/// The negative patterns.
		/// </summary>
		private string[ ] percentNegativePatterns = null;

		/// <summary>
		/// Occurs when the <see cref="DoubleValue"/> property is changed.
		/// </summary>
		[ 
		Category( "PropertyChanged" ), 
		Description( "Occurs when the DoubleValue property is changed." ) 
		]
		public event EventHandler DoubleValueChanged;
		#endregion

		#region INITIALIZATION
		/// <summary>
		/// Overloaded. Creates an object of type PercentTextBox. 
		/// </summary>
		/// <remarks>
		/// The PercentTextBox object will be initialized with the default values
		/// for the display and data properties. You need to set any specific 
		/// values.
		/// </remarks>
		public PercentTextBox()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new LicensedComponent( typeof( PercentTextBox ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
			this.SetDefaultValue( ( Double )0 );
			//0 -n % 
			//1 -n% 
			//2 -%n 
			this.percentNegativePatterns = new String[ ]
				{
					"^-(.*) (<PercentSymbol>){0,<PercentSymbolLength>}$", //-n %
					"^-(.*)(<PercentSymbol>){0,<PercentSymbolLength>}$", //-n% 
					"^-(<PercentSymbol>){0,<PercentSymbolLength>}(.*)" //-%n 

				};

			InitializeComponent();
			this.NullString = String.Format( "{0:P}", String.Empty );
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}
			}

			base.Dispose( disposing );
		}
		/// <summary></summary>
		private void InitializeComponent()
		{
			this.Multiline = false;
		}

		/// <summary>
		/// Overrides <see cref="NumberTextBoxBase.InitializeNumberTextBox"/>.
		/// </summary>
		protected override void InitializeNumberTextBox()
		{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			bool bPrevReturnBaseText = this.ReturnBaseText;
			this.ReturnBaseText = true;
#endif
			base.ignoreTextChange = true;

			bool setNullString = false;
			if ( AllowNull && this.initDoubleValue.Equals(0.0) 
				&& (this.Text == this.NullString  || this.Text == string.Empty) )
				setNullString = true;

			base.InitializeNumberTextBox();
			this.SetNumberFormatInfoInitValues();

			if (this.initDoubleValue != 0)
				this.DoubleValue = this.initDoubleValue;
			else if (setNullString)
			{
				this.SetNullNumberValue();
				this.SetTextBoxText(this.NullString);
			}

			base.ignoreTextChange = false;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			this.ReturnBaseText = bPrevReturnBaseText;
#endif
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="s"/>
		private bool ContainsNumbers( string s )
		{
			Match m = null;
			m = Regex.Match( s, "[0-9]" );
			if( m.Success == true )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="formattedText"/>
		protected override string RemoveFormatting(string formattedText, NumberFormatInfo info, bool padIfEmpty)
		{
			string newText = formattedText;

			if( this.ContainsNumbers( this.PercentSymbol ) == true )
			{
				newText = formattedText.Replace( this.PercentSymbol, "" );
			}

			if( newText.IndexOf( this.PercentSymbol ) >= 0 && ( this.PercentSymbol != String.Empty ) && ( this.PercentSymbol != "" ) )
			{
				newText = newText.Replace( this.PercentSymbol, "" );
			}

			return base.RemoveFormatting(newText, info, padIfEmpty);
		}

		/// <summary></summary>
		private void SetNumberFormatInfoInitValues()
		{
			NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
			bool nfiChanged = false;

			if( this.initPercentDecimalDigits != -1 )
			{
				this.NumberFormatInfoObject.PercentDecimalDigits = this.initPercentDecimalDigits;
				nfiChanged = true;
			}

			if( this.initPercentDecimalSeparator != null && this.initPercentGroupSeparator != null )
			{
				if( this.initPercentDecimalSeparator != this.initPercentGroupSeparator )
				{
					this.NumberFormatInfoObject.PercentDecimalSeparator = this.initPercentDecimalSeparator;
					this.NumberFormatInfoObject.NumberDecimalSeparator = this.initPercentDecimalSeparator;
					this.NumberFormatInfoObject.PercentGroupSeparator = this.initPercentGroupSeparator;
					this.NumberFormatInfoObject.NumberGroupSeparator = this.initPercentGroupSeparator;
					nfiChanged = true;
				}
			}
			else
			{
				if( this.initPercentDecimalSeparator != null && this.initPercentDecimalSeparator != this.NumberFormatInfoObject.PercentGroupSeparator )
				{
					this.NumberFormatInfoObject.PercentDecimalSeparator = this.initPercentDecimalSeparator;
					this.NumberFormatInfoObject.NumberDecimalSeparator = this.initPercentDecimalSeparator;
					nfiChanged = true;
				}

				if( this.initPercentGroupSeparator != null && this.initPercentGroupSeparator != this.NumberFormatInfoObject.PercentDecimalSeparator )
				{
					this.NumberFormatInfoObject.PercentGroupSeparator = this.initPercentGroupSeparator;
					this.NumberFormatInfoObject.NumberGroupSeparator = this.initPercentGroupSeparator;
					nfiChanged = true;
				}
			}

			if( this.initPercentGroupSizes != null )
			{
				this.NumberFormatInfoObject.PercentGroupSizes = this.initPercentGroupSizes;
				nfiChanged = true;
			}
			if( this.initPercentNegativePattern != -1 )
			{
				this.NumberFormatInfoObject.PercentNegativePattern = this.initPercentNegativePattern;
				nfiChanged = true;
			}
			if( this.initPercentPositivePattern != -1 )
			{
				this.NumberFormatInfoObject.PercentPositivePattern = this.initPercentPositivePattern;
				nfiChanged = true;
			}
			if( this.initPercentSymbol != null )
			{
				this.NumberFormatInfoObject.PercentSymbol = this.initPercentSymbol;
				nfiChanged = true;
			}

			// We will set the Currency and Number attributes of the NumberFormatInfo to be the default values to prevent 
			// the NumberFormatInfo parsing failing if the Currency or Number attributes are changed
			if( this.UseUserOverride )
			{
				CultureInfo defaultCulture = new CultureInfo( CultureInfo.CurrentCulture.LCID, false );
				if( this.NumberFormatInfoObject.CurrencyDecimalSeparator == this.NumberFormatInfoObject.NumberGroupSeparator || this.NumberFormatInfoObject.CurrencyGroupSeparator == this.NumberFormatInfoObject.NumberDecimalSeparator )
				{
					this.NumberFormatInfoObject.CurrencyDecimalSeparator = defaultCulture.NumberFormat.CurrencyDecimalSeparator;
					this.NumberFormatInfoObject.NumberGroupSeparator = defaultCulture.NumberFormat.NumberGroupSeparator;

					this.NumberFormatInfoObject.CurrencyGroupSeparator = defaultCulture.NumberFormat.CurrencyGroupSeparator;
					this.NumberFormatInfoObject.NumberDecimalSeparator = defaultCulture.NumberFormat.NumberDecimalSeparator;
					nfiChanged = true;
				}
			}

			if( nfiChanged )
			{
				this.FormatChanged( this.GetTextBoxText(), prevFormat );
			}
		}
		#endregion

		#region FORMATTING
		/// <summary>
		/// Gets / sets the percent symbol to be used in the PercentTextBox.
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
        Description("Gets / sets the percent symbol to be used in the PercentTextBox.")
		]
		public string PercentSymbol
		{
			get
			{
				return this.NumberFormatInfoObject.PercentSymbol;
			}

			set
			{
				if( this.Initializing == false )
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.PercentSymbol = value;
					this.FormatChanged( currentText, prevFormat );
				}
				else
				{
					this.initPercentSymbol = value;
					this.NumberFormatInfoObject.PercentSymbol = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the PercentSymbol should not be serialized if the value is the same 
		/// as the one for the current culture.
		/// </summary>
		/// <returns>True if the value is different from the value for the current culture.</returns>
		private bool ShouldSerializePercentSymbol()
		{
			if( this.Culture.NumberFormat.PercentSymbol.Equals( this.PercentSymbol ) == false )
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetPercentSymbol()
		{
			this.PercentSymbol = this.Culture.NumberFormat.PercentSymbol;
		}

		/// <summary>
		/// Gets / sets the pattern to use when the value is positive.
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
        Description("Gets / sets the pattern to use when the value is positive.")
		]
		public int PercentPositivePattern
		{
			get
			{
				return this.NumberFormatInfoObject.PercentPositivePattern;
			}

			set
			{
				if( this.Initializing == false )
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.PercentPositivePattern = value;
					this.FormatChanged( currentText, prevFormat );
				}
				else
				{
					this.initPercentPositivePattern = value;
					this.NumberFormatInfoObject.PercentPositivePattern = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the PercentPositivePattern should not be serialized if
		/// the UseLocaleDefault property is set.
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializePercentPositivePattern()
		{
			if( this.Culture.NumberFormat.PercentPositivePattern != this.PercentPositivePattern )
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetPercentPositivePattern()
		{
			this.PercentPositivePattern = this.Culture.NumberFormat.PercentPositivePattern;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="rawValue"/>
		[ DocumentationExclude() ]
		protected override bool CheckIfNegative( string rawValue )
		{
			try
			{
				if( rawValue != null && rawValue != String.Empty )
				{
                    double dValue;
                    bool isNum = Double.TryParse(rawValue, System.Globalization.NumberStyles.Number,
                                                 this.NumberFormatInfoObject, out dValue);
					if(dValue >= 0.0)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
			}
			catch
			{
			}

			return false;
		}

		/// <summary>
		/// Gets / sets the maximum number of digits for the decimal portion of the percentage.
		/// </summary>
		/// <remarks>
		/// If there is a need to have a different value based on the locale, it will be
		/// automatically changed if the <see cref="Syncfusion.Windows.Forms.Tools.MaskedEditBox.UseLocaleDefault"/> property
		/// is True.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Localizable( false ),
		RefreshProperties( RefreshProperties.Repaint ),
        Description("Gets / sets the maximum number of digits for the decimal portion of the percentage.")
		]
		public int PercentDecimalDigits
		{
			get
			{
				return this.NumberFormatInfoObject.PercentDecimalDigits;
			}

			set
			{
				if( this.Initializing == false )
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.PercentDecimalDigits = value;
					this.FormatChanged( currentText, prevFormat );
				}
				else
				{
					this.initPercentDecimalDigits = value;
					this.NumberFormatInfoObject.PercentDecimalDigits = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the PercentDecimalDigits should not be serialized if
		/// the UseLocaleDefault property is set.
		/// </summary>
		/// <returns>True if the </returns>
		private bool ShouldSerializePercentDecimalDigits()
		{
			if( this.Culture.NumberFormat.PercentDecimalDigits != this.PercentDecimalDigits )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetPercentDecimalDigits()
		{
			this.PercentDecimalDigits = this.Culture.NumberFormat.PercentDecimalDigits;
		}

		/// <summary>
		/// Gets / sets the decimal separator character that will be used for the display.
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
        Description("Gets / sets the decimal separator character that will be used for the display.")
		]
		public string PercentDecimalSeparator
		{
			get
			{
				return this.NumberFormatInfoObject.PercentDecimalSeparator;
			}

			set
			{
			if (!Char.IsLetterOrDigit(value, 0) && value.Length == 1)
				if( this.Initializing == false )
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.PercentDecimalSeparator = value;
					this.FormatChanged( currentText, prevFormat );
				}
				else
				{
					this.initPercentDecimalSeparator = value;
					this.NumberFormatInfoObject.PercentDecimalSeparator = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the PercentDecimalSeparator should not be serialized if
		/// the UseLocaleDefault property is set.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializePercentDecimalSeparator()
		{
			if( this.Culture.NumberFormat.PercentDecimalSeparator != this.PercentDecimalSeparator )
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetPercentDecimalSeparator()
		{
			this.PercentDecimalSeparator = this.Culture.NumberFormat.PercentDecimalSeparator;
		}

		/// <summary>
		/// Gets / sets the separator to be used for grouping digits.
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
        Description("Gets / sets the separator to be used for grouping digits.")
		]
		public string PercentGroupSeparator
		{
			get
			{
				return this.NumberFormatInfoObject.PercentGroupSeparator;
			}

			set
			{
				if( this.Initializing == false )
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.PercentGroupSeparator = value;
					this.FormatChanged( currentText, prevFormat );
				}
				else
				{
					this.initPercentGroupSeparator = value;
					this.NumberFormatInfoObject.PercentGroupSeparator = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether PercentGroupSeparator should not be serialized if
		/// the UseLocaleDefault property is set.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializePercentGroupSeparator()
		{
			if( this.Culture.NumberFormat.PercentGroupSeparator != this.PercentGroupSeparator )
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetPercentGroupSeparator()
		{
			this.PercentGroupSeparator = this.Culture.NumberFormat.PercentGroupSeparator;
		}

		/// <summary>
		/// Gets / sets the grouping of PercentDigits in the PercentTextBox.
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
        Description("Gets / sets the grouping of PercentDigits in the PercentTextBox.")
		]
		public int[ ] PercentGroupSizes
		{
			get
			{
				return this.NumberFormatInfoObject.PercentGroupSizes;
			}

			set
			{
				if( this.Initializing == false )
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.PercentGroupSizes = value;
					this.FormatChanged( currentText, prevFormat );
				}
				else
				{
					this.initPercentGroupSizes = value;
					this.NumberFormatInfoObject.PercentGroupSizes = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the PercentGroupPattern should not be serialized if
		/// the UseLocaleDefault property is set.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializePercentGroupSizes()
		{
			if( this.Culture.NumberFormat.PercentGroupSizes.Length == this.PercentGroupSizes.Length )
			{
				for( int index = 0 ; index < this.Culture.NumberFormat.PercentGroupSizes.Length ; index++ )
				{
					if( this.Culture.NumberFormat.PercentGroupSizes[ index ].Equals( this.PercentGroupSizes[ index ] ) == false )
					{
						return true;
					}
				}
			}

			return false;

		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetPercentGroupSizes()
		{
			this.PercentGroupSizes = this.Culture.NumberFormat.PercentGroupSizes;
		}

		/// <summary>
		/// Gets / sets the pattern to use when the value is negative.
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
        Description("Gets / sets the pattern to use when the value is negative.")
		]
		public int PercentNegativePattern
		{
			get
			{
				return this.NumberFormatInfoObject.PercentNegativePattern;
			}

			set
			{
				if( this.Initializing == false )
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.PercentNegativePattern = value;
					this.FormatChanged( currentText, prevFormat );
				}
				else
				{
					this.initPercentNegativePattern = value;
					this.NumberFormatInfoObject.PercentNegativePattern = value;
				}
			}
		}

		/// <summary>
		/// The negative patterns.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
        Description("The negative patterns.")
		]
		protected string[ ] PercentNegativePatterns
		{
			get
			{
				return this.percentNegativePatterns;
			}

			set
			{
				this.percentNegativePatterns = value;
			}
		}

		/// <summary>
		/// Indicates whether the PercentNegativePattern should not be serialized if
		/// the UseLocaleDefault property is set.
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializePercentNegativePattern()
		{
			if( this.Culture.NumberFormat.PercentNegativePattern != this.PercentNegativePattern )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetPercentNegativePattern()
		{
			this.PercentNegativePattern = this.Culture.NumberFormat.PercentNegativePattern;
		}
		#endregion

		#region FOCUS
		/// <summary>
		/// Overrides the <see cref="System.Windows.Forms.Control.OnEnter"/> method.
		/// </summary>
		/// <param name="args">The event data.</param>
		/// <remarks>
		/// Saves the current DoubleValue so that it can be compared 
		/// during validation. The DoubleValueChanged and TextChanged event
		/// will only be raised if the value is different during validation.
		/// Also switches mode if the <see cref="SwitchModeOnFocus"/> property is set.
		/// </remarks>
		protected override void OnEnter( EventArgs args )
		{
			this.enterDoubleValue = this.DoubleValue;
			if( SwitchModeOnFocus == true )
			{
				if( this.ReadOnly == false && this.internalMode == PercentTextBoxMode.PercentMode )
				{
					this.ChangeMode( PercentTextBoxMode.NumberMode );
				}
			}
			base.OnEnter( args );
		}

		/// <summary>
		/// Overrides the <see cref="System.Windows.Forms.Control.OnLeave"/> method.
		/// </summary>
		/// <param name="args"></param>
		protected override void OnLeave( EventArgs args )
		{
			base.OnLeave( args );
			if( SwitchModeOnFocus == true )
			{
				if( this.ReadOnly == false && this.internalMode == PercentTextBoxMode.NumberMode )
				{
					this.ChangeMode( PercentTextBoxMode.PercentMode );
				}
			}

		}

		/// <summary>
		/// Indicates whether the PercentTextBox should allow editing in numeric mode
		/// when it receives focus.
		/// </summary>
		[
		Description( "Specifies if the mode is to be automatically switched on focus." ),
		DefaultValue( false )
		]
		public bool SwitchModeOnFocus
		{
			get
			{
				return this.switchModeOnFocus;
			}

			set
			{
				this.switchModeOnFocus = value;
				if( value == false )
				{
					this.ChangeMode( PercentTextBoxMode.PercentMode );
				}
			}
		}
		#endregion

		#region INTERNAL OPERATIONS
		/// <summary>
		/// Formats the given text according to the current setting.
		/// </summary>
		/// <param name="rawValue"></param>
		/// <returns></returns>
		[ DocumentationExclude() ]
		protected override string ApplyFormatting( string rawValue )
		{
			return this.GetFormattedText( rawValue, this.internalMode );
		}

		/// <summary></summary>
		/// <param name="incomingValue"/>
		/// <param name="mode"></param>
		/// <returns></returns>
		private string GetFormattedText( string incomingValue, PercentTextBoxMode mode )
		{
			string returnString = String.Empty;
			double dValue = 0.0;
			string rawValue;
			if( incomingValue[ 0 ] != '-' )
			{
				rawValue = base.RemoveFormatting( incomingValue );
			}
			else
			{
				rawValue = incomingValue;
			}
			bool isNum=false;
			if( rawValue != null && rawValue != String.Empty )
			{
				isNum = Double.TryParse(rawValue, System.Globalization.NumberStyles.Any,
                                             this.NumberFormatInfoObject, out dValue);
				if(!isNum)
					dValue = 0.0;
			}

			if (!this.IsNull || isNum)
			{
				// If the internal mode is NumberMode, we just return the 
				// internalValue.
				if( mode == PercentTextBoxMode.NumberMode )
				{
					returnString = this.internalValue;
				}
				else
				{
					dValue = dValue / 100.0;
					returnString = dValue.ToString( "P", this.NumberFormatInfoObject );
				}
			}
			else
			{
				// If the internal mode is NumberMode, we just return the 
				// internalValue.
				if( mode == PercentTextBoxMode.NumberMode )
				{
					returnString = this.internalValue;
				}
				else
				{
					returnString = String.Format( this.NullFormat, "{0}", this.NullString );
				}
			}

			return returnString;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="currentText"/>
		[ DocumentationExclude() ]
		protected override string ToggleNegative( string currentText )
		{
            double dValue;
            bool isNum = Double.TryParse(currentText, System.Globalization.NumberStyles.Number,
                                         this.NumberFormatInfoObject, out dValue);

			dValue = dValue * -1;
			if( dValue.Equals( 0.0 ) )
			{
				this.SetZeroNegative( !this.GetZeroNegative() );
			}
			return dValue.ToString();
		}

		/// <summary>
		/// Override this to return PercentDecimalSeparator in PercentTextBox.
		/// </summary>
		/// <returns></returns>
		/// <param name="info"/>
		[ DocumentationExclude() ]
		protected override string GetDecimalSeparator( NumberFormatInfo info )
		{
			return info.PercentDecimalSeparator;
		}

		/// <summary>
		/// Override this to return PercentGroupSeparator in PercentTextBox.
		/// </summary>
		/// <returns></returns>
		/// <param name="info"/>
		[ DocumentationExclude() ]
		protected override string GetGroupSeparator( NumberFormatInfo info )
		{
			return info.PercentGroupSeparator;
		}

		/// <summary>
		/// Overrides <see cref="NumberTextBoxBase.ParseForNegativeFormat(string)"/>.
		/// </summary>
		/// <param name="currentText">The text to be parsed.</param>
		/// <returns>True if the value is negative; false otherwise.</returns>
		/// <param name="info"/>
		protected override bool ParseForNegativeFormat( string currentText, NumberFormatInfo info )
		{
			string pattern = null;
			Match m = null;
			string perSymbol = info.PercentSymbol;
			int perSymbolLength = info.PercentSymbol.Length;

			if( perSymbol == "%" )
			{
				perSymbol = "\\" + perSymbol;
			}
			else if( perSymbol == String.Empty )
			{
				perSymbol = "\\%";
				perSymbolLength = 1;
			}

			if( PercentNegativePatterns != null )
			{
				foreach( string sec in this.PercentNegativePatterns )
				{
					pattern = Regex.Replace( sec, "<PercentSymbol>", perSymbol );
					pattern = Regex.Replace( pattern, "<PercentSymbolLength>", perSymbol.Length.ToString() );
					m = Regex.Match( currentText, pattern );

					if( m.Success == true )
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary></summary>
		/// <returns></returns>
		[ DocumentationExclude() ]
		protected override int GetDecimalDigits()
		{
			return this.NumberFormatInfoObject.PercentDecimalDigits;
		}

		/// <summary></summary>
		/// <param name="previousFormat"></param>
		/// <param name="currentText"/>
		protected override void FormatChanged( string currentText, NumberFormatInfo previousFormat )
		{
			double currentValue = 0;
			string rawValue = this.RemoveFormatting( currentText, previousFormat );

			if( this.Initializing == false )
			{
				double value;
				if( double.TryParse( rawValue, NumberStyles.Number, previousFormat, out value ) )
				{
					currentValue = value;
				}
					
				this.ApplyFormattingAndSetText( currentValue.ToString() );
			}
		}

		/// <summary></summary>
		/// <param name="newMode"/>
		private void ChangeMode( PercentTextBoxMode newMode )
		{
			if( this.internalMode != newMode )
			{
				string rawValue = String.Empty;
				this.internalMode = newMode;
				this.internalValue = this.GetNumberValue( this.GetTextBoxText(), 0 );
				if( this.internalValue == String.Empty )
				{
					this.internalValue = "0" + this.NumberFormatInfoObject.PercentDecimalSeparator + "0";
				}
				else
				{
					rawValue = this.internalValue;
					rawValue = this.RemoveFormatting( rawValue );
					this.internalValue = rawValue;
				}

                double dValue;
                bool isNum = Double.TryParse(this.internalValue, System.Globalization.NumberStyles.Any,
                                             this.NumberFormatInfoObject, out dValue);

				if( newMode == PercentTextBoxMode.PercentMode )
				{
					dValue = dValue * 100.0;
				}
				else
				{
					dValue = dValue / 100.0;
				}

				this.internalValue = String.Format( this.NumberFormatInfoObject, "{0}", dValue );

				if( newMode == PercentTextBoxMode.NumberMode )
				{
					if( this.internalValue.IndexOf( this.NumberFormatInfoObject.PercentDecimalSeparator ) == -1 )
					{
						this.internalValue += this.NumberFormatInfoObject.PercentDecimalSeparator;
					}
				}

				this.ignoreTextChange = true;
				this.ApplyFormattingAndSetText( this.internalValue );
				this.ignoreTextChange = false;
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="numberValue"/>
		[ DocumentationExclude() ]
		protected int GetNumberPartLength( double numberValue )
		{
			return this.GetNumberPartLength( numberValue.ToString() );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="formattedText"/>
		/// <param name="startPosition"/>
		[ DocumentationExclude() ]
		protected override string GetNumberValue( string formattedText, int startPosition )
		{
			double percentValue = 0.0;
			string extractedString = String.Empty;
			string modifiedRawText = String.Empty;

			if( startPosition < formattedText.Length )
			{
				extractedString = formattedText.Substring( 0, startPosition ) + formattedText.Substring( startPosition );
			}
			else
			{
				extractedString = formattedText;
			}
			try
			{
				modifiedRawText = RemoveFormatting(extractedString);
                if (extractedString != null && extractedString != String.Empty)
                {
                    bool isNum = Double.TryParse(modifiedRawText, System.Globalization.NumberStyles.Any,
                                                 this.NumberFormatInfoObject, out percentValue);
                }
			}
			catch
			{
				// Rollback the operation.
				rollBackOperation = true;
			}

			if( this.IsNegative && percentValue > 0.0 )
			{
				percentValue = percentValue * -1;
			}

			return String.Format( this.NumberFormatInfoObject, "{0}", percentValue );

		}
		#endregion

		#region VALIDATION
		/// <summary>
		/// Gets / sets the maximum value that can be set through the PercentTextBox.
		/// </summary>
		/// <remarks></remarks>
		[
		Category( "Behavior" ),
        Description("Gets / sets the maximum value that can be set through the PercentTextBox.")
		]
		public double MaxValue
		{
			get
			{
				return this.maxValue;
			}

			set
			{
				if (value < this.MinValue)
				{
					this.maxValue = double.MaxValue;
					throw new ArgumentOutOfRangeException("MaxValue", value, "MaxValue Cannot be lesser than MinValue");
				}
				if( value >= double.MinValue / 100.0 && value <= double.MaxValue / 100.0 )
				{
					this.maxValue = value;
				}
                if (!CheckForMinMax(this.DoubleValue.ToString()))
                {
                    this.DoubleValue = value;
                }
			}
		}

		/// <summary>
		/// Indicates whether the MaxValue property should be serialized.
		/// </summary>
		/// <returns>True if the value is not equal to <see cref="double.MaxValue"/>.</returns>
		public bool ShouldSerializeMaxValue()
		{
			if( this.maxValue != 1 )
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Resets the value to the default.
		/// </summary>
		public void ResetMaxValue()
		{
			this.MaxValue = 1;
		}

		/// <summary>
		/// Gets / sets the minimum value that can be set through the PercentTextBox.
		/// </summary>
		/// <remarks></remarks>
		[
		Category( "Behavior" ),
        Description("Gets / sets the minimum value that can be set through the PercentTextBox.")
		]
		public double MinValue
		{
			get
			{
				return this.minValue;
			}

			set
			{
                if (value > this.MaxValue)
                {
                    this.minValue = double.MinValue;
                    throw new ArgumentOutOfRangeException("MinValue", value, "MinValue Cannot be greater than MaxValue");
                }
				if( value >= double.MinValue / 100.0 && value <= double.MaxValue / 100.0 )
				{
					this.minValue = value;
				}
                if (!CheckForMinMax(this.DoubleValue.ToString()))
                {
                    this.DoubleValue= value;
                }
			}
		}

		/// <summary>
		/// Indicates whether the MinValue property should be serialized.
		/// </summary>
		/// <returns>True if the value is not equal to <see cref="decimal.MaxValue"/>.</returns>
		public bool ShouldSerializeMinValue()
		{
			if( this.minValue != -1 )
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Resets the value to the default.
		/// </summary>
		public void ResetMinValue()
		{
			this.MinValue = -1;
		}

/*        /// <summary>
        /// Resets value to the default.
        /// </summary>
        public new void ResetText()
        {
            NumberFormatInfo format = NumberFormatInfoObject;
            double currentValue = 0;

			double value;
			if( double.TryParse( Text, NumberStyles.Number, format, out value ) )
			{
				currentValue = value;
			}
				
			this.ApplyFormattingAndSetText( currentValue.ToString() );
        }*/

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="currentTextValue"/>
		/// <param name="ignoreLength"/>
		[ DocumentationExclude() ]
		protected override bool CheckForMinMax( string currentTextValue, bool ignoreLength )
		{
			bool returnValue = true;
            if (this.MinMaxValidation == MinMaxValidation.OnLostFocus)
                return true;

			string absCurrentTextValue = this.RemoveFormatting( currentTextValue );

			double numericValue = 0.0;
            bool isNum = Double.TryParse(this.GetNumberValue(absCurrentTextValue, 0), System.Globalization.NumberStyles.Any,
                                         this.NumberFormatInfoObject, out numericValue);


			double percentMinValue = this.MinValue * 100.0;
			double percentMaxValue = this.MaxValue * 100.0;

            // The below block of code is removed as the length need not to be considered with the current implementation as we've the 
            // MinMaxValidation property that determines whether the control has to perform MinMaxValidation on KeyPress or on 
            // Focus Lost.

            // The old behavior of the control is to perform the validaton based on the length of the input string and the value,
            // instead it's purely based on the value alone henceforth.

            // User can set the MinMaxValidation to be OnLostFocus if he wishes to allow the users to enter character and perform validation 
            // only when they are done with the input.

            /*
			if( ignoreLength == false )
			{
				int numericValueLength, minValueLength, maxValueLength;

				numericValueLength = this.GetNumberPartLength( absCurrentTextValue );
				minValueLength = this.GetNumberPartLength( Math.Abs( this.MinValue ) );
				maxValueLength = this.GetNumberPartLength( Math.Abs( this.MaxValue ) );

                isNum = Double.TryParse(this.GetNumberValue(absCurrentTextValue, 0), System.Globalization.NumberStyles.Any,
                                        this.NumberFormatInfoObject, out numericValue);

				if( this.internalMode != PercentTextBoxMode.PercentMode )
				{
					numericValue = numericValue * 100.0;
					numericValueLength = this.GetNumberPartLength( numericValue.ToString() );
				}

				if( numericValueLength >= minValueLength )
				{
					if( numericValue < percentMinValue )
					{
						returnValue = false;
					}
				}

				if( numericValueLength >= maxValueLength )
				{
					if( numericValue > percentMaxValue )
					{
						returnValue = false;
					}
				}
				else
				{
					returnValue = true;
				}
			}
			else */
			{
                if (currentTextValue==String.Empty && this.AllowNull)
                    return true;
                if( numericValue * 100 < percentMinValue || numericValue * 100 > percentMaxValue )
				{
					returnValue = false;
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Validates the control.
		/// </summary>
		/// <param name="bRaiseValidationError">Indicates whether the validation error is to be raised.</param>
		/// <returns></returns>
		public override bool Validate( bool bRaiseValidationError )
		{
			string currentValue = this.GetNumberValue( this.internalValue, 0 );
			bool validateSuccess = this.CheckForMinMax( currentValue );
			if( validateSuccess == false )
			{
				if( bRaiseValidationError )
				{
					this.RaiseValidationError( this.FormattedText, 0, "Current value does not meet minimum and maximum requirements." );
				}
			}

			return validateSuccess;
		}

		/// <summary></summary>
		/// <param name="e"/>
		protected override void OnValidating( CancelEventArgs e )
		{
			if( Text.Length == 0 && this.BindableValue != null )
			{
				this.DoubleValue = 0.0;
			}
			else
			{
				try
				{
					if( this.enterDoubleValue != this.DoubleValue && !this.IsNull)
					{
						this.DoubleValue = this.DoubleValue;
					}
				}
				catch
				{
				}
			}
            oldDoubleValue = this.Text;
            if (this.MinMaxValidation == MinMaxValidation.OnLostFocus)
            {
                if (!(this.AllowNull && this.Text == this.NullString))
                {
                    if ((this.DoubleValue < this.MinValue || this.DoubleValue > this.MaxValue))
                    {
                    string invalidText = this.FormattedText;

                    switch (this.OnValidationFailed)
                    {
                        case OnValidationFailed.SetNullString:
                            if (this.AllowNull)
                            {
                                this.SetNullNumberValue();
                                this.SetTextBoxText(this.NullString);
                            }
                            else
                            {
                                this.Focus();
                                this.SelectionStart = this.Text.Length;
                            }
                            break;
                        case OnValidationFailed.SetMinOrMax:
                            this.DoubleValue = this.DoubleValue < this.MinValue ? this.MinValue : this.DoubleValue;
                            this.DoubleValue = this.DoubleValue > this.MaxValue ? this.MaxValue : this.DoubleValue;
                            break;
                        case OnValidationFailed.KeepFocus:
                            {
                                this.Focus();
                                this.SelectionStart = this.Text.Length;
                            }
                            break;
                    }
                    //Raises the ValidationError here as the CheckforMinMax will always return True with MinMaxValidation.OnLostFocus
                    this.RaiseValidationError(invalidText, 0, "Current value does not meet MinValue and MaxValue requirements.");
                    }
                }
            }
            newDoubleValue = this.Text;
			base.OnValidating( e );
		}

        protected override void OnValidated(EventArgs e)
        {
            base.OnValidated(e);
            this.OnControlValidated(newDoubleValue, oldDoubleValue);
        }

		#endregion

		#region KEY HANDLING
		/// <summary>
		/// Overrides the base behavior to implement support for NumberMode. In this mode, the
		/// base behavior of formatting each key is not followed. The key is validated internally to check for
		/// Max and Min conditions and then displayed as is without the formatting. The key will be ignored if
		/// the supressKeyPress value is not set to False. This is set to False when a valid key press is encountered.
		/// </summary>
		/// <returns></returns>
		/// <param name="charToBeInserted"/>
		[ DocumentationExclude() ]
		protected override bool HandleCharacterKey( char charToBeInserted )
		{
			if( this.internalMode == PercentTextBoxMode.NumberMode )
			{
				string baseText = this.GetTextBoxText();
				string newText = baseText.Substring( 0, this.SelectionStart ) + charToBeInserted + baseText.Substring( this.SelectionStart + this.SelectionLength );
				bool valid = this.CheckForMinMax( newText, false );
				this.supressKeyPress = !valid;
				return !valid;
			}
			else
			{
				return base.HandleCharacterKey( charToBeInserted );
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		[ DocumentationExclude() ]
		protected override bool HandleDecimalKey()
		{
			if( this.internalMode == PercentTextBoxMode.NumberMode )
			{
				return false;
			}
			else
			{
				return base.HandleDecimalKey();
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		[ DocumentationExclude() ]
		protected override bool HandleBackspaceKey()
		{
			if( this.internalMode == PercentTextBoxMode.NumberMode )
			{
				return false;
			}
            if (this.CheckIsZero() && this.AllowNull)
            {
                this.SetNullNumberValue();
                this.SetTextBoxText(this.NullString);
                return true;
            }
            return base.HandleBackspaceKey();
		}

		/// <summary></summary>
		/// <returns></returns>
		[ DocumentationExclude() ]
		protected override bool HandleBackspaceKeyChar()
		{
			if( this.internalMode == PercentTextBoxMode.NumberMode )
			{
				return false;
			}
			else
			{
				return base.HandleBackspaceKeyChar();
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		[ DocumentationExclude() ]
		protected override bool HandleDeleteKey()
		{
			if( this.internalMode == PercentTextBoxMode.NumberMode )
			{
				return false;
			}
			else
			{
                if (this.CheckIsZero() && this.AllowNull)
                {
                    this.SetNullNumberValue();
                    this.SetTextBoxText(this.NullString);
                    return true;
                }
				return base.HandleDeleteKey();
			}
		}

		/// <summary>
		/// Invoked when the negative key is pressed.
		/// </summary>
		/// <returns>True if the key is handled; false otherwise.</returns>
		/// <remarks>
		/// The defined behavior for this key is to toggle the sign (negativity)
		/// of the content of the CurrencyTextBox.
		/// </remarks>
		[ DocumentationExclude() ]
		protected override NumberModifyState HandleSubtractKey()
		{
			NumberModifyState state = new NumberModifyState();

			if( this.internalMode == PercentTextBoxMode.NumberMode )
			{
				state.success = false;
				this.SelectionStart = 0;
				this.SelectionLength = 0;
			}
			else
			{
				state.success = true;
                string strValue = this.ToggleNegative(DoubleValue.ToString());
                double newValue = Double.Parse(strValue, this.NumberFormatInfoObject);
				if( this.CheckForMinMax( newValue.ToString(), true ) == true )
				{
					double adjustedValue = GetAdjustedValue( newValue );
					string formattedText = this.ApplyFormatting( adjustedValue.ToString() );
					state.changedString = formattedText;
				}
				else
				{
					state.success = false;
				}

			}

			return state;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="state"/>
		[ DocumentationExclude() ]
		protected override bool CompleteSubtractKey( NumberModifyState state )
		{
			if( state.success == false )
			{
				this.SelectionStart = 0;
				this.SelectionLength = 0;
				return false;
			}
			else
			{
				this.DoubleValue = this.DoubleValue * -1;
				return true;
			}
		}
		#endregion

		#region DATA
		/// <summary>
		/// Gets / sets the double value of the control. This will be formatted and
		/// displayed.
		/// </summary>
		[
		Browsable( true ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Category( "Data" ),
		Description( "The double value of the PercentTextBox control." ),
		RefreshProperties( RefreshProperties.Repaint ),
		]
		public double DoubleValue
		{
			get
			{
				double dValue = 0.0;
				if( this.internalMode == PercentTextBoxMode.NumberMode )
				{
					this.internalValue = this.GetTextBoxText();
                    bool isNum = Double.TryParse(internalValue, System.Globalization.NumberStyles.Number,
                                                 this.NumberFormatInfoObject, out dValue);
				}
				else
				{
                    this.internalValue = this.GetNumberValue(this.GetTextBoxText(), 0);
                    bool isNum = Double.TryParse(internalValue, System.Globalization.NumberStyles.Number,
                                                 this.NumberFormatInfoObject, out dValue);
                    dValue = dValue/100.0;
				}
				return dValue;
			}

			set
			{
				if( this.Initializing == false )
				{
					SetDoubleValue( value );
				}
				else
				{
					this.initDoubleValue = value;
				}
			}
		}

		/// <summary></summary>
		/// <param name="newValue"/>
		protected void SetDoubleValue( double newValue )
		{
			this.SelectionStart = 0;
			this.SelectionLength = base.TextLength;
			if( this.CheckForMinMax( newValue.ToString(), true ) == true )
			{
				double adjustedValue = GetAdjustedValue( newValue );

				// Set the DoubleValue before TextChanged event is raised.
				this.preservedDoubleValue = adjustedValue;
				this.SetPreserveData( true );
				// Text is set and TextChanged and DoubleValueChanged events are raised.
				this.ApplyFormattingAndSetText( adjustedValue.ToString() );
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="newValue"/>
		public virtual double GetAdjustedValue( double newValue )
		{
			double adjustedValue = newValue;
			if( newValue != 0 && Convert.IsDBNull( newValue ) == false )
			{
				adjustedValue = newValue * 100.0;
			}

			return adjustedValue;
		}

		/// <summary></summary>
		/// <param name="val"/>
		protected void SetNullState( double val )
		{
/*			if( this.IsAssignable( this.DefaultValue ) && ( Double )this.DefaultValue == val )
			{
				this.NullState = false;
			}
			else if( val == 0 || Convert.IsDBNull( val ) )
			{
				this.NullState = !this.RaiseSetNull( val );
			}
			else
			{
				this.NullState = false;
			}*/
		}

		/// <summary>
		/// Gets / sets the percent value of the control. This will be formatted and
		/// displayed.
		/// </summary>
		[
		Browsable( true ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Category( "Data" ),
		Description( "The double value of the PercentTextBox control." ),
		RefreshProperties( RefreshProperties.Repaint ),
		]
		public double PercentValue
		{
			get
			{
				return DoubleValue * 100.0;
			}

			set
			{
				this.DoubleValue = value / 100.0;
			}
		}

		/// <summary>
		/// Wrapper property around the selected value. Use this property if you
		/// want to be able to set the value of the control to NULL.
		/// </summary>
		[
		Description( "Wrapper property that indicates the PercentValue. Can be set to NULL." ), 
		Category( "Behavior" ), 
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ), 
		Bindable( true ) 
		]
		public object BindablePercentValue
		{
			get
			{
				if( this.IsNull)
				{
					return null;
				}
				else
				{
					return this.PercentValue;
				}
			}
			set
			{
				try
				{
					bool valueAssigned = false;

					if( value == DBNull.Value || value == null )
					{
						if (!this.RaiseSetNull(value))
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
						{
							throw new ArgumentException();
						}
						else
						{
							SetPercentValue( value );
						}
					}
				}
				catch( Exception ex )
				{
					Console.WriteLine( ex.Message );
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Syncfusion.Windows.Forms.Tools.NumberTextBoxBase.BindableValue"/> property is changed.
		/// </summary>
		[
		Category( @"Property Changed" ),
		Description( @"Occurs when the BindablePercentValue property is changed." )
		]
		public event EventHandler BindablePercentValueChanged;
		/// <summary>
		/// Raises the <see cref="BindablePercentValueChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs"/> that contains the event data.</param>
		protected virtual void OnBindablePercentValueChanged( EventArgs e )
		{
			if( BindablePercentValueChanged != null )
			{
				BindablePercentValueChanged( this, e );
			}
		}

		/// <summary>
		/// Overrides the Text property of <see cref="System.Windows.Forms.TextBox"/>.
		/// </summary>
		/// <remarks>
		/// This property is overriden in order to normalize the data that is set
		/// to the Text property and format it as needed. The method <see cref="Syncfusion.Windows.Forms.Tools.NumberTextBoxBase.InsertString(string, int, int, string)"/>
		/// is used to format the data.
		/// </remarks>
		[
		Browsable( true ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		RefreshProperties( RefreshProperties.Repaint )
		]
		public override string Text
		{
			get
			{
				if( this.ReturnBaseText )
				{
					return base.Text;
				}
				else
				{
				if(this.IsNull && this.AllowNull)
					{
						return this.NullString;
					}
					else
					{
						return this.FormattedText;
					}
				}
			}
			set
			{
				this.SetTextProperty( value );
			}
		}
		/// <summary>
		/// Indicates whether to serialize the Text property
		/// if it is null or quals NullString
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeText()
		{
			return (this.NullString != string.Empty || this.Text != this.NullString);
		}

		/// <summary>
		/// Restores the CurrencyNumberDigits to the MaximumLength.
		/// </summary>
		new protected void ResetText()
		{
			Text = this.DefaultValue.ToString();
		}

		/// <summary></summary>
		/// <param name="newText"/>
		protected override void SetTextProperty( string newText )
		{
			bool success = true;

			if (this.AllowNull && newText == this.NullString)
			{
				this.SetNullNumberValue();
				this.SetTextBoxText(this.NullString);
				return;
			}

			if( newText == null || newText == String.Empty )
			{
				success = false;
			}
			else
			{
				try
				{
                    IFormatProvider format = this.Culture;
                    double parsedValue;
                    bool isNum = Double.TryParse(newText, System.Globalization.NumberStyles.Number,
                                                 this.NumberFormatInfoObject, out parsedValue);

					// Try to parse the text using the Decimal.Parse method.
					this.SetDoubleValue(parsedValue);
				}
				catch
				{
					success = false;
				}
			}

			if( success == false )
			{
				base.SetTextProperty( newText );
			}
		}

        /// <summary>
        /// Gets or sets the NULL string to be displayed.
        /// </summary>
        [
        Category("Behavior"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Specify whether the TextBox can be empty "),
        DefaultValue(false)
        ]
        public override bool AllowNull
        {
            get
            {
                return base.AllowNull;
            }

            set
            {
                if (base.AllowNull != value)
                {
                    base.AllowNull = value;
                }
                if (!value && this.IsNull)
                    this.SetTextBoxText(this.FormattedText);
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
        string formattedText = String.Empty;
        public override string FormattedText
        {
            get
            {
                string formattedText = String.Empty;

                if (this.internalMode == PercentTextBoxMode.NumberMode)
                {
                    this.internalValue = this.GetTextBoxText();
                }
                else
                {
                    this.internalValue = this.GetNumberValue(this.GetTextBoxText(), 0);
                }

                formattedText = this.internalValue;
                if (this.returnActualText == false)
                {
                    formattedText = this.GetFormattedText(this.internalValue, PercentTextBoxMode.PercentMode);
                }
                return formattedText;
            }
        }

		/// <summary>
		/// Returns the clipped text without the formatting.
		/// </summary>
		/// <remarks>
		/// For example, if the Text in the CurrencyTextBox is $45,000.00, the
		/// ClipText property will give 45000.00.
		/// </remarks>
		[
		Browsable( false ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public override string ClipText
		{
			get
			{
				if( this.internalMode == PercentTextBoxMode.NumberMode )
				{
					this.internalValue = this.GetTextBoxText();
				}
				else
				{
					this.internalValue = this.GetNumberValue( this.GetTextBoxText(), 0 );
				}

				string formattedText = this.GetFormattedText( this.internalValue, PercentTextBoxMode.PercentMode );
				string clipText = this.RemoveFormatting( formattedText );
				return clipText;
			}
		}

		/// <summary>
		/// Raises the <see cref="PercentTextBox.DoubleValueChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs"/> that contains the event data.</param>
		protected virtual void OnDoubleValueChanged( EventArgs e )
		{
			if( base.ignoreTextChange != true && DoubleValueChanged != null )
			{
				DoubleValueChanged( this, e );
			}
		}
        internal override void OnAllowNullChanged()
        {
            if (this.IsNull)
            {
                if (this.AllowNull)
                {
                    this.SetNullNumberValue();
                    SetTextBoxText(this.NullString);
                }
                else
                {
                    string minValue = this.MinValue < 0 ? "0" : this.MinValue.ToString();
                    this.ApplyFormattingAndSetText(minValue);
                }
            }
        }
		/// <summary>
		/// Overrides OnTextChanged.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnTextChanged( EventArgs e )
		{
			// Don't need to check for ignoreTextChange as the 
			// functions being called check that.
			base.OnTextChanged( e );
			this.OnDoubleValueChanged( e );
			this.OnBindableValueChanged( e );
			this.OnBindablePercentValueChanged( e );
		}

		/// <summary></summary>
		protected override void SetNullNumberValue()
		{
			this.preservedDoubleValue = 0.0;
			this.SetPreserveData( true );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="val"/>
		protected override bool IsAssignable( object val )
		{
			if( val == null || Convert.IsDBNull( val ) == true )
			{
				return false;
			}
			else
			{
				return true;
			}
		}
        protected override bool CheckIsZero()
        {
            return (!this.IsNull && this.DoubleValue.Equals(0.0));
        }

		/// <summary></summary>
		/// <param name="val"/>
		protected override void SetValue( object val )
		{
            double retNum;
            bool isNum = Double.TryParse(val as string, System.Globalization.NumberStyles.Number,
                                         this.NumberFormatInfoObject, out retNum);
            this.DoubleValue = retNum;
		}

		/// <summary></summary>
		/// <returns></returns>
		protected override object GetValue()
		{
			return this.DoubleValue;
		}

		/// <summary></summary>
		/// <param name="val"/>
		protected void SetPercentValue( object val )
		{
            double retNum;
            bool isNum = Double.TryParse(val as string, System.Globalization.NumberStyles.Number,
                                         this.NumberFormatInfoObject, out retNum);
            this.DoubleValue = retNum / 100;
		}
		#endregion

		#region CLIPBOARD
		/// <summary>
		/// Pastes the data in the clipboard into the PercentTextBox control.
		/// </summary>
		/// <remarks>
		/// The data will be formatted before being pasted into the text box.
		/// </remarks>
		public new void Paste()
		{
			if( this.ReadOnly == false )
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
						this.InsertString( this.GetTextBoxText(), this.SelectionStart, this.SelectionLength, clipBoardData, true );
					}
				}
			}
		}
		#endregion

		#region Overrides

		protected override NumberModifyState PrepareInsertString( string currentText, int startPosition, int selectionLength, string textToBeInserted, bool pasteOperation )
		{
			if( this.PercentValue == 0.0 )
			{
				currentText = currentText.Substring( 0, startPosition + selectionLength );
			}

			return base.PrepareInsertString( currentText, startPosition, selectionLength, textToBeInserted, pasteOperation );
		}

		#endregion
	}

	#region PERCENTTEXTBOXMODE
	/// <summary></summary>
	public enum PercentTextBoxMode
	{
		/// <summary>
		/// values are edited as percentage values.
		/// </summary>
		PercentMode = 0,

		/// <summary>
		/// values are edited as double values and then displayed using percentage formatting.
		/// </summary>
		NumberMode
	}
	#endregion
}