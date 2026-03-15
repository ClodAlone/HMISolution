#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using System.Drawing;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Primitives
{
#if NETFX_CORE || WP
    public class PdfArray:
#else
    internal class PdfArray :
#endif
        IPdfPrimitive,
        IEnumerable,
        IPdfChangable
    {
        #region Constants
        public const string StartMark = "[";
        public const string EndMark = "]";
        #endregion

        #region Fields
        /// <summary>
        /// The elements of the PDF array.
        /// </summary>
        private List<IPdfPrimitive> m_elements;
        /// <summary>
        /// Indicates if the array was changed.
        /// </summary>
        private bool m_bChanged;
        /// <summary>
        /// Shows the type of object status whether it is object registered or other status;
        /// </summary>
        private ObjectStatus m_status;
        /// <summary>
        /// Indicates if the object is currently in saving state or not.
        /// </summary>
        private bool m_isSaving;
        /// <summary>
        /// Holds the index number of the object.
        /// </summary>
        private int m_index;
        /// <summary>
        /// Internal variable to store the position.
        /// </summary>
        private int m_position = -1;
        /// <summary>
        /// Internal variable to hold PdfCrossTable reference.
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Internal variable to hold cloned object.
        /// </summary>
        private PdfArray m_clonedObject = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="T:IPdfSavable"/> at the specified index.
        /// </summary>
#if NETFX_CORE || WP
        public IPdfPrimitive this[int index]
#else
        internal IPdfPrimitive this[int index]
#endif
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", 
                        "The index can't be less then zero or greater then Count.");

                return m_elements[index] as IPdfPrimitive;
            }
        }
        /// <summary>
        /// Gets the count.
        /// </summary>
#if NETFX_CORE || WP
        public int Count
#else
        internal int Count
