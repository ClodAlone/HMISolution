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

#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI.Xaml; 
#endif

namespace Syncfusion.UI.Xaml.Diagram.Stencil
{
    public interface ISymbol
    {
        object Symbol { get; set; }
        DataTemplate SymbolTemplate { get; set; }
        ISymbol Clone();
    }
    
    public class SymbolGroupProvider 
    {
        public string MappingName { get; set; }
        public Func<object, object> Header { get; set; }
    }

    public class SymbolGroups : ObservableCollection<SymbolGroupProvider>
    {
    }

    public class SymbolFilterProvider
    {
        public object Content { get; set; }

        public Predicate Filter { get; set; }
    }

    public delegate bool Predicate(SymbolFilterProvider sender, ISymbol symbol);

    public class SymbolFilters : ObservableCollection<SymbolFilterProvider>
    {
    }

    public class DragSymbol
    {
        public ISymbol Symbol { get; set; }
    }
}
