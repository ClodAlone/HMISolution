#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;

using Syncfusion.Pdf.Compression.JBIG2.Internal;

namespace Syncfusion.Pdf.Compression.JBIG2
{
    /// <summary>Base class for both JPEG compressor and decompresor.</summary>
    abstract class CommonStruct
    {
        internal enum JpegState
        {
            DESTROYED = 0,
            CSTATE_START = 100,
            CSTATE_SCANNING = 101,
            CSTATE_RAW_OK = 102,
            CSTATE_WRCOEFS = 103,
            DSTATE_START = 200,
            DSTATE_INHEADER = 201,
            DSTATE_READY = 202,
            DSTATE_PRELOAD = 203,
            DSTATE_PRESCAN = 204,
            DSTATE_SCANNING = 205,
            DSTATE_RAW_OK = 206,
            DSTATE_BUFIMAGE = 207,
            DSTATE_BUFPOST = 208,
            DSTATE_RDCOEFS = 209,
            DSTATE_STOPPING = 210
        }

        //internal ErrorMgr m_err;
        internal ProgressMgr m_progress;
        internal JpegState m_global_state;

        /// <summary>
        /// Base constructor.
        /// </summary>
        public CommonStruct()
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is Jpeg decompressor.
        /// </summary>
        public abstract bool IsDecompressor
        {
            get;
        }

        /// <summary>
        /// Progress monitor.
        /// </summary>
        public ProgressMgr Progress
        {
            get
            {
                return m_progress;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_progress = value;
            }
        }

        /// <summary>
        /// Creates the array of samples.
        /// </summary>
        public static jvirt_array<byte> CreateSamplesArray(int samplesPerRow, int numberOfRows)
        {
            return new jvirt_array<byte>(samplesPerRow, numberOfRows, AllocJpegSamples);
        }

        /// <summary>
        /// Creates the array of blocks.
        /// </summary>
        public static jvirt_array<JBLOCK> CreateBlocksArray(int blocksPerRow, int numberOfRows)
        {
            return new jvirt_array<JBLOCK>(blocksPerRow, numberOfRows, allocJpegBlocks);
        }

        /// <summary>
        /// Creates 2-D sample array.
        /// </summary>
        public static byte[][] AllocJpegSamples(int samplesPerRow, int numberOfRows)
        {
            byte[][] result = new byte[numberOfRows][];
            for (int i = 0; i < numberOfRows; ++i)
                result[i] = new byte[samplesPerRow];

            return result;
        }

        private static JBLOCK[][] allocJpegBlocks(int blocksPerRow, int numberOfRows)
        {
            JBLOCK[][] result = new JBLOCK[numberOfRows][];
            for (int i = 0; i < numberOfRows; ++i)
            {
                result[i] = new JBLOCK[blocksPerRow];
                for (int j = 0; j < blocksPerRow; ++j)
                    result[i][j] = new JBLOCK();
            }
            return result;
        }

        public void jpeg_abort()
        {
            if (IsDecompressor)
            {
                m_global_state = JpegState.DSTATE_START;

                DecompressStruct s = this as DecompressStruct;
                if (s != null)
                    s.m_marker_list = null;
            }
            else
            {
                m_global_state = JpegState.CSTATE_START;
            }
        }

        public void jpeg_destroy()
        {
            m_global_state = JpegState.DESTROYED;
        }
    }

    /// <summary>
    /// JPEG decompression routine.
    /// </summary>
    class DecompressStruct : CommonStruct
    {
        /// <summary>
        /// The delegate for application-supplied marker processing methods.
        /// </summary>
        public delegate bool jpeg_marker_parser_method(DecompressStruct cinfo);

        internal SourceMgr m_src;

        internal int m_image_width;
        internal int m_image_height;
        internal int m_num_components;
        internal J_COLOR_SPACE m_jpeg_color_space;

        internal J_COLOR_SPACE m_out_color_space;
        internal int m_scale_num;
        internal int m_scale_denom;
        internal bool m_buffered_image;
        internal bool m_raw_data_out;  
        internal J_DCT_METHOD m_dct_method;    
        internal bool m_do_fancy_upsampling;  
        internal bool m_do_block_smoothing;   
        internal bool m_quantize_colors;  
        internal J_DITHER_MODE m_dither_mode; 
        internal bool m_two_pass_quantize; 
        internal int m_desired_number_of_colors;
        internal bool m_enable_1pass_quant;  
        internal bool m_enable_external_quant;
        internal bool m_enable_2pass_quant;   

        internal int m_output_width;  
        internal int m_output_height;  
        internal int m_out_color_components;
        internal int m_output_components;

        internal int m_rec_outbuf_height;  

        internal int m_actual_number_of_colors;   
        internal byte[][] m_colormap;  

        internal int m_output_scanline;

        internal int m_input_scan_number;
        internal int m_input_iMCU_row; 

        internal int m_output_scan_number;
        internal int m_output_iMCU_row; 

        internal int[][] m_coef_bits;

        internal JQUANT_TBL[] m_quant_tbl_ptrs = new JQUANT_TBL[JpegConstants.NUM_QUANT_TBLS];

