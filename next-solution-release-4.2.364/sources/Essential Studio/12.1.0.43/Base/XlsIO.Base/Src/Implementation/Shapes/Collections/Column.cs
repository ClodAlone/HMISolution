#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation
{
    internal class Column
    {
        //BinaryInformation for OutlineLevel, Collapsed, IsBestFit
        private byte binrayInfo;
        public double defaultWidth;
        private int styleIndex;
        private short minCol;
        private WorksheetImpl worksheet;


        public int Index
        {
            get
            {
                return this.minCol;
            }
        }

        public bool IsHidden
        {
            get
            {
                return this.GetHiddenInfo();
            }
            set
            {
                this.SetHiddenInfo(value);
                this.SetBestFitInfo(false);
                if (!value && (this.defaultWidth == 0.0))
                {
                    this.defaultWidth = this.worksheet.Columnss.Width;
                }
            }
        }

        public double Width
        {
            get
            {
                return this.defaultWidth;
            }
            set
            {
                if (value < double.Epsilon)
                {
                    this.SetHiddenInfo(true);
                }
                else
                {
                    this.defaultWidth = value;
                    this.SetHiddenInfo(false);
                }
                this.SetBestFitInfo(false);
            }
        }

        internal Column(int minCol, WorksheetImpl worksheet, double defaultWidth)
        {
            this.styleIndex = -1;
            this.minCol = (short)minCol;
            this.worksheet = worksheet;
            this.defaultWidth = defaultWidth;
        }
      

        internal void SetMinColumnIndex(int minCol)
        {
            this.minCol = (short)minCol;
        }
        internal void SetCollapsedInfo(bool isCollapsed)
        {
            if (isCollapsed)
            {
                this.binrayInfo = (byte)(this.binrayInfo | 0x10);
            }
            else
            {
                this.binrayInfo = (byte)(this.binrayInfo & 0xef);
            }
        }
        internal void SetBestFitInfo(bool isBestFit)
        {
            if (isBestFit)
            {
                this.binrayInfo = (byte)(this.binrayInfo | 0x40);
            }
            else
            {
                this.binrayInfo = (byte)(this.binrayInfo & 0xbf);
            }
        }
        internal void SetWidth(int width)
        {
            int num = this.worksheet.GetAppImpl().GetFontCalc2();
            int num2 = this.worksheet.GetAppImpl().GetFontCalc1();
            int num3 = this.worksheet.GetAppImpl().GetFontCalc3();
            int num4 = width;
            if (num4 < (num + num3))
            {
                double num5 = (1.0 * num4) / ((double)(num + num3));
                this.defaultWidth = num5;
            }
            else
            {
                double num6 = ((double)((int)((((num4 - ((int)((((double)(num * num2)) / 256.0) + 0.5))) * 100.0) / ((double)num)) + 0.5))) / 100.0;
                this.defaultWidth = num6;
            }
            this.SetBestFitInfo(false);
        }
        internal void SetOutLineLevel(byte outLineLevel)
        {
            this.binrayInfo = (byte)(this.binrayInfo & 240);
            this.binrayInfo = (byte)(this.binrayInfo | outLineLevel);
        }
        internal void SetStyleIndex(int styleIndex)
        {
            this.styleIndex = styleIndex;
        }
        internal bool GetHiddenInfo()
        {
            return ((this.binrayInfo & 0x20) != 0);
        }
        internal void SetHiddenInfo(bool isHidden)
        {
            if (!isHidden)
            {
                this.binrayInfo = (byte)(this.binrayInfo & 0xdf);
            }
            else
            {
                this.binrayInfo = (byte)(this.binrayInfo | 0x20);
            }
        }

        
    }
}
