#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.codestream;
namespace Syncfusion.Pdf.JPEG2000.entropy
{
    internal class Progression
    {
        public int type;
        public int cs;
        public int ce;
        public int rs;
        public int re;
        public int lye;
        public Progression(int type, int cs, int ce, int rs, int re, int lye)
        {
            this.type = type;
            this.cs = cs;
            this.ce = ce;
            this.rs = rs;
            this.re = re;
            this.lye = lye;
        }
        public override System.String ToString()
        {
            System.String str = "type= ";
            switch (type)
            {
                case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.LY_RES_COMP_POS_PROG:
                    str += "layer, ";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_LY_COMP_POS_PROG:
                    str += "res, ";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_POS_COMP_LY_PROG:
                    str += "res-pos, ";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.POS_COMP_RES_LY_PROG:
                    str += "pos-comp, ";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.COMP_POS_RES_LY_PROG:
                    str += "pos-comp, ";
                    break;
                //default:
                    //throw new System.ApplicationException("Unknown progression type");
            }
            str += ("comp.: " + cs + "-" + ce + ", ");
            str += ("res.: " + rs + "-" + re + ", ");
            str += ("layer: up to " + lye);
            return str;
        }
    }
}