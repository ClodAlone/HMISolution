#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.IO;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Text;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Collection of the pages layers.
    /// </summary>
    public class PdfPageLayerCollection : PdfCollection
    {
        #region Fields
        /// <summary>
        /// Parent page.
        /// </summary>
        private PdfPageBase m_page;

        /// <summary>
        /// Indicates if Sublayer is present.
        /// </summary>
        internal bool m_sublayer = false;
        /// <summary>
        /// Stores the number of first level layers in the document.
        /// </summary>
        private int parentLayerCount = 0;
        #endregion

        public PdfPageLayerCollection()
            : base()
        {
        }
        #region Properties
        /// <summary>
        /// Gets or sets element by its index.
        /// </summary>
        /// <remarks>The layers belonging to the same page can be added to the collection only.</remarks>
        public PdfPageLayer this[int index]
        {
            get
            {
                object obj = List[index];

                return (obj as PdfPageLayer);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("layer");
                if (value.Page != m_page)
                    throw new ArgumentException("The layer belongs to another page");

                // Add/remove the layer.
                PdfPageLayer layer = this[index];
                if (layer != null)
                {
                    RemoveLayer(layer);
                }

                List[index] = value;
                InsertLayer(index, value);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new collection.
        /// </summary>
        /// <param name="page">Parent page for the layers in the collection.</param>
        public PdfPageLayerCollection(PdfPageBase page)
            : base()
        {
            if (page == null)
                throw new ArgumentNullException("page");

            m_page = page;

            PdfPageBase lPage = page;

            if (lPage != null)
            {
                ParseLayers(lPage);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates a new layer and adds it to the end of the collection.
        /// </summary>
        /// <returns>Created layer.</returns>
        public PdfPageLayer Add()
        {
            PdfPageLayer layer = new PdfPageLayer(m_page);

            layer.Name = string.Empty;

            Add(layer);

            return layer;
        }

        /// <summary>
        /// Creates a new layer and adds it to the end of the collection.
        /// </summary>
        /// <param name="LayerName">Layer Name.</param>
        /// <param name="Visible">Layer Visibility.</param>
        /// <returns>Created layer.</returns>
        public PdfPageLayer Add(string LayerName, bool Visible)
        {
            PdfPageLayer layer = new PdfPageLayer(m_page);
            layer.Name = LayerName;
            layer.Visible = Visible;
            layer.LayerId = "OCG_" + Guid.NewGuid().ToString();
            Add(layer);
            CreateLayer(layer);
            return layer;
        }

        /// <summary>
        /// Creates a new layer and adds it to the end of the collection.
        /// </summary>
        /// <param name="LayerName">Layer Name.</param>
        /// <returns>Created layer.</returns>
        public PdfPageLayer Add(string LayerName)
        {
            PdfPageLayer layer = new PdfPageLayer(m_page);
            layer.Name = LayerName;
            layer.LayerId = "OCG_" + Guid.NewGuid().ToString();
            Add(layer);
            CreateLayer(layer);

            return layer;
        }

        /// <summary>
        /// Adds layer to the collection.
        /// </summary>
        /// <param name="layer">Layer object.</param>
        /// <remarks>The layers belonging to the same page can be added to the collection only.</remarks>
        public int Add(PdfPageLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            if (layer.Page != m_page)
                throw new ArgumentException("The layer belongs to another page");

            int index = List.Add(layer);

            // Register layer.
            AddLayer(index, layer);

            return index;
        }

        /// <summary>
        /// Creates a  Optional Content Properties and adds it to Catalog.
        /// </summary>
        /// <param name="layer">Layer.</param>
        private void CreateLayer(PdfPageLayer layer)
        {
            PdfPage m_catalog = this.m_page as PdfPage;

            PdfDictionary OCProperties = new PdfDictionary();
            IPdfPrimitive ocgroups = CreateOptionalContentDictionary(layer);
            OCProperties[DictionaryProperties.Ocg] = ocgroups;
            OCProperties[DictionaryProperties.Defaultview] = CreateOptionalContentViews(layer);
            PdfDictionary dic = OCProperties;

            m_catalog.Document.Catalog.SetProperty(DictionaryProperties.OCProperties, OCProperties);
        }
        /// <summary>
        /// Inserts layer into collection.
        /// </summary>
        /// <param name="index">Index of the layer.</param>
        /// <param name="layer">Layer object.</param>
        /// <remarks>The layers belonging to the same page can be added to the collection only.</remarks>
        public void Insert(int index, PdfPageLayer layer)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Value can not be less 0");

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (layer.Page != m_page)
                throw new ArgumentException("The layer belongs to another page");

            List.Insert(index, layer);

            // Register layer.
            InsertLayer(index, layer);
        }

        /// <summary>
        /// Creates a  Optional Content Groups and adds it to OC Properties.
        /// </summary>
        /// <param name="layer">Layer.</param>
        /// <returns>primitive as pdfarray.</returns>
        private IPdfPrimitive CreateOptionalContentDictionary(PdfPageLayer layer)
        {
            PdfPage m_layer = this.m_page as PdfPage;

            PdfDictionary m_dictionary = new PdfDictionary();

            m_dictionary[DictionaryProperties.OCGName] = new PdfString(layer.Name );
            m_dictionary[DictionaryProperties.Type] = new PdfName("OCG");
            m_dictionary[DictionaryProperties.OCGLayerID] = new PdfName(layer.LayerId);
            m_dictionary[DictionaryProperties.OCGVisible] = new PdfBoolean(layer.Visible);

            PdfReferenceHolder reference = new PdfReferenceHolder(m_dictionary);
            m_layer.Document.primitive.Insert(m_layer.Document.m_positon, reference);

            // Order of the layers     
            if (m_sublayer == false)
            {
                layer.m_sublayer = false;
                if (m_layer.Document.m_sublayerposition > 0)
                {
                    int index = m_page.Contents.Count;
                    m_page.Contents.RemoveAt(parentLayerCount - 1);

                    PdfStream stream = new PdfStream();
#if SILVERLIGHT || NETFX_CORE || WP
                    byte[] endmark = Encoding.UTF8.GetBytes("EMC\n");
#else
                    byte[] endmark = Encoding.ASCII.GetBytes("EMC\n");
#endif
                    stream.Write(endmark);

                    m_page.Contents.Insert(index - 2, new PdfReferenceHolder(stream));
                }
                m_layer.Document.m_sublayerposition = 0;
                m_layer.Document.m_sublayer = new PdfArray();
                m_layer.Document.m_order.Insert(m_layer.Document.m_orderposition, reference);
                m_layer.Document.m_orderposition++;

                WriteEndMark();
                parentLayerCount = m_page.Contents.Count;
            }
            else
            {
                layer.m_sublayer = true;
                m_layer.Document.m_sublayer.Insert(m_layer.Document.m_sublayerposition, reference);
                if (m_layer.Document.m_sublayerposition != 0)
                {
                    m_layer.Document.m_order.RemoveAt(m_layer.Document.m_orderposition - 1);
                    m_layer.Document.m_orderposition--;
                }
                m_layer.Document.m_order.Insert(m_layer.Document.m_orderposition, m_layer.Document.m_sublayer);
                m_layer.Document.m_sublayerposition++;
                m_layer.Document.m_orderposition++;
                WriteEndMark();
            }
            // Check if layer is visible
            if (layer.Visible)
            {
                m_layer.Document.m_on.Insert(m_layer.Document.m_onpositon, reference);
                m_layer.Document.m_onpositon++;
            }
            else
            {
                m_layer.Document.m_off.Insert(m_layer.Document.m_offpositon, reference);
                m_layer.Document.m_offpositon++;
            }

            m_layer.Document.m_positon++;

            // Adds Properties in Pdf Resources
            PdfResources resoure = m_layer.GetResources();
            resoure.AddProperties(layer.LayerId, reference);

            return m_layer.Document.primitive;
        }
        /// <summary>
        /// Writes End of Marked Content in Content Stream
        /// </summary>
        private void WriteEndMark()
        {

            PdfStream stream = new PdfStream();
#if SILVERLIGHT || NETFX_CORE || WP
            byte[] endmark = Encoding.UTF8.GetBytes("EMC\n");
#else
            byte[] endmark = Encoding.ASCII.GetBytes("EMC\n");
#endif

            stream.Write(endmark);

            m_page.Contents.Add(new PdfReferenceHolder(stream));
        }
        /// <summary>
        /// Creates  Optional Content Views and adds it to OC Properties.
        /// </summary>
        /// <param name="layer">Layer.</param>
        /// <returns>m_dictionary.</returns>
        private IPdfPrimitive CreateOptionalContentViews(PdfPageLayer layer)
        {
            PdfPage m_layer = this.m_page as PdfPage;

            PdfDictionary m_dictionary = new PdfDictionary();
            m_dictionary[DictionaryProperties.OCGName] = new PdfString("Layers");
            m_dictionary[DictionaryProperties.OCGOrder] = m_layer.Document.m_order;
            m_dictionary[DictionaryProperties.OCGON] = m_layer.Document.m_on;
            m_dictionary[DictionaryProperties.OCGOFF] = m_layer.Document.m_off;

            return m_dictionary;
        }
        /// <summary>
        /// Removes layer from the collection.
        /// </summary>
        /// <param name="layer">Layer object.</param>
        public void Remove(PdfPageLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            List.Remove(layer);

            // Remove layer.
            RemoveLayer(layer);
        }

        /// <summary>
        /// Removes layer by its index.
        /// </summary>
        /// <param name="index">Index of the layer.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index > List.Count - 1)
                throw new ArgumentOutOfRangeException("index", "Value can not be less 0 and greater List.Count - 1");

            PdfPageLayer layer = this[index];
            List.RemoveAt(index);

            if (layer != null)
            {
                // Remove layer.
                RemoveLayer(layer);
            }
        }

        /// <summary>
        /// Checks whether collection contains layer.
        /// </summary>
        /// <param name="layer">Layer object.</param>
        /// <returns>True - if collection contains layer, False otherwise.</returns>
        public bool Contains(PdfPageLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            bool result = List.Contains(layer);

            return result;
        }

        /// <summary>
        /// Returns index of the layer in the collection if exists, -1 otherwise.
        /// </summary>
        /// <param name="layer">Layer object.</param>
        /// <returns>Returns index of the layer in the collection if exists, -1 otherwise.</returns>
        public int IndexOf(PdfPageLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            int index = List.IndexOf(layer);

            return index;
        }

        /// <summary>
        /// Cleares the collection.
        /// </summary>
        public void Clear()
        {
            // Remove the layers.
            for (int i = 0, len = List.Count; i < len; i++)
            {
                PdfPageLayer layer = this[i];
                RemoveLayer(layer);
                m_page = null;
            }

            List.Clear();
        }

        /// <summary>
        /// Combines the content into the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
#if NETFX_CORE || WP
        public void CombineContent(Stream stream)
#else
        internal void CombineContent(Stream stream)
#endif
        {
            bool decompress = (m_page is PdfLoadedPage);
            PdfLoadedPage lPage = m_page as PdfLoadedPage;
            byte[] endl = PdfString.StringToByte(Operators.NewLine);

            for (int i = 0, count = Count; i < count; ++i)
            {
                PdfPageLayer layer = this[i];
                PdfStream layerStream = (layer as IPdfWrapper).Element as PdfStream;

                if (decompress)
                {
                    layerStream.Decompress();
                }

                layerStream.InternalStream.WriteTo(stream);

                stream.Write(endl, 0, endl.Length);
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Registers layer at the page.
        /// </summary>
        /// <param name="index">Index of the layer in the collection.</param>
        /// <param name="layer">The new layer.</param>
        private void AddLayer(int index, PdfPageLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            PdfReferenceHolder reference = new PdfReferenceHolder(layer);

            m_page.Contents.Add(reference);
        }

        /// <summary>
        /// Removes layer from the page.
        /// </summary>
        /// <param name="layer">The layer.</param>
        private void RemoveLayer(PdfPageLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            PdfReferenceHolder reference = new PdfReferenceHolder(layer);
            if (m_page != null)
                m_page.Contents.Remove(reference);
        }

        /// <summary>
        /// Registers layer at the page.
        /// </summary>
        /// <param name="index">Index of the layer in the collection.</param>
        /// <param name="layer">The new layer.</param>
        private void InsertLayer(int index, PdfPageLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            PdfReferenceHolder reference = new PdfReferenceHolder(layer);

            m_page.Contents.Insert(index, reference);
        }

        /// <summary>
        /// Parses the layers.
        /// </summary>
        /// <param name="loadedPage">The loaded page.</param>
        private void ParseLayers(PdfPageBase loadedPage)
        {
            if (loadedPage == null)
                throw new ArgumentNullException("loadedPage");

            PdfArray contents = m_page.Contents;
            PdfCrossTable crossTable = null;
            if (loadedPage is PdfPage)
                crossTable = (loadedPage as PdfPage).CrossTable;
            else
                crossTable = (loadedPage as PdfLoadedPage).CrossTable;
            PdfStream saveStream = new PdfStream();
            PdfStream restoreStream = new PdfStream();
            byte SaveState = (byte)'q';

            byte NewLine = (byte)'\n';

            byte RestoreState = (byte)'Q';
            byte[] saveData = new byte[1];
            saveData[0] = SaveState;
            saveStream.Data = saveData;
            contents.Insert(0, new PdfReferenceHolder(saveStream));
            saveData[0] = RestoreState;
            restoreStream.Data = saveData;
            contents.Insert(contents.Count, new PdfReferenceHolder(restoreStream));
            foreach (IPdfPrimitive obj in contents)
            {
                try
                {
                    PdfStream stream = crossTable.GetObject(obj) as PdfStream;

                    if (stream == null)
                        throw new PdfDocumentException("Invalid contents array.");

                    // if (stream.Compress)
                    {
                        if (!loadedPage.Imported)
                            stream.Decompress();
                    }


                    if (!loadedPage.Imported && (contents.Count == 1) && ((stream.Data[stream.Data.Length - 2] == RestoreState) || (stream.Data[stream.Data.Length - 1] == RestoreState)))
                    {
                        byte[] content = stream.Data;
                        byte[] data = new byte[content.Length + 4];
                        data[0] = SaveState;
                        data[1] = NewLine;
                        content.CopyTo(data, 2);
                        data[data.Length - 2] = NewLine;
                        data[data.Length - 1] = RestoreState;
                        stream.Data = data;
                    }

                    PdfPageLayer layer = new PdfPageLayer(loadedPage, stream);
                    List.Add(layer);
                }

                catch (Exception exception)
                {
                    if (exception is PdfDocumentException)
                    {
                        throw new PdfDocumentException("Invalid contents array.");
                    }
                }
            }
        }
        #endregion
    }
}
