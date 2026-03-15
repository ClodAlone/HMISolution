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
#if WinRT
using Windows.UI;
using Windows.UI.Xaml;
using System.Threading.Tasks;
#endif

using System.Collections.ObjectModel;
using System.Windows;

namespace Syncfusion.UI.Xaml.Grid
{
    public class StackedHeaderRow : DependencyObject
    {
        public StackedHeaderRow()
        {
            SetValue(StackedColumnsProperty, new StackedColumns());
        }
        
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(StackedHeaderRow), new PropertyMetadata(null));




        public StackedColumns StackedColumns
        {
            get { return (StackedColumns)GetValue(StackedColumnsProperty); }
        }

        // Using a DependencyProperty as the backing store for StackedColumns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StackedColumnsProperty =
            DependencyProperty.Register("StackedColumns", typeof(StackedColumns), typeof(StackedHeaderRow), new PropertyMetadata(new StackedColumns()));

    }

    internal delegate void ChildColumnsChanged();

    public class StackedColumn : DependencyObject
    {
        internal ChildColumnsChanged ChildColumnChanged;
        internal List<int> ChildColumnsIndex { get; set; }

        public string ChildColumns
        {
            get { return (string)GetValue(ChildColumnsProperty); }
            set { SetValue(ChildColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildColumns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildColumnsProperty =
            DependencyProperty.Register("ChildColumns", typeof(string), typeof(StackedColumn), new PropertyMetadata(string.Empty,OnChildColumnsChanged));

        private static void OnChildColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as StackedColumn;
            if (column.ChildColumnChanged != null)
                column.ChildColumnChanged();
        }

        

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(StackedColumn), new PropertyMetadata(null));

    }

    public class StackedHeaderRows : ObservableCollection<StackedHeaderRow>
    {
        public StackedHeaderRows()
        {

        }
    }

    public class StackedColumns : ObservableCollection<StackedColumn>
    {
        public StackedColumns()
        {
              
        }
    }
}
