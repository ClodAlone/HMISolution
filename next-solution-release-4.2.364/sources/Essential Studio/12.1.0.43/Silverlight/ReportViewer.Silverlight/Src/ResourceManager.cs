#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RESX = Syncfusion.Windows.Report.Viewer.Properties.Resources;

namespace Syncfusion.Windows.Report.Viewer.Utils
{    
    public class ResourceManager
    {
        public ResourceManager()
            {
            }

            /// <summary>
            ///   Looks up a localized string similar to Cancel.
            /// </summary>
            public string btnCancel
            {
                get
                {
                    return RESX.btnCancel;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Report Issue.
            /// </summary>
            public string btnIssue
            {
                get
                {
                    return RESX.btnIssue;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to OK.
            /// </summary>
            public string btnOK
            {
                get
                {
                    return RESX.btnOK;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Do you want report this issue?.
            /// </summary>
            public string btnToolTipIssue
            {
                get
                {
                    return RESX.btnToolTipIssue;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to View Report.
            /// </summary>
            public string btnViewReport
            {
                get
                {
                    return RESX.btnViewReport;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Dimension Preview.
            /// </summary>
            public string headerDimensionPreview
            {
                get
                {
                    return RESX.headerDimensionPreview;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Orientation.
            /// </summary>
            public string headerOrientation
            {
                get
                {
                    return RESX.headerOrientation;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Paper.
            /// </summary>
            public string headerPaper
            {
                get
                {
                    return RESX.headerPaper;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Bottom :.
            /// </summary>
            public string labelBottom
            {
                get
                {
                    return RESX.labelBottom;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Left :.
            /// </summary>
            public string labelLeft
            {
                get
                {
                    return RESX.labelLeft;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Right :.
            /// </summary>
            public string labelRight
            {
                get
                {
                    return RESX.labelRight;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Size :.
            /// </summary>
            public string labelSize
            {
                get
                {
                    return RESX.labelSize;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Source :.
            /// </summary>
            public string labelSource
            {
                get
                {
                    return RESX.labelSource;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Top :.
            /// </summary>
            public string labelTop
            {
                get
                {
                    return RESX.labelTop;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Export.
            /// </summary>
            public string menuItemExport
            {
                get
                {
                    return RESX.menuItemExport;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Export to Excel.
            /// </summary>
            public string menuItemExportToExcel
            {
                get
                {
                    return RESX.menuItemExportToExcel;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Export to Pdf.
            /// </summary>
            public string menuItemExportToPDF
            {
                get
                {
                    return RESX.menuItemExportToPDF;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Export to Xps.
            /// </summary>
            public string menuItemExportToXps
            {
                get
                {
                    return RESX.menuItemExportToXps;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to First.
            /// </summary>
            public string menuItemFirst
            {
                get
                {
                    return RESX.menuItemFirst;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Last.
            /// </summary>
            public string menuItemLast
            {
                get
                {
                    return RESX.menuItemLast;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Next.
            /// </summary>
            public string menuItemNext
            {
                get
                {
                    return RESX.menuItemNext;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Normal Layout.
            /// </summary>
            public string menuItemNormalLayout
            {
                get
                {
                    return RESX.menuItemNormalLayout;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Open.
            /// </summary>
            public string menuItemOpen
            {
                get
                {
                    return RESX.menuItemOpen;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Previous.
            /// </summary>
            public string menuItemPrevious
            {
                get
                {
                    return RESX.menuItemPrevious;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Print.
            /// </summary>
            public string menuItemPrint
            {
                get
                {
                    return RESX.menuItemPrint;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Print Layout.
            /// </summary>
            public string menuItemPrintLayout
            {
                get
                {
                    return RESX.menuItemPrintLayout;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Refresh.
            /// </summary>
            public string menuItemRefresh
            {
                get
                {
                    return RESX.menuItemRefresh;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Show /Hide Document.
            /// </summary>
            public string menuItemShowHide
            {
                get
                {
                    return RESX.menuItemShowHide;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Land Scape.
            /// </summary>
            public string optionLandScape
            {
                get
                {
                    return RESX.optionLandScape;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Portrait.
            /// </summary>
            public string optionPortrait
            {
                get
                {
                    return RESX.optionPortrait;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Password:.
            /// </summary>
            public string textBlockPassword
            {
                get
                {
                    return RESX.textBlockPassword;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to User name:.
            /// </summary>
            public string textBlockUserName
            {
                get
                {
                    return RESX.textBlockUserName;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Show Details.
            /// </summary>
            public string tglBtnShowDetails
            {
                get
                {
                    return RESX.tglBtnShowDetails;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Enter Data Source Credentials.
            /// </summary>
            public string titleCredentialsWindow
            {
                get
                {
                    return RESX.titleCredentialsWindow;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to PageSetup.
            /// </summary>
            public string titlePageSetup
            {
                get
                {
                    return RESX.titlePageSetup;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Back To Parent.
            /// </summary>
            public string toolTipBackToParent
            {
                get
                {
                    return RESX.toolTipBackToParent;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Export to Excel.
            /// </summary>
            public string toolTipExportToExcel
            {
                get
                {
                    return RESX.toolTipExportToExcel;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Export to Pdf.
            /// </summary>
            public string toolTipExportToPDF
            {
                get
                {
                    return RESX.toolTipExportToPDF;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Export to Xps.
            /// </summary>
            public string toolTipExportToXps
            {
                get
                {
                    return RESX.toolTipExportToXps;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Find.
            /// </summary>
            public string toolTipFind
            {
                get
                {
                    return RESX.toolTipFind;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Find Next.
            /// </summary>
            public string toolTipFindNext
            {
                get
                {
                    return RESX.toolTipFindNext;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Find text in the report..
            /// </summary>
            public string toolTipFindTextBox
            {
                get
                {
                    return RESX.toolTipFindTextBox;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Switch to normal layout.
            /// </summary>
            public string toolTipNormalLayout
            {
                get
                {
                    return RESX.toolTipNormalLayout;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Page Setup.
            /// </summary>
            public string toolTipPageSetUp
            {
                get
                {
                    return RESX.toolTipPageSetUp;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Parameters.
            /// </summary>
            public string toolTipParameters
            {
                get
                {
                    return RESX.toolTipParameters;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Switch to print layout.
            /// </summary>
            public string toolTipPrintLayout
            {
                get
                {
                    return RESX.toolTipPrintLayout;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Refresh.
            /// </summary>
            public string toolTipRefresh
            {
                get
                {
                    return RESX.toolTipRefresh;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Stop.
            /// </summary>
            public string toolTipStop
            {
                get
                {
                    return RESX.toolTipStop;
                }
            }

            /// <summary>
            ///   Looks up a localized string similar to Zoom.
            /// </summary>
            public string toolTipZoom
            {
                get
                {
                    return RESX.toolTipZoom;
                }
            }
        }
}
