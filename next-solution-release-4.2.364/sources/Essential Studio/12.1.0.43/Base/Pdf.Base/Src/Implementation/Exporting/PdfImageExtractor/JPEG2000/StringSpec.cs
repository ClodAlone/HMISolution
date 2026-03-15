#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.util;
namespace Syncfusion.Pdf.JPEG2000
{
    internal class StringSpec : ModuleSpec
    {
        internal StringSpec(int nt, int nc, byte type)
            : base(nt, nc, type)
        {
        }
        internal StringSpec(int nt, int nc, byte type, System.String optName, System.String[] list, JPXParameters pl)
            : base(nt, nc, type)
        {
            System.String param = pl.getParameter(optName);
            bool recognized = false;
            if (param == null)
            {
                param = pl.DefaultParameterList.getParameter(optName);
                for (int i = list.Length - 1; i >= 0; i--)
                    if (param.ToUpper().Equals(list[i].ToUpper()))
                        recognized = true;
                if (!recognized)
                    throw new System.ArgumentException("The Option name is not supported");
                setDefault(param);
                return;
            }
            SupportClass.Tokenizer stk = new SupportClass.Tokenizer(param);
            System.String word;
            byte curSpecType = SPEC_DEF;
            bool[] tileSpec = null;
            bool[] compSpec = null;
            while (stk.HasMoreTokens())
            {
                word = stk.NextToken();
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
                            curSpecType = SPEC_COMP_DEF;
                        break;
                    default:
                        recognized = false;
                        for (int i = list.Length - 1; i >= 0; i--)
                            if (word.ToUpper().Equals(list[i].ToUpper()))
                                recognized = true;
                        if (!recognized)
                            throw new System.ArgumentException("The Option name is not supported");
                        if (curSpecType == SPEC_DEF)
                        {
                            setDefault(word);
                        }
                        else if (curSpecType == SPEC_TILE_DEF)
                        {
                            for (int i = tileSpec.Length - 1; i >= 0; i--)
                                if (tileSpec[i])
                                {
                                    setTileDef(i, word);
                                }
                        }
                        else if (curSpecType == SPEC_COMP_DEF)
                        {
                            for (int i = compSpec.Length - 1; i >= 0; i--)
                                if (compSpec[i])
                                {
                                    setCompDef(i, word);
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
                                        setTileCompVal(i, j, word);
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
                    param = pl.DefaultParameterList.getParameter(optName);
                    for (int i = list.Length - 1; i >= 0; i--)
                        if (param.ToUpper().Equals(list[i].ToUpper()))
                            recognized = true;
                    if (!recognized)
                        throw new System.ArgumentException("Default parameter of " + "option -" + optName + " not" + " recognized: " + param);
                    setDefault(param);
                }
                else
                {
                    setDefault(getSpec(0, 0));
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