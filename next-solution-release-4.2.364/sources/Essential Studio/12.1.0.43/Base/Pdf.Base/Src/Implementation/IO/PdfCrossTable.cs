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

#if !SyncfusionFramework1_0 && !SyncfusionFramework1_1
#define Generics
#endif

#region Using directives
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;
using System.Collections.Generic;

#endregion

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// PDFCrossTable is responsible for intermediate level parsing
    /// and savingof a PDF document.
    /// </summary>
#if NETFX_CORE || WP
    public class PdfCrossTable : IDisposable
#else
    internal class PdfCrossTable : IDisposable
#endif
    {
        #region Fields
        /// <summary>
        /// The low level implementation of the cross-reference table.
        /// </summary>
        private CrossTable m_crossTable;
        /// <summary>
        /// The document catalog.
        /// </summary>
        private PdfDictionary m_documentCatalog;
        /// <summary>
        /// The stream the file within.
        /// </summary>
        private Stream m_stream;
        /// <summary>
        /// The modified objects that should be saved.
        /// </summary>
        private Dictionary<long, RegisteredObject> m_objects = new Dictionary<long, RegisteredObject>();
        /// <summary>
        /// The number of the objects.
        /// </summary>
        private int m_count;
        /// <summary>
        /// Shows if the class have been dicposed.
        /// </summary>
        private bool m_bDisposed = false;
        /// <summary>
        /// The trailer for a new document.
        /// </summary>
        private IPdfPrimitive m_trailer;
        /// <summary>
        /// The main PdfDocument class instance.
        /// </summary>
        private PdfDocumentBase m_document;
        /// <summary>
        /// Flag that forces an object to be 'a new'.
        /// </summary>
        private bool m_bForceNew;
        /// <summary>
        /// The obj number stack. Holds object numbers
        /// that are used to decode strings and streams.
        /// </summary>
        private Stack<PdfReference> m_objNumbers = new Stack<PdfReference>();
        /// <summary>
        /// Holds maximal generation number or offset to object.
        /// </summary>
        private long m_maxGenNumIndex = 0;
        /// <summary>
        /// The current object archive.
        /// </summary>
        private PdfArchiveStream m_archive;
        /// <summary>
        /// The mapped references.
        /// </summary>
        Dictionary<PdfReference, PdfReference> m_mappedReferences;
        /// <summary>
        /// The value of the count stored
        /// before count was wiped out.
        /// </summary>
        private int m_storedCount;
        /// <summary>
        /// The list of the completed archives.
        /// </summary>
        private List<ArchiveInfo> m_archives;

        private PdfDictionary m_encryptorDictionary;
        private PdfMainObjectCollection m_items;
        /// <summary>
        /// Internal variable to identify the current object is PdfEncryptor or not.
        /// </summary>
        private bool m_bEncrypt = false;

        /// <summary>
        /// Internal variable to store pages.
        /// </summary>
        private Dictionary<IPdfPrimitive, Object> m_pageCorrespondance;

        /// <summary>
        /// Internal variable to store reference.
        /// </summary>
        private List<PdfReference> m_preReference;

        /// <summary>
        /// Internal variable to store if document is being merged.
        /// </summary>
        private bool m_isMerging;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current object is PdfEncryptor or not. 
        /// </summary>
        internal bool Encrypted
        {
            get
            {
                return m_bEncrypt;
            }
            set
            {
                m_bEncrypt = value;
            }
        }

        /// <summary>
        /// Returns the document catalog.
        /// </summary>
        public PdfDictionary DocumentCatalog
        {
            get
            {
                if (m_documentCatalog == null && m_crossTable != null)
                {
                    m_documentCatalog = Dereference(m_crossTable.DocumentCatalog)
                        as PdfDictionary;
                }

                return m_documentCatalog;
            }
        }

        /// <summary>
        /// Returns the source stream.
        /// </summary>
        internal Stream Stream
        {
            get
            {
                return m_crossTable.Stream;
            }
        }

        /// <summary>
        /// Returns next available object number.
        /// </summary>
        internal int NextObjNumber
        {
            get
            {
                if (Count == 0)
                {
                    Count++;
                }
                return Count++;
            }
        }

        /// <summary>
        /// Returns a low-level cross-reference table parser.
        /// </summary>
        internal CrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
        }

        /// <summary>
        /// Gets or sets the number of the objects within the document.
        /// </summary>
        internal int Count
        {
            get
            {
                if (m_count == 0)
                {
                    IPdfPrimitive obj = null;
                    PdfNumber count;

                    if (m_crossTable != null)
                    {
                        obj = m_crossTable.Trailer[DictionaryProperties.Size];
                    }

                    if (obj != null)
                    {
                        count = Dereference(obj) as PdfNumber;
                    }
                    else
                    {
                        count = new PdfNumber(0);
                    }

                    m_count = count.IntValue;
                }
                return m_count;
            }
            set
            {
                if (value == 0)
                    throw new ArgumentException("The value can't be 0.", "Count");

                m_count = value;
            }
        }

        /// <summary>
        /// Gets or sets the main PdfDocument class instance.
        /// </summary>
        internal PdfDocumentBase Document
        {
            get
            {
                return m_document;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Document");

                m_document = value;
                m_items = m_document.PdfObjects;
            }
        }

        /// <summary>
        /// Gets the chached PDF object main collection.
        /// </summary>
        internal PdfMainObjectCollection PdfObjects
        {
            get
            {
                return m_items;
            }
        }

        /// <summary>
        /// Gets the trailer.
        /// </summary>
        internal PdfDictionary Trailer
        {
            get
            {
                if (m_trailer == null)
                {
                    m_trailer = (m_crossTable == null) ? new PdfStream() : m_crossTable.Trailer;
                }
                if ( (m_trailer as PdfDictionary).ContainsKey("XRefStm"))
                {
                    (m_trailer as PdfDictionary).Remove(new PdfName("XRefStm"));
                }
                return m_trailer as PdfDictionary;
            }
        }

        /// <summary>
        /// Gets or sets if the document is merged.
        /// </summary>
        internal bool IsMerging
        {
            get
            {
                return m_isMerging;
            }
            set
            {
                m_isMerging = value;
            }
        }

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets or sets the encryptor.
        /// </summary>
        internal PdfEncryptor Encryptor
        {
            get
            {
                return m_crossTable.Encryptor;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Encryptor");

                m_crossTable.Encryptor = value.Clone();
            }
        }
