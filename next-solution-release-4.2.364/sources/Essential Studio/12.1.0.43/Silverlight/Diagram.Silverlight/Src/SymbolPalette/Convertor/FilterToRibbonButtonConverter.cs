// <copyright file="FilterToRibbonButtonConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections;

namespace Syncfusion.Windows.Diagram
{
   /// <summary>
    /// Represents GalleryFilter converter.
    /// </summary>
    /// <exclude/>
    public class PalleteFilterConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value from a PaletteFilter to FilterRibbonButton.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ICollection collection = (ICollection)value;

            if (collection != null && collection.Count > 0)
            {
                ObservableCollection<FilterRibbonButton> resultCollection = new ObservableCollection<FilterRibbonButton>();

                foreach (SymbolPaletteFilter filter in collection)
                {
                    FilterRibbonButton button = new FilterRibbonButton();
                    button.Style = parameter as Style;
                    button.Label = filter.Label;
                    button.Content = filter;                    
                    button.ClickMode = ClickMode.Release;
                    resultCollection.Add(button);
                }
                resultCollection[0].IsSelected = true;
                return resultCollection;
            }

            return null;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    /// <summary>
    /// Represents Filter to RibbonButton converter.
    /// </summary>
    /// <exclude/>
    public class FilterToRibbonButtonConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts a value from a Filter to RibbonButton.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value.GetType() != typeof(SymbolPaletteFilter))
            {
                throw new ArgumentException("Value of SymbolGalleryFilter type is expected.", "value");
            }

            FilterRibbonButton button = new FilterRibbonButton();
            button.Style = parameter as Style;
            button.Label = (value as SymbolPaletteFilter).Label;
            button.Content = (value as SymbolPaletteFilter);
            button.ClickMode = ClickMode.Release;
            return button;
        }

        /// <summary>
        /// Converts a value from  FilterRibbonButton to a Filter.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            FilterRibbonButton button = value as FilterRibbonButton;

            if (button == null)
            {
                throw new ArgumentException("Value of FilterRibbonButton type is expected.", "value");
            }

            SymbolPaletteFilter filter = button.Content as SymbolPaletteFilter;

            return filter;
        }

        #endregion
    }
}

