// <copyright file="BindingUtils.cs" company="Syncfusion">
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
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class that stores methods used to operate on data bindings.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class BindingUtils
    {
        /// <summary>
        /// Property to Enable or Disable DataBinding Errors.
        /// </summary>
        public static readonly DependencyProperty EnableBindingErrorsProperty = DependencyProperty.RegisterAttached(
            "EnableBindingErrors",
            typeof(bool),
            typeof(BindingUtils),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnEnableBindingErrorsChanged)));

        /// <summary>
        /// Gets EnableBindingErrors for given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        public static bool GetEnableBindingErrors(DependencyObject obj)
        {
            if (obj != null)
                return (bool)obj.GetValue(EnableBindingErrorsProperty);
            else
                return true;
        }

        /// <summary>
        /// Sets EnableBindingErrors for given object.
        /// </summary>
        /// <param name="obj">Given object</param>
        /// <param name="value">Given value</param>
        public static void SetEnableBindingErrors(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableBindingErrorsProperty, value);         
        }

        private static void OnEnableBindingErrorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            BindingUtils.SetEnableBindingErrors(obj,(bool)e.NewValue);
        }
        #region Static Public Methods

        /// <summary>
        /// Creates and associates a new instance of <see cref="BindingExpressionBase"/> with the specified binding target property.
        /// </summary>
        /// <param name="target">The binding target of the binding.</param>
        /// <param name="source">Source of the data for the binding.</param>
        /// <param name="dp">The target property of the binding.</param>
        /// <param name="propertyPath">Path (can be DependencyProperty, or just string with property name) to the source property.</param>
        /// <returns>A new instance of <see cref="BindingExpressionBase"/>.</returns>
        public static BindingExpressionBase SetBinding(DependencyObject target, object source, DependencyProperty dp, object propertyPath)
        {
            return SetBinding(target, source, dp, propertyPath, BindingMode.Default, null);
        }

        /// <summary>
        /// Creates and associates a new instance of <see cref="BindingExpressionBase"/> with the specified binding target property.
        /// </summary>
        /// <param name="target">The binding target of the binding.</param>
        /// <param name="source">Source of the data for the binding.</param>
        /// <param name="dp">The target property of the binding.</param>
        /// <param name="propertyPath">Path (can be DependencyProperty, or just string with property name) to the source property.</param>
        /// <param name="mode">Binding mode.</param>
        /// <returns>A new instance of <see cref="BindingExpressionBase"/>.</returns>
        public static BindingExpressionBase SetBinding(DependencyObject target, object source, DependencyProperty dp, object propertyPath, BindingMode mode)
        {
            return SetBinding(target, source, dp, propertyPath, mode, null);
        }

        /// <summary>
        /// Creates and associates a new instance of <see cref="BindingExpressionBase"/> with the specified binding target property.
        /// </summary>
        /// <param name="target">The binding target of the binding.</param>
        /// <param name="source">Source of the data for the binding.</param>
        /// <param name="dp">The target property of the binding.</param>
        /// <param name="propertyPath">Path (can be DependencyProperty, or just string with property name) to the source property.</param>
        /// <param name="mode">Binding mode</param>
        /// <param name="converter">Binding converter.</param>
        /// <returns>A new instance of <see cref="BindingExpressionBase"/>.</returns>
        public static BindingExpressionBase SetBinding(
            DependencyObject target, 
            object source,
            DependencyProperty dp,
            object propertyPath,
            BindingMode mode,
            IValueConverter converter)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }

            if (propertyPath == null)
            {
                throw new ArgumentNullException("sourcePropertyName");
            }

            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath(propertyPath);
            binding.Mode = mode;
            binding.Converter = converter;

            return BindingOperations.SetBinding(target, dp, binding);
        }

        /// <summary>
        /// Sets the relative binding.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dp">The dependency property.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="sourceProperty">The source property.</param>
        /// <returns>Returns relative binding reference</returns>
        public static BindingExpressionBase SetRelativeBinding(DependencyObject target, DependencyProperty dp, Type sourceType, object sourceProperty)
        {
            return SetRelativeBinding(target, dp, sourceType, sourceProperty, BindingMode.Default, 1);
        }

        /// <summary>
        /// Sets the relative binding.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dp">The dependency property.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="mode">The binding mode.</param>
        /// <returns>Returns relative binding reference</returns>
        public static BindingExpressionBase SetRelativeBinding(DependencyObject target, DependencyProperty dp, Type sourceType, object sourceProperty, BindingMode mode)
        {
            return SetRelativeBinding(target, dp, sourceType, sourceProperty, mode, 1);
        }

        /// <summary>
        /// Creates and associates a new instance of <see cref="BindingExpressionBase"/> with the specified relative binding target property.
        /// </summary>
        /// <param name="target">The binding target of the binding.</param>
        /// <param name="dp">The target property of the binding.</param>
        /// <param name="sourceType">Type of the source (to be found among the ancestors) of the data for the binding.</param>
        /// <param name="sourceProperty">Path (can be <see cref="DependencyProperty"/>, or just string with property name) to the source property.</param>
        /// <param name="mode">Binding mode.</param>
        /// <param name="ancestorLevel">Ancestor level, should be greater or equal to 1.</param>
        /// <returns>A new instance of <see cref="BindingExpressionBase"/>.</returns>
        public static BindingExpressionBase SetRelativeBinding(DependencyObject target, DependencyProperty dp, Type sourceType, object sourceProperty, BindingMode mode, int ancestorLevel)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException("sourceType");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }

            if (sourceProperty == null)
            {
                throw new ArgumentNullException("sourcePropertyName");
            }

            Binding binding = new Binding();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, sourceType, ancestorLevel);
            binding.Path = new PropertyPath(sourceProperty);
            binding.Mode = mode;

            return BindingOperations.SetBinding(target, dp, binding);
        }

        /// <summary>
        /// Creates and associates a new instance of <see cref="BindingExpressionBase"/> with the specified template parent property.
        /// </summary>
        /// <param name="target">The binding target.</param>
        /// <param name="dp">The target property of the binding.</param>
        /// <param name="sourceProperty">Source of the data for the binding.</param>
        /// <param name="mode">Binding mode.</param>
        /// <returns>A new instance of <see cref="BindingExpressionBase"/>.</returns>
        public static BindingExpressionBase SetTemplatedParentBinding(DependencyObject target, DependencyProperty dp, object sourceProperty, BindingMode mode)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }

            if (sourceProperty == null)
            {
                throw new ArgumentNullException("sourcePropertyName");
            }

            Binding binding = new Binding();
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
            binding.Path = new PropertyPath(sourceProperty);
            binding.Mode = mode;

            return BindingOperations.SetBinding(target, dp, binding);
        }

        /// <summary>
        /// Enumerates all element visual children of the specified type and sets binding for the specified property.
        /// </summary>
        /// <param name="rootelement">Specifies root element of the visual tree to inspect.</param>
        /// <param name="typeChild">Specifies type of the visual children to add binding to.</param>
        /// <param name="source">Specifies source of the binding.</param>
        /// <param name="dp">Specifies dependency property on the children to be binded.</param>
        /// <param name="sourcePropertyName">Specifies dependency property on the source to create binding to.</param>
        public static void SetBindingToVisualChild(FrameworkElement rootelement, Type typeChild, object source, DependencyProperty dp, object sourcePropertyName)
        {
            if (rootelement == null)
            {
                throw new ArgumentNullException("rootelement");
            }

            if (typeChild == null)
            {
                throw new ArgumentNullException("typeChild");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }

            if (sourcePropertyName == null)
            {
                throw new ArgumentNullException("sourcePropertyName");
            }

            if (!(sourcePropertyName is string) && !(sourcePropertyName is DependencyProperty))
            {
                throw new ArgumentException("Value should be specified as string or as dependency property instance.", "sourcePropertyName");
            }

            Binding binding = null;

            foreach (Visual vis in VisualUtils.EnumChildrenOfType(rootelement, typeChild))
            {
                if (binding != null)
                {
                    binding = new Binding();
                    binding.Source = source;
                    binding.Path = new PropertyPath(sourcePropertyName);
                    binding.Mode = BindingMode.Default;
                }

                BindingOperations.SetBinding(vis, dp, binding);
            }
        }
        #endregion
    }
}
