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

#if !SILVERLIGHT

namespace Syncfusion.DocIO.DLS
{
    internal class EPubConstants
    {
        internal const string Container = "container";

        internal const string ContainerNamespace = "urn:oasis:names:tc:opendocument:xmlns:container";

        internal const string Version = "version";

        internal const string Rootfiles = "rootfiles";

        internal const string Rootfile = "rootfile";

        internal const string FullPath = "full-path";

        internal const string PackageFileName = "content.opf";

        internal const string MediaType = "media-type";

        internal const string OEBPSContentType = "application/oebps-package+xml";

        internal const string EPubContentType = "application/epub+zip";

        internal const string PackageContentType = "application/x-dtbncx+xml";

        internal const string XHTMLContentType = "application/xhtml+xml";

        internal const string PackageElement = "package";

        internal const string IDPFNamespace = "http://www.idpf.org/2007/opf";

        internal const string UID = "unique-identifier";

        internal const string GUID = "guid";

        internal const string XmlPrefix = "xmlns";

        internal const string XSIPrefix = "xsi";

        internal const string XSIPartType = "http://www.w3.org/2001/XMLSchema-instance";

        internal const string MetadataElement = "metadata";

        internal const string DublinCorePrefix = "dc";

        internal const string DublinCorePartType = "http://purl.org/dc/elements/1.1/";

        internal const string PackagePrefix = "opf";

        internal const string TitleElement = "title";

        internal const string CreatorElement = "creator";

        internal const string IdentifierElement = "identifier";

        internal const string IdAttribute = "id";

        internal const string LanguageElement = "language";

        internal const string Type = "type";

        internal const string DublinCoreTermsPrefix = "dcterms";

        internal const string LanguageTag = "RFC3066";

        internal const string ManifestElement = "manifest";

        internal const string ItemElement = "item";

        internal const string HyperlinkAttribute = "href";

        internal const string NavigationFileName = "toc.ncx";

        internal const string SpineElement = "spine";

        internal const string ItemReferenceElement = "itemref";

        internal const string IdReferenceAttribute = "idref";

        internal const string NavigationNamespace = "http://www.daisy.org/z3986/2005/ncx/";

        internal const string HeadElement = "head";

        internal const string MetaElement = "meta";

        internal const string NameAttribute = "name";

        internal const string ContentAttribute = "content";

        internal const string DTBookPrefix = "dtb";

        internal const string DocTitleElement = "docTitle";

        internal const string DocAuthorElement = "docAuthor";

        internal const string Text = "text";

        internal const string NavigationPrefix = "ncx";

        internal const string ContainerFileName = @"META-INF\container.xml";

        internal const string NavigationMapElement = "navMap";

        internal const string NavigationPointElement = "navPoint";

        internal const string NavigationLabelElement = "navLabel";

        internal const string SourceAttribute = "src";

        internal const string PlayOrder = "playOrder";

        internal const string NavigationPubid = "-//NISO//DTD ncx 2005-1//EN";

        internal const string NavigationSysid = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
    }
}

#endif