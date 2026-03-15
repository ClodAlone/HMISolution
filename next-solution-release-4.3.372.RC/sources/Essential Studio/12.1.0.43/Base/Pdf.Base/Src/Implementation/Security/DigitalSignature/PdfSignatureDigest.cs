#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Collections;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Native;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Syncfusion.Pdf.IO;

/// <summary>
/// The Syncfusion.Pdf.Security namespace contains classes for creating protected PDF document.
/// </summary>
namespace Syncfusion.Pdf.Security
{
    /// <summary>
    /// Computing hash for Pdf digital signature.
    /// </summary>
    internal class PdfSignatureDigest
    {
#region Fields
        /// <summary>
        /// Array of objects for hashing.
        /// </summary>
        private List<object> m_objList;

        /// <summary>
        /// Contains ID's of Pdf objects.
        /// </summary>
        private PdfPrimitiveId m_id;
        #endregion

#region Constructor
        /// <summary>
        /// Creating hashing object.
        /// </summary>
        internal PdfSignatureDigest()
        {
            m_objList = new List<object>();
            m_id = new PdfPrimitiveId();
        }
        #endregion

#region Implementation
        /// <summary>
        /// Hashes the document.
        /// </summary>
        /// <param name="doc">The PdfDocument.</param>
        /// <returns>The hash byte array.</returns>
        public byte[] HashDocument(PdfDocumentBase doc)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");

            Md5_Ctx ctx = new Md5_Ctx();
            Md5_Ctx context1 = new Md5_Ctx();
            CryptoApi.MD5Init(ref ctx);
            CryptoApi.MD5Init(ref context1);
            byte[] digest;
            int len;

            PdfCatalog catalog = doc.Catalog;
            HashDictionaryName(catalog, "AA", ref context1, m_objList, false, false);
            HashDictionaryName(catalog, "Legal", ref context1, m_objList, false, false);
            HashDictionaryName(catalog, "Perms", ref context1, m_objList, false, false);
            CryptoApi.MD5Final(ref context1);
            digest = context1.digest;
            len = digest.Length;
            CryptoApi.MD5Update(ref ctx, digest, len);
            context1 = new Md5_Ctx();
            CryptoApi.MD5Init(ref context1);
            PdfDictionary info = doc.DocumentInformation.Dictionary;
            HashDictionaryName(info, "Title", ref context1, m_objList, false, false);
            HashDictionaryName(info, "Author", ref context1, m_objList, false, false);
            HashDictionaryName(info, "Keywords", ref context1, m_objList, false, false);
            HashDictionaryName(info, "Subject", ref context1, m_objList, false, false);
            CryptoApi.MD5Final(ref context1);
            digest = context1.digest;
            len = digest.Length;
            CryptoApi.MD5Update(ref ctx, digest, len);

            context1 = new Md5_Ctx();
            CryptoApi.MD5Init(ref context1);
            CryptoApi.MD5Final(ref context1);
            digest = context1.digest;
            len = digest.Length;

            CryptoApi.MD5Update(ref ctx, digest, len);

            PdfDocument document = (doc as PdfDocument);

            if (document != null)
            {
                HashPages((document.Pages as PdfDocumentPageCollection), ref ctx, m_objList);
                CryptoApi.MD5Update(ref ctx, digest, len);

                HashEmbeddedFiles((document.Pages as PdfDocumentPageCollection), ref ctx, m_objList);
                CryptoApi.MD5Update(ref ctx, digest, len);
            }

            CryptoApi.MD5Final(ref ctx);

            return ctx.digest;
        }

        public byte[] HashDocument(PdfWriter writer, int firstRangeEnd, int secondRangeStart)
        {
            byte[] digest;

            writer.Position = firstRangeEnd;

            writer.Write(Operators.LessThan);
            writer.Position = secondRangeStart;
            writer.Write(Operators.GreaterThan);


            byte[] firstRange = new byte[firstRangeEnd + 1];
            writer.Position = 0;
            writer.GetStream().Read(firstRange, 0, firstRange.Length);

            byte[] secondRange = new byte[writer.Length - secondRangeStart];
            writer.Position = secondRangeStart;
            writer.GetStream().Read(secondRange, 0, secondRange.Length);

            byte[] allRange = new byte[firstRange.Length + secondRange.Length];
            Array.Copy(firstRange, allRange, firstRange.Length);
            Array.Copy(secondRange, 0, allRange, firstRange.Length, secondRange.Length);

            File.WriteAllBytes("tt.pdf", allRange);

            digest = SHA1.Create().ComputeHash(allRange, 0, allRange.Length);

            return digest;
        }

