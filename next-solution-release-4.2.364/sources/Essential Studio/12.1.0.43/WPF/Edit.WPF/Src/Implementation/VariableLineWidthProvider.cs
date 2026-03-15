#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class VariableLineWidthProvider : IEditableLineSizeHost
    {
        private EditScrollControl _presenter;
        private LineSizeCollection linesizecollection;
        private bool isRow = true;

        /// <summary>
        ///
        /// </summary>
        /// <param name="presenter"></param>
        /// <param name="collection"></param>
        /// <param name="isRowItem"></param>
        public VariableLineWidthProvider(EditScrollControl presenter, LineSizeCollection collection, bool isRowItem)
        {
            linesizecollection = collection;
            _presenter = presenter;
            isRow = isRowItem;
        }

        #region IEditableLineSizeHost Members

        /// <summary>
        ///
        /// </summary>
        public double TotalExtent
        {
            get { return linesizecollection.TotalExtent; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool SupportsNestedLines
        {
            get { return linesizecollection.SupportsNestedLines; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IEditableLineSizeHost GetNestedLines(int index)
        {
            return linesizecollection.GetNestedLines(index);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nestedLines"></param>
        public void SetNestedLines(int index, IEditableLineSizeHost nestedLines)
        {
            linesizecollection.SetNestedLines(index, nestedLines);
        }

        /// <summary>
        ///
        /// </summary>
        public bool SupportsInsertRemove
        {
            get { return linesizecollection.SupportsInsertRemove; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="insertAtLine"></param>
        /// <param name="count"></param>
        /// <param name="moveLines"></param>
        public void InsertLines(int insertAtLine, int count, IEditableLineSizeHost moveLines)
        {
            linesizecollection.InsertLines(insertAtLine, count, moveLines);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="removeAtLine"></param>
        /// <param name="count"></param>
        /// <param name="moveLines"></param>
        public void RemoveLines(int removeAtLine, int count, IEditableLineSizeHost moveLines)
        {
            linesizecollection.RemoveLines(removeAtLine, count, moveLines);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEditableLineSizeHost CreateMoveLines()
        {
            return linesizecollection.CreateMoveLines();
        }

        /// <summary>
        ///
        /// </summary>
        public double DefaultLineSize
        {
            get
            {
                return linesizecollection.DefaultLineSize;
            }

            set
            {
                linesizecollection.DefaultLineSize = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int FooterLineCount
        {
            get
            {
                return linesizecollection.FooterLineCount;
            }

            set
            {
                linesizecollection.FooterLineCount = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int HeaderLineCount
        {
            get
            {
                return linesizecollection.HeaderLineCount;
            }

            set
            {
                linesizecollection.HeaderLineCount = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int LineCount
        {
            get
            {
                return linesizecollection.LineCount;
            }

            set
            {
                linesizecollection.LineCount = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="hide"></param>
        public void SetHidden(int from, int to, bool hide)
        {
            linesizecollection.SetHidden(from, to, hide);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="size"></param>
        public void SetRange(int from, int to, double size)
        {
            linesizecollection.SetRange(from, to, size);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[int index]
        {
            get
            {
                return linesizecollection[index];
            }

            set
            {
                linesizecollection[index] = value;
            }
        }

        #endregion IEditableLineSizeHost Members

        #region ILineSizeHost Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public double GetDefaultLineSize()
        {
            return linesizecollection.GetDefaultLineSize();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public int GetLineCount()
        {
            return linesizecollection.GetLineCount();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="repeatValueCount"></param>
        /// <returns></returns>
        public double GetSize(int index, out int repeatValueCount)
        {
            repeatValueCount = 1;

            if (this.isRow)
            {
                return linesizecollection.DefaultLineSize;
            }

            return linesizecollection[index];
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public int GetHeaderLineCount()
        {
            return linesizecollection.GetHeaderLineCount();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public int GetFooterLineCount()
        {
            return linesizecollection.GetFooterLineCount();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="repeatValueCount"></param>
        /// <returns></returns>
        public bool GetHidden(int index, out int repeatValueCount)
        {
            return linesizecollection.GetHidden(index, out repeatValueCount);
        }

        /// <summary>
        ///
        /// </summary>
        public event RangeChangedEventHandler LineSizeChanged
        {
            add { linesizecollection.LineSizeChanged += value; }
            remove { linesizecollection.LineSizeChanged -= value; }
        }

        /// <summary>
        ///
        /// </summary>
        public event HiddenRangeChangedEventHandler LineHiddenChanged
        {
            add { linesizecollection.LineHiddenChanged += value; }
            remove { linesizecollection.LineHiddenChanged -= value; }
        }

        /// <summary>
        ///
        /// </summary>
        public event DefaultLineSizeChangedEventHandler DefaultLineSizeChanged
        {
            add { linesizecollection.DefaultLineSizeChanged += value; }
            remove { linesizecollection.DefaultLineSizeChanged -= value; }
        }

        /// <summary>
        ///
        /// </summary>
        public event EventHandler LineCountChanged
        {
            add { linesizecollection.LineCountChanged += value; }
            remove { linesizecollection.LineCountChanged -= value; }
        }

        /// <summary>
        ///
        /// </summary>
        public event EventHandler HeaderLineCountChanged
        {
            add { linesizecollection.HeaderLineCountChanged += value; }
            remove { linesizecollection.HeaderLineCountChanged -= value; }
        }

        /// <summary>
        ///
        /// </summary>
        public event EventHandler FooterLineCountChanged
        {
            add { linesizecollection.FooterLineCountChanged += value; }
            remove { linesizecollection.FooterLineCountChanged -= value; }
        }

        /// <summary>
        ///
        /// </summary>
        public event LinesInsertedEventHandler LinesInserted
        {
            add { linesizecollection.LinesInserted += value; }
            remove { linesizecollection.LinesInserted -= value; }
        }

        /// <summary>
        ///
        /// </summary>
        public event LinesRemovedEventHandler LinesRemoved
        {
            add { linesizecollection.LinesRemoved += value; }
            remove { linesizecollection.LinesRemoved -= value; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="scrollAxis"></param>
        public void InitializeScrollAxis(ScrollAxisBase scrollAxis)
        {
            linesizecollection.InitializeScrollAxis(scrollAxis);
        }

        #endregion ILineSizeHost Members

        /// <summary>
        ///
        /// </summary>
        public EditControl ParentControl
        {
            get
            {
                return (EditControl)VisualUtils.FindAncestor(_presenter, typeof(EditControl));
            }
        }

        #region IDisposable Members

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion IDisposable Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class ItemsRowHeightsEventArgs : SyncfusionCancelEventArgs
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="colIndex"></param>
        /// <param name="isRow"></param>
        public ItemsRowHeightsEventArgs(int colIndex, bool isRow)
        {
            this.Index = colIndex;
            this.Cancel = false;
            this.IsRow = isRow;
        }

        /// <summary>
        ///
        /// </summary>
        public int Index
        {
            get;
            private set;
        }

        /// <summary>
        ///
        /// </summary>
        public double Value
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsRow
        {
            get;
            set;
        }
    }
}