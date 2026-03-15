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

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  /// <summary>
  /// Represents single directory entry in the compound file.
  /// </summary>
  public class DirectoryEntry
  {
    #region Constants
    /// <summary>
    /// Size of a single directory entry.
    /// </summary>
    public const int SizeInFile = 0x80;
    /// <summary>
    /// Size of the stream name field.
    /// </summary>
    private const int StreamNameSize = 0x40;
    /// <summary>
    /// Possible entry types.
    /// </summary>
    public enum EntryType
    {
      /// <summary>
      /// Invalid entry.
      /// </summary>
      Invalid = 0,
      /// <summary>
      /// Entry is storage.
      /// </summary>
      Storage = 1,
      /// <summary>
      /// Entry is stream.
      /// </summary>
      Stream = 2,
      LockBytes = 3,
      Property = 4,
      /// <summary>
      /// Root entry.
      /// </summary>
      Root = 5,
    }
    #endregion

    #region Members
    /// <summary>
    /// Entry name.
    /// </summary>
    private string m_strName;
    /// <summary>
    /// Entry type.
    /// </summary>
    private EntryType m_entryType;
    /// <summary>
    /// Entry "color" in red-black tree.
    /// </summary>
    private byte m_color = 1;
    /// <summary>
    /// Id of the left-sibling.
    /// </summary>
    private int m_leftId = -1;
    /// <summary>
    /// Id of the right-sibling.
    /// </summary>
    private int m_rightId = -1;
    /// <summary>
    /// Id of the child acting as the root of all the children of thes element (if entry type if Storage).
    /// </summary>
    private int m_childId = -1;
    /// <summary>
    /// Storage CLSID.
    /// </summary>
    private Guid m_storageGuid;
    /// <summary>
    /// User flags of this storage.
    /// </summary>
    private int m_iStorageFlags;
    /// <summary>
    /// Create time-stamp for a storage.
    /// </summary>
    private DateTime m_dateCreate;
    /// <summary>
    /// Modify time-stamp for a storage.
    /// </summary>
    private DateTime m_dateModify;
    /// <summary>
    /// Starting stream sector.
    /// </summary>
    private int m_iStartSector = -2;
    /// <summary>
    /// Stream size.
    /// </summary>
    private uint m_uiSize;
    /// <summary>
    /// Reserved. Must be zero.
    /// </summary>
    private int m_iReserved;
    /// <summary>
    /// Entry id.
    /// </summary>
    private int m_iEntryId;
    /// <summary>
    /// Last sector id.
    /// </summary>
    public int LastSector = -1;
    public int LastOffset = -1;
    #endregion

    #region Properties
    /// <summary>
    /// Entry name.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }
    /// <summary>
    /// Entry type.
    /// </summary>
    public EntryType Type
    {
      get
      {
        return m_entryType;
      }
      set
      {
        m_entryType = value;
      }
    }
    /// <summary>
    /// Entry "color" in red-black tree.
    /// </summary>
    public byte Color
    {
      get
      {
        return m_color;
      }
      set
      {
        m_color = value;
      }
    }
    /// <summary>
    /// Id of the left-sibling.
    /// </summary>
    public int LeftId
    {
      get
      {
        return m_leftId;
      }
      set
      {
        m_leftId = value;
      }
    }
    /// <summary>
    /// Id of the right-sibling.
    /// </summary>
    public int RightId
    {
      get
      {
        return m_rightId;
      }
      set
      {
        m_rightId = value;
      }
    }
    /// <summary>
    /// Id of the child acting as the root of all the children of thes element (if entry type if Storage).
    /// </summary>
    public int ChildId
    {
      get
      {
        return m_childId;
      }
      set
      {
        m_childId = value;
      }
    }
    /// <summary>
    /// Storage CLSID.
    /// </summary>
    public Guid StorageGuid
    {
      get
      {
        return m_storageGuid;
      }
      set
      {
        m_storageGuid = value;
      }
    }
    /// <summary>
    /// User flags of this storage.
    /// </summary>
    public int StorageFlags
    {
      get
      {
        return m_iStorageFlags;
      }
      set
      {
        m_iStorageFlags = value;
      }
    }
    /// <summary>
    /// Create time-stamp for a storage.
    /// </summary>
    public DateTime DateCreate
    {
      get
      {
        return m_dateCreate;
      }
      set
      {
        m_dateCreate = value;
      }
    }
    /// <summary>
    /// Modify time-stamp for a storage.
    /// </summary>
    public DateTime DateModify
    {
      get
      {
        return m_dateModify;
      }
      set
      {
        m_dateModify = value;
      }
    }
    /// <summary>
    /// Starting stream sector.
    /// </summary>
    public int StartSector
    {
      get
      {
        return m_iStartSector;
      }
      set
      {
        m_iStartSector = value;
      }
    }
    /// <summary>
    /// Stream size.
    /// </summary>
    public uint Size
    {
      get
      {
        return m_uiSize;
      }
      set
      {
        m_uiSize = value;
      }
    }
    /// <summary>
    /// Reserved. Must be zero.
    /// </summary>
    public int Reserved
    {
      get
      {
        return m_iReserved;
      }
      set
      {
      }
    }
    /// <summary>
    /// Returns entry id. Read-only.
    /// </summary>
    public int EntryId
    {
      get
      {
        return m_iEntryId;
      }
      internal set
      {
        m_iEntryId = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the entry.
    /// </summary>
    /// <param name="name">Name of the new entry.</param>
    /// <param name="type">Type of the new entry.</param>
    /// <param name="entryId">Id of the new entry.</param>
        public DirectoryEntry(string name, EntryType type, int entryId)
    {
      m_strName = name;
      m_entryType = type;
      m_iEntryId = entryId;
      m_dateModify = m_dateCreate = DateTime.Now;
    }
    /// <summary>
    /// Initializes new instance of the entry.
    /// </summary>
    /// <param name="data">Data of the new entry.</param>
    /// <param name="offset">Offset to the entry data.</param>
    /// <param name="entryId">Entry id.</param>
        public DirectoryEntry(byte[] data, int offset, int entryId)
    {
            if (data == null)
                throw new ArgumentNullException("data");

            if (offset < 0 || offset >= data.Length)
                throw new ArgumentOutOfRangeException("offset");

      m_iEntryId = entryId;
            int iNameLength = BitConverter.ToUInt16(data, offset + StreamNameSize);
            m_strName = Encoding.Unicode.GetString(data, offset, iNameLength);

      iNameLength = m_strName.Length;

            if (iNameLength > 0 && m_strName[iNameLength - 1] == '\0')
                m_strName = m_strName.Substring(0, iNameLength - 1);

      offset += StreamNameSize + 2;

            m_entryType = (EntryType)data[offset];
      offset++;

            m_color = data[offset];
      offset++;

            m_leftId = BitConverter.ToInt32(data, offset);
      offset += 4;

            m_rightId = BitConverter.ToInt32(data, offset);
      offset += 4;

            m_childId = BitConverter.ToInt32(data, offset);
      offset += 4;

            byte[] arrGuid = new byte[16];
            Buffer.BlockCopy(data, offset, arrGuid, 0, 16);
            m_storageGuid = new Guid(arrGuid);
      offset += 16;

            m_iStorageFlags = BitConverter.ToInt32(data, offset);
      offset += 4;

      // skip dates
            long time = BitConverter.ToInt64(data, offset);
            long maxValue = DateTime.MaxValue.ToFileTime();
            if ((time >= 0) && (time <= maxValue))
            m_dateCreate = DateTime.FromFileTime(time);
      offset += 8;

            time = BitConverter.ToInt64(data, offset);
            if ((time >= 0) && (time <= maxValue))
            m_dateModify = DateTime.FromFileTime(time);
      offset += 8;

            m_iStartSector = BitConverter.ToInt32(data, offset);
      offset += 4;

            m_uiSize = BitConverter.ToUInt32(data, offset);
      offset += 4;

            m_iReserved = BitConverter.ToInt32(data, offset);
      offset += 4;

      //Console.WriteLine( "--------------------" );
      ////Console.WriteLine( "{0}:{1}:{2}:{3}:{4}", m_strName, m_uiSize, m_entryType, m_dateCreate, m_dataModify );
      //Console.WriteLine( "{0}:{1}:{2}:{3}", m_strName, m_leftId, m_rightId, m_childId );
    }
    /// <summary>
    /// Writes directory entry data inside specified stream.
    /// </summary>
    /// <param name="stream">Stream to write data into.</param>
        public void Write(Stream stream)
    {
            if (stream == null)
                throw new ArgumentNullException("stream");

      long lStartPosition = stream.Position;

            if (m_entryType == EntryType.Invalid)
        m_leftId = m_rightId = m_childId = -1;

            byte[] arrBuffer = Encoding.Unicode.GetBytes(m_strName);
            stream.Write(arrBuffer, 0, arrBuffer.Length);
            stream.WriteByte(0);
            stream.WriteByte(0);
      stream.Position = lStartPosition + StreamNameSize;

            arrBuffer = BitConverter.GetBytes((short)(arrBuffer.Length + 2));
            stream.Write(arrBuffer, 0, FileHeader.ShortSize);

            stream.WriteByte((byte)m_entryType);
            stream.WriteByte((byte)m_color);

            arrBuffer = BitConverter.GetBytes(m_leftId);
            stream.Write(arrBuffer, 0, FileHeader.IntSize);

            arrBuffer = BitConverter.GetBytes(m_rightId);
            stream.Write(arrBuffer, 0, FileHeader.IntSize);

            arrBuffer = BitConverter.GetBytes(m_childId);
            stream.Write(arrBuffer, 0, FileHeader.IntSize);

            if (Type == EntryType.Root && m_storageGuid.CompareTo(Guid.Empty) == 0)
      {
                m_storageGuid = new Guid("00020820-0000-0000-c000-000000000046");//Guid.NewGuid();
      }

      arrBuffer = m_storageGuid.ToByteArray();
            stream.Write(arrBuffer, 0, arrBuffer.Length);

            arrBuffer = BitConverter.GetBytes(m_iStorageFlags);
            stream.Write(arrBuffer, 0, FileHeader.IntSize);

      // skip dates
      long time = 0;//m_dateCreate.ToFileTime();
            arrBuffer = BitConverter.GetBytes(time);
            stream.Write(arrBuffer, 0, arrBuffer.Length);

      time = 0;//m_dateModify.ToFileTime();
            arrBuffer = BitConverter.GetBytes(time);
            stream.Write(arrBuffer, 0, arrBuffer.Length);

            arrBuffer = BitConverter.GetBytes(m_iStartSector);
            stream.Write(arrBuffer, 0, FileHeader.IntSize);

            arrBuffer = BitConverter.GetBytes(m_uiSize);
            stream.Write(arrBuffer, 0, FileHeader.IntSize);

            arrBuffer = BitConverter.GetBytes(m_iReserved);
            stream.Write(arrBuffer, 0, FileHeader.IntSize);
    }
    #endregion
  }
}
