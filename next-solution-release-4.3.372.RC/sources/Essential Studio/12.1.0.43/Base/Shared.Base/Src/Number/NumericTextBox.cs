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
	/// Extends the <see cref="NumberTextBoxBase"/> class to handle numeric input
	/// and validation.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The NumericTextBox is not to be used directly.
	/// </para>
	/// </remarks>
	[
	ToolboxItem(false)
	]
	public class NumericTextBox: NumberTextBoxBase
	{
		/// <summary>
		/// For initialization of culture-related values. Needed when SpecialCultureValue is
		/// not the default value. Since the base class will reset the Culture property during
		/// ISupportInitialize.EndInit, we will have to hold these values and set them on the
		/// correct cultureinfo / numberformatinfo.
		/// </summary>
		private int initNumberDecimalDigits = -1;
		private string initNumberDecimalSeparator = null;
		private string initNumberGroupSeparator = null;
		private int[] initNumberGroupSizes = null;
		private int initNumberNegativePattern = -1;

		#region INITIALIZATION
		/// <summary>
        /// Overloaded. Creates an object of type NumericTextBox. 
		/// </summary>
		/// <remarks>
		/// The NumericTextBox object will be initialized with the default values
		/// for the display and data properties. You need to set any specific
		/// values.
		/// </remarks>
		public NumericTextBox()
		{
			InitializeComponent();
		}

		/// <summary>
		///
		/// </summary>
		private void InitializeComponent()
		{
			this.Multiline = false;
		}

		protected void SetNumberFormatInfoInitValues()
		{
			NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
			bool nfiChanged = false;

			if(this.initNumberDecimalDigits != -1)
			{
				this.NumberFormatInfoObject.NumberDecimalDigits = this.initNumberDecimalDigits;
				nfiChanged = true;
			}

			if(this.initNumberDecimalSeparator != null && this.initNumberGroupSeparator != null)
			{
				if(this.initNumberDecimalSeparator != this.initNumberGroupSeparator)
				{
					this.NumberFormatInfoObject.CurrencyDecimalSeparator = this.initNumberDecimalSeparator;
					this.NumberFormatInfoObject.NumberDecimalSeparator = this.initNumberDecimalSeparator;

					this.NumberFormatInfoObject.CurrencyGroupSeparator = this.initNumberGroupSeparator;
					this.NumberFormatInfoObject.NumberGroupSeparator = this.initNumberGroupSeparator;
					nfiChanged = true;
				}
			}
			else
			{
				if(this.initNumberDecimalSeparator != null && this.initNumberDecimalSeparator != this.NumberFormatInfoObject.NumberGroupSeparator)
				{
					this.NumberFormatInfoObject.CurrencyDecimalSeparator = this.initNumberDecimalSeparator;
					this.NumberFormatInfoObject.NumberDecimalSeparator = this.initNumberDecimalSeparator;
					nfiChanged = true;
				}

				if(this.initNumberGroupSeparator != null && this.initNumberGroupSeparator !=  this.NumberFormatInfoObject.NumberDecimalSeparator)
				{
					this.NumberFormatInfoObject.CurrencyGroupSeparator = this.initNumberGroupSeparator;
					this.NumberFormatInfoObject.NumberGroupSeparator = this.initNumberGroupSeparator;
					nfiChanged = true;
				}
			}

			if(this.initNumberGroupSizes != null)
			{
				this.NumberFormatInfoObject.NumberGroupSizes = this.initNumberGroupSizes;
				nfiChanged = true;
			}
			if(this.initNumberNegativePattern != -1)
			{
				this.NumberFormatInfoObject.NumberNegativePattern = this.initNumberNegativePattern;
				nfiChanged = true;
			}

			if(nfiChanged)
				this.FormatChanged(this.GetTextBoxText(), prevFormat);
		}

		#endregion

		#region FORMATTING

		/// <summary>
		/// Gets or sets the maximum number of digits for the decimal portion of the number.
		/// </summary>
		/// <remarks>
		/// If the number requires two decimal points to accommodate the smallest
		/// denomination and this property will have the value 2 in this case. If there
		/// is a need to have a different value based on the locale, it will be
		/// automatically changed if the <see cref="NumberTextBoxBase.UseLocaleDefault"/> property
		/// is True.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Localizable(false),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Gets or sets the maximum number of digits for the decimal portion of the number." )
		]
		public int NumberDecimalDigits
		{
			get
			{
				return this.NumberFormatInfoObject.NumberDecimalDigits;
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.NumberDecimalDigits = value;
					this.FormatChanged(currentText,prevFormat);
				}
				else
				{
					this.initNumberDecimalDigits = value;
					this.NumberFormatInfoObject.NumberDecimalDigits = value;
				}
			}
		}


		/// <summary>
		/// Indicates whether the NumberDecimalDigits should not be serialized if the UseLocaleDefault property is set.
		/// </summary>
		[Description( "Indicates whether the NumberDecimalDigits should not be serialized if the UseLocaleDefault property is set." )]
		private bool ShouldSerializeNumberDecimalDigits()
		{
			return this.Culture.NumberFormat.CurrencyDecimalDigits != this.NumberDecimalDigits;
		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetNumberDecimalDigits()
		{
			this.NumberDecimalDigits = this.Culture.NumberFormat.NumberDecimalDigits;
		}


		/// <summary>
		/// Gets or sets the decimal separator character that will be used for the display.
		/// </summary>
		/// <remarks>
		/// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/>
		/// and can be changed based on your requirements or based on the locale.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Localizable(false),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Gets or sets the decimal separator character that will be used for the display." )
		]
		public string NumberDecimalSeparator
		{
			get
			{
				return this.NumberFormatInfoObject.NumberDecimalSeparator;
			}

			set
			{
				if (value.Equals(string.Empty))
				{
					throw new ArgumentOutOfRangeException("NumberDecimalSeparator", "Decimal separator cannot be the empty string.");
				}
				if(!Char.IsLetterOrDigit(value,0) && value.Length==1)
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.NumberDecimalSeparator = value;
					this.NumberFormatInfoObject.CurrencyDecimalSeparator = value;
					this.FormatChanged(currentText,prevFormat);
				}
				else
				{
					this.initNumberDecimalSeparator = value;
					this.NumberFormatInfoObject.NumberDecimalSeparator = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the NumberDecimalSeparator should not be serialized if the UseLocaleDefault property is set.
		/// </summary>
		[Description( "Indicates whether the NumberDecimalSeparator should not be serialized if the UseLocaleDefault property is set." )]
		protected bool ShouldSerializeNumberDecimalSeparator()
		{
			return this.Culture.NumberFormat.CurrencyDecimalSeparator != this.NumberDecimalSeparator;
		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetNumberDecimalSeparator()
		{
			this.NumberDecimalSeparator = this.Culture.NumberFormat.NumberDecimalSeparator;
		}

		/// <summary>
		/// Gets or sets the separator to be used for grouping digits.
		/// </summary>
		/// <remarks>
		/// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/>
		/// and can be changed based on your requirements or based on the locale.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Localizable(false),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Gets or sets the separator to be used for grouping digits." )
		]
		public string NumberGroupSeparator
		{
			get
			{
				return this.NumberFormatInfoObject.NumberGroupSeparator;
			}

			set
			{
			if ( value.Equals(string.Empty) || !Char.IsLetterOrDigit(value, 0) && value.Length == 1)
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.NumberGroupSeparator = value;
					this.NumberFormatInfoObject.CurrencyGroupSeparator = value;
					this.FormatChanged(currentText, prevFormat);
				}
				else
				{
					this.initNumberGroupSeparator = value;
					this.NumberFormatInfoObject.NumberGroupSeparator = value;
				}
			}
		}


		/// <summary>
		/// Indicates whether the NumberGroupSeparator should not be serialized if
		/// the UseLocaleDefault property is set.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeNumberGroupSeparator()
		{
			if(this.Culture.NumberFormat.NumberGroupSeparator != this.NumberGroupSeparator)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetNumberGroupSeparator()
		{
			this.NumberGroupSeparator = this.Culture.NumberFormat.NumberGroupSeparator;
		}

		/// <summary>
		/// Gets or sets the grouping of NumberDigits in the NumberTextBox.
		/// </summary>
		/// <remarks>
		/// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/>
		/// and can be changed based on your requirements or based on the locale.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Localizable(false),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Gets or sets the grouping of NumberDigits in the NumberTextBox." )
		]
		public int[] NumberGroupSizes
		{
			get
			{
				return this.NumberFormatInfoObject.NumberGroupSizes;
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.NumberGroupSizes = value;
					this.FormatChanged(currentText, prevFormat);
				}
				else
				{
					this.initNumberGroupSizes = value;
					this.NumberFormatInfoObject.NumberGroupSizes = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the NumberGroupPattern should not be serialized if
		/// the UseLocaleDefault property is set.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeNumberGroupSizes()
		{
			if(this.Culture.NumberFormat.NumberGroupSizes.Length == this.NumberGroupSizes.Length)
			{
				for(int index = 0; index < this.Culture.NumberFormat.NumberGroupSizes.Length; index++)
				{
					if(this.Culture.NumberFormat.NumberGroupSizes[index].Equals(this.NumberGroupSizes[index]) == false)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetNumberGroupSizes()
		{
			this.NumberGroupSizes = this.Culture.NumberFormat.NumberGroupSizes;
		}

		/// <summary>
		/// Gets or sets the pattern to use when the value is negative.
		/// </summary>
		/// <remarks>
		/// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/>
		/// and can be changed based on your requirements or based on the locale.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Localizable(false),
		RefreshProperties( RefreshProperties.Repaint ),
		Description( "Gets or sets the pattern to use when the value is negative." )
		]
		public int NumberNegativePattern
		{
			get
			{
				return this.NumberFormatInfoObject.NumberNegativePattern;
			}

			set
			{
				if(this.Initializing == false)
				{
					string currentText = base.GetTextBoxText();
					NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();
					this.NumberFormatInfoObject.NumberNegativePattern = value;
					this.FormatChanged(currentText, prevFormat);

				}
				else
				{
					this.initNumberNegativePattern = value;
					this.NumberFormatInfoObject.NumberNegativePattern = value;
				}
			}
		}


		/// <summary>
		/// Indicates whether the NumberNegativePattern should not be serialized if
		/// the UseLocaleDefault property is set.
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializeNumberNegativePattern()
		{
			if(this.Culture.NumberFormat.NumberNegativePattern != this.NumberNegativePattern)
				return true;
			else
				return false;
		}


		/// <summary>
		/// Resets the value to the culture specific value.
		/// </summary>
		private void ResetNumberNegativePattern()
		{
			this.NumberNegativePattern = this.Culture.NumberFormat.NumberNegativePattern;
		}

		#endregion

		#region INTERNAL OPERATIONS
		protected virtual int GetNumberStartPosition()
		{
			string text = this.FormattedText;
			if( text == null || text.Length == 0 ) return -1;

			for( int i = 0, len = text.Length; i < len; i++ )
			{
				if( !Char.IsNumber( text[ i ] ) ) continue;

				return i;
			}

			return -1;
		}


		protected virtual int GetNumberEndPosition()
		{
			string text = this.FormattedText;
			if( text == null || text.Length == 0 ) return -1;

			for( int i = text.Length - 1; i >= 0; i-- )
			{
				if( !Char.IsNumber( text[ i ] ) && 
					( text[ i ].ToString() != NegativeSign ) ) continue;

				return i;
			}

			return -1;
		}


		private bool m_bDeleteSelectionOnNegative = false;

		/// <summary>
		/// Gets or sets a value indicating whether to delete selection when number is changed to negative.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [delete selection on negative]; otherwise, <c>false</c>.
		/// </value>
		[ DefaultValue( false ) ]
		[Description( "Gets or sets a value indicating whether to delete selection when number is changed to negative." )]
		public bool DeleteSelectionOnNegative
		{
			get
			{
				return m_bDeleteSelectionOnNegative;
			}
			set
			{
				if( value != m_bDeleteSelectionOnNegative )
				{
					m_bDeleteSelectionOnNegative = value;
				}
			}
		}
		protected override NumberModifyState HandleSubtractKey()
		{
			bool wasNegative = IsNegative;
			NumberModifyState state = null;
			if( m_bDeleteSelectionOnNegative )
			{
				if( SelectionLength == 0 )
				{
					state = base.HandleSubtractKey();
				}
				else
				{
					int selLength = SelectionLength;
					int prevTextLength = FormattedText.Length;
					int negativeSignIndex = FormattedText.IndexOf( NegativeSign );

					if( negativeSignIndex >= SelectionStart &&
						negativeSignIndex <= SelectionStart + SelectionLength )
					{						

						base.HandleDeleteKey();

						IsNegative = wasNegative;
						state = new NumberModifyState();
						state.handled = false;						
					}
					else
					{
						base.HandleDeleteKey();
                        
						if( selLength == prevTextLength )
						{
							IsNegative = wasNegative;
							state = new NumberModifyState();
							state.handled = false;
						}
						else
						{
							state = base.HandleSubtractKey();
						}
						this.SetZeroNegative( !this.GetZeroNegative() );
					}
				}
			}
			else
			{
				state = base.HandleSubtractKey();
			}

			return state;
		}

		protected override bool HandleCharacterKey(char charToBeInserted)
		{
			if( this.DeleteSelectionOnNegative && this.GetTextBoxText().Length == this.SelectionLength )
			{
				this.IsNegative = false;
			}
			return base.HandleCharacterKey (charToBeInserted);
		}


		protected override bool HandleBackspaceKey()
		{			
			// change value to positive, if Negative Sign is just deleted through
			// Backspace key press
			if( ( SelectionStart >= 0 && SelectionStart <= FormattedText.Length ) &&
				( ( SelectionLength > 0 && FormattedText.Substring( SelectionStart, SelectionLength ).IndexOf( NegativeSign ) >= 0 ) || 
				( SelectionStart > 0 && SelectionLength == 0 && FormattedText[ SelectionStart - 1 ].ToString() == NegativeSign ) ) )
			{
				int numStartPos = GetNumberStartPosition();
				int numEndPos = GetNumberEndPosition();
				int selEndPos = SelectionStart + SelectionLength;
				int selStart = SelectionStart;
				int length = ( numStartPos >= selStart ) ? ( selEndPos - numStartPos ) :
					( numEndPos - selStart );

				// change sign to positive
				NumberModifyState state = base.HandleSubtractKey();
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
			// else process BackSpace key press event normally
			else
			{
				return base.HandleBackspaceKey();
			}
		}

		protected override bool HandleDeleteKey()
		{
			// change value to positive, if Negative Sign is just deleted through
			// Backspace key press
			if( SelectionStart >= 0 && SelectionStart < FormattedText.Length &&
				( ( FormattedText.Substring( SelectionStart, SelectionLength ).IndexOf( NegativeSign ) >= 0 ) 
				|| FormattedText[ SelectionStart ].ToString() == NegativeSign ) ) 
			{
				int numStartPos = GetNumberStartPosition();
				int numEndPos = GetNumberEndPosition();
				int selEndPos = SelectionStart + SelectionLength;
				int selStart = SelectionStart;
				int length = ( numStartPos >= selStart ) ? ( selEndPos - numStartPos ) :
					( numEndPos - selStart );

				// change sign to positive
				NumberModifyState state = base.HandleSubtractKey();
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

			// else process BackSpace key press event normally
			else
			{
				return base.HandleDeleteKey();
			}
		}

		#endregion
	}
}

