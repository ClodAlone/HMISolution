#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Linq;
using System.Windows;
using Syncfusion.PivotAnalysis.Base;
using System.Collections;
using System.ComponentModel;
using System.Data;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Controls.PivotSchemaDesigner
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial class PopupWindow : ChromelessWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupWindow"/> class.
        /// </summary>
        /// <param name="pivotControl">PivotControl</param>
        /// <param name="filterItemsCollection">Collection of filter items</param>
        public PopupWindow(IPivotControl pivotControl, FilterItemsCollection filterItemsCollection)
        {
            InitializeComponent();
            this.DataContext = this;
            this.IsFilterWindow = true;
            this.FilterList = filterItemsCollection;
            this.PivotControl = pivotControl;
            this.btnOK.DataContext = this.FilterList.AllFilterItem;
        }

        /// <summary>
        /// Gets or sets whether the pop up window is a filter window or not.
        /// </summary>
        public bool IsFilterWindow { get; set; }

        /// <summary>
        /// Gets whether (All) filter item is selected or not.
        /// </summary>
        public bool? IsAnyItemSelected 
        {
            get
            {
                return this.FilterList.AllFilterItem.IsSelected;
            }
        }

        /// <summary>
        /// Gets or sets the collection of filter items.
        /// </summary>
        public FilterItemsCollection FilterList { get; set; }

        /// <summary>
        /// Gets or sets the PivotGridControl
        /// </summary>
        public IPivotControl PivotControl { get; set; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.PivotControl != null)
            {
                this.FilterList.AcceptChanges();
                FilterExpression filterExpression = this.PivotControl.Filters.Where(x => x.Name == this.FilterList.Name).FirstOrDefault();
                if (filterExpression == null)
                {
                    if (PivotControl.ItemSource is DataView || PivotControl.ItemSource is DataTable)
                    {
                        this.PivotControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpressionForDataView(), Tag = this.FilterList });
                    }
                    else if (PivotControl.ItemSource is IEnumerable)
                    {
                        this.PivotControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpression(true), Tag=this.FilterList });
                    }
                    else if(PivotControl.ItemSource is IListSource)
                    {
                        this.PivotControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpression(false), Tag=this.FilterList });
                    }
                   
                }
                else
                {
                    if (PivotControl.ItemSource is DataView)
                    {
                        filterExpression.Expression = this.FilterList.GetFilterExpressionForDataView();
                    }
                    else if (PivotControl.ItemSource is IEnumerable)
                    {
                        filterExpression.Expression = this.FilterList.GetFilterExpression(true);
                    }
                    else if (PivotControl.ItemSource is IListSource)
                    {
                        filterExpression.Expression = this.FilterList.GetFilterExpression(false);
                    }
                    ////On FilterExpression change raise PivotSchemaDesigner changed event.
                    this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                }
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.FilterList.RejectChanges();
            this.Close();
        }
    }
}
