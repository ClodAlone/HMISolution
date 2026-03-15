#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Reflection;
#if WPF
#else
using Windows.UI.Xaml;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class DefaultValueDictionary : Dictionary<string, object>
    {
        private object ElementFactory
        {
            get;
            set;
        }

        public DefaultValueDictionary(object input)
            : base()
        {
            ElementFactory = input;
            CollectDefaultValues();
        }

        public DefaultValueDictionary()
        {
        }

        internal void CollectDefaultValues()
        {
            FieldInfo[] properties = ElementFactory.GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo field in properties)
            {
                if (field.FieldType == typeof(DependencyProperty))
                {
                    DependencyProperty dp = (DependencyProperty)field.GetValue(null);
                    if (!field.Name.Contains("Template") && !field.Name.Contains("Style") && !field.Name.Equals("Resources") && !field.Name.Equals("Items")
                        && !field.Name.Contains("Transform") && !field.Name.Contains("Panel") && !field.Name.Contains("MemberPath"))
                    {
                        Type type = dp.GetType();
                        try
                        {
                            object value = dp.GetMetadata(type);
                            Add(field.Name.ToLower(), value);
                        }
                        catch { }
                    }
                }
            }
        }

        internal bool HasDefaultValue(PropertyInfo propinfo)
        {
            return this.ContainsKey(propinfo.Name.ToLower() + "property");
        }
    }
}
