#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// The collection of all objects within a PDF document.
    /// </summary>
    internal class PdfMainObjectCollection
    {
        #region Fields
        /// <summary>
        /// The collection of the indirect objects.
        /// </summary>
        private List<ObjectInfo> m_objectCollection = new List<ObjectInfo>();
        /// <summary>
        /// Holds the index of the object.
        /// </summary>
        private int m_index;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="T:ObjectInfo"/> at the specified index.
        /// </summary>
        internal ObjectInfo this[int index]
        {
            get
            {
                if (index < 0 || index > m_objectCollection.Count)
                    throw new ArgumentOutOfRangeException("index");

                return m_objectCollection[index];
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        internal int Count
        {
            get
            {
                return m_objectCollection.Count;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:PdfMainObjectCollection"/> class.
        /// </summary>
        internal PdfMainObjectCollection()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void Add(IPdfPrimitive element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element is IPdfWrapper)
            {
                element = (element as IPdfWrapper).Element;
            }

            //if (!Contains(element))
            {
                m_objectCollection.Add(new ObjectInfo(element));
                element.Position = m_index = m_objectCollection.Count - 1;
                element.Status = ObjectStatus.Registered;
            }
        }

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="reference">The reference.</param>
        internal void Add(IPdfPrimitive obj, PdfReference reference)
        {
            if (obj == null)
                throw new ArgumentNullException("element");

            if (reference == null)
                throw new ArgumentNullException("reference");

            if (obj is IPdfWrapper)
            {
                obj = (obj as IPdfWrapper).Element;
            }

            //if (!Contains(obj))
            {
                ObjectInfo objInfo = new ObjectInfo(obj, reference);
                m_objectCollection.Add(objInfo);
            }

            obj.Position = reference.Position = m_objectCollection.Count - 1;
        }

        /// <summary>
        /// Removes the entry using index.
        /// </summary>
        internal void Remove(int index)
        {
            m_objectCollection.RemoveAt(index);
        }

        /// <summary>
        /// Determines whether the specified element is within the collection.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// 	<c>true</c> if the specified element is within the collection; otherwise, <c>false</c>.
        /// </returns>
        internal bool Contains(IPdfPrimitive element)
        {
            bool result = (LookFor(element) >= 0);
            return result;
        }

        /// <summary>
        /// Determines whether the collection contains the specified reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>
        /// 	<c>true</c> if there is the specified reference; otherwise, <c>false</c>.
        /// </returns>
        internal bool ContainsReference(PdfReference reference)
        {
            int index = LookForReference(reference);
            return (index >= 0);
        }

        /// <summary>
        /// Gets the reference of the object.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The reference of the object.</returns>
        internal PdfReference GetReference(int index)
        {
            ObjectInfo oi = m_objectCollection[index];

            return oi.Reference;
        }

        /// <summary>
        /// Gets the reference of the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="isNew">if set to <c>true</c> the object hasn't been found in the collection.</param>
        /// <returns>The reference of the object.</returns>
        internal PdfReference GetReference(IPdfPrimitive obj, out bool isNew)
        {
            m_index = LookFor(obj);
            PdfReference reference;

            if (m_index < 0 || m_index > Count)
            {
                isNew = true;
                reference = null;
            }
            else
            {
                isNew = false;
                ObjectInfo oi = m_objectCollection[m_index];
                reference = oi.Reference;
            }

            return reference;
        }

        /// <summary>
        /// Gets the object specified by the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The object.</returns>
        internal IPdfPrimitive GetObject(int index)
        {
            return (m_objectCollection[index]).Object;
        }

        /// <summary>
        /// Gets the index of the object.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The index of the object within the general object collection.</returns>
        internal int GetObjectIndex(PdfReference reference)
        {
            int index = -1;

            if (reference.Position != -1)
                return reference.Position;

            for (int i = m_objectCollection.Count - 1; i >= 0; i--)
            {
                ObjectInfo oi = m_objectCollection[i];

                if (oi.Reference != null && (oi.Reference.ObjNum == reference.ObjNum) && (oi.Reference.GenNum == reference.GenNum))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// Tries to set the reference to the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="found">if set to <c>true</c> the object was found.</param>
        /// <returns>
        /// True if the reference have been set successfully.
        /// </returns>
        internal bool TrySetReference(IPdfPrimitive obj, PdfReference reference, out bool found)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (reference == null)
                throw new ArgumentNullException("reference");

            bool result = true;
            found = true;
            m_index = LookFor(obj);

            if (m_index < 0 || m_index >= m_objectCollection.Count)
            {
                result = false;
                found = false;
            }
            else
            {
                ObjectInfo oi = m_objectCollection[m_index];

                if (oi.Reference != null)
                {
                    result = false;
                }
                else
                {
                    oi.SetReference(reference);
                }
            }
            return result;
        }

        /// <summary>
        /// Determines the index of the element within the collection.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The index of the element.</returns>
        internal int IndexOf(IPdfPrimitive element)
        {
            return LookFor(element);
        }

        /// <summary>
        /// Reregisters the reference.
        /// </summary>
        /// <param name="oldObjIndex">Old index of the obj.</param>
        /// <param name="newObj">The new object.</param>
        internal void ReregisterReference(int oldObjIndex, IPdfPrimitive newObj)
        {
            if (newObj == null)
                throw new ArgumentNullException("newObj");

            if (oldObjIndex < 0 || oldObjIndex > Count)
                throw new ArgumentOutOfRangeException("oldObjectIndex");

            ObjectInfo oi = m_objectCollection[oldObjIndex];
            oi.Object = newObj;
            newObj.Position = oldObjIndex;
        }

        /// <summary>
        /// Reregisters reference from one object to another.
        /// </summary>
        /// <param name="oldObj">The old (primitive) object.</param>
        /// <param name="newObj">The new (complex) object.</param>
        /// <remarks>This method is useful when PDF primitives are converted into
        /// More complex objects.</remarks>
        internal void ReregisterReference(IPdfPrimitive oldObj, IPdfPrimitive newObj)
        {
            if (oldObj == null)
                throw new ArgumentNullException("oldObj");

            if (newObj == null)
                throw new ArgumentNullException("newObj");

            int index = IndexOf(oldObj);

            if (index < 0)
                throw new ArgumentException("Can't reregister an object.", "oldObj");

            ReregisterReference(index, newObj);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Looks through the collection for the object specified.
        /// </summary>
        /// <param name="obj">The object to look for.</param>
        /// <returns>The index of the object.</returns>
        private int LookFor(IPdfPrimitive obj)
        {
            int index = -1;

            if (obj.Position != -1)
                return obj.Position;

            for (int i = Count-1; i >=0; i--)
            {
                ObjectInfo oi = m_objectCollection[i];

                if (oi.Object == obj)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Looks through the collection for the object specified by the reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The index of the objec.</returns>
        private int LookForReference(PdfReference reference)
        {
            int index = -1;

            if (reference.Position != -1)
                return reference.Position;

            for (int i = Count-1; i >= 0; i--)
            {
                ObjectInfo oi = m_objectCollection[i];

                if (oi.Reference == reference)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        #endregion

        #region Internals
        /// <summary>
        /// Stores info about objects in the PDF document.
        /// </summary>
        internal class ObjectInfo
        {
            #region Fields
            /// <summary>
            /// Shows if the object was modified and requires saving.
            /// </summary>
            private bool m_bModified;
            /// <summary>
            /// The PDF object.
            /// </summary>
            private IPdfPrimitive m_object;
            /// <summary>
            /// Object number and generation number of the object.
            /// </summary>
            private PdfReference m_reference;
            #endregion

            #region Properties
            /// <summary>
            /// Gets a value indicating whether the object has been modified.
            /// </summary>
            internal bool Modified
            {
                get
                {
                    bool modified = m_bModified;
                    IPdfChangable chbl = (Object as IPdfChangable);

                    if (chbl != null)
                    {
                        modified |= chbl.Changed;
                    }

                    return modified;
                }
            }
            /// <summary>
            /// Gets the reference.
            /// </summary>
            internal PdfReference Reference
            {
                get
                {
                    return m_reference;
                }
            }
            /// <summary>
            /// Gets the object.
            /// </summary>
            internal IPdfPrimitive Object
            {
                get
                {
                    return m_object;
                }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("Object");

                    m_object = value;
                }
            }
            #endregion

            #region Initialize/Finalize
            /// <summary>
            /// Initializes a new instance of the <see cref="T:ObjectInfo"/> class.
            /// </summary>
            /// <param name="obj">The PDF object.</param>
            internal ObjectInfo(IPdfPrimitive obj)
            {
                if (obj == null)
                    throw new ArgumentNullException("obj");

                m_object = obj;
                // Leave reference uninitialized.
                m_bModified = true;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="T:ObjectInfo"/> class.
            /// </summary>
            /// <param name="obj">The PDF object.</param>
            /// <param name="reference">The reference.</param>
            internal ObjectInfo(IPdfPrimitive obj, PdfReference reference)
            {
                if (obj == null)
                    throw new ArgumentNullException("obj");

                if (reference == null)
                    throw new ArgumentNullException("reference");

                m_object = obj;
                m_reference = reference;
            }
            #endregion

            #region Public Methods
            /// <summary>
            /// Marks the object modified.
            /// </summary>
            public void SetModified()
            {
                m_bModified = true;
            }
            /// <summary>
            /// Sets the reference.
            /// </summary>
            /// <param name="reference">The reference.</param>
            internal void SetReference(PdfReference reference)
            {
                if (reference == null)
                    throw new ArgumentNullException("reference");

                if (m_reference != null)
                    throw new ArgumentException("The object has the reference bound to it.", "reference");

                m_reference = reference;
            }
            #endregion

            #region Overloads
            /// <summary>
            /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
            /// </returns>
            public override string ToString()
            {
                string str = ((m_reference != null) ? m_reference.ToString() : string.Empty) + " : " +
                    Object.GetType().Name;
                return str;
            }

            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="oi">The object information.</param>
            /// <param name="obj">The object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(ObjectInfo oi, object obj)
            {
                bool retVal = false;

                if (oi != null)
                {
                    retVal = oi.Equals(obj);
                }

                return retVal;
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="oi">The oi.</param>
            /// <param name="obj">The obj.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(ObjectInfo oi, object obj)
            {
                return !(oi == obj);
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object"></see>
            /// is equal to the current <see cref="T:System.Object"></see>.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object"></see>
            /// to compare with the current <see cref="T:System.Object"></see>.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object"></see>
            /// is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                bool retVal = false;

                if (obj != null)
                {
                    IPdfPrimitive prim = obj as IPdfPrimitive;
                    ObjectInfo oi = obj as ObjectInfo;

                    if (prim != null)
                    {
                        retVal = (Object == prim);
                    }
                    else if (oi != null)
                    {
                        retVal = (oi.Object == Object);
                    }
                }

                return retVal;
            }
            #endregion
        }

        #endregion
    }
}
