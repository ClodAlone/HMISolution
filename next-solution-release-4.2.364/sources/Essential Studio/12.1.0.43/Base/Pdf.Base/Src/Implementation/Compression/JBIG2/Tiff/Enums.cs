using System;

namespace Syncfusion.Pdf.Compression.JBIG2
{
    /// <summary>
    /// Regenerated line info.
    /// </summary>
    enum CleanFaxData
    {
        /// <summary>
        /// No errors detected.
        /// </summary>
        CLEAN = 0,

        /// <summary>
        /// Receiver regenerated lines.
        /// </summary>
        REGENERATED = 1,

        /// <summary>
        /// Uncorrected errors exist.
        /// </summary>
        UNCLEAN = 2,
    }

    /// <summary>
    /// Compression scheme.
    /// </summary>
    enum Compression
    {
        /// <summary>
        /// Dump mode.
        /// </summary>
        NONE = 1,

        /// <summary>
        /// CCITT modified Huffman RLE.
        /// </summary>
        CCITTRLE = 2,

        /// <summary>
        /// CCITT Group 3 fax encoding.
        /// </summary>
        CCITTFAX3 = 3,

        /// <summary>
        /// CCITT T.4 (TIFF 6 name for CCITT Group 3 fax encoding).
        /// </summary>
        CCITT_T4 = 3,

        /// <summary>
        /// CCITT Group 4 fax encoding.
        /// </summary>
        CCITTFAX4 = 4,

        /// <summary>
        /// CCITT T.6 (TIFF 6 name for CCITT Group 4 fax encoding).
        /// </summary>
        CCITT_T6 = 4,

        /// <summary>
        /// Lempel-Ziv &amp; Welch.
        /// </summary>
        LZW = 5,

        /// <summary>
        /// Original JPEG / Old-style JPEG (6.0).
        /// </summary>
        OJPEG = 6,

        /// <summary>
        /// JPEG DCT compression. 
        /// </summary>
        JPEG = 7,

        /// <summary>
        /// NeXT 2-bit RLE.
        /// </summary>
        NEXT = 32766,

        /// <summary>
        /// CCITT RLE.
        /// </summary>
        CCITTRLEW = 32771,

        /// <summary>
        /// Macintosh RLE.
        /// </summary>
        PACKBITS = 32773,

        /// <summary>
        /// ThunderScan RLE.
        /// </summary>
        THUNDERSCAN = 32809,

        /// <summary>
        /// IT8 CT w/padding. Reserved for ANSI IT8 TIFF/IT.
        /// </summary>
        IT8CTPAD = 32895,

        /// <summary>
        /// IT8 Linework RLE. Reserved for ANSI IT8 TIFF/IT.
        /// </summary>
        IT8LW = 32896,

        /// <summary>
        /// IT8 Monochrome picture. Reserved for ANSI IT8 TIFF/IT.
        /// </summary>
        IT8MP = 32897,

        /// <summary>
        /// IT8 Binary line art. Reserved for ANSI IT8 TIFF/IT.
        /// </summary>
        IT8BL = 32898,

        /// <summary>
        /// Pixar companded 10bit LZW. Reserved for Pixar.
        /// </summary>
        PIXARFILM = 32908,

        /// <summary>
        /// Pixar companded 11bit ZIP. Reserved for Pixar.
        /// </summary>
        PIXARLOG = 32909,

        /// <summary>
        /// Deflate compression.
        /// </summary>
        DEFLATE = 32946,

        /// <summary>
        /// Deflate compression, as recognized by Adobe.
        /// </summary>
        ADOBE_DEFLATE = 8,

        /// <summary>
        /// Kodak DCS encoding.
        /// </summary>
        DCS = 32947,

        /// <summary>
        /// ISO JBIG.
        /// </summary>
        JBIG = 34661,

        /// <summary>
        /// SGI Log Luminance RLE.
        /// </summary>
        SGILOG = 34676,

        /// <summary>
        /// SGI Log 24-bit packed.
        /// </summary>
        SGILOG24 = 34677,

        /// <summary>
        /// Leadtools JPEG2000.
        /// </summary>
        JP2000 = 34712,
    }

    /// <summary>
    /// Information about extra samples.
    /// </summary>
    enum ExtraSample
    {
        /// <summary>
        /// Unspecified data.
        /// </summary>
        UNSPECIFIED = 0,

        /// <summary>
        /// Associated alpha data.
        /// </summary>
        ASSOCALPHA = 1,

        /// <summary>
        /// Unassociated alpha data.
        /// </summary>
        UNASSALPHA = 2,
    }

    /// <summary>
    /// Group 3/4 format control.
    /// </summary>
    enum FaxMode
    {
        /// <summary>
        /// Default, include RTC.
        /// </summary>
        CLASSIC = 0x0000,

        /// <summary>
        /// No RTC at end of data.
        /// </summary>
        NORTC = 0x0001,

        /// <summary>
        /// No EOL code at end of row.
        /// </summary>
        NOEOL = 0x0002,

        /// <summary>
        /// Byte align row.
        /// </summary>
        BYTEALIGN = 0x0004,

        /// <summary>
        /// Word align row.
        /// </summary>
        WORDALIGN = 0x0008,

        /// <summary>
        /// TIFF Class F.
        /// </summary>
        CLASSF = NORTC,
    }

    /// <summary>
    /// Subfile data descriptor.
    /// </summary>
    enum FileType
    {
        /// <summary>
        /// Reduced resolution version.
        /// </summary>
        REDUCEDIMAGE = 0x1,

        /// <summary>
        /// One page of many.
        /// </summary>
        PAGE = 0x2,

        /// <summary>
        /// Transparency mask.
        /// </summary>
        MASK = 0x4
    }

    /// <summary>
    /// Data order within a byte.
    /// </summary>
    enum FillOrder
    {
        /// <summary>
        /// Most significant -> least.
        /// </summary>
        MSB2LSB = 1,

        /// <summary>
        /// Least significant -> most.
        /// </summary>
        LSB2MSB = 2,
    }

    /// <summary>
    /// Options for CCITT Group 3/4 fax encoding.
    /// </summary>
    enum Group3Opt
    {
        /// <summary>
        /// Unknown (uninitialized).
        /// </summary>
        UNKNOWN = -1,

        /// <summary>
        /// 2-dimensional coding.
        /// </summary>
        ENCODING2D = 0x1,

        /// <summary>
        /// Data not compressed.
        /// </summary>
        UNCOMPRESSED = 0x2,

        /// <summary>
        /// Fill to byte boundary.
        /// </summary>
        FILLBITS = 0x4,
    }

    /// <summary>
    /// Inks in separated image.
    /// </summary>
    enum InkSet
    {
        /// <summary>
        /// Cyan-magenta-yellow-black color.
        /// </summary>
        CMYK = 1,

        /// <summary>
        /// Multi-ink or hi-fi color.
        /// </summary>
        MULTIINK = 2,
    }

    /// <summary>
    /// Auto RGB&lt;=&gt;YCbCr convert.
    /// </summary>
    enum JpegColorMode
    {
        /// <summary>
        /// No conversion (default).
        /// </summary>
        RAW = 0x0000,

        /// <summary>
        /// Do auto conversion.
        /// </summary>
        RGB = 0x0001,
    }

    /// <summary>
    /// Jpeg Tables Mode.
    /// </summary>
    enum JpegTablesMode
    {
        /// <summary>
        /// None.
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Include quantization tables.
        /// </summary>
        QUANT = 0x0001,

        /// <summary>
        /// Include Huffman tables.
        /// </summary>
        HUFF = 0x0002,
    }

    /// <summary>
    /// Kind of data in subfile.
    /// </summary>
    enum OFileType
    {
        /// <summary>
        /// Full resolution image data.
        /// </summary>
        IMAGE = 1,

        /// <summary>
        /// Reduced size image data.
        /// </summary>
        REDUCEDIMAGE = 2,

        /// <summary>
        /// One page of many.
        /// </summary>
        PAGE = 3
    }

    /// <summary>
    /// Image orientation.
    /// </summary>
    enum Orientation
    {
        /// <summary>
        /// Row 0 top, Column 0 lhs.
        /// </summary>
        TOPLEFT = 1,

        /// <summary>
        /// Row 0 top, Column 0 rhs.
        /// </summary>
        TOPRIGHT = 2,

