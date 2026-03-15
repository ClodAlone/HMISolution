#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Collection of <see cref="ChartDateTimeInterval"/>. Each <see cref="ChartDateTimeRange"/> object has an associated set of intervals that
    /// can be used to iterate over the range. ChartIntervalCollection is the repository for these intervals.
    /// <seealso cref="ChartDateTimeRange.Intervals"/>
    /// </summary>
    public sealed class ChartIntervalCollection : IEnumerable
    {
        #region Members
        private Hashtable m_table = new Hashtable();
        private ChartDateTimeRange m_parent;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of 
        /// </summary>
        /// <param name="parent" type="Syncfusion.Windows.Forms.Chart.ChartDateTimeRange">
        ///     <para>
        ///     Range that is to be associated with all <see cref="ChartDateTimeInterval"/> registered with this collection.
        ///     </para>
        /// </param>
        public ChartIntervalCollection(ChartDateTimeRange parent)
        {
            m_parent = parent;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Removes all registered intervals except the default interval.
        /// </summary>
        public void Reset()
        {
            ChartDateTimeInterval defaultInterval = this[ChartDateTimeInterval.DefaultIntervalName];
            this.Clear();

            if (defaultInterval != null)
            {
                m_table.Add(ChartDateTimeInterval.DefaultIntervalName, defaultInterval);
            }
        }

        /// <summary>
        /// Removes all registered intervals including the default interval.
        /// </summary>
        public void Clear()
        {
            m_table.Clear();
        }

        /// <summary>
        /// Registers an interval with this collection.
        /// </summary>
        /// <param name="name" type="string">
        ///     <para>
        ///     The registration name of the <see cref="ChartDateTimeInterval"/> that is to be registered.
        ///     </para>
        /// </param>
        /// <param name="interval" type="Syncfusion.Windows.Forms.Chart.ChartDateTimeInterval">
        ///     <para>
        ///     The interval that is to be registered.
        ///     </para>
        /// </param>
        public void Register(string name, ChartDateTimeInterval interval)
        {
            interval.SetParent(m_parent);
            m_table[name] = interval;
        }

        /// <summary>
        /// Looks up the collection and removes the <see cref="ChartDateTimeInterval"/> with the specified name.
        /// </summary>
        /// <param name="name" type="string">
        ///     <para>
        ///     The registration name of the <see cref="ChartDateTimeInterval"/> to look for.
        ///     </para>
        /// </param>
        public void Remove(string name)
        {
            ChartDateTimeInterval interval = this[name];
            interval.SetParent(null);
            m_table.Remove(name);
        }

        /// <summary>
        /// Looks up the collection and returns the <see cref="ChartDateTimeInterval"/> with the specified name.
        /// </summary>
        /// <value>
        ///     <para>
        ///     The registration name of the <see cref="ChartDateTimeInterval"/> to look for.
        ///     </para>
        /// </value>
        public ChartDateTimeInterval this[string name]
        {
            get
            {
                object o = m_table[name];

                if (o != null)
                {
                    return o as ChartDateTimeInterval;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Overridden. Returns a string representation of this collection.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string name in m_table.Keys)
            {
                ChartDateTimeInterval interval = this[name];
                sb.AppendFormat("Interval Name: {0}, Details: {1}\r\n", name, interval.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_table.Values.GetEnumerator();
        }
        #endregion
    }
}