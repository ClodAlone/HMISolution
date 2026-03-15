#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    internal class SynWTFilterSpec : ModuleSpec
    {
        public SynWTFilterSpec(int nt, int nc, byte type)
            : base(nt, nc, type)
        {
        }
        public virtual int getWTDataType(int t, int c)
        {
            SynWTFilter[][] an = (SynWTFilter[][])getSpec(t, c);
            return an[0][0].DataType;
        }
        public virtual SynWTFilter[] getHFilters(int t, int c)
        {
            SynWTFilter[][] an = (SynWTFilter[][])getSpec(t, c);
            return an[0];
        }
        public virtual SynWTFilter[] getVFilters(int t, int c)
        {
            SynWTFilter[][] an = (SynWTFilter[][])getSpec(t, c);
            return an[1];
        }
        public override System.String ToString()
        {
            System.String str = "";
            SynWTFilter[][] an;
            str += ("nTiles=" + nTiles + "\nnComp=" + nComp + "\n\n");
            for (int t = 0; t < nTiles; t++)
            {
                for (int c = 0; c < nComp; c++)
                {
                    an = (SynWTFilter[][])getSpec(t, c);
                    str += ("(t:" + t + ",c:" + c + ")\n");
                    str += "\tH:";
                    for (int i = 0; i < an[0].Length; i++)
                    {
                        str += (" " + an[0][i]);
                    }
                    str += "\n\tV:";
                    for (int i = 0; i < an[1].Length; i++)
                    {
                        str += (" " + an[1][i]);
                    }
                    str += "\n";
                }
            }
            return str;
        }
        public virtual bool isReversible(int t, int c)
        {
            SynWTFilter[] hfilter = getHFilters(t, c), vfilter = getVFilters(t, c);
            for (int i = hfilter.Length - 1; i >= 0; i--)
                if (!hfilter[i].Reversible || !vfilter[i].Reversible)
                    return false;
            return true;
        }
    }
}