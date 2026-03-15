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
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Globalization;
	using System.Text.RegularExpressions;


	/// <summary>
	/// Extends the <see cref="System.Windows.Forms.TextBox"/> class to handle currency input
	/// and validation.
	/// </summary>
	/// <remarks>
	/// The CurrencyTextBox is derived from the textbox and provides all the functionality
	/// of a text box and adds additional functionality of its own.
	/// <para>
	/// Collecting currency input in a consistent format requires a lot of validation code
	/// that needs to be built into the application when using the Windows Forms textbox control.
	/// The CurrencyTextBox includes all this logic into its methods and properties
	/// and makes it easy for the developer and the end user to collect and enter currency data.
	/// </para>
	/// <para>
	/// The CurrencyTextBox is also closely tied to the globalization settings of the
	/// operating system for Currency related properties. Please refer to the <see cref="System.Globalization.NumberFormatInfo"/>
	/// class for a detailed explanation of globalization and Currency related attributes.
	/// </para>
	/// <para>
	/// The CurrencyTextBox has full support for the Windows Forms designer and you can
	/// just drag-and-drop and set properties on the control just as you would with the
	/// Windows Forms textbox.
	/// </para>
	/// <para>
	/// The CurrencyTextBox also raises a <see cref="NumberTextBoxBase.ValidationError"/> event when
	/// inappropriate data is entered into the control.
	/// </para>
	/// <para>
	/// All clipboard functions such as copy, paste and cut are also supported with
	/// special accommodations for currency related issues.
	/// </para>
	/// </remarks>
	/// <example>
	/// <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\currencydemo\CS\MainForm.cs" name="Currency InitializeComponent" lang="CS">
	/// <code lang="C#">
	/// 
	///    // Create the Calculator Control.
	///    this.currencyTextBox1 = new CurrencyTextBox();
	/// 
	///    // Set the initial value.
	///    this.currencyTextBox1.Text = "$1.00";
	/// 
	///    // Set the clipmode.
	///    this.currencyTextBox1.ClipMode = CurrencyClipModes.IncludeFormatting;
	/// 
	///    // Set formatting properties.
	///    this.currencyTextBox1.CurrencyDecimalDigits = 2;
	///    this.currencyTextBox1.CurrencyDecimalSeparator = ".";
	///    this.currencyTextBox1.CurrencyGroupSeparator = ",";
	///    this.currencyTextBox1.CurrencyGroupSizes = new int[] {3};
	///    this.currencyTextBox1.CurrencyNegativePattern = 1;
	///    this.currencyTextBox1.CurrencyNumberDigits = 27;
	///    this.currencyTextBox1.CurrencyPositivePattern = 0;
	///    this.currencyTextBox1.CurrencySymbol = "$";
	///    this.currencyTextBox1.ForeColor = System.Drawing.Color.Black;
	///    this.currencyTextBox1.NegativeColor = System.Drawing.Color.Red;
	///    this.currencyTextBox1.NegativeSign = "-";
	///    this.currencyTextBox1.PositiveColor = System.Drawing.Color.Black;
	/// 
	///    this.currencyTextBox1.Size = new System.Drawing.Size(256, 20);
	///    this.currencyTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
	/// 
	///    // Add the CurrencyTextBox control to the form.
	///    this.Controls.Add(this.currencyTextBox1);
	///    </code>
	///    </coderef>
	///    
	///    <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\currencydemo\VB\MainForm.vb" name="Currency InitializeComponent" lang="VB"><code lang="VB">
	///    ' Create the CurrencyTextBox
	///    Me.currencyTextBox1 = New CurrencyTextBox
	///    ' Set the initial value
	///    Me.currencyTextBox1.Text = "$1.00"
	///    ' Set the clipmode
	///    Me.currencyTextBox1.ClipMode = CurrencyClipModes.IncludeFormatting
	///    ' Set formatting properties
	///    Me.currencyTextBox1.CurrencyDecimalDigits = 2
	///    Me.currencyTextBox1.CurrencyDecimalSeparator = "."
	///    Me.currencyTextBox1.CurrencyGroupSeparator = ","
	///    Me.currencyTextBox1.CurrencyGroupSizes = New Integer() {3}
	///    Me.currencyTextBox1.CurrencyNegativePattern = 1
	///    Me.currencyTextBox1.CurrencyNumberDigits = 27
	///    Me.currencyTextBox1.CurrencyPositivePattern = 0
	///    Me.currencyTextBox1.CurrencySymbol = "$"
	///    Me.currencyTextBox1.ForeColor = System.Drawing.Color.Black
	///    Me.currencyTextBox1.NegativeColor = System.Drawing.Color.Red
	///    Me.currencyTextBox1.NegativeSign = "-"
	///    Me.currencyTextBox1.PositiveColor = System.Drawing.Color.Black
	///    Me.currencyTextBox1.Size = New System.Drawing.Size(256, 20)
	///    Me.currencyTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
	///    ' Add the CurrencyTextBox control to the form
	///    Me.Controls.Add(Me.currencyTextBox1)</code></coderef>
	/// </example>
	[
	ToolboxItem(true),
	ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.CurrencyTextBox.bmp"),
	Description("Represents a TextBox to handle currency input and validation.")
	]
	public class CurrencyTextBox: NumberTextBoxBase
	{
		#region FIELDS
		/// <summary>
		/// The maximum number of valid digits the textbox can take.
		/// This is set to the maximum value a decimal type can hold.
		/// </summary>
		public static int MaxNumberLength;

		/// <summary>
		/// The negative patterns.
		/// </summary>
		string[] currencyNegativePatterns = null;

		/// <summary>
		/// The minimum value.
		/// </summary>
		private decimal minValue = decimal.MinValue;

		/// <summary>
		/// The maximum value.
		/// </summary>
		private decimal maxValue = decimal.MaxValue;

		/// <summary>
		/// The maximum number of currency digits.
		/// </summary>
		private int numberDigitsValue;

		/// <summary>
		/// The initial decimal value set in InitializeComponent.
		/// </summary>
		private decimal initDecimalValue = 0m;

		/// <summary>
		/// The decimal value when the control gets the focus. Used when validating.
		/// </summary>
		private decimal enterDecimalValue = 0m;

		/// <summary>
		/// The decimal value that is set through the DecimalValue property.
		/// </summary>
		private decimal preservedDecimalValue = 0m;

        /// <summary>
        /// 
        /// </summary>
        private string oldDecimalValue = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string newDecimalValue = string.Empty;

		/// <summary>
		/// For initialization of Culture related values. Needed when SpecialCultureValue is
		/// not the default value. Since the base class will reset the Culture property during
		/// ISupportInitialize.EndInit, we will have to hold these values and set them on the
		/// correct cultureinfo / numberformatinfo.
		/// </summary>
		private int initCurrencyDecimalDigits = -1;
		private string initCurrencyDecimalSeparator = null;
		private string initCurrencyGroupSeparator = null;
		private int[] initCurrencyGroupSizes = null;
		private int initCurrencyNegativePattern = -1;
		private int initCurrencyPositivePattern = -1;
		private string initCurrencySymbol = null;

		#endregion

		#region INITIALIZATION

		/// <summary>
		/// Static initializer for the CurrencyTextBox.
		/// </summary>
		static CurrencyTextBox()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
				new Syncfusion.Core.Licensing.LicensedComponent(typeof(CurrencyTextBox));
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			CurrencyTextBox.MaxNumberLength = 27;
		}

		/// <summary>
		/// Gets the currency formatted text and the decimal value for a given NumberFormatInfo object and
		/// text value.
		/// </summary>
		/// <param name="nfi">The NumberFormatFormatInfo object to be used for the formatting.</param>
		/// <param name="currencyText">The text to be formatted.</param>
		/// <param name="nullString">The value to be returned if the currencyText is considered a NULL value. This should be NULL if the string is not to be formatted as NULL even if value is NULL.</param>
		/// <param name="dValue">The decimal value.</param>
		/// <returns></returns>
		public static string CurrencyFormattedText(NumberFormatInfo nfi, string currencyText,  string nullString, out decimal dValue)
		{
			decimal currencyValue = 0m;
			bool nullState = false;

			if(currencyText != null && currencyText != String.Empty)
			{
				try
				{
					currencyValue = Decimal.Parse(currencyText,NumberStyles.Currency,nfi);
				}
				catch
				{
					Console.WriteLine("Exception parsing {0}", currencyText);
				}
			}
			else
				nullState = true;


			//			if(currencyValue.Equals(0m))
			//				nullState = true;
		
			dValue = currencyValue;

			if(nullState == false || nullString == null)
				return String.Format(nfi, "{0:C}",currencyValue);
			else
				return String.Format(nfi, "{0}", nullString);

		}

		/// <summary>
        /// Overloaded. Creates an object of type CurrencyTextBox. 
		/// </summary>
		/// <remarks>
		/// The CurrencyTextBox object will be initialized with the default values
		/// for the display and data properties. You need to set any specific 
		/// values.
		/// </remarks>
		public CurrencyTextBox() 
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
				new Syncfusion.Core.Licensing.LicensedComponent(typeof(CurrencyTextBox));
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			this.SetDefaultValue((Decimal)0);
			this.currencyNegativePatterns = new String[]{	
															"^\\((<CurrencySymbol>){0,<CurrencySymbolLength>}(.*)\\)$",		//($n)
															"^-(<CurrencySymbol>){0,<CurrencySymbolLength>}(.*)",				//-$n 
															"^(<CurrencySymbol>){0,<CurrencySymbolLength>}-(.*)",				//$-n 
															"^(<CurrencySymbol>){0,<CurrencySymbolLength>}(.*)-$",				//$n- 
															"^\\((.*)(<CurrencySymbol>){0,<CurrencySymbolLength>}\\)$",		//(n$) 
															"^-(.*)(<CurrencySymbol>){0,<CurrencySymbolLength>}$",				//-n$ 
															"(.*)-(<CurrencySymbol>){0,<CurrencySymbolLength>}$",				//n-$ 
															"(.*)(<CurrencySymbol>){0,<CurrencySymbolLength>}-$",				//n$- 
															"^-(.*) (<CurrencySymbol>){0,<CurrencySymbolLength>}$",			//-n $ 
															"^-(<CurrencySymbol>){0,<CurrencySymbolLength>} (.*)",				//-$ n 
															"(.*) (<CurrencySymbol>){0,<CurrencySymbolLength>}-$",				//n $- 
															"^(<CurrencySymbol>){0,<CurrencySymbolLength>} (.*)-$",			//$ n- 
															"^(<CurrencySymbol>){0,<CurrencySymbolLength>} -(.*)",				//$ -n 
															"(.*)- {0,<CurrencySymbolLength>}(<CurrencySymbol>)$",				//n- $ 
															"^\\({0,<CurrencySymbolLength>}(<CurrencySymbol>) (.*)\\)$",		//($ n) 
															"^\\((.*) {0,<CurrencySymbolLength>}(<CurrencySymbol>)\\)$"		//(n $) 
														};
			this.numberDigitsValue = CurrencyTextBox.MaxNumberLength;

			InitializeComponent();
		}

		/// <summary>
		/// 
		/// </summary>
		private void InitializeComponent() 
		{
			this.Multiline = false;
		}

		/// <summary>
		/// Overrides <see cref="NumberTextBoxBase.InitializeNumberTextBox"/>.
		/// </summary>
		protected override void InitializeNumberTextBox()
		{
			base.ignoreTextChange = true;
			bool setNullString = false;

			if (this.initDecimalValue == 0 && AllowNull && this.Text == this.NullString)
				setNullString = true;

			base.InitializeNumberTextBox();
			this.SetNumberFormatInfoInitValues();

			if(this.initDecimalValue != 0)
				this.DecimalValue = this.initDecimalValue;
			else if(setNullString)
				this.SetTextBoxText(this.NullString);

			if(CheckForMinMax(this.DecimalValue) == false)
				this.DecimalValue = this.MinValue;

            base.ignoreTextChange = false;
		}

		private void SetNumberFormatInfoInitValues()
		{
			NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
			bool nfiChanged = false;

			if(this.initNumberFormatInfoObject != null)
				this.NumberFormatInfoObject = this.initNumberFormatInfoObject;

			if(this.initCurrencyDecimalDigits != -1)
			{
				this.NumberFormatInfoObject.CurrencyDecimalDigits = this.initCurrencyDecimalDigits;
				nfiChanged = true;
			}

			if(this.initCurrencyDecimalSeparator != null && this.initCurrencyGroupSeparator != null)
			{
				if(this.initCurrencyDecimalSeparator != this.initCurrencyGroupSeparator)
				{
					this.NumberFormatInfoObject.CurrencyDecimalSeparator = this.initCurrencyDecimalSeparator;
					this.NumberFormatInfoObject.NumberDecimalSeparator = this.initCurrencyDecimalSeparator;

					this.NumberFormatInfoObject.CurrencyGroupSeparator = this.initCurrencyGroupSeparator;
					this.NumberFormatInfoObject.NumberGroupSeparator = this.initCurrencyGroupSeparator;
					nfiChanged = true;
				}
			}
			else
			{
				// Only the decimal separator is changing.
				if(this.initCurrencyDecimalSeparator != null && this.initCurrencyDecimalSeparator != this.NumberFormatInfoObject.CurrencyGroupSeparator)
				{
					this.NumberFormatInfoObject.CurrencyDecimalSeparator = this.initCurrencyDecimalSeparator;
					this.NumberFormatInfoObject.NumberDecimalSeparator = this.initCurrencyDecimalSeparator;
					nfiChanged = true;
				}

				// Only the group separator is changing.
				if(this.initCurrencyGroupSeparator != null && this.initCurrencyGroupSeparator != this.NumberFormatInfoObject.CurrencyDecimalSeparator)
				{
					this.NumberFormatInfoObject.CurrencyGroupSeparator = this.initCurrencyGroupSeparator;
					this.NumberFormatInfoObject.NumberGroupSeparator = this.initCurrencyGroupSeparator;
					nfiChanged = true;
				}
			}

			if(this.initCurrencyGroupSizes != null)
			{
				this.NumberFormatInfoObject.CurrencyGroupSizes = this.initCurrencyGroupSizes;
				nfiChanged = true;
			}
			if(this.initCurrencyNegativePattern != -1)
			{
				this.NumberFormatInfoObject.CurrencyNegativePattern = this.initCurrencyNegativePattern;
				nfiChanged = true;
			}
			if(this.initCurrencyPositivePattern != -1)
			{
				this.NumberFormatInfoObject.CurrencyPositivePattern = this.initCurrencyPositivePattern;
				nfiChanged = true;
			}
			if(this.initCurrencySymbol != null)
			{
				this.NumberFormatInfoObject.CurrencySymbol = this.initCurrencySymbol;
				nfiChanged = true;
			}

			if(nfiChanged)
				this.FormatChanged(this.GetTextBoxText(), prevFormat);
		}

		#endregion

		#region FORMATTING

		/// <summary>
		/// The number of digits for the number part. This is not part of the globalization structure.
		/// </summary>
		/// <remarks>
		/// This value is initially set based on the maximum value of the
		/// Currency data type.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Localizable( true ),
		Description( "The number of digits for the number part. This is not part of the globalization structure." )
		]
		public int NumberDigits
		{
			get
			{
				return this.numberDigitsValue;
			}

			set
			{
				if(value <= CurrencyTextBox.MaxNumberLength)
				{
					this.numberDigitsValue = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether to serialize the CurrencyNumberDigits property
		/// if its the same as the MaximumLength.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeNumberDigits()
		{
			return NumberDigits != CurrencyTextBox.MaxNumberLength;
		}

		/// <summary>
		/// Restores the CurrencyNumberDigits to the MaximumLength.
		/// </summary>
		protected void ResetNumberDigits()
		{
			NumberDigits = CurrencyTextBox.MaxNumberLength;
		}


		/// <summary>
		/// The maximum number of digits for the decimal portion of the currency.
		/// </summary>
		/// <remarks>
		/// The US dollar requires 2 decimal points to accomodate the smallest
		/// denomination and this property will have the value 2 in this case. If there
		/// is a need to have a different value based on the locale, it will be
		/// automatically changed based on the current locale.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Localizable( true ),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "The maximum number of digits for the decimal portion of the currency." )
		]
		public int CurrencyDecimalDigits
		{
			get
			{
				return this.NumberFormatInfoObject.CurrencyDecimalDigits;
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.CurrencyDecimalDigits = value;
					this.FormatChanged(currentText, prevFormat);
				}
				else
				{
					this.initCurrencyDecimalDigits = value;
					this.NumberFormatInfoObject.CurrencyDecimalDigits = value;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		protected string[] CurrencyNegativePatterns
		{
			get
			{
				return this.currencyNegativePatterns;
			}

			set
			{
				this.currencyNegativePatterns = value;
			}
		}

		/// <summary>
		/// Indicates whether the CurrencyDecimalDigits should not be serialized if the value is the same 
		/// as the one for the current culture.
		/// </summary>
		/// <returns>True if the value is different from the value for the current culture.</returns>
		protected bool ShouldSerializeCurrencyDecimalDigits()
		{
			if(this.Culture.NumberFormat.CurrencyDecimalDigits != this.CurrencyDecimalDigits)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Reset the value to the culture specific value.
		/// </summary>
		protected void ResetCurrencyDecimalDigits()
		{
			this.CurrencyDecimalDigits = this.Culture.NumberFormat.CurrencyDecimalDigits;
		}

		/// <summary>
		/// The decimal separator character that will be used for the display.
		/// </summary>
		/// <remarks>
		/// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
		/// and can be changed based on your requirements or based on the locale.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Localizable( true ),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "The decimal separator character that will be used for the display." )
		]
		public string CurrencyDecimalSeparator
		{
			get
			{
				return this.NumberFormatInfoObject.CurrencyDecimalSeparator;
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.CurrencyDecimalSeparator = value;
					this.FormatChanged(currentText, prevFormat);
				}
				else
				{
					this.initCurrencyDecimalSeparator = value;
					this.NumberFormatInfoObject.CurrencyDecimalSeparator = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the CurrencyDecimalSeparator should not be serialized if the value is the same 
		/// as the one for the current culture.
		/// </summary>
		/// <returns>True if the value is different from the value for the current culture.</returns>
		protected bool ShouldSerializeCurrencyDecimalSeparator()
		{
			if(this.Culture.NumberFormat.CurrencyDecimalSeparator != this.CurrencyDecimalSeparator)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		protected void ResetCurrencyDecimalSeparator()
		{
			this.CurrencyDecimalSeparator = this.Culture.NumberFormat.CurrencyDecimalSeparator;
		}

		/// <summary>
		/// This property specifies the separator to be used for grouping digits.
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
		Description( "This property specifies the separator to be used for grouping digits." )
		]
		public string CurrencyGroupSeparator
		{
			get
			{
				return this.NumberFormatInfoObject.CurrencyGroupSeparator;
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.CurrencyGroupSeparator = value;
					this.FormatChanged(currentText,prevFormat);
				}
				else
				{
					this.initCurrencyGroupSeparator = value;
					this.NumberFormatInfoObject.CurrencyGroupSeparator = value;
				}
			}
		}


		/// <summary>
		/// Indicates whether the CurrencyGroupSeparator should not be serialized if the value is the same 
		/// as the one for the current culture.
		/// </summary>
		/// <returns>True if the value is different from the value for the current culture.</returns>
		protected bool ShouldSerializeCurrencyGroupSeparator()
		{
			if(this.Culture.NumberFormat.CurrencyGroupSeparator != this.CurrencyGroupSeparator)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		protected void ResetCurrencyGroupSeparator()
		{
			this.CurrencyGroupSeparator = this.Culture.NumberFormat.CurrencyGroupSeparator;
		}

		/// <summary>
		/// This property specifies the grouping of CurrencyDigits in the CurrencyTextBox.
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
		Description( "This property specifies the grouping of CurrencyDigits in the CurrencyTextBox." )
		]
		public int[] CurrencyGroupSizes
		{
			get
			{
				return this.NumberFormatInfoObject.CurrencyGroupSizes;
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.CurrencyGroupSizes = value;
					this.FormatChanged(currentText, prevFormat);
				}
				else
				{
					this.initCurrencyGroupSizes = value;
					this.NumberFormatInfoObject.CurrencyGroupSizes = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the CurrencyGroupSizes should not be serialized if the value is the same 
		/// as the one for the current culture.
		/// </summary>
		/// <returns>True if the value is different from the value for the current culture.</returns>
		protected bool ShouldSerializeCurrencyGroupSizes()
		{
			if(this.Culture.NumberFormat.CurrencyGroupSizes.Length == this.CurrencyGroupSizes.Length)
			{
				for(int index = 0; index < this.Culture.NumberFormat.CurrencyGroupSizes.Length; index++)
				{
					if(this.Culture.NumberFormat.CurrencyGroupSizes[index].Equals(this.CurrencyGroupSizes[index]) == false)
						return true;
				}
			}
			
			return false;

		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		protected void ResetCurrencyGroupSizes()
		{
			this.CurrencyGroupSizes = this.Culture.NumberFormat.CurrencyGroupSizes;
		}

		/// <summary>
		/// This property specifies the pattern to use when the value is negative.
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
		Description( "This property specifies the pattern to use when the value is negative." )
		]
		public int CurrencyNegativePattern
		{
			get
			{
				return this.NumberFormatInfoObject.CurrencyNegativePattern;
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.CurrencyNegativePattern = value;
					this.FormatChanged(currentText, prevFormat);
				}
				else
				{
					this.initCurrencyNegativePattern = value;
					this.NumberFormatInfoObject.CurrencyNegativePattern = value;
				}
			}
		}


		/// <summary>
		/// Indicates whether the CurrencyNegativePattern should not be serialized if the value is the same 
		/// as the one for the current culture.
		/// </summary>
		/// <returns>True if the value is different from the value for the current culture.</returns>
		protected bool ShouldSerializeCurrencyNegativePattern()
		{
			if(this.Culture.NumberFormat.CurrencyNegativePattern != this.CurrencyNegativePattern)
				return true;
			else
				return false;
		}


		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		protected void ResetCurrencyNegativePattern()
		{
			this.CurrencyNegativePattern = this.Culture.NumberFormat.CurrencyNegativePattern;
		}

		/// <summary>
		/// This property specifies the pattern to use when the value is positive.
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
		Description( "This property specifies the pattern to use when the value is positive." )
		]
		public int CurrencyPositivePattern
		{
			get
			{
				return this.NumberFormatInfoObject.CurrencyPositivePattern;
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.CurrencyPositivePattern = value;
					this.FormatChanged(currentText,prevFormat);
				}
				else
				{
					this.initCurrencyPositivePattern = value;
					this.NumberFormatInfoObject.CurrencyPositivePattern = value;
				}
			}
		}


		/// <summary>
		/// Indicates whether the CurrencyPositivePattern should not be serialized if the value is the same 
		/// as the one for the current culture.
		/// </summary>
		/// <returns>True if the value is different from the value for the current culture.</returns>
		protected bool ShouldSerializeCurrencyPositivePattern()
		{
			if(this.Culture.NumberFormat.CurrencyPositivePattern != this.CurrencyPositivePattern)
				return true;
			else
				return false;

		}


		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		protected void ResetCurrencyPositivePattern()
		{
			this.CurrencyPositivePattern = this.Culture.NumberFormat.CurrencyPositivePattern;
		}

		/// <summary>
		/// This property specifies the currency symbol to be used in the CurrencyTextBox.
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
		Description( "This property specifies the currency symbol to be used in the CurrencyTextBox." )
		]
		public string CurrencySymbol
		{
			get
			{
				return this.NumberFormatInfoObject.CurrencySymbol;		
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.CurrencySymbol = value;
					this.FormatChanged(currentText,prevFormat);
				}
				else
				{
					this.initCurrencySymbol = value;
					this.NumberFormatInfoObject.CurrencySymbol = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the CurrencySymbol should not be serialized if the value is the same 
		/// as the one for the current culture.
		/// </summary>
		/// <returns>True if the value is different from the value for the current culture.</returns>
		protected bool ShouldSerializeCurrencySymbol()
		{
			if(this.Culture.NumberFormat.CurrencySymbol.Equals(this.CurrencySymbol) == false)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Resets the value to the culture specific value
		/// </summary>
		protected void ResetCurrencySymbol()
		{
			this.CurrencySymbol = this.Culture.NumberFormat.CurrencySymbol;
		}

		/// <summary>
		/// The number of digits for the number part. This is not part of the globalization structure.
		/// </summary>
		/// <remarks>
		/// This value is initially set based on the maximum value of the
		/// Currency data type.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Localizable( true ),
		Description( "The number of digits for the number part. This is not part of the globalization structure." )
		]
		public int CurrencyNumberDigits
		{
			get
			{
				return this.NumberDigits;
			}

			set
			{
				this.NumberDigits = value;
			}
		}

		/// <summary>
		/// Indicates whether to serialize the CurrencyNumberDigits property
		/// if its the same as the MaximumLength
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeCurrencyNumberDigits()
		{
			return CurrencyNumberDigits != CurrencyTextBox.MaxNumberLength;
		}

		/// <summary>
		/// Restores the CurrencyNumberDigits to the MaximumLength.
		/// </summary>
		protected void ResetCurrencyNumberDigits()
		{
			CurrencyNumberDigits = CurrencyTextBox.MaxNumberLength;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="previousFormat"></param>
		protected override void FormatChanged(string currentText,NumberFormatInfo previousFormat)
		{
			decimal currentValue = 0m;

			if(this.Initializing == false)
			{
				try
				{
					if(this.GetPreserveData() == true)
						currentValue = this.DecimalValue;
					else
					{
						currentText = currentText.Trim();
						if(currentText != String.Empty)
							currentValue = Decimal.Parse(currentText,NumberStyles.Currency,previousFormat);
					}
				}
				catch
				{
					currentValue = Convert.ToDecimal(this.GetNumberValue(currentText, 0));
				}
				finally
				{
					this.ApplyFormattingAndSetText(currentValue.ToString());
				}
			}
		}
		/// <summary>
		/// Leading symbol.
		/// </summary>
		private char c_cLeadingSymbol = '0';

		/// <summary>
		/// Remove last decimal zeros.
		/// </summary>
		private bool m_bRemoveDecimalZeros = false;
		/// <summary>
		/// If need correct ConcatModifiedText.
		/// </summary>
		private bool m_bCorrectConcatModifiedText = false;
		/// <summary>
		/// Zeros need added count.
		/// </summary>
		private int m_iZerosNeedAddedCount = 0;

		/// <summary>
		/// Gets or sets remove last decimal zeros.
		/// </summary>
		[ DefaultValue( false ) ]
		[ Description( "Gets or sets remove last decimal zeros." ) ]
		[ Category("Appearance") ]
		public bool RemoveDecimalZeros
		{
			get
			{
				return m_bRemoveDecimalZeros;
			}
			set
			{
				if( m_bRemoveDecimalZeros != value )
				{
					m_bRemoveDecimalZeros = value;
					this.ApplyFormattingAndSetText( this.DecimalValue.ToString() );
				}
			}
		}

		protected override string ConcatModifiedText(string startCurrentText, string inputRawText, string endCurrentText)
		{
			
			if( RemoveDecimalZeros && m_bCorrectConcatModifiedText 
				&& startCurrentText.IndexOf( this.CurrencyDecimalSeparator ) == -1 
				&& endCurrentText.IndexOf( this.CurrencyDecimalSeparator ) == -1 )
			{
				startCurrentText += this.CurrencyDecimalSeparator;
				for( int i = 0; i < m_iZerosNeedAddedCount ; i++ )
				{
					startCurrentText += c_cLeadingSymbol;
				}
			}

			return base.ConcatModifiedText( startCurrentText, inputRawText, endCurrentText );
		}

		protected override bool HandleDecimalKey()
		{
			if( this.RemoveDecimalZeros && this.Text.IndexOf( this.CurrencyDecimalSeparator ) == -1 )
			{
				m_bCorrectConcatModifiedText = true;
			}

            string currentText = this.GetTextBoxText();
            int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( currentText );
			if( decimalSeparatorPosition > 0 && this.SelectionLength != 0 )
			{
                string modifiedText = this.DeleteSelectedText( currentText, this.SelectionStart, decimalSeparatorPosition );
                currentText = ApplyFormattingAndSetText( this.GetNumberValue( modifiedText, this.SelectionStart ) );
            }

			decimalSeparatorPosition = this.GetDecimalSeparatorPosition( currentText );
			if( decimalSeparatorPosition > 0 )
			{
				decimalSeparatorPosition++;

				this.SelectionStart = decimalSeparatorPosition;
                this.SelectionLength = this.CurrencyDecimalDigits;

                m_bDecimalMode = false;
			}
			else if( this.IsNull && this.NumberFormatInfoObject.CurrencyDecimalDigits > 0 )
			{
				m_bDecimalMode = true;
			}

			return true;
		}


		/// <summary>
		/// Format the given text according to the current setting.
		/// </summary>
		/// <param name="rawValue"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string ApplyFormatting(string rawValue) 
		{
			string returnString;
			decimal dValue = 0m;
            bool isNum = false;
			if(rawValue != null && rawValue != String.Empty)
                isNum = decimal.TryParse(rawValue,out dValue);

			NumberFormatInfo nfi = (NumberFormatInfo) this.NumberFormatInfoObject.Clone();
			
			if( RemoveDecimalZeros )
			{
				int decimalPosition = rawValue.IndexOf( System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator );
				if( decimalPosition != -1 )
				{
					string strValue = rawValue;
					int decSymbol = 0;
					for( int i = rawValue.Length - 1 ; i > 0 ; i-- )
					{
						if( rawValue[i] == c_cLeadingSymbol )
						{
							decSymbol++;
						}
						else
						{
							break;
						}
					}

					nfi.CurrencyDecimalDigits = rawValue.Length - ( decimalPosition + decSymbol +
						System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Length );
				}
				else
				{
					nfi.CurrencyDecimalDigits = 0;
				}
			}

			//if(dValue.Equals(0m) && this.NullState == false)
			//	this.NullState = true;

            if ((isNum /*&& !dValue.Equals(0m)*/) || !this.AllowNull)
				returnString = String.Format( nfi, "{0:C}",dValue);
			else
				returnString = String.Format(this.NullFormat, "{0}", this.NullString);

			return returnString;
		}

		#endregion

		#region DATA

		/// <summary>
		/// Overrides the Text property of <see cref="System.Windows.Forms.TextBox"/>.
		/// </summary>
		/// <remarks>
		/// This property is overriden in order to normalize the data that is set
		/// to the Text property and format it as needed. The method <see cref="Syncfusion.Windows.Forms.Tools.NumberTextBoxBase.InsertString(string, int, int, string, bool)"/>
		/// is used to format the data.
		/// </remarks>
		[
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		RefreshProperties(RefreshProperties.Repaint)
		]
		public override string Text
		{
			get
			{
				if(!this.IsNull && !this.AllowNull)
				{
                    if (this.ClipMode == CurrencyClipModes.IncludeFormatting || !this.IsHandleCreated)
					{
						return this.FormattedText;
					}
					return this.ClipText;
				}
				return base.Text;
			}
			set
			{	
				this.SetTextProperty(value);
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
		protected override void SetTextProperty(string newText)
		{
			bool success = true;

			if (this.AllowNull && newText == this.NullString)
			{
				this.SetTextBoxText(this.NullString);
				return;
			}

			if(newText == null || newText == String.Empty)
				success = false;
			else
			{
				try
				{
					// Try to parse the text using the Decimal.Parse method
					decimal parsedValue = Decimal.Parse(newText, NumberStyles.Currency, this.NumberFormatInfoObject);
					this.SetDecimalValue(parsedValue);
				}
				catch
				{
					success = false;
				}
			}
           
			if(success == false)
				base.SetTextProperty(newText);
		}

		/// <summary>
		/// The Maximum Value that can be set through the CurrencyTextBox.
		/// </summary>
		/// <remarks>
		/// The default value is the MaxValue for <see cref="decimal"/>.
		/// </remarks>
		[
		Category( "Behavior" ),
		Description( "The Maximum Value that can be set through the CurrencyTextBox." )
		]
		public decimal MaxValue
		{
			get
			{
				return this.maxValue;
			}

			set
			{
                if (value < this.MinValue)
                {
                    this.maxValue = decimal.MaxValue;
                    throw new ArgumentOutOfRangeException("MaxValue", value, "MaxValue Cannot be lesser than MinValue");
                }
				this.maxValue = value;
                if (!CheckForMinMax(this.DecimalValue.ToString()))
                {
                    this.DecimalValue= value;
                }
			}
		}

		/// <summary>
		/// Indicates whether the MaxValue property should be serialized.
		/// </summary>
		/// <returns>true if the value is not equal to <see cref="decimal.MaxValue"/></returns>
		protected bool ShouldSerializeMaxValue()
		{
			if(this.maxValue != decimal.MaxValue)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Resets the value to the default.
		/// </summary>
		protected void ResetMaxValue()
		{
			this.MaxValue = decimal.MaxValue;
		}

		

		/// <summary>
		/// The Minimum Value that can be set through the CurrencyTextBox.
		/// </summary>
		[
		Category( "Behavior" ),
		Description( "The Minimum Value that can be set through the CurrencyTextBox." )
		]
		public decimal MinValue
		{
			get
			{
				return this.minValue;
			}

			set
			{
                if (value > this.MaxValue)
                {
                    this.minValue = decimal.MinValue;
                    throw new ArgumentOutOfRangeException("MinValue", value, "MinValue Cannot be greater than MaxValue");
                }
				this.minValue = value;
                if (!CheckForMinMax(this.DecimalValue.ToString()))
                {
                    this.DecimalValue = value;
                }
			}
		}


		/// <summary>
		/// Indicates whether the MinValue property should be serialized.
		/// </summary>
		/// <returns>True if the value is not equal to <see cref="decimal.MaxValue"/></returns>
		protected bool ShouldSerializeMinValue()
		{
			if(this.minValue != decimal.MinValue)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Resets the value to the default.
		/// </summary>
		protected void ResetMinValue()
		{
			this.MinValue = decimal.MinValue;
		}

		/// <summary>
		/// Occurs when the <see cref="DecimalValue"/> property is changed.
		/// </summary>
		[Category("PropertyChanged")]
		[Description("Occurs when the DecimalValue property is changed.")]
		public event EventHandler DecimalValueChanged;

		
		/// <summary>
		/// The decimal value of the control. This will be formatted and
		/// displayed.
		/// </summary>
		[
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Data"),
		Description("The decimal value of the currency control."),
		RefreshProperties(RefreshProperties.Repaint), 
		]
		public decimal DecimalValue
		{
			get
			{
				if(this.GetPreserveData() == true)
					return this.preservedDecimalValue;
				else
					return Convert.ToDecimal(this.GetNumberValue(GetTextBoxText(),0));
			}

			set
			{
				if(this.Initializing == false)
					this.SetDecimalValue(value);
				else
					this.initDecimalValue = value;
			}
		}

		protected void SetDecimalValue(decimal newValue)
		{
			this.SelectionStart = 0;
			this.SelectionLength = base.TextLength;
			if(this.CheckForMinMax(newValue) == true)
			{
				// Set the DecimalValue before TextChanged event is raised
				this.preservedDecimalValue = newValue;
				this.SetPreserveData(true);
				// Text is set and TextChanged and DecimalValueChanged events are raised
				this.ApplyFormattingAndSetText(newValue.ToString());
				this.SetPreserveData(false);
			}
		}


		protected override bool IsAssignable(object val)
		{
			if(val == null || Convert.IsDBNull(val) == true)
				return false;

			bool assignable = typeof(Decimal).IsAssignableFrom(val.GetType());
			return assignable;
		}

		protected override void SetValue(object val)
		{
			this.DecimalValue = (Decimal)val;
		}

		protected override object GetValue()
		{
			return this.DecimalValue;
		}

		/// <summary>
		/// Raises the <see cref="CurrencyTextBox.DecimalValueChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnDecimalValueChanged(EventArgs e)
		{
			if (base.ignoreTextChange != true && DecimalValueChanged != null)
				DecimalValueChanged(this, e);
		}

		/// <summary>
		/// Overrides OnTextChanged.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnTextChanged(EventArgs e)
		{
			// Dont need to check for ignoreTextChange as the 
			// functions being called check that
			base.OnTextChanged(e);
			this.OnDecimalValueChanged(e);
			this.OnBindableValueChanged(e);
			
		}

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue >= 49 && e.KeyValue <= 58 && 
                this.ClipText.Length - this.CurrencyDecimalDigits - 1 == this.NumberDigits
                && GetDecimalSeparatorPosition(this.Text) == this.SelectionStart)
            {
                this.SelectionStart = this.Text.IndexOf('.') + 1;
            }
            base.OnKeyDown(e);
        }
        internal override void OnAllowNullChanged()
        {
            if (this.IsNull)
            {
                if (this.AllowNull)
                    SetTextBoxText(this.NullString);
                else
                {
                    string minValue = this.MinValue < 0 ? "0" : this.MinValue.ToString();
                    this.ApplyFormattingAndSetText(minValue);
                }
            }
        }
		protected override void SetNullNumberValue()
		{
			this.preservedDecimalValue = 0m;
			this.SetPreserveData(true);
		}

		#endregion

		#region INTERNAL OPERATIONS
		/// <summary>
		/// Calculates start position of numeric value.
		/// </summary>
		/// <returns></returns>
		private int GetNumberStartPosition()
		{
			string text = FormattedText;
			int symbolStartPos = FormattedText.IndexOf( CurrencySymbol );
			int symbolEndPos = symbolStartPos + CurrencySymbol.Length;

			for( int i = 0, len = text.Length; i < len; i++ )
			{
				if( !Char.IsNumber( text[ i ] ) ||
					( symbolStartPos >= i && symbolEndPos <= i ) ) continue;

				return i;
			}

			return -1;
		}

		/// <summary>
		/// Calculates end position of numeric value.
		/// </summary>
		/// <returns></returns>
		private int GetNumberEndPosition()
		{
            string text = FormattedText;

            if (CurrencySymbol == null || CurrencySymbol.Length == 0)
                return text.Length;

 			int symbolStartPos = FormattedText.IndexOf( CurrencySymbol );
			int symbolEndPos = symbolStartPos + CurrencySymbol.Length;

			for( int i = text.Length - 1; i >= 0; i-- )
			{
				if( !Char.IsNumber( text[ i ] ) ||
					( symbolStartPos >= i && symbolEndPos <= i ) ) continue;

				return i;
			}

			return -1;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override bool HandleBackspaceKey()
		{			
			// change value to positive, if Negative Sign is just deleted through
			// Backspace key press
			if( ( SelectionStart >= 0 && SelectionStart <= FormattedText.Length ) &&
                ((SelectionLength > 0 && NegativeSign != "" && FormattedText.Substring(SelectionStart, SelectionLength).IndexOf(NegativeSign) >= 0) || 
				( SelectionStart > 0 && SelectionLength == 0 && FormattedText[ SelectionStart - 1 ].ToString() == NegativeSign ) ) )
			{
				int numStartPos = GetNumberStartPosition();
				int numEndPos = GetNumberEndPosition();
				int selEndPos = SelectionStart + SelectionLength;
				int selStart = SelectionStart;
				int length = ( numStartPos >= selStart ) ? ( selEndPos - numStartPos ) :
					( numEndPos - selStart );

				// change sign to positive
				NumberModifyState state = this.HandleSubtractKey();
				this.CompleteSubtractKey( state );

				// process deleting operation
				if( length > 0 )
				{
					SelectionStart = ( numStartPos >= selStart ) ? GetNumberStartPosition() :
						( selStart - numStartPos );
					SelectionLength = length;

					return base.HandleBackspaceKey();
				}

				// move cursor to the beginning
				else
				{					
					SelectionStart = 0;
					SelectionLength = 0;

					return true;
				}
			}

			else if (this.DecimalValue == 0m && this.AllowNull)
			{
				base.SetTextBoxText(this.NullString);
				return true;
			}
				// else process BackSpace key press event normally
			else
			{
				return base.HandleBackspaceKey();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override bool HandleDeleteKey()
		{
			// change value to positive, if Negative Sign is just deleted through
			// Backspace key press
			if( SelectionStart >= 0 && SelectionStart < FormattedText.Length &&
                ((NegativeSign != "" && FormattedText.Substring(SelectionStart, SelectionLength).IndexOf(NegativeSign) >= 0) 
				|| FormattedText[ SelectionStart ].ToString() == NegativeSign ) ) 
			{

				int numStartPos = GetNumberStartPosition();
				int numEndPos = GetNumberEndPosition();
				int selEndPos = SelectionStart + SelectionLength;
				int selStart = SelectionStart;

				int length = ( numStartPos >= selStart ) ? ( selEndPos - numStartPos ) :
					( numEndPos - selStart );

				// change sign to positive
				NumberModifyState state = this.HandleSubtractKey();
				this.CompleteSubtractKey( state );

				// process deleting operation
				if( length > 0 )
				{
					SelectionStart = ( numStartPos >= selStart ) ? GetNumberStartPosition() :
						( selStart - numStartPos );
					SelectionLength = length;

					return base.HandleDeleteKey();
				}
				
				// move cursor to the beginning
				else
				{					
					SelectionStart = 0;
					SelectionLength = 0;

					return true;
				}
			}

			else if (this.DecimalValue == 0m && this.AllowNull)
			{
				base.SetTextBoxText(this.NullString);
				return true;
			}
				// else process BackSpace key press event normally
			else
			{
				return base.HandleDeleteKey();
			}
		}


		/// <summary>
		/// Checks if the text can be inserted subject to the constraints.
		/// </summary>
		/// <param name="currentText">The current content of the TextBox.</param>
		/// <param name="inputText">The text to be inserted.</param>
		/// <returns>The length of the acceptable string to be inserted.</returns>
		protected override int CanInsert(string currentText, string inputText, int initialZeroCount) 
		{
			// Get the current cursor position
			int startPosition = this.SelectionStart;

			// Decimal separator position in the current text
			int decimalSeparatorPosition = this.GetDecimalSeparatorPosition(currentText);

			// Decimal seprator position in the text to be inserted
			int inputTextDecimalSeparatorPosition = this.GetDecimalSeparatorPosition(inputText);

			// Handle the empty box case
			if (currentText == null || currentText.Length == 0) 
			{
				if(inputTextDecimalSeparatorPosition < 0)
					return this.NumberDigits;
				else
				{
					return this.NumberDigits + (inputText.Length - inputTextDecimalSeparatorPosition) + this.GetDecimalSeparator().Length;
				}
			}
		
			// Current text is not empty

			if (decimalSeparatorPosition >= 0 && startPosition > decimalSeparatorPosition) 
			{
				// Point of insertion is after the decimal separator position
				return this.GetDecimalDigits();
			} 
			else 
			{
				int zeroAdjustment = initialZeroCount;

				// Point of insertion is NOT in the decimal part
				int charLengthAcceptable = 0;

				// If there is no decimal seprator in the current text
				if (decimalSeparatorPosition < 0)
					decimalSeparatorPosition = currentText.Length;

				// Get the number part string into a string (with formatting)
				string partialFormattedText = currentText.Substring(0,decimalSeparatorPosition);
				// Remove the formatting for the number part string
				string rawText = this.RemoveFormatting(partialFormattedText, false);

				int rawTextSelectionStart = Math.Max(0, startPosition - (partialFormattedText.Length - rawText.Length));
				if(rawTextSelectionStart < zeroAdjustment)
					zeroAdjustment = zeroAdjustment - 1;

				// If the text to be inserted does not contain a decimal separator, the length that can be inserted is
				// the NumberDigits minus the existing number portion's length
				if(inputTextDecimalSeparatorPosition < 0)
					charLengthAcceptable = Math.Max(0, this.NumberDigits  - (rawText.Length - zeroAdjustment));
				else
				{
					charLengthAcceptable = Math.Max(0, this.NumberDigits  - (rawText.Length - zeroAdjustment) + inputText.Length - inputTextDecimalSeparatorPosition + this.GetDecimalSeparator().Length);
				}

				return charLengthAcceptable;
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override bool CheckIfNegative(string rawValue)
		{
			if(rawValue != null && rawValue != String.Empty)
			{
				decimal dValue = Convert.ToDecimal(rawValue);
				if(dValue >= 0m)
					return false;
				else
					return true;
			}
			return false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int GetNumberPartLength(decimal numberValue)
		{
			return this.GetNumberPartLength(numberValue.ToString());
		}


		protected bool CheckForMinMax(decimal numericValue)
		{
			return CheckForMinMax(numericValue.ToString(),true);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override bool CheckForMinMax(string currentTextValue, bool ignoreLength)
		{
            if (MinMaxValidation == MinMaxValidation.OnLostFocus)
                return true;
			bool returnValue = true;

			string absCurrentTextValue = this.RemoveFormatting(currentTextValue);
            decimal numericValue = 0;
            bool isNum = Decimal.TryParse(GetNumberValue(absCurrentTextValue, 0),out numericValue);

            // The below block of code is removed as the length need not to be considered with the current implementation as we've the 
            // MinMaxValidation property that determines whether the control has to perform MinMaxValidation on KeyPress or on 
            // Focus Lost.

            // The old behavior of the control is to perform the validaton based on the length of the input string and the value,
            // instead it's purely based on the value alone henceforth.

            // User can set the MinMaxValidation to be OnLostFocus if he wishes to allow the users to enter character and perform validation 
            // only when they are done with the input.

			/* if(ignoreLength == false)
			{
				int numericValueLength, minValueLength, maxValueLength;
				numericValueLength = this.GetNumberPartLength(absCurrentTextValue);
				minValueLength = this.GetNumberPartLength(Math.Abs(this.MinValue));
				maxValueLength = this.GetNumberPartLength(Math.Abs(this.MaxValue));
				numericValue = Convert.ToDecimal(this.GetNumberValue(absCurrentTextValue, 0));

				if(numericValueLength >= minValueLength)
				{
					if(numericValue < this.MinValue)
						returnValue = false;
				}

				if(numericValueLength >= maxValueLength)
				{
					if(numericValue > this.MaxValue)
						returnValue = false;
				}
				else
					returnValue = true;
			}
			else */
			{
                if ((currentTextValue==String.Empty) && this.AllowNull)
                    return true;
				if(numericValue < this.MinValue || numericValue > this.MaxValue)
					returnValue = false;
			}

			return returnValue;
		}


		/// <summary>
		/// Returns the position of the decimal separator taking into account
		/// the CurrencySymbol.
		/// </summary>
		/// <param name="currentText">The text in which to look for the decimal separator.</param>
		/// <returns>The decimal separator's position.</returns>
		protected override int GetDecimalSeparatorPosition(string currentText)
		{
            int symbolPosition = CurrencySymbol.Length > 0 ? currentText.IndexOf(this.CurrencySymbol) : -1;
			int symbolLength = this.CurrencySymbol.Length;
			int startSeek = 0;
			int decimalPosition = 0;

			decimalPosition = currentText.IndexOf(this.CurrencyDecimalSeparator, startSeek);

			if(symbolPosition != -1)
			{
				if (decimalPosition > symbolPosition && decimalPosition < symbolPosition + symbolLength)
				{
					startSeek = symbolPosition + symbolLength + 1;
					if (startSeek > currentText.Length)
					{
						return -1;
					}
					decimalPosition = currentText.IndexOf(this.CurrencyDecimalSeparator, startSeek);
				}
			}

			return decimalPosition;

		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override bool IsInDecimalPosition(string currentText, int startPosition)
		{
			bool isInDecimalPosition = false;
            int symbolPosition = CurrencySymbol.Length > 0 ? currentText.IndexOf(this.CurrencySymbol) : -1;

			if(symbolPosition != -1 && symbolPosition < startPosition)
			{//The symbol appears before the start position
				int decimalPosition = this.GetDecimalSeparatorPosition(currentText);
				if(decimalPosition != -1 && startPosition > decimalPosition)
				{
					isInDecimalPosition = true;
				}
			}
			else
			{	//The symbol appears before the start position
				int decimalSeparatorPosition = this.GetDecimalSeparatorPosition(currentText);

				if(decimalSeparatorPosition != -1 && startPosition > decimalSeparatorPosition)
				{
					isInDecimalPosition = true;
				}
			}

			

			return isInDecimalPosition;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string GetNumberValue(string formattedText, int startPosition)
		{
			decimal currencyValue = 0m;
			string extractedString = String.Empty;
			string modifiedRawText = String.Empty;
			bool negativeRemoved = false;

			if(startPosition < formattedText.Length)
				extractedString = formattedText.Substring(0, startPosition) + formattedText.Substring(startPosition);
			else
				extractedString = formattedText;
			try
			{
				modifiedRawText = RemoveFormatting(extractedString);
				if(modifiedRawText[0] == '-')
				{
					modifiedRawText = modifiedRawText.Substring(1);
					negativeRemoved = true;
				}
				currencyValue = Decimal.Parse(modifiedRawText,NumberStyles.Any,this.NumberFormatInfoObject);
				if(negativeRemoved)
					currencyValue = currencyValue * -1;

			}
			catch(Exception e)
			{
				// Raise an exception
				throw new Exception("Invalid value has been set for the currency.", e);
			}

			if(this.IsNegative && currencyValue > 0)
				currencyValue = currencyValue * -1;

			return currencyValue.ToString();
	
		}

		/// <summary>
		/// Override this to return CurrencyDecimalSeparator in CurrencyTextBox
		/// and PercentDecimalSeparator in PercentTextBox.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string GetDecimalSeparator(NumberFormatInfo info)
		{
			return info.CurrencyDecimalSeparator;
		}

		/// <summary>
		/// Override this to return CurrencyDecimalSeparator in CurrencyTextBox
		/// and PercentGroupSeparator in PercentTextBox.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string GetGroupSeparator(NumberFormatInfo info)
		{
			return info.CurrencyGroupSeparator;
		}

		/// <summary>
		/// Overrides <see cref="NumberTextBoxBase.ParseForNegativeFormat(string, NumberFormatInfo)"/>
		/// </summary>
		/// <param name="currentText">The text to be parsed.</param>
		/// <returns>True if the value is negative; false, otherwise</returns>
		protected override bool ParseForNegativeFormat(string currentText, NumberFormatInfo info)
		{
			string pattern = null;
			Match m = null;
			string currSymbol = info.CurrencySymbol;

			if(currSymbol == "$")
				currSymbol = "\\" + currSymbol;
			else if(currSymbol == String.Empty)
			{
				currSymbol = "\\$";
			}

			foreach (string sec in this.CurrencyNegativePatterns)
			{
				pattern = Regex.Replace(sec, "<CurrencySymbol>", currSymbol);
				pattern = Regex.Replace(pattern, "<CurrencySymbolLength>", currSymbol.Length.ToString());
				m = Regex.Match(currentText, pattern);

				if(m.Success == true)
					return true;
			}
			return false;
		}

		protected override string RemoveFormatting(string formattedText, NumberFormatInfo info, bool padIfEmpty)
		{
			string newText = formattedText;

			if(this.ContainsNumbers(this.CurrencySymbol) == true)
			{
				newText = formattedText.Replace(this.CurrencySymbol, "");
			}

			if(newText.IndexOf(this.CurrencySymbol) >= 0 && (this.CurrencySymbol != String.Empty) && (this.CurrencySymbol != ""))
				newText = newText.Replace(this.CurrencySymbol, "");

			return base.RemoveFormatting(newText, info, padIfEmpty);
		}

		protected override int GetStartPositionJustNumbers(string formattedText, int startPosition)
		{
			if(this.ContainsNumbers(this.CurrencySymbol) == true)
			{
				int currSymbolPos = formattedText.IndexOf(this.CurrencySymbol,0);
				if(startPosition >= currSymbolPos)
				{
					string newText = formattedText.Replace(this.CurrencySymbol, "");
					return base.GetStartPositionJustNumbers(newText, Math.Max(startPosition-this.CurrencySymbol.Length, 0));
				}
			}

			return base.GetStartPositionJustNumbers(formattedText, startPosition);
		}

		protected override void PositionCursorAfterEdit(int startPositionJustNumbers, bool inDecimalPosition, bool back, bool delete)
		{
			if(this.ContainsNumbers(this.CurrencySymbol) == true)
			{
				string currentText = base.GetTextBoxText();
				int currSymbolPos = currentText.IndexOf(this.CurrencySymbol,0);
				if(startPositionJustNumbers >= currSymbolPos)
				{
					int numbersCount = GetNumberCharactersCount(this.CurrencySymbol);
					base.PositionCursorAfterEdit(startPositionJustNumbers+numbersCount, inDecimalPosition, back, delete);
					return;
				}
			}

			if( m_bCorrectConcatModifiedText )
			{
				int currencyDecimalPosition = this.Text.IndexOf( this.CurrencyDecimalSeparator );
				if( currencyDecimalPosition != -1 )
				{
					this.SelectionStart = currencyDecimalPosition + this.CurrencyDecimalSeparator.Length
						+ m_iZerosNeedAddedCount + 1;
					return;
				}
			}

			base.PositionCursorAfterEdit(startPositionJustNumbers, inDecimalPosition, back, delete);
		}

		private bool ContainsNumbers(string s)
		{
			Match m = null;
			m = Regex.Match(s, "[0-9]");
			if(m.Success == true)
				return true;
			else
				return false;
		}

		private int GetNumberCharactersCount(string s)
		{
			int numbersCount = 0;
			for(int i = 0; i < s.Length; i++)
			{
				if (Char.IsDigit(s[i]))
					numbersCount++;
			}

			return numbersCount;
		}

		protected override bool HandleCharacterKey(char charToBeInserted) 
		{
			if(this.ContainsNumbers(this.CurrencySymbol) == true)
			{
				string currentText = base.GetTextBoxText();
				int currSymbolPos = currentText.IndexOf(this.CurrencySymbol,0);
				if(this.SelectionStart > currSymbolPos && this.SelectionStart < currSymbolPos + this.CurrencySymbol.Length)
				{
					this.SelectionStart = base.GetNextDataPos(currSymbolPos + this.CurrencySymbol.Length, false, false, false);
				}
			}

			bool returnValue = base.HandleCharacterKey(charToBeInserted);

			if( m_bCorrectConcatModifiedText )
			{
				if( charToBeInserted == c_cLeadingSymbol )
				{
					m_iZerosNeedAddedCount++;
				}
				else
				{
					m_bCorrectConcatModifiedText = false;
					m_iZerosNeedAddedCount = 0;
				}
			}

			return returnValue;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override int GetDecimalDigits()
		{
			return this.NumberFormatInfoObject.CurrencyDecimalDigits;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string ToggleNegative(string currentText)
		{
			decimal dValue = Decimal.Parse(currentText,NumberStyles.Any,this.NumberFormatInfoObject);
			dValue = dValue * -1;
			if(dValue == 0)
				this.SetZeroNegative(!this.GetZeroNegative());
			return dValue.ToString();
		}
		#endregion

		#region Overrides

		protected override bool IsDataPosition(string formattedText, int position)
		{
			if(position >= formattedText.Length)
				return false;

			// Replace the CurrencySymbol with some other Character
			StringBuilder valueText = new StringBuilder(formattedText);
			int currSymbolPos = formattedText.IndexOf(this.CurrencySymbol);
			if(currSymbolPos >= 0 && this.CurrencySymbol != "")
			{
				string replaceString = new String((char)256,this.CurrencySymbol.Length);
				valueText.Replace(this.CurrencySymbol,replaceString);
				formattedText = valueText.ToString();
			}

			return Char.IsDigit(formattedText[position]);
		}

		protected override NumberModifyState PrepareInsertString(string currentText, int startPosition, int selectionLength, string textToBeInserted, bool pasteOperation)
		{
			if (this.DecimalValue == 0m)
			{
				currentText = currentText.Substring(0, startPosition + selectionLength);
			}

			return base.PrepareInsertString(currentText, startPosition, selectionLength, textToBeInserted, pasteOperation);
		}

		#endregion

		#region VALIDATION

		/// <summary>
		/// Overrides the <see cref="System.Windows.Forms.Control.OnEnter"/> method.
		/// </summary>
		/// <param name="args">The event data.</param>
		/// <remarks>
		/// Saves the current DecimalValue so that it can be compared 
		/// during validation. The DecimalValueChanged and TextChanged event
		/// will only be raised if the value is different during validation.
		/// </remarks>
		protected override void OnEnter(EventArgs args)
		{
			this.enterDecimalValue = this.DecimalValue;
			base.OnEnter(args);
		}

		protected override void OnValidating(CancelEventArgs e) 
		{
			if(GetTextBoxText().Length == 0 && this.BindableValue != null) 
			{
				this.DecimalValue = 0m; 
			}
            oldDecimalValue = this.Text;
            if (MinMaxValidation == MinMaxValidation.OnLostFocus)
            {
                if (!(this.AllowNull && this.Text == this.NullString))
                {
                    if ((this.DecimalValue < this.MinValue || this.DecimalValue > this.MaxValue))
                    {
                        string invalidText = this.FormattedText;

                        switch (this.OnValidationFailed)
                        {
                            case OnValidationFailed.SetNullString:
                                if (this.AllowNull)
                                    this.SetTextBoxText(this.NullString);
                                else
                                {
                                    this.Focus();
                                    this.SelectionStart = this.Text.Length;
                                }
                                break;
                            case OnValidationFailed.SetMinOrMax:
                                this.DecimalValue = this.DecimalValue < this.MinValue ? this.MinValue : this.DecimalValue;
                                this.DecimalValue = this.DecimalValue > this.MaxValue ? this.MaxValue : this.DecimalValue;
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
            newDecimalValue = this.Text;
			base.OnValidating(e);
		}

        protected override void OnValidated(EventArgs e)
        {
            base.OnValidated(e);
            this.OnControlValidated(newDecimalValue, oldDecimalValue);
        }

		#endregion

	}
}   
        
