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
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
#if !SILVERLIGHT && !NETFX_CORE && !WP
using Syncfusion.Pdf.Exporting;
#endif
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;


namespace Syncfusion.Pdf
{
    /// <summary>
    /// The base class for all pages.
    /// </summary>
    public abstract class PdfPageBase : IPdfWrapper
    {
        #region Members
        private PdfDictionary m_pageDictionary;

        private PdfResources m_resources;
        /// <summary>
        /// Collection of the layers of the page.
        /// </summary>
        private PdfPageLayerCollection m_layers;

        /// <summary>
        /// Collection of the annotations of the page.
        /// </summary>
        private PdfLoadedAnnotationCollection m_annotations;

        /// <summary>
        /// Index of the default layer.
        /// </summary>
        private int m_defLayerIndex = -1;

        /// <summary>
        /// Local variable to store the Font Names.
        /// </summary>
        private List<PdfName> m_fontNames = null;

        /// <summary>
        /// Local variable to store the Font Refences.
        /// </summary>
        internal List<IPdfPrimitive> m_fontReference = null;

        /// <summary>
        /// Local variable to store information about images.
        /// </summary>
        private List<RectangleF> m_extractedImagesBounds = null;

        /// <summary>
        /// Local variable to store page template.
        /// </summary>
        private PdfTemplate m_contentTemplate = null;

        /// <summary>
        /// Local variable to store if page updated.
        /// </summary>
        private bool m_modified = false;
        
        /// <summary>
        /// Local variable to store annotation count in the page.
        /// </summary>
        private int m_annotCount = 0;

        /// <summary>
        /// Internal variable to store layers count.
        /// </summary>
        private int m_layersCount = 0;

        /// <summary>
        /// Internal variable to store combined length.
        /// </summary>
        private long m_pageContentLength = 0;

        /// <summary>
        /// Internal variable to store if the page is imported.
        /// </summary>
        private bool m_imported;

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Local variable to store resource information
        /// </summary>
        private PageResourceLoader resourceLoader = new PageResourceLoader();

        /// <summary>
        /// Local variable to store image information.
        /// </summary>
        private PdfImageInfo[] m_imageinfo = null;
        PdfRecordCollection m_recordCollection;
        PdfPageResources m_pageResources;
        private string m_currentFont;
        private Stack<System.Drawing.Drawing2D.GraphicsState> m_graphicsState = new Stack<System.Drawing.Drawing2D.GraphicsState>();
        char[] m_symbolChars = new char[] { '(', ')', '[', ']', '<', '>' };
#endif
        #endregion

        #region Properties
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets the information about the extracted image.
        /// </summary>
        public PdfImageInfo[] ImagesInfo
        {
            get
            {
                if (m_imageinfo == null)
                {
                    try
                    {
                        Image[] images = this.ExtractImages(true);
                        int index = 0;
                        int count = 0;
                        IEnumerator myEnumerator = m_extractedImagesBounds.GetEnumerator();
                        while (myEnumerator.MoveNext())
                        {
                            if (!m_imageinfo[index].IsImageExtracted)
                            {
                                m_imageinfo[index].Bounds = (RectangleF)(myEnumerator.Current);
                                m_imageinfo[index].Image = null;
                                m_imageinfo[index].Index = index;
                                index++;
                            }
                            else if (m_imageinfo[index].IsImageExtracted)
                            {
                                Image image = images[count];
                                m_imageinfo[index].Bounds = (RectangleF)(myEnumerator.Current);
                                m_imageinfo[index].Image = image;
                                m_imageinfo[index].Index = index;
                                index++;
                                count++;
                            }
                        }
                    }
                    catch (Exception e)
                    { }
                    return m_imageinfo;
                }
                else
                {
                    return m_imageinfo;
                }

            }
        }
#endif
        /// <summary>
        /// Gets the graphics of the <see cref="DefaultLayer"/>.
        /// </summary>
        public PdfGraphics Graphics
        {
            get
            {
                return DefaultLayer.Graphics;
            }
        }

        /// <summary>
        /// Gets or sets if a page is imported.
        /// </summary>
        internal bool Imported
        {
            get
            {
                return m_imported;
            }
            set
            {
                m_imported = value;
            }
        }

        /// <summary>
        /// Gets the collection of the page's layers.
        /// </summary>
        public PdfPageLayerCollection Layers
        {
            get
            {
                if (m_layers == null)
                {
                    m_layers = new PdfPageLayerCollection(this);
                }

                return m_layers;
            }
        }

        /// <summary>
        /// Gets the collection of the page's annotations.
        /// </summary>
        public PdfLoadedAnnotationCollection Annotations
        {
            get
            {
                if (m_annotations == null || m_annotations.Annotations.Count == 0)
                {
                    m_annotations = new PdfLoadedAnnotationCollection(this as PdfLoadedPage);
                }

                return m_annotations;

            }
        }

        /// <summary>
        /// Gets or sets index of the default layer.
        /// </summary>
        public int DefaultLayerIndex
        {
            get
            {
                if (Layers.Count == 0 || m_defLayerIndex == -1)
                {
                    PdfPageLayer layer = Layers.Add();
                    m_defLayerIndex = Layers.IndexOf(layer);
                }

                return m_defLayerIndex;
            }
            set
            {
                if (value < 0 || value > Layers.Count - 1)
                    throw new ArgumentOutOfRangeException("value", "Index can not be less 0 and greater Layers.Count - 1");

                m_defLayerIndex = value;
                m_modified = true;
            }
        }

        /// <summary>
        /// Gets the default layer of the page.
        /// </summary>
        public PdfPageLayer DefaultLayer
        {
            get
            {
                PdfPageLayer layer = Layers[DefaultLayerIndex];

                return layer;
            }
        }

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        public abstract SizeF Size
        {
            get;
        }

        /// <summary>
        /// Gets the origin of the page
        /// </summary>
        internal abstract PointF Origin
        {
            get;
        }
        /// <summary>
        /// Gets array of page's content.
        /// </summary>
        internal PdfArray Contents
        {
            get
            {
                IPdfPrimitive obj = m_pageDictionary[DictionaryProperties.Contents];
                PdfArray contents = obj as PdfArray;
                PdfReferenceHolder rh = obj as PdfReferenceHolder;

                if (rh != null)
                {
                    contents = rh.Object as PdfArray;

                    if (contents == null)
                    {
                        PdfStream contentStream = rh.Object as PdfStream;

                        if (contentStream != null)
                        {
                            contents = new PdfArray();
                            contents.Add(new PdfReferenceHolder(contentStream));
                            m_pageDictionary[DictionaryProperties.Contents] = contents;
                        }
                    }
                }

                if (contents == null)
                {
                    contents = new PdfArray();
                    m_pageDictionary[DictionaryProperties.Contents] = contents;
                }
                return contents;
            }
        }

        /// <summary>
        /// Gets the page dictionary.
        /// </summary>
#if NETFX_CORE || WP
        public PdfDictionary Dictionary
#else
        internal PdfDictionary Dictionary
#endif
        {
            get
            {
                return m_pageDictionary;
            }
        }

        /// <summary>
        /// Gets the page rotation.
        /// </summary>
        public PdfPageRotateAngle Rotation
        {
            get
            {
                return GetRotation();
            }
        }

        /// <summary>
        /// Gets the page orientation.
        /// </summary>
        internal PdfPageOrientation Orientation
        {
            get
            {
                return GetOrientation();
            }
        }

