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
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.IO;

#if WINDOWS || MVC
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.PdfViewer.Base;
using System.Globalization;
using System.Threading;
#elif WPF
using System.Windows;
using Syncfusion.Windows.PdfViewer;
using Syncfusion.PdfViewer.WPF;
using Syncfusion.PdfViewer.Base;
#endif


namespace Syncfusion.Pdf
{
    internal class XObjectElement
    {
        private string m_objectName;
        private string m_objectType;
        private PdfMatrix m_imageInfo;
        private PdfDictionary m_xObjectDictionary;


        internal string ObjectName
        {
            get
            {
                return m_objectName;
            }
            set
            {
                m_objectName = value;
            }
        }
        internal PdfMatrix ImageInfo
        {
            get
            {
                return m_imageInfo;
            }
            set
            {
                m_imageInfo = value;
            }
        }
        internal PdfDictionary XObjectDictionary
        {
            get
            {
                return m_xObjectDictionary;
            }
            set
            {
                m_xObjectDictionary = value;
            }
        }
        internal string ObjectType
        {
            get
            {
                return m_objectType;
            }
            set
            {
                m_objectType = value;
            }
        }
        


        public XObjectElement(PdfDictionary xobjectDictionary, string name)
        {
            m_xObjectDictionary = xobjectDictionary;
            this.m_objectName = name;
            GetObjectType();            
        }
        public XObjectElement(PdfDictionary xobjectDictionary, string name, PdfMatrix tm)
        {
            m_xObjectDictionary = xobjectDictionary;
            this.m_objectName = name;
            ImageInfo = tm;
            GetObjectType();
        }
        public PdfRecordCollection Render(PdfPageResources resources, Stack<System.Drawing.Drawing2D.GraphicsState> graphicsStates)
        {
            if (this.ObjectType == "Form")
            {
                PdfStream stream = m_xObjectDictionary as PdfStream;
                stream.Decompress();
                ContentParser parser = new ContentParser(stream.InternalStream.ToArray());
                PdfRecordCollection contentTree = parser.ReadContent();
                return contentTree;
            }
            else
                return null;
        }
#if WINDOWS || MVC
        public Stack<GraphicsState> Render(System.Drawing.Graphics g, PdfPageResources resources, Stack<GraphicsState> graphicsStates)
        {
            if (this.ObjectType == "Form")
            {
                PdfStream stream = m_xObjectDictionary as PdfStream;
                stream.Decompress();
                ContentParser parser = new ContentParser(stream.InternalStream.ToArray());
                PdfRecordCollection contentTree = parser.ReadContent();
                PageResourceLoader resourceLoader = new PageResourceLoader();
                PdfDictionary pageDictionary = new PdfDictionary();
                PdfDictionary xobjects = this.XObjectDictionary;
                PdfPageResources childResource = new PdfPageResources();

                if (xobjects.ContainsKey(DictionaryProperties.Resources))
                {
                    if (xobjects[DictionaryProperties.Resources] is PdfReference)
                        pageDictionary = ((xobjects[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary);
                    else if (xobjects[DictionaryProperties.Resources] is PdfReferenceHolder)
                        pageDictionary = ((xobjects[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary);
                    else
                        pageDictionary = xobjects[DictionaryProperties.Resources] as PdfDictionary;
                    Dictionary<string, PdfMatrix> commonMatrix = new Dictionary<string, PdfMatrix>();
                    childResource = resourceLoader.UpdatePageResources(childResource, resourceLoader.GetImageResources(pageDictionary,null,ref commonMatrix));
                    childResource = resourceLoader.UpdatePageResources(childResource, resourceLoader.GetFontResources(pageDictionary));
                    childResource = resourceLoader.UpdatePageResources(childResource, resourceLoader.GetExtendedGraphicResources(pageDictionary));
                }

                if (xobjects.ContainsKey(DictionaryProperties.Matrix))
                {
                    PdfArray matrixArray = new PdfArray();
                    if (xobjects[DictionaryProperties.Matrix] is PdfArray)
                    {
                        matrixArray = (xobjects[DictionaryProperties.Matrix] as PdfArray);
                        if (matrixArray != null)
                        {
                            float a = (matrixArray[0] as PdfNumber).FloatValue;
                            float b = (matrixArray[1] as PdfNumber).FloatValue;
                            float c = (matrixArray[2] as PdfNumber).FloatValue;
                            float d = (matrixArray[3] as PdfNumber).FloatValue;
                            float e = (matrixArray[4] as PdfNumber).FloatValue;
                            float f = (matrixArray[5] as PdfNumber).FloatValue;
                            if (e != 0 || f != 0)
                            {
                                g.TranslateTransform(e, -f);
                             }
                             if(a!=0 || d!=0)
                             {
                                 g.ScaleTransform(a, d);
                             }

                            //check for rotate transform
                            double rad = Math.Acos(a);
                            double degree = Math.Round((180 / Math.PI) * rad);

                            double checkRad = Math.Asin(b);
                            double checkDegree = Math.Round((180 / Math.PI) * checkRad);

                            if (degree == checkDegree)
                            {
                                g.RotateTransform(-(float)degree);
                            }
                            else
                            {
                                if (!double.IsNaN(checkDegree))
                                {
                                    g.RotateTransform(-(float)(checkDegree));
                                }
                                else
                                {
                                    if (!double.IsNaN(degree))
                                        g.RotateTransform(-(float)degree);
                                }
                            }
                        }
                    }
                }

                if (pageDictionary.Count != 0)
                {
                    DeviceCMYK cmyk=new DeviceCMYK();
                    ImageRenderer renderer = new ImageRenderer(contentTree, childResource, g, false, cmyk);
                    CultureInfo current = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    renderer.RenderAsImage();
                    Thread.CurrentThread.CurrentCulture = current;
                }
            }
            return graphicsStates;

        }
        
#elif WPF
        public Stack<GraphicsState> Render(WPFGraphics g, PdfPageResources resources, Stack<GraphicsState> graphicsStates)
        {
            if (this.ObjectType == "Form")
            {
                PdfStream stream = m_xObjectDictionary as PdfStream;
                stream.Decompress();
                ContentParser parser = new ContentParser(stream.InternalStream.ToArray());
                PdfRecordCollection contentTree = parser.ReadContent();

                PageResourceLoader resourceLoader = new PageResourceLoader();
                PdfDictionary pageDictionary = new PdfDictionary();
                PdfDictionary xobjects = this.XObjectDictionary;
                PdfPageResources childResource = new PdfPageResources();
                if (xobjects.ContainsKey(DictionaryProperties.Resources))
                {
                    if (xobjects[DictionaryProperties.Resources] is PdfReference)
                        pageDictionary = ((xobjects[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary);
                    else
                    {
                        pageDictionary = (xobjects[DictionaryProperties.Resources]) as PdfDictionary;
                        if (pageDictionary == null)
                        {
                            pageDictionary = (xobjects[DictionaryProperties.Resources] as PdfReferenceHolder).Object as PdfDictionary;
                        }
                    }

                    Dictionary<string, PdfMatrix> commonMatrix = new Dictionary<string, PdfMatrix>();
                    childResource = resourceLoader.UpdatePageResources(childResource, resourceLoader.GetImageResources(pageDictionary,null,ref commonMatrix));
                    childResource = resourceLoader.UpdatePageResources(childResource, resourceLoader.GetFontResources(pageDictionary));
                    childResource = resourceLoader.UpdatePageResources(childResource, resourceLoader.GetExtendedGraphicResources(pageDictionary));
                }

                if (xobjects.ContainsKey(DictionaryProperties.Matrix))
                {
                    PdfArray matrixArray = new PdfArray();
                    if (xobjects[DictionaryProperties.Matrix] is PdfArray)
                    {
                        matrixArray = (xobjects[DictionaryProperties.Matrix] as PdfArray);
                        if (matrixArray != null)
                        {
                            float a = (matrixArray[0] as PdfNumber).FloatValue;
                            float b = (matrixArray[1] as PdfNumber).FloatValue;
                            float c = (matrixArray[2] as PdfNumber).FloatValue;
                            float d = (matrixArray[3] as PdfNumber).FloatValue;
                            float e = (matrixArray[4] as PdfNumber).FloatValue;
                            float f = (matrixArray[5] as PdfNumber).FloatValue;
                            if (e != 0 || f != 0)
                            {
                                g.PushTranslateTransform(e, f, true);
                            }
                            if(a!=0 || d!=0)
                            {
                                g.PushScaleTransform(a, d);
                            }
                        }
                    }
                }
                if (pageDictionary.Count != 0)
                {
                    WPFRenderer renderer = new WPFRenderer(contentTree, childResource, g, Rect.Empty, false);
                    System.Globalization.CultureInfo current = System.Threading.Thread.CurrentThread.CurrentCulture;
                    System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                    renderer.Render();
                    System.Threading.Thread.CurrentThread.CurrentCulture = current;
                }
            }
            return graphicsStates;

        }
#endif
        private void GetObjectType()
        {
            if (m_xObjectDictionary.ContainsKey(DictionaryProperties.Subtype))
            {
                m_objectType = (m_xObjectDictionary[DictionaryProperties.Subtype] as PdfName).Value;
            }
        }



    }
}