        /// <summary>
        /// Row 0 bottom, Column 0 rhs.
        /// </summary>
        BOTRIGHT = 3,

        /// <summary>
        /// Row 0 bottom, Column 0 lhs.
        /// </summary>
        BOTLEFT = 4,

        /// <summary>
        /// Row 0 lhs, Column 0 top.
        /// </summary>
        LEFTTOP = 5,

        /// <summary>
        /// Row 0 rhs, Column 0 top.
        /// </summary>
        RIGHTTOP = 6,

        /// <summary>
        /// Row 0 rhs, Column 0 bottom.
        /// </summary>
        RIGHTBOT = 7,

        /// <summary>
        /// Row 0 lhs, Column 0 bottom.
        /// </summary>
        LEFTBOT = 8,
    }

    /// <summary>
    /// Photometric interpretation.
    /// </summary>
    enum Photometric
    {
        /// <summary>
        /// Min value is white.
        /// </summary>
        MINISWHITE = 0,

        /// <summary>
        /// Min value is black.
        /// </summary>
        MINISBLACK = 1,

        /// <summary>
        /// RGB color model.
        /// </summary>
        RGB = 2,

        /// <summary>
        /// Color map indexed.
        /// </summary>
        PALETTE = 3,

        /// <summary>
        /// [obsoleted by TIFF rev. 6.0] Holdout mask.
        /// </summary>
        MASK = 4,

        /// <summary>
        /// Color separations.
        /// </summary>
        SEPARATED = 5,

        /// <summary>
        /// CCIR 601.
        /// </summary>
        YCBCR = 6,

        /// <summary>
        /// 1976 CIE L*a*b*.
        /// </summary>
        CIELAB = 8,

        /// <summary>
        /// ICC L*a*b*. Introduced post TIFF rev 6.0 by Adobe TIFF Technote 4.
        /// </summary>
        ICCLAB = 9,

        /// <summary>
        /// ITU L*a*b*.
        /// </summary>
        ITULAB = 10,

        /// <summary>
        /// CIE Log2(L).
        /// </summary>
        LOGL = 32844,

        /// <summary>
        /// CIE Log2(L) (u',v').
        /// </summary>
        LOGLUV = 32845,
    }

    /// <summary>
    /// Storage organization.
    /// </summary>
    enum PlanarConfig
    {
        /// <summary>
        /// Unknown (uninitialized).
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// Single image plane.
        /// </summary>
        CONTIG = 1,

        /// <summary>
        /// Separate planes of data.
        /// </summary>
        SEPARATE = 2
    }

    /// <summary>
    /// Prediction scheme w/ LZW.
    /// </summary>
    enum Predictor
    {
        /// <summary>
        /// No prediction scheme used.
        /// </summary>
        NONE = 1,

        /// <summary>
        /// Horizontal differencing.
        /// </summary>
        HORIZONTAL = 2,

        /// <summary>
        /// Floating point predictor.
        /// </summary>
        FLOATINGPOINT = 3,
    }

    /// <summary>
    /// Units of resolutions.
    /// </summary>
    enum ResUnit
    {
        /// <summary>
        /// No meaningful units.
        /// </summary>
        NONE = 1,

        /// <summary>
        /// English.
        /// </summary>
        INCH = 2,

        /// <summary>
        /// Metric.
        /// </summary>
        CENTIMETER = 3,
    }

    /// <summary>
    /// Data sample format.
    /// </summary>
    enum SampleFormat
    {
        /// <summary>
        /// Unsigned integer data
        /// </summary>
        UINT = 1,

        /// <summary>
        /// Signed integer data
        /// </summary>
        INT = 2,

        /// <summary>
        /// IEEE floating point data
        /// </summary>
        IEEEFP = 3,

        /// <summary>
        /// Untyped data
        /// </summary>
        VOID = 4,

        /// <summary>
        /// Complex signed int
        /// </summary>
        COMPLEXINT = 5,

        /// <summary>
        /// Complex ieee floating
        /// </summary>
        COMPLEXIEEEFP = 6,
    }

    /// <summary>
    /// Thresholding used on data.
    /// </summary>
    enum Threshold
    {
        /// <summary>
        /// B&amp;W art scan.
        /// </summary>
        BILEVEL = 1,

        /// <summary>
        /// Dithered scan.
        /// </summary>
        HALFTONE = 2,

        /// <summary>
        /// Usually Floyd-Steinberg.
        /// </summary>
        ERRORDIFFUSE = 3,
    }

    /// <summary>
    /// Tag data type.
    /// </summary>
    enum TiffType : short
    {
        /// <summary>
        /// Placeholder.
        /// </summary>
        NOTYPE = 0,

        /// <summary>
        /// For field descriptor searching.
        /// </summary>
        ANY = NOTYPE,

        /// <summary>
        /// 8-bit unsigned integer.
        /// </summary>
        BYTE = 1,

        /// <summary>
        /// 8-bit bytes with last byte <c>null</c>.
        /// </summary>
        ASCII = 2,

        /// <summary>
        /// 16-bit unsigned integer.
        /// </summary>
        SHORT = 3,

        /// <summary>
        /// 32-bit unsigned integer.
        /// </summary>
        LONG = 4,

        /// <summary>
        /// 64-bit unsigned fraction.
        /// </summary>
        RATIONAL = 5,

        /// <summary>
        /// 8-bit signed integer.
        /// </summary>
        SBYTE = 6,

        /// <summary>
        /// 8-bit untyped data.
        /// </summary>
        UNDEFINED = 7,

        /// <summary>
        /// 16-bit signed integer.
        /// </summary>
        SSHORT = 8,

        /// <summary>
        /// 32-bit signed integer.
        /// </summary>
        SLONG = 9,

        /// <summary>
        /// 64-bit signed fraction.
        /// </summary>
        SRATIONAL = 10,

        /// <summary>
        /// 32-bit IEEE floating point.
        /// </summary>
        FLOAT = 11,

        /// <summary>
        /// 64-bit IEEE floating point.
        /// </summary>
        DOUBLE = 12,

        /// <summary>
        /// 32-bit unsigned integer (offset)
        /// </summary>
        IFD = 13
    }

    /// <summary>
    /// Subsample positioning.
    /// </summary>
    enum YCbCrPosition
    {
        /// <summary>
        /// As in PostScript Level 2
        /// </summary>
        CENTERED = 1,

        /// <summary>
        /// As in CCIR 601-1
        /// </summary>
        COSITED = 2,
    }
    /// <summary>
    /// TIFF tag definitions.
    /// </summary>
    enum TiffTag
    {
        /// <summary>
        /// Tag placeholder
        /// </summary>
        IGNORE = 0,
        
        /// <summary>
        /// Subfile data descriptor.
        /// For the list of possible values, see <see cref="FileType"/>.
        /// </summary>
        SUBFILETYPE = 254,
        
        /// <summary>
        /// Kind of data in subfile. For the list of possible values, see <see cref="OFileType"/>.
        /// </summary>
        OSUBFILETYPE = 255,
        
        /// <summary>
        /// Image width in pixels.
        /// </summary>
        IMAGEWIDTH = 256,
        
        /// <summary>
        /// Image height in pixels.
        /// </summary>
        IMAGELENGTH = 257,
        
        /// <summary>
        /// Bits per channel (sample).
        /// </summary>
        BITSPERSAMPLE = 258,
        
        /// <summary>
        /// Data compression technique.
        /// For the list of possible values, see <see cref="Compression"/>.
        /// </summary>
        COMPRESSION = 259,
        
        /// <summary>
        /// Photometric interpretation.
        /// For the list of possible values, see <see cref="Photometric"/>.
        /// </summary>
        PHOTOMETRIC = 262,
        
        /// <summary>
        /// Thresholding used on data. For the list of possible values, see <see cref="Threshold"/>.
        /// </summary>
        THRESHHOLDING = 263,
        
        /// <summary>
        /// Dithering matrix width.
        /// </summary>
        CELLWIDTH = 264,
        
        /// <summary>
        /// Dithering matrix height.
        /// </summary>
        CELLLENGTH = 265,
        
        /// <summary>
        /// Data order within a byte.
        /// </summary>
        FILLORDER = 266,
        
        /// <summary>
        /// Name of document which holds for image.
        /// </summary>
        DOCUMENTNAME = 269,
        
