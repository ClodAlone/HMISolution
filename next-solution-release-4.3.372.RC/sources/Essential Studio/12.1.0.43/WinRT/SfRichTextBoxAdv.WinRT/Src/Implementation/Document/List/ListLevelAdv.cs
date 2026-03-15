#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WPF
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class ListLevelAdv : BaseNode
    {
        #region Constants
        internal const string DOTBULLET = "\uf0b7";
        internal const string SQUAREBULLET = "\uf0a7";//Symbol font \u25aa.
        internal const string ARROWBULLET = "\u27a4";
        #endregion

        #region Properties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        internal DocumentAdv OwnerDocument
        {
            get
            {
                if (OwnerAbstractList != null)
                    return OwnerAbstractList.OwnerDocument;
                return null;
            }
        }
        /// <summary>
        /// Gets the owner list.
        /// </summary>
        /// <value>
        /// The owner list.
        /// </value>
        internal AbstractListAdv OwnerAbstractList
        {
            get
            {
                return OwnerBase as AbstractListAdv;
            }
        }
        /// <summary>
        /// Gets or sets the paragraph format.
        /// </summary>
        /// <value>
        /// The paragraph format.
        /// </value>
        internal ParagraphFormat ParagraphFormat
        {
            get
            {
                return (ParagraphFormat)GetValue(ParagraphFormatProperty);
            }
            set
            {
                SetValue(ParagraphFormatProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the character format.
        /// </summary>
        /// <value>
        /// The character format.
        /// </value>
        internal CharacterFormat CharacterFormat
        {
            get
            {
                return (CharacterFormat)GetValue(CharacterFormatProperty);
            }
            set
            {
                SetValue(CharacterFormatProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the list level pattern.
        /// </summary>
        /// <value>
        /// The list level pattern.
        /// </value>
        internal ListLevelPattern ListLevelPattern
        {
            get
            {
                return (ListLevelPattern)GetValue(ListLevelPatternProperty);
            }
            set
            {
                SetValue(ListLevelPatternProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the bullet character.
        /// </summary>
        /// <value>
        /// The bullet character.
        /// </value>
        internal string BulletCharacter
        {
            get
            {
                return (string)GetValue(BulletCharacterProperty);
            }
            set
            {
                SetValue(BulletCharacterProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the start at.
        /// </summary>
        /// <value>
        /// The start at.
        /// </value>
        internal int StartAt
        {
            get
            {
                return (int)GetValue(StartAtProperty);
            }
            set
            {
                SetValue(StartAtProperty, value);
            }
        }
        #endregion

        #region Static Dependency Properties
        internal static readonly DependencyProperty ListLevelPatternProperty = DependencyProperty.Register("ListLevelPattern", typeof(ListLevelPattern), typeof(ListLevelAdv), new PropertyMetadata(ListLevelPattern.Arabic));
        internal static readonly DependencyProperty StartAtProperty = DependencyProperty.Register("StartAt", typeof(int), typeof(ListLevelAdv), new PropertyMetadata(0));
        internal static readonly DependencyProperty BulletCharacterProperty = DependencyProperty.Register("BulletCharacter", typeof(string), typeof(ListLevelAdv), new PropertyMetadata(""));
        /// <summary>
        /// Identifies the ParagraphFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the ParagraphFormat dependency property.</returns>
        internal static readonly DependencyProperty ParagraphFormatProperty = DependencyProperty.Register("ParagraphFormat", typeof(ParagraphFormat), typeof(ListLevelAdv), new PropertyMetadata(null, OnParagraphFormatChanged));
        /// <summary>
        /// Identifies the CharacterFormat dependency property.
        /// </summary>
        /// <returns>The identifier of the CharacterFormat dependency property.</returns>
        internal static readonly DependencyProperty CharacterFormatProperty = DependencyProperty.Register("CharacterFormat", typeof(CharacterFormat), typeof(ListLevelAdv), new PropertyMetadata(null, OnCharacterFormatChanged));
        #endregion

        #region Static Events
        /// <summary>
        /// Called when paragraph format changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnParagraphFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as ParagraphFormat).SetOwner(d as ListLevelAdv);
        }
        /// <summary>
        /// Called when character format changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnCharacterFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (e.NewValue as CharacterFormat).SetOwner(d as ListLevelAdv);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListLevelAdv" /> class.
        /// </summary>
        /// <param name="abstractListAdv">The abstract list adv.</param>
        internal ListLevelAdv(AbstractListAdv abstractListAdv)
            : base(abstractListAdv)
        {
            CharacterFormat = new CharacterFormat(this);
            ParagraphFormat = new ParagraphFormat(this);
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            SetOwner(null);
            CharacterFormat.Dispose();
            ClearValue(CharacterFormatProperty);
            ParagraphFormat.Dispose();
            ClearValue(ParagraphFormatProperty);
            ClearValue(BulletCharacterProperty);
            ClearValue(StartAtProperty);
        }
        /// <summary>
        /// Gets the list text.
        /// </summary>
        /// <param name="listValue">The list value.</param>
        /// <returns></returns>
        internal string GetListText(int listValue)
        {
            switch (ListLevelPattern)
            {
                case ListLevelPattern.UpRoman:
                    return GetAsRoman(listValue).ToUpper();
                case ListLevelPattern.LowRoman:
                    return GetAsRoman(listValue).ToLower();
                case ListLevelPattern.UpLetter:
                    return GetAsLetter(listValue).ToUpper();
                case ListLevelPattern.LowLetter:
                    return GetAsLetter(listValue).ToLower();
                case ListLevelPattern.Arabic:
                    return (listValue).ToString();
                case ListLevelPattern.None:
                    return "";
                default:
                    return (listValue).ToString();
            }
        }

        #region Roman number
        /// <summary>
        /// Gets as roman.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        private string GetAsRoman(int number)
        {
            StringBuilder retval = new StringBuilder();
            retval.Append(GenerateNumber(ref number, 1000, "M"));
            retval.Append(GenerateNumber(ref number, 900, "CM"));
            retval.Append(GenerateNumber(ref number, 500, "D"));
            retval.Append(GenerateNumber(ref number, 400, "CD"));
            retval.Append(GenerateNumber(ref number, 100, "C"));
            retval.Append(GenerateNumber(ref number, 90, "XC"));
            retval.Append(GenerateNumber(ref number, 50, "L"));
            retval.Append(GenerateNumber(ref number, 40, "XL"));
            retval.Append(GenerateNumber(ref number, 10, "X"));
            retval.Append(GenerateNumber(ref number, 9, "IX"));
            retval.Append(GenerateNumber(ref number, 5, "V"));
            retval.Append(GenerateNumber(ref number, 4, "IV"));
            retval.Append(GenerateNumber(ref number, 1, "I"));
            return retval.ToString();
        }
        /// <summary>
        /// Generates the number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="magnitude">The magnitude.</param>
        /// <param name="letter">The letter.</param>
        /// <returns></returns>
        private string GenerateNumber(ref int value, int magnitude, string letter)
        {
            StringBuilder numberstring = new StringBuilder();
            while (value >= magnitude)
            {
                value -= magnitude;
                numberstring.Append(letter);
            }
            return numberstring.ToString();
        }
        #endregion

        #region Letter
        /// <summary>
        /// Gets as letter.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        private string GetAsLetter(int number)
        {
            Stack<int> stack = ConvertToLetter(number);
            StringBuilder result = new StringBuilder();

            while (stack.Count > 0)
            {
                int num = stack.Pop();
                AppendChar(result, num);
            }

            return result.ToString();
        }
        /// <summary>
        /// Converts to letter.
        /// </summary>
        /// <param name="arabic">The arabic.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">arabic;Value can not be less 0</exception>
        private static Stack<int> ConvertToLetter(float arabic)
        {
            if (arabic < 0)
                throw new ArgumentOutOfRangeException("arabic", "Value can not be less 0");
            Stack<int> stack = new Stack<int>();
            while (((int)arabic) > 26)
            {
                float remainder = arabic % 26;
                if (remainder == 0.0f)
                {
                    arabic = arabic / 26 - 1f;
                    remainder = 26;
                }
                else
                    arabic /= 26;
                stack.Push((int)remainder);
            }
            if (arabic > 0f)
                stack.Push((int)arabic);
            return stack;
        }
        /// <summary>
        /// Appends the char.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="number">The number.</param>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">number;Value can not be less 0 and greater 26</exception>
        private void AppendChar(StringBuilder builder, int number)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (number <= 0 || number > 26)
                throw new ArgumentOutOfRangeException("number", "Value can not be less 0 and greater 26");
            int aASCIIIndex = (int)('A' - 1);
            char letter = (char)(aASCIIIndex + number);
            builder.Append(letter);
        }
        #endregion
        #endregion
    }
}
