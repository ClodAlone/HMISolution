#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.util;
namespace Syncfusion.Pdf.JPEG2000.quantization
{
    internal class QuantTypeSpec : ModuleSpec
    {
        
        public QuantTypeSpec(int nt, int nc, byte type)
            : base(nt, nc, type)
        {
        }
     
        public virtual bool isDerived(int t, int c)
        {
            if (((System.String)getTileCompVal(t, c)).Equals("derived"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public virtual bool isReversible(int t, int c)
        {
            if (((System.String)getTileCompVal(t, c)).Equals("reversible"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}