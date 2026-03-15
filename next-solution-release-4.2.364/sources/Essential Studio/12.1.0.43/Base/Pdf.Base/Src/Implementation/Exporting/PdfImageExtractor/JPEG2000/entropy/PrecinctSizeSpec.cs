#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using ArrayList = System.Collections.Generic.List<object>;
namespace Syncfusion.Pdf.JPEG2000.entropy
{
    internal class PrecinctSizeSpec : ModuleSpec
    {
        private const System.String optName = "Cpp";
        private IntegerSpec dls;
        public PrecinctSizeSpec(int nt, int nc, byte type, IntegerSpec dls)
            : base(nt, nc, type)
        {
            this.dls = dls;
        }
        internal PrecinctSizeSpec(int nt, int nc, byte type, BlockImageDataSource imgsrc, IntegerSpec dls, JPXParameters pl)
            : base(nt, nc, type)
        {
            this.dls = dls;
            bool wasReadingPrecinctSize = false;
            System.String param = pl.getParameter(optName);
            ArrayList[] tmpv = new ArrayList[2];
            tmpv[0] = (new ArrayList(10));
            tmpv[0].Add((System.Int32)Syncfusion.Pdf.JPEG2000.codestream.Markers.PRECINCT_PARTITION_DEF_SIZE);
            tmpv[1] = (new ArrayList(10));
            tmpv[1].Add((System.Int32)Syncfusion.Pdf.JPEG2000.codestream.Markers.PRECINCT_PARTITION_DEF_SIZE);
            setDefault(tmpv);
            if (param == null)
            {
                return;
            }
            SupportClass.Tokenizer stk = new SupportClass.Tokenizer(param);
            byte curSpecType = SPEC_DEF;
            bool[] tileSpec = null;
            bool[] compSpec = null;
            int ci, ti;
            bool endOfParamList = false;
            System.String word = null;
            System.Int32 w, h;
            System.String errMsg = null;
            while ((stk.HasMoreTokens() || wasReadingPrecinctSize) && !endOfParamList)
            {
                ArrayList[] v = new ArrayList[2];
                if (!wasReadingPrecinctSize)
                {
                    word = stk.NextToken();
                }
                wasReadingPrecinctSize = false;
                switch (word[0])
                {
                    case 't':
                        tileSpec = parseIdx(word, nTiles);
                        if (curSpecType == SPEC_COMP_DEF)
                        {
                            curSpecType = SPEC_TILE_COMP;
                        }
                        else
                        {
                            curSpecType = SPEC_TILE_DEF;
                        }
                        break;
                    case 'c':
                        compSpec = parseIdx(word, nComp);
                        if (curSpecType == SPEC_TILE_DEF)
                        {
                            curSpecType = SPEC_TILE_COMP;
                        }
                        else
                        {
                            curSpecType = SPEC_COMP_DEF;
                        }
                        break;
                    default:
                        if (!System.Char.IsDigit(word[0]))
                        {
                            errMsg = "Bad construction for parameter: " + word;
                            throw new System.ArgumentException(errMsg);
                        }
                        v[0] = (new ArrayList(10)); // ppx
                        v[1] = (new ArrayList(10)); // ppy
                        while (true)
                        {
                            try
                            {
                                w = System.Int32.Parse(word);
                                try
                                {
                                    word = stk.NextToken();
                                }
                                catch (System.ArgumentOutOfRangeException)
                                {
                                    errMsg = "'" + optName + "' option : could not " + "parse the precinct's width";
                                    throw new System.ArgumentException(errMsg);
                                }
                                h = System.Int32.Parse(word);
                                if (w != (1 << MathUtil.log2(w)) || h != (1 << MathUtil.log2(h)))
                                {
                                    errMsg = "Precinct dimensions must be powers of 2";
                                    throw new System.ArgumentException(errMsg);
                                }
                            }
                            catch (System.FormatException)
                            {
                                errMsg = "'" + optName + "' option : the argument '" + word + "' could not be parsed.";
                                throw new System.ArgumentException(errMsg);
                            }
                            v[0].Add(w);
                            v[1].Add(h);
                            if (stk.HasMoreTokens())
                            {
                                word = stk.NextToken();
                                if (!System.Char.IsDigit(word[0]))
                                {
                                    wasReadingPrecinctSize = true;
                                    if (curSpecType == SPEC_DEF)
                                    {
                                        setDefault(v);
                                    }
                                    else if (curSpecType == SPEC_TILE_DEF)
                                    {
                                        for (ti = tileSpec.Length - 1; ti >= 0; ti--)
                                        {
                                            if (tileSpec[ti])
                                            {
                                                setTileDef(ti, v);
                                            }
                                        }
                                    }
                                    else if (curSpecType == SPEC_COMP_DEF)
                                    {
                                        for (ci = compSpec.Length - 1; ci >= 0; ci--)
                                        {
                                            if (compSpec[ci])
                                            {
                                                setCompDef(ci, v);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (ti = tileSpec.Length - 1; ti >= 0; ti--)
                                        {
                                            for (ci = compSpec.Length - 1; ci >= 0; ci--)
                                            {
                                                if (tileSpec[ti] && compSpec[ci])
                                                {
                                                    setTileCompVal(ti, ci, v);
                                                }
                                            }
                                        }
                                    }
                                    curSpecType = SPEC_DEF;
                                    tileSpec = null;
                                    compSpec = null;
                                    break;
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                if (curSpecType == SPEC_DEF)
                                {
                                    setDefault(v);
                                }
                                else if (curSpecType == SPEC_TILE_DEF)
                                {
                                    for (ti = tileSpec.Length - 1; ti >= 0; ti--)
                                    {
                                        if (tileSpec[ti])
                                        {
                                            setTileDef(ti, v);
                                        }
                                    }
                                }
                                else if (curSpecType == SPEC_COMP_DEF)
                                {
                                    for (ci = compSpec.Length - 1; ci >= 0; ci--)
                                    {
                                        if (compSpec[ci])
                                        {
                                            setCompDef(ci, v);
                                        }
                                    }
                                }
                                else
                                {
                                    for (ti = tileSpec.Length - 1; ti >= 0; ti--)
                                    {
                                        for (ci = compSpec.Length - 1; ci >= 0; ci--)
                                        {
                                            if (tileSpec[ti] && compSpec[ci])
                                            {
                                                setTileCompVal(ti, ci, v);
                                            }
                                        }
                                    }
                                }
                                endOfParamList = true;
                                break;
                            }
                        }
                        break;
                }
            }
        }
        public virtual int getPPX(int t, int c, int rl)
        {
            int mrl, idx;
            ArrayList[] v = null;
            bool tileSpecified = (t != -1 ? true : false);
            bool compSpecified = (c != -1 ? true : false);
            if (tileSpecified && compSpecified)
            {
                mrl = ((System.Int32)dls.getTileCompVal(t, c));
                v = (ArrayList[])getTileCompVal(t, c);
            }
            else if (tileSpecified && !compSpecified)
            {
                mrl = ((System.Int32)dls.getTileDef(t));
                v = (ArrayList[])getTileDef(t);
            }
            else if (!tileSpecified && compSpecified)
            {
                mrl = ((System.Int32)dls.getCompDef(c));
                v = (ArrayList[])getCompDef(c);
            }
            else
            {
                mrl = ((System.Int32)dls.getDefault());
                v = (ArrayList[])getDefault();
            }
            idx = mrl - rl;
            if (v[0].Count > idx)
            {
                return ((System.Int32)v[0][idx]);
            }
            else
            {
                return ((System.Int32)v[0][v[0].Count - 1]);
            }
        }
        public virtual int getPPY(int t, int c, int rl)
        {
            int mrl, idx;
            ArrayList[] v = null;
            bool tileSpecified = (t != -1 ? true : false);
            bool compSpecified = (c != -1 ? true : false);
            if (tileSpecified && compSpecified)
            {
                mrl = ((System.Int32)dls.getTileCompVal(t, c));
                v = (ArrayList[])getTileCompVal(t, c);
            }
            else if (tileSpecified && !compSpecified)
            {
                mrl = ((System.Int32)dls.getTileDef(t));
                v = (ArrayList[])getTileDef(t);
            }
            else if (!tileSpecified && compSpecified)
            {
                mrl = ((System.Int32)dls.getCompDef(c));
                v = (ArrayList[])getCompDef(c);
            }
            else
            {
                mrl = ((System.Int32)dls.getDefault());
                v = (ArrayList[])getDefault();
            }
            idx = mrl - rl;
            if (v[1].Count > idx)
            {
                return ((System.Int32)v[1][idx]);
            }
            else
            {
                return ((System.Int32)v[1][v[1].Count - 1]);
            }
        }
    }
}