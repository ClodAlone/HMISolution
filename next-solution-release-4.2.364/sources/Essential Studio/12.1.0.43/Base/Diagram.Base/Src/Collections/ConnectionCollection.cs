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
    /// Collection of end points.
    /// </summary>
    [Serializable()]
    [Description("Set of connections in a port container.")]
    [DefaultProperty("Item")]
    public class EndPointCollection
        : CollectionEx
    {
        #region Class nested classes
        private sealed class EndPointEnumerator
            : IEnumerator
        {
            #region Class members
            private EndPointCollection m_collection;
            private int m_nIndex;
            private int m_nCollectionMembers;
            #endregion

            #region Class initilize/finalize methods
            internal EndPointEnumerator(EndPointCollection connections)
            {
                if (connections == null)
                    throw new ArgumentNullException("connections");

                m_collection = connections;
                m_nIndex = -1;
                m_nCollectionMembers = m_collection.Count;
            }
            #endregion

            #region IEnumerator
            public object Current
            {
                get { return m_collection[m_nIndex]; }
            }
            public bool MoveNext()
            {
                if (m_collection.Count != m_nCollectionMembers)
                    throw new InvalidOperationException("collection was modified");

                return ++m_nIndex < m_collection.Count;
            }
            public void Reset()
            {
                m_nIndex = -1;
            }
            #endregion
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="EndPointCollection"/> class.
        /// </summary>
        public EndPointCollection()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndPointCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public EndPointCollection(object owner)
            : base()
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndPointCollection"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public EndPointCollection(EndPointCollection src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndPointCollection"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public EndPointCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.EndPoint"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <value>The end point.</value>
        public EndPoint this[int index]
        {
            get { return this.Members[index] as EndPoint; }
            set { Set(index, value); }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Adds the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>The value.</returns>
        public int Add(EndPoint endPoint)
        {
            return AddValue(endPoint);
        }

        /// <summary>
        /// Returns the index of end point in current collection.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>The index</returns>
        public int IndexOf(EndPoint endPoint)
        {
            return this.Members.IndexOf(endPoint);
        }

        /// <summary>
        /// Insert end point to the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="endPoint">The end point.</param>
        public void Insert(int index, EndPoint endPoint)
        {
            InsertValue(index, endPoint);
        }

        /// <summary>
        /// Removes the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        public void Remove(EndPoint endPoint)
        {
            RemoveValue(endPoint);
        }

        /// <summary>
        /// Determines whether collection contains the specified end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified end point]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(EndPoint endPoint)
        {
            return this.Members.Contains(endPoint);
        }

        /// <summary>
        /// Copies members to array.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(EndPoint[] endPoint, int index)
        {
            this.Members.CopyTo(endPoint, index);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates enumerator.
        /// </summary>
        /// <returns>Created enumerator</returns>
        public override IEnumerator GetEnumerator()
        {
            return new EndPointEnumerator(this);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new EndPointCollection(this);
        }

        /// <summary>
        /// Validates given value.
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(object value)
        {
            if (!(value is EndPoint))
                throw new InvalidCastException("value");
        }

        /// <summary>
        /// Validates given values.
        /// </summary>
        /// <param name="values">values to validate</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(ICollection values)
        {
            // Get collection enumerator
            IEnumerator enumerator = values.GetEnumerator();
            
            // Iterate through collection members checking their types
            while (enumerator.MoveNext())
            {
                if (!(enumerator.Current is EndPoint))
                    throw new InvalidCastException("value");
            }
        }

        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected override void UpdateServiceReferences(CollectionExEventArgs evtArgs)
        {
            EndPoint endPoint;
            IEnumerator enumerator = evtArgs.Elements.GetEnumerator();

            while (enumerator.MoveNext())
            {
                endPoint = enumerator.Current as EndPoint;

                if (endPoint != null)
                {
                    if (evtArgs.ChangeType == CollectionExChangeType.Insert
                        || evtArgs.ChangeType == CollectionExChangeType.Set)
                    {
                        endPoint.Port = (ConnectionPoint)this.Owner;
                    }
                }
            }
        }

        /// <summary>
        /// Raise ChangesComplete event.
        /// </summary>
        /// <param name="evtArgs">event args</param>
        protected override void RaiseChangesCompleteEvent(CollectionExEventArgs evtArgs)
        {
            if (!this.QuietMode && this.EventSink != null)
            {
                ((DocumentEventSink)this.EventSink).RaiseConnectionsChangedEvent(evtArgs);
            }
        }

        /// <summary>
        /// Raise Changing event.
        /// </summary>
        /// <param name="evtArgs">event args</param>
        protected override void RaiseChangingEvent(CollectionExEventArgs evtArgs)
        {
            if (!this.QuietMode && this.EventSink != null)
            {
                ((DocumentEventSink)this.EventSink).RaiseConnectionsChangingEvent(evtArgs);
            }
        }
        #endregion
    }
}