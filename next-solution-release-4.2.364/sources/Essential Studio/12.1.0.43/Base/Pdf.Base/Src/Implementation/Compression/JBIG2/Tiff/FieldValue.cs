#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Pdf.Compression.JBIG2
{
    /// <summary>
    /// Field bits (flags) for tags.
    /// </summary>
    static class FieldBit
    {
        internal const int SetLongs = 4;

        internal const short ImageDimensions = 1;
        internal const short TileDimensions = 2;
        internal const short Resolution = 3;
        internal const short Position = 4;

        internal const short SubFileType = 5;
        internal const short BitsPerSample = 6;
        internal const short Compression = 7;
        internal const short Photometric = 8;
        internal const short Thresholding = 9;
        internal const short FillOrder = 10;
        internal const short Orientation = 15;
        internal const short SamplesPerPixel = 16;
        internal const short RowsPerStrip = 17;
        internal const short MinSampleValue = 18;
        internal const short MaxSampleValue = 19;
        internal const short PlaneConfig = 20;
        internal const short ResolutionUnit = 22;
        internal const short PageNumber = 23;
        internal const short StripByteCounts = 24;
        internal const short StripOffsets = 25;
        internal const short ColorMap = 26;
        internal const short ExtraSamples = 31;
        internal const short SampleFormat = 32;
        internal const short SMinSampleValue = 33;
        internal const short SMaxSampleValue = 34;
        internal const short ImageDepth = 35;
        internal const short TileDepth = 36;
        internal const short HalftoneHints = 37;
        internal const short YCbCrSubsampling = 39;
        internal const short YCbCrPositioning = 40;
        internal const short RefBlackWhite = 41;
        internal const short TransferFunction = 44;
        internal const short InkNames = 46;
        internal const short SubIFD = 49;

        public const short Ignore = 0;
        public const short Pseudo = 0;

        /// <summary>
        /// This value is used to signify custom tags.
        /// </summary>
        public const short Custom = 65;

        /// <summary>
        /// This value is used as a base (starting) value for codec-private tags.
        /// </summary>
        public const short Codec = 66;

        /// <summary>
        /// Last usable value for field bit. All tags values should be less than this value.
        /// </summary>
        public const short Last = (32 * SetLongs - 1);
    }

    /// <summary>
    /// Holds a value of a Tiff tag.
    /// </summary>
    struct FieldValue
    {
        private object m_value;

        internal FieldValue(object o)
        {
            m_value = o;
        }

        static internal FieldValue[] FromParams(params object[] list)
        {
            FieldValue[] values = new FieldValue[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] is FieldValue)
                    values[i] = new FieldValue(((FieldValue)(list[i])).Value);
                else
                    values[i] = new FieldValue(list[i]);
            }

            return values;
        }

        internal void Set(object o)
        {
            m_value = o;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return m_value; }
        }

        /// <summary>
        /// Retrieves value converted to byte.
        /// </summary>
        public byte ToByte()
        {
            return Convert.ToByte(m_value);
        }

        /// <summary>
        /// Retrieves value converted to short.
        /// </summary>
        public short ToShort()
        {
            switch (Type.GetTypeCode(m_value.GetType()))
            {
                case TypeCode.UInt16:
                    return (short)((ushort)m_value);

                case TypeCode.Int16:
                    return (short)m_value;
            }

            return Convert.ToInt16(m_value);
        }

        /// <summary>
        /// Retrieves value converted to ushort.
        /// </summary>
        public ushort ToUShort()
        {
            switch (Type.GetTypeCode(m_value.GetType()))
            {
                case TypeCode.UInt16:
                    return (ushort)m_value;

                case TypeCode.Int16:
                    return (ushort)((short)m_value);
            }

            return Convert.ToUInt16(m_value);
        }

        /// <summary>
        /// Retrieves value converted to int.
        /// </summary>
        public int ToInt()
        {
            switch (Type.GetTypeCode(m_value.GetType()))
            {
                case TypeCode.UInt32:
                    return (int)((uint)m_value);

                case TypeCode.Int32:
                    return (int)m_value;
            }

            return Convert.ToInt32(m_value);
        }

        /// <summary>
        /// Retrieves value converted to uint.
        /// </summary>
        public uint ToUInt()
        {
            switch (Type.GetTypeCode(m_value.GetType()))
            {
                case TypeCode.UInt32:
                    return (uint)m_value;

                case TypeCode.Int32:
                    return (uint)((int)m_value);
            }

            return Convert.ToUInt32(m_value);
        }

        /// <summary>
        /// Retrieves value converted to float.
        /// </summary>
        public float ToFloat()
        {
            return Convert.ToSingle(m_value);
        }

        /// <summary>
        /// Retrieves value converted to double.
        /// </summary>
        public double ToDouble()
        {
            return Convert.ToDouble(m_value);
        }

        /// <summary>
        /// Retrieves value converted to string.
        /// </summary>
        public override string ToString()
        {
            if (m_value is byte[])
                return Tiff.Latin1Encoding.GetString(m_value as byte[]);

            return Convert.ToString(m_value);
        }

        /// <summary>
        /// Retrieves value converted to byte array.
        /// </summary>
        public byte[] GetBytes()
        {
            if (m_value == null)
                return null;

            Type t = m_value.GetType();
            if (t.IsArray)
            {
                if (m_value is byte[])
                    return m_value as byte[];
                else if (m_value is short[])
                {
                    short[] temp = m_value as short[];
                    byte[] result = new byte[temp.Length * sizeof(short)];
                    Buffer.BlockCopy(temp, 0, result, 0, result.Length);
                    return result;
                }
                else if (m_value is ushort[])
                {
                    ushort[] temp = m_value as ushort[];
                    byte[] result = new byte[temp.Length * sizeof(ushort)];
                    Buffer.BlockCopy(temp, 0, result, 0, result.Length);
                    return result;
                }
                else if (m_value is int[])
                {
                    int[] temp = m_value as int[];
                    byte[] result = new byte[temp.Length * sizeof(int)];
                    Buffer.BlockCopy(temp, 0, result, 0, result.Length);
                    return result;
                }
                else if (m_value is uint[])
                {
                    uint[] temp = m_value as uint[];
                    byte[] result = new byte[temp.Length * sizeof(uint)];
                    Buffer.BlockCopy(temp, 0, result, 0, result.Length);
                    return result;
                }
                else if (m_value is float[])
                {
                    float[] temp = m_value as float[];
                    byte[] result = new byte[temp.Length * sizeof(float)];
                    Buffer.BlockCopy(temp, 0, result, 0, result.Length);
                    return result;
                }
                else if (m_value is double[])
                {
                    double[] temp = m_value as double[];
                    byte[] result = new byte[temp.Length * sizeof(double)];
                    Buffer.BlockCopy(temp, 0, result, 0, result.Length);
                    return result;
                }
            }
            else if (m_value is string)
            {
                return Tiff.Latin1Encoding.GetBytes(m_value as string);
            }
            else if (m_value is int)
            {
                return BitConverter.GetBytes((int)m_value);
            }

            return null;
        }

        /// <summary>
        /// Retrieves value converted to array of bytes.
        /// </summary>
        public byte[] ToByteArray()
        {
            if (m_value == null)
                return null;

            Type t = m_value.GetType();
            if (t.IsArray)
            {
                if (m_value is byte[])
                    return m_value as byte[];
                else if (m_value is short[])
                {
                    short[] temp = m_value as short[];
                    byte[] result = new byte[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (byte)temp[i];

                    return result;
                }
                else if (m_value is ushort[])
                {
                    ushort[] temp = m_value as ushort[];
                    byte[] result = new byte[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (byte)temp[i];

                    return result;
                }
                else if (m_value is int[])
                {
                    int[] temp = m_value as int[];
                    byte[] result = new byte[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (byte)temp[i];

                    return result;
                }
                else if (m_value is uint[])
                {
                    uint[] temp = m_value as uint[];
                    byte[] result = new byte[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (byte)temp[i];

                    return result;
                }
            }
            else if (m_value is string)
                return Tiff.Latin1Encoding.GetBytes(m_value as string);

            return null;
        }

        /// <summary>
        /// Retrieves value converted to array of short values.
        /// </summary>
        public short[] ToShortArray()
        {
            if (m_value == null)
                return null;

            Type t = m_value.GetType();
            if (t.IsArray)
            {
                if (m_value is short[])
                    return m_value as short[];
                else if (m_value is byte[])
                {
                    byte[] temp = m_value as byte[];
                    if (temp.Length % sizeof(short) != 0)
                        return null;

                    int totalShorts = temp.Length / sizeof(short);
                    short[] result = new short[totalShorts];

                    int byteOffset = 0;
                    for (int i = 0; i < totalShorts; i++)
                    {
                        short s = BitConverter.ToInt16(temp, byteOffset);
                        result[i] = s;
                        byteOffset += sizeof(short);
                    }

                    return result;
                }
                else if (m_value is ushort[])
                {
                    ushort[] temp = m_value as ushort[];
                    short[] result = new short[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (short)temp[i];

                    return result;
                }
                else if (m_value is int[])
                {
                    int[] temp = m_value as int[];
                    short[] result = new short[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (short)temp[i];

                    return result;
                }
                else if (m_value is uint[])
                {
                    uint[] temp = m_value as uint[];
                    short[] result = new short[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (short)temp[i];

                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves value converted to array of ushort values.
        /// </summary>
        public ushort[] ToUShortArray()
        {
            if (m_value == null)
                return null;

            Type t = m_value.GetType();
            if (t.IsArray)
            {
                if (m_value is ushort[])
                    return m_value as ushort[];
                else if (m_value is byte[])
                {
                    byte[] temp = m_value as byte[];
                    if (temp.Length % sizeof(ushort) != 0)
                        return null;

                    int totalUShorts = temp.Length / sizeof(ushort);
                    ushort[] result = new ushort[totalUShorts];

                    int byteOffset = 0;
                    for (int i = 0; i < totalUShorts; i++)
                    {
                        ushort s = BitConverter.ToUInt16(temp, byteOffset);
                        result[i] = s;
                        byteOffset += sizeof(ushort);
                    }

                    return result;
                }
                else if (m_value is short[])
                {
                    short[] temp = m_value as short[];
                    ushort[] result = new ushort[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (ushort)temp[i];

                    return result;
                }
                else if (m_value is int[])
                {
                    int[] temp = m_value as int[];
                    ushort[] result = new ushort[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (ushort)temp[i];

                    return result;
                }
                else if (m_value is uint[])
                {
                    uint[] temp = m_value as uint[];
                    ushort[] result = new ushort[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (ushort)temp[i];

                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves value converted to array of int values.
        /// </summary>
        public int[] ToIntArray()
        {
            if (m_value == null)
                return null;

            Type t = m_value.GetType();
            if (t.IsArray)
            {
                if (m_value is int[])
                    return m_value as int[];
                else if (m_value is byte[])
                {
                    byte[] temp = m_value as byte[];
                    if (temp.Length % sizeof(int) != 0)
                        return null;

                    int totalInts = temp.Length / sizeof(int);
                    int[] result = new int[totalInts];

                    int byteOffset = 0;
                    for (int i = 0; i < totalInts; i++)
                    {
                        int s = BitConverter.ToInt32(temp, byteOffset);
                        result[i] = s;
                        byteOffset += sizeof(int);
                    }

                    return result;
                }
                else if (m_value is short[])
                {
                    short[] temp = m_value as short[];
                    int[] result = new int[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (int)temp[i];

                    return result;
                }
                else if (m_value is ushort[])
                {
                    ushort[] temp = m_value as ushort[];
                    int[] result = new int[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (int)temp[i];

                    return result;
                }
                else if (m_value is uint[])
                {
                    uint[] temp = m_value as uint[];
                    int[] result = new int[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (int)temp[i];

                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves value converted to array of uint values.
        /// </summary>
        public uint[] ToUIntArray()
        {
            if (m_value == null)
                return null;

            Type t = m_value.GetType();
            if (t.IsArray)
            {
                if (m_value is uint[])
                    return m_value as uint[];
                else if (m_value is byte[])
                {
                    byte[] temp = m_value as byte[];
                    if (temp.Length % sizeof(uint) != 0)
                        return null;

                    int totalUInts = temp.Length / sizeof(uint);
                    uint[] result = new uint[totalUInts];

                    int byteOffset = 0;
                    for (int i = 0; i < totalUInts; i++)
                    {
                        uint s = BitConverter.ToUInt32(temp, byteOffset);
                        result[i] = s;
                        byteOffset += sizeof(uint);
                    }

                    return result;
                }
                else if (m_value is short[])
                {
                    short[] temp = m_value as short[];
                    uint[] result = new uint[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (uint)temp[i];

                    return result;
                }
                else if (m_value is ushort[])
                {
                    ushort[] temp = m_value as ushort[];
                    uint[] result = new uint[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (uint)temp[i];

                    return result;
                }
                else if (m_value is int[])
                {
                    int[] temp = m_value as int[];
                    uint[] result = new uint[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (uint)temp[i];

                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves value converted to array of float values.
        /// </summary>
        public float[] ToFloatArray()
        {
            if (m_value == null)
                return null;

            Type t = m_value.GetType();
            if (t.IsArray)
            {
                if (m_value is float[])
                    return m_value as float[];
                else if (m_value is double[])
                {
                    double[] temp = m_value as double[];
                    float[] result = new float[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (float)temp[i];

                    return result;
                }
                else if (m_value is byte[])
                {
                    byte[] temp = m_value as byte[];
                    if (temp.Length % sizeof(float) != 0)
                        return null;

                    int tempPos = 0;

                    int floatCount = temp.Length / sizeof(float);
                    float[] result = new float[floatCount];

                    for (int i = 0; i < floatCount; i++)
                    {
                        float f = BitConverter.ToSingle(temp, tempPos);
                        result[i] = f;
                        tempPos += sizeof(float);
                    }

                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves value converted to array of double values.
        /// </summary>
        public double[] ToDoubleArray()
        {
            if (m_value == null)
                return null;

            Type t = m_value.GetType();
            if (t.IsArray)
            {
                if (m_value is double[])
                    return m_value as double[];
                else if (m_value is float[])
                {
                    float[] temp = m_value as float[];
                    double[] result = new double[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                        result[i] = (double)temp[i];

                    return result;
                }
                else if (m_value is byte[])
                {
                    byte[] temp = m_value as byte[];
                    if (temp.Length % sizeof(double) != 0)
                        return null;

                    int tempPos = 0;

                    int floatCount = temp.Length / sizeof(double);
                    double[] result = new double[floatCount];

                    for (int i = 0; i < floatCount; i++)
                    {
                        double d = BitConverter.ToDouble(temp, tempPos);
                        result[i] = d;
                        tempPos += sizeof(double);
                    }

                    return result;
                }
            }

            return null;
        }
    }
}
