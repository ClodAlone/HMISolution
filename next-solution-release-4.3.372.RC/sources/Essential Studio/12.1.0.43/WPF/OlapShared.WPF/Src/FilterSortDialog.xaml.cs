//-------------------------------------------------------------------------------------------------
// <copyright file="FilterSortDialog.xaml.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Reports;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Interaction logic for FilterSortDialog.xaml
    /// </summary>
    public partial class FilterSortDialog : ChromelessWindow
    {
        #region Private Variables
        OlapDataManager _cubeModel;

        /// <summary>
        /// Column string which should be appended while loading the Window
        /// </summary>
        string columnString = new Syncfusion.Windows.Shared.Olap.Resources.ResourceWrapper().OlapToolsFilterSortingDlgOnColumn;// " On Columns";

        /// <summary>
        /// Row string which should be appended while loading the Window
        /// </summary>
        string rowString = new Syncfusion.Windows.Shared.Olap.Resources.ResourceWrapper().OlapToolsFilterSortingDlgOnRow;// " On Rows";

        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSortDialog"/> class.
        /// </summary>
        /// <param name="cubeModel">The cube model which is used to default combos</param>
        /// <param name="axis">The Axis position of the filter elements</param>
        /// <param name="isFilter">bool value which is used to set the categorial or series element
        /// property</param>
        /// <param name="showAllMeasures">bool value which is used to set whehter show all measures or current report measures
        /// property</param>
        public FilterSortDialog(IOlapDataManager cubeModel, AxisPosition axis, bool isFilter, bool showAllMeasures, string visualStyle)
        {
            InitializeComponent();
            InitalizeDialog(cubeModel, axis, isFilter);
            this.ShowAllMeasuresInFilterSortDlg = showAllMeasures;
            this.VisualStyle = visualStyle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSortDialog"/> class.
        /// </summary>
        /// <param name="cubeModel">The cube model which is used to default combos</param>
        /// <param name="axis">The Axis position of the filter elements</param>
        /// <param name="isFilter">bool value which is used to set the categorial or series element
        /// property</param>
        public FilterSortDialog(IOlapDataManager cubeModel, AxisPosition axis, bool isFilter)
        {
            InitializeComponent();
            InitalizeDialog(cubeModel, axis, isFilter);
            this.ShowAllMeasuresInFilterSortDlg = true;
        }

        private void InitalizeDialog(IOlapDataManager cubeModel, AxisPosition axis, bool isFilter)
        {
            this.Axis = axis;
            if (this.Axis == AxisPosition.Series)
            {
                this.Title = this.Title + rowString;
            }
            else if (this.Axis == AxisPosition.Categorical)
            {
                this.Title = this.Title + columnString;
            }

            this.ResizeMode = ResizeMode.NoResize;
            this.Width = 400;
            this.Height = 400;
            LoadFilterCaseEnum(conditionBoxOne);
            LoadFilterCaseEnum(conditionBoxTwo);
            this._cubeModel = (OlapDataManager)cubeModel;
            if (this.Axis == AxisPosition.Categorical)
            {
                this.filterEmptyColumnCheckBox.Visibility = Visibility.Visible;
                this.filterEmptyRowCheckBox.Visibility = Visibility.Hidden;
            }
            else if (this.Axis == AxisPosition.Series)
            {
                this.filterEmptyColumnCheckBox.Visibility = Visibility.Hidden;
                this.filterEmptyRowCheckBox.Visibility = Visibility.Visible;
            }
            else if (this.Axis == AxisPosition.Categorical && this.Axis == AxisPosition.Series)
            {
                this.filterEmptyColumnCheckBox.Visibility = Visibility.Visible;
                this.filterEmptyRowCheckBox.Visibility = Visibility.Visible;
            }

            this.filterTwoGroup.IsEnabled = false;
            this.DisableFilterOneCombos();
            this.DisableFilterTwoCombos();
            this.DisableSortControls();
            this.IsFilter = isFilter;
            this.Loaded += new RoutedEventHandler(FilterSortDialog_Loaded);
            this.tabControl.ContextMenuOpening += new ContextMenuEventHandler(tabControl_ContextMenuOpening);
        }

        #endregion

        #region Property Declaration
        /// <summary>
        /// Gets or sets the Height of the Textbox for Metro theme to display the value properly
        /// </summary>
        public string VisualStyle { get; set; }

        /// <summary>
        /// Gets or sets whether to show all the measures or only show the current report measures.
        /// </summary>
        public bool ShowAllMeasuresInFilterSortDlg { get; set; }
        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        /// <value>Holds the axis position of the Filter Elements</value>
        public AxisPosition Axis { get; set; }

        /// <summary>
        /// Gets or sets the cube model.
        /// </summary>
        /// <value>Holds the OlapDataManager</value>
        public OlapDataManager OlapDataManager
        {
            get
            {
                return _cubeModel;
            }

            set
            {
                if (_cubeModel != value)
                {
                    _cubeModel = (OlapDataManager)value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter.
        /// </summary>
        /// <value>This property is used to set the cubemodel items property</value>
        public bool IsFilter { get; set; }

        /// <summary>
        /// Gets the Metatreenodes property from the parent Window
        /// </summary>
        public static readonly DependencyProperty MetaTreeNodesProperty =
DependencyProperty.Register("MetaTreeNodes", typeof(MetaTreeNodeCollection), typeof(FilterSortDialog), new UIPropertyMetadata(new MetaTreeNodeCollection(null), FilterSortDialog.OnMetaTreeNodesChanged));

        /// <summary>
        /// Gets or sets the meta tree nodes.
        /// </summary>
        /// <value>Holds the metatreenodes value of type Measures</value>
        public MetaTreeNodeCollection MetaTreeNodes
        {
            get { return (MetaTreeNodeCollection)GetValue(MetaTreeNodesProperty); }
            set { SetValue(MetaTreeNodesProperty, value); }
        }
        #endregion

        #region Diabling Controls
        private void DisableFilterOneCombos()
        {
            this.measuresBoxOne.IsEnabled = false;
            this.measuresTextOne.IsEnabled = false;
            this.conditionBoxOne.IsEnabled = false;
            this.conditionOneText.IsEnabled = false;
            this.valueTextBoxOne.IsEnabled = false;
            this.valueTextOne.IsEnabled = false;
        }

        private void DisableFilterTwoCombos()
        {
            this.measuresBoxTwo.IsEnabled = false;
            this.measuresTextTwo.IsEnabled = false;
            this.conditionBoxTwo.IsEnabled = false;
            this.conditionTwoText.IsEnabled = false;
            this.valueTextBoxTwo.IsEnabled = false;
            this.valueTextTwo.IsEnabled = false;
        }

        private void DisableSortControls()
        {
            this.sortingHierarchy.IsEnabled = false;
            this.sortingMeasuresBox.IsEnabled = false;
            this.sortingTypeAsc.IsEnabled = false;
            this.sortingTypeDesc.IsEnabled = false;
        }
        #endregion

        #region Enabling Controls
        private void EnableFilterOneCombos()
        {
            this.measuresBoxOne.IsEnabled = true;
            this.measuresTextOne.IsEnabled = true;
            this.conditionBoxOne.IsEnabled = true;
            this.conditionOneText.IsEnabled = true;
            this.valueTextBoxOne.IsEnabled = true;
            this.valueTextOne.IsEnabled = true;
            if (this.VisualStyle == "Default" || this.VisualStyle == "Metro")
                this.valueTextBoxOne.Height = 24;
        }

        private void EnableFilterTwoCombos()
        {
            this.measuresBoxTwo.IsEnabled = true;
            this.measuresTextTwo.IsEnabled = true;
            this.conditionBoxTwo.IsEnabled = true;
            this.conditionTwoText.IsEnabled = true;
            this.valueTextBoxTwo.IsEnabled = true;
            this.valueTextTwo.IsEnabled = true;
            if (this.VisualStyle == "Default" || this.VisualStyle == "Metro")
                this.valueTextBoxTwo.Height = 24;
        }

        private void EnableSortControls()
        {
            this.sortingHierarchy.IsEnabled = true;
            this.sortingMeasuresBox.IsEnabled = true;
            this.sortingTypeAsc.IsEnabled = true;
            this.sortingTypeDesc.IsEnabled = true;
        }
        #endregion

        #region defaulting combos from OlapDataManager
        /// <summary>
        /// Loads the values from cube model.
        /// </summary>
        public void LoadValuesFromOlapDataManager()
        {
            filterEmptyColumnCheckBox.IsChecked = !this.OlapDataManager.CurrentReport.ShowEmptyColumnData;
            filterEmptyRowCheckBox.IsChecked = !this.OlapDataManager.CurrentReport.ShowEmptyRowData;
            var var_FilterElements = this.OlapDataManager.CurrentReport.FilterElements.List.Where(i => i.Axis == this.Axis).Select(i => i);
            if (var_FilterElements.Count() > 1)
            {
                filterOneGroupCheckBox.IsChecked = true;
                EnableFilterOneCombos();
                filterTwoGroupCheckBox.IsEnabled = true;
                filterTwoGroupCheckBox.IsChecked = true;
                EnableFilterTwoCombos();
                LoadFilterElements(true, var_FilterElements);
            }
            else if (var_FilterElements.Count() > 0)
            {
                filterOneGroupCheckBox.IsChecked = true;
                EnableFilterOneCombos();
                filterTwoGroupCheckBox.IsEnabled = true;
                LoadFilterElements(false, var_FilterElements);
            }

            PopulateSortElements();
        }

        #region Populate Filter and Sort Values from OlapDataManager
        private void LoadFilterElements(bool isFilterTwo, IEnumerable<Item> var_FilterElements)
        {
            int itemcount = 0;
            foreach (var item in var_FilterElements)
            {
                FilterElement filterElement1 = new FilterElement();
                MeasureElement measureElement = new MeasureElement();
                FilterCase filterCase = new FilterCase();
                FilterValue filterValue = new FilterValue();
                if (itemcount == 0)
                {
                    filterElement1 = (FilterElement)item.ElementValue;
                    measureElement = (MeasureElement)filterElement1.FilterValue[0];
                    filterCase = filterElement1.FilterCase;
                    filterValue = (FilterValue)filterElement1.FilterValue[1];
                    PopulateMeasuresCombo(measuresBoxOne, measureElement.UniqueName);
                    PopulateCombo(conditionBoxOne, filterCase.ToString());
                    valueTextBoxOne.Text = filterValue.Filter_Value.ToString();
                }
                else
                {
                    if (isFilterTwo)
                    {
                        filterElement1 = (FilterElement)item.ElementValue;
                        measureElement = (MeasureElement)filterElement1.FilterValue[0];
                        filterCase = filterElement1.FilterCase;
                        filterValue = (FilterValue)filterElement1.FilterValue[1];
                        PopulateMeasuresCombo(measuresBoxTwo, measureElement.UniqueName);
                        PopulateCombo(conditionBoxTwo, filterCase.ToString());
                        valueTextBoxTwo.Text = filterValue.Filter_Value.ToString();
                    }
                }

                itemcount++;
            }
        }

        private void PopulateCombo(ComboBox measuresBox, string uniquename)
        {
            measuresBox.SelectedItem = uniquename;
        }

        private void PopulateMeasuresCombo(ComboBox measuresBox, string uniquename)
        {
            for (int i = 0; i < this.MetaTreeNodes.Count; i++)
            {
                MetaTreeNode mtNode = this.MetaTreeNodes[i];
                if (mtNode.UniqueName == uniquename)
                {
                    measuresBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void PopulateSortElements()
        {
            Items items;
            if (Axis == AxisPosition.Series)
            {
                items = this.OlapDataManager.CurrentReport.SeriesElements;
            }
            else if (Axis == AxisPosition.Categorical)
            {
                items = this.OlapDataManager.CurrentReport.CategoricalElements;
            }
            else
            {
                items = null;
            }

            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item.ElementValue is SortElement)
                {
                    sortingGroupCheckBox.IsChecked = true;
                    EnableSortControls();
                    SortElement sortElement = (SortElement)item.ElementValue;
                    PopulateMeasuresCombo(sortingMeasuresBox, sortElement.Element.UniqueName);
                    switch (sortElement.SortOrder)
                    {
                        case SortOrder.ASC:
                            {
                                sortingTypeAsc.IsChecked = true;
                                sortingHierarchy.IsChecked = true;
                                break;
                            }

                        case SortOrder.DESC:
                            {
                                sortingTypeDesc.IsChecked = true;
                                sortingHierarchy.IsChecked = true;
                                break;
                            }

                        case SortOrder.BASC:
                            {
                                sortingTypeAsc.IsChecked = true;
                                sortingHierarchy.IsChecked = false;
                                break;
                            }

                        case SortOrder.BDESC:
                            {
                                sortingTypeDesc.IsChecked = true;
                                sortingHierarchy.IsChecked = false;
                                break;
                            }

                        default: break;
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Common Methods
        /// <summary>
        /// Determines whether the specified string is double.
        /// </summary>
        /// <param name="s">string to be parsed</param>
        /// <returns>
        /// <c>true</c> if the specified s is double; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDouble(string s)
        {
            try
            {
                double.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Load Filter Case Combo Values
        /// <summary>
        /// Loads the filter case enum.
        /// </summary>
        /// <param name="comboBox">The combo box.</param>
        public void LoadFilterCaseEnum(ComboBox comboBox)
        {
            string[] filterCaseEnum = Enum.GetNames(typeof(FilterCase));
            foreach (string filterCase in filterCaseEnum)
            {
                comboBox.Items.Add(filterCase);
            }
        }
        #endregion

        #region Populating Measures Element to the Tree Node
        private void FilterSortDialog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CubeSchema cubeSchema = OlapDataManager.DataProvider.GetCubeSchema(OlapDataManager.CurrentCubeName);
                MetaTreeNode mtNode = new MetaTreeNode();
                if (ShowAllMeasuresInFilterSortDlg)
                {
                    MetaTreeHelper.FillMetaTreeNode(mtNode, cubeSchema.Measures, false, true);
                }
                else
                {
                    MeasureCollection reportMeasures = new MeasureCollection();
                    foreach (var categItem in  this.OlapDataManager.CurrentReport.CategoricalElements)
                    {
                        if (categItem != null && categItem.ElementValue is MeasureElements)
                        {
                            MeasureElements measureElements = categItem.ElementValue as MeasureElements;
                            foreach (var measureElement in measureElements.Elements)
                            {
                                if (measureElement is MeasureElement)
                                {
                                    MeasureElement element = measureElement as MeasureElement;

                                    foreach (var item in cubeSchema.Measures)
                                    {
                                        if (element.Name.ToUpper() == item.Name.ToUpper() || element.UniqueName.ToUpper() == item.UniqueName.ToUpper())
                                        {
                                            reportMeasures.Add(item);
                                        }
                                    }
                                }
                            }
                            break;
                        }

                    }
                    foreach (var serisItem in this.OlapDataManager.CurrentReport.SeriesElements)
                    {
                        if (serisItem != null && serisItem.ElementValue is MeasureElements)
                        {
                            MeasureElements measureElements = serisItem.ElementValue as MeasureElements;
                            foreach (var measureElement in measureElements.Elements)
                            {
                                if (measureElement is MeasureElement)
                                {
                                    MeasureElement element = measureElement as MeasureElement;

                                    foreach (var item in cubeSchema.Measures)
                                    {
                                        if (element.Name.ToUpper() == item.Name.ToUpper() || element.UniqueName.ToUpper() == item.UniqueName.ToUpper())
                                        {
                                            reportMeasures.Add(item);
                                        }
                                    }
                                }
                            }
                            break;
                        }

                    }
                    foreach (var slicerItem in this.OlapDataManager.CurrentReport.SlicerElements)
                    {
                        if (slicerItem != null && slicerItem.ElementValue is MeasureElements)
                        {
                            MeasureElements measureElements = slicerItem.ElementValue as MeasureElements;
                            foreach (var measureElement in measureElements.Elements)
                            {
                                if (measureElement is MeasureElement)
                                {
                                    MeasureElement element = measureElement as MeasureElement;

                                    foreach (var item in cubeSchema.Measures)
                                    {
                                        if (element.Name.ToUpper() == item.Name.ToUpper() || element.UniqueName.ToUpper() == item.UniqueName.ToUpper())
                                        {
                                            reportMeasures.Add(item);
                                        }
                                    }
                                }
                            }
                            break;
                        }

                    }

                    MetaTreeHelper.FillMetaTreeNode(mtNode, reportMeasures, false, true);
                }
                MetaTreeNodes = mtNode.ChildNodes;
                LoadValuesFromOlapDataManager();

                if (!this.IsFilter)
                {
                    this.tabItemSortingInfo.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Preserving Values while closing the Filterdialog
        private void filterSortDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            FilterSortDialog filterWindow = sender as FilterSortDialog;
            if (filterWindow != null)
            {
                filterWindow.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Binding Measure elements to all Combos
        /// <summary>
        /// Called when [meta tree node value is changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnMetaTreeNodesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FilterSortDialog filterSortDialog = dependencyObject as FilterSortDialog;
            if (filterSortDialog != null)
            {
                filterSortDialog.measuresBoxOne.ItemsSource = filterSortDialog.MetaTreeNodes;
                filterSortDialog.measuresBoxTwo.ItemsSource = filterSortDialog.MetaTreeNodes;
                filterSortDialog.sortingMeasuresBox.ItemsSource = filterSortDialog.MetaTreeNodes;
            }
        }
        #endregion

        #region updating the Cube Model
        /// <summary>
        /// Updates the cube model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void updateOlapDataManager(object sender, RoutedEventArgs e)
        {
            this.OlapDataManager.CurrentReport.ShowEmptyColumnData = !(bool)filterEmptyColumnCheckBox.IsChecked;
            this.OlapDataManager.CurrentReport.ShowEmptyRowData = !(bool)filterEmptyRowCheckBox.IsChecked;

            if (filterOneGroupCheckBox.IsChecked == true)
            {
                if (!ValidateFilterCondition(measuresBoxOne, conditionBoxOne, valueTextBoxOne))
                {
                    return;
                }
            }

            if (filterTwoGroupCheckBox.IsChecked == true)
            {
                if (!ValidateFilterCondition(measuresBoxTwo, conditionBoxTwo, valueTextBoxTwo))
                {
                    return;
                }
            }

            if (sortingGroupCheckBox.IsChecked == true)
            {
                if (!ValidateSortCondition(sortingMeasuresBox))
                {
                    return;
                }
            }

            if (filterOneGroupCheckBox.IsChecked == true && validFilter(measuresBoxOne, conditionBoxOne, valueTextBoxOne) && filterTwoGroupCheckBox.IsChecked == true && validFilter(measuresBoxTwo, conditionBoxTwo, valueTextBoxTwo))
            {
                this.OlapDataManager.CurrentReport.FilterElements.RemoveAll(this.Axis);
                List<FilterElement> filterElement = new List<FilterElement>();
                FilterElement dummyFilterElement = new FilterElement(this.Axis);
                AddFilterElements(measuresBoxOne, conditionBoxOne, valueTextBoxOne, filterElement);
                FilterCase filterCase = (FilterCase)Enum.Parse(typeof(FilterCase), conditionBoxTwo.SelectedItem.ToString());
                dummyFilterElement.FilterCase = filterCase;
                dummyFilterElement.IsFilterCondition = filterTwoGroup.IsEnabled;
                dummyFilterElement.Visible = true;
                filterElement.Add(dummyFilterElement);
                AddFilterValues(measuresBoxTwo, conditionBoxTwo, valueTextBoxTwo, filterElement[1]);
                updateOlapDataManager(filterElement[0]);
                updateOlapDataManager(filterElement[1]);
            }
            else if (filterOneGroupCheckBox.IsChecked == true && validFilter(measuresBoxOne, conditionBoxOne, valueTextBoxOne))
            {
                this.OlapDataManager.CurrentReport.FilterElements.RemoveAll(this.Axis);
                List<FilterElement> filterElement = new List<FilterElement>();
                AddFilterElements(measuresBoxOne, conditionBoxOne, valueTextBoxOne, filterElement);
                updateOlapDataManager(filterElement[0]);
            }
            else if (!(bool)filterOneGroupCheckBox.IsChecked)
            {
                this.OlapDataManager.CurrentReport.FilterElements.RemoveAll(this.Axis);
                //// If filter is set to series elements then adding the filter elements to the
                //// series collection and clearing the values from the fiter elements
                if (Axis == AxisPosition.Series)
                {
                    this.OlapDataManager.CurrentReport.SeriesElements.IsFilterOrSortOn = false;
                }
                else if (Axis == AxisPosition.Categorical)
                {
                    this.OlapDataManager.CurrentReport.CategoricalElements.IsFilterOrSortOn = false;
                }
            }
            else if (!(bool)filterTwoGroupCheckBox.IsChecked && (bool)filterOneGroupCheckBox.IsChecked)
            {
                var var_FilterElements = this.OlapDataManager.CurrentReport.FilterElements.List.Where(i => i.Axis == this.Axis).Select(i => i);
                int itemindex = 0;
                foreach (var item in var_FilterElements)
                {
                    itemindex++;
                }

                ////Removing last item from the collection
                this.OlapDataManager.CurrentReport.FilterElements.RemoveAt(itemindex);
            }

            /*            if (!(bool)sortingHierarchy.IsChecked)
                        {
                            this.OlapDataManager.CurrentReport.ShowGrandTotal = true;
                        }
                        else
                        {
                            this.OlapDataManager.CurrentReport.ShowGrandTotal = false;
                        }*/

            if (sortingGroupCheckBox.IsChecked == true)
            {
                bool hierarchy;
                SortOrder sortOrder = new SortOrder();

                hierarchy = (bool)sortingHierarchy.IsChecked;
                MetaTreeNode metaTreeNode = sortingMeasuresBox.SelectedItem as MetaTreeNode;
                if ((bool)sortingTypeAsc.IsChecked)
                {
                    if (hierarchy)
                    {
                        sortOrder = SortOrder.ASC;
                    }
                    else
                    {
                        sortOrder = SortOrder.BASC;
                    }
                }

                else if ((bool)sortingTypeDesc.IsChecked)
                {
                    if (hierarchy)
                    {
                        sortOrder = SortOrder.DESC;
                    }
                    else
                    {
                        sortOrder = SortOrder.BDESC;
                    }
                }

                SortElement sortElement = new SortElement(Axis, sortOrder, (bool)sortingGroupCheckBox.IsChecked);
                sortElement.Element.UniqueName = metaTreeNode.UniqueName;
                if (Axis == AxisPosition.Series)
                {
                    int rowValue = -1;
                    ////finding the index of sort elements based on Axis Type.
                    rowValue = this.CheckSortElements(this.OlapDataManager.CurrentReport.SeriesElements);

                    ////removing sort element from the tuple collection of the axis
                    ////to avoid duplicates in the collection
                    if (rowValue > 0)
                    {
                        this.OlapDataManager.CurrentReport.SeriesElements.RemoveAt(rowValue);
                    }

                    this.OlapDataManager.CurrentReport.SeriesElements.Add(new Item { ElementValue = sortElement });
                }
                else if (Axis == AxisPosition.Categorical)
                {
                    int columnValue = -1;
                    columnValue = this.CheckSortElements(this.OlapDataManager.CurrentReport.CategoricalElements);
                    if (columnValue > 0)
                    {
                        this.OlapDataManager.CurrentReport.CategoricalElements.RemoveAt(columnValue);
                    }

                    this.OlapDataManager.CurrentReport.CategoricalElements.Add(new Item { ElementValue = sortElement });
                }
                if (this.OlapDataManager.CurrentReport.ShowExpanders)
                    this.OlapDataManager.CurrentReport.ShowExpanders = true;
            }
            else
            {
                int rowValue = -1;
                int columnValue = -1;
                ////finding index of sort elements based on Axis Type.
                if (this.Axis == AxisPosition.Series)
                {
                    rowValue = this.CheckSortElements(this.OlapDataManager.CurrentReport.SeriesElements);
                }
                else if (this.Axis == AxisPosition.Categorical)
                {
                    columnValue = this.CheckSortElements(this.OlapDataManager.CurrentReport.CategoricalElements);
                }

                ////removing sort elements from the tuple collection
                if (rowValue > 0)
                {
                    this.OlapDataManager.CurrentReport.SeriesElements.RemoveAt(rowValue);
                }

                if (columnValue > 0)
                {
                    this.OlapDataManager.CurrentReport.CategoricalElements.RemoveAt(columnValue);
                }

                ////checking for if any sort elements exists in either of the axises.
                if (this.CheckSortElements(this.OlapDataManager.CurrentReport.SeriesElements) > 0 || this.CheckSortElements(this.OlapDataManager.CurrentReport.CategoricalElements) > 0
                    || this.CheckSortElements(this.OlapDataManager.CurrentReport.FilterElements) >= 0)
                {
                    this.OlapDataManager.CurrentReport.ShowExpanders = false;
                }
                else
                {
                    this.OlapDataManager.CurrentReport.ShowExpanders = true;
                }
            }

            try
            {
                this.OlapDataManager.NotifyElementModified();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading Data");
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// Checks and returns the Index of Sort elements present in the collection
        /// returns -1 if sort element is not found in the collection.
        /// </summary>
        /// <param name="items">Gets the Items collection based on Series or Categorial</param>
        /// <returns>the index of sortElements</returns>
        private int CheckSortElements(Items items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Element sortElement = items[i].ElementValue;
                if (sortElement is SortElement)
                {
                    return i;
                }
                else if (sortElement is FilterElement)
                {
                    return i;
                }
            }

            return -1;
        }
        #endregion

        #region Adding Filter Elements to OlapDataManager
        /// <summary>
        /// Adds the filter elements.
        /// </summary>
        /// <param name="measuresBox">The measures box.</param>
        /// <param name="conditionBox">The condition box.</param>
        /// <param name="valueTextBox">The value text box.</param>
        /// <param name="filterElement">The filter element.</param>
        private void AddFilterElements(ComboBox measuresBox, ComboBox conditionBox, TextBox valueTextBox, List<FilterElement> filterElement)
        {
            FilterCase filterCase = (FilterCase)Enum.Parse(typeof(FilterCase), conditionBox.SelectedItem.ToString());
            FilterElement filterElements = new FilterElement(this.Axis);
            filterElements.FilterCase = filterCase;
            filterElements.IsFilterCondition = true;
            filterElements.Visible = true;
            MetaTreeNode metaTreeNode = measuresBox.SelectedItem as MetaTreeNode;
            Item filterSet = new Item();

            //// If filter is set to series elements then adding the series elements to the
            //// filtercollection and clearing the values from the series elements
            if (Axis == AxisPosition.Series)
            {
                foreach (Item item in this.OlapDataManager.CurrentReport.SeriesElements)
                {
                    filterElements.Elements.Add(item.ElementValue);
                }

                this.OlapDataManager.CurrentReport.SeriesElements.IsFilterOrSortOn = true;
            }
            else if (Axis == AxisPosition.Categorical)
            {
                foreach (Item item in this.OlapDataManager.CurrentReport.CategoricalElements)
                {
                    filterElements.Elements.Add(item.ElementValue);
                }

                this.OlapDataManager.CurrentReport.CategoricalElements.IsFilterOrSortOn = true;
            }

            AddFilterValues(measuresBox, conditionBox, valueTextBox, filterElements);
            filterElement.Add(filterElements);
        }

        /// <summary>
        /// Adds the filter values.
        /// </summary>
        /// <param name="measuresBox">The measures box.</param>
        /// <param name="conditionBox">The condition box.</param>
        /// <param name="valueTextBox">The value text box.</param>
        /// <param name="filterElements">The filter elements.</param>
        private void AddFilterValues(ComboBox measuresBox, ComboBox conditionBox, TextBox valueTextBox, FilterElement filterElements)
        {
            MetaTreeNode metaTreeNode = measuresBox.SelectedItem as MetaTreeNode;
            filterElements.FilterValue.Add(new MeasureElement { UniqueName = metaTreeNode.UniqueName, Visible = true });
            filterElements.FilterValue.Add(new FilterValue { Filter_Value = double.Parse(valueTextBox.Text), Visible = true });
        }

        /// <summary>
        /// Updates the cube model.
        /// </summary>
        /// <param name="filterElements">The filter elements.</param>
        public void updateOlapDataManager(FilterElement filterElements)
        {
            this.OlapDataManager.CurrentReport.FilterElements.Add(new Item { Axis = Axis, ElementValue = filterElements });
        }
        #endregion

        #region Handling Validations
        /// <summary>
        /// Checks for Valid the filter.
        /// </summary>
        /// <param name="measuresBoxOne">The measures box one.</param>
        /// <param name="conditionBoxOne">The condition box one.</param>
        /// <param name="valueTextBoxOne">The value text box one.</param>
        /// <returns>returns true if all the values are specified to filter condition</returns>
        public bool validFilter(ComboBox measuresBoxOne, ComboBox conditionBoxOne, TextBox valueTextBoxOne)
        {
            if (measuresBoxOne.SelectedItem != null && conditionBoxOne.SelectedItem != null && (valueTextBoxOne.Text != null && IsDouble(valueTextBoxOne.Text.ToString())))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates the sort condition.
        /// </summary>
        /// <param name="sortBox">The sort box.</param>
        /// <returns>returns true if sortbox is selected else false is returned</returns>
        private bool ValidateSortCondition(ComboBox sortBox)
        {
            if (sortBox.SelectedItem == null)
            {
                MessageBox.Show("Please choose the Measure to Sort the Cube", "Invalid Choice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the filter condition.
        /// </summary>
        /// <param name="measuresBox">The measures box.</param>
        /// <param name="conditionBox">The condition box.</param>
        /// <param name="valueTextBox">The value text box.</param>
        /// <returns>validates the datas in the parameters specified and returns true if 
        /// it matches the condition else returns false</returns>
        private bool ValidateFilterCondition(ComboBox measuresBox, ComboBox conditionBox, TextBox valueTextBox)
        {
            double filterValue;
            if (measuresBox.SelectedItem == null)
            {
                MessageBox.Show("Please choose the Measure to filter the Cube", "Invalid Choice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (conditionBox.SelectedItem == null)
            {
                MessageBox.Show("Please choose a Condition on Measure to filter the Cube", "Invalid Choice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if ((valueTextBox.Text == null) || (valueTextBox.Text == string.Empty))
            {
                MessageBox.Show("Please enter a value Measure to filter the Cube", "Invalid Entry", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                valueTextBox.Focus();
                return false;
            }

            if (!Double.TryParse(valueTextBox.Text.ToString(), out filterValue))
            {
                MessageBox.Show("Please enter a numeric Value in Value Textbox", "Invalid Value", MessageBoxButton.OK, MessageBoxImage.Error);
                valueTextBox.Focus();
                return false;
            }

            return true;
        }
        #endregion

        #region Group Box Events
        private void filterOneGroupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)filterOneGroupCheckBox.IsChecked)
            {
                filterTwoGroup.IsEnabled = true;
                EnableFilterOneCombos();
                if (filterTwoGroupCheckBox.IsChecked == true)
                    EnableFilterTwoCombos();
            }
            else
            {
                filterTwoGroup.IsEnabled = false;
                DisableFilterTwoCombos();
            }
        }

        private void filterOneGroupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)filterOneGroupCheckBox.IsChecked)
            {
                filterTwoGroup.IsEnabled = false;
                DisableFilterOneCombos();
                DisableFilterTwoCombos();
            }
        }

        private void filterTwoGroupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)filterTwoGroupCheckBox.IsChecked)
            {
                EnableFilterTwoCombos();
            }
            else
            {
                DisableFilterTwoCombos();
            }
        }

        private void filterTwoGroupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)filterTwoGroupCheckBox.IsChecked)
            {
                DisableFilterTwoCombos();
            }
        }

        private void sortingGroupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)sortingGroupCheckBox.IsChecked)
            {
                EnableSortControls();
            }
            else
            {
                DisableSortControls();
            }
        }

        private void sortingGroupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)sortingGroupCheckBox.IsChecked)
            {
                DisableSortControls();
            }
        }

        void tabControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region Cancelling Cubemodel
        private void CancelOlapDataManager(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