        /// <summary>
        /// Information about image.
        /// </summary>
        IMAGEDESCRIPTION = 270,
        
        /// <summary>
        /// Scanner manufacturer name.
        /// </summary>
        MAKE = 271,
        
        /// <summary>
        /// Scanner model name/number.
        /// </summary>
        MODEL = 272,
        
        /// <summary>
        /// Offsets to data strips.
        /// </summary>
        STRIPOFFSETS = 273,
        
        /// <summary>
        /// Image orientation. For the list of possible values, see <see cref="Orientation"/>.
        /// </summary>
        ORIENTATION = 274,
        
        /// <summary>
        /// Samples per pixel.
        /// </summary>
        SAMPLESPERPIXEL = 277,
        
        /// <summary>
        /// Rows per strip of data.
        /// </summary>
        ROWSPERSTRIP = 278,
        
        /// <summary>
        /// Bytes counts for strips.
        /// </summary>
        STRIPBYTECOUNTS = 279,
        
        /// <summary>
        /// Minimum sample value.
        /// </summary>
        MINSAMPLEVALUE = 280,
        
        /// <summary>
        /// Maximum sample value.
        /// </summary>
        MAXSAMPLEVALUE = 281,
        
        /// <summary>
        /// Pixels/resolution in x.
        /// </summary>
        XRESOLUTION = 282,
        
        /// <summary>
        /// Pixels/resolution in y.
        /// </summary>
        YRESOLUTION = 283,
        
        /// <summary>
        /// Storage organization.
        /// For the list of possible values, see <see cref="PlanarConfig"/>.
        /// </summary>
        PLANARCONFIG = 284,
        
        /// <summary>
        /// Page name image is from.
        /// </summary>
        PAGENAME = 285,
        
        /// <summary>
        /// X page offset of image lhs.
        /// </summary>
        XPOSITION = 286,
        
        /// <summary>
        /// Y page offset of image lhs.
        /// </summary>
        YPOSITION = 287,
        
        /// <summary>
        /// Byte offset to free block.
        /// </summary>
        FREEOFFSETS = 288,
        
        /// <summary>
        /// Sizes of free blocks.
        /// </summary>
        FREEBYTECOUNTS = 289,
        
        /// <summary>
        /// Gray scale curve accuracy.
        /// </summary>
        GRAYRESPONSEUNIT = 290,
        
        /// <summary>
        /// Gray scale response curve.
        /// </summary>
        GRAYRESPONSECURVE = 291,
        
        /// <summary>
        /// Options for CCITT Group 3 fax encoding. 32 flag bits.
        /// </summary>
        GROUP3OPTIONS = 292,
        
        /// <summary>
        /// </summary>
        T4OPTIONS = 292,
        
        /// <summary>
        /// Options for CCITT Group 4 fax encoding. 32 flag bits.
        /// </summary>
        GROUP4OPTIONS = 293,
        
        /// <summary>
        /// </summary>
        T6OPTIONS = 293,
        
        /// <summary>
        /// Units of resolutions.
        /// </summary>
        RESOLUTIONUNIT = 296,
        
        /// <summary>
        /// Page numbers of multi-page.
        /// </summary>
        PAGENUMBER = 297,
        
        /// <summary>
        /// Color curve accuracy.
        /// </summary>
        COLORRESPONSEUNIT = 300,
        
        /// <summary>
        /// Colorimetry info.
        /// </summary>
        TRANSFERFUNCTION = 301,
        
        /// <summary>
        /// Name &amp; release.
        /// </summary>
        SOFTWARE = 305,
        
        /// <summary>
        /// Creation date and time.
        /// </summary>
        DATETIME = 306,
        
        /// <summary>
        /// Creator of image.
        /// </summary>
        ARTIST = 315,
        
        /// <summary>
        /// Machine where created.
        /// </summary>
        HOSTCOMPUTER = 316,
        
        /// <summary>
        /// Prediction scheme w/ LZW.
        /// </summary>
        PREDICTOR = 317,
        
        /// <summary>
        /// Image white point.
        /// </summary>
        WHITEPOINT = 318,
        
        /// <summary>
        /// Primary chromaticities.
        /// </summary>
        PRIMARYCHROMATICITIES = 319,
        
        /// <summary>
        /// RGB map for pallette image.
        /// </summary>
        COLORMAP = 320,
        
        /// <summary>
        /// Highlight + shadow info.
        /// </summary>
        HALFTONEHINTS = 321,
        
        /// <summary>
        /// Tile width in pixels.
        /// </summary>
        TILEWIDTH = 322,
        
        /// <summary>
        /// Tile height in pixels.
        /// </summary>
        TILELENGTH = 323,
        
        /// <summary>
        /// Offsets to data tiles.
        /// </summary>
        TILEOFFSETS = 324,
        
        /// <summary>
        /// Byte counts for tiles.
        /// </summary>
        TILEBYTECOUNTS = 325,
        
        /// <summary>
        /// Lines with wrong pixel count.
        /// </summary>
        BADFAXLINES = 326,
        
        /// <summary>
        /// Regenerated line info.
        /// </summary>
        CLEANFAXDATA = 327,
        
        /// <summary>
        /// Max consecutive bad lines.
        /// </summary>
        CONSECUTIVEBADFAXLINES = 328,
        
        /// <summary>
        /// Subimage descriptors.
        /// </summary>
        SUBIFD = 330,
        
        /// <summary>
        /// Inks in separated image.
        /// </summary>
        INKSET = 332,
        
        /// <summary>
        /// ASCII names of inks.
        /// </summary>
        INKNAMES = 333,
        
        /// <summary>
        /// Number of inks.
        /// </summary>
        NUMBEROFINKS = 334,
        
        /// <summary>
        /// 0% and 100% dot codes.
        /// </summary>
        DOTRANGE = 336,
        
        /// <summary>
        /// Separation target.
        /// </summary>
        TARGETPRINTER = 337,
        
        /// <summary>
        /// Information about extra samples.
        /// </summary>
        EXTRASAMPLES = 338,
        
        /// <summary>
        /// Data sample format.
        /// </summary>
        SAMPLEFORMAT = 339,
        
        /// <summary>
        /// Variable MinSampleValue.
        /// </summary>
        SMINSAMPLEVALUE = 340,
        
        /// <summary>
        /// Variable MaxSampleValue.
        /// </summary>
        SMAXSAMPLEVALUE = 341,
        
        /// <summary>
        /// ClipPath.
        /// </summary>
        CLIPPATH = 343,
        
        /// <summary>
        /// XClipPathUnits. 
        /// </summary>
        XCLIPPATHUNITS = 344,
        
        /// <summary>
        /// YClipPathUnits. 
        /// </summary>
        YCLIPPATHUNITS = 345,
        
        /// <summary>
        /// Indexed.
        /// </summary>
        INDEXED = 346,
        
        /// <summary>
        /// JPEG table stream.
        /// </summary>
        JPEGTABLES = 347,
        
        /// <summary>
        /// OPI Proxy.
        /// </summary>
        OPIPROXY = 351,
        
        /// <summary>
        /// JPEG processing algorithm.
        /// </summary>
        JPEGPROC = 512,
        
        /// <summary>
        /// Pointer to SOI marker.
        /// </summary>
        JPEGIFOFFSET = 513,
        
        /// <summary>
        /// JFIF stream length
        /// </summary>
        JPEGIFBYTECOUNT = 514,
        
        /// <summary>
        /// Restart interval length.
        /// </summary>
        JPEGRESTARTINTERVAL = 515,
        
        /// <summary>
        /// Lossless proc predictor.
        /// </summary>
        JPEGLOSSLESSPREDICTORS = 517,
        
        /// <summary>
        /// Lossless point transform.
        /// </summary>
        JPEGPOINTTRANSFORM = 518,
        
        /// <summary>
        /// Q matrice offsets.
        /// </summary>
        JPEGQTABLES = 519,
        
        /// <summary>
        /// DCT table offsets.
        /// </summary>
        JPEGDCTABLES = 520,
        
        /// <summary>
        /// AC coefficient offsets.
        /// </summary>
        JPEGACTABLES = 521,
        
        /// <summary>
        /// RGB -> YCbCr transform.
        /// </summary>
        YCBCRCOEFFICIENTS = 529,
        
