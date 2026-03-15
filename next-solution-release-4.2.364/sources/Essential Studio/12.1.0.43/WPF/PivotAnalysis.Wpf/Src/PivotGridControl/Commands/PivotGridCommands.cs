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
using System.Windows.Input;

#if !SILVERLIGHT
namespace Syncfusion.Windows.Controls.PivotGrid
#else
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Represents the class for PivotGrid to define its operations through routed UI commands.
    /// </summary>
#if !SILVERLIGHT
    public static class PivotGridCommands
    {
        /// <summary>
        /// A routed UI command to sort pivot item.
        /// </summary>
        public static RoutedUICommand SortPivotItem = new RoutedUICommand("SortPivotItem", "SortPivotItem",
                                                        typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to show filter popup window.
        /// </summary>
        public static RoutedUICommand ShowFilter = new RoutedUICommand("ShowFilter", "ShowFilter",
                                                typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to show field list window.
        /// </summary>
        public static RoutedUICommand ShowFieldList = new RoutedUICommand("ShowFieldList", "ShowFieldList",
                                                typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to reload data.
        /// </summary>
        public static RoutedUICommand ReloadData = new RoutedUICommand("ReloadData", "ReloadData",
                                                typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to re-order the pivot items.
        /// </summary>
        public static RoutedUICommand Order = new RoutedUICommand("Order", "Order",
                                                typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to delete pivot item.
        /// </summary>
        public static RoutedUICommand DeleteItem = new RoutedUICommand("DeleteItem", "DeleteItem",
                                                typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to expand pivot item.
        /// </summary>
        public static RoutedUICommand ExpandItem = new RoutedUICommand("ExpandItem", "ExpandItem",
                                              typeof(PivotGridCommands));

        //public static RoutedUICommand AllowFilter = new RoutedUICommand("AllowFilter", "AllowFilter", typeof(PivotGridCommands));

       // public static RoutedUICommand AllowSort = new RoutedUICommand("AllowSort", "AllowSort", typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to clear filter.
        /// </summary>
        public static RoutedUICommand ClearFilter = new RoutedUICommand("ClearFilters", "ClearFilters", typeof(PivotGridCommands));

        /// <summary>
        /// A routed UI command to allow sorting on column values.
        /// </summary>
        public static RoutedUICommand AllowValueSort = new RoutedUICommand("AllowValueSort", "AllowValueSort", typeof(PivotGridCommands));
       /// <summary>
       /// A routed UI command to clear the sorting applied on column values
       /// </summary>
        public static RoutedUICommand ClearValueSorts = new RoutedUICommand("ClearValueSorts", "ClearValueSorts", typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to allow filtering on column values
        /// </summary>
        public static RoutedUICommand AllowValueFiltering = new RoutedUICommand("AllowValueFiltering", "AllowValueFiltering", typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to clear the filtering applied on column values
        /// </summary>
        public static RoutedUICommand ClearValueFilters = new RoutedUICommand("ClearValueFilters", "ClearValueFilters", typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to hide the value columns
        /// </summary>
        public static RoutedUICommand HideValueColumn = new RoutedUICommand("HideValueColumn", "HideValueColumn", typeof(PivotGridCommands));
        /// <summary>
        /// A routed UI command to show PivotValueChooser window.
        /// </summary>
        public static RoutedUICommand ShowPivotValueChooser = new RoutedUICommand("ShowPivotValueChooser", "ShowPivotValueChooser", typeof(PivotGridCommands));

        /// <summary>
        /// Initializes c_Refresh RoutedUICommand
        /// </summary>
        private static RoutedUICommand c_Refresh = new RoutedUICommand("Refresh", "Refresh", typeof(PivotGridCommands));

        /// <summary>
        /// Initializes c_ResetPivotData RoutedUICommand
        /// </summary>
        private static RoutedUICommand c_ResetPivotData = new RoutedUICommand("ResetPivotData", "ResetPivotData", typeof(PivotGridCommands));

        /// <summary>
        /// Initializes c_Serialize RoutedUICommand
        /// </summary>
        private static RoutedUICommand c_Serialize = new RoutedUICommand("Serialize", "Serialize", typeof(PivotGridCommands));

        /// <summary>
        /// Initializes c_Deserialize RoutedUICommand
        /// </summary>
        private static RoutedUICommand c_Deserialize = new RoutedUICommand("Deserialize", "Deserialize", typeof(PivotGridCommands));
        private static RoutedUICommand c_PopulateDefaultPropertyFields = new RoutedUICommand("PopulateDefaultPropertyFields", "PopulateDefaultPropertyFields", typeof(PivotGridCommands));
        private static RoutedUICommand c_expandAll = new RoutedUICommand("ExpandAll", "ExpandAll", typeof(PivotGridCommands));
        private static RoutedUICommand c_collapseAll = new RoutedUICommand("CollapseAll", "CollapseAll", typeof(PivotGridCommands));
        private static RoutedUICommand c_expandRow = new RoutedUICommand("ExpandRow", "ExpandRow", typeof(PivotGridCommands));
        private static RoutedUICommand c_expandColumn = new RoutedUICommand("ExpandColumn", "ExpandColumn", typeof(PivotGridCommands));
        private static RoutedUICommand c_collapseRow = new RoutedUICommand("CollapseRow", "CollapseRow", typeof(PivotGridCommands));
        private static RoutedUICommand c_collapseColumn = new RoutedUICommand("CollapseColumn", "CollapseColumn", typeof(PivotGridCommands));
       
        /// <summary>
        /// Initializes c_ShowCalculationFilter RoutedUICommand
        /// </summary>
        public static RoutedUICommand c_ShowCalculationFilter = new RoutedUICommand("ShowFilter", "ShowFilter", typeof(PivotSortHeaderCell));
        /// <summary>
        /// Gets the Serialize
        /// </summary>
        public static RoutedUICommand ShowCalculationFilter
        {
            get { return c_ShowCalculationFilter; }
        }

        /// <summary>
        /// Gets the Serialize
        /// </summary>
        public static RoutedUICommand Serialize
        {
            get { return c_Serialize; }
        }

        /// <summary>
        /// Gets the Deserialize
        /// </summary>
        public static RoutedUICommand Deserialize
        {
            get { return c_Deserialize; }
        }

        /// <summary>
        /// Gets the Refresh
        /// </summary>
        public static RoutedUICommand Refresh
        {
            get { return c_Refresh; }
        }

        /// <summary>
        /// Gets the ResetPivotData
        /// </summary>
        public static RoutedUICommand ResetPivotData
        {
            get { return c_ResetPivotData; }
        }

        /// <summary>
        /// Gets the populate default property fields command.
        /// </summary>
        /// <value>The populate default property fields.</value>
        public static RoutedUICommand PopulateDefaultPropertyFields
        {
            get { return c_PopulateDefaultPropertyFields; }
        }

        /// <summary>
        /// Gets the expand all command.
        /// </summary>
        /// <value>The expand all.</value>
        public static RoutedUICommand ExpandAll
        {
            get { return c_expandAll; }
        }

        /// <summary>
        /// Gets the collapse all command.
        /// </summary>
        /// <value>The collapse all.</value>
        public static RoutedUICommand CollapseAll
        {
            get { return c_collapseAll; }
        }

        /// <summary>
        /// Gets the expand row command.
        /// </summary>
        /// <value>The expand row.</value>
        public static RoutedUICommand ExpandRow
        {
            get { return c_expandRow; }
        }

        /// <summary>
        /// Gets the expand column command.
        /// </summary>
        /// <value>The expand column.</value>
        public static RoutedUICommand ExpandColumn
        {
            get { return c_expandColumn; }
        }

        /// <summary>
        /// Gets the collapse row command.
        /// </summary>
        /// <value>The collapse row.</value>
        public static RoutedUICommand CollapseRow
        {
            get { return c_collapseRow; }
        }

        /// <summary>
        /// Gets the collapse column command.
        /// </summary>
        /// <value>The collapse column.</value>
        public static RoutedUICommand CollapseColumn
        {
            get { return c_collapseColumn; }
        }
        
    }

#else

    public class SortPivotRowItemCommand : ICommand
    {
        public SortPivotRowItemCommand()
        {

        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            CanExecuteChanged(parameter, new EventArgs());
        }
    }

    public class SortPivotColumnItemCommand : ICommand
    {
        public SortPivotColumnItemCommand()
        {

        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            CanExecuteChanged(parameter, new EventArgs());
        }
    }

    public class FilterPivotItemCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            //throw new NotImplementedException();
            return true;
        }        

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            CanExecuteChanged(parameter, new EventArgs());
        }
    }

#endif
}
