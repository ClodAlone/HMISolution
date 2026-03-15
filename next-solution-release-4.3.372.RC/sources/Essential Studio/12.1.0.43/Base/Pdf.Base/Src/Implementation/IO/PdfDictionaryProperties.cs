#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// PDF dictionary properties.
    /// </summary>
#if NETFX_CORE || WP
    public class DictionaryProperties
#else
    internal class DictionaryProperties
#endif
    {
        public const string Size = "Size";
        public const string First = "First";
        public const string N = "N";
        public const string W = "W";
        public const string Count = "Count";
        public const string Length = "Length";
        public const string Length1 = "Length1";
        public const string Length2 = "Length2";
        public const string Length3 = "Length3";
        public const string Names = "Names";
        public const string Index = "Index";
        public const string Type = "Type";
        public const string Lang = "Lang";
        public const string ViewerPreferences = "ViewerPreferences";
        public const string Filter = "Filter";
        public const string Prev = "Prev";
        public const string Kids = "Kids";
        public const string DecodeParms = "DecodeParms";
        public const string ID = "ID";
        public const string Encrypt = "Encrypt";
        public const string Root = "Root";
        public const string Pages = "Pages";
        public const string Columns = "Columns";
        public const string Rows = "Rows";
        public const string BitsPerComponent = "BitsPerComponent";
        public const string Matte = "Matte";
        public const string Parent = "Parent";
        public const string Metadata = "Metadata";
        public const string XML = "XML";
        public const string Pg = "Pg";
        public const string ParentTree = "ParentTree";
        public const string ParentTreeNextKey = "ParentTreeNextKey";

        public const string MediaBox = "MediaBox";
        public const string CropBox = "CropBox";
        public const string BleedBox = "BleedBox";
        public const string TrimBox = "TrimBox";
        public const string ArtBox = "ArtBox";

        public const string Resources = "Resources";
        public const string ExtGState = "ExtGState";
        public const string ColorSpace = "ColorSpace";
        public const string Pattern = "Pattern";
        public const string Shading = "Shading";
        public const string XObject = "XObject";
        public const string Group = "Group";

        public const string Font = "Font";
        public const string Type1 = "Type1";
        public const string Type0 = "Type0";
        public const string DescendantFonts = "DescendantFonts";
        public const string CIDFontType2 = "CIDFontType2";
        public const string CIDFontType0 = "CIDFontType0";
        public const string CIDToGIDMap = "CIDToGIDMap";
        public const string CIDSystemInfo = "CIDSystemInfo";
        public const string Registry = "Registry";

        public const string Ordering = "Ordering";
        public const string Supplement = "Supplement";

        public const string DW = "DW";
        public const string Identity = "Identity";
        public const string IdentityH = "Identity-H";
        public const string ToUnicode = "ToUnicode";
        public const string FirstChar = "FirstChar";
        public const string LastChar = "LastChar";
        public const string Widths = "Widths";
        public const string FontDescriptor = "FontDescriptor";
        public const string TrueType = "TrueType";
        public const string BaseFont = "BaseFont";
        public const string Encoding = "Encoding";
        public const string FontName = "FontName";
        public const string Flags = "Flags";
        public const string FontBBox = "FontBBox";
        public const string MissingWidth = "MissingWidth";
        public const string StemV = "StemV";
        public const string StemH = "StemH";
        public const string ItalicAngle = "ItalicAngle";
        public const string CapHeight = "CapHeight";
        public const string XHeight = "XHeight";
        public const string Ascent = "Ascent";
        public const string Descent = "Descent";
        public const string Leading = "Leading";
        public const string MaxWidth = "MaxWidth";
        public const string AvgWidth = "AvgWidth";
        public const string FontFile = "FontFile";
        public const string FontFile2 = "FontFile2";
        public const string FontFile3 = "FontFile3";
        public const string WinAnsiEncoding = "WinAnsiEncoding";

        public const string ProcSet = "ProcSet";
        public const string Properties = "Properties";
        public const string Contents = "Contents";
        public const string Form = "Form";
        public const string Subtype = "Subtype";
        public const string BBox = "BBox";
        public const string Rotate = "Rotate";
        public const string UserUnit = "UserUnit";
        public const string XFA = "XFA";

        public const string PatternType = "PatternType";
        public const string PaintType = "PaintType";
        public const string TilingType = "TilingType";
        public const string XStep = "XStep";
        public const string YStep = "YStep";
        public const string Matrix = "Matrix";
        public const string AntiAlias = "AntiAlias";
        public const string Background = "Background";
        public const string Domain = "Domain";
        public const string Range = "Range";
        public const string Function = "Function";
        public const string FunctionType = "FunctionType";
        public const string BitsPerSample = "BitsPerSample";
        public const string Extend = "Extend";
        public const string Coords = "Coords";
        public const string ShadingType = "ShadingType";

        public const string Rect = "Rect";
        public const string Annots = "Annots";
        public const string Annot = "Annot";
        public const string F = "F";
        public const string Name = "Name";
        public const string Open = "Open";
        public const string Sound = "Sound";
        public const string R = "R";
        public const string B = "B";
        public const string C = "C";
        public const string CF = "CF";
        public const string CFM = "CFM";
        public const string StdCF = "StdCF";


        public const string E = "E";

        public const string Border = "Border";
        public const string A = "A";
        public const string S = "S";
        public const string URI = "URI";
        public const string FS = "FS";
        public const string EF = "EF";
        public const string Dest = "Dest";
        public const string D = "D";
        public const string AP = "AP";
        public const string APN = "AP /N";

        public const string Image = "Image";
        public const string Decode = "Decode";
        public const string Width = "Width";
        public const string Height = "Height";

        public const string Predictor = "Predictor";
        public const string Colors = "Colors";
        public const string EarlyChange = "EarlyChange";
        public const string Mask = "Mask";
        public const string SMask = "SMask";
        public const string SMaskInData = "SMaskInData";
        public const string ImageMask = "ImageMask";
        public const string K = "K";
        public const string BlackIs1 = "BlackIs1";
        public const string EndOfBlock = "EndOfBlock";

        public const string DeviceRGB = "DeviceRGB";
        public const string DeviceCMYK = "DeviceCMYK";
        public const string DeviceGray = "DeviceGray";
        public const string Indexed = "Indexed";


        public const string FlateDecode = "FlateDecode";
        public const string FlateDecodeShort = "Fl";
        public const string LZWDecode = "LZWDecode";
        public const string LZWDecodeShort = "LZW";
        public const string DCTDecode = "DCTDecode";
        public const string DCTDecodeShort = "DCT";
        public const string ASCIIHexDecode = "ASCIIHexDecode";
        public const string ASCIIHexDecodeShort = "AHx";
        public const string ASCII85Decode = "ASCII85Decode";
        public const string ASCII85DecodeShort = "A85";
        public const string Crypt = "Crypt";
        public const string JPXDecode = "JPXDecode";
        public const string JBIG2Decode = "JBIG2Decode";
        public const string JBIG2Globals = "JBIG2Globals";
        public const string CCITTFaxDecode = "CCITTFaxDecode";
        public const string CCITTFaxDecodeShort = "CCF";
        public const string RunLengthDecode = "RunLengthDecode";
        public const string RunLenghtDecodeShort = "RL";
        public const string Info = "Info";

        public const string CenterWindow = "CenterWindow";
        public const string DisplayDocTitle = "DisplayDocTitle";
        public const string FitWindow = "FitWindow";
        public const string HideMenubar = "HideMenubar";
        public const string HideToolbar = "HideToolbar";
        public const string HideWindowUI = "HideWindowUI";
        public const string PageMode = "PageMode";
        public const string PageLayout = "PageLayout";

        public const string Launch = "Launch";
        public const string Link = "Link";
        public const string XYZ = "XYZ";
        public const string Fit = "Fit";
        public const string FitR = "FitR";
        public const string Text = "Text";
        public const string FileAttachment = "FileAttachment";
        public const string EmbeddedFile = "EmbeddedFile";
        public const string Filespec = "Filespec";

        public const string Author = "Author";
        public const string Title = "Title";
        public const string Subject = "Subject";
        public const string Keywords = "Keywords";
        public const string Creator = "Creator";
        public const string Producer = "Producer";
        public const string CreationDate = "CreationDate";
        public const string ModificationDate = "ModDate";

        public const string Last = "Last";
        public const string Outlines = "Outlines";
        public const string Next = "Next";

        public const string Action = "Action";
        public const string GoTo = "GoTo";
        public const string OpenAction = "OpenAction";
        public const string Volume = "Volume";
        public const string Synchronous = "Synchronous";
        public const string Repeat = "Repeat";
        public const string Mix = "Mix";
        public const string Named = "Named";
        public const string JavaScript = "JavaScript";
        public const string JS = "JS";
        public const string URL = "URL";
        public const string SubmitForm = "SubmitForm";
        public const string ResetForm = "ResetForm";

        public const string EmbeddedFiles = "EmbeddedFiles";
        public const string Description = "Desc";
        public const string Params = "Params";
        public const string UF = "UF";

        public const string CA = "CA";
        public const string ca = "ca";
        public const string BM = "BM";
        public const string OPM = "OPM";
        public const string HT = "HT";

        public const string St = "St";
        public const string r = "r";
        public const string a = "a";
        public const string P = "P";
        public const string U = "U";
        public const string O = "O";
        public const string OE = "OE";
        public const string UE = "UE";

        public const string PageLabels = "PageLabels";
        public const string Nums = "Nums";

        public const string Standard = "Standard";
        public const string AcroForm = "AcroForm";
        public const string Fields = "Fields";
        public const string T = "T";
        public const string FT = "FT";
        public const string Btn = "Btn";
        public const string Tx = "Tx";
        public const string Ch = "Ch";
        public const string Sig = "Sig";
        public const string FieldFlags = "Ff";
        public const string Widget = "Widget";
        public const string BS = "BS";
        public const string MK = "MK";
        public const string H = "H";
        public const string NeedAppearances = "NeedAppearances";
        public const string On = "On";
        public const string Off = "Off";
        public const string V = "V";
        public const string BC = "BC";
        public const string BG = "BG";
        public const string DA = "DA";
        public const string DR = "DR";
        public const string AA = "AA";
        public const string TM = "TM";
        public const string TU = "TU";
        public const string MaxLen = "MaxLen";
        public const string Yes = "Yes";
        public const string Opt = "Opt";
        public const string I = "I";

        public const string Perms = "Perms";
        public const string TransformParams = "TransformParams";
        public const string FieldMDP = "FieldMDP";
        public const string DigestValue = "DigestValue";
        public const string DigestLocation = "DigestLocation";
        public const string DigestMethod = "DigestMethod";
        public const string MD5 = "MD5";
        public const string SigRef = "SigRef";
        public const string Data = "Data";
        public const string Include = "Include";
        public const string Reference = "Reference";
        public const string PPKMS = "Adobe.PPKMS";
        public const string DocMDP = "DocMDP";

        public const string Location = "Location";
        public const string ContactInfo = "ContactInfo";
        public const string Reason = "Reason";
        public const string M = "M";
        public const string SubFilter = "SubFilter";
        public const string ByteRange = "ByteRange";
        public const string TransformMethod = "TransformMethod";
        public const string SigFlags = "SigFlags";
        public const string WC = "WC";
        public const string WS = "WS";
        public const string DS = "DS";
        public const string WP = "WP";
        public const string DP = "DP";

        public const string Q = "Q";

        public const string X = "X";
        public const string Fo = "Fo";
        public const string Bl = "Bl";

        public const string PageDuration = "Dur";
        public const string Transition = "Trans";
        public const string Scale = "SS";
        public const string Style = "S";
        public const string Duration = "D";
        public const string Dimension = "Dm";
        public const string Motion = "M";
        public const string Direction = "Di";

        public const string AS = "AS";

        public const string Dests = "Dests";
        public const string Limits = "Limits";

        public const string OnInstantiate = "OnInstantiate";
        public const string _3D = "3D";
        public const string _3DD = "3DD";
        public const string _3DA = "3DA";
        public const string U3D = "U3D";
        public const string _3DB = "3DB";
        public const string _3DV = "3DV";
        public const string DV = "DV";
        public const string C2W = "C2W";
        public const string IN = "IN";
        public const string MS = "MS";
        public const string XN = "XN";
        public const string AN = "AN";
        public const string PROJECTION = "P";
        public const string CLIPPINGSTYLE = "CS";
        public const string XNF = "XNF";
        public const string ANF = "ANF";
        public const string FOV = "FOV";
        public const string PS = "PS";
        public const string OS = "OS";
        public const string OB = "OB";
        public const string VA = "VA";
        public const string _3DLightingScheme = "3DLightingScheme";
        public const string _3DBG = "3DBG";
        public const string SC = "SC";
        public const string CS = "CS";
        public const string EA = "EA";
        public const string _3DRenderMode = "3DRenderMode";
        public const string AC = "AC";
        public const string FC = "FC";
        public const string CV = "CV";
        public const string _3DAnimationStyle = "3DAnimationStyle";
        public const string PC = "PC";
        public const string PO = "PO";
        public const string PV = "PV";
        public const string XA = "XA";
        public const string AIS = "AIS";
        public const string L = "L";
        public const string PI = "PI";
        public const string XD = "XD";
        public const string DIS = "DIS";
        public const string TB = "TB";
        public const string NP = "NP";
        public const string _3DCrossSection = "3DCrossSection";
        public const string IV = "IV";
        public const string IC = "IC";
        public const string _3DView = "3DView";
        public const string U3DPath = "U3DPath";
        public const string CO = "CO";
        public const string RM = "RM";
        public const string LS = "LS";
        public const string SA = "SA";
        public const string NA = "NA";
        public const string NR = "NR";
        public const string _3DNode = "3DNode";
        public const string FormType = "FormType";

        public const string Stamp = "Stamp";

        public const string Line = "Line";
        public const string LE = "LE";
        public const string LLE = "LLE";
        public const string LL = "LL";
        public const string Cap = "Cap";
        public const string IT = "IT";
        public const string CP = "CP";
        public const string Inline = "Inline";
        public const string Top = "Top";
        public const string LineHeight = "LineHeight";

        public const string QuadPoints = "QuadPoints";

        public const string PrintScaling = "PrintScaling";

        public const string GTS_PDFX = "GTS_PDFX";
        public const string OutputConditionIdentifier = "OutputConditionIdentifier";
        public const string OutputIntent = "OutputIntent";
        public const string OutputIntents = "OutputIntents";
        public const string RegistryName = "RegistryName";
        public const string OutputCondition = "OutputCondition";
        public const string GTS_PDFXConformance = "GTS_PDFXConformance";
        public const string Trapped = "Trapped";
        public const string GTS_PDFXVersion = "GTS_PDFXVersion";

        public const string EncryptMetadata = "EncryptMetadata";

        public const string CalGray = "CalGray";
        public const string CalRGB = "CalRGB";
        public const string Lab = "Lab";
        public const string ICCBased = "ICCBased";
        public const string WhitePoint = "WhitePoint";
        public const string BlackPoint = "BlackPoint";
        public const string Gamma = "Gamma";
        public const string Alternate = "Alternate";
        public const string Alt = "Alt";
        public const string C0 = "C0";
        public const string C1 = "C1";
        public const string CidSet = "CIDSet";

        public const string Differences = "Differences";

        public const string Print = "Print";

        public const string XFdf = "xfdf";
        public const string Field = "field";
        public const string Value = "value";
        public const string Ids = "Ids";

        public const string MarkInfo = "MarkInfo";
        public const string Marked = "Marked";

        public const string OCProperties = "OCProperties";
        public const string Ocg = "OCGs";
        public const string Defaultview = "D";
        public const string OCGName = "Name";
        public const string OCGOrder = "Order";
        public const string OCGON = "ON";
        public const string OCGOFF = "OFF";
        public const string OCGVisible = "Visible";
        public const string OCGLayerID = "LayerID";
        
        public const string InkList = "InkList";
        public const string Ink = "Ink";
        public const string StructTreeRoot = "StructTreeRoot";
        public const string StructParents = "StructParents";

        public const string Collection = "Collection";
        public const string CollectionSchema = "CollectionSchema";
        public const string CollectionField = "CollectionField";
        public const string Schema = "Schema";
        public const string CollectionItem = "CollectionItem";
        public const string CI = "CI";
        public const string View = "View";

        public const string Xref = "xref";
        public const string Obj = "obj";

    }
}
