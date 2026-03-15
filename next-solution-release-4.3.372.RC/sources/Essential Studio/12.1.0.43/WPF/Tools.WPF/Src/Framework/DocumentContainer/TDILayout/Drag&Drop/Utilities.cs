// <copyright file="Utilities.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Collections;
using System.Windows.Documents;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents kit class for dragging
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class Utilities
    {
        /// <summary>
        /// Represents itemsCount
        /// </summary>
        private static int iitemsCount;

        #region Public methods
        /// <summary>
        /// Gets the item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="bottomMostVisual">The bottom most visual.</param>
        /// <returns>TabItemExt itemContainer</returns>
        public static TabItemExt GetItemContainer(DocumentTabControl itemsControl, Visual bottomMostVisual)
        {
            TabItemExt itemContainer = null;

            iitemsCount = itemsControl.Items.Count;

            if (itemsControl != null && bottomMostVisual != null
                && itemsControl.Items.Count >= 1)
            {
                var firstContainer = itemsControl.ItemContainerGenerator.ContainerFromIndex(0);

                if (firstContainer != null)
                {
                    Type containerType = firstContainer.GetType();

                    itemContainer = (TabItemExt)VisualUtils.FindAncestor(bottomMostVisual, containerType);

                    if (itemContainer != null && itemContainer.DataContext != null)
                    {
                        FrameworkElement itemContainerVerify = itemsControl.ItemContainerGenerator.ContainerFromItem(itemContainer.DataContext) as FrameworkElement;

                        if (itemContainer != itemContainerVerify && itemContainerVerify != null)
                        {
                            itemContainer = null;
                        }
                    }
                }
            }

            return itemContainer;
        }
        
        /// <summary>
        /// Inserts the item in items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemToInsert">The item to insert.</param>
        /// <param name="insertionIndex">Index of the insertion.</param>
        public static void InsertItemInItemsControl(DocumentTabControl itemsControl, object itemToInsert, int insertionIndex)
        {
            if (itemToInsert != null)
            {
                IEnumerable itemsSource = itemsControl.ItemsSource;

                if (itemsSource == null)
                {
                    if (DragDropHelper.m_Source == DragDropHelper.m_Target && iitemsCount <= 1)
                    {
                    }
                    else
                    {
                        TabItemExt element = itemToInsert as TabItemExt;
                        if(element!=null)
                        {
                            element.SetValue(TDILayoutPanel.WayOfTDIGroupProperty,itemsControl.GetValue(TDILayoutPanel.WayOfTDIGroupProperty));
                            ContentPresenter presenter = element.Content as ContentPresenter;
                            FrameworkElement presenterelement = presenter.Content as FrameworkElement;
                            if(presenterelement!=null)
                            {
                                presenterelement.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, itemsControl.GetValue(TDILayoutPanel.WayOfTDIGroupProperty));
                            }
                        }
                        itemsControl.Items.Insert(insertionIndex, itemToInsert);
                    }
                }
                else if (itemsSource is IList)
                {
                    ((IList)itemsSource).Insert(insertionIndex, itemToInsert);
                }
                else
                {
                    ReflectionAction(itemsSource, "RemoveAt", new[] { insertionIndex, itemToInsert });
                }
            }
        }
        
        /// <summary>
        /// Removes the item from items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemToRemove">The item to remove.</param>
        /// <returns>int indexToBeRemoved</returns>
        public static int RemoveItemFromItemsControl(DocumentTabControl itemsControl, object itemToRemove)
        {
            int indexToBeRemoved = -1;

            if (itemToRemove != null)
            {
                indexToBeRemoved = itemsControl.Items.IndexOf(itemToRemove);

                if (indexToBeRemoved != -1)
                {
                    IEnumerable itemsSource = itemsControl.ItemsSource;

                    if (itemsSource == null)
                    {
                        if (DragDropHelper.m_Source == DragDropHelper.m_Target && iitemsCount <= 1) 
                        { 
                        }
                        else
                        {
                            itemsControl.Items.RemoveAt(indexToBeRemoved);
                        }
                    }
                    else if (itemsSource is IList)
                    {
                        ((IList)itemsSource).RemoveAt(indexToBeRemoved);
                    }
                    else
                    {
                        ReflectionAction(itemsSource, "RemoveAt", new object[] { indexToBeRemoved });
                    }
                }
            }

            return indexToBeRemoved;
        }
        
        /// <summary>
        /// Determines whether [is in first half] [the specified container].
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="clickedPoint">The clicked point.</param>
        /// <param name="hasVerticalOrientation">if set to <c>true</c> [has vertical orientation].</param>
        /// <returns>
        /// <c>true</c> if [is in first half] [the specified container]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInFirstHalf(FrameworkElement container, Point clickedPoint, bool hasVerticalOrientation)
        {
            return hasVerticalOrientation
                ? clickedPoint.Y < container.ActualHeight / 2
                : clickedPoint.X < container.ActualWidth / 2;
        }

        public static bool IsInFirstHalf(FrameworkElement container, Dock TabStripPlacement, Point clickedPoint, bool hasVerticalOrientation)
        {
            bool m_IsInFirstHalf = false;
            switch (TabStripPlacement)
            {
                case Dock.Top:
                case Dock.Right:
                    m_IsInFirstHalf = hasVerticalOrientation ? clickedPoint.Y < container.ActualHeight / 2
                                                            : clickedPoint.X < container.ActualWidth / 2;
                    break;
                case Dock.Bottom:
                case Dock.Left:
                    m_IsInFirstHalf = hasVerticalOrientation ? clickedPoint.Y > container.ActualHeight / 2
                                                            : clickedPoint.X > container.ActualWidth / 2;
                    break;
            }
            return m_IsInFirstHalf;
        }

        /// <summary>
        /// Determines whether [is movement big enough] [the specified initial position].
        /// </summary>
        /// <param name="initialPosition">The initial position.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <returns>
        /// <c>true</c> if [is movement big enough] [the specified initial position]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMovementBigEnough(Point initialPosition, Point currentPosition)
        {
            return Math.Abs(currentPosition.X - initialPosition.X)
                >= SystemParameters.MinimumHorizontalDragDistance
                || Math.Abs(currentPosition.Y - initialPosition.Y)
                >= SystemParameters.MinimumVerticalDragDistance;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Reflections the action.
        /// </summary>
        /// <param name="itemsSource">The items source.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        private static void ReflectionAction(IEnumerable itemsSource, string methodName, object[] parameters)
        {
            Type type = itemsSource.GetType();
            Type genericIListType = type.GetInterface("IList`1");

            if (genericIListType != null)
            {
                type.GetMethod(methodName).Invoke(itemsSource, parameters);
            }
        }
        #endregion
    }
}
