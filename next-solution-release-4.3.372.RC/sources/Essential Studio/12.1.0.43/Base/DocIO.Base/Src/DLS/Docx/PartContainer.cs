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
using Syncfusion.Compression.Zip;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS.Convertors;
using System.Xml;
using System.IO;
using System.Text;

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for PartContainer.
    /// </summary>
    internal class PartContainer
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        protected string m_name;
        protected Dictionary<string, Part> m_xmlParts;
        protected Dictionary<string, PartContainer> m_xmlPartContainers;
        protected Dictionary<string, Relations> m_relations = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the XML parts.
        /// </summary>
        /// <value>The XML parts.</value>
        internal Dictionary<string, Part> XmlParts
        {
            get
            {
                if (m_xmlParts == null)
                {
                    m_xmlParts = new Dictionary<string, Part>();
                }
                return m_xmlParts;
            }
        }
        /// <summary>
        /// Gets the XML part containers.
        /// </summary>
        /// <value>The XML part containers.</value>
        internal Dictionary<string, PartContainer> XmlPartContainers
        {
            get
            {
                if (m_xmlPartContainers == null)
                {
                    m_xmlPartContainers = new Dictionary<string, PartContainer>();
                }

                return m_xmlPartContainers;
            }
        }
        /// <summary>
        /// Gets the XML relations collection.
        /// </summary>
        /// <value>The XML relations.</value>
        internal Dictionary<string, Relations> Relations
        {
            get
            {
                if (m_relations == null)
                {
                    m_relations = new Dictionary<string, Relations>();
                }

                return m_relations;
            }
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        internal string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the part.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void AddPart(ZipArchiveItem item)
        {
            Part part = new Part(item.DataStream);
            part.Name = GetPartName(item.ItemName);
            AddPart(part);
        }
        /// <summary>
        /// Adds the part.
        /// </summary>
        /// <param name="xmlPart">The XML part.</param>
        internal void AddPart(Part xmlPart)
        {
            XmlParts.Add(xmlPart.Name, xmlPart);
        }
        /// <summary>
        /// Adds the part container.
        /// </summary>
        /// <param name="container">The container.</param>
        internal void AddPartContainer(PartContainer container)
        {
            XmlPartContainers.Add(container.Name, container);
        }
        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="nameParts">The name parts.</param>
        /// <param name="startNameIndex">Start index of the name.</param>
        /// <returns></returns>
        internal PartContainer EnsurePartContainer(string[] nameParts, int startNameIndex)
        {
            if (nameParts.Length == 1)
                return this;

            string containerName = nameParts[startNameIndex] + "/";
            PartContainer container = null;
            if (XmlPartContainers.ContainsKey(containerName))
            {
                container = XmlPartContainers[containerName];
            }

            // Adds container if it not exists
            if (container == null)
            {
                if (containerName.EndsWith("_rels/"))
                {
                    return this;
                }
                else
                {
                    container = new PartContainer();
                    container.Name = containerName;
                    AddPartContainer(container);
                }
            }

            // Finds next sub container...
            if (startNameIndex < nameParts.Length - 2)
            {
                return container.EnsurePartContainer(nameParts, ++startNameIndex);
            }

            return container;
        }
        /// <summary>
        /// Loads the relations.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void LoadRelations(ZipArchiveItem item)
        {
            Relations rels = new Relations(item);
            Relations.Add(item.ItemName, rels);
        }
        /// <summary>
        /// Get xml part name
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        private string GetPartName(string fullPath)
        {
            int startIndex = fullPath.LastIndexOf('/') + 1;
            int nameLen = fullPath.Length - startIndex;
            return fullPath.Substring(startIndex, nameLen);
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal PartContainer Clone()
        {
            PartContainer newContainer = new PartContainer();
            newContainer.Name = m_name;

            // Clone Parts
            if (m_xmlParts != null && m_xmlParts.Count > 0)
            {
                foreach (string key in m_xmlParts.Keys)
                {
                    newContainer.XmlParts.Add(key, m_xmlParts[key].Clone());
                }
            }

            // Clone Relations
            if (m_relations != null && m_relations.Count > 0)
            {
                foreach (string key in m_relations.Keys)
                {
                    newContainer.Relations.Add(key, m_relations[key].Clone() as Relations);
                }
            }

            // Clone PartContainers
            if (m_xmlPartContainers != null && m_xmlPartContainers.Count > 0)
            {
                foreach (string key in m_xmlPartContainers.Keys)
                {
                    newContainer.XmlPartContainers.Add(key, m_xmlPartContainers[key].Clone());
                }
            }

            return newContainer;
        }
        /// <summary>
        /// Copies the XML part container.
        /// </summary>
        /// <param name="newContainer">The new container.</param>
        /// <param name="srcPackage">The SRC package.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal string CopyXmlPartContainer(PartContainer newContainer, Package srcPackage, string[] parts, int index)
        {
            string newPartName = "";
            for (int i = 0; i < parts.Length; i++)
            {
                if (i < index)
                    continue;
                if (i == parts.Length - 1)
                    newPartName = CopyXmlPartItems(newContainer, srcPackage, parts[i]);
                else
                {
                    string containerName = parts[i] + "/";
                    PartContainer container = new PartContainer();
                    newPartName = m_xmlPartContainers[containerName].CopyXmlPartContainer(container, srcPackage, parts, i + 1);
                    newContainer.XmlPartContainers.Add(containerName, container);
                    break;
                }
            }
            return newPartName;
        }
        /// <summary>
        /// Copies the XML part items.
        /// </summary>
        /// <param name="newContainer">The new container.</param>
        /// <param name="srcPackage">The SRC package.</param>
        /// <param name="partName">Name of the part.</param>
        internal string CopyXmlPartItems(PartContainer newContainer, Package srcPackage, string partName)
        {
            string newPartName = "";
            if (newContainer.XmlParts.ContainsKey(partName))
            {
                string extension = Path.GetExtension(partName);
                string partNameWitoutExt = partName.Replace(extension, "");
                string prefix = partNameWitoutExt.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                int id = 0;
                int.TryParse(partNameWitoutExt.Replace(prefix, ""), out id);
                newPartName = prefix + id.ToString() + extension;
                while (newContainer.XmlParts.ContainsKey(newPartName))
                {
                    id++;
                    newPartName = prefix + id.ToString() + extension;
                }
            }
            // Clone Parts
            if (m_xmlParts != null && m_xmlParts.ContainsKey(partName))
            {
                Part newPart = m_xmlParts[partName].Clone();
                if (!string.IsNullOrEmpty(newPartName))
                    newPart.Name = newPartName;
                //Updates xml part from source to destination document.
                newContainer.XmlParts.Add(newPart.Name, newPart);
            }
            // Copy Relations
            if (m_relations != null)
            {
                string xmlPartRelationKey = GetXmlPartRelationKey(partName);
                if (m_relations.ContainsKey(xmlPartRelationKey))
                {
                    Relations newRelation = m_relations[xmlPartRelationKey].Clone() as Relations;
                    if (!string.IsNullOrEmpty(newPartName))
                        newRelation.Name = xmlPartRelationKey.Replace(partName + ".rels", newPartName + ".rels");
                    if (newContainer.Relations.ContainsKey(newRelation.Name))
                        newContainer.Relations.Remove(newRelation.Name);
                    newContainer.Relations.Add(newRelation.Name, newRelation);
                    //Handles copying of internally related items.
                    Dictionary<string, string> innerRelationTarget = CopyInnerRelatedXmlParts(newContainer, srcPackage, newRelation, partName);
                    UpdateInnerRelationTarget(newRelation, innerRelationTarget);
                }
            }
            return newPartName;
        }
        /// <summary>
        /// Gets the XML part relation key.
        /// </summary>
        /// <param name="partName">Name of the part.</param>
        /// <returns></returns>
        internal string GetXmlPartRelationKey(string partName)
        {
            string xmlPartRelationKey = "";
            if (m_relations == null)
                return xmlPartRelationKey;
            foreach (string key in m_relations.Keys)
            {
                if (key.EndsWith(partName + ".rels"))
                {
                    xmlPartRelationKey = key;
                    break;
                }
            }
            return xmlPartRelationKey;
        }
        /// <summary>
        /// Updates the inner relation target.
        /// </summary>
        /// <param name="relation">The relation.</param>
        /// <param name="innerRelationTarget">The inner relation target.</param>
        private void UpdateInnerRelationTarget(Relations relation, Dictionary<string, string> innerRelationTarget)
        {
            Stream stream = relation.DataStream;
            stream.Position = 0;
            XmlReader reader = UtilityMethods.CreateReader(stream);
            MemoryStream outputStream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(outputStream, settings);
            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            switch (reader.LocalName)
                            {
                                case "target":
                                case "Target":
                                    writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI,
                                        (innerRelationTarget.ContainsKey(reader.Value) ? innerRelationTarget[reader.Value] : reader.Value));
                                    break;
                                default:
                                    writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                                    break;
                            }
                        }
                        reader.MoveToElement();
                        if (reader.IsEmptyElement)
                            writer.WriteEndElement();
                        break;
                    case XmlNodeType.Text:
                        writer.WriteString(reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        writer.WriteEndElement();
                        break;
                    case XmlNodeType.SignificantWhitespace:
                        writer.WriteWhitespace(reader.Value);
                        break;
                    default:
                        break;
                }
            } while (reader.Read());
            writer.Flush();
            outputStream.Flush();
            outputStream.Position = 0;
            relation.SetDataStream(outputStream);
        }
        /// <summary>
        /// Copies the inner related XML parts.
        /// </summary>
        /// <param name="newContainer">The new container.</param>
        /// <param name="srcPackage">The SRC package.</param>
        /// <param name="relation">The relation.</param>
        /// <param name="curPartName">Name of the cur part.</param>
        /// <returns></returns>
        private Dictionary<string, string> CopyInnerRelatedXmlParts(PartContainer newContainer, Package srcPackage, Relations relation, string curPartName)
        {
            Dictionary<string, string> innerRelationTarget = new Dictionary<string, string>();
            Stream stream = relation.DataStream;
            stream.Position = 0;
            XmlReader xmlReader = UtilityMethods.CreateReader(stream);

            xmlReader.MoveToContent();
            if (xmlReader.LocalName != "Relationships")
            {
                xmlReader.ReadInnerXml();
                return innerRelationTarget;
            }
            if (xmlReader.IsEmptyElement)
                return innerRelationTarget;
            do
            {
                xmlReader.Read();
                string targetMode = xmlReader.GetAttribute("TargetMode");
                bool isExternal = (targetMode == "External") ? true : false;
                if (!isExternal)
                {
                    string target = xmlReader.GetAttribute("Target");
                    if (!string.IsNullOrEmpty(target))
                    {
                        if (target.StartsWith("../"))
                        {
                            PartContainer srcPartContainer = srcPackage.GetXmlPartContainer(this, target);
                            string partName = target.Substring(target.LastIndexOf('/') + 1);
                            PartContainer container = null;
                            if (newContainer.XmlPartContainers.ContainsKey("embeddings/"))
                                container = newContainer.XmlPartContainers["embeddings/"];
                            else
                            {
                                container = new PartContainer();
                                container.Name = "embeddings/";
                                newContainer.XmlPartContainers.Add(container.Name, container);
                            }
                            string newPartName = srcPartContainer.CopyXmlPartItems(container, srcPackage, partName);
                            if (!string.IsNullOrEmpty(newPartName))
                                innerRelationTarget.Add(target, container.Name + newPartName);
                            else
                                innerRelationTarget.Add(target, container.Name + partName);
                        }
                        else
                        {
                            string[] parts = target.Split('/');
                            //Updates unique name for parts and adds old & new name to dictionary for updating target in relations
                            string newPartName = CopyXmlPartContainer(newContainer, srcPackage, parts, 0);
                            if (!string.IsNullOrEmpty(newPartName))
                                innerRelationTarget.Add(target, target.Replace(parts[parts.Length - 1], newPartName));
                        }
                    }
                }
            }
            while (xmlReader.LocalName != "Relationships");
            return innerRelationTarget;
        }
        /// <summary>
        /// Gets the part container by name.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        internal PartContainer GetXmlPartContainer(PartContainer srcContainer, string target)
        {
            string targetPath = GetXmlPartContainerPath(srcContainer, target);
            string[] parts = targetPath.Split('/');
            return EnsurePartContainer(parts, 0);
        }
        /// <summary>
        /// Gets the XML part container path.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        internal string GetXmlPartContainerPath(PartContainer container, string target)
        {
            string[] parts = target.Split('/');
            int moveParentCount = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Trim('.') == "")
                    moveParentCount++;
                else
                    break;
            }
            string targetPath = GetParentPartPath(container);
            while (moveParentCount > 1)
            {
                targetPath = targetPath.TrimEnd('/');
                if (targetPath.Contains("/"))
                    targetPath = targetPath.Remove(targetPath.LastIndexOf('/'));
                else
                    targetPath = "";
                moveParentCount--;
            }
            targetPath += target.TrimStart(new char[] { '.', '/' });
            return targetPath;
        }
        /// <summary>
        /// Gets the parent part path.
        /// </summary>
        /// <param name="srcContainer">The SRC container.</param>
        /// <returns></returns>
        private string GetParentPartPath(PartContainer srcContainer)
        {
            string targetPath = "";
            if (m_xmlPartContainers == null)
                return targetPath;
            if (m_xmlPartContainers.ContainsValue(srcContainer))
                return targetPath + Name;
            foreach (KeyValuePair<string, PartContainer> keyValue in m_xmlPartContainers)
            {
                if (keyValue.Value.m_xmlPartContainers == null || keyValue.Value.m_xmlPartContainers.Count == 0)
                    continue;
                string tempPath = keyValue.Value.GetParentPartPath(srcContainer);
                if (!string.IsNullOrEmpty(tempPath))
                {
                    targetPath += Name + tempPath;
                    break;
                }
            }
            return targetPath;
        }
        #endregion
    }
}

