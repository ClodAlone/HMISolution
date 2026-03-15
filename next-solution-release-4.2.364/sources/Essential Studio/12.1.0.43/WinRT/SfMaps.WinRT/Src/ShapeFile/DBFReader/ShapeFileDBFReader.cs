#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
#if WINRT
    using Windows.UI.Xaml;
    using System.Net.Http;
    using Windows.Storage;
#endif
    using System.Reflection;
    using System.Net;
    using System.ComponentModel;
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    internal class ShapeFileDBFReader
    {
        #region CLR Properties

        public ShapeFileDBFData DBFData
        {
            get
            {
                return this.dbfData;
            }
            internal set
            {
                this.dbfData = value;
            }

        }


        #endregion

        #region PrivateFields

        private BinaryReader dbfBinaryReader;
        private ShapeFileDBFData dbfData;


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DBFReader.ShapeFileDBFReader">ShapeFileDBFReader</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ShapeFileDBFReader()
        {
           
        }

#if WPF || WINRT
        public event ProgressChangedEventHandler DBFFileDownloaded;
        private void OnDBFFileDownloaded()
        {
            var eventHandler = DBFFileDownloaded;
            if (eventHandler != null)
            {
                eventHandler(this, new ProgressChangedEventArgs(100, null));
            }
        }
#endif


        /// <summary>
        /// Method that Reads the DBF Data.
        /// </summary>
        /// <remarks></remarks>
        public void ReadDBFData(string shapefilename)
        {
            if (!shapefilename.Equals(string.Empty))
            {
                shapefilename = shapefilename.Replace(".shp", ".dbf");
                Stream dbfStream = null;
#if WPF
                if (shapefilename.StartsWith("http") || shapefilename.StartsWith("www"))
                {
                    WebClient client = new WebClient();
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(new Uri(shapefilename), "shapes.dbf");
                    client.Dispose();

                }
                else if (File.Exists(shapefilename))
                {
                    dbfStream = File.OpenRead(shapefilename);
                }
                else if (shapefilename.Contains("component"))
                {
                    dbfStream = Application.GetResourceStream(new Uri(shapefilename)).Stream;
                }
                else if (Application.Current == null)
                {
                    String[] resources = Application.ResourceAssembly.GetManifestResourceNames();
                    foreach (var item in resources)
                    {
                        if (item.Contains(".dbf"))
                        {
                            dbfStream = Application.ResourceAssembly.GetManifestResourceStream(item);
                        }
                    }
                }
                else
                {
                    dbfStream = Application.Current.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(shapefilename);
                }

#elif WINRT
                if (shapefilename.StartsWith("http") || shapefilename.StartsWith("www"))
                {
                    this.ReadFromWeb(shapefilename);
                }
                else if (shapefilename.StartsWith("ms-appx"))
                {
                    ReadFromAssembly(shapefilename);
                }
                else
                {
                    dbfStream =
                        Application.Current.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(shapefilename);
                }
#elif WINDOWSFORMS
                if (File.Exists(shapefilename))
                {
                    dbfStream = File.OpenRead(shapefilename);
                }
#else
          dbfStream = Application.Current.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(shapefilename);
   
#endif
                if (dbfStream != null)
                {
                    this.ReadFromStream(dbfStream);
                }
            }
        }

#if WPF
        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                FileStream file = File.OpenRead("shapes.dbf");
                this.ReadFromStream(file);
                OnDBFFileDownloaded();

            }      
        }
#endif
#if WINRT
        async private void ReadFromAssembly(string dbffilename)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(dbffilename));
                if (file != null)
                {
                    Stream filestream = await file.OpenStreamForReadAsync();
                    if (filestream != null)
                    {
                        this.ReadFromStream(filestream);
                        OnDBFFileDownloaded();
                    }


                }
            }
            catch (Exception)
            {

            }

        }
        async private void ReadFromWeb(string dbffilename)
        {
            Stream responseBody = new MemoryStream();
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(dbffilename);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStreamAsync();
                if (responseBody != null)
                {
                    this.ReadFromStream(responseBody);
                    OnDBFFileDownloaded();
                }
                client.Dispose();

            }
            catch (HttpRequestException e)
            {
                throw new NotSupportedException(e.Message);
            }

        }

#endif
            
        public void ReadFromStream(Stream dbfStream)
        {
                this.dbfBinaryReader = new BinaryReader(dbfStream);
                this.dbfData = new ShapeFileDBFData();
                this.dbfData.DBFHeader = this.ReadHeader(this.dbfBinaryReader);
                this.dbfData.DBFFields = this.ReadDBFDataValues(this.dbfBinaryReader);
        }
        public ShapeFileDBFHeader ReadHeader(BinaryReader reader)
        {
            ShapeFileDBFHeader header = new ShapeFileDBFHeader();
            header.FileType = reader.ReadByte();
            header.Year = reader.ReadByte();
            header.Month = reader.ReadByte();
            header.Day = reader.ReadByte();
            header.NumberOfRecords = reader.ReadInt32();
            header.Length = reader.ReadInt16();
            header.RecordLength = reader.ReadInt16();
            header.Reserve1 = reader.ReadInt16();
            header.IncompleteTransaction = reader.ReadByte();
            header.EncriptionFlag = reader.ReadByte();
            header.FreeRecordThread = reader.ReadInt32();
            header.Reserve2 = reader.ReadInt32();
            header.Reserve3 = reader.ReadInt32();
            header.MDXFlag = reader.ReadByte();
            header.LanguageDriver = reader.ReadByte();
            header.Reserve4 = reader.ReadInt16();
            return header;
            
        }
        private List<DBFValues> ReadDBFDataValues(BinaryReader reader)
        {
            ShapeFileDBFField field = new ShapeFileDBFField();
            List<ShapeFileDBFField> m_tempFields = new List<ShapeFileDBFField>();
            List<DBFValues> vals = new List<DBFValues>();
            while (field.ReadFields(reader))
            {
                m_tempFields.Add(field);
                field = new ShapeFileDBFField();
            }
            int t_dataStartIndex = this.dbfData.DBFHeader.Length - (32 + (32 * m_tempFields.Count)) - 1;
            reader.ReadByte();
            for (int i = 0; i < this.dbfData.DBFHeader.NumberOfRecords; i++)
            {
                DBFValues val = new DBFValues();
                for (int j = 0; j < m_tempFields.Count; j++)
                {
                    val.Add(new DBFFieldValues { Key = m_tempFields[j].Name.ToString(), Value = ASCIIEncoder.GetString(reader.ReadBytes(m_tempFields[j].m_Length)) });

                }
                vals.Add(val);
                if (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    reader.ReadByte();
                }
            }
            return vals;
        }

       
    }
}
