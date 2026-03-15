#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Collections;
#if WINRT_USING
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls; 
using TappedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Diagram.Stencil
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public partial class SymbolGroup : ItemsControl
    {
        private Stencil _mStencil = null;
        private Queue<SymbolGroup> _mLeafGroup = null;
        public SymbolGroup()
        {
            this.DefaultStyleKey = typeof(SymbolGroup);
            this.Loaded += SymbolGroup_Loaded;
            _mLeafGroup = new Queue<SymbolGroup>();


        }

        private void DoApplyTemplate()
        {
            _mStencil = this.FindVisualParent<Stencil>();
            ContentPresenter header = GetTemplateChild("PART_Header") as ContentPresenter;
            if (header != null)
            {
                #if WINRT
                header.Tapped += header_Tapped;
#endif
            }
            base.OnApplyTemplate();
        }

        void header_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Header != null)
            {
                if (_mStencil != null)
                {
                    if (_mStencil.ExpandMode == ExpandMode.One)
                    {
                        if (_mStencil._mExpandedGroup.Count == 1)
                        {
                            SymbolGroup parent = this.FindVisualParent<SymbolGroup>();
                            if (parent != null && parent._mLeafGroup.Count > 1)
                            {
                                for (int i = 0; i < parent._mLeafGroup.Count; i++)
                                {
                                    if (this != parent._mLeafGroup.ToList()[i])
                                    {
                                        if (parent._mLeafGroup.ToList()[i].IsExpanded)
                                        {
                                            parent._mLeafGroup.ToList()[i].IsExpanded = false;
                                            this.IsExpanded = true;
                                            if (this._mLeafGroup.Count > 0)
                                            {
                                                for (int j = 0; j < this._mLeafGroup.Count; j++)
                                                {
                                                    if (j == 0)
                                                    {
                                                        this._mLeafGroup.ToList()[j].IsExpanded = true;
                                                    }
                                                    else
                                                    {
                                                        this._mLeafGroup.ToList()[j].IsExpanded = false;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (_mStencil.ExpandMode == ExpandMode.OneOrMore)
                    {
                        SymbolGroup parent = this.FindVisualParent<SymbolGroup>();
                        if (!this.IsExpanded)
                        {
                            this.IsExpanded = true;
                        }
                        else
                        {
                            if (parent != null && parent._mLeafGroup.Count >= 1)
                            {
                                for (int i = 0; i < parent._mLeafGroup.Count; i++)
                                {
                                    if (this.IsExpanded)
                                    {
                                        if (parent._mLeafGroup.ToList()[i] != this)
                                        {
                                            if (parent._mLeafGroup.ToList()[i].IsExpanded)
                                            {
                                                this.IsExpanded = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.IsExpanded = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (_mStencil.ExpandMode == ExpandMode.ZeroOrOne)
                    {
                        SymbolGroup parent = this.FindVisualParent<SymbolGroup>();

                        if (!this.IsExpanded)
                        {
                            this.IsExpanded = true;
                            if (parent != null)
                            {
                                for (int i = 0; i < parent._mLeafGroup.Count; i++)
                                {
                                    if (parent._mLeafGroup.ToList()[i] != this)
                                    {
                                        if (parent._mLeafGroup.ToList()[i].IsExpanded)
                                        {
                                            parent._mLeafGroup.ToList()[i].IsExpanded = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.IsExpanded = false;
                        }
                    }
                    else if (_mStencil.ExpandMode == ExpandMode.ZeroOrMore)
                    {
                        if (this.IsExpanded)
                        {
                            this.IsExpanded = false;
                        }
                        else
                        {
                            this.IsExpanded = true;
                        }
                    }
                }
            }
        }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(SymbolGroup), new PropertyMetadata(null, OnHeaderChanged));

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolGroup sg = d as SymbolGroup;
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(SymbolGroup), new PropertyMetadata(null, OnHeaderTemplateChanged));

        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolGroup sg = d as SymbolGroup;
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SymbolGroup), new PropertyMetadata(true, OnIsExpandedChanged));

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolGroup s = d as SymbolGroup;
            Header h = s.FindVisualChild_Depth<Header>();
            if (s.IsExpanded)
            {
                s.GoToState(true, "Expanded");
                h.GoToState(true, "Expanded");
            }
            else
            {
                s.GoToState(true, "Collapsed");
                h.GoToState(true, "Collapsed");
            }

        }

        void GoToState(bool useTransitions, string name)
        {
            VisualStateManager.GoToState(this, name, useTransitions);
        }
        bool first = true;

        #region Logic for Expand

        //void SymbolGroup_Tapped(object sender, TappedRoutedEventArgs e)
        //{
        //    if ((sender as SymbolGroup).Header != null)
        //    {
        //        if (_mStencil != null)
        //        {
        //            if (_mStencil.ExpandMode == ExpandMode.One)
        //            {

        //                if (_mStencil._mExpandedGroup.Count == 1)
        //                {
        //                    SymbolGroup parent = this.FindVisualParent<SymbolGroup>();
        //                    if (parent != null && parent._mLeafGroup.Count > 1)
        //                    {
        //                        for (int i = 0; i < parent._mLeafGroup.Count; i++)
        //                        {
        //                            if (this != parent._mLeafGroup.ToList()[i])
        //                            {
        //                                if (parent._mLeafGroup.ToList()[i].IsExpanded)
        //                                {
        //                                    parent._mLeafGroup.ToList()[i].IsExpanded = false;
        //                                    this.IsExpanded = true;
        //                                    if (this._mLeafGroup.Count > 0)
        //                                    {
        //                                        for (int j = 0; j < this._mLeafGroup.Count; j++)
        //                                        {
        //                                            if (j == 0)
        //                                            {
        //                                                this._mLeafGroup.ToList()[j].IsExpanded = true;
        //                                            }
        //                                            else
        //                                            {
        //                                                this._mLeafGroup.ToList()[j].IsExpanded = false;
        //                                            }
        //                                        }
        //                                    }
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (_mStencil.ExpandMode == ExpandMode.OneOrMore)
        //            {
        //                SymbolGroup parent = this.FindVisualParent<SymbolGroup>();
        //                if (!this.IsExpanded)
        //                {
        //                    this.IsExpanded = true;
        //                }
        //                else
        //                {
        //                    if (parent != null && parent._mLeafGroup.Count >= 1)
        //                    {
        //                        for (int i = 0; i < parent._mLeafGroup.Count; i++)
        //                        {
        //                            if (this.IsExpanded)
        //                            {
        //                                if (parent._mLeafGroup.ToList()[i] != this)
        //                                {
        //                                    if (parent._mLeafGroup.ToList()[i].IsExpanded)
        //                                    {
        //                                        this.IsExpanded = false;
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                this.IsExpanded = true;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (_mStencil.ExpandMode == ExpandMode.ZeroOrOne)
        //            {
        //                SymbolGroup parent = this.FindVisualParent<SymbolGroup>();

        //                if (!this.IsExpanded)
        //                {
        //                    this.IsExpanded = true;
        //                    if (parent != null)
        //                    {
        //                        for (int i = 0; i < parent._mLeafGroup.Count; i++)
        //                        {
        //                            if (parent._mLeafGroup.ToList()[i] != this)
        //                            {
        //                                if (parent._mLeafGroup.ToList()[i].IsExpanded)
        //                                {
        //                                    parent._mLeafGroup.ToList()[i].IsExpanded = false;
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    this.IsExpanded = false;
        //                }
        //            }
        //            else if (_mStencil.ExpandMode == ExpandMode.ZeroOrMore)
        //            {
        //                if (this.IsExpanded)
        //                {
        //                    this.IsExpanded = false;
        //                }
        //                else
        //                {
        //                    this.IsExpanded = true;
        //                }
        //            }
        //        }
        //    }
        //    e.Handled = true;
        //}

        void SymbolGroup_Loaded(object sender, RoutedEventArgs e)
        {
            if (first)
            {
                if (_mStencil == null)
                {
                    _mStencil = this.FindVisualParent<Stencil>();
                }
                if (_mStencil.ExpandMode == ExpandMode.One)
                {
                    if (_mStencil._mExpandedGroup.Count == 1)
                    {
                        SymbolGroup parent = this.FindVisualParent<SymbolGroup>();
                        if (parent != null)
                        {
                            for (int i = 1; i < parent._mLeafGroup.Count; i++)
                            {
                                parent._mLeafGroup.ToList()[i].IsExpanded = false;
                            }
                            SymbolGroup g = _mStencil._mExpandedGroup.Peek();
                            for (int i = 0; i < g._mLeafGroup.Count; i++)
                            {
                                if (i == 0)
                                {
                                    g._mLeafGroup.ToList()[i].IsExpanded = true;
                                }
                                else
                                {
                                    g._mLeafGroup.ToList()[i].IsExpanded = false;
                                }
                            }
                        }


                    }
                }
                else
                {
                    if (this.Header != null)
                        this.IsExpanded = false;
                }
                first = false;

            }
        }


        #endregion

        protected override DependencyObject GetContainerForItemOverride()
        {
            if (ItemsSource is IEnumerable)
            {
                IEnumerable source = ItemsSource as IEnumerable;
                foreach (var temp in source)
                {
                    if (temp is GroupedSymbols)
                    {
                        return new SymbolGroup();
                    }
                    break;
                }
                return new Symbol();
            }
            return new Symbol();
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            _mStencil._mSymbolGroup.Clear();
            _mStencil._mExpandedGroup.Clear();
            _mLeafGroup.Clear();
            base.ClearContainerForItemOverride(element, item);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            GroupedSymbols itemSource = item as GroupedSymbols;
            SymbolGroup childGroup = element as SymbolGroup;
            Symbol symbol = element as Symbol;
            if (childGroup != null)
            {
                if (itemSource.SubGroups != null)
                {
                    childGroup.ItemsSource = itemSource.SubGroups;
                    if (itemSource.Header == null)
                    {
                        childGroup.Header = itemSource.Key;
                    }
                    else
                    {
                        childGroup.Header = itemSource.Header(itemSource.Key);
                    }
                }
                else
                {
                    childGroup.ItemsSource = itemSource.Items;
                    if (itemSource.Header == null)
                    {
                        childGroup.Header = itemSource.Key;
                    }
                    else
                    {
                        childGroup.Header = itemSource.Header(itemSource.Key);
                    }
                }

                SymbolGroup parent = childGroup.FindVisualParent<SymbolGroup>();
                if (parent != null)
                {
                    parent._mLeafGroup.Enqueue(childGroup);
                    _mStencil._mSymbolGroup.Enqueue(childGroup);
                }
                if (_mStencil.ExpandMode == ExpandMode.One)
                {
                    if (_mStencil._mSymbolGroup.Count == 1)
                    {
                        childGroup.IsExpanded = true;
                        _mStencil._mExpandedGroup.Enqueue(childGroup);
                    }
                    else
                    {
                        SymbolGroup current = _mStencil._mExpandedGroup.Peek();
                        if (current == parent)
                        {
                            if (current._mLeafGroup.Count == 1)
                            {
                                childGroup.IsExpanded = true;
                            }
                        }
                    }
                }

            }
            else if (symbol != null)
            {
                ISymbol symbolSource = item as ISymbol;
                symbol.DataContext = symbolSource;
                symbol.Content = symbolSource.Symbol;
                symbol.ContentTemplate = symbolSource.SymbolTemplate;
            }
        }
    }

    public class Header : ContentControl
    {
        public Header()
        {

        }
        internal void GoToState(bool useTransitions, string name)
        {
            VisualStateManager.GoToState(this, name, useTransitions);
        }
    }
}
