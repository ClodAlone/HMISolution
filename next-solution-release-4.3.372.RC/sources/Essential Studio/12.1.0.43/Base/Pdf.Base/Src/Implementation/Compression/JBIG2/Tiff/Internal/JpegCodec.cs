#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.Compression.JBIG2;

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    class JpegCodec : TiffCodec
    {
        public const int FIELD_JPEGTABLES = (FieldBit.Codec + 0);
        public const int FIELD_RECVPARAMS = (FieldBit.Codec + 1);
        public const int FIELD_SUBADDRESS = (FieldBit.Codec + 2);
        public const int FIELD_RECVTIME = (FieldBit.Codec + 3);
        public const int FIELD_FAXDCS = (FieldBit.Codec + 4);

        internal DecompressStruct m_decompression;
        internal CommonStruct m_common;

        internal int m_h_sampling;
        internal int m_v_sampling;

        internal byte[] m_jpegtables;
        internal int m_jpegtables_length;
        internal int m_jpegquality;
        internal JpegColorMode m_jpegcolormode;
        internal JpegTablesMode m_jpegtablesmode;

        internal bool m_ycbcrsampling_fetched;

        internal int m_recvparams;
        internal string m_subaddress;
        internal int m_recvtime;
        internal string m_faxdcs;

        private static readonly TiffFieldInfo[] jpegFieldInfo = 
        {
            new TiffFieldInfo(TiffTag.JPEGTABLES, -3, -3, TiffType.UNDEFINED, FIELD_JPEGTABLES, false, true, "JPEGTables"), 
            new TiffFieldInfo(TiffTag.JPEGQUALITY, 0, 0, TiffType.ANY, FieldBit.Pseudo, true, false, string.Empty), 
            new TiffFieldInfo(TiffTag.JPEGCOLORMODE, 0, 0, TiffType.ANY, FieldBit.Pseudo, false, false, string.Empty), 
            new TiffFieldInfo(TiffTag.JPEGTABLESMODE, 0, 0, TiffType.ANY, FieldBit.Pseudo, false, false, string.Empty), 
            new TiffFieldInfo(TiffTag.FAXRECVPARAMS, 1, 1, TiffType.LONG, FIELD_RECVPARAMS, true, false, "FaxRecvParams"), 
            new TiffFieldInfo(TiffTag.FAXSUBADDRESS, -1, -1, TiffType.ASCII, FIELD_SUBADDRESS, true, false, "FaxSubAddress"), 
            new TiffFieldInfo(TiffTag.FAXRECVTIME, 1, 1, TiffType.LONG, FIELD_RECVTIME, true, false, "FaxRecvTime"), 
            new TiffFieldInfo(TiffTag.FAXDCS, -1, -1, TiffType.ASCII, FIELD_FAXDCS, true, false, "FaxDcs"), 
        };

        private bool m_rawDecode;
        private bool m_rawEncode;
        private TiffTagMethods m_tagMethods;
        private TiffTagMethods m_parentTagMethods;
        private bool m_cinfo_initialized;
        private Photometric m_photometric;
        private int m_bytesperline; 
        private byte[][][] m_ds_buffer = new byte[JpegConstants.MAX_COMPONENTS][][];
        private int m_scancount;
        private int m_samplesperclump;

        public JpegCodec(Tiff tif, Compression scheme, string name)
            : base(tif, scheme, name)
        {
            m_tagMethods = new JpegCodecTagMethods();
        }

        private void cleanState()
        {
            m_decompression = null;
            m_common = null;

            m_h_sampling = 0;
            m_v_sampling = 0;

            m_jpegtables = null;
            m_jpegtables_length = 0;
            m_jpegquality = 0;
            m_jpegcolormode = 0;
            m_jpegtablesmode = 0;

            m_ycbcrsampling_fetched = false;

            m_recvparams = 0;
            m_subaddress = null;
            m_recvtime = 0;
            m_faxdcs = null;
            m_rawDecode = false;
            m_rawEncode = false;

            m_cinfo_initialized = false;

            m_photometric = 0;

            m_bytesperline = 0;
            m_ds_buffer = new byte[JpegConstants.MAX_COMPONENTS][][];
            m_scancount = 0;
            m_samplesperclump = 0;
        }

        public override bool Init()
        {
            m_tif.MergeFieldInfo(jpegFieldInfo, jpegFieldInfo.Length);
            cleanState();
            m_parentTagMethods = m_tif.m_tagmethods;
            m_tif.m_tagmethods = m_tagMethods;

            m_jpegquality = 75; 
            m_jpegcolormode = JpegColorMode.RGB;
            m_jpegtablesmode = JpegTablesMode.QUANT | JpegTablesMode.HUFF;

            m_tif.m_flags |= TiffFlags.NOBITREV;

            if (m_tif.m_diroff == 0)
            {
                const int SIZE_OF_JPEGTABLES = 2000;
                m_jpegtables_length = SIZE_OF_JPEGTABLES;
                m_jpegtables = new byte[m_jpegtables_length];
            }

            m_tif.setFieldBit(FieldBit.YCbCrSubsampling);
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this codec can decode data.
        /// </summary>
        public override bool CanDecode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Setups the decoder part of the codec.
        /// </summary>
        public override bool SetupDecode()
        {
            return JPEGSetupDecode();
        }

        /// <summary>
        /// Prepares the decoder part of the codec for a decoding.
        /// </summary>
        public override bool PreDecode(short plane)
        {
            return JPEGPreDecode(plane);
        }

        /// <summary>
        /// Decodes one row of image data.
        /// </summary>
        public override bool DecodeRow(byte[] buffer, int offset, int count, short plane)
        {
            if (m_rawDecode)
                return JPEGDecodeRaw(buffer, offset, count, plane);

            return JPEGDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one strip of image data.
        /// </summary>
        public override bool DecodeStrip(byte[] buffer, int offset, int count, short plane)
        {
            if (m_rawDecode)
                return JPEGDecodeRaw(buffer, offset, count, plane);

            return JPEGDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one tile of image data.
        /// </summary>
        public override bool DecodeTile(byte[] buffer, int offset, int count, short plane)
        {
            if (m_rawDecode)
                return JPEGDecodeRaw(buffer, offset, count, plane);

            return JPEGDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Cleanups the state of the codec.
        /// </summary>
        public override void Cleanup()
        {
            JPEGCleanup();
        }

        /// <summary>
        /// Calculates and/or constrains a strip size.
        /// </summary>
        public override int DefStripSize(int size)
        {
            return JPEGDefaultStripSize(size);
        }

        /// <summary>
        /// Calculate and/or constrains a tile size
        /// </summary>
        public override void DefTileSize(ref int width, ref int height)
        {
            JPEGDefaultTileSize(ref width, ref height);
        }

        public bool InitializeJpeg(bool force_encode, bool force_decode)
        {
            int[] byte_counts = null;
            bool data_is_empty = true;
            bool decompress;

            if (m_cinfo_initialized)
            {
                if (force_encode && m_common.IsDecompressor)
                    TIFFjpeg_destroy();
                else if (force_decode && !m_common.IsDecompressor)
                    TIFFjpeg_destroy();
                else
                    return true;

                m_cinfo_initialized = false;
            }

            FieldValue[] result = m_tif.GetField(TiffTag.TILEBYTECOUNTS);
            if (m_tif.IsTiled() && result != null)
            {
                byte_counts = result[0].ToIntArray();
                if (byte_counts != null)
                    data_is_empty = byte_counts[0] == 0;
            }

            result = m_tif.GetField(TiffTag.STRIPBYTECOUNTS);
            if (!m_tif.IsTiled() && result != null)
            {
                byte_counts = result[0].ToIntArray();
                if (byte_counts != null)
                    data_is_empty = byte_counts[0] == 0;
            }

            if (force_decode)
                decompress = true;
            else if (force_encode)
                decompress = false;
            else if (m_tif.m_mode == Tiff.O_RDONLY)
                decompress = true;
            else if (data_is_empty)
                decompress = false;
            else
                decompress = true;

            if (decompress)
            {
                if (!TIFFjpeg_create_decompress())
                    return false;
            }
            else
            {
                return false;
            }

            m_cinfo_initialized = true;
            return true;
        }

        public Tiff GetTiff()
        {
            return m_tif;
        }

        public void JPEGResetUpsampled()
        {
            m_tif.m_flags &= ~TiffFlags.UPSAMPLED;
            if (m_tif.m_dir.td_planarconfig == PlanarConfig.CONTIG)
            {
                if (m_tif.m_dir.td_photometric == Photometric.YCBCR && m_jpegcolormode == JpegColorMode.RGB)
                    m_tif.m_flags |= TiffFlags.UPSAMPLED;
            }

            if (m_tif.m_tilesize > 0)
                m_tif.m_tilesize = m_tif.IsTiled() ? m_tif.TileSize() : -1;

            if (m_tif.m_scanlinesize > 0)
                m_tif.m_scanlinesize = m_tif.ScanlineSize();
        }

        private void JPEGCleanup()
        {
            m_tif.m_tagmethods = m_parentTagMethods;

            if (m_cinfo_initialized)
            {
                TIFFjpeg_destroy();
            }
        }

        private bool JPEGPreDecode(short s)
        {
            TiffDirectory td = m_tif.m_dir;
            const string module = "JPEGPreDecode";
            int segment_width;
            int segment_height;
            int ci;

            if (!TIFFjpeg_abort())
                return false;

            if (TIFFjpeg_read_header(true) != ReadResult.JPEG_HEADER_OK)
                return false;

            segment_width = td.td_imagewidth;
            segment_height = td.td_imagelength - m_tif.m_row;
            if (m_tif.IsTiled())
            {
                segment_width = td.td_tilewidth;
                segment_height = td.td_tilelength;
                m_bytesperline = m_tif.TileRowSize();
            }
            else
            {
                if (segment_height > td.td_rowsperstrip && td.td_rowsperstrip != -1)
                    segment_height = td.td_rowsperstrip;
                m_bytesperline = m_tif.oldScanlineSize();
            }

            if (td.td_planarconfig == PlanarConfig.SEPARATE && s > 0)
            {
                segment_width = Tiff.howMany(segment_width, m_h_sampling);
                segment_height = Tiff.howMany(segment_height, m_v_sampling);
            }

            if (m_decompression.Image_width < segment_width || m_decompression.Image_height < segment_height)
            {
            }

            if (m_decompression.Image_width > segment_width || m_decompression.Image_height > segment_height)
            {
                return false;
            }

            if (m_decompression.Num_components != (td.td_planarconfig == PlanarConfig.CONTIG ? (int)td.td_samplesperpixel : 1))
            {
                return false;
            }

            if (m_decompression.Data_precision != td.td_bitspersample)
            {
                return false;
            }

            if (td.td_planarconfig == PlanarConfig.CONTIG)
            {
                if (m_decompression.Comp_info[0].H_samp_factor != m_h_sampling ||
                    m_decompression.Comp_info[0].V_samp_factor != m_v_sampling)
                {
                    if (m_decompression.Comp_info[0].H_samp_factor > m_h_sampling ||
                        m_decompression.Comp_info[0].V_samp_factor > m_v_sampling)
                    {
                        return false;
                    }

                    if (m_tif.FindFieldInfo((TiffTag)33918, TiffType.ANY) == null)
                    {
                        m_h_sampling = m_decompression.Comp_info[0].H_samp_factor;
                        m_v_sampling = m_decompression.Comp_info[0].V_samp_factor;
                    }
                }

                for (ci = 1; ci < m_decompression.Num_components; ci++)
                {
                    if (m_decompression.Comp_info[ci].H_samp_factor != 1 ||
                        m_decompression.Comp_info[ci].V_samp_factor != 1)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (m_decompression.Comp_info[0].H_samp_factor != 1 ||
                    m_decompression.Comp_info[0].V_samp_factor != 1)
                {
                    return false;
                }
            }

            bool downsampled_output = false;
            if (td.td_planarconfig == PlanarConfig.CONTIG &&
                m_photometric == Photometric.YCBCR &&
                m_jpegcolormode == JpegColorMode.RGB)
            {
                m_decompression.Jpeg_color_space = J_COLOR_SPACE.JCS_YCbCr;
                m_decompression.Out_color_space = J_COLOR_SPACE.JCS_RGB;
            }
            else
            {
                m_decompression.Jpeg_color_space = J_COLOR_SPACE.JCS_UNKNOWN;
                m_decompression.Out_color_space = J_COLOR_SPACE.JCS_UNKNOWN;
                if (td.td_planarconfig == PlanarConfig.CONTIG &&
                    (m_h_sampling != 1 || m_v_sampling != 1))
                {
                    downsampled_output = true;
                }
            }

            if (downsampled_output)
            {
                m_decompression.Raw_data_out = true;
                m_rawDecode = true;
            }
            else
            {
                m_decompression.Raw_data_out = false;
                m_rawDecode = false;
            }

            if (!TIFFjpeg_start_decompress())
                return false;

            if (downsampled_output)
            {
                if (!alloc_downsampled_buffers(m_decompression.Comp_info, m_decompression.Num_components))
                    return false;

                m_scancount = JpegConstants.DCTSIZE; /* mark buffer empty */
            }

            return true;
        }

        private bool JPEGSetupDecode()
        {
            TiffDirectory td = m_tif.m_dir;

            InitializeJpeg(false, true);

            if (m_tif.fieldSet(FIELD_JPEGTABLES))
            {
                m_decompression.Src = new JpegTablesSource(this);
                if (TIFFjpeg_read_header(false) != ReadResult.JPEG_HEADER_TABLES_ONLY)
                {
                    return false;
                }
            }

            m_photometric = td.td_photometric;
            switch (m_photometric)
            {
                case Photometric.YCBCR:
                    m_h_sampling = td.td_ycbcrsubsampling[0];
                    m_v_sampling = td.td_ycbcrsubsampling[1];
                    break;
                default:
                    m_h_sampling = 1;
                    m_v_sampling = 1;
                    break;
            }

            m_decompression.Src = new JpegStdSource(this);
            m_tif.m_postDecodeMethod = Tiff.PostDecodeMethodType.pdmNone;
            return true;
        }

        private int TIFFjpeg_read_scanlines(byte[][] scanlines, int max_lines)
        {
            int n = 0;
            try
            {
                n = m_decompression.jpeg_read_scanlines(scanlines, max_lines);
            }
            catch (Exception)
            {
                return -1;
            }

            return n;
        }

        /// <summary>
        /// Decode a chunk of pixels.
        /// "Standard" case: returned data is not downsampled.
        /// </summary>
        private bool JPEGDecode(byte[] buffer, int offset, int count, short plane)
        {
            int nrows = count / m_bytesperline;
            if ((count % m_bytesperline) != 0)
            { }

            if (nrows > (int)m_decompression.Image_height)
                nrows = m_decompression.Image_height;

            if (nrows != 0)
            {
                byte[][] bufptr = new byte[1][];
                bufptr[0] = new byte[m_bytesperline];
                do
                {
                    Array.Clear(bufptr[0], 0, m_bytesperline);
                    if (TIFFjpeg_read_scanlines(bufptr, 1) != 1)
                        return false;

                    ++m_tif.m_row;
                    Buffer.BlockCopy(bufptr[0], 0, buffer, offset, m_bytesperline);
                    offset += m_bytesperline;
                    count -= m_bytesperline;
                }
                while (--nrows > 0);
            }

            return m_decompression.Output_scanline < m_decompression.Output_height || TIFFjpeg_finish_decompress();
        }

        /// <summary>
        /// Decode a chunk of pixels. 
        /// </summary>
        private bool JPEGDecodeRaw(byte[] buffer, int offset, int count, short plane)
        {
            int nrows = m_decompression.Image_height;
            if (nrows != 0)
            {
                int clumps_per_line = m_decompression.Comp_info[1].Downsampled_width;

                do
                {
                    if (m_scancount >= JpegConstants.DCTSIZE)
                    {
                        int n = m_decompression.Max_v_samp_factor * JpegConstants.DCTSIZE;
                        if (TIFFjpeg_read_raw_data(m_ds_buffer, n) != n)
                            return false;

                        m_scancount = 0;
                    }

                    int clumpoffset = 0;
                    for (int ci = 0; ci < m_decompression.Num_components; ci++)
                    {
                        int hsamp = m_decompression.Comp_info[ci].H_samp_factor;
                        int vsamp = m_decompression.Comp_info[ci].V_samp_factor;

                        for (int ypos = 0; ypos < vsamp; ypos++)
                        {
                            byte[] inBuf = m_ds_buffer[ci][m_scancount * vsamp + ypos];
                            int inptr = 0;

                            int outptr = offset + clumpoffset;
                            if (outptr >= buffer.Length)
                                break;

                            if (hsamp == 1)
                            {
                                for (int nclump = clumps_per_line; nclump-- > 0; )
                                {
                                    buffer[outptr] = inBuf[inptr];
                                    inptr++;
                                    outptr += m_samplesperclump;
                                }
                            }
                            else
                            {
                                for (int nclump = clumps_per_line; nclump-- > 0; )
                                {
                                    for (int xpos = 0; xpos < hsamp; xpos++)
                                    {
                                        buffer[outptr + xpos] = inBuf[inptr];
                                        inptr++;
                                    }

                                    outptr += m_samplesperclump;
                                }
                            }

                            clumpoffset += hsamp;
                        }
                    }

                    ++m_scancount;
                    m_tif.m_row += m_v_sampling;

                    offset += m_bytesperline;
                    count -= m_bytesperline;
                    nrows -= m_v_sampling;
                }
                while (nrows > 0);
            }

            return m_decompression.Output_scanline < m_decompression.Output_height || TIFFjpeg_finish_decompress();
        }

        private int JPEGDefaultStripSize(int s)
        {
            s = base.DefStripSize(s);
            if (s < m_tif.m_dir.td_imagelength)
                s = Tiff.roundUp(s, m_tif.m_dir.td_ycbcrsubsampling[1] * JpegConstants.DCTSIZE);

            return s;
        }

        private void JPEGDefaultTileSize(ref int tw, ref int th)
        {
            base.DefTileSize(ref tw, ref th);
            tw = Tiff.roundUp(tw, m_tif.m_dir.td_ycbcrsubsampling[0] * JpegConstants.DCTSIZE);
            th = Tiff.roundUp(th, m_tif.m_dir.td_ycbcrsubsampling[1] * JpegConstants.DCTSIZE);
        }

        private bool TIFFjpeg_create_decompress()
        {
            try
            {
                m_decompression = new DecompressStruct();
                m_common = m_decompression;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private ReadResult TIFFjpeg_read_header(bool require_image)
        {
            ReadResult res = ReadResult.JPEG_SUSPENDED;
            try
            {
                res = m_decompression.jpeg_read_header(require_image);
            }
            catch (Exception)
            {
                return ReadResult.JPEG_SUSPENDED;
            }

            return res;
        }

        private bool TIFFjpeg_start_decompress()
        {
            try
            {
                m_decompression.jpeg_start_decompress();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private int TIFFjpeg_read_raw_data(byte[][][] data, int max_lines)
        {
            int n = 0;
            try
            {
                n = m_decompression.jpeg_read_raw_data(data, max_lines);
            }
            catch (Exception)
            {
                return -1;
            }

            return n;
        }

        private bool TIFFjpeg_finish_decompress()
        {
            bool res = true;
            try
            {
                res = m_decompression.jpeg_finish_decompress();
            }
            catch (Exception)
            {
                return false;
            }

            return res;
        }

        private bool TIFFjpeg_abort()
        {
            try
            {
                m_common.jpeg_abort();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool TIFFjpeg_destroy()
        {
            try
            {
                m_common.jpeg_destroy();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static byte[][] TIFFjpeg_alloc_sarray(int samplesperrow, int numrows)
        {
            byte[][] result = new byte[numrows][];
            for (int i = 0; i < numrows; i++)
                result[i] = new byte[samplesperrow];

            return result;
        }

        private bool alloc_downsampled_buffers(ComponentInfo[] comp_info, int num_components)
        {
            int samples_per_clump = 0;
            for (int ci = 0; ci < num_components; ci++)
            {
                ComponentInfo compptr = comp_info[ci];
                samples_per_clump += compptr.H_samp_factor * compptr.V_samp_factor;

                byte[][] buf = TIFFjpeg_alloc_sarray(
                    compptr.Width_in_blocks * JpegConstants.DCTSIZE,
                    compptr.V_samp_factor * JpegConstants.DCTSIZE);
                m_ds_buffer[ci] = buf;
            }

            m_samplesperclump = samples_per_clump;
            return true;
        }
    }

    class JpegCodecTagMethods : TiffTagMethods
    {
        public override bool SetField(Tiff tif, TiffTag tag, FieldValue[] ap)
        {
            JpegCodec sp = tif.m_currentCodec as JpegCodec;
            switch (tag)
            {
                case TiffTag.JPEGTABLES:
                    int v32 = ap[0].ToInt();
                    if (v32 == 0)
                    {
                        // XXX
                        return false;
                    }

                    sp.m_jpegtables = new byte[v32];
                    Buffer.BlockCopy(ap[1].ToByteArray(), 0, sp.m_jpegtables, 0, v32);
                    sp.m_jpegtables_length = v32;
                    tif.setFieldBit(JpegCodec.FIELD_JPEGTABLES);
                    break;

                case TiffTag.JPEGQUALITY:
                    sp.m_jpegquality = ap[0].ToInt();
                    return true; // pseudo tag

                case TiffTag.JPEGCOLORMODE:
                    sp.m_jpegcolormode = (JpegColorMode)ap[0].ToShort();
                    sp.JPEGResetUpsampled();
                    return true; // pseudo tag

                case TiffTag.PHOTOMETRIC:
                    bool ret_value = base.SetField(tif, tag, ap);
                    sp.JPEGResetUpsampled();
                    return ret_value;

                case TiffTag.JPEGTABLESMODE:
                    sp.m_jpegtablesmode = (JpegTablesMode)ap[0].ToShort();
                    return true; // pseudo tag

                case TiffTag.YCBCRSUBSAMPLING:
                    sp.m_ycbcrsampling_fetched = true;
                    return base.SetField(tif, tag, ap);

                case TiffTag.FAXRECVPARAMS:
                    sp.m_recvparams = ap[0].ToInt();
                    break;

                case TiffTag.FAXSUBADDRESS:
                    Tiff.setString(out sp.m_subaddress, ap[0].ToString());
                    break;

                case TiffTag.FAXRECVTIME:
                    sp.m_recvtime = ap[0].ToInt();
                    break;

                case TiffTag.FAXDCS:
                    Tiff.setString(out sp.m_faxdcs, ap[0].ToString());
                    break;

                default:
                    return base.SetField(tif, tag, ap);
            }

            TiffFieldInfo fip = tif.FieldWithTag(tag);
            if (fip != null)
                tif.setFieldBit(fip.Bit);
            else
                return false;

            tif.m_flags |= TiffFlags.DIRTYDIRECT;
            return true;
        }

        public override FieldValue[] GetField(Tiff tif, TiffTag tag)
        {
            JpegCodec sp = tif.m_currentCodec as JpegCodec;

            FieldValue[] result = null;

            switch (tag)
            {
                case TiffTag.JPEGTABLES:
                    result = new FieldValue[2];
                    result[0].Set(sp.m_jpegtables_length);
                    result[1].Set(sp.m_jpegtables);
                    break;

                case TiffTag.JPEGQUALITY:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_jpegquality);
                    break;

                case TiffTag.JPEGCOLORMODE:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_jpegcolormode);
                    break;

                case TiffTag.JPEGTABLESMODE:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_jpegtablesmode);
                    break;

                case TiffTag.YCBCRSUBSAMPLING:
                    JPEGFixupTestSubsampling(tif);
                    return base.GetField(tif, tag);

                case TiffTag.FAXRECVPARAMS:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_recvparams);
                    break;

                case TiffTag.FAXSUBADDRESS:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_subaddress);
                    break;

                case TiffTag.FAXRECVTIME:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_recvtime);
                    break;

                case TiffTag.FAXDCS:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_faxdcs);
                    break;

                default:
                    return base.GetField(tif, tag);
            }

            return result;
        }

        private static void JPEGFixupTestSubsampling(Tiff tif)
        {
            if (Tiff.CHECK_JPEG_YCBCR_SUBSAMPLING)
            {
                JpegCodec sp = tif.m_currentCodec as JpegCodec;
                sp.InitializeJpeg(false, false);

                if (!sp.m_common.IsDecompressor || sp.m_ycbcrsampling_fetched ||
                    tif.m_dir.td_photometric != Photometric.YCBCR)
                {
                    return;
                }

                sp.m_ycbcrsampling_fetched = true;
                if (tif.IsTiled())
                {
                    if (!tif.fillTile(0))
                        return;
                }
                else
                {
                    if (!tif.fillStrip(0))
                        return;
                }

                tif.SetField(TiffTag.YCBCRSUBSAMPLING, sp.m_h_sampling, sp.m_v_sampling);
                tif.m_curstrip = -1;
            }
        }
    }

    /// <summary>
    /// JPEG library source data manager.
    /// </summary>
    class JpegStdSource : SourceMgr
    {
        private static readonly byte[] dummy_EOI = { 0xFF, (byte)JPEG_MARKER.EOI };
        protected JpegCodec m_sp;

        public JpegStdSource(JpegCodec sp)
        {
            initInternalBuffer(null, 0);
            m_sp = sp;
        }

        public override void init_source()
        {
            Tiff tif = m_sp.GetTiff();
            initInternalBuffer(tif.m_rawdata, tif.m_rawcc);
        }

        public override bool fill_input_buffer()
        {
            initInternalBuffer(dummy_EOI, 2);
            return true;
        }
    }

    /// <summary>
    /// Alternate source manager for reading from JPEGTables.
    /// </summary>
    class JpegTablesSource : JpegStdSource
    {
        public JpegTablesSource(JpegCodec sp)
            : base(sp)
        {
        }

        public override void init_source()
        {
            initInternalBuffer(m_sp.m_jpegtables, m_sp.m_jpegtables_length);
        }
    }
}
