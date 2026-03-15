#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Browser;
using System.Reflection;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for chartPngEncoder
    /// </summary>
    public class ChartPngEncoder
    {
        private const int _ADLER32_BASE = 65521;
        private const int _MAXBLOCK = 0xFFFF;
        private static byte[] _HEADER = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private static byte[] _IHDR = { (byte)'I', (byte)'H', (byte)'D', (byte)'R' };
        private static byte[] _GAMA = { (byte)'g', (byte)'A', (byte)'M', (byte)'A' };
        private static byte[] _IDAT = { (byte)'I', (byte)'D', (byte)'A', (byte)'T' };
        private static byte[] _IEND = { (byte)'I', (byte)'E', (byte)'N', (byte)'D' };
        private static byte[] _4BYTEDATA = { 0, 0, 0, 0 };
        private static byte[] _ARGB = { 0, 0, 0, 0, 0, 0, 0, 0, 8, 6, 0, 0, 0 };


        /// <summary>
        /// Return Stream values from the given values 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Stream Encode(byte[] data, int width, int height)
        {
            MemoryStream memorystream = new MemoryStream();
            byte[] size;
            memorystream.Write(_HEADER, 0, _HEADER.Length);

            size = BitConverter.GetBytes(width);
            _ARGB[0] = size[3]; _ARGB[1] = size[2]; _ARGB[2] = size[1]; _ARGB[3] = size[0];

            size = BitConverter.GetBytes(height);
            _ARGB[4] = size[3]; _ARGB[5] = size[2]; _ARGB[6] = size[1]; _ARGB[7] = size[0];

            // Write IHDR chunk
            WriteImage(memorystream, _IHDR, _ARGB);

            // Set gamma = 1
            size = BitConverter.GetBytes(1 * 100000);
            _4BYTEDATA[0] = size[3]; _4BYTEDATA[1] = size[2]; _4BYTEDATA[2] = size[1]; _4BYTEDATA[3] = size[0];

            // Write gAMA chunk
            WriteImage(memorystream, _GAMA, _4BYTEDATA);

            // Write IDAT chunk
            uint widthLength = (uint)(width * 4) + 1;
            uint dcSize = widthLength * (uint)height;

            uint adler = ComputeAdler32(data);
            MemoryStream comp = new MemoryStream();

            // Calculate number of 64K blocks
            uint rowsPerBlock = _MAXBLOCK / widthLength;
            uint blockSize = rowsPerBlock * widthLength;
            uint blockCount;
            ushort length;
            uint remainder = dcSize;

            if ((dcSize % blockSize) == 0)
            {
                blockCount = dcSize / blockSize;
            }
            else
            {
                blockCount = (dcSize / blockSize) + 1;
            }

            // Write headers
            comp.WriteByte(0x78);
            comp.WriteByte(0xDA);

            for (uint blocks = 0; blocks < blockCount; blocks++)
            {
                // Write LEN
                length = (ushort)((remainder < blockSize) ? remainder : blockSize);

                if (length == remainder)
                {
                    comp.WriteByte(0x01);
                }
                else
                {
                    comp.WriteByte(0x00);
                }

                comp.Write(BitConverter.GetBytes(length), 0, 2);

                // Write one's compliment of LEN
                comp.Write(BitConverter.GetBytes((ushort)~length), 0, 2);

                // Write blocks
                comp.Write(data, (int)(blocks * blockSize), length);

                // Next block
                remainder -= blockSize;
            }

            WriteReversedBuffer(comp, BitConverter.GetBytes(adler));
            comp.Seek(0, SeekOrigin.Begin);

            byte[] dat = new byte[comp.Length];
            comp.Read(dat, 0, (int)comp.Length);

            WriteImage(memorystream, _IDAT, dat);

            // Write IEND chunk
            WriteImage(memorystream, _IEND, new byte[0]);

            // Reset stream
            memorystream.Seek(0, SeekOrigin.Begin);

            return memorystream;

           
        }

        private static void WriteReversedBuffer(Stream stream, byte[] data)
        {
            int size = data.Length;
            byte[] reorder = new byte[size];

            for (int idx = 0; idx < size; idx++)
            {
                reorder[idx] = data[size - idx - 1];
            }
            stream.Write(reorder, 0, size);
        }

        private static void WriteImage(Stream stream, byte[] type, byte[] data)
        {
            int idx;
            int size = type.Length;
            byte[] buffer = new byte[type.Length + data.Length];

            // Initialize buffer
            for (idx = 0; idx < type.Length; idx++)
            {
                buffer[idx] = type[idx];
            }

            for (idx = 0; idx < data.Length; idx++)
            {
                buffer[idx + size] = data[idx];
            }

            // Write length
            WriteReversedBuffer(stream, BitConverter.GetBytes(data.Length));

            // Write type and data
            stream.Write(buffer, 0, buffer.Length);   // Should always be 4 bytes

            // Compute and write the CRC
            WriteReversedBuffer(stream, BitConverter.GetBytes(GetCRC(buffer)));
        }

        private static uint[] _crcTable = new uint[256];
        private static bool _crcTableComputed = false;

        private static void MakeCRCTable()
        {
            uint c;

            for (int n = 0; n < 256; n++)
            {
                c = (uint)n;
                for (int k = 0; k < 8; k++)
                {
                    if ((c & (0x00000001)) > 0)
                        c = 0xEDB88320 ^ (c >> 1);
                    else
                        c = c >> 1;
                }
                _crcTable[n] = c;
            }

            _crcTableComputed = true;
        }

        private static uint UpdateCRC(uint crc, byte[] buf, int len)
        {
            uint c = crc;

            if (!_crcTableComputed)
            {
                MakeCRCTable();
            }

            for (int n = 0; n < len; n++)
            {
                c = _crcTable[(c ^ buf[n]) & 0xFF] ^ (c >> 8);
            }

            return c;
        }

        /* Return the CRC of the bytes buf[0..len-1]. */
        private static uint GetCRC(byte[] buf)
        {
            return UpdateCRC(0xFFFFFFFF, buf, buf.Length) ^ 0xFFFFFFFF;
        }

        private static uint ComputeAdler32(byte[] buf)
        {
            uint s1 = 1;
            uint s2 = 0;
            int length = buf.Length;

            for (int idx = 0; idx < length; idx++)
            {
                s1 = (s1 + (uint)buf[idx]) % _ADLER32_BASE;
                s2 = (s2 + s1) % _ADLER32_BASE;
            }

            return (s2 << 16) + s1;
        }
    }
}
