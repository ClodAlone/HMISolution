#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    internal static class SkinStorageExtension
    {
		static ObservableCollection<string> MergedDictionaryPathCollection;
        internal static void AddIntoMergedDictionaryPath(this FrameworkElement element, string path)
        {
            MergedDictionaryPathCollection = SkinStorage.GetMergedDictionaryPath(element);
            if (MergedDictionaryPathCollection == null)
            {
                MergedDictionaryPathCollection = new ObservableCollection<string>();
            }
            if (!MergedDictionaryPathCollection.Contains(path))
            {
                MergedDictionaryPathCollection.Add(path);
            }
        }
        internal static int IndexOfMergedDictionaryPath(this FrameworkElement element, string path)
        {
            MergedDictionaryPathCollection = SkinStorage.GetMergedDictionaryPath(element);
            return MergedDictionaryPathCollection != null ? MergedDictionaryPathCollection.IndexOf(path) : -1;
        }

        internal static void RemoveFromMergedDictionaryPath(this FrameworkElement element, string path)
        {
            MergedDictionaryPathCollection = SkinStorage.GetMergedDictionaryPath(element);
            if (MergedDictionaryPathCollection!=null && MergedDictionaryPathCollection.Contains(path))
            {
                MergedDictionaryPathCollection.Remove(path);
            }
        }

        internal static bool IsContainsInMergedDictionaryPath(this FrameworkElement element, string path)
        {
            MergedDictionaryPathCollection = SkinStorage.GetMergedDictionaryPath(element);
            return MergedDictionaryPathCollection!=null?MergedDictionaryPathCollection.Contains(path):false;
        }

        internal static ObservableCollection<string> GetMergedDictionaryPath(this FrameworkElement element)
        {
            MergedDictionaryPathCollection = SkinStorage.GetMergedDictionaryPath(element);
            return MergedDictionaryPathCollection;
        }

        internal static void ClearMergedDictionaryPath(this FrameworkElement element)
        {
            MergedDictionaryPathCollection = SkinStorage.GetMergedDictionaryPath(element);
            if (MergedDictionaryPathCollection != null)
            {
                MergedDictionaryPathCollection.Clear();
                MergedDictionaryPathCollection = null;
            }
        }
    }
}