        internal JHUFF_TBL[] m_dc_huff_tbl_ptrs = new JHUFF_TBL[JpegConstants.NUM_HUFF_TBLS];
        internal JHUFF_TBL[] m_ac_huff_tbl_ptrs = new JHUFF_TBL[JpegConstants.NUM_HUFF_TBLS];
        internal int m_data_precision; 

        private ComponentInfo[] m_comp_info;
        internal bool m_progressive_mode; 
        internal int m_restart_interval;

        internal bool m_saw_JFIF_marker; 
        internal byte m_JFIF_major_version;
        internal byte m_JFIF_minor_version;

        internal DensityUnit m_density_unit;  
        internal short m_X_density;    
        internal short m_Y_density;    

        internal bool m_saw_Adobe_marker; 
        internal byte m_Adobe_transform;

        internal bool m_CCIR601_sampling; 

        internal List<MarkerStruct> m_marker_list;
        internal int m_max_h_samp_factor; 
        internal int m_max_v_samp_factor; 
        internal int m_min_DCT_scaled_size; 
        internal int m_total_iMCU_rows;
        internal byte[] m_sample_range_limit; 
        internal int m_sampleRangeLimitOffset;

        internal int m_comps_in_scan;  
        internal int[] m_cur_comp_info = new int[JpegConstants.MAX_COMPS_IN_SCAN];
        internal int m_MCUs_per_row; 
        internal int m_MCU_rows_in_scan; 

        internal int m_blocks_in_MCU; 
        internal int[] m_MCU_membership = new int[JpegConstants.D_MAX_BLOCKS_IN_MCU];
        internal int m_Ss;
        internal int m_Se;
        internal int m_Ah;
        internal int m_Al;
        internal int m_unread_marker;
        internal DecompMaster m_master;
        internal DMainController m_main;
        internal DCoefController m_coef;
        internal DPostController m_post;
        internal InputController m_inputctl;
        internal MarkerReader m_marker;
        internal EntropyDecoder m_entropy;
        internal InverseDCT m_idct;
        internal Upsampler m_upsample;
        internal ColorDeconverter m_cconvert;
        internal ColorQuantizer m_cquantize;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecompressStruct"/> class.
        /// </summary>
        public DecompressStruct()
            : base()
        {
            initialize();
        }

        /// <summary>
        /// Retrieves <c>true</c> because this is a decompressor.
        /// </summary>
        public override bool IsDecompressor
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the source for decompression.
        /// </summary>
        public SourceMgr Src
        {
            get { return m_src; }
            set { m_src = value; }
        }
      
        /// <summary>
        /// Gets the width of image, set by <see cref="DecompressStruct.jpeg_read_header"/>
        /// </summary>
        public int Image_width
        {
            get { return m_image_width; }
        }

        /// <summary>
        /// Gets the height of image, set by <see cref="DecompressStruct.jpeg_read_header"/>
        /// </summary>
        public int Image_height
        {
            get { return m_image_height; }
        }
        
        /// <summary>
        /// Gets the number of color components in JPEG image.
        /// </summary>
        public int Num_components
        {
            get { return m_num_components; }
        }

        /// <summary>
        /// Gets or sets the colorspace of JPEG image.
        /// </summary>
        public J_COLOR_SPACE Jpeg_color_space
        {
            get { return m_jpeg_color_space; }
            set { m_jpeg_color_space = value; }
        }

