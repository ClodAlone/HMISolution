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
using System.Text.RegularExpressions;
#endregion

namespace Syncfusion.DocIO.DLS
{
   
    internal class AlternateChunk : TextBodyItem
    {
        #region Fields
        string m_targetId;
        string m_contentPath;
        string m_contentType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / sets the targer Id.
        /// </summary>
        /// <value>The Targer id.</value>
        internal string TargetId
        {
            get
            {
                return m_targetId;
            }
            set
            {
                m_targetId = value;
            }
        }
        /// <summary>
        /// Gets / sets the Content extension.
        /// </summary>
        /// <value>The Content extension.</value>
        internal string ContentExtension
        {
            get
            {
                if (m_contentPath != null)
                    return Path.GetExtension(m_contentPath).Replace(".", "");
                else
                    return string.Empty;
            }
           
        }
        /// <summary>
        /// Gets / sets the Content Type.
        /// </summary>
        /// <value>The Content Type.</value>
        internal string ContentType
        {
            get
            {
                return m_contentType;
            }
            set
            {
                m_contentType = value;
            }
        }
        /// <summary>
        /// Gets / sets the Content Path.
        /// </summary>
        /// <value>The Content Path.</value>
        internal string ContentPath
        {
            get
            {
                return m_contentPath;
            }
            set
            {
                m_contentPath = value;
            }
        }
         /// <summary>
        /// Gets the alternate chunk stream.
        /// </summary>
        /// <value>The alternate chunk stream.</value>
        internal Stream Stream
        {
            get
            {
                return Document.DocxPackage.FindPart(m_contentPath).DataStream;
            }
        }
        #endregion

        # region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AlternateChunk"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal AlternateChunk(WordDocument doc)
            : base((WordDocument)doc)
        {
            //TODO: Need to implement
        }
        # endregion

        # region Implementation
        internal AlternateChunk Clone()
        {
            return (AlternateChunk)CloneImpl();
        }
        /// <summary>
        /// Creates a duplicate of the entity.
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            AlternateChunk altChunk = (AlternateChunk)base.CloneImpl();
            return altChunk;
        }
        /// <summary>
        /// Gets Next the text body item in the document.
        /// </summary>
        /// <returns></returns>
        internal override TextBodyItem GetNextTextBodyItem()
        {
            if (this.NextSibling != null)
                return this.NextSibling as TextBodyItem;

            if (this.Owner is WTableCell)
            {
                return (this.Owner as WTableCell).GetNextTextBodyItem();
            }
            else if (this.Owner is WTextBody)
            {
                if (this.OwnerTextBody.Owner is WTextBox)
                    return (this.OwnerTextBody.Owner as WTextBox).GetNextTextBodyItem();
                else if (this.OwnerTextBody.Owner is WSection)
                    return GetNextInSection(this.OwnerTextBody.Owner as WSection);
            }

            return null;
        }
        /// <summary>
        /// Checks a value indicating whether this item was deleted from the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>
        /// <value>
        /// 	if this instance is delete revision, set to <c>true</c>.
        /// </value>
        internal override bool CheckDeleteRev()
        {
            //TODO: Need to implement
            return false;

        }
        /// <summary>
        /// Sets the changed Paragraph format for table.
        /// </summary>
        /// <param name="check">if it specifies the format to be changed, set to <c>true</c>.</param>
        internal override void SetChangedPFormat(bool check)
        {
            //TODO: Need to implement
        }
        /// <summary>
        /// Sets the changed C format.
        /// </summary>
        /// <param name="check">if it specifies formatting, set to <c>true</c>.</param>
        internal override void SetChangedCFormat(bool check)
        {
            //TODO: Need to be implemented
        }

