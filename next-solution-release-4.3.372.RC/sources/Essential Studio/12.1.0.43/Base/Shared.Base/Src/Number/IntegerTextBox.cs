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
	using System.Diagnostics;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Text;
	using System.Globalization;
	using System.Reflection;

	/// <summary>
	/// Extends the <see cref="System.Windows.Forms.TextBox"/> class to handle integer input
	/// and validation.
	/// </summary>
	/// <remarks>
	/// The IntegerTextBox is derived from textbox and provides all the functionality
	/// of a textbox and adds additional functionality of its own.
	/// <para>
	/// Collecting integer input in a consistent format requires validation code
	/// that needs to be built into the application when using the Windows Forms textbox control.
	/// The IntegerTextBox includes all this logic into its methods and properties
	/// and makes it easy for the developer and the end user to collect and enter double data.
	/// </para>
	/// <para>
	/// The IntegerTextBox is also closely tied to the globalization settings of the
	/// operating system for number related properties. Please refer to the 
	/// <see cref="System.Globalization.NumberFormatInfo"/>
	/// class for a detailed explanation of globalization and number related attributes.
	/// </para>
	/// <para>
	/// The IntegerTextBox has full support for the Windows Forms designer and you can
	/// just drag-and-drop and set properties on the control just as you would with the
	/// Windows Forms textbox.
	/// </para>
	/// <para>
	/// The IntegerTextBox also raises a <see cref="NumberTextBoxBase.ValidationError"/> event when
	/// inappropriate data is entered into the control.
	/// </para>
	/// <para>
	/// All clipboard functions such as copy, paste and cut are also supported with
	/// special accommodations for a number related issues.
	/// </para>
	/// </remarks>
	[
	ToolboxItem(true),
	ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.IntegerTextBox.bmp"),
	Description("Represents a TextBox to handle integer input and validation.")
	]
	public class IntegerTextBox: NumericTextBox
	{
		#region Constants

		private const char c_chZeroChar	= '0';
		private const char c_chMinusChar = '-';
		private const char c_chGroupSeparator = ',';
		#endregion

		#region FIELDS

        /// <summary>
        /// Specifies whether MinMaxValidation is being in progress.
        /// </summary>
        private bool m_bPerformingMinMaxValidation = false;

		/// <summary>
		/// The minimum value.
		/// </summary>
		private Int64 minValue = Int64.MinValue;

		/// <summary>
		/// The maximum value.
		/// </summary>
		private Int64 maxValue = Int64.MaxValue;

		/// <summary>
		/// The initial Int64 value set in InitializeComponent.
		/// </summary>
		private Int64 initIntegerValue = 0;

        /// <summary>
        /// 
        /// </summary>
        private string oldIntegerValue = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string newIntegerValue = string.Empty;

		/// <summary>
		/// The integer value when the control gets the focus. Used when validating.
		/// </summary>
		private Int64 enterIntegerValue = 0;

        /// <summary>
        /// The text when the control gets the focus. Used when validating.
        /// </summary>
        private string m_enterString = "";

		/// <summary>
		/// The Int64 value that is set through the IntegerValue property.
		/// </summary>
		private Int64 preservedIntegerValue = 0;

		/// <summary>
		/// Indicates whether to allow insets zero in the beginning value.
		/// </summary>
		private bool m_bAllowLeadingZeros = false;

        /// <summary>
        /// Indicates whether NullString should be set if UseNullString is true.
        /// </summary>
        private bool m_bIsNullValue = false;
		/// <summary>
		/// Occurs when the <see cref="Syncfusion.Windows.Forms.Tools.CurrencyTextBox.DecimalValue"/> property is changed.
		/// </summary>
		[Category("PropertyChanged")]
		[Description("Occurs when the IntegerValue property is changed.")]
		public event EventHandler IntegerValueChanged;

		#endregion

        #region PROPERTIES

        ///// <summary>
        ///// Specifies when the MinMax Validation Need to be performed.
        ///// </summary>
        //[
        //Description("Specifies when the MinMax Validation Need to be performed."),
        //DefaultValue(typeof(MinMaxValidation), "OnKeyPress")
        //]
        //public MinMaxValidation MinMaxValidation
        //{
        //    get
        //    {
        //        return this.m_MinMaxValidation;
        //    }
        //    set
        //    {
        //        if (this.m_MinMaxValidation != value)
        //            this.m_MinMaxValidation = value;
        //    }
        //}

        #endregion

        #region INITIALIZATION

		/// <summary>
        /// Overloaded. Creates an object of type IntegerTextBox. 
		/// </summary>
		/// <remarks>
		/// The IntegerTextBox object will be initialized with the default values
		/// for the display and data properties. You need to set any specific 
		/// values.
		/// </remarks>
		public IntegerTextBox() 
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			   new Syncfusion.Core.Licensing.LicensedComponent(typeof(IntegerTextBox));
			}
			finally
			{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			this.SetDefaultValue((Int64)0);
			this.NumberDecimalDigits = 0;
			InitializeComponent();
			//this.IntegerValue = 0;
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
			if (this.initIntegerValue == 0 && AllowNull && this.Text == this.NullString)
				setNullString = true;

			base.InitializeNumberTextBox();
			this.SetNumberFormatInfoInitValues();

			if (this.initIntegerValue != 0 )
				this.IntegerValue = this.initIntegerValue;
			else if (setNullString)
			{
				this.SetNullNumberValue();
				this.SetTextBoxText(this.NullString);
			}

			if(this.CheckForMinMax(this.FormattedText, true) == false)
				this.IntegerValue = this.MinValue;

            base.ignoreTextChange = false;
		}
		#endregion

		#region DATA

		/// <summary>
		/// Overrides the Text property of <see cref="System.Windows.Forms.TextBox"/>.
		/// </summary>
		/// <remarks>
		/// This property is overriden in order to normalize the data that is set
		/// to the Text property and format it as needed. The method <see cref="Syncfusion.Windows.Forms.Tools.NumberTextBoxBase.InsertString(String, int, int, string, bool)"/>
		/// is used to format the data.
		/// </remarks>
		[
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		RefreshProperties(RefreshProperties.Repaint)
		]
		public new string Text
		{
			get
			{
                return base.Text;
			}

			set
			{
                if( this.Parent != null && base.Text != value )
                {
                    this.SetTextProperty( value );
                    if (this.MinMaxValidation == MinMaxValidation.OnLostFocus || this.MinMaxValidation == MinMaxValidation.OnKeyPress)
                    {
                        this.PerformMinMaxValidation();
                    }
                }
			}
		}
        public override void ResetText()
        {
            if (this.AllowNull)
                base.ResetText();
            else
                this.IntegerValue = 0;
            
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
        protected override void SetTextProperty(string newText)
		{
            Int64 parsedValue = 0;
			bool success = true;

			if (this.AllowNull && newText == this.NullString)
			{
				this.SetNullNumberValue();
				this.SetTextBoxText(this.NullString);
				return;
			}

			if(newText == null || newText == String.Empty)
				success = false;
			else
			{
				try
				{
					// Try to parse the text using the Decimal.Parse method.
					parsedValue = Int64.Parse(newText, NumberStyles.Number, this.NumberFormatInfoObject);
				
					if( this.AllowLeadingZeros )
					{
						string zero = this.SaveZeroInValue(	this.GetNumberValue( newText, 0 ), ( parsedValue < 0 ) );
						this.ApplyFormattingAndSetText( zero + Math.Abs( parsedValue ).ToString() );
					}
					else
					{
                        this.ApplyFormattingAndSetText( parsedValue.ToString() );
					}
				}
				catch
				{
					success = false;
				}
			}
           
			if( !success )
			{
				base.SetTextProperty( newText );

				try
				{
					// Can't use Int64.TryParse() - this method is absent in .NET 1.0 and 1.1
					IntegerValue = Int64.Parse( base.Text, NumberStyles.Number, this.NumberFormatInfoObject );
				}
				catch
				{
				}
			}
            else
            {
                IntegerValue = parsedValue;
            }
		}


		/// <summary>
		/// Gets / sets the integer value of the control. This will be formatted and
		/// displayed.
		/// </summary>
		[
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Data"),
		Description("The integer value of the text."),
		RefreshProperties(RefreshProperties.Repaint), 
		]
		public Int64 IntegerValue
		{
			get
			{
				if(this.GetPreserveData() == true)
					return this.preservedIntegerValue;
				else
					return Convert.ToInt64(this.GetNumberValue(base.Text,0));
			}

			set
			{
				if(this.Initializing == false)
					this.SetIntegerValue(value);
				else
                    this.initIntegerValue = value;
			}
		}

		protected void SetIntegerValue(Int64 newValue)
		{

			this.SelectionStart = 0;
			this.SelectionLength = base.TextLength;
			if(this.CheckForMinMax(newValue.ToString(),true) == true)
			{
				// Set the IntegerValue before TextChanged event is raised.
				this.preservedIntegerValue = newValue;
				this.SetPreserveData(true);
				// Text is set and TextChanged and IntegerValueChanged events are raised.

                this.ApplyFormattingAndSetText( newValue.ToString() );                
			}
		}

		/// <summary>
		/// Gets / sets the maximum value that can be set through the IntegerTextBox.
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		[
		Category("Behavior")
		]
		public Int64 MaxValue
		{
			get
			{
				return this.maxValue;
			}

			set
			{
				if (value < this.MinValue)
				{
					this.maxValue = Int64.MaxValue;
					throw new ArgumentOutOfRangeException("MaxValue", value, "MaxValue Cannot be lesser than MinValue");
				}
				this.maxValue = value;
                if (!this.Initializing && !CheckForMinMax(this.IntegerValue.ToString()))
                {
                    this.IntegerValue = value;
                }
			}
		}

		/// <summary>
		/// Indicates whether the MaxValue property should be serialized.
		/// </summary>
		/// <returns>True if the value is not equal to <see cref="double.MaxValue"/>.</returns>
		private bool ShouldSerializeMaxValue()
		{
			if(this.maxValue != Int64.MaxValue)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Resets the max value to the default.
		/// </summary>
		private void ResetMaxValue()
		{
			this.MaxValue = Int64.MaxValue;
		}

		/// <summary>
		/// Generates string format for use in Conver.Format.
		/// </summary>
		/// <param name="valueLength"></param>
		/// <param name="negative"></param>
		/// <param name="gs"></param>
		/// <returns></returns>
		private string GenerateFormat(int valueLength, bool negative ,int[] gs)
		{
			string format = string.Empty;
			int num = 0;
			int num2 = 0;			
			int length = valueLength;
					
			if( negative ) 
			{
				length--;
			}

			for( int i = 0 ; length > i; ++i, ++num2 )
			{
				if( gs != null && gs.Length > 0 &&
						num2 == gs[num] && gs[num] != 0 )
				{
					format = c_chGroupSeparator + format;  
					num2 = 0;

					if( gs.Length - 1 > num ) 
					{
						num++;
					}
				}

				format = c_chZeroChar + format;
			}

			return format;
		}
    
		/// <summary>
		/// Adds negative symbol.
		/// </summary>
		/// <param name="sValue"></param>
		/// <param name="negativ"></param>
		/// <param name="numberNegativePattern"></param>
		/// <returns></returns>
		private string AddNegaviveSymbol( string sValue, string negativ, int numberNegativePattern )
		{				
			string returnString = String.Empty;

			switch ( numberNegativePattern )
			{					
				case 0:
					returnString = String.Format( "({0})", sValue );
					break;

				case 1:
					returnString = String.Format( "{0}{1}", negativ, sValue );
				break;

				case 2:
					returnString = String.Format( "{0} {1}", negativ, sValue );
					break;

				case 3:
					returnString = String.Format( "{0}{1}", sValue, negativ );
					break;

				case 4:
					returnString = String.Format( "{0} {1}", sValue, negativ );
					break;
			}
			return returnString;

		}

		/// <summary>
		/// Returns zero string as began sValue.
		/// </summary>
		/// <param name="sValue"></param>
		/// <param name="Negative"></param>
		/// <returns></returns>
		private string SaveZeroInValue(string sValue, bool Negative)
		{
			string zero = ( Negative ) ? c_chMinusChar.ToString() : string.Empty;
			int i = 0;
      
			if( sValue.Length > 0 && sValue[0] == c_chMinusChar ) 
			{
				i = 1;
			}
							
			for( ; sValue.Length - 1 > i && sValue[i] == c_chZeroChar ; ++i )
			{
				zero += c_chZeroChar;
			}
			return zero;
		}
		

		/// <summary>
		/// Gets / sets the minimum value that can be set through the IntegerTextBox.
		/// </summary>
		[
		Category("Behavior")
		]
		public Int64 MinValue
		{
			get
			{
				return this.minValue;
			}

			set
			{
				if (value > this.MaxValue)
				{
					this.minValue = Int64.MinValue;
					throw new ArgumentOutOfRangeException("MinValue", value, "MinValue Cannot be greater than MaxValue");
				}
				this.minValue = value;
                if (!this.Initializing && !CheckForMinMax(this.IntegerValue.ToString()))
                {
                    this.IntegerValue = value;
                }
            }
		}


		/// <summary>
		/// Indicates whether the MinValue property should be serialized.
		/// </summary>
		/// <returns>True if the value is not equal to <see cref="decimal.MaxValue"/>.</returns>
		private bool ShouldSerializeMinValue()
		{
			if(this.minValue != Int64.MinValue)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Resets the value to the default.
		/// </summary>
		private void ResetMinValue()
		{
			this.MinValue = Int64.MinValue;
		}

		/// <summary>
		/// Indicates whether to allow insets zero in the beginning value.
		/// </summary>
		[
			DefaultValue( false ),
			Category( "Behavior" ),
			Description( "Gets or sets allow insets zero in begining value." )
		]
		public bool AllowLeadingZeros
		{
			get
			{
				return this.m_bAllowLeadingZeros;
			}
			set
			{
				if( this.m_bAllowLeadingZeros != value )
				{
					this.m_bAllowLeadingZeros = value;
					this.SetTextProperty( this.GetTextBoxText() );
				}
			}
		}

        [
           DefaultValue(false), Browsable(false),
           Category("Behavior"),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
           Description(@"Gets or sets value that indicates whether NullString should be set if UseNullString is true."),
           Obsolete("This property will not be used in future. Use NullState property to get the null state of the text box")
        ]
        public bool IsNullValue
        {
            get
            {
                return m_bIsNullValue;
            }

            set
            {/*
                if( !this.Initializing )
                {
                    if( this.UseNullString )
                    {*/
                        m_bIsNullValue = value;
                        /*if( m_bIsNullValue )
                        {
                            this.SetTextBoxText( this.NullString );
                        }
                        else
                        {
                            this.SetTextBoxText( this.IntegerValue.ToString() );
                        }
                    }
                    else
                    {
                        throw new ArgumentException( "The value can be changed only if UseNullString is true." );
                    }
                }
                else
                {
                    m_initIsNullValue = value;
                }*/
            }
        }
 
        [
           DefaultValue(false), Browsable(false),
           Category("Behavior"),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
           Description(@"Gets or sets value that indicates whether NullString should be set"),
           Obsolete("This property will not be used in future.")
        ]
        public new bool UseNullString
        {
            get
            {
                return base.UseNullString;
            }

            set
            {/*
                if( base.UseNullString != value )
                {*/
                    base.UseNullString = value;

                    /*if( !value && m_bIsNullValue )
                    {
                        m_bIsNullValue = false;
                        this.SetTextBoxText( this.IntegerValue.ToString() );
                    }
                }*/
            }
        }
        protected override bool CheckNullStringIsInRange(string nullString)
        {
            Int64 integerValue;
            bool isNumber = Int64.TryParse(nullString, NumberStyles.Any, this.NumberFormatInfoObject, out integerValue);
            if (isNumber)
            {
                return CheckForMinMax(integerValue.ToString(), true);
            }
            else
                return true;
        }
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override bool CheckForMinMax(string currentTextValue, bool ignoreLength)
		{
            if (this.MinMaxValidation == MinMaxValidation.OnLostFocus)
            {
                if(!this.m_bPerformingMinMaxValidation)
                    return true;
            }
			bool returnValue = true;
			string absCurrentTextValue = this.RemoveFormatting(currentTextValue);
            Int64 numericValue=0;
            bool isNum = Int64.TryParse(GetNumberValue(absCurrentTextValue, 0), NumberStyles.Any, this.NumberFormatInfoObject, out numericValue);

            // The below block of code is removed as the length need not to be considered with the current implementation as we've the 
            // MinMaxValidation property that determines whether the control has to perform MinMaxValidation on KeyPress or on 
            // Focus Lost.

            // The old behavior of the control is to perform the validaton based on the length of the input string and the value,
            // instead it's purely based on the value alone henceforth.

            // User can set the MinMaxValidation to be OnLostFocus if he wishes to allow the users to enter character and perform validation 
            // only when they are done with the input.

			/* If we take the length of the current text into account, we have
			 to find the length of the MaxValue and MinValue and then do the
			 comparison - otherwise there will no be meaningful comparison - like
			 when the user is still entering input.
			if(ignoreLength == false)
			{
				int numericValueLength, minValueLength, maxValueLength;

				numericValueLength = this.GetNumberPartLength(absCurrentTextValue);

				Int64 min = ( this.MinValue < 0 ) ? -this.MinValue : this.MinValue;
				Int64 max = ( this.MaxValue < 0 ) ? -this.MaxValue : this.MaxValue;

				minValueLength = this.GetNumberPartLength( min );
				maxValueLength = this.GetNumberPartLength( max );

				numericValue = Convert.ToInt64(this.GetNumberValue(absCurrentTextValue, 0));

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
			}
			else */
			{
                if (currentTextValue==String.Empty && this.AllowNull)
                    return true;
                if(numericValue < this.MinValue || numericValue > this.MaxValue)
					returnValue = false;
			}

			return returnValue;
		}

		private Int64 GetAbsValue(Int64 val)
		{
			// Change the value to positive if needed.
			if(val < 0)
			{
				// If value is very close to Int64.Min, the negative conversion will not work
				// (as the positive value will be higher than Int64.Max
				if(val < (Int64.MinValue + 1))
					val = val +1;

				// Multiply by -1 to make the value positive
				val = val * -1;
			}
			return val;
		}

		/// <summary>
		/// Raises the <see cref="IntegerTextBox.IntegerValueChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnIntegerValueChanged(EventArgs e)
		{
            if (base.ignoreTextChange != true && IntegerValueChanged != null)
            {
                IntegerValueChanged(this, e);
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
		protected override void OnTextChanged(EventArgs e)
		{
            bool bPreserve = GetPreserveData();
            SetPreserveData( false );
            // Dont need to check for ignoreTextChange as the 
            // functions being called check that
            base.OnTextChanged( e );
            this.OnIntegerValueChanged( e );

           


            SetPreserveData( bPreserve );

            
		}

		/// <summary>
		/// Overrides the <see cref="System.Windows.Forms.Control.OnEnter"/> method.
		/// </summary>
		/// <param name="args">The event data.</param>
		/// <remarks>
		/// Saves the current IntegerValue so that it can be compared 
		/// during validation. The IntegerValueChanged and TextChanged event
		/// will only be raised if the value is different during validation.
		/// </remarks>
		protected override void OnEnter(EventArgs args)
		{
			this.enterIntegerValue = this.IntegerValue;
            this.m_enterString = base.Text;

			base.OnEnter(args);
		}

		protected override void OnValidating(CancelEventArgs e) 
		{
            oldIntegerValue = this.Text;
            if (this.MinMaxValidation == MinMaxValidation.OnLostFocus)
            {
                if (!(this.AllowNull && this.Text == this.NullString))
                    this.PerformMinMaxValidation();
            }
            newIntegerValue = this.Text;

            if (Text.Length == 0 && this.BindableValue != null)
            {
                this.IntegerValue = 0;
            }
            
            if( this.m_enterString != base.Text )
            {
                OnBindableValueChanged( EventArgs.Empty );
            } 
            
            base.OnValidating( e );
		}

        protected override void OnValidated(EventArgs e)
        {
            base.OnValidated(e);
            OnControlValidated(newIntegerValue, oldIntegerValue);
        }

        /// <summary>
        /// performs the Min/Max validation.
        /// </summary>
        protected virtual void PerformMinMaxValidation()
        {
            this.m_bPerformingMinMaxValidation = true;
            string currentText = this.GetTextBoxText();
            int length = this.GetTextBoxTextLength();
            string textToBeInserted = currentText;
            
            String invalidText = this.FormattedText;
            bool bRaiseValidationError = false;
            long currentValue = Convert.ToInt64(this.GetNumberValue(currentText, 0));
            
            if ((currentValue < this.MinValue || currentValue > this.MaxValue))
            {
            bRaiseValidationError = true;
            switch (this.OnValidationFailed)
            {
                case OnValidationFailed.SetNullString:
                    if (this.AllowNull)
                        textToBeInserted = this.NullString;
                    else
                    {
                        this.Focus();
                        this.SelectionStart = this.Text.Length;
                    }
                    break;
                case OnValidationFailed.SetMinOrMax:
                    currentValue = currentValue < this.MinValue ? this.MinValue : currentValue;
                    currentValue = currentValue > this.MaxValue ? this.MaxValue : currentValue;
                    textToBeInserted = currentValue.ToString();
                    break;
                case OnValidationFailed.KeepFocus:
                    {
                        this.Focus();
                        this.SelectionStart = this.Text.Length;
                    }
                    break;
               
            }
            }
            bool previousState = this.ignoreTextChange;
            this.ignoreTextChange = true;
            this.SetTextBoxText(string.Empty);
            //foreach (char charToBeInserted in textToBeInserted)
            //   this.HandleCharacterKey(charToBeInserted);
            //Fix if an empty string is returned.
            if (this.Text == string.Empty)
            {
                this.SetTextBoxText(textToBeInserted);
            }
            
            this.ignoreTextChange = previousState;
            this.m_bPerformingMinMaxValidation = false;

            //Raises the ValidationError here as the CheckforMinMax will always return True with MinMaxValidation.OnLostFocus
            if(bRaiseValidationError)
                this.RaiseValidationError(invalidText, 0, "Current value does not meet MinValue and MaxValue requirements.");
        }

		protected override void SetNullNumberValue()
		{
			this.preservedIntegerValue = 0;
			this.SetPreserveData(true);
		}

		protected override bool IsAssignable(object val)
		{
			if(val == null || Convert.IsDBNull(val) == true)
				return false;

			bool assignable = typeof(Int64).IsAssignableFrom(val.GetType()) || typeof(Int32).IsAssignableFrom(val.GetType())
                                || typeof(Int16).IsAssignableFrom(val.GetType());
			return assignable;
		}

		protected override void SetValue(object val)
		{
			if(val.GetType() == typeof(Int64))
				this.IntegerValue = (Int64)val;
			else if(val.GetType() == typeof(Int32))
				this.IntegerValue = (Int32)val;
		   else if (val.GetType() == typeof(Int16))
                this.IntegerValue = (Int16)val;

        }

		protected override object GetValue()
		{
			return this.IntegerValue;
		}

		#endregion

		#region FORMATTING

		/// <summary>
		/// Gets / sets the decimal separator character that will be used for the display.
		/// </summary>
		/// <remarks>
		/// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
		/// and can be changed based on your requirements or based on the locale.
		/// </remarks>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new string NumberDecimalSeparator
		{
			get
			{
				return " ";
			}

			set
			{
			}
		}

		/// <summary>
		/// Gets / sets the maximum number of digits for the decimal portion.
		/// </summary>
		/// <remarks>
		/// This property is always set to zero for the IntegerTextBox.
		/// </remarks>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new int NumberDecimalDigits
		{
			get
			{
				return 0;
			}

			set
			{
				this.NumberFormatInfoObject.NumberDecimalDigits = 0;
			}
		}

        /// <summary>
        /// Indicates whether to serialize <see cref="NumberDecimalDigits"/>  property value.
        /// </summary>
        /// <returns></returns>
		public bool ShouldSerializeNumberDecimalDigits()
		{
			return false;
		}

        /// <summary>
        /// Indicates whether to serialize <see cref="NumberDecimalSeparator"/>  property value
        /// </summary>
        /// <returns></returns>
		public new bool ShouldSerializeNumberDecimalSeparator()
		{
			return false;
		}

		#endregion

		#region INTERNAL OPERATIONS

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int GetNumberPartLength(Int64 numberValue)
		{
			return this.GetNumberPartLength(numberValue.ToString());
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string GetNumberValue(string formattedText, int startPosition)
		{
			Int64 iValue = 0;
			string extractedString = String.Empty;
			string modifiedRawText = String.Empty;

			if(startPosition < formattedText.Length)
				extractedString = formattedText.Substring(0, startPosition) + formattedText.Substring(startPosition);
			else
				extractedString = formattedText;
			try
			{
				modifiedRawText = RemoveFormatting(extractedString);
				if(modifiedRawText == String.Empty)
					modifiedRawText = "0";
				iValue = Int64.Parse(modifiedRawText,NumberStyles.Any,this.NumberFormatInfoObject);
			}
			catch
			{
				// Rollback the operation.
				rollBackOperation = true;
			}

			if(this.IsNegative && iValue > 0)
			{
				iValue = iValue * -1;
			}

			string returnValue = iValue.ToString();
			
			if( this.AllowLeadingZeros )
			{
				string zero = this.SaveZeroInValue( modifiedRawText, ( iValue < 0 ) );
				returnValue = zero + Math.Abs(iValue).ToString();
			}

			return returnValue;	
		}
        protected override bool CheckIsZero()
        {
            return (!this.IsNull && this.IntegerValue.Equals(0));
        }
		protected override bool IsValidNumberValue(string intString)
		{
			bool valid = true;
			Int64 iValue = 0;
			string intPartString = intString;
			try
			{
				string decimalSeparator = this.NumberFormatInfoObject.NumberDecimalSeparator;
				int decimalSeparatorPos = -1;
				if(decimalSeparator != null && decimalSeparator != String.Empty)
				{
					decimalSeparatorPos = intString.IndexOf(decimalSeparator);
					if(decimalSeparatorPos != -1)
						intPartString = intString.Substring(0, decimalSeparatorPos);
				}

				iValue = Int64.Parse(intPartString);
			}
			catch
			{
				valid = false;
			}
			return valid;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override bool CheckIfNegative(string rawValue)
		{
			if(rawValue != null && rawValue != String.Empty)
			{
				Int64 iValue = Convert.ToInt64(rawValue);
				if(iValue >= 0)
					return false;
				else
					return true;
			}
			return false;

		}

		protected override int GetInitialZeroCount(string currentText, int startPosition)
		{
			if( this.AllowLeadingZeros )
			{
				return 0;
			}
			else
			{
				return base.GetInitialZeroCount( currentText, startPosition );
			}
		}
 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="previousFormat"></param>
		protected override void FormatChanged(string currentText,NumberFormatInfo previousFormat)
		{
			Int64 currentValue = 0;

			if(this.Initializing == false)
			{
				try
				{
					if(this.GetPreserveData() == true)
						currentValue = this.IntegerValue;
					else
					{
						currentText = currentText.Trim();
						if(currentText != String.Empty)
						{
							currentValue = Int64.Parse(currentText,NumberStyles.Integer,previousFormat);							
						}
					}
				}
				catch
				{
					currentValue = Convert.ToInt64(this.GetNumberValue(currentText, 0));
				}
				finally
				{	
					if( this.AllowLeadingZeros )
					{
						string zero = this.SaveZeroInValue(	this.GetNumberValue( currentText, 0 ), ( currentValue < 0 ) );
						this.ApplyFormattingAndSetText( zero + Math.Abs( currentValue ).ToString() );
					}
					else
					{
						this.ApplyFormattingAndSetText(currentValue.ToString());
					}
				}
			}
		}

		protected override bool HandleDecimalKey()
		{
			return true;
		}

        protected override bool HandleBackspaceKey()
        {
            if( this.ReadOnly == true )
                return false;

            int startPosition = this.SelectionStart;
            string currentText = this.GetTextBoxText();
            string modifiedRawText = String.Empty;
            int startPositionJustNumbers = 0;
            bool deleteInDecimalPosition = false;
            int realSelectionLength = 0;

            startPositionJustNumbers = this.GetStartPositionJustNumbers(currentText, startPosition);

            // Find if the cursor is after the decimal separator.
            int decimalSeparatorPosition = this.GetDecimalSeparatorPosition( currentText );
            deleteInDecimalPosition = this.IsInDecimalPosition( currentText, startPosition );
            if( startPosition == decimalSeparatorPosition + this.GetDecimalSeparator().Length )
                deleteInDecimalPosition = false;

            // Change the text and put it in the control.
            if( this.SelectionLength == 0 )
            {
                realSelectionLength = 0;
                int oldStartPosition = startPosition;
                startPosition = GetPrevDataPos( startPosition - 1 );
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

            if( !CheckForMinMax( currentText ) )
                return true;

            // Check for null state.
            if( currentText == String.Empty )
            {
                if( this.AllowNull )
                {
                    if( !RaiseSetNull( currentText ) )
                    {
                        this.SetNullNumberValue();
                        SetTextBoxText( this.NullString );
                    }
                }
                else
                {
                    currentText = ( string ) this.DefaultValue.ToString();
                    if( this.IsValidNumberValue( RemoveFormatting( currentText ) ) )
                    {
                        // Round trip the formatted text.
                        currentText = this.ApplyFormattingAndSetText( this.GetNumberValue( currentText, startPosition ) );
                        this.PositionCursorAfterEdit( startPositionJustNumbers, deleteInDecimalPosition, true, false );
                    }
                }
            }
            else if( this.Text != this.NullString && this.IsValidNumberValue( RemoveFormatting( currentText ) ) )
            {
                // Round trip the formatted text.
                currentText = this.ApplyFormattingAndSetText( this.GetNumberValue( currentText, startPosition ) );
                this.PositionCursorAfterEdit( startPositionJustNumbers, deleteInDecimalPosition, true, false );
            }
            this.SetControlColor();
            return true;
        }

		protected override bool HandleDeleteKey() 
		{
			if( this.ReadOnly == true )
				return false;

			int startPosition = this.SelectionStart;
			string currentText = this.GetTextBoxText();
			string modifiedRawText = null;
			int startPositionJustNumbers = 0;
			bool inDecimalPosition = false;

			startPositionJustNumbers = this.GetStartPositionJustNumbers( currentText, startPosition );

			inDecimalPosition = this.IsInDecimalPosition( currentText, startPosition );

			// If there is no selection, fake a selection from a starting point to another valid
			// position and then delete the fake selected text.
			if( this.SelectionLength == 0)
			{
				// Special case - when the start position is in the number part and there is only 
				// a 0, the cursor should be moved to the next position.
				int prevDataPos = this.GetPrevDataPos( Math.Max( 0,startPosition - 1 ) );
				if( inDecimalPosition == false && prevDataPos == this.GetFirstDataPos() )
				{
					if(currentText != String.Empty && currentText[ prevDataPos ] == '0')
						startPosition = CursorShouldBeMoved( startPosition,inDecimalPosition );
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
				if( !IsDataPosition( currentText,startPosition ) )
					startPosition = GetNextDataPos( startPosition,inDecimalPosition, false, false );

				int newEndPosition = 0;
				if( inDecimalPosition )
					newEndPosition = GetNextDataPos( startPosition,inDecimalPosition, false, false );
				else
					newEndPosition = GetNextDataPos( startPosition+1,inDecimalPosition, false, false );
				this.SelectionStart = startPosition;
				this.SelectionLength = newEndPosition - startPosition;
			}
            //Deletes All the Text When Selection Length is Zero and the Value is '0'  [this.CheckIsZero()] 
            if(this.CheckIsZero())
                this.SelectAll();
            
			currentText = DeleteSelectedText();
			
			if( !CheckForMinMax( currentText ) ) return true;

			// Check for NULL state.
			if( currentText == String.Empty )
			{
                if( this.AllowNull )
				{
					if (!RaiseSetNull(modifiedRawText))
					{
						this.SetNullNumberValue();
						SetTextBoxText( this.NullString );
					}
				}
				else
				{
					modifiedRawText = ( string ) this.DefaultValue.ToString();
					if( this.IsValidNumberValue( modifiedRawText ) )
					{
						currentText = ApplyFormattingAndSetText( this.GetNumberValue( modifiedRawText, startPosition ) );
						this.PositionCursorAfterEdit( startPositionJustNumbers, inDecimalPosition, false, true );
					}
				}
			}
			else if( this.Text != this.NullString)
			{
                modifiedRawText = RemoveFormatting(currentText.Substring(0, startPosition) + currentText.Substring(startPosition));
                if (this.IsValidNumberValue(modifiedRawText))
                {
                    currentText = ApplyFormattingAndSetText(this.GetNumberValue(modifiedRawText, startPosition));
                    this.PositionCursorAfterEdit(startPositionJustNumbers, inDecimalPosition, false, true);
                }
			}
			this.SetControlColor();
			return true;
		}

        protected override bool InsertString( string currentText, int startPosition, int selectionLength, string textToBeInserted, bool pasteOperation )
        {
            if( this.AllowNull && ( textToBeInserted == String.Empty || textToBeInserted == this.NullString ) )
            {
                this.SetNullNumberValue();
                this.SetTextBoxText( this.NullString );
                return true;
            }
            return base.InsertString( currentText, startPosition, selectionLength, textToBeInserted, pasteOperation );
        }

		/// <summary>
		/// Formats the given text according to the current setting.
		/// </summary>
		/// <param name="rawValue"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string ApplyFormatting(string rawValue) 
		{
			string returnString;

			Int64 iValue = (rawValue != null && rawValue != String.Empty) ? Convert.ToInt64(rawValue) : 0;

			string strNull = this.NullString; 

			if (rawValue == null || rawValue == strNull)
			{
				returnString = String.Format(this.NullFormat, "{0}", strNull);
			}
			else
			{				
				if( !this.AllowLeadingZeros )
				{				
					returnString = String.Format(this.NumberFormatInfoObject, "{0:n}", iValue);
				}
				else
				{
					string format = this.GenerateFormat( rawValue.Length, ( iValue < 0 ), 
						this.NumberFormatInfoObject.NumberGroupSizes );

					returnString = String.Format(this.NumberFormatInfoObject, "{0:" + format + "}", Math.Abs( iValue ));
					
					if( iValue < 0 )
					{
						returnString = AddNegaviveSymbol( returnString, 
							this.NumberFormatInfoObject.NegativeSign, 
							this.NumberFormatInfoObject.NumberNegativePattern );						
					}
				
				}				
			}

			return returnString.Trim();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string ToggleNegative(string currentText)
		{
			Int64 iValue = Convert.ToInt64(currentText);
			iValue = iValue * -1;
			if(iValue == 0)
				this.SetZeroNegative(!this.GetZeroNegative());
			
			string returnValue = iValue.ToString();
			
			if( this.AllowLeadingZeros )
			{
				returnValue = currentText;
			}
			return returnValue;
		}

		protected override int CursorShouldBeMoved(int startPosition, bool inDecimalPosition)
		{			
			int returnValue = startPosition;

			if( !this.AllowLeadingZeros )
			{
				returnValue = base.CursorShouldBeMoved (startPosition, inDecimalPosition);
			}
			return returnValue;			
		}

		//[Syncfusion.Documentation.DocumentationExclude()]
		//internal override void OnNullStringChanged()
		//{
			//if (this.AllowNull)
			//{
			//	SetTextBoxText(this.NullString);
			//}
		//}
		#endregion

		#region Overrides

		protected override NumberModifyState PrepareInsertString( string currentText, int startPosition, int selectionLength, string textToBeInserted, bool pasteOperation )
		{
			if( this.IntegerValue == 0 )
			{
				currentText = currentText.Substring( 0, startPosition + selectionLength );
			}

			return base.PrepareInsertString( currentText, startPosition, selectionLength, textToBeInserted, pasteOperation );
		}

		#endregion
    }
}   
        