        /// <summary>
        /// Hashes the signature field.
        /// </summary>
        /// <param name="ipage">The page where signature is located.</param>
        /// <returns></returns>
        public byte[] HashSignatureFields(PdfPage ipage)
        {
            byte[] digest = new byte[16];

            if (ipage != null)
            {
                PdfPage page = ipage as PdfPage;
                PdfArray array = page.Annotations.Annotations;

                if (array != null)
                {
                    Md5_Ctx ctx = new Md5_Ctx();
                    CryptoApi.MD5Init(ref ctx);
                    int count = array.Count;

                    Md5_Ctx context = new Md5_Ctx();
                    CryptoApi.MD5Init(ref context);

                    for (int i = 0; i < count; i++)
                    {
                        object sig = array[i];
                        if (sig is PdfReferenceHolder)
                        {
                            sig = (sig as PdfReferenceHolder).Object;
                        }
                        if (sig is PdfSignature)
                        {
                            PdfSignature signature = sig as PdfSignature;
                            HashField(signature.Field, ref context, m_objList);
                        }
                    }
                    CryptoApi.MD5Final(ref context);

                    byte[] fieldhash = context.digest;
                    int len = fieldhash.Length;

                    CryptoApi.MD5Update(ref ctx, fieldhash, len);
                    CryptoApi.MD5Final(ref ctx);

                    digest = ctx.digest;
                }
            }

            return digest;
        }