        /// <summary>
        /// YCbCr subsampling factors.
        /// </summary>
        YCBCRSUBSAMPLING = 530,
        
        /// <summary>
        /// Subsample positioning.
        /// </summary>
        YCBCRPOSITIONING = 531,
        
        /// <summary>
        /// Colorimetry info.
        /// </summary>
        REFERENCEBLACKWHITE = 532,
        
        /// <summary>
        /// XML packet.
        /// </summary>
        XMLPACKET = 700,
        
        /// <summary>
        /// OPI ImageID.
        /// </summary>
        OPIIMAGEID = 32781,
        
        /// <summary>
        /// Image reference points.
        /// </summary>
        REFPTS = 32953,
        
        /// <summary>
        /// Region-xform tack point.
        /// </summary>
        REGIONTACKPOINT = 32954,
        
        /// <summary>
        /// Warp quadrilateral.
        /// </summary>
        REGIONWARPCORNERS = 32955,
        
        /// <summary>
        /// Affine transformation matrix.
        /// </summary>
        REGIONAFFINE = 32956,
        
        /// <summary>
        /// Use EXTRASAMPLE tag.
        /// </summary>
        MATTEING = 32995,
        
        /// <summary>
        /// Use SAMPLEFORMAT tag.
        /// </summary>
        DATATYPE = 32996,
        
        /// <summary>
        /// Z depth of image.
        /// </summary>
        IMAGEDEPTH = 32997,
        
        /// <summary>
        /// Z depth/data tile.
        /// </summary>
        TILEDEPTH = 32998,
        
        /// <summary>
        /// Full image size in X.
        /// </summary>
        PIXAR_IMAGEFULLWIDTH = 33300,
        
        /// <summary>
        /// Full image size in Y.
        /// </summary>
        PIXAR_IMAGEFULLLENGTH = 33301,
        
        /// <summary>
        /// Texture map format.
        /// </summary>
        PIXAR_TEXTUREFORMAT = 33302,
        
        /// <summary>
        /// S&amp;T wrap modes. 
        /// </summary>
        PIXAR_WRAPMODES = 33303,
        
        /// <summary>
        /// Cotan(fov) for env. maps.
        /// </summary>
        PIXAR_FOVCOT = 33304,
        
        /// <summary>
        /// Used to identify special image modes and data used by Pixar's texture formats.
        /// </summary>
        PIXAR_MATRIX_WORLDTOSCREEN = 33305,
        
        /// <summary>
        /// Used to identify special image modes and data used by Pixar's texture formats.
        /// </summary>
        PIXAR_MATRIX_WORLDTOCAMERA = 33306,
        
        /// <summary>
        /// Device serial number.
        /// </summary>
        WRITERSERIALNUMBER = 33405,
        
        /// <summary>
        /// Copyright string.
        /// </summary>
        COPYRIGHT = 33432,
        
        /// <summary>
        /// IPTC TAG from RichTIFF specifications.
        /// </summary>
        RICHTIFFIPTC = 33723,
        
        /// <summary>
        /// Site name.
        /// </summary>
        IT8SITE = 34016,
        
        /// <summary>
        /// Color seq. [RGB, CMYK, etc].
        /// </summary>
        IT8COLORSEQUENCE = 34017,
        
        /// <summary>
        /// DDES Header.
        /// </summary>
        IT8HEADER = 34018,
        
        /// <summary>
        /// Raster scanline padding.
        /// </summary>
        IT8RASTERPADDING = 34019,
        
        /// <summary>
        /// The number of bits in short run.
        /// </summary>
        IT8BITSPERRUNLENGTH = 34020,
        
        /// <summary>
        /// The number of bits in long run.
        /// </summary>
        IT8BITSPEREXTENDEDRUNLENGTH = 34021,
        
        /// <summary>
        /// LW colortable.
        /// </summary>
        IT8COLORTABLE = 34022,
        
        /// <summary>
        /// BP/BL image color switch.
        /// </summary>
        IT8IMAGECOLORINDICATOR = 34023,
        
        /// <summary>
        /// BP/BL bg color switch.
        /// </summary>
        IT8BKGCOLORINDICATOR = 34024,
        
        /// <summary>
        /// BP/BL image color value.
        /// </summary>
        IT8IMAGECOLORVALUE = 34025,
        
        /// <summary>
        /// BP/BL bg color value.
        /// </summary>
        IT8BKGCOLORVALUE = 34026,
        
        /// <summary>
        /// MP pixel intensity value.
        /// </summary>
        IT8PIXELINTENSITYRANGE = 34027,
        
        /// <summary>
        /// HC transparency switch.
        /// </summary>
        IT8TRANSPARENCYINDICATOR = 34028,
        
        /// <summary>
        /// Color characterization table.
        /// </summary>
        IT8COLORCHARACTERIZATION = 34029,
        
        /// <summary>
        /// HC usage indicator. 
        /// </summary>
        IT8HCUSAGE = 34030,
        
        /// <summary>
        /// Trapping indicator (untrapped = 0, trapped = 1).
        /// </summary>
        IT8TRAPINDICATOR = 34031,
        
        /// <summary>
        /// CMYK color equivalents.
        /// </summary>
        IT8CMYKEQUIVALENT = 34032,
        
        /// <summary>
        /// Sequence Frame Count.
        /// </summary>
        FRAMECOUNT = 34232,
        
        /// <summary>
        /// </summary>
        PHOTOSHOP = 34377,
        
        /// <summary>
        /// Pointer to EXIF private directory.
        /// </summary>
        EXIFIFD = 34665,
        
        /// <summary>
        /// ICC profile data. 
        /// </summary>
        ICCPROFILE = 34675,
        
        /// <summary>
        /// JBIG options.
        /// </summary>
        JBIGOPTIONS = 34750,
        
        /// <summary>
        /// Pointer to GPS private directory. 
        /// </summary>
        GPSIFD = 34853,
        
        /// <summary>
        /// Encoded Class 2 ses. params. 
        /// </summary>
        FAXRECVPARAMS = 34908,
        
        /// <summary>
        /// Received SubAddr string. 
        /// </summary>
        FAXSUBADDRESS = 34909,
        
        /// <summary>
        /// Receive time (secs).
        /// </summary>
        FAXRECVTIME = 34910,
        
        /// <summary>
        /// Encoded fax ses. params, Table 2/T.30. 
        /// </summary>
        FAXDCS = 34911,
        
        /// <summary>
        /// Sample value to Nits.
        /// </summary>
        STONITS = 37439,
        
        /// <summary>
        /// </summary>
        FEDEX_EDR = 34929,
        
        /// <summary>
        /// Pointer to Interoperability private directory.
        /// </summary>
        INTEROPERABILITYIFD = 40965,
        
        /// <summary>
        /// DNG version number.
        /// </summary>
        DNGVERSION = 50706,
        
        /// <summary>
        /// DNG compatibility version.
        /// </summary>
        DNGBACKWARDVERSION = 50707,
        
        /// <summary>
        /// Name for the camera model.
        /// </summary>
        UNIQUECAMERAMODEL = 50708,
        
        /// <summary>
        /// Localized camera model name.
        /// </summary>
        LOCALIZEDCAMERAMODEL = 50709,
        
        /// <summary>
        /// CFAPattern->LinearRaw space mapping.
        /// </summary>
        CFAPLANECOLOR = 50710,
        
        /// <summary>
        /// Spatial layout of the CFA.
        /// </summary>
        CFALAYOUT = 50711,
        
        /// <summary>
        /// Lookup table description.
        /// </summary>
        LINEARIZATIONTABLE = 50712,
        
        /// <summary>
        /// Repeat pattern size for the BlackLevel tag.
        /// </summary>
        BLACKLEVELREPEATDIM = 50713,
        
        /// <summary>
        /// Zero light encoding level.
        /// </summary>
        BLACKLEVEL = 50714,
        
        /// <summary>
        /// Zero light encoding level differences (columns).
        /// </summary>
        BLACKLEVELDELTAH = 50715,
        
        /// <summary>
        /// Zero light encoding level differences (rows). 
        /// </summary>
        BLACKLEVELDELTAV = 50716,
        
        /// <summary>
        /// Fully saturated encoding level. 
        /// </summary>
        WHITELEVEL = 50717,
        
