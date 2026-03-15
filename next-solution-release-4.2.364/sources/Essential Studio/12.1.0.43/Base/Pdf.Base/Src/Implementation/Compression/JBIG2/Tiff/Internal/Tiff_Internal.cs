#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Text;

using Syncfusion.Pdf.Compression.JBIG2.Internal;
using System.Globalization;

namespace Syncfusion.Pdf.Compression.JBIG2
{
    partial class Tiff
    {
        private const int TIFF_VERSION = 42;
        private const int TIFF_BIGTIFF_VERSION = 43;

        private const short TIFF_BIGENDIAN = 0x4d4d;
        private const short TIFF_LITTLEENDIAN = 0x4949;
        private const short MDI_LITTLEENDIAN = 0x5045;

        // reference white
        private const float D50_X0 = 96.4250F;
        private const float D50_Y0 = 100.0F;
        private const float D50_Z0 = 82.4680F;

        internal const int STRIP_SIZE_DEFAULT = 8192;
        internal const TiffFlags STRIPCHOP_DEFAULT = TiffFlags.STRIPCHOP;
        internal const bool DEFAULT_EXTRASAMPLE_AS_ALPHA = true;
        internal const bool CHECK_JPEG_YCBCR_SUBSAMPLING = true;
        internal static readonly Encoding Latin1Encoding = Encoding.GetEncoding("Latin1");

        internal enum PostDecodeMethodType
        {
            pdmNone,
            pdmSwab16Bit,
            pdmSwab24Bit,
            pdmSwab32Bit,
            pdmSwab64Bit
        };

        /// <summary>
        /// name of open file
        /// </summary>
        internal string m_name;

        /// <summary>
        /// open mode (O_*)
        /// </summary>
        internal int m_mode;
        internal TiffFlags m_flags;

        /// <summary>
        /// file offset of current directory
        /// </summary>
        internal uint m_diroff;

        /// <summary>
        /// internal rep of current directory
        /// </summary>
        internal TiffDirectory m_dir;

        /// <summary>
        /// current scanline
        /// </summary>
        internal int m_row;

        /// <summary>
        /// current strip for read/write
        /// </summary>
        internal int m_curstrip;

        // tiling support

        /// <summary>
        /// current tile for read/write
        /// </summary>
        internal int m_curtile;

        /// <summary>
        /// # of bytes in a tile
        /// </summary>
        internal int m_tilesize;

        // compression scheme hooks
        internal TiffCodec m_currentCodec;

        // input/output buffering

        /// <summary>
        /// # of bytes in a scanline
        /// </summary>
        internal int m_scanlinesize;

        /// <summary>
        /// raw data buffer
        /// </summary>
        internal byte[] m_rawdata;

        /// <summary>
        /// # of bytes in raw data buffer
        /// </summary>
        internal int m_rawdatasize;

        /// <summary>
        /// current spot in raw buffer
        /// </summary>
        internal int m_rawcp;

        /// <summary>
        /// bytes unread from raw buffer
        /// </summary>
        internal int m_rawcc;

        /// <summary>
        /// callback parameter
        /// </summary>
        internal object m_clientdata;

        // post-decoding support

        /// <summary>
        /// post decoding method type
        /// </summary>
        internal PostDecodeMethodType m_postDecodeMethod;

        // tag support

        /// <summary>
        /// tag get/set/print routines
        /// </summary>
        internal TiffTagMethods m_tagmethods;

        private class codecList
        {
            public codecList next;
            public TiffCodec codec;
        };

        private class clientInfoLink
        {
            public clientInfoLink next;
            public object data;
            public string name;
        };

        // the first directory

        /// <summary>
        /// file offset of following directory
        /// </summary>
        private uint m_nextdiroff;

        /// <summary>
        /// list of offsets to already seen directories to prevent IFD looping
        /// </summary>
        private uint[] m_dirlist;

        /// <summary>
        /// number of entires in offset list
        /// </summary>
        private int m_dirlistsize;

        /// <summary>
        /// number of already seen directories
        /// </summary>
        private short m_dirnumber;

        /// <summary>
        /// file's header block
        /// </summary>
        private Syncfusion.Pdf.Compression.JBIG2.Internal.TiffHeader m_header;

        /// <summary>
        /// data type shift counts
        /// </summary>
        private int[] m_typeshift;

        /// <summary>
        /// data type masks
        /// </summary>
        private uint[] m_typemask;

        /// <summary>
        /// current directory (index)
        /// </summary>
        private short m_curdir;

        /// <summary>
        /// current offset for read/write
        /// </summary>
        private uint m_curoff;

        /// <summary>
        /// current offset for writing dir
        /// </summary>
        private uint m_dataoff;

        //
        // SubIFD support
        // 

        /// <summary>
        /// remaining subifds to write
        /// </summary>
        private short m_nsubifd;

        /// <summary>
        /// offset for patching SubIFD link
        /// </summary>
        private uint m_subifdoff;

        // tiling support

        /// <summary>
        /// current column (offset by row too)
        /// </summary>
        private int m_col;

        // compression scheme hooks

        private bool m_decodestatus;

        // tag support

        /// <summary>
        /// sorted table of registered tags
        /// </summary>
        private TiffFieldInfo[] m_fieldinfo;

        /// <summary>
        /// # entries in registered tag table
        /// </summary>
        private int m_nfields;

        /// <summary>
        /// cached pointer to already found tag
        /// </summary>
        private TiffFieldInfo m_foundfield;

        /// <summary>
        /// extra client information.
        /// </summary>
        private clientInfoLink m_clientinfo;

        private TiffCodec[] m_builtInCodecs;
        private codecList m_registeredCodecs;

        private TiffTagMethods m_defaultTagMethods;

        private bool m_disposed;
        private Stream m_fileStream;

        /// <summary>
        /// stream used for read|write|etc.
        /// </summary>
        private TiffStream m_stream;

        private Tiff()
        {
            m_clientdata = 0;
            m_postDecodeMethod = PostDecodeMethodType.pdmNone;

            setupBuiltInCodecs();

            m_defaultTagMethods = new TiffTagMethods();
        }

        private void Dispose(bool disposing)
        {
            if (!this.m_disposed)
            {
                if (disposing)
                {
                    Close();

                    if (m_fileStream != null)
                        m_fileStream.Dispose();
                }

                m_disposed = true;
            }
        }

        internal static void SwabUInt(ref uint lp)
        {
            byte[] cp = new byte[4];
            cp[0] = (byte)lp;
            cp[1] = (byte)(lp >> 8);
            cp[2] = (byte)(lp >> 16);
            cp[3] = (byte)(lp >> 24);

            byte t = cp[3];
            cp[3] = cp[0];
            cp[0] = t;

            t = cp[2];
            cp[2] = cp[1];
            cp[1] = t;

            lp = (uint)(cp[0] & 0xFF);
            lp += (uint)((cp[1] & 0xFF) << 8);
            lp += (uint)((cp[2] & 0xFF) << 16);
            lp += (uint)(cp[3] << 24);
        }

        internal static uint[] Realloc(uint[] buffer, int elementCount, int newElementCount)
        {
            uint[] newBuffer = new uint[newElementCount];
            if (buffer != null)
            {
                int copyLength = Math.Min(elementCount, newElementCount);
                Buffer.BlockCopy(buffer, 0, newBuffer, 0, copyLength * sizeof(uint));
            }

            return newBuffer;
        }

        internal static TiffFieldInfo[] Realloc(TiffFieldInfo[] buffer, int elementCount, int newElementCount)
        {
            TiffFieldInfo[] newBuffer = new TiffFieldInfo [newElementCount];

            if (buffer != null)
            {
                int copyLength = Math.Min(elementCount, newElementCount);
                Array.Copy(buffer, newBuffer, copyLength);
            }

            return newBuffer;
        }

        internal static TiffTagValue[] Realloc(TiffTagValue[] buffer, int elementCount, int newElementCount)
        {
            TiffTagValue[] newBuffer = new TiffTagValue[newElementCount];

            if (buffer != null)
            {
                int copyLength = Math.Min(elementCount, newElementCount);
                Array.Copy(buffer, newBuffer, copyLength);
            }

            return newBuffer;
        }

        internal bool setCompressionScheme(Compression scheme)
        {
            TiffCodec c = FindCodec(scheme);
            if (c == null)
            {
                c = m_builtInCodecs[0];
            }

            m_decodestatus = c.CanDecode;
            m_flags &= ~(TiffFlags.NOBITREV | TiffFlags.NOREADRAW);

            m_currentCodec = c;
            return c.Init();
        }

        /// <summary>
        /// post decoding routine
        /// </summary>
        private void postDecode(byte[] buffer, int offset, int count)
        {
            switch (m_postDecodeMethod)
            {
                case PostDecodeMethodType.pdmSwab16Bit:
                    swab16BitData(buffer, offset, count);
                    break;
                case PostDecodeMethodType.pdmSwab24Bit:
                    swab24BitData(buffer, offset, count);
                    break;
                case PostDecodeMethodType.pdmSwab32Bit:
                    swab32BitData(buffer, offset, count);
                    break;
                case PostDecodeMethodType.pdmSwab64Bit:
                    swab64BitData(buffer, offset, count);
                    break;
            }
        }

        private static bool defaultTransferFunction(TiffDirectory td)
        {
            short[][] tf = td.td_transferfunction;
            tf[0] = null;
            tf[1] = null;
            tf[2] = null;

            if (td.td_bitspersample >= sizeof(int) * 8 - 2)
                return false;

            int n = 1 << td.td_bitspersample;
            tf[0] = new short[n];
            tf[0][0] = 0;
            for (int i = 1; i < n; i++)
            {
                double t = (double)i / ((double)n - 1.0);
                tf[0][i] = (short)Math.Floor(65535.0 * Math.Pow(t, 2.2) + 0.5);
            }

            if (td.td_samplesperpixel - td.td_extrasamples > 1)
            {
                tf[1] = new short[n];
                Buffer.BlockCopy(tf[0], 0, tf[1], 0, tf[0].Length * sizeof(short));

                tf[2] = new short[n];
                Buffer.BlockCopy(tf[0], 0, tf[2], 0, tf[0].Length * sizeof(short));
            }

            return true;
        }

        private static void defaultRefBlackWhite(TiffDirectory td)
        {
            td.td_refblackwhite = new float[6];
            if (td.td_photometric == Photometric.YCBCR)
            {
                td.td_refblackwhite[0] = 0.0F;
                td.td_refblackwhite[1] = td.td_refblackwhite[3] = td.td_refblackwhite[5] = 255.0F;
                td.td_refblackwhite[2] = td.td_refblackwhite[4] = 128.0F;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    td.td_refblackwhite[2 * i + 0] = 0;
                    td.td_refblackwhite[2 * i + 1] = (float)((1L << td.td_bitspersample) - 1L);
                }
            }
        }

        internal static int readInt(byte[] buffer, int offset)
        {
            int value = buffer[offset++] & 0xFF;
            value += (buffer[offset++] & 0xFF) << 8;
            value += (buffer[offset++] & 0xFF) << 16;
            value += buffer[offset++] << 24;
            return value;
        }

