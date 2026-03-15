#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Globalization;

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    /// <summary>
    /// Internal format of a TIFF directory entry.
    /// </summary>
    class TiffDirectory
    {
        /// <summary>
        /// bit vector of fields that are set
        /// </summary>
        public int[] td_fieldsset = new int[FieldBit.SetLongs];

        public int td_imagewidth;
        public int td_imagelength;
        public int td_imagedepth;
        public int td_tilewidth;
        public int td_tilelength;
        public int td_tiledepth;
        public FileType td_subfiletype;
        public short td_bitspersample;
        public SampleFormat td_sampleformat;
        public Compression td_compression;
        public Photometric td_photometric;
        public Threshold td_threshholding;
        public FillOrder td_fillorder;
        public Orientation td_orientation;
        public short td_samplesperpixel;
        public int td_rowsperstrip;
        public ushort td_minsamplevalue;
        public ushort td_maxsamplevalue;
        public double td_sminsamplevalue;
        public double td_smaxsamplevalue;
        public float td_xresolution;
        public float td_yresolution;
        public ResUnit td_resolutionunit;
        public PlanarConfig td_planarconfig;
        public float td_xposition;
        public float td_yposition;
        public short[] td_pagenumber = new short[2];
        public short[][] td_colormap = { null, null, null };
        public short[] td_halftonehints = new short[2];
        public short td_extrasamples;
        public ExtraSample[] td_sampleinfo;
        public int td_stripsperimage;

        /// <summary>
        /// size of offset and bytecount arrays
        /// </summary>
        public int td_nstrips;
        public uint[] td_stripoffset;
        public uint[] td_stripbytecount;

        /// <summary>
        /// is the bytecount array sorted ascending?
        /// </summary>
        public bool td_stripbytecountsorted;

        public short td_nsubifd;
        public int[] td_subifd;

        // YCbCr parameters
        public short[] td_ycbcrsubsampling = new short[2];
        public YCbCrPosition td_ycbcrpositioning;
        
        // Colorimetry parameters
        public float[] td_refblackwhite;
        public short[][] td_transferfunction = { null, null, null };
        
        // CMYK parameters
        public int td_inknameslen;
        public string td_inknames;

        public int td_customValueCount;
        public TiffTagValue[] td_customValues;

        public TiffDirectory()
        {
            td_subfiletype = 0;
            td_compression = 0;
            td_photometric = 0;
            td_planarconfig = 0;

            td_fillorder = FillOrder.MSB2LSB;
            td_bitspersample = 1;
            td_threshholding = Threshold.BILEVEL;
            td_orientation = Orientation.TOPLEFT;
            td_samplesperpixel = 1;
            td_rowsperstrip = -1;
            td_tiledepth = 1;
            td_stripbytecountsorted = true; // Our own arrays always sorted.
            td_resolutionunit = ResUnit.INCH;
            td_sampleformat = SampleFormat.UINT;
            td_imagedepth = 1;
            td_ycbcrsubsampling[0] = 2;
            td_ycbcrsubsampling[1] = 2;
            td_ycbcrpositioning = YCbCrPosition.CENTERED;
        }
    }

    class TiffDirEntry
    {
        public const int SizeInBytes = 12;

        public TiffTag tdir_tag;
        public TiffType tdir_type;

        /// <summary>
        /// number of items; length in spec
        /// </summary>
        public int tdir_count;

        /// <summary>
        /// byte offset to field data
        /// </summary>
        public uint tdir_offset;

        public new string ToString()
        {
            return tdir_tag.ToString() + ", " + tdir_type.ToString() + " " +
                tdir_offset.ToString(CultureInfo.InvariantCulture);
        }
    }

    struct TiffHeader
    {
        public const int TIFF_MAGIC_SIZE = 2;
        public const int TIFF_VERSION_SIZE = 2;
        public const int TIFF_DIROFFSET_SIZE = 4;

        public const int SizeInBytes = TIFF_MAGIC_SIZE + TIFF_VERSION_SIZE + TIFF_DIROFFSET_SIZE;

        /// <summary>
        /// magic number (defines byte order)
        /// </summary>
        public short tiff_magic;

        /// <summary>
        /// TIFF version number
        /// </summary>
        public short tiff_version;

        /// <summary>
        /// byte offset to first directory
        /// </summary>
        public uint tiff_diroff;
    }

    struct TiffTagValue
    {
        public TiffFieldInfo info;
        public int count;
        public byte[] value;
    }
}
