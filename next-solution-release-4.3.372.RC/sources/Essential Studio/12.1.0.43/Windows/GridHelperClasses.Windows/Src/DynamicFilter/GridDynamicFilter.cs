//-------------------------------------------------------------------------------------------------
// <copyright file="GridDynamicFilter.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Drawing;
    using Syncfusion.Styles;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Collections;
    using System.Diagnostics;
    using System.Windows.Forms;
    using System.ComponentModel;
 
    /// <summary>
    /// A filtering utility that allows the user to view the filter results for each and every character that is being typed.
    /// </summary>
    public class GridDynamicFilter
    {
        bool applyFilterOnlyOnCellLostFocus;
        GridTableFilterBarExtCellModel filterBarExtCell = null;
        GridListFilterBarCellModel filterBarExtCellGridList = null;
        bool allowIndividualColumnWiring = false;
        /// <summary>
        /// It allows to set the desired filter on specified column,if the value is set to True,
        /// through which the filter can be set in column using this.gridGroupingControl1.TableDescriptor.Columns[ColumnName].Appearance.FilterBarCell.CellType = "DynamicFilterCell"
        /// </summary>
        [DefaultValue(false)]
        public bool AllowIndividualColumnWiring
        {
            get
            {
                return allowIndividualColumnWiring;
            }
            set
            {
                allowIndividualColumnWiring = value;
            }
        }

        /// <summary>
        /// Turn On/Off the filtering on each key stroke. 
        /// Default value is false, it allows filtering on each key stroke. 
        /// If it is set to 'true', the filtering is done only when the filter cell lost focus(including Enter,Arrow keys,Tab keys).
        /// </summary>
        /// 
        public bool ApplyFilterOnlyOnCellLostFocus
        {
            get
            {
                return applyFilterOnlyOnCellLostFocus;
            }

            set
            {
                applyFilterOnlyOnCellLostFocus = value;
            }
        }
        /// <summary>
        /// Creates an instance of GridDynamicFilter.
        /// </summary>
        public GridDynamicFilter()
        {
            ApplyFilterOnlyOnCellLostFocus = false;
        }

        /// <summary>
        /// Occurs immediately before the RecordFilterCollectionEditor Dialog is displayed. The ControlEventArgs.Control 
        /// the form.
        /// </summary>
        public event ControlEventHandler ShowingCustomFilterDialog;

        /// <summary>
        /// Hooks up the grouping grid with the dynamic filter.
        /// </summary>
        /// <param name="groupingGrid">The grouping grid.</param>
        public void WireGrid(GridGroupingControl groupingGrid)
        {
            if (groupingGrid != null)
            {
                    groupingGrid.BeginUpdate();
                    if (groupingGrid.TableModel.EnableGridListControlInComboBox && !groupingGrid.TableModel.EnableLegacyStyle)
                    {
                        filterBarExtCellGridList = new GridListFilterBarCellModel(groupingGrid.TableModel, ApplyFilterOnlyOnCellLostFocus, true);
                        if (!AllowIndividualColumnWiring)
                            groupingGrid.TableModel.CellModels["FilterBarCell"] = filterBarExtCellGridList;
                        else
                            groupingGrid.TableModel.CellModels.Add("DynamicFilterCell", filterBarExtCellGridList);
                        filterBarExtCellGridList.ShowingCustomFilterDialog += new ControlEventHandler(filterBarExtCellGridList_ShowingCustomFilterDialog);
                        groupingGrid.Appearance.FilterBarCell.CellType = "FilterBarCell";
                    }
                    else
                    {
                        filterBarExtCell = new GridTableFilterBarExtCellModel(groupingGrid.TableModel, ApplyFilterOnlyOnCellLostFocus);
                        if (!AllowIndividualColumnWiring)
                            groupingGrid.TableModel.CellModels["FilterBarCell"] = filterBarExtCell;
                        else
                            groupingGrid.TableModel.CellModels.Add("DynamicFilterCell", filterBarExtCell);
                        filterBarExtCell.ShowingCustomFilterDialog += new ControlEventHandler(filterBarExtCell_ShowingCustomFilterDialog);
                    }
                    this.LoadCompareOperator();
                    groupingGrid.EndUpdate(true);
                    groupingGrid.Refresh();
                }
        }
        void filterBarExtCellGridList_ShowingCustomFilterDialog(object sender, ControlEventArgs e)
        {
            if (ShowingCustomFilterDialog != null)
            {
                ShowingCustomFilterDialog(sender, e);
            }
        }

        /// <summary>
        /// Used internally
        /// </summary>
        private void filterBarExtCell_ShowingCustomFilterDialog(object sender, ControlEventArgs e)
        {
            if (ShowingCustomFilterDialog != null)
            {
                ShowingCustomFilterDialog(sender, e);
            }
        }

        /// <summary>
        /// Unhook the grouping grid from the dynamic filter.
        /// </summary>
        /// <param name="groupingGrid">The grouping grid.</param>
        public void UnWireGrid(GridGroupingControl groupingGrid)
        {
            if (groupingGrid != null)
            {
                groupingGrid.BeginUpdate();
                groupingGrid.TableModel.CellModels.Remove("FilterBarCell");
                groupingGrid.EndUpdate(true);
                groupingGrid.Refresh();
            }
        }
        
        /// <summary>
        /// Saves the compare Operator state
        /// </summary>
        public void SaveCompareOperator()
        {
            FileStream fs = new FileStream
                   ("CompareOperatorStore.dat", FileMode.OpenOrCreate, FileAccess.Write);

            if (filterBarExtCell == null && filterBarExtCellGridList == null)
            {
#if DEBUG
                Debug.WriteLine("Compare Operator serialiization Failed");
#endif
                return;
            }

            try
            {   
                BinaryFormatter bf = new BinaryFormatter();
                //as easy as 1,2,3...we serialize a to a binary 
                //formating using file stream.
                if (filterBarExtCell != null)
                bf.Serialize(fs, this.filterBarExtCell.CompareOperators);
                else
                    bf.Serialize(fs, this.filterBarExtCellGridList.CompareOperators);
            }
            catch
            {
#if DEBUG
                Debug.WriteLine("Compare Operator serialiization Failed");
#endif
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Loads the saved compare operator state if any.
        /// </summary>
        public void LoadCompareOperator()
        {
            FileStream fs = new FileStream
                   ("CompareOperatorStore.dat", FileMode.OpenOrCreate, FileAccess.Read);

            if (filterBarExtCell != null && fs != null && fs.Length > 0)
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.filterBarExtCell.CompareOperators = bf.Deserialize(fs) as Hashtable;
                    CreateCompareOperatorListEventArgs arg = new CreateCompareOperatorListEventArgs(this.filterBarExtCell.customFilters);
                    this.filterBarExtCell.CreateCompareOperatorList += new CreateCompareOperatorListHandler(this.filterBarExtCell.GridTableFilterBarExtCellModel_CreateCompareOperatorList);
                    this.filterBarExtCell.RaiseCreateCompareOperatorList(this, arg);
                }
                catch
                {
#if DEBUG
                    Debug.WriteLine("Compare Operator Deserialiization Failed");
#endif
                }
                finally
                {
                    fs.Close();
                }
            }
            else if (filterBarExtCellGridList != null && fs != null && fs.Length > 0)
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.filterBarExtCellGridList.CompareOperators = bf.Deserialize(fs) as Hashtable;
                    CreateCompareOperatorListEventArgs arg = new CreateCompareOperatorListEventArgs(this.filterBarExtCellGridList.customFilters);
                    this.filterBarExtCellGridList.CreateCompareOperatorList += new CreateCompareOperatorListHandler(this.filterBarExtCellGridList.GridTableFilterBarExtCellModel_CreateCompareOperatorList);
                    this.filterBarExtCellGridList.RaiseCreateCompareOperatorList(this, arg);
                }
                catch
                {
#if DEBUG
                    Debug.WriteLine("Compare Operator Deserialiization Failed");
#endif
                }
                finally
                {
                    fs.Close();
                }
            }
        }
    }
}
