#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000;
namespace Syncfusion.Pdf.JPEG2000.quantization
{
    internal class GuardBitsSpec : ModuleSpec
    {
        public GuardBitsSpec(int nt, int nc, byte type)
            : base(nt, nc, type)
        {
        }
        internal GuardBitsSpec(int nt, int nc, byte type, JPXParameters pl)
            : base(nt, nc, type)
        {
            System.String param = pl.getParameter("Qguard_bits");
            if (param == null)
            {
                throw new System.ArgumentException("Qguard_bits option not " + "specified");
            }
            SupportClass.Tokenizer stk = new SupportClass.Tokenizer(param);
            System.String word;
            byte curSpecType = SPEC_DEF;
            bool[] tileSpec = null;
            bool[] compSpec = null;
            System.Int32 value_Renamed;
            while (stk.HasMoreTokens())
            {
                word = stk.NextToken().ToLower();
                switch (word[0])
                {
                    case 't':
                        tileSpec = parseIdx(word, nTiles);
                        if (curSpecType == SPEC_COMP_DEF)
                            curSpecType = SPEC_TILE_COMP;
                        else
                            curSpecType = SPEC_TILE_DEF;
                        break;
                    case 'c':
                        compSpec = parseIdx(word, nComp);
                        if (curSpecType == SPEC_TILE_DEF)
                            curSpecType = SPEC_TILE_COMP;
                        else
                            curSpecType = SPEC_COMP_DEF;
                        break;
                    default:
                        try
                        {
                            value_Renamed = System.Int32.Parse(word);
                        }
                        catch (System.FormatException)
                        {
                            throw new System.ArgumentException("Bad parameter for " + "-Qguard_bits option" + " : " + word);
                        }
                        if ((float)value_Renamed <= 0.0f)
                        {
                            throw new System.ArgumentException("Guard bits value " + "must be positive : " + value_Renamed);
                        }
                        if (curSpecType == SPEC_DEF)
                        {
                            setDefault((System.Object)value_Renamed);
                        }
                        else if (curSpecType == SPEC_TILE_DEF)
                        {
                            for (int i = tileSpec.Length - 1; i >= 0; i--)
                                if (tileSpec[i])
                                {
                                    setTileDef(i, (System.Object)value_Renamed);
                                }
                        }
                        else if (curSpecType == SPEC_COMP_DEF)
                        {
                            for (int i = compSpec.Length - 1; i >= 0; i--)
                                if (compSpec[i])
                                {
                                    setCompDef(i, (System.Object)value_Renamed);
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
                                        setTileCompVal(i, j, (System.Object)value_Renamed);
                                    }
                                }
                            }
                        }
                        curSpecType = SPEC_DEF;
                        tileSpec = null;
                        compSpec = null;
                        break;
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
                    setDefault((System.Object)System.Int32.Parse(pl.DefaultParameterList.getParameter("Qguard_bits")));
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
    }
}