        /// <summary>
        /// Default scale factors.
        /// </summary>
        DEFAULTSCALE = 50718,
        
        /// <summary>
        /// Origin of the final image area.
        /// </summary>
        DEFAULTCROPORIGIN = 50719,
        
        /// <summary>
        /// Size of the final image area. 
        /// </summary>
        DEFAULTCROPSIZE = 50720,
        
        /// <summary>
        /// XYZ->reference color space transformation matrix 1.
        /// </summary>
        COLORMATRIX1 = 50721,
        
        /// <summary>
        /// XYZ->reference color space transformation matrix 2.
        /// </summary>
        COLORMATRIX2 = 50722,
        
        /// <summary>
        /// Calibration matrix 1.
        /// </summary>
        CAMERACALIBRATION1 = 50723,
        
        /// <summary>
        /// Calibration matrix 2.
        /// </summary>
        CAMERACALIBRATION2 = 50724,
        
        /// <summary>
        /// Dimensionality reduction matrix 1.
        /// </summary>
        REDUCTIONMATRIX1 = 50725,
        
        /// <summary>
        /// Dimensionality reduction matrix 2.
        /// </summary>
        REDUCTIONMATRIX2 = 50726,
        
        /// <summary>
        /// Gain applied the stored raw values.
        /// </summary>
        ANALOGBALANCE = 50727,
        
        /// <summary>
        /// Selected white balance in linear reference space.
        /// </summary>
        ASSHOTNEUTRAL = 50728,
        
        /// <summary>
        /// Selected white balance in x-y chromaticity coordinates.
        /// </summary>
        ASSHOTWHITEXY = 50729,
        
        /// <summary>
        /// How much to move the zero point.
        /// </summary>
        BASELINEEXPOSURE = 50730,
        
        /// <summary>
        /// Relative noise level.
        /// </summary>
        BASELINENOISE = 50731,
        
        /// <summary>
        /// Relative amount of sharpening.
        /// </summary>
        BASELINESHARPNESS = 50732,
        
        /// <summary>
        /// How closely the values of the green pixels in the blue/green rows 
        /// track the values of the green pixels in the red/green rows.
        /// </summary>
        BAYERGREENSPLIT = 50733,
        
        /// <summary>
        /// Non-linear encoding range.
        /// </summary>
        LINEARRESPONSELIMIT = 50734,
        
        /// <summary>
        /// Camera's serial number.
        /// </summary>
        CAMERASERIALNUMBER = 50735,
        
        /// <summary>
        /// Information about the lens.
        /// </summary>
        LENSINFO = 50736,
        
        /// <summary>
        /// Chroma blur radius.
        /// </summary>
        CHROMABLURRADIUS = 50737,
        
        /// <summary>
        /// Relative strength of the camera's anti-alias filter.
        /// </summary>
        ANTIALIASSTRENGTH = 50738,
        
        /// <summary>
        /// Used by Adobe Camera Raw.
        /// </summary>
        SHADOWSCALE = 50739,
        
        /// <summary>
        /// Manufacturer's private data.
        /// </summary>
        DNGPRIVATEDATA = 50740,
        
        /// <summary>
        /// Whether the EXIF MakerNote tag is safe to preserve along with the rest of the EXIF data.
        /// </summary>
        MAKERNOTESAFETY = 50741,
        
        /// <summary>
        /// Illuminant 1.
        /// </summary>
        CALIBRATIONILLUMINANT1 = 50778,
        
        /// <summary>
        /// Illuminant 2.
        /// </summary>
        CALIBRATIONILLUMINANT2 = 50779,
        
        /// <summary>
        /// Best quality multiplier.
        /// </summary>
        BESTQUALITYSCALE = 50780,
        
        /// <summary>
        /// Unique identifier for the raw image data. 
        /// </summary>
        RAWDATAUNIQUEID = 50781,
        
        /// <summary>
        /// File name of the original raw file. 
        /// </summary>
        ORIGINALRAWFILENAME = 50827,
        
        /// <summary>
        /// Contents of the original raw file.
        /// </summary>
        ORIGINALRAWFILEDATA = 50828,
        
        /// <summary>
        /// Active (non-masked) pixels of the sensor. 
        /// </summary>
        ACTIVEAREA = 50829,
        
        /// <summary>
        /// List of coordinates of fully masked pixels. 
        /// </summary>
        MASKEDAREAS = 50830,
        
        /// <summary>
        /// Used to map cameras's color space into ICC profile space.
        /// </summary>
        ASSHOTICCPROFILE = 50831,
        
        /// <summary>
        /// Used to map cameras's color space into ICC profile space.
        /// </summary>
        ASSHOTPREPROFILEMATRIX = 50832,
        
        /// <summary>
        /// 
        /// </summary>
        CURRENTICCPROFILE = 50833,
        
        /// <summary>
        /// 
        /// </summary>
        CURRENTPREPROFILEMATRIX = 50834,
        
        /// <summary>
        /// Undefined tag used by Eastman Kodak, hue shift correction data.
        /// </summary>
        DCSHUESHIFTVALUES = 65535,

        /// <summary>
        /// Group 3/4 format control.
        /// </summary>
        FAXMODE = 65536,
        
        /// <summary>
        /// Compression quality level. Quality level is on the IJG 0-100 scale. Default value is 75.
        /// </summary>
        JPEGQUALITY = 65537,
        
        /// <summary>
        /// Auto RGB&lt;=&gt;YCbCr convert.
        /// </summary>
        JPEGCOLORMODE = 65538,
        
        /// <summary>
        /// Default is <see cref="JpegTablesMode.QUANT"/> | <see cref="JpegTablesMode.HUFF"/>.
        /// </summary>
        JPEGTABLESMODE = 65539,
        
        /// <summary>
        /// G3/G4 fill function.
        /// </summary>
        FAXFILLFUNC = 65540,
        
        /// <summary>
        /// PixarLogCodec I/O data sz.
        /// </summary>
        PIXARLOGDATAFMT = 65549,
        
        /// <summary>
        /// Imager mode &amp; filter.
        /// </summary>
        DCSIMAGERTYPE = 65550,
        
        /// <summary>
        /// Interpolation mode.
        /// </summary>
        DCSINTERPMODE = 65551,
        
        /// <summary>
        /// Color balance values.
        /// </summary>
        DCSBALANCEARRAY = 65552,
        
        /// <summary>
        /// Color correction values.
        /// </summary>
        DCSCORRECTMATRIX = 65553,
        
        /// <summary>
        /// Gamma value.
        /// </summary>
        DCSGAMMA = 65554,
        
        /// <summary>
        /// Toe &amp; shoulder points.
        /// </summary>
        DCSTOESHOULDERPTS = 65555,
        
        /// <summary>
        /// Calibration file description.
        /// </summary>
        DCSCALIBRATIONFD = 65556,
        
        /// <summary>
        /// Compression quality level.
        /// Quality level is on the ZLIB 1-9 scale. Default value is -1.
        /// </summary>
        ZIPQUALITY = 65557,
        
        /// <summary>
        /// PixarLog uses same scale.
        /// </summary>
        PIXARLOGQUALITY = 65558,
        
        /// <summary>
        /// Area of image to acquire.
        /// </summary>
        DCSCLIPRECTANGLE = 65559,
        
        /// <summary>
        /// SGILog user data format.
        /// </summary>
        SGILOGDATAFMT = 65560,
        
        /// <summary>
        /// SGILog data encoding control.
        /// </summary>
        SGILOGENCODE = 65561,

        /// <summary>
        /// Exposure time.
        /// </summary>
        EXIF_EXPOSURETIME = 33434,
        
        /// <summary>
        /// F number.
        /// </summary>
        EXIF_FNUMBER = 33437,
        
        /// <summary>
        /// Exposure program.
        /// </summary>
        EXIF_EXPOSUREPROGRAM = 34850,
        
        /// <summary>
        /// Spectral sensitivity.
        /// </summary>
        EXIF_SPECTRALSENSITIVITY = 34852,
        
        /// <summary>
        /// ISO speed rating.
        /// </summary>
        EXIF_ISOSPEEDRATINGS = 34855,
        
        /// <summary>
        /// Optoelectric conversion factor.
        /// </summary>
        EXIF_OECF = 34856,
        
        /// <summary>
        /// Exif version.
        /// </summary>
        EXIF_EXIFVERSION = 36864,
        
