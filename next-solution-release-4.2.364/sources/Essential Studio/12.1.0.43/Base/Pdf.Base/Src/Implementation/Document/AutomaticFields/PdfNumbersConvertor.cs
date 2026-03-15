#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Utility class for number conversion.
    /// </summary>
    internal class PdfNumbersConvertor
    {
        #region Fields
        /// <summary>
        /// Limit number of converting arabic to \"A\" format.
        /// </summary>
        private const float LetterLimit = 26.0f;

        /// <summary>
        /// Index of A char in the ASCII table.
        /// </summary>
        private const int AcsiiStartIndex = (int)('A' - 1);
        #endregion

        #region Static methods
        /// <summary>
        /// Converts the specified number to numberStyle format.
        /// </summary>
        /// <param name="intArabic">The arabic value.</param>
        /// <param name="numberStyle">The number style.</param>
        /// <returns></returns>
        public static string Convert(int intArabic, PdfNumberStyle numberStyle)
        {
            switch (numberStyle)
            {
                case PdfNumberStyle.None:
                    return String.Empty;

                case PdfNumberStyle.Numeric:
                    return intArabic.ToString();

                case PdfNumberStyle.LowerLatin:
                    return ArabicToLetter(intArabic).ToLower();

                case PdfNumberStyle.LowerRoman:
                    return ArabicToRoman(intArabic).ToLower();

                case PdfNumberStyle.UpperLatin:
                    return ArabicToLetter(intArabic);

                case PdfNumberStyle.UpperRoman:
                    return ArabicToRoman(intArabic);
            }

            return String.Empty;
        }

        /// <summary>
        /// Converts arabic number to roman.
        /// </summary>
        /// <param name="intArabic">Number in arabic format.</param>
        /// <returns>Number in Roman format.</returns>
        private static string ArabicToRoman(int intArabic)
        {
            StringBuilder retval = new StringBuilder();

            retval.Append(GenerateNumber(ref intArabic, 1000, "M"));
            retval.Append(GenerateNumber(ref intArabic, 900, "CM"));
            retval.Append(GenerateNumber(ref intArabic, 500, "D"));
            retval.Append(GenerateNumber(ref intArabic, 400, "CD"));
            retval.Append(GenerateNumber(ref intArabic, 100, "C"));
            retval.Append(GenerateNumber(ref intArabic, 90, "XC"));
            retval.Append(GenerateNumber(ref intArabic, 50, "L"));
            retval.Append(GenerateNumber(ref intArabic, 40, "XL"));
            retval.Append(GenerateNumber(ref intArabic, 10, "X"));
            retval.Append(GenerateNumber(ref intArabic, 9, "IX"));
            retval.Append(GenerateNumber(ref intArabic, 5, "V"));
            retval.Append(GenerateNumber(ref intArabic, 4, "IV"));
            retval.Append(GenerateNumber(ref intArabic, 1, "I"));

            return retval.ToString();
        }

        /// <summary>
        /// Converts arabic number to \"A\" format.
        /// </summary>
        /// <param name="arabic">Number in arabic format.</param>
        /// <returns>Number in \"A\" format.</returns>
        private static string ArabicToLetter(int arabic)
        {
            Stack<int> stack = ConvertToLetter(arabic);
            StringBuilder result = new StringBuilder();

            while (stack.Count > 0)
            {
                int num = stack.Pop();
                AppendChar(result, num);
            }

            return result.ToString();
        }

        /// <summary>
        /// Utility metnod for converting arabic number to roman format.
        /// </summary>
        /// <param name="value">Current number value.</param>
        /// <param name="magnitude">Max current number.</param>
        /// <param name="letter">Roman equivalent.</param>
        /// <returns>Roman equivalent.</returns>
        private static string GenerateNumber(ref int value, int magnitude, string letter)
        {
            StringBuilder numberstring = new StringBuilder();

            while (value >= magnitude)
            {
                value -= magnitude;
                numberstring.Append(letter);
            }

            return numberstring.ToString();
        }

        /// <summary>
        /// Utility metnod. Helps to convert arabic number to \"A\" format.
        /// </summary>
        /// <param name="arabic">Arabic number.</param>
        /// <returns>Sequence of number.</returns>
        private static Stack<int> ConvertToLetter(float arabic)
        {
            if (arabic <= 0)
            {
                throw new ArgumentOutOfRangeException("arabic", "Value can not be less 0");
            }

            Stack<int> stack = new Stack<int>();


            while (((int)arabic) > LetterLimit)
            {
                float remainder = arabic % LetterLimit;

                if (remainder == 0.0f)
                {
                    arabic = arabic / LetterLimit - 1f;
                    remainder = LetterLimit;
                }
                else
                {
                    arabic /= LetterLimit;
                }

                stack.Push((int)remainder);
            }

            if (arabic > 0f)
            {
                stack.Push((int)arabic);
            }

            return stack;
        }

        /// <summary>
        /// Adds letter instead of number.
        /// </summary>
        /// <param name="builder">String builder object.</param>
        /// <param name="number">Number to be converted to letter.</param>
        private static void AppendChar(StringBuilder builder, int number)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            if (number <= 0 || number > 26)
            {
                throw new ArgumentOutOfRangeException("number", "Value can not be less 0 and greater 26");
            }

            char letter = (char)(AcsiiStartIndex + number);
            builder.Append(letter);
        }
        #endregion
    }
}
