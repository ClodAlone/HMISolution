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
using System.Reflection;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Data;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Grid.Utility
{
    internal static class BindingUtility
    {
        public static Binding CreateDisplayBinding(this Binding source)
        {
            return CreateDisplayBinding(source, true);
        }

        internal static Binding CreateDisplayBinding(this Binding source, bool enableErrorNotification)
        {
            var binding = new Binding
                {
                    Converter = source.Converter,
                    ConverterParameter = source.ConverterParameter,
                    Mode = BindingMode.OneWay,
                    Path = source.Path,
#if !WinRT
                    BindsDirectlyToSource = source.BindsDirectlyToSource,
                    ConverterCulture = source.ConverterCulture,
                    FallbackValue = source.FallbackValue,
                    NotifyOnValidationError = enableErrorNotification,
                    StringFormat = source.StringFormat,
                    TargetNullValue = source.TargetNullValue,
                    UpdateSourceTrigger = source.UpdateSourceTrigger,
                    ValidatesOnDataErrors = enableErrorNotification,
                    ValidatesOnExceptions = source.ValidatesOnExceptions,
#if SyncfusionFramework4_5
                    ValidatesOnNotifyDataErrors = enableErrorNotification,
#endif
#if !SILVERLIGHT && !WP
                    AsyncState = source.AsyncState,
                    BindingGroupName = source.BindingGroupName,
#if SyncfusionFramework4_5
                    Delay = source.Delay,
#endif
                    IsAsync = source.IsAsync,
                    NotifyOnSourceUpdated = source.NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = source.NotifyOnTargetUpdated,
                    UpdateSourceExceptionFilter = source.UpdateSourceExceptionFilter,
                    XPath= source.XPath
#endif
#endif
                };
            if (source.ElementName != null)
            {
                binding.ElementName = source.ElementName;
                return binding;
            }
            if (source.RelativeSource != null)
            {
                binding.RelativeSource = source.RelativeSource;
                return binding;
            }
            if (source.Source != null)
            {
                binding.Source = source.Source;
            }
#if WPF
            foreach (var validationRule in source.ValidationRules)
            {
                binding.ValidationRules.Add(validationRule);
            }
#endif
            return binding;
        }

#if !WinRT
        public static Binding CreateEditBinding(this Binding source, UpdateSourceTrigger updateSourceTrigger)
        {
            return CreateEditBinding(source, updateSourceTrigger, true);
        }
#else
        public static Binding CreateEditBinding(this Binding source)
        {
            return CreateEditBinding(source, true);
        }
#endif

#if !WinRT
        internal static Binding CreateEditBinding(this Binding source, UpdateSourceTrigger updateSourceTrigger, bool enableErrorNotification)
#else
        internal static Binding CreateEditBinding(this Binding source, bool enableErrorNotification)
#endif

        {
            var binding = new Binding
            {
                Converter = source.Converter,
                ConverterParameter = source.ConverterParameter,
                Mode = BindingMode.TwoWay,
                Path = source.Path,
#if !WinRT
                BindsDirectlyToSource = source.BindsDirectlyToSource,
                ConverterCulture = source.ConverterCulture,
                FallbackValue = source.FallbackValue,
                NotifyOnValidationError = enableErrorNotification,
                StringFormat = source.StringFormat,
                TargetNullValue = source.TargetNullValue,
#if !WP
#if SILVERLIGHT
                //NOTE:Syncfusion's Tools Control doesn't have support for UpdateSourceTrigger while Binding
                //As a workaround UpdateSourceTrigger will be set as Explicit and BindingExpression will be updated on LostFocus of the Renderer
                //Silverlight4 : Enums - Default/Explicit; Silverlight5 : Enums - Default/PropertyChanged/Explicit
                //https://syncfusion.atlassian.net/browse/SL-2531
                UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
#else
                UpdateSourceTrigger = updateSourceTrigger==UpdateSourceTrigger.PropertyChanged ? UpdateSourceTrigger.Explicit : updateSourceTrigger,
#endif
#else
                UpdateSourceTrigger=updateSourceTrigger,
#endif
                ValidatesOnDataErrors = enableErrorNotification,
                ValidatesOnExceptions = source.ValidatesOnExceptions,
#if SyncfusionFramework4_5
				ValidatesOnNotifyDataErrors = enableErrorNotification,
#endif
#if !SILVERLIGHT && !WP
                AsyncState = source.AsyncState,
                BindingGroupName = source.BindingGroupName,
#if SyncfusionFramework4_5
                    Delay = source.Delay,
#endif
                IsAsync = source.IsAsync,
                NotifyOnSourceUpdated = source.NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = source.NotifyOnTargetUpdated,
                UpdateSourceExceptionFilter = source.UpdateSourceExceptionFilter,
                XPath = source.XPath
#endif
#endif
            };
            if (source.ElementName != null)
            {
                binding.ElementName = source.ElementName;
                return binding;
            }
            if (source.RelativeSource != null)
            {
                binding.RelativeSource = source.RelativeSource;
                return binding;
            }
            if (source.Source != null)
            {
                binding.Source = source.Source;
            }
#if WPF
            foreach (var validationRule in source.ValidationRules)
            {
                binding.ValidationRules.Add(validationRule);
            }
#endif
            return binding;
        }

        public static Binding CreateDisplayBinding(this string mappingName)
        {
            var binding = new Binding
                {
                    Path = new PropertyPath(mappingName),
                    Mode= BindingMode.OneWay,
#if !WinRT
                    NotifyOnValidationError = true,
                    ValidatesOnDataErrors = true,
#if SyncfusionFramework4_5
                    ValidatesOnNotifyDataErrors = true,
#endif
#endif
                };
            return binding;
        }

        public static Binding CreateEditableBinding(this string mappingName)
        {
            return CreateEditableBinding(mappingName, true);
        }

        internal static Binding CreateEditableBinding(this string mappingName, bool enableErrorNotification)
        {
            var binding = new Binding
            {
                Path = new PropertyPath(mappingName),
                Mode = BindingMode.TwoWay,
#if !WinRT
                NotifyOnValidationError = enableErrorNotification,
                ValidatesOnDataErrors = enableErrorNotification,
#if SyncfusionFramework4_5
				ValidatesOnNotifyDataErrors = enableErrorNotification
#endif
#endif
            };
            return binding;
        }

        public static Binding CreateBinding(this Binding binding, object source)
        {
            var bind = new Binding
            {
                Converter = binding.Converter,
                ConverterParameter = binding.ConverterParameter,
                Mode = BindingMode.OneWay,
                Path = binding.Path,
                Source = source,
#if !WinRT
                    BindsDirectlyToSource = binding.BindsDirectlyToSource,
                    ConverterCulture = binding.ConverterCulture,
                    FallbackValue = binding.FallbackValue,
                    NotifyOnValidationError = binding.NotifyOnValidationError,
                    StringFormat = binding.StringFormat,
                    TargetNullValue = binding.TargetNullValue,
                    UpdateSourceTrigger = binding.UpdateSourceTrigger,
                    ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
                    ValidatesOnExceptions = binding.ValidatesOnExceptions,
#if SyncfusionFramework4_5
                    ValidatesOnNotifyDataErrors = binding.ValidatesOnNotifyDataErrors,
#endif
#if !SILVERLIGHT && !WP
                    AsyncState = binding.AsyncState,
                    BindingGroupName = binding.BindingGroupName,
#if SyncfusionFramework4_5
                    Delay = binding.Delay,
#endif
                    IsAsync = binding.IsAsync,
                    NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
                    UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
                    XPath= binding.XPath
#endif
#endif
            };
#if WPF
            foreach (var validationRule in binding.ValidationRules)
            {
                bind.ValidationRules.Add(validationRule);
            }
#endif
            return bind;
        }   
    }
}
