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


#region File using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Collections.Generic;
using Syncfusion.Layouting;
using System.IO;
using Syncfusion.DocIO.DLS.Convertors;
using System.Text;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for XmlParagraphItem.
    /// </summary>
    internal class XmlParagraphItem : ParagraphItem
    {
        #region Fields
        private Stream m_xmlNode = null;
        private WCharacterFormat m_chFormat;
        private Dictionary<string, DictionaryEntry> m_relations;
        internal string m_shapeHyperlink;
        private Dictionary<string, ImageRecord> m_imageRelations;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the image relations.
        /// </summary>
        /// <value>The image relations.</value>
        internal Dictionary<string, ImageRecord> ImageRelations
        {
            get
            {
                if (m_imageRelations == null)
                {
                    m_imageRelations = new Dictionary<string, ImageRecord>();
                }
                return m_imageRelations;
            }
        }
        /// <summary>
        /// Gets the relations.
        /// </summary>
        /// <value>The relations.</value>
        internal Dictionary<string, DictionaryEntry> Relations
        {
            get
            {
                if (m_relations == null)
                {
                    m_relations = new Dictionary<string, DictionaryEntry>();
                }
                return m_relations;
            }
        }
        /// <summary>
        /// Gets the data node.
        /// </summary>
        /// <value>The data node.</value>
        internal Stream DataNode
        {
            get
            {
                return m_xmlNode;
            }
            set
            {
                m_xmlNode = value;
            }
        }
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <value>The character format.</value>
        internal WCharacterFormat CharacterFormat
        {
            get
            {
                return m_chFormat;
            }
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.XmlParaItem;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParagraphItem"/> class.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <param name="wordDocument">The word document.</param>
        public XmlParagraphItem(Stream xmlNode, IWordDocument wordDocument)
            : base(wordDocument as WordDocument)
        {
            m_xmlNode = xmlNode;
            m_chFormat = new WCharacterFormat(wordDocument);
        }
        #endregion;

        #region Implementation
        /// <summary>
        /// Sets the character format.
        /// </summary>
        /// <param name="charFormat">The character format.</param>
        internal void ApplyCharacterFormat(WCharacterFormat charFormat)
        {
            if (charFormat != null)
                m_chFormat = charFormat.CloneInt() as WCharacterFormat;
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            XmlParagraphItem item = base.CloneImpl() as XmlParagraphItem;

            if (m_charFormat != null)
                item.CharacterFormat.ImportContainer(m_charFormat);
            if (item.m_xmlNode != null)
                item.m_xmlNode = UtilityMethods.CloneStream(m_xmlNode as MemoryStream);
            item.m_relations = new Dictionary<string, DictionaryEntry>();
            if (m_relations != null)
            {
                foreach (string key in m_relations.Keys)
                {
                    DictionaryEntry srcRel = m_relations[key];
                    DictionaryEntry relEntry = new DictionaryEntry((string)srcRel.Key, (string)srcRel.Value);
                    item.Relations.Add(key, relEntry);
                }
            }
            item.m_imageRelations = new Dictionary<string, ImageRecord>();
            if (m_imageRelations != null)
            {
                foreach (string key in m_imageRelations.Keys)
                {
                    item.ImageRelations.Add(key, m_imageRelations[key]);
                }
            }

            return item;
        }

        #endregion

        #region Implementation / overrides
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo();
        }
#endif
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            base.CloneRelationsTo(doc, nextOwner);
            //Updates image from source to destination document.
            if (ImageRelations.Count > 0)
            {
                string[] keys = new string[ImageRelations.Count];
                ImageRelations.Keys.CopyTo(keys, 0);
                for (int i = 0; i < keys.Length; i++)
                {
                    ImageRecord imageRecord = ImageRelations[keys[i]];
                    if (imageRecord.IsMetafile)
                        imageRecord = doc.Images.LoadMetaFileImage(imageRecord.m_imageBytes, true);
                    else
                        imageRecord = doc.Images.LoadImage(imageRecord.ImageBytes);
                    ImageRelations[keys[i]] = imageRecord;
                }
            }
            if (doc.DocxPackage == null
                && Document.DocxPackage != null)
                doc.DocxPackage = Document.DocxPackage.Clone();
            else if (doc.DocxPackage != null
                && Document.DocxPackage != null)
                UpdateXmlParts(doc);
        }
        /// <summary>
        /// Updates the XML parts.
        /// </summary>
        /// <param name="destination">The destination.</param>
        private void UpdateXmlParts(WordDocument destination)
        {
            if (Relations.Count == 0)
                return;
            string[] keys = new string[Relations.Count];
            Relations.Keys.CopyTo(keys, 0);
            for (int i = 0; i < keys.Length; i++)
            {
                DictionaryEntry itemRelation = Relations[keys[i]];
                string[] parts = itemRelation.Value.ToString().Split('/');
                PartContainer srcContainer = Document.DocxPackage.FindPartContainer("word/");
                PartContainer destContainer = destination.DocxPackage.FindPartContainer("word/");
                string newPart = UpdateXmlPartContainer(Document.DocxPackage, srcContainer, destContainer, parts, 0);
                if (newPart != string.Empty)
                {
                    itemRelation.Value = itemRelation.Value.ToString().Replace(parts[parts.Length - 1], newPart);
                    Relations[keys[i]] = itemRelation;
                }
            }
        }
        /// <summary>
        /// Updates the XML part container.
        /// </summary>
        /// <param name="srcPackage">The SRC package.</param>
        /// <param name="srcContainer">The SRC container.</param>
        /// <param name="destContainer">The dest container.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private string UpdateXmlPartContainer(Package srcPackage, PartContainer srcContainer, PartContainer destContainer, string[] parts, int index)
        {
            string newPart = string.Empty;
            for (int i = index; i < parts.Length; i++)
            {
                if (i < parts.Length - 1)
                {
                    string containerName = parts[i] + "/";
                    if (destContainer.XmlPartContainers.ContainsKey(containerName))
                    {
                        destContainer = destContainer.XmlPartContainers[containerName];
                        srcContainer = srcContainer.XmlPartContainers[containerName];
                        //Updates xml part from source to destination document.
                        newPart = UpdateXmlPartContainer(srcPackage, srcContainer, destContainer, parts, i + 1);
                        break;
                    }
                    else
                    {
                        PartContainer newContainer = new PartContainer();
                        newContainer.Name = containerName;
                        newPart = srcContainer.XmlPartContainers[containerName].CopyXmlPartContainer(newContainer, srcPackage, parts, i + 1);
                        destContainer.XmlPartContainers.Add(containerName, newContainer);
                        break;
                    }
                }
                else
                {
                    newPart = srcContainer.CopyXmlPartItems(destContainer, srcPackage, parts[i]);
                }
            }
            return newPart;
        }
        #endregion
    }
}

