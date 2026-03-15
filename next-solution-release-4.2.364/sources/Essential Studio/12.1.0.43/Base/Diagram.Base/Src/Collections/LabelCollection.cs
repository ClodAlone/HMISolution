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
    /// A collection of <see cref="Syncfusion.Windows.Forms.Diagram.Label"/> objects.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Label"/>
    /// </remarks>
    [Serializable]
    [Description("Collection of labels in a symbol.")]
    [DefaultProperty("Item")]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class LabelCollection
        : CollectionEx
    {
        #region Class nested classes
        private sealed class LabelEnumerator
            : IEnumerator
        {
            #region Class members
            private LabelCollection m_collection;
            private int m_nIndex;
            private int m_nCollectionMembers;
            #endregion

            #region Class initilize/finalize methods
            internal LabelEnumerator(LabelCollection labels)
            {
                if (labels == null)
                    throw new ArgumentNullException("labels");

                m_collection = labels;
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
        /// Initializes a new instance of the <see cref="LabelCollection"/> class.
        /// </summary>
        public LabelCollection()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public LabelCollection(Node owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            this.Owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelCollection"/> class.
        /// </summary>
        /// <param name="src">The label collection.</param>
        public LabelCollection(LabelCollection src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelCollection"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public LabelCollection(SerializationInfo info, StreamingContext context)
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
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.Label"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <value>The label at the specified index.</value>
        public Label this[int index]
        {
            get { return this.Members[index] as Label; }
            set { Set(index, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.Label"/> with the specified STR label name.
        /// </summary>
        /// <param name="strLabelName">The index</param>
        /// <value>The label at the specified index.</value>
        public Label this[string strLabelName]
        {
            get 
            { 
                return FindLabelByName(strLabelName); 
            }
            set
            {
                int nLabelIndex = -1;
                Label labelTemp;
                
                // find label to modify
                for (int nCounter = 0, nLength = 0; nCounter < nLength; nCounter++)
                {
                    labelTemp = this.Members[nCounter] as Label;

                    if (labelTemp.Text == strLabelName)
                    {
                        nLabelIndex = nCounter;
                        break;
                    }
                }

                Set(nLabelIndex, value);
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Adds the specified label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>The value.</returns>
        public int Add(Label label)
        {
            return AddValue(label);
        }

        /// <summary>
        /// Returns the index of the label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>The value.</returns>
        public int IndexOf(Label label)
        {
            return this.Members.IndexOf(label);
        }

        /// <summary>
        /// Inserts label at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="label">The label.</param>
        public void Insert(int index, Label label)
        {
            InsertValue(index, label);
        }

        /// <summary>
        /// Removes the specified label.
        /// </summary>
        /// <param name="label">The label.</param>
        public void Remove(Label label)
        {
            RemoveValue(label);
        }

        /// <summary>
        /// Determines whether the member contains the specified label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>
        /// <c>true</c> if it contains the specified label; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Label label)
        {
            return this.Members.Contains(label);
        }

        /// <summary>
        /// Copies the labels collection to the specified index.
        /// </summary>
        /// <param name="labels">The labels.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Label[] labels, int index)
        {
            this.Members.CopyTo(labels, index);
        }

        /// <summary>
        /// Determines specified label name.
        /// </summary>
        /// <param name="strLabelName">Name of the label.</param>
        /// <returns>
        /// <c>true</c> if the specified STR label name is available; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string strLabelName)
        {
            return FindLabelByName(strLabelName) == null;
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
            return new LabelEnumerator(this);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new LabelCollection(this);
        }

        /// <summary>
        /// Validates given value.
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(object value)
        {
            if (!(value is Label))
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
                if (!(enumerator.Current is Label))
                    throw new InvalidCastException("value");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ChangesComplete"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected override void OnChangesComplete(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.ChangeType == CollectionExChangeType.Insert || evtArgs.ChangeType == CollectionExChangeType.Set)
            {
                foreach (Label label in evtArgs.Elements)
                {
                    label.Container = this.Container;
                }
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
                ((DocumentEventSink)this.EventSink).RaiseLabelsChangedEvent(evtArgs);
            }
        }
        #endregion

        #region Class helper methods
        private Label FindLabelByName(string strLabelName)
        {
            Label labelToReturn = null;

            // iterate through layers' collection
            // looking for layer with the name specified
            foreach (Label label in this.Members)
            {
                if (label.Text == strLabelName)
                {
                    labelToReturn = label;
                    break;
                }
            }

            return labelToReturn;
        }
        #endregion
    }
}