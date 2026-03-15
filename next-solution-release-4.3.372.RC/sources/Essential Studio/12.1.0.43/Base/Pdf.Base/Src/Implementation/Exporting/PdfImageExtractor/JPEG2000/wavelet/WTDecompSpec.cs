#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000;
namespace Syncfusion.Pdf.JPEG2000.wavelet
{
    internal class WTDecompSpec
    {
        virtual public int MainDefDecompType
        {
            get
            {
                return mainDefDecompType;
            }
        }
        virtual public int MainDefLevels
        {
            get
            {
                return mainDefLevels;
            }
        }
        public const int WT_DECOMP_DYADIC = 0;
        public const int WT_DECOMP_SPACL = 2;
        public const int WT_DECOMP_PACKET = 1;
        public const byte DEC_SPEC_MAIN_DEF = 0;
        public const byte DEC_SPEC_COMP_DEF = 1;
        public const byte DEC_SPEC_TILE_DEF = 2;
        public const byte DEC_SPEC_TILE_COMP = 3;
        private byte[] specValType;
        private int mainDefDecompType;
        private int mainDefLevels;
        private int[] compMainDefDecompType;
        private int[] compMainDefLevels;
        public WTDecompSpec(int nc, int dec, int lev)
        {
            mainDefDecompType = dec;
            mainDefLevels = lev;
            specValType = new byte[nc];
        }
        public virtual void setMainCompDefDecompType(int n, int dec, int lev)
        {
            if (dec < 0 && lev < 0)
            {
                throw new System.ArgumentException();
            }
            specValType[n] = DEC_SPEC_COMP_DEF;
            if (compMainDefDecompType == null)
            {
                compMainDefDecompType = new int[specValType.Length];
                compMainDefLevels = new int[specValType.Length];
            }
            compMainDefDecompType[n] = (dec >= 0) ? dec : mainDefDecompType;
            compMainDefLevels[n] = (lev >= 0) ? lev : mainDefLevels;
            throw new ArgumentException("Components and tiles are having difffrent decomposition type and levels");
        }
        public virtual byte getDecSpecType(int n)
        {
            return specValType[n];
        }
        public virtual int getDecompType(int n)
        {
            switch (specValType[n])
            {
                case DEC_SPEC_MAIN_DEF:
                    return mainDefDecompType;
                case DEC_SPEC_COMP_DEF:
                    return compMainDefDecompType[n];
                case DEC_SPEC_TILE_DEF:
                    throw new ArgumentException("The Tile elemet is not supported in JPX");
                case DEC_SPEC_TILE_COMP:
                    throw new ArgumentException("The Componet elemet is not supported in JPX");
                default:
                    throw new System.ArgumentException();
            }
        }
        public virtual int getLevels(int n)
        {
            switch (specValType[n])
            {
                case DEC_SPEC_MAIN_DEF:
                    return mainDefLevels;
                case DEC_SPEC_COMP_DEF:
                    return compMainDefLevels[n];
                case DEC_SPEC_TILE_DEF:
                    throw new ArgumentException();
                case DEC_SPEC_TILE_COMP:
                    throw new ArgumentException();
                default:
                    throw new System.ArgumentException();
            }
        }
    }
}