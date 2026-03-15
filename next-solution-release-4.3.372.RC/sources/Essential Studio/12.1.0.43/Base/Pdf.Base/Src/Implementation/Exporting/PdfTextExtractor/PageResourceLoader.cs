#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.IO;
using System.Collections;
using Syncfusion.Pdf.Primitives;
using System.IO;
using System.Drawing;

namespace Syncfusion.Pdf
{
    internal sealed class PageResourceLoader
    {
        static PageResourceLoader s_instance;
        static object s_lock = new object();

        public static PageResourceLoader Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (s_lock)
                    {
                        s_instance = new PageResourceLoader();
                    }
                }

                return s_instance;
            }
        }
        /// <summary>
        /// Extracts the pageResource from the page
        /// </summary>
        /// <param name="page">Page whose resouce is needed</param>
        /// <returns>pageResource of the given page</returns>
        public PdfPageResources GetPageResources(PdfPageBase page)
        {
            PdfPageResources pageResources = new PdfPageResources();

            PdfDictionary resources = page.GetResources();
            PdfArray annots = page.GetAnnots();
            Dictionary<string, PdfMatrix> commonMatrix = new Dictionary<string, PdfMatrix>();
            pageResources = UpdatePageResources(pageResources, GetFontResources(resources as PdfDictionary, page));
            pageResources = UpdatePageResources(pageResources, GetImageResources(resources as PdfDictionary, page, ref commonMatrix));
            pageResources = UpdatePageResources(pageResources, GetExtendedGraphicResources(resources as PdfDictionary));
            if (annots != null)
            {
                pageResources.Add("Annotations", annots);
            }

            while ( resources!= null && resources.ContainsKey(DictionaryProperties.XObject))
            {
                PdfDictionary xobjects;
                if (resources[DictionaryProperties.XObject] is PdfReferenceHolder)
                    xobjects = (resources[DictionaryProperties.XObject] as PdfReferenceHolder).Object as PdfDictionary;
                else
                    xobjects = resources[DictionaryProperties.XObject] as PdfDictionary;
                resources = xobjects[DictionaryProperties.Resources] as PdfDictionary;
                foreach (KeyValuePair<PdfName, IPdfPrimitive> objects in xobjects.Items)
                {
                    PdfDictionary xobjectDictionary;
                    if (objects.Value is PdfReferenceHolder)
                    {
                        xobjectDictionary = (objects.Value as PdfReferenceHolder).Object as PdfDictionary;
                    }
                    else
                    {
                        xobjectDictionary = objects.Value as PdfDictionary;
                    }
                    if (xobjectDictionary.ContainsKey(DictionaryProperties.Resources))
                    {
                        if (xobjectDictionary[DictionaryProperties.Resources] is PdfReferenceHolder)
                        {
                            resources = (xobjectDictionary[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary;
                        }
                        else
                            resources = xobjectDictionary[DictionaryProperties.Resources] as PdfDictionary;
                        pageResources = UpdatePageResources(pageResources, GetFontResources(resources as PdfDictionary, page));
                        pageResources = UpdatePageResources(pageResources, GetImageResources(resources as PdfDictionary, page, ref commonMatrix));

                    }
                    //else
                    //{
                    //    resources = new PdfDictionary();
                    //    break;
                    //}
                }
            }
            if (page.Rotation == PdfPageRotateAngle.RotateAngle90)
                pageResources.Add(DictionaryProperties.Rotate, (float)90);
            else if (page.Rotation == PdfPageRotateAngle.RotateAngle180)
                pageResources.Add(DictionaryProperties.Rotate, (float)180);
            else if (page.Rotation == PdfPageRotateAngle.RotateAngle270)
                pageResources.Add(DictionaryProperties.Rotate, (float)270);
            return pageResources;
        }
        /// <summary>
        /// Collects all the ExtendedGraphicsSatate elements in the pdf document
        /// </summary>
        /// <param name="resourceDictionary">containing all the resources of the document</param>
        /// <returns>dictionary of ExtGState elements</returns>
        internal Dictionary<string, object> GetExtendedGraphicResources(PdfDictionary resourceDictionary)
        {
            Dictionary<string, object> pageResources = new Dictionary<string, object>();
            
            if (resourceDictionary != null && resourceDictionary.ContainsKey(DictionaryProperties.ExtGState))
            {
                IPdfPrimitive XExtGStateObjects;
                if (resourceDictionary[DictionaryProperties.ExtGState] is PdfDictionary)
                    XExtGStateObjects = resourceDictionary[DictionaryProperties.ExtGState];
                else
                    XExtGStateObjects = (resourceDictionary[DictionaryProperties.ExtGState] as PdfReferenceHolder).Object;

                if (XExtGStateObjects is PdfDictionary)
                {
                    Dictionary<PdfName, IPdfPrimitive> resC = ((PdfDictionary)XExtGStateObjects).Items;
                    foreach (KeyValuePair<PdfName, IPdfPrimitive> XExtGStateObject in resC)
                    {
                        PdfDictionary objectDictionary;
                        if (XExtGStateObject.Value is PdfReferenceHolder)
                        {
                            objectDictionary = (XExtGStateObject.Value as PdfReferenceHolder).Object as PdfDictionary;
                            pageResources.Add(XExtGStateObject.Key.Value, new XObjectElement(objectDictionary, XExtGStateObject.Key.Value));
                        }
                        else
                        {
                            objectDictionary = XExtGStateObject.Value as PdfDictionary;
                            pageResources.Add(XExtGStateObject.Key.Value, new XObjectElement(objectDictionary, XExtGStateObject.Key.Value));
                        }
                    }
                }
            }

            return pageResources;
        }
        
        // <summary>
        /// <summary>
        /// Collects all the fonts in the page in a dictionary
        /// </summary>
        /// <param name="resourceDictionary">dictionary containing all the resources in the Xobjects</param>
        /// <returns>dictionary containing font name and the font</returns>
        internal Dictionary<string, object> GetFontResources(PdfDictionary resourceDictionary)
        {
            Dictionary<string, object> pageResources = new Dictionary<string, object>();
            if (resourceDictionary != null)
            {
                IPdfPrimitive fonts = resourceDictionary[DictionaryProperties.Font];            
                if (fonts != null)
                {
                    PdfDictionary fontsDictionary;
                    if (fonts is PdfReferenceHolder)
                        fontsDictionary = (fonts as PdfReferenceHolder).Object as PdfDictionary;
                    else
                        fontsDictionary = fonts as PdfDictionary;

                    if (fontsDictionary != null)
                    {
                        foreach (KeyValuePair<PdfName, IPdfPrimitive> item in fontsDictionary.Items)
                        {
                            if (item.Value is PdfReferenceHolder)
                            {
                                pageResources.Add(
                                    item.Key.Value, new FontStructure((item.Value as PdfReferenceHolder).Object));
                            }
                            else
                                pageResources.Add(item.Key.Value, new FontStructure(item.Value));
                        }
                    }
                }
            }
            return pageResources;
        }
        /// <summary>
        /// Extracts the text from the page given
        /// </summary>
        /// <param name="page">page from which text is extracted</param>
        /// <param name="fontName">font used in the text</param>
        /// <param name="textToDecode">text in the page to be decoded</param>
        /// <returns>decoded text</returns>
        public string DecodeTest(PdfPageBase page, string fontName, string textToDecode)
        {
            PdfPageResources resources = GetPageResources(page);
            byte[] bytesToDecode = System.Text.Encoding.Default.GetBytes(textToDecode);
            object font = resources[fontName];

            //if (font != null)
            //    return null;

            return (font as FontStructure).Decode(textToDecode,resources.isSameFont());
        }
        /// <summary>
        /// Collects all the fonts in the page in a dictionary
        /// </summary>
        /// <param name="resourceDictionary">dictionary containing all the resources in the page</param>
        /// <param name="page">page in which text is to be extracted</param>
        /// <returns>dictionary containing font name and the font</returns>
        internal Dictionary<string, object> GetFontResources(PdfDictionary resourceDictionary, PdfPageBase page)
        {
                Dictionary<string, object> pageResources = new Dictionary<string, object>();

                if (resourceDictionary != null)
                {
                    IPdfPrimitive fonts = resourceDictionary[DictionaryProperties.Font];

                    if (fonts != null)
                    {
                        PdfDictionary fontsDictionary;
                        if (fonts is PdfReferenceHolder)
                            fontsDictionary = (fonts as PdfReferenceHolder).Object as PdfDictionary;
                        else
                            fontsDictionary = fonts as PdfDictionary;

                        if (fontsDictionary != null)
                        {
                            foreach (KeyValuePair<PdfName, IPdfPrimitive> item in fontsDictionary.Items)
                            {
                                if (item.Value is PdfReferenceHolder)
                                {
                                    pageResources.Add(
                                        item.Key.Value, new FontStructure((item.Value as PdfReferenceHolder).Object));
                                }
                                else
                                    pageResources.Add(item.Key.Value, new FontStructure(item.Value));
                            }
                        }
                    }

                    IPdfPrimitive parentPage = page.Dictionary[DictionaryProperties.Parent];
                    if (parentPage != null)
                    {
                        IPdfPrimitive parentRef = (parentPage as PdfReferenceHolder).Object;
                        PdfResources parentResources = new PdfResources(parentRef as PdfDictionary);

                        fonts = parentResources[DictionaryProperties.Font];
                        if (fonts != null)
                        {
                            PdfDictionary fontsDictionary = (fonts as PdfDictionary);
                            if (fontsDictionary != null)
                            {
                                foreach (KeyValuePair<PdfName, IPdfPrimitive> item in fontsDictionary.Items)
                                {
                                    if (item.Value is PdfDictionary)
                                    {
                                        pageResources.Add(
                                            item.Key.Value, (item.Value as PdfReferenceHolder).Object);
                                    }
                                    pageResources.Add(item.Key.Value, new FontStructure(item.Value));
                                }
                            }
                        }
                    }
                }
             return pageResources;            
        }
        /// <summary>
        /// Collects all the images in the pdf document
        /// </summary>
        /// <param name="resourceDictionary">containing all the resources of the document</param>
        /// <returns>dictionary of images</returns>
        internal Dictionary<string, object> GetImageResources(PdfDictionary resourceDictionary, PdfPageBase page, ref Dictionary<string, PdfMatrix> commonMatrix)
        {
            Dictionary<string, object> pageResources = new Dictionary<string, object>();
            if (resourceDictionary != null && resourceDictionary.ContainsKey(DictionaryProperties.XObject))
            {
                IPdfPrimitive XObjects;
                if (resourceDictionary[DictionaryProperties.XObject] is PdfDictionary)
                    XObjects = resourceDictionary[DictionaryProperties.XObject];
                else
                    XObjects = (resourceDictionary[DictionaryProperties.XObject] as PdfReferenceHolder).Object;

                if (XObjects is PdfDictionary)
                {
                    Dictionary<PdfName, IPdfPrimitive> resC = ((PdfDictionary)XObjects).Items;
                    foreach (KeyValuePair<PdfName, IPdfPrimitive> xObject in resC)
                    {
                        PdfDictionary objectDictionary;
                        if (xObject.Value is PdfReferenceHolder)
                        {
                            objectDictionary = (xObject.Value as PdfReferenceHolder).Object as PdfDictionary;
                            if (objectDictionary.ContainsKey("Subtype"))
                            {
                                if ((objectDictionary[DictionaryProperties.Subtype] as PdfName).Value == "Image")
                                {
                                    ImageStructure imgStruct = new ImageStructure(objectDictionary, new PdfMatrix());
                                    if (commonMatrix.ContainsKey(xObject.Key.Value))
                                    {
                                        imgStruct = new ImageStructure(objectDictionary, commonMatrix[xObject.Key.Value]);
                                    }
                                    pageResources.Add(xObject.Key.Value, imgStruct);
                                }
                                else if ((objectDictionary[DictionaryProperties.Subtype] as PdfName).Value == "Form" && page != null)
                                {
                                    if (objectDictionary.ContainsKey(DictionaryProperties.Resources))
                                    {
                                        if (objectDictionary[DictionaryProperties.Resources] is PdfDictionary)
                                        {
                                            PdfDictionary resource = objectDictionary[DictionaryProperties.Resources] as PdfDictionary;
                                            foreach (KeyValuePair<PdfName, IPdfPrimitive> objects in resource.Items)
                                            {
                                                if (objects.Key.Value == DictionaryProperties.XObject)
                                                {
                                                    if (objects.Value is PdfDictionary)
                                                    {
                                                        Dictionary<PdfName, IPdfPrimitive> imageName = (objects.Value as PdfDictionary).Items;
                                                        foreach (KeyValuePair<PdfName, IPdfPrimitive> image in imageName) 
                                                        {
                                                            PdfStream str = objectDictionary as PdfStream;
                                                            str.Decompress();
                                                            MemoryStream mstream = str.InternalStream;
                                                           mstream.Position = 0;
                                                            PdfReader contentReader = new PdfReader(mstream);
                                                            contentReader.Position = 0;
                                                            PdfMatrix coOrdinates = new PdfMatrix(contentReader, image.Key.Value, page.Size);
                                                            if (!commonMatrix.ContainsKey(image.Key.Value))
                                                                commonMatrix.Add(image.Key.Value, coOrdinates);
                                                        } 
                                                        
                                                    }
                                                }
                                            }
                                        }
                                        
                                    }
                                    pageResources.Add(xObject.Key.Value, new XObjectElement(objectDictionary, xObject.Key.Value));
                                }
                                if (!pageResources.ContainsKey(xObject.Key.Value))
                                {
                                    pageResources.Add(xObject.Key.Value, new XObjectElement(objectDictionary, xObject.Key.Value));
                                }
                            }

                        }
                        else
                        {
                            objectDictionary = xObject.Value as PdfDictionary;
                            //if (objectDictionary.ContainsKey("Subtype"))
                            //{
                            //    if ((objectDictionary[DictionaryProperties.Subtype] as PdfName).Value == "Image")
                            //    {
                            pageResources.Add(xObject.Key.Value, new XObjectElement(objectDictionary, xObject.Key.Value));
                            //    }
                            //}
                        }
                    }
                }
            }

            return pageResources;
        }
        /// <summary>
        /// Updates the resources in the page
        /// </summary>
        /// <param name="pageResources">Existing page resources</param>
        /// <param name="objects">Dictionary items to the updated</param>
        /// <returns>Updated page resource</returns>
        internal PdfPageResources UpdatePageResources(PdfPageResources pageResources, Dictionary<string, object> objects)
        {
            foreach (KeyValuePair<string, object> xobject in objects)
            {
                pageResources.Add(xobject.Key, xobject.Value);
            }
                
            return pageResources;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class PdfPageResources
    {
        private Dictionary<string, object> m_resources;
        Dictionary<string, FontStructure> fontCollection = new Dictionary<string, FontStructure>();

        /// <summary>
        /// Returns the resources.
        /// </summary>
        public Dictionary<string, object> Resources
        {
            get
            {
                return m_resources;
            }
        }

        /// <summary>
        /// Returns the value associated with the key.
        /// </summary>
        public object this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                if (m_resources.ContainsKey(key))
                    return m_resources[key];
                else
                    return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (key == null)
                    throw new ArgumentNullException("key");

                m_resources[key] = value;
            }
        }
        /// <summary>
        /// Returns if the FontCollection has same font face.
        /// </summary>
        /// <returns></returns>
        public bool isSameFont()
        {
            int i = 0;
            
            foreach (KeyValuePair<string, FontStructure> item in fontCollection)
            {
                foreach (KeyValuePair<string, FontStructure> nextItem in fontCollection)
                {
                    if (item.Value.FontName != nextItem.Value.FontName)
                    {
                        i = 1;
                    }
                }

            }

            if (i == 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Initializes the class.
        /// </summary>
        public PdfPageResources()
        {
            m_resources = new Dictionary<string, object>();
        }

        /// <summary>
        /// Adds the resource with the specified name.
        /// </summary>
        /// <param name="resourceName">Name of the resource</param>
        /// <param name="resource">Resource to add</param>
        public void Add(string resourceName, object resource)
        {
            if (string.Equals(resourceName, "ProcSet"))
                return;

            if (!m_resources.ContainsKey(resourceName))
            {
                m_resources.Add(resourceName, resource);
                
                if (resource.GetType().Name == "FontStructure")
                {
                    fontCollection.Add(resourceName, resource as FontStructure);
                }
            }
        }

        /// <summary>
        /// Returns if the key already exists.
        /// </summary>
        public bool ContainsKey(string key)
        {
            return m_resources.ContainsKey(key);
        }

     
    }
}
