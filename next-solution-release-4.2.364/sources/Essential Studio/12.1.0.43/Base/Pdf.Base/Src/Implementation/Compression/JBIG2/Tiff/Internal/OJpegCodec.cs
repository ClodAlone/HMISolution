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

using Syncfusion.Pdf.Compression.JBIG2;

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    class OJpegCodec : TiffCodec
    {
        internal const int FIELD_OJPEG_JPEGINTERCHANGEFORMAT = (FieldBit.Codec + 0);
        internal const int FIELD_OJPEG_JPEGINTERCHANGEFORMATLENGTH = (FieldBit.Codec + 1);
        internal const int FIELD_OJPEG_JPEGQTABLES = (FieldBit.Codec + 2);
        internal const int FIELD_OJPEG_JPEGDCTABLES = (FieldBit.Codec + 3);
        internal const int FIELD_OJPEG_JPEGACTABLES = (FieldBit.Codec + 4);
        internal const int FIELD_OJPEG_JPEGPROC = (FieldBit.Codec + 5);
        internal const int FIELD_OJPEG_JPEGRESTARTINTERVAL = (FieldBit.Codec + 6);
        internal const int FIELD_OJPEG_COUNT = 7;

        private const int OJPEG_BUFFER = 2048;

        private enum OJPEGStateInBufferSource
        {
            osibsNotSetYet,
            osibsJpegInterchangeFormat,
            osibsStrile,
            osibsEof
        }

        private enum OJPEGStateOutState
        {
            ososSoi,

            ososQTable0,
            ososQTable1,
            ososQTable2,
            ososQTable3,

            ososDcTable0,
            ososDcTable1,
            ososDcTable2,
            ososDcTable3,

            ososAcTable0,
            ososAcTable1,
            ososAcTable2,
            ososAcTable3,

            ososDri,
            ososSof,
            ososSos,
            ososCompressed,
            ososRst,
            ososEoi
        }

        private static readonly TiffFieldInfo[] ojpeg_field_info =
        {
            new TiffFieldInfo(TiffTag.JPEGIFOFFSET, 1, 1, TiffType.LONG, FIELD_OJPEG_JPEGINTERCHANGEFORMAT, true, false, "JpegInterchangeFormat"),
            new TiffFieldInfo(TiffTag.JPEGIFBYTECOUNT, 1, 1, TiffType.LONG, FIELD_OJPEG_JPEGINTERCHANGEFORMATLENGTH, true, false, "JpegInterchangeFormatLength"),
            new TiffFieldInfo(TiffTag.JPEGQTABLES, -1, -1, TiffType.LONG, FIELD_OJPEG_JPEGQTABLES, false, true, "JpegQTables"),
            new TiffFieldInfo(TiffTag.JPEGDCTABLES, -1, -1, TiffType.LONG, FIELD_OJPEG_JPEGDCTABLES, false, true, "JpegDcTables"),
            new TiffFieldInfo(TiffTag.JPEGACTABLES, -1, -1, TiffType.LONG, FIELD_OJPEG_JPEGACTABLES, false, true, "JpegAcTables"),
            new TiffFieldInfo(TiffTag.JPEGPROC, 1, 1, TiffType.SHORT, FIELD_OJPEG_JPEGPROC, false, false, "JpegProc"),
            new TiffFieldInfo(TiffTag.JPEGRESTARTINTERVAL, 1, 1, TiffType.SHORT, FIELD_OJPEG_JPEGRESTARTINTERVAL, false, false, "JpegRestartInterval"),
        };

        private struct SosEnd
        {
            public bool m_log;
            public OJPEGStateInBufferSource m_in_buffer_source;
            public uint m_in_buffer_next_strile;
            public uint m_in_buffer_file_pos;
            public uint m_in_buffer_file_togo;
        }

        internal uint m_jpeg_interchange_format;
        internal uint m_jpeg_interchange_format_length;
        internal byte m_jpeg_proc;

        internal bool m_subsamplingcorrect_done;
        internal bool m_subsampling_tag;
        internal byte m_subsampling_hor;
        internal byte m_subsampling_ver;

        internal byte m_qtable_offset_count;
        internal byte m_dctable_offset_count;
        internal byte m_actable_offset_count;
        internal uint[] m_qtable_offset = new uint[3];
        internal uint[] m_dctable_offset = new uint[3];
        internal uint[] m_actable_offset = new uint[3];

        internal ushort m_restart_interval;

        internal DecompressStruct m_libjpeg_jpeg_decompress_struct;

        private TiffTagMethods m_tagMethods;
        private TiffTagMethods m_parentTagMethods;

        private uint m_file_size;
        private uint m_image_width;
        private uint m_image_length;
        private uint m_strile_width;
        private uint m_strile_length;
        private uint m_strile_length_total;
        private byte m_samples_per_pixel;
        private byte m_plane_sample_offset;
        private byte m_samples_per_pixel_per_plane;
        private bool m_subsamplingcorrect;
        private bool m_subsampling_force_desubsampling_inside_decompression;
        private byte[][] m_qtable = new byte[4][];
        private byte[][] m_dctable = new byte[4][];
        private byte[][] m_actable = new byte[4][];
        private byte m_restart_index;
        private bool m_sof_log;
        private byte m_sof_marker_id;
        private uint m_sof_x;
        private uint m_sof_y;
        private byte[] m_sof_c = new byte[3];
        private byte[] m_sof_hv = new byte[3];
        private byte[] m_sof_tq = new byte[3];
        private byte[] m_sos_cs = new byte[3];
        private byte[] m_sos_tda = new byte[3];
        private SosEnd[] m_sos_end = new SosEnd[3];
        private bool m_readheader_done;
        private bool m_writeheader_done;
        private short m_write_cursample;
        private uint m_write_curstrile;
        private bool m_libjpeg_session_active;
        private byte m_libjpeg_jpeg_query_style;
        //private ErrorMgr m_libjpeg_jpeg_error_mgr;
        private SourceMgr m_libjpeg_jpeg_source_mgr;
        private bool m_subsampling_convert_log;
        private uint m_subsampling_convert_ylinelen;
        private uint m_subsampling_convert_ylines;
        private uint m_subsampling_convert_clinelen;
        private uint m_subsampling_convert_clines;
        private byte[][] m_subsampling_convert_ybuf;
        private byte[][] m_subsampling_convert_cbbuf;
        private byte[][] m_subsampling_convert_crbuf;
        private byte[][][] m_subsampling_convert_ycbcrimage;
        private uint m_subsampling_convert_clinelenout;
        private uint m_subsampling_convert_state;
        private uint m_bytes_per_line; 
        private uint m_lines_per_strile;
        private OJPEGStateInBufferSource m_in_buffer_source;
        private uint m_in_buffer_next_strile;
        private uint m_in_buffer_strile_count;
        private uint m_in_buffer_file_pos;
        private bool m_in_buffer_file_pos_log;
        private uint m_in_buffer_file_togo;
        private ushort m_in_buffer_togo;
        private int m_in_buffer_cur; // index into m_in_buffer
        private byte[] m_in_buffer = new byte[OJPEG_BUFFER];
        private OJPEGStateOutState m_out_state;
        private byte[] m_out_buffer = new byte[OJPEG_BUFFER];
        private byte[] m_skip_buffer;
        private bool m_forceProcessedRgbOutput;

        public OJpegCodec(Tiff tif, Compression scheme, string name)
            : base(tif, scheme, name)
        {
            m_tagMethods = new OJpegCodecTagMethods();
        }

        private void cleanState()
        {
            m_jpeg_interchange_format = 0;
            m_jpeg_interchange_format_length = 0;
            m_jpeg_proc = 0;

            m_subsamplingcorrect_done = false;
            m_subsampling_tag = false;
            m_subsampling_hor = 0;
            m_subsampling_ver = 0;

            m_qtable_offset_count = 0;
            m_dctable_offset_count = 0;
            m_actable_offset_count = 0;
            m_qtable_offset = new uint[3];
            m_dctable_offset = new uint[3];
            m_actable_offset = new uint[3];

            m_restart_interval = 0;

            m_libjpeg_jpeg_decompress_struct = null;

            m_file_size = 0;
            m_image_width = 0;
            m_image_length = 0;
            m_strile_width = 0;
            m_strile_length = 0;
            m_strile_length_total = 0;
            m_samples_per_pixel = 0;
            m_plane_sample_offset = 0;
            m_samples_per_pixel_per_plane = 0;
            m_subsamplingcorrect = false;
            m_subsampling_force_desubsampling_inside_decompression = false;
            m_qtable = new byte[4][];
            m_dctable = new byte[4][];
            m_actable = new byte[4][];
            m_restart_index = 0;
            m_sof_log = false;
            m_sof_marker_id = 0;
            m_sof_x = 0;
            m_sof_y = 0;
            m_sof_c = new byte[3];
            m_sof_hv = new byte[3];
            m_sof_tq = new byte[3];
            m_sos_cs = new byte[3];
            m_sos_tda = new byte[3];
            m_sos_end = new SosEnd[3];
            m_readheader_done = false;
            m_writeheader_done = false;
            m_write_cursample = 0;
            m_write_curstrile = 0;
            m_libjpeg_session_active = false;
            m_libjpeg_jpeg_query_style = 0;
            //m_libjpeg_jpeg_error_mgr = null;
            m_libjpeg_jpeg_source_mgr = null;
            m_subsampling_convert_log = false;
            m_subsampling_convert_ylinelen = 0;
            m_subsampling_convert_ylines = 0;
            m_subsampling_convert_clinelen = 0;
            m_subsampling_convert_clines = 0;
            m_subsampling_convert_ybuf = null;
            m_subsampling_convert_cbbuf = null;
            m_subsampling_convert_crbuf = null;
            m_subsampling_convert_ycbcrimage = null;
            m_subsampling_convert_clinelenout = 0;
            m_subsampling_convert_state = 0;
            m_bytes_per_line = 0;
            m_lines_per_strile = 0;
            m_in_buffer_source = OJPEGStateInBufferSource.osibsNotSetYet;
            m_in_buffer_next_strile = 0;
            m_in_buffer_strile_count = 0;
            m_in_buffer_file_pos = 0;
            m_in_buffer_file_pos_log = false;
            m_in_buffer_file_togo = 0;
            m_in_buffer_togo = 0;
            m_in_buffer_cur = 0; // index into m_in_buffer
            m_in_buffer = new byte[OJPEG_BUFFER];
            m_out_state = 0;
            m_out_buffer = new byte[OJPEG_BUFFER];
            m_skip_buffer = null;
            m_forceProcessedRgbOutput = false;
        }

        public override bool Init()
        {
            m_tif.MergeFieldInfo(ojpeg_field_info, ojpeg_field_info.Length);

            cleanState();
            m_jpeg_proc = 1;
            m_subsampling_hor = 2;
            m_subsampling_ver = 2;

            m_tif.SetField(TiffTag.YCBCRSUBSAMPLING, 2, 2);

            m_parentTagMethods = m_tif.m_tagmethods;
            m_tif.m_tagmethods = m_tagMethods;

            m_tif.m_flags |= TiffFlags.NOREADRAW;
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

        public Tiff GetTiff()
        {
            return m_tif;
        }

        /// <summary>
        /// Setups the decoder part of the codec.
        /// </summary>
        public override bool SetupDecode()
        {
            return OJPEGSetupDecode();
        }

        /// <summary>
        /// Prepares the decoder part of the codec for a decoding.
        /// </summary>
        public override bool PreDecode(short plane)
        {
            return OJPEGPreDecode(plane);
        }

        /// <summary>
        /// Decodes one row of image data.
        /// </summary>
        public override bool DecodeRow(byte[] buffer, int offset, int count, short plane)
        {
            return OJPEGDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one strip of image data.
        /// </summary>
        public override bool DecodeStrip(byte[] buffer, int offset, int count, short plane)
        {
            return OJPEGDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Decodes one tile of image data.
        /// </summary>
        public override bool DecodeTile(byte[] buffer, int offset, int count, short plane)
        {
            return OJPEGDecode(buffer, offset, count, plane);
        }

        /// <summary>
        /// Cleanups the state of the codec.
        /// </summary>
        public override void Cleanup()
        {
            OJPEGCleanup();
        }

        private bool OJPEGSetupDecode()
        {
            return true;
        }

        private bool OJPEGPreDecode(short s)
        {
            uint m;
            if (!m_subsamplingcorrect_done)
                OJPEGSubsamplingCorrect();

            if (!m_readheader_done)
            {
                if (!OJPEGReadHeaderInfo())
                    return false;
            }

            if (!m_sos_end[s].m_log)
            {
                if (!OJPEGReadSecondarySos(s))
                    return false;
            }

            if (m_tif.IsTiled())
                m = (uint)m_tif.m_curtile;
            else
                m = (uint)m_tif.m_curstrip;

            if (m_writeheader_done && ((m_write_cursample != s) || (m_write_curstrile > m)))
            {
                if (m_libjpeg_session_active)
                    OJPEGLibjpegSessionAbort();
                m_writeheader_done = false;
            }

            if (!m_writeheader_done)
            {
                m_plane_sample_offset = (byte)s;
                m_write_cursample = s;
                m_write_curstrile = (uint)(s * m_tif.m_dir.td_stripsperimage);
                if (!m_in_buffer_file_pos_log ||
                    (m_in_buffer_file_pos - m_in_buffer_togo != m_sos_end[s].m_in_buffer_file_pos))
                {
                    m_in_buffer_source = m_sos_end[s].m_in_buffer_source;
                    m_in_buffer_next_strile = m_sos_end[s].m_in_buffer_next_strile;
                    m_in_buffer_file_pos = m_sos_end[s].m_in_buffer_file_pos;
                    m_in_buffer_file_pos_log = false;
                    m_in_buffer_file_togo = m_sos_end[s].m_in_buffer_file_togo;
                    m_in_buffer_togo = 0;
                    m_in_buffer_cur = 0;
                }
                if (!OJPEGWriteHeaderInfo())
                    return false;
            }

            while (m_write_curstrile < m)
            {
                if (m_libjpeg_jpeg_query_style == 0)
                {
                    if (!OJPEGPreDecodeSkipRaw())
                        return false;
                }
                else
                {
                    if (!OJPEGPreDecodeSkipScanlines())
                        return false;
                }
                m_write_curstrile++;
            }

            return true;
        }

        private bool OJPEGDecode(byte[] buf, int offset, int cc, short s)
        {
            if (m_libjpeg_jpeg_query_style == 0)
            {
                if (!OJPEGDecodeRaw(buf, offset, cc))
                    return false;
            }
            else
            {
                if (!OJPEGDecodeScanlines(buf, offset, cc))
                    return false;
            }
            return true;
        }

        private void OJPEGCleanup()
        {
            m_tif.m_tagmethods = m_parentTagMethods;
            if (m_libjpeg_session_active)
                OJPEGLibjpegSessionAbort();
        }

        private bool OJPEGPreDecodeSkipRaw()
        {
            uint m;
            m = m_lines_per_strile;
            if (m_subsampling_convert_state != 0)
            {
                if (m_subsampling_convert_clines - m_subsampling_convert_state >= m)
                {
                    m_subsampling_convert_state += m;
                    if (m_subsampling_convert_state == m_subsampling_convert_clines)
                        m_subsampling_convert_state = 0;
                    return true;
                }
                m -= m_subsampling_convert_clines - m_subsampling_convert_state;
                m_subsampling_convert_state = 0;
            }
            while (m >= m_subsampling_convert_clines)
            {
                if (jpeg_read_raw_data_encap(m_subsampling_ver * 8) == 0)
                    return false;
                m -= m_subsampling_convert_clines;
            }
            if (m > 0)
            {
                if (jpeg_read_raw_data_encap(m_subsampling_ver * 8) == 0)
                    return false;
                m_subsampling_convert_state = m;
            }
            return true;
        }

        private bool OJPEGPreDecodeSkipScanlines()
        {
            uint m;
            if (m_skip_buffer == null)
                m_skip_buffer = new byte[m_bytes_per_line];

            for (m = 0; m < m_lines_per_strile; m++)
            {
                if (jpeg_read_scanlines_encap(m_skip_buffer, 1) == 0)
                    return false;
            }
            return true;
        }

        private bool OJPEGDecodeRaw(byte[] buf, int offset, int cc)
        {
            const string module = "OJPEGDecodeRaw";

            if (cc % m_bytes_per_line != 0)
            {
                return false;
            }

            int m = offset;
            int n = cc;
            do
            {
                if (m_subsampling_convert_state == 0)
                {
                    if (jpeg_read_raw_data_encap(m_subsampling_ver * 8) == 0)
                        return false;
                }

                uint oy = m_subsampling_convert_state * m_subsampling_ver * m_subsampling_convert_ylinelen;
                uint ocb = m_subsampling_convert_state * m_subsampling_convert_clinelen;
                uint ocr = m_subsampling_convert_state * m_subsampling_convert_clinelen;

                int i = 0;
                int ii = 0;
                int p = m;
                for (uint q = 0; q < m_subsampling_convert_clinelenout; q++)
                {
                    uint r = oy;
                    for (byte sy = 0; sy < m_subsampling_ver; sy++)
                    {
                        for (byte sx = 0; sx < m_subsampling_hor; sx++)
                        {
                            i = (int)(r / m_subsampling_convert_ylinelen);
                            ii = (int)(r % m_subsampling_convert_ylinelen);
                            r++;
                            buf[p++] = m_subsampling_convert_ybuf[i][ii];
                        }

                        r += m_subsampling_convert_ylinelen - m_subsampling_hor;
                    }
                    oy += m_subsampling_hor;

                    i = (int)(ocb / m_subsampling_convert_clinelen);
                    ii = (int)(ocb % m_subsampling_convert_clinelen);
                    ocb++;
                    buf[p++] = m_subsampling_convert_cbbuf[i][ii];

                    i = (int)(ocr / m_subsampling_convert_clinelen);
                    ii = (int)(ocr % m_subsampling_convert_clinelen);
                    ocr++;
                    buf[p++] = m_subsampling_convert_crbuf[i][ii];
                }
                m_subsampling_convert_state++;
                if (m_subsampling_convert_state == m_subsampling_convert_clines)
                    m_subsampling_convert_state = 0;
                m += (int)m_bytes_per_line;
                n -= (int)m_bytes_per_line;
            } while (n > 0);
            return true;
        }

        private bool OJPEGDecodeScanlines(byte[] buf, int offset, int cc)
        {
            const string module = "OJPEGDecodeScanlines";

            if (cc % m_bytes_per_line != 0)
            {
                return false;
            }

            int m = offset;
            byte[] temp = new byte[m_bytes_per_line];
            int n = cc;
            do
            {
                if (jpeg_read_scanlines_encap(temp, 1) == 0)
                    return false;

                Buffer.BlockCopy(temp, 0, buf, m, temp.Length);
                m += (int)m_bytes_per_line;
                n -= (int)m_bytes_per_line;
            } while (n > 0);

            return true;
        }

        public void OJPEGSubsamplingCorrect()
        {
            const string module = "OJPEGSubsamplingCorrect";
            byte mh;
            byte mv;

            if ((m_tif.m_dir.td_samplesperpixel != 3) || ((m_tif.m_dir.td_photometric != Photometric.YCBCR) &&
                (m_tif.m_dir.td_photometric != Photometric.ITULAB)))
            {
                if (m_subsampling_tag)
                {
                }

                m_subsampling_hor = 1;
                m_subsampling_ver = 1;
                m_subsampling_force_desubsampling_inside_decompression = false;
            }
            else
            {
                m_subsamplingcorrect_done = true;
                mh = m_subsampling_hor;
                mv = m_subsampling_ver;
                m_subsamplingcorrect = true;
                OJPEGReadHeaderInfoSec();
                if (m_subsampling_force_desubsampling_inside_decompression)
                {
                    m_subsampling_hor = 1;
                    m_subsampling_ver = 1;
                }
                m_subsamplingcorrect = false;
            }

            m_subsamplingcorrect_done = true;
        }

        private bool OJPEGReadHeaderInfo()
        {
            const string module = "OJPEGReadHeaderInfo";
            m_image_width = (uint)m_tif.m_dir.td_imagewidth;
            m_image_length = (uint)m_tif.m_dir.td_imagelength;
            if (m_tif.IsTiled())
            {
                m_strile_width = (uint)m_tif.m_dir.td_tilewidth;
                m_strile_length = (uint)m_tif.m_dir.td_tilelength;
                m_strile_length_total = ((m_image_length + m_strile_length - 1) / m_strile_length) * m_strile_length;
            }
            else
            {
                m_strile_width = m_image_width;
                m_strile_length = (uint)m_tif.m_dir.td_rowsperstrip;
                m_strile_length_total = m_image_length;
            }
            m_samples_per_pixel = (byte)m_tif.m_dir.td_samplesperpixel;
            if (m_samples_per_pixel == 1)
            {
                m_plane_sample_offset = 0;
                m_samples_per_pixel_per_plane = m_samples_per_pixel;
                m_subsampling_hor = 1;
                m_subsampling_ver = 1;
            }
            else
            {
                if (m_samples_per_pixel != 3)
                {
                    return false;
                }

                m_plane_sample_offset = 0;
                if (m_tif.m_dir.td_planarconfig == PlanarConfig.CONTIG)
                    m_samples_per_pixel_per_plane = 3;
                else
                    m_samples_per_pixel_per_plane = 1;
            }
            if (m_strile_length < m_image_length)
            {
                if (m_strile_length % (m_subsampling_ver * 8) != 0)
                {
                    return false;
                }
                m_restart_interval = (ushort)(((m_strile_width + m_subsampling_hor * 8 - 1) / (m_subsampling_hor * 8)) * (m_strile_length / (m_subsampling_ver * 8)));
            }

            if (!OJPEGReadHeaderInfoSec())
                return false;

            m_sos_end[0].m_log = true;
            m_sos_end[0].m_in_buffer_source = m_in_buffer_source;
            m_sos_end[0].m_in_buffer_next_strile = m_in_buffer_next_strile;
            m_sos_end[0].m_in_buffer_file_pos = m_in_buffer_file_pos - m_in_buffer_togo;
            m_sos_end[0].m_in_buffer_file_togo = m_in_buffer_file_togo + m_in_buffer_togo;
            m_readheader_done = true;
            return true;
        }

        private bool OJPEGReadSecondarySos(short s)
        {
            m_plane_sample_offset = (byte)(s - 1);
            while (!m_sos_end[m_plane_sample_offset].m_log)
                m_plane_sample_offset--;

            m_in_buffer_source = m_sos_end[m_plane_sample_offset].m_in_buffer_source;
            m_in_buffer_next_strile = m_sos_end[m_plane_sample_offset].m_in_buffer_next_strile;
            m_in_buffer_file_pos = m_sos_end[m_plane_sample_offset].m_in_buffer_file_pos;
            m_in_buffer_file_pos_log = false;
            m_in_buffer_file_togo = m_sos_end[m_plane_sample_offset].m_in_buffer_file_togo;
            m_in_buffer_togo = 0;
            m_in_buffer_cur = 0;

            while (m_plane_sample_offset < s)
            {
                do
                {
                    byte m;
                    if (!OJPEGReadByte(out m))
                        return false;

                    if (m == 255)
                    {
                        do
                        {
                            if (!OJPEGReadByte(out m))
                                return false;

                            if (m != 255)
                                break;
                        } while (true);

                        if (m == (byte)JPEG_MARKER.SOS)
                            break;
                    }
                } while (true);

                m_plane_sample_offset++;
                if (!OJPEGReadHeaderInfoSecStreamSos())
                    return false;

                m_sos_end[m_plane_sample_offset].m_log = true;
                m_sos_end[m_plane_sample_offset].m_in_buffer_source = m_in_buffer_source;
                m_sos_end[m_plane_sample_offset].m_in_buffer_next_strile = m_in_buffer_next_strile;
                m_sos_end[m_plane_sample_offset].m_in_buffer_file_pos = m_in_buffer_file_pos - m_in_buffer_togo;
                m_sos_end[m_plane_sample_offset].m_in_buffer_file_togo = m_in_buffer_file_togo + m_in_buffer_togo;
            }

            return true;
        }

        private bool OJPEGWriteHeaderInfo()
        {
            m_out_state = OJPEGStateOutState.ososSoi;
            m_restart_index = 0;

            //m_libjpeg_jpeg_error_mgr = new OJpegErrorManager(this);
            if (!jpeg_create_decompress_encap())
                return false;

            m_libjpeg_session_active = true;
            m_libjpeg_jpeg_source_mgr = new OJpegSrcManager(this);
            m_libjpeg_jpeg_decompress_struct.Src = m_libjpeg_jpeg_source_mgr;

            if (jpeg_read_header_encap(true) == ReadResult.JPEG_SUSPENDED)
                return false;

            if (!m_subsampling_force_desubsampling_inside_decompression && (m_samples_per_pixel_per_plane > 1))
            {
                m_libjpeg_jpeg_decompress_struct.Raw_data_out = true;
                m_libjpeg_jpeg_decompress_struct.Do_fancy_upsampling = false;

                m_libjpeg_jpeg_query_style = 0;
                if (!m_subsampling_convert_log)
                {
                    m_subsampling_convert_ylinelen = (uint)((m_strile_width + m_subsampling_hor * 8 - 1) / (m_subsampling_hor * 8) * m_subsampling_hor * 8);
                    m_subsampling_convert_ylines = (uint)(m_subsampling_ver * 8);
                    m_subsampling_convert_clinelen = m_subsampling_convert_ylinelen / m_subsampling_hor;
                    m_subsampling_convert_clines = 8;

                    m_subsampling_convert_ybuf = new byte[m_subsampling_convert_ylines][];
                    for (int i = 0; i < m_subsampling_convert_ylines; i++)
                        m_subsampling_convert_ybuf[i] = new byte[m_subsampling_convert_ylinelen];

                    m_subsampling_convert_cbbuf = new byte[m_subsampling_convert_clines][];
                    m_subsampling_convert_crbuf = new byte[m_subsampling_convert_clines][];
                    for (int i = 0; i < m_subsampling_convert_clines; i++)
                    {
                        m_subsampling_convert_cbbuf[i] = new byte[m_subsampling_convert_clinelen];
                        m_subsampling_convert_crbuf[i] = new byte[m_subsampling_convert_clinelen];
                    }

                    m_subsampling_convert_ycbcrimage = new byte[3][][];
                    m_subsampling_convert_ycbcrimage[0] = new byte[m_subsampling_convert_ylines][];
                    for (uint n = 0; n < m_subsampling_convert_ylines; n++)
                        m_subsampling_convert_ycbcrimage[0][n] = m_subsampling_convert_ybuf[n];

                    m_subsampling_convert_ycbcrimage[1] = new byte[m_subsampling_convert_clines][];
                    for (uint n = 0; n < m_subsampling_convert_clines; n++)
                        m_subsampling_convert_ycbcrimage[1][n] = m_subsampling_convert_cbbuf[n];

                    m_subsampling_convert_ycbcrimage[2] = new byte[m_subsampling_convert_clines][];
                    for (uint n = 0; n < m_subsampling_convert_clines; n++)
                        m_subsampling_convert_ycbcrimage[2][n] = m_subsampling_convert_crbuf[n];

                    m_subsampling_convert_clinelenout = ((m_strile_width + m_subsampling_hor - 1) / m_subsampling_hor);
                    m_subsampling_convert_state = 0;
                    m_bytes_per_line = (uint)(m_subsampling_convert_clinelenout * (m_subsampling_ver * m_subsampling_hor + 2));
                    m_lines_per_strile = ((m_strile_length + m_subsampling_ver - 1) / m_subsampling_ver);
                    m_subsampling_convert_log = true;
                }
            }
            else
            {
                if (m_forceProcessedRgbOutput)
                {
                    m_libjpeg_jpeg_decompress_struct.Do_fancy_upsampling = false;
                    m_libjpeg_jpeg_decompress_struct.Jpeg_color_space = J_COLOR_SPACE.JCS_YCbCr;
                    m_libjpeg_jpeg_decompress_struct.Out_color_space = J_COLOR_SPACE.JCS_RGB;
                }
                else
                {
                    m_libjpeg_jpeg_decompress_struct.Jpeg_color_space = J_COLOR_SPACE.JCS_UNKNOWN;
                    m_libjpeg_jpeg_decompress_struct.Out_color_space = J_COLOR_SPACE.JCS_UNKNOWN;
                }

                m_libjpeg_jpeg_query_style = 1;
                m_bytes_per_line = m_samples_per_pixel_per_plane * m_strile_width;
                m_lines_per_strile = m_strile_length;
            }

            if (!jpeg_start_decompress_encap())
                return false;

            m_writeheader_done = true;
            return true;
        }

        private void OJPEGLibjpegSessionAbort()
        {
            m_libjpeg_jpeg_decompress_struct.jpeg_destroy();
            m_libjpeg_session_active = false;
        }

        private bool OJPEGReadHeaderInfoSec()
        {
            const string module = "OJPEGReadHeaderInfoSec";
            byte m;
            ushort n;
            byte o;
            if (m_file_size == 0)
                m_file_size = (uint)m_tif.GetStream().Size(m_tif.m_clientdata);

            if (m_jpeg_interchange_format != 0)
            {
                if (m_jpeg_interchange_format >= m_file_size)
                {
                    m_jpeg_interchange_format = 0;
                    m_jpeg_interchange_format_length = 0;
                }
                else
                {
                    if ((m_jpeg_interchange_format_length == 0) || (m_jpeg_interchange_format + m_jpeg_interchange_format_length > m_file_size))
                        m_jpeg_interchange_format_length = m_file_size - m_jpeg_interchange_format;
                }
            }

            m_in_buffer_source = OJPEGStateInBufferSource.osibsNotSetYet;
            m_in_buffer_next_strile = 0;
            m_in_buffer_strile_count = (uint)m_tif.m_dir.td_nstrips;
            m_in_buffer_file_togo = 0;
            m_in_buffer_togo = 0;

            do
            {
                if (!OJPEGReadBytePeek(out m))
                    return false;

                if (m != 255)
                    break;

                OJPEGReadByteAdvance();
                do
                {
                    if (!OJPEGReadByte(out m))
                        return false;
                } while (m == 255);

                switch ((JPEG_MARKER)m)
                {
                    case JPEG_MARKER.SOI:
                        /* this type of marker has no data, and should be skipped */
                        break;
                    case JPEG_MARKER.COM:
                    case JPEG_MARKER.APP0:
                    case JPEG_MARKER.APP1:
                    case JPEG_MARKER.APP2:
                    case JPEG_MARKER.APP3:
                    case JPEG_MARKER.APP4:
                    case JPEG_MARKER.APP5:
                    case JPEG_MARKER.APP6:
                    case JPEG_MARKER.APP7:
                    case JPEG_MARKER.APP8:
                    case JPEG_MARKER.APP9:
                    case JPEG_MARKER.APP10:
                    case JPEG_MARKER.APP11:
                    case JPEG_MARKER.APP12:
                    case JPEG_MARKER.APP13:
                    case JPEG_MARKER.APP14:
                    case JPEG_MARKER.APP15:
                        if (!OJPEGReadWord(out n))
                            return false;
                        if (n < 2)
                        {
                            if (!m_subsamplingcorrect)
                            { }
                            return false;
                        }
                        if (n > 2)
                            OJPEGReadSkip((ushort)(n - 2));
                        break;
                    case JPEG_MARKER.DRI:
                        if (!OJPEGReadHeaderInfoSecStreamDri())
                            return false;
                        break;
                    case JPEG_MARKER.DQT:
                        if (!OJPEGReadHeaderInfoSecStreamDqt())
                            return false;
                        break;
                    case JPEG_MARKER.DHT:
                        if (!OJPEGReadHeaderInfoSecStreamDht())
                            return false;
                        break;
                    case JPEG_MARKER.SOF0:
                    case JPEG_MARKER.SOF1:
                    case JPEG_MARKER.SOF3:
                        if (!OJPEGReadHeaderInfoSecStreamSof(m))
                            return false;
                        if (m_subsamplingcorrect)
                            return true;
                        break;
                    case JPEG_MARKER.SOS:
                        if (m_subsamplingcorrect)
                            return true;
                        if (!OJPEGReadHeaderInfoSecStreamSos())
                            return false;
                        break;
                    default:
                        return false;
                }
            } while (m != (byte)JPEG_MARKER.SOS);

            if (m_subsamplingcorrect)
                return true;

            if (!m_sof_log)
            {
                if (!OJPEGReadHeaderInfoSecTablesQTable())
                    return false;

                m_sof_marker_id = (byte)JPEG_MARKER.SOF0;
                for (o = 0; o < m_samples_per_pixel; o++)
                    m_sof_c[o] = o;

                m_sof_hv[0] = (byte)((m_subsampling_hor << 4) | m_subsampling_ver);
                for (o = 1; o < m_samples_per_pixel; o++)
                    m_sof_hv[o] = 17;

                m_sof_x = m_strile_width;
                m_sof_y = m_strile_length_total;
                m_sof_log = true;

                if (!OJPEGReadHeaderInfoSecTablesDcTable())
                    return false;

                if (!OJPEGReadHeaderInfoSecTablesAcTable())
                    return false;

                for (o = 1; o < m_samples_per_pixel; o++)
                    m_sos_cs[o] = o;
            }

            return true;
        }

        private bool OJPEGReadHeaderInfoSecStreamDri()
        {
            const string module = "OJPEGReadHeaderInfoSecStreamDri";
            ushort m;
            if (!OJPEGReadWord(out m))
                return false;

            if (m != 4)
            {
                return false;
            }

            if (!OJPEGReadWord(out m))
                return false;

            m_restart_interval = m;
            return true;
        }

        private bool OJPEGReadHeaderInfoSecStreamDqt()
        {
            const string module = "OJPEGReadHeaderInfoSecStreamDqt";
            ushort m;
            uint na;
            byte[] nb;
            byte o;
            if (!OJPEGReadWord(out m))
                return false;

            if (m <= 2)
            {
                if (!m_subsamplingcorrect)
                { }
                return false;
            }

            if (m_subsamplingcorrect)
            {
                OJPEGReadSkip((ushort)(m - 2));
            }
            else
            {
                m -= 2;
                do
                {
                    if (m < 65)
                    {
                        return false;
                    }

                    na = 69;
                    nb = new byte[na];
                    nb[0] = 255;
                    nb[1] = (byte)JPEG_MARKER.DQT;
                    nb[2] = 0;
                    nb[3] = 67;
                    if (!OJPEGReadBlock(65, nb, 4))
                        return false;

                    o = (byte)(nb[4] & 15);
                    if (3 < o)
                    {
                        return false;
                    }

                    m_qtable[o] = nb;
                    m -= 65;
                } while (m > 0);
            }
            return true;
        }

        private bool OJPEGReadHeaderInfoSecStreamDht()
        {
            const string module = "OJPEGReadHeaderInfoSecStreamDht";
            ushort m;
            uint na;
            byte[] nb;
            byte o;
            if (!OJPEGReadWord(out m))
                return false;
            if (m <= 2)
            {
                if (!m_subsamplingcorrect)
                { }
                return false;
            }
            if (m_subsamplingcorrect)
            {
                OJPEGReadSkip((ushort)(m - 2));
            }
            else
            {
                na = (uint)(2 + m);
                nb = new byte[na];
                nb[0] = 255;
                nb[1] = (byte)JPEG_MARKER.DHT;
                nb[2] = (byte)(m >> 8);
                nb[3] = (byte)(m & 255);
                if (!OJPEGReadBlock((ushort)(m - 2), nb, 4))
                    return false;
                o = nb[4];
                if ((o & 240) == 0)
                {
                    if (3 < o)
                    {
                        return false;
                    }
                    m_dctable[o] = nb;
                }
                else
                {
                    if ((o & 240) != 16)
                    {
                        return false;
                    }
                    o &= 15;
                    if (3 < o)
                    {
                        return false;
                    }
                    m_actable[o] = nb;
                }
            }
            return true;
        }

        private bool OJPEGReadHeaderInfoSecStreamSof(byte marker_id)
        {
            const string module = "OJPEGReadHeaderInfoSecStreamSof";
            ushort m;
            ushort n;
            byte o;
            ushort p;
            ushort q;
            if (m_sof_log)
            {
                return false;
            }
            if (!m_subsamplingcorrect)
                m_sof_marker_id = marker_id;
            /* Lf: data length */
            if (!OJPEGReadWord(out m))
                return false;
            if (m < 11)
            {
                if (!m_subsamplingcorrect)
                { }
                return false;
            }
            m -= 8;
            if (m % 3 != 0)
            {
                if (!m_subsamplingcorrect)
                { }
                return false;
            }
            n = (ushort)(m / 3);
            if (!m_subsamplingcorrect)
            {
                if (n != m_samples_per_pixel)
                {
                    return false;
                }
            }
            /* P: Sample precision */
            if (!OJPEGReadByte(out o))
                return false;
            if (o != 8)
            {
                if (!m_subsamplingcorrect)
                return false;
            }
            /* Y: Number of lines, X: Number of samples per line */
            if (m_subsamplingcorrect)
                OJPEGReadSkip(4);
            else
            {
                if (!OJPEGReadWord(out p))
                    return false;
                if ((p < m_image_length) && (p < m_strile_length_total))
                {
                    return false;
                }
                m_sof_y = p;
                /* X: Number of samples per line */
                if (!OJPEGReadWord(out p))
                    return false;
                if ((p < m_image_width) && (p < m_strile_width))
                {
                    return false;
                }
                m_sof_x = p;
            }
            /* Nf: Number of image components in frame */
            if (!OJPEGReadByte(out o))
                return false;
            if (o != n)
            {
                if (!m_subsamplingcorrect)
                { }
                return false;
            }
            for (q = 0; q < n; q++)
            {
                /* C: Component identifier */
                if (!OJPEGReadByte(out o))
                    return false;
                if (!m_subsamplingcorrect)
                    m_sof_c[q] = o;
                /* H: Horizontal sampling factor, and V: Vertical sampling factor */
                if (!OJPEGReadByte(out o))
                    return false;
                if (m_subsamplingcorrect)
                {
                    if (q == 0)
                    {
                        m_subsampling_hor = (byte)(o >> 4);
                        m_subsampling_ver = (byte)(o & 15);
                        if (((m_subsampling_hor != 1) && (m_subsampling_hor != 2) && (m_subsampling_hor != 4)) ||
                            ((m_subsampling_ver != 1) && (m_subsampling_ver != 2) && (m_subsampling_ver != 4)) ||
                            m_forceProcessedRgbOutput)
                        {
                            m_subsampling_force_desubsampling_inside_decompression = true;
                        }
                    }
                    else
                    {
                        if (o != 17)
                            m_subsampling_force_desubsampling_inside_decompression = true;
                    }
                }
                else
                {
                    m_sof_hv[q] = o;
                    if (!m_subsampling_force_desubsampling_inside_decompression)
                    {
                        if (q == 0)
                        {
                            if (o != ((m_subsampling_hor << 4) | m_subsampling_ver))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (o != 17)
                            {
                                return false;
                            }
                        }
                    }
                }
                /* Tq: Quantization table destination selector */
                if (!OJPEGReadByte(out o))
                    return false;
                if (!m_subsamplingcorrect)
                    m_sof_tq[q] = o;
            }
            if (!m_subsamplingcorrect)
                m_sof_log = true;
            return true;
        }

        private bool OJPEGReadHeaderInfoSecStreamSos()
        {
            const string module = "OJPEGReadHeaderInfoSecStreamSos";
            ushort m;
            byte n;
            byte o;
            if (!m_sof_log)
            {
                return false;
            }
            /* Ls */
            if (!OJPEGReadWord(out m))
                return false;
            if (m != 6 + m_samples_per_pixel_per_plane * 2)
            {
                return false;
            }
            /* Ns */
            if (!OJPEGReadByte(out n))
                return false;
            if (n != m_samples_per_pixel_per_plane)
            {
                return false;
            }
            /* Cs, Td, and Ta */
            for (o = 0; o < m_samples_per_pixel_per_plane; o++)
            {
                /* Cs */
                if (!OJPEGReadByte(out n))
                    return false;
                m_sos_cs[m_plane_sample_offset + o] = n;
                /* Td and Ta */
                if (!OJPEGReadByte(out n))
                    return false;
                m_sos_tda[m_plane_sample_offset + o] = n;
            }
            OJPEGReadSkip(3);
            return true;
        }

        private bool OJPEGReadHeaderInfoSecTablesQTable()
        {
            const string module = "OJPEGReadHeaderInfoSecTablesQTable";
            byte m;
            byte n;
            uint oa;
            byte[] ob;
            uint p;
            if (m_qtable_offset[0] == 0)
            {
                return false;
            }
            m_in_buffer_file_pos_log = false;
            for (m = 0; m < m_samples_per_pixel; m++)
            {
                if ((m_qtable_offset[m] != 0) && ((m == 0) || (m_qtable_offset[m] != m_qtable_offset[m - 1])))
                {
                    for (n = 0; n < m - 1; n++)
                    {
                        if (m_qtable_offset[m] == m_qtable_offset[n])
                        {
                            return false;
                        }
                    }
                    oa = 69;
                    ob = new byte[oa];
                    ob[0] = 255;
                    ob[1] = (byte)JPEG_MARKER.DQT;
                    ob[2] = 0;
                    ob[3] = 67;
                    ob[4] = m;
                    TiffStream stream = m_tif.GetStream();
                    stream.Seek(m_tif.m_clientdata, m_qtable_offset[m], SeekOrigin.Begin);
                    p = (uint)stream.Read(m_tif.m_clientdata, ob, 5, 64);
                    if (p != 64)
                        return false;
                    m_qtable[m] = ob;
                    m_sof_tq[m] = m;
                }
                else
                    m_sof_tq[m] = m_sof_tq[m - 1];
            }
            return true;
        }

        private bool OJPEGReadHeaderInfoSecTablesDcTable()
        {
            const string module = "OJPEGReadHeaderInfoSecTablesDcTable";
            byte m;
            byte n;
            byte[] o = new byte[16];
            uint p;
            uint q;
            uint ra;
            byte[] rb;
            if (m_dctable_offset[0] == 0)
            {
                return false;
            }
            m_in_buffer_file_pos_log = false;
            for (m = 0; m < m_samples_per_pixel; m++)
            {
                if ((m_dctable_offset[m] != 0) && ((m == 0) || (m_dctable_offset[m] != m_dctable_offset[m - 1])))
                {
                    for (n = 0; n < m - 1; n++)
                    {
                        if (m_dctable_offset[m] == m_dctable_offset[n])
                        {
                            return false;
                        }
                    }

                    TiffStream stream = m_tif.GetStream();
                    stream.Seek(m_tif.m_clientdata, m_dctable_offset[m], SeekOrigin.Begin);
                    p = (uint)stream.Read(m_tif.m_clientdata, o, 0, 16);
                    if (p != 16)
                        return false;
                    q = 0;
                    for (n = 0; n < 16; n++)
                        q += o[n];
                    ra = 21 + q;
                    rb = new byte[ra];
                    rb[0] = 255;
                    rb[1] = (byte)JPEG_MARKER.DHT;
                    rb[2] = (byte)((19 + q) >> 8);
                    rb[3] = (byte)((19 + q) & 255);
                    rb[4] = m;
                    for (n = 0; n < 16; n++)
                        rb[5 + n] = o[n];

                    p = (uint)stream.Read(m_tif.m_clientdata, rb, 21, (int)q);
                    if (p != q)
                        return false;
                    m_dctable[m] = rb;
                    m_sos_tda[m] = (byte)(m << 4);
                }
                else
                    m_sos_tda[m] = m_sos_tda[m - 1];
            }
            return true;
        }

        private bool OJPEGReadHeaderInfoSecTablesAcTable()
        {
            const string module = "OJPEGReadHeaderInfoSecTablesAcTable";
            byte m;
            byte n;
            byte[] o = new byte[16];
            uint p;
            uint q;
            uint ra;
            byte[] rb;
            if (m_actable_offset[0] == 0)
            {
                return false;
            }
            m_in_buffer_file_pos_log = false;
            for (m = 0; m < m_samples_per_pixel; m++)
            {
                if ((m_actable_offset[m] != 0) && ((m == 0) || (m_actable_offset[m] != m_actable_offset[m - 1])))
                {
                    for (n = 0; n < m - 1; n++)
                    {
                        if (m_actable_offset[m] == m_actable_offset[n])
                        {
                            return false;
                        }
                    }
                    TiffStream stream = m_tif.GetStream();
                    stream.Seek(m_tif.m_clientdata, m_actable_offset[m], SeekOrigin.Begin);
                    p = (uint)stream.Read(m_tif.m_clientdata, o, 0, 16);
                    if (p != 16)
                        return false;
                    q = 0;
                    for (n = 0; n < 16; n++)
                        q += o[n];
                    ra = 21 + q;
                    rb = new byte[ra];
                    rb[0] = 255;
                    rb[1] = (byte)JPEG_MARKER.DHT;
                    rb[2] = (byte)((19 + q) >> 8);
                    rb[3] = (byte)((19 + q) & 255);
                    rb[4] = (byte)(16 | m);
                    for (n = 0; n < 16; n++)
                        rb[5 + n] = o[n];

                    p = (uint)stream.Read(m_tif.m_clientdata, rb, 21, (int)q);
                    if (p != q)
                        return false;
                    m_actable[m] = rb;
                    m_sos_tda[m] = (byte)(m_sos_tda[m] | m);
                }
                else
                    m_sos_tda[m] = (byte)(m_sos_tda[m] | (m_sos_tda[m - 1] & 15));
            }
            return true;
        }

        private bool OJPEGReadBufferFill()
        {
            ushort m;
            int n;
            do
            {
                if (m_in_buffer_file_togo != 0)
                {
                    TiffStream stream = m_tif.GetStream();
                    if (!m_in_buffer_file_pos_log)
                    {
                        stream.Seek(m_tif.m_clientdata, m_in_buffer_file_pos, SeekOrigin.Begin);
                        m_in_buffer_file_pos_log = true;
                    }
                    m = OJPEG_BUFFER;
                    if (m > m_in_buffer_file_togo)
                        m = (ushort)m_in_buffer_file_togo;

                    n = stream.Read(m_tif.m_clientdata, m_in_buffer, 0, (int)m);
                    if (n == 0)
                        return false;
                    m = (ushort)n;
                    m_in_buffer_togo = m;
                    m_in_buffer_cur = 0;
                    m_in_buffer_file_togo -= m;
                    m_in_buffer_file_pos += m;
                    break;
                }
                m_in_buffer_file_pos_log = false;
                switch (m_in_buffer_source)
                {
                    case OJPEGStateInBufferSource.osibsNotSetYet:
                        if (m_jpeg_interchange_format != 0)
                        {
                            m_in_buffer_file_pos = m_jpeg_interchange_format;
                            m_in_buffer_file_togo = m_jpeg_interchange_format_length;
                        }
                        m_in_buffer_source = OJPEGStateInBufferSource.osibsJpegInterchangeFormat;
                        break;
                    case OJPEGStateInBufferSource.osibsJpegInterchangeFormat:
                        m_in_buffer_source = OJPEGStateInBufferSource.osibsStrile;
                        goto case OJPEGStateInBufferSource.osibsStrile;
                    case OJPEGStateInBufferSource.osibsStrile:
                        if (m_in_buffer_next_strile == m_in_buffer_strile_count)
                            m_in_buffer_source = OJPEGStateInBufferSource.osibsEof;
                        else
                        {
                            if (m_tif.m_dir.td_stripoffset == null)
                            {
                                return false;
                            }
                            m_in_buffer_file_pos = m_tif.m_dir.td_stripoffset[m_in_buffer_next_strile];
                            if (m_in_buffer_file_pos != 0)
                            {
                                if (m_in_buffer_file_pos >= m_file_size)
                                    m_in_buffer_file_pos = 0;
                                else
                                {
                                    m_in_buffer_file_togo = m_tif.m_dir.td_stripbytecount[m_in_buffer_next_strile];
                                    if (m_in_buffer_file_togo == 0)
                                        m_in_buffer_file_pos = 0;
                                    else if (m_in_buffer_file_pos + m_in_buffer_file_togo > m_file_size)
                                        m_in_buffer_file_togo = m_file_size - m_in_buffer_file_pos;
                                }
                            }
                            m_in_buffer_next_strile++;
                        }
                        break;
                    default:
                        return false;
                }
            } while (true);
            return true;
        }

        private bool OJPEGReadByte(out byte b)
        {
            if (m_in_buffer_togo == 0)
            {
                if (!OJPEGReadBufferFill())
                {
                    b = 0;
                    return false;
                }
            }

            b = m_in_buffer[m_in_buffer_cur];
            m_in_buffer_cur++;
            m_in_buffer_togo--;
            return true;
        }

        public bool OJPEGReadBytePeek(out byte b)
        {
            if (m_in_buffer_togo == 0)
            {
                if (!OJPEGReadBufferFill())
                {
                    b = 0;
                    return false;
                }
            }

            b = m_in_buffer[m_in_buffer_cur];
            return true;
        }

        private void OJPEGReadByteAdvance()
        {
            m_in_buffer_cur++;
            m_in_buffer_togo--;
        }

        private bool OJPEGReadWord(out ushort word)
        {
            word = 0;
            byte m;
            if (!OJPEGReadByte(out m))
                return false;

            word = (ushort)(m << 8);
            if (!OJPEGReadByte(out m))
                return false;

            word |= m;
            return true;
        }

        public bool OJPEGReadBlock(ushort len, byte[] mem, int offset)
        {
            ushort mlen;
            ushort n;
            mlen = len;
            int mmem = offset;
            do
            {
                if (m_in_buffer_togo == 0)
                {
                    if (!OJPEGReadBufferFill())
                        return false;
                }
                n = mlen;
                if (n > m_in_buffer_togo)
                    n = m_in_buffer_togo;

                Buffer.BlockCopy(m_in_buffer, m_in_buffer_cur, mem, mmem, n);
                m_in_buffer_cur += n;
                m_in_buffer_togo -= n;
                mlen -= n;
                mmem += n;
            } while (mlen > 0);
            return true;
        }

        private void OJPEGReadSkip(ushort len)
        {
            ushort m;
            ushort n;
            m = len;
            n = m;
            if (n > m_in_buffer_togo)
                n = m_in_buffer_togo;
            m_in_buffer_cur += n;
            m_in_buffer_togo -= n;
            m -= n;
            if (m > 0)
            {
                n = m;
                if (n > m_in_buffer_file_togo)
                    n = (ushort)m_in_buffer_file_togo;
                m_in_buffer_file_pos += n;
                m_in_buffer_file_togo -= n;
                m_in_buffer_file_pos_log = false;
            }
        }

        internal bool OJPEGWriteStream(out byte[] mem, out uint len)
        {
            mem = null;
            len = 0;
            do
            {
                switch (m_out_state)
                {
                    case OJPEGStateOutState.ososSoi:
                        OJPEGWriteStreamSoi(out mem, out len);
                        break;
                    case OJPEGStateOutState.ososQTable0:
                        OJPEGWriteStreamQTable(0, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososQTable1:
                        OJPEGWriteStreamQTable(1, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososQTable2:
                        OJPEGWriteStreamQTable(2, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososQTable3:
                        OJPEGWriteStreamQTable(3, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososDcTable0:
                        OJPEGWriteStreamDcTable(0, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososDcTable1:
                        OJPEGWriteStreamDcTable(1, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososDcTable2:
                        OJPEGWriteStreamDcTable(2, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososDcTable3:
                        OJPEGWriteStreamDcTable(3, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososAcTable0:
                        OJPEGWriteStreamAcTable(0, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososAcTable1:
                        OJPEGWriteStreamAcTable(1, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososAcTable2:
                        OJPEGWriteStreamAcTable(2, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososAcTable3:
                        OJPEGWriteStreamAcTable(3, out mem, out len);
                        break;
                    case OJPEGStateOutState.ososDri:
                        OJPEGWriteStreamDri(out mem, out len);
                        break;
                    case OJPEGStateOutState.ososSof:
                        OJPEGWriteStreamSof(out mem, out len);
                        break;
                    case OJPEGStateOutState.ososSos:
                        OJPEGWriteStreamSos(out mem, out len);
                        break;
                    case OJPEGStateOutState.ososCompressed:
                        if (!OJPEGWriteStreamCompressed(out mem, out len))
                            return false;
                        break;
                    case OJPEGStateOutState.ososRst:
                        OJPEGWriteStreamRst(out mem, out len);
                        break;
                    case OJPEGStateOutState.ososEoi:
                        OJPEGWriteStreamEoi(out mem, out len);
                        break;
                }
            } while (len == 0);
            return true;
        }

        private void OJPEGWriteStreamSoi(out byte[] mem, out uint len)
        {
            m_out_buffer[0] = 255;
            m_out_buffer[1] = (byte)JPEG_MARKER.SOI;
            len = 2;
            mem = m_out_buffer;
            m_out_state++;
        }

        private void OJPEGWriteStreamQTable(byte table_index, out byte[] mem, out uint len)
        {
            mem = null;
            len = 0;

            if (m_qtable[table_index] != null)
            {
                mem = m_qtable[table_index];
                len = (uint)m_qtable[table_index].Length;
            }
            m_out_state++;
        }

        private void OJPEGWriteStreamDcTable(byte table_index, out byte[] mem, out uint len)
        {
            mem = null;
            len = 0;

            if (m_dctable[table_index] != null)
            {
                mem = m_dctable[table_index];
                len = (uint)m_dctable[table_index].Length;
            }
            m_out_state++;
        }

        private void OJPEGWriteStreamAcTable(byte table_index, out byte[] mem, out uint len)
        {
            mem = null;
            len = 0;

            if (m_actable[table_index] != null)
            {
                mem = m_actable[table_index];
                len = (uint)m_actable[table_index].Length;
            }
            m_out_state++;
        }

        private void OJPEGWriteStreamDri(out byte[] mem, out uint len)
        {
            mem = null;
            len = 0;

            if (m_restart_interval != 0)
            {
                m_out_buffer[0] = 255;
                m_out_buffer[1] = (byte)JPEG_MARKER.DRI;
                m_out_buffer[2] = 0;
                m_out_buffer[3] = 4;
                m_out_buffer[4] = (byte)(m_restart_interval >> 8);
                m_out_buffer[5] = (byte)(m_restart_interval & 255);
                len = 6;
                mem = m_out_buffer;
            }
            m_out_state++;
        }

        private void OJPEGWriteStreamSof(out byte[] mem, out uint len)
        {
            byte m;
            m_out_buffer[0] = 255;
            m_out_buffer[1] = m_sof_marker_id;
            /* Lf */
            m_out_buffer[2] = 0;
            m_out_buffer[3] = (byte)(8 + m_samples_per_pixel_per_plane * 3);
            /* P */
            m_out_buffer[4] = 8;
            /* Y */
            m_out_buffer[5] = (byte)(m_sof_y >> 8);
            m_out_buffer[6] = (byte)(m_sof_y & 255);
            /* X */
            m_out_buffer[7] = (byte)(m_sof_x >> 8);
            m_out_buffer[8] = (byte)(m_sof_x & 255);
            /* Nf */
            m_out_buffer[9] = m_samples_per_pixel_per_plane;
            for (m = 0; m < m_samples_per_pixel_per_plane; m++)
            {
                /* C */
                m_out_buffer[10 + m * 3] = m_sof_c[m_plane_sample_offset + m];
                /* H and V */
                m_out_buffer[10 + m * 3 + 1] = m_sof_hv[m_plane_sample_offset + m];
                /* Tq */
                m_out_buffer[10 + m * 3 + 2] = m_sof_tq[m_plane_sample_offset + m];
            }
            len = (uint)(10 + m_samples_per_pixel_per_plane * 3);
            mem = m_out_buffer;
            m_out_state++;
        }

        private void OJPEGWriteStreamSos(out byte[] mem, out uint len)
        {
            byte m;
            m_out_buffer[0] = 255;
            m_out_buffer[1] = (byte)JPEG_MARKER.SOS;
            /* Ls */
            m_out_buffer[2] = 0;
            m_out_buffer[3] = (byte)(6 + m_samples_per_pixel_per_plane * 2);
            /* Ns */
            m_out_buffer[4] = m_samples_per_pixel_per_plane;
            for (m = 0; m < m_samples_per_pixel_per_plane; m++)
            {
                /* Cs */
                m_out_buffer[5 + m * 2] = m_sos_cs[m_plane_sample_offset + m];
                /* Td and Ta */
                m_out_buffer[5 + m * 2 + 1] = m_sos_tda[m_plane_sample_offset + m];
            }
            /* Ss */
            m_out_buffer[5 + m_samples_per_pixel_per_plane * 2] = 0;
            /* Se */
            m_out_buffer[5 + m_samples_per_pixel_per_plane * 2 + 1] = 63;
            /* Ah and Al */
            m_out_buffer[5 + m_samples_per_pixel_per_plane * 2 + 2] = 0;
            len = (uint)(8 + m_samples_per_pixel_per_plane * 2);
            mem = m_out_buffer;
            m_out_state++;
        }

        private bool OJPEGWriteStreamCompressed(out byte[] mem, out uint len)
        {
            mem = null;
            len = 0;

            if (m_in_buffer_togo == 0)
            {
                if (!OJPEGReadBufferFill())
                    return false;
            }
            len = m_in_buffer_togo;

            if (m_in_buffer_cur == 0)
            {
                mem = m_in_buffer;
            }
            else
            {
                mem = new byte[len];
                Buffer.BlockCopy(m_in_buffer, m_in_buffer_cur, mem, 0, (int)len);
            }

            m_in_buffer_togo = 0;
            if (m_in_buffer_file_togo == 0)
            {
                switch (m_in_buffer_source)
                {
                    case OJPEGStateInBufferSource.osibsStrile:
                        if (m_in_buffer_next_strile < m_in_buffer_strile_count)
                            m_out_state = OJPEGStateOutState.ososRst;
                        else
                            m_out_state = OJPEGStateOutState.ososEoi;
                        break;
                    case OJPEGStateInBufferSource.osibsEof:
                        m_out_state = OJPEGStateOutState.ososEoi;
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        private void OJPEGWriteStreamRst(out byte[] mem, out uint len)
        {
            m_out_buffer[0] = 255;
            m_out_buffer[1] = (byte)((byte)JPEG_MARKER.RST0 + m_restart_index);
            m_restart_index++;
            if (m_restart_index == 8)
                m_restart_index = 0;
            len = 2;
            mem = m_out_buffer;
            m_out_state = OJPEGStateOutState.ososCompressed;
        }

        private void OJPEGWriteStreamEoi(out byte[] mem, out uint len)
        {
            m_out_buffer[0] = 255;
            m_out_buffer[1] = (byte)JPEG_MARKER.EOI;
            len = 2;
            mem = m_out_buffer;
        }

        private bool jpeg_create_decompress_encap()
        {
            try
            {
                m_libjpeg_jpeg_decompress_struct = new DecompressStruct();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private ReadResult jpeg_read_header_encap(bool require_image)
        {
            ReadResult res = ReadResult.JPEG_SUSPENDED;
            try
            {
                res = m_libjpeg_jpeg_decompress_struct.jpeg_read_header(require_image);
            }
            catch (Exception)
            {
                return ReadResult.JPEG_SUSPENDED;
            }

            return res;
        }

        private bool jpeg_start_decompress_encap()
        {
            try
            {
                m_libjpeg_jpeg_decompress_struct.jpeg_start_decompress();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private int jpeg_read_scanlines_encap(byte[] scanlines, int max_lines)
        {
            int n = 0;
            try
            {
                byte[][] temp = new byte[1][];
                temp[0] = scanlines;
                n = m_libjpeg_jpeg_decompress_struct.jpeg_read_scanlines(temp, max_lines);
            }
            catch (Exception)
            {
                return 0;
            }

            return n;
        }

        private int jpeg_read_raw_data_encap(int max_lines)
        {
            int n = 0;
            try
            {
                n = m_libjpeg_jpeg_decompress_struct.jpeg_read_raw_data(m_subsampling_convert_ycbcrimage, max_lines);
            }
            catch (Exception)
            {
                return 0;
            }

            return n;
        }
    }

    class OJpegCodecTagMethods : TiffTagMethods
    {
        public override bool SetField(Tiff tif, TiffTag tag, FieldValue[] ap)
        {
            const string module = "OJPEGVSetField";
            OJpegCodec sp = tif.m_currentCodec as OJpegCodec;

            uint ma;
            uint[] mb;
            uint n;
            switch (tag)
            {
                case TiffTag.JPEGIFOFFSET:
                    sp.m_jpeg_interchange_format = ap[0].ToUInt();
                    break;
                case TiffTag.JPEGIFBYTECOUNT:
                    sp.m_jpeg_interchange_format_length = ap[0].ToUInt();
                    break;
                case TiffTag.YCBCRSUBSAMPLING:
                    sp.m_subsampling_tag = true;
                    sp.m_subsampling_hor = ap[0].ToByte();
                    sp.m_subsampling_ver = ap[1].ToByte();
                    tif.m_dir.td_ycbcrsubsampling[0] = sp.m_subsampling_hor;
                    tif.m_dir.td_ycbcrsubsampling[1] = sp.m_subsampling_ver;
                    break;
                case TiffTag.JPEGQTABLES:
                    ma = ap[0].ToUInt();
                    if (ma != 0)
                    {
                        if (ma > 3)
                        {
                            return false;
                        }
                        sp.m_qtable_offset_count = (byte)ma;
                        mb = ap[1].ToUIntArray();
                        for (n = 0; n < ma; n++)
                            sp.m_qtable_offset[n] = mb[n];
                    }
                    break;
                case TiffTag.JPEGDCTABLES:
                    ma = ap[0].ToUInt();
                    if (ma != 0)
                    {
                        if (ma > 3)
                        {
                            return false;
                        }
                        sp.m_dctable_offset_count = (byte)ma;
                        mb = ap[1].ToUIntArray();
                        for (n = 0; n < ma; n++)
                            sp.m_dctable_offset[n] = mb[n];
                    }
                    break;
                case TiffTag.JPEGACTABLES:
                    ma = ap[0].ToUInt();
                    if (ma != 0)
                    {
                        if (ma > 3)
                        {
                            return false;
                        }
                        sp.m_actable_offset_count = (byte)ma;
                        mb = ap[1].ToUIntArray();
                        for (n = 0; n < ma; n++)
                            sp.m_actable_offset[n] = mb[n];
                    }
                    break;
                case TiffTag.JPEGPROC:
                    sp.m_jpeg_proc = ap[0].ToByte();
                    break;
                case TiffTag.JPEGRESTARTINTERVAL:
                    sp.m_restart_interval = ap[0].ToUShort();
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
            OJpegCodec sp = tif.m_currentCodec as OJpegCodec;

            FieldValue[] result = null;

            switch (tag)
            {
                case TiffTag.JPEGIFOFFSET:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_jpeg_interchange_format);
                    break;
                case TiffTag.JPEGIFBYTECOUNT:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_jpeg_interchange_format_length);
                    break;
                case TiffTag.YCBCRSUBSAMPLING:
                    if (!sp.m_subsamplingcorrect_done)
                        sp.OJPEGSubsamplingCorrect();

                    result = new FieldValue[2];
                    result[0].Set(sp.m_subsampling_hor);
                    result[1].Set(sp.m_subsampling_ver);
                    break;
                case TiffTag.JPEGQTABLES:
                    result = new FieldValue[2];
                    result[0].Set(sp.m_qtable_offset_count);
                    result[1].Set(sp.m_qtable_offset);
                    break;
                case TiffTag.JPEGDCTABLES:
                    result = new FieldValue[2];
                    result[0].Set(sp.m_dctable_offset_count);
                    result[1].Set(sp.m_dctable_offset);
                    break;
                case TiffTag.JPEGACTABLES:
                    result = new FieldValue[2];
                    result[0].Set(sp.m_actable_offset_count);
                    result[1].Set(sp.m_actable_offset);
                    break;
                case TiffTag.JPEGPROC:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_jpeg_proc);
                    break;
                case TiffTag.JPEGRESTARTINTERVAL:
                    result = new FieldValue[1];
                    result[0].Set(sp.m_restart_interval);
                    break;
                default:
                    return base.GetField(tif, tag);
            }

            return result;
        }
    }

    class OJpegSrcManager : SourceMgr
    {
        protected OJpegCodec m_sp;

        public OJpegSrcManager(OJpegCodec sp)
        {
            initInternalBuffer(null, 0);
            m_sp = sp;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void init_source()
        {
        }

        /// <summary>
        /// Fills input buffer
        /// </summary>
        public override bool fill_input_buffer()
        {
            Tiff tif = m_sp.GetTiff();
            byte[] mem = null;
            uint len = 0;
            if (!m_sp.OJPEGWriteStream(out mem, out len))
            { }
            initInternalBuffer(mem, (int)len);
            return true;
        }

        /// <summary>
        /// Skip data - used to skip over a potentially large amount of
        /// uninteresting data (such as an APPn marker).
        /// </summary>
        public override void skip_input_data(int num_bytes)
        {
            Tiff tif = m_sp.GetTiff();
        }

        /// <summary>
        /// This is the default resync_to_restart method for data source
        /// managers to use if they don't have any better approach.
        /// </summary>
        public override bool resync_to_restart(DecompressStruct cinfo, int desired)
        {
            Tiff tif = m_sp.GetTiff();
            return false;
        }

        /// <summary>
        /// Terminate source - called by jpeg_finish_decompress
        /// after all data has been read.  Often a no-op.
        /// </summary>
        public override void term_source()
        {
        }
    }
}
