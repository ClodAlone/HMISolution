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
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using Syncfusion.PivotAnalysis.Base;
using System.Collections;
using System.ComponentModel;
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
using System.Data;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    ///  This class represents a Filter Popup which is used to Filter Pivot Elements
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FilterPopup : Popup
    {
        #region [ Private Members ]

        private Button m_ButtonApply;
        private Button m_ButtonCancel;
        private Button button;
        private Dictionary<string, bool> _checkFiltered;
        private FilterItemsCollection m_FilterItemsCollection;

        #endregion

        #region [ Initialize/Fianlize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterPopup"/> class.
        /// </summary>
        public FilterPopup()
        {
            this.DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterPopup"/> class.
        /// </summary>
        /// <param name="GridControl">The grid control.</param>
        /// <param name="filterItemsCollection">The filter list.</param>
        public FilterPopup(PivotGridControl GridControl, FilterItemsCollection filterItemsCollection)
        {
            this.DataContext = this;
            this.IsFilterWindow = true;
            this.FilterList = filterItemsCollection;
            this.GridControl = GridControl;
        }

        #endregion

        #region [ Public Properties ]

        /// <summary>
        /// Gets or sets the Apply Button.
        /// </summary>
        public Button ButtonApply
        {
            get
            {
                return m_ButtonApply;
            }
            set
            {
                m_ButtonApply = value;
                WireButtonApplyEvent();
            }
        }

        /// <summary>
        /// Gets or sets the Cancel Button.
        /// </summary>
        public Button ButtonCancel
        {
            get
            {
                return m_ButtonCancel;
            }
            set
            {
                m_ButtonCancel = value;
                WireButtonCancelEvent();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter window.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is filter window; otherwise, <c>false</c>.
        /// </value>
        public bool IsFilterWindow { get; set; }

        /// <summary>
        /// Gets a value indicating whether any item selected in the filterlist
        /// </summary>
        /// <value>
        /// 	<c>true</c> if any item selected; otherwise, <c>false</c>.
        /// </value>
        public bool? IsAnyItemSelected
        {
            get
            {
                if (this.FilterList != null && this.FilterList.AllFilterItem != null)
                {
                    return this.FilterList.AllFilterItem.IsSelected;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the filter list box.
        /// </summary>
        /// <value>The filter list box.</value>
        public ListBox FilterListBox { get; set; }

        internal FilterItemsCollection FilterListForDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter list.
        /// </summary>
        /// <value>The filter list.</value>
        public FilterItemsCollection FilterList
        {
            get
            {
                return m_FilterItemsCollection;
            }
            set
            {
                m_FilterItemsCollection = value;
                this.FilterListBox.ItemsSource = m_FilterItemsCollection;
                this.FilterList.AllFilterItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(AllFilterItem_PropertyChanged);
                if (m_FilterItemsCollection.AllFilterItem.IsSelected == true)
                {
                    this.ButtonApply.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the grid control.
        /// </summary>
        /// <value>The grid control.</value>
        public PivotGridControl GridControl { get; set; }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Wires the apply button event.
        /// </summary>
        private void WireButtonApplyEvent()
        {
            this.ButtonApply.Click += new RoutedEventHandler(btnOK_Click);
        }

        /// <summary>
        /// Wires the button cancel event.
        /// </summary>
        private void WireButtonCancelEvent()
        {
            this.ButtonCancel.Click += new RoutedEventHandler(btnCancel_Click);
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            this.FilterList.AcceptChanges();
            FilterExpression filterExpression = this.GridControl.Filters.Where(x => x.DimensionName == this.FilterList.Name).FirstOrDefault();
            FilterExpression filterExpressionExp = this.GridControl.Filters.Where(x => x.Name == this.FilterList.Name).FirstOrDefault();
            if (this.GridControl != null && !this.GridControl.GroupingBar.Filters.Any(x => x.Name == FilterList.Name) && this.GridControl.PivotEngine != null && this.GridControl.PivotEngine.EnableOnDemandCalculations 
                && !this.GridControl.PivotEngine.UseIndexedEngine)
            {
                this.GridControl.Cursor = System.Windows.Input.Cursors.Wait;

                this.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        if (filterExpression == null && filterExpressionExp == null)
                        {
                            this.GridControl.UpdateGridLayout = true;
                            if (this.GridControl.ItemSource is DataView || this.GridControl.ItemSource is DataTable)
                            {
                                this.GridControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Tag = this.FilterList });
                            }
                            else if (this.GridControl.ItemSource is IEnumerable)
                            {
                                this.GridControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Tag = this.FilterList });
                            }
                            else if (this.GridControl.ItemSource is IListSource)
                            {
                                this.GridControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Tag = this.FilterList });
                            }
                        }
                        this.GridControl.InternalGrid.ApplyGroupingBarFilters(FilterList, true);
                        this.GridControl.Cursor = null;
                    }), System.Windows.Threading.DispatcherPriority.SystemIdle);
            }
            else if (this.GridControl != null && ((this.FilterList as FilterItemsCollection)[0].IsSelected == true && (this.GridControl.Filters.Any(x => x.DimensionName == this.FilterList.Name) || this.GridControl.Filters.Any(x => x.Name == this.FilterList.Name))) || ((this.FilterList as FilterItemsCollection)[0].IsSelected != true))
            {
                if (filterExpression == null && filterExpressionExp == null)
                {
                    this.GridControl.UpdateGridLayout = true;
                    if (this.GridControl.ItemSource is DataView || this.GridControl.ItemSource is DataTable)
                    {
                        this.GridControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpressionForDataView(), Tag = this.FilterList });
                    }
                    else if (this.GridControl.ItemSource is IEnumerable)
                    {
                        if (this.FilterListForDateTime != null && this.FilterListForDateTime.Count > 0 && this.FilterList.FilterProperty.PropertyType == typeof(DateTime))
                        {
                            this.GridControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpression(true,this.FilterListForDateTime,this.FilterListForDateTime.Format), Format =this.FilterListForDateTime.Format,  Tag = this.FilterList });
                        }
                        else
                            this.GridControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpression(true), Tag = this.FilterList });
                    }
                    else if (this.GridControl.ItemSource is IListSource)
                    {
                        this.GridControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpression(false), Tag = this.FilterList });
                    }
                }
                else
                {
                    this.GridControl.UpdateGridLayout = true;
                    if (this.GridControl.ItemSource is DataView || this.GridControl.ItemSource is DataTable)
                    {
                        if (filterExpression != null)
                        {
                            filterExpression.Expression = this.FilterList.GetFilterExpressionForDataView();
                        }
                        else if( filterExpressionExp!=null)
                            filterExpressionExp.Expression=this.FilterList.GetFilterExpressionForDataView();

                    }
                    else if (this.GridControl.ItemSource is IEnumerable)
                    {
                        if (filterExpression != null)
                        {
                            if (this.FilterListForDateTime != null && this.FilterListForDateTime.Count > 0 && this.FilterList.FilterProperty.PropertyType == typeof(DateTime))
                            {
                                filterExpression.Expression = this.FilterList.GetFilterExpression(true, this.FilterListForDateTime, this.FilterListForDateTime.Format);
                            }
                            else
                                filterExpression.Expression = this.FilterList.GetFilterExpression(true);
                        }
                        else if (filterExpressionExp != null)
                        {
                            if (this.FilterListForDateTime != null && this.FilterListForDateTime.Count > 0 && this.FilterList.FilterProperty.PropertyType == typeof(DateTime))
                            {
                                filterExpressionExp.Expression = this.FilterList.GetFilterExpression(true, this.FilterListForDateTime, this.FilterListForDateTime.Format);
                            }
                            else
                                filterExpressionExp.Expression = this.FilterList.GetFilterExpression(true);
                        }
                    
                    }
                    else if (this.GridControl.ItemSource is IListSource)
                    {
                        if (filterExpression != null)
                        {
                            filterExpression.Expression = this.FilterList.GetFilterExpression(false);
                        }
                        else if(filterExpressionExp!=null)
                            filterExpressionExp.Expression = this.FilterList.GetFilterExpression(false);
                    }
                    ////On FilterExpression change raise PivotSchemaDesigner changed event.
                    this.GridControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                }
            }
            if (_checkFiltered == null)
            {
                _checkFiltered = new Dictionary<string, bool>();
            }
            if (this.FilterList.AllFilterItem.IsSelected == null)
            {
                if (this.button != null)
                {
                    Image img = (Image)button.Template.FindName("Images", button);
                    img.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Syncfusion.PivotAnalysis.WPF;component/PivotGridControl/Resources/Filtered.png", UriKind.RelativeOrAbsolute));
                    if (!_checkFiltered.Keys.Contains(this.FilterList.Name))
                    {
                        _checkFiltered.Add(this.FilterList.Name, true);
                    }
                    ImageConverter imgconv = new ImageConverter();
                    imgconv.GetDictionary(_checkFiltered);

                }
            }
            else
            {
                Image img = (Image)button.Template.FindName("Images", button);
                img.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Syncfusion.PivotAnalysis.WPF;component/PivotGridControl/Resources/Filter.png", UriKind.RelativeOrAbsolute));
                if (_checkFiltered.Keys.Contains(this.FilterList.Name))
                {
                    _checkFiltered.Remove(this.FilterList.Name);
                }
                ImageConverter imgconv = new ImageConverter();
                imgconv.GetDictionary(_checkFiltered);
            }
            this.GridControl.InvalidateCells();
        }
        /// <summary>
        /// Gives the instance of Button
        /// </summary>
        /// <param name="button">The button</param>
        public void GetButton(Button button)
        {
            this.button = button;
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.FilterList.RejectChanges();
            this.IsOpen = false;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the AllFilterItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void AllFilterItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FilterItemElement element = sender as FilterItemElement;
            if (element.IsSelected != null)
            {
                if (element.IsSelected.Value == false)
                {
                    this.ButtonApply.IsEnabled = false;
                }
                else
                    this.ButtonApply.IsEnabled = true;
            }
            else
            {
                this.ButtonApply.IsEnabled = true;
            }
        }

        #endregion
    }
}
