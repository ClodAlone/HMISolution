#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.wavelet;
using HashTable = System.Collections.Generic.Dictionary<object, object>;
using ArrayList = System.Collections.Generic.List<object>;
namespace Syncfusion.Pdf.JPEG2000.codestream
{
    internal class HeaderInformation
    {
        virtual public SIZ NewSIZ
        {
            get
            {
                return new SIZ(this);
            }
        }
        virtual public SOT NewSOT
        {
            get
            {
                return new SOT(this);
            }
        }
        virtual public COD NewCOD
        {
            get
            {
                return new COD(this);
            }
        }
        virtual public COC NewCOC
        {
            get
            {
                return new COC(this);
            }
        }
        virtual public RGN NewRGN
        {
            get
            {
                return new RGN(this);
            }
        }
        virtual public QCD NewQCD
        {
            get
            {
                return new QCD(this);
            }
        }
        virtual public QCC NewQCC
        {
            get
            {
                return new QCC(this);
            }
        }
        virtual public POC NewPOC
        {
            get
            {
                return new POC(this);
            }
        }
        virtual public CRG NewCRG
        {
            get
            {
                return new CRG(this);
            }
        }
        virtual public COM NewCOM
        {
            get
            {
                ncom++; return new COM(this);
            }
        }
        virtual public int NumCOM
        {
            get
            {
                return ncom;
            }
        }
        internal class SIZ 
        {
            public SIZ(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            virtual public int MaxCompWidth
            {
                get
                {
                    if (compWidth == null)
                    {
                        compWidth = new int[csiz];
                        for (int cc = 0; cc < csiz; cc++)
                        {
                            compWidth[cc] = (int)(System.Math.Ceiling((xsiz) / (double)xrsiz[cc]) - System.Math.Ceiling(x0siz / (double)xrsiz[cc]));
                        }
                    }
                    if (maxCompWidth == -1)
                    {
                        for (int c = 0; c < csiz; c++)
                        {
                            if (compWidth[c] > maxCompWidth)
                            {
                                maxCompWidth = compWidth[c];
                            }
                        }
                    }
                    return maxCompWidth;
                }
            }
            virtual public int MaxCompHeight
            {
                get
                {
                    if (compHeight == null)
                    {
                        compHeight = new int[csiz];
                        for (int cc = 0; cc < csiz; cc++)
                        {
                            compHeight[cc] = (int)(System.Math.Ceiling((ysiz) / (double)yrsiz[cc]) - System.Math.Ceiling(y0siz / (double)yrsiz[cc]));
                        }
                    }
                    if (maxCompHeight == -1)
                    {
                        for (int c = 0; c < csiz; c++)
                        {
                            if (compHeight[c] != maxCompHeight)
                            {
                                maxCompHeight = compHeight[c];
                            }
                        }
                    }
                    return maxCompHeight;
                }
            }
            virtual public int NumTiles
            {
                get
                {
                    if (numTiles == -1)
                    {
                        numTiles = ((xsiz - xt0siz + xtsiz - 1) / xtsiz) * ((ysiz - yt0siz + ytsiz - 1) / ytsiz);
                    }
                    return numTiles;
                }
            }
            virtual public SIZ Copy
            {
                get
                {
                    SIZ ms = null;
                    try
                    {
                        ms = (SIZ)this.Clone();
                    }
                    catch (System.Exception)
                    {
                        //throw new System.ApplicationException("Cannot clone SIZ marker segment");
                    }
                    return ms;
                }
            }
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lsiz;
            public int rsiz;
            public int xsiz;
            public int ysiz;
            public int x0siz;
            public int y0siz;
            public int xtsiz;
            public int ytsiz;
            public int xt0siz;
            public int yt0siz;
            public int csiz;
            public int[] ssiz;
            public int[] xrsiz;
            public int[] yrsiz;
            private int[] compWidth = null;
            private int maxCompWidth = -1;
            private int[] compHeight = null;
            private int maxCompHeight = -1;
            public virtual int getCompImgWidth(int c)
            {
                if (compWidth == null)
                {
                    compWidth = new int[csiz];
                    for (int cc = 0; cc < csiz; cc++)
                    {
                        compWidth[cc] = (int)(System.Math.Ceiling((xsiz) / (double)xrsiz[cc]) - System.Math.Ceiling(x0siz / (double)xrsiz[cc]));
                    }
                }
                return compWidth[c];
            }
            public virtual int getCompImgHeight(int c)
            {
                if (compHeight == null)
                {
                    compHeight = new int[csiz];
                    for (int cc = 0; cc < csiz; cc++)
                    {
                        compHeight[cc] = (int)(System.Math.Ceiling((ysiz) / (double)yrsiz[cc]) - System.Math.Ceiling(y0siz / (double)yrsiz[cc]));
                    }
                }
                return compHeight[c];
            }
            private int numTiles = -1;
            private bool[] origSigned = null;
            public virtual bool isOrigSigned(int c)
            {
                if (origSigned == null)
                {
                    origSigned = new bool[csiz];
                    for (int cc = 0; cc < csiz; cc++)
                    {
                        origSigned[cc] = ((SupportClass.URShift(ssiz[cc], Syncfusion.Pdf.JPEG2000.codestream.Markers.SSIZ_DEPTH_BITS)) == 1);
                    }
                }
                return origSigned[c];
            }
            private int[] origBitDepth = null;
            public virtual int getOrigBitDepth(int c)
            {
                if (origBitDepth == null)
                {
                    origBitDepth = new int[csiz];
                    for (int cc = 0; cc < csiz; cc++)
                    {
                        origBitDepth[cc] = (ssiz[cc] & ((1 << Syncfusion.Pdf.JPEG2000.codestream.Markers.SSIZ_DEPTH_BITS) - 1)) + 1;
                    }
                }
                return origBitDepth[c];
            }
            public override System.String ToString()
            {
                System.String str = "\n --- SIZ (" + lsiz + " bytes) ---\n";
                str += (" Capabilities : " + rsiz + "\n");
                str += (" Image dim.   : " + (xsiz - x0siz) + "x" + (ysiz - y0siz) + ", (off=" + x0siz + "," + y0siz + ")\n");
                str += (" Tile dim.    : " + xtsiz + "x" + ytsiz + ", (off=" + xt0siz + "," + yt0siz + ")\n");
                str += (" Component(s) : " + csiz + "\n");
                str += " Orig. depth  : ";
                for (int i = 0; i < csiz; i++)
                {
                    str += (getOrigBitDepth(i) + " ");
                }
                str += "\n";
                str += " Orig. signed : ";
                for (int i = 0; i < csiz; i++)
                {
                    str += (isOrigSigned(i) + " ");
                }
                str += "\n";
                str += " Subs. factor : ";
                for (int i = 0; i < csiz; i++)
                {
                    str += (xrsiz[i] + "," + yrsiz[i] + " ");
                }
                str += "\n";
                return str;
            }
            virtual public System.Object Clone()
            {
                return null;
            }
        }
        internal class SOT
        {
            public SOT(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lsot;
            public int isot;
            public int psot;
            public int tpsot;
            public int tnsot;
            public override System.String ToString()
            {
                System.String str = "\n --- SOT (" + lsot + " bytes) ---\n";
                str += ("Tile index         : " + isot + "\n");
                str += ("Tile-part length   : " + psot + " bytes\n");
                str += ("Tile-part index    : " + tpsot + "\n");
                str += ("Num. of tile-parts : " + tnsot + "\n");
                str += "\n";
                return str;
            }
        }
        internal class COD //: System.ICloneable
        {
            public COD(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            virtual public COD Copy
            {
                get
                {
                    COD ms = null;
                    try
                    {
                        ms = (COD)this.Clone();
                    }
                    catch (System.Exception)
                    {
                        //throw new System.ApplicationException("Cannot clone SIZ marker segment");
                    }
                    return ms;
                }
            }
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lcod;
            public int scod;
            public int sgcod_po; // Progression order
            public int sgcod_nl; // Number of layers
            public int sgcod_mct; // Multiple component transformation
            public int spcod_ndl; // Number of decomposition levels
            public int spcod_cw; // Code-blocks width
            public int spcod_ch; // Code-blocks height
            public int spcod_cs; // Code-blocks style
            public int[] spcod_t = new int[1]; // Transformation
            public int[] spcod_ps; // Precinct size
            /// <summary>Display information found in this COD marker segment </summary>
            public override System.String ToString()
            {
                System.String str = "\n --- COD (" + lcod + " bytes) ---\n";
                str += " Coding style   : ";
                if (scod == 0)
                {
                    str += "Default";
                }
                else
                {
                    if ((scod & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_PRECINCT_PARTITION) != 0)
                        str += "Precints ";
                    if ((scod & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_SOP) != 0)
                        str += "SOP ";
                    if ((scod & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_EPH) != 0)
                        str += "EPH ";
                    int cb0x = ((scod & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_HOR_CB_PART) != 0) ? 1 : 0;
                    int cb0y = ((scod & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_VER_CB_PART) != 0) ? 1 : 0;
                    if (cb0x != 0 || cb0y != 0)
                    {
                        str += "Code-blocks offset";
                        str += ("\n Cblk partition : " + cb0x + "," + cb0y);
                    }
                }
                str += "\n";
                str += " Cblk style     : ";
                if (spcod_cs == 0)
                {
                    str += "Default";
                }
                else
                {
                    if ((spcod_cs & 0x1) != 0)
                        str += "Bypass ";
                    if ((spcod_cs & 0x2) != 0)
                        str += "Reset ";
                    if ((spcod_cs & 0x4) != 0)
                        str += "Terminate ";
                    if ((spcod_cs & 0x8) != 0)
                        str += "Vert_causal ";
                    if ((spcod_cs & 0x10) != 0)
                        str += "Predict ";
                    if ((spcod_cs & 0x20) != 0)
                        str += "Seg_symb ";
                }
                str += "\n";
                str += (" Num. of levels : " + spcod_ndl + "\n");
                switch (sgcod_po)
                {
                    case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.LY_RES_COMP_POS_PROG:
                        str += " Progress. type : LY_RES_COMP_POS_PROG\n";
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_LY_COMP_POS_PROG:
                        str += " Progress. type : RES_LY_COMP_POS_PROG\n";
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_POS_COMP_LY_PROG:
                        str += " Progress. type : RES_POS_COMP_LY_PROG\n";
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.POS_COMP_RES_LY_PROG:
                        str += " Progress. type : POS_COMP_RES_LY_PROG\n";
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.COMP_POS_RES_LY_PROG:
                        str += " Progress. type : COMP_POS_RES_LY_PROG\n";
                        break;
                }
                str += (" Num. of layers : " + sgcod_nl + "\n");
                str += (" Cblk dimension : " + (1 << (spcod_cw + 2)) + "x" + (1 << (spcod_ch + 2)) + "\n");
                switch (spcod_t[0])
                {
                    case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W9X7:
                        str += " Filter         : 9-7 irreversible\n";
                        break;
                    case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W5X3:
                        str += " Filter         : 5-3 reversible\n";
                        break;
                }
                str += (" Multi comp tr. : " + (sgcod_mct == 1) + "\n");
                if (spcod_ps != null)
                {
                    str += " Precincts      : ";
                    for (int i = 0; i < spcod_ps.Length; i++)
                    {
                        str += ((1 << (spcod_ps[i] & 0x000F)) + "x" + (1 << (((spcod_ps[i] & 0x00F0) >> 4))) + " ");
                    }
                }
                str += "\n";
                return str;
            }
            virtual public System.Object Clone()
            {
                return null;
            }
        }
        internal class COC
        {
            public COC(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lcoc;
            public int ccoc;
            public int scoc;
            public int spcoc_ndl; // Number of decomposition levels
            public int spcoc_cw;
            public int spcoc_ch;
            public int spcoc_cs;
            public int[] spcoc_t = new int[1];
            public int[] spcoc_ps;
            /// <summary>Display information found in this COC marker segment </summary>
            public override System.String ToString()
            {
                System.String str = "\n --- COC (" + lcoc + " bytes) ---\n";
                str += (" Component      : " + ccoc + "\n");
                str += " Coding style   : ";
                if (scoc == 0)
                {
                    str += "Default";
                }
                else
                {
                    if ((scoc & 0x1) != 0)
                        str += "Precints ";
                    if ((scoc & 0x2) != 0)
                        str += "SOP ";
                    if ((scoc & 0x4) != 0)
                        str += "EPH ";
                }
                str += "\n";
                str += " Cblk style     : ";
                if (spcoc_cs == 0)
                {
                    str += "Default";
                }
                else
                {
                    if ((spcoc_cs & 0x1) != 0)
                        str += "Bypass ";
                    if ((spcoc_cs & 0x2) != 0)
                        str += "Reset ";
                    if ((spcoc_cs & 0x4) != 0)
                        str += "Terminate ";
                    if ((spcoc_cs & 0x8) != 0)
                        str += "Vert_causal ";
                    if ((spcoc_cs & 0x10) != 0)
                        str += "Predict ";
                    if ((spcoc_cs & 0x20) != 0)
                        str += "Seg_symb ";
                }
                str += "\n";
                str += (" Num. of levels : " + spcoc_ndl + "\n");
                str += (" Cblk dimension : " + (1 << (spcoc_cw + 2)) + "x" + (1 << (spcoc_ch + 2)) + "\n");
                switch (spcoc_t[0])
                {
                    case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W9X7:
                        str += " Filter         : 9-7 irreversible\n";
                        break;
                    case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W5X3:
                        str += " Filter         : 5-3 reversible\n";
                        break;
                }
                if (spcoc_ps != null)
                {
                    str += " Precincts      : ";
                    for (int i = 0; i < spcoc_ps.Length; i++)
                    {
                        str += ((1 << (spcoc_ps[i] & 0x000F)) + "x" + (1 << (((spcoc_ps[i] & 0x00F0) >> 4))) + " ");
                    }
                }
                str += "\n";
                return str;
            }
        }
        internal class RGN
        {
            public RGN(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lrgn;
            public int crgn;
            public int srgn;
            public int sprgn;
            /// <summary>Display information found in this RGN marker segment </summary>
            public override System.String ToString()
            {
                System.String str = "\n --- RGN (" + lrgn + " bytes) ---\n";
                str += (" Component : " + crgn + "\n");
                if (srgn == 0)
                {
                    str += " ROI style : Implicit\n";
                }
                else
                {
                    str += " ROI style : Unsupported\n";
                }
                str += (" ROI shift : " + sprgn + "\n");
                str += "\n";
                return str;
            }
        }
        internal class QCD
        {
            public QCD(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            virtual public int QuantType
            {
                get
                {
                    if (qType == -1)
                    {
                        qType = sqcd & ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_GB_MSK << Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_GB_SHIFT);
                    }
                    return qType;
                }
            }
            virtual public int NumGuardBits
            {
                get
                {
                    if (gb == -1)
                    {
                        gb = (sqcd >> Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_GB_SHIFT) & Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_GB_MSK;
                    }
                    return gb;
                }
            }
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lqcd;
            public int sqcd;
            public int[][] spqcd;
            private int qType = -1;
            private int gb = -1;
            public override System.String ToString()
            {
                System.String str = "\n --- QCD (" + lqcd + " bytes) ---\n";
                str += " Quant. type    : ";
                int qt = QuantType;
                if (qt == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION)
                    str += "No quantization \n";
                else if (qt == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_DERIVED)
                    str += "Scalar derived\n";
                else if (qt == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_EXPOUNDED)
                    str += "Scalar expounded\n";
                str += (" Guard bits     : " + NumGuardBits + "\n");
                if (qt == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION)
                {
                    str += " Exponents   :\n";
                    int exp;
                    for (int i = 0; i < spqcd.Length; i++)
                    {
                        for (int j = 0; j < spqcd[i].Length; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                exp = (spqcd[0][0] >> Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_SHIFT) & Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_MASK;
                                str += ("\tr=0 : " + exp + "\n");
                            }
                            else if (i != 0 && j > 0)
                            {
                                exp = (spqcd[i][j] >> Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_SHIFT) & Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_MASK;
                                str += ("\tr=" + i + ",s=" + j + " : " + exp + "\n");
                            }
                        }
                    }
                }
                else
                {
                    str += " Exp / Mantissa : \n";
                    int exp;
                    double mantissa;
                    for (int i = 0; i < spqcd.Length; i++)
                    {
                        for (int j = 0; j < spqcd[i].Length; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                exp = (spqcd[0][0] >> 11) & 0x1f;
                                mantissa = (-1f - ((float)(spqcd[0][0] & 0x07ff)) / (1 << 11)) / (-1 << exp);
                                str += ("\tr=0 : " + exp + " / " + mantissa + "\n");
                            }
                            else if (i != 0 && j > 0)
                            {
                                exp = (spqcd[i][j] >> 11) & 0x1f;
                                mantissa = (-1f - ((float)(spqcd[i][j] & 0x07ff)) / (1 << 11)) / (-1 << exp);
                                str += ("\tr=" + i + ",s=" + j + " : " + exp + " / " + mantissa + "\n");
                            }
                        }
                    }
                }
                str += "\n";
                return str;
            }
        }
        internal class QCC
        {
            public QCC(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            virtual public int QuantType
            {
                get
                {
                    if (qType == -1)
                    {
                        qType = sqcc & ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_GB_MSK << Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_GB_SHIFT);
                    }
                    return qType;
                }
            }
            virtual public int NumGuardBits
            {
                get
                {
                    if (gb == -1)
                    {
                        gb = (sqcc >> Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_GB_SHIFT) & Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_GB_MSK;
                    }
                    return gb;
                }
            }
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lqcc;
            public int cqcc;
            public int sqcc;
            public int[][] spqcc;
            private int qType = -1;
            private int gb = -1;
            public override System.String ToString()
            {
                System.String str = "\n --- QCC (" + lqcc + " bytes) ---\n";
                str += (" Component      : " + cqcc + "\n");
                str += " Quant. type    : ";
                int qt = QuantType;
                if (qt == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION)
                    str += "No quantization \n";
                else if (qt == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_DERIVED)
                    str += "Scalar derived\n";
                else if (qt == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_EXPOUNDED)
                    str += "Scalar expounded\n";
                str += (" Guard bits     : " + NumGuardBits + "\n");
                if (qt == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION)
                {
                    str += " Exponents   :\n";
                    int exp;
                    for (int i = 0; i < spqcc.Length; i++)
                    {
                        for (int j = 0; j < spqcc[i].Length; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                exp = (spqcc[0][0] >> Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_SHIFT) & Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_MASK;
                                str += ("\tr=0 : " + exp + "\n");
                            }
                            else if (i != 0 && j > 0)
                            {
                                exp = (spqcc[i][j] >> Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_SHIFT) & Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_MASK;
                                str += ("\tr=" + i + ",s=" + j + " : " + exp + "\n");
                            }
                        }
                    }
                }
                else
                {
                    str += " Exp / Mantissa : \n";
                    int exp;
                    double mantissa;
                    for (int i = 0; i < spqcc.Length; i++)
                    {
                        for (int j = 0; j < spqcc[i].Length; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                exp = (spqcc[0][0] >> 11) & 0x1f;
                                mantissa = (-1f - ((float)(spqcc[0][0] & 0x07ff)) / (1 << 11)) / (-1 << exp);
                                str += ("\tr=0 : " + exp + " / " + mantissa + "\n");
                            }
                            else if (i != 0 && j > 0)
                            {
                                exp = (spqcc[i][j] >> 11) & 0x1f;
                                mantissa = (-1f - ((float)(spqcc[i][j] & 0x07ff)) / (1 << 11)) / (-1 << exp);
                                str += ("\tr=" + i + ",s=" + j + " : " + exp + " / " + mantissa + "\n");
                            }
                        }
                    }
                }
                str += "\n";
                return str;
            }
        }
        internal class POC
        {
            public POC(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lpoc;
            public int[] rspoc;
            public int[] cspoc;
            public int[] lyepoc;
            public int[] repoc;
            public int[] cepoc;
            public int[] ppoc;
            public override System.String ToString()
            {
                System.String str = "\n --- POC (" + lpoc + " bytes) ---\n";
                str += " Chg_idx RSpoc CSpoc LYEpoc REpoc CEpoc Ppoc\n";
                for (int chg = 0; chg < rspoc.Length; chg++)
                {
                    str += ("   " + chg + "      " + rspoc[chg] + "     " + cspoc[chg] + "     " + lyepoc[chg] + "      " + repoc[chg] + "     " + cepoc[chg]);
                    switch (ppoc[chg])
                    {
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.LY_RES_COMP_POS_PROG:
                            str += "  LY_RES_COMP_POS_PROG\n";
                            break;
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_LY_COMP_POS_PROG:
                            str += "  RES_LY_COMP_POS_PROG\n";
                            break;
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_POS_COMP_LY_PROG:
                            str += "  RES_POS_COMP_LY_PROG\n";
                            break;
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.POS_COMP_RES_LY_PROG:
                            str += "  POS_COMP_RES_LY_PROG\n";
                            break;
                        case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.COMP_POS_RES_LY_PROG:
                            str += "  COMP_POS_RES_LY_PROG\n";
                            break;
                    }
                }
                str += "\n";
                return str;
            }
        }
        internal class CRG
        {
            public CRG(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lcrg;
            public int[] xcrg;
            public int[] ycrg;
            public override System.String ToString()
            {
                System.String str = "\n --- CRG (" + lcrg + " bytes) ---\n";
                for (int c = 0; c < xcrg.Length; c++)
                {
                    str += (" Component " + c + " offset : " + xcrg[c] + "," + ycrg[c] + "\n");
                }
                str += "\n";
                return str;
            }
        }
        internal class COM
        {
            public COM(HeaderInformation enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }
            private void InitBlock(HeaderInformation enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private HeaderInformation enclosingInstance;
            public HeaderInformation Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
            public int lcom;
            public int rcom;
            public byte[] ccom;
            public override System.String ToString()
            {
                System.String str = "\n --- COM (" + lcom + " bytes) ---\n";
                if (rcom == 0)
                {
                    str += " Registration : General use (binary values)\n";
                }
                else if (rcom == 1)
                {
                    str += (" Registration : General use (IS 8859-15:1999 " + "(Latin) values)\n");
                    //str += (" Text         : " + System.Text.ASCIIEncoding.ASCII.GetString(ccom) + "\n");
                }
                else
                {
                    str += " Registration : Unknown\n";
                }
                str += "\n";
                return str;
            }
        }
        public SIZ sizValue;
        public HashTable sotValue = new HashTable();

