#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.io
{
    public struct EndianType_Fields
    {
        public const int BIG_ENDIAN = 0;
        public const int LITTLE_ENDIAN = 1;
    }  
    internal interface BinaryDataOutput
    {
        int ByteOrdering
        {
            get;
        }
        void writeByte(int v);
        void writeShort(int v);
        void writeInt(int v);
        void writeLong(long v);
        void writeFloat(float v);
        void writeDouble(double v);
        void flush();
    }
   
    internal interface BinaryDataInput
    {
        int ByteOrdering
        {
            get;
        }
        byte readByte();
        byte readUnsignedByte();
        short readShort();
        int readUnsignedShort();
        int readInt();
        long readUnsignedInt();
        long readLong();
        float readFloat();
        double readDouble();
        int skipBytes(int n);
    }
}