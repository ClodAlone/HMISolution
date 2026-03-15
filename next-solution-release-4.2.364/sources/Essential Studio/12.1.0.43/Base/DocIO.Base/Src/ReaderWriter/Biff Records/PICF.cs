#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;

using Syncfusion.DocIO.ReaderWriter.Biff_Records;

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for PICF.
    /// </summary>
    internal class PICF
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal const int DEF_PICF_LENGTH = 68;
        internal const int DEF_SCALING_FACTOR = 1000;
        #endregion

        #region Class members
        internal int lcb;
        internal short cbHeader;
        internal short mm;
        internal short xExt;
        internal short yExt;
        internal short hMf;
        internal short bm_rcWinMF;
        internal short bm_rcWinMF1;
        internal short bm_rcWinMF2;
        internal short bm_rcWinMF3;
        internal short bm_rcWinMF4;
        internal short bm_rcWinMF5;
        internal short bm_rcWinMF6;
        internal short dxaGoal;
        internal short dyaGoal;
        internal ushort mx;
        internal ushort my;
        internal short dyaCropLeft;
        internal short dyaCropTop;
        internal short dxaCropRight;
        internal short dyaCropBottom;
        internal short brcl;
        internal bool fFrameEmpty;
        internal bool fBitmap;
        internal bool fDrawHatch;
        internal bool fError;
        internal short bpp;
        internal BorderCode brcTop = new BorderCode();
        internal BorderCode brcLeft = new BorderCode();
        internal BorderCode brcBottom = new BorderCode();
        internal BorderCode brcRight = new BorderCode();
        internal short dxaOrigin;
        internal short dyaOrigin;
        internal short cProps;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int ScaleHeight
        {
            get
            {
                return (int)(dyaGoal * ScaleY);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int ScaleWidth
        {
            get
            {
                return (int)(dxaGoal * ScaleX);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal double ScaleX
        {
            get
            {
                return (((double)mx) / DEF_SCALING_FACTOR);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal double ScaleY
        {
            get
            {
                return (((double)my) / DEF_SCALING_FACTOR);
            }
        }
        /// <summary>
        /// Gets the border top.
        /// </summary>
        /// <value>The border top.</value>
        internal BorderCode BorderTop
        {
            get
            {
                return brcTop;
            }
        }
        /// <summary>
        /// Gets the border left.
        /// </summary>
        /// <value>The border left.</value>
        internal BorderCode BorderLeft
        {
            get
            {
                return brcLeft;
            }
        }
        /// <summary>
        /// Gets the border right.
        /// </summary>
        /// <value>The border right.</value>
        internal BorderCode BorderRight
        {
            get
            {
                return brcRight;
            }
        }
        /// <summary>
        /// Gets the border bottom.
        /// </summary>
        /// <value>The border bottom.</value>
        internal BorderCode BorderBottom
        {
            get
            {
                return brcBottom;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal PICF()
        {
            cbHeader = DEF_PICF_LENGTH;
            mx = DEF_SCALING_FACTOR;
            my = DEF_SCALING_FACTOR;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal PICF(BinaryReader reader)
        {
            Read(reader);
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal void Read(BinaryReader reader)
        {
            int pos = (int)reader.BaseStream.Position;
            lcb = reader.ReadInt32();
            cbHeader = reader.ReadInt16();
            mm = reader.ReadInt16();
            xExt = reader.ReadInt16();
            yExt = reader.ReadInt16();
            hMf = reader.ReadInt16();
            bm_rcWinMF = reader.ReadInt16();
            bm_rcWinMF1 = reader.ReadInt16();
            bm_rcWinMF2 = reader.ReadInt16();
            bm_rcWinMF3 = reader.ReadInt16();
            bm_rcWinMF4 = reader.ReadInt16();
            bm_rcWinMF5 = reader.ReadInt16();
            bm_rcWinMF6 = reader.ReadInt16();
            dxaGoal = reader.ReadInt16();
            dyaGoal = reader.ReadInt16();
            mx = (mx = reader.ReadUInt16()) == 0 ? (ushort)1000 : mx;
            my = (my = reader.ReadUInt16()) == 0 ? (ushort)1000 : my;
            dyaCropLeft = reader.ReadInt16();
            dyaCropTop = reader.ReadInt16();
            dxaCropRight = reader.ReadInt16();
            dyaCropBottom = reader.ReadInt16();
            int tmp = reader.ReadInt16();
            brcl = (short)(tmp & 15);
            fFrameEmpty = (tmp & 0x10) != 0;
            fBitmap = (tmp & 0x20) != 0;
            fDrawHatch = (tmp & 0x40) != 0;
            fError = (tmp & 0x80) != 0;
            bpp = (short)((tmp & 0xff00) >> 8);

            brcTop.Read(reader);
            brcLeft.Read(reader);
            brcBottom.Read(reader);
            brcRight.Read(reader);

            dxaOrigin = reader.ReadInt16();
            dyaOrigin = reader.ReadInt16();
            cProps = reader.ReadInt16();

            reader.BaseStream.Position = pos + cbHeader;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            Read(reader);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(lcb);
            writer.Write((short)cbHeader);
            writer.Write((short)mm);
            writer.Write((short)xExt);
            writer.Write((short)yExt);
            writer.Write((short)hMf);
            writer.Write((short)bm_rcWinMF);
            writer.Write((short)bm_rcWinMF1);
            writer.Write((short)bm_rcWinMF2);
            writer.Write((short)bm_rcWinMF3);
            writer.Write((short)bm_rcWinMF4);
            writer.Write((short)bm_rcWinMF5);
            writer.Write((short)bm_rcWinMF6);
            writer.Write((short)dxaGoal);
            writer.Write((short)dyaGoal);
            writer.Write((short)mx);
            writer.Write((short)my);
            writer.Write((short)dyaCropLeft);
            writer.Write((short)dyaCropTop);
            writer.Write((short)dxaCropRight);
            writer.Write((short)dyaCropBottom);
            int tmp = brcl;
            tmp |= (fFrameEmpty ? 0x10 : 0);
            tmp |= (fBitmap ? 0x20 : 0);
            tmp |= (fDrawHatch ? 0x40 : 0);
            tmp |= (fError ? 0x80 : 0);
            tmp |= (bpp << 8);
            writer.Write((short)tmp);

            brcTop.Write(stream);
            brcLeft.Write(stream);
            brcBottom.Write(stream);
            brcRight.Write(stream);

            writer.Write((short)dxaOrigin);
            writer.Write((short)dyaOrigin);
            writer.Write((short)cProps);
        }

        /// <summary>
        /// Clones current PictureDescriptor.
        /// </summary>
        /// <returns></returns>
        internal PICF Clone()
        {
            PICF pictDesc = base.MemberwiseClone() as PICF;

            pictDesc.brcBottom = this.brcBottom.Clone();
            pictDesc.brcLeft = this.brcLeft.Clone();
            pictDesc.brcRight = this.brcRight.Clone();
            pictDesc.brcTop = this.brcTop.Clone();

            return pictDesc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="heightScale"></param>
        /// <param name="widthScale"></param>
        internal void SetBasePictureOptions(int height, int width, float heightScale, float widthScale)
        {
            if (width > short.MaxValue)
            {
                dxaGoal = short.MaxValue;
            }
            else
            {
                dxaGoal = (short)width;
            }
            if (height > short.MaxValue)
            {
                dyaGoal = short.MaxValue;
            }
            else
            {
                dyaGoal = (short)height;
            }

            mx = (ushort)Math.Round(widthScale * DLSConstants.ImageScalingFactor);
            my = (ushort)Math.Round(heightScale * DLSConstants.ImageScalingFactor);
        }
        #endregion
    }
}