        public HashTable codValue = new HashTable();
        public HashTable cocValue = new HashTable();
        public HashTable rgnValue = new HashTable();
        public HashTable qcdValue = new HashTable();
        public HashTable qccValue = new HashTable();
        public HashTable pocValue = new HashTable();
        public CRG crgValue;
        public HashTable comValue = new HashTable();
        private int ncom = 0;
        public virtual System.String toStringMainHeader()
        {
            int nc = sizValue.csiz;
            // SIZ
            System.String str = "" + sizValue;
            // COD
            if (codValue["main"] != null)
            {
                str += ("" + ((COD)codValue["main"]));
            }
            // COCs
            for (int c = 0; c < nc; c++)
            {
                if (cocValue["main_c" + c] != null)
                {
                    str += ("" + ((COC)cocValue["main_c" + c]));
                }
            }
            // QCD
            if (qcdValue["main"] != null)
            {
                str += ("" + ((QCD)qcdValue["main"]));
            }
            // QCCs
            for (int c = 0; c < nc; c++)
            {
                if (qccValue["main_c" + c] != null)
                {
                    str += ("" + ((QCC)qccValue["main_c" + c]));
                }
            }
            // RGN
            for (int c = 0; c < nc; c++)
            {
                if (rgnValue["main_c" + c] != null)
                {
                    str += ("" + ((RGN)rgnValue["main_c" + c]));
                }
            }
            // POC
            if (pocValue["main"] != null)
            {
                str += ("" + ((POC)pocValue["main"]));
            }
            // CRG
            if (crgValue != null)
            {
                str += ("" + crgValue);
            }
            // COM
            for (int i = 0; i < ncom; i++)
            {
                if (comValue["main_" + i] != null)
                {
                    str += ("" + ((COM)comValue["main_" + i]));
                }
            }
            return str;
        }
        public virtual System.String toStringTileHeader(int t, int ntp)
        {
            int nc = sizValue.csiz;
            System.String str = "";
            // SOT
            for (int i = 0; i < ntp; i++)
            {
                str += ("Tile-part " + i + ", tile " + t + ":\n");
                str += ("" + ((SOT)sotValue["t" + t + "_tp" + i]));
            }
            // COD
            if (codValue["t" + t] != null)
            {
                str += ("" + ((COD)codValue["t" + t]));
            }
            // COCs
            for (int c = 0; c < nc; c++)
            {
                if (cocValue["t" + t + "_c" + c] != null)
                {
                    str += ("" + ((COC)cocValue["t" + t + "_c" + c]));
                }
            }
            // QCD
            if (qcdValue["t" + t] != null)
            {
                str += ("" + ((QCD)qcdValue["t" + t]));
            }
            // QCCs
            for (int c = 0; c < nc; c++)
            {
                if (qccValue["t" + t + "_c" + c] != null)
                {
                    str += ("" + ((QCC)qccValue["t" + t + "_c" + c]));
                }
            }
            // RGN
            for (int c = 0; c < nc; c++)
            {
                if (rgnValue["t" + t + "_c" + c] != null)
                {
                    str += ("" + ((RGN)rgnValue["t" + t + "_c" + c]));
                }
            }
            // POC
            if (pocValue["t" + t] != null)
            {
                str += ("" + ((POC)pocValue["t" + t]));
            }
            return str;
        }
        public virtual System.String toStringThNoSOT(int t, int ntp)
        {
            int nc = sizValue.csiz;
            System.String str = "";
            // COD
            if (codValue["t" + t] != null)
            {
                str += ("" + ((COD)codValue["t" + t]));
            }
            // COCs
            for (int c = 0; c < nc; c++)
            {
                if (cocValue["t" + t + "_c" + c] != null)
                {
                    str += ("" + ((COC)cocValue["t" + t + "_c" + c]));
                }
            }
            // QCD
            if (qcdValue["t" + t] != null)
            {
                str += ("" + ((QCD)qcdValue["t" + t]));
            }
            // QCCs
            for (int c = 0; c < nc; c++)
            {
                if (qccValue["t" + t + "_c" + c] != null)
                {
                    str += ("" + ((QCC)qccValue["t" + t + "_c" + c]));
                }
            }
            // RGN
            for (int c = 0; c < nc; c++)
            {
                if (rgnValue["t" + t + "_c" + c] != null)
                {
                    str += ("" + ((RGN)rgnValue["t" + t + "_c" + c]));
                }
            }
            // POC
            if (pocValue["t" + t] != null)
            {
                str += ("" + ((POC)pocValue["t" + t]));
            }
            return str;
        }
        public virtual HeaderInformation getCopy(int nt)
        {
            HeaderInformation nhi = null;
            // SIZ
            try
            {
                nhi = (HeaderInformation)Clone();
            }
            catch (System.Exception)
            {
                //throw new System.ApplicationException("Cannot clone HeaderInfo instance");
            }
            nhi.sizValue = sizValue.Copy;
            // COD
            if (codValue["main"] != null)
            {
                COD ms = (COD)codValue["main"];
                nhi.codValue["main"] = ms.Copy;
            }
            for (int t = 0; t < nt; t++)
            {
                if (codValue["t" + t] != null)
                {
                    COD ms = (COD)codValue["t" + t];
                    nhi.codValue["t" + t] = ms.Copy;
                }
            }
            return nhi;
        }
        virtual public System.Object Clone()
        {
            return null;
        }
    }
}