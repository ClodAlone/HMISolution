//-------------------------------------------------------------------------------------------------
// <copyright file="ScheduleAppointment.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Syncfusion.Schedule
{
    #region ListObject

    /// <summary>
    /// A wrapper class for maintaining list choices that can have a valueMember, displayMember and colorMember associated with them.
    /// </summary>
    [Serializable]
    public class ListObject : ILookUpObject
    {
        private int id;
        private string display;
        private Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListObject"/> class.
        /// </summary>
        /// <param name="valueMember">The valueMember that is stored in the data objects.</param>
        /// <param name="displayMember">The displayMember that is used for display.</param>
        /// <param name="colorMember">A color that is associated with this valueMember.</param>
        public ListObject(int valueMember, string displayMember, Color colorMember)
        {
            this.id = valueMember;
            this.display = displayMember;
            this.color = colorMember;
        }

        /// <summary>
        /// A Virtual property that gets/sets color asscoiated with this object.
        /// </summary>
        public virtual Color ColorMember
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// A Virtual property that gets/sets string that is used when this object is displayed.
        /// </summary>
        public virtual string DisplayMember
        {
            get { return display; }
            set { display = value; }
        }

        /// <summary>
        /// A Virtual property that gets/sets an integer that is stored in the data objects to represent this object.
        /// </summary>
        public virtual int ValueMember
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// An Overridden method to return DisplayMember.
        /// </summary>
        /// <returns>The DisplayMember.</returns>
        public override string ToString()
        {
            return display;
        }
    }
    #endregion

    #region ListObjectList
    /// <summary>
    /// A strongly typed Arraylist class that maintain list option values.
    /// </summary>
    [Serializable]
    public class ListObjectList : ArrayList, ITypedList, ILookUpObjectList
    {
        #region ITypedList Members

        /// <summary>
        /// A method that gets ListObject property descriptors collection.
        /// </summary>
        /// <param name="listAccessors">An array of PropertyDescriptor objects to find in the collection as bindable. This can be a null reference.</param>
        /// <returns>The property descriptors for each property in ListObject.</returns>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return TypeDescriptor.GetProperties(typeof(ListObject));
        }

        /// <summary>
        /// A method that gets the name of the list, i.e. &quot;ListObject&quot;.
        /// </summary>
        /// <param name="listAccessors">An array of PropertyDescriptor objects to find in
        /// the collection as bindable. This can be a null reference.</param>
        /// <returns>
        /// The list name.
        /// </returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return "ListObject";
        }

        #endregion

        #region ILookUpObjectList Members

        /// <summary>
        /// Gets or sets the i-th item in the list.
        /// </summary>
        /// <param name="i">The key to identify the desired item.</param>
        public new ILookUpObject this[int i]
        {
            get
            {
                return (ILookUpObject)base[i];
            }

            set
            {
                base[i] = value;
            }
        }

        #endregion
    }
    #endregion

    #region ScheduleAppointment

    /// <summary>
    /// A class that define the objects that represent appointments in the Schedule Control.
    /// </summary>
    /// <remarks>
    ///  This class implements IScheduleAppointment to provide an object to hold the concrete data associated
    ///  with appointments.
    ///  You can either derive this class or implement IScheduleAppointment yourself to extend or modify the information
    ///  managed by the ScheduleAppointment class.
    /// </remarks>
    [Serializable]
    public class ScheduleAppointment : IScheduleAppointment
    {
        private bool allDay = false;
        private string content = string.Empty;
        private bool dirty = false;
        private DateTime end = DateTime.Now.AddMinutes(30);
        private int id = 0;
        private bool ignoreChanges = false;
        private int labelValue = 0;
        private string locationValue = string.Empty;
        private ScheduleAppointmentToolTip m_eAppointmentToolTip = ScheduleAppointmentToolTip.All;
        private bool m_bAllowClickable = true;
        private bool m_bAllowDrag = true;
        private bool m_bAllowResize = true;
        private bool m_bBlocked = false;
        private Color m_colorBackColor = Color.Empty;
        private Color m_colorTimeSpanColor = Color.Empty;
        private bool m_bNotifyOnOveride = false;
        private bool m_bRecurringOnOverride = false;
        private int m_nPriority = 0;
        private string m_sCustomToolTip = string.Empty;
        private int markerValue = 0;
        private int owner = 0;
        private bool reminder = false;
        private int reminderValue = 0;
        private DateTime start = DateTime.Now;
        private string subject = string.Empty;
        private object tag;
        private int version = 0;

        /// <summary>
        /// Initializes a new instance of the ScheduleAppointment class.
        /// </summary>
        public ScheduleAppointment()
        {
            // TODO: Add constructor logic here
        }

        #region Properties
        /// <summary>
        /// A Virtual property that gets or sets a value indicating whether this appointment is an all-day appointment.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets whether this appointment is an all-day appointment."),
            DefaultValue(false)]
        public virtual bool AllDay
        {
            get
            {
                return allDay;
            }

            set
            {
                if (!IgnoreChanges && allDay != value)
                {
                    Dirty = true;
                }

                allDay = value;
            }
        }

        /// <summary>
        /// A property that gets or sets a value indicating whether or not blocked appointment is allowed to click.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Specifies whether or not blocked appointment is allowed to click."),
            DefaultValue(true)]
        public bool AllowClickable
        {
            get
            {
                return m_bAllowClickable;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_bAllowClickable)
                {
                    this.Dirty = true;
                }

                m_bAllowClickable = value;
            }
        }

        /// <summary>
        /// A property that gets or sets a value indicating whether or not an appointment is allowed to be dragged.The total hours doesn't gets changed by dragging an appointment.
        /// </summary>
        /// <value>Default is true.</value>
        /// <remarks>
        /// Setting this property to true enables moving an appointment by clicking on a appointment and moving it to a new place.
        /// The total duration of an Appoinment is not altered by dragging.
        /// </remarks>
        [Browsable(true),
            Bindable(true),
            Description("Specifies whether or not an appointment is allowed to drag."),
            DefaultValue(true)]
        public bool AllowDrag
        {
            get
            {
                return m_bAllowDrag;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_bAllowDrag)
                {
                    this.Dirty = true;
                }

                m_bAllowDrag = value;
            }
        }

        /// <summary>
        /// A property that gets or sets a value indicating whether or not an appointment is allowed to resize and modify its duration.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Specifies whether or not an appointment is allowed to resize."),
            DefaultValue(true)]
        public bool AllowResize
        {
            get
            {
                return m_bAllowResize;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_bAllowResize)
                {
                    this.Dirty = true;
                }

                m_bAllowResize = value;
            }
        }
        
        /// <summary>
        /// A property that gets or sets back color of the appointment panel
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Specifies the back color of the appointment panel."),
            DefaultValue(typeof(Color), "")]
        public Color BackColor
        {
            get
            {
                return m_colorBackColor;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_colorBackColor)
                {
                    this.Dirty = true;
                }

                m_colorBackColor = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a value indicating whether the block appointment is blocked.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Specifies whether the block appointment is blocked."),
            DefaultValue(false)]
        public virtual bool Blocked
        {
            get
            {
                return m_bBlocked;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_bBlocked)
                {
                    this.Dirty = true;
                }

                m_bBlocked = value;
            }
        }

        /// <summary>
        /// A property that gets or sets the custom tooltip text to be displayed for the item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets the custom tooltip text to be displayed for the appointment."),
            DefaultValue(""),]
        public string CustomToolTip
        {
            get
            {
                return m_sCustomToolTip;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_sCustomToolTip)
                {
                    this.Dirty = true;
                }

                m_sCustomToolTip = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a text string holding the details or comments for this appointment item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets a text string holding the details or comments for this appointment item."),
            DefaultValue("")]
        public virtual string Content
        {
            get
            {
                return content;
            }

            set
            {
                if (!IgnoreChanges && content != value)
                {
                    Dirty = true;
                }

                content = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a value indicating whether this item has been modified.
        /// </summary>
        [Browsable(false),
            Bindable(false),
            Description("Gets or sets an arbitrary object associated with this item."),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            DefaultValue(false),
            XmlIgnore]
        public virtual bool Dirty
        {
            get
            {
                return dirty;
            }

            set
            {
                dirty = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets the end time for this item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets the end time for this item.")]
        public virtual DateTime EndTime
        {
            get
            {
                return end;
            }

            set
            {
                if (!IgnoreChanges && end != value)
                {
                    Dirty = true;
                }

                end = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a value indicating whether changes to this item affect the Dirty property.
        /// </summary>
        [Browsable(false),
            Description("Gets or sets whether changes to this item affect the Dirty property."),
            DefaultValue(false),
            XmlIgnore]
        public virtual bool IgnoreChanges
        {
            get
            {
                return ignoreChanges;
            }

            set
            {
                dirty = ignoreChanges;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a integer categorizer value for this item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets a integer categorizer value for this item."),
            DefaultValue(0)]
        public virtual int LabelValue
        {
            get
            {
                return labelValue;
            }

            set
            {
                if (!IgnoreChanges && labelValue != value)
                {
                    Dirty = true;
                }

                labelValue = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a string associated with this item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets a string associated with this item."),
            DefaultValue(""),]
        public virtual string LocationValue
        {
            get
            {
                return locationValue;
            }

            set
            {
                if (!IgnoreChanges && locationValue != value)
                {
                    Dirty = true;
                }

                locationValue = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a integer categorizer value for this item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets a integer categorizer value for this item."),
            DefaultValue(0)]
        public virtual int MarkerValue
        {
            get
            {
                return markerValue;
            }

            set
            {
                if (!IgnoreChanges && markerValue != value)
                {
                    Dirty = true;
                }

                markerValue = value;
            }
        }

        /// <summary>
        /// A property that gets or sets a value indicating whether the block appointment is notify on overide.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Specifies whether the block appointment is notify on overide."),
            DefaultValue(true)]
        public bool NotifyOnOveride
        {
            get
            {
                return m_bNotifyOnOveride;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_bNotifyOnOveride)
                {
                    this.Dirty = true;
                }

                m_bNotifyOnOveride = value;
            }
        }

         /// <summary>
        /// A property that gets or sets a value indicating whether the block appointment is notify on overide.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Specifies whether the block appointment recurring date need override."),
            DefaultValue(false)]
        public bool RecurringOnOverride
        {
            get
            {
                return m_bRecurringOnOverride;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_bRecurringOnOverride)
                {
                    this.Dirty = true;
                }
                
                m_bRecurringOnOverride = value;
            }
        }
        /// <summary>
        /// A Virtual property that gets or sets an integer that can be used to identify the owner (if any) of this item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets an integer that can be used to identify the owner (if any)  of this item."),
            DefaultValue(0)]
        public virtual int Owner
        {
            get
            {
                return owner;
            }

            set
            {
                if (!IgnoreChanges && value != owner)
                {
                    Dirty = true;
                }

                owner = value;
            }
        }

        /// <summary>
        /// A property that gets or sets priority of the block appointment.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets priority of the block appointment."),
            DefaultValue(0)]
        public int Priority
        {
            get
            {
                return m_nPriority;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_nPriority)
                {
                    this.Dirty = true;
                }

                m_nPriority = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a value indicating whether you want a reminder event raised when the StartTime of this item gets close.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets whether you want a reminder event raised when the StartTime of this item gets close."),
            DefaultValue(false)]
        public virtual bool Reminder
        {
            get
            {
                return reminder;
            }

            set
            {
                if (!IgnoreChanges && reminder != value)
                {
                    Dirty = true;
                }

                reminder = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets the type of reminder event raised when the StartTime of this item gets close.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets the type of reminder event raised when the StartTime of this item gets close."),
            DefaultValue(0)]
        public virtual int ReminderValue
        {
            get
            {
                return reminderValue;
            }

            set
            {
                if (!IgnoreChanges && reminderValue != value)
                {
                    Dirty = true;
                }

                reminderValue = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets the start time for this item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets the start time for this item.")]
        public virtual DateTime StartTime
        {
            get
            {
                return start;
            }

            set
            {
                if (!IgnoreChanges && start != value)
                {
                    Dirty = true;
                }

                start = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a text string identifying the topic of this item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets a text string identifying the topic of this item."),
            DefaultValue("")]
        public virtual string Subject
        {
            get
            {
                return subject;
            }

            set
            {
                if (!IgnoreChanges && subject != value)
                {
                    Dirty = true;
                }

                subject = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets an arbitrary object associated with this item.
        /// </summary>
        [Browsable(false),
            Bindable(false),
            Description("Gets or sets an arbitrary object associated with this item."),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            XmlIgnore]
        public virtual object Tag
        {
            get
            {
                return tag;
            }

            set
            {
                if (!IgnoreChanges && tag != value)
                {
                    Dirty = true;
                }

                tag = value;
            }
        }

        /// <summary>
        /// A property that gets or sets color of appointment time span element.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Specifies the color of appointment time span element."),
            DefaultValue(typeof(Color), "")]
        public Color TimeSpanColor
        {
            get
            {
                return m_colorTimeSpanColor;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_colorTimeSpanColor)
                {
                    this.Dirty = true;
                }

                m_colorTimeSpanColor = value;
            }
        }

        /// <summary>
        /// A property that gets or sets the tooltip text to be displayed for the item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets the tooltip text to be displayed for the item."),
            DefaultValue(ScheduleAppointmentToolTip.All),]
        public ScheduleAppointmentToolTip ToolTip
        {
            get
            {
                return m_eAppointmentToolTip;
            }

            set
            {
                if (!this.IgnoreChanges && value != m_eAppointmentToolTip)
                {
                    this.Dirty = true;
                }

                m_eAppointmentToolTip = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a unique integer associated with this item.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Description("Gets or sets a unique integer associated with this item."),
            DefaultValue(0)]
        public virtual int UniqueID
        {
            get
            {
                return id;
            }

            set
            {
                if (!IgnoreChanges && value != id)
                {
                    Dirty = true;
                }

                id = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets in integer format version number.
        /// </summary>
        /// <remarks>
        /// If you modify this class in a manner that changes its serialization, you should change
        /// the version number so there is a unique number that can be used to identify the serialization
        /// format.
        /// </remarks>
        [Browsable(false),
            Bindable(false),
            Description("Gets in integer format version number."),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            DefaultValue(0)]
        public virtual int Version
        {
            get
            {
                return version;
            }
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// A method that creates a copy of this item; using MemberwiseClone.
        /// </summary>
        /// <returns>A new instance of this item.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// implemented to check UniqueID property for equality.
        /// </summary>
        /// <param name="obj">The object to compare this instance to.</param>
        /// <returns>Zero if the objects have the same UniqueID. Nonzero otherwise.</returns>
        public int CompareTo(object obj)
        {
            ScheduleAppointment item = obj as ScheduleAppointment;
            if (item != null)
            {
                ////return this.StartTime.CompareTo(item.StartTime);
                return this.UniqueID.Equals(item.UniqueID) ? 0 : this.GetHashCode().CompareTo(item.GetHashCode());
            }

            return 1;
        }

        #endregion

        /// <summary>
        /// Determine whether the item has conflict with other item.
        /// </summary>
        /// <param name="appVal">A IScheduleAppointment</param>
        /// <returns>True, it the item is in conflict; Fasle, otherwise.</returns>
        public virtual bool IsConflict(IScheduleAppointment appVal)
        {
            bool bRes = IsConflict(appVal.StartTime, appVal.EndTime);
            return bRes;
        }

        /// <summary>
        /// Determine whether the item has conflict with other item.
        /// </summary>
        /// <param name="dtStart">Item start time.</param>
        /// <param name="dtEnd">Item end time.</param>
        /// <returns>
        /// True, if the item is in conflict; False, otherwise.
        /// </returns>
        /// <exclude />
        public virtual bool IsConflict(DateTime dtStart, DateTime dtEnd)
        {
            bool bRes = MaxDate(dtStart, this.StartTime) < MinDate(dtEnd, this.EndTime);
            return bRes;
        }

        private static DateTime MaxDate(DateTime dtDateTime1, DateTime dtDateTime2)
        {
            DateTime dtMaxDate = (dtDateTime1 > dtDateTime2) ?
                dtDateTime1 : dtDateTime2;
            return dtMaxDate;
        }

        private static DateTime MinDate(DateTime dtDateTime1, DateTime dtDateTime2)
        {
            DateTime dtMinDate = (dtDateTime1 < dtDateTime2) ?
                dtDateTime1 : dtDateTime2;
            return dtMinDate;
        }

        /// <summary>
        /// An Overridden method that returns a text representation of the appointment.
        /// </summary>
        /// <returns>
        /// A string holding the schedule appointment data.
        /// </returns>
        /// <override/>
        public override string ToString()
        {
            if (this.AllDay)
            {
                return string.Format("subject:{0}", this.Subject);
            }
            else
            {
                return string.Format("[{0}  {1}] subject:{2}", this.StartTime, this.EndTime, this.Subject);
            }
        }
    }
    #endregion

    #region ScheduleAppointmentList

    /// <summary>
    /// A class that maintains collection of IScheduleAppointments that serves as the data for the Schedule Control.
    /// </summary>
    /// <remarks>
    /// This class is a wrapper class for an ArrayList and implements IComparer which orders the
    /// list by the item's StartTime. If two items start at the same time, then the
    /// EndTime is used as well to determine the order.
    /// </remarks>
    [Serializable]
    public class ScheduleAppointmentList : IScheduleAppointmentList
    {
        private ArrayList list;
        [NonSerialized]
        private StartDateComparer startDateComparer;

        /// <summary>
        /// Initializes a new instance of the ScheduleAppointmentList class.
        /// </summary>
        public ScheduleAppointmentList()
        {
            list = new ArrayList();
            startDateComparer = new StartDateComparer();
        }

        /// <summary>
        /// A read-only property that gets the number of IScheduleAppointments in this list.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return list.Count;
            }
        }

        int ICollection.Count
        {
            get
            {
                return this.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return this.List.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this.List.SyncRoot;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return this.List.IsFixedSize;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return this.List.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets list can be serialized in derived classes.
        /// </summary>
        protected ArrayList List
        {
            get { return list; }
            set { list = value; }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                IScheduleAppointment appVal = value as IScheduleAppointment;
                Debug.Assert(null != appVal, "unable to convert value to type IScheduleAppointment");
                this.List[index] = appVal;
            }
        }

        /// <summary>
        /// Gets or sets the i-th IScheduleAppointment in this list.
        /// </summary>
        /// <param name="i">The index to identify the desired ScheduleAppointment.</param>
        public virtual IScheduleAppointment this[int i]
        {
            get
            {
                if (i < 0 || i >= list.Count)
                {
                    throw new ArgumentOutOfRangeException("i", string.Format("out of range [0-{0}", list.Count - 1));
                }

                return list[i] as IScheduleAppointment;
            }

            set
            {
                if (i < 0 || i > list.Count)
                {
                    throw new ArgumentOutOfRangeException("i", string.Format("out of range [0-{0}", list.Count - 1));
                }

                list[i] = value;
            }
        }

        /// <summary>
        /// IComparer implementation that uses the StartTime/EndTime in its comparison.
        /// </summary>
        public class StartDateComparer : IComparer
        {
            #region IComparer Members

            /// <summary>
            /// Compares two ScheduleAppointment objects and returns a value indicating whether one is
            /// less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">First item.</param>
            /// <param name="y">Second item</param>
            /// <returns>1 if x > y, -1 if x &lt; y and zero otherwise.</returns>
        public int Compare(object x, object y)
        {
            IScheduleAppointment xItem = x as IScheduleAppointment;
            IScheduleAppointment yItem = y as IScheduleAppointment;
            if (xItem == null && yItem == null)
            {
                return 0;
            }
            else if (xItem == null)
            {
                return -1;
            }
            else if (yItem == null)
            {
                return 1;
            }
            else
            {
                int c = xItem.StartTime.CompareTo(yItem.StartTime);
                bool xSpan = RecurrenceSupport.IsSpanItem(xItem);
                bool ySpan = RecurrenceSupport.IsSpanItem(yItem);
                if (xSpan && ySpan)
                {
                    c = xItem.StartTime.Date.CompareTo(yItem.StartTime.Date);
                    if (c == 0)
                    {
                        c = ((IRecurringScheduleAppointment)xItem).DateList.BaseDate.CompareTo(((IRecurringScheduleAppointment)yItem).DateList.BaseDate);
                        if (c == 0)
                        {
                            c = -((IRecurringScheduleAppointment)xItem).DateList.TerminalDate.CompareTo(((IRecurringScheduleAppointment)yItem).DateList.TerminalDate);
                        }
                    }

                    return c;
                }
                else if (xItem.AllDay && yItem.AllDay)
                {
                    int c1 = xItem.StartTime.Date.CompareTo(yItem.StartTime.Date);
                    if (c1 == 0)
                    {
                        if (xSpan && !ySpan)
                        {
                            c = -1;
                        }
                        else if (!xSpan && ySpan)
                        {
                            c = 1;
                        }
                    }
                }
                else if (xSpan && !ySpan && yItem.StartTime.Date >= xItem.StartTime.Date
                   && yItem.EndTime.Date <= xItem.EndTime.Date)
                {
                    c = -1;
                }
                else if (ySpan && !xSpan && xItem.StartTime.Date >= yItem.StartTime.Date
                   && xItem.EndTime.Date <= yItem.EndTime.Date)
                {
                    c = 1;
                }

                if (c == 0)
                {
                    c = yItem.EndTime.CompareTo(xItem.EndTime);
                }

                return c;
            }
        }
            #endregion
        }

        /// <summary>
        /// A Virtual method that adds an IScheduleAppointment to this list.
        /// </summary>
        /// <param name="item">The IScheduleAppointment to be added.</param>
        public virtual void Add(IScheduleAppointment item)
        {
            list.Add(item);
        }
        
        /// <summary>
        /// Determines whether the collection contains the specified item.
        /// </summary>
        /// <param name="item">A IScheduleAppointment to search for in the collection.</param>
        /// <returns>true if the collection contains the specified item; otherwise, false. </returns>
        public bool Contains(IScheduleAppointment item) 
        {
            return list.Contains(item);
        }
        
        /// <summary>
        /// A Virtual method that finds the item of the key passed in. Not implemented.
        /// </summary>
        /// <param name="nUniqueID">The unique search key.</param>
        /// <returns>The found item.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public virtual IScheduleAppointment Find(int nUniqueID) 
        {
            IScheduleAppointment appRes = null;

            for (int i = 0; i < this.Count; i++) 
        {
                if (this[i].UniqueID == nUniqueID) 
                {
                    appRes = this[i] as IScheduleAppointment;
                    break;
                }
            }

            return appRes;
        }

////        public virtual IScheduleAppointment Find(object uniqueID)
////        {
////            ScheduleAppointment b = new ScheduleAppointment();
////            b.UniqueID = uniqueID;
////            int i = IndexOf(b);
////            if(i > -1)
////                return list[i] as IScheduleAppointment;
////            else
////                return null;
////        }

        /// <summary>
        /// A method that gets the iterator of the list.
        /// </summary>
        /// <returns>The iterator.</returns>
        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.List.CopyTo(array, index);
        }

        int IList.Add(object value)
        {
            IScheduleAppointment appVal = value as IScheduleAppointment;
            Debug.Assert(null != appVal, "unable to convert value to type IScheduleAppointment");
            this.Add(appVal);
            return 0;
        }

        void IList.Clear()
        {
            this.List.Clear();
        }

        bool IList.Contains(object value)
        {
            IScheduleAppointment appVal = value as IScheduleAppointment;
            Debug.Assert(null != appVal, "unable to convert value to type IScheduleAppointment");
            return this.Contains(appVal);
        }

        int IList.IndexOf(object value)
        {
            IScheduleAppointment appVal = value as IScheduleAppointment;
            Debug.Assert(null != appVal, "unable to convert value to type IScheduleAppointment");
            return this.IndexOf(appVal);
        }

        void IList.Insert(int index, object value)
        {
            IScheduleAppointment appVal = value as IScheduleAppointment;
            Debug.Assert(null != appVal, "unable to convert value to type IScheduleAppointment");
            this.Insert(index, appVal);
        }

        void IList.Remove(object value)
        {
            IScheduleAppointment appVal = value as IScheduleAppointment;
            Debug.Assert(null != appVal, "unable to convert value to type IScheduleAppointment");
            this.Remove(appVal);
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        /// <summary>
        /// A Virtual method that returns the position of the specified item within this list.
        /// </summary>
        /// <param name="item">The search item.</param>
        /// <returns>The index in the list of the search item.</returns>
        public virtual int IndexOf(IScheduleAppointment item)
        {
            return list.IndexOf(item);
        }

        /// <summary>
        /// A Virtual method that inserts an IScheduleAppointment into this list.
        /// </summary>
        /// <param name="index">The position in the list where the item is to be inserted.</param>
        /// <param name="item">The IScheduleAppointment to be inserted.</param>
        public virtual void Insert(int index, IScheduleAppointment item)
        {
            list.Insert(index, item);
        }
        
        /// <summary>
        /// Determine if any items overlap the schedule area.
        /// </summary>
        /// <param name="dtStart">Start date</param>
        /// <param name="nMinutes">Time interval</param>
        /// <returns>True, if items overlap schedule area; False otherwise.</returns>
        public virtual bool IsAreaAvailable(DateTime dtStart, double nMinutes)
        {
            DateTime dtEnd = dtStart;
            dtEnd = dtEnd.AddMinutes(nMinutes);

            return IsAreaAvailable(dtStart, dtEnd);
        }

        /// <summary>
        /// Determine if any items of this collection overlap the schedule area.
        /// </summary>
        /// <param name="dtStart">The start date</param>
        /// <param name="dtEnd">The end date</param>
        /// <returns>True, if items overlap schedule area; False otherwise.</returns>
        public virtual bool IsAreaAvailable(DateTime dtStart, DateTime dtEnd) 
        {
            bool bRes = true;

            foreach (IScheduleAppointment iApp in this) 
            {
                if (iApp.IsConflict(dtStart, dtEnd)) 
                {
                    bRes = false;
                    break;
                }
            }

            return bRes;
        }
        
        /// <summary>
        /// Determine whether item has conflict with items of this collection.
        /// </summary>
        /// <param name="appApp">A IScheduleAppointment</param>
        /// <returns>True, if the item is in conflict; False, otherwise.</returns>
        public virtual bool IsConflict(IScheduleAppointment appApp) 
        {
            bool bRes = false;

            foreach (IScheduleAppointment iApp in this) 
            {
                if (iApp.IsConflict(appApp))
                {
                    bRes = true;
                    break;
                }
            }

            return bRes;
        }

        /// <summary>
        /// A Virtual method that returns a default ScheduleAppointment.
        /// </summary>
        /// <returns>New ScheduleAppointment.</returns>
        public virtual IScheduleAppointment NewScheduleAppointment()
        {
            return new ScheduleAppointment();
        }
        
        /// <summary>
        /// Returns the next available slot after date.
        /// </summary>
        /// <param name="dtStart">Start date</param>
        /// <param name="nMinutes">Time interval</param>
        /// <returns>Next available slot.</returns>
        public virtual ScheduleAppointment NextAreaAvailable(DateTime dtStart, int nMinutes)
        {
            return NextAreaAvailable(dtStart, nMinutes, 30);
        }

        /// <summary>
        /// Returns the next available slot after date.
        /// </summary>
        /// <param name="dtStart">Start date</param>
        /// <param name="nMinutes">Time interval</param>
        /// <param name="nTimeStep">Time step in minutes</param>
        /// <returns>Next available slot.</returns>
        public virtual ScheduleAppointment NextAreaAvailable(DateTime dtStart, int nMinutes, int nTimeStep) 
        {
            return NextAreaAvailable(dtStart, nMinutes, nTimeStep, DateTime.MinValue, DateTime.MaxValue);
        }

        /// <summary>
        /// Returns the next available slot after date.
        /// </summary>
        /// <param name="dtStart">Start date</param>
        /// <param name="nMinutes">Time interval</param>
        /// <param name="nTimeStep">Time step in minutes</param>
        /// <param name="dtMinDate">Schedule min date</param>
        /// <param name="dtMaxDate">Schedule max date</param>
        /// <returns>Next available slot.</returns>
        public virtual ScheduleAppointment NextAreaAvailable(DateTime dtStart, int nMinutes, int nTimeStep, DateTime dtMinDate, DateTime dtMaxDate) 
        {
            return NextAreaAvailable(dtStart, nMinutes, nTimeStep, dtMinDate, dtMaxDate, 0, 24);
        }

        /// <summary>
        /// Returns the next available slot after date.
        /// </summary>
        /// <param name="dtStart">Start date</param>
        /// <param name="nMinutes">Time interval</param>
        /// <param name="nTimeStep">Time step in minutes</param>
        /// <param name="dtMinDate">Schedule min date</param>
        /// <param name="dtMaxDate">Schedule max date</param>
        /// <param name="nDayStartHour">Schedule day start hour</param>
        /// <param name="nDayEndHour">Schedule day end hour</param>
        /// <returns>Next available slot.</returns>
        public virtual ScheduleAppointment NextAreaAvailable(DateTime dtStart, int nMinutes, int nTimeStep, DateTime dtMinDate, DateTime dtMaxDate, int nDayStartHour, int nDayEndHour) 
        {
            ScheduleAppointment appAppointment = null;

            if (nTimeStep <= 0) 
            {
                nTimeStep = 30;
            }

            if (nMinutes <= 0) 
            {
                nMinutes = nTimeStep;
            }

            if (nDayStartHour < 0) 
            {
                nDayStartHour = 0;
            }

            if ((nDayEndHour <= nDayStartHour) || (nDayEndHour > 24))
            {
                nDayEndHour = 24;
            }

            if (dtStart < dtMinDate) 
            {
                dtStart = dtMinDate;
            }

            DateTime dtEnd = dtStart.AddMinutes(nMinutes);
            Debug.Assert(dtStart < dtEnd, " Start date is less then the end date");

            if (dtMinDate <= dtStart && dtMaxDate >= dtEnd)
            {
                if ((dtStart.Hour > nDayEndHour || dtStart.Hour < nDayStartHour)  ||
                     (dtEnd.Hour > nDayEndHour || dtEnd.Hour < nDayStartHour)) 
                {
                    dtStart = dtStart.AddHours(-dtStart.Hour + nDayStartHour);
                    dtEnd = dtStart.AddMinutes(nMinutes);
                }

                if (!IsAreaAvailable(dtStart, nMinutes))
                {
                    do
                    {
                        dtStart = dtStart.AddMinutes(nTimeStep);
                        dtEnd = dtStart.AddMinutes(nMinutes);

                        if (dtEnd > dtMaxDate)
                        {
                            break;
                        }

                        if (nDayEndHour < dtEnd.Hour || (nDayEndHour == dtEnd.Hour && 0 < dtEnd.Minute))
                        {
                            dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day + 1, nDayStartHour, 0, 0);
                            dtEnd = dtStart.AddMinutes(nMinutes);
                        }
                    }
                    while (!IsAreaAvailable(dtStart.AddMilliseconds(1), dtEnd.AddMilliseconds(-1)));
                }

                if ((dtMinDate <= dtStart) && (dtMaxDate >= dtEnd)) 
                {
                    appAppointment = this.NewScheduleAppointment() as ScheduleAppointment;
                    appAppointment.StartTime = dtStart;
                    appAppointment.EndTime = dtEnd;
                }
            }

            return appAppointment;
        }

        /// <summary>
        /// A Virtual method that removes an IScheduleAppointment from this list.
        /// </summary>
        /// <param name="item">The IScheduleAppointment to be removed.</param>
        public virtual void Remove(IScheduleAppointment item)
        {
            list.Remove(item);
        }

        /// <summary>
        /// A Virtual method that removes an IScheduleAppointment from this list.
        /// </summary>
        /// <param name="index">The position of the item to be removed.</param>
        public virtual void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

         /// <summary>
        /// A Virtual method that sorts this list on the IScheduleAppointment.StartTime property.
        /// </summary>
        public virtual void SortStartTime()
        {
            list.Sort(startDateComparer);
        }
 }

    #endregion

    #region ScheduleDataProvider

    /// <summary>
    /// A class that provides an empty implementation of IScheduleDataProvider. The implementation is done
    /// through virtual methods. You can then derive this class and through its overrides, set up an
    /// IScheduleDataProvider. See the ArrayListDataProvider class in the ScheduleSample sample.
    /// </summary>
    [Serializable]
    public class ScheduleDataProvider : IScheduleDataProvider
    {
        private SaveOnCloseBehavior autoCommitOnDestroy = SaveOnCloseBehavior.PromptBeforeSave;
        private bool isDirty = false;
        private ListObjectList labelList;
        private ListObjectList locationList;
        private IScheduleResourceList m_arrOwners = null;
        private ListObjectList markerList;
        private ListObjectList reminderList;

        /// <summary>
        /// Initializes a new instance of the ScheduleDataProvider class.
        /// </summary>
        public ScheduleDataProvider()
        {
            InitLists();
        }

        #region Properties
        /// <summary>
        /// A Virtual property that gets or sets a value indicating whether data source is modified or not.
        /// </summary>
        [XmlIgnore]
        public virtual bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        /// <summary>
        /// A property that gets or sets the list for the LabelValue options.
        /// </summary>
        protected ListObjectList LabelList
        {
            get
            {
                return labelList;
            }

            set
            {
                labelList = value;
            }
        }

        /// <summary>
        /// A property that gets or sets the list for the LocationValue options.
        /// </summary>
        protected ListObjectList LocationList
        {
            get
            {
                return locationList;
            }

            set
            {
                locationList = value;
            }
        }

        /// <summary>
        /// A property that gets or sets the list for the MarkerValue options.
        /// </summary>
        protected ListObjectList MarkerList
        {
            get
            {
                return markerList;
            }

            set
            {
                markerList = value;
            }
        }

        /// <summary>
        /// A property that gets or sets resources collection.
        /// </summary>
        [XmlIgnore]
        public IScheduleResourceList Owners
        {
            get
            {
                if (null == m_arrOwners)
                {
                    m_arrOwners = this.NewScheduleResourceList();
                }

                return m_arrOwners;
            }

            set
            {
                m_arrOwners = value;
            }
        }

        /// <summary>
        /// A property that gets or sets the list for the ReminderValue options.
        /// </summary>
        protected ListObjectList ReminderList
        {
            get
            {
                return reminderList;
            }

            set
            {
                reminderList = value;
            }
        }

        /// <summary>
        /// A property that gets or sets whether CommitChanges is called when the ScheduleControl is disposed.
        /// </summary>
        [XmlIgnore]
        public SaveOnCloseBehavior SaveOnCloseBehaviorAction
        {
            get { return autoCommitOnDestroy; }
            set { autoCommitOnDestroy = value; }
        }

        #endregion

        /// <summary>
        /// A Virtual method that adds IScheduleAppointment passed in. No implementation.
        /// </summary>
        /// <param name="item">The IScheduleAppointment to be added.</param>
        public virtual void AddItem(IScheduleAppointment item)
        {
        }

        /// <summary>
        /// A Virtual method that allows to commit the changes made in item. No implementation.
        /// </summary>
        public virtual void CommitChanges()
        {
            // TODO:  Add ScheduleDataProvider.CommitChanges implementation
        }

        /// <summary>
        /// A Virtual method that returns the list for the LabelValue options.
        /// </summary>
        /// <returns>LabelValue list.</returns>
        public virtual ILookUpObjectList GetLabels()
        {
            return LabelList;
        }

        /// <summary>
        /// A Virtual method that returns the list for the LocationValue options.
        /// </summary>
        /// <returns>LocationValue list.</returns>
        public virtual ILookUpObjectList GetLocations()
        {
            return LocationList;
        }

        /// <summary>
        /// A Virtual method that returns the list for the MarkerValue options.
        /// </summary>
        /// <returns>MarkerValue list.</returns>
        public virtual ILookUpObjectList GetMarkers()
        {
            return MarkerList;
        }

        /// <summary>
        /// A Virtual method that gets resources collection.
        /// </summary>
        /// <returns>The resources collection.</returns>
        public virtual IScheduleResourceList GetOwners()
        {
            return this.Owners;
        }

        /// <summary>
        /// A Virtual method that returns the list for the ReminderValue options.
        /// </summary>
        /// <returns>ReminderValue list.</returns>
        public virtual ILookUpObjectList GetReminders()
        {
            return ReminderList;
        }

        /// <summary>
        /// A Virtual method that gets the schedule added in dates passed in. No implementation.
        /// </summary>
        /// <param name="startDate">The first date.</param>
        /// <param name="endDate">The second date.</param>
        /// <returns>An IScheduleAppointmentList holding the schedule items between the given dates. </returns>
        public virtual IScheduleAppointmentList GetSchedule(DateTime startDate, DateTime endDate)
        {
            // TODO:  Add ScheduleDataProvider.ScheduleControl.IScheduleDataProvider.GetSchedule implementation
            return null;
        }

        /// <summary>
        /// A Virtual method that gets the schedule added in dates passed in for the respective owner. No implementation.
        /// </summary>
        /// <param name="startDate">The first date.</param>
        /// <param name="endDate">The second date.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>An IScheduleAppointmentList holding the schedule items for a particular owner between the given dates.</returns>
        public virtual IScheduleAppointmentList GetSchedule(DateTime startDate, DateTime endDate, int owner)
        {
            // TODO:  Add ScheduleDataProvider.ScheduleControl.IScheduleDataProvider.GetSchedule implementation
            return null;
        }

        /// <summary>
        /// A Virtual method that gets the schedule for the day passed in. No implementation.
        /// </summary>
        /// <param name="day">The DateTime</param>
        /// <returns>An IScheduleAppointmentList holding the schedule items for the given date. </returns>
        public virtual IScheduleAppointmentList GetScheduleForDay(DateTime day)
        {
            // TODO:  Add ScheduleDataProvider.GetSchedule implementation
            return null;
        }

        /// <summary>
        /// A Virtual method that gets the schedule for the day passed in for the respective owner. No implementation.
        /// </summary>
        /// <param name="day">The given date.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>An IScheduleAppointmentList holding the schedule items for a particular owner on the given date.</returns>
        public virtual IScheduleAppointmentList GetScheduleForDay(DateTime day, int owner)
        {
            // TODO:  Add ScheduleDataProvider.GetSchedule implementation
            return null;
        }

        /*/// <summary>
        /// Provides default droplists for entering IScheduleAppointment data.
        /// </summary>
        /// <remarks>
        /// You can override this method to provide customized droplists.
        /// </remarks>
        /// <example>
        /// Here is some sample code.
        /// <code lang="C#">
        ///
        /// public virtual void InitLists()
        /// {
        ///        labelList = new ListObjectList();
        ///        labelList.Add(new ListObject(0,"None", Color.White));
        ///        labelList.Add(new ListObject(1,"Important", Color.FromArgb(255,128,64)));
        ///        labelList.Add(new ListObject(2,"Business",  Color.FromArgb(86,152,233)));
        ///        labelList.Add(new ListObject(3,"Personal",  Color.FromArgb(57,210,53)));
        ///        labelList.Add(new ListObject(4,"Vacation",  Color.FromArgb(199,198,182)));
        ///        labelList.Add(new ListObject(5,"Must Attend",  Color.FromArgb(255,128,0)));
        ///        labelList.Add(new ListObject(6,"Travel Required",  Color.FromArgb(0,255,255)));
        ///        labelList.Add(new ListObject(7,"Needs Preparation",  Color.FromArgb(171,171,88)));
        ///        labelList.Add(new ListObject(8,"Birthday",  Color.FromArgb(186,117,255)));
        ///        labelList.Add(new ListObject(9,"Anniversary",  Color.FromArgb(255,128,64)));
        ///        labelList.Add(new ListObject(10,"Phone Call",  Color.FromArgb(255,128,64)));
        ///
        ///        markerList = new ListObjectList();
        ///        markerList.Add(new ListObject(0,"Free", Color.FromArgb(50, Color.RoyalBlue))); //same as noMarkColor
        ///        markerList.Add(new ListObject(1,"Tentative", Color.FromArgb(255, 206, 206)));
        ///        markerList.Add(new ListObject(2,"Busy",  Color.FromArgb(0,0,242)));
        ///        markerList.Add(new ListObject(3,"Out of Office",  Color.FromArgb(128, 0 ,64)));
        ///
        ///        reminderList = new ListObjectList();
        ///        reminderList.Add(new ListObject(0,"0 minutes", Color.White));
        ///        reminderList.Add(new ListObject(1,"5 minutes", Color.White));
        ///        reminderList.Add(new ListObject(2,"10 minutes", Color.White));
        ///        reminderList.Add(new ListObject(3,"15 minutes", Color.White));
        ///        reminderList.Add(new ListObject(4,"30 minutes", Color.White));
        ///        reminderList.Add(new ListObject(5,"1 hour", Color.White));
        ///        reminderList.Add(new ListObject(6,"2 hours", Color.White));
        ///        reminderList.Add(new ListObject(7,"3 hours", Color.White));
        ///        reminderList.Add(new ListObject(8,"4 hours", Color.White));
        ///        reminderList.Add(new ListObject(9,"5 hours", Color.White));
        ///        reminderList.Add(new ListObject(10,"6 hours", Color.White));
        ///        reminderList.Add(new ListObject(11,"7 hours", Color.White));
        ///        reminderList.Add(new ListObject(12,"8 hours", Color.White));
        ///        reminderList.Add(new ListObject(13,"9 hours", Color.White));
        ///        reminderList.Add(new ListObject(14,"10 hours", Color.White));
        ///        reminderList.Add(new ListObject(15,"11 hours", Color.White));
        ///        reminderList.Add(new ListObject(16,"12 hours", Color.White));
        ///        reminderList.Add(new ListObject(17,"18 hours", Color.White));
        ///        reminderList.Add(new ListObject(18,"1 day", Color.White));
        ///        reminderList.Add(new ListObject(19,"2 days", Color.White));
        ///        reminderList.Add(new ListObject(20,"3 days", Color.White));
        ///        reminderList.Add(new ListObject(21,"4 days", Color.White));
        ///        reminderList.Add(new ListObject(22,"1 week", Color.White));
        ///        reminderList.Add(new ListObject(23,"2 weeks", Color.White));
        ///
        ///        this.locationList = new ListObjectList();
        ///        locationList.Add(new ListObject(0,"", Color.White));
        ///        locationList.Add(new ListObject(1,"RoomB", Color.White));
        ///        locationList.Add(new ListObject(2,"RoomC", Color.White));
        ///        locationList.Add(new ListObject(3,"RoomD", Color.White));
        ///        locationList.Add(new ListObject(4,"RoomE", Color.White));
        ///    }
        /// </code>
        /// <code lang="VB">
        /// Public Overridable Sub InitLists()
        ///        labelList = New ListObjectList()
        ///        labelList.Add(New ListObject(0, "None", Color.White))
        ///        labelList.Add(New ListObject(1, "Important", Color.FromArgb(255, 128, 64)))
        ///        labelList.Add(New ListObject(2, "Business", Color.FromArgb(86, 152, 233)))
        ///        labelList.Add(New ListObject(3, "Personal", Color.FromArgb(57, 210, 53)))
        ///        labelList.Add(New ListObject(4, "Vacation", Color.FromArgb(199, 198, 182)))
        ///        labelList.Add(New ListObject(5, "Must Attend", Color.FromArgb(255, 128, 0)))
        ///        labelList.Add(New ListObject(6, "Travel Required", Color.FromArgb(0, 255, 255)))
        ///        labelList.Add(New ListObject(7, "Needs Preparation", Color.FromArgb(171, 171, 88)))
        ///        labelList.Add(New ListObject(8, "Birthday", Color.FromArgb(186, 117, 255)))
        ///        labelList.Add(New ListObject(9, "Anniversary", Color.FromArgb(255, 128, 64)))
        ///        labelList.Add(New ListObject(10, "Phone Call", Color.FromArgb(255, 128, 64)))
        ///
        ///        markerList = New ListObjectList()
        ///        markerList.Add(New ListObject(0, "Free", Color.FromArgb(50, Color.RoyalBlue))) 'same as noMarkColor
        ///        markerList.Add(New ListObject(1, "Tentative", Color.FromArgb(255, 206, 206)))
        ///        markerList.Add(New ListObject(2, "Busy", Color.FromArgb(0, 0, 242)))
        ///        markerList.Add(New ListObject(3, "Out of Office", Color.FromArgb(128, 0, 64)))
        ///
        ///        reminderList = New ListObjectList()
        ///        reminderList.Add(New ListObject(0, "0 minutes", Color.White))
        ///        reminderList.Add(New ListObject(1, "5 minutes", Color.White))
        ///        reminderList.Add(New ListObject(2, "10 minutes", Color.White))
        ///        reminderList.Add(New ListObject(3, "15 minutes", Color.White))
        ///        reminderList.Add(New ListObject(4, "30 minutes", Color.White))
        ///        reminderList.Add(New ListObject(5, "1 hour", Color.White))
        ///        reminderList.Add(New ListObject(6, "2 hours", Color.White))
        ///        reminderList.Add(New ListObject(7, "3 hours", Color.White))
        ///        reminderList.Add(New ListObject(8, "4 hours", Color.White))
        ///        reminderList.Add(New ListObject(9, "5 hours", Color.White))
        ///        reminderList.Add(New ListObject(10, "6 hours", Color.White))
        ///
        ///        Me.locationList = New ListObjectList()
        ///        locationList.Add(New ListObject(0, "", Color.White))
        ///        locationList.Add(New ListObject(1, "RoomB", Color.White))
        ///        locationList.Add(New ListObject(2, "RoomC", Color.White))
        ///        locationList.Add(New ListObject(3, "RoomD", Color.White))
        ///        locationList.Add(New ListObject(4, "RoomE", Color.White))
        ///    End Sub 'InitLists
        /// </code>
        /// </example>
        ///
        ////        reminderList.Add(New ListObject(11, "7 hours", Color.White))*/
        //        reminderList.Add(New ListObject(12, "8 hours", Color.White))
        //        reminderList.Add(New ListObject(13, "9 hours", Color.White))
        //        reminderList.Add(New ListObject(14, "10 hours", Color.White))
        //        reminderList.Add(New ListObject(15, "11 hours", Color.White))
        //        reminderList.Add(New ListObject(16, "12 hours", Color.White))
        //        reminderList.Add(New ListObject(17, "18 hours", Color.White))
        //        reminderList.Add(New ListObject(18, "1 day", Color.White))
        //        reminderList.Add(New ListObject(19, "2 days", Color.White))
        //        reminderList.Add(New ListObject(20, "3 days", Color.White))
        //        reminderList.Add(New ListObject(21, "4 days", Color.White))
        //        reminderList.Add(New ListObject(22, "1 week", Color.White))
        //        reminderList.Add(New ListObject(23, "2 weeks", Color.White))

        /// <summary>
        /// A Virtual method that provides default droplists for entering IScheduleAppointment data.
        /// </summary>
        /// <remarks>
        /// You can override this method to provide customized droplists.
        /// </remarks>
        public virtual void InitLists()
        {
            labelList = new ListObjectList();
            labelList.Add(new ListObject(0, "None", Color.White));
            labelList.Add(new ListObject(1, "Important", Color.FromArgb(255, 128, 64)));
            labelList.Add(new ListObject(2, "Business", Color.FromArgb(86, 152, 233)));
            labelList.Add(new ListObject(3, "Personal", Color.FromArgb(57, 210, 53)));
            labelList.Add(new ListObject(4, "Vacation", Color.FromArgb(199, 198, 182)));
            labelList.Add(new ListObject(5, "Must Attend", Color.FromArgb(255, 128, 0)));
            labelList.Add(new ListObject(6, "Travel Required", Color.FromArgb(0, 255, 255)));
            labelList.Add(new ListObject(7, "Needs Preparation", Color.FromArgb(171, 171, 88)));
            labelList.Add(new ListObject(8, "Birthday", Color.FromArgb(186, 117, 255)));
            labelList.Add(new ListObject(9, "Anniversary", Color.FromArgb(255, 128, 64)));
            labelList.Add(new ListObject(10, "Phone Call", Color.FromArgb(255, 128, 64)));

            markerList = new ListObjectList();
            markerList.Add(new ListObject(0, "Free", Color.FromArgb(50, Color.RoyalBlue))); ////same as noMarkColor
            markerList.Add(new ListObject(1, "Tentative", Color.FromArgb(255, 206, 206)));
            markerList.Add(new ListObject(2, "Busy", Color.FromArgb(0, 0, 242)));
            markerList.Add(new ListObject(3, "Out of Office", Color.FromArgb(128, 0, 64)));

            reminderList = new ListObjectList();
            reminderList.Add(new ListObject(0, "0 minutes", Color.White));
            reminderList.Add(new ListObject(1, "5 minutes", Color.White));
            reminderList.Add(new ListObject(2, "10 minutes", Color.White));
            reminderList.Add(new ListObject(3, "15 minutes", Color.White));
            reminderList.Add(new ListObject(4, "30 minutes", Color.White));
            reminderList.Add(new ListObject(5, "1 hour", Color.White));
            reminderList.Add(new ListObject(6, "2 hours", Color.White));
            reminderList.Add(new ListObject(7, "3 hours", Color.White));
            reminderList.Add(new ListObject(8, "4 hours", Color.White));
            reminderList.Add(new ListObject(9, "5 hours", Color.White));
            reminderList.Add(new ListObject(10, "6 hours", Color.White));
            reminderList.Add(new ListObject(11, "7 hours", Color.White));
            reminderList.Add(new ListObject(12, "8 hours", Color.White));
            reminderList.Add(new ListObject(13, "9 hours", Color.White));
            reminderList.Add(new ListObject(14, "10 hours", Color.White));
            reminderList.Add(new ListObject(15, "11 hours", Color.White));
            reminderList.Add(new ListObject(16, "12 hours", Color.White));
            reminderList.Add(new ListObject(17, "18 hours", Color.White));
            ////reminderList.Add(new ListObject(18, "1 day", Color.White));
            ////reminderList.Add(new ListObject(19, "2 days", Color.White));
            ////reminderList.Add(new ListObject(20, "3 days", Color.White));
            ////reminderList.Add(new ListObject(21, "4 days", Color.White));
            ////reminderList.Add(new ListObject(22, "1 week", Color.White));
            ////reminderList.Add(new ListObject(23, "2 weeks", Color.White));
            this.locationList = new ListObjectList();
            locationList.Add(new ListObject(0, string.Empty, Color.White));
            locationList.Add(new ListObject(1, "RoomB", Color.White));
            locationList.Add(new ListObject(2, "RoomC", Color.White));
            locationList.Add(new ListObject(3, "RoomD", Color.White));
            locationList.Add(new ListObject(4, "RoomE", Color.White));
        }

        /// <summary>
        /// Returns a default ScheduleAppointment.
        /// </summary>
        /// <returns>New ScheduleAppointment.</returns>
        public virtual IScheduleAppointment NewScheduleAppointment()
        {
            return new ScheduleAppointment();
        }

        /// <summary>
        /// Returns a default ScheduleAppointmentList.
        /// </summary>
        /// <returns>New ScheduleAppointmentList.</returns>
        public virtual IScheduleAppointmentList NewScheduleAppointmentList()
        {
            return new ScheduleAppointmentList();
        }

        /// <summary>
        /// Returns a default ScheduleResource.
        /// </summary>
        /// <returns>New ScheduleResource.</returns>
        public virtual IScheduleResource NewScheduleResource()
        {
            return new ScheduleResource();
        }

        /// <summary>
        /// Returns a default ScheduleAppointmentList.
        /// </summary>
        /// <returns>New ScheduleAppointment.</returns>
        public virtual IScheduleResourceList NewScheduleResourceList()
        {
            return new ScheduleResourceList();
        }

        /// <summary>
        /// No implementation.
        /// </summary>
        /// <param name="item">The IScheduleAppointment to be removed.</param>
        public virtual void RemoveItem(IScheduleAppointment item)
        {
        }

        /// <summary>
        /// No implementation.
        /// </summary>
        /// <param name="appModifiedItem">The modified IScheduleAppointment.</param>
        /// <param name="appOriginalItem">The original IScheduleAppointment.</param>
        public virtual void SaveModifiedItem(IScheduleAppointment appModifiedItem, IScheduleAppointment appOriginalItem)
        {
        }

        ///// <summary>
        ///// Gets recurring appointments are supported.
        ///// </summary>
        ///// <remarks>This property must be overridden to support recurrences.</remarks>
        ////public virtual bool SupportsRecurrences
        ////{
        //    get { return false; }
        ////}
    }
    #endregion

    #region ScheduleResource

    /// <summary>
    /// A class that represents a single schedule resource.
    /// </summary>
    [Serializable]
    public class ScheduleResource : IScheduleResource
    {
        #region Fields

        private int m_nUniqueID = 0;
        private string m_sName = string.Empty;

        #endregion //Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ScheduleResource class.
        /// </summary>
        public ScheduleResource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ScheduleResource class using the specified resource UniqueID.
        /// </summary>
        /// <param name="nUniqueID">Resource unique id.</param>
        public ScheduleResource(int nUniqueID) :
            this()
        {
            m_nUniqueID = nUniqueID;
        }

        /// <summary>
        /// Initializes a new instance of the ScheduleResource class using the specified resource UniqueID and name.
        /// </summary>
        /// <param name="nUniqueID">Resource unique id.</param>
        /// <param name="sName">Resource name.</param>
        public ScheduleResource(int nUniqueID, string sName) :
            this(nUniqueID) 
        {
            m_sName = sName;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// A Virtual property that gets/sets text to be displayed for the resources in Schedule.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Localizable(true),
            Category("Misc"),
            DefaultValue(""),
            Description("Gets or sets a text to be diplayed as a resource in the Schedule.")]
        public virtual string Name
        {
            get
            {
                return m_sName;
            }

            set
            {
                m_sName = value;
            }
        }

        /// <summary>
        /// A Virtual property that gets or sets a unique integer associated with this resource.
        /// </summary>
        [Browsable(true),
            Bindable(true),
            Category("Misc"),
            DefaultValue(0),
            Description("Gets or sets a unique integer associated with this resource.")]
        public virtual int UniqueID
        {
            get
            {
                return m_nUniqueID;
            }

            set
            {
                m_nUniqueID = value;
            }
        }

        #endregion //Properties

        #region Overrides

        /// <summary>An Overridden method that returns the string representation of ScheduleResource object.</summary>
        /// <returns>A string holding the ScheudleResource object.</returns>
        /// <override/>
        public override string ToString()
        {
            string sRes = this.Name;
            if ((null == sRes) || (String.Empty == sRes))
            {
                sRes = "Resource";
            }

            return sRes;
        }

        #endregion //Overrides
    }

    #endregion //ScheduleResource

    #region ScheduleResourceCollection

    /// <summary>
    /// A Collection of IScheduleResources to be displayed in Schedule's resource row.
    /// </summary>
    /// <remarks>
    /// Collection contains objects displayed in a Schedule as additional columns/rows of a Schedule.
    /// Resource objects allow for displaying schedules for different resources or schedules along each other.
    /// This allows for easy analysis and comparing two or more schedules linked to Resource objects.
    /// If there is no objects in a collection then the Schedule displays one schedule
    /// </remarks>
    [Serializable]
    public class ScheduleResourceList : CollectionBase, IScheduleResourceList
    {
        /// <summary>
        /// Initializes a new instance of the ScheduleResourceList class.
        /// </summary>
        public ScheduleResourceList()
        {
        }

        /// <summary>
        /// A Virtual method that appends the IScheduleResource to the end of the ScheduleResourceCollection.
        /// </summary>
        /// <param name="resource">The IScheduleResource to add to the ScheduleResourceCollection.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        public virtual int Add(IScheduleResource resource) 
        {
            return base.List.Add(resource);
        }
        
        /// <summary>
        /// Determines whether the collection contains the specified resource.
        /// </summary>
        /// <param name="resource">A IScheduleResource to search for in the collection.</param>
        /// <returns>true if the collection contains the specified item; otherwise, false. </returns>
        public virtual bool Contains(IScheduleResource resource) 
        {
            return base.List.Contains(resource);
        }

        /// <summary>
        /// A method that finds resource by UniqueID.
        /// </summary>
        /// <param name="nOwnerID">resource UniqueID</param>
        /// <returns>A IScheduleResource</returns>
        public IScheduleResource Find(int nOwnerID) 
        {
            IScheduleResource iRes = null;

            foreach (IScheduleResource iVal in this) 
            {
                if (iVal.UniqueID == nOwnerID) 
                {
                    iRes = iVal;
                    break;
                }
            }

            return iRes;
        }
        
        /// <summary>
        /// Determines the index value that represents the position of the IScheduleResource in the ScheduleResourceCollection.
        /// </summary>
        /// <param name="resource">The IScheduleResource to locate in the ScheduleResourceCollection.</param>
        /// <returns>The index position of the specified IScheduleResource in the collection.</returns>
        public virtual int IndexOf(IScheduleResource resource) 
        {
            return base.List.IndexOf(resource);
        }

        /// <summary>
        /// A Virtual method that inserts a IScheduleResource in the ScheduleResourceCollection at the specified index location.
        /// </summary>
        /// <param name="nIdx">The location in the collection to insert the IScheduleResource.</param>
        /// <param name="resource">A IScheduleResource to add to the collection.</param>
        public virtual void Insert(int nIdx, IScheduleResource resource) 
        {
            base.List.Insert(nIdx, resource);
        }
        
        /// <summary>
        /// Returns a default ScheduleResource.
        /// </summary>
        /// <returns>New ScheduleResource.</returns>
        public virtual IScheduleResource NewScheduleResource()
        {
            return new ScheduleResource();
        }

        /// <summary>
        /// A Virtual method that removes the specified IScheduleResource from the ScheduleResourceCollection.
        /// </summary>
        /// <param name="resource">The IScheduleResource to remove from the collection.</param>
        public virtual void Remove(IScheduleResource resource) 
        {
            base.List.Remove(resource);
        }

        /// <summary>
        /// Declares indexer.
        /// </summary>
        /// <param name="nIdx">The zero-based index at which value should be returned.</param>
        /// <returns>A IScheduleResource</returns>
        public IScheduleResource this[int nIdx]
        {
            get
            {
                return (IScheduleResource) this.List[nIdx];
            }

            set
            {
                this.List[nIdx] = value;
            }
        }
    }

    #endregion //ScheduleResourceCollection
}