        /// <summary>
        /// Hashes the dictionary item.
        /// </summary>
        /// <param name="dic">The PdfDictionary.</param>
        /// <param name="item">The dictionary item.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list of the Pdf objects.</param>
        private int HashDictionaryItem(PdfDictionary dic, string item, ref Md5_Ctx ctx, List<object> list)
        {
            object obj = dic[item];

            if (obj != null)
            {
                if (obj is PdfReferenceHolder)
                {
                    obj = (obj as PdfReferenceHolder).Object;
                }

                CryptoApi.MD5Update(ref ctx, m_id.Name, 1);
                int len = item.Length;
                CryptoApi.MD5Update(ref ctx, LittleToBigEndian(BitConverter.GetBytes(len)), 4);
                byte[] itembytes = PdfString.StringToByte(item);
                CryptoApi.MD5Update(ref ctx, itembytes, len);
                HashObject(obj, ref ctx, list);
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Hashes the name of the dictionary.
        /// </summary>
        /// <param name="dic">The PdfDictionary.</param>
        /// <param name="name">The name of the dictionary.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list of the Pdf objects.</param>
        /// <param name="isInheritable">Is dictionary inheritable.</param>
        /// <param name="isNull">If dictionary is null then hash
        /// either null.</param>
        /// <returns>
        /// Returns state of the hashing.
        /// </returns>
        private bool HashDictionaryName(PdfDictionary dic, string name, ref Md5_Ctx ctx, List<object> list, bool isInheritable, bool isNull)
        {
            bool flag = false;
            object obj = dic[name];

            if (dic != null)
            {
                do
                {
                    if (obj != null)
                    {
                        if (obj is PdfReferenceHolder)
                        {
                            obj = (obj as PdfReferenceHolder).Object;
                        }

                        flag = true;
                        HashObject(obj, ref ctx, list);
                        break;
                    }

                    if (!isInheritable)
                    {
                        break;
                    }

                    obj = dic["Parent"];

                    if (obj is PdfReferenceHolder)
                    {
                        obj = (obj as PdfReferenceHolder).Object;
                    }

                    if (obj is PdfDictionary)
                    {
                        dic = obj as PdfDictionary;
                    }
                    else
                    {
                        dic = null;
                    }
                }
                while (dic != null);
            }
            if (!flag && isNull)
            {
                CryptoApi.MD5Update(ref ctx, m_id.Null, 1);
            }
            return flag;
        }

        /// <summary>
        /// Hashes the Pdf object.
        /// </summary>
        /// <param name="obj">The object to hash.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list of the Pdf objects.</param>
        private void HashObject(object obj, ref Md5_Ctx ctx, List<object> list)
        {
            if (obj is PdfReferenceHolder)
            {
                obj = (obj as PdfReferenceHolder).Object;
            }
            if (obj is PdfNull)
            {
                CryptoApi.MD5Update(ref ctx, m_id.Null, 1);
            }
            else if (obj is PdfPage)
            {
                HashPage(obj as PdfPage, ref ctx, list);
            }
            else if (obj is PdfNumber)
            {
                PdfNumber number = obj as PdfNumber;

                if (number.IsInteger)
                {
                    CryptoApi.MD5Update(ref ctx, m_id.Integer, 1);
                    int value = (int)number.IntValue;
                    byte[] b = BitConverter.GetBytes(value);
                    b = LittleToBigEndian(b);
                    CryptoApi.MD5Update(ref ctx, b, 4);
                }
                else
                {
                    CryptoApi.MD5Update(ref ctx, m_id.Real, 1);
                    int value = (int)number.FloatValue;
                    byte[] b = BitConverter.GetBytes(value);
                    b = LittleToBigEndian(b);
                    CryptoApi.MD5Update(ref ctx, b, 4);
                }
            }
            else if (obj is PdfBoolean)
            {
                CryptoApi.MD5Update(ref ctx, m_id.Boolean, 1);
                PdfBoolean value = obj as PdfBoolean;

                if (value.Value)
                {
                    CryptoApi.MD5Update(ref ctx, m_id.True, 1);
                }
                else
                {
                    CryptoApi.MD5Update(ref ctx, m_id.False, 1);
                }
            }
            else if (obj is PdfName)
            {
                CryptoApi.MD5Update(ref ctx, m_id.Name, 1);

                string name = (obj as PdfName).Value;
                int value = name.Length;
                byte[] b = BitConverter.GetBytes(value);
                b = LittleToBigEndian(b);
                CryptoApi.MD5Update(ref ctx, b, 4);
                b = PdfString.StringToByte(name);
                CryptoApi.MD5Update(ref ctx, b, b.Length);
            }
            else if (obj is PdfString)
            {
                CryptoApi.MD5Update(ref ctx, m_id.String, 1);
                string str = (obj as PdfString).Value;
                int value = str.Length;
                byte[] b = BitConverter.GetBytes(value);
                b = LittleToBigEndian(b);
                CryptoApi.MD5Update(ref ctx, b, 4);
                b = PdfString.StringToByte(str);
                CryptoApi.MD5Update(ref ctx, b, b.Length);
            }
            else if (obj is PdfArray)
            {
                Md5_Ctx context1 = new Md5_Ctx();
                CryptoApi.MD5Update(ref context1, m_id.Array, 1);

                byte[] digest;
                int len;

                if (!list.Contains(obj))
                {
                    Md5_Ctx context2 = new Md5_Ctx();
                    CryptoApi.MD5Init(ref context2);
                    PdfArray array = obj as PdfArray;
                    len = array.Count;
                    byte[] count = BitConverter.GetBytes(len);
                    count = LittleToBigEndian(count);
                    CryptoApi.MD5Update(ref context1, count, 4);

                    list.Add(obj);

                    for (int i = 0; i < len; i++)
                    {
                        HashObject(array[i], ref context2, list);
                    }

                    CryptoApi.MD5Final(ref context2);

                    digest = context2.digest;
                    len = digest.Length;

                    CryptoApi.MD5Update(ref context1, digest, len);

                    list.Remove(obj);
                }
                else
                {
                    CryptoApi.MD5Update(ref context1, m_id.Visited, 4);
                }

                CryptoApi.MD5Final(ref context1);

                digest = context1.digest;
                len = digest.Length;

                CryptoApi.MD5Update(ref ctx, digest, len);
            }
            else if (obj is PdfDictionary)
            {
                Md5_Ctx context1 = new Md5_Ctx();

                CryptoApi.MD5Init(ref context1);

                CryptoApi.MD5Update(ref context1, m_id.Dictionary, 1);
                byte[] digest;
                int len;

                if (!list.Contains(obj))
                {
                    Md5_Ctx context2 = new Md5_Ctx();
                    CryptoApi.MD5Init(ref context2);
                    list.Add(obj);
                    PdfDictionary dic = obj as PdfDictionary;
                    int itemsCount = dic.Count;
                    byte[] b = BitConverter.GetBytes(itemsCount);
                    b = LittleToBigEndian(b);
                    CryptoApi.MD5Update(ref context1, b, 4);

#if Generics
          List<PdfName> PdfNames = new List<PdfName>();
#else
                    ArrayList PdfNames = new ArrayList();
#endif
                    PdfNames.AddRange(dic.Keys);
                    //PdfNames.Sort( 0, itemsCount, null );

                    for (int i = 0; i < itemsCount; i++)
                    {
                        CryptoApi.MD5Update(ref context2, m_id.Name, 1);
                        PdfName name = PdfNames[i] as PdfName;
                        string value = name.Value;
                        b = BitConverter.GetBytes(value.Length);
                        b = LittleToBigEndian(b);
                        CryptoApi.MD5Update(ref context2, b, 4);
                        b = PdfString.StringToByte(value);
                        CryptoApi.MD5Update(ref context2, b, b.Length);
                        HashObject(dic[name], ref context2, list);
                    }

                    CryptoApi.MD5Final(ref context2);
                    digest = context2.digest;
                    len = digest.Length;
                    CryptoApi.MD5Update(ref context1, digest, len);
                    list.Remove(obj);
                }
                else
                {
                    CryptoApi.MD5Update(ref context1, m_id.Visited, 4);
                }

                CryptoApi.MD5Final(ref context1);
                digest = context1.digest;
                len = digest.Length;
                CryptoApi.MD5Update(ref ctx, digest, len);

            }
            else if (obj is PdfStream)
            {
                PdfDictionary dic = obj as PdfDictionary;
                PdfStream stream = obj as PdfStream;

                Md5_Ctx context1 = new Md5_Ctx();
                byte[] digest;
                int len;

                CryptoApi.MD5Update(ref context1, m_id.Stream, 1);

                if (!list.Contains(obj))
                {
                    Md5_Ctx context2 = new Md5_Ctx();
                    CryptoApi.MD5Init(ref context2);
                    list.Add(obj);

                    int itemscount = HashDictionaryItem(dic, "F", ref context2, list);
                    itemscount += HashDictionaryItem(dic, "Filter", ref context2, list);
                    itemscount += HashDictionaryItem(dic, "Length", ref context2, list);
                    itemscount += HashDictionaryItem(dic, "FFilter", ref context2, list);

                    CryptoApi.MD5Update(ref context2, LittleToBigEndian(BitConverter.GetBytes(itemscount)), 4);
                    CryptoApi.MD5Final(ref context2);
                    digest = context2.digest;
                    len = digest.Length;
                    CryptoApi.MD5Update(ref context1, digest, len);
                    list.Remove(obj);

                    byte[] data = stream.Data;
                    byte[] streamLen = BitConverter.GetBytes(stream.Data.Length);
                    streamLen = LittleToBigEndian(streamLen);

                    CryptoApi.MD5Update(ref context1, streamLen, streamLen.Length);
                    CryptoApi.MD5Update(ref context1, data, data.Length);
                    CryptoApi.MD5Final(ref context1);

                    digest = context1.digest;
                    len = digest.Length;

                    CryptoApi.MD5Update(ref ctx, digest, len);
                    HashDictionaryName(dic, "Resources", ref ctx, list, false, false);
                }
                else
                {
                    CryptoApi.MD5Update(ref ctx, m_id.Visited, 4);
                }
            }
        }

        /// <summary>
        /// Hashes the page.
        /// </summary>
        /// <param name="page">The current page for hashing.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list of the Pdf objects.</param>
        private void HashPage(PdfPage page, ref Md5_Ctx ctx, List<object> list)
        {
            list.Add(page);
            Md5_Ctx context1 = new Md5_Ctx();

            CryptoApi.MD5Init(ref context1);

            HashDictionaryName(page.Dictionary, "MediaBox", ref context1, list, true, false);

            if (!HashDictionaryName(page.Dictionary, "CropBox", ref context1, list, true, false))
            {
                HashDictionaryName(page.Dictionary, "MediaBox", ref context1, list, true, false);
            }

            HashDictionaryName(page.Dictionary, "Resources", ref context1, list, true, true);
            HashDictionaryName(page.Dictionary, "Contents", ref context1, list, false, true);
            HashDictionaryName(page.Dictionary, "Rotate", ref context1, list, true, true);
            HashDictionaryName(page.Dictionary, "AA", ref context1, list, false, false);

            CryptoApi.MD5Final(ref context1);

            byte[] digest = context1.digest;
            int len = digest.Length;

            CryptoApi.MD5Update(ref ctx, digest, len);

            list.Remove(page);
        }

        /// <summary>
        /// Hashes the pages.
        /// </summary>
        /// <param name="pages">The collection of Pdf pages.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list of the Pdf objects.</param>
        private void HashPages(PdfDocumentPageCollection pages, ref Md5_Ctx ctx, List<object> list)
        {
            for (int i = 0, count = pages.Count; i < count; i++)
            {
                HashPage((pages[i] as PdfPage), ref ctx, list);
                HashAnnots((pages[i] as PdfPage), ref ctx, list);
            }
        }

        /// <summary>
        /// Hashes the field.
        /// </summary>
        /// <param name="field">The PdfField.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list.</param>
        private void HashField(PdfField field, ref Md5_Ctx ctx, List<object> list)
        {
            if (field != null)
            {
                Md5_Ctx context = new Md5_Ctx();
                CryptoApi.MD5Init(ref context);

                PdfDictionary fieldDictionary = field.Dictionary;
                HashObject(fieldDictionary["T"], ref context, list);
                HashObject(fieldDictionary["FT"], ref context, list);

                object obj = fieldDictionary["DV"];

                if (obj == null)
                {
                    obj = new PdfNull();
                }

                HashObject(obj, ref context, list);

                if (fieldDictionary["Lock"] == null)
                {
                    obj = fieldDictionary["V"];
                    if (obj == null)
                    {
                        obj = fieldDictionary["AS"];

                        if (obj == null)
                        {
                            obj = new PdfNull();
                        }
                    }

                    HashObject(obj, ref context, list);
                }

                obj = fieldDictionary["A"];

                if (obj is PdfReferenceHolder)
                {
                    obj = (obj as PdfReferenceHolder).Object;
                }

                if (obj is PdfDictionary)
                {
                    HashAction((obj as PdfDictionary), ref context, list);
                }

                if (!(fieldDictionary["Ff"] is PdfNumber))
                {
                    obj = new PdfNumber(0);
                }
                else
                {
                    int value = (int)(obj as PdfNumber).FloatValue;
                    value &= -2;
                    (obj as PdfNumber).FloatValue = value;

                }

                HashObject(obj, ref context, list);

                if (!(fieldDictionary["F"] is PdfNumber))
                {
                    obj = new PdfNumber(0);
                }
                else
                {
                    int value = (int)(obj as PdfNumber).FloatValue;
                    value &= 0x7F;
                    (obj as PdfNumber).FloatValue = value;
                }

                HashObject(obj, ref context, list);

                obj = fieldDictionary["Lock"];
                if (obj is PdfReferenceHolder)
                {
                    obj = (obj as PdfReferenceHolder).Object;
                }

                if (obj is PdfDictionary)
                {
                    HashObject(obj, ref context, list);
                }

                CryptoApi.MD5Final(ref context);

                byte[] digest = context.digest;
                int len = digest.Length;

                CryptoApi.MD5Update(ref ctx, digest, len);
            }
        }

        /// <summary>
        /// Hashes the action.
        /// </summary>
        /// <param name="action">The PdfAction.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list of the Pdf objects.</param>
        private void HashAction(PdfDictionary action, ref Md5_Ctx ctx, List<object> list)
        {
            Md5_Ctx context = new Md5_Ctx();
            CryptoApi.MD5Init(ref context);

            HashDictionaryName(action, "S", ref context, list, false, false);
            HashDictionaryName(action, "D", ref context, list, false, false);
            HashDictionaryName(action, "F", ref context, list, false, false);
            HashDictionaryName(action, "NewWindow", ref context, list, false, false);
            HashDictionaryName(action, "O", ref context, list, false, false);
            HashDictionaryName(action, "P", ref context, list, false, false);
            HashDictionaryName(action, "B", ref context, list, false, false);
            HashDictionaryName(action, "Base", ref context, list, false, false);
            HashDictionaryName(action, "Sound", ref context, list, false, false);
            HashDictionaryName(action, "Vol", ref context, list, false, false);
            HashDictionaryName(action, "Annot", ref context, list, false, false);
            HashDictionaryName(action, "T", ref context, list, false, false);
            HashDictionaryName(action, "H", ref context, list, false, false);
            HashDictionaryName(action, "N", ref context, list, false, false);
            HashDictionaryName(action, "JS", ref context, list, false, false);
            HashDictionaryName(action, "URI", ref context, list, false, false);

            CryptoApi.MD5Final(ref context);

            byte[] digest = context.digest;
            int len = digest.Length;

            CryptoApi.MD5Update(ref ctx, digest, len);
        }

        /// <summary>
        /// Hashes the annotations.
        /// </summary>
        /// <param name="page">The current page.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list of the Pdf objects.</param>
        private void HashAnnots(PdfPage page, ref Md5_Ctx ctx, List<object> list)
        {
            PdfArray array = page.Annotations.Annotations;

            if (array != null)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    object sig = array[i];
                    if (sig is PdfReferenceHolder)
                    {
                        sig = (sig as PdfReferenceHolder).Object;
                    }

                    if (sig is PdfSignature)
                    {
                        PdfSignature signature = sig as PdfSignature;
                        HashField(signature.Field, ref ctx, list);
                    }
                }
            }
        }

