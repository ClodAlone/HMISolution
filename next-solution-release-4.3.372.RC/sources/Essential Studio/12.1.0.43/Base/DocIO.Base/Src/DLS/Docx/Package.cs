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

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for Package.
    /// </summary>
    internal class Package : PartContainer
    {
        #region Implementation
        /// <summary>
        /// Gets the part container by name.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        internal PartContainer FindPartContainer(string containerName)
        {
            string[] nameParts = containerName.Split('/');
            return EnsurePartContainer(nameParts, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPartName"></param>
        /// <returns></returns>
        internal Part FindPart(string fullPartName)
        {
            int lastPathIndex = fullPartName.LastIndexOf("/");
            string partContName = fullPartName.Substring(0, lastPathIndex + 1);

            PartContainer partCont = FindPartContainer(partContName);
            if (partCont != null)
            {
                string partName = fullPartName.Substring(lastPathIndex + 1, fullPartName.Length - (lastPathIndex + 1));
                if (partCont.XmlParts.ContainsKey(partName))
                {
                    return partCont.XmlParts[partName];
                }
            }

            return null;
        }
        /// <summary>
        /// Loads the specified zip archive.
        /// </summary>
        /// <param name="zipArc">The zip archive.</param>
        internal void Load(ZipArchive zipArc)
        {
            for (int i = 0, cnt = zipArc.Count; i < cnt; i++)
            {
                LoadPart(zipArc[i]);
            }
        }
        /// <summary>
        /// Loads the part.
        /// </summary>
        /// <param name="item">The item.</param>
        private void LoadPart(ZipArchiveItem item)
        {
            string[] nameParts = item.ItemName.Split('/');
            string itemName = nameParts[nameParts.Length - 1];

            PartContainer container = EnsurePartContainer(nameParts, 0);
            if (!(item.ExternalAttributes.ToString().Contains("Directory")))
            {
                // If current item is relations load it as relations
                if (nameParts.Length > 1 &&
                  nameParts[nameParts.Length - 2].EndsWith("_rels"))
                {
                    container.LoadRelations(item);
                }
                else
                {
                     container.AddPart(item);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        new internal Package Clone()
        {
            Package newPackage = new Package();
            newPackage.Name = m_name;

            // Clone Parts
            if (m_xmlParts != null && m_xmlParts.Count > 0)
            {
                foreach (string key in m_xmlParts.Keys)
                {
                    newPackage.XmlParts.Add(key, m_xmlParts[key].Clone());
                }
            }

            // Clone Relations
            if (m_relations != null && m_relations.Count > 0)
            {
                foreach (string key in m_relations.Keys)
                {
                    newPackage.Relations.Add(key, m_relations[key].Clone() as Relations);
                }
            }

            // Clone PartContainers
            if (m_xmlPartContainers != null && m_xmlPartContainers.Count > 0)
            {
                foreach (string key in m_xmlPartContainers.Keys)
                {
                    newPackage.XmlPartContainers.Add(key, m_xmlPartContainers[key].Clone());
                }
            }

            return newPackage;
        }
        #endregion
    }
}

