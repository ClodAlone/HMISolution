#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using compression = System.IO.Compression;
using System.Text.RegularExpressions;
namespace Syncfusion.Compression.Zip
{
   public class ZipArchive
    {

        private Dictionary<string, ZipArchiveItem> m_dicItems;
        /// <summary>
        /// Collection of archive items.
        /// </summary>
        private List<ZipArchiveItem> m_arrItems = new List<ZipArchiveItem>();
        private CompressionLevel m_defaultLevel;
        /// <summary>
        /// Gets / sets default compression level - compression level for new items.
        /// By default is equal to CompressionLevel.Best.
        /// </summary>
        public CompressionLevel DefaultCompressionLevel
        {
            get
            {
                return m_defaultLevel;
            }
            set
            {
                m_defaultLevel = value;
            }
        }
        public ZipArchive()
        {
            m_dicItems = new Dictionary<string, ZipArchiveItem>();
        }
        /// <summary>
        /// Creates a copy of the current instance.
        /// </summary>
        /// <returns>A copy of the current instance.</returns>
        public ZipArchive Clone()
        {
            ZipArchive result = (ZipArchive)MemberwiseClone();
            result.m_arrItems = new List<ZipArchiveItem>();
            result.m_dicItems = new Dictionary<string, ZipArchiveItem>();

            for (int i = 0, len = m_arrItems.Count; i < len; i++)
            {
                ZipArchiveItem item = (ZipArchiveItem)m_arrItems[i];
                item = item.Clone();
                result.AddItem(item);
            }
            return result;
        }
        /// <summary>
        /// Searches for the item with specified name.
        /// </summary>
        /// <param name="itemRegex">Regular expression that defines item to find.</param>
        /// <returns>Zero-based item index if found; -1 otherwise.</returns>
        public int Find(Regex itemRegex)
        {
            int iResult = -1;

            for (int i = 0, len = m_arrItems.Count; i < len; i++)
            {
                ZipArchiveItem currentItem = m_arrItems[i];
                string strItemName = currentItem.ItemName;

                if (itemRegex.IsMatch(strItemName))
                {
                    iResult = i;
                    break;
                }
            }

            return iResult;
        }
        /// <summary>
        /// Searches for the item with specified name.
        /// </summary>
        /// <param name="itemName">Item to find.</param>
        /// <returns>Zero-based item index if found; -1 otherwise.</returns>
        public int Find(string itemName)
        {
            // TODO: maybe this can (or should) be optimized.
            ZipArchiveItem item;
            int iResult = -1;

            if (m_dicItems.TryGetValue(itemName, out item))
            {
                for (int i = 0, len = m_arrItems.Count; i < len; i++)
                {
                    ZipArchiveItem currentItem = m_arrItems[i];

                    if (currentItem == item)
                    {
                        iResult = i;
                        break;
                    }
                }
            }

            return iResult;
        }
        /// <summary>
        /// Adds new item to the archive
        /// </summary>
        /// <param name="itemName">Item name to add.</param>
        /// <param name="data">Items data stream (can be null for empty files or folders).</param>
        /// <param name="bControlStream">Indicates whether ZipArchive is responsible for stream closing.</param>
        /// <param name="attributes">File attributes.</param>
        /// <returns>Item that has been added.</returns>
        public void UpdateItem(string itemName, Stream data, bool bControlStream, FileAttributes attributes)
        {
            ZipArchiveItem item = this[itemName];

            if (item != null)
            {
                item.Update(data, bControlStream);
            }
            else
            {
                AddItem(itemName, data, bControlStream, attributes);
            }
        }
        /// <summary>
        /// Extracts Int32 value from the stream.
        /// </summary>
        /// <param name="stream">Stream to read data from.</param>
        /// <returns>Extracted value.</returns>
        public static int ReadInt32(Stream stream)
        {
            byte[] tempBuffer = new byte[Constants.IntSize];
            if (stream.Read(tempBuffer, 0, Constants.IntSize) != Constants.IntSize)
            {
#if DOCIO
                throw new Exception("Unable to read value at the specified position - end of stream was reached.");
#else
                throw new ZipException("Unable to read value at the specified position - end of stream was reached.");
#endif
            }

            return BitConverter.ToInt32(tempBuffer, 0);
        }
        /// <summary>
        /// Extracts Int16 value from the stream.
        /// </summary>
        /// <param name="stream">Stream to read data from.</param>
        /// <returns>Extracted value.</returns>
        public static short ReadInt16(Stream stream)
        {
            byte[] tempBuffer = new byte[Constants.ShortSize];
            if (stream.Read(tempBuffer, 0, Constants.ShortSize) != Constants.ShortSize)
            {
#if DOCIO
                throw new Exception("Unable to read value at the specified position - end of stream was reached.");
#else
                throw new ZipException("Unable to read value at the specified position - end of stream was reached.");
#endif
            }

            return BitConverter.ToInt16(tempBuffer, 0);
        }
       
