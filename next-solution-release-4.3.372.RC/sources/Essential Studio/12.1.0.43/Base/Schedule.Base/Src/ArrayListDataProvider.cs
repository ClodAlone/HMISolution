//-------------------------------------------------------------------------------------------------
// <copyright file="ArrayListDataProvider.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Syncfusion.Schedule
{
    #region ArrayListAppointment
    /// <summary>
    /// Derives <see cref="ScheduleAppointment"/> to implement IScheduleAppointment.
    /// </summary>
    [Serializable]
    public class ArrayListAppointment : ScheduleAppointment, ISerializable, IRecurringScheduleAppointment
    {
        [NonSerialized]
        private RecurrenceList dateList;

        private string recurrenceRule = string.Empty;

        private int recurrenceRuleID = 0;
        
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ArrayListAppointment class.
        /// </summary>
        public ArrayListAppointment()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ArrayListAppointment class to handle serilaization.
        /// </summary>
        /// <param name="info">The SerialazationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        protected ArrayListAppointment(SerializationInfo info, StreamingContext context)
        {
            this.UniqueID = (int)info.GetValue("UniqueID", typeof(int));
            this.Subject = (string)info.GetValue("Subject", typeof(string));
            this.StartTime = (DateTime)info.GetValue("StartTime", typeof(DateTime));
            this.ReminderValue = (int)info.GetValue("ReminderValue", typeof(int));
            this.Reminder = (bool)info.GetValue("Reminder", typeof(bool));
            this.Owner = (int)info.GetValue("Owner", typeof(int));
            this.MarkerValue = (int)info.GetValue("MarkerValue", typeof(int));
            this.LocationValue = (string)info.GetValue("LocationValue", typeof(string));
            this.LabelValue = (int)info.GetValue("LabelValue", typeof(int));
            this.EndTime = (DateTime)info.GetValue("EndTime", typeof(DateTime));
            this.Content = (string)info.GetValue("Content", typeof(string));
            this.AllDay = (bool)info.GetValue("AllDay", typeof(bool));

            this.recurrenceRule = (string)info.GetValue("RecurrenceRule", typeof(string));
            this.recurrenceRuleID = (int)info.GetValue("RecurrenceRuleID", typeof(int));

            if (info.MemberCount > 14)
            {
                this.TimeSpanColor = (Color)info.GetValue("TimeSpanColor", typeof(Color));
                this.BackColor = (Color)info.GetValue("BackColor", typeof(Color));
                this.AllowDrag = (bool)info.GetValue("AllowDrag", typeof(bool));
                this.AllowResize = (bool)info.GetValue("AllowResize", typeof(bool));
                this.ToolTip = (ScheduleAppointmentToolTip)info.GetValue("ToolTip", typeof(ScheduleAppointmentToolTip));
                this.CustomToolTip = (string)info.GetValue("CustomToolTip", typeof(string));
            }

            if (this.recurrenceRule.StartsWith(RecurrenceSupport.SpanMarker))
            {
                string[] parts = this.recurrenceRule.Split(new char[] { RecurrenceSupport.RuleDelimiter });
                if (parts.GetLength(0) == 3)
                {
                    this.DateList = new RecurrenceList();
                    this.DateList.BaseDate = DateTime.Parse(parts[1], System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
                    this.DateList.TerminalDate = DateTime.Parse(parts[2], System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
                }
            }

            if (info.MemberCount > 20)
            {
                this.Blocked = (bool)info.GetValue("Blocked", typeof(bool));
                this.NotifyOnOveride = (bool)info.GetValue("NotifyOnOveride", typeof(bool));
                this.Priority = (int)info.GetValue("Priority", typeof(int));
                this.AllowClickable = (bool)info.GetValue("AllowClickable", typeof(bool));
            }
            if (info.MemberCount > 24)
            {
                this.RecurringOnOverride = (bool)info.GetValue("RecurringOnOverride", typeof(bool));
            }
            ////if (info.MemberCount > 20)
            ////{
            //    this.DateList = (RecurrenceList)info.GetValue("DateList", typeof(RecurrenceList));
            ////}
            ////temporay code...
            //            if (this.recurrenceRule.StartsWith(RecurrenceSupport.SpanMarker))
            //            {
            //                this.recurrenceRule = RecurrenceSupport.SpanMarker
            //                    + RecurrenceSupport.RuleDelimiter
            //                    + this.dateList.BaseDate.ToString(System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat)
            //                    + RecurrenceSupport.RuleDelimiter
            //                    + this.dateList.TerminalDate.ToString(System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
            //            }
            this.Dirty = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets list of dates defined for this recurrence definition.
        /// </summary>
        [XmlIgnore]
        public RecurrenceList DateList
        {
            get
            {
                if (dateList == null &&
                    recurrenceRule != null &&
                    recurrenceRule.StartsWith(RecurrenceSupport.SpanMarker))
                {
                    InitSpanDateList();
                }

                return dateList;
            }

            set
            {
                dateList = value;
            }
        }

        /// <summary>
        /// Gets or sets the string that defines this recurrence.
        /// </summary>
        [DefaultValue("")]
        public string RecurrenceRule
        {
            get
            {
                return recurrenceRule;
            }

            set
            {
                recurrenceRule = value;
            }
        }

        /// <summary>
        /// Gets or sets Unique identifier to this recurrence appointment.
        /// </summary>
        /// <remarks>This value is zero for all non-recurring appointments. For a recurring appointment,
        /// this value is the same for all appointments in the same recurrence definition.</remarks>
        [DefaultValue(0)]
        public int RecurrenceRuleID
        {
            get { return recurrenceRuleID; }
            set { recurrenceRuleID = value; }
        }
        #endregion

        /// <summary>
        /// Handle serilaization.
        /// </summary>
        /// <param name="info">The SerialazationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("UniqueID", this.UniqueID);
            info.AddValue("Subject", this.Subject);
            info.AddValue("StartTime", this.StartTime);
            info.AddValue("ReminderValue", this.ReminderValue);
            info.AddValue("Reminder", this.Reminder);
            info.AddValue("NotifyOnOveride", this.NotifyOnOveride);
            info.AddValue("Priority", this.Priority);
            info.AddValue("Blocked", this.Blocked);
            info.AddValue("Owner", this.Owner);
            info.AddValue("MarkerValue", this.MarkerValue);
            info.AddValue("LocationValue", this.LocationValue);
            info.AddValue("LabelValue", this.LabelValue);
            info.AddValue("EndTime", this.EndTime);
            info.AddValue("Content", this.Content);
            info.AddValue("AllDay", this.AllDay);
            info.AddValue("TimeSpanColor", this.TimeSpanColor);
            info.AddValue("BackColor", this.BackColor);
            info.AddValue("AllowDrag", this.AllowDrag);
            info.AddValue("AllowResize", this.AllowResize);
            info.AddValue("AllowClickable", this.AllowClickable);
            info.AddValue("ToolTip", this.ToolTip);
            info.AddValue("CustomToolTip", this.CustomToolTip);

            info.AddValue("RecurrenceRule", this.recurrenceRule);
            info.AddValue("RecurrenceRuleID", this.recurrenceRuleID);
            info.AddValue("RecurringOnOverride", this.RecurringOnOverride);
            ////info.AddValue("DateList", this.dateList);
        }

        ////Added to provide dynamic initialization for DateList in span rules.
        private void InitSpanDateList()
        {
            string[] parts = RecurrenceRule.Split(new char[] { RecurrenceSupport.RuleDelimiter });
            if (parts.GetLength(0) == 3)
            {
                dateList = new RecurrenceList();
                dateList.BaseDate = DateTime.Parse(parts[1], System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
                dateList.TerminalDate = DateTime.Parse(parts[2], System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
            }
        }
    }
    #endregion   

    #region ArrayListAppointmentList

    /// <summary>
    /// Derives <see cref="ScheduleAppointmentList"/> to implement IScheduleAppointmentList.
    /// </summary>
    [Serializable]
    public class ArrayListAppointmentList : ScheduleAppointmentList, ISerializable
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ArrayListAppointmentList class.
        /// </summary>
        public ArrayListAppointmentList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ArrayListAppointmentList class. Used in serialization.
        /// </summary>
        /// <param name="info"> The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        protected ArrayListAppointmentList(SerializationInfo info, StreamingContext context)
        {
            this.List = (ArrayList)info.GetValue("List", typeof(ArrayList));
        }
        #endregion

        /// <summary>
        /// Returns the underlying ArrayList that holds these objects.
        /// </summary>
        /// <returns>The underlying ArrayList.</returns>
        public ArrayList GetList()
        {
            return this.List;
        }

        /// <summary>
        /// Override to control serialization.
        /// </summary>
        /// <param name="info"> The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("List", this.List);
        }

        /// <summary>
        /// Used in serialization.
        /// </summary>
        /// <param name="info"> The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        /// <summary>
        /// Overridden to return a <see cref="ArrayListAppointment"/>.
        /// </summary>
        /// <returns>An ArrayListAppointment.</returns>
        public override IScheduleAppointment NewScheduleAppointment()
        {
            return new ArrayListAppointment();
        }
    }
    #endregion

    #region ArrayListDataProvider
    /// <summary>
    /// Derives <see cref="ScheduleDataProvider"/> and implements <see cref="IRecurringScheduleDataProvider"/>.
    /// </summary>
    /// <remarks>
    /// This implementation of IRecurringScheduleDataProvider uses two collection of
    /// <see cref="ArrayListAppointment"/>
    /// objects to hold the items displayed in the schedule. These collections is serialized to disk as a
    /// binary file. The ArrayListDataProvider.MasterList holds the appointment items that appear
    /// in the schedule. The ArrayListDataProvider.RecurList holds any non-terminiating
    /// appoint definitions.
    /// </remarks>
    [Serializable]
    public class ArrayListDataProvider : ScheduleDataProvider, IRecurringScheduleDataProvider
    {
        private string fileName;

        private ArrayListAppointmentList masterList;

        private ArrayListAppointment[] masterListArray = null;

        private int nextUniqueID = -1;

        private ArrayListAppointmentList recurringList = null;

        private ArrayListAppointment[] recurringListArray = null;

        private RecurrenceSupport recurSupport = null;

        /// <summary>
        /// Initializes a new instance of the ArrayListDataProvider class
        /// </summary>
        public ArrayListDataProvider()
            : base()
        {
        }

        #region Properties
        /// <summary>
        /// A property that gets or sets the file name used to store the serialized data.
        /// </summary>
        [XmlIgnore]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// An overridden property that gets/sets whether the MasterList has been modified.
        /// </summary>
        [XmlIgnore]
        public override bool IsDirty
        {
            get
            {
                bool val = base.IsDirty;
                ////if no global setting marked list as dirty, check individual items
                if (!val) 
                {
                    foreach (IScheduleAppointment item in this.MasterList)
                    {
                        if (item.Dirty)
                        {
                            val = true;
                            break;
                        }
                    }
                }

                return val;
            }

            set
            {
                base.IsDirty = value;
            }
        }

        /// <summary>
        /// Gets or sets an IScheduleAppointmentList collection that holds the IRecurringScheduleAppointments.
        /// </summary>
        [XmlIgnore]
        public virtual ArrayListAppointmentList MasterList
        {
            get
            {
                if (null == masterList)
                {
                    masterList = this.NewScheduleAppointmentList() as ArrayListAppointmentList;
                }

                return masterList;
            }

            set
            {
                masterList = value;
            }
        }

        /// <summary>
        /// Gets or sets MasterListArrray for XML serialization.
        /// </summary>
        [XmlElement("MasterList")]
        public virtual ArrayListAppointment[] MasterListArray
        {
            get
            {
                if (masterListArray == null)
                {
                    masterListArray = this.MasterList.GetList().ToArray(typeof(ArrayListAppointment)) as ArrayListAppointment[];
                }

                return masterListArray;
            }

            set
            {
                masterListArray = value;
            }
        }

        /// <summary>
        /// Gets or sets the largest ID value used so far in this ScheduleDataProvider.
        /// </summary>
        public int NextUniqueID
        {
            get
            {
                if (nextUniqueID == -1)
                {
                    nextUniqueID = 0;
                    foreach (IRecurringScheduleAppointment item in RecurringList)
                    {
                        if (nextUniqueID < item.RecurrenceRuleID)
                        {
                            nextUniqueID = item.RecurrenceRuleID;
                        }
                    }
                }

                if (nextUniqueID == int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("NextUniqueID", "must not exceed int.MaxValue");
                }

                return ++nextUniqueID;
            }

            set
            {
                nextUniqueID = value;
            }
        }

        /// <summary>
        /// Gets a ArrayListAppointmentList collection that holds ScheduleRecurringAppointments.
        /// </summary>
        [XmlIgnore]
        public IScheduleAppointmentList RecurringList
        {
            get
            {
                if (recurringList == null)
                {
                    recurringList = this.NewScheduleAppointmentList() as ArrayListAppointmentList;
                }

                return recurringList;
            }
        }

        /// <summary>
        /// Gets or sets RecurringListArray which is used internally for XML serialization.
        /// </summary>
        [XmlElement("RecurringList")]
        public ArrayListAppointment[] RecurringListArray
        {
            get
            {
                if (recurringListArray == null)
                {
                    recurringListArray = ((ArrayListAppointmentList)this.RecurringList).GetList().ToArray(typeof(ArrayListAppointment)) as ArrayListAppointment[];
                }

                return recurringListArray;
            }

            set
            {
                recurringListArray = value;
            }
        }

        [XmlIgnore]
        internal RecurrenceSupport RecurSupport
        {
            get
            {
                if (recurSupport == null)
                {
                    recurSupport = new RecurrenceSupport();
                }

                return recurSupport;
            }

            set
            {
                recurSupport = value;
            }
        }
        #endregion

        private void AddAppointmentFromItem(IRecurringScheduleAppointment item, DateTime dt)
        {
            IRecurringScheduleAppointment item1 = item.Clone() as IRecurringScheduleAppointment;
            item1.StartTime = new DateTime(dt.Year, dt.Month, dt.Day, item.StartTime.Hour, item.StartTime.Minute, 0);
            item1.EndTime = new DateTime(dt.Year, dt.Month, dt.Day, item.EndTime.Hour, item.EndTime.Minute, 0);
            this.MasterList.Add(item1);
        }

        /// <summary>
        /// Overridden to add the item to the MasterList.
        /// </summary>
        /// <param name="item">IScheduleAppointment item to be added.</param>
        public override void AddItem(IScheduleAppointment item)
        {
            if (!(item is ArrayListAppointment))
            {
                throw new ArgumentException("Item must be of type ArrayListAppointment.");
            }

            this.MasterList.Add(item);
        }

        /// <summary>
        /// Adds recurring appointments.
        /// </summary>
        /// <param name="item">The recurring appointment definition.</param>
        /// <param name="dateLimit">A date limit at which the recurring appointments end.</param>
        /// <remarks> The date limit may be dateTime.MaxValue which indicates no limit.</remarks>
        public void AddNewRecurringAppointments(IRecurringScheduleAppointment item, DateTime dateLimit)
        {
            string[] rulesArray = item.RecurrenceRule.Split(new char[] { RecurrenceSupport.RuleDelimiter });
            DateTime baseDate, date;
            if (item.RecurringOnOverride)
            {
                baseDate = DateTime.ParseExact(rulesArray[0], "dd.MM.yyyy", System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
                date = rulesArray[1].Length > 0 ? DateTime.ParseExact(rulesArray[1], "dd.MM.yyyy", System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat) : dateLimit;
            }
            else
            {
                baseDate = DateTime.Parse(rulesArray[0], System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
                date = rulesArray[1].Length > 0 ? DateTime.Parse(rulesArray[1], System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat) : dateLimit;
            }

            if (item.DateList == null)
            {
                ////if null, then need to create it.
                CreateDateListFromItem(item);
                ////conditionally set terminal date if present in rule.
                if (rulesArray[1].Length > 0)
                {
                    item.DateList.TerminalDate = date;
                }
            }

            item.DateList.IsValidRecurrence(date); ////this populates DateList up through date
            bool needToSort = false;
            foreach (DateTime dt in item.DateList)
            {
                ////now go through and add appointments that match the datelist entries
                if (dt >= item.DateList.BaseDate && dt <= item.DateList.TerminalDate)
                {
                    AddAppointmentFromItem(item, dt);
                    needToSort = true;
                }
            }

            if (needToSort)
            {
                ResetBaseDate(item);
                MasterList.SortStartTime();
            }
        }

        /// <summary>
        /// Used after the initial load to add additional recurring appointments to the dataprovider.
        /// </summary>
        /// <param name="date">The recurring appointment definition.</param>
        /// <returns>True if dates were added.</returns>
        /// <remarks>Dynamically provide appointments on demand as new dates are exposed.</remarks>
        public bool CheckAndAddIfNeededRecurringAppointments(DateTime date)
        {
            bool ret = false;
            foreach (IRecurringScheduleAppointment item in RecurringList)
            {
                if (item.DateList == null && item.RecurrenceRule != null && item.RecurrenceRule.Length > 0)
                {
                    ////create initial DateList and add any appointments
                    CreateDateListFromItem(item);
                    if (item.DateList != null)
                    {
                        item.RecurringOnOverride = true;
                        AddNewRecurringAppointments(item, item.DateList.BaseDate);
                    }
                }

                RecurrenceList list = item.DateList;
                if (list != null)
                {
                    ////now loop thru and add appointments if needed
                    DateTime date1 = date.CompareTo(list.TerminalDate) > 0 ? list.TerminalDate : date;
                    if (date1.CompareTo(list[list.Count - 1]) > 0)
                    {
                        ////need to add
                        int start = list.Count;
                        list.IsValidRecurrence(date1);
                        bool needToSort = false;
                        while (start < list.Count)
                        {
                            date1 = list[start];
                            if (date1 >= item.DateList.BaseDate && date1 <= item.DateList.TerminalDate)
                            {
                                AddAppointmentFromItem(item, date1);
                                needToSort = true;
                            }

                            start++;
                        }

                        if (needToSort)
                        {
                            ResetBaseDate(item);
                            MasterList.SortStartTime();
                            ret = true;
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Saves this ArrayListDataProvider object as a diskfile.
        /// </summary>
        /// <remarks>If the <see cref="FileName"/> ends with ".XML", then
        /// the data object is saved to disk as an XML file. If the
        /// FileName ends with somethings else, then the object is saved
        /// as a binary file.</remarks>
        public override void CommitChanges()
        {
            ////reset lists so changes are serialized
            masterListArray = null;
            int i = this.MasterListArray.GetLength(0);
            recurringListArray = null;
            i = this.RecurringListArray.GetLength(0);

            if (FileName != null && FileName.Length > 0)
            {
                if (FileName.ToUpper().EndsWith(".XML"))
                {
                    SaveXML(FileName);
                }
                else
                {
                    SaveBinary(FileName);
                }
            }

            this.IsDirty = false;
        }

        private void CreateDateListFromItem(IRecurringScheduleAppointment item)
        {
            string[] rulesArray = item.RecurrenceRule.Split(new char[] { RecurrenceSupport.RuleDelimiter });
            DateTime baseDate, tempDate = DateTime.MinValue;
            if (item.RecurringOnOverride)
            {
                baseDate = DateTime.ParseExact(rulesArray[0], "dd.MM.yyyy", System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
                tempDate = (rulesArray[1].Length > 0) ? DateTime.ParseExact(rulesArray[1], "dd.MM.yyyy", System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat) : tempDate;
            }
            else
            {
                baseDate = DateTime.Parse(rulesArray[0], System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
                tempDate = (rulesArray[1].Length > 0) ? DateTime.Parse(rulesArray[1], System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat) : tempDate;
            }
            if (rulesArray[1].Length == 0 ||
                baseDate.Date < tempDate.Date)
            {
                string s = rulesArray[2];
                for (int i = 3; i < rulesArray.GetLength(0); ++i)
                {
                    s += RecurrenceSupport.RuleDelimiter + rulesArray[i];
                }

                item.DateList = new RecurrenceList(s, baseDate, null);
            }
        }

        private static void DisplayList(string title, ScheduleAppointmentList list)
        {
#if console
			Console.WriteLine("*************" + title);
			foreach(ScheduleAppointment item in list)
			{
				Console.WriteLine(item);
			}
#endif
        }

        /// <summary>
        /// Returns a the subset of MasterList between the 2 dates.
        /// </summary>
        /// <param name="startDate">Starting date limit for the returned items.</param>
        /// <param name="endDate">Ending date limit for the returned items.</param>
        /// <returns>Returns a the subset of MasterList.</returns>
        public override IScheduleAppointmentList GetSchedule(DateTime startDate, DateTime endDate)
        {
            ScheduleAppointmentList list = this.NewScheduleAppointmentList() as ScheduleAppointmentList;
            DateTime start = startDate.Date;
            DateTime end = endDate.Date;
            CheckAndAddIfNeededRecurringAppointments(endDate);
            foreach (ScheduleAppointment item in this.MasterList)
            {
                ////item.EndTime.AddMinutes(-1) is to make sure an item that ends at
                ////midnight is not shown on the next days calendar
                if ((item.StartTime.Date >= start && item.StartTime.Date <= end)
                    || (item.EndTime.AddMinutes(-1).Date > start && item.EndTime.Date <= end))
                {
                    list.Add(item);
                }
            }

            list.SortStartTime();
            ////DisplayList(string.Format("************dates between {0} and {1}", startDate, endDate), list);
            return list;
        }

        /// <summary>
        /// Returns a the subset of MasterList for a given date.
        /// </summary>
        /// <param name="day">Date for the returned items.</param>
        /// <returns>Returns a the subset of MasterList.</returns>
        public override IScheduleAppointmentList GetScheduleForDay(DateTime day)
        {
            ScheduleAppointmentList list = this.NewScheduleAppointmentList() as ScheduleAppointmentList;
            day = day.Date;
            bool sort = CheckAndAddIfNeededRecurringAppointments(day);
            foreach (ScheduleAppointment item in this.MasterList)
            {
                ////do not want anything that ends at 12AM on the day
                if (item.StartTime.Date == day || (item.EndTime.Date == day && item.EndTime > day))
                {
                    list.Add(item);
                }
            }

            if (sort)
            {
                list.SortStartTime();
            }
            ////DisplayList(string.Format("*************day {0}", day), list);
            return list;
        }

        /// <summary>
        /// Returns a unique integer that serves to identify a recurring family of appointments.
        /// </summary>
        /// <returns>A unique integer.</returns>
        public int GetUniqueID()
        {
            return NextUniqueID;
        }

        /// <summary>
        /// A static method that provides random data, not really a part of the implementations.
        /// </summary>
        /// <returns>A ArrayListAppointmentList object holding sample data.</returns>
        public static ArrayListAppointmentList InitializeRandomData()
        {
            ArrayListAppointmentList masterList = new ArrayListAppointmentList();
            masterList = InitializeRandomData(masterList) as ArrayListAppointmentList;
            return masterList;
        }

        /// <summary>
        /// A static method that provides random data, not really a part of the implementations.
        /// </summary>
        /// <param name="masterList">A ScheduleAppointmentList</param>        
        /// <returns>A ArrayListAppointmentList object holding sample data.</returns>
        protected static IScheduleAppointmentList InitializeRandomData(IScheduleAppointmentList masterList)
        {
            ////int tc = Environment.TickCount;
            ////int tc = 26260100;// simple spread
            int tc = 28882701; //// split the appointment across midnight & 3 items at 8am on 2 days ago

            ////Console.WriteLine("Random seed: {0}", tc);
            Random r = new Random(tc);
            Random r1 = new Random(tc);

            // set the number of sample items you want in this list.
            ////int count = r.Next(20) + 4;
            int count = 400; ////1000;//200;//30;

            if (null == masterList)
            {
                masterList = new ArrayListAppointmentList();
            }

            DateTime now = DateTime.Now.Date;

            for (int i = 0; i < count; ++i)
            {
                IScheduleAppointment item = masterList.NewScheduleAppointment();

                ////int dayOffSet = 0;
                ////int hourOffSet = 8 - r.Next(16);

                ////int dayOffSet = 3 - r.Next(6);
                int dayOffSet = 30 - r.Next(60);
                ////int dayOffSet = 100 - r.Next(200);
                int hourOffSet = 24 - r.Next(48);

                int len = 30 * (r.Next(4) + 1);
                item.UniqueID = i;
                item.StartTime = now.AddDays((double)dayOffSet).AddHours((double)hourOffSet);
                item.EndTime = item.StartTime.AddMinutes((double)len);
                item.Subject = string.Format("subject{0}", i);
                item.Content = string.Format("content{0}", i);
                item.LabelValue = r1.Next(10) < 3 ? 0 : r1.Next(10);
                item.LocationValue = string.Format("location{0}", r1.Next(5));

                item.ReminderValue = r1.Next(10) < 5 ? 0 : r1.Next(12);
                item.Reminder = r1.Next(10) > 1;
                item.AllDay = r1.Next(10) < 1;

                item.MarkerValue = r1.Next(4);
                item.Dirty = false;

                int startHour = item.StartTime.TimeOfDay.Hours;
                int endHour = item.EndTime.TimeOfDay.Hours;
                int nextDayMaybe = endHour != 0 ? 0 : 1;
                if (item.StartTime.Date < item.EndTime.Date)
                {

                    int dayCount;

                    if (item.EndTime.Day > item.StartTime.Day)
                    {

                        dayCount = ((TimeSpan)(item.EndTime.Date - item.StartTime.Date)).Days + 1; //(item.EndTime.Day - item.StartTime.Day) + 1;

                    }
                    else
                    {
                        dayCount = ((TimeSpan)(item.EndTime.Date - item.StartTime.Date)).Days + 1; //(item.StartTime.Day - item.EndTime.Day) + 1;                        
                    }
                    #pragma warning disable
                    for (int j = 0; j < dayCount; j++)
                    {
                        IRecurringScheduleAppointment item1 = item.Clone() as IRecurringScheduleAppointment;
                        item1.RecurrenceRule = RecurrenceSupport.SpanMarker
                                                + RecurrenceSupport.RuleDelimiter
                                                + item.StartTime.ToString(System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat)
                                                + RecurrenceSupport.RuleDelimiter
                                                + item.EndTime.ToString(System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
                        item1.RecurrenceRuleID = i;
                        if (j == 0)
                        {
                            item1.StartTime = item.StartTime;
                            DateTime next = item.StartTime.AddDays(j);
                            item1.EndTime = new DateTime(next.Year, next.Month, next.Day, endHour, 0, 0);
                        }
                        else if (j < count - 1)
                        {
                            DateTime next = item.StartTime.AddDays(j);
                            item1.StartTime = new DateTime(next.Year, next.Month, next.Day, startHour, 0, 0);
                            next = next.AddDays(nextDayMaybe);
                            item1.EndTime = new DateTime(next.Year, next.Month, next.Day, endHour, 0, 0);
                        }
                        else
                        {
                            DateTime next = item.StartTime.AddDays(j);
                            item1.StartTime = new DateTime(next.Year, next.Month, next.Day, startHour, 0, 0);
                            item1.EndTime = item.EndTime.AddDays(j - count + 1);
                        }
                        item1.DateList = new RecurrenceList();
                        item1.DateList.BaseDate = item.StartTime;
                        item1.DateList.TerminalDate = item.EndTime;
                        masterList.Add(item1);
                        break;
                    }
                }
                else
                    masterList.Add(item);
            }

            ////set explicit values if needed for testing...
            ////masterList[142].Reminder = true;
            ////masterList[142].ReminderValue = 9;//  hrs; // 7;//3 hrs

            ////DisplayList("Before Sort", masterList);
            masterList.SortStartTime();
            ////DisplayList("After Sort", masterList);
            return masterList;
        }

        /// <summary>
        /// A Virtual method that loads a binary file whose name is given by<see cref="FileName"/>
        /// into this class instance.
        /// </summary>
        public virtual void LoadBinary()
        {
            ArrayListDataProvider t = this;
            Stream s = null;
            try
            {
                s = File.OpenRead(fileName);
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Syncfusion.ScheduleBaseAssembly.AssemblyResolver);
                BinaryFormatter b = new BinaryFormatter();
                b.AssemblyFormat = FormatterAssemblyStyle.Simple;
                object obj = b.Deserialize(s);
                t.MasterList = obj as ArrayListAppointmentList;
                obj = b.Deserialize(s);
                t.recurringList = obj as ArrayListAppointmentList;
                t.nextUniqueID = (int)b.Deserialize(s);
            }
            finally
            {
                s.Close();
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(Syncfusion.ScheduleBaseAssembly.AssemblyResolver);
            }
        }

        /// <summary>
        /// A static method that creates an instance of <see cref="ArrayListDataProvider"/> and loads
        /// a previously serialized ArrayListDataProvider into the instance.
        /// </summary>
        /// <param name="fileName">The serialized filename.</param>
        /// <returns>A ArrayListDataProvider.</returns>
        /// <remarks>
        /// This method depends upon System.AppDomain.CurrentDomain.AssemblyResolve to
        /// avoid versioning issues with the binary serialization of the MasterList.
        /// </remarks>
        public static ArrayListDataProvider LoadBinary(string fileName)
        {
            ArrayListDataProvider t = new ArrayListDataProvider();
            t.FileName = fileName;
            t.LoadBinary();
            return t;
        }

        /// <summary>
        /// Creates an instance of <see cref="ArrayListDataProvider"/> and loads
        /// a previously serialized ArrayListDataProvider into the instance from an XML file.
        /// </summary>
        /// <param name="fileName">The serialized filename.</param>
        /// <returns>A ArrayListDataProvider.</returns>
        public static ArrayListDataProvider LoadXML(string fileName)
        {
            ArrayListDataProvider t = null;

            Stream s = File.OpenRead(fileName);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ArrayListDataProvider));
                t = serializer.Deserialize(s) as ArrayListDataProvider;
                t.masterList = new ArrayListAppointmentList();
                t.recurringList = new ArrayListAppointmentList();
                if (t.masterListArray != null && t.masterListArray.GetLength(0) > 0)
                {
                    t.MasterList.GetList().AddRange(t.masterListArray);
                }

                if (t.recurringListArray != null && t.recurringListArray.GetLength(0) > 0)
                {
                    ((ArrayListAppointmentList)t.RecurringList).GetList().AddRange(t.recurringListArray);
                }
            }
            finally
            {
                s.Close();
            }

            return t;
        }

        /// <summary>
        /// An Overridden method to return a <see cref="ArrayListAppointment"/>.
        /// </summary>
        /// <returns>New ArrayListAppointment.</returns>
        public override IScheduleAppointment NewScheduleAppointment()
        {
            return new ArrayListAppointment();
        }

        /// <summary>
        /// An Overridden method to return a <see cref="ArrayListAppointmentList"/>.
        /// </summary>
        /// <returns>New ArrayListAppointmentList.</returns>
        public override IScheduleAppointmentList NewScheduleAppointmentList()
        {
            return new ArrayListAppointmentList();
        }

        /// <summary>
        /// Strip out any unnecessary recurring appointments (already added appts up thru timelimit)
        /// </summary>
        /// <exclude/>
        protected void PreprocessListsBeforeWrite()
        {   
            ArrayList list = new ArrayList();
            foreach (IRecurringScheduleAppointment item in this.RecurringList)
            {
                if (item.DateList != null && item.DateList.TerminalDate < DateTime.MaxValue)
                {
                    list.Add(item);
                }
            }

            foreach (IRecurringScheduleAppointment item in list)
            {
                this.RecurringList.Remove(item);
            }

            ////trim the sizes before serializing...
            ((ArrayListAppointmentList)this.MasterList).GetList().TrimToSize();
            ((ArrayListAppointmentList)this.RecurringList).GetList().TrimToSize();
        }

        /// <summary>
        /// An Overridden method to remove the item from the MasterList.
        /// </summary>
        /// <param name="item">IScheduleAppointment item to be removed.</param>
        public override void RemoveItem(IScheduleAppointment item)
        {
            this.MasterList.Remove(item);
        }

        /// <summary>
        /// Removes all occurrences the given appointment.
        /// </summary>
        /// <param name="recurItem">The recurring appointment to be removed.</param>
        /// <returns>True, if the appointment is removed; False otherwise.</returns>
        public bool RemoveRecurringAppointments(IRecurringScheduleAppointment recurItem)
        {
            bool deleted = false;
            ArrayList list = new ArrayList();
            foreach (IRecurringScheduleAppointment item in this.MasterList)
            {
                if (item.RecurrenceRuleID == recurItem.RecurrenceRuleID)
                {
                    list.Add(item);
                }
            }

            foreach (IRecurringScheduleAppointment item in list)
            {
                this.MasterList.Remove(item);
                deleted = true;
            }

            IRecurringScheduleAppointment found = null;

            foreach (IRecurringScheduleAppointment item in this.RecurringList)
            {
                if (item.RecurrenceRuleID == recurItem.RecurrenceRuleID)
                {
                    found = item;
                    deleted = true;
                    break;
                }
            }

            if (found != null)
            {
                RecurringList.Remove(found);
            }

            return deleted;
        }

        private void ResetBaseDate(IRecurringScheduleAppointment item)
        {
            item.DateList.BaseDate = item.DateList[item.DateList.Count - 1].AddDays(1);
            if (item.DateList.BaseDate > item.DateList.TerminalDate)
            {
                item.DateList.BaseDate = item.DateList.TerminalDate;
            }

            int i = item.RecurrenceRule.IndexOf(RecurrenceSupport.RuleDelimiter);
            if (i > -1)
            {
                if (item.RecurringOnOverride)
                    item.RecurrenceRule = string.Format("{0}{1}", item.DateList.BaseDate.ToString("dd.MM.yyyy"), item.RecurrenceRule.Substring(i));
                else
                    item.RecurrenceRule = string.Format("{0}{1}", item.DateList.BaseDate.ToShortDateString(), item.RecurrenceRule.Substring(i));
            }
        }

        /// <summary>
        /// Saves the <see cref="ArrayListDataProvider"/> object to a stream in binary format.
        /// </summary>
        /// <param name="s">The Stream object.</param>
        public virtual void SaveBinary(Stream s)
        {
            PreprocessListsBeforeWrite();

            ////save the 2 lists and the ID
            try
            {
                BinaryFormatter b = new BinaryFormatter();
                b.AssemblyFormat = FormatterAssemblyStyle.Simple;
                b.Serialize(s, this.MasterList);
                b.Serialize(s, this.RecurringList);
                b.Serialize(s, this.nextUniqueID);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Problem writing binary file: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Saves the current <see cref="ArrayListDataProvider"/> object in binary format to a file
        /// with the specified filename.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public virtual void SaveBinary(string fileName)
        {
            Stream s = File.Create(fileName);
            SaveBinary(s);
            s.Close();
        }

        /// <summary>
        /// Overridden to make changes to appointments.
        /// </summary>
        /// <param name="appModifiedItem">The edited appointment.</param>
        /// <param name="appOriginalItem">The original appointment.</param>
        public override void SaveModifiedItem(IScheduleAppointment appModifiedItem, IScheduleAppointment appOriginalItem)
        {
            if (null != this.MasterList)
            {
                for (int i = 0; i < this.MasterList.Count; i++)
                {
                    if (this.MasterList[i].Equals(appOriginalItem))
                    {
                        this.MasterList[i] = appModifiedItem;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Makes changes to appointments in a recurring sequence of appointments.
        /// </summary>
        /// <param name="modifiedItem">The edited appointment.</param>
        /// <param name="originalItem">The original appointment.</param>
        /// <param name="action">The requested edit action.</param>
        public void SaveModifiedRecurringAppointment(IRecurringScheduleAppointment modifiedItem, IRecurringScheduleAppointment originalItem, RecurringAppointmentEditAction action)
        {
            switch (action)
            {
                case RecurringAppointmentEditAction.ChangeAllAppointments:
                    for (int i = 0; i < this.MasterList.Count; ++i)
                    {
                        IRecurringScheduleAppointment item = this.MasterList[i] as IRecurringScheduleAppointment;
                        if (item.RecurrenceRuleID == originalItem.RecurrenceRuleID)
                        {
                            this.MasterList[i] = SetExceptDates(this.MasterList[i], modifiedItem);
                        }
                    }

                    for (int i = 0; i < this.RecurringList.Count; ++i)
                    {
                        IRecurringScheduleAppointment item = this.MasterList[i] as IRecurringScheduleAppointment;
                        if (item.RecurrenceRuleID == originalItem.RecurrenceRuleID
                             && this.RecurringList[i].StartTime >= originalItem.StartTime)
                        {
                            this.RecurringList[i] = SetExceptDates(this.RecurringList[i], modifiedItem);
                        }
                    }

                    break;
                case RecurringAppointmentEditAction.ChangeAllFutureAppointments:

                    for (int i = 0; i < this.MasterList.Count; ++i)
                    {
                        IRecurringScheduleAppointment item = this.MasterList[i] as IRecurringScheduleAppointment;

                        if (item.RecurrenceRuleID == originalItem.RecurrenceRuleID
                            && this.MasterList[i].StartTime >= originalItem.StartTime)
                        {
                            // this.MasterList[i] = modifiedItem;
                            this.MasterList[i] = SetExceptDates(this.MasterList[i], modifiedItem);
                        }
                    }

                    for (int i = 0; i < this.RecurringList.Count; ++i)
                    {
                        IRecurringScheduleAppointment item = this.MasterList[i] as IRecurringScheduleAppointment;

                        if (item.RecurrenceRuleID == originalItem.RecurrenceRuleID
                             && this.RecurringList[i].StartTime >= originalItem.StartTime)
                        {
                            this.RecurringList[i] = SetExceptDates(this.RecurringList[i], modifiedItem);
                        }
                    }

                    break;
                case RecurringAppointmentEditAction.ChangeSingleAppointmentOnly:
                    for (int i = 0; i < this.MasterList.Count; ++i)
                    {
                        if (this.MasterList[i].Equals(originalItem))
                        {
                            this.MasterList[i] = modifiedItem;
                            break;
                        }
                    }

                    break;
                case RecurringAppointmentEditAction.Cancel:
                    ////no action....
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Saves this <see cref="ArrayListDataProvider"/> object in an XML file
        /// with the specified filename.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public virtual void SaveXML(string fileName)
        {
            PreprocessListsBeforeWrite();

            XmlSerializer serializer = new XmlSerializer(typeof(ArrayListDataProvider));
            TextWriter writer = new StreamWriter(fileName);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        private IScheduleAppointment SetExceptDates(IScheduleAppointment targetItem, IScheduleAppointment sourceItem)
        {
            DateTime start = targetItem.StartTime;
            DateTime end = targetItem.EndTime;
            targetItem = sourceItem.Clone() as IRecurringScheduleAppointment;
            targetItem.StartTime = start;
            targetItem.EndTime = end;
            return targetItem;
        }
    }
    #endregion
}
