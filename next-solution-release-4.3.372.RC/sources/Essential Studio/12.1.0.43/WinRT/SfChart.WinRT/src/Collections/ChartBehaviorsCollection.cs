#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if !WINDOWS_PHONE
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// A collection class which holds ChartBehaviors.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartBehaviorsCollection : ObservableCollection<ChartBehavior>
    {
        private SfChart Area;
        /// <summary>
        /// Called when instance created for ChartBehaviourCollection
        /// </summary>
        /// <param name="area"></param>
        public ChartBehaviorsCollection(SfChart area)
        {
            Area = area;
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert.</param>
        protected override void InsertItem(int index, ChartBehavior item)
        {
            item.ChartArea = Area;
            item.AdorningCanvas = Area.GetAdorningCanvas();
            if (item.AdorningCanvas != null)
                item.InternalAttachElements();
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            var item = this.Items[index];            
            item.DetachElements();
            item.ChartArea = Area;
            base.RemoveItem(index);
        }
        protected override void ClearItems()
        {
            foreach (ChartBehavior behavior in Items)
            {
                behavior.DetachElements();
                behavior.ChartArea = Area;
            }

            base.ClearItems();
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="ChartAxisLabel"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartAxisLabelCollection : ObservableCollection<ChartAxisLabel>
    {
        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert.</param>
        protected override void InsertItem(int index, ChartAxisLabel item)
        {
            base.InsertItem(index, item);
        }

        /// <summary>
        /// ChartAxisLabelsCollection Clear Items
        /// </summary>    
        /// <seealso>
        ///     <cref>ChartAxisLabelsCollection</cref>
        /// </seealso>
        protected override void ClearItems()
        {
            base.ClearItems();
        }
    }
}
