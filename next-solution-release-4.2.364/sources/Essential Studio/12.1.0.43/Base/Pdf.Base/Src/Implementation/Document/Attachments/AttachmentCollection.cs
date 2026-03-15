#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Text;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents a collection of the attachment objects.
    /// </summary>
    public class PdfAttachmentCollection :
        PdfCollection,
        IPdfWrapper
    {
        #region Fields

        /// <summary>
        /// Internal variable to store internal array of attachments.
        /// </summary>
        private PdfArray m_array = new PdfArray();

        /// <summary>
        /// Internal variable to store dictionary wrapper.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        /// <summary>
        /// internal Variable to store filename and attachment.
        /// </summary>
        private Dictionary<string, PdfReferenceHolder> dic = new Dictionary<string, PdfReferenceHolder>();
        /// <summary>
        /// internal variable to store ordered filename.
        /// </summary>
        private List<string> orderList = null;
        /// <summary>
        /// internal variable to store file count value.
        /// </summary>
        private int count = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAttachmentCollection"/> class.
        /// </summary>
        public PdfAttachmentCollection()
            : base()
        {
            m_dictionary.SetProperty(DictionaryProperties.Names, m_array);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAttachmentCollection"/> class.
        /// </summary>
        /// <param name="attachmentDictionary">The attachment dictionary.</param>
        /// <param name="table">The table.</param>
        internal PdfAttachmentCollection(PdfDictionary attachmentDictionary, PdfCrossTable table)
        {
            m_dictionary = attachmentDictionary;

            PdfReferenceHolder embedDictionary = m_dictionary[DictionaryProperties.EmbeddedFiles] as PdfReferenceHolder;
            if (embedDictionary != null)
            {
                PdfDictionary NameDictionary = embedDictionary.Object as PdfDictionary;
                m_array = NameDictionary[DictionaryProperties.Names] as PdfArray;

                if (m_array.Count != 0)
                {
                    int k = 1;
                    for (int i = 0; i < (m_array.Count / 2); i++)
                    {
                        //List.Add(m_array[k] as PdfReferenceHolder);

                        if (m_array[k] is PdfReferenceHolder)
                        {
                            PdfReferenceHolder holder = m_array[k] as PdfReferenceHolder;
                            PdfDictionary attachmentDic = holder.Object as PdfDictionary;
                            PdfStream stream = new PdfStream();
                            if (attachmentDic.ContainsKey(DictionaryProperties.EF))
                            {
                                PdfDictionary tempDic = attachmentDic[DictionaryProperties.EF] as PdfDictionary;
                                PdfReferenceHolder holder1 = tempDic[DictionaryProperties.F] as PdfReferenceHolder;
                                if (holder1 != null)
                                    stream = holder1.Object as PdfStream;
                            }

                            PdfAttachment attachment;

                            if (stream != null)
                            {
                                stream.Decompress();
                                if (attachmentDic.ContainsKey("F"))
                                {
                                    attachment = new PdfAttachment((attachmentDic["F"] as PdfString).Value, stream.Data);
                                    if (attachmentDic.ContainsKey("Desc"))
                                        attachment.Description = (attachmentDic["Desc"] as PdfString).Value;
                                    if (attachmentDic.ContainsKey(DictionaryProperties.CI))
                                    {
                                        PdfDictionary dictionary = attachmentDic[DictionaryProperties.CI] as PdfDictionary;
                                        if (dictionary != null)
                                        {
                                            PdfPortfolioAttributes attributes = new PdfPortfolioAttributes(dictionary);
                                            attachment.PortfolioAttributes = attributes;
                                        }
                                    }
                                }
                                else
                                    attachment = new PdfAttachment((attachmentDic["Desc"] as PdfString).Value, stream.Data);
                            }
                            else
                            {
                                if (attachmentDic.ContainsKey("Desc"))
                                    attachment = new PdfAttachment((attachmentDic["Desc"] as PdfString).Value);
                                else
                                    attachment = new PdfAttachment((attachmentDic["F"] as PdfString).Value);
                            }
                            List.Add(attachment);
                        }  

                        k = k + 2;
                    }
                }
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets attachment by its index in the collection.
        /// </summary>
        /// <param name="index">Index of the attachment.</param>
        /// <returns>Attachment object by its index in the collection.</returns>
        public PdfAttachment this[int index]
        {
            get
            {
                //if (List[index] is PdfReferenceHolder)
                //{
                //    PdfReferenceHolder holder = List[index] as PdfReferenceHolder;
                //    PdfDictionary attachmentDic = holder.Object as PdfDictionary;
                //     PdfStream stream = new PdfStream();
                //    if (attachmentDic.ContainsKey(DictionaryProperties.EF))
                //    {
                //        PdfDictionary tempDic = attachmentDic[DictionaryProperties.EF] as PdfDictionary;
                //        PdfReferenceHolder holder1 = tempDic[DictionaryProperties.F] as PdfReferenceHolder;
                //        if(holder1 != null)
                //            stream = holder1.Object as PdfStream;
                //    }

                //     PdfAttachment attachment ;

                //    if(stream != null)
                //        attachment = new PdfAttachment((attachmentDic["Desc"] as PdfString).Value,stream.Data);
                //    else
                //     attachment = new PdfAttachment((attachmentDic["Desc"] as PdfString).Value);
                //    return attachment;
                //}                   
                return (PdfAttachment)List[index];
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified attachment.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <returns>Position of the inserted attachment.</returns>
        public int Add(PdfAttachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException("attachment");
            }

            int position = DoAdd(attachment);
            m_dictionary.Modify();

            return position;
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="attachment">The attachment.</param>
        public void Insert(int index, PdfAttachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException("attachment");
            }

            DoInsert(index, attachment);
        }

        /// <summary>
        /// Removes the specified attachment.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        public void Remove(PdfAttachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException("attachment");
            }

            DoRemove(attachment);
        }

        /// <summary>
        /// Removes attachment at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            DoRemoveAt(index);
        }

        /// <summary>
        /// Indexes the of attachment.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <returns></returns>
        public int IndexOf(PdfAttachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException("attachment");
            }

            return List.IndexOf(attachment);
        }

        /// <summary>
        /// Determines whether 
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <returns>
        /// if it contains the specified attachment, set to <c>true</c>.
        /// </returns>
        public bool Contains(PdfAttachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException("attachment");
            }

            return List.Contains(attachment);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            DoClear();
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Adds the attachment.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <returns>The index of the attachment.</returns>
        private int DoAdd(PdfAttachment attachment)
        {
            string fileName = attachment.FileName;
            string converted = "";

            if (PdfString.IsUnicode(fileName))
            {
#if SILVERLIGHT || NETFX_CORE || WP
                converted="Attachment "+count++;
#else

                System.Text.Encoding uniText = System.Text.Encoding.Unicode;
                Byte[] encodedBytes = uniText.GetBytes(fileName);
                Byte[] convertedBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, encodedBytes);
                System.Text.Encoding ascii = System.Text.Encoding.ASCII;
                converted = ascii.GetString(convertedBytes);
#endif

            }

            else
            {
                converted = fileName;
            }

            System.StringComparer ordCmp = System.StringComparer.Ordinal;

            if (dic.Count == 0 && m_array.Count > 0)
            {
                for (int i = 0; i < m_array.Count; i += 2)
                {
                    if (!dic.ContainsKey((m_array[i] as PdfString).Value))
                        dic.Add((m_array[i] as PdfString).Value, m_array[i + 1] as PdfReferenceHolder);
                    else
                    {
                        string value = (m_array[i] as PdfString).Value+"_copy";
                        dic.Add(value, m_array[i + 1] as PdfReferenceHolder);
                         
                    }
                }
            }

            if (!dic.ContainsKey(converted))
            {
                dic.Add(converted, new PdfReferenceHolder(attachment));
            }

            else
            {
                string value = converted + "_copy";
                dic.Add(value, new PdfReferenceHolder(attachment));

            }
            
           
            orderList = new List<string>(dic.Keys);
            orderList.Sort(ordCmp);
            m_array.Clear();

            foreach (string key in orderList)
            {

                m_array.Add(new PdfString(key));
                m_array.Add(dic[key]);

            }

            return List.Add(attachment);
        }


        /// <summary>
        /// Does the insert.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="attachment">The attachment.</param>
        private void DoInsert(int index, PdfAttachment attachment)
        {
            m_array.Insert(2 * index, new PdfString(attachment.FileName));
            m_array.Insert(2 * index + 1, new PdfReferenceHolder(attachment));

            List.Insert(index, attachment);
        }

        /// <summary>
        /// Removes the attachment.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        private void DoRemove(PdfAttachment attachment)
        {
            int index = List.IndexOf(attachment);
            m_array.RemoveAt(2 * index);
            m_array.RemoveAt(2 * index);

            List.Remove(attachment);
        }

        /// <summary>
        /// Removes the attachment.
        /// </summary>
        /// <param name="index">The index.</param>
        private void DoRemoveAt(int index)
        {
            m_array.RemoveAt(2 * index);
            m_array.RemoveAt(2 * index);

            List.RemoveAt(index);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        private void DoClear()
        {
            List.Clear();
            m_array.Clear();
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        Syncfusion.Pdf.Primitives.IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion
    }
}
