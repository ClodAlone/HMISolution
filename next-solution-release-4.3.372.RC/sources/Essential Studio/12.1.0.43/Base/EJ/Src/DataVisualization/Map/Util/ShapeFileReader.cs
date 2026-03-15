#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Syncfusion.JavaScript.DataVisualization.Maps
{
    public enum ShapeType
    {
        /// <summary>
        /// Nullshape / placeholder record.
        /// </summary>
        NullShape = 0,

        /// <summary>
        /// Point record, for defining point locations such as a city.
        /// </summary>
        Point = 1,

        /// <summary>
        /// One or more sets of connected points. Used to represent roads,
        /// hydrography, etc.
        /// </summary>
        PolyLine = 3,

        /// <summary>
        /// One or more sets of closed figures. Used to represent political
        /// boundaries for countries, lakes, etc.
        /// </summary>
        Polygon = 5,

        /// <summary>
        /// A cluster of points represented by a single shape record.
        /// </summary>
        Multipoint = 8

        // Unsupported types:
        // PointZ = 11,        
        // PolyLineZ = 13,        
        // PolygonZ = 15,        
        // MultiPointZ = 18,        
        // PointM = 21,        
        // PolyLineM = 23,        
        // PolygonM = 25,        
        // MultiPointM = 28,        
        // MultiPatch = 31
    }

    internal class ShapeFileReader
    {
        private byte[] shapeFileContents;
        internal ShapeFileHeader Header = new ShapeFileHeader();
        private int currentIndex = 0;
        internal List<ShapeFileRecord> Records = new List<ShapeFileRecord>();
        
        public ShapeFileReader()
        {
        }

        public void ReadShapeFile(string shapefilename)
        {
            Stream shapeStream = null;
            if (File.Exists(shapefilename))
            {
                shapeStream = File.OpenRead(shapefilename);
            }
            if (shapeStream != null)
            {
                this.ReadFromStream(shapeStream);
            }
        }

        internal void ReadFromStream(Stream fs)
        {
            this.shapeFileContents = ConvertBytes(fs);
            var header = this.ReadHeader(fs);
            this.Header = header;
            this.ReadRecords(fs);
        }

        private ShapeFileHeader ReadHeader(Stream stream)
        {
            var fileHeader = new ShapeFileHeader();
            var fileCode = BitConverter.ToInt32(Helpers.SplitReverse(this.shapeFileContents, this.currentIndex, (this.currentIndex += 4)), 0);


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

        private ShapeFileRecord ReadRecordLine(Stream stream)
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
            var pt = new ShapePoint();
            pt.X = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            pt.Y = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
            record.Points.Add(pt);
            record.MinX = record.MaxX = pt.X;
            record.MinY = record.MaxY = pt.Y;
        }

        private static ShapeType GetShapeTypeFromShapeStream(int shapeType)
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
                record.Points.Add(new ShapePoint(BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0), BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0)));
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
                var pt = new ShapePoint();
                pt.X = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
                pt.Y = BitConverter.ToDouble(Helpers.Split(this.shapeFileContents, this.currentIndex, (this.currentIndex += 8)), 0);
                record.Points.Add(pt);

            }
        }

        private void ReadRecords(Stream fs)
        {
            // parse records 
            this.Records.Clear();
            while (this.currentIndex < this.shapeFileContents.Length)
            {
                var record = this.ReadRecordLine(fs);
                this.Records.Add(record);
            }
            fs.Dispose();
            Array.Clear(this.shapeFileContents, 0, this.shapeFileContents.Length);

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
    }
}
