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
using System.Xml;
using System.IO;

namespace Syncfusion.DocIO.DLS.Convertors
{
    class DocxConstants
    {
        #region Constants
        //Part path
        internal const string DocumentPath = @"word\document.xml";
        internal const string StylePath = @"word\styles.xml";
        internal const string NumberingPath = @"word\numbering.xml";
        internal const string SettingsPath = @"word\settings.xml";
        internal const string HeaderPath = @"word\header";
        internal const string FooterPath = @"word\footer";
        internal const string CommentsPath = @"word\comments.xml";
        internal const string ImagePath = @"word\media\image";
        internal const string FootnotesPath = @"word\footnotes.xml";
        internal const string EndnotesPath = @"word\endnotes.xml";
        internal const string AppPath = @"docProps\app.xml";
        internal const string CorePath = @"docProps\core.xml";
        internal const string CustomPath = @"docProps\custom.xml";
        internal const string FontTablePath = @"word\fontTable.xml";
        internal const string ContentTypesPath = @"[Content_Types].xml";
        internal const string ChartsPath = "word/charts/";
        internal const string DefaultEmbeddingPath = @"word/embeddings/";
        internal const string EmbeddingPath = @"word\embeddings\";
        internal const string DrawingPath = @"word\drawings\drawing1.xml";
        internal const string ThemePath = @"word/theme/";
        internal const string DiagramPath = "word/diagrams/";
        internal const string ControlPath = "word/activeX/";
        internal const string VbaProject = "vbaProject.bin";
        internal const string VbaData = "vbaData.xml";
        internal const string VbaProjectPath = @"word\vbaProject.bin";
        internal const string VbaDataPath = @"word\vbaData.xml";
        internal const string CustomXMLPath = @"customXml\";

        //Relationship path
        internal const string GeneralRelationPath = @"_rels\.rels";
        internal const string WordRelationPath = @"word\_rels\document.xml.rels";
        internal const string CommentsRelationPath = @"word\_rels\comments.xml.rels";
        internal const string FootnotesRelationPath = @"word\_rels\footnotes.xml.rels";
        internal const string EndnotesRelationPath = @"word\_rels\endnotes.xml.rels";
        internal const string NumberingRelationPath = @"word\_rels\numbering.xml.rels";
        internal const string HeaderRelationPath = @"word\_rels\header";
        internal const string FooterRelationPath = @"word\_rels\footer";
        internal const string SettingsRelationpath = @"word\_rels\settings.xml.rels";
        internal const string VbaProjectRelsPath = @"word\_rels\vbaProject.bin.rels";

        //Content type of the parts
        internal const string XmlContentType = @"application/xml";
        internal const string DocumentContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml";
        internal const string TemplateContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.template.main+xml";
        internal const string CommentsContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.comments+xml";
        internal const string SettingsContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml";
        internal const string EndnoteContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml";
        internal const string FontTableContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.fontTable+xml";
        internal const string FooterContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml";
        internal const string FootnoteContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml";
        internal const string GlossaryDocumentContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.document.glossary+xml";
        internal const string HeaderContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml";
        internal const string NumberingContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml";
        internal const string StylesContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml";
        internal const string WebSettingsContentType = @"application/vnd.openxmlformats-officedocument.wordprocessingml.webSettings+xml";
        internal const string AppContentType = @"application/vnd.openxmlformats-officedocument.extended-properties+xml";
        internal const string CoreContentType = @"application/vnd.openxmlformats-package.core-properties+xml";
        internal const string CustomContentType = @"application/vnd.openxmlformats-officedocument.custom-properties+xml";
        internal const string CustomXmlContentType = @"application/vnd.openxmlformats-officedocument.customXmlProperties+xml";
        internal const string RelationContentType = @"application/vnd.openxmlformats-package.relationships+xml";
        internal const string DiagramColor = @"application/vnd.openxmlformats-officedocument.drawingml.diagramColors+xml";
        internal const string DiagramData = @"application/vnd.openxmlformats-officedocument.drawingml.diagramData+xml";
        internal const string DiagramLayout = @"application/vnd.openxmlformats-officedocument.drawingml.diagramLayout+xml";
        internal const string DiagramStyle = @"application/vnd.openxmlformats-officedocument.drawingml.diagramStyle+xml";
        internal const string ChartsContentType = @"application/vnd.openxmlformats-officedocument.drawingml.chart+xml";
        internal const string ThemeContentType = @"application/vnd.openxmlformats-officedocument.theme+xml";
        internal const string ChartDrawingContentType = "application/vnd.openxmlformats-officedocument.drawingml.chartshapes+xml";
        internal const string ActiveXContentType = @"application/vnd.ms-office.activeX+xml";
        internal const string ActiveXBinContentType = "application/vnd.ms-office.activeX";
        internal const string TableStyleContentType = @"application/vnd.openxmlformats-officedocument.presentationml.tableStyles+xml";
        internal const string XlsxContentType = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        internal const string ChartStyleContentType = @"application/vnd.ms-office.chartstyle+xml";
        internal const string ChartColorStyleContentType = @"application/vnd.ms-office.chartcolorstyle+xml";
        internal const string VbaProjectContentType = @"application/vnd.ms-office.vbaProject";
        internal const string VbaDataContentType = @"application/vnd.ms-word.vbaData+xml";
        internal const string MacroDocumentContentType = @"application/vnd.ms-word.document.macroEnabled.main+xml";
        internal const string MacroTemplateContentType = @"application/vnd.ms-word.template.macroEnabledTemplate.main+xml";