        /// <summary>
        /// Returns the page template.
        /// </summary>
        internal PdfTemplate ContentTemplate
        {
            get
            {
                bool refreshTemplate = false;

                using (MemoryStream ms = new MemoryStream())
                {
                    Layers.CombineContent(ms);
                    if (m_pageContentLength != ms.Length)
                        refreshTemplate = true;
                }

                if (m_contentTemplate == null || m_contentTemplate.m_content.Data.Length == 0 || m_layersCount != (m_layers == null ? 0 : m_layers.Count) || m_annotCount != GetAnnotationCount())
                    refreshTemplate = true;

                if (m_modified || refreshTemplate)
                    m_contentTemplate = GetContent();

                return m_contentTemplate;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageBase"/> class.
        /// </summary>
        /// <param name="dic">The page dictionary.</param>
        internal PdfPageBase(PdfDictionary dic)
        {
            if (dic == null)
                throw new ArgumentNullException("dic");

            m_pageDictionary = dic;
        }
        #endregion

        #region Public Methods
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Replace the Image at index's Position.
        /// </summary>
        /// <param name="index">index of an image</param>
        /// <param name="image">The New Replace image</param>
        public void ReplaceImage(int imageIndex, PdfImage image)
        {

            if (image is PdfMetafile)
                throw new NotSupportedException("Meta file image can't replaced");

            if (imageIndex < 0)
                throw new ArgumentException("Image index is not valid");

            if (image == null)
                throw new NullReferenceException("image");

            m_modified = true;

            try
            {
                PdfImageInfo[] imageInfo = this.ImagesInfo;
                image.Save();
                PdfReferenceHolder imageReference = new PdfReferenceHolder(image);
                PdfResources resource = this.GetResources();
                if (resource.ContainsKey(DictionaryProperties.XObject))
                {
                    if (resource[DictionaryProperties.XObject] is PdfDictionary)
                    {
                        int index = 0;
                        PdfDictionary resoureDictionary = resource[DictionaryProperties.XObject] as PdfDictionary;
                        PdfDictionary dictionaryCollection = new PdfDictionary();
                        while (resoureDictionary != null && resoureDictionary is PdfDictionary)
                        {
                            Dictionary<PdfName, IPdfPrimitive> items = resoureDictionary.Items;
                            if (imageIndex >= items.Count)
                            {
                                throw new ArgumentException("Image Index is not valid");
                            }
                            foreach (KeyValuePair<PdfName, IPdfPrimitive> item in items)
                            {
                                bool replace = false;
                                PdfDictionary xobjectDictionary;
                                if (item.Value is PdfReferenceHolder)
                                {
                                    xobjectDictionary = (item.Value as PdfReferenceHolder).Object as PdfDictionary;
                                }
                                else
                                {
                                    xobjectDictionary = item.Value as PdfDictionary;
                                }
                                if (xobjectDictionary.ContainsKey("Subtype"))
                                {
                                    if ((xobjectDictionary[DictionaryProperties.Subtype] as PdfName).Value == "Image")
                                    {
                                        PdfDictionary dic = resource[DictionaryProperties.XObject] as PdfDictionary;

                                            if (index == imageIndex)
                                            {
                                                string imageName = item.Key.ToString();
                                                foreach (PdfImageInfo information in imageInfo)
                                                {
                                                    imageName = StripSlashes(imageName);
                                                    if (information.Name == imageName)
                                                    {
                                                        replace = true;
                                                        if (information.Image != null)
                                                        {
                                                            long objIndex = (resoureDictionary[imageName] as PdfReferenceHolder).Reference.ObjNum;

                                                            int i = 0;
                                                            foreach (KeyValuePair<PdfName, IPdfPrimitive> dictionaryObject in dictionaryCollection.Items)
                                                            {
                                                                if (i == dictionaryCollection.Count - 1)
                                                                {
                                                                    resoureDictionary = dictionaryCollection[dictionaryObject.Key.Value] as PdfDictionary;
                                                                }
                                                                i = i + 1;
                                                            }
                                                            dictionaryCollection.Clear();
                                                            if (((item.Value as PdfReferenceHolder).Object) is PdfStream)
                                                            {
                                                                PdfStream stream = (((item.Value as PdfReferenceHolder).Object) as PdfStream);
                                                                if (this is PdfPage)
                                                                {
                                                                    if ((this as PdfPage).Document.FileStructure.IncrementalUpdate)
                                                                    {
                                                                        stream.Modify();
                                                                        stream.Clear();
                                                                    }
                                                                    else
                                                                    {
                                                                        stream.Clear();
                                                                    }
                                                                }
                                                                else if (this is PdfLoadedPage)
                                                                {
                                                                    if ((this as PdfLoadedPage).Document.FileStructure.IncrementalUpdate)
                                                                    {
                                                                        stream.Modify();
                                                                        stream.Clear();
                                                                    }
                                                                    else
                                                                    {
                                                                        stream.Clear();
                                                                    }
                                                                }
                                                            }

                                                            resoureDictionary.Items.Remove(item.Key);
                                                            float height = information.Image.Height;
                                                            float yPosition = information.Bounds.Top;
                                                            float paginateHeight = (this.Size.Height - (yPosition + height));
                                                            if (paginateHeight >= 0)
                                                            {
                                                                resoureDictionary.Items.Add(item.Key, imageReference);
                                                                resoureDictionary.Modify();
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                resoureDictionary.Items.Add(item.Key, imageReference);
                                                                resoureDictionary.Modify();
                                                                if (this is PdfLoadedPage)
                                                                {
                                                                    int pageIndex = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.IndexOf(this);
                                                                    int Count = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.Count;
                                                                    if (pageIndex < ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.Count - 1)
                                                                    {
                                                                        PdfLoadedPage page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex + 1] as PdfLoadedPage;
                                                                        bool exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                        if (!exist)
                                                                        {
                                                                            if (pageIndex > 0)
                                                                            {
                                                                                page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex - 1] as PdfLoadedPage;
                                                                                exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (pageIndex > 0)
                                                                        {
                                                                            PdfLoadedPage page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex - 1] as PdfLoadedPage;
                                                                            bool exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                            if (!exist)
                                                                            {
                                                                                if (pageIndex < Count - 1)
                                                                                {
                                                                                    page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex + 1] as PdfLoadedPage;
                                                                                    exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (((item.Value as PdfReferenceHolder).Object) is PdfStream)
                                                            {
                                                                PdfStream stream = (((item.Value as PdfReferenceHolder).Object) as PdfStream);
                                                                if (this is PdfPage)
                                                                {
                                                                    if ((this as PdfPage).Document.FileStructure.IncrementalUpdate)
                                                                    {
                                                                        stream.Modify();
                                                                        stream.Clear();
                                                                    }
                                                                    else
                                                                    {
                                                                        stream.Clear();
                                                                    }
                                                                }
                                                                else if (this is PdfLoadedPage)
                                                                {
                                                                    if ((this as PdfLoadedPage).Document.FileStructure.IncrementalUpdate)
                                                                    {
                                                                        stream.Modify();
                                                                        stream.Clear();
                                                                    }
                                                                    else
                                                                    {
                                                                        stream.Clear();
                                                                    }
                                                                }
                                                            }
                                                            long objIndex = (item.Value as PdfReferenceHolder).Reference.ObjNum;
                                                            resoureDictionary.Items.Remove(item.Key);
                                                            float height = information.Bounds.Height;
                                                            float yPosition = information.Bounds.Top;
                                                            float paginateHeight = (this.Size.Height - (yPosition + height));
                                                            if (paginateHeight >= 0)
                                                            {
                                                                resoureDictionary.Items.Add(item.Key, imageReference);
                                                                resoureDictionary.Modify();
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                resoureDictionary.Items.Add(item.Key, imageReference);
                                                                resoureDictionary.Modify();
                                                                if (this is PdfLoadedPage)
                                                                {
                                                                    int pageIndex = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.IndexOf(this);
                                                                    int Count = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.Count;
                                                                    if (pageIndex < ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.Count - 1)
                                                                    {
                                                                        PdfLoadedPage page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex + 1] as PdfLoadedPage;
                                                                        bool exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                        if (!exist)
                                                                        {
                                                                            if (pageIndex > 0)
                                                                            {
                                                                                page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex - 1] as PdfLoadedPage;
                                                                                exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (pageIndex > 0)
                                                                        {
                                                                            PdfLoadedPage page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex - 1] as PdfLoadedPage;
                                                                            bool exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                            if (!exist)
                                                                            {
                                                                                if (pageIndex < Count - 1)
                                                                                {
                                                                                    page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex + 1] as PdfLoadedPage;
                                                                                    exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                index++;    
                                            }

                                        
                                    }

                                }
                                if (xobjectDictionary.ContainsKey(DictionaryProperties.Resources))
                                {
                                    if (xobjectDictionary[DictionaryProperties.Resources] is PdfReferenceHolder)
                                    {
                                        xobjectDictionary = (xobjectDictionary[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary;
                                    }
                                    else
                                        xobjectDictionary = xobjectDictionary[DictionaryProperties.Resources] as PdfDictionary;
                                    if (xobjectDictionary.ContainsKey(DictionaryProperties.XObject))
                                    {

                                        resoureDictionary = xobjectDictionary[DictionaryProperties.XObject] as PdfDictionary;
                                        PdfDictionary.SetProperty(dictionaryCollection, item.Key.Value, resoureDictionary);
                                    }
                                }
                                else
                                    if (replace == true)
                                    {
                                        resoureDictionary = null;
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentException)
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// Replace the Image at index's Position.
        /// </summary>
        /// <param name="index">index of an image</param>
        /// <param name="image">The New Replace image</param>
        internal void ReplaceImageByName(string imgName, PdfImage image)
        {

            if (image is PdfMetafile)
                throw new NotSupportedException("Meta file image can't replaced");

            //if (imageIndex < 0)
            //    throw new ArgumentException("Image index is not valid");

            if (image == null)
                throw new NullReferenceException("image");

            m_modified = true;

            try
            {
                PdfImageInfo[] imageInfo = this.ImagesInfo;
                image.Save();
                PdfReferenceHolder imageReference = new PdfReferenceHolder(image);
                PdfResources resource = this.GetResources();
                if (resource.ContainsKey(DictionaryProperties.XObject))
                {
                    PdfDictionary resourceCollection = null;
                    if (resource[DictionaryProperties.XObject] is PdfDictionary)
                    {
                        resourceCollection = resource[DictionaryProperties.XObject] as PdfDictionary;
                    }
                    else if (resource[DictionaryProperties.XObject] is PdfReferenceHolder)
                    {
                        resourceCollection = (resource[DictionaryProperties.XObject] as PdfReferenceHolder).Object as PdfDictionary;
                    }
                    if (resourceCollection != null)
                    {

                        int index = 0;
                        PdfDictionary resoureDictionary = resourceCollection;
                        PdfDictionary dictionaryCollection = new PdfDictionary();
                        while (resoureDictionary != null && resoureDictionary is PdfDictionary)
                        {
                            Dictionary<PdfName, IPdfPrimitive> items = resoureDictionary.Items;
                            //if (imageIndex >= items.Count)
                            //{
                            //    throw new ArgumentException("Image Index is not valid");
                            //}
                            foreach (KeyValuePair<PdfName, IPdfPrimitive> item in items)
                            {
                                bool replace = false;
                                PdfDictionary xobjectDictionary;
                                if (item.Value is PdfReferenceHolder)
                                {
                                    xobjectDictionary = (item.Value as PdfReferenceHolder).Object as PdfDictionary;
                                }
                                else
                                {
                                    xobjectDictionary = item.Value as PdfDictionary;
                                }
                                if (xobjectDictionary.ContainsKey("Subtype"))
                                {
                                    if ((xobjectDictionary[DictionaryProperties.Subtype] as PdfName).Value == "Image")
                                    {
                                        PdfDictionary dic = resource[DictionaryProperties.XObject] as PdfDictionary;

                                        //if (index == imageIndex)
                                        {
                                            string imageName = item.Key.ToString();
                                            foreach (PdfImageInfo information in imageInfo)
                                            {
                                                imageName = StripSlashes(imageName);
                                                if (information.Name == imageName && imageName == imgName)
                                                {
                                                    replace = true;
                                                    if (information.Image != null)
                                                    {
                                                        long objIndex = (resoureDictionary[imageName] as PdfReferenceHolder).Reference.ObjNum;

                                                        int i = 0;
                                                        foreach (KeyValuePair<PdfName, IPdfPrimitive> dictionaryObject in dictionaryCollection.Items)
                                                        {
                                                            if (i == dictionaryCollection.Count - 1)
                                                            {
                                                                resoureDictionary = dictionaryCollection[dictionaryObject.Key.Value] as PdfDictionary;
                                                            }
                                                            i = i + 1;
                                                        }
                                                        dictionaryCollection.Clear();
                                                        if (((item.Value as PdfReferenceHolder).Object) is PdfStream)
                                                        {
                                                            PdfStream stream = (((item.Value as PdfReferenceHolder).Object) as PdfStream);
                                                            if (this is PdfPage)
                                                            {
                                                                if ((this as PdfPage).Document.FileStructure.IncrementalUpdate)
                                                                {
                                                                    stream.Modify();
                                                                    stream.Clear();
                                                                }
                                                                else
                                                                {
                                                                    stream.Clear();
                                                                }
                                                            }
                                                            else if (this is PdfLoadedPage)
                                                            {
                                                                if ((this as PdfLoadedPage).Document.FileStructure.IncrementalUpdate)
                                                                {
                                                                    stream.Modify();
                                                                    stream.Clear();
                                                                }
                                                                else
                                                                {
                                                                    stream.Clear();
                                                                }
                                                            }
                                                        }

                                                        resoureDictionary.Items.Remove(item.Key);
                                                        float height = information.Image.Height;
                                                        float yPosition = information.Bounds.Top;
                                                        float paginateHeight = (this.Size.Height - (yPosition + height));
                                                        if (paginateHeight >= 0)
                                                        {
                                                            resoureDictionary.Items.Add(item.Key, imageReference);
                                                            resoureDictionary.Modify();
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            resoureDictionary.Items.Add(item.Key, imageReference);
                                                            resoureDictionary.Modify();
                                                            if (this is PdfLoadedPage)
                                                            {
                                                                int pageIndex = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.IndexOf(this);
                                                                int Count = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.Count;
                                                                if (pageIndex < ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.Count - 1)
                                                                {
                                                                    PdfLoadedPage page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex + 1] as PdfLoadedPage;
                                                                    bool exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                    if (!exist)
                                                                    {
                                                                        if (pageIndex > 0)
                                                                        {
                                                                            page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex - 1] as PdfLoadedPage;
                                                                            exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (pageIndex > 0)
                                                                    {
                                                                        PdfLoadedPage page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex - 1] as PdfLoadedPage;
                                                                        bool exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                        if (!exist)
                                                                        {
                                                                            if (pageIndex < Count - 1)
                                                                            {
                                                                                page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex + 1] as PdfLoadedPage;
                                                                                exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (((item.Value as PdfReferenceHolder).Object) is PdfStream)
                                                        {
                                                            PdfStream stream = (((item.Value as PdfReferenceHolder).Object) as PdfStream);
                                                            if (this is PdfPage)
                                                            {
                                                                if ((this as PdfPage).Document.FileStructure.IncrementalUpdate)
                                                                {
                                                                    stream.Modify();
                                                                    stream.Clear();
                                                                }
                                                                else
                                                                {
                                                                    stream.Clear();
                                                                }
                                                            }
                                                            else if (this is PdfLoadedPage)
                                                            {
                                                                if ((this as PdfLoadedPage).Document.FileStructure.IncrementalUpdate)
                                                                {
                                                                    stream.Modify();
                                                                    stream.Clear();
                                                                }
                                                                else
                                                                {
                                                                    stream.Clear();
                                                                }
                                                            }
                                                        }
                                                        long objIndex = (item.Value as PdfReferenceHolder).Reference.ObjNum;
                                                        resoureDictionary.Items.Remove(item.Key);
                                                        float height = information.Bounds.Height;
                                                        float yPosition = information.Bounds.Top;
                                                        float paginateHeight = (this.Size.Height - (yPosition + height));
                                                        if (paginateHeight >= 0)
                                                        {
                                                            resoureDictionary.Items.Add(item.Key, imageReference);
                                                            resoureDictionary.Modify();
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            resoureDictionary.Items.Add(item.Key, imageReference);
                                                            resoureDictionary.Modify();
                                                            if (this is PdfLoadedPage)
                                                            {
                                                                int pageIndex = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.IndexOf(this);
                                                                int Count = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.Count;
                                                                if (pageIndex < ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages.Count - 1)
                                                                {
                                                                    PdfLoadedPage page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex + 1] as PdfLoadedPage;
                                                                    bool exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                    if (!exist)
                                                                    {
                                                                        if (pageIndex > 0)
                                                                        {
                                                                            page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex - 1] as PdfLoadedPage;
                                                                            exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (pageIndex > 0)
                                                                    {
                                                                        PdfLoadedPage page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex - 1] as PdfLoadedPage;
                                                                        bool exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                        if (!exist)
                                                                        {
                                                                            if (pageIndex < Count - 1)
                                                                            {
                                                                                page = ((this as PdfLoadedPage).Document as PdfLoadedDocument).Pages[pageIndex + 1] as PdfLoadedPage;
                                                                                exist = ReplacePaginatedImage(page, item.Key.ToString(), imageReference, objIndex);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        //else
                                        //{
                                        //    index++;    
                                        //}


                                    }

                                }
                                if (xobjectDictionary.ContainsKey(DictionaryProperties.Resources))
                                {
                                    if (xobjectDictionary[DictionaryProperties.Resources] is PdfReferenceHolder)
                                    {
                                        xobjectDictionary = (xobjectDictionary[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary;
                                    }
                                    else
                                        xobjectDictionary = xobjectDictionary[DictionaryProperties.Resources] as PdfDictionary;
                                    if (xobjectDictionary.ContainsKey(DictionaryProperties.XObject))
                                    {

                                        resoureDictionary = xobjectDictionary[DictionaryProperties.XObject] as PdfDictionary;
                                        PdfDictionary.SetProperty(dictionaryCollection, item.Key.Value, resoureDictionary);
                                    }
                                }
                                else
                                    if (replace == true)
                                    {
                                        resoureDictionary = null;
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentException)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Replace the Paginated Image.
        /// </summary>
        /// <param name="page">Loaded Page</param>
        /// <param name="name">Image key name</param>
        /// <param name="imageReference">New image Reference</param>
        /// <param name="objIndex">Object index</param>
        /// <returns>The image is replaced or not</returns>
        private bool ReplacePaginatedImage(PdfLoadedPage page, string name, PdfReferenceHolder imageReference, long objIndex)
        {
            m_modified = true;

            PdfResources resource = page.GetResources();
            if (resource.ContainsKey(DictionaryProperties.XObject))
            {
                if (resource[DictionaryProperties.XObject] is PdfDictionary)
                {
                    PdfDictionary resoureDictionary = resource[DictionaryProperties.XObject] as PdfDictionary;
                    Dictionary<PdfName, IPdfPrimitive> items = resoureDictionary.Items;
                    foreach (KeyValuePair<PdfName, IPdfPrimitive> item in items)
                    {
                        long index = (item.Value as PdfReferenceHolder).Reference.ObjNum;
                        if (index == objIndex)
                        {
                            resoureDictionary.Items.Remove(item.Key);
                            resoureDictionary.Items.Add(item.Key, imageReference);
                            resoureDictionary.Modify();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
#endif
        /// <summary>
        /// Creates a template from page content and all annotation appearances.
        /// </summary>
        /// <returns>The created template.</returns>
        public PdfTemplate CreateTemplate()
        {
            //PdfTemplate pageTemplate = new PdfTemplate(Size);
            PdfTemplate content = GetContent();

            //PdfGraphics g = pageTemplate.Graphics;

            //g.DrawPdfTemplate(content, PointF.Empty);

            //DrawAnnotationTemplates(g);

            return content;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        string resultantText = null;
        /// <summary>
        /// Extracts text from the given PDF Page.
        /// </summary>
        /// <returns>The Extracted Text.</returns>
        public string ExtractText()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                this.Layers.CombineContent(stream);
                stream.Position = 0;

                ContentParser parser = new ContentParser(stream.ToArray());
                m_recordCollection = parser.ReadContent();
            }
            m_pageResources = PageResourceLoader.Instance.GetPageResources(this);

            RenderText(m_recordCollection, m_pageResources);

            if (resultantText != null)
                resultantText = SkipEscapeSequence(resultantText);
            return resultantText;
        }
        private void RenderText(PdfRecordCollection recordCollection, PdfPageResources m_pageResources)
        {
            if (recordCollection != null)
                foreach (PdfRecord record in recordCollection)
                {
                    string token = record.OperatorName;
                    string[] element = record.Operands;

                    foreach (char ch in m_symbolChars)
                    {
                        if (token.Contains(ch.ToString()))
                            token = token.Replace(ch.ToString(), "");
                    }
                    switch (token.Trim())
                    {
                        case "T*":
                            {
                                resultantText += "\r\n";
                                break;
                            }
                        case "Tf":
                            {
                                RenderFont(element);
                                break;
                            }
                        case "ET":
                            {
                                resultantText += "\r\n";
                                break;
                            }
                        case "TJ":
                        case "Tj":
                        case "'":
                            {
                                resultantText += RenderTextElement(element, token, m_pageResources);
                                if (token == "'")
                                {
                                    resultantText += "\r\n";
                                }
                                break;
                            }
                        case "Do":
                            {
                                GetXObject(element, m_pageResources);
                                break;
                            }

                    }
                }
        }

        private void GetXObject(string[] xobjectElement, PdfPageResources m_pageResources)
        {
            if (m_pageResources.ContainsKey(StripSlashes(xobjectElement[0])))
            {
                if (m_pageResources[StripSlashes(xobjectElement[0])] is ImageStructure)
                    return;
                PdfRecordCollection collection = (m_pageResources[StripSlashes(xobjectElement[0])] as XObjectElement).Render(m_pageResources, m_graphicsState);

                PdfDictionary xobjects = ((m_pageResources[StripSlashes(xobjectElement[0])] as XObjectElement).XObjectDictionary);
                PdfPageResources childResource = new PdfPageResources();
                Dictionary<string, PdfMatrix> commonMatrix = new Dictionary<string, PdfMatrix>();
                if (xobjects.ContainsKey(DictionaryProperties.Resources))
                {
                    PdfDictionary pageDictionary = new PdfDictionary();
                    if (xobjects[DictionaryProperties.Resources] is PdfReferenceHolder)
                        pageDictionary = ((xobjects[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary);
                    else
                        pageDictionary = xobjects[DictionaryProperties.Resources] as PdfDictionary;
                    childResource = resourceLoader.UpdatePageResources(childResource, resourceLoader.GetImageResources(pageDictionary, this, ref commonMatrix));
                    childResource = resourceLoader.UpdatePageResources(childResource, resourceLoader.GetFontResources(pageDictionary));
                }

                RenderText(collection, childResource);
            }
        }
        private string RenderTextElement(string[] textElements, string tokenType, PdfPageResources m_pageResources)
        {
            try
            {
                string text = string.Join("", textElements);
                if (m_pageResources.ContainsKey(m_currentFont))
                {
                    FontStructure structure = m_pageResources[m_currentFont] as FontStructure;
                    structure.IsTextExtraction = true;
                    text = structure.Decode(text, true);
                }
                return text;
            }
            catch
            {
                return null;
            }
        }
        private void RenderFont(string[] fontElements)
        {
            int i;
            for (i = 0; i < fontElements.Length; i++)
            {
                if (fontElements[i].Contains("/"))
                {
                    m_currentFont = fontElements[i].Replace("/", "");
                    break;
                }
            }
        }

        private string SkipEscapeSequence(string text)
        {
            int index = -1;
            do
            {
                index = text.IndexOf("\\", (index + 1));

                if (text.Length > index + 1)
                {
                    string nextLiteral = text[index + 1].ToString();
                    if ((index >= 0) && (nextLiteral == "\\" || nextLiteral == "(" || nextLiteral == ")"))
                        text = text.Remove(index, 1);
                }
                else
                {
                    text = text.Remove(index, 1);
                    index = -1;
                }

            }
            while (index >= 0);

            return text;
        }

        /// <summary>
        /// Gets the Font from the Xobject.
        /// </summary>
        internal Dictionary<PdfName, IPdfPrimitive> GetFontDictionary(PdfDictionary xobjectStream)
        {
            if (xobjectStream != null && xobjectStream.ContainsKey(DictionaryProperties.Font) && xobjectStream[DictionaryProperties.Font] is PdfDictionary)
            {
                PdfDictionary xobjectFontDictionary = xobjectStream[DictionaryProperties.Font] as PdfDictionary;
                List<IPdfPrimitive> xobject_fontReference = new List<IPdfPrimitive>();
                List<PdfName> xobject_fontname = new List<PdfName>();
                Dictionary<PdfName, IPdfPrimitive> xitems = xobjectFontDictionary.Items;
                foreach (KeyValuePair<PdfName, IPdfPrimitive> xitem in xitems)
                {
                    xobject_fontname.Add(xitem.Key);
                    xobject_fontReference.Add(xitem.Value);
                }
                return xitems;
            }
            else
                return null;
        }
#endif
        /// <summary>
        /// Collects the Font.
        /// </summary>
        internal void GetFontStream()
        {
            PdfDictionary pagedic = Dictionary;
            if (Dictionary.ContainsKey(DictionaryProperties.Resources))
            {
                PdfDictionary ResourceDic = Dictionary[DictionaryProperties.Resources] as PdfDictionary;
                if (ResourceDic is PdfDictionary)
                {
                    PdfDictionary FontDic = null;

                    if (ResourceDic.ContainsKey(DictionaryProperties.Font))
                        FontDic = ResourceDic[DictionaryProperties.Font] as PdfDictionary;

                    else
                    {

                        PdfDictionary resourceDictionary = null;
                        PdfResources resources = this.GetResources();
                        IPdfPrimitive primitive = GetXObject(resources);

                        while (true)
                        {
                            if (primitive != null && primitive is PdfDictionary)
                            {
                                Dictionary<PdfName, IPdfPrimitive> resC = ((PdfDictionary)primitive).Items;
                                foreach (KeyValuePair<PdfName, IPdfPrimitive> items in resC)
                                {
                                    if (items.Value is PdfReferenceHolder)
                                    {
                                        PdfStream str = ((PdfReferenceHolder)items.Value).Object as PdfStream;
                                        PdfDictionary dr = str as PdfDictionary;
                                        if (dr.ContainsKey(DictionaryProperties.Resources))
                                        {
                                            resourceDictionary = dr[DictionaryProperties.Resources] as PdfDictionary;
                                        }
                                    }
                                }
                            }

                            if (resourceDictionary != null)
                            {
                                if (resourceDictionary.ContainsKey(DictionaryProperties.Font))
                                {
                                    FontDic = resourceDictionary[DictionaryProperties.Font] as PdfDictionary;
                                    break;
                                }

                                else if (primitive == null)
                                    break;

                                else
                                {
                                    resources = new PdfResources((PdfCrossTable.Dereference(resourceDictionary[DictionaryProperties.XObject] as PdfDictionary)) as PdfDictionary);
                                    primitive = GetXObject(resources);
                                }
                            }
                            else
                                break;
                        }

                    }

                    if (FontDic != null)
                    {
                        m_fontNames = new List<PdfName>();
                        m_fontReference = new List<IPdfPrimitive>();
                        Dictionary<PdfName, IPdfPrimitive> m_fontcollect = FontDic.Items;
                        foreach (KeyValuePair<PdfName, IPdfPrimitive> item in m_fontcollect)
                        {
                            m_fontNames.Add(item.Key);
                            m_fontReference.Add(item.Value);
                        }
                    }
                    else if (ResourceDic[DictionaryProperties.Font] is PdfReferenceHolder)
                    {
                        FontDic = (ResourceDic[DictionaryProperties.Font] as PdfReferenceHolder).Object as PdfDictionary; ;
                        if (FontDic != null)
                        {
                            m_fontNames = new List<PdfName>();
                            m_fontReference = new List<IPdfPrimitive>();
                            Dictionary<PdfName, IPdfPrimitive> m_fontcollect = FontDic.Items;
                            foreach (KeyValuePair<PdfName, IPdfPrimitive> item in m_fontcollect)
                            {
                                m_fontNames.Add(item.Key);
                                m_fontReference.Add(item.Value);
                            }
                        }
                    }
                    if (FontDic == null && ResourceDic.ContainsKey(DictionaryProperties.XObject))
                    {
                        PdfDictionary xObject = ResourceDic[DictionaryProperties.XObject] as PdfDictionary;

                        if (xObject is PdfDictionary)
                        {
                            Dictionary<PdfName, IPdfPrimitive> m_xobjectCollection = xObject.Items;

                            foreach (KeyValuePair<PdfName, IPdfPrimitive> item in m_xobjectCollection)
                            {
                                if (item.Value is PdfReferenceHolder)
                                {

                                    PdfReferenceHolder holder = item.Value as PdfReferenceHolder;
                                    PdfDictionary dictionary = holder.Object as PdfDictionary;
                                    if (dictionary.ContainsKey(DictionaryProperties.Resources))
                                    {
                                        dictionary = dictionary[DictionaryProperties.Resources] as PdfDictionary;
                                        if (dictionary.ContainsKey(DictionaryProperties.Font))
                                        {
                                            m_fontNames = new List<PdfName>();
                                            m_fontReference = new List<IPdfPrimitive>();
                                            PdfDictionary fontDictionary = dictionary[DictionaryProperties.Font] as PdfDictionary;

                                            if (fontDictionary == null)
                                            {
                                                PdfReferenceHolder fontRefHolder = dictionary[DictionaryProperties.Font] as PdfReferenceHolder;
                                                if (fontRefHolder != null)
                                                {
                                                    fontDictionary = fontRefHolder.Object as PdfDictionary;
                                                }
                                            }

                                            Dictionary<PdfName, IPdfPrimitive> m_fontcollect = fontDictionary.Items;
                                            foreach (KeyValuePair<PdfName, IPdfPrimitive> fontitem in m_fontcollect)
                                            {
                                                m_fontNames.Add(fontitem.Key);
                                                m_fontReference.Add(fontitem.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                else if (ResourceDic == null)
                {
                    PdfReferenceHolder resourceDic = Dictionary[DictionaryProperties.Resources] as PdfReferenceHolder;
                    if (resourceDic != null)
                    {
                        if (resourceDic.Object is PdfDictionary)
                        {
                            PdfDictionary Resourcedic = resourceDic.Object as PdfDictionary;
                            if (Resourcedic[DictionaryProperties.Font] is PdfDictionary)
                            {
                                PdfDictionary FontDic = Resourcedic[DictionaryProperties.Font] as PdfDictionary;
                                if (FontDic != null)
                                {
                                    m_fontNames = new List<PdfName>();
                                    m_fontReference = new List<IPdfPrimitive>();
                                    Dictionary<PdfName, IPdfPrimitive> m_fontcollect = FontDic.Items;

                                    foreach (KeyValuePair<PdfName, IPdfPrimitive> item in m_fontcollect)
                                    {
                                        m_fontNames.Add(item.Key);
                                        m_fontReference.Add(item.Value);
                                    }
                                }
                            }
                            else if (Resourcedic[DictionaryProperties.Font] is PdfReferenceHolder)
                            {
                                PdfDictionary FontDic = ((Resourcedic[DictionaryProperties.Font] as PdfReferenceHolder).Object) as PdfDictionary;
                                if (FontDic != null)
                                {
                                    m_fontNames = new List<PdfName>();
                                    m_fontReference = new List<IPdfPrimitive>();
                                    Dictionary<PdfName, IPdfPrimitive> m_fontcollect = FontDic.Items;

                                    foreach (KeyValuePair<PdfName, IPdfPrimitive> item in m_fontcollect)
                                    {
                                        m_fontNames.Add(item.Key);
                                        m_fontReference.Add(item.Value);
                                    }
                                }
                            }


                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets the XObject from the Resource dictionary
        /// </summary>
        /// <param name="resources">Page resource</param>
        /// <returns>XObject</returns>
        private IPdfPrimitive GetXObject(PdfResources resources)
        {
            IPdfPrimitive xObject = null;
            Dictionary<PdfName, IPdfPrimitive> resCollection = resources.Items;
            foreach (KeyValuePair<PdfName, IPdfPrimitive> entry in resCollection)
            {
                if (entry.Key.ToString() == "/XObject")
                    xObject = PdfCrossTable.Dereference(entry.Value);
            }

            return xObject;

        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Extracts images from the given PDF Page.
        /// </summary>
        /// <returns>Returns the extracted image as Image[].</returns>
        internal Image[] ExtractImages(bool imageExtraction)
        {
            IPdfPrimitive primitive = null;
            IPdfPrimitive internalResources = null;

            PdfDictionary imageDictionary = new PdfDictionary();
            ArrayList extractedImages = new ArrayList();
            List<PdfMatrix> extractedImageMatrix = new List<PdfMatrix>();
            List<bool> maskImageCollection = new List<bool>();
            m_extractedImagesBounds = new List<RectangleF>();

            // Reading Content Stream
            PdfArray contentArray = Contents;
            PdfResources resources = this.GetResources();
            Dictionary<PdfName, IPdfPrimitive> resCollection = resources.Items;
            Dictionary<string, PdfMatrix> XObjectCollection = new Dictionary<string, PdfMatrix>();
            PdfStream temp = null;
            string key = string.Empty;

            m_pageResources = PageResourceLoader.Instance.GetPageResources(this);

            PdfStream stream = null;
            List<PdfImageInfo> m_imageInfoList = new List<PdfImageInfo>();

            foreach (KeyValuePair<string, object> entry in m_pageResources.Resources)
            {
                if (!(entry.Value is ImageStructure) && !(entry.Value is XObjectElement))
                    continue;

                PdfMatrix tm = null;
                PdfReader contentReader = null;

                try
                {
                    if (entry.Value is XObjectElement)
                    {
                        key = entry.Key.ToString();
                        for (int i = 0; i < contentArray.Count; i++)
                        {
                            PdfStream str;
                            if (temp != null)
                                str = temp;
                            else
                                str = (contentArray[i] as PdfReferenceHolder).Object as PdfStream;
                            str.Decompress();
                            MemoryStream mstream = str.InternalStream;
                            mstream.Position = 0;
                            contentReader = new PdfReader(mstream);
                            contentReader.Position = 0;
                            if (contentReader.ReadStream().Contains(key))
                            {
                                PdfStream strXObject = (entry.Value as XObjectElement).XObjectDictionary as PdfStream;

                                MemoryStream internalStream = strXObject.InternalStream;
                                internalStream.Position = 0;
                                PdfReader contentReaderXObject = new PdfReader(internalStream);
                                contentReaderXObject.Position = 0;
                                bool containsFontStructure = false;
                                foreach (KeyValuePair<string, object> entryImage in m_pageResources.Resources)
                                {
                                    if (!(entryImage.Value is ImageStructure) && !(entryImage.Value is FontStructure))
                                        continue;
                                    string imageKey = entryImage.Key.ToString();

                                    if ((entryImage.Value is ImageStructure))
                                    {
                                        if (contentReaderXObject.ReadStream().Contains(imageKey) && !containsFontStructure)
                                        {
                                            XObjectCollection.Add(imageKey, new PdfMatrix(contentReader, key, this.Size));
                                        }
                                    }
                                    else if ((entryImage.Value is FontStructure))
                                    {
                                        if (contentReaderXObject.ReadStream().Contains(imageKey))
                                        {
                                            containsFontStructure = true;
                                        }
                                        contentReaderXObject.Position = 0;
                                    }
                                }
                                containsFontStructure = false;
                                break;
                            }
                        }
                        continue;
                    }
                    stream = (entry.Value as ImageStructure).ImageDictionary as PdfStream;
                    key = entry.Key.ToString();

                    tm = (entry.Value as ImageStructure).ImageInfo;
                    for (int i = 0; i < contentArray.Count; i++)
                    {
                        PdfStream str;
                        if (temp != null)
                            str = temp;
                        else
                            str = (contentArray[i] as PdfReferenceHolder).Object as PdfStream;
                        str.Decompress();
                        MemoryStream mstream = str.InternalStream;
                        mstream.Position = 0;
                        contentReader = new PdfReader(mstream);
                        contentReader.Position = 0;
                        if (contentReader.ReadStream().Contains(key))
                        {
                            tm = new PdfMatrix(contentReader, key, this.Size);
                            break;
                        }
                    }
                    RectangleF bounds;
                    bool isMask = false;
                    if (entry.Value is ImageStructure)
                    {
                        PdfDictionary maskDictionary = (entry.Value as ImageStructure).ImageDictionary;
                        if (maskDictionary.ContainsKey(DictionaryProperties.Mask))
                        {
                            isMask = true;
                        }
                    }

                    int imageWidth = 0;
                    int imageHeight = 0;
                    if (stream.ContainsKey(DictionaryProperties.Width))
                    {
                        if (stream[DictionaryProperties.Width] is PdfNumber)
                            imageWidth = (stream[DictionaryProperties.Width] as PdfNumber).IntValue;
                        if (stream[DictionaryProperties.Width] is PdfReferenceHolder)
                            imageWidth = ((stream[DictionaryProperties.Width] as PdfReferenceHolder).Object as PdfNumber).IntValue;

                        if (stream[DictionaryProperties.Height] is PdfNumber)
                            imageHeight = (stream[DictionaryProperties.Height] as PdfNumber).IntValue;
                        if (stream[DictionaryProperties.Height] is PdfReferenceHolder)
                            imageHeight = ((stream[DictionaryProperties.Height] as PdfReferenceHolder).Object as PdfNumber).IntValue;
                    }
                    else
                    {
                        if (stream.ContainsKey(DictionaryProperties.BBox))
                        {
                            PdfArray array = stream[DictionaryProperties.BBox] as PdfArray;
                            imageWidth = (array[2] as PdfNumber).IntValue;
                            imageHeight = (array[3] as PdfNumber).IntValue;
                        }
                    }

                    if (tm != null)
                    {
                        if (tm.GetWidth == -1 && tm.GetHeight == -1)
                            bounds = new RectangleF(new PointF(Math.Abs(tm.GetScaleX), Math.Abs(tm.GetScaleY)), new SizeF(Math.Abs(imageWidth), Math.Abs(imageHeight)));
                        else
                            bounds = new RectangleF(new PointF(Math.Abs(tm.GetScaleX), Math.Abs(tm.GetScaleY)), new SizeF(Math.Abs(tm.GetWidth), Math.Abs(tm.GetHeight)));
                    }
                    else
                        bounds = new RectangleF(0, 0, imageWidth, imageHeight);
                    if (!tm.m_scaledBounds.IsEmpty)
                    {
                        bounds = tm.m_scaledBounds;
                    }
                    PdfImageInfo info = new PdfImageInfo();
                    info.Name = entry.Key.ToString();

                    info.IsImageExtracted = true;
                    Image image = (entry.Value as ImageStructure).EmbeddedImage;
                    if (image != null)
                    {
                        extractedImages.Add(image);
                        m_extractedImagesBounds.Add(bounds);
                        m_imageInfoList.Add(info);
                        extractedImageMatrix.Add(tm);
                        maskImageCollection.Add(isMask);
                    }
                }
                catch (Exception exception)
                {
                }
            }
            if (XObjectCollection.Count > 0)
            {
                for (int i = 0; i < m_imageInfoList.Count; i++)
                {
                    if (XObjectCollection.ContainsKey(m_imageInfoList[i].Name))
                    {
                        RectangleF newImageBounds = m_extractedImagesBounds[i];
                        PdfMatrix newMatrix = XObjectCollection[m_imageInfoList[i].Name];

                        if (newMatrix.GetWidth < 1 && newMatrix.GetHeight < 1)
                            newImageBounds = new RectangleF(newMatrix.GetScaleX + (newImageBounds.X * newMatrix.GetWidth),newMatrix.TopMargin + (newImageBounds.Y * newMatrix.GetHeight), newImageBounds.Width * newMatrix.GetWidth, newImageBounds.Height * newMatrix.GetHeight);
                        else
                            newImageBounds = new RectangleF(newMatrix.GetScaleX + newImageBounds.X, newMatrix.GetScaleX + newImageBounds.Y, newMatrix.GetWidth, newMatrix.GetHeight);

                        m_extractedImagesBounds[i] = newImageBounds;
                    }
                }

            }
            m_imageinfo = m_imageInfoList.ToArray();
            Image[] images = new Image[extractedImages.Count];
            extractedImages.CopyTo(images);
            int index = 0;
            int count = 0;
            IEnumerator myEnumerator = m_extractedImagesBounds.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                if (!m_imageinfo[index].IsImageExtracted)
                {
                    m_imageinfo[index].Bounds = (RectangleF)(myEnumerator.Current);
                    m_imageinfo[index].Image = null;
                    m_imageinfo[index].Index = index;
                    m_imageinfo[index].Matrix = extractedImageMatrix[index];
                    m_imageinfo[index].MaskImage = maskImageCollection[index];
                    index++;
                }
                else if (m_imageinfo[index].IsImageExtracted)
                {
                    Image image = images[count];
                    m_imageinfo[index].Bounds = (RectangleF)(myEnumerator.Current);
                    m_imageinfo[index].Image = image;
                    m_imageinfo[index].Index = index;
                    m_imageinfo[index].Matrix = extractedImageMatrix[index];
                    m_imageinfo[index].MaskImage = maskImageCollection[index];
                    index++;
                    count++;
                }
            }
            return images;
        }

        /// <summary>
        /// Extract the images from the PDF
        /// </summary>
        /// <returns>returns the List of images</returns>
        public Image[] ExtractImages()
        {
            IPdfPrimitive primitive = null;
            IPdfPrimitive internalResources = null;

            PdfDictionary imageDictionary = new PdfDictionary();
            ArrayList extractedImages = new ArrayList();
            m_extractedImagesBounds = new List<RectangleF>();

            PdfResources resources = this.GetResources();
            Dictionary<PdfName, IPdfPrimitive> resCollection = resources.Items;
            PdfStream temp = null;
            string key = string.Empty;

            m_pageResources = PageResourceLoader.Instance.GetPageResources(this);

            PdfStream stream = null;
            List<PdfImageInfo> m_imageInfoList = new List<PdfImageInfo>();

            foreach (KeyValuePair<string, object> entry in m_pageResources.Resources)
            {
                if (!(entry.Value is ImageStructure))
                    continue;
                try
                {
                    if ((entry.Value as ImageStructure).ImageFilter[0] == "JPXDecode")
                    {
                        continue;
                    }

                    PdfDictionary dic = (entry.Value as ImageStructure).ImageDictionary;
                    if (dic != null)
                    {
                        PdfArray value = null;
                        PdfArray tempArray = null;
                        if (dic[DictionaryProperties.ColorSpace] == null)
                        {
                            continue;
                        }
                        if (dic[DictionaryProperties.ColorSpace] is PdfArray)
                            value = dic[DictionaryProperties.ColorSpace] as PdfArray;
                        if (dic[DictionaryProperties.ColorSpace] is PdfReferenceHolder)
                            value = (dic[DictionaryProperties.ColorSpace] as PdfReferenceHolder).Object as PdfArray;
                        if (value != null && value[1] is PdfReferenceHolder)
                            tempArray = (value[1] as PdfReferenceHolder).Object as PdfArray;
                        if (tempArray != null)
                        {
                            if (tempArray[1] is PdfReferenceHolder)
                            {
                                PdfDictionary internalColorDic = (tempArray[1] as PdfReferenceHolder).Object as PdfDictionary;
                                if (!internalColorDic.ContainsKey(DictionaryProperties.Alternate))
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    stream = (entry.Value as ImageStructure).ImageDictionary as PdfStream;
                    int imageWidth = 0;
                    int imageHeight = 0;
                    if (stream.ContainsKey(DictionaryProperties.Width))
                    {
                        if (stream[DictionaryProperties.Width] is PdfNumber)
                            imageWidth = (stream[DictionaryProperties.Width] as PdfNumber).IntValue;
                        if (stream[DictionaryProperties.Width] is PdfReferenceHolder)
                            imageWidth = ((stream[DictionaryProperties.Width] as PdfReferenceHolder).Object as PdfNumber).IntValue;

                        if (stream[DictionaryProperties.Height] is PdfNumber)
                            imageHeight = (stream[DictionaryProperties.Height] as PdfNumber).IntValue;
                        if (stream[DictionaryProperties.Height] is PdfReferenceHolder)
                            imageHeight = ((stream[DictionaryProperties.Height] as PdfReferenceHolder).Object as PdfNumber).IntValue;
                    }
                    else
                    {
                        if (stream.ContainsKey(DictionaryProperties.BBox))
                        {
                            PdfArray array = stream[DictionaryProperties.BBox] as PdfArray;
                            imageWidth = (array[2] as PdfNumber).IntValue;
                            imageHeight = (array[3] as PdfNumber).IntValue;
                        }
                    }

                    RectangleF bounds;

                    bounds = new RectangleF(0, 0, imageWidth, imageHeight);

                    PdfImageInfo info = new PdfImageInfo();
                    info.Name = entry.Key.ToString();

                    info.IsImageExtracted = true;
                    Image image = (entry.Value as ImageStructure).EmbeddedImage;
                    if (image != null)
                    {
                        extractedImages.Add(image);
                        m_extractedImagesBounds.Add(bounds);
                        m_imageInfoList.Add(info);
                    }
                }
                catch (Exception exception)
                {
                }
            }
            m_imageinfo = m_imageInfoList.ToArray();
            Image[] images = new Image[extractedImages.Count];
            extractedImages.CopyTo(images);
            int index = 0;
            int count = 0;
            IEnumerator myEnumerator = m_extractedImagesBounds.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                if (!m_imageinfo[index].IsImageExtracted)
                {
                    m_imageinfo[index].Bounds = (RectangleF)(myEnumerator.Current);
                    m_imageinfo[index].Image = null;
                    m_imageinfo[index].Index = index;
                    index++;
                }
                else if (m_imageinfo[index].IsImageExtracted)
                {
                    Image image = images[count];
                    m_imageinfo[index].Bounds = (RectangleF)(myEnumerator.Current);
                    m_imageinfo[index].Image = image;
                    m_imageinfo[index].Index = index;
                    index++;
                    count++;
                }
            }
            return images;
        }



        private string StripSlashes(string text)
        {
            return text.Replace("/", "");
        }
#endif
        #endregion

        #region Handlers
        /// <summary>
        /// Gets the resources and modifies the page dictionary.
        /// </summary>
        /// <returns>Pdf resources.</returns>
#if NETFX_CORE || WP
        public virtual PdfResources GetResources()
#else
        internal virtual PdfResources GetResources()
#endif
        {
            if (m_resources == null)
            {
                m_resources = new PdfResources();
                Dictionary[DictionaryProperties.Resources] = m_resources;
            }

            return m_resources;
        }

        /// <summary>
        /// Sets the resources.
        /// </summary>
        /// <param name="res">The resources object.</param>
        internal void SetResources(PdfResources res)
        {
            m_resources = res;
            m_modified = true;
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the wrapped element.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_pageDictionary;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the content of the page in form of a PDF template.
        /// </summary>
        /// <returns>A read-only PdfTemplate object that represents the content of the page
        /// (w/o annotations and other interactive elements).</returns>
        internal PdfTemplate GetContent()
        {
            m_modified = false;

            m_layersCount = m_layers == null ? 0 : m_layers.Count;
            m_annotCount = GetAnnotationCount();

            // Combine all content streams into a single one.
            MemoryStream ms = new MemoryStream();

            Layers.CombineContent(ms);

            m_pageContentLength = ms.Length;

            // Create read-only PdfTemplate object.
            // Copy resources and sizes.
            PdfDictionary resources = PdfCrossTable.Dereference(Dictionary[DictionaryProperties.Resources]) as PdfDictionary;
            PdfTemplate template = new PdfTemplate(Origin, Size, ms, resources);

            return template;
        }


        /// <summary>
        /// Gets the page orientation.
        /// </summary>
        /// <returns>The orientation of the page.</returns>
        private PdfPageOrientation GetOrientation()
        {
            PdfPageOrientation orientation = (Size.Width > Size.Height) ?
                PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;

            return orientation;
        }

        /// <summary>
        /// Clears PdfPageBase.
        /// </summary>
        internal virtual void Clear()
        {
#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (m_pageResources != null)
            {
                m_pageResources.Resources.Clear();
                m_pageResources = null;
            }
            m_graphicsState = null;
# endif
            if (m_layers != null)
                m_layers.Clear();
            m_layers = null;
            m_resources = null;
            m_pageDictionary = null;
            m_annotations = null;
            m_fontNames = null;
            m_fontReference = null;
            if (m_contentTemplate != null)
                m_contentTemplate = null;
        }

        /// <summary>
        /// Imports all annotations from a page.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="page">The page.</param>
        /// <param name="destinations">The destination list containing the destinations from annotations.</param>
        internal void ImportAnnotations(PdfLoadedDocument ldDoc, PdfPageBase page, List<PdfArray> destinations)
        {
            PdfArray lAnnots = page.GetAnnots();

            if (lAnnots != null)
            {
                PdfArray annots = new PdfArray();
                if (Dictionary.ContainsKey(DictionaryProperties.Annots))
                    annots = Dictionary[DictionaryProperties.Annots] as PdfArray;
                else
                    Dictionary[DictionaryProperties.Annots] = annots;

                foreach (IPdfPrimitive obj in lAnnots)
                {
                    PdfDictionary original = PdfCrossTable.Dereference(obj) as PdfDictionary;
                    bool isOptimized = false;
                    if (this is PdfPage)
                        isOptimized = (this as PdfPage).Section.ParentDocument.EnableMemoryOptimization;
                    else
                        isOptimized = (this as PdfLoadedPage).Document.EnableMemoryOptimization;

                    if (isOptimized)
                    {
                        // Formfields will be copied separately.
                        if (original.ContainsKey(DictionaryProperties.Subtype) && (original[DictionaryProperties.Subtype] as PdfName).Value == "Widget")
                            continue;

                        m_modified = true;

                        PdfDictionary temp = new PdfDictionary(original);
                        PdfArray dest = null;

                        if (temp.ContainsKey(DictionaryProperties.Dest))
                            dest = GetDestination(ldDoc, temp);
                        temp.Remove(DictionaryProperties.Dest);

                        if (temp.ContainsKey(DictionaryProperties.A) && temp[DictionaryProperties.A] is PdfReferenceHolder)
                        {
                            PdfDictionary dict = (temp[DictionaryProperties.A] as PdfReferenceHolder).Object as PdfDictionary;
                            dict.Remove(DictionaryProperties.AN);
                        }

                        // Remove optional dictionary that requires solving indirect reference to existing object.
                        temp.Remove("Popup");
                        temp.Remove(DictionaryProperties.P);
                        temp.Remove(DictionaryProperties.Parent);

                        PdfCrossTable crossTable = null;
                        if (this is PdfPage)
                            crossTable = (this as PdfPage).Section.ParentDocument.CrossTable;
                        else
                            crossTable = (this as PdfLoadedPage).Document.CrossTable;

                        // New Annotation.
                        PdfDictionary annot = temp.Clone(crossTable) as PdfDictionary;
                        PdfReferenceHolder rh = new PdfReferenceHolder(this);
                        annot.SetProperty(DictionaryProperties.P, rh);
                        annots.Add(new PdfReferenceHolder(annot));

                        if (dest != null)
                            annot[DictionaryProperties.Dest] = dest.Clone(crossTable);
                    }
                    else
                    {
                        PdfDictionary annot = new PdfDictionary(original);
                        annot.SetProperty(DictionaryProperties.P, new PdfReferenceHolder(this));
                        annots.Add(new PdfReferenceHolder(annot));

                        if (annot.ContainsKey(DictionaryProperties.Dest))
                        {
                            PdfArray dest = GetDestination(ldDoc, annot);

                            if (dest != null)
                            {
                                destinations.Add(dest);
                                annot[DictionaryProperties.Dest] = dest;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Imports all annotations from a page.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="page">The page.</param>
        internal void ImportAnnotations(PdfLoadedDocument ldDoc, PdfPageBase page)
        {
            PdfArray PageAnnotations = page.GetAnnots();

            if (PageAnnotations != null)
            {
                PdfArray AnnotationArray = new PdfArray();
                PdfReferenceHolder refHolder = new PdfReferenceHolder(this);
                Dictionary[DictionaryProperties.Annots] = AnnotationArray;

                m_modified = true;

                foreach (IPdfPrimitive obj in PageAnnotations)
                {
                    PdfDictionary AnnotationDictionary = new PdfDictionary(PdfCrossTable.Dereference(obj) as PdfDictionary);
                    AnnotationDictionary.SetProperty(DictionaryProperties.P, refHolder);
                    AnnotationArray.Add(new PdfReferenceHolder(AnnotationDictionary));
                }
            }
        }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <param name="ldDoc">The loaded document.</param>
        /// <param name="annotation">The annotation.</param>
        /// <returns>The destination dictionary.</returns>
        private PdfArray GetDestination(PdfLoadedDocument ldDoc, PdfDictionary annotation)
        {
            IPdfPrimitive obj = annotation[DictionaryProperties.Dest];

            obj = PdfCrossTable.Dereference(obj);

            PdfName name = obj as PdfName;
            PdfString stringName = obj as PdfString;
            PdfArray destination = obj as PdfArray;

            if (name != null)
            {
                destination = ldDoc.GetNamedDestination(name);
            }
            else if (stringName != null)
            {
                destination = ldDoc.GetNamedDestination(stringName);
            }

            if (destination != null)
                destination = new PdfArray(destination);

            return destination;
        }

        /// <summary>
        /// Gets the annotations array.
        /// </summary>
        /// <returns>The array of the annotations.</returns>
#if NETFX_CORE || WP
        public PdfArray GetAnnots()
#else
        internal PdfArray GetAnnots()
#endif
        {
            IPdfPrimitive obj = Dictionary.GetValue(DictionaryProperties.Annots, DictionaryProperties.Parent);
            PdfReferenceHolder rh = obj as PdfReferenceHolder;
            PdfArray annots;

            if (rh != null)
            {
                annots = rh.Object as PdfArray;
            }
            else
            {
                annots = obj as PdfArray;
            }

            return annots;
        }

        /// <summary>
        /// Returns number of annotations in the page.
        /// </summary>
        /// <returns></returns>
        private int GetAnnotationCount()
        {
            if (m_annotations != null)
                return m_annotations.Count;

            return 0;
        }

        /// <summary>
        /// Gets the page rotation.
        /// </summary>
        /// <returns>The rotation of the page.</returns>
        private PdfPageRotateAngle GetRotation()
        {
            int RotateFactor = 90;

            PdfDictionary parent = Dictionary;
            PdfNumber angle = null;

            while (parent != null && angle == null)
            {
                if (parent[DictionaryProperties.Rotate] is PdfReferenceHolder)
                    angle = (PdfNumber)(parent[DictionaryProperties.Rotate] as PdfReferenceHolder).Object;

                else
                    angle = (PdfNumber)parent[DictionaryProperties.Rotate];

                parent = PdfCrossTable.Dereference(parent[DictionaryProperties.Parent]) as PdfDictionary;
            }

            if (angle == null)
            {
                angle = new PdfNumber(0);
            }

            PdfPageRotateAngle rotateAngle = (PdfPageRotateAngle)(angle.IntValue / RotateFactor);

            return rotateAngle;
        }

        /// <summary>
        /// Draws the annotation templates.
        /// </summary>
        /// <param name="g">The graphics.</param>
        private void DrawAnnotationTemplates(PdfGraphics g)
        {
            PdfArray annots = GetAnnots();

            if (annots != null)
            {
                foreach (IPdfPrimitive obj in annots)
                {
                    PdfReferenceHolder rh = obj as PdfReferenceHolder;
                    PdfDictionary annotation;

                    if (rh != null)
                    {
                        annotation = rh.Object as PdfDictionary;
                    }
                    else
                    {
                        annotation = obj as PdfDictionary;
                    }

                    PdfTemplate template = GetAnnotTemplate(annotation);

                    if (template != null)
                    {
                        PointF location = GetAnnotationLocation(annotation);

                        location = NormalizeAnnotationLocation(location, g, template);
                        g.DrawPdfTemplate(template, location);
                    }
                }
            }
        }

        /// <summary>
        /// Normalizes the annotation location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="template">The annotation template.</param>
        /// <returns>The normalized location.</returns>
        /// <remarks>Annotation location treats low left page corner as coordinates origin.
        /// However, graphics translate the origin to the top left corner. This method makes
        /// location relative to the top left page corner.</remarks>
        private PointF NormalizeAnnotationLocation(PointF location, PdfGraphics graphics, PdfTemplate template)
        {
            location.Y = graphics.Size.Height - location.Y - template.Height;

            return location;
        }

        /// <summary>
        /// Gets the annotation location.
        /// </summary>
        /// <param name="annotation">The annotation dictionary.</param>
        /// <returns>The annotation location.</returns>
        private PointF GetAnnotationLocation(PdfDictionary annotation)
        {
            IPdfPrimitive obj = annotation[DictionaryProperties.Rect];
            PdfArray arrayRect = PdfCrossTable.Dereference(obj) as PdfArray;

            if (arrayRect == null)
                throw new PdfDocumentException("Invalid format: annotation dictionary doesn't contain rectangle array.");

            if (arrayRect.Count < 4)
                throw new PdfDocumentException("Invalid format: annotation rectangle has less then four elements.");

            PdfNumber numX1 = arrayRect[0] as PdfNumber;
            PdfNumber numY1 = arrayRect[1] as PdfNumber;
            PdfNumber numX2 = arrayRect[2] as PdfNumber;
            PdfNumber numY2 = arrayRect[3] as PdfNumber;

            float numX = Math.Min(numX1.FloatValue, numX2.FloatValue);
            float numY = Math.Min(numY1.FloatValue, numY2.FloatValue);

            PointF location = new PointF(numX, numY);

            return location;
        }

        /// <summary>
        /// Gets the size of the annotation.
        /// </summary>
        /// <param name="annotation">The annotation dictionary.</param>
        /// <returns>The size of the annotation.</returns>
        private SizeF GetAnnotationSize(PdfDictionary annotation)
        {
            return GetElementSize(annotation, DictionaryProperties.Rect);
        }

        /// <summary>
        /// Gets the size of the interactive element.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="propertyName">Name of the property containing size rectangle (e.g. Rect, BBox).</param>
        /// <returns>The size of the annotation.</returns>
        private SizeF GetElementSize(PdfDictionary dictionary, string propertyName)
        {
            IPdfPrimitive obj = dictionary[propertyName];
            PdfArray arrayRect = PdfCrossTable.Dereference(obj) as PdfArray;

            if (arrayRect == null)
                throw new PdfDocumentException("Invalid format: dictionary doesn't contain rectangle array.");

            if (arrayRect.Count < 4)
                throw new PdfDocumentException("Invalid format: rectangle array has less then four elements.");

            PdfNumber numX1 = arrayRect[0] as PdfNumber;
            PdfNumber numY1 = arrayRect[1] as PdfNumber;
            PdfNumber numX2 = arrayRect[2] as PdfNumber;
            PdfNumber numY2 = arrayRect[3] as PdfNumber;

            float numW = Math.Abs(numX1.FloatValue - numX2.FloatValue);
            float numH = Math.Abs(numY1.FloatValue - numY2.FloatValue);

            SizeF size = new SizeF(numW, numH);

            return size;
        }

        /// <summary>
        /// Gets the annotation template.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns>The annotation template.</returns>
        private PdfTemplate GetAnnotTemplate(PdfDictionary annotation)
        {
            IPdfPrimitive obj = annotation[DictionaryProperties.AP];
            PdfDictionary dic = PdfCrossTable.Dereference(obj) as PdfDictionary;

            PdfTemplate template = null;

            if (dic != null)
            {
                obj = dic[DictionaryProperties.N];
                dic = PdfCrossTable.Dereference(obj) as PdfDictionary;

                if (dic != null)
                {
                    PdfStream stream = dic as PdfStream;

                    if (stream == null)
                    {
                        PdfName key = null;

                        // Check if the Appearance State (AS) entry present
                        if (annotation.ContainsKey(DictionaryProperties.AS))
                        {
                            key = PdfCrossTable.Dereference(annotation[DictionaryProperties.AS]) as PdfName;
                        }
                        else
                        {
                            IEnumerator keys = dic.Keys.GetEnumerator();

                            if (keys.MoveNext()) // Get the first entry.
                            {
                                key = keys.Current as PdfName;
                            }
                        }

                        if (key != null)
                        {
                            stream = PdfCrossTable.Dereference(dic[key]) as PdfStream;
                        }
                    }

                    if (stream != null)
                    {
                        obj = stream[DictionaryProperties.Resources];
                        PdfDictionary res = PdfCrossTable.Dereference(obj) as PdfDictionary;
                        SizeF size = GetElementSize(stream, DictionaryProperties.BBox);

                        template = new PdfTemplate(size, stream.InternalStream, res);
                    }
                }
            }

            return template;
        }
        #endregion
    }
}
