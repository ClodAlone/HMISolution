#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Implements routines for manipulation with loaded pages.
    /// </summary>
    public class PdfLoadedPageCollection : IEnumerable
    {
        #region Members
        private PdfDocumentBase m_document;
        private PdfCrossTable m_crossTable;
        private Dictionary<PdfDictionary, PdfPageBase> m_pagesCash;

        private PdfLoadedDocument m_loadedDocument;
        private int m_pageDuplicaton;
        internal static int m_repeatIndex;
        internal static int m_parentKidsCount;
        internal static int m_parentKidsCounttemp;
        internal static int m_nestedPages;
        private int m_sectionCount;
        #endregion

        #region Properties

        /// <summary>
        /// Get the Section Count.
        /// </summary>
        public int SectionCount
        {
            get
            {
                IPdfPrimitive obj = m_document.Catalog[DictionaryProperties.Pages];
                PdfDictionary node = m_crossTable.GetObject(obj) as PdfDictionary;
                PdfArray m_sectionarray = node[DictionaryProperties.Kids] as PdfArray;
                int count = m_sectionarray.Count;
                return count;
            }
        }
        /// <summary>
        ///  Get and set the Pdfloaded Document.
        /// </summary>
        private PdfLoadedDocument LoadedDocument
        {
            get
            {
                return m_loadedDocument;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:PdfPageBase"/> at the specified index.
        /// </summary>
        public PdfPageBase this[int index]
        {
            get
            {
                PdfPageBase page = GetPage(index);

                return page;
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                IPdfPrimitive obj = m_document.Catalog[DictionaryProperties.Pages];
                PdfDictionary node = m_crossTable.GetObject(obj) as PdfDictionary;
                int count = GetNodeCount(node);
                return count;
            }
        }

        /// <summary>
        /// Gets the page cache.
        /// </summary>
        private Dictionary<PdfDictionary, PdfPageBase> PageCache
        {
            get
            {
                if (m_pagesCash == null)
                {
                    m_pagesCash = new Dictionary<PdfDictionary, PdfPageBase>();
                }

                return m_pagesCash;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLoadedPageCollection"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedPageCollection(PdfDocumentBase document, PdfCrossTable crossTable)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (crossTable == null)
                throw new ArgumentNullException("crossTable");

            m_document = document;

            m_crossTable = crossTable;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new page and adds it to the collection.
        /// </summary>
        /// <returns>The created page.</returns>
        public PdfPageBase Add()
        {
            return Insert(Count);
        }

        /// <summary>
        /// Creates a new page of the specified size and adds it to the collection.
        /// </summary>
        /// <param name="size">The size of the new page.</param>
        /// <returns>The created page.</returns>
        public PdfPageBase Add(SizeF size)
        {
            return Insert(Count, size);
        }

        /// <summary>
        /// Creates a new page of the specified size and with the specified margins
        /// and adds it to the collection.
        /// </summary>
        /// <param name="size">The size of the new page.</param>
        /// <param name="margins">The margins of the new page.</param>
        /// <returns>The created page.</returns>
        public PdfPageBase Add(SizeF size, PdfMargins margins)
        {
            return Insert(Count, size, margins);
        }

        /// <summary>
        /// Creates a new page of the specified size and with the specified margins
        /// and adds it to the collection.
        /// </summary>
        /// <param name="size">The size of the new page.</param>
        /// <param name="margins">The margins of the new page.</param>
        /// <param name="rotation">The rotation of the new page.</param>
        /// <returns>The created page.</returns>		
        public PdfPageBase Add(SizeF size, PdfMargins margins, PdfPageRotateAngle rotation)
        {
            return Insert(Count, size, margins, rotation);
        }

        /// <summary>
        /// Creates a new page of the specified size and with the specified margins
        /// and adds it to the collection.
        /// </summary>
        /// <param name="size">The size of the new page.</param>
        /// <param name="margins">The margins of the new page.</param>
        /// <param name="rotation">The rotation of the new page.</param>
        /// <param name="loc">The location of the new page.</param>
        /// <returns>The created page.</returns>		
        internal PdfPageBase Add(SizeF size, PdfMargins margins, PdfPageRotateAngle rotation, int location)
        {
            return Insert(location, size, margins, rotation);
        }

        /// <summary>
        /// Adds a cloned page from a loaded document.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="page">The page.</param>
        /// <param name="destinations">The destinations.</param>
        /// <returns></returns>
        internal PdfPageBase Add(PdfLoadedDocument ldDoc, PdfPageBase page, List<PdfArray> destinations)
        {
            if (ldDoc == null)
                throw new ArgumentNullException("ldDoc");

            if (page == null)
                throw new ArgumentNullException("page");

            // Obtain the concatenated content streams as XObject.
            PdfTemplate xobject = page.ContentTemplate;

            // Create a new page and draw the XObject there.
            PdfPage newPage = Add(page.Size, new PdfMargins(), page.Rotation) as PdfPage;

            if (xobject != null)
            {
                newPage.Graphics.DrawPdfTemplate(xobject, PointF.Empty);
            }

            // Copy annotation dictionaries to the new page.
            newPage.ImportAnnotations(ldDoc, page, destinations);

            return newPage;
        }

        /// <summary>
        /// Adds a cloned page from a loaded document.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        internal PdfPageBase Add(PdfLoadedDocument ldDoc, PdfPageBase page)
        {
            if (ldDoc == null)
                throw new ArgumentNullException("ldDoc");

            if (page == null)
                throw new ArgumentNullException("page");

            // Obtain the concatenated content streams as XObject.
            PdfTemplate xobject = page.GetContent();

            // Create a new page and draw the XObject there.
            PdfPage newPage = Add(page.Size, new PdfMargins(), page.Rotation) as PdfPage;

            if (xobject != null)
            {
                newPage.Graphics.DrawPdfTemplate(xobject, PointF.Empty);
            }

            // Copy annotation dictionaries to the new page.
            if (newPage.Document != null && !newPage.Document.EnableMemoryOptimization)
                newPage.ImportAnnotations(ldDoc, page);

            return newPage;
        }

        /// <summary>
        /// Creates a new page and inserts it at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The created page.</returns>
        public PdfPageBase Insert(int index)
        {
            return Insert(index, SizeF.Empty);
        }

        /// <summary>
        /// Creates a new page and inserts it at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="size">The size of the page.</param>
        /// <returns>The created page.</returns>
        public PdfPageBase Insert(int index, SizeF size)
        {
            return Insert(index, size, null);
        }

        /// <summary>
        /// Creates a new page and inserts it at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="size">The size of the page.</param>
        /// <param name="margins">The margins of the page.</param>
        /// <returns>The created page.</returns>
        public PdfPageBase Insert(int index, SizeF size, PdfMargins margins)
        {
            PdfPageRotateAngle rotation = PdfPageRotateAngle.RotateAngle0;

            return Insert(index, size, margins, rotation);
        }

        /// <summary>
        /// Creates a new page and inserts it at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="size">The size of the page.</param>
        /// <param name="margins">The margins of the page.</param>
        /// <param name="rotation">The rotation of the new page.</param>
        /// <returns>The created page.</returns>
        public PdfPageBase Insert(int index, SizeF size, PdfMargins margins, PdfPageRotateAngle rotation)
        {
            PdfPageOrientation po = (size.Width > size.Height) ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;

            return Insert(index, size, margins, rotation, po);
        }

        /// <summary>
        /// Removes the page at the given specified index.
        /// </summary>
        /// <param name="index"> Index of the page.</param>
        public void RemoveAt(int index)
        {
            PdfPageBase page = GetPage(index);
            Remove(page);
        }

        /// <summary>
        /// Removes the specified page.
        /// </summary>
        /// <param name="page">The page to be remove.</param>
        public void Remove(PdfPageBase page)
        {
            int pageIndex = IndexOf(page);
            int localIndex;

            if (pageIndex > -1)
            {
                // Remove BookMarks    
                PdfLoadedDocument doc = m_document as PdfLoadedDocument;
                Dictionary<PdfPageBase, object> pageToBookmarkDic = doc.CreateBookmarkDestinationDictionary();
                bool bookmarkPresent = (pageToBookmarkDic != null);

                if (bookmarkPresent)
                {
                    List<object> bookmarks = null;

                    if (pageToBookmarkDic.ContainsKey(page))
                        bookmarks = pageToBookmarkDic[page] as List<object>;

                    if (bookmarks != null)
                    {
                        for (int i = 0; i < bookmarks.Count; i++)
                        {
                            PdfBookmarkBase current = bookmarks[i] as PdfBookmarkBase;
                            PdfDestination m_destination = null;
                            PdfDictionary bookmarkDic = current.Dictionary;
                            if (bookmarkDic[DictionaryProperties.A] != null)
                            {
                                current.Dictionary.SetProperty(DictionaryProperties.A, m_destination);
                            }
                            current.Dictionary.SetProperty(DictionaryProperties.Dest, m_destination);
                        }

                    }
                    //doc.Bookmarks.ReproduceTree();
                }

                PdfDictionary dic = (page as IPdfWrapper).Element as PdfDictionary;
                PdfDictionary parent = GetParent(pageIndex, out localIndex, true);
                dic[DictionaryProperties.Parent] = new PdfReferenceHolder(parent);
                PdfArray kids = GetNodeKids(parent);

                //Removing Pages
                if (pageIndex == 0)
                {
                    PdfCrossTable table = m_document.CrossTable;
                    if (table.DocumentCatalog != null)
                    {
                        PdfArray documentCatalog = table.DocumentCatalog["OpenAction"] as PdfArray;
                        if (documentCatalog != null)
                            documentCatalog.Remove(new PdfReferenceHolder(dic));
                        else
                        {
                            PdfReferenceHolder documentHolder = table.DocumentCatalog["OpenAction"] as PdfReferenceHolder;
                            if (documentHolder != null)
                            {
                                PdfDictionary documentDic = documentHolder.Object as PdfDictionary;
                                documentCatalog = documentDic["D"] as PdfArray;
                                if (documentCatalog != null)
                                    documentCatalog.Remove(new PdfReferenceHolder(dic));
                            }
                        }
                    }
                }
                PdfReferenceHolder remove = null;
                foreach (PdfReferenceHolder holder in kids)
                {
                    if (holder.Object == dic)
                    {
                        remove = holder;
                        break;
                    }
                }
                if (remove != null)
                    kids.Remove(remove);
                UpdateCountDecrement(parent);
            }
        }


        /// <summary>
        /// ReArrange the Pages in the Loaded Document.
        /// </summary>
        /// <param name="orderArray">The page sequence to arrange the pages.</param>
        public void ReArrange(int[] orderArray)
        {
            int[] repeatcheck = new int[orderArray.Length];
            int[] repeatcondition = new int[orderArray.Length];
            int[] repeat = new int[orderArray.Length];
            int k = 0;
            int orderArraySize = orderArray.Length;
            int orderArrayLength = orderArraySize;
            int m_parentKidsSize = Count;
            int startRepeat = 0;
            int duplicateCount;
            int duplicateDelete = m_parentKidsSize;
            int m_tempParentKidsSize = m_parentKidsSize;
            int repeatcount = 0;

            for (int index = 0; index < orderArray.Length; index++)
            {
                if (orderArray[index] >= Count)
                {
                    throw new ArgumentException("The page Index is not Valid");
                }
            }

            IPdfPrimitive parentobj = m_document.Catalog[DictionaryProperties.Pages];
            PdfDictionary dictionary = m_crossTable.GetObject(parentobj) as PdfDictionary;
            PdfArray kidsArray = dictionary[DictionaryProperties.Kids] as PdfArray;
            int size = kidsArray.Count;

            m_loadedDocument = m_document as PdfLoadedDocument;

            m_parentKidsCount = Count;

            //To check the duplication is occured or not.
            for (int x = 0; x < orderArraySize; x++)
            {
                for (int y = x + 1; y < orderArraySize; y++)
                {
                    if (orderArray[x] == orderArray[y])
                    {
                        m_pageDuplicaton = 1;
                        if (repeatcheck[y] == 0)
                        {
                            repeatcount += 1;
                            repeatcheck[y] = 1;
                            repeatcondition[k] = y;
                            k++;
                        }
                    }
                }
            }

            //If Duplication is occured .
            if (m_pageDuplicaton == 1)
            {
                //To find Index of the first duplication.
                for (int x = 0; x < repeatcondition.Length; x++)
                {
                    for (int y = x + 1; y < repeatcondition.Length; y++)
                    {
                        if (repeatcondition[y] != 0)
                        {
                            if (repeatcondition[x] > repeatcondition[y])
                            {
                                int temp = repeatcondition[x];
                                repeatcondition[x] = repeatcondition[y];
                                repeatcondition[y] = temp;
                            }
                        }
                    }
                }

                startRepeat = repeatcondition[0];
                m_repeatIndex = startRepeat;
                duplicateCount = repeatcount;

                //Add the n number of Duplicated  pages if m_order size > page kids array size.
                if (orderArraySize > m_parentKidsSize)
                {
                    int pagedif = orderArraySize - m_parentKidsSize;
                    int repeatIndex = startRepeat;
                    for (int i = 0; i < duplicateCount; i++)
                    {
                        int tempcount = Count;
                        PdfPageBase tempPage = GetPage(orderArray[repeatIndex]);
                        m_loadedDocument.Pages.Add(m_loadedDocument, tempPage);
                        repeat[repeatIndex] = 1;
                        repeatIndex += 1;
                    }
                }

                //Add the n number of Duplicated  pages if m_order size < page kids array size.
                else
                {
                    int index = startRepeat;
                    for (int i = 0; i < duplicateCount; i++)
                    {
                        int tempcount = Count;
                        PdfPageBase tempPage = GetPage(orderArray[index]);
                        m_loadedDocument.Pages.Add(m_loadedDocument, tempPage);
                        repeat[index] = 1;
                        index += 1;
                        int temp = Count;
                    }
                }

                //Add the Remaining pages after the page duplication.
                for (int i = startRepeat; i < orderArrayLength; i++)
                {
                    if (repeat[i] == 0)
                    {
                        int tempcount = Count;
                        PdfPageBase tempPage = GetPage(orderArray[i]);
                        m_loadedDocument.Pages.Add(m_loadedDocument, tempPage);
                    }
                }

            }

            int kidsLength;
            int localIndex;
            int pageDiffrence;

            PdfReference referenceObjectNumber = null;
            List<long> kidsReference = new List<long>();

            PdfDictionary parent = GetParent(0, out localIndex, true);
            PdfArray kids = GetNodeKids(parent);
            m_parentKidsCounttemp = kids.Count;
            kidsLength = kids.Count;
            for (int i = 0; i < kids.Count; i++)
            {
                referenceObjectNumber = (kids[i] as PdfReferenceHolder).Reference as PdfReference;
                kidsReference.Add(referenceObjectNumber.ObjNum);
            }
            for (int lc = 0; kidsLength < Count; lc++)
            {
                m_nestedPages = 1;
                PdfDictionary parent2 = GetParent(kidsLength, out localIndex, true);
                PdfArray kidsArrayCollection = parent2[DictionaryProperties.Kids] as PdfArray;
                for (int klen = 0; klen < GetNodeKids(parent2).Count; klen++)
                {
                    PdfDictionary kidsDictionary = (kidsArrayCollection[klen] as PdfReferenceHolder).Object as PdfDictionary;
                    referenceObjectNumber = (kidsArrayCollection[klen] as PdfReferenceHolder).Reference as PdfReference;
                    if (kidsDictionary[DictionaryProperties.Type].ToString() == "/Pages")
                    {
                        PdfArray nodeKids = GetNodeKids(kidsDictionary);
                        for (int j = 0; j < nodeKids.Count; j++)
                        {
                            referenceObjectNumber = (nodeKids[j] as PdfReferenceHolder).Reference as PdfReference;
                            if (!kidsReference.Contains(referenceObjectNumber.ObjNum))
                            {
                                kidsReference.Add(referenceObjectNumber.ObjNum);
                                kids.Insert(kidsLength, GetNodeKids(parent2)[klen]);
                                kidsLength++;
                            }
                        }
                    }

                    if (kidsDictionary[DictionaryProperties.Type].ToString() == "/Page")
                    {
                        if (!kidsReference.Contains(referenceObjectNumber.ObjNum))
                        {
                            kidsReference.Add(referenceObjectNumber.ObjNum);
                            kids.Insert(kidsLength, GetNodeKids(parent2)[klen]);
                            kidsLength++;
                        }
                    }
                }
            }

            m_parentKidsCounttemp = kids.Count;
            kids.ReArrange(orderArray);
            pageDiffrence = kidsLength - orderArray.Length;

            //Decrement the pages count .
            if (pageDiffrence != 0)
            {
                for (int i = 0; i < pageDiffrence; i++)
                {
                    UpdateCountDecrement(parent);
                }
            }

            if (m_nestedPages == 1)
            {
                PdfReferenceHolder tempreffirst = kidsArray[0] as PdfReferenceHolder;
                PdfReference reffirst = tempreffirst.Reference;
                long first = reffirst.ObjNum;

                int[] kidsDelete = new int[kidsLength];
                int[] parentIndex = new int[kidsLength];
                long[] kidsLocation = new long[kidsLength];
                PdfReferenceHolder[] parentkidstemp = new PdfReferenceHolder[kidsLength];
                long parentLocation = 0;
                int location = 0;
                int parentlocation = 0;
                int tempKidsLength = kidsLength;
                PdfArray tempKids = parent[DictionaryProperties.Kids] as PdfArray;
                PdfReferenceHolder[] tempKidsReferenceHolder = new PdfReferenceHolder[tempKids.Count];
                PdfReferenceHolder[] kidsReferenceHolder = new PdfReferenceHolder[tempKids.Count];
                
                PdfDictionary parentDictionary = tempreffirst.Object as PdfDictionary;
                PdfReferenceHolder parentReferenceHolder = parentDictionary[DictionaryProperties.Parent] as PdfReferenceHolder;

                PdfArray KidsCollection = new PdfArray();
                for (int l = 0; l < tempKids.Count; l++)
                {
                    PdfArray cBox = new PdfArray();
                    PdfArray mBox = new PdfArray();
                    tempKidsReferenceHolder[l] = tempKids[l] as PdfReferenceHolder;
                    PdfDictionary tstdid = tempKidsReferenceHolder[l].Object as PdfDictionary;
                    PdfDictionary parentPageDictionary = (tstdid[DictionaryProperties.Parent] as PdfReferenceHolder).Object as PdfDictionary;
                  
                    if (tstdid[DictionaryProperties.Type].ToString() == "/Pages")
                    {
                        if (tstdid.ContainsKey(DictionaryProperties.CropBox))
                        {
                            cBox = tstdid[DictionaryProperties.CropBox] as PdfArray;
                            tstdid.SetProperty(DictionaryProperties.CropBox, cBox);
                        }
                        if (tstdid.ContainsKey(DictionaryProperties.MediaBox))
                        {
                            mBox = tstdid[DictionaryProperties.MediaBox] as PdfArray;
                            tstdid.SetProperty(DictionaryProperties.MediaBox, mBox);
                        }
                    }
                    else
                    {
                        if (parentPageDictionary.ContainsKey(DictionaryProperties.CropBox))
                        {
                            cBox = parentPageDictionary[DictionaryProperties.CropBox] as PdfArray;
                            tstdid.SetProperty(DictionaryProperties.CropBox, cBox);
                        }
                        if (parentPageDictionary.ContainsKey(DictionaryProperties.MediaBox))
                        {
                            mBox = parentPageDictionary[DictionaryProperties.MediaBox] as PdfArray;
                            tstdid.SetProperty(DictionaryProperties.MediaBox, mBox);
                        }
                    }
                    tstdid.SetProperty(DictionaryProperties.Parent, parentReferenceHolder);
                    kidsReferenceHolder[l] = tstdid[DictionaryProperties.Parent] as PdfReferenceHolder;                    
                    KidsCollection.Add(tempKidsReferenceHolder[l]);
                }                              

                m_parentKidsCounttemp = KidsCollection.Count;
                dictionary.SetProperty(DictionaryProperties.Kids, KidsCollection);
                dictionary.SetNumber(DictionaryProperties.Count, orderArray.Length);

                if (KidsCollection.Count == 0)
                {
                    parent.SetNumber(DictionaryProperties.Count, 0);
                }

            }
        }


        #region ok
        //public void Remap(int n1, int n2, int n3)
        //{
        //    PdfPageBase page = GetPage(0);
        //    int pageIndex = IndexOf(page);
        //    int localIndex;

        //    PdfDictionary dic = (page as IPdfWrapper).Element as PdfDictionary;
        //    PdfDictionary parent = GetParent(pageIndex, out localIndex, true);
        //    dic[DictionaryProperties.Parent] = new PdfReferenceHolder(parent);
        //    PdfArray kids = GetNodeKids(parent);
        //    kids.interchange(n1, n2, n3);
        //}
        #endregion



        private void UpdateCountDecrement(PdfDictionary parent)
        {
            while (parent != null)
            {
                int count = GetNodeCount(parent) - 1;
                if (count == 0)
                {
                    PdfDictionary node = parent;
                    PdfDictionary parent1 = PdfCrossTable.Dereference(parent[DictionaryProperties.Parent]) as PdfDictionary;
                    if (parent1 != null)
                    {
                        PdfArray kids = parent1[DictionaryProperties.Kids] as PdfArray;
                        if (kids != null)
                        {
                            kids.Remove(new PdfReferenceHolder(node));
                        }
                    }
                }
                count = GetNodeCount(parent) - 1;
                parent.SetNumber(DictionaryProperties.Count, count);
                parent = PdfCrossTable.Dereference(parent[DictionaryProperties.Parent]) as PdfDictionary;
            }
        }

        /// <summary>
        /// Creates a new page and inserts it at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="size">The size of the page.</param>
        /// <param name="margins">The margins of the page.</param>
        /// <param name="rotation">The rotation of the new page.</param>
        /// <param name="orientation">The orientation of the new page.</param>
        /// <returns>The created page.</returns>
        public PdfPageBase Insert(int index, SizeF size, PdfMargins margins, PdfPageRotateAngle rotation, PdfPageOrientation orientation)
        {
            if (size == SizeF.Empty)
            {
                size = PdfPageSize.A4;
            }

            PdfPage page = new PdfPage();
            PdfPageSettings settings = new PdfPageSettings(size, orientation, 0);

            settings.Size = size;

            if (margins == null)
            {
                margins = new PdfMargins();
                margins.All = PdfDocument.DefaultMargin;
            }

            settings.Margins = margins;
            settings.Rotate = rotation;

            PdfSection sec = new PdfSection(m_document, settings);

            sec.DropCropBox();

            sec.Add(page);

            // TODO: Assign a proper parent.
            PdfDictionary dic = (sec as IPdfWrapper).Element as PdfDictionary;

            int localIndex;

            PdfDictionary parent = GetParent(index, out localIndex, false);

            dic[DictionaryProperties.Parent] = new PdfReferenceHolder(parent);

            PdfArray kids = GetNodeKids(parent);


            kids.Insert(localIndex, new PdfReferenceHolder(dic));
            UpdateCount(parent);

            dic = (page as IPdfWrapper).Element as PdfDictionary;
            PageCache[dic] = page;
            page.Graphics.ColorSpace = (m_document as PdfLoadedDocument).ColorSpace;
            page.Graphics.Layer.Colorspace = (m_document as PdfLoadedDocument).ColorSpace;
            return page;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="dic">The page dictionary.</param>
        /// <returns>The loaded page.</returns>
        internal PdfPageBase GetPage(PdfDictionary dic)
        {
            Dictionary<PdfDictionary, PdfPageBase> pageCach = PageCache;
            PdfPageBase page = null;

            if (pageCach.ContainsKey(dic))
                page = pageCach[dic] as PdfPageBase;

            if (page == null)
            {
                page = new PdfLoadedPage(m_document, m_crossTable, dic);
                pageCach[dic] = page;
            }

            return page;
        }

        /// <summary>
        /// Updates number of leaf nodes of corresponding page tree nodes starting with <see cref="parent"/>.
        /// </summary>
        /// <param name="parent">The parent dictionary </param>
        internal void UpdateCount(PdfDictionary parent)
        {
            while (parent != null)
            {
                int count = GetNodeCount(parent) + 1;
                parent.SetNumber(DictionaryProperties.Count, count);
                parent = PdfCrossTable.Dereference(parent[DictionaryProperties.Parent]) as PdfDictionary;
            }
        }

        /// <summary>
        /// Returns the index of the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The index of the page.</returns>
        /// <remarks>This might be a time consuming operation.</remarks>
        internal int IndexOf(PdfPageBase page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            int index = -1;

            for (int i = 0, count = Count; i < count; ++i)
            {
                PdfPageBase p = GetPage(i);

                if (p == page)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the page by its index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The proper PdfPageBase instance.</returns>
        private PdfPageBase GetPage(int index)
        {
            // Search for the correct leaf.
            int localIndex;

            PdfDictionary node = GetParent(index, out localIndex, true);
            PdfArray kids = GetNodeKids(node);
            int i = localIndex, j = 0;
            while (true)
            {
                node = m_crossTable.GetObject(kids[localIndex]) as PdfDictionary;
                string typeValue = (node[DictionaryProperties.Type] as PdfName).Value;
                if(typeValue=="Pages")
                {
                    i++;
                    node = m_crossTable.GetObject(kids[i]) as PdfDictionary;
                    PdfArray innerKids = GetNodeKids(node);
					if (innerKids == null) break;
                    if (innerKids.Count > 0)
                    {
                        node = m_crossTable.GetObject(innerKids[j]) as PdfDictionary;
                        j++;
                        break;
                    }
                }
                else
                    break;
            }
            PdfPageBase page = GetPage(node);

            return page;
        }

        /// <summary>
        /// Determines whether a node is a leaf nide.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// 	<c>true</c> if the specified node is a leaf node; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNodeLeaf(PdfDictionary node)
        {
            int count = GetNodeCount(node);

            return (count == 0);
        }

        /// <summary>
        /// Gets the node kids.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The node kids array.</returns>
        private PdfArray GetNodeKids(PdfDictionary node)
        {
            IPdfPrimitive obj = node[DictionaryProperties.Kids];
            PdfArray kids = m_crossTable.GetObject(obj) as PdfArray;

            return kids;
        }

        /// <summary>
        /// Gets the node count.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The number of the kids in the node.</returns>
        private int GetNodeCount(PdfDictionary node)
        {
            IPdfPrimitive obj = node[DictionaryProperties.Count];
            PdfNumber number = m_crossTable.GetObject(obj) as PdfNumber;
            int count = (number == null) ? 0 : number.IntValue;

            return count;
        }

        /// <summary>
        /// Gets the parent parent node.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="localIndex">Index of the local.</param>
        /// <param name="zeroValid">if set to <c>true</c> zero index is a valid index.</param>
        /// <returns></returns>
        private PdfDictionary GetParent(int index, out int localIndex, bool zeroValid)
        {
            if (index < 0 && index > Count)
                throw new ArgumentOutOfRangeException("index",
                    "The index should be within this range: [0; Count]");

            IPdfPrimitive obj = m_document.Catalog[DictionaryProperties.Pages];

            //PdfReferenceHolder r = new PdfReferenceHolder(obj) as PdfReferenceHolder;

            PdfDictionary node = m_crossTable.GetObject(obj) as PdfDictionary;
            int lowIndex = 0;

            localIndex = GetNodeCount(node);

            if (index == 0 && !zeroValid)
            {
                localIndex = 0;
            }
            else if (index < Count)
            {
                PdfArray kids = GetNodeKids(node);

                for (int i = 0, count = kids.Count; i < count; ++i)
                {
                    PdfDictionary subNode = m_crossTable.GetObject(kids[i]) as PdfDictionary;

                    if (IsNodeLeaf(subNode))
                    {
                        if ((lowIndex + i) == index)
                        {
                            //page = GetPage( subNode );
                            localIndex = i;
                            break;
                        }
                    }
                    else
                    {
                        int nodeCount = GetNodeCount(subNode);

                        if (index < lowIndex + nodeCount + i)
                        {
                            lowIndex += i;
                            node = subNode;
                            kids = GetNodeKids(node);
                            i = -1;
                            count = kids.Count;
                            continue;
                        }
                        else // Prevent the page to be added out of array limits.
                        {
                            lowIndex += nodeCount - 1;
                        }
                    }
                }
            }
            else
            {
                localIndex = GetNodeKids(node).Count;
            }

            return node;
        }

        /// <summary>
        /// Clears page cache.
        /// </summary>
        internal void Clear()
        {
            if (m_pagesCash != null)
            {
                IEnumerator pageCash = m_pagesCash.Keys.GetEnumerator();

                while (pageCash.MoveNext())
                {
                    PdfDictionary dict = pageCash.Current as PdfDictionary;
                    Remove(m_pagesCash[dict]);
                    dict.Clear();
                    m_pagesCash.Remove(dict);
                    pageCash = m_pagesCash.Keys.GetEnumerator();
                }
                m_pagesCash.Clear();
                pageCash = null;
            }
            
            m_crossTable = null;
            m_document = null;
            m_loadedDocument = null;
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
            PdfLoadedPageEnumerator enumer = new PdfLoadedPageEnumerator(this);

            return enumer;
        }
        #endregion
    }
}
