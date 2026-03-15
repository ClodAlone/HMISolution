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

using System;
using System.Collections;
using System.Diagnostics;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Formatting
{
    /// <summary>
    /// Manages dynamic formats. Dyniamic formatting can be applied to any part of text.
    /// </summary>
    public class DynamicFormatLayer
        : IDynamicFormatsLayer
        , IDisposable
    {
        #region Fields
        /// <summary>
        /// List of formatting.
        /// </summary>
        private ArrayList m_list;
        /// <summary>
        /// Specifies if this layer is hidden for layers merge.
        /// </summary>
        private readonly bool m_bHidden;
        /// <summary>
        /// Delegate for PhysicalPoint_OffsetChanged method.
        /// </summary>
        private ParsePointParameterChangedEventHandler m_handlerOffsetChanged;
        /// <summary>
        /// Delgate for PointDeleted method.
        /// </summary>
        private CoordinatePointDeletedEventHandler m_handlerPointDeleted;
        /// <summary>
        /// List of formats that should be updated because it's start or end points were deleted.
        /// </summary>
        private IList m_arrFormatsToUpdate = new ArrayList();
        #endregion

        #region Properties
        /// <summary>
        /// Gets delegate for PhysicalPoint_OffsetChanged method.
        /// </summary>
        private ParsePointParameterChangedEventHandler HandlerOffsetChanged
        {
            get
            {
                if (null == m_handlerOffsetChanged)
                {
                    m_handlerOffsetChanged = new ParsePointParameterChangedEventHandler(PhysicalPoint_OffsetChanged);
                }
                return m_handlerOffsetChanged;
            }
        }
        /// <summary>
        /// Gets delegate for PointDeleted method.
        /// </summary>
        private CoordinatePointDeletedEventHandler HandlerPointDeleted
        {
            get
            {
                if (null == m_handlerPointDeleted)
                {
                    m_handlerPointDeleted = new CoordinatePointDeletedEventHandler(PointDeletedInLayer);
                }
                return m_handlerPointDeleted;
            }
        }
        /// <summary>
        /// Gets value that specifies whether this layer is hidden.
        /// </summary>
        public bool Hidden
        {
            get
            {
                return m_bHidden;
            }
        }
        /// <summary>
        /// Gets dynamic formatting by ParsePoint.
        /// </summary>
        public IDynamicFormat this[CoordinatePoint point]
        {
            get
            {
                IDynamicFormat result = null;

                int index = m_list.BinarySearch(point, DynamicFormat.DefaultStartComparer);
                if (index >= 0)
                {
                    result = this[index];
                }
                return result;
            }
        }
        /// <summary>
        /// Gets list of dynamic formatting, that intersepts with given range.
        /// </summary>
        public IList this[CoordinatePoint start, CoordinatePoint end]
        {
            get
            {
                int indexFirst = m_list.BinarySearch(start, DynamicFormat.DefaultEndComparer);
                int indexLast = (start != end) ? m_list.BinarySearch(end, DynamicFormat.DefaultEndComparer) : indexFirst;
                if (indexFirst < 0)
                {
                    indexFirst = ~indexFirst;
                }
                if (indexLast < 0)
                {
                    indexLast = ~indexLast - 1;
                }

                ArrayList list = new ArrayList();
                if (indexLast > indexFirst)
                {
                    IDynamicFormat df = m_list[indexLast] as IDynamicFormat;
                    if (df.Start == end)
                    {
                        indexLast--;
                    }
                }

                if (indexLast >= indexFirst)
                {
                    list.AddRange(m_list.GetRange(indexFirst, indexLast - indexFirst + 1));
                }

                return list;
            }
        }
        /// <summary>
        /// Returns false, list is not thread safe.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Gets count of items in internal collection of formatting.
        /// </summary>
        public int Count
        {
            get
            {
                return m_list.Count;
            }
        }
        /// <summary>
        /// Returns synchronization object.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return m_list.SyncRoot;
            }
        }
        /// <summary>
        /// Gets formatting by index.
        /// </summary>
        public IDynamicFormat this[int index]
        {
            get
            {
                DynamicFormat format = m_list[index] as DynamicFormat;
                return format;
            }
        }
        #endregion

        #region Initialization & Finalization
        /// <summary>
        /// Creates new instance of the class and initializes it.
        /// </summary>
        public DynamicFormatLayer()
            : this(false)
        { }
        /// <summary>
        /// Creates new instance and initializes it.
        /// </summary>
        /// <param name="bHidden">Bool that determines whether layer should be hidden.</param>
        public DynamicFormatLayer(bool bHidden)
        {
            m_list = new ArrayList();
            m_bHidden = bHidden;
        }
        /// <summary>
        /// Unsubscribes start and end points of formatting from events.
        /// </summary>
        public void Dispose()
        {
            for (int i = 0, len = m_list.Count; i < len; i++)
            {
                DynamicFormat frm = (DynamicFormat)m_list[i];
                UnsubscribeStartPointEvents(frm.Start);
                UnsubscribeEndPointEvents(frm.End);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when data is changed whithin the layer.
        /// </summary>
        public event EventHandler DataChanged;
        #endregion

        #region Nonpublic Methods
        /// <summary>
        /// Splits existing format.
        /// </summary>
        /// <param name="index">Index of the format to be splitted.</param>
        /// <param name="divisionPoint">Point of division.</param>
        /// <returns>True if split was successfully done, false if there was no split operation done.</returns>
        protected bool Split(int index, CoordinatePoint divisionPoint)
        {
            if (index < 0 || index > m_list.Count - 1) throw new ArgumentOutOfRangeException(
               "index", index, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_127);
            if (divisionPoint == null) throw new ArgumentNullException("divisionPoint");

#if VERBOSE && DEBUG
			Debug.WriteLine( "Splitting" );
      Debug.WriteLine( "Before Split" );
      Debug.Indent();
#endif

            CheckRanges();
            DynamicFormat format = m_list[index] as DynamicFormat;

            try
            {
                if ((divisionPoint != format.Start) && (divisionPoint < format.End))
                {
                    divisionPoint = GetTrackablePoint(divisionPoint);
                    DynamicFormat newFormat = new DynamicFormat(divisionPoint, format.End, format.Format);
                    newFormat.StartReassigned += new EventHandler(frmt_StartReassigned);
                    newFormat.EndReassigned += new EventHandler(frmt_EndReassigned);
                    newFormat.BeforeStartReassigned += new EventHandler(frmt_BeforeStartReassigned);
                    newFormat.BeforeEndReassigned += new EventHandler(frmt_BeforeEndReassigned);

                    format.End = divisionPoint;

                    SubscribeStartPointEvents(divisionPoint);
                    SubscribeEndPointEvents(divisionPoint);

                    m_list.Insert(index + 1, newFormat);
                    RaiseDataChangedEvent();
                    divisionPoint.PhysicalPoint.ParsePointParameterChanged += new ParsePointParameterChangedEventHandler(PhysicalPoint_OffsetChanged);
                    return true;
                }

                return false;
            }
            finally
            {
#if VERBOSE && DEBUG
				Debug.Unindent();
        Debug.WriteLine( "After Split" );
        Debug.Indent();
#endif

                CheckRanges();

#if VERBOSE && DEBUG
				Debug.Unindent();
#endif
            }
        }
        /// <summary>
        /// Raises DataChanged event.
        /// </summary>
        protected void RaiseDataChangedEvent()
        {
            if (DataChanged != null && !m_bHidden)
            {
                DataChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Raises DataChanges event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhysicalPoint_OffsetChanged(object sender, ParsePointParameterChangedEventArgs e)
        {
            if (e.OffsetChanged)
            {
                RaiseDataChangedEvent();
            }
        }
        /// <summary>
        /// Raises DataChanges event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_PointReset(object sender, EventArgs e)
        {
            RaiseDataChangedEvent();
        }
        /// <summary>
        /// Marks start or end points of formatting as deleted if needed.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lNewOffset"></param>
        private void PointDeletedInLayer(CoordinatePoint point, long lNewOffset)
        {
            DynamicFormat format = null;

            int index = m_list.BinarySearch(point, DynamicFormat.DefaultStartComparer);

            if (index >= 0)
            {
                format = (DynamicFormat)m_list[index];
            }
            else
            {
                index = m_list.BinarySearch(point, DynamicFormat.DefaultEndComparer);
                if (index >= 0)
                {
                    format = (DynamicFormat)m_list[index];
                }
            }

            if (index < 0) return;

            bool bStartDeleted = (format.Start == point);
            bool bEndDeleted = (format.End == point);

            if (bStartDeleted || bEndDeleted)
            {
                if (bStartDeleted)
                {
                    format.StartDeleted = true;
                    if (lNewOffset != -1)
                    {
                        format.StartOffset = lNewOffset;
                    }
                    else
                    {
                        format.StartOffset = point.PhysicalPoint.Offset;
                    }
                }
                if (bEndDeleted)
                {
                    format.EndDeleted = true;
                    if (lNewOffset != -1)
                    {
                        format.EndOffset = lNewOffset;
                    }
                    else
                    {
                        format.StartOffset = point.PhysicalPoint.Offset;
                    }
                }

                m_arrFormatsToUpdate.Add(format);
            }

            RaiseDataChangedEvent();
        }
        /// <summary>
        /// Unsubscribes start point of formatting from events.
        /// </summary>
        /// <param name="startPoint">Start point of formatting that should be unsubscribed.</param>
        private void UnsubscribeStartPointEvents(CoordinatePoint startPoint)
        {
            if (startPoint.IsValid)
            {
                startPoint.PhysicalPoint.ParsePointParameterChanged -= HandlerOffsetChanged;
            }

            startPoint.Deleted -= HandlerPointDeleted;
            startPoint.PointReset -= new EventHandler(start_PointReset);
        }
        /// <summary>
        /// Unsubscribes end point of formatting from events.
        /// </summary>
        /// <param name="endPoint">End point of formatting that should be unsubscribed.</param>
        private void UnsubscribeEndPointEvents(CoordinatePoint endPoint)
        {
            endPoint.PhysicalPoint.ParsePointParameterChanged -= HandlerOffsetChanged;
            endPoint.Deleted -= HandlerPointDeleted;
        }
        /// <summary>
        /// Subscribes start point of formatting for events.
        /// </summary>
        /// <param name="startPoint">Start point of formatting that should be subscribed.</param>
        private void SubscribeStartPointEvents(CoordinatePoint startPoint)
        {
            startPoint.PhysicalPoint.ParsePointParameterChanged += HandlerOffsetChanged;
            startPoint.Deleted += HandlerPointDeleted;
            startPoint.PointReset += new EventHandler(start_PointReset);
        }
        /// <summary>
        /// Subscribes end point of formatting for events.
        /// </summary>
        /// <param name="endPoint">End point of formatting that should be subscribed.</param>
        private void SubscribeEndPointEvents(CoordinatePoint endPoint)
        {
            endPoint.PhysicalPoint.ParsePointParameterChanged += HandlerOffsetChanged;
            endPoint.Deleted += HandlerPointDeleted;
        }
        /// <summary>
        /// Checks whether the specified coordinate point is trackable and creates the trackable one if it is not.
        /// </summary>
        /// <param name="point">CoordinatePoint to check.</param>
        /// <returns>CoordinatePoint specified in point parameter or the newly created one.</returns>
        private CoordinatePoint GetTrackablePoint(CoordinatePoint point)
        {
            if (!point.AttachToEvents)
                return new CoordinatePoint(point, true);
            else
                return point;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Creates new dynamic format object and adds it to the list.
        /// All existing dynamic formatting in specified range will be deleted or trimmed.
        /// </summary>
        /// <param name="start">Starting positions.</param>
        /// <param name="end">End positions.</param>
        /// <param name="format">Format to be added.</param>
        /// <returns>Newly created format.</returns>
        public IDynamicFormat Add(CoordinatePoint start, CoordinatePoint end, ISnippetFormat format)
        {
            if (start == null) throw new ArgumentNullException("start");
            if (end == null) throw new ArgumentNullException("end");
            if (format == null) throw new ArgumentNullException("format");
            if (start.PhysicalPoint == null) throw new ArgumentNullException(
               "start.PhysicalPoint", Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_128);
            if (end.PhysicalPoint == null) throw new ArgumentNullException(
               "end.PhysicalPoint", Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_128);

#if VERBOSE && DEBUG
			Debug.WriteLine( "Adding range" );
      Debug.Indent();
#endif

            Remove(start, end);
            DynamicFormat frmt = null;
            if (start != end)
            {
                start = GetTrackablePoint(start);
                end = GetTrackablePoint(end);

                frmt = new DynamicFormat(start, end, format);

                frmt.StartReassigned += new EventHandler(frmt_StartReassigned);
                frmt.EndReassigned += new EventHandler(frmt_EndReassigned);
                frmt.BeforeStartReassigned += new EventHandler(frmt_BeforeStartReassigned);
                frmt.BeforeEndReassigned += new EventHandler(frmt_BeforeEndReassigned);

                int index = m_list.BinarySearch(frmt.Start, DynamicFormat.DefaultEndComparer);

                if (index < 0)
                    index = ~index;

                m_list.Insert(index, frmt);
                //        m_list.Sort();
                RaiseDataChangedEvent();

                SubscribeStartPointEvents(start);
                SubscribeEndPointEvents(end);
            }

            CheckRanges();

#if VERBOSE && DEBUG
			Debug.Unindent();
#endif

            return frmt;
        }
        /// <summary>
        /// Removes all formatting in specified range.
        /// </summary>
        /// <param name="start">Start of the range.</param>
        /// <param name="end">End of the range.</param>
        public void Remove(CoordinatePoint start, CoordinatePoint end)
        {
            if (start == null)
                throw new ArgumentNullException("start");

            if (end == null)
                throw new ArgumentNullException("end");

            //if( start > end ) throw new ArgumentException(
            //    Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_129 );

#if VERBOSE && DEBUG
			Debug.WriteLine( "Removing range" );
      Debug.Indent();
#endif

            if (start <= end)
            {
                int indexFirst = m_list.BinarySearch(start, DynamicFormat.DefaultEndComparer);

                if (indexFirst < 0)
                    indexFirst = ~indexFirst;
                else
                {
                    // start = GetTrackablePoint(start);
                    if (Split(indexFirst, start))
                        indexFirst++;
                }

                int indexLast = m_list.BinarySearch(end, DynamicFormat.DefaultEndComparer);

                if (indexLast < 0)
                    indexLast = ~indexLast - 1;
                else
                {
                    // end = GetTrackablePoint(end);
                    if (!Split(indexLast, end) && ((DynamicFormat)m_list[indexLast]).Start == end)
                    {
                        indexLast--; // fix for def. OT6306
                    }
                }

                ArrayList list = m_list.GetRange(indexFirst, indexLast - indexFirst + 1);
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        DynamicFormat fmt = list[i] as DynamicFormat;
                        if (fmt != null)
                        {
                            if (fmt.Start != null)
                            {
                                UnsubscribeStartPointEvents(fmt.Start);
                                fmt.Start.Dispose();
                            }
                            if (fmt.End != null)
                            {
                                UnsubscribeEndPointEvents(fmt.End);
                                fmt.End.Dispose();
                            }
                        }
                    }
                }
                m_list.RemoveRange(indexFirst, indexLast - indexFirst + 1);          


                RaiseDataChangedEvent();
                CheckRanges();
            }

#if VERBOSE && DEBUG
			Debug.Unindent();
#endif

        }
        /// <summary>
        /// Checks ranges of formatting.
        /// </summary>
        [Conditional("DEBUG")]
        protected void CheckRanges()
        {

#if VERBOSE && DEBUG
			Debug.WriteLine( "Checking Ranges" );
#endif

            CoordinatePoint lastPoint = null;

            foreach (DynamicFormat format in m_list)
            {

#if VERBOSE && DEBUG
        Debug.WriteLine( string.Format( "Start: {0}; End: {1}", format.Start, format.End ) );
#endif

                if (format.Start <= format.End) 
                //throw new ApplicationException(
                //Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_130 );
                {
                    if (!format.Start.AttachToEvents)
                        throw new ApplicationException("Start point of the dynamic format is not trackable.");

                    if (!format.End.AttachToEvents)
                        throw new ApplicationException("End point of the dynamic format is not trackable.");

                    if (lastPoint != null)
                    {
                        if (lastPoint > format.Start) throw new ApplicationException(
                           Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_131);
                    }

                    lastPoint = format.End;
                }
            }
        }
        /// <summary>
        /// Removes given formatting.
        /// </summary>
        /// <param name="format">Formatting to be deleted.</param>
        public void Remove(IDynamicFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            RemoveAt(IndexOf(format));
        }
        /// <summary>
        /// Removes formatting by given index.
        /// </summary>
        /// <param name="index">Index of the formatting to be removed.</param>
        public void RemoveAt(int index)
        {
            m_list.RemoveAt(index);
            RaiseDataChangedEvent();
        }
        /// <summary>
        /// Copies list to array.
        /// </summary>
        /// <param name="array">Destination array, </param>
        /// <param name="index">Start index in destination array.</param>
        public void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            m_list.CopyTo(array, index);
        }
        /// <summary>
        /// Returns enumerator of the list of formats.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return m_list.GetEnumerator();
        }
        /// <summary>
        /// Returns index of the dynamic formatting.
        /// </summary>
        /// <param name="format">Formatting to be found.</param>
        /// <returns>Index of the formatting in the internal list.</returns>
        public int IndexOf(IDynamicFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return m_list.BinarySearch(format);
        }
        /// <summary>
        /// Checks whether coordinate point belongs to current layer.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <param name="bIncludeAfter">If true, point at the beginning of region is considered as belonging to layer.</param>
        /// <param name="bIncludeBefore">If true, point at the end of region is considered as belonging to layer.</param>
        /// <returns>Bool indicating whether given point belongs to current layer.</returns>
        public bool PointInLayer(CoordinatePoint point, bool bIncludeBefore, bool bIncludeAfter)
        {
            if (null != point)
            {
                for (int i = 0, len = m_list.Count; i < len; i++)
                {
                    DynamicFormat format = (DynamicFormat)m_list[i];
                    if ((format.Start < point || (format.Start == point && bIncludeBefore))
                        && (format.End > point || (format.End == point && bIncludeAfter))) return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Updates state of start and end points of each dynamic formatting.
        /// </summary>
        public void UpdateFormats()
        {
            for (int i = 0, len = m_arrFormatsToUpdate.Count; i < len; i++)
            {
                DynamicFormat frm = (DynamicFormat)m_arrFormatsToUpdate[i];

                if (!frm.UpdateState())
                {
                    m_list.Remove(frm);
                }
            }

            m_arrFormatsToUpdate.Clear();
        }
        /// <summary>
        /// Clears all dynamic formatting.
        /// </summary>
        public void Clear()
        {
            m_arrFormatsToUpdate.Clear();
            m_list.Clear();
        }
        #endregion

        #region Class Event Handlers
        /// <summary>
        /// Subscribes start point of format for events.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        private void frmt_StartReassigned(object sender, EventArgs e)
        {
            SubscribeStartPointEvents(((DynamicFormat)sender).Start);
        }
        /// <summary>
        /// Subscribes end point of format for events.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        private void frmt_EndReassigned(object sender, EventArgs e)
        {
            SubscribeEndPointEvents(((DynamicFormat)sender).End);
        }
        /// <summary>
        /// Unsubscribes start point of format from events.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        private void frmt_BeforeStartReassigned(object sender, EventArgs e)
        {
            UnsubscribeStartPointEvents(((DynamicFormat)sender).Start);
        }
        /// <summary>
        /// Unsubscribes end point of format from events.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        private void frmt_BeforeEndReassigned(object sender, EventArgs e)
        {
            UnsubscribeEndPointEvents(((DynamicFormat)sender).End);
        }
        #endregion
    }
}