#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;

namespace Syncfusion.Pdf.Graphics.Images.Decoder
{
    public static class StreamExtensions
    {
        #region Fields
        private static byte[] m_jpegSignature = { 255, 216 };
        private static byte[] m_pngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };
        private static byte[] m_bmpSignature = { 66, 77 };
        #endregion

        #region Helper Methods
        public static int ReadUInt32(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);

            return ((buffer[0] << 24) + (buffer[1] << 16) + (buffer[2] << 8) + buffer[3]);
        }

        public static Int32 ReadInt32(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);

            return (buffer[0] + (buffer[1] << 8) + (buffer[2] << 16) + (buffer[3] << 24));
        }

        public static int ReadUInt16(this Stream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);

            return (buffer[0] << 8) + buffer[1];
        }

        public static int ReadInt16(this Stream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);

            return buffer[0] | (buffer[1] << 8);
        }

        public static int ReadWord(this Stream stream)
        {
            int num = stream.ReadByte();
            return (num + (stream.ReadByte() << 8)) & 0xffff;
        }

        public static int ReadShortLE(this Stream stream)
        {
            int num = stream.ReadWord();
            if (num > 0x7fff)
                num -= 0x10000;
            return num;
        }

        public static string ReadString(this Stream stream, int len)
        {
            char[] chars = new char[len];

            for (int i = 0; i < len; i++)
            {
                chars[i] = (char)stream.ReadByte();
            }

            return new string(chars);
        }

        public static void Reset(this Stream stream)
        {
            stream.Position = 0;
        }

        public static void Skip(this Stream stream, int noOfBytes)
        {
            byte[] temp = new byte[noOfBytes];
            stream.Read(temp, 0, temp.Length);
        }

        public static int ReadByte(this Stream stream)
        {
            return stream.ReadByte();
        }

        public static bool IsJpeg(this Stream stream)
        {
            stream.Reset();            

            for (int i = 0; i < m_jpegSignature.Length; i++)
            {
                if (m_jpegSignature[i] != stream.ReadByte())
                    return false;
            }
            return true;
        }

        public static bool IsBmp(this Stream stream)
        {
            stream.Reset();

            for (int i = 0; i < m_bmpSignature.Length; i++)
            {
                if (m_bmpSignature[i] != stream.ReadByte())
                    return false;
            }
            return true;
        }


        public static bool IsPng(this Stream stream)
        {
            stream.Reset();

            for (int i = 0; i < m_pngSignature.Length; i++)
            {
                if (m_pngSignature[i] != stream.ReadByte())
                    return false;
            }

            return true;
        }
        #endregion
    }
}
