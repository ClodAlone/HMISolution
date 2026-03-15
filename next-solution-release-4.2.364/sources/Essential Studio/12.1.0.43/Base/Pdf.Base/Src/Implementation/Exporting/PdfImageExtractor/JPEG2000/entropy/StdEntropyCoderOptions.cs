#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.entropy
{
    public struct StdEntropyCoderOptions
    {
        public readonly static int OPT_BYPASS = 1;
        public readonly static int OPT_RESET_MQ = 1 << 1;
        public readonly static int OPT_TERM_PASS = 1 << 2;
        public readonly static int OPT_VERT_STR_CAUSAL = 1 << 3;
        public readonly static int OPT_PRED_TERM = 1 << 4;
        public readonly static int OPT_SEG_SYMBOLS = 1 << 5;
        public readonly static int MIN_CB_DIM = 4;
        public readonly static int MAX_CB_DIM = 1024;
        public readonly static int MAX_CB_AREA = 4096;
        public readonly static int STRIPE_HEIGHT = 4;
        public readonly static int NUM_PASSES = 3;
        public readonly static int NUM_NON_BYPASS_MS_BP = 4;
        public readonly static int NUM_EMPTY_PASSES_IN_MS_BP = 2;
        public readonly static int FIRST_BYPASS_PASS_IDX;
        static StdEntropyCoderOptions()
        {
            FIRST_BYPASS_PASS_IDX = Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_PASSES * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_NON_BYPASS_MS_BP - Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_EMPTY_PASSES_IN_MS_BP;
        }
    }
}