        /// <summary>
        /// Removes item from the archive.
        /// </summary>
        /// <param name="itemName">Item name to remove.</param>
        public void RemoveItem(string itemName)
        {
            int iItemIndex = Find(itemName);

            if (iItemIndex >= 0)
            {
                RemoveAt(iItemIndex);
            }
        }
        /// <summary>
        /// Returns single archive item from the collection. Read-only.
        /// </summary>
        /// <param name="index">Zero-based index of the item to return.</param>
        /// <returns>Single archive item from the collection.</returns>
        public ZipArchiveItem this[int index]
        {
            get
            {
                if (index < 0 || index > m_arrItems.Count)
                    throw new ArgumentOutOfRangeException("index");

                return m_arrItems[index];
            }
        }
        /// <summary>
        /// Returns item by its name. Null if item wasn't found. Read-only.
        /// </summary>
        public ZipArchiveItem this[string itemName]
        {
            get
            {
                ZipArchiveItem result;
                m_dicItems.TryGetValue(itemName, out result);
                return result;
            }
        }
        /// <summary>
        /// Returns number of items inside archive. Read-only.
        /// </summary>
        public int Count
        {
            get
            {
                return (m_arrItems != null) ? m_arrItems.Count : 0;
            }
        }

        /// <summary>
        /// Returns the items inside archive. Read-only.
        /// </summary>
        public ZipArchiveItem[] Items
        {
            get
            {
                if (m_arrItems != null)
                    return m_arrItems.ToArray();
                else
                    throw new ArgumentOutOfRangeException("Items");
            }
        }


        /// <summary>
        /// Removes item at the specified position.
        /// </summary>
        /// <param name="index">Item index to remove.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= m_arrItems.Count)
                throw new ArgumentOutOfRangeException("index");

            ZipArchiveItem item = this[index];
            m_arrItems.RemoveAt(index);
            m_dicItems.Remove(item.ItemName);
        }
        public async void Open(Stream inFileStream,bool isControlStream)
        {
            //TODO WINRT: Need to implement properly.
            Open(inFileStream);
        }

