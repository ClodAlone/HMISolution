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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Syncfusion.Calculate;

namespace Syncfusion.Windows.Calculate
{
    /// <summary>
    /// Use this IValueConverter instance to convert a value using a formula or expression in XAML.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that the formula variable name is set to "a" by default and you can change it through
    /// the <see cref="FormulaVariableName"/> property. You can then refer to the binding source value in the formula as "[a]".
    /// </para>
    /// <example>
    /// Here is a XAML sample:
    ///     <TextBox Text="{Binding ElementName=textbox1, Path=Text, Mode=OneWay,
    ///         Converter={StaticResource sveConverter}, ConverterParameter=[a] ^ 2}">
    /// </example>
    /// </remarks>
    public class SingleVariableExpressionConverter : IValueConverter
    {
        private string formulaVariableName = "a";

        public string FormulaVariableName { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CalcQuickBase calculator = new CalcQuickBase();
            calculator[formulaVariableName] = value.ToString();

            string formula = parameter.ToString();
            return calculator.ParseAndCompute(formula);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Use this IMultiValueConverter instance to convert a set of values into a result using a formula.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that the first value should be referred to as "[a]" in the formula, the 2nd value as "[b]" and so on. 
    /// </para>
    /// <example>
    /// Here is a XAML sample:
    ///    <StackPanel Orientation="Vertical">
    ///        <TextBox x:Name="tb2" Text="10"></TextBox>
    ///        <TextBox x:Name="tb3" Text="10"></TextBox>
    ///        <TextBox x:Name="tb4" Text="10"></TextBox>
    ///        <TextBox x:Name="tb5" Text="10"></TextBox>
    ///    </StackPanel>
    ///    
    ///    <TextBox>
    ///        <TextBox.Text>
    ///            <MultiBinding Converter="{StaticResource multiVarExpConverter}">
    ///                <MultiBinding.ConverterParameter>[a]+[b]+[c]+[d]</MultiBinding.ConverterParameter>
    ///                
    ///                <Binding ElementName="tb2" Path="Text"></Binding>
    ///                <Binding ElementName="tb3" Path="Text"></Binding>
    ///                <Binding ElementName="tb4" Path="Text"></Binding>
    ///                <Binding ElementName="tb5" Path="Text"></Binding>
    ///                
    ///            </MultiBinding>
    ///            
    ///        </TextBox.Text>
    ///    </TextBox>
    /// </example>
    /// </remarks>
    public class MultiVariableExpressionConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CalcQuickBase calculator = new CalcQuickBase();

            int i = -1;
            foreach (object value in values)
            {
                i++;
                calculator[IntToAlphabet(i)] = value.ToString();
            }

            string formula = parameter.ToString();
            return calculator.ParseAndCompute(formula);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        private string IntToAlphabet(int i)
        {
            switch (i)
            {
                case 0: return "A";
                case 1: return "B";
                case 2: return "C";
                case 3: return "D";
                case 4: return "E";
                case 5: return "F";
                case 6: return "G";
                case 7: return "H";
                case 8: return "I";
                case 9: return "J";
                case 10: return "K";
                case 11: return "L";
                case 12: return "M";
                case 13: return "N";
                case 14: return "O";
                case 15: return "P";
                case 16: return "Q";
                case 17: return "R";
                case 18: return "S";
                case 19: return "T";
                case 20: return "U";
                case 21: return "V";
                case 22: return "W";
                case 23: return "X";
                case 24: return "Y";
                case 25: return "Z";
                default:
                    throw new Exception("More than 26 parameters passed in for MultiVariableExpressionConverter. Not supported.");
            }
        }
    }

}
