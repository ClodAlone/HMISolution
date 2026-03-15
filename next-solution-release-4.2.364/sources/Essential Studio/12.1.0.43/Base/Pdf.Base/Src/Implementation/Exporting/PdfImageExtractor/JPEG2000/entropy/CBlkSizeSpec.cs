#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.codestream;
using Syncfusion.Pdf.JPEG2000.wavelet;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000;
namespace Syncfusion.Pdf.JPEG2000.entropy
{
    internal class CBlkSizeSpec : ModuleSpec
    {
        virtual public int MaxCBlkWidth
        {
            get
            {
                return maxCBlkWidth;
            }
        }
        virtual public int MaxCBlkHeight
        {
            get
            {
                return maxCBlkHeight;
            }
        }
        private const System.String optName = "Cblksiz";
        private int maxCBlkWidth = 0;
        private int maxCBlkHeight = 0;
        public CBlkSizeSpec(int nt, int nc, byte type)
            : base(nt, nc, type)
        {
        }
        internal CBlkSizeSpec(int nt, int nc, byte type, JPXParameters pl)
            : base(nt, nc, type)
        {
            bool firstVal = true;
            System.String param = pl.getParameter(optName);
            SupportClass.Tokenizer stk = new SupportClass.Tokenizer(param);
            byte curSpecType = SPEC_DEF;
            bool[] tileSpec = null;
            bool[] compSpec = null;
            int ci, ti;
            System.String word = null;
            System.String errMsg = null;
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
                        System.Int32[] dim = new System.Int32[2];
                        try
                        {
                            dim[0] = System.Int32.Parse(word);
                            if (dim[0] > Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_DIM)
                            {
                                errMsg = "'" + optName + "' option : the code-block's " + "width cannot be greater than " + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_DIM;
                                throw new System.ArgumentException(errMsg);
                            }
                            if (dim[0] < Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MIN_CB_DIM)
                            {
                                errMsg = "'" + optName + "' option : the code-block's " + "width cannot be less than " + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MIN_CB_DIM;
                                throw new System.ArgumentException(errMsg);
                            }
                            if (dim[0] != (1 << MathUtil.log2(dim[0])))
                            {
                                errMsg = "'" + optName + "' option : the code-block's " + "width must be a power of 2";
                                throw new System.ArgumentException(errMsg);
                            }
                        }
                        catch (System.FormatException)
                        {
                            errMsg = "'" + optName + "' option : the code-block's " + "width could not be parsed.";
                            throw new System.ArgumentException(errMsg);
                        }
                        try
                        {
                            word = stk.NextToken();
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            errMsg = "'" + optName + "' option : could not parse the " + "code-block's height";
                            throw new System.ArgumentException(errMsg);
                        }
                        try
                        {
                            dim[1] = System.Int32.Parse(word);
                            if (dim[1] > Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_DIM)
                            {
                                errMsg = "'" + optName + "' option : the code-block's " + "height cannot be greater than " + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_DIM;
                                throw new System.ArgumentException(errMsg);
                            }
                            if (dim[1] < Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MIN_CB_DIM)
                            {
                                errMsg = "'" + optName + "' option : the code-block's " + "height cannot be less than " + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MIN_CB_DIM;
                                throw new System.ArgumentException(errMsg);
                            }
                            if (dim[1] != (1 << MathUtil.log2(dim[1])))
                            {
                                errMsg = "'" + optName + "' option : the code-block's " + "height must be a power of 2";
                                throw new System.ArgumentException(errMsg);
                            }
                            if (dim[0] * dim[1] > Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_AREA)
                            {
                                errMsg = "'" + optName + "' option : The " + "code-block's area (i.e. width*height) " + "cannot be greater than " + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_AREA;
                                throw new System.ArgumentException(errMsg);
                            }
                        }
                        catch (System.FormatException)
                        {
                            errMsg = "'" + optName + "' option : the code-block's height " + "could not be parsed.";
                            throw new System.ArgumentException(errMsg);
                        }
                        if (dim[0] > maxCBlkWidth)
                        {
                            maxCBlkWidth = dim[0];
                        }
                        if (dim[1] > maxCBlkHeight)
                        {
                            maxCBlkHeight = dim[1];
                        }
                        if (firstVal)
                        {
                            setDefault((System.Object)(dim));
                            firstVal = false;
                        }
                        switch (curSpecType)
                        {
                            case SPEC_DEF:
                                setDefault((System.Object)(dim));
                                break;
                            case SPEC_TILE_DEF:
                                for (ti = tileSpec.Length - 1; ti >= 0; ti--)
                                {
                                    if (tileSpec[ti])
                                    {
                                        setTileDef(ti, (System.Object)(dim));
                                    }
                                }
                                break;
                            case SPEC_COMP_DEF:
                                for (ci = compSpec.Length - 1; ci >= 0; ci--)
                                {
                                    if (compSpec[ci])
                                    {
                                        setCompDef(ci, (System.Object)(dim));
                                    }
                                }
                                break;
                            default:
                                for (ti = tileSpec.Length - 1; ti >= 0; ti--)
                                {
                                    for (ci = compSpec.Length - 1; ci >= 0; ci--)
                                    {
                                        if (tileSpec[ti] && compSpec[ci])
                                        {
                                            setTileCompVal(ti, ci, (System.Object)(dim));
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }
        public virtual int getCBlkWidth(byte type, int t, int c)
        {
            System.Int32[] dim = null;
            switch (type)
            {
                case SPEC_DEF:
                    dim = (System.Int32[])getDefault();
                    break;
                case SPEC_COMP_DEF:
                    dim = (System.Int32[])getCompDef(c);
                    break;
                case SPEC_TILE_DEF:
                    dim = (System.Int32[])getTileDef(t);
                    break;
                case SPEC_TILE_COMP:
                    dim = (System.Int32[])getTileCompVal(t, c);
                    break;
            }
            return dim[0];
        }
        public virtual int getCBlkHeight(byte type, int t, int c)
        {
            System.Int32[] dim = null;
            switch (type)
            {
                case SPEC_DEF:
                    dim = (System.Int32[])getDefault();
                    break;
                case SPEC_COMP_DEF:
                    dim = (System.Int32[])getCompDef(c);
                    break;
                case SPEC_TILE_DEF:
                    dim = (System.Int32[])getTileDef(t);
                    break;
                case SPEC_TILE_COMP:
                    dim = (System.Int32[])getTileCompVal(t, c);
                    break;
            }
            return dim[1];
        }
        public override void setDefault(System.Object value_Renamed)
        {
            base.setDefault(value_Renamed);
            storeHighestDims((System.Int32[])value_Renamed);
        }
        public override void setTileDef(int t, System.Object value_Renamed)
        {
            base.setTileDef(t, value_Renamed);
            storeHighestDims((System.Int32[])value_Renamed);
        }
        public override void setCompDef(int c, System.Object value_Renamed)
        {
            base.setCompDef(c, value_Renamed);
            storeHighestDims((System.Int32[])value_Renamed);
        }
        public override void setTileCompVal(int t, int c, System.Object value_Renamed)
        {
            base.setTileCompVal(t, c, value_Renamed);
            storeHighestDims((System.Int32[])value_Renamed);
        }
        private void storeHighestDims(System.Int32[] dim)
        {
            if (dim[0] > maxCBlkWidth)
            {
                maxCBlkWidth = dim[0];
            }
            if (dim[1] > maxCBlkHeight)
            {
                maxCBlkHeight = dim[1];
            }
        }
    }
}