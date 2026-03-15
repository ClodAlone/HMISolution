// <copyright file="ActualWidthSubstractBorderConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class subtracts the width of side borders from the width
    /// of docking window.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ActualWidthSubstractBorderConverter : IMultiValueConverter
    {
        #region Constants
        /// <summary>
        /// Width constant. Is used to find width parameter in xaml file.
        /// </summary>
        private const string C_WIDTHParam = "ContentWidthParameter";
        
        /// <summary>
        /// Height constant. Is used to find height parameter in xaml
        /// file. 
        /// </summary>
        private const string C_HEIGHTParam = "ContentHeightParameter";
        #endregion

        #region Public method
        /// <summary>
        /// Converts to width minus docking window borders.
        /// </summary>
        /// <param name="values">Width to be converted.</param>
        /// <param name="targetType">Type, the width is to be converted
        /// to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used
        /// here.</param>
        /// <returns>Converted width.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;

            if (DependencyProperty.UnsetValue != values[0])
            {
                double desiredSize = (double)values[0];
                double renderSize = (double)values[1];
                double splitterSize = (double)values[2];
                Dock panelSide = (Dock)values[3];
                string paramTostring = parameter.ToString();
                Thickness borderThickness = (Thickness)values[4];

                switch (paramTostring)
                {
                    case C_WIDTHParam:

                        if (Dock.Left == panelSide || Dock.Right == panelSide)
                        {
                            result = desiredSize - splitterSize - borderThickness.Left - borderThickness.Right;
                        }
                        else
                        {
                            result = renderSize - borderThickness.Top - borderThickness.Bottom;
                        }

                        break;
                    case C_HEIGHTParam:

                        if (Dock.Top == panelSide || Dock.Bottom == panelSide)
                        {
                            result = desiredSize - splitterSize - borderThickness.Top - borderThickness.Bottom;
                        }
                        else
                        {
                            result = renderSize - borderThickness.Left - borderThickness.Right;
                        }

                        break;
                    default:
                        throw new ArgumentException("Incorrect parameter set in Binding");
                }
            }

            return result;
        }

        /// <summary>
        /// This method does nothing.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
