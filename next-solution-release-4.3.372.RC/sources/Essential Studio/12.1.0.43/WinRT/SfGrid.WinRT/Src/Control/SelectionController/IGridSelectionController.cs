#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.UI.Xaml.ScrollAxis;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using DoubleTappedEventArgs = Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs;
using TappedEventArgs = Windows.UI.Xaml.Input.TappedRoutedEventArgs;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#else
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

#endif

#if WPF
using DoubleTappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using TappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif

#if SILVERLIGHT
using DoubleTappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using TappedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#elif WP
using DoubleTappedEventArgs = System.Windows.Input.GestureEventArgs;
using TappedEventArgs = System.Windows.Input.GestureEventArgs;
#endif

namespace Syncfusion.UI.Xaml.Grid
{

    /// <summary>
    /// Interface which contains the API's and Methods for Selections in SfDataGrid 
    /// </summary>
    /// <remarks></remarks>
    [ClassReference(IsReviewed = false)]
    public interface IGridSelectionController : INotifyPropertyChanged
    {
        /// <summary>
        /// Property which contains the collection of Selected RowIndexes in SfDataGrid.
        /// </summary>
        List<int> SelectedRows { get; }
        
        /// <summary>
        /// Property which Get or set the Brush value for Selection in DataRow
        /// </summary>
        Brush RowSelectionBrush { get; set; }

        /// <summary>
        /// Property which Get or set the Brush value for Selection in Group Header and Summaries.
        /// </summary>
        Brush GroupRowSelectionBrush { get; set; }

        /// <summary>
        /// Property which Get or Set the Brush value for RowHover Highliting.
        /// </summary>
        Brush RowHoverBackgroundBrush { get; set; }

        /// <summary>
        /// Property which helps to Get the CurrentCell Manager of SfDataGrid
        /// </summary>
        GridCurrentCellManager CurrentCellManager { get; }

        /// <summary>
        /// Method which is called while doing Sorting,Filtering,Grouping,Paging and Pasting operations.
        /// </summary>
        /// <param name="args">argument contains the Operation type and Operation arguments</param>
        void HandleGridOperations(GridOperationsHandle args);

        /// <summary>
        /// Method which is called when doing Pointer operations like Pressed,Released,Tapped and Double Tapped.
        /// </summary>
        /// <param name="args">args containd the Pointer operation type, Pointer event args</param>
        /// <param name="rowColumnIndex">Row column index of the cell which has been clicked.</param>
        void HandlePointerOperations(GridPointerEventArgs args, RowColumnIndex rowColumnIndex);

        /// <summary>
        /// Method which called when Selection properties like SelectedIndex,SelectedItem and SelectionMode property changed.
        /// </summary>
        /// <param name="handle">Handle which contains the values of changed property</param>
        void HandleSelectionPropertyChanges(SelectionPropertyChangeHandle handle);

        /// <summary>
        /// Method which called when the collections like SelectedItems, Columns and DataSource changed.
        /// </summary>
        /// <param name="e">Collection Changed Event Args</param>
        /// <param name="reason">Which collection has changed</param>
        void HandleCollectionChanged(NotifyCollectionChangedEventArgs e, CollectionChangedReason reason);

        /// <summary>
        /// Method which called when the Group Expand or Collapsed
        /// </summary>
        /// <param name="index">Group Row Index</param>
        /// <param name="count">Number of rows collapsed or Expanded</param>
        /// <param name="isExpanded">Whether Expanded or collapsed</param>
        void HandleGroupExpandCollapse(int index, int count, bool isExpanded);

        /// <summary>
        /// Method which helps to select the rows between start and end index
        /// </summary>
        /// <param name="startRowIndex">Row start index</param>
        /// <param name="endRowIndex">Row end index</param>
        void SelectRows(int startRowIndex, int endRowIndex);

        /// <summary>
        /// Method which helps to select all the rows.
        /// </summary>
        void SelectAll();

        /// <summary>
        /// Method which helps to clear the selection in Grid.
        /// </summary>
        /// <param name="exceptCurrentRow">Flag which decides whether Current row selection should remove or not</param>
        void ClearSelections(bool exceptCurrentRow);

#if !WP
        
        /// <summary>
        /// Method whcih helps to move the Current row selection to certain Row Column index.
        /// </summary>
        /// <param name="rowColumnIndex">Corresponding Row and Column index where the selection to move.</param>
        void MoveCurrentCell(RowColumnIndex rowColumnIndex);

        /// <summary>
        /// Method which handles the DetailsViewGrid key down.
        /// </summary>
        /// <param name="args">Corresponding KeyEvent argument</param>
        /// <returns>return whether the key down handled by grid or not.</returns>
        bool HandleDetailsViewGridKeyDown(KeyEventArgs args);

        /// <summary>
        /// Method which helps to handle the selection in DataGrid.
        /// </summary>
        /// <param name="args">Corresponding Key Event Args</param>
        /// <returns>return whether the key down handled by grid or not.</returns>
        bool HandleKeyDown(KeyEventArgs args);
#endif
    }

    /// <summary>
    /// Class which used as argument of Selection Property changes.
    /// </summary>
    public class SelectionPropertyChangeHandle
    {
        /// <summary>
        /// New modified vale
        /// </summary>
        public object NewValue { get; set; }
        /// <summary>
        /// old value of the property.
        /// </summary>
        public object OldValue { get; set; }
        /// <summary>
        /// Changed property name.
        /// </summary>
        public string PropertyName { get; set; }
    }

    public class GridPointerEventArgs
    {
        internal GridPointerEventArgs(PointerOperation operation, object eventArgs)
        {
            Operation = operation;
            OriginalEventArgs = eventArgs;
        }

        /// <summary>
        /// Type of PointerOperation. 
        /// </summary>
        public PointerOperation Operation { get; private set; }

        /// <summary>
        /// Pointer Event arguments.
        /// </summary>
        public object OriginalEventArgs { get; private set; }
    }

    public class GridOperationsHandle
    {
        internal GridOperationsHandle(GridOperation operation, object operationArgs)
        {
            Operation = operation;
            OperationArgs = operationArgs;
        }
        /// <summary>
        /// Type of Grid Operation
        /// </summary>
        public GridOperation Operation { get; private set; }

        /// <summary>
        /// Grid Operation arguments.
        /// </summary>
        public object OperationArgs { get; private set; }
    }
}
