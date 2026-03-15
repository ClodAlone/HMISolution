#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if WinRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public static class GridDependencyProperty
    {
        private static Dictionary<DependencyProperty, GridPropertyMetadata> propertyMetadatas;

        static GridDependencyProperty()
        {
            propertyMetadatas = new Dictionary<DependencyProperty, GridPropertyMetadata>();
        }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, GridPropertyMetadata propertyMetadata)
        {
            propertyMetadata.DependencyPropertyName = name;
            var dependencyProperty = DependencyProperty.Register(name, propertyType, ownerType, propertyMetadata);
            propertyMetadatas.Add(dependencyProperty, propertyMetadata);
            return dependencyProperty;
        }

        public static GridPropertyMetadata GetDependencyPropertyMetadata(DependencyProperty p)
        {
            return propertyMetadatas[p];
        }
    }

    public class GridPropertyMetadata : PropertyMetadata
    {
        public string DependencyPropertyName { get; set; }

        public PropertyChangedCallback DependencyPropertyChangedCallback { get; set; }

        public GridPropertyMetadata(object defaultValue)
            : base(defaultValue, OnInternalDependencyPropertyChanged)
        {
            
        }

        public GridPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, OnInternalDependencyPropertyChanged)
        {
            DependencyPropertyChangedCallback = propertyChangedCallback;
        }

#if WPF
        public GridPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
            : base(defaultValue, OnInternalDependencyPropertyChanged, coerceValueCallback)
        {
            DependencyPropertyChangedCallback = propertyChangedCallback;
        }
#endif

        private static void OnInternalDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDependencyProperty.GetDependencyPropertyMetadata(e.Property).OnDependencyPropertyChanged(d, e);
        }

        private void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DependencyPropertyChangedCallback != null)
                DependencyPropertyChangedCallback(d, e);

            if (d is INotifyDependencyPropertyChanged)
                (d as INotifyDependencyPropertyChanged).OnDependencyPropertyChanged(DependencyPropertyName, e);
        }
    }

    public interface INotifyDependencyPropertyChanged
    {
        void OnDependencyPropertyChanged(string propertyName, DependencyPropertyChangedEventArgs e);
    }
}
