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

#region file using directives
using System;
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for ArrayOfShorts.
    /// </summary>
    //	[ StructLayout( LayoutKind.Sequential ) ]
    [CLSCompliant(false)]
    internal class ArrayOfFCLCB : BaseArrayOfLongs
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MEMBER_SIZE = 4;

        /// <summary>
        /// Number of known members.
        /// </summary>
        private const int DEF_KNOWN_MEMBERS = 186;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_FCLCB_NUMBER = 173;
        // TODO: For support of versions correctly we have to add this constants
        // but for the moment only members will be enough.
        //    private const int DEF_OFFSET_fcStshfOrig    = 0;
        //    private const int DEF_OFFSET_lcbStshfOrig   = 1;
        //    private const int DEF_OFFSET_fcStshf        = 2;
        //    private const int DEF_OFFSET_lcbStshf       = 3;
        //    private const int DEF_OFFSET_fcPlcffndRef   = 4;
        //    private const int DEF_OFFSET_lcbPlcffndRef  = 5;
        //    private const int DEF_OFFSET_fcPlcffndTxt   = 6;
        //    private const int DEF_OFFSET_lcbPlcffndTxt  = 7;
        //    private const int DEF_OFFSET_fcPlcfandRef   = 8;
        //    private const int DEF_OFFSET_lcbPlcfandRef  = 9;
        //    private const int DEF_OFFSET_fcPlcfandTxt   = 10;
        //    private const int DEF_OFFSET_lcbPlcfandTxt  = 11;
        //    private const int DEF_OFFSET_fcPlcfsed      = 12;
        //    private const int DEF_OFFSET_lcbPlcfsed     = 13;
        //    private const int DEF_OFFSET_fcPlcpad       = 14;
        //    private const int DEF_OFFSET_lcbPlcpad      = 15;
        //    private const int DEF_OFFSET_fcPlcfphe      = 16;
        //    private const int DEF_OFFSET_lcbPlcfphe     = 17;
        //    private const int DEF_OFFSET_fcSttbfglsy    = 18;
        //    private const int DEF_OFFSET_lcbSttbfglsy   = 19;
        //    private const int DEF_OFFSET_fcPlcfglsy     = 20;
        //    private const int DEF_OFFSET_lcbPlcfglsy    = 21;
        //
        //    private const int DEF_OFFSET_fcPlcfhdd  = ;
        //    private const int DEF_OFFSET_lcbPlcfhdd  = ;
        //    private const int DEF_OFFSET_fcPlcfbteChpx  = ;
        //    private const int DEF_OFFSET_lcbPlcfbteChpx  = ;
        //    private const int DEF_OFFSET_fcPlcfbtePapx  = ;
        //    private const int DEF_OFFSET_lcbPlcfbtePapx  = ;
        //    private const int DEF_OFFSET_fcPlcfsea  = ;
        //    private const int DEF_OFFSET_lcbPlcfsea  = ;
        //    private const int DEF_OFFSET_fcSttbfffn  = ;
        //    private const int DEF_OFFSET_lcbSttbfffn  = ;
        //    private const int DEF_OFFSET_fcPlcffldMom  = ;
        //    private const int DEF_OFFSET_lcbPlcffldMom  = ;
        //    private const int DEF_OFFSET_fcPlcffldHdr = ;
        //    private const int DEF_OFFSET_lcbPlcffldHdr  = ;
        //    private const int DEF_OFFSET_fcPlcffldFtn  = ;
        //    private const int DEF_OFFSET_lcbPlcffldFtn  = ;
        //    private const int DEF_OFFSET_fcPlcffldAtn  = ;
        //    private const int DEF_OFFSET_lcbPlcffldAtn  = ;
        //    private const int DEF_OFFSET_fcPlcffldMcr  = ;
        //    private const int DEF_OFFSET_lcbPlcffldMcr  = ;
        //    private const int DEF_OFFSET_fcSttbfbkmk  = ;
        //    private const int DEF_OFFSET_lcbSttbfbkmk  = ;
        //    private const int DEF_OFFSET_fcPlcfbkf  = ;
        //    private const int DEF_OFFSET_lcbPlcfbkf = ;
        //    private const int DEF_OFFSET_fcPlcfbkl  = ;
        //    private const int DEF_OFFSET_lcbPlcfbkl  = ;
        //    private const int DEF_OFFSET_fcCmds  = ;
        //    private const int DEF_OFFSET_lcbCmds  = ;
        //    private const int DEF_OFFSET_fcPlcmcr  = ;
        //    private const int DEF_OFFSET_lcbPlcmcr  = ;
        //    private const int DEF_OFFSET_fcSttbfmcr  = ;
        //    private const int DEF_OFFSET_lcbSttbfmcr  = ;
        //    private const int DEF_OFFSET_fcPrDrvr  = ;
        //    private const int DEF_OFFSET_lcbPrDrvr  = ;
        //    private const int DEF_OFFSET_fcPrEnvPort  = ;
        //    private const int DEF_OFFSET_lcbPrEnvPort  = ;
        //    private const int DEF_OFFSET_fcPrEnvLand  = ;
        //    private const int DEF_OFFSET_lcbPrEnvLand  = ;
        //    private const int DEF_OFFSET_fcWss  = ;
        //    private const int DEF_OFFSET_lcbWss  = ;
        //    private const int DEF_OFFSET_fcDop  = ;
        //    private const int DEF_OFFSET_lcbDop  = ;
        //    private const int DEF_OFFSET_fcSttbfAssoc  = ;
        //    private const int DEF_OFFSET_lcbSttbfAssoc  = ;
        //    private const int DEF_OFFSET_fcClx  = ;
        //    private const int DEF_OFFSET_lcbClx  = ;
        //    private const int DEF_OFFSET_fcPlcfpgdFtn  = ;
        //    private const int DEF_OFFSET_lcbPlcfpgdFtn  = ;
        //    private const int DEF_OFFSET_fcAutosaveSource  = ;
        //    private const int DEF_OFFSET_lcbAutosaveSource  = ;
        //    private const int DEF_OFFSET_fcGrpXstAtnOwners  = ;
        //    private const int DEF_OFFSET_lcbGrpXstAtnOwners  = ;
        //    private const int DEF_OFFSET_fcSttbfAtnbkmk  = ;
        //    private const int DEF_OFFSET_lcbSttbfAtnbkmk  = ;
        //    private const int DEF_OFFSET_fcPlcdoaMom  = ;
        //    private const int DEF_OFFSET_lcbPlcdoaMom  = ;
        //    private const int DEF_OFFSET_fcPlcdoaHdr  = ;
        //    private const int DEF_OFFSET_lcbPlcdoaHdr  = ;
        //    private const int DEF_OFFSET_fcPlcspaMom  = ;
        //    private const int DEF_OFFSET_lcbPlcspaMom  = ;
        //    private const int DEF_OFFSET_fcPlcspaHdr  = ;
        //    private const int DEF_OFFSET_lcbPlcspaHdr  = ;
        //    private const int DEF_OFFSET_fcPlcfAtnbkf  = ;
        //    private const int DEF_OFFSET_lcbPlcfAtnbkf  = ;
        //    private const int DEF_OFFSET_fcPlcfAtnbkl  = ;
        //    private const int DEF_OFFSET_lcbPlcfAtnbkl  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        //    private const int DEF_OFFSET_  = ;
        #endregion

        #region Internal Classes
        /// <summary>
        /// 
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [StructLayout(LayoutKind.Sequential)]
#endif
        internal class KnownPart : DataStructure
        {
            #region Class members
            //    /// <summary>
            //    /// Beginning of array of FC/LCB pairs.
            //    /// </summary>
            //    //[ FieldOffset( 154 ) ]
            //    public  rgfclcb;
            /// <summary>
            /// file offset of original allocation for STSH in table stream. During fast save Word will attempt to reuse this allocation if STSH is small enough to fit.
            /// </summary>
            //[ FieldOffset( 154 ) ]
            public int fcStshfOrig;
            /// <summary>
            /// count of bytes of original STSH allocation
            /// </summary>
            //[ FieldOffset( 158 ) ]
            public uint lcbStshfOrig;
            /// <summary>
            /// offset of STSH in table stream.
            /// </summary>
            //[ FieldOffset( 162 ) ]
            public int fcStshf;
            /// <summary>
            /// count of bytes of current STSH allocation
            /// </summary>
            //[ FieldOffset( 166 ) ]
            public uint lcbStshf;
            /// <summary>
            /// offset in table stream of footnote reference PLCF of FRD structures. CPs in PLC are relative to main document text stream and give location of footnote references.
            /// </summary>
            //[ FieldOffset( 170 ) ]
            public int fcPlcffndRef;
            /// <summary>
            /// count of bytes of footnote reference PLC== 0 if no footnotes defined in document.
            /// </summary>
            //[ FieldOffset( 174 ) ]
            public uint lcbPlcffndRef;
            /// <summary>
            /// offset in table stream of footnote text PLC. CPs in PLC are relative to footnote subdocument text stream and give location of beginnings of footnote text for corresponding references recorded in plcffndRef. No structure is stored in this plc. There will just be n+1 FC entries in this PLC when there are n footnotes
            /// </summary>
            //[ FieldOffset( 178 ) ]
            public int fcPlcffndTxt;
            /// <summary>
            /// count of bytes of footnote text PLC. == 0 if no footnotes defined in document
            /// </summary>
            //[ FieldOffset( 182 ) ]
            public uint lcbPlcffndTxt;
            /// <summary>
            /// offset in table stream of annotation reference ATRD PLC. The CPs recorded in this PLC give the offset of annotation references in the main document.
            /// </summary>
            //[ FieldOffset( 186 ) ]
            public int fcPlcfandRef;
            /// <summary>
            /// count of bytes of annotation reference PLC.
            /// </summary>
            //[ FieldOffset( 190 ) ]
            public uint lcbPlcfandRef;
            /// <summary>
            /// offset in table stream of annotation text PLC. The Cps recorded in this PLC give the offset of the annotation text in the annotation sub document corresponding to the references stored in the plcfandRef. There is a 1 to 1 correspondence between entries recorded in the plcfandTxt and the plcfandRef. No structure is stored in this PLC.
            /// </summary>
            //[ FieldOffset( 194 ) ]
            public int fcPlcfandTxt;
            /// <summary>
            /// count of bytes of the annotation text PLC
            /// </summary>
            //[ FieldOffset( 198 ) ]
            public uint lcbPlcfandTxt;
            /// <summary>
            /// offset in table stream of section descriptor SED PLC. CPs in PLC are relative to main document.
            /// </summary>
            //[ FieldOffset( 202 ) ]
            public int fcPlcfsed;
            /// <summary>
            /// count of bytes of section descriptor PLC.
            /// </summary>
            //[ FieldOffset( 206 ) ]
            public uint lcbPlcfsed;
            /// <summary>
            /// no longer used
            /// </summary>
            //[ FieldOffset( 210 ) ]
            public int fcPlcpad;
            /// <summary>
            /// no longer used
            /// </summary>
            //[ FieldOffset( 214 ) ]
            public uint lcbPlcpad;
            /// <summary>
            /// offset in table stream of PHE PLC of paragraph heights. CPs in PLC are relative to main document text stream. Only written for files in complex format. Should not be written by third party creators of Word files.
            /// </summary>
            //[ FieldOffset( 218 ) ]
            public int fcPlcfphe;
            /// <summary>
            /// count of bytes of paragraph height PLC. ==0 when file is non-complex.
            /// </summary>
            //[ FieldOffset( 222 ) ]
            public uint lcbPlcfphe;
            /// <summary>
            /// offset in table stream of glossary string table. This table consists of Pascal style strings (strings stored prefixed with a length byte) concatenated one after another.
            /// </summary>
            //[ FieldOffset( 226 ) ]
            public int fcSttbfglsy;
            /// <summary>
            /// count of bytes of glossary string table. == 0 for non-glossary documents.!=0 for glossary documents.
            /// </summary>
            //[ FieldOffset( 230 ) ]
            public uint lcbSttbfglsy;
            /// <summary>
            /// offset in table stream of glossary PLC. CPs in PLC are relative to main document and mark the beginnings of glossary entries and are in 1-1 correspondence with entries of sttbfglsy. No structure is stored in this PLC. There will be n+1 FC entries in this PLC when there are n glossary entries.
            /// </summary>
            //[ FieldOffset( 234 ) ]
            public int fcPlcfglsy;
            /// <summary>
            /// count of bytes of glossary PLC.== 0 for non-glossary documents.!=0 for glossary documents.
            /// </summary>
            //[ FieldOffset( 238 ) ]
            public uint lcbPlcfglsy;
            /// <summary>
            /// byte offset in table stream of header HDD PLC. CPs are relative to header subdocument and mark the beginnings of individual headers in the header subdocument. No structure is stored in this PLC. There will be n+1 FC entries in this PLC when there are n headers stored for the document.
            /// </summary>
            //[ FieldOffset( 242 ) ]
            public int fcPlcfhdd;
            /// <summary>
            /// count of bytes of header PLC.
            /// </summary>
            //[ FieldOffset( 246 ) ]
            public uint lcbPlcfhdd;
            /// <summary>
            /// offset in table stream of character property bin table.PLC. FCs in PLC are file offsets in the main stream. Describes text of main document and all subdocuments.
            /// </summary>
            //[ FieldOffset( 250 ) ]
            public int fcPlcfbteChpx;
            /// <summary>
            /// count of bytes of character property bin table PLC.
            /// </summary>
            //[ FieldOffset( 254 ) ]
            public uint lcbPlcfbteChpx;
            /// <summary>
            /// offset in table stream of paragraph property bin table.PLC. FCs in PLC are file offsets in the main stream. Describes text of main document and all subdocuments.
            /// </summary>
            //[ FieldOffset( 258 ) ]
            public int fcPlcfbtePapx;
            /// <summary>
            /// count of bytes of paragraph property bin table PLC
            /// </summary>
            //[ FieldOffset( 262 ) ]
            public uint lcbPlcfbtePapx;
            /// <summary>
            /// offset in table stream of PLC reserved for private use. The SEA is 6 bytes long.
            /// </summary>
            //[ FieldOffset( 266 ) ]
            public int fcPlcfsea;
            /// <summary>
            /// count of bytes of private use PLC.
            /// </summary>
            //[ FieldOffset( 270 ) ]
            public uint lcbPlcfsea;
            /// <summary>
            /// offset in table stream of font information STTBF. The sttbfffn is a STTBF where is string is actually an FFN structure. The nth entry in the STTBF describes the font that will be displayed when the chp.ftc for text is equal to n. See the FFN file structure definition.
            /// </summary>
            //[ FieldOffset( 274 ) ]
            public int fcSttbfffn;
            /// <summary>
            /// count of bytes in sttbfffn.
            /// </summary>
            //[ FieldOffset( 278 ) ]
            public uint lcbSttbfffn;
            /// <summary>
            /// offset in table stream to the FLD PLC of field positions in the main document. The CPs point to the beginning CP of a field, the CP of field separator character inside a field and the ending CP of the field. A field may be nested within another field. 20 levels of field nesting are allowed.
            /// </summary>
            //[ FieldOffset( 282 ) ]
            public int fcPlcffldMom;
            /// <summary>
            /// count of bytes in plcffldMom
            /// </summary>
            //[ FieldOffset( 286 ) ]
            public uint lcbPlcffldMom;
            /// <summary>
            /// offset in table stream to the FLD PLC of field positions in the header subdocument.
            /// </summary>
            //[ FieldOffset( 290 ) ]
            public int fcPlcffldHdr;
            /// <summary>
            /// count of bytes in plcffldHdr
            /// </summary>
            //[ FieldOffset( 294 ) ]
            public uint lcbPlcffldHdr;
            /// <summary>
            /// offset in table stream to the FLD PLC of field positions in the footnote subdocument.
            /// </summary>
            //[ FieldOffset( 298 ) ]
            public int fcPlcffldFtn;
            /// <summary>
            /// count of bytes in plcffldFtn
            /// </summary>
            //[ FieldOffset( 302 ) ]
            public uint lcbPlcffldFtn;
            /// <summary>
            /// offset in table stream to the FLD PLC of field positions in the annotation subdocument.
            /// </summary>
            //[ FieldOffset( 306 ) ]
            public int fcPlcffldAtn;
            /// <summary>
            /// count of bytes in plcffldAtn
            /// </summary>
            //[ FieldOffset( 310 ) ]
            public uint lcbPlcffldAtn;
            /// <summary>
            /// no longer used
            /// </summary>
            //[ FieldOffset( 314 ) ]
            public int fcPlcffldMcr;
            /// <summary>
            /// no longer used
            /// </summary>
            //[ FieldOffset( 318 ) ]
            public uint lcbPlcffldMcr;
            /// <summary>
            /// offset in table stream of the STTBF that records bookmark names in the main document
            /// </summary>
            //[ FieldOffset( 322 ) ]
            public int fcSttbfbkmk;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 326 ) ]
            public uint lcbSttbfbkmk;
            /// <summary>
            /// offset in table stream of the PLCF that records the beginning CP offsets of bookmarks in the main document. See BKF structure definition
            /// </summary>
            //[ FieldOffset( 330 ) ]
            public int fcPlcfbkf;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 334 ) ]
            public uint lcbPlcfbkf;
            /// <summary>
            /// offset in table stream of the PLCF that records the ending CP offsets of bookmarks recorded in the main document. No structure is stored in this PLCF.
            /// </summary>
            //[ FieldOffset( 338 ) ]
            public int fcPlcfbkl;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 342 ) ]
            public uint lcbPlcfbkl;
            /// <summary>
            /// offset in table stream of the macro commands. These commands are private and undocumented.
            /// </summary>
            //[ FieldOffset( 346 ) ]
            public int fcCmds;
            /// <summary>
            /// undocument size of undocument structure not documented above
            /// </summary>
            //[ FieldOffset( 350 ) ]
            public uint lcbCmds;
            /// <summary>
            /// no longer used
            /// </summary>
            //[ FieldOffset( 354 ) ]
            public int fcPlcmcr;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 358 ) ]
            public uint lcbPlcmcr;
            /// <summary>
            /// no longer used
            /// </summary>
            //[ FieldOffset( 362 ) ]
            public int fcSttbfmcr;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 366 ) ]
            public uint lcbSttbfmcr;
            /// <summary>
            /// offset in table stream of the printer driver information (names of drivers, port, etc.)
            /// </summary>
            //[ FieldOffset( 370 ) ]
            public int fcPrDrvr;
            /// <summary>
            /// count of bytes of the printer driver information (names of drivers, port, etc.)
            /// </summary>
            //[ FieldOffset( 374 ) ]
            public uint lcbPrDrvr;
            /// <summary>
            /// offset in table stream of the print environment in portrait mode.
            /// </summary>
            //[ FieldOffset( 378 ) ]
            public int fcPrEnvPort;
            /// <summary>
            /// count of bytes of the print environment in portrait mode.
            /// </summary>
            //[ FieldOffset( 382 ) ]
            public uint lcbPrEnvPort;
            /// <summary>
            /// offset in table stream of the print environment in landscape mode.
            /// </summary>
            //[ FieldOffset( 386 ) ]
            public int fcPrEnvLand;
            /// <summary>
            /// count of bytes of the print environment in landscape mode.
            /// </summary>
            //[ FieldOffset( 390 ) ]
            public uint lcbPrEnvLand;
            /// <summary>
            /// offset in table stream of Window Save State data structure. WSS contains dimensions of document's main text window and the last selection made by Word user.
            /// </summary>
            //[ FieldOffset( 394 ) ]
            public int fcWss;
            /// <summary>
            /// count of bytes of WSS. ==0 if unable to store the window state. Should not be written by third party creators of Word files.
            /// </summary>
            //[ FieldOffset( 398 ) ]
            public uint lcbWss;
            /// <summary>
            /// offset in table stream of document property data structure.
            /// </summary>
            //[ FieldOffset( 402 ) ]
            public int fcDop;
            /// <summary>
            /// count of bytes of document properties.
            /// </summary>
            //[ FieldOffset( 406 ) ]
            public uint lcbDop;
            /// <summary>
            /// offset in table stream of STTBF of associated strings. The strings in this table specify document summary info and the paths to special documents related to this document. See documentation of the STTBFASSOC.
            /// </summary>
            //[ FieldOffset( 410 ) ]
            public int fcSttbfAssoc;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 414 ) ]
            public uint lcbSttbfAssoc;
            /// <summary>
            /// offset in table stream of beginning of information for complex files. Consists of an encoding of all of the prms quoted by the document followed by the plcpcd (piece table) for the document.
            /// </summary>
            //[ FieldOffset( 418 ) ]
            public int fcClx;
            /// <summary>
            /// count of bytes of complex file information == 0 if file is non-complex.
            /// </summary>
            //[ FieldOffset( 422 ) ]
            public uint lcbClx;
            /// <summary>
            /// not used
            /// </summary>
            //[ FieldOffset( 426 ) ]
            public int fcPlcfpgdFtn;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 430 ) ]
            public uint lcbPlcfpgdFtn;
            /// <summary>
            /// offset in table stream of the name of the original file. fcAutosaveSource and cbAutosaveSource should both be 0 if autosave is off.
            /// </summary>
            //[ FieldOffset( 434 ) ]
            public int fcAutosaveSource;
            /// <summary>
            /// count of bytes of the name of the original file.
            /// </summary>
            //[ FieldOffset( 438 ) ]
            public uint lcbAutosaveSource;
            /// <summary>
            /// offset in table stream of group of strings recording the names of the owners of annotations stored in the document
            /// </summary>
            //[ FieldOffset( 442 ) ]
            public int fcGrpXstAtnOwners;
            /// <summary>
            /// count of bytes of the group of strings
            /// </summary>
            //[ FieldOffset( 446 ) ]
            public uint lcbGrpXstAtnOwners;
            /// <summary>
            /// offset in table stream of the sttbf that records names of bookmarks for the annotation subdocument
            /// </summary>
            //[ FieldOffset( 450 ) ]
            public int fcSttbfAtnbkmk;
            /// <summary>
            /// length in bytes of the sttbf that records names of bookmarks for the annotation subdocument
            /// </summary>
            //[ FieldOffset( 454 ) ]
            public uint lcbSttbfAtnbkmk;
            /// <summary>
            /// no longer used
            /// </summary>
            //[ FieldOffset( 458 ) ]
            public int fcPlcdoaMom;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 462 ) ]
            public uint lcbPlcdoaMom;
            /// <summary>
            /// no longer used
            /// </summary>
            //[ FieldOffset( 466 ) ]
            public int fcPlcdoaHdr;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 470 ) ]
            public uint lcbPlcdoaHdr;
            /// <summary>
            /// offset in table stream of the FSPA PLC for main document. == 0 if document has no office art objects.
            /// </summary>
            //[ FieldOffset( 474 ) ]
            public int fcPlcspaMom;
            /// <summary>
            /// length in bytes of the FSPA PLC of the main document.
            /// </summary>
            //[ FieldOffset( 478 ) ]
            public uint lcbPlcspaMom;
            /// <summary>
            /// offset in table stream of the FSPA PLC for header document. == 0 if document has no office art objects.
            /// </summary>
            //[ FieldOffset( 482 ) ]
            public int fcPlcspaHdr;
            /// <summary>
            /// length in bytes of the FSPA PLC of the header document.
            /// </summary>
            //[ FieldOffset( 486 ) ]
            public uint lcbPlcspaHdr;
            /// <summary>
            /// offset in table stream of BKF (bookmark first) PLC of the annotation subdocument
            /// </summary>
            //[ FieldOffset( 490 ) ]
            public int fcPlcfAtnbkf;
            /// <summary>
            /// length in bytes of BKF (bookmark first) PLC of the annotation subdocument
            /// </summary>
            //[ FieldOffset( 494 ) ]
            public uint lcbPlcfAtnbkf;
            /// <summary>
            /// offset in table stream of BKL (bookmark last) PLC of the annotation subdocument
            /// </summary>
            //[ FieldOffset( 498 ) ]
            public int fcPlcfAtnbkl;
            /// <summary>
            /// length in bytes of PLC marking the CP limits of the annotation bookmarks. No structure is stored in this PLC.
            /// </summary>
            //[ FieldOffset( 502 ) ]
            public uint lcbPlcfAtnbkl;
            /// <summary>
            /// offset in table stream of PMS (Print Merge State) information block. This contains the current state of a print merge operation
            /// </summary>
            //[ FieldOffset( 506 ) ]
            public int fcPms;
            /// <summary>
            /// length in bytes of PMS. ==0 if no current print merge state. Should not be written by third party creators of Word files.
            /// </summary>
            //[ FieldOffset( 510 ) ]
            public uint lcbPms;
            /// <summary>
            /// offset in table stream of form field Sttbf which contains strings used in form field dropdown controls
            /// </summary>
            //[ FieldOffset( 514 ) ]
            public int fcFormFldSttbs;
            /// <summary>
            /// length in bytes of form field Sttbf
            /// </summary>
            //[ FieldOffset( 518 ) ]
            public uint lcbFormFldSttbs;
            /// <summary>
            /// offset in table stream of endnote reference PLCF of FRD structures. CPs in PLCF are relative to main document text stream and give location of endnote references.
            /// </summary>
            //[ FieldOffset( 522 ) ]
            public int fcPlcfendRef;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 526 ) ]
            public uint lcbPlcfendRef;
            /// <summary>
            /// offset in table stream of PlcfendRef which points to endnote text in the endnote document stream which corresponds with the plcfendRef. No structure is stored in this PLC.
            /// </summary>
            //[ FieldOffset( 530 ) ]
            public int fcPlcfendTxt;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 534 ) ]
            public uint lcbPlcfendTxt;
            /// <summary>
            /// offset in table stream to FLD PLCF of field positions in the endnote subdoc
            /// </summary>
            //[ FieldOffset( 538 ) ]
            public int fcPlcffldEdn;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 542 ) ]
            public uint lcbPlcffldEdn;
            /// <summary>
            /// not used
            /// </summary>
            //[ FieldOffset( 546 ) ]
            public int fcPlcfpgdEdn;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 550 ) ]
            public uint lcbPlcfpgdEdn;
            /// <summary>
            /// offset in table stream of the office art object table data. The format of office art object table data is found in a separate document.
            /// </summary>
            //[ FieldOffset( 554 ) ]
            public int fcDggInfo;
            /// <summary>
            /// length in bytes of the office art object table data
            /// </summary>
            //[ FieldOffset( 558 ) ]
            public uint lcbDggInfo;
            /// <summary>
            /// offset in table stream to STTBF that records the author abbreviations for authors who have made revisions in the document.
            /// </summary>
            //[ FieldOffset( 562 ) ]
            public int fcSttbfRMark;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 566 ) ]
            public uint lcbSttbfRMark;
            /// <summary>
            /// offset in table stream to STTBF that records caption titles used in the document.
            /// </summary>
            //[ FieldOffset( 570 ) ]
            public int fcSttbCaption;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 574 ) ]
            public uint lcbSttbCaption;
            /// <summary>
            /// offset in table stream to the STTBF that records the object names and indices into the caption STTBF for objects which get auto captions.
            /// </summary>
            //[ FieldOffset( 578 ) ]
            public int fcSttbAutoCaption;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 582 ) ]
            public uint lcbSttbAutoCaption;
            /// <summary>
            /// offset in table stream to WKB PLCF that describes the boundaries of contributing documents in a master document
            /// </summary>
            //[ FieldOffset( 586 ) ]
            public int fcPlcfwkb;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 590 ) ]
            public uint lcbPlcfwkb;
            /// <summary>
            /// offset in table stream of PLCF (of SPLS structures) that records spell check state
            /// </summary>
            //[ FieldOffset( 594 ) ]
            public int fcPlcfspl;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 598 ) ]
            public uint lcbPlcfspl;
            /// <summary>
            /// offset in table stream of PLCF that records the beginning CP in the text box subdoc of the text of individual text box entries. No structure is stored in this PLCF
            /// </summary>
            //[ FieldOffset( 602 ) ]
            public int fcPlcftxbxTxt;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 606 ) ]
            public uint lcbPlcftxbxTxt;
            /// <summary>
            /// offset in table stream of the FLD PLCF that records field boundaries recorded in the textbox subdoc.
            /// </summary>
            //[ FieldOffset( 610 ) ]
            public int fcPlcffldTxbx;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 614 ) ]
            public uint lcbPlcffldTxbx;
            /// <summary>
            /// offset in table stream of PLCF that records the beginning CP in the header text box subdoc of the text of individual header text box entries. No structure is stored in this PLC.
            /// </summary>
            //[ FieldOffset( 618 ) ]
            public int fcPlcfhdrtxbxTxt;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 622 ) ]
            public uint lcbPlcfhdrtxbxTxt;
            /// <summary>
            /// offset in table stream of the FLD PLCF that records field boundaries recorded in the header textbox subdoc.
            /// </summary>
            //[ FieldOffset( 626 ) ]
            public int fcPlcffldHdrTxbx;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 630 ) ]
            public uint lcbPlcffldHdrTxbx;
            /// <summary>
            /// Macro User storage
            /// </summary>
            //[ FieldOffset( 634 ) ]
            public int fcStwUser;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 638 ) ]
            public uint lcbStwUser;
            /// <summary>
            /// offset in table stream of embedded true type font data.
            /// </summary>
            //[ FieldOffset( 642 ) ]
            public int fcSttbttmbd;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 646 ) ]
            public uint cbSttbttmbd;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 650 ) ]
            public int fcUnused;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 654 ) ]
            public uint lcbUnused;
            /// <summary>
            /// offset in table stream of the PLF that records the page descriptors for the main text of the doc.
            /// </summary>
            //[ FieldOffset( 658 ) ]
            public int fcPgdMother;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 662 ) ]
            public uint lcbPgdMother;
            /// <summary>
            /// offset in table stream of the PLCF that records the break descriptors for the main text of the doc.
            /// </summary>
            //[ FieldOffset( 666 ) ]
            public int fcBkdMother;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 670 ) ]
            public uint lcbBkdMother;
            /// <summary>
            /// offset in table stream of the PLF that records the page descriptors for the footnote text of the doc.
            /// </summary>
            //[ FieldOffset( 674 ) ]
            public int fcPgdFtn;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 678 ) ]
            public uint lcbPgdFtn;
            /// <summary>
            /// offset in table stream of the PLCF that records the break descriptors for the footnote text of the doc.
            /// </summary>
            //[ FieldOffset( 682 ) ]
            public int fcBkdFtn;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 686 ) ]
            public uint lcbBkdFtn;
            /// <summary>
            /// offset in table stream of the PLF that records the page descriptors for the endnote text of the doc.
            /// </summary>
            //[ FieldOffset( 690 ) ]
            public int fcPgdEdn;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 694 ) ]
            public uint lcbPgdEdn;
            /// <summary>
            /// offset in table stream of the PLCF that records the break descriptors for the endnote text of the doc.
            /// </summary>
            //[ FieldOffset( 698 ) ]
            public int fcBkdEdn;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 702 ) ]
            public uint lcbBkdEdn;
            /// <summary>
            /// offset in table stream of the STTBF containing field keywords. This is only used in a small number of the international versions of word. This field is no longer written to the file for nFib >= 167.
            /// </summary>
            //[ FieldOffset( 706 ) ]
            public int fcSttbfIntlFld;
            /// <summary>
            /// Always 0 for nFib >= 167.
            /// </summary>
            //[ FieldOffset( 710 ) ]
            public uint lcbSttbfIntlFld;
            /// <summary>
            /// offset in table stream of a mailer routing slip.
            /// </summary>
            //[ FieldOffset( 714 ) ]
            public int fcRouteSlip;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 718 ) ]
            public uint lcbRouteSlip;
            /// <summary>
            /// offset in table stream of STTBF recording the names of the users who have saved this document alternating with the save locations.
            /// </summary>
            //[ FieldOffset( 722 ) ]
            public int fcSttbSavedBy;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 726 ) ]
            public uint lcbSttbSavedBy;
            /// <summary>
            /// offset in table stream of STTBF recording filenames of documents which are referenced by this document.
            /// </summary>
            //[ FieldOffset( 730 ) ]
            public int fcSttbFnm;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 734 ) ]
            public uint lcbSttbFnm;
            /// <summary>
            /// offset in the table stream of list format information.
            /// </summary>
            //[ FieldOffset( 738 ) ]
            public int fcPlcfLst;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 742 ) ]
            public uint lcbPlcfLst;
            /// <summary>
            /// offset in the table stream of list format override information.
            /// </summary>
            //[ FieldOffset( 746 ) ]
            public int fcPlfLfo;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 750 ) ]
            public uint lcbPlfLfo;
            /// <summary>
            /// offset in the table stream of the textbox break table (a PLCF of BKDs) for the main document
            /// </summary>
            //[ FieldOffset( 754 ) ]
            public int fcPlcftxbxBkd;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 758 ) ]
            public uint lcbPlcftxbxBkd;
            /// <summary>
            /// offset in the table stream of the textbox break table (a PLCF of BKDs) for the header subdocument
            /// </summary>
            //[ FieldOffset( 762 ) ]
            public int fcPlcftxbxHdrBkd;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 766 ) ]
            public uint lcbPlcftxbxHdrBkd;
            /// <summary>
            /// offset in main stream of undocumented undo / versioning data
            /// </summary>
            //[ FieldOffset( 770 ) ]
            public int fcDocUndo;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 774 ) ]
            public uint lcbDocUndo;
            /// <summary>
            /// offset in main stream of undocumented undo / versioning data
            /// </summary>
            //[ FieldOffset( 778 ) ]
            public int fcRgbuse;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 782 ) ]
            public uint lcbRgbuse;
            /// <summary>
            /// offset in main stream of undocumented undo / versioning data
            /// </summary>
            //[ FieldOffset( 786 ) ]
            public int fcUsp;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 790 ) ]
            public uint lcbUsp;
            /// <summary>
            /// offset in table stream of undocumented undo / versioning data
            /// </summary>
            //[ FieldOffset( 794 ) ]
            public int fcUskf;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 798 ) ]
            public uint lcbUskf;
            /// <summary>
            /// offset in table stream of undocumented undo / versioning data
            /// </summary>
            //[ FieldOffset( 802 ) ]
            public int fcPlcupcRgbuse;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 806 ) ]
            public uint lcbPlcupcRgbuse;
            /// <summary>
            /// offset in table stream of undocumented undo / versioning data
            /// </summary>
            //[ FieldOffset( 810 ) ]
            public int fcPlcupcUsp;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 814 ) ]
            public uint lcbPlcupcUsp;
            /// <summary>
            /// offset in table stream of string table of style names for glossary entries
            /// </summary>
            //[ FieldOffset( 818 ) ]
            public int fcSttbGlsyStyle;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 822 ) ]
            public uint lcbSttbGlsyStyle;
            /// <summary>
            /// offset in table stream of undocumented grammar options PL
            /// </summary>
            //[ FieldOffset( 826 ) ]
            public int fcPlgosl;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 830 ) ]
            public uint lcbPlgosl;
            /// <summary>
            /// offset in table stream of undocumented ocx data
            /// </summary>
            //[ FieldOffset( 834 ) ]
            public int fcPlcocx;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 838 ) ]
            public uint lcbPlcocx;
            /// <summary>
            /// offset in table stream of character property bin table.PLC. FCs in PLC are file offsets. Describes text of main document and all subdocuments.
            /// </summary>
            //[ FieldOffset( 842 ) ]
            public int fcPlcfbteLvc;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 846 ) ]
            public uint lcbPlcfbteLvc;
            //    /// <summary>
            //    /// 
            //    /// </summary>
            //    //[ FieldOffset( 850 ) ]
            //    public FILETIME ftModified;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 850 ) ]
            public uint dwLowDateTime;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 854 ) ]
            public uint dwHighDateTime;
            /// <summary>
            /// offset in table stream of LVC PLCF
            /// </summary>
            //[ FieldOffset( 858 ) ]
            public int fcPlcflvc;
            /// <summary>
            /// size of LVC PLCF, ==0 for non-complex files
            /// </summary>
            //[ FieldOffset( 862 ) ]
            public uint lcbPlcflvc;
            /// <summary>
            /// offset in table stream of autosummary ASUMY PLCF.
            /// </summary>
            //[ FieldOffset( 866 ) ]
            public int fcPlcasumy;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 870 ) ]
            public uint lcbPlcasumy;
            /// <summary>
            /// offset in table stream of PLCF (of SPLS structures) which records grammar check state
            /// </summary>
            //[ FieldOffset( 874 ) ]
            public int fcPlcfgram;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 878 ) ]
            public uint lcbPlcfgram;
            /// <summary>
            /// offset in table stream of list names string table
            /// </summary>
            //[ FieldOffset( 882 ) ]
            public int fcSttbListNames;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 886 ) ]
            public uint lcbSttbListNames;
            /// <summary>
            /// offset in table stream of undocumented undo / versioning data
            /// </summary>
            //[ FieldOffset( 890 ) ]
            public int fcSttbfUssr;
            /// <summary>
            /// 
            /// </summary>
            //[ FieldOffset( 894 ) ]
            public uint lcbSttbfUssr;
            #endregion

            #region Class constants
            /// <summary>
            /// 
            /// </summary>
            public const int DEF_SIZE = 186; // 93 pair of FC / LCB.
            #endregion

            #region Properties
            /// <summary>
            /// Gets the size of the structure.
            /// </summary>
            /// <value>The length.</value>
            internal override int Length
            {
                get
                {
                    return 898;
                }
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Parses the specified structure.
            /// </summary>
            /// <param name="arrBytes">The arr bytes.</param>
            internal override void Parse(byte[] arrBytes, int iOffset)
            {
                fcStshfOrig = ReadInt32(arrBytes, ref iOffset);
                lcbStshfOrig = ReadUInt32(arrBytes, ref iOffset);
                fcStshf = ReadInt32(arrBytes, ref iOffset);
                lcbStshf = ReadUInt32(arrBytes, ref iOffset);
                fcPlcffndRef = ReadInt32(arrBytes, ref iOffset);
                lcbPlcffndRef = ReadUInt32(arrBytes, ref iOffset);
                fcPlcffndTxt = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 182 ) ]
                lcbPlcffndTxt = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 186 ) ]
                fcPlcfandRef = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 190 ) ]
                lcbPlcfandRef = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 194 ) ]
                fcPlcfandTxt = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 198 ) ]
                lcbPlcfandTxt = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 202 ) ]
                fcPlcfsed = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 206 ) ]
                lcbPlcfsed = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 210 ) ]
                fcPlcpad = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 214 ) ]
                lcbPlcpad = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 218 ) ]
                fcPlcfphe = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 222 ) ]
                lcbPlcfphe = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 226 ) ]
                fcSttbfglsy = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 230 ) ]
                lcbSttbfglsy = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 234 ) ]
                fcPlcfglsy = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 238 ) ]
                lcbPlcfglsy = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 242 ) ]
                fcPlcfhdd = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 246 ) ]
                lcbPlcfhdd = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 250 ) ]
                fcPlcfbteChpx = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 254 ) ]
                lcbPlcfbteChpx = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 258 ) ]
                fcPlcfbtePapx = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 262 ) ]
                lcbPlcfbtePapx = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 266 ) ]
                fcPlcfsea = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 270 ) ]
                lcbPlcfsea = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 274 ) ]
                fcSttbfffn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 278 ) ]
                lcbSttbfffn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 282 ) ]
                fcPlcffldMom = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 286 ) ]
                lcbPlcffldMom = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 290 ) ]
                fcPlcffldHdr = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 294 ) ]
                lcbPlcffldHdr = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 298 ) ]
                fcPlcffldFtn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 302 ) ]
                lcbPlcffldFtn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 306 ) ]
                fcPlcffldAtn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 310 ) ]
                lcbPlcffldAtn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 314 ) ]
                fcPlcffldMcr = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 318 ) ]
                lcbPlcffldMcr = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 322 ) ]
                fcSttbfbkmk = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 326 ) ]
                lcbSttbfbkmk = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 330 ) ]
                fcPlcfbkf = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 334 ) ]
                lcbPlcfbkf = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 338 ) ]
                fcPlcfbkl = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 342 ) ]
                lcbPlcfbkl = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 346 ) ]
                fcCmds = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 350 ) ]
                lcbCmds = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 354 ) ]
                fcPlcmcr = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 358 ) ]
                lcbPlcmcr = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 362 ) ]
                fcSttbfmcr = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 366 ) ]
                lcbSttbfmcr = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 370 ) ]
                fcPrDrvr = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 374 ) ]
                lcbPrDrvr = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 378 ) ]
                fcPrEnvPort = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 382 ) ]
                lcbPrEnvPort = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 386 ) ]
                fcPrEnvLand = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 390 ) ]
                lcbPrEnvLand = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 394 ) ]
                fcWss = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 398 ) ]
                lcbWss = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 402 ) ]
                fcDop = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 406 ) ]
                lcbDop = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 410 ) ]
                fcSttbfAssoc = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 414 ) ]
                lcbSttbfAssoc = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 418 ) ]
                fcClx = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 422 ) ]
                lcbClx = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 426 ) ]
                fcPlcfpgdFtn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 430 ) ]
                lcbPlcfpgdFtn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 434 ) ]
                fcAutosaveSource = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 438 ) ]
                lcbAutosaveSource = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 442 ) ]
                fcGrpXstAtnOwners = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 446 ) ]
                lcbGrpXstAtnOwners = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 450 ) ]
                fcSttbfAtnbkmk = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 454 ) ]
                lcbSttbfAtnbkmk = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 458 ) ]
                fcPlcdoaMom = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 462 ) ]
                lcbPlcdoaMom = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 466 ) ]
                fcPlcdoaHdr = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 470 ) ]
                lcbPlcdoaHdr = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 474 ) ]
                fcPlcspaMom = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 478 ) ]
                lcbPlcspaMom = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 482 ) ]
                fcPlcspaHdr = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 486 ) ]
                lcbPlcspaHdr = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 490 ) ]
                fcPlcfAtnbkf = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 494 ) ]
                lcbPlcfAtnbkf = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 498 ) ]
                fcPlcfAtnbkl = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 502 ) ]
                lcbPlcfAtnbkl = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 506 ) ]
                fcPms = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 510 ) ]
                lcbPms = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 514 ) ]
                fcFormFldSttbs = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 518 ) ]
                lcbFormFldSttbs = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 522 ) ]
                fcPlcfendRef = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 526 ) ]
                lcbPlcfendRef = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 530 ) ]
                fcPlcfendTxt = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 534 ) ]
                lcbPlcfendTxt = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 538 ) ]
                fcPlcffldEdn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 542 ) ]
                lcbPlcffldEdn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 546 ) ]
                fcPlcfpgdEdn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 550 ) ]
                lcbPlcfpgdEdn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 554 ) ]
                fcDggInfo = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 558 ) ]
                lcbDggInfo = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 562 ) ]
                fcSttbfRMark = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 566 ) ]
                lcbSttbfRMark = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 570 ) ]
                fcSttbCaption = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 574 ) ]
                lcbSttbCaption = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 578 ) ]
                fcSttbAutoCaption = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 582 ) ]
                lcbSttbAutoCaption = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 586 ) ]
                fcPlcfwkb = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 590 ) ]
                lcbPlcfwkb = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 594 ) ]
                fcPlcfspl = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 598 ) ]
                lcbPlcfspl = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 602 ) ]
                fcPlcftxbxTxt = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 606 ) ]
                lcbPlcftxbxTxt = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 610 ) ]
                fcPlcffldTxbx = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 614 ) ]
                lcbPlcffldTxbx = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 618 ) ]
                fcPlcfhdrtxbxTxt = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 622 ) ]
                lcbPlcfhdrtxbxTxt = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 626 ) ]
                fcPlcffldHdrTxbx = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 630 ) ]
                lcbPlcffldHdrTxbx = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 634 ) ]
                fcStwUser = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 638 ) ]
                lcbStwUser = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 642 ) ]
                fcSttbttmbd = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 646 ) ]
                cbSttbttmbd = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 650 ) ]
                fcUnused = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 654 ) ]
                lcbUnused = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 658 ) ]
                fcPgdMother = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 662 ) ]
                lcbPgdMother = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 666 ) ]
                fcBkdMother = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 670 ) ]
                lcbBkdMother = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 674 ) ]
                fcPgdFtn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 678 ) ]
                lcbPgdFtn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 682 ) ]
                fcBkdFtn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 686 ) ]
                lcbBkdFtn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 690 ) ]
                fcPgdEdn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 694 ) ]
                lcbPgdEdn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 698 ) ]
                fcBkdEdn = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 702 ) ]
                lcbBkdEdn = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 706 ) ]
                fcSttbfIntlFld = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 710 ) ]
                lcbSttbfIntlFld = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 714 ) ]
                fcRouteSlip = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 718 ) ]
                lcbRouteSlip = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 722 ) ]
                fcSttbSavedBy = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 726 ) ]
                lcbSttbSavedBy = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 730 ) ]
                fcSttbFnm = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 734 ) ]
                lcbSttbFnm = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 738 ) ]
                fcPlcfLst = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 742 ) ]
                lcbPlcfLst = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 746 ) ]
                fcPlfLfo = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 750 ) ]
                lcbPlfLfo = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 754 ) ]
                fcPlcftxbxBkd = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 758 ) ]
                lcbPlcftxbxBkd = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 762 ) ]
                fcPlcftxbxHdrBkd = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 766 ) ]
                lcbPlcftxbxHdrBkd = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 770 ) ]
                fcDocUndo = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 774 ) ]
                lcbDocUndo = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 778 ) ]
                fcRgbuse = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 782 ) ]
                lcbRgbuse = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 786 ) ]
                fcUsp = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 790 ) ]
                lcbUsp = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 794 ) ]
                fcUskf = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 798 ) ]
                lcbUskf = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 802 ) ]
                fcPlcupcRgbuse = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 806 ) ]
                lcbPlcupcRgbuse = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 810 ) ]
                fcPlcupcUsp = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 814 ) ]
                lcbPlcupcUsp = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 818 ) ]
                fcSttbGlsyStyle = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 822 ) ]
                lcbSttbGlsyStyle = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 826 ) ]
                fcPlgosl = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 830 ) ]
                lcbPlgosl = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 834 ) ]
                fcPlcocx = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 838 ) ]
                lcbPlcocx = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 842 ) ]
                fcPlcfbteLvc = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 846 ) ]
                lcbPlcfbteLvc = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 850 ) ]
                dwLowDateTime = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 854 ) ]
                dwHighDateTime = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 858 ) ]
                fcPlcflvc = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 862 ) ]
                lcbPlcflvc = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 866 ) ]
                fcPlcasumy = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 870 ) ]
                lcbPlcasumy = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 874 ) ]
                fcPlcfgram = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 878 ) ]
                lcbPlcfgram = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 882 ) ]
                fcSttbListNames = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 886 ) ]
                lcbSttbListNames = ReadUInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 890 ) ]
                fcSttbfUssr = ReadInt32(arrBytes, ref iOffset);
                //[ FieldOffset( 894 ) ]
                lcbSttbfUssr = ReadUInt32(arrBytes, ref iOffset);
            }
            /// <summary>
            /// Saves the data structure.
            /// </summary>
            /// <param name="arrData">The destination array.</param>
            /// <param name="iOffset">The offset.</param>
            internal override int Save(byte[] arrBytes, int iOffset)
            {
                WriteInt32(arrBytes, ref iOffset, fcStshfOrig);
                WriteUInt32(arrBytes, ref iOffset, lcbStshfOrig);
                WriteInt32(arrBytes, ref iOffset, fcStshf);
                WriteUInt32(arrBytes, ref iOffset, lcbStshf);
                WriteInt32(arrBytes, ref iOffset, fcPlcffndRef);
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffndRef);
                WriteInt32(arrBytes, ref iOffset, fcPlcffndTxt);
                //[ FieldOffset( 182 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffndTxt);
                //[ FieldOffset( 186 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfandRef);
                //[ FieldOffset( 190 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfandRef);
                //[ FieldOffset( 194 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfandTxt);
                //[ FieldOffset( 198 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfandTxt);
                //[ FieldOffset( 202 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfsed);
                //[ FieldOffset( 206 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfsed);
                //[ FieldOffset( 210 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcpad);
                //[ FieldOffset( 214 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcpad);
                //[ FieldOffset( 218 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfphe);
                //[ FieldOffset( 222 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfphe);
                //[ FieldOffset( 226 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbfglsy);
                //[ FieldOffset( 230 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbfglsy);
                //[ FieldOffset( 234 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfglsy);
                //[ FieldOffset( 238 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfglsy);
                //[ FieldOffset( 242 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfhdd);
                //[ FieldOffset( 246 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfhdd);
                //[ FieldOffset( 250 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfbteChpx);
                //[ FieldOffset( 254 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfbteChpx);
                //[ FieldOffset( 258 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfbtePapx);
                //[ FieldOffset( 262 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfbtePapx);
                //[ FieldOffset( 266 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfsea);
                //[ FieldOffset( 270 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfsea);
                //[ FieldOffset( 274 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbfffn);
                //[ FieldOffset( 278 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbfffn);
                //[ FieldOffset( 282 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcffldMom);
                //[ FieldOffset( 286 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffldMom);
                //[ FieldOffset( 290 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcffldHdr);
                //[ FieldOffset( 294 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffldHdr);
                //[ FieldOffset( 298 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcffldFtn);
                //[ FieldOffset( 302 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffldFtn);
                //[ FieldOffset( 306 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcffldAtn);
                //[ FieldOffset( 310 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffldAtn);
                //[ FieldOffset( 314 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcffldMcr);
                //[ FieldOffset( 318 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffldMcr);
                //[ FieldOffset( 322 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbfbkmk);
                //[ FieldOffset( 326 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbfbkmk);
                //[ FieldOffset( 330 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfbkf);
                //[ FieldOffset( 334 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfbkf);
                //[ FieldOffset( 338 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfbkl);
                //[ FieldOffset( 342 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfbkl);
                //[ FieldOffset( 346 ) ]
                WriteInt32(arrBytes, ref iOffset, fcCmds);
                //[ FieldOffset( 350 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbCmds);
                //[ FieldOffset( 354 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcmcr);
                //[ FieldOffset( 358 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcmcr);
                //[ FieldOffset( 362 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbfmcr);
                //[ FieldOffset( 366 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbfmcr);
                //[ FieldOffset( 370 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPrDrvr);
                //[ FieldOffset( 374 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPrDrvr);
                //[ FieldOffset( 378 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPrEnvPort);
                //[ FieldOffset( 382 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPrEnvPort);
                //[ FieldOffset( 386 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPrEnvLand);
                //[ FieldOffset( 390 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPrEnvLand);
                //[ FieldOffset( 394 ) ]
                WriteInt32(arrBytes, ref iOffset, fcWss);
                //[ FieldOffset( 398 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbWss);
                //[ FieldOffset( 402 ) ]
                WriteInt32(arrBytes, ref iOffset, fcDop);
                //[ FieldOffset( 406 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbDop);
                //[ FieldOffset( 410 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbfAssoc);
                //[ FieldOffset( 414 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbfAssoc);
                //[ FieldOffset( 418 ) ]
                WriteInt32(arrBytes, ref iOffset, fcClx);
                //[ FieldOffset( 422 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbClx);
                //[ FieldOffset( 426 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfpgdFtn);
                //[ FieldOffset( 430 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfpgdFtn);
                //[ FieldOffset( 434 ) ]
                WriteInt32(arrBytes, ref iOffset, fcAutosaveSource);
                //[ FieldOffset( 438 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbAutosaveSource);
                //[ FieldOffset( 442 ) ]
                WriteInt32(arrBytes, ref iOffset, fcGrpXstAtnOwners);
                //[ FieldOffset( 446 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbGrpXstAtnOwners);
                //[ FieldOffset( 450 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbfAtnbkmk);
                //[ FieldOffset( 454 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbfAtnbkmk);
                //[ FieldOffset( 458 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcdoaMom);
                //[ FieldOffset( 462 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcdoaMom);
                //[ FieldOffset( 466 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcdoaHdr);
                //[ FieldOffset( 470 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcdoaHdr);
                //[ FieldOffset( 474 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcspaMom);
                //[ FieldOffset( 478 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcspaMom);
                //[ FieldOffset( 482 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcspaHdr);
                //[ FieldOffset( 486 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcspaHdr);
                //[ FieldOffset( 490 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfAtnbkf);
                //[ FieldOffset( 494 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfAtnbkf);
                //[ FieldOffset( 498 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfAtnbkl);
                //[ FieldOffset( 502 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfAtnbkl);
                //[ FieldOffset( 506 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPms);
                //[ FieldOffset( 510 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPms);
                //[ FieldOffset( 514 ) ]
                WriteInt32(arrBytes, ref iOffset, fcFormFldSttbs);
                //[ FieldOffset( 518 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbFormFldSttbs);
                //[ FieldOffset( 522 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfendRef);
                //[ FieldOffset( 526 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfendRef);
                //[ FieldOffset( 530 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfendTxt);
                //[ FieldOffset( 534 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfendTxt);
                //[ FieldOffset( 538 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcffldEdn);
                //[ FieldOffset( 542 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffldEdn);
                //[ FieldOffset( 546 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfpgdEdn);
                //[ FieldOffset( 550 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfpgdEdn);
                //[ FieldOffset( 554 ) ]
                WriteInt32(arrBytes, ref iOffset, fcDggInfo);
                //[ FieldOffset( 558 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbDggInfo);
                //[ FieldOffset( 562 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbfRMark);
                //[ FieldOffset( 566 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbfRMark);
                //[ FieldOffset( 570 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbCaption);
                //[ FieldOffset( 574 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbCaption);
                //[ FieldOffset( 578 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbAutoCaption);
                //[ FieldOffset( 582 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbAutoCaption);
                //[ FieldOffset( 586 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfwkb);
                //[ FieldOffset( 590 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfwkb);
                //[ FieldOffset( 594 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfspl);
                //[ FieldOffset( 598 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfspl);
                //[ FieldOffset( 602 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcftxbxTxt);
                //[ FieldOffset( 606 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcftxbxTxt);
                //[ FieldOffset( 610 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcffldTxbx);
                //[ FieldOffset( 614 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffldTxbx);
                //[ FieldOffset( 618 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfhdrtxbxTxt);
                //[ FieldOffset( 622 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfhdrtxbxTxt);
                //[ FieldOffset( 626 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcffldHdrTxbx);
                //[ FieldOffset( 630 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcffldHdrTxbx);
                //[ FieldOffset( 634 ) ]
                WriteInt32(arrBytes, ref iOffset, fcStwUser);
                //[ FieldOffset( 638 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbStwUser);
                //[ FieldOffset( 642 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbttmbd);
                //[ FieldOffset( 646 ) ]
                WriteUInt32(arrBytes, ref iOffset, cbSttbttmbd);
                //[ FieldOffset( 650 ) ]
                WriteInt32(arrBytes, ref iOffset, fcUnused);
                //[ FieldOffset( 654 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbUnused);
                //[ FieldOffset( 658 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPgdMother);
                //[ FieldOffset( 662 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPgdMother);
                //[ FieldOffset( 666 ) ]
                WriteInt32(arrBytes, ref iOffset, fcBkdMother);
                //[ FieldOffset( 670 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbBkdMother);
                //[ FieldOffset( 674 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPgdFtn);
                //[ FieldOffset( 678 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPgdFtn);
                //[ FieldOffset( 682 ) ]
                WriteInt32(arrBytes, ref iOffset, fcBkdFtn);
                //[ FieldOffset( 686 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbBkdFtn);
                //[ FieldOffset( 690 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPgdEdn);
                //[ FieldOffset( 694 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPgdEdn);
                //[ FieldOffset( 698 ) ]
                WriteInt32(arrBytes, ref iOffset, fcBkdEdn);
                //[ FieldOffset( 702 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbBkdEdn);
                //[ FieldOffset( 706 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbfIntlFld);
                //[ FieldOffset( 710 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbfIntlFld);
                //[ FieldOffset( 714 ) ]
                WriteInt32(arrBytes, ref iOffset, fcRouteSlip);
                //[ FieldOffset( 718 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbRouteSlip);
                //[ FieldOffset( 722 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbSavedBy);
                //[ FieldOffset( 726 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbSavedBy);
                //[ FieldOffset( 730 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbFnm);
                //[ FieldOffset( 734 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbFnm);
                //[ FieldOffset( 738 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfLst);
                //[ FieldOffset( 742 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfLst);
                //[ FieldOffset( 746 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlfLfo);
                //[ FieldOffset( 750 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlfLfo);
                //[ FieldOffset( 754 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcftxbxBkd);
                //[ FieldOffset( 758 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcftxbxBkd);
                //[ FieldOffset( 762 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcftxbxHdrBkd);
                //[ FieldOffset( 766 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcftxbxHdrBkd);
                //[ FieldOffset( 770 ) ]
                WriteInt32(arrBytes, ref iOffset, fcDocUndo);
                //[ FieldOffset( 774 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbDocUndo);
                //[ FieldOffset( 778 ) ]
                WriteInt32(arrBytes, ref iOffset, fcRgbuse);
                //[ FieldOffset( 782 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbRgbuse);
                //[ FieldOffset( 786 ) ]
                WriteInt32(arrBytes, ref iOffset, fcUsp);
                //[ FieldOffset( 790 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbUsp);
                //[ FieldOffset( 794 ) ]
                WriteInt32(arrBytes, ref iOffset, fcUskf);
                //[ FieldOffset( 798 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbUskf);
                //[ FieldOffset( 802 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcupcRgbuse);
                //[ FieldOffset( 806 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcupcRgbuse);
                //[ FieldOffset( 810 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcupcUsp);
                //[ FieldOffset( 814 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcupcUsp);
                //[ FieldOffset( 818 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbGlsyStyle);
                //[ FieldOffset( 822 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbGlsyStyle);
                //[ FieldOffset( 826 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlgosl);
                //[ FieldOffset( 830 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlgosl);
                //[ FieldOffset( 834 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcocx);
                //[ FieldOffset( 838 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcocx);
                //[ FieldOffset( 842 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfbteLvc);
                //[ FieldOffset( 846 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfbteLvc);
                //[ FieldOffset( 850 ) ]
                WriteUInt32(arrBytes, ref iOffset, dwLowDateTime);
                //[ FieldOffset( 854 ) ]
                WriteUInt32(arrBytes, ref iOffset, dwHighDateTime);
                //[ FieldOffset( 858 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcflvc);
                //[ FieldOffset( 862 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcflvc);
                //[ FieldOffset( 866 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcasumy);
                //[ FieldOffset( 870 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcasumy);
                //[ FieldOffset( 874 ) ]
                WriteInt32(arrBytes, ref iOffset, fcPlcfgram);
                //[ FieldOffset( 878 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbPlcfgram);
                //[ FieldOffset( 882 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbListNames);
                //[ FieldOffset( 886 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbListNames);
                //[ FieldOffset( 890 ) ]
                WriteInt32(arrBytes, ref iOffset, fcSttbfUssr);
                //[ FieldOffset( 894 ) ]
                WriteUInt32(arrBytes, ref iOffset, lcbSttbfUssr);

                return Length;// temporary width 
            }
            #endregion
        }
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        public KnownPart knownPart = new KnownPart();
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrBuffer"></param>
        /// <param name="provider"></param>
        internal override void SetBuffer(byte[] arrBuffer)
        {
            if (arrBuffer == null)
                throw new ArgumentNullException("arrBuffer");

#if AllowUnsafeCode && !SILVERLIGHT && !WP
            MemoryConverter.Instance.Copy(arrBuffer, knownPart);
#else
      knownPart.Parse( arrBuffer, 0 );      
#endif

            Resize(arrBuffer.Length / DEF_MEMBER_SIZE - DEF_KNOWN_MEMBERS);

            int iUnknownStart = DEF_KNOWN_MEMBERS * DEF_MEMBER_SIZE;
            int iUnknownLen = arrBuffer.Length - iUnknownStart;
            byte[] arrUnknownBuffer = new byte[iUnknownLen];

            Array.Copy(arrBuffer, iUnknownStart, arrUnknownBuffer, 0, iUnknownLen);
            Buffer.BlockCopy(arrUnknownBuffer, 0, m_arrLongs, 0, arrUnknownBuffer.Length);
            //API.CopyMemory( m_arrLongs, arrUnknownBuffer, arrUnknownBuffer.Length );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="iOffset"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentOutOfRangeException("arrData");

            int iLength = KnownPart.DEF_SIZE;

            if (iOffset < 0 || iOffset + iLength > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            //byte[] arrBuffer = new byte[ iLength ];

            int[] arrBuffer = new int[iLength];
            int iSize = iLength * Constants.BytesInInt;
            byte[] arrByteBuffer = new byte[iSize];

#if AllowUnsafeCode && !SILVERLIGHT && !WP
            MemoryConverter.Instance.Copy(knownPart, arrByteBuffer, 0, iSize);
#else
      knownPart.Save( arrByteBuffer, 0 );   
#endif

            Buffer.BlockCopy(arrByteBuffer, 0, arrBuffer, 0, iSize);
            //API.CopyMemory( arrBuffer, arrByteBuffer, iSize );

            /*      for( int i = 2; i < DEF_FCLCB_NUMBER; i+= 2 )
                  {
                    arrBuffer[ i ] = arrBuffer[ i - 2 ] + arrBuffer[ i - 1 ];
                  }
            */
            //arrBuffer.CopyTo( arrData, iOffset );
            //      API.CopyMemory( ref arrData[ iOffset ], arrBuffer, iSize );
            Buffer.BlockCopy(arrBuffer, 0, arrData, iOffset, iSize);
            iOffset += iSize;

            return iSize + base.Save(arrData, iOffset);
        }

        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        public int BytesCount
        {
            get
            {
                return KnownPart.DEF_SIZE * Constants.BytesInInt;
            }
        }
        #endregion
    }
}
