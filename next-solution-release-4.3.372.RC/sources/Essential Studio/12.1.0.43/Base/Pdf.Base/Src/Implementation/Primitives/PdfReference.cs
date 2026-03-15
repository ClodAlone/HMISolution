#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
//
#endregion

#region file using directives
using System;
using System.Globalization;

using Syncfusion.Pdf.IO;
#endregion

namespace Syncfusion.Pdf.Primitives
{
    /// <summary>
    /// Represents a PDF reference.
    /// </summary>
#if NETFX_CORE || WP
    public class PdfReference
#else
    internal class PdfReference
#endif
        : IPdfPrimitive
    {
        #region Fields
        /// <summary>
        /// Holds the object number.
        /// </summary>
        public readonly long ObjNum;
        /// <summary>
        /// Holds the generation number of the object.
        /// </summary>
        public readonly int GenNum;
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
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="objNum">The object number.</param>
        /// <param name="genNum">The generation number.</param>
        public PdfReference(long objNum, int genNum)
        {
            ObjNum = objNum;
            GenNum = genNum;
        }
        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="objNum">The object number.</param>
        /// <param name="genNum">The generation number.</param>
        public PdfReference(string objNum, string genNum)
        {
            double objN, genN;
            if (!double.TryParse(objNum, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out objN))
            {
                throw new ArgumentException("Invalid format (must be an integer)", "objNum");
            }

            if (!double.TryParse(genNum, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out genN))
            {
                throw new ArgumentException("Invalid format (must be an integer)", "genNum");
            }

            ObjNum = (int)objN;
            GenNum = (int)genN;
        }
        #endregion

        #region Properties
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
        /// Returns cloned object.
        /// </summary>
        public IPdfPrimitive ClonedObject
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns a string representing the object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} R", ObjNum, GenNum);
        }
        /// <summary>
        /// Compares two object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>The result of comparison.</returns>
        public override bool Equals(object obj)
        {
            PdfReference r = obj as PdfReference;

            if (r == null) return false;

            return !((r.ObjNum != ObjNum) || (r.GenNum != GenNum));
        }

        /// <summary>
        /// Returns a hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return (int)(ObjNum + ((long)GenNum) << 24);
        }

        /// <summary>
        /// Creates a copy of PdfReference.
        /// </summary>
        IPdfPrimitive IPdfPrimitive.Clone(PdfCrossTable crossTable)
        {
            return null;
        }

        /// <summary>
        /// Compares two reference objects.
        /// </summary>
        /// <param name="ref1">The first object to compare.</param>
        /// <param name="ref2">The second object to compare.</param>
        /// <returns>The result of the comparison.</returns>
        public static bool operator ==(PdfReference ref1, PdfReference ref2)
        {
            object r1 = ref1, r2 = ref2;

            if (r1 == null || r2 == null)
            {
                return (r1 == r2);
            }
            else
            {
                return ((ref1.ObjNum == ref2.ObjNum) && (ref1.GenNum == ref2.GenNum));
            }
        }
        /// <summary>
        /// Compares two reference objects.
        /// </summary>
        /// <param name="ref1">The first object to compare.</param>
        /// <param name="ref2">The second object to compare.</param>
        /// <returns>The result of the comparison.</returns>
        public static bool operator !=(PdfReference ref1, PdfReference ref2)
        {
            return !(ref1 == ref2);
        }
        #endregion

        #region IPDFSave Members
        /// <summary>
        /// Writes a reference into a PDF document.
        /// </summary>
        /// <param name="writer">A PDF writer.</param>
        public void Save(IPdfWriter writer)
        {
            writer.Write(ToString());
        }

        #endregion
    }
    /// <summary>
    /// Class that is like a reference but during saving it is replaced by
    /// a real reference to the object it holds.
    /// </summary>
#if NETFX_CORE || WP
    public class PdfReferenceHolder
#else
    internal class PdfReferenceHolder