        /// <summary>
        /// Hashes the annotation.
        /// </summary>
        /// <param name="annot">The dictionary.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list of the Pdf objects.</param>
        private void HashAnnotation(PdfDictionary annot, ref Md5_Ctx ctx, List<object> list)
        {
            list.Add(annot);
            Md5_Ctx context = new Md5_Ctx();
            CryptoApi.MD5Init(ref context);

            HashDictionaryName(annot, "T", ref context, list, false, false);

            if (!HashDictionaryName(annot, "F", ref context, list, false, false))
            {
                byte[] prefix = new byte[] { 1, 0, 0, 0, 0 };
                CryptoApi.MD5Update(ref context, prefix, 5);
            }

            object obj = annot["A"];

            if (obj is PdfReferenceHolder)
            {
                obj = (obj as PdfReferenceHolder).Object;
            }

            if (obj is PdfDictionary)
            {
                HashAction(obj as PdfDictionary, ref context, list);
            }

            obj = annot["AA"];

            if (obj is PdfReferenceHolder)
            {
                obj = (obj as PdfReferenceHolder).Object;
            }

            if (obj is PdfDictionary)
            {
                HashAction(obj as PdfDictionary, ref context, list);
            }

            HashDictionaryName(annot, "Dest", ref context, list, false, false);
            HashDictionaryName(annot, "QuadPoints", ref context, list, false, false);
            HashDictionaryName(annot, "Inklist", ref context, list, false, false);
            HashDictionaryName(annot, "Name", ref context, list, false, false);
            HashDictionaryName(annot, "FS", ref context, list, false, false);
            HashDictionaryName(annot, "Sound", ref context, list, false, false);
            HashDictionaryName(annot, "AP", ref context, list, false, false);

            CryptoApi.MD5Final(ref context);

            byte[] digest = context.digest;
            int len = digest.Length;

            CryptoApi.MD5Update(ref ctx, digest, len);

            list.Remove(annot);
        }

