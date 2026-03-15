#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.codestream;
using Syncfusion.Pdf.JPEG2000.util;
namespace Syncfusion.Pdf.JPEG2000.entropy
{
    internal class ProgressionSpec : ModuleSpec
    {
        public ProgressionSpec(int nt, int nc, byte type)
            : base(nt, nc, type)
        {
            if (type != ModuleSpec.SPEC_TYPE_TILE)
            {
                throw new System.ApplicationException("Illegal use of class ProgressionSpec !");
            }
        }
        internal ProgressionSpec(int nt, int nc, int nl, IntegerSpec dls, byte type, JPXParameters pl)
            : base(nt, nc, type)
        {
            System.String param = pl.getParameter("Aptype");
            Progression[] prog;
            int mode = -1;
            if (param == null)
            {
                if (pl.getParameter("Rroi") == null)
                {
                    mode = checkProgMode("res");
                }
                else
                {
                    mode = checkProgMode("layer");
                }
                if (mode == -1)
                {
                    System.String errMsg = "Unknown progression type : '" + param + "'";
                    throw new System.ArgumentException(errMsg);
                }
                prog = new Progression[1];
                prog[0] = new Progression(mode, 0, nc, 0, dls.Max + 1, nl);
                setDefault(prog);
                return;
            }
            SupportClass.Tokenizer stk = new SupportClass.Tokenizer(param);
            byte curSpecType = SPEC_DEF;
            bool[] tileSpec = null;
            System.String word = null;
            System.String errMsg2 = null;
            bool needInteger = false;
            int intType = 0;
            System.Collections.ArrayList progression = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
            int tmp = 0;
            Progression curProg = null;
            while (stk.HasMoreTokens())
            {
                word = stk.NextToken();
                switch (word[0])
                {
                    case 't':
                        if (progression.Count > 0)
                        {
                            curProg.ce = nc;
                            curProg.lye = nl;
                            curProg.re = dls.Max + 1;
                            prog = new Progression[progression.Count];
                            progression.CopyTo(prog);
                            if (curSpecType == SPEC_DEF)
                            {
                                setDefault(prog);
                            }
                            else if (curSpecType == SPEC_TILE_DEF)
                            {
                                for (int i = tileSpec.Length - 1; i >= 0; i--)
                                    if (tileSpec[i])
                                    {
                                        setTileDef(i, prog);
                                    }
                            }
                        }
                        progression.Clear();
                        intType = -1;
                        needInteger = false;
                        tileSpec = parseIdx(word, nTiles);
                        curSpecType = SPEC_TILE_DEF;
                        break;
                    default:
                        if (needInteger)
                        {
                            try
                            {
                                tmp = (System.Int32.Parse(word));
                            }
                            catch (System.FormatException)
                            {
                                throw new System.ArgumentException("Progression " + "order" + " specification " + "has missing " + "parameters: " + param);
                            }
                            switch (intType)
                            {
                                case 0:
                                    if (tmp < 0 || tmp > (dls.Max + 1))
                                        throw new System.ArgumentException("Invalid res_start " + "in '-Aptype'" + " option: " + tmp);
                                    curProg.rs = tmp; break;
                                case 1:
                                    if (tmp < 0 || tmp > nc)
                                    {
                                        throw new System.ArgumentException("Invalid comp_start " + "in '-Aptype' " + "option: " + tmp);
                                    }
                                    curProg.cs = tmp; break;
                                case 2:
                                    if (tmp < 0)
                                        throw new System.ArgumentException("Invalid layer_end " + "in '-Aptype'" + " option: " + tmp);
                                    if (tmp > nl)
                                    {
                                        tmp = nl;
                                    }
                                    curProg.lye = tmp; break;
                                case 3:
                                    if (tmp < 0)
                                        throw new System.ArgumentException("Invalid res_end " + "in '-Aptype'" + " option: " + tmp);
                                    if (tmp > (dls.Max + 1))
                                    {
                                        tmp = dls.Max + 1;
                                    }
                                    curProg.re = tmp; break;
                                case 4:
                                    if (tmp < 0)
                                        throw new System.ArgumentException("Invalid comp_end " + "in '-Aptype'" + " option: " + tmp);
                                    if (tmp > nc)
                                    {
                                        tmp = nc;
                                    }
                                    curProg.ce = tmp; break;
                            }
                            if (intType < 4)
                            {
                                intType++;
                                needInteger = true;
                                break;
                            }
                            else if (intType == 4)
                            {
                                intType = 0;
                                needInteger = false;
                                break;
                            }
                            else
                            {
                                throw new System.ApplicationException("Error in usage of 'Aptype' " + "option: " + param);
                            }
                        }
                        if (!needInteger)
                        {
                            mode = checkProgMode(word);
                            if (mode == -1)
                            {
                                errMsg2 = "Unknown progression type : '" + word + "'";
                                throw new System.ArgumentException(errMsg2);
                            }
                            needInteger = true;
                            intType = 0;
                            if (progression.Count == 0)
                            {
                                curProg = new Progression(mode, 0, nc, 0, dls.Max + 1, nl);
                            }
                            else
                            {
                                curProg = new Progression(mode, 0, nc, 0, dls.Max + 1, nl);
                            }
                            progression.Add(curProg);
                        }
                        break;
                }
            }
            if (progression.Count == 0)
            {
                if (pl.getParameter("Rroi") == null)
                {
                    mode = checkProgMode("res");
                }
                else
                {
                    mode = checkProgMode("layer");
                }
                if (mode == -1)
                {
                    errMsg2 = "Unknown progression type : '" + param + "'";
                    throw new System.ArgumentException(errMsg2);
                }
                prog = new Progression[1];
                prog[0] = new Progression(mode, 0, nc, 0, dls.Max + 1, nl);
                setDefault(prog);
                return;
            }
            curProg.ce = nc;
            curProg.lye = nl;
            curProg.re = dls.Max + 1;
            prog = new Progression[progression.Count];
            progression.CopyTo(prog);
            if (curSpecType == SPEC_DEF)
            {
                setDefault(prog);
            }
            else if (curSpecType == SPEC_TILE_DEF)
            {
                for (int i = tileSpec.Length - 1; i >= 0; i--)
                    if (tileSpec[i])
                    {
                        setTileDef(i, prog);
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
                    if (pl.getParameter("Rroi") == null)
                    {
                        mode = checkProgMode("res");
                    }
                    else
                    {
                        mode = checkProgMode("layer");
                    }
                    if (mode == -1)
                    {
                        errMsg2 = "Unknown progression type : '" + param + "'";
                        throw new System.ArgumentException(errMsg2);
                    }
                    prog = new Progression[1];
                    prog[0] = new Progression(mode, 0, nc, 0, dls.Max + 1, nl);
                    setDefault(prog);
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
        }
        private int checkProgMode(System.String mode)
        {
            if (mode.Equals("res"))
            {
                return Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_LY_COMP_POS_PROG;
            }
            else if (mode.Equals("layer"))
            {
                return Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.LY_RES_COMP_POS_PROG;
            }
            else if (mode.Equals("pos-comp"))
            {
                return Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.POS_COMP_RES_LY_PROG;
            }
            else if (mode.Equals("comp-pos"))
            {
                return Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.COMP_POS_RES_LY_PROG;
            }
            else if (mode.Equals("res-pos"))
            {
                return Syncfusion.Pdf.JPEG2000.codestream.ProgressionType.RES_POS_COMP_LY_PROG;
            }
            else
            {
                return -1;
            }
        }
    }
}