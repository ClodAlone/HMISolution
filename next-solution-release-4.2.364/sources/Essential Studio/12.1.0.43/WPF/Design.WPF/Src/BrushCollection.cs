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
using System.Windows.Media;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Design
{
    public class BrushCollection : ObservableCollection<BrushObject>
    {
        public BrushCollection()
            : base()
        {
            Type type = typeof(Brushes);
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (propertyInfo.PropertyType == typeof(SolidColorBrush))
                    Add(new BrushObject(propertyInfo.Name, (SolidColorBrush)propertyInfo.GetValue(null, null)));
            }
        }
    }
    public class BrushObject
    {
        string _name;
        SolidColorBrush _brush;
        public BrushObject()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public SolidColorBrush Brush
        {
            get
            {
                return _brush;
            }
            set
            {
                _brush = value;
            }
        }
        public BrushObject(string name, SolidColorBrush brush)
        {
            _name = name;
            _brush = brush;
            Color color = brush.Color;
        }

    }
}
