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

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

using Microsoft.Vsa;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Tools;


namespace Syncfusion.Scripting.Design
{
  /// <summary>
  /// Script Engine control. Support writing multi-language script.
  /// </summary>
  [
   ToolboxItem(false),
   DesignTimeVisible(false)
  ]
  public class ScriptEditControl : UserControl
  {
    /// <summary>
    /// Minimal height for hiding box.
    /// </summary>
    private const int DEF_HEIGHT_MIN = 10;
    /// <summary>
    /// Minimal width for hiding box.
    /// </summary>
    private const int DEF_WIDTH_MIN = 10;
    /// <summary>
    /// Default height for expand ErrorMessage window.
    /// </summary>
    private const int DEF_HEIGHT_ERRORMESSWIND = 100;

    private const string DEF_CANNOT_SWITCH_TO_SAFE_MODE = "You can't use < MemberEdit > Mode because script contain invalid xml";
    private const string DEF_MESS_CONFIRM_NEW = "Create new method ?";

    private const string DEF_SCRIPTFILE_EXTENSION = ".ess";

    private const string DEF_FILEDIALOG_CSFILTER = "C# Files|*.cs|";
    private const string DEF_FILEDIALOG_VBFILTER = "VB Files|*.vb|";
    private const string DEF_FILEDIALOG_JSCRIPTFILTER = "JavaScript Files|*.js|";
    private const string DEF_FILEDIALOG_ESSFILTER = "Essential Suite Scripting Files|*.ess";

    /// <summary>
    /// Send when script is changed.
    /// </summary>
    public event EventHandler ScriptChanged;

    private string m_strScript = string.Empty;
    private Script scriptRef = new Script();
    private Type typeBaseClass = typeof(object);
    private ArrayList lastCompilerErrors = new ArrayList();
    private Hashtable m_toolText = new Hashtable();
    private ScriptObjectCollection scriptableObjects = new ScriptObjectCollection();
    protected string nameSpaceDefine = String.Empty;
	protected string assemblyDirectives = String.Empty;
    protected string globalCode = String.Empty;
    protected string strScriptStart = String.Empty;
    protected string strScriptStop = String.Empty;
    private static Hashtable htTypeMap = new Hashtable();

    protected ScriptingManager scriptManager = null;
    protected bool bExternalCompile = false;
    protected bool bExternalRun = false;

    protected bool bPendingSave = false;

    [
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
        Browsable( false )
      ]
    public string ScriptName
    {
      get
      {
        return this.tbScriptName.Text;
      }
      set
      {
        if( this.tbScriptName.Text != value )
        {
          this.tbScriptName.Text = value;         
        }
      }
    }

    [
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
        Browsable( false )
      ]
    public ScriptLanguages ScriptLanguage
    {
      get
      {
        if( this.cbLanguage.SelectedItem != null )
        {
          return ( ScriptLanguages )TypeDescriptor.GetConverter( typeof( ScriptLanguages ) ).ConvertFromString( ( string )this.cbLanguage.SelectedItem );
        }
        else
        {
          return ScriptLanguages.VisualBasic;
        }
      }
      set
      {
        string language = TypeDescriptor.GetConverter( value ).ConvertToString( value );
        if( ( this.cbLanguage.Items.Contains( language ) ) && ( ( string )this.cbLanguage.SelectedItem != language ) )
        {
          this.cbLanguage.SelectedItem = language;
        }
      }
    }

    public Type BaseClass
    {
      get { return this.typeBaseClass; }
    }

    [
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
        Browsable( false )
      ]
    public string RootMoniker
    {
      get
      {
        return this.tbRootMoniker.Text;
      }
      set
      {
        this.tbRootMoniker.Text = value;
      }
    }

    [
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
        Browsable( false )
      ]
    public string RootNamespace
    {
      get
      {
        return this.tbRootNamespace.Text;
      }
      set
      {
        this.tbRootNamespace.Text = value;
      }
    }

    [
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
        Browsable( false )
      ]
    public string EntryPoint
    {
      get
      {
        return this.tbEntryPoint.Text;
      }
      set
      {
        this.tbEntryPoint.Text = value;
      }
    }

    /// <summary>
    /// Gets or sets script source code.
    /// </summary>
    [
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
        Browsable( false )
      ]
    public string ScriptText
    {
      get
      {
        this.SaveCurrentScript();        
        return this.m_strScript;
      }

      set
      {
        if( this.ScriptText != value )
        {
          this.m_strScript = value;          
          this.OnScriptChange();
        }
      }
    }

    [
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
        Browsable( false )
      ]
    public Script Script
    {
      get
      {
        this.UpdateScript(this.scriptRef);
        return this.scriptRef;
      }

      set
      {
        if( this.scriptRef != value )
        {
          this.scriptRef = value;
          this.InitializeScriptEditor( value );
        }
      }
    }
   

    [
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
        Browsable( false )
      ]
    public ScriptObject SelectedItem
    {
      get
      {
        return this.objectBrowser.SelectedObject;
      }

      set
      {
        if( this.objectBrowser.SelectedObject != value )
        {
          this.objectBrowser.SelectedObject = value;
          string objectname = value.Name;
          if( ( this.comboObjects.SelectedItem == null ) || ( ( string )this.comboObjects.SelectedItem != objectname ) )
          {
            this.comboObjects.SelectedItem = objectname;
          }          
        }
      }
    }

    public string[] AssemblyReferences
    {
      get
      {
        string[] usingdir = new string[this.lbAssemblyReferences.Items.Count];
        this.lbAssemblyReferences.Items.CopyTo( usingdir, 0 );
        return usingdir;
      }
    }

    /// <summary>
    /// Indicates if the ScriptingManager is being used for external compilation of the script.
    /// </summary>
    [ DefaultValue( false ) ]
    public bool EnableExternalCompile
    {
      get
      {
        return this.bExternalCompile;
      }
      set
      {
        this.bExternalCompile = value;
        this.SetCompileButtonsState();
      }
    }

    [ DefaultValue( false ) ]
    public bool EnableExternalRun
    {
      get
      {
        return this.bExternalRun;
      }
      set
      {
        if( this.bExternalRun != value )
        {
          this.bExternalRun = value;
          if( this.bExternalRun == true )
          {
            this.bExternalCompile = true;
          }
          this.SetCompileButtonsState();
        }
      }
    }

    public ScriptingManager ScriptingManager
    {
      get
      {
        return this.scriptManager;
      }

      set
      {
		  if(this.scriptManager != value)
		  {
			  if(this.scriptManager != null)
			  {
				  this.scriptManager.CompileError -= new VsaErrorEventHandler( this.scriptingManager_CompileError );
			  }
			  this.scriptManager = value;
			  if(value != null)
			  {
				  this.objectBrowser.ScriptSite = value.ScriptSite;
				  this.scriptManager.CompileError += new VsaErrorEventHandler( this.scriptingManager_CompileError );
				  this.SetCompileButtonsState();
			  }
		  }
      }
    }

    [
      Browsable( false ),
        DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
      ]
    public bool PendingSave
    {
      get
      {
        return this.bPendingSave;
      }
    }

