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

using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents names dictionary of the document's catalog entry.
    /// </summary>
    /// <seealso cref="IPdfWrapper"/> Interface    
    internal class PdfCatalogNames : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store collection of attachments.
        /// </summary>
        private PdfAttachmentCollection m_attachments = null;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCatalogNames"/> class.
        /// </summary>
        public PdfCatalogNames()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCatalogNames"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        public PdfCatalogNames(PdfDictionary root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            m_dictionary = root;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the embedded files.
        /// </summary>
        /// <value>The embedded files.</value>
        public PdfAttachmentCollection EmbeddedFiles
        {
            get
            {
                return m_attachments;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("EmbeddedFiles");
                }

                if (m_attachments != value)
                {
                    m_attachments = value;
                    m_dictionary.SetProperty(DictionaryProperties.EmbeddedFiles,
                        new PdfReferenceHolder(m_attachments));
                }
            }
        }

        /// <summary>
        /// Gets the destinations.
        /// </summary>
        internal PdfDictionary Destinations
        {
            get
            {
                IPdfPrimitive obj = PdfCrossTable.Dereference(m_dictionary[DictionaryProperties.Dests]);
                PdfDictionary dests = obj as PdfDictionary;
                return dests;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the named object from a tree.
        /// </summary>
        /// <param name="root">The tree root.</param>
        /// <param name="name">The name.</param>
        /// <returns>The named object.</returns>
        internal IPdfPrimitive GetNamedObjectFromTree(PdfDictionary root, PdfString name)
        {
            bool found = false;
            PdfDictionary current = root;
            IPdfPrimitive obj = null;

            while (!found && current != null)
            {
                if (current.ContainsKey(DictionaryProperties.Kids))
                {
                    current = GetProperKid(current, name);
                }
                else if (current.ContainsKey(DictionaryProperties.Names))
                {
                    obj = FindName(current, name);
                    found = true;
                }
            }

            return obj;
        }

        /// <summary>
        /// Finds the name in the tree.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="name">The name.</param>
        /// <returns>The object specified by its name or null.</returns>
        private IPdfPrimitive FindName(PdfDictionary current, PdfString name)
        {
            PdfArray names = PdfCrossTable.Dereference(current[DictionaryProperties.Names]) as PdfArray;

            int halfLength = names.Count / 2;
            int lowIndex = 0, topIndex = halfLength - 1, half = 0;
            bool found = false;

            while (!found)
            {
                half = (lowIndex + topIndex) / 2;

                if (lowIndex > topIndex)
                {
                    break;
                }

                PdfString str = PdfCrossTable.Dereference(names[half * 2]) as PdfString;
                int cmp = PdfString.ByteCompare(name, str);

                if (cmp > 0)
                {
                    lowIndex = half + 1;
                }
                else if (cmp < 0)
                {
                    topIndex = half - 1;
                }
                else
                {
                    found = true;
                    break;
                }
            }

            IPdfPrimitive obj = null;

            if (found)
            {
                obj = PdfCrossTable.Dereference(names[half * 2 + 1]);
            }

            return obj;
        }

        /// <summary>
        /// Gets the proper kid from an array.
        /// </summary>
        /// <param name="current">The current node.</param>
        /// <param name="name">The name we're looking for.</param>
        /// <returns>The proper kid.</returns>
        /// <remarks>The name should be within the kid limits.</remarks>
        private PdfDictionary GetProperKid(PdfDictionary current, PdfString name)
        {
            PdfArray kids = PdfCrossTable.Dereference(current[DictionaryProperties.Kids]) as PdfArray;
            PdfDictionary kid = null;

            foreach (IPdfPrimitive obj in kids)
            {
                kid = PdfCrossTable.Dereference(obj) as PdfDictionary;

                if (CheckLimits(kid, name))
                {
                    break;
                }
                else
                {
                    kid = null;
                }
            }

            return kid;
        }

        /// <summary>
        /// Checks the limits of the named tree node.
        /// </summary>
        /// <param name="kid">The kid.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns true if the kid should have the name (the name is within its limits).</returns>
        private bool CheckLimits(PdfDictionary kid, PdfString name)
        {
            IPdfPrimitive obj = kid[DictionaryProperties.Limits];
            PdfArray limits = obj as PdfArray;
            bool result = false;

            if (limits != null && limits.Count >= 2)
            {
                obj = limits[0];

                PdfString lowerLimit = obj as PdfString;

                obj = limits[1];

                PdfString higherLimit = obj as PdfString;
                int lowCmp = PdfString.ByteCompare(lowerLimit, name);
                int hiCmp = PdfString.ByteCompare(higherLimit, name);

                if (lowCmp == 0 || hiCmp == 0)
                {
                    result = true;
                }
                else if (lowCmp < 0 && hiCmp > 0)
                {
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Merges the embedded name trees.
        /// </summary>
        /// <param name="names">The names.</param>
        internal void MergeEmbedded(PdfCatalogNames names, PdfCrossTable crossTable)
        {
            List<IPdfPrimitive> al = names.GetEmbedded();

            AppendEmbedded(al, crossTable);
        }

        /// <summary>
        /// Appends the list of embedded file dictionaries.
        /// </summary>
        /// <param name="embedded">The list.</param>
        private void AppendEmbedded(List<IPdfPrimitive> embedded, PdfCrossTable crossTable)
        {
            if (embedded != null && embedded.Count > 0)
            {
                IPdfPrimitive obj = m_dictionary[DictionaryProperties.EmbeddedFiles];
                PdfDictionary root = PdfCrossTable.Dereference(obj) as PdfDictionary;

                if (root == null)
                {
                    root = new PdfDictionary();
                    root[DictionaryProperties.Names] = new PdfArray();
                    m_dictionary[DictionaryProperties.EmbeddedFiles] = new PdfReferenceHolder(root);
                }

                string baseName = string.Empty;
                PdfDictionary node = null;

                if (root.ContainsKey(DictionaryProperties.Names))
                {
                    node = root;
                    baseName = GetNodeRightLimit(node);
                }
                else if (root.ContainsKey(DictionaryProperties.Kids))
                {
                    // Get the latest tree leaf node name.
                    PdfArray rootKids = PdfCrossTable.Dereference(root[DictionaryProperties.Kids]) as PdfArray;
                    node = PdfCrossTable.Dereference(rootKids[rootKids.Count - 1]) as PdfDictionary;
                    baseName = GetNodeRightLimit(node);
                    // Create a new name base.
                    node = new PdfDictionary();
                    node[DictionaryProperties.Names] = new PdfArray();
                    rootKids.Add(new PdfReferenceHolder(node));
                }

                // Append names to the tree.
                AppendObjects(baseName, node, embedded, (node != root), crossTable);
            }
        }

        /// <summary>
        /// Gets the node left limit.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The node left limit.</returns>
        private string GetNodeRightLimit(PdfDictionary node)
        {
            PdfArray limits = PdfCrossTable.Dereference(node[DictionaryProperties.Limits]) as PdfArray;
            PdfString rightLimit = null;

            if (limits != null)
            {
                rightLimit = PdfCrossTable.Dereference(limits[1]) as PdfString;
            }

            string retValue = string.Empty;

            if (rightLimit != null)
            {
                retValue = rightLimit.Value;
            }
            else if (node.ContainsKey(DictionaryProperties.Names))
            {
                PdfArray names = PdfCrossTable.Dereference(node[DictionaryProperties.Names]) as PdfArray;

                if (names.Count > 1)
                {
                    rightLimit = PdfCrossTable.Dereference(names[names.Count - 2]) as PdfString;
                    retValue = rightLimit.Value;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Appends the objects to the node of the name tree.
        /// </summary>
        /// <param name="baseName">Name of the base.</param>
        /// <param name="node">The node.</param>
        /// <param name="embedded">The embedded.</param>
        /// <param name="updateLimits">if set to <c>true</c> the limits should be updated.</param>
        private void AppendObjects(string baseName, PdfDictionary node, List<IPdfPrimitive> embedded, bool updateLimits, PdfCrossTable crossTable)
        {
            PdfArray names = PdfCrossTable.Dereference(node[DictionaryProperties.Names]) as PdfArray;
            int count = embedded.Count, start = 0;

            for (int i = 0; i < count; i++)
            {
                //names.Add(new PdfString(baseName + i.ToString("X")));
                //names.Add(embedded[i] as IPdfPrimitive);
                if (crossTable == null)
                    names.Add(embedded[i]);
                else
                    names.Add(embedded[i].Clone(crossTable));
            }

            if (updateLimits)
            {
                PdfString hiLimit = new PdfString(baseName + count.ToString("X"));
                PdfString loLimit = new PdfString(baseName + start.ToString("X"));

                PdfArray limits = new PdfArray();

                node[DictionaryProperties.Limits] = limits;
                limits.Add(loLimit);
                limits.Add(hiLimit);
            }
        }

        /// <summary>
        /// Generates the list of embedded file dictionaries.
        /// </summary>
        /// <returns>The list of embedded file dictionaries.</returns>
        private List<IPdfPrimitive> GetEmbedded()
        {
            IPdfPrimitive obj = m_dictionary[DictionaryProperties.EmbeddedFiles];
            PdfDictionary root = PdfCrossTable.Dereference(obj) as PdfDictionary;

            List<IPdfPrimitive> embedded = null;

            if (root != null)
            {
                if (m_dictionary.ContainsKey(DictionaryProperties.EmbeddedFiles))
                {
                    embedded = new List<IPdfPrimitive>();
                    Stack<NodeInfo> stack = new Stack<NodeInfo>();
                    PdfDictionary node = root;
                    NodeInfo ni = new NodeInfo(0, 1);

                    do
                    {
                        for (; ni.Index < ni.Count; ++ni.Index)
                        {
                            if (ni.Kids != null)
                            {
                                node = PdfCrossTable.Dereference(ni.Kids[ni.Index]) as PdfDictionary;
                            }

                            if (node.ContainsKey(DictionaryProperties.Kids))
                            {
                                stack.Push(ni);

                                ni = new NodeInfo(node);
                                break;
                            }
                            else if (node.ContainsKey(DictionaryProperties.Names))
                            {
                                // Collect embedded dictionaries
                                CollectObjects(node, embedded);
                                // Go to the parent (actually move one level up and switch to a sibling)
                                if (stack.Count > 0)
                                {
                                    ni = stack.Pop();
                                    continue;
                                }
                            }
                        }
                    } while (stack.Count > 0);
                }
            }

            return embedded;
        }

        /// <summary>
        /// Collects the objects.
        /// </summary>
        /// <param name="leafNode">The leaf node.</param>
        /// <param name="array">The array.</param>
        private void CollectObjects(PdfDictionary leafNode, List<IPdfPrimitive> array)
        {
            IPdfPrimitive obj = leafNode[DictionaryProperties.Names];
            PdfArray names = PdfCrossTable.Dereference(obj) as PdfArray;

            for (int i = 0, count = names.Count; i < count; i ++)
            {
                array.Add(names[i]);
            }
        }

        /// <summary>
        /// Clear catalog names.
        /// </summary>
        internal void Clear()
        {
            if (m_attachments != null)
                m_attachments.Clear();

            if (m_dictionary != null)
                m_dictionary.Clear();
        }
        #endregion

        #region Internals
        /// <summary>
        /// Holds info about current base node and its current child.
        /// </summary>
        private class NodeInfo
        {
            #region Members
            /// <summary>
            /// Internal variable to store Dictionary entry.
            /// </summary>
            public PdfDictionary Node;

            /// <summary>
            /// Internal variable to store index value.
            /// </summary>
            public int Index;

            /// <summary>
            /// Internal variable to store dictionary entries count.
            /// </summary>
            public int Count;

            /// <summary>
            /// Internal variable to store Kids value.
            /// </summary>
            public PdfArray Kids;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="NodeInfo"/> class.
            /// </summary>
            /// <param name="node">The node.</param>
            public NodeInfo(PdfDictionary node)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }

                Node = node;

                IPdfPrimitive obj = node[DictionaryProperties.Kids];
                Kids = PdfCrossTable.Dereference(obj) as PdfArray;

                Count = Kids.Count;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NodeInfo"/> class.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="count">The count.</param>
            public NodeInfo(int index, int count)
            {
                Index = index;
                Count = count;
            }
            #endregion
        }
        #endregion
    }
}
