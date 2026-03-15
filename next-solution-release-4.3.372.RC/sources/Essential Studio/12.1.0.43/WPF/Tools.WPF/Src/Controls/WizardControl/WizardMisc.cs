// <copyright file="WizardMisc.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Argument for the <see cref="WizardControl.SelectedPageChanging"/> event.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class WizardPageSelectionChangeEventArgs : CancelRoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardPageSelectionChangeEventArgs"/> class.
        /// </summary>
        /// <param name="newPage">The new page.</param>
        /// <param name="oldPage">The old page.</param>
        /// <param name="cause">The cause.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The source.</param>
        public WizardPageSelectionChangeEventArgs(WizardPage newPage, WizardPage oldPage, WizardPageSelectionChangeCause cause, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            this.NewPage = newPage;
            this.OldPage = oldPage;
            this.Cause = cause;
        }

        /// <summary>
        /// Gets or sets the new page.
        /// </summary>
        /// <value>The new page.</value>
        public WizardPage NewPage 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the old page.
        /// </summary>
        /// <value>The old page.</value>
        public WizardPage OldPage 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the cause.
        /// </summary>
        /// <value>The cause.</value>
        public WizardPageSelectionChangeCause Cause 
        { 
            get; 
            set; 
        }
    }

    /// <summary>
    /// Represents WizardSelectPageCommandParameter class
    /// </summary>
    internal class WizardSelectPageCommandParameter
    {
        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>The page WizardPage.</value>
        public WizardPage Page 
        { 
            get; 
            set; 
        }
        
        /// <summary>
        /// Gets or sets the cause.
        /// </summary>
        /// <value>The cause.</value>
        public WizardPageSelectionChangeCause Cause 
        { 
            get; 
            set; 
        }
    }

    /// <summary>
    /// Enumerates the different causes for the change in current page selection of a <see cref="WizardControl"/>.
    /// </summary>
    public enum WizardPageSelectionChangeCause
    {
        /// <summary>
        /// Selection changed because of a <see cref="WizardControl.SelectedWizardPage"/> setting.
        /// </summary>
        Programmatic = 1,
        
        /// <summary>
        /// Selection changed because the user hit the Next button.
        /// </summary>
        NextPageCommand = 2,
        
        /// <summary>
        /// Selection changed because the user hit the Previous button.
        /// </summary>
        PreviousPageCommand = 4,
        
        /// <summary>
        /// Selection changed because someone invoked the WizardCommands.Select page command.
        /// </summary>
        SelectPageCommand = 8
    }
    
    /// <summary>
    /// A cancellable RoutedEventArgs
    /// </summary>
    public class CancelRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRoutedEventArgs"/> class.
        /// </summary>
        public CancelRoutedEventArgs() : this(false, null, null) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public CancelRoutedEventArgs(bool cancel) : this(cancel, null, null) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public CancelRoutedEventArgs(RoutedEvent routedEvent) : this(false, routedEvent, null) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        /// <param name="routedEvent">The routed event.</param>
        public CancelRoutedEventArgs(bool cancel, RoutedEvent routedEvent) : this(cancel, routedEvent, null) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public CancelRoutedEventArgs(RoutedEvent routedEvent, object source) : this(false, routedEvent, null) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The source.</param>
        public CancelRoutedEventArgs(bool cancel, RoutedEvent routedEvent, object source) : base(routedEvent, source) 
        { 
            this.Cancel = cancel; 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancelRoutedEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel 
        { 
            get; 
            set; 
        }
    }

    /// <summary>
    /// Represents WizardPageVisibility class
    /// </summary>
    public class WizardPageVisibility : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isSelected = (bool)value;

            if (isSelected)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary> 
    /// Represents VisibilityResolver class
    /// </summary>
    internal class VisibilityResolver : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visible;
            bool.TryParse(values[0].ToString(), out visible);
            bool? visibleN = (bool?)values[1];

            bool isvisible = false;
            if (visibleN != null)
            {
                isvisible = visibleN == true;
            }
            else
            {
                isvisible = visible;
            }

            if (isvisible)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