        /// <summary>
        /// Gets the list of loaded special markers.
        /// </summary>
        public ReadOnlyCollection<MarkerStruct> Marker_list
        {
            get
            {
                return m_marker_list.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets the output color space.
        /// </summary>
        public J_COLOR_SPACE Out_color_space
        {
            get { return m_out_color_space; }
            set { m_out_color_space = value; }
        }

        /// <summary>
        /// Gets or sets the numerator of the fraction of image scaling.
        /// </summary>
        public int Scale_num
        {
            get { return m_scale_num; }
            set { m_scale_num = value; }
        }

        /// <summary>
        /// Gets or sets the denominator of the fraction of image scaling.
        /// </summary>
        public int Scale_denom
        {
            get { return m_scale_denom; }
            set { m_scale_denom = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use buffered-image mode.
        /// </summary>
        public bool Buffered_image
        {
            get { return m_buffered_image; }
            set { m_buffered_image = value; }
        }

        /// <summary>
        /// Enable or disable raw data output.
        /// </summary>
        public bool Raw_data_out
        {
            get { return m_raw_data_out; }
            set { m_raw_data_out = value; }
        }

        /// <summary>
        /// Gets or sets the algorithm used for the DCT step.
        /// </summary>
        public J_DCT_METHOD Dct_method
        {
            get { return m_dct_method; }
            set { m_dct_method = value; }
        }
        
        /// <summary>
        /// Enable or disable upsampling of chroma components.
        /// </summary>
        public bool Do_fancy_upsampling
        {
            get { return m_do_fancy_upsampling; }
            set { m_do_fancy_upsampling = value; }
        }
        
        /// <summary>
        /// Apply interblock smoothing in early stages of decoding progressive JPEG files.
        /// </summary>
        public bool Do_block_smoothing
        {
            get { return m_do_block_smoothing; }
            set { m_do_block_smoothing = value; }
        }
        
        /// <summary>
        /// Colors quantization.
        /// </summary>
        public bool Quantize_colors
        {
            get { return m_quantize_colors; }
            set { m_quantize_colors = value; }
        }

        /// <summary>
        /// Selects color dithering method.
        /// </summary>
        public J_DITHER_MODE Dither_mode
        {
            get { return m_dither_mode; }
            set { m_dither_mode = value; }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to use two-pass color quantization.
        /// </summary>
        public bool Two_pass_quantize
        {
            get { return m_two_pass_quantize; }
            set { m_two_pass_quantize = value; }
        }

        /// <summary>
        /// Maximum number of colors to use in generating a library-supplied color map.
        /// </summary>
        public int Desired_number_of_colors
        {
            get { return m_desired_number_of_colors; }
            set { m_desired_number_of_colors = value; }
        }
        
        /// <summary>
        /// Enable future use of 1-pass quantizer.
        /// </summary>
        public bool Enable_1pass_quant
        {
            get { return m_enable_1pass_quant; }
            set { m_enable_1pass_quant = value; }
        }
        
        /// <summary>
        /// Enable future use of external colormap.
        /// </summary>
        public bool Enable_external_quant
        {
            get { return m_enable_external_quant; }
            set { m_enable_external_quant = value; }
        }

        /// <summary>
        /// Enable future use of 2-pass quantizer.
        /// </summary>
        public bool Enable_2pass_quant
        {
            get { return m_enable_2pass_quant; }
            set { m_enable_2pass_quant = value; }
        }

        /// <summary>
        /// Gets the actual width of output image.
        /// </summary>
        public int Output_width
        {
            get { return m_output_width; }
        }

        /// <summary>
        /// Gets the actual height of output image.
        /// </summary>
        public int Output_height
        {
            get { return m_output_height; }
        }
        
        /// <summary>
        /// Gets the number of color components in <see cref="DecompressStruct.Out_color_space"/>.
        /// </summary>
        public int Out_color_components
        {
            get { return m_out_color_components; }
        }

        /// <summary>
        /// Gets the number of color components returned.
        /// </summary>
        public int Output_components
        {
            get { return m_output_components; }
        }

        /// <summary>
        /// Gets the recommended height of scanline buffer.
        /// </summary>
        public int Rec_outbuf_height
        {
            get { return m_rec_outbuf_height; }
        }

        /// <summary>
        /// The number of colors in the color map.
        /// </summary>
        public int Actual_number_of_colors
        {
            get { return m_actual_number_of_colors; }
            set { m_actual_number_of_colors = value; }
        }

        /// <summary>
        /// The color map, represented as a 2-D pixel array of <see cref="DecompressStruct.Out_color_components"/> rows
        /// and <see cref="DecompressStruct.Actual_number_of_colors"/> columns.
        /// </summary>
        public byte[][] Colormap
        {
            get { return m_colormap; }
            set { m_colormap = value; }
        }

        /// <summary>
        /// Gets the number of scanlines returned so far.
        /// </summary>
        public int Output_scanline
        {
            get { return m_output_scanline; }
        }

        /// <summary>
        /// Gets the number of SOS markers seen so far.
        /// </summary>
        public int Input_scan_number
        {
            get { return m_input_scan_number; }
        }

        /// <summary>
        /// Gets the number of iMCU rows completed.
        /// </summary>
        public int Input_iMCU_row
        {
            get { return m_input_iMCU_row; }
        }

        /// <summary>
        /// Gets the nominal scan number being displayed.
        /// </summary>
        public int Output_scan_number
        {
            get { return m_output_scan_number; }
        }
        
        /// <summary>
        /// Gets the number of iMCU rows read.
        /// </summary>
        public int Output_iMCU_row
        {
            get { return m_output_iMCU_row; }
        }

        /// <summary>
        /// Gets the current progression status..
        /// </summary>
        public int[][] Coef_bits
        {
            get { return m_coef_bits; }
        }

        /// <summary>
        /// Gets the resolution information from JFIF marker.
        /// </summary>
        public DensityUnit Density_unit
        {
            get { return m_density_unit; }
        }

        /// <summary>
        /// Gets the horizontal component of pixel ratio.
        /// </summary>
        public short X_density
        {
            get { return m_X_density; }
        }

        /// <summary>
        /// Gets the vertical component of pixel ratio.
        /// </summary>
        public short Y_density
        {
            get { return m_Y_density; }
        }

        /// <summary>
        /// Gets the data precision.
        /// </summary>
        public int Data_precision
        {
            get { return m_data_precision; }
        }

        /// <summary>
        /// Gets the largest vertical sample factor.
        /// </summary>
        public int Max_v_samp_factor
        {
            get { return m_max_v_samp_factor; }
        }

        /// <summary>
        /// Gets the last read and unprocessed JPEG marker.
        /// </summary>
        public int Unread_marker
        {
            get { return m_unread_marker; }
        }

        /// <summary>
        /// Comp_info[i] describes component that appears i'th in SOF
        /// </summary>
        public ComponentInfo[] Comp_info
        {
            get { return m_comp_info; }
            internal set { m_comp_info = value; }
        }

        /// <summary>
        /// Sets input stream.
        /// </summary>
        public void jpeg_stdio_src(Stream infile)
        {
            if (m_src == null)
            {
                m_src = new ExSourceMgr(this);
            }

            ExSourceMgr m = m_src as ExSourceMgr;
            if (m != null)
                m.Attach(infile);
        }

        /// <summary>
        /// Decompression startup: this will read the source datastream header markers, up to the beginning of the compressed data proper.
        /// </summary>
        public ReadResult jpeg_read_header(bool require_image)
        {
            ReadResult retcode = jpeg_consume_input();

            switch (retcode)
            {
                case ReadResult.JPEG_REACHED_SOS:
                    return ReadResult.JPEG_HEADER_OK;
                case ReadResult.JPEG_REACHED_EOI:
                    jpeg_abort();
                    return ReadResult.JPEG_HEADER_TABLES_ONLY;

                case ReadResult.JPEG_SUSPENDED:
                    break;
            }

            return ReadResult.JPEG_SUSPENDED;
        }

        /// <summary>
        /// Decompression initialization.
        /// </summary>
        public bool jpeg_start_decompress()
        {
            if (m_global_state == JpegState.DSTATE_READY)
            {
                m_master = new DecompMaster(this);
                if (m_buffered_image)
                {
                    m_global_state = JpegState.DSTATE_BUFIMAGE;
                    return true;
                }
                m_global_state = JpegState.DSTATE_PRELOAD;
            }

            if (m_global_state == JpegState.DSTATE_PRELOAD)
            {
                if (m_inputctl.HasMultipleScans())
                {
                    for ( ; ; )
                    {
                        ReadResult retcode;
                        if (m_progress != null)
                            m_progress.Updated();

                        retcode = m_inputctl.consume_input();
                        if (retcode == ReadResult.JPEG_SUSPENDED)
                            return false;

                        if (retcode == ReadResult.JPEG_REACHED_EOI)
                            break;

                        if (m_progress != null && (retcode == ReadResult.JPEG_ROW_COMPLETED || retcode == ReadResult.JPEG_REACHED_SOS))
                        {
                            m_progress.Pass_counter++;
                            if (m_progress.Pass_counter >= m_progress.Pass_limit)
                            {
                                m_progress.Pass_limit += m_total_iMCU_rows;
                            }
                        }
                    }
                }

                m_output_scan_number = m_input_scan_number;
            }

            return output_pass_setup();
        }

        /// <summary>
        /// Read some scanlines of data from the JPEG decompressor.
        /// </summary>
        public int jpeg_read_scanlines(byte[][] scanlines, int max_lines)
        {
            if (m_output_scanline >= m_output_height)
            {
                return 0;
            }

            if (m_progress != null)
            {
                m_progress.Pass_counter = m_output_scanline;
                m_progress.Pass_limit = m_output_height;
                m_progress.Updated();
            }

            int row_ctr = 0;
            m_main.process_data(scanlines, ref row_ctr, max_lines);
            m_output_scanline += row_ctr;
            return row_ctr;
        }

        /// <summary>
        /// Finish JPEG decompression.
        /// </summary>
        public bool jpeg_finish_decompress()
        {
            if ((m_global_state == JpegState.DSTATE_SCANNING || m_global_state == JpegState.DSTATE_RAW_OK) && !m_buffered_image)
            {
                m_master.finish_output_pass();
                m_global_state = JpegState.DSTATE_STOPPING;
            }
            else if (m_global_state == JpegState.DSTATE_BUFIMAGE)
            {
                m_global_state = JpegState.DSTATE_STOPPING;
            }
            else if (m_global_state != JpegState.DSTATE_STOPPING)
            {
            }

            while (!m_inputctl.EOIReached())
            {
                if (m_inputctl.consume_input() == ReadResult.JPEG_SUSPENDED)
                {
                    return false;
                }
            }

            m_src.term_source();

            jpeg_abort();
            return true;
        }

        /// <summary>
        /// Alternate entry point to read raw data.
        /// </summary>
        public int jpeg_read_raw_data(byte[][][] data, int max_lines)
        {
            if (m_output_scanline >= m_output_height)
            {
                return 0;
            }

            if (m_progress != null)
            {
                m_progress.Pass_counter = m_output_scanline;
                m_progress.Pass_limit = m_output_height;
                m_progress.Updated();
            }

            int lines_per_iMCU_row = m_max_v_samp_factor * m_min_DCT_scaled_size;
            int componentCount = data.Length;
            ComponentBuffer[] cb = new ComponentBuffer[componentCount];
            for (int i = 0; i < componentCount; i++)
            {
                cb[i] = new ComponentBuffer();
                cb[i].SetBuffer(data[i], null, 0);
            }

            if (m_coef.decompress_data(cb) == ReadResult.JPEG_SUSPENDED)
            {
                return 0;
            }

            m_output_scanline += lines_per_iMCU_row;
            return lines_per_iMCU_row;
        }

        /// <summary>
        /// Is there more than one scan?
        /// </summary>
        public bool jpeg_has_multiple_scans()
        {
            return m_inputctl.HasMultipleScans();
        }

        /// <summary>
        /// Initialize for an output pass in <see href="6dba59c5-d32e-4dfc-87fe-f9eff7004146.htm" target="_self">buffered-image mode</see>
        /// </summary>
        public bool jpeg_start_output(int scan_number)
        {
            if (scan_number <= 0)
                scan_number = 1;

            if (m_inputctl.EOIReached() && scan_number > m_input_scan_number)
                scan_number = m_input_scan_number;

            m_output_scan_number = scan_number;
            return output_pass_setup();
        }

        /// <summary>
        /// Finish up after an output pass in <see href="6dba59c5-d32e-4dfc-87fe-f9eff7004146.htm" target="_self">buffered-image mode</see>.
        /// </summary>
        public bool jpeg_finish_output()
        {
            if ((m_global_state == JpegState.DSTATE_SCANNING || m_global_state == JpegState.DSTATE_RAW_OK) && m_buffered_image)
            {
                m_master.finish_output_pass();
                m_global_state = JpegState.DSTATE_BUFPOST;
            }
            else if (m_global_state != JpegState.DSTATE_BUFPOST)
            {
            }

            while (m_input_scan_number <= m_output_scan_number && !m_inputctl.EOIReached())
            {
                if (m_inputctl.consume_input() == ReadResult.JPEG_SUSPENDED)
                {
                    return false;
                }
            }

            m_global_state = JpegState.DSTATE_BUFIMAGE;
            return true;
        }

        /// <summary>
        /// Indicates if we have finished reading the input file.
        /// </summary>
        public bool jpeg_input_complete()
        {
            return m_inputctl.EOIReached();
        }

        /// <summary>
        /// Consume data in advance of what the decompressor requires.
        /// </summary>
        public ReadResult jpeg_consume_input()
        {
            ReadResult retcode = ReadResult.JPEG_SUSPENDED;

            switch (m_global_state)
            {
                case JpegState.DSTATE_START:
                    jpeg_consume_input_start();
                    retcode = jpeg_consume_input_inHeader();
                    break;
                case JpegState.DSTATE_INHEADER:
                    retcode = jpeg_consume_input_inHeader();
                    break;
                case JpegState.DSTATE_READY:
                    /* Can't advance past first SOS until start_decompress is called */
                    retcode = ReadResult.JPEG_REACHED_SOS;
                    break;
                case JpegState.DSTATE_PRELOAD:
                case JpegState.DSTATE_PRESCAN:
                case JpegState.DSTATE_SCANNING:
                case JpegState.DSTATE_RAW_OK:
                case JpegState.DSTATE_BUFIMAGE:
                case JpegState.DSTATE_BUFPOST:
                case JpegState.DSTATE_STOPPING:
                    retcode = m_inputctl.consume_input();
                    break;
                default:
                    break;
            }
            return retcode;
        }

        /// <summary>
        /// Pre-calculate output image dimensions and related values for current decompression parameters.
        /// </summary>
        public void jpeg_calc_output_dimensions()
        {
            if (m_scale_num * 8 <= m_scale_denom)
            {
                m_output_width = JpegUtils.jdiv_round_up(m_image_width, 8);
                m_output_height = JpegUtils.jdiv_round_up(m_image_height, 8);
                m_min_DCT_scaled_size = 1;
            }
            else if (m_scale_num * 4 <= m_scale_denom)
            {
                m_output_width = JpegUtils.jdiv_round_up(m_image_width, 4);
                m_output_height = JpegUtils.jdiv_round_up(m_image_height, 4);
                m_min_DCT_scaled_size = 2;
            }
            else if (m_scale_num * 2 <= m_scale_denom)
            {
                m_output_width = JpegUtils.jdiv_round_up(m_image_width, 2);
                m_output_height = JpegUtils.jdiv_round_up(m_image_height, 2);
                m_min_DCT_scaled_size = 4;
            }
            else
            {
                /* Provide 1/1 scaling */
                m_output_width = m_image_width;
                m_output_height = m_image_height;
                m_min_DCT_scaled_size = JpegConstants.DCTSIZE;
            }

            for (int ci = 0; ci < m_num_components; ci++)
            {
                int ssize = m_min_DCT_scaled_size;
                while (ssize < JpegConstants.DCTSIZE && 
                    (m_comp_info[ci].H_samp_factor * ssize * 2 <= m_max_h_samp_factor * m_min_DCT_scaled_size) &&
                    (m_comp_info[ci].V_samp_factor * ssize * 2 <= m_max_v_samp_factor * m_min_DCT_scaled_size))
                {
                    ssize = ssize * 2;
                }

                m_comp_info[ci].DCT_scaled_size = ssize;
            }

            for (int ci = 0; ci < m_num_components; ci++)
            {
                m_comp_info[ci].downsampled_width = JpegUtils.jdiv_round_up(
                    m_image_width * m_comp_info[ci].H_samp_factor * m_comp_info[ci].DCT_scaled_size,
                    m_max_h_samp_factor * JpegConstants.DCTSIZE);

                m_comp_info[ci].downsampled_height = JpegUtils.jdiv_round_up(
                    m_image_height * m_comp_info[ci].V_samp_factor * m_comp_info[ci].DCT_scaled_size,
                    m_max_v_samp_factor * JpegConstants.DCTSIZE);
            }

            switch (m_out_color_space)
            {
                case J_COLOR_SPACE.JCS_GRAYSCALE:
                    m_out_color_components = 1;
                    break;
                case J_COLOR_SPACE.JCS_RGB:
                case J_COLOR_SPACE.JCS_YCbCr:
                    m_out_color_components = 3;
                    break;
                case J_COLOR_SPACE.JCS_CMYK:
                case J_COLOR_SPACE.JCS_YCCK:
                    m_out_color_components = 4;
                    break;
                default:
                    m_out_color_components = m_num_components;
                    break;
            }

            m_output_components = (m_quantize_colors ? 1 : m_out_color_components);

            if (use_merged_upsample())
                m_rec_outbuf_height = m_max_v_samp_factor;
            else
                m_rec_outbuf_height = 1;
        }

        /// <summary>
        /// Read or write the raw DCT coefficient arrays from a JPEG file (useful for lossless transcoding).
        /// </summary>
        public jvirt_array<JBLOCK>[] jpeg_read_coefficients()
        {
            if (m_global_state == JpegState.DSTATE_READY)
            {
                transdecode_master_selection();
                m_global_state = JpegState.DSTATE_RDCOEFS;
            }

            if (m_global_state == JpegState.DSTATE_RDCOEFS)
            {
                for ( ; ; )
                {
                    ReadResult retcode;
                    if (m_progress != null)
                        m_progress.Updated();

                    retcode = m_inputctl.consume_input();
                    if (retcode == ReadResult.JPEG_SUSPENDED)
                        return null;

                    if (retcode == ReadResult.JPEG_REACHED_EOI)
                        break;

                    if (m_progress != null && (retcode == ReadResult.JPEG_ROW_COMPLETED || retcode == ReadResult.JPEG_REACHED_SOS))
                    {
                        m_progress.Pass_counter++;
                        if (m_progress.Pass_counter >= m_progress.Pass_limit)
                        {
                            m_progress.Pass_limit += m_total_iMCU_rows;
                        }
                    }
                }

                m_global_state = JpegState.DSTATE_STOPPING;
            }

            if ((m_global_state == JpegState.DSTATE_STOPPING || m_global_state == JpegState.DSTATE_BUFIMAGE) && m_buffered_image)
                return m_coef.GetCoefArrays();

            return null;
        }

        /// <summary>
        /// Aborts processing of a JPEG decompression operation.
        /// </summary>
        public void jpeg_abort_decompress()
        {
            jpeg_abort();
        }

        /// <summary>
        /// Sets processor for special marker.
        /// </summary>
        public void jpeg_set_marker_processor(int marker_code, jpeg_marker_parser_method routine)
        {
            m_marker.jpeg_set_marker_processor(marker_code, routine);
        }

        /// <summary>
        /// Control saving of COM and APPn markers into <see cref="DecompressStruct.Marker_list">Marker_list</see>.
        /// </summary>
        public void jpeg_save_markers(int marker_code, int length_limit)
        {
            m_marker.jpeg_save_markers(marker_code, length_limit);
        }

        /// <summary>
        /// Determine whether merged upsample/color conversion should be used.
        /// </summary>
        internal bool use_merged_upsample()
        {
            if (m_do_fancy_upsampling || m_CCIR601_sampling)
                return false;

            if (m_jpeg_color_space != J_COLOR_SPACE.JCS_YCbCr || m_num_components != 3 ||
                m_out_color_space != J_COLOR_SPACE.JCS_RGB || m_out_color_components != JpegConstants.RGB_PIXELSIZE)
            {
                return false;
            }

            if (m_comp_info[0].H_samp_factor != 2 || m_comp_info[1].H_samp_factor != 1 ||
                m_comp_info[2].H_samp_factor != 1 || m_comp_info[0].V_samp_factor > 2 ||
                m_comp_info[1].V_samp_factor != 1 || m_comp_info[2].V_samp_factor != 1)
            {
                return false;
            }

            /* furthermore, it doesn't work if we've scaled the IDCTs differently */
            if (m_comp_info[0].DCT_scaled_size != m_min_DCT_scaled_size ||
                m_comp_info[1].DCT_scaled_size != m_min_DCT_scaled_size ||
                m_comp_info[2].DCT_scaled_size != m_min_DCT_scaled_size)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initialization of JPEG compression objects.
        /// </summary>
        private void initialize()
        {
            m_progress = null;
            m_src = null;

            for (int i = 0; i < JpegConstants.NUM_QUANT_TBLS; i++)
                m_quant_tbl_ptrs[i] = null;

            for (int i = 0; i < JpegConstants.NUM_HUFF_TBLS; i++)
            {
                m_dc_huff_tbl_ptrs[i] = null;
                m_ac_huff_tbl_ptrs[i] = null;
            }

            m_marker_list = new List<MarkerStruct>();
            m_marker = new MarkerReader(this);

            m_inputctl = new InputController(this);

            m_global_state = JpegState.DSTATE_START;
        }

        /// <summary>
        /// Master selection of decompression modules for transcoding (that is, reading 
        /// raw DCT coefficient arrays from an input JPEG file.)
        /// </summary>
        private void transdecode_master_selection()
        {
            m_buffered_image = true;

            if (m_progressive_mode)
                m_entropy = new PHuffEntropyDecoder(this);
            else
                m_entropy = new HuffEntropyDecoder(this);

            m_coef = new DCoefController(this, true);

            m_inputctl.start_input_pass();

            if (m_progress != null)
            {
                int nscans = 1;
                if (m_progressive_mode)
                {
                    nscans = 2 + 3 * m_num_components;
                }
                else if (m_inputctl.HasMultipleScans())
                {
                    nscans = m_num_components;
                }

                m_progress.Pass_counter = 0;
                m_progress.Pass_limit = m_total_iMCU_rows * nscans;
                m_progress.Completed_passes = 0;
                m_progress.Total_passes = 1;
            }
        }

        /// <summary>
        /// Set up for an output pass, and perform any dummy pass(es) needed.
        /// </summary>
        private bool output_pass_setup()
        {
            if (m_global_state != JpegState.DSTATE_PRESCAN)
            {
                m_master.prepare_for_output_pass();
                m_output_scanline = 0;
                m_global_state = JpegState.DSTATE_PRESCAN;
            }

            while (m_master.IsDummyPass())
            {
                while (m_output_scanline < m_output_height)
                {
                    int last_scanline;
                    if (m_progress != null)
                    {
                        m_progress.Pass_counter = m_output_scanline;
                        m_progress.Pass_limit = m_output_height;
                        m_progress.Updated();
                    }

                    last_scanline = m_output_scanline;
                    m_main.process_data(null, ref m_output_scanline, 0);
                    if (m_output_scanline == last_scanline)
                    {
                        return false;
                    }
                }

                m_master.finish_output_pass();
                m_master.prepare_for_output_pass();
                m_output_scanline = 0;
            }

            m_global_state = m_raw_data_out ? JpegState.DSTATE_RAW_OK : JpegState.DSTATE_SCANNING;
            return true;
        }

        /// <summary>
        /// Set default decompression parameters.
        /// </summary>
        private void default_decompress_parms()
        {
            switch (m_num_components)
            {
                case 1:
                    m_jpeg_color_space = J_COLOR_SPACE.JCS_GRAYSCALE;
                    m_out_color_space = J_COLOR_SPACE.JCS_GRAYSCALE;
                    break;

                case 3:
                    if (m_saw_JFIF_marker)
                    {
                        m_jpeg_color_space = J_COLOR_SPACE.JCS_YCbCr;
                    }
                    else if (m_saw_Adobe_marker)
                    {
                        switch (m_Adobe_transform)
                        {
                            case 0:
                                m_jpeg_color_space = J_COLOR_SPACE.JCS_RGB;
                                break;
                            case 1:
                                m_jpeg_color_space = J_COLOR_SPACE.JCS_YCbCr;
                                break;
                            default:
                                m_jpeg_color_space = J_COLOR_SPACE.JCS_YCbCr; /* assume it's YCbCr */
                                break;
                        }
                    }
                    else
                    {
                        /* Saw no special markers, try to guess from the component IDs */
                        int cid0 = m_comp_info[0].Component_id;
                        int cid1 = m_comp_info[1].Component_id;
                        int cid2 = m_comp_info[2].Component_id;

                        if (cid0 == 1 && cid1 == 2 && cid2 == 3)
                        {
                            /* assume JFIF w/out marker */
                            m_jpeg_color_space = J_COLOR_SPACE.JCS_YCbCr;
                        }
                        else if (cid0 == 82 && cid1 == 71 && cid2 == 66)
                        {
                            m_jpeg_color_space = J_COLOR_SPACE.JCS_RGB;
                        }
                        else
                        {
                            m_jpeg_color_space = J_COLOR_SPACE.JCS_YCbCr;
                        }
                    }
                    m_out_color_space = J_COLOR_SPACE.JCS_RGB;
                    break;

                case 4:
                    if (m_saw_Adobe_marker)
                    {
                        switch (m_Adobe_transform)
                        {
                            case 0:
                                m_jpeg_color_space = J_COLOR_SPACE.JCS_CMYK;
                                break;
                            case 2:
                                m_jpeg_color_space = J_COLOR_SPACE.JCS_YCCK;
                                break;
                            default:
                                m_jpeg_color_space = J_COLOR_SPACE.JCS_YCCK;
                                break;
                        }
                    }
                    else
                    {
                        m_jpeg_color_space = J_COLOR_SPACE.JCS_CMYK;
                    }

                    m_out_color_space = J_COLOR_SPACE.JCS_CMYK;
                    break;

                default:
                    m_jpeg_color_space = J_COLOR_SPACE.JCS_UNKNOWN;
                    m_out_color_space = J_COLOR_SPACE.JCS_UNKNOWN;
                    break;
            }

            m_scale_num = 1;       /* 1:1 scaling */
            m_scale_denom = 1;
            m_buffered_image = false;
            m_raw_data_out = false;
            m_dct_method = JpegConstants.JDCT_DEFAULT;
            m_do_fancy_upsampling = true;
            m_do_block_smoothing = true;
            m_quantize_colors = false;

            /* We set these in case application only sets quantize_colors. */
            m_dither_mode = J_DITHER_MODE.JDITHER_FS;
            m_two_pass_quantize = true;
            m_desired_number_of_colors = 256;
            m_colormap = null;

            /* Initialize for no mode change in buffered-image mode. */
            m_enable_1pass_quant = false;
            m_enable_external_quant = false;
            m_enable_2pass_quant = false;
        }

        private void jpeg_consume_input_start()
        {
            m_inputctl.reset_input_controller();

            m_src.init_source();
            m_global_state = JpegState.DSTATE_INHEADER;
        }

        private ReadResult jpeg_consume_input_inHeader()
        {
            ReadResult retcode = m_inputctl.consume_input();
            if (retcode == ReadResult.JPEG_REACHED_SOS)
            {
                default_decompress_parms();

                m_global_state = JpegState.DSTATE_READY;
            }

            return retcode;
        }
    }

    /// <summary>
    /// Basic info about one component (color channel).
    /// </summary>
    class ComponentInfo
    {
        private int component_id;
        private int component_index;
        private int h_samp_factor;
        private int v_samp_factor;
        private int quant_tbl_no;
        private int dc_tbl_no;
        private int ac_tbl_no;
        private int width_in_blocks;
        internal int height_in_blocks;
        internal int DCT_scaled_size;
        internal int downsampled_width;   
        internal int downsampled_height;
        internal bool component_needed; 
        internal int MCU_width; 
        internal int MCU_height;  
        internal int MCU_blocks;  
        internal int MCU_sample_width;   
        internal int last_col_width; 
        internal int last_row_height;  
        internal JQUANT_TBL quant_table;

        internal ComponentInfo()
        {
        }

        internal void Assign(ComponentInfo ci)
        {
            component_id = ci.component_id;
            component_index = ci.component_index;
            h_samp_factor = ci.h_samp_factor;
            v_samp_factor = ci.v_samp_factor;
            quant_tbl_no = ci.quant_tbl_no;
            dc_tbl_no = ci.dc_tbl_no;
            ac_tbl_no = ci.ac_tbl_no;
            width_in_blocks = ci.width_in_blocks;
            height_in_blocks = ci.height_in_blocks;
            DCT_scaled_size = ci.DCT_scaled_size;
            downsampled_width = ci.downsampled_width;
            downsampled_height = ci.downsampled_height;
            component_needed = ci.component_needed;
            MCU_width = ci.MCU_width;
            MCU_height = ci.MCU_height;
            MCU_blocks = ci.MCU_blocks;
            MCU_sample_width = ci.MCU_sample_width;
            last_col_width = ci.last_col_width;
            last_row_height = ci.last_row_height;
            quant_table = ci.quant_table;
        }

        /// <summary>
        /// Identifier for this component (0..255)
        /// </summary>
        public int Component_id
        {
            get { return component_id; }
            set { component_id = value; }
        }

        /// <summary>
        /// Its index in SOF or <see cref="DecompressStruct.Comp_info"/>.
        /// </summary>
        public int Component_index
        {
            get { return component_index; }
            set { component_index = value; }
        }

        /// <summary>
        /// Horizontal sampling factor (1..4)
        /// </summary>
        public int H_samp_factor
        {
            get { return h_samp_factor; }
            set { h_samp_factor = value; }
        }

        /// <summary>
        /// Vertical sampling factor (1..4)
        /// </summary>
        public int V_samp_factor
        {
            get { return v_samp_factor; }
            set { v_samp_factor = value; }
        }

        /// <summary>
        /// Quantization table selector (0..3)
        /// </summary>
        public int Quant_tbl_no
        {
            get { return quant_tbl_no; }
            set { quant_tbl_no = value; }
        }

        /// <summary>
        /// DC entropy table selector (0..3)
        /// </summary>
        public int Dc_tbl_no
        {
            get { return dc_tbl_no; }
            set { dc_tbl_no = value; }
        }

        /// <summary>
        /// AC entropy table selector (0..3)
        /// </summary>
        public int Ac_tbl_no
        {
            get { return ac_tbl_no; }
            set { ac_tbl_no = value; }
        }

        /// <summary>
        /// Gets or sets the width in blocks.
        /// </summary>
        public int Width_in_blocks
        {
            get { return width_in_blocks; }
            set { width_in_blocks = value; }
        }

        /// <summary>
        /// Gets the downsampled width.
        /// </summary>
        public int Downsampled_width
        {
            get { return downsampled_width; }
        }

        internal static ComponentInfo[] createArrayOfComponents(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            ComponentInfo[] result = new ComponentInfo[length];
            for (int i = 0; i < result.Length; ++i)
                result[i] = new ComponentInfo();

            return result;
        }
    }

    /// <summary>
    /// Representation of special JPEG marker.
    /// </summary>
    class MarkerStruct
    {
        private byte m_marker;
        private int m_originalLength;
        private byte[] m_data;

        internal MarkerStruct(byte marker, int originalDataLength, int lengthLimit)
        {
            m_marker = marker;
            m_originalLength = originalDataLength;
            m_data = new byte[lengthLimit];
        }

        /// <summary>
        /// Gets the special marker.
        /// </summary>
        public byte Marker
        {
            get
            {
                return m_marker;
            }
        }

        /// <summary>
        /// Gets the full length of original data associated with the marker.
        /// </summary>
        public int OriginalLength
        {
            get
            {
                return m_originalLength;
            }
        }

        /// <summary>
        /// Gets the data associated with the marker.
        /// </summary>
        public byte[] Data
        {
            get
            {
                return m_data;
            }
        }
    }
}
