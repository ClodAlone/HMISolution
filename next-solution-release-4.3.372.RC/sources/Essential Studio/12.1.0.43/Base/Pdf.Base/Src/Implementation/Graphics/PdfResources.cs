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

using Syncfusion.Pdf.ColorSpace;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf
{
#if NETFX_CORE || WP
    public class PdfResources : PdfDictionary
#else
    internal class PdfResources : PdfDictionary
#endif
    {
        #region Fields
        /// <summary>
        /// Dictionary of the objects names.
        /// </summary>
        private Dictionary<IPdfPrimitive, PdfName> m_names;
        /// <summary>
        /// Dictionary of the properties names.
        /// </summary>
        private PdfDictionary m_properties = new PdfDictionary();
        #endregion

        #region Properties
        // ExtGState
        // ColorSpace
        // Pattern
        // Shading
        // XObject
        // Font
        // ProcSet
        // Properties

        /// <summary>
        /// Gets the font names.
        /// </summary>
        private Dictionary<IPdfPrimitive, PdfName> Names
        {
            get
            {
                return GetNames();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfResources"/> class.
        /// </summary>
        internal PdfResources()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfResources"/> class.
        /// </summary>
        /// <param name="baseDictionary">The base dictionary.</param>
#if NETFX_CORE || WP
        public PdfResources(PdfDictionary baseDictionary)
#else
        internal PdfResources(PdfDictionary baseDictionary)
#endif
            : base(baseDictionary)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Generates name for the object and adds to the resource if the object is new. Otherwise
        /// returns object's name in the context of the resources.
        /// </summary>
        /// <param name="obj">Object contained by a resource.</param>
        /// <returns>Returns object's name in the context of the resources.</returns>
        internal PdfName GetName(IPdfWrapper obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            IPdfPrimitive primitive = obj.Element;
            PdfName name = null;

            if(Names.ContainsKey(primitive))
                name = Names[primitive];

            // Object is new.
            if (name == null)
            {
                string sName = GenerateName();
                name = new PdfName(sName);
                Names[primitive] = name;

                Add(obj, name);
            }

            return name;
        }

        /// <summary>
        /// Gets resource names to font dictionaries.
        /// </summary>
        /// <returns>The names to font dictionaries.</returns>
        internal Dictionary<IPdfPrimitive, PdfName> GetNames()
        {
            if (m_names == null)
            {
                m_names = new Dictionary<IPdfPrimitive,PdfName>();

                IPdfPrimitive fonts = this[DictionaryProperties.Font];

                if (fonts != null)
                {
                    PdfReferenceHolder reference = fonts as PdfReferenceHolder;
                    PdfDictionary dictionary = fonts as PdfDictionary;

                    if (reference != null)
                    {
                        dictionary = PdfCrossTable.Dereference(fonts) as PdfDictionary;
                    }

                    if (dictionary != null)
                    {
                        foreach(KeyValuePair<PdfName, IPdfPrimitive> item in dictionary.Items)
                        {
                            IPdfPrimitive primitive = PdfCrossTable.Dereference(item.Value);
                            PdfName name = item.Key;

                            m_names[primitive] = name;
                        }
                    }
                }
            }

            return m_names;
        }

        /// <summary>
        /// Requires the proc set.
        /// </summary>
        /// <param name="procSetName">Name of the proc set.</param>
        internal void RequireProcSet(string procSetName)
        {
            if (procSetName == null)
                throw new ArgumentNullException("procSetName");

            PdfArray procSets = this[DictionaryProperties.ProcSet] as PdfArray;

            if (procSets == null)
            {
                procSets = new PdfArray();
                this[DictionaryProperties.ProcSet] = procSets;
            }

            PdfName name = new PdfName(procSetName);

            if (!procSets.Contains(name))
            {
                procSets.Add(name);
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Generates Unique string name.
        /// </summary>
        /// <returns></returns>
        private string GenerateName()
        {
            string name = Guid.NewGuid().ToString();

            return name;
        }

        /// <summary>
        /// Adds object to the resources.
        /// </summary>
        /// <param name="obj">Object to be added to resources.</param>
        /// <param name="name">Name of the object.</param>
        private void Add(IPdfWrapper obj, PdfName name)
        {
            PdfFont font = obj as PdfFont;
            if (font != null)
            {
                Add(font, name);
                return;
            }

            PdfTemplate template = obj as PdfTemplate;
            if (template != null)
            {
                Add(template, name);
                return;
            }

            PdfImage image = obj as PdfImage;
            if (image != null)
            {
                Add(image, name);
                return;
            }

            PdfBrush brush = obj as PdfBrush;
            if (brush != null)
            {
                Add(brush, name);
                return;
            }

            PdfTransparency transparancy = obj as PdfTransparency;

            if (transparancy != null)
            {
                Add(transparancy, name);
                return;
            }


            PdfColorSpaces colorspace = obj as PdfColorSpaces;
            if (colorspace != null)
            {
                Add(colorspace, name);
                return;
            }

            PdfDictionary d_colorspace = obj as PdfDictionary;
            if (d_colorspace != null)
            {
                Add(colorspace, name);
                return;
            }
        }

        /// <summary>
        /// Adds the font to the "Fonts" sub dictionary.
        /// </summary>
        /// <param name="font">The font to add.</param>
        /// <param name="name">The name.</param>
        internal void Add(PdfFont font, PdfName name)
        {
            PdfDictionary dictionary = null;

            IPdfPrimitive fonts = this[DictionaryProperties.Font];

            if (fonts != null)
            {
                PdfReferenceHolder reference = fonts as PdfReferenceHolder;
                dictionary = fonts as PdfDictionary;

                if (reference != null)
                {
                    dictionary = PdfCrossTable.Dereference(fonts) as PdfDictionary;
                }
            }

            // Create fonts dictionary.
            else
            {
                dictionary = new PdfDictionary();
                this[DictionaryProperties.Font] = dictionary;
            }

            dictionary[name] = new PdfReferenceHolder(((IPdfWrapper)font).Element);
        }
        /// <summary>
        /// Adds the Layer Properties to the "Properties"  dictionary.
        /// </summary>
        /// <param name="layerid">The layer properties to add.</param>
        /// <param name="reff">The reference.</param>
        internal void AddProperties( String  layerid ,PdfReferenceHolder reff)
        {
            m_properties[layerid] = reff;
         
            this[DictionaryProperties.Properties] = m_properties;
        }

        /// <summary>
        /// Adds the template to the "XObject" sub dictionary.
        /// </summary>
        /// <param name="template">The template to add.</param>
        /// <param name="name">The name.</param>
        private void Add(PdfTemplate template, PdfName name)
        {

            PdfDictionary xobjects;
            if (this[DictionaryProperties.XObject] is PdfReferenceHolder)
                xobjects = (this[DictionaryProperties.XObject] as PdfReferenceHolder).Object as PdfDictionary;
            else
                xobjects = this[DictionaryProperties.XObject] as PdfDictionary; 

            // Create fonts dictionary.
            if (xobjects == null)
            {
                xobjects = new PdfDictionary();
                this[DictionaryProperties.XObject] = xobjects;
            }

            xobjects[name] = new PdfReferenceHolder(((IPdfWrapper)template).Element);
        }

        /// <summary>
        /// Adds the image to the "XObject" sub dictionary.
        /// </summary>
        /// <param name="image">The image to add.</param>
        /// <param name="name">The name.</param>
        private void Add(PdfImage image, PdfName name)
        {

            PdfDictionary xobjects = this[DictionaryProperties.XObject] as PdfDictionary;

            PdfReferenceHolder xobjectsRefHold = this[DictionaryProperties.XObject] as PdfReferenceHolder;

            PdfDictionary dic = new PdfDictionary();
            if (xobjects == null)
            {

                if (xobjectsRefHold != null)
                {
                    dic = xobjectsRefHold.Object as PdfDictionary;
                }

            }

            // Create fonts dictionary.
            if (xobjects == null)
            {
                xobjects = new PdfDictionary();
                this[DictionaryProperties.XObject] = xobjects;
                foreach (KeyValuePair<PdfName, IPdfPrimitive> item in dic.Items)
                {
                    xobjects[item.Key] = item.Value as PdfReferenceHolder;
                }
            }

            xobjects[name] = new PdfReferenceHolder(((IPdfWrapper)image).Element);
        }

        /// <summary>
        /// Adds the specified brush to the resources.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="name">The name of the brush.</param>
        private void Add(PdfBrush brush, PdfName name)
        {
            IPdfPrimitive savable = (brush as IPdfWrapper).Element;

            if (savable != null)
            {
                PdfDictionary pattern = this[DictionaryProperties.Pattern] as PdfDictionary;

                // Create a new pattern dictionary.
                if (pattern == null)
                {
                    pattern = new PdfDictionary();
                    this[DictionaryProperties.Pattern] = pattern;
                }

                pattern[name] = new PdfReferenceHolder(savable);
            }
        }

        /// <summary>
        /// Adds the specified transparancy to the resources.
        /// </summary>
        /// <param name="transparancy">The transparancy.</param>
        /// <param name="name">The name of the brush.</param>
        private void Add(PdfTransparency transparancy, PdfName name)
        {
            IPdfPrimitive savable = (transparancy as IPdfWrapper).Element;

            if (savable != null)
            {
                PdfDictionary transDic=null;
                if (this[DictionaryProperties.ExtGState] is PdfDictionary)
                {
                   transDic= this[DictionaryProperties.ExtGState] as PdfDictionary;
                }
                else if (this[DictionaryProperties.ExtGState] is PdfReferenceHolder)
                {
                    PdfReferenceHolder holder = this[DictionaryProperties.ExtGState] as PdfReferenceHolder;
                    transDic = holder.Object as PdfDictionary;
                }

                // Create a new pattern dictionary.
                if (transDic == null)
                {
                    transDic = new PdfDictionary();
                    this[DictionaryProperties.ExtGState] = transDic;
                }

                transDic[name] = new PdfReferenceHolder(savable);
            }
        }

        /// <summary>
        /// Adds the specified ColorSpaces to the resources.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="name"></param>
        internal void Add(PdfColorSpaces color, PdfName name)
        {
            PdfDictionary dictionary = null;

            IPdfPrimitive colorspaces = this[DictionaryProperties.ColorSpace];

            if (colorspaces != null)
            {
                PdfReferenceHolder reference = colorspaces as PdfReferenceHolder;
                dictionary = colorspaces as PdfDictionary;

                if (reference != null)
                {
                    dictionary = PdfCrossTable.Dereference(colorspaces) as PdfDictionary;
                }
            }

            // Create fonts dictionary.
            else
            {
                dictionary = new PdfDictionary();
                this[DictionaryProperties.ColorSpace] = dictionary;
            }

            dictionary[name] = new PdfReferenceHolder(((IPdfWrapper)color).Element);
        }

        /// <summary>
        /// Adds the specified Dictionary to the resources.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="name"></param>
        internal void Add(PdfDictionary color, PdfName name)
        {
            PdfDictionary dictionary = null;

            IPdfPrimitive colorspaces = this[DictionaryProperties.ColorSpace];

            if (colorspaces != null)
            {
                PdfReferenceHolder reference = colorspaces as PdfReferenceHolder;
                dictionary = colorspaces as PdfDictionary;

                if (reference != null)
                {
                    dictionary = PdfCrossTable.Dereference(colorspaces) as PdfDictionary;
                }
            }

            // Create fonts dictionary.
            else
            {
                dictionary = new PdfDictionary();
                this[DictionaryProperties.ColorSpace] = dictionary;
            }

            dictionary[name] = new PdfReferenceHolder(((IPdfWrapper)color).Element);
        }

        internal void RemoveFont(string name)
        {
            IPdfPrimitive key = null;
            foreach (KeyValuePair<IPdfPrimitive, PdfName> kvp in m_names)
            {
                if (kvp.Value == new PdfName(name))
                {
                    key = kvp.Key;
                    break;
                }
            }

            if (key != null)
                m_names.Remove(key);
        }
        #endregion
    }
}
