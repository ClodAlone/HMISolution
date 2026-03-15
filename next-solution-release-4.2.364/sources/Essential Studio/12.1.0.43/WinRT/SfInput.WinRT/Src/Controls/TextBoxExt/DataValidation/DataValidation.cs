#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a class for Data validation
    /// </summary>
    public class DataValidation
    {
        private static PropertyChangedEventHandler handler;

        /// <summary>
        /// Returns a value when set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        public static bool GetNotifyOnDataErrors(DependencyObject obj)
        {
            return (bool)obj.GetValue(NotifyOnDataErrorsProperty);
        }

        /// <summary>
        /// Sets the value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetNotifyOnDataErrors(DependencyObject obj, bool value)
        {
            obj.SetValue(NotifyOnDataErrorsProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NotifyOnDataErrors.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NotifyOnDataErrorsProperty =
            DependencyProperty.RegisterAttached("NotifyOnDataErrors", typeof(bool), typeof(DataValidation), new PropertyMetadata(false, OnNotifyOnDataErrorsChanged));

        private static void OnNotifyOnDataErrorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)  
        {
            var element = sender as FrameworkElement;
            if(element != null)
            {
                element.Loaded -= element_Loaded;
                element.Loaded += element_Loaded;
            }
        }

        static void element_Unloaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                var datamodel = element.DataContext as INotifyPropertyChanged;
                if (handler != null)
                {
                    if (datamodel != null)
                    {
                        datamodel.PropertyChanged -= handler;
                    }
                }
                element.Unloaded -= element_Unloaded;
            }
        }

        static void element_Loaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                var datamodel = element.DataContext as INotifyPropertyChanged;
                if (GetNotifyOnDataErrors(sender as DependencyObject))
                {
                    if (datamodel != null)
                    {
                        //Need validation on loaded.
                        ValidateData(element, GetPropertyPath(element), datamodel);

                        handler = (o, args) => ValidateData(element, args.PropertyName, o);
                        datamodel.PropertyChanged += handler;
                    }
                }
                element.Unloaded += element_Unloaded;
            }
        }

        private static void ValidateData(FrameworkElement element, string propertyName, object datamodel)
        {
            string propertypath = GetPropertyPath(element);

            if (propertyName == propertypath)
            {
                var dataValidation = datamodel as IDataValidation;
                if (dataValidation != null)
                {
                    var validator = element as IDataValidator;
                    if (validator != null)
                    {
                        string errormessage = dataValidation[propertypath];
                        bool hasError = !String.IsNullOrEmpty(errormessage);
                        DataValidation.SetHasError(element, hasError);
                        DataValidation.SetErrorMessage(element, errormessage);
                        var _args = new ValidationEventArgs
                                        {
                                            ErrorMessage = errormessage,
                                            HasError = hasError
                                        };
                        validator.Validate(_args);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Property path
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetPropertyPath(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyPathProperty);
        }

        /// <summary>
        /// Sets the Property path
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetPropertyPath(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyPathProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PropertyPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PropertyPathProperty =
            DependencyProperty.RegisterAttached("PropertyPath", typeof(string), typeof(DataValidation), new PropertyMetadata(String.Empty));

        /// <summary>
        /// Returns a value when set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        public static bool GetHasError(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasErrorProperty);
        }

        /// <summary>
        /// Sets if there is an error
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetHasError(DependencyObject obj, bool value)
        {
            obj.SetValue(HasErrorProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasError.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.RegisterAttached("HasError", typeof(bool), typeof(DataValidation), new PropertyMetadata(false));


        /// <summary>
        /// Gets a string as error message.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetErrorMessage(DependencyObject obj)
        {
            return (string)obj.GetValue(ErrorMessageProperty);
        }

        /// <summary>
        /// Sets a string as error message.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetErrorMessage(DependencyObject obj, string value)
        {
            obj.SetValue(ErrorMessageProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ErrorMessage.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.RegisterAttached("ErrorMessage", typeof(string), typeof(DataValidation), new PropertyMetadata(String.Empty));
    }
}
