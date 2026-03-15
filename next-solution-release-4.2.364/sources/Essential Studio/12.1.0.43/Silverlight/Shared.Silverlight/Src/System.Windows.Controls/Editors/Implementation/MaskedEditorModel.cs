// <copyright file="MaskedEditorModel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Collections.Generic;
using System.Threading;

#if WPF

namespace Syncfusion.Windows.Shared
#endif

#if SILVERLIGHT
namespace Syncfusion.Windows.Tools.Controls
#endif
{
    /// <summary>
    /// MaskedEditorModel provides the model for MaskedTextBox class
    /// </summary>
    public class MaskedEditorModel
    {
        internal MaskedTextBox maskedText = new MaskedTextBox();

        #region Constructor

        /// <summary>
        /// Define digit symbol
        /// </summary>
        private static readonly string defDigitSymbols = "09#";

        /// <summary>
        /// Define other symbol
        /// </summary>
        private static readonly string defOtherSymbols = "Ll?&CcAa";

        /// <summary>
        /// Define separator symbol
        /// </summary>
        private static readonly string defSeparatorSymbols = ".,:/$";

        /// <summary>
        /// Define shift symbol
        /// </summary>
        private static readonly string defShiftSymbols = "<>|";

        #endregion Constructor

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MaskedEditorModel"/> class.
        /// </summary>
        public MaskedEditorModel()
        {
            this.DateSeparator = "/";
            this.DecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            this.TimeSeparator = ":";
            if (maskedText.GroupSeperatorEnabled)
            {
                this.NumberGroupSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
            }
            this.CurrencySymbol = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol;
            this.PromptChar = '_';
            this.Mask = string.Empty;
            this.TextMaskIndexes = new Dictionary<int, int>();
            this.ShiftStatusIndexes = new Dictionary<int, ShiftStatus>();
        }

        #endregion Constructor

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        /// <value>The currency symbol.</value>
        public string CurrencySymbol
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date separator.
        /// </summary>
        /// <value>The date separator.</value>
        public string DateSeparator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the decimal separator.
        /// </summary>
        /// <value>The decimal separator.</value>
        public string DecimalSeparator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mask.
        /// </summary>
        /// <value>The mask value.</value>
        public string Mask
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number group separator.
        /// </summary>
        /// <value>The number group separator.</value>
        public string NumberGroupSeparator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the prompt char.
        /// </summary>
        /// <value>The prompt char.</value>
        public char PromptChar
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text value.</value>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time separator.
        /// </summary>
        /// <value>The time separator.</value>
        public string TimeSeparator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the mask.
        /// </summary>
        /// <value>The index of the mask.</value>
        private int MaskIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the shift status indexes.
        /// </summary>
        /// <value>The shift status indexes.</value>
        private Dictionary<int, ShiftStatus> ShiftStatusIndexes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text mask indexes.
        /// </summary>
        /// <value>The text mask indexes.</value>
        private Dictionary<int, int> TextMaskIndexes
        {
            get;
            set;
        }

        #endregion DP getters & setters

        #region Implementation

        /// <summary>
        /// Gets the masked text.
        /// </summary>
        /// <param name="mask">The mask value.</param>
        /// <param name="text">The text value.</param>
        /// <param name="dateSeparator">The date separator.</param>
        /// <param name="timeSeparator">The time separator.</param>
        /// <param name="decimalSeparator">The decimal separator.</param>
        /// <param name="numberGroupSeparator">The number group separator.</param>
        /// <param name="promptChar">The prompt char.</param>
        /// <param name="currencySymbol">The currency symbol.</param>
        /// <returns>Return the Mask text</returns>
        public static string GetMaskedText(string mask, string text, string dateSeparator, string timeSeparator, string decimalSeparator, string numberGroupSeparator, char promptChar, string currencySymbol)
        {
            if (mask != null && text != null && dateSeparator != null && timeSeparator != null && decimalSeparator != null && numberGroupSeparator != null && currencySymbol != null)
                return new MaskedEditorModel()
                {
                    Mask = mask,
                    Text = text,
                    DateSeparator = dateSeparator,
                    TimeSeparator = timeSeparator,
                    DecimalSeparator = decimalSeparator,
                    NumberGroupSeparator = numberGroupSeparator,
                    PromptChar = promptChar,
                    CurrencySymbol = currencySymbol
                }
            .GetMaskedText();
            else
                return null;
        }

        /// <summary>
        /// Gets the masked text.
        /// </summary>
        /// <returns>Return the mask text</returns>
        public string GetMaskedText()
        {
            this.ApplyNewMask();
            return this.Text;
        }

        /// <summary>
        /// Applies the new mask.
        /// </summary>
        protected internal void ApplyNewMask()
        {
            string text = string.Empty, maskSymbol = string.Empty;
            this.MaskIndex = -1;
            var shiftStatus = ShiftStatus.None;

            // When the dictionaries are not empty, clear them in order to fill in new values.
            if (this.TextMaskIndexes != null && this.TextMaskIndexes.Count > 0)
            {
                this.TextMaskIndexes.Clear();
            }
            if (this.ShiftStatusIndexes != null && this.ShiftStatusIndexes.Count > 0)
            {
                this.ShiftStatusIndexes.Clear();
            }

            while (this.MaskIndex < this.Mask.Length - 1)
            {
                maskSymbol = this.GetNextMaskSymbol(true);
                if (!string.IsNullOrEmpty(maskSymbol))
                {
                    if (this.IsSymbolLiteral(maskSymbol))
                    {
                        if (maskSymbol[0] == '\\' && maskSymbol.Length == 2)
                        {
                            text += maskSymbol[1].ToString();
                        }
                        else
                        {
                            text += maskSymbol;
                        }
                    }
                    else if (this.IsSymbolSeparator(maskSymbol))
                    {
                        text += this.GetSeparatorText(maskSymbol[0]);
                    }
                    else if (this.IsShiftSymbol(maskSymbol))
                    {
                        shiftStatus = this.GetShiftStatus(maskSymbol);
                    }
                    else
                    {
                        if (this.TextMaskIndexes != null)
                            this.TextMaskIndexes.Add(text.Length, this.MaskIndex);
                        if (this.ShiftStatusIndexes != null)
                            this.ShiftStatusIndexes.Add(text.Length, shiftStatus);
                        text += this.PromptChar.ToString();
                    }
                }
            }

            var savedText = this.Text;
            this.MaskIndex = -1;
            this.Text = string.IsNullOrEmpty(this.Mask) ? savedText : text;

            if (!string.IsNullOrEmpty(this.Mask))
            {
                var j = 0;
                for (var i = 0; i < this.Text.Length && j < savedText.Length; i++)
                {
                    if (this.TextMaskIndexes != null && this.TextMaskIndexes.ContainsKey(i))
                    {
                        if (this.IsAcceptableSymbol(this.Mask[TextMaskIndexes[i]].ToString(), savedText[j]))
                        {
                            this.ReplaceTextSymbol(i, savedText[j++].ToString());
                        }
                    }
                    else
                    {
                        if (this.Text[i] == savedText[j])
                        {
                            this.ReplaceTextSymbol(i, savedText[j++].ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets next symbol in the mask.
        /// </summary>
        /// <param name="bMoveCursor">Determines whether the cursor in the mask should be moved forward.</param>
        /// <returns>Return the next mask symbol</returns>
        protected string GetNextMaskSymbol(bool bMoveCursor)
        {
            var res = string.Empty;
            if (this.MaskIndex < this.Mask.Length - 1)
            {
                res = this.Mask[bMoveCursor ? ++this.MaskIndex : this.MaskIndex + 1].ToString();
                if (res == @"\" && this.MaskIndex < this.Mask.Length - 1)
                {
                    res += this.Mask[bMoveCursor ? ++this.MaskIndex : this.MaskIndex + 2];
                }
            }

            return res;
        }

        /// <summary>
        /// Gets text of the separator depending on the entered mask symbol.
        /// </summary>
        /// <param name="symbol">Character that determines kind of the separator.</param>
        /// <returns>Text associated with the appropriate separator.</returns>
        protected string GetSeparatorText(char symbol)
        {
            switch (symbol)
            {
                case '.':
                    return this.DecimalSeparator;
                case ',':
                    return this.NumberGroupSeparator;
                case ':':
                    return this.TimeSeparator;
                case '/':
                    return this.DateSeparator;
                case '$':
                    return this.CurrencySymbol;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets register status of the following letters depending on the entered mask symbol.
        /// </summary>
        /// <param name="maskSymbol">Mask symbol.</param>
        /// <returns><see cref="ShiftStatus"/> value that is defining the register of letter.</returns>
        protected ShiftStatus GetShiftStatus(string maskSymbol)
        {
            switch (maskSymbol)
            {
                case ">":
                    return ShiftStatus.Uppercase;
                case "<":
                    return ShiftStatus.Lowercase;
                default:
                    return ShiftStatus.None;
            }
        }

        /// <summary>
        /// Determines whether [is acceptable symbol] [the specified mask symbol].
        /// </summary>
        /// <param name="maskSymbol">The mask symbol.</param>
        /// <param name="input">The input.</param>
        /// <returns>
        /// <c>true</c> if [is acceptable symbol] [the specified mask symbol]; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsAcceptableSymbol(string maskSymbol, char input)
        {
            switch (maskSymbol)
            {
                case "0":
                    return char.IsDigit(input);

                case "9":
                    return (char.IsDigit(input) || char.IsWhiteSpace(input)) && input != '\t';

                case "#":
                    return (char.IsDigit(input) || char.IsWhiteSpace(input) || input == '+' || input == '-') && input != '\t';

                case "l":
                case "L":
                    return char.IsLetter(input);

                case "?":
                    return (char.IsLetter(input) || char.IsWhiteSpace(input)) && input != '\t';

                case "&":
                case "c":
                case "C":
                    return input != '\t';

                case "a":
                case "A":
                    return char.IsLetterOrDigit(input);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the entered mask symbol denotes user input.
        /// </summary>
        /// <param name="maskSymbol">The mask symbol.</param>
        /// <returns>
        /// True if symbol can be input, otherwise - false.
        /// </returns>
        protected bool IsInputSymbol(string maskSymbol)
        {
            return defDigitSymbols.Contains(maskSymbol) || defOtherSymbols.Contains(maskSymbol);
        }

        /// <summary>
        /// Determines whether the entered mask symbol causes the uppercase, lowercase shift or cancels one.
        /// </summary>
        /// <param name="maskSymbol">The mask symbol.</param>
        /// <returns>
        /// <c>true</c> if [is shift symbol] [the specified mask symbol]; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsShiftSymbol(string maskSymbol)
        {
            return defShiftSymbols.Contains(maskSymbol);
        }

        /// <summary>
        /// Determines whether the entered mask symbol is literal.
        /// </summary>
        /// <param name="maskSymbol">The mask symbol.</param>
        /// <returns>True is symbol is literal, else false.</returns>
        protected bool IsSymbolLiteral(string maskSymbol)
        {
            if (this.IsSymbolSeparator(maskSymbol) ||
                this.IsShiftSymbol(maskSymbol) ||
                this.IsInputSymbol(maskSymbol))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether the entered mask symbol is separator.
        /// </summary>
        /// <param name="maskSymbol">The mask symbol.</param>
        /// <returns>True if symbol is separator.</returns>
        protected bool IsSymbolSeparator(string maskSymbol)
        {
            return defSeparatorSymbols.Contains(maskSymbol);
        }

        /// <summary>
        /// Replaces the symbol located at the entered index with the new one.
        /// </summary>
        /// <param name="index">Index of symbol that should be replaced.</param>
        /// <param name="symbol">New symbol.</param>
        /// <returns>Symbol replaced = true, not replaced = false</returns>
        protected bool ReplaceTextSymbol(int index, string symbol)
        {
            if (index > -1 && index < this.Text.Length)
            {
                if (this.ShiftStatusIndexes.ContainsKey(index))
                {
                    switch (this.ShiftStatusIndexes[index])
                    {
                        case ShiftStatus.Uppercase:
                            symbol = symbol.ToUpper();
                            break;
                        case ShiftStatus.Lowercase:
                            symbol = symbol.ToLower();
                            break;
                    }
                }

                var text = this.Text.Remove(index, 1);
                this.Text = text.Insert(index, symbol);
                return true;
            }

            return false;
        }

        #endregion Implementation
    }
}