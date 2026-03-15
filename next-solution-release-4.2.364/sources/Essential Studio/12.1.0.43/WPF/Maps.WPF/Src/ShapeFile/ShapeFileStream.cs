#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Maps.IO
{
    using System;
    using System.Collections.Generic;
#if WPF
    using System.Collections;
    using System.Data;
    using System.Data.OleDb;
#endif
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Diagnostics;

    /// From ESRI Shapefile Technical Description document
    /// 
    /// http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
    public class ShapeFileStream : Stream
    {
        // as per the ShapeFile specification the file code is always 9994 or hexvalue - 0x0000270a
        private const int ExpectedFileCode = 9994;
        private string fileName = string.Empty;
        private string dbFileName = string.Empty;
        private string indexFileName = string.Empty;

        // Hold the ShapeFile Cotents as byte Array;
        private byte[] shapeFileContents;

        // Hold the Current Pointer in the shapeFileContents byte Array;
        private int currentIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeFileStream"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public ShapeFileStream(string fileName)
        {
            this.fileName = fileName;
            this.dbFileName = fileName.ToLower().Replace(".shp", ".dbf");
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        public override bool CanWrite
        {
            get { return false; }
        }

        #region Implement this later
        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <value></value>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <value></value>
        /// <returns>The current position within the stream.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
        #endregion

        /// <summary>
        ///  This method reads the ShapeFiledata toward end direction
        /// </summary>
        /// <returns>
        /// ShapeFileData
        /// </returns>
        public ShapeFileData ReadToEnd()
        {
            ShapeFileData fileData;
            using (var fs = new FileStream(this.fileName, FileMode.Open, FileAccess.Read))
            {
                fileData =this.ReadFromStream(fs);
            }

            return fileData;
        }

        /// <summary>
        /// Convert given stream as byte Array
        /// </summary>
        /// <param name="input"></param>
        /// <returns>byte Array</returns>        
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

        /// <summary>
        ///  This Method reads the Stream and convert and return  the stream as
        /// ShapeFileData
        /// </summary>
        /// <param name="fs">Stream</param>
        /// <returns>
        /// ShapeFileData
        /// </returns>
        public ShapeFileData ReadFromStream(Stream fs)
        {
          
            this.shapeFileContents = ConvertBytes(fs);
            var fileData = new ShapeFileData(this.fileName);
            // parse header
            var header = this.ReadHeader(fs);
            fileData.FileHeader = header;
            this.ReadRecords(fileData, fs);           
            return fileData;
        }

        private void ReadRecords(ShapeFileData fileData, Stream fs)
        {
            // parse records            
            while (this.currentIndex < this.shapeFileContents.Length)
            {
                var record = this.ReadRecordLine(fs);
                fileData.Records.Add(record);
            }
            fs.Close();
            Array.Clear(this.shapeFileContents, 0, this.shapeFileContents.Length);
#if WPF
            // merge from DBF file
            this.ReadAttributes(this.dbFileName, fileData);
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
            if (fileCode != ShapeFileStream.ExpectedFileCode)
            {
                string msg = String.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid FileCode encountered. Expecting {0}.", ShapeFileStream.ExpectedFileCode);
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

        /// <summary>
        ///  This Method reads the Records line by line and retured as ShapeFileRecoed
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>
        /// ShapeFileRecord
        /// </returns>
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
            var pt = new Point();
            pt.X = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            pt.Y = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            record.Points.Add(pt);
            record.MinX = record.MaxX = pt.X;
            record.MinY = record.MaxY = pt.Y;
        }

#if WPF
        private void ReadAttributes(string dbaseFile, ShapeFileData fileData)
        {
            if (string.IsNullOrEmpty(dbaseFile))
            {
                throw new ArgumentNullException("dbaseFile");
            }

            // Check if the file exists. If it doesn't exist, this is not an error.
            if (!File.Exists(dbaseFile))
            {
                return;
            }

            // Get the directory in which the dBASE (DBF) file resides.
            FileInfo fi = new FileInfo(dbaseFile);
            string directory = fi.DirectoryName;

            // Get the filename minus the extension.
            string fileNameWithoutExtension = fi.Name.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            if (fileNameWithoutExtension.EndsWith(".DBF"))
            {
                fileNameWithoutExtension = fileNameWithoutExtension.Substring(0, fileNameWithoutExtension.Length - 4);
            }

            // Convert to a short filename (may not work in every case!).
            if (fileNameWithoutExtension.Length > 8)
            {
                if (fileNameWithoutExtension.Contains(" "))
                {
                    string noSpaces = fileNameWithoutExtension.Replace(" ", " ");
                    if (noSpaces.Length > 8)
                    {
                        fileNameWithoutExtension = noSpaces;
                    }
                }

                fileNameWithoutExtension = fileNameWithoutExtension.Substring(0, 6) + "~1";
            }

            // connection string.
            string connectionString = "PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=" + directory + ";Extended Properties=dBASE 5.0";
            string selectQuery = "SELECT * FROM [" + fileNameWithoutExtension + "#DBF];";

            // Create a database connection object using the connection string.
            OleDbConnection connection = new OleDbConnection(connectionString);
            OleDbCommand command = new OleDbCommand(selectQuery, connection);

            try
            {
                connection.Open();
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter();
                dataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dataSet.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(dataSet);

                // Merge attributes into the shape file.
                if (dataSet.Tables.Count > 0)
                {
                    this.MergeAttributes(dataSet.Tables[0], fileData);
                }
            }
            catch (OleDbException)
            {
                // Note: An exception will occur if the filename of the dBASE
                // file does not follow 8.3 naming conventions.
                // Rethrow the exception.
                throw;
            }
            finally
            {
                // Dispose of connection.
                ((IDisposable)connection).Dispose();
            }
        }

        private void MergeAttributes(DataTable dataTable, ShapeFileData fileData)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (i < fileData.Records.Count)
                {
                    var record = fileData.Records[i];
                    record.Attributes.Clear();
                    var row = dataTable.Rows[i];
                    for (int j = 0; j < row.ItemArray.Length; j++)
                    {
                        var col = dataTable.Columns[j];
                        var rowItem = row[col];
                        record.Attributes.Add(col.ColumnName, rowItem != null ? rowItem : String.Empty);
                    }
                }
            }
        }
#endif

        /// <summary>
        ///  Destructor of ShapeFilestream
        /// </summary>
        ~ShapeFileStream()
        {

        }

        #region Stream Helpers

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
                record.Points.Add(new Point(BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0), BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0)));
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
                var pt = new Point();
                pt.X = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
                pt.Y = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
                record.Points.Add(pt);

            }
        }

        /// <summary>
        ///  This method gets the ShapeType from its ShapeSream
        /// </summary>
        /// <param name="shapeType">Int</param>
        /// <returns>
        /// ShapeType
        /// </returns>
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