        /// <summary>
        /// Date and time of original data generation.
        /// </summary>
        EXIF_DATETIMEORIGINAL = 36867,
        
        /// <summary>
        /// Date and time of digital data generation.
        /// </summary>
        EXIF_DATETIMEDIGITIZED = 36868,
        
        /// <summary>
        /// Meaning of each component.
        /// </summary>
        EXIF_COMPONENTSCONFIGURATION = 37121,
        
        /// <summary>
        /// Image compression mode.
        /// </summary>
        EXIF_COMPRESSEDBITSPERPIXEL = 37122,
        
        /// <summary>
        /// Shutter speed.
        /// </summary>
        EXIF_SHUTTERSPEEDVALUE = 37377,
        
        /// <summary>
        /// Aperture.
        /// </summary>
        EXIF_APERTUREVALUE = 37378,
        
        /// <summary>
        /// Brightness.
        /// </summary>
        EXIF_BRIGHTNESSVALUE = 37379,
        
        /// <summary>
        /// Exposure bias.
        /// </summary>
        EXIF_EXPOSUREBIASVALUE = 37380,
        
        /// <summary>
        /// Maximum lens aperture.
        /// </summary>
        EXIF_MAXAPERTUREVALUE = 37381,
        
        /// <summary>
        /// Subject distance.
        /// </summary>
        EXIF_SUBJECTDISTANCE = 37382,
        
        /// <summary>
        /// Metering mode.
        /// </summary>
        EXIF_METERINGMODE = 37383,
        
        /// <summary>
        /// Light source.
        /// </summary>
        EXIF_LIGHTSOURCE = 37384,
        
        /// <summary>
        /// Flash.
        /// </summary>
        EXIF_FLASH = 37385,
        
        /// <summary>
        /// Lens focal length.
        /// </summary>
        EXIF_FOCALLENGTH = 37386,
        
        /// <summary>
        /// Subject area.
        /// </summary>
        EXIF_SUBJECTAREA = 37396,
        
        /// <summary>
        /// Manufacturer notes.
        /// </summary>
        EXIF_MAKERNOTE = 37500,
        
        /// <summary>
        /// User comments.
        /// </summary>
        EXIF_USERCOMMENT = 37510,
        
        /// <summary>
        /// DateTime subseconds.
        /// </summary>
        EXIF_SUBSECTIME = 37520,
        
        /// <summary>
        /// DateTimeOriginal subseconds.
        /// </summary>
        EXIF_SUBSECTIMEORIGINAL = 37521,
        
        /// <summary>
        /// DateTimeDigitized subseconds.
        /// </summary>
        EXIF_SUBSECTIMEDIGITIZED = 37522,
        
        /// <summary>
        /// Supported Flashpix version.
        /// </summary>
        EXIF_FLASHPIXVERSION = 40960,
        
        /// <summary>
        /// Color space information.
        /// </summary>
        EXIF_COLORSPACE = 40961,
        
        /// <summary>
        /// Valid image width.
        /// </summary>
        EXIF_PIXELXDIMENSION = 40962,
        
        /// <summary>
        /// Valid image height.
        /// </summary>
        EXIF_PIXELYDIMENSION = 40963,
        
        /// <summary>
        /// Related audio file.
        /// </summary>
        EXIF_RELATEDSOUNDFILE = 40964,
        
        /// <summary>
        /// Flash energy.
        /// </summary>
        EXIF_FLASHENERGY = 41483,
        
        /// <summary>
        /// Spatial frequency response.
        /// </summary>
        EXIF_SPATIALFREQUENCYRESPONSE = 41484,
        
        /// <summary>
        /// Focal plane X resolution.
        /// </summary>
        EXIF_FOCALPLANEXRESOLUTION = 41486,
        
        /// <summary>
        /// Focal plane Y resolution.
        /// </summary>
        EXIF_FOCALPLANEYRESOLUTION = 41487,
        
        /// <summary>
        /// Focal plane resolution unit.
        /// </summary>
        EXIF_FOCALPLANERESOLUTIONUNIT = 41488,
        
        /// <summary>
        /// Subject location.
        /// </summary>
        EXIF_SUBJECTLOCATION = 41492,
        
        /// <summary>
        /// Exposure index.
        /// </summary>
        EXIF_EXPOSUREINDEX = 41493,
        
        /// <summary>
        /// Sensing method.
        /// </summary>
        EXIF_SENSINGMETHOD = 41495,
        
        /// <summary>
        /// File source.
        /// </summary>
        EXIF_FILESOURCE = 41728,
        
        /// <summary>
        /// Scene type.
        /// </summary>
        EXIF_SCENETYPE = 41729,
        
        /// <summary>
        /// CFA pattern.
        /// </summary>
        EXIF_CFAPATTERN = 41730,
        
        /// <summary>
        /// Custom image processing.
        /// </summary>
        EXIF_CUSTOMRENDERED = 41985,
        
        /// <summary>
        /// Exposure mode.
        /// </summary>
        EXIF_EXPOSUREMODE = 41986,
        
        /// <summary>
        /// White balance.
        /// </summary>
        EXIF_WHITEBALANCE = 41987,
        
        /// <summary>
        /// Digital zoom ratio.
        /// </summary>
        EXIF_DIGITALZOOMRATIO = 41988,
        
        /// <summary>
        /// Focal length in 35 mm film.
        /// </summary>
        EXIF_FOCALLENGTHIN35MMFILM = 41989,
        
        /// <summary>
        /// Scene capture type.
        /// </summary>
        EXIF_SCENECAPTURETYPE = 41990,
        
        /// <summary>
        /// Gain control.
        /// </summary>
        EXIF_GAINCONTROL = 41991,
        
        /// <summary>
        /// Contrast.
        /// </summary>
        EXIF_CONTRAST = 41992,
        
        /// <summary>
        /// Saturation.
        /// </summary>
        EXIF_SATURATION = 41993,
        
        /// <summary>
        /// Sharpness.
        /// </summary>
        EXIF_SHARPNESS = 41994,
        
        /// <summary>
        /// Device settings description.
        /// </summary>
        EXIF_DEVICESETTINGDESCRIPTION = 41995,
        
        /// <summary>
        /// Subject distance range.
        /// </summary>
        EXIF_SUBJECTDISTANCERANGE = 41996,
        
        /// <summary>
        /// Unique image ID.
        /// </summary>
        EXIF_IMAGEUNIQUEID = 42016,
    }

    /// <summary>
    /// The unit of density.
    /// </summary>
    enum DensityUnit
    {
        /// <summary>
        /// Unknown density
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Dots/inch
        /// </summary>
        DotsInch = 1,

        /// <summary>
        /// Dots/cm
        /// </summary>
        DotsCm = 2
    }

    /// <summary>
    /// Known color spaces.
    /// </summary>
    enum J_COLOR_SPACE
    {
        /// <summary>
        /// Unspecified color space.
        /// </summary>
        JCS_UNKNOWN,

        /// <summary>
        /// Grayscale
        /// </summary>
        JCS_GRAYSCALE,

        /// <summary>
        /// RGB
        /// </summary>
        JCS_RGB,

        /// <summary>
        /// YCbCr (also known as YUV)
        /// </summary>
        JCS_YCbCr,

        /// <summary>
        /// CMYK
        /// </summary>
        JCS_CMYK,

        /// <summary>
        /// YCbCrK
        /// </summary>
        JCS_YCCK
    }

    /// <summary>
    /// Algorithm used for the DCT step.
    /// </summary>
    enum J_DCT_METHOD
    {
        /// <summary>
        /// Slow but accurate integer algorithm.
        /// </summary>
        JDCT_ISLOW,

        /// <summary>
        /// Faster, less accurate integer method.
        /// </summary>
        JDCT_IFAST,

        /// <summary>
        /// Floating-point method.
        /// </summary>
        JDCT_FLOAT
    }

    /// <summary>
    /// Dithering options for decompression.
    /// </summary>
    enum J_DITHER_MODE
    {
        /// <summary>
        /// No dithering: fast, very low quality
        /// </summary>
        JDITHER_NONE,

        /// <summary>
        /// Ordered dither: moderate speed and quality
        /// </summary>
        JDITHER_ORDERED,

        /// <summary>
        /// Floyd-Steinberg dither: slow, high quality
        /// </summary>
        JDITHER_FS
    }

