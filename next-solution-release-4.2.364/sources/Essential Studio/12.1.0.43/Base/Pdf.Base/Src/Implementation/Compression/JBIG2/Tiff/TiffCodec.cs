#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.IO;
using System;
using Syncfusion.Pdf.Compression.JBIG2.Internal;

namespace Syncfusion.Pdf.Compression.JBIG2
{
    /// <summary>
    /// Base class for all codecs within the library.
    /// </summary>
    class TiffCodec
    {
        /// <summary>
        /// An instance of <see cref="Tiff"/>.
        /// </summary>
        protected Tiff m_tif;

        /// <summary>
        /// Compression scheme this codec impelements.
        /// </summary>
        protected internal Compression m_scheme;

        /// <summary>
        /// Codec name.
        /// </summary>
        protected internal string m_name;

        /// <summary>
        /// Initializes a new instance of the <see cref="TiffCodec"/> class.
        /// </summary>
        public TiffCodec(Tiff tif, Compression scheme, string name)
        {
            m_scheme = scheme;
            m_tif = tif;
            m_name = name;
        }

        /// <summary>
        /// Gets a value indicating whether this codec can decode data.
        /// </summary>
        public virtual bool CanDecode
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual bool Init()
        {
            return true;
        }

        /// <summary>
        /// Setups the decoder part of the codec.
        /// </summary>
        public virtual bool SetupDecode()
        {
            return true;
        }

        /// <summary>
        /// Prepares the decoder part of the codec for a decoding.
        /// </summary>
        public virtual bool PreDecode(short plane)
        {
            return true;
        }

        /// <summary>
        /// Decodes one row of image data.
        /// </summary>
        public virtual bool DecodeRow(byte[] buffer, int offset, int count, short plane)
        {
            return noDecode("scanline");
        }

        /// <summary>
        /// Decodes one strip of image data.
        /// </summary>
        public virtual bool DecodeStrip(byte[] buffer, int offset, int count, short plane)
        {
            return noDecode("strip");
        }

        /// <summary>
        /// Decodes one tile of image data.
        /// </summary>
        public virtual bool DecodeTile(byte[] buffer, int offset, int count, short plane)
        {
            return noDecode("tile");
        }

        /// <summary>
        /// Flushes any internal data buffers and terminates current operation.
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// Seeks the specified row in the strip being processed.
        /// </summary>
        public virtual bool Seek(int row)
        {
            return false;
        }

        /// <summary>
        /// Cleanups the state of the codec.
        /// </summary>
        public virtual void Cleanup()
        {
        }

        /// <summary>
        /// Calculates and/or constrains a strip size.
        /// </summary>
        public virtual int DefStripSize(int size)
        {
            if (size < 1)
            {
                int scanline = m_tif.ScanlineSize();
                size = Tiff.STRIP_SIZE_DEFAULT / (scanline == 0 ? 1 : scanline);
                if (size == 0)
                {
                    // very wide images
                    size = 1;
                }
            }

            return size;
        }

        /// <summary>
        /// Calculate and/or constrains a tile size
        /// </summary>
        public virtual void DefTileSize(ref int width, ref int height)
        {
            if (width < 1)
                width = 256;
            
            if (height < 1)
                height = 256;
            
            if ((width & 0xf) != 0)
                width = Tiff.roundUp(width, 16);

            if ((height & 0xf) != 0)
                height = Tiff.roundUp(height, 16);
        }

        private bool noDecode(string method)
        {
            TiffCodec c = m_tif.FindCodec(m_tif.m_dir.td_compression);
            if (c != null)
            {
            }
            else
            {
            }

            return false;
        }
    }

    /// <summary>
    /// Represents a TIFF field information.
    /// </summary>
    class TiffFieldInfo
    {
        private TiffTag m_tag;
        private short m_readCount;
        private short m_writeCount;
        private TiffType m_type;
        private short m_bit;
        private bool m_okToChange;
        private bool m_passCount;
        private string m_name;

        /// <summary>
        /// marker for variable length tags
        /// </summary>
        public const short Variable = -1;

        /// <summary>
        /// marker for SamplesPerPixel-bound tags
        /// </summary>
        public const short Spp = -2;

        /// <summary>
        /// marker for integer variable length tags
        /// </summary>
        public const short Variable2 = -3;

        /// <summary>
        /// Initializes a new instance of the <see cref="TiffFieldInfo"/> class.
        /// </summary>
        public TiffFieldInfo(TiffTag tag, short readCount, short writeCount,
            TiffType type, short bit, bool okToChange, bool passCount, string name)
        {
            m_tag = tag;
            m_readCount = readCount;
            m_writeCount = writeCount;
            m_type = type;
            m_bit = bit;
            m_okToChange = okToChange;
            m_passCount = passCount;
            m_name = name;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            if (m_bit != FieldBit.Custom || m_name.Length == 0)
                return m_tag.ToString();

            return m_name;
        }

