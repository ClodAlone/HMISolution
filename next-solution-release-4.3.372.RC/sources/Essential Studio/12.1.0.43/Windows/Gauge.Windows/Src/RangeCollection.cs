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
using System.Collections;

namespace Syncfusion.Windows.Forms.Gauge
{
    public class RangeCollection : CollectionBase
    {
        private RadialGauge Owner;
        /// <summary>
        /// Constructor for Rangecollection
        /// </summary>
        /// <param name="sender"></param>
        public RangeCollection(RadialGauge sender) { Owner = sender; }
        /// <summary>
        /// Gets the index of for the Range
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Range this[int index] { get { return (Range)List[index]; } }
        /// <summary>
        /// Returns whether the list contains the range type
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public bool Contains(Range itemType) { return List.Contains(itemType); }
        /// <summary>
        /// Adds the range type to the list
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int Add(Range itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            return List.Add(itemType);
        }
        /// <summary>
        ///Removes the type from the list
        /// </summary>
        /// <param name="itemType"></param>
        public void Remove(Range itemType) { List.Remove(itemType); }
        /// <summary>
        /// Inserts teh range type into the list
        /// </summary>
        /// <param name="index"></param>
        /// <param name="itemType"></param>
        public void Insert(int index, Range itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            List.Insert(index, itemType);
        }
        /// <summary>
        /// Returns the index of the range type
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int IndexOf(Range itemType) { return List.IndexOf(itemType); }
        /// <summary>
        /// searches the name in the list
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Range FindByName(string name)
        {
            foreach (Range ptrRange in List)
            {
                if (ptrRange.Name == name) return ptrRange;
            }
            return null;
        }
        /// <summary>
        /// Overrides the oninsert method
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnInsert(int index, object value)
        {
            if (string.IsNullOrEmpty(((Range)value).Name)) ((Range)value).Name = GetUniqueName();
            base.OnInsert(index, value);
            ((Range)value).SetOwner(Owner);
        }
        /// <summary>
        /// overrides the onremove method
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnRemove(int index, object value)
        {
            if (Owner != null) Owner.RepaintControl();
        }
        /// <summary>
        /// Overrides the clear method
        /// </summary>
        protected override void OnClear()
        {
            if (Owner != null) Owner.RepaintControl();
        }
        /// <summary>
        /// Gets the unique name
        /// </summary>
        /// <returns></returns>
        private string GetUniqueName()
        {
            const string Prefix = "GaugeRange";
            int index = 1;
            bool valid;
            while (this.Count != 0)
            {
                valid = true;
                for (int x = 0; x < this.Count; x++)
                {
                    if (this[x].Name == (Prefix + index.ToString()))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid) break;
                index++;
            };
            return Prefix + index.ToString();
        }
    }
    public class LinearRangeCollection : CollectionBase
    {
        private LinearGauge Owner;
        /// <summary>
        /// Constructor for LinearRangecollection
        /// </summary>
        /// <param name="sender"></param>
        public LinearRangeCollection(LinearGauge sender) { Owner = sender; }
        /// <summary>
        /// Gets the index of for the Range
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LinearRange this[int index] { get { return (LinearRange)List[index]; } }
        /// <summary>
        /// Returns whether the list contains the range type
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public bool Contains(LinearRange itemType) { return List.Contains(itemType); }
        /// <summary>
        /// Adds the range type to the list
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int Add(LinearRange itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            return List.Add(itemType);
        }
        /// <summary>
        ///Removes the type from the list
        /// </summary>
        /// <param name="itemType"></param>
        public void Remove(LinearRange itemType) { List.Remove(itemType); }
        /// <summary>
        /// Inserts teh range type into the list
        /// </summary>
        /// <param name="index"></param>
        /// <param name="itemType"></param>
        public void Insert(int index, LinearRange itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            List.Insert(index, itemType);
        }
        /// <summary>
        /// Returns the index of the range type
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int IndexOf(LinearRange itemType) { return List.IndexOf(itemType); }
        /// <summary>
        /// searches the name in the list
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LinearRange FindByName(string name)
        {
            foreach (LinearRange ptrRange in List)
            {
                if (ptrRange.Name == name) return ptrRange;
            }
            return null;
        }
        /// <summary>
        /// Overrides the oninsert method
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnInsert(int index, object value)
        {
            if (string.IsNullOrEmpty(((LinearRange)value).Name)) ((LinearRange)value).Name = GetUniqueName();
            base.OnInsert(index, value);
            ((LinearRange)value).SetOwner(Owner);
        }
        /// <summary>
        /// overrides the onremove method
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnRemove(int index, object value)
        {
            if (Owner != null) Owner.RepaintControl();
        }
        /// <summary>
        /// Overrides the clear method
        /// </summary>
        protected override void OnClear()
        {
            if (Owner != null) Owner.RepaintControl();
        }
        /// <summary>
        /// Gets the unique name
        /// </summary>
        /// <returns></returns>
        private string GetUniqueName()
        {
            const string Prefix = "GaugeRange";
            int index = 1;
            bool valid;
            while (this.Count != 0)
            {
                valid = true;
                for (int x = 0; x < this.Count; x++)
                {
                    if (this[x].Name == (Prefix + index.ToString()))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid) break;
                index++;
            };
            return Prefix + index.ToString();
        }
    } 
}
