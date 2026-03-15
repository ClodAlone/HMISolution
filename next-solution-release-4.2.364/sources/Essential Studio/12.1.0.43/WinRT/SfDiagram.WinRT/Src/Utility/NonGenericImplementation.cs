#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public class DiagramKeyValue: DiagramKeyValue<DataTemplate>
    {
    }

    public class DataTemplateDictionary : DiagramDictionary<DataTemplate>
    {
    }

    public class DiagramCollection<T> : ObservableCollection<T>
    {
    }

    public sealed class DiagramCollection : DiagramCollection<object>
    {
    }
}
