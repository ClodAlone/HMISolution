#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

using Syncfusion.Pdf.Compression.JBIG2.Internal;

namespace Syncfusion.Pdf.Compression.JBIG2
{
    /// <summary>
    /// Tag Image File Format (TIFF)
    /// </summary>
    partial class Tiff : IDisposable
    {
        /// <summary>
        /// Delegate for LibTiff.Net extender method
        /// </summary>
        public delegate void TiffExtendProc(Tiff tif);

        /// <summary>
        /// Delegate for a method used to image decoded spans.        
        /// </summary>
        public delegate void FaxFillFunc(
            byte[] buffer, int offset, int[] runs, int thisRunOffset, int nextRunOffset, int width);
      
        /// <summary>
        /// Retrieves the codec registered for the specified compression scheme.
        /// </summary>
        public TiffCodec FindCodec(Compression scheme)
        {
            for (codecList list = m_registeredCodecs; list != null; list = list.next)
            {
                if (list.codec.m_scheme == scheme)
                    return list.codec;
            }

            for (int i = 0; m_builtInCodecs[i] != null; i++)
            {
                TiffCodec codec = m_builtInCodecs[i];
                if (codec.m_scheme == scheme)
                    return codec;
            }

            return null;
        }

        /// <summary>
        /// Checks whether library has working codec for the specific compression scheme.
        /// </summary>
        public bool IsCodecConfigured(Compression scheme)
        {
            TiffCodec codec = FindCodec(scheme);

            if (codec == null)
                return false;

            if (codec.CanDecode != false)
                return true;

            return false;
        }

        /// <summary>
        /// Initializes new instance of <see cref="Tiff"/> class and opens a TIFF file for
        /// reading or writing.
        /// </summary>
        public static Tiff Open(string fileName, string mode)
        {
            return Tiff.Open(fileName, mode, null);
        }

        /// <summary>
        /// Initializes new instance of <see cref="Tiff"/> class and opens a stream with TIFF data
        /// for reading or writing.
        /// </summary>
        public static Tiff ClientOpen(string name, string mode, object clientData, TiffStream stream)
        {
            return ClientOpen(name, mode, clientData, stream, null);
        }

        /// <summary>
        /// Closes a previously opened TIFF file.
        /// </summary>
        public void Close()
        {
            m_stream.Close(m_clientdata);

            if (m_fileStream != null)
                m_fileStream.Close();
        }

        /// <summary>
        /// Frees and releases all resources allocated by this <see cref="Tiff"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Merges given field information to existing one.
        /// </summary>
        public void MergeFieldInfo(TiffFieldInfo[] info, int count)
        {
            m_foundfield = null;

            if (m_nfields <= 0)
                m_fieldinfo = new TiffFieldInfo[count];

            for (int i = 0; i < count; i++)
            {
                TiffFieldInfo fip = FindFieldInfo(info[i].Tag, info[i].Type);

                if (fip == null)
                {
                    m_fieldinfo = Realloc(m_fieldinfo, m_nfields, m_nfields + 1);
                    m_fieldinfo[m_nfields] = info[i];
                    m_nfields++;
                }
            }

            // Sort the field info by tag number
            IComparer myComparer = new TagCompare();
            Array.Sort(m_fieldinfo, 0, m_nfields, myComparer);
        }

        /// <summary>
        /// Retrieves field information for the specified tag.
        /// </summary>
        public TiffFieldInfo FindFieldInfo(TiffTag tag, TiffType type)
        {
            if (m_foundfield != null && m_foundfield.Tag == tag &&
                (type == TiffType.ANY || type == m_foundfield.Type))
            {
                return m_foundfield;
            }

            if (m_fieldinfo == null)
                return null;

            m_foundfield = null;

            foreach (TiffFieldInfo info in m_fieldinfo)
            {
                if (info != null && info.Tag == tag && (type == TiffType.ANY || type == info.Type))
                {
                    m_foundfield = info;
                    break;
                }
            }

            return m_foundfield;
        }

        /// <summary>
        /// Retrieves field information for the tag with specified name.
        /// </summary>
        public TiffFieldInfo FindFieldInfoByName(string name, TiffType type)
        {
            if (m_foundfield != null && m_foundfield.Name == name &&
                (type == TiffType.ANY || type == m_foundfield.Type))
            {
                return m_foundfield;
            }

            if (m_fieldinfo == null)
                return null;

            m_foundfield = null;

            foreach (TiffFieldInfo info in m_fieldinfo)
            {
                if (info != null && info.Name == name &&
                    (type == TiffType.ANY || type == info.Type))
                {
                    m_foundfield = info;
                    break;
                }
            }

            return m_foundfield;
        }

        /// <summary>
        /// Retrieves field information for the specified tag.
        /// </summary>
        public TiffFieldInfo FieldWithTag(TiffTag tag)
        {
            TiffFieldInfo fip = FindFieldInfo(tag, TiffType.ANY);
            if (fip != null)
                return fip;

            return null;
        }

        /// <summary>
        /// Gets the value(s) of a tag in an open TIFF file.
        /// </summary>
        public FieldValue[] GetField(TiffTag tag)
        {
            TiffFieldInfo fip = FindFieldInfo(tag, TiffType.ANY);
            if (fip != null && (isPseudoTag(tag) || fieldSet(fip.Bit)))
                return m_tagmethods.GetField(this, tag);

            return null;
        }

