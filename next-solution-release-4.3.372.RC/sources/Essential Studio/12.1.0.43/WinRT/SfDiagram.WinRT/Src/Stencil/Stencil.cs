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
using System.ComponentModel;
using System.Linq;
#if WINRT_USING
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#else
using System.Windows.Controls; 
#endif
using System.Reflection;
using System.Windows;
using ScrollViewer = Syncfusion.UI.Xaml.Diagram.Controls.ScrollViewer;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Diagram.Stencil
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public partial class Stencil : Control
    {
        internal Queue<SymbolGroup> _mSymbolGroup = new Queue<SymbolGroup>();
        internal Queue<SymbolGroup> _mExpandedGroup = new Queue<SymbolGroup>();

        public Stencil()
        {
            this.DefaultStyleKey = typeof(Stencil);
            SymbolPreview = new ContentPresenter()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0.5,
                //HorizontalContentAlignment = HorizontalAlignment.Center,
                //VerticalContentAlignment = VerticalAlignment.Center
            };
        }

        private void DoApplyTemplate()
        {
            if (SelectedFilter != null)
            {
                InvalidateGroupingFiltering();
            }
        }

        public IEnumerable<ISymbol> SymbolSource
        {
            get { return (IEnumerable<ISymbol>)GetValue(SymbolSourceProperty); }
            set { SetValue(SymbolSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolSourceProperty =
            DependencyProperty.Register("SymbolSource", typeof(IEnumerable<ISymbol>), typeof(Stencil), new PropertyMetadata(null, OnSymbolSourceChanged));

        private static void OnSymbolSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Stencil stencil = d as Stencil;
        }

        public SymbolGroups SymbolGroups
        {
            get { return (SymbolGroups)GetValue(SymbolGroupsProperty); }
            set { SetValue(SymbolGroupsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolGroups.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolGroupsProperty =
            DependencyProperty.Register("SymbolGroups", typeof(SymbolGroups), typeof(Stencil), new PropertyMetadata(null, OnSymbolGroupsChanged));

        private static void OnSymbolGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Stencil stencil = d as Stencil;
        }

        public SymbolFilters SymbolFilters
        {
            get { return (SymbolFilters)GetValue(SymbolFiltersProperty); }
            set { SetValue(SymbolFiltersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolFilters.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolFiltersProperty =
            DependencyProperty.Register("SymbolFilters", typeof(SymbolFilters), typeof(Stencil), new PropertyMetadata(null, OnSymbolFiltersChanged));

        private static void OnSymbolFiltersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Stencil stencil = d as Stencil;

        }

        public SymbolFilterProvider SelectedFilter
        {
            get { return (SymbolFilterProvider)GetValue(SelectedFilterProperty); }
            set { SetValue(SelectedFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(SymbolFilterProvider), typeof(Stencil), new PropertyMetadata(null, OnSelectedFilterChanged));

        private static void OnSelectedFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Stencil stencil = d as Stencil;
            if (e.NewValue != null)
            {
                stencil.InvalidateGroupingFiltering();
            }
        }    

        private void InvalidateGroupingFiltering()
        {
            Func<ISymbol, object>[] groups = new Func<ISymbol, object>[SymbolGroups.Count];
            Func<object, object>[] headers = new Func<object, object>[SymbolGroups.Count];
            int i = 0;
            foreach (var symbolGroup in SymbolGroups)
            {
                string map = symbolGroup.MappingName;
                Func<ISymbol, object> group =
                    symbol => symbol.GetType().GetRuntimeProperty(map).GetValue(symbol);
                Func<object, object> head = symbolGroup.Header;
                headers[i] = head;
                groups[i] = group;
                i++;
            }

            IEnumerable<GroupedSymbols> groupedSymbols = SymbolSource.GroupByMany(SelectedFilter, headers, groups);
            //this.DataContext = groupedSymbols;
            SetValue(SymbolGroupProperty, groupedSymbols);
        }

        //internal object SymbolGroup
        //{
        //    get { return (object)GetValue(SymbolGroupProperty); }
        //    set { SetValue(SymbolGroupProperty, value); }
        //}

        // Using a DependencyProperty as the backing store for SymbolGroup.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SymbolGroupProperty =
            DependencyProperty.Register("SymbolGroup", typeof(object), typeof(Stencil), new PropertyMetadata(null));

        public Symbol SelectedSymbol
        {
            get { return (Symbol)GetValue(SelectedSymbolProperty); }
            set { SetValue(SelectedSymbolProperty, value); }
        }

        public ContentPresenter SymbolPreview
        {
            get { return (ContentPresenter)GetValue(SymbolPreviewProperty); }
            set { SetValue(SymbolPreviewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreiviewControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolPreviewProperty =
            DependencyProperty.Register("SymbolPreview", typeof(ContentPresenter), typeof(Stencil), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for SelectedSymbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedSymbolProperty =
            DependencyProperty.Register("SelectedSymbol", typeof(Symbol), typeof(Stencil), new PropertyMetadata(null,OnSelectedSymbolChanged));

        private static void OnSelectedSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Stencil stencil = d as Stencil;
            if (e.NewValue != null)
            {
                stencil.PrepareDragDropPreview();
            }           
        }

        public StencilConstraints Constraints
        {
            get { return (StencilConstraints)GetValue(ConstraintsProperty); }
            set { SetValue(ConstraintsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Constraints.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConstraintsProperty =
            DependencyProperty.Register("Constraints", typeof(StencilConstraints), typeof(Stencil), new PropertyMetadata(StencilConstraints.Default));

        public ExpandMode ExpandMode
        {
            get { return (ExpandMode)GetValue(ExpandModeProperty); }
            set { SetValue(ExpandModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandModeProperty =
            DependencyProperty.Register("ExpandMode", typeof(ExpandMode), typeof(Stencil), new PropertyMetadata(ExpandMode.One));
      
        protected virtual void PrepareDragDropPreview()
        {
            ISymbol clone =(this.SelectedSymbol.DataContext as ISymbol).Clone();
            SymbolPreview.Content = clone.Symbol;
            SymbolPreview.ContentTemplate = clone.SymbolTemplate;   
        }
    }

    internal class GroupedSymbols
    {
        public object Key { get; private set; }
        public Func<object, object> Header { get; private set; }
        public IEnumerable Items { get; set; }
        public IEnumerable<GroupedSymbols> SubGroups { get; private set; }

        public GroupedSymbols(object key, Func<object, object> header, IEnumerable<GroupedSymbols> enu)
        {
            Key = key;
            Header = header;
            SubGroups = enu;
        }
    }

    internal static class GroupExtension
    {
        public static IEnumerable<GroupedSymbols> GroupByMany(
            this IEnumerable<ISymbol> elements,
            SymbolFilterProvider selectedFilter,
            Func<object, object>[] headers,
            params Func<ISymbol, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();
                var header = headers.First();
                var nextSelectors = groupSelectors.Skip(1).ToArray();
                var nextHeaders = headers.Skip(1).ToArray();
                if (groupSelectors.Length > 1)
                {
                    return
                        elements
                            .Where(symbol => selectedFilter.Filter(selectedFilter, symbol))
                            .GroupBy(selector)
                            .Select(
                                g => new GroupedSymbols(g.Key, header, g.GroupByMany(selectedFilter, nextHeaders, nextSelectors)) { Items = g });
                }
                else
                {
                    return elements
                        .Where(symbol => selectedFilter.Filter(selectedFilter, symbol))
                        .GroupBy(selector).Select(g => new GroupedSymbols(g.Key, header, null) { Items = g });
                }
            }
            else
                return null;
        }
    }
}