    /// <summary>
    /// Message codes used in code to signal errors, warning and trace messages.
    /// </summary>
    enum J_MESSAGE_CODE
    {
        /// <summary>
        /// Must be first entry!
        /// </summary>
        JMSG_NOMESSAGE,

        /// <summary>
        /// 
        /// </summary>
        JERR_ARITH_NOTIMPL,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_BUFFER_MODE,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_COMPONENT_ID,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_DCT_COEF,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_DCTSIZE,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_HUFF_TABLE,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_IN_COLORSPACE,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_J_COLORSPACE,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_LENGTH,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_MCU_SIZE,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_PRECISION,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_PROGRESSION,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_PROG_SCRIPT,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_SAMPLING,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_SCAN_SCRIPT,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_STATE,
        /// <summary>
        /// 
        /// </summary>
        JERR_BAD_VIRTUAL_ACCESS,
        /// <summary>
        /// 
        /// </summary>
        JERR_BUFFER_SIZE,
        /// <summary>
        /// 
        /// </summary>
        JERR_CANT_SUSPEND,
        /// <summary>
        /// 
        /// </summary>
        JERR_CCIR601_NOTIMPL,
        /// <summary>
        /// 
        /// </summary>
        JERR_COMPONENT_COUNT,
        /// <summary>
        /// 
        /// </summary>
        JERR_CONVERSION_NOTIMPL,
        /// <summary>
        /// 
        /// </summary>
        JERR_DHT_INDEX,
        /// <summary>
        /// 
        /// </summary>
        JERR_DQT_INDEX,
        /// <summary>
        /// 
        /// </summary>
        JERR_EMPTY_IMAGE,
        /// <summary>
        /// 
        /// </summary>
        JERR_EOI_EXPECTED,
        /// <summary>
        /// 
        /// </summary>
        JERR_FILE_WRITE,
        /// <summary>
        /// 
        /// </summary>
        JERR_FRACT_SAMPLE_NOTIMPL,
        /// <summary>
        /// 
        /// </summary>
        JERR_HUFF_CLEN_OVERFLOW,
        /// <summary>
        /// 
        /// </summary>
        JERR_HUFF_MISSING_CODE,
        /// <summary>
        /// 
        /// </summary>
        JERR_IMAGE_TOO_BIG,
        /// <summary>
        /// 
        /// </summary>
        JERR_INPUT_EMPTY,
        /// <summary>
        /// 
        /// </summary>
        JERR_INPUT_EOF,
        /// <summary>
        /// 
        /// </summary>
        JERR_MISMATCHED_QUANT_TABLE,
        /// <summary>
        /// 
        /// </summary>
        JERR_MISSING_DATA,
        /// <summary>
        /// 
        /// </summary>
        JERR_MODE_CHANGE,
        /// <summary>
        /// 
        /// </summary>
        JERR_NOTIMPL,
        /// <summary>
        /// 
        /// </summary>
        JERR_NOT_COMPILED,
        /// <summary>
        /// 
        /// </summary>
        JERR_NO_HUFF_TABLE,
        /// <summary>
        /// 
        /// </summary>
        JERR_NO_IMAGE,
        /// <summary>
        /// 
        /// </summary>
        JERR_NO_QUANT_TABLE,
        /// <summary>
        /// 
        /// </summary>
        JERR_NO_SOI,
        /// <summary>
        /// 
        /// </summary>
        JERR_OUT_OF_MEMORY,
        /// <summary>
        /// 
        /// </summary>
        JERR_QUANT_COMPONENTS,
        /// <summary>
        /// 
        /// </summary>
        JERR_QUANT_FEW_COLORS,
        /// <summary>
        /// 
        /// </summary>
        JERR_QUANT_MANY_COLORS,
        /// <summary>
        /// 
        /// </summary>
        JERR_SOF_DUPLICATE,
        /// <summary>
        /// 
        /// </summary>
        JERR_SOF_NO_SOS,
        /// <summary>
        /// 
        /// </summary>
        JERR_SOF_UNSUPPORTED,
        /// <summary>
        /// 
        /// </summary>
        JERR_SOI_DUPLICATE,
        /// <summary>
        /// 
        /// </summary>
        JERR_SOS_NO_SOF,
        /// <summary>
        /// 
        /// </summary>
        JERR_TOO_LITTLE_DATA,
        /// <summary>
        /// 
        /// </summary>
        JERR_UNKNOWN_MARKER,
        /// <summary>
        /// 
        /// </summary>
        JERR_WIDTH_OVERFLOW,
        /// <summary>
        /// 
        /// </summary>
        JTRC_16BIT_TABLES,
        /// <summary>
        /// 
        /// </summary>
        JTRC_ADOBE,
        /// <summary>
        /// 
        /// </summary>
        JTRC_APP0,
        /// <summary>
        /// 
        /// </summary>
        JTRC_APP14,
        /// <summary>
        /// 
        /// </summary>
        JTRC_DHT,
        /// <summary>
        /// 
        /// </summary>
        JTRC_DQT,
        /// <summary>
        /// 
        /// </summary>
        JTRC_DRI,
        /// <summary>
        /// 
        /// </summary>
        JTRC_EOI,
        /// <summary>
        /// 
        /// </summary>
        JTRC_HUFFBITS,
        /// <summary>
        /// 
        /// </summary>
        JTRC_JFIF,
        /// <summary>
        /// 
        /// </summary>
        JTRC_JFIF_BADTHUMBNAILSIZE,
        /// <summary>
        /// 
        /// </summary>
        JTRC_JFIF_EXTENSION,
        /// <summary>
        /// 
        /// </summary>
        JTRC_JFIF_THUMBNAIL,
        /// <summary>
        /// 
        /// </summary>
        JTRC_MISC_MARKER,
        /// <summary>
        /// 
        /// </summary>
        JTRC_PARMLESS_MARKER,
        /// <summary>
        /// 
        /// </summary>
        JTRC_QUANTVALS,
        /// <summary>
        /// 
        /// </summary>
        JTRC_QUANT_3_NCOLORS,
        /// <summary>
        /// 
        /// </summary>
        JTRC_QUANT_NCOLORS,
        /// <summary>
        /// 
        /// </summary>
        JTRC_QUANT_SELECTED,
        /// <summary>
        /// 
        /// </summary>
        JTRC_RECOVERY_ACTION,
        /// <summary>
        /// 
        /// </summary>
        JTRC_RST,
        /// <summary>
        /// 
        /// </summary>
        JTRC_SMOOTH_NOTIMPL,
        /// <summary>
        /// 
        /// </summary>
        JTRC_SOF,
        /// <summary>
        /// 
        /// </summary>
        JTRC_SOF_COMPONENT,
        /// <summary>
        /// 
        /// </summary>
        JTRC_SOI,
        /// <summary>
        /// 
        /// </summary>
        JTRC_SOS,
        /// <summary>
        /// 
        /// </summary>
        JTRC_SOS_COMPONENT,
        /// <summary>
        /// 
        /// </summary>
        JTRC_SOS_PARAMS,
        /// <summary>
        /// 
        /// </summary>
        JTRC_THUMB_JPEG,
        /// <summary>
        /// 
        /// </summary>
        JTRC_THUMB_PALETTE,
        /// <summary>
        /// 
        /// </summary>
        JTRC_THUMB_RGB,
        /// <summary>
        /// 
        /// </summary>
        JTRC_UNKNOWN_IDS,
        /// <summary>
        /// 
        /// </summary>
        JWRN_ADOBE_XFORM,
        /// <summary>
        /// 
        /// </summary>
        JWRN_BOGUS_PROGRESSION,
        /// <summary>
        /// 
        /// </summary>
        JWRN_EXTRANEOUS_DATA,
        /// <summary>
        /// 
        /// </summary>
        JWRN_HIT_MARKER,
        /// <summary>
        /// 
        /// </summary>
        JWRN_HUFF_BAD_CODE,
        /// <summary>
        /// 
        /// </summary>
        JWRN_JFIF_MAJOR,
        /// <summary>
        /// 
        /// </summary>
        JWRN_JPEG_EOF,
        /// <summary>
        /// 
        /// </summary>
        JWRN_MUST_RESYNC,
        /// <summary>
        /// 
        /// </summary>
        JWRN_NOT_SEQUENTIAL,
        /// <summary>
        /// 
        /// </summary>
        JWRN_TOO_MUCH_DATA,
        /// <summary>
        /// 
        /// </summary>
        JMSG_UNKNOWNMSGCODE,
        /// <summary>
        /// 
        /// </summary>
        JMSG_LASTMSGCODE
    }