#endif
        {
            get
            {
                return m_elements.Count;
            }

        }

        /// <summary>
        /// Gets or sets the Status of the specified object.
        /// </summary>
        public ObjectStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this document is saving or not.
        /// </summary>
        public bool IsSaving
        {
            get
            {
                return m_isSaving;
            }
            set
            {
                m_isSaving = value;
            }
        }

        /// <summary>
        /// Gets or sets the integer value of the specified object.
        /// </summary>
        public int ObjectCollectionIndex
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the object.
        /// </summary>
        public int Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        /// <summary>
        /// Returns PdfCrossTable associated with the object.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
        }

        /// <summary>
        /// Returns cloned object.
        /// </summary>
        public IPdfPrimitive ClonedObject
        {
            get
            {
                return m_clonedObject;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfArray"/> class.
        /// </summary>
#if NETFX_CORE || WP
        public PdfArray()
#else
        internal PdfArray()
#endif
        {
            m_elements = new List<IPdfPrimitive>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfArray"/> class.
        /// </summary>
        /// <param name="array">The array.</param>
        internal PdfArray(PdfArray array)
        {
            m_elements = new List<IPdfPrimitive>(array.m_elements);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfArray"/> class.
        /// </summary>
        /// <param name="array">The array.</param>
        internal PdfArray(int[] array)
            : this()
        {
            foreach (int element in array)
            {
                Add(new PdfNumber(element));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfArray"/> class.
        /// </summary>
        /// <param name="array">The array.</param>
        internal PdfArray(float[] array)
            : this()
        {
            foreach (float element in array)
            {
                Add(new PdfNumber(element));
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfArray"/> class.
        /// </summary>
        /// <param name="array">The array.</param>
        public PdfArray(double[] array)
            : this()
        {
            foreach (double element in array)
            {
                Add(new PdfNumber(element));
            }
        }

        #endregion

        #region Static public methods
        /// <summary>
        /// Creates filled PDF array from the rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>The filled in PdfArray instance.</returns>
        public static PdfArray FromRectangle(RectangleF rectangle)
        {
            float[] values = { rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom };
            PdfArray array = new PdfArray(values);

            return array;
        }

        /// <summary>
        /// Creates filled PDF array from the rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>The filled in PdfArray instance.</returns>
        public static PdfArray FromRectangle(System.Drawing.Rectangle rectangle)
        {
            int[] values = { rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom };
            PdfArray array = new PdfArray(values);

            return array;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified element to the PDF array.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void Add(IPdfPrimitive element)
        {
            if (element == null)
                throw new ArgumentNullException("obj");

            m_elements.Add(element);
            MarkChanged();
        }

        /// <summary>
        /// Adds the specified list of elements to array.
        /// </summary>
        /// <param name="list">The list.</param>
        internal void Add(params IPdfPrimitive[] list)
        {
            foreach (IPdfPrimitive primitive in list)
            {
                if (primitive == null)
                {
                    throw new ArgumentNullException("list");
                }

                m_elements.Add(primitive);
            }

            if (list.Length > 0)
            {
                MarkChanged();
            }
        }

        /// <summary>
        /// Determines whether the specified element is within the array.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// 	<c>true</c> if the array contains the specified element; otherwise, <c>false</c>.
        /// </returns>
        internal bool Contains(IPdfPrimitive element)
        {
            return m_elements.Contains(element);
        }

        /// <summary>
        /// Inserts the element into the array.
        /// </summary>
        /// <param name="index">Zero-based index of the element in the array.</param>
        /// <param name="element">The element that should be added to the array.</param>
        internal void Insert(int index, IPdfPrimitive element)
        {
            m_elements.Insert(index, element);
            MarkChanged();
        }

        /// <summary>
        /// Checks whether array contains the element.
        /// </summary>
        /// <param name="element">The element object.</param>
        /// <returns>Index of the element in the array if exists, -1 otherwise.</returns>
        internal int IndexOf(IPdfPrimitive element)
        {
            return m_elements.IndexOf(element);
        }
        /// <summary>
        /// Removes element from the array.
        /// </summary>
        /// <param name="element">The element that should be removed from the array.</param>
        internal void Remove(IPdfPrimitive element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            m_elements.Remove(element);
            MarkChanged();
        }

        /// <summary>
        /// ReArrange the Nested page kids array
        /// </summary>
        /// <param name="orderArray">To specify the in which sequence the pages are arranged.</param>
        internal void ReArrange(int[] orderArray)
        {
            int orderArrayCount = orderArray.Length;
            PdfReferenceHolder[] tempPageKids = new PdfReferenceHolder[Count];
            int[] remainingKids = new int[Count];

            //Create the temp page kids array.
            for (int i = 0; i < Count; i++)
            {
                tempPageKids[i] = m_elements[i] as PdfReferenceHolder;
            }

            //It Identifies m_order pages are only exist.
            if (orderArrayCount <= Count)
            {
                if (PdfLoadedPageCollection.m_repeatIndex != 0)
                {
                    for (int i = 0; i < PdfLoadedPageCollection.m_repeatIndex; i++)
                    {
                        m_elements[i] = tempPageKids[orderArray[i]];
                        remainingKids[orderArray[i]] = 1;
                    }
                }
                else
                {
                    for (int i = 0; i < orderArrayCount; i++)
                    {
                        m_elements[i] = tempPageKids[orderArray[i]];
                        remainingKids[orderArray[i]] = 1;
                    }
                }
            }

            //It Identifies all pages are exist.
            if (orderArrayCount > Count)
            {
                for (int i = 0; i < Count; i++)
                {
                    m_elements[i] = tempPageKids[orderArray[i]];
                    remainingKids[orderArray[i]] = 1;
                }
            }

            //Delete the missed pages from m_order.
            if (Count != orderArrayCount)
            {
                //  int count = tempPageKids.Length;
                int count;
                if (PdfLoadedPageCollection.m_nestedPages == 1)
                {
                    count = PdfLoadedPageCollection.m_parentKidsCounttemp;
                }
                else
                {
                    count = PdfLoadedPageCollection.m_parentKidsCount;
                }
                for (int i = 0; i < count; i++)
                {

                    if (remainingKids[i] == 0)
                    {
                        if (PdfLoadedPageCollection.m_repeatIndex != 0)
                        {
                            RemoveAt(PdfLoadedPageCollection.m_repeatIndex);
                        }
                        else
                        {
                            RemoveAt(orderArrayCount);
                        }
                    }
                }
            }

            MarkChanged();
        }

        /// <summary>
        /// Remove the element from the array by its index.
        /// </summary>
        /// <param name="index">Zero-based index of the element in the array.</param>
        internal void RemoveAt(int index)
        {
            m_elements.RemoveAt(index);
            MarkChanged();
        }
        /// <summary>
        /// Cleares the array.
        /// </summary>
        internal void Clear()
        {
            m_elements.Clear();
            MarkChanged();
        }

        /// <summary>
        /// Converts an instance of the PdfArray to the RectangleF.
        /// </summary>
        /// <returns>The properly filled RectangleF structure.</returns>
        public RectangleF ToRectangle()
        {
            if (Count < 4)
                throw new InvalidOperationException("Can't convert to rectangle.");

            float x1, x2, y1, y2;


            PdfNumber num = GetNumber(0);

            x1 = num.FloatValue;

            num = GetNumber(1);
            y1 = num.FloatValue;

            num = GetNumber(2);
            x2 = num.FloatValue;

            num = GetNumber(3);
            y2 = num.FloatValue;

            float x = Math.Min(x1, x2);
            float y = Math.Min(y1, y2);
            float width = Math.Abs(x1 - x2);
            float height = Math.Abs(y1 - y2);

            RectangleF rect = new RectangleF(x, y, width, height);

            return rect;
        }
        #endregion

        #region IPdfSavable Members
        /// <summary>
        /// Saves the object using the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void Save(IPdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(StartMark);

            for (int i = 0, len = Count; i < len; i++)
            {
                this[i].Save(writer);

                if (i + 1 != len)
                {
                    writer.Write(Operators.WhiteSpace);
                }
            }

            writer.Write(EndMark);
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see>
        /// object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return m_elements.GetEnumerator();
        }
        #endregion

        #region IPdfChangable Members
        /// <summary>
        /// Marks the object changed.
        /// </summary>
        public void MarkChanged()
        {
            m_bChanged = true;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:PdfArray"/> is changed.
        /// </summary>
        public bool Changed
        {
            get
            {
                return m_bChanged;
            }
        }

        /// <summary>
        /// Freezes the changes.
        /// </summary>
        /// <param name="freezer">The freezer.</param>
        public void FreezeChanges(object freezer)
        {
            if (freezer is PdfParser || freezer == this)
            {
                m_bChanged = false;
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Gets the number from the array.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The proper instance of the PdfNumber.</returns>
        private PdfNumber GetNumber(int index)
        {
            PdfNumber num = this[index] as PdfNumber;

            if (num == null)
                throw new InvalidOperationException("Can't convert to rectangle.");

            return num;
        }

        /// <summary>
        /// Creates a copy of PdfArray.
        /// </summary>
        public IPdfPrimitive Clone(PdfCrossTable crossTable)
        {
            if (m_clonedObject != null && m_clonedObject.CrossTable == crossTable)
                return m_clonedObject;
            else
                m_clonedObject = null;

            // Else clone the object.
            PdfArray newArray = new PdfArray();

            foreach (IPdfPrimitive obj in m_elements)
                newArray.Add(obj.Clone(crossTable));

            newArray.m_crossTable = crossTable;
            m_clonedObject = newArray;

            return newArray;
        }
        #endregion
    }
}
