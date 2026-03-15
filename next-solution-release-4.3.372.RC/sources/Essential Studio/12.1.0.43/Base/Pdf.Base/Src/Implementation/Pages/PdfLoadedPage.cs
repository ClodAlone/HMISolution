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
using System.Globalization;
using System.IO;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;


namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents a page loaded from a document.
    /// </summary>
    public class PdfLoadedPage :
        PdfPageBase
    {
        #region Members
        private PdfCrossTable m_crossTable;
        private bool m_bCheckResources;
        private PdfDocumentBase m_document;
        /// <summary>
        /// Dictionaries of terminal annotation.
        /// </summary>
        private List<PdfDictionary> m_terminalannots = new List<PdfDictionary>();

        /// <summary>
        /// Collection of annotations.
        /// </summary>
        private PdfLoadedAnnotationCollection m_annots;

       
        internal static bool m_annotChanged = false;

        /// <summary>
        /// Holds the reference numbers of the widget annotations
        /// </summary>
        private List<long> m_widgetReferences;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the field collection.
        /// </summary>
        public new PdfLoadedAnnotationCollection Annotations
        {
            get
            {
                if (m_annots == null)
                {
                    m_annots = new PdfLoadedAnnotationCollection(this);
                }

                return m_annots;
            }
            set
            {
                m_annots = value;
            }
        }
        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        public override SizeF Size
        {
            get
            {
                PdfArray mBox = Dictionary.GetValue(CrossTable,
                    DictionaryProperties.MediaBox, DictionaryProperties.Parent) as PdfArray;

                //mBox[ 2 ] - mBox[ 0 ]
                //mBox[ 3 ] - mBox[ 1 ]
                float width = (mBox[2] as PdfNumber).FloatValue;
                float height = (mBox[3] as PdfNumber).FloatValue != 0 ? (mBox[3] as PdfNumber).FloatValue : (mBox[1] as PdfNumber).FloatValue;

                //width -= (mBox[0] as PdfNumber).FloatValue;
                //height -= (mBox[1] as PdfNumber).FloatValue;

                //swap height and width if page dictionary contains rotation angle
                //if (Dictionary.ContainsKey(DictionaryProperties.Rotate))
                //{
                //    if ((Dictionary.GetValue(CrossTable,
                //    DictionaryProperties.Rotate, DictionaryProperties.Parent) as PdfNumber).FloatValue == 90)
                //    {
                //        width = height + width;
                //        height = width - height;
                //        width = width - height;
                //    }
                //}
                //else
                //{
                //    if (CrossTable.DocumentCatalog != null)
                //    {
                //        PdfDictionary pageDictionary = CrossTable.DocumentCatalog as PdfDictionary;

                //        if (pageDictionary.ContainsKey(DictionaryProperties.Pages))
                //        {
                //            pageDictionary = (pageDictionary[DictionaryProperties.Pages] as PdfReferenceHolder).Object as PdfDictionary;
                //            if (pageDictionary.ContainsKey(DictionaryProperties.Rotate))
                //            {
                //                if ((pageDictionary.GetValue(CrossTable,
                //                DictionaryProperties.Rotate, DictionaryProperties.Pages) as PdfNumber).FloatValue == 90)
                //                {
                //                    width = height + width;
                //                    height = width - height;
                //                    width = width - height;
                //                }
                //            }
                //        }
                //    }

                //}
    

                SizeF size = new SizeF(width, height);

                return size;
            }
        }

        /// <summary>
        /// Returns the visible region of the page.
        /// </summary>
        public RectangleF CropBox
        {
            get
            {
                RectangleF rect = RectangleF.Empty;
                if (Dictionary.ContainsKey(DictionaryProperties.CropBox))
                {
                    PdfArray mBox = Dictionary.GetValue(CrossTable,
                        DictionaryProperties.CropBox, DictionaryProperties.Parent) as PdfArray;

                    float width = (mBox[2] as PdfNumber).FloatValue;
                    float height = (mBox[3] as PdfNumber).FloatValue != 0 ? (mBox[3] as PdfNumber).FloatValue : (mBox[1] as PdfNumber).FloatValue;
                    float x = (mBox[0] as PdfNumber).FloatValue;
                    float y = (mBox[1] as PdfNumber).FloatValue;

                    rect = new RectangleF(new PointF(x, y), new SizeF(width, height));
                }

                return rect;
            }
        }

        /// <summary>
        /// Returns page region after clipping.
        /// </summary>
        public RectangleF BleedBox
        {
            get
            {
                RectangleF rect = RectangleF.Empty;
                if (Dictionary.ContainsKey(DictionaryProperties.BleedBox))
                {
                    PdfArray mBox = Dictionary.GetValue(CrossTable,
                        DictionaryProperties.BleedBox, DictionaryProperties.Parent) as PdfArray;

                    float width = (mBox[2] as PdfNumber).FloatValue;
                    float height = (mBox[3] as PdfNumber).FloatValue != 0 ? (mBox[3] as PdfNumber).FloatValue : (mBox[1] as PdfNumber).FloatValue;
                    float x = (mBox[0] as PdfNumber).FloatValue;
                    float y = (mBox[1] as PdfNumber).FloatValue;

                    rect = new RectangleF(new PointF(x, y), new SizeF(width, height));
                }

                return rect;
            }
        }

        /// <summary>
        /// Returns page region after trimming.
        /// </summary>
        public RectangleF TrimBox
        {
            get
            {
                RectangleF rect = RectangleF.Empty;
                if (Dictionary.ContainsKey(DictionaryProperties.TrimBox))
                {
                    PdfArray mBox = Dictionary.GetValue(CrossTable,
                        DictionaryProperties.TrimBox, DictionaryProperties.Parent) as PdfArray;

                    float width = (mBox[2] as PdfNumber).FloatValue;
                    float height = (mBox[3] as PdfNumber).FloatValue != 0 ? (mBox[3] as PdfNumber).FloatValue : (mBox[1] as PdfNumber).FloatValue;
                    float x = (mBox[0] as PdfNumber).FloatValue;
                    float y = (mBox[1] as PdfNumber).FloatValue;

                    rect = new RectangleF(new PointF(x, y), new SizeF(width, height));
                }

                return rect;
            }
        }

        /// <summary>
        /// Returns page region containing meaningful content.
        /// </summary>
        public RectangleF ArtBox
        {
            get
            {
                RectangleF rect = RectangleF.Empty;
                if (Dictionary.ContainsKey(DictionaryProperties.ArtBox))
                {
                    PdfArray mBox = Dictionary.GetValue(CrossTable,
                        DictionaryProperties.ArtBox, DictionaryProperties.Parent) as PdfArray;

                    float width = (mBox[2] as PdfNumber).FloatValue;
                    float height = (mBox[3] as PdfNumber).FloatValue != 0 ? (mBox[3] as PdfNumber).FloatValue : (mBox[1] as PdfNumber).FloatValue;
                    float x = (mBox[0] as PdfNumber).FloatValue;
                    float y = (mBox[1] as PdfNumber).FloatValue;

                    rect = new RectangleF(new PointF(x, y), new SizeF(width, height));
                }

                return rect;
            }
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        public PdfDocumentBase Document
        {
            get
            {
                return m_document;
            }
        }

        /// <summary>
        /// Gets the cross table.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
        }

        /// <summary>
        /// Gets the resources and modifies the page dictionary.
        /// </summary>
        /// <returns>Pdf resources.</returns>
#if NETFX_CORE || WP
        public override PdfResources GetResources()
#else
        internal override PdfResources GetResources()
#endif
        {
            PdfResources resources = null;

            if (!Dictionary.ContainsKey(DictionaryProperties.Resources) || m_bCheckResources)
            {
                resources = base.GetResources();

               // Check for the resources in the corresponding page section.
                if (resources.GetNames().Count == 0
                    || resources.Items.Count == 0)
                {
                    if (Dictionary.ContainsKey(DictionaryProperties.Parent))
                    {
                        IPdfPrimitive obj = Dictionary[DictionaryProperties.Parent];
                        PdfDictionary parentDic = null;
                        if (obj is PdfReferenceHolder)
                            parentDic = ((obj as PdfReferenceHolder).Object as PdfDictionary);
                        else
                            parentDic = (obj as PdfDictionary);

                        if (parentDic.ContainsKey(DictionaryProperties.Resources))
                        {
                            obj = parentDic[DictionaryProperties.Resources];

                            if (obj is PdfDictionary &&
                                (obj as PdfDictionary).Items.Count > 0)
                            {
                                Dictionary[DictionaryProperties.Resources] = obj;
                                resources = new PdfResources((PdfDictionary)obj);
                            }
                            else if (obj is PdfReferenceHolder)
                            {
                                Dictionary[DictionaryProperties.Resources] = obj;
                                resources = new PdfResources((PdfDictionary)(obj as PdfReferenceHolder).Object);   
                            }
                        }
                    }
                }
            }
            else
            {
                IPdfPrimitive dicObj = Dictionary[DictionaryProperties.Resources];
                PdfDictionary dic = m_crossTable.GetObject(dicObj) as PdfDictionary;

                resources = new PdfResources(dic);

                if (dic != dicObj)
                {
                    m_crossTable.Document.PdfObjects.ReregisterReference(dic, resources);
                    if (!m_crossTable.IsMerging)
                        resources.Position = -1;
                }
                else
                {
                    Dictionary[DictionaryProperties.Resources] = resources;
                }

                SetResources(resources);
            }

            m_bCheckResources = true;
            return resources;
        }
        /// <summary>
        /// Gets or sets the terminal fields.
        /// </summary>
        internal List<PdfDictionary> TerminalAnnotation
        {
            get
            {
                return m_terminalannots;
            }
            set
            {
                m_terminalannots = value;
            }
        }

        /// <summary>
        /// Gets the origin coordinate of the loaded page
        /// </summary>
        internal override PointF Origin
        {
            get
            {
                PdfArray mBox = Dictionary.GetValue(CrossTable,
                   DictionaryProperties.MediaBox, DictionaryProperties.Parent) as PdfArray;

                float x = (mBox[0] as PdfNumber).FloatValue;
                float y = (mBox[1] as PdfNumber).FloatValue;

                PointF origin = new PointF(x, y);
                return origin;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLoadedPage"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="cTable">The cross-reference table.</param>
        /// <param name="dictionary">The page's dictionary.</param>
        internal PdfLoadedPage(PdfDocumentBase document, PdfCrossTable cTable, PdfDictionary dictionary)
            : base(dictionary)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (cTable == null)
                throw new ArgumentNullException("cTable");
            m_document = document;
            m_crossTable = cTable;
            cTable.PageCorrespondance.Add(Dictionary, null);

            if (m_document.IsPdfViewerDocumentDisable)
            {
                // Create the annotations.
                CreateAnnotations();
                Dictionary.BeginSave += new SavePdfPrimitiveEventHandler(PageBeginSave);
                Dictionary.EndSave += new SavePdfPrimitiveEventHandler(PageEndSave);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raises before the page saves.
        /// </summary>
        public event EventHandler BeginSave;
        #endregion

        #region Implementation

        /// <summary>
        /// Retrieves the terminal annotations.
        /// </summary>
        internal void CreateAnnotations()
        {
            m_annotChanged = true;
            PdfArray annots = null;

            if (Dictionary.ContainsKey(DictionaryProperties.Annots))
            {
                annots = m_crossTable.GetObject(Dictionary[DictionaryProperties.Annots]) as PdfArray;

                PdfLoadedDocument ldoc = Document as PdfLoadedDocument;

                for (int count = 0; count < annots.Count; ++count)
                {
                    PdfDictionary annotDicrionary = m_crossTable.GetObject(annots[count]) as PdfDictionary;

                    if (!ldoc.IsXFAForm)
                    {
                        if (annotDicrionary != null)
                        {
                            if (!CheckFormField(annots[count]))
                            {
                                if (annotDicrionary.ContainsKey(DictionaryProperties.FT))
                                    annotDicrionary.Remove(DictionaryProperties.FT);
                                if (annotDicrionary.ContainsKey(DictionaryProperties.V))
                                    annotDicrionary.Remove(DictionaryProperties.V);
                                m_terminalannots.Add(annotDicrionary);
                            }
                        }
                    }
                }
                m_annots = new PdfLoadedAnnotationCollection(this);
                this.Annotations = m_annots;
                Annotations = m_annots;
            }
        }

        /// <summary>
        /// Check whether annotation is form field or not.
        /// </summary>
        /// <param name="iPdfPrimitive">The annotation need to be check</param>
        /// <returns>Returns true if the annotation is used by Acroform field, otherwise false.</returns>
        private bool CheckFormField(IPdfPrimitive iPdfPrimitive)
        {
            PdfLoadedDocument ldoc = Document as PdfLoadedDocument;
            if (m_widgetReferences == null && ldoc.Form != null)
            {
                bool wasNew;
                m_widgetReferences = new List<long>();
                foreach (object loadField in ldoc.Form.Fields)
                {
                    PdfLoadedField field= loadField as PdfLoadedField;
                    if (field  != null)
                    {
                        IPdfPrimitive widget = field.GetWidgetAnnotation(field.Dictionary, field.CrossTable);
                        PdfReference widgetReference = Document.PdfObjects.GetReference(widget, out wasNew);
                        if (wasNew)
                        {
                            widgetReference = CrossTable.GetReference(widget);
                        }
                        m_widgetReferences.Add(widgetReference.ObjNum);
                    }
                }
            }
            PdfReferenceHolder holder = iPdfPrimitive as PdfReferenceHolder;

            if (ldoc.Form != null && m_widgetReferences.Count > 0 && holder.Reference != null)
            {
                if (m_widgetReferences.Contains(holder.Reference.ObjNum))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Removes field and kids annotation from dictionaries.
        /// </summary>
        /// <param name="field">The field.</param>
        internal void RemoveFromDictionaries(PdfAnnotation annot)
        {
            if (Dictionary.ContainsKey(DictionaryProperties.Annots))
            {
                PdfArray array = m_crossTable.GetObject(Dictionary[DictionaryProperties.Annots]) as PdfArray;

                PdfReferenceHolder holder = new PdfReferenceHolder(annot.Dictionary);

                array.Remove(holder);
                array.MarkChanged();

                Dictionary.SetProperty(DictionaryProperties.Annots, array);
            }

            if (annot is PdfLoadedAnnotation)
            {
                // DeleteFromPages(annot);
                //  DeleteAnnottation(annot);
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Raises <see cref="BeginSave"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnBeginSave(EventArgs e)
        {
            if (BeginSave != null)
            {
                BeginSave(this, e);
            }
        }

        /// <summary>
        /// Raises when page dictionary is going to be saved.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        private void PageBeginSave(object sender, SavePdfPrimitiveEventArgs args)
        {
            OnBeginSave(new EventArgs());
        }

        /// <summary>
        /// Raises after the page dictionary was saved.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">The <see cref="T:Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> 
        /// instance containing the event data.</param>
        private void PageEndSave(object sender, SavePdfPrimitiveEventArgs args)
        {
        }
        #endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Extract the font which are used in a given page.
        /// </summary>
        /// <returns>returns the extracted fonts as a PdfFont[].</returns>
        internal PdfFont[] ExtractFonts()
        {
            List<PdfFont> fonts = new List<PdfFont>();
            this.GetFontStream();
            if (this.m_fontReference != null)
            {
                foreach (PdfReferenceHolder fontList in this.m_fontReference)
                {
                    PdfDictionary fontDictionary = fontList.Object as PdfDictionary;
                    float height = 12.0f;

                    PdfName baseFont = CrossTable.GetObject(fontDictionary[DictionaryProperties.BaseFont]) as PdfName;

                    PdfFont font = new PdfStandardFont((PdfStandardFont)PdfDocument.DefaultFont, height);

                    string key = GetKey(baseFont);

                    height = GetContentHeight(key);

                    if (fontDictionary.ContainsKey(DictionaryProperties.Subtype))
                    {
                        // Font Subtype as Type1
                        PdfName fontSubtype = CrossTable.GetObject(fontDictionary[DictionaryProperties.Subtype]) as PdfName;
                        if (fontSubtype.Value == DictionaryProperties.Type1)
                        {
                            try
                            {
                                PdfFontFamily fontFamily = GetFontFamily(baseFont.Value);
                                PdfFontStyle fontStyle = GetFontStyle(baseFont.Value);
                                font = new PdfStandardFont(fontFamily, height, fontStyle);
                            }
                            catch (ArgumentException)
                            {
                                PdfFontMetrics fontMetrics = CreateFont(fontDictionary, height, baseFont);

                                string fontNameStr = baseFont.Value.Substring(baseFont.Value.IndexOf('+') + 1);

                                PdfFontStyle style = PdfFontStyle.Regular;
                                // Removing PSMT string if its present in the fontname
                                if (fontNameStr.Contains("PSMT"))
                                    fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("PSMT"));
                                else if (fontNameStr.Contains("Bold"))
                                    style = PdfFontStyle.Bold;
                                else if (fontNameStr.Contains("Italic"))
                                    style = PdfFontStyle.Italic;
                                if (fontNameStr.Contains("BoldItalic"))
                                    style = PdfFontStyle.Italic | PdfFontStyle.Bold;
                                if (fontNameStr.Contains("PS"))
                                    fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("PS"));
                                if (fontNameStr.Contains("-"))
                                    fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("-"));

                                font = new PdfStandardFont((PdfStandardFont)PdfDocument.DefaultFont, height, style);

                                // font = new PdfTrueTypeFont(new Font(fontNameStr, height, style), false);

                                WidthTable widthTable = font.Metrics.WidthTable;
                                font.Metrics = fontMetrics;
                                font.Metrics.Name = fontNameStr;
                                font.Metrics.WidthTable = widthTable;
                            }
                        }
                        else if (fontSubtype.Value == DictionaryProperties.TrueType)
                        {
                            PdfFontMetrics fontMetrics = CreateFont(fontDictionary, height, baseFont);

                            string fontNameStr = baseFont.Value.Substring(baseFont.Value.IndexOf('+') + 1);

                            FontStyle style = FontStyle.Regular;
                            // Removing PSMT string if its present in the fontname
                            if (fontNameStr.Contains("PSMT"))
                                fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("PSMT"));
                            else if (fontNameStr.Contains("Bold"))
                                style = FontStyle.Bold;
                            else if (fontNameStr.Contains("Italic"))
                                style = FontStyle.Italic;
                            if (fontNameStr.Contains("BoldItalic"))
                                style = FontStyle.Italic | FontStyle.Bold;
                            if (fontNameStr.Contains("PS"))
                                fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("PS"));
                            if (fontNameStr.Contains("-"))
                                fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("-"));

                            FontFamily[] families = FontFamily.Families;
                            foreach (FontFamily family in families)
                            {
                                string systemFontName = family.Name.Replace(" ", string.Empty);
                                if (fontNameStr.Contains(systemFontName))
                                {
                                    fontNameStr = family.Name;
                                    break;
                                }
                            }

                            bool unicode = false;

                            if (fontDictionary.ContainsKey(DictionaryProperties.ToUnicode))
                                unicode = true;

                            font = new PdfTrueTypeFont(new Font(fontNameStr, height, style), unicode);
                            WidthTable widthTable = font.Metrics.WidthTable;
                            font.Metrics = fontMetrics;
                            font.Metrics.Name = fontNameStr;
                            font.Metrics.WidthTable = widthTable;
                        }
                        else if (fontSubtype.Value == DictionaryProperties.Type0)
                        {
                            if (fontDictionary.ContainsKey(DictionaryProperties.ToUnicode))
                            {
                                PdfArray descendantFontsArray = fontDictionary[DictionaryProperties.DescendantFonts] as PdfArray;
                                PdfDictionary descendantFontsDic = (descendantFontsArray[0] as PdfReferenceHolder).Object as PdfDictionary;
                                PdfReferenceHolder fontDescriptor = descendantFontsDic[DictionaryProperties.FontDescriptor] as PdfReferenceHolder;
                                PdfDictionary fontDescriptorDic = fontDescriptor.Object as PdfDictionary;

                                PdfName font_name = fontDescriptorDic[DictionaryProperties.FontName] as PdfName;

                                PdfFontMetrics fontMetrics = CreateFont(descendantFontsDic, height, font_name);

                                string fontNameStr = font_name.Value.Substring(font_name.Value.IndexOf('+') + 1);

                                FontStyle style = FontStyle.Regular;
                                // Removing PSMT string if its present in the fontname
                                if (fontNameStr.Contains("PSMT"))
                                    fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("PSMT"));
                                else if (fontNameStr.Contains("Bold"))
                                    style = FontStyle.Bold;
                                else if (fontNameStr.Contains("Italic"))
                                    style = FontStyle.Italic;
                                if (fontNameStr.Contains("BoldItalic"))
                                    style = FontStyle.Italic | FontStyle.Bold;
                                if (fontNameStr.Contains("PS"))
                                    fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("PS"));
                                if (fontNameStr.Contains("-"))
                                    fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("-"));

                                FontFamily[] families = FontFamily.Families;
                                foreach (FontFamily family in families)
                                {
                                    string systemFontName = family.Name.Replace(" ", string.Empty);
                                    if (fontNameStr.Contains(systemFontName))
                                    {
                                        fontNameStr = family.Name;
                                        break;
                                    }
                                }

                                font = new PdfTrueTypeFont(new Font(fontNameStr, height, style), true);
                                WidthTable widthTable = font.Metrics.WidthTable;
                                font.Metrics = fontMetrics;
                                font.Metrics.Name = fontNameStr;
                                font.Metrics.WidthTable = widthTable;
                            }
                        }

                    }
                    if (font != null)
                    {
                        font.InternalFontName = baseFont.Value;
                        fonts.Add(font);
                    }
                }
                return fonts.ToArray();
            }
            return fonts.ToArray();
        }
