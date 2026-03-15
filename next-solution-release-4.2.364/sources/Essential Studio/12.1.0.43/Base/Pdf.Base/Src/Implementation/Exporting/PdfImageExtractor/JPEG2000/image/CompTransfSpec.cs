#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.image.invcomptransf;
using Syncfusion.Pdf.JPEG2000.wavelet;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000;
namespace Syncfusion.Pdf.JPEG2000.image
{
    internal class CompTransfSpec : ModuleSpec
    {
        virtual public bool CompTransfUsed
        {
            get
            {
                if (((System.Int32)def) != InverseComponetTransformation.NONE)
                {
                    return true;
                }
                if (tileDef != null)
                {
                    for (int t = nTiles - 1; t >= 0; t--)
                    {
                        if (tileDef[t] != null && (((System.Int32)tileDef[t]) != InverseComponetTransformation.NONE))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public CompTransfSpec(int nt, int nc, byte type)
            : base(nt, nc, type)
        {
        }
    }
}