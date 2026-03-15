#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;


/// <summary>
/// The Syncfusion.Pdf.Security namespace contains classes for creating protected PDF document.
/// </summary>
namespace Syncfusion.Pdf.Security
{
    /// <summary>
    /// Represents signature dictionary.
    /// </summary>
    internal class PdfSignatureDictionary : IPdfWrapper
    {
#region Constants

        /// <summary>
        /// Name of type
        /// </summary>
        private const string c_Type = "Sig";

        /// <summary>
        /// Name of the filter.
        /// </summary>
        private const string c_FilterType = "adbe.pkcs7.detached";

        /// <summary>
        /// Name of the document type.
        /// </summary>
        private const string c_DocMdp = "DocMDP";

        /// <summary>
        /// Name of the taransformation parameters.
        /// </summary>
        private const string c_TransParam = "TransformParams";
        #endregion

#region Fields
        /// <summary>
        /// Holds pdf document for siging.
        /// </summary>
        PdfDocumentBase m_doc;

        /// <summary>
        /// Holds pdf signature object.
        /// </summary>
        PdfSignature m_sig;

        /// <summary>
        /// Holds pdf certificate object.
        /// </summary>
        PdfCertificate m_cert;

        /// <summary>
        /// First range length;
        /// </summary>
        private int m_firstRangeLength;

        /// <summary>
        /// Second range index.
        /// </summary>
        private int m_secondRangeIndex;

        /// <summary>
        /// Start position byte range.
        /// </summary>
        private int m_startPositionByteRange;

        /// <summary>
        /// Position of the digest value for docMDP method.
        /// </summary>
        private int m_docDigestPosition;

        /// <summary>
        /// Position of the digest value for FieldMDP method.
        /// </summary>
        private int m_fieldsDigestPosition;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

#region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PdfSignatureDictionary"/> is archive.
        /// </summary>
        /// <value><c>true</c> if archive; otherwise, <c>false</c>.</value>
        public bool Archive
        {
            get
            {
                return m_dictionary.Archive;
            }
            set
            {
                m_dictionary.Archive = value;
            }
        }
        #endregion

#region Constructor
        /// <summary>
        /// Creates new pdf signature dictionary object.
        /// </summary>
        /// <param name="doc">The pdf document for signing.</param>
        /// <param name="sig">The pdf signature.</param>
        /// <param name="cert">The pdf certificate.</param>
        internal PdfSignatureDictionary(PdfDocumentBase doc, PdfSignature sig, PdfCertificate cert)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");

            if (sig == null)
                throw new ArgumentNullException("sig");

            if (cert == null)
                throw new ArgumentNullException("cert");

            m_doc = doc;
            m_sig = sig;
            m_cert = cert;

            doc.DocumentSaved += new Syncfusion.Pdf.PdfDocument.DocumentSavedEventHandler(DocumentSaved);
            m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);
        }
        #endregion

#region Implementation
        /// <summary>
        /// Adds required items.
        /// </summary>
        private void AddRequiredItems()
        {
            if (m_sig.Certificated && AllowMDP())
            {
                AddReference();
            }

            AddType();
            AddName();
            AddDate();
            AddFilter();
            AddSubFilter();
        }

        /// <summary>
        /// Adds optional items.
        /// </summary>
        private void AddOptionalItems()
        {
            AddReason();
            AddLocation();
            AddContactInfo();
        }

        /// <summary>
        /// Adds the location.
        /// </summary>
        private void AddLocation()
        {
            if (m_sig.LocationInfo != null)
            {
                m_dictionary.SetProperty(DictionaryProperties.Location, new PdfString(m_sig.LocationInfo));
            }
        }

        /// <summary>
        /// Adds the contact info.
        /// </summary>
        private void AddContactInfo()
        {
            if (m_sig.ContactInfo != null)
            {
                m_dictionary.SetProperty(DictionaryProperties.ContactInfo, new PdfString(m_sig.ContactInfo));
            }
        }

        /// <summary>
        /// Adds required items to annotation dictionary.
        /// </summary>
        private void AddType()
        {
            m_dictionary.SetName(DictionaryProperties.Type, c_Type);
        }

        /// <summary>
        /// Adds required items to annotation dictionary.
        /// </summary>
        private void AddName()
        {
            m_dictionary.SetProperty(DictionaryProperties.Name, new PdfString(m_cert.IssuerName));
        }

        /// <summary>
        /// Adds required items to annotation dictionary.
        /// </summary>
        private void AddDate()
        {
            string date = String.Format("D:{0:yyyyMMddHHmmss}", DateTime.Now);
            m_dictionary.SetProperty(DictionaryProperties.M, new PdfString(date));
        }

        /// <summary>
        /// Adds optional items to annotation dictionary.
        /// </summary>
        private void AddReason()
        {
            if (m_sig.Reason != null)
            {
                m_dictionary.SetProperty(DictionaryProperties.Reason, new PdfString(m_sig.Reason));
            }
        }

        /// <summary>
        /// Adds required items to annotation dictionary.
        /// </summary>
        private void AddFilter()
        {
            m_dictionary.SetName(DictionaryProperties.Filter, "Adobe.PPKMS");
        }

        /// <summary>
        /// Adds required items to annotation dictionary.
        /// </summary>
        private void AddSubFilter()
        {
            m_dictionary.SetName(DictionaryProperties.SubFilter, c_FilterType);
        }

        /// <summary>
        /// Adds required items to annotation dictionary.
        /// </summary>
        private void AddContents(IPdfWriter writer)
        {
            writer.Write(Operators.Slash + DictionaryProperties.Contents + Operators.WhiteSpace);
            m_firstRangeLength = (int)writer.Position;

            uint writerPosition = (uint)writer.Position + 10000;
            //writer.Write( Operators.LessThan );

            // Length of the string object. It consists of data and angle brakects.
            // Length of the string object. It consists of data and angle brakects.
            uint length = (m_sig.TimeStampServer == null) ? m_cert.GetSignatureLength() * 2 + 2
                : writerPosition * 2 + 2;

            byte[] contents = new byte[length];
            writer.Write(contents);

            //writer.Write( Operators.GreaterThan );
            m_secondRangeIndex = (int)writer.Position;
            writer.Write(Operators.NewLine);
        }

        /// <summary>
        /// Adds required items to annotation dictionary.
        /// </summary>
        private void AddRange(IPdfWriter writer)
        {
            writer.Write(Operators.Slash + DictionaryProperties.ByteRange + Operators.WhiteSpace + PdfArray.StartMark);

            m_startPositionByteRange = (int)writer.Position;

            byte[] range = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                writer.Write(Operators.WhiteSpace);
            }
            writer.Write(PdfArray.EndMark + Operators.NewLine);
        }

        /// <summary>
        /// Allow single instance of MDP signature.
        /// </summary>
        /// <returns><c>true</c> if allow; otherwise, <c>false</c>.</returns>
        private bool AllowMDP()
        {
            PdfDictionary perms = PdfCrossTable.Dereference(m_doc.Catalog[DictionaryProperties.Perms]) as PdfDictionary;

            IPdfPrimitive DocMDP = PdfCrossTable.Dereference(perms[DictionaryProperties.DocMDP]);
            IPdfPrimitive DicSig = m_dictionary;

            return DicSig.Equals(DocMDP);
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Adds the certification dictionary.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void AddDigest(IPdfWriter writer)
        {
            if (AllowMDP())
            {
                PdfDictionary cat = writer.Document.Catalog as PdfDictionary;
                writer.Write(new PdfName(DictionaryProperties.Reference));
                writer.Write(PdfArray.StartMark);
                writer.Write(Operators.LessThan + Operators.LessThan);

                writer.Write(Operators.Slash + c_TransParam);
                PdfDictionary trans = new PdfDictionary();

                int p = (int)m_sig.DocumentPermissions;

                trans[DictionaryProperties.V] = new PdfName("1.2");
                trans[DictionaryProperties.P] = new PdfNumber(p);
                trans[DictionaryProperties.Type] = new PdfName(c_TransParam);
                writer.Write(trans);

                writer.Write(new PdfName(DictionaryProperties.TransformMethod));
                writer.Write(new PdfName(c_DocMdp));
                writer.Write(new PdfName(DictionaryProperties.Type));
                writer.Write(new PdfName(DictionaryProperties.SigRef));

                writer.Write(new PdfName(DictionaryProperties.DigestValue));
                int position = (int)writer.Position;
                m_docDigestPosition = position;
                writer.Write(new PdfString(new byte[16]));
                PdfArray digestLocation = new PdfArray();

                digestLocation.Add(new PdfNumber(position));
                digestLocation.Add(new PdfNumber(34));

                writer.Write(new PdfName(DictionaryProperties.DigestLocation));
                writer.Write(digestLocation);
                writer.Write(new PdfName(DictionaryProperties.DigestMethod));
                writer.Write(new PdfName(DictionaryProperties.MD5));
                writer.Write(new PdfName(DictionaryProperties.Data));
                PdfReferenceHolder refh = new PdfReferenceHolder(cat);
                writer.Write(Operators.WhiteSpace);
                writer.Write(refh);
                writer.Write(Operators.GreaterThan + Operators.GreaterThan);
                writer.Write(Operators.LessThan + Operators.LessThan);

                writer.Write(new PdfName(c_TransParam));

                trans = new PdfDictionary();
                trans[DictionaryProperties.V] = new PdfName("1.2");
                PdfArray fields = new PdfArray();
                fields.Add(new PdfString(m_sig.Field.Name));
                trans[DictionaryProperties.Fields] = fields;
                trans[DictionaryProperties.Type] = new PdfName(c_TransParam);
                trans[DictionaryProperties.Action] = new PdfName(DictionaryProperties.Include);
                writer.Write(trans);

                writer.Write(new PdfName(DictionaryProperties.TransformMethod));
                writer.Write(new PdfName(DictionaryProperties.FieldMDP));
                writer.Write(new PdfName(DictionaryProperties.Type));
                writer.Write(new PdfName(DictionaryProperties.SigRef));

                writer.Write(new PdfName(DictionaryProperties.DigestValue));
                position = (int)writer.Position;
                m_fieldsDigestPosition = position;
                writer.Write(new PdfString(new byte[16]));
                digestLocation = new PdfArray();

                digestLocation.Add(new PdfNumber(position));
                digestLocation.Add(new PdfNumber(34));

                writer.Write(new PdfName(DictionaryProperties.DigestLocation));
                writer.Write(digestLocation);
                writer.Write(new PdfName(DictionaryProperties.DigestMethod));
                writer.Write(new PdfName(DictionaryProperties.MD5));
                writer.Write(new PdfName(DictionaryProperties.Data));
                writer.Write(Operators.WhiteSpace);
                writer.Write(new PdfReferenceHolder(cat));
                writer.Write(Operators.GreaterThan + Operators.GreaterThan);
                writer.Write(PdfArray.EndMark);
                writer.Write(Operators.WhiteSpace);
            }
        }

        /// <summary>
        /// Event handler of document saved.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void DocumentSaved(Object sender, DocumentSavedEventArgs e)
        {

            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            bool enabled = this.m_doc.Security.Enabled;
            this.m_doc.Security.Enabled = false;
            PdfWriter writer = e.Writer;
            byte[] buffer = new byte[this.m_firstRangeLength];
            int num = ((int)e.Writer.Length) - this.m_secondRangeIndex;
            byte[] buffer2 = new byte[num];
            string str = "0 ";
            string str2 = this.m_firstRangeLength.ToString() + " ";
            string str3 = this.m_secondRangeIndex.ToString() + " ";
            string str4 = num.ToString();
            int startPosition = this.SaveRangeItem(writer, str, this.m_startPositionByteRange);
            startPosition = this.SaveRangeItem(writer, str2, startPosition);
            startPosition = this.SaveRangeItem(writer, str3, startPosition);
            this.SaveRangeItem(e.Writer, str4, startPosition);
            if (this.m_sig.Certificated && this.AllowMDP())
            {
                PdfSignatureDigest digest = new PdfSignatureDigest();
                byte[] buffer3 = digest.HashDocument(e.Writer.Document);
                e.Writer.Position = this.m_docDigestPosition;
                e.Writer.Write(new PdfString(buffer3));
                buffer3 = new PdfSignatureDigest().HashSignatureFields(this.m_sig.Field.Page as PdfPage);
                e.Writer.Position = this.m_fieldsDigestPosition;
                e.Writer.Write(new PdfString(buffer3));
            }
            Stream stream = writer.GetStream();
            writer.Position = 0L;
            stream.Read(buffer, 0, buffer.Length);
            writer.Position = this.m_secondRangeIndex;
            stream.Read(buffer2, 0, buffer2.Length);
            byte[][] dataBlocks = new byte[][] { buffer, buffer2 };
            PdfString str5 = new PdfString(this.m_cert.GetSignatureValue(dataBlocks));
            e.Writer.Position = this.m_firstRangeLength;
            e.Writer.Write(str5.PdfEncode(writer.Document));
            byte[] array = new byte[buffer.Length + buffer2.Length];
            buffer.CopyTo(array, 0);
            buffer2.CopyTo(array, buffer.Length);
            if (this.m_sig.TimeStampServer != null)
            {
                byte[] hash = SHA1.Create().ComputeHash(array);
                byte[] asnEncodedTimestampRequest = new Syncfusion.Pdf.Security.TimeStampRequest(true).GetAsnEncodedTimestampRequest(hash);
                TimeStampResponse response = new TimeStampResponse(this.m_sig.TimeStampServer.GetTimeStampResponse(asnEncodedTimestampRequest));
                byte[] encoded = response.GetEncoded(response.Object);
                CmsSigner signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, this.m_sig.Certificate.X509Certificate);
                signer.IncludeOption = X509IncludeOption.EndCertOnly;
                ContentInfo contentInfo = new ContentInfo(array);
                SignedCms cms = new SignedCms(contentInfo, false);
                AsnEncodedData asnEncodedData = new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.14"), encoded);
                signer.UnsignedAttributes.Add(asnEncodedData);
                cms.ComputeSignature(signer);
                byte[] bytes = cms.Encode();
                e.Writer.Position = this.m_firstRangeLength;
                e.Writer.Write("<");
                string text = PdfString.BytesToHex(bytes);
                e.Writer.Write(text);
                int num3 = (this.m_secondRangeIndex - ((int)e.Writer.Position)) / 2;
                e.Writer.Write(PdfString.BytesToHex(new byte[num3]));
                e.Writer.Write(">");
            }
            this.m_doc.Security.Enabled = enabled;

        }

        /// <summary>
        /// Saves range item.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        /// <param name="str">item value.</param>
        /// <param name="startPosition">Position for save.</param>
        /// <returns>Point for save next item.</returns>
        private int SaveRangeItem(PdfWriter writer, string str, int startPosition)
        {
            byte[] date = Encoding.UTF8.GetBytes(str);
            writer.Position = startPosition;
            Stream writerStream = writer.GetStream();
            writerStream.Write(date, 0, date.Length);

            int rezult = startPosition + str.Length;

            return rezult;
        }

        /// <summary>
        /// Adds the certefication reference.
        /// </summary>
        private void AddReference()
        {
            PdfDictionary trans = new PdfDictionary();
            PdfDictionary reference = new PdfDictionary();
            PdfArray array = new PdfArray();

            int p = (int)m_sig.DocumentPermissions;

            trans[DictionaryProperties.V] = new PdfName("1.2");
            trans[DictionaryProperties.P] = new PdfNumber(p);
            trans[DictionaryProperties.Type] = new PdfName(c_TransParam);

            reference[DictionaryProperties.TransformMethod] = new PdfName(c_DocMdp);
            reference[DictionaryProperties.Type] = new PdfName("SigRef");
            reference[c_TransParam] = trans;

            array.Add(reference);

            m_dictionary.SetProperty("Reference", array);
        }

        /// <summary>
        /// Creates timestamp request in ASN.1 format.
        /// </summary>
        /// <param name="sha1Hash">SHA1 hash of data which need to be timestamped.</param>
        /// <param name="input">Stream where request will be written.</param>
        /// <returns>Request length</returns>
        private int CreateAsn1TspRequest(byte[] sha1Hash, Stream input)
        {

            byte[] head = { 0x30, 0x27, 0x02, 0x01, 0x01, 0x30, 0x1F, 0x30, 0x07, 0x06, 0x05, 
											0x2B, 0x0E, 0x03, 0x02, 0x1A, 0x04, 0x14 };

            byte[] foot = { 0x01, 0x01, 0xFF };

            input.Write(head, 0, head.Length);
            input.Write(sha1Hash, 0, sha1Hash.Length);
            input.Write(foot, 0, foot.Length);

            int len = head.Length + sha1Hash.Length + foot.Length;

            return len;
        }
        #endregion

#region Overrides
        /// <summary>
        /// Handles the BeginSave event of the pdf signature dictionary object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The events arguments.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs args)
        {
            bool state = m_doc.Security.Enabled;
            m_dictionary.Encrypt = state;

            AddRequiredItems();
            AddOptionalItems();

            m_doc.Security.Enabled = false;

            AddContents(args.Writer);
            AddRange(args.Writer);

            if (m_sig.Certificated)
            {
                AddDigest(args.Writer);
            }

            m_doc.Security.Enabled = state;
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
    }
}
#endif