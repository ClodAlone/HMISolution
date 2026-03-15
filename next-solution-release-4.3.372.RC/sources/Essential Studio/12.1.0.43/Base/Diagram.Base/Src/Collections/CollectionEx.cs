#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for list collections that fire events before and after changes
    /// occur.
    /// </summary>
    /// <remarks>
    /// This class provides a base implementation of the IList and IEnumerable
    /// interfaces. The internal implementation of the collection is an ArrayList.
    /// </remarks>
    [Serializable]
    public abstract class CollectionEx
        : IList,
          ICloneable,
          ISerializable,
          IServiceReferenceHolder,
          IServiceReferenceProvider
    {
        #region Class members
        private IServiceReferenceProvider m_provider;
        private HistoryManager m_mgrHistory;
        private EventSink m_eventSink;
        private LinkManager m_mgrLink;
        private BridgeManager m_mgrBridge;

        /// <summary>
        /// Typed members storage.
        /// </summary>
        private ArrayList m_members;

        /// <summary>
        /// Collection owner.
        /// </summary>
        private object m_owner;

        /// <summary>
        /// Indicates whether collection events are raized.
        /// </summary>
        private bool m_bQuietMode;

        /// <summary>
        /// Indicates whether collection will update its members service refences.
        /// </summary>
        /// <remarks>
        /// Used for helper collections like Selection containing or Active layers collection.
        /// </remarks>
        private bool m_bUpdateReferences;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEx"/> class.
        /// </summary>
        public CollectionEx()
        {
            m_bUpdateReferences = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEx"/> class.
        /// </summary>
        /// <param name="owner">The collection owner</param>
        public CollectionEx(object owner)
        {
            m_owner = owner;
            m_bUpdateReferences = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEx"/> class.
        /// </summary>
        /// <param name="src">The collection.</param>
        public CollectionEx(CollectionEx src)
        {
            m_members = new ArrayList();

            foreach (object curObj in src)
            {
                ICloneable cloneableObj = curObj as ICloneable;

                if (cloneableObj != null)
                {
                    m_members.Add(cloneableObj.Clone());
                }
                else
                {
                    m_members.Add(curObj);
                }
            }
            m_owner = src.m_owner;
            m_bQuietMode = src.m_bQuietMode;
            m_bUpdateReferences = src.m_bUpdateReferences;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEx"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        public CollectionEx(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "items":
                        m_members = info.GetValue("items", typeof(ArrayList)) as ArrayList;
                        break;
                    case "quietmode":
                        m_bQuietMode = info.GetBoolean("quietmode");
                        break;
                    case "updateReferences":
                        m_bUpdateReferences = info.GetBoolean("updateReferences");
                        break;
                    case "owner":
                        m_owner = info.GetValue("owner", typeof(object));
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        #region public
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return this.Members.Count == 0; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quiet mode for events raising.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool QuietMode
        {
            get { return m_bQuietMode; }
            set { m_bQuietMode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether members service references will be updated.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool UpdateReferences
        {
            get { return m_bUpdateReferences; }
            set { m_bUpdateReferences = value; }
        }
        #endregion

        #region protected
        /// <summary>
        /// Gets or sets the collection owner.
        /// </summary>
        /// <value>The owner.</value>
        public object Owner
        {
            get 
            { 
                return m_owner; 
            }
            set
            {
                m_owner = value;
            }
        }

        /// <summary>
        /// Gets the collection members.
        /// </summary>
        /// <value>The members.</value>
        public ArrayList Members
        {
            get
            {
                if (m_members == null)
                    m_members = new ArrayList();

                return m_members;
            }

            set
            {
                m_members = value;
            }
        }

        /// <summary>
        /// Gets the reference event sink.
        /// </summary>
        /// <value>The event sink.</value>
        protected EventSink EventSink
        {
            get { return m_eventSink; }
        }
        #endregion

        #endregion

        #region ISerializable Members
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("items", m_members);
            info.AddValue("quietmode", m_bQuietMode);
            info.AddValue("updateReferences", m_bUpdateReferences);
            info.AddValue("owner", m_owner);
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public abstract object Clone();
        #endregion

        #region IList Members
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to add to the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        int IList.Add(object value)
        {
            return AddValue(value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception>
        void IList.Clear()
        {
            Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
        /// </returns>
        bool IList.Contains(object value)
        {
            return this.Members.Contains(value);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object value)
        {
            return this.Members.IndexOf(value);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to insert into the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// <paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.</exception>
        void IList.Insert(int index, object value)
        {
            InsertValue(index, value);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.</returns>
        bool IList.IsFixedSize
        {
            get { return this.Members.IsFixedSize; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> is read-only; otherwise, false.</returns>
        bool IList.IsReadOnly
        {
            get { return this.Members.IsReadOnly; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to remove from the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        void IList.Remove(object value)
        {
            RemoveValue(value);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.IList"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <value>The object in list.</value>
        object IList.this[int index]
        {
            get { return this.Members[index]; }
            set { Set(index, value); }
        }
        #endregion

        #region ICollection Members
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array"/> is multidimensional.-or- <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. </exception>
        /// <exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. </exception>
        public void CopyTo(Array array, int index)
        {
            this.Members.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.</returns>
        public int Count
        {
            get { return this.Members.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get { return this.Members.IsSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.</returns>
        public object SyncRoot
        {
            get { return this.Members.SyncRoot; }
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        #region IServiceReferenceHolder Members
        /// <summary>
        /// Updates the service references using service provider.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            if (provider == null)
            {
                m_mgrHistory = null;
                m_eventSink = null;
                m_mgrBridge = null;
                m_mgrLink = null;
            }
            else
            {
                m_mgrHistory = (HistoryManager)provider.ProvideServiceReference(typeof(HistoryManager).TypeHandle);
                m_eventSink = (EventSink)provider.ProvideServiceReference(typeof(EventSink).TypeHandle);
                m_mgrBridge = (BridgeManager)provider.ProvideServiceReference(typeof(BridgeManager).TypeHandle);
                m_mgrLink = (LinkManager)provider.ProvideServiceReference(typeof(LinkManager).TypeHandle);
            }

            m_provider = provider;
        }
        #endregion

        #region IServiceReferenceProvider Members
        /// <summary>
        /// Provides the service reference for object that use that instance.
        /// </summary>
        /// <param name="typeHandle">The type handle.</param>
        /// <returns>The object.</returns>
        public object ProvideServiceReference(RuntimeTypeHandle typeHandle)
        {
            object objToReturn = null;

            if (m_provider != null)
            {
                objToReturn = m_provider.ProvideServiceReference(typeHandle);
            }

            return objToReturn;
        }
        #endregion

        #region Class Utility methods
        /// <summary>
        /// Raises viewer event.
        /// </summary>
        /// <param name="selectedEvent">true if NodeSelected event should be raised; otherwise, false.</param>
        /// <param name="value">value.</param>
        private void RaiseViewerEvent(object value, bool selectedEvent)
        {
            if (this.EventSink != null)
                if (this.EventSink is ViewerEventSink)
                    if (selectedEvent)
                        // Raise NodeSelected event
                        ((ViewerEventSink)this.EventSink).RaiseNodeSelected(new NodeSelectedEventArgs(value as Node));
                    else
                        // Raise NodeDeselected event
                        ((ViewerEventSink)this.EventSink).RaiseNodeDeselected(new NodeSelectedEventArgs(value as Node));
        }

        /// <summary>
        /// Sort list using the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer to sort items.</param>
        public void Sort(IComparer comparer)
        {
            this.Members.Sort(comparer);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public abstract IEnumerator GetEnumerator();

        /// <summary>
        /// Validates given value.
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <exception cref="System.InvalidCastException"/>
        protected abstract void OnValidate(object value);

        /// <summary>
        /// Validates given values.
        /// </summary>
        /// <param name="values">The values to validate.</param>
        /// <exception cref="System.InvalidCastException"/>
        protected abstract void OnValidate(ICollection values);

        /// <summary>
        /// Raises the <see cref="E:Changing"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected virtual void OnChanging(CollectionExEventArgs evtArgs)
        {
            RaiseChangingEvent(evtArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:ChangesComplete"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected virtual void OnChangesComplete(CollectionExEventArgs evtArgs)
        {
            if (m_bUpdateReferences && m_provider != null)
            {
                UpdateServiceReferences(evtArgs);
            }

            RaiseChangesCompleteEvent(evtArgs);
        }
        #endregion

        #region Class Helper methods
        /// <summary>
        /// Adds an item to the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to add to the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.</param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        protected int AddValue(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            // Validate given value
            OnValidate(value);

            int nIndexToReturn = -1;
            
            // Provide user ability to cancel pending changes if not QuietMode
            CollectionExChangeType changeType = CollectionExChangeType.Insert;
            CollectionExEventArgs evtArgs = null;

            if (!this.QuietMode)
            {
                evtArgs = new CollectionExEventArgs(this.Owner, changeType, value, -1);
                OnChanging(evtArgs);
            }

            // Proceed depending on evtArgs.Cancel value
            if ((evtArgs == null) || (!evtArgs.Cancel))
            {
                // Add value
                if (!this.Members.Contains(value))
                {
                    nIndexToReturn = this.Members.Add(value);
                }
                if (!this.QuietMode)
                {
                    // ! Recreate event args -- including inserted object index
                    evtArgs = new CollectionExEventArgs(this.Owner, changeType, value, nIndexToReturn);
                    
                    // Change complete
                    OnChangesComplete(evtArgs);
                    
                    if (evtArgs.Cancel)
                    {
                        if (!this.QuietMode && this.EventSink != null)
                        {
                            this.EventSink.RaiseCancelCollectionChangedEvent(evtArgs);
                        }
                        this.Members.Remove(value);
                        nIndexToReturn = -1;
                    }
                    else
                    {
                        RaiseViewerEvent(value, true);
                        // record collection modification
                        RecordCollectionChanged(changeType, this, value, -1);
                    }
                }
            }

            return nIndexToReturn;
        }

        /// <summary>
        /// Adds an items to the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.
        /// </summary>
        /// <param name="values">The <see cref="System.Collections.ICollection"/> to add to the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.</param>
        public void AddRange(ICollection values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            // Validate given value
            OnValidate(values);
            
            // Provide user ability to cancel pending changes
            CollectionExChangeType changeType = CollectionExChangeType.Insert;
            CollectionExEventArgs evtArgs = new CollectionExEventArgs(this.Owner, changeType, values, -1);
            OnChanging(evtArgs);

            // Proceed depending on evtArgs.Cancel value
            if (!evtArgs.Cancel)
            {
                //// Add value
                this.Members.AddRange(values);
                //// Change complete
                OnChangesComplete(evtArgs);

                if (evtArgs.Cancel)
                {
                    if (!this.QuietMode && this.EventSink != null)
                    {
                        this.EventSink.RaiseCancelCollectionChangedEvent(evtArgs);
                    }
                    this.Members.RemoveRange(this.Members.Count - values.Count, values.Count); 
                }
                else
                {
                    // record collection modification
                    RecordCollectionChanged(changeType, this, values, -1);
                    foreach (object value in values)
                        RaiseViewerEvent(value, true);
                }
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.
        /// </summary>
        public void Clear()
        {
            // Provide user ability to cancel pending changes
            CollectionExChangeType changeType = CollectionExChangeType.Clear;
            CollectionExEventArgs evtArgs = new CollectionExEventArgs(this.Owner, CollectionExChangeType.Clear, this.Members.ToArray(), -1);
            OnChanging(evtArgs);

            // Proceed depending on evtArgs.Cancel value
            if (!evtArgs.Cancel)
            {
                //// record collection modification
                RecordCollectionChanged(changeType, this, this.Members.ToArray(), -1);
                foreach (object value in this.Members)
                    RaiseViewerEvent(value, false);
                //// Clear collection
                this.Members.Clear();
                //// Change complete
                OnChangesComplete(evtArgs);
            }
        }

        /// <summary>
        /// Inserts an item to the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index ar which <paramref name="index"/> should be inserted.</param>
        /// <param name="value">The <see cref="System.Object"/> to insert into the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.</param>
        protected void InsertValue(int index, object value)
        {
            if ((0 > index) && (index < (this.Members.Count - 1)))
                throw new ArgumentOutOfRangeException("index");

            // Validate given value
            OnValidate(value);
            
            // Provide user ability to cancel pending changes
            CollectionExChangeType changeType = CollectionExChangeType.Insert;
            CollectionExEventArgs evtArgs = new CollectionExEventArgs(this.Owner, CollectionExChangeType.Insert, value, index);
            OnChanging(evtArgs);

            // Proceed depending on evtArgs.Cancel value
            if (!evtArgs.Cancel)
            {
                //// record collection modification
                RecordCollectionChanged(changeType, this, value, index);
                //// Insert value
                this.Members.Insert(index, value);
                //// Change complete
                OnChangesComplete(evtArgs);
                RaiseViewerEvent(value, true);
            }
        }

        /// <summary>
        /// Assigns new value to member in specified position .
        /// </summary>
        /// <param name="index">The index of element.</param>
        /// <param name="value">The new value to set.</param>
        protected void Set(int index, object value)
        {
            if ((0 > index) && (index > (this.Members.Count - 1)))
                throw new ArgumentOutOfRangeException("index");

            if (value == null)
                throw new ArgumentNullException("value");

            // Validate given value
            OnValidate(value);

            if (!this.Members[index].Equals(value))
            {
                // Provide user ability to cancel pending changes
                CollectionExChangeType changeType = CollectionExChangeType.Set;
                CollectionExEventArgs evtArgs =
                    new CollectionExEventArgs(this.Owner, CollectionExChangeType.Set, value, this.Members.IndexOf(value));
                OnChanging(evtArgs);

                // Proceed depending on evtArgs.Cancel value
                if (!evtArgs.Cancel)
                {
                    // record collection modification
                    RecordCollectionChanged(changeType, this, this.Members[index], index);
                    object oldValue = this.Members[index];
                    // Insert value
                    this.Members[index] = value;
                    RaiseViewerEvent(oldValue, false);
                    // Change complete
                    OnChangesComplete(evtArgs);
                    RaiseViewerEvent(this.Members[index], true);
                }
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to remove from the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.</param>
        /// <returns>true, if remove value otherwise, false.</returns>
        protected bool RemoveValue(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            // Validate given value
            OnValidate(value);

            int nIndex = this.Members.IndexOf(value);
            
            // Provide user ability to cancel pending changes
            CollectionExChangeType changeType = CollectionExChangeType.Remove;
            CollectionExEventArgs evtArgs =
                new CollectionExEventArgs(this.Owner, changeType, value, nIndex);
            OnChanging(evtArgs);

            // Proceed depending on evtArgs.Cancel value
            if (!evtArgs.Cancel)
            {
                //// record collection modification
                RecordCollectionChanged(changeType, this, value, nIndex);
                //// Remove value
                this.Members.Remove(value);
                //// Change complete
                OnChangesComplete(evtArgs);
                if (evtArgs.Cancel)
                {
                    if (!this.QuietMode && this.EventSink != null)
                    {
                        this.EventSink.RaiseCancelCollectionChangedEvent(evtArgs);
                    }
                    this.Members.Insert(nIndex,  value);
                }
                RaiseViewerEvent(value, false);
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Removes the values occurrence of a specific object from the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.
        /// </summary>
        /// <param name="values">The <see cref="System.Collections.ICollection"/> to remove from the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.</param>
        protected void RemoveRange(ICollection values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            // Validate given value
            OnValidate(values);
            
            // Provide user ability to cancel pending changes
            CollectionExEventArgs evtArgs =
                new CollectionExEventArgs(this.Owner, CollectionExChangeType.Remove, values, -1);
            OnChanging(evtArgs);

            // Proceed depending on evtArgs.Cancel value
            if (!evtArgs.Cancel)
            {
                int nIndex;
                
                // Remove value
                IEnumerator enumerator = values.GetEnumerator();

                object value = null;
                while (enumerator.MoveNext())
                {
                    nIndex = this.Members.IndexOf(enumerator.Current);
                    value = this.Members[nIndex];
                    this.Members.RemoveAt(nIndex);
                    RaiseViewerEvent(value, false);
                }
                
                // Change complete
                OnChangesComplete(evtArgs);
                if (evtArgs.Cancel)
                {
                    if (!this.QuietMode && this.EventSink != null)
                    {
                        this.EventSink.RaiseCancelCollectionChangedEvent(evtArgs);
                    }
                    enumerator = values.GetEnumerator();

                    value = null;
                    while (enumerator.MoveNext())
                    {
                        nIndex = this.Members.IndexOf(enumerator.Current);
                        value = this.Members[nIndex];
                        this.Members.Insert(nIndex, value);
                    }
                }

            }
        }

        /// <summary>
        /// Removes the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="Syncfusion.Windows.Forms.Diagram.CollectionEx"/>.</exception>
        public void RemoveAt(int index)
        {
            if ((0 > index) || (index > (this.Members.Count - 1)))
                throw new ArgumentOutOfRangeException("index");

            // Provide user ability to cancel pending changes
            CollectionExChangeType changeType = CollectionExChangeType.Remove;
            CollectionExEventArgs evtArgs =
                new CollectionExEventArgs(this.Owner, changeType, this.Members[index], index);
            OnChanging(evtArgs);

            // Proceed depending on evtArgs.Cancel value
            if (!evtArgs.Cancel)
            {
                //// record collection modification
                RecordCollectionChanged(changeType, this, this.Members[index], index);
                object oldValue = this.Members[index];
                //// Remove value
                this.Members.RemoveAt(index);
                //// Change complete
                OnChangesComplete(evtArgs);
                if (evtArgs.Cancel)
                {
                    if (!this.QuietMode && this.EventSink != null)
                    {
                        this.EventSink.RaiseCancelCollectionChangedEvent(evtArgs);
                    }
                    this.Members.Insert(index, oldValue);
                }
                RaiseViewerEvent(oldValue, false);
            }
        }

        /// <summary>
        /// Updates the service references the collection members.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected virtual void UpdateServiceReferences(CollectionExEventArgs evtArgs)
        {
            IServiceReferenceHolder referenceHolder;
            IEnumerator enumerator = evtArgs.Elements.GetEnumerator();

            while (enumerator.MoveNext())
            {
                referenceHolder = enumerator.Current as IServiceReferenceHolder;

                if (referenceHolder != null)
                {
                    if (evtArgs.ChangeType == CollectionExChangeType.Insert
                        || evtArgs.ChangeType == CollectionExChangeType.Set)
                    {
                        referenceHolder.UpdateServiceReferences(this);
                    }
                }
            }
        }

        /// <summary>
        /// Records the collection changes.
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="collection">The changed collection.</param>
        /// <param name="element">The changed element.</param>
        /// <param name="nIndex">Index of the element in specified collection.</param>
        private void RecordCollectionChanged(CollectionExChangeType changeType, CollectionEx collection, object element, int nIndex)
        {
            ArrayList elements = new ArrayList(1);
            elements.Add(element);

            RecordCollectionChanged(changeType, collection, elements, nIndex);
        }

        /// <summary>
        /// Records the collection changes.
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="collection">The changed collection.</param>
        /// <param name="elements">The changed collection elements.</param>
        /// <param name="nIndex">Index of the first element of the changes elements in specified collection.</param>
        private void RecordCollectionChanged(CollectionExChangeType changeType, CollectionEx collection, ICollection elements, int nIndex)
        {
            if (!m_bQuietMode && m_mgrHistory != null)
                m_mgrHistory.RecordCollectionChanged(changeType, collection, elements, nIndex);
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raise Changing event.
        /// </summary>
        /// <param name="evtArgs">event args</param>
        protected virtual void RaiseChangingEvent(CollectionExEventArgs evtArgs)
        { 
        }

        /// <summary>
        /// Raises ChangesComplete event.
        /// </summary>
        /// <param name="evtArgs">event args</param>
        protected virtual void RaiseChangesCompleteEvent(CollectionExEventArgs evtArgs)
        { 
        }
        #endregion
    }
}