        /// <summary>
        /// Hashes the embedded files.
        /// </summary>
        /// <param name="pages">The dictionary.</param>
        /// <param name="ctx">The crypto context.</param>
        /// <param name="list">The list of the Pdf objects.</param>
        private void HashEmbeddedFiles(PdfDocumentPageCollection pages, ref Md5_Ctx ctx, List<object> list)
        {
            Md5_Ctx context = new Md5_Ctx();

            CryptoApi.MD5Init(ref context);

            int count = pages.Count;

            for (int i = 0; i < count; i++)
            {
                object obj = pages[i].Dictionary["Names"];

                if (obj != null)
                {
                    if (obj is PdfReferenceHolder)
                    {
                        obj = (obj as PdfReferenceHolder).Object;
                    }

                    if (obj is PdfDictionary)
                    {
                        obj = (obj as PdfDictionary)["EmbeddedFiles"];

                        if (obj != null)
                        {
                            list.Add(obj);
                            HashObject(obj, ref context, list);
                            list.Remove(obj);
                        }
                    }
                }
            }
            CryptoApi.MD5Final(ref context);

            byte[] digest = context.digest;
            int len = digest.Length;

            CryptoApi.MD5Update(ref ctx, digest, len);
        }

        /// <summary>
        /// Converts little endian characters to big endian.
        /// </summary>
        /// <param name="buffer">The input buffer.</param>
        /// <returns></returns>
        private byte[] LittleToBigEndian(byte[] buffer)
        {
            if (buffer != null)
            {
                int count = buffer.Length;
                byte[] output = new byte[count];

                for (int cnt = count - 1, i = cnt; i >= 0; i--)
                {
                    output[cnt - i] = buffer[i];
                }
                buffer = output;
            }

            return buffer;
        }
        #endregion
    }
}
#endif