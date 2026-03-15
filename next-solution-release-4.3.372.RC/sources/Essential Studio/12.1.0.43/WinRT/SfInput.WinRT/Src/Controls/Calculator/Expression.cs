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
using System.Linq.Expressions;
using System.Text;

#if WINDOWS_PHONE || WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#elif WPF

namespace Syncfusion.Windows.Controls.Input
#elif SILVERLIGHT

namespace Syncfusion.Tools.Controls.Input
#else
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    //http://www.lawrence.edu/fast/greggj/CMSC150/Infix/Expressions.html
    /// <summary>
    /// Represents a class for defining the expression list
    /// </summary>
    public class ExpressionList : List<object>
    {
        /// <summary>
        /// When the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/> control is Ready to
        /// get another input and has finished processing the previous input.
        /// </summary>
        public bool ReadyForOperand = false;

        /// <summary>
        /// Gets or sets the calculated value by the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/> control.
        /// </summary>
        public decimal Value = 0;

        /// <summary>
        /// Converts the object into a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var str in this)
            {
                builder.Append(str.ToString() + " ");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Evaluates the operators.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="calcoperator"></param>
        /// <returns></returns>
        public decimal Evaluate(decimal value, string calcoperator)
        {
            var operators = from op in this
                           where op is string
                           select op;

            string _operator;

            if (operators.Any())
            {
                _operator = operators.Last().ToString();
            }
            else
            {
                Value = value;
                return Value;
            }

            if (IsSpecialFunction(_operator))
            {
                _operator = calcoperator;
            }
            

            if (_operator.Equals(CalcConstatnts.CAdd))
            {
                Value += value;
            }
            else if (_operator.Equals(CalcConstatnts.CMinus))
            {
                Value -= value;
            }
            else if (_operator.Equals(CalcConstatnts.CMultiply))
            {
                Value *= value;
            }
            else
            {
                Value /= value;
            }

            return Value;
        }

        /// <summary>
        /// Returns true if the function is squae root, otherwise false.
        /// </summary>
        /// <param name="_operator"></param>
        /// <returns></returns>
        public static bool IsSpecialFunction(string _operator)
        {
            return _operator.Contains(CalcConstatnts.CSquareRoot.Substring(0, 4));
        }
    }
}