        internal static void writeInt(int value, byte[] buffer, int offset)
        {
            buffer[offset++] = (byte)value;
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)(value >> 16);
            buffer[offset++] = (byte)(value >> 24);
        }

        internal static short readShort(byte[] buffer, int offset)
        {
            short value = (short)(buffer[offset] & 0xFF);
            value += (short)((buffer[offset + 1] & 0xFF) << 8);
            return value;
        }

        /// <summary>
        /// Compression schemes statically built into the library.
        /// </summary>
        private void setupBuiltInCodecs()
        {
            m_builtInCodecs = new TiffCodec[]
            {
                new TiffCodec(this, (Compression)(-1), "Not configured"),
                new DumpModeCodec(this, Compression.NONE, "None"),
                new LZWCodec(this, Compression.LZW, "LZW"),
                new PackBitsCodec(this, Compression.PACKBITS, "PackBits"),
                new TiffCodec(this, Compression.THUNDERSCAN, "ThunderScan"),
                new TiffCodec(this, Compression.NEXT, "NeXT"),
                new JpegCodec(this, Compression.JPEG, "JPEG"),
                new OJpegCodec(this, Compression.OJPEG, "Old-style JPEG"),
                new CCITTCodec(this, Compression.CCITTRLE, "CCITT RLE"),
                new CCITTCodec(this, Compression.CCITTRLEW, "CCITT RLE/W"),
                new CCITTCodec(this, Compression.CCITTFAX3, "CCITT Group 3"),
                new CCITTCodec(this, Compression.CCITTFAX4, "CCITT Group 4"),
                new TiffCodec(this, Compression.JBIG, "ISO JBIG"),
                new DeflateCodec(this, Compression.DEFLATE, "Deflate"),
                new DeflateCodec(this, Compression.ADOBE_DEFLATE, "AdobeDeflate"),
                new TiffCodec(this, Compression.PIXARLOG, "PixarLog"),
                new TiffCodec(this, Compression.SGILOG, "SGILog"),
                new TiffCodec(this, Compression.SGILOG24, "SGILog24"),
                null,
            };
        }

        internal static bool isPseudoTag(TiffTag t)
        {
            return ((int)t > 0xffff);
        }

        private bool isFillOrder(FillOrder o)
        {
            TiffFlags order = (TiffFlags)o;
            return ((m_flags & order) == order);
        }

        private static int BITn(int n)
        {
            return (1 << (n & 0x1f));
        }

        private bool okToChangeTag(TiffTag tag)
        {
            TiffFieldInfo fip = FindFieldInfo(tag, TiffType.ANY);
            if (fip == null)
            {
                return false;
            }

            if (tag != TiffTag.IMAGELENGTH &&
                (m_flags & TiffFlags.BEENWRITING) == TiffFlags.BEENWRITING &&
                !fip.OkToChange)
            {
                return false;
            }

            return true;
        }

        private void setupDefaultDirectory()
        {
            int tiffFieldInfoCount;
            TiffFieldInfo[] tiffFieldInfo = getFieldInfo(out tiffFieldInfoCount);
            setupFieldInfo(tiffFieldInfo, tiffFieldInfoCount);

            m_dir = new TiffDirectory();
            m_postDecodeMethod = PostDecodeMethodType.pdmNone;
            m_foundfield = null;

            m_tagmethods = m_defaultTagMethods;

            SetField(TiffTag.COMPRESSION, Compression.NONE);

            m_flags &= ~TiffFlags.DIRTYDIRECT;
            m_flags &= ~TiffFlags.ISTILED;

            m_tilesize = -1;
            m_scanlinesize = -1;
        }

        internal static void setString(out string cpp, string cp)
        {
            cpp = cp;
        }

        internal static void setShortArray(out short[] wpp, short[] wp, int n)
        {
            wpp = new short[n];
            for (int i = 0; i < n; i++)
                wpp[i] = wp[i];
        }

        internal static void setLongArray(out int[] lpp, int[] lp, int n)
        {
            lpp = new int[n];
            for (int i = 0; i < n; i++)
                lpp[i] = lp[i];
        }

        internal static void setFloatArray(out float[] fpp, float[] fp, int n)
        {
            fpp = new float[n];
            for (int i = 0; i < n; i++)
                fpp[i] = fp[i];
        }

        internal bool fieldSet(int field)
        {
            return ((m_dir.td_fieldsset[field / 32] & BITn(field)) != 0);
        }

        internal void setFieldBit(int field)
        {
            m_dir.td_fieldsset[field / 32] |= BITn(field);
        }

        internal void clearFieldBit(int field)
        {
            m_dir.td_fieldsset[field / 32] &= ~BITn(field);
        }

        private static readonly TiffFieldInfo[] tiffFieldInfo = 
        {
            new TiffFieldInfo(TiffTag.SUBFILETYPE, 1, 1, TiffType.LONG, FieldBit.SubFileType, true, false, "SubfileType"), 
            new TiffFieldInfo(TiffTag.SUBFILETYPE, 1, 1, TiffType.SHORT, FieldBit.SubFileType, true, false, "SubfileType"), 
            new TiffFieldInfo(TiffTag.OSUBFILETYPE, 1, 1, TiffType.SHORT, FieldBit.SubFileType, true, false, "OldSubfileType"), 
            new TiffFieldInfo(TiffTag.IMAGEWIDTH, 1, 1, TiffType.LONG, FieldBit.ImageDimensions, false, false, "ImageWidth"), 
            new TiffFieldInfo(TiffTag.IMAGEWIDTH, 1, 1, TiffType.SHORT, FieldBit.ImageDimensions, false, false, "ImageWidth"), 
            new TiffFieldInfo(TiffTag.IMAGELENGTH, 1, 1, TiffType.LONG, FieldBit.ImageDimensions, true, false, "ImageLength"), 
            new TiffFieldInfo(TiffTag.IMAGELENGTH, 1, 1, TiffType.SHORT, FieldBit.ImageDimensions, true, false, "ImageLength"), 
            new TiffFieldInfo(TiffTag.BITSPERSAMPLE, -1, -1, TiffType.SHORT, FieldBit.BitsPerSample, false, false, "BitsPerSample"), 
            new TiffFieldInfo(TiffTag.BITSPERSAMPLE, -1, -1, TiffType.LONG, FieldBit.BitsPerSample, false, false, "BitsPerSample"), 
            new TiffFieldInfo(TiffTag.COMPRESSION, -1, 1, TiffType.SHORT, FieldBit.Compression, false, false, "Compression"), 
            new TiffFieldInfo(TiffTag.COMPRESSION, -1, 1, TiffType.LONG, FieldBit.Compression, false, false, "Compression"), 
            new TiffFieldInfo(TiffTag.PHOTOMETRIC, 1, 1, TiffType.SHORT, FieldBit.Photometric, false, false, "PhotometricInterpretation"), 
            new TiffFieldInfo(TiffTag.PHOTOMETRIC, 1, 1, TiffType.LONG, FieldBit.Photometric, false, false, "PhotometricInterpretation"), 
            new TiffFieldInfo(TiffTag.THRESHHOLDING, 1, 1, TiffType.SHORT, FieldBit.Thresholding, true, false, "Threshholding"), 
            new TiffFieldInfo(TiffTag.CELLWIDTH, 1, 1, TiffType.SHORT, FieldBit.Ignore, true, false, "CellWidth"), 
            new TiffFieldInfo(TiffTag.CELLLENGTH, 1, 1, TiffType.SHORT, FieldBit.Ignore, true, false, "CellLength"), 
            new TiffFieldInfo(TiffTag.FILLORDER, 1, 1, TiffType.SHORT, FieldBit.FillOrder, false, false, "FillOrder"), 
            new TiffFieldInfo(TiffTag.DOCUMENTNAME, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "DocumentName"), 
            new TiffFieldInfo(TiffTag.IMAGEDESCRIPTION, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "ImageDescription"), 
            new TiffFieldInfo(TiffTag.MAKE, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "Make"), 
            new TiffFieldInfo(TiffTag.MODEL, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "Model"), 
            new TiffFieldInfo(TiffTag.STRIPOFFSETS, -1, -1, TiffType.LONG, FieldBit.StripOffsets, false, false, "StripOffsets"), 
            new TiffFieldInfo(TiffTag.STRIPOFFSETS, -1, -1, TiffType.SHORT, FieldBit.StripOffsets, false, false, "StripOffsets"), 
            new TiffFieldInfo(TiffTag.ORIENTATION, 1, 1, TiffType.SHORT, FieldBit.Orientation, false, false, "Orientation"), 
            new TiffFieldInfo(TiffTag.SAMPLESPERPIXEL, 1, 1, TiffType.SHORT, FieldBit.SamplesPerPixel, false, false, "SamplesPerPixel"), 
            new TiffFieldInfo(TiffTag.ROWSPERSTRIP, 1, 1, TiffType.LONG, FieldBit.RowsPerStrip, false, false, "RowsPerStrip"), 
            new TiffFieldInfo(TiffTag.ROWSPERSTRIP, 1, 1, TiffType.SHORT, FieldBit.RowsPerStrip, false, false, "RowsPerStrip"), 
            new TiffFieldInfo(TiffTag.STRIPBYTECOUNTS, -1, -1, TiffType.LONG, FieldBit.StripByteCounts, false, false, "StripByteCounts"), 
            new TiffFieldInfo(TiffTag.STRIPBYTECOUNTS, -1, -1, TiffType.SHORT, FieldBit.StripByteCounts, false, false, "StripByteCounts"), 
            new TiffFieldInfo(TiffTag.MINSAMPLEVALUE, -2, -1, TiffType.SHORT, FieldBit.MinSampleValue, true, false, "MinSampleValue"), 
            new TiffFieldInfo(TiffTag.MAXSAMPLEVALUE, -2, -1, TiffType.SHORT, FieldBit.MaxSampleValue, true, false, "MaxSampleValue"), 
            new TiffFieldInfo(TiffTag.XRESOLUTION, 1, 1, TiffType.RATIONAL, FieldBit.Resolution, true, false, "XResolution"), 
            new TiffFieldInfo(TiffTag.YRESOLUTION, 1, 1, TiffType.RATIONAL, FieldBit.Resolution, true, false, "YResolution"), 
            new TiffFieldInfo(TiffTag.PLANARCONFIG, 1, 1, TiffType.SHORT, FieldBit.PlaneConfig, false, false, "PlanarConfiguration"), 
            new TiffFieldInfo(TiffTag.PAGENAME, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "PageName"), 
            new TiffFieldInfo(TiffTag.XPOSITION, 1, 1, TiffType.RATIONAL, FieldBit.Position, true, false, "XPosition"), 
            new TiffFieldInfo(TiffTag.YPOSITION, 1, 1, TiffType.RATIONAL, FieldBit.Position, true, false, "YPosition"), 
            new TiffFieldInfo(TiffTag.FREEOFFSETS, -1, -1, TiffType.LONG, FieldBit.Ignore, false, false, "FreeOffsets"), 
            new TiffFieldInfo(TiffTag.FREEBYTECOUNTS, -1, -1, TiffType.LONG, FieldBit.Ignore, false, false, "FreeByteCounts"), 
            new TiffFieldInfo(TiffTag.GRAYRESPONSEUNIT, 1, 1, TiffType.SHORT, FieldBit.Ignore, true, false, "GrayResponseUnit"), 
            new TiffFieldInfo(TiffTag.GRAYRESPONSECURVE, -1, -1, TiffType.SHORT, FieldBit.Ignore, true, false, "GrayResponseCurve"), 
            new TiffFieldInfo(TiffTag.RESOLUTIONUNIT, 1, 1, TiffType.SHORT, FieldBit.ResolutionUnit, true, false, "ResolutionUnit"), 
            new TiffFieldInfo(TiffTag.PAGENUMBER, 2, 2, TiffType.SHORT, FieldBit.PageNumber, true, false, "PageNumber"), 
            new TiffFieldInfo(TiffTag.COLORRESPONSEUNIT, 1, 1, TiffType.SHORT, FieldBit.Ignore, true, false, "ColorResponseUnit"), 
            new TiffFieldInfo(TiffTag.TRANSFERFUNCTION, -1, -1, TiffType.SHORT, FieldBit.TransferFunction, true, false, "TransferFunction"), 
            new TiffFieldInfo(TiffTag.SOFTWARE, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "Software"), 
            new TiffFieldInfo(TiffTag.DATETIME, 20, 20, TiffType.ASCII, FieldBit.Custom, true, false, "DateTime"), 
            new TiffFieldInfo(TiffTag.ARTIST, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "Artist"), 
            new TiffFieldInfo(TiffTag.HOSTCOMPUTER, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "HostComputer"), 
            new TiffFieldInfo(TiffTag.WHITEPOINT, 2, 2, TiffType.RATIONAL, FieldBit.Custom, true, false, "WhitePoint"), 
            new TiffFieldInfo(TiffTag.PRIMARYCHROMATICITIES, 6, 6, TiffType.RATIONAL, FieldBit.Custom, true, false, "PrimaryChromaticities"), 
            new TiffFieldInfo(TiffTag.COLORMAP, -1, -1, TiffType.SHORT, FieldBit.ColorMap, true, false, "ColorMap"), 
            new TiffFieldInfo(TiffTag.HALFTONEHINTS, 2, 2, TiffType.SHORT, FieldBit.HalftoneHints, true, false, "HalftoneHints"), 
            new TiffFieldInfo(TiffTag.TILEWIDTH, 1, 1, TiffType.LONG, FieldBit.TileDimensions, false, false, "TileWidth"), 
            new TiffFieldInfo(TiffTag.TILEWIDTH, 1, 1, TiffType.SHORT, FieldBit.TileDimensions, false, false, "TileWidth"), 
            new TiffFieldInfo(TiffTag.TILELENGTH, 1, 1, TiffType.LONG, FieldBit.TileDimensions, false, false, "TileLength"), 
            new TiffFieldInfo(TiffTag.TILELENGTH, 1, 1, TiffType.SHORT, FieldBit.TileDimensions, false, false, "TileLength"), 
            new TiffFieldInfo(TiffTag.TILEOFFSETS, -1, 1, TiffType.LONG, FieldBit.StripOffsets, false, false, "TileOffsets"), 
            new TiffFieldInfo(TiffTag.TILEBYTECOUNTS, -1, 1, TiffType.LONG, FieldBit.StripByteCounts, false, false, "TileByteCounts"), 
            new TiffFieldInfo(TiffTag.TILEBYTECOUNTS, -1, 1, TiffType.SHORT, FieldBit.StripByteCounts, false, false, "TileByteCounts"), 
            new TiffFieldInfo(TiffTag.SUBIFD, -1, -1, TiffType.IFD, FieldBit.SubIFD, true, true, "SubIFD"), 
            new TiffFieldInfo(TiffTag.SUBIFD, -1, -1, TiffType.LONG, FieldBit.SubIFD, true, true, "SubIFD"), 
            new TiffFieldInfo(TiffTag.INKSET, 1, 1, TiffType.SHORT, FieldBit.Custom, false, false, "InkSet"), 
            new TiffFieldInfo(TiffTag.INKNAMES, -1, -1, TiffType.ASCII, FieldBit.InkNames, true, true, "InkNames"), 
            new TiffFieldInfo(TiffTag.NUMBEROFINKS, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "NumberOfInks"), 
            new TiffFieldInfo(TiffTag.DOTRANGE, 2, 2, TiffType.SHORT, FieldBit.Custom, false, false, "DotRange"), 
            new TiffFieldInfo(TiffTag.DOTRANGE, 2, 2, TiffType.BYTE, FieldBit.Custom, false, false, "DotRange"), 
            new TiffFieldInfo(TiffTag.TARGETPRINTER, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "TargetPrinter"), 
            new TiffFieldInfo(TiffTag.EXTRASAMPLES, -1, -1, TiffType.SHORT, FieldBit.ExtraSamples, false, true, "ExtraSamples"), 
            new TiffFieldInfo(TiffTag.EXTRASAMPLES, -1, -1, TiffType.BYTE, FieldBit.ExtraSamples, false, true, "ExtraSamples"), 
            new TiffFieldInfo(TiffTag.SAMPLEFORMAT, -1, -1, TiffType.SHORT, FieldBit.SampleFormat, false, false, "SampleFormat"), 
            new TiffFieldInfo(TiffTag.SMINSAMPLEVALUE, -2, -1, TiffType.ANY, FieldBit.SMinSampleValue, true, false, "SMinSampleValue"), 
            new TiffFieldInfo(TiffTag.SMAXSAMPLEVALUE, -2, -1, TiffType.ANY, FieldBit.SMaxSampleValue, true, false, "SMaxSampleValue"), 
            new TiffFieldInfo(TiffTag.CLIPPATH, -1, -3, TiffType.BYTE, FieldBit.Custom, false, true, "ClipPath"), 
            new TiffFieldInfo(TiffTag.XCLIPPATHUNITS, 1, 1, TiffType.SLONG, FieldBit.Custom, false, false, "XClipPathUnits"), 
            new TiffFieldInfo(TiffTag.XCLIPPATHUNITS, 1, 1, TiffType.SSHORT, FieldBit.Custom, false, false, "XClipPathUnits"), 
            new TiffFieldInfo(TiffTag.XCLIPPATHUNITS, 1, 1, TiffType.SBYTE, FieldBit.Custom, false, false, "XClipPathUnits"), 
            new TiffFieldInfo(TiffTag.YCLIPPATHUNITS, 1, 1, TiffType.SLONG, FieldBit.Custom, false, false, "YClipPathUnits"), 
            new TiffFieldInfo(TiffTag.YCLIPPATHUNITS, 1, 1, TiffType.SSHORT, FieldBit.Custom, false, false, "YClipPathUnits"), 
            new TiffFieldInfo(TiffTag.YCLIPPATHUNITS, 1, 1, TiffType.SBYTE, FieldBit.Custom, false, false, "YClipPathUnits"), 
            new TiffFieldInfo(TiffTag.YCBCRCOEFFICIENTS, 3, 3, TiffType.RATIONAL, FieldBit.Custom, false, false, "YCbCrCoefficients"), 
            new TiffFieldInfo(TiffTag.YCBCRSUBSAMPLING, 2, 2, TiffType.SHORT, FieldBit.YCbCrSubsampling, false, false, "YCbCrSubsampling"), 
            new TiffFieldInfo(TiffTag.YCBCRPOSITIONING, 1, 1, TiffType.SHORT, FieldBit.YCbCrPositioning, false, false, "YCbCrPositioning"), 
            new TiffFieldInfo(TiffTag.REFERENCEBLACKWHITE, 6, 6, TiffType.RATIONAL, FieldBit.RefBlackWhite, true, false, "ReferenceBlackWhite"), 
            new TiffFieldInfo(TiffTag.REFERENCEBLACKWHITE, 6, 6, TiffType.LONG, FieldBit.RefBlackWhite, true, false, "ReferenceBlackWhite"), 
            new TiffFieldInfo(TiffTag.XMLPACKET, -3, -3, TiffType.BYTE, FieldBit.Custom, false, true, "XMLPacket"), 
            new TiffFieldInfo(TiffTag.MATTEING, 1, 1, TiffType.SHORT, FieldBit.ExtraSamples, false, false, "Matteing"), 
            new TiffFieldInfo(TiffTag.DATATYPE, -2, -1, TiffType.SHORT, FieldBit.SampleFormat, false, false, "DataType"), 
            new TiffFieldInfo(TiffTag.IMAGEDEPTH, 1, 1, TiffType.LONG, FieldBit.ImageDepth, false, false, "ImageDepth"), 
            new TiffFieldInfo(TiffTag.IMAGEDEPTH, 1, 1, TiffType.SHORT, FieldBit.ImageDepth, false, false, "ImageDepth"), 
            new TiffFieldInfo(TiffTag.TILEDEPTH, 1, 1, TiffType.LONG, FieldBit.TileDepth, false, false, "TileDepth"), 
            new TiffFieldInfo(TiffTag.TILEDEPTH, 1, 1, TiffType.SHORT, FieldBit.TileDepth, false, false, "TileDepth"), 
            new TiffFieldInfo(TiffTag.PIXAR_IMAGEFULLWIDTH, 1, 1, TiffType.LONG, FieldBit.Custom, true, false, "ImageFullWidth"), 
            new TiffFieldInfo(TiffTag.PIXAR_IMAGEFULLLENGTH, 1, 1, TiffType.LONG, FieldBit.Custom, true, false, "ImageFullLength"), 
            new TiffFieldInfo(TiffTag.PIXAR_TEXTUREFORMAT, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "TextureFormat"), 
            new TiffFieldInfo(TiffTag.PIXAR_WRAPMODES, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "TextureWrapModes"), 
            new TiffFieldInfo(TiffTag.PIXAR_FOVCOT, 1, 1, TiffType.FLOAT, FieldBit.Custom, true, false, "FieldOfViewCotangent"), 
            new TiffFieldInfo(TiffTag.PIXAR_MATRIX_WORLDTOSCREEN, 16, 16, TiffType.FLOAT, FieldBit.Custom, true, false, "MatrixWorldToScreen"), 
            new TiffFieldInfo(TiffTag.PIXAR_MATRIX_WORLDTOCAMERA, 16, 16, TiffType.FLOAT, FieldBit.Custom, true, false, "MatrixWorldToCamera"), 
            new TiffFieldInfo(TiffTag.COPYRIGHT, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "Copyright"), 
            new TiffFieldInfo(TiffTag.RICHTIFFIPTC, -3, -3, TiffType.LONG, FieldBit.Custom, false, true, "RichTIFFIPTC"), 
            new TiffFieldInfo(TiffTag.PHOTOSHOP, -3, -3, TiffType.BYTE, FieldBit.Custom, false, true, "Photoshop"), 
            new TiffFieldInfo(TiffTag.EXIFIFD, 1, 1, TiffType.LONG, FieldBit.Custom, false, false, "EXIFIFDOffset"), 
            new TiffFieldInfo(TiffTag.ICCPROFILE, -3, -3, TiffType.UNDEFINED, FieldBit.Custom, false, true, "ICC Profile"), 
            new TiffFieldInfo(TiffTag.GPSIFD, 1, 1, TiffType.LONG, FieldBit.Custom, false, false, "GPSIFDOffset"), 
            new TiffFieldInfo(TiffTag.STONITS, 1, 1, TiffType.DOUBLE, FieldBit.Custom, false, false, "StoNits"), 
            new TiffFieldInfo(TiffTag.INTEROPERABILITYIFD, 1, 1, TiffType.LONG, FieldBit.Custom, false, false, "InteroperabilityIFDOffset"), 
            new TiffFieldInfo(TiffTag.DNGVERSION, 4, 4, TiffType.BYTE, FieldBit.Custom, false, false, "DNGVersion"), 
            new TiffFieldInfo(TiffTag.DNGBACKWARDVERSION, 4, 4, TiffType.BYTE, FieldBit.Custom, false, false, "DNGBackwardVersion"), 
            new TiffFieldInfo(TiffTag.UNIQUECAMERAMODEL, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "UniqueCameraModel"), 
            new TiffFieldInfo(TiffTag.LOCALIZEDCAMERAMODEL, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "LocalizedCameraModel"), 
            new TiffFieldInfo(TiffTag.LOCALIZEDCAMERAMODEL, -1, -1, TiffType.BYTE, FieldBit.Custom, true, true, "LocalizedCameraModel"), 
            new TiffFieldInfo(TiffTag.CFAPLANECOLOR, -1, -1, TiffType.BYTE, FieldBit.Custom, false, true, "CFAPlaneColor"), 
            new TiffFieldInfo(TiffTag.CFALAYOUT, 1, 1, TiffType.SHORT, FieldBit.Custom, false, false, "CFALayout"), 
            new TiffFieldInfo(TiffTag.LINEARIZATIONTABLE, -1, -1, TiffType.SHORT, FieldBit.Custom, false, true, "LinearizationTable"), 
            new TiffFieldInfo(TiffTag.BLACKLEVELREPEATDIM, 2, 2, TiffType.SHORT, FieldBit.Custom, false, false, "BlackLevelRepeatDim"), 
            new TiffFieldInfo(TiffTag.BLACKLEVEL, -1, -1, TiffType.LONG, FieldBit.Custom, false, true, "BlackLevel"), 
            new TiffFieldInfo(TiffTag.BLACKLEVEL, -1, -1, TiffType.SHORT, FieldBit.Custom, false, true, "BlackLevel"), 
            new TiffFieldInfo(TiffTag.BLACKLEVEL, -1, -1, TiffType.RATIONAL, FieldBit.Custom, false, true, "BlackLevel"), 
            new TiffFieldInfo(TiffTag.BLACKLEVELDELTAH, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "BlackLevelDeltaH"), 
            new TiffFieldInfo(TiffTag.BLACKLEVELDELTAV, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "BlackLevelDeltaV"), 
            new TiffFieldInfo(TiffTag.WHITELEVEL, -2, -2, TiffType.LONG, FieldBit.Custom, false, false, "WhiteLevel"), 
            new TiffFieldInfo(TiffTag.WHITELEVEL, -2, -2, TiffType.SHORT, FieldBit.Custom, false, false, "WhiteLevel"), 
            new TiffFieldInfo(TiffTag.DEFAULTSCALE, 2, 2, TiffType.RATIONAL, FieldBit.Custom, false, false, "DefaultScale"), 
            new TiffFieldInfo(TiffTag.BESTQUALITYSCALE, 1, 1, TiffType.RATIONAL, FieldBit.Custom, false, false, "BestQualityScale"), 
            new TiffFieldInfo(TiffTag.DEFAULTCROPORIGIN, 2, 2, TiffType.LONG, FieldBit.Custom, false, false, "DefaultCropOrigin"), 
            new TiffFieldInfo(TiffTag.DEFAULTCROPORIGIN, 2, 2, TiffType.SHORT, FieldBit.Custom, false, false, "DefaultCropOrigin"), 
            new TiffFieldInfo(TiffTag.DEFAULTCROPORIGIN, 2, 2, TiffType.RATIONAL, FieldBit.Custom, false, false, "DefaultCropOrigin"), 
            new TiffFieldInfo(TiffTag.DEFAULTCROPSIZE, 2, 2, TiffType.LONG, FieldBit.Custom, false, false, "DefaultCropSize"), 
            new TiffFieldInfo(TiffTag.DEFAULTCROPSIZE, 2, 2, TiffType.SHORT, FieldBit.Custom, false, false, "DefaultCropSize"), 
            new TiffFieldInfo(TiffTag.DEFAULTCROPSIZE, 2, 2, TiffType.RATIONAL, FieldBit.Custom, false, false, "DefaultCropSize"), 
            new TiffFieldInfo(TiffTag.COLORMATRIX1, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "ColorMatrix1"), 
            new TiffFieldInfo(TiffTag.COLORMATRIX2, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "ColorMatrix2"), 
            new TiffFieldInfo(TiffTag.CAMERACALIBRATION1, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "CameraCalibration1"), 
            new TiffFieldInfo(TiffTag.CAMERACALIBRATION2, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "CameraCalibration2"), 
            new TiffFieldInfo(TiffTag.REDUCTIONMATRIX1, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "ReductionMatrix1"), 
            new TiffFieldInfo(TiffTag.REDUCTIONMATRIX2, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "ReductionMatrix2"), 
            new TiffFieldInfo(TiffTag.ANALOGBALANCE, -1, -1, TiffType.RATIONAL, FieldBit.Custom, false, true, "AnalogBalance"), 
            new TiffFieldInfo(TiffTag.ASSHOTNEUTRAL, -1, -1, TiffType.SHORT, FieldBit.Custom, false, true, "AsShotNeutral"), 
            new TiffFieldInfo(TiffTag.ASSHOTNEUTRAL, -1, -1, TiffType.RATIONAL, FieldBit.Custom, false, true, "AsShotNeutral"), 
            new TiffFieldInfo(TiffTag.ASSHOTWHITEXY, 2, 2, TiffType.RATIONAL, FieldBit.Custom, false, false, "AsShotWhiteXY"), 
            new TiffFieldInfo(TiffTag.BASELINEEXPOSURE, 1, 1, TiffType.SRATIONAL, FieldBit.Custom, false, false, "BaselineExposure"), 
            new TiffFieldInfo(TiffTag.BASELINENOISE, 1, 1, TiffType.RATIONAL, FieldBit.Custom, false, false, "BaselineNoise"), 
            new TiffFieldInfo(TiffTag.BASELINESHARPNESS, 1, 1, TiffType.RATIONAL, FieldBit.Custom, false, false, "BaselineSharpness"), 
            new TiffFieldInfo(TiffTag.BAYERGREENSPLIT, 1, 1, TiffType.LONG, FieldBit.Custom, false, false, "BayerGreenSplit"), 
            new TiffFieldInfo(TiffTag.LINEARRESPONSELIMIT, 1, 1, TiffType.RATIONAL, FieldBit.Custom, false, false, "LinearResponseLimit"), 
            new TiffFieldInfo(TiffTag.CAMERASERIALNUMBER, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "CameraSerialNumber"), 
            new TiffFieldInfo(TiffTag.LENSINFO, 4, 4, TiffType.RATIONAL, FieldBit.Custom, false, false, "LensInfo"), 
            new TiffFieldInfo(TiffTag.CHROMABLURRADIUS, 1, 1, TiffType.RATIONAL, FieldBit.Custom, false, false, "ChromaBlurRadius"), 
            new TiffFieldInfo(TiffTag.ANTIALIASSTRENGTH, 1, 1, TiffType.RATIONAL, FieldBit.Custom, false, false, "AntiAliasStrength"), 
            new TiffFieldInfo(TiffTag.SHADOWSCALE, 1, 1, TiffType.RATIONAL, FieldBit.Custom, false, false, "ShadowScale"), 
            new TiffFieldInfo(TiffTag.DNGPRIVATEDATA, -1, -1, TiffType.BYTE, FieldBit.Custom, false, true, "DNGPrivateData"), 
            new TiffFieldInfo(TiffTag.MAKERNOTESAFETY, 1, 1, TiffType.SHORT, FieldBit.Custom, false, false, "MakerNoteSafety"), 
            new TiffFieldInfo(TiffTag.CALIBRATIONILLUMINANT1, 1, 1, TiffType.SHORT, FieldBit.Custom, false, false, "CalibrationIlluminant1"), 
            new TiffFieldInfo(TiffTag.CALIBRATIONILLUMINANT2, 1, 1, TiffType.SHORT, FieldBit.Custom, false, false, "CalibrationIlluminant2"), 
            new TiffFieldInfo(TiffTag.RAWDATAUNIQUEID, 16, 16, TiffType.BYTE, FieldBit.Custom, false, false, "RawDataUniqueID"), 
            new TiffFieldInfo(TiffTag.ORIGINALRAWFILENAME, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "OriginalRawFileName"), 
            new TiffFieldInfo(TiffTag.ORIGINALRAWFILENAME, -1, -1, TiffType.BYTE, FieldBit.Custom, true, true, "OriginalRawFileName"), 
            new TiffFieldInfo(TiffTag.ORIGINALRAWFILEDATA, -1, -1, TiffType.UNDEFINED, FieldBit.Custom, false, true, "OriginalRawFileData"), 
            new TiffFieldInfo(TiffTag.ACTIVEAREA, 4, 4, TiffType.LONG, FieldBit.Custom, false, false, "ActiveArea"), 
            new TiffFieldInfo(TiffTag.ACTIVEAREA, 4, 4, TiffType.SHORT, FieldBit.Custom, false, false, "ActiveArea"), 
            new TiffFieldInfo(TiffTag.MASKEDAREAS, -1, -1, TiffType.LONG, FieldBit.Custom, false, true, "MaskedAreas"), 
            new TiffFieldInfo(TiffTag.ASSHOTICCPROFILE, -1, -1, TiffType.UNDEFINED, FieldBit.Custom, false, true, "AsShotICCProfile"), 
            new TiffFieldInfo(TiffTag.ASSHOTPREPROFILEMATRIX, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "AsShotPreProfileMatrix"), 
            new TiffFieldInfo(TiffTag.CURRENTICCPROFILE, -1, -1, TiffType.UNDEFINED, FieldBit.Custom, false, true, "CurrentICCProfile"), 
            new TiffFieldInfo(TiffTag.CURRENTPREPROFILEMATRIX, -1, -1, TiffType.SRATIONAL, FieldBit.Custom, false, true, "CurrentPreProfileMatrix"),
        };

        private static readonly TiffFieldInfo[] exifFieldInfo = 
        {
            new TiffFieldInfo(TiffTag.EXIF_EXPOSURETIME, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "ExposureTime"), 
            new TiffFieldInfo(TiffTag.EXIF_FNUMBER, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "FNumber"), 
            new TiffFieldInfo(TiffTag.EXIF_EXPOSUREPROGRAM, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "ExposureProgram"), 
            new TiffFieldInfo(TiffTag.EXIF_SPECTRALSENSITIVITY, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "SpectralSensitivity"), 
            new TiffFieldInfo(TiffTag.EXIF_ISOSPEEDRATINGS, -1, -1, TiffType.SHORT, FieldBit.Custom, true, true, "ISOSpeedRatings"), 
            new TiffFieldInfo(TiffTag.EXIF_OECF, -1, -1, TiffType.UNDEFINED, FieldBit.Custom, true, true, "OptoelectricConversionFactor"), 
            new TiffFieldInfo(TiffTag.EXIF_EXIFVERSION, 4, 4, TiffType.UNDEFINED, FieldBit.Custom, true, false, "ExifVersion"), 
            new TiffFieldInfo(TiffTag.EXIF_DATETIMEORIGINAL, 20, 20, TiffType.ASCII, FieldBit.Custom, true, false, "DateTimeOriginal"), 
            new TiffFieldInfo(TiffTag.EXIF_DATETIMEDIGITIZED, 20, 20, TiffType.ASCII, FieldBit.Custom, true, false, "DateTimeDigitized"), 
            new TiffFieldInfo(TiffTag.EXIF_COMPONENTSCONFIGURATION, 4, 4, TiffType.UNDEFINED, FieldBit.Custom, true, false, "ComponentsConfiguration"), 
            new TiffFieldInfo(TiffTag.EXIF_COMPRESSEDBITSPERPIXEL, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "CompressedBitsPerPixel"), 
            new TiffFieldInfo(TiffTag.EXIF_SHUTTERSPEEDVALUE, 1, 1, TiffType.SRATIONAL, FieldBit.Custom, true, false, "ShutterSpeedValue"), 
            new TiffFieldInfo(TiffTag.EXIF_APERTUREVALUE, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "ApertureValue"), 
            new TiffFieldInfo(TiffTag.EXIF_BRIGHTNESSVALUE, 1, 1, TiffType.SRATIONAL, FieldBit.Custom, true, false, "BrightnessValue"), 
            new TiffFieldInfo(TiffTag.EXIF_EXPOSUREBIASVALUE, 1, 1, TiffType.SRATIONAL, FieldBit.Custom, true, false, "ExposureBiasValue"), 
            new TiffFieldInfo(TiffTag.EXIF_MAXAPERTUREVALUE, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "MaxApertureValue"), 
            new TiffFieldInfo(TiffTag.EXIF_SUBJECTDISTANCE, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "SubjectDistance"), 
            new TiffFieldInfo(TiffTag.EXIF_METERINGMODE, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "MeteringMode"), 
            new TiffFieldInfo(TiffTag.EXIF_LIGHTSOURCE, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "LightSource"), 
            new TiffFieldInfo(TiffTag.EXIF_FLASH, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "Flash"), 
            new TiffFieldInfo(TiffTag.EXIF_FOCALLENGTH, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "FocalLength"), 
            new TiffFieldInfo(TiffTag.EXIF_SUBJECTAREA, -1, -1, TiffType.SHORT, FieldBit.Custom, true, true, "SubjectArea"), 
            new TiffFieldInfo(TiffTag.EXIF_MAKERNOTE, -1, -1, TiffType.UNDEFINED, FieldBit.Custom, true, true, "MakerNote"), 
            new TiffFieldInfo(TiffTag.EXIF_USERCOMMENT, -1, -1, TiffType.UNDEFINED, FieldBit.Custom, true, true, "UserComment"), 
            new TiffFieldInfo(TiffTag.EXIF_SUBSECTIME, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "SubSecTime"), 
            new TiffFieldInfo(TiffTag.EXIF_SUBSECTIMEORIGINAL, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "SubSecTimeOriginal"), 
            new TiffFieldInfo(TiffTag.EXIF_SUBSECTIMEDIGITIZED, -1, -1, TiffType.ASCII, FieldBit.Custom, true, false, "SubSecTimeDigitized"), 
            new TiffFieldInfo(TiffTag.EXIF_FLASHPIXVERSION, 4, 4, TiffType.UNDEFINED, FieldBit.Custom, true, false, "FlashpixVersion"), 
            new TiffFieldInfo(TiffTag.EXIF_COLORSPACE, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "ColorSpace"),
            new TiffFieldInfo(TiffTag.EXIF_PIXELXDIMENSION, 1, 1, TiffType.LONG, FieldBit.Custom, true, false, "PixelXDimension"), 
            new TiffFieldInfo(TiffTag.EXIF_PIXELXDIMENSION, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "PixelXDimension"), 
            new TiffFieldInfo(TiffTag.EXIF_PIXELYDIMENSION, 1, 1, TiffType.LONG, FieldBit.Custom, true, false, "PixelYDimension"), 
            new TiffFieldInfo(TiffTag.EXIF_PIXELYDIMENSION, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "PixelYDimension"), 
            new TiffFieldInfo(TiffTag.EXIF_RELATEDSOUNDFILE, 13, 13, TiffType.ASCII, FieldBit.Custom, true, false, "RelatedSoundFile"), 
            new TiffFieldInfo(TiffTag.EXIF_FLASHENERGY, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "FlashEnergy"), 
            new TiffFieldInfo(TiffTag.EXIF_SPATIALFREQUENCYRESPONSE, -1, -1, TiffType.UNDEFINED, FieldBit.Custom, true, true, "SpatialFrequencyResponse"), 
            new TiffFieldInfo(TiffTag.EXIF_FOCALPLANEXRESOLUTION, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "FocalPlaneXResolution"), 
            new TiffFieldInfo(TiffTag.EXIF_FOCALPLANEYRESOLUTION, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "FocalPlaneYResolution"), 
            new TiffFieldInfo(TiffTag.EXIF_FOCALPLANERESOLUTIONUNIT, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "FocalPlaneResolutionUnit"), 
            new TiffFieldInfo(TiffTag.EXIF_SUBJECTLOCATION, 2, 2, TiffType.SHORT, FieldBit.Custom, true, false, "SubjectLocation"), 
            new TiffFieldInfo(TiffTag.EXIF_EXPOSUREINDEX, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "ExposureIndex"), 
            new TiffFieldInfo(TiffTag.EXIF_SENSINGMETHOD, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "SensingMethod"), 
            new TiffFieldInfo(TiffTag.EXIF_FILESOURCE, 1, 1, TiffType.UNDEFINED, FieldBit.Custom, true, false, "FileSource"), 
            new TiffFieldInfo(TiffTag.EXIF_SCENETYPE, 1, 1, TiffType.UNDEFINED, FieldBit.Custom, true, false, "SceneType"), 
            new TiffFieldInfo(TiffTag.EXIF_CFAPATTERN, -1, -1, TiffType.UNDEFINED, FieldBit.Custom, true, true, "CFAPattern"), 
            new TiffFieldInfo(TiffTag.EXIF_CUSTOMRENDERED, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "CustomRendered"), 
            new TiffFieldInfo(TiffTag.EXIF_EXPOSUREMODE, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "ExposureMode"), 
            new TiffFieldInfo(TiffTag.EXIF_WHITEBALANCE, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "WhiteBalance"), 
            new TiffFieldInfo(TiffTag.EXIF_DIGITALZOOMRATIO, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "DigitalZoomRatio"), 
            new TiffFieldInfo(TiffTag.EXIF_FOCALLENGTHIN35MMFILM, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "FocalLengthIn35mmFilm"), 
            new TiffFieldInfo(TiffTag.EXIF_SCENECAPTURETYPE, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "SceneCaptureType"), 
            new TiffFieldInfo(TiffTag.EXIF_GAINCONTROL, 1, 1, TiffType.RATIONAL, FieldBit.Custom, true, false, "GainControl"), 
            new TiffFieldInfo(TiffTag.EXIF_CONTRAST, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "Contrast"), 
            new TiffFieldInfo(TiffTag.EXIF_SATURATION, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "Saturation"), 
            new TiffFieldInfo(TiffTag.EXIF_SHARPNESS, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "Sharpness"), 
            new TiffFieldInfo(TiffTag.EXIF_DEVICESETTINGDESCRIPTION, -1, -1, TiffType.UNDEFINED, FieldBit.Custom, true, true, "DeviceSettingDescription"), 
            new TiffFieldInfo(TiffTag.EXIF_SUBJECTDISTANCERANGE, 1, 1, TiffType.SHORT, FieldBit.Custom, true, false, "SubjectDistanceRange"), 
            new TiffFieldInfo(TiffTag.EXIF_IMAGEUNIQUEID, 33, 33, TiffType.ASCII, FieldBit.Custom, true, false, "ImageUniqueID")
        };

        private static TiffFieldInfo[] getFieldInfo(out int size)
        {
            size = tiffFieldInfo.Length;
            return tiffFieldInfo;
        }

        private void setupFieldInfo(TiffFieldInfo[] info, int n)
        {
            m_nfields = 0;
            MergeFieldInfo(info, n);
        }

        private static TiffFieldInfo createAnonFieldInfo(TiffTag tag, TiffType field_type)
        {
            TiffFieldInfo fld = new TiffFieldInfo(tag, TiffFieldInfo.Variable2,
                TiffFieldInfo.Variable2, field_type, FieldBit.Custom, true, true, null);

            fld.Name = string.Format(CultureInfo.InvariantCulture, "Tag {0}", tag);
            return fld;
        }

        internal static int dataSize(TiffType type)
        {
            switch (type)
            {
                case TiffType.BYTE:
                case TiffType.SBYTE:
                case TiffType.ASCII:
                case TiffType.UNDEFINED:
                    return 1;

                case TiffType.SHORT:
                case TiffType.SSHORT:
                    return 2;

                case TiffType.LONG:
                case TiffType.SLONG:
                case TiffType.FLOAT:
                case TiffType.IFD:
                case TiffType.RATIONAL:
                case TiffType.SRATIONAL:
                    return 4;

                case TiffType.DOUBLE:
                    return 8;

                default:
                    return 0;
            }
        }

        private int extractData(TiffDirEntry dir)
        {
            int type = (int)dir.tdir_type;
            if (m_header.tiff_magic == TIFF_BIGENDIAN)
                return (int)((dir.tdir_offset >> m_typeshift[type]) & m_typemask[type]);

            return (int)(dir.tdir_offset & m_typemask[type]);
        }

        private bool byteCountLooksBad(TiffDirectory td)
        {
            return
            (
                (td.td_stripbytecount[0] == 0 && td.td_stripoffset[0] != 0) ||
                (td.td_compression == Compression.NONE && td.td_stripbytecount[0] > getFileSize() - td.td_stripoffset[0]) ||
                (m_mode == O_RDONLY && td.td_compression == Compression.NONE && td.td_stripbytecount[0] < ScanlineSize() * td.td_imagelength)
            );
        }

        private static int howMany8(int x)
        {
            return ((x & 0x07) != 0 ? (x >> 3) + 1 : x >> 3);
        }

        private bool estimateStripByteCounts(TiffDirEntry[] dir, short dircount)
        {
            const string module = "estimateStripByteCounts";

            m_dir.td_stripbytecount = new uint[m_dir.td_nstrips];

            if (m_dir.td_compression != Compression.NONE)
            {
                long space = TiffHeader.SizeInBytes + sizeof(short) + (dircount * TiffDirEntry.SizeInBytes) + sizeof(int);
                long filesize = getFileSize();

                for (short n = 0; n < dircount; n++)
                {
                    int cc = DataWidth((TiffType)dir[n].tdir_type);
                    if (cc == 0)
                    {
                        return false;
                    }

                    cc = cc * dir[n].tdir_count;
                    if (cc > sizeof(int))
                        space += cc;
                }

                space = filesize - space;
                if (m_dir.td_planarconfig == PlanarConfig.SEPARATE)
                    space /= m_dir.td_samplesperpixel;

                int strip = 0;
                for (; strip < m_dir.td_nstrips; strip++)
                    m_dir.td_stripbytecount[strip] = (uint)space;

                strip--;
                if ((m_dir.td_stripoffset[strip] + m_dir.td_stripbytecount[strip]) > filesize)
                    m_dir.td_stripbytecount[strip] = (uint)(filesize - m_dir.td_stripoffset[strip]);
            }
            else if (IsTiled())
            {
                int bytespertile = TileSize();
                for (int strip = 0; strip < m_dir.td_nstrips; strip++)
                    m_dir.td_stripbytecount[strip] = (uint)bytespertile;
            }
            else
            {
                int rowbytes = ScanlineSize();
                int rowsperstrip = m_dir.td_imagelength / m_dir.td_stripsperimage;
                for (int strip = 0; strip < m_dir.td_nstrips; strip++)
                    m_dir.td_stripbytecount[strip] = (uint)(rowbytes * rowsperstrip);
            }

            setFieldBit(FieldBit.StripByteCounts);
            if (!fieldSet(FieldBit.RowsPerStrip))
                m_dir.td_rowsperstrip = m_dir.td_imagelength;

            return true;
        }

        private void missingRequired(string tagname)
        {
            const string module = "missingRequired";
        }

        private int fetchFailed(TiffDirEntry dir)
        {
            string format = "Error fetching data for field \"{0}\"";

            TiffFieldInfo fi = FieldWithTag(dir.tdir_tag);
            if (fi.Bit == FieldBit.Custom)
            {
            }
            else
            {
            }

            return 0;
        }

        private static int readDirectoryFind(TiffDirEntry[] dir, short dircount, TiffTag tagid)
        {
            for (short n = 0; n < dircount; n++)
            {
                if (dir[n].tdir_tag == tagid)
                    return n;
            }

            return -1;
        }

        /// <summary>
        /// Checks the directory offset against the list of already seen directory
        /// offsets.
        /// </summary>
        private bool checkDirOffset(uint diroff)
        {
            if (diroff == 0)
            {
                return false;
            }

            for (short n = 0; n < m_dirnumber && m_dirlist != null; n++)
            {
                if (m_dirlist[n] == diroff)
                    return false;
            }

            m_dirnumber++;

            if (m_dirnumber > m_dirlistsize)
            {
                uint[] new_dirlist = Realloc(m_dirlist, m_dirnumber - 1, 2 * m_dirnumber);
                m_dirlistsize = 2 * m_dirnumber;
                m_dirlist = new_dirlist;
            }

            m_dirlist[m_dirnumber - 1] = diroff;
            return true;
        }

        /// <summary>
        /// Reads IFD structure from the specified offset.
        /// </summary>
        private short fetchDirectory(uint diroff, out TiffDirEntry[] pdir, out uint nextdiroff)
        {
            const string module = "fetchDirectory";

            m_diroff = diroff;
            nextdiroff = 0;

            short dircount;
            TiffDirEntry[] dir = null;
            pdir = null;

            if (!seekOK(m_diroff))
            {
                return 0;
            }

            if (!readShortOK(out dircount))
            {
                return 0;
            }

            if ((m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
                SwabShort(ref dircount);

            dir = new TiffDirEntry[dircount];
            if (!readDirEntryOk(dir, dircount))
            {
                return 0;
            }

            int temp;
            readIntOK(out temp);
            nextdiroff = (uint)temp;

            if ((m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
            {
                temp = (int)nextdiroff;
                SwabLong(ref temp);
                nextdiroff = (uint)temp;
            }

            pdir = dir;
            return dircount;
        }

        private bool fetchSubjectDistance(TiffDirEntry dir)
        {
            if (dir.tdir_count != 1 || dir.tdir_type != TiffType.RATIONAL)
            {
                return false;
            }

            bool ok = false;

            byte[] b = new byte[2 * sizeof(int)];
            int read = fetchData(dir, b);
            if (read != 0)
            {
                int[] l = new int[2];
                l[0] = readInt(b, 0);
                l[1] = readInt(b, sizeof(int));

                float v;
                if (cvtRational(dir, l[0], l[1], out v))
                {
                    ok = SetField(dir.tdir_tag, (l[0] != -1) ? v : -v);
                }
            }

            return ok;
        }

        private bool checkDirCount(TiffDirEntry dir, int count)
        {
            if (count > dir.tdir_count)
            {
                return false;
            }
            else if (count < dir.tdir_count)
            {
                dir.tdir_count = count;
                return true;
            }

            return true;
        }

        /// <summary>
        /// Fetches a contiguous directory item.
        /// </summary>
        private int fetchData(TiffDirEntry dir, byte[] buffer)
        {
            int width = DataWidth(dir.tdir_type);
            int count = (int)dir.tdir_count * width;

            // Check for overflow.
            if (dir.tdir_count == 0 || width == 0 || (count / width) != dir.tdir_count)
                fetchFailed(dir);

            if (!seekOK(dir.tdir_offset))
                fetchFailed(dir);

            if (!readOK(buffer, count))
                fetchFailed(dir);

            if ((m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
            {
                switch (dir.tdir_type)
                {
                    case TiffType.SHORT:
                    case TiffType.SSHORT:
                        short[] s = ByteArrayToShorts(buffer, 0, count);
                        SwabArrayOfShort(s, dir.tdir_count);
                        ShortsToByteArray(s, 0, dir.tdir_count, buffer, 0);
                        break;

                    case TiffType.LONG:
                    case TiffType.SLONG:
                    case TiffType.FLOAT:
                    case TiffType.IFD:
                        int[] l = ByteArrayToInts(buffer, 0, count);
                        SwabArrayOfLong(l, dir.tdir_count);
                        IntsToByteArray(l, 0, dir.tdir_count, buffer, 0);
                        break;

                    case TiffType.RATIONAL:
                    case TiffType.SRATIONAL:
                        int[] r = ByteArrayToInts(buffer, 0, count);
                        SwabArrayOfLong(r, 2 * dir.tdir_count);
                        IntsToByteArray(r, 0, 2 * dir.tdir_count, buffer, 0);
                        break;

                    case TiffType.DOUBLE:
                        swab64BitData(buffer, 0, count);
                        break;
                }
            }

            return count;
        }

        /// <summary>
        /// Fetches an ASCII item from the file.
        /// </summary>
        private int fetchString(TiffDirEntry dir, out string cp)
        {
            byte[] bytes = null;

            if (dir.tdir_count <= 4)
            {
                int l = (int)dir.tdir_offset;
                if ((m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
                    SwabLong(ref l);

                bytes = new byte[sizeof(int)];
                writeInt(l, bytes, 0);
                cp = Latin1Encoding.GetString(bytes, 0, dir.tdir_count);
                return 1;
            }

            bytes = new byte[dir.tdir_count];
            int res = fetchData(dir, bytes);
            cp = Latin1Encoding.GetString(bytes, 0, dir.tdir_count);
            return res;
        }

        private bool cvtRational(TiffDirEntry dir, int num, int denom, out float rv)
        {
            if (denom == 0)
            {
                rv = float.NaN;
                return false;
            }
            else
            {
                rv = ((float)num / (float)denom);
                return true;
            }
        }

        private float fetchRational(TiffDirEntry dir)
        {
            byte[] bytes = new byte[sizeof(int) * 2];
            int read = fetchData(dir, bytes);
            if (read != 0)
            {
                int[] l = new int[2];
                l[0] = readInt(bytes, 0);
                l[1] = readInt(bytes, sizeof(int));

                float v;
                bool res = cvtRational(dir, l[0], l[1], out v);
                if (res)
                    return v;
            }

            return 1.0f;
        }

        /// <summary>
        /// Fetch a single floating point value from the offset field and
        /// return it as a native float.
        /// </summary>
        private float fetchFloat(TiffDirEntry dir)
        {
            int l = extractData(dir);
            return BitConverter.ToSingle(BitConverter.GetBytes(l), 0);
        }

        /// <summary>
        /// Fetches an array of BYTE or SBYTE values.
        /// </summary>
        private bool fetchByteArray(TiffDirEntry dir, byte[] v)
        {
            if (dir.tdir_count <= 4)
            {
                // Extract data from offset field.
                int count = dir.tdir_count;

                if (m_header.tiff_magic == TIFF_BIGENDIAN)
                {
                    if (count == 4)
                        v[3] = (byte)(dir.tdir_offset & 0xff);

                    if (count >= 3)
                        v[2] = (byte)((dir.tdir_offset >> 8) & 0xff);

                    if (count >= 2)
                        v[1] = (byte)((dir.tdir_offset >> 16) & 0xff);

                    if (count >= 1)
                        v[0] = (byte)(dir.tdir_offset >> 24);
                }
                else
                {
                    if (count == 4)
                        v[3] = (byte)(dir.tdir_offset >> 24);

                    if (count >= 3)
                        v[2] = (byte)((dir.tdir_offset >> 16) & 0xff);

                    if (count >= 2)
                        v[1] = (byte)((dir.tdir_offset >> 8) & 0xff);

                    if (count >= 1)
                        v[0] = (byte)(dir.tdir_offset & 0xff);
                }

                return true;
            }

            return (fetchData(dir, v) != 0);
        }

        /// <summary>
        /// Fetch an array of SHORT or SSHORT values.
        /// </summary>
        private bool fetchShortArray(TiffDirEntry dir, short[] v)
        {
            if (dir.tdir_count <= 2)
            {
                int count = dir.tdir_count;

                if (m_header.tiff_magic == TIFF_BIGENDIAN)
                {
                    if (count == 2)
                        v[1] = (short)(dir.tdir_offset & 0xffff);

                    if (count >= 1)
                        v[0] = (short)(dir.tdir_offset >> 16);
                }
                else
                {
                    if (count == 2)
                        v[1] = (short)(dir.tdir_offset >> 16);

                    if (count >= 1)
                        v[0] = (short)(dir.tdir_offset & 0xffff);
                }

                return true;
            }

            int cc = dir.tdir_count * sizeof(short);
            byte[] b = new byte[cc];
            int read = fetchData(dir, b);
            if (read != 0)
                Buffer.BlockCopy(b, 0, v, 0, b.Length);

            return (read != 0);
        }

        private bool fetchShortPair(TiffDirEntry dir)
        {
            if (dir.tdir_count > 2)
            {
                return false;
            }

            switch (dir.tdir_type)
            {
                case TiffType.BYTE:
                case TiffType.SBYTE:
                    byte[] bytes = new byte[4];
                    return fetchByteArray(dir, bytes) && SetField(dir.tdir_tag, bytes[0], bytes[1]);

                case TiffType.SHORT:
                case TiffType.SSHORT:
                    short[] shorts = new short[2];
                    return fetchShortArray(dir, shorts) && SetField(dir.tdir_tag, shorts[0], shorts[1]);
            }

            return false;
        }

        /// <summary>
        /// Fetches an array of LONG or SLONG values.
        /// </summary>
        private bool fetchLongArray(TiffDirEntry dir, int[] v)
        {
            if (dir.tdir_count == 1)
            {
                v[0] = (int)dir.tdir_offset;
                return true;
            }

            int cc = dir.tdir_count * sizeof(int);
            byte[] b = new byte[cc];
            int read = fetchData(dir, b);
            if (read != 0)
                Buffer.BlockCopy(b, 0, v, 0, b.Length);

            return (read != 0);
        }

        /// <summary>
        /// Fetch an array of RATIONAL or SRATIONAL values.
        /// </summary>
        private bool fetchRationalArray(TiffDirEntry dir, float[] v)
        {
            bool ok = false;
            byte[] l = new byte[dir.tdir_count * DataWidth(dir.tdir_type)];
            if (fetchData(dir, l) != 0)
            {
                int offset = 0;
                int[] pair = new int[2];
                for (int i = 0; i < dir.tdir_count; i++)
                {
                    pair[0] = readInt(l, offset);
                    offset += sizeof(int);
                    pair[1] = readInt(l, offset);
                    offset += sizeof(int);

                    ok = cvtRational(dir, pair[0], pair[1], out v[i]);
                    if (!ok)
                        break;
                }
            }

            return ok;
        }

        /// <summary>
        /// Fetches an array of FLOAT values.
        /// </summary>
        private bool fetchFloatArray(TiffDirEntry dir, float[] v)
        {
            if (dir.tdir_count == 1)
            {
                v[0] = BitConverter.ToSingle(BitConverter.GetBytes(dir.tdir_offset), 0);
                return true;
            }

            int w = DataWidth(dir.tdir_type);
            int cc = dir.tdir_count * w;
            byte[] b = new byte[cc];
            int read = fetchData(dir, b);
            if (read != 0)
            {
                int byteOffset = 0;
                for (int i = 0; i < read / 4; i++)
                {
                    v[i] = BitConverter.ToSingle(b, byteOffset);
                    byteOffset += 4;
                }
            }

            return (read != 0);
        }

        /// <summary>
        /// Fetches an array of DOUBLE values.
        /// </summary>
        private bool fetchDoubleArray(TiffDirEntry dir, double[] v)
        {
            int w = DataWidth(dir.tdir_type);
            int cc = dir.tdir_count * w;
            byte[] b = new byte[cc];
            int read = fetchData(dir, b);
            if (read != 0)
            {
                int byteOffset = 0;
                for (int i = 0; i < read / 8; i++)
                {
                    v[i] = BitConverter.ToDouble(b, byteOffset);
                    byteOffset += 8;
                }
            }

            return (read != 0);
        }

        /// <summary>
        /// Fetches an array of ANY values.
        /// </summary>
        private bool fetchAnyArray(TiffDirEntry dir, double[] v)
        {
            int i = 0;
            bool res = false;
            switch (dir.tdir_type)
            {
                case TiffType.BYTE:
                case TiffType.SBYTE:
                    byte[] b = new byte[dir.tdir_count];
                    res = fetchByteArray(dir, b);
                    if (res)
                    {
                        for (i = dir.tdir_count - 1; i >= 0; i--)
                            v[i] = b[i];
                    }

                    if (!res)
                        return false;

                    break;
                case TiffType.SHORT:
                case TiffType.SSHORT:
                    short[] u = new short[dir.tdir_count];
                    res = fetchShortArray(dir, u);
                    if (res)
                    {
                        for (i = dir.tdir_count - 1; i >= 0; i--)
                            v[i] = u[i];
                    }

                    if (!res)
                        return false;

                    break;
                case TiffType.LONG:
                case TiffType.SLONG:
                    int[] l = new int[dir.tdir_count];
                    res = fetchLongArray(dir, l);
                    if (res)
                    {
                        for (i = dir.tdir_count - 1; i >= 0; i--)
                            v[i] = l[i];
                    }

                    if (!res)
                        return false;

                    break;
                case TiffType.RATIONAL:
                case TiffType.SRATIONAL:
                    float[] r = new float[dir.tdir_count];
                    res = fetchRationalArray(dir, r);
                    if (res)
                    {
                        for (i = dir.tdir_count - 1; i >= 0; i--)
                            v[i] = r[i];
                    }

                    if (!res)
                        return false;

                    break;
                case TiffType.FLOAT:
                    float[] f = new float[dir.tdir_count];
                    res = fetchFloatArray(dir, f);
                    if (res)
                    {
                        for (i = dir.tdir_count - 1; i >= 0; i--)
                            v[i] = f[i];
                    }

                    if (!res)
                        return false;

                    break;
                case TiffType.DOUBLE:
                    return fetchDoubleArray(dir, v);
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Fetches a tag that is not handled by special case code.
        /// </summary>
        private bool fetchNormalTag(TiffDirEntry dir)
        {
            bool ok = false;
            TiffFieldInfo fip = FieldWithTag(dir.tdir_tag);

            if (dir.tdir_count > 1)
            {
                switch (dir.tdir_type)
                {
                    case TiffType.BYTE:
                    case TiffType.SBYTE:
                        byte[] bytes = new byte[dir.tdir_count];
                        ok = fetchByteArray(dir, bytes);
                        if (ok)
                        {
                            if (fip.PassCount)
                                ok = SetField(dir.tdir_tag, dir.tdir_count, bytes);
                            else
                                ok = SetField(dir.tdir_tag, bytes);
                        }
                        break;

                    case TiffType.SHORT:
                    case TiffType.SSHORT:
                        short[] shorts = new short[dir.tdir_count];
                        ok = fetchShortArray(dir, shorts);
                        if (ok)
                        {
                            if (fip.PassCount)
                                ok = SetField(dir.tdir_tag, dir.tdir_count, shorts);
                            else
                                ok = SetField(dir.tdir_tag, shorts);
                        }
                        break;

                    case TiffType.LONG:
                    case TiffType.SLONG:
                        int[] ints = new int[dir.tdir_count];
                        ok = fetchLongArray(dir, ints);
                        if (ok)
                        {
                            if (fip.PassCount)
                                ok = SetField(dir.tdir_tag, dir.tdir_count, ints);
                            else
                                ok = SetField(dir.tdir_tag, ints);
                        }
                        break;

                    case TiffType.RATIONAL:
                    case TiffType.SRATIONAL:
                        float[] rs = new float[dir.tdir_count];
                        ok = fetchRationalArray(dir, rs);
                        if (ok)
                        {
                            if (fip.PassCount)
                                ok = SetField(dir.tdir_tag, dir.tdir_count, rs);
                            else
                                ok = SetField(dir.tdir_tag, rs);
                        }
                        break;

                    case TiffType.FLOAT:
                        float[] fs = new float[dir.tdir_count];
                        ok = fetchFloatArray(dir, fs);
                        if (ok)
                        {
                            if (fip.PassCount)
                                ok = SetField(dir.tdir_tag, dir.tdir_count, fs);
                            else
                                ok = SetField(dir.tdir_tag, fs);
                        }
                        break;

                    case TiffType.DOUBLE:
                        double[] ds = new double[dir.tdir_count];
                        ok = fetchDoubleArray(dir, ds);
                        if (ok)
                        {
                            if (fip.PassCount)
                                ok = SetField(dir.tdir_tag, dir.tdir_count, ds);
                            else
                                ok = SetField(dir.tdir_tag, ds);
                        }
                        break;

                    case TiffType.ASCII:
                    case TiffType.UNDEFINED:
                        string cp;
                        ok = fetchString(dir, out cp) != 0;
                        if (ok)
                        {
                            if (fip.PassCount)
                                ok = SetField(dir.tdir_tag, dir.tdir_count, cp);
                            else
                                ok = SetField(dir.tdir_tag, cp);
                        }
                        break;
                }
            }
            else if (checkDirCount(dir, 1))
            {
                int v32 = 0;
                // singleton value
                switch (dir.tdir_type)
                {
                    case TiffType.BYTE:
                    case TiffType.SBYTE:
                    case TiffType.SHORT:
                    case TiffType.SSHORT:
                        TiffType type = fip.Type;
                        if (type != TiffType.LONG && type != TiffType.SLONG)
                        {
                            short v = (short)extractData(dir);
                            if (fip.PassCount)
                            {
                                short[] a = new short[1];
                                a[0] = v;
                                ok = SetField(dir.tdir_tag, 1, a);
                            }
                            else
                                ok = SetField(dir.tdir_tag, v);

                            break;
                        }

                        v32 = extractData(dir);
                        if (fip.PassCount)
                        {
                            int[] a = new int[1];
                            a[0] = (int)v32;
                            ok = SetField(dir.tdir_tag, 1, a);
                        }
                        else
                            ok = SetField(dir.tdir_tag, v32);

                        break;

                    case TiffType.LONG:
                    case TiffType.SLONG:
                    case TiffType.IFD:
                        v32 = extractData(dir);
                        if (fip.PassCount)
                        {
                            int[] a = new int[1];
                            a[0] = (int)v32;
                            ok = SetField(dir.tdir_tag, 1, a);
                        }
                        else
                            ok = SetField(dir.tdir_tag, v32);

                        break;

                    case TiffType.RATIONAL:
                    case TiffType.SRATIONAL:
                    case TiffType.FLOAT:
                        float f = (dir.tdir_type == TiffType.FLOAT ? fetchFloat(dir) : fetchRational(dir));
                        if (fip.PassCount)
                        {
                            float[] a = new float[1];
                            a[0] = f;
                            ok = SetField(dir.tdir_tag, 1, a);
                        }
                        else
                            ok = SetField(dir.tdir_tag, f);

                        break;

                    case TiffType.DOUBLE:
                        double[] ds = new double[1];
                        ok = fetchDoubleArray(dir, ds);
                        if (ok)
                        {
                            if (fip.PassCount)
                                ok = SetField(dir.tdir_tag, 1, ds);
                            else
                                ok = SetField(dir.tdir_tag, ds[0]);
                        }
                        break;

                    case TiffType.ASCII:
                    case TiffType.UNDEFINED:
                        // bit of a cheat...
                        string c;
                        ok = fetchString(dir, out c) != 0;
                        if (ok)
                        {
                            if (fip.PassCount)
                                ok = SetField(dir.tdir_tag, 1, c);
                            else
                                ok = SetField(dir.tdir_tag, c);
                        }
                        break;
                }
            }

            return ok;
        }

        /// <summary>
        /// Fetches samples/pixel short values for the specified tag and verify
        /// that all values are the same.
        /// </summary>
        private bool fetchPerSampleShorts(TiffDirEntry dir, out short pl)
        {
            pl = 0;
            short samples = m_dir.td_samplesperpixel;
            bool status = false;

            if (checkDirCount(dir, samples))
            {
                short[] v = new short[dir.tdir_count];
                if (fetchShortArray(dir, v))
                {
                    int check_count = dir.tdir_count;
                    if (samples < check_count)
                        check_count = samples;

                    bool failed = false;
                    for (ushort i = 1; i < check_count; i++)
                    {
                        if (v[i] != v[0])
                        {
                            failed = true;
                            break;
                        }
                    }

                    if (!failed)
                    {
                        pl = v[0];
                        status = true;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Fetches samples/pixel long values for the specified tag and verify
        /// that all values are the same.
        /// </summary>
        private bool fetchPerSampleLongs(TiffDirEntry dir, out int pl)
        {
            pl = 0;
            short samples = m_dir.td_samplesperpixel;
            bool status = false;

            if (checkDirCount(dir, samples))
            {
                int[] v = new int[dir.tdir_count];
                if (fetchLongArray(dir, v))
                {
                    int check_count = dir.tdir_count;
                    if (samples < check_count)
                        check_count = samples;

                    bool failed = false;
                    for (ushort i = 1; i < check_count; i++)
                    {
                        if (v[i] != v[0])
                        {
                            failed = true;
                            break;
                        }
                    }

                    if (!failed)
                    {
                        pl = (int)v[0];
                        status = true;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Fetches samples/pixel ANY values for the specified tag and verify
        /// that all values are the same.
        /// </summary>
        private bool fetchPerSampleAnys(TiffDirEntry dir, out double pl)
        {
            pl = 0;
            short samples = m_dir.td_samplesperpixel;
            bool status = false;

            if (checkDirCount(dir, samples))
            {
                double[] v = new double[dir.tdir_count];
                if (fetchAnyArray(dir, v))
                {
                    int check_count = dir.tdir_count;
                    if (samples < check_count)
                        check_count = samples;

                    bool failed = false;
                    for (ushort i = 1; i < check_count; i++)
                    {
                        if (v[i] != v[0])
                        {
                            failed = true;
                            break;
                        }
                    }

                    if (!failed)
                    {
                        pl = v[0];
                        status = true;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Fetches a set of offsets or lengths.
        /// </summary>
        private bool fetchStripThing(TiffDirEntry dir, int nstrips, ref int[] lpp)
        {
            checkDirCount(dir, nstrips);

            if (lpp == null)
                lpp = new int[nstrips];
            else
                Array.Clear(lpp, 0, lpp.Length);

            bool status = false;
            if (dir.tdir_type == TiffType.SHORT)
            {
                short[] dp = new short[dir.tdir_count];
                status = fetchShortArray(dir, dp);
                if (status)
                {
                    for (int i = 0; i < nstrips && i < dir.tdir_count; i++)
                        lpp[i] = dp[i];
                }
            }
            else if (nstrips != dir.tdir_count)
            {
                // Special case to correct length
                int[] dp = new int[dir.tdir_count];
                status = fetchLongArray(dir, dp);
                if (status)
                {
                    for (int i = 0; i < nstrips && i < dir.tdir_count; i++)
                        lpp[i] = dp[i];
                }
            }
            else
            {
                status = fetchLongArray(dir, lpp);
            }

            return status;
        }

        private bool fetchStripThing(TiffDirEntry dir, int nstrips, ref uint[] lpp)
        {
            int[] temp = null;
            if (lpp != null)
                temp = new int[lpp.Length];

            bool res = fetchStripThing(dir, nstrips, ref temp);
            if (res)
            {
                if (lpp == null)
                    lpp = new uint[temp.Length];

                Buffer.BlockCopy(temp, 0, lpp, 0, temp.Length * sizeof(uint));
            }

            return res;
        }

        /// <summary>
        /// Fetches and sets the RefBlackWhite tag.
        /// </summary>
        private bool fetchRefBlackWhite(TiffDirEntry dir)
        {
            if (dir.tdir_type == TiffType.RATIONAL)
            {
                bool res = fetchNormalTag(dir);
                if (res)
                {
                    for (int i = 0; i < m_dir.td_refblackwhite.Length; i++)
                    {
                        if (m_dir.td_refblackwhite[i] > 1)
                            return true;
                    }
                }
            }

            dir.tdir_type = TiffType.LONG;
            int[] cp = new int[dir.tdir_count];
            bool ok = fetchLongArray(dir, cp);
            dir.tdir_type = TiffType.RATIONAL;

            if (ok)
            {
                float[] fp = new float[dir.tdir_count];
                for (int i = 0; i < dir.tdir_count; i++)
                    fp[i] = (float)cp[i];

                ok = SetField(dir.tdir_tag, fp);
            }

            return ok;
        }

        /// <summary>
        /// Replace a single strip (tile) of uncompressed data with multiple
        /// strips (tiles), each approximately 8Kbytes.
        /// </summary>
        private void chopUpSingleUncompressedStrip()
        {
            uint bytecount = m_dir.td_stripbytecount[0];
            uint offset = m_dir.td_stripoffset[0];

            int rowbytes = VTileSize(1);
            uint stripbytes;
            int rowsperstrip;
            if (rowbytes > STRIP_SIZE_DEFAULT)
            {
                stripbytes = (uint)rowbytes;
                rowsperstrip = 1;
            }
            else if (rowbytes > 0)
            {
                rowsperstrip = STRIP_SIZE_DEFAULT / rowbytes;
                stripbytes = (uint)(rowbytes * rowsperstrip);
            }
            else
            {
                return;
            }

            // never increase the number of strips in an image
            if (rowsperstrip >= m_dir.td_rowsperstrip)
                return;

            uint nstrips = howMany(bytecount, stripbytes);
            if (nstrips == 0)
            {
                // something is wonky, do nothing.
                return;
            }

            uint[] newcounts = new uint[nstrips];
            uint[] newoffsets = new uint[nstrips];

            for (int strip = 0; strip < nstrips; strip++)
            {
                if (stripbytes > bytecount)
                    stripbytes = bytecount;

                newcounts[strip] = stripbytes;
                newoffsets[strip] = offset;
                offset += stripbytes;
                bytecount -= stripbytes;
            }

            m_dir.td_nstrips = (int)nstrips;
            m_dir.td_stripsperimage = (int)nstrips;
            SetField(TiffTag.ROWSPERSTRIP, rowsperstrip);

            m_dir.td_stripbytecount = newcounts;
            m_dir.td_stripoffset = newoffsets;
            m_dir.td_stripbytecountsorted = true;
        }

        internal static int roundUp(int x, int y)
        {
            return (howMany(x, y) * y);
        }

        internal static int howMany(int x, int y)
        {
            long res = (((long)x + ((long)y - 1)) / (long)y);
            if (res > int.MaxValue)
                return 0;

            return (int)res;
        }

        internal static uint howMany(uint x, uint y)
        {
            long res = (((long)x + ((long)y - 1)) / (long)y);
            if (res > uint.MaxValue)
                return 0;

            return (uint)res;
        }

        private static readonly uint[] typemask = 
        {
            0,              // TIFF_NOTYPE
            0x000000ff,     // TIFF_BYTE
            0xffffffff,     // TIFF_ASCII
            0x0000ffff,     // TIFF_SHORT
            0xffffffff,     // TIFF_LONG
            0xffffffff,     // TIFF_RATIONAL
            0x000000ff,     // TIFF_SBYTE
            0x000000ff,     // TIFF_UNDEFINED
            0x0000ffff,     // TIFF_SSHORT
            0xffffffff,     // TIFF_SLONG
            0xffffffff,     // TIFF_SRATIONAL
            0xffffffff,     // TIFF_FLOAT
            0xffffffff,     // TIFF_DOUBLE
            0xffffffff,     // TIFF_IFD
        };

        private static readonly int[] bigTypeshift = 
        {
            0,      // TIFF_NOTYPE
            24,     // TIFF_BYTE
            0,      // TIFF_ASCII
            16,     // TIFF_SHORT
            0,      // TIFF_LONG
            0,      // TIFF_RATIONAL
            24,     // TIFF_SBYTE
            24,     // TIFF_UNDEFINED
            16,     // TIFF_SSHORT
            0,      // TIFF_SLONG
            0,      // TIFF_SRATIONAL
            0,      // TIFF_FLOAT
            0,      // TIFF_DOUBLE
            0,      // TIFF_IFD
        };

        private static readonly int[] litTypeshift = 
        {
            0,  // TIFF_NOTYPE
            0,  // TIFF_BYTE
            0,  // TIFF_ASCII
            0,  // TIFF_SHORT
            0,  // TIFF_LONG
            0,  // TIFF_RATIONAL
            0,  // TIFF_SBYTE
            0,  // TIFF_UNDEFINED
            0,  // TIFF_SSHORT
            0,  // TIFF_SLONG
            0,  // TIFF_SRATIONAL
            0,  // TIFF_FLOAT
            0,  // TIFF_DOUBLE
            0,  // TIFF_IFD
        };

        private void initOrder(int magic)
        {
            m_typemask = typemask;
            if (magic == TIFF_BIGENDIAN)
            {
                m_typeshift = bigTypeshift;
                m_flags |= TiffFlags.SWAB;
            }
            else
            {
                m_typeshift = litTypeshift;
            }
        }

        private static int getMode(string mode, string module, out FileMode m, out FileAccess a)
        {
            m = 0;
            a = 0;
            int tiffMode = -1;

            if (mode.Length == 0)
                return tiffMode;

            switch (mode[0])
            {
                case 'r':
                    m = FileMode.Open;
                    a = FileAccess.Read;
                    tiffMode = O_RDONLY;
                    if (mode.Length > 1 && mode[1] == '+')
                    {
                        a = FileAccess.ReadWrite;
                        tiffMode = O_RDWR;
                    }
                    break;

                case 'w':
                    m = FileMode.Create;
                    a = FileAccess.ReadWrite;
                    tiffMode = O_RDWR | O_CREAT | O_TRUNC;
                    break;

                case 'a':
                    m = FileMode.Open;
                    a = FileAccess.ReadWrite;
                    tiffMode = O_RDWR | O_CREAT;
                    break;

                default:
                    break;
            }

            return tiffMode;
        }

        /// <summary>
        /// undefined state
        /// </summary>
        private const int NOSTRIP = -1;

        /// <summary>
        /// undefined state
        /// </summary>
        private const int NOTILE = -1;

        internal const int O_RDONLY = 0;
        internal const int O_WRONLY = 0x0001;
        internal const int O_CREAT = 0x0100;
        internal const int O_TRUNC = 0x0200;
        internal const int O_RDWR = 0x0002;

        //
        // Default Read/Seek/Write definitions.
        //

        private int readFile(byte[] buf, int offset, int size)
        {
            return m_stream.Read(m_clientdata, buf, offset, size);
        }

        private long seekFile(long off, SeekOrigin whence)
        {
            return m_stream.Seek(m_clientdata, off, whence);
        }

        private long getFileSize()
        {
            return m_stream.Size(m_clientdata);
        }

        private bool readOK(byte[] buf, int size)
        {
            return (readFile(buf, 0, size) == size);
        }

        private bool readShortOK(out short value)
        {
            byte[] bytes = new byte[2];
            bool res = readOK(bytes, 2);
            value = 0;
            if (res)
            {
                value = (short)(bytes[0] & 0xFF);
                value += (short)((bytes[1] & 0xFF) << 8);
            }

            return res;
        }

        private bool readUIntOK(out uint value)
        {
            int temp;
            bool res = readIntOK(out temp);
            if (res)
                value = (uint)temp;
            else
                value = 0;

            return res;
        }

        private bool readIntOK(out int value)
        {
            byte[] cp = new byte[4];
            bool res = readOK(cp, 4);
            value = 0;
            if (res)
            {
                value = cp[0] & 0xFF;
                value += (cp[1] & 0xFF) << 8;
                value += (cp[2] & 0xFF) << 16;
                value += cp[3] << 24;
            }

            return res;
        }

        private bool readDirEntryOk(TiffDirEntry[] dir, short dircount)
        {
            int entrySize = sizeof(short) * 2 + sizeof(int) * 2;
            int totalSize = entrySize * dircount;
            byte[] bytes = new byte[totalSize];
            bool res = readOK(bytes, totalSize);
            if (res)
                readDirEntry(dir, dircount, bytes, 0);

            return res;
        }

        private static void readDirEntry(TiffDirEntry[] dir, short dircount, byte[] bytes, int offset)
        {
            int pos = offset;
            for (int i = 0; i < dircount; i++)
            {
                TiffDirEntry entry = new TiffDirEntry();
                entry.tdir_tag = (TiffTag)(ushort)readShort(bytes, pos);
                pos += sizeof(short);
                entry.tdir_type = (TiffType)readShort(bytes, pos);
                pos += sizeof(short);
                entry.tdir_count = readInt(bytes, pos);
                pos += sizeof(int);
                entry.tdir_offset = (uint)readInt(bytes, pos);
                pos += sizeof(int);
                dir[i] = entry;
            }
        }

        private bool readHeaderOk(ref Syncfusion.Pdf.Compression.JBIG2.Internal.TiffHeader header)
        {
            bool res = readShortOK(out header.tiff_magic);

            if (res)
                res = readShortOK(out header.tiff_version);

            if (res)
                res = readUIntOK(out header.tiff_diroff);

            return res;
        }

        private bool seekOK(long off)
        {
            return (seekFile(off, SeekOrigin.Begin) == off);
        }

        private bool seek(int row, short sample)
        {
            if (row >= m_dir.td_imagelength)
            {
                return false;
            }

            int strip;
            if (m_dir.td_planarconfig == PlanarConfig.SEPARATE)
            {
                if (sample >= m_dir.td_samplesperpixel)
                {
                    return false;
                }

                if (m_dir.td_rowsperstrip != -1)
                    strip = sample * m_dir.td_stripsperimage + row / m_dir.td_rowsperstrip;
                else
                    strip = 0;
            }
            else
            {
                if (m_dir.td_rowsperstrip != -1)
                    strip = row / m_dir.td_rowsperstrip;
                else
                    strip = 0;
            }

            if (strip != m_curstrip)
            {
                /* different strip, refill */
                if (!fillStrip(strip))
                    return false;
            }
            else if (row < m_row)
            {
                if (!startStrip(strip))
                    return false;
            }

            if (row != m_row)
            {
                if (!m_currentCodec.Seek(row - m_row))
                    return false;

                m_row = row;
            }

            return true;
        }

        private int readRawStrip1(int strip, byte[] buf, int offset, int size, string module)
        {
            if (!seekOK(m_dir.td_stripoffset[strip]))
            {
                return -1;
            }

            int cc = readFile(buf, offset, size);
            if (cc != size)
            {
                return -1;
            }

            return size;
        }

        private int readRawTile1(int tile, byte[] buf, int offset, int size, string module)
        {
            if (!seekOK(m_dir.td_stripoffset[tile]))
            {
                return -1;
            }

            int cc = readFile(buf, offset, size);
            if (cc != size)
            {
                return -1;
            }

            return size;
        }

        /// <summary>
        /// Set state to appear as if a strip has just been read in.
        /// </summary>
        private bool startStrip(int strip)
        {
            if ((m_flags & TiffFlags.CODERSETUP) != TiffFlags.CODERSETUP)
            {
                if (!m_currentCodec.SetupDecode())
                    return false;

                m_flags |= TiffFlags.CODERSETUP;
            }

            m_curstrip = strip;
            m_row = (strip % m_dir.td_stripsperimage) * m_dir.td_rowsperstrip;
            m_rawcp = 0;

            if ((m_flags & TiffFlags.NOREADRAW) == TiffFlags.NOREADRAW)
                m_rawcc = 0;
            else
                m_rawcc = (int)m_dir.td_stripbytecount[strip];

            return m_currentCodec.PreDecode((short)(strip / m_dir.td_stripsperimage));
        }

        private bool startTile(int tile)
        {
            if ((m_flags & TiffFlags.CODERSETUP) != TiffFlags.CODERSETUP)
            {
                if (!m_currentCodec.SetupDecode())
                    return false;

                m_flags |= TiffFlags.CODERSETUP;
            }

            m_curtile = tile;
            m_row = (tile % howMany(m_dir.td_imagewidth, m_dir.td_tilewidth)) * m_dir.td_tilelength;
            m_col = (tile % howMany(m_dir.td_imagelength, m_dir.td_tilelength)) * m_dir.td_tilewidth;
            m_rawcp = 0;

            if ((m_flags & TiffFlags.NOREADRAW) == TiffFlags.NOREADRAW)
                m_rawcc = 0;
            else
                m_rawcc = (int)m_dir.td_stripbytecount[tile];

            return m_currentCodec.PreDecode((short)(tile / m_dir.td_stripsperimage));
        }

        private bool checkRead(bool tiles)
        {
            if (m_mode == O_WRONLY)
            {
                return false;
            }

            if (tiles ^ IsTiled())
            {
                return false;
            }

            return true;
        }

        private static void swab16BitData(byte[] buffer, int offset, int count)
        {
            short[] swabee = ByteArrayToShorts(buffer, offset, count);
            SwabArrayOfShort(swabee, count / 2);
            ShortsToByteArray(swabee, 0, count / 2, buffer, offset);
        }

        private static void swab24BitData(byte[] buffer, int offset, int count)
        {
            SwabArrayOfTriples(buffer, offset, count / 3);
        }

        private static void swab32BitData(byte[] buffer, int offset, int count)
        {
            int[] swabee = ByteArrayToInts(buffer, offset, count);
            SwabArrayOfLong(swabee, count / 4);
            IntsToByteArray(swabee, 0, count / 4, buffer, offset);
        }

        private static void swab64BitData(byte[] buffer, int offset, int count)
        {
            int doubleCount = count / 8;
            double[] doubles = new double[doubleCount];
            int byteOffset = offset;
            for (int i = 0; i < doubleCount; i++)
            {
                doubles[i] = BitConverter.ToDouble(buffer, byteOffset);
                byteOffset += 8;
            }

            SwabArrayOfDouble(doubles, doubleCount);

            byteOffset = offset;
            for (int i = 0; i < doubleCount; i++)
            {
                byte[] bytes = BitConverter.GetBytes(doubles[i]);
                Buffer.BlockCopy(bytes, 0, buffer, byteOffset, bytes.Length);
                byteOffset += bytes.Length;
            }
        }

        /// <summary>
        /// Read the specified strip and setup for decoding.
        /// The data buffer is expanded, as necessary, to hold the strip's data.
        /// </summary>
        internal bool fillStrip(int strip)
        {
            const string module = "fillStrip";

            if ((m_flags & TiffFlags.NOREADRAW) != TiffFlags.NOREADRAW)
            {
                int bytecount = (int)m_dir.td_stripbytecount[strip];
                if (bytecount <= 0)
                {
                    return false;
                }

                if (bytecount > m_rawdatasize)
                {
                    m_curstrip = NOSTRIP;
                    if ((m_flags & TiffFlags.MYBUFFER) != TiffFlags.MYBUFFER)
                    {
                        return false;
                    }

                    ReadBufferSetup(null, roundUp(bytecount, 1024));
                }

                if (readRawStrip1(strip, m_rawdata, 0, bytecount, module) != bytecount)
                    return false;

                if (!isFillOrder(m_dir.td_fillorder) && (m_flags & TiffFlags.NOBITREV) != TiffFlags.NOBITREV)
                    ReverseBits(m_rawdata, bytecount);
            }

            return startStrip(strip);
        }

        /// <summary>
        /// Read the specified tile and setup for decoding. 
        /// The data buffer is expanded, as necessary, to hold the tile's data.
        /// </summary>
        internal bool fillTile(int tile)
        {
            const string module = "fillTile";

            if ((m_flags & TiffFlags.NOREADRAW) != TiffFlags.NOREADRAW)
            {
                int bytecount = (int)m_dir.td_stripbytecount[tile];
                if (bytecount <= 0)
                {
                    return false;
                }

                if (bytecount > m_rawdatasize)
                {
                    m_curtile = NOTILE;
                    if ((m_flags & TiffFlags.MYBUFFER) != TiffFlags.MYBUFFER)
                    {
                        return false;
                    }

                    ReadBufferSetup(null, roundUp(bytecount, 1024));
                }

                if (readRawTile1(tile, m_rawdata, 0, bytecount, module) != bytecount)
                    return false;

                if (!isFillOrder(m_dir.td_fillorder) && (m_flags & TiffFlags.NOBITREV) != TiffFlags.NOBITREV)
                    ReverseBits(m_rawdata, bytecount);
            }

            return startTile(tile);
        }

        private int summarize(int summand1, int summand2, string where)
        {
            int bytes = summand1 + summand2;
            if (bytes - summand1 != summand2)
            {
                bytes = 0;
            }

            return bytes;
        }

        private int multiply(int nmemb, int elem_size, string where)
        {
            int bytes = nmemb * elem_size;
            if (elem_size != 0 && bytes / elem_size != nmemb)
            {
                bytes = 0;
            }

            return bytes;
        }

        internal int newScanlineSize()
        {
            int scanline;
            if (m_dir.td_planarconfig == PlanarConfig.CONTIG)
            {
                if (m_dir.td_photometric == Photometric.YCBCR && !IsUpSampled())
                {
                    FieldValue[] result = GetField(TiffTag.YCBCRSUBSAMPLING);
                    ushort ycbcrsubsampling0 = result[0].ToUShort();
                    ushort ycbcrsubsampling1 = result[1].ToUShort();

                    if (ycbcrsubsampling0 * ycbcrsubsampling1 == 0)
                    {
                        return 0;
                    }

                    return ((((m_dir.td_imagewidth + ycbcrsubsampling0 - 1) / ycbcrsubsampling0) * (ycbcrsubsampling0 * ycbcrsubsampling1 + 2) * m_dir.td_bitspersample + 7) / 8) / ycbcrsubsampling1;
                }
                else
                {
                    scanline = multiply(m_dir.td_imagewidth, m_dir.td_samplesperpixel, "TIFFScanlineSize");
                }
            }
            else
            {
                scanline = m_dir.td_imagewidth;
            }

            return howMany8(multiply(scanline, m_dir.td_bitspersample, "TIFFScanlineSize"));
        }

        internal int oldScanlineSize()
        {
            int scanline = multiply(m_dir.td_bitspersample, m_dir.td_imagewidth, "TIFFScanlineSize");
            if (m_dir.td_planarconfig == PlanarConfig.CONTIG)
                scanline = multiply(scanline, m_dir.td_samplesperpixel, "TIFFScanlineSize");

            return howMany8(scanline);
        }

        private static readonly byte[] TIFFBitRevTable = 
        {
            0x00, 0x80, 0x40, 0xc0, 0x20, 0xa0, 0x60, 0xe0, 0x10, 0x90, 0x50, 0xd0, 
            0x30, 0xb0, 0x70, 0xf0, 0x08, 0x88, 0x48, 0xc8, 0x28, 0xa8, 0x68, 0xe8, 
            0x18, 0x98, 0x58, 0xd8, 0x38, 0xb8, 0x78, 0xf8, 0x04, 0x84, 0x44, 0xc4, 
            0x24, 0xa4, 0x64, 0xe4, 0x14, 0x94, 0x54, 0xd4, 0x34, 0xb4, 0x74, 0xf4, 
            0x0c, 0x8c, 0x4c, 0xcc, 0x2c, 0xac, 0x6c, 0xec, 0x1c, 0x9c, 0x5c, 0xdc, 
            0x3c, 0xbc, 0x7c, 0xfc, 0x02, 0x82, 0x42, 0xc2, 0x22, 0xa2, 0x62, 0xe2, 
            0x12, 0x92, 0x52, 0xd2, 0x32, 0xb2, 0x72, 0xf2, 0x0a, 0x8a, 0x4a, 0xca, 
            0x2a, 0xaa, 0x6a, 0xea, 0x1a, 0x9a, 0x5a, 0xda, 0x3a, 0xba, 0x7a, 0xfa, 
            0x06, 0x86, 0x46, 0xc6, 0x26, 0xa6, 0x66, 0xe6, 0x16, 0x96, 0x56, 0xd6, 
            0x36, 0xb6, 0x76, 0xf6, 0x0e, 0x8e, 0x4e, 0xce, 0x2e, 0xae, 0x6e, 0xee, 
            0x1e, 0x9e, 0x5e, 0xde, 0x3e, 0xbe, 0x7e, 0xfe, 0x01, 0x81, 0x41, 0xc1, 
            0x21, 0xa1, 0x61, 0xe1, 0x11, 0x91, 0x51, 0xd1, 0x31, 0xb1, 0x71, 0xf1, 
            0x09, 0x89, 0x49, 0xc9, 0x29, 0xa9, 0x69, 0xe9, 0x19, 0x99, 0x59, 0xd9, 
            0x39, 0xb9, 0x79, 0xf9, 0x05, 0x85, 0x45, 0xc5, 0x25, 0xa5, 0x65, 0xe5, 
            0x15, 0x95, 0x55, 0xd5, 0x35, 0xb5, 0x75, 0xf5, 0x0d, 0x8d, 0x4d, 0xcd, 
            0x2d, 0xad, 0x6d, 0xed, 0x1d, 0x9d, 0x5d, 0xdd, 0x3d, 0xbd, 0x7d, 0xfd, 
            0x03, 0x83, 0x43, 0xc3, 0x23, 0xa3, 0x63, 0xe3, 0x13, 0x93, 0x53, 0xd3, 
            0x33, 0xb3, 0x73, 0xf3, 0x0b, 0x8b, 0x4b, 0xcb, 0x2b, 0xab, 0x6b, 0xeb, 
            0x1b, 0x9b, 0x5b, 0xdb, 0x3b, 0xbb, 0x7b, 0xfb, 0x07, 0x87, 0x47, 0xc7, 
            0x27, 0xa7, 0x67, 0xe7, 0x17, 0x97, 0x57, 0xd7, 0x37, 0xb7, 0x77, 0xf7, 
            0x0f, 0x8f, 0x4f, 0xcf, 0x2f, 0xaf, 0x6f, 0xef, 0x1f, 0x9f, 0x5f, 0xdf, 
            0x3f, 0xbf, 0x7f, 0xff
        };

        private static readonly byte[] TIFFNoBitRevTable = 
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 
            0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 
            0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f, 0x20, 0x21, 0x22, 0x23, 
            0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 
            0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 
            0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 
            0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f, 
            0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 
            0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 
            0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f, 0x80, 0x81, 0x82, 0x83, 
            0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c, 0x8d, 0x8e, 0x8f, 
            0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 
            0x9c, 0x9d, 0x9e, 0x9f, 0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 
            0xa8, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xaf, 0xb0, 0xb1, 0xb2, 0xb3, 
            0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 0xbe, 0xbf, 
            0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 
            0xcc, 0xcd, 0xce, 0xcf, 0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 
            0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf, 0xe0, 0xe1, 0xe2, 0xe3, 
            0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef, 
            0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa, 0xfb, 
            0xfc, 0xfd, 0xfe, 0xff, 
        };
    }
}
