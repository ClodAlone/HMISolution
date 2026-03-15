#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Scroll
{

    /// <summary>
    /// A collection that manages lines with varying height and hidden state. 
    /// It has properties for header and footer lines, total line count, default
    /// size of a line and also lets you add nested collections.
    /// </summary>
    public interface ILineSizeHost : IDisposable
    {
        /// <summary>
        /// Returns the default line size.
        /// </summary>
        /// <returns></returns>
        double GetDefaultLineSize();

        /// <summary>
        /// Returns the line count.
        /// </summary>
        /// <returns></returns>
        int GetLineCount();

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="repeatValueCount">The number of subsequent values with same size.</param>
        /// <returns></returns>
        double GetSize(int index, out int repeatValueCount);

        /// <summary>
        /// Gets the header line count.
        /// </summary>
        /// <returns></returns>
        int GetHeaderLineCount();

        /// <summary>
        /// Gets the footer line count.
        /// </summary>
        /// <returns></returns>
        int GetFooterLineCount();

        /// <summary>
        /// Gets the hidden state for a line.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="repeatValueCount">The number of subsequent lines with same state.</param>
        /// <returns></returns>
        bool GetHidden(int index, out int repeatValueCount);

        /// <summary>
        /// Occurs when a lines size was changed.
        /// </summary>
        event RangeChangedEventHandler LineSizeChanged;

        /// <summary>
        /// Occurs when a lines hidden state changed.
        /// </summary>
        event HiddenRangeChangedEventHandler LineHiddenChanged;

        /// <summary>
        /// Occurs when the default line size changed.
        /// </summary>
        event DefaultLineSizeChangedEventHandler DefaultLineSizeChanged;

        /// <summary>
        /// Occurs when the line count was changed.
        /// </summary>
        event EventHandler LineCountChanged;

        /// <summary>
        /// Occurs when the header line count was changed.
        /// </summary>
        event EventHandler HeaderLineCountChanged;

        /// <summary>
        /// Occurs when the footer line count was changed.
        /// </summary>
        event EventHandler FooterLineCountChanged;

        /// <summary>
        /// Occurs when lines were inserted.
        /// </summary>
        event LinesInsertedEventHandler LinesInserted;

        /// <summary>
        /// Occurs when lines were removed.
        /// </summary>
        event LinesRemovedEventHandler LinesRemoved;

        /// <summary>
        /// Initializes the scroll axis.
        /// </summary>
        /// <param name="scrollAxis">The scroll axis.</param>
        void InitializeScrollAxis(ScrollAxisBase scrollAxis);
    }

    /// <summary>
    /// Implements the <see cref="ILineSizeHost"/> interface for an empty collection
    /// that cannot be modified.
    /// </summary>
    public class EmptyLineSizeHost : ILineSizeHost, IDistancesHost
    {
        /// <summary>
        /// Returns the empty collection.
        /// </summary>
        public static readonly EmptyLineSizeHost Empty = new EmptyLineSizeHost();

        #region ILineSizeHost Members

        double ILineSizeHost.GetDefaultLineSize()
        {
            return 1; 
        }

        int ILineSizeHost.GetLineCount()
        {
            return 0;
        }

        double ILineSizeHost.GetSize(int index, out int repeatValueCount)
        {
            repeatValueCount = 0;
            return 1;
        }

        int ILineSizeHost.GetHeaderLineCount()
        {
            return 0;
        }

        int ILineSizeHost.GetFooterLineCount()
        {
            return 0;
        }

        bool ILineSizeHost.GetHidden(int index, out int repeatValueCount)
        {
            repeatValueCount = 0;
            return false;
        }

        event RangeChangedEventHandler ILineSizeHost.LineSizeChanged
        {
            add { }
            remove { }
        }

        event HiddenRangeChangedEventHandler ILineSizeHost.LineHiddenChanged
        {
            add { }
            remove { }
        }

        event DefaultLineSizeChangedEventHandler ILineSizeHost.DefaultLineSizeChanged
        {
            add { }
            remove { }
        }

        event EventHandler ILineSizeHost.LineCountChanged
        {
            add { }
            remove { }
        }

        event EventHandler ILineSizeHost.HeaderLineCountChanged
        {
            add { }
            remove { }
        }

        event EventHandler ILineSizeHost.FooterLineCountChanged
        {
            add { }
            remove { }
        }

        event LinesInsertedEventHandler ILineSizeHost.LinesInserted
        {
            add { }
            remove { }
        }

        event LinesRemovedEventHandler ILineSizeHost.LinesRemoved
        {
            add { }
            remove { }
        }

        void ILineSizeHost.InitializeScrollAxis(ScrollAxisBase scrollAxis)
        {
            scrollAxis.DefaultLineSize = 1;
            scrollAxis.LineCount = 0;
        }

        #endregion

        #region IDistancesHost Members

        IDistanceCounterCollection IDistancesHost.Distances
        {
            get { return DistanceRangeCounterCollection.Empty; }
        }

        #endregion

        public void Dispose()
        {
            
        }
    }
    

    /// <summary>
    /// An object that implements the <see cref="Distances"/> property.
    /// </summary>
    public interface IDistancesHost : IDisposable
    {
        /// <summary>
        /// Gets or sets the distances.
        /// </summary>
        /// <value>The distances.</value>
        IDistanceCounterCollection Distances { get; }
    }

    /// <summary>
    /// An object that implements the <see cref="GetDistances"/> method.
    /// </summary>
    public interface INestedDistancesHost : IDisposable
    {
        /// <summary>
        /// Gets the nested distances if a line contains a nested lines collection; null otherwise.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        IDistanceCounterCollection GetDistances(int line);
    }

}