        /// <summary>
        /// Sets the delete rev.
        /// </summary>
        /// <param name="check">if specifies delete revision, set to <c>true</c>.</param>
        internal override void SetDeleteRev(bool check)
        {
            //TODO: Need to implement
        }
        /// <summary>
        /// Sets the insert rev.
        /// </summary>
        /// <param name="check">if it specifies insert revision, set to <c>true</c>.</param>
        internal override void SetInsertRev(bool check)
        {
            //TODO: Need to implement
        }
        /// <summary>
        /// Determines whether item has tracked changes.
        /// </summary>
        /// <returns>
        /// 	if has tracked changes, set to <c>true</c>.
        /// </returns>
        internal override bool HasTrackedChanges()
        {
            //TODO: Need to implement
            return false;
        }

        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, string replace)
        {
            //TODO: Need to be implemented
            return 1;
        }
        /// <summary>
        /// Replaces all entries of given string with replace string, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given text to replace.</param>
        /// <param name="replace">The replace text .</param>
        /// <param name="caseSensitive">if specifies case sensitive, set to <c>true</c> .</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public override int Replace(string given, string replace, bool caseSensitive, bool wholeWord)
        {
            //TODO: Need to be implemented
            return 0;

        }
        /// <summary>
        /// Replaces all entries of given regular expression with TextRangesHolder.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, TextSelection textSelection)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces all entries of given regular expression with TextRangesHolder.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public override int Replace(Regex pattern, TextSelection textSelection, bool saveFormatting)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces all entries of given string with TextRangesHolder, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to check whole word, set to <c>true</c> .</param>
        public int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces all entries of given string with TextRangesHolder, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c> .</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c> .</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord, bool saveFormatting)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces first entry of given string with replace string, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The string to replace</param>
        /// <param name="replace">Replace string</param>
        /// <param name="caseSensitive">Is case sensitive replace?</param>
        /// <param name="wholeWord">Search for whole word?</param>
        /// <returns></returns>
        internal int ReplaceFirst(string given, string replace, bool caseSensitive, bool wholeWord)
        {
            //TODO: Need to be implemented
            return 0;
        }
        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="replace"></param>
        internal int ReplaceFirst(Regex pattern, string replace)
        {
            //TODO: Need to be implemented
            return 0;
        }
#if !SILVERLIGHT && !WP
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo();
            //TODO: Need to be implemented
        }
#endif
        /// <summary>
        /// Removes the character format changes.
        /// </summary>
        internal override void RemoveCFormatChanges()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Removes the paragraph/table format changes.
        /// </summary>
        internal override void RemovePFormatChanges()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Accepts the changes for character format.
        /// </summary>
        internal override void AcceptCChanges()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Accepts changes in paragraph/table format.
        /// </summary>
        internal override void AcceptPChanges()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Defines whether format was changed.
        /// </summary>
        /// <returns></returns>
        internal override bool CheckChangedCFormat()
        {
            //TODO: Need to be implemented

            return false;
        }
        /// <summary>
        /// Checks a value indicating whether this item was inserted to the document,
        /// when "Track Changes" is or was set to "true".
        /// </summary>
        /// <returns></returns>
        /// <value>
        /// 	if this instance was inserted, set to <c>true</c>.
        /// </value>
        internal override bool CheckInsertRev()
        {
            //TODO: Need to be implemented
            return false;
        }
        /// <summary>
        /// Returns first entry of given regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public override TextSelection Find(Regex pattern)
        {
            //TODO: Need to be implemented
            return null;
        }
        /// <summary>
        /// Returns first entry of given string, taking into consideration caseSensitive
        /// and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="caseSensitive">if it is case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public TextSelection Find(string given, bool caseSensitive, bool wholeWord)
        {
            //TODO: Need to be implemeneted
            return null;
        }
        /// <summary>
        /// Accepts or rejects changes tracked from the moment of last change acceptance.
        /// </summary>
        /// <param name="acceptChanges">if it accepts changes, set to <c>true</c>.</param>
        internal override void MakeChanges(bool acceptChanges)
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Returns all entries of given regex.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        internal override TextSelectionList FindAll(Regex pattern)
        {
            //TODO: Need to be implemented
            return null;
        }
        /// <summary>
        /// Closes the item.
        /// </summary>
        internal override void Close()
        {
            //TODO: Need to be implemented
        }
        /// <summary>
        /// Defines whether paragraph format is changed.
        /// </summary>
        /// <returns></returns>
        internal override bool CheckChangedPFormat()
        {
            //TODO: Need to be implemented
            return false;
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.AlternateChunk;
            }
        }
        /// <summary>
        /// Gets the child entities.
        /// </summary>
        /// <value>The child entities.</value>
        public EntityCollection ChildEntities
        {
            get
            {
                return null;
            }
        }
        # endregion

