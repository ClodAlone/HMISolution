#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Escher
{
    /// <summary>
    /// Summary description for MSOBlipType.
    /// </summary>
    internal enum MSOBI
    {
        /// <summary>
        /// Unknow
        /// </summary>
        msobiUNKNOWN = 0,
        /// <summary>
        /// Metafile header then compressed WMF
        /// </summary>
        msobiWMF = 0x216,
        /// <summary>
        /// Metafile header then compressed EMF
        /// </summary>
        msobiEMF = 0x3D4,
        /// <summary>
        /// Metafile header then compressed PICT
        /// </summary>
        msobiPICT = 0x542,
        /// <summary>
        /// One byte tag then PNG data
        /// </summary>
        msobiPNG = 0x6E0,
        /// <summary>
        /// One byte tag then JFIF data
        /// </summary>
        msobiJFIF = 0x46A,
        /// <summary>
        /// One byte tag then JPEG data
        /// </summary>
        msobiJPEG = msobiJFIF,
        /// <summary>
        /// One byte tag then DIB data
        /// </summary>
        msobiDIB = 0x7A8,
        /// <summary>
        /// Clients should set this bit
        /// </summary>
        msobiClient = 0x800
    }

    /// <summary>
    /// GEL provided types...
    /// </summary>
    internal enum MSOBlipType
    {
        /// <summary>
        /// An error occured during loading
        /// </summary>
        msoblipERROR = 0,
        /// <summary>
        /// An unknown blip type
        /// </summary>
        msoblipUNKNOWN,
        /// <summary>
        /// Windows Enhanced Metafile
        /// </summary>
        msoblipEMF,
        /// <summary>
        /// Windows Metafile
        /// </summary>
        msoblipWMF,
        /// <summary>
        /// Macintosh PICT
        /// </summary>
        msoblipPICT,
        /// <summary>
        /// JFIF
        /// </summary>
        msoblipJPEG,
        /// <summary>
        /// PNG
        /// </summary>
        msoblipPNG,
        /// <summary>
        /// Windows DIB
        /// </summary>
        msoblipDIB,
        /// <summary>
        /// First client defined blip type
        /// </summary>
        msoblipFirstClient = 32,
        /// <summary>
        /// Last client defined blip type
        /// </summary>
        msoblipLastClient = 255
    }

    /// <summary>
    /// Blip usage
    /// </summary>
    internal enum MSOBlipUsage
    {
        /// <summary>
        /// All non-texture fill blips get this.
        /// </summary>
        msoblipUsageDefault,
        /// <summary>
        /// 
        /// </summary>
        msoblipUsageTexture,
        /// <summary>
        /// Since this is stored in a byte
        /// </summary>
        msoblipUsageMax = 255
    }

    /// <summary>
    /// Blip Foptes
    /// </summary>
    internal enum BlipFopte
    {
        /// <summary>
        /// Blip to display
        /// </summary>
        pib = 260,
        /// <summary>
        /// Blip file name
        /// </summary>
        pibName = 261,
        /// <summary>
        /// Blip flags
        /// </summary>
        pibFlags = 262,
        /// <summary>
        /// transparent color (none if ~0UL) 
        /// </summary>
        pictureTransparent = 263,
        /// <summary>
        /// Draw a dashed line if no line
        /// </summary>
        NoLineDrawDash = 511
    }

    /// <summary>
    /// 
    /// </summary>
    internal enum MSOFBT
    {
        msofbtDggContainer = 0xf000,
        msofbtBstoreContainer = 0xf001,
        msofbtDgContainer = 0xf002,
        msofbtSpgrContainer = 0xf003,
        msofbtSpContainer = 0xf004,
        msofbtSolverContainer = 0xf005,
        msofbtDgg = 0xf006,
        msofbtBSE = 0xf007,
        msofbtDg = 0xf008,
        msofbtSpgr = 0xf009,
        msofbtSp = 0xf00a,
        msofbtOPT = 0xf00b,
        msofbtTextbox = 0xF00C,
        msofbtClientTextbox = 0xf00d,
        msofbtAnchor = 0xF00E,
        msofbtChildAnchor = 0xF00F,
        msofbtClientAnchor = 0xf010,
        msofbtClientData = 0xf011,
        msofbtConnectorRule = 0xF012,
        msofbtAlignRule = 0xF013,
        msofbtArcRule = 0xF014,
        msofbtClientRule = 0xF015,
        msofbtCLSID = 0xF016,
        msofbtCalloutRule = 0xF017,
        msofbtBlipFirst = 0xf018,
        msofbtBlipEMF = 0xf01a,
        msofbtBlipWMF = 0xf01b,
        msofbtBlipJPEG = 0xf01d,
        msofbtBlipPNG = 0xf01e,
        msofbtBlipDIB = 0xf01f,
        msofbtREGROUPItems = 0xF118,
        msofbtSelection = 0xF119,
        msofbtColorMRU = 0xF11A,
        msofbtDeletedPspl = 0xF11D,
        msofbtSplitMenuColors = 0xF11E,
        msofbtOleObject = 0xF11F,
        msofbtColorScheme = 0xF120,
        msofbtSecondaryFOPT = 0xF121,
        msofbtTertiaryFOPT = 0xF122
    };

    /// <summary>
    /// 
    /// </summary>
    internal enum CompressionMethod
    {
        msocompressionZip = 0,
        msocompressionNone = 254
    };
}

