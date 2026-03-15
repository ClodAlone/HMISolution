#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.codestream;
using Syncfusion.Pdf.JPEG2000.fileformat;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.io;
using System.Collections.Generic;
using HashTable = System.Collections.Generic.Dictionary<object, object>;
using ArrayList = System.Collections.Generic.List<object>;
namespace Syncfusion.Pdf.JPEG2000.fileformat.reader
{
    internal class JPXFormatReader
    {
        virtual public long[] CodeStreamPos
        {
            get
            {
                int size = codeStreamPos.Count;
                long[] pos = new long[size];
                for (int i = 0; i < size; i++)
                    pos[i] = (long)((System.Int32)(codeStreamPos[i]));
                return pos;
            }
        }
        virtual public int FirstCodeStreamPos
        {
            get
            {
                return ((System.Int32)(codeStreamPos[0]));
            }
        }
        virtual public int FirstCodeStreamLength
        {
            get
            {
                return ((System.Int32)(codeStreamLength[0]));
            }
        }
        private JPXRandomAccessStream in_Renamed;
        private ArrayList codeStreamPos;
        private ArrayList codeStreamLength;
        public bool JP2FFUsed;
        internal JPXFormatReader(JPXRandomAccessStream in_Renamed)
        {
            this.in_Renamed = in_Renamed;
        }
        public virtual void readFileFormat()
        {
            int box;
            int length;
            long longLength = 0;
            int pos;
            short marker;
            bool jp2HeaderBoxFound = false;
            bool lastBoxFound = false;
            try
            {
                if (in_Renamed.readInt() != 0x0000000c || in_Renamed.readInt() != Syncfusion.Pdf.JPEG2000.fileformat.FileFormatBoxes.JP2_SIGNATURE_BOX || in_Renamed.readInt() != 0x0d0a870a)
                {
                    in_Renamed.seek(0);
                    marker = (short)in_Renamed.readShort();
                    if (marker != Syncfusion.Pdf.JPEG2000.codestream.Markers.SOC)
                        //throw new System.ApplicationException("File is neither valid JP2 file nor " + "valid JPEG 2000 codestream");
                    JP2FFUsed = false;
                    in_Renamed.seek(0);
                    return;
                }
                JP2FFUsed = true;
                if (!readFileTypeBox())
                {
                    //throw new System.ApplicationException("Invalid JP2 file: File Type box missing");
                }
                while (!lastBoxFound)
                {
                    pos = in_Renamed.Pos;
                    length = in_Renamed.readInt();
                    if ((pos + length) == in_Renamed.length())
                        lastBoxFound = true;
                    box = in_Renamed.readInt();
                    if (length == 0)
                    {
                        lastBoxFound = true;
                        length = in_Renamed.length() - in_Renamed.Pos;
                    }
                    else if (length == 1)
                    {
                        longLength = in_Renamed.readLong();
                        throw new System.IO.IOException("File too long.");
                    }
                    else
                        longLength = (long)0;
                    switch (box)
                    {
                        case Syncfusion.Pdf.JPEG2000.fileformat.FileFormatBoxes.CONTIGUOUS_CODESTREAM_BOX:
                            if (!jp2HeaderBoxFound)
                            {
                                //throw new System.ApplicationException("Invalid JP2 file: JP2Header box not " + "found before Contiguous codestream " + "box ");
                            }
                            readContiguousCodeStreamBox(pos, length, longLength);
                            break;
                        case Syncfusion.Pdf.JPEG2000.fileformat.FileFormatBoxes.JP2_HEADER_BOX:
                            if (jp2HeaderBoxFound)
                                //throw new System.ApplicationException("Invalid JP2 file: Multiple " + "JP2Header boxes found");
                            readJP2HeaderBox(pos, length, longLength);
                            jp2HeaderBoxFound = true;
                            break;
                    }
                    if (!lastBoxFound)
                        in_Renamed.seek(pos + length);
                }
            }
            catch (System.IO.EndOfStreamException)
            {
                //throw new System.ApplicationException("EOF reached before finding Contiguous " + "Codestream Box");
            }
            if (codeStreamPos.Count == 0)
            {
                //throw new System.ApplicationException("Invalid JP2 file: Contiguous codestream box " + "missing");
            }
            return;
        }
        public virtual bool readFileTypeBox()
        {
            int length;
            long longLength = 0;
            int pos;
            int nComp;
            bool foundComp = false;
            pos = in_Renamed.Pos;
            length = in_Renamed.readInt();
            if (length == 0)
            {
                //throw new System.ApplicationException("Zero-length of Profile Box");
            }
            if (in_Renamed.readInt() != Syncfusion.Pdf.JPEG2000.fileformat.FileFormatBoxes.FILE_TYPE_BOX)
            {
                return false;
            }
            if (length == 1)
            {
                longLength = in_Renamed.readLong();
                throw new System.IO.IOException("File too long.");
            }
            in_Renamed.readInt();
            in_Renamed.readInt();
            nComp = (length - 16) / 4;
            for (int i = nComp; i > 0; i--)
            {
                if (in_Renamed.readInt() == Syncfusion.Pdf.JPEG2000.fileformat.FileFormatBoxes.FT_BR)
                    foundComp = true;
            }
            if (!foundComp)
            {
                return false;
            }
            return true;
        }
        public virtual bool readJP2HeaderBox(long pos, int length, long longLength)
        {
            if (length == 0)
            {
                //throw new System.ApplicationException("Zero-length of JP2Header Box");
            }
            return true;
        }
        public virtual bool readContiguousCodeStreamBox(long pos, int length, long longLength)
        {
            int ccpos = in_Renamed.Pos;
            if (codeStreamPos == null)
                codeStreamPos = (new ArrayList(10));
            codeStreamPos.Add((System.Int32)ccpos);
            if (codeStreamLength == null)
                codeStreamLength = (new ArrayList(10));
            codeStreamLength.Add((System.Int32)length);
            return true;
        }
    }
}