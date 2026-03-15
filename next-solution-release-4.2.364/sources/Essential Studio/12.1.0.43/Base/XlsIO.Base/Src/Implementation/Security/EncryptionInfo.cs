#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
namespace Syncfusion.XlsIO.Implementation.Security
{
  public class EncryptionInfo
  {
    #region Members
    /// <summary>
    /// A Version structure where Version.vMajor MUST be 0x0003, and Version.vMinor MUST be 0x0002.
    /// </summary>
    private int m_iVersionInfo;
    /// <summary>
    /// A copy of the Flags stored in the EncryptionHeader field of this structure.
    /// </summary>
    private int m_iFlags;
    /// <summary>
    /// An EncryptionHeader structure that specifies parameters used to encrypt data.
    /// </summary>
    private EncryptionHeader m_header = new EncryptionHeader();
    /// <summary>
    /// An EncryptionVerifier structure.
    /// </summary>
    private EncryptionVerifier m_verifier = new EncryptionVerifier();
    /// <summary>
    /// This structure specifies parameters to encrypt the Key.
    /// </summary>
    private EncryptedKeyInfo m_keyInfo;
    /// <summary>
    /// This structure specifies parameters to ensure the data integrity.
    /// </summary>
    private DataIntegrityInfo m_dataIntegrity;
    /// <summary>
    /// This structure specifies parameters to Encrypt the Data.
    /// </summary>
    private DataEncryptionInfo m_dataEncryption;
    #endregion

    #region Properties
    /// <summary>
    /// This structure specifies parameters to encrypt the Key.
    /// </summary>
    internal EncryptedKeyInfo KeyInfo
    {
        get
        {
            return m_keyInfo;
        }
        set
        {
            m_keyInfo=value;
        }
    }
    /// <summary>
    /// This structure specifies parameters to ensure the data integrity.
    /// </summary>
    internal DataIntegrityInfo DataIntegrity
    {
        get
        {
            return m_dataIntegrity;
        }
        set
        {
            m_dataIntegrity = value;
        }
    }
    /// <summary>
    /// This structure specifies parameters to Encrypt the Data.
    /// </summary>
    internal DataEncryptionInfo DataEncryption
    {
        get
        {
            return m_dataEncryption;
        }
        set
        {
            m_dataEncryption = value;
        }
    }
    /// <summary>
    /// A Version structure where Version.vMajor MUST be 0x0003, and Version.vMinor MUST be 0x0002.
    /// </summary>
    public int VersionInfo
    {
      get
      {
        return m_iVersionInfo;
      }
      set
      {
        m_iVersionInfo = value;
      }
    }
    /// <summary>
    /// A copy of the Flags stored in the EncryptionHeader field of this structure.
    /// </summary>
    public int Flags
    {
      get
      {
        return m_iFlags;
      }
      set
      {
        m_iFlags = value;
      }
    }
    /// <summary>
    /// An EncryptionHeader structure that specifies parameters used to encrypt data.
    /// </summary>
    public EncryptionHeader Header
    {
      get
      {
        return m_header;
      }
    }
    /// <summary>
    /// An EncryptionVerifier structure.
    /// </summary>
    public EncryptionVerifier Verifier
    {
      get
      {
        return m_verifier;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public EncryptionInfo()
    {
        m_keyInfo = new EncryptedKeyInfo();
        m_dataIntegrity = new DataIntegrityInfo();
        m_dataEncryption = new DataEncryptionInfo();
    }
    /// <summary>
    /// Initializes new instance.
    /// </summary>
    /// <param name="stream">Stream to get data from.</param>
    public EncryptionInfo( Stream stream )
    {
      byte[] arrBuffer = new byte[ ExcelConstants.IntSize ];
      m_iVersionInfo = SecurityHelper.ReadInt32( stream, arrBuffer );
      m_iFlags = SecurityHelper.ReadInt32( stream, arrBuffer );
      if (m_iVersionInfo == SecurityHelper.Excel2010Version)
      {
          XmlReader reader = UtilityMethods.CreateReader(stream);
          reader.Read();
          m_dataEncryption = new DataEncryptionInfo();
          m_dataEncryption.Parse(reader);
          
          reader.Read();
          m_dataIntegrity = new DataIntegrityInfo();
          m_dataIntegrity.Parse(reader);
          reader.Read();
          reader.Read();
          reader.Read();
          m_keyInfo = new EncryptedKeyInfo();
          m_keyInfo.Parse(reader);
      }
      else
      {
          m_header.Parse(stream);
          m_verifier.Parse(stream);
      }
    }
    /// <summary>
    /// Serializes object into stream.
    /// </summary>
    /// <param name="stream">Stream to serialize into.</param>
    public void Serialize( Stream stream )
    {
        SecurityHelper.WriteInt32(stream, m_iVersionInfo);
        SecurityHelper.WriteInt32(stream, m_iFlags);
        if (m_iVersionInfo == SecurityHelper.Excel2010Version)
        {

            MemoryStream memoryStream = new MemoryStream();
            XmlWriter writer = UtilityMethods.CreateWriter(memoryStream, Encoding.UTF8);
            writer.WriteStartDocument();

            writer.WriteStartElement(EncryptionConstants.EncryptionTag, EncryptionConstants.EncryptionNameSpace);
            writer.WriteAttributeString(EncryptionConstants.Xmlns, EncryptionConstants.PPrefix, null, EncryptionConstants.PasswordNameSpace);
            m_dataEncryption.Serialize(writer);
            if (m_dataIntegrity != null)
                m_dataIntegrity.Serialize(writer);
            writer.WriteStartElement(EncryptionConstants.KeyEncryptorsTag);
            m_keyInfo.Serialize(writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            //FileStream s = File.Create("d.xml");
            memoryStream.Position = 0;
            byte[] bt = new byte[memoryStream.Length];
            memoryStream.Read(bt, 0, bt.Length);
            //s.Write(bt, 0, bt.Length);

            //s.Close();
#if ( WINRT )
            memoryStream.Dispose();
#else
            memoryStream.Close();
#endif
            stream.Write(bt, 0, bt.Length);
        }
        else
        {

            m_header.Serialize(stream);
            m_verifier.Serialize(stream);
        }
      
    }
    #endregion
  }
}
