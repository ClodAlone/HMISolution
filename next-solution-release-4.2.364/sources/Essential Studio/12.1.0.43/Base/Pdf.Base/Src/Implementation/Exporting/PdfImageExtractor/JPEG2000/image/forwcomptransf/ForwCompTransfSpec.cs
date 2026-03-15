#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.wavelet.analysis;
using Syncfusion.Pdf.JPEG2000.util;
namespace Syncfusion.Pdf.JPEG2000.image.forwcomptransf
{
    internal class ForwCompTransfSpec : CompTransfSpec
    {
        internal ForwCompTransfSpec(int nt, int nc, byte type, AnWTFilterSpec wfs, JPXParameters pl)
            : base(nt, nc, type)
        {
            System.String param = pl.getParameter("Mct");
            if (param == null)
            {
                if (nc < 3)
                {
                    setDefault("none");
                    return;
                }
                else if (pl.getBooleanParameter("lossless"))
                {
                    setDefault("rct");
                    return;
                }
                else
                {
                    AnWTFilter[][] anfilt;
                    int[] filtType = new int[nComp];
                    for (int c = 0; c < 3; c++)
                    {
                        anfilt = (AnWTFilter[][])wfs.getCompDef(c);
                        filtType[c] = anfilt[0][0].FilterType;
                    }
                    bool reject = false;
                    for (int c = 1; c < 3; c++)
                    {
                        if (filtType[c] != filtType[0])
                            reject = true;
                    }
                    if (reject)
                    {
                        setDefault("none");
                    }
                    else
                    {
                        anfilt = (AnWTFilter[][])wfs.getCompDef(0);
                        if (anfilt[0][0].FilterType == Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W9X7)
                        {
                            setDefault("ict");
                        }
                        else
                        {
                            setDefault("rct");
                        }
                    }
                }
                for (int t = 0; t < nt; t++)
                {
                    AnWTFilter[][] anfilt;
                    int[] filtType = new int[nComp];
                    for (int c = 0; c < 3; c++)
                    {
                        anfilt = (AnWTFilter[][])wfs.getTileCompVal(t, c);
                        filtType[c] = anfilt[0][0].FilterType;
                    }
                    bool reject = false;
                    for (int c = 1; c < nComp; c++)
                    {
                        if (filtType[c] != filtType[0])
                            reject = true;
                    }
                    if (reject)
                    {
                        setTileDef(t, "none");
                    }
                    else
                    {
                        anfilt = (AnWTFilter[][])wfs.getTileCompVal(t, 0);
                        if (anfilt[0][0].FilterType == Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W9X7)
                        {
                            setTileDef(t, "ict");
                        }
                        else
                        {
                            setTileDef(t, "rct");
                        }
                    }
                }
                return;
            }
            SupportClass.Tokenizer stk = new SupportClass.Tokenizer(param);
            System.String word;
            byte curSpecType = SPEC_DEF;
            bool[] tileSpec = null;
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
                        throw new System.ArgumentException("Component specific " + " parameters" + " not allowed with " + "'-Mct' option");
                    default:
                        if (word.Equals("off"))
                        {
                            if (curSpecType == SPEC_DEF)
                            {
                                setDefault("none");
                            }
                            else if (curSpecType == SPEC_TILE_DEF)
                            {
                                for (int i = tileSpec.Length - 1; i >= 0; i--)
                                    if (tileSpec[i])
                                    {
                                        setTileDef(i, "none");
                                    }
                            }
                        }
                        else if (word.Equals("on"))
                        {
                            if (nc < 3)
                            {
                                throw new System.ArgumentException("Cannot use component" + " transformation on a " + "image with less than " + "three components");
                            }
                            if (curSpecType == SPEC_DEF)
                            {
                                setDefault("rct");
                            }
                            else if (curSpecType == SPEC_TILE_DEF)
                            {
                                for (int i = tileSpec.Length - 1; i >= 0; i--)
                                {
                                    if (tileSpec[i])
                                    {
                                        if (getFilterType(i, wfs) == Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W5X3)
                                        {
                                            setTileDef(i, "rct");
                                        }
                                        else
                                        {
                                            setTileDef(i, "ict");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new System.ArgumentException("Default parameter of " + "option Mct not" + " recognized: " + param);
                        }
                        curSpecType = SPEC_DEF;
                        tileSpec = null;
                        break;
                }
            }
            if (getDefault() == null)
            {
                setDefault("none");
                for (int t = 0; t < nt; t++)
                {
                    if (isTileSpecified(t))
                    {
                        continue;
                    }
                    AnWTFilter[][] anfilt;
                    int[] filtType = new int[nComp];
                    for (int c = 0; c < 3; c++)
                    {
                        anfilt = (AnWTFilter[][])wfs.getTileCompVal(t, c);
                        filtType[c] = anfilt[0][0].FilterType;
                    }
                    bool reject = false;
                    for (int c = 1; c < nComp; c++)
                    {
                        if (filtType[c] != filtType[0])
                            reject = true;
                    }
                    if (reject)
                    {
                        setTileDef(t, "none");
                    }
                    else
                    {
                        anfilt = (AnWTFilter[][])wfs.getTileCompVal(t, 0);
                        if (anfilt[0][0].FilterType == Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W9X7)
                        {
                            setTileDef(t, "ict");
                        }
                        else
                        {
                            setTileDef(t, "rct");
                        }
                    }
                }
            }
            for (int t = nt - 1; t >= 0; t--)
            {
                if (((System.String)getTileDef(t)).Equals("none"))
                {
                    continue;
                }
                else if (((System.String)getTileDef(t)).Equals("rct"))
                {
                    int filterType = getFilterType(t, wfs);
                    switch (filterType)
                    {
                        case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W5X3:
                            break;
                        case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W9X7:
                            if (isTileSpecified(t))
                            {
                                throw new System.ArgumentException("Cannot use RCT " + "with 9x7 filter " + "in tile " + t);
                            }
                            else
                            {
                                setTileDef(t, "ict");
                            }
                            break;
                        default:
                            throw new System.ArgumentException("Default filter is " + "not JPEG 2000 part" + " I compliant");
                    }
                }
                else
                {
                    int filterType = getFilterType(t, wfs);
                    switch (filterType)
                    {
                        case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W5X3: 
                            if (isTileSpecified(t))
                            {
                                throw new System.ArgumentException("Cannot use ICT " + "with filter 5x3 " + "in tile " + t);
                            }
                            else
                            {
                                setTileDef(t, "rct");
                            }
                            break;
                        case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W9X7: 
                            break;
                        default:
                            throw new System.ArgumentException("Default filter is " + "not JPEG 2000 part" + " I compliant");
                    }
                }
            }
        }
        private int getFilterType(int t, AnWTFilterSpec wfs)
        {
            AnWTFilter[][] anfilt;
            int[] filtType = new int[nComp];
            for (int c = 0; c < nComp; c++)
            {
                if (t == -1)
                {
                    anfilt = (AnWTFilter[][])wfs.getCompDef(c);
                }
                else
                {
                    anfilt = (AnWTFilter[][])wfs.getTileCompVal(t, c);
                }
                filtType[c] = anfilt[0][0].FilterType;
            }
            bool reject = false;
            for (int c = 1; c < nComp; c++)
            {
                if (filtType[c] != filtType[0])
                    reject = true;
            }
            if (reject)
            {
                throw new System.ArgumentException("Can not use component" + " transformation when " + "components do not use " + "the same filters");
            }
            return filtType[0];
        }
    }
}