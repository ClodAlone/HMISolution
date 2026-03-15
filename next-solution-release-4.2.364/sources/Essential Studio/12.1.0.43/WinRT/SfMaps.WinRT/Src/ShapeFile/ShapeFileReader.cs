#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINRT
using System.Net.Http;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;

#else
using System.Windows;
#if !WINDOWSFORMS
using System.Windows.Media;
#else
using Syncfusion.Windows.Forms.Maps;
#endif
#endif
using System.Reflection;
using System.IO;
using System.Net;
using System.ComponentModel;



namespace Syncfusion.UI.Xaml.Maps
{
    internal class ShapeFileReader
    {
        #region PrivateFields
        // as per the ShapeFile specification the file code is always 9994 or hexvalue - 0x0000270a
        private const int ExpectedFileCode = 9994;
        // Hold the ShapeFile Cotents as byte Array;
        private byte[] shapeFileContents;

        // Hold the Current Pointer in the shapeFileContents byte Array;
        private int currentIndex = 0;

#if WPF || WINRT
        public event ProgressChangedEventHandler ShapeFileDownloaded;
        private void OnShapeFileDownloaded()
        {
            var eventHandler = ShapeFileDownloaded;
            if (eventHandler != null)
            {
                eventHandler(this, new ProgressChangedEventArgs(100,null));
            }
        }
#endif


        #endregion

        #region Internal Fields

        internal List<ShapeFileRecord> Records = new List<ShapeFileRecord>();
        internal ShapeFileHeader Header = new ShapeFileHeader();
        
        #endregion
       
        #region private fields

        private string shapefilename;
        private string dbffilename;

        #endregion

        #region Constrctor

        public ShapeFileReader()
        {
            
        }

        public ShapeFileReader(string shapefilename)
        {
            this.shapefilename = shapefilename;
            this.dbffilename = shapefilename.Replace(".shp", ".dbf");
        }

        #endregion

        #region HelperMethods

        public void ReadShapeFile()
        {
            if (!this.shapefilename.Equals(string.Empty))
            {
                Stream shapeStream = null;
#if WPF            
                if (this.shapefilename.StartsWith("http") || this.shapefilename.StartsWith("www"))
                {
                    WebClient client = new WebClient();
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(new Uri(shapefilename), "shapes.shp");
                    client.Dispose();

                }
                else if (File.Exists(shapefilename))
                {
                    shapeStream = File.OpenRead(this.shapefilename);
                }
                else if (this.shapefilename.Contains("component"))
                {
                    shapeStream = Application.GetResourceStream(new Uri(this.shapefilename)).Stream;
                }
                else if (Application.Current == null)
                {
                    String[] resources = Application.ResourceAssembly.GetManifestResourceNames();
                    foreach (var item in resources)
                    {
                        if (item.Contains(".shp"))
                        {
                            shapeStream = Application.ResourceAssembly.GetManifestResourceStream(item);
                        }
                    }
                }
                else
                {
                    shapeStream = Application.Current.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(this.shapefilename);
                }


#elif WINRT
                if (this.shapefilename.StartsWith("http") || this.shapefilename.StartsWith("www"))
                {
                  ReadFromWeb();
                }
                else if (shapefilename.StartsWith("ms-appx"))
                {
                    ReadFromAssembly();
                }
                else
                {
                    shapeStream = Application.Current.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(this.shapefilename);

                }
                #elif WINDOWSFORMS
                if (File.Exists(shapefilename))
                {
                    shapeStream = File.OpenRead(shapefilename);
                }

#else
                {
                   
                    shapeStream = Application.Current.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(this.shapefilename);
                    
                }

#endif

                if (shapeStream != null)
                {
                    this.ReadFromStream(shapeStream);
                }
            }

           
        }
      
#if WPF 
        void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {               
                FileStream file = File.OpenRead("shapes.shp");
                this.ReadFromStream(file);
                OnShapeFileDownloaded();

            }            
        }
#endif
#if WINRT
        async private void ReadFromAssembly()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(this.shapefilename));
                if (file != null)
                {
                   Stream filestream= await file.OpenStreamForReadAsync();
                    if (filestream != null)
                    {
                        this.ReadFromStream(filestream);
                        OnShapeFileDownloaded();
                    }


                }
            }
            catch (Exception)
            {

            }
           
        }
        async private void ReadFromWeb()
        {
            Stream responseBody= new MemoryStream();
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(this.shapefilename);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStreamAsync();
                if (responseBody != null)
                {
                    this.ReadFromStream(responseBody);
                    OnShapeFileDownloaded();
                }
                client.Dispose();
                
            }
            catch (HttpRequestException e)
            {
                throw new NotSupportedException(e.Message);
            }
        }

