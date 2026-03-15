#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using Syncfusion.Pdf.Compression.JBIG2.Internal;

namespace Syncfusion.Pdf.Compression.JBIG2
{
    partial class Tiff
    {
        private static Tiff Open(string fileName, string mode, TiffExtendProc extender)
        {
            const string module = "Open";

            FileMode fileMode;
            FileAccess fileAccess;
            getMode(mode, module, out fileMode, out fileAccess);

            FileStream stream = null;
            try
            {
                if (fileAccess == FileAccess.Read)
                    stream = File.Open(fileName, fileMode, fileAccess, FileShare.Read);
                else
                    stream = File.Open(fileName, fileMode, fileAccess);
            }
            catch (Exception e)
            {
                return null;
            }

            Tiff tif = ClientOpen(fileName, mode, stream, new TiffStream(), extender);
            if (tif == null)
                stream.Dispose();
            else
                tif.m_fileStream = stream;

            return tif;
        }

        private static Tiff ClientOpen(string name, string mode, object clientData, TiffStream stream, TiffExtendProc extender)
        {
            const string module = "ClientOpen";

            if (mode == null || mode.Length == 0)
            {
                return null;
            }

            FileMode fileMode;
            FileAccess fileAccess;
            int m = getMode(mode, module, out fileMode, out fileAccess);

            Tiff tif = new Tiff();

            tif.m_name = name;

            tif.m_mode = m & ~(O_CREAT | O_TRUNC);
            tif.m_curdir = -1; // non-existent directory
            tif.m_curoff = 0;
            tif.m_curstrip = -1; // invalid strip
            tif.m_row = -1; // read/write pre-increment
            tif.m_clientdata = clientData;

            if (stream == null)
            {
                return null;
            }

            tif.m_stream = stream;

            // setup default state
            tif.m_currentCodec = tif.m_builtInCodecs[0];

            tif.m_flags = TiffFlags.MSB2LSB;

            if (m == O_RDONLY || m == O_RDWR)
                tif.m_flags |= STRIPCHOP_DEFAULT;

            int modelength = mode.Length;
            for (int i = 0; i < modelength; i++)
            {
                switch (mode[i])
                {
                    case 'b':
                        if ((m & O_CREAT) != 0)
                            tif.m_flags |= TiffFlags.SWAB;
                        break;
                    case 'l':
                        break;
                    case 'B':
                        tif.m_flags = (tif.m_flags & ~TiffFlags.FILLORDER) | TiffFlags.MSB2LSB;
                        break;
                    case 'L':
                        tif.m_flags = (tif.m_flags & ~TiffFlags.FILLORDER) | TiffFlags.LSB2MSB;
                        break;
                    case 'H':
                        tif.m_flags = (tif.m_flags & ~TiffFlags.FILLORDER) | TiffFlags.LSB2MSB;
                        break;
                    case 'C':
                        if (m == O_RDONLY)
                            tif.m_flags |= TiffFlags.STRIPCHOP;
                        break;
                    case 'c':
                        if (m == O_RDONLY)
                            tif.m_flags &= ~TiffFlags.STRIPCHOP;
                        break;
                    case 'h':
                        tif.m_flags |= TiffFlags.HEADERONLY;
                        break;
                }
            }

            if ((tif.m_mode & O_TRUNC) != 0 || !tif.readHeaderOk(ref tif.m_header))
            {
                if (tif.m_mode == O_RDONLY)
                {
                    return null;
                }

                if ((tif.m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
                    tif.m_header.tiff_magic = TIFF_BIGENDIAN;
                else
                    tif.m_header.tiff_magic = TIFF_LITTLEENDIAN;

                tif.m_header.tiff_version = TIFF_VERSION;
                if ((tif.m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
                    SwabShort(ref tif.m_header.tiff_version);

                tif.m_header.tiff_diroff = 0; // filled in later

                tif.seekFile(0, SeekOrigin.Begin);

                //if (!tif.writeHeaderOK(tif.m_header))
                //{
                //    ErrorExt(tif, tif.m_clientdata, name, "Error writing TIFF header");
                //    tif.m_mode = O_RDONLY;
                //    return null;
                //}

                // Setup the byte order handling.
                tif.initOrder(tif.m_header.tiff_magic);

                // Setup default directory.
                tif.setupDefaultDirectory();
                tif.m_diroff = 0;
                tif.m_dirlist = null;
                tif.m_dirlistsize = 0;
                tif.m_dirnumber = 0;
                return tif;
            }

            // Setup the byte order handling.
            if (tif.m_header.tiff_magic != TIFF_BIGENDIAN &&
                tif.m_header.tiff_magic != TIFF_LITTLEENDIAN &&
                tif.m_header.tiff_magic != MDI_LITTLEENDIAN)
            {
                tif.m_mode = O_RDONLY;
                return null;
            }

            tif.initOrder(tif.m_header.tiff_magic);

            // Swap header if required.
            if ((tif.m_flags & TiffFlags.SWAB) == TiffFlags.SWAB)
            {
                SwabShort(ref tif.m_header.tiff_version);
                SwabUInt(ref tif.m_header.tiff_diroff);
            }

            if (tif.m_header.tiff_version == TIFF_BIGTIFF_VERSION)
            {
                tif.m_mode = O_RDONLY;
                return null;
            }

            if (tif.m_header.tiff_version != TIFF_VERSION)
            {
                tif.m_mode = O_RDONLY;
                return null;
            }

            tif.m_flags |= TiffFlags.MYBUFFER;
            tif.m_rawcp = 0;
            tif.m_rawdata = null;
            tif.m_rawdatasize = 0;

            if ((tif.m_flags & TiffFlags.HEADERONLY) == TiffFlags.HEADERONLY)
                return tif;

            // Setup initial directory.
            switch (mode[0])
            {
                case 'r':
                    tif.m_nextdiroff = tif.m_header.tiff_diroff;

                    if (tif.ReadDirectory())
                    {
                        tif.m_rawcc = -1;
                        tif.m_flags |= TiffFlags.BUFFERSETUP;
                        return tif;
                    }
                    break;
                case 'a':
                    tif.setupDefaultDirectory();
                    return tif;
            }

            tif.m_mode = O_RDONLY;
            return null;
        }
    }
}
