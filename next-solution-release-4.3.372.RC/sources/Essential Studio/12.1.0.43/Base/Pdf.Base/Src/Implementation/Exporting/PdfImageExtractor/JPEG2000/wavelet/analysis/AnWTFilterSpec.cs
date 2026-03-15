#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.quantization;
using Syncfusion.Pdf.JPEG2000.util;
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    internal class AnWTFilterSpec : ModuleSpec
    {
        private const System.String REV_FILTER_STR = "w5x3";
        private const System.String NON_REV_FILTER_STR = "w9x7";
        internal AnWTFilterSpec(int nt, int nc, byte type, QuantTypeSpec qts, JPXParameters pl)
            : base(nt, nc, type)
        {
            pl.checkList(AnWTFilter.OPT_PREFIX, Syncfusion.Pdf.JPEG2000.util.JPXParameters.toNameArray(AnWTFilter.ParameterInfo));
            System.String param = pl.getParameter("Ffilters");
            bool isFilterSpecified = true;
            if (param == null)
            {
                isFilterSpecified = false;
                if (pl.getBooleanParameter("lossless"))
                {
                    setDefault(parseFilters(REV_FILTER_STR));
                    return;
                }
                for (int t = nt - 1; t >= 0; t--)
                {
                    for (int c = nc - 1; c >= 0; c--)
                    {
                        switch (qts.getSpecValType(t, c))
                        {
                            case SPEC_DEF:
                                if (getDefault() == null)
                                {
                                    if (pl.getBooleanParameter("lossless"))
                                        setDefault(parseFilters(REV_FILTER_STR));
                                    if (((System.String)qts.getDefault()).Equals("reversible"))
                                    {
                                        setDefault(parseFilters(REV_FILTER_STR));
                                    }
                                    else
                                    {
                                        setDefault(parseFilters(NON_REV_FILTER_STR));
                                    }
                                }
                                specValType[t][c] = SPEC_DEF;
                                break;
                            case SPEC_COMP_DEF:
                                if (!isCompSpecified(c))
                                {
                                    if (((System.String)qts.getCompDef(c)).Equals("reversible"))
                                    {
                                        setCompDef(c, parseFilters(REV_FILTER_STR));
                                    }
                                    else
                                    {
                                        setCompDef(c, parseFilters(NON_REV_FILTER_STR));
                                    }
                                }
                                specValType[t][c] = SPEC_COMP_DEF;
                                break;
                            case SPEC_TILE_DEF:
                                if (!isTileSpecified(t))
                                {
                                    if (((System.String)qts.getTileDef(t)).Equals("reversible"))
                                    {
                                        setTileDef(t, parseFilters(REV_FILTER_STR));
                                    }
                                    else
                                    {
                                        setTileDef(t, parseFilters(NON_REV_FILTER_STR));
                                    }
                                }
                                specValType[t][c] = SPEC_TILE_DEF;
                                break;
                            case SPEC_TILE_COMP:
                                if (!isTileCompSpecified(t, c))
                                {
                                    if (((System.String)qts.getTileCompVal(t, c)).Equals("reversible"))
                                    {
                                        setTileCompVal(t, c, parseFilters(REV_FILTER_STR));
                                    }
                                    else
                                    {
                                        setTileCompVal(t, c, parseFilters(NON_REV_FILTER_STR));
                                    }
                                }
                                specValType[t][c] = SPEC_TILE_COMP;
                                break;
                            default:
                                throw new System.ArgumentException("Unsupported " + "specification " + "type");
                        }
                    }
                }
                return;
            }
            SupportClass.Tokenizer stk = new SupportClass.Tokenizer(param);
            System.String word;
            byte curSpecType = SPEC_DEF;
            bool[] tileSpec = null;
            bool[] compSpec = null;
            AnWTFilter[][] filter;
            while (stk.HasMoreTokens())
            {
                word = stk.NextToken();
                switch (word[0])
                {
                    case 't':
                    case 'T':
                        tileSpec = parseIdx(word, nTiles);
                        if (curSpecType == SPEC_COMP_DEF)
                            curSpecType = SPEC_TILE_COMP;
                        else
                            curSpecType = SPEC_TILE_DEF;
                        break;
                    case 'c':
                    case 'C':
                        compSpec = parseIdx(word, nComp);
                        if (curSpecType == SPEC_TILE_DEF)
                            curSpecType = SPEC_TILE_COMP;
                        else
                            curSpecType = SPEC_COMP_DEF;
                        break;
                    case 'w':
                    case 'W':
                        if (pl.getBooleanParameter("lossless") && word.ToUpper().Equals("w9x7".ToUpper()))
                        {
                            throw new System.ArgumentException("Cannot use non " + "reversible " + "wavelet transform with" + " '-lossless' option");
                        }
                        filter = parseFilters(word);
                        if (curSpecType == SPEC_DEF)
                        {
                            setDefault(filter);
                        }
                        else if (curSpecType == SPEC_TILE_DEF)
                        {
                            for (int i = tileSpec.Length - 1; i >= 0; i--)
                                if (tileSpec[i])
                                {
                                    setTileDef(i, filter);
                                }
                        }
                        else if (curSpecType == SPEC_COMP_DEF)
                        {
                            for (int i = compSpec.Length - 1; i >= 0; i--)
                                if (compSpec[i])
                                {
                                    setCompDef(i, filter);
                                }
                        }
                        else
                        {
                            for (int i = tileSpec.Length - 1; i >= 0; i--)
                            {
                                for (int j = compSpec.Length - 1; j >= 0; j--)
                                {
                                    if (tileSpec[i] && compSpec[j])
                                    {
                                        setTileCompVal(i, j, filter);
                                    }
                                }
                            }
                        }
                        curSpecType = SPEC_DEF;
                        tileSpec = null;
                        compSpec = null;
                        break;
                    default:
                        throw new System.ArgumentException("Bad construction for " + "parameter: " + word);
                }
            }
            if (getDefault() == null)
            {
                int ndefspec = 0;
                for (int t = nt - 1; t >= 0; t--)
                {
                    for (int c = nc - 1; c >= 0; c--)
                    {
                        if (specValType[t][c] == SPEC_DEF)
                        {
                            ndefspec++;
                        }
                    }
                }
                if (ndefspec != 0)
                {
                    if (((System.String)qts.getDefault()).Equals("reversible"))
                        setDefault(parseFilters(REV_FILTER_STR));
                    else
                        setDefault(parseFilters(NON_REV_FILTER_STR));
                }
                else
                {
                    setDefault(getTileCompVal(0, 0));
                    switch (specValType[0][0])
                    {
                        case SPEC_TILE_DEF:
                            for (int c = nc - 1; c >= 0; c--)
                            {
                                if (specValType[0][c] == SPEC_TILE_DEF)
                                    specValType[0][c] = SPEC_DEF;
                            }
                            tileDef[0] = null;
                            break;
                        case SPEC_COMP_DEF:
                            for (int t = nt - 1; t >= 0; t--)
                            {
                                if (specValType[t][0] == SPEC_COMP_DEF)
                                    specValType[t][0] = SPEC_DEF;
                            }
                            compDef[0] = null;
                            break;
                        case SPEC_TILE_COMP:
                            specValType[0][0] = SPEC_DEF;
                            tileCompVal["t0c0"] = null;
                            break;
                    }
                }
            }
            for (int t = nt - 1; t >= 0; t--)
            {
                for (int c = nc - 1; c >= 0; c--)
                {
                    if (((System.String)qts.getTileCompVal(t, c)).Equals("reversible"))
                    {
                        if (isReversible(t, c))
                            continue;
                        if (!isFilterSpecified)
                        {
                            setTileCompVal(t, c, parseFilters(REV_FILTER_STR));
                        }
                        else
                        {
                            throw new System.ArgumentException("Filter of " + "tile-component" + " (" + t + "," + c + ") does" + " not allow " + "reversible " + "quantization. " + "Specify '-Qtype " + "expounded' or " + "'-Qtype derived'" + "in " + "the command line.");
                        }
                    }
                    else
                    {
                        if (!isReversible(t, c))
                            continue;
                        if (!isFilterSpecified)
                        {
                            setTileCompVal(t, c, parseFilters(NON_REV_FILTER_STR));
                        }
                        else
                        {
                            throw new System.ArgumentException("Filter of " + "tile-component" + " (" + t + "," + c + ") does" + " not allow " + "non-reversible " + "quantization. " + "Specify '-Qtype " + "reversible' in " + "the command line");
                        }
                    }
                }
            }
        }
        private AnWTFilter[][] parseFilters(System.String word)
        {
            AnWTFilter[][] filt = new AnWTFilter[2][];
            for (int i = 0; i < 2; i++)
            {
                filt[i] = new AnWTFilter[1];
            }
            if (word.ToUpper().Equals("w5x3".ToUpper()))
            {
                filt[0][0] = new AnWTFilterIntLift5x3();
                filt[1][0] = new AnWTFilterIntLift5x3();
                return filt;
            }
            else if (word.ToUpper().Equals("w9x7".ToUpper()))
            {
                filt[0][0] = new AnWTFilterFloatLift9x7();
                filt[1][0] = new AnWTFilterFloatLift9x7();
                return filt;
            }
            else
            {
                throw new System.ArgumentException("Non JPEG 2000 part I filter: " + word);
            }
        }
        public virtual int getWTDataType(int t, int c)
        {
            AnWTFilter[][] an = (AnWTFilter[][])getSpec(t, c);
            return an[0][0].DataType;
        }
        public virtual AnWTFilter[] getHFilters(int t, int c)
        {
            AnWTFilter[][] an = (AnWTFilter[][])getSpec(t, c);
            return an[0];
        }
        public virtual AnWTFilter[] getVFilters(int t, int c)
        {
            AnWTFilter[][] an = (AnWTFilter[][])getSpec(t, c);
            return an[1];
        }
        public override System.String ToString()
        {
            System.String str = "";
            AnWTFilter[][] an;
            str += ("nTiles=" + nTiles + "\nnComp=" + nComp + "\n\n");
            for (int t = 0; t < nTiles; t++)
            {
                for (int c = 0; c < nComp; c++)
                {
                    an = (AnWTFilter[][])getSpec(t, c);
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
            AnWTFilter[] hfilter = getHFilters(t, c), vfilter = getVFilters(t, c);
            for (int i = hfilter.Length - 1; i >= 0; i--)
                if (!hfilter[i].Reversible || !vfilter[i].Reversible)
                    return false;
            return true;
        }
    }
}