#endif

        public void ReadFromStream(Stream fs)
        {
            this.shapeFileContents = ConvertBytes(fs);
            var header = this.ReadHeader(fs);
            this.Header = header;
            this.ReadRecords(fs);
            
        }

        internal void ReadPathRecord(byte[] _bytes,int index)
        {
            var _pathStream = new MemoryStream(_bytes);
            var _pathBinary = new BinaryReader(_pathStream);
            var record = this.ReadPathBinary(_pathBinary,index);

            if (this.Header == null)
            {
                this.Header = new ShapeFileHeader();
            }

            this.Records.Add(record);

            if (index == 0)
            {
                this.Header.MinX = record.MinX;
                this.Header.MaxX = record.MaxX;
                this.Header.MinY = record.MinY;
                this.Header.MaxY = record.MaxY;
                this.Header.ShapeType = this.Records[index].ShapeType;
             }
            else
            {
                if (this.Records[index].MinX < this.Header.MinX) this.Header.MinX = this.Records[index].MinX;
                if (this.Records[index].MaxX > this.Header.MaxX) this.Header.MaxX = this.Records[index].MaxX;
                if (this.Records[index].MinY < this.Header.MinY) this.Header.MinY = this.Records[index].MinY;
                if (this.Records[index].MaxY > this.Header.MaxY) this.Header.MaxY = this.Records[index].MaxY;
            }
        }

        private void ReadRecords(Stream fs)
        {
            // parse records            
            while (this.currentIndex < this.shapeFileContents.Length)
            {
                var record = this.ReadRecordLine(fs);
                this.Records.Add(record);
            }
            fs.Dispose();
            Array.Clear(this.shapeFileContents, 0, this.shapeFileContents.Length);
#if !WPF
            // merge from DBF file
          //  this.ReadAttributes(this.dbFileName, fileData);
#endif
        }

        /// <summary>
        /// Reads and parses the header of the .shp index file
        /// </summary>
        /// <remarks>        
        /// 
        /// Byte
        /// Position    Field           Value       Type    Order
        /// -----------------------------------------------------
        /// Byte 0      File Code       9994        Integer Big
        /// Byte 4      Unused          0           Integer Big
        /// Byte 8      Unused          0           Integer Big
        /// Byte 12     Unused          0           Integer Big
        /// Byte 16     Unused          0           Integer Big
        /// Byte 20     Unused          0           Integer Big
        /// Byte 24     File Length     File Length Integer Big
        /// Byte 28     Version         1000        Integer Little
        /// Byte 32     Shape Type      Shape Type  Integer Little
        /// Byte 36     Bounding Box    Xmin        Double  Little
        /// Byte 44     Bounding Box    Ymin        Double  Little
        /// Byte 52     Bounding Box    Xmax        Double  Little
        /// Byte 60     Bounding Box    Ymax        Double  Little
        /// Byte 68*    Bounding Box    Zmin        Double  Little
        /// Byte 76*    Bounding Box    Zmax        Double  Little
        /// Byte 84*    Bounding Box    Mmin        Double  Little
        /// Byte 92*    Bounding Box    Mmax        Double  Little
        /// 
        /// * Unused, with value 0.0, if not Measured or Z type
        /// 
        /// The "Integer" type corresponds to the CLS Int32 type, and "Double" to CLS Double (IEEE 754).
        /// </remarks>

        public ShapeFileHeader ReadHeader(Stream stream)
        {
            var fileHeader = new ShapeFileHeader();
            var fileCode = BitConverter.ToInt32(Helpers.SplitReverse(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0);
            if (fileCode != ShapeFileReader.ExpectedFileCode)
            {
                string msg = String.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid FileCode encountered. Expecting {0}.", ShapeFileReader.ExpectedFileCode);
                throw new InvalidOperationException(msg);
            }

            // set file code
            fileHeader.FileCode = fileCode;

            // five unused values

            this.currentIndex += 20;
            // file length
            fileHeader.FileLength = BitConverter.ToInt32(Helpers.SplitReverse(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0);

            // version
            fileHeader.Version = BitConverter.ToInt32(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0);

            // shape type
            fileHeader.ShapeType = GetShapeTypeFromShapeStream(BitConverter.ToInt32(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0));

            // bounding box
            fileHeader.MinX = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            fileHeader.MinY = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            fileHeader.MaxX = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            fileHeader.MaxY = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            fileHeader.MinZ = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            fileHeader.MaxZ = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            fileHeader.MinM = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            fileHeader.MaxM = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);

            // header construction complete
            return fileHeader;
        }

        public ShapeFileRecord ReadRecordLine(Stream stream)
        {
            var record = new ShapeFileRecord();

            // parse record header
            record.RecordIndex = BitConverter.ToInt32(Helpers.SplitReverse(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0); ;
            record.ContentLength = BitConverter.ToInt32(Helpers.SplitReverse(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0); ;

            // shape type
            record.ShapeType = GetShapeTypeFromShapeStream(BitConverter.ToInt32(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0));
            switch (record.ShapeType)
            {
                case ShapeType.NullShape:

                    // do nothing
                    break;
                case ShapeType.Point:
                    ReadPoint(record);
                    break;
                case ShapeType.PolyLine:

                    // polyline is as same as polygon
                    ReadPolygon(record);
                    break;
                case ShapeType.Polygon:
                    ReadPolygon(record);
                    break;
                case ShapeType.Multipoint:
                    ReadMultiPoint(record);
                    break;
                default:

                    // TODO : Implement other shape types
                    break;
            }

            return record;
        }

        private void ReadPoint(ShapeFileRecord record)
        {
#if WINDOWSFORMS
            var pt = new System.Drawing.Point();
            pt.X = (int)BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            pt.Y = (int)BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
#else
            var pt = new Point();
            pt.X = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            pt.Y = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
#endif
            record.Points.Add(pt);
            record.MinX = record.MaxX = pt.X;
            record.MinY = record.MaxY = pt.Y;
        }

        public static ShapeType GetShapeTypeFromShapeStream(int shapeType)
        {
            switch (shapeType)
            {
                case 0:
                    return ShapeType.NullShape;
                case 1:
                    return ShapeType.Point;
                case 3:
                    return ShapeType.PolyLine;
                case 5:
                    return ShapeType.Polygon;
                case 8:
                    return ShapeType.Multipoint;
                default:
                    throw new InvalidOperationException(string.Format("ShapeType {0} not found", shapeType));
            }
        }

        private void ReadPolygon(ShapeFileRecord record)
        {
            // form the bounding box
            record.MinX = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            record.MinY = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            record.MaxX = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            record.MaxY = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);

            // num parts and points
            int numparts = BitConverter.ToInt32(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0);
            int points = BitConverter.ToInt32(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0);

            for (int i = 0; i < numparts; i++)
            {
                record.Parts.Add(BitConverter.ToInt32(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0));
            }

            for (int i = 0; i < points; i++)
            {
#if WINDOWSFORMS
                record.Points.Add(new System.Drawing.Point((int)BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0), (int)BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0)));
#else
                record.Points.Add(new Point(BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0), BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0)));
