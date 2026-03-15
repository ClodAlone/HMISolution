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
namespace Syncfusion.Pdf.Util
{
    internal class EndianBinaryReader : BinaryReader
    {
        private bool _bigEndian = false;
        public EndianBinaryReader(Stream input)
            : base(input)
        {
        }
        public EndianBinaryReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {
        }
        public EndianBinaryReader(Stream input, Encoding encoding, bool bigEndian)
            : base(input, encoding)
        {
            _bigEndian = bigEndian;
        }
        public EndianBinaryReader(Stream input, bool bigEndian)
            : base(input, bigEndian ? Encoding.BigEndianUnicode : Encoding.UTF8)
        {
            _bigEndian = bigEndian;
        }
        public override short ReadInt16()
        {
            if (_bigEndian)
            {
                byte[] buf = this.ReadBytes(2);
                Array.Reverse(buf);
                return BitConverter.ToInt16(buf, 0);
            }
            else
                return base.ReadInt16();
        }
        public override int ReadInt32()
        {
            if (_bigEndian)
            {
                byte[] buf = this.ReadBytes(4);
                Array.Reverse(buf);
                return BitConverter.ToInt32(buf, 0);
            }
            else
                return base.ReadInt32();
        }
        public override long ReadInt64()
        {
            if (_bigEndian)
            {
                byte[] buf = this.ReadBytes(8);
                Array.Reverse(buf);
                return BitConverter.ToInt64(buf, 0);
            }
            else
                return base.ReadInt64();
        }
        public override float ReadSingle()
        {
            if (_bigEndian)
            {
                byte[] buf = this.ReadBytes(4);
                Array.Reverse(buf);
                return BitConverter.ToSingle(buf, 0);
            }
            else
                return base.ReadSingle();
        }
        public override ushort ReadUInt16()
        {
            if (_bigEndian)
            {
                byte[] buf = this.ReadBytes(2);
                Array.Reverse(buf);
                return BitConverter.ToUInt16(buf, 0);
            }
            else
                return base.ReadUInt16();
        }
        public override uint ReadUInt32()
        {
            if (_bigEndian)
            {
                byte[] buf = this.ReadBytes(4);
                Array.Reverse(buf);
                return BitConverter.ToUInt32(buf, 0);
            }
            else
                return base.ReadUInt32();
        }
        public override ulong ReadUInt64()
        {
            if (_bigEndian)
            {
                byte[] buf = this.ReadBytes(8);
                Array.Reverse(buf);
                return BitConverter.ToUInt64(buf, 0);
            }
            else
                return base.ReadUInt64();
        }
    }
}