        public async void Open(Stream inFileStream)
        {
            byte[] buffer = null;
            MemoryStream zipFileStream = new MemoryStream();
            if (inFileStream is MemoryStream)
            {
                zipFileStream = inFileStream as MemoryStream;
            }
            else
            {
                buffer = new byte[inFileStream.Length];
                inFileStream.Read(buffer, 0, buffer.Length);
                zipFileStream.Write(buffer, 0, buffer.Length);
                inFileStream.Dispose();
            }
            using (compression.ZipArchive zipArchive = new compression.ZipArchive(zipFileStream))
            {
                foreach (compression.ZipArchiveEntry entry in zipArchive.Entries)
                {
                    using (Stream dataStream = entry.Open())
                    {
                        ZipArchiveItem item = new ZipArchiveItem(this);
                        item.DataStream = new MemoryStream();
                        dataStream.CopyTo(item.DataStream);
#if XLSIO
                        item.DataStream.Position=0;
#endif
                        item.ItemName = entry.FullName;
                        m_dicItems.Add(entry.FullName, item);
                        m_arrItems.Add(item);
                    }
                }
            }

        }
        public async void Save(Stream outStream)
        {
            MemoryStream zipStream = new MemoryStream();
            byte[] buffer = null;
            using (compression.ZipArchive zipArchive = new compression.ZipArchive(zipStream, compression.ZipArchiveMode.Create))
            {
                Dictionary<string, ZipArchiveItem>.Enumerator itemsEnum = m_dicItems.GetEnumerator();
                while (itemsEnum.MoveNext())
                {
                    KeyValuePair<string, ZipArchiveItem> itemKeyValuePair = itemsEnum.Current;
                    compression.ZipArchiveEntry entry = zipArchive.CreateEntry(itemKeyValuePair.Key);
                    using (Stream entryStream = entry.Open())
                    {
                        using (MemoryStream entryOutStream = itemKeyValuePair.Value.DataStream as MemoryStream)
                        {
                            buffer = entryOutStream.ToArray();
                            await entryStream.WriteAsync(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
            buffer = zipStream.ToArray();
            await outStream.WriteAsync(buffer, 0, buffer.Length);
            outStream.Flush();
            outStream.Dispose();
        }
        /// <summary>
        /// Saves archive into specified file.
        /// </summary>
        /// <param name="outputFileName">Output file name.</param>
        /// <param name="createFilePath">Indicates whether we should create full path to the file if it doesn't exist.</param>
        public async void Save(string outputFileName, bool createFilePath)
        {
            if (outputFileName == null || outputFileName.Length == 0)
            {
                throw new ArgumentOutOfRangeException("outputFileName");
            }
            MemoryStream zipStream = new MemoryStream();
            byte[] buffer = null;
            using (compression.ZipArchive zipArchive = new compression.ZipArchive(zipStream, compression.ZipArchiveMode.Create,true))
            {
                Dictionary<string, ZipArchiveItem>.Enumerator itemsEnum = m_dicItems.GetEnumerator();
                while (itemsEnum.MoveNext())
                {
                    KeyValuePair<string, ZipArchiveItem> itemKeyValuePair = itemsEnum.Current;
                    compression.ZipArchiveEntry entry = zipArchive.CreateEntry(itemKeyValuePair.Key);
                    using (Stream entryStream = entry.Open())
                    {
                        using (MemoryStream entryOutStream = itemKeyValuePair.Value.DataStream as MemoryStream)
                        {
                            buffer = entryOutStream.ToArray();
                            await entryStream.WriteAsync(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
            //buffer = zipStream.ToArray();
            //await outStream.WriteAsync(buffer, 0, buffer.Length);
            //outStream.Flush();
            //outStream.Dispose();

        }
        public async void Save(Stream outStream, bool createFilePath)
        {
            using (MemoryStream zipStream = new MemoryStream())
            {
                byte[] buffer = null;
                using (compression.ZipArchive zipArchive = new compression.ZipArchive(zipStream, compression.ZipArchiveMode.Create,true))
                {
                    Dictionary<string, ZipArchiveItem>.Enumerator itemsEnum = m_dicItems.GetEnumerator();
                    while (itemsEnum.MoveNext())
                    {
                        KeyValuePair<string, ZipArchiveItem> itemKeyValuePair = itemsEnum.Current;
                        compression.ZipArchiveEntry entry = zipArchive.CreateEntry(itemKeyValuePair.Key);
                        using (Stream entryStream = entry.Open())
                        {
                            using (MemoryStream entryOutStream = itemKeyValuePair.Value.DataStream as MemoryStream)
                            {
                                buffer = entryOutStream.ToArray();
                                entryStream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }
                zipStream.Position = 0;
                zipStream.CopyTo(outStream);
                zipStream.Flush();
            }

        }
        /// <summary>
        /// Adds new item to the archive
        /// </summary>
        /// <param name="itemName">Item name to add.</param>
        /// <param name="data">Items data stream (can be null for empty files or folders).</param>
        /// <param name="bControlStream">Indicates whether ZipArchive is responsible for stream closing.</param>
        /// <param name="attributes">File attributes.</param>
        /// <returns>Item that has been added.</returns>
        public ZipArchiveItem AddItem(string itemName, Stream data)
        {
            itemName = itemName.Replace('\\', '/');

            if (itemName.IndexOf(':') != itemName.LastIndexOf(':'))
                throw new ArgumentOutOfRangeException("ZipItem name contains illegal characters.", "itemName");

            if (m_dicItems.ContainsKey(itemName))
                throw new ArgumentOutOfRangeException("Item " + itemName + " already exists in the archive");

#if WINRT
            ZipArchiveItem item = new ZipArchiveItem(this, itemName, data);
#else
            ZipArchiveItem item = new ZipArchiveItem(this, itemName, data, bControlStream, attributes);
#endif
            
            return AddItem(item);
        }
        /// <summary>
        /// Adds new item to the archive
        /// </summary>
        /// <param name="itemName">Item name to add.</param>
        /// <param name="data">Items data stream (can be null for empty files or folders).</param>
        /// <param name="bControlStream">Indicates whether ZipArchive is responsible for stream closing.</param>
        /// <param name="attributes">File attributes.</param>
        /// <returns>Item that has been added.</returns>
        public ZipArchiveItem AddItem(string itemName, Stream data, bool bControlStream, FileAttributes attributes)
        {
            itemName = itemName.Replace('\\', '/');

            if (itemName.IndexOf(':') != itemName.LastIndexOf(':'))
                throw new ArgumentOutOfRangeException("ZipItem name contains illegal characters.", "itemName");

            if (m_dicItems.ContainsKey(itemName))
                throw new ArgumentOutOfRangeException("Item " + itemName + " already exists in the archive");

            ZipArchiveItem item = new ZipArchiveItem(this, itemName, data, bControlStream, attributes);
           // item.CompressionLevel = m_defaultLevel;
            return AddItem(item);
        }
        /// <summary>
        /// Adds existing item to the archive.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <returns>Added item.</returns>
        public ZipArchiveItem AddItem(ZipArchiveItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            m_arrItems.Add(item);
            m_dicItems.Add(item.ItemName, item);
            return item;
        }

        internal void Dispose()
        {
            foreach (var items in m_arrItems)
                items.DataStream.Dispose();
            foreach (var items in m_dicItems)
                items.Value.DataStream.Dispose();
            m_dicItems.Clear();
            m_arrItems.Clear();
            //this.Dispose();//throw new NotImplementedException();
        }

        
    }
}