    /// <summary>
    /// Describes a result of read operation.
    /// </summary>
    enum ReadResult
    {
        /// <summary>
        /// Suspended due to lack of input data. Can occur only if a suspending data source is used.
        /// </summary>
        JPEG_SUSPENDED = 0,
        /// <summary>
        /// Found valid image datastream.
        /// </summary>
        JPEG_HEADER_OK = 1,
        /// <summary>
        /// Found valid table-specs-only datastream.
        /// </summary>
        JPEG_HEADER_TABLES_ONLY = 2,
        /// <summary>
        /// Reached a SOS marker (the start of a new scan)
        /// </summary>
        JPEG_REACHED_SOS = 3,
        /// <summary>
        /// Reached the EOI marker (end of image)
        /// </summary>
        JPEG_REACHED_EOI = 4,
        /// <summary>
        /// Completed reading one MCU row of compressed data.
        /// </summary>
        JPEG_ROW_COMPLETED = 5,
        /// <summary>
        /// Completed reading last MCU row of current scan.
        /// </summary>
        JPEG_SCAN_COMPLETED = 6
    }

    /// <summary>
    /// JPEG marker codes.
    /// </summary>
    enum JPEG_MARKER
    {
        /// <summary>
        /// 
        /// </summary>
        SOF0 = 0xc0,
        /// <summary>
        /// 
        /// </summary>
        SOF1 = 0xc1,
        /// <summary>
        /// 
        /// </summary>
        SOF2 = 0xc2,
        /// <summary>
        /// 
        /// </summary>
        SOF3 = 0xc3,
        /// <summary>
        /// 
        /// </summary>
        SOF5 = 0xc5,
        /// <summary>
        /// 
        /// </summary>
        SOF6 = 0xc6,
        /// <summary>
        /// 
        /// </summary>
        SOF7 = 0xc7,
        /// <summary>
        /// 
        /// </summary>
        JPG = 0xc8,
        /// <summary>
        /// 
        /// </summary>
        SOF9 = 0xc9,
        /// <summary>
        /// 
        /// </summary>
        SOF10 = 0xca,
        /// <summary>
        /// 
        /// </summary>
        SOF11 = 0xcb,
        /// <summary>
        /// 
        /// </summary>
        SOF13 = 0xcd,
        /// <summary>
        /// 
        /// </summary>
        SOF14 = 0xce,
        /// <summary>
        /// 
        /// </summary>
        SOF15 = 0xcf,
        /// <summary>
        /// 
        /// </summary>
        DHT = 0xc4,
        /// <summary>
        /// 
        /// </summary>
        DAC = 0xcc,
        /// <summary>
        /// 
        /// </summary>
        RST0 = 0xd0,
        /// <summary>
        /// 
        /// </summary>
        RST1 = 0xd1,
        /// <summary>
        /// 
        /// </summary>
        RST2 = 0xd2,
        /// <summary>
        /// 
        /// </summary>
        RST3 = 0xd3,
        /// <summary>
        /// 
        /// </summary>
        RST4 = 0xd4,
        /// <summary>
        /// 
        /// </summary>
        RST5 = 0xd5,
        /// <summary>
        /// 
        /// </summary>
        RST6 = 0xd6,
        /// <summary>
        /// 
        /// </summary>
        RST7 = 0xd7,
        /// <summary>
        /// 
        /// </summary>
        SOI = 0xd8,
        /// <summary>
        /// 
        /// </summary>
        EOI = 0xd9,
        /// <summary>
        /// 
        /// </summary>
        SOS = 0xda,
        /// <summary>
        /// 
        /// </summary>
        DQT = 0xdb,
        /// <summary>
        /// 
        /// </summary>
        DNL = 0xdc,
        /// <summary>
        /// 
        /// </summary>
        DRI = 0xdd,
        /// <summary>
        /// 
        /// </summary>
        DHP = 0xde,
        /// <summary>
        /// 
        /// </summary>
        EXP = 0xdf,
        /// <summary>
        /// 
        /// </summary>
        APP0 = 0xe0,
        /// <summary>
        /// 
        /// </summary>
        APP1 = 0xe1,
        /// <summary>
        /// 
        /// </summary>
        APP2 = 0xe2,
        /// <summary>
        /// 
        /// </summary>
        APP3 = 0xe3,
        /// <summary>
        /// 
        /// </summary>
        APP4 = 0xe4,
        /// <summary>
        /// 
        /// </summary>
        APP5 = 0xe5,
        /// <summary>
        /// 
        /// </summary>
        APP6 = 0xe6,
        /// <summary>
        /// 
        /// </summary>
        APP7 = 0xe7,
        /// <summary>
        /// 
        /// </summary>
        APP8 = 0xe8,
        /// <summary>
        /// 
        /// </summary>
        APP9 = 0xe9,
        /// <summary>
        /// 
        /// </summary>
        APP10 = 0xea,
        /// <summary>
        /// 
        /// </summary>
        APP11 = 0xeb,
        /// <summary>
        /// 
        /// </summary>
        APP12 = 0xec,
        /// <summary>
        /// 
        /// </summary>
        APP13 = 0xed,
        /// <summary>
        /// 
        /// </summary>
        APP14 = 0xee,
        /// <summary>
        /// 
        /// </summary>
        APP15 = 0xef,
        /// <summary>
        /// 
        /// </summary>
        JPG0 = 0xf0,
        /// <summary>
        /// 
        /// </summary>
        JPG13 = 0xfd,
        /// <summary>
        /// 
        /// </summary>
        COM = 0xfe,
        /// <summary>
        /// 
        /// </summary>
        TEM = 0x01,
        /// <summary>
        /// 
        /// </summary>
        ERROR = 0x100
    }
}

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    /// <summary>
    /// Operating modes for buffer controllers
    /// </summary>
    enum J_BUF_MODE
    {
        JBUF_PASS_THRU,
        JBUF_SAVE_SOURCE,
        JBUF_CRANK_DEST,
        JBUF_SAVE_AND_PASS
    }

    [Flags]
    enum TiffFlags
    {
        /// <summary>
        /// Use MSB2LSB (most significant -> least) fill order
        /// </summary>
        MSB2LSB = 1,

        /// <summary>
        /// Use LSB2MSB (least significant -> most) fill order
        /// </summary>
        LSB2MSB = 2,

        /// <summary>
        /// natural bit fill order for machine
        /// </summary>
        FILLORDER = 0x0003,

        /// <summary>
        /// current directory must be written
        /// </summary>
        DIRTYDIRECT = 0x0008,

        /// <summary>
        /// data buffers setup
        /// </summary>
        BUFFERSETUP = 0x0010,

        /// <summary>
        /// encoder/decoder setup done
        /// </summary>
        CODERSETUP = 0x0020,

        /// <summary>
        /// written 1+ scanlines to file
        /// </summary>
        BEENWRITING = 0x0040,

        /// <summary>
        /// byte swap file information
        /// </summary>
        SWAB = 0x0080,

        /// <summary>
        /// inhibit bit reversal logic
        /// </summary>
        NOBITREV = 0x0100,

        /// <summary>
        /// my raw data buffer; free on close
        /// </summary>
        MYBUFFER = 0x0200,

        /// <summary>
        /// file is tile, not strip- based
        /// </summary>
        ISTILED = 0x0400,

        /// <summary>
        /// need call to postencode routine
        /// </summary>
        POSTENCODE = 0x1000,

        /// <summary>
        /// currently writing a subifd
        /// </summary>
        INSUBIFD = 0x2000,

        /// <summary>
        /// library is doing data up-sampling
        /// </summary>
        UPSAMPLED = 0x4000,

        /// <summary>
        /// enable strip chopping support
        /// </summary>
        STRIPCHOP = 0x8000,

        /// <summary>
        /// read header only, do not process the first directory
        /// </summary>
        HEADERONLY = 0x10000,

        /// <summary>
        /// skip reading of raw uncompressed image data
        /// </summary>
        NOREADRAW = 0x20000,
    }
}