#endif

        /// <summary>
        /// Clears PdfLoadedPage.
        /// </summary>
        internal override void Clear()
        {
            if (m_annots != null)
                m_annots.Clear();

            base.Clear();

            if (m_terminalannots != null)
                m_terminalannots.Clear();

            if (m_widgetReferences != null)
                m_widgetReferences.Clear();

            if (m_fontReference != null)
                m_fontReference.Clear();
        }

        /// <summary>
        /// Gets the text size of a specified font.
        /// </summary>
        /// <param name="key">Font key</param>
        /// <returns>Returns the text size of the specified font</returns>
        private float GetContentHeight(string key)
        {
            MemoryStream ms = new MemoryStream();
            Layers.CombineContent(ms);

            float height = 0.0f;

            // Content Array
            string contentValue = PdfString.ByteToString(ms.ToArray());

            StringTokenizer token = new StringTokenizer(contentValue);

            if (contentValue.Contains(key))
            {
                int indexOf = contentValue.IndexOf(key);
                token.Position = indexOf;
                string value = token.ReadLine();

                string[] keyValues = value.Split(' ');
                if (keyValues.Length == 3)
                    height = float.Parse(keyValues[1]);
                else
                    height = 12.00f;
                //int indexofTF = contentValue.IndexOf("Tf");
                //string[] values = contentValue.Split(' ');

            }
            return height;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Reading Font Name from Dictionary.
        /// </summary>
        /// <param name="fontString"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        internal string FontName(string fontString, out float height)
        {
            byte[] buf = ASCIIEncoding.ASCII.GetBytes(fontString);
            MemoryStream stream = new MemoryStream(buf);
            PdfReader reader = new PdfReader(stream);

            reader.Position = 0;

            string prevToken = reader.GetNextToken();
            string token = reader.GetNextToken();
            string name = null;
            height = 0.0f;
            while (token != null && token != string.Empty)
            {
                name = prevToken;
                prevToken = token;
                token = reader.GetNextToken();

                if (token == Operators.SetFont)
                {
                    height = (float)double.Parse(prevToken, NumberStyles.Float,
                        CultureInfo.InvariantCulture);

                    break;
                }
            }
            return name;
        }
#endif
        /// <summary>
        /// Create metrics for embed font
        /// </summary>
        /// <param name="fontDictionary"></param>
        /// <returns></returns>
        private PdfFontMetrics CreateFont(PdfDictionary fontDictionary, float height, PdfName baseFont)
        {
            PdfFontMetrics fontMetrics = new PdfFontMetrics();

            if (fontDictionary.ContainsKey(DictionaryProperties.FontDescriptor))
            {
                PdfDictionary dic = ((fontDictionary[DictionaryProperties.FontDescriptor] as PdfReferenceHolder).Object) as PdfDictionary;
                fontMetrics.Ascent = (dic[DictionaryProperties.Ascent] as PdfNumber).IntValue;
                fontMetrics.Descent = (dic[DictionaryProperties.Descent] as PdfNumber).IntValue;
                fontMetrics.Size = height;
                fontMetrics.Height = fontMetrics.Ascent - fontMetrics.Descent;
                fontMetrics.PostScriptName = baseFont.Value;
                PdfArray array = null;

                if (fontDictionary.ContainsKey(DictionaryProperties.Widths))
                {
                    array = (fontDictionary[DictionaryProperties.Widths] as PdfArray);
                    if(array == null)
                    {
                        PdfReferenceHolder widthReferceHolder = (fontDictionary[DictionaryProperties.Widths] as PdfReferenceHolder);
                        array = widthReferceHolder.Object as PdfArray;
                    }
                    int[] widthTable = new int[array.Count];

                    for (int i = 0; i < array.Count; i++)
                    {
                        widthTable[i] = (array[i] as PdfNumber).IntValue;
                    }

                    fontMetrics.WidthTable = new StandardWidthTable(widthTable);
                }


                fontMetrics.Name = baseFont.Value;
            }
            return fontMetrics;
        }

        /// <summary>
        /// Gets the font style.
        /// </summary>
        /// <param name="fontFamilyString">The font family string.</param>
        /// <returns>The style of pdf font.</returns>
        private PdfFontStyle GetFontStyle(string fontFamilyString)
        {
            int position = fontFamilyString.IndexOf("-");

            PdfFontStyle style = PdfFontStyle.Regular;

            if (position >= 0)
            {
                string standardName = fontFamilyString.Substring(position + 1, fontFamilyString.Length - position - 1);

                switch (standardName)
                {
                    case "Italic":
                    case "Oblique":
                        style = PdfFontStyle.Italic;
                        break;

                    case "Bold":
                        style = PdfFontStyle.Bold;
                        break;

                    case "BoldItalic":
                    case "BoldOblique":
                        style = PdfFontStyle.Italic | PdfFontStyle.Bold;
                        break;
                }
            }
            return style;
        }

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <param name="fontFamilyString">The font family string.</param>
        /// <returns>The font family.</returns>
        private PdfFontFamily GetFontFamily(string fontFamilyString)
        {
            int position = fontFamilyString.IndexOf("-");

            PdfFontFamily fontFamily = PdfFontFamily.Helvetica;

            string standardName = fontFamilyString;

            if (position >= 0)
            {
                standardName = fontFamilyString.Substring(0, position);
            }

            if (standardName == "Times")
            {
                fontFamily = PdfFontFamily.TimesRoman;
            }
            else
            {
                fontFamily = (PdfFontFamily)Enum.Parse(typeof(PdfFontFamily), standardName, true);
            }
            return fontFamily;
        }

        /// <summary>
        /// Gets the key value for a given font
        /// </summary>
        /// <param name="fontName">PdfFont value.</param>
        /// <returns>Returns the key value.</returns>
        private string GetKey(PdfName fontName)
        {
            PdfResources resource = this.GetResources();

            if (resource.ContainsKey(DictionaryProperties.Font))
            {
                if (resource[DictionaryProperties.Font] is PdfDictionary)
                {
                    PdfDictionary resoureDictionary = resource[DictionaryProperties.Font] as PdfDictionary;

                    Dictionary<PdfName, IPdfPrimitive> items = resoureDictionary.Items;

                    string key = string.Empty;

                    foreach (KeyValuePair<PdfName, IPdfPrimitive> item in items)
                    {
                        PdfReferenceHolder holder = item.Value as PdfReferenceHolder;

                        PdfDictionary temp = holder.Object as PdfDictionary;
                        PdfName baseFont = CrossTable.GetObject(temp[DictionaryProperties.BaseFont]) as PdfName;
                        if (baseFont.Value == fontName.Value)
                        {
                            key = item.Key.Value;
                            return key;
                        }
                    }
                }
            }
            return null;
        }
    }
}