    private ToolBar toolOperations;
    private ToolBarButton btnSave;
    private ToolBarButton btnCompile;
    private ToolBarButton btnOpen;
    private ScriptObjectBrowser objectBrowser;
    private EditControl cntrlEdit;
    private ImageList imageList;
    private SaveFileDialog saveDlg;
    private OpenFileDialog openDlg;
    private ToolBarButton btnNew;
    private ListBox errorMessages;
    private ToolBarButton btnShowObjectBrowser;
    private ToolBarButton btnShowErrorMessages;
    private ToolBarButton bntSep2;
    private ContextMenu mnuTools;
    private MenuItem mniShowTooltip;
    private DockingManager dockingManager1;
    private DockingClientPanel dockingClientPanel1;
    private ToolBarButton btnShowGeneral;
    private ToolBarButton btnShowAssemblies;
    private Panel generalPanel;
    private Panel assembliesPanel;
    private Label label1;
    private Label label2;
    private Label label3;
    private Label label4;
    private Label label5;
    private Panel panel1;
    private Button btnMoveAssemblyRefDown;
    private Button btnMoveAssemblyRefUp;
    private Button btnRemoveAssemblyRef;
    private Button btnAddAssemblyRef;
    private TextBox tbEntryPoint;
    private TextBox tbRootNamespace;
    private TextBox tbRootMoniker;
    private TextBox tbScriptName;
    private ListBox lbAssemblyReferences;
    private ComboBox cbLanguage;
    private ComboBox comboObjects;
    private Label label6;
    private ToolBarButton btnRun;
    private ToolBarButton btnStop;
    private ToolBarButton btnSep3;
    private Panel pnlObjectBrowser;
    private FolderBrowser folderBrowser1;
    private ContextMenu mnuSaveScript;
    private MenuItem miSaveScript;
    private MenuItem miSaveScriptAs;
    private IContainer components;

    /// <summary>
    /// Default constructor
    /// </summary>
	  public ScriptEditControl()
	  {
		  // Create the ScriptObjectBrowser and attach a handler for the NodeDoubleClick event
		  this.objectBrowser = new ScriptObjectBrowser();
		  this.objectBrowser.Visible = true;
		  this.objectBrowser.NodeDoubleClick += new NodeDoubleClickEventHandler( this.ObjectBrowser_NodeDoubleClick );

		  // This call is required by the Windows.Forms Form Designer.
		  InitializeComponent();

		  this.cntrlEdit.SharedFileMode = true;	
      
		  // Initializing cbLanguage
		  ICollection languages = ScriptEngineFactory.SupportedLanguages;
		  TypeConverter converter = TypeDescriptor.GetConverter(typeof(ScriptLanguages));
		  foreach(ScriptLanguages language in languages)
		  {
			  this.cbLanguage.Items.Add(converter.ConvertToString(language));
		  }
		  // Initializing ScriptEdit control
		  this.SelectLanguage(ScriptLanguages.VisualBasic);
		  this.cntrlEdit.ApplyConfiguration(KnownLanguages.VBNET);

		  // StatusBar Settings for the ScriptEditControl
		  this.cntrlEdit.StatusBarSettings.Visible = true;
		  this.cntrlEdit.StatusBarSettings.TextPanel.Visible = false;
		  this.cntrlEdit.StatusBarSettings.FileNamePanel.Width = 60;
      
		  // Initialize the General Panel with default Script attributes
		  this.tbScriptName.Text = "Script";
		  this.tbRootMoniker.Text = "Syncfusion://script";
		  this.tbRootNamespace.Text = "Syncfusion";
		  this.tbEntryPoint.Text = String.Empty; 

		  // Add the objectBrowser control to it's dockable host
		  this.pnlObjectBrowser.Controls.AddRange(new Control[] {this.comboObjects, this.objectBrowser});
		  this.comboObjects.Dock = DockStyle.Top;
		  this.objectBrowser.Dock = DockStyle.Fill;
		  this.objectBrowser.BringToFront();

		  // Set new file
		  this.New();

		  // Set the FolderBrowser startlocation
		  this.folderBrowser1.Description = "Assembly Name: ";
		  this.folderBrowser1.StartLocation = FolderBrowserFolder.MyComputer;
		  this.folderBrowser1.FolderBrowserCallback += new FolderBrowserCallbackEventHandler( this.folderBrowser1_BrowseCallback );
	  }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
	  protected override void Dispose( bool disposing )
	  {
		  base.Dispose( disposing );

		  if( disposing )
		  {
			  if( components != null )
			  {
				  if( this.scriptManager != null )
				  {
					  this.scriptManager.CompileError -= new VsaErrorEventHandler( this.scriptingManager_CompileError );
					  this.scriptManager = null;
				  }
				  this.folderBrowser1.FolderBrowserCallback -= new FolderBrowserCallbackEventHandler( this.folderBrowser1_BrowseCallback );

				  components.Dispose();
			  }
		  }
	  }

