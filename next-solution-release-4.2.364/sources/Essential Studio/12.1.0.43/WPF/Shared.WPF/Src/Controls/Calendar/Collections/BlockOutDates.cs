#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    ///
    /// </summary>
    public class BlackoutDatesRange
    {
        /// <summary>
        ///
        /// </summary>
        public DateTime StartDate
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public DateTime EndDate
        {
            get;
            set;
        }

        // public event PropertyChangedEventHandler PropertyChanged;

        // public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    ///
    /// </summary>
    [ContentPropertyAttribute("BlockoutDatesRange")]
    public class BlackDatesCollection : ObservableCollection<BlackoutDatesRange>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }
    }
}