#if !SILVERLIGHT && !WP
        #region Implementation / overrides
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            
            base.CloneRelationsTo(doc, nextOwner);
            if (doc.DocxPackage == null
                && Document.DocxPackage != null)
                doc.DocxPackage = Document.DocxPackage.Clone();
            else if (doc.DocxPackage != null
                && Document.DocxPackage != null)
                UpdateXmlParts(doc);            
            TargetId = "AltChunkId" + nextOwner.Document.AlternateChunkCount.ToString();
        }
        /// <summary>
        /// Updates the XML parts.
        /// </summary>
        /// <param name="destination">The destination.</param>
        private void UpdateXmlParts(WordDocument destination)
        {
            string[] parts = ContentPath.Replace("word/","").Split('/');
            PartContainer srcContainer = Document.DocxPackage.FindPartContainer("word/");
            PartContainer destContainer = destination.DocxPackage.FindPartContainer("word/");
            string newPart = UpdateXmlPartContainer(srcContainer, destContainer, parts, 0);
            if (newPart != string.Empty)
            {
                ContentPath = ContentPath.Replace(parts[parts.Length - 1], newPart);                
            }
        }
        /// <summary>
        /// Updates the XML part container.
        /// </summary>
        /// <param name="srcContainer">The SRC container.</param>
        /// <param name="destContainer">The dest container.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private string UpdateXmlPartContainer(PartContainer srcContainer, PartContainer destContainer, string[] parts, int index)
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
                        newPart = UpdateXmlPartContainer(srcContainer, destContainer, parts, i + 1);
                        break;
                    }
                    else
                    {
                        PartContainer container = srcContainer.XmlPartContainers[containerName].Clone();
                        //Updates xml part container from source to destination document.
                        destContainer.XmlPartContainers.Add(containerName, container);
                        break;
                    }
                }
                else
                {
                    newPart = UpdateXmlPart(srcContainer, destContainer, parts[i]);
                }
            }
            return newPart;
        }
        /// <summary>
        /// Updates the XML part.
        /// </summary>
        /// <param name="srcContainer">The SRC container.</param>
        /// <param name="destContainer">The dest container.</param>
        /// <param name="xmlPart">The XML part.</param>
        /// <returns></returns>
        private string UpdateXmlPart(PartContainer srcContainer, PartContainer destContainer, string xmlPart)
        {
            string newPart = string.Empty;
            if (destContainer.XmlParts.ContainsKey(xmlPart))
            {
                string extension = Path.GetExtension(xmlPart);
                string partName = xmlPart.Replace(extension, "");
                string prefix = partName.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                int id = 0;
                int.TryParse(partName.Replace(prefix, ""), out id);
                newPart = prefix + id.ToString() + extension;
                while (destContainer.XmlParts.ContainsKey(newPart))
                {
                    id++;
                    newPart = prefix + id.ToString() + extension;
                }
                Part part = srcContainer.XmlParts[xmlPart].Clone();
                part.Name = newPart;
                //Updates xml part from source to destination document.
                destContainer.XmlParts.Add(newPart, part);
            }
            else
                //Updates xml part from source to destination document.
                destContainer.XmlParts.Add(xmlPart, srcContainer.XmlParts[xmlPart].Clone());
            return newPart;
        }
        #endregion
#endif

    }
}