#endif
        : IPdfPrimitive
    {
        #region Fields
        /// <summary>
        /// The object which the reference is of.
        /// </summary>
        private IPdfPrimitive m_object;
        /// <summary>
        /// The cross-reference table, which the object is within.
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// The reference to the object, which was read from the PDF document.
        /// </summary>
        private PdfReference m_reference;
        /// <summary>
        /// The index of the object within the object collection.
        /// </summary>
        private int m_objectIndex = -1;
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
        #endregion

        #region Properties
        /// <summary>
        /// Gets the object the reference is of.
        /// </summary>
#if NETFX_CORE || WP
        public IPdfPrimitive Object
#else
        internal IPdfPrimitive Object
#endif
        {
            get
            {
                if ((m_reference!=null)||(m_object == null))
                    m_object = GetObject();

                return m_object;
            }
        }
        /// <summary>
        /// Gets the index of the object.
        /// </summary>
        internal int Index
        {
            get
            {
                if (m_objectIndex < 0)
                {
                    PdfMainObjectCollection items = m_crossTable.PdfObjects;
                    m_objectIndex = items.GetObjectIndex(m_reference);

                    if (m_objectIndex < 0)
                    {
                        IPdfPrimitive obj = m_crossTable.GetObject(m_reference);
                        m_objectIndex = items.Count - 1;
                    }
                }

                return m_objectIndex;
            }
        }
        /// <summary>
        /// Gets the reference.
        /// </summary>
        public PdfReference Reference
        {
            get
            {
                return m_reference;
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
        /// Returns cloned object.
        /// </summary>
        public IPdfPrimitive ClonedObject
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region Class Initialize/Finalize method
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfReferenceHolder"/> class.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        public PdfReferenceHolder(IPdfWrapper wrapper)
            : this(wrapper.Element)
        {
        }

        /// <summary>
        /// Initializes the class instance with an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public PdfReferenceHolder(IPdfPrimitive obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            m_object = obj;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PDFReferenceHolder"/> class.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="crossTable">The cross-reference table.</param>
        internal PdfReferenceHolder(PdfReference reference, PdfCrossTable crossTable)
        {
            if (crossTable == null)
                throw new ArgumentNullException("crossTable");

            if (reference == null)
                throw new ArgumentNullException("reference");

            m_crossTable = crossTable;
            m_reference = reference;
        }
        #endregion

        #region IPDFSaveable Members
        /// <summary>
        /// Saves the object.
        /// </summary>
        /// <param name="writer">A PDF writer.</param>
        public void Save(IPdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            long position = writer.Position;
            PdfCrossTable cTable = writer.Document.CrossTable;
            if (cTable.Document is PdfDocument)
                Object.IsSaving = true;
            PdfReference reference = cTable.GetReference(Object);

            if (writer.Position != position)
            {
                writer.Position = position;
            }

            reference.Save(writer);
        }

        /// <summary>
        /// Create a copy of the referenced object.
        /// </summary>
        public IPdfPrimitive Clone(PdfCrossTable crossTable)
        {
            PdfReferenceHolder refHolder = null;
            IPdfPrimitive temp = null;
            string refNum = string.Empty;
            PdfReference reference = null;

            // Restricts addition of same object multiple time.
            if (Reference != null && m_crossTable != null && m_crossTable.PageCorrespondance.ContainsKey(Reference))
            {
                refHolder = new PdfReferenceHolder(m_crossTable.PageCorrespondance[Reference] as PdfReference, crossTable);

                return refHolder;
            }

            // Resolves Page references for annotations.
            if (m_crossTable != null && m_crossTable.PageCorrespondance.ContainsKey(Object))
            {
                PdfPageBase page = m_crossTable.PageCorrespondance[Object] as PdfPageBase;
                if (page != null)
                    temp = page.Dictionary;
                else
                    return new PdfNull();
            }
            else
            {
                if (Object is PdfNumber)
                    return new PdfNumber((Object as PdfNumber).FloatValue);

                if (Object is PdfDictionary)
                {
                    // Meaning the referenced page is not available for import.
                    PdfName type = new PdfName(DictionaryProperties.Type);
                    PdfDictionary dict = Object as PdfDictionary;
                    if (dict.ContainsKey(type) && (dict[type] as PdfName).Value == "Page")
                        return new PdfNull();
                }

                // Resolves circular references.
                if (crossTable.PrevReference != null && crossTable.PrevReference.Contains(Reference))
                {
                    IPdfPrimitive obj = m_crossTable.GetObject(Reference).ClonedObject;
                    if (obj != null)
                    {
                        reference = crossTable.GetReference(obj);
                        return new PdfReferenceHolder(reference, crossTable);
                    }
                    else
                        return new PdfNull();
                }

                if(Reference!=null)
                crossTable.PrevReference.Add(Reference);

                if (!(Object is PdfCatalog))
                    temp = Object.Clone(crossTable);
                else
                    temp = crossTable.Document.Catalog;
            }

            reference = crossTable.GetReference(temp);
            refHolder = new PdfReferenceHolder(reference, crossTable);

            return refHolder;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Compares two reference holders.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>The result of comparison.</returns>
        public override bool Equals(object obj)
        {
            PdfReferenceHolder reference = obj as PdfReferenceHolder;

            bool result = (reference != null);

            if (result)
            {
                if (m_reference != null && reference.m_reference != null)
                {
                    result &= reference.m_reference == m_reference;
                }
                else
                {
                    result &= (reference.Object == Object);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a hashcode for a hashtable.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }

        /// <summary>
        /// Compares two reference holders whether they are equal.
        /// </summary>
        /// <param name="rh1">A reference holder to compare.</param>
        /// <param name="rh2">A reference holder to compare.</param>
        /// <returns>The result of the comparison.</returns>
        public static bool operator ==(PdfReferenceHolder rh1, PdfReferenceHolder rh2)
        {
            object obj1 = rh1 as object;
            object obj2 = rh2 as object;

            if (obj1 == null || obj2 == null)
            {
                return (obj1 == obj2);
            }
            else return rh1.Equals(rh2);
        }
        /// <summary>
        /// Compares two reference holders whether they are different.
        /// </summary>
        /// <param name="rh1">A reference holder to compare.</param>
        /// <param name="rh2">A reference holder to compare.</param>
        /// <returns>The result of the comparison.</returns>
        public static bool operator !=(PdfReferenceHolder rh1, PdfReferenceHolder rh2)
        {
            return !(rh1 == rh2);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <returns></returns>
        private IPdfPrimitive GetObject()
        {
            IPdfPrimitive obj = null;

            if (m_reference != null)
            {
                obj = m_crossTable.PdfObjects.GetObject(Index);
            }
            else if (m_object != null)
            {
                obj = m_object;
            }

            return obj;
        }
        #endregion

    }
}
