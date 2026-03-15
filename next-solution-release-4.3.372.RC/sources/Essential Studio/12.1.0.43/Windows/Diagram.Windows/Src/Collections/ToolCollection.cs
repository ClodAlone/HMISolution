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

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A collection of <see cref="Syncfusion.Windows.Forms.Diagram.Tool"/> objects.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    [Serializable]
    public class ToolCollection
        : CollectionEx
    {
        #region Class nested classes
        private sealed class ToolEnumerator
            : IEnumerator
        {
            #region Class members
            private ToolCollection m_collection;
            private int m_nIndex;
            #endregion

            #region Class initilize/finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="ToolEnumerator"/> class.
            /// </summary>
            /// <param name="tools">The tools.</param>
            internal ToolEnumerator(ToolCollection tools)
            {
                if (tools == null)
                    throw new ArgumentNullException("tools");

                m_collection = tools;
                m_nIndex = -1;
            }
            #endregion

            #region IEnumerator
            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>The current element in the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception>
            public object Current
            {
                get { return m_collection[m_nIndex]; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                return ++m_nIndex < m_collection.Count;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                m_nIndex = -1;
            }
            #endregion
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolCollection"/> class.
        /// </summary>
        public ToolCollection()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolCollection"/> class.
        /// </summary>
        /// <param name="src">The source instance.</param>
        public ToolCollection(ToolCollection src)
            : base(src)
        { 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.Tool"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the tool.</param>
        /// <value>The tool.</value>
        public Tool this[int index]
        {
            get { return this.Members[index] as Tool; }
            set { Set(index, value); }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Adds the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>The tool index.</returns>
        public int Add(Tool tool)
        {
            return Add(tool);
        }

        /// <summary>
        /// Gets the specified tool index in current collection.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>The index.</returns>
        public int IndexOf(Tool tool)
        {
            return this.Members.IndexOf(tool);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="tool">The tool.</param>
        public void Insert(int index, Tool tool)
        {
            Insert(index, tool);
        }

        /// <summary>
        /// Removes the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        public void Remove(Tool tool)
        {
            Remove(tool);
        }

        /// <summary>
        /// Determines whether collection contains the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>
        /// <c>true</c> if collection contains the specified tool; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Tool tool)
        {
            return this.Members.Contains(tool);
        }

        /// <summary>
        /// Copies all collection member to array.
        /// </summary>
        /// <param name="tools">The tools.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Tool[] tools, int index)
        {
            this.Members.CopyTo(tools, index);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The tool enumerator.</returns>
        public override IEnumerator GetEnumerator()
        {
            return new ToolEnumerator(this);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new ToolCollection(this);
        }

        /// <summary>
        /// Validates given value.
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(object value)
        {
            if (!(value is Tool))
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
                if (!(enumerator.Current is Tool))
                    throw new InvalidCastException("value");
            }
        }
        #endregion
    }
}
