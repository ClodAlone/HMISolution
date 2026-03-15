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

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    class JpegUtils
    {
        public static readonly int[] jpeg_natural_order = 
        { 
            0, 1, 8, 16, 9, 2, 3, 10, 17, 24, 32, 25, 18, 11, 4, 5, 12,
            19, 26, 33, 40, 48, 41, 34, 27, 20, 13, 6, 7, 14, 21, 28, 35,
            42, 49, 56, 57, 50, 43, 36, 29, 22, 15, 23, 30, 37, 44, 51,
            58, 59, 52, 45, 38, 31, 39, 46, 53, 60, 61, 54, 47, 55, 62,
            63, 63, 63, 63, 63, 63, 63, 63, 63,
            63, 63, 63, 63, 63, 63, 63, 63 
        };

        public static int RIGHT_SHIFT(int x, int shft)
        {
            return (x >> shft);
        }
        
        public static int DESCALE(int x, int n)
        {
            return RIGHT_SHIFT(x + (1 << (n - 1)), n);
        }

        /// <summary>
        /// Compute a/b rounded up to next integer, ie, ceil(a/b)
        /// </summary>
        public static int jdiv_round_up(int a, int b)
        {
            return (a + b - 1) / b;
        }

        /// <summary>
        /// Compute a rounded up to next multiple of b, ie, ceil(a/b)*b
        /// </summary>
        public static int jround_up(int a, int b)
        {
            a += b - 1;
            return a - (a % b);
        }

        /// <summary>
        /// Copy some rows of samples from one place to another.
        /// </summary>
        public static void jcopy_sample_rows(ComponentBuffer input_array, int source_row, byte[][] output_array, int dest_row, int num_rows, int num_cols)
        {
            for (int row = 0; row < num_rows; row++)
                Buffer.BlockCopy(input_array[source_row + row], 0, output_array[dest_row + row], 0, num_cols);
        }

        public static void jcopy_sample_rows(ComponentBuffer input_array, int source_row, ComponentBuffer output_array, int dest_row, int num_rows, int num_cols)
        {
            for (int row = 0; row < num_rows; row++)
                Buffer.BlockCopy(input_array[source_row + row], 0, output_array[dest_row + row], 0, num_cols);
        }

        public static void jcopy_sample_rows(byte[][] input_array, int source_row, byte[][] output_array, int dest_row, int num_rows, int num_cols)
        {
            for (int row = 0; row < num_rows; row++)
                Buffer.BlockCopy(input_array[source_row++], 0, output_array[dest_row++], 0, num_cols);
        }
    }

    /// <summary>
    /// Expanded entropy decoder object for Huffman decoding.
    /// </summary>
    class HuffEntropyDecoder : EntropyDecoder
    {
        private class savable_state
        {
            public int[] last_dc_val = new int[JpegConstants.MAX_COMPS_IN_SCAN];

            public void Assign(savable_state ss)
            {
                Buffer.BlockCopy(ss.last_dc_val, 0, last_dc_val, 0, last_dc_val.Length * sizeof(int));
            }
        }

        private BitReadPermState m_bitstate;
        private savable_state m_saved = new savable_state();
        private int m_restarts_to_go;

        private DDerivedTbl[] m_dc_derived_tbls = new DDerivedTbl[JpegConstants.NUM_HUFF_TBLS];
        private DDerivedTbl[] m_ac_derived_tbls = new DDerivedTbl[JpegConstants.NUM_HUFF_TBLS];

        private DDerivedTbl[] m_dc_cur_tbls = new DDerivedTbl[JpegConstants.D_MAX_BLOCKS_IN_MCU];
        private DDerivedTbl[] m_ac_cur_tbls = new DDerivedTbl[JpegConstants.D_MAX_BLOCKS_IN_MCU];

        private bool[] m_dc_needed = new bool[JpegConstants.D_MAX_BLOCKS_IN_MCU];
        private bool[] m_ac_needed = new bool[JpegConstants.D_MAX_BLOCKS_IN_MCU];

        public HuffEntropyDecoder(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;

            for (int i = 0; i < JpegConstants.NUM_HUFF_TBLS; i++)
                m_dc_derived_tbls[i] = m_ac_derived_tbls[i] = null;
        }

        /// <summary>
        /// Initialize for a Huffman-compressed scan.
        /// </summary>
        public override void start_pass()
        {
            for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
            {
                ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]];
                int dctbl = componentInfo.Dc_tbl_no;
                int actbl = componentInfo.Ac_tbl_no;

                jpeg_make_d_derived_tbl(true, dctbl, ref m_dc_derived_tbls[dctbl]);
                jpeg_make_d_derived_tbl(false, actbl, ref m_ac_derived_tbls[actbl]);

                m_saved.last_dc_val[ci] = 0;
            }

            for (int blkn = 0; blkn < m_cinfo.m_blocks_in_MCU; blkn++)
            {
                int ci = m_cinfo.m_MCU_membership[blkn];
                ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]];

                m_dc_cur_tbls[blkn] = m_dc_derived_tbls[componentInfo.Dc_tbl_no];
                m_ac_cur_tbls[blkn] = m_ac_derived_tbls[componentInfo.Ac_tbl_no];

                if (componentInfo.component_needed)
                {
                    m_dc_needed[blkn] = true;
                    m_ac_needed[blkn] = (componentInfo.DCT_scaled_size > 1);
                }
                else
                {
                    m_dc_needed[blkn] = m_ac_needed[blkn] = false;
                }
            }

            m_bitstate.bits_left = 0;
            m_bitstate.get_buffer = 0;
            m_insufficient_data = false;

            m_restarts_to_go = m_cinfo.m_restart_interval;
        }

        /// <summary>
        /// Decode and return one MCU's worth of Huffman-compressed coefficients.
        /// </summary>
        public override bool decode_mcu(JBLOCK[] MCU_data)
        {
            if (m_cinfo.m_restart_interval != 0)
            {
                if (m_restarts_to_go == 0)
                {
                    if (!process_restart())
                        return false;
                }
            }

            if (!m_insufficient_data)
            {
                int get_buffer;
                int bits_left;
                BitReadWorkingState br_state = new BitReadWorkingState();
                BITREAD_LOAD_STATE(m_bitstate, out get_buffer, out bits_left, ref br_state);
                savable_state state = new savable_state();
                state.Assign(m_saved);

                for (int blkn = 0; blkn < m_cinfo.m_blocks_in_MCU; blkn++)
                {
                    int s;
                    if (!HUFF_DECODE(out s, ref br_state, m_dc_cur_tbls[blkn], ref get_buffer, ref bits_left))
                        return false;

                    if (s != 0)
                    {
                        if (!CHECK_BIT_BUFFER(ref br_state, s, ref get_buffer, ref bits_left))
                            return false;

                        int r = GET_BITS(s, get_buffer, ref bits_left);
                        s = HUFF_EXTEND(r, s);
                    }

                    if (m_dc_needed[blkn])
                    {
                        int ci = m_cinfo.m_MCU_membership[blkn];
                        s += state.last_dc_val[ci];
                        state.last_dc_val[ci] = s;

                        MCU_data[blkn][0] = (short)s;
                    }

                    if (m_ac_needed[blkn])
                    {
                        for (int k = 1; k < JpegConstants.DCTSIZE2; k++)
                        {
                            if (!HUFF_DECODE(out s, ref br_state, m_ac_cur_tbls[blkn], ref get_buffer, ref bits_left))
                                return false;

                            int r = s >> 4;
                            s &= 15;

                            if (s != 0)
                            {
                                k += r;
                                if (!CHECK_BIT_BUFFER(ref br_state, s, ref get_buffer, ref bits_left))
                                    return false;
                                r = GET_BITS(s, get_buffer, ref bits_left);
                                s = HUFF_EXTEND(r, s);

                                MCU_data[blkn][JpegUtils.jpeg_natural_order[k]] = (short)s;
                            }
                            else
                            {
                                if (r != 15)
                                    break;

                                k += 15;
                            }
                        }
                    }
                    else
                    {
                        for (int k = 1; k < JpegConstants.DCTSIZE2; k++)
                        {
                            if (!HUFF_DECODE(out s, ref br_state, m_ac_cur_tbls[blkn], ref get_buffer, ref bits_left))
                                return false;

                            int r = s >> 4;
                            s &= 15;

                            if (s != 0)
                            {
                                k += r;
                                if (!CHECK_BIT_BUFFER(ref br_state, s, ref get_buffer, ref bits_left))
                                    return false;

                                DROP_BITS(s, ref bits_left);
                            }
                            else
                            {
                                if (r != 15)
                                    break;

                                k += 15;
                            }
                        }
                    }
                }

                BITREAD_SAVE_STATE(ref m_bitstate, get_buffer, bits_left);
                m_saved.Assign(state);
            }

            m_restarts_to_go--;

            return true;

        }

        /// <summary>
        /// Check for a restart marker and resynchronize decoder.
        /// Returns false if must suspend.
        /// </summary>
        private bool process_restart()
        {
            m_cinfo.m_marker.SkipBytes(m_bitstate.bits_left / 8);
            m_bitstate.bits_left = 0;

            if (!m_cinfo.m_marker.read_restart_marker())
                return false;

            for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
                m_saved.last_dc_val[ci] = 0;

            m_restarts_to_go = m_cinfo.m_restart_interval;

            if (m_cinfo.m_unread_marker == 0)
                m_insufficient_data = false;

            return true;
        }
    }

    /// <summary>
    /// Colorspace conversion
    /// </summary>
    class ColorDeconverter
    {
        private const int SCALEBITS = 16;
        private const int ONE_HALF = 1 << (SCALEBITS - 1);

        private enum ColorConverter
        {
            grayscale_converter,
            ycc_rgb_converter,
            gray_rgb_converter,
            null_converter,
            ycck_cmyk_converter
        }

        private ColorConverter m_converter;
        private DecompressStruct m_cinfo;

        private int[] m_perComponentOffsets;

        private int[] m_Cr_r_tab;
        private int[] m_Cb_b_tab;
        private int[] m_Cr_g_tab;
        private int[] m_Cb_g_tab;

        /// <summary>
        /// Module initialization routine for output colorspace conversion.
        /// </summary>
        public ColorDeconverter(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;

            switch (cinfo.m_jpeg_color_space)
            {
                case J_COLOR_SPACE.JCS_GRAYSCALE:
                    break;

                case J_COLOR_SPACE.JCS_RGB:
                case J_COLOR_SPACE.JCS_YCbCr:
                    break;

                case J_COLOR_SPACE.JCS_CMYK:
                case J_COLOR_SPACE.JCS_YCCK:
                    break;

                default:
                    break;
            }

            switch (cinfo.m_out_color_space)
            {
                case J_COLOR_SPACE.JCS_GRAYSCALE:
                    cinfo.m_out_color_components = 1;
                    if (cinfo.m_jpeg_color_space == J_COLOR_SPACE.JCS_GRAYSCALE || cinfo.m_jpeg_color_space == J_COLOR_SPACE.JCS_YCbCr)
                    {
                        m_converter = ColorConverter.grayscale_converter;
                        for (int ci = 1; ci < cinfo.m_num_components; ci++)
                            cinfo.Comp_info[ci].component_needed = false;
                    }
                    break;

                case J_COLOR_SPACE.JCS_RGB:
                    cinfo.m_out_color_components = JpegConstants.RGB_PIXELSIZE;
                    if (cinfo.m_jpeg_color_space == J_COLOR_SPACE.JCS_YCbCr)
                    {
                        m_converter = ColorConverter.ycc_rgb_converter;
                        build_ycc_rgb_table();
                    }
                    else if (cinfo.m_jpeg_color_space == J_COLOR_SPACE.JCS_GRAYSCALE)
                        m_converter = ColorConverter.gray_rgb_converter;
                    else if (cinfo.m_jpeg_color_space == J_COLOR_SPACE.JCS_RGB)
                        m_converter = ColorConverter.null_converter;
                    break;

                case J_COLOR_SPACE.JCS_CMYK:
                    cinfo.m_out_color_components = 4;
                    if (cinfo.m_jpeg_color_space == J_COLOR_SPACE.JCS_YCCK)
                    {
                        m_converter = ColorConverter.ycck_cmyk_converter;
                        build_ycc_rgb_table();
                    }
                    else if (cinfo.m_jpeg_color_space == J_COLOR_SPACE.JCS_CMYK)
                        m_converter = ColorConverter.null_converter;
                    break;

                default:
                    if (cinfo.m_out_color_space == cinfo.m_jpeg_color_space)
                    {
                        cinfo.m_out_color_components = cinfo.m_num_components;
                        m_converter = ColorConverter.null_converter;
                    }
                    else
                    {
                    }
                    break;
            }

            if (cinfo.m_quantize_colors)
                cinfo.m_output_components = 1; /* single colormapped output component */
            else
                cinfo.m_output_components = cinfo.m_out_color_components;
        }

        /// <summary>
        /// Convert some rows of samples to the output colorspace.
        /// </summary>
        public void color_convert(ComponentBuffer[] input_buf, int[] perComponentOffsets, int input_row, byte[][] output_buf, int output_row, int num_rows)
        {
            m_perComponentOffsets = perComponentOffsets;

            switch (m_converter)
            {
                case ColorConverter.grayscale_converter:
                    grayscale_convert(input_buf, input_row, output_buf, output_row, num_rows);
                    break;
                case ColorConverter.ycc_rgb_converter:
                    ycc_rgb_convert(input_buf, input_row, output_buf, output_row, num_rows);
                    break;
                case ColorConverter.gray_rgb_converter:
                    gray_rgb_convert(input_buf, input_row, output_buf, output_row, num_rows);
                    break;
                case ColorConverter.null_converter:
                    null_convert(input_buf, input_row, output_buf, output_row, num_rows);
                    break;
                case ColorConverter.ycck_cmyk_converter:
                    ycck_cmyk_convert(input_buf, input_row, output_buf, output_row, num_rows);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initialize tables for YCC->RGB colorspace conversion.
        /// </summary>
        private void build_ycc_rgb_table()
        {
            m_Cr_r_tab = new int[JpegConstants.MAXJSAMPLE + 1];
            m_Cb_b_tab = new int[JpegConstants.MAXJSAMPLE + 1];
            m_Cr_g_tab = new int[JpegConstants.MAXJSAMPLE + 1];
            m_Cb_g_tab = new int[JpegConstants.MAXJSAMPLE + 1];

            for (int i = 0, x = -JpegConstants.CENTERJSAMPLE; i <= JpegConstants.MAXJSAMPLE; i++, x++)
            {
                m_Cr_r_tab[i] = JpegUtils.RIGHT_SHIFT(FIX(1.40200) * x + ONE_HALF, SCALEBITS);
                m_Cb_b_tab[i] = JpegUtils.RIGHT_SHIFT(FIX(1.77200) * x + ONE_HALF, SCALEBITS);
                m_Cr_g_tab[i] = (-FIX(0.71414)) * x;
                m_Cb_g_tab[i] = (-FIX(0.34414)) * x + ONE_HALF;
            }
        }

        private void ycc_rgb_convert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
        {
            int component0RowOffset = m_perComponentOffsets[0];
            int component1RowOffset = m_perComponentOffsets[1];
            int component2RowOffset = m_perComponentOffsets[2];

            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset;

            for (int row = 0; row < num_rows; row++)
            {
                int columnOffset = 0;
                for (int col = 0; col < m_cinfo.m_output_width; col++)
                {
                    int y = input_buf[0][input_row + component0RowOffset][col];
                    int cb = input_buf[1][input_row + component1RowOffset][col];
                    int cr = input_buf[2][input_row + component2RowOffset][col];

                    /* Range-limiting is essential due to noise introduced by DCT losses. */
                    output_buf[output_row + row][columnOffset + JpegConstants.RGB_RED] = limit[limitOffset + y + m_Cr_r_tab[cr]];
                    output_buf[output_row + row][columnOffset + JpegConstants.RGB_GREEN] = limit[limitOffset + y + JpegUtils.RIGHT_SHIFT(m_Cb_g_tab[cb] + m_Cr_g_tab[cr], SCALEBITS)];
                    output_buf[output_row + row][columnOffset + JpegConstants.RGB_BLUE] = limit[limitOffset + y + m_Cb_b_tab[cb]];
                    columnOffset += JpegConstants.RGB_PIXELSIZE;
                }

                input_row++;
            }
        }

        /// <summary>
        /// Adobe-style YCCK->CMYK conversion.
        /// </summary>
        private void ycck_cmyk_convert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
        {
            int component0RowOffset = m_perComponentOffsets[0];
            int component1RowOffset = m_perComponentOffsets[1];
            int component2RowOffset = m_perComponentOffsets[2];
            int component3RowOffset = m_perComponentOffsets[3];

            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset;

            int num_cols = m_cinfo.m_output_width;
            for (int row = 0; row < num_rows; row++)
            {
                int columnOffset = 0;
                for (int col = 0; col < num_cols; col++)
                {
                    int y = input_buf[0][input_row + component0RowOffset][col];
                    int cb = input_buf[1][input_row + component1RowOffset][col];
                    int cr = input_buf[2][input_row + component2RowOffset][col];

                    output_buf[output_row + row][columnOffset] = limit[limitOffset + JpegConstants.MAXJSAMPLE - (y + m_Cr_r_tab[cr])]; /* red */
                    output_buf[output_row + row][columnOffset + 1] = limit[limitOffset + JpegConstants.MAXJSAMPLE - (y + JpegUtils.RIGHT_SHIFT(m_Cb_g_tab[cb] + m_Cr_g_tab[cr], SCALEBITS))]; /* green */
                    output_buf[output_row + row][columnOffset + 2] = limit[limitOffset + JpegConstants.MAXJSAMPLE - (y + m_Cb_b_tab[cb])]; /* blue */

                    output_buf[output_row + row][columnOffset + 3] = input_buf[3][input_row + component3RowOffset][col];
                    columnOffset += 4;
                }

                input_row++;
            }
        }

        /// <summary>
        /// Convert grayscale to RGB: just duplicate the graylevel three times.
        /// </summary>
        private void gray_rgb_convert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
        {
            int component0RowOffset = m_perComponentOffsets[0];
            int component1RowOffset = m_perComponentOffsets[1];
            int component2RowOffset = m_perComponentOffsets[2];

            int num_cols = m_cinfo.m_output_width;
            for (int row = 0; row < num_rows; row++)
            {
                int columnOffset = 0;
                for (int col = 0; col < num_cols; col++)
                {
                    output_buf[output_row + row][columnOffset + JpegConstants.RGB_RED] = input_buf[0][input_row + component0RowOffset][col];
                    output_buf[output_row + row][columnOffset + JpegConstants.RGB_GREEN] = input_buf[0][input_row + component1RowOffset][col];
                    output_buf[output_row + row][columnOffset + JpegConstants.RGB_BLUE] = input_buf[0][input_row + component2RowOffset][col];
                    columnOffset += JpegConstants.RGB_PIXELSIZE;
                }

                input_row++;
            }
        }

        /// <summary>
        /// Color conversion for grayscale: just copy the data.
        /// </summary>
        private void grayscale_convert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
        {
            JpegUtils.jcopy_sample_rows(input_buf[0], input_row + m_perComponentOffsets[0], output_buf, output_row, num_rows, m_cinfo.m_output_width);
        }

        /// <summary>
        /// Color conversion for no colorspace change: just copy the data,
        /// converting from separate-planes to interleaved representation.
        /// </summary>
        private void null_convert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
        {
            for (int row = 0; row < num_rows; row++)
            {
                for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
                {
                    int columnIndex = 0;
                    int componentOffset = 0;
                    int perComponentOffset = m_perComponentOffsets[ci];

                    for (int count = m_cinfo.m_output_width; count > 0; count--)
                    {
                        output_buf[output_row + row][ci + componentOffset] = input_buf[ci][input_row + perComponentOffset][columnIndex];
                        componentOffset += m_cinfo.m_num_components;
                        columnIndex++;
                    }
                }

                input_row++;
            }
        }

        private static int FIX(double x)
        {
            return (int)(x * (1L << SCALEBITS) + 0.5);
        }
    }

    /// <summary>
    /// Color quantization or color precision reduction
    /// </summary>
    interface ColorQuantizer
    {
        void start_pass(bool is_pre_scan);

        void color_quantize(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows);

        void finish_pass();
        void new_color_map();
    }

    /// <summary>
    /// Coefficient buffer control
    /// </summary>
    class DCoefController
    {
        private const int SAVED_COEFS = 6;
        private const int Q01_POS = 1;
        private const int Q10_POS = 8;
        private const int Q20_POS = 16;
        private const int Q11_POS = 9;
        private const int Q02_POS = 2;

        private enum DecompressorType
        {
            Ordinary,
            Smooth,
            OnePass
        }

        private DecompressStruct m_cinfo;
        private bool m_useDummyConsumeData;
        private DecompressorType m_decompressor;

        private int m_MCU_ctr;
        private int m_MCU_vert_offset;
        private int m_MCU_rows_per_iMCU_row;
        private JBLOCK[] m_MCU_buffer = new JBLOCK[JpegConstants.D_MAX_BLOCKS_IN_MCU];

        private jvirt_array<JBLOCK>[] m_whole_image = new jvirt_array<JBLOCK>[JpegConstants.MAX_COMPONENTS];
        private jvirt_array<JBLOCK>[] m_coef_arrays;

        private int[] m_coef_bits_latch;
        private int m_coef_bits_savedOffset;

        public DCoefController(DecompressStruct cinfo, bool need_full_buffer)
        {
            m_cinfo = cinfo;

            if (need_full_buffer)
            {
                for (int ci = 0; ci < cinfo.m_num_components; ci++)
                {
                    m_whole_image[ci] = CommonStruct.CreateBlocksArray(
                        JpegUtils.jround_up(cinfo.Comp_info[ci].Width_in_blocks, cinfo.Comp_info[ci].H_samp_factor),
                        JpegUtils.jround_up(cinfo.Comp_info[ci].height_in_blocks, cinfo.Comp_info[ci].V_samp_factor));
                    m_whole_image[ci].ErrorProcessor = cinfo;
                }

                m_useDummyConsumeData = false;
                m_decompressor = DecompressorType.Ordinary;
                m_coef_arrays = m_whole_image; /* link to virtual arrays */
            }
            else
            {
                JBLOCK[] buffer = new JBLOCK[JpegConstants.D_MAX_BLOCKS_IN_MCU];
                for (int i = 0; i < JpegConstants.D_MAX_BLOCKS_IN_MCU; i++)
                {
                    buffer[i] = new JBLOCK();
                    for (int ii = 0; ii < buffer[i].data.Length; ii++)
                        buffer[i].data[ii] = -12851;

                    m_MCU_buffer[i] = buffer[i];
                }

                m_useDummyConsumeData = true;
                m_decompressor = DecompressorType.OnePass;
                m_coef_arrays = null; /* flag for no virtual arrays */
            }
        }

        /// <summary>
        /// Initialize for an input processing pass.
        /// </summary>
        public void start_input_pass()
        {
            m_cinfo.m_input_iMCU_row = 0;
            start_iMCU_row();
        }

        /// <summary>
        /// Consume input data and store it in the full-image coefficient buffer.
        /// </summary>
        public ReadResult consume_data()
        {
            if (m_useDummyConsumeData)
                return ReadResult.JPEG_SUSPENDED;

            JBLOCK[][][] buffer = new JBLOCK[JpegConstants.MAX_COMPS_IN_SCAN][][];

            for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
            {
                ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]];

                buffer[ci] = m_whole_image[componentInfo.Component_index].Access(
                    m_cinfo.m_input_iMCU_row * componentInfo.V_samp_factor, componentInfo.V_samp_factor);
            }
            for (int yoffset = m_MCU_vert_offset; yoffset < m_MCU_rows_per_iMCU_row; yoffset++)
            {
                for (int MCU_col_num = m_MCU_ctr; MCU_col_num < m_cinfo.m_MCUs_per_row; MCU_col_num++)
                {
                    int blkn = 0;
                    for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
                    {
                        ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]];
                        int start_col = MCU_col_num * componentInfo.MCU_width;
                        for (int yindex = 0; yindex < componentInfo.MCU_height; yindex++)
                        {
                            for (int xindex = 0; xindex < componentInfo.MCU_width; xindex++)
                            {
                                m_MCU_buffer[blkn] = buffer[ci][yindex + yoffset][start_col + xindex];
                                blkn++;
                            }
                        }
                    }

                    if (!m_cinfo.m_entropy.decode_mcu(m_MCU_buffer))
                    {
                        m_MCU_vert_offset = yoffset;
                        m_MCU_ctr = MCU_col_num;
                        return ReadResult.JPEG_SUSPENDED;
                    }
                }

                m_MCU_ctr = 0;
            }

            m_cinfo.m_input_iMCU_row++;
            if (m_cinfo.m_input_iMCU_row < m_cinfo.m_total_iMCU_rows)
            {
                start_iMCU_row();
                return ReadResult.JPEG_ROW_COMPLETED;
            }

            m_cinfo.m_inputctl.finish_input_pass();
            return ReadResult.JPEG_SCAN_COMPLETED;
        }

        /// <summary>
        /// Initialize for an output processing pass.
        /// </summary>
        public void start_output_pass()
        {
            if (m_coef_arrays != null)
            {
                if (m_cinfo.m_do_block_smoothing && smoothing_ok())
                    m_decompressor = DecompressorType.Smooth;
                else
                    m_decompressor = DecompressorType.Ordinary;
            }

            m_cinfo.m_output_iMCU_row = 0;
        }

        public ReadResult decompress_data(ComponentBuffer[] output_buf)
        {
            switch (m_decompressor)
            {
                case DecompressorType.Ordinary:
                    return decompress_data_ordinary(output_buf);

                case DecompressorType.Smooth:
                    return decompress_smooth_data(output_buf);

                case DecompressorType.OnePass:
                    return decompress_onepass(output_buf);
            }

            return 0;
        }

        public jvirt_array<JBLOCK>[] GetCoefArrays()
        {
            return m_coef_arrays;
        }

        /// <summary>
        /// Decompress and return some data in the single-pass case.
        /// </summary>
        private ReadResult decompress_onepass(ComponentBuffer[] output_buf)
        {
            int last_MCU_col = m_cinfo.m_MCUs_per_row - 1;
            int last_iMCU_row = m_cinfo.m_total_iMCU_rows - 1;

            for (int yoffset = m_MCU_vert_offset; yoffset < m_MCU_rows_per_iMCU_row; yoffset++)
            {
                for (int MCU_col_num = m_MCU_ctr; MCU_col_num <= last_MCU_col; MCU_col_num++)
                {
                    for (int i = 0; i < m_cinfo.m_blocks_in_MCU; i++)
                        Array.Clear(m_MCU_buffer[i].data, 0, m_MCU_buffer[i].data.Length);

                    if (!m_cinfo.m_entropy.decode_mcu(m_MCU_buffer))
                    {
                        m_MCU_vert_offset = yoffset;
                        m_MCU_ctr = MCU_col_num;
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    int blkn = 0;
                    for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
                    {
                        ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]];

                        if (!componentInfo.component_needed)
                        {
                            blkn += componentInfo.MCU_blocks;
                            continue;
                        }

                        int useful_width = (MCU_col_num < last_MCU_col) ? componentInfo.MCU_width : componentInfo.last_col_width;
                        int outputIndex = yoffset * componentInfo.DCT_scaled_size;
                        int start_col = MCU_col_num * componentInfo.MCU_sample_width;
                        for (int yindex = 0; yindex < componentInfo.MCU_height; yindex++)
                        {
                            if (m_cinfo.m_input_iMCU_row < last_iMCU_row || yoffset + yindex < componentInfo.last_row_height)
                            {
                                int output_col = start_col;
                                for (int xindex = 0; xindex < useful_width; xindex++)
                                {
                                    m_cinfo.m_idct.inverse(componentInfo.Component_index,
                                        m_MCU_buffer[blkn + xindex].data, output_buf[componentInfo.Component_index],
                                        outputIndex, output_col);

                                    output_col += componentInfo.DCT_scaled_size;
                                }
                            }

                            blkn += componentInfo.MCU_width;
                            outputIndex += componentInfo.DCT_scaled_size;
                        }
                    }
                }

                m_MCU_ctr = 0;
            }

            m_cinfo.m_output_iMCU_row++;
            m_cinfo.m_input_iMCU_row++;
            if (m_cinfo.m_input_iMCU_row < m_cinfo.m_total_iMCU_rows)
            {
                start_iMCU_row();
                return ReadResult.JPEG_ROW_COMPLETED;
            }

            m_cinfo.m_inputctl.finish_input_pass();
            return ReadResult.JPEG_SCAN_COMPLETED;
        }

        /// <summary>
        /// Decompress and return some data in the multi-pass case.
        /// </summary>
        private ReadResult decompress_data_ordinary(ComponentBuffer[] output_buf)
        {
            while (m_cinfo.m_input_scan_number < m_cinfo.m_output_scan_number ||
                   (m_cinfo.m_input_scan_number == m_cinfo.m_output_scan_number &&
                    m_cinfo.m_input_iMCU_row <= m_cinfo.m_output_iMCU_row))
            {
                if (m_cinfo.m_inputctl.consume_input() == ReadResult.JPEG_SUSPENDED)
                    return ReadResult.JPEG_SUSPENDED;
            }

            int last_iMCU_row = m_cinfo.m_total_iMCU_rows - 1;

            /* OK, output from the virtual arrays. */
            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                ComponentInfo componentInfo = m_cinfo.Comp_info[ci];

                if (!componentInfo.component_needed)
                    continue;

                JBLOCK[][] buffer = m_whole_image[ci].Access(m_cinfo.m_output_iMCU_row * componentInfo.V_samp_factor,
                    componentInfo.V_samp_factor);

                int block_rows;
                if (m_cinfo.m_output_iMCU_row < last_iMCU_row)
                    block_rows = componentInfo.V_samp_factor;
                else
                {
                    block_rows = componentInfo.height_in_blocks % componentInfo.V_samp_factor;
                    if (block_rows == 0)
                        block_rows = componentInfo.V_samp_factor;
                }

                int rowIndex = 0;
                for (int block_row = 0; block_row < block_rows; block_row++)
                {
                    int output_col = 0;
                    for (int block_num = 0; block_num < componentInfo.Width_in_blocks; block_num++)
                    {
                        m_cinfo.m_idct.inverse(componentInfo.Component_index,
                            buffer[block_row][block_num].data, output_buf[ci], rowIndex, output_col);

                        output_col += componentInfo.DCT_scaled_size;
                    }

                    rowIndex += componentInfo.DCT_scaled_size;
                }
            }

            m_cinfo.m_output_iMCU_row++;
            if (m_cinfo.m_output_iMCU_row < m_cinfo.m_total_iMCU_rows)
                return ReadResult.JPEG_ROW_COMPLETED;

            return ReadResult.JPEG_SCAN_COMPLETED;
        }

        /// <summary>
        /// Variant of decompress_data for use when doing block smoothing.
        /// </summary>
        private ReadResult decompress_smooth_data(ComponentBuffer[] output_buf)
        {
            while (m_cinfo.m_input_scan_number <= m_cinfo.m_output_scan_number && !m_cinfo.m_inputctl.EOIReached())
            {
                if (m_cinfo.m_input_scan_number == m_cinfo.m_output_scan_number)
                {
                    int delta = (m_cinfo.m_Ss == 0) ? 1 : 0;
                    if (m_cinfo.m_input_iMCU_row > m_cinfo.m_output_iMCU_row + delta)
                        break;
                }

                if (m_cinfo.m_inputctl.consume_input() == ReadResult.JPEG_SUSPENDED)
                    return ReadResult.JPEG_SUSPENDED;
            }

            int last_iMCU_row = m_cinfo.m_total_iMCU_rows - 1;

            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                ComponentInfo componentInfo = m_cinfo.Comp_info[ci];

                if (!componentInfo.component_needed)
                    continue;

                int block_rows;
                int access_rows;
                bool last_row;
                if (m_cinfo.m_output_iMCU_row < last_iMCU_row)
                {
                    block_rows = componentInfo.V_samp_factor;
                    access_rows = block_rows * 2; /* this and next iMCU row */
                    last_row = false;
                }
                else
                {
                    block_rows = componentInfo.height_in_blocks % componentInfo.V_samp_factor;
                    if (block_rows == 0)
                        block_rows = componentInfo.V_samp_factor;
                    access_rows = block_rows; /* this iMCU row only */
                    last_row = true;
                }

                JBLOCK[][] buffer = null;
                bool first_row;
                int bufferRowOffset = 0;
                if (m_cinfo.m_output_iMCU_row > 0)
                {
                    access_rows += componentInfo.V_samp_factor; /* prior iMCU row too */
                    buffer = m_whole_image[ci].Access((m_cinfo.m_output_iMCU_row - 1) * componentInfo.V_samp_factor, access_rows);
                    bufferRowOffset = componentInfo.V_samp_factor; /* point to current iMCU row */
                    first_row = false;
                }
                else
                {
                    buffer = m_whole_image[ci].Access(0, access_rows);
                    first_row = true;
                }

                int coefBitsOffset = ci * SAVED_COEFS;
                int Q00 = componentInfo.quant_table.quantval[0];
                int Q01 = componentInfo.quant_table.quantval[Q01_POS];
                int Q10 = componentInfo.quant_table.quantval[Q10_POS];
                int Q20 = componentInfo.quant_table.quantval[Q20_POS];
                int Q11 = componentInfo.quant_table.quantval[Q11_POS];
                int Q02 = componentInfo.quant_table.quantval[Q02_POS];
                int outputIndex = ci;

                for (int block_row = 0; block_row < block_rows; block_row++)
                {
                    int bufferIndex = bufferRowOffset + block_row;

                    int prev_block_row = 0;
                    if (first_row && block_row == 0)
                        prev_block_row = bufferIndex;
                    else
                        prev_block_row = bufferIndex - 1;

                    int next_block_row = 0;
                    if (last_row && block_row == block_rows - 1)
                        next_block_row = bufferIndex;
                    else
                        next_block_row = bufferIndex + 1;
                    int DC1 = buffer[prev_block_row][0][0];
                    int DC2 = DC1;
                    int DC3 = DC1;

                    int DC4 = buffer[bufferIndex][0][0];
                    int DC5 = DC4;
                    int DC6 = DC4;

                    int DC7 = buffer[next_block_row][0][0];
                    int DC8 = DC7;
                    int DC9 = DC7;

                    int output_col = 0;
                    int last_block_column = componentInfo.Width_in_blocks - 1;
                    for (int block_num = 0; block_num <= last_block_column; block_num++)
                    {
                        JBLOCK workspace = new JBLOCK();
                        Buffer.BlockCopy(buffer[bufferIndex][0].data, 0, workspace.data, 0, workspace.data.Length * sizeof(short));

                        if (block_num < last_block_column)
                        {
                            DC3 = buffer[prev_block_row][1][0];
                            DC6 = buffer[bufferIndex][1][0];
                            DC9 = buffer[next_block_row][1][0];
                        }

                        int Al = m_coef_bits_latch[m_coef_bits_savedOffset + coefBitsOffset + 1];
                        if (Al != 0 && workspace[1] == 0)
                        {
                            int pred;
                            int num = 36 * Q00 * (DC4 - DC6);
                            if (num >= 0)
                            {
                                pred = ((Q01 << 7) + num) / (Q01 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                            }
                            else
                            {
                                pred = ((Q01 << 7) - num) / (Q01 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                                pred = -pred;
                            }
                            workspace[1] = (short)pred;
                        }

                        /* AC10 */
                        Al = m_coef_bits_latch[m_coef_bits_savedOffset + coefBitsOffset + 2];
                        if (Al != 0 && workspace[8] == 0)
                        {
                            int pred;
                            int num = 36 * Q00 * (DC2 - DC8);
                            if (num >= 0)
                            {
                                pred = ((Q10 << 7) + num) / (Q10 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                            }
                            else
                            {
                                pred = ((Q10 << 7) - num) / (Q10 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                                pred = -pred;
                            }
                            workspace[8] = (short)pred;
                        }

                        /* AC20 */
                        Al = m_coef_bits_latch[m_coef_bits_savedOffset + coefBitsOffset + 3];
                        if (Al != 0 && workspace[16] == 0)
                        {
                            int pred;
                            int num = 9 * Q00 * (DC2 + DC8 - 2 * DC5);
                            if (num >= 0)
                            {
                                pred = ((Q20 << 7) + num) / (Q20 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                            }
                            else
                            {
                                pred = ((Q20 << 7) - num) / (Q20 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                                pred = -pred;
                            }
                            workspace[16] = (short)pred;
                        }

                        /* AC11 */
                        Al = m_coef_bits_latch[m_coef_bits_savedOffset + coefBitsOffset + 4];
                        if (Al != 0 && workspace[9] == 0)
                        {
                            int pred;
                            int num = 5 * Q00 * (DC1 - DC3 - DC7 + DC9);
                            if (num >= 0)
                            {
                                pred = ((Q11 << 7) + num) / (Q11 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                            }
                            else
                            {
                                pred = ((Q11 << 7) - num) / (Q11 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                                pred = -pred;
                            }
                            workspace[9] = (short)pred;
                        }

                        /* AC02 */
                        Al = m_coef_bits_latch[m_coef_bits_savedOffset + coefBitsOffset + 5];
                        if (Al != 0 && workspace[2] == 0)
                        {
                            int pred;
                            int num = 9 * Q00 * (DC4 + DC6 - 2 * DC5);
                            if (num >= 0)
                            {
                                pred = ((Q02 << 7) + num) / (Q02 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                            }
                            else
                            {
                                pred = ((Q02 << 7) - num) / (Q02 << 8);
                                if (Al > 0 && pred >= (1 << Al))
                                    pred = (1 << Al) - 1;
                                pred = -pred;
                            }
                            workspace[2] = (short)pred;
                        }

                        /* OK, do the IDCT */
                        m_cinfo.m_idct.inverse(componentInfo.Component_index, workspace.data, output_buf[outputIndex], 0, output_col);
                        DC1 = DC2;
                        DC2 = DC3;
                        DC4 = DC5;
                        DC5 = DC6;
                        DC7 = DC8;
                        DC8 = DC9;

                        bufferIndex++;
                        prev_block_row++;
                        next_block_row++;

                        output_col += componentInfo.DCT_scaled_size;
                    }

                    outputIndex += componentInfo.DCT_scaled_size;
                }
            }

            m_cinfo.m_output_iMCU_row++;
            if (m_cinfo.m_output_iMCU_row < m_cinfo.m_total_iMCU_rows)
                return ReadResult.JPEG_ROW_COMPLETED;

            return ReadResult.JPEG_SCAN_COMPLETED;
        }

        /// <summary>
        /// Determine whether block smoothing is applicable and safe.
        /// </summary>
        private bool smoothing_ok()
        {
            if (!m_cinfo.m_progressive_mode || m_cinfo.m_coef_bits == null)
                return false;

            if (m_coef_bits_latch == null)
            {
                m_coef_bits_latch = new int[m_cinfo.m_num_components * SAVED_COEFS];
                m_coef_bits_savedOffset = 0;
            }

            bool smoothing_useful = false;
            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                /* All components' quantization values must already be latched. */
                JQUANT_TBL qtable = m_cinfo.Comp_info[ci].quant_table;
                if (qtable == null)
                    return false;

                /* Verify DC & first 5 AC quantizers are nonzero to avoid zero-divide. */
                if (qtable.quantval[0] == 0 || qtable.quantval[Q01_POS] == 0 ||
                    qtable.quantval[Q10_POS] == 0 || qtable.quantval[Q20_POS] == 0 ||
                    qtable.quantval[Q11_POS] == 0 || qtable.quantval[Q02_POS] == 0)
                {
                    return false;
                }

                /* DC values must be at least partly known for all components. */
                if (m_cinfo.m_coef_bits[ci][0] < 0)
                    return false;

                /* Block smoothing is helpful if some AC coefficients remain inaccurate. */
                for (int coefi = 1; coefi <= 5; coefi++)
                {
                    m_coef_bits_latch[m_coef_bits_savedOffset + coefi] = m_cinfo.m_coef_bits[ci][coefi];
                    if (m_cinfo.m_coef_bits[ci][coefi] != 0)
                        smoothing_useful = true;
                }

                m_coef_bits_savedOffset += SAVED_COEFS;
            }

            return smoothing_useful;
        }

        /// <summary>
        /// Reset within-iMCU-row counters for a new row (input side)
        /// </summary>
        private void start_iMCU_row()
        {
            if (m_cinfo.m_comps_in_scan > 1)
            {
                m_MCU_rows_per_iMCU_row = 1;
            }
            else
            {
                ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[0]];

                if (m_cinfo.m_input_iMCU_row < (m_cinfo.m_total_iMCU_rows - 1))
                    m_MCU_rows_per_iMCU_row = componentInfo.V_samp_factor;
                else
                    m_MCU_rows_per_iMCU_row = componentInfo.last_row_height;
            }

            m_MCU_ctr = 0;
            m_MCU_vert_offset = 0;
        }
    }

    /// <summary>
    /// Main buffer control (downsampled-data buffer)
    /// </summary>
    class DMainController
    {
        private enum DataProcessor
        {
            context_main,
            simple_main,
            crank_post
        }

        private const int CTX_PREPARE_FOR_IMCU = 0;
        private const int CTX_PROCESS_IMCU = 1;
        private const int CTX_POSTPONED_ROW = 2;

        private DataProcessor m_dataProcessor;
        private DecompressStruct m_cinfo;

        private byte[][][] m_buffer = new byte[JpegConstants.MAX_COMPONENTS][][];

        private bool m_buffer_full;
        private int m_rowgroup_ctr;
        private int[][][] m_funnyIndices = new int[2][][] { new int[JpegConstants.MAX_COMPONENTS][], new int[JpegConstants.MAX_COMPONENTS][] };
        private int[] m_funnyOffsets = new int[JpegConstants.MAX_COMPONENTS];
        private int m_whichFunny;

        private int m_context_state;
        private int m_rowgroups_avail;
        private int m_iMCU_row_ctr;

        public DMainController(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;

            int ngroups = cinfo.m_min_DCT_scaled_size;
            if (cinfo.m_upsample.NeedContextRows())
            {
                alloc_funny_pointers();
                ngroups = cinfo.m_min_DCT_scaled_size + 2;
            }

            for (int ci = 0; ci < cinfo.m_num_components; ci++)
            {
                /* height of a row group of component */
                int rgroup = (cinfo.Comp_info[ci].V_samp_factor * cinfo.Comp_info[ci].DCT_scaled_size) / cinfo.m_min_DCT_scaled_size;

                m_buffer[ci] = CommonStruct.AllocJpegSamples(
                    cinfo.Comp_info[ci].Width_in_blocks * cinfo.Comp_info[ci].DCT_scaled_size,
                    rgroup * ngroups);
            }
        }

        /// <summary>
        /// Initialize for a processing pass.
        /// </summary>
        public void start_pass(J_BUF_MODE pass_mode)
        {
            switch (pass_mode)
            {
                case J_BUF_MODE.JBUF_PASS_THRU:
                    if (m_cinfo.m_upsample.NeedContextRows())
                    {
                        m_dataProcessor = DataProcessor.context_main;
                        make_funny_pointers(); /* Create the xbuffer[] lists */
                        m_whichFunny = 0; /* Read first iMCU row into xbuffer[0] */
                        m_context_state = CTX_PREPARE_FOR_IMCU;
                        m_iMCU_row_ctr = 0;
                    }
                    else
                    {
                        /* Simple case with no context needed */
                        m_dataProcessor = DataProcessor.simple_main;
                    }
                    m_buffer_full = false;  /* Mark buffer empty */
                    m_rowgroup_ctr = 0;
                    break;
                case J_BUF_MODE.JBUF_CRANK_DEST:
                    /* For last pass of 2-pass quantization, just crank the postprocessor */
                    m_dataProcessor = DataProcessor.crank_post;
                    break;
                default:
                    break;
            }
        }

        public void process_data(byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            switch (m_dataProcessor)
            {
                case DataProcessor.simple_main:
                    process_data_simple_main(output_buf, ref out_row_ctr, out_rows_avail);
                    break;

                case DataProcessor.context_main:
                    process_data_context_main(output_buf, ref out_row_ctr, out_rows_avail);
                    break;

                case DataProcessor.crank_post:
                    process_data_crank_post(output_buf, ref out_row_ctr, out_rows_avail);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Process some data.
        /// This handles the simple case where no context is required.
        /// </summary>
        private void process_data_simple_main(byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            ComponentBuffer[] cb = new ComponentBuffer[JpegConstants.MAX_COMPONENTS];
            for (int i = 0; i < JpegConstants.MAX_COMPONENTS; i++)
            {
                cb[i] = new ComponentBuffer();
                cb[i].SetBuffer(m_buffer[i], null, 0);
            }

            if (!m_buffer_full)
            {
                if (m_cinfo.m_coef.decompress_data(cb) == ReadResult.JPEG_SUSPENDED)
                {
                    return;
                }

                m_buffer_full = true;
            }

            int rowgroups_avail = m_cinfo.m_min_DCT_scaled_size;

            m_cinfo.m_post.post_process_data(cb, ref m_rowgroup_ctr, rowgroups_avail, output_buf, ref out_row_ctr, out_rows_avail);

            if (m_rowgroup_ctr >= rowgroups_avail)
            {
                m_buffer_full = false;
                m_rowgroup_ctr = 0;
            }
        }

        /// <summary>
        /// Process some data.
        /// This handles the case where context rows must be provided.
        /// </summary>
        private void process_data_context_main(byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            ComponentBuffer[] cb = new ComponentBuffer[m_cinfo.m_num_components];
            for (int i = 0; i < m_cinfo.m_num_components; i++)
            {
                cb[i] = new ComponentBuffer();
                cb[i].SetBuffer(m_buffer[i], m_funnyIndices[m_whichFunny][i], m_funnyOffsets[i]);
            }

            if (!m_buffer_full)
            {
                if (m_cinfo.m_coef.decompress_data(cb) == ReadResult.JPEG_SUSPENDED)
                {
                    return;
                }

                m_buffer_full = true;

                m_iMCU_row_ctr++;
            }

            if (m_context_state == CTX_POSTPONED_ROW)
            {
                m_cinfo.m_post.post_process_data(cb, ref m_rowgroup_ctr,
                    m_rowgroups_avail, output_buf, ref out_row_ctr, out_rows_avail);

                if (m_rowgroup_ctr < m_rowgroups_avail)
                {
                    return;
                }

                m_context_state = CTX_PREPARE_FOR_IMCU;

                if (out_row_ctr >= out_rows_avail)
                {
                    return;
                }
            }

            if (m_context_state == CTX_PREPARE_FOR_IMCU)
            {
                m_rowgroup_ctr = 0;
                m_rowgroups_avail = m_cinfo.m_min_DCT_scaled_size - 1;

                if (m_iMCU_row_ctr == m_cinfo.m_total_iMCU_rows)
                    set_bottom_pointers();

                m_context_state = CTX_PROCESS_IMCU;
            }

            if (m_context_state == CTX_PROCESS_IMCU)
            {
                m_cinfo.m_post.post_process_data(cb, ref m_rowgroup_ctr,
                    m_rowgroups_avail, output_buf, ref out_row_ctr, out_rows_avail);

                if (m_rowgroup_ctr < m_rowgroups_avail)
                {
                    return;
                }

                if (m_iMCU_row_ctr == 1)
                    set_wraparound_pointers();

                m_whichFunny ^= 1;
                m_buffer_full = false;

                m_rowgroup_ctr = m_cinfo.m_min_DCT_scaled_size + 1;
                m_rowgroups_avail = m_cinfo.m_min_DCT_scaled_size + 2;
                m_context_state = CTX_POSTPONED_ROW;
            }
        }

        /// <summary>
        /// Process some data.
        /// </summary>
        private void process_data_crank_post(byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            int dummy = 0;
            m_cinfo.m_post.post_process_data(null, ref dummy, 0, output_buf, ref out_row_ctr, out_rows_avail);
        }

        /// <summary>
        /// Allocate space for the funny pointer lists.
        /// </summary>
        private void alloc_funny_pointers()
        {
            int M = m_cinfo.m_min_DCT_scaled_size;
            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                int rgroup = (m_cinfo.Comp_info[ci].V_samp_factor * m_cinfo.Comp_info[ci].DCT_scaled_size) / m_cinfo.m_min_DCT_scaled_size;

                m_funnyIndices[0][ci] = new int[rgroup * (M + 4)];
                m_funnyIndices[1][ci] = new int[rgroup * (M + 4)];
                m_funnyOffsets[ci] = rgroup;
            }
        }

        /// <summary>
        /// Create the funny pointer lists discussed in the comments above.
        /// </summary>
        private void make_funny_pointers()
        {
            int M = m_cinfo.m_min_DCT_scaled_size;
            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                int rgroup = (m_cinfo.Comp_info[ci].V_samp_factor * m_cinfo.Comp_info[ci].DCT_scaled_size) / m_cinfo.m_min_DCT_scaled_size;

                int[] ind0 = m_funnyIndices[0][ci];
                int[] ind1 = m_funnyIndices[1][ci];

                for (int i = 0; i < rgroup * (M + 2); i++)
                {
                    ind0[i + rgroup] = i;
                    ind1[i + rgroup] = i;
                }

                for (int i = 0; i < rgroup * 2; i++)
                {
                    ind1[rgroup * (M - 1) + i] = rgroup * M + i;
                    ind1[rgroup * (M + 1) + i] = rgroup * (M - 2) + i;
                }

                for (int i = 0; i < rgroup; i++)
                    ind0[i] = ind0[rgroup];
            }
        }

        /// <summary>
        /// Set up the "wraparound" pointers at top and bottom of the pointer lists.
        /// </summary>
        private void set_wraparound_pointers()
        {
            int M = m_cinfo.m_min_DCT_scaled_size;
            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                /* height of a row group of component */
                int rgroup = (m_cinfo.Comp_info[ci].V_samp_factor * m_cinfo.Comp_info[ci].DCT_scaled_size) / m_cinfo.m_min_DCT_scaled_size;

                int[] ind0 = m_funnyIndices[0][ci];
                int[] ind1 = m_funnyIndices[1][ci];

                for (int i = 0; i < rgroup; i++)
                {
                    ind0[i] = ind0[rgroup * (M + 2) + i];
                    ind1[i] = ind1[rgroup * (M + 2) + i];

                    ind0[rgroup * (M + 3) + i] = ind0[i + rgroup];
                    ind1[rgroup * (M + 3) + i] = ind1[i + rgroup];
                }
            }
        }

        /// <summary>
        /// Change the pointer lists to duplicate the last sample row at the bottom
        /// of the image.
        /// </summary>
        private void set_bottom_pointers()
        {
            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                /* Count sample rows in one iMCU row and in one row group */
                int iMCUheight = m_cinfo.Comp_info[ci].V_samp_factor * m_cinfo.Comp_info[ci].DCT_scaled_size;
                int rgroup = iMCUheight / m_cinfo.m_min_DCT_scaled_size;

                /* Count nondummy sample rows remaining for this component */
                int rows_left = m_cinfo.Comp_info[ci].downsampled_height % iMCUheight;
                if (rows_left == 0)
                    rows_left = iMCUheight;

                if (ci == 0)
                    m_rowgroups_avail = (rows_left - 1) / rgroup + 1;

                for (int i = 0; i < rgroup * 2; i++)
                    m_funnyIndices[m_whichFunny][ci][rows_left + i + rgroup] = m_funnyIndices[m_whichFunny][ci][rows_left - 1 + rgroup];
            }
        }
    }

    /// <summary>
    /// Decompression postprocessing (color quantization buffer control)
    /// </summary>
    class DPostController
    {
        private enum ProcessorType
        {
            OnePass,
            PrePass,
            Upsample,
            SecondPass
        }

        private ProcessorType m_processor;
        private DecompressStruct m_cinfo;
        private jvirt_array<byte> m_whole_image;  /* virtual array, or null if one-pass */
        private byte[][] m_buffer;
        private int m_strip_height;
        private int m_starting_row;
        private int m_next_row;

        /// <summary>
        /// Initialize postprocessing controller.
        /// </summary>
        public DPostController(DecompressStruct cinfo, bool need_full_buffer)
        {
            m_cinfo = cinfo;

            if (cinfo.m_quantize_colors)
            {
                m_strip_height = cinfo.m_max_v_samp_factor;

                if (need_full_buffer)
                {
                    m_whole_image = CommonStruct.CreateSamplesArray(
                        cinfo.m_output_width * cinfo.m_out_color_components,
                        JpegUtils.jround_up(cinfo.m_output_height, m_strip_height));
                    m_whole_image.ErrorProcessor = cinfo;
                }
                else
                {
                    m_buffer = CommonStruct.AllocJpegSamples(
                        cinfo.m_output_width * cinfo.m_out_color_components, m_strip_height);
                }
            }
        }

        /// <summary>
        /// Initialize for a processing pass.
        /// </summary>
        public void start_pass(J_BUF_MODE pass_mode)
        {
            switch (pass_mode)
            {
                case J_BUF_MODE.JBUF_PASS_THRU:
                    if (m_cinfo.m_quantize_colors)
                    {
                        m_processor = ProcessorType.OnePass;
                        if (m_buffer == null)
                            m_buffer = m_whole_image.Access(0, m_strip_height);
                    }
                    else
                    {
                        m_processor = ProcessorType.Upsample;
                    }
                    break;
                case J_BUF_MODE.JBUF_SAVE_AND_PASS:
                    m_processor = ProcessorType.PrePass;
                    break;
                case J_BUF_MODE.JBUF_CRANK_DEST:
                    m_processor = ProcessorType.SecondPass;
                    break;
                default:
                    break;
            }
            m_starting_row = m_next_row = 0;
        }

        public void post_process_data(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            switch (m_processor)
            {
                case ProcessorType.OnePass:
                    post_process_1pass(input_buf, ref in_row_group_ctr, in_row_groups_avail, output_buf, ref out_row_ctr, out_rows_avail);
                    break;
                case ProcessorType.PrePass:
                    post_process_prepass(input_buf, ref in_row_group_ctr, in_row_groups_avail, ref out_row_ctr);
                    break;
                case ProcessorType.Upsample:
                    m_cinfo.m_upsample.upsample(input_buf, ref in_row_group_ctr, in_row_groups_avail, output_buf, ref out_row_ctr, out_rows_avail);
                    break;
                case ProcessorType.SecondPass:
                    post_process_2pass(output_buf, ref out_row_ctr, out_rows_avail);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Process some data in the one-pass (strip buffer) case.
        /// This is used for color precision reduction as well as one-pass quantization.
        /// </summary>
        private void post_process_1pass(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            int max_rows = out_rows_avail - out_row_ctr;
            if (max_rows > m_strip_height)
                max_rows = m_strip_height;

            int num_rows = 0;
            m_cinfo.m_upsample.upsample(input_buf, ref in_row_group_ctr, in_row_groups_avail, m_buffer, ref num_rows, max_rows);

            m_cinfo.m_cquantize.color_quantize(m_buffer, 0, output_buf, out_row_ctr, num_rows);
            out_row_ctr += num_rows;
        }

        /// <summary>
        /// Process some data in the first pass of 2-pass quantization.
        /// </summary>
        private void post_process_prepass(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, ref int out_row_ctr)
        {
            int old_next_row, num_rows;

            /* Reposition virtual buffer if at start of strip. */
            if (m_next_row == 0)
                m_buffer = m_whole_image.Access(m_starting_row, m_strip_height);

            old_next_row = m_next_row;
            m_cinfo.m_upsample.upsample(input_buf, ref in_row_group_ctr, in_row_groups_avail, m_buffer, ref m_next_row, m_strip_height);

            if (m_next_row > old_next_row)
            {
                num_rows = m_next_row - old_next_row;
                m_cinfo.m_cquantize.color_quantize(m_buffer, old_next_row, null, 0, num_rows);
                out_row_ctr += num_rows;
            }

            if (m_next_row >= m_strip_height)
            {
                m_starting_row += m_strip_height;
                m_next_row = 0;
            }
        }

        /// <summary>
        /// Process some data in the second pass of 2-pass quantization.
        /// </summary>
        private void post_process_2pass(byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            int num_rows, max_rows;

            if (m_next_row == 0)
                m_buffer = m_whole_image.Access(m_starting_row, m_strip_height);

            num_rows = m_strip_height - m_next_row;
            max_rows = out_rows_avail - out_row_ctr;
            if (num_rows > max_rows)
                num_rows = max_rows;

            max_rows = m_cinfo.m_output_height - m_starting_row;
            if (num_rows > max_rows)
                num_rows = max_rows;

            /* Quantize and emit data. */
            m_cinfo.m_cquantize.color_quantize(m_buffer, m_next_row, output_buf, out_row_ctr, num_rows);
            out_row_ctr += num_rows;

            /* Advance if we filled the strip. */
            m_next_row += num_rows;
            if (m_next_row >= m_strip_height)
            {
                m_starting_row += m_strip_height;
                m_next_row = 0;
            }
        }
    }

    /// <summary>
    /// Master control module
    /// </summary>
    class DecompMaster
    {
        private DecompressStruct m_cinfo;

        private int m_pass_number;
        private bool m_is_dummy_pass;
        private bool m_using_merged_upsample;
        private ColorQuantizer m_quantizer_1pass;
        private ColorQuantizer m_quantizer_2pass;

        public DecompMaster(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            master_selection();
        }

        /// <summary>
        /// Per-pass setup.
        /// </summary>
        public void prepare_for_output_pass()
        {
            if (m_is_dummy_pass)
            {
                m_is_dummy_pass = false;
                m_cinfo.m_cquantize.start_pass(false);
                m_cinfo.m_post.start_pass(J_BUF_MODE.JBUF_CRANK_DEST);
                m_cinfo.m_main.start_pass(J_BUF_MODE.JBUF_CRANK_DEST);
            }
            else
            {
                if (m_cinfo.m_quantize_colors && m_cinfo.m_colormap == null)
                {
                    /* Select new quantization method */
                    if (m_cinfo.m_two_pass_quantize && m_cinfo.m_enable_2pass_quant)
                    {
                        m_cinfo.m_cquantize = m_quantizer_2pass;
                        m_is_dummy_pass = true;
                    }
                    else if (m_cinfo.m_enable_1pass_quant)
                        m_cinfo.m_cquantize = m_quantizer_1pass;
                }

                m_cinfo.m_idct.start_pass();
                m_cinfo.m_coef.start_output_pass();

                if (!m_cinfo.m_raw_data_out)
                {
                    m_cinfo.m_upsample.start_pass();

                    if (m_cinfo.m_quantize_colors)
                        m_cinfo.m_cquantize.start_pass(m_is_dummy_pass);

                    m_cinfo.m_post.start_pass((m_is_dummy_pass ? J_BUF_MODE.JBUF_SAVE_AND_PASS : J_BUF_MODE.JBUF_PASS_THRU));
                    m_cinfo.m_main.start_pass(J_BUF_MODE.JBUF_PASS_THRU);
                }
            }

            if (m_cinfo.m_progress != null)
            {
                m_cinfo.m_progress.Completed_passes = m_pass_number;
                m_cinfo.m_progress.Total_passes = m_pass_number + (m_is_dummy_pass ? 2 : 1);

                if (m_cinfo.m_buffered_image && !m_cinfo.m_inputctl.EOIReached())
                    m_cinfo.m_progress.Total_passes += (m_cinfo.m_enable_2pass_quant ? 2 : 1);
            }
        }

        /// <summary>
        /// Finish up at end of an output pass.
        /// </summary>
        public void finish_output_pass()
        {
            if (m_cinfo.m_quantize_colors)
                m_cinfo.m_cquantize.finish_pass();

            m_pass_number++;
        }

        public bool IsDummyPass()
        {
            return m_is_dummy_pass;
        }

        /// <summary>
        /// Master selection of decompression modules.
        /// </summary>
        private void master_selection()
        {
            m_cinfo.jpeg_calc_output_dimensions();
            prepare_range_limit_table();

            long samplesperrow = m_cinfo.m_output_width * m_cinfo.m_out_color_components;
            int jd_samplesperrow = (int)samplesperrow;

            m_pass_number = 0;
            m_using_merged_upsample = m_cinfo.use_merged_upsample();

            m_quantizer_1pass = null;
            m_quantizer_2pass = null;

            if (!m_cinfo.m_quantize_colors || !m_cinfo.m_buffered_image)
            {
                m_cinfo.m_enable_1pass_quant = false;
                m_cinfo.m_enable_external_quant = false;
                m_cinfo.m_enable_2pass_quant = false;
            }

            if (m_cinfo.m_quantize_colors)
            {
                if (m_cinfo.m_out_color_components != 3)
                {
                    m_cinfo.m_enable_1pass_quant = true;
                    m_cinfo.m_enable_external_quant = false;
                    m_cinfo.m_enable_2pass_quant = false;
                    m_cinfo.m_colormap = null;
                }
                else if (m_cinfo.m_colormap != null)
                    m_cinfo.m_enable_external_quant = true;
                else if (m_cinfo.m_two_pass_quantize)
                    m_cinfo.m_enable_2pass_quant = true;
                else
                    m_cinfo.m_enable_1pass_quant = true;

                if (m_cinfo.m_enable_1pass_quant)
                {
                    m_cinfo.m_cquantize = new Ex1PassCQuantizer(m_cinfo);
                    m_quantizer_1pass = m_cinfo.m_cquantize;
                }

                if (m_cinfo.m_enable_2pass_quant || m_cinfo.m_enable_external_quant)
                {
                    m_cinfo.m_cquantize = new Ex2PassCQuantizer(m_cinfo);
                    m_quantizer_2pass = m_cinfo.m_cquantize;
                }
            }

            if (!m_cinfo.m_raw_data_out)
            {
                if (m_using_merged_upsample)
                {
                    m_cinfo.m_upsample = new ExMergedUpsampler(m_cinfo);
                }
                else
                {
                    m_cinfo.m_cconvert = new ColorDeconverter(m_cinfo);
                    m_cinfo.m_upsample = new ExUpsampler(m_cinfo);
                }

                m_cinfo.m_post = new DPostController(m_cinfo, m_cinfo.m_enable_2pass_quant);
            }

            /* Inverse DCT */
            m_cinfo.m_idct = new InverseDCT(m_cinfo);

            if (m_cinfo.m_progressive_mode)
                m_cinfo.m_entropy = new PHuffEntropyDecoder(m_cinfo);
            else
                m_cinfo.m_entropy = new HuffEntropyDecoder(m_cinfo);

            /* Initialize principal buffer controllers. */
            bool use_c_buffer = m_cinfo.m_inputctl.HasMultipleScans() || m_cinfo.m_buffered_image;
            m_cinfo.m_coef = new DCoefController(m_cinfo, use_c_buffer);

            if (!m_cinfo.m_raw_data_out)
                m_cinfo.m_main = new DMainController(m_cinfo);

            /* Initialize input side of decompressor to consume first scan. */
            m_cinfo.m_inputctl.start_input_pass();

            if (m_cinfo.m_progress != null && !m_cinfo.m_buffered_image && m_cinfo.m_inputctl.HasMultipleScans())
            {
                int nscans;
                if (m_cinfo.m_progressive_mode)
                {
                    nscans = 2 + 3 * m_cinfo.m_num_components;
                }
                else
                {
                    nscans = m_cinfo.m_num_components;
                }

                m_cinfo.m_progress.Pass_counter = 0;
                m_cinfo.m_progress.Pass_limit = m_cinfo.m_total_iMCU_rows * nscans;
                m_cinfo.m_progress.Completed_passes = 0;
                m_cinfo.m_progress.Total_passes = (m_cinfo.m_enable_2pass_quant ? 3 : 2);

                m_pass_number++;
            }
        }

        /// <summary>
        /// Allocate and fill in the sample_range_limit table.
        /// </summary>
        private void prepare_range_limit_table()
        {
            byte[] table = new byte[5 * (JpegConstants.MAXJSAMPLE + 1) + JpegConstants.CENTERJSAMPLE];

            /* allow negative subscripts of simple table */
            int tableOffset = JpegConstants.MAXJSAMPLE + 1;
            m_cinfo.m_sample_range_limit = table;
            m_cinfo.m_sampleRangeLimitOffset = tableOffset;

            Array.Clear(table, 0, JpegConstants.MAXJSAMPLE + 1);

            for (int i = 0; i <= JpegConstants.MAXJSAMPLE; i++)
                table[tableOffset + i] = (byte)i;

            tableOffset += JpegConstants.CENTERJSAMPLE; /* Point to where post-IDCT table starts */

            for (int i = JpegConstants.CENTERJSAMPLE; i < 2 * (JpegConstants.MAXJSAMPLE + 1); i++)
                table[tableOffset + i] = JpegConstants.MAXJSAMPLE;

            Array.Clear(table, tableOffset + 2 * (JpegConstants.MAXJSAMPLE + 1),
                2 * (JpegConstants.MAXJSAMPLE + 1) - JpegConstants.CENTERJSAMPLE);

            Buffer.BlockCopy(m_cinfo.m_sample_range_limit, 0, table,
                tableOffset + 4 * (JpegConstants.MAXJSAMPLE + 1) - JpegConstants.CENTERJSAMPLE, JpegConstants.CENTERJSAMPLE);
        }
    }

    /// <summary>
    /// Entropy decoding
    /// </summary>
    abstract class EntropyDecoder
    {
        private static readonly int[] extend_test = 
        { 
            0, 0x0001, 0x0002, 0x0004, 0x0008, 0x0010, 0x0020, 
            0x0040, 0x0080, 0x0100, 0x0200, 0x0400, 0x0800, 
            0x1000, 0x2000, 0x4000 
        };

        private static readonly int[] extend_offset = 
        { 
            0, (-1 << 1) + 1, (-1 << 2) + 1, 
            (-1 << 3) + 1, (-1 << 4) + 1, (-1 << 5) + 1,
            (-1 << 6) + 1, (-1 << 7) + 1, (-1 << 8) + 1,
            (-1 << 9) + 1, (-1 << 10) + 1,
            (-1 << 11) + 1, (-1 << 12) + 1,
            (-1 << 13) + 1, (-1 << 14) + 1,
            (-1 << 15) + 1 
        };

        protected const int BIT_BUF_SIZE = 32;
        protected const int MIN_GET_BITS = BIT_BUF_SIZE - 7;
        protected DecompressStruct m_cinfo;
        protected bool m_insufficient_data; /* set true after emitting warning */
        public abstract void start_pass();
        public abstract bool decode_mcu(JBLOCK[] MCU_data);
        protected static int HUFF_EXTEND(int x, int s)
        {
            return ((x) < extend_test[s] ? (x) + extend_offset[s] : (x));
        }

        protected void BITREAD_LOAD_STATE(BitReadPermState bitstate, out int get_buffer, out int bits_left, ref BitReadWorkingState br_state)
        {
            br_state.cinfo = m_cinfo;
            get_buffer = bitstate.get_buffer;
            bits_left = bitstate.bits_left;
        }

        protected static void BITREAD_SAVE_STATE(ref BitReadPermState bitstate, int get_buffer, int bits_left)
        {
            bitstate.get_buffer = get_buffer;
            bitstate.bits_left = bits_left;
        }

        /// <summary>
        /// Expand a Huffman table definition into the derived format
        /// </summary>
        protected void jpeg_make_d_derived_tbl(bool isDC, int tblno, ref DDerivedTbl dtbl)
        {
            JHUFF_TBL htbl = isDC ? m_cinfo.m_dc_huff_tbl_ptrs[tblno] : m_cinfo.m_ac_huff_tbl_ptrs[tblno];

            if (dtbl == null)
                dtbl = new DDerivedTbl();

            dtbl.pub = htbl;       /* fill in back link */

            int p = 0;
            char[] huffsize = new char[257];
            for (int l = 1; l <= 16; l++)
            {
                int i = htbl.Bits[l];

                while ((i--) != 0)
                    huffsize[p++] = (char)l;
            }
            huffsize[p] = (char)0;
            int numsymbols = p;

            int code = 0;
            int si = huffsize[0];
            int[] huffcode = new int[257];
            p = 0;
            while (huffsize[p] != 0)
            {
                while (((int)huffsize[p]) == si)
                {
                    huffcode[p++] = code;
                    code++;
                }
                code <<= 1;
                si++;
            }

            p = 0;
            for (int l = 1; l <= 16; l++)
            {
                if (htbl.Bits[l] != 0)
                {
                    dtbl.valoffset[l] = p - huffcode[p];
                    p += htbl.Bits[l];
                    dtbl.maxcode[l] = huffcode[p - 1]; /* maximum code of length l */
                }
                else
                {
                    dtbl.maxcode[l] = -1;
                }
            }
            dtbl.maxcode[17] = 0xFFFFF; /* ensures jpeg_huff_decode terminates */

            Array.Clear(dtbl.look_nbits, 0, dtbl.look_nbits.Length);
            p = 0;
            for (int l = 1; l <= JpegConstants.HUFF_LOOKAHEAD; l++)
            {
                for (int i = 1; i <= htbl.Bits[l]; i++, p++)
                {
                    int lookbits = huffcode[p] << (JpegConstants.HUFF_LOOKAHEAD - l);
                    for (int ctr = 1 << (JpegConstants.HUFF_LOOKAHEAD - l); ctr > 0; ctr--)
                    {
                        dtbl.look_nbits[lookbits] = l;
                        dtbl.look_sym[lookbits] = htbl.Huffval[p];
                        lookbits++;
                    }
                }
            }

            if (isDC)
            {
                for (int i = 0; i < numsymbols; i++)
                {
                    int sym = htbl.Huffval[i];
                }
            }
        }

        protected static bool CHECK_BIT_BUFFER(ref BitReadWorkingState state, int nbits, ref int get_buffer, ref int bits_left)
        {
            if (bits_left < nbits)
            {
                if (!jpeg_fill_bit_buffer(ref state, get_buffer, bits_left, nbits))
                    return false;

                get_buffer = state.get_buffer;
                bits_left = state.bits_left;
            }

            return true;
        }

        protected static int GET_BITS(int nbits, int get_buffer, ref int bits_left)
        {
            return (((int)(get_buffer >> (bits_left -= nbits))) & ((1 << nbits) - 1));
        }

        protected static int PEEK_BITS(int nbits, int get_buffer, int bits_left)
        {
            return (((int)(get_buffer >> (bits_left - nbits))) & ((1 << nbits) - 1));
        }

        protected static void DROP_BITS(int nbits, ref int bits_left)
        {
            bits_left -= nbits;
        }

        protected static bool jpeg_fill_bit_buffer(ref BitReadWorkingState state, int get_buffer, int bits_left, int nbits)
        {
            bool noMoreBytes = false;

            if (state.cinfo.m_unread_marker == 0)
            {
                while (bits_left < MIN_GET_BITS)
                {
                    int c;
                    state.cinfo.m_src.GetByte(out c);

                    if (c == 0xFF)
                    {
                        do
                        {
                            state.cinfo.m_src.GetByte(out c);
                        }
                        while (c == 0xFF);

                        if (c == 0)
                        {
                            c = 0xFF;
                        }
                        else
                        {
                            state.cinfo.m_unread_marker = c;
                            noMoreBytes = true;
                            break;
                        }
                    }

                    get_buffer = (get_buffer << 8) | c;
                    bits_left += 8;
                }
            }
            else
                noMoreBytes = true;

            if (noMoreBytes)
            {
                if (nbits > bits_left)
                {
                    if (!state.cinfo.m_entropy.m_insufficient_data)
                    {
                        state.cinfo.m_entropy.m_insufficient_data = true;
                    }

                    get_buffer <<= MIN_GET_BITS - bits_left;
                    bits_left = MIN_GET_BITS;
                }
            }

            state.get_buffer = get_buffer;
            state.bits_left = bits_left;

            return true;
        }

        protected static bool HUFF_DECODE(out int result, ref BitReadWorkingState state, DDerivedTbl htbl, ref int get_buffer, ref int bits_left)
        {
            int nb = 0;
            bool doSlow = false;

            if (bits_left < JpegConstants.HUFF_LOOKAHEAD)
            {
                if (!jpeg_fill_bit_buffer(ref state, get_buffer, bits_left, 0))
                {
                    result = -1;
                    return false;
                }

                get_buffer = state.get_buffer;
                bits_left = state.bits_left;
                if (bits_left < JpegConstants.HUFF_LOOKAHEAD)
                {
                    nb = 1;
                    doSlow = true;
                }
            }

            if (!doSlow)
            {
                int look = PEEK_BITS(JpegConstants.HUFF_LOOKAHEAD, get_buffer, bits_left);
                if ((nb = htbl.look_nbits[look]) != 0)
                {
                    DROP_BITS(nb, ref bits_left);
                    result = htbl.look_sym[look];
                    return true;
                }

                nb = JpegConstants.HUFF_LOOKAHEAD + 1;
            }

            result = jpeg_huff_decode(ref state, get_buffer, bits_left, htbl, nb);
            if (result < 0)
                return false;

            get_buffer = state.get_buffer;
            bits_left = state.bits_left;

            return true;
        }

        protected static int jpeg_huff_decode(ref BitReadWorkingState state, int get_buffer, int bits_left, DDerivedTbl htbl, int min_bits)
        {
            int l = min_bits;
            if (!CHECK_BIT_BUFFER(ref state, l, ref get_buffer, ref bits_left))
                return -1;

            int code = GET_BITS(l, get_buffer, ref bits_left);

            while (code > htbl.maxcode[l])
            {
                code <<= 1;
                if (!CHECK_BIT_BUFFER(ref state, 1, ref get_buffer, ref bits_left))
                    return -1;

                code |= GET_BITS(1, get_buffer, ref bits_left);
                l++;
            }

            state.get_buffer = get_buffer;
            state.bits_left = bits_left;

            if (l > 16)
            {
                return 0;
            }

            return htbl.pub.Huffval[code + htbl.valoffset[l]];
        }
    }

    /// <summary>
    /// Input control module
    /// </summary>
    class InputController
    {
        private DecompressStruct m_cinfo;
        private bool m_consumeData;
        private bool m_inheaders;
        private bool m_has_multiple_scans;
        private bool m_eoi_reached;

        /// <summary>
        /// Initialize the input controller module.
        /// </summary>
        public InputController(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            m_inheaders = true;
        }

        public ReadResult consume_input()
        {
            if (m_consumeData)
                return m_cinfo.m_coef.consume_data();

            return consume_markers();
        }

        /// <summary>
        /// Reset state to begin a fresh datastream.
        /// </summary>
        public void reset_input_controller()
        {
            m_consumeData = false;
            m_has_multiple_scans = false;
            m_eoi_reached = false;
            m_inheaders = true;

            m_cinfo.m_marker.reset_marker_reader();

            m_cinfo.m_coef_bits = null;
        }

        /// <summary>
        /// Initialize the input modules to read a scan of compressed data.
        /// </summary>
        public void start_input_pass()
        {
            per_scan_setup();
            latch_quant_tables();
            m_cinfo.m_entropy.start_pass();
            m_cinfo.m_coef.start_input_pass();
            m_consumeData = true;
        }

        /// <summary>
        /// Finish up after inputting a compressed-data scan.
        /// </summary>
        public void finish_input_pass()
        {
            m_consumeData = false;
        }

        public bool HasMultipleScans()
        {
            return m_has_multiple_scans;
        }

        public bool EOIReached()
        {
            return m_eoi_reached;
        }

        /// <summary>
        /// Read JPEG markers before, between, or after compressed-data scans.
        /// </summary>
        private ReadResult consume_markers()
        {
            ReadResult val;

            if (m_eoi_reached)
                return ReadResult.JPEG_REACHED_EOI;

            val = m_cinfo.m_marker.read_markers();

            switch (val)
            {
                case ReadResult.JPEG_REACHED_SOS:
                    if (m_inheaders)
                    {
                        initial_setup();
                        m_inheaders = false;
                    }
                    else
                    {
                        if (!m_has_multiple_scans)
                        {
                        }

                        m_cinfo.m_inputctl.start_input_pass();
                    }
                    break;
                case ReadResult.JPEG_REACHED_EOI:
                    /* Found EOI */
                    m_eoi_reached = true;
                    if (m_inheaders)
                    {
                        if (m_cinfo.m_marker.SawSOF())
                        { }
                    }
                    else
                    {
                        if (m_cinfo.m_output_scan_number > m_cinfo.m_input_scan_number)
                            m_cinfo.m_output_scan_number = m_cinfo.m_input_scan_number;
                    }
                    break;
                case ReadResult.JPEG_SUSPENDED:
                    break;
            }

            return val;
        }

        /// <summary>
        /// Routines to calculate various quantities related to the size of the image.
        /// </summary>
        private void initial_setup()
        {
            if (m_cinfo.m_image_height > JpegConstants.JPEG_MAX_DIMENSION ||
                m_cinfo.m_image_width > JpegConstants.JPEG_MAX_DIMENSION)
            {

            }

            m_cinfo.m_max_h_samp_factor = 1;
            m_cinfo.m_max_v_samp_factor = 1;

            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                if (m_cinfo.Comp_info[ci].H_samp_factor <= 0 || m_cinfo.Comp_info[ci].H_samp_factor > JpegConstants.MAX_SAMP_FACTOR ||
                    m_cinfo.Comp_info[ci].V_samp_factor <= 0 || m_cinfo.Comp_info[ci].V_samp_factor > JpegConstants.MAX_SAMP_FACTOR)
                {
                }

                m_cinfo.m_max_h_samp_factor = Math.Max(m_cinfo.m_max_h_samp_factor, m_cinfo.Comp_info[ci].H_samp_factor);
                m_cinfo.m_max_v_samp_factor = Math.Max(m_cinfo.m_max_v_samp_factor, m_cinfo.Comp_info[ci].V_samp_factor);
            }

            m_cinfo.m_min_DCT_scaled_size = JpegConstants.DCTSIZE;

            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                m_cinfo.Comp_info[ci].DCT_scaled_size = JpegConstants.DCTSIZE;

                m_cinfo.Comp_info[ci].Width_in_blocks = JpegUtils.jdiv_round_up(
                    m_cinfo.m_image_width * m_cinfo.Comp_info[ci].H_samp_factor,
                    m_cinfo.m_max_h_samp_factor * JpegConstants.DCTSIZE);

                m_cinfo.Comp_info[ci].height_in_blocks = JpegUtils.jdiv_round_up(
                    m_cinfo.m_image_height * m_cinfo.Comp_info[ci].V_samp_factor,
                    m_cinfo.m_max_v_samp_factor * JpegConstants.DCTSIZE);

                m_cinfo.Comp_info[ci].downsampled_width = JpegUtils.jdiv_round_up(
                    m_cinfo.m_image_width * m_cinfo.Comp_info[ci].H_samp_factor,
                    m_cinfo.m_max_h_samp_factor);

                m_cinfo.Comp_info[ci].downsampled_height = JpegUtils.jdiv_round_up(
                    m_cinfo.m_image_height * m_cinfo.Comp_info[ci].V_samp_factor,
                    m_cinfo.m_max_v_samp_factor);

                m_cinfo.Comp_info[ci].component_needed = true;

                m_cinfo.Comp_info[ci].quant_table = null;
            }

            m_cinfo.m_total_iMCU_rows = JpegUtils.jdiv_round_up(
                m_cinfo.m_image_height, m_cinfo.m_max_v_samp_factor * JpegConstants.DCTSIZE);

            if (m_cinfo.m_comps_in_scan < m_cinfo.m_num_components || m_cinfo.m_progressive_mode)
                m_cinfo.m_inputctl.m_has_multiple_scans = true;
            else
                m_cinfo.m_inputctl.m_has_multiple_scans = false;
        }

        /// <summary>
        /// Save away a copy of the Q-table referenced by each component present
        /// in the current scan, unless already saved during a prior scan.
        /// </summary>
        private void latch_quant_tables()
        {
            for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
            {
                ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]];

                if (componentInfo.quant_table != null)
                    continue;

                int qtblno = componentInfo.Quant_tbl_no;

                JQUANT_TBL qtbl = new JQUANT_TBL();
                Buffer.BlockCopy(m_cinfo.m_quant_tbl_ptrs[qtblno].quantval, 0,
                    qtbl.quantval, 0, qtbl.quantval.Length * sizeof(short));
                qtbl.Sent_table = m_cinfo.m_quant_tbl_ptrs[qtblno].Sent_table;
                componentInfo.quant_table = qtbl;
                m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]] = componentInfo;
            }
        }

        /// <summary>
        /// Do computations that are needed before processing a JPEG scan
        /// cinfo.comps_in_scan and cinfo.cur_comp_info[] were set from SOS marker
        /// </summary>
        private void per_scan_setup()
        {
            if (m_cinfo.m_comps_in_scan == 1)
            {
                /* Noninterleaved (single-component) scan */
                ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[0]];

                /* Overall image size in MCUs */
                m_cinfo.m_MCUs_per_row = componentInfo.Width_in_blocks;
                m_cinfo.m_MCU_rows_in_scan = componentInfo.height_in_blocks;

                /* For noninterleaved scan, always one block per MCU */
                componentInfo.MCU_width = 1;
                componentInfo.MCU_height = 1;
                componentInfo.MCU_blocks = 1;
                componentInfo.MCU_sample_width = componentInfo.DCT_scaled_size;
                componentInfo.last_col_width = 1;

                int tmp = componentInfo.height_in_blocks % componentInfo.V_samp_factor;
                if (tmp == 0)
                    tmp = componentInfo.V_samp_factor;
                componentInfo.last_row_height = tmp;
                m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[0]] = componentInfo;

                /* Prepare array describing MCU composition */
                m_cinfo.m_blocks_in_MCU = 1;
                m_cinfo.m_MCU_membership[0] = 0;
            }
            else
            {
                m_cinfo.m_MCUs_per_row = JpegUtils.jdiv_round_up(
                    m_cinfo.m_image_width, m_cinfo.m_max_h_samp_factor * JpegConstants.DCTSIZE);

                m_cinfo.m_MCU_rows_in_scan = JpegUtils.jdiv_round_up(
                    m_cinfo.m_image_height, m_cinfo.m_max_v_samp_factor * JpegConstants.DCTSIZE);

                m_cinfo.m_blocks_in_MCU = 0;

                for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
                {
                    ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]];

                    /* Sampling factors give # of blocks of component in each MCU */
                    componentInfo.MCU_width = componentInfo.H_samp_factor;
                    componentInfo.MCU_height = componentInfo.V_samp_factor;
                    componentInfo.MCU_blocks = componentInfo.MCU_width * componentInfo.MCU_height;
                    componentInfo.MCU_sample_width = componentInfo.MCU_width * componentInfo.DCT_scaled_size;

                    /* Figure number of non-dummy blocks in last MCU column & row */
                    int tmp = componentInfo.Width_in_blocks % componentInfo.MCU_width;
                    if (tmp == 0)
                        tmp = componentInfo.MCU_width;
                    componentInfo.last_col_width = tmp;

                    tmp = componentInfo.height_in_blocks % componentInfo.MCU_height;
                    if (tmp == 0)
                        tmp = componentInfo.MCU_height;
                    componentInfo.last_row_height = tmp;

                    /* Prepare array describing MCU composition */
                    int mcublks = componentInfo.MCU_blocks;

                    m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]] = componentInfo;

                    while (mcublks-- > 0)
                        m_cinfo.m_MCU_membership[m_cinfo.m_blocks_in_MCU++] = ci;
                }
            }
        }
    }

    /// <summary>
    /// </summary>
    class InverseDCT
    {
        private const int IFAST_SCALE_BITS = 2;
        private const int RANGE_MASK = (JpegConstants.MAXJSAMPLE * 4 + 3); /* 2 bits wider than legal samples */
        private const int SLOW_INTEGER_CONST_BITS = 13;
        private const int SLOW_INTEGER_PASS1_BITS = 2;
        private const int SLOW_INTEGER_FIX_0_298631336 = 2446;   /* SLOW_INTEGER_FIX(0.298631336) */
        private const int SLOW_INTEGER_FIX_0_390180644 = 3196;   /* SLOW_INTEGER_FIX(0.390180644) */
        private const int SLOW_INTEGER_FIX_0_541196100 = 4433;   /* SLOW_INTEGER_FIX(0.541196100) */
        private const int SLOW_INTEGER_FIX_0_765366865 = 6270;   /* SLOW_INTEGER_FIX(0.765366865) */
        private const int SLOW_INTEGER_FIX_0_899976223 = 7373;   /* SLOW_INTEGER_FIX(0.899976223) */
        private const int SLOW_INTEGER_FIX_1_175875602 = 9633;   /* SLOW_INTEGER_FIX(1.175875602) */
        private const int SLOW_INTEGER_FIX_1_501321110 = 12299;  /* SLOW_INTEGER_FIX(1.501321110) */
        private const int SLOW_INTEGER_FIX_1_847759065 = 15137;  /* SLOW_INTEGER_FIX(1.847759065) */
        private const int SLOW_INTEGER_FIX_1_961570560 = 16069;  /* SLOW_INTEGER_FIX(1.961570560) */
        private const int SLOW_INTEGER_FIX_2_053119869 = 16819;  /* SLOW_INTEGER_FIX(2.053119869) */
        private const int SLOW_INTEGER_FIX_2_562915447 = 20995;  /* SLOW_INTEGER_FIX(2.562915447) */
        private const int SLOW_INTEGER_FIX_3_072711026 = 25172;  /* SLOW_INTEGER_FIX(3.072711026) */
        private const int FAST_INTEGER_CONST_BITS = 8;
        private const int FAST_INTEGER_PASS1_BITS = 2;
        private const int FAST_INTEGER_FIX_1_082392200 = 277;        /* FAST_INTEGER_FIX(1.082392200) */
        private const int FAST_INTEGER_FIX_1_414213562 = 362;        /* FAST_INTEGER_FIX(1.414213562) */
        private const int FAST_INTEGER_FIX_1_847759065 = 473;        /* FAST_INTEGER_FIX(1.847759065) */
        private const int FAST_INTEGER_FIX_2_613125930 = 669;        /* FAST_INTEGER_FIX(2.613125930) */

        private const int REDUCED_CONST_BITS = 13;
        private const int REDUCED_PASS1_BITS = 2;
        private const int REDUCED_FIX_0_211164243 = 1730;    /* REDUCED_FIX(0.211164243) */
        private const int REDUCED_FIX_0_509795579 = 4176;    /* REDUCED_FIX(0.509795579) */
        private const int REDUCED_FIX_0_601344887 = 4926;    /* REDUCED_FIX(0.601344887) */
        private const int REDUCED_FIX_0_720959822 = 5906;    /* REDUCED_FIX(0.720959822) */
        private const int REDUCED_FIX_0_765366865 = 6270;    /* REDUCED_FIX(0.765366865) */
        private const int REDUCED_FIX_0_850430095 = 6967;    /* REDUCED_FIX(0.850430095) */
        private const int REDUCED_FIX_0_899976223 = 7373;    /* REDUCED_FIX(0.899976223) */
        private const int REDUCED_FIX_1_061594337 = 8697;    /* REDUCED_FIX(1.061594337) */
        private const int REDUCED_FIX_1_272758580 = 10426;   /* REDUCED_FIX(1.272758580) */
        private const int REDUCED_FIX_1_451774981 = 11893;   /* REDUCED_FIX(1.451774981) */
        private const int REDUCED_FIX_1_847759065 = 15137;   /* REDUCED_FIX(1.847759065) */
        private const int REDUCED_FIX_2_172734803 = 17799;   /* REDUCED_FIX(2.172734803) */
        private const int REDUCED_FIX_2_562915447 = 20995;   /* REDUCED_FIX(2.562915447) */
        private const int REDUCED_FIX_3_624509785 = 29692;   /* REDUCED_FIX(3.624509785) */

        private static readonly short[] aanscales = 
        {
            16384, 22725, 21407, 19266, 16384, 12873, 8867, 4520, 22725, 31521, 29692, 26722, 22725, 17855,
            12299, 6270, 21407, 29692, 27969, 25172, 21407, 16819, 11585,
            5906, 19266, 26722, 25172, 22654, 19266, 15137, 10426, 5315,
            16384, 22725, 21407, 19266, 16384, 12873, 8867, 4520, 12873,
            17855, 16819, 15137, 12873, 10114, 6967, 3552, 8867, 12299,
            11585, 10426, 8867, 6967, 4799, 2446, 4520, 6270, 5906, 5315,
            4520, 3552, 2446, 1247 
        };

        private const int CONST_BITS = 14;

        private static readonly double[] aanscalefactor = 
        { 
            1.0, 1.387039845, 1.306562965, 1.175875602, 1.0,
            0.785694958, 0.541196100, 0.275899379 
        };

        private enum InverseMethod
        {
            Unknown,
            idct_1x1_method,
            idct_2x2_method,
            idct_4x4_method,
            idct_islow_method,
            idct_ifast_method,
            idct_float_method
        }

        private InverseMethod[] m_inverse_DCT_method = new InverseMethod[JpegConstants.MAX_COMPONENTS];
        private class multiplier_table
        {
            public int[] int_array = new int[JpegConstants.DCTSIZE2];
            public float[] float_array = new float[JpegConstants.DCTSIZE2];
        };

        private multiplier_table[] m_dctTables;
        private DecompressStruct m_cinfo;
        private int[] m_cur_method = new int[JpegConstants.MAX_COMPONENTS];
        private ComponentBuffer m_componentBuffer;

        public InverseDCT(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;

            m_dctTables = new multiplier_table[cinfo.m_num_components];
            for (int ci = 0; ci < cinfo.m_num_components; ci++)
            {
                m_dctTables[ci] = new multiplier_table();
                m_cur_method[ci] = -1;
            }
        }

        /// <summary>
        /// Prepare for an output pass.
        /// </summary>
        public void start_pass()
        {
            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                ComponentInfo componentInfo = m_cinfo.Comp_info[ci];

                InverseMethod im = InverseMethod.Unknown;
                int method = 0;
                switch (componentInfo.DCT_scaled_size)
                {
                    case 1:
                        im = InverseMethod.idct_1x1_method;
                        method = (int)J_DCT_METHOD.JDCT_ISLOW;    /* jidctred uses islow-style table */
                        break;
                    case 2:
                        im = InverseMethod.idct_2x2_method;
                        method = (int)J_DCT_METHOD.JDCT_ISLOW;    /* jidctred uses islow-style table */
                        break;
                    case 4:
                        im = InverseMethod.idct_4x4_method;
                        method = (int)J_DCT_METHOD.JDCT_ISLOW;    /* jidctred uses islow-style table */
                        break;
                    case JpegConstants.DCTSIZE:
                        switch (m_cinfo.m_dct_method)
                        {
                            case J_DCT_METHOD.JDCT_ISLOW:
                                im = InverseMethod.idct_islow_method;
                                method = (int)J_DCT_METHOD.JDCT_ISLOW;
                                break;
                            case J_DCT_METHOD.JDCT_IFAST:
                                im = InverseMethod.idct_ifast_method;
                                method = (int)J_DCT_METHOD.JDCT_IFAST;
                                break;
                            case J_DCT_METHOD.JDCT_FLOAT:
                                im = InverseMethod.idct_float_method;
                                method = (int)J_DCT_METHOD.JDCT_FLOAT;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                m_inverse_DCT_method[ci] = im;

                if (!componentInfo.component_needed || m_cur_method[ci] == method)
                    continue;

                if (componentInfo.quant_table == null)
                {
                    continue;
                }

                m_cur_method[ci] = method;
                switch ((J_DCT_METHOD)method)
                {
                    case J_DCT_METHOD.JDCT_ISLOW:
                        int[] ismtbl = m_dctTables[ci].int_array;
                        for (int i = 0; i < JpegConstants.DCTSIZE2; i++)
                            ismtbl[i] = componentInfo.quant_table.quantval[i];
                        break;

                    case J_DCT_METHOD.JDCT_IFAST:
                        int[] ifmtbl = m_dctTables[ci].int_array;

                        for (int i = 0; i < JpegConstants.DCTSIZE2; i++)
                        {
                            ifmtbl[i] = JpegUtils.DESCALE((int)componentInfo.quant_table.quantval[i] * (int)aanscales[i], CONST_BITS - IFAST_SCALE_BITS);
                        }
                        break;

                    case J_DCT_METHOD.JDCT_FLOAT:
                        float[] fmtbl = m_dctTables[ci].float_array;
                        int ii = 0;
                        for (int row = 0; row < JpegConstants.DCTSIZE; row++)
                        {
                            for (int col = 0; col < JpegConstants.DCTSIZE; col++)
                            {
                                fmtbl[ii] = (float)((double)componentInfo.quant_table.quantval[ii] * aanscalefactor[row] * aanscalefactor[col]);
                                ii++;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public void inverse(int component_index, short[] coef_block, ComponentBuffer output_buf, int output_row, int output_col)
        {
            m_componentBuffer = output_buf;
            switch (m_inverse_DCT_method[component_index])
            {
                case InverseMethod.idct_1x1_method:
                    jpeg_idct_1x1(component_index, coef_block, output_row, output_col);
                    break;
                case InverseMethod.idct_2x2_method:
                    jpeg_idct_2x2(component_index, coef_block, output_row, output_col);
                    break;
                case InverseMethod.idct_4x4_method:
                    jpeg_idct_4x4(component_index, coef_block, output_row, output_col);
                    break;
                case InverseMethod.idct_islow_method:
                    jpeg_idct_islow(component_index, coef_block, output_row, output_col);
                    break;
                case InverseMethod.idct_ifast_method:
                    jpeg_idct_ifast(component_index, coef_block, output_row, output_col);
                    break;
                case InverseMethod.idct_float_method:
                    jpeg_idct_float(component_index, coef_block, output_row, output_col);
                    break;
                case InverseMethod.Unknown:
                default:
                    break;
            }
        }

        /// <summary>
        /// Perform dequantization and inverse DCT on one block of coefficients.
        /// </summary>
        private void jpeg_idct_islow(int component_index, short[] coef_block, int output_row, int output_col)
        {
            int[] workspace = new int[JpegConstants.DCTSIZE2];
            int coefBlockIndex = 0;
            int[] quantTable = m_dctTables[component_index].int_array;
            int quantTableIndex = 0;

            int workspaceIndex = 0;

            for (int ctr = JpegConstants.DCTSIZE; ctr > 0; ctr--)
            {
                if (coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 2] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 4] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 6] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7] == 0)
                {
                    int dcval = SLOW_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                        quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]) << SLOW_INTEGER_PASS1_BITS;

                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 2] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 3] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 4] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 5] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 6] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 7] = dcval;

                    coefBlockIndex++;
                    quantTableIndex++;
                    workspaceIndex++;
                    continue;
                }

                int z2 = SLOW_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 2],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 2]);
                int z3 = SLOW_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 6],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 6]);

                int z1 = (z2 + z3) * SLOW_INTEGER_FIX_0_541196100;
                int tmp2 = z1 + z3 * (-SLOW_INTEGER_FIX_1_847759065);
                int tmp3 = z1 + z2 * SLOW_INTEGER_FIX_0_765366865;

                z2 = SLOW_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]);
                z3 = SLOW_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 4],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 4]);

                int tmp0 = (z2 + z3) << SLOW_INTEGER_CONST_BITS;
                int tmp1 = (z2 - z3) << SLOW_INTEGER_CONST_BITS;

                int tmp10 = tmp0 + tmp3;
                int tmp13 = tmp0 - tmp3;
                int tmp11 = tmp1 + tmp2;
                int tmp12 = tmp1 - tmp2;

                tmp0 = SLOW_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 7]);
                tmp1 = SLOW_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 5]);
                tmp2 = SLOW_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 3]);
                tmp3 = SLOW_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 1]);

                z1 = tmp0 + tmp3;
                z2 = tmp1 + tmp2;
                z3 = tmp0 + tmp2;
                int z4 = tmp1 + tmp3;
                int z5 = (z3 + z4) * SLOW_INTEGER_FIX_1_175875602; /* sqrt(2) * c3 */

                tmp0 = tmp0 * SLOW_INTEGER_FIX_0_298631336; /* sqrt(2) * (-c1+c3+c5-c7) */
                tmp1 = tmp1 * SLOW_INTEGER_FIX_2_053119869; /* sqrt(2) * ( c1+c3-c5+c7) */
                tmp2 = tmp2 * SLOW_INTEGER_FIX_3_072711026; /* sqrt(2) * ( c1+c3+c5-c7) */
                tmp3 = tmp3 * SLOW_INTEGER_FIX_1_501321110; /* sqrt(2) * ( c1+c3-c5-c7) */
                z1 = z1 * (-SLOW_INTEGER_FIX_0_899976223); /* sqrt(2) * (c7-c3) */
                z2 = z2 * (-SLOW_INTEGER_FIX_2_562915447); /* sqrt(2) * (-c1-c3) */
                z3 = z3 * (-SLOW_INTEGER_FIX_1_961570560); /* sqrt(2) * (-c3-c5) */
                z4 = z4 * (-SLOW_INTEGER_FIX_0_390180644); /* sqrt(2) * (c5-c3) */

                z3 += z5;
                z4 += z5;

                tmp0 += z1 + z3;
                tmp1 += z2 + z4;
                tmp2 += z2 + z3;
                tmp3 += z1 + z4;

                workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = JpegUtils.DESCALE(tmp10 + tmp3, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 7] = JpegUtils.DESCALE(tmp10 - tmp3, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = JpegUtils.DESCALE(tmp11 + tmp2, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 6] = JpegUtils.DESCALE(tmp11 - tmp2, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 2] = JpegUtils.DESCALE(tmp12 + tmp1, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 5] = JpegUtils.DESCALE(tmp12 - tmp1, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 3] = JpegUtils.DESCALE(tmp13 + tmp0, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 4] = JpegUtils.DESCALE(tmp13 - tmp0, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);

                coefBlockIndex++;
                quantTableIndex++;
                workspaceIndex++;
            }

            workspaceIndex = 0;
            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset + JpegConstants.CENTERJSAMPLE;

            for (int ctr = 0; ctr < JpegConstants.DCTSIZE; ctr++)
            {
                int currentOutRow = output_row + ctr;
                if (workspace[workspaceIndex + 1] == 0 &&
                    workspace[workspaceIndex + 2] == 0 &&
                    workspace[workspaceIndex + 3] == 0 &&
                    workspace[workspaceIndex + 4] == 0 &&
                    workspace[workspaceIndex + 5] == 0 &&
                    workspace[workspaceIndex + 6] == 0 &&
                    workspace[workspaceIndex + 7] == 0)
                {
                    byte dcval = limit[limitOffset + JpegUtils.DESCALE(workspace[workspaceIndex + 0], SLOW_INTEGER_PASS1_BITS + 3) & RANGE_MASK];

                    m_componentBuffer[currentOutRow][output_col + 0] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 1] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 2] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 3] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 4] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 5] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 6] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 7] = dcval;

                    workspaceIndex += JpegConstants.DCTSIZE;       /* advance pointer to next row */
                    continue;
                }

                int z2 = workspace[workspaceIndex + 2];
                int z3 = workspace[workspaceIndex + 6];

                int z1 = (z2 + z3) * SLOW_INTEGER_FIX_0_541196100;
                int tmp2 = z1 + z3 * (-SLOW_INTEGER_FIX_1_847759065);
                int tmp3 = z1 + z2 * SLOW_INTEGER_FIX_0_765366865;

                int tmp0 = (workspace[workspaceIndex + 0] + workspace[workspaceIndex + 4]) << SLOW_INTEGER_CONST_BITS;
                int tmp1 = (workspace[workspaceIndex + 0] - workspace[workspaceIndex + 4]) << SLOW_INTEGER_CONST_BITS;

                int tmp10 = tmp0 + tmp3;
                int tmp13 = tmp0 - tmp3;
                int tmp11 = tmp1 + tmp2;
                int tmp12 = tmp1 - tmp2;

                tmp0 = workspace[workspaceIndex + 7];
                tmp1 = workspace[workspaceIndex + 5];
                tmp2 = workspace[workspaceIndex + 3];
                tmp3 = workspace[workspaceIndex + 1];

                z1 = tmp0 + tmp3;
                z2 = tmp1 + tmp2;
                z3 = tmp0 + tmp2;
                int z4 = tmp1 + tmp3;
                int z5 = (z3 + z4) * SLOW_INTEGER_FIX_1_175875602; /* sqrt(2) * c3 */

                tmp0 = tmp0 * SLOW_INTEGER_FIX_0_298631336; /* sqrt(2) * (-c1+c3+c5-c7) */
                tmp1 = tmp1 * SLOW_INTEGER_FIX_2_053119869; /* sqrt(2) * ( c1+c3-c5+c7) */
                tmp2 = tmp2 * SLOW_INTEGER_FIX_3_072711026; /* sqrt(2) * ( c1+c3+c5-c7) */
                tmp3 = tmp3 * SLOW_INTEGER_FIX_1_501321110; /* sqrt(2) * ( c1+c3-c5-c7) */
                z1 = z1 * (-SLOW_INTEGER_FIX_0_899976223); /* sqrt(2) * (c7-c3) */
                z2 = z2 * (-SLOW_INTEGER_FIX_2_562915447); /* sqrt(2) * (-c1-c3) */
                z3 = z3 * (-SLOW_INTEGER_FIX_1_961570560); /* sqrt(2) * (-c3-c5) */
                z4 = z4 * (-SLOW_INTEGER_FIX_0_390180644); /* sqrt(2) * (c5-c3) */

                z3 += z5;
                z4 += z5;

                tmp0 += z1 + z3;
                tmp1 += z2 + z4;
                tmp2 += z2 + z3;
                tmp3 += z1 + z4;

                /* Final output stage: inputs are tmp10..tmp13, tmp0..tmp3 */

                m_componentBuffer[currentOutRow][output_col + 0] = limit[limitOffset + JpegUtils.DESCALE(tmp10 + tmp3, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 7] = limit[limitOffset + JpegUtils.DESCALE(tmp10 - tmp3, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 1] = limit[limitOffset + JpegUtils.DESCALE(tmp11 + tmp2, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 6] = limit[limitOffset + JpegUtils.DESCALE(tmp11 - tmp2, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 2] = limit[limitOffset + JpegUtils.DESCALE(tmp12 + tmp1, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 5] = limit[limitOffset + JpegUtils.DESCALE(tmp12 - tmp1, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 3] = limit[limitOffset + JpegUtils.DESCALE(tmp13 + tmp0, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 4] = limit[limitOffset + JpegUtils.DESCALE(tmp13 - tmp0, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 3) & RANGE_MASK];

                /* advance pointer to next row */
                workspaceIndex += JpegConstants.DCTSIZE;
            }
        }

        /// <summary>
        /// Dequantize a coefficient by multiplying it by the multiplier-table
        /// entry; produce an int result.  In this module, both inputs and result
        /// are 16 bits or less, so either int or short multiply will work.
        /// </summary>
        private static int SLOW_INTEGER_DEQUANTIZE(int coef, int quantval)
        {
            return (coef * quantval);
        }

        /// <summary>
        /// Perform dequantization and inverse DCT on one block of coefficients.
        /// </summary>
        private void jpeg_idct_ifast(int component_index, short[] coef_block, int output_row, int output_col)
        {
            int[] workspace = new int[JpegConstants.DCTSIZE2];

            int coefBlockIndex = 0;
            int workspaceIndex = 0;

            int[] quantTable = m_dctTables[component_index].int_array;
            int quantTableIndex = 0;

            for (int ctr = JpegConstants.DCTSIZE; ctr > 0; ctr--)
            {
                if (coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 2] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 4] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 6] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7] == 0)
                {
                    /* AC terms all zero */
                    int dcval = FAST_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                        quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]);

                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 2] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 3] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 4] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 5] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 6] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 7] = dcval;

                    /* advance pointers to next column */
                    coefBlockIndex++;
                    quantTableIndex++;
                    workspaceIndex++;
                    continue;
                }

                int tmp0 = FAST_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]);
                int tmp1 = FAST_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 2],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 2]);
                int tmp2 = FAST_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 4],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 4]);
                int tmp3 = FAST_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 6],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 6]);

                int tmp10 = tmp0 + tmp2;    /* phase 3 */
                int tmp11 = tmp0 - tmp2;

                int tmp13 = tmp1 + tmp3;    /* phases 5-3 */
                int tmp12 = FAST_INTEGER_MULTIPLY(tmp1 - tmp3, FAST_INTEGER_FIX_1_414213562) - tmp13; /* 2*c4 */

                tmp0 = tmp10 + tmp13;   /* phase 2 */
                tmp3 = tmp10 - tmp13;
                tmp1 = tmp11 + tmp12;
                tmp2 = tmp11 - tmp12;

                /* Odd part */

                int tmp4 = FAST_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 1]);
                int tmp5 = FAST_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 3]);
                int tmp6 = FAST_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 5]);
                int tmp7 = FAST_INTEGER_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 7]);

                int z13 = tmp6 + tmp5;      /* phase 6 */
                int z10 = tmp6 - tmp5;
                int z11 = tmp4 + tmp7;
                int z12 = tmp4 - tmp7;

                tmp7 = z11 + z13;       /* phase 5 */
                tmp11 = FAST_INTEGER_MULTIPLY(z11 - z13, FAST_INTEGER_FIX_1_414213562); /* 2*c4 */

                int z5 = FAST_INTEGER_MULTIPLY(z10 + z12, FAST_INTEGER_FIX_1_847759065); /* 2*c2 */
                tmp10 = FAST_INTEGER_MULTIPLY(z12, FAST_INTEGER_FIX_1_082392200) - z5; /* 2*(c2-c6) */
                tmp12 = FAST_INTEGER_MULTIPLY(z10, -FAST_INTEGER_FIX_2_613125930) + z5; /* -2*(c2+c6) */

                tmp6 = tmp12 - tmp7;    /* phase 2 */
                tmp5 = tmp11 - tmp6;
                tmp4 = tmp10 + tmp5;

                workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = tmp0 + tmp7;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 7] = tmp0 - tmp7;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = tmp1 + tmp6;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 6] = tmp1 - tmp6;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 2] = tmp2 + tmp5;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 5] = tmp2 - tmp5;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 4] = tmp3 + tmp4;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 3] = tmp3 - tmp4;

                coefBlockIndex++;
                quantTableIndex++;
                workspaceIndex++;
            }

            workspaceIndex = 0;
            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset + JpegConstants.CENTERJSAMPLE;

            for (int ctr = 0; ctr < JpegConstants.DCTSIZE; ctr++)
            {
                int currentOutRow = output_row + ctr;
                if (workspace[workspaceIndex + 1] == 0 &&
                    workspace[workspaceIndex + 2] == 0 &&
                    workspace[workspaceIndex + 3] == 0 &&
                    workspace[workspaceIndex + 4] == 0 &&
                    workspace[workspaceIndex + 5] == 0 &&
                    workspace[workspaceIndex + 6] == 0 &&
                    workspace[workspaceIndex + 7] == 0)
                {
                    /* AC terms all zero */
                    byte dcval = limit[limitOffset + FAST_INTEGER_IDESCALE(workspace[workspaceIndex + 0], FAST_INTEGER_PASS1_BITS + 3) & RANGE_MASK];

                    m_componentBuffer[currentOutRow][output_col + 0] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 1] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 2] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 3] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 4] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 5] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 6] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 7] = dcval;

                    /* advance pointer to next row */
                    workspaceIndex += JpegConstants.DCTSIZE;
                    continue;
                }

                /* Even part */

                int tmp10 = workspace[workspaceIndex + 0] + workspace[workspaceIndex + 4];
                int tmp11 = workspace[workspaceIndex + 0] - workspace[workspaceIndex + 4];

                int tmp13 = workspace[workspaceIndex + 2] + workspace[workspaceIndex + 6];
                int tmp12 = FAST_INTEGER_MULTIPLY(workspace[workspaceIndex + 2] - workspace[workspaceIndex + 6], FAST_INTEGER_FIX_1_414213562) - tmp13;

                int tmp0 = tmp10 + tmp13;
                int tmp3 = tmp10 - tmp13;
                int tmp1 = tmp11 + tmp12;
                int tmp2 = tmp11 - tmp12;

                /* Odd part */

                int z13 = workspace[workspaceIndex + 5] + workspace[workspaceIndex + 3];
                int z10 = workspace[workspaceIndex + 5] - workspace[workspaceIndex + 3];
                int z11 = workspace[workspaceIndex + 1] + workspace[workspaceIndex + 7];
                int z12 = workspace[workspaceIndex + 1] - workspace[workspaceIndex + 7];

                int tmp7 = z11 + z13;       /* phase 5 */
                tmp11 = FAST_INTEGER_MULTIPLY(z11 - z13, FAST_INTEGER_FIX_1_414213562); /* 2*c4 */

                int z5 = FAST_INTEGER_MULTIPLY(z10 + z12, FAST_INTEGER_FIX_1_847759065); /* 2*c2 */
                tmp10 = FAST_INTEGER_MULTIPLY(z12, FAST_INTEGER_FIX_1_082392200) - z5; /* 2*(c2-c6) */
                tmp12 = FAST_INTEGER_MULTIPLY(z10, -FAST_INTEGER_FIX_2_613125930) + z5; /* -2*(c2+c6) */

                int tmp6 = tmp12 - tmp7;    /* phase 2 */
                int tmp5 = tmp11 - tmp6;
                int tmp4 = tmp10 + tmp5;

                m_componentBuffer[currentOutRow][output_col + 0] = limit[limitOffset + FAST_INTEGER_IDESCALE(tmp0 + tmp7, FAST_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 7] = limit[limitOffset + FAST_INTEGER_IDESCALE(tmp0 - tmp7, FAST_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 1] = limit[limitOffset + FAST_INTEGER_IDESCALE(tmp1 + tmp6, FAST_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 6] = limit[limitOffset + FAST_INTEGER_IDESCALE(tmp1 - tmp6, FAST_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 2] = limit[limitOffset + FAST_INTEGER_IDESCALE(tmp2 + tmp5, FAST_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 5] = limit[limitOffset + FAST_INTEGER_IDESCALE(tmp2 - tmp5, FAST_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 4] = limit[limitOffset + FAST_INTEGER_IDESCALE(tmp3 + tmp4, FAST_INTEGER_PASS1_BITS + 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 3] = limit[limitOffset + FAST_INTEGER_IDESCALE(tmp3 - tmp4, FAST_INTEGER_PASS1_BITS + 3) & RANGE_MASK];

                /* advance pointer to next row */
                workspaceIndex += JpegConstants.DCTSIZE;
            }
        }

        /// <summary>
        /// Multiply a DCTELEM variable by an int constant, and immediately
        /// descale to yield a DCTELEM result.
        /// </summary>
        private static int FAST_INTEGER_MULTIPLY(int var, int c)
        {
            return (JpegUtils.RIGHT_SHIFT(var * c, FAST_INTEGER_CONST_BITS));
        }

        /// <summary>
        /// Dequantize a coefficient by multiplying it by the multiplier-table
        /// entry; produce a DCTELEM result. 
        /// </summary>
        private static int FAST_INTEGER_DEQUANTIZE(short coef, int quantval)
        {
            return ((int)coef * quantval);
        }

        /// <summary>
        /// Like DESCALE, but applies to a DCTELEM and produces an int.
        /// </summary>
        private static int FAST_INTEGER_IRIGHT_SHIFT(int x, int shft)
        {
            return (x >> shft);
        }

        private static int FAST_INTEGER_IDESCALE(int x, int n)
        {
            return (FAST_INTEGER_IRIGHT_SHIFT(x, n));
        }

        /// <summary>
        /// Perform dequantization and inverse DCT on one block of coefficients.
        /// </summary>
        private void jpeg_idct_float(int component_index, short[] coef_block, int output_row, int output_col)
        {
            float[] workspace = new float[JpegConstants.DCTSIZE2];
            int coefBlockIndex = 0;
            int workspaceIndex = 0;

            float[] quantTable = m_dctTables[component_index].float_array;
            int quantTableIndex = 0;

            for (int ctr = JpegConstants.DCTSIZE; ctr > 0; ctr--)
            {
                if (coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 2] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 4] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 6] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7] == 0)
                {
                    float dcval = FLOAT_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                        quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]);

                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 2] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 3] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 4] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 5] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 6] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 7] = dcval;

                    coefBlockIndex++;            /* advance pointers to next column */
                    quantTableIndex++;
                    workspaceIndex++;
                    continue;
                }

                float tmp0 = FLOAT_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]);
                float tmp1 = FLOAT_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 2],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 2]);
                float tmp2 = FLOAT_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 4],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 4]);
                float tmp3 = FLOAT_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 6],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 6]);

                float tmp10 = tmp0 + tmp2;
                float tmp11 = tmp0 - tmp2;

                float tmp13 = tmp1 + tmp3;
                float tmp12 = (tmp1 - tmp3) * 1.414213562f - tmp13; /* 2*c4 */

                tmp0 = tmp10 + tmp13;
                tmp3 = tmp10 - tmp13;
                tmp1 = tmp11 + tmp12;
                tmp2 = tmp11 - tmp12;

                float tmp4 = FLOAT_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 1]);
                float tmp5 = FLOAT_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 3]);
                float tmp6 = FLOAT_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 5]);
                float tmp7 = FLOAT_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 7]);

                float z13 = tmp6 + tmp5;      /* phase 6 */
                float z10 = tmp6 - tmp5;
                float z11 = tmp4 + tmp7;
                float z12 = tmp4 - tmp7;

                tmp7 = z11 + z13;       /* phase 5 */
                tmp11 = (z11 - z13) * 1.414213562f; /* 2*c4 */

                float z5 = (z10 + z12) * 1.847759065f; /* 2*c2 */
                tmp10 = 1.082392200f * z12 - z5; /* 2*(c2-c6) */
                tmp12 = -2.613125930f * z10 + z5; /* -2*(c2+c6) */

                tmp6 = tmp12 - tmp7;    /* phase 2 */
                tmp5 = tmp11 - tmp6;
                tmp4 = tmp10 + tmp5;

                workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = tmp0 + tmp7;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 7] = tmp0 - tmp7;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = tmp1 + tmp6;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 6] = tmp1 - tmp6;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 2] = tmp2 + tmp5;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 5] = tmp2 - tmp5;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 4] = tmp3 + tmp4;
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 3] = tmp3 - tmp4;

                coefBlockIndex++;            /* advance pointers to next column */
                quantTableIndex++;
                workspaceIndex++;
            }

            workspaceIndex = 0;
            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset + JpegConstants.CENTERJSAMPLE;

            for (int ctr = 0; ctr < JpegConstants.DCTSIZE; ctr++)
            {
                float tmp10 = workspace[workspaceIndex + 0] + workspace[workspaceIndex + 4];
                float tmp11 = workspace[workspaceIndex + 0] - workspace[workspaceIndex + 4];

                float tmp13 = workspace[workspaceIndex + 2] + workspace[workspaceIndex + 6];
                float tmp12 = (workspace[workspaceIndex + 2] - workspace[workspaceIndex + 6]) * 1.414213562f - tmp13;

                float tmp0 = tmp10 + tmp13;
                float tmp3 = tmp10 - tmp13;
                float tmp1 = tmp11 + tmp12;
                float tmp2 = tmp11 - tmp12;

                float z13 = workspace[workspaceIndex + 5] + workspace[workspaceIndex + 3];
                float z10 = workspace[workspaceIndex + 5] - workspace[workspaceIndex + 3];
                float z11 = workspace[workspaceIndex + 1] + workspace[workspaceIndex + 7];
                float z12 = workspace[workspaceIndex + 1] - workspace[workspaceIndex + 7];

                float tmp7 = z11 + z13;
                tmp11 = (z11 - z13) * 1.414213562f;

                float z5 = (z10 + z12) * 1.847759065f; /* 2*c2 */
                tmp10 = 1.082392200f * z12 - z5; /* 2*(c2-c6) */
                tmp12 = -2.613125930f * z10 + z5; /* -2*(c2+c6) */

                float tmp6 = tmp12 - tmp7;
                float tmp5 = tmp11 - tmp6;
                float tmp4 = tmp10 + tmp5;

                /* Final output stage: scale down by a factor of 8 and range-limit */
                int currentOutRow = output_row + ctr;
                m_componentBuffer[currentOutRow][output_col + 0] = limit[limitOffset + JpegUtils.DESCALE((int)(tmp0 + tmp7), 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 7] = limit[limitOffset + JpegUtils.DESCALE((int)(tmp0 - tmp7), 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 1] = limit[limitOffset + JpegUtils.DESCALE((int)(tmp1 + tmp6), 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 6] = limit[limitOffset + JpegUtils.DESCALE((int)(tmp1 - tmp6), 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 2] = limit[limitOffset + JpegUtils.DESCALE((int)(tmp2 + tmp5), 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 5] = limit[limitOffset + JpegUtils.DESCALE((int)(tmp2 - tmp5), 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 4] = limit[limitOffset + JpegUtils.DESCALE((int)(tmp3 + tmp4), 3) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 3] = limit[limitOffset + JpegUtils.DESCALE((int)(tmp3 - tmp4), 3) & RANGE_MASK];

                workspaceIndex += JpegConstants.DCTSIZE;       /* advance pointer to next row */
            }
        }

        /// <summary>
        /// Dequantize a coefficient by multiplying it by the multiplier-table
        /// entry; produce a float result.
        /// </summary>
        private static float FLOAT_DEQUANTIZE(short coef, float quantval)
        {
            return (((float)(coef)) * (quantval));
        }

        /// <summary>
        /// Inverse-DCT routines that produce reduced-size output:
        /// either 4x4, 2x2, or 1x1 pixels from an 8x8 DCT block.
        /// </summary>
        private void jpeg_idct_4x4(int component_index, short[] coef_block, int output_row, int output_col)
        {
            int[] workspace = new int[JpegConstants.DCTSIZE * 4];

            int coefBlockIndex = 0;
            int workspaceIndex = 0;

            int[] quantTable = m_dctTables[component_index].int_array;
            int quantTableIndex = 0;

            for (int ctr = JpegConstants.DCTSIZE; ctr > 0; coefBlockIndex++, quantTableIndex++, workspaceIndex++, ctr--)
            {
                /* Don't bother to process column 4, because second pass won't use it */
                if (ctr == JpegConstants.DCTSIZE - 4)
                    continue;

                if (coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 2] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 6] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7] == 0)
                {
                    /* AC terms all zero; we need not examine term 4 for 4x4 output */
                    int dcval = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                        quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]) << REDUCED_PASS1_BITS;

                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 2] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 3] = dcval;

                    continue;
                }

                int tmp0 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]);
                tmp0 <<= (REDUCED_CONST_BITS + 1);

                int z2 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 2],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 2]);
                int z3 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 6],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 6]);

                int tmp2 = z2 * REDUCED_FIX_1_847759065 + z3 * (-REDUCED_FIX_0_765366865);

                int tmp10 = tmp0 + tmp2;
                int tmp12 = tmp0 - tmp2;

                int z1 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 7]);
                z2 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 5]);
                z3 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 3]);
                int z4 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 1]);

                tmp0 = z1 * (-REDUCED_FIX_0_211164243) /* sqrt(2) * (c3-c1) */ +
                       z2 * REDUCED_FIX_1_451774981 /* sqrt(2) * (c3+c7) */ +
                       z3 * (-REDUCED_FIX_2_172734803) /* sqrt(2) * (-c1-c5) */ +
                       z4 * REDUCED_FIX_1_061594337; /* sqrt(2) * (c5+c7) */

                tmp2 = z1 * (-REDUCED_FIX_0_509795579) /* sqrt(2) * (c7-c5) */ +
                       z2 * (-REDUCED_FIX_0_601344887) /* sqrt(2) * (c5-c1) */ +
                       z3 * REDUCED_FIX_0_899976223 /* sqrt(2) * (c3-c7) */ +
                       z4 * REDUCED_FIX_2_562915447; /* sqrt(2) * (c1+c3) */

                workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = JpegUtils.DESCALE(tmp10 + tmp2, REDUCED_CONST_BITS - REDUCED_PASS1_BITS + 1);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 3] = JpegUtils.DESCALE(tmp10 - tmp2, REDUCED_CONST_BITS - REDUCED_PASS1_BITS + 1);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = JpegUtils.DESCALE(tmp12 + tmp0, REDUCED_CONST_BITS - REDUCED_PASS1_BITS + 1);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 2] = JpegUtils.DESCALE(tmp12 - tmp0, REDUCED_CONST_BITS - REDUCED_PASS1_BITS + 1);
            }

            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset + JpegConstants.CENTERJSAMPLE;

            workspaceIndex = 0;
            for (int ctr = 0; ctr < 4; ctr++)
            {
                int currentOutRow = output_row + ctr;

                if (workspace[workspaceIndex + 1] == 0 &&
                    workspace[workspaceIndex + 2] == 0 &&
                    workspace[workspaceIndex + 3] == 0 &&
                    workspace[workspaceIndex + 5] == 0 &&
                    workspace[workspaceIndex + 6] == 0 &&
                    workspace[workspaceIndex + 7] == 0)
                {
                    byte dcval = limit[limitOffset + JpegUtils.DESCALE(workspace[workspaceIndex + 0], REDUCED_PASS1_BITS + 3) & RANGE_MASK];

                    m_componentBuffer[currentOutRow][output_col + 0] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 1] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 2] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 3] = dcval;

                    workspaceIndex += JpegConstants.DCTSIZE;       /* advance pointer to next row */
                    continue;
                }

                int tmp0 = (workspace[workspaceIndex + 0]) << (REDUCED_CONST_BITS + 1);

                int tmp2 = workspace[workspaceIndex + 2] * REDUCED_FIX_1_847759065 + workspace[workspaceIndex + 6] * (-REDUCED_FIX_0_765366865);

                int tmp10 = tmp0 + tmp2;
                int tmp12 = tmp0 - tmp2;

                /* Odd part */

                int z1 = workspace[workspaceIndex + 7];
                int z2 = workspace[workspaceIndex + 5];
                int z3 = workspace[workspaceIndex + 3];
                int z4 = workspace[workspaceIndex + 1];

                tmp0 = z1 * (-REDUCED_FIX_0_211164243) /* sqrt(2) * (c3-c1) */ +
                       z2 * REDUCED_FIX_1_451774981 /* sqrt(2) * (c3+c7) */ +
                       z3 * (-REDUCED_FIX_2_172734803) /* sqrt(2) * (-c1-c5) */ +
                       z4 * REDUCED_FIX_1_061594337; /* sqrt(2) * (c5+c7) */

                tmp2 = z1 * (-REDUCED_FIX_0_509795579) /* sqrt(2) * (c7-c5) */ +
                       z2 * (-REDUCED_FIX_0_601344887) /* sqrt(2) * (c5-c1) */ +
                       z3 * REDUCED_FIX_0_899976223 /* sqrt(2) * (c3-c7) */ +
                       z4 * REDUCED_FIX_2_562915447; /* sqrt(2) * (c1+c3) */

                m_componentBuffer[currentOutRow][output_col + 0] = limit[limitOffset + JpegUtils.DESCALE(tmp10 + tmp2, REDUCED_CONST_BITS + REDUCED_PASS1_BITS + 3 + 1) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 3] = limit[limitOffset + JpegUtils.DESCALE(tmp10 - tmp2, REDUCED_CONST_BITS + REDUCED_PASS1_BITS + 3 + 1) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 1] = limit[limitOffset + JpegUtils.DESCALE(tmp12 + tmp0, REDUCED_CONST_BITS + REDUCED_PASS1_BITS + 3 + 1) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 2] = limit[limitOffset + JpegUtils.DESCALE(tmp12 - tmp0, REDUCED_CONST_BITS + REDUCED_PASS1_BITS + 3 + 1) & RANGE_MASK];

                workspaceIndex += JpegConstants.DCTSIZE;       /* advance pointer to next row */
            }
        }

        /// <summary>
        /// Perform dequantization and inverse DCT on one block of coefficients,
        /// producing a reduced-size 2x2 output block.
        /// </summary>
        private void jpeg_idct_2x2(int component_index, short[] coef_block, int output_row, int output_col)
        {
            /* buffers data between passes */
            int[] workspace = new int[JpegConstants.DCTSIZE * 2];

            int coefBlockIndex = 0;
            int workspaceIndex = 0;

            int[] quantTable = m_dctTables[component_index].int_array;
            int quantTableIndex = 0;

            for (int ctr = JpegConstants.DCTSIZE; ctr > 0; coefBlockIndex++, quantTableIndex++, workspaceIndex++, ctr--)
            {
                /* Don't bother to process columns 2,4,6 */
                if (ctr == JpegConstants.DCTSIZE - 2 || ctr == JpegConstants.DCTSIZE - 4 || ctr == JpegConstants.DCTSIZE - 6)
                    continue;

                if (coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5] == 0 &&
                    coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7] == 0)
                {
                    int dcval = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                        quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]) << REDUCED_PASS1_BITS;

                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = dcval;
                    workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = dcval;

                    continue;
                }

                int z1 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 0],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 0]);
                int tmp10 = z1 << (REDUCED_CONST_BITS + 2);

                z1 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 7],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 7]);
                int tmp0 = z1 * -REDUCED_FIX_0_720959822; /* sqrt(2) * (c7-c5+c3-c1) */
                z1 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 5],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 5]);
                tmp0 += z1 * REDUCED_FIX_0_850430095; /* sqrt(2) * (-c1+c3+c5+c7) */
                z1 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 3],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 3]);
                tmp0 += z1 * (-REDUCED_FIX_1_272758580); /* sqrt(2) * (-c1+c3-c5-c7) */
                z1 = REDUCED_DEQUANTIZE(coef_block[coefBlockIndex + JpegConstants.DCTSIZE * 1],
                    quantTable[quantTableIndex + JpegConstants.DCTSIZE * 1]);
                tmp0 += z1 * REDUCED_FIX_3_624509785; /* sqrt(2) * (c1+c3+c5+c7) */

                workspace[workspaceIndex + JpegConstants.DCTSIZE * 0] = JpegUtils.DESCALE(tmp10 + tmp0, REDUCED_CONST_BITS - REDUCED_PASS1_BITS + 2);
                workspace[workspaceIndex + JpegConstants.DCTSIZE * 1] = JpegUtils.DESCALE(tmp10 - tmp0, REDUCED_CONST_BITS - REDUCED_PASS1_BITS + 2);
            }

            workspaceIndex = 0;
            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset + JpegConstants.CENTERJSAMPLE;

            for (int ctr = 0; ctr < 2; ctr++)
            {
                int currentOutRow = output_row + ctr;

                if (workspace[workspaceIndex + 1] == 0 &&
                    workspace[workspaceIndex + 3] == 0 &&
                    workspace[workspaceIndex + 5] == 0 &&
                    workspace[workspaceIndex + 7] == 0)
                {
                    byte dcval = limit[limitOffset + JpegUtils.DESCALE(workspace[workspaceIndex + 0], REDUCED_PASS1_BITS + 3) & RANGE_MASK];

                    m_componentBuffer[currentOutRow][output_col + 0] = dcval;
                    m_componentBuffer[currentOutRow][output_col + 1] = dcval;

                    workspaceIndex += JpegConstants.DCTSIZE;       /* advance pointer to next row */
                    continue;
                }

                int tmp10 = (workspace[workspaceIndex + 0]) << (REDUCED_CONST_BITS + 2);
                int tmp0 = workspace[workspaceIndex + 7] * (-REDUCED_FIX_0_720959822) /* sqrt(2) * (c7-c5+c3-c1) */ +
                       workspace[workspaceIndex + 5] * REDUCED_FIX_0_850430095 /* sqrt(2) * (-c1+c3+c5+c7) */ +
                       workspace[workspaceIndex + 3] * (-REDUCED_FIX_1_272758580) /* sqrt(2) * (-c1+c3-c5-c7) */ +
                       workspace[workspaceIndex + 1] * REDUCED_FIX_3_624509785; /* sqrt(2) * (c1+c3+c5+c7) */

                m_componentBuffer[currentOutRow][output_col + 0] = limit[limitOffset + JpegUtils.DESCALE(tmp10 + tmp0, REDUCED_CONST_BITS + REDUCED_PASS1_BITS + 3 + 2) & RANGE_MASK];
                m_componentBuffer[currentOutRow][output_col + 1] = limit[limitOffset + JpegUtils.DESCALE(tmp10 - tmp0, REDUCED_CONST_BITS + REDUCED_PASS1_BITS + 3 + 2) & RANGE_MASK];

                workspaceIndex += JpegConstants.DCTSIZE;       /* advance pointer to next row */
            }
        }

        /// <summary>
        /// Perform dequantization and inverse DCT on one block of coefficients,
        /// producing a reduced-size 1x1 output block.
        /// </summary>
        private void jpeg_idct_1x1(int component_index, short[] coef_block, int output_row, int output_col)
        {
            int[] quantptr = m_dctTables[component_index].int_array;
            int dcval = REDUCED_DEQUANTIZE(coef_block[0], quantptr[0]);
            dcval = JpegUtils.DESCALE(dcval, 3);

            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset + JpegConstants.CENTERJSAMPLE;

            m_componentBuffer[output_row + 0][output_col] = limit[limitOffset + dcval & RANGE_MASK];
        }

        /// <summary>
        /// Dequantize a coefficient by multiplying it by the multiplier-table
        /// entry; produce an int result.  
        /// </summary>
        private static int REDUCED_DEQUANTIZE(short coef, int quantval)
        {
            return ((int)coef * quantval);
        }
    }

    /// <summary>
    /// Marker reading and parsing
    /// </summary>
    class MarkerReader
    {
        private const int APP0_DATA_LEN = 14;  /* Length of interesting data in APP0 */
        private const int APP14_DATA_LEN = 12;  /* Length of interesting data in APP14 */
        private const int APPN_DATA_LEN = 14;  /* Must be the largest of the above!! */
        private DecompressStruct m_cinfo;
        private DecompressStruct.jpeg_marker_parser_method m_process_COM;
        private DecompressStruct.jpeg_marker_parser_method[] m_process_APPn = new DecompressStruct.jpeg_marker_parser_method[16];
        private int m_length_limit_COM;
        private int[] m_length_limit_APPn = new int[16];
        private bool m_saw_SOI;       /* found SOI? */
        private bool m_saw_SOF;       /* found SOF? */
        private int m_next_restart_num;       /* next restart number expected (0-7) */
        private int m_discarded_bytes;   /* # of bytes skipped looking for a marker */
        private MarkerStruct m_cur_marker; /* null if not processing a marker */
        private int m_bytes_read;        /* data bytes read so far in marker */

        /// <summary>
        /// Initialize the marker reader module.
        /// </summary>
        public MarkerReader(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            m_process_COM = skip_variable;

            for (int i = 0; i < 16; i++)
            {
                m_process_APPn[i] = skip_variable;
                m_length_limit_APPn[i] = 0;
            }

            m_process_APPn[0] = get_interesting_appn;
            m_process_APPn[14] = get_interesting_appn;

            reset_marker_reader();
        }

        /// <summary>
        /// Reset marker processing state to begin a fresh datastream.
        /// </summary>
        public void reset_marker_reader()
        {
            m_cinfo.Comp_info = null;        /* until allocated by get_sof */
            m_cinfo.m_input_scan_number = 0;       /* no SOS seen yet */
            m_cinfo.m_unread_marker = 0;       /* no pending marker */
            m_saw_SOI = false;        /* set internal state too */
            m_saw_SOF = false;
            m_discarded_bytes = 0;
            m_cur_marker = null;
        }

        /// <summary>
        /// Read markers until SOS or EOI.
        /// </summary>
        public ReadResult read_markers()
        {
            for (; ; )
            {
                if (m_cinfo.m_unread_marker == 0)
                {
                    if (!m_cinfo.m_marker.m_saw_SOI)
                    {
                        if (!first_marker())
                            return ReadResult.JPEG_SUSPENDED;
                    }
                    else
                    {
                        if (!next_marker())
                            return ReadResult.JPEG_SUSPENDED;
                    }
                }

                switch ((JPEG_MARKER)m_cinfo.m_unread_marker)
                {
                    case JPEG_MARKER.SOI:
                        if (!get_soi())
                            return ReadResult.JPEG_SUSPENDED;
                        break;

                    case JPEG_MARKER.SOF0:
                    /* Baseline */
                    case JPEG_MARKER.SOF1:
                        /* Extended sequential, Huffman */
                        if (!get_sof(false))
                            return ReadResult.JPEG_SUSPENDED;
                        break;

                    case JPEG_MARKER.SOF2:
                        /* Progressive, Huffman */
                        if (!get_sof(true))
                            return ReadResult.JPEG_SUSPENDED;
                        break;

                    /* Currently unsupported SOFn types */
                    case JPEG_MARKER.SOF3:
                    /* Lossless, Huffman */
                    case JPEG_MARKER.SOF5:
                    /* Differential sequential, Huffman */
                    case JPEG_MARKER.SOF6:
                    /* Differential progressive, Huffman */
                    case JPEG_MARKER.SOF7:
                    /* Differential lossless, Huffman */
                    case JPEG_MARKER.SOF9:
                    /* Extended sequential, arithmetic */
                    case JPEG_MARKER.SOF10:
                    /* Progressive, arithmetic */
                    case JPEG_MARKER.JPG:
                    /* Reserved for JPEG extensions */
                    case JPEG_MARKER.SOF11:
                    /* Lossless, arithmetic */
                    case JPEG_MARKER.SOF13:
                    /* Differential sequential, arithmetic */
                    case JPEG_MARKER.SOF14:
                    /* Differential progressive, arithmetic */
                    case JPEG_MARKER.SOF15:
                        break;

                    case JPEG_MARKER.SOS:
                        if (!get_sos())
                            return ReadResult.JPEG_SUSPENDED;
                        m_cinfo.m_unread_marker = 0;   /* processed the marker */
                        return ReadResult.JPEG_REACHED_SOS;

                    case JPEG_MARKER.EOI:
                        m_cinfo.m_unread_marker = 0;   /* processed the marker */
                        return ReadResult.JPEG_REACHED_EOI;

                    case JPEG_MARKER.DAC:
                        if (!skip_variable(m_cinfo))
                            return ReadResult.JPEG_SUSPENDED;
                        break;

                    case JPEG_MARKER.DHT:
                        if (!get_dht())
                            return ReadResult.JPEG_SUSPENDED;
                        break;

                    case JPEG_MARKER.DQT:
                        if (!get_dqt())
                            return ReadResult.JPEG_SUSPENDED;
                        break;

                    case JPEG_MARKER.DRI:
                        if (!get_dri())
                            return ReadResult.JPEG_SUSPENDED;
                        break;

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
                        if (!m_cinfo.m_marker.m_process_APPn[m_cinfo.m_unread_marker - (int)JPEG_MARKER.APP0](m_cinfo))
                            return ReadResult.JPEG_SUSPENDED;
                        break;

                    case JPEG_MARKER.COM:
                        if (!m_cinfo.m_marker.m_process_COM(m_cinfo))
                            return ReadResult.JPEG_SUSPENDED;
                        break;

                    /* these are all parameterless */
                    case JPEG_MARKER.RST0:
                    case JPEG_MARKER.RST1:
                    case JPEG_MARKER.RST2:
                    case JPEG_MARKER.RST3:
                    case JPEG_MARKER.RST4:
                    case JPEG_MARKER.RST5:
                    case JPEG_MARKER.RST6:
                    case JPEG_MARKER.RST7:
                    case JPEG_MARKER.TEM:
                        break;

                    case JPEG_MARKER.DNL:
                        if (!skip_variable(m_cinfo))
                            return ReadResult.JPEG_SUSPENDED;
                        break;

                    default:
                        break;
                }

                m_cinfo.m_unread_marker = 0;
            }
        }

        /// <summary>
        /// Read a restart marker, which is expected to appear next in the datastream;
        /// </summary>
        public bool read_restart_marker()
        {
            if (m_cinfo.m_unread_marker == 0)
            {
                if (!next_marker())
                    return false;
            }

            if (m_cinfo.m_unread_marker == ((int)JPEG_MARKER.RST0 + m_cinfo.m_marker.m_next_restart_num))
            {
                m_cinfo.m_unread_marker = 0;
            }
            else
            {
                if (!m_cinfo.m_src.resync_to_restart(m_cinfo, m_cinfo.m_marker.m_next_restart_num))
                    return false;
            }

            m_cinfo.m_marker.m_next_restart_num = (m_cinfo.m_marker.m_next_restart_num + 1) & 7;

            return true;
        }

        /// <summary>
        /// Find the next JPEG marker, save it in cinfo.unread_marker.
        /// </summary>
        public bool next_marker()
        {
            int c;
            for (; ; )
            {
                if (!m_cinfo.m_src.GetByte(out c))
                    return false;
                while (c != 0xFF)
                {
                    m_cinfo.m_marker.m_discarded_bytes++;
                    if (!m_cinfo.m_src.GetByte(out c))
                        return false;
                }

                do
                {
                    if (!m_cinfo.m_src.GetByte(out c))
                        return false;
                }
                while (c == 0xFF);

                if (c != 0)
                {
                    break;
                }

                m_cinfo.m_marker.m_discarded_bytes += 2;
            }

            if (m_cinfo.m_marker.m_discarded_bytes != 0)
            {
                m_cinfo.m_marker.m_discarded_bytes = 0;
            }

            m_cinfo.m_unread_marker = c;
            return true;
        }

        /// <summary>
        /// Install a special processing method for COM or APPn markers.
        /// </summary>
        public void jpeg_set_marker_processor(int marker_code, DecompressStruct.jpeg_marker_parser_method routine)
        {
            if (marker_code == (int)JPEG_MARKER.COM)
                m_process_COM = routine;
            else if (marker_code >= (int)JPEG_MARKER.APP0 && marker_code <= (int)JPEG_MARKER.APP15)
                m_process_APPn[marker_code - (int)JPEG_MARKER.APP0] = routine;
        }

        public void jpeg_save_markers(int marker_code, int length_limit)
        {
            DecompressStruct.jpeg_marker_parser_method processor;
            if (length_limit != 0)
            {
                processor = save_marker;
                if (marker_code == (int)JPEG_MARKER.APP0 && length_limit < APP0_DATA_LEN)
                    length_limit = APP0_DATA_LEN;
                else if (marker_code == (int)JPEG_MARKER.APP14 && length_limit < APP14_DATA_LEN)
                    length_limit = APP14_DATA_LEN;
            }
            else
            {
                processor = skip_variable;
                if (marker_code == (int)JPEG_MARKER.APP0 || marker_code == (int)JPEG_MARKER.APP14)
                    processor = get_interesting_appn;
            }

            if (marker_code == (int)JPEG_MARKER.COM)
            {
                m_process_COM = processor;
                m_length_limit_COM = length_limit;
            }
            else if (marker_code >= (int)JPEG_MARKER.APP0 && marker_code <= (int)JPEG_MARKER.APP15)
            {
                m_process_APPn[marker_code - (int)JPEG_MARKER.APP0] = processor;
                m_length_limit_APPn[marker_code - (int)JPEG_MARKER.APP0] = length_limit;
            }
        }

        public bool SawSOI()
        {
            return m_saw_SOI;
        }

        public bool SawSOF()
        {
            return m_saw_SOF;
        }

        public int NextRestartNumber()
        {
            return m_next_restart_num;
        }

        public int DiscardedByteCount()
        {
            return m_discarded_bytes;
        }

        public void SkipBytes(int count)
        {
            m_discarded_bytes += count;
        }

        /// <summary>
        /// Save an APPn or COM marker into the marker list
        /// </summary>
        private static bool save_marker(DecompressStruct cinfo)
        {
            MarkerStruct cur_marker = cinfo.m_marker.m_cur_marker;

            byte[] data = null;
            int length = 0;
            int bytes_read;
            int data_length;
            int dataOffset = 0;

            if (cur_marker == null)
            {
                if (!cinfo.m_src.GetTwoBytes(out length))
                    return false;

                length -= 2;
                if (length >= 0)
                {
                    int limit;
                    if (cinfo.m_unread_marker == (int)JPEG_MARKER.COM)
                        limit = cinfo.m_marker.m_length_limit_COM;
                    else
                        limit = cinfo.m_marker.m_length_limit_APPn[cinfo.m_unread_marker - (int)JPEG_MARKER.APP0];

                    if (length < limit)
                        limit = length;

                    cur_marker = new MarkerStruct((byte)cinfo.m_unread_marker, length, limit);

                    data = cur_marker.Data;
                    cinfo.m_marker.m_cur_marker = cur_marker;
                    cinfo.m_marker.m_bytes_read = 0;
                    bytes_read = 0;
                    data_length = limit;
                }
                else
                {
                    bytes_read = data_length = 0;
                    data = null;
                }
            }
            else
            {
                bytes_read = cinfo.m_marker.m_bytes_read;
                data_length = cur_marker.Data.Length;
                data = cur_marker.Data;
                dataOffset = bytes_read;
            }

            byte[] tempData = null;
            if (data_length != 0)
                tempData = new byte[data.Length];

            while (bytes_read < data_length)
            {
                cinfo.m_marker.m_bytes_read = bytes_read;

                if (!cinfo.m_src.MakeByteAvailable())
                    return false;

                int read = cinfo.m_src.GetBytes(tempData, data_length - bytes_read);
                Buffer.BlockCopy(tempData, 0, data, dataOffset, read);
                bytes_read += read;
                dataOffset += read;
            }

            if (cur_marker != null)
            {
                cinfo.m_marker_list.Add(cur_marker);

                data = cur_marker.Data;
                dataOffset = 0;
                length = cur_marker.OriginalLength - data_length;
            }

            cinfo.m_marker.m_cur_marker = null;

            JPEG_MARKER currentMarker = (JPEG_MARKER)cinfo.m_unread_marker;
            if (data_length != 0 && (currentMarker == JPEG_MARKER.APP0 || currentMarker == JPEG_MARKER.APP14))
            {
                tempData = new byte[data.Length];
                Buffer.BlockCopy(data, dataOffset, tempData, 0, data.Length - dataOffset);
            }

            switch ((JPEG_MARKER)cinfo.m_unread_marker)
            {
                case JPEG_MARKER.APP0:
                    examine_app0(cinfo, tempData, data_length, length);
                    break;
                case JPEG_MARKER.APP14:
                    examine_app14(cinfo, tempData, data_length, length);
                    break;
                default:
                    break;
            }

            if (length > 0)
                cinfo.m_src.skip_input_data(length);

            return true;
        }

        /// <summary>
        /// Skip over an unknown or uninteresting variable-length marker
        /// </summary>
        private static bool skip_variable(DecompressStruct cinfo)
        {
            int length;
            if (!cinfo.m_src.GetTwoBytes(out length))
                return false;

            length -= 2;

            if (length > 0)
                cinfo.m_src.skip_input_data(length);

            return true;
        }

        /// <summary>
        /// Process an APP0 or APP14 marker without saving it
        /// </summary>
        private static bool get_interesting_appn(DecompressStruct cinfo)
        {
            int length;
            if (!cinfo.m_src.GetTwoBytes(out length))
                return false;

            length -= 2;

            int numtoread = 0;
            if (length >= APPN_DATA_LEN)
                numtoread = APPN_DATA_LEN;
            else if (length > 0)
                numtoread = length;

            byte[] b = new byte[APPN_DATA_LEN];
            for (int i = 0; i < numtoread; i++)
            {
                int temp = 0;
                if (!cinfo.m_src.GetByte(out temp))
                    return false;

                b[i] = (byte)temp;
            }

            length -= numtoread;

            /* process it */
            switch ((JPEG_MARKER)cinfo.m_unread_marker)
            {
                case JPEG_MARKER.APP0:
                    examine_app0(cinfo, b, numtoread, length);
                    break;
                case JPEG_MARKER.APP14:
                    examine_app14(cinfo, b, numtoread, length);
                    break;
                default:
                    break;
            }

            if (length > 0)
                cinfo.m_src.skip_input_data(length);

            return true;
        }

        /// <summary>
        /// Examine first few bytes from an APP0.
        /// </summary>
        private static void examine_app0(DecompressStruct cinfo, byte[] data, int datalen, int remaining)
        {
            int totallen = datalen + remaining;

            if (datalen >= APP0_DATA_LEN &&
                data[0] == 0x4A &&
                data[1] == 0x46 &&
                data[2] == 0x49 &&
                data[3] == 0x46 &&
                data[4] == 0)
            {
                cinfo.m_saw_JFIF_marker = true;
                cinfo.m_JFIF_major_version = data[5];
                cinfo.m_JFIF_minor_version = data[6];
                cinfo.m_density_unit = (DensityUnit)data[7];
                cinfo.m_X_density = (short)((data[8] << 8) + data[9]);
                cinfo.m_Y_density = (short)((data[10] << 8) + data[11]);

                totallen -= APP0_DATA_LEN;
            }
            else if (datalen >= 6 && data[0] == 0x4A && data[1] == 0x46 && data[2] == 0x58 && data[3] == 0x58 && data[4] == 0)
            {
            }
            else
            {
            }
        }

        /// <summary>
        /// Examine first few bytes from an APP14.
        /// </summary>
        private static void examine_app14(DecompressStruct cinfo, byte[] data, int datalen, int remaining)
        {
            if (datalen >= APP14_DATA_LEN &&
                data[0] == 0x41 &&
                data[1] == 0x64 &&
                data[2] == 0x6F &&
                data[3] == 0x62 &&
                data[4] == 0x65)
            {
                /* Found Adobe APP14 marker */
                int version = (data[5] << 8) + data[6];
                int flags0 = (data[7] << 8) + data[8];
                int flags1 = (data[9] << 8) + data[10];
                int transform = data[11];
                cinfo.m_saw_Adobe_marker = true;
                cinfo.m_Adobe_transform = (byte)transform;
            }
            else
            {
            }
        }

        /// <summary>
        /// Process an SOI marker
        /// </summary>
        private bool get_soi()
        {
            m_cinfo.m_restart_interval = 0;
            m_cinfo.m_jpeg_color_space = J_COLOR_SPACE.JCS_UNKNOWN;
            m_cinfo.m_CCIR601_sampling = false; /* Assume non-CCIR sampling??? */

            m_cinfo.m_saw_JFIF_marker = false;
            m_cinfo.m_JFIF_major_version = 1; /* set default JFIF APP0 values */
            m_cinfo.m_JFIF_minor_version = 1;
            m_cinfo.m_density_unit = DensityUnit.Unknown;
            m_cinfo.m_X_density = 1;
            m_cinfo.m_Y_density = 1;
            m_cinfo.m_saw_Adobe_marker = false;
            m_cinfo.m_Adobe_transform = 0;

            m_cinfo.m_marker.m_saw_SOI = true;

            return true;
        }

        /// <summary>
        /// Process a SOFn marker
        /// </summary>
        private bool get_sof(bool is_prog)
        {
            m_cinfo.m_progressive_mode = is_prog;

            int length;
            if (!m_cinfo.m_src.GetTwoBytes(out length))
                return false;

            if (!m_cinfo.m_src.GetByte(out m_cinfo.m_data_precision))
                return false;

            int temp = 0;
            if (!m_cinfo.m_src.GetTwoBytes(out temp))
                return false;
            m_cinfo.m_image_height = temp;

            if (!m_cinfo.m_src.GetTwoBytes(out temp))
                return false;
            m_cinfo.m_image_width = temp;

            if (!m_cinfo.m_src.GetByte(out m_cinfo.m_num_components))
                return false;

            length -= 8;

            if (m_cinfo.Comp_info == null)
            {
                m_cinfo.Comp_info = ComponentInfo.createArrayOfComponents(m_cinfo.m_num_components);
            }

            for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                m_cinfo.Comp_info[ci].Component_index = ci;

                int component_id;
                if (!m_cinfo.m_src.GetByte(out component_id))
                    return false;

                m_cinfo.Comp_info[ci].Component_id = component_id;

                int c;
                if (!m_cinfo.m_src.GetByte(out c))
                    return false;

                m_cinfo.Comp_info[ci].H_samp_factor = (c >> 4) & 15;
                m_cinfo.Comp_info[ci].V_samp_factor = (c) & 15;

                int quant_tbl_no;
                if (!m_cinfo.m_src.GetByte(out quant_tbl_no))
                    return false;

                m_cinfo.Comp_info[ci].Quant_tbl_no = quant_tbl_no;
            }

            m_cinfo.m_marker.m_saw_SOF = true;
            return true;
        }

        /// <summary>
        /// Process a SOS marker
        /// </summary>
        private bool get_sos()
        {
            int length;
            if (!m_cinfo.m_src.GetTwoBytes(out length))
                return false;

            int n;
            if (!m_cinfo.m_src.GetByte(out n))
                return false;

            m_cinfo.m_comps_in_scan = n;

            for (int i = 0; i < n; i++)
            {
                int cc;
                if (!m_cinfo.m_src.GetByte(out cc))
                    return false;

                int c;
                if (!m_cinfo.m_src.GetByte(out c))
                    return false;

                bool idFound = false;
                int foundIndex = -1;
                for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
                {
                    if (cc == m_cinfo.Comp_info[ci].Component_id)
                    {
                        foundIndex = ci;
                        idFound = true;
                        break;
                    }
                }

                m_cinfo.m_cur_comp_info[i] = foundIndex;
                m_cinfo.Comp_info[foundIndex].Dc_tbl_no = (c >> 4) & 15;
                m_cinfo.Comp_info[foundIndex].Ac_tbl_no = (c) & 15;
            }

            int temp;
            if (!m_cinfo.m_src.GetByte(out temp))
                return false;

            m_cinfo.m_Ss = temp;
            if (!m_cinfo.m_src.GetByte(out temp))
                return false;

            m_cinfo.m_Se = temp;
            if (!m_cinfo.m_src.GetByte(out temp))
                return false;

            m_cinfo.m_Ah = (temp >> 4) & 15;
            m_cinfo.m_Al = (temp) & 15;

            m_cinfo.m_marker.m_next_restart_num = 0;
            m_cinfo.m_input_scan_number++;
            return true;
        }

        /// <summary>
        /// Process a DHT marker
        /// </summary>
        private bool get_dht()
        {
            int length;
            if (!m_cinfo.m_src.GetTwoBytes(out length))
                return false;

            length -= 2;

            byte[] bits = new byte[17];
            byte[] huffval = new byte[256];
            while (length > 16)
            {
                int index;
                if (!m_cinfo.m_src.GetByte(out index))
                    return false;

                bits[0] = 0;
                int count = 0;
                for (int i = 1; i <= 16; i++)
                {
                    int temp = 0;
                    if (!m_cinfo.m_src.GetByte(out temp))
                        return false;

                    bits[i] = (byte)temp;
                    count += bits[i];
                }

                length -= 1 + 16;

                for (int i = 0; i < count; i++)
                {
                    int temp = 0;
                    if (!m_cinfo.m_src.GetByte(out temp))
                        return false;

                    huffval[i] = (byte)temp;
                }

                length -= count;

                JHUFF_TBL htblptr = null;
                if ((index & 0x10) != 0)
                {
                    index -= 0x10;
                    if (m_cinfo.m_ac_huff_tbl_ptrs[index] == null)
                        m_cinfo.m_ac_huff_tbl_ptrs[index] = new JHUFF_TBL();

                    htblptr = m_cinfo.m_ac_huff_tbl_ptrs[index];
                }
                else
                {
                    if (m_cinfo.m_dc_huff_tbl_ptrs[index] == null)
                        m_cinfo.m_dc_huff_tbl_ptrs[index] = new JHUFF_TBL();

                    htblptr = m_cinfo.m_dc_huff_tbl_ptrs[index];
                }

                Buffer.BlockCopy(bits, 0, htblptr.Bits, 0, htblptr.Bits.Length);
                Buffer.BlockCopy(huffval, 0, htblptr.Huffval, 0, htblptr.Huffval.Length);
            }
            return true;
        }

        /// <summary>
        /// Process a DQT marker
        /// </summary>
        private bool get_dqt()
        {
            int length;
            if (!m_cinfo.m_src.GetTwoBytes(out length))
                return false;

            length -= 2;
            while (length > 0)
            {
                int n;
                if (!m_cinfo.m_src.GetByte(out n))
                    return false;

                int prec = n >> 4;
                n &= 0x0F;

                if (m_cinfo.m_quant_tbl_ptrs[n] == null)
                    m_cinfo.m_quant_tbl_ptrs[n] = new JQUANT_TBL();

                JQUANT_TBL quant_ptr = m_cinfo.m_quant_tbl_ptrs[n];

                for (int i = 0; i < JpegConstants.DCTSIZE2; i++)
                {
                    int tmp;
                    if (prec != 0)
                    {
                        int temp = 0;
                        if (!m_cinfo.m_src.GetTwoBytes(out temp))
                            return false;

                        tmp = temp;
                    }
                    else
                    {
                        int temp = 0;
                        if (!m_cinfo.m_src.GetByte(out temp))
                            return false;

                        tmp = temp;
                    }

                    quant_ptr.quantval[JpegUtils.jpeg_natural_order[i]] = (short)tmp;
                }

                length -= JpegConstants.DCTSIZE2 + 1;
                if (prec != 0)
                    length -= JpegConstants.DCTSIZE2;
            }

            return true;
        }

        /// <summary>
        /// Process a DRI marker
        /// </summary>
        private bool get_dri()
        {
            int length;
            if (!m_cinfo.m_src.GetTwoBytes(out length))
                return false;

            int temp = 0;
            if (!m_cinfo.m_src.GetTwoBytes(out temp))
                return false;

            int tmp = temp;
            m_cinfo.m_restart_interval = tmp;

            return true;
        }

        /// <summary>
        /// Like next_marker, but used to obtain the initial SOI marker.
        /// </summary>
        private bool first_marker()
        {
            int c;
            if (!m_cinfo.m_src.GetByte(out c))
                return false;

            int c2;
            if (!m_cinfo.m_src.GetByte(out c2))
                return false;

            m_cinfo.m_unread_marker = c2;
            return true;
        }
    }

    /// <summary>
    /// Upsampling (note that upsampler must also call color converter)
    /// </summary>
    abstract class Upsampler
    {
        protected bool m_need_context_rows;

        public abstract void start_pass();
        public abstract void upsample(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, byte[][] output_buf, ref int out_row_ctr, int out_rows_avail);

        public bool NeedContextRows()
        {
            return m_need_context_rows;
        }
    }

    /// <summary>
    /// Expanded entropy decoder object for progressive Huffman decoding.
    /// </summary>
    class PHuffEntropyDecoder : EntropyDecoder
    {
        private class savable_state
        {
            public int EOBRUN;
            public int[] last_dc_val = new int[JpegConstants.MAX_COMPS_IN_SCAN]; /* last DC coef for each component */

            public void Assign(savable_state ss)
            {
                EOBRUN = ss.EOBRUN;
                Buffer.BlockCopy(ss.last_dc_val, 0, last_dc_val, 0, last_dc_val.Length * sizeof(int));
            }
        }

        private enum MCUDecoder
        {
            mcu_DC_first_decoder,
            mcu_AC_first_decoder,
            mcu_DC_refine_decoder,
            mcu_AC_refine_decoder
        }

        private MCUDecoder m_decoder;
        private BitReadPermState m_bitstate;
        private savable_state m_saved = new savable_state();
        private int m_restarts_to_go;
        private DDerivedTbl[] m_derived_tbls = new DDerivedTbl[JpegConstants.NUM_HUFF_TBLS];
        private DDerivedTbl m_ac_derived_tbl;

        public PHuffEntropyDecoder(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            for (int i = 0; i < JpegConstants.NUM_HUFF_TBLS; i++)
                m_derived_tbls[i] = null;

            cinfo.m_coef_bits = new int[cinfo.m_num_components][];
            for (int i = 0; i < cinfo.m_num_components; i++)
                cinfo.m_coef_bits[i] = new int[JpegConstants.DCTSIZE2];

            for (int ci = 0; ci < cinfo.m_num_components; ci++)
            {
                for (int i = 0; i < JpegConstants.DCTSIZE2; i++)
                    cinfo.m_coef_bits[ci][i] = -1;
            }
        }

        /// <summary>
        /// Initialize for a Huffman-compressed scan.
        /// </summary>
        public override void start_pass()
        {
            bool bad = false;
            bool is_DC_band = (m_cinfo.m_Ss == 0);
            if (is_DC_band)
            {
                if (m_cinfo.m_Se != 0)
                    bad = true;
            }
            else
            {
                if (m_cinfo.m_Ss > m_cinfo.m_Se || m_cinfo.m_Se >= JpegConstants.DCTSIZE2)
                    bad = true;

                if (m_cinfo.m_comps_in_scan != 1)
                    bad = true;
            }

            if (m_cinfo.m_Ah != 0)
            {
                if (m_cinfo.m_Al != m_cinfo.m_Ah - 1)
                    bad = true;
            }

            if (m_cinfo.m_Al > 13)
            {
                bad = true;
            }

            for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
            {
                int cindex = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]].Component_index;

                for (int coefi = m_cinfo.m_Ss; coefi <= m_cinfo.m_Se; coefi++)
                {
                    int expected = m_cinfo.m_coef_bits[cindex][coefi];
                    if (expected < 0)
                        expected = 0;

                    m_cinfo.m_coef_bits[cindex][coefi] = m_cinfo.m_Al;
                }
            }

            if (m_cinfo.m_Ah == 0)
            {
                if (is_DC_band)
                    m_decoder = MCUDecoder.mcu_DC_first_decoder;
                else
                    m_decoder = MCUDecoder.mcu_AC_first_decoder;
            }
            else
            {
                if (is_DC_band)
                    m_decoder = MCUDecoder.mcu_DC_refine_decoder;
                else
                    m_decoder = MCUDecoder.mcu_AC_refine_decoder;
            }

            for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
            {
                ComponentInfo componentInfo = m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]];
                if (is_DC_band)
                {
                    if (m_cinfo.m_Ah == 0)
                    {
                        jpeg_make_d_derived_tbl(true, componentInfo.Dc_tbl_no, ref m_derived_tbls[componentInfo.Dc_tbl_no]);
                    }
                }
                else
                {
                    jpeg_make_d_derived_tbl(false, componentInfo.Ac_tbl_no, ref m_derived_tbls[componentInfo.Ac_tbl_no]);

                    m_ac_derived_tbl = m_derived_tbls[componentInfo.Ac_tbl_no];
                }

                m_saved.last_dc_val[ci] = 0;
            }

            m_bitstate.bits_left = 0;
            m_bitstate.get_buffer = 0; /* unnecessary, but keeps Purify quiet */
            m_insufficient_data = false;
            m_saved.EOBRUN = 0;
            m_restarts_to_go = m_cinfo.m_restart_interval;
        }

        public override bool decode_mcu(JBLOCK[] MCU_data)
        {
            switch (m_decoder)
            {
                case MCUDecoder.mcu_DC_first_decoder:
                    return decode_mcu_DC_first(MCU_data);
                case MCUDecoder.mcu_AC_first_decoder:
                    return decode_mcu_AC_first(MCU_data);
                case MCUDecoder.mcu_DC_refine_decoder:
                    return decode_mcu_DC_refine(MCU_data);
                case MCUDecoder.mcu_AC_refine_decoder:
                    return decode_mcu_AC_refine(MCU_data);
            }

            return false;
        }

        /// <summary>
        /// MCU decoding for DC initial scan (either spectral selection,
        /// or first pass of successive approximation).
        /// </summary>
        private bool decode_mcu_DC_first(JBLOCK[] MCU_data)
        {
            if (m_cinfo.m_restart_interval != 0)
            {
                if (m_restarts_to_go == 0)
                {
                    if (!process_restart())
                        return false;
                }
            }

            if (!m_insufficient_data)
            {
                int get_buffer;
                int bits_left;
                BitReadWorkingState br_state = new BitReadWorkingState();
                BITREAD_LOAD_STATE(m_bitstate, out get_buffer, out bits_left, ref br_state);
                savable_state state = new savable_state();
                state.Assign(m_saved);

                for (int blkn = 0; blkn < m_cinfo.m_blocks_in_MCU; blkn++)
                {
                    int ci = m_cinfo.m_MCU_membership[blkn];
                    int s;
                    if (!HUFF_DECODE(out s, ref br_state, m_derived_tbls[m_cinfo.Comp_info[m_cinfo.m_cur_comp_info[ci]].Dc_tbl_no], ref get_buffer, ref bits_left))
                        return false;

                    if (s != 0)
                    {
                        if (!CHECK_BIT_BUFFER(ref br_state, s, ref get_buffer, ref bits_left))
                            return false;

                        int r = GET_BITS(s, get_buffer, ref bits_left);
                        s = HUFF_EXTEND(r, s);
                    }

                    s += state.last_dc_val[ci];
                    state.last_dc_val[ci] = s;

                    MCU_data[blkn][0] = (short)(s << m_cinfo.m_Al);
                }

                BITREAD_SAVE_STATE(ref m_bitstate, get_buffer, bits_left);
                m_saved.Assign(state);
            }

            m_restarts_to_go--;

            return true;
        }

        /// <summary>
        /// MCU decoding for AC initial scan (either spectral selection,
        /// or first pass of successive approximation).
        /// </summary>
        private bool decode_mcu_AC_first(JBLOCK[] MCU_data)
        {
            if (m_cinfo.m_restart_interval != 0)
            {
                if (m_restarts_to_go == 0)
                {
                    if (!process_restart())
                        return false;
                }
            }

            if (!m_insufficient_data)
            {
                int EOBRUN = m_saved.EOBRUN; /* only part of saved state we need */
                if (EOBRUN > 0)
                {
                    EOBRUN--;
                }
                else
                {
                    int get_buffer;
                    int bits_left;
                    BitReadWorkingState br_state = new BitReadWorkingState();
                    BITREAD_LOAD_STATE(m_bitstate, out get_buffer, out bits_left, ref br_state);

                    for (int k = m_cinfo.m_Ss; k <= m_cinfo.m_Se; k++)
                    {
                        int s;
                        if (!HUFF_DECODE(out s, ref br_state, m_ac_derived_tbl, ref get_buffer, ref bits_left))
                            return false;

                        int r = s >> 4;
                        s &= 15;
                        if (s != 0)
                        {
                            k += r;

                            if (!CHECK_BIT_BUFFER(ref br_state, s, ref get_buffer, ref bits_left))
                                return false;

                            r = GET_BITS(s, get_buffer, ref bits_left);
                            s = HUFF_EXTEND(r, s);

                            MCU_data[0][JpegUtils.jpeg_natural_order[k]] = (short)(s << m_cinfo.m_Al);
                        }
                        else
                        {
                            if (r == 15)
                            {
                                k += 15;        /* skip 15 zeroes in band */
                            }
                            else
                            {
                                EOBRUN = 1 << r;
                                if (r != 0)
                                {
                                    if (!CHECK_BIT_BUFFER(ref br_state, r, ref get_buffer, ref bits_left))
                                        return false;

                                    r = GET_BITS(r, get_buffer, ref bits_left);
                                    EOBRUN += r;
                                }

                                EOBRUN--;
                                break;
                            }
                        }
                    }

                    BITREAD_SAVE_STATE(ref m_bitstate, get_buffer, bits_left);
                }

                m_saved.EOBRUN = EOBRUN;
            }

            m_restarts_to_go--;

            return true;
        }

        /// <summary>
        /// MCU decoding for DC successive approximation refinement scan.
        /// </summary>
        private bool decode_mcu_DC_refine(JBLOCK[] MCU_data)
        {
            if (m_cinfo.m_restart_interval != 0)
            {
                if (m_restarts_to_go == 0)
                {
                    if (!process_restart())
                        return false;
                }
            }

            int get_buffer;
            int bits_left;
            BitReadWorkingState br_state = new BitReadWorkingState();
            BITREAD_LOAD_STATE(m_bitstate, out get_buffer, out bits_left, ref br_state);

            for (int blkn = 0; blkn < m_cinfo.m_blocks_in_MCU; blkn++)
            {
                if (!CHECK_BIT_BUFFER(ref br_state, 1, ref get_buffer, ref bits_left))
                    return false;

                if (GET_BITS(1, get_buffer, ref bits_left) != 0)
                {
                    MCU_data[blkn][0] |= (short)(1 << m_cinfo.m_Al);
                }
            }

            BITREAD_SAVE_STATE(ref m_bitstate, get_buffer, bits_left);

            m_restarts_to_go--;

            return true;
        }

        private bool decode_mcu_AC_refine(JBLOCK[] MCU_data)
        {
            int p1 = 1 << m_cinfo.m_Al;    /* 1 in the bit position being coded */
            int m1 = -1 << m_cinfo.m_Al; /* -1 in the bit position being coded */

            if (m_cinfo.m_restart_interval != 0)
            {
                if (m_restarts_to_go == 0)
                {
                    if (!process_restart())
                        return false;
                }
            }

            if (!m_insufficient_data)
            {
                int get_buffer;
                int bits_left;
                BitReadWorkingState br_state = new BitReadWorkingState();
                BITREAD_LOAD_STATE(m_bitstate, out get_buffer, out bits_left, ref br_state);
                int EOBRUN = m_saved.EOBRUN; /* only part of saved state we need */

                int num_newnz = 0;
                int[] newnz_pos = new int[JpegConstants.DCTSIZE2];

                int k = m_cinfo.m_Ss;

                if (EOBRUN == 0)
                {
                    for (; k <= m_cinfo.m_Se; k++)
                    {
                        int s;
                        if (!HUFF_DECODE(out s, ref br_state, m_ac_derived_tbl, ref get_buffer, ref bits_left))
                        {
                            undo_decode_mcu_AC_refine(MCU_data, newnz_pos, num_newnz);
                            return false;
                        }

                        int r = s >> 4;
                        s &= 15;
                        if (s != 0)
                        {
                            if (s != 1)
                            {
                            }

                            if (!CHECK_BIT_BUFFER(ref br_state, 1, ref get_buffer, ref bits_left))
                            {
                                undo_decode_mcu_AC_refine(MCU_data, newnz_pos, num_newnz);
                                return false;
                            }

                            if (GET_BITS(1, get_buffer, ref bits_left) != 0)
                            {
                                s = p1;
                            }
                            else
                            {
                                s = m1;
                            }
                        }
                        else
                        {
                            if (r != 15)
                            {
                                EOBRUN = 1 << r;
                                if (r != 0)
                                {
                                    if (!CHECK_BIT_BUFFER(ref br_state, r, ref get_buffer, ref bits_left))
                                    {
                                        undo_decode_mcu_AC_refine(MCU_data, newnz_pos, num_newnz);
                                        return false;
                                    }

                                    r = GET_BITS(r, get_buffer, ref bits_left);
                                    EOBRUN += r;
                                }
                                break;      /* rest of block is handled by EOB logic */
                            }
                        }

                        do
                        {
                            int blockIndex = JpegUtils.jpeg_natural_order[k];
                            short thiscoef = MCU_data[0][blockIndex];
                            if (thiscoef != 0)
                            {
                                if (!CHECK_BIT_BUFFER(ref br_state, 1, ref get_buffer, ref bits_left))
                                {
                                    undo_decode_mcu_AC_refine(MCU_data, newnz_pos, num_newnz);
                                    return false;
                                }

                                if (GET_BITS(1, get_buffer, ref bits_left) != 0)
                                {
                                    if ((thiscoef & p1) == 0)
                                    {
                                        /* do nothing if already set it */
                                        if (thiscoef >= 0)
                                            MCU_data[0][blockIndex] += (short)p1;
                                        else
                                            MCU_data[0][blockIndex] += (short)m1;
                                    }
                                }
                            }
                            else
                            {
                                if (--r < 0)
                                    break;      /* reached target zero coefficient */
                            }

                            k++;
                        }
                        while (k <= m_cinfo.m_Se);

                        if (s != 0)
                        {
                            int pos = JpegUtils.jpeg_natural_order[k];

                            MCU_data[0][pos] = (short)s;

                            newnz_pos[num_newnz++] = pos;
                        }
                    }
                }

                if (EOBRUN > 0)
                {
                    for (; k <= m_cinfo.m_Se; k++)
                    {
                        int blockIndex = JpegUtils.jpeg_natural_order[k];
                        short thiscoef = MCU_data[0][blockIndex];
                        if (thiscoef != 0)
                        {
                            if (!CHECK_BIT_BUFFER(ref br_state, 1, ref get_buffer, ref bits_left))
                            {
                                undo_decode_mcu_AC_refine(MCU_data, newnz_pos, num_newnz);
                                return false;
                            }

                            if (GET_BITS(1, get_buffer, ref bits_left) != 0)
                            {
                                if ((thiscoef & p1) == 0)
                                {
                                    /* do nothing if already changed it */
                                    if (thiscoef >= 0)
                                        MCU_data[0][blockIndex] += (short)p1;
                                    else
                                        MCU_data[0][blockIndex] += (short)m1;
                                }
                            }
                        }
                    }

                    EOBRUN--;
                }

                BITREAD_SAVE_STATE(ref m_bitstate, get_buffer, bits_left);
                m_saved.EOBRUN = EOBRUN; /* only part of saved state we need */
            }

            m_restarts_to_go--;

            return true;
        }

        /// <summary>
        /// Check for a restart marker and resynchronize decoder.
        /// </summary>
        private bool process_restart()
        {
            m_cinfo.m_marker.SkipBytes(m_bitstate.bits_left / 8);
            m_bitstate.bits_left = 0;
            if (!m_cinfo.m_marker.read_restart_marker())
                return false;

            for (int ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
                m_saved.last_dc_val[ci] = 0;

            m_saved.EOBRUN = 0;
            m_restarts_to_go = m_cinfo.m_restart_interval;

            if (m_cinfo.m_unread_marker == 0)
                m_insufficient_data = false;

            return true;
        }

        /// <summary>
        /// MCU decoding for AC successive approximation refinement scan.
        /// </summary>
        private static void undo_decode_mcu_AC_refine(JBLOCK[] block, int[] newnz_pos, int num_newnz)
        {
            while (num_newnz > 0)
                block[0][newnz_pos[--num_newnz]] = 0;
        }
    }
}
