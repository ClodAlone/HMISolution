#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Syncfusion.Windows.Controls.Scroll
{
    /// <summary>
    /// A collection that manages lines with varying height and hidden state. 
    /// It has properties for header and footer lines, total line count, default
    /// size of a line and also lets you add nested collections.
    /// </summary>
    public class LineSizeCollection : IPaddedEditableLineSizeHost, IDistancesHost, INestedDistancesHost, IDisposable
    {
        int lineCount;
        double defaultLineSize = 1.0;
        SortedRangeValueList<double> lineSizes = new SortedRangeValueList<double>(-1);
        SortedRangeValueList<bool> lineHidden = new SortedRangeValueList<bool>();
        Dictionary<int, LineSizeCollection> lineNested = new Dictionary<int, LineSizeCollection>();
        private int headerLineCount;
        private int footerLineCount;
        IDistanceCounterCollection distances;
        
        /// <summary>
        /// Returns an empty collection.
        /// </summary>
        public static readonly LineSizeCollection Empty = new LineSizeCollection();

        /// <summary>
        /// Gets the total extent which is the total of all line sizes. Note: This propert only 
        /// works if the DistanceCollection has been setup for pixel scrolling; otherwise it returns
        /// double.NaN.
        /// </summary>
        /// <value>The total extent or double.NaN.</value>
        public virtual double TotalExtent
        {
            get
            {
                // This only works if the DistanceCollection has been setup for pixel scrolling.
                if (Distances.DefaultDistance == defaultLineSize)
                    return distances.TotalDistance;

                return double.NaN;
            }
        }

        #region IPaddedEditableLineSizeHost Members
        private double paddingDistance = 0d;
        public double PaddingDistance
        {
            get { return this.paddingDistance; }
            set
            {
                if (this.paddingDistance != value)
                {
                    this.paddingDistance = value;

                    if (this.IsSuspendUpdates) 
                    {
                        return;
                    }

                    this.distances = new DistanceRangeCounterCollection(this.PaddingDistance);
                    InitializeDistances();
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the header line count.
        /// </summary>
        /// <value>The header line count.</value>
        public int HeaderLineCount
        {
            get
            {
                return this.headerLineCount;
            }
            set
            {
                if (this.headerLineCount != value)
                {
                    this.headerLineCount = value;
                    if (HeaderLineCountChanged != null)
                        HeaderLineCountChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the footer line count.
        /// </summary>
        /// <value>The footer line count.</value>
        public int FooterLineCount
        {
            get
            {
                return this.footerLineCount;
            }
            set
            {
                if (this.footerLineCount != value)
                {
                    this.footerLineCount = value;
                    if (FooterLineCountChanged != null)
                        FooterLineCountChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the line count.
        /// </summary>
        /// <value>The line count.</value>
        public int LineCount
        {
            get
            {
                return lineCount;
            }
            set
            {
                if (lineCount != value)
                {
                    lineCount = value;

                    if (IsSuspendUpdates) return;

                    if (distances != null)
                        distances.Count = lineCount;
                    
                    if (LineCountChanged != null)
                        LineCountChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the default size of lines.
        /// </summary>
        /// <value>The default size of lines.</value>
        public double DefaultLineSize
        {
            get
            {
                return defaultLineSize;
            }
            set
            {
                if (defaultLineSize != value)
                {
                    double SavedValue = defaultLineSize;
                    defaultLineSize = value;

                    if (IsSuspendUpdates) return;

                    if (distances != null)
                        InitializeDistances();

                    if (DefaultLineSizeChanged != null)
                        DefaultLineSizeChanged(this, new DefaultLineSizeChangedEventArgs(SavedValue, defaultLineSize));
                }
            }
        }

        /// <summary>
        /// Sets the line size for a range.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="size">The size.</param>
        public void SetRange(int from, int to, double size)
        {
            /*if (lineNested.Count > 0)
            {
                for (int n = from; n <= to; n++)
                {
                    if (lineNested.ContainsKey(n))
                        throw new InvalidOperationException("Cannot change size of a nested LineSizeCollection.");
                }
            }*/
            int count;
            double savevalue = GetRange(from, out count);
            lineSizes.SetRange(from, to - from + 1, size);

            if (IsSuspendUpdates) return;

            // DistancesLineHiddenChanged checks both hidden state and sizes together ...
            if (distances != null)
                DistancesUtil.DistancesLineHiddenChanged(distances, this, from, to);

            if (LineSizeChanged != null)
                LineSizeChanged(this, new RangeChangedEventArgs(from, to, savevalue, size));
        }

        /// <summary>
        /// Gets or sets the line size at the specified index.
        /// </summary>
        /// <value></value>
        public double this[int index]
        {
            get
            {
                int repeatValueCount;
                return GetRange(index, out repeatValueCount);
            }
            set
            {
                SetRange(index, index, value);
            }
        }

        double GetRange(int index, out int repeatValueCount)
        {
            repeatValueCount = 1;

            if (lineNested.ContainsKey(index))
                return lineNested[index].TotalExtent;

            bool hide = lineHidden.GetRange(index, out repeatValueCount);
            if (hide)
                return 0.0;

            double size = lineSizes.GetRange(index, out repeatValueCount);
            Debug.Assert(size != -2, "-2 indicates a nested collection. Why is it not in lineNested?");

            if (size >= 0)
                return size;
            
            return DefaultLineSize;
        }

        /// <summary>
        /// Sets the hidden state for a range of lines.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="hide">if set to <c>true</c> hide the lines.</param>
        public void SetHidden(int from, int to, bool hide)
        {
            lineHidden.SetRange(from, to - from + 1, hide);

            if (IsSuspendUpdates) return;

            // DistancesLineHiddenChanged checks both hidden state and sizes together ...
            if (distances != null)
                DistancesUtil.DistancesLineHiddenChanged(distances, this, from, to);

            if (LineHiddenChanged != null)
                LineHiddenChanged(this, new HiddenRangeChangedEventArgs(from, to, hide));
        }

        /// <summary>
        /// Gets whether the host supports nesting.
        /// </summary>
        /// <value></value>
        public bool SupportsNestedLines 
        { 
            get { return true; } 
        }


        /// <summary>
        /// Gets the nested lines.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public IEditableLineSizeHost GetNestedLines(int index)
        {
            if (lineNested.ContainsKey(index))
                return lineNested[index];

            return null;
        }

        /// <summary>
        /// Sets the nested lines. 
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="nestedLines">The nested lines. If parameter is null the line will be converted to a normal (not nested) line with default line size.</param>
        public void SetNestedLines(int index, IEditableLineSizeHost nestedLines)
        {
            if (nestedLines != null)
            {
                lineSizes[index] = -2; // -1 indicates default value, -2 indicates nested.
                lineNested[index] = (LineSizeCollection) nestedLines;
            }
            else
            {
                lineSizes[index] = -1; // -1 indicates default value, -2 indicates nested.
                lineNested.Remove(index);
            }

            if (IsSuspendUpdates) return;

            if (distances != null)
                DistancesUtil.DistancesLineSizeChanged(distances, this, index, index);

            if (LineSizeChanged != null)
                LineSizeChanged(this, new RangeChangedEventArgs(index, index));
        }

        /// <summary>
        /// Reset the line to become a normal (not nested) line with default line size.
        /// </summary>
        public void ResetNestedLines()
        {
            foreach (var item in lineNested)
            {
                lineSizes[item.Key] = -1;
            }
            lineNested.Clear();
        }


        /// <summary>
        /// Reset the line to become a normal (not nested) line with default line size.
        /// </summary>
        /// <param name="index">The index.</param>
        public void ResetNestedLines(int index)
        {
            SetNestedLines(index, null);
        }

        #region ILineSizeHost Members

        /// <summary>
        /// Returns the default line size.
        /// </summary>
        /// <returns></returns>
        public double GetDefaultLineSize()
        {
            return DefaultLineSize;
        }

        /// <summary>
        /// Returns the line count.
        /// </summary>
        /// <returns></returns>
        public int GetLineCount()
        {
            return LineCount;
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="repeatValueCount">The number of subsequent values with same size.</param>
        /// <returns></returns>
        public virtual double GetSize(int index, out int repeatValueCount)
        {
            repeatValueCount = 1;
            IEditableLineSizeHost nested = GetNestedLines(index);
            if (nested != null)
                return nested.TotalExtent;

            return GetRange(index, out repeatValueCount);
        }

        /// <summary>
        /// Gets the header line count.
        /// </summary>
        /// <returns></returns>
        public int GetHeaderLineCount()
        {
            return HeaderLineCount;
        }

        /// <summary>
        /// Gets the footer line count.
        /// </summary>
        /// <returns></returns>
        public int GetFooterLineCount()
        {
            return FooterLineCount;
        }

        /// <summary>
        /// Gets the hidden state for a line.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="repeatValueCount">The number of subsequent lines with same state.</param>
        /// <returns></returns>
        public bool GetHidden(int index, out int repeatValueCount)
        {
            return lineHidden.GetRange(index, out repeatValueCount);
        }

        /// <summary>
        /// Initializes the scroll axis.
        /// </summary>
        /// <param name="scrollAxis">The scroll axis.</param>
        public void InitializeScrollAxis(ScrollAxisBase scrollAxis)
        {
            PixelScrollAxis pixelScrollAxis = scrollAxis as PixelScrollAxis;
            if (lineNested.Count > 0 && pixelScrollAxis == null)
                throw new InvalidOperationException("When you have nested line collections you need to use PixelScrolling!");

            //scrollAxis.distances.Clear();
            scrollAxis.DefaultLineSize = DefaultLineSize;
            scrollAxis.LineCount = LineCount;

            foreach (RangeValuePair<double> entry in lineSizes)
            {
                if (entry.Value != -2)
                    scrollAxis.SetLineSize(entry.Start, entry.End, entry.Value < 0 ? DefaultLineSize : entry.Value);
            }

            foreach (KeyValuePair<int, LineSizeCollection> entry in lineNested)
                pixelScrollAxis.SetNestedLines(entry.Key, entry.Value.Distances);

            foreach (RangeValuePair<bool> entry in lineHidden)
                scrollAxis.SetLineHiddenState(entry.Start, entry.End, entry.Value);
        }

        /// <summary>
        /// Occurs when a lines size was changed.
        /// </summary>
        public event RangeChangedEventHandler LineSizeChanged;

        /// <summary>
        /// Occurs when a lines hidden state changed.
        /// </summary>
        public event HiddenRangeChangedEventHandler LineHiddenChanged;

        /// <summary>
        /// Occurs when the default line size changed.
        /// </summary>
        public event DefaultLineSizeChangedEventHandler DefaultLineSizeChanged;

        /// <summary>
        /// Occurs when the line count was changed.
        /// </summary>
        public event EventHandler LineCountChanged;

        /// <summary>
        /// Occurs when the header line count was changed.
        /// </summary>
        public event EventHandler HeaderLineCountChanged;

        /// <summary>
        /// Occurs when the footer line count was changed.
        /// </summary>
        public event EventHandler FooterLineCountChanged;

        /// <summary>
        /// Occurs when lines were inserted.
        /// </summary>
        public event LinesInsertedEventHandler LinesInserted;

        /// <summary>
        /// Occurs when lines were removed.
        /// </summary>
        public event LinesRemovedEventHandler LinesRemoved;


        #endregion

        #region IDistancesHost Members

        /// <summary>
        /// Gets or sets the distances.
        /// </summary>
        /// <value>The distances.</value>
        public IDistanceCounterCollection Distances
        {
            get
            {
                if (distances == null)
                {
                    distances = new DistanceRangeCounterCollection(this.PaddingDistance);
                    InitializeDistances();
                }
                return distances;
            }
        }

        private void InitializeDistances()
        {
            distances.Clear();
            distances.Count = GetLineCount();
            distances.DefaultDistance = DefaultLineSize;
            
            foreach (KeyValuePair<int, LineSizeCollection> entry in lineNested)
            {
                int repeatSizeCount;
                bool hide = GetHidden(entry.Key, out repeatSizeCount);
                if (hide)
                    distances.SetNestedDistances(entry.Key, null);
                else
                    distances.SetNestedDistances(entry.Key, entry.Value.Distances);
            }

            foreach (RangeValuePair<double> entry in lineSizes)
            {
                if (entry.Value != -2)
                    distances.SetRange(entry.Start, entry.End, entry.Value < 0 ? DefaultLineSize : entry.Value);
            }

            foreach (RangeValuePair<bool> entry in lineHidden)
            {
                if (entry.Value)
                    distances.SetRange(entry.Start, entry.End, 0);
            }
        }

        #endregion

        #region INestedDistancesHost Members

        /// <summary>
        /// Gets the nested distances if a line contains a nested lines collection; null otherwise.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public IDistanceCounterCollection GetDistances(int line)
        {
            IDistancesHost nestedLines = (IDistancesHost)GetNestedLines(line);
            if (nestedLines != null)
                return nestedLines.Distances;
            return null;
        }

        #endregion

        /// <summary>
        /// Inserts lines in the collection and raises the <see cref="LinesInserted"/> event.
        /// </summary>
        /// <param name="insertAtLine">The index of the first line to insert.</param>
        /// <param name="count">The count.</param>
        public void InsertLines(int insertAtLine, int count)
        {
            InsertLines(insertAtLine, count, null);
        }

        /// <summary>
        /// Inserts lines in the collection and raises the <see cref="LinesInserted"/> event.
        /// </summary>
        /// <param name="insertAtLine">The index of the first line to insert.</param>
        /// <param name="count">The count.</param>
        /// <param name="movelines">A container with saved state from a preceeding <see cref="RemoveLines"/> call when lines should be moved. When it is null empty lines with default size are inserted.</param>
        public void InsertLines(int insertAtLine, int count, IEditableLineSizeHost movelines)
        {
            var moveLines = (LineSizeCollection)movelines;
            lineSizes.Insert(insertAtLine, count, moveLines == null ? null : moveLines.lineSizes);
            lineHidden.Insert(insertAtLine, count, moveLines == null ? null : moveLines.lineHidden);

            Dictionary<int, LineSizeCollection> _lineNested = lineNested;
            lineNested = new Dictionary<int, LineSizeCollection>();

            foreach (KeyValuePair<int, LineSizeCollection> entry in _lineNested)
            {
                if (entry.Key >= insertAtLine)
                    lineNested.Add(entry.Key + count, entry.Value);
                else
                    lineNested.Add(entry.Key, entry.Value);
            }

            if (moveLines != null)
            {
                foreach (KeyValuePair<int, LineSizeCollection> entry in moveLines.lineNested)
                {
                    lineNested.Add(entry.Key + insertAtLine, entry.Value);
                }
            }

            lineCount += count;

            if (IsSuspendUpdates) return;

            if (distances != null)
                DistancesUtil.OnInserted(distances, this, insertAtLine, count);

            if (LinesInserted != null)
                LinesInserted(this, new LinesInsertedEventArgs(insertAtLine, count));
        }

        /// <summary>
        /// Removes lines from the collection and raises the <see cref="LinesRemoved"/> event.
        /// </summary>
        /// <param name="removeAtLine">The index of the first line to be removed.</param>
        /// <param name="count">The count.</param>
        public void RemoveLines(int removeAtLine, int count)
        {
            RemoveLines(removeAtLine, count, null);
        }

        /// <summary>
        /// Removes lines from the collection and raises the <see cref="LinesRemoved"/> event.
        /// </summary>
        /// <param name="removeAtLine">The index of the first line to be removed.</param>
        /// <param name="count">The count.</param>
        /// <param name="movelines">A container to save state for a subsequent <see cref="InsertLines"/> call when lines should be moved.</param>
        public void RemoveLines(int removeAtLine, int count, IEditableLineSizeHost movelines)
        {
            LineSizeCollection moveLines = (LineSizeCollection)movelines;
            lineSizes.Remove(removeAtLine, count, moveLines == null ? null : moveLines.lineSizes);
            lineHidden.Remove(removeAtLine, count, moveLines == null ? null : moveLines.lineHidden);

            Dictionary<int, LineSizeCollection> _lineNested = lineNested;
            lineNested = new Dictionary<int, LineSizeCollection>();
 
            foreach (KeyValuePair<int, LineSizeCollection> entry in _lineNested)
            {
                if (entry.Key >= removeAtLine)
                {
                    if (entry.Key >= removeAtLine + count)
                        lineNested.Add(entry.Key - count, entry.Value);
                    else if (moveLines != null)
                        moveLines.lineNested.Add(entry.Key - removeAtLine, entry.Value);
                }
                else
                    lineNested.Add(entry.Key, entry.Value);
            }

            lineCount -= count;

            if (IsSuspendUpdates) return;

            if (distances != null)
                distances.Remove(removeAtLine, count);

            if (LinesRemoved != null)
                LinesRemoved(this, new LinesRemovedEventArgs(removeAtLine, count));
        }

        #region IEditableLineSizeHost Members

        /// <summary>
        /// Gets whether the host supports inserting and removing lines.
        /// </summary>
        /// <value></value>
        public bool SupportsInsertRemove
        {
            get { return true; }
        }

        /// <summary>
        /// Creates the object which holds temporary state when moving lines.
        /// </summary>
        /// <returns></returns>
        public IEditableLineSizeHost CreateMoveLines()
        {
            return new LineSizeCollection();
        }

        #endregion

        public void ResetHiddenState()
        {
            this.lineHidden = new SortedRangeValueList<bool>();
        }

        /// <summary>
        /// Initialize the collection with a pattern of hidden lines. 
        /// </summary>
        /// <param name="start">The index of the first line where the pattern should be
        /// started to be applied.</param>
        /// <param name="lineCount">The pattern is applied up to until the lineCount given. 
        /// The last initialized line is at index lineCount-1.</param>
        /// <param name="values">The pattern that is applied repeatedly.</param>
        public void SetHiddenInterval(int start, int lineCount, bool[] values)
        {
            SuspendUpdates();

            lineHidden = new SortedRangeValueList<bool>();

            for (int index = start; index < lineCount; index += values.Length)
            {
                for (int n = 0; n < values.Length; n++)
                {
                    if (n + index < lineCount)
                        lineHidden[index + n] = values[n];
                }
            }

            ResumeUpdates();
        }

        public void SetHiddenIntervalWithState(int start, int lineCount, bool[] values)
        {
            this.SuspendUpdates();

            for (int index = start; index < lineCount; index += values.Length)
            {
                for (int n = 0; n < values.Length; n++)
                {
                    if (n + index < lineCount)
                    {
                        lineHidden[index + n] = values[n];
                    }
                }
            }

            this.ResumeUpdates();
        }

        /// <summary>
        /// Set the hidden state all at once in one operation. Use this method if you want to change the hidden
        /// state of many rows at once since this will be much faster instead of individually setting rows hidden.
        /// </summary>
        /// <param name="values">The new hidden state for rows. </param>
        public void SetHiddenState(bool[] values)
        {
            SuspendUpdates();

            lineHidden = new SortedRangeValueList<bool>();

            int count = Math.Min(lineCount, values.Length);
            for (int index = 0; index < count; index ++)
            {
                lineHidden[index] = values[index];
            }

            ResumeUpdates();
        }

        int isSuspendUpdates = 0;

        bool IsSuspendUpdates 
        { 
            get { return isSuspendUpdates > 0; } 
        }

        public void SuspendUpdates()
        {
            isSuspendUpdates++;
        }

        public void ResumeUpdates()
        {
            this.ResumeUpdates(false);
        }

        public void ResumeUpdates(bool canreset)
        {
            isSuspendUpdates--;
            if (isSuspendUpdates == 0)
            {
                if (distances != null)
                {
                    if (canreset)
                        distances = new DistanceRangeCounterCollection(this.PaddingDistance);
                    InitializeDistances();
                    //distances.Clear();
                    //distances.Count = lineCount;
                    //DistancesUtil.DistancesLineHiddenChanged(distances, this, 0, lineCount-1);
                }


                if (LineHiddenChanged != null)
                    LineHiddenChanged(this, new HiddenRangeChangedEventArgs(0, lineCount - 1, false));
            }
        }

        class LineSizeCollectionDisposable : IDisposable
        {
            private LineSizeCollection lineSizeCollection = null;
            public LineSizeCollectionDisposable(LineSizeCollection lineSizeCollection)
            {
                this.lineSizeCollection = lineSizeCollection;
                this.lineSizeCollection.SuspendUpdates();
            }

            #region IDisposable Members

            public void Dispose()
            {
                this.lineSizeCollection.ResumeUpdates();
                this.lineSizeCollection = null;
            }

            #endregion
        }

        public IDisposable DeferRefresh()
        {
            return new LineSizeCollectionDisposable(this);
        }

        public void Dispose()
        {
            if (lineNested != null)
                lineNested.Clear();
            if (distances != null)
                distances.Clear();
            //lineSizes = null;
            //lineHidden = null;
            //lineNested = null;
            //distances = null;
            LineSizeChanged = null;
            LineHiddenChanged = null;
            DefaultLineSizeChanged = null;
            LineCountChanged = null;
            HeaderLineCountChanged = null;
            FooterLineCountChanged = null;
            LinesInserted = null;
            LinesRemoved = null;
        }
    }



    /// <summary>
    /// An empty LineSizeCollection.
    /// </summary>
    public class EmptyLineSizeCollection : LineSizeCollection, ILineSizeHost, IDistancesHost
    {
        /// <summary>
        /// Returns an empty collection.
        /// </summary>
        public static new readonly EmptyLineSizeCollection Empty = new EmptyLineSizeCollection();

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
    }
    
    class DistancesUtil
    {
        public static int GetRangeToHelper(int n, int to, int repeatSizeCount)
        {
            if (repeatSizeCount == int.MaxValue)
                return to;
            return Math.Min(to, n + repeatSizeCount - 1);
        }

        public static void OnInserted(IDistanceCounterCollection distances, ILineSizeHost linesHost, int insertAt, int count)
        {
            distances.Insert(insertAt, count);
            int to = insertAt + count - 1;
            int repeatSizeCount;

            // Set line sizes
            for (int index = insertAt; index <= to; index++)
            {
                double size = linesHost.GetSize(index, out repeatSizeCount);
                if (size != distances.DefaultDistance)
                {
                    int rangeTo = GetRangeToHelper(index, to, repeatSizeCount);
                    distances.SetRange(index, rangeTo, size);
                    index = rangeTo;
                }
            }

            // Also check for hidden rows and reset line sizes for them.
            for (int index = insertAt; index <= to; index++)
            {
                bool hide = linesHost.GetHidden(index, out repeatSizeCount);
                if (hide)
                {
                    int rangeTo = GetRangeToHelper(index, to, repeatSizeCount);
                    distances.SetRange(index, rangeTo, 0.0);
                    index = rangeTo;
                }
            }
        }

        public static void DistancesLineHiddenChanged(IDistanceCounterCollection distances, ILineSizeHost linesHost, int from, int to)
        {
            INestedDistancesHost ndh = linesHost as INestedDistancesHost;
            for (int n = from; n <= to; n++)
            {
                int repeatSizeCount;
                bool hide = linesHost.GetHidden(n, out repeatSizeCount);

                if (ndh == null || ndh.GetDistances(n) == null)
                {
                    int rangeTo = GetRangeToHelper(n, to, repeatSizeCount);
                    if (hide)
                        distances.SetRange(n, rangeTo, 0.0);
                    else
                        DistancesLineSizeChanged(distances, linesHost, n, rangeTo);
                    n = rangeTo;
                }
                else
                {
                    if (hide)
                    {
                        distances.SetNestedDistances(n, null);
                    }
                    else
                    {
                        distances.SetNestedDistances(n, ndh.GetDistances(n));
                    }
                }
            }
        }

        public static void DistancesLineSizeChanged(IDistanceCounterCollection distances, ILineSizeHost linesHost, int from, int to)
        {
            INestedDistancesHost ndh = linesHost as INestedDistancesHost;
            for (int n = from; n <= to; n++)
            {
                if (ndh == null || ndh.GetDistances(n) == null)
                {
                    int repeatSizeCount;
                    double size = linesHost.GetSize(n, out repeatSizeCount);
                    int rangeTo = GetRangeToHelper(n, to, repeatSizeCount);
                    distances.SetRange(n, rangeTo, size);
                    n = rangeTo;
                }
                else
                    distances.SetNestedDistances(n, ndh.GetDistances(n));
            }
        }


    }


    public class LineSizeUtil
    {
        public static double GetTotal(ILineSizeHost lines, int from, int to)
        {
            int repeatCount;
            int index = from;
            double total = 0;

            while (index <= to)
            {
                double w = lines.GetSize(index, out repeatCount);
                repeatCount = Math.Min(to - index + 1, repeatCount);
                total += w * repeatCount;
                index += repeatCount;
            }

            return total;
        }

        public static double[] GetRange(ILineSizeHost lines, int from, int to)
        {
            int count = to - from + 1;
            double[] values = new double[count];

            int repeatCount;
            int index = from;
            int n = 0;

            while (index <= to)
            {
                double w = lines.GetSize(index, out repeatCount);
                repeatCount = Math.Min(to - index + 1, repeatCount);
                for (int i = 0; i < repeatCount; i++)
                    values[n++] = w;
                index += repeatCount;
            }

            return values;
        }

        public static void SetRange(IEditableLineSizeHost lines, int from, int to, double[] values)
        {
            int index = from;
            int n = 0;

            while (index <= to)
            {
                lines.SetRange(index, index, values[n++]);
                index++;
            }
        }
    }

}
