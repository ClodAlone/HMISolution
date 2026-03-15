#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image.invcomptransf;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.quantization.dequantizer
{
    public abstract class Dequantizer : MultiResImgDataAdapter, CBlkWTDataSrcDec
    {
        virtual public int CbULX
        {
            get
            {
                return src.CbULX;
            }
        }
        virtual public int CbULY
        {
            get
            {
                return src.CbULY;
            }
        }
        public static System.String[][] ParameterInfo
        {
            get
            {
                return pinfo;
            }
        }
        public const char OPT_PREFIX = 'Q';
        private static readonly System.String[][] pinfo = null;
        internal CBlkQuantDataSrcDec src;
        internal int[] rb = null;
        internal int[] utrb = null;
        private CompTransfSpec cts;
        private SynWTFilterSpec wfs;
        internal Dequantizer(CBlkQuantDataSrcDec src, int[] utrb, DecodeHelper decSpec)
            : base(src)
        {
            if (utrb.Length != src.NumComps)
            {
                throw new System.ArgumentException();
            }
            this.src = src;
            this.utrb = utrb;
            this.cts = decSpec.cts;
            this.wfs = decSpec.wfs;
        }
        public virtual int getNomRangeBits(int c)
        {
            return rb[c];
        }
        public override SubbandSyn getSynSubbandTree(int t, int c)
        {
            return src.getSynSubbandTree(t, c);
        }
        public override void setTile(int x, int y)
        {
            src.setTile(x, y);
            tIdx = TileIdx;
            int cttype = 0;
            if (((System.Int32)cts.getTileDef(tIdx)) == InverseComponetTransformation.NONE)
                cttype = InverseComponetTransformation.NONE;
            else
            {
                int nc = src.NumComps > 3 ? 3 : src.NumComps;
                int rev = 0;
                for (int c = 0; c < nc; c++)
                    rev += (wfs.isReversible(tIdx, c) ? 1 : 0);
                if (rev == 3)
                {
                    cttype = InverseComponetTransformation.INV_RCT;
                }
                else if (rev == 0)
                {
                    cttype = InverseComponetTransformation.INV_ICT;
                }
                else
                {
                    throw new System.ArgumentException("Wavelet transformation " + "and " + "component transformation" + " not coherent in tile" + tIdx);
                }
            }
            switch (cttype)
            {
                case InverseComponetTransformation.NONE:
                    rb = utrb;
                    break;
                case InverseComponetTransformation.INV_RCT:
                    rb = InverseComponetTransformation.calcMixedBitDepths(utrb, InverseComponetTransformation.INV_RCT, null);
                    break;
                case InverseComponetTransformation.INV_ICT:
                    rb = InverseComponetTransformation.calcMixedBitDepths(utrb, InverseComponetTransformation.INV_ICT, null);
                    break;
                default:
                    throw new System.ArgumentException("Non JPEG 2000 part I " + "component" + " transformation for tile: " + tIdx);
            }
        }
        public override void nextTile()
        {
            src.nextTile();
            tIdx = TileIdx;
            int cttype = ((System.Int32)cts.getTileDef(tIdx));
            switch (cttype)
            {
                case InverseComponetTransformation.NONE:
                    rb = utrb;
                    break;
                case InverseComponetTransformation.INV_RCT:
                    rb = InverseComponetTransformation.calcMixedBitDepths(utrb, InverseComponetTransformation.INV_RCT, null);
                    break;
                case InverseComponetTransformation.INV_ICT:
                    rb = InverseComponetTransformation.calcMixedBitDepths(utrb, InverseComponetTransformation.INV_ICT, null);
                    break;
                default:
                    throw new System.ArgumentException("Non JPEG 2000 part I " + "component" + " transformation for tile: " + tIdx);
            }
        }
        public abstract Syncfusion.Pdf.JPEG2000.image.DataBlock getCodeBlock(int param1, int param2, int param3, Syncfusion.Pdf.JPEG2000.wavelet.synthesis.SubbandSyn param4, Syncfusion.Pdf.JPEG2000.image.DataBlock param5);
        public abstract int getFixedPoint(int param1);
        public abstract Syncfusion.Pdf.JPEG2000.image.DataBlock getInternCodeBlock(int param1, int param2, int param3, Syncfusion.Pdf.JPEG2000.wavelet.synthesis.SubbandSyn param4, Syncfusion.Pdf.JPEG2000.image.DataBlock param5);
    }
}