	  private void InitializeComponent()
	  {
		  this.components = new System.ComponentModel.Container();
		  System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ScriptEditControl));
		  this.toolOperations = new System.Windows.Forms.ToolBar();
		  this.btnNew = new System.Windows.Forms.ToolBarButton();
		  this.btnOpen = new System.Windows.Forms.ToolBarButton();
		  this.btnSave = new System.Windows.Forms.ToolBarButton();
		  this.mnuSaveScript = new System.Windows.Forms.ContextMenu();
		  this.miSaveScript = new System.Windows.Forms.MenuItem();
		  this.miSaveScriptAs = new System.Windows.Forms.MenuItem();
		  this.bntSep2 = new System.Windows.Forms.ToolBarButton();
		  this.btnShowGeneral = new System.Windows.Forms.ToolBarButton();
		  this.btnShowAssemblies = new System.Windows.Forms.ToolBarButton();
		  this.btnShowObjectBrowser = new System.Windows.Forms.ToolBarButton();
		  this.btnSep3 = new System.Windows.Forms.ToolBarButton();
		  this.btnCompile = new System.Windows.Forms.ToolBarButton();
		  this.btnRun = new System.Windows.Forms.ToolBarButton();
		  this.btnStop = new System.Windows.Forms.ToolBarButton();
		  this.btnShowErrorMessages = new System.Windows.Forms.ToolBarButton();
		  this.mnuTools = new System.Windows.Forms.ContextMenu();
		  this.mniShowTooltip = new System.Windows.Forms.MenuItem();
		  this.imageList = new System.Windows.Forms.ImageList(this.components);
		  this.label6 = new System.Windows.Forms.Label();
		  this.comboObjects = new System.Windows.Forms.ComboBox();
		  this.cntrlEdit = new Syncfusion.Windows.Forms.Edit.EditControl();
		  this.errorMessages = new System.Windows.Forms.ListBox();
		  this.saveDlg = new System.Windows.Forms.SaveFileDialog();
		  this.openDlg = new System.Windows.Forms.OpenFileDialog();
		  this.dockingManager1 = new Syncfusion.Windows.Forms.Tools.DockingManager(this.components);
		  this.generalPanel = new System.Windows.Forms.Panel();
		  this.tbEntryPoint = new System.Windows.Forms.TextBox();
		  this.label5 = new System.Windows.Forms.Label();
		  this.tbRootNamespace = new System.Windows.Forms.TextBox();
		  this.label3 = new System.Windows.Forms.Label();
		  this.tbRootMoniker = new System.Windows.Forms.TextBox();
		  this.label4 = new System.Windows.Forms.Label();
		  this.cbLanguage = new System.Windows.Forms.ComboBox();
		  this.label2 = new System.Windows.Forms.Label();
		  this.tbScriptName = new System.Windows.Forms.TextBox();
		  this.label1 = new System.Windows.Forms.Label();
		  this.assembliesPanel = new System.Windows.Forms.Panel();
		  this.lbAssemblyReferences = new System.Windows.Forms.ListBox();
		  this.panel1 = new System.Windows.Forms.Panel();
		  this.btnMoveAssemblyRefDown = new System.Windows.Forms.Button();
		  this.btnMoveAssemblyRefUp = new System.Windows.Forms.Button();
		  this.btnRemoveAssemblyRef = new System.Windows.Forms.Button();
		  this.btnAddAssemblyRef = new System.Windows.Forms.Button();
		  this.pnlObjectBrowser = new System.Windows.Forms.Panel();
		  this.dockingClientPanel1 = new Syncfusion.Windows.Forms.Tools.DockingClientPanel();
		  this.folderBrowser1 = new Syncfusion.Windows.Forms.FolderBrowser(this.components);
		  ((System.ComponentModel.ISupportInitialize)(this.cntrlEdit)).BeginInit();
		  ((System.ComponentModel.ISupportInitialize)(this.dockingManager1)).BeginInit();
		  this.generalPanel.SuspendLayout();
		  this.assembliesPanel.SuspendLayout();
		  this.panel1.SuspendLayout();
		  this.dockingClientPanel1.SuspendLayout();
		  this.SuspendLayout();
		  // 
		  // toolOperations
		  // 
		  this.toolOperations.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
		  this.toolOperations.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																							this.btnNew,
																							this.btnOpen,
																							this.btnSave,
																							this.bntSep2,
																							this.btnShowGeneral,
																							this.btnShowAssemblies,
																							this.btnShowObjectBrowser,
																							this.btnSep3,
																							this.btnCompile,
																							this.btnRun,
																							this.btnStop,
																							this.btnShowErrorMessages});
		  this.toolOperations.ButtonSize = new System.Drawing.Size(16, 16);
		  this.toolOperations.ContextMenu = this.mnuTools;
		  this.toolOperations.DropDownArrows = true;
		  this.toolOperations.ImageList = this.imageList;
		  this.toolOperations.Name = "toolOperations";
		  this.toolOperations.ShowToolTips = true;
		  this.toolOperations.Size = new System.Drawing.Size(832, 39);
		  this.toolOperations.TabIndex = 3;
		  this.toolOperations.Wrappable = false;
		  this.toolOperations.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolOperations_ButtonClick);
		  // 
		  // btnNew
		  // 
		  this.btnNew.ImageIndex = 0;
		  this.btnNew.Text = "New";
		  this.btnNew.ToolTipText = "New";
		  // 
		  // btnOpen
		  // 
		  this.btnOpen.ImageIndex = 4;
		  this.btnOpen.Text = "Open";
		  this.btnOpen.ToolTipText = "Open";
		  // 
		  // btnSave
		  // 
		  this.btnSave.DropDownMenu = this.mnuSaveScript;
		  this.btnSave.ImageIndex = 1;
		  this.btnSave.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
		  this.btnSave.Text = "Save";
		  this.btnSave.ToolTipText = "Save";
		  // 
		  // mnuSaveScript
		  // 
		  this.mnuSaveScript.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.miSaveScript,
																						this.miSaveScriptAs});
		  // 
		  // miSaveScript
		  // 
		  this.miSaveScript.DefaultItem = true;
		  this.miSaveScript.Index = 0;
		  this.miSaveScript.Text = "Save Script";
		  this.miSaveScript.Click += new System.EventHandler(this.miSaveScript_Click);
		  // 
		  // miSaveScriptAs
		  // 
		  this.miSaveScriptAs.Index = 1;
		  this.miSaveScriptAs.Text = "Save Script As...";
		  this.miSaveScriptAs.Click += new System.EventHandler(this.miSaveScriptAs_Click);
		  // 
		  // bntSep2
		  // 
		  this.bntSep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
		  // 
		  // btnShowGeneral
		  // 
		  this.btnShowGeneral.ImageIndex = 8;
		  this.btnShowGeneral.Pushed = true;
		  this.btnShowGeneral.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
		  this.btnShowGeneral.Text = "General";
		  this.btnShowGeneral.ToolTipText = "General";
		  // 
		  // btnShowAssemblies
		  // 
		  this.btnShowAssemblies.ImageIndex = 8;
		  this.btnShowAssemblies.Pushed = true;
		  this.btnShowAssemblies.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
		  this.btnShowAssemblies.Text = "Assemblies";
		  this.btnShowAssemblies.ToolTipText = "Assemblies";
		  // 
		  // btnShowObjectBrowser
		  // 
		  this.btnShowObjectBrowser.ImageIndex = 8;
		  this.btnShowObjectBrowser.Pushed = true;
		  this.btnShowObjectBrowser.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
		  this.btnShowObjectBrowser.Text = "Browser";
		  this.btnShowObjectBrowser.ToolTipText = "Show object browser";
		  // 
		  // btnSep3
		  // 
		  this.btnSep3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
		  // 
		  // btnCompile
		  // 
		  this.btnCompile.Enabled = false;
		  this.btnCompile.ImageIndex = 3;
		  this.btnCompile.Text = "Compile";
		  this.btnCompile.ToolTipText = "Compile";
		  // 
		  // btnRun
		  // 
		  this.btnRun.Enabled = false;
		  this.btnRun.ImageIndex = 3;
		  this.btnRun.Text = "Run";
		  this.btnRun.ToolTipText = "Run";
		  // 
		  // btnStop
		  // 
		  this.btnStop.Enabled = false;
		  this.btnStop.ImageIndex = 3;
		  this.btnStop.Text = "Stop";
		  this.btnStop.ToolTipText = "Stop";
		  // 
		  // btnShowErrorMessages
		  // 
		  this.btnShowErrorMessages.Enabled = false;
		  this.btnShowErrorMessages.ImageIndex = 8;
		  this.btnShowErrorMessages.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
		  this.btnShowErrorMessages.Text = "Errors";
		  this.btnShowErrorMessages.ToolTipText = "Show error message box";
		  // 
		  // mnuTools
		  // 
		  this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.mniShowTooltip});
		  // 
		  // mniShowTooltip
		  // 
		  this.mniShowTooltip.Checked = true;
		  this.mniShowTooltip.Index = 0;
		  this.mniShowTooltip.Text = "Show tooltip";
		  this.mniShowTooltip.Click += new System.EventHandler(this.mniShowTooltip_Click);
		  // 
		  // imageList
		  // 
		  this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
		  this.imageList.ImageSize = new System.Drawing.Size(16, 16);
		  this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
		  this.imageList.TransparentColor = System.Drawing.Color.Transparent;
		  // 
		  // label6
		  // 
		  this.label6.Name = "label6";
		  this.label6.TabIndex = 0;
		  // 
		  // comboObjects
		  // 
		  this.comboObjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		  this.comboObjects.Location = new System.Drawing.Point(8, 8);
		  this.comboObjects.Name = "comboObjects";
		  this.comboObjects.Size = new System.Drawing.Size(184, 21);
		  this.comboObjects.TabIndex = 0;
		  this.comboObjects.SelectedIndexChanged += new System.EventHandler(this.comboObjects_SelectedIndexChanged);
		  // 
		  // cntrlEdit
		  // 
		  this.cntrlEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		  this.cntrlEdit.Name = "cntrlEdit";
		  this.cntrlEdit.Size = new System.Drawing.Size(352, 168);
		  this.cntrlEdit.StatusBarSettings.FileNamePanel.AutoSize = false;
          this.cntrlEdit.StatusBarSettings.TextPanel.AutoSize = false;
		  this.cntrlEdit.StatusBarSettings.TextPanel.Visible = false;
		  this.cntrlEdit.StatusBarSettings.Visible = true;
		  this.cntrlEdit.TabIndex = 1;
		  this.cntrlEdit.Text = "edtCode";
		  this.cntrlEdit.TextChanged += new System.EventHandler(this.cntrlEdit_TextChanged);
		  // 
		  // errorMessages
		  // 
		  this.dockingManager1.SetEnableDocking(this.errorMessages, true);
		  this.errorMessages.HorizontalScrollbar = true;
		  this.errorMessages.Location = new System.Drawing.Point(1, 21);
		  this.errorMessages.Name = "errorMessages";
		  this.errorMessages.Size = new System.Drawing.Size(830, 160);
		  this.errorMessages.TabIndex = 3;
		  this.errorMessages.DoubleClick += new System.EventHandler(this.listErrorMessages_DoubleClick);
		  // 
		  // dockingManager1
		  // 
		  this.dockingManager1.DockLayoutStream = ((System.IO.MemoryStream)(resources.GetObject("dockingManager1.DockLayoutStream")));
		  this.dockingManager1.HostControl = this;
		  this.dockingManager1.ImageList = this.imageList;
		  this.dockingManager1.DockVisibilityChanged += new Syncfusion.Windows.Forms.Tools.DockVisibilityChangedEventHandler(this.dockingManager1_DockVisibilityChanged);
		  this.dockingManager1.SetDockLabel(this.errorMessages, "Error Messages");
		  this.dockingManager1.SetDockIcon(this.errorMessages, 8);
		  this.dockingManager1.SetHiddenOnLoad(this.errorMessages, true);
		  this.dockingManager1.SetDockLabel(this.generalPanel, "General Panel");
		  this.dockingManager1.SetDockIcon(this.generalPanel, 8);
		  this.dockingManager1.SetDockLabel(this.assembliesPanel, "Assemblies Panel");
		  this.dockingManager1.SetDockIcon(this.assembliesPanel, 8);
		  this.dockingManager1.SetDockLabel(this.pnlObjectBrowser, "Object Browser");
		  this.dockingManager1.SetDockIcon(this.pnlObjectBrowser, 8);
		  // 
		  // generalPanel
		  // 
		  this.generalPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.tbEntryPoint,
																					 this.label5,
																					 this.tbRootNamespace,
																					 this.label3,
																					 this.tbRootMoniker,
																					 this.label4,
																					 this.cbLanguage,
																					 this.label2,
																					 this.tbScriptName,
																					 this.label1});
		  this.dockingManager1.SetEnableDocking(this.generalPanel, true);
		  this.generalPanel.Location = new System.Drawing.Point(1, 21);
		  this.generalPanel.Name = "generalPanel";
		  this.generalPanel.Size = new System.Drawing.Size(830, 142);
		  this.generalPanel.TabIndex = 16;
		  // 
		  // tbEntryPoint
		  // 
		  this.tbEntryPoint.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			  | System.Windows.Forms.AnchorStyles.Right);
		  this.tbEntryPoint.Location = new System.Drawing.Point(112, 112);
		  this.tbEntryPoint.Name = "tbEntryPoint";
		  this.tbEntryPoint.Size = new System.Drawing.Size(702, 20);
		  this.tbEntryPoint.TabIndex = 13;
		  this.tbEntryPoint.Text = "";
		  this.tbEntryPoint.TextChanged += new System.EventHandler(this.tbGeneralPanel_TextChanged);
		  // 
		  // label5
		  // 
		  this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
		  this.label5.Location = new System.Drawing.Point(24, 112);
		  this.label5.Name = "label5";
		  this.label5.Size = new System.Drawing.Size(80, 16);
		  this.label5.TabIndex = 12;
		  this.label5.Text = "Entry Point:";
		  this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		  // 
		  // tbRootNamespace
		  // 
		  this.tbRootNamespace.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			  | System.Windows.Forms.AnchorStyles.Right);
		  this.tbRootNamespace.Location = new System.Drawing.Point(112, 88);
		  this.tbRootNamespace.Name = "tbRootNamespace";
		  this.tbRootNamespace.Size = new System.Drawing.Size(702, 20);
		  this.tbRootNamespace.TabIndex = 9;
		  this.tbRootNamespace.Text = "";
		  this.tbRootNamespace.TextChanged += new System.EventHandler(this.tbGeneralPanel_TextChanged);
		  // 
		  // label3
		  // 
		  this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
		  this.label3.Location = new System.Drawing.Point(8, 88);
		  this.label3.Name = "label3";
		  this.label3.Size = new System.Drawing.Size(100, 16);
		  this.label3.TabIndex = 11;
		  this.label3.Text = "Root Namespace:";
		  this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		  // 
		  // tbRootMoniker
		  // 
		  this.tbRootMoniker.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			  | System.Windows.Forms.AnchorStyles.Right);
		  this.tbRootMoniker.Location = new System.Drawing.Point(112, 64);
		  this.tbRootMoniker.Name = "tbRootMoniker";
		  this.tbRootMoniker.Size = new System.Drawing.Size(702, 20);
		  this.tbRootMoniker.TabIndex = 8;
		  this.tbRootMoniker.Text = "";
		  this.tbRootMoniker.TextChanged += new System.EventHandler(this.tbGeneralPanel_TextChanged);
		  // 
		  // label4
		  // 
		  this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
		  this.label4.Location = new System.Drawing.Point(24, 64);
		  this.label4.Name = "label4";
		  this.label4.Size = new System.Drawing.Size(80, 16);
		  this.label4.TabIndex = 10;
		  this.label4.Text = "Root Moniker:";
		  this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		  // 
		  // cbLanguage
		  // 
		  this.cbLanguage.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			  | System.Windows.Forms.AnchorStyles.Right);
		  this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		  this.cbLanguage.Location = new System.Drawing.Point(112, 40);
		  this.cbLanguage.Name = "cbLanguage";
		  this.cbLanguage.Size = new System.Drawing.Size(702, 21);
		  this.cbLanguage.TabIndex = 5;
		  this.cbLanguage.SelectedIndexChanged += new System.EventHandler(this.cbLanguage_SelectedIndexChanged);
		  // 
		  // label2
		  // 
		  this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
		  this.label2.Location = new System.Drawing.Point(16, 40);
		  this.label2.Name = "label2";
		  this.label2.Size = new System.Drawing.Size(80, 16);
		  this.label2.TabIndex = 6;
		  this.label2.Text = "Language:";
		  this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		  // 
		  // tbScriptName
		  // 
		  this.tbScriptName.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			  | System.Windows.Forms.AnchorStyles.Right);
		  this.tbScriptName.Location = new System.Drawing.Point(112, 16);
		  this.tbScriptName.Name = "tbScriptName";
		  this.tbScriptName.Size = new System.Drawing.Size(702, 20);
		  this.tbScriptName.TabIndex = 3;
		  this.tbScriptName.Text = "";
		  this.tbScriptName.TextChanged += new System.EventHandler(this.tbScriptName_TextChanged);
		  // 
		  // label1
		  // 
		  this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
		  this.label1.Location = new System.Drawing.Point(24, 16);
		  this.label1.Name = "label1";
		  this.label1.Size = new System.Drawing.Size(76, 16);
		  this.label1.TabIndex = 2;
		  this.label1.Text = "Script Name:";
		  this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		  // 
		  // assembliesPanel
		  // 
		  this.assembliesPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.lbAssemblyReferences,
																						this.panel1});
		  this.dockingManager1.SetEnableDocking(this.assembliesPanel, true);
		  this.assembliesPanel.Location = new System.Drawing.Point(1, 21);
		  this.assembliesPanel.Name = "assembliesPanel";
		  this.assembliesPanel.Size = new System.Drawing.Size(830, 142);
		  this.assembliesPanel.TabIndex = 17;
		  // 
		  // lbAssemblyReferences
		  // 
		  this.lbAssemblyReferences.Dock = System.Windows.Forms.DockStyle.Fill;
		  this.lbAssemblyReferences.Name = "lbAssemblyReferences";
		  this.lbAssemblyReferences.Size = new System.Drawing.Size(742, 134);
		  this.lbAssemblyReferences.TabIndex = 2;
		  // 
		  // panel1
		  // 
		  this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																			   this.btnMoveAssemblyRefDown,
																			   this.btnMoveAssemblyRefUp,
																			   this.btnRemoveAssemblyRef,
																			   this.btnAddAssemblyRef});
		  this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
		  this.panel1.Location = new System.Drawing.Point(742, 0);
		  this.panel1.Name = "panel1";
		  this.panel1.Size = new System.Drawing.Size(88, 142);
		  this.panel1.TabIndex = 1;
		  // 
		  // btnMoveAssemblyRefDown
		  // 
		  this.btnMoveAssemblyRefDown.Location = new System.Drawing.Point(8, 112);
		  this.btnMoveAssemblyRefDown.Name = "btnMoveAssemblyRefDown";
		  this.btnMoveAssemblyRefDown.Size = new System.Drawing.Size(72, 24);
		  this.btnMoveAssemblyRefDown.TabIndex = 3;
		  this.btnMoveAssemblyRefDown.Text = "Move Down";
		  this.btnMoveAssemblyRefDown.Click += new System.EventHandler(this.btnMoveAssemblyRefDown_Click);
		  // 
		  // btnMoveAssemblyRefUp
		  // 
		  this.btnMoveAssemblyRefUp.Location = new System.Drawing.Point(8, 80);
		  this.btnMoveAssemblyRefUp.Name = "btnMoveAssemblyRefUp";
		  this.btnMoveAssemblyRefUp.Size = new System.Drawing.Size(72, 24);
		  this.btnMoveAssemblyRefUp.TabIndex = 2;
		  this.btnMoveAssemblyRefUp.Text = "Move Up";
		  this.btnMoveAssemblyRefUp.Click += new System.EventHandler(this.btnMoveAssemblyRefUp_Click);
		  // 
		  // btnRemoveAssemblyRef
		  // 
		  this.btnRemoveAssemblyRef.Location = new System.Drawing.Point(8, 48);
		  this.btnRemoveAssemblyRef.Name = "btnRemoveAssemblyRef";
		  this.btnRemoveAssemblyRef.Size = new System.Drawing.Size(72, 24);
		  this.btnRemoveAssemblyRef.TabIndex = 1;
		  this.btnRemoveAssemblyRef.Text = "Remove";
		  this.btnRemoveAssemblyRef.Click += new System.EventHandler(this.btnRemoveAssemblyRef_Click);
		  // 
		  // btnAddAssemblyRef
		  // 
		  this.btnAddAssemblyRef.Location = new System.Drawing.Point(8, 16);
		  this.btnAddAssemblyRef.Name = "btnAddAssemblyRef";
		  this.btnAddAssemblyRef.Size = new System.Drawing.Size(72, 24);
		  this.btnAddAssemblyRef.TabIndex = 0;
		  this.btnAddAssemblyRef.Text = "Add...";
		  this.btnAddAssemblyRef.Click += new System.EventHandler(this.btnAddAssemblyRef_Click);
		  // 
		  // pnlObjectBrowser
		  // 
		  this.dockingManager1.SetEnableDocking(this.pnlObjectBrowser, true);
		  this.pnlObjectBrowser.Location = new System.Drawing.Point(1, 21);
		  this.pnlObjectBrowser.Name = "pnlObjectBrowser";
		  this.pnlObjectBrowser.Size = new System.Drawing.Size(155, 298);
		  this.pnlObjectBrowser.TabIndex = 17;
		  // 
		  // dockingClientPanel1
		  // 
		  this.dockingClientPanel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																							this.cntrlEdit});
		  this.dockingClientPanel1.Location = new System.Drawing.Point(288, 120);
		  this.dockingClientPanel1.Name = "dockingClientPanel1";
		  this.dockingClientPanel1.Size = new System.Drawing.Size(352, 168);
		  this.dockingClientPanel1.TabIndex = 9;
		  // 
		  // folderBrowser1
		  // 
		  this.folderBrowser1.StartLocation = Syncfusion.Windows.Forms.FolderBrowserFolder.Desktop;
		  this.folderBrowser1.Style = ((Syncfusion.Windows.Forms.FolderBrowserStyles.RestrictToFilesystem | Syncfusion.Windows.Forms.FolderBrowserStyles.ShowTextBox) 
			  | Syncfusion.Windows.Forms.FolderBrowserStyles.BrowseForEverything);
		  // 
		  // ScriptEditControl
		  // 
		  this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		this.dockingClientPanel1,
																		this.toolOperations});
		  this.Name = "ScriptEditControl";
		  this.Size = new System.Drawing.Size(832, 552);
		  ((System.ComponentModel.ISupportInitialize)(this.cntrlEdit)).EndInit();
		  ((System.ComponentModel.ISupportInitialize)(this.dockingManager1)).EndInit();
		  this.generalPanel.ResumeLayout(false);
		  this.assembliesPanel.ResumeLayout(false);
		  this.panel1.ResumeLayout(false);
		  this.dockingClientPanel1.ResumeLayout(false);
		  this.ResumeLayout(false);

	  }
	
	  protected void SetCompileButtonsState()
	  {
		  if( ( this.bExternalCompile == true ) && ( this.scriptManager != null ) && ( this.scriptManager.Script != null ) )
		  {
			  this.btnCompile.Enabled = true;
			  this.btnShowErrorMessages.Enabled = true;
			  if( this.bExternalRun == true )
			  {
				  this.btnRun.Enabled = this.scriptManager.IsScriptCompiled;
				  this.btnStop.Enabled = this.scriptManager.IsScriptRunning;
			  }
		  }
		  else
		  {
			  this.btnCompile.Enabled = false;
			  this.btnRun.Enabled = false;
			  this.btnStop.Enabled = false;
			  this.btnShowErrorMessages.Enabled = false;
		  }
	  }

	  public void AddScriptableObject( ScriptObject scriptableitem )
	  {
		  if( scriptableitem == null )
		  {
			  throw new ArgumentNullException( "Invalid ScriptObject" );
		  }
		  if( this.scriptableObjects.Contains( scriptableitem ) == false )
		  {
			  this.scriptableObjects.Add( scriptableitem );

			  // Add an entry for this ScriptableObject in the Object combo
			  this.comboObjects.Items.Add( scriptableitem.Name );
			  if( this.comboObjects.SelectedIndex == -1 )
			  {
				  this.comboObjects.SelectedIndex = 0;
			  }
		  }     
	  }

	  public void RemoveScriptableObject( ScriptObject scriptableitem )
	  {
		  if( this.scriptableObjects.Contains( scriptableitem ) == true )
		  {
			  this.comboObjects.Items.Remove( scriptableitem.Name );
			  this.scriptableObjects.Remove( scriptableitem );
		  }
		  if( this.SelectedItem == scriptableitem )
		  {
			  if( this.scriptableObjects.Count > 0 )
			  {
				  this.SelectedItem = this.scriptableObjects[ 0 ];
			  }
		  }
	  }

	  public void New()
	  {
		  this.cntrlEdit.NewFile();
	  }

	  protected virtual string GetFileDialogFilter( ScriptLanguages language )
	  {
		  string filter = string.Empty;
		  switch( language )
		  {
			  case ScriptLanguages.CSharp:
				  filter = DEF_FILEDIALOG_CSFILTER + DEF_FILEDIALOG_ESSFILTER;
				  break;
			  case ScriptLanguages.VisualBasic:
				  filter = DEF_FILEDIALOG_VBFILTER + DEF_FILEDIALOG_ESSFILTER;
				  break;
			  case ScriptLanguages.JScript:
				  filter = DEF_FILEDIALOG_JSCRIPTFILTER + DEF_FILEDIALOG_ESSFILTER;
				  break;
		  }
		  return filter;
	  }

	  public void SaveToFile()
	  {
		  this.SaveCurrentScript();

		  this.saveDlg.Filter = this.GetFileDialogFilter( this.ScriptLanguage );
		  if( this.saveDlg.ShowDialog() == DialogResult.OK )
		  {
			  string filePath = Path.GetDirectoryName( saveDlg.FileName );
			  filePath += Path.DirectorySeparatorChar;
			  string fileName = Path.GetFileNameWithoutExtension( saveDlg.FileName );
			  filePath += fileName;

			  if( Path.GetExtension( saveDlg.FileName ) == DEF_SCRIPTFILE_EXTENSION ) // Essential Suite Scripting file
			  {
				  filePath += DEF_SCRIPTFILE_EXTENSION;
				  this.UpdateScript(this.scriptRef);

				  using( FileStream fstream = File.Open( filePath, FileMode.Create ) )
				  {
					  BinaryFormatter fmtr = new BinaryFormatter();
					  fmtr.Serialize( fstream, this.Script );
					  fstream.Close();
				  }
			  }
			  else
			  {
				  filePath += ScriptLanguageUtilities.GetFileExtensionFromLanguage( this.ScriptLanguage );
				  using( StreamWriter writer = new StreamWriter( filePath ) )
				  {
					  string resScript = this.ScriptText;
					  writer.Write( resScript );
					  writer.Close();
				  }
			  }
		  }
	  }

	  public void OpenFile()
	  {
		  this.openDlg.Filter = this.GetFileDialogFilter( this.ScriptLanguage );
		  if( DialogResult.OK == openDlg.ShowDialog() )
		  {
			  if( Path.GetExtension( openDlg.FileName ) == DEF_SCRIPTFILE_EXTENSION ) // Essential Suite Scripting file
			  {
				  using( FileStream fstream = File.Open( openDlg.FileName, FileMode.Open ) )
				  {
					  BinaryFormatter fmtr = new BinaryFormatter();
					  this.Script = fmtr.Deserialize( fstream ) as Script;
					  fstream.Close();
				  }
			  }
			  else
			  {
				  using( StreamReader reader = new StreamReader( openDlg.FileName ) )
				  {
					  this.ScriptText = reader.ReadToEnd();
					  reader.Close();
				  }
			  }
		  }
	  }

    
	  public bool CompileScript()
	  {
		  bool breturn = false;

		  // Invoke compilation on the ScriptingManager compile function.
		  if( ( this.EnableExternalCompile ) && ( this.scriptManager != null ) && ( this.scriptManager.Script != null ) )
		  {
			  this.SaveCurrentScript();
			  this.lastCompilerErrors.Clear();
			  this.errorMessages.Items.Clear();

			  this.UpdateScript(this.scriptManager.Script);
			  breturn = this.scriptManager.CompileScript();
			  this.SetCompileButtonsState();
		  }
		  return breturn;
	  }

	  public void RunScript()
	  {
		  if( ( this.EnableExternalCompile ) && ( this.scriptManager != null ) && ( this.scriptManager.IsScriptRunning == false ) &&
			  ( this.scriptManager.Script != null ) )
		  {
			  this.scriptManager.RunScript();
			  this.SetCompileButtonsState();
		  }
	  }

	  public void StopScript()
	  {
		  if( ( this.EnableExternalCompile ) && ( this.scriptManager != null ) && ( this.scriptManager.IsScriptRunning == true ) )
		  {
			  this.scriptManager.ResetScriptEngine();
			  this.scriptManager.ClearScriptSite();
			  this.SetCompileButtonsState();
		  }
	  }

	  protected void scriptingManager_CompileError( object sender, VsaErrorEventArgs eventargs )
	  {
		  if( this.dockingManager1.GetDockVisibility( this.errorMessages ) == false )
		  {
			  this.dockingManager1.SetDockVisibility( this.errorMessages, true );
		  }
          if (eventargs.Error != null)
          {
              string errmessage = "Line: " + eventargs.Error.Line + ", Column" + eventargs.Error.StartColumn + "; Error:" + eventargs.Error.Description;
              ErrorDescriptor errordesc = new ErrorDescriptor(errmessage, 1, 1);
              this.lastCompilerErrors.Add(errordesc);
              this.errorMessages.Items.Add(errordesc);
          }

          //IVsaError error = eventargs.Error;
          //ITextRange editrange = this.cntrlEdit.FindRange (error.LineText.TrimEnd( ' ', '\n', '\r' ), this.cntrlEdit.ConvertVirtualPointToCoordinatePoint(1, 1),  false );
          //ErrorDescriptor errordesc = null;
          //if( ( editrange.Start.VirtualLine >= 0 ) && ( editrange.Start.VirtualColumn >= 0 ) )
          //{
          //    string errmessage = "Line: " + editrange.Start.VirtualLine + ", " +
          //        error.LineText + "; " + " Error: " + error.Description;
          //    errordesc = new ErrorDescriptor( errmessage, editrange.Start.VirtualLine, editrange.Start.VirtualColumn );
          //}
          //else
          //{
          //    string errmessage = error.Description;
          //    errordesc = new ErrorDescriptor( errmessage, 1, 1 );
          //}

          //this.lastCompilerErrors.Add( errordesc );
          //this.errorMessages.Items.Add( errordesc );
	  }

    #region Class event handlers    
    private void toolOperations_ButtonClick( object sender, ToolBarButtonClickEventArgs e )
    {
      if( e.Button == this.btnSave )
      {
        this.SaveCurrentScript();
        if(this.scriptRef != null )
        {
          this.UpdateScript(this.scriptRef);
        }
      }
      else if( e.Button == this.btnOpen )
      {
        this.OpenFile();
      }
      else if( e.Button == this.btnNew )
      {
        this.New();
      }     
      else if( e.Button == this.btnCompile )
      {
        this.CompileScript();
      }
      else if( e.Button == this.btnRun )
      {
        this.RunScript();
      }
      else if( e.Button == this.btnStop )
      {
        this.StopScript();
      }
      else if(e.Button == this.btnShowGeneral)
      {
        this.dockingManager1.SetDockVisibility(this.generalPanel, !this.dockingManager1.GetDockVisibility(this.generalPanel) );
      }
      else if(e.Button == this.btnShowAssemblies)
      {
        this.dockingManager1.SetDockVisibility(this.assembliesPanel, !this.dockingManager1.GetDockVisibility(this.assembliesPanel));
      }
      else if(e.Button == btnShowObjectBrowser)
      {
        this.dockingManager1.SetDockVisibility(this.pnlObjectBrowser, !this.dockingManager1.GetDockVisibility(this.pnlObjectBrowser));
      }
      else if(e.Button == btnShowErrorMessages)
      {
        this.dockingManager1.SetDockVisibility( this.errorMessages, !this.dockingManager1.GetDockVisibility(this.errorMessages));
      }
    }

    private void miSaveScript_Click( object sender, EventArgs e )
    {
      this.SaveCurrentScript();
      if(this.scriptRef != null)
      {
        this.UpdateScript(this.scriptRef);
      }
    }

    private void miSaveScriptAs_Click( object sender, EventArgs e )
    {
      this.SaveToFile();
    }

    private void cbLanguage_SelectedIndexChanged( object sender, EventArgs e )
    {
      this.SelectLanguage( ( ScriptLanguages )( TypeDescriptor.GetConverter( typeof( ScriptLanguages ) ).ConvertFromString( ( string )this.cbLanguage.SelectedItem ) ) );
    }   

    private void comboObjects_SelectedIndexChanged( object sender, EventArgs e )
    {
      String selecteditem = ( string )this.comboObjects.SelectedItem;
      foreach( ScriptObject browsableobject in this.scriptableObjects )
      {
        if( browsableobject.Name == selecteditem )
        {
          this.SelectedItem = browsableobject;
          break;
        }
      }
    }

    private void dockingManager1_DockVisibilityChanged( object sender, DockVisibilityChangedEventArgs e )
    {
      bool pushed = this.dockingManager1.GetDockVisibility( e.Control );

      if( e.Control == this.generalPanel )
      {
        this.btnShowGeneral.Pushed = pushed;
      }
      else if( e.Control == this.assembliesPanel )
      {
        this.btnShowAssemblies.Pushed = pushed;
      }
      else if( e.Control == this.pnlObjectBrowser )
      {
        this.btnShowObjectBrowser.Pushed = pushed;
      }
      else if( e.Control == this.errorMessages )
      {
        this.btnShowErrorMessages.Pushed = pushed;
      }
    }

    private void listErrorMessages_DoubleClick( object sender, EventArgs e )
    {
      ErrorDescriptor errDescr = errorMessages.SelectedItem as ErrorDescriptor;
      if( errDescr != null )
      {
        this.cntrlEdit.CurrentLine = errDescr.Line;
        this.cntrlEdit.CurrentColumn = errDescr.Column;
		  LexemLine line = (LexemLine)this.cntrlEdit.CurrentLineInstance;
		  this.cntrlEdit.SetSelectionStart(line.GetStartPoint());
		  this.cntrlEdit.SetSelectionEnd(line.GetEndPoint());
        this.cntrlEdit.GoTo( errDescr.Line );
        this.cntrlEdit.Focus();
      }
    }

    private void mniShowTooltip_Click( object sender, EventArgs e )
    {
      mniShowTooltip.Checked = !mniShowTooltip.Checked;

      toolOperations.AutoSize = false;
      toolOperations.DropDownArrows = false;
      if( !mniShowTooltip.Checked )
      {
        m_toolText.Clear();
        foreach( ToolBarButton btn in toolOperations.Buttons )
        {
          m_toolText[ btn ] = btn.Text;
          btn.Text = "";
        }
      }
      else
      {
        foreach( ToolBarButton btn in toolOperations.Buttons )
        {
          btn.Text = ( string )m_toolText[ btn ];
        }
        m_toolText.Clear();
      }
      toolOperations.DropDownArrows = true;
      toolOperations.AutoSize = true;
    }

    private void cntrlEdit_TextChanged( object sender, EventArgs e )
    {
		this.bPendingSave = true;
		if( this.ScriptChanged != null )
		{
			this.ScriptChanged( this, EventArgs.Empty );
		}
    }
    
    #endregion

    #region Class ObjectBrowser handlers    

    public void ObjectBrowser_NodeDoubleClick(object sender, NodeDoubleClickEventArgs e)
    {
      string eventName = e.NodeName;
      TagContainer tag = e.TagContainer;

      switch(tag.NodeType)
      {
          // User clicked on event node
        case TreeNodeType.Event:
			string unwrappedscript = ScriptWrapper.GetUnwrappedScript(this.ScriptLanguage, this.ScriptText, ref this.nameSpaceDefine, ref this.assemblyDirectives, 
				ref this.globalCode, this.ScriptName, this.BaseClass.FullName);
			string eventhandlerscript = ScriptWrapper.GetEventHandlerScriptByType(this.ScriptLanguage, (tag as TagEventContainer).ClassName, tag.EventInfo);
			string eventstart = ScriptWrapper.GetEventSubscriberScript(this.ScriptLanguage, this.RootNamespace, this.ScriptName, (tag as TagEventContainer).ClassName, tag.EventInfo);		   
		    string eventstop = ScriptWrapper.GetEventUnsubscriberScript(this.ScriptLanguage, this.RootNamespace, this.ScriptName, (tag as TagEventContainer).ClassName, tag.EventInfo);
		    if(eventstart != String.Empty)
				unwrappedscript = ScriptWrapper.AddEventSubscriptionToScript(this.ScriptLanguage, unwrappedscript, this.scriptRef.ScriptStartCall, eventstart);
			if(eventstop != String.Empty)
				unwrappedscript = ScriptWrapper.AddEventSubscriptionToScript(this.ScriptLanguage, unwrappedscript, this.scriptRef.ScriptStopCall, eventstop);		    
			unwrappedscript += "\r\n\n" + eventhandlerscript;
			this.ScriptText = ScriptWrapper.GetWrappedScript(this.ScriptLanguage, unwrappedscript, this.nameSpaceDefine, this.assemblyDirectives, 
				this.globalCode, this.ScriptName, this.BaseClass.FullName);
          break;
          // User clicked on event handler node
        case TreeNodeType.EventHandler:          
          break;
      }
    }
    #endregion

	  protected virtual void OnLanguageChange(ScriptLanguages language)
	  {
		  cntrlEdit.NewFile();

		  switch (language)
		  {
			  case ScriptLanguages.CSharp:
				  cntrlEdit.ApplyConfiguration(KnownLanguages.CSharp);
				  cntrlEdit.FileName = "Untitled.cs";
				  break;

			  case ScriptLanguages.VisualBasic:
				  cntrlEdit.ApplyConfiguration(KnownLanguages.VBNET);
				  cntrlEdit.FileName = "Untitled.vb";
				  break;

			  case ScriptLanguages.JScript:
				  cntrlEdit.ApplyConfiguration(KnownLanguages.JScript);
				  cntrlEdit.FileName = "Untitled.js";
				  break;
		  }
	  }

	  protected virtual void OnScriptChange()
	  {
		  this.UpdateEdit();
	  }

	  private void SelectLanguage(ScriptLanguages language)
	  {
		  objectBrowser.ScriptLanguage = language;
		  string selectedlanguage = TypeDescriptor.GetConverter(language).ConvertToString(language);
		  if((string)this.cbLanguage.SelectedItem != selectedlanguage )
		  {
			  this.cbLanguage.SelectedItem = selectedlanguage;
		  }
		  this.OnLanguageChange( language );
	  }      

    /// <summary>
    /// Saved current script from EditControl.
    /// </summary>
    private void SaveCurrentScript()
    {
		// Saves edited script in member variable
	    this.m_strScript = this.cntrlEdit.Text;      
		this.bPendingSave = false;
    }       

    /// <summary>
    /// Copy script code to Edit control.
    /// </summary>
    private void UpdateEdit()
    {
		this.cntrlEdit.Text = this.m_strScript;	
		SetConfiguration(this.ScriptLanguage);
    }

	  /// <summary>
	  /// Loads the specified configuration settings to the Edit Control.
	  /// </summary>
	  private void SetConfiguration(ScriptLanguages language)
	  {
		  switch (language.ToString())
		  {
			  case "CSharp" : 
				  this.cntrlEdit.ApplyConfiguration(KnownLanguages.CSharp);
				  break;
			  case "VisualBasic" : 
				  this.cntrlEdit.ApplyConfiguration(KnownLanguages.VBNET);
				  break;
			  case "JScript" : 
				  this.cntrlEdit.ApplyConfiguration(KnownLanguages.JScript);
				  break;
		  }
	  }




    /// <summary>
    /// Adds the assembly to the list of referenced assemblies.
    /// </summary>
    /// <param name="assembly">New assembly to reference.</param>
    /// <exception cref="System.ArgumentException">
    /// When assembly is null.
    /// </exception>	
    public void AddAssemblyReference( string assembly )
    {
      if( ( assembly == null ) || ( assembly.Length == 0 ) )
      {
        throw new ArgumentException( "assembly" );
      }
      if( this.lbAssemblyReferences.Items.Contains( assembly ) == false )
      {
        this.lbAssemblyReferences.Items.Add( assembly );
      }
    }

    public void ClearAssemblyReferences()
    {
      this.lbAssemblyReferences.Items.Clear();
    }

    private void btnAddAssemblyRef_Click( object sender, EventArgs e )
    {
      try
      {
        if( this.folderBrowser1.ShowDialog() == DialogResult.OK )
        {
          if( ( this.folderBrowser1.DirectoryPath != String.Empty ) && ( File.Exists( this.folderBrowser1.DirectoryPath ) == true ) )
          {
            AssemblyDescriptor newassembly = new AssemblyDescriptor( this.folderBrowser1.DirectoryPath );
            if( newassembly != null )
            {
              bool bexisting = false;
              foreach( AssemblyDescriptor assemblydesc in this.lbAssemblyReferences.Items )
              {
                if( assemblydesc.DisplayName == newassembly.DisplayName )
                {
                  bexisting = true;
                  break;
                }
              }
              if( bexisting == false )
              {
                this.lbAssemblyReferences.Items.Add( newassembly );
              }
            }
          }
        }
      }
      catch( Exception ex )
      {
        Console.WriteLine( ex.ToString() );
      }
    }

    private void btnRemoveAssemblyRef_Click( object sender, EventArgs e )
    {
      int selIdx = this.lbAssemblyReferences.SelectedIndex;
      if( selIdx >= 0 )
      {
        this.lbAssemblyReferences.Items.RemoveAt( selIdx );
      }
    }

    private void btnMoveAssemblyRefUp_Click( object sender, EventArgs e )
    {
      int selIdx = this.lbAssemblyReferences.SelectedIndex;
      if( selIdx > 0 )
      {
        string tempAssemblyName = ( string )this.lbAssemblyReferences.Items[ selIdx ];
        this.lbAssemblyReferences.Items[ selIdx ] = this.lbAssemblyReferences.Items[ selIdx - 1 ];
        this.lbAssemblyReferences.Items[ selIdx - 1 ] = tempAssemblyName;
        this.lbAssemblyReferences.SelectedIndex = selIdx - 1;
      }
    }

    private void btnMoveAssemblyRefDown_Click( object sender, EventArgs e )
    {
      int selIdx = this.lbAssemblyReferences.SelectedIndex;
      if( selIdx >= 0 && selIdx < ( lbAssemblyReferences.Items.Count - 1 ) )
      {
        string tempAssemblyName = ( string )this.lbAssemblyReferences.Items[ selIdx ];
        this.lbAssemblyReferences.Items[ selIdx ] = this.lbAssemblyReferences.Items[ selIdx + 1 ];
        this.lbAssemblyReferences.Items[ selIdx + 1 ] = tempAssemblyName;
        this.lbAssemblyReferences.SelectedIndex = selIdx + 1;
      }
    }

    private void folderBrowser1_BrowseCallback( object sender, FolderBrowserCallbackEventArgs e )
    {
      if( ( e.Window != null ) && ( e.Window.Handle != IntPtr.Zero ) )
      {
        String caption = new String( 't', 1001 );
        NativeMethods.GetWindowText( e.Window.Handle, caption, 1001 );
        if( caption != "Select Assembly" )
        {
          NativeMethods.SetWindowText( e.Window.Handle, "Add Assembly Reference" );
        }
      }
    }

    private void tbGeneralPanel_TextChanged( object sender, EventArgs e )
    {
      this.bPendingSave = true;
    }

    private void tbScriptName_TextChanged( object sender, EventArgs e )
    {
      this.bPendingSave = true;
    }

    public void InitializeScriptEditor( ScriptingManager scriptmanager )
    {
		this.ScriptingManager = scriptmanager;
	    if( scriptmanager.Script != null )
        {
			this.Script = scriptmanager.Script;
        }
    }

    /// <summary>
    /// Loads the data from the script into the editor.
    /// </summary>
    /// <param name="script">Script to load into the editor</param>
    public virtual void InitializeScriptEditor( Script script )
    {
      this.ScriptName = script.Name;
      this.EntryPoint = script.EntryPoint;
      this.ScriptLanguage = script.Language;
      this.ScriptText = script.SourceText;
      this.RootMoniker = script.RootMoniker;
      this.RootNamespace = script.RootNamespace;

      this.lbAssemblyReferences.Items.Clear();
      foreach( AssemblyDescriptor assemblydesc in script.AssemblyReferences )
      {
        if( this.lbAssemblyReferences.Items.Contains( assemblydesc ) == false )
        {
          this.lbAssemblyReferences.Items.Add( assemblydesc );
        }
      }

      foreach( ScriptObject scriptableobject in script.GlobalInstances )
      {
        this.AddScriptableObject( scriptableobject );
      }
      foreach( ScriptObject scriptableobject in script.EventSources )
      {
        this.AddScriptableObject( scriptableobject );
      }
    }

    /// <summary>
    /// Updates the script object with the current values in the editor.
    /// </summary>
    /// <param name="script">Script to update</param>
    public void UpdateScript(Script script)
    {
      script.Name = this.ScriptName;
      script.EntryPoint = this.EntryPoint;
      script.Language = this.ScriptLanguage;
      script.SourceText = this.ScriptText;
      script.RootMoniker = this.RootMoniker;
      script.RootNamespace = this.RootNamespace;

      script.AssemblyReferences.Clear();
      foreach( AssemblyDescriptor assemblydesc in this.lbAssemblyReferences.Items )
      {
        script.AssemblyReferences.Add( assemblydesc );
      }
    }
  }

}