        /// <summary>
        /// The tag described by this instance.
        /// </summary>
        public TiffTag Tag
        {
            get { return m_tag; }
        }

        /// <summary>
        /// Number of values to read when reading field information or
        /// one of <see cref="Variable"/>, <see cref="Spp"/> and <see cref="Variable2"/>.
        /// </summary>
        public short ReadCount
        {
            get { return m_readCount; }
        }

        /// <summary>
        /// Number of values to write when writing field information or
        /// one of <see cref="Variable"/>, <see cref="Spp"/> and <see cref="Variable2"/>.
        /// </summary>
        public short WriteCount
        {
            get { return m_writeCount; }
        }

        /// <summary>
        /// Type of the field values.
        /// </summary>
        public TiffType Type
        {
            get { return m_type; }
        }

        /// <summary>
        /// Index of the bit to use in "Set Fields Vector" when this instance
        /// is merged into field info collection. Take a look at <see cref="FieldBit"/> class.
        /// </summary>
        public short Bit
        {
            get { return m_bit; }
        }

        /// <summary>
        /// If true, then it is permissible to set the tag's value even after writing has commenced.
        /// </summary>
        public bool OkToChange
        {
            get { return m_okToChange; }
        }

        /// <summary>
        /// If true, then number of value elements should be passed to <see cref="Tiff.SetField"/>
        /// method as second parameter (right after tag type AND before values itself).
        /// </summary>
        public bool PassCount
        {
            get { return m_passCount; }
        }

