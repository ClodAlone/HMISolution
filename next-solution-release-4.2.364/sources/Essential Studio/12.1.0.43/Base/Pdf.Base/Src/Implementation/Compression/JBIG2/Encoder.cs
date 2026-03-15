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
using System.Drawing;
using System.Drawing.Imaging;
using Syncfusion.Pdf.Compression.JBIG2;

namespace Syncfusion.Pdf.Compression
{
    internal class PdfJBIG2Compressor
    {
        private float m_threshold = 0.85f;
        private int m_bwThreshold = 188;
        private bool m_up2 = false, m_up4 = false;
        private int i;
        int pageno = -1;
        int numsubimages = 0, subimage = 0;
        int m_removeCmapToBinary = 0, m_removeCmapToGrayScale = 1, m_removeCmapToFullColor = 2, m_removeCmapBasedOnSrc = 3;
        JBIG2Context m_jbig2Context;
        int MORPH_BC = 1;
        const int m_red = 0, m_green = 1, m_blue = 2, m_alpha = 3;
        int m_redShift = 8 * (sizeof(uint) - 1 - m_red);           /* 24 */
        int m_greenShift = 8 * (sizeof(uint) - 1 - m_green);         /* 16 */
        int m_blueShift = 8 * (sizeof(uint) - 1 - m_blue);          /*  8 */
        int m_alphaShift = 8 * (sizeof(uint) - 1 - m_alpha);     /*  0 */
        const int m_insert = 0, m_copy = 1, m_clone = 2, m_copyClone = 3;
        const int m_jbAddedPixels = 6;
        const int m_maxDiffWidth = 2; 
        const int m_maxDiffHeight = 2;
        IntEncRange[] m_intEncRange = new IntEncRange[] { new IntEncRange(0, 3, 0, 2, 0, 2), new IntEncRange(-1, -1, 9, 4, 0, 0), new IntEncRange(-3, -2, 5, 3, 2, 1), new IntEncRange(4, 19, 2, 3, 4, 4), new IntEncRange(-19, -4, 3, 3, 4, 4), new IntEncRange(20, 83, 6, 4, 20, 6), new IntEncRange(-83, -20, 7, 4, 20, 6),
         new IntEncRange( 84, 339, 14, 5, 84, 8), new IntEncRange( -339, -84, 15, 5, 84, 8), new IntEncRange( 340, 4435, 30, 6, 340, 12), new IntEncRange(-4435, -340, 31, 6, 340, 12), new IntEncRange(4436, 2000000000, 62, 6, 4436, 32), new IntEncRange(-2000000000, -4436, 63, 6, 4436, 32) };
        private List<byte> m_symbolArray;
        private List<byte> m_symbolPage;

        internal List<byte> SymbolArray
        {
            get
            { 
                return m_symbolArray; 
            }
            set 
            {
                m_symbolArray = value;
            }
        }

        internal List<byte> SymbolPage
        {
            get
            {
                return m_symbolPage;
            }
            set
            {
                m_symbolPage = value;
            }
        }

        internal void Dispose()
        {
            if (m_symbolPage != null)
                m_symbolPage.Clear();
            if (m_symbolArray != null)
                m_symbolArray.Clear();
        }

        /// <summary>
        /// Compresses the TIFF using JBIG2 encoder.
        /// </summary>
        internal PdfJBIG2Compressor(Object file)
        {
            int num_pages = 0;
            Initialize();

            Pix source = PixRead(file);
            if (source == null) // color image.
                return;
            Pix pixl, gray, pixt;

            if ((pixl = PixRemoveColormap(source, m_removeCmapBasedOnSrc)) == null)
            {
                Console.WriteLine("Failed to remove colormap");
                return;
            }

            pageno++;

            if (pixl.D > 1)
            {
                if (pixl.D > 8)
                {
                    gray = PixConvertRGBToGrayFast(pixl);
                    if (gray == null)
                        return;
                }
                else
                    gray = pixl;
                if (m_up2)
                    pixt = PixScaleGray2xLIThresh(gray, m_bwThreshold);
                else if (m_up4)
                    pixt = PixScaleGray4xLIThresh(gray, m_bwThreshold);
                else
                    pixt = PixThresholdToBinary(gray, m_bwThreshold);
            }
            else
                pixt = pixl;

            AddPage(m_jbig2Context, pixt);
            num_pages++;

            int length = -1;
            SymbolArray = JBIG2PagesComplete(m_jbig2Context); //".sym"
            
            for (int i = 0; i < num_pages; ++i)
                SymbolPage = JBIG2ProducePage(m_jbig2Context, i, -1, -1, ref length); //".0000"
        }

        /// <summary>
        /// Read Pix information for the image.
        /// </summary>
        private Pix PixRead(Object file)
        {
            Pix pix = null;
            //int format = FindFileFormatStream(file);
            //switch (format)
            //{
            //    case JBIG2Statics.Tiff:
            //    case JBIG2Statics.TiffPackBits:
            //    case JBIG2Statics.TiffRle:
            //    case JBIG2Statics.TiffG3:
            //    case JBIG2Statics.TiffG4:
            //    case JBIG2Statics.TiffLzw:
            //    case JBIG2Statics.TiffZip:
            //        if ((pix = PixReadStreamTiff(file)) == null)
            //            return null;
            //        break;
            //}
            pix = PixReadStreamTiff(file);
            //if (pix != null)
            //    pix.Informat = format; //pixSetInputFormat(pix, format);
            return pix;
        }

        /// <summary>
        /// Read pix information from Tiff.
        /// </summary>
        Pix PixReadStreamTiff(Object file)
        {
            if (file is Stream)
            {
                using (Stream temp = (file as Stream))
                {
                    temp.Position = 0;
                    byte[] array = new byte[temp.Length];
                    temp.Read(array, 0, array.Length);

                    using (MemoryStream ms = new MemoryStream(array))
                    {
                        using (Tiff tiff = Tiff.ClientOpen("in-memory", "r", ms, new TiffStream()))
                            return ReadTiff(tiff);
                    }
                }
            }
            else if (file is string)
            {
                using (Tiff tiff = Tiff.Open(file as string, "r"))
                    return ReadTiff(tiff);
            }

            return null;
        }

        /// <summary>
        /// Create Pix from the tiff.
        /// </summary>
        Pix ReadTiff(Tiff tiff)
        {
            if (tiff != null)
            {
                int w = tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.IMAGEWIDTH)[0].ToInt();
                int h = tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.IMAGELENGTH)[0].ToInt();
                int bps = tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.BITSPERSAMPLE)[0].ToInt();
                int spp = tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.SAMPLESPERPIXEL)[0].ToInt();
                int xres = tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.XRESOLUTION)[0].ToInt();
                int yres = tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.YRESOLUTION)[0].ToInt();
                int compression = tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.COMPRESSION)[0].ToInt();
                int d = 0;
                PlanarConfig planarconfig = (PlanarConfig)tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.PLANARCONFIG)[0].ToInt();

                int bpp = bps * spp;
                if (bpp > 32)
                    throw new Exception();
                if (spp == 1)
                    d = bps;
                else if (spp == 3 || spp == 4)
                    d = 32;

                int wpl = (w * d + 31) / 32;
                int bpl = 4 * wpl;

                Pix pix = new Pix();
                pix.W = w;
                pix.H = h;
                pix.D = d;
                pix.Wpl = wpl;
                pix.XRes = xres;
                pix.YRes = yres;
                pix.Informat = GetTiffCompressedFormat(compression);

                if (spp == 1)// black and white
                {
                    int len = tiff.ScanlineSize() + 1;
                    uint[] src = new uint[h * wpl];
                    int c = 0;
                    for (int i = 0; i < h; i++)
                    {
                        byte[] temp = new byte[bpl];
                        if (tiff.ReadScanline(temp, i, 0))
                        {
                            for (int k = 0; k < temp.Length; k++)
                            {
                                byte[] com = new byte[] { temp[k], temp[++k], temp[++k], temp[++k] };
                                src[c++] = BitConverter.ToUInt32(com, 0);
                            }
                        }
                    }

                    if (bps <= 8)
                        PixEndianByteSwap(pix, src);
                    else
                        PixEndianTwoByteSwap(pix, src);
                }
                else //color image.
                {
                    return null;
                    //int[] raster = new int[w * h];
                    //uint[] src = new uint[raster.Length];
                    //bool isokay = tiff.ReadRGBAImageOriented(w, h, raster, Orientation.TOPLEFT, false);

                    //if (isokay)
                    //{
                    //    for (int i = 0; i < h * w; i++)
                    //    {
                    //        byte[] colors = BitConverter.GetBytes(raster[i]);
                    //        byte rval = colors[2];
                    //        byte gval = colors[1];
                    //        byte bval = colors[0];
                    //        src[i] = (uint)((rval << m_redShift) | (gval << m_greenShift) | (bval << m_blueShift));
                    //    }
                    //    pix.Data = src;
                    //}
                }

                FieldValue[] result = tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.COLORMAP);
                if (result != null)
                {
                    short[] red_orig = result[0].ToShortArray();
                    short[] green_orig = result[1].ToShortArray();
                    short[] blue_orig = result[2].ToShortArray();

                    int n_color = (1 << bps);
                    ushort[] redcmap = new ushort[n_color];
                    ushort[] greencmap = new ushort[n_color];
                    ushort[] bluecmap = new ushort[n_color];

                    Buffer.BlockCopy(red_orig, 0, redcmap, 0, n_color * sizeof(ushort));
                    Buffer.BlockCopy(green_orig, 0, greencmap, 0, n_color * sizeof(ushort));
                    Buffer.BlockCopy(blue_orig, 0, bluecmap, 0, n_color * sizeof(ushort));

                    if (planarconfig == PlanarConfig.CONTIG && spp != 1 && bps < 8)
                    {
                    }
                    else
                    {
                        PixColormap cmap = JBIG2Statics.CreatePixCmap(bps);
                        int ncolors = 1 << bps;
                        for (i = 0; i < ncolors; i++)
                            PixCmapAddColor(ref cmap, redcmap[i] >> 8, greencmap[i] >> 8, bluecmap[i] >> 8);
                        pix.Colormap = cmap;
                    }
                }
                result = tiff.GetField(Syncfusion.Pdf.Compression.JBIG2.TiffTag.ORIENTATION);
                if (result != null)
                {
                    int orientation = result[0].ToInt();
                    if (orientation >= 1 && orientation <= 8)
                    {
                        //tiff_transform transform = tiff_orientation_transforms[orientation - 1];
                        //if (transform.vflip != 0) pixFlipTB(pix, pix);
                        //if (transform.hflip != 0) pixFlipLR(pix, pix);
                        //if (transform.rotate != 0) {
                        //    Pix oldpix = pix;
                        //    pix = pixRotate90(oldpix, transform->rotate);
                        //}
                    }
                }
                return pix;
            }
            return null;
        }

        /// <summary>
        /// Swap bytes for Little Endian byte order.
        /// </summary>
        void PixEndianByteSwap(Pix pixs, uint[] src)
        {
            List<uint> data;
            int i, j, h, wpl;
            uint word;

            if (!BitConverter.IsLittleEndian)
                return;
            else
            {
                wpl = pixs.Wpl;
                h = pixs.H;
                data = new List<uint>();
                int index = 0;
                for (i = 0; i < h; i++)
                {
                    for (j = 0; j < wpl; j++, index++)
                    {
                        word = src[index];
                        uint temp = (uint)((word >> 24) | ((word >> 8) & 0x0000ff00) | ((word << 8) & 0x00ff0000) | (word << 24));
                        data.Add(temp);
                    }
                }

                pixs.Data = data.ToArray();
            }
        }

        /// <summary>
        /// Swap bytes for Little Endian byte order.
        /// </summary>
        void PixEndianTwoByteSwap(Pix pixs, uint[] src)
        {
            uint[] data;
            int i, j, h, wpl;
            uint word;

            if (!BitConverter.IsLittleEndian)
                return;
            else
            {
                wpl = pixs.Wpl;
                h = pixs.H;
                data = new uint[h * wpl];
                for (i = 0; i < h * wpl; i++)
                {
                    word = src[i];
                    data[i] = (word << 16) | (word >> 16);
                }

                pixs.Data = data;
            }
        }

        //private List<int[]> listToMatrix(int[] list, int elementsPerSubArray)
        //{
        //    List<int[]> matrix = new List<int[]>();
        //    int i, k = -1, c = 0;

        //    for (i = 0, k = -1; i < list.Length; i++)
        //    {
        //        if (i % elementsPerSubArray == 0)
        //        {
        //            k++;
        //            c = 0;
        //            matrix.Add(new int[elementsPerSubArray]);
        //        }
        //        matrix[k][c++] = list[i];
        //    }

        //    return matrix;
        //}

        /// <summary>
        /// Read Tiff compression format.
        /// </summary>
        int GetTiffCompressedFormat(int tiffcomp)
        {
            int comptype;

            switch (tiffcomp)
            {
                case JBIG2Statics.CompressionCcittFax4:
                    comptype = JBIG2Statics.TiffG4;
                    break;
                case JBIG2Statics.CompressionCcittFax3:
                    comptype = JBIG2Statics.TiffG3;
                    break;
                case JBIG2Statics.CompressionCcittRle:
                    comptype = JBIG2Statics.TiffRle;
                    break;
                case JBIG2Statics.CompressionPackBits:
                    comptype = JBIG2Statics.TiffPackBits;
                    break;
                case JBIG2Statics.CompressionLzw:
                    comptype = JBIG2Statics.TiffLzw;
                    break;
                case JBIG2Statics.CompressionAdobeDeflate:
                    comptype = JBIG2Statics.TiffZip;
                    break;
                default:
                    comptype = JBIG2Statics.Tiff;
                    break;
            }
            return comptype;
        }

        /// <summary>
        /// Read file format.
        /// </summary>
        int FindFileFormatStream(object fp)
        {
            int format = 0;
            Image img = null;
            Stream str = fp as Stream;
            if (fp is string)
                img = Image.FromFile(fp as string);
            else
                img = Image.FromStream(str);

            if (ImageFormat.Jpeg.Equals(img.RawFormat))
                format = 2;
            else if (ImageFormat.Png.Equals(img.RawFormat))
                format = 3;
            else if (ImageFormat.Gif.Equals(img.RawFormat))
                format = 13;
            else if (ImageFormat.Tiff.Equals(img.RawFormat))
                format = 4;
            else if (ImageFormat.Bmp.Equals(img.RawFormat))
                format = 1;
            else
                return 1;

            img.Dispose();
            return format;
        }

        /// <summary>
        /// Initialize encoder.
        /// </summary>
        void Initialize()
        {
            m_jbig2Context = new JBIG2Context(m_threshold, 0.5f, 0, 0, false);
        }

        int CompareCharArray(int[] buf, char[] comp)
        {
            for (int i = 0; i < buf.Length; i++)
            {
                if ((char)buf[i] != comp[i])
                    return 0;
            }

            return 1;
        }

        List<byte> JBIG2ProducePage(JBIG2Context ctx, int page_no, int xres, int yres, ref int length)
        {
            bool last_page = page_no == ctx.Classifier.NPages;
            bool include_trailer = last_page && ctx.FullHeaders;
            int segment_end_of_file = 51;
            int segment_imm_text_region = 6;
            int segment_end_of_page = 49;
            JBIG2EncoderContext ectx = new JBIG2EncoderContext();
            Initialize(ref ectx);

            Segment seg = new Segment(), symseg = new Segment();
            Segment endseg = new Segment(), trailerseg = new Segment();
            JBIG2PageInfo pageinfo = new JBIG2PageInfo();
            JBIG2TextRegion textreg = new JBIG2TextRegion();
            JBIG2TextRegionSym textreg_syminsts = new JBIG2TextRegionSym();
            JBIG2TextRegionAtFlags textreg_atflags = new JBIG2TextRegionAtFlags();
            Segment segr = new Segment();

            // page information segment
            seg.Number = (uint)ctx.SegNumber;
            ctx.SegNumber++;
            seg.SType = 48;// segment_page_information;
            seg.Page = (uint)(ctx.PDFPageNumbering ? 1 : 1 + page_no);
            seg.Length = 19;// (uint)System.Runtime.InteropServices.Marshal.SizeOf(pageinfo);

            pageinfo.Width = JBIG2Statics.Htonl(ctx.PageWidth[page_no]);
            pageinfo.Height = JBIG2Statics.Htonl(ctx.PageHeight[page_no]);
            pageinfo.XRes = JBIG2Statics.Htonl(xres == -1 ? ctx.PageXRes[page_no] : xres);
            pageinfo.YRes = JBIG2Statics.Htonl(yres == -1 ? ctx.PageYRes[page_no] : yres);
            pageinfo.IsLossless = (byte)(ctx.Refinement ? 1 : 0);

            SortedDictionary<int, int> second_symbol_map = new SortedDictionary<int, int>();
            bool extrasymtab = ctx.SingleUseSymbols.ContainsKey(page_no) ? ctx.SingleUseSymbols[page_no].Count > 0 : false;
            JBIG2EncoderContext extrasymtab_ctx = new JBIG2EncoderContext();
            JBIG2SymbolDict symtab = new JBIG2SymbolDict();

            if (extrasymtab)
            {
                Initialize(ref extrasymtab_ctx);
                symseg.Number = (uint)ctx.SegNumber++;
                symseg.SType = 0;// segment_symbol_table;
                symseg.Page = (uint)(ctx.PDFPageNumbering ? 1 : 1 + page_no);
                List<int> temp = ctx.SingleUseSymbols[page_no];
                JBIG2SymbolTable(ref extrasymtab_ctx, (ctx.AvgTemplates != null ? ctx.AvgTemplates : ctx.Classifier.Pixat), ref temp, ref second_symbol_map, (ctx.AvgTemplates == null));
                ctx.SingleUseSymbols[page_no] = temp;
                symtab.a1x = 3;
                symtab.a1y = -1;
                symtab.a2x = -3;
                symtab.a2y = -1;
                symtab.a3x = 2;
                symtab.a3y = -2;
                symtab.a4x = -2;
                symtab.a4y = -2;
                symtab.ExSyms = symtab.NewSyms = JBIG2Statics.Htonl(ctx.SingleUseSymbols[page_no].Count);

                symseg.Length = (byte)(JBIG2EncSize(extrasymtab_ctx) + System.Runtime.InteropServices.Marshal.SizeOf(symtab));
            }

            int numsyms = ctx.NumGlobalSymbols + (ctx.SingleUseSymbols.ContainsKey(page_no) ? ctx.SingleUseSymbols[page_no].Count : 0);
            //Box const boxes = ctx.Refinement ? ctx.Boxes[page_no] : null;
            int baseindex = ctx.Refinement ? (ctx.BaseIndexes.Contains(page_no) ? ctx.BaseIndexes[page_no] : 0) : 0;
            JBIG2EncText(ectx, ctx.SymbolMap, second_symbol_map, ctx.PageComps[page_no], ctx.Classifier.PtaLL,
                                (ctx.AvgTemplates != null ? ctx.AvgTemplates : ctx.Classifier.Pixat), ctx.Classifier.NaClass, 1, Log2Up(numsyms), //ctx.Refinement ? ctx.Comps[page_no] : null,
                                null, /* boxes */ null, baseindex, ctx.RefineLevel, ctx.AvgTemplates == null);
            int textdatasize = (int)JBIG2EncSize(ectx);
            textreg.Width = (uint)JBIG2Statics.Htonl(ctx.PageWidth[page_no]);
            textreg.Height = (uint)JBIG2Statics.Htonl(ctx.PageHeight[page_no]);
            textreg.LogSBStrips = 0;
            textreg.SBRefine = (byte)(ctx.Refinement ? 1 : 0);
            textreg_syminsts.SBNumInstances = (uint)JBIG2Statics.Htonl(ctx.PageComps[page_no].Count);

            textreg_atflags.a1x = -1;
            textreg_atflags.a1y = -1;
            textreg_atflags.a2x = -1;
            textreg_atflags.a2y = -1;

            segr.Number = (uint)ctx.SegNumber;
            ctx.SegNumber++;
            segr.SType = (byte)segment_imm_text_region;
            segr.ReferredTo.Add(ctx.SymbolTableSegment);
            if (extrasymtab)
                segr.ReferredTo.Add((int)symseg.Number);
            if (ctx.Refinement)
                segr.Length = (uint)(System.Runtime.InteropServices.Marshal.SizeOf(textreg) + System.Runtime.InteropServices.Marshal.SizeOf(textreg_syminsts) + System.Runtime.InteropServices.Marshal.SizeOf(textreg_atflags) + textdatasize);
            else
                segr.Length = (uint)(23 + textdatasize);// (uint)(System.Runtime.InteropServices.Marshal.SizeOf(textreg) + System.Runtime.InteropServices.Marshal.SizeOf(textreg_syminsts) + textdatasize);

            segr.RetainBits = 2;
            segr.Page = (uint)(ctx.PDFPageNumbering ? 1 : 1 + page_no);

            int extrasymtab_size = (extrasymtab ? JBIG2EncSize(extrasymtab_ctx) : 0);

            if (ctx.FullHeaders)
            {
                endseg.Number = (uint)ctx.SegNumber;
                ctx.SegNumber++;
                endseg.SType = segment_end_of_page;
                endseg.Page = (uint)(ctx.PDFPageNumbering ? 1 : 1 + page_no);
            }

            if (include_trailer)
            {
                trailerseg.Number = (uint)ctx.SegNumber;
                ctx.SegNumber++;
                trailerseg.SType = segment_end_of_file;
                trailerseg.Page = 0;
            }

            int totalsize = (int)(seg.Length + System.Runtime.InteropServices.Marshal.SizeOf(pageinfo) + (extrasymtab ? (extrasymtab_size + symseg.Length + System.Runtime.InteropServices.Marshal.SizeOf(symtab)) : 0) + segr.Length + System.Runtime.InteropServices.Marshal.SizeOf(textreg) + System.Runtime.InteropServices.Marshal.SizeOf(textreg_syminsts) +
                                  (ctx.Refinement ? System.Runtime.InteropServices.Marshal.SizeOf(textreg_atflags) : 0) + textdatasize + (ctx.FullHeaders ? endseg.Length : 0) + (include_trailer ? trailerseg.Length : 0));

            List<byte> ret = new List<byte>(); // = (char*)malloc(totalsize);
            int offset = 0;

            UpdateSegment(seg, ref offset, ref ret);
            Fill(pageinfo, ref offset, ref ret);
            if (extrasymtab)
            {
                UpdateSegment(symseg, ref offset, ref ret);
                Fill(symtab, ref offset, ref ret);
                JBIG2EncBuffer(extrasymtab_ctx, ref ret);//.Count + offset);
                offset += extrasymtab_size;
            }
            UpdateSegment(segr, ref offset, ref ret);
            Fill(textreg, ref offset, ref ret);
            if (ctx.Refinement)
                Fill(textreg_atflags, ref offset, ref ret);
            Fill(textreg_syminsts, ref offset, ref ret);
            JBIG2EncBuffer(ectx, ref ret);//.Count + offset);
            offset = ret.Count;
            if (ctx.FullHeaders)
                UpdateSegment(endseg, ref offset, ref ret);
            if (include_trailer)
                UpdateSegment(trailerseg, ref offset, ref ret);

            if (offset == 0)
                return null;

            ectx = null;
            if (extrasymtab)
                extrasymtab_ctx = null;

            length = offset;
            return ret;
        }

        int Log2Up(int v)
        {
            int r = 0;
            bool is_pow_of_2 = (v & (v - 1)) == 0;

            while ((v = v >> 1) > 0) r++;
            if (is_pow_of_2) return r;

            return r + 1;
        }

        void UpdateSegment(Segment x, ref int offset, ref List<byte> ret)
        {
            x.Write(ret);// + offset);
            offset = ret.Count;
        }

        void JBIG2EncText(JBIG2EncoderContext ctx, SortedDictionary<int, int> symmap, SortedDictionary<int, int> symmap2, List<int> comps, Pta in_ll, Pixa symbols, Numa assignments, int stripwidth, int symbits, Pixa source, Boxa boxes, int baseindex, int refine_level, bool unborder_symbols)
        {
            int JBIG2_IAAI = 0, JBIG2_IADH = 1, JBIG2_IADS = 2, JBIG2_IADT = 3, JBIG2_IADW = 4, JBIG2_IAEX = 5, JBIG2_IAFS = 6, JBIG2_IAIT = 7, JBIG2_IARDH = 8, JBIG2_IARDW = 9, JBIG2_IARDX = 10, JBIG2_IARDY = 11, JBIG2_IARI = 12;
            const int kBorderSize = 6;

            if (stripwidth != 1 && stripwidth != 2 && stripwidth != 4 && stripwidth != 8)
                return;

            Pta ll;

            if (source != null)
            {
                ll = JBIG2Statics.CreatePta(0);
                for (int i = 0; i < boxes.N; ++i)
                    PtaAddPt(ll, boxes.Box[i].X, boxes.Box[i].Y + boxes.Box[i].H - 1);
            }
            else
                ll = in_ll;

            int n = comps.Count;
            List<int> syms = comps;

            HeightSorter(ref syms, ll);

            int stript = 0;
            int firsts = 0;
            IntegerEncoder(ctx, JBIG2_IADT, 0);

            List<int> strip = new List<int>(); // elements of strip: I
            for (int i = 0; i < n; )
            {   // i: II
                int height = (int)(ll.Y[syms[i]] / stripwidth) * stripwidth;
                int j;
                strip.Clear();
                strip.Add(syms[i]);

                for (j = i + 1; j < n; ++j)
                {  // j: II
                    if (ll.Y[syms[j]] < height)
                        return;
                    if (ll.Y[syms[j]] >= height + stripwidth)
                        break;
                    strip.Add(syms[j]);
                }

                WidthSorter(ref strip, ll);
                int deltat = height - stript;
                IntegerEncoder(ctx, JBIG2_IADT, deltat / stripwidth);
                stript = height;

                bool firstsymbol = true;
                int curs = 0;
                foreach (int k in strip)
                {
                    int sym = k;  // sym: I
                    if (firstsymbol)
                    {
                        firstsymbol = false;
                        int deltafs = (int)(ll.X[sym] - firsts);
                        IntegerEncoder(ctx, JBIG2_IAFS, deltafs);
                        firsts += deltafs;
                        curs = firsts;
                    }
                    else
                    {
                        int deltas = (int)(ll.X[sym] - curs);
                        IntegerEncoder(ctx, JBIG2_IADS, deltas);
                        curs += deltas;
                    }

                    if (stripwidth > 1)
                    {
                        int deltatt = (int)(ll.Y[sym] - stript);
                        IntegerEncoder(ctx, JBIG2_IAIT, deltatt);
                    }

                    int assigned = (int)assignments.Array[sym + (source != null ? baseindex : 0)];
                    int symid;
                    if (symmap.ContainsKey(assigned))
                        symid = symmap[assigned];
                    else if (symmap2.ContainsKey(assigned))
                        symid = symmap2[assigned] + symmap.Count;
                    else
                        return;

                    JBIG2EncIaid(ctx, symbits, symid);

                    // refinement is enabled if the original source components are given
                    if (source != null)
                    {
                        int abssym = baseindex + sym;

                        Pix symbol;
                        if (unborder_symbols)
                            symbol = PixRemoveBorder(symbols.Pix[assigned], kBorderSize);
                        else
                            symbol = symbols.Pix[assigned];
                        PixSetPadBits(ref symbol, 0);

                        int targetw = boxes.Box[sym].W;
                        int targeth = boxes.Box[sym].H;
                        int targetx = boxes.Box[sym].X;
                        int targety = boxes.Box[sym].Y;

                        int symboly = (int)(in_ll.Y[abssym] - symbol.H) + 1;
                        int symbolx = (int)in_ll.X[abssym];

                        int deltaw = targetw - symbol.W;
                        int deltah = targeth - symbol.H;
                        int deltax = targetx - symbolx;
                        int deltay = targety - symboly;

                        Pix temp = source.Pix[sym];
                        PixSetPadBits(ref temp, 0);
                        source.Pix[sym] = temp;
                        Pix targetcopy = PixCopy(null, source.Pix[sym]);
                        PixRasterop(targetcopy, deltax, deltay, symbol.W, symbol.H, JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst, symbol, 0, 0);
                        int deltacount = 0;
                        PixCountPixels(targetcopy, ref deltacount, null);

                        if (deltacount <= refine_level || deltax < -1 || deltax > 1)
                        {
                            // refinement disabled.
                            IntegerEncoder(ctx, JBIG2_IARI, 0);
                            curs += (symbols.Pix[assigned].W - (unborder_symbols ? 2 * kBorderSize : 0)) - 1;
                        }
                        //else
                        //{
                        //    wibble++;
                        //    IntegerEncoder(ctx, JBIG2_IARI, 1);

                        //    IntegerEncoder(ctx, JBIG2_IARDW, deltaw);
                        //    IntegerEncoder(ctx, JBIG2_IARDH, deltah);
                        //    IntegerEncoder(ctx, JBIG2_IARDX, deltax - (deltaw >> 1));
                        //    IntegerEncoder(ctx, JBIG2_IARDY, deltay - (deltah >> 1));

                        //    //jbig2enc_refine(ctx, symbol.Data, symbol.W, symbol.H, source.Pix[sym].Data, targetw, targeth, deltax, -deltay);
                        //    curs += targetw - 1;
                        //}
                    }
                    else
                    {
                        curs += (symbols.Pix[assigned].W - (unborder_symbols ? 2 * kBorderSize : 0)) - 1;
                    }
                }
                JBIG2EncOob(ctx, JBIG2_IADS);
                i = j;
            }

            JBIG2EncFinal(ctx);
        }

        void JBIG2EncFinal(JBIG2EncoderContext ctx)
        {
            // SETBITS
            int tempc = ctx.C + ctx.A;
            ctx.C |= 0xffff;
            if (ctx.C >= tempc)
                ctx.C -= 0x8000;

            ctx.C <<= ctx.CT;
            ByteOut(ctx);
            ctx.C <<= ctx.CT;
            ByteOut(ctx);
            Flush(ctx);
            if (ctx.B != 0xff)
            {
                ctx.B = 0xff;
                Flush(ctx);
            }

            ctx.B = 0xac;
            Flush(ctx);
        }

        void ByteOut(JBIG2EncoderContext ctx)
        {
            if (ctx.B == 0xff) goto rblock;

            if (ctx.C < 0x8000000) goto lblock;
            ctx.B += 1;
            if (ctx.B != 0xff) goto lblock;
            ctx.C &= 0x7ffffff;

        rblock:
            if (ctx.BP >= 0)
                Flush(ctx);
            ctx.B = (byte)(ctx.C >> 20);
            ctx.BP++;
            ctx.C &= 0xfffff;
            ctx.CT = 7;
            return;

        lblock:
            if (ctx.BP >= 0)
                Flush(ctx);
            ctx.B = (byte)(ctx.C >> 19);
            ctx.BP++;
            ctx.C &= 0x7ffff;
            ctx.CT = 8;
            return;
        }

        void Flush(JBIG2EncoderContext ctx)
        {
            int JBIG2_OUTPUTBUFFER_SIZE = 20 * 1024;
            if (ctx.OutbufUsed == JBIG2_OUTPUTBUFFER_SIZE)
            {
                ctx.OutputChunks.Add(ctx.OutputChunks.Count, ctx.Outbuf);
                ctx.OutbufUsed = 0;
                ctx.Outbuf = new byte[JBIG2_OUTPUTBUFFER_SIZE];
            }

            ctx.Outbuf[ctx.OutbufUsed++] = ctx.B;
        }

        //void jbig2enc_refine(JBIG2EncoderContext ctx, int[] itempl, int tx, int ty, uint[] itarget, int mx, int my, int ox, int oy)
        //{
        //    uint[] templdata = new uint[itempl.Length];
        //    uint[] data = itarget;
        //    List<int> context = new List<int>(ctx.Context);

        //    int image_counter = 0;

        //    image_counter++;

        //    int templwords_per_row = (tx + 31) / 32;
        //    int words_per_row = (mx + 31) / 32;

        //    for (int y = 0; y < my; ++y)
        //    {
        //        int x;
        //        int temply = y + oy;
        //        // the template is fixed to the 13 pixel template with the floating bits in
        //        // the default locations.
        //        // we have 5 words of context. The first three are the last, current and
        //        // next rows of the template. The last two are the last and current rows of
        //        // the target.
        //        // To form the 14 bits of content these are packed from the least
        //        // significant bits rightward.
        //        uint c1, c2, c3, c4, c5;
        //        // the w* values contain words from each of the corresponding rows. The
        //        // next bit to be part of the context is kept at the top of these words
        //        uint w1, w2, w3, w4, w5;
        //        w1 = w2 = w3 = w4 = w5 = 0;

        //        if (temply >= 1 && (temply - 1) < ty) w1 = templdata[(temply - 1) * templwords_per_row];
        //        if (temply >= 0 && temply < ty) w2 = templdata[temply * templwords_per_row];
        //        if (temply >= -1 && temply + 1 < ty) w3 = templdata[(temply + 1) * templwords_per_row];

        //        // the x offset prevents a hassel because we are dealing with bits. Thus we
        //        // restrict it to being {-1, 0, 1}.
        //        if (y >= 1) w4 = data[(y - 1) * words_per_row];
        //        w5 = data[y * words_per_row];

        //        int shiftoffset = 30 + ox;
        //        c1 = w1 >> shiftoffset;
        //        c2 = w2 >> shiftoffset;
        //        c3 = w3 >> shiftoffset;

        //        c4 = w4 >> 30;
        //        c5 = 0;

        //        // the w* should contain the next bit to be included in the context, in the
        //        // MSB position. Thus we need to roll the used bits out of the way.
        //        int bits_to_trim = 2 - ox;
        //        w1 <<= bits_to_trim;
        //        w2 <<= bits_to_trim;
        //        w3 <<= bits_to_trim;

        //        w4 <<= 2;

        //        for (x = 0; x < mx; ++x)
        //        {
        //            uint tval = (c1 << 10) | (c2 << 7) | (c3 << 4) | (c4 << 1) | c5;
        //            uint v = w5 >> 31;

        //            EncodeBit(ctx, ref context, tval, v);
        //            c1 <<= 1;
        //            c2 <<= 1;
        //            c3 <<= 1;
        //            c4 <<= 1;
        //            c1 |= w1 >> 31;
        //            c2 |= w2 >> 31;
        //            c3 |= w3 >> 31;
        //            c4 |= w4 >> 31;
        //            c5 = v;

        //            int m = x % 32;
        //            int wordno = (x / 32) + 1;
        //            if (m == 29 + ox)
        //            {
        //                // have run out of bits in the w[123] values. Need to get more.

        //                if (wordno >= templwords_per_row)
        //                    w1 = w2 = w3 = 0;
        //                else
        //                {
        //                    if (temply >= 1 && (temply - 1 < ty))
        //                        w1 = templdata[(temply - 1) * templwords_per_row + wordno];
        //                    else
        //                        w1 = 0;
        //                    if (temply >= 0 && temply < ty)
        //                        w2 = templdata[temply * templwords_per_row + wordno];
        //                    else
        //                        w2 = 0;
        //                    if (temply >= -1 && (temply + 1) < ty)
        //                        w3 = templdata[(temply + 1) * templwords_per_row + wordno];
        //                    else
        //                        w3 = 0;
        //                }
        //            }
        //            else
        //            {
        //                w1 <<= 1;
        //                w2 <<= 1;
        //                w3 <<= 1;
        //            }

        //            if (m == 29 && y >= 1)
        //            {
        //                // run out of data from w4
        //                if (wordno >= words_per_row)
        //                    w4 = 0;
        //                else
        //                    w4 = data[(y - 1) * words_per_row + wordno];
        //            }
        //            else
        //                w4 <<= 1;

        //            if (m == 31)
        //            {
        //                // run out of data from w5
        //                if (wordno >= words_per_row)
        //                    w5 = 0;
        //                else
        //                    w5 = data[y * words_per_row + wordno];
        //            }
        //            else
        //                w5 <<= 1;

        //            c1 &= 7;
        //            c2 &= 7;
        //            c3 &= 7;
        //            c4 &= 7;
        //        }
        //    }
        //}

        void JBIG2EncIaid(JBIG2EncoderContext ctx, int symcodelen, int value)
        {
            if (ctx.Iaidctx == null | ctx.Iaidctx.Count == 0)
                ctx.Iaidctx = new List<int>(1 << symcodelen);
            uint mask = (uint)(1 << (symcodelen + 1)) - 1;

            value <<= (32 - symcodelen);
            uint prev = 1;
            for (int i = 0; i < symcodelen; ++i)
            {
                uint tval = prev & mask;
                uint v = (uint)(value & 0x80000000) >> 31;
                List<int> temp = ctx.Iaidctx;
                EncodeBit(ctx, ref temp, tval, v);
                ctx.Iaidctx = temp;
                prev = (prev << 1) | v;
                value <<= 1;
            }
        }

        /// <summary>
        /// Initialize encoder context.
        /// </summary>
        void Initialize(ref JBIG2EncoderContext ctx)
        {
            ctx.A = 0x8000;
            ctx.C = 0;
            ctx.CT = 12;
            ctx.BP = -1;
            ctx.B = 0;
            ctx.OutbufUsed = 0;
            ctx.OutputChunks = new Dictionary<int, byte[]>();
        }

        /// <summary>
        /// Produces symbol.
        /// </summary>
        List<byte> JBIG2PagesComplete(JBIG2Context ctx)
        {
            int segment_symbol_table = 0;
            bool single_page = ctx.Classifier.NPages == 1;

            List<int> SymbolUsed = new List<int>(ctx.Classifier.Pixat.N);
            for (int i = 0; i < ctx.Classifier.NaClass.N; ++i)
            {
                int n = 0;
                NumaGetIValue(ctx.Classifier.NaClass, i, ref n);
                while (SymbolUsed.Count <= n)
                    SymbolUsed.Add(0);
                SymbolUsed[n]++;
            }

            // the multiuse symbols are the ones which go into the global dictionary
            List<int> multiUseSymbols = new List<int>();
            for (int i = 0; i < ctx.Classifier.Pixat.N; ++i)
            {
                if (SymbolUsed[i] == 0) break;
                if (SymbolUsed[i] > 1 || single_page)
                    multiUseSymbols.Add(i);
            }
            ctx.NumGlobalSymbols = multiUseSymbols.Count;

            for (int i = 0; i < ctx.Classifier.NaPage.N; ++i)
            {
                int page_num = 0;
                NumaGetIValue(ctx.Classifier.NaPage, i, ref page_num);
                if (ctx.PageComps.ContainsKey(page_num))
                    ctx.PageComps[page_num].Add(i);
                else
                {
                    List<int> intpComp = new List<int>();
                    intpComp.Add(i);
                    ctx.PageComps.Add(page_num, intpComp);
                }
                int symbol = 0;
                NumaGetIValue(ctx.Classifier.NaClass, i, ref symbol);
                if (SymbolUsed[symbol] == 1 && !single_page)
                {
                    List<int> temp = new List<int>();
                    temp.Add(symbol);
                    ctx.SingleUseSymbols.Add(page_num, temp);
                }
            }

            JBGetLLCorners(ctx.Classifier);

            JBIG2EncoderContext ectx = new JBIG2EncoderContext();

            JBIG2FileHeader header = new JBIG2FileHeader();
            byte[] JBIG2_FILE_MAGIC = new byte[] { 0x97, 0x4a, 0x42, 0x32, 0x0d, 0x0a, 0x1a, 0x0a };

            if (ctx.FullHeaders)
            {
                header.NPages = (uint)JBIG2Statics.Htonl(ctx.Classifier.NPages);
                header.OrganisationType = 1;
                header.Id = new byte[JBIG2_FILE_MAGIC.Length];
                int i = 0;

                foreach (byte c in JBIG2_FILE_MAGIC)
                {
                    header.Id[i] = (byte)Convert.ToInt32(c);
                    i++;
                }
            }

            Segment seg = new Segment();
            JBIG2SymbolDict symtab = new JBIG2SymbolDict();

            SortedDictionary<int, int> symMap = new SortedDictionary<int, int>();
            ctx.SymbolMap = symMap;

            JBIG2SymbolTable(ref ectx, (ctx.AvgTemplates != null ? ctx.AvgTemplates : ctx.Classifier.Pixat), ref multiUseSymbols, ref symMap, ctx.AvgTemplates == null);

            int symdatasize = JBIG2EncSize(ectx);

            symtab.a1x = 3;
            symtab.a1y = -1;
            symtab.a2x = -3;
            symtab.a2y = -1;
            symtab.a3x = 2;
            symtab.a3y = -2;
            symtab.a4x = -2;
            symtab.a4y = -2;
            symtab.ExSyms = symtab.NewSyms = JBIG2Statics.Htonl(multiUseSymbols.Count);

            ctx.SymbolTableSegment = ctx.SegNumber;
            seg.Number = (uint)ctx.SegNumber;
            ctx.SegNumber++;
            seg.SType = segment_symbol_table;
            seg.Length = (uint)(18 + symdatasize);//System.Runtime.InteropServices.Marshal.SizeOf(symtab)
            seg.Page = 0;
            seg.RetainBits = 1;

            List<byte> ret = new List<byte>();
            int offset = 0;
            if (ctx.FullHeaders)
                Fill(header, ref offset, ref ret);
            UpdateSegment(seg, ref offset, ref ret);
            Fill(symtab, ref offset, ref ret);
            JBIG2EncBuffer(ectx, ref ret);
            offset += symdatasize;

            return ret;
        }

        void JBIG2EncBuffer(JBIG2EncoderContext ctx, ref List<byte> buffer)
        {
            foreach (KeyValuePair<int, byte[]> key in ctx.OutputChunks)
                buffer.AddRange(key.Value);

            byte[] tempArray = new byte[ctx.OutbufUsed];
            Array.Copy(ctx.Outbuf, tempArray, ctx.OutbufUsed);

            buffer.AddRange(tempArray);
            tempArray = new byte[1];
        }

        void Fill(object x, ref int offset, ref List<byte> ret)
        {
            if (x is JBIG2FileHeader)
            {
                JBIG2FileHeader temp = (JBIG2FileHeader)x;
                foreach (char c in temp.Id)
                    ret.Add((byte)c);

                ret.Add(temp.OrganisationType);
                //ret.Add(temp.Reserved);
                //ret.Add(temp.UnknownNPages);
                ret.AddRange(BitConverter.GetBytes(temp.NPages));
            }
            if (x is JBIG2SymbolDict)
            {
                JBIG2SymbolDict temp = (JBIG2SymbolDict)x;

                ret.Add(temp.sdrtemplate);
                ret.Add(temp.sdtemplate);

                ret.Add((byte)temp.a1x);
                ret.Add((byte)temp.a1y);
                ret.Add((byte)temp.a2x);
                ret.Add((byte)temp.a2y);

                ret.Add((byte)temp.a3x);
                ret.Add((byte)temp.a3y);
                ret.Add((byte)temp.a4x);
                ret.Add((byte)temp.a4y);

                ret.AddRange(BitConverter.GetBytes(temp.ExSyms));
                ret.AddRange(BitConverter.GetBytes(temp.NewSyms));
            }
            if (x is JBIG2PageInfo)
            {
                JBIG2PageInfo temp = (JBIG2PageInfo)x;
                ret.AddRange(BitConverter.GetBytes(temp.Width));
                ret.AddRange(BitConverter.GetBytes(temp.Height));
                ret.AddRange(BitConverter.GetBytes(temp.XRes));
                ret.AddRange(BitConverter.GetBytes(temp.YRes));
                ret.Add(temp.IsLossless);
                ret.Add(temp.ContainsRefinements);
                ret.Add(temp.DefaultPixel);
            }
            if (x is JBIG2TextRegion)
            {
                JBIG2TextRegion temp = (JBIG2TextRegion)x;
                ret.AddRange(BitConverter.GetBytes(temp.Width));
                ret.AddRange(BitConverter.GetBytes(temp.Height));
                ret.AddRange(BitConverter.GetBytes(temp.X));
                ret.AddRange(BitConverter.GetBytes(temp.Y));
                ret.Add(temp.LogSBStrips);
                ret.Add(temp.SBRefine);
                ret.Add(temp.Sbrtemplate);
            }
            if (x is JBIG2TextRegionSym)
            {
                JBIG2TextRegionSym temp = (JBIG2TextRegionSym)x;
                ret.AddRange(BitConverter.GetBytes(temp.SBNumInstances));
            }
            offset = ret.Count;
        }

        int JBIG2EncSize(JBIG2EncoderContext ctx)
        {
            int JBIG2_OUTPUTBUFFER_SIZE = 20 * 1024;
            return JBIG2_OUTPUTBUFFER_SIZE * ctx.OutputChunks.Count + ctx.OutbufUsed;
        }

        void JBIG2SymbolTable(ref JBIG2EncoderContext ctx, Pixa symbols, ref List<int> symbol_list, ref SortedDictionary<int, int> symmap, bool unborder_symbols)
        {
            int n = symbol_list.Count;
            int number = 0;
            int kBorderSize = 6;
            int JBIG2_IADW = 4;
            int JBIG2_IAEX = 5;
            int JBIG2_IADH = 1;

            List<int> syms = symbol_list;

            // sort that vector by height
            HeightSorter(ref syms, symbols.Pix);
            List<int> hc = new List<int>();
            int hcheight = 0;
            for (int i = 0; i < n; )
            {
                int height = symbols.Pix[syms[i]].H - (unborder_symbols ? 2 * kBorderSize : 0);

                int j;
                hc.Clear();
                hc.Add(syms[i]);
                for (j = i + 1; j < n; ++j)
                {
                    if (symbols.Pix[syms[j]].H - (unborder_symbols ? 2 * kBorderSize : 0) != height) break;
                    hc.Add(syms[j]);
                }

                // sort them into increasing width
                WidthSorter(ref hc, symbols.Pix);
                int deltaheight = height - hcheight;
                IntegerEncoder(ctx, JBIG2_IADH, deltaheight);
                hcheight = height;
                int symwidth = 0;
                // encode each symbol
                foreach (int k in hc)
                {
                    int sym = k;
                    int thissymwidth = symbols.Pix[sym].W - (unborder_symbols ? 2 * kBorderSize : 0);
                    int deltawidth = thissymwidth - symwidth;

                    symwidth += deltawidth;
                    IntegerEncoder(ctx, JBIG2_IADW, deltawidth);

                    Pix unbordered = null;
                    if (unborder_symbols)
                        unbordered = PixRemoveBorder(symbols.Pix[sym], kBorderSize);
                    else
                        unbordered = symbols.Pix[sym];
                    PixSetPadBits(ref unbordered, 0);
                    JBIG2EncBitImage(ctx, unbordered.Data, thissymwidth, height, false);
                    symmap[sym] = number++;
                    unbordered = null;
                }
                JBIG2EncOob(ctx, JBIG2_IADW);
                i = j;
            }

            IntegerEncoder(ctx, JBIG2_IAEX, 0);
            IntegerEncoder(ctx, JBIG2_IAEX, n);

            JBIG2EncFinal(ctx);
        }

        void HeightSorter(ref List<int> list, Pta symbols)
        {
            int[] values = list.ToArray();
            float[] keys = new float[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                keys[i] = symbols.Y[list[i]];
            }

            SortedDictionary<float, List<int>> temp = new SortedDictionary<float, List<int>>();

            for (int i = 0; i < list.Count; i++)
            {
                if (temp.ContainsKey(keys[i]))
                    temp[keys[i]].Add(list[i]);
                else
                {
                    temp.Add(keys[i], new List<int>());
                    temp[keys[i]].Add(list[i]);
                }
            }

            list = new List<int>();

            foreach (KeyValuePair<float, List<int>> key in temp)
                list.AddRange(key.Value);
        }

        void HeightSorter(ref List<int> list, List<Pix> symbols)
        {
            int[] values = list.ToArray();
            int[] keys = new int[list.Count];

            for (int i = 0; i < list.Count; i++)
                keys[i] = symbols[list[i]].H;

            SortedDictionary<int, List<int>> temp = new SortedDictionary<int, List<int>>();

            for (int i = 0; i < list.Count; i++)
            {
                if (temp.ContainsKey(keys[i]))
                    temp[keys[i]].Add(list[i]);
                else
                {
                    temp.Add(keys[i], new List<int>());
                    temp[keys[i]].Add(list[i]);
                }
            }

            list = new List<int>();

            foreach (KeyValuePair<int, List<int>> key in temp)
                list.AddRange(key.Value);
        }

        void WidthSorter(ref List<int> list, Pta symbols)
        {
            if (list.Count <= 1)
                return;

            int[] values = list.ToArray();
            float[] keys = new float[list.Count];

            for (int i = 0; i < list.Count; i++)
                keys[i] = symbols.X[list[i]];

            SortedDictionary<float, List<int>> temp = new SortedDictionary<float, List<int>>();

            for (int i = 0; i < list.Count; i++)
            {
                if (temp.ContainsKey(keys[i]))
                    temp[keys[i]].Add(list[i]);
                else
                {
                    temp.Add(keys[i], new List<int>());
                    temp[keys[i]].Add(list[i]);
                }
            }

            list = new List<int>();

            foreach (KeyValuePair<float, List<int>> key in temp)
                list.AddRange(key.Value);
        }

        void WidthSorter(ref List<int> list, List<Pix> symbols)
        {
            if (list.Count <= 1)
                return;

            int[] values = list.ToArray();
            int[] keys = new int[list.Count];

            for (int i = 0; i < list.Count; i++)
                keys[i] = symbols[list[i]].W;

            SortedDictionary<int, List<int>> temp = new SortedDictionary<int, List<int>>();

            for (int i = 0; i < list.Count; i++)
            {
                if (temp.ContainsKey(keys[i]))
                    temp[keys[i]].Add(list[i]);
                else
                {
                    temp.Add(keys[i], new List<int>());
                    temp[keys[i]].Add(list[i]);
                }
            }

            list = new List<int>();

            foreach (KeyValuePair<int, List<int>> key in temp)
                list.AddRange(key.Value);
        }

        void JBIG2EncOob(JBIG2EncoderContext ctx, int proc)
        {
            List<int> context = ctx.IndexContext[proc];

            EncodeBit(ctx, ref context, 1, 1);
            EncodeBit(ctx, ref context, 3, 0);
            EncodeBit(ctx, ref context, 6, 0);
            EncodeBit(ctx, ref context, 12, 0);

            ctx.IndexContext[proc] = context;
        }

        void JBIG2EncBitImage(JBIG2EncoderContext ctx, uint[] idata, int mx, int my, bool duplicate_line_removal)
        {
            uint[] data = idata;
            List<int> context = ctx.Context;
            int words_per_row = (mx + 31) / 32;
            int bytes_per_row = words_per_row * 4;

            uint ltp = 0, sltp = 0;

            for (int y = 0; y < my; ++y)
            {
                int x = 0;
                uint c1, c2, c3, w1, w2, w3;
                w1 = w2 = w3 = 0;

                if (y >= 2) w1 = data[(y - 2) * words_per_row];
                if (y >= 1)
                {
                    w2 = data[(y - 1) * words_per_row];

                    if (duplicate_line_removal)
                    {
                        // it's possible that the last row was the same as this row
                        if (bytes_per_row == 0) //memcmp(data[y * words_per_row], data[(y - 1) * words_per_row], bytes_per_row) == 0)
                        {
                            sltp = ltp ^ 1;
                            ltp = 1;
                        }
                        else
                        {
                            Array.Copy(data, (y - 1) * words_per_row, data, y * words_per_row, bytes_per_row);
                            sltp = ltp;
                            ltp = 0;
                        }
                    }
                }
                if (duplicate_line_removal)
                {
                    uint TPGDCTX = 0x9b25;
                    EncodeBit(ctx, ref context, TPGDCTX, sltp);
                    if (ltp == 0) continue;
                }
                w3 = data[y * words_per_row];

                c1 = w1 >> 29;
                c2 = w2 >> 28;
                w1 <<= 3;
                w2 <<= 4;
                c3 = 0;
                for (x = 0; x < mx; ++x)
                {
                    uint tval = (c1 << 11) | (c2 << 4) | c3;
                    uint v = (w3 & 0x80000000) >> 31;
                    EncodeBit(ctx, ref context, tval, v);
                    c1 <<= 1;
                    c2 <<= 1;
                    c3 <<= 1;
                    c1 |= (w1 & 0x80000000) >> 31;
                    c2 |= (w2 & 0x80000000) >> 31;
                    c3 |= v;
                    int m = x % 32;
                    if (m == 28 && y >= 2)
                    {
                        int wordno = (x / 32) + 1;
                        if (wordno >= words_per_row)
                            w1 = 0;
                        else
                            w1 = data[(y - 2) * words_per_row + wordno];
                    }
                    else
                        w1 <<= 1;

                    if (m == 27 && y >= 1)
                    {
                        int wordno = (x / 32) + 1;
                        if (wordno >= words_per_row)
                            w2 = 0;
                        else
                            w2 = data[(y - 1) * words_per_row + wordno];
                    }
                    else
                        w2 <<= 1;

                    if (m == 31)
                    {
                        int wordno = (x / 32) + 1;
                        if (wordno >= words_per_row)
                            w3 = 0;
                        else
                            w3 = data[y * words_per_row + wordno];
                    }
                    else
                        w3 <<= 1;

                    c1 &= 31;
                    c2 &= 127;
                    c3 &= 15;
                }
            }
            ctx.Context = context;
        }

        void IntegerEncoder(JBIG2EncoderContext ctx, int proc, int value)
        {
            List<int> context = ctx.IndexContext[proc];
            int i;

            if (value > 2000000000 || value < -2000000000)
                return;

            uint prev = 1;

            for (i = 0; ; ++i)
                if (m_intEncRange[i].Bot <= value && m_intEncRange[i].Top >= value) break;

            if (value < 0) value = -value;
            value -= m_intEncRange[i].Delta;

            char data = (char)m_intEncRange[i].Data;
            for (int j = 0; j < m_intEncRange[i].Bits; ++j)
            {
                uint v = (uint)(data & 1);
                EncodeBit(ctx, ref context, prev, v);
                data >>= 1;
                if ((prev & 0x100) > 0)
                    prev = (((prev << 1) | v) & 0x1ff) | 0x100;
                else
                    prev = (prev << 1) | v;
            }

            value <<= (32 - m_intEncRange[i].IntBits);
            for (int j = 0; j < m_intEncRange[i].IntBits; ++j)
            {
                uint v = (uint)(value & 0x80000000) >> 31;
                EncodeBit(ctx, ref context, prev, v);
                value <<= 1;
                if ((prev & 0x100) > 0)
                    prev = (((prev << 1) | v) & 0x1ff) | 0x100;
                else
                    prev = (prev << 1) | v;
            }

            ctx.IndexContext[proc] = context;
        }

        void EncodeBit(JBIG2EncoderContext ctx, ref List<int> context, uint ctxnum, uint d)
        {
            bool isCtx = false;
            if (context == null)
            {
                isCtx = true;
                context = new List<int>(ctx.Context);
            }
            int i = context.Count > ctxnum ? context[(int)ctxnum] : 0;
            int mps = i > 46 ? 1 : 0;
            int qe = ContextCollection.StateTable[i].Qe;

            if (d != mps) goto codelps;
            ctx.A -= qe;
            if ((ctx.A & 0x8000) == 0)
            {
                if (ctx.A < qe)
                    ctx.A = qe;
                else
                    ctx.C += qe;
                if (context.Count <= ctxnum)
                {
                    while (context.Count < ctxnum)
                        context.Add(0);
                    context.Add(ContextCollection.StateTable[i].Mps);
                }
                else
                    context[(int)ctxnum] = ContextCollection.StateTable[i].Mps;
                goto renorme;
            }
            else
                ctx.C += qe;

            return;

        codelps:
            ctx.A -= qe;
            if (ctx.A < qe)
                ctx.C += qe;
            else
                ctx.A = qe;
            if (context.Count <= ctxnum)
            {
                while (context.Count < ctxnum)
                    context.Add(0);
                context.Add(ContextCollection.StateTable[i].Lps);
            }
            else
                context[(int)ctxnum] = ContextCollection.StateTable[i].Lps;

        renorme:
            do
            {
                ctx.A <<= 1;
                ctx.C <<= 1;
                ctx.CT -= 1;
                if (ctx.CT == 0)
                    ByteOut(ctx);
            } while ((ctx.A & 0x8000) == 0);

            if (isCtx)
                ctx.Context = context;
        }

        void JBGetLLCorners(JBIG2Classifier classer)
        {
            int i, iclass = 0, n, x1 = 0, y1 = 0, h;
            Numa naclass;
            Pix pix;
            Pixa pixat;
            Pta ptaul, ptall;

            ptaul = classer.PtaUL;
            naclass = classer.NaClass;
            pixat = classer.Pixat;

            n = ptaul.N;
            ptall = JBIG2Statics.CreatePta(n);
            classer.PtaLL = ptall;

            for (i = 0; i < n; i++)
            {
                PtaGetIPt(ptaul, i, ref x1, ref y1);
                NumaGetIValue(naclass, i, ref iclass);
                pix = PixaGetPix(pixat, iclass, m_clone);
                h = pix.H;
                PtaAddPt(ptall, x1, y1 + h - 1 - 2 * m_jbAddedPixels);
            }
        }

        /// <summary>
        /// Classify and record information about a page.
        /// </summary>
        private void AddPage(JBIG2Context ctx, Pix pix)
        {
            JBAddPage(ctx.Classifier, pix);
            ctx.PageWidth.Add(pix.W);
            ctx.PageHeight.Add(pix.H);
            ctx.PageXRes.Add(pix.XRes);
            ctx.PageYRes.Add(pix.YRes);
        }

        Pix RemoveSpot(Pix source, int size)
        {
            Sel sel_5h = SelCreateBrick(1, size, 0, 2, 1);
            Sel sel_5v = SelCreateBrick(size, 1, 2, 0, 1);

            Pix pixt = PixOpen(null, source, sel_5h);
            Pix pixd = PixOpen(null, source, sel_5v);
            PixOr(pixd, pixd, pixt);

            return pixd;
        }

        Pix PixOr(Pix pixd, Pix pixs1, Pix pixs2)
        {
            if (pixd == pixs2)
            {
                return null;// "cannot have pixs2 == pixd"
            }
            if (pixs1.D != pixs2.D)
            { // depths of pixs* unequal"
                return null;
            }

            if ((pixd = PixCopy(pixd, pixs1)) == null)
            { return null; }// "pixd not made"

            PixRasterop(pixd, 0, 0, pixd.W, pixd.H, JBIG2Statics.PixSrc | JBIG2Statics.PixDst, pixs2, 0, 0);

            return pixd;
        }

        /// <summary>
        /// Adds page.
        /// </summary>
        private void JBAddPage(JBIG2Classifier classer, Pix pixs)
        {
            Boxa boxas = null;
            Pixa pixas = null;

            if (pixs == null || pixs.D != 1)
            {
#if DEBUG
                Console.WriteLine("pixs not defined or not 1 bpp");
#endif
                return;
            }

            classer.W = pixs.W;
            classer.H = pixs.H;

            if (!JBGetComponents(pixs, classer.Components, classer.MaxWidth, classer.MaxHeight, ref boxas, ref pixas))
                return;

            JBAddPageComponents(classer, pixs, boxas, pixas);
            boxas = null;
            pixas = null;
        }

        void JBAddPageComponents(JBIG2Classifier classer, Pix pixs, Boxa boxas, Pixa pixas)
        {
            int n;

            if (boxas == null || pixas == null || boxas.N == 0)
            {
                classer.NPages++;
                return;
            }

            if (classer.Method == 0)
                JBClassifyRankHaus(classer, boxas, pixas);
            else
                JBClassifyCorrelation(classer, boxas, pixas);

            if (JBGetULCorners(classer, pixs, boxas))
            { }// return ERROR_INT("UL corners not found", procName, 1);

            /* Update total component counts and number of pages processed. */
            n = boxas.N;
            classer.BaseIndex += n;
            NumaAddNumber(classer.NaComps, n);
            classer.NPages++;
        }

        bool JBGetULCorners(JBIG2Classifier classer, Pix pixs, Boxa boxa)
        {
            int i, baseindex, index, n, iclass = 0, idelx, idely, x, y, dx = 0, dy = 0;
            int[] sumtab;
            float x1 = 0, x2 = 0, y1 = 0, y2 = 0, delx, dely;
            Box box;
            Numa naclass;
            Pix pixt;
            Pta ptac, ptact, ptaul;

            n = boxa.N;
            ptaul = classer.PtaUL;
            naclass = classer.NaClass;
            ptac = classer.Ptac;
            ptact = classer.PtaTemplate;
            baseindex = classer.BaseIndex;  /* num components before this page */
            sumtab = MakePixelSumTab8();
            for (i = 0; i < n; i++)
            {
                index = baseindex + i;
                PtaGetPt(ptac, index, ref x1, ref y1);
                NumaGetIValue(naclass, index, ref iclass);
                PtaGetPt(ptact, iclass, ref x2, ref y2);
                delx = x2 - x1;
                dely = y2 - y1;
                if (delx >= 0)
                    idelx = (int)(delx + 0.5);
                else
                    idelx = (int)(delx - 0.5);
                if (dely >= 0)
                    idely = (int)(dely + 0.5);
                else
                    idely = (int)(dely - 0.5);
                if ((box = BoxaGetBox(boxa, i, m_clone)) == null)
                {
                    Console.WriteLine("box not found"); return false;
                }
                x = box.X; y = box.Y;

                /* Get final increments dx and dy for best alignment */
                pixt = PixaGetPix(classer.Pixat, iclass, m_clone);
                FinalPositioningForAlignment(pixs, x, y, idelx, idely, pixt, sumtab, ref dx, ref dy);
                PtaAddPt(ptaul, x - idelx + dx, y - idely + dy);
            }

            return false;
        }

        int FinalPositioningForAlignment(Pix pixs, int x, int y, int idelx, int idely, Pix pixt, int[] sumtab, ref int pdx, ref int pdy)
        {
            int w, h, i, j, minx = 0, miny = 0, count = 0, mincount;
            Pix pixi;  /* clipped from source pixs */
            Pix pixr;  /* temporary storage */
            Box box;

            w = pixt.W;
            h = pixt.H;
            box = BoxCreate(x - idelx - m_jbAddedPixels, y - idely - m_jbAddedPixels, w, h);
            pixi = PixClipRectangle(pixs, box, null);
            if (pixi == null)
            { }// return ERROR_INT("pixi not made", procName, 1);

            pixr = PixCreate(pixi.W, pixi.H, 1);
            mincount = 0x7fffffff;
            for (i = -1; i <= 1; i++)
            {
                for (j = -1; j <= 1; j++)
                {
                    count = 0;
                    pixr = PixCopy(pixr, pixi);
                    PixRasterop(pixr, j, i, w, h, JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst, pixt, 0, 0);
                    PixCountPixels(pixr, ref count, sumtab);
                    if (count < mincount)
                    {
                        minx = j;
                        miny = i;
                        mincount = count;
                    }
                }
            }

            pdx = minx;
            pdy = miny;
            return 0;
        }

        bool JBClassifyCorrelation(JBIG2Classifier classer, Boxa boxa, Pixa pixas)
        {
            int n, nt, i, iclass, wt, ht, area = 0, area1, area2 = 0, npages;
            bool overthreshold, found;
            int[] sumtab, centtab;
            uint[] row; uint word;
            float x1 = 0.0f, y1 = 0.0f, x2 = 0.0f, y2 = 0.0f, xsum, ysum;
            float thresh, weight, m_threshold;
            Box box;
            Numa naclass, napage;
            Numa nafgt;   /* fg area of all templates */
            Numa naarea;   /* w * h area of all templates */
            JbTemplatesState findcontext;
            NumaHash nahash;
            Pix pix, pix1, pix2;
            Pixa pixa, pixa1, pixat;
            Pixaa pixaa;
            Pta pta, ptac, ptact;
            int[] pixcts;  /* pixel counts of each pixa */
            List<int[]> pixrowcts;  /* row-by-row pixel counts of each pixa */
            int x, y, rowcount, downcount, wpl;
            char bytes;

            npages = classer.NPages;

            n = pixas.N;
            pixa1 = JBIG2Statics.CreatePixa(n);
            for (i = 0; i < n; i++)
            {
                pix = PixaGetPix(pixas, i, m_clone);
                pix1 = PixAddBorderGeneral(pix, m_jbAddedPixels, m_jbAddedPixels, m_jbAddedPixels, m_jbAddedPixels, 0);
                PixaAddPix(pixa1, pix1, m_insert);
            }

            naclass = classer.NaClass;
            napage = classer.NaPage;

            nafgt = classer.Nafgt;    /* holds fg areas of the templates */
            sumtab = MakePixelSumTab8();

            pixcts = new int[n];
            pixrowcts = new List<int[]>(n);
            centtab = MakePixelCentroidTab8();
            if (pixcts == null || pixrowcts == null || centtab == null)
            {
                Console.WriteLine("calloc fail in pix*cts or centtab"); return false;
            }

            pta = JBIG2Statics.CreatePta(n);
            for (i = 0; i < n; i++)
            {
                pix = PixaGetPix(pixa1, i, m_clone);
                pixrowcts.Add(new int[pix.H]);
                xsum = 0;
                ysum = 0;
                wpl = pix.Wpl;
                int count = (pix.H - 1) * wpl;
                row = new uint[count];
                Array.Copy(pix.Data, 0, row, 0, row.Length);
                int len = row.Length - wpl;
                downcount = 0;
                for (y = pix.H - 2; y >= 0; y--, len -= wpl)
                {
                    pixrowcts[i][y] = downcount;
                    rowcount = 0;
                    for (x = len; x < len + wpl; x++)
                    {
                        word = row[x];
                        bytes = (char)(word & 0xff);
                        rowcount += sumtab[bytes];
                        xsum += centtab[bytes] + ((x - len) * 32 + 24) * sumtab[bytes];
                        bytes = (char)((word >> 8) & 0xff);
                        rowcount += sumtab[bytes];
                        xsum += centtab[bytes] + ((x - len) * 32 + 16) * sumtab[bytes];
                        bytes = (char)((word >> 16) & 0xff);
                        rowcount += sumtab[bytes];
                        xsum += centtab[bytes] + ((x - len) * 32 + 8) * sumtab[bytes];
                        bytes = (char)((word >> 24) & 0xff);
                        rowcount += sumtab[bytes];
                        xsum += centtab[bytes] + (x - len) * 32 * sumtab[bytes];
                    }
                    downcount += rowcount;
                    ysum += rowcount * y;
                }
                pixcts[i] = downcount;
                PtaAddPt(pta, xsum / (float)downcount, ysum / (float)downcount);
            }

            ptac = classer.Ptac;  /* holds centroids of components up to this page */
            PtaJoin(ptac, pta, 0, 0);  /* save centroids of all components */
            ptact = classer.PtaTemplate;  /* holds centroids of templates */

            pixaa = classer.Pixaa;
            pixat = classer.Pixat;
            thresh = classer.Thresh;
            weight = classer.WeightFactor;
            naarea = classer.NaArea;
            nahash = classer.NaHash;
            for (i = 0; i < n; i++)
            {
                pix1 = PixaGetPix(pixa1, i, m_clone);
                area1 = pixcts[i];
                PtaGetPt(pta, i, ref x1, ref y1);  /* centroid for this instance */
                nt = pixat.N;
                found = false;
                findcontext = FindSimilarSizedTemplatesInit(classer, pix1);
                while ((iclass = FindSimilarSizedTemplatesNext(findcontext)) > -1)
                {
                    /* Get the template */
                    pix2 = PixaGetPix(pixat, iclass, m_clone);
                    NumaGetIValue(nafgt, iclass, ref area2);
                    PtaGetPt(ptact, iclass, ref x2, ref y2);  /* template centroid */

                    if (weight > 0.0)
                    {
                        NumaGetIValue(naarea, iclass, ref area);
                        m_threshold = thresh + (1 - thresh) * weight * area2 / area;
                    }
                    else
                        m_threshold = thresh;

                    overthreshold = PixCorrelationScoreThresholded(pix1, pix2, area1, area2, x1 - x2, y1 - y2, m_maxDiffWidth, m_maxDiffHeight, sumtab, pixrowcts[i], m_threshold);

                    if (overthreshold)
                    {  
                        found = true;
                        NumaAddNumber(naclass, iclass);
                        NumaAddNumber(napage, npages);
                        if (classer.KeepPixaa > 0)
                        {
                            pixa = PixaaGetPixa(pixaa, iclass, m_clone);
                            pix = PixaGetPix(pixas, i, m_clone);
                            PixaAddPix(pixa, pix, m_insert);
                            box = BoxaGetBox(boxa, i, m_clone);
                            PixaAddBox(pixa, box, m_insert);
                        }
                        break;
                    }
                }
                FindSimilarSizedTemplatesDestroy(ref findcontext);
                if (found == false)
                { 
                    NumaAddNumber(naclass, nt);
                    NumaAddNumber(napage, npages);
                    pixa = JBIG2Statics.CreatePixa(0);
                    pix = PixaGetPix(pixas, i, m_clone);  
                    PixaAddPix(pixa, pix, m_insert);
                    wt = pix.W;
                    ht = pix.H;
                    NumaHashAdd(nahash, ht * wt, nt);
                    box = BoxaGetBox(boxa, i, m_clone);
                    PixaAddBox(pixa, box, m_insert);
                    PixaaAddPixa(pixaa, pixa, m_insert);  
                    PtaAddPt(ptact, x1, y1);
                    NumaAddNumber(nafgt, area1);
                    PixaAddPix(pixat, pix1, m_insert);   
                    area = (pix1.W - 2 * m_jbAddedPixels) * (pix1.H - 2 * m_jbAddedPixels);
                    NumaAddNumber(naarea, area);
                }
                else
                {   /* don't save it */
                }
            }
            classer.NClass = pixat.N;

            return false;
        }

        void FindSimilarSizedTemplatesDestroy(ref JbTemplatesState pstate)
        {
            pstate.Numa = null;
        }

        bool PixCorrelationScoreThresholded(Pix pix1, Pix pix2, int area1, int area2, float delx, float dely, int maxdiffw, int maxdiffh, int[] tab, int[] downcount, float score_threshold)
        {
            int wi, hi, wt, ht, delw, delh, idelx, idely, count;
            int wpl1, wpl2, lorow, hirow, locol, hicol, untouchable = 0;
            int x, y, pix1lskip, pix2lskip, rowwords1, rowwords2;
            uint word1, word2, andw;
            uint[] row1, row2;
            float score;
            int m_threshold;

            if (pix1 == null || pix1.D != 1)
            { }// return ERROR_INT("pix1 not 1 bpp", procName, 0);
            if (pix2 == null || pix2.D != 1)
            { }// return ERROR_INT("pix2 not 1 bpp", procName, 0);
            if (tab == null)
            { }// return ERROR_INT("tab not defined", procName, 0);
            if (area1 <= 0 || area2 <= 0)
            { }// return ERROR_INT("areas must be > 0", procName, 0);

            /* Eliminate based on size difference */
            wi = pix1.W; hi = pix1.H;
            wt = pix2.W; ht = pix2.H;
            delw = Math.Abs(wi - wt);
            if (delw > maxdiffw)
                return false;
            delh = Math.Abs(hi - ht);
            if (delh > maxdiffh)
                return false;

            /* Round difference to nearest integer */
            if (delx >= 0)
                idelx = (int)(delx + 0.5);
            else
                idelx = (int)(delx - 0.5);
            if (dely >= 0)
                idely = (int)(dely + 0.5);
            else
                idely = (int)(dely - 0.5);

            /* Compute the correlation count that is needed so that count * count / (area1 * area2) >= score_threshold */
            decimal gross = (decimal)score_threshold * area1 * area2; //Needed split up otherwise rounding is not correct.
            decimal sqr = (decimal)Math.Sqrt((double)gross);
            string temp = Math.Ceiling(sqr).ToString();
            m_threshold = int.Parse(temp);

            count = 0;
            wpl1 = pix1.Wpl;
            wpl2 = pix2.Wpl;
            rowwords2 = wpl2;

            lorow = Math.Max(idely, 0);
            hirow = Math.Min(ht + idely, hi);

            int c = wpl1 * lorow;
            row1 = new uint[pix1.Data.Length - c];
            Array.Copy(pix1.Data, c, row1, 0, row1.Length);
            c = wpl2 * (lorow - idely);
            row2 = new uint[pix2.Data.Length - c];
            Array.Copy(pix2.Data, c, row2, 0, row2.Length);
            if (hirow <= hi)
                untouchable = downcount[hirow - 1];

            locol = Math.Max(idelx, 0);
            hicol = Math.Min(wt + idelx, wi);

            int row1Index = 0, row2Index = 0;

            if (idelx >= 32)
            {
                pix1lskip = idelx >> 5; 
                row1Index += pix1lskip;
                locol -= pix1lskip << 5;
                hicol -= pix1lskip << 5;
                idelx &= 31;
            }
            else if (idelx <= -32)
            {
                pix2lskip = -((idelx + 31) >> 5);
                row2Index += pix2lskip;
                rowwords2 -= pix2lskip;
                idelx += pix2lskip << 5;
            }

            if ((locol >= hicol) || (lorow >= hirow))
                count = 0;
            else
            {
                rowwords1 = (hicol + 31) >> 5;

                if (idelx == 0)
                {
                    for (y = lorow; y < hirow; y++, row1Index += wpl1, row2Index += wpl2)
                    {
                        for (x = 0; x < rowwords1; x++)
                        {
                            andw = row1[row1Index + x] & row2[row2Index + x];
                            count += tab[andw & 0xff] + tab[(andw >> 8) & 0xff] + tab[(andw >> 16) & 0xff] + tab[andw >> 24];
                        }

                        if (count >= m_threshold) return true;
                        if (count + downcount[y] - untouchable < m_threshold)
                            return false;
                    }
                }
                else if (idelx > 0)
                {
                    if (rowwords2 < rowwords1)
                    {
                        for (y = lorow; y < hirow; y++, row1Index += wpl1, row2Index += wpl2)
                        {
                            word1 = row1[row1Index + 0];
                            word2 = row2[row2Index + 0] >> idelx;
                            andw = word1 & word2;
                            count += tab[andw & 0xff] + tab[(andw >> 8) & 0xff] + tab[(andw >> 16) & 0xff] + tab[andw >> 24];

                            for (x = 1; x < rowwords2; x++)
                            {
                                word1 = row1[row1Index + x];
                                word2 = (row2[row2Index + x] >> idelx) | (row2[row2Index + x - 1] << (32 - idelx));
                                andw = word1 & word2;
                                count += tab[andw & 0xff] + tab[(andw >> 8) & 0xff] + tab[(andw >> 16) & 0xff] + tab[andw >> 24];
                            }

                            word1 = row1[row1Index + x];
                            word2 = row2[row2Index + x - 1] << (32 - idelx);
                            andw = word1 & word2;
                            count += tab[andw & 0xff] + tab[(andw >> 8) & 0xff] + tab[(andw >> 16) & 0xff] + tab[andw >> 24];

                            if (count >= m_threshold) return true;
                            if (count + downcount[y] - untouchable < m_threshold)
                                return false;
                        }
                    }
                    else
                    {
                        for (y = lorow; y < hirow; y++, row1Index += wpl1, row2Index += wpl2)
                        {
                            word1 = row1[row1Index + 0];
                            word2 = row2[row2Index + 0] >> idelx;
                            andw = word1 & word2;
                            count += tab[andw & 0xff] + tab[(andw >> 8) & 0xff] + tab[(andw >> 16) & 0xff] + tab[andw >> 24];

                            for (x = 1; x < rowwords1; x++)
                            {
                                word1 = row1[row1Index + x];
                                word2 = (row2[row2Index + x] >> idelx) | (row2[row2Index + x - 1] << (32 - idelx));
                                andw = word1 & word2;
                                count += tab[andw & 0xff] + tab[(andw >> 8) & 0xff] + tab[(andw >> 16) & 0xff] + tab[andw >> 24];
                            }

                            if (count >= m_threshold) return true;
                            if (count + downcount[y] - untouchable < m_threshold)
                                return false;
                        }
                    }
                }
                else
                {
                    if (rowwords1 < rowwords2)
                    {
                        for (y = lorow; y < hirow; y++, row1Index += wpl1, row2Index += wpl2)
                        {
                            for (x = 0; x < rowwords1; x++)
                            {
                                word1 = row1[row1Index + x];
                                word2 = row2[row2Index + x] << -idelx;
                                word2 |= row2[row2Index + x + 1] >> (32 + idelx);
                                andw = word1 & word2;
                                count += tab[andw & 0xff] + tab[(andw >> 8) & 0xff] + tab[(andw >> 16) & 0xff] + tab[andw >> 24];
                            }

                            if (count >= m_threshold) return true;
                            if (count + downcount[y] - untouchable < m_threshold)
                                return false;
                        }
                    }
                    else
                    {
                        for (y = lorow; y < hirow; y++, row1Index += wpl1, row2Index += wpl2)
                        {
                            for (x = 0; x < rowwords1 - 1; x++)
                            {
                                word1 = row1[row1Index + x];
                                word2 = row2[row2Index + x] << -idelx;
                                word2 |= row2[row2Index + x + 1] >> (32 + idelx);
                                andw = word1 & word2;
                                count += tab[andw & 0xff] + tab[(andw >> 8) & 0xff] + tab[(andw >> 16) & 0xff] + tab[andw >> 24];
                            }

                            word1 = row1[row1Index + x];
                            word2 = row2[row2Index + x] << -idelx;
                            andw = word1 & word2;
                            count += tab[andw & 0xff] + tab[(andw >> 8) & 0xff] + tab[(andw >> 16) & 0xff] + tab[andw >> 24];

                            if (count >= m_threshold) return true;
                            if (count + downcount[y] - untouchable < m_threshold)
                                return false;
                        }
                    }
                }
            }

            score = (float)(count * count) / (float)(area1 * area2);
            if (score >= score_threshold)
            { }  //fprintf(stderr, "count %d < m_threshold %d but score %g >= score_threshold %g\n",                count, m_threshold, score, score_threshold);

            return false;
        }

        bool JBClassifyRankHaus(JBIG2Classifier classer, Boxa boxa, Pixa pixas)
        {
            int n, nt, i, wt, ht, iclass, size, testval = 0;
            bool found;
            int[] sumtab;
            int npages, area1 = 0, area3 = 0;
            int[] tab8;
            float rank, x1 = 0f, y1 = 0f, x2 = 0f, y2 = 0f;
            Box box;
            Numa naclass, napage;
            Numa nafg;   /* fg area of all instances */
            Numa nafgt;  /* fg area of all templates */
            JbTemplatesState findcontext;
            NumaHash nahash;
            Pix pix, pix1, pix2, pix3, pix4;
            Pixa pixa, pixa1, pixa2, pixat, pixatd;
            Pixaa pixaa;
            Pta pta, ptac, ptact;
            Sel sel;

            npages = classer.NPages;
            size = classer.SizeHaus;
            sel = SelCreateBrick(size, size, size / 2, size / 2, 1);

            n = pixas.N;
            pixa1 = JBIG2Statics.CreatePixa(n);
            pixa2 = JBIG2Statics.CreatePixa(n);
            for (i = 0; i < n; i++)
            {
                pix = PixaGetPix(pixas, i, m_clone);
                pix1 = PixAddBorderGeneral(pix, m_jbAddedPixels, m_jbAddedPixels, m_jbAddedPixels, m_jbAddedPixels, 0);
                pix2 = PixDilate(null, pix1, sel);
                PixaAddPix(pixa1, pix1, m_insert);   /* un-dilated */
                PixaAddPix(pixa2, pix2, m_insert);   /* dilated */
            }

            pta = PixaCentroids(pixa1);
            ptac = classer.Ptac;
            PtaJoin(ptac, pta, 0, 0);
            ptact = classer.PtaTemplate; 

            naclass = classer.NaClass;
            napage = classer.NaPage;
            sumtab = MakePixelSumTab8();

            pixaa = classer.Pixaa;

            pixat = classer.Pixat;
            pixatd = classer.Pixatd;

            rank = classer.RankHaus;
            nahash = classer.NaHash;
            if (rank == 1.0)
            {
                for (i = 0; i < n; i++)
                {
                    pix1 = PixaGetPix(pixa1, i, m_clone);
                    pix2 = PixaGetPix(pixa2, i, m_clone);
                    PtaGetPt(pta, i, ref x1, ref y1);
                    nt = pixat.N; 
                    found = false;
                    findcontext = FindSimilarSizedTemplatesInit(classer, pix1);
                    while ((iclass = FindSimilarSizedTemplatesNext(findcontext)) > -1)
                    {
                        pix3 = PixaGetPix(pixat, iclass, m_clone);
                        pix4 = PixaGetPix(pixatd, iclass, m_clone);
                        PtaGetPt(ptact, iclass, ref x2, ref y2);
                        testval = PixHausTest(pix1, pix2, pix3, pix4, x1 - x2, y1 - y2, m_maxDiffWidth, m_maxDiffHeight);

                        if (testval == 1)
                        {
                            found = true;
                            NumaAddNumber(naclass, iclass);
                            NumaAddNumber(napage, npages);
                            if (classer.KeepPixaa == 1)
                            {
                                pixa = PixaaGetPixa(pixaa, iclass, m_clone);
                                pix = PixaGetPix(pixas, i, m_clone);
                                PixaAddPix(pixa, pix, m_insert);
                                box = BoxaGetBox(boxa, i, m_clone);
                                PixaAddBox(pixa, box, m_insert);
                            }
                            break;
                        }
                    }

                    if (!found)
                    {  /* new class */
                        NumaAddNumber(naclass, nt);
                        NumaAddNumber(napage, npages);
                        pixa = JBIG2Statics.CreatePixa(0);
                        pix = PixaGetPix(pixas, i, m_clone);
                        PixaAddPix(pixa, pix, m_insert);
                        wt = pix.W;
                        ht = pix.H;
                        NumaHashAdd(nahash, ht * wt, nt);
                        box = BoxaGetBox(boxa, i, m_clone);
                        PixaAddBox(pixa, box, m_insert);
                        PixaaAddPixa(pixaa, pixa, m_insert);
                        PtaAddPt(ptact, x1, y1);
                        PixaAddPix(pixat, pix1, m_insert); 
                        PixaAddPix(pixatd, pix2, m_insert);
                    }
                    else
                    {   /* don't save them */
                    }
                }
            }
            else
            {  
                if ((nafg = PixaCountPixels(pixas)) == null)
                { Console.WriteLine("nafg not made"); return false; }
                nafgt = classer.Nafgt;
                tab8 = MakePixelSumTab8();
                for (i = 0; i < n; i++)
                {  
                    pix1 = PixaGetPix(pixa1, i, m_clone);
                    NumaGetIValue(nafg, i, ref area1);
                    pix2 = PixaGetPix(pixa2, i, m_clone);
                    PtaGetPt(pta, i, ref x1, ref y1); 
                    nt = pixat.N;
                    found = false;
                    findcontext = FindSimilarSizedTemplatesInit(classer, pix1);
                    while ((iclass = FindSimilarSizedTemplatesNext(findcontext)) > -1)
                    {
                        pix3 = PixaGetPix(pixat, iclass, m_clone);
                        NumaGetIValue(nafgt, iclass, ref area3);
                        pix4 = PixaGetPix(pixatd, iclass, m_clone);
                        PtaGetPt(ptact, iclass, ref x2, ref y2);
                        testval = PixRankHaustest(pix1, pix2, pix3, pix4, x1 - x2, y1 - y2, m_maxDiffWidth, m_maxDiffHeight, area1, area3, rank, tab8);
                        if (testval == 1)
                        {
                            found = true;
                            NumaAddNumber(naclass, iclass);
                            NumaAddNumber(napage, npages);
                            if (classer.KeepPixaa > 0)
                            {
                                pixa = PixaaGetPixa(pixaa, iclass, m_clone);
                                pix = PixaGetPix(pixas, i, m_clone);
                                PixaAddPix(pixa, pix, m_insert);
                                box = BoxaGetBox(boxa, i, m_clone);
                                PixaAddBox(pixa, box, m_insert);
                            }
                            break;
                        }
                    }

                    if (!found)
                    {
                        NumaAddNumber(naclass, nt);
                        NumaAddNumber(napage, npages);
                        pixa = JBIG2Statics.CreatePixa(0);
                        pix = PixaGetPix(pixas, i, m_clone);
                        PixaAddPix(pixa, pix, m_insert);
                        wt = pix.W;
                        ht = pix.H;
                        NumaHashAdd(nahash, ht * wt, nt);
                        box = BoxaGetBox(boxa, i, m_clone);
                        PixaAddBox(pixa, box, m_insert);
                        PixaaAddPixa(pixaa, pixa, m_insert);
                        PtaAddPt(ptact, x1, y1);
                        PixaAddPix(pixat, pix1, m_insert);
                        PixaAddPix(pixatd, pix2, m_insert);
                        NumaAddNumber(nafgt, area1);
                    }
                    else
                    { 
                    }
                }
            }
            classer.NClass = pixat.N;

            return false;
        }

        Numa PixaCountPixels(Pixa pixa)
        {
            int d, i, n, count = 0;
            int[] tab;
            Numa na;
            Pix pix;

            if ((n = pixa.N) == 0)
                return JBIG2Statics.CreateNuma(1);

            pix = PixaGetPix(pixa, 0, m_clone);
            d = pix.D;
            if (d != 1)
            { }// return (NUMA*)ERROR_PTR("pixa not 1 bpp", procName, null);

            tab = MakePixelSumTab8();
            if ((na = JBIG2Statics.CreateNuma(n)) == null)
            { }// return (NUMA*)ERROR_PTR("na not made", procName, null);
            for (i = 0; i < n; i++)
            {
                pix = PixaGetPix(pixa, i, m_clone);
                PixCountPixels(pix, ref count, tab);
                NumaAddNumber(na, count);
            }

            return na;
        }

        int PixCountPixels(Pix pix, ref int pcount, int[] tab8)
        {
            uint endmask;
            int w, h, wpl, i, j;
            int fullwords, endbits, sum;
            int[] tab;
            uint[] data;

            if (pix == null || pix.D != 1)
            { return -1; }

            if (tab8 == null)
                tab = MakePixelSumTab8();
            else
                tab = tab8;

            w = pix.W; h = pix.H;
            wpl = pix.Wpl;
            data = pix.Data;
            fullwords = w >> 5;
            endbits = w & 31;
            endmask = (0xffffffff << (32 - endbits));

            sum = 0;
            int index = 0;
            for (i = 0; i < h; i++, index += wpl)
            {
                for (j = 0; j < fullwords; j++)
                {
                    uint word = data[index + j];
                    if (word > 0)
                        sum += (tab[word & 0xff] + tab[(word >> 8) & 0xff] + tab[(word >> 16) & 0xff] + tab[(word >> 24) & 0xff]);
                }
                if (endbits > 0)
                {
                    uint word = data[index + j] & endmask;
                    if (word > 0)
                        sum += (tab[word & 0xff] + tab[(word >> 8) & 0xff] + tab[(word >> 16) & 0xff] + tab[(word >> 24) & 0xff]);
                }
            }
            pcount = sum;

            return 0;
        }

        int PixRankHaustest(Pix pix1, Pix pix2, Pix pix3, Pix pix4, float delx, float dely, int maxdiffw, int maxdiffh, int area1, int area3, float rank, int[] tab8)
        {
            int wi, hi, wt, ht, delw, delh, idelx, idely, boolmatch = 0;
            int thresh1, thresh3;
            Pix pixt;

            wi = pix1.W;
            hi = pix1.H;
            wt = pix3.W;
            ht = pix3.H;
            delw = Math.Abs(wi - wt);
            if (delw > maxdiffw)
                return 1;
            delh = Math.Abs(hi - ht);
            if (delh > maxdiffh)
                return 1;

            thresh1 = (int)(area1 * (1 - rank) + 0.5);
            thresh3 = (int)(area3 * (1 - rank) + 0.5);

            if (delx >= 0)
                idelx = (int)(delx + 0.5);
            else
                idelx = (int)(delx - 0.5);
            if (dely >= 0)
                idely = (int)(dely + 0.5);
            else
                idely = (int)(dely - 0.5);

            pixt = PixCreateTemplate(pix1);
            PixRasterop(pixt, 0, 0, wi, hi, JBIG2Statics.PixSrc, pix1, 0, 0);
            PixRasterop(pixt, idelx, idely, wi, hi, JBIG2Statics.PixDst & JBIG2Statics.PixNot(JBIG2Statics.PixSrc), pix4, 0, 0);
            PixThresholdPixelSum(pixt, thresh1, boolmatch, tab8);
            if (boolmatch == 1)
                return 1;/* above thresh1 */

            PixRasterop(pixt, idelx, idely, wt, ht, JBIG2Statics.PixSrc, pix3, 0, 0);
            PixRasterop(pixt, 0, 0, wt, ht, JBIG2Statics.PixDst & JBIG2Statics.PixNot(JBIG2Statics.PixSrc), pix2, 0, 0);
            PixThresholdPixelSum(pixt, thresh3, boolmatch, tab8);
            if (boolmatch == 1)  /* above thresh3 */
                return 1;
            else
                return 0;
        }

        void PixThresholdPixelSum(Pix pix, int thresh, int pabove, int[] tab8)
        {
            uint word, endmask;
            int[] tab;
            int w, h, wpl, i, j;
            int fullwords, endbits, sum;
            uint[] data;
            int index;

            if (pix == null || pix.D != 1)
            { }// return ERROR_INT("pix not defined or not 1 bpp", procName, 1);

            if (tab8 == null)
                tab = MakePixelSumTab8();
            else
                tab = tab8;

            w = pix.W; h = pix.H;
            wpl = pix.Wpl;
            data = pix.Data;
            fullwords = w >> 5;
            endbits = w & 31;
            endmask = 0xffffffff << (32 - endbits);

            sum = 0;
            for (i = 0; i < h; i++)
            {
                index = wpl * i;
                for (j = 0; j < fullwords; j++)
                {
                    word = data[index + j];
                    if (word > 0)
                        sum += tab[word & 0xff] + tab[(word >> 8) & 0xff] + tab[(word >> 16) & 0xff] + tab[(word >> 24) & 0xff];
                }
                if (endbits > 0)
                {
                    word = data[index + j] & endmask;
                    if (word > 0)
                        sum += tab[word & 0xff] + tab[(word >> 8) & 0xff] + tab[(word >> 16) & 0xff] + tab[(word >> 24) & 0xff];
                }
                if (sum > thresh)
                {
                    pabove = 1;
                    break;
                }
            }
        }

        int NumaHashAdd(NumaHash nahash, int key, float value)
        {
            int bucket;
            Numa na = null;

            if (key < 0)
            { }// return ERROR_INT("key < 0", procName, 1);
            bucket = key % nahash.NBuckets;
            if (nahash.Numa.ContainsKey(bucket))
                na = nahash.Numa[bucket];
            if (na == null)
            {
                if ((na = JBIG2Statics.CreateNuma(nahash.InitSize)) == null)
                { }// return ERROR_INT("na not made", procName, 1);
                nahash.Numa.Add(bucket, na);
            }
            NumaAddNumber(na, value);
            return 0;
        }


        int PixaaAddPixa(Pixaa pixaa, Pixa pixa, int copyflag)
        {
            int n;
            Pixa pixac;

            if (copyflag != m_insert && copyflag != m_copy && copyflag != m_clone && copyflag != m_copyClone)
            { }// return ERROR_INT("invalid copyflag", procName, 1);

            if (copyflag == m_insert)
                pixac = pixa;
            else
            {
                if ((pixac = PixaCopy(pixa, copyflag)) == null)
                { }// return ERROR_INT("pixac not made", procName, 1);
            }

            n = pixaa.N;
            if (n >= pixaa.Nalloc)
                pixaa.Nalloc *= 2;
            pixaa.Pixa.Add(pixac);
            pixaa.N++;

            return 0;
        }

        Pixa PixaaGetPixa(Pixaa pixaa, int index, int accesstype)
        {
            Pixa pixa;

            if (index < 0 || index >= pixaa.N)
            {
                Console.WriteLine("index not valid"); return null;
            }
            if (accesstype != m_copy && accesstype != m_clone && accesstype != m_copyClone)
            {
                Console.WriteLine("invalid accesstype"); return null;
            }

            if ((pixa = pixaa.Pixa[index]) == null)  /* shouldn't happen! */
            {
                Console.WriteLine("no pixa[index]"); return null;
            }
            return PixaCopy(pixa, accesstype);
        }

        Pixa PixaCopy(Pixa pixa, int copyflag)
        {
            int i;
            Box boxc;
            Pix pixc;
            Pixa pixac;

            if (copyflag == m_clone)
            {
                pixa.RefCount += 1;
                return pixa;
            }

            if (copyflag != m_copy && copyflag != m_copyClone)
            { }// return (PIXA*)ERROR_PTR("invalid copyflag", procName, null);

            if ((pixac = JBIG2Statics.CreatePixa(pixa.N)) == null)
            { }// return (PIXA*)ERROR_PTR("pixac not made", procName, null);
            for (i = 0; i < pixa.N; i++)
            {
                if (copyflag == m_copy)
                {
                    pixc = PixaGetPix(pixa, i, m_copy);
                    boxc = PixaGetBox(pixa, i, m_copy);
                }
                else
                {  /* copy-clone */
                    pixc = PixaGetPix(pixa, i, m_clone);
                    boxc = PixaGetBox(pixa, i, m_clone);
                }
                PixaAddPix(pixac, pixc, m_insert);
                PixaAddBox(pixac, boxc, m_insert);
            }

            return pixac;
        }

        int PixHausTest(Pix pix1, Pix pix2, Pix pix3, Pix pix4, float delx, float dely, int maxdiffw, int maxdiffh)
        {
            int wi, hi, wt, ht, delw, delh, idelx, idely;
            int boolmatch = 0;
            Pix pixt;

            wi = pix1.W;
            hi = pix1.H;
            wt = pix3.W;
            ht = pix3.H;
            delw = Math.Abs(wi - wt);
            if (delw > maxdiffw)
                return 1;
            delh = Math.Abs(hi - ht);
            if (delh > maxdiffh)
                return 1;

            if (delx >= 0)
                idelx = (int)(delx + 0.5);
            else
                idelx = (int)(delx - 0.5);
            if (dely >= 0)
                idely = (int)(dely + 0.5);
            else
                idely = (int)(dely - 0.5);

            pixt = PixCreateTemplate(pix1);
            PixRasterop(pixt, 0, 0, wi, hi, JBIG2Statics.PixSrc, pix1, 0, 0);
            PixRasterop(pixt, idelx, idely, wi, hi, JBIG2Statics.PixDst & JBIG2Statics.PixNot(JBIG2Statics.PixSrc), pix4, 0, 0);
            PixZero(pixt, ref boolmatch);
            if (boolmatch == 0)
                return boolmatch;

            PixRasterop(pixt, idelx, idely, wt, ht, JBIG2Statics.PixSrc, pix3, 0, 0);
            PixRasterop(pixt, 0, 0, wt, ht, JBIG2Statics.PixDst & JBIG2Statics.PixNot(JBIG2Statics.PixSrc), pix2, 0, 0);
            PixZero(pixt, ref boolmatch);
            return boolmatch;
        }

        int FindSimilarSizedTemplatesNext(JbTemplatesState state)
        {
            int desiredh, desiredw, size, templ;
            Pix pixt;

            while (true)
            {  
                if (state.I >= 25)
                    return -1;

                desiredw = state.W + JBIG2Statics.MatchOffset[2 * state.I];
                desiredh = state.H + JBIG2Statics.MatchOffset[2 * state.I + 1];
                if (desiredh < 1 || desiredw < 1)
                {
                    state.I++;
                    continue;
                }

                if (state.Numa == null)
                {
                    /* We have yet to start walking the array for the step 'i' */
                    state.Numa = NumaHashGetNuma(state.Classer.NaHash, desiredh * desiredw);
                    if (state.Numa == null)
                    {  /* nothing there */
                        state.I++;
                        continue;
                    }

                    state.N = 0;  
                }

                size = state.Numa.N;
                for (; state.N < size; )
                {
                    templ = (int)(state.Numa.Array[state.N++] + 0.5);
                    pixt = PixaGetPix(state.Classer.Pixat, templ, m_clone);
                    if (((pixt.W - 2 * m_jbAddedPixels) == desiredw) && ((pixt.H - 2 * m_jbAddedPixels) == desiredh))
                        return templ;
                }

                state.I++;
                state.Numa = null;
                continue;
            }
        }

        Numa NumaHashGetNuma(NumaHash nahash, int key)
        {
            int bucket;
            Numa na = null;

            bucket = key % nahash.NBuckets;
            if (nahash.Numa == null)
                return null;
            if (nahash.Numa.ContainsKey(bucket))
                na = nahash.Numa[bucket];
            if (na == null)
                return null;

            return na;
        }

        JbTemplatesState FindSimilarSizedTemplatesInit(JBIG2Classifier classer, Pix pixs)
        {
            JbTemplatesState state = new JbTemplatesState();

            state.W = pixs.W - 2 * m_jbAddedPixels;
            state.H = pixs.H - 2 * m_jbAddedPixels;
            state.Classer = classer;

            return state;
        }

        void PtaGetPt(Pta pta, int index, ref float px, ref float py)
        {
            px = py = -1;
            if (pta.X.Count > index)
                px = pta.X[index];
            if (pta.Y.Count > index)
                py = pta.Y[index];
        }

        Pta PixaCentroids(Pixa pixa)
        {
            int i, n;
            int[] centtab = new int[] { };
            int[] sumtab = new int[] { };
            float x = 0, y = 0;
            Pix pix;
            Pta pta;

            if ((n = pixa.N) == 0)
            { }// return (PTA*)ERROR_PTR("no pix in pixa", procName, null);

            if ((pta = JBIG2Statics.CreatePta(n)) == null)
            { }// return (PTA*)ERROR_PTR("pta not defined", procName, null);
            centtab = MakePixelCentroidTab8();
            sumtab = MakePixelSumTab8();

            for (i = 0; i < n; i++)
            {
                pix = PixaGetPix(pixa, i, m_clone);
                if (PixCentroid(pix, centtab, sumtab, x, y) == 1)
                { }// L_ERROR_INT("centroid failure for pix %d", procName, i);
                PtaAddPt(pta, x, y);
            }

            return pta;
        }

        int PixCentroid(Pix pix, int[] centtab, int[] sumtab, float pxave, float pyave)
        {
            int w, h, d, i, j, wpl, pixsum, rowsum;
            float xsum, ysum;
            uint[] data;
            uint word, bytes, val;
            int[] ctab, stab;

            w = pix.W; h = pix.H; d = pix.D;
            if (d != 1 && d != 8)
            { }// return ERROR_INT("pix not 1 or 8 bpp", procName, 1);

            if (centtab != null)
                ctab = MakePixelCentroidTab8();
            else
                ctab = centtab;
            if (sumtab != null)
                stab = MakePixelSumTab8();
            else
                stab = sumtab;

            data = pix.Data;
            wpl = pix.Wpl;
            xsum = ysum = 0.0f;
            pixsum = 0;
            int index;
            if (d == 1)
            {
                for (i = 0; i < h; i++)
                {
                    index = wpl * i;
                    rowsum = 0;
                    for (j = 0; j < wpl; j++)
                    {
                        word = data[index + j];
                        if (word == 1)
                        {
                            bytes = word & 0xff;
                            rowsum += stab[bytes];
                            xsum += ctab[bytes] + (j * 32 + 24) * stab[bytes];
                            bytes = (word >> 8) & 0xff;
                            rowsum += stab[bytes];
                            xsum += ctab[bytes] + (j * 32 + 16) * stab[bytes];
                            bytes = (word >> 16) & 0xff;
                            rowsum += stab[bytes];
                            xsum += ctab[bytes] + (j * 32 + 8) * stab[bytes];
                            bytes = (word >> 24) & 0xff;
                            rowsum += stab[bytes];
                            xsum += ctab[bytes] + j * 32 * stab[bytes];
                        }
                    }
                    pixsum += rowsum;
                    ysum += rowsum * i;
                }
                if (pixsum == 0)
                { }//L_WARNING("no ON pixels in pix", procName);
                else
                {
                    pxave = xsum / (float)pixsum;
                    pyave = ysum / (float)pixsum;
                }
            }
            else
            {  /* d == 8 */
                for (i = 0; i < h; i++)
                {
                    index = wpl * i;
                    for (j = 0; j < w; j++)
                    {
                        val = JBIG2Statics.GetDataByte(data, index + j);
                        xsum += val * j;
                        ysum += val * i;
                        pixsum += (int)val;
                    }
                }
                if (pixsum == 0)
                { }//L_WARNING("all pixels are 0", procName);
                else
                {
                    pxave = xsum / (float)pixsum;
                    pyave = ysum / (float)pixsum;
                }
            }
            return 0;
        }

        int[] MakePixelCentroidTab8()
        {
            int i;
            int[] tab = new int[256];

            tab[0] = 0;
            tab[1] = 7;
            for (i = 2; i < 4; i++)
                tab[i] = tab[i - 2] + 6;
            for (i = 4; i < 8; i++)
                tab[i] = tab[i - 4] + 5;
            for (i = 8; i < 16; i++)
                tab[i] = tab[i - 8] + 4;
            for (i = 16; i < 32; i++)
                tab[i] = tab[i - 16] + 3;
            for (i = 32; i < 64; i++)
                tab[i] = tab[i - 32] + 2;
            for (i = 64; i < 128; i++)
                tab[i] = tab[i - 64] + 1;
            for (i = 128; i < 256; i++)
                tab[i] = tab[i - 128];

            return tab;
        }


        int PtaJoin(Pta ptad, Pta ptas, int istart, int iend)
        {
            int ns, i, x = 0, y = 0;
            ns = ptas.N;
            if (istart < 0)
                istart = 0;
            if (istart >= ns)
            { }// return ERROR_INT("istart out of bounds", procName, 1);
            if (iend <= 0)
                iend = ns - 1;
            if (iend >= ns)
            { }// return ERROR_INT("iend out of bounds", procName, 1);
            if (istart > iend)
            { }// return ERROR_INT("istart > iend; no pts", procName, 1);

            for (i = istart; i <= iend; i++)
            {
                PtaGetIPt(ptas, i, ref x, ref y);
                PtaAddPt(ptad, x, y);
            }

            return 0;
        }

        void PtaAddPt(Pta pta, float x, float y)
        {
            int n = pta.N;
            if (n >= pta.Nalloc)
                pta.Nalloc *= 2;
            pta.X.Add(x);
            pta.Y.Add(y);
            pta.N++;
        }

        int PtaGetIPt(Pta pta, int index, ref int px, ref int py)
        {
            px = (int)(pta.X[index] + 0.5);
            py = (int)(pta.Y[index] + 0.5);
            return 0;
        }

        /// <summary>
        /// Finds image components.
        /// </summary>
        private bool JBGetComponents(Pix pixs, int components, int maxwidth, int maxheight, ref Boxa pboxad, ref Pixa ppixad)
        {
            int empty = 0, res, redfactor;
            Boxa boxa;
            Pix pixt1, pixt2, pixt3;
            Pixa pixa = null, pixat = null;

            int JB_CONN_COMPS = 0, JB_CHARACTERS = 1, JB_WORDS = 2;

            if (components != JB_CONN_COMPS && components != JB_CHARACTERS && components != JB_WORDS)
                return false;

            PixZero(pixs, ref empty);
            if (empty < 0)
            {
                pboxad = JBIG2Statics.CreateBoxa(0);
                ppixad = JBIG2Statics.CreatePixa(0);
                return true;
            }

            if (components == JB_CONN_COMPS)
                boxa = PixConnComp(pixs, ref pixa, 8);
            else if (components == JB_CHARACTERS)
            {
                pixt1 = PixMorphSequence(pixs, "c1.6".ToCharArray(), 0);
                boxa = PixConnComp(pixt1, ref pixat, 8);
                pixa = PixaClipToPix(pixat, pixs);
            }
            else
            {
                res = pixs.XRes;
                if (res <= 200)
                {
                    redfactor = 1;
                    pixt1 = pixs;
                }
                else if (res <= 400)
                {
                    redfactor = 2;
                    pixt1 = PixReduceRankBinaryCascade(pixs, 1, 0, 0, 0);
                }
                else
                {
                    redfactor = 4;
                    pixt1 = PixReduceRankBinaryCascade(pixs, 1, 1, 0, 0);
                }

                pixt2 = PixWordMaskByDilation(pixt1, 0, -1);
                pixt3 = PixExpandReplicate(pixt2, redfactor);

                boxa = PixConnComp(pixt3, ref pixat, 4);
                pixa = PixaClipToPix(pixat, pixs);
            }

            ppixad = PixaSelectBySize(pixa, maxwidth, maxheight, JBIG2Statics.SelectIfBoth, JBIG2Statics.SelectIfLte, false);
            pboxad = BoxaSelectBySize(boxa, maxwidth, maxheight, JBIG2Statics.SelectIfBoth, JBIG2Statics.SelectIfLte, false);
            ppixad.RefCount--;
            pboxad.RefCount--;

            return true;
        }

        Boxa BoxaSelectBySize(Boxa boxas, int width, int height, int type, int relation, bool pchanged)
        {
            Boxa boxad;
            Numa na;

            if (type != JBIG2Statics.SelectWidth && type != JBIG2Statics.SelectHeight &&
                type != JBIG2Statics.SelectIfEither && type != JBIG2Statics.SelectIfBoth)
            { }
            if (relation != JBIG2Statics.SelectIfLt && relation != JBIG2Statics.SelectIfGt &&
                relation != JBIG2Statics.SelectIfLte && relation != JBIG2Statics.SelectIfGte)
            { }
            if (pchanged)
                pchanged = false;

            na = BoxaMakeSizeIndicator(boxas, width, height, type, relation);
            boxad = BoxaSelectWithIndicator(boxas, na, pchanged);

            return boxad;
        }

        Boxa BoxaSelectWithIndicator(Boxa boxas, Numa na, bool pchanged)
        {
            int i, n, ival = 0, nsave;
            Box box;
            Boxa boxad;

            nsave = 0;
            n = na.N;
            for (i = 0; i < n; i++)
            {
                NumaGetIValue(na, i, ref ival);
                if (ival == 1) nsave++;
            }

            if (nsave == n)
            {
                if (pchanged)
                    pchanged = false;
                return BoxaCopy(boxas, m_clone);
            }
            if (pchanged) pchanged = true;
            boxad = CreateBoxa(nsave);
            for (i = 0; i < n; i++)
            {
                NumaGetIValue(na, i, ref ival);
                if (ival == 0)
                    continue;
                box = BoxaGetBox(boxas, i, m_clone);
                BoxaAddBox(boxad, box, m_insert);
            }

            return boxad;
        }

        Boxa CreateBoxa(int n)
        {
            int INITIAL_PTR_ARRAYSIZE = 20;
            if (n <= 0)
                n = INITIAL_PTR_ARRAYSIZE;

            Boxa boxa = new Boxa(n);
            return boxa;
        }

        Pixa PixaSelectBySize(Pixa pixas, int width, int height, int type, int relation, bool pchanged)
        {
            Boxa boxa;
            Numa na;
            Pixa pixad;

            if (type != JBIG2Statics.SelectWidth && type != JBIG2Statics.SelectHeight &&
                type != JBIG2Statics.SelectIfEither && type != JBIG2Statics.SelectIfBoth)
            {
                Console.WriteLine("invalid type"); return null;
            }
            if (relation != JBIG2Statics.SelectIfLt && relation != JBIG2Statics.SelectIfGt &&
                relation != JBIG2Statics.SelectIfLte && relation != JBIG2Statics.SelectIfGte)
            {
                Console.WriteLine("invalid relation"); return null;
            }

            boxa = PixaGetBoxa(pixas, m_clone);
            na = BoxaMakeSizeIndicator(boxa, width, height, type, relation);
            boxa.RefCount--;

            pixad = PixaSelectWithIndicator(pixas, na, pchanged);

            return pixad;
        }

        /// <summary>
        /// Filter.
        /// </summary>
        Pixa PixaSelectWithIndicator(Pixa pixas, Numa na, bool pchanged)
        {
            int i, n, ival = 0, nsave;
            Box box;
            Pix pixt;
            Pixa pixad;

            nsave = 0;
            n = na.N;
            for (i = 0; i < n; i++)
            {
                NumaGetIValue(na, i, ref ival);
                if (ival == 1) nsave++;
            }

            if (nsave == n)
            {
                if (pchanged) pchanged = false;
                return PixaCopy(pixas, m_clone);
            }
            if (pchanged) pchanged = true;
            pixad = JBIG2Statics.CreatePixa(nsave);
            for (i = 0; i < n; i++)
            {
                NumaGetIValue(na, i, ref ival);
                if (ival == 0) continue;
                pixt = PixaGetPix(pixas, i, m_clone);
                box = PixaGetBox(pixas, i, m_clone);
                PixaAddPix(pixad, pixt, m_insert);
                PixaAddBox(pixad, box, m_insert);
            }

            return pixad;
        }

        /// <summary>
        /// Gets the value from the array.
        /// </summary>
        int NumaGetIValue(Numa na, int index, ref int pival)
        {
            float val;

            pival = 0;

            if (index < 0 || index >= na.N)
            {//    return ERROR_INT("index not valid", procName, 1);
            }

            val = na.Array[index];
            int temp = val < 0 ? -1 : 1;
            pival = (int)(val + temp * 0.5);
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        Numa BoxaMakeSizeIndicator(Boxa boxa, int width, int height, int type, int relation)
        {
            int i, n, w = 0, h = 0, ival;
            Numa na;

            if (type != JBIG2Statics.SelectWidth && type != JBIG2Statics.SelectHeight &&
                type != JBIG2Statics.SelectIfEither && type != JBIG2Statics.SelectIfBoth)
            {
                Console.WriteLine("invalid type"); return null;
            }
            if (relation != JBIG2Statics.SelectIfLt && relation != JBIG2Statics.SelectIfGt &&
                relation != JBIG2Statics.SelectIfLte && relation != JBIG2Statics.SelectIfGte)
            {
                Console.WriteLine("invalid relation"); return null;
            }

            n = boxa.N;
            na = JBIG2Statics.CreateNuma(n);
            for (i = 0; i < n; i++)
            {
                ival = 0;
                int x = 0, y = 0;
                BoxaGetBoxGeometry(boxa, i, ref x, ref y, ref w, ref h);
                switch (type)
                {
                    case JBIG2Statics.SelectWidth:
                        if ((relation == JBIG2Statics.SelectIfLt && w < width) ||
                            (relation == JBIG2Statics.SelectIfGt && w > width) ||
                            (relation == JBIG2Statics.SelectIfLte && w <= width) ||
                            (relation == JBIG2Statics.SelectIfGte && w >= width))
                            ival = 1;
                        break;
                    case JBIG2Statics.SelectHeight:
                        if ((relation == JBIG2Statics.SelectIfLt && h < height) ||
                            (relation == JBIG2Statics.SelectIfGt && h > height) ||
                            (relation == JBIG2Statics.SelectIfLte && h <= height) ||
                            (relation == JBIG2Statics.SelectIfGte && h >= height))
                            ival = 1;
                        break;
                    case JBIG2Statics.SelectIfEither:
                        if (((relation == JBIG2Statics.SelectIfLt) && (w < width || h < height)) ||
                            ((relation == JBIG2Statics.SelectIfGt) && (w > width || h > height)) ||
                           ((relation == JBIG2Statics.SelectIfLte) && (w <= width || h <= height)) ||
                            ((relation == JBIG2Statics.SelectIfGte) && (w >= width || h >= height)))
                            ival = 1;
                        break;
                    case JBIG2Statics.SelectIfBoth:
                        if (((relation == JBIG2Statics.SelectIfLt) && (w < width && h < height)) ||
                            ((relation == JBIG2Statics.SelectIfGt) && (w > width && h > height)) ||
                           ((relation == JBIG2Statics.SelectIfLte) && (w <= width && h <= height)) ||
                            ((relation == JBIG2Statics.SelectIfGte) && (w >= width && h >= height)))
                            ival = 1;
                        break;
                    default:
                        break;
                }
                NumaAddNumber(na, ival);
            }

            return na;
        }

        /// <summary>
        /// Adds number and extends the array.
        /// </summary>
        /// <param name="na"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        void NumaAddNumber(Numa na, float val)
        {
            int n = na.N;
            if (n >= na.Nalloc)
                na.Nalloc *= 2;
            na.Array.Add(val);
            na.N++;
        }

        /// <summary>
        /// Returns Box rectangle.
        /// </summary>
        void BoxaGetBoxGeometry(Boxa boxa, int index, ref int px, ref int py, ref int pw, ref int ph)
        {
            Box box = BoxaGetBox(boxa, index, 2);
            px = box.X;
            py = box.Y;
            pw = box.W;
            ph = box.H;
        }

        /// <summary>
        /// Returns Pixa
        /// </summary>
        Boxa PixaGetBoxa(Pixa pixa, int accesstype)
        {
            if (pixa.Boxa == null)
            {
                Console.WriteLine("boxa not defined"); return null;
            }
            if (accesstype != 1 && accesstype != 2 && accesstype != 3)
            {
                Console.WriteLine("invalid accesstype"); return null;
            }

            return BoxaCopy(pixa.Boxa, accesstype);
        }

        /// <summary>
        /// Return pix with dilated word mask.
        /// </summary>
        private Pix PixWordMaskByDilation(Pix pixs, int maxsize, int psize)
        {
            int MAX_ALLOWED_DILATION = 14;
            int i, diffmin, ndiff, imin = 0;
            int[] ncc = new int[MAX_ALLOWED_DILATION + 1];
            Boxa boxa;
            Numa nacc;
            Pix pixt1, pixt2, pixt3;
            Pixa pixa;
            Sel sel;

            diffmin = 1000000;
            pixa = JBIG2Statics.CreatePixa(8);
            pixt1 = PixCopy(null, pixs);
            PixaAddPix(pixa, pixt1, 2);

            if (maxsize <= 0)
                maxsize = 7;
            if (maxsize > MAX_ALLOWED_DILATION)
                maxsize = MAX_ALLOWED_DILATION;
            nacc = JBIG2Statics.CreateNuma(maxsize);
            for (i = 0; i <= maxsize; i++)
            {
                if (i == 0) 
                    pixt2 = PixCopy(null, pixt1);
                else   
                    pixt2 = PixMorphSequence(pixt1, "d2.1".ToCharArray(), 0);
                boxa = PixConnCompBB(pixt2, 4);
                ncc[i] = boxa.N;
                NumaAddNumber(nacc, ncc[i]);
                if (i > 0)
                {
                    ndiff = ncc[i - 1] - ncc[i];
                    if (ndiff < diffmin)
                    {
                        imin = i;
                        diffmin = ndiff;
                    }
                }
                PixaAddPix(pixa, pixt2, 1);
                pixt1 = null;
                pixt1 = pixt2;
                boxa = null;
            }
            pixt1 = null;

            pixt2 = PixaGetPix(pixa, imin, 2);
            sel = SelCreateBrick(1, imin, 0, imin - 1, 1);
            pixt3 = PixErode(null, pixt2, sel);
            sel = null;
            pixt2 = null;
            pixa = null;
            if (psize < 0)
                psize = imin + 1;

            nacc = null;
            return pixt3;
        }

        /// <summary>
        /// Sequence of binary rasterop morphological operations.
        /// </summary>
        private Pix PixMorphSequence(Pix pixs, char[] sequence, int dispsep)
        {
            string rawop;
            char[] op;
            int nops, i, j, nred, fact, w, h, x, y, border;
            int[] level = new int[4];
            Pix pixt1 = null, pixt2;
            Sarray sa;

            sa = SarrayCreate(0);
            SarraySplitString(sa, sequence, '+');
            nops = sa.N;

            if (MorphSequenceVerify(sa) != 1)
            {
                sa = null;
                Console.WriteLine("sequence not valid"); return null;
            }

            /* Parse and operate */
            border = 0;
            pixt1 = PixCopy(null, pixs);
            pixt2 = null;
            x = y = 0;
            w = 1; h = 6;
            for (i = 0; i < nops; i++)
            {
                rawop = sa.Array[i];
                op = rawop.Replace(" \n\t", "").ToCharArray();
                switch (op[0])
                {
                    case 'd':
                    case 'D':
                        pixt2 = PixDilateBrick(null, pixt1, w, h);
                        pixt1 = null;
                        pixt1 = pixt2;
                        pixt2 = null;
                        if (dispsep > 0)
                        {
                            PixDisplay(pixt1, x, y);
                            x += dispsep;
                        }
                        break;
                    case 'e':
                    case 'E':
                        pixt2 = PixErodeBrick(null, pixt1, w, h);
                        pixt1 = null;
                        pixt1 = pixt2;
                        pixt2 = null;
                        if (dispsep > 0)
                        {
                            PixDisplay(pixt1, x, y);
                            x += dispsep;
                        }
                        break;
                    case 'o':
                    case 'O':
                        PixOpenBrick(pixt1, pixt1, w, h);
                        if (dispsep > 0)
                        {
                            PixDisplay(pixt1, x, y);
                            x += dispsep;
                        }
                        break;
                    case 'c':
                    case 'C':
                        PixCloseSafeBrick(pixt1, pixt1, w, h);
                        if (dispsep > 0)
                        {
                            PixDisplay(pixt1, x, y);
                            x += dispsep;
                        }
                        break;
                    case 'r':
                    case 'R':
                        nred = op.Length - 1;
                        for (j = 0; j < nred; j++)
                            level[j] = op[j + 1] - '0';
                        for (j = nred; j < 4; j++)
                            level[j] = 0;
                        pixt2 = PixReduceRankBinaryCascade(pixt1, level[0], level[1], level[2], level[3]);
                        pixt1 = null;
                        pixt1 = pixt2;
                        pixt2 = null;
                        if (dispsep > 0)
                        {
                            PixDisplay(pixt1, x, y);
                            x += dispsep;
                        }
                        break;
                    case 'x':
                    case 'X':
                        fact = 1;
                        pixt2 = PixExpandReplicate(pixt1, fact);
                        pixt1 = null;
                        pixt1 = pixt2;
                        pixt2 = null;
                        if (dispsep > 0)
                        {
                            PixDisplay(pixt1, x, y);
                            x += dispsep;
                        }
                        break;
                    case 'b':
                    case 'B':
                        border = 1;
                        pixt2 = PixAddBorder(pixt1, border, 0);
                        pixt1 = null;
                        pixt1 = pixt2;
                        pixt2 = null;
                        if (dispsep > 0)
                        {
                            PixDisplay(pixt1, x, y);
                            x += dispsep;
                        }
                        break;
                    default:
                        /* All invalid ops are caught in the first pass */
                        break;
                }
                op = null;
            }
            if (border > 0)
            {
                pixt2 = PixRemoveBorder(pixt1, border);
                pixt1 = null;
                pixt1 = pixt2;
                pixt2 = null;
            }

            sa = null;
            return pixt1;
        }

        /// <summary>
        /// Replicated integer expansion.
        /// </summary>
        Pix PixExpandReplicate(Pix pixs, int factor)
        {
            int w, h, d, wd, hd, wpls, wpld, bpld, start, i, j, k;
            uint sval, sval32;
            short sval16;
            uint[] datas, datad;
            Pix pixd = null;

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (d != 1 && d != 2 && d != 4 && d != 8 && d != 16 && d != 32)
             return null;
            if (factor <= 0)
             return null;
            if (factor == 1)
                return PixCopy(null, pixs);

            if (d == 1)
                return PixExpandBinaryReplicate(pixs, factor);

            wd = factor * w;
            hd = factor * h;
            if ((pixd = PixCreate(wd, hd, d)) == null)
            {
                Console.WriteLine("pixd not made"); return null;
            }
            PixCopyColormap(pixd, pixs);
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, (float)factor, (float)factor);
            datas = pixs.Data;
            wpls = pixs.Wpl;
            datad = pixd.Data;
            wpld = pixd.Wpl;

            int indexs = 0, indexd = 0;

            switch (d)
            {
                case 2:
                    bpld = (wd + 3) / 4; 
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = factor * i * wpld;
                        for (j = 0; j < w; j++)
                        {
                            sval = JBIG2Statics.GetDataDibit(datas[indexs], j);
                            start = factor * j;
                            for (k = 0; k < factor; k++)
                                JBIG2Statics.SetDataDibit(ref datad, indexd + start + k, sval);
                        }
                        for (k = 1; k < factor; k++)
                        {
                            Array.Copy(datad, 0, datad, k * wpld, k * wpld);
                            uint[] temp = new uint[k * wpld];
                            Array.Copy(temp, datad, temp.Length);
                        }
                    }
                    break;
                case 4:
                    bpld = (wd + 1) / 2; 
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = factor * i * wpld;
                        for (j = 0; j < w; j++)
                        {
                            sval = JBIG2Statics.GetDataQbit(datas[indexs], j);
                            start = factor * j;
                            for (k = 0; k < factor; k++)
                                JBIG2Statics.SetDataQbit(ref datad, indexd + start + k, sval);
                        }
                        for (k = 1; k < factor; k++)
                        {
                            Array.Copy(datad, 0, datad, k * wpld, k * wpld);
                            uint[] temp = new uint[k * wpld];
                            Array.Copy(temp, datad, temp.Length);
                        }
                    }
                    break;
                case 8:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = factor * i * wpld;
                        for (j = 0; j < w; j++)
                        {
                            sval = JBIG2Statics.GetDataByte(datas, indexs + j);
                            start = factor * j;
                            for (k = 0; k < factor; k++)
                                JBIG2Statics.SetDataByte(ref datad, indexs + start + k, sval);
                        }
                        for (k = 1; k < factor; k++)
                        {
                            Array.Copy(datad, 0, datad, k * wpld, k * wpld);
                            uint[] temp = new uint[k * wpld];
                            Array.Copy(temp, datad, temp.Length);
                        }
                    }
                    break;
                case 16:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = factor * i * wpld;
                        for (j = 0; j < w; j++)
                        {
                            sval16 = JBIG2Statics.GetDataTwoBytes(datas, indexs + j);
                            start = factor * j;
                            for (k = 0; k < factor; k++)
                                JBIG2Statics.SetDataTwoBytes(ref datad, indexd + start + k, sval16);
                        }
                        for (k = 1; k < factor; k++)
                        {
                            Array.Copy(datad, 0, datad, k * wpld, k * wpld);
                            uint[] temp = new uint[k * wpld];
                            Array.Copy(temp, datad, temp.Length);
                        }
                    }
                    break;
                case 32:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = factor * i * wpld;
                        for (j = 0; j < w; j++)
                        {
                            sval32 = datas[indexs + j];
                            start = factor * j;
                            for (k = 0; k < factor; k++)
                                datad[indexd + start + k] = sval32;
                        }
                        for (k = 1; k < factor; k++)
                        {
                            Array.Copy(datad, 0, datad, k * wpld, k * wpld);
                            uint[] temp = new uint[k * wpld];
                            Array.Copy(temp, datad, temp.Length);
                        }
                    }
                    break;
                default:
                    Console.WriteLine("invalid depth");
                    break;
            }

            return pixd;
        }

        /// <summary>
        /// Replicated expansion integer scaling.
        /// </summary>
        Pix PixExpandBinaryReplicate(Pix pixs, int factor)
        {
            int w, h, d, wd, hd, wpls, wpld, i, j, k, start;
            uint[] datas, datad;
            Pix pixd = null;

            w = pixs.W; h = pixs.H; d = pixs.D;
           
            if (factor == 1)
                return PixCopy(null, pixs);
            if (factor == 2 || factor == 4 || factor == 8 || factor == 16)
                return PixExpandBinaryPower2(pixs, factor);

            wpls = pixs.Wpl;
            datas = pixs.Data;
            wd = factor * w;
            hd = factor * h;
            pixd = PixCreate(wd, hd, 1);
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, (float)factor, (float)factor);
            wpld = pixd.Wpl;
            datad = pixd.Data;

            int indexs = 0, indexd = 0;

            for (i = 0; i < h; i++)
            {
                indexs = wpls * i;
                indexd = factor * i * wpld;

                for (j = 0; j < w; j++)
                {
                    if (JBIG2Statics.GetDataBit(datas, indexs, j) == 1)
                    {
                        start = factor * j;
                        for (k = 0; k < factor; k++)
                            JBIG2Statics.SetDataBit(ref datad[indexd], start + k);
                    }
                }
                for (k = 1; k < factor; k++)
                {
                    Array.Copy(datad, 0, datad, k * wpld, k * wpld);
                    uint[] temp = new uint[k * wpld];
                    Array.Copy(temp, datad, temp.Length);
                }
            }

            return pixd;
        }

        /// <summary>
        /// Power of 2 expansion.
        /// </summary>
        Pix PixExpandBinaryPower2(Pix pixs, int factor)
        {
            int w, h, d, wd, hd, wpls, wpld;
            uint[] datas, datad;
            Pix pixd = null;

            w = pixs.W; h = pixs.H; d = pixs.D;
           
            if (factor == 1)
                return PixCopy(null, pixs);
            if (factor != 2 && factor != 4 && factor != 8 && factor != 16)
                return null;

            wpls = pixs.Wpl;
            datas = pixs.Data;
            wd = factor * w;
            hd = factor * h;
            pixd = PixCreate(wd, hd, 1);
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, (float)factor, (float)factor);
            wpld = pixd.Wpl;
            datad = pixd.Data;

            ExpandBinaryPower2Low(datad, wd, hd, wpld, datas, w, h, wpls, factor);

            return pixd;
        }

        /// <summary>
        /// Binary morphological (raster) ops with brick Sels.
        /// </summary>
        Pix PixCloseSafeBrick(Pix pixd, Pix pixs, int hsize, int vsize)
        {
            int maxtrans, bordsize;
            Pix pixsb, pixt, pixdb;
            Sel sel, selh, selv;

            if (pixs.D != 1)
            {//return (PIX *)ERROR_PTR("pixs not 1 bpp", procName, pixd);
            }
            if (hsize < 1 || vsize < 1)
            {//       return (PIX *)ERROR_PTR("hsize and vsize not >= 1", procName, pixd);
            }

            if (hsize == 1 && vsize == 1)
                return PixCopy(pixd, pixs);

            if (MORPH_BC == 0)
                return PixCloseBrick(pixd, pixs, hsize, vsize);

            maxtrans = Math.Max(hsize / 2, vsize / 2);
            bordsize = 32 * ((maxtrans + 31) / 32);  /* full 32 bit words */
            pixsb = PixAddBorder(pixs, bordsize, 0);

            if (hsize == 1 || vsize == 1)
            { 
                sel = SelCreateBrick(vsize, hsize, vsize / 2, hsize / 2, JBIG2Statics.SelHit);
                pixdb = PixClose(null, pixsb, sel);
            }
            else
            {
                selh = SelCreateBrick(1, hsize, 0, hsize / 2, JBIG2Statics.SelHit);
                selv = SelCreateBrick(vsize, 1, vsize / 2, 0, JBIG2Statics.SelHit);
                pixt = PixDilate(null, pixsb, selh);
                pixdb = PixDilate(null, pixt, selv);
                PixErode(pixt, pixdb, selh);
                PixErode(pixdb, pixt, selv);
            }

            pixt = PixRemoveBorder(pixdb, bordsize);

            if (pixd == null)
                pixd = pixt;
            else
                PixCopy(pixd, pixt);

            return pixd;
        }

        /// <summary>
        /// Removes border of the pix.
        /// </summary>
        Pix PixRemoveBorder(Pix pixs, int npix)
        {
            if (npix == 0)
                return pixs;
            return PixRemoveBorderGeneral(pixs, npix, npix, npix, npix);
        }

        /// <summary>
        /// Pix with pixels removed around border.
        /// </summary>
        Pix PixRemoveBorderGeneral(Pix pixs, int left, int right, int top, int bot)
        {
            int ws, hs, wd, hd, d;
            Pix pixd;

            if (left < 0 || right < 0 || top < 0 || bot < 0)
            {//return (PIX*)ERROR_PTR("negative border removed!", procName, null);
            }

            ws = pixs.W; hs = pixs.H; d = pixs.D;
            wd = ws - left - right;
            hd = hs - top - bot;
            if (wd <= 0)
            {//return (PIX*)ERROR_PTR("width must be > 0", procName, null);
            }
            if (hd <= 0)
            {//return (PIX*)ERROR_PTR("height must be > 0", procName, null);
            }
            if ((pixd = PixCreateNoInit(wd, hd, d)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixCopyColormap(pixd, pixs);

            PixRasterop(pixd, 0, 0, wd, hd, JBIG2Statics.PixSrc, pixs, left, top);
            return pixd;
        }

        /// <summary>
        /// Adds border to the pix.
        /// </summary>
        Pix PixAddBorder(Pix pixs, int npix, uint val)
        {
            if (npix == 0)
                return pixs;
            return PixAddBorderGeneral(pixs, npix, npix, npix, npix, val);
        }

        /// <summary>
        /// Adds border to the pix.
        /// </summary>
        Pix PixAddBorderGeneral(Pix pixs, int left, int right, int top, int bot, uint val)
        {
            int ws, hs, wd, hd, d = 0, op;
            Pix pixd = null;

            if (left < 0 || right < 0 || top < 0 || bot < 0)
                return null;

            ws = pixs.W; hs = pixs.H; d = pixs.D;
            wd = ws + left + right;
            hd = hs + top + bot;
            if ((pixd = PixCreateNoInit(wd, hd, d)) == null)
                return null;
            PixCopyResolution(pixd, pixs);
            PixCopyColormap(pixd, pixs);

            /* Set the new border pixels */
            op = JBIG2Statics.Undef;
            if (val == 0)
                op = JBIG2Statics.PixClr;
            else if ((d == 1 && val == 1) || (d == 2 && val == 3) || (d == 4 && val == 0xf) || (d == 8 && val == 0xff) ||
                     (d == 16 && val == 0xffff) || (d == 32 && (val >> 8) == 0xffffff))
                op = JBIG2Statics.PixSet;
            if (op == JBIG2Statics.Undef)
                PixSetAllArbitrary(pixd, val);   /* a little extra writing ! */
            else
            {
                PixRasterop(pixd, 0, 0, left, hd, op, null, 0, 0);
                PixRasterop(pixd, wd - right, 0, right, hd, op, null, 0, 0);
                PixRasterop(pixd, 0, 0, wd, top, op, null, 0, 0);
                PixRasterop(pixd, 0, hd - bot, wd, bot, op, null, 0, 0);
            }

            /* m_copy pixs into the interior */
            PixRasterop(pixd, left, top, ws, hs, JBIG2Statics.PixSrc, pixs, 0, 0);
            return pixd;
        }

        /// <summary>
        /// Binary morphological (raster) ops with brick Sels.
        /// </summary>
        Pix PixCloseBrick(Pix pixd, Pix pixs, int hsize, int vsize)
        {
            Pix pixt;
            Sel sel, selh, selv;

            if (pixs.D != 1)
            {//return (PIX *)ERROR_PTR("pixs not 1 bpp", procName, pixd);
            }
            if (hsize < 1 || vsize < 1)
            {//       return (PIX *)ERROR_PTR("hsize and vsize not >= 1", procName, pixd);
            }
            if (hsize == 1 && vsize == 1)
                return PixCopy(pixd, pixs);
            if (hsize == 1 || vsize == 1)
            { 
                sel = SelCreateBrick(vsize, hsize, vsize / 2, hsize / 2, JBIG2Statics.SelHit);
                pixd = PixClose(pixd, pixs, sel);
            }
            else
            {
                selh = SelCreateBrick(1, hsize, 0, hsize / 2, JBIG2Statics.SelHit);
                selv = SelCreateBrick(vsize, 1, vsize / 2, 0, JBIG2Statics.SelHit);
                pixt = PixDilate(null, pixs, selh);
                pixd = PixDilate(pixd, pixt, selv);
                PixErode(pixt, pixd, selh);
                PixErode(pixd, pixt, selv);
            }
            return pixd;
        }

        Pix PixClose(Pix pixd, Pix pixs, Sel sel)
        {
            Pix pixt;

            if ((pixd = ProcessMorphArgs2(pixd, pixs, sel)) == null)
            {//return (PIX*)ERROR_PTR("pixd not returned", procName, pixd);
            }

            if ((pixt = PixDilate(null, pixs, sel)) == null)
            {//return (PIX*)ERROR_PTR("pixt not made", procName, pixd);
            }
            PixErode(pixd, pixt, sel);

            return pixd;
        }

        /// <summary>
        /// Binary morphological (raster) ops with brick Sels.
        /// </summary>
        Pix PixOpenBrick(Pix pixd, Pix pixs, int hsize, int vsize)
        {
            Pix pixt;
            Sel sel, selh, selv;

            if (pixs.D != 1)
            {//return (PIX *)ERROR_PTR("pixs not 1 bpp", procName, pixd);
            }
            if (hsize < 1 || vsize < 1)
            {//return (PIX *)ERROR_PTR("hsize and vsize not >= 1", procName, pixd);
            }

            if (hsize == 1 && vsize == 1)
                return PixCopy(pixd, pixs);
            if (hsize == 1 || vsize == 1)
            {
                sel = SelCreateBrick(vsize, hsize, vsize / 2, hsize / 2, JBIG2Statics.SelHit);
                pixd = PixOpen(pixd, pixs, sel);
            }
            else
            { 
                selh = SelCreateBrick(1, hsize, 0, hsize / 2, JBIG2Statics.SelHit);
                selv = SelCreateBrick(vsize, 1, vsize / 2, 0, JBIG2Statics.SelHit);
                pixt = PixErode(null, pixs, selh);
                pixd = PixErode(pixd, pixt, selv);
                PixDilate(pixt, pixd, selh);
                PixDilate(pixd, pixt, selv);
            }

            return pixd;
        }

        Pix PixOpen(Pix pixd, Pix pixs, Sel sel)
        {
            Pix pixt;

            if ((pixd = ProcessMorphArgs2(pixd, pixs, sel)) == null)
            {//return (PIX*)ERROR_PTR("pixd not returned", procName, pixd);
            }

            if ((pixt = PixErode(null, pixs, sel)) == null)
            {//return (PIX*)ERROR_PTR("pixt not made", procName, pixd);
            }
            PixDilate(pixd, pixt, sel);

            return pixd;
        }

        Pix ProcessMorphArgs2(Pix pixd, Pix pixs, Sel sel)
        {
            int sx, sy;

            if (pixs.D != 1)
            {//return (PIX*)ERROR_PTR("pixs not 1 bpp", procName, pixd);
            }

            sx = sel.SX; sy = sel.SY;
            if (sx == 0 || sy == 0)
            {//return (PIX*)ERROR_PTR("sel of size 0", procName, pixd);
            }

            if (pixd == null)
                return PixCreateTemplate(pixs);
            PixResizeImageData(pixd, pixs);
            return pixd;
        }

        /// <summary>
        /// Image display for debugging.
        /// </summary>
        void PixDisplay(Pix pixs, int x, int y)
        {
            PixDisplayWithTitle(pixs, x, y, char.MinValue, 1);
        }

        /// <summary>
        /// Image display for debugging.
        /// </summary>
        void PixDisplayWithTitle(Pix pixs, int x, int y, char title, int dispflag)
        {
            int L_DISPLAY_WITH_XV = 1;
            int L_DISPLAY_WITH_XLI = 2;
            int L_DISPLAY_WITH_XZGV = 3;
            int L_DISPLAY_WITH_IV = 4;
            int var_DISPLAY_PROG = L_DISPLAY_WITH_IV;
            int MAX_DISPLAY_WIDTH = 1000;
            int MAX_DISPLAY_HEIGHT = 800;

            int L_BUF_SIZE = 512;
            int _MAX_PATH = 260;
            char[] buffer = new char[L_BUF_SIZE];
            int w, h, d;
            float ratw, rath, ratmin;
            Pix pixt;
            char[] fullpath = new char[_MAX_PATH];

            if (dispflag != 1) return;

            if (var_DISPLAY_PROG != L_DISPLAY_WITH_XV && var_DISPLAY_PROG != L_DISPLAY_WITH_XLI && var_DISPLAY_PROG != L_DISPLAY_WITH_XZGV && var_DISPLAY_PROG != L_DISPLAY_WITH_IV)
            {
                //return ERROR_INT("no program chosen for display", procName, 1);
            }

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (w <= MAX_DISPLAY_WIDTH && h <= MAX_DISPLAY_HEIGHT)
            {
                if (d == 16)  /* take MSB */
                    pixt = PixConvert16To8(pixs, 1);
                else
                    pixt = pixs;
            }
            else
            {
                ratw = (float)MAX_DISPLAY_WIDTH / (float)w;
                rath = (float)MAX_DISPLAY_HEIGHT / (float)h;
                ratmin = Math.Min(ratw, rath);
                if (ratmin < 0.125 && d == 1)
                    pixt = PixScaleToGray8(pixs);
                else if (ratmin < 0.25 && d == 1)
                    pixt = PixScaleToGray4(pixs);
                else if (ratmin < 0.33 && d == 1)
                    pixt = PixScaleToGray3(pixs);
                else if (ratmin < 0.5 && d == 1)
                    pixt = PixScaleToGray2(pixs);
                else
                    pixt = PixScale(pixs, ratmin, ratmin);
                //if (!pixt)
                //    return ERROR_INT("pixt not made", procName, 1);
            }
        }

        /// <summary>
        /// Top level scaling dispatcher.
        /// </summary>
        private Pix PixScale(Pix pixs, float scalex, float scaley)
        {
            int sharpwidth;
            float maxscale, sharpfract;

            maxscale = Math.Max(scalex, scaley);
            sharpfract = (maxscale < 0.7f) ? 0.2f : 0.4f;
            sharpwidth = (maxscale < 0.7) ? 1 : 2;

            return PixScaleGeneral(pixs, scalex, scaley, sharpfract, sharpwidth);
        }

        /// <summary>
        /// Top level scaling dispatcher without sharpening.
        /// </summary>
        Pix PixScaleGeneral(Pix pixs, float scalex, float scaley, float sharpfract, int sharpwidth)
        {
            int d;
            float maxscale;
            Pix pixt, pixt2, pixd;

            d = pixs.D;
            if (d != 1 && d != 2 && d != 4 && d != 8 && d != 16 && d != 32)
            {//                return (PIX*)ERROR_PTR("pixs not {1,2,4,8,16,32} bpp", procName, null);
            }
            if (scalex == 1.0 && scaley == 1.0)
                return PixCopy(null, pixs);

            if (d == 1)
                return PixScaleBinary(pixs, scalex, scaley);

            if ((pixt = PixConvertTo8Or32(pixs, 0, 1)) == null)
            {//return (PIX*)ERROR_PTR("pixt not made", procName, null);
            }

            /* Scale (up or down) */
            d = pixt.D;
            maxscale = Math.Max(scalex, scaley);
            if (maxscale < 0.7)
            { 
                pixt2 = PixScaleAreaMap(pixt, scalex, scaley);
                if (maxscale > 0.2 && sharpfract > 0.0 && sharpwidth > 0)
                    pixd = PixUnsharpMasking(pixt2, sharpwidth, sharpfract);
                else
                    pixd = pixt2;
            }
            else
            {
                if (d == 8)
                    pixt2 = PixScaleGrayLI(pixt, scalex, scaley);
                else  /* d == 32 */
                    pixt2 = PixScaleColorLI(pixt, scalex, scaley);
                if (maxscale < 1.4 && sharpfract > 0.0 && sharpwidth > 0)
                    pixd = PixUnsharpMasking(pixt2, sharpwidth, sharpfract);
                else
                    pixd = pixt2;
            }
            return pixd;
        }

        /// <summary>
        /// Linearly interpreted (up) scaling.
        /// </summary>
        Pix PixScaleColorLI(Pix pixs, float scalex, float scaley)
        {
            int ws, hs, wpls, wd, hd, wpld;
            uint[] datas, datad;
            float maxscale;
            Pix pixd;

            if (pixs == null || pixs.D != 32)
            {//return (PIX*)ERROR_PTR("pixs undefined or not 32 bpp", procName, null);
            }
            maxscale = Math.Max(scalex, scaley);
            if (maxscale < 0.7)
                return PixScale(pixs, scalex, scaley);

            if (scalex == 1.0 && scaley == 1.0)
                return PixCopy(null, pixs);
            if (scalex == 2.0 && scaley == 2.0)
                return PixScaleColor2xLI(pixs);
            if (scalex == 4.0 && scaley == 4.0)
                return PixScaleColor4xLI(pixs);

            ws = pixs.W; hs = pixs.H;
            datas = pixs.Data;
            wpls = pixs.Wpl;
            wd = (int)(scalex * (float)ws + 0.5);
            hd = (int)(scaley * (float)hs + 0.5);
            if ((pixd = PixCreate(wd, hd, 32)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, scalex, scaley);
            datad = pixd.Data;
            wpld = pixd.Wpl;
            ScaleColorLILow(datad, wd, hd, wpld, datas, ws, hs, wpls);
            return pixd;
        }

        /// <summary>
        /// Color interpolated scaling.
        /// </summary>
        void ScaleColorLILow(uint[] datad, int wd, int hd, int wpld, uint[] datas, int ws, int hs, int wpls)
        {
            int i, j, wm2, hm2;
            uint xpm, ypm;
            uint xp, yp, xf, yf;
            uint v00r, v01r, v10r, v11r, v00g, v01g, v10g, v11g;
            uint v00b, v01b, v10b, v11b, area00, area01, area10, area11;
            uint pixels1, pixels2, pixels3, pixels4, pixel;
            float scx, scy;
            int indexs = 0, indexd = 0;

            scx = 16 * (float)ws / (float)wd;
            scy = 16 * (float)hs / (float)hd;

            wm2 = ws - 2;
            hm2 = hs - 2;

            for (i = 0; i < hd; i++)
            {
                ypm = (uint)(scy * (float)i);
                yp = ypm >> 4;
                yf = ypm & 0x0f;
                indexd = i * wpld;
                indexs = (int)(yp * wpls);
                for (j = 0; j < wd; j++)
                {
                    xpm = (uint)(scx * (float)j);
                    xp = xpm >> 4;
                    xf = xpm & 0x0f;

                    pixels1 = datas[xp];

                    if (xp > wm2 || yp > hm2)
                    {
                        if (yp > hm2 && xp <= wm2)
                        {
                            pixels2 = datas[xp + 1];
                            pixels3 = pixels1;
                            pixels4 = pixels2;
                        }
                        else if (xp > wm2 && yp <= hm2)
                        {
                            pixels2 = pixels1;
                            pixels3 = datas[wpls + xp];
                            pixels4 = pixels3;
                        }
                        else
                        {
                            pixels4 = pixels3 = pixels2 = pixels1;
                        }
                    }
                    else
                    {
                        pixels2 = datas[xp + 1];
                        pixels3 = datas[wpls + xp];
                        pixels4 = datas[wpls + xp + 1];
                    }

                    area00 = (16 - xf) * (16 - yf);
                    area10 = xf * (16 - yf);
                    area01 = (16 - xf) * yf;
                    area11 = xf * yf;
                    v00r = area00 * ((pixels1 >> m_redShift) & 0xff);
                    v00g = area00 * ((pixels1 >> m_greenShift) & 0xff);
                    v00b = area00 * ((pixels1 >> m_blueShift) & 0xff);
                    v10r = area10 * ((pixels2 >> m_redShift) & 0xff);
                    v10g = area10 * ((pixels2 >> m_greenShift) & 0xff);
                    v10b = area10 * ((pixels2 >> m_blueShift) & 0xff);
                    v01r = area01 * ((pixels3 >> m_redShift) & 0xff);
                    v01g = area01 * ((pixels3 >> m_greenShift) & 0xff);
                    v01b = area01 * ((pixels3 >> m_blueShift) & 0xff);
                    v11r = area11 * ((pixels4 >> m_redShift) & 0xff);
                    v11g = area11 * ((pixels4 >> m_greenShift) & 0xff);
                    v11b = area11 * ((pixels4 >> m_blueShift) & 0xff);
                    pixel = (((v00r + v10r + v01r + v11r + 128) << 16) & 0xff000000) | (((v00g + v10g + v01g + v11g + 128) << 8) & 0x00ff0000) | ((v00b + v10b + v01b + v11b + 128) & 0x0000ff00);
                    datad[indexd + j] = pixel;
                }
            }
        }

        /// <summary>
        /// Linearly interpreted (up) scaling.
        /// </summary>
        Pix PixScaleColor4xLI(Pix pixs)
        {
            Pix pixr, pixg, pixb;
            Pix pixrs, pixgs, pixbs;
            Pix pixd;

            if (pixs == null || pixs.D != 32)
            {//return (PIX *)ERROR_PTR("pixs undefined or not 32 bpp", procName, null);
            }

            pixr = PixGetRGBComponent(pixs, m_red);
            pixrs = PixScaleGray4xLI(pixr);
            pixg = PixGetRGBComponent(pixs, m_green);
            pixgs = PixScaleGray4xLI(pixg);
            pixb = PixGetRGBComponent(pixs, m_blue);
            pixbs = PixScaleGray4xLI(pixb);

            if ((pixd = PixCreateRGBImage(pixrs, pixgs, pixbs)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }

            return pixd;
        }

        /// <summary>
        /// Linearly interpreted (up) scaling.
        /// </summary>
        Pix PixScaleColor2xLI(Pix pixs)
        {
            int ws, hs, wpls, wpld;
            uint[] datas, datad;
            Pix pixd;

            if (pixs == null || pixs.D != 32)
            {//return (PIX*)ERROR_PTR("pixs undefined or not 32 bpp", procName, null);
            }

            ws = pixs.W; hs = pixs.H;
            datas = pixs.Data;
            wpls = pixs.Wpl;
            if ((pixd = PixCreate(2 * ws, 2 * hs, 32)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 2.0f, 2.0f);
            datad = pixd.Data;
            wpld = pixd.Wpl;
            ScaleColor2xLILow(ref datad, wpld, datas, ws, hs, wpls);
            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Color interpolated scaling: 2x upscaling.
        /// </summary>
        void ScaleColor2xLILow(ref uint[] datad, int wpld, uint[] datas, int ws, int hs, int wpls)
        {
            int i, hsm;
            int indexs = 0, indexd = 0;
            hsm = hs - 1;

            for (i = 0; i < hsm; i++)
            {
                indexs = i * wpls;
                indexd = 2 * i * wpld;
                ScaleColor2xLILineLow(ref datad, indexd, wpld, datas, indexs, ws, wpls, 0);
            }

            indexs = hsm * wpls;
            indexd = 2 * hsm * wpld;
            ScaleColor2xLILineLow(ref datad, indexd, wpld, datas, indexs, ws, wpls, 1);
        }

        /// <summary>
        /// Color interpolated scaling: 2x upscaling.
        /// </summary>
        void ScaleColor2xLILineLow(ref uint[] datad, int destIndex, int wpld, uint[] datas, int srcIndex, int ws, int wpls, int lastlineflag)
        {
            int j, jd, wsm;
            uint rval1, rval2, rval3, rval4, gval1, gval2, gval3, gval4;
            uint bval1, bval2, bval3, bval4;
            uint pixels1, pixels2, pixels3, pixels4, pixel;
            int linesp, linedp;

            wsm = ws - 1;

            if (lastlineflag == 0)
            {
                linesp = srcIndex + wpls;
                linedp = destIndex + wpld;
                pixels1 = datas[srcIndex];
                pixels3 = datas[linesp];

                rval2 = pixels1 >> 24;
                gval2 = (pixels1 >> 16) & 0xff;
                bval2 = (pixels1 >> 8) & 0xff;
                rval4 = pixels3 >> 24;
                gval4 = (pixels3 >> 16) & 0xff;
                bval4 = (pixels3 >> 8) & 0xff;

                for (j = 0, jd = 0; j < wsm; j++, jd += 2)
                {
                    rval1 = rval2;
                    gval1 = gval2;
                    bval1 = bval2;
                    rval3 = rval4;
                    gval3 = gval4;
                    bval3 = bval4;
                    
                    pixels2 = datas[srcIndex + j + 1];
                    pixels4 = datas[linesp + j + 1];
                    rval2 = pixels2 >> 24;
                    gval2 = (pixels2 >> 16) & 0xff;
                    bval2 = (pixels2 >> 8) & 0xff;
                    rval4 = pixels4 >> 24;
                    gval4 = (pixels4 >> 16) & 0xff;
                    bval4 = (pixels4 >> 8) & 0xff;
                    
                    pixel = (rval1 << 24 | gval1 << 16 | bval1 << 8);
                    datad[destIndex + jd] = pixel;                               /* pix 1 */
                    pixel = ((((rval1 + rval2) << 23) & 0xff000000) |
                             (((gval1 + gval2) << 15) & 0x00ff0000) |
                             (((bval1 + bval2) << 7) & 0x0000ff00));
                    datad[destIndex + jd + 1] = pixel;                           /* pix 2 */
                    pixel = ((((rval1 + rval3) << 23) & 0xff000000) |
                             (((gval1 + gval3) << 15) & 0x00ff0000) |
                             (((bval1 + bval3) << 7) & 0x0000ff00));
                    datad[linedp + jd] = pixel;                              /* pix 3 */
                    pixel = ((((rval1 + rval2 + rval3 + rval4) << 22) & 0xff000000) |
                             (((gval1 + gval2 + gval3 + gval4) << 14) & 0x00ff0000) |
                             (((bval1 + bval2 + bval3 + bval4) << 6) & 0x0000ff00));
                    datad[linedp + jd + 1] = pixel;                          /* pix 4 */
                }
                
                rval1 = rval2;
                gval1 = gval2;
                bval1 = bval2;
                rval3 = rval4;
                gval3 = gval4;
                bval3 = bval4;
                pixel = (rval1 << 24 | gval1 << 16 | bval1 << 8);
                datad[destIndex + 2 * wsm] = pixel;                        /* pix 1 */
                datad[destIndex + 2 * wsm + 1] = pixel;                    /* pix 2 */
                pixel = ((((rval1 + rval3) << 23) & 0xff000000) |
                         (((gval1 + gval3) << 15) & 0x00ff0000) |
                         (((bval1 + bval3) << 7) & 0x0000ff00));
                datad[linedp + 2 * wsm] = pixel;                       /* pix 3 */
                datad[linedp + 2 * wsm + 1] = pixel;                   /* pix 4 */
            }
            else
            {   
                linedp = srcIndex + wpld;
                pixels2 = datas[srcIndex];
                rval2 = pixels2 >> 24;
                gval2 = (pixels2 >> 16) & 0xff;
                bval2 = (pixels2 >> 8) & 0xff;
                for (j = 0, jd = 0; j < wsm; j++, jd += 2)
                {
                    rval1 = rval2;
                    gval1 = gval2;
                    bval1 = bval2;
                    pixels2 = datas[srcIndex + j + 1];
                    rval2 = pixels2 >> 24;
                    gval2 = (pixels2 >> 16) & 0xff;
                    bval2 = (pixels2 >> 8) & 0xff;
                    pixel = (rval1 << 24 | gval1 << 16 | bval1 << 8);
                    datad[destIndex + jd] = pixel;                            /* pix 1 */
                    datad[linedp + j] = pixel;                           /* pix 2 */
                    pixel = ((((rval1 + rval2) << 23) & 0xff000000) | (((gval1 + gval2) << 15) & 0x00ff0000) | (((bval1 + bval2) << 7) & 0x0000ff00));
                    datad[destIndex + jd + 1] = pixel;                        /* pix 3 */
                    datad[linedp + jd + 1] = pixel;                       /* pix 4 */
                }
                rval1 = rval2;
                gval1 = gval2;
                bval1 = bval2;
                pixel = (rval1 << 24 | gval1 << 16 | bval1 << 8);
                datad[destIndex + 2 * wsm] = pixel;                           /* pix 1 */
                datad[destIndex + 2 * wsm + 1] = pixel;                       /* pix 2 */
                datad[linedp + 2 * wsm] = pixel;                          /* pix 3 */
                datad[linedp + 2 * wsm + 1] = pixel;                      /* pix 4 */
            }
        }

        /// <summary>
        /// Linearly interpreted (up) scaling.
        /// </summary>
        Pix PixScaleGrayLI(Pix pixs, float scalex, float scaley)
        {
            int ws, hs, wpls, wd, hd, wpld;
            uint[] datas, datad;
            float maxscale;
            Pix pixd;

            if (pixs == null || (pixs.D != 8))
            {//return (PIX*)ERROR_PTR("pixs undefined or not 8 bpp", procName, null);
            }
            maxscale = Math.Max(scalex, scaley);
            if (maxscale < 0.7)
                return PixScale(pixs, scalex, scaley);
            if (pixs.Colormap == null)
            { }
            
            if (scalex == 1.0 && scaley == 1.0)
                return PixCopy(null, pixs);
            if (scalex == 2.0 && scaley == 2.0)
                return PixScaleGray2xLI(pixs);
            if (scalex == 4.0 && scaley == 4.0)
                return PixScaleGray4xLI(pixs);

            ws = pixs.W; hs = pixs.H;
            datas = pixs.Data;
            wpls = pixs.Wpl;
            wd = (int)(scalex * (float)ws + 0.5);
            hd = (int)(scaley * (float)hs + 0.5);
            if ((pixd = PixCreate(wd, hd, 8)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, scalex, scaley);
            datad = pixd.Data;
            wpld = pixd.Wpl;
            ScaleGrayLILow(ref datad, wd, hd, wpld, datas, ws, hs, wpls);
            pixd.Data = datad;
            return pixd;
        }

        Pix PixScaleGray4xLI(Pix pixs)
        {
            int ws, hs, wpls, wpld;
            uint[] datas, datad;
            Pix pixd;

            if (pixs == null || pixs.D != 8)
            {//return (PIX*)ERROR_PTR("pixs undefined or not 8 bpp", procName, null);
            }
            if (pixs.Colormap == null)
            { }

            ws = pixs.W; hs = pixs.H;
            datas = pixs.Data;
            wpls = pixs.Wpl;
            if ((pixd = PixCreate(4 * ws, 4 * hs, 8)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 4.0f, 4.0f);
            datad = pixd.Data;
            wpld = pixd.Wpl;
            ScaleGray4xLILow(ref datad, wpld, datas, ws, hs, wpls);
            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Grayscale interpolated scaling 4x upscaling.
        /// </summary>
        void ScaleGray4xLILow(ref uint[] datad, int wpld, uint[] datas, int ws, int hs, int wpls)
        {
            int i, hsm;
            int indexs = 0, indexd = 0;

            hsm = hs - 1;

            for (i = 0; i < hsm; i++)
            {
                indexs = i * wpls;
                indexd = 4 * i * wpld;
                ScaleGray4xLILineLow(ref datad, indexd, wpld, datas, indexs, ws, wpls, 0);
            }

            /* last src line */
            indexs = hsm * wpls;
            indexd = 4 * hsm * wpld;
            ScaleGray4xLILineLow(ref datad, indexd, wpld, datas, indexs, ws, wpls, 1);
        }

        /// <summary>
        /// Grayscale interpolated scaling.
        /// </summary>
        void ScaleGrayLILow(ref uint[] datad, int wd, int hd, int wpld, uint[] datas, int ws, int hs, int wpls)
        {
            int i, j, wm2, hm2;
            uint xpm, ypm;
            uint xp, yp, xf, yf;
            uint v00, v01, v10, v11, v00_val, v01_val, v10_val, v11_val;
            uint val;
            float scx, scy;
            int indexs = 0, indexd = 0;

            scx = 16 * (float)ws / (float)wd;
            scy = 16 * (float)hs / (float)hd;

            wm2 = ws - 2;
            hm2 = hs - 2;

            for (i = 0; i < hd; i++)
            {
                ypm = (uint)(scy * (float)i);
                yp = ypm >> 4;
                yf = ypm & 0x0f;
                indexd = i * wpld;
                indexs = (int)yp * wpls;
                for (j = 0; j < wd; j++)
                {
                    xpm = (uint)(scx * (float)j);
                    xp = xpm >> 4;
                    xf = xpm & 0x0f;

                    v00_val = JBIG2Statics.GetDataByte(datas, indexs + (int)xp);
                    if (xp > wm2 || yp > hm2)
                    {
                        if (yp > hm2 && xp <= wm2)
                        {
                            v01_val = v00_val;
                            v10_val = JBIG2Statics.GetDataByte(datas, indexs + (int)xp + 1);
                            v11_val = v10_val;
                        }
                        else if (xp > wm2 && yp <= hm2)
                        {
                            v01_val = JBIG2Statics.GetDataByte(datas, indexs + wpls + (int)xp);
                            v10_val = v00_val;
                            v11_val = v01_val;
                        }
                        else
                        {
                            v10_val = v01_val = v11_val = v00_val;
                        }
                    }
                    else
                    {
                        v10_val = JBIG2Statics.GetDataByte(datas, indexs + (int)xp + 1);
                        v01_val = JBIG2Statics.GetDataByte(datas, indexs + wpls + (int)xp);
                        v11_val = JBIG2Statics.GetDataByte(datas, indexs + wpls + (int)xp + 1);
                    }

                    v00 = (16 - xf) * (16 - yf) * v00_val;
                    v10 = xf * (16 - yf) * v10_val;
                    v01 = (16 - xf) * yf * v01_val;
                    v11 = xf * yf * v11_val;

                    val = ((v00 + v01 + v10 + v11 + 128) / 256);
                    JBIG2Statics.SetDataByte(ref datad, j, val);
                }
            }
        }

        /// <summary>
        /// Linearly interpreted (up) scaling.
        /// </summary>
        private Pix PixScaleGray2xLI(Pix pixs)
        {
            int ws, hs, wpls, wpld;
            uint[] datas, datad;
            Pix pixd;

            if (pixs == null || (pixs.D != 8))
            {//return (PIX *)ERROR_PTR("pixs undefined or not 8 bpp", procName, null);
            }
            if (pixs.Colormap == null)
            { }

            ws = pixs.W; hs = pixs.H;
            datas = pixs.Data;
            wpls = pixs.Wpl;
            if ((pixd = PixCreate(2 * ws, 2 * hs, 8)) == null)
            {//return (PIX *)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 2.0f, 2.0f);
            datad = pixd.Data;
            wpld = pixd.Wpl;
            ScaleGray2xLILow(ref datad, wpld, datas, ws, hs, wpls);
            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Grayscale interpolated scaling: 2x upscaling.
        /// </summary>
        void ScaleGray2xLILow(ref uint[] datad, int wpld, uint[] datas, int ws, int hs, int wpls)
        {
            int i, hsm;
            int indexs = 0, indexd = 0;

            hsm = hs - 1;

            for (i = 0; i < hsm; i++)
            {
                indexs = i * wpls;
                indexd = 2 * i * wpld;
                ScaleGray2xLILineLow(ref datad, indexd, wpld, datas, indexs, ws, wpls, 0);
            }

            indexs = hsm * wpls;
            indexd = 2 * hsm * wpld;
            ScaleGray2xLILineLow(ref datad, indexd, wpld, datas, indexs, ws, wpls, 1);
        }

        /// <summary>
        /// Unsharp masking.
        /// </summary>
        Pix PixUnsharpMasking(Pix pixs, int halfwidth, float fract)
        {
            int d;
            Pix pixt, pixd, pixr, pixrs, pixg, pixgs, pixb, pixbs;

            if (pixs == null || pixs.D == 1)
            {//return (PIX*)ERROR_PTR("pixs not defined or 1 bpp", procName, null);
            }
            if (fract <= 0.0 || halfwidth <= 0)
            {
                //L_WARNING("no sharpening requested; clone returned", procName);
                return pixs;
            }

            if (halfwidth == 1 || halfwidth == 2)
                return PixUnsharpMaskingFast(pixs, halfwidth, fract, 3);

            if ((pixt = PixConvertTo8Or32(pixs, 0, 1)) == null)
            {//return (PIX*)ERROR_PTR("pixt not made", procName, null);
            }

            d = pixt.D;
            if (d == 8)
                pixd = PixUnsharpMaskingGray(pixt, halfwidth, fract);
            else
            {
                pixr = PixGetRGBComponent(pixs, m_red);
                pixrs = PixUnsharpMaskingGray(pixr, halfwidth, fract);
                pixg = PixGetRGBComponent(pixs, m_green);
                pixgs = PixUnsharpMaskingGray(pixg, halfwidth, fract);
                pixb = PixGetRGBComponent(pixs, m_blue);
                pixbs = PixUnsharpMaskingGray(pixb, halfwidth, fract);
                pixd = PixCreateRGBImage(pixrs, pixgs, pixbs);
            }

            return pixd;
        }

        Pix PixUnsharpMaskingFast(Pix pixs, int halfwidth, float fract, int direction)
        {
            int d;
            Pix pixt, pixd, pixr, pixrs, pixg, pixgs, pixb, pixbs;

            if (pixs == null || pixs.D == 1)
            {//return (PIX*)ERROR_PTR("pixs not defined or 1 bpp", procName, null);
            }
            if (fract <= 0.0 || halfwidth <= 0)
                return pixs;
            if (halfwidth != 1 && halfwidth != 2)
            {//return (PIX*)ERROR_PTR("halfwidth must be 1 or 2", procName, null);
            }
            if (direction != 1 && direction != 2 && direction != 3)
            {//return (PIX*)ERROR_PTR("invalid direction", procName, null);
            }

            if ((pixt = PixConvertTo8Or32(pixs, 0, 1)) == null)
            {//return (PIX*)ERROR_PTR("pixt not made", procName, null);
            }

            d = pixt.D;
            if (d == 8)
                pixd = PixUnsharpMaskingGrayFast(pixt, halfwidth, fract, direction);
            else
            {
                pixr = PixGetRGBComponent(pixs, m_red);
                pixrs = PixUnsharpMaskingGrayFast(pixr, halfwidth, fract, direction);
                pixg = PixGetRGBComponent(pixs, m_green);
                pixgs = PixUnsharpMaskingGrayFast(pixg, halfwidth, fract, direction);
                pixb = PixGetRGBComponent(pixs, m_blue);
                pixbs = PixUnsharpMaskingGrayFast(pixb, halfwidth, fract, direction);
                pixd = PixCreateRGBImage(pixrs, pixgs, pixbs);
            }

            return pixd;
        }

        Pix PixCreateRGBImage(Pix pixr, Pix pixg, Pix pixb)
        {
            int wr, wg, wb, hr, hg, hb, dr, dg, db;
            Pix pixd;

            wr = pixr.W; hr = pixr.H; dr = pixr.D;
            wg = pixg.W; hg = pixg.H; dg = pixg.D;
            wb = pixb.W; hb = pixb.H; db = pixb.D;

            if (dr != 8 || dg != 8 || db != 8)
            {//return (PIX*)ERROR_PTR("input pix not all 8 bpp", procName, null);
            }
            if (wr != wg || wr != wb)
            {//return (PIX*)ERROR_PTR("widths not the same", procName, null);
            }
            if (hr != hg || hr != hb)
            {//return (PIX*)ERROR_PTR("heights not the same", procName, null);
            }

            if ((pixd = PixCreate(wr, hr, 32)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixr);
            PixSetRGBComponent(pixd, pixr, m_red);
            PixSetRGBComponent(pixd, pixg, m_green);
            PixSetRGBComponent(pixd, pixb, m_blue);

            return pixd;
        }

        void PixSetRGBComponent(Pix pixd, Pix pixs, int color)
        {
            uint srcbyte;
            int i, j, w, h;
            int wpls, wpld;
            int indexs = 0, indexd = 0;
            uint[] datas, datad;

            if (pixd.D != 32)
            {//return ERROR_INT("pixd not 32 bpp", procName, 1);
            }
            if (pixs.D != 8)
            {//return ERROR_INT("pixs not 8 bpp", procName, 1);
            }
            if (color != m_red && color != m_green && color != m_blue && color != m_alpha)
            {//return ERROR_INT("invalid color", procName, 1);
            }
            w = pixs.W; h = pixs.H;
            if (w != pixd.W || h != pixd.H)
            {//return ERROR_INT("sizes not commensurate", procName, 1);
            }

            datas = pixs.Data;
            datad = pixd.Data;
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;
            for (i = 0; i < h; i++)
            {
                indexs = i * wpls;
                indexd = i * wpld;
                for (j = 0; j < w; j++)
                {
                    srcbyte = JBIG2Statics.GetDataByte(datas, indexs + j);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + color, srcbyte);
                }
            }
        }

        private Pix PixGetRGBComponent(Pix pixs, int color)
        {
            uint srcbyte;
            int indexs = 0, indexd = 0;
            uint[] datas, datad;
            int i, j, w, h;
            int wpls, wpld;
            Pix pixd = null;

            if (pixs.D != 32)
            {//return (PIX *)ERROR_PTR("pixs not 32 bpp", procName, null);
            }
            if (color != m_red && color != m_green && color != m_blue && color != m_alpha)
            {//return (PIX*)ERROR_PTR("invalid color", procName, null);
            }

            w = pixs.W; h = pixs.H;
            if ((pixd = PixCreate(w, h, 8)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;
            datas = pixs.Data;
            datad = pixd.Data;

            for (i = 0; i < h; i++)
            {
                indexs = i * wpls;
                indexd = i * wpld;
                for (j = 0; j < w; j++)
                {
                    srcbyte = JBIG2Statics.GetDataByte(datas, indexs + j + color);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j, srcbyte);
                }
            }

            pixd.Data = datad;
            return pixd;
        }

        Pix PixUnsharpMaskingGray(Pix pixs, int halfwidth, float fract)
        {
            int w, h, d;
            Pix pixc, pixd;
            Pixacc pixacc;

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (d != 8 || pixs.Colormap != null)
            {//return (PIX*)ERROR_PTR("pixs not 8 bpp or has cmap", procName, null);
            }
            if (fract <= 0.0 || halfwidth <= 0)
                return pixs;
            if (halfwidth == 1 || halfwidth == 2)
                return PixUnsharpMaskingGrayFast(pixs, halfwidth, fract, 3);

            if ((pixc = PixBlockconvGray(pixs, null, halfwidth, halfwidth)) == null)
            {//return (PIX*)ERROR_PTR("pixc not made", procName, null);
            }

            pixacc = PixaccCreate(w, h, 1);
            PixaccAdd(pixacc, pixs);
            PixaccSubtract(pixacc, pixc);
            PixaccMultConst(pixacc, fract);
            PixaccAdd(pixacc, pixs);
            pixd = PixaccFinal(pixacc, 8);
            return pixd;
        }

        private Pix PixaccFinal(Pixacc pixacc, int outdepth)
        {
            return PixFinalAccumulate(pixacc.Pix, pixacc.Offset, outdepth);
        }

        private Pix PixFinalAccumulate(Pix pixs, uint offset, int depth)
        {
            int w, h, wpls, wpld;
            uint[] datas, datad;
            Pix pixd;

            if (pixs.D != 32)
            {//return (PIX *)ERROR_PTR("pixs not 32 bpp", procName, null);
            }
            if (depth != 8 && depth != 16 && depth != 32)
            {//return (PIX *)ERROR_PTR("dest depth not 8, 16, 32 bpp", procName, null);
            }
            if (offset > 0x40000000)
                offset = 0x40000000;

            w = pixs.W; h = pixs.H;
            if ((pixd = PixCreate(w, h, depth)) == null)
            {//return (PIX *)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);  /* but how did pixs get it initially? */
            datas = pixs.Data;
            datad = pixd.Data;
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;

            FinalAccumulateLow(ref datad, w, h, depth, wpld, datas, wpls, offset);
            return pixd;
        }

        void FinalAccumulateLow(ref uint[] datad, int w, int h, int d, int wpld, uint[] datas, int wpls, uint offset)
        {
            int i, j;
            uint val;
            int indexs = 0, indexd = 0;

            switch (d)
            {
                case 8:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = i * wpld;
                        for (j = 0; j < w; j++)
                        {
                            val = datas[indexs + j] - offset;
                            val = Math.Max(0, val);
                            val = Math.Min(255, val);
                            JBIG2Statics.SetDataByte(ref datad, indexd + j, val);
                        }
                    }
                    break;
                case 16:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = i * wpld;
                        for (j = 0; j < w; j++)
                        {
                            val = datas[indexs + j] - offset;
                            val = Math.Max(0, val);
                            val = Math.Min(0xffff, val);
                            JBIG2Statics.SetDataTwoBytes(ref datad, indexd + j, (short)val);
                        }
                    }
                    break;
                case 32:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = i * wpld;
                        for (j = 0; j < w; j++)
                            datad[indexd + j] = datas[indexs + j] - (uint)offset;
                    }
                    break;
            }
        }

        void PixaccMultConst(Pixacc pixacc, float factor)
        {
            PixMultConstAccumulate(pixacc.Pix, factor, pixacc.Offset);
        }

        void PixMultConstAccumulate(Pix pixs, float factor, uint offset)
        {
            int w, h, wpl;
            uint[] data;

            if (pixs.D != 32)
            {//return ERROR_INT("pixs not 32 bpp", procName, 1);
            }
            if (offset > 0x40000000)
                offset = 0x40000000;

            w = pixs.W; h = pixs.H;
            data = pixs.Data;
            wpl = pixs.Wpl;

            MultConstAccumulateLow(data, w, h, wpl, factor, offset);
        }

        void MultConstAccumulateLow(uint[] data, int w, int h, int wpl, float factor, uint offset)
        {
            int i, j;
            uint val;
            int index;

            for (i = 0; i < h; i++)
            {
                index = i * wpl;
                for (j = 0; j < w; j++)
                {
                    val = data[index + j] - offset;
                    val = (uint)(val * factor);
                    val += offset;
                    data[index + j] = val;
                }
            }
        }

        void PixaccSubtract(Pixacc pixacc, Pix pix)
        {
            PixAccumulate(pixacc.Pix, pix, 2);
        }

        void PixaccAdd(Pixacc pixacc, Pix pix)
        {
            PixAccumulate(pixacc.Pix, pix, 1);
        }

        void PixAccumulate(Pix pixd, Pix pixs, int op)
        {
            int w, h, d, wd, hd, wpls, wpld;
            uint[] datas, datad;

            if (pixd == null || pixd.D != 32)
            {//return ERROR_INT("pixd not defined or not 32 bpp", procName, 1);
            }
            d = pixs.D;
            if (d != 1 && d != 8 && d != 16 && d != 32)
            {//return ERROR_INT("pixs not 1, 8, 16 or 32 bpp", procName, 1);
            }
            if (op != 1 && op != 2)
            {//return ERROR_INT("op must be in {L_ARITH_ADD, L_ARITH_SUBTRACT}", procName, 1);
            }

            datas = pixs.Data;
            datad = pixd.Data;
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;
            w = pixs.W; h = pixs.H;
            wd = pixd.W; hd = pixd.H;
            w = Math.Min(w, wd);
            h = Math.Min(h, hd);

            AccumulateLow(ref datad, w, h, wpld, datas, d, wpls, op);
            pixd.Data = datad;
        }

        void AccumulateLow(ref uint[] datad, int w, int h, int wpld, uint[] datas, int d, int wpls, int op)
        {
            int i, j;
            int indexs = 0, indexd = 0;

            switch (d)
            {
                case 1:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = i * wpld;
                        if (op == 1)
                        {
                            for (j = 0; j < w; j++)
                                datad[indexd + j] += JBIG2Statics.GetDataBit(datas, indexd, j);
                        }
                        else
                        {
                            for (j = 0; j < w; j++)
                                datad[indexd + j] -= JBIG2Statics.GetDataBit(datas, indexd, j);
                        }
                    }
                    break;
                case 8:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = i * wpld;
                        if (op == 1)
                        {
                            for (j = 0; j < w; j++)
                                datad[indexd + j] += JBIG2Statics.GetDataByte(datas, indexs + j);
                        }
                        else
                        {
                            for (j = 0; j < w; j++)
                                datad[indexd + j] -= JBIG2Statics.GetDataByte(datas, indexs + j);
                        }
                    }
                    break;
                case 16:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = i * wpld;
                        if (op == 1)
                        {
                            for (j = 0; j < w; j++)
                                datad[indexd + j] += (uint)JBIG2Statics.GetDataTwoBytes(datas, indexs + j);
                        }
                        else
                        {
                            for (j = 0; j < w; j++)
                                datad[indexd + j] -= (uint)JBIG2Statics.GetDataTwoBytes(datas, indexs + j);
                        }
                    }
                    break;
                case 32:
                    for (i = 0; i < h; i++)
                    {
                        indexs = i * wpls;
                        indexd = i * wpld;
                        if (op == 1)
                        {
                            for (j = 0; j < w; j++)
                                datad[indexd + j] += datas[indexs + j];
                        }
                        else
                        {
                            for (j = 0; j < w; j++)
                                datad[indexd + j] -= datas[indexs + j];
                        }
                    }
                    break;
            }
        }

        Pixacc PixaccCreate(int w, int h, int negflag)
        {
            Pixacc pixacc = new Pixacc();
            pixacc.W = w;
            pixacc.H = h;

            if ((pixacc.Pix = PixCreate(w, h, 32)) == null)
            {//return (PIXACC*)ERROR_PTR("pix not made", procName, null);
            }

            if (negflag > 0)
            {
                pixacc.Offset = 0x40000000;
                PixSetAllArbitrary(pixacc.Pix, pixacc.Offset);
            }

            return pixacc;
        }

        /// <summary>
        /// Full image set to arbitrary value
        /// </summary>
        void PixSetAllArbitrary(Pix pix, uint val)
        {
            int n, i, j, w, h, d, wpl, npix, index;
            uint maxval, wordval;
            PixColormap cmap;

            if ((cmap = pix.Colormap) != null)
            {
                n = cmap.N;
                if (val < 0)
                    val = 0;
                else if (val >= n)
                    val = (uint)n - 1;
            }

            w = pix.W; h = pix.H; d = pix.D;
            if (d == 32)
                maxval = 0xffffffff;
            else
                maxval = (uint)(1 << d) - 1;
            if (val < 0)
                val = 0;
            if (val > maxval)
                val = maxval;

            wordval = 0;
            npix = 32 / d;
            for (j = 0; j < npix; j++)
                wordval |= (val << (j * d));

            wpl = pix.Wpl;

            for (i = 0; i < h; i++)
            {
                index = i * wpl;
                for (j = 0; j < wpl; j++)
                    pix.Data[index + j] = wordval;
            }
        }

        /// <summary>
        /// Grayscale block convolution
        /// </summary>
        private Pix PixBlockconvGray(Pix pixs, Pix pixacc, int wc, int hc)
        {
            int w, h, d, wpl, wpla;
            uint[] datad, dataa;
            Pix pixd, pixt;

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (d != 8)
            {//return (PIX *)ERROR_PTR("pixs not 8 bpp", procName, null);
            }
            if (wc < 0) wc = 0;
            if (hc < 0) hc = 0;
            if (w < 2 * wc + 1 || h < 2 * hc + 1)
            {
                wc = Math.Min(wc, (w - 1) / 2);
                hc = Math.Min(hc, (h - 1) / 2);
                //L_WARNING("kernel too large; reducing!", procName);
                //L_INFO_INT2("wc = %d, hc = %d", procName, wc, hc);
            }
            if (wc == 0 && hc == 0)   /* no-op */
                return PixCopy(null, pixs);

            if (pixacc != null)
            {
                if (pixacc.D == 32)
                    pixt = pixacc;
                else
                {
                    //L_WARNING("pixacc not 32 bpp; making new one", procName);
                    if ((pixt = PixBlockconvAccum(pixs)) == null)
                    {//return (PIX *)ERROR_PTR("pixt not made", procName, null);
                    }
                }
            }
            else
            {
                if ((pixt = PixBlockconvAccum(pixs)) == null)
                {//return (PIX *)ERROR_PTR("pixt not made", procName, null);
                }
            }

            if ((pixd = PixCreateTemplate(pixs)) == null)
            {        //return (PIX *)ERROR_PTR("pixd not made", procName, null);
            }

            wpl = pixs.Wpl;
            wpla = pixt.Wpl;
            datad = pixd.Data;
            dataa = pixt.Data;
            BlockconvLow(ref datad, w, h, wpl, dataa, wpla, wc, hc);
            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Grayscale Block Convolution
        /// </summary>
        void BlockconvLow(ref uint[] data, int w, int h, int wpl, uint[] dataa, int wpla, int wc, int hc)
        {
            int i, j, imax, imin, jmax, jmin;
            int wn, hn, fwc, fhc, wmwc, hmhc;
            float norm, normh, normw;
            uint val;
            int linemina, linemaxa, index;

            wmwc = w - wc;
            hmhc = h - hc;
            if (wmwc <= 0 || hmhc <= 0)
                return;
            fwc = 2 * wc + 1;
            fhc = 2 * hc + 1;
            norm = 1 / (fwc * fhc);

            for (i = 0; i < h; i++)
            {
                imin = Math.Max(i - 1 - hc, 0);
                imax = Math.Min(i + hc, h - 1);
                index = wpl * i;
                linemina = wpla * imin;
                linemaxa = wpla * imax;
                for (j = 0; j < w; j++)
                {
                    jmin = Math.Max(j - 1 - wc, 0);
                    jmax = Math.Min(j + wc, w - 1);
                    val = dataa[linemaxa + jmax] - dataa[linemaxa + jmin] + dataa[linemina + jmin] - dataa[linemina + jmax];
                    val = (uint)(norm * val + 0.5f);
                    JBIG2Statics.SetDataByte(ref data, index + j, val);
                }
            }

            for (i = 0; i <= hc; i++)
            { 
                hn = hc + i;
                normh = (float)fhc / (float)hn;   /* > 1 */
                index = wpl * i;
                for (j = 0; j <= wc; j++)
                {
                    wn = wc + j;
                    normw = (float)fwc / (float)wn;   /* > 1 */
                    val = JBIG2Statics.GetDataByte(data, index + j);
                    val = (uint)Math.Min(val * normh * normw, 255);
                    JBIG2Statics.SetDataByte(ref data, index + j, val);
                }
                for (j = wc + 1; j < wmwc; j++)
                {
                    val = JBIG2Statics.GetDataByte(data, index + j);
                    val = (uint)Math.Min(val * normh, 255);
                    JBIG2Statics.SetDataByte(ref data, index + j, val);
                }
                for (j = wmwc; j < w; j++)
                {
                    wn = wc + w - j;
                    normw = (float)fwc / (float)wn;   /* > 1 */
                    val = JBIG2Statics.GetDataByte(data, index + j);
                    val = (uint)Math.Min(val * normh * normw, 255);
                    JBIG2Statics.SetDataByte(ref data, index + j, val);
                }
            }

            for (i = hmhc; i < h; i++)
            {
                hn = hc + h - i;
                normh = (float)fhc / (float)hn;   /* > 1 */
                index = wpl * i;
                for (j = 0; j <= wc; j++)
                {
                    wn = wc + j;
                    normw = (float)fwc / (float)wn;   /* > 1 */
                    val = JBIG2Statics.GetDataByte(data, index + j);
                    val = (uint)Math.Min(val * normh * normw, 255);
                    JBIG2Statics.SetDataByte(ref data, index + j, val);
                }
                for (j = wc + 1; j < wmwc; j++)
                {
                    val = JBIG2Statics.GetDataByte(data, index + j);
                    val = (uint)Math.Min(val * normh, 255);
                    JBIG2Statics.SetDataByte(ref data, index + j, val);
                }
                for (j = wmwc; j < w; j++)
                {
                    wn = wc + w - j;
                    normw = (float)fwc / (float)wn;   /* > 1 */
                    val = JBIG2Statics.GetDataByte(data, index + j);
                    val = (uint)Math.Min(val * normh * normw, 255);
                    JBIG2Statics.SetDataByte(ref data, index + j, val);
                }
            }

            for (i = hc + 1; i < hmhc; i++)
            {
                index = wpl * i;
                for (j = 0; j <= wc; j++)
                { 
                    wn = wc + j;
                    normw = (float)fwc / (float)wn;   /* > 1 */
                    val = JBIG2Statics.GetDataByte(data, index + j);
                    val = (uint)Math.Min(val * normw, 255);
                    JBIG2Statics.SetDataByte(ref data, index + j, val);
                }
                for (j = wmwc; j < w; j++)
                { 
                    wn = wc + w - j;
                    normw = (float)fwc / (float)wn;   /* > 1 */
                    val = JBIG2Statics.GetDataByte(data, index + j);
                    val = (uint)Math.Min(val * normw, 255);
                    JBIG2Statics.SetDataByte(ref data, index + j, val);
                }
            }
        }

        /// <summary>
        /// Accumulator for 1, 8 and 32 bpp convolution
        /// </summary>
        private Pix PixBlockconvAccum(Pix pixs)
        {
            int w, h, d, wpls, wpld;
            uint[] datas, datad;
            Pix pixd;

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (d != 1 && d != 8 && d != 32)
            {//return (PIX *)ERROR_PTR("pixs not 1, 8 or 32 bpp", procName, null);
            }
            if ((pixd = PixCreate(w, h, 32)) == null)
            {//return (PIX *)ERROR_PTR("pixd not made", procName, null);
            }
            datas = pixs.Data;
            datad = pixd.Data;
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;
            BlockconvAccumLow(ref datad, w, h, wpld, datas, d, wpls);
            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Grayscale block convolution.
        /// </summary>
        void BlockconvAccumLow(ref uint[] datad, int w, int h, int wpld, uint[] datas, int d, int wpls)
        {
            uint val, val32;
            int i, j;
            int indexs = 0, indexd = 0, indexdp = 0;

            if (d == 1)
            {
                for (j = 0; j < w; j++)
                {
                    val = JBIG2Statics.GetDataBit(datas, indexs, j);
                    if (j == 0)
                        datad[0] = val;
                    else
                        datad[j] = datad[j - 1] + val;
                }

                for (i = 1; i < h; i++)
                {
                    indexs = i * wpls;
                    indexd = i * wpld;
                    indexdp = indexd - wpld;
                    for (j = 0; j < w; j++)
                    {
                        val = JBIG2Statics.GetDataBit(datas, indexs, j);
                        if (j == 0)
                            datad[indexd + 0] = val + datad[indexdp + 0];
                        else
                            datad[indexd + j] = val + datad[indexd + j - 1] + datad[indexdp + j] - datad[indexdp + j - 1];
                    }
                }
            }
            else if (d == 8)
            {
                for (j = 0; j < w; j++)
                {
                    val = JBIG2Statics.GetDataByte(datas, indexs + j);
                    if (j == 0)
                        datad[0] = val;
                    else
                        datad[j] = datad[j - 1] + val;
                }

                for (i = 1; i < h; i++)
                {
                    indexs = i * wpls;
                    indexd = i * wpld;
                    indexdp = indexd - wpld;
                    for (j = 0; j < w; j++)
                    {
                        val = JBIG2Statics.GetDataByte(datas, indexs + j);
                        if (j == 0)
                            datad[indexd + 0] = val + datad[indexdp + 0];
                        else
                            datad[indexd + j] = val + datad[indexd + j - 1] + datad[indexdp + j] - datad[indexdp + j - 1];
                    }
                }
            }
            else if (d == 32)
            {
                for (j = 0; j < w; j++)
                {
                    val32 = datas[j];
                    if (j == 0)
                        datad[0] = val32;
                    else
                        datad[j] = datad[j - 1] + val32;
                }

                for (i = 1; i < h; i++)
                {
                    indexs = i * wpls;
                    indexd = i * wpld;
                    indexdp = indexd - wpld;
                    for (j = 0; j < w; j++)
                    {
                        val32 = datas[indexs + j];
                        if (j == 0)
                            datad[indexd + 0] = val32 + datad[indexdp + 0];
                        else
                            datad[indexd + j] = val32 + datad[indexd + j - 1] + datad[indexdp + j] - datad[indexdp + j - 1];
                    }
                }
            }
            //else
            //{//L_ERROR("depth not 1, 8 or 32 bpp", procName);
            //}
        }

        /// <summary>
        /// Unsharp masking.
        /// </summary>
        Pix PixUnsharpMaskingGrayFast(Pix pixs, int halfwidth, float fract, int direction)
        {
            Pix pixd;

            if (pixs.D != 8 || pixs.Colormap != null)
            {//return (PIX*)ERROR_PTR("pixs not 8 bpp or has cmap", procName, null);
            }
            if (fract <= 0.0 || halfwidth <= 0)
                return pixs;
            if (halfwidth != 1 && halfwidth != 2)
            {//return (PIX*)ERROR_PTR("halfwidth must be 1 or 2", procName, null);
            }
            if (direction != 1 && direction != 2 && direction != 3)
            {//return (PIX*)ERROR_PTR("invalid direction", procName, null);
            }

            if (direction != 3)
                pixd = PixUnsharpMaskingGray1D(pixs, halfwidth, fract, direction);
            else 
                pixd = PixUnsharpMaskingGray2D(pixs, halfwidth, fract);

            return pixd;
        }

        /// <summary>
        /// Unsharp masking.
        /// </summary>
        Pix PixUnsharpMaskingGray2D(Pix pixs, int halfwidth, float fract)
        {
            int w, h, d, wpls, wpld, wplf, i, j;
            uint ival, sval;
            uint[] datas, datad;
            float[] dataf;
            int indexs, indexs0, indexs1, indexs2, indexd;
            uint val;
            float[] a = new float[9];
            int indexf, indexf0, indexf1, indexf2, indexf3, indexf4;
            Pix pixd = null;
            FPix fpix;

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (d != 8 || pixs.Colormap != null)
            {//return (PIX *)ERROR_PTR("pixs not 8 bpp or has cmap", procName, null);
            }
            if (fract <= 0.0 || halfwidth <= 0)
                return pixs;
            if (halfwidth != 1 && halfwidth != 2)
            {//return (PIX *)ERROR_PTR("halfwidth must be 1 or 2", procName, null);
            }

            pixd = PixCopyBorder(null, pixs, halfwidth, halfwidth, halfwidth, halfwidth);
            datad = pixd.Data;
            wpld = pixd.Wpl;
            datas = pixs.Data;
            wpls = pixs.Wpl;

            if (halfwidth == 1)
            {
                for (i = 0; i < 9; i++)
                    a[i] = -fract / 9.0f;
                a[4] = (float)1.0 + fract * 8.0f / 9.0f;
                for (i = 1; i < h - 1; i++)
                {
                    indexs0 = (i - 1) * wpls;
                    indexs1 = i * wpls;
                    indexs2 = (i + 1) * wpls;
                    indexd = i * wpld;
                    for (j = 1; j < w - 1; j++)
                    {
                        val = (uint)(a[0] * JBIG2Statics.GetDataByte(datas, indexs0 + j - 1) +
                              a[1] * JBIG2Statics.GetDataByte(datas, indexs0 + j) +
                              a[2] * JBIG2Statics.GetDataByte(datas, indexs0 + j + 1) +
                              a[3] * JBIG2Statics.GetDataByte(datas, indexs1 + j - 1) +
                              a[4] * JBIG2Statics.GetDataByte(datas, indexs1 + j) +
                              a[5] * JBIG2Statics.GetDataByte(datas, indexs1 + j + 1) +
                              a[6] * JBIG2Statics.GetDataByte(datas, indexs2 + j - 1) +
                              a[7] * JBIG2Statics.GetDataByte(datas, indexs2 + j) +
                              a[8] * JBIG2Statics.GetDataByte(datas, indexs2 + j + 1));
                        ival = (uint)(val + 0.5);
                        ival = Math.Max(0, ival);
                        ival = Math.Min(255, ival);
                        JBIG2Statics.SetDataByte(ref datad, indexd + j, ival);
                    }
                }

                return pixd;
            }

            fpix = FpixCreate(w, h);
            dataf = fpix.Data;
            wplf = fpix.Wpl;
            for (i = 2; i < h - 2; i++)
            {
                indexs = i * wpls;
                indexf = i * wplf;
                for (j = 2; j < w - 2; j++)
                {
                    val = JBIG2Statics.GetDataByte(datas, indexs + j - 2) +
                          JBIG2Statics.GetDataByte(datas, indexs + j - 1) +
                          JBIG2Statics.GetDataByte(datas, indexs + j) +
                          JBIG2Statics.GetDataByte(datas, indexs + j + 1) +
                          JBIG2Statics.GetDataByte(datas, indexs + j + 2);
                    dataf[indexf + j] = val;
                }
            }

            for (i = 2; i < h - 2; i++)
            {
                indexf0 = (i - 2) * wplf;
                indexf1 = (i - 1) * wplf;
                indexf2 = i * wplf;
                indexf3 = (i + 1) * wplf;
                indexf4 = (i + 2) * wplf;
                indexd = i * wpld;
                indexs = i * wpls;
                for (j = 2; j < w - 2; j++)
                {
                    val = (uint)(0.04f * (dataf[indexf0 + j] + dataf[indexf1 + j] + dataf[indexf2 + j] +
                                  dataf[indexf3 + j] + dataf[indexf4 + j])); 
                    sval = JBIG2Statics.GetDataByte(datas, indexs + j); 
                    ival = (uint)(sval + fract * (sval - val) + 0.5);
                    ival = Math.Max(0, ival);
                    ival = Math.Min(255, ival);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j, ival);
                }
            }
            pixd.Data = datad;
            return pixd;
        }

        FPix FpixCreate(int width, int height)
        {
            FPix fpixd = new FPix();

            fpixd.W = width; fpixd.H = height;
            fpixd.Wpl = width;
            fpixd.RefCount = 1;
            fpixd.Data = new float[width * height];

            return fpixd;
        }

        /// <summary>
        /// Unsharp masking.
        /// </summary>
        Pix PixUnsharpMaskingGray1D(Pix pixs, int halfwidth, float fract, int direction)
        {
            int w, h, d, wpls, wpld, i, j;
            uint ival;
            uint[] datas, datad;
            int indexs, indexs0, indexs1, indexs2, indexs3, indexs4, indexd;
            float val; float[] a = new float[5];
            Pix pixd = null;

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (d != 8 || pixs.Colormap != null)
            {//return (PIX*)ERROR_PTR("pixs not 8 bpp or has cmap", procName, null);
            }
            if (fract <= 0.0 || halfwidth <= 0)
                return pixs;
            if (halfwidth != 1 && halfwidth != 2)
            {//return (PIX*)ERROR_PTR("halfwidth must be 1 or 2", procName, null);
            }

            pixd = PixCopyBorder(null, pixs, halfwidth, halfwidth, halfwidth, halfwidth);
            datas = pixs.Data;
            datad = pixd.Data;
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;

            if (halfwidth == 1)
            {
                a[0] = -fract / 3.0f;
                a[1] = 1.0f + fract * 2.0f / 3.0f;
                a[2] = a[0];
            }
            else
            {
                a[0] = -fract / 5.0f;
                a[1] = a[0];
                a[2] = 1.0f + fract * 4.0f / 5.0f;
                a[3] = a[0];
                a[4] = a[0];
            }

            if (direction == 1)
            {
                for (i = 0; i < h; i++)
                {
                    indexs = i * wpls;
                    indexd = i * wpld;
                    if (halfwidth == 1)
                    {
                        for (j = 1; j < w - 1; j++)
                        {
                            val = a[0] * JBIG2Statics.GetDataByte(datas, indexs + j - 1) +
                                  a[1] * JBIG2Statics.GetDataByte(datas, indexs + j) +
                                  a[2] * JBIG2Statics.GetDataByte(datas, indexs + j + 1);
                            ival = (uint)val;
                            ival = Math.Max(0, ival);
                            ival = Math.Min(255, ival);
                            JBIG2Statics.SetDataByte(ref datad, indexd + j, ival);
                        }
                    }
                    else
                    {  /* halfwidth == 2 */
                        for (j = 2; j < w - 2; j++)
                        {
                            val = a[0] * JBIG2Statics.GetDataByte(datas, indexs + j - 2) +
                                  a[1] * JBIG2Statics.GetDataByte(datas, indexs + j - 1) +
                                  a[2] * JBIG2Statics.GetDataByte(datas, indexs + j) +
                                  a[3] * JBIG2Statics.GetDataByte(datas, indexs + j + 1) +
                                  a[4] * JBIG2Statics.GetDataByte(datas, indexs + j + 2);
                            ival = (uint)val;
                            ival = Math.Max(0, ival);
                            ival = Math.Min(255, ival);
                            JBIG2Statics.SetDataByte(ref datad, indexd + j, ival);
                        }
                    }
                }
            }
            else
            {
                if (halfwidth == 1)
                {
                    for (i = 1; i < h - 1; i++)
                    {
                        indexs0 = (i - 1) * wpls;
                        indexs1 = i * wpls;
                        indexs2 = (i + 1) * wpls;
                        indexd = i * wpld;
                        for (j = 0; j < w; j++)
                        {
                            val = a[0] * JBIG2Statics.GetDataByte(datas, indexs0 + j) +
                                  a[1] * JBIG2Statics.GetDataByte(datas, indexs1 + j) +
                                  a[2] * JBIG2Statics.GetDataByte(datas, indexs2 + j);
                            ival = (uint)val;
                            ival = Math.Max(0, ival);
                            ival = Math.Min(255, ival);
                            JBIG2Statics.SetDataByte(ref datad, indexd + j, ival);
                        }
                    }
                }
                else
                {
                    for (i = 2; i < h - 2; i++)
                    {
                        indexs0 = (i - 2) * wpls;
                        indexs1 = (i - 1) * wpls;
                        indexs2 = i * wpls;
                        indexs3 = (i + 1) * wpls;
                        indexs4 = (i + 2) * wpls;
                        indexd = i * wpld;
                        for (j = 0; j < w; j++)
                        {
                            val = a[0] * JBIG2Statics.GetDataByte(datas, indexs0 + j) +
                                  a[1] * JBIG2Statics.GetDataByte(datas, indexs1 + j) +
                                  a[2] * JBIG2Statics.GetDataByte(datas, indexs2 + j) +
                                  a[3] * JBIG2Statics.GetDataByte(datas, indexs3 + j) +
                                  a[4] * JBIG2Statics.GetDataByte(datas, indexs4 + j);
                            ival = (uint)val;
                            ival = Math.Max(0, ival);
                            ival = Math.Min(255, ival);
                            JBIG2Statics.SetDataByte(ref datad, indexd + j, ival);
                        }
                    }
                }
            }

            return pixd;
        }

        /// <summary>
        /// Assign border pixels.
        /// </summary>
        Pix PixCopyBorder(Pix pixd, Pix pixs, int left, int right, int top, int bot)
        {
            int w, h;

            if (pixd != null)
            {
                if (pixd == pixs)
                    return pixd;
                else if ((pixs.W != pixd.W) || (pixs.H != pixd.H) || (pixs.D != pixd.D))
                {//return (PIX*)ERROR_PTR("pixs and pixd sizes differ",procName, pixd);
                }
            }
            else
            {
                if ((pixd = PixCreateTemplateNoInit(pixs)) == null)
                {//return (PIX*)ERROR_PTR("pixd not made", procName, pixd);
                }
            }

            w = pixs.W; h = pixs.H;
            PixRasterop(pixd, 0, 0, left, h, JBIG2Statics.PixSrc, pixs, 0, 0);
            PixRasterop(pixd, w - right, 0, right, h, JBIG2Statics.PixSrc, pixs, w - right, 0);
            PixRasterop(pixd, 0, 0, w, top, JBIG2Statics.PixSrc, pixs, 0, 0);
            PixRasterop(pixd, 0, h - bot, w, bot, JBIG2Statics.PixSrc, pixs, 0, h - bot);

            return pixd;
        }

        /// <summary>
        /// Downscaling with (antialias) area mapping.
        /// </summary>
        Pix PixScaleAreaMap(Pix pix, float scalex, float scaley)
        {
            int ws, hs, d, wd, hd, wpls, wpld;
            uint[] datas, datad;
            float maxscale;
            Pix pixs, pixd, pixt1, pixt2, pixt3;

            d = pix.D;
            if (d != 2 && d != 4 && d != 8 && d != 32)
            {//return (PIX*)ERROR_PTR("pix not 2, 4, 8 or 32 bpp", procName, null);
            }
            maxscale = Math.Max(scalex, scaley);
            if (maxscale >= 0.7)
            {
                //L_WARNING("scaling factors not < 0.7; doing regular scaling", procName);
                return PixScale(pix, scalex, scaley);
            }

            if (scalex == 0.5 && scaley == 0.5)
                return PixScaleAreaMap2(pix);
            if (scalex == 0.25 && scaley == 0.25)
            {
                pixt1 = PixScaleAreaMap2(pix);
                pixd = PixScaleAreaMap2(pixt1);

                return pixd;
            }
            if (scalex == 0.125 && scaley == 0.125)
            {
                pixt1 = PixScaleAreaMap2(pix);
                pixt2 = PixScaleAreaMap2(pixt1);
                pixd = PixScaleAreaMap2(pixt2);

                return pixd;
            }
            if (scalex == 0.0625 && scaley == 0.0625)
            {
                pixt1 = PixScaleAreaMap2(pix);
                pixt2 = PixScaleAreaMap2(pixt1);
                pixt3 = PixScaleAreaMap2(pixt2);
                pixd = PixScaleAreaMap2(pixt3);

                return pixd;
            }

            if ((d == 2 || d == 4 || d == 8) && pix.Colormap != null)
            {//                L_WARNING("pix has colormap; removing", procName);
                pixs = PixRemoveColormap(pix, m_removeCmapBasedOnSrc);
                d = pixs.D;
            }
            else if (d == 2 || d == 4)
            {
                pixs = PixConvertTo8(pix, 0);
                d = 8;
            }
            else
                pixs = pix;

            ws = pixs.W; hs = pixs.H;
            datas = pixs.Data;
            wpls = pixs.Wpl;
            wd = (int)(scalex * (float)ws + 0.5);
            hd = (int)(scaley * (float)hs + 0.5);
            if (wd < 1 || hd < 1)
            {//                return (PIX*)ERROR_PTR("pixd too small", procName, null);
            }
            if ((pixd = PixCreate(wd, hd, d)) == null)
            {

            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, scalex, scaley);
            datad = pixd.Data;
            wpld = pixd.Wpl;
            if (d == 8)
                ScaleGrayAreaMapLow(ref datad, wd, hd, wpld, datas, ws, hs, wpls);
            else
                ScaleColorAreaMapLow(ref datad, wd, hd, wpld, datas, ws, hs, wpls);

            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Color and grayscale downsampling with (antialias) area mapping.
        /// </summary>
        void ScaleColorAreaMapLow(ref uint[] datad, int wd, int hd, int wpld, uint[] datas, int ws, int hs, int wpls)
        {
            int i, j, k, m, wm2, hm2;
            uint area00, area10, area01, area11, areal, arear, areat, areab;
            uint xu, yu;
            uint xl, yl;
            uint xup, yup, xuf, yuf;
            uint xlp, ylp, xlf, ylf;
            uint delx, dely, area;
            uint v00r, v00g, v00b;  
            uint v01r, v01g, v01b;  
            uint v10r, v10g, v10b;  
            uint v11r, v11g, v11b;  
            uint vinr, ving, vinb;  
            uint vmidr, vmidg, vmidb;
            uint rval, gval, bval;
            uint pixel00, pixel10, pixel01, pixel11, pixel;
            int indexs, indexd;
            float scx, scy;

            scx = 16 * (float)ws / (float)wd;
            scy = 16 * (float)hs / (float)hd;

            wm2 = ws - 2;
            hm2 = hs - 2;

            for (i = 0; i < hd; i++)
            {
                yu = (uint)(scy * i);
                yl = (uint)(scy * (i + 1.0));
                yup = yu >> 4;
                yuf = yu & 0x0f;
                ylp = yl >> 4;
                ylf = yl & 0x0f;
                dely = ylp - yup;
                indexd = i * wpld;
                indexs = (int)(yup * wpls);
                for (j = 0; j < wd; j++)
                {
                    xu = (uint)(scx * j);
                    xl = (uint)(scx * (j + 1.0));
                    xup = xu >> 4;
                    xuf = xu & 0x0f;
                    xlp = xl >> 4;
                    xlf = xl & 0x0f;
                    delx = xlp - xup;

                    if (xlp > wm2 || ylp > hm2)
                    {
                        datad[indexd + j] = datas[indexs + xup];
                        continue;
                    }

                    area = ((16 - xuf) + 16 * (delx - 1) + xlf) *
                           ((16 - yuf) + 16 * (dely - 1) + ylf);

                    pixel00 = datas[indexs + xup];
                    pixel10 = datas[indexs + xlp];
                    pixel01 = datas[indexs + dely * wpls + xup];
                    pixel11 = datas[indexs + dely * wpls + xlp];
                    area00 = (16 - xuf) * (16 - yuf);
                    area10 = xlf * (16 - yuf);
                    area01 = (16 - xuf) * ylf;
                    area11 = xlf * ylf;
                    v00r = area00 * ((pixel00 >> m_redShift) & 0xff);
                    v00g = area00 * ((pixel00 >> m_greenShift) & 0xff);
                    v00b = area00 * ((pixel00 >> m_blueShift) & 0xff);
                    v10r = area10 * ((pixel10 >> m_redShift) & 0xff);
                    v10g = area10 * ((pixel10 >> m_greenShift) & 0xff);
                    v10b = area10 * ((pixel10 >> m_blueShift) & 0xff);
                    v01r = area01 * ((pixel01 >> m_redShift) & 0xff);
                    v01g = area01 * ((pixel01 >> m_greenShift) & 0xff);
                    v01b = area01 * ((pixel01 >> m_blueShift) & 0xff);
                    v11r = area11 * ((pixel11 >> m_redShift) & 0xff);
                    v11g = area11 * ((pixel11 >> m_greenShift) & 0xff);
                    v11b = area11 * ((pixel11 >> m_blueShift) & 0xff);
                    vinr = ving = vinb = 0;
                    for (k = 1; k < dely; k++)
                    {
                        for (m = 1; m < delx; m++)
                        {
                            pixel = datas[indexs + k * wpls + xup + m];
                            vinr += 256 * ((pixel >> m_redShift) & 0xff);
                            ving += (int)256 * ((pixel >> m_greenShift) & 0xff);
                            vinb += 256 * ((pixel >> m_blueShift) & 0xff);
                        }
                    }
                    vmidr = vmidg = vmidb = 0;
                    areal = (16 - xuf) * 16;
                    arear = xlf * 16;
                    areat = 16 * (16 - yuf);
                    areab = 16 * ylf;
                    for (k = 1; k < dely; k++)
                    {
                        pixel = datas[indexs + k * wpls + xup];
                        vmidr += areal * ((pixel >> m_redShift) & 0xff);
                        vmidg += areal * ((pixel >> m_greenShift) & 0xff);
                        vmidb += areal * ((pixel >> m_blueShift) & 0xff);
                    }
                    for (k = 1; k < dely; k++)
                    { 
                        pixel = datas[indexs + k * wpls + xlp];
                        vmidr += arear * ((pixel >> m_redShift) & 0xff);
                        vmidg += arear * ((pixel >> m_greenShift) & 0xff);
                        vmidb += arear * ((pixel >> m_blueShift) & 0xff);
                    }
                    for (m = 1; m < delx; m++)
                    { 
                        pixel = datas[indexs + xup + m];
                        vmidr += areat * ((pixel >> m_redShift) & 0xff);
                        vmidg += areat * ((pixel >> m_greenShift) & 0xff);
                        vmidb += areat * ((pixel >> m_blueShift) & 0xff);
                    }
                    for (m = 1; m < delx; m++)
                    { 
                        pixel = datas[indexs + dely * wpls + xup + m];
                        vmidr += areab * ((pixel >> m_redShift) & 0xff);
                        vmidg += areab * ((pixel >> m_greenShift) & 0xff);
                        vmidb += areab * ((pixel >> m_blueShift) & 0xff);
                    }

                    rval = (v00r + v01r + v10r + v11r + vinr + vmidr + 128) / area;
                    gval = (v00g + v01g + v10g + v11g + ving + vmidg + 128) / area;
                    bval = (v00b + v01b + v10b + v11b + vinb + vmidb + 128) / area;

                    ComposeRGBPixel((int)rval, (int)gval, (int)bval, (int)datad[indexd + j]);
                }
            }
        }

        /// <summary>
        /// Color and grayscale downsampling with (antialias) area mapping.
        /// </summary>
        void ScaleGrayAreaMapLow(ref uint[] datad, int wd, int hd, int wpld, uint[] datas, int ws, int hs, int wpls)
        {
            int i, j, k, m, wm2, hm2;
            int xu, yu;
            int xl, yl;
            int xup, yup, xuf, yuf;
            int xlp, ylp, xlf, ylf;
            int delx, dely, area;
            uint v00;
            uint v01;
            uint v10;
            uint v11;
            uint vin;
            uint vmid;
            uint val;
            int indexs = 0, indexd = 0;
            float scx, scy;

            scx = 16 * (float)ws / (float)wd;
            scy = 16 * (float)hs / (float)hd;

            wm2 = ws - 2;
            hm2 = hs - 2;

            for (i = 0; i < hd; i++)
            {
                yu = (int)(scy * i);
                yl = (int)(scy * (i + 1.0));
                yup = yu >> 4;
                yuf = yu & 0x0f;
                ylp = yl >> 4;
                ylf = yl & 0x0f;
                dely = ylp - yup;
                indexd = i * wpld;
                indexs = yup * wpls;
                for (j = 0; j < wd; j++)
                {
                    xu = (int)(scx * j);
                    xl = (int)(scx * (j + 1.0));
                    xup = xu >> 4;
                    xuf = xu & 0x0f;
                    xlp = xl >> 4;
                    xlf = xl & 0x0f;
                    delx = xlp - xup;

                    if (xlp > wm2 || ylp > hm2)
                    {
                        JBIG2Statics.SetDataByte(ref datad, indexd + j, JBIG2Statics.GetDataByte(datas, indexs + xup));
                        continue;
                    }

                    area = ((16 - xuf) + 16 * (delx - 1) + xlf) * ((16 - yuf) + 16 * (dely - 1) + ylf);

                    v00 = (uint)((16 - xuf) * (16 - yuf) * JBIG2Statics.GetDataByte(datas, indexs + xup));
                    v10 = (uint)(xlf * (16 - yuf) * JBIG2Statics.GetDataByte(datas, indexs + xlp));
                    v01 = (uint)((16 - xuf) * ylf * JBIG2Statics.GetDataByte(datas, indexs + dely * wpls + xup));
                    v11 = (uint)(xlf * ylf * JBIG2Statics.GetDataByte(datas, indexs + dely * wpls + xlp));
                    for (vin = 0, k = 1; k < dely; k++)
                    {
                        for (m = 1; m < delx; m++)
                        {
                            vin += 256 * JBIG2Statics.GetDataByte(datas, indexs + k * wpls + xup + m);
                        }
                    }
                    for (vmid = 0, k = 1; k < dely; k++) 
                        vmid += (uint)((16 - xuf) * 16 * JBIG2Statics.GetDataByte(datas, indexs + k * wpls + xup));
                    for (k = 1; k < dely; k++)
                        vmid += (uint)(xlf * 16 * JBIG2Statics.GetDataByte(datas, indexs + k * wpls + xlp));
                    for (m = 1; m < delx; m++)
                        vmid += (uint)(16 * (16 - yuf) * JBIG2Statics.GetDataByte(datas, indexs + xup + m));
                    for (m = 1; m < delx; m++)
                        vmid += (uint)(16 * ylf * JBIG2Statics.GetDataByte(datas, indexs + dely * wpls + xup + m));
                    val = (uint)((v00 + v01 + v10 + v11 + vin + vmid + 128) / area);

                    JBIG2Statics.SetDataByte(ref datad, indexd + j, val);
                }
            }
        }

        /// <summary>
        /// Downscaling with (antialias) area mapping.
        /// </summary>
        Pix PixScaleAreaMap2(Pix pix)
        {
            int wd, hd, d, wpls, wpld;
            uint[] datas, datad;
            Pix pixs, pixd;

            d = pix.D;
            if (d != 2 && d != 4 && d != 8 && d != 32)
            {//return (PIX *)ERROR_PTR("pix not 2, 4, 8 or 32 bpp", procName, null);
            }

            if ((d == 2 || d == 4 || d == 8) && (pix.Colormap != null))
            {                //L_WARNING("pix has colormap; removing", procName);
                pixs = PixRemoveColormap(pix, m_removeCmapBasedOnSrc);
                d = pixs.D;
            }
            else if (d == 2 || d == 4)
            {
                pixs = PixConvertTo8(pix, 0);
                d = 8;
            }
            else
                pixs = pix;

            wd = pixs.W / 2;
            hd = pixs.H / 2;
            datas = pixs.Data;
            wpls = pixs.Wpl;
            pixd = PixCreate(wd, hd, d);
            datad = pixd.Data;
            wpld = pixd.Wpl;
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 0.5f, 0.5f);
            ScaleAreaMapLow2(datad, wd, hd, wpld, datas, d, wpls);

            return pixd;
        }

        /// <summary>
        /// 2x area mapped downscaling.
        /// </summary>
        void ScaleAreaMapLow2(uint[] datad, int wd, int hd, int wpld, uint[] datas, int d, int wpls)
        {
            int i, j;
            int indexs, indexd;
            uint rval, gval, bval, val, pixel;

            if (d == 8)
            {
                for (i = 0; i < hd; i++)
                {
                    indexs = 2 * i * wpls;
                    indexd = i * wpld;
                    for (j = 0; j < wd; j++)
                    {
                        val = JBIG2Statics.GetDataByte(datas, indexs + 2 * j);
                        val += JBIG2Statics.GetDataByte(datas, indexs + 2 * j + 1);
                        val += JBIG2Statics.GetDataByte(datas, indexs + wpls + 2 * j);
                        val += JBIG2Statics.GetDataByte(datas, indexs + wpls + 2 * j + 1);
                        val >>= 2;
                        JBIG2Statics.SetDataByte(ref datad, indexd + j, val);
                    }
                }
            }
            else
            {  /* d == 32 */
                for (i = 0; i < hd; i++)
                {
                    indexs = 2 * i * wpls;
                    indexd = i * wpld;
                    for (j = 0; j < wd; j++)
                    {
                        pixel = datas[indexs + 2 * j];
                        rval = (pixel >> m_redShift) & 0xff;
                        gval = (pixel >> m_greenShift) & 0xff;
                        bval = (pixel >> m_blueShift) & 0xff;
                        pixel = datas[indexs + 2 * j + 1];
                        rval += (pixel >> m_redShift) & 0xff;
                        gval += (pixel >> m_greenShift) & 0xff;
                        bval += (pixel >> m_blueShift) & 0xff;
                        pixel = datas[indexs + wpls + 2 * j];
                        rval += (pixel >> m_redShift) & 0xff;
                        gval += (pixel >> m_greenShift) & 0xff;
                        bval += (pixel >> m_blueShift) & 0xff;
                        pixel = datas[indexs + wpls + 2 * j + 1];
                        rval += (pixel >> m_redShift) & 0xff;
                        gval += (pixel >> m_greenShift) & 0xff;
                        bval += (pixel >> m_blueShift) & 0xff;
                        ComposeRGBPixel((int)rval >> 2, (int)gval >> 2, (int)bval >> 2, (int)pixel);
                        datad[indexd + j] = pixel;
                    }
                }
            }
        }

        /// <summary>
        /// Top-level conversion to 8 or 32 bpp, without colormap.
        /// </summary>
        Pix PixConvertTo8Or32(Pix pixs, int copyflag, int warnflag)
        {
            int d;
            Pix pixd;

            d = pixs.D;
            if (pixs.Colormap != null)
            {
                //if (warnflag) L_WARNING("pix has colormap; removing", procName);
                pixd = PixRemoveColormap(pixs, m_removeCmapBasedOnSrc);
            }
            else if (d == 8 || d == 32)
            {
                if (copyflag == 0)
                    pixd = pixs;
                else
                    pixd = PixCopy(null, pixs);
            }
            else
                pixd = PixConvertTo8(pixs, 0);

            d = pixd.D;
            if (d != 8 && d != 32)
            {                //return (PIX*)ERROR_PTR("depth not 8 or 32 bpp", procName, null);
            }

            return pixd;
        }
        
        /// <summary>
        /// Top level conversion to 8 bpp.
        /// </summary>
        Pix PixConvertTo8(Pix pixs, int cmapflag)
        {
            int d;
            Pix pixd;
            PixColormap cmap;

            //if (!pixs)
            //    return (PIX*)ERROR_PTR("pixs not defined", procName, null);
            d = pixs.D;
            if (d != 1 && d != 2 && d != 4 && d != 8 && d != 16 && d != 32)
            {//                return (PIX*)ERROR_PTR("depth not {1,2,4,8,16,32}", procName, null);
            }

            if (d == 1)
            {
                if (cmapflag == 0)
                    return PixConvert1To8(null, pixs, 255, 0);
                else
                {
                    pixd = PixConvert1To8(null, pixs, 0, 1);
                    cmap = CreatePixCmap(8);
                    PixCmapAddColor(ref cmap, 255, 255, 255);
                    PixCmapAddColor(ref cmap, 0, 0, 0);
                    pixd.Colormap = cmap;
                    return pixd;
                }
            }
            else if (d == 2)
                return PixConvert2To8(pixs, 0, 85, 170, 255, cmapflag);
            else if (d == 4)
                return PixConvert4To8(pixs, cmapflag);
            else if (d == 8)
            {
                cmap = pixs.Colormap;
                if ((cmap != null && (cmapflag == 1)) || (cmap != null && cmapflag != null))
                    return PixCopy(null, pixs);
                else if (cmap != null)
                    return PixRemoveColormap(pixs, m_removeCmapToGrayScale);
                else
                { 
                    pixd = PixCopy(null, pixs);
                    PixAddGrayColormap8(pixd);
                    return pixd;
                }
            }
            else if (d == 16)
            {
                pixd = PixConvert16To8(pixs, 1);
                if (cmapflag > 0)
                    PixAddGrayColormap8(pixd);
                return pixd;
            }
            else
            {
                pixd = PixConvertRGBToLuminance(pixs);
                if (cmapflag > 0)
                    PixAddGrayColormap8(pixd);
                return pixd;
            }
        }

        /// <summary>
        /// Conversion from RGB color to grayscale.
        /// </summary>
        private Pix PixConvertRGBToLuminance(Pix pixs)
        {
            return PixConvertRGBToGray(pixs, 0.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Conversion from RGB color to grayscale.
        /// </summary>
        Pix PixConvertRGBToGray(Pix pixs, float rwt, float gwt, float bwt)
        {
            int i, j, w, h, wpls, wpld;
            uint val, word;
            uint[] lined;
            float sum;
            Pix pixd = null;

            if (pixs.D != 32)
            {//return (PIX *)ERROR_PTR("pixs not 32 bpp", procName, null);
            }
            if (rwt < 0.0 || gwt < 0.0 || bwt < 0.0)
            {//return (PIX*)ERROR_PTR("weights not all >= 0.0", procName, null);
            }

            if (rwt == 0.0 && gwt == 0.0 && bwt == 0.0)
            {
                rwt = JBIG2Statics.RedWeight;
                gwt = JBIG2Statics.GreenWeight;
                bwt = JBIG2Statics.BlueWeight;
            }
            sum = rwt + gwt + bwt;
            if (Math.Abs(sum - 1.0) > 0.0001)
            {
                //L_WARNING("weights don't sum to 1; maintaining ratios", procName);
                rwt = rwt / sum;
                gwt = gwt / sum;
                bwt = bwt / sum;
            }

            w = pixs.W; h = pixs.H;
            wpls = pixs.Wpl;
            if ((pixd = PixCreate(w, h, 8)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            lined = new uint[pixs.Data.Length];
            wpld = pixd.Wpl;

            for (i = 0; i < h * w; i++)
            {
                word = pixs.Data[i];

                byte[] check = new byte[4];
                check[3] = 255;
                check[2] = (byte)((word >> 16) & 0x0ff); //red
                check[1] = (byte)((word >> 8) & 0x0ff); //green
                check[0] = (byte)((word) & 0x0ff); //blue

                int tmp = (check[2] << 1);                   // 2 * red
                tmp += (check[1] << 2 + check[1]);      // 5 * green
                tmp += (check[0]);                     // 1 * blue
                tmp = (tmp >> 3); // divide by 8

                lined[i] = (uint)tmp;
            }
            pixd.Data = lined;
            return pixd;
        }

        /// <summary>
        /// Convert colored image to grayscale.
        /// </summary>
        private Pix PixConvertRGBToGrayFast(Pix pixs)
        {
            int i, j, w, h, wpls, wpld;
            uint val;
            uint[] lined;
            Pix pixd = null;

            if (pixs.D != 32)
            {
#if DEBUG
                Console.WriteLine("pixs not 32 bpp");
#endif
                return null;
            }
            wpls = pixs.Wpl; w = pixs.W; h = pixs.H;
            pixd = PixCreate(w, h, 8);
            PixCopyResolution(pixd, pixs);
            wpld = pixd.Wpl;
            lined = new uint[h * wpld];

            int s = 0, d = 0;
            for (i = 0; i < h; i++)
            {
                s = i * wpls;
                d = i * wpld;
                for (j = 0; j < w; j++, s++)
                {
                    int a = (d * 4) + j; //4 bytes per int.
                    val = ((pixs.Data[s] >> m_greenShift) & 0xff);
                    JBIG2Statics.SetDataByte(ref lined, a, val);
                }
            }
            pixd.Data = lined;
            return pixd;
        }

        /// <summary>
        /// Add colormap losslessly (8 to 8).
        /// </summary>
        private void PixAddGrayColormap8(Pix pixs)
        {
            PixColormap cmap;

            if (pixs == null || pixs.D != 8)
            {
                Console.WriteLine("pixs not defined or not 8 bpp");
                return;
            }
            if (pixs.Colormap == null)
                return;

            cmap = PixcmapCreateLinear(8, 256);
            pixs.Colormap = cmap;
        }

        /// <summary>
        /// Colormap creation.
        /// </summary>
        PixColormap PixcmapCreateLinear(int d, int nlevels)
        {
            int maxlevels, i, val;
            PixColormap cmap;

            if (d != 1 && d != 2 && d != 4 && d != 8)
            {//return (PIXCMAP*)ERROR_PTR("d not in {1, 2, 4, 8}", procName, null);
            }
            maxlevels = 1 << d;
            if (nlevels < 2 || nlevels > maxlevels)
            {//   return (PIXCMAP*)ERROR_PTR("invalid nlevels", procName, null);
            }

            cmap = CreatePixCmap(d);
            for (i = 0; i < nlevels; i++)
            {
                val = (255 * i) / (nlevels - 1);
                PixCmapAddColor(ref cmap, val, val, val);
            }
            return cmap;
        }

        /// <summary>
        /// Unpacking conversion from 1, 2 and 4 bpp to 8 bpp.
        /// </summary>
        Pix PixConvert4To8(Pix pixs, int cmapflag)
        {
            int w, h, i, j, wpls, wpld, ncolor;
            int rval = 0, gval = 0, bval = 0;
            uint bytes, qbit;
            uint[] datas, datad;
            int indexs, indexd;
            Pix pixd = null;
            PixColormap cmaps, cmapd;

            if (pixs.W != 4)
            {//        return (PIX *)ERROR_PTR("pixs not 4 bpp", procName, null);
            }

            cmaps = pixs.Colormap;
            if (cmaps == null && cmapflag == 0)
                return PixRemoveColormap(pixs, m_removeCmapToGrayScale);

            w = pixs.W; h = pixs.H;
            if ((pixd = PixCreate(w, h, 8)) == null)
            {//return (PIX *)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            datas = pixs.Data;
            wpls = pixs.Wpl;
            datad = pixd.Data;
            wpld = pixd.Wpl;

            if (cmapflag > 0)
            {
                cmapd = CreatePixCmap(8);
                if (cmaps != null)
                { 
                    ncolor = cmaps.N;
                    for (i = 0; i < ncolor; i++)
                    {
                        PixCmapGetColor(cmaps, i, rval, gval, bval);
                        PixCmapAddColor(ref cmapd, rval, gval, bval);
                    }
                }
                else
                {
                    for (i = 0; i < 16; i++)
                        PixCmapAddColor(ref cmapd, 17 * i, 17 * i, 17 * i);
                }
                pixd.Colormap = cmapd;
                for (i = 0; i < h; i++)
                {
                    indexs = i * wpls;
                    indexd = i * wpld;
                    for (j = 0; j < w; j++)
                    {
                        qbit = JBIG2Statics.GetDataQbit(datas[indexs], j);
                        JBIG2Statics.SetDataByte(ref datad, indexd + j, qbit);
                    }
                }
                return pixd;
            }

            for (i = 0; i < h; i++)
            {
                indexs = i * wpls;
                indexd = i * wpld;
                for (j = 0; j < w; j++)
                {
                    qbit = JBIG2Statics.GetDataQbit(datas[indexs], j);
                    bytes = (qbit << 4) | qbit;
                    JBIG2Statics.SetDataByte(ref datad, indexd + j, bytes);
                }
            }
            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Unpacking conversion from 1, 2 and 4 bpp to 8 bpp.
        /// </summary>
        Pix PixConvert2To8(Pix pixs, byte val0, byte val1, byte val2, byte val3, int cmapflag)
        {
            int w, h, i, j, nbytes, wpls, wpld, ncolor, indexs = 0, indexd = 0;
            int rval = 0, gval = 0, bval = 0;
            byte[] val = new byte[4];
            uint index, dibit, bytes;
            uint[] tab = new uint[256], datas, datad;
            Pix pixd = null;
            PixColormap cmaps, cmapd;

            if (pixs.D != 2)
            {//               return (PIX*)ERROR_PTR("pixs not 2 bpp", procName, null);
            }

            cmaps = pixs.Colormap;
            if ((cmaps == null) && (cmapflag == 0))
                return PixRemoveColormap(pixs, m_removeCmapToGrayScale);

            w = pixs.W; h = pixs.H;
            if ((pixd = PixCreate(w, h, 8)) == null)
            {//                return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            datas = pixs.Data;
            wpls = pixs.Wpl;
            datad = pixd.Data;
            wpld = pixd.Wpl;

            if (cmapflag > 0)
            { 
                cmapd = CreatePixCmap(8);
                if (cmaps != null)
                {
                    ncolor = cmaps.N;
                    for (i = 0; i < ncolor; i++)
                    {
                        PixCmapGetColor(cmaps, i, rval, gval, bval);
                        PixCmapAddColor(ref cmapd, rval, gval, bval);
                    }
                }
                else
                { 
                    PixCmapAddColor(ref cmapd, val0, val0, val0);
                    PixCmapAddColor(ref cmapd, val1, val1, val1);
                    PixCmapAddColor(ref cmapd, val2, val2, val2);
                    PixCmapAddColor(ref cmapd, val3, val3, val3);
                }
                pixd.Colormap = cmapd;
                for (i = 0; i < h; i++)
                {
                    indexs = i * wpls;
                    indexd = i * wpld;
                    for (j = 0; j < w; j++)
                    {
                        dibit = JBIG2Statics.GetDataDibit(datas[indexs], j);
                        JBIG2Statics.SetDataByte(ref datad, indexd + j, dibit);
                    }
                }
                return pixd;
            }

            val[0] = val0;
            val[1] = val1;
            val[2] = val2;
            val[3] = val3;
            for (index = 0; index < 256; index++)
                tab[index] = (uint)((val[(index >> 6) & 3] << 24) | (val[(index >> 4) & 3] << 16) | (val[(index >> 2) & 3] << 8) | val[index & 3]);

            nbytes = (w + 3) / 4;
            for (i = 0; i < h; i++)
            {
                indexs = i * wpls;
                indexd = i * wpld;
                for (j = 0; j < nbytes; j++)
                {
                    bytes = JBIG2Statics.GetDataByte(datas, indexs + j);
                    datad[indexd + j] = tab[bytes];
                }
            }

            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Colormap creation.
        /// </summary>
        void PixCmapAddColor(ref PixColormap cmap, int rval, int gval, int bval)
        {
            if (cmap.N >= cmap.Nalloc)
            {//return ERROR_INT("no free color entries", procName, 1);
            }
            if (cmap.Array[cmap.N] == null)
                cmap.Array[cmap.N] = new RGBA_Quad();
            cmap.Array[cmap.N].Red = rval;
            cmap.Array[cmap.N].Green = gval;
            cmap.Array[cmap.N].Blue = bval;
            cmap.N++;
        }

        /// <summary>
        /// Colormap creation and addition.
        /// </summary>
        PixColormap CreatePixCmap(int depth)
        {
            PixColormap cmap = new PixColormap();

            if (depth != 1 && depth != 2 && depth != 4 && depth != 8)
            {//                return (PIXCMAP*)ERROR_PTR("depth not in {1,2,4,8}", procName, null);
            }

            cmap.Depth = depth;
            cmap.Nalloc = 1 << depth;
            cmap.N = 0;

            return cmap;
        }

        /// <summary>
        /// Conversion from 1, 2 and 4 bpp to 8 bpp.
        /// </summary>
        Pix PixConvert1To8(Pix pixd, Pix pixs, byte val0, byte val1)
        {
            int w, h, i, j, nqbits, wpls, wpld, indexs = 0, indexd = 0;
            byte[] val = new byte[2];
            uint index, qbit;
            uint[] tab = new uint[16], datas, datad;

            if (pixs.D != 1)
            {//return (PIX*)ERROR_PTR("pixs not 1 bpp", procName, pixd);
            }

            w = pixs.W; h = pixs.H;
            if (pixd != null)
            {
                if (w != pixd.W || h != pixd.H)
                {//return (PIX*)ERROR_PTR("pix sizes unequal", procName, pixd);
                }
                if (pixd.D != 8)
                {//return (PIX*)ERROR_PTR("pixd not 8 bpp", procName, pixd);
                }
            }
            else
            {
                if ((pixd = PixCreate(w, h, 8)) == null)
                {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
                }
            }
            PixCopyResolution(pixd, pixs);

            val[0] = val0;
            val[1] = val1;
            for (index = 0; index < 16; index++)
                tab[index] = (uint)((val[(index >> 3) & 1] << 24) | (val[(index >> 2) & 1] << 16) | (val[(index >> 1) & 1] << 8) | val[index & 1]);

            datas = pixs.Data;
            wpls = pixs.Wpl;
            datad = pixd.Data;
            wpld = pixd.Wpl;
            nqbits = (w + 3) / 4;
            for (i = 0; i < h; i++)
            {
                indexs = i * wpls;
                indexd = i * wpld;
                for (j = 0; j < nqbits; j++)
                {
                    qbit = JBIG2Statics.GetDataQbit(datas[indexs], j);
                    datad[indexd + j] = tab[qbit];
                }
            }
            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Binary scaling by closest pixel sampling.
        /// </summary>
        Pix PixScaleBinary(Pix pixs, float scalex, float scaley)
        {
            int ws, hs, wpls, wd, hd, wpld;
            uint[] datas, datad;
            Pix pixd;

            if (pixs.D != 1)
            {//return (PIX*)ERROR_PTR("pixs must be 1 bpp", procName, null);
            }
            if (scalex == 1.0 && scaley == 1.0)
                return PixCopy(null, pixs);

            ws = pixs.W; hs = pixs.H; 
            datas = pixs.Data;
            wpls = pixs.Wpl;
            wd = (int)(scalex * (float)ws + 0.5);
            hd = (int)(scaley * (float)hs + 0.5);
            if ((pixd = PixCreate(wd, hd, 1)) == null)
            {//return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyColormap(pixd, pixs);
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, scalex, scaley);
            datad = pixd.Data;
            wpld = pixd.Wpl;
            ScaleBinaryLow(ref datad, wd, hd, wpld, datas, ws, hs, wpls);
            return pixd;
        }

        /// <summary>
        /// Binary scaling by closest pixel sampling.
        /// </summary>
        void ScaleBinaryLow(ref uint[] datad, int wd, int hd, int wpld, uint[] datas, int ws, int hs, int wpls)
        {
            int i, j, bpld;
            int xs, prevxs;
            uint sval;
            int[] srow, scol;
            int indexs, prevlines, indexd, prevlined;
            float wratio, hratio;

            bpld = 4 * wpld;
            datad = new uint[hd * bpld];

            srow = new int[hd];
            scol = new int[wd];

            wratio = (float)ws / (float)wd;
            hratio = (float)hs / (float)hd;
            for (i = 0; i < hd; i++)
                srow[i] = Math.Min((int)(hratio * i + 0.5), hs - 1);
            for (j = 0; j < wd; j++)
                scol[j] = Math.Min((int)(wratio * j + 0.5), ws - 1);

            prevlines = char.MinValue;
            prevxs = -1;
            sval = 0;
            for (i = 0; i < hd; i++)
            {
                indexs = srow[i] * wpls;
                indexd = i * wpld;
                if (datas[indexs] != prevlines)
                {
                    for (j = 0; j < wd; j++)
                    {
                        xs = scol[j];
                        if (xs != prevxs)
                        {
                            if ((sval = JBIG2Statics.GetDataBit(datas, indexs, xs)) != 0)
                                JBIG2Statics.SetDataBit(ref datad[indexd], j);
                            prevxs = xs;
                        }
                        else
                        { 
                            if (sval > 0)
                                JBIG2Statics.SetDataBit(ref datad[indexd], j);
                        }
                    }
                }
                else
                { 
                    prevlined = indexd - wpld;
                    Array.Copy(datad, prevlined, datad, indexd, bpld);
                }
                prevlines = indexs;
            }
        }

        /// <summary>
        /// Scale-to-gray (1 bpp --> 8 bpp; integer downscaling).
        /// </summary>
        Pix PixScaleToGray2(Pix pixs)
        {
            byte[] valtab;
            int ws, hs, wd, hd;
            int wpld, wpls;
            uint[] sumtab;
            uint[] datas, datad;
            Pix pixd = null;

            ws = pixs.W; hs = pixs.H;
            wd = ws / 2;
            hd = hs / 2;

            if ((pixd = PixCreate(wd, hd, 8)) == null)
            {     //   return (PIX *)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 0.5f, 0.5f);
            datas = pixs.Data;
            datad = pixd.Data;
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;

            if ((sumtab = MakeSumTabSG2()) == null)
            {//return (PIX *)ERROR_PTR("sumtab not made", procName, null);
            }
            if ((valtab = MakeValTabSG2()) == null)
            {//   return (PIX *)ERROR_PTR("valtab not made", procName, null);
            }

            ScaleToGray2Low(ref datad, wd, hd, wpld, datas, wpls, sumtab, valtab);

            return pixd;
        }

        /// <summary>
        /// Scale to gray 2x.
        /// </summary>
        void ScaleToGray2Low(ref uint[] datad, int wd, int hd, int wpld, uint[] datas, int wpls, uint[] sumtab, byte[] valtab)
        {
            int i, j, l, k, m, wd4, extra;
            uint sbyte1, sbyte2, sum;
            int indexs = 0, indexd = 0;

            wd4 = (int)(wd & 0xfffffffc);
            extra = wd - wd4;
            for (i = 0, l = 0; i < hd; i++, l += 2)
            {
                indexs = l * wpls;
                indexd = i * wpld;
                for (j = 0, k = 0; j < wd4; j += 4, k++)
                {
                    sbyte1 = JBIG2Statics.GetDataByte(datas, indexs + k);
                    sbyte2 = JBIG2Statics.GetDataByte(datas, indexs + wpls + k);
                    sum = sumtab[sbyte1] + sumtab[sbyte2];
                    JBIG2Statics.SetDataByte(ref datad, indexd + j, valtab[sum >> 24]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 1, valtab[(sum >> 16) & 0xff]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 2, valtab[(sum >> 8) & 0xff]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 3, valtab[sum & 0xff]);
                }
                if (extra > 0)
                {
                    sbyte1 = JBIG2Statics.GetDataByte(datas, indexs + k);
                    sbyte2 = JBIG2Statics.GetDataByte(datas, indexs + wpls + k);
                    sum = sumtab[sbyte1] + sumtab[sbyte2];
                    for (m = 0; m < extra; m++)
                        JBIG2Statics.SetDataByte(ref datad, indexd + j + m, valtab[((sum >> (24 - 8 * m)) & 0xff)]);
                }
            }
        }
        
        /// <summary>
        /// Returns an 8 bit value for the sum of ON pixels in a 2x2 square.
        /// </summary>
        byte[] MakeValTabSG2()
        {
            int i;
            byte[] tab = new byte[5];

            for (i = 0; i < 5; i++)
                tab[i] = (byte)(255 - (i * 255) / 4);

            return tab;
        }

        /// <summary>
        /// Table of 256 uint.
        /// </summary>
        uint[] MakeSumTabSG2()
        {
            int i;
            int[] sum = { 0, 1, 1, 2 };
            uint[] tab = new uint[256];

            /* Pack the four sums separately in four bytes */
            for (i = 0; i < 256; i++)
                tab[i] = (uint)(sum[i & 0x3] | sum[(i >> 2) & 0x3] << 8 | sum[(i >> 4) & 0x3] << 16 | sum[(i >> 6) & 0x3] << 24);

            return tab;
        }

        /// <summary>
        /// Scale-to-gray (1 bpp --> 8 bpp; integer downscaling).
        /// </summary>
        Pix PixScaleToGray3(Pix pixs)
        {
            byte[] valtab;
            int ws, hs, wd, hd;
            int wpld, wpls;
            uint[] sumtab;
            uint[] datas, datad;
            Pix pixd = null;

            ws = pixs.W; hs = pixs.H;
            wd = (int)((ws / 3) & 0xfffffff8);   
            hd = hs / 3;

            if ((pixd = PixCreate(wd, hd, 8)) == null)
            {
                //return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 0.33333f, 0.33333f);
            datas = pixs.Data;
            datad = pixd.Data;
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;

            if ((sumtab = MakeSumTabSG3()) == null)
            {//return (PIX*)ERROR_PTR("sumtab not made", procName, null);
            }
            if ((valtab = MakeValTabSG3()) == null)
            {
                //return (PIX*)ERROR_PTR("valtab not made", procName, null);
            }

            ScaleToGray3Low(ref datad, wd, hd, wpld, datas, wpls, sumtab, valtab);

            return pixd;
        }

        /// <summary>
        /// Scale to gray 3x.
        /// </summary>
        void ScaleToGray3Low(ref uint[] datad, int wd, int hd, int wpld, uint[] datas, int wpls, uint[] sumtab, byte[] valtab)
        {
            int i, j, l, k;
            uint threebytes1, threebytes2, threebytes3, sum;
            int indexs = 0, indexd = 0;

            for (i = 0, l = 0; i < hd; i++, l += 3)
            {
                indexs = l * wpls;
                indexd = i * wpld;
                for (j = 0, k = 0; j < wd; j += 8, k += 3)
                {
                    threebytes1 = (JBIG2Statics.GetDataByte(datas, indexs + k) << 16) |
                                  (JBIG2Statics.GetDataByte(datas, indexs + k + 1) << 8) |
                                  JBIG2Statics.GetDataByte(datas, indexs + k + 2);
                    threebytes2 = (JBIG2Statics.GetDataByte(datas, indexs + wpls + k) << 16) |
                                  (JBIG2Statics.GetDataByte(datas, indexs + +wpls + k + 1) << 8) |
                                  JBIG2Statics.GetDataByte(datas, indexs + wpls + k + 2);
                    threebytes3 = (JBIG2Statics.GetDataByte(datas, indexs + +2 * wpls + k) << 16) |
                                  (JBIG2Statics.GetDataByte(datas, indexs + 2 * wpls + k + 1) << 8) |
                                  JBIG2Statics.GetDataByte(datas, indexs + 2 * wpls + k + 2);

                    sum = (uint)(sumtab[(threebytes1 >> 18)] + sumtab[(threebytes2 >> 18)] + sumtab[(threebytes3 >> 18)]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j, valtab[JBIG2Statics.GetDataByte(sum, 2)]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 1, valtab[JBIG2Statics.GetDataByte(sum, 3)]);

                    sum = (uint)(sumtab[((threebytes1 >> 12) & 0x3f)] + sumtab[((threebytes2 >> 12) & 0x3f)] +
                          sumtab[((threebytes3 >> 12) & 0x3f)]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 2, valtab[JBIG2Statics.GetDataByte(sum, 2)]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 3, valtab[JBIG2Statics.GetDataByte(sum, 3)]);

                    sum = (uint)(sumtab[((threebytes1 >> 6) & 0x3f)] + sumtab[((threebytes2 >> 6) & 0x3f)] +
                          sumtab[((threebytes3 >> 6) & 0x3f)]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 4, valtab[JBIG2Statics.GetDataByte(sum, 2)]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 5, valtab[JBIG2Statics.GetDataByte(sum, 3)]);

                    sum = (uint)(sumtab[(threebytes1 & 0x3f)] + sumtab[(threebytes2 & 0x3f)] + sumtab[(threebytes3 & 0x3f)]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 6, valtab[JBIG2Statics.GetDataByte(sum, 2)]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 7, valtab[JBIG2Statics.GetDataByte(sum, 3)]);
                }
            }
        }

        /// <summary>
        /// Returns an 8 bit value for the sum of ON pixels in a 3x3 square.
        /// </summary>
        byte[] MakeValTabSG3()
        {
            int i;
            byte[] tab = new byte[10];

            for (i = 0; i < 10; i++)
                tab[i] = (byte)(0xff - (i * 255) / 9);

            return tab;
        }

        /// <summary>
        /// Table of 64 uint.
        /// </summary>
        uint[] MakeSumTabSG3()
        {
            int i;
            uint[] sum = { 0, 1, 1, 2, 1, 2, 2, 3 };
            uint[] tab = new uint[64];

            for (i = 0; i < 64; i++)
                tab[i] = (sum[i & 0x07]) | (sum[(i >> 3) & 0x07] << 8);

            return tab;
        }

        /// <summary>
        /// Pix scaled down by 4x in each direction.
        /// </summary>
        Pix PixScaleToGray4(Pix pixs)
        {
            byte[] valtab;
            int ws, hs, wd, hd;
            int wpld, wpls;
            uint[] sumtab;
            uint[] datas, datad;
            Pix pixd = null;

            ws = pixs.W; hs = pixs.H;
            wd = (int)((ws / 4) & 0xfffffffe);
            hd = hs / 4;

            if ((pixd = PixCreate(wd, hd, 8)) == null)
            {
                //return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 0.25f, 0.25f);
            datas = pixs.Data;
            datad = pixd.Data;
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;

            if ((sumtab = MakeSumTabSG4()) == null)
            {
                //return (PIX*)ERROR_PTR("sumtab not made", procName, null);
            }
            if ((valtab = MakeValTabSG4()) == null)
            {
                //return (PIX*)ERROR_PTR("valtab not made", procName, null);
            }

            ScaleToGray4Low(ref datad, wd, hd, wpld, datas, wpls, sumtab, valtab);

            return pixd;
        }

        /// <summary>
        /// Scale to gray 4x.
        /// </summary>
        void ScaleToGray4Low(ref uint[] datad, int wd, int hd, int wpld, uint[] datas, int wpls, uint[] sumtab, byte[] valtab)
        {
            int i, j, l, k;
            uint sbyte1, sbyte2, sbyte3, sbyte4, sum;
            int indexs, indexd;

            for (i = 0, l = 0; i < hd; i++, l += 4)
            {
                indexs = l * wpls;
                indexd = i * wpld;
                for (j = 0, k = 0; j < wd; j += 2, k++)
                {
                    sbyte1 = JBIG2Statics.GetDataByte(datas, indexs + k);
                    sbyte2 = JBIG2Statics.GetDataByte(datas, indexs + wpls + k);
                    sbyte3 = JBIG2Statics.GetDataByte(datas, indexs + 2 * wpls + k);
                    sbyte4 = JBIG2Statics.GetDataByte(datas, indexs + 3 * wpls + k);
                    sum = sumtab[sbyte1] + sumtab[sbyte2] + sumtab[sbyte3] + sumtab[sbyte4];
                    JBIG2Statics.SetDataByte(ref datad, indexd + j, valtab[JBIG2Statics.GetDataByte(sum, 2)]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j + 1, valtab[JBIG2Statics.GetDataByte(sum, 3)]);
                }
            }
        }
        
        /// <summary>
        /// 8 bit value for the sum of ON pixels in a 4x4 square.
        /// </summary>
        byte[] MakeValTabSG4()
        {
            int i;
            byte[] tab = new byte[17];

            for (i = 0; i < 17; i++)
                tab[i] = (byte)(0xff - (i * 255) / 16);

            return tab;
        }

        /// <summary>
        /// Table of 256 uint.
        /// </summary>
        uint[] MakeSumTabSG4()
        {
            int i;
            uint[] sum = { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };
            uint[] tab = new uint[256];

            for (i = 0; i < 256; i++)
                tab[i] = (sum[i & 0xf]) | (sum[(i >> 4) & 0xf] << 8);

            return tab;
        }

        /// <summary>
        /// Scale-to-gray (1 bpp --> 8 bpp; integer downscaling).
        /// </summary>
        private Pix PixScaleToGray8(Pix pixs)
        {
            short[] valtab;
            int ws, hs, wd, hd;
            int wpld, wpls;
            int[] tab8;
            uint[] datas, datad;
            Pix pixd = null;

            if (pixs.D != 1)
            {
                //return (PIX*)ERROR_PTR("pixs must be 1 bpp", procName, null);
            }
            ws = pixs.W; hs = pixs.H;
            wd = ws / 8;  /* truncate to nearest dest byte */
            hd = hs / 8;
            if (wd == 0 || hd == 0)
            {
                //return (PIX*)ERROR_PTR("pixs too small", procName, null);
            }

            if ((pixd = PixCreate(wd, hd, 8)) == null)
            {
                //return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 0.125f, 0.125f);
            datas = pixs.Data;
            datad = pixd.Data;
            wpls = pixs.Wpl;
            wpld = pixd.Wpl;

            if ((tab8 = MakePixelSumTab8()) == null)
            {
                //   return (PIX*)ERROR_PTR("tab8 not made", procName, null);
            }
            if ((valtab = MakeValTabSG8()) == null)
            {
                //   return (PIX*)ERROR_PTR("valtab not made", procName, null);
            }

            ScaleToGray8Low(ref datad, wd, hd, wpld, datas, wpls, tab8, valtab);

            tab8 = new int[1];
            valtab = new short[0];
            return pixd;
        }

        /// <summary>
        /// Scale to gray 8x.
        /// </summary>
        void ScaleToGray8Low(ref uint[] datad, int wd, int hd, int wpld, uint[] datas, int wpls, int[] tab8, short[] valtab)
        {
            int i, j, k;
            uint sbyte0, sbyte1, sbyte2, sbyte3, sbyte4, sbyte5, sbyte6, sbyte7, sum;
            int indexs, indexd;

            for (i = 0, k = 0; i < hd; i++, k += 8)
            {
                indexs = k * wpls;
                indexd = i * wpld;
                for (j = 0; j < wd; j++)
                {
                    sbyte0 = JBIG2Statics.GetDataByte(datas, indexs + j);
                    sbyte1 = JBIG2Statics.GetDataByte(datas, indexs + wpls + j);
                    sbyte2 = JBIG2Statics.GetDataByte(datas, indexs + 2 * wpls + j);
                    sbyte3 = JBIG2Statics.GetDataByte(datas, indexs + 3 * wpls + j);
                    sbyte4 = JBIG2Statics.GetDataByte(datas, indexs + 4 * wpls + j);
                    sbyte5 = JBIG2Statics.GetDataByte(datas, indexs + 5 * wpls + j);
                    sbyte6 = JBIG2Statics.GetDataByte(datas, indexs + 6 * wpls + j);
                    sbyte7 = JBIG2Statics.GetDataByte(datas, indexs + 7 * wpls + j);
                    sum = (uint)(tab8[sbyte0] + tab8[sbyte1] + tab8[sbyte2] + tab8[sbyte3] + tab8[sbyte4] + tab8[sbyte5] + tab8[sbyte6] + tab8[sbyte7]);
                    JBIG2Statics.SetDataByte(ref datad, indexd + j, (uint)valtab[sum]);
                }
            }
        }

        /// <summary>
        /// Scale to gray 8x.
        /// </summary>
        short[] MakeValTabSG8()
        {
            int i;
            short[] tab = new short[65];

            for (i = 0; i < 65; i++)
                tab[i] = (short)(0xff - (i * 255) / 64);

            return tab;
        }
        
        /// <summary>
        /// Table of integers.
        /// </summary>
        private int[] MakePixelSumTab8()
        {
            short bytes;
            int i;
            int[] tab = new int[256];

            for (i = 0; i < 256; i++)
            {
                bytes = (short)i;
                tab[i] = (bytes & 0x1) +
                         ((bytes >> 1) & 0x1) +
                         ((bytes >> 2) & 0x1) +
                         ((bytes >> 3) & 0x1) +
                         ((bytes >> 4) & 0x1) +
                         ((bytes >> 5) & 0x1) +
                         ((bytes >> 6) & 0x1) +
                         ((bytes >> 7) & 0x1);
            }

            return tab;
        }

        /// <summary>
        /// Conversion from 16 bpp to 8 bpp.
        /// </summary>
        private Pix PixConvert16To8(Pix pixs, int whichbyte)
        {
            int w, h, wpls, wpld, i, j, indexs = 0, indexd = 0;
            uint sword, dsword;
            uint[] datas, datad;
            Pix pixd = null;

            if (pixs.D != 16)
            {
                //    return (PIX*)ERROR_PTR("pixs not 16 bpp", procName, null);
            }

            w = pixs.W; h = pixs.H;
            if ((pixd = PixCreate(w, h, 8)) == null)
            {
                //   return (PIX*)ERROR_PTR("pixd not made", procName, null);
            }
            PixCopyResolution(pixd, pixs);
            wpls = pixs.Wpl;
            datas = pixs.Data;
            wpld = pixd.Wpl;
            datad = pixd.Data;

            for (i = 0; i < h; i++)
            {
                indexs = i * wpls;
                indexd = i * wpld;
                if (whichbyte == 0)
                {
                    for (j = 0; j < wpls; j++)
                    {
                        sword = datas[indexs + j];
                        dsword = ((sword >> 8) & 0xff00) | (sword & 0xff);
                        JBIG2Statics.SetDataTwoBytes(ref datad, indexd + j, (short)dsword);
                    }
                }
                else
                { 
                    for (j = 0; j < wpls; j++)
                    {
                        sword = datas[indexs + j];
                        dsword = ((sword >> 16) & 0xff00) | ((sword >> 8) & 0xff);
                        JBIG2Statics.SetDataTwoBytes(ref datad, indexd + j, (short)dsword);
                    }
                }
            }
            pixd.Data = datad;

            return pixd;
        }

        /// <summary>
        /// Binary morphological (raster) ops with brick Sels.
        /// </summary>
        private Pix PixErodeBrick(Pix pixd, Pix pixs, int hsize, int vsize)
        {
            Pix pixt;
            Sel sel, selh, selv;

            if (pixs.D != 1)
            {
                Console.WriteLine("pixs not 1 bpp");
                return null;
            }
            if (hsize < 1 || vsize < 1)
            {
                Console.WriteLine("hsize and vsize not >= 1");
                return null;
            }

            if (hsize == 1 && vsize == 1)
                return PixCopy(pixd, pixs);
            if (hsize == 1 || vsize == 1)
            {
                sel = SelCreateBrick(vsize, hsize, vsize / 2, hsize / 2, 1);
                pixd = PixErode(pixd, pixs, sel);
                sel = null;
            }
            else
            {
                selh = SelCreateBrick(1, hsize, 0, hsize / 2, 1);
                selv = SelCreateBrick(vsize, 1, vsize / 2, 0, 1);
                pixt = PixErode(null, pixs, selh);
                pixd = PixErode(pixd, pixt, selv);
                pixt = null;
                selh = null;
                selv = null;
            }

            return pixd;
        }

        /// <summary>
        /// Erodes source Pix using hits in Sel.
        /// </summary>
        private Pix PixErode(Pix pixd, Pix pixs, Sel sel)
        {
            int i, j, w, h, sx, sy, cx, cy, seldata = 0;
            int xp = 0, yp = 0, xn = 0, yn = 0;
            Pix pixt = null;

            pixd = ProcessMorphArgs1(pixd, pixs, sel, ref pixt);
            if (pixd == null)
                return null;

            w = pixs.W; h = pixs.H;
            sy = sel.SY; sx = sel.SX; cy = sel.CY; cx = sel.CX;

            PixSetAll(pixd);
            for (i = 0; i < sy; i++)
            {
                for (j = 0; j < sx; j++)
                {
                    seldata = sel.Data[i][j];
                    if (seldata == 1)
                        PixRasterop(pixd, cx - j, cy - i, w, h, JBIG2Statics.PixSrc & JBIG2Statics.PixDst, pixt, 0, 0);
                }
            }

            if (MORPH_BC == 1)
            {
                SelFindMaxTranslations(sel, xp, yp, xn, yn);
                if (xp > 0)
                    PixRasterop(pixd, 0, 0, xp, h, JBIG2Statics.PixClr, null, 0, 0);
                if (xn > 0)
                    PixRasterop(pixd, w - xn, 0, xn, h, JBIG2Statics.PixClr, null, 0, 0);
                if (yp > 0)
                    PixRasterop(pixd, 0, 0, w, yp, JBIG2Statics.PixClr, null, 0, 0);
                if (yn > 0)
                    PixRasterop(pixd, 0, h - yn, w, yn, JBIG2Statics.PixClr, null, 0, 0);
            }

            pixt = null;
            return pixd;
        }

        /// <summary>
        /// Max translations for erosion and hmt.
        /// </summary>
        private void SelFindMaxTranslations(Sel sel, int pxp, int pyp, int pxn, int pyn)
        {
            int sx, sy, cx, cy, i, j;
            int maxxp, maxyp, maxxn, maxyn;

            pxp = pyp = pxn = pyn = 0;
            sy = sel.SY; sx = sel.SX; cy = sel.CY; cx = sel.CX;

            maxxp = maxyp = maxxn = maxyn = 0;
            for (i = 0; i < sy; i++)
            {
                for (j = 0; j < sx; j++)
                {
                    if (sel.Data[i][j] == 1)
                    {
                        maxxp = Math.Max(maxxp, cx - j);
                        maxyp = Math.Max(maxyp, cy - i);
                        maxxn = Math.Max(maxxn, j - cx);
                        maxyn = Math.Max(maxyn, i - cy);
                    }
                }
            }

            pxp = maxxp;
            pyp = maxyp;
            pxn = maxxn;
            pyn = maxyn;
        }

        /// <summary>
        /// Sets all data to 1.
        /// </summary>
        void PixSetAll(Pix pix)
        {
            int n;
            PixColormap cmap;
            int PixSet = 0xf << 1;

            if ((cmap = pix.Colormap) != null)
            {
                n = cmap.N;
                if (n < cmap.Nalloc)
                    return;
            }

            PixRasterop(pix, 0, 0, pix.W, pix.H, PixSet, null, 0, 0);
        }

        /// <summary>
        /// Helpers for arg processing.
        /// </summary>
        private Pix ProcessMorphArgs1(Pix pixd, Pix pixs, Sel sel, ref Pix ppixt)
        {
            int sx, sy;

            ppixt = null;
            if (pixs.D != 1)
            {
                Console.WriteLine("pixs not 1 bpp");
                return null;
            }

            sx = sel.SX; sy = sel.SY;

            if (pixd == null)
            {
                if ((pixd = PixCreateTemplate(pixs)) == null)
                {
                    Console.WriteLine("pixd not made");
                    return null;
                }
                ppixt = pixs;
            }
            else
            {
                PixResizeImageData(pixd, pixs);
                if (pixd == pixs)
                {
                    if ((ppixt = PixCopy(null, pixs)) == null)
                    {
                        Console.WriteLine("pixt not made"); return null;
                    }
                }
                else
                    ppixt = pixs;
            }
            return pixd;
        }

        /// <summary>
        /// Reallocate image data if sizes are different.
        /// </summary>
        private int PixResizeImageData(Pix pixd, Pix pixs)
        {
            int w, h, d, wpl, bytes;
            int[] data = new int[] { };

            if (pixs == pixd)
                return 0;
            if ((pixs.W == pixd.W) || (pixs.H == pixd.H) || (pixs.D == pixd.D))
                return 0;
            w = pixs.W; h = pixs.H; d = pixs.D;
            wpl = pixs.Wpl;
            pixd.W = w;
            pixd.H = h;
            pixd.D = d;
            pixd.Wpl = wpl;
            bytes = 4 * wpl * h;

            pixd.Data = new uint[bytes];
            return 0;
        }

        /// <summary>
        /// Binary morphological (raster) ops with brick Sels.
        /// </summary>
        private Pix PixDilateBrick(Pix pixd, Pix pixs, int hsize, int vsize)
        {
            Pix pixt;
            Sel sel, selh, selv;

            if (hsize == 1 && vsize == 1)
                return PixCopy(pixd, pixs);
            if (hsize == 1 || vsize == 1)
            {
                sel = SelCreateBrick(vsize, hsize, vsize / 2, hsize / 2, JBIG2Statics.SelHit);
                pixd = PixDilate(pixd, pixs, sel);
                sel = null;
            }
            else
            {
                selh = SelCreateBrick(1, hsize, 0, hsize / 2, 1);
                selv = SelCreateBrick(vsize, 1, vsize / 2, 0, 1);
                pixt = PixDilate(null, pixs, selh);
                pixd = PixDilate(pixd, pixt, selv);
                pixt = null;
                selh = null;
                selv = null;
            }

            return pixd;
        }

        /// <summary>
        /// Creates rectangular sel of all hits, misses or don't cares.
        /// </summary>
        private Sel SelCreateBrick(int h, int w, int cy, int cx, int type)
        {
            int i, j;
            Sel sel = null;

            if (h <= 0 || w <= 0)
                return null;
            if (type != JBIG2Statics.SelHit && type != JBIG2Statics.SelMiss && type != JBIG2Statics.SelDontCare)
                return null;

            sel = JBIG2Statics.CreateSel(h, w, null);
            sel.CY = cy; sel.CX = cx;
            for (i = 0; i < h; i++)
                for (j = 0; j < w; j++)
                    sel.Data[i][j] = type;

            return sel;
        }

        /// <summary>
        /// Generic binary morphological ops implemented with rasterop.
        /// </summary>
        private Pix PixDilate(Pix pixd, Pix pixs, Sel sel)
        {
            int i, j, w, h, sx, sy, cx, cy, seldata = 0;
            Pix pixt = null;

            if ((pixd = ProcessMorphArgs1(pixd, pixs, sel, ref pixt)) == null)
            {
                Console.WriteLine("processMorphArgs1 failed");
                return null;
            }

            w = pixs.W; h = pixs.H;
            sy = sel.SY; sx = sel.SX; cy = sel.CY; cx = sel.CX;
            PixClearAll(ref pixd);
            for (i = 0; i < sy; i++)
            {
                for (j = 0; j < sx; j++)
                {
                    seldata = sel.Data[i][j];
                    if (seldata == 1)
                        PixRasterop(pixd, j - cx, i - cy, w, h, JBIG2Statics.PixSrc | JBIG2Statics.PixDst, pixt, 0, 0);
                }
            }

            return pixd;
        }

        /// <summary>
        /// Full image clear to arbitrary value.
        /// </summary>
        private void PixClearAll(ref Pix pix)
        {
            PixRasterop(pix, 0, 0, pix.W, pix.H, JBIG2Statics.PixClr, null, 0, 0);
        }

        /// <summary>
        /// Parser verifier for binary morphological operations.
        /// </summary>
        private int MorphSequenceVerify(Sarray sa)
        {
            string rawop;
            char[] op;
            int nops, i, j, nred = 0, fact, w, h, netred, border;
            int[] level = new int[4];
            int[] intlogbase2 = { 1, 2, 3, 0, 4 };

            nops = sa.N;
            bool valid = true;
            netred = 0;
            border = 0;
            for (i = 0; i < nops; i++)
            {
                rawop = sa.Array[i];
                op = rawop.Replace(" \n\t", "").ToCharArray();
                switch (op[0])
                {
                    case 'd':
                    case 'D':
                    case 'e':
                    case 'E':
                    case 'o':
                    case 'O':
                    case 'c':
                    case 'C':
                        //if (sscanf(&op[1], "%d.%d", &w, &h) != 2)
                        //{
                        //    //fprintf(stderr, "*** op: %s invalid\n", op);
                        //    valid = false;
                        //    break;
                        //}
                        w = 1; h = 6;
                        if (w <= 0 || h <= 0)
                        {
                            valid = false;
                            break;
                        }
                        break;
                    case 'r':
                    case 'R':
                        nred = op.Length - 1;
                        netred += nred;
                        if (nred < 1 || nred > 4)
                        {
                            valid = false;
                            break;
                        }
                        for (j = 0; j < nred; j++)
                        {
                            level[j] = op[j + 1] - '0';
                            if (level[j] < 1 || level[j] > 4)
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (!valid)
                            break;
                        for (j = 0; j < nred; j++)
                        {
                            level[j] = op[j + 1] - '0';
                        }
                        break;
                    case 'x':
                    case 'X':
                        //if (sscanf(op[1], "%d", fact) != 1)
                        //{
                        //    //fprintf(stderr, "*** op: %s; fact invalid\n", op);
                        //    valid = false;
                        //    break;
                        //}
                        fact = 1;
                        if (fact != 2 && fact != 4 && fact != 8 && fact != 16)
                        {
                            valid = false;
                            break;
                        }
                        netred -= intlogbase2[fact / 4];
                        break;
                    case 'b':
                    case 'B':
                        //if (sscanf(op[1], "%d", fact) != 1)
                        //{
                        //    //fprintf(stderr, "*** op: %s; fact invalid\n", op);
                        //    valid = false;
                        //    break;
                        //}
                        fact = 1;
                        if (i > 0)
                        {
                            valid = false;
                            break;
                        }
                        if (fact < 1)
                        {
                            valid = false;
                            break;
                        }
                        border = fact;
                        break;
                    default:
                        valid = false;
                        break;
                }
                op = null;
            }

            if (border != 0 && netred != 0)
                valid = false;
            return valid == true ? 1 : 0;
        }

        private string StringRemoveChars(string src, char[] remchars)
        {
            char ch;
            char[] dest = new char[src.Length];
            int nsrc, i, k;

            if (remchars == null)
                return src;

            nsrc = src.Length;
            for (i = 0, k = 0; i < nsrc; i++)
            {
                ch = src[i];
                //if (ch != remchars)
                dest[k++] = ch;
            }

            return new string(dest);
        }

        private int SarraySplitString(Sarray sa, char[] str, char separators)
        {
            string cstr;
            char[] substr;
            char[] saveptr;

            cstr = new String(str);
            substr = StrtokSafe(cstr, separators, out saveptr);
            if (substr.Length != 0)
                SarrayAddString(sa, substr, 0);
            while ((substr == StrtokSafe(null, separators, out saveptr)))
                SarrayAddString(sa, substr, 0);
            cstr = null;

            return 0;
        }

        private int SarrayAddString(Sarray sa, char[] str, int copyflag)
        {
            int n;

            n = sa.N;
            if (n >= sa.Nalloc)
                sa.Nalloc *= 2;

            if (sa.Array.Count <= n)
            {
                while (sa.Array.Count < n)
                    sa.Array.Add(string.Empty);
                sa.Array.Add(new string(str));
            }
            else
                sa.Array[n] = new string(str);
            sa.N++;

            return 0;
        }

        private char[] StrtokSafe(string cstr, char seps, out char[] psaveptr)
        {
            char nextc;
            char[] start, substr = new char[] { };
            int istart, i, j, nchars;
            string temp = String.Empty;
            psaveptr = new char[cstr.Length];

            if (cstr == String.Empty)
                start = psaveptr;
            else
                start = cstr.ToCharArray();

            istart = 0;
            if (cstr != string.Empty)
            {
                for (istart = 0; ; istart++)
                {
                    if ((nextc = start[istart]) == '\0')
                    {
                        psaveptr = null; 
                        return null;
                    }
                    temp = new string(new char[] { seps });
                    if (temp.IndexOf(nextc) == -1)
                        break;
                }
            }

            for (i = istart; ; i++)
            {
                if ((nextc = start[i]) == '\0')
                    break;
                temp = new string(new char[] { seps });
                if (temp.IndexOf(nextc) > -1)
                    break;
            }

            nchars = i - istart;
            substr = new char[nchars + 1];
            temp = new string(start);
            temp = temp + (char)istart;
            string temp1 = new string(substr);
            substr = string.Concat(temp1, temp.Substring(0, nchars)).ToCharArray();

            for (j = i; ; j++)
            {
                if ((nextc = start[j]) == '\0')
                {
                    psaveptr = null;
                    break;
                }
                temp = new string(new char[] { seps });
                if (temp.IndexOf(nextc) == -1)
                {
                    temp1 = new string(start);
                    psaveptr = (temp1 + j).ToCharArray();
                    break;
                }
            }

            return substr;
        }

        /// <summary>
        /// Rank filtered binary reductions.
        /// </summary>
        private Pix PixReduceRankBinaryCascade(Pix pixs, int level1, int level2, int level3, int level4)
        {
            Pix pix1, pix2, pix3, pix4;
            short[] tab;

            if (pixs.D != 1)
                return null;
            if (level1 > 4 || level2 > 4 || level3 > 4 || level4 > 4)
                return null;

            if (level1 <= 0)
            {
#if DEBUG
                Console.WriteLine("no reduction because level1 not > 0");
#endif
                return PixCopy(null, pixs);
            }

            tab = MakeSubsampleTab2x();
            pix1 = PixReduceRankBinary2(pixs, level1, tab);
            if (level2 <= 0)
            {
                tab = new short[0];
                return pix1;
            }

            pix2 = PixReduceRankBinary2(pix1, level2, tab);
            pix1 = null;
            if (level3 <= 0)
            {
                tab = new short[0];
                return pix2;
            }

            pix3 = PixReduceRankBinary2(pix2, level3, tab);
            pix2 = null;
            if (level4 <= 0)
            {
                tab = new short[0];
                return pix3;
            }

            pix4 = PixReduceRankBinary2(pix3, level4, tab);
            pix3 = null;
            tab = new short[0];
            return pix4;
        }

        /// <summary>
        /// Pix is downscaled by 2x from source
        /// </summary>
        private Pix PixReduceRankBinary2(Pix pixs, int level, short[] intab)
        {
            short[] tab;
            int ws, hs, wpls, wpld;
            uint[] datas, datad;
            Pix pixd;

            if (level < 1 || level > 4)
                return null;

            if (intab.Length != 0)
                tab = intab;
            else
                tab = MakeSubsampleTab2x();

            ws = pixs.W;
            hs = pixs.H;
            if (hs <= 1)
                return null;
            wpls = pixs.Wpl;
            datas = pixs.Data;

            pixd = PixCreate(ws / 2, hs / 2, 1);
            if (pixd == null)
                return null;           
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 0.5f, 0.5f);
            wpld = pixd.Wpl;
            datad = pixd.Data;

            ReduceRankBinary2Low(ref datad, wpld, datas, hs, wpls, tab, level);
            pixd.Data = datad;
            tab = new short[1];

            return pixd;
        }

        /// <summary>
        /// Low level subsampled reduction
        /// </summary>
        private void ReduceRankBinary2Low(ref uint[] datad, int wpld, uint[] datas, int hs, int wpls, short[] tab, int level)
        {
            int i, id, j, wplsi;
            byte byte0, byte1;
            short shortd;
            uint word1, word2, word3, word4;

            wplsi = Math.Min(wpls, 2 * wpld);
            int indexd = 0, indexs = 0;

            switch (level)
            {
                case 1:
                    for (i = 0, id = 0; i < hs - 1; i += 2, id++)
                    {
                        indexs = i * wpls;
                        indexd = id * wpld;
                        for (j = 0; j < wplsi; j++)
                        {
                            word1 = datas[indexs + j];
                            word2 = datas[indexs + wpls + j];

                            word2 = word1 | word2;
                            word2 = word2 | (word2 << 1);

                            word2 = (word2 & 0xaaaaaaaa);
                            word1 = word2 | (word2 << 7);
                            byte0 = (byte)(word1 >> 24);
                            byte1 = (byte)((word1 >> 8) & 0xff);
                            shortd = (short)((tab[byte0] << 8) | tab[byte1]);
                            JBIG2Statics.SetDataTwoBytes(ref datad, indexd + j, shortd);
                        }
                    }
                    break;

                case 2:
                    for (i = 0, id = 0; i < hs - 1; i += 2, id++)
                    {
                        indexs = i * wpls;
                        indexd = id * wpld;
                        for (j = 0; j < wplsi; j++)
                        {
                            word1 = datas[indexs + j];
                            word2 = datas[indexs + wpls + j];

                            word3 = word1 & word2;
                            word3 = word3 | (word3 << 1);
                            word4 = word1 | word2;
                            word4 = word4 & (word4 << 1);
                            word2 = word3 | word4;

                            word2 = (word2 & 0xaaaaaaaa);
                            word1 = word2 | (word2 << 7);
                            byte0 = (byte)(word1 >> 24);
                            byte1 = (byte)((word1 >> 8) & 0xff);
                            shortd = (short)((tab[byte0] << 8) | tab[byte1]);
                            JBIG2Statics.SetDataTwoBytes(ref datad, indexd + j, shortd);
                        }
                    }
                    break;

                case 3:
                    for (i = 0, id = 0; i < hs - 1; i += 2, id++)
                    {
                        indexs = i * wpls;
                        indexd = id * wpld;
                        for (j = 0; j < wplsi; j++)
                        {
                            word1 = datas[indexs + j];
                            word2 = datas[indexs + wpls + j];

                            word3 = word1 & word2;
                            word3 = word3 | (word3 << 1);
                            word4 = word1 | word2;
                            word4 = word4 & (word4 << 1);
                            word2 = word3 & word4;

                            word2 = (word2 & 0xaaaaaaaa);  
                            word1 = word2 | (word2 << 7);
                            byte0 = (byte)(word1 >> 24);
                            byte1 = (byte)((word1 >> 8) & 0xff);
                            shortd = (short)((tab[byte0] << 8) | tab[byte1]);
                            JBIG2Statics.SetDataTwoBytes(ref datad, indexd + j, shortd);
                        }
                    }
                    break;

                case 4:
                    for (i = 0, id = 0; i < hs - 1; i += 2, id++)
                    {
                        indexs = i * wpls;
                        indexd = id * wpld;
                        for (j = 0; j < wplsi; j++)
                        {
                            word1 = datas[indexs + j];
                            word2 = datas[indexs + wpls + j];

                            word2 = word1 & word2;
                            word2 = word2 & (word2 << 1);

                            word2 = word2 & 0xaaaaaaaa;
                            word1 = word2 | (word2 << 7); 
                            byte0 = (byte)(word1 >> 24);
                            byte1 = (byte)((word1 >> 8) & 0xff);
                            shortd = (short)((tab[byte0] << 8) | tab[byte1]);
                            JBIG2Statics.SetDataTwoBytes(ref datad, indexd + j, shortd);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// This table permutes the bits in a byte.
        /// </summary>
        private short[] MakeSubsampleTab2x()
        {
            short[] tab = new short[256];

            for (int i = 0; i < 256; i++)
                tab[i] = (short)((i & 0x01) |    /* 7 */
                         ((i & 0x04) >> 1) |    /* 6 */
                         ((i & 0x10) >> 2) |    /* 5 */
                         ((i & 0x40) >> 3) |    /* 4 */
                         ((i & 0x02) << 3) |    /* 3 */
                         ((i & 0x08) << 2) |    /* 2 */
                         ((i & 0x20) << 1) |    /* 1 */
                         (i & 0x80));     /* 0 */

            return tab;
        }

        private Pixa PixaClipToPix(Pixa pixas, Pix pixs)
        {
            int i, n;
            Box box;
            Pix pix, pixc;
            Pixa pixad;

            n = pixas.N;
            if ((pixad = JBIG2Statics.CreatePixa(n)) == null)
            {
                Console.WriteLine("pixad not made"); return null;
            }

            for (i = 0; i < n; i++)
            {
                pix = PixaGetPix(pixas, i, 2);
                box = PixaGetBox(pixas, i, 1);
                pixc = PixClipRectangle(pixs, box, null);
                PixAnd(pixc, pixc, pix);
                PixaAddPix(pixad, pixc, 0);
                PixaAddBox(pixad, box, 0);
                pix = null;
            }

            return pixad;
        }

        private Pix PixAnd(Pix pixd, Pix pixs1, Pix pixs2)
        {
            if ((pixd = PixCopy(pixd, pixs1)) == null)
            { }// return (PIX*)ERROR_PTR("pixd not made", procName, pixd);

            PixRasterop(pixd, 0, 0, pixd.W, pixd.H, JBIG2Statics.PixSrc & JBIG2Statics.PixDst, pixs2, 0, 0);

            return pixd;
        }

        private void PixaAddBox(Pixa pixa, Box box, int copyflag)
        {
            BoxaAddBox(pixa.Boxa, box, copyflag);
        }

        private Box PixaGetBox(Pixa pixa, int index, int accesstype)
        {
            Box box;

            box = pixa.Boxa.Box[index];
            if (box != null)
                return new Box(box.X, box.Y, box.W, box.H);
            else
                return null;
        }

        /// <summary>
        /// Returns pix at the index.
        /// </summary>
        private Pix PixaGetPix(Pixa pixa, int index, int accesstype)
        {
            if (index < 0 || index >= pixa.N)
                return null;

            if (accesstype == 1 || accesstype == 2)
                return PixCopy(null, pixa.Pix[index]);
            else
                return null;
        }

        /// <summary>
        /// Gets bounding boxes or Pixa of the components.
        /// </summary>
        private Boxa PixConnComp(Pix pixs, ref Pixa ppixa, int connectivity)
        {
            if (pixs.D != 1)
                return null;
            if (connectivity != 4 && connectivity != 8)
                return null;

            if (ppixa != null)
                return PixConnCompBB(pixs, connectivity);
            else
                return PixConnCompPixa(pixs, ref ppixa, connectivity);
        }

        private Boxa PixConnCompPixa(Pix pixs, ref Pixa ppixa, int connectivity)
        {
            int h, iszero = 0;
            int x = 0, y = 0, xstart, ystart;
            Pix pixt1 = null, pixt2 = null, pixt3, pixt4;
            Pixa pixa;
            Box box;
            Boxa boxa = null;
            L_Stack lstack = null, auxstack;

            if (pixs == null || pixs.D != 1)
                return null;
            if (connectivity != 4 && connectivity != 8)
                return null;

            pixa = JBIG2Statics.CreatePixa(0);
            ppixa = pixa;
            PixZero(pixs, ref iszero);
            if (iszero == 1)
                return JBIG2Statics.CreateBoxa(1); 

            pixt1 = PixCopy(null, pixs);
            pixt2 = PixCopy(null, pixs);

            h = pixs.H;
            lstack = JBIG2Statics.CreateLStack(h);
            auxstack = JBIG2Statics.CreateLStack(0);
            lstack.AuxStack = auxstack;
            boxa = CreateBoxa(0);

            xstart = 0;
            ystart = 0;
            while (true)
            {
                if (NextOnPixelInRaster(pixt1, xstart, ystart, ref x, ref y) == 0)
                    break;

                box = PixSeedfillBB(pixt1, lstack, x, y, connectivity);
                if (box == null)
                {
#if DEBUG
                    Console.WriteLine("box not made");
#endif
                    return null;
                }
                BoxaAddBox(boxa, box, 0);

                pixt3 = PixClipRectangle(pixt1, box, null);
                pixt4 = PixClipRectangle(pixt2, box, null);
                PixXor(pixt3, pixt3, pixt4);
                PixRasterop(pixt2, box.X, box.Y, box.W, box.H, JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst, pixt3, 0, 0);
                PixaAddPix(pixa, pixt3, m_insert);
                pixt4 = null;

                xstart = x;
                ystart = y;
            }

#if DEBUG
            PixCountPixels(pixt1, ref iszero, null);
            Console.WriteLine("Number of remaining pixels = %d\n", iszero);
#endif
            pixa.Boxa = null;
            pixa.Boxa = BoxaCopy(boxa, 2);

            lstack = null;
            pixt1 = null;
            pixt2 = null;

            return boxa;
        }

        private Pix PixXor(Pix pixd, Pix pixs1, Pix pixs2)
        {
            pixd = PixCopy(pixd, pixs1);
            if (pixd == null)
            {
                //return (PIX*)ERROR_PTR("pixd not made", procName, pixd);
            }

            PixRasterop(pixd, 0, 0, pixd.W, pixd.H, JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst, pixs2, 0, 0);

            return pixd;
        }

        /// <summary>
        /// Adds Pix
        /// </summary>
        private void PixaAddPix(Pixa pixa, Pix pix, int copyflag)
        {
            int n;
            Pix pixc = new Pix();

            if (copyflag == 0)
                pixc = pix;
            else if (copyflag == 1 || copyflag == 2)
                pixc = PixCopy(null, pix);

            n = pixa.N;
            if (n >= pixa.Nalloc)
                PixaExtendArray(pixa);
            if (pixa.Pix.Count <= n)
            {
                while (pixa.Pix.Count < n)
                    pixa.Pix.Add(null);
                pixa.Pix.Add(pixc);
            }
            else
                pixa.Pix[n] = pixc;
            pixa.N++;
        }

        /// <summary>
        /// Doubles the size of the pixa and boxa ptr arrays.
        /// </summary>
        void PixaExtendArray(Pixa pixa)
        {
            PixaExtendArrayToSize(pixa, 2 * pixa.Nalloc);
        }

        /// <summary>
        /// Doubles the size of the pixa and boxa ptr arrays.
        /// </summary>
        void PixaExtendArrayToSize(Pixa pixa, int size)
        {
            if (size > pixa.Nalloc)
                pixa.Nalloc = size;
            BoxaExtendArrayToSize(pixa.Boxa, size);
        }

        /// <summary>
        /// Copies Boxa.
        /// </summary>
        private Boxa BoxaCopy(Boxa boxa, int copyflag)
        {
            int i;
            Box boxc;
            if (copyflag == m_clone)
            {
                boxa.RefCount++;
                return boxa;
            }

            Boxa boxac = JBIG2Statics.CreateBoxa(boxa.Nalloc);

            for (i = 0; i < boxa.N; i++)
            {
                boxc = BoxaGetBox(boxa, i, m_clone);
                BoxaAddBox(boxac, boxc, 0);
            }
            return boxac;
        }

        /// <summary>
        /// Returns Box at the specified index.
        /// </summary>
        private Box BoxaGetBox(Boxa boxa, int index, int accessflag)
        {
            if (index < 0 || index >= boxa.N)
                return null;

            Box bx = boxa.Box[index];

            if (accessflag == 1)
                return new Box(bx.W, bx.Y, bx.X, bx.H);

            return bx;
        }

        /// <summary>
        /// Extract retangular region.
        /// </summary>
        private Pix PixClipRectangle(Pix pixs, Box box, Box pboxc)
        {
            int w, h, d, bx, by, bw, bh;
            Box boxc;
            Pix pixd;
            int PixSrc = 0xc << 1;

            w = pixs.W; h = pixs.H; d = pixs.D;
            boxc = BoxClipToRectangle(box, w, h);
            bx = boxc.X; by = boxc.Y; bw = boxc.W; bh = boxc.H;

            pixd = PixCreate(bw, bh, d);
            PixCopyResolution(pixd, pixs);
            PixCopyColormap(pixd, pixs);
            PixRasterop(pixd, 0, 0, bw, bh, PixSrc, pixs, bx, by);

            if (pboxc != null)
                pboxc = boxc;

            return pixd;
        }

        /// <summary>
        /// Clip box to the rectangle
        /// </summary>
        private Box BoxClipToRectangle(Box box, int wi, int hi)
        {
            Box boxd;

            boxd = new Box(box.X, box.Y, box.W, box.H);
            if (boxd.X < 0)
            {
                boxd.W += boxd.X;
                boxd.W = 0;
            }
            if (boxd.Y < 0)
            {
                boxd.H += boxd.Y;
                boxd.Y = 0;
            }
            if (boxd.X + boxd.W > wi)
                boxd.W = wi - boxd.X;
            if (boxd.Y + boxd.H > hi)
                boxd.H = hi - boxd.Y;
            return boxd;
        }

        /// <summary>
        /// Finds bounding boxes of 4 or 8 connected components in a binary image.
        /// </summary>
        private Boxa PixConnCompBB(Pix pixs, int connectivity)
        {
            int h, iszero = 0, x = 0, y = 0, xstart, ystart;
            Pix pixt;
            Box box;
            Boxa boxa;
            L_Stack lstack, auxstack;

            PixZero(pixs, ref iszero);
            if (iszero == 0)
                return JBIG2Statics.CreateBoxa(1);

            pixt = PixCopy(null, pixs);
            h = pixs.H;
            lstack = JBIG2Statics.CreateLStack(h);
            auxstack = JBIG2Statics.CreateLStack(0);
            lstack.AuxStack = auxstack;
            boxa = JBIG2Statics.CreateBoxa(0);

            xstart = 0;
            ystart = 0;
            while (true)
            {
                if (NextOnPixelInRaster(pixt, xstart, ystart, ref x, ref y) > 0)
                    break;

                if ((box = PixSeedfillBB(pixt, lstack, x, y, connectivity)) == null)
                {
#if DEBUG
                    Console.WriteLine("box not made"); 
#endif
                    return null;
                }
                BoxaAddBox(boxa, box, 0);

                xstart = x;
                ystart = y;
            }

#if DEBUG
            PixCountPixels(pixt, ref iszero, null);
            Console.WriteLine("Number of remaining pixels = %d\n", iszero);
#endif

            lstack = null;
            pixt = null;

            return boxa;
        }

        /// <summary>
        /// Adds Box.
        /// </summary>
        void BoxaAddBox(Boxa boxa, Box box, int copyflag)
        {
            int n;
            Box boxc = null;

            if (copyflag == 0)
                boxc = box;
            else if (copyflag == 1 || copyflag == 2)
                boxc = new Box(box.X, box.Y, box.W, box.H);
            else
                return;
            if (boxc == null)
                return;

            n = boxa.N;
            if (n >= boxa.Nalloc)
                BoxaExtendArray(boxa);
            if (boxa.Box.Count < n)
                boxa.Box[n] = boxc;
            else
            {
                while (boxa.Box.Count <= n)
                    boxa.Box.Add(null);
                boxa.Box[n] = boxc;
            }
            boxa.N++;
        }

        void BoxaExtendArray(Boxa boxa)
        {
            BoxaExtendArrayToSize(boxa, 2 * boxa.Nalloc);
        }

        /// <summary>
        /// Doubles the size of the boxa ptr array.
        /// </summary>
        void BoxaExtendArrayToSize(Boxa boxa, int size)
        {
            if (size > boxa.Nalloc)
                boxa.Nalloc = size;
        }

        /// <summary>
        /// Stack based Seed fill algorithm.
        /// </summary>
        Box PixSeedfillBB(Pix pixs, L_Stack lstack, int x, int y, int connectivity)
        {
            Box box = null;

            if (pixs == null || pixs.D != 1)
                return null;
            if (connectivity != 4 && connectivity != 8)
                return null;

            if (connectivity == 4)
            {
                if ((box = PixSeedfill4BB(pixs, lstack, x, y)) == null)
                    return null;
            }
            else if (connectivity == 8)
            {
                if ((box = PixSeedfill8BB(pixs, lstack, x, y)) == null)
                    return null;
            }
            else
                return null;

            return box;
        }

        /// <summary>
        /// Stack based 4 connected components seedfill algorithm.
        /// </summary>
        private Box PixSeedfill4BB(Pix pixs, L_Stack lstack, int x, int y)
        {
            int w, h, wpl, x1 = 0, x2 = 0, dy = 0, xstart = 0;
            int xmax, ymax;
            int minx, maxx, miny, maxy;
            uint[] data, line;
            Box box;

            if (pixs == null || pixs.D != 1)
                return null;

            w = pixs.W; h = pixs.H;
            xmax = w - 1;
            ymax = h - 1;
            data = pixs.Data;
            wpl = pixs.Wpl;
            int index = y * wpl;
            line = data;

            if (x < 0 || x > xmax || y < 0 || y > ymax || (JBIG2Statics.GetDataBit(data, index, x) == 0))
                return null;

            minx = miny = 100000;
            maxx = maxy = 0;
            PushFillsegBB(lstack, x, x, y, 1, ymax, ref minx, ref maxx, ref miny, ref maxy);
            PushFillsegBB(lstack, x, x, y + 1, -1, ymax, ref minx, ref maxx, ref miny, ref maxy);
            minx = maxx = x;
            miny = maxy = y;

            while (lstack.Array.Count > 0)
            {
                PopFillseg(lstack, ref x1, ref x2, ref y, ref dy);
                index = y * wpl;

                for (x = x1; x >= 0 && (JBIG2Statics.GetDataBit(line, index, x) == 1); x--)
                    JBIG2Statics.ClearDataBit(ref line, index, x);
                bool flag = false;
                if (x >= x1) 
                    flag = true;
                if (!flag)
                {
                    xstart = x + 1;
                    if (xstart < x1 - 1)
                        PushFillsegBB(lstack, xstart, x1 - 1, y, -dy, ymax, ref minx, ref maxx, ref miny, ref maxy);

                    x = x1 + 1;
                }
                do
                {
                    if (!flag)
                    {
                        for (; x <= xmax && (JBIG2Statics.GetDataBit(line, index, x) == 1); x++)
                            JBIG2Statics.ClearDataBit(ref line, index, x);
                        PushFillsegBB(lstack, xstart, x - 1, y, dy, ymax, ref minx, ref maxx, ref miny, ref maxy);
                        if (x > x2 + 1) 
                            PushFillsegBB(lstack, x2 + 1, x - 1, y, -dy, ymax, ref minx, ref maxx, ref miny, ref maxy);
                    }
                    for (x++; x <= x2 && x <= xmax && (JBIG2Statics.GetDataBit(line, index, x) == 0); x++) ;
                    xstart = x;
                    flag = false;
                } while (x <= x2 && x <= xmax);
            }

            if ((box = BoxCreate(minx, miny, maxx - minx + 1, maxy - miny + 1)) == null)
                return null;
            return box;
        }

        /// <summary>
        /// Stack based 8 connected component seedfill algorithm.
        /// </summary>
        private Box PixSeedfill8BB(Pix pixs, L_Stack lstack, int x, int y)
        {
            int w, h, index, xstart = 0, wpl, x1 = 0, x2 = 0, dy = 0;
            int xmax, ymax;
            int minx, maxx, miny, maxy; 
            uint[] data, line;
            Box box;

            if (pixs == null || pixs.D != 1)
                return null;

            w = pixs.W; h = pixs.H;
            xmax = w - 1;
            ymax = h - 1;
            data = pixs.Data;
            wpl = pixs.Wpl;
            line = data;
            index = y * wpl;

            if (x < 0 || x > xmax || y < 0 || y > ymax || (JBIG2Statics.GetDataBit(line, index, x) == 0))
                return null;

            minx = miny = 100000;
            maxx = maxy = 0;
            PushFillsegBB(lstack, x, x, y, 1, ymax, ref minx, ref maxx, ref miny, ref maxy);
            PushFillsegBB(lstack, x, x, y + 1, -1, ymax, ref minx, ref maxx, ref miny, ref maxy);
            minx = maxx = x;
            miny = maxy = y;

            while (lstack.Array.Count > 0)
            {
                PopFillseg(lstack, ref x1, ref x2, ref y, ref dy);
                index = y * wpl;

                for (x = x1 - 1; x >= 0 && (JBIG2Statics.GetDataBit(line, index, x) == 1); x--)
                    JBIG2Statics.ClearDataBit(ref line, index, x);
                bool flag = false;
                if (x >= x1 - 1)
                    flag = true;
                if (!flag)
                {
                    xstart = x + 1;
                    if (xstart < x1)
                        PushFillsegBB(lstack, xstart, x1 - 1, y, -dy, ymax, ref minx, ref maxx, ref miny, ref maxy);

                    x = x1;
                }
                do
                {
                    if (!flag)
                    {
                        for (; x <= xmax && (JBIG2Statics.GetDataBit(line, index, x) == 1); x++)
                            JBIG2Statics.ClearDataBit(ref line, index, x);
                        PushFillsegBB(lstack, xstart, x - 1, y, dy, ymax, ref minx, ref maxx, ref miny, ref maxy);
                        if (x > x2)
                            PushFillsegBB(lstack, x2 + 1, x - 1, y, -dy, ymax, ref minx, ref maxx, ref miny, ref maxy);
                    }
                    for (x++; x <= x2 + 1 && x <= xmax && (JBIG2Statics.GetDataBit(line, index, x) == 0); x++) ;
                    xstart = x;
                    flag = false;
                } while (x <= x2 + 1 && x <= xmax);
            }

            if ((box = BoxCreate(minx, miny, maxx - minx + 1, maxy - miny + 1)) == null)
                return null;
            return box;
        }

        private Box BoxCreate(int x, int y, int w, int h)
        {
            Box box = null;

            if (w < 0 || h < 0)
            {
                //return (BOX*)ERROR_PTR("w and h not both >= 0", procName, null);
            }
            if (x < 0)
            { 
                w = w + x;
                x = 0;
                if (w <= 0)
                {
                    //return (BOX*)ERROR_PTR("x < 0 and box off +quad", procName, null);
                }
            }
            if (y < 0)
            {
                h = h + y;
                y = 0;
                if (h <= 0)
                {
                    //return (BOX*)ERROR_PTR("y < 0 and box off +quad", procName, null);
                }
            }

            box = new Box(x, y, w, h);
            box.RefCount = 1;

            return box;
        }

        /// <summary>
        /// Removes line segment.
        /// </summary>
        private void PopFillseg(L_Stack lstack, ref int pxleft, ref int pxright, ref int py, ref int pdy)
        {
            FillSeg fseg;
            L_Stack auxstack;

            if (lstack == null)
                return;
            if ((auxstack = lstack.AuxStack) == null)
                return;
            if ((fseg = (FillSeg)LstackRemove(lstack)) == null)
                return;

            pxleft = fseg.XLeft;
            pxright = fseg.XRight;
            py = fseg.Y + fseg.Dy;
            pdy = fseg.Dy;

            LstackAdd(auxstack, fseg);
        }

        /// <summary>
        /// Stack helper functions.
        /// </summary>
        void PushFillsegBB(L_Stack lstack, int xleft, int xright, int y, int dy, int ymax, ref int pminx, ref int pmaxx, ref int pminy, ref int pmaxy)
        {
            FillSeg fseg = null;
            L_Stack auxstack;

            pminx = Math.Min(pminx, xleft);
            pmaxx = Math.Max(pmaxx, xright);
            pminy = Math.Min(pminy, y);
            pmaxy = Math.Max(pmaxy, y);

            if (y + dy >= 0 && y + dy <= ymax)
            {
                if ((auxstack = lstack.AuxStack) == null)
                    return;

                if (lstack.Array.Contains(auxstack))
                    fseg = (FillSeg)LstackRemove(auxstack);
                else
                    fseg = new FillSeg();

                fseg.XLeft = xleft;
                fseg.XRight = xright;
                fseg.Y = y;
                fseg.Dy = dy;
                LstackAdd(lstack, fseg);
            }
        }

        private int LstackAdd(L_Stack lstack, Object item)
        {
            /* Do we need to extend the array? */
            //if (lstack.N >= lstack.Nalloc)
            //    lstackExtendArray(lstack);

            lstack.Array.Add(item);
            return lstack.Array.Count;
        }

        private FillSeg LstackRemove(L_Stack lstack)
        {
            if (lstack.Array.Count == 0)
                return null;

            //L_Stack.N--;
            FillSeg item = (FillSeg)lstack.Array[lstack.Array.Count - 1];

            lstack.Array.RemoveAt(lstack.Array.Count - 1);
            return item;
        }

        /// <summary>
        /// Identify the connection component to be erased.
        /// </summary>
        private int NextOnPixelInRaster(Pix pixs, int xstart, int ystart, ref int px, ref int py)
        {
            int w, h, d, wpl;
            uint[] data;

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (d != 1)
            {
#if DEBUG
                Console.WriteLine(string.Format("pixs not 1 bpp x = {0}, y = {1}", px, py));
#endif
                return 0;
            }

            wpl = pixs.Wpl;
            data = pixs.Data;
            return NextOnPixelInRasterLow(data, w, h, wpl, xstart, ystart, ref px, ref py);
        }

        /// <summary>
        /// Identify the connected component to be erased.
        /// </summary>
        private int NextOnPixelInRasterLow(uint[] data, int w, int h, int wpl, int xstart, int ystart, ref int px, ref int py)
        {
            int i, x, y, xend, startword;
            uint pword;
            uint[] line;
            int a = 0;

            line = data;
            /* Look at the first word */
            a = ystart * wpl;
            pword = line[a + (xstart / 32)];
            if (pword > 0)
            {
                xend = xstart - (xstart % 32) + 31;
                for (x = xstart; x <= xend && x < w; x++)
                {
                    if (JBIG2Statics.GetDataBit(line, a, x) == 1)
                    {
                        px = x;
                        py = ystart;
                        return 1;
                    }
                }
            }

            /* Continue with the rest of the line */
            startword = (xstart / 32) + 1;
            x = 32 * startword;
            for (; x < w; startword++, x += 32)
            {
                pword = line[a + startword];
                if (pword > 0)
                {
                    for (i = 0; i < 32 && x < w; i++, x++)
                    {
                        if (JBIG2Statics.GetDataBit(line, a, x) == 1)
                        {
                            px = x;
                            py = ystart;
                            return 1;
                        }
                    }
                }
            }

            /* Continue with following lines */
            for (y = ystart + 1; y < h; y++)
            {
                a = y * wpl;
                for (x = 0; x < w; a++, x += 32)
                {
                    pword = line[a];
                    if (pword > 0)
                    {
                        for (i = 0; i < 32 && x < w; i++, x++)
                        {
                            if (JBIG2Statics.GetDataBit(line, y * wpl, x) == 1)
                            {
                                px = x;
                                py = y;
                                return 1;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Pixel counting.
        /// </summary>
        /// <remarks>For a binary image, if there are no black pixels, returns 1. </remarks>
        /// <remarks>For a grayscale image, if all pixels are black, returns 1. </remarks>
        /// <remarks>For an RGB image, if all 4 components in every pixel is 0, returns 1. </remarks>
        void PixZero(Pix pix, ref int pempty)
        {
            int w, h, wpl, j, fullwords, endbits;
            uint endmask;

            pempty = 0;
            if (pix.Colormap != null)
                throw new Exception("pix is colormapped");

            w = pix.W * pix.D;
            h = pix.H;
            wpl = pix.Wpl;

            fullwords = w / 32;
            endbits = w & 31;
            endmask = 0xffffffff << (32 - endbits);

            int index = 0;
            for (int i = 0; i < h; i++)
            {
                index = wpl * i;
                for (j = 0; j < fullwords; j++)
                {
                    if (pix.Data[index++] != 0)
                    {
                        pempty = 0;
                        return;
                    }
                }
                if (endbits > 0)
                {
                    if ((pix.Data[index] & endmask) != 0)
                    {
                        pempty = 0;
                        return;
                    }
                }
            }
        }

        private Sarray SarrayCreate(int n)
        {
            int INITIAL_PTR_ARRAYSIZE = 50;

            if (n <= 0)
                n = INITIAL_PTR_ARRAYSIZE;

            Sarray sa = new Sarray(n);
            return sa;
        }

        private Pix PixSubtract(Pix pixd, Pix pixs1, Pix pixs2)
        {
            int w, h;

            w = pixs1.W; h = pixs1.H; 
            if (pixd == null)
            {
                pixd = PixCopy(null, pixs1);
                PixRasterop(pixd, 0, 0, w, h, JBIG2Statics.PixDst & JBIG2Statics.PixNot(JBIG2Statics.PixSrc), pixs2, 0, 0);   
            }
            else if (pixd == pixs1)
            {
                PixRasterop(pixd, 0, 0, w, h, JBIG2Statics.PixDst & JBIG2Statics.PixNot(JBIG2Statics.PixSrc), pixs2, 0, 0); 
            }
            else if (pixd == pixs2)
            {
                PixRasterop(pixd, 0, 0, w, h, JBIG2Statics.PixNot(JBIG2Statics.PixDst) & JBIG2Statics.PixSrc, pixs1, 0, 0); 
            }
            else
            {
                PixCopy(pixd, pixs1);
                PixRasterop(pixd, 0, 0, w, h, JBIG2Statics.PixDst & JBIG2Statics.PixNot(JBIG2Statics.PixSrc),
                    pixs2, 0, 0); 
            }

            return pixd;
        }

        /// <summary>
        /// Low level power of 2 binary expansion
        /// </summary>
        private void ExpandBinaryPower2Low(uint[] datad, int wd, int hd, int wpld, uint[] datas, int ws, int hs, int wpls, int factor)
        {
            int i, j, k, sdibits, sqbits, sbytes;
            uint sval;
            short[] tab2, tab4, tab8;
            int indexs = 0, indexd = 0;
            uint[] expandtab16 = { 0x00000000, 0x0000ffff, 0xffff0000, 0xffffffff };

            switch (factor)
            {
                case 2:
                    tab2 = MakeExpandTab2x();
                    sbytes = (ws + 7) / 8;
                    for (i = 0; i < hs; i++)
                    {
                        indexs = i * wpls;
                        indexd = 2 * i * wpld;
                        for (j = 0; j < sbytes; j++)
                        {
                            sval = JBIG2Statics.GetDataByte(datas, j);
                            JBIG2Statics.SetDataTwoBytes(ref datad, indexd + j, tab2[sval]);
                        }

                        Array.Copy(datad, 0, datad, wpld, wpld);
                        uint[] temp = new uint[wpld];
                        Array.Copy(temp, datad, temp.Length);
                    }
                    tab2 = new short[1];
                    break;
                case 4:
                    tab4 = MakeExpandTab4x();
                    sbytes = (ws + 7) / 8;
                    for (i = 0; i < hs; i++)
                    {
                        indexs = i * wpls;
                        indexd = 4 * i * wpld;
                        for (j = 0; j < sbytes; j++)
                        {
                            sval = JBIG2Statics.GetDataByte(datas, indexs + j);
                            datad[indexd + j] = (uint)tab4[sval];
                        }
                        for (k = 1; k < 4; k++)
                        {
                            Array.Copy(datad, 0, datad, k * wpld, k * wpld);
                            uint[] temp = new uint[k * wpld];
                            Array.Copy(temp, datad, temp.Length);
                        }
                    }
                    tab4 = new short[1];
                    break;
                case 8:
                    tab8 = MakeExpandTab8x();
                    sqbits = (ws + 3) / 4;
                    for (i = 0; i < hs; i++)
                    {
                        indexs = i * wpls;
                        indexd = 8 * i * wpld;
                        for (j = 0; j < sqbits; j++)
                        {
                            sval = JBIG2Statics.GetDataQbit(datas[indexs], j);
                            if (sval > 15)
                                Console.WriteLine("sval = %d; should be < 16", sval);
                            datad[indexd + j] = (uint)tab8[sval];
                        }
                        for (k = 1; k < 8; k++)
                        {
                            Array.Copy(datad, 0, datad, k * wpld, k * wpld);
                            uint[] temp = new uint[k * wpld];
                            Array.Copy(temp, datad, temp.Length);
                        }
                    }
                    tab8 = new short[1];
                    break;
                case 16:
                    sdibits = (ws + 1) / 2;
                    for (i = 0; i < hs; i++)
                    {
                        indexs = i * wpls;
                        indexd = 16 * i * wpld;
                        for (j = 0; j < sdibits; j++)
                        {
                            sval = JBIG2Statics.GetDataDibit(datas[indexs], j);
                            datad[indexd + j] = expandtab16[sval];
                        }
                        for (k = 1; k < 16; k++)
                        {
                            Array.Copy(datad, 0, datad, k * wpld, k * wpld);
                            uint[] temp = new uint[k * wpld];
                            Array.Copy(temp, datad, temp.Length);
                        }
                    }
                    break;
                default:
                    Console.WriteLine("expansion factor not in {2,4,8,16}");
                    break;
            }
        }

        /// <summary>
        /// Expansion tables for 8x expansion.
        /// </summary>
        short[] MakeExpandTab8x()
        {
            short[] tab = new short[16];

            for (int i = 0; i < 16; i++)
            {
                if ((i & 0x01) != 0)
                    tab[i] = 0xff;
                if ((i & 0x02) != 0)
                    tab[i] = (short)(tab[i] | 0xff00);
                if ((i & 0x04) != 0)
                    tab[i] = (short)(tab[i] | 0xff0000);
                if ((i & 0x08) != 0)
                    tab[i] = (short)(tab[i] | 0xff000000);
            }

            return tab;
        }

        /// <summary>
        /// Expansion tables for 2x expansion.
        /// </summary>
        short[] MakeExpandTab2x()
        {
            short[] tab = new short[256];

            for (int i = 0; i < 256; i++)
            {
                if ((i & 0x01) != 0)
                    tab[i] = 0x3;
                if ((i & 0x02) != 0)
                    tab[i] |= 0xc;
                if ((i & 0x04) != 0)
                    tab[i] |= 0x30;
                if ((i & 0x08) != 0)
                    tab[i] |= 0xc0;
                if ((i & 0x10) != 0)
                    tab[i] |= 0x300;
                if ((i & 0x20) != 0)
                    tab[i] |= 0xc00;
                if ((i & 0x40) != 0)
                    tab[i] |= 0x3000;
                if ((i & 0x80) != 0)
                    tab[i] = (short)(tab[i] | 0xc000);
            }

            return tab;
        }

        /// <summary>
        /// Expansion tables for 4x expansion.
        /// </summary>
        private short[] MakeExpandTab4x()
        {
            short[] tab = new short[256];

            for (int i = 0; i < 256; i++)
            {
                if ((i & 0x01) != 0)
                    tab[i] = 0xf;
                if ((i & 0x02) != 0)
                    tab[i] |= 0xf0;
                if ((i & 0x04) != 0)
                    tab[i] |= 0xf00;
                if ((i & 0x08) != 0)
                    tab[i] = (short)(tab[i] | 0xf000);
                if ((i & 0x10) != 0)
                    tab[i] = (short)(tab[i] | 0xf0000);
                if ((i & 0x20) != 0)
                    tab[i] = (short)(tab[i] | 0xf00000);
                if ((i & 0x40) != 0)
                    tab[i] = (short)(tab[i] | 0xf000000);
                if ((i & 0x80) != 0)
                    tab[i] = (short)(tab[i] | 0xf0000000);
            }

            return tab;
        }

        /// <summary>
        /// Pixelwise binarization with fixed threshold.
        /// </summary>
        private Pix PixThresholdToBinary(Pix pixs, int thresh)
        {
            int d, w, h, wplt, wpld;
            uint[] datat, datad;
            Pix pixt, pixd;

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (d != 4 && d != 8)
            {
#if DEBUG
                Console.WriteLine("pixs must be 4 or 8 bpp");
#endif
                return null;
            }

            pixt = PixRemoveColormap(pixs, m_removeCmapToGrayScale);
            datat = pixt.Data;
            wplt = pixt.Wpl;
            if (pixs.Colormap != null && d == 4)
            {
                d = 8;
                thresh *= 16;
            }

            pixd = PixCreate(w, h, 1);
            PixCopyResolution(pixd, pixs);
            wpld = pixd.Wpl;

            ThresholdToBinaryLow(out datad, w, h, wpld, datat, d, wplt, thresh);
            pixd.Data = datad;

            return pixd;
        }

        /// <summary>
        /// Binarization with fixed threshold.
        /// </summary>
        private void ThresholdToBinaryLow(out uint[] datad, int w, int h, int wpld, uint[] datas, int d, int wpls, int thresh)
        {
            datad = new uint[h * wpld];
            int a = 0, j = 0;
            for (int i = 0; i < h; i++)
            {
                a = i * wpls;
                j = i * wpld;
                ThresholdToBinaryLineLow(ref datad, w, datas, d, thresh, ref a, ref j);
            }
        }

        /// <summary>
        /// Simple pixelwise binarization.
        /// </summary>
        void ThresholdToBinaryLineLow(ref uint[] lined, int w, uint[] lines, int d, int thresh, ref int srcIndex, ref int destIndex)
        {
            int j, k, scount = srcIndex, dcount = destIndex;
            uint sword = 0, dword, gval;

            switch (d)
            {
                case 4:
                    for (j = 0; j + 31 < w; j += 32)
                    {
                        dword = 0;
                        for (k = 0; k < 4; k++)
                        {
                            sword = lines[scount++];
                            dword <<= 8;
                            gval = (sword >> 28) & 0xf;

                            dword |= ((gval - (uint)thresh) >> 24) & 128;
                            gval = (sword >> 24) & 0xf;
                            dword |= ((gval - (uint)thresh) >> 25) & 64;
                            gval = (sword >> 20) & 0xf;
                            dword |= ((gval - (uint)thresh) >> 26) & 32;
                            gval = (sword >> 16) & 0xf;
                            dword |= ((gval - (uint)thresh) >> 27) & 16;
                            gval = (sword >> 12) & 0xf;
                            dword |= ((gval - (uint)thresh) >> 28) & 8;
                            gval = (sword >> 8) & 0xf;
                            dword |= ((gval - (uint)thresh) >> 29) & 4;
                            gval = (sword >> 4) & 0xf;
                            dword |= ((gval - (uint)thresh) >> 30) & 2;
                            gval = sword & 0xf;
                            dword |= ((gval - (uint)thresh) >> 31) & 1;
                        }
                        lined[dcount++] = dword;
                    }

                    srcIndex = scount;
                    destIndex = dcount;

                    if (j < w)
                    {
                        dword = 0;
                        for (; j < w; j++)
                        {
                            if ((j & 7) == 0)
                            {
                                sword = lines[scount++];
                            }
                            gval = (sword >> 28) & 0xf;
                            sword <<= 4;
                            dword |= (((gval - (uint)thresh) >> 31) & 1) << (31 - (j & 31));
                        }
                        lined[dcount] = dword;
                    }
                    break;
                case 8:
                    for (j = 0; j + 31 < w; j += 32)
                    {
                        dword = 0;
                        for (k = 0; k < 8; k++)
                        {
                            sword = lines[scount++];
                            dword <<= 4;
                            gval = (sword >> 24) & 0xff;
                            dword |= ((gval - (uint)thresh) >> 28) & 8;
                            gval = (sword >> 16) & 0xff;
                            dword |= ((gval - (uint)thresh) >> 29) & 4;
                            gval = (sword >> 8) & 0xff;
                            dword |= ((gval - (uint)thresh) >> 30) & 2;
                            gval = sword & 0xff;
                            dword |= ((gval - (uint)thresh) >> 31) & 1;
                        }
                        lined[dcount++] = dword;
                    }

                    if (j < w)
                    {
                        dword = 0;
                        for (; j < w; j++)
                        {
                            if ((j & 3) == 0)
                            {
                                sword = lines[scount++];
                            }
                            gval = (sword >> 24) & 0xff;
                            sword <<= 8;
                            dword |= (((gval - (uint)thresh) >> 31) & 1) << (31 - (j & 31));
                        }
                        lined[dcount] = dword;
                    }
                    break;
                default:
                    break;
            }

            srcIndex = scount;
            destIndex = dcount;
        }

        /// <summary>
        /// 4x upscale Pix, using linear interpolation, followed by thresholding to binary.
        /// </summary>
        private Pix PixScaleGray4xLIThresh(Pix pixs, int thresh)
        {
            int i, j, ws, hs, hsm, wd, hd, wpls, wplb, wpld;
            uint[] datas, datad, lines, lined, lineb;
            Pix pixd = null;

            if (pixs.D != 8)
            {
#if DEBUG
                Console.WriteLine("pixs must be 8 bpp");
#endif
                return null;
            }

            ws = pixs.W; hs = pixs.H;
            wd = 4 * ws;
            hd = 4 * hs;
            hsm = hs - 1;
            datas = pixs.Data;
            wpls = pixs.Wpl;

            wplb = (wd + 3) / 4;
            lineb = new uint[4 * wplb];

            if ((pixd = PixCreate(wd, hd, 1)) == null)
                return null;
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 4.0f, 4.0f);
            wpld = pixd.Wpl;
            datad = pixd.Data;

            int indexs = 0, indexd = 0;
            int s = 0, d = 0;
            /* Do all but last src line */
            for (i = 0; i < hsm; i++)
            {
                indexs = i * wpls; 
                indexd = 4 * i * wpld;
                ScaleGray4xLILineLow(ref lineb, 0, wplb, datas, indexs, ws, wpls, 0);
                for (j = 0; j < 4; j++)
                {
                    d = j * wpld; s = j * wplb;
                    ThresholdToBinaryLineLow(ref datad, wd, lineb, 8, thresh, ref s, ref d);
                }
            }

            /* Do last src line */
            indexs = hsm * wpls;
            indexd = 4 * hsm * wpld;
            ScaleGray4xLILineLow(ref lineb, 0, wplb, datas, indexs, ws, wpls, 1);
            s = d = 0;
            for (j = 0; j < 4; j++)
            {
                d = j * wpld;
                s = j * wplb;
                ThresholdToBinaryLineLow(ref datad, wd, lineb, 8, thresh, ref s, ref d);
            }

            pixd.Data = datad;
            return pixd;
        }

        private void ScaleGray4xLILineLow(ref uint[] lined, int indexd, int wpld, uint[] lines, int indexs, int ws, int wpls, int lastlineflag)
        {
            int j, jd, wsm, wsm4;
            uint s1, s2, s3, s4, s1t, s2t, s3t, s4t;
            int indexsp, indexdp1, indexdp2, indexdp3;

            wsm = ws - 1;
            wsm4 = 4 * wsm;

            if (lastlineflag == 0)
            {
                indexsp = indexs + wpls;
                indexdp1 = indexd + wpld;
                indexdp2 = indexd + 2 * wpld;
                indexdp3 = indexd + 3 * wpld;
                s2 = JBIG2Statics.GetDataByte(lines, indexs + 0);
                s4 = JBIG2Statics.GetDataByte(lines, indexsp + 0);
                for (j = 0, jd = 0; j < wsm; j++, jd += 4)
                {
                    s1 = s2;
                    s3 = s4;
                    s2 = JBIG2Statics.GetDataByte(lines, indexs + j + 1);
                    s4 = JBIG2Statics.GetDataByte(lines, indexsp + j + 1);
                    s1t = 3 * s1;
                    s2t = 3 * s2;
                    s3t = 3 * s3;
                    s4t = 3 * s4;
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd, s1);                             /* d1 */
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd + 1, (s1t + s2) / 4);             /* d2 */
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd + 2, (s1 + s2) / 2);              /* d3 */
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd + 3, (s1 + s2t) / 4);             /* d4 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp1 + jd, (s1t + s3) / 4);                /* d5 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp1 + jd + 1, (9 * s1 + s2t + s3t + s4) / 16); /*d6*/
                    JBIG2Statics.SetDataByte(ref lined, indexdp1 + jd + 2, (s1t + s2t + s3 + s4) / 8); /* d7 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp1 + jd + 3, (s1t + 9 * s2 + s3 + s4t) / 16);/*d8*/
                    JBIG2Statics.SetDataByte(ref lined, indexdp2 + jd, (s1 + s3) / 2);                /* d9 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp2 + jd + 1, (s1t + s2 + s3t + s4) / 8);/* d10 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp2 + jd + 2, (s1 + s2 + s3 + s4) / 4);  /* d11 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp2 + jd + 3, (s1 + s2t + s3 + s4t) / 8);/* d12 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp3 + jd, (s1 + s3t) / 4);               /* d13 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp3 + jd + 1, (s1t + s2 + 9 * s3 + s4t) / 16);/*d14*/
                    JBIG2Statics.SetDataByte(ref lined, indexdp3 + jd + 2, (s1 + s2 + s3t + s4t) / 8); /* d15 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp3 + jd + 3, (s1 + s2t + s3t + 9 * s4) / 16);/*d16*/
                }
                s1 = s2;
                s3 = s4;
                s1t = 3 * s1;
                s3t = 3 * s3;
                JBIG2Statics.SetDataByte(ref lined, indexd + wsm4, s1);                               /* d1 */
                JBIG2Statics.SetDataByte(ref lined, indexd + wsm4 + 1, s1);                           /* d2 */
                JBIG2Statics.SetDataByte(ref lined, indexd + wsm4 + 2, s1);                           /* d3 */
                JBIG2Statics.SetDataByte(ref lined, indexd + wsm4 + 3, s1);                           /* d4 */
                JBIG2Statics.SetDataByte(ref lined, indexdp1 + wsm4, (s1t + s3) / 4);                 /* d5 */
                JBIG2Statics.SetDataByte(ref lined, indexdp1 + wsm4 + 1, (s1t + s3) / 4);             /* d6 */
                JBIG2Statics.SetDataByte(ref lined, indexdp1 + wsm4 + 2, (s1t + s3) / 4);             /* d7 */
                JBIG2Statics.SetDataByte(ref lined, indexdp1 + wsm4 + 3, (s1t + s3) / 4);             /* d8 */
                JBIG2Statics.SetDataByte(ref lined, indexdp2 + wsm4, (s1 + s3) / 2);                  /* d9 */
                JBIG2Statics.SetDataByte(ref lined, indexdp2 + wsm4 + 1, (s1 + s3) / 2);              /* d10 */
                JBIG2Statics.SetDataByte(ref lined, indexdp2 + wsm4 + 2, (s1 + s3) / 2);              /* d11 */
                JBIG2Statics.SetDataByte(ref lined, indexdp2 + wsm4 + 3, (s1 + s3) / 2);              /* d12 */
                JBIG2Statics.SetDataByte(ref lined, indexdp3 + wsm4, (s1 + s3t) / 4);                 /* d13 */
                JBIG2Statics.SetDataByte(ref lined, indexdp3 + wsm4 + 1, (s1 + s3t) / 4);             /* d14 */
                JBIG2Statics.SetDataByte(ref lined, indexdp3 + wsm4 + 2, (s1 + s3t) / 4);             /* d15 */
                JBIG2Statics.SetDataByte(ref lined, indexdp3 + wsm4 + 3, (s1 + s3t) / 4);             /* d16 */
            }
            else
            {   /* last row of src pixels: lastlineflag == 1 */
                indexdp1 = indexd + wpld;
                indexdp2 = indexd + 2 * wpld;
                indexdp3 = indexd + 3 * wpld;
                s2 = JBIG2Statics.GetDataByte(lines, indexs + 0);
                for (j = 0, jd = 0; j < wsm; j++, jd += 4)
                {
                    s1 = s2;
                    s2 = JBIG2Statics.GetDataByte(lines, j + 1);
                    s1t = 3 * s1;
                    s2t = 3 * s2;
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd, s1);                            /* d1 */
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd + 1, (s1t + s2) / 4);           /* d2 */
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd + 2, (s1 + s2) / 2);            /* d3 */
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd + 3, (s1 + s2t) / 4);           /* d4 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp1 + jd, s1);                          /* d5 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp1 + jd + 1, (s1t + s2) / 4);         /* d6 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp1 + jd + 2, (s1 + s2) / 2);          /* d7 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp1 + jd + 3, (s1 + s2t) / 4);         /* d8 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp2 + jd, s1);                          /* d9 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp2 + jd + 1, (s1t + s2) / 4);         /* d10 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp2 + jd + 2, (s1 + s2) / 2);          /* d11 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp2 + jd + 3, (s1 + s2t) / 4);         /* d12 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp3 + jd, s1);                          /* d13 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp3 + jd + 1, (s1t + s2) / 4);         /* d14 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp3 + jd + 2, (s1 + s2) / 2);          /* d15 */
                    JBIG2Statics.SetDataByte(ref lined, indexdp3 + jd + 3, (s1 + s2t) / 4);         /* d16 */
                }
                s1 = s2;
                JBIG2Statics.SetDataByte(ref lined, indexd + wsm4, s1);                              /* d1 */
                JBIG2Statics.SetDataByte(ref lined, indexd + wsm4 + 1, s1);                          /* d2 */
                JBIG2Statics.SetDataByte(ref lined, indexd + wsm4 + 2, s1);                          /* d3 */
                JBIG2Statics.SetDataByte(ref lined, indexd + wsm4 + 3, s1);                          /* d4 */
                JBIG2Statics.SetDataByte(ref lined, indexdp1 + wsm4, s1);                            /* d5 */
                JBIG2Statics.SetDataByte(ref lined, indexdp1 + wsm4 + 1, s1);                        /* d6 */
                JBIG2Statics.SetDataByte(ref lined, indexdp1 + wsm4 + 2, s1);                        /* d7 */
                JBIG2Statics.SetDataByte(ref lined, indexdp1 + wsm4 + 3, s1);                        /* d8 */
                JBIG2Statics.SetDataByte(ref lined, indexdp2 + wsm4, s1);                            /* d9 */
                JBIG2Statics.SetDataByte(ref lined, indexdp2 + wsm4 + 1, s1);                        /* d10 */
                JBIG2Statics.SetDataByte(ref lined, indexdp2 + wsm4 + 2, s1);                        /* d11 */
                JBIG2Statics.SetDataByte(ref lined, indexdp2 + wsm4 + 3, s1);                        /* d12 */
                JBIG2Statics.SetDataByte(ref lined, indexdp3 + wsm4, s1);                            /* d13 */
                JBIG2Statics.SetDataByte(ref lined, indexdp3 + wsm4 + 1, s1);                        /* d14 */
                JBIG2Statics.SetDataByte(ref lined, indexdp3 + wsm4 + 2, s1);                        /* d15 */
                JBIG2Statics.SetDataByte(ref lined, indexdp3 + wsm4 + 3, s1);                        /* d16 */
            }
        }

        /// <summary>
        /// 2x upscale Pix, using linear interpolation, followed by thresholding to binary.
        /// </summary>
        private Pix PixScaleGray2xLIThresh(Pix pixs, int thresh)
        {
            int i, ws, hs, hsm, wd, hd, wpls, wplb, wpld;
            uint[] datas, datad, lineb;
            Pix pixd = null;

            if (pixs.D != 8)
            {
#if DEBUG
                Console.WriteLine("pixs must be 8 bpp");
#endif
                return null;
            }

            ws = pixs.XRes;
            hs = pixs.YRes;
            wd = 2 * ws;
            hd = 2 * hs;
            hsm = hs - 1;
            datas = pixs.Data;
            wpls = pixs.Wpl;

            wplb = (wd + 3) / 4;
            lineb = new uint[2 * wplb];

            pixd = PixCreate(wd, hd, 1);
            PixCopyResolution(pixd, pixs);
            PixScaleResolution(pixd, 2.0f, 2.0f);
            wpld = pixd.Wpl;
            datad = pixd.Data;

            int indexd = 0, indexs = 0;
            int a = 0;

            /* Do all but last src line */
            for (i = 0; i < hsm; i++)
            {
                indexs = i * wpls; 
                indexd = 2 * i * wpld;
                ScaleGray2xLILineLow(ref lineb, indexd, wplb, datas, indexs, ws, wpls, 0);
                ThresholdToBinaryLineLow(ref datad, wd, lineb, 8, thresh, ref a, ref indexd);
                indexd += wpld;
                a += wplb;
                ThresholdToBinaryLineLow(ref datad, wd, lineb, 8, thresh, ref a, ref indexd);
            }

            /* Do last src line */
            a = 0;
            indexs = hsm * wpls;
            indexd = 2 * hsm * wpld;
            ScaleGray2xLILineLow(ref lineb, indexd, wplb, datas, indexs, ws, wpls, 1);
            ThresholdToBinaryLineLow(ref datad, wd, lineb, 8, thresh, ref a, ref indexd);
            indexd += wpld;
            a += wplb;
            ThresholdToBinaryLineLow(ref datad, wd, lineb, 8, thresh, ref a, ref indexd);

            lineb = null;
            pixd.Data = datad;
            return pixd;
        }

        /// <summary>
        /// Grayscale interpolated scaling: 2x upscaling.
        /// </summary>
        private void ScaleGray2xLILineLow(ref uint[] lined, int indexd, int wpld, uint[] lines, int indexs, int ws, int wpls, int lastlineflag)
        {
            int j, jd, wsm, w;
            uint sval1, sval2, sval3, sval4;
            int linesp, linedp; //indexes
            uint words, wordsp, wordd, worddp;

            wsm = ws - 1;

            if (lastlineflag == 0)
            {
                linesp = indexs + wpls;
                linedp = indexd + wpld;

                /* Unroll the loop 4x and work on full words */
                words = lines[indexs];
                wordsp = lines[linesp];
                sval2 = (words >> 24) & 0xff;
                sval4 = (wordsp >> 24) & 0xff;
                for (j = 0, jd = 0, w = 0; j + 3 < wsm; j += 4, jd += 8, w++)
                {
                    sval1 = sval2;
                    sval2 = (words >> 16) & 0xff;
                    sval3 = sval4;
                    sval4 = (wordsp >> 16) & 0xff;
                    wordd = (sval1 << 24) | (((sval1 + sval2) >> 1) << 16);
                    worddp = (((sval1 + sval3) >> 1) << 24) |
                        (((sval1 + sval2 + sval3 + sval4) >> 2) << 16);

                    sval1 = sval2;
                    sval2 = (words >> 8) & 0xff;
                    sval3 = sval4;
                    sval4 = (wordsp >> 8) & 0xff;
                    wordd |= (sval1 << 8) | ((sval1 + sval2) >> 1);
                    worddp |= (((sval1 + sval3) >> 1) << 8) |
                        ((sval1 + sval2 + sval3 + sval4) >> 2);
                    lined[indexd + w * 2] = wordd;
                    lined[linedp + w * 2] = worddp;

                    sval1 = sval2;
                    sval2 = words & 0xff;
                    sval3 = sval4;
                    sval4 = wordsp & 0xff;
                    wordd = (sval1 << 24) |                              /* pix 1 */
                        (((sval1 + sval2) >> 1) << 16);                  /* pix 2 */
                    worddp = (((sval1 + sval3) >> 1) << 24) |            /* pix 3 */
                        (((sval1 + sval2 + sval3 + sval4) >> 2) << 16);  /* pix 4 */

                    /* Load the next word as we need its first byte */
                    words = lines[indexd + w + 1];
                    wordsp = lines[linesp + w + 1];
                    sval1 = sval2;
                    sval2 = (words >> 24) & 0xff;
                    sval3 = sval4;
                    sval4 = (wordsp >> 24) & 0xff;
                    wordd |= (sval1 << 8) |                              /* pix 1 */
                        ((sval1 + sval2) >> 1);                          /* pix 2 */
                    worddp |= (((sval1 + sval3) >> 1) << 8) |            /* pix 3 */
                        ((sval1 + sval2 + sval3 + sval4) >> 2);          /* pix 4 */
                    lined[indexd + w * 2 + 1] = wordd;
                    lined[linedp + w * 2 + 1] = worddp;
                }

                /* Finish up the last word */
                for (; j < wsm; j++, jd += 2)
                {
                    sval1 = sval2;
                    sval3 = sval4;
                    sval2 = JBIG2Statics.GetDataByte(lines, indexs + j + 1);
                    sval4 = JBIG2Statics.GetDataByte(lines, linesp + j + 1);
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd, sval1);                     /* pix 1 */
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd + 1, (sval1 + sval2) / 2);   /* pix 2 */
                    JBIG2Statics.SetDataByte(ref lined, linedp + jd, (sval1 + sval3) / 2);      /* pix 3 */
                    JBIG2Statics.SetDataByte(ref lined, linedp + jd + 1, (sval1 + sval2 + sval3 + sval4) / 4);  /* pix 4 */
                }
                sval1 = sval2;
                sval3 = sval4;
                JBIG2Statics.SetDataByte(ref lined, indexd + 2 * wsm, sval1);                     /* pix 1 */
                JBIG2Statics.SetDataByte(ref lined, indexd + 2 * wsm + 1, sval1);                 /* pix 2 */
                JBIG2Statics.SetDataByte(ref lined, linedp + 2 * wsm, (sval1 + sval3) / 2);      /* pix 3 */
                JBIG2Statics.SetDataByte(ref lined, linedp + 2 * wsm + 1, (sval1 + sval3) / 2);  /* pix 4 */
            }
            else
            {   /* last row of src pixels: lastlineflag == 1 */
                linedp = indexd + wpld;
                sval2 = JBIG2Statics.GetDataByte(lines, indexs + 0);
                for (j = 0, jd = 0; j < wsm; j++, jd += 2)
                {
                    sval1 = sval2;
                    sval2 = JBIG2Statics.GetDataByte(lines, indexs + j + 1);
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd, sval1);                       /* pix 1 */
                    JBIG2Statics.SetDataByte(ref lined, linedp + jd, sval1);                      /* pix 3 */
                    JBIG2Statics.SetDataByte(ref lined, indexd + jd + 1, (sval1 + sval2) / 2);     /* pix 2 */
                    JBIG2Statics.SetDataByte(ref lined, linedp + jd + 1, (sval1 + sval2) / 2);    /* pix 4 */
                }
                sval1 = sval2;
                JBIG2Statics.SetDataByte(ref lined, indexd + 2 * wsm, sval1);                     /* pix 1 */
                JBIG2Statics.SetDataByte(ref lined, indexd + 2 * wsm + 1, sval1);                 /* pix 2 */
                JBIG2Statics.SetDataByte(ref lined, linedp + 2 * wsm, sval1);                    /* pix 3 */
                JBIG2Statics.SetDataByte(ref lined, linedp + 2 * wsm + 1, sval1);                /* pix 4 */
            }
        }

        /// <summary>
        /// Scales resolution of the Pix
        /// </summary>
        void PixScaleResolution(Pix pix, float xscale, float yscale)
        {
            if (pix.XRes != 0 && pix.YRes != 0)
            {
                pix.XRes = (int)(xscale * (float)(pix.XRes) + 0.5);
                pix.YRes = (int)(yscale * (float)(pix.YRes) + 0.5);
            }
        }

        /// <summary>
        /// Removes colormap for the Pix.
        /// </summary>
        private Pix PixRemoveColormap(Pix pixs, int type)
        {
            uint sval, rval, gval, bval;
            int i, j, k, w, h, d, wpls, wpld, ncolors, count;
            bool colorfound = false;
            int[] rmap, gmap, bmap, graymap;
            uint[] datas, datad, lut;
            uint sword, dword;
            PixColormap cmap;
            Pix pixd = null;

            int m_removeCmapToBinary = 0, m_removeCmapToGrayScale = 1, m_removeCmapToFullColor = 2, m_removeCmapBasedOnSrc = 3;

            if (pixs == null)
                throw new NullReferenceException("Pixs not defined");

            if (pixs.Colormap == null)
                return pixs;

            cmap = pixs.Colormap;
            if (type != m_removeCmapToBinary && type != m_removeCmapToGrayScale && type != m_removeCmapToFullColor && type != m_removeCmapBasedOnSrc)
            {
                Console.WriteLine("Invalid type; converting based on src");
                type = m_removeCmapBasedOnSrc;
            }

            w = pixs.W; h = pixs.H; d = pixs.D;
            if (d != 1 && d != 2 && d != 4 && d != 8)
                throw new Exception("Pixs must be {1,2,4,8} bpp");

            rmap = new int[] { }; gmap = new int[] { }; bmap = new int[] { };
            rval = gval = bval = 0;

            if (PixCmapToArrays(cmap, ref rmap, ref gmap, ref bmap))
            {
                Console.WriteLine("colormap arrays not made");
                return null;
            }

            if (d != 1 && type == m_removeCmapToBinary)
            {
                Console.WriteLine("not 1 bpp; can't remove cmap to binary");
                type = m_removeCmapBasedOnSrc;
            }

            if (type == m_removeCmapBasedOnSrc)
            {
                PixCmapHasColor(cmap, ref colorfound);
                if (!colorfound)
                {
                    if (d == 1)
                        type = m_removeCmapToBinary;
                    else
                        type = m_removeCmapToGrayScale;
                }
                else
                    type = m_removeCmapToFullColor;
            }

            ncolors = cmap.N;
            datas = pixs.Data;
            wpls = pixs.Wpl;
            if (type == m_removeCmapToBinary)
            {
                if ((pixd = PixCopy(null, pixs)) == null)
                    throw new NullReferenceException("pixd not made");
                PixCmapGetColor(cmap, 0, (int)rval, (int)gval, (int)bval);
                if (rval == 0)
                    PixInvert(pixd, pixd);
            }
            else if (type == m_removeCmapToGrayScale)
            {
                if ((pixd = PixCreate(w, h, 8)) == null)
                    throw new Exception("pixd not made");
                pixd.XRes = pixs.XRes; pixd.YRes = pixs.YRes;
                datad = pixd.Data;
                wpld = pixd.Wpl;
                graymap = new int[ncolors];

                for (i = 0; i < cmap.N; i++)
                    graymap[i] = (rmap[i] + 2 * gmap[i] + bmap[i]) / 4;
                int indexd = 0;
                int indexs = 0;
                for (i = 0; i < h; i++)
                {
                    indexs = i * wpls;
                    indexd = i * wpld;
                    switch (d)
                    {
                        case 8:
                            for (j = 0, count = 0; j + 3 < w; j += 4, count++)
                            {
                                sword = datas[indexs + count];
                                dword = (uint)((graymap[(sword >> 24) & 0xff] << 24) |
                                    (graymap[(sword >> 16) & 0xff] << 16) | (graymap[(sword >> 8) & 0xff] << 8) | graymap[sword & 0xff]);
                                datad[indexd + count] = dword;
                            }
                            for (; j < w; j++)
                            {
                                sval = JBIG2Statics.GetDataByte(datas, indexs + j);
                                gval = (uint)graymap[sval];
                                JBIG2Statics.SetDataByte(ref datad, indexd + j, gval);
                            }
                            break;
                        case 4:
                            for (j = 0, count = 0; j + 7 < w; j += 8, count++)
                            {
                                sword = datas[indexs + count];
                                dword = (uint)((graymap[(sword >> 28) & 0xf] << 24) |
                                    (graymap[(sword >> 24) & 0xf] << 16) | (graymap[(sword >> 20) & 0xf] << 8) |
                                    graymap[(sword >> 16) & 0xf]);
                                datad[indexd + 2 * count] = dword;
                                dword = (uint)((graymap[(sword >> 12) & 0xf] << 24) |
                                    (graymap[(sword >> 8) & 0xf] << 16) | (graymap[(sword >> 4) & 0xf] << 8) |
                                    graymap[sword & 0xf]);
                                datad[indexd + 2 * count + 1] = dword;
                            }
                            for (; j < w; j++)
                            {
                                sval = JBIG2Statics.GetDataQbit(datas[indexs], indexs + j);
                                gval = (uint)graymap[sval];
                                JBIG2Statics.SetDataByte(ref datad, indexd + j, gval);
                            }
                            break;
                        case 2:
                            /* Unrolled 16x */
                            for (j = 0, count = 0; j + 15 < w; j += 16, count++)
                            {
                                sword = datas[indexs + count];
                                dword = (uint)((graymap[(sword >> 30) & 0x3] << 24) |
                                    (graymap[(sword >> 28) & 0x3] << 16) | (graymap[(sword >> 26) & 0x3] << 8) |
                                    graymap[(sword >> 24) & 0x3]);
                                datad[indexd + 4 * count] = dword;
                                dword = (uint)((graymap[(sword >> 22) & 0x3] << 24) |
                                    (graymap[(sword >> 20) & 0x3] << 16) | (graymap[(sword >> 18) & 0x3] << 8) |
                                    graymap[(sword >> 16) & 0x3]);
                                datad[indexd + 4 * count + 1] = dword;
                                dword = (uint)((graymap[(sword >> 14) & 0x3] << 24) |
                                    (graymap[(sword >> 12) & 0x3] << 16) | (graymap[(sword >> 10) & 0x3] << 8) |
                                    graymap[(sword >> 8) & 0x3]);
                                datad[indexd + 4 * count + 2] = dword;
                                dword = (uint)((graymap[(sword >> 6) & 0x3] << 24) |
                                    (graymap[(sword >> 4) & 0x3] << 16) | (graymap[(sword >> 2) & 0x3] << 8) |
                                    graymap[sword & 0x3]);
                                datad[indexd + 4 * count + 3] = dword;
                            }
                            for (; j < w; j++)
                            {
                                sval = JBIG2Statics.GetDataDibit(datas[indexs], indexs + j);
                                gval = (uint)graymap[sval];
                                JBIG2Statics.SetDataByte(ref datad, indexd + j, gval);
                            }
                            break;
                        case 1:
                            for (j = 0, count = 0; j + 31 < w; j += 32, count++)
                            {
                                sword = datas[indexs + count];
                                for (k = 0; k < 4; k++)
                                {
                                    dword = (uint)((graymap[(sword >> 31) & 0x1] << 24) |
                                        (graymap[(sword >> 30) & 0x1] << 16) | (graymap[(sword >> 29) & 0x1] << 8) |
                                        graymap[(sword >> 28) & 0x1]);
                                    datad[indexd + 8 * count + 2 * k] = dword;
                                    dword = (uint)((graymap[(sword >> 27) & 0x1] << 24) |
                                        (graymap[(sword >> 26) & 0x1] << 16) | (graymap[(sword >> 25) & 0x1] << 8) |
                                        graymap[(sword >> 24) & 0x1]);
                                    datad[indexd + 8 * count + 2 * k + 1] = dword;
                                    sword <<= 8; 
                                }
                            }
                            for (; j < w; j++)
                            {
                                sval = JBIG2Statics.GetDataBit(datas, indexs, indexs + j);
                                gval = (uint)graymap[sval];
                                JBIG2Statics.SetDataByte(ref datad, indexd + j, gval);
                            }
                            break;
                        default:
                            return null;
                    }
                }

                pixd.Data = datad;
                graymap = null;
            }
            else
            {
                if ((pixd = PixCreate(w, h, 32)) == null)
                    throw new Exception("Pixd not made");
                pixd.XRes = pixs.XRes; pixd.YRes = pixs.YRes;
                datad = pixd.Data;
                wpld = pixd.Wpl;
                lut = new uint[ncolors];
                for (i = 0; i < ncolors; i++)
                    ComposeRGBPixel(rmap[i], gmap[i], bmap[i], (int)lut[i]);
                int indexs = 0, indexd = 0;
                for (i = 0; i < h; i++)
                {
                    indexs = i * wpls;
                    indexd = i * wpld;
                    for (j = 0; j < w; j++)
                    {
                        if (d == 8)
                            sval = JBIG2Statics.GetDataByte(datas, indexs + j);
                        else if (d == 4)
                            sval = JBIG2Statics.GetDataQbit(datas[indexs], indexs + j);
                        else if (d == 2)
                            sval = JBIG2Statics.GetDataDibit(datas[indexs], indexs + j);
                        else if (d == 1)
                            sval = JBIG2Statics.GetDataBit(datas, indexs, indexs + j);
                        else
                            return null;
                        if (sval >= ncolors)
                            Console.WriteLine("pixel value out of bounds");
                        else
                            datad[indexd + j] = lut[sval];
                    }
                }

                pixd.Data = datad;
                lut = null;
            }

            rmap = null;
            gmap = null;
            bmap = null;

            return pixd;
        }

        private Pix PixInvert(Pix pixd, Pix pixs)
        {
            int PixDst = (0xa << 1);

            if ((pixd = PixCopy(pixd, pixs)) == null)
                throw new NullReferenceException("pixd not made");

            PixRasterop(pixd, 0, 0, (int)pixd.W, (int)pixd.H, JBIG2Statics.PixNot(PixDst), null, 0, 0);   /* invert pixd */

            return pixd;
        }

        /// <summary>
        /// General raster operation.
        /// </summary>
        private void PixRasterop(Pix pixd, int dx, int dy, int dw, int dh, int op, Pix pixs, int sx, int sy)
        {
            int dd;

            if (op == JBIG2Statics.PixDst)  
                return;

            dd = (int)pixd.D;
            if (op == JBIG2Statics.PixClr || op == JBIG2Statics.PixSet || op == (JBIG2Statics.PixDst ^ 0x1e))
            {
                uint[] temp = pixd.Data;
                RasteropUniLow(ref temp, pixd.W, pixd.H, dd, pixd.Wpl, dx, dy, dw, dh, op);
                pixd.Data = temp;
                return;
            }

            if (dd != pixs.D)
                return;

            uint[] datad = pixd.Data;

            RasteropLow(ref datad, pixd.W, pixd.H, dd, pixd.Wpl, dx, dy, dw, dh, op, pixs.Data, pixs.W, pixs.H, pixs.Wpl, sx, sy);
            pixd.Data = datad;
        }

        /// <summary>
        /// Scales width, performs clipping, checks alignment, and dispatches for the rasterop.
        /// </summary>
        void RasteropLow(ref uint[] datad, int dpixw, int dpixh, int depth, int dwpl, int dx, int dy, int dw, int dh, int op, uint[] datas, int spixw, int spixh, int swpl, int sx, int sy)
        {
            int dhangw, shangw, dhangh, shangh;

            if (depth != 1)
            {
                dpixw *= depth;
                dx *= depth;
                dw *= depth;
                spixw *= depth;
                sx *= depth;
            }

            if (dx < 0)
            {
                sx -= dx; 
                dw += dx; 
                dx = 0;
            }
            if (sx < 0)
            {
                dx -= sx;
                dw += sx;
                sx = 0;
            }
            dhangw = dx + dw - dpixw; 
            if (dhangw > 0)
                dw -= dhangw; 
            shangw = sx + dw - spixw;
            if (shangw > 0)
                dw -= shangw; 

            if (dy < 0)
            {
                sy -= dy;
                dh += dy;
                dy = 0;
            }
            if (sy < 0)
            {
                dy -= sy;
                dh += sy;
                sy = 0;
            }
            dhangh = dy + dh - dpixh; 
            if (dhangh > 0)
                dh -= dhangh;
            shangh = sy + dh - spixh; 
            if (shangh > 0)
                dh -= shangh;

            if ((dw <= 0) || (dh <= 0))
                return;

            if (((dx & 31) == 0) && ((sx & 31) == 0))
                RasteropWordAlignedLow(ref datad, dwpl, dx, dy, dw, dh, op, datas, swpl, sx, sy);
            else if ((dx & 31) == (sx & 31))
                RasteropVAlignedLow(ref datad, dwpl, dx, dy, dw, dh, op, datas, swpl, sx, sy);
            else
                RasteropGeneralLow(ref datad, dwpl, dx, dy, dw, dh, op, datas, swpl, sx, sy);

            return;
        }

        /// <summary>
        /// Rasterop without vertical word alignment.
        /// </summary>
        void RasteropGeneralLow(ref uint[] datad, int dwpl, int dx, int dy, int dw, int dh, int op, uint[] datas, int swpl, int sx, int sy)
        {
            int dfwpartb; 
            int dfwpart2b;
            uint dfwmask = 0;
            int dfwbits;
            int dhang;
            bool dfwfullb;
            int dnfullw;  
            int dlwpartb; 
            uint dlwmask = 0;
            int dlwbits;
            uint sword; 
            int sfwbits;
            int shang;  
            int sleftshift;
            int srightshift;
            uint srightmask = 0;
            int sfwshiftdir = 0;
            bool sfwaddb = false;
            bool slwaddb = false;
            int i, j;

            int pdfwpartCount = 0;
            int psfwpartCount = 0;
            int pdfwfullCount = 0;
            int psfwfullCount = 0;
            int pdlwpartCount = 0;
            int pslwpartCount = 0;

            if ((sx & 31) == 0)
                shang = 0;
            else
                shang = 32 - (sx & 31);
            if ((dx & 31) == 0)
                dhang = 0;
            else
                dhang = 32 - (dx & 31);

            if (shang == 0 && dhang == 0)
            {
                sleftshift = 0;
                srightshift = 0;
                srightmask = JBIG2Statics.RightMask[0];
            }
            else
            {
                if (dhang > shang)
                    sleftshift = dhang - shang;
                else
                    sleftshift = 32 - (shang - dhang);
                srightshift = 32 - sleftshift;
                srightmask = JBIG2Statics.RightMask[sleftshift];
            }

            if ((dx & 31) == 0)
            { 
                dfwpartb = 0;
                dfwbits = 0;
            }
            else
            {  /* if so */
                dfwpartb = 1;
                dfwbits = 32 - (dx & 31);
                dfwmask = JBIG2Statics.RightMask[dfwbits];
                pdfwpartCount = dwpl * dy + (dx >> 5);
                psfwpartCount = swpl * sy + (sx >> 5);
                sfwbits = 32 - (sx & 31);
                if (dfwbits > sfwbits)
                {
                    sfwshiftdir = JBIG2Statics.ShiftLeft; 
                    if (dw < shang)
                        sfwaddb = false;
                    else
                        sfwaddb = true;
                }
                else
                    sfwshiftdir = JBIG2Statics.ShiftRight; 
            }

            if (dw >= dfwbits)
                dfwpart2b = 0;
            else
            {
                dfwpart2b = 1;
                dfwmask &= JBIG2Statics.LeftMask[32 - dfwbits + dw];
            }

            if (dfwpart2b == 1)
            {
                dfwfullb = false;
                dnfullw = 0;
            }
            else
            {
                dnfullw = (dw - dfwbits) >> 5;
                if (dnfullw == 0)
                    dfwfullb = false;
                else
                { 
                    dfwfullb = true;
                    pdfwfullCount = dwpl * dy + ((dx + dhang) >> 5);
                    psfwfullCount = swpl * sy + ((sx + dhang) >> 5);
                }
            }

            dlwbits = (dx + dw) & 31;
            if (dfwpart2b == 1 || dlwbits == 0)  /* if not */
                dlwpartb = 0;
            else
            {
                dlwpartb = 1;
                dlwmask = JBIG2Statics.LeftMask[dlwbits];
                pdlwpartCount = dwpl * dy + ((dx + dhang) >> 5) + dnfullw;
                pslwpartCount = swpl * sy + ((sx + dhang) >> 5) + dnfullw;
                if (dlwbits <= srightshift) 
                    slwaddb = false;
                else
                    slwaddb = true;
            }

            int indexs = 0;
            int indexd = 0;
            if (op == JBIG2Statics.PixSrc)
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;

                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);
                        }
                        else
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], sword, dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j]) << sleftshift, (datas[indexs + j + 1]) >> srightshift, srightmask);
                            datad[indexd + j] = sword;
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], sword, dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == JBIG2Statics.PixNot(JBIG2Statics.PixSrc))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);
                        }
                        else
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~sword, dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j]) << sleftshift, (datas[indexs + j + 1]) >> srightshift, srightmask);
                            datad[j] = ~sword;
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~sword, dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc | JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);
                        }
                        else
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword | datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j]) << sleftshift, (datas[indexs + j + 1]) >> srightshift, srightmask);
                            datad[j] |= sword;
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword | datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc & JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs] + 1) >> srightshift, srightmask);
                        }
                        else
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword & datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j]) << sleftshift, (datas[indexs + j + 1]) >> srightshift, srightmask);
                            datad[j] &= sword;
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword & datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);
                        }
                        else 
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword ^ datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j]) << sleftshift, (datas[indexs + j + 1]) >> srightshift, srightmask);
                            datad[indexd + j] ^= sword;
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword ^ datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc) | JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);
                        }
                        else 
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~sword | datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j] << sleftshift), (datas[indexs + j + 1] >> srightshift), srightmask);
                            datad[j] |= ~sword;
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~sword | datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc) & JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);
                        }
                        else
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~sword & datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j] << sleftshift), (datas[indexs + j + 1] >> srightshift), srightmask);
                            datad[j] &= ~sword;
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~sword & datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc | JBIG2Statics.PixNot(JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);
                        }
                        else
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword | ~(datad[indexd])), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j] << sleftshift), (datas[indexs + j + 1] >> srightshift), srightmask);
                            datad[indexd + j] = sword | ~((datad[indexd + j]));
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1]) >> srightshift, srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword | ~(datad[indexd])), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc & JBIG2Statics.PixNot(JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);
                        }
                        else
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword & ~(datad[indexd])), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j] << sleftshift), (datas[indexs + j + 1] >> srightshift), srightmask);
                            datad[indexd + j] = sword & ~(datad[indexd + j]);
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (sword & ~(datad[indexd])), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc | JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);
                        }
                        else
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(sword | datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j] << sleftshift), (datas[indexs + j + 1] >> srightshift), srightmask);
                            datad[indexd + j] = ~(sword | datad[indexd + j]);
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(sword | datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc & JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);
                        }
                        else
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(sword & datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j] << sleftshift),
                                           (datas[indexs + j + 1] >> srightshift), srightmask);
                            datad[indexd + j] = ~(sword & (datad[indexd + j]));
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        sword = datas[indexs] << sleftshift;
                        if (slwaddb)
                            sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(sword & datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        if (sfwshiftdir == JBIG2Statics.ShiftLeft)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (sfwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);
                        }
                        else 
                            sword = datas[indexs] >> srightshift;

                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(sword ^ datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                        {
                            sword = COMBINE_PARTIAL((datas[indexs + j] << sleftshift), (datas[indexs + j + 1] >> srightshift), srightmask);
                            datad[indexd + j] = ~(sword ^ (datad[indexd + j]));
                        }
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                        for (i = 0; i < dh; i++)
                        {
                            sword = datas[indexs] << sleftshift;
                            if (slwaddb)
                                sword = COMBINE_PARTIAL(sword, (datas[indexs + 1] >> srightshift), srightmask);

                            datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(sword ^ datad[indexd]), dlwmask);
                            indexd += dwpl;
                            indexs += swpl;
                        }
                }
            }
        }

        /// <summary>
        /// Rasterop with vertical word alignment.
        /// </summary>
        void RasteropVAlignedLow(ref uint[] datad, int dwpl, int dx, int dy, int dw, int dh, int op, uint[] datas, int swpl, int sx, int sy)
        {
            int dfwpartb; 
            int dfwpart2b;
            uint dfwmask = 0;
            int dfwbits;   
            int dfwfullb;  
            int dnfullw;   
            int dlwpartb;  
            uint dlwmask = 0;
            int dlwbits;
            int i, j;
            int indexd = 0;
            int indexs = 0;
            int pdfwfullCount = 0, psfwfullCount = 0, pdfwpartCount = 0, psfwpartCount = 0, pdlwpartCount = 0, pslwpartCount = 0;

            if ((dx & 31) == 0)
            { 
                dfwpartb = 0;
                dfwbits = 0;
            }
            else
            { 
                dfwpartb = 1;
                dfwbits = 32 - (dx & 31);
                dfwmask = JBIG2Statics.RightMask[dfwbits];
                pdfwpartCount = dwpl * dy + (dx >> 5);
                psfwpartCount = swpl * sy + (sx >> 5);
            }

            if (dw >= dfwbits)
                dfwpart2b = 0;
            else
            {
                dfwpart2b = 1;
                dfwmask &= JBIG2Statics.LeftMask[32 - dfwbits + dw];
            }

            if (dfwpart2b == 1)
            {
                dfwfullb = 0;
                dnfullw = 0;
            }
            else
            {
                dnfullw = (dw - dfwbits) >> 5;
                if (dnfullw == 0)
                    dfwfullb = 0;
                else
                {
                    dfwfullb = 1;
                    if (dfwpartb > 0)
                    {
                        pdfwfullCount = pdfwpartCount + 1;
                        psfwfullCount = psfwpartCount + 1;
                    }
                    else
                    {
                        pdfwfullCount = dwpl * dy + (dx >> 5);
                        psfwfullCount = swpl * sy + (sx >> 5);
                    }
                }
            }

            dlwbits = (dx + dw) & 31;
            if (dfwpart2b == 1 || dlwbits == 0)
                dlwpartb = 0;
            else
            {
                dlwpartb = 1;
                dlwmask = JBIG2Statics.LeftMask[dlwbits];
                if (dfwpartb > 0)
                {
                    pdlwpartCount = pdfwpartCount + 1 + dnfullw;
                    pslwpartCount = psfwpartCount + 1 + dnfullw;
                }
                else
                {
                    pdlwpartCount = dwpl * dy + (dx >> 5) + dnfullw;
                    pslwpartCount = swpl * sy + (sx >> 5) + dnfullw;
                }
            }

            if (op == JBIG2Statics.PixSrc)
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], datas[indexs], dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = (datas[indexs + j]);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], datas[indexs], dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == JBIG2Statics.PixNot(JBIG2Statics.PixSrc))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = ~(datas[indexs + j]);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc | JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] | datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] |= datas[indexs + j];
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] | datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc & JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] & datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] &= datas[indexs + j];
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] & datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] ^ datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                /* do the full words */
                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] ^= datas[indexs + j];
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] ^ datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc) | JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~(datas[indexs]) | datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] |= ~(datas[indexs + j]);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                /* do the last partial word */
                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~(datas[indexs]) | datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc) & JBIG2Statics.PixDst))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~(datas[indexs]) & datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] &= ~(datas[indexs + j]);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~(datas[indexs]) & datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc | JBIG2Statics.PixNot(JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] | ~(datad[indexd])), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = (datas[indexs + j]) | ~(datad[indexd + j]);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] | ~(datad[indexd])), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixSrc & JBIG2Statics.PixNot(JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] & ~(datad[indexd])), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = (datas[indexs + j]) & ~(datad[indexd + j]);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] & ~(datad[indexd])), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc | JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs] | datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = ~(datas[indexs + j] | datad[indexd + j]);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs] | datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc & JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs] & datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = ~(datas[indexs + j] & datad[indexd + j]);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs] & datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst)))
            {
                if (dfwpartb > 0)
                {
                    indexs = psfwpartCount;
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs] ^ datad[indexd]), dfwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dfwfullb > 0)
                {
                    indexs = psfwfullCount;
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = ~(datas[indexs + j] ^ datad[indexd + j]);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }

                if (dlwpartb > 0)
                {
                    indexs = pslwpartCount;
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs] ^ datad[indexd]), dlwmask);
                        indexd += dwpl;
                        indexs += swpl;
                    }
                }
            }
        }

        /// <summary>
        /// Rasterop with vertical word alignment.
        /// </summary>
        private void RasteropWordAlignedLow(ref uint[] datad, int dwpl, int dx, int dy, int dw, int dh, int op, uint[] datas, int swpl, int sx, int sy)
        {
            int nfullw;
            int lwbits;
            uint lwmask = 0;
            int i, j;

            nfullw = dw >> 5;
            lwbits = dw & 31;
            if (lwbits > 0)
                lwmask = JBIG2Statics.LeftMask[lwbits];
            int psfwordCount = swpl * sy + (sx >> 5);
            int pdfwordCount = dwpl * dy + (dx >> 5);
            int indexd = 0, indexs = 0;

            if (op == JBIG2Statics.PixSrc)
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = datas[indexs];
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], datas[indexs], lwmask);
                }
            }
            else if (op == JBIG2Statics.PixNot(JBIG2Statics.PixSrc))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = ~(datas[indexs]);
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs]), lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixSrc | JBIG2Statics.PixDst))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = (datas[indexs] | datad[indexd]);
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] | datad[indexd]), lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixSrc & JBIG2Statics.PixDst))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = (datas[indexs] & datad[indexd]);
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] & datad[indexd]), lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = (datas[indexs] ^ datad[indexd]);
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] ^ datad[indexd]), lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc) | JBIG2Statics.PixDst))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = (~(datas[indexs]) | datad[indexd]);
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~(datas[indexs]) | datad[indexd]), lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc) & JBIG2Statics.PixDst))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = (~(datas[indexs]) & datad[indexd]);
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (~(datas[indexs]) & datad[indexd]), lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixSrc | JBIG2Statics.PixNot(JBIG2Statics.PixDst)))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = (datas[indexs] | ~(datad[indexd]));
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] | ~(datad[indexd])), lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixSrc & JBIG2Statics.PixNot(JBIG2Statics.PixDst)))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = (datas[indexs] & ~(datad[indexd]));
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], (datas[indexs] & ~(datad[indexd])), lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc | JBIG2Statics.PixDst)))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = ~(datas[indexs] | datad[indexd]);
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs] | datad[indexd]), lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixSrc & JBIG2Statics.PixDst)))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = ~(datas[indexs] & datad[indexd]);
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs] & datad[indexd]), lwmask);
                }
            }
            else if (op == JBIG2Statics.PixNot(JBIG2Statics.PixSrc ^ JBIG2Statics.PixDst))
            {
                for (i = 0; i < dh; i++)
                {
                    indexs = i * swpl + psfwordCount;
                    indexd = i * dwpl + pdfwordCount;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[indexd] = ~(datas[indexs] ^ datad[indexd]);
                        indexd++;
                        indexs++;
                    }
                    if (lwbits > 0)
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datas[indexs] ^ datad[indexd]), lwmask);
                }
            }
        }

        /// <summary>
        /// Scales width, performs clipping, checks alignment, and dispatches for the rasterop.
        /// </summary>
        void RasteropUniLow(ref uint[] datad, int dpixw, int dpixh, int depth, int dwpl, int dx, int dy, int dw, int dh, int op)
        {
            int dhangw, dhangh;

            if (depth != 1)
            {
                dpixw *= depth;
                dx *= depth;
                dw *= depth;
            }

            if (dx < 0)
            {
                dw += dx; 
                dx = 0;
            }
            dhangw = dx + dw - dpixw; 
            if (dhangw > 0)
                dw -= dhangw;

            if (dy < 0)
            {
                dh += dy;
                dy = 0;
            }
            dhangh = dy + dh - dpixh;
            if (dhangh > 0)
                dh -= dhangh;

            if ((dw <= 0) || (dh <= 0))
                return;

            if ((dx & 31) == 0)
                RasteropUniWordAlignedLow(ref datad, dwpl, dx, dy, dw, dh, op);
            else
                RasteropUniGeneralLow(ref datad, dwpl, dx, dy, dw, dh, op);
            return;
        }

        /// <summary>
        /// Low level uni rasterop.
        /// </summary>
        void RasteropUniGeneralLow(ref uint[] datad, int dwpl, int dx, int dy, int dw, int dh, int op)
        {
            bool dfwpartb;
            bool dfwpart2b;
            uint dfwmask = 0;
            int dfwbits;
            bool dfwfullb;
            int dnfullw;
            bool dlwpartb;
            uint dlwmask = 0;
            int dlwbits;
            int i, j;
            int indexd = 0;
            int pdfwpartCount = 0;
            int pdfwfullCount = 0;
            int pdlwpartCount = 0;

            if ((dx & 31) == 0)
            {
                dfwpartb = false;
                dfwbits = 0;
            }
            else
            {
                dfwpartb = true;
                dfwbits = 32 - (dx & 31);
                dfwmask = JBIG2Statics.RightMask[dfwbits];
                pdfwpartCount = dwpl * dy + (dx >> 5);
            }

            if (dw >= dfwbits)
                dfwpart2b = false;
            else
            {  /* if so */
                dfwpart2b = true;
                dfwmask &= JBIG2Statics.LeftMask[32 - dfwbits + dw];
            }

            if (dfwpart2b)
            {
                dfwfullb = false;
                dnfullw = 0;
            }
            else
            {
                dnfullw = (dw - dfwbits) >> 5;
                if (dnfullw == 0)
                    dfwfullb = false;
                else
                {
                    dfwfullb = true;
                    if (dfwpartb)
                        pdfwfullCount = pdfwpartCount + 1;
                    else
                        pdfwfullCount = dwpl * dy + (dx >> 5);
                }
            }

            dlwbits = (dx + dw) & 31;
            if (dfwpart2b || dlwbits == 0)
                dlwpartb = false;
            else
            {
                dlwpartb = true;
                dlwmask = JBIG2Statics.LeftMask[dlwbits];
                if (dfwpartb)
                    pdlwpartCount = pdfwpartCount + 1 + dnfullw;
                else
                    pdlwpartCount = dwpl * dy + (dx >> 5) + dnfullw;
            }

            if (op == JBIG2Statics.PixClr)
            {
                if (dfwpartb)
                {
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], 0x0, dfwmask);
                        indexd += dwpl;
                    }
                }

                if (dfwfullb)
                {
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = 0x0;
                        indexd += dwpl;
                    }
                }

                if (dlwpartb)
                {
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], 0x0, dlwmask);
                        indexd += dwpl;
                    }
                }
            }
            else if (op == JBIG2Statics.PixSet)
            {
                if (dfwpartb)
                {
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], 0xffffffff, dfwmask);
                        indexd += dwpl;
                    }
                }

                if (dfwfullb)
                {
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = 0xffffffff;
                        indexd += dwpl;
                    }
                }

                if (dlwpartb)
                {
                    indexd = pdlwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], 0xffffffff, dlwmask);
                        indexd += dwpl;
                    }
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixDst)))
            {
                if (dfwpartb) 
                {
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datad[indexd]), dfwmask);
                        indexd += dwpl;
                    }
                }

                if (dfwfullb)
                {
                    indexd = pdfwfullCount;
                    for (i = 0; i < dh; i++)
                    {
                        for (j = 0; j < dnfullw; j++)
                            datad[indexd + j] = ~(datad[indexd + j]);
                        indexd += dwpl;
                    }
                }

                if (dlwpartb)
                {
                    indexd = pdfwpartCount;
                    for (i = 0; i < dh; i++)
                    {
                        datad[indexd] = COMBINE_PARTIAL(datad[indexd], ~(datad[indexd]), dlwmask);
                        indexd += dwpl;
                    }
                }
            }
        }

        /// <summary>
        /// Dest rect is left aligned on (32-bit) word boundaries.
        /// </summary>
        void RasteropUniWordAlignedLow(ref uint[] datad, int dwpl, int dx, int dy, int dw, int dh, int op)
        {
            int nfullw; 
            int lwbits; 
            uint lwmask = 0;
            int i, j;

            nfullw = dw >> 5;
            lwbits = dw & 31;
            uint[] lined = new uint[dh * nfullw];
            if (nfullw == 0)
                lined = new uint[dh * 1];
            if (lwbits > 0)
                lwmask = JBIG2Statics.LeftMask[lwbits];

            int count = dwpl * dy + (dx >> 5);
            int index = 0;

            if (op == JBIG2Statics.PixClr)
            {
                for (i = 0; i < dh; i++)
                {
                    index = i * dwpl + count;
                    for (j = 0; j < nfullw; j++)
                        datad[index++] = 0x0;
                    if (lwbits > 0)
                        datad[index] = COMBINE_PARTIAL(datad[index], 0x0, lwmask);
                }
            }
            else if (op == JBIG2Statics.PixSet)
            {
                for (i = 0; i < dh; i++)
                {
                    index = i * dwpl + count;
                    for (j = 0; j < nfullw; j++)
                        datad[index++] = 0xffffffff;
                    if (lwbits > 0)
                        datad[index] = COMBINE_PARTIAL(datad[index], 0xffffffff, lwmask);
                }
            }
            else if (op == (JBIG2Statics.PixNot(JBIG2Statics.PixDst)))
            {
                for (i = 0; i < dh; i++)
                {
                    index = i * dwpl + count;
                    for (j = 0; j < nfullw; j++)
                    {
                        datad[index] = ~(datad[index]);
                        index++;
                    }
                    if (lwbits > 0)
                        datad[index] = COMBINE_PARTIAL(datad[index], ~(datad[index]), lwmask);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private uint COMBINE_PARTIAL(uint d, uint s, uint m)
        {
            return (d & ~(m)) | (s & m);
        }

        /// <summary>
        /// Color sample setting and extraction.
        /// </summary>
        private void ComposeRGBPixel(int rval, int gval, int bval, int ppixel)
        {
            ppixel = (rval << m_redShift) | (gval << m_greenShift) | (bval << m_blueShift);
        }

        /// <summary>
        /// Creates Pix.
        /// </summary>
        private Pix PixCreate(int width, int height, int depth)
        {
            return PixCreateNoInit(width, height, depth);
        }

        /// <summary>
        /// Reads color for colormap.
        /// </summary>
        private void PixCmapGetColor(PixColormap cmap, int index, int prval, int pgval, int pbval)
        {
            RGBA_Quad[] cta;

            if (index < 0 || index >= cmap.N)
                throw new NullReferenceException("index out of bounds");

            cta = cmap.Array;
            prval = cta[index].Red;
            pgval = cta[index].Green;
            pbval = cta[index].Blue;
        }

        /// <summary>
        /// Copy Pix.
        /// </summary>
        private Pix PixCopy(Pix pixd, Pix pixs)
        {
            int bytes;
            uint[] datas, datad;

            if (pixs == null)
                throw new NullReferenceException("Pixs not defined");
            if (pixs == pixd)
                return pixd;

            bytes = 4 * pixs.Wpl * pixs.H;

            if (pixd == null)
            {
                if ((pixd = PixCreateTemplate(pixs)) == null)
                    throw new NullReferenceException("pixd not made");
                datas = pixs.Data;
                if (datas != null)
                {
                    pixd.Data = new uint[datas.Length];
                    Array.Copy(datas, pixd.Data, datas.Length);
                }
                return pixd;
            }

            if (PixResizeImageData(pixd, pixs) == 1)
            { return null; }

            PixCopyColormap(pixd, pixs);
            PixCopyResolution(pixd, pixs);
            pixd.Informat = pixs.Informat;
            pixd.Text = pixs.Text;

            pixd.Data = new uint[bytes / 4];
            datad = pixd.Data;

            Array.Copy(pixs.Data, datad, bytes / 4);
            pixd.Data = datad;

            return pixd;
        }

        /// <summary>
        /// Creates Pix of the same size as the input Pix.
        /// </summary>
        private Pix PixCreateTemplate(Pix pixs)
        {
            Pix pixd;

            if ((pixd = PixCreateTemplateNoInit(pixs)) == null)
            {
                Console.WriteLine("pixd not made");
                return null;
            }
            //memset(pixd.Data, 0, 4 * pixd.Wpl * pixd.H); //TODO
            return pixd;
        }

        /// <summary>
        /// Creates Pix of the same size as the input Pix.
        /// </summary>
        private Pix PixCreateTemplateNoInit(Pix pixs)
        {
            int w, h, d;
            Pix pixd;

            w = pixs.W; h = pixs.H; d = pixs.D;
            pixd = PixCreateNoInit(w, h, d);
            if (pixd == null)
                throw new Exception("Pixd not made"); //return (PIX *)ERROR_PTR("pixd not made", procName, null);
            PixCopyResolution(pixd, pixs);
            PixCopyColormap(pixd, pixs);
            pixd.Text = pixs.Text; 
            pixd.Informat = pixs.Informat;

            return pixd;
        }

        /// <summary>
        /// Copies colormap from source Pix.
        /// </summary>
        void PixCopyColormap(Pix pixd, Pix pixs)
        {
            PixColormap cmaps, cmapd;

            if (pixs == pixd)
                return;

            pixd = null;
            if ((cmaps = pixs.Colormap) == null)
                return;

            if ((cmapd = PixCmapCopy(cmaps)) == null)
                throw new NullReferenceException("Cmapd not made");
            pixd.Colormap = cmapd;
        }

        private PixColormap PixCmapCopy(PixColormap cmaps)
        {
            int nbytes;
            PixColormap cmapd = new PixColormap();

            cmapd.N = cmaps.N;
            cmapd.Nalloc = cmaps.Nalloc;
            cmapd.Depth = cmaps.Depth;
            cmapd.Array = cmaps.Array;

            return cmapd;
        }

        /// <summary>
        /// Copies resolution from source Pix.
        /// </summary>
        void PixCopyResolution(Pix pixd, Pix pixs)
        {
            pixd.XRes = pixs.XRes;
            pixd.YRes = pixs.YRes;
        }

        /// <summary>
        /// Creates Pix.
        /// </summary>
        private Pix PixCreateNoInit(int width, int height, int depth)
        {
            int wpl;
            Pix pixd;

            pixd = PixCreateHeader(width, height, depth);
            if (pixd == null)
                throw new NullReferenceException("Pixd is null");
            wpl = pixd.Wpl;

            pixd.Data = new uint[wpl * height];
            for (int i = 0; i < wpl * height; i++)
                pixd.Data[i] = 3452816845; //This automatically happens in C++.
            PixSetPadBits(ref pixd, 0);
            return pixd;
        }

        private void PixSetPadBits(ref Pix pix, int val)
        {
            int w, h, d, wpl, endbits, fullwords;
            uint mask;
            uint[] data;

            w = pix.W; h = pix.H; d = pix.D;
            if (d == 32) 
                return;

            data = pix.Data;
            wpl = pix.Wpl;
            endbits = 32 - ((w * d) % 32);
            if (endbits == 32)
                return;
            fullwords = w * d / 32;

            mask = JBIG2Statics.RightMask[endbits];
            if (val == 0)
                mask = ~mask;

            int k = 0;
            for (int i = 0; i < h; i++)
            {
                k = i * wpl + fullwords;
                {
                    if (val == 0) /* clear */
                        data[k] = data[k] & mask;
                    else  /* set */
                        data[k] = data[k] | mask;
                }
            }

            pix.Data = data;
        }

        /// <summary>
        /// Create Pix.
        /// </summary>
        private Pix PixCreateHeader(int width, int height, int depth)
        {
            int wpl;
            Pix pixd = new Pix();

            if ((depth != 1) && (depth != 2) && (depth != 4) && (depth != 8)
                 && (depth != 16) && (depth != 24) && (depth != 32))
                throw new ArgumentOutOfRangeException("depth must be {1, 2, 4, 8, 16, 24, 32}"); 
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width must be > 0");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height must be > 0");

            pixd.W = width;
            pixd.H = height;
            pixd.D = depth;
            wpl = (width * depth + 31) / 32;
            pixd.Wpl = wpl;

            pixd.Informat = 0;
            return pixd;
        }

        /// <summary>
        /// Checks if Colormap has color.
        /// </summary>
        private void PixCmapHasColor(PixColormap cmap, ref bool pcolor)
        {
            int n, i;
            int[] rmap = new int[] { };
            int[] gmap = new int[] { };
            int[] bmap = new int[] { };

            pcolor = false;
            if (cmap == null)
                throw new NullReferenceException("cmap not defined");

            if (!PixCmapToArrays(cmap, ref rmap, ref gmap, ref bmap))
                return;
            n = cmap.N;
            for (i = 0; i < n; i++)
            {
                if ((rmap[i] != gmap[i]) || (rmap[i] != bmap[i]))
                {
                    pcolor = true;
                    break;
                }
            }

            rmap = null;
            gmap = null;
            bmap = null;
        }

        /// <summary>
        /// Read colormap array.
        /// </summary>
        private bool PixCmapToArrays(PixColormap cmap, ref int[] rmap, ref int[] gmap, ref int[] bmap)
        {
            int i, ncolors;
            RGBA_Quad[] cta;

            if (cmap == null)
                throw new NullReferenceException("cmap not defined");

            ncolors = cmap.N;
            rmap = new int[ncolors]; gmap = new int[ncolors]; bmap = new int[ncolors];
            cta = cmap.Array;
            for (i = 0; i < ncolors; i++)
            {
                rmap[i] = cta[i].Red;
                gmap[i] = cta[i].Green;
                bmap[i] = cta[i].Blue;
            }

            return false;
        }
    }
}