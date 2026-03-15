// <copyright file="FolderBrowser.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a dialog box that allows the user to choose a folder.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FolderBrowser : CommonDialog
    {
        #region Enums
        /// <summary>
        /// Messages for FolderBrowser interaction.
        /// </summary>
        private enum FolderBrowserMessages
        {
            /// <summary>
            /// Represents initialized
            /// </summary>
            BFFM_INITIALIZED = 1,
            
            /// <summary>
            /// Represents changed
            /// </summary>
            BFFM_SELCHANGED = 2,
            
            /// <summary>
            /// Represents validation 
            /// </summary>
            BFFM_VALIDATEFAILEDA = 3,
            
            /// <summary>
            /// Represents validation fail
            /// </summary>
            BFFM_VALIDATEFAILEDW = 4,
            
            /// <summary>
            /// Represents unknown
            /// </summary>
            BFFM_IUNKNOWN = 5,

            /// <summary>
            /// Represents set status
            /// </summary>
            BFFM_SETSTATUSTEXTA = (0x0400 + 100),
            
            /// <summary>
            /// Represents  enable ok
            /// </summary>
            BFFM_ENABLEOK = (0x0400 + 101),
            
            /// <summary>
            /// Represents set selection
            /// </summary>
            BFFM_SETSELECTIONA = (0x0400 + 102),
            
            /// <summary>
            /// Represents set selectionw
            /// </summary>
            BFFM_SETSELECTIONW = (0x0400 + 103),
            
            /// <summary>
            /// Represents set statustrest
            /// </summary>
            BFFM_SETSTATUSTEXTW = (0x0400 + 104),
            
            /// <summary>
            /// Represents set ok test
            /// </summary>
            BFFM_SETOKTEXT = (0x0400 + 105),
            
            /// <summary>
            /// Represents set expanded
            /// </summary>
            BFFM_SETEXPANDED = (0x0400 + 106),
            
            /// <summary>
            /// Represents set text
            /// </summary>
            WM_SETTEXT = 0x000C,
            
            /// <summary>
            /// Represents get text
            /// </summary>
            WM_GETTEXT = 0x000D,
            
            /// <summary>
            /// Represents disabled
            /// </summary>
            WS_DISABLED = 0x8000000,
            
            /// <summary>
            /// Represents set read only
            /// </summary>
            EM_SETREADONLY = 0xcf,
            
            /// <summary>
            /// Represents  set style
            /// </summary>
            GWL_STYLE = -16,
        }
        
        /// <summary>
        /// Options which provides a way for manipulations with FolderBrowser view and functional settings. Used in the BROWSEINFO.ulFlags field.
        /// </summary>
        [Flags]
        private enum BffStyles
        {
            /// <summary>
            /// Only return file system directories. If the user selects folders that are not part of the file system, the OK button is grayed.
            /// </summary>
            RestrictToFilesystem = 0x0001, // BIF_RETURNONLYFSDIRS
            
            /// <summary>
            /// Do not include network folders below the domain level in the dialog box's tree view control.
            /// </summary>
            RestrictToDomain = 0x0002, // BIF_DONTGOBELOWDOMAIN
            
            /// <summary>
            /// Set this flag to enable status text area in the dialog
            /// This flag is not supported when NewDialogStyle is specified.
            /// </summary>
            ShowStatusText = 0x0004,
            
            /// <summary>
            /// Only return file system ancestors. An ancestor is a subfolder that is beneath the root folder in the namespace hierarchy. If the user selects an ancestor of the root folder that is not part of the file system, the OK button is grayed.
            /// </summary>
            RestrictToSubfolders = 0x0008, // BIF_RETURNFSANCESTORS
            
            /// <summary>
            ///  Include an edit control in the browse dialog box that allows the user to type the name of an item.
            /// </summary>
            ShowEditBox = 0x0010, // BIF_EDITBOX
            
            /// <summary>
            /// If the user types an invalid name into the edit box, the browse dialog box will call the application's BrowseCallbackProc with the BFFM_VALIDATEFAILED message. This flag is ignored if BIF_EDITBOX is not specified.
            /// </summary>
            ValidateSelection = 0x0020, // BIF_VALIDATE
            
            /// <summary>
            ///  Use the new user interface. Setting this flag provides the user with a larger dialog box that can be resized.
            /// </summary>
            NewDialogStyle = 0x0040, // BIF_NEWDIALOGSTYLE
            
            /// <summary>
            /// Use the new user interface, including an edit box. This flag is equivalent to BIF_EDITBOX | BIF_NEWDIALOGSTYLE. 
            /// </summary>
            UseNewUI = (NewDialogStyle | ShowEditBox), // BIF_USENEWUI
            
            /// <summary>
            /// The browse dialog box can display URLs. The BIF_USENEWUI and BIF_BROWSEINCLUDEFILES flags must also be set. 
            /// <remarks>If these three flags are not set, the browser dialog box will reject URLs. Even when these flags are set, the browse dialog box will only display URLs if the folder that contains the selected item supports them.</remarks>
            /// </summary>
            BrowseIncludeURL = 0x0080, // BIF_BROWSEINCLUDEURLS
            
            /// <summary>
            /// When combined with BIF_NEWDIALOGSTYLE, adds a usage hint to the dialog box in place of the edit box. BIF_EDITBOX overrides this flag.
            /// </summary>
            ShowHint = 0x0100, // BIF_UAHINT
            
            /// <summary>
            /// Do not add the "New Folder" button to the dialog.  Only applicable with BIF_NEWDIALOGSTYLE.
            /// </summary>
            NoNewFolderButton = 0x0200, // BIF_NONEWFOLDERBUTTON                
            
            /// <summary>
            /// Don't traverse target as shortcut
            /// </summary>                
            NoTranslateTargets = 0x0400, // BIF_NOTRANSLATETARGETS
            
            /// <summary>
            /// Only return computers. If the user selects anything other than a computer, the OK button is grayed.
            /// </summary>
            BrowseForComputer = 0x1000, // BIF_BROWSEFORCOMPUTER
            
            /// <summary>
            /// Only allow the selection of printers. If the user selects anything other than a printer, the OK button is grayed. 
            /// </summary>
            BrowseForPrinter = 0x2000, // BIF_BROWSEFORPRINTER
            
            /// <summary>
            /// The browse dialog box will display files as well as folders
            /// </summary>
            BrowseForEverything = 0x4000, // BIF_BROWSEINCLUDEFILES                                
            
            /// <summary>
            /// The browse dialog box can display shareable resources on remote systems.
            /// <remarks>The NewDialogStyle flag must also be set.</remarks>
            /// </summary>
            ShowShareble = 0x8000, // BIF_SHAREABLE   
        }
        #endregion

        #region Constants
        /// <summary>
        /// Identifier of IFolderFilterSite interface.
        /// <remarks>For more information see http://msdn2.microsoft.com/en-us/library/ms645490.aspx</remarks>
        /// </summary>
        private const string StrIFolderFilterSiteGuid = "{C0A651F5-B48B-11d2-B5ED-006097C686F6}";
        
        /// <summary>
        /// Max path length.
        /// </summary>
        private const int MAX_PATH = 260;
        #endregion

        #region Private members
        /// <summary>
        /// Identifier of IFolderFilterSite interface guid.        
        /// </summary>
        private static Guid m_iFolderFilterSiteGuid = new Guid(StrIFolderFilterSiteGuid);
        
        /// <summary>
        /// Address of CallBack function that the dialog box calls when an event occurs.
        /// </summary>
        private IntPtr m_ptrBrowseCallbackProc = IntPtr.Zero;
        
        /// <summary>
        /// CallBack delegate.
        /// </summary>
        private Interop.BFFCALLBACK m_delegateCallBack;
        
        /// <summary>
        /// Handle interptr
        /// </summary>
        private IntPtr m_handle;
        
        /// <summary>
        /// Edit box handle.
        /// </summary>
        private IntPtr m_editHandle;
        
        /// <summary>
        /// path to the root of FolderBrowser tree
        /// </summary>
        private LocationID m_rootLocation = LocationID.Desktop;
        
        /// <summary>
        /// Custom path to the folder which will be shown as a start root
        /// </summary>
        private string m_rootPath;
        
        /// <summary>
        /// Text that will be shown as a dialog title
        /// </summary>
        private string m_dialogTitle;
        
        /// <summary>
        /// Folder chosen by the user.
        /// </summary>
        private string m_selectedDirectory;
        
        /// <summary>
        /// Description text to be show under the tree.
        /// </summary>
        private string m_descriptionText;
        
        /// <summary>
        /// path to the directory which will be selected when the dialog appears.
        /// </summary>
        private string m_startSelectedDirectory;
        
        /// <summary>
        /// path to the directory which be expanded when the dialog appears.
        /// </summary>
        private string m_startExpandedDirectory;
        
        /// <summary>
        /// Defines whether to show path textbox as read-only or not.
        /// </summary>
        private bool m_isEditBoxReadOnly;
        
        /// <summary>
        /// Defines whether to show path textbox as disabled or not.
        /// </summary>
        private bool m_isEditBoxDisabled;
        
        /// <summary>
        /// Defines whether to show full path in the edit box or not.
        /// </summary>
        private bool m_showFullPath;
        
        /// <summary>
        /// The browse dialog box can display shareable resources on remote systems. UseNewDialogStyle must also be set to true.
        /// </summary>     
        private bool m_showShareable;
        
        /// <summary>
        /// Only return printers. If the user selects anything other than a computer, the OK button is grayed.
        /// </summary>
        private bool m_browseForPrinterOnly;
        
        /// <summary>
        /// Only return computers. If the user selects anything other than a computer, the OK button is grayed.
        /// </summary>
        private bool m_browseForComputerOnly;
        
        /// <summary>
        /// If the user types an invalid name into the edit box, the browse dialog box will raise the ValidateFailed event.
        /// It is ignored if ShowEditBox is not set to true.
        /// </summary>
        private bool m_validateSelection;
        
        /// <summary>
        ///  The browse dialog box will display files as well as folders.
        /// </summary>
        private bool m_browseForEverything;
        
        /// <summary>
        /// Do not include network folders below the domain level in the dialog box's tree view control.
        /// </summary>
        private bool m_restrictToDomain;
        
        /// <summary>
        /// The browse dialog box can display URLs. The UseNewUI and BrowseForEverything flags must also be set. 
        /// <remarks>If these three flags are not set, the browser dialog box will reject URLs. Even when these flags are set, the browse dialog box will only display URLs if the folder that contains the selected item supports them.</remarks>
        /// </summary>
        private bool m_browseIncludeURL;
        
        /// <summary>
        /// Include an edit control in the browse dialog box that allows the user to type the name of an item.
        /// </summary>
        private bool m_showEditBox;
        
        /// <summary>
        ///  Use the new user interface. Setting this flag provides the user with a larger dialog box that can be resized.
        /// </summary>
        private bool m_useNewDialogStyle;
        
        /// <summary>
        /// Defines whether include the New Folder button in the browse dialog or not.
        /// </summary>
        private bool m_showNewFolderButton = true;
        
        /// <summary>
        /// Don't traverse target as shortcut if true.
        /// </summary>                
        private bool m_noTranslateTargets;
        
        /// <summary>
        /// Only return file system ancestors. An ancestor is a subfolder that is beneath the root folder in the namespace hierarchy. If the user selects an ancestor of the root folder that is not part of the file system, the OK button is grayed.
        /// </summary>
        private bool m_restrictToSubfolders;
        
        /// <summary>
        /// Only return file system directories. If the user selects folders that are not part of the file system, the OK button is grayed.
        /// </summary>
        private bool m_restrictToFilesystem = true;
        
        /// <summary>
        /// If set to true, enables status text area in the dialog
        /// This is not supported when UseNewDialogStyle is set to true.
        /// </summary>
        private bool m_showStatusText;
        
        /// <summary>
        /// Status text that can be shown if old dialog style is using and ShowStatusText is set to true. 
        /// </summary>
        private string m_statusText;
        
        /// <summary>
        /// When UseNewDialogStyle is set to true, adds a usage hint to the dialog box in place of the edit box. ShowEditBox sets to true overrides this property.
        /// </summary>
        private bool m_showHint;
        
        /// <summary>
        /// Use the new user interface, including an edit box. This is equivalent to UseNewDialogStyle and ShowEditBox set to true. 
        /// </summary>
        private bool m_useNewUI;
        
        /// <summary>
        /// Specifies file filter extensions.
        /// </summary>
        /// <remarks>
        /// Works only if UseNewUi and BrowseForEverything are set to true.
        /// </remarks>
        private string[] m_filterExtensions;
        
        /// <summary>
        /// Extension filter.
        /// </summary>
        private FilterByExtension filter = new FilterByExtension();
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets m_RootLocation. path to the root of FolderBrowser tree
        /// </summary>
        public LocationID RootLocation
        {
            get
            {
                return m_rootLocation;
            }

            set
            {
                new UIPermission(UIPermissionWindow.AllWindows).Demand();
                m_rootLocation = value;
            }
        }
        
        /// <summary>
        /// Gets or sets m_RootPath Custom path to the folder which will be shown as a start root
        /// </summary>
        public string RootPath
        {
            get
            {
                return m_rootPath;
            }

            set
            {
                m_rootPath = value;
            }
        }
        
        /// <summary>
        /// Gets or sets m_SelectedDirectory. Folder chosen by the user.
        /// </summary>
        public string SelectedDirectory
        {
            get
            {
                return m_selectedDirectory;
            }

            set
            {
                m_selectedDirectory = value;
            }
        }
        
        /// <summary>
        /// Gets or sets Description m_DescriptionText. text to be show under the tree.
        /// </summary>
        public string DescriptionText
        {
            get
            {
                return m_descriptionText;
            }

            set
            {
                m_descriptionText = value;
            }
        }
        
        /// <summary>
        /// Gets or sets m_StartSelectedDirectory. path to the directory which will be selected when the dialog appears.
        /// </summary>
        public string StartSelectedDirectory
        {
            get
            {
                return m_startSelectedDirectory;
            }

            set
            {
                m_startSelectedDirectory = value;
            }
        }
        
        /// <summary>
        /// Gets or sets m_StartExpandedDirectory. path to the directory which be expanded when the dialog appears.
        /// </summary>
        public string StartExpandedDirectory
        {
            get
            {
                return m_startExpandedDirectory;
            }

            set
            {
                m_startExpandedDirectory = value;
            }
        }
        
        /// <summary>
        /// Gets or sets m_DialogTitle. Text that will be shown as a dialog title
        /// </summary>
        public string DialogTitle
        {
            get
            {
                return m_dialogTitle;
            }

            set
            {
                m_dialogTitle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show full path].
        /// </summary>
        /// <value><c>true</c> if [show full path]; otherwise, <c>false</c>.</value>
        public bool ShowFullPath
        {
            get
            {
                return m_showFullPath;
            }

            set
            {
                m_showFullPath = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_IsEditBoxReadOnly. Defines whether to show path textbox as read-only or not.
        /// </summary>
        public bool IsEditBoxReadOnly
        {
            get
            {
                return m_isEditBoxReadOnly;
            }

            set
            {
                m_isEditBoxReadOnly = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_IsEditBoxDisabled. Defines whether to show path textbox as disabled or not.
        /// </summary>
        public bool IsEditBoxDisabled
        {
            get
            {
                return m_isEditBoxDisabled;
            }

            set
            {
                m_isEditBoxDisabled = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_BrowseForComputerOnly. Only return computers. If the user selects anything other than a computer, the OK button is grayed.
        /// </summary>
        public bool BrowseForComputerOnly
        {
            get
            {
                return m_browseForComputerOnly;
            }

            set
            {
                m_browseForComputerOnly = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_BrowseForPrinterOnly. Only return printers. If the user selects anything other than a computer, the OK button is grayed.
        /// </summary>
        public bool BrowseForPrinterOnly
        {
            get
            {
                return m_browseForPrinterOnly;
            }

            set
            {
                m_browseForPrinterOnly = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_ShowShareable. The browse dialog box can display shareable resources on remote systems. UseNewDialogStyle must also be set to true.
        /// </summary>
        public bool ShowShareable
        {
            get
            {
                return m_showShareable;
            }

            set
            {
                m_showShareable = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_ValidateSelection. If the user types an invalid name into the edit box, the browse dialog box will raise the ValidateFailed event.
        /// It is ignored if ShowEditBox is not set to true.
        /// </summary>
        public bool ValidateSelection
        {
            get
            {
                return m_validateSelection;
            }

            set
            {
                m_validateSelection = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_BrowseForEverything. The browse dialog box will display files as well as folders.
        /// </summary>
        public bool BrowseForEverything
        {
            get
            {
                return m_browseForEverything;
            }

            set
            {
                m_browseForEverything = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_RestrictToDomain. Do not include network folders below the domain level in the dialog box's tree view control.
        /// </summary>
        public bool RestrictToDomain
        {
            get
            {
                return m_restrictToDomain;
            }

            set
            {
                m_restrictToDomain = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_BrowseIncludeURL. The browse dialog box can display URLs. The UseNewUI and BrowseForEverything flags must also be set. 
        /// <remarks>If these three flags are not set, the browser dialog box will reject URLs. Even when these flags are set, the browse dialog box will only display URLs if the folder that contains the selected item supports them.</remarks>
        /// </summary>
        public bool BrowseIncludeURL
        {
            get
            {
                return m_browseIncludeURL;
            }

            set
            {
                m_browseIncludeURL = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_ShowEditBox. Include an edit control in the browse dialog box that allows the user to type the name of an item.
        /// </summary>
        public bool ShowEditBox
        {
            get
            {
                return m_showEditBox;
            }

            set
            {
                m_showEditBox = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_UseNewDialogStyle. Use the new user interface. Setting this flag provides the user with a larger dialog box that can be resized.
        /// </summary>
        public bool UseNewDialogStyle
        {
            get
            {
                return m_useNewDialogStyle;
            }

            set
            {
                m_useNewDialogStyle = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_ShowNewFolderButton. Defines whether include the New Folder button in the browse dialog or not.
        /// </summary>
        public bool ShowNewFolderButton
        {
            get
            {
                return m_showNewFolderButton;
            }

            set
            {
                m_showNewFolderButton = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_NoTranslateTargets. Don't traverse target as shortcut if true.
        /// </summary>                
        public bool NoTranslateTargets
        {
            get
            {
                return m_noTranslateTargets;
            }

            set
            {
                m_noTranslateTargets = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_RestrictToSubfolders. Only return file system ancestors. An ancestor is a subfolder that is beneath the root folder in the namespace hierarchy. If the user selects an ancestor of the root folder that is not part of the file system, the OK button is grayed.
        /// </summary>
        public bool RestrictToSubfolders
        {
            get
            {
                return m_restrictToSubfolders;
            }

            set
            {
                m_restrictToSubfolders = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_RestrictToFilesystem. Only return file system directories. If the user selects folders that are not part of the file system, the OK button is grayed.
        /// </summary>
        public bool RestrictToFilesystem
        {
            get
            {
                return m_restrictToFilesystem;
            }

            set
            {
                m_restrictToFilesystem = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_ShowStatusText. If set to true, enables status text area in the dialog
        /// This is not supported when UseNewDialogStyle is set to true.
        /// </summary>
        public bool ShowStatusText
        {
            get
            {
                return m_showStatusText;
            }

            set
            {
                m_showStatusText = value;
            }
        }
        
        /// <summary>
        /// Gets or sets m_StatusText. Status text that can be shown if old dialog style is using and ShowStatusText is set to true. 
        /// </summary>
        public string StatusText
        {
            get
            {
                return m_statusText;
            }

            set
            {
                m_statusText = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_ShowHint. When UseNewDialogStyle is set to true, adds a usage hint to the dialog box in place of the edit box. ShowEditBox sets to true overrides this property.
        /// </summary>
        public bool ShowHint
        {
            get
            {
                return m_showHint;
            }

            set
            {
                m_showHint = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether m_UseNewUI. Use the new user interface, including an edit box. This is equivalent to UseNewDialogStyle and ShowEditBox set to true. 
        /// </summary>
        public bool UseNewUI
        {
            get
            {
                return m_useNewUI;
            }

            set
            {
                m_useNewUI = value;
                if (value == true)
                {
                    m_showEditBox = true;
                    m_useNewDialogStyle = true;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets m_FilterExtensions. Specifies file filter extensions.
        /// </summary>
        /// <remarks>
        /// Works only if UseNewUI and BrowseForEverything are set to true.
        /// </remarks>
        public string[] FilterExtensions
        {
            get
            {
                return m_filterExtensions;
            }

            set
            {
                m_filterExtensions = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowser"/> class.
        /// </summary>
        public FolderBrowser()
        {
            m_useNewDialogStyle = true;
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(FolderBrowser));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowser"/> class.
        /// </summary>
        /// <param name="rootLocation">The root location.</param>
        public FolderBrowser(LocationID rootLocation)
            : this()
        {
            m_rootLocation = rootLocation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowser"/> class.
        /// </summary>
        /// <param name="rootLocation">The root location.</param>
        /// <param name="descriptionText">The description text.</param>
        public FolderBrowser(LocationID rootLocation, string descriptionText)
            : this(rootLocation)
        {
            m_descriptionText = descriptionText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowser"/> class.
        /// </summary>
        /// <param name="rootLocationPath">The root location path.</param>
        public FolderBrowser(string rootLocationPath)
            : this()
        {
            m_rootLocation = LocationID.Custom;
            m_rootPath = rootLocationPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowser"/> class.
        /// </summary>
        /// <param name="rootLocationPath">The root location path.</param>
        /// <param name="descriptionText">The description text.</param>
        public FolderBrowser(string rootLocationPath, string descriptionText)
            : this(rootLocationPath)
        {
            m_descriptionText = descriptionText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowser"/> class.
        /// </summary>
        /// <param name="rootLocation">The root location.</param>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="selectedOrExpanded">if set to <c>true</c> [selected or expanded].</param>
        /// <param name="descriptionText">The description text.</param>
        public FolderBrowser(LocationID rootLocation, string directoryPath, bool selectedOrExpanded, string descriptionText)
            : this(rootLocation, descriptionText)
        {
            if (selectedOrExpanded)
            {
                m_startSelectedDirectory = directoryPath;
            }
            else
            {
                m_startExpandedDirectory = directoryPath;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowser"/> class.
        /// </summary>
        /// <param name="rootLocationPath">The root location path.</param>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="selectedOrExpanded">if set to <c>true</c> [selected or expanded].</param>
        /// <param name="descriptionText">The description text.</param>
        public FolderBrowser(string rootLocationPath, string directoryPath, bool selectedOrExpanded, string descriptionText)
            : this(rootLocationPath, descriptionText)
        {
            if (selectedOrExpanded)
            {
                m_startSelectedDirectory = directoryPath;
            }
            else
            {
                m_startExpandedDirectory = directoryPath;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets dialog title
        /// </summary>
        private void SetDialogTitle()
        {
            if (m_handle != IntPtr.Zero)
            {
                Interop.SendMessage(m_handle, (uint)FolderBrowserMessages.WM_SETTEXT, 0, m_dialogTitle);
            }
        }
        
        /// <summary>
        /// Sets dialog title
        /// </summary>
        private void GetDialogTitle()
        {
            if (m_handle != IntPtr.Zero)
            {
                StringBuilder title = new StringBuilder(MAX_PATH);
                Interop.SendMessage(m_handle, (uint)FolderBrowserMessages.WM_GETTEXT, (uint)MAX_PATH, title);
                m_dialogTitle = title.ToString();
            }
        }
        
        /// <summary>
        /// Gets flag combination according to the property values.
        /// </summary>
        /// <returns>unsigned int</returns>
        private uint GetFlagsValue()
        {
            BffStyles flags = 0x0000;

            if (BrowseForComputerOnly)
            {
                flags |= BffStyles.BrowseForComputer;
            }

            if (BrowseForPrinterOnly)
            {
                flags |= BffStyles.BrowseForPrinter;
            }

            if (BrowseForEverything)
            {
                flags |= BffStyles.BrowseForEverything;
            }

            if (BrowseIncludeURL)
            {
                flags |= BffStyles.BrowseIncludeURL;
            }

            if (RestrictToDomain)
            {
                flags |= BffStyles.RestrictToDomain;
            }

            if (ShowEditBox)
            {
                flags |= BffStyles.ShowEditBox;
            }

            if (UseNewDialogStyle)
            {
                flags |= BffStyles.NewDialogStyle;
            }

            if (!ShowNewFolderButton)
            {
                flags |= BffStyles.NoNewFolderButton;
            }

            if (NoTranslateTargets)
            {
                flags |= BffStyles.NoTranslateTargets;
            }

            if (RestrictToSubfolders)
            {
                flags |= BffStyles.RestrictToSubfolders;
            }

            if (RestrictToFilesystem)
            {
                flags |= BffStyles.RestrictToFilesystem;
            }

            if (ShowShareable)
            {
                flags |= BffStyles.ShowShareble;
            }

            if (ShowStatusText)
            {
                flags |= BffStyles.ShowStatusText;
            }

            if (ShowHint)
            {
                flags |= BffStyles.ShowHint;
            }

            if (UseNewUI)
            {
                flags |= BffStyles.UseNewUI;
            }

            if (ValidateSelection)
            {
                flags |= BffStyles.ValidateSelection;
            }

            return (uint)flags;
        }
        
        /// <summary>
        /// Enables or disables OK button. Works only after the dialog has been shown.
        /// </summary>
        /// <param name="enabled">If true - enables, false - disables OK button</param>
        public void EnableOkButton(bool enabled)
        {
            if (m_handle != IntPtr.Zero)
            {
                Interop.SendMessage(m_handle, (uint)FolderBrowserMessages.BFFM_ENABLEOK, 0, enabled ? 1 : 0);
            }
        }
        
        /// <summary>
        /// Selects the specified folder. Works only after the dialog has been shown. 
        /// </summary>
        /// <param name="path">Folder path</param>
        public void SelectFolder(string path)
        {
            if (m_handle != IntPtr.Zero)
            {
                Interop.SendMessage(m_handle, (uint)FolderBrowserMessages.BFFM_SETSELECTIONW, 1, path);
            }
        }
        
        /// <summary>
        /// Expands the specified folder. Works only after the dialog has been shown. 
        /// </summary>
        /// <param name="path">Folder path.</param>
        public void ExpandFolder(string path)
        {
            if (m_handle != IntPtr.Zero)
            {
                Interop.SendMessage(m_handle, (uint)FolderBrowserMessages.BFFM_SETEXPANDED, 1, path);
            }
        }
        
        /// <summary>
        /// Sets OK button text. Works only after the dialog has been shown.
        /// </summary>
        /// <param name="text">Specified text</param>
        public void SetOkText(string text)
        {
            if (m_handle != IntPtr.Zero)
            {
                Interop.SendMessage(m_handle, (uint)FolderBrowserMessages.BFFM_SETOKTEXT, 0, text);
            }
        }
        
        /// <summary>
        /// Sets status text. Works only after the dialog has been shown and old dialog style is used.
        /// </summary>
        /// <param name="text">Specified text</param>
        public void SetStatusText(string text)
        {
            if (m_handle != IntPtr.Zero && !m_useNewDialogStyle && m_showStatusText)
            {
                Interop.SendMessage(m_handle, (uint)FolderBrowserMessages.BFFM_SETSTATUSTEXTW, 1, text);
            }
        }
        #endregion

        #region CallBack Handler
        /// <summary>
        /// Delegate type used in BROWSEINFO.lpfn field.
        /// </summary>
        /// <param name="hwnd">The HWND IntPtr.</param>
        /// <param name="uMsg">The dialog box event that generated the message.</param>
        /// <param name="lParam">A value whose meaning depends on the event specified in uMsg.</param>
        /// <param name="lpData">An application-defined value that was specified in the lParam member of the BROWSEINFO structure used in the call to SHBrowseForFolder.</param>
        /// <returns>
        /// Returns zero except in the case of BFFM_VALIDATEFAILED. For that flag, returns zero to dismiss the dialog or nonzero to keep the dialog displayed.
        /// </returns>
        private Int32 BrowseCallbackProc(IntPtr hwnd, UInt32 uMsg, Int32 lParam, Int32 lpData)
        {
            switch ((FolderBrowserMessages)uMsg)
            {
                case FolderBrowserMessages.BFFM_INITIALIZED:
                    OnInitialize(hwnd, lParam);
                    break;

                case FolderBrowserMessages.BFFM_IUNKNOWN:
                    OnIUnknown(hwnd, lParam);
                    break;

                case FolderBrowserMessages.BFFM_SELCHANGED:
                    OnSelectedChanged(hwnd, lParam);
                    break;

                case FolderBrowserMessages.BFFM_VALIDATEFAILEDA:
                    return OnValidationFailed(hwnd, lParam);

                case FolderBrowserMessages.BFFM_VALIDATEFAILEDW:
                    return OnValidationFailed(hwnd, lParam);
            }

            return 0;
        }

        /// <summary>
        /// Invoked when a BFFM_INITIALIZED message is sent through the CallBack function
        /// </summary>
        /// <param name="hwnd">The HWND IntPtr.</param>
        /// <param name="lParama">A value whose meaning depends on the event specified in uMsg.</param>
        private void OnInitialize(IntPtr hwnd, Int32 lParama)
        {
            m_handle = hwnd;

            if (Initialized != null)
            {
                FolderBrowserInitializedEventArgs e = new FolderBrowserInitializedEventArgs(hwnd);
                Initialized(this, e);
            }

            if (m_showEditBox || m_useNewUI)
            {
                m_editHandle = Interop.FindWindowEx(hwnd, IntPtr.Zero, "Edit", string.Empty);
                Trace.WriteLineIf(m_editHandle == IntPtr.Zero, "EditBox can not be found");
            }

            if (m_isEditBoxReadOnly)
            {
                Interop.SendMessage(m_editHandle, (uint)FolderBrowserMessages.EM_SETREADONLY, 1, 0);
            }

            if (m_isEditBoxDisabled && m_editHandle != IntPtr.Zero)
            {
                Int32 iStyle = Interop.GetWindowLong(m_editHandle, (int)FolderBrowserMessages.GWL_STYLE);
                iStyle = iStyle | (Int32)FolderBrowserMessages.WS_DISABLED;
                Interop.SetWindowLong(m_editHandle, (int)FolderBrowserMessages.GWL_STYLE, iStyle);
            }

            if (m_startSelectedDirectory != string.Empty)
            {
                SelectFolder(StartSelectedDirectory);
            }

            if (m_startExpandedDirectory != string.Empty)
            {
                ExpandFolder(StartExpandedDirectory);
            }

            if (m_statusText != string.Empty)
            {
                SetStatusText(m_statusText);
            }

            if (m_dialogTitle != string.Empty)
            {
                SetDialogTitle();
            }
            else
            {
                GetDialogTitle();
            }
        }

        /// <summary>
        /// Invoked when a BFFM_IUNKNOWN message is sent through the CallBack function
        /// </summary>
        /// <param name="hwnd">The HWND IntPtr.</param>
        /// <param name="lParam">A value whose meaning depends on the event specified in uMsg.</param>
        private void OnIUnknown(IntPtr hwnd, Int32 lParam)
        {
            IntPtr iUnknown = (IntPtr)lParam;

            if (IUnknown != null)
            {
                FolderBrowserIUnknownEventArgs e = new FolderBrowserIUnknownEventArgs(hwnd, iUnknown);
                IUnknown(this, e);
            }

            if (iUnknown != IntPtr.Zero && BrowseForEverything && UseNewUI && m_filterExtensions != null)
            {
                IntPtr iFolderFilterSite;
                int iErr = Marshal.QueryInterface(iUnknown, ref m_iFolderFilterSiteGuid, out iFolderFilterSite);

                if (iErr != 0)
                {
                    throw Marshal.GetExceptionForHR(iErr);
                }

                Object obj = Marshal.GetTypedObjectForIUnknown(iFolderFilterSite, typeof(IFolderFilterSite));
                IFolderFilterSite folderFilterSite = (IFolderFilterSite)obj;
                filter.ValidExtension = m_filterExtensions;
                folderFilterSite.SetFilter(filter);
            }
        }

        /// <summary>
        /// Invoked when a BFFM_SELCHANGED message is sent through the CallBack function
        /// </summary>
        /// <param name="hwnd">The HWND IntPtr.</param>
        /// <param name="lParam">A value whose meaning depends on the event specified in uMsg.</param>
        private void OnSelectedChanged(IntPtr hwnd, Int32 lParam)
        {
            StringBuilder sb = new StringBuilder(MAX_PATH);
            Interop.Shell32.SHGetPathFromIDList((IntPtr)lParam, sb);

            if (SelectedChanged != null)
            {
                FolderBrowserSelectionChangedEventArgs e = new FolderBrowserSelectionChangedEventArgs(hwnd, sb.ToString());
                SelectedChanged(this, e);
            }

            if (m_showFullPath && m_editHandle != IntPtr.Zero)
            {
                Interop.SendMessage(m_editHandle, (uint)FolderBrowserMessages.WM_SETTEXT, 0, sb.ToString());
            }
        }

        /// <summary>
        /// Invoked when a BFFM_VALIDATEFAILEDA or BFFM_VALIDATEFAILEDW message is sent through the CallBack function
        /// </summary>
        /// <param name="hwnd">The HWND IntPtr.</param>
        /// <param name="lParam">A value whose meaning depends on the event specified in uMsg.</param>
        /// <returns>int value type</returns>
        private int OnValidationFailed(IntPtr hwnd, Int32 lParam)
        {
            if (ValidateFailed != null)
            {
                string failedSelection = Marshal.PtrToStringAuto((IntPtr)lParam);
                FolderBrowserValidateFailedEventArgs e = new FolderBrowserValidateFailedEventArgs(hwnd, failedSelection);
                return ValidateFailed(this, e);
            }

            return 0;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Resets properties of FolderBrowser to their default values. 
        /// </summary>
        public override void Reset()
        {
            m_rootLocation = LocationID.Desktop;
            m_dialogTitle = string.Empty;
            m_startExpandedDirectory = string.Empty;
            m_startExpandedDirectory = string.Empty;
            m_descriptionText = string.Empty;
            m_selectedDirectory = string.Empty;
            m_useNewUI = true;
            m_showNewFolderButton = true;
            m_restrictToFilesystem = true;
            m_showFullPath = false;
            m_isEditBoxDisabled = false;
            m_isEditBoxReadOnly = false;
            m_validateSelection = false;
        }
        
        /// <summary>
        /// Runs a common dialog box with the specified owner. 
        /// </summary>
        /// <param name="hwndOwner">Owner IntPtr</param>
        /// <returns>bool value type</returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            IntPtr pidlRoot = IntPtr.Zero;
            IntPtr pidlRet = IntPtr.Zero;
            uint iAtribute;

            if (RootLocation != LocationID.Custom)
            {
                //// Get the IDL for the specific startLocation.
                Interop.Shell32.SHGetSpecialFolderLocation(hwndOwner, (int)RootLocation, out pidlRoot);
            }
            else
            {
                //// Get the IDL for the specific path.
                Interop.Shell32.SHParseDisplayName(RootPath, IntPtr.Zero, out pidlRoot, 0, out iAtribute);
            }

            if (pidlRoot == IntPtr.Zero)
            {
                Trace.WriteLine("Invalid location path");
                return false;
            }

            try
            {
                if (m_ptrBrowseCallbackProc == IntPtr.Zero)
                {
                    m_delegateCallBack = new Interop.BFFCALLBACK(this.BrowseCallbackProc);
                    m_ptrBrowseCallbackProc = Marshal.GetFunctionPointerForDelegate(m_delegateCallBack);
                }

                //// Construct a BROWSEINFO.
                Interop.BROWSEINFO bi = new Interop.BROWSEINFO();
                IntPtr buffer = Marshal.AllocHGlobal(MAX_PATH);
                bi.pidlRoot = pidlRoot;
                bi.hwndOwner = hwndOwner;
                bi.pszDisplayName = buffer;
                bi.lpszTitle = DescriptionText;
                bi.ulFlags = (int)GetFlagsValue();
                bi.lpfn = m_ptrBrowseCallbackProc;

                //// Show the dialog.
                pidlRet = Interop.Shell32.SHBrowseForFolder(ref bi);
                bi.lpfn = IntPtr.Zero;

                //// Free the buffer, you have allocated on the global heap.
                Marshal.FreeHGlobal(buffer);

                if (pidlRet == IntPtr.Zero)
                {
                    //// User clicked Cancel.
                    return false;
                }

                //// Then retrieve the path from the IDList.
                StringBuilder sb = new StringBuilder(MAX_PATH);

                if (0 == Interop.Shell32.SHGetPathFromIDList(pidlRet, sb))
                {
                    return false;
                }

                SelectedDirectory = sb.ToString();
            }
            finally
            {
                ////Disposing...
                IMalloc malloc;
                Interop.Shell32.SHGetMalloc(out malloc);

                if (pidlRet != IntPtr.Zero)
                {
                    malloc.Free(pidlRet);
                }

                if (pidlRoot != IntPtr.Zero)
                {
                    malloc.Free(pidlRoot);
                }
            }

            return true;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when this FolderBrowser is initialized.
        /// </summary>
        public event FolderBrowserInitializedEventHandler Initialized;
        
        /// <summary>
        /// Occurs after initializing and gives a pointer to IUnknown interface.
        /// </summary>
        public event FolderBrowserIUnknownEventHandler IUnknown;
        
        /// <summary>
        /// Occurs when the user changes his selection.
        /// </summary>
        public event FolderBrowserSelectionChangedEventHandler SelectedChanged;
        
        /// <summary>
        ///  If the property ShowEditBox is true, meaning you let the user enter string, 
        /// and the user enters an invalid string (the folder he entered does not exists) the shell will notify you of this.
        /// </summary>
        public event FolderBrowserValidateFailedEventHandler ValidateFailed;
        #endregion
    }
}