        /// <summary>
        /// The name (or description) of the tag this instance describes.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            internal set { m_name = value; }
        }
    }

    /// <summary>
    /// A stream used by the library for TIFF reading and writing.
    /// </summary>
    class TiffStream
    {
        /// <summary>
        /// Reads a sequence of bytes from the stream and advances the position within the stream
        /// by the number of bytes read.
        /// </summary>
        public virtual int Read(object clientData, byte[] buffer, int offset, int count)
        {
            Stream stream = clientData as Stream;
            if (stream == null)
                throw new ArgumentException("Can't get underlying stream to read from");

            return stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        public virtual long Seek(object clientData, long offset, SeekOrigin origin)
        {
            if (offset == -1)
                return -1; // was 0xFFFFFFFF

            Stream stream = clientData as Stream;
            if (stream == null)
                throw new ArgumentException("Can't get underlying stream to seek in");

            return stream.Seek(offset, origin);
        }

        /// <summary>
        /// Closes the current stream.
        /// </summary>
        public virtual void Close(object clientData)
        {
            Stream stream = clientData as Stream;
            if (stream == null)
                throw new ArgumentException("Can't get underlying stream to close");

            stream.Close();
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public virtual long Size(object clientData)
        {
            Stream stream = clientData as Stream;
            if (stream == null)
                throw new ArgumentException("Can't get underlying stream to retrieve size from");

            return stream.Length;
        }
    }

    /// <summary>
    /// Tiff tag methods.
    /// </summary>
    class TiffTagMethods
    {
        /// <summary>
        /// untyped data
        /// </summary>
        private const short DATATYPE_VOID = 0;

        /// <summary>
        /// signed integer data
        /// </summary>
        private const short DATATYPE_INT = 1;

        /// <summary>
        /// unsigned integer data
        /// </summary>
        private const short DATATYPE_UINT = 2;

        /// <summary>
        /// IEEE floating point data
        /// </summary>
        private const short DATATYPE_IEEEFP = 3;

        /// <summary>
        /// Sets the value(s) of a tag in a TIFF file/stream open for writing.
        /// </summary>
        public virtual bool SetField(Tiff tif, TiffTag tag, FieldValue[] value)
        {
            const string module = "vsetfield";

            TiffDirectory td = tif.m_dir;
            bool status = true;
            int v32 = 0;
            int v = 0;

            bool end = false;
            bool badvalue = false;
            bool badvalue32 = false;

            switch (tag)
            {
                case TiffTag.SUBFILETYPE:
                    td.td_subfiletype = (FileType)value[0].ToByte();
                    break;
                case TiffTag.IMAGEWIDTH:
                    td.td_imagewidth = value[0].ToInt();
                    break;
                case TiffTag.IMAGELENGTH:
                    td.td_imagelength = value[0].ToInt();
                    break;
                case TiffTag.BITSPERSAMPLE:
                    td.td_bitspersample = value[0].ToShort();
                    if ((tif.m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
                    {
                        if (td.td_bitspersample == 16)
                            tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmSwab16Bit;
                        else if (td.td_bitspersample == 24)
                            tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmSwab24Bit;
                        else if (td.td_bitspersample == 32)
                            tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmSwab32Bit;
                        else if (td.td_bitspersample == 64)
                            tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmSwab64Bit;
                        else if (td.td_bitspersample == 128)
                        {
                            tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmSwab64Bit;
                        }
                    }
                    break;
                case TiffTag.COMPRESSION:
                    v = value[0].ToInt() & 0xffff;
                    Compression comp = (Compression)v;
                    if (tif.fieldSet(FieldBit.Compression))
                    {
                        if (td.td_compression == comp)
                            break;

                        tif.m_currentCodec.Cleanup();
                        tif.m_flags &= ~TiffFlags.CODERSETUP;
                    }
                    status = tif.setCompressionScheme(comp);
                    if (status)
                        td.td_compression = comp;
                    else
                        status = false;
                    break;

                case TiffTag.PHOTOMETRIC:
                    td.td_photometric = (Photometric)value[0].ToInt();
                    break;
                case TiffTag.THRESHHOLDING:
                    td.td_threshholding = (Threshold)value[0].ToByte();
                    break;
                case TiffTag.FILLORDER:
                    v = value[0].ToInt();
                    FillOrder fo = (FillOrder)v;
                    if (fo != FillOrder.LSB2MSB && fo != FillOrder.MSB2LSB)
                    {
                        badvalue = true;
                        break;
                    }

                    td.td_fillorder = fo;
                    break;
                case TiffTag.ORIENTATION:
                    v = value[0].ToInt();
                    Orientation or = (Orientation)v;
                    if (or < Orientation.TOPLEFT || Orientation.LEFTBOT < or)
                    {
                        badvalue = true;
                        break;
                    }
                    else
                        td.td_orientation = or;
                    break;
                case TiffTag.SAMPLESPERPIXEL:
                    v = value[0].ToInt();
                    if (v == 0)
                    {
                        badvalue = true;
                        break;
                    }

                    td.td_samplesperpixel = (short)v;
                    break;
                case TiffTag.ROWSPERSTRIP:
                    v32 = value[0].ToInt();
                    if (v32 == 0)
                    {
                        badvalue32 = true;
                        break;
                    }

                    td.td_rowsperstrip = v32;
                    if (!tif.fieldSet(FieldBit.TileDimensions))
                    {
                        td.td_tilelength = v32;
                        td.td_tilewidth = td.td_imagewidth;
                    }
                    break;
                case TiffTag.MINSAMPLEVALUE:
                    td.td_minsamplevalue = value[0].ToUShort();
                    break;
                case TiffTag.MAXSAMPLEVALUE:
                    td.td_maxsamplevalue = value[0].ToUShort();
                    break;
                case TiffTag.SMINSAMPLEVALUE:
                    td.td_sminsamplevalue = value[0].ToDouble();
                    break;
                case TiffTag.SMAXSAMPLEVALUE:
                    td.td_smaxsamplevalue = value[0].ToDouble();
                    break;
                case TiffTag.XRESOLUTION:
                    td.td_xresolution = value[0].ToFloat();
                    break;
                case TiffTag.YRESOLUTION:
                    td.td_yresolution = value[0].ToFloat();
                    break;
                case TiffTag.PLANARCONFIG:
                    v = value[0].ToInt();
                    PlanarConfig pc = (PlanarConfig)v;
                    if (pc != PlanarConfig.CONTIG && pc != PlanarConfig.SEPARATE)
                    {
                        badvalue = true;
                        break;
                    }
                    td.td_planarconfig = pc;
                    break;
                case TiffTag.XPOSITION:
                    td.td_xposition = value[0].ToFloat();
                    break;
                case TiffTag.YPOSITION:
                    td.td_yposition = value[0].ToFloat();
                    break;
                case TiffTag.RESOLUTIONUNIT:
                    v = value[0].ToInt();
                    ResUnit ru = (ResUnit)v;
                    if (ru < ResUnit.NONE || ResUnit.CENTIMETER < ru)
                    {
                        badvalue = true;
                        break;
                    }

                    td.td_resolutionunit = ru;
                    break;
                case TiffTag.PAGENUMBER:
                    td.td_pagenumber[0] = value[0].ToShort();
                    td.td_pagenumber[1] = value[1].ToShort();
                    break;
                case TiffTag.HALFTONEHINTS:
                    td.td_halftonehints[0] = value[0].ToShort();
                    td.td_halftonehints[1] = value[1].ToShort();
                    break;
                case TiffTag.COLORMAP:
                    v32 = 1 << td.td_bitspersample;
                    Tiff.setShortArray(out td.td_colormap[0], value[0].ToShortArray(), v32);
                    Tiff.setShortArray(out td.td_colormap[1], value[1].ToShortArray(), v32);
                    Tiff.setShortArray(out td.td_colormap[2], value[2].ToShortArray(), v32);
                    break;
                case TiffTag.EXTRASAMPLES:
                    if (!setExtraSamples(td, ref v, value))
                    {
                        badvalue = true;
                        break;
                    }

                    break;
                case TiffTag.MATTEING:
                    if (value[0].ToShort() != 0)
                        td.td_extrasamples = 1;
                    else
                        td.td_extrasamples = 0;

                    if (td.td_extrasamples != 0)
                    {
                        td.td_sampleinfo = new ExtraSample[1];
                        td.td_sampleinfo[0] = ExtraSample.ASSOCALPHA;
                    }
                    break;
                case TiffTag.TILEWIDTH:
                    v32 = value[0].ToInt();
                    if ((v32 % 16) != 0)
                    {
                        if (tif.m_mode != Tiff.O_RDONLY)
                        {
                            badvalue32 = true;
                            break;
                        }
                    }
                    td.td_tilewidth = v32;
                    tif.m_flags |= TiffFlags.ISTILED;
                    break;
                case TiffTag.TILELENGTH:
                    v32 = value[0].ToInt();
                    if ((v32 % 16) != 0)
                    {
                        if (tif.m_mode != Tiff.O_RDONLY)
                        {
                            badvalue32 = true;
                            break;
                        }
                    }
                    td.td_tilelength = v32;
                    tif.m_flags |= TiffFlags.ISTILED;
                    break;
                case TiffTag.TILEDEPTH:
                    v32 = value[0].ToInt();
                    if (v32 == 0)
                    {
                        badvalue32 = true;
                        break;
                    }

                    td.td_tiledepth = v32;
                    break;
                case TiffTag.DATATYPE:
                    v = value[0].ToInt();
                    SampleFormat sf = SampleFormat.VOID;
                    switch (v)
                    {
                        case DATATYPE_VOID:
                            sf = SampleFormat.VOID;
                            break;
                        case DATATYPE_INT:
                            sf = SampleFormat.INT;
                            break;
                        case DATATYPE_UINT:
                            sf = SampleFormat.UINT;
                            break;
                        case DATATYPE_IEEEFP:
                            sf = SampleFormat.IEEEFP;
                            break;
                        default:
                            badvalue = true;
                            break;
                    }

                    if (!badvalue)
                        td.td_sampleformat = sf;

                    break;
                case TiffTag.SAMPLEFORMAT:
                    v = value[0].ToInt();
                    sf = (SampleFormat)v;
                    if (sf < SampleFormat.UINT || SampleFormat.COMPLEXIEEEFP < sf)
                    {
                        badvalue = true;
                        break;
                    }

                    td.td_sampleformat = sf;

                    if (td.td_sampleformat == SampleFormat.COMPLEXINT &&
                        td.td_bitspersample == 32 && tif.m_postDecodeMethod == Tiff.PostDecodeMethodType.pdmSwab32Bit)
                    {
                        tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmSwab16Bit;
                    }
                    else if ((td.td_sampleformat == SampleFormat.COMPLEXINT ||
                        td.td_sampleformat == SampleFormat.COMPLEXIEEEFP) &&
                        td.td_bitspersample == 64 && tif.m_postDecodeMethod == Tiff.PostDecodeMethodType.pdmSwab64Bit)
                    {
                        tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmSwab32Bit;
                    }
                    break;
                case TiffTag.IMAGEDEPTH:
                    td.td_imagedepth = value[0].ToInt();
                    break;
                case TiffTag.SUBIFD:
                    if ((tif.m_flags & TiffFlags.INSUBIFD) != TiffFlags.INSUBIFD)
                    {
                        td.td_nsubifd = value[0].ToShort();
                        Tiff.setLongArray(out td.td_subifd, value[1].ToIntArray(), td.td_nsubifd);
                    }
                    else
                    {
                        status = false;
                    }
                    break;
                case TiffTag.YCBCRPOSITIONING:
                    td.td_ycbcrpositioning = (YCbCrPosition)value[0].ToByte();
                    break;
                case TiffTag.YCBCRSUBSAMPLING:
                    td.td_ycbcrsubsampling[0] = value[0].ToShort();
                    td.td_ycbcrsubsampling[1] = value[1].ToShort();
                    break;
                case TiffTag.TRANSFERFUNCTION:
                    v = ((td.td_samplesperpixel - td.td_extrasamples) > 1 ? 3 : 1);
                    for (int i = 0; i < v; i++)
                    {
                        Tiff.setShortArray(out td.td_transferfunction[i], value[0].ToShortArray(), 1 << td.td_bitspersample);
                    }
                    break;
                case TiffTag.REFERENCEBLACKWHITE:
                    // XXX should check for null range
                    Tiff.setFloatArray(out td.td_refblackwhite, value[0].ToFloatArray(), 6);
                    break;
                case TiffTag.INKNAMES:
                    v = value[0].ToInt();
                    string s = value[1].ToString();
                    v = checkInkNamesString(tif, v, s);
                    status = v > 0;
                    if (v > 0)
                    {
                        setNString(out td.td_inknames, s, v);
                        td.td_inknameslen = v;
                    }
                    break;
                default:
                    TiffFieldInfo fip = tif.FindFieldInfo(tag, TiffType.ANY);
                    if (fip == null || fip.Bit != FieldBit.Custom)
                    {
                        status = false;
                        break;
                    }

                    int tvIndex = -1;
                    for (int iCustom = 0; iCustom < td.td_customValueCount; iCustom++)
                    {
                        if (td.td_customValues[iCustom].info.Tag == tag)
                        {
                            tvIndex = iCustom;
                            td.td_customValues[iCustom].value = null;
                            break;
                        }
                    }

                    if (tvIndex == -1)
                    {
                        td.td_customValueCount++;
                        TiffTagValue[] new_customValues = Tiff.Realloc(
                            td.td_customValues, td.td_customValueCount - 1, td.td_customValueCount);
                        td.td_customValues = new_customValues;

                        tvIndex = td.td_customValueCount - 1;
                        td.td_customValues[tvIndex].info = fip;
                        td.td_customValues[tvIndex].value = null;
                        td.td_customValues[tvIndex].count = 0;
                    }

                    int tv_size = Tiff.dataSize(fip.Type);
                    if (tv_size == 0)
                    {
                        status = false;
                        end = true;
                        break;
                    }

                    int paramIndex = 0;
                    if (fip.PassCount)
                    {
                        if (fip.WriteCount == TiffFieldInfo.Variable2)
                            td.td_customValues[tvIndex].count = value[paramIndex++].ToInt();
                        else
                            td.td_customValues[tvIndex].count = value[paramIndex++].ToInt();
                    }
                    else if (fip.WriteCount == TiffFieldInfo.Variable ||
                        fip.WriteCount == TiffFieldInfo.Variable2)
                    {
                        td.td_customValues[tvIndex].count = 1;
                    }
                    else if (fip.WriteCount == TiffFieldInfo.Spp)
                    {
                        td.td_customValues[tvIndex].count = td.td_samplesperpixel;
                    }
                    else
                    {
                        td.td_customValues[tvIndex].count = fip.WriteCount;
                    }

                    if (fip.Type == TiffType.ASCII)
                    {
                        string ascii;
                        Tiff.setString(out ascii, value[paramIndex++].ToString());
                        td.td_customValues[tvIndex].value = Tiff.Latin1Encoding.GetBytes(ascii);
                    }
                    else
                    {
                        td.td_customValues[tvIndex].value = new byte[tv_size * td.td_customValues[tvIndex].count];
                        if ((fip.PassCount ||
                            fip.WriteCount == TiffFieldInfo.Variable ||
                            fip.WriteCount == TiffFieldInfo.Variable2 ||
                            fip.WriteCount == TiffFieldInfo.Spp ||
                            td.td_customValues[tvIndex].count > 1) &&
                            fip.Tag != TiffTag.PAGENUMBER &&
                            fip.Tag != TiffTag.HALFTONEHINTS &&
                            fip.Tag != TiffTag.YCBCRSUBSAMPLING &&
                            fip.Tag != TiffTag.DOTRANGE)
                        {
                            byte[] apBytes = value[paramIndex++].GetBytes();
                            Buffer.BlockCopy(apBytes, 0, td.td_customValues[tvIndex].value, 0, Math.Min(apBytes.Length, td.td_customValues[tvIndex].value.Length));
                        }
                        else
                        {
                            byte[] val = td.td_customValues[tvIndex].value;
                            int valPos = 0;
                            for (int i = 0; i < td.td_customValues[tvIndex].count; i++, valPos += tv_size)
                            {
                                switch (fip.Type)
                                {
                                    case TiffType.BYTE:
                                    case TiffType.UNDEFINED:
                                        val[valPos] = value[paramIndex + i].GetBytes()[0];
                                        break;
                                    case TiffType.SBYTE:
                                        val[valPos] = value[paramIndex + i].ToByte();
                                        break;
                                    case TiffType.SHORT:
                                        Buffer.BlockCopy(BitConverter.GetBytes(value[paramIndex + i].ToShort()), 0, val, valPos, tv_size);
                                        break;
                                    case TiffType.SSHORT:
                                        Buffer.BlockCopy(BitConverter.GetBytes(value[paramIndex + i].ToShort()), 0, val, valPos, tv_size);
                                        break;
                                    case TiffType.LONG:
                                    case TiffType.IFD:
                                        Buffer.BlockCopy(BitConverter.GetBytes(value[paramIndex + i].ToInt()), 0, val, valPos, tv_size);
                                        break;
                                    case TiffType.SLONG:
                                        Buffer.BlockCopy(BitConverter.GetBytes(value[paramIndex + i].ToInt()), 0, val, valPos, tv_size);
                                        break;
                                    case TiffType.RATIONAL:
                                    case TiffType.SRATIONAL:
                                    case TiffType.FLOAT:
                                        Buffer.BlockCopy(BitConverter.GetBytes(value[paramIndex + i].ToFloat()), 0, val, valPos, tv_size);
                                        break;
                                    case TiffType.DOUBLE:
                                        Buffer.BlockCopy(BitConverter.GetBytes(value[paramIndex + i].ToDouble()), 0, val, valPos, tv_size);
                                        break;
                                    default:
                                        Array.Clear(val, valPos, tv_size);
                                        status = false;
                                        break;
                                }
                            }
                        }
                    }
                    break;
            }

            if (!end && !badvalue && !badvalue32)
            {
                if (status)
                {
                    tif.setFieldBit(tif.FieldWithTag(tag).Bit);
                    tif.m_flags |= TiffFlags.DIRTYDIRECT;
                }
            }

            if (badvalue)
            {
                return false;
            }

            if (badvalue32)
            {
                return false;
            }

            return status;
        }

        /// <summary>
        /// Gets the value(s) of a tag in an open TIFF file.
        /// </summary>
        public virtual FieldValue[] GetField(Tiff tif, TiffTag tag)
        {
            TiffDirectory td = tif.m_dir;
            FieldValue[] result = null;

            switch (tag)
            {
                case TiffTag.SUBFILETYPE:
                    result = new FieldValue[1];
                    result[0].Set(td.td_subfiletype);
                    break;
                case TiffTag.IMAGEWIDTH:
                    result = new FieldValue[1];
                    result[0].Set(td.td_imagewidth);
                    break;
                case TiffTag.IMAGELENGTH:
                    result = new FieldValue[1];
                    result[0].Set(td.td_imagelength);
                    break;
                case TiffTag.BITSPERSAMPLE:
                    result = new FieldValue[1];
                    result[0].Set(td.td_bitspersample);
                    break;
                case TiffTag.COMPRESSION:
                    result = new FieldValue[1];
                    result[0].Set(td.td_compression);
                    break;
                case TiffTag.PHOTOMETRIC:
                    result = new FieldValue[1];
                    result[0].Set(td.td_photometric);
                    break;
                case TiffTag.THRESHHOLDING:
                    result = new FieldValue[1];
                    result[0].Set(td.td_threshholding);
                    break;
                case TiffTag.FILLORDER:
                    result = new FieldValue[1];
                    result[0].Set(td.td_fillorder);
                    break;
                case TiffTag.ORIENTATION:
                    result = new FieldValue[1];
                    result[0].Set(td.td_orientation);
                    break;
                case TiffTag.SAMPLESPERPIXEL:
                    result = new FieldValue[1];
                    result[0].Set(td.td_samplesperpixel);
                    break;
                case TiffTag.ROWSPERSTRIP:
                    result = new FieldValue[1];
                    result[0].Set(td.td_rowsperstrip);
                    break;
                case TiffTag.MINSAMPLEVALUE:
                    result = new FieldValue[1];
                    result[0].Set(td.td_minsamplevalue);
                    break;
                case TiffTag.MAXSAMPLEVALUE:
                    result = new FieldValue[1];
                    result[0].Set(td.td_maxsamplevalue);
                    break;
                case TiffTag.SMINSAMPLEVALUE:
                    result = new FieldValue[1];
                    result[0].Set(td.td_sminsamplevalue);
                    break;
                case TiffTag.SMAXSAMPLEVALUE:
                    result = new FieldValue[1];
                    result[0].Set(td.td_smaxsamplevalue);
                    break;
                case TiffTag.XRESOLUTION:
                    result = new FieldValue[1];
                    result[0].Set(td.td_xresolution);
                    break;
                case TiffTag.YRESOLUTION:
                    result = new FieldValue[1];
                    result[0].Set(td.td_yresolution);
                    break;
                case TiffTag.PLANARCONFIG:
                    result = new FieldValue[1];
                    result[0].Set(td.td_planarconfig);
                    break;
                case TiffTag.XPOSITION:
                    result = new FieldValue[1];
                    result[0].Set(td.td_xposition);
                    break;
                case TiffTag.YPOSITION:
                    result = new FieldValue[1];
                    result[0].Set(td.td_yposition);
                    break;
                case TiffTag.RESOLUTIONUNIT:
                    result = new FieldValue[1];
                    result[0].Set(td.td_resolutionunit);
                    break;
                case TiffTag.PAGENUMBER:
                    result = new FieldValue[2];
                    result[0].Set(td.td_pagenumber[0]);
                    result[1].Set(td.td_pagenumber[1]);
                    break;
                case TiffTag.HALFTONEHINTS:
                    result = new FieldValue[2];
                    result[0].Set(td.td_halftonehints[0]);
                    result[1].Set(td.td_halftonehints[1]);
                    break;
                case TiffTag.COLORMAP:
                    result = new FieldValue[3];
                    result[0].Set(td.td_colormap[0]);
                    result[1].Set(td.td_colormap[1]);
                    result[2].Set(td.td_colormap[2]);
                    break;
                case TiffTag.STRIPOFFSETS:
                case TiffTag.TILEOFFSETS:
                    result = new FieldValue[1];
                    result[0].Set(td.td_stripoffset);
                    break;
                case TiffTag.STRIPBYTECOUNTS:
                case TiffTag.TILEBYTECOUNTS:
                    result = new FieldValue[1];
                    result[0].Set(td.td_stripbytecount);
                    break;
                case TiffTag.MATTEING:
                    result = new FieldValue[1];
                    result[0].Set((td.td_extrasamples == 1 && td.td_sampleinfo[0] == ExtraSample.ASSOCALPHA));
                    break;
                case TiffTag.EXTRASAMPLES:
                    result = new FieldValue[2];
                    result[0].Set(td.td_extrasamples);
                    result[1].Set(td.td_sampleinfo);
                    break;
                case TiffTag.TILEWIDTH:
                    result = new FieldValue[1];
                    result[0].Set(td.td_tilewidth);
                    break;
                case TiffTag.TILELENGTH:
                    result = new FieldValue[1];
                    result[0].Set(td.td_tilelength);
                    break;
                case TiffTag.TILEDEPTH:
                    result = new FieldValue[1];
                    result[0].Set(td.td_tiledepth);
                    break;
                case TiffTag.DATATYPE:
                    switch (td.td_sampleformat)
                    {
                        case SampleFormat.UINT:
                            result = new FieldValue[1];
                            result[0].Set(DATATYPE_UINT);
                            break;
                        case SampleFormat.INT:
                            result = new FieldValue[1];
                            result[0].Set(DATATYPE_INT);
                            break;
                        case SampleFormat.IEEEFP:
                            result = new FieldValue[1];
                            result[0].Set(DATATYPE_IEEEFP);
                            break;
                        case SampleFormat.VOID:
                            result = new FieldValue[1];
                            result[0].Set(DATATYPE_VOID);
                            break;
                    }
                    break;
                case TiffTag.SAMPLEFORMAT:
                    result = new FieldValue[1];
                    result[0].Set(td.td_sampleformat);
                    break;
                case TiffTag.IMAGEDEPTH:
                    result = new FieldValue[1];
                    result[0].Set(td.td_imagedepth);
                    break;
                case TiffTag.SUBIFD:
                    result = new FieldValue[2];
                    result[0].Set(td.td_nsubifd);
                    result[1].Set(td.td_subifd);
                    break;
                case TiffTag.YCBCRPOSITIONING:
                    result = new FieldValue[1];
                    result[0].Set(td.td_ycbcrpositioning);
                    break;
                case TiffTag.YCBCRSUBSAMPLING:
                    result = new FieldValue[2];
                    result[0].Set(td.td_ycbcrsubsampling[0]);
                    result[1].Set(td.td_ycbcrsubsampling[1]);
                    break;
                case TiffTag.TRANSFERFUNCTION:
                    result = new FieldValue[3];
                    result[0].Set(td.td_transferfunction[0]);
                    if (td.td_samplesperpixel - td.td_extrasamples > 1)
                    {
                        result[1].Set(td.td_transferfunction[1]);
                        result[2].Set(td.td_transferfunction[2]);
                    }
                    break;
                case TiffTag.REFERENCEBLACKWHITE:
                    if (td.td_refblackwhite != null)
                    {
                        result = new FieldValue[1];
                        result[0].Set(td.td_refblackwhite);
                    }
                    break;
                case TiffTag.INKNAMES:
                    result = new FieldValue[1];
                    result[0].Set(td.td_inknames);
                    break;
                default:
                    TiffFieldInfo fip = tif.FindFieldInfo(tag, TiffType.ANY);
                    if (fip == null || fip.Bit != FieldBit.Custom)
                    {
                        result = null;
                        break;
                    }

                    result = null;
                    for (int i = 0; i < td.td_customValueCount; i++)
                    {
                        TiffTagValue tv = td.td_customValues[i];
                        if (tv.info.Tag != tag)
                            continue;

                        if (fip.PassCount)
                        {
                            result = new FieldValue[2];

                            if (fip.ReadCount == TiffFieldInfo.Variable2)
                            {
                                result[0].Set(tv.count);
                            }
                            else
                            {
                                result[0].Set(tv.count);
                            }

                            result[1].Set(tv.value);
                        }
                        else
                        {
                            if ((fip.Type == TiffType.ASCII ||
                                fip.ReadCount == TiffFieldInfo.Variable ||
                                fip.ReadCount == TiffFieldInfo.Variable2 ||
                                fip.ReadCount == TiffFieldInfo.Spp ||
                                tv.count > 1) && fip.Tag != TiffTag.PAGENUMBER &&
                                fip.Tag != TiffTag.HALFTONEHINTS &&
                                fip.Tag != TiffTag.YCBCRSUBSAMPLING &&
                                fip.Tag != TiffTag.DOTRANGE)
                            {
                                result = new FieldValue[1];
                                byte[] value = tv.value;

                                if (fip.Type == TiffType.ASCII &&
                                    tv.value.Length > 0 &&
                                    tv.value[tv.value.Length - 1] == 0)
                                {
                                    value = new byte[Math.Max(tv.value.Length - 1, 0)];
                                    Buffer.BlockCopy(tv.value, 0, value, 0, value.Length);
                                }

                                result[0].Set(value);
                            }
                            else
                            {
                                result = new FieldValue[tv.count];
                                byte[] val = tv.value;
                                int valPos = 0;
                                for (int j = 0; j < tv.count; j++, valPos += Tiff.dataSize(tv.info.Type))
                                {
                                    switch (fip.Type)
                                    {
                                        case TiffType.BYTE:
                                        case TiffType.UNDEFINED:
                                        case TiffType.SBYTE:
                                            result[j].Set(val[valPos]);
                                            break;
                                        case TiffType.SHORT:
                                        case TiffType.SSHORT:
                                            result[j].Set(BitConverter.ToInt16(val, valPos));
                                            break;
                                        case TiffType.LONG:
                                        case TiffType.IFD:
                                        case TiffType.SLONG:
                                            result[j].Set(BitConverter.ToInt32(val, valPos));
                                            break;
                                        case TiffType.RATIONAL:
                                        case TiffType.SRATIONAL:
                                        case TiffType.FLOAT:
                                            result[j].Set(BitConverter.ToSingle(val, valPos));
                                            break;
                                        case TiffType.DOUBLE:
                                            result[j].Set(BitConverter.ToDouble(val, valPos));
                                            break;
                                        default:
                                            result = null;
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Install extra samples information.
        /// </summary>
        private static bool setExtraSamples(TiffDirectory td, ref int v, FieldValue[] ap)
        {
            const short EXTRASAMPLE_COREL_UNASSALPHA = 999;

            v = ap[0].ToInt();
            if (v > td.td_samplesperpixel)
                return false;

            byte[] va = ap[1].ToByteArray();
            if (v > 0 && va == null)
            {
                // typically missing param
                return false;
            }

            for (int i = 0; i < v; i++)
            {
                if ((ExtraSample)va[i] > ExtraSample.UNASSALPHA)
                {
                    if (i < v - 1)
                    {
                        short s = BitConverter.ToInt16(va, i);
                        if (s == EXTRASAMPLE_COREL_UNASSALPHA)
                            va[i] = (byte)ExtraSample.UNASSALPHA;
                    }
                    else
                        return false;
                }
            }

            td.td_extrasamples = (short)v;
            td.td_sampleinfo = new ExtraSample[td.td_extrasamples];
            for (int i = 0; i < td.td_extrasamples; i++)
                td.td_sampleinfo[i] = (ExtraSample)va[i];

            return true;
        }

        private static int checkInkNamesString(Tiff tif, int slen, string s)
        {
            bool failed = false;
            short i = tif.m_dir.td_samplesperpixel;

            if (slen > 0)
            {
                int endPos = slen;
                int pos = 0;

                for (; i > 0; i--)
                {
                    for (; s[pos] != '\0'; pos++)
                    {
                        if (pos >= endPos)
                        {
                            failed = true;
                            break;
                        }
                    }

                    if (failed)
                        break;

                    pos++; // skip \0
                }

                if (!failed)
                    return pos;
            }

            return 0;
        }

        private static void setNString(out string cpp, string cp, int n)
        {
            cpp = cp.Substring(0, n);
        }
    }
}
