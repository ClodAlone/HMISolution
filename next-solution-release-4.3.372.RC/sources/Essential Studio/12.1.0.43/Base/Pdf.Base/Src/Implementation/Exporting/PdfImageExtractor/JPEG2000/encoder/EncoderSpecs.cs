#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.quantization.quantizer;
using Syncfusion.Pdf.JPEG2000.image.forwcomptransf;
using Syncfusion.Pdf.JPEG2000.wavelet.analysis;
using Syncfusion.Pdf.JPEG2000.quantization;
using Syncfusion.Pdf.JPEG2000.entropy;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.roi;
namespace Syncfusion.Pdf.JPEG2000.encoder
{	
	internal class EncoderSpecs
	{		
		public MaxShiftSpec rois;
		public QuantTypeSpec qts;	
		public QuantStepSizeSpec qsss;		
		public GuardBitsSpec gbs;
		public AnWTFilterSpec wfs;
		public CompTransfSpec cts;
		public IntegerSpec dls;
		public StringSpec lcs;	
		public StringSpec tts;
		public StringSpec sss;
		public StringSpec css;
		public StringSpec rts;
		public StringSpec mqrs;
		public StringSpec bms;
		public PrecinctSizeSpec pss;
		public StringSpec sops;
		public StringSpec ephs;	
		public CBlkSizeSpec cblks;	
		public int nTiles;
        public int nComp;
	
	}
}