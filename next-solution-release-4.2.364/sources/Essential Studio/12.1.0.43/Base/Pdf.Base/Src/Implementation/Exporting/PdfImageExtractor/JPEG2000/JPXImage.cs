#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region Using Statements
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Syncfusion.Pdf.JPEG2000.codestream;
using Syncfusion.Pdf.JPEG2000.codestream.reader;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.entropy.decoder;
using Syncfusion.Pdf.JPEG2000.fileformat.reader;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.image.invcomptransf;
using Syncfusion.Pdf.JPEG2000.io;
using Syncfusion.Pdf.JPEG2000.quantization.dequantizer;
using Syncfusion.Pdf.JPEG2000.roi;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
#endregion
namespace Syncfusion.Pdf
{
    internal class JPXImage
    {
        internal Image FromStream(Stream stream)
        {
            JPXRandomAccessStream in_stream = new JPXRandomAccessStream(stream, 1 << 18, 1 << 18, System.Int32.MaxValue);
            JPXParameters defpl = GetParameters(decoder_pinfo);
            JPXParameters pl = new JPXParameters(defpl);
            JPXFormatReader ff = new JPXFormatReader(in_stream);
            ff.readFileFormat();
            if (ff.JP2FFUsed)
            {
                in_stream.seek(ff.FirstCodeStreamPos);
            }
            HeaderInformation hdrInfo = new HeaderInformation();
            HeaderDecoder hdr = new HeaderDecoder(in_stream, pl, hdrInfo);
            int nComponets = hdr.NumComps;
            int nTiles = hdrInfo.sizValue.NumTiles;
            DecodeHelper decodeHlpr = hdr.DecoderHelper;
            int[] depth = new int[nComponets];
            for (int i = 0; i < nComponets; i++)
            {
                depth[i] = hdr.GetActualBitDepth(i);
            }
            BitstreamReader breader = BitstreamReader.createInstance(in_stream, hdr, pl, decodeHlpr, false, hdrInfo);
            EntropyDecoder entdec = hdr.createEntropyDecoder(breader, pl);
            DeScalerROI roids = hdr.createROIDeScaler(entdec, pl, decodeHlpr);
            Dequantizer deq = hdr.createDequantizer(roids, depth, decodeHlpr);
            WaveletTransformInverse invWT = WaveletTransformInverse.createInstance(deq, decodeHlpr);
            int res = breader.ImgRes;
            invWT.ImgResLevel = res;
            ImageDataConverter converter = new ImageDataConverter(invWT, 0);
            InverseComponetTransformation inverseTransformation = new InverseComponetTransformation(converter, decodeHlpr, depth, pl);
            BlockImageDataSource decodedImage = inverseTransformation;
            int numComps = decodedImage.NumComps;
            int bytesPerPixel = (numComps == 4 ? 4 : 3);
            PixelFormat pixelFormat;
            switch (numComps)
            {
                case 1:
                    pixelFormat = PixelFormat.Format24bppRgb; break;
                case 3:
                    pixelFormat = PixelFormat.Format24bppRgb; break;
                case 4:
                    pixelFormat = PixelFormat.Format32bppArgb; break;
                default:
                    throw new ApplicationException("Unsupported PixelFormat.  " + numComps + " components.");
            }
            Bitmap jpxBitmap = new Bitmap(decodedImage.ImgWidth, decodedImage.ImgHeight, pixelFormat);
            int num = (int)numComps;
            byte[] buffer = new byte[decodedImage.ImgWidth * numComps];
            JPXImageCoordinates numTiles = decodedImage.getNumTiles(null);
            int tIdx = 0;
            int tileCount = 0;
            for (int y = 0; y < numTiles.y; y++)
            {
                for (int x = 0; x < numTiles.x; x++, tIdx++)
                {
                    decodedImage.setTile(x, y);
                    int height = decodedImage.getTileComponentHeight(tIdx, 0);
                    int width = decodedImage.getTileComponentWidth(tIdx, 0);
                    int tOffx = decodedImage.getCompUpperLeftCornerX(0) - (int)Math.Ceiling(decodedImage.ImgULX / (double)decodedImage.getCompSubsX(0));
                    int tOffy = decodedImage.getCompUpperLeftCornerY(0) - (int)Math.Ceiling(decodedImage.ImgULY / (double)decodedImage.getCompSubsY(0));
                    DataBlockInt[] db = new DataBlockInt[numComps];
                    int[] ls = new int[numComps];
                    int[] mv = new int[numComps];
                    int[] fb = new int[numComps];
                    for (int i = 0; i < numComps; i++)
                    {
                        db[i] = new DataBlockInt();
                        ls[i] = 1 << (decodedImage.getNomRangeBits(0) - 1);
                        mv[i] = (1 << decodedImage.getNomRangeBits(0)) - 1;
                        fb[i] = decodedImage.getFixedPoint(0);
                    }
                    for (int l = 0; l < height; l++)
                    {
                        for (int i = numComps - 1; i >= 0; i--)
                        {
                            db[i].ulx = 0;
                            db[i].uly = l;
                            db[i].w = width;
                            db[i].h = 1;
                            decodedImage.getInternCompData(db[i], i);
                        }
                        int[] k = new int[numComps];
                        for (int i = numComps - 1; i >= 0; i--) k[i] = db[i].offset + width - 1;
                        byte[] rowvalues = new byte[width * bytesPerPixel];
                        for (int i = width - 1; i >= 0; i--)
                        {
                            int[] tmp = new int[numComps];
                            for (int j = numComps - 1; j >= 0; j--)
                            {
                                tmp[j] = (db[j].data_array[k[j]--] >> fb[j]) + ls[j];
                                tmp[j] = (tmp[j] < 0) ? 0 : ((tmp[j] > mv[j]) ? mv[j] : tmp[j]);
                                if (decodedImage.getNomRangeBits(j) != 8)
                                    tmp[j] = (int)Math.Round(((double)tmp[j] / Math.Pow(2D, (double)decodedImage.getNomRangeBits(j))) * 255D);
                            }
                            int offset = i * bytesPerPixel;
                            switch (numComps)
                            {
                                case 1:
                                    rowvalues[offset + 0] = (byte)tmp[0];
                                    rowvalues[offset + 1] = (byte)tmp[0];
                                    rowvalues[offset + 2] = (byte)tmp[0];
                                    break;
                                case 3:
                                    rowvalues[offset + 0] = (byte)tmp[2];
                                    rowvalues[offset + 1] = (byte)tmp[1];
                                    rowvalues[offset + 2] = (byte)tmp[0];
                                    break;
                                case 4:
                                    {
                                        double div = 255;
                                        double inCyan = (tmp[0]) / div;
                                        double inMagenta = (tmp[1]) / div;
                                        double inYellow = (tmp[2]) / div;
                                        double inBlack = (tmp[3]) / div;
                                        double r1 = 0, g1 = 0, b1 = 0;
                                        double c1, m1, y1, k1;
                                        double finalC = -1, finalM = -1.12, finalY = -1.12, finalK = -1.21;
                                        if ((finalC != inCyan) || (finalM != inMagenta) || (finalY != inYellow) || (finalK != inBlack))
                                        {
                                            c1 = inCyan;
                                            m1 = inMagenta;
                                            y1 = inYellow;
                                            k1 = inBlack;
                                            r1 = 255 * (1 - c1) * (1 - k1);
                                            g1 = 255 * (1 - m1) * (1 - k1);
                                            b1 = 255 * (1 - y1) * (1 - k1);
                                        }
                                        r1 = r1 > 255 ? 255 : r1 < 0 ? 0 : r1;
                                        g1 = g1 > 255 ? 255 : g1 < 0 ? 0 : g1;
                                        b1 = b1 > 255 ? 255 : b1 < 0 ? 0 : b1;
                                        rowvalues[offset + 2] = (byte)r1;
                                        rowvalues[offset + 1] = (byte)g1;
                                        rowvalues[offset + 0] = (byte)b1;
                                        break;
                                    }
                            }
                        }
                        BitmapData dstdata = jpxBitmap.LockBits(
                            new System.Drawing.Rectangle(tOffx, tOffy + l, width, 1),
                            ImageLockMode.ReadWrite, pixelFormat);
                        IntPtr ptr = dstdata.Scan0;
                        System.Runtime.InteropServices.Marshal.Copy(rowvalues, 0, ptr, rowvalues.Length);
                        jpxBitmap.UnlockBits(dstdata);
                    }
                }
            }
            return jpxBitmap;
        }
        #region Default Parameter Loader
        internal JPXParameters GetParameters(string[][] pinfo)
        {
            JPXParameters pl = new JPXParameters();
            string[][] str;
            str = BitstreamReader.ParameterInfo;
            if (str != null) for (int i = str.Length - 1; i >= 0; i--)
                    pl.Add(str[i][0], str[i][3]);
            str = EntropyDecoder.ParameterInfo;
            if (str != null) for (int i = str.Length - 1; i >= 0; i--)
                    pl.Add(str[i][0], str[i][3]);
            str = DeScalerROI.ParameterInfo;
            if (str != null) for (int i = str.Length - 1; i >= 0; i--)
                    pl.Add(str[i][0], str[i][3]);
            str = Dequantizer.ParameterInfo;
            if (str != null) for (int i = str.Length - 1; i >= 0; i--)
                    pl.Add(str[i][0], str[i][3]);
            str = InverseComponetTransformation.ParameterInfo;
            if (str != null) for (int i = str.Length - 1; i >= 0; i--)
                    pl.Add(str[i][0], str[i][3]);
            str = HeaderDecoder.ParameterInfo;
            if (str != null) for (int i = str.Length - 1; i >= 0; i--)
                    pl.Add(str[i][0], str[i][3]);
            str = pinfo;
            if (str != null) for (int i = str.Length - 1; i >= 0; i--)
                    pl.Add(str[i][0], str[i][3]);
            return pl;
        }
        #endregion
        #region Decoder Parameters
        private String[][] decoder_pinfo = {
        new string[] { "u", "[on|off]", ""+"","off"},
        new string[] { "v", "[on|off]", "","off"},
        new string[] { "verbose", "[on|off]","","on"},
        new string[] { "pfile", "",""+""+""+ "" + "  "+""+ ""+ ""+ ""+""+" "+"",null},  
        new string[] { "res", "",  ""+""+""+ ""+""+""+""+" "+""+"", null},
        new string[] { "i", "",""+""+""+""+""+ ""+""+"", null},
        new string[] { "o", "",""+ ""+""+""+ " "+  ""+ ""+""+ ""+""+""+ "",null},
        new string[] { "rate","",""+ ""+" "+""+ ""+ ""+"","-1"},
        new string[] { "nbytes","",""+" "+""+"","-1"},
	    new string[] { "parsing", null,""+""+ ""+"true, "+ ""+  "","on"},
        new string[] { "ncb_quit","",""+ ""+ ""+ ""+ ""+ ""+"","-1"},
        new string[] { "l_quit","",""+"","-1"},
        new string[] { "m_quit","", ""+ "","-1"},
        new string[] { "poc_quit",null,""+"","off"},
        new string[] { "one_tp",null,""+"","off"},
        new string[] { "comp_transf",null,""+ "","on"},
        new string[] { "debug", null,"","off"},
        new string[] { "cdstr_info", null,""+""+  "", "off"},
	    new string[] { "nocolorspace",null,"","off"},
	    new string[] { "", null, ""+ "","off"}
        };
        #endregion
    }
}
