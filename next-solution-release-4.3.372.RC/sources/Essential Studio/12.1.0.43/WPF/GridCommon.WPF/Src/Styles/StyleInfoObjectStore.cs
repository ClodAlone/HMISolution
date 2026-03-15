#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Windows;
using Syncfusion.Windows.Internal;

namespace Syncfusion.Windows.Styles
{
    /// <internalonly/>
    internal class StyleInfoObjectStore : IDisposable
    {
        // Fields
        private static int currentKey;

        FrugalMap frugalMap;

        // Ctor
        static StyleInfoObjectStore()
        {
        }

        public StyleInfoObjectStore()
        {
            frugalMap = new FrugalMap();
        }

        public void Dispose()
        {
            ArrayList list = new ArrayList();
            Iterate(list, new FrugalMapIterationCallback(DisposeCallback));
            frugalMap._mapStore = null;
            GC.SuppressFinalize(this);
        }

        void DisposeCallback(ArrayList list, int key, object value)
        {
            _Dispose(key, value);
        }


        void _Dispose(int key, object obj)
        {
            if (obj is StyleInfoSubObjectBase)
                ((StyleInfoSubObjectBase)obj).Dispose();
            frugalMap._mapStore.RemoveEntry(key);
            frugalMap[key] = null;
            obj = null;
        }


        // Methods

        object this[int key]
        {
            get
            {
                return frugalMap[key];
            }
            set
            {
                frugalMap[key] =value;
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
            frugalMap.GetKeyValuePair(index, out key, out value);
        }

        internal void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            frugalMap.Iterate(list, callback);
        }

        public void Sort()
        {
            frugalMap.Sort();
        }

        // Properties
        public int Count
        {
            get
            {
                return frugalMap.Count;
            }
        }

    }
}