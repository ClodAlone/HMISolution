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

#region file using directives
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
#if !SILVERLIGHT && !WP
using System.Drawing.Imaging;
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
#else
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Syncfusion.DocIO.DLS.Entities;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS.XML
{
    /// <summary>
    /// Summary description for DLSXmlWriter.
    /// </summary>
    public class XDLSWriter
      : IXDLSAttributeWriter,
        IXDLSContentWriter
    {
        #region Class constants
        /// <summary>
        /// Symbol, used during color saving to XML.
        /// </summary>
        private const string DEF_SHARP = "#";
        /// <summary>
        /// format for color convertion to string.
        /// </summary>
        private const string DEF_HEX_FORMAT = "X2";
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private readonly XmlWriter m_writer;
        private string m_rootTagName = "DLS";
        private XDLSCustomRW m_customRW = new XDLSCustomRW();
        private Metafile m_srcMetafile = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        public XDLSWriter(XmlWriter writer)
        {
            m_writer = writer;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Serialize(IXDLSSerializable value)
        {
            value.XDLSHolder.BeforeSerialization();
            WriteElement(m_rootTagName, value, false);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        /// <param name="isWriteID"></param>
        private void WriteElement(string tagName, IXDLSSerializable value, bool isWriteID)
        {
            if (!value.XDLSHolder.SkipMe)
            {
                m_writer.WriteStartElement(tagName);

                if (isWriteID && value.XDLSHolder.EnableID)
                {
                    WriteValue("id", value.XDLSHolder.ID);
                }

                value.WriteXmlAttributes(this);
                value.WriteXmlContent(this);
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        private void WriteCollectionElement(string tagName,
                                             IXDLSSerializableCollection value)
        {
            if (value.Count > 0)
            {
                m_writer.WriteStartElement(tagName);
                //for( int i = 0; i < value.Count; i++ )
                foreach (IXDLSSerializable dlsSerItem in value)
                {
                    //IXDLSSerializable dlsSerItem = value[ i ] as IXDLSSerializable;
                    if (dlsSerItem != null)
                    {
                        WriteElement(value.TagItemName, dlsSerItem, true);
                    }

                }
                m_writer.WriteEndElement();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        protected virtual void WriteCustomElement(string tagName, object value)
        {
            if (!m_customRW.Write(m_writer, tagName, value))
            {
                WriteDefElement(tagName, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        private void WriteDefElement(string tagName, object value)
        {
            IXmlSerializable xmlSer = value as IXmlSerializable;

            if (xmlSer != null)
            {
                xmlSer.WriteXml(m_writer);
            }
            else
            {
#if false        
        m_writer.WriteStartElement( tagName );

        if
        (
        value
        !=
        null
        )
          m_writer
          .
          WriteAttributeString
          (
          "type"
          ,
          value
          .
          GetType
          (
          )
          .
          ToString
          (
          )
          )
          ;
        else
          m_writer
          .
          WriteAttributeString
          (
          "type"
          ,
          "null"
          )
          ;

        m_writer
        .
        WriteEndElement
        (
        )
        ;
#endif
            }
        }
        #endregion

        #region IDLSXmlAttributeWriter implement
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void WriteValue(string name, float value)
        {
            m_writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void WriteValue(string name, double value)
        {
            m_writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void WriteValue(string name, int value)
        {
            m_writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void WriteValue(string name, string value)
        {
            m_writer.WriteAttributeString(name, value);
        }
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void WriteValue(string name, Enum value)
        {
            m_writer.WriteAttributeString(name, value.ToString());
        }
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">if it is specifies value, set to <c>true</c>.</param>
        public void WriteValue(string name, bool value)
        {
            m_writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }
        /// <summary>
        /// Writes color as string to XML.
        /// </summary>
        /// <param name="name">Name of attribute.</param>
        /// <param name="value">Color structure.</param>
        public void WriteValue(string name, Color value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            StringBuilder builder = new StringBuilder();
            if (!value.IsEmpty)
            {
                builder.Append(DEF_SHARP);
                builder.Append(value.A.ToString(DEF_HEX_FORMAT));
                builder.Append(value.R.ToString(DEF_HEX_FORMAT));
                builder.Append(value.G.ToString(DEF_HEX_FORMAT));
                builder.Append(value.B.ToString(DEF_HEX_FORMAT));
            }
            m_writer.WriteAttributeString(name, builder.ToString());
        }
        /// <summary>
        /// Writes DateTime as string to XML.
        /// </summary>
        /// <param name="name">Name of attribute.</param>
        /// <param name="value">Color structure.</param>
        public void WriteValue(string name, DateTime value)
        {
#if !SyncfusionFramework2_0
      string utcTimeString = XmlConvert.ToString( value, "yyyy-MM-ddTHH:mm:ssZ" );
      m_writer.WriteAttributeString( name, utcTimeString );
#else
            m_writer.WriteAttributeString(name, XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc));
#endif
        }
        #endregion

        #region IDLSXmlContentWriter implement
        /// <summary>
        /// Writes the child string element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void WriteChildStringElement(string name, string value)
        {
            m_writer.WriteStartElement(name);
            m_writer.WriteString(value);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Writes binary value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void WriteChildBinaryElement(string name, byte[] value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            InnerWriter.WriteStartElement(name);
            InnerWriter.WriteBase64(value, 0, value.Length);
            InnerWriter.WriteEndElement();
        }
        /// <summary>
        /// Writes the child element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void WriteChildElement(string name, object value)
        {
            IXDLSSerializable DLSSer = value as IXDLSSerializable;

            if (DLSSer != null)
            {
                WriteElement(name, DLSSer, false);
            }
            else
            {
                IXDLSSerializableCollection DLSSerColl =
                  value as IXDLSSerializableCollection;

                if (DLSSerColl != null)
                {
                    WriteCollectionElement(name, DLSSerColl);
                }
                else
                {
                    if (value is String)
                    {
                        m_writer.WriteStartElement(name);
                        WriteValue(XDLSConstants.TypeTag, "String");
                        WriteValue("value", (string)value);
                        m_writer.WriteEndElement();
                    }
                    else if (value is Int32)
                    {
                        m_writer.WriteStartElement(name);
                        WriteValue(XDLSConstants.TypeTag, "Int32");
                        WriteValue("value", (int)value);
                        m_writer.WriteEndElement();
                    }
                    else if (value is Single)
                    {
                        m_writer.WriteStartElement(name);
                        WriteValue(XDLSConstants.TypeTag, "Single");
                        WriteValue("value", (float)value);
                        m_writer.WriteEndElement();
                    }
                    else if (value is Boolean)
                    {
                        m_writer.WriteStartElement(name);
                        WriteValue(XDLSConstants.TypeTag, "Boolean");
                        WriteValue("value", value.ToString());
                        m_writer.WriteEndElement();
                    }
                    else if (value is Enum)
                    {
                        m_writer.WriteStartElement(name);
                        WriteValue(XDLSConstants.TypeTag, value.GetType().ToString());
                        WriteValue("value", value.ToString());
                        m_writer.WriteEndElement();
                    }
                    else
                    {
                        WriteCustomElement(name, value);
                    }
                }
            }
        }
        /// <summary>
        /// Writes the child ref element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="refToElement">The ref to element.</param>
        public void WriteChildRefElement(string name, int refToElement)
        {
            m_writer.WriteStartElement(name);
            WriteValue("ref", refToElement);
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// Writes the image.
        /// </summary>
        /// <param name="image">The image.</param>
        internal void WriteImage(Image image)
        {
            if (image != null)
            {
                MemoryStream memStream = CreateStreamFromImage(image);
                byte[] buf = new byte[memStream.Length];
                memStream.Position = 0;
                memStream.Read(buf, 0, buf.Length);
                WriteChildBinaryElement(XDLSConstants.ImageTag, buf);
            }
        }
        /// <summary>
        /// Gets the inner writer.
        /// </summary>
        /// <value>The inner writer.</value>
        public XmlWriter InnerWriter
        {
            get
            {
                return m_writer;
            }
        }
        #endregion

        #region Class Metafile helper methods
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordType"></param>
        /// <param name="flags"></param>
        /// <param name="dataSize"></param>
        /// <param name="data"></param>
        /// <param name="callbackData"></param>
        /// <returns></returns>
        private bool PlayInMeta(EmfPlusRecordType recordType, int flags,
          int dataSize, IntPtr data, PlayRecordCallback callbackData)
        {
            byte[] recordData = new byte[dataSize];

            if (data != IntPtr.Zero)
            {
                Marshal.Copy(data, recordData, 0, dataSize);
            }
            m_srcMetafile.PlayRecord(recordType, flags, dataSize, recordData);

            return true;
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private MemoryStream CreateStreamFromImage(Image image)
        {
#if !SILVERLIGHT && !WP
            MemoryStream memStream = new MemoryStream();

            if (image is Metafile)
            {
                m_srcMetafile = image as Metafile;
                System.Drawing.Rectangle rect = m_srcMetafile.GetMetafileHeader().Bounds;
                Bitmap bitmap = new Bitmap(rect.Width, rect.Height, m_srcMetafile.PixelFormat);
                Graphics graphics1 = Graphics.FromImage(bitmap);
                IntPtr ptr = graphics1.GetHdc();
                Metafile metafile = new Metafile(memStream, ptr, EmfType.EmfOnly);
                graphics1.ReleaseHdc(ptr);
                using (Graphics g = Graphics.FromImage(metafile))
                {
                    g.EnumerateMetafile(m_srcMetafile, rect.Location,
                      new Graphics.EnumerateMetafileProc(PlayInMeta));
                }
            }
            else
            {
                try
                {
                    image.Save(memStream, image.RawFormat);
                }
                catch
                {
                    image.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            return memStream;
#else
			return null;
#endif
        }
        #endregion
    }
}