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
using System.Reflection;
using System.Windows;

namespace Syncfusion.Styles
{
    ///// <summary> 
    /////     FrugalMapIterationCallback
    ///// </summary>
    //internal delegate void StyleInfoFrugalMapIterationCallback(ArrayList list, int key, object value);

    public class StyleInfoObjectStore : IDisposable
    {
        // Fields
        private static int currentKey;

        object frugalMap;

        static Type frugalMapType;
        static MethodInfo getItem;
        static MethodInfo setItem;
        static MethodInfo sort;
        static MethodInfo getKeyValuePair;
        //static MethodInfo iterate;
        static MethodInfo getCount;

        // Ctor
        static StyleInfoObjectStore()
        {
            Assembly asm = Assembly.Load("WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            frugalMapType = asm.GetType("MS.Utility.FrugalMap", true);

            PropertyInfo item = frugalMapType.GetProperty("Item");
            getItem = item.GetGetMethod();
            setItem = item.GetSetMethod();

            sort = frugalMapType.GetMethod("Sort");

                getKeyValuePair = frugalMapType.GetMethod("GetKeyValuePair");

            //iterate = frugalMapType.GetMethod("Iterate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            PropertyInfo count = frugalMapType.GetProperty("Count");
            getCount = count.GetGetMethod();
        }

        public StyleInfoObjectStore()
        {
            frugalMap = Activator.CreateInstance(frugalMapType);
        }

        public void Dispose()
        {
        }

        // Methods

        object this[int key]
        {
            get
            {
                return getItem.Invoke(frugalMap, new object[] { key });
            }
            set
            {
                setItem.Invoke(frugalMap, new object[] { key, value });
            }
        }

        public object GetObject(int key)
        {
            object obj = this[key];

            if (obj == DependencyProperty.UnsetValue)
            {
                obj = null;
            }

            return obj;
        }

        public bool ContainsObject(int key)
        {
            object obj = this[key];
            return obj != DependencyProperty.UnsetValue;
        }

        public static int CreateKey()
        {
            return currentKey++;
        }


        public object GetObject(int key, out bool found)
        {
            object obj = this[key];
            found = true;

            if (obj == DependencyProperty.UnsetValue)
            {
                found = false;
                obj = null;
            }

            return obj;
        }

        public void SetObject(int key, object value)
        {
            this[key] = value;
        }

        public void RemoveObject(int key)
        {
            this[key] = DependencyProperty.UnsetValue;
        }

        public void GetKeyValuePair(int index, out int key, out object value)
        {
            object[] arrParms = new object[3];
            arrParms.SetValue(index, 0);

            // no need to set the second and third parameter -- it will be filled in
            getKeyValuePair.Invoke(frugalMap, arrParms);
            key = (int)arrParms[1];
            value = arrParms[2];
        }

        //public void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        //{
        //}

        public void Sort()
        {
            sort.Invoke(frugalMap, new object[0]);
        }

        // Properties
        public int Count
        {
            get
            {
                return (int) getCount.Invoke(frugalMap, new object[0]);
            }
        }

    }
}