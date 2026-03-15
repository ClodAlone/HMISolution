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
#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif
#endregion

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Represents an Excel worksheet Tab.
	/// </summary>
	public interface ITabSheet : IParentApplication
	{
    #region Interface properties
    /// <summary>
    /// Gets / sets tab color.
    /// </summary>
    ExcelKnownColors TabColor { get; set; }   
    /// <summary>
    /// Gets / sets tab color.
    /// </summary>
    Color TabColorRGB { get; set; }   
    /// <summary>
    /// Returns charts collection. Read-only.
    /// </summary>
    IChartShapes    Charts { get; }
    /// <summary>
    /// Returns pictures collection. Read-only.
    /// </summary>
    IPictures       Pictures { get; }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    IWorkbook       Workbook { get; }
    /// <summary>
    /// Returns shapes collection. Read-only.
    /// </summary>
    IShapes         Shapes { get; }
    /// <summary>
    /// Indicates whether worksheet is displayed right to left.
    /// </summary>
    bool IsRightToLeft { get; set; }
    /// <summary>
    /// Indicates whether tab of this sheet is selected. Read-only.
    /// </summary>
    bool IsSelected { get; }
    /// <summary>
    /// Returns index in the parent ITabSheets collection. Read-only.
    /// </summary>
    int TabIndex { get; }
    /// <summary>
    /// Gets / sets name of the tab sheet.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Control visibility of worksheet to end user.
    /// </summary>
    WorksheetVisibility Visibility{ get; set; }
    /// <summary>
    /// Returns collection with all textboxes inside this worksheet. Read-only.
    /// </summary>
    ITextBoxes TextBoxes { get; }
    /// <summary>
    /// Returns collection with all checkboxes inside this worksheet. Read-only.
    /// </summary>
    ICheckBoxes CheckBoxes { get; }
    /// <summary>
    /// Returns collection with all option buttons inside this worksheet. Read-only.
    /// </summary>
    IOptionButtons OptionButtons { get; }
    /// <summary>

    /// Returns collection with all comboboxes inside this worksheet. Read-only.
    /// </summary>
    IComboBoxes ComboBoxes { get; }
    /// <summary>
    /// Name used by macros to access workbook items. Read-only.
    /// </summary>
    string CodeName { get; }
    /// <summary>
    /// Indicates is current sheet is protected.
    /// </summary>
    bool            ProtectContents { get; }
    /// <summary>
    /// True if objects are protected. Read-only.
    /// </summary>
    bool            ProtectDrawingObjects{ get; }
    /// <summary>
    /// True if the scenarios of the current sheet are protected. Read-only.
    /// </summary>
    bool            ProtectScenarios { get; }
    /// <summary>
    /// Gets protected options. Read-only. For sets protection options use "Protect" method.
    /// </summary>
    ExcelSheetProtection Protection { get; }
    /// <summary>
    /// Indicates if the worksheet is password protected.
    /// </summary>
    bool            IsPasswordProtected { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Makes the current sheet the active sheet. Equivalent to clicking the
    /// sheet's tab.
    /// </summary>
    void Activate();
    /// <summary>
    /// Selects current tab sheet.
    /// </summary>
    void Select();
    /// <summary>
    /// Unselects current tab sheet.
    /// </summary>
    void Unselect();
    /// <summary>
    /// Protects worksheet's content with password.
    /// </summary>
    /// <param name="password">Password to protect with.</param>
    void Protect( string password );
    /// <summary>
    /// Protects current worksheet.
    /// </summary>
    /// <param name="password">Represents password to protect.</param>
    /// <param name="options">Represents params to protect.</param>
    void Protect( string password, ExcelSheetProtection options );
    /// <summary>
    /// Unprotects worksheet's content with password.
    /// </summary>
    /// <param name="password">Password to unprotect.</param>
    void Unprotect( string password );
    #endregion
  }
}
