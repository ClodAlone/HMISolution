// <copyright file="TreeViewAdvMeasureTable.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System.Collections;
using System.Windows;
using System;
using System.Diagnostics;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a class that provides functionality for save measured 
    /// data for items of the TreeViewAdv. Using for virtualizing.
    /// </summary>
    internal static class TreeViewAdvMeasureTable
    {
        #region Members
        /// <summary>
        /// Hash table for save TreeViewAdvMeasureData for items.
        /// </summary>
        internal static Hashtable m_table;

        /// <summary>
        /// Average height for item.
        /// </summary>
        private static double m_averageHeight = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets average height for item.
        /// </summary>
        internal static double AverageHeight
        {
            get
            {
                return m_averageHeight;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="TreeViewAdvMeasureTable"/> class.
        /// </summary>
        static TreeViewAdvMeasureTable()
        {
            m_table = new Hashtable();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Determines whether [is contains key] [the specified key].
        /// </summary>
        /// <param name="key">The key isContains.</param>
        /// <returns>
        /// <c>true</c> if [is contains key] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsContaisKey(object key)
        {
            bool isContains = false;

            if (key != null && m_table.Contains(key))
            {
                isContains = true;
            }

            return isContains;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The keyTreeViewAdvMeasureData .</param>
        /// <param name="value">The value TreeViewAdvMeasureData.</param>
        internal static void Add(object key, TreeViewAdvMeasureData value)
        {
            if (!value.IsVisible)
            {
                if (m_table.Contains(key))
                {
                    m_table.Remove(key);
                }

                return;
            }

            if (m_table.Contains(key))
            {
                TreeViewAdvMeasureData oldData = m_table[key] as TreeViewAdvMeasureData;

                value.IsExpanded = oldData.IsExpanded;
                value.Delta = oldData.Delta;
                value.IsInProgress = oldData.IsInProgress;
                value.IsStartExpand = oldData.IsStartExpand;
                value.IsStartCollapse = oldData.IsStartCollapse;

                Size newExtSize = new Size(oldData.InternalExtendedSize.Width, oldData.InternalExtendedSize.Height);
                newExtSize.Height -= Math.Min(oldData.InternalExtendedSize.Height, oldData.Size.Height);
                newExtSize.Height += value.Size.Height;

                double width = GetObjectChildrenWidthExceptItem(key, null);
                if (width < value.Size.Width)
                {
                    newExtSize.Width -= Math.Min(oldData.InternalExtendedSize.Width, oldData.ExtendedSize.Width);
                    newExtSize.Width += value.Size.Width;
                }

                value.InternalExtendedSize = newExtSize;

                if (oldData.ParentKey == null && value.ParentKey != null)
                {
                    AddToParentExtended(key, value.ParentKey, value.IsExpanded ? value.InternalExtendedSize : value.Size);
                }
                else
                {
                    SubFromParentExtended(key, oldData.ParentKey, oldData.IsExpanded ? oldData.InternalExtendedSize : oldData.Size);
                    AddToParentExtended(key, oldData.ParentKey, value.IsExpanded ? value.InternalExtendedSize : value.Size);
                }

                m_averageHeight = (m_averageHeight == 0) ?
                    value.Size.Height / 2 : (m_averageHeight + value.Size.Height) / 2;

                m_table[key] = value;
            }
            else
            {
                m_table.Add(key, value);
                AddToParentExtended(key, value.ParentKey, value.Size);
            }
        }

        /// <summary>
        /// Adds the animation.
        /// </summary>
        /// <param name="key">The key TreeViewAdvMeasureData.</param>
        /// <param name="delta">The delta TreeViewAdvMeasureData.</param>
        /// <param name="isExpand">if set to <c>true</c> [is expand].</param>
        internal static void AddAnimation(object key, double delta, bool isExpand)
        {
            if (key != null && m_table.Contains(key))
            {
                ChangeDelta(key, delta, isExpand, true, 0, false, false);
            }
        }

        /// <summary>
        /// Collapses the specified key.
        /// </summary>
        /// <param name="key">The key TreeViewAdvMeasureData.</param>
        internal static void Collapse(object key)
        {
            if (key != null && m_table.Contains(key))
            {
                TreeViewAdvMeasureData data = m_table[key] as TreeViewAdvMeasureData;
                if (data.IsExpanded)
                {
                    data.IsExpanded = false;

                    if (data.ParentKey != null && data.IsVisible)
                    {
                        Size size = new Size(data.InternalExtendedSize.Width - data.Size.Width, data.InternalExtendedSize.Height - data.Size.Height);
                        SubFromParentExtended(key, data.ParentKey, size);
                    }

                    ResetDelta(key);
                }
            }
        }

        /// <summary>
        /// Expands the specified key.
        /// </summary>
        /// <param name="key">The key TreeViewAdvMeasureData.</param>
        internal static void Expand(object key)
        {
            if (key != null && m_table.Contains(key))
            {
                TreeViewAdvMeasureData data = m_table[key] as TreeViewAdvMeasureData;
                if (!data.IsExpanded)
                {
                    data.IsExpanded = true;

                    if (data.ParentKey != null && data.IsVisible)
                    {
                        Size size = new Size(data.InternalExtendedSize.Width, data.InternalExtendedSize.Height - data.Size.Height);
                        AddToParentExtended(key, data.ParentKey, size);
                    }

                    ResetDelta(key);
                }
            }
        }

        /// <summary>
        /// Obsolete. Use RemoveExt instead.
        /// </summary>
        /// <param name="key">TreeViewAdv MeasureData</param>
        internal static void Remove(object key)
        {
            if (key != null && m_table.Contains(key))
            {
                TreeViewAdvMeasureData data = m_table[key] as TreeViewAdvMeasureData;

                if (data.ParentKey != null)
                {
                    Size size = new Size(data.Size.Width, data.Size.Height);
                    SubFromParentExtended(key, data.ParentKey, size);
                }

                m_table.Remove(key);
            }
        }

        /// <summary>
        /// Removes item and all it's children.
        /// </summary>
        /// <param name="key">TreeViewAdv MeasureData</param>
        internal static void RemoveExt(object key)
        {
            if (key != null && m_table.Contains(key))
            {
                ArrayList list = new ArrayList(m_table.Keys);
                for (int i = 0; i < list.Count; i++)
                {
                    object k = list[i];
                    TreeViewAdvMeasureData data = m_table[k] as TreeViewAdvMeasureData;

                    if (data != null && data.ParentKey != null && object.Equals(data.ParentKey, key))
                    {
                        RemoveExt(k);
                        list.RemoveAt(i);
                        i--;
                    }
                }

                Remove(key);
            }
        }

        /// <summary>
        /// Removes it's all children's only. Not actual item.
        /// </summary>
        /// <param name="key">TreeViewAdv MeasureData</param>
        internal static void RemoveChilds(object key)
        {
            if (key != null && m_table.Contains(key))
            {
                ArrayList list = new ArrayList(m_table.Keys);
                for (int i = 0; i < list.Count; i++)
                {
                    object k = list[i];
                    TreeViewAdvMeasureData data = m_table[k] as TreeViewAdvMeasureData;

                    if (data != null && data.ParentKey != null && object.Equals(data.ParentKey, key))
                    {
                        Remove(k);
                        list.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        internal static void RemoveAll()
        {
            m_table.Clear();
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="key">The key TreeViewAdvMeasureData.</param>
        /// <returns>TreeViewAdv MeasureData</returns>
        internal static TreeViewAdvMeasureData GetData(object key)
        {
            TreeViewAdvMeasureData data = null;

            if (key != null && m_table.ContainsKey(key))
            {
                data = m_table[key] as TreeViewAdvMeasureData;
            }

            return data;
        }

        public static Hashtable DataTable
        {
            get
            {
                return m_table;
            }
        }


        /// <summary>
        /// Adds to parent extended.
        /// </summary>
        /// <param name="key">The key TreeViewAdvMeasureData.</param>
        /// <param name="parentKey">The parent key TreeViewAdvMeasureData.</param>
        /// <param name="size">The size TreeViewAdvMeasureData.</param>
        private static void AddToParentExtended(object key, object parentKey, Size size)
        {
            if (parentKey != null)
            {
                TreeViewAdvMeasureData data = null;

                if (m_table.ContainsKey(parentKey))
                {
                    data = m_table[parentKey] as TreeViewAdvMeasureData;
                }
                else
                {
                    data = TreeViewItemAdv.GetDefaultMeasureData(null);
                    m_table.Add(parentKey, data);
                }

                if (data != null && data.IsVisible)
                {
                    TreeViewAdvMeasureData itemData = m_table[key] as TreeViewAdvMeasureData;

                    if (itemData != null && itemData.IsVisible)
                    {
                        double height = data.InternalExtendedSize.Height + size.Height;
                        double width = data.InternalExtendedSize.Width;

                        double diffWidth = size.Width + data.ItemsOffset - width;
                        if (diffWidth > 0)
                        {
                            width = size.Width + data.ItemsOffset;
                        }

                        size.Width = width;
                        data.InternalExtendedSize = new Size(width, height);

                        if (data != null && data.IsExpanded)
                        {
                            AddToParentExtended(parentKey, data.ParentKey, size);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Subs from parent extended.
        /// </summary>
        /// <param name="key">The key TreeViewAdvMeasureData.</param>
        /// <param name="parentKey">The parent key TreeViewAdvMeasureData.</param>
        /// <param name="size">The size TreeViewAdvMeasureData.</param>
        internal static void SubFromParentExtended(object key, object parentKey, Size size)
        {
            if (parentKey != null && m_table.ContainsKey(parentKey))
            {
                TreeViewAdvMeasureData data = m_table[parentKey] as TreeViewAdvMeasureData;

                if (data != null)
                {
                    double height = data.InternalExtendedSize.Height - size.Height;
                    double width = data.InternalExtendedSize.Width;
                    double newWidth = GetObjectChildrenWidthExceptItem(parentKey, key);

                    if (newWidth < width)
                    {
                        if (newWidth < data.Size.Width)
                        {
                            newWidth = data.Size.Width;
                        }

                        width = newWidth;
                        size.Width += data.ItemsOffset;
                    }

                    if (height < 0)
                    {
                        height = 0;
                    }

                    data.InternalExtendedSize = new Size(width, height);

                    if (data.IsExpanded)
                    {
                        SubFromParentExtended(parentKey, data.ParentKey, size);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the object children.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <returns>Return the list of collection.</returns>
        private static IList GetObjectChildren(object key)
        {
            ArrayList list = new ArrayList();

            foreach (object obj in m_table.Keys)
            {
                TreeViewAdvMeasureData data = m_table[obj] as TreeViewAdvMeasureData;
                if (object.Equals(key, data.ParentKey))
                {
                    list.Add(obj);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the object children width except item.
        /// </summary>
        /// <param name="parentKey">The parent key.</param>
        /// <param name="childKey">The child key.</param>
        /// <returns>Return item</returns>
        private static double GetObjectChildrenWidthExceptItem(object parentKey, object childKey)
        {
            TreeViewAdvMeasureData pData = m_table[parentKey] as TreeViewAdvMeasureData;
            double width = object.Equals(parentKey, childKey) ? 0.0 : pData.Size.Width;

            IList list = GetObjectChildren(parentKey);
            foreach (object obj in list)
            {
                TreeViewAdvMeasureData data = m_table[obj] as TreeViewAdvMeasureData;
                if (data.IsVisible && (width < data.ExtendedSize.Width + data.ItemsOffset) &&
                    (childKey == null || !object.Equals(obj, childKey)))
                {
                    width = data.ExtendedSize.Width + data.ItemsOffset;
                }
            }

            return width;
        }

        /// <summary>
        /// Changes the delta.
        /// </summary>
        /// <param name="key">The key TreeViewAdvMeasureData.</param>
        /// <param name="delta">The delta TreeViewAdvMeasureData.</param>
        /// <param name="isExpand">if set to <c>true</c> [is expand].</param>
        /// <param name="isFirst">if set to <c>true</c> [is first].</param>
        /// <param name="offset">The offset.</param>
        /// <param name="bIsStartExpand">if set to <c>true</c> [b is start expand].</param>
        /// <param name="bIsStartCollapse">if set to <c>true</c> [b is start collapse].</param>
        private static void ChangeDelta(object key, double delta, bool isExpand, bool isFirst, double offset, bool bIsStartExpand, bool bIsStartCollapse)
        {
            if (key != null && m_table.Contains(key))
            {
                TreeViewAdvMeasureData data = m_table[key] as TreeViewAdvMeasureData;

                if (isFirst)
                {
                    if (!data.IsInProgress && !data.IsStartCollapse && !data.IsStartExpand)
                    {
                        bIsStartExpand = isExpand;
                        bIsStartCollapse = !isExpand;
                    }
                    else
                    {
                        bIsStartExpand = data.IsStartExpand;
                        bIsStartCollapse = data.IsStartCollapse;
                    }

                    offset = data.InternalExtendedSize.Height - data.Size.Height;
                    data.Delta = data.Size.Height + delta;
                }
                else
                {
                    data.Delta = data.InternalExtendedSize.Height;
                    data.Delta += delta;

                    if (bIsStartCollapse)
                    {
                        data.Delta -= offset;
                    }
                }

                data.IsStartExpand = bIsStartExpand;
                data.IsStartCollapse = bIsStartCollapse;
                data.IsInProgress = true;
                ChangeDelta(data.ParentKey, delta, isExpand, false, offset, bIsStartExpand, bIsStartCollapse);
            }
        }

        /// <summary>
        /// Resets the delta.
        /// </summary>
        /// <param name="key">The key TreeViewAdvMeasureData.</param>
        private static void ResetDelta(object key)
        {
            if (key != null && m_table.Contains(key))
            {
                TreeViewAdvMeasureData data = m_table[key] as TreeViewAdvMeasureData;
                data.Delta = 0;
                data.IsInProgress = false;
                data.IsStartExpand = false;
                data.IsStartCollapse = false;
                ResetDelta(data.ParentKey);
            }
        }
        #endregion
    }
}
