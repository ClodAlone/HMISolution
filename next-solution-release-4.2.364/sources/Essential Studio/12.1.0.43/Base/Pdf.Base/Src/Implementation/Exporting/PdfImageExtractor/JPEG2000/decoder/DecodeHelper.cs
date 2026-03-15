#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.entropy;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.quantization;
using Syncfusion.Pdf.JPEG2000.roi;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
namespace Syncfusion.Pdf.JPEG2000.decoder
{
    internal class DecodeHelper
    {
        virtual public DecodeHelper Copy
        {
            get
            {
                DecodeHelper decSpec2=null;
                try
                {
                    decSpec2 = (DecodeHelper)this.Clone();
                }
                catch (System.Exception)
                {
                    //throw new System.ApplicationException("Cannot clone the DecoderSpecs instance");
                }
                decSpec2.qts = (QuantTypeSpec)qts.Copy;
                decSpec2.qsss = (QuantStepSizeSpec)qsss.Copy;
                decSpec2.gbs = (GuardBitsSpec)gbs.Copy;
                decSpec2.wfs = (SynWTFilterSpec)wfs.Copy;
                decSpec2.dls = (IntegerSpec)dls.Copy;
                decSpec2.cts = (CompTransfSpec)cts.Copy;
                if (rois != null)
                {
                    decSpec2.rois = (MaxShiftSpec)rois.Copy;
                }
                return decSpec2;
            }
        }
        public ModuleSpec iccs;
        public MaxShiftSpec rois;
        public QuantTypeSpec qts;
        public QuantStepSizeSpec qsss;
        internal GuardBitsSpec gbs;
        public SynWTFilterSpec wfs;
        public IntegerSpec dls;
        public IntegerSpec nls;
        public IntegerSpec pos;
        public ModuleSpec ecopts;
        public CompTransfSpec cts;
        public ModuleSpec pcs;
        public ModuleSpec ers;
        public PrecinctSizeSpec pss;
        public ModuleSpec sops;
        public ModuleSpec ephs;
        public CBlkSizeSpec cblks;
        public ModuleSpec pphs;
        public DecodeHelper(int nt, int nc)
        {
            qts = new QuantTypeSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP);
            qsss = new QuantStepSizeSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP);
            gbs = new GuardBitsSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP);
            wfs = new SynWTFilterSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP);
            dls = new IntegerSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP);
            cts = new CompTransfSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP);
            ecopts = new ModuleSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP);
            ers = new ModuleSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP);
            cblks = new CBlkSizeSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP);
            pss = new PrecinctSizeSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE_COMP, dls);
            nls = new IntegerSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE);
            pos = new IntegerSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE);
            pcs = new ModuleSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE);
            sops = new ModuleSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE);
            ephs = new ModuleSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE);
            pphs = new ModuleSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE);
            iccs = new ModuleSpec(nt, nc, ModuleSpec.SPEC_TYPE_TILE);
            pphs.setDefault((System.Object)false);
        }
        virtual public System.Object Clone()
        {
            return null;
        }
    }
}