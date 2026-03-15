//-------------------------------------------------------------------------------------------------
// <copyright file="IScheduleAppointment.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace Syncfusion.Schedule
{
    #region ILookUpObject
    /// <summary>
    /// An interface that defines items that can be included in a <see cref="ILookUpObjectList"/>.
    /// </summary>
    /// <remarks>
    /// Choice lists within the ScheduleControl are used to provide possible
    /// schedule item information like location or a reminder. ILookUpObject
    /// allows such list items to have a ValueMember/DisplayMember associated with
    /// the choices as well as a color that will be used in dropdowns showing these
    /// lists. Value members are normally the values serialized to data stores.
    /// </remarks>
    public interface ILookUpObject
    {
        /// <summary>
        /// A property that gets/sets a color associated with this item.
        /// </summary>
        Color ColorMember { get; set; }

        /// <summary>
        /// A property that gets/sets display member associated with this item.
        /// </summary>
        string DisplayMember { get; set; }

        /// <summary>
        /// A property that gets/sets value member associated with this item.
        /// </summary>
        int ValueMember { get; set; }
    }
    #endregion

    #region ILookUpObjectList

    /// <summary>
    /// An Interface that defines Collection of <see cref="ILookUpObject"/> items.
    /// </summary>
    public interface ILookUpObjectList
    {
        /// <summary>
        /// Indexer that returns a <see cref="ILookUpObject"/>.
        /// </summary>
        /// <param name="i">Get i th Object</param>        
        ILookUpObject this[int i] 
        { 
            get; set;
        }
    }

    #endregion

    #region IRecurringScheduleAppointment
    /// <summary>
    /// Interface defining individual schedule items.
    /// </summary>
    /// <remarks>
    /// IScheduleAppointments must implement IComparable and ICloneable. IComparable is used to order
    /// the incidents in sorts so the items can be properly arranged in the schedule. ICloneable is
    /// also required to handle support for drag and drop in the ScheduleControl.
    /// </remarks>
    public interface IRecurringScheduleAppointment : IScheduleAppointment, IComparable, ICloneable
    {
        /// <summary>
        /// Gets or sets the recurrenceList holding the dates associated with this recurring appointment.
        /// </summary>
        RecurrenceList DateList { get; set; }

        /// <summary>
        /// Gets or sets the RecurrenceRule including both start and end dates.
        /// </summary>
        string RecurrenceRule { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier assigned to this recurrence appointment.
        /// </summary>
        int RecurrenceRuleID { get; set; }
    }
    #endregion

    #region IRecurringScheduleDataProvider
    /// <summary>
    /// Implement this interface to support recurring appointment in your IScheduleDataProvider.
    /// </summary>
    public interface IRecurringScheduleDataProvider
    {
        /// <summary>
        /// Gets a list of recurring appointments.
        /// </summary>
        IScheduleAppointmentList RecurringList
        {
            get;
        }
        
        /// <summary>
        /// Initially adds recurring appointments to the dataprovider.
        /// </summary>
        /// <param name="item">The recurring appointment definition.</param>
        /// <param name="dateLimit">The minimal date up to which all recurring appointments should be added.</param>
        /// <remarks>Use this method on the initial load to handle adding recurring appointments.</remarks>
        void AddNewRecurringAppointments(IRecurringScheduleAppointment item, DateTime dateLimit);

        /// <summary>
        /// Used after the initial load to add additional recurring appointments to the dataprovider.
        /// </summary>
        /// <param name="date">The date to which recurring appointmets need to be extended.</param>
        /// <returns>True if dates were added.</returns>
        /// <remarks>Dynamically provide appointments on demand as new dates are exposed.</remarks>
        bool CheckAndAddIfNeededRecurringAppointments(DateTime date);

        /// <summary>
        /// Returns a unique integer that serves to identify a recurring family of appointments.
        /// </summary>
        /// <returns>A unique integer.</returns>
        int GetUniqueID();

        /// <summary>
        /// Removes all occurrences the given appointment.
        /// </summary>
        /// <param name="item">Recurring appointment form.</param>
        /// <returns>True if the operation succeeds.</returns>
        bool RemoveRecurringAppointments(IRecurringScheduleAppointment item);

        /// <summary>
        /// Makes changes to appointments in a recurring sequence of appointments.
        /// </summary>
        /// <param name="modifiedItem">The edited appointment.</param>
        /// <param name="originalItem">The un-edited appointment.</param>
        /// <param name="action">The requested edit action.</param>
        void SaveModifiedRecurringAppointment(IRecurringScheduleAppointment modifiedItem, IRecurringScheduleAppointment originalItem, RecurringAppointmentEditAction action);
    }
    #endregion

    #region IScheduleAppointment

    /// <summary>
    /// An Interface defining individual schedule items.
    /// </summary>
    /// <remarks>
    /// IScheduleAppointments must implement IComparable and ICloneable. IComparable is used to order
    /// the incidents in sorts so the items can be properly arranged in the schedule. ICloneable is
    /// also required to handle support for drag and drop in the ScheduleControl.
    /// </remarks>
    public interface IScheduleAppointment : IComparable, ICloneable
    {
        /// <summary>
        /// A property that gets/sets a value indicating whether allday appointment or not.
        /// </summary>
        bool AllDay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not blocked appointment is allowed to click.
        /// </summary>
        bool AllowClickable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not an appointment is allowed to drag.
        /// </summary>
        bool AllowDrag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not an appointment is allowed to resize and modify its duration.
        /// </summary>
        bool AllowResize
        {
            get;
            set;
        }

        /// <summary>
        /// A property that gets/sets appointment html element background color.
        /// </summary>
        Color BackColor
        {
            get;
            set;
        }

        /// <summary>
        /// A property that gets/sets text displayed as the main content of this schedule item.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// A property that gets or sets associates ToolTip text with the appointment.
        /// </summary>
        string CustomToolTip
        {
            get;
            set;
        }

        /// <summary>
        /// A property that gets/sets a value indicating whether or not this items has been modified.
        /// </summary>
        bool Dirty { get; set; }

        /// <summary>
        /// A property that gets/sets endtime for this item.
        /// </summary>
        DateTime EndTime { get; set; }

        /// <summary>
        /// A property that gets or sets a value indicating whether or not changes to this item should be ignored.
        /// </summary>
        bool IgnoreChanges { get; set; }

        /// <summary>
        /// A property that gets or sets a categorizer value for this item.
        /// </summary>
        int LabelValue { get; set; }

        /// <summary>
        /// A property that gets or sets some item dependent string-like the location of a meeting.
        /// </summary>
        string LocationValue { get; set; }

        /// <summary>
        /// A property that gets or sets some item dependent string-like the location of a meeting.
        /// </summary>
        bool RecurringOnOverride { get; set; }

        /// <summary>
        /// A property that gets or sets categorizer value for this item.
        /// </summary>
        int MarkerValue { get; set; }

        /// <summary>
        /// A property that gets or sets owner of this schedule item.
        /// </summary>
        int Owner { get; set; }

        /// <summary>
        /// A property that gets or sets a value indicating whether or not some reminder action should be taken as this item becomes current.
        /// </summary>
        bool Reminder { get; set; }

        /// <summary>
        /// A property that gets or sets a remind value of this item
        /// </summary>
        int ReminderValue { get; set; }

        /// <summary>
        /// A property that gets or sets starttime of this item.
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// A property that gets or sets subject or topic title for this schedule item.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// A property that gets or sets arbitrary object associated with this item.
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// A property that gets or sets color of appointment time span element.
        /// </summary>
        Color TimeSpanColor
        {
            get;
            set;
        }

        /// <summary>
        /// A property that gets or sets ToolTip text with the appointment.
        /// </summary>
        ScheduleAppointmentToolTip ToolTip
        {
            get;
            set;
        }

        /// <summary>
        /// A property that gets or sets a unique identifier for this schedule item.
        /// </summary>
        int UniqueID { get; set; }

        /// <summary>
        /// A read-only property that gets a version associated with the data schema for this item.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Determine whether item has conflict with other item.
        /// </summary>
        /// <param name="item">A IScheduleAppointment</param>
        /// <returns>True if the item is in conflict.</returns>
        bool IsConflict(IScheduleAppointment item);

        /// <summary>
        /// Determine whether item has conflict with other item.
        /// </summary>        
        /// <param name="dtStart">start time</param>
        /// <param name="dtEnd">end time</param>
        /// <returns>True if the item is in conflict.</returns>
        bool IsConflict(DateTime dtStart, DateTime dtEnd);
    }
    #endregion

    #region IScheduleAppointmentList

    /// <summary>
    /// An Interface defining collection of schedule items.
    /// </summary>
    public interface IScheduleAppointmentList :
        IList
    {
        /// <summary>
        /// A read-only property thats gets number of IScheduleAppointments in this list.
        /// </summary>
        new int Count 
        {
            get;
        }

        /// <summary>
        /// An IScheduleAppointment referenced through an indexer.
        /// </summary>
        /// <param name="i"> Get the ScheduleAppointment based on index </param>
        new IScheduleAppointment this[int i] 
        {
            get; set;
        }

        /// <summary>
        /// A method that adds an IScheduleAppointment at the end of this collection.
        /// </summary>
        /// <param name="item">An IScheduleAppointment item.</param>
        void Add(IScheduleAppointment item);

        /// <summary>
        /// A method that finds an IScheduleAppointment in the collection using its IScheduleAppointment.UniqueID.
        /// </summary>
        /// <param name="uniqueID">The IScheduleAppointment.UniqueID of the IScheduleAppointment being searched for.</param>
        /// <returns>The IScheduleAppointment whose IScheduleAppointment.UniqueID is the given uniqueID.</returns>
        IScheduleAppointment Find(int uniqueID);

        /// <summary>
        /// A method that locates the index of a particular IScheduleAppointment in this collection.
        /// </summary>
        /// <param name="item">The item to be located.</param>
        /// <returns>The index of the given item.</returns>
        int IndexOf(IScheduleAppointment item);

        /// <summary>
        /// A method that inserts the given IScheduleAppointment at a particular position in this collection.
        /// </summary>
        /// <param name="index">Where the item should be inserted.</param>
        /// <param name="item">The item to be inserted.</param>
        void Insert(int index, IScheduleAppointment item);

        /// <summary>
        /// A method that returns an instance of a new schedule item.
        /// </summary>
        /// <returns>A new schedule item that can be added to this list. </returns>
        IScheduleAppointment NewScheduleAppointment();

        /// <summary>
        /// A method that removes a given IScheduleAppointment from this collection.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        void Remove(IScheduleAppointment item);

        /// <summary>
        /// A method that removes an IScheduleAppointment at the given index from this collection.
        /// </summary>
        /// <param name="index">The index of the IScheduleAppointment to be removed.</param>
        new void RemoveAt(int index);

        /// <summary>
        /// A method that arranges the IScheduleAppointments in this list according to IScheduleAppointment.StartTime.
        /// </summary>
        void SortStartTime();
    }

    #endregion

    #region IScheduleDataProvider

    /// <summary>
    /// An Interface defining the framework for providing schedule item data to the ScheduleControl.
    /// </summary>
    public interface IScheduleDataProvider
    {
        /// <summary>
        /// A property that gets/sets a value indicating whether data source is modified or not.
        /// </summary>
        bool IsDirty
        {
            get;
            set;
        }

        /// <summary>
        /// A property that gets/sets the SaveOnCloseBehavioourAction which determines whether CommitChanges is called when the toplevel Form holding the ScheduleControl is closed.
        /// </summary>
        SaveOnCloseBehavior SaveOnCloseBehaviorAction
        {
            get;
            set;
        }

        /// <summary>
        /// A method that adds a schedule item to this list.
        /// </summary>
        /// <param name="item">The schedule item to be added.</param>
        void AddItem(IScheduleAppointment item);

        /// <summary>
        /// A method that can be called when the ScheduleControl needs to save modifications to the schedule
        /// items back to the data store.
        /// </summary>
        void CommitChanges();

        /// <summary>
        /// A method that returns a list holding the possible values for the <see cref="IScheduleAppointment.LabelValue"/> property.
        /// </summary>
        /// <returns>A list of possible values.</returns>
        ILookUpObjectList GetLabels();

        /// <summary>
        /// A method that returns a list holding the possible values for the <see cref="IScheduleAppointment.LocationValue"/> property.
        /// </summary>
        /// <returns>A list of possible values.</returns>
        ILookUpObjectList GetLocations();

        /// <summary>
        /// A method that returns a list holding the possible values for the <see cref="IScheduleAppointment.MarkerValue"/> property.
        /// </summary>
        /// <returns>A list of possible values.</returns>
        ILookUpObjectList GetMarkers();

        /// <summary>
        /// A method that returns a list holding the possible values for the <see cref="IScheduleAppointment.Owner"/> property.
        /// </summary>
        /// <returns>Resources collection</returns>
        IScheduleResourceList GetOwners();

        /// <summary>
        /// A method that returns a list holding the possible values for the <see cref="IScheduleAppointment.ReminderValue"/> property.
        /// </summary>
        /// <returns>A list of possible values.</returns>
        ILookUpObjectList GetReminders();

        /// <summary>
        /// A method that gets a list of schedule items for a range of dates.
        /// </summary>
        /// <param name="startDate">The start date of the requested range.</param>
        /// <param name="endDate">The end date of the requested range.</param>
        /// <returns>IScheduleAppointmentList object that holds the list of schedule items.</returns>
        IScheduleAppointmentList GetSchedule(DateTime startDate, DateTime endDate);

        /// <summary>
        /// A method that gets a list of schedule items for a range of dates and a specified owner.
        /// </summary>
        /// <param name="startDate">The start date of the requested range.</param>
        /// <param name="endDate">The end date of the requested range.</param>
        /// <param name="owner">The requested owner.</param>
        /// <returns>IScheduleAppointmentList object that holds the list of schedule items.</returns>
        IScheduleAppointmentList GetSchedule(DateTime startDate, DateTime endDate, int owner);

        /// <summary>
        /// A method that gets a list of schedule items for a particular day.
        /// </summary>
        /// <param name="day">The requested date.</param>
        /// <returns>IScheduleAppointmentList object that holds the list of schedule items.</returns>
        IScheduleAppointmentList GetScheduleForDay(DateTime day);

        /// <summary>
        /// A method that gets a list of schedule items for a particular day and owner.
        /// </summary>
        /// <param name="day">The requested date.</param>
        /// <param name="owner">The requested owner.</param>
        /// <returns>IScheduleAppointmentList object that holds the list of schedule items.</returns>
        IScheduleAppointmentList GetScheduleForDay(DateTime day, int owner);

        /// <summary>
        /// A method that initializes the contents of the ILookUpObjectList lists obtained from
        /// these methods: <see cref="GetLocations"/>, <see cref="GetMarkers"/>, <see cref="GetLabels"/>, <see cref="GetReminders"/>, <see cref="GetOwners"/>
        /// </summary>
        void InitLists();

        /// <summary>
        /// A method that returns an instance of a new schedule item.
        /// </summary>
        /// <returns>A new schedule item that can be added to this list. </returns>
        IScheduleAppointment NewScheduleAppointment();

        /// <summary>
        /// Returns an instance of a new schedule items collection.
        /// </summary>
        /// <returns>A new schedule items collection. </returns>
        IScheduleAppointmentList NewScheduleAppointmentList();

        /// <summary>
        /// Returns an instance of a new schedule resource.
        /// </summary>
        /// <returns>A new schedule resource. </returns>
        IScheduleResource NewScheduleResource();

        /// <summary>
        /// Returns an instance of a new schedule resources collection.
        /// </summary>
        /// <returns>A new schedule resources collection. </returns>
        IScheduleResourceList NewScheduleResourceList();

        /// <summary>
        /// A method that removes a schedule item from this list.
        /// </summary>
        /// <param name="item">The schedule item to be removed.</param>
        void RemoveItem(IScheduleAppointment item);

        /// <summary>
        /// Makes changes to appointments.
        /// </summary>
        /// <param name="appModifiedItem">The edited appointment.</param>
        /// <param name="appOriginalItem">The original appointment.</param>
        void SaveModifiedItem(IScheduleAppointment appModifiedItem, IScheduleAppointment appOriginalItem);
    }
    #endregion

    #region IScheduleResource

    /// <summary>
    /// Interface defining individual schedule resources.
    /// </summary>
    public interface IScheduleResource
    {
        /// <summary>
        /// A property that gets/sets name of the resource.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// A property that gets/sets a unique integer associated with this resource.
        /// </summary>
        int UniqueID
        {
            get;
            set;
        }
    }

    #endregion

    #region IScheduleResourceCollection

    /// <summary>
    /// A collection of schedule resources.
    /// </summary>
    public interface IScheduleResourceList :
        IList
    {
        /// <summary>
        /// Indexer that returns a <see cref="IScheduleResource"/>.
        /// </summary>
        /// <param name="i"> Get the ScheduleResource based on index </param>
        new IScheduleResource this[int i]
        {
            get;
        }

        /// <summary>
        /// A method that appends the IScheduleResource to the end of the IScheduleResourceCollection.
        /// </summary>
        /// <param name="resource">The IScheduleResource to add to the IScheduleResourceCollection.</param>
        /// <returns>Index at which the item is added.</returns>
        int Add(IScheduleResource resource);

        /// <summary>
        /// Determines whether the collection contains the specified resource.
        /// </summary>
        /// <param name="resource">A IScheduleResource to search for in the collection.</param>
        /// <returns>true if the collection contains the specified item; otherwise, false. </returns>
        bool Contains(IScheduleResource resource);

        /// <summary>
        /// A method that finds resource by UniqueID.
        /// </summary>
        /// <param name="nOwnerID">resource UniqueID</param>
        /// <returns>A IScheduleResource</returns>
        IScheduleResource Find(int nOwnerID);

        /// <summary>
        /// A method that determines the index value that represents the position of the IScheduleResource in the ScheduleResourceCollection.
        /// </summary>
        /// <param name="resource">The IScheduleResource to locate in the IScheduleResourceCollection.</param>
        /// <returns>The index position of the specified IScheduleResource in the collection.</returns>
        int IndexOf(IScheduleResource resource);

        /// <summary>
        /// A method that inserts a IScheduleResource in the IScheduleResourceCollection at the specified index location.
        /// </summary>
        /// <param name="nIdx">The location in the collection to insert the ScheduleResource.</param>
        /// <param name="resource">A IScheduleResource to add to the collection.</param>
        void Insert(int nIdx, IScheduleResource resource);

        /// <summary>
        /// Returns an instance of a new schedule resource.
        /// </summary>
        /// <returns>A new schedule resource that can be added to this list. </returns>
        IScheduleResource NewScheduleResource();

        /// <summary>
        /// A method that removes the specified ScheduleResource from the ScheduleResourceCollection.
        /// </summary>
        /// <param name="resource">The IScheduleResource to remove from the collection.</param>
        void Remove(IScheduleResource resource);
    }

    #endregion //IScheduleResourceCollection

    #region event code

    /// <summary>
    /// Declare a delegate type for handling an event with <see cref="ScheduleAppointmentCancelEventArgs"/> arguments
    /// which is raised when any <see cref="IScheduleAppointment"/> is about to change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void ScheduleAppointmentChangingEventHandler(object sender, ScheduleAppointmentCancelEventArgs e);

    /// <summary>
    /// Represents a method that handles an event with <see cref="ScheduleAppointmentEventArgs"/> arguments
    /// which is raised when any <see cref="IScheduleAppointment"/> has changed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void ScheduleAppointmentChangedEventHandler(object sender, ScheduleAppointmentEventArgs e);

    /// <summary>
    /// A class that maintain event arguments for the ItemChanging event.
    /// </summary>
    public class ScheduleAppointmentCancelEventArgs : CancelEventArgs
    {
        private ItemAction action;
        private ItemDragHitContext context;
        private IScheduleAppointment currentItem;
        private IScheduleAppointment proposedItem;

        /// <summary>
        /// Initializes a new instance of the ScheduleAppointmentCancelEventArgs class.
        /// The event arguments for a cancellable event raised prior to changes in an IScheduleAppointment object.
        /// </summary>
        /// <param name="currentItem">Current value of the IScheduleAppointment object.</param>
        /// <param name="proposedItem">Modified value of the IScheduleAppointment object.</param>
        /// <param name="action">An <see cref="ItemAction"/> value that indicates the type of the change.</param>
        public ScheduleAppointmentCancelEventArgs(IScheduleAppointment currentItem, IScheduleAppointment proposedItem, ItemAction action)
            : base()
        {
            this.currentItem = currentItem;
            this.proposedItem = proposedItem;
            this.action = action;
        }

        /// <summary>
        /// Initializes a new instance of the ScheduleAppointmentCancelEventArgs class.
        /// The event arguments for a cancellable event raised prior to changes in an IScheduleAppointment object.
        /// </summary>
        /// <param name="currentItem">Current value of the IScheduleAppointment object.</param>
        /// <param name="proposedItem">Modified value of the IScheduleAppointment object.</param>
        /// <param name="action">An <see cref="ItemAction"/> value that indicates the type of the change.</param>
        /// <param name="context">An <see cref="ItemDragHitContext"/> value that indicates the drag hit context.</param>
        public ScheduleAppointmentCancelEventArgs(IScheduleAppointment currentItem, IScheduleAppointment proposedItem, ItemAction action, ItemDragHitContext context)
            : base()
        {
            this.currentItem = currentItem;
            this.proposedItem = proposedItem;
            this.action = action;
            this.context = context;
        }

        /// <summary>
        /// A property that gets/sets type of action that is being taken such as a Drag or an Edit.
        /// </summary>
        public ItemAction Action
        {
            get { return action; }
            set { action = value; }
        }

        /// <summary>
        /// A property that gets/sets drag hit context in a Month or Week View
        /// </summary>
        public ItemDragHitContext ItemDragHitContext
        {
            get { return context; }
            set { context = value; }
        }

        /// <summary>
        /// Gets or sets IScheduleAppointment prior to the change.
        /// </summary>
        public IScheduleAppointment CurrentItem
        {
            get { return currentItem; }
            set { currentItem = value; }
        }

        /// <summary>
        /// Gets or sets IScheduleAppointment after the change.
        /// </summary>
        public IScheduleAppointment ProposedItem
        {
            get { return proposedItem; }
            set { proposedItem = value; }
        }

        /// <summary>An Overridden method that returns the string representation of ScheduleAppointmentCancelEventArgs object.</summary>
        /// <returns>A string holding the event args object.</returns>
        /// <override/>
        public override string ToString()
        {
            return string.Format("[current={0}] [proposed={1}] action={2} cancel={3}", CurrentItem, ProposedItem, Action, Cancel);
        }
    }
    
    /// <summary>
    /// A class that maintain event arguments for the ItemChanged event.
    /// </summary>
    public class ScheduleAppointmentEventArgs : EventArgs
    {
        private ItemAction action;
        private IScheduleAppointment currentItem;

        /// <summary>
        /// Initializes a new instance of the ScheduleAppointmentEventArgs class for a notification event raised after changes to an IScheduleAppointment object.
        /// </summary>
        /// <param name="currentItem">Current value of the IScheduleAppointment object.</param>
        /// <param name="action">An <see cref="ItemAction"/> value that indicates the type of the change.</param>
        public ScheduleAppointmentEventArgs(IScheduleAppointment currentItem, ItemAction action)
            : base()
        {
            this.currentItem = currentItem;
            this.action = action;
        }

        /// <summary>
        /// A property that gets/sets type of action that is being taken such as a Drag or an Edit.
        /// </summary>
        public ItemAction Action
        {
            get { return action; }
            set { action = value; }
        }

        /// <summary>
        /// Gets or sets IScheduleAppointment after the change.
        /// </summary>
        public IScheduleAppointment CurrentItem
        {
            get { return currentItem; }
            set { currentItem = value; }
        }

        /// <summary>An Overridden method that returns the string representation of ScheduleAppointmentEventArgs object.</summary>
        /// <returns>A string holding the Event Args object.</returns>
        /// <override/>
        public override string ToString()
        {
            return string.Format("[current={0}] action={1}", CurrentItem, Action);
        }
    }
    #endregion

    #region enumerations

    /// <summary>
    /// Specifies how the fixed panel holding the navigation calendars is displayed.
    /// </summary>
    public enum CalendarNavigationPanelPosition
    {
        /// <summary>
        /// No navigation calendars are visible.
        /// </summary>
        Hidden,
        
        /// <summary>
        /// Navigation calendars are positioned to the left of Schedule display.
        /// </summary>
        Left,
        
        /// <summary>
        /// Navigation calendars are positioned to the right of Schedule display.
        /// </summary>
        Right
    }

    /// <summary>
    /// Specifies the type of change an IScheduleAppointment is undergoing due to a user action.
    /// </summary>
    public enum ItemAction
    {
        /// <summary>
        /// A new IScheduleAppointment is being added.
        /// </summary>
        Add,
        
        /// <summary>
        /// A default IScheduleAppointment is being requested.
        /// </summary>
        Default,
        
        /// <summary>
        /// An IScheduleAppointment is being deleted.
        /// </summary>
        Delete,
        
        /// <summary>
        /// An existing IScheduleAppointment is being edited.
        /// </summary>
        Edit,
        
        /// <summary>
        /// A IScheduleAppointment is being moved with a Mouse Drag.
        /// </summary>
        ItemDrag,
        
        /// <summary>
        /// The bottom or top border of a IScheduleAppointment is being dragged.
        /// </summary>
        TimeDrag
    }

    /// <summary>
    /// Used to specify the type of information under a Point.
    /// </summary>
    public enum ItemHitType
    {
        /// <summary>
        /// There is a AllDay IScheduleAppointment item under the Point.
        /// </summary>
        AllDayItem,
        
        /// <summary>
        /// The Point is in a header line within a cell in a Week or Month Schedule view.
        /// </summary>
        CellHeader,
        
        /// <summary>
        /// The Point is in a column header of a Day or WorkWeek Schedule view.
        /// </summary>
        Header,
        
        /// <summary>
        /// An IScheduleAppointment item under the Point.
        /// </summary>
        Item,
        
        /// <summary>
        /// The Point is in the rectangle occupied by the MoreItemsBitmap.
        /// </summary>
        MoreItemsBitmap,
        
        /// <summary>
        /// There is no IScheduleAppointment item under the Point.
        /// </summary>
        None
    }

    /// <summary>
    /// Defines the possible edit actions on recurring appointments.
    /// </summary>
    public enum RecurringAppointmentEditAction
    {
        /// <summary>
        /// Cancel the edit action.
        /// </summary>
        Cancel,
        
        /// <summary>
        /// Modify all appointments in this recurrence definition.
        /// </summary>
        ChangeAllAppointments,
        
        /// <summary>
        /// Modify this and all future appointments.
        /// </summary>
        ChangeAllFutureAppointments,
        
        /// <summary>
        /// Modify a single appointment.
        /// </summary>
        ChangeSingleAppointmentOnly
    }

    /// <summary>
    /// Specifies the SaveOnClose behavior on how IScheduleDataProvider.Commit will be called when the toplevel Form holding the ScheduleControl is closed.
    /// </summary>
    public enum SaveOnCloseBehavior
    {
        /// <summary>
        /// Do not call Commit.
        /// </summary>
        DoNotSave,
        
        /// <summary>
        /// Propmt before calling Commit.
        /// </summary>
        PromptBeforeSave,
        
        /// <summary>
        /// Call Commit without a prompt.
        /// </summary>
        SaveWithoutPrompt
    }

    /// <summary>
    /// Specifies the details of a <see cref="ScheduleAppointment"/> that get displayed in a ToolTip.
    /// </summary>
    public enum ScheduleAppointmentToolTip
    {
        /// <summary>
        /// Retreives all the details of any appointment
        /// </summary>
        All,
        
        /// <summary>
        /// When the <see cref="ScheduleAppointment.ToolTip"/>is set to custom you can define your own custom tooltip
        /// <see cref="ScheduleAppointment.CustomToolTip"/>
        /// </summary>
        Custom,
        
        /// <summary>
        /// Retreives the <see cref="ScheduleAppointment.EndTime"/> of a appointment
        /// </summary>
        EndTime,
        
        /// <summary>
        /// Retreives the <see cref="ScheduleAppointment.LocationValue"/> of a appointment
        /// </summary>
        Location,
        
        /// <summary>
        /// Retreives the ResourceName of Appointment"/> of a appointment
        /// </summary>
        ResourceName,
        
        /// <summary>
        /// Retreives the <see cref="ScheduleAppointment.EndTime"/> and <see cref="ScheduleAppointment.StartTime"/> of a appointment
        /// </summary>
        StartAndEndTime,
        
        /// <summary>
        ///  Retreives the <see cref="ScheduleAppointment.StartTime"/> of a appointment
        /// </summary>
        StartTime,
        
        /// <summary>
        /// Retreives the <see cref="ScheduleAppointment.Subject"/> of a appointment
        /// </summary>
        Subject
    }

    /// <summary>
    /// Specifies where the mouse is during a appointment drag in a Week or Month view.
    /// </summary>
    public enum ItemDragHitContext
    {
        /// <summary>
        /// Mouse is over the schedule control.
        /// </summary>
        Schedule,
       
        /// <summary>
        /// Mouse is over the calendar.
        /// </summary>
        Calendar
    }
    #endregion
}
