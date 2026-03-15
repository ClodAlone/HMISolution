#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
#if !WinRT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public abstract class ViewDefinition : DependencyObject
    {
        public string RelationalColumn
        {
            get { return (string) GetValue(RelationalColumnProperty); }
            set { SetValue(RelationalColumnProperty, value); }
        }

        public static readonly DependencyProperty RelationalColumnProperty =
            DependencyProperty.Register("RelationalColumn", typeof(string), typeof(ViewDefinition), new PropertyMetadata(default(string)));

    }

    public class GridViewDefinition : ViewDefinition
    {
        public GridViewDefinition()
        {
            DataGrid  = new SfDataGrid();
        }

        internal DetailsViewNotifyListener NotifyListener { get; set; }

        public static readonly DependencyProperty DataGridProperty =
            DependencyProperty.Register("DataGrid", typeof (SfDataGrid), typeof (GridViewDefinition), new PropertyMetadata(null));

        public SfDataGrid DataGrid
        {
            get { return (SfDataGrid) GetValue(DataGridProperty); }
            set { SetValue(DataGridProperty, value); }
        }
    }

    public class DetailsViewDefinition : ObservableCollection<ViewDefinition>
    {
        
    }
}
