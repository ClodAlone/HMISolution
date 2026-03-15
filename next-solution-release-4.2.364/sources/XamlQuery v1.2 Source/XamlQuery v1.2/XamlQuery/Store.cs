//Store.cs

/*
 * XamlQuery Silverlight Library v1.2
 * http://XamlQuery.codeplex.com/
 * 
 * Author: cincoutprabu (cincoutprabu@gmail.com)
 * Copyright 2010 cincoutprabu. All rights reserved.
 * 
 * Released under Microsoft Public License.
 * http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 *
 * Date: 2010-Sep-10 06:00 IST
 * Revision: 1
 */

using System;
using System.Collections.Generic;
using System.Windows;

namespace XamlQuery
{
    internal class Store : Dictionary<string, object>
    {
    }

    internal class DependencyObjectStore : DependencyObject
    {
        #region Properties

        public static DependencyProperty StoreProperty = DependencyProperty.RegisterAttached("Store", typeof(Store), typeof(DependencyObject), null);

        public Store Store
        {
            get
            {
                return (Store)base.GetValue(StoreProperty);
            }
            set
            {
                base.SetValue(StoreProperty, value);
            }
        }

        #endregion

        #region Methods

        private static Store GetStore(DependencyObject control)
        {
            Store store = (Store)control.GetValue(DependencyObjectStore.StoreProperty);
            if (store == null)
            {
                store = new Store();
                control.SetValue(DependencyObjectStore.StoreProperty, store);
            }
            return (store);
        }

        public static void SetData(DependencyObject control, string key, object value)
        {
            Store store = GetStore(control);
            if (store.ContainsKey(key))
            {
                store[key] = value;
            }
            else
            {
                store.Add(key, value);
            }
        }

        public static object GetData(DependencyObject control, string key)
        {
            Store store = GetStore(control);
            if (store.ContainsKey(key))
            {
                return (store[key]);
            }
            else
            {
                return (null);
            }
        }

        #endregion
    }
}
