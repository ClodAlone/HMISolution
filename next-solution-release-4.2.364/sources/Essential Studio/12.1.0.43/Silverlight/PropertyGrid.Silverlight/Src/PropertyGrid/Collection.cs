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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.PropertyGrid
{
    public class PropertyCategoryViewItemCollection
    {
        private ObservableCollection<object> _Properties = new ObservableCollection<object>();
        public ObservableCollection<object> Properties
        {
            get { return _Properties; }
            set { _Properties = value; }
        }

        public string Category
        {
            get;
            set;
        }
    }

    public class PropertyCollection : ObservableCollection<object>
    {

    }

    public class PropertyItemCollection : ObservableCollection<PropertyItem>
    {
        public PropertyItem this[string PropertyName]
        {
            get
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    PropertyItem item = this.Items[i] as PropertyItem;
                    if (item.Name == PropertyName)
                    {
                        return this.Items[i];
                    }
                }
                return null;
            }
        }
    }

    public class CategoryEditorCollection : ObservableCollection<CategoryEditor>
    {
        //public PropertyItem this[string PropertyName]
        //{
        //    get
        //    {
        //        for (int i = 0; i < this.Items.Count; i++)
        //        {
        //            PropertyItem item = this.Items[i] as PropertyItem;
        //            if (item.Name == PropertyName)
        //            {
        //                return this.Items[i];
        //            }
        //        }
        //        return null;
        //    }
        //}
    }

    public class CategoryEditorPropertyCollection : ObservableCollection<CategoryEditorProperty>
    {

    }

    public interface IProperty
    {
        string PropertyName
        {
            get;
            set;
        }

        string PropertyType
        {
            get;
            set;
        }

        
    }

}
