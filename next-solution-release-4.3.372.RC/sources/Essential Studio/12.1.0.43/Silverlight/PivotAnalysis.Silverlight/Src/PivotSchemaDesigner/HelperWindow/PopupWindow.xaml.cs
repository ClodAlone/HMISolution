#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Linq;
using System.Windows;
using System.Collections;
using System.ComponentModel;
using Syncfusion.PivotAnalysis.Base.Silverlight;
using Syncfusion.Silverlight.Controls.PivotGrid;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Silverlight.Controls.PivotSchemaDesigner
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial class PopupWindow : WindowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupWindow"/> class.
        /// </summary>
        /// <param name="pivotControl">The pivot control.</param>
        /// <param name="filterItemsCollection">The filter items collection.</param>
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
        /// Gets or sets a value indicating whether this instance is filter window.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is filter window; otherwise, <c>false</c>.
        /// </value>
        public bool IsFilterWindow { get; set; }

        /// <summary>
        /// Gets the is any item selected.
        /// </summary>
        public bool? IsAnyItemSelected 
        {
            get
            {
                return this.FilterList.AllFilterItem.IsSelected;
            }
        }

        /// <summary>
        /// Gets or sets the filter list.
        /// </summary>
        /// <value>
        /// The filter list.
        /// </value>
        public FilterItemsCollection FilterList { get; set; }

        /// <summary>
        /// Gets or sets the pivot control.
        /// </summary>
        /// <value>
        /// The pivot control.
        /// </value>
        public IPivotControl PivotControl { get; set; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.PivotControl != null)
            {
                this.FilterList.AcceptChanges();
                FilterExpression filterExpression = this.PivotControl.Filters.Where(x => x.Name == this.FilterList.Name).FirstOrDefault();
                if (filterExpression == null)
                {
                    if (PivotControl.ItemSource is IEnumerable)
                    {
                        this.PivotControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpression(true), Tag=this.FilterList });
                    }
                }
                else
                {
                    if (PivotControl.ItemSource is IEnumerable)
                    {
                        filterExpression.Expression = this.FilterList.GetFilterExpression(true);
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