        // Relationship types of document parts
        internal const string AltChunkRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/aFChunk";
        internal const string CommentsRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments";
        internal const string SettingsRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings";
        internal const string EndnoteRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/endnotes";
        internal const string FontTableRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/fontTable";
        internal const string FooterRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer";
        internal const string FootnoteRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/footnotes";
        internal const string HeaderRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/header";
        internal const string DocumentRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
        internal const string NumberingRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/numbering";
        internal const string StylesRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";
        internal const string OleObjectRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/oleObject";
        internal const string ChartRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart";
        internal const string ThemeRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme";
        internal const string TableStyleRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/tableStyles";
        internal const string CoreRelType = @"http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties";
        internal const string AppRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties";
        internal const string CustomRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties";
        internal const string ImageRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
        internal const string HyperlinkRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";
        internal const string ControlRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/control";
        internal const string PackageRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/package";
        internal const string VbaProjectRelType = @"http://schemas.microsoft.com/office/2006/relationships/vbaProject";
        internal const string VbaDataRelType = @"http://schemas.microsoft.com/office/2006/relationships/wordVbaData";
        internal const string CustomXmlRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/customXml";
        internal const string CustomUIRelType = @"http://schemas.microsoft.com/office/2006/relationships/ui/extensibility";
        internal const string AttachedTemplateRelType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/attachedTemplate";
        // Namespaces
        internal const string W_namespace = @"http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        internal const string WP_namespace = @"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";
        internal const string PIC_namespace = @"http://schemas.openxmlformats.org/drawingml/2006/picture";
        internal const string A_namespace = @"http://schemas.openxmlformats.org/drawingml/2006/main";
        internal const string R_namespace = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        internal const string RP_namespace = @"http://schemas.openxmlformats.org/package/2006/relationships";
        internal const string V_namespace = @"urn:schemas-microsoft-com:vml";
        internal const string O_namespace = @"urn:schemas-microsoft-com:office:office";
        internal const string Xml_namespace = @"http://www.w3.org/XML/1998/namespace";
        internal const string W10_namespace = @"urn:schemas-microsoft-com:office:word";
        internal const string CP_namespace = @"http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
        internal const string DC_namespace = @"http://purl.org/dc/elements/1.1/";
        internal const string DCTERMS_namespace = @"http://purl.org/dc/terms/";
        internal const string XSI_namespace = @"http://www.w3.org/2001/XMLSchema-instance";
        internal const string docProps_namespace = @"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
        internal const string VE_namespace = @"http://schemas.openxmlformats.org/markup-compatibility/2006";
        internal const string M_namespace = @"http://schemas.openxmlformats.org/officeDocument/2006/math";
        internal const string WNE_namespace = @"http://schemas.microsoft.com/office/word/2006/wordml";
        internal const string DCMI_namespace = @"http://purl.org/dc/dcmitype/";
        internal const string CustomProps_namespace = @"http://schemas.openxmlformats.org/officeDocument/2006/custom-properties";
        internal const string VT_namespace = @"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";
        internal const string CHART_namespace = @"http://schemas.openxmlformats.org/drawingml/2006/chart";
        internal const string SL_namespace = @"http://schemas.openxmlformats.org/schemaLibrary/2006/main";
        //2010 namespaces
        internal const string W14_namespace = @"http://schemas.microsoft.com/office/word/2010/wordml";
        internal const string WPC_namesapce = @"http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas";
        internal const string WP14_namespace = @"http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing";
        internal const string WPG_namespace = @"http://schemas.microsoft.com/office/word/2010/wordprocessingGroup";
        internal const string WPI_namespace = @"http://schemas.microsoft.com/office/word/2010/wordprocessingInk";
        internal const string WPS_namespace = @"http://schemas.microsoft.com/office/word/2010/wordprocessingShape";
        //2013 namespaces
        internal const string W15_namespace = @"http://schemas.microsoft.com/office/word/2012/wordml";

        //Encryption namespaces
        internal const string E_namespace = @"http://schemas.microsoft.com/office/2006/encryption";
        internal const string P_namespace = @"http://schemas.microsoft.com/office/2006/keyEncryptor/password";
        internal const string Cert_namespace = @"http://schemas.microsoft.com/office/2006/keyEncryptor/certificate";

        // Dls xml tags
        internal const string c_relationshipsTag = "Relationships";
        internal const string c_relationshipTag = "Relationship";
        internal const string c_idTag = "Id";
        internal const string c_typeTag = "Type";
        internal const string c_targetTag = "Target";

        internal const int TwentiethOfPoint = 20;
        internal const int BorderMultiplier = 8;
        internal const char TOC_SYMBOL = (char)0x01;
        internal const char FOOTNOTE_SYMBOL = (char)0x02;
        internal const char PAGENUMBER_SYMBOL = (char)0xB;

        /// <summary>
        /// String constants
        /// </summary>
        internal const string DEF_FIT_TEXT_TO_SHAPE = "mso-fit-shape-to-text:t";

        // Document tags
        internal const string c_conditionalTableStyleTag = "tblStylePr";
        internal const string c_tableFormatTag = "tblPr";
        internal const string c_rowFormatTag = "trPr";
        internal const string c_cellFormatTag = "tcPr";
        internal const string c_paragraphFormatTag = "pPr";
        internal const string c_characterFormatTag = "rPr";
        #endregion Constants
    }
}
