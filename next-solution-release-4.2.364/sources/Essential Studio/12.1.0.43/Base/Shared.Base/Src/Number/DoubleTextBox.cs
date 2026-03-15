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
	/// Extends the <see cref="System.Windows.Forms.TextBox"/> class to handle double input
	/// and validation.
	/// </summary>
	/// <remarks>
	/// The DoubleTextBox is derived from textbox and provides all the functionality
	/// of a textbox and adds additional functionality of its own.
	/// <para>
	/// Collecting double input in a consistent format requires validation code
	/// that needs to be built into the application when using the Windows Forms text box control.
	/// The DoubleTextBox includes all this logic into its methods and properties
	/// and makes it easy for the developer and the end user to collect and enter double data.
	/// </para>
	/// <para>
	/// The DoubleTextBox is also closely tied to the globalization settings of the
	/// operating system for number related properties. Please refer to the 
	/// <see cref="System.Globalization.NumberFormatInfo"/>
	/// class for a detailed explanation of globalization and number related attributes.
	/// </para>
	/// <para>
	/// The DoubleTextBox has full support for the Windows Forms designer and you can
	/// just drag-and-drop and set properties on the control just as you would with the
	/// Windows Forms text box.
	/// </para>
	/// <para>
	/// The DoubleTextBox also raises a <see cref="NumberTextBoxBase.ValidationError"/> event when
	/// in appropriate data is entered into the control.
	/// </para>
	/// <para>
	/// All clipboard functions such as copy, paste and cut are also supported with
	/// special accommodations for number related issues.
	/// </para>
	/// </remarks>
	[
	ToolboxItem(true),
	ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.DoubleTextBox.bmp"),
	Description("Represents a TextBox to handle double input and validation.")
	]
	public class DoubleTextBox: NumericTextBox
	{
		#region FIELDS

		/// <summary>
		/// The minimum value.
		/// </summary>
		private double minValue = double.MinValue;

        /// <summary>
        /// 
        /// </summary>
        private string newDoubleValue = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string oldDoubleValue = string.Empty;

		/// <summary>
		/// The maximum value.
		/// </summary>
		private double maxValue = double.MaxValue;

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
		/// Occurs when the <see cref="DoubleValue"/> property is changed.
		/// </summary>
		[Category("PropertyChanged")]
		[Description("Occurs when the DoubleValue property is changed.")]
		public event EventHandler DoubleValueChanged;

		/// <summary>
		/// The line of the null value.
		/// </summary>
		private const string DEF_NULL_VALUE = "0";

        private bool m_bIsNullValue = false;


		#endregion

		#region INITIALIZATION

		/// <summary>
        /// Overloaded. Creates an object of type DoubleTextBox. 
		/// </summary>
		/// <remarks>
		/// The DoubleTextBox object will be initialized with the default values
		/// for the display and data properties. You need to set any specific 
		/// values.
		/// </remarks>
		public DoubleTextBox() 
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			   new Syncfusion.Core.Licensing.LicensedComponent(typeof(DoubleTextBox));
			}
			finally
			{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			this.SetDefaultValue((Double)0);
			InitializeComponent();
		}

		/// <summary>
		/// 
		/// </summary>
		private void InitializeComponent() 
		{
			this.Multiline = false;
			this.NullString = String.Empty;
			//this.DoubleValue = 0.0;
		}

		/// <summary>
		/// Overrides <see cref="NumberTextBoxBase.InitializeNumberTextBox"/>.
		/// </summary>
		protected override void InitializeNumberTextBox()
		{
			base.ignoreTextChange = true;
			bool setNullString = false;

			if (this.initDoubleValue == 0 && AllowNull && this.Text == this.NullString)
				setNullString = true;

			base.InitializeNumberTextBox();
			this.SetNumberFormatInfoInitValues();

			if (setNullString)
			{
				this.SetNullNumberValue();
				this.SetTextBoxText(this.NullString);
			}
			else if (IsValid(this.initDoubleValue))
			{
				this.DoubleValue = this.initDoubleValue;
			}
			else
				this.DoubleValue =  this.MinValue;

			base.ignoreTextChange = false;

		}
		#endregion

		#region INTERNAL OPERATIONS

        protected override bool CheckIsZero()
        {
            return (!this.IsNull && this.DoubleValue.Equals(0.0));
        }

		protected override bool IsValidNumberValue(string doubleString)
		{
			bool valid = true;
			double dValue = 0.0;
			try
			{
				dValue = Double.Parse(doubleString,this.NumberFormatInfoObject);
			}
			catch
			{
				valid = false;
			}
			return valid;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int GetNumberPartLength(double numberValue)
		{
			return this.GetNumberPartLength(  numberValue.ToString() );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string GetNumberValue(string formattedText, int startPosition)
		{
			double doubleValue = 0.0;
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

				string sMaxValue = String.Format( this.NumberFormatInfoObject, "{0:n}", double.MaxValue );
				string sMinValue = String.Format( this.NumberFormatInfoObject, "{0:n}", double.MinValue );

				if( !sMaxValue.Equals( formattedText ) && !sMinValue.Equals( formattedText ) )
				{
					doubleValue = Double.Parse(modifiedRawText,NumberStyles.Any,this.NumberFormatInfoObject);
				}
			}
			catch
			{
				// Rollback the operation.
				rollBackOperation = true;
			}

			if(this.IsNegative && doubleValue > 0)
				doubleValue = doubleValue * -1;

			return doubleValue.ToString();
	
		}


		[Syncfusion.Documentation.DocumentationExclude()]
		protected override bool CheckIfNegative(string rawValue)
		{
			if(rawValue != null && rawValue != String.Empty)
			{
					
				double dValue = Convert.ToDouble(rawValue);
				if( dValue >= 0.0 || double.IsNaN( dValue ) ) 
					return false;
				else
					return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="previousFormat"></param>
		protected override void FormatChanged(string currentText,NumberFormatInfo previousFormat)
		{
			double currentValue = 0.0;

			if(this.Initializing == false)
			{
				try
				{
					if(this.GetPreserveData() == true)
						currentValue = this.DoubleValue;
					else
					{
						currentText = currentText.Trim();
						if(currentText != String.Empty)
							currentValue = Double.Parse(currentText,NumberStyles.Float ,previousFormat);
					}
				}
				catch
				{
					currentValue = Convert.ToDouble(this.GetNumberValue(currentText, 0));
				}
				finally
				{
					this.ApplyFormattingAndSetText(currentValue.ToString());
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string ToggleNegative(string currentText)
		{
			double dValue = Convert.ToDouble(currentText);
			dValue = dValue * -1.0;
			if(dValue.Equals(0.0))
				this.SetZeroNegative(!this.GetZeroNegative());
			return dValue.ToString();
		}
		
	
		/// <summary>
		/// Formats the given text according to the current setting.
		/// </summary>
		/// <param name="rawValue"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override string ApplyFormatting(string rawValue) 
		{
			double dValue = 0;

			if (rawValue != null && rawValue != String.Empty)
			{
				dValue = Convert.ToDouble(rawValue);
			}
			else if (this.AllowNull)
			{
				return String.Format(this.NullFormat, "{0}", this.NullString);
			}

			return String.Format(this.NumberFormatInfoObject, "{0:n}", dValue);
		}

		private bool IsValid(double value)
		{
			bool bResult = double.IsNaN(value) || double.IsInfinity(value);
			
			if (!bResult)
			{
				bResult = value >= this.MinValue && value <= this.MaxValue;
			}
			
			return bResult;
		}

		#endregion

		#region DATA
		/// <summary>
		/// Overrides the Text property of <see cref="System.Windows.Forms.TextBox"/>.
		/// </summary>
		/// <remarks>
		/// This property is overriden in order to normalize the data that is set
		/// to the Text property and format it as needed. The method <see cref="Syncfusion.Windows.Forms.NumberTextBoxBase.InsertString"/>
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

        /// <summary>
        /// Indicates whether the NULLString property will be used.
        /// </summary>
        [Browsable(false), Category("Behavior"), Description("Specifies if the NULLString will be used when the value is NULL."), DefaultValue(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("This property will not be used in future.Insead Use AllowNull")
        ]
        public new bool UseNullString
        {
            get
            {
                return base.UseNullString;
            }

            set
            {
                base.UseNullString = value;

                if (!value && m_bIsNullValue)
                {
                    //m_bIsNullValue = false;
                    //this.SetTextBoxText(this.DoubleValue.ToString());
                }
            }
        }

        protected override void SetTextProperty( string newText )
		{
			bool success = true;

            bool bNullText = newText == null || newText == String.Empty;

            if (this.AllowNull && newText == this.NullString)
            {
                this.SetNullNumberValue();
                this.SetTextBoxText(this.NullString);
                return;
            }

            if( this.AllowNull && bNullText )
            {
                success = false;
            }
            else
            {
                try
                {
                    // Try to parse the text using the Decimal.Parse method.
                    double parsedValue = ( bNullText ) ? 
						Double.Parse( DEF_NULL_VALUE, NumberStyles.Number, this.NumberFormatInfoObject ) : 
                        Double.Parse( newText, NumberStyles.Number, this.NumberFormatInfoObject );

                    this.SetDoubleValue( parsedValue );
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
		/// Gets or sets the double value of the control. This will be formatted and
		/// displayed.
		/// </summary>
		[
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Data"),
		Description("The double value of the currency control."),
		RefreshProperties(RefreshProperties.Repaint), 
		]
		public double DoubleValue
		{
			get
			{
				if(this.GetPreserveData() == true)
					return this.preservedDoubleValue;
				else
					return Convert.ToDouble(this.GetNumberValue(base.Text,0));
			}

			set
			{
                if (this.Initializing == false)
                    this.SetDoubleValue(value);
                else
                    this.initDoubleValue = value;
			}
		}

		protected void SetDoubleValue(double newValue)
		{
			this.SelectionStart = 0;
			this.SelectionLength = base.TextLength;

			if (IsValid(newValue))
			{
				IsNegative = newValue < 0;

                newValue = Math.Round(newValue, this.NumberDecimalDigits);
                
				// Set the DoubleValue before TextChanged event is raised.
				this.preservedDoubleValue = newValue;
				this.SetPreserveData(true);
				// Text is set and TextChanged and DoubleValueChanged events are raised.
				this.ApplyFormattingAndSetText(newValue.ToString("r"));
			}
		}

		/// <summary>
		/// Gets or sets the maximum value that can be set through the DoubleTextBox.
		/// </summary>
		[
		Category( "Behavior" ),
		Description( "Gets or sets the maximum value that can be set through the DoubleTextBox." )
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
				this.maxValue = value;

				if (!IsValid(this.DoubleValue))
				{
					this.DoubleValue = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the MaxValue property should be serialized.
		/// </summary>
		/// <returns>True if the value is not equal to <see cref="double.MaxValue"/>.</returns>
		private bool ShouldSerializeMaxValue()
		{
			if(this.maxValue != double.MaxValue)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Resets the Max value to the default.
		/// </summary>
		private void ResetMaxValue()
		{
			this.MaxValue = double.MaxValue;
		}

		

		/// <summary>
		/// Gets or sets the minimum value that can be set through the DoubleTextBox.
		/// </summary>
		[
		Category( "Behavior" ),
		Description( "Gets or sets the minimum value that can be set through the DoubleTextBox." )
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
				this.minValue = value;

				if (!IsValid(this.DoubleValue))
				{
					this.DoubleValue = value;
				}
			}
		}


		/// <summary>
		/// Indicates whether the MinValue property should be serialized.
		/// </summary>
		/// <returns>True if the value is not equal to <see cref="double.MaxValue"/>.</returns>
		private bool ShouldSerializeMinValue()
		{
			if(this.minValue != double.MinValue)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Resets the value to the default.
		/// </summary>
		private void ResetMinValue()
		{
			this.MinValue = double.MinValue;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override bool CheckForMinMax(string currentTextValue, bool ignoreLength)
		{
            if (MinMaxValidation == MinMaxValidation.OnLostFocus)
                return true;
            int textLength = (currentTextValue.IndexOf('-') != -1) ? currentTextValue.Length - 1 : currentTextValue.Length;

            Double doubleVal;
            bool isNum = Double.TryParse(currentTextValue,NumberStyles.Any,this.NumberFormatInfoObject, out doubleVal);

            if (!isNum && this.AllowNull)
                return true;
            if (doubleVal < this.MinValue || doubleVal > this.MaxValue)
                return false;
            else
                return true;
		}

        protected override bool CheckNullStringIsInRange(string nullString)
        {
             Double doubleVal;
             bool isNumber = Double.TryParse(nullString, NumberStyles.Any, this.NumberFormatInfoObject, out doubleVal);
             if (isNumber)
             {
                 return CheckForMinMax(doubleVal.ToString(), true);
             }
             else
                 return true;
        }
		/// <summary>
		/// Raises the <see cref="DoubleTextBox.DoubleValueChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnDoubleValueChanged(EventArgs e)
		{
			if (base.ignoreTextChange != true && DoubleValueChanged != null)
				DoubleValueChanged(this, e);
		}
/*
        /// <summary>
        /// Gets or sets value that indicates whether NullString should be set if UseNullString is true
        /// </summary>
        /// <value>
        /// 	<c>true</c> if NullString should be set; otherwise, <c>false</c>.
        /// </value>
        [
            DefaultValue(false), Category("Behavior"), Description(@"Gets or sets value that indicates whether NullString should be set if UseNullString is true."),
            Obsolete("This property will not be used in future. Use NullState property to get the null state of the text box")
        ]
        public bool IsNullValue
        {
            get
            {
                return m_bIsNullValue;
            }

            set
            {
                if (!this.Initializing)
                {
                    if (this.UseNullString)
                    {
                        m_bIsNullValue = value;
                        if (m_bIsNullValue)
                        {
                            this.SetTextBoxText(this.NullString);
                        }
                        else
                        {
                            this.SetTextBoxText(this.DoubleValue.ToString());
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The value can be changed only if UseNullString is true.");
                    }
                }
                else
                {
                    m_initIsNullValue = value;
                }
            }
        }
        */
        //internal override void OnNullStringChanged()
        //{
        //    if (this.AllowNull && this.IsNull)
        //    {
        //        SetTextBoxText(this.NullString);
        //    }
        //    base.OnNullStringChanged();
        //}

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
			base.OnTextChanged(e);
			this.OnDoubleValueChanged(e);
			this.OnBindableValueChanged(e);

			SetPreserveData( bPreserve );
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
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            /*if (this.DoubleValue == 0 && this.UseNullString && this.AllowNull)
            {
                this.Text = this.NullString;
            }*/

            base.OnKeyDown(e);
        }
		/// <summary>
		/// Overrides the <see cref="System.Windows.Forms.Control.OnEnter"/> method.
		/// </summary>
		/// <param name="args">The event data.</param>
		/// <remarks>
		/// Saves the current DoubleValue so that it can be compared 
		/// during validation. The DoubleValueChanged and TextChanged event
		/// will only be raised if the value is different during validation.
		/// </remarks>
		protected override void OnEnter(EventArgs args)
		{
			this.enterDoubleValue = this.DoubleValue;
			base.OnEnter(args);
		}

		protected override void OnValidating(CancelEventArgs e) 
		{
            newDoubleValue = this.Text;
			double value = this.DoubleValue;

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
                            SetTextBoxText(this.NullString);
                        }
                        else
                        {
                            this.Focus();
                            this.SelectionStart = this.Text.Length;
                        }
                        break;

                    case OnValidationFailed.SetMinOrMax:
                        value = value < this.MinValue ? this.MinValue : value;
                        value = value > this.MaxValue ? this.MaxValue : value;
                        this.DoubleValue = value;
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
            oldDoubleValue = this.Text;
			base.OnValidating(e);
		}

        protected override void OnValidated(EventArgs e)
        {
            base.OnValidated(e);
            this.OnControlValidated(newDoubleValue, oldDoubleValue);
        }

		protected override void SetNullNumberValue()
		{
			this.preservedDoubleValue = 0.0;
			this.SetPreserveData(true);
		}

		protected override bool IsAssignable(object val)
		{
			if(val == null || Convert.IsDBNull(val) == true)
				return false;

			bool assignable = typeof(Double).IsAssignableFrom(val.GetType());
			return assignable;
		}

		protected override void SetValue(object val)
		{
			this.DoubleValue = (Double)val;
		}

		protected override object GetValue()
		{
			return this.DoubleValue;
		}

		#endregion

		#region KEY HANDLING
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override bool HandleBackspaceKey()
        {
            if (this.CheckIsZero() && this.AllowNull)
            {
                this.SetNullNumberValue();
                this.SetTextBoxText(this.NullString);
                return true;
            }
            return base.HandleBackspaceKey();
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override bool HandleDeleteKey()
        {
            if (this.CheckIsZero() && this.AllowNull)
            {
                this.SetNullNumberValue();
                this.SetTextBoxText(this.NullString);
                return true;
            }
            return base.HandleDeleteKey();
        }
        [Syncfusion.Documentation.DocumentationExclude()]
		protected override bool HandleDecimalKey() 
		{
			int cursorPosition = this.SelectionStart;
			string currentText = base.Text;
		
			int decimalSeparatorPosition = this.GetDecimalSeparatorPosition(currentText);
            if (decimalSeparatorPosition != -1)
			{
				decimalSeparatorPosition++;
				this.SetEmptySelection(decimalSeparatorPosition);
			}
            else if (this.IsNull && this.NumberFormatInfoObject.CurrencyDecimalDigits > 0)
            {
                m_bDecimalMode = true;
            }
			else
				return false;

			return true;
		}

		protected override NumberModifyState HandleSubtractKey()
		{
			NumberModifyState ret = base.HandleSubtractKey();
			if( this.DeleteSelectionOnNegative )
			{
				this.SetEmptySelection( this.GetDecimalSeparatorPosition( this.GetTextBoxText() ) );
			}
			return ret;
		}


		#endregion

		#region Overrides

		protected override NumberModifyState PrepareInsertString( string currentText, int startPosition, int selectionLength, string textToBeInserted, bool pasteOperation )
		{
			if( this.DoubleValue == 0.0 )
			{
				currentText = currentText.Substring( 0, startPosition + selectionLength );
			}

            NumberModifyState state = base.PrepareInsertString(currentText, startPosition, selectionLength, textToBeInserted, pasteOperation);
            /*if (state.success)
            {
                if (this.MinMaxValidation!=MinMaxValidation.OnLostFocus && MaxValue < Double.Parse(GetNumberValue(state.changedValue, 0)))
                {
                    state.success = false;
                }
                else
                {
                    state.success = true;
                }
            }*/
            return state;
		}

		#endregion
	}
}   
        
