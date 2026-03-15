#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Primitives
{
    /// <summary>
    /// Source of data
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public abstract class DataSource : ILoopingSelectorDataSource
    {
        #region Variables

        private DateTimeWrapper _selectedItem;

        /// <summary>
        /// Gets or sets the item that has been selected
        /// </summary>
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (value != _selectedItem)
                {
                    DateTimeWrapper valueWrapper = (DateTimeWrapper)value;
                    if ((null == valueWrapper) || (null == _selectedItem) || (valueWrapper.DateTime != _selectedItem.DateTime))
                    {
                        object previousSelectedItem = _selectedItem;
                        _selectedItem = valueWrapper;
                        var handler = SelectionChanged;
                        if (null != handler)
                        {
                            handler(this, new SelectionChangedEventArgs(new[] { previousSelectedItem }, new object[] { _selectedItem }));
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the next value.
        /// </summary>
        /// <param name="relativeTo"></param>
        /// <returns></returns>
        public object GetNext(object relativeTo)
        {
            DateTime? next = GetRelativeTo(((DateTimeWrapper)relativeTo).DateTime, 1);
            return next.HasValue ? new DateTimeWrapper(next.Value) : null;
        }

        /// <summary>
        /// Gets the previous value.
        /// </summary>
        /// <param name="relativeTo"></param>
        /// <returns></returns>
        public object GetPrevious(object relativeTo)
        {
            DateTime? next = GetRelativeTo(((DateTimeWrapper)relativeTo).DateTime, -1);
            return next.HasValue ? new DateTimeWrapper(next.Value) : null;
        }

        /// <summary>
        /// Gets the updated minimum date time.
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="minuteinterval"></param>
        /// <param name="secondinterval"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public int UpdateMinimumDateTime(DateTime relativeDate, int minuteinterval, int secondinterval, out DateTime datetime)
        {
            DataSource source = this;
            int maximumcount;
            datetime = relativeDate;
            if (source is DayDataSource)
            {
                maximumcount = DateTime.DaysInMonth(relativeDate.Year, relativeDate.Month);
                datetime = new DateTime(relativeDate.Year, relativeDate.Month, 1, relativeDate.Hour,
                                             relativeDate.Minute, relativeDate.Second);
            }
            else if (source is MonthDataSource)
            {
                maximumcount = 12;
                datetime = new DateTime(relativeDate.Year, 1, relativeDate.Day, relativeDate.Hour,
                                             relativeDate.Minute, relativeDate.Second);
            }
            else if (source is YearDataSource)
            {
                maximumcount = 9998;
                if (DateTime.IsLeapYear(relativeDate.Year) && relativeDate.Month == 2 && relativeDate.Day == 29)
                    datetime = new DateTime(1, relativeDate.Month, 28, relativeDate.Hour,
                                             relativeDate.Minute, relativeDate.Second);
                else
                    datetime = new DateTime(1, relativeDate.Month, relativeDate.Day, relativeDate.Hour,
                                             relativeDate.Minute, relativeDate.Second);
            }
            else if (source is TwelveHourDataSource)
            {
                maximumcount = 12;
                datetime = new DateTime(relativeDate.Year, relativeDate.Month, relativeDate.Day, 1,
                                             relativeDate.Minute, relativeDate.Second);
            }
            else if (source is TwentyFourHourDataSource)
            {
                maximumcount = 24;
                datetime = new DateTime(relativeDate.Year, relativeDate.Month, relativeDate.Day, 0,
                                             relativeDate.Minute, relativeDate.Second);
            }
            else if (source is MinuteDataSource)
            {
                maximumcount = minuteinterval != 1 ? (60 % minuteinterval) == 0 ? (int) 60/minuteinterval : (int)60/minuteinterval + 1 : 60;
                datetime = new DateTime(relativeDate.Year, relativeDate.Month, relativeDate.Day, relativeDate.Hour,
                                        0, relativeDate.Second);
            }
            else if (source is SecondDataSource)
            {
                maximumcount = secondinterval != 1 ? (60 % secondinterval) == 0 ? (int)60 / secondinterval : (int) 60 / secondinterval + 1 : 60;
                datetime = new DateTime(relativeDate.Year, relativeDate.Month, relativeDate.Day, relativeDate.Hour,
                                        relativeDate.Minute, 0);
            }
            else
            {
                maximumcount = 2;
                if (relativeDate.ToString().Contains("PM"))
                    datetime = relativeDate.AddHours(-12);
                return maximumcount;
            }
            if (relativeDate.ToString().Contains("PM") && !datetime.ToString().Contains("PM"))
                datetime = datetime.AddHours(12);
            return maximumcount;
        }

        /// <summary>
        /// Gets the collection of dates.
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="minuteinterval"></param>
        /// <param name="secondinterval"></param>
        /// <returns></returns>
        public List<DateTimeWrapper> GetDatePartCollection(DateTime relativeDate, int minuteinterval, int secondinterval)
        {
            List<DateTimeWrapper> datepartcollection = new List<DateTimeWrapper>();
            DateTime datetime;
            var maximumcount = UpdateMinimumDateTime(relativeDate, minuteinterval, secondinterval, out datetime);
            object wrapper = new DateTimeWrapper(datetime);
            while (datepartcollection.Count < maximumcount)
            {
                if (wrapper != null)
                {
                    if ((wrapper as DateTimeWrapper).DateTime.Equals(relativeDate))
                        SelectedItem = wrapper;
                    datepartcollection.Add(wrapper as DateTimeWrapper);
                }
                wrapper = wrapper != null ? GetNext(wrapper) : null;
                if (wrapper != null && this is YearDataSource && DateTime.IsLeapYear(relativeDate.Year) 
                    && relativeDate.Day == 29 && relativeDate.Month == 2
                    && DateTime.IsLeapYear((wrapper as DateTimeWrapper).DateTime.Year))
                {
                    DateTime currentdate = (wrapper as DateTimeWrapper).DateTime;
                    (wrapper as DateTimeWrapper).DateTime = new DateTime(currentdate.Year, currentdate.Month,
                                                                         relativeDate.Day, currentdate.Hour,
                                                                         currentdate.Minute, currentdate.Second);

                }
                if (wrapper != null)
                {
                    DateTime nextdate = (wrapper as DateTimeWrapper).DateTime;
                    if (this is MonthDataSource &&
                        relativeDate.Day <= DateTime.DaysInMonth(relativeDate.Year, nextdate.Month)
                        && nextdate.Day != relativeDate.Day)
                    {
                        (wrapper as DateTimeWrapper).DateTime = new DateTime(nextdate.Year, nextdate.Month,
                                                                             relativeDate.Day,
                                                                             nextdate.Hour, nextdate.Minute,
                                                                             nextdate.Second);
                    }
                }
            }
            return datepartcollection;
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the relative DateTime value
        /// </summary>
        /// <param name="relativeDate"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        protected abstract DateTime? GetRelativeTo(DateTime relativeDate, int delta);

        #endregion

        #region Events
        /// <summary>
        /// Event invoked on changing the selection
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        #endregion
    }
}
