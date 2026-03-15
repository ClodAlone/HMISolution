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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Constants
{
  /// <summary>
  /// This class contains constants related with external links storing in Excel 2007 format.
  /// </summary>
  public sealed class ExternalLinks
  {
    #region Constants
    /// <summary>
    /// Main namespace for external links file.
    /// </summary>
    public const string Namespace = "";
    /// <summary>
    /// This element is a container for specific types of external links.
    /// </summary>
    public const string ExternalLinkTag = "externalLink";
    /// <summary>
    /// This element represents an external workbook which is supplying data to the current workbook.
    /// </summary>
    public const string ExternalBookTag = "externalBook";
    /// <summary>
    /// This element is the container for all of the worksheet names in a supporting workbook.
    /// </summary>
    public const string SheetNamesTag = "sheetNames";
    /// <summary>
    /// Name of a worksheet in the supporting workbook.
    /// </summary>
    public const string SheetNameTag = "sheetName";
    /// <summary>
    /// This element defines the collection of external references for this workbook.
    /// </summary>
    public const string ExternalReferencesTag = "externalReferences";
    /// <summary>
    /// This element defines an external reference that stores data for workbook elements.
    /// </summary>
    public const string ExternalReferenceTag = "externalReference";
    /// <summary>
    /// Name of the xml attribute that stores sheet name value.
    /// </summary>
    public const string ExternalSheetNameAttribute = "val";
    /// <summary>
    /// This element serves as the collection for 1 or more sheetData elements.
    /// </summary>
    public const string SheetDataSetTag = "sheetDataSet";
    /// <summary>
    /// This element is a collection of the defined names associated with the supporting workbook.
    /// </summary>
    public const string DefinedNamesTag = "definedNames";
    /// <summary>
    /// This element contains information about a named range in an external workbook.
    /// </summary>
    public const string DefinedNameTag = "definedName";
    /// <summary>
    /// The defined name attribute.
    /// </summary>
    public const string NameAttribute = "name";
    /// <summary>
    /// Name range definition string.
    /// </summary>
    public const string RefersToAttribute = "refersTo";
    /// <summary>
    /// The index of the worksheet that the named range applies to for named ranges
    /// that are scoped to a particular worksheet rather than the full workbook.
    /// </summary>
    public const string SheetIdAttribute = "sheetId";
    public const string CellTag = "cell";
    public const string OleLink = "oleLink";
    public const string DdeLink = "ddeLink";
    public const string OleItems = "oleItems";
    public const string OleItem = "oleItem";
    public const string IconAttribute = "icon";
    public const string AdviseAttribute = "advise";
    public const string PreferPictureAttribute = "preferPic";
    #endregion
  }
}
