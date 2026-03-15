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
    /// Collection of ConnectionPoint objects.
    /// </summary>
    [Serializable()]
    [Description("Collection of ports in a port collection")]
    [DefaultProperty("Item")]
    public class ConnectionPointCollection
        : CollectionEx
    {
        #region Class nested classes
        private sealed class ConnectionPointEnumerator
            : IEnumerator
        {
            #region Class members
            private ConnectionPointCollection m_collection;
            private int m_nIndex;
            private int m_nCollectionMembers;
            #endregion

            #region Class initilize/finalize methods
            internal ConnectionPointEnumerator(ConnectionPointCollection ports)
            {
                if (ports == null)
                    throw new ArgumentNullException("ports");

                m_collection = ports;
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
        /// Initializes a new instance of the <see cref="ConnectionPointCollection"/> class.
        /// </summary>
        public ConnectionPointCollection()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPointCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public ConnectionPointCollection(Node owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPointCollection"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public ConnectionPointCollection(ConnectionPointCollection src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPointCollection"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public ConnectionPointCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        public Node Container
        {
            get { return this.Owner as Node; }
            set { this.Owner = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.ConnectionPoint"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <value>The connection point.</value>
        public ConnectionPoint this[int index]
        {
            get { return this.Members[index] as ConnectionPoint; }
            set { Set(index, value); }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Adds the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>The value.</returns>
        public int Add(ConnectionPoint port)
        {
            return AddValue(port);
        }

        /// <summary>
        /// Port index in current collection.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>The index.</returns>
        public int IndexOf(ConnectionPoint port)
        {
            return this.Members.IndexOf(port);
        }

        /// <summary>
        /// Inserts port to the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="port">The port.</param>
        public void Insert(int index, ConnectionPoint port)
        {
            InsertValue(index, port);
        }

        /// <summary>
        /// Removes the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        public void Remove(ConnectionPoint port)
        {
            RemoveValue(port);
        }

        /// <summary>
        /// Determines whether collection contains the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>
        /// <c>true</c> if collection contains the specified port; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ConnectionPoint port)
        {
            return this.Members.Contains(port);
        }

        /// <summary>
        /// Copy collection members to array.
        /// </summary>
        /// <param name="ports">The ports.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(ConnectionPoint[] ports, int index)
        {
            this.Members.CopyTo(ports, index);
        }

        /// <summary>
        /// Finds the connection point by ID.
        /// </summary>
        /// <param name="nID">The ID.</param>
        /// <returns>First end point with given ID.</returns>
        public ConnectionPoint FindConnectionPointByID(int nID)
        {
            ConnectionPoint portToReturn = null;

            // iterate through layers' collection
            // looking for layer with the name specified
            foreach (ConnectionPoint port in this.Members)
            {
                if (port.ID == nID)
                {
                    portToReturn = port;
                    break;
                }
            }

            return portToReturn;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public override IEnumerator GetEnumerator()
        {
            return new ConnectionPointEnumerator(this);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new ConnectionPointCollection(this);
        }

        /// <summary>
        /// Validates given value.
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(object value)
        {
            if (!(value is ConnectionPoint))
                throw new InvalidCastException("value");
        }

        /// <summary>
        /// Validates given values.
        /// </summary>
        /// <param name="values">The values to validate.</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(ICollection values)
        {
            // Get collection enumerator
            IEnumerator enumerator = values.GetEnumerator();
            
            // Iterate through collection members checking their types
            while (enumerator.MoveNext())
            {
                if (!(enumerator.Current is ConnectionPoint))
                    throw new InvalidCastException("value");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ChangesComplete"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected override void OnChangesComplete(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.ChangeType == CollectionExChangeType.Insert || evtArgs.ChangeType == CollectionExChangeType.Set || evtArgs.ChangeType == CollectionExChangeType.Remove)
            {
                ConnectionPoint port;

                // iterate through adding ports and 
                for (int n = 0, nLength = this.Members.Count; nLength > n; n++)
                {
                    port = (ConnectionPoint)this.Members[n];
                    port.ID = n;
                    port.Container = this.Container;
                }

                port = evtArgs.Element as ConnectionPoint;
                if (port != null)
                    if (port.Container.GraphicsPath != null)
                        port.Container.UpdateContainerBounds();
            }

            base.OnChangesComplete(evtArgs);
        }

        /// <summary>
        /// Raises ChangesComplete event.
        /// </summary>
        /// <param name="evtArgs">event args</param>
        protected override void RaiseChangesCompleteEvent(CollectionExEventArgs evtArgs)
        {
            if (!this.QuietMode && this.EventSink != null)
            {
                ((DocumentEventSink)this.EventSink).RaisePortsChangedEvent(evtArgs);
            }
        }
        #endregion
    }
}