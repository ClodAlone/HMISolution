#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Tools.Controls
{
    internal class MaskEditModel
    {
        public static MaskEditModel maskEditorModelHelp = new MaskEditModel();

        /// <summary>
        /// Creates the regular expression.
        /// </summary>
        /// <param name="MaskedTextBoxAdv">The masked text box.</param>
        /// <returns></returns>
        public ObservableCollection<CharacterProperties> CreateRegularExpression(MaskedTextBoxAdv MaskedTextBoxAdv)
        {
            if (MaskedTextBoxAdv.MaskType == MaskType.Standard)
            {
                #region Standard
                bool mEscapeSequence = false;
                bool? mIsUpper = null;
                var charCollection = new ObservableCollection<CharacterProperties>();
                string mask = MaskedTextBoxAdv.Mask;
                //CultureInfo culture = MaskedTextBoxAdv.Culture ?? CultureInfo.CurrentCulture;
                CultureInfo culture = MaskedTextBoxAdv.GetCulture();
                foreach (var ch in mask.ToString())
                {
                    CharacterProperties p;
                    switch (ch)
                    {
                        case '0':
                            p = new CharacterProperties
                            {
                                RegExpression = mEscapeSequence ? ch.ToString() : @"\d",
                                IsLiteral = mEscapeSequence,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };

                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;
                        case '9':
                            p = new CharacterProperties
                            {
                                //RegExpression = mEscapeSequence ? ch.ToString() : @"[\d]?",
                                RegExpression = mEscapeSequence ? ch.ToString() : @"[\s\d]",
                                IsLiteral = mEscapeSequence,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };

                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;
                        case '#':
                            p = new CharacterProperties
                            {
                                //RegExpression = mEscapeSequence ? ch.ToString() : @"[\d+-]?",
                                RegExpression = mEscapeSequence ? ch.ToString() : @"[\s\d+-]",
                                IsLiteral = mEscapeSequence,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;
                        case 'L':
                            p = new CharacterProperties
                            {
                                RegExpression = mEscapeSequence ? ch.ToString() : @"[a-zA-Z]",
                                IsLiteral = mEscapeSequence,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;

                        case '?':
                            p = new CharacterProperties
                            {
                                //RegExpression = mEscapeSequence ? ch.ToString() : @"[a-zA-Z]?",
                                RegExpression = mEscapeSequence ? ch.ToString() : @"[\sa-zA-Z]",
                                IsLiteral = mEscapeSequence,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;

                        case '&':
                            p = new CharacterProperties
                            {
                                RegExpression =
                                    mEscapeSequence ? ch.ToString() : @"[\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}]",
                                IsLiteral = mEscapeSequence,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;

                        case 'C':
                            p = new CharacterProperties
                            {
                                RegExpression =
                                    mEscapeSequence ? ch.ToString() : @"[\s\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}]?",
                                //mEscapeSequence ? ch.ToString() : @"[\p{Ll}\p{Lu}\p{Lt}\p{Lm}\p{Lo}]?",
                                IsLiteral = mEscapeSequence,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;

                        case 'A':
                            p = new CharacterProperties
                            {
                                RegExpression =
                                    mEscapeSequence ? ch.ToString() : @"\w",
                                IsLiteral = mEscapeSequence,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;

                        case ',':
                            p = new CharacterProperties
                            {
                                RegExpression =
                                    mEscapeSequence
                                        ? ch.ToString()
                                        : culture.NumberFormat.NumberGroupSeparator,
                                IsLiteral = true,
                                // mEscapeSequence,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;

                        case '.':
                            p = new CharacterProperties
                            {
                                RegExpression =
                                    mEscapeSequence
                                        ? ch.ToString()
                                        : culture.NumberFormat.NumberDecimalSeparator,
                                IsLiteral = true,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;

                        case ':':
                            p = new CharacterProperties
                            {
                                RegExpression =
                                    mEscapeSequence ? ch.ToString() : ":",
                                IsLiteral = true,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;
                        case '/':
                            p = new CharacterProperties
                            {
                                RegExpression =
                                    mEscapeSequence ? ch.ToString() : "/",
                                IsLiteral = true,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;
                        case '$':
                            p = new CharacterProperties
                            {
                                RegExpression =
                                    mEscapeSequence
                                        ? ch.ToString()
                                        : culture.NumberFormat.CurrencySymbol,
                                IsLiteral = true,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;
                        case '\\':
                            switch (mEscapeSequence)
                            {
                                case false:
                                    mEscapeSequence = true;
                                    break;
                                default:
                                    p = new CharacterProperties
                                    {
                                        RegExpression =
                                            ch.ToString(),
                                        IsLiteral = true,
                                        IsUpper = mIsUpper,
                                        IsPromptCharacter = null
                                    };
                                    charCollection.Add(p);
                                    mEscapeSequence = false;
                                    break;
                            }
                            break;
                        case '>':
                            mIsUpper = true;
                            break;
                        case '<':
                            mIsUpper = false;
                            break;
                        default:
                            p = new CharacterProperties
                            {
                                RegExpression =
                                    ch.ToString(),
                                IsLiteral = true,
                                IsUpper = mIsUpper,
                                IsPromptCharacter = null
                            };
                            charCollection.Add(p);
                            mEscapeSequence = false;
                            break;
                    }
                }
                return charCollection;
                #endregion
            }
            return null;
        }

        /// <summary>
        /// Creates the display text.
        /// </summary>
        /// <param name="MaskedTextBoxAdv">The masked text box.</param>
        /// <returns></returns>
        public string CreateDisplayText(MaskedTextBoxAdv MaskedTextBoxAdv)
        {
            if (MaskedTextBoxAdv.MaskType == MaskType.Standard)
            {
                #region Standard
                int valuecount = 0;
                string displayText = string.Empty;
                for (int i = 0; i < MaskedTextBoxAdv.CharCollection.Count; i++)
                {
                    var charProperty = MaskedTextBoxAdv.CharCollection[i];
                    if (charProperty.IsLiteral)
                    {
                        displayText += charProperty.IsUpper == null
                                           ? charProperty.RegExpression.ToString()
                                           : (charProperty.IsUpper == true
                                                  ? charProperty.RegExpression.ToUpper()
                                                  : charProperty.RegExpression.ToLower());
                        charProperty.IsPromptCharacter = null;
                    }
                    else
                    {
                        char valuechar;
                        if (valuecount < MaskedTextBoxAdv.mValue.ToString().Length)
                        {
                            valuechar = MaskedTextBoxAdv.Value.ToString()[valuecount];
                            string pattern = charProperty.RegExpression;
                            bool match = Regex.IsMatch(valuechar.ToString(), pattern);
                            if (match)
                            {
                                displayText += charProperty.IsUpper == null
                                                   ? valuechar.ToString()
                                                   : (charProperty.IsUpper == true
                                                          ? Char.ToUpper(valuechar).ToString()
                                                          : Char.ToLower(valuechar).ToString());
                                valuecount++;
                                charProperty.IsPromptCharacter = false;
                            }
                            else
                            {
                                displayText += MaskedTextBoxAdv.PromptChar;
                                for (int j = i + 1; j < MaskedTextBoxAdv.CharCollection.Count; j++)
                                {
                                    charProperty = MaskedTextBoxAdv.CharCollection[i];
                                    if (charProperty.IsLiteral)
                                    {
                                        displayText += charProperty.IsUpper == null
                                                           ? charProperty.RegExpression.ToString()
                                                           : (charProperty.IsUpper == true
                                                                  ? charProperty.RegExpression.ToUpper()
                                                                  : charProperty.RegExpression.ToLower());
                                        charProperty.IsPromptCharacter = null;
                                    }
                                    else
                                    {
                                        displayText += charProperty.IsLiteral
                                                           ? charProperty.RegExpression
                                                           : MaskedTextBoxAdv.PromptChar.ToString();
                                    }
                                }
                                return displayText;
                            }
                        }
                        else
                        {
                            displayText += MaskedTextBoxAdv.PromptChar;
                            charProperty.IsPromptCharacter = true;
                        }
                    }
                }
                return displayText;
                #endregion
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Double)
            {
                return null;
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Integer)
            {
                return null;
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Currency)
            {
                return null;
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Percentage)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates the value from text.
        /// </summary>
        /// <param name="MaskedTextBoxAdv">The masked text box.</param>
        /// <returns></returns>
        public object CreateValueFromText(MaskedTextBoxAdv MaskedTextBoxAdv)
        {
            if (MaskedTextBoxAdv.MaskType == MaskType.Standard)
            {
                string valueText = string.Empty;
                for (int i = 0; i < MaskedTextBoxAdv.CharCollection.Count; i++)
                {
                    var charProperty = MaskedTextBoxAdv.CharCollection[i];
                    if (charProperty.IsLiteral)
                        valueText += MaskedTextBoxAdv.Text[i];
                    else
                    {
                        if (charProperty.IsPromptCharacter == false)
                            valueText += MaskedTextBoxAdv.Text[i];
                    }
                }
                return valueText;
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Double)
            {
                double preValue;
                if (double.TryParse(MaskedTextBoxAdv.MaskedText, NumberStyles.Number, MaskedTextBoxAdv.GetCulture().NumberFormat, out preValue))
                {
                    return preValue;
                }
                else
                {
                    return 0.0;
                }
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Integer)
            {
                Int64 preValue;
                if (Int64.TryParse(MaskedTextBoxAdv.MaskedText, NumberStyles.Number, MaskedTextBoxAdv.GetCulture().NumberFormat, out preValue))
                {
                    return preValue;
                }
                else
                {
                    return 0;
                }
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Currency)
            {
                double preValue;
                if (double.TryParse(MaskedTextBoxAdv.MaskedText, NumberStyles.Currency, MaskedTextBoxAdv.GetCulture().NumberFormat, out preValue))
                {
                    return preValue;
                }
                else
                {
                    return 0.0;
                }
            }
            else if (MaskedTextBoxAdv.MaskType == MaskType.Percentage)
            {
                double preValue;
                int loc = MaskedTextBoxAdv.MaskedText.IndexOf(MaskedTextBoxAdv.GetCulture().NumberFormat.PercentSymbol[0]);
                string text = MaskedTextBoxAdv.MaskedText.Remove(loc, 1);
                //if (double.TryParse(MaskedTextBoxAdv.MaskedText, NumberStyles.Any,  MaskedTextBoxAdv.GetCulture().NumberFormat, out preValue))
                if (double.TryParse(text, NumberStyles.Any, MaskedTextBoxAdv.GetCulture().NumberFormat, out preValue))
                {
                    return preValue;
                }
                else
                {
                    return 0.0;
                }
            }
            else
            {
                return null;
            }

        }

    }
}
