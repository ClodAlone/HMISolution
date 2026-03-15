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

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    /// <summary>
    /// </summary>
    class Ex1PassCQuantizer : ColorQuantizer
    {
        private enum QuantizerType
        {
            color_quantizer3,
            color_quantizer,
            quantize3_ord_dither_quantizer,
            quantize_ord_dither_quantizer,
            quantize_fs_dither_quantizer
        }

        private static readonly int[] RGB_order = { JpegConstants.RGB_GREEN, JpegConstants.RGB_RED, JpegConstants.RGB_BLUE };
        private const int MAX_Q_COMPS = 4;
        private const int ODITHER_SIZE = 16;
        private const int ODITHER_CELLS = (ODITHER_SIZE * ODITHER_SIZE); /* # cells in matrix */
        private const int ODITHER_MASK = (ODITHER_SIZE-1); 
        private static readonly byte[][] base_dither_matrix = new byte[][] 
        {
            new byte[] {   0, 192, 48, 240, 12, 204, 60, 252,  3, 195, 51, 243, 15, 207, 63, 255 },
            new byte[] { 128, 64, 176, 112, 140, 76, 188, 124, 131, 67, 179, 115, 143, 79, 191, 127 },
            new byte[] {  32, 224, 16, 208, 44, 236, 28, 220, 35, 227, 19, 211, 47, 239, 31, 223 },
            new byte[] { 160, 96, 144, 80, 172, 108, 156, 92, 163, 99, 147, 83, 175, 111, 159, 95 },
            new byte[] {   8, 200, 56, 248,  4, 196, 52, 244, 11, 203, 59, 251,  7, 199, 55, 247 },
            new byte[] { 136, 72, 184, 120, 132, 68, 180, 116, 139, 75, 187, 123, 135, 71, 183, 119 },
            new byte[] {  40, 232, 24, 216, 36, 228, 20, 212, 43, 235, 27, 219, 39, 231, 23, 215 },
            new byte[] { 168, 104, 152, 88, 164, 100, 148, 84, 171, 107, 155, 91, 167, 103, 151, 87 },
            new byte[] {   2, 194, 50, 242, 14, 206, 62, 254,  1, 193, 49, 241, 13, 205, 61, 253 },
            new byte[] { 130, 66, 178, 114, 142, 78, 190, 126, 129, 65, 177, 113, 141, 77, 189, 125 },
            new byte[] {  34, 226, 18, 210, 46, 238, 30, 222, 33, 225, 17, 209, 45, 237, 29, 221 },
            new byte[] { 162, 98, 146, 82, 174, 110, 158, 94, 161, 97, 145, 81, 173, 109, 157, 93 },
            new byte[] {  10, 202, 58, 250,  6, 198, 54, 246,  9, 201, 57, 249,  5, 197, 53, 245 },
            new byte[] { 138, 74, 186, 122, 134, 70, 182, 118, 137, 73, 185, 121, 133, 69, 181, 117 },
            new byte[] {  42, 234, 26, 218, 38, 230, 22, 214, 41, 233, 25, 217, 37, 229, 21, 213 },
            new byte[] { 170, 106, 154, 90, 166, 102, 150, 86, 169, 105, 153, 89, 165, 101, 149, 85 }
        };

        private QuantizerType m_quantizer;
        private DecompressStruct m_cinfo;
        private byte[][] m_sv_colormap;
        private int m_sv_actual;	
        private byte[][] m_colorindex;
        private int[] m_colorindexOffset;
        private bool m_is_padded;	
        private int[] m_Ncolors = new int[MAX_Q_COMPS];
        private int m_row_index;	
        private int[][][] m_odither = new int[MAX_Q_COMPS][][];
        private short[][] m_fserrors = new short[MAX_Q_COMPS][];
        private bool m_on_odd_row;

        /// <summary>
        /// Module initialization routine for 1-pass color quantization.
        /// </summary>
        public Ex1PassCQuantizer(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            m_fserrors[0] = null; /* Flag FS workspace not allocated */
            m_odither[0] = null;    /* Also flag odither arrays not allocated */

            create_colormap();
            create_colorindex();

            if (cinfo.m_dither_mode == J_DITHER_MODE.JDITHER_FS)
                alloc_fs_workspace();
        }

        /// <summary>
        /// Initialize for one-pass color quantization.
        /// </summary>
        public virtual void start_pass(bool is_pre_scan)
        {
            m_cinfo.m_colormap = m_sv_colormap;
            m_cinfo.m_actual_number_of_colors = m_sv_actual;

            switch (m_cinfo.m_dither_mode)
            {
                case J_DITHER_MODE.JDITHER_NONE:
                    if (m_cinfo.m_out_color_components == 3)
                        m_quantizer = QuantizerType.color_quantizer3;
                    else
                        m_quantizer = QuantizerType.color_quantizer;

                    break;
                case J_DITHER_MODE.JDITHER_ORDERED:
                    if (m_cinfo.m_out_color_components == 3)
                        m_quantizer = QuantizerType.quantize3_ord_dither_quantizer;
                    else
                        m_quantizer = QuantizerType.quantize3_ord_dither_quantizer;

                    m_row_index = 0;
                    if (!m_is_padded)
                        create_colorindex();

                    if (m_odither[0] == null)
                        create_odither_tables();

                    break;
                case J_DITHER_MODE.JDITHER_FS:
                    m_quantizer = QuantizerType.quantize_fs_dither_quantizer;

                    m_on_odd_row = false;

                    if (m_fserrors[0] == null)
                        alloc_fs_workspace();

                    int arraysize = m_cinfo.m_output_width + 2;
                    for (int i = 0; i < m_cinfo.m_out_color_components; i++)
                        Array.Clear(m_fserrors[i], 0, arraysize);

                    break;
                default:
                    break;
            }
        }

        public virtual void color_quantize(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows)
        {
            switch (m_quantizer)
            {
                case QuantizerType.color_quantizer3:
                    quantize3(input_buf, in_row, output_buf, out_row, num_rows);
                    break;
                case QuantizerType.color_quantizer:
                    quantize(input_buf, in_row, output_buf, out_row, num_rows);
                    break;
                case QuantizerType.quantize3_ord_dither_quantizer:
                    quantize3_ord_dither(input_buf, in_row, output_buf, out_row, num_rows);
                    break;
                case QuantizerType.quantize_ord_dither_quantizer:
                    quantize_ord_dither(input_buf, in_row, output_buf, out_row, num_rows);
                    break;
                case QuantizerType.quantize_fs_dither_quantizer:
                    quantize_fs_dither(input_buf, in_row, output_buf, out_row, num_rows);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Finish up at the end of the pass.
        /// </summary>
        public virtual void finish_pass()
        {
            
        }

        /// <summary>
        /// Switch to a new external colormap between output passes.
        /// </summary>
        public virtual void new_color_map()
        {
        }

        /// <summary>
        /// Map some rows of pixels to the output colormapped representation.
        /// </summary>
        private void quantize(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows)
        {
            int nc = m_cinfo.m_out_color_components;

            for (int row = 0; row < num_rows; row++)
            {
                int inIndex = 0;
                int inRow = in_row + row;

                int outIndex = 0;
                int outRow = out_row + row;

                for (int col = m_cinfo.m_output_width; col > 0; col--)
                {
                    int pixcode = 0;
                    for (int ci = 0; ci < nc; ci++)
                    {
                        pixcode += m_colorindex[ci][m_colorindexOffset[ci] + input_buf[inRow][inIndex]];
                        inIndex++;
                    }

                    output_buf[outRow][outIndex] = (byte)pixcode;
                    outIndex++;
                }
            }
        }

        /// <summary>
        /// Map some rows of pixels to the output colormapped representation.
        /// </summary>
        private void quantize3(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows)
        {
            int width = m_cinfo.m_output_width;

            for (int row = 0; row < num_rows; row++)
            {
                int inIndex = 0;
                int inRow = in_row + row;

                int outIndex = 0;
                int outRow = out_row + row;

                for (int col = width; col > 0; col--)
                {
                    int pixcode = m_colorindex[0][m_colorindexOffset[0] + input_buf[inRow][inIndex]];
                    inIndex++;

                    pixcode += m_colorindex[1][m_colorindexOffset[1] + input_buf[inRow][inIndex]];
                    inIndex++;

                    pixcode += m_colorindex[2][m_colorindexOffset[2] + input_buf[inRow][inIndex]];
                    inIndex++;

                    output_buf[outRow][outIndex] = (byte)pixcode;
                    outIndex++;
                }
            }
        }

        /// <summary>
        /// Map some rows of pixels to the output colormapped representation.
        /// </summary>
        private void quantize_ord_dither(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows)
        {
            int nc = m_cinfo.m_out_color_components;
            int width = m_cinfo.m_output_width;

            for (int row = 0; row < num_rows; row++)
            {
                Array.Clear(output_buf[out_row + row], 0, width);

                int row_index = m_row_index;
                for (int ci = 0; ci < nc; ci++)
                {
                    int inputIndex = ci;
                    int outIndex = 0;
                    int outRow = out_row + row;

                    int col_index = 0;
                    for (int col = width; col > 0; col--)
                    {
                        output_buf[outRow][outIndex] += m_colorindex[ci][m_colorindexOffset[ci] + input_buf[in_row + row][inputIndex] + m_odither[ci][row_index][col_index]];
                        inputIndex += nc;
                        outIndex++;
                        col_index = (col_index + 1) & ODITHER_MASK;
                    }
                }

                row_index = (row_index + 1) & ODITHER_MASK;
                m_row_index = row_index;
            }
        }

        /// <summary>
        /// Map some rows of pixels to the output colormapped representation.
        /// </summary>
        private void quantize3_ord_dither(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows)
        {
            int width = m_cinfo.m_output_width;

            for (int row = 0; row < num_rows; row++)
            {
                int row_index = m_row_index;
                int inRow = in_row + row;
                int inIndex = 0;

                int outIndex = 0;
                int outRow = out_row + row;

                int col_index = 0;
                for (int col = width; col > 0; col--)
                {
                    int pixcode = m_colorindex[0][m_colorindexOffset[0] + input_buf[inRow][inIndex] + m_odither[0][row_index][col_index]];
                    inIndex++;

                    pixcode += m_colorindex[1][m_colorindexOffset[1] + input_buf[inRow][inIndex] + m_odither[1][row_index][col_index]];
                    inIndex++;

                    pixcode += m_colorindex[2][m_colorindexOffset[2] + input_buf[inRow][inIndex] + m_odither[2][row_index][col_index]];
                    inIndex++;

                    output_buf[outRow][outIndex] = (byte)pixcode;
                    outIndex++;

                    col_index = (col_index + 1) & ODITHER_MASK;
                }

                row_index = (row_index + 1) & ODITHER_MASK;
                m_row_index = row_index;
            }
        }

        /// <summary>
        /// Map some rows of pixels to the output colormapped representation.
        /// </summary>
        private void quantize_fs_dither(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows)
        {
            int nc = m_cinfo.m_out_color_components;
            int width = m_cinfo.m_output_width;

            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset;

            for (int row = 0; row < num_rows; row++)
            {
                Array.Clear(output_buf[out_row + row], 0, width);

                for (int ci = 0; ci < nc; ci++)
                {
                    int inRow = in_row + row;
                    int inIndex = ci;

                    int outIndex = 0;
                    int outRow = out_row + row;

                    int errorIndex = 0;
                    int dir;            /* 1 for left-to-right, -1 for right-to-left */
                    if (m_on_odd_row)
                    {
                        /* work right to left in this row */
                        inIndex += (width - 1) * nc; /* so point to rightmost pixel */
                        outIndex += width - 1;
                        dir = -1;
                        errorIndex = width + 1; /* => entry after last column */
                    }
                    else
                    {
                        /* work left to right in this row */
                        dir = 1;
                        errorIndex = 0; /* => entry before first column */
                    }
                    int dirnc = dir * nc;

                    int cur = 0;
                    int belowerr = 0;
                    int bpreverr = 0;

                    for (int col = width; col > 0; col--)
                    {
                        cur = JpegUtils.RIGHT_SHIFT(cur + m_fserrors[ci][errorIndex + dir] + 8, 4);
                        cur += input_buf[inRow][inIndex];
                        cur = limit[limitOffset + cur];

                        int pixcode = m_colorindex[ci][m_colorindexOffset[ci] + cur];
                        output_buf[outRow][outIndex] += (byte)pixcode;
                        
                        cur -= m_sv_colormap[ci][pixcode];
                        
                        int bnexterr = cur;
                        int delta = cur * 2;
                        cur += delta;       /* form error * 3 */
                        m_fserrors[ci][errorIndex + 0] = (short) (bpreverr + cur);
                        cur += delta;       /* form error * 5 */
                        bpreverr = belowerr + cur;
                        belowerr = bnexterr;
                        cur += delta;       /* form error * 7 */
                        
                        inIndex += dirnc; /* advance input to next column */
                        outIndex += dir;  /* advance output to next column */
                        errorIndex += dir;    /* advance errorIndex to current column */
                    }
                    m_fserrors[ci][errorIndex + 0] = (short) bpreverr; /* unload prev err into array */
                }

                m_on_odd_row = (m_on_odd_row ? false : true);
            }
        }

        /// <summary>
        /// Create the colormap.
        /// </summary>
        private void create_colormap()
        {
            int total_colors = select_ncolors(m_Ncolors);
            
            byte[][] colormap = CommonStruct.AllocJpegSamples(total_colors, m_cinfo.m_out_color_components);
            int blkdist = total_colors;
            for (int i = 0; i < m_cinfo.m_out_color_components; i++)
            {
                int nci = m_Ncolors[i]; /* # of distinct values for this color */
                int blksize = blkdist / nci;
                for (int j = 0; j < nci; j++)
                {
                    int val = output_value(j, nci - 1);
                    for (int ptr = j * blksize; ptr < total_colors; ptr += blkdist)
                    {
                        for (int k = 0; k < blksize; k++)
                            colormap[i][ptr + k] = (byte)val;
                    }
                }

                blkdist = blksize;
            }
            m_sv_colormap = colormap;
            m_sv_actual = total_colors;
        }

        /// <summary>
        /// Create the color index table.
        /// </summary>
        private void create_colorindex()
        {
            int pad;
            if (m_cinfo.m_dither_mode == J_DITHER_MODE.JDITHER_ORDERED)
            {
                pad = JpegConstants.MAXJSAMPLE * 2;
                m_is_padded = true;
            }
            else
            {
                pad = 0;
                m_is_padded = false;
            }

            m_colorindex = CommonStruct.AllocJpegSamples(JpegConstants.MAXJSAMPLE + 1 + pad, m_cinfo.m_out_color_components);
            m_colorindexOffset = new int[m_cinfo.m_out_color_components];

            int blksize = m_sv_actual;
            for (int i = 0; i < m_cinfo.m_out_color_components; i++)
            {
                int nci = m_Ncolors[i]; /* # of distinct values for this color */
                blksize = blksize / nci;

                if (pad != 0)
                    m_colorindexOffset[i] += JpegConstants.MAXJSAMPLE;

                int val = 0;
                int k = largest_input_value(0, nci - 1);
                for (int j = 0; j <= JpegConstants.MAXJSAMPLE; j++)
                {
                    while (j > k)
                    {
                        k = largest_input_value(++val, nci - 1);
                    }

                    m_colorindex[i][m_colorindexOffset[i] + j] = (byte)(val * blksize);
                }

                if (pad != 0)
                {
                    for (int j = 1; j <= JpegConstants.MAXJSAMPLE; j++)
                    {
                        m_colorindex[i][m_colorindexOffset[i] + -j] = m_colorindex[i][m_colorindexOffset[i]];
                        m_colorindex[i][m_colorindexOffset[i] + JpegConstants.MAXJSAMPLE + j] = m_colorindex[i][m_colorindexOffset[i] + JpegConstants.MAXJSAMPLE];
                    }
                }
            }
        }

        /// <summary>
        /// Create the ordered-dither tables.
        /// </summary>
        private void create_odither_tables()
        {
            for (int i = 0; i < m_cinfo.m_out_color_components; i++)
            {
                int nci = m_Ncolors[i]; 
                int foundPos = -1;
                for (int j = 0; j < i; j++)
                {
                    if (nci == m_Ncolors[j])
                    {
                        foundPos = j;
                        break;
                    }
                }

                if (foundPos == -1)
                {
                    m_odither[i] = make_odither_array(nci);
                }
                else
                    m_odither[i] = m_odither[foundPos];
            }
        }

        /// <summary>
        /// Allocate workspace for Floyd-Steinberg errors.
        /// </summary>
        private void alloc_fs_workspace()
        {
            for (int i = 0; i < m_cinfo.m_out_color_components; i++)
                m_fserrors[i] = new short[m_cinfo.m_output_width + 2];
        }

        /// <summary>
        /// Return largest input value that should map to j'th output value
        /// </summary>
        private static int largest_input_value(int j, int maxj)
        {
            return (int)(((2 * j + 1) * JpegConstants.MAXJSAMPLE + maxj) / (2 * maxj));
        }

        /// <summary>
        /// Return j'th output value, where j will range from 0 to maxj
        /// </summary>
        private static int output_value(int j, int maxj)
        {
            return (int)((j * JpegConstants.MAXJSAMPLE + maxj / 2) / maxj);
        }

        /// <summary>
        /// Determine allocation of desired colors to components,
        /// and fill in Ncolors[] array to indicate choice.
        /// Return value is total number of colors (product of Ncolors[] values).
        /// </summary>
        private int select_ncolors(int[] Ncolors)
        {
            int nc = m_cinfo.m_out_color_components; /* number of color components */
            int max_colors = m_cinfo.m_desired_number_of_colors;
            
            int iroot = 1;
            long temp = 0;
            do
            {
                iroot++;
                temp = iroot;       /* set temp = iroot ** nc */
                for (int i = 1; i < nc; i++)
                    temp *= iroot;
            }
            while (temp <= max_colors); /* repeat till iroot exceeds root */

            iroot--;

            int total_colors = 1;
            for (int i = 0; i < nc; i++)
            {
                Ncolors[i] = iroot;
                total_colors *= iroot;
            }

            bool changed = false;
            do
            {
                changed = false;
                for (int i = 0; i < nc; i++)
                {
                    int j = (m_cinfo.m_out_color_space == J_COLOR_SPACE.JCS_RGB ? RGB_order[i] : i);
                    /* calculate new total_colors if Ncolors[j] is incremented */
                    temp = total_colors / Ncolors[j];
                    temp *= Ncolors[j] + 1; /* done in long arith to avoid oflo */

                    if (temp > max_colors)
                        break;          /* won't fit, done with this pass */
                    
                    Ncolors[j]++;       /* OK, apply the increment */
                    total_colors = (int)temp;
                    changed = true;
                }
            }
            while (changed);

            return total_colors;
        }

        /// <summary>
        /// Create an ordered-dither array for a component having ncolors
        /// distinct output values.
        /// </summary>
        private static int[][] make_odither_array(int ncolors)
        {
            int[][] odither = new int[ODITHER_SIZE][];
            for (int i = 0; i < ODITHER_SIZE; i++)
                odither[i] = new int[ODITHER_SIZE];

            int den = 2 * ODITHER_CELLS * (ncolors - 1);
            for (int j = 0; j < ODITHER_SIZE; j++)
            {
                for (int k = 0; k < ODITHER_SIZE; k++)
                {
                    int num = ((int)(ODITHER_CELLS - 1 - 2 * ((int)base_dither_matrix[j][k]))) * JpegConstants.MAXJSAMPLE;
                    odither[j][k] = num < 0 ? -((-num) / den) : num / den;
                }
            }

            return odither;
        }
    }

    /// <summary>
    /// </summary>
    class Ex2PassCQuantizer : ColorQuantizer
    {
        private struct box
        {
            public int c0min;
            public int c0max;
            public int c1min;
            public int c1max;
            public int c2min;
            public int c2max;
            public int volume;
            public long colorcount;
        }

        private enum QuantizerType
        {
            prescan_quantizer,
            pass2_fs_dither_quantizer,
            pass2_no_dither_quantizer
        }

        private const int MAXNUMCOLORS = (JpegConstants.MAXJSAMPLE + 1); /* maximum size of colormap */
        private const int HIST_C0_BITS = 5;     /* bits of precision in R/B histogram */
        private const int HIST_C1_BITS = 6;     /* bits of precision in G histogram */
        private const int HIST_C2_BITS = 5;     /* bits of precision in B/R histogram */
        private const int HIST_C0_ELEMS = (1 << HIST_C0_BITS);
        private const int HIST_C1_ELEMS = (1 << HIST_C1_BITS);
        private const int HIST_C2_ELEMS = (1 << HIST_C2_BITS);
        private const int C0_SHIFT = (JpegConstants.BITS_IN_JSAMPLE - HIST_C0_BITS);
        private const int C1_SHIFT = (JpegConstants.BITS_IN_JSAMPLE - HIST_C1_BITS);
        private const int C2_SHIFT = (JpegConstants.BITS_IN_JSAMPLE - HIST_C2_BITS);

        private const int R_SCALE = 2;       /* scale R distances by this much */
        private const int G_SCALE = 3;       /* scale G distances by this much */
        private const int B_SCALE = 1;       /* and B by this much */
        private const int BOX_C0_LOG = (HIST_C0_BITS - 3);
        private const int BOX_C1_LOG = (HIST_C1_BITS - 3);
        private const int BOX_C2_LOG = (HIST_C2_BITS - 3);

        private const int BOX_C0_ELEMS = (1 << BOX_C0_LOG); /* # of hist cells in update box */
        private const int BOX_C1_ELEMS = (1 << BOX_C1_LOG);
        private const int BOX_C2_ELEMS = (1 << BOX_C2_LOG);

        private const int BOX_C0_SHIFT = (C0_SHIFT + BOX_C0_LOG);
        private const int BOX_C1_SHIFT = (C1_SHIFT + BOX_C1_LOG);
        private const int BOX_C2_SHIFT = (C2_SHIFT + BOX_C2_LOG);

        private QuantizerType m_quantizer;

        private bool m_useFinishPass1;

        private DecompressStruct m_cinfo;
        private byte[][] m_sv_colormap;  /* colormap allocated at init time */
        private int m_desired;            /* desired # of colors = size of colormap */
        private ushort[][] m_histogram;     /* pointer to the histogram */

        private bool m_needs_zeroed;      /* true if next pass must zero histogram */
        private short[] m_fserrors;      /* accumulated errors */
        private bool m_on_odd_row;        /* flag to remember which row we are on */
        private int[] m_error_limiter;     /* table for clamping the applied error */

        /// <summary>
        /// Module initialization routine for 2-pass color quantization.
        /// </summary>
        public Ex2PassCQuantizer(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;

            /* Allocate the histogram/inverse colormap storage */
            m_histogram = new ushort[HIST_C0_ELEMS][];
            for (int i = 0; i < HIST_C0_ELEMS; i++)
                m_histogram[i] = new ushort[HIST_C1_ELEMS * HIST_C2_ELEMS];

            m_needs_zeroed = true; /* histogram is garbage now */

            if (cinfo.m_enable_2pass_quant)
            {
                int desired_local = cinfo.m_desired_number_of_colors;

                m_sv_colormap = CommonStruct.AllocJpegSamples(desired_local, 3);
                m_desired = desired_local;
            }

            if (cinfo.m_dither_mode != J_DITHER_MODE.JDITHER_NONE)
                cinfo.m_dither_mode = J_DITHER_MODE.JDITHER_FS;

            if (cinfo.m_dither_mode == J_DITHER_MODE.JDITHER_FS)
            {
                m_fserrors = new short[(cinfo.m_output_width + 2) * 3];

                init_error_limit();
            }
        }

        /// <summary>
        /// Initialize for each processing pass.
        /// </summary>
        public virtual void start_pass(bool is_pre_scan)
        {
            if (m_cinfo.m_dither_mode != J_DITHER_MODE.JDITHER_NONE)
                m_cinfo.m_dither_mode = J_DITHER_MODE.JDITHER_FS;

            if (is_pre_scan)
            {
                m_quantizer = QuantizerType.prescan_quantizer;
                m_useFinishPass1 = true;
                m_needs_zeroed = true; /* Always zero histogram */
            }
            else
            {
                if (m_cinfo.m_dither_mode == J_DITHER_MODE.JDITHER_FS)
                    m_quantizer = QuantizerType.pass2_fs_dither_quantizer;
                else
                    m_quantizer = QuantizerType.pass2_no_dither_quantizer;

                m_useFinishPass1 = false;
                int i = m_cinfo.m_actual_number_of_colors;

                if (m_cinfo.m_dither_mode == J_DITHER_MODE.JDITHER_FS)
                {
                    /* Allocate Floyd-Steinberg workspace if we didn't already. */
                    if (m_fserrors == null)
                    {
                        int arraysize = (m_cinfo.m_output_width + 2) * 3;
                        m_fserrors = new short[arraysize];
                    }
                    else
                    {
                        /* Initialize the propagated errors to zero. */
                        Array.Clear(m_fserrors, 0, m_fserrors.Length);
                    }

                    /* Make the error-limit table if we didn't already. */
                    if (m_error_limiter == null)
                        init_error_limit();

                    m_on_odd_row = false;
                }
            }

            /* Zero the histogram or inverse color map, if necessary */
            if (m_needs_zeroed)
            {
                for (int i = 0; i < HIST_C0_ELEMS; i++)
                    Array.Clear(m_histogram[i], 0, m_histogram[i].Length);

                m_needs_zeroed = false;
            }
        }

        public virtual void color_quantize(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows)
        {
            switch (m_quantizer)
            {
                case QuantizerType.prescan_quantizer:
                    prescan_quantize(input_buf, in_row, num_rows);
                    break;
                case QuantizerType.pass2_fs_dither_quantizer:
                    pass2_fs_dither(input_buf, in_row, output_buf, out_row, num_rows);
                    break;
                case QuantizerType.pass2_no_dither_quantizer:
                    pass2_no_dither(input_buf, in_row, output_buf, out_row, num_rows);
                    break;
                default:
                    break;
            }
        }

        public virtual void finish_pass()
        {
            if (m_useFinishPass1)
                finish_pass1();
        }

        /// <summary>
        /// Switch to a new external colormap between output passes.
        /// </summary>
        public virtual void new_color_map()
        {
            m_needs_zeroed = true;
        }

        /// <summary>
        /// Prescan some rows of pixels.
        /// </summary>
        private void prescan_quantize(byte[][] input_buf, int in_row, int num_rows)
        {
            for (int row = 0; row < num_rows; row++)
            {
                int inputIndex = 0;
                for (int col = m_cinfo.m_output_width; col > 0; col--)
                {
                    int rowIndex = (int)input_buf[in_row + row][inputIndex] >> C0_SHIFT;
                    int columnIndex = ((int)input_buf[in_row + row][inputIndex + 1] >> C1_SHIFT) * HIST_C2_ELEMS +
                        ((int)input_buf[in_row + row][inputIndex + 2] >> C2_SHIFT);

                    /* increment pixel value, check for overflow and undo increment if so. */
                    m_histogram[rowIndex][columnIndex]++;
                    if (m_histogram[rowIndex][columnIndex] <= 0)
                        m_histogram[rowIndex][columnIndex]--;

                    inputIndex += 3;
                }
            }
        }

        /// <summary>
        /// Map some rows of pixels to the output colormapped representation.
        /// </summary>
        private void pass2_fs_dither(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows)
        {
            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset;

            for (int row = 0; row < num_rows; row++)
            {
                int inputPixelIndex = 0;
                int outputPixelIndex = 0;
                int errorIndex = 0;
                int dir;
                int dir3;
                if (m_on_odd_row)
                {
                    /* work right to left in this row */
                    inputPixelIndex += (m_cinfo.m_output_width - 1) * 3;   /* so point to rightmost pixel */
                    outputPixelIndex += m_cinfo.m_output_width - 1;
                    dir = -1;
                    dir3 = -3;
                    errorIndex = (m_cinfo.m_output_width + 1) * 3; /* => entry after last column */
                    m_on_odd_row = false; /* flip for next time */
                }
                else
                {
                    dir = 1;
                    dir3 = 3;
                    errorIndex = 0; /* => entry before first real column */
                    m_on_odd_row = true; /* flip for next time */
                }

                int cur0 = 0;
                int cur1 = 0;
                int cur2 = 0;
                int belowerr0 = 0;
                int belowerr1 = 0;
                int belowerr2 = 0;
                int bpreverr0 = 0;
                int bpreverr1 = 0;
                int bpreverr2 = 0;

                for (int col = m_cinfo.m_output_width; col > 0; col--)
                {
                    cur0 = JpegUtils.RIGHT_SHIFT(cur0 + m_fserrors[errorIndex + dir3] + 8, 4);
                    cur1 = JpegUtils.RIGHT_SHIFT(cur1 + m_fserrors[errorIndex + dir3 + 1] + 8, 4);
                    cur2 = JpegUtils.RIGHT_SHIFT(cur2 + m_fserrors[errorIndex + dir3 + 2] + 8, 4);

                    cur0 = m_error_limiter[JpegConstants.MAXJSAMPLE + cur0];
                    cur1 = m_error_limiter[JpegConstants.MAXJSAMPLE + cur1];
                    cur2 = m_error_limiter[JpegConstants.MAXJSAMPLE + cur2];

                    cur0 += input_buf[in_row + row][inputPixelIndex];
                    cur1 += input_buf[in_row + row][inputPixelIndex + 1];
                    cur2 += input_buf[in_row + row][inputPixelIndex + 2];
                    cur0 = limit[limitOffset + cur0];
                    cur1 = limit[limitOffset + cur1];
                    cur2 = limit[limitOffset + cur2];

                    /* Index into the cache with adjusted pixel value */
                    int hRow = cur0 >> C0_SHIFT;
                    int hColumn = (cur1 >> C1_SHIFT) * HIST_C2_ELEMS + (cur2 >> C2_SHIFT);

                    if (m_histogram[hRow][hColumn] == 0)
                        fill_inverse_cmap(cur0 >> C0_SHIFT, cur1 >> C1_SHIFT, cur2 >> C2_SHIFT);

                    /* Now emit the colormap index for this cell */
                    int pixcode = m_histogram[hRow][hColumn] - 1;
                    output_buf[out_row + row][outputPixelIndex] = (byte)pixcode;

                    /* Compute representation error for this pixel */
                    cur0 -= m_cinfo.m_colormap[0][pixcode];
                    cur1 -= m_cinfo.m_colormap[1][pixcode];
                    cur2 -= m_cinfo.m_colormap[2][pixcode];

                    int bnexterr = cur0;    /* Process component 0 */
                    int delta = cur0 * 2;
                    cur0 += delta;      /* form error * 3 */
                    m_fserrors[errorIndex] = (short)(bpreverr0 + cur0);
                    cur0 += delta;      /* form error * 5 */
                    bpreverr0 = belowerr0 + cur0;
                    belowerr0 = bnexterr;
                    cur0 += delta;      /* form error * 7 */
                    bnexterr = cur1;    /* Process component 1 */
                    delta = cur1 * 2;
                    cur1 += delta;      /* form error * 3 */
                    m_fserrors[errorIndex + 1] = (short)(bpreverr1 + cur1);
                    cur1 += delta;      /* form error * 5 */
                    bpreverr1 = belowerr1 + cur1;
                    belowerr1 = bnexterr;
                    cur1 += delta;      /* form error * 7 */
                    bnexterr = cur2;    /* Process component 2 */
                    delta = cur2 * 2;
                    cur2 += delta;      /* form error * 3 */
                    m_fserrors[errorIndex + 2] = (short)(bpreverr2 + cur2);
                    cur2 += delta;      /* form error * 5 */
                    bpreverr2 = belowerr2 + cur2;
                    belowerr2 = bnexterr;
                    cur2 += delta;      /* form error * 7 */

                    inputPixelIndex += dir3;      /* Advance pixel pointers to next column */
                    outputPixelIndex += dir;
                    errorIndex += dir3;       /* advance errorIndex to current column */
                }

                m_fserrors[errorIndex] = (short)bpreverr0; /* unload prev errs into array */
                m_fserrors[errorIndex + 1] = (short)bpreverr1;
                m_fserrors[errorIndex + 2] = (short)bpreverr2;
            }
        }

        /// <summary>
        /// Map some rows of pixels to the output colormapped representation.
        /// </summary>
        private void pass2_no_dither(byte[][] input_buf, int in_row, byte[][] output_buf, int out_row, int num_rows)
        {
            for (int row = 0; row < num_rows; row++)
            {
                int inRow = row + in_row;
                int inIndex = 0;
                int outIndex = 0;
                int outRow = out_row + row;
                for (int col = m_cinfo.m_output_width; col > 0; col--)
                {
                    /* get pixel value and index into the cache */
                    int c0 = (int)input_buf[inRow][inIndex] >> C0_SHIFT;
                    inIndex++;

                    int c1 = (int)input_buf[inRow][inIndex] >> C1_SHIFT;
                    inIndex++;

                    int c2 = (int)input_buf[inRow][inIndex] >> C2_SHIFT;
                    inIndex++;

                    int hRow = c0;
                    int hColumn = c1 * HIST_C2_ELEMS + c2;

                    if (m_histogram[hRow][hColumn] == 0)
                        fill_inverse_cmap(c0, c1, c2);

                    /* Now emit the colormap index for this cell */
                    output_buf[outRow][outIndex] = (byte)(m_histogram[hRow][hColumn] - 1);
                    outIndex++;
                }
            }
        }

        /// <summary>
        /// Finish up at the end of each pass.
        /// </summary>
        private void finish_pass1()
        {
            m_cinfo.m_colormap = m_sv_colormap;
            select_colors(m_desired);

            m_needs_zeroed = true;
        }

        /// <summary>
        /// Compute representative color for a box, put it in colormap[icolor]
        /// </summary>
        private void compute_color(box[] boxlist, int boxIndex, int icolor)
        {
            long total = 0;
            long c0total = 0;
            long c1total = 0;
            long c2total = 0;
            box curBox = boxlist[boxIndex];
            for (int c0 = curBox.c0min; c0 <= curBox.c0max; c0++)
            {
                for (int c1 = curBox.c1min; c1 <= curBox.c1max; c1++)
                {
                    int histogramIndex = c1 * HIST_C2_ELEMS + curBox.c2min;
                    for (int c2 = curBox.c2min; c2 <= curBox.c2max; c2++)
                    {
                        long count = m_histogram[c0][histogramIndex];
                        histogramIndex++;

                        if (count != 0)
                        {
                            total += count;
                            c0total += ((c0 << C0_SHIFT) + ((1 << C0_SHIFT) >> 1)) * count;
                            c1total += ((c1 << C1_SHIFT) + ((1 << C1_SHIFT) >> 1)) * count;
                            c2total += ((c2 << C2_SHIFT) + ((1 << C2_SHIFT) >> 1)) * count;
                        }
                    }
                }
            }

            m_cinfo.m_colormap[0][icolor] = (byte)((c0total + (total >> 1)) / total);
            m_cinfo.m_colormap[1][icolor] = (byte)((c1total + (total >> 1)) / total);
            m_cinfo.m_colormap[2][icolor] = (byte)((c2total + (total >> 1)) / total);
        }

        /// <summary>
        /// Master routine for color selection
        /// </summary>
        private void select_colors(int desired_colors)
        {
            box[] boxlist = new box[desired_colors];

            int numboxes = 1;
            boxlist[0].c0min = 0;
            boxlist[0].c0max = JpegConstants.MAXJSAMPLE >> C0_SHIFT;
            boxlist[0].c1min = 0;
            boxlist[0].c1max = JpegConstants.MAXJSAMPLE >> C1_SHIFT;
            boxlist[0].c2min = 0;
            boxlist[0].c2max = JpegConstants.MAXJSAMPLE >> C2_SHIFT;

            update_box(boxlist, 0);

            numboxes = median_cut(boxlist, numboxes, desired_colors);

            for (int i = 0; i < numboxes; i++)
                compute_color(boxlist, i, i);

            m_cinfo.m_actual_number_of_colors = numboxes;
        }

        /// <summary>
        /// Repeatedly select and split the largest box until we have enough boxes
        /// </summary>
        private int median_cut(box[] boxlist, int numboxes, int desired_colors)
        {
            while (numboxes < desired_colors)
            {
                int foundIndex;
                if (numboxes * 2 <= desired_colors)
                    foundIndex = find_biggest_color_pop(boxlist, numboxes);
                else
                    foundIndex = find_biggest_volume(boxlist, numboxes);

                if (foundIndex == -1)     /* no splittable boxes left! */
                    break;

                /* Copy the color bounds to the new box. */
                boxlist[numboxes].c0max = boxlist[foundIndex].c0max;
                boxlist[numboxes].c1max = boxlist[foundIndex].c1max;
                boxlist[numboxes].c2max = boxlist[foundIndex].c2max;
                boxlist[numboxes].c0min = boxlist[foundIndex].c0min;
                boxlist[numboxes].c1min = boxlist[foundIndex].c1min;
                boxlist[numboxes].c2min = boxlist[foundIndex].c2min;

                int c0 = ((boxlist[foundIndex].c0max - boxlist[foundIndex].c0min) << C0_SHIFT) * R_SCALE;
                int c1 = ((boxlist[foundIndex].c1max - boxlist[foundIndex].c1min) << C1_SHIFT) * G_SCALE;
                int c2 = ((boxlist[foundIndex].c2max - boxlist[foundIndex].c2min) << C2_SHIFT) * B_SCALE;

                int cmax = c1;
                int n = 1;

                if (c0 > cmax)
                {
                    cmax = c0;
                    n = 0;
                }

                if (c2 > cmax)
                {
                    n = 2;
                }

                int lb;
                switch (n)
                {
                    case 0:
                        lb = (boxlist[foundIndex].c0max + boxlist[foundIndex].c0min) / 2;
                        boxlist[foundIndex].c0max = lb;
                        boxlist[numboxes].c0min = lb + 1;
                        break;
                    case 1:
                        lb = (boxlist[foundIndex].c1max + boxlist[foundIndex].c1min) / 2;
                        boxlist[foundIndex].c1max = lb;
                        boxlist[numboxes].c1min = lb + 1;
                        break;
                    case 2:
                        lb = (boxlist[foundIndex].c2max + boxlist[foundIndex].c2min) / 2;
                        boxlist[foundIndex].c2max = lb;
                        boxlist[numboxes].c2min = lb + 1;
                        break;
                }

                update_box(boxlist, foundIndex);
                update_box(boxlist, numboxes);
                numboxes++;
            }

            return numboxes;
        }

        /// <summary>
        /// Find the splittable box with the largest color population
        /// </summary>
        private static int find_biggest_color_pop(box[] boxlist, int numboxes)
        {
            long maxc = 0;
            int which = -1;
            for (int i = 0; i < numboxes; i++)
            {
                if (boxlist[i].colorcount > maxc && boxlist[i].volume > 0)
                {
                    which = i;
                    maxc = boxlist[i].colorcount;
                }
            }

            return which;
        }

        /// <summary>
        /// Find the splittable box with the largest (scaled) volume
        /// </summary>
        private static int find_biggest_volume(box[] boxlist, int numboxes)
        {
            int maxv = 0;
            int which = -1;
            for (int i = 0; i < numboxes; i++)
            {
                if (boxlist[i].volume > maxv)
                {
                    which = i;
                    maxv = boxlist[i].volume;
                }
            }

            return which;
        }

        /// <summary>
        /// Shrink the min/max bounds of a box to enclose only nonzero elements,
        /// and recompute its volume and population
        /// </summary>
        private void update_box(box[] boxlist, int boxIndex)
        {
            box curBox = boxlist[boxIndex];
            bool have_c0min = false;

            if (curBox.c0max > curBox.c0min)
            {
                for (int c0 = curBox.c0min; c0 <= curBox.c0max; c0++)
                {
                    for (int c1 = curBox.c1min; c1 <= curBox.c1max; c1++)
                    {
                        int histogramIndex = c1 * HIST_C2_ELEMS + curBox.c2min;
                        for (int c2 = curBox.c2min; c2 <= curBox.c2max; c2++)
                        {
                            if (m_histogram[c0][histogramIndex++] != 0)
                            {
                                curBox.c0min = c0;
                                have_c0min = true;
                                break;
                            }
                        }

                        if (have_c0min)
                            break;
                    }

                    if (have_c0min)
                        break;
                }
            }

            bool have_c0max = false;
            if (curBox.c0max > curBox.c0min)
            {
                for (int c0 = curBox.c0max; c0 >= curBox.c0min; c0--)
                {
                    for (int c1 = curBox.c1min; c1 <= curBox.c1max; c1++)
                    {
                        int histogramIndex = c1 * HIST_C2_ELEMS + curBox.c2min;
                        for (int c2 = curBox.c2min; c2 <= curBox.c2max; c2++)
                        {
                            if (m_histogram[c0][histogramIndex++] != 0)
                            {
                                curBox.c0max = c0;
                                have_c0max = true;
                                break;
                            }
                        }

                        if (have_c0max)
                            break;
                    }

                    if (have_c0max)
                        break;
                }
            }

            bool have_c1min = false;
            if (curBox.c1max > curBox.c1min)
            {
                for (int c1 = curBox.c1min; c1 <= curBox.c1max; c1++)
                {
                    for (int c0 = curBox.c0min; c0 <= curBox.c0max; c0++)
                    {
                        int histogramIndex = c1 * HIST_C2_ELEMS + curBox.c2min;
                        for (int c2 = curBox.c2min; c2 <= curBox.c2max; c2++)
                        {
                            if (m_histogram[c0][histogramIndex++] != 0)
                            {
                                curBox.c1min = c1;
                                have_c1min = true;
                                break;
                            }
                        }

                        if (have_c1min)
                            break;
                    }

                    if (have_c1min)
                        break;
                }
            }

            bool have_c1max = false;
            if (curBox.c1max > curBox.c1min)
            {
                for (int c1 = curBox.c1max; c1 >= curBox.c1min; c1--)
                {
                    for (int c0 = curBox.c0min; c0 <= curBox.c0max; c0++)
                    {
                        int histogramIndex = c1 * HIST_C2_ELEMS + curBox.c2min;
                        for (int c2 = curBox.c2min; c2 <= curBox.c2max; c2++)
                        {
                            if (m_histogram[c0][histogramIndex++] != 0)
                            {
                                curBox.c1max = c1;
                                have_c1max = true;
                                break;
                            }
                        }

                        if (have_c1max)
                            break;
                    }

                    if (have_c1max)
                        break;
                }
            }

            bool have_c2min = false;
            if (curBox.c2max > curBox.c2min)
            {
                for (int c2 = curBox.c2min; c2 <= curBox.c2max; c2++)
                {
                    for (int c0 = curBox.c0min; c0 <= curBox.c0max; c0++)
                    {
                        int histogramIndex = curBox.c1min * HIST_C2_ELEMS + c2;
                        for (int c1 = curBox.c1min; c1 <= curBox.c1max; c1++, histogramIndex += HIST_C2_ELEMS)
                        {
                            if (m_histogram[c0][histogramIndex] != 0)
                            {
                                curBox.c2min = c2;
                                have_c2min = true;
                                break;
                            }
                        }

                        if (have_c2min)
                            break;
                    }

                    if (have_c2min)
                        break;
                }
            }

            bool have_c2max = false;
            if (curBox.c2max > curBox.c2min)
            {
                for (int c2 = curBox.c2max; c2 >= curBox.c2min; c2--)
                {
                    for (int c0 = curBox.c0min; c0 <= curBox.c0max; c0++)
                    {
                        int histogramIndex = curBox.c1min * HIST_C2_ELEMS + c2;
                        for (int c1 = curBox.c1min; c1 <= curBox.c1max; c1++, histogramIndex += HIST_C2_ELEMS)
                        {
                            if (m_histogram[c0][histogramIndex] != 0)
                            {
                                curBox.c2max = c2;
                                have_c2max = true;
                                break;
                            }
                        }

                        if (have_c2max)
                            break;
                    }

                    if (have_c2max)
                        break;
                }
            }

            int dist0 = ((curBox.c0max - curBox.c0min) << C0_SHIFT) * R_SCALE;
            int dist1 = ((curBox.c1max - curBox.c1min) << C1_SHIFT) * G_SCALE;
            int dist2 = ((curBox.c2max - curBox.c2min) << C2_SHIFT) * B_SCALE;
            curBox.volume = dist0 * dist0 + dist1 * dist1 + dist2 * dist2;

            long ccount = 0;
            for (int c0 = curBox.c0min; c0 <= curBox.c0max; c0++)
            {
                for (int c1 = curBox.c1min; c1 <= curBox.c1max; c1++)
                {
                    int histogramIndex = c1 * HIST_C2_ELEMS + curBox.c2min;
                    for (int c2 = curBox.c2min; c2 <= curBox.c2max; c2++, histogramIndex++)
                    {
                        if (m_histogram[c0][histogramIndex] != 0)
                            ccount++;
                    }
                }
            }

            curBox.colorcount = ccount;
            boxlist[boxIndex] = curBox;
        }

        /// <summary>
        /// Initialize the error-limiting transfer function (lookup table).
        /// </summary>
        private void init_error_limit()
        {
            m_error_limiter = new int[JpegConstants.MAXJSAMPLE * 2 + 1];
            int tableOffset = JpegConstants.MAXJSAMPLE;

            const int STEPSIZE = ((JpegConstants.MAXJSAMPLE + 1) / 16);

            int output = 0;
            int input = 0;
            for (; input < STEPSIZE; input++, output++)
            {
                m_error_limiter[tableOffset + input] = output;
                m_error_limiter[tableOffset - input] = -output;
            }

            for (; input < STEPSIZE * 3; input++)
            {
                m_error_limiter[tableOffset + input] = output;
                m_error_limiter[tableOffset - input] = -output;
                output += (input & 1) != 0 ? 1 : 0;
            }

            for (; input <= JpegConstants.MAXJSAMPLE; input++)
            {
                m_error_limiter[tableOffset + input] = output;
                m_error_limiter[tableOffset - input] = -output;
            }
        }

        /// <summary>
        /// </summary>
        private int find_nearby_colors(int minc0, int minc1, int minc2, byte[] colorlist)
        {
            int maxc0 = minc0 + ((1 << BOX_C0_SHIFT) - (1 << C0_SHIFT));
            int centerc0 = (minc0 + maxc0) >> 1;

            int maxc1 = minc1 + ((1 << BOX_C1_SHIFT) - (1 << C1_SHIFT));
            int centerc1 = (minc1 + maxc1) >> 1;

            int maxc2 = minc2 + ((1 << BOX_C2_SHIFT) - (1 << C2_SHIFT));
            int centerc2 = (minc2 + maxc2) >> 1;

            int minmaxdist = 0x7FFFFFFF;
            int[] mindist = new int[MAXNUMCOLORS];    /* min distance to colormap entry i */

            for (int i = 0; i < m_cinfo.m_actual_number_of_colors; i++)
            {
                int x = m_cinfo.m_colormap[0][i];
                int min_dist;
                int max_dist;

                if (x < minc0)
                {
                    int tdist = (x - minc0) * R_SCALE;
                    min_dist = tdist * tdist;
                    tdist = (x - maxc0) * R_SCALE;
                    max_dist = tdist * tdist;
                }
                else if (x > maxc0)
                {
                    int tdist = (x - maxc0) * R_SCALE;
                    min_dist = tdist * tdist;
                    tdist = (x - minc0) * R_SCALE;
                    max_dist = tdist * tdist;
                }
                else
                {
                    min_dist = 0;
                    if (x <= centerc0)
                    {
                        int tdist = (x - maxc0) * R_SCALE;
                        max_dist = tdist * tdist;
                    }
                    else
                    {
                        int tdist = (x - minc0) * R_SCALE;
                        max_dist = tdist * tdist;
                    }
                }

                x = m_cinfo.m_colormap[1][i];
                if (x < minc1)
                {
                    int tdist = (x - minc1) * G_SCALE;
                    min_dist += tdist * tdist;
                    tdist = (x - maxc1) * G_SCALE;
                    max_dist += tdist * tdist;
                }
                else if (x > maxc1)
                {
                    int tdist = (x - maxc1) * G_SCALE;
                    min_dist += tdist * tdist;
                    tdist = (x - minc1) * G_SCALE;
                    max_dist += tdist * tdist;
                }
                else
                {
                    if (x <= centerc1)
                    {
                        int tdist = (x - maxc1) * G_SCALE;
                        max_dist += tdist * tdist;
                    }
                    else
                    {
                        int tdist = (x - minc1) * G_SCALE;
                        max_dist += tdist * tdist;
                    }
                }

                x = m_cinfo.m_colormap[2][i];
                if (x < minc2)
                {
                    int tdist = (x - minc2) * B_SCALE;
                    min_dist += tdist * tdist;
                    tdist = (x - maxc2) * B_SCALE;
                    max_dist += tdist * tdist;
                }
                else if (x > maxc2)
                {
                    int tdist = (x - maxc2) * B_SCALE;
                    min_dist += tdist * tdist;
                    tdist = (x - minc2) * B_SCALE;
                    max_dist += tdist * tdist;
                }
                else
                {
                    /* within cell range so no contribution to min_dist */
                    if (x <= centerc2)
                    {
                        int tdist = (x - maxc2) * B_SCALE;
                        max_dist += tdist * tdist;
                    }
                    else
                    {
                        int tdist = (x - minc2) * B_SCALE;
                        max_dist += tdist * tdist;
                    }
                }

                mindist[i] = min_dist;  /* save away the results */
                if (max_dist < minmaxdist)
                    minmaxdist = max_dist;
            }

            int ncolors = 0;
            for (int i = 0; i < m_cinfo.m_actual_number_of_colors; i++)
            {
                if (mindist[i] <= minmaxdist)
                    colorlist[ncolors++] = (byte)i;
            }

            return ncolors;
        }

        /// <summary>
        /// Find the closest colormap entry for each cell in the update box,
        /// </summary>
        private void find_best_colors(int minc0, int minc1, int minc2, int numcolors, byte[] colorlist, byte[] bestcolor)
        {
            const int STEP_C0 = ((1 << C0_SHIFT) * R_SCALE);
            const int STEP_C1 = ((1 << C1_SHIFT) * G_SCALE);
            const int STEP_C2 = ((1 << C2_SHIFT) * B_SCALE);

            int[] bestdist = new int[BOX_C0_ELEMS * BOX_C1_ELEMS * BOX_C2_ELEMS];

            int bestIndex = 0;
            for (int i = BOX_C0_ELEMS * BOX_C1_ELEMS * BOX_C2_ELEMS - 1; i >= 0; i--)
            {
                bestdist[bestIndex] = 0x7FFFFFFF;
                bestIndex++;
            }

            for (int i = 0; i < numcolors; i++)
            {
                int icolor = colorlist[i];

                int inc0 = (minc0 - m_cinfo.m_colormap[0][icolor]) * R_SCALE;
                int dist0 = inc0 * inc0;

                int inc1 = (minc1 - m_cinfo.m_colormap[1][icolor]) * G_SCALE;
                dist0 += inc1 * inc1;

                int inc2 = (minc2 - m_cinfo.m_colormap[2][icolor]) * B_SCALE;
                dist0 += inc2 * inc2;

                inc0 = inc0 * (2 * STEP_C0) + STEP_C0 * STEP_C0;
                inc1 = inc1 * (2 * STEP_C1) + STEP_C1 * STEP_C1;
                inc2 = inc2 * (2 * STEP_C2) + STEP_C2 * STEP_C2;

                bestIndex = 0;
                int colorIndex = 0;
                int xx0 = inc0;
                for (int ic0 = BOX_C0_ELEMS - 1; ic0 >= 0; ic0--)
                {
                    int dist1 = dist0;
                    int xx1 = inc1;
                    for (int ic1 = BOX_C1_ELEMS - 1; ic1 >= 0; ic1--)
                    {
                        int dist2 = dist1;
                        int xx2 = inc2;
                        for (int ic2 = BOX_C2_ELEMS - 1; ic2 >= 0; ic2--)
                        {
                            if (dist2 < bestdist[bestIndex])
                            {
                                bestdist[bestIndex] = dist2;
                                bestcolor[colorIndex] = (byte)icolor;
                            }

                            dist2 += xx2;
                            xx2 += 2 * STEP_C2 * STEP_C2;
                            bestIndex++;
                            colorIndex++;
                        }

                        dist1 += xx1;
                        xx1 += 2 * STEP_C1 * STEP_C1;
                    }

                    dist0 += xx0;
                    xx0 += 2 * STEP_C0 * STEP_C0;
                }
            }
        }

        /// <summary>
        /// Fill the inverse-colormap entries in the update box that contains
        /// histogram cell c0/c1/c2. 
        /// </summary>
        private void fill_inverse_cmap(int c0, int c1, int c2)
        {
            c0 >>= BOX_C0_LOG;
            c1 >>= BOX_C1_LOG;
            c2 >>= BOX_C2_LOG;

            int minc0 = (c0 << BOX_C0_SHIFT) + ((1 << C0_SHIFT) >> 1);
            int minc1 = (c1 << BOX_C1_SHIFT) + ((1 << C1_SHIFT) >> 1);
            int minc2 = (c2 << BOX_C2_SHIFT) + ((1 << C2_SHIFT) >> 1);

            byte[] colorlist = new byte[MAXNUMCOLORS];
            int numcolors = find_nearby_colors(minc0, minc1, minc2, colorlist);

            byte[] bestcolor = new byte[BOX_C0_ELEMS * BOX_C1_ELEMS * BOX_C2_ELEMS];
            find_best_colors(minc0, minc1, minc2, numcolors, colorlist, bestcolor);

            c0 <<= BOX_C0_LOG;
            c1 <<= BOX_C1_LOG;
            c2 <<= BOX_C2_LOG;
            int bestcolorIndex = 0;
            for (int ic0 = 0; ic0 < BOX_C0_ELEMS; ic0++)
            {
                for (int ic1 = 0; ic1 < BOX_C1_ELEMS; ic1++)
                {
                    int histogramIndex = (c1 + ic1) * HIST_C2_ELEMS + c2;
                    for (int ic2 = 0; ic2 < BOX_C2_ELEMS; ic2++)
                    {
                        m_histogram[c0 + ic0][histogramIndex] = (ushort)((int)bestcolor[bestcolorIndex] + 1);
                        histogramIndex++;
                        bestcolorIndex++;
                    }
                }
            }
        }
    }

    class ExMergedUpsampler : Upsampler
    {
        private const int SCALEBITS = 16;
        private const int ONE_HALF = 1 << (SCALEBITS - 1);
        private DecompressStruct m_cinfo;
        private bool m_use_2v_upsample;
        private int[] m_Cr_r_tab;
        private int[] m_Cb_b_tab;
        private int[] m_Cr_g_tab;
        private int[] m_Cb_g_tab;
        private byte[] m_spare_row;
        private bool m_spare_full;        /* T if spare buffer is occupied */

        private int m_out_row_width;   /* samples per output row */
        private int m_rows_to_go;  /* counts rows remaining in image */

        public ExMergedUpsampler(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            m_need_context_rows = false;

            m_out_row_width = cinfo.m_output_width * cinfo.m_out_color_components;

            if (cinfo.m_max_v_samp_factor == 2)
            {
                m_use_2v_upsample = true;
                m_spare_row = new byte[m_out_row_width];
            }
            else
            {
                m_use_2v_upsample = false;
            }

            build_ycc_rgb_table();
        }

        /// <summary>
        /// Initialize for an upsampling pass.
        /// </summary>
        public override void start_pass()
        {
            m_spare_full = false;
            m_rows_to_go = m_cinfo.m_output_height;
        }

        public override void upsample(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            if (m_use_2v_upsample)
                merged_2v_upsample(input_buf, ref in_row_group_ctr, output_buf, ref out_row_ctr, out_rows_avail);
            else
                merged_1v_upsample(input_buf, ref in_row_group_ctr, output_buf, ref out_row_ctr);
        }

        /// <summary>
        /// Control routine to do upsampling (and color conversion).
        /// </summary>
        private void merged_1v_upsample(ComponentBuffer[] input_buf, ref int in_row_group_ctr, byte[][] output_buf, ref int out_row_ctr)
        {
            /* Just do the upsampling. */
            h2v1_merged_upsample(input_buf, in_row_group_ctr, output_buf, out_row_ctr);

            out_row_ctr++;
            in_row_group_ctr++;
        }

        /// <summary>
        /// Control routine to do upsampling (and color conversion).
        /// </summary>
        private void merged_2v_upsample(ComponentBuffer[] input_buf, ref int in_row_group_ctr, byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            int num_rows;
            if (m_spare_full)
            {
                byte[][] temp = new byte[1][];
                temp[0] = m_spare_row;
                JpegUtils.jcopy_sample_rows(temp, 0, output_buf, out_row_ctr, 1, m_out_row_width);
                num_rows = 1;
                m_spare_full = false;
            }
            else
            {
                num_rows = 2;
                if (num_rows > m_rows_to_go)
                    num_rows = m_rows_to_go;
                out_rows_avail -= out_row_ctr;
                if (num_rows > out_rows_avail)
                    num_rows = out_rows_avail;
                byte[][] work_ptrs = new byte[2][];
                work_ptrs[0] = output_buf[out_row_ctr];
                if (num_rows > 1)
                {
                    work_ptrs[1] = output_buf[out_row_ctr + 1];
                }
                else
                {
                    work_ptrs[1] = m_spare_row;
                    m_spare_full = true;
                }
                h2v2_merged_upsample(input_buf, in_row_group_ctr, work_ptrs);
            }

            out_row_ctr += num_rows;
            m_rows_to_go -= num_rows;

            if (!m_spare_full)
                in_row_group_ctr++;
        }

        /// <summary>
        /// Upsample and color convert for the case of 2:1 horizontal and 1:1 vertical.
        /// </summary>
        private void h2v1_merged_upsample(ComponentBuffer[] input_buf, int in_row_group_ctr, byte[][] output_buf, int outRow)
        {
            int inputIndex0 = 0;
            int inputIndex1 = 0;
            int inputIndex2 = 0;
            int outputIndex = 0;

            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset;

            for (int col = m_cinfo.m_output_width >> 1; col > 0; col--)
            {
                int cb = input_buf[1][in_row_group_ctr][inputIndex1];
                inputIndex1++;

                int cr = input_buf[2][in_row_group_ctr][inputIndex2];
                inputIndex2++;

                int cred = m_Cr_r_tab[cr];
                int cgreen = JpegUtils.RIGHT_SHIFT(m_Cb_g_tab[cb] + m_Cr_g_tab[cr], SCALEBITS);
                int cblue = m_Cb_b_tab[cb];

                int y = input_buf[0][in_row_group_ctr][inputIndex0];
                inputIndex0++;

                output_buf[outRow][outputIndex + JpegConstants.RGB_RED] = limit[limitOffset + y + cred];
                output_buf[outRow][outputIndex + JpegConstants.RGB_GREEN] = limit[limitOffset + y + cgreen];
                output_buf[outRow][outputIndex + JpegConstants.RGB_BLUE] = limit[limitOffset + y + cblue];
                outputIndex += JpegConstants.RGB_PIXELSIZE;

                y = input_buf[0][in_row_group_ctr][inputIndex0];
                inputIndex0++;

                output_buf[outRow][outputIndex + JpegConstants.RGB_RED] = limit[limitOffset + y + cred];
                output_buf[outRow][outputIndex + JpegConstants.RGB_GREEN] = limit[limitOffset + y + cgreen];
                output_buf[outRow][outputIndex + JpegConstants.RGB_BLUE] = limit[limitOffset + y + cblue];
                outputIndex += JpegConstants.RGB_PIXELSIZE;
            }

            if ((m_cinfo.m_output_width & 1) != 0)
            {
                int cb = input_buf[1][in_row_group_ctr][inputIndex1];
                int cr = input_buf[2][in_row_group_ctr][inputIndex2];
                int cred = m_Cr_r_tab[cr];
                int cgreen = JpegUtils.RIGHT_SHIFT(m_Cb_g_tab[cb] + m_Cr_g_tab[cr], SCALEBITS);
                int cblue = m_Cb_b_tab[cb];

                int y = input_buf[0][in_row_group_ctr][inputIndex0];
                output_buf[outRow][outputIndex + JpegConstants.RGB_RED] = limit[limitOffset + y + cred];
                output_buf[outRow][outputIndex + JpegConstants.RGB_GREEN] = limit[limitOffset + y + cgreen];
                output_buf[outRow][outputIndex + JpegConstants.RGB_BLUE] = limit[limitOffset + y + cblue];
            }
        }

        /// <summary>
        /// Upsample and color convert for the case of 2:1 horizontal and 2:1 vertical.
        /// </summary>
        private void h2v2_merged_upsample(ComponentBuffer[] input_buf, int in_row_group_ctr, byte[][] output_buf)
        {
            int inputRow00 = in_row_group_ctr * 2;
            int inputIndex00 = 0;

            int inputRow01 = in_row_group_ctr * 2 + 1;
            int inputIndex01 = 0;

            int inputIndex1 = 0;
            int inputIndex2 = 0;

            int outIndex0 = 0;
            int outIndex1 = 0;

            byte[] limit = m_cinfo.m_sample_range_limit;
            int limitOffset = m_cinfo.m_sampleRangeLimitOffset;

            for (int col = m_cinfo.m_output_width >> 1; col > 0; col--)
            {
                int cb = input_buf[1][in_row_group_ctr][inputIndex1];
                inputIndex1++;

                int cr = input_buf[2][in_row_group_ctr][inputIndex2];
                inputIndex2++;

                int cred = m_Cr_r_tab[cr];
                int cgreen = JpegUtils.RIGHT_SHIFT(m_Cb_g_tab[cb] + m_Cr_g_tab[cr], SCALEBITS);
                int cblue = m_Cb_b_tab[cb];

                int y = input_buf[0][inputRow00][inputIndex00];
                inputIndex00++;

                output_buf[0][outIndex0 + JpegConstants.RGB_RED] = limit[limitOffset + y + cred];
                output_buf[0][outIndex0 + JpegConstants.RGB_GREEN] = limit[limitOffset + y + cgreen];
                output_buf[0][outIndex0 + JpegConstants.RGB_BLUE] = limit[limitOffset + y + cblue];
                outIndex0 += JpegConstants.RGB_PIXELSIZE;

                y = input_buf[0][inputRow00][inputIndex00];
                inputIndex00++;

                output_buf[0][outIndex0 + JpegConstants.RGB_RED] = limit[limitOffset + y + cred];
                output_buf[0][outIndex0 + JpegConstants.RGB_GREEN] = limit[limitOffset + y + cgreen];
                output_buf[0][outIndex0 + JpegConstants.RGB_BLUE] = limit[limitOffset + y + cblue];
                outIndex0 += JpegConstants.RGB_PIXELSIZE;

                y = input_buf[0][inputRow01][inputIndex01];
                inputIndex01++;

                output_buf[1][outIndex1 + JpegConstants.RGB_RED] = limit[limitOffset + y + cred];
                output_buf[1][outIndex1 + JpegConstants.RGB_GREEN] = limit[limitOffset + y + cgreen];
                output_buf[1][outIndex1 + JpegConstants.RGB_BLUE] = limit[limitOffset + y + cblue];
                outIndex1 += JpegConstants.RGB_PIXELSIZE;

                y = input_buf[0][inputRow01][inputIndex01];
                inputIndex01++;

                output_buf[1][outIndex1 + JpegConstants.RGB_RED] = limit[limitOffset + y + cred];
                output_buf[1][outIndex1 + JpegConstants.RGB_GREEN] = limit[limitOffset + y + cgreen];
                output_buf[1][outIndex1 + JpegConstants.RGB_BLUE] = limit[limitOffset + y + cblue];
                outIndex1 += JpegConstants.RGB_PIXELSIZE;
            }

            if ((m_cinfo.m_output_width & 1) != 0)
            {
                int cb = input_buf[1][in_row_group_ctr][inputIndex1];
                int cr = input_buf[2][in_row_group_ctr][inputIndex2];
                int cred = m_Cr_r_tab[cr];
                int cgreen = JpegUtils.RIGHT_SHIFT(m_Cb_g_tab[cb] + m_Cr_g_tab[cr], SCALEBITS);
                int cblue = m_Cb_b_tab[cb];

                int y = input_buf[0][inputRow00][inputIndex00];
                output_buf[0][outIndex0 + JpegConstants.RGB_RED] = limit[limitOffset + y + cred];
                output_buf[0][outIndex0 + JpegConstants.RGB_GREEN] = limit[limitOffset + y + cgreen];
                output_buf[0][outIndex0 + JpegConstants.RGB_BLUE] = limit[limitOffset + y + cblue];

                y = input_buf[0][inputRow01][inputIndex01];
                output_buf[1][outIndex1 + JpegConstants.RGB_RED] = limit[limitOffset + y + cred];
                output_buf[1][outIndex1 + JpegConstants.RGB_GREEN] = limit[limitOffset + y + cgreen];
                output_buf[1][outIndex1 + JpegConstants.RGB_BLUE] = limit[limitOffset + y + cblue];
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

        private static int FIX(double x)
        {
            return ((int)((x) * (1L << SCALEBITS) + 0.5));
        }
    }

    /// <summary>
    /// Expanded data source object for stdio input
    /// </summary>
    class ExSourceMgr : SourceMgr
    {
        private const int INPUT_BUF_SIZE = 4096;

        private DecompressStruct m_cinfo;

        private Stream m_infile;
        private byte[] m_buffer;
        private bool m_start_of_file;

        /// <summary>
        /// Initialize source - called by jpeg_read_header
        /// before any data is actually read.
        /// </summary>
        public ExSourceMgr(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            m_buffer = new byte[INPUT_BUF_SIZE];
        }

        public void Attach(Stream infile)
        {
            m_infile = infile;
            m_infile.Seek(0, SeekOrigin.Begin);
            initInternalBuffer(null, 0);
        }

        public override void init_source()
        {
            m_start_of_file = true;
        }

        /// <summary>
        /// Fill the input buffer - called whenever buffer is emptied.
        /// </summary>
        public override bool fill_input_buffer()
        {
            int nbytes = m_infile.Read(m_buffer, 0, INPUT_BUF_SIZE);
            if (nbytes <= 0)
            {
                m_buffer[0] = (byte)0xFF;
                m_buffer[1] = (byte)JPEG_MARKER.EOI;
                nbytes = 2;
            }

            initInternalBuffer(m_buffer, nbytes);
            m_start_of_file = false;

            return true;
        }
    }

    class ExUpsampler : Upsampler
    {
        private enum ComponentUpsampler
        {
            noop_upsampler,
            fullsize_upsampler,
            h2v1_fancy_upsampler,
            h2v1_upsampler,
            h2v2_fancy_upsampler,
            h2v2_upsampler,
            int_upsampler
        }

        private DecompressStruct m_cinfo;
        private ComponentBuffer[] m_color_buf = new ComponentBuffer[JpegConstants.MAX_COMPONENTS];
        private int[] m_perComponentOffsets = new int[JpegConstants.MAX_COMPONENTS];
        private ComponentUpsampler[] m_upsampleMethods = new ComponentUpsampler[JpegConstants.MAX_COMPONENTS];
        private int m_currentComponent;
        private int m_upsampleRowOffset;
        private int m_next_row_out;
        private int m_rows_to_go;
        private int[] m_rowgroup_height = new int[JpegConstants.MAX_COMPONENTS];
        private byte[] m_h_expand = new byte[JpegConstants.MAX_COMPONENTS];
        private byte[] m_v_expand = new byte[JpegConstants.MAX_COMPONENTS];

        public ExUpsampler(DecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            m_need_context_rows = false;

            bool do_fancy = cinfo.m_do_fancy_upsampling && cinfo.m_min_DCT_scaled_size > 1;
            for (int ci = 0; ci < cinfo.m_num_components; ci++)
            {
                ComponentInfo componentInfo = cinfo.Comp_info[ci];

                int h_in_group = (componentInfo.H_samp_factor * componentInfo.DCT_scaled_size) / cinfo.m_min_DCT_scaled_size;
                int v_in_group = (componentInfo.V_samp_factor * componentInfo.DCT_scaled_size) / cinfo.m_min_DCT_scaled_size;
                int h_out_group = cinfo.m_max_h_samp_factor;
                int v_out_group = cinfo.m_max_v_samp_factor;

                m_rowgroup_height[ci] = v_in_group;
                bool need_buffer = true;
                if (!componentInfo.component_needed)
                {
                    m_upsampleMethods[ci] = ComponentUpsampler.noop_upsampler;
                    need_buffer = false;
                }
                else if (h_in_group == h_out_group && v_in_group == v_out_group)
                {
                    m_upsampleMethods[ci] = ComponentUpsampler.fullsize_upsampler;
                    need_buffer = false;
                }
                else if (h_in_group * 2 == h_out_group && v_in_group == v_out_group)
                {
                    if (do_fancy && componentInfo.downsampled_width > 2)
                        m_upsampleMethods[ci] = ComponentUpsampler.h2v1_fancy_upsampler;
                    else
                        m_upsampleMethods[ci] = ComponentUpsampler.h2v1_upsampler;
                }
                else if (h_in_group * 2 == h_out_group && v_in_group * 2 == v_out_group)
                {
                    if (do_fancy && componentInfo.downsampled_width > 2)
                    {
                        m_upsampleMethods[ci] = ComponentUpsampler.h2v2_fancy_upsampler;
                        m_need_context_rows = true;
                    }
                    else
                    {
                        m_upsampleMethods[ci] = ComponentUpsampler.h2v2_upsampler;
                    }
                }
                else if ((h_out_group % h_in_group) == 0 && (v_out_group % v_in_group) == 0)
                {
                    m_upsampleMethods[ci] = ComponentUpsampler.int_upsampler;
                    m_h_expand[ci] = (byte)(h_out_group / h_in_group);
                    m_v_expand[ci] = (byte)(v_out_group / v_in_group);
                }

                if (need_buffer)
                {
                    ComponentBuffer cb = new ComponentBuffer();
                    cb.SetBuffer(CommonStruct.AllocJpegSamples(JpegUtils.jround_up(cinfo.m_output_width,
                        cinfo.m_max_h_samp_factor), cinfo.m_max_v_samp_factor), null, 0);

                    m_color_buf[ci] = cb;
                }
            }
        }

        /// <summary>
        /// Initialize for an upsampling pass.
        /// </summary>
        public override void start_pass()
        {
            m_next_row_out = m_cinfo.m_max_v_samp_factor;
            m_rows_to_go = m_cinfo.m_output_height;
        }

        /// <summary>
        /// Control routine to do upsampling (and color conversion).
        /// </summary>
        public override void upsample(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            if (m_next_row_out >= m_cinfo.m_max_v_samp_factor)
            {
                for (int ci = 0; ci < m_cinfo.m_num_components; ci++)
                {
                    m_perComponentOffsets[ci] = 0;

                    m_currentComponent = ci;
                    m_upsampleRowOffset = in_row_group_ctr * m_rowgroup_height[ci];
                    upsampleComponent(ref input_buf[ci]);
                }

                m_next_row_out = 0;
            }
            int num_rows = m_cinfo.m_max_v_samp_factor - m_next_row_out;
            if (num_rows > m_rows_to_go)
                num_rows = m_rows_to_go;
            out_rows_avail -= out_row_ctr;
            if (num_rows > out_rows_avail)
                num_rows = out_rows_avail;

            m_cinfo.m_cconvert.color_convert(m_color_buf, m_perComponentOffsets, m_next_row_out, output_buf, out_row_ctr, num_rows);

            out_row_ctr += num_rows;
            m_rows_to_go -= num_rows;
            m_next_row_out += num_rows;

            /* When the buffer is emptied, declare this input row group consumed */
            if (m_next_row_out >= m_cinfo.m_max_v_samp_factor)
                in_row_group_ctr++;
        }

        private void upsampleComponent(ref ComponentBuffer input_data)
        {
            switch (m_upsampleMethods[m_currentComponent])
            {
                case ComponentUpsampler.noop_upsampler:
                    noop_upsample();
                    break;
                case ComponentUpsampler.fullsize_upsampler:
                    fullsize_upsample(ref input_data);
                    break;
                case ComponentUpsampler.h2v1_fancy_upsampler:
                    h2v1_fancy_upsample(m_cinfo.Comp_info[m_currentComponent].downsampled_width, ref input_data);
                    break;
                case ComponentUpsampler.h2v1_upsampler:
                    h2v1_upsample(ref input_data);
                    break;
                case ComponentUpsampler.h2v2_fancy_upsampler:
                    h2v2_fancy_upsample(m_cinfo.Comp_info[m_currentComponent].downsampled_width, ref input_data);
                    break;
                case ComponentUpsampler.h2v2_upsampler:
                    h2v2_upsample(ref input_data);
                    break;
                case ComponentUpsampler.int_upsampler:
                    int_upsample(ref input_data);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This is a no-op version used for "uninteresting" components.
        /// </summary>
        private static void noop_upsample()
        {

        }

        /// <summary>
        /// For full-size components, we just make color_buf[ci] point at the
        /// input buffer, and thus avoid copying any data.
        /// </summary>
        private void fullsize_upsample(ref ComponentBuffer input_data)
        {
            m_color_buf[m_currentComponent] = input_data;
            m_perComponentOffsets[m_currentComponent] = m_upsampleRowOffset;
        }

        /// <summary>
        /// Fancy processing for the common case of 2:1 horizontal and 1:1 vertical.
        /// </summary>
        private void h2v1_fancy_upsample(int downsampled_width, ref ComponentBuffer input_data)
        {
            ComponentBuffer output_data = m_color_buf[m_currentComponent];

            for (int inrow = 0; inrow < m_cinfo.m_max_v_samp_factor; inrow++)
            {
                int row = m_upsampleRowOffset + inrow;
                int inIndex = 0;

                int outIndex = 0;

                int invalue = input_data[row][inIndex];
                inIndex++;

                output_data[inrow][outIndex] = (byte)invalue;
                outIndex++;
                output_data[inrow][outIndex] = (byte)((invalue * 3 + (int)input_data[row][inIndex] + 2) >> 2);
                outIndex++;

                for (int colctr = downsampled_width - 2; colctr > 0; colctr--)
                {
                    /* General case: 3/4 * nearer pixel + 1/4 * further pixel */
                    invalue = (int)input_data[row][inIndex] * 3;
                    inIndex++;

                    output_data[inrow][outIndex] = (byte)((invalue + (int)input_data[row][inIndex - 2] + 1) >> 2);
                    outIndex++;

                    output_data[inrow][outIndex] = (byte)((invalue + (int)input_data[row][inIndex] + 2) >> 2);
                    outIndex++;
                }

                /* Special case for last column */
                invalue = input_data[row][inIndex];
                output_data[inrow][outIndex] = (byte)((invalue * 3 + (int)input_data[row][inIndex - 1] + 1) >> 2);
                outIndex++;
                output_data[inrow][outIndex] = (byte)invalue;
                outIndex++;
            }
        }

        /// <summary>
        /// Fast processing for the common case of 2:1 horizontal and 1:1 vertical.
        /// </summary>
        private void h2v1_upsample(ref ComponentBuffer input_data)
        {
            ComponentBuffer output_data = m_color_buf[m_currentComponent];

            for (int inrow = 0; inrow < m_cinfo.m_max_v_samp_factor; inrow++)
            {
                int row = m_upsampleRowOffset + inrow;
                int outIndex = 0;

                for (int col = 0; outIndex < m_cinfo.m_output_width; col++)
                {
                    byte invalue = input_data[row][col]; /* don't need GETJSAMPLE() here */
                    output_data[inrow][outIndex] = invalue;
                    outIndex++;
                    output_data[inrow][outIndex] = invalue;
                    outIndex++;
                }
            }
        }

        /// <summary>
        /// Fancy processing for the common case of 2:1 horizontal and 2:1 vertical.
        /// </summary>
        private void h2v2_fancy_upsample(int downsampled_width, ref ComponentBuffer input_data)
        {
            ComponentBuffer output_data = m_color_buf[m_currentComponent];

            int inrow = m_upsampleRowOffset;
            int outrow = 0;
            while (outrow < m_cinfo.m_max_v_samp_factor)
            {
                for (int v = 0; v < 2; v++)
                {
                    int inIndex0 = 0;

                    int inIndex1 = 0;
                    int inRow1 = -1;
                    if (v == 0)
                    {
                        inRow1 = inrow - 1;
                    }
                    else
                    {
                        inRow1 = inrow + 1;
                    }

                    int row = outrow;
                    int outIndex = 0;
                    outrow++;

                    /* Special case for first column */
                    int thiscolsum = (int)input_data[inrow][inIndex0] * 3 + (int)input_data[inRow1][inIndex1];
                    inIndex0++;
                    inIndex1++;

                    int nextcolsum = (int)input_data[inrow][inIndex0] * 3 + (int)input_data[inRow1][inIndex1];
                    inIndex0++;
                    inIndex1++;

                    output_data[row][outIndex] = (byte)((thiscolsum * 4 + 8) >> 4);
                    outIndex++;

                    output_data[row][outIndex] = (byte)((thiscolsum * 3 + nextcolsum + 7) >> 4);
                    outIndex++;

                    int lastcolsum = thiscolsum;
                    thiscolsum = nextcolsum;

                    for (int colctr = downsampled_width - 2; colctr > 0; colctr--)
                    {
                        nextcolsum = (int)input_data[inrow][inIndex0] * 3 + (int)input_data[inRow1][inIndex1];
                        inIndex0++;
                        inIndex1++;

                        output_data[row][outIndex] = (byte)((thiscolsum * 3 + lastcolsum + 8) >> 4);
                        outIndex++;

                        output_data[row][outIndex] = (byte)((thiscolsum * 3 + nextcolsum + 7) >> 4);
                        outIndex++;

                        lastcolsum = thiscolsum;
                        thiscolsum = nextcolsum;
                    }

                    /* Special case for last column */
                    output_data[row][outIndex] = (byte)((thiscolsum * 3 + lastcolsum + 8) >> 4);
                    outIndex++;
                    output_data[row][outIndex] = (byte)((thiscolsum * 4 + 7) >> 4);
                    outIndex++;
                }

                inrow++;
            }
        }

        /// <summary>
        /// Fast processing for the common case of 2:1 horizontal and 2:1 vertical.
        /// </summary>
        private void h2v2_upsample(ref ComponentBuffer input_data)
        {
            ComponentBuffer output_data = m_color_buf[m_currentComponent];

            int inrow = 0;
            int outrow = 0;
            while (outrow < m_cinfo.m_max_v_samp_factor)
            {
                int row = m_upsampleRowOffset + inrow;
                int outIndex = 0;

                for (int col = 0; outIndex < m_cinfo.m_output_width; col++)
                {
                    byte invalue = input_data[row][col]; /* don't need GETJSAMPLE() here */
                    output_data[outrow][outIndex] = invalue;
                    outIndex++;
                    output_data[outrow][outIndex] = invalue;
                    outIndex++;
                }

                JpegUtils.jcopy_sample_rows(output_data, outrow, output_data, outrow + 1, 1, m_cinfo.m_output_width);
                inrow++;
                outrow += 2;
            }
        }

        /// <summary>
        /// This version handles any integral sampling ratios.
        /// </summary>
        private void int_upsample(ref ComponentBuffer input_data)
        {
            ComponentBuffer output_data = m_color_buf[m_currentComponent];
            int h_expand = m_h_expand[m_currentComponent];
            int v_expand = m_v_expand[m_currentComponent];

            int inrow = 0;
            int outrow = 0;
            while (outrow < m_cinfo.m_max_v_samp_factor)
            {
                /* Generate one output row with proper horizontal expansion */
                int row = m_upsampleRowOffset + inrow;
                for (int col = 0; col < m_cinfo.m_output_width; col++)
                {
                    byte invalue = input_data[row][col]; /* don't need GETJSAMPLE() here */
                    int outIndex = 0;
                    for (int h = h_expand; h > 0; h--)
                    {
                        output_data[outrow][outIndex] = invalue;
                        outIndex++;
                    }
                }

                if (v_expand > 1)
                {
                    JpegUtils.jcopy_sample_rows(output_data, outrow, output_data,
                        outrow + 1, v_expand - 1, m_cinfo.m_output_width);
                }

                inrow++;
                outrow += v_expand;
            }
        }
    }
}