#endif

        /// <summary>
        /// Gets the object collection.
        /// </summary>
        private PdfMainObjectCollection ObjectCollection
        {
            get
            {
                return m_document.PdfObjects;
            }
        }

        /// <summary>
        /// Gets the security dictionary.
        /// </summary>
        /// <value>The security dictionary.</value>
        internal PdfDictionary EncryptorDictionary
        {
            get
            {
                if (m_encryptorDictionary == null)
                {
                    m_bEncrypt = true;
                    m_encryptorDictionary = Dereference(Trailer[DictionaryProperties.Encrypt]) as PdfDictionary;
                }
                m_bEncrypt = false;
                return m_encryptorDictionary;
            }
        }

        /// <summary>
        /// Gets or sets page correspondance up on each page import.
        /// </summary>
        internal Dictionary<IPdfPrimitive, Object> PageCorrespondance
        {
            get
            {
                if (m_pageCorrespondance == null)
                    m_pageCorrespondance = new Dictionary<IPdfPrimitive, Object>();
                return m_pageCorrespondance;
            }
            set
            {
                m_pageCorrespondance = value;
            }
        }

        /// <summary>
        /// Gets or sets the PdfReference of latest processed object.
        /// </summary>
        internal List<PdfReference> PrevReference
        {
            get
            {
                if (m_preReference == null)
                    m_preReference = new List<PdfReference>();

                return m_preReference;
            }
            set
            {
                m_preReference = value;
            }
        }
        
        internal bool StructureAltered
        {
            get
            {
                return m_crossTable.IsStructureAltered;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// The costructor of the table.
        /// </summary>
        /// <param name="docStream">A stream which contains the document.</param>
        public PdfCrossTable(Stream docStream)
        {
            if (docStream == null)
                throw new ArgumentNullException("stream");

            m_stream = docStream;
            m_crossTable = new CrossTable(docStream, this);
        }
        /// <summary>
        /// A costructor thar initialize a new cross table.
        /// </summary>
        public PdfCrossTable()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCrossTable"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="encryptionDictionary">The encryption dictionary.</param>
        internal PdfCrossTable(int count, PdfDictionary encryptionDictionary)
            : this()
        {
            m_storedCount = count;
            m_bForceNew = true;
            m_encryptorDictionary = encryptionDictionary;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="PdfCrossTable"/> is reclaimed by garbage collection.
        /// </summary>
        ~PdfCrossTable()
        {
            Dispose(false);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Dereferences the specified primitive object.
        /// </summary>
        /// <param name="obj">The primitive object.</param>
        /// <returns>Dereferenced object.</returns>
        public static IPdfPrimitive Dereference(IPdfPrimitive obj)
        {
            //if( obj == null )
            //  throw new ArgumentNullException( "obj" );

            PdfReferenceHolder rh = obj as PdfReferenceHolder;

            if (rh != null)
            {
                obj = rh.Object;
            }

            return obj;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Retrieves an object by its reference.
        /// </summary>
        /// <param name="pointer">The reference of the object.</param>
        /// <returns>The object read from its reference.</returns>
        public IPdfPrimitive GetObject(IPdfPrimitive pointer)
        {
            IPdfPrimitive result = pointer;

            if (pointer is PdfReferenceHolder)
            {
                result = (pointer as PdfReferenceHolder).Object;
            }
            else if (pointer is PdfReference)
            {
                PdfReference reference = pointer as PdfReference;
                m_objNumbers.Push(pointer as PdfReference);

                IPdfPrimitive obj;
                if (m_crossTable != null)
                    obj = m_crossTable.GetObject(pointer);
                else
                    obj = PdfObjects.GetObject(PdfObjects.GetObjectIndex(reference));
                obj = PageProceed(obj);

                PdfMainObjectCollection goc = PdfObjects;
                // TODO: Register reference.
                if (obj != null)
                {
                    if (goc.Contains(obj))
                    {
                        // Do nothing...
                    }
                    else if (goc.ContainsReference(reference))
                    {
                        int index = goc.GetObjectIndex(reference);
                        obj = goc.GetObject(index);
                    }
                    else // neither reference nor object.
                    {
                        goc.Add(obj, reference);
                        if (!m_isMerging)
                        {
                            obj.Position = -1;
                            reference.Position = -1;
                        }
                    }
                }
                result = obj;

                if (Document.WasEncrypted)
                {
#if SILVERLIGHT || WP
                    throw new NotSupportedException("Encrypted Document are currently not supported.");
#else
                    Decrypt(result);
#endif
                }
            }

            if (Document.WasEncrypted)
            {
                IPdfDecryptable encryptedObj = result as IPdfDecryptable;
#if SILVERLIGHT || WP
                throw new NotSupportedException("Encrypted Document are currently not supported.");
#else
                    Decrypt(encryptedObj);
#endif

            }

            if (pointer is PdfReference)
            {
                m_objNumbers.Pop();
            }

            return result;
        }

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Decrypt a decryptable object. Should be called during Prepare().
        /// </summary>
        /// <param name="obj">The decrypted object.</param>
        private void Decrypt(IPdfDecryptable obj)
        {
            if (Document.WasEncrypted
                && obj != null
                && !obj.Decrypted
                && m_objNumbers.Count > 0
                && Encryptor != null)
            {
                PdfEncryptor encryptor = Encryptor;
                long currObjNumber = (m_objNumbers.Peek() as PdfReference).ObjNum;

                obj.Decrypt(encryptor, currObjNumber);
            }
        }

        /// <summary>
        /// Decrypts the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void Decrypt(IPdfPrimitive obj)
        {
            PdfDictionary dic = obj as PdfDictionary;
            PdfArray arr = obj as PdfArray;

            if (dic != null && !dic.IsDecrypted)
            {
                foreach (IPdfPrimitive element in dic.Values)
                {
                    Decrypt(element);
                }

                Decrypt(dic as IPdfDecryptable);
            }
            else if (arr != null)
            {
                foreach (IPdfPrimitive element in arr)
                {
                    Decrypt(element);
                }
            }
            else if ((obj is PdfString))
            {
                PdfString str = obj as PdfString;
                if(!str.Decrypted && !str.Hex)
                   Decrypt(obj as IPdfDecryptable);
            }
            else
            {
                Decrypt(obj as IPdfDecryptable);
            }
        }
#endif

        /// <summary>
        /// Retrieves a PDF stream from a PDF document.
        /// </summary>
        /// <param name="streamRef">The reference object to the stream.</param>
        /// <returns>The array of bytes taken from the PDF stream.</returns>
        public byte[] GetStream(IPdfPrimitive streamRef)
        {
            if (streamRef == null)
                throw new ArgumentNullException("streamRef");

            return m_crossTable.GetStream(streamRef);
        }

        /// <summary>
        /// Registers the object in the cross reference table.
        /// </summary>
        /// <param name="offset">The offset of the object within the file</param>
        /// <param name="reference">The representation of the reference to the object.
        /// </param>
        public void RegisterObject(long offset, PdfReference reference)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");

            // Register the object by its number.
            m_objects[reference.ObjNum] = new RegisteredObject(offset, reference);
            m_maxGenNumIndex = Math.Max(m_maxGenNumIndex, reference.GenNum);
        }

        /// <summary>
        /// Registers an archived object.
        /// </summary>
        /// <param name="archive">The archive.</param>
        /// <param name="reference">The reference to the object.</param>
        public void RegisterObject(PdfArchiveStream archive, PdfReference reference)
        {
            m_objects[reference.ObjNum] = new RegisteredObject(this, archive, reference);
            m_maxGenNumIndex = Math.Max(m_maxGenNumIndex, archive.Count);
        }

        /// <summary>
        /// Registers the object in the cross reference table.
        /// </summary>
        /// <param name="offset">The offset of the object within the file</param>
        /// <param name="reference">The representation of the reference to the object.
        /// </param>
        /// <param name="free">True if object is free.</param>
        public void RegisterObject(long offset, PdfReference reference, bool free)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");

            // Register the object by its number.
            m_objects[reference.ObjNum] = new RegisteredObject(offset, reference, free);
            m_maxGenNumIndex = Math.Max(m_maxGenNumIndex, reference.GenNum);
        }

        /// <summary>
        /// Saves the cross-reference table into the stream.
        /// </summary>
        /// <param name="writer">The stream writer to save the cross-reference table into.
        /// </param>
        public void Save(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            SaveHead(writer);

            bool state = false;
#if !SILVERLIGHT && !NETFX_CORE && !WP
            PdfSecurity pdfSecurity = m_document.Security;
#endif
            m_mappedReferences = null;

            if (m_archives != null)
                m_archives.Clear();

            m_archive = null;

            if (m_objects != null)
                m_objects.Clear();

            MarkTrailerReferences();

#if SILVERLIGHT || NETFX_CORE || WP
            SaveObjects(writer);
#else
            if ((m_document.FileStructure.CrossReferenceType == PdfCrossReferenceType.CrossReferenceTable) && (pdfSecurity!=null))
            {
                if (pdfSecurity.Enabled==true && pdfSecurity.Encryptor.Encrypt==true &&  (m_document is PdfDocument) && ((pdfSecurity.Encryptor.UserPassword.Length==0) &&((pdfSecurity.Encryptor.OwnerPassword.Length==0))))
                {
                    state = pdfSecurity.Enabled;
                    pdfSecurity.Enabled = false;
                }
            }

            SaveObjects(writer);

            if ((m_document.FileStructure.CrossReferenceType == PdfCrossReferenceType.CrossReferenceTable) && (pdfSecurity != null))
            {
                if (pdfSecurity.Enabled == true && pdfSecurity.Encryptor.Encrypt == true && (m_document is PdfDocument) && ((pdfSecurity.Encryptor.UserPassword.Length == 0) && ((pdfSecurity.Encryptor.OwnerPassword.Length == 0))))
                {
                    pdfSecurity.Enabled = state;
                }
            }
#endif

            int saveCount = Count;

            SaveArchives(writer);

            if (writer.GetStream().CanSeek)
            {
                writer.Position = writer.Length;
            }
            long xrefPos = writer.Position;

            RegisterObject(0, new PdfReference(0, -1), true);

            long prevXRef = (m_crossTable == null) ? 0 : m_crossTable.XRefOffset;
            prevXRef = m_bForceNew ? 0 : prevXRef;

            if (IsCrossReferenceStream(writer.Document))
            {
                PdfReference xRefReference;
                PdfStream xRefStream = PrepareXRefStream(prevXRef, xrefPos, out xRefReference);
                xRefStream.BlockEncryption();
                //SaveIndirectObject( xRefStream, writer );
                DoSaveObject(xRefStream, xRefReference, writer);
            }
            else
            {
                writer.Write(Operators.xref);
                writer.Write(Operators.NewLine);
                SaveSections(writer);
                SaveTrailer(writer, Count, prevXRef);
            }

            SaveTheEndess(writer, xrefPos);

            Count = saveCount;
            for (int i = 0; i < ObjectCollection.Count; ++i)
            {
                PdfMainObjectCollection.ObjectInfo oi = ObjectCollection[i];
                oi.Object.IsSaving = false;
            }
        }

        /// <summary>
        /// Retrieves the reference of the object given.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The reference.</returns>
        /// <remarks>If there is no registered reference, create a new one and register
        /// it.</remarks>
        internal PdfReference GetReference(IPdfPrimitive obj)
        {
            bool wasNew;

            return GetReference(obj, out wasNew);
        }

        /// <summary>
        /// Retrieves the reference of the object given.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="bNew">The output flag, which shows if the object is new.</param>
        /// <returns>The reference.</returns>
        internal PdfReference GetReference(IPdfPrimitive obj, out bool bNew)
        {
            bool isNew = false;


            if (obj is PdfArchiveStream)
            {
                PdfReference r = FindArchiveReference(obj as PdfArchiveStream);
                bNew = isNew;
                return r;
            }

            if (obj is PdfReferenceHolder)
            {
                obj = (obj as PdfReferenceHolder).Object;
                if (m_document is PdfDocument)
                    obj.IsSaving = true;
            }

            if (obj is IPdfWrapper)
            {
                obj = (obj as IPdfWrapper).Element;
            }

            bool wasNew;
            PdfReference reference = null;

            if (obj.IsSaving)
            {
                if (m_items.Count > 0 && obj.ObjectCollectionIndex > 0 && m_items.Count > obj.ObjectCollectionIndex - 1)
                {
                    if (m_items[obj.ObjectCollectionIndex - 1].Equals(obj))
                        reference = Document.PdfObjects.GetReference(obj.ObjectCollectionIndex - 1);
                    else
                        reference = Document.PdfObjects.GetReference(obj, out wasNew);
                }
            }
            else
                reference = Document.PdfObjects.GetReference(obj, out wasNew);

            if (reference == null)
            {
                if (obj.Status == ObjectStatus.Registered)
                    wasNew = false;
                else
                    wasNew = true;
            }
            else
                wasNew = false;

            if (m_bForceNew)
            {
                if (reference == null)
                {
                    long maxObj = (m_storedCount > 0) ? m_storedCount++ : Document.PdfObjects.Count;

                    if (maxObj <= 0)
                    {
                        maxObj = 1;
                        m_storedCount = 2;
                    }

                    reference = new PdfReference(maxObj, 0);

                    if (wasNew)
                    {
                        Document.PdfObjects.Add(obj, reference);
                        if (!m_isMerging)
                        {
                            obj.Position = -1;
                            reference.Position = -1;
                        }
                    }
                    else
                    {
                        bool found;
                        Document.PdfObjects.TrySetReference(obj, reference, out found);
                    }
                }

                reference = GetMappedReference(reference);
            }

            if (reference == null)
            {
                // If the object is new, always use 0 generation number.
                reference = new PdfReference(NextObjNumber, 0);
                bool found;

                if (wasNew)
                {
                    Document.PdfObjects.Add(obj);
                    Document.PdfObjects.TrySetReference(obj, reference, out found);

                    if (!m_isMerging)
                        obj.Position = -1;
                }
                else
                    Document.PdfObjects.TrySetReference(obj, reference, out found);

                obj.ObjectCollectionIndex = (int)reference.ObjNum;
                obj.Status = ObjectStatus.None;
                isNew = true;
            }

            bNew = isNew || m_bForceNew;

            return reference;
        }

        /// <summary>
        /// Forces all object to be 'a new'.
        /// </summary>
        internal void ForceNew()
        {
            m_crossTable.Trailer.Remove(DictionaryProperties.Size);
            m_crossTable.Trailer.Remove(DictionaryProperties.Prev);

            if (m_count > 0)
            {
                m_storedCount = m_count;
            }

            m_count = 0;
            m_bForceNew = true;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Marks the trailer references being saved.
        /// </summary>
        private void MarkTrailerReferences()
        {
            foreach (IPdfPrimitive obj in Trailer.Values)
            {
                PdfReferenceHolder rh = obj as PdfReferenceHolder;

                if (rh != null)
                {
                    if (!Document.PdfObjects.Contains(rh.Object))
                    {
                        Document.PdfObjects.Add(rh.Object);
                        if (!m_isMerging)
                            rh.Object.Position = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if an object is a page and convert it into a loaded page if necessary.
        /// </summary>
        /// <param name="obj">The object, which should be checked.</param>
        /// <returns>The checked and modified object.</returns>
        private IPdfPrimitive PageProceed(IPdfPrimitive obj)
        {
            if (obj is PdfLoadedPage)
            {
                return obj;
            }

            PdfDictionary dic = obj as PdfDictionary;

            if (dic != null && !(obj is PdfPage))
            {
                if (dic.ContainsKey(DictionaryProperties.Type))
                {
                    IPdfPrimitive objType = dic[DictionaryProperties.Type];
                    if (objType.GetType().Name == "PdfName")
                    {
                        PdfName type = GetObject(objType) as PdfName;

                        if (type.Value == "Page")
                        {
                            if (!dic.ContainsKey(DictionaryProperties.Kids))
                            {
                                PdfPageBase lPage = (Document as PdfLoadedDocument).Pages.GetPage(dic);

                                obj = ((IPdfWrapper)lPage).Element;
                                PdfMainObjectCollection items = Document.PdfObjects;

                                int index = items.IndexOf(dic);

                                if (index >= 0)
                                {
                                    items.ReregisterReference(index, obj);
                                    if (!m_isMerging)
                                        obj.Position = -1;
                                }
                            }
                        }
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Prepares the cross-reference stream.
        /// </summary>
        /// <param name="prevXRef">The offset to the previous cross-reference stream.</param>
        /// <param name="position">The current position.</param>
        /// <param name="reference">The reference.</param>
        /// <returns>Prepared cross-reference stream.</returns>
        private PdfStream PrepareXRefStream(long prevXRef, long position, out PdfReference reference)
        {
            PdfStream xRefStream;

            xRefStream = Trailer as PdfStream;

            if (xRefStream == null)
            {
                xRefStream = new PdfStream();
            }
            else
            {
                xRefStream.Remove(DictionaryProperties.Filter);
                xRefStream.Remove(DictionaryProperties.DecodeParms);
            }

            PdfArray sectionIndeces = new PdfArray();


            reference = new PdfReference(NextObjNumber, 0);
            RegisterObject(position, reference);

            long objectNum = 0;
            long count = 0;
            int[] paramsFormat = new int[] { 1, 8, 1 };
            paramsFormat[1] = Math.Max(GetSize((ulong)position), GetSize((ulong)Count));
            paramsFormat[2] = GetSize((ulong)m_maxGenNumIndex);

            using (MemoryStream ms = new MemoryStream(100))
            using (BinaryWriter bw = new BinaryWriter(ms))
            {

                while ((count = PrepareSubsection(ref objectNum)) > 0)
                {
                    sectionIndeces.Add(new PdfNumber(objectNum));
                    sectionIndeces.Add(new PdfNumber(count));
                    SaveSubsection(bw, objectNum, count, paramsFormat);
                    objectNum += count;
                }

                bw.Flush();
                xRefStream.Data = ms.ToArray();
            }

            xRefStream[DictionaryProperties.Index] = sectionIndeces;
            xRefStream[DictionaryProperties.Size] = new PdfNumber(Count);
            if (prevXRef != 0)
            {
                xRefStream[DictionaryProperties.Prev] = new PdfNumber(prevXRef);
            }
            xRefStream[DictionaryProperties.Type] = new PdfName("XRef");

            xRefStream[DictionaryProperties.W] = new PdfArray(paramsFormat);

            if (m_crossTable != null)
            {
                PdfDictionary trailer = m_crossTable.Trailer;

                foreach (PdfName key in trailer.Keys)
                {
                    bool contains = xRefStream.ContainsKey(key);

                    if (!contains && key.Value != DictionaryProperties.DecodeParms && key.Value != DictionaryProperties.Filter)
                    {
                        xRefStream[key] = trailer[key];
                    }
                }
            }

            ForceIDHex(xRefStream);
            xRefStream.Encrypt = false;
            return xRefStream;
        }

        /// <summary>
        /// Gets the minimal number of bytes required to save the number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>How much bytes required.</returns>
        private int GetSize(ulong number)
        {
            int size = 0;

            if (number < uint.MaxValue) // less than or equal to 4 bytes
            {
                if (number < ushort.MaxValue) // 2 bytes
                {
                    if (number < byte.MaxValue) // 1 byte
                    {
                        size = 1;
                    }
                    else
                    {
                        size = 2;
                    }
                }
                else // 3-4 bytes
                {
                    if (number < (ushort.MaxValue | ushort.MaxValue << 8))
                    {
                        size = 3;
                    }
                    else
                    {
                        size = 4;
                    }
                }
            }
            else // more than 4 bytes
            {
                size = 8;
            }

            return size;
        }

        /// <summary>
        /// Saves the subsection.
        /// </summary>
        /// <param name="xRefStream">The binary writer of cross-reference stream.</param>
        /// <param name="objectNum">The object number.</param>
        /// <param name="count">The count.</param>
        /// <param name="format">The format.</param>
        private void SaveSubsection(BinaryWriter xRefStream, long objectNum, long count, int[] format)
        {
            for (long i = objectNum; i < objectNum + count; ++i)
            {
                RegisteredObject obj = m_objects[i] as RegisteredObject;

                xRefStream.Write((byte)obj.Type);

                switch (obj.Type)
                {
                    case CrossTable.ObjectType.Free:
                        SaveLong(xRefStream, obj.ObjectNumber, format[1]);
                        SaveLong(xRefStream, (long)obj.GenerationNumber, format[2]);
                        break;

                    case CrossTable.ObjectType.Normal:
                        SaveLong(xRefStream, obj.Offset, format[1]);
                        SaveLong(xRefStream, (long)obj.GenerationNumber, format[2]);
                        break;

                    case CrossTable.ObjectType.Packed:
                        SaveLong(xRefStream, obj.ObjectNumber, format[1]);
                        SaveLong(xRefStream, obj.Offset, format[2]);
                        break;

                    default:
                        throw new PdfDocumentException("Internal error: Undefined object type.");
                }

            }
        }

        /// <summary>
        /// Saves the long.
        /// </summary>
        /// <param name="xRefStream">The xref stream.</param>
        /// <param name="number">The number.</param>
        /// <param name="count">The count of bytes.</param>
        private void SaveLong(BinaryWriter xRefStream, long number, int count)
        {
            for (int i = count - 1; i >= 0; --i)
            {
                byte b = (byte)(number >> (i << 3) & 0xff);
                xRefStream.Write(b);
            }
        }

#if !SILVERLIGHT && !WP
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Sets the security to the document.
        /// </summary>
        private void SetSecurity()
        {
            PdfSecurity security = m_document.Security;

            Trailer.Encrypt = false;

            if (security.Encryptor.Encrypt)
            {
                PdfDictionary securityDictionary = EncryptorDictionary;

                if (securityDictionary == null)
                {
                    securityDictionary = new PdfDictionary();
                    securityDictionary.Encrypt = false;
                    m_document.PdfObjects.Add(securityDictionary);
                    if (!m_isMerging)
                        securityDictionary.Position = -1;
                    PdfReferenceHolder reference = new PdfReferenceHolder(securityDictionary);
                    Trailer[DictionaryProperties.Encrypt] = reference;
                }

                security.Encryptor.SaveToDictionary(securityDictionary);

                Trailer[DictionaryProperties.ID] = security.Encryptor.FileID;
                Trailer[DictionaryProperties.Encrypt] = new PdfReferenceHolder(securityDictionary);
            }
        }
#endif

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Saves all objects in the collection.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        private void SaveObjects(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            PdfMainObjectCollection objectCollection = ObjectCollection;

            if (m_bForceNew)
            {
                Count = 1;
                m_mappedReferences = null;
            }

#if !SILVERLIGHT && !WP
            SetSecurity();
#endif
            for (int i = 0; i < objectCollection.Count; ++i)
            {
                PdfMainObjectCollection.ObjectInfo oi = objectCollection[i];
                if (oi.Modified || m_bForceNew)
                {
                    IPdfPrimitive obj = oi.Object;
                    if (this.Document is PdfDocument)
                    {
                        obj.IsSaving = true;
                    }
                    if (obj != Trailer)
                    {
                        SaveIndirectObject(obj, writer);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the archives.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void SaveArchives(PdfWriter writer)
        {
            if (m_archives != null)
            {
                foreach (ArchiveInfo ai in m_archives)
                {
                    PdfReference reference = ai.Reference;

                    if (reference == null)
                    {
                        reference = new PdfReference(NextObjNumber, 0);
                        ai.Reference = reference;
                    }

                    m_document.CurrentSavingObj = reference;
                    RegisterObject(writer.Position, reference);
                    DoSaveObject(ai.Archive, reference, writer);
                }
            }
        }

        /// <summary>
        /// Gets the mapped reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The mapped reference.</returns>
        private PdfReference GetMappedReference(PdfReference reference)
        {
            if (reference == null)
            {
                return null;
            }

            if (m_mappedReferences == null)
            {
                m_mappedReferences = new Dictionary<PdfReference, PdfReference>(100);
            }

            PdfReference mref = m_mappedReferences.ContainsKey(reference) ?
                m_mappedReferences[reference] as PdfReference : null;

            if (mref == null)
            {
                mref = new PdfReference(NextObjNumber, 0);
                m_mappedReferences[reference] = mref;
            }

            return mref;
        }

        /// <summary>
        /// Finds the archive reference.
        /// </summary>
        /// <param name="archive">The archive.</param>
        /// <returns>The reference found.</returns>
        private PdfReference FindArchiveReference(PdfArchiveStream archive)
        {
            int i = 0;
            ArchiveInfo ai = null;

            for (int count = m_archives.Count; i < count; ++i)
            {
                ai = m_archives[i] as ArchiveInfo;

                if (ai.Archive == archive)
                {
                    break;
                }
            }

            PdfReference reference = ai.Reference;

            if (reference == null)
            {
                reference = new PdfReference(NextObjNumber, 0);
            }

            ai.Reference = reference;

            return reference;
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Saves indirect object.
        /// </summary>
        /// <param name="obj">Indirect object that should be saved.</param>
        /// <param name="writer">Writer object.</param>
        internal void SaveIndirectObject(IPdfPrimitive obj, PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (obj == null)
                throw new ArgumentNullException("obj");

            /*if( obj is IPDFDisposable )
            {
                if( ( obj as IPDFDisposable ).IsDisposed ) return;
            }
            else */
#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (obj is PdfEncryptor)
            {
                if (!(obj as PdfEncryptor).Encrypt)
                {
                    return;
                }
            }
#endif

            PdfReference reference = GetReference(obj);


            if (obj is PdfCatalog)
            {
                Trailer[DictionaryProperties.Root] = reference;

#if !SILVERLIGHT && !NETFX_CORE && !WP
                //NOTE: This is needed to get PDF/A Conformance.
                if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B || PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_X1A2001)
                {
                    PdfSecurity security = m_document.Security;
                    Trailer[DictionaryProperties.ID] = security.Encryptor.FileID;
                }
#endif
            }

            // NOTE: This is needed for correct string objects encryption.
            m_document.CurrentSavingObj = reference;

            bool archive = (obj is PdfDictionary) ? (obj as PdfDictionary).Archive : true;

            bool allowedType = !((obj is PdfStream) || !archive ||
                (obj is PdfCatalog) || (obj is Pdf3DStream));

            if (allowedType && IsCrossReferenceStream(writer.Document) && reference.GenNum == 0)
            {
                DoArchiveObject(obj, reference, writer);
            }
            else
            {
                RegisterObject(writer.Position, reference);
                DoSaveObject(obj, reference, writer);
                if (obj == m_archive)
                {
                    m_archive = null;
                }
            }
        }

        /// <summary>
        /// Retrieves the reference of the given object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="writer">The writer.</param>
        private void DoArchiveObject(IPdfPrimitive obj, PdfReference reference, PdfWriter writer)
        {
            if (m_archive == null)
            {
                m_archive = new PdfArchiveStream(m_document);
                SaveArchive(writer);
            }

            long objIndex = m_archive.ObjCount;

            RegisterObject(m_archive, reference);

            m_archive.SaveObject(obj, reference);

            if (m_archive.ObjCount >= 100)
            {
                m_archive = null;
            }
        }

        /// <summary>
        /// Saves the current archive.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void SaveArchive(PdfWriter writer)
        {
            ArchiveInfo ai = new ArchiveInfo(null, m_archive);

            if (m_archives == null)
            {
                m_archives = new List<ArchiveInfo>(10);
            }

            m_archives.Add(ai);
        }

        /// <summary>
        /// Performs real saving of the save object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="writer">The writer.</param>
        private void DoSaveObject(IPdfPrimitive obj, PdfReference reference, PdfWriter writer)
        {
            long correctPosition = writer.Length;

            if (writer.GetStream().CanSeek)
            {
                if (writer.Position != correctPosition)
                {
                    writer.Position = correctPosition;
                }
            }

            writer.Write(reference.ObjNum.ToString(CultureInfo.InvariantCulture));
            writer.Write(Operators.WhiteSpace);
            writer.Write(reference.GenNum.ToString(CultureInfo.InvariantCulture));
            writer.Write(Operators.WhiteSpace);
            writer.Write(Operators.obj);
            writer.Write(Operators.NewLine);
            lock (PdfDocument.Cache)
            {
                obj.Save(writer);
            }

            if (obj is PdfName || obj is PdfNumber || obj is PdfNull)
            {
                writer.Write(Operators.NewLine);
            }
            if (writer.GetStream().CanRead)
            {
                Stream stream = writer.GetStream();
                BinaryReader br = new BinaryReader(stream);
                if (br.BaseStream.CanRead)
                {
                    br.BaseStream.Position = stream.Length - 1;
                    char c = br.ReadChar();
                    if (c != '\n')
                        writer.Write(Operators.NewLine);
                }
            }
            writer.Write(Operators.endobj);
            writer.Write(Operators.NewLine);
        }

        /// <summary>
        /// Generates the document page root dictionary object.
        /// </summary>
        /// <returns>The document page root dictionary object.</returns>
        private PdfDictionary GeneratePagesRoot()
        {
            PdfDictionary pagesRoot = null;
            IPdfPrimitive obj = DocumentCatalog[DictionaryProperties.Pages];

            if (obj == null)
                throw new PdfDocumentException(PdfMessages.InvalidFormat);

            pagesRoot = obj as PdfDictionary;

            if (pagesRoot == null)
                throw new PdfDocumentException(PdfMessages.InvalidFormat);

            return pagesRoot;
        }

        /// <summary>
        /// Saves the xref section.
        /// </summary>
        /// <param name="writer">The stream writer.</param>
        private void SaveSections(PdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            long objectNum = 0;
            long count = 0;

            do
            {
                count = PrepareSubsection(ref objectNum);
                SaveSubsection(writer, objectNum, count);
                objectNum += count;
            } while (count != 0);
        }

        /// <summary>
        /// Prepares a subsection of the current section within the cross-reference table.
        /// </summary>
        /// <param name="objectNum">The first object in the subsection.</param>
        /// <returns>The number of the entries in the section.</returns>
        private long PrepareSubsection(ref long objectNum)
        {
            long count = 0;
            long i;
            int total = Count;
            if (total <= 0)
            {
                total = Document.PdfObjects.Count + 1;
            }

            // TODO: a more sophisticated condition.
            if (objectNum >= total)
                return count;

            // search for first changed indirect object.
            for (i = objectNum; i < total; ++i)
            {
                if (m_objects.ContainsKey(i))
                {
                    break;
                }
            }

            objectNum = i;

            // look up for all indirect objects in one subsection.
            for (; i < total; ++i)
            {
                if (!m_objects.ContainsKey(i))
                {
                    break;
                }
                ++count;
            }

            return count;
        }

        /// <summary>
        /// Saves a subsection.
        /// </summary>
        /// <param name="writer">A PDF writer.</param>
        /// <param name="objectNum">The firs object in the subsection.</param>
        /// <param name="count">The number of the indirect objects in the subsection.
        /// </param>
        private void SaveSubsection(PdfWriter writer, long objectNum, long count)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (count <= 0 || objectNum >= Count)
            {
                return;
            }

            const string subsectionHead = "{0} {1}{2}";

            writer.Write(string.Format(subsectionHead, objectNum, count,
                Operators.NewLine));

            for (long i = objectNum; i < objectNum + count; ++i)
            {
                RegisteredObject obj = m_objects[i] as RegisteredObject;

                string str = GetItem(obj.Offset, obj.GenerationNumber, obj.Type == CrossTable.ObjectType.Free);
                writer.Write(str);
            }
        }

        /// <summary>
        /// Generates string for xref table item.
        /// </summary>
        /// <param name="offset">Offset of the object in the file.</param>
        /// <param name="genNumber">The generation number of the object.</param>
        /// <param name="isFree">Indicates whether object is free.</param>
        /// <returns>String representation of the item.</returns>
        internal static string GetItem(long offset, long genNumber, bool isFree)
        {
            const string offsetFormat = "0000000000 ";
            const string genNumFormat = "00000 ";

            StringBuilder builder = new StringBuilder();
            builder.Append(offset.ToString(offsetFormat));
            builder.Append(((ushort)(genNumber)).ToString(genNumFormat));
            builder.Append((isFree) ? Operators.f : Operators.n);
            builder.Append(Operators.NewLine);

            return builder.ToString();
        }

        /// <summary>
        /// Saves the new trailer dictionary.
        /// </summary>
        /// <param name="writer">A PDF writer.</param>
        /// <param name="count">The total number of the objects.</param>
        /// <param name="prevXRef">The PrevXRef value.</param>
        private void SaveTrailer(PdfWriter writer, long count, long prevXRef)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(Operators.trailer + Operators.NewLine);
            // Save the dictionary.

            PdfDictionary trailer = Trailer;

            if (prevXRef != 0)
            {
                trailer[DictionaryProperties.Prev] = new PdfNumber(prevXRef);
            }

            ForceIDHex(trailer);
            trailer[DictionaryProperties.Size] = new PdfNumber(m_count);
            trailer = new PdfDictionary(trailer); // Make it real dictionary.
            trailer.Encrypt = false;
            trailer.Save(writer);
        }

        /// <summary>
        /// Forces the ID to be in hex.
        /// </summary>
        /// <param name="trailer">The trailer.</param>
        private void ForceIDHex(PdfDictionary trailer)
        {
            PdfArray id = Dereference(trailer[DictionaryProperties.ID]) as PdfArray;

            if (id != null)
            {
                foreach (PdfString idPart in id)
                {
                    idPart.Encode = PdfString.ForceEncoding.ASCII;
                    idPart.ToHex();
                }
            }
        }

        /// <summary>
        /// Saves the endess of the file.
        /// </summary>
        /// <param name="writer">A PDF writer.</param>
        /// <param name="xrefPos">The xref position.</param>
        private void SaveTheEndess(PdfWriter writer, long xrefPos)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(Operators.NewLine + Operators.startxref + Operators.NewLine);
            writer.Write(xrefPos.ToString() + Operators.NewLine);
            writer.Write(Operators.EOF + Operators.NewLine);
        }

        /// <summary>
        /// Saves the head.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void SaveHead(PdfWriter writer)
        {
            byte[] binData = { 0x25, 0x83, 0x92, 0xfa, 0xfe };
            writer.Write("%PDF-");

            string version = GenerateFileVersion(writer.Document);
            writer.Write(version);
            writer.Write(Operators.NewLine);

            writer.Write(binData);
            writer.Write(Operators.NewLine);
        }

        /// <summary>
        /// Generates the version of the file.
        /// </summary>
        /// <param name="document">the parent document.</param>
        /// <returns>The version of the file.</returns>
        private string GenerateFileVersion(PdfDocumentBase document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            int iVersion = (int)document.FileStructure.Version;

            string version = "1." + iVersion.ToString();
            return version;
        }

        /// <summary>
        /// Checks the cross-reference type in the document.
        /// </summary>
        /// <param name="document">The parent document.</param>
        /// <returns>True if cross-reference is a stream, False otherwise.</returns>
        private bool IsCrossReferenceStream(PdfDocumentBase document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            bool result = false;

            if (m_crossTable != null)
            {
                result = m_crossTable.Trailer is PdfStream;
            }
            else
            {
                result = (document.FileStructure.CrossReferenceType ==
                    PdfCrossReferenceType.CrossReferenceStream);
            }

            return result;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        internal void Close(bool completely)
        {
            if (completely)
            {
                if (m_archives != null)
                {
                    m_archives.Clear();
                    m_archives = null;
                }

                if (m_archive != null)
                {
                    m_archive.Clear();
                    m_archive = null;
                }

                if (m_items != null && m_items.Count > 0 && completely)
                {
                    for (int i = m_items.Count - 1; i >= 0; i--)
                    {
                        Syncfusion.Pdf.IO.PdfMainObjectCollection.ObjectInfo oi = m_items[i];
                        m_items.Remove(i);
                        if (oi.Object is PdfStream)
                            (oi.Object as PdfStream).Clear();
                        else if (oi.Object is PdfCatalog)
                            (oi.Object as PdfCatalog).Clear();
                        //else if (oi.Object is PdfDictionary)//This corrupts consecutive document's font.
                        //    (oi.Object as PdfDictionary).Clear();
                        else if (oi.Object is PdfArray)
                            (oi.Object as PdfArray).Clear();

                        oi = null;
                    }
                }

                m_preReference = null;

                if (m_pageCorrespondance != null)
                {
                    m_pageCorrespondance.Clear();
                    m_pageCorrespondance = null;
                }
            }
            Dispose();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="completely"><c>true</c> to release both managed and
        /// unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public void Dispose(bool completely)
        {
            if (!m_bDisposed)
            {
                if (m_stream != null)
                {
                    m_stream.Dispose();
                    m_stream = null;
                }

                if (m_objects != null)
                {
                    m_objects.Clear();
                    m_objects = null;
                }

                m_crossTable = null;
                m_documentCatalog = null;
                m_trailer = null;
                m_document = null;
                m_bDisposed = true;
                m_items = null;
            }
        }
        #endregion

        #region Internals
        /// <summary>
        /// Represents a registered object.
        /// </summary>
        public class RegisteredObject
        {
            #region Fields
            /// <summary>
            /// The object number of the indirect object.
            /// </summary>
            private long m_objectNumber;
            /// <summary>
            /// The generation number of the indirect object.
            /// </summary>
            public int GenerationNumber;
            /// <summary>
            /// The offset of the indirect object within the file.
            /// </summary>
            private long m_offset;
            /// <summary>
            /// Archive.
            /// </summary>
            private PdfArchiveStream m_archive;
            /// <summary>
            /// Shows if the object is free.
            /// </summary>
            public CrossTable.ObjectType Type;
            /// <summary>
            /// Holds the current cross-reference table.
            /// </summary>
            private PdfCrossTable m_xrefTable;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the object number.
            /// </summary>
            internal long ObjectNumber
            {
                get
                {

                    if (m_objectNumber == 0)
                    {
                        if (m_archive != null)
                        {
                            m_objectNumber = m_xrefTable.GetReference(m_archive).ObjNum;
                        }
                    }

                    return m_objectNumber;
                }
            }
            /// <summary>
            /// Gets the offset.
            /// </summary>
            internal long Offset
            {
                get
                {
                    long result;

                    if (m_archive != null)
                    {
                        result = m_archive.GetIndex(m_offset);
                    }
                    else
                    {
                        result = m_offset;
                    }

                    return result;
                }
            }
            #endregion

            #region Constructors
            /// <summary>
            /// Initialize the structure with the proper values.
            /// </summary>
            /// <param name="offset">The offset of the object.</param>
            /// <param name="reference">The reference representing the object number and
            /// the generation number of the indirect object.</param>
            public RegisteredObject(long offset, PdfReference reference)
            {
                if (reference == null)
                    throw new ArgumentNullException("reference");

                m_offset = offset;
                GenerationNumber = reference.GenNum;
                m_objectNumber = reference.ObjNum;
                Type = CrossTable.ObjectType.Normal;
            }
            /// <summary>
            /// Initialize the structure with the proper values.
            /// </summary>
            /// <param name="offset">The offset of the object.</param>
            /// <param name="reference">The reference representing the object number and
            /// the generation number of the indirect object.</param>
            /// <param name="free">Shows if the object is free.</param>
            public RegisteredObject(long offset, PdfReference reference, bool free)
                : this(offset, reference)
            {
                if (reference == null)
                    throw new ArgumentNullException("reference");

                Type = (free) ? CrossTable.ObjectType.Free : CrossTable.ObjectType.Normal;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="T:RegisteredObject"/> class.
            /// </summary>
            /// <param name="xrefTable">The xref table.</param>
            /// <param name="archive">The archive.</param>
            /// <param name="reference">The reference.</param>
            public RegisteredObject(PdfCrossTable xrefTable, PdfArchiveStream archive, PdfReference reference)
            {
                m_xrefTable = xrefTable;
                m_archive = archive;
                m_offset = reference.ObjNum;
                Type = CrossTable.ObjectType.Packed;
            }
            #endregion
        }

        /// <summary>
        /// Stores information about an archive.
        /// </summary>
        internal class ArchiveInfo
        {
            #region Fields
            /// <summary>
            /// The object number of the archive.
            /// </summary>
            public PdfReference Reference;
            /// <summary>
            /// The archive stream.
            /// </summary>
            public PdfArchiveStream Archive;
            #endregion

            #region Initialize / Finalize
            /// <summary>
            /// Initializes a new instance of the <see cref="ArchiveInfo"/> class.
            /// </summary>
            /// <param name="reference">The reference.</param>
            /// <param name="archive">The archive.</param>
            public ArchiveInfo(PdfReference reference, PdfArchiveStream archive)
            {
                Reference = reference;
                Archive = archive;
            }
            #endregion
        }
        #endregion
    }
}