#endif
            }
        }

        private void ReadMultiPoint(ShapeFileRecord record)
        {
            // form the bounding box
            record.MinX = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            record.MinY = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            record.MaxX = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            record.MaxY = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);

            // points
            var points = BitConverter.ToInt32(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0);

            // populate points
            for (int i = 0; i < points; i++)
            {
                

#if WINDOWSFORMS
                var pt = new System.Drawing.Point();
                pt.X = (int)BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
                pt.Y = (int)BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
#else
                  var pt = new Point();
                pt.X = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
                pt.Y = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
                record.Points.Add(pt);
#endif
            }
        }

        //Sqlbytes Reader 
        ShapeFileRecord ReadPathBinary(BinaryReader reader,int index)
        {
            var record = new ShapeFileRecord();
            double minx = 0.0;
            double maxx = 0.0;
            double miny = 0.0;
            double maxy = 0.0;

            byte num = reader.ReadByte();
            uint isNum = reader.ReadUInt32();
            if (isNum == 6)
            {
                //PolygonSignedArea count
                uint shape = reader.ReadUInt32();
                if (shape != 0)
                {
                    for (int i = 0; i < shape; i++)
                    {
                        //SignedArea Start
                        byte tnum = reader.ReadByte();
                        uint tisNum = reader.ReadUInt32();
                        if (num == tnum && tisNum == 3)
                        {
                            int snum = reader.ReadInt32();
                            if (record.Parts.Count == 0)
                            {
                                record.Parts.Add(0);
                            }
                            else
                            {
                                record.Parts.Add(record.Points.Count);
                            }
                        }
                        else
                        {
                            reader.BaseStream.Position -= 5;
                        }
                        int count = reader.ReadInt32();
            
                        for (int j = 0; j < count; j++)
                        {
#if WINDOWSFORMS
                            var pt = new System.Drawing.Point();
                             pt.X= (int)reader.ReadDouble();
                             pt.Y = (int)reader.ReadDouble();
#else
                        var pt = new Point();

                             pt.X= reader.ReadDouble();
                             pt.Y = reader.ReadDouble();
#endif

                             record.Points.Add(pt);
                        }

                        int start = record.Points.Count - count;

                        minx = record.MinX == 0.0 ? record.Points[0].X : record.MinX;

                        maxx = record.MaxX == 0.0 ? record.Points[0].X : record.MaxX;

                        for (int j = start; j < count; j++)
                        {
                            if (record.Points[j].X < minx) minx = record.Points[j].X;
                            if (record.Points[j].X > maxx) maxx = record.Points[j].X;
                        }

                        miny = record.MinY == 0.0 ? record.Points[0].Y : record.MinY;

                        maxy = record.MaxY == 0.0 ? record.Points[0].Y : record.MaxY;

                        for (int j = start; j < count; j++)
                        {
                            if (record.Points[j].Y < miny) miny = record.Points[j].Y;
                            if (record.Points[j].Y > maxy) maxy = record.Points[j].Y;
                        }

                        record.MaxX = maxx;
                        record.MaxY = maxy;
                        record.MinX = minx;
                        record.MinY = miny;

                        //SignedArea End
                    }
                    record.RecordIndex = index;
                }
            }
            if (record.Points.Count == 1 && record.Parts.Count == 0)
            {
                record.ShapeType = ShapeType.Point;
            }
            else if (record.Points.Count > 1 && record.Parts.Count == 0)
            {
                record.ShapeType = ShapeType.Multipoint;                
            }
            else
            {
                record.ShapeType = ShapeType.Polygon;                                
            }
            return record;
        }
         

        private static byte[] ConvertBytes(Stream input)
        {
            // Array to hold the stream as bytes.
            byte[] bytes = new byte[16 * 1024];

            // Read Memory stream
            using (MemoryStream ms = new MemoryStream())
            {
                int read;

                // Read till end of the stream.
                while ((read = input.Read(bytes, 0, bytes.Length)) > 0)
                {
                    // Write the readed in to buffer.
                    ms.Write(bytes, 0, read);
                }

                // Return byte array.
                return ms.ToArray();
            }
        }


        #endregion

    }
    #region HelperClasses

    internal static class Helpers
    {
        /// <summary>
        /// Get the array slice between the two indexes.
        /// ... Inclusive for start index, exclusive for end index.
        /// </summary>
        public static T[] Split<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }
        public static T[] SplitReverse<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            int k = 0;
            for (int i = len - 1; i >= 0; i--)
            {
                res[k] = source[i + start];
                k++;
            }
            return res;
        }
    }
    #endregion

}