        /// <summary>
        /// Gets the value(s) of a tag in an open TIFF file or default value(s) of a tag if a tag
        /// is not defined in the current directory and it has a default value(s).
        /// </summary>
        public FieldValue[] GetFieldDefaulted(TiffTag tag)
        {
            TiffDirectory td = m_dir;

            FieldValue[] result = GetField(tag);
            if (result != null)
                return result;

            switch (tag)
            {
                case TiffTag.SUBFILETYPE:
                    result = new FieldValue[1];
                    result[0].Set(td.td_subfiletype);
                    break;
                case TiffTag.BITSPERSAMPLE:
                    result = new FieldValue[1];
                    result[0].Set(td.td_bitspersample);
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
                case TiffTag.PLANARCONFIG:
                    result = new FieldValue[1];
                    result[0].Set(td.td_planarconfig);
                    break;
                case TiffTag.RESOLUTIONUNIT:
                    result = new FieldValue[1];
                    result[0].Set(td.td_resolutionunit);
                    break;
                case TiffTag.PREDICTOR:
                    CodecWithPredictor sp = m_currentCodec as CodecWithPredictor;
                    if (sp != null)
                    {
                        result = new FieldValue[1];
                        result[0].Set(sp.GetPredictorValue());
                    }
                    break;
                case TiffTag.DOTRANGE:
                    result = new FieldValue[2];
                    result[0].Set(0);
                    result[1].Set((1 << td.td_bitspersample) - 1);
                    break;
                case TiffTag.INKSET:
                    result = new FieldValue[1];
                    result[0].Set(InkSet.CMYK);
                    break;
                case TiffTag.NUMBEROFINKS:
                    result = new FieldValue[1];
                    result[0].Set(4);
                    break;
                case TiffTag.EXTRASAMPLES:
                    result = new FieldValue[2];
                    result[0].Set(td.td_extrasamples);
                    result[1].Set(td.td_sampleinfo);
                    break;
                case TiffTag.MATTEING:
                    result = new FieldValue[1];
                    result[0].Set((td.td_extrasamples == 1 && td.td_sampleinfo[0] == ExtraSample.ASSOCALPHA));
                    break;
                case TiffTag.TILEDEPTH:
                    result = new FieldValue[1];
                    result[0].Set(td.td_tiledepth);
                    break;
                case TiffTag.DATATYPE:
                    result = new FieldValue[1];
                    result[0].Set(td.td_sampleformat - 1);
                    break;
                case TiffTag.SAMPLEFORMAT:
                    result = new FieldValue[1];
                    result[0].Set(td.td_sampleformat);
                    break;
                case TiffTag.IMAGEDEPTH:
                    result = new FieldValue[1];
                    result[0].Set(td.td_imagedepth);
                    break;
                case TiffTag.YCBCRCOEFFICIENTS:
                    {
                        // defaults are from CCIR Recommendation 601-1
                        float[] ycbcrcoeffs = new float[3];
                        ycbcrcoeffs[0] = 0.299f;
                        ycbcrcoeffs[1] = 0.587f;
                        ycbcrcoeffs[2] = 0.114f;

                        result = new FieldValue[1];
                        result[0].Set(ycbcrcoeffs);
                        break;
                    }
                case TiffTag.YCBCRSUBSAMPLING:
                    result = new FieldValue[2];
                    result[0].Set(td.td_ycbcrsubsampling[0]);
                    result[1].Set(td.td_ycbcrsubsampling[1]);
                    break;
                case TiffTag.YCBCRPOSITIONING:
                    result = new FieldValue[1];
                    result[0].Set(td.td_ycbcrpositioning);
                    break;
                case TiffTag.WHITEPOINT:
                    {
                        float[] whitepoint = new float[2];
                        whitepoint[0] = D50_X0 / (D50_X0 + D50_Y0 + D50_Z0);
                        whitepoint[1] = D50_Y0 / (D50_X0 + D50_Y0 + D50_Z0);

                        result = new FieldValue[1];
                        result[0].Set(whitepoint);
                        break;
                    }
                case TiffTag.TRANSFERFUNCTION:
                    if (td.td_transferfunction[0] == null && !defaultTransferFunction(td))
                    {
                        return null;
                    }

                    result = new FieldValue[3];
                    result[0].Set(td.td_transferfunction[0]);
                    if (td.td_samplesperpixel - td.td_extrasamples > 1)
                    {
                        result[1].Set(td.td_transferfunction[1]);
                        result[2].Set(td.td_transferfunction[2]);
                    }
                    break;
                case TiffTag.REFERENCEBLACKWHITE:
                    if (td.td_refblackwhite == null)
                        defaultRefBlackWhite(td);

                    result = new FieldValue[1];
                    result[0].Set(td.td_refblackwhite);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Reads the contents of the next TIFF directory in an open TIFF file/stream and makes
        /// it the current directory.
        /// </summary>
        public bool ReadDirectory()
        {
            const string module = "ReadDirectory";

            m_diroff = m_nextdiroff;
            if (m_diroff == 0)
            {
                // no more directories
                return false;
            }

            if (!checkDirOffset(m_nextdiroff))
                return false;

            m_currentCodec.Cleanup();
            m_curdir++;
            TiffDirEntry[] dir;
            short dircount = fetchDirectory(m_nextdiroff, out dir, out m_nextdiroff);
            if (dircount == 0)
            {
                return false;
            }

            // reset before new dir
            m_flags &= ~TiffFlags.BEENWRITING;

            FreeDirectory();
            setupDefaultDirectory();
            SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

            for (int i = 0; i < dircount; i++)
            {
                TiffDirEntry dp = dir[i];
                if ((m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
                {
                    short temp = (short)dp.tdir_tag;
                    SwabShort(ref temp);
                    dp.tdir_tag = (TiffTag)(ushort)temp;

                    temp = (short)dp.tdir_type;
                    SwabShort(ref temp);
                    dp.tdir_type = (TiffType)temp;

                    SwabLong(ref dp.tdir_count);
                    SwabUInt(ref dp.tdir_offset);
                }

                if (dp.tdir_tag == TiffTag.SAMPLESPERPIXEL)
                {
                    if (!fetchNormalTag(dir[i]))
                        return false;

                    dp.tdir_tag = TiffTag.IGNORE;
                }
            }

            // First real pass over the directory.
            int fix = 0;
            bool diroutoforderwarning = false;
            bool haveunknowntags = false;

            for (int i = 0; i < dircount; i++)
            {
                if (dir[i].tdir_tag == TiffTag.IGNORE)
                    continue;

                if (fix >= m_nfields)
                    fix = 0;

                if (dir[i].tdir_tag < m_fieldinfo[fix].Tag)
                {
                    if (!diroutoforderwarning)
                    {
                        diroutoforderwarning = true;
                    }

                    fix = 0; // O(n^2)
                }

                while (fix < m_nfields && m_fieldinfo[fix].Tag < dir[i].tdir_tag)
                    fix++;

                if (fix >= m_nfields || m_fieldinfo[fix].Tag != dir[i].tdir_tag)
                {
                    haveunknowntags = true;
                    continue;
                }

                // null out old tags that we ignore.
                if (m_fieldinfo[fix].Bit == FieldBit.Ignore)
                {
                    dir[i].tdir_tag = TiffTag.IGNORE;
                    continue;
                }

                TiffFieldInfo fip = m_fieldinfo[fix];
                bool tagIgnored = false;
                while (dir[i].tdir_type != fip.Type && fix < m_nfields)
                {
                    if (fip.Type == TiffType.ANY)
                    {
                        break;
                    }

                    fip = m_fieldinfo[++fix];
                    if (fix >= m_nfields || fip.Tag != dir[i].tdir_tag)
                    {
                        dir[i].tdir_tag = TiffTag.IGNORE;
                        tagIgnored = true;
                        break;
                    }
                }

                if (tagIgnored)
                    continue;

                if (fip.ReadCount != TiffFieldInfo.Variable &&
                    fip.ReadCount != TiffFieldInfo.Variable2)
                {
                    int expected = fip.ReadCount;
                    if (fip.ReadCount == TiffFieldInfo.Spp)
                        expected = m_dir.td_samplesperpixel;

                    if (!checkDirCount(dir[i], expected))
                    {
                        dir[i].tdir_tag = TiffTag.IGNORE;
                        continue;
                    }
                }

                switch (dir[i].tdir_tag)
                {
                    case TiffTag.COMPRESSION:
                        if (dir[i].tdir_count == 1)
                        {
                            int v = extractData(dir[i]);
                            if (!SetField(dir[i].tdir_tag, v))
                                return false;

                            break;
                        }
                        else if (dir[i].tdir_type == TiffType.LONG)
                        {
                            int v;
                            if (!fetchPerSampleLongs(dir[i], out v) || !SetField(dir[i].tdir_tag, v))
                                return false;
                        }
                        else
                        {
                            short iv;
                            if (!fetchPerSampleShorts(dir[i], out iv) || !SetField(dir[i].tdir_tag, iv))
                                return false;
                        }
                        dir[i].tdir_tag = TiffTag.IGNORE;
                        break;
                    case TiffTag.STRIPOFFSETS:
                    case TiffTag.STRIPBYTECOUNTS:
                    case TiffTag.TILEOFFSETS:
                    case TiffTag.TILEBYTECOUNTS:
                        setFieldBit(fip.Bit);
                        break;
                    case TiffTag.IMAGEWIDTH:
                    case TiffTag.IMAGELENGTH:
                    case TiffTag.IMAGEDEPTH:
                    case TiffTag.TILELENGTH:
                    case TiffTag.TILEWIDTH:
                    case TiffTag.TILEDEPTH:
                    case TiffTag.PLANARCONFIG:
                    case TiffTag.ROWSPERSTRIP:
                    case TiffTag.EXTRASAMPLES:
                        if (!fetchNormalTag(dir[i]))
                            return false;
                        dir[i].tdir_tag = TiffTag.IGNORE;
                        break;
                }
            }

            if (haveunknowntags)
            {
                fix = 0;
                for (int i = 0; i < dircount; i++)
                {
                    if (dir[i].tdir_tag == TiffTag.IGNORE)
                        continue;

                    if (fix >= m_nfields || dir[i].tdir_tag < m_fieldinfo[fix].Tag)
                    {
                        // O(n^2)
                        fix = 0;
                    }

                    while (fix < m_nfields && m_fieldinfo[fix].Tag < dir[i].tdir_tag)
                        fix++;

                    if (fix >= m_nfields || m_fieldinfo[fix].Tag != dir[i].tdir_tag)
                    {
                        TiffFieldInfo[] arr = new TiffFieldInfo[1];
                        arr[0] = createAnonFieldInfo(dir[i].tdir_tag, dir[i].tdir_type);
                        MergeFieldInfo(arr, 1);

                        fix = 0;
                        while (fix < m_nfields && m_fieldinfo[fix].Tag < dir[i].tdir_tag)
                            fix++;
                    }

                    // Check data type.
                    TiffFieldInfo fip = m_fieldinfo[fix];
                    while (dir[i].tdir_type != fip.Type && fix < m_nfields)
                    {
                        if (fip.Type == TiffType.ANY)
                        {
                            // wildcard
                            break;
                        }

                        fip = m_fieldinfo[++fix];
                        if (fix >= m_nfields || fip.Tag != dir[i].tdir_tag)
                        {
                            dir[i].tdir_tag = TiffTag.IGNORE;
                            break;
                        }
                    }
                }
            }

            if ((m_dir.td_compression == Compression.OJPEG) && (m_dir.td_planarconfig == PlanarConfig.SEPARATE))
            {
                int dpIndex = readDirectoryFind(dir, dircount, TiffTag.STRIPOFFSETS);
                if (dpIndex != -1 && dir[dpIndex].tdir_count == 1)
                {
                    dpIndex = readDirectoryFind(dir, dircount, TiffTag.STRIPBYTECOUNTS);
                    if (dpIndex != -1 && dir[dpIndex].tdir_count == 1)
                    {
                        m_dir.td_planarconfig = PlanarConfig.CONTIG;
                    }
                }
            }

            if (!fieldSet(FieldBit.ImageDimensions))
            {
                missingRequired("ImageLength");
                return false;
            }

            if (!fieldSet(FieldBit.TileDimensions))
            {
                m_dir.td_nstrips = NumberOfStrips();
                m_dir.td_tilewidth = m_dir.td_imagewidth;
                m_dir.td_tilelength = m_dir.td_rowsperstrip;
                m_dir.td_tiledepth = m_dir.td_imagedepth;
                m_flags &= ~TiffFlags.ISTILED;
            }
            else
            {
                m_dir.td_nstrips = NumberOfTiles();
                m_flags |= TiffFlags.ISTILED;
            }

            if (m_dir.td_nstrips == 0)
            {
                return false;
            }

            m_dir.td_stripsperimage = m_dir.td_nstrips;
            if (m_dir.td_planarconfig == PlanarConfig.SEPARATE)
                m_dir.td_stripsperimage /= m_dir.td_samplesperpixel;

            if (!fieldSet(FieldBit.StripOffsets))
            {
                if ((m_dir.td_compression == Compression.OJPEG) && !IsTiled() && (m_dir.td_nstrips == 1))
                {
                    setFieldBit(FieldBit.StripOffsets);
                }
                else
                {
                    missingRequired(IsTiled() ? "TileOffsets" : "StripOffsets");
                    return false;
                }
            }

            for (int i = 0; i < dircount; i++)
            {
                if (dir[i].tdir_tag == TiffTag.IGNORE)
                    continue;

                switch (dir[i].tdir_tag)
                {
                    case TiffTag.MINSAMPLEVALUE:
                    case TiffTag.MAXSAMPLEVALUE:
                    case TiffTag.BITSPERSAMPLE:
                    case TiffTag.DATATYPE:
                    case TiffTag.SAMPLEFORMAT:
                        if (dir[i].tdir_count == 1)
                        {
                            int v = extractData(dir[i]);
                            if (!SetField(dir[i].tdir_tag, v))
                                return false;
                        }
                        else if (dir[i].tdir_tag == TiffTag.BITSPERSAMPLE && dir[i].tdir_type == TiffType.LONG)
                        {
                            int v;
                            if (!fetchPerSampleLongs(dir[i], out v) || !SetField(dir[i].tdir_tag, v))
                                return false;
                        }
                        else
                        {
                            short iv;
                            if (!fetchPerSampleShorts(dir[i], out iv) || !SetField(dir[i].tdir_tag, iv))
                                return false;
                        }
                        break;
                    case TiffTag.SMINSAMPLEVALUE:
                    case TiffTag.SMAXSAMPLEVALUE:
                        double dv;
                        if (!fetchPerSampleAnys(dir[i], out dv) || !SetField(dir[i].tdir_tag, dv))
                            return false;
                        break;
                    case TiffTag.STRIPOFFSETS:
                    case TiffTag.TILEOFFSETS:
                        if (!fetchStripThing(dir[i], m_dir.td_nstrips, ref m_dir.td_stripoffset))
                            return false;
                        break;
                    case TiffTag.STRIPBYTECOUNTS:
                    case TiffTag.TILEBYTECOUNTS:
                        if (!fetchStripThing(dir[i], m_dir.td_nstrips, ref m_dir.td_stripbytecount))
                            return false;
                        break;
                    case TiffTag.COLORMAP:
                    case TiffTag.TRANSFERFUNCTION:
                        {
                            int v = 1 << m_dir.td_bitspersample;
                            if (dir[i].tdir_tag == TiffTag.COLORMAP || dir[i].tdir_count != v)
                            {
                                if (!checkDirCount(dir[i], 3 * v))
                                    break;
                            }

                            byte[] cp = new byte[dir[i].tdir_count * sizeof(short)];
                            if (fetchData(dir[i], cp) != 0)
                            {
                                int c = 1 << m_dir.td_bitspersample;
                                if (dir[i].tdir_count == c)
                                {
                                    short[] u = ByteArrayToShorts(cp, 0, dir[i].tdir_count * sizeof(short));
                                    SetField(dir[i].tdir_tag, u, u, u);
                                }
                                else
                                {
                                    v *= sizeof(short);
                                    short[] u0 = ByteArrayToShorts(cp, 0, v);
                                    short[] u1 = ByteArrayToShorts(cp, v, v);
                                    short[] u2 = ByteArrayToShorts(cp, 2 * v, v);
                                    SetField(dir[i].tdir_tag, u0, u1, u2);
                                }
                            }
                            break;
                        }
                    case TiffTag.PAGENUMBER:
                    case TiffTag.HALFTONEHINTS:
                    case TiffTag.YCBCRSUBSAMPLING:
                    case TiffTag.DOTRANGE:
                        fetchShortPair(dir[i]);
                        break;
                    case TiffTag.REFERENCEBLACKWHITE:
                        fetchRefBlackWhite(dir[i]);
                        break;
                    case TiffTag.OSUBFILETYPE:
                        FileType ft = 0;
                        switch ((OFileType)extractData(dir[i]))
                        {
                            case OFileType.REDUCEDIMAGE:
                                ft = FileType.REDUCEDIMAGE;
                                break;
                            case OFileType.PAGE:
                                ft = FileType.PAGE;
                                break;
                        }

                        if (ft != 0)
                            SetField(TiffTag.SUBFILETYPE, ft);

                        break;
                    default:
                        fetchNormalTag(dir[i]);
                        break;
                }
            }

            if (m_dir.td_compression == Compression.OJPEG)
            {
                if (!fieldSet(FieldBit.Photometric))
                {
                    if (!SetField(TiffTag.PHOTOMETRIC, Photometric.YCBCR))
                        return false;
                }
                else if (m_dir.td_photometric == Photometric.RGB)
                {
                    m_dir.td_photometric = Photometric.YCBCR;
                }

                if (!fieldSet(FieldBit.BitsPerSample))
                {
                    if (!SetField(TiffTag.BITSPERSAMPLE, 8))
                        return false;
                }

                if (!fieldSet(FieldBit.SamplesPerPixel))
                {
                    if ((m_dir.td_photometric == Photometric.RGB) ||
                        (m_dir.td_photometric == Photometric.YCBCR))
                    {
                        if (!SetField(TiffTag.SAMPLESPERPIXEL, 3))
                            return false;
                    }
                    else if ((m_dir.td_photometric == Photometric.MINISWHITE) ||
                        (m_dir.td_photometric == Photometric.MINISBLACK))
                    {
                        if (!SetField(TiffTag.SAMPLESPERPIXEL, 1))
                            return false;
                    }
                }
            }

            if (m_dir.td_photometric == Photometric.PALETTE && !fieldSet(FieldBit.ColorMap))
            {
                missingRequired("Colormap");
                return false;
            }

            if (m_dir.td_compression != Compression.OJPEG)
            {
                if (!fieldSet(FieldBit.StripByteCounts))
                {
                    if ((m_dir.td_planarconfig == PlanarConfig.CONTIG && m_dir.td_nstrips > 1) ||
                        (m_dir.td_planarconfig == PlanarConfig.SEPARATE && m_dir.td_nstrips != m_dir.td_samplesperpixel))
                    {
                        missingRequired("StripByteCounts");
                        return false;
                    }

                    if (!estimateStripByteCounts(dir, dircount))
                        return false;
                }
                else if (m_dir.td_nstrips == 1 && m_dir.td_stripoffset[0] != 0 && byteCountLooksBad(m_dir))
                {
                    if (!estimateStripByteCounts(dir, dircount))
                        return false;
                }
                else if (m_dir.td_planarconfig == PlanarConfig.CONTIG && m_dir.td_nstrips > 2 &&
                    m_dir.td_compression == Compression.NONE &&
                    m_dir.td_stripbytecount[0] != m_dir.td_stripbytecount[1])
                {
                    if (!estimateStripByteCounts(dir, dircount))
                        return false;
                }
            }

            dir = null;

            if (!fieldSet(FieldBit.MaxSampleValue))
                m_dir.td_maxsamplevalue = (ushort)((1 << m_dir.td_bitspersample) - 1);

            if (m_dir.td_nstrips > 1)
            {
                m_dir.td_stripbytecountsorted = true;
                for (int strip = 1; strip < m_dir.td_nstrips; strip++)
                {
                    if (m_dir.td_stripoffset[strip - 1] > m_dir.td_stripoffset[strip])
                    {
                        m_dir.td_stripbytecountsorted = false;
                        break;
                    }
                }
            }

            if (!fieldSet(FieldBit.Compression))
                SetField(TiffTag.COMPRESSION, Compression.NONE);

            if (m_dir.td_nstrips == 1 && m_dir.td_compression == Compression.NONE &&
                (m_flags & TiffFlags.STRIPCHOP) == TiffFlags.STRIPCHOP &&
                (m_flags & TiffFlags.ISTILED) != TiffFlags.ISTILED)
            {
                chopUpSingleUncompressedStrip();
            }

            m_row = -1;
            m_curstrip = -1;
            m_col = -1;
            m_curtile = -1;
            m_tilesize = -1;

            m_scanlinesize = ScanlineSize();
            if (m_scanlinesize == 0)
            {
                return false;
            }

            if (IsTiled())
            {
                m_tilesize = TileSize();
                if (m_tilesize == 0)
                {
                    return false;
                }
            }
            else
            {
                if (StripSize() == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads a custom directory from the arbitrary offset within file/stream.
        /// </summary>
        public bool ReadCustomDirectory(long offset, TiffFieldInfo[] info, int count)
        {
            const string module = "ReadCustomDirectory";

            setupFieldInfo(info, count);

            uint dummyNextDirOff;
            TiffDirEntry[] dir;
            short dircount = fetchDirectory((uint)offset, out dir, out dummyNextDirOff);
            if (dircount == 0)
            {
                return false;
            }

            FreeDirectory();
            m_dir = new TiffDirectory();

            int fix = 0;
            for (short i = 0; i < dircount; i++)
            {
                if ((m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
                {
                    short temp = (short)dir[i].tdir_tag;
                    SwabShort(ref temp);
                    dir[i].tdir_tag = (TiffTag)(ushort)temp;

                    temp = (short)dir[i].tdir_type;
                    SwabShort(ref temp);
                    dir[i].tdir_type = (TiffType)temp;

                    SwabLong(ref dir[i].tdir_count);
                    SwabUInt(ref dir[i].tdir_offset);
                }

                if (fix >= m_nfields || dir[i].tdir_tag == TiffTag.IGNORE)
                    continue;

                while (fix < m_nfields && m_fieldinfo[fix].Tag < dir[i].tdir_tag)
                    fix++;

                if (fix >= m_nfields || m_fieldinfo[fix].Tag != dir[i].tdir_tag)
                {
                    TiffFieldInfo[] arr = new TiffFieldInfo[1];
                    arr[0] = createAnonFieldInfo(dir[i].tdir_tag, dir[i].tdir_type);
                    MergeFieldInfo(arr, 1);

                    fix = 0;
                    while (fix < m_nfields && m_fieldinfo[fix].Tag < dir[i].tdir_tag)
                        fix++;
                }

                if (m_fieldinfo[fix].Bit == FieldBit.Ignore)
                {
                    dir[i].tdir_tag = TiffTag.IGNORE;
                    continue;
                }

                TiffFieldInfo fip = m_fieldinfo[fix];
                while (dir[i].tdir_type != fip.Type && fix < m_nfields)
                {
                    if (fip.Type == TiffType.ANY)
                    {
                        break;
                    }

                    fip = m_fieldinfo[++fix];
                    if (fix >= m_nfields || fip.Tag != dir[i].tdir_tag)
                    {
                        dir[i].tdir_tag = TiffTag.IGNORE;
                        continue;
                    }
                }

                if (fip.ReadCount != TiffFieldInfo.Variable &&
                    fip.ReadCount != TiffFieldInfo.Variable2)
                {
                    int expected = fip.ReadCount;
                    if (fip.ReadCount == TiffFieldInfo.Spp)
                        expected = m_dir.td_samplesperpixel;

                    if (!checkDirCount(dir[i], expected))
                    {
                        dir[i].tdir_tag = TiffTag.IGNORE;
                        continue;
                    }
                }

                switch (dir[i].tdir_tag)
                {
                    case TiffTag.EXIF_SUBJECTDISTANCE:
                        fetchSubjectDistance(dir[i]);
                        break;
                    default:
                        fetchNormalTag(dir[i]);
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Calculates the size in bytes of a row of data as it would be returned in a call to
        /// <see cref="O:Syncfusion.Pdf.Compression.JBIG2.Tiff.ReadScanline"/>, or as it would be
        /// expected in a call to <see cref="O:Syncfusion.Pdf.Compression.JBIG2.Tiff.WriteScanline"/>.
        /// </summary>
        public int ScanlineSize()
        {
            int scanline;
            if (m_dir.td_planarconfig == PlanarConfig.CONTIG)
            {
                if (m_dir.td_photometric == Photometric.YCBCR && !IsUpSampled())
                {
                    FieldValue[] result = GetFieldDefaulted(TiffTag.YCBCRSUBSAMPLING);
                    short ycbcrsubsampling0 = result[0].ToShort();

                    if (ycbcrsubsampling0 == 0)
                    {
                        return 0;
                    }

                    scanline = roundUp(m_dir.td_imagewidth, ycbcrsubsampling0);
                    scanline = howMany8(multiply(scanline, m_dir.td_bitspersample, "ScanlineSize"));
                    return summarize(scanline, multiply(2, scanline / ycbcrsubsampling0, "VStripSize"), "VStripSize");
                }
                else
                {
                    scanline = multiply(m_dir.td_imagewidth, m_dir.td_samplesperpixel, "ScanlineSize");
                }
            }
            else
            {
                scanline = m_dir.td_imagewidth;
            }

            return howMany8(multiply(scanline, m_dir.td_bitspersample, "ScanlineSize"));
        }

        /// <summary>
        /// Computes the number of bytes in a row-aligned strip.
        /// </summary>
        public int StripSize()
        {
            int rps = m_dir.td_rowsperstrip;
            if (rps > m_dir.td_imagelength)
                rps = m_dir.td_imagelength;

            return VStripSize(rps);
        }

        /// <summary>
        /// Computes the number of bytes in a row-aligned strip with specified number of rows.
        /// </summary>
        public int VStripSize(int rowCount)
        {
            if (rowCount == -1)
                rowCount = m_dir.td_imagelength;

            if (m_dir.td_planarconfig == PlanarConfig.CONTIG &&
                m_dir.td_photometric == Photometric.YCBCR && !IsUpSampled())
            {
                FieldValue[] result = GetFieldDefaulted(TiffTag.YCBCRSUBSAMPLING);
                short ycbcrsubsampling0 = result[0].ToShort();
                short ycbcrsubsampling1 = result[1].ToShort();

                int samplingarea = ycbcrsubsampling0 * ycbcrsubsampling1;
                if (samplingarea == 0)
                {
                    return 0;
                }

                int w = roundUp(m_dir.td_imagewidth, ycbcrsubsampling0);
                int scanline = howMany8(multiply(w, m_dir.td_bitspersample, "VStripSize"));
                rowCount = roundUp(rowCount, ycbcrsubsampling1);
                scanline = multiply(rowCount, scanline, "VStripSize");
                return summarize(scanline, multiply(2, scanline / samplingarea, "VStripSize"), "VStripSize");
            }

            return multiply(rowCount, ScanlineSize(), "VStripSize");
        }

        /// <summary>
        /// Computes the number of bytes in a raw (i.e. not decoded) strip.
        /// </summary>
        public long RawStripSize(int strip)
        {
            long bytecount = m_dir.td_stripbytecount[strip];
            if (bytecount <= 0)
            {
                bytecount = -1;
            }

            return bytecount;
        }

        /// <summary>
        /// Retrives the number of strips in the image.
        /// </summary>
        public int NumberOfStrips()
        {
            int nstrips = (m_dir.td_rowsperstrip == -1 ? 1 : howMany(m_dir.td_imagelength, m_dir.td_rowsperstrip));
            if (m_dir.td_planarconfig == PlanarConfig.SEPARATE)
                nstrips = multiply(nstrips, m_dir.td_samplesperpixel, "NumberOfStrips");

            return nstrips;
        }

        /// <summary>
        /// Compute the number of bytes in a row-aligned tile.
        /// </summary>
        public int TileSize()
        {
            return VTileSize(m_dir.td_tilelength);
        }

        /// <summary>
        /// Computes the number of bytes in a row-aligned tile with specified number of rows.
        /// </summary>
        public int VTileSize(int rowCount)
        {
            if (m_dir.td_tilelength == 0 || m_dir.td_tilewidth == 0 || m_dir.td_tiledepth == 0)
                return 0;

            int tilesize;
            if (m_dir.td_planarconfig == PlanarConfig.CONTIG &&
                m_dir.td_photometric == Photometric.YCBCR && !IsUpSampled())
            {
                int w = roundUp(m_dir.td_tilewidth, m_dir.td_ycbcrsubsampling[0]);
                int rowsize = howMany8(multiply(w, m_dir.td_bitspersample, "VTileSize"));
                int samplingarea = m_dir.td_ycbcrsubsampling[0] * m_dir.td_ycbcrsubsampling[1];
                if (samplingarea == 0)
                {
                    return 0;
                }

                rowCount = roundUp(rowCount, m_dir.td_ycbcrsubsampling[1]);
                // NB: don't need howMany here 'cuz everything is rounded
                tilesize = multiply(rowCount, rowsize, "VTileSize");
                tilesize = summarize(tilesize, multiply(2, tilesize / samplingarea, "VTileSize"), "VTileSize");
            }
            else
            {
                tilesize = multiply(rowCount, TileRowSize(), "VTileSize");
            }

            return multiply(tilesize, m_dir.td_tiledepth, "VTileSize");
        }

        /// <summary>
        /// Compute the number of bytes in each row of a tile.
        /// </summary>
        public int TileRowSize()
        {
            if (m_dir.td_tilelength == 0 || m_dir.td_tilewidth == 0)
                return 0;

            int rowsize = multiply(m_dir.td_bitspersample, m_dir.td_tilewidth, "TileRowSize");
            if (m_dir.td_planarconfig == PlanarConfig.CONTIG)
                rowsize = multiply(rowsize, m_dir.td_samplesperpixel, "TileRowSize");

            return howMany8(rowsize);
        }

        /// <summary>
        /// Computes which tile contains the specified coordinates (x, y, z, plane).
        /// </summary>
        public int ComputeTile(int x, int y, int z, short plane)
        {
            if (m_dir.td_imagedepth == 1)
                z = 0;

            int dx = m_dir.td_tilewidth;
            if (dx == -1)
                dx = m_dir.td_imagewidth;

            int dy = m_dir.td_tilelength;
            if (dy == -1)
                dy = m_dir.td_imagelength;

            int dz = m_dir.td_tiledepth;
            if (dz == -1)
                dz = m_dir.td_imagedepth;

            int tile = 1;
            if (dx != 0 && dy != 0 && dz != 0)
            {
                int xpt = howMany(m_dir.td_imagewidth, dx);
                int ypt = howMany(m_dir.td_imagelength, dy);
                int zpt = howMany(m_dir.td_imagedepth, dz);

                if (m_dir.td_planarconfig == PlanarConfig.SEPARATE)
                    tile = (xpt * ypt * zpt) * plane + (xpt * ypt) * (z / dz) + xpt * (y / dy) + x / dx;
                else
                    tile = (xpt * ypt) * (z / dz) + xpt * (y / dy) + x / dx;
            }

            return tile;
        }

        /// <summary>
        /// Checks whether the specified (x, y, z, plane) coordinates are within the bounds of
        /// the image.
        /// </summary>
        public bool CheckTile(int x, int y, int z, short plane)
        {
            if (x >= m_dir.td_imagewidth)
            {
                return false;
            }

            if (y >= m_dir.td_imagelength)
            {
                return false;
            }

            if (z >= m_dir.td_imagedepth)
            {
                return false;
            }

            if (m_dir.td_planarconfig == PlanarConfig.SEPARATE && plane >= m_dir.td_samplesperpixel)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrives the number of tiles in the image.
        /// </summary>
        public int NumberOfTiles()
        {
            int dx = m_dir.td_tilewidth;
            if (dx == -1)
                dx = m_dir.td_imagewidth;

            int dy = m_dir.td_tilelength;
            if (dy == -1)
                dy = m_dir.td_imagelength;

            int dz = m_dir.td_tiledepth;
            if (dz == -1)
                dz = m_dir.td_imagedepth;

            int ntiles;
            if (dx == 0 || dy == 0 || dz == 0)
            {
                ntiles = 0;
            }
            else
            {
                ntiles = multiply(
                    multiply(howMany(m_dir.td_imagewidth, dx), howMany(m_dir.td_imagelength, dy), "NumberOfTiles"),
                    howMany(m_dir.td_imagedepth, dz), "NumberOfTiles");
            }
            
            if (m_dir.td_planarconfig == PlanarConfig.SEPARATE)
                ntiles = multiply(ntiles, m_dir.td_samplesperpixel, "NumberOfTiles");

            return ntiles;
        }

        /// <summary>
        /// Gets the value indicating whether the image data of this <see cref="Tiff"/> has a
        /// tiled organization.
        /// </summary>
        public bool IsTiled()
        {
            return ((m_flags & TiffFlags.ISTILED) == TiffFlags.ISTILED);
        }

        /// <summary>
        /// Gets the value indicating whether the image data returned through the read interface
        /// methods is being up-sampled.
        /// </summary>
        public bool IsUpSampled()
        {
            return ((m_flags & TiffFlags.UPSAMPLED) == TiffFlags.UPSAMPLED);
        }

        /// <summary>
        /// Gets the tiff stream.
        /// </summary>
        public TiffStream GetStream()
        {
            return m_stream;
        }

        /// <summary>
        /// Sets up the data buffer used to read raw (encoded) data from a file.
        /// </summary>
        public void ReadBufferSetup(byte[] buffer, int size)
        {
            m_rawdata = null;

            if (buffer != null)
            {
                m_rawdatasize = size;
                m_rawdata = buffer;
                m_flags &= ~TiffFlags.MYBUFFER;
            }
            else
            {
                m_rawdatasize = roundUp(size, 1024);
                if (m_rawdatasize > 0)
                {
                    m_rawdata = new byte[m_rawdatasize];
                }
                else
                {
                    m_rawdatasize = 0;
                }

                m_flags |= TiffFlags.MYBUFFER;
            }
        }

        /// <summary>
        /// Setups the strips.
        /// </summary>
        public bool SetupStrips()
        {
            m_dir.td_nstrips = m_dir.td_stripsperimage;

            if (m_dir.td_planarconfig == PlanarConfig.SEPARATE)
                m_dir.td_stripsperimage /= m_dir.td_samplesperpixel;

            m_dir.td_stripoffset = new uint[m_dir.td_nstrips];
            m_dir.td_stripbytecount = new uint[m_dir.td_nstrips];

            setFieldBit(FieldBit.StripOffsets);
            setFieldBit(FieldBit.StripByteCounts);
            return true;
        }

        /// <summary>
        /// Releases storage associated with current directory.
        /// </summary>
        public void FreeDirectory()
        {
            if (m_dir != null)
            {
                clearFieldBit(FieldBit.YCbCrSubsampling);
                clearFieldBit(FieldBit.YCbCrPositioning);

                m_dir = null;
            }
        }

        /// <summary>
        /// Sets the value(s) of a tag in a TIFF file/stream open for writing.
        /// </summary>
        public bool SetField(TiffTag tag, params object[] value)
        {
            if (okToChangeTag(tag))
                return m_tagmethods.SetField(this, tag, FieldValue.FromParams(value));

            return false;
        }

        /// <summary>
        /// Reads and decodes a scanline of data from an open TIFF file/stream.
        /// </summary>
        public bool ReadScanline(byte[] buffer, int row, short plane)
        {
            return ReadScanline(buffer, 0, row, plane);
        }

        /// <summary>
        /// Reads and decodes a scanline of data from an open TIFF file/stream.
        /// </summary>
        public bool ReadScanline(byte[] buffer, int offset, int row, short plane)
        {
            if (!checkRead(false))
                return false;

            bool e = seek(row, plane);
            if (e)
            {
                // Decompress desired row into user buffer.
                e = m_currentCodec.DecodeRow(buffer, offset, m_scanlinesize, plane);

                // we are now poised at the beginning of the next row
                m_row = row + 1;

                if (e)
                    postDecode(buffer, offset, m_scanlinesize);
            }

            return e;
        }

        /// <summary>
        /// Gets the number of bytes occupied by the item of given type.
        /// </summary>
        public static int DataWidth(TiffType type)
        {
            switch (type)
            {
                case TiffType.NOTYPE:
                case TiffType.BYTE:
                case TiffType.ASCII:
                case TiffType.SBYTE:
                case TiffType.UNDEFINED:
                    return 1;

                case TiffType.SHORT:
                case TiffType.SSHORT:
                    return 2;

                case TiffType.LONG:
                case TiffType.SLONG:
                case TiffType.FLOAT:
                case TiffType.IFD:
                    return 4;

                case TiffType.RATIONAL:
                case TiffType.SRATIONAL:
                case TiffType.DOUBLE:
                    return 8;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Swaps the bytes in a single 16-bit item.
        /// </summary>
        public static void SwabShort(ref short value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);

            byte temp = bytes[1];
            bytes[1] = bytes[0];
            bytes[0] = temp;

            value = (short)(bytes[0] & 0xFF);
            value += (short)((bytes[1] & 0xFF) << 8);
        }

        /// <summary>
        /// Swaps the bytes in a single 32-bit item.
        /// </summary>
        public static void SwabLong(ref int value)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);

            byte temp = bytes[3];
            bytes[3] = bytes[0];
            bytes[0] = temp;

            temp = bytes[2];
            bytes[2] = bytes[1];
            bytes[1] = temp;

            value = bytes[0] & 0xFF;
            value += (bytes[1] & 0xFF) << 8;
            value += (bytes[2] & 0xFF) << 16;
            value += bytes[3] << 24;
        }

        /// <summary>
        /// Swaps the bytes in specified number of values in the array of 16-bit items.
        /// </summary>
        public static void SwabArrayOfShort(short[] array, int count)
        {
            SwabArrayOfShort(array, 0, count);
        }

        /// <summary>
        /// Swaps the bytes in specified number of values in the array of 16-bit items starting at
        /// specified offset.
        /// </summary>
        public static void SwabArrayOfShort(short[] array, int offset, int count)
        {
            byte[] bytes = new byte[2];
            for (int i = 0; i < count; i++, offset++)
            {
                bytes[0] = (byte)array[offset];
                bytes[1] = (byte)(array[offset] >> 8);

                byte temp = bytes[1];
                bytes[1] = bytes[0];
                bytes[0] = temp;

                array[offset] = (short)(bytes[0] & 0xFF);
                array[offset] += (short)((bytes[1] & 0xFF) << 8);
            }
        }

        /// <summary>
        /// Swaps the bytes in specified number of values in the array of triples (24-bit items)
        /// starting at specified offset.
        /// </summary>
        public static void SwabArrayOfTriples(byte[] array, int offset, int count)
        {
            while (count-- > 0)
            {
                byte t = array[offset + 2];
                array[offset + 2] = array[offset];
                array[offset] = t;
                offset += 3;
            }
        }

        /// <summary>
        /// Swaps the bytes in specified number of values in the array of 32-bit items.
        /// </summary>
        public static void SwabArrayOfLong(int[] array, int count)
        {
            SwabArrayOfLong(array, 0, count);
        }

        /// <summary>
        /// Swaps the bytes in specified number of values in the array of 32-bit items
        /// starting at specified offset.
        /// </summary>
        public static void SwabArrayOfLong(int[] array, int offset, int count)
        {
            byte[] bytes = new byte[4];

            for (int i = 0; i < count; i++, offset++)
            {
                bytes[0] = (byte)array[offset];
                bytes[1] = (byte)(array[offset] >> 8);
                bytes[2] = (byte)(array[offset] >> 16);
                bytes[3] = (byte)(array[offset] >> 24);

                byte temp = bytes[3];
                bytes[3] = bytes[0];
                bytes[0] = temp;

                temp = bytes[2];
                bytes[2] = bytes[1];
                bytes[1] = temp;

                array[offset] = bytes[0] & 0xFF;
                array[offset] += (bytes[1] & 0xFF) << 8;
                array[offset] += (bytes[2] & 0xFF) << 16;
                array[offset] += bytes[3] << 24;
            }
        }

        /// <summary>
        /// Swaps the bytes in specified number of values in the array of double-precision
        /// floating-point numbers.
        /// </summary>
        public static void SwabArrayOfDouble(double[] array, int count)
        {
            SwabArrayOfDouble(array, 0, count);
        }

        /// <summary>
        /// Swaps the bytes in specified number of values in the array of double-precision
        /// floating-point numbers starting at specified offset.
        /// </summary>
        public static void SwabArrayOfDouble(double[] array, int offset, int count)
        {
            int[] ints = new int[count * sizeof(double) / sizeof(int)];
            Buffer.BlockCopy(array, offset * sizeof(double), ints, 0, ints.Length * sizeof(int));

            SwabArrayOfLong(ints, ints.Length);

            int pos = 0;
            while (count-- > 0)
            {
                int temp = ints[pos];
                ints[pos] = ints[pos + 1];
                ints[pos + 1] = temp;
                pos += 2;
            }

            Buffer.BlockCopy(ints, 0, array, offset * sizeof(double), ints.Length * sizeof(int));
        }

        /// <summary>
        /// Replaces specified number of bytes in <paramref name="buffer"/> with the
        /// equivalent bit-reversed bytes.
        /// </summary>
        public static void ReverseBits(byte[] buffer, int count)
        {
            ReverseBits(buffer, 0, count);
        }

        /// <summary>
        /// Replaces specified number of bytes in <paramref name="buffer"/> with the
        /// equivalent bit-reversed bytes starting at specified offset.
        /// </summary>
        public static void ReverseBits(byte[] buffer, int offset, int count)
        {
            for (; count > 8; count -= 8)
            {
                buffer[offset + 0] = TIFFBitRevTable[buffer[offset + 0]];
                buffer[offset + 1] = TIFFBitRevTable[buffer[offset + 1]];
                buffer[offset + 2] = TIFFBitRevTable[buffer[offset + 2]];
                buffer[offset + 3] = TIFFBitRevTable[buffer[offset + 3]];
                buffer[offset + 4] = TIFFBitRevTable[buffer[offset + 4]];
                buffer[offset + 5] = TIFFBitRevTable[buffer[offset + 5]];
                buffer[offset + 6] = TIFFBitRevTable[buffer[offset + 6]];
                buffer[offset + 7] = TIFFBitRevTable[buffer[offset + 7]];
                offset += 8;
            }

            while (count-- > 0)
            {
                buffer[offset] = TIFFBitRevTable[buffer[offset]];
                offset++;
            }
        }

        /// <summary>
        /// Retrieves a bit reversal table.
        /// </summary>
        public static byte[] GetBitRevTable(bool reversed)
        {
            return (reversed ? TIFFBitRevTable : TIFFNoBitRevTable);
        }

        /// <summary>
        /// Converts a byte buffer into array of 32-bit values.
        /// </summary>
        public static int[] ByteArrayToInts(byte[] buffer, int offset, int count)
        {
            int intCount = count / sizeof(int);
            int[] integers = new int[intCount];
            Buffer.BlockCopy(buffer, offset, integers, 0, intCount * sizeof(int));            
            return integers;
        }

        /// <summary>
        /// Converts array of 32-bit values into array of bytes.
        /// </summary>
        public static void IntsToByteArray(int[] source, int srcOffset, int srcCount, byte[] bytes, int offset)
        {
            Buffer.BlockCopy(source, srcOffset * sizeof(int), bytes, offset, srcCount * sizeof(int));
        }

        /// <summary>
        /// Converts a byte buffer into array of 16-bit values.
        /// </summary>
        public static short[] ByteArrayToShorts(byte[] buffer, int offset, int count)
        {
            int shortCount = count / sizeof(short);
            short[] shorts = new short[shortCount];
            Buffer.BlockCopy(buffer, offset, shorts, 0, shortCount * sizeof(short));
            return shorts;
        }

        /// <summary>
        /// Converts array of 16-bit values into array of bytes.
        /// </summary>
        public static void ShortsToByteArray(short[] source, int srcOffset, int srcCount, byte[] bytes, int offset)
        {
            Buffer.BlockCopy(source, srcOffset * sizeof(short), bytes, offset, srcCount * sizeof(short));
        }
    }
}
