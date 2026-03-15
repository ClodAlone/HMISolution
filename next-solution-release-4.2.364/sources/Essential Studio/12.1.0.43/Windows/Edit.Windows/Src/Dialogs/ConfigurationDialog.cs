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
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	#region *** ConfigurationDialog
	/// <summary>
	/// Dialog for customizing EditControl configuration.
	/// </summary>
	public class ConfigurationDialog
		: System.Windows.Forms.Form
	{
		#region Classes
		/// <summary>
		/// Structure for saving start format settings.
		/// </summary>
		internal struct FormatState
		{
			#region Fields
			/// <summary>
			/// Name of the format.
			/// </summary>
			private string m_name;
			/// <summary>
			/// Color of the font.
			/// </summary>
			private Color m_fontColor;
			/// <summary>
			/// Color of the borders.
			/// </summary>
			private Color m_foreColor;
			/// <summary>
			/// Back color.
			/// </summary>
			private Color m_backColor;
			/// <summary>
			/// Lines color.
			/// </summary>
			private Color m_lineColor;
			/// <summary>
			/// Hatch Style of the font.
			/// </summary>
			private HatchStyle m_hatchStyle;
			/// <summary>
			/// Underline Style.
			/// </summary>
			private UnderlineStyle m_undStyle;
			/// <summary>
			/// Underline Weight.
			/// </summary>
			private UnderlineWeight m_undWeight;
			/// <summary>
			/// Font of the format.
			/// </summary>
			private Font m_font;
			#endregion

			#region Properties
			/// <summary>
			/// Returns default state of the format.
			/// </summary>
			public static FormatState Empty
			{
				get
				{
					return new FormatState( string.Empty );
				}
			}
			#endregion

			#region Initialize/Finalize Methods
			/// <summary>
			/// Creates new format state.
			/// </summary>
			/// <param name="name"></param>
			public FormatState( string name )
			{
				if( name == null )
					throw new ArgumentNullException( "name" );

				m_name = name;

				m_fontColor = Color.Empty;
				m_foreColor = Color.Empty;
				m_backColor = Color.Empty;
				m_lineColor = Color.Empty;

				m_hatchStyle = HatchStyle.Percent05;
				m_undStyle = UnderlineStyle.None;
				m_undWeight = UnderlineWeight.Thin;
				m_font = SystemInformation.MenuFont.Clone() as Font;
			}
			#endregion

			#region Public Methods
			/// <summary>
			/// Saves all settings from format to this object.
			/// </summary>
			/// <param name="format"></param>
			/// <returns>TRUE - if settings saved, FLASE - otherwise.</returns>
			public bool SaveState( Format format )
			{
				if( format == null )
					throw new ArgumentNullException( "format" );

				if( format.Name != m_name ) return false;

				m_fontColor = format.FontColor;
				m_foreColor = format.ForeColor;
				m_backColor = format.BackColor;
				m_lineColor = format.LineColor;

				m_hatchStyle = format.HatchStyle;
				m_undStyle = format.UnderlineStyle;
				m_undWeight = format.UnderlineWeight;

				if( m_font != null )
				{
					m_font.Dispose();
				}
				m_font = format.Font.Clone() as Font;

				return true;
			}
			/// <summary>
			/// Restores all settings of format.
			/// </summary>
			/// <param name="format"></param>
			/// <returns>TRUE - if restored, FALSE - otherwise.</returns>
			public bool RestoreState( Format format )
			{
				if( format == null )
					throw new ArgumentNullException( "format" );

				if( format.Name != m_name ) return false;

				format.FontColor = m_fontColor;
				format.ForeColor = m_foreColor;
				format.BackColor = m_backColor;
				format.LineColor = m_lineColor;

				format.HatchStyle = m_hatchStyle;
				format.UnderlineStyle = m_undStyle;
				format.UnderlineWeight = m_undWeight;
				format.Font = m_font;

				return true;
			}
			#endregion
		}
		#endregion

		#region Constants
		/// <summary>
		/// Item in language list for creating new configuration.
		/// </summary>
		private string DEF_NEW_LANGUAGE = Localizer.DEF_CONFIG_CREATE_LANGUAGE;
		/// <summary>
		/// Language changed text.
		/// </summary>
		private string DEF_CHANGED_TEXT = Localizer.DEF_CONFIG_SAVE_OTHERS_CHANGES;
		/// <summary>
		/// Language changed caption.
		/// </summary>
		private string DEF_CHANGED_CAPTION = Localizer.DEF_CONFIG_SAVE_CHANGES;
		/// <summary>
		/// Text for format sample.
		/// </summary>
		private string DEF_FORMAT_SAMPLE = Localizer.DEF_FORMAT_SETTINGS_SAMPLE_TEXT;
		/// <summary>
		/// Name of empty color.
		/// </summary>
		private static string DEF_EMPTY_COLOR = Localizer.DEF_COLOR_EMPTY;
		/// <summary>
		/// Name of empty color.
		/// </summary>
		private static string DEF_NONE_HATCH = Localizer.DEF_COLOR_EMPTY;
		/// <summary>
		/// Root of lexems tree.
		/// </summary>
		private string DEF_LEXEM_ROOT = Localizer.DEF_LEXEM_TREE_ROOT;
		/// <summary>
		/// Width of the preview box.
		/// </summary>
		private const int PREVIEW_BOX_WIDTH = 20;
		/// <summary>
		/// Width of the arrow.
		/// </summary>
		public const int ARROW_WIDTH = 12;
		#endregion

		#region Static Members
		/// <summary>
		/// Holds all names of Colors.
		/// </summary>
		private static ArrayList m_colors;
		/// <summary>
		/// Holds fields of HatchStyle enumeration.
		/// </summary>
		private static ArrayList m_hatchStyle;
		/// <summary>
		/// Holds fields of UnderlineStyle enumeration.
		/// </summary>
		private static ArrayList m_underlineStyle;
		/// <summary>
		/// Holds fields of UnderlineWeight enumeration.
		/// </summary>
		private static ArrayList m_underlineWeight;
		/// <summary>
		/// Holds Format Type enum fields.
		/// </summary>
		private static ArrayList m_formatType;
		#endregion

		#region Form Controls
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnHelp;
		private System.Windows.Forms.TabControl tabsConfiguration;
		private System.Windows.Forms.TabPage tabFileExtensions;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.ComboBox comboLanguages;
		private System.Windows.Forms.Label lblLanguages;
		private System.Windows.Forms.Label lblOneChar;
		private System.Windows.Forms.Label lblMultiChars;
		private System.Windows.Forms.Label lblFileExtensions;
		private System.Windows.Forms.TextBox txtOneChar;
		private Syncfusion.Windows.Forms.Edit.Dialogs.ListItemsEditor lstMulti;
		private Syncfusion.Windows.Forms.Edit.Dialogs.ListItemsEditor lstExtensions;
		private System.Windows.Forms.TabPage tabFormats;
		private System.Windows.Forms.TabPage tabLexems;
		private System.Windows.Forms.SaveFileDialog saveDlg;
		private System.Windows.Forms.Label lblFormatsList;
		private System.Windows.Forms.Button btnAddFormat;
		private System.Windows.Forms.Button btnRemoveFormat;
		private System.Windows.Forms.Label lblSample;
		private System.Windows.Forms.Panel pnlSample;
		private System.Windows.Forms.Button btnFontFormat;
		private System.Windows.Forms.Label lblFontColor;
		private System.Windows.Forms.ComboBox comboFontColor;
		private System.Windows.Forms.Label lblForeColor;
		private System.Windows.Forms.ComboBox comboForeColor;
		private System.Windows.Forms.Label lblBackColor;
		private System.Windows.Forms.ComboBox comboBackColor;
		private System.Windows.Forms.ComboBox comboLineColor;
		private System.Windows.Forms.Label lblLineColor;
		private System.Windows.Forms.Label lblHatchStyle;
		private System.Windows.Forms.ComboBox comboHatchStyle;
		private System.Windows.Forms.Label lblUnderlineStyle;
		private System.Windows.Forms.ComboBox comboUnderlineStyle;
		private System.Windows.Forms.Label lblUnderlineWeight;
		private System.Windows.Forms.ComboBox comboUnderlineWeight;
		private System.Windows.Forms.FontDialog fontDlg;
		private System.Windows.Forms.TreeView treeLexems;
		private System.Windows.Forms.Label lblLexems;
		private System.Windows.Forms.Button btnAddSubLexem;
		private System.Windows.Forms.Button btnRemoveLexem;
		private System.Windows.Forms.Button btnAddLexem;
		private System.Windows.Forms.Label lblBeginToken;
		private System.Windows.Forms.TextBox txtBeginToken;
		private System.Windows.Forms.CheckBox chkBeginToken;
		private System.Windows.Forms.TextBox txtContinueToken;
		private System.Windows.Forms.CheckBox chkContinueToken;
		private System.Windows.Forms.Label lblContinueToken;
		private System.Windows.Forms.Label lblEndToken;
		private System.Windows.Forms.TextBox txtEndToken;
		private System.Windows.Forms.CheckBox chkEndToken;
		private System.Windows.Forms.Label lblFormat;
		private System.Windows.Forms.ComboBox comboFormat;
		private System.Windows.Forms.CheckBox chkOnlyLocals;
		private System.Windows.Forms.Label lblPriority;
		private System.Windows.Forms.NumericUpDown upDownPriority;
		private System.Windows.Forms.ListBox lsbFormats;
		private System.Windows.Forms.CheckBox chkIsComplex;
		private System.Windows.Forms.Button btnRestore;
		private System.Windows.Forms.Button btnSetFonts;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Fields
		/// <summary>
		/// Configurator instance.
		/// </summary>
		private Config m_config;
		/// <summary>
		/// Array of created new languages.
		/// </summary>
		private ArrayList m_newLanguages;
		/// <summary>
		/// Active index of list of languages.
		/// </summary>
		private int m_activeIndex;
		/// <summary>
		/// Information for drawing format sample.
		/// </summary>
		private TextDrawInfo m_info;
		/// <summary>
		/// Default state of formats.
		/// </summary>
		private Hashtable m_formatState;
		/// <summary>
		/// Index of selected lexem.
		/// </summary>
		private ConfigLexem m_activeLexem;
		private System.Windows.Forms.Button btnOpen;
		private System.Windows.Forms.Button btnSaveAs;
		private System.Windows.Forms.OpenFileDialog dlgOpen;
		/// <summary>
		/// Active language of control.
		/// </summary>
		private IConfigLanguage m_activeLang;
		/// <summary>
		/// Name of the last loaded (or saved) configuration file.
		/// </summary>
		private string m_strLoadedConfigurationFileName;
		#endregion

		#region Properties
		/// <summary>
		/// Gets Config instance with all edited languages.
		/// </summary>
		public Config Configurator
		{
			get
			{
				return m_config;
			}
		}
		#endregion

		#region Initialize/Finalize Methods
		/// <summary>
		/// Static constructor.
		/// </summary>
		static ConfigurationDialog()
		{
			m_colors = new ArrayList();
			m_colors.Add( DEF_EMPTY_COLOR );
			m_colors.AddRange( Localizer.GetEnumNames( typeof( KnownColor ) ) );

			m_hatchStyle = new ArrayList();
			m_hatchStyle.Add( DEF_NONE_HATCH );
			m_hatchStyle.AddRange( Localizer.GetEnumNames( typeof( HatchStyle ) ) );

			m_underlineStyle = new ArrayList();
			m_underlineStyle.AddRange( Localizer.GetEnumNames( typeof( UnderlineStyle ) ) );

			m_underlineWeight = new ArrayList();
			m_underlineWeight.AddRange( Localizer.GetEnumNames( typeof( UnderlineWeight ) ) );

			m_formatType = new ArrayList();
			m_formatType.AddRange( Enum.GetNames( typeof( FormatType ) ) );
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		private ConfigurationDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			EnableDoubleBuffering();

			m_newLanguages = new ArrayList();
			m_activeIndex = -1;

			m_info = new TextDrawInfo();
			m_info.Text = DEF_FORMAT_SAMPLE;
			m_info.VerticalAlignment = StringAlignment.Near;
			m_info.DynamicFormattings = null;

			m_formatState = new Hashtable();
		}
		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="configurator"><see cref="Config"/> class instance.</param>
		/// <param name="activeLang">Active configuration language.</param>
		public ConfigurationDialog( Config configurator, IConfigLanguage activeLang )
			: this()
		{
			if( configurator == null ) throw new ArgumentNullException( "configurator" );

			m_activeLang = activeLang;

			// Create new config instance from defined.
			XmlDocument document = new XmlDocument();
			configurator.Save( document );
			m_config = new Config( document );

			m_newLanguages.AddRange( m_config.KnownLanguages );

			InfillLanguageList();
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}

				m_config = null;
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( ConfigurationDialog ) );
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnHelp = new System.Windows.Forms.Button();
			this.tabsConfiguration = new System.Windows.Forms.TabControl();
			this.tabFileExtensions = new System.Windows.Forms.TabPage();
			this.lstExtensions = new Syncfusion.Windows.Forms.Edit.Dialogs.ListItemsEditor();
			this.lstMulti = new Syncfusion.Windows.Forms.Edit.Dialogs.ListItemsEditor();
			this.txtOneChar = new System.Windows.Forms.TextBox();
			this.lblOneChar = new System.Windows.Forms.Label();
			this.lblMultiChars = new System.Windows.Forms.Label();
			this.lblFileExtensions = new System.Windows.Forms.Label();
			this.tabFormats = new System.Windows.Forms.TabPage();
			this.comboFontColor = new System.Windows.Forms.ComboBox();
			this.lblFontColor = new System.Windows.Forms.Label();
			this.btnFontFormat = new System.Windows.Forms.Button();
			this.pnlSample = new System.Windows.Forms.Panel();
			this.lblSample = new System.Windows.Forms.Label();
			this.btnAddFormat = new System.Windows.Forms.Button();
			this.lsbFormats = new System.Windows.Forms.ListBox();
			this.lblFormatsList = new System.Windows.Forms.Label();
			this.btnRemoveFormat = new System.Windows.Forms.Button();
			this.lblForeColor = new System.Windows.Forms.Label();
			this.comboForeColor = new System.Windows.Forms.ComboBox();
			this.lblBackColor = new System.Windows.Forms.Label();
			this.comboBackColor = new System.Windows.Forms.ComboBox();
			this.comboLineColor = new System.Windows.Forms.ComboBox();
			this.lblLineColor = new System.Windows.Forms.Label();
			this.lblHatchStyle = new System.Windows.Forms.Label();
			this.comboHatchStyle = new System.Windows.Forms.ComboBox();
			this.lblUnderlineStyle = new System.Windows.Forms.Label();
			this.comboUnderlineStyle = new System.Windows.Forms.ComboBox();
			this.comboUnderlineWeight = new System.Windows.Forms.ComboBox();
			this.lblUnderlineWeight = new System.Windows.Forms.Label();
			this.btnRestore = new System.Windows.Forms.Button();
			this.btnSetFonts = new System.Windows.Forms.Button();
			this.tabLexems = new System.Windows.Forms.TabPage();
			this.upDownPriority = new System.Windows.Forms.NumericUpDown();
			this.chkOnlyLocals = new System.Windows.Forms.CheckBox();
			this.comboFormat = new System.Windows.Forms.ComboBox();
			this.lblFormat = new System.Windows.Forms.Label();
			this.chkBeginToken = new System.Windows.Forms.CheckBox();
			this.txtBeginToken = new System.Windows.Forms.TextBox();
			this.lblBeginToken = new System.Windows.Forms.Label();
			this.btnAddSubLexem = new System.Windows.Forms.Button();
			this.lblLexems = new System.Windows.Forms.Label();
			this.treeLexems = new System.Windows.Forms.TreeView();
			this.btnRemoveLexem = new System.Windows.Forms.Button();
			this.btnAddLexem = new System.Windows.Forms.Button();
			this.txtContinueToken = new System.Windows.Forms.TextBox();
			this.chkContinueToken = new System.Windows.Forms.CheckBox();
			this.lblContinueToken = new System.Windows.Forms.Label();
			this.lblEndToken = new System.Windows.Forms.Label();
			this.txtEndToken = new System.Windows.Forms.TextBox();
			this.chkEndToken = new System.Windows.Forms.CheckBox();
			this.lblPriority = new System.Windows.Forms.Label();
			this.chkIsComplex = new System.Windows.Forms.CheckBox();
			this.lblLanguages = new System.Windows.Forms.Label();
			this.comboLanguages = new System.Windows.Forms.ComboBox();
			this.btnDelete = new System.Windows.Forms.Button();
			this.saveDlg = new System.Windows.Forms.SaveFileDialog();
			this.fontDlg = new System.Windows.Forms.FontDialog();
			this.btnOpen = new System.Windows.Forms.Button();
			this.btnSaveAs = new System.Windows.Forms.Button();
			this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
			this.tabsConfiguration.SuspendLayout();
			this.tabFileExtensions.SuspendLayout();
			this.tabFormats.SuspendLayout();
			this.tabLexems.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize )( this.upDownPriority ) ).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.AccessibleDescription = resources.GetString( "btnOK.AccessibleDescription" );
			this.btnOK.AccessibleName = resources.GetString( "btnOK.AccessibleName" );
			this.btnOK.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnOK.Anchor" ) ) );
			this.btnOK.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnOK.BackgroundImage" ) ) );
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnOK.Dock" ) ) );
			this.btnOK.Enabled = ( ( bool )( resources.GetObject( "btnOK.Enabled" ) ) );
			this.btnOK.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnOK.FlatStyle" ) ) );
			this.btnOK.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnOK.Font" ) ) );
			this.btnOK.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnOK.Image" ) ) );
			this.btnOK.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnOK.ImageAlign" ) ) );
			this.btnOK.ImageIndex = ( ( int )( resources.GetObject( "btnOK.ImageIndex" ) ) );
			this.btnOK.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnOK.ImeMode" ) ) );
			this.btnOK.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnOK.Location" ) ) );
			this.btnOK.Name = "btnOK";
			this.btnOK.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnOK.RightToLeft" ) ) );
			this.btnOK.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnOK.Size" ) ) );
			this.btnOK.TabIndex = ( ( int )( resources.GetObject( "btnOK.TabIndex" ) ) );
			this.btnOK.Text = resources.GetString( "btnOK.Text" );
			this.btnOK.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnOK.TextAlign" ) ) );
			this.btnOK.Visible = ( ( bool )( resources.GetObject( "btnOK.Visible" ) ) );
			this.btnOK.Click += new System.EventHandler( this.btnOK_Click );
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleDescription = resources.GetString( "btnCancel.AccessibleDescription" );
			this.btnCancel.AccessibleName = resources.GetString( "btnCancel.AccessibleName" );
			this.btnCancel.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnCancel.Anchor" ) ) );
			this.btnCancel.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnCancel.BackgroundImage" ) ) );
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnCancel.Dock" ) ) );
			this.btnCancel.Enabled = ( ( bool )( resources.GetObject( "btnCancel.Enabled" ) ) );
			this.btnCancel.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnCancel.FlatStyle" ) ) );
			this.btnCancel.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnCancel.Font" ) ) );
			this.btnCancel.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnCancel.Image" ) ) );
			this.btnCancel.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnCancel.ImageAlign" ) ) );
			this.btnCancel.ImageIndex = ( ( int )( resources.GetObject( "btnCancel.ImageIndex" ) ) );
			this.btnCancel.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnCancel.ImeMode" ) ) );
			this.btnCancel.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnCancel.Location" ) ) );
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnCancel.RightToLeft" ) ) );
			this.btnCancel.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnCancel.Size" ) ) );
			this.btnCancel.TabIndex = ( ( int )( resources.GetObject( "btnCancel.TabIndex" ) ) );
			this.btnCancel.Text = resources.GetString( "btnCancel.Text" );
			this.btnCancel.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnCancel.TextAlign" ) ) );
			this.btnCancel.Visible = ( ( bool )( resources.GetObject( "btnCancel.Visible" ) ) );
			// 
			// btnHelp
			// 
			this.btnHelp.AccessibleDescription = resources.GetString( "btnHelp.AccessibleDescription" );
			this.btnHelp.AccessibleName = resources.GetString( "btnHelp.AccessibleName" );
			this.btnHelp.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnHelp.Anchor" ) ) );
			this.btnHelp.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnHelp.BackgroundImage" ) ) );
			this.btnHelp.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnHelp.Dock" ) ) );
			this.btnHelp.Enabled = ( ( bool )( resources.GetObject( "btnHelp.Enabled" ) ) );
			this.btnHelp.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnHelp.FlatStyle" ) ) );
			this.btnHelp.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnHelp.Font" ) ) );
			this.btnHelp.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnHelp.Image" ) ) );
			this.btnHelp.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnHelp.ImageAlign" ) ) );
			this.btnHelp.ImageIndex = ( ( int )( resources.GetObject( "btnHelp.ImageIndex" ) ) );
			this.btnHelp.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnHelp.ImeMode" ) ) );
			this.btnHelp.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnHelp.Location" ) ) );
			this.btnHelp.Name = "btnHelp";
			this.btnHelp.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnHelp.RightToLeft" ) ) );
			this.btnHelp.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnHelp.Size" ) ) );
			this.btnHelp.TabIndex = ( ( int )( resources.GetObject( "btnHelp.TabIndex" ) ) );
			this.btnHelp.Text = resources.GetString( "btnHelp.Text" );
			this.btnHelp.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnHelp.TextAlign" ) ) );
			this.btnHelp.Visible = ( ( bool )( resources.GetObject( "btnHelp.Visible" ) ) );
			// 
			// tabsConfiguration
			// 
			this.tabsConfiguration.AccessibleDescription = resources.GetString( "tabsConfiguration.AccessibleDescription" );
			this.tabsConfiguration.AccessibleName = resources.GetString( "tabsConfiguration.AccessibleName" );
			this.tabsConfiguration.Alignment = ( ( System.Windows.Forms.TabAlignment )( resources.GetObject( "tabsConfiguration.Alignment" ) ) );
			this.tabsConfiguration.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "tabsConfiguration.Anchor" ) ) );
			this.tabsConfiguration.Appearance = ( ( System.Windows.Forms.TabAppearance )( resources.GetObject( "tabsConfiguration.Appearance" ) ) );
			this.tabsConfiguration.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "tabsConfiguration.BackgroundImage" ) ) );
			this.tabsConfiguration.Controls.Add( this.tabFileExtensions );
			this.tabsConfiguration.Controls.Add( this.tabFormats );
			this.tabsConfiguration.Controls.Add( this.tabLexems );
			this.tabsConfiguration.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "tabsConfiguration.Dock" ) ) );
			this.tabsConfiguration.Enabled = ( ( bool )( resources.GetObject( "tabsConfiguration.Enabled" ) ) );
			this.tabsConfiguration.Font = ( ( System.Drawing.Font )( resources.GetObject( "tabsConfiguration.Font" ) ) );
			this.tabsConfiguration.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "tabsConfiguration.ImeMode" ) ) );
			this.tabsConfiguration.ItemSize = ( ( System.Drawing.Size )( resources.GetObject( "tabsConfiguration.ItemSize" ) ) );
			this.tabsConfiguration.Location = ( ( System.Drawing.Point )( resources.GetObject( "tabsConfiguration.Location" ) ) );
			this.tabsConfiguration.Name = "tabsConfiguration";
			this.tabsConfiguration.Padding = ( ( System.Drawing.Point )( resources.GetObject( "tabsConfiguration.Padding" ) ) );
			this.tabsConfiguration.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "tabsConfiguration.RightToLeft" ) ) );
			this.tabsConfiguration.SelectedIndex = 0;
			this.tabsConfiguration.ShowToolTips = ( ( bool )( resources.GetObject( "tabsConfiguration.ShowToolTips" ) ) );
			this.tabsConfiguration.Size = ( ( System.Drawing.Size )( resources.GetObject( "tabsConfiguration.Size" ) ) );
			this.tabsConfiguration.TabIndex = ( ( int )( resources.GetObject( "tabsConfiguration.TabIndex" ) ) );
			this.tabsConfiguration.Text = resources.GetString( "tabsConfiguration.Text" );
			this.tabsConfiguration.Visible = ( ( bool )( resources.GetObject( "tabsConfiguration.Visible" ) ) );
			this.tabsConfiguration.SelectedIndexChanged += new System.EventHandler( this.tabsConfiguration_SelectedIndexChanged );
			// 
			// tabFileExtensions
			// 
			this.tabFileExtensions.AccessibleDescription = resources.GetString( "tabFileExtensions.AccessibleDescription" );
			this.tabFileExtensions.AccessibleName = resources.GetString( "tabFileExtensions.AccessibleName" );
			this.tabFileExtensions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "tabFileExtensions.Anchor" ) ) );
			this.tabFileExtensions.AutoScroll = ( ( bool )( resources.GetObject( "tabFileExtensions.AutoScroll" ) ) );
			this.tabFileExtensions.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "tabFileExtensions.AutoScrollMargin" ) ) );
			this.tabFileExtensions.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "tabFileExtensions.AutoScrollMinSize" ) ) );
			this.tabFileExtensions.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "tabFileExtensions.BackgroundImage" ) ) );
			this.tabFileExtensions.Controls.Add( this.lstExtensions );
			this.tabFileExtensions.Controls.Add( this.lstMulti );
			this.tabFileExtensions.Controls.Add( this.txtOneChar );
			this.tabFileExtensions.Controls.Add( this.lblOneChar );
			this.tabFileExtensions.Controls.Add( this.lblMultiChars );
			this.tabFileExtensions.Controls.Add( this.lblFileExtensions );
			this.tabFileExtensions.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "tabFileExtensions.Dock" ) ) );
			this.tabFileExtensions.Enabled = ( ( bool )( resources.GetObject( "tabFileExtensions.Enabled" ) ) );
			this.tabFileExtensions.Font = ( ( System.Drawing.Font )( resources.GetObject( "tabFileExtensions.Font" ) ) );
			this.tabFileExtensions.ImageIndex = ( ( int )( resources.GetObject( "tabFileExtensions.ImageIndex" ) ) );
			this.tabFileExtensions.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "tabFileExtensions.ImeMode" ) ) );
			this.tabFileExtensions.Location = ( ( System.Drawing.Point )( resources.GetObject( "tabFileExtensions.Location" ) ) );
			this.tabFileExtensions.Name = "tabFileExtensions";
			this.tabFileExtensions.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "tabFileExtensions.RightToLeft" ) ) );
			this.tabFileExtensions.Size = ( ( System.Drawing.Size )( resources.GetObject( "tabFileExtensions.Size" ) ) );
			this.tabFileExtensions.TabIndex = ( ( int )( resources.GetObject( "tabFileExtensions.TabIndex" ) ) );
			this.tabFileExtensions.Text = resources.GetString( "tabFileExtensions.Text" );
			this.tabFileExtensions.ToolTipText = resources.GetString( "tabFileExtensions.ToolTipText" );
			this.tabFileExtensions.Visible = ( ( bool )( resources.GetObject( "tabFileExtensions.Visible" ) ) );
			// 
			// lstExtensions
			// 
			this.lstExtensions.AccessibleDescription = resources.GetString( "lstExtensions.AccessibleDescription" );
			this.lstExtensions.AccessibleName = resources.GetString( "lstExtensions.AccessibleName" );
			this.lstExtensions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lstExtensions.Anchor" ) ) );
			this.lstExtensions.AutoScroll = ( ( bool )( resources.GetObject( "lstExtensions.AutoScroll" ) ) );
			this.lstExtensions.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "lstExtensions.AutoScrollMargin" ) ) );
			this.lstExtensions.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "lstExtensions.AutoScrollMinSize" ) ) );
			this.lstExtensions.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "lstExtensions.BackgroundImage" ) ) );
			this.lstExtensions.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lstExtensions.Dock" ) ) );
			this.lstExtensions.Enabled = ( ( bool )( resources.GetObject( "lstExtensions.Enabled" ) ) );
			this.lstExtensions.Example = "\'cs\' OR \'vb\' OR \'txt\'";
			this.lstExtensions.Font = ( ( System.Drawing.Font )( resources.GetObject( "lstExtensions.Font" ) ) );
			this.lstExtensions.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lstExtensions.ImeMode" ) ) );
			this.lstExtensions.Location = ( ( System.Drawing.Point )( resources.GetObject( "lstExtensions.Location" ) ) );
			this.lstExtensions.Name = "lstExtensions";
			this.lstExtensions.ReverseValidation = true;
			this.lstExtensions.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lstExtensions.RightToLeft" ) ) );
			this.lstExtensions.Size = ( ( System.Drawing.Size )( resources.GetObject( "lstExtensions.Size" ) ) );
			this.lstExtensions.TabIndex = ( ( int )( resources.GetObject( "lstExtensions.TabIndex" ) ) );
			this.lstExtensions.Validator = ( ( System.Text.RegularExpressions.Regex )( resources.GetObject( "lstExtensions.Validator" ) ) );
			this.lstExtensions.Visible = ( ( bool )( resources.GetObject( "lstExtensions.Visible" ) ) );
			this.lstExtensions.OnAddClick += new System.EventHandler( this.lstExtensions_OnAddClick );
			// 
			// lstMulti
			// 
			this.lstMulti.AccessibleDescription = resources.GetString( "lstMulti.AccessibleDescription" );
			this.lstMulti.AccessibleName = resources.GetString( "lstMulti.AccessibleName" );
			this.lstMulti.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lstMulti.Anchor" ) ) );
			this.lstMulti.AutoScroll = ( ( bool )( resources.GetObject( "lstMulti.AutoScroll" ) ) );
			this.lstMulti.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "lstMulti.AutoScrollMargin" ) ) );
			this.lstMulti.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "lstMulti.AutoScrollMinSize" ) ) );
			this.lstMulti.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "lstMulti.BackgroundImage" ) ) );
			this.lstMulti.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lstMulti.Dock" ) ) );
			this.lstMulti.Enabled = ( ( bool )( resources.GetObject( "lstMulti.Enabled" ) ) );
			this.lstMulti.Example = "This can be complex constructions which conatins more then one char, for exam" +
				"ple: \'+=\' or \'/*\', \'++\' or \'#region\'";
			this.lstMulti.Font = ( ( System.Drawing.Font )( resources.GetObject( "lstMulti.Font" ) ) );
			this.lstMulti.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lstMulti.ImeMode" ) ) );
			this.lstMulti.Location = ( ( System.Drawing.Point )( resources.GetObject( "lstMulti.Location" ) ) );
			this.lstMulti.Name = "lstMulti";
			this.lstMulti.ReverseValidation = false;
			this.lstMulti.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lstMulti.RightToLeft" ) ) );
			this.lstMulti.Size = ( ( System.Drawing.Size )( resources.GetObject( "lstMulti.Size" ) ) );
			this.lstMulti.TabIndex = ( ( int )( resources.GetObject( "lstMulti.TabIndex" ) ) );
			this.lstMulti.Validator = ( ( System.Text.RegularExpressions.Regex )( resources.GetObject( "lstMulti.Validator" ) ) );
			this.lstMulti.Visible = ( ( bool )( resources.GetObject( "lstMulti.Visible" ) ) );
			// 
			// txtOneChar
			// 
			this.txtOneChar.AccessibleDescription = resources.GetString( "txtOneChar.AccessibleDescription" );
			this.txtOneChar.AccessibleName = resources.GetString( "txtOneChar.AccessibleName" );
			this.txtOneChar.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "txtOneChar.Anchor" ) ) );
			this.txtOneChar.AutoSize = ( ( bool )( resources.GetObject( "txtOneChar.AutoSize" ) ) );
			this.txtOneChar.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "txtOneChar.BackgroundImage" ) ) );
			this.txtOneChar.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "txtOneChar.Dock" ) ) );
			this.txtOneChar.Enabled = ( ( bool )( resources.GetObject( "txtOneChar.Enabled" ) ) );
			this.txtOneChar.Font = ( ( System.Drawing.Font )( resources.GetObject( "txtOneChar.Font" ) ) );
			this.txtOneChar.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "txtOneChar.ImeMode" ) ) );
			this.txtOneChar.Location = ( ( System.Drawing.Point )( resources.GetObject( "txtOneChar.Location" ) ) );
			this.txtOneChar.MaxLength = ( ( int )( resources.GetObject( "txtOneChar.MaxLength" ) ) );
			this.txtOneChar.Multiline = ( ( bool )( resources.GetObject( "txtOneChar.Multiline" ) ) );
			this.txtOneChar.Name = "txtOneChar";
			this.txtOneChar.PasswordChar = ( ( char )( resources.GetObject( "txtOneChar.PasswordChar" ) ) );
			this.txtOneChar.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "txtOneChar.RightToLeft" ) ) );
			this.txtOneChar.ScrollBars = ( ( System.Windows.Forms.ScrollBars )( resources.GetObject( "txtOneChar.ScrollBars" ) ) );
			this.txtOneChar.Size = ( ( System.Drawing.Size )( resources.GetObject( "txtOneChar.Size" ) ) );
			this.txtOneChar.TabIndex = ( ( int )( resources.GetObject( "txtOneChar.TabIndex" ) ) );
			this.txtOneChar.Text = resources.GetString( "txtOneChar.Text" );
			this.txtOneChar.TextAlign = ( ( System.Windows.Forms.HorizontalAlignment )( resources.GetObject( "txtOneChar.TextAlign" ) ) );
			this.txtOneChar.Visible = ( ( bool )( resources.GetObject( "txtOneChar.Visible" ) ) );
			this.txtOneChar.WordWrap = ( ( bool )( resources.GetObject( "txtOneChar.WordWrap" ) ) );
			// 
			// lblOneChar
			// 
			this.lblOneChar.AccessibleDescription = resources.GetString( "lblOneChar.AccessibleDescription" );
			this.lblOneChar.AccessibleName = resources.GetString( "lblOneChar.AccessibleName" );
			this.lblOneChar.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblOneChar.Anchor" ) ) );
			this.lblOneChar.AutoSize = ( ( bool )( resources.GetObject( "lblOneChar.AutoSize" ) ) );
			this.lblOneChar.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblOneChar.Dock" ) ) );
			this.lblOneChar.Enabled = ( ( bool )( resources.GetObject( "lblOneChar.Enabled" ) ) );
			this.lblOneChar.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblOneChar.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblOneChar.Font" ) ) );
			this.lblOneChar.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblOneChar.Image" ) ) );
			this.lblOneChar.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblOneChar.ImageAlign" ) ) );
			this.lblOneChar.ImageIndex = ( ( int )( resources.GetObject( "lblOneChar.ImageIndex" ) ) );
			this.lblOneChar.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblOneChar.ImeMode" ) ) );
			this.lblOneChar.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblOneChar.Location" ) ) );
			this.lblOneChar.Name = "lblOneChar";
			this.lblOneChar.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblOneChar.RightToLeft" ) ) );
			this.lblOneChar.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblOneChar.Size" ) ) );
			this.lblOneChar.TabIndex = ( ( int )( resources.GetObject( "lblOneChar.TabIndex" ) ) );
			this.lblOneChar.Text = resources.GetString( "lblOneChar.Text" );
			this.lblOneChar.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblOneChar.TextAlign" ) ) );
			this.lblOneChar.Visible = ( ( bool )( resources.GetObject( "lblOneChar.Visible" ) ) );
			// 
			// lblMultiChars
			// 
			this.lblMultiChars.AccessibleDescription = resources.GetString( "lblMultiChars.AccessibleDescription" );
			this.lblMultiChars.AccessibleName = resources.GetString( "lblMultiChars.AccessibleName" );
			this.lblMultiChars.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblMultiChars.Anchor" ) ) );
			this.lblMultiChars.AutoSize = ( ( bool )( resources.GetObject( "lblMultiChars.AutoSize" ) ) );
			this.lblMultiChars.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblMultiChars.Dock" ) ) );
			this.lblMultiChars.Enabled = ( ( bool )( resources.GetObject( "lblMultiChars.Enabled" ) ) );
			this.lblMultiChars.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblMultiChars.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblMultiChars.Font" ) ) );
			this.lblMultiChars.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblMultiChars.Image" ) ) );
			this.lblMultiChars.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblMultiChars.ImageAlign" ) ) );
			this.lblMultiChars.ImageIndex = ( ( int )( resources.GetObject( "lblMultiChars.ImageIndex" ) ) );
			this.lblMultiChars.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblMultiChars.ImeMode" ) ) );
			this.lblMultiChars.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblMultiChars.Location" ) ) );
			this.lblMultiChars.Name = "lblMultiChars";
			this.lblMultiChars.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblMultiChars.RightToLeft" ) ) );
			this.lblMultiChars.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblMultiChars.Size" ) ) );
			this.lblMultiChars.TabIndex = ( ( int )( resources.GetObject( "lblMultiChars.TabIndex" ) ) );
			this.lblMultiChars.Text = resources.GetString( "lblMultiChars.Text" );
			this.lblMultiChars.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblMultiChars.TextAlign" ) ) );
			this.lblMultiChars.Visible = ( ( bool )( resources.GetObject( "lblMultiChars.Visible" ) ) );
			// 
			// lblFileExtensions
			// 
			this.lblFileExtensions.AccessibleDescription = resources.GetString( "lblFileExtensions.AccessibleDescription" );
			this.lblFileExtensions.AccessibleName = resources.GetString( "lblFileExtensions.AccessibleName" );
			this.lblFileExtensions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblFileExtensions.Anchor" ) ) );
			this.lblFileExtensions.AutoSize = ( ( bool )( resources.GetObject( "lblFileExtensions.AutoSize" ) ) );
			this.lblFileExtensions.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblFileExtensions.Dock" ) ) );
			this.lblFileExtensions.Enabled = ( ( bool )( resources.GetObject( "lblFileExtensions.Enabled" ) ) );
			this.lblFileExtensions.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblFileExtensions.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblFileExtensions.Font" ) ) );
			this.lblFileExtensions.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblFileExtensions.Image" ) ) );
			this.lblFileExtensions.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblFileExtensions.ImageAlign" ) ) );
			this.lblFileExtensions.ImageIndex = ( ( int )( resources.GetObject( "lblFileExtensions.ImageIndex" ) ) );
			this.lblFileExtensions.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblFileExtensions.ImeMode" ) ) );
			this.lblFileExtensions.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblFileExtensions.Location" ) ) );
			this.lblFileExtensions.Name = "lblFileExtensions";
			this.lblFileExtensions.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblFileExtensions.RightToLeft" ) ) );
			this.lblFileExtensions.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblFileExtensions.Size" ) ) );
			this.lblFileExtensions.TabIndex = ( ( int )( resources.GetObject( "lblFileExtensions.TabIndex" ) ) );
			this.lblFileExtensions.Text = resources.GetString( "lblFileExtensions.Text" );
			this.lblFileExtensions.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblFileExtensions.TextAlign" ) ) );
			this.lblFileExtensions.Visible = ( ( bool )( resources.GetObject( "lblFileExtensions.Visible" ) ) );
			// 
			// tabFormats
			// 
			this.tabFormats.AccessibleDescription = resources.GetString( "tabFormats.AccessibleDescription" );
			this.tabFormats.AccessibleName = resources.GetString( "tabFormats.AccessibleName" );
			this.tabFormats.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "tabFormats.Anchor" ) ) );
			this.tabFormats.AutoScroll = ( ( bool )( resources.GetObject( "tabFormats.AutoScroll" ) ) );
			this.tabFormats.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "tabFormats.AutoScrollMargin" ) ) );
			this.tabFormats.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "tabFormats.AutoScrollMinSize" ) ) );
			this.tabFormats.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "tabFormats.BackgroundImage" ) ) );
			this.tabFormats.Controls.Add( this.comboFontColor );
			this.tabFormats.Controls.Add( this.lblFontColor );
			this.tabFormats.Controls.Add( this.btnFontFormat );
			this.tabFormats.Controls.Add( this.pnlSample );
			this.tabFormats.Controls.Add( this.lblSample );
			this.tabFormats.Controls.Add( this.btnAddFormat );
			this.tabFormats.Controls.Add( this.lsbFormats );
			this.tabFormats.Controls.Add( this.lblFormatsList );
			this.tabFormats.Controls.Add( this.btnRemoveFormat );
			this.tabFormats.Controls.Add( this.lblForeColor );
			this.tabFormats.Controls.Add( this.comboForeColor );
			this.tabFormats.Controls.Add( this.lblBackColor );
			this.tabFormats.Controls.Add( this.comboBackColor );
			this.tabFormats.Controls.Add( this.comboLineColor );
			this.tabFormats.Controls.Add( this.lblLineColor );
			this.tabFormats.Controls.Add( this.lblHatchStyle );
			this.tabFormats.Controls.Add( this.comboHatchStyle );
			this.tabFormats.Controls.Add( this.lblUnderlineStyle );
			this.tabFormats.Controls.Add( this.comboUnderlineStyle );
			this.tabFormats.Controls.Add( this.comboUnderlineWeight );
			this.tabFormats.Controls.Add( this.lblUnderlineWeight );
			this.tabFormats.Controls.Add( this.btnRestore );
			this.tabFormats.Controls.Add( this.btnSetFonts );
			this.tabFormats.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "tabFormats.Dock" ) ) );
			this.tabFormats.Enabled = ( ( bool )( resources.GetObject( "tabFormats.Enabled" ) ) );
			this.tabFormats.Font = ( ( System.Drawing.Font )( resources.GetObject( "tabFormats.Font" ) ) );
			this.tabFormats.ImageIndex = ( ( int )( resources.GetObject( "tabFormats.ImageIndex" ) ) );
			this.tabFormats.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "tabFormats.ImeMode" ) ) );
			this.tabFormats.Location = ( ( System.Drawing.Point )( resources.GetObject( "tabFormats.Location" ) ) );
			this.tabFormats.Name = "tabFormats";
			this.tabFormats.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "tabFormats.RightToLeft" ) ) );
			this.tabFormats.Size = ( ( System.Drawing.Size )( resources.GetObject( "tabFormats.Size" ) ) );
			this.tabFormats.TabIndex = ( ( int )( resources.GetObject( "tabFormats.TabIndex" ) ) );
			this.tabFormats.Text = resources.GetString( "tabFormats.Text" );
			this.tabFormats.ToolTipText = resources.GetString( "tabFormats.ToolTipText" );
			this.tabFormats.Visible = ( ( bool )( resources.GetObject( "tabFormats.Visible" ) ) );
			// 
			// comboFontColor
			// 
			this.comboFontColor.AccessibleDescription = resources.GetString( "comboFontColor.AccessibleDescription" );
			this.comboFontColor.AccessibleName = resources.GetString( "comboFontColor.AccessibleName" );
			this.comboFontColor.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboFontColor.Anchor" ) ) );
			this.comboFontColor.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboFontColor.BackgroundImage" ) ) );
			this.comboFontColor.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboFontColor.Dock" ) ) );
			this.comboFontColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboFontColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboFontColor.Enabled = ( ( bool )( resources.GetObject( "comboFontColor.Enabled" ) ) );
			this.comboFontColor.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboFontColor.Font" ) ) );
			this.comboFontColor.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboFontColor.ImeMode" ) ) );
			this.comboFontColor.IntegralHeight = ( ( bool )( resources.GetObject( "comboFontColor.IntegralHeight" ) ) );
			this.comboFontColor.ItemHeight = ( ( int )( resources.GetObject( "comboFontColor.ItemHeight" ) ) );
			this.comboFontColor.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboFontColor.Location" ) ) );
			this.comboFontColor.MaxDropDownItems = ( ( int )( resources.GetObject( "comboFontColor.MaxDropDownItems" ) ) );
			this.comboFontColor.MaxLength = ( ( int )( resources.GetObject( "comboFontColor.MaxLength" ) ) );
			this.comboFontColor.Name = "comboFontColor";
			this.comboFontColor.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboFontColor.RightToLeft" ) ) );
			this.comboFontColor.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboFontColor.Size" ) ) );
			this.comboFontColor.TabIndex = ( ( int )( resources.GetObject( "comboFontColor.TabIndex" ) ) );
			this.comboFontColor.Text = resources.GetString( "comboFontColor.Text" );
			this.comboFontColor.Visible = ( ( bool )( resources.GetObject( "comboFontColor.Visible" ) ) );
			this.comboFontColor.SelectedIndexChanged += new System.EventHandler( this.Format_Changed );
			this.comboFontColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboFontColor_DrawItem );
			// 
			// lblFontColor
			// 
			this.lblFontColor.AccessibleDescription = resources.GetString( "lblFontColor.AccessibleDescription" );
			this.lblFontColor.AccessibleName = resources.GetString( "lblFontColor.AccessibleName" );
			this.lblFontColor.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblFontColor.Anchor" ) ) );
			this.lblFontColor.AutoSize = ( ( bool )( resources.GetObject( "lblFontColor.AutoSize" ) ) );
			this.lblFontColor.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblFontColor.Dock" ) ) );
			this.lblFontColor.Enabled = ( ( bool )( resources.GetObject( "lblFontColor.Enabled" ) ) );
			this.lblFontColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblFontColor.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblFontColor.Font" ) ) );
			this.lblFontColor.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblFontColor.Image" ) ) );
			this.lblFontColor.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblFontColor.ImageAlign" ) ) );
			this.lblFontColor.ImageIndex = ( ( int )( resources.GetObject( "lblFontColor.ImageIndex" ) ) );
			this.lblFontColor.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblFontColor.ImeMode" ) ) );
			this.lblFontColor.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblFontColor.Location" ) ) );
			this.lblFontColor.Name = "lblFontColor";
			this.lblFontColor.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblFontColor.RightToLeft" ) ) );
			this.lblFontColor.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblFontColor.Size" ) ) );
			this.lblFontColor.TabIndex = ( ( int )( resources.GetObject( "lblFontColor.TabIndex" ) ) );
			this.lblFontColor.Text = resources.GetString( "lblFontColor.Text" );
			this.lblFontColor.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblFontColor.TextAlign" ) ) );
			this.lblFontColor.Visible = ( ( bool )( resources.GetObject( "lblFontColor.Visible" ) ) );
			// 
			// btnFontFormat
			// 
			this.btnFontFormat.AccessibleDescription = resources.GetString( "btnFontFormat.AccessibleDescription" );
			this.btnFontFormat.AccessibleName = resources.GetString( "btnFontFormat.AccessibleName" );
			this.btnFontFormat.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnFontFormat.Anchor" ) ) );
			this.btnFontFormat.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnFontFormat.BackgroundImage" ) ) );
			this.btnFontFormat.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnFontFormat.Dock" ) ) );
			this.btnFontFormat.Enabled = ( ( bool )( resources.GetObject( "btnFontFormat.Enabled" ) ) );
			this.btnFontFormat.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnFontFormat.FlatStyle" ) ) );
			this.btnFontFormat.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnFontFormat.Font" ) ) );
			this.btnFontFormat.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnFontFormat.Image" ) ) );
			this.btnFontFormat.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnFontFormat.ImageAlign" ) ) );
			this.btnFontFormat.ImageIndex = ( ( int )( resources.GetObject( "btnFontFormat.ImageIndex" ) ) );
			this.btnFontFormat.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnFontFormat.ImeMode" ) ) );
			this.btnFontFormat.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnFontFormat.Location" ) ) );
			this.btnFontFormat.Name = "btnFontFormat";
			this.btnFontFormat.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnFontFormat.RightToLeft" ) ) );
			this.btnFontFormat.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnFontFormat.Size" ) ) );
			this.btnFontFormat.TabIndex = ( ( int )( resources.GetObject( "btnFontFormat.TabIndex" ) ) );
			this.btnFontFormat.Text = resources.GetString( "btnFontFormat.Text" );
			this.btnFontFormat.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnFontFormat.TextAlign" ) ) );
			this.btnFontFormat.Visible = ( ( bool )( resources.GetObject( "btnFontFormat.Visible" ) ) );
			this.btnFontFormat.Click += new System.EventHandler( this.btnFontFormat_Click );
			// 
			// pnlSample
			// 
			this.pnlSample.AccessibleDescription = resources.GetString( "pnlSample.AccessibleDescription" );
			this.pnlSample.AccessibleName = resources.GetString( "pnlSample.AccessibleName" );
			this.pnlSample.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "pnlSample.Anchor" ) ) );
			this.pnlSample.AutoScroll = ( ( bool )( resources.GetObject( "pnlSample.AutoScroll" ) ) );
			this.pnlSample.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "pnlSample.AutoScrollMargin" ) ) );
			this.pnlSample.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "pnlSample.AutoScrollMinSize" ) ) );
			this.pnlSample.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "pnlSample.BackgroundImage" ) ) );
			this.pnlSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlSample.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "pnlSample.Dock" ) ) );
			this.pnlSample.Enabled = ( ( bool )( resources.GetObject( "pnlSample.Enabled" ) ) );
			this.pnlSample.Font = ( ( System.Drawing.Font )( resources.GetObject( "pnlSample.Font" ) ) );
			this.pnlSample.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "pnlSample.ImeMode" ) ) );
			this.pnlSample.Location = ( ( System.Drawing.Point )( resources.GetObject( "pnlSample.Location" ) ) );
			this.pnlSample.Name = "pnlSample";
			this.pnlSample.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "pnlSample.RightToLeft" ) ) );
			this.pnlSample.Size = ( ( System.Drawing.Size )( resources.GetObject( "pnlSample.Size" ) ) );
			this.pnlSample.TabIndex = ( ( int )( resources.GetObject( "pnlSample.TabIndex" ) ) );
			this.pnlSample.Text = resources.GetString( "pnlSample.Text" );
			this.pnlSample.Visible = ( ( bool )( resources.GetObject( "pnlSample.Visible" ) ) );
			this.pnlSample.Paint += new System.Windows.Forms.PaintEventHandler( this.pnlSample_Paint );
			// 
			// lblSample
			// 
			this.lblSample.AccessibleDescription = resources.GetString( "lblSample.AccessibleDescription" );
			this.lblSample.AccessibleName = resources.GetString( "lblSample.AccessibleName" );
			this.lblSample.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblSample.Anchor" ) ) );
			this.lblSample.AutoSize = ( ( bool )( resources.GetObject( "lblSample.AutoSize" ) ) );
			this.lblSample.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblSample.Dock" ) ) );
			this.lblSample.Enabled = ( ( bool )( resources.GetObject( "lblSample.Enabled" ) ) );
			this.lblSample.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblSample.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblSample.Font" ) ) );
			this.lblSample.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblSample.Image" ) ) );
			this.lblSample.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblSample.ImageAlign" ) ) );
			this.lblSample.ImageIndex = ( ( int )( resources.GetObject( "lblSample.ImageIndex" ) ) );
			this.lblSample.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblSample.ImeMode" ) ) );
			this.lblSample.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblSample.Location" ) ) );
			this.lblSample.Name = "lblSample";
			this.lblSample.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblSample.RightToLeft" ) ) );
			this.lblSample.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblSample.Size" ) ) );
			this.lblSample.TabIndex = ( ( int )( resources.GetObject( "lblSample.TabIndex" ) ) );
			this.lblSample.Text = resources.GetString( "lblSample.Text" );
			this.lblSample.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblSample.TextAlign" ) ) );
			this.lblSample.Visible = ( ( bool )( resources.GetObject( "lblSample.Visible" ) ) );
			// 
			// btnAddFormat
			// 
			this.btnAddFormat.AccessibleDescription = resources.GetString( "btnAddFormat.AccessibleDescription" );
			this.btnAddFormat.AccessibleName = resources.GetString( "btnAddFormat.AccessibleName" );
			this.btnAddFormat.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnAddFormat.Anchor" ) ) );
			this.btnAddFormat.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnAddFormat.BackgroundImage" ) ) );
			this.btnAddFormat.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnAddFormat.Dock" ) ) );
			this.btnAddFormat.Enabled = ( ( bool )( resources.GetObject( "btnAddFormat.Enabled" ) ) );
			this.btnAddFormat.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnAddFormat.FlatStyle" ) ) );
			this.btnAddFormat.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnAddFormat.Font" ) ) );
			this.btnAddFormat.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnAddFormat.Image" ) ) );
			this.btnAddFormat.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAddFormat.ImageAlign" ) ) );
			this.btnAddFormat.ImageIndex = ( ( int )( resources.GetObject( "btnAddFormat.ImageIndex" ) ) );
			this.btnAddFormat.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnAddFormat.ImeMode" ) ) );
			this.btnAddFormat.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnAddFormat.Location" ) ) );
			this.btnAddFormat.Name = "btnAddFormat";
			this.btnAddFormat.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnAddFormat.RightToLeft" ) ) );
			this.btnAddFormat.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnAddFormat.Size" ) ) );
			this.btnAddFormat.TabIndex = ( ( int )( resources.GetObject( "btnAddFormat.TabIndex" ) ) );
			this.btnAddFormat.Text = resources.GetString( "btnAddFormat.Text" );
			this.btnAddFormat.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAddFormat.TextAlign" ) ) );
			this.btnAddFormat.Visible = ( ( bool )( resources.GetObject( "btnAddFormat.Visible" ) ) );
			this.btnAddFormat.Click += new System.EventHandler( this.btnAddFormat_Click );
			// 
			// lsbFormats
			// 
			this.lsbFormats.AccessibleDescription = resources.GetString( "lsbFormats.AccessibleDescription" );
			this.lsbFormats.AccessibleName = resources.GetString( "lsbFormats.AccessibleName" );
			this.lsbFormats.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lsbFormats.Anchor" ) ) );
			this.lsbFormats.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "lsbFormats.BackgroundImage" ) ) );
			this.lsbFormats.ColumnWidth = ( ( int )( resources.GetObject( "lsbFormats.ColumnWidth" ) ) );
			this.lsbFormats.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lsbFormats.Dock" ) ) );
			this.lsbFormats.Enabled = ( ( bool )( resources.GetObject( "lsbFormats.Enabled" ) ) );
			this.lsbFormats.Font = ( ( System.Drawing.Font )( resources.GetObject( "lsbFormats.Font" ) ) );
			this.lsbFormats.HorizontalExtent = ( ( int )( resources.GetObject( "lsbFormats.HorizontalExtent" ) ) );
			this.lsbFormats.HorizontalScrollbar = ( ( bool )( resources.GetObject( "lsbFormats.HorizontalScrollbar" ) ) );
			this.lsbFormats.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lsbFormats.ImeMode" ) ) );
			this.lsbFormats.IntegralHeight = ( ( bool )( resources.GetObject( "lsbFormats.IntegralHeight" ) ) );
			this.lsbFormats.ItemHeight = ( ( int )( resources.GetObject( "lsbFormats.ItemHeight" ) ) );
			this.lsbFormats.Location = ( ( System.Drawing.Point )( resources.GetObject( "lsbFormats.Location" ) ) );
			this.lsbFormats.Name = "lsbFormats";
			this.lsbFormats.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lsbFormats.RightToLeft" ) ) );
			this.lsbFormats.ScrollAlwaysVisible = ( ( bool )( resources.GetObject( "lsbFormats.ScrollAlwaysVisible" ) ) );
			this.lsbFormats.Size = ( ( System.Drawing.Size )( resources.GetObject( "lsbFormats.Size" ) ) );
			this.lsbFormats.TabIndex = ( ( int )( resources.GetObject( "lsbFormats.TabIndex" ) ) );
			this.lsbFormats.Visible = ( ( bool )( resources.GetObject( "lsbFormats.Visible" ) ) );
			this.lsbFormats.SelectedIndexChanged += new System.EventHandler( this.lsbFormats_SelectedIndexChanged );
			// 
			// lblFormatsList
			// 
			this.lblFormatsList.AccessibleDescription = resources.GetString( "lblFormatsList.AccessibleDescription" );
			this.lblFormatsList.AccessibleName = resources.GetString( "lblFormatsList.AccessibleName" );
			this.lblFormatsList.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblFormatsList.Anchor" ) ) );
			this.lblFormatsList.AutoSize = ( ( bool )( resources.GetObject( "lblFormatsList.AutoSize" ) ) );
			this.lblFormatsList.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblFormatsList.Dock" ) ) );
			this.lblFormatsList.Enabled = ( ( bool )( resources.GetObject( "lblFormatsList.Enabled" ) ) );
			this.lblFormatsList.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblFormatsList.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblFormatsList.Font" ) ) );
			this.lblFormatsList.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblFormatsList.Image" ) ) );
			this.lblFormatsList.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblFormatsList.ImageAlign" ) ) );
			this.lblFormatsList.ImageIndex = ( ( int )( resources.GetObject( "lblFormatsList.ImageIndex" ) ) );
			this.lblFormatsList.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblFormatsList.ImeMode" ) ) );
			this.lblFormatsList.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblFormatsList.Location" ) ) );
			this.lblFormatsList.Name = "lblFormatsList";
			this.lblFormatsList.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblFormatsList.RightToLeft" ) ) );
			this.lblFormatsList.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblFormatsList.Size" ) ) );
			this.lblFormatsList.TabIndex = ( ( int )( resources.GetObject( "lblFormatsList.TabIndex" ) ) );
			this.lblFormatsList.Text = resources.GetString( "lblFormatsList.Text" );
			this.lblFormatsList.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblFormatsList.TextAlign" ) ) );
			this.lblFormatsList.Visible = ( ( bool )( resources.GetObject( "lblFormatsList.Visible" ) ) );
			// 
			// btnRemoveFormat
			// 
			this.btnRemoveFormat.AccessibleDescription = resources.GetString( "btnRemoveFormat.AccessibleDescription" );
			this.btnRemoveFormat.AccessibleName = resources.GetString( "btnRemoveFormat.AccessibleName" );
			this.btnRemoveFormat.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnRemoveFormat.Anchor" ) ) );
			this.btnRemoveFormat.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnRemoveFormat.BackgroundImage" ) ) );
			this.btnRemoveFormat.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnRemoveFormat.Dock" ) ) );
			this.btnRemoveFormat.Enabled = ( ( bool )( resources.GetObject( "btnRemoveFormat.Enabled" ) ) );
			this.btnRemoveFormat.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnRemoveFormat.FlatStyle" ) ) );
			this.btnRemoveFormat.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnRemoveFormat.Font" ) ) );
			this.btnRemoveFormat.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnRemoveFormat.Image" ) ) );
			this.btnRemoveFormat.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnRemoveFormat.ImageAlign" ) ) );
			this.btnRemoveFormat.ImageIndex = ( ( int )( resources.GetObject( "btnRemoveFormat.ImageIndex" ) ) );
			this.btnRemoveFormat.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnRemoveFormat.ImeMode" ) ) );
			this.btnRemoveFormat.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnRemoveFormat.Location" ) ) );
			this.btnRemoveFormat.Name = "btnRemoveFormat";
			this.btnRemoveFormat.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnRemoveFormat.RightToLeft" ) ) );
			this.btnRemoveFormat.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnRemoveFormat.Size" ) ) );
			this.btnRemoveFormat.TabIndex = ( ( int )( resources.GetObject( "btnRemoveFormat.TabIndex" ) ) );
			this.btnRemoveFormat.Text = resources.GetString( "btnRemoveFormat.Text" );
			this.btnRemoveFormat.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnRemoveFormat.TextAlign" ) ) );
			this.btnRemoveFormat.Visible = ( ( bool )( resources.GetObject( "btnRemoveFormat.Visible" ) ) );
			this.btnRemoveFormat.Click += new System.EventHandler( this.btnRemoveFormat_Click );
			// 
			// lblForeColor
			// 
			this.lblForeColor.AccessibleDescription = resources.GetString( "lblForeColor.AccessibleDescription" );
			this.lblForeColor.AccessibleName = resources.GetString( "lblForeColor.AccessibleName" );
			this.lblForeColor.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblForeColor.Anchor" ) ) );
			this.lblForeColor.AutoSize = ( ( bool )( resources.GetObject( "lblForeColor.AutoSize" ) ) );
			this.lblForeColor.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblForeColor.Dock" ) ) );
			this.lblForeColor.Enabled = ( ( bool )( resources.GetObject( "lblForeColor.Enabled" ) ) );
			this.lblForeColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblForeColor.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblForeColor.Font" ) ) );
			this.lblForeColor.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblForeColor.Image" ) ) );
			this.lblForeColor.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblForeColor.ImageAlign" ) ) );
			this.lblForeColor.ImageIndex = ( ( int )( resources.GetObject( "lblForeColor.ImageIndex" ) ) );
			this.lblForeColor.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblForeColor.ImeMode" ) ) );
			this.lblForeColor.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblForeColor.Location" ) ) );
			this.lblForeColor.Name = "lblForeColor";
			this.lblForeColor.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblForeColor.RightToLeft" ) ) );
			this.lblForeColor.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblForeColor.Size" ) ) );
			this.lblForeColor.TabIndex = ( ( int )( resources.GetObject( "lblForeColor.TabIndex" ) ) );
			this.lblForeColor.Text = resources.GetString( "lblForeColor.Text" );
			this.lblForeColor.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblForeColor.TextAlign" ) ) );
			this.lblForeColor.Visible = ( ( bool )( resources.GetObject( "lblForeColor.Visible" ) ) );
			// 
			// comboForeColor
			// 
			this.comboForeColor.AccessibleDescription = resources.GetString( "comboForeColor.AccessibleDescription" );
			this.comboForeColor.AccessibleName = resources.GetString( "comboForeColor.AccessibleName" );
			this.comboForeColor.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboForeColor.Anchor" ) ) );
			this.comboForeColor.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboForeColor.BackgroundImage" ) ) );
			this.comboForeColor.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboForeColor.Dock" ) ) );
			this.comboForeColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboForeColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboForeColor.Enabled = ( ( bool )( resources.GetObject( "comboForeColor.Enabled" ) ) );
			this.comboForeColor.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboForeColor.Font" ) ) );
			this.comboForeColor.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboForeColor.ImeMode" ) ) );
			this.comboForeColor.IntegralHeight = ( ( bool )( resources.GetObject( "comboForeColor.IntegralHeight" ) ) );
			this.comboForeColor.ItemHeight = ( ( int )( resources.GetObject( "comboForeColor.ItemHeight" ) ) );
			this.comboForeColor.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboForeColor.Location" ) ) );
			this.comboForeColor.MaxDropDownItems = ( ( int )( resources.GetObject( "comboForeColor.MaxDropDownItems" ) ) );
			this.comboForeColor.MaxLength = ( ( int )( resources.GetObject( "comboForeColor.MaxLength" ) ) );
			this.comboForeColor.Name = "comboForeColor";
			this.comboForeColor.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboForeColor.RightToLeft" ) ) );
			this.comboForeColor.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboForeColor.Size" ) ) );
			this.comboForeColor.TabIndex = ( ( int )( resources.GetObject( "comboForeColor.TabIndex" ) ) );
			this.comboForeColor.Text = resources.GetString( "comboForeColor.Text" );
			this.comboForeColor.Visible = ( ( bool )( resources.GetObject( "comboForeColor.Visible" ) ) );
			this.comboForeColor.SelectedIndexChanged += new System.EventHandler( this.Format_Changed );
			this.comboForeColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboFontColor_DrawItem );
			// 
			// lblBackColor
			// 
			this.lblBackColor.AccessibleDescription = resources.GetString( "lblBackColor.AccessibleDescription" );
			this.lblBackColor.AccessibleName = resources.GetString( "lblBackColor.AccessibleName" );
			this.lblBackColor.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblBackColor.Anchor" ) ) );
			this.lblBackColor.AutoSize = ( ( bool )( resources.GetObject( "lblBackColor.AutoSize" ) ) );
			this.lblBackColor.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblBackColor.Dock" ) ) );
			this.lblBackColor.Enabled = ( ( bool )( resources.GetObject( "lblBackColor.Enabled" ) ) );
			this.lblBackColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblBackColor.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblBackColor.Font" ) ) );
			this.lblBackColor.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblBackColor.Image" ) ) );
			this.lblBackColor.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblBackColor.ImageAlign" ) ) );
			this.lblBackColor.ImageIndex = ( ( int )( resources.GetObject( "lblBackColor.ImageIndex" ) ) );
			this.lblBackColor.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblBackColor.ImeMode" ) ) );
			this.lblBackColor.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblBackColor.Location" ) ) );
			this.lblBackColor.Name = "lblBackColor";
			this.lblBackColor.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblBackColor.RightToLeft" ) ) );
			this.lblBackColor.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblBackColor.Size" ) ) );
			this.lblBackColor.TabIndex = ( ( int )( resources.GetObject( "lblBackColor.TabIndex" ) ) );
			this.lblBackColor.Text = resources.GetString( "lblBackColor.Text" );
			this.lblBackColor.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblBackColor.TextAlign" ) ) );
			this.lblBackColor.Visible = ( ( bool )( resources.GetObject( "lblBackColor.Visible" ) ) );
			// 
			// comboBackColor
			// 
			this.comboBackColor.AccessibleDescription = resources.GetString( "comboBackColor.AccessibleDescription" );
			this.comboBackColor.AccessibleName = resources.GetString( "comboBackColor.AccessibleName" );
			this.comboBackColor.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboBackColor.Anchor" ) ) );
			this.comboBackColor.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboBackColor.BackgroundImage" ) ) );
			this.comboBackColor.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboBackColor.Dock" ) ) );
			this.comboBackColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBackColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBackColor.Enabled = ( ( bool )( resources.GetObject( "comboBackColor.Enabled" ) ) );
			this.comboBackColor.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboBackColor.Font" ) ) );
			this.comboBackColor.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboBackColor.ImeMode" ) ) );
			this.comboBackColor.IntegralHeight = ( ( bool )( resources.GetObject( "comboBackColor.IntegralHeight" ) ) );
			this.comboBackColor.ItemHeight = ( ( int )( resources.GetObject( "comboBackColor.ItemHeight" ) ) );
			this.comboBackColor.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboBackColor.Location" ) ) );
			this.comboBackColor.MaxDropDownItems = ( ( int )( resources.GetObject( "comboBackColor.MaxDropDownItems" ) ) );
			this.comboBackColor.MaxLength = ( ( int )( resources.GetObject( "comboBackColor.MaxLength" ) ) );
			this.comboBackColor.Name = "comboBackColor";
			this.comboBackColor.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboBackColor.RightToLeft" ) ) );
			this.comboBackColor.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboBackColor.Size" ) ) );
			this.comboBackColor.TabIndex = ( ( int )( resources.GetObject( "comboBackColor.TabIndex" ) ) );
			this.comboBackColor.Text = resources.GetString( "comboBackColor.Text" );
			this.comboBackColor.Visible = ( ( bool )( resources.GetObject( "comboBackColor.Visible" ) ) );
			this.comboBackColor.SelectedIndexChanged += new System.EventHandler( this.Format_Changed );
			this.comboBackColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboFontColor_DrawItem );
			// 
			// comboLineColor
			// 
			this.comboLineColor.AccessibleDescription = resources.GetString( "comboLineColor.AccessibleDescription" );
			this.comboLineColor.AccessibleName = resources.GetString( "comboLineColor.AccessibleName" );
			this.comboLineColor.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboLineColor.Anchor" ) ) );
			this.comboLineColor.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboLineColor.BackgroundImage" ) ) );
			this.comboLineColor.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboLineColor.Dock" ) ) );
			this.comboLineColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboLineColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLineColor.Enabled = ( ( bool )( resources.GetObject( "comboLineColor.Enabled" ) ) );
			this.comboLineColor.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboLineColor.Font" ) ) );
			this.comboLineColor.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboLineColor.ImeMode" ) ) );
			this.comboLineColor.IntegralHeight = ( ( bool )( resources.GetObject( "comboLineColor.IntegralHeight" ) ) );
			this.comboLineColor.ItemHeight = ( ( int )( resources.GetObject( "comboLineColor.ItemHeight" ) ) );
			this.comboLineColor.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboLineColor.Location" ) ) );
			this.comboLineColor.MaxDropDownItems = ( ( int )( resources.GetObject( "comboLineColor.MaxDropDownItems" ) ) );
			this.comboLineColor.MaxLength = ( ( int )( resources.GetObject( "comboLineColor.MaxLength" ) ) );
			this.comboLineColor.Name = "comboLineColor";
			this.comboLineColor.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboLineColor.RightToLeft" ) ) );
			this.comboLineColor.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboLineColor.Size" ) ) );
			this.comboLineColor.TabIndex = ( ( int )( resources.GetObject( "comboLineColor.TabIndex" ) ) );
			this.comboLineColor.Text = resources.GetString( "comboLineColor.Text" );
			this.comboLineColor.Visible = ( ( bool )( resources.GetObject( "comboLineColor.Visible" ) ) );
			this.comboLineColor.SelectedIndexChanged += new System.EventHandler( this.Format_Changed );
			this.comboLineColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboFontColor_DrawItem );
			// 
			// lblLineColor
			// 
			this.lblLineColor.AccessibleDescription = resources.GetString( "lblLineColor.AccessibleDescription" );
			this.lblLineColor.AccessibleName = resources.GetString( "lblLineColor.AccessibleName" );
			this.lblLineColor.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblLineColor.Anchor" ) ) );
			this.lblLineColor.AutoSize = ( ( bool )( resources.GetObject( "lblLineColor.AutoSize" ) ) );
			this.lblLineColor.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblLineColor.Dock" ) ) );
			this.lblLineColor.Enabled = ( ( bool )( resources.GetObject( "lblLineColor.Enabled" ) ) );
			this.lblLineColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblLineColor.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblLineColor.Font" ) ) );
			this.lblLineColor.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblLineColor.Image" ) ) );
			this.lblLineColor.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblLineColor.ImageAlign" ) ) );
			this.lblLineColor.ImageIndex = ( ( int )( resources.GetObject( "lblLineColor.ImageIndex" ) ) );
			this.lblLineColor.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblLineColor.ImeMode" ) ) );
			this.lblLineColor.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblLineColor.Location" ) ) );
			this.lblLineColor.Name = "lblLineColor";
			this.lblLineColor.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblLineColor.RightToLeft" ) ) );
			this.lblLineColor.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblLineColor.Size" ) ) );
			this.lblLineColor.TabIndex = ( ( int )( resources.GetObject( "lblLineColor.TabIndex" ) ) );
			this.lblLineColor.Text = resources.GetString( "lblLineColor.Text" );
			this.lblLineColor.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblLineColor.TextAlign" ) ) );
			this.lblLineColor.Visible = ( ( bool )( resources.GetObject( "lblLineColor.Visible" ) ) );
			// 
			// lblHatchStyle
			// 
			this.lblHatchStyle.AccessibleDescription = resources.GetString( "lblHatchStyle.AccessibleDescription" );
			this.lblHatchStyle.AccessibleName = resources.GetString( "lblHatchStyle.AccessibleName" );
			this.lblHatchStyle.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblHatchStyle.Anchor" ) ) );
			this.lblHatchStyle.AutoSize = ( ( bool )( resources.GetObject( "lblHatchStyle.AutoSize" ) ) );
			this.lblHatchStyle.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblHatchStyle.Dock" ) ) );
			this.lblHatchStyle.Enabled = ( ( bool )( resources.GetObject( "lblHatchStyle.Enabled" ) ) );
			this.lblHatchStyle.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblHatchStyle.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblHatchStyle.Font" ) ) );
			this.lblHatchStyle.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblHatchStyle.Image" ) ) );
			this.lblHatchStyle.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblHatchStyle.ImageAlign" ) ) );
			this.lblHatchStyle.ImageIndex = ( ( int )( resources.GetObject( "lblHatchStyle.ImageIndex" ) ) );
			this.lblHatchStyle.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblHatchStyle.ImeMode" ) ) );
			this.lblHatchStyle.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblHatchStyle.Location" ) ) );
			this.lblHatchStyle.Name = "lblHatchStyle";
			this.lblHatchStyle.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblHatchStyle.RightToLeft" ) ) );
			this.lblHatchStyle.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblHatchStyle.Size" ) ) );
			this.lblHatchStyle.TabIndex = ( ( int )( resources.GetObject( "lblHatchStyle.TabIndex" ) ) );
			this.lblHatchStyle.Text = resources.GetString( "lblHatchStyle.Text" );
			this.lblHatchStyle.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblHatchStyle.TextAlign" ) ) );
			this.lblHatchStyle.Visible = ( ( bool )( resources.GetObject( "lblHatchStyle.Visible" ) ) );
			// 
			// comboHatchStyle
			// 
			this.comboHatchStyle.AccessibleDescription = resources.GetString( "comboHatchStyle.AccessibleDescription" );
			this.comboHatchStyle.AccessibleName = resources.GetString( "comboHatchStyle.AccessibleName" );
			this.comboHatchStyle.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboHatchStyle.Anchor" ) ) );
			this.comboHatchStyle.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboHatchStyle.BackgroundImage" ) ) );
			this.comboHatchStyle.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboHatchStyle.Dock" ) ) );
			this.comboHatchStyle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboHatchStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboHatchStyle.Enabled = ( ( bool )( resources.GetObject( "comboHatchStyle.Enabled" ) ) );
			this.comboHatchStyle.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboHatchStyle.Font" ) ) );
			this.comboHatchStyle.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboHatchStyle.ImeMode" ) ) );
			this.comboHatchStyle.IntegralHeight = ( ( bool )( resources.GetObject( "comboHatchStyle.IntegralHeight" ) ) );
			this.comboHatchStyle.ItemHeight = ( ( int )( resources.GetObject( "comboHatchStyle.ItemHeight" ) ) );
			this.comboHatchStyle.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboHatchStyle.Location" ) ) );
			this.comboHatchStyle.MaxDropDownItems = ( ( int )( resources.GetObject( "comboHatchStyle.MaxDropDownItems" ) ) );
			this.comboHatchStyle.MaxLength = ( ( int )( resources.GetObject( "comboHatchStyle.MaxLength" ) ) );
			this.comboHatchStyle.Name = "comboHatchStyle";
			this.comboHatchStyle.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboHatchStyle.RightToLeft" ) ) );
			this.comboHatchStyle.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboHatchStyle.Size" ) ) );
			this.comboHatchStyle.TabIndex = ( ( int )( resources.GetObject( "comboHatchStyle.TabIndex" ) ) );
			this.comboHatchStyle.Text = resources.GetString( "comboHatchStyle.Text" );
			this.comboHatchStyle.Visible = ( ( bool )( resources.GetObject( "comboHatchStyle.Visible" ) ) );
			this.comboHatchStyle.SelectedIndexChanged += new System.EventHandler( this.Format_Changed );
			this.comboHatchStyle.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboHatchStyle_DrawItem );
			// 
			// lblUnderlineStyle
			// 
			this.lblUnderlineStyle.AccessibleDescription = resources.GetString( "lblUnderlineStyle.AccessibleDescription" );
			this.lblUnderlineStyle.AccessibleName = resources.GetString( "lblUnderlineStyle.AccessibleName" );
			this.lblUnderlineStyle.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblUnderlineStyle.Anchor" ) ) );
			this.lblUnderlineStyle.AutoSize = ( ( bool )( resources.GetObject( "lblUnderlineStyle.AutoSize" ) ) );
			this.lblUnderlineStyle.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblUnderlineStyle.Dock" ) ) );
			this.lblUnderlineStyle.Enabled = ( ( bool )( resources.GetObject( "lblUnderlineStyle.Enabled" ) ) );
			this.lblUnderlineStyle.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblUnderlineStyle.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblUnderlineStyle.Font" ) ) );
			this.lblUnderlineStyle.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblUnderlineStyle.Image" ) ) );
			this.lblUnderlineStyle.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblUnderlineStyle.ImageAlign" ) ) );
			this.lblUnderlineStyle.ImageIndex = ( ( int )( resources.GetObject( "lblUnderlineStyle.ImageIndex" ) ) );
			this.lblUnderlineStyle.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblUnderlineStyle.ImeMode" ) ) );
			this.lblUnderlineStyle.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblUnderlineStyle.Location" ) ) );
			this.lblUnderlineStyle.Name = "lblUnderlineStyle";
			this.lblUnderlineStyle.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblUnderlineStyle.RightToLeft" ) ) );
			this.lblUnderlineStyle.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblUnderlineStyle.Size" ) ) );
			this.lblUnderlineStyle.TabIndex = ( ( int )( resources.GetObject( "lblUnderlineStyle.TabIndex" ) ) );
			this.lblUnderlineStyle.Text = resources.GetString( "lblUnderlineStyle.Text" );
			this.lblUnderlineStyle.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblUnderlineStyle.TextAlign" ) ) );
			this.lblUnderlineStyle.Visible = ( ( bool )( resources.GetObject( "lblUnderlineStyle.Visible" ) ) );
			// 
			// comboUnderlineStyle
			// 
			this.comboUnderlineStyle.AccessibleDescription = resources.GetString( "comboUnderlineStyle.AccessibleDescription" );
			this.comboUnderlineStyle.AccessibleName = resources.GetString( "comboUnderlineStyle.AccessibleName" );
			this.comboUnderlineStyle.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboUnderlineStyle.Anchor" ) ) );
			this.comboUnderlineStyle.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboUnderlineStyle.BackgroundImage" ) ) );
			this.comboUnderlineStyle.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboUnderlineStyle.Dock" ) ) );
			this.comboUnderlineStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUnderlineStyle.Enabled = ( ( bool )( resources.GetObject( "comboUnderlineStyle.Enabled" ) ) );
			this.comboUnderlineStyle.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboUnderlineStyle.Font" ) ) );
			this.comboUnderlineStyle.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboUnderlineStyle.ImeMode" ) ) );
			this.comboUnderlineStyle.IntegralHeight = ( ( bool )( resources.GetObject( "comboUnderlineStyle.IntegralHeight" ) ) );
			this.comboUnderlineStyle.ItemHeight = ( ( int )( resources.GetObject( "comboUnderlineStyle.ItemHeight" ) ) );
			this.comboUnderlineStyle.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboUnderlineStyle.Location" ) ) );
			this.comboUnderlineStyle.MaxDropDownItems = ( ( int )( resources.GetObject( "comboUnderlineStyle.MaxDropDownItems" ) ) );
			this.comboUnderlineStyle.MaxLength = ( ( int )( resources.GetObject( "comboUnderlineStyle.MaxLength" ) ) );
			this.comboUnderlineStyle.Name = "comboUnderlineStyle";
			this.comboUnderlineStyle.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboUnderlineStyle.RightToLeft" ) ) );
			this.comboUnderlineStyle.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboUnderlineStyle.Size" ) ) );
			this.comboUnderlineStyle.TabIndex = ( ( int )( resources.GetObject( "comboUnderlineStyle.TabIndex" ) ) );
			this.comboUnderlineStyle.Text = resources.GetString( "comboUnderlineStyle.Text" );
			this.comboUnderlineStyle.Visible = ( ( bool )( resources.GetObject( "comboUnderlineStyle.Visible" ) ) );
			this.comboUnderlineStyle.SelectedIndexChanged += new System.EventHandler( this.Format_Changed );
			// 
			// comboUnderlineWeight
			// 
			this.comboUnderlineWeight.AccessibleDescription = resources.GetString( "comboUnderlineWeight.AccessibleDescription" );
			this.comboUnderlineWeight.AccessibleName = resources.GetString( "comboUnderlineWeight.AccessibleName" );
			this.comboUnderlineWeight.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboUnderlineWeight.Anchor" ) ) );
			this.comboUnderlineWeight.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboUnderlineWeight.BackgroundImage" ) ) );
			this.comboUnderlineWeight.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboUnderlineWeight.Dock" ) ) );
			this.comboUnderlineWeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUnderlineWeight.Enabled = ( ( bool )( resources.GetObject( "comboUnderlineWeight.Enabled" ) ) );
			this.comboUnderlineWeight.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboUnderlineWeight.Font" ) ) );
			this.comboUnderlineWeight.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboUnderlineWeight.ImeMode" ) ) );
			this.comboUnderlineWeight.IntegralHeight = ( ( bool )( resources.GetObject( "comboUnderlineWeight.IntegralHeight" ) ) );
			this.comboUnderlineWeight.ItemHeight = ( ( int )( resources.GetObject( "comboUnderlineWeight.ItemHeight" ) ) );
			this.comboUnderlineWeight.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboUnderlineWeight.Location" ) ) );
			this.comboUnderlineWeight.MaxDropDownItems = ( ( int )( resources.GetObject( "comboUnderlineWeight.MaxDropDownItems" ) ) );
			this.comboUnderlineWeight.MaxLength = ( ( int )( resources.GetObject( "comboUnderlineWeight.MaxLength" ) ) );
			this.comboUnderlineWeight.Name = "comboUnderlineWeight";
			this.comboUnderlineWeight.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboUnderlineWeight.RightToLeft" ) ) );
			this.comboUnderlineWeight.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboUnderlineWeight.Size" ) ) );
			this.comboUnderlineWeight.TabIndex = ( ( int )( resources.GetObject( "comboUnderlineWeight.TabIndex" ) ) );
			this.comboUnderlineWeight.Text = resources.GetString( "comboUnderlineWeight.Text" );
			this.comboUnderlineWeight.Visible = ( ( bool )( resources.GetObject( "comboUnderlineWeight.Visible" ) ) );
			this.comboUnderlineWeight.SelectedIndexChanged += new System.EventHandler( this.Format_Changed );
			// 
			// lblUnderlineWeight
			// 
			this.lblUnderlineWeight.AccessibleDescription = resources.GetString( "lblUnderlineWeight.AccessibleDescription" );
			this.lblUnderlineWeight.AccessibleName = resources.GetString( "lblUnderlineWeight.AccessibleName" );
			this.lblUnderlineWeight.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblUnderlineWeight.Anchor" ) ) );
			this.lblUnderlineWeight.AutoSize = ( ( bool )( resources.GetObject( "lblUnderlineWeight.AutoSize" ) ) );
			this.lblUnderlineWeight.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblUnderlineWeight.Dock" ) ) );
			this.lblUnderlineWeight.Enabled = ( ( bool )( resources.GetObject( "lblUnderlineWeight.Enabled" ) ) );
			this.lblUnderlineWeight.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblUnderlineWeight.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblUnderlineWeight.Font" ) ) );
			this.lblUnderlineWeight.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblUnderlineWeight.Image" ) ) );
			this.lblUnderlineWeight.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblUnderlineWeight.ImageAlign" ) ) );
			this.lblUnderlineWeight.ImageIndex = ( ( int )( resources.GetObject( "lblUnderlineWeight.ImageIndex" ) ) );
			this.lblUnderlineWeight.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblUnderlineWeight.ImeMode" ) ) );
			this.lblUnderlineWeight.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblUnderlineWeight.Location" ) ) );
			this.lblUnderlineWeight.Name = "lblUnderlineWeight";
			this.lblUnderlineWeight.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblUnderlineWeight.RightToLeft" ) ) );
			this.lblUnderlineWeight.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblUnderlineWeight.Size" ) ) );
			this.lblUnderlineWeight.TabIndex = ( ( int )( resources.GetObject( "lblUnderlineWeight.TabIndex" ) ) );
			this.lblUnderlineWeight.Text = resources.GetString( "lblUnderlineWeight.Text" );
			this.lblUnderlineWeight.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblUnderlineWeight.TextAlign" ) ) );
			this.lblUnderlineWeight.Visible = ( ( bool )( resources.GetObject( "lblUnderlineWeight.Visible" ) ) );
			// 
			// btnRestore
			// 
			this.btnRestore.AccessibleDescription = resources.GetString( "btnRestore.AccessibleDescription" );
			this.btnRestore.AccessibleName = resources.GetString( "btnRestore.AccessibleName" );
			this.btnRestore.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnRestore.Anchor" ) ) );
			this.btnRestore.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnRestore.BackgroundImage" ) ) );
			this.btnRestore.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnRestore.Dock" ) ) );
			this.btnRestore.Enabled = ( ( bool )( resources.GetObject( "btnRestore.Enabled" ) ) );
			this.btnRestore.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnRestore.FlatStyle" ) ) );
			this.btnRestore.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnRestore.Font" ) ) );
			this.btnRestore.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnRestore.Image" ) ) );
			this.btnRestore.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnRestore.ImageAlign" ) ) );
			this.btnRestore.ImageIndex = ( ( int )( resources.GetObject( "btnRestore.ImageIndex" ) ) );
			this.btnRestore.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnRestore.ImeMode" ) ) );
			this.btnRestore.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnRestore.Location" ) ) );
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnRestore.RightToLeft" ) ) );
			this.btnRestore.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnRestore.Size" ) ) );
			this.btnRestore.TabIndex = ( ( int )( resources.GetObject( "btnRestore.TabIndex" ) ) );
			this.btnRestore.Text = resources.GetString( "btnRestore.Text" );
			this.btnRestore.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnRestore.TextAlign" ) ) );
			this.btnRestore.Visible = ( ( bool )( resources.GetObject( "btnRestore.Visible" ) ) );
			this.btnRestore.Click += new System.EventHandler( this.btnRestore_Click );
			// 
			// btnSetFonts
			// 
			this.btnSetFonts.AccessibleDescription = resources.GetString( "btnSetFonts.AccessibleDescription" );
			this.btnSetFonts.AccessibleName = resources.GetString( "btnSetFonts.AccessibleName" );
			this.btnSetFonts.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnSetFonts.Anchor" ) ) );
			this.btnSetFonts.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnSetFonts.BackgroundImage" ) ) );
			this.btnSetFonts.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnSetFonts.Dock" ) ) );
			this.btnSetFonts.Enabled = ( ( bool )( resources.GetObject( "btnSetFonts.Enabled" ) ) );
			this.btnSetFonts.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnSetFonts.FlatStyle" ) ) );
			this.btnSetFonts.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnSetFonts.Font" ) ) );
			this.btnSetFonts.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnSetFonts.Image" ) ) );
			this.btnSetFonts.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnSetFonts.ImageAlign" ) ) );
			this.btnSetFonts.ImageIndex = ( ( int )( resources.GetObject( "btnSetFonts.ImageIndex" ) ) );
			this.btnSetFonts.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnSetFonts.ImeMode" ) ) );
			this.btnSetFonts.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnSetFonts.Location" ) ) );
			this.btnSetFonts.Name = "btnSetFonts";
			this.btnSetFonts.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnSetFonts.RightToLeft" ) ) );
			this.btnSetFonts.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnSetFonts.Size" ) ) );
			this.btnSetFonts.TabIndex = ( ( int )( resources.GetObject( "btnSetFonts.TabIndex" ) ) );
			this.btnSetFonts.Text = resources.GetString( "btnSetFonts.Text" );
			this.btnSetFonts.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnSetFonts.TextAlign" ) ) );
			this.btnSetFonts.Visible = ( ( bool )( resources.GetObject( "btnSetFonts.Visible" ) ) );
			this.btnSetFonts.Click += new System.EventHandler( this.btnSetFonts_Click );
			// 
			// tabLexems
			// 
			this.tabLexems.AccessibleDescription = resources.GetString( "tabLexems.AccessibleDescription" );
			this.tabLexems.AccessibleName = resources.GetString( "tabLexems.AccessibleName" );
			this.tabLexems.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "tabLexems.Anchor" ) ) );
			this.tabLexems.AutoScroll = ( ( bool )( resources.GetObject( "tabLexems.AutoScroll" ) ) );
			this.tabLexems.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "tabLexems.AutoScrollMargin" ) ) );
			this.tabLexems.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "tabLexems.AutoScrollMinSize" ) ) );
			this.tabLexems.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "tabLexems.BackgroundImage" ) ) );
			this.tabLexems.Controls.Add( this.upDownPriority );
			this.tabLexems.Controls.Add( this.chkOnlyLocals );
			this.tabLexems.Controls.Add( this.comboFormat );
			this.tabLexems.Controls.Add( this.lblFormat );
			this.tabLexems.Controls.Add( this.chkBeginToken );
			this.tabLexems.Controls.Add( this.txtBeginToken );
			this.tabLexems.Controls.Add( this.lblBeginToken );
			this.tabLexems.Controls.Add( this.btnAddSubLexem );
			this.tabLexems.Controls.Add( this.lblLexems );
			this.tabLexems.Controls.Add( this.treeLexems );
			this.tabLexems.Controls.Add( this.btnRemoveLexem );
			this.tabLexems.Controls.Add( this.btnAddLexem );
			this.tabLexems.Controls.Add( this.txtContinueToken );
			this.tabLexems.Controls.Add( this.chkContinueToken );
			this.tabLexems.Controls.Add( this.lblContinueToken );
			this.tabLexems.Controls.Add( this.lblEndToken );
			this.tabLexems.Controls.Add( this.txtEndToken );
			this.tabLexems.Controls.Add( this.chkEndToken );
			this.tabLexems.Controls.Add( this.lblPriority );
			this.tabLexems.Controls.Add( this.chkIsComplex );
			this.tabLexems.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "tabLexems.Dock" ) ) );
			this.tabLexems.Enabled = ( ( bool )( resources.GetObject( "tabLexems.Enabled" ) ) );
			this.tabLexems.Font = ( ( System.Drawing.Font )( resources.GetObject( "tabLexems.Font" ) ) );
			this.tabLexems.ImageIndex = ( ( int )( resources.GetObject( "tabLexems.ImageIndex" ) ) );
			this.tabLexems.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "tabLexems.ImeMode" ) ) );
			this.tabLexems.Location = ( ( System.Drawing.Point )( resources.GetObject( "tabLexems.Location" ) ) );
			this.tabLexems.Name = "tabLexems";
			this.tabLexems.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "tabLexems.RightToLeft" ) ) );
			this.tabLexems.Size = ( ( System.Drawing.Size )( resources.GetObject( "tabLexems.Size" ) ) );
			this.tabLexems.TabIndex = ( ( int )( resources.GetObject( "tabLexems.TabIndex" ) ) );
			this.tabLexems.Text = resources.GetString( "tabLexems.Text" );
			this.tabLexems.ToolTipText = resources.GetString( "tabLexems.ToolTipText" );
			this.tabLexems.Visible = ( ( bool )( resources.GetObject( "tabLexems.Visible" ) ) );
			// 
			// upDownPriority
			// 
			this.upDownPriority.AccessibleDescription = resources.GetString( "upDownPriority.AccessibleDescription" );
			this.upDownPriority.AccessibleName = resources.GetString( "upDownPriority.AccessibleName" );
			this.upDownPriority.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "upDownPriority.Anchor" ) ) );
			this.upDownPriority.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "upDownPriority.Dock" ) ) );
			this.upDownPriority.Enabled = ( ( bool )( resources.GetObject( "upDownPriority.Enabled" ) ) );
			this.upDownPriority.Font = ( ( System.Drawing.Font )( resources.GetObject( "upDownPriority.Font" ) ) );
			this.upDownPriority.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "upDownPriority.ImeMode" ) ) );
			this.upDownPriority.Location = ( ( System.Drawing.Point )( resources.GetObject( "upDownPriority.Location" ) ) );
			this.upDownPriority.Name = "upDownPriority";
			this.upDownPriority.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "upDownPriority.RightToLeft" ) ) );
			this.upDownPriority.Size = ( ( System.Drawing.Size )( resources.GetObject( "upDownPriority.Size" ) ) );
			this.upDownPriority.TabIndex = ( ( int )( resources.GetObject( "upDownPriority.TabIndex" ) ) );
			this.upDownPriority.TextAlign = ( ( System.Windows.Forms.HorizontalAlignment )( resources.GetObject( "upDownPriority.TextAlign" ) ) );
			this.upDownPriority.ThousandsSeparator = ( ( bool )( resources.GetObject( "upDownPriority.ThousandsSeparator" ) ) );
			this.upDownPriority.UpDownAlign = ( ( System.Windows.Forms.LeftRightAlignment )( resources.GetObject( "upDownPriority.UpDownAlign" ) ) );
			this.upDownPriority.Visible = ( ( bool )( resources.GetObject( "upDownPriority.Visible" ) ) );
			// 
			// chkOnlyLocals
			// 
			this.chkOnlyLocals.AccessibleDescription = resources.GetString( "chkOnlyLocals.AccessibleDescription" );
			this.chkOnlyLocals.AccessibleName = resources.GetString( "chkOnlyLocals.AccessibleName" );
			this.chkOnlyLocals.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "chkOnlyLocals.Anchor" ) ) );
			this.chkOnlyLocals.Appearance = ( ( System.Windows.Forms.Appearance )( resources.GetObject( "chkOnlyLocals.Appearance" ) ) );
			this.chkOnlyLocals.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "chkOnlyLocals.BackgroundImage" ) ) );
			this.chkOnlyLocals.CheckAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkOnlyLocals.CheckAlign" ) ) );
			this.chkOnlyLocals.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "chkOnlyLocals.Dock" ) ) );
			this.chkOnlyLocals.Enabled = ( ( bool )( resources.GetObject( "chkOnlyLocals.Enabled" ) ) );
			this.chkOnlyLocals.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "chkOnlyLocals.FlatStyle" ) ) );
			this.chkOnlyLocals.Font = ( ( System.Drawing.Font )( resources.GetObject( "chkOnlyLocals.Font" ) ) );
			this.chkOnlyLocals.Image = ( ( System.Drawing.Image )( resources.GetObject( "chkOnlyLocals.Image" ) ) );
			this.chkOnlyLocals.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkOnlyLocals.ImageAlign" ) ) );
			this.chkOnlyLocals.ImageIndex = ( ( int )( resources.GetObject( "chkOnlyLocals.ImageIndex" ) ) );
			this.chkOnlyLocals.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "chkOnlyLocals.ImeMode" ) ) );
			this.chkOnlyLocals.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkOnlyLocals.Location" ) ) );
			this.chkOnlyLocals.Name = "chkOnlyLocals";
			this.chkOnlyLocals.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "chkOnlyLocals.RightToLeft" ) ) );
			this.chkOnlyLocals.Size = ( ( System.Drawing.Size )( resources.GetObject( "chkOnlyLocals.Size" ) ) );
			this.chkOnlyLocals.TabIndex = ( ( int )( resources.GetObject( "chkOnlyLocals.TabIndex" ) ) );
			this.chkOnlyLocals.Text = resources.GetString( "chkOnlyLocals.Text" );
			this.chkOnlyLocals.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkOnlyLocals.TextAlign" ) ) );
			this.chkOnlyLocals.Visible = ( ( bool )( resources.GetObject( "chkOnlyLocals.Visible" ) ) );
			// 
			// comboFormat
			// 
			this.comboFormat.AccessibleDescription = resources.GetString( "comboFormat.AccessibleDescription" );
			this.comboFormat.AccessibleName = resources.GetString( "comboFormat.AccessibleName" );
			this.comboFormat.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboFormat.Anchor" ) ) );
			this.comboFormat.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboFormat.BackgroundImage" ) ) );
			this.comboFormat.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboFormat.Dock" ) ) );
			this.comboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboFormat.Enabled = ( ( bool )( resources.GetObject( "comboFormat.Enabled" ) ) );
			this.comboFormat.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboFormat.Font" ) ) );
			this.comboFormat.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboFormat.ImeMode" ) ) );
			this.comboFormat.IntegralHeight = ( ( bool )( resources.GetObject( "comboFormat.IntegralHeight" ) ) );
			this.comboFormat.ItemHeight = ( ( int )( resources.GetObject( "comboFormat.ItemHeight" ) ) );
			this.comboFormat.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboFormat.Location" ) ) );
			this.comboFormat.MaxDropDownItems = ( ( int )( resources.GetObject( "comboFormat.MaxDropDownItems" ) ) );
			this.comboFormat.MaxLength = ( ( int )( resources.GetObject( "comboFormat.MaxLength" ) ) );
			this.comboFormat.Name = "comboFormat";
			this.comboFormat.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboFormat.RightToLeft" ) ) );
			this.comboFormat.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboFormat.Size" ) ) );
			this.comboFormat.TabIndex = ( ( int )( resources.GetObject( "comboFormat.TabIndex" ) ) );
			this.comboFormat.Text = resources.GetString( "comboFormat.Text" );
			this.comboFormat.Visible = ( ( bool )( resources.GetObject( "comboFormat.Visible" ) ) );
			// 
			// lblFormat
			// 
			this.lblFormat.AccessibleDescription = resources.GetString( "lblFormat.AccessibleDescription" );
			this.lblFormat.AccessibleName = resources.GetString( "lblFormat.AccessibleName" );
			this.lblFormat.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblFormat.Anchor" ) ) );
			this.lblFormat.AutoSize = ( ( bool )( resources.GetObject( "lblFormat.AutoSize" ) ) );
			this.lblFormat.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblFormat.Dock" ) ) );
			this.lblFormat.Enabled = ( ( bool )( resources.GetObject( "lblFormat.Enabled" ) ) );
			this.lblFormat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblFormat.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblFormat.Font" ) ) );
			this.lblFormat.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblFormat.Image" ) ) );
			this.lblFormat.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblFormat.ImageAlign" ) ) );
			this.lblFormat.ImageIndex = ( ( int )( resources.GetObject( "lblFormat.ImageIndex" ) ) );
			this.lblFormat.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblFormat.ImeMode" ) ) );
			this.lblFormat.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblFormat.Location" ) ) );
			this.lblFormat.Name = "lblFormat";
			this.lblFormat.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblFormat.RightToLeft" ) ) );
			this.lblFormat.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblFormat.Size" ) ) );
			this.lblFormat.TabIndex = ( ( int )( resources.GetObject( "lblFormat.TabIndex" ) ) );
			this.lblFormat.Text = resources.GetString( "lblFormat.Text" );
			this.lblFormat.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblFormat.TextAlign" ) ) );
			this.lblFormat.Visible = ( ( bool )( resources.GetObject( "lblFormat.Visible" ) ) );
			// 
			// chkBeginToken
			// 
			this.chkBeginToken.AccessibleDescription = resources.GetString( "chkBeginToken.AccessibleDescription" );
			this.chkBeginToken.AccessibleName = resources.GetString( "chkBeginToken.AccessibleName" );
			this.chkBeginToken.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "chkBeginToken.Anchor" ) ) );
			this.chkBeginToken.Appearance = ( ( System.Windows.Forms.Appearance )( resources.GetObject( "chkBeginToken.Appearance" ) ) );
			this.chkBeginToken.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "chkBeginToken.BackgroundImage" ) ) );
			this.chkBeginToken.CheckAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkBeginToken.CheckAlign" ) ) );
			this.chkBeginToken.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "chkBeginToken.Dock" ) ) );
			this.chkBeginToken.Enabled = ( ( bool )( resources.GetObject( "chkBeginToken.Enabled" ) ) );
			this.chkBeginToken.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "chkBeginToken.FlatStyle" ) ) );
			this.chkBeginToken.Font = ( ( System.Drawing.Font )( resources.GetObject( "chkBeginToken.Font" ) ) );
			this.chkBeginToken.Image = ( ( System.Drawing.Image )( resources.GetObject( "chkBeginToken.Image" ) ) );
			this.chkBeginToken.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkBeginToken.ImageAlign" ) ) );
			this.chkBeginToken.ImageIndex = ( ( int )( resources.GetObject( "chkBeginToken.ImageIndex" ) ) );
			this.chkBeginToken.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "chkBeginToken.ImeMode" ) ) );
			this.chkBeginToken.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkBeginToken.Location" ) ) );
			this.chkBeginToken.Name = "chkBeginToken";
			this.chkBeginToken.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "chkBeginToken.RightToLeft" ) ) );
			this.chkBeginToken.Size = ( ( System.Drawing.Size )( resources.GetObject( "chkBeginToken.Size" ) ) );
			this.chkBeginToken.TabIndex = ( ( int )( resources.GetObject( "chkBeginToken.TabIndex" ) ) );
			this.chkBeginToken.Text = resources.GetString( "chkBeginToken.Text" );
			this.chkBeginToken.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkBeginToken.TextAlign" ) ) );
			this.chkBeginToken.Visible = ( ( bool )( resources.GetObject( "chkBeginToken.Visible" ) ) );
			// 
			// txtBeginToken
			// 
			this.txtBeginToken.AccessibleDescription = resources.GetString( "txtBeginToken.AccessibleDescription" );
			this.txtBeginToken.AccessibleName = resources.GetString( "txtBeginToken.AccessibleName" );
			this.txtBeginToken.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "txtBeginToken.Anchor" ) ) );
			this.txtBeginToken.AutoSize = ( ( bool )( resources.GetObject( "txtBeginToken.AutoSize" ) ) );
			this.txtBeginToken.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "txtBeginToken.BackgroundImage" ) ) );
			this.txtBeginToken.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "txtBeginToken.Dock" ) ) );
			this.txtBeginToken.Enabled = ( ( bool )( resources.GetObject( "txtBeginToken.Enabled" ) ) );
			this.txtBeginToken.Font = ( ( System.Drawing.Font )( resources.GetObject( "txtBeginToken.Font" ) ) );
			this.txtBeginToken.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "txtBeginToken.ImeMode" ) ) );
			this.txtBeginToken.Location = ( ( System.Drawing.Point )( resources.GetObject( "txtBeginToken.Location" ) ) );
			this.txtBeginToken.MaxLength = ( ( int )( resources.GetObject( "txtBeginToken.MaxLength" ) ) );
			this.txtBeginToken.Multiline = ( ( bool )( resources.GetObject( "txtBeginToken.Multiline" ) ) );
			this.txtBeginToken.Name = "txtBeginToken";
			this.txtBeginToken.PasswordChar = ( ( char )( resources.GetObject( "txtBeginToken.PasswordChar" ) ) );
			this.txtBeginToken.ReadOnly = true;
			this.txtBeginToken.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "txtBeginToken.RightToLeft" ) ) );
			this.txtBeginToken.ScrollBars = ( ( System.Windows.Forms.ScrollBars )( resources.GetObject( "txtBeginToken.ScrollBars" ) ) );
			this.txtBeginToken.Size = ( ( System.Drawing.Size )( resources.GetObject( "txtBeginToken.Size" ) ) );
			this.txtBeginToken.TabIndex = ( ( int )( resources.GetObject( "txtBeginToken.TabIndex" ) ) );
			this.txtBeginToken.Text = resources.GetString( "txtBeginToken.Text" );
			this.txtBeginToken.TextAlign = ( ( System.Windows.Forms.HorizontalAlignment )( resources.GetObject( "txtBeginToken.TextAlign" ) ) );
			this.txtBeginToken.Visible = ( ( bool )( resources.GetObject( "txtBeginToken.Visible" ) ) );
			this.txtBeginToken.WordWrap = ( ( bool )( resources.GetObject( "txtBeginToken.WordWrap" ) ) );
			// 
			// lblBeginToken
			// 
			this.lblBeginToken.AccessibleDescription = resources.GetString( "lblBeginToken.AccessibleDescription" );
			this.lblBeginToken.AccessibleName = resources.GetString( "lblBeginToken.AccessibleName" );
			this.lblBeginToken.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblBeginToken.Anchor" ) ) );
			this.lblBeginToken.AutoSize = ( ( bool )( resources.GetObject( "lblBeginToken.AutoSize" ) ) );
			this.lblBeginToken.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblBeginToken.Dock" ) ) );
			this.lblBeginToken.Enabled = ( ( bool )( resources.GetObject( "lblBeginToken.Enabled" ) ) );
			this.lblBeginToken.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblBeginToken.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblBeginToken.Font" ) ) );
			this.lblBeginToken.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblBeginToken.Image" ) ) );
			this.lblBeginToken.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblBeginToken.ImageAlign" ) ) );
			this.lblBeginToken.ImageIndex = ( ( int )( resources.GetObject( "lblBeginToken.ImageIndex" ) ) );
			this.lblBeginToken.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblBeginToken.ImeMode" ) ) );
			this.lblBeginToken.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblBeginToken.Location" ) ) );
			this.lblBeginToken.Name = "lblBeginToken";
			this.lblBeginToken.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblBeginToken.RightToLeft" ) ) );
			this.lblBeginToken.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblBeginToken.Size" ) ) );
			this.lblBeginToken.TabIndex = ( ( int )( resources.GetObject( "lblBeginToken.TabIndex" ) ) );
			this.lblBeginToken.Text = resources.GetString( "lblBeginToken.Text" );
			this.lblBeginToken.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblBeginToken.TextAlign" ) ) );
			this.lblBeginToken.Visible = ( ( bool )( resources.GetObject( "lblBeginToken.Visible" ) ) );
			// 
			// btnAddSubLexem
			// 
			this.btnAddSubLexem.AccessibleDescription = resources.GetString( "btnAddSubLexem.AccessibleDescription" );
			this.btnAddSubLexem.AccessibleName = resources.GetString( "btnAddSubLexem.AccessibleName" );
			this.btnAddSubLexem.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnAddSubLexem.Anchor" ) ) );
			this.btnAddSubLexem.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnAddSubLexem.BackgroundImage" ) ) );
			this.btnAddSubLexem.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnAddSubLexem.Dock" ) ) );
			this.btnAddSubLexem.Enabled = ( ( bool )( resources.GetObject( "btnAddSubLexem.Enabled" ) ) );
			this.btnAddSubLexem.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnAddSubLexem.FlatStyle" ) ) );
			this.btnAddSubLexem.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnAddSubLexem.Font" ) ) );
			this.btnAddSubLexem.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnAddSubLexem.Image" ) ) );
			this.btnAddSubLexem.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAddSubLexem.ImageAlign" ) ) );
			this.btnAddSubLexem.ImageIndex = ( ( int )( resources.GetObject( "btnAddSubLexem.ImageIndex" ) ) );
			this.btnAddSubLexem.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnAddSubLexem.ImeMode" ) ) );
			this.btnAddSubLexem.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnAddSubLexem.Location" ) ) );
			this.btnAddSubLexem.Name = "btnAddSubLexem";
			this.btnAddSubLexem.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnAddSubLexem.RightToLeft" ) ) );
			this.btnAddSubLexem.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnAddSubLexem.Size" ) ) );
			this.btnAddSubLexem.TabIndex = ( ( int )( resources.GetObject( "btnAddSubLexem.TabIndex" ) ) );
			this.btnAddSubLexem.Text = resources.GetString( "btnAddSubLexem.Text" );
			this.btnAddSubLexem.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAddSubLexem.TextAlign" ) ) );
			this.btnAddSubLexem.Visible = ( ( bool )( resources.GetObject( "btnAddSubLexem.Visible" ) ) );
			this.btnAddSubLexem.Click += new System.EventHandler( this.btnAddSubLexem_Click );
			// 
			// lblLexems
			// 
			this.lblLexems.AccessibleDescription = resources.GetString( "lblLexems.AccessibleDescription" );
			this.lblLexems.AccessibleName = resources.GetString( "lblLexems.AccessibleName" );
			this.lblLexems.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblLexems.Anchor" ) ) );
			this.lblLexems.AutoSize = ( ( bool )( resources.GetObject( "lblLexems.AutoSize" ) ) );
			this.lblLexems.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblLexems.Dock" ) ) );
			this.lblLexems.Enabled = ( ( bool )( resources.GetObject( "lblLexems.Enabled" ) ) );
			this.lblLexems.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblLexems.Font" ) ) );
			this.lblLexems.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblLexems.Image" ) ) );
			this.lblLexems.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblLexems.ImageAlign" ) ) );
			this.lblLexems.ImageIndex = ( ( int )( resources.GetObject( "lblLexems.ImageIndex" ) ) );
			this.lblLexems.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblLexems.ImeMode" ) ) );
			this.lblLexems.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblLexems.Location" ) ) );
			this.lblLexems.Name = "lblLexems";
			this.lblLexems.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblLexems.RightToLeft" ) ) );
			this.lblLexems.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblLexems.Size" ) ) );
			this.lblLexems.TabIndex = ( ( int )( resources.GetObject( "lblLexems.TabIndex" ) ) );
			this.lblLexems.Text = resources.GetString( "lblLexems.Text" );
			this.lblLexems.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblLexems.TextAlign" ) ) );
			this.lblLexems.Visible = ( ( bool )( resources.GetObject( "lblLexems.Visible" ) ) );
			// 
			// treeLexems
			// 
			this.treeLexems.AccessibleDescription = resources.GetString( "treeLexems.AccessibleDescription" );
			this.treeLexems.AccessibleName = resources.GetString( "treeLexems.AccessibleName" );
			this.treeLexems.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "treeLexems.Anchor" ) ) );
			this.treeLexems.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "treeLexems.BackgroundImage" ) ) );
			this.treeLexems.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "treeLexems.Dock" ) ) );
			this.treeLexems.Enabled = ( ( bool )( resources.GetObject( "treeLexems.Enabled" ) ) );
			this.treeLexems.Font = ( ( System.Drawing.Font )( resources.GetObject( "treeLexems.Font" ) ) );
			this.treeLexems.ImageIndex = ( ( int )( resources.GetObject( "treeLexems.ImageIndex" ) ) );
			this.treeLexems.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "treeLexems.ImeMode" ) ) );
			this.treeLexems.Indent = ( ( int )( resources.GetObject( "treeLexems.Indent" ) ) );
			this.treeLexems.ItemHeight = ( ( int )( resources.GetObject( "treeLexems.ItemHeight" ) ) );
			this.treeLexems.Location = ( ( System.Drawing.Point )( resources.GetObject( "treeLexems.Location" ) ) );
			this.treeLexems.Name = "treeLexems";
			this.treeLexems.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "treeLexems.RightToLeft" ) ) );
			this.treeLexems.SelectedImageIndex = ( ( int )( resources.GetObject( "treeLexems.SelectedImageIndex" ) ) );
			this.treeLexems.Size = ( ( System.Drawing.Size )( resources.GetObject( "treeLexems.Size" ) ) );
			this.treeLexems.TabIndex = ( ( int )( resources.GetObject( "treeLexems.TabIndex" ) ) );
			this.treeLexems.Text = resources.GetString( "treeLexems.Text" );
			this.treeLexems.Visible = ( ( bool )( resources.GetObject( "treeLexems.Visible" ) ) );
			this.treeLexems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.treeLexems_AfterSelect );
			// 
			// btnRemoveLexem
			// 
			this.btnRemoveLexem.AccessibleDescription = resources.GetString( "btnRemoveLexem.AccessibleDescription" );
			this.btnRemoveLexem.AccessibleName = resources.GetString( "btnRemoveLexem.AccessibleName" );
			this.btnRemoveLexem.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnRemoveLexem.Anchor" ) ) );
			this.btnRemoveLexem.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnRemoveLexem.BackgroundImage" ) ) );
			this.btnRemoveLexem.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnRemoveLexem.Dock" ) ) );
			this.btnRemoveLexem.Enabled = ( ( bool )( resources.GetObject( "btnRemoveLexem.Enabled" ) ) );
			this.btnRemoveLexem.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnRemoveLexem.FlatStyle" ) ) );
			this.btnRemoveLexem.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnRemoveLexem.Font" ) ) );
			this.btnRemoveLexem.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnRemoveLexem.Image" ) ) );
			this.btnRemoveLexem.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnRemoveLexem.ImageAlign" ) ) );
			this.btnRemoveLexem.ImageIndex = ( ( int )( resources.GetObject( "btnRemoveLexem.ImageIndex" ) ) );
			this.btnRemoveLexem.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnRemoveLexem.ImeMode" ) ) );
			this.btnRemoveLexem.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnRemoveLexem.Location" ) ) );
			this.btnRemoveLexem.Name = "btnRemoveLexem";
			this.btnRemoveLexem.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnRemoveLexem.RightToLeft" ) ) );
			this.btnRemoveLexem.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnRemoveLexem.Size" ) ) );
			this.btnRemoveLexem.TabIndex = ( ( int )( resources.GetObject( "btnRemoveLexem.TabIndex" ) ) );
			this.btnRemoveLexem.Text = resources.GetString( "btnRemoveLexem.Text" );
			this.btnRemoveLexem.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnRemoveLexem.TextAlign" ) ) );
			this.btnRemoveLexem.Visible = ( ( bool )( resources.GetObject( "btnRemoveLexem.Visible" ) ) );
			this.btnRemoveLexem.Click += new System.EventHandler( this.btnRemoveLexem_Click );
			// 
			// btnAddLexem
			// 
			this.btnAddLexem.AccessibleDescription = resources.GetString( "btnAddLexem.AccessibleDescription" );
			this.btnAddLexem.AccessibleName = resources.GetString( "btnAddLexem.AccessibleName" );
			this.btnAddLexem.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnAddLexem.Anchor" ) ) );
			this.btnAddLexem.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnAddLexem.BackgroundImage" ) ) );
			this.btnAddLexem.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnAddLexem.Dock" ) ) );
			this.btnAddLexem.Enabled = ( ( bool )( resources.GetObject( "btnAddLexem.Enabled" ) ) );
			this.btnAddLexem.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnAddLexem.FlatStyle" ) ) );
			this.btnAddLexem.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnAddLexem.Font" ) ) );
			this.btnAddLexem.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnAddLexem.Image" ) ) );
			this.btnAddLexem.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAddLexem.ImageAlign" ) ) );
			this.btnAddLexem.ImageIndex = ( ( int )( resources.GetObject( "btnAddLexem.ImageIndex" ) ) );
			this.btnAddLexem.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnAddLexem.ImeMode" ) ) );
			this.btnAddLexem.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnAddLexem.Location" ) ) );
			this.btnAddLexem.Name = "btnAddLexem";
			this.btnAddLexem.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnAddLexem.RightToLeft" ) ) );
			this.btnAddLexem.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnAddLexem.Size" ) ) );
			this.btnAddLexem.TabIndex = ( ( int )( resources.GetObject( "btnAddLexem.TabIndex" ) ) );
			this.btnAddLexem.Text = resources.GetString( "btnAddLexem.Text" );
			this.btnAddLexem.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAddLexem.TextAlign" ) ) );
			this.btnAddLexem.Visible = ( ( bool )( resources.GetObject( "btnAddLexem.Visible" ) ) );
			this.btnAddLexem.Click += new System.EventHandler( this.btnAddLexem_Click );
			// 
			// txtContinueToken
			// 
			this.txtContinueToken.AccessibleDescription = resources.GetString( "txtContinueToken.AccessibleDescription" );
			this.txtContinueToken.AccessibleName = resources.GetString( "txtContinueToken.AccessibleName" );
			this.txtContinueToken.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "txtContinueToken.Anchor" ) ) );
			this.txtContinueToken.AutoSize = ( ( bool )( resources.GetObject( "txtContinueToken.AutoSize" ) ) );
			this.txtContinueToken.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "txtContinueToken.BackgroundImage" ) ) );
			this.txtContinueToken.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "txtContinueToken.Dock" ) ) );
			this.txtContinueToken.Enabled = ( ( bool )( resources.GetObject( "txtContinueToken.Enabled" ) ) );
			this.txtContinueToken.Font = ( ( System.Drawing.Font )( resources.GetObject( "txtContinueToken.Font" ) ) );
			this.txtContinueToken.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "txtContinueToken.ImeMode" ) ) );
			this.txtContinueToken.Location = ( ( System.Drawing.Point )( resources.GetObject( "txtContinueToken.Location" ) ) );
			this.txtContinueToken.MaxLength = ( ( int )( resources.GetObject( "txtContinueToken.MaxLength" ) ) );
			this.txtContinueToken.Multiline = ( ( bool )( resources.GetObject( "txtContinueToken.Multiline" ) ) );
			this.txtContinueToken.Name = "txtContinueToken";
			this.txtContinueToken.PasswordChar = ( ( char )( resources.GetObject( "txtContinueToken.PasswordChar" ) ) );
			this.txtContinueToken.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "txtContinueToken.RightToLeft" ) ) );
			this.txtContinueToken.ScrollBars = ( ( System.Windows.Forms.ScrollBars )( resources.GetObject( "txtContinueToken.ScrollBars" ) ) );
			this.txtContinueToken.Size = ( ( System.Drawing.Size )( resources.GetObject( "txtContinueToken.Size" ) ) );
			this.txtContinueToken.TabIndex = ( ( int )( resources.GetObject( "txtContinueToken.TabIndex" ) ) );
			this.txtContinueToken.Text = resources.GetString( "txtContinueToken.Text" );
			this.txtContinueToken.TextAlign = ( ( System.Windows.Forms.HorizontalAlignment )( resources.GetObject( "txtContinueToken.TextAlign" ) ) );
			this.txtContinueToken.Visible = ( ( bool )( resources.GetObject( "txtContinueToken.Visible" ) ) );
			this.txtContinueToken.WordWrap = ( ( bool )( resources.GetObject( "txtContinueToken.WordWrap" ) ) );
			// 
			// chkContinueToken
			// 
			this.chkContinueToken.AccessibleDescription = resources.GetString( "chkContinueToken.AccessibleDescription" );
			this.chkContinueToken.AccessibleName = resources.GetString( "chkContinueToken.AccessibleName" );
			this.chkContinueToken.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "chkContinueToken.Anchor" ) ) );
			this.chkContinueToken.Appearance = ( ( System.Windows.Forms.Appearance )( resources.GetObject( "chkContinueToken.Appearance" ) ) );
			this.chkContinueToken.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "chkContinueToken.BackgroundImage" ) ) );
			this.chkContinueToken.CheckAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkContinueToken.CheckAlign" ) ) );
			this.chkContinueToken.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "chkContinueToken.Dock" ) ) );
			this.chkContinueToken.Enabled = ( ( bool )( resources.GetObject( "chkContinueToken.Enabled" ) ) );
			this.chkContinueToken.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "chkContinueToken.FlatStyle" ) ) );
			this.chkContinueToken.Font = ( ( System.Drawing.Font )( resources.GetObject( "chkContinueToken.Font" ) ) );
			this.chkContinueToken.Image = ( ( System.Drawing.Image )( resources.GetObject( "chkContinueToken.Image" ) ) );
			this.chkContinueToken.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkContinueToken.ImageAlign" ) ) );
			this.chkContinueToken.ImageIndex = ( ( int )( resources.GetObject( "chkContinueToken.ImageIndex" ) ) );
			this.chkContinueToken.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "chkContinueToken.ImeMode" ) ) );
			this.chkContinueToken.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkContinueToken.Location" ) ) );
			this.chkContinueToken.Name = "chkContinueToken";
			this.chkContinueToken.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "chkContinueToken.RightToLeft" ) ) );
			this.chkContinueToken.Size = ( ( System.Drawing.Size )( resources.GetObject( "chkContinueToken.Size" ) ) );
			this.chkContinueToken.TabIndex = ( ( int )( resources.GetObject( "chkContinueToken.TabIndex" ) ) );
			this.chkContinueToken.Text = resources.GetString( "chkContinueToken.Text" );
			this.chkContinueToken.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkContinueToken.TextAlign" ) ) );
			this.chkContinueToken.Visible = ( ( bool )( resources.GetObject( "chkContinueToken.Visible" ) ) );
			// 
			// lblContinueToken
			// 
			this.lblContinueToken.AccessibleDescription = resources.GetString( "lblContinueToken.AccessibleDescription" );
			this.lblContinueToken.AccessibleName = resources.GetString( "lblContinueToken.AccessibleName" );
			this.lblContinueToken.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblContinueToken.Anchor" ) ) );
			this.lblContinueToken.AutoSize = ( ( bool )( resources.GetObject( "lblContinueToken.AutoSize" ) ) );
			this.lblContinueToken.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblContinueToken.Dock" ) ) );
			this.lblContinueToken.Enabled = ( ( bool )( resources.GetObject( "lblContinueToken.Enabled" ) ) );
			this.lblContinueToken.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblContinueToken.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblContinueToken.Font" ) ) );
			this.lblContinueToken.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblContinueToken.Image" ) ) );
			this.lblContinueToken.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblContinueToken.ImageAlign" ) ) );
			this.lblContinueToken.ImageIndex = ( ( int )( resources.GetObject( "lblContinueToken.ImageIndex" ) ) );
			this.lblContinueToken.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblContinueToken.ImeMode" ) ) );
			this.lblContinueToken.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblContinueToken.Location" ) ) );
			this.lblContinueToken.Name = "lblContinueToken";
			this.lblContinueToken.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblContinueToken.RightToLeft" ) ) );
			this.lblContinueToken.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblContinueToken.Size" ) ) );
			this.lblContinueToken.TabIndex = ( ( int )( resources.GetObject( "lblContinueToken.TabIndex" ) ) );
			this.lblContinueToken.Text = resources.GetString( "lblContinueToken.Text" );
			this.lblContinueToken.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblContinueToken.TextAlign" ) ) );
			this.lblContinueToken.Visible = ( ( bool )( resources.GetObject( "lblContinueToken.Visible" ) ) );
			// 
			// lblEndToken
			// 
			this.lblEndToken.AccessibleDescription = resources.GetString( "lblEndToken.AccessibleDescription" );
			this.lblEndToken.AccessibleName = resources.GetString( "lblEndToken.AccessibleName" );
			this.lblEndToken.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblEndToken.Anchor" ) ) );
			this.lblEndToken.AutoSize = ( ( bool )( resources.GetObject( "lblEndToken.AutoSize" ) ) );
			this.lblEndToken.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblEndToken.Dock" ) ) );
			this.lblEndToken.Enabled = ( ( bool )( resources.GetObject( "lblEndToken.Enabled" ) ) );
			this.lblEndToken.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblEndToken.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblEndToken.Font" ) ) );
			this.lblEndToken.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblEndToken.Image" ) ) );
			this.lblEndToken.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblEndToken.ImageAlign" ) ) );
			this.lblEndToken.ImageIndex = ( ( int )( resources.GetObject( "lblEndToken.ImageIndex" ) ) );
			this.lblEndToken.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblEndToken.ImeMode" ) ) );
			this.lblEndToken.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblEndToken.Location" ) ) );
			this.lblEndToken.Name = "lblEndToken";
			this.lblEndToken.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblEndToken.RightToLeft" ) ) );
			this.lblEndToken.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblEndToken.Size" ) ) );
			this.lblEndToken.TabIndex = ( ( int )( resources.GetObject( "lblEndToken.TabIndex" ) ) );
			this.lblEndToken.Text = resources.GetString( "lblEndToken.Text" );
			this.lblEndToken.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblEndToken.TextAlign" ) ) );
			this.lblEndToken.Visible = ( ( bool )( resources.GetObject( "lblEndToken.Visible" ) ) );
			// 
			// txtEndToken
			// 
			this.txtEndToken.AccessibleDescription = resources.GetString( "txtEndToken.AccessibleDescription" );
			this.txtEndToken.AccessibleName = resources.GetString( "txtEndToken.AccessibleName" );
			this.txtEndToken.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "txtEndToken.Anchor" ) ) );
			this.txtEndToken.AutoSize = ( ( bool )( resources.GetObject( "txtEndToken.AutoSize" ) ) );
			this.txtEndToken.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "txtEndToken.BackgroundImage" ) ) );
			this.txtEndToken.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "txtEndToken.Dock" ) ) );
			this.txtEndToken.Enabled = ( ( bool )( resources.GetObject( "txtEndToken.Enabled" ) ) );
			this.txtEndToken.Font = ( ( System.Drawing.Font )( resources.GetObject( "txtEndToken.Font" ) ) );
			this.txtEndToken.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "txtEndToken.ImeMode" ) ) );
			this.txtEndToken.Location = ( ( System.Drawing.Point )( resources.GetObject( "txtEndToken.Location" ) ) );
			this.txtEndToken.MaxLength = ( ( int )( resources.GetObject( "txtEndToken.MaxLength" ) ) );
			this.txtEndToken.Multiline = ( ( bool )( resources.GetObject( "txtEndToken.Multiline" ) ) );
			this.txtEndToken.Name = "txtEndToken";
			this.txtEndToken.PasswordChar = ( ( char )( resources.GetObject( "txtEndToken.PasswordChar" ) ) );
			this.txtEndToken.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "txtEndToken.RightToLeft" ) ) );
			this.txtEndToken.ScrollBars = ( ( System.Windows.Forms.ScrollBars )( resources.GetObject( "txtEndToken.ScrollBars" ) ) );
			this.txtEndToken.Size = ( ( System.Drawing.Size )( resources.GetObject( "txtEndToken.Size" ) ) );
			this.txtEndToken.TabIndex = ( ( int )( resources.GetObject( "txtEndToken.TabIndex" ) ) );
			this.txtEndToken.Text = resources.GetString( "txtEndToken.Text" );
			this.txtEndToken.TextAlign = ( ( System.Windows.Forms.HorizontalAlignment )( resources.GetObject( "txtEndToken.TextAlign" ) ) );
			this.txtEndToken.Visible = ( ( bool )( resources.GetObject( "txtEndToken.Visible" ) ) );
			this.txtEndToken.WordWrap = ( ( bool )( resources.GetObject( "txtEndToken.WordWrap" ) ) );
			// 
			// chkEndToken
			// 
			this.chkEndToken.AccessibleDescription = resources.GetString( "chkEndToken.AccessibleDescription" );
			this.chkEndToken.AccessibleName = resources.GetString( "chkEndToken.AccessibleName" );
			this.chkEndToken.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "chkEndToken.Anchor" ) ) );
			this.chkEndToken.Appearance = ( ( System.Windows.Forms.Appearance )( resources.GetObject( "chkEndToken.Appearance" ) ) );
			this.chkEndToken.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "chkEndToken.BackgroundImage" ) ) );
			this.chkEndToken.CheckAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkEndToken.CheckAlign" ) ) );
			this.chkEndToken.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "chkEndToken.Dock" ) ) );
			this.chkEndToken.Enabled = ( ( bool )( resources.GetObject( "chkEndToken.Enabled" ) ) );
			this.chkEndToken.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "chkEndToken.FlatStyle" ) ) );
			this.chkEndToken.Font = ( ( System.Drawing.Font )( resources.GetObject( "chkEndToken.Font" ) ) );
			this.chkEndToken.Image = ( ( System.Drawing.Image )( resources.GetObject( "chkEndToken.Image" ) ) );
			this.chkEndToken.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkEndToken.ImageAlign" ) ) );
			this.chkEndToken.ImageIndex = ( ( int )( resources.GetObject( "chkEndToken.ImageIndex" ) ) );
			this.chkEndToken.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "chkEndToken.ImeMode" ) ) );
			this.chkEndToken.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkEndToken.Location" ) ) );
			this.chkEndToken.Name = "chkEndToken";
			this.chkEndToken.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "chkEndToken.RightToLeft" ) ) );
			this.chkEndToken.Size = ( ( System.Drawing.Size )( resources.GetObject( "chkEndToken.Size" ) ) );
			this.chkEndToken.TabIndex = ( ( int )( resources.GetObject( "chkEndToken.TabIndex" ) ) );
			this.chkEndToken.Text = resources.GetString( "chkEndToken.Text" );
			this.chkEndToken.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkEndToken.TextAlign" ) ) );
			this.chkEndToken.Visible = ( ( bool )( resources.GetObject( "chkEndToken.Visible" ) ) );
			// 
			// lblPriority
			// 
			this.lblPriority.AccessibleDescription = resources.GetString( "lblPriority.AccessibleDescription" );
			this.lblPriority.AccessibleName = resources.GetString( "lblPriority.AccessibleName" );
			this.lblPriority.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblPriority.Anchor" ) ) );
			this.lblPriority.AutoSize = ( ( bool )( resources.GetObject( "lblPriority.AutoSize" ) ) );
			this.lblPriority.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblPriority.Dock" ) ) );
			this.lblPriority.Enabled = ( ( bool )( resources.GetObject( "lblPriority.Enabled" ) ) );
			this.lblPriority.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblPriority.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblPriority.Font" ) ) );
			this.lblPriority.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblPriority.Image" ) ) );
			this.lblPriority.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblPriority.ImageAlign" ) ) );
			this.lblPriority.ImageIndex = ( ( int )( resources.GetObject( "lblPriority.ImageIndex" ) ) );
			this.lblPriority.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblPriority.ImeMode" ) ) );
			this.lblPriority.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblPriority.Location" ) ) );
			this.lblPriority.Name = "lblPriority";
			this.lblPriority.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblPriority.RightToLeft" ) ) );
			this.lblPriority.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblPriority.Size" ) ) );
			this.lblPriority.TabIndex = ( ( int )( resources.GetObject( "lblPriority.TabIndex" ) ) );
			this.lblPriority.Text = resources.GetString( "lblPriority.Text" );
			this.lblPriority.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblPriority.TextAlign" ) ) );
			this.lblPriority.Visible = ( ( bool )( resources.GetObject( "lblPriority.Visible" ) ) );
			// 
			// chkIsComplex
			// 
			this.chkIsComplex.AccessibleDescription = resources.GetString( "chkIsComplex.AccessibleDescription" );
			this.chkIsComplex.AccessibleName = resources.GetString( "chkIsComplex.AccessibleName" );
			this.chkIsComplex.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "chkIsComplex.Anchor" ) ) );
			this.chkIsComplex.Appearance = ( ( System.Windows.Forms.Appearance )( resources.GetObject( "chkIsComplex.Appearance" ) ) );
			this.chkIsComplex.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "chkIsComplex.BackgroundImage" ) ) );
			this.chkIsComplex.CheckAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkIsComplex.CheckAlign" ) ) );
			this.chkIsComplex.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "chkIsComplex.Dock" ) ) );
			this.chkIsComplex.Enabled = ( ( bool )( resources.GetObject( "chkIsComplex.Enabled" ) ) );
			this.chkIsComplex.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "chkIsComplex.FlatStyle" ) ) );
			this.chkIsComplex.Font = ( ( System.Drawing.Font )( resources.GetObject( "chkIsComplex.Font" ) ) );
			this.chkIsComplex.Image = ( ( System.Drawing.Image )( resources.GetObject( "chkIsComplex.Image" ) ) );
			this.chkIsComplex.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkIsComplex.ImageAlign" ) ) );
			this.chkIsComplex.ImageIndex = ( ( int )( resources.GetObject( "chkIsComplex.ImageIndex" ) ) );
			this.chkIsComplex.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "chkIsComplex.ImeMode" ) ) );
			this.chkIsComplex.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkIsComplex.Location" ) ) );
			this.chkIsComplex.Name = "chkIsComplex";
			this.chkIsComplex.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "chkIsComplex.RightToLeft" ) ) );
			this.chkIsComplex.Size = ( ( System.Drawing.Size )( resources.GetObject( "chkIsComplex.Size" ) ) );
			this.chkIsComplex.TabIndex = ( ( int )( resources.GetObject( "chkIsComplex.TabIndex" ) ) );
			this.chkIsComplex.Text = resources.GetString( "chkIsComplex.Text" );
			this.chkIsComplex.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkIsComplex.TextAlign" ) ) );
			this.chkIsComplex.Visible = ( ( bool )( resources.GetObject( "chkIsComplex.Visible" ) ) );
			// 
			// lblLanguages
			// 
			this.lblLanguages.AccessibleDescription = resources.GetString( "lblLanguages.AccessibleDescription" );
			this.lblLanguages.AccessibleName = resources.GetString( "lblLanguages.AccessibleName" );
			this.lblLanguages.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblLanguages.Anchor" ) ) );
			this.lblLanguages.AutoSize = ( ( bool )( resources.GetObject( "lblLanguages.AutoSize" ) ) );
			this.lblLanguages.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblLanguages.Dock" ) ) );
			this.lblLanguages.Enabled = ( ( bool )( resources.GetObject( "lblLanguages.Enabled" ) ) );
			this.lblLanguages.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblLanguages.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblLanguages.Font" ) ) );
			this.lblLanguages.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblLanguages.Image" ) ) );
			this.lblLanguages.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblLanguages.ImageAlign" ) ) );
			this.lblLanguages.ImageIndex = ( ( int )( resources.GetObject( "lblLanguages.ImageIndex" ) ) );
			this.lblLanguages.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblLanguages.ImeMode" ) ) );
			this.lblLanguages.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblLanguages.Location" ) ) );
			this.lblLanguages.Name = "lblLanguages";
			this.lblLanguages.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblLanguages.RightToLeft" ) ) );
			this.lblLanguages.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblLanguages.Size" ) ) );
			this.lblLanguages.TabIndex = ( ( int )( resources.GetObject( "lblLanguages.TabIndex" ) ) );
			this.lblLanguages.Text = resources.GetString( "lblLanguages.Text" );
			this.lblLanguages.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblLanguages.TextAlign" ) ) );
			this.lblLanguages.Visible = ( ( bool )( resources.GetObject( "lblLanguages.Visible" ) ) );
			// 
			// comboLanguages
			// 
			this.comboLanguages.AccessibleDescription = resources.GetString( "comboLanguages.AccessibleDescription" );
			this.comboLanguages.AccessibleName = resources.GetString( "comboLanguages.AccessibleName" );
			this.comboLanguages.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboLanguages.Anchor" ) ) );
			this.comboLanguages.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboLanguages.BackgroundImage" ) ) );
			this.comboLanguages.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboLanguages.Dock" ) ) );
			this.comboLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLanguages.Enabled = ( ( bool )( resources.GetObject( "comboLanguages.Enabled" ) ) );
			this.comboLanguages.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboLanguages.Font" ) ) );
			this.comboLanguages.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboLanguages.ImeMode" ) ) );
			this.comboLanguages.IntegralHeight = ( ( bool )( resources.GetObject( "comboLanguages.IntegralHeight" ) ) );
			this.comboLanguages.ItemHeight = ( ( int )( resources.GetObject( "comboLanguages.ItemHeight" ) ) );
			this.comboLanguages.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboLanguages.Location" ) ) );
			this.comboLanguages.MaxDropDownItems = ( ( int )( resources.GetObject( "comboLanguages.MaxDropDownItems" ) ) );
			this.comboLanguages.MaxLength = ( ( int )( resources.GetObject( "comboLanguages.MaxLength" ) ) );
			this.comboLanguages.Name = "comboLanguages";
			this.comboLanguages.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboLanguages.RightToLeft" ) ) );
			this.comboLanguages.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboLanguages.Size" ) ) );
			this.comboLanguages.TabIndex = ( ( int )( resources.GetObject( "comboLanguages.TabIndex" ) ) );
			this.comboLanguages.Text = resources.GetString( "comboLanguages.Text" );
			this.comboLanguages.Visible = ( ( bool )( resources.GetObject( "comboLanguages.Visible" ) ) );
			this.comboLanguages.SelectedIndexChanged += new System.EventHandler( this.comboLanguages_SelectedIndexChanged );
			// 
			// btnDelete
			// 
			this.btnDelete.AccessibleDescription = resources.GetString( "btnDelete.AccessibleDescription" );
			this.btnDelete.AccessibleName = resources.GetString( "btnDelete.AccessibleName" );
			this.btnDelete.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnDelete.Anchor" ) ) );
			this.btnDelete.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnDelete.BackgroundImage" ) ) );
			this.btnDelete.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnDelete.Dock" ) ) );
			this.btnDelete.Enabled = ( ( bool )( resources.GetObject( "btnDelete.Enabled" ) ) );
			this.btnDelete.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnDelete.FlatStyle" ) ) );
			this.btnDelete.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnDelete.Font" ) ) );
			this.btnDelete.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnDelete.Image" ) ) );
			this.btnDelete.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnDelete.ImageAlign" ) ) );
			this.btnDelete.ImageIndex = ( ( int )( resources.GetObject( "btnDelete.ImageIndex" ) ) );
			this.btnDelete.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnDelete.ImeMode" ) ) );
			this.btnDelete.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnDelete.Location" ) ) );
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnDelete.RightToLeft" ) ) );
			this.btnDelete.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnDelete.Size" ) ) );
			this.btnDelete.TabIndex = ( ( int )( resources.GetObject( "btnDelete.TabIndex" ) ) );
			this.btnDelete.Text = resources.GetString( "btnDelete.Text" );
			this.btnDelete.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnDelete.TextAlign" ) ) );
			this.btnDelete.Visible = ( ( bool )( resources.GetObject( "btnDelete.Visible" ) ) );
			this.btnDelete.Click += new System.EventHandler( this.btnDelete_Click );
			// 
			// saveDlg
			// 
			this.saveDlg.DefaultExt = "xml";
			this.saveDlg.Filter = resources.GetString( "saveDlg.Filter" );
			this.saveDlg.Title = resources.GetString( "saveDlg.Title" );
			// 
			// btnOpen
			// 
			this.btnOpen.AccessibleDescription = resources.GetString( "btnOpen.AccessibleDescription" );
			this.btnOpen.AccessibleName = resources.GetString( "btnOpen.AccessibleName" );
			this.btnOpen.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnOpen.Anchor" ) ) );
			this.btnOpen.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnOpen.BackgroundImage" ) ) );
			this.btnOpen.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnOpen.Dock" ) ) );
			this.btnOpen.Enabled = ( ( bool )( resources.GetObject( "btnOpen.Enabled" ) ) );
			this.btnOpen.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnOpen.FlatStyle" ) ) );
			this.btnOpen.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnOpen.Font" ) ) );
			this.btnOpen.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnOpen.Image" ) ) );
			this.btnOpen.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnOpen.ImageAlign" ) ) );
			this.btnOpen.ImageIndex = ( ( int )( resources.GetObject( "btnOpen.ImageIndex" ) ) );
			this.btnOpen.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnOpen.ImeMode" ) ) );
			this.btnOpen.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnOpen.Location" ) ) );
			this.btnOpen.Name = "btnOpen";
			this.btnOpen.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnOpen.RightToLeft" ) ) );
			this.btnOpen.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnOpen.Size" ) ) );
			this.btnOpen.TabIndex = ( ( int )( resources.GetObject( "btnOpen.TabIndex" ) ) );
			this.btnOpen.Text = resources.GetString( "btnOpen.Text" );
			this.btnOpen.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnOpen.TextAlign" ) ) );
			this.btnOpen.Visible = ( ( bool )( resources.GetObject( "btnOpen.Visible" ) ) );
			this.btnOpen.Click += new System.EventHandler( this.btnOpen_Click );
			// 
			// btnSaveAs
			// 
			this.btnSaveAs.AccessibleDescription = resources.GetString( "btnSaveAs.AccessibleDescription" );
			this.btnSaveAs.AccessibleName = resources.GetString( "btnSaveAs.AccessibleName" );
			this.btnSaveAs.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnSaveAs.Anchor" ) ) );
			this.btnSaveAs.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnSaveAs.BackgroundImage" ) ) );
			this.btnSaveAs.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnSaveAs.Dock" ) ) );
			this.btnSaveAs.Enabled = ( ( bool )( resources.GetObject( "btnSaveAs.Enabled" ) ) );
			this.btnSaveAs.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnSaveAs.FlatStyle" ) ) );
			this.btnSaveAs.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnSaveAs.Font" ) ) );
			this.btnSaveAs.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnSaveAs.Image" ) ) );
			this.btnSaveAs.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnSaveAs.ImageAlign" ) ) );
			this.btnSaveAs.ImageIndex = ( ( int )( resources.GetObject( "btnSaveAs.ImageIndex" ) ) );
			this.btnSaveAs.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnSaveAs.ImeMode" ) ) );
			this.btnSaveAs.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnSaveAs.Location" ) ) );
			this.btnSaveAs.Name = "btnSaveAs";
			this.btnSaveAs.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnSaveAs.RightToLeft" ) ) );
			this.btnSaveAs.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnSaveAs.Size" ) ) );
			this.btnSaveAs.TabIndex = ( ( int )( resources.GetObject( "btnSaveAs.TabIndex" ) ) );
			this.btnSaveAs.Text = resources.GetString( "btnSaveAs.Text" );
			this.btnSaveAs.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnSaveAs.TextAlign" ) ) );
			this.btnSaveAs.Visible = ( ( bool )( resources.GetObject( "btnSaveAs.Visible" ) ) );
			this.btnSaveAs.Click += new System.EventHandler( this.btnSaveAs_Click_1 );
			// 
			// dlgOpen
			// 
			this.dlgOpen.DefaultExt = "xml";
			this.dlgOpen.Filter = resources.GetString( "dlgOpen.Filter" );
			this.dlgOpen.Title = resources.GetString( "dlgOpen.Title" );
			// 
			// ConfigurationDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AccessibleDescription = resources.GetString( "$this.AccessibleDescription" );
			this.AccessibleName = resources.GetString( "$this.AccessibleName" );
			this.AutoScaleBaseSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScaleBaseSize" ) ) );
			this.AutoScroll = ( ( bool )( resources.GetObject( "$this.AutoScroll" ) ) );
			this.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMargin" ) ) );
			this.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMinSize" ) ) );
			this.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "$this.BackgroundImage" ) ) );
			this.CancelButton = this.btnCancel;
			this.ClientSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.ClientSize" ) ) );
			this.Controls.Add( this.btnSaveAs );
			this.Controls.Add( this.btnOpen );
			this.Controls.Add( this.btnDelete );
			this.Controls.Add( this.comboLanguages );
			this.Controls.Add( this.lblLanguages );
			this.Controls.Add( this.tabsConfiguration );
			this.Controls.Add( this.btnOK );
			this.Controls.Add( this.btnCancel );
			this.Controls.Add( this.btnHelp );
			this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
			this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "$this.Icon" ) ) );
			this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
			this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
			this.MaximizeBox = false;
			this.MaximumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MaximumSize" ) ) );
			this.MinimizeBox = false;
			this.MinimumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MinimumSize" ) ) );
			this.Name = "ConfigurationDialog";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.RightToLeftLayout = true;
			this.StartPosition = ( ( System.Windows.Forms.FormStartPosition )( resources.GetObject( "$this.StartPosition" ) ) );
			this.Text = resources.GetString( "$this.Text" );
			this.tabsConfiguration.ResumeLayout( false );
			this.tabFileExtensions.ResumeLayout( false );
			this.tabFormats.ResumeLayout( false );
			this.tabLexems.ResumeLayout( false );
			( ( System.ComponentModel.ISupportInitialize )( this.upDownPriority ) ).EndInit();
			this.ResumeLayout( false );

		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Tab page changed. Load all settings into controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabsConfiguration_SelectedIndexChanged( object sender, EventArgs e )
		{
			TabPage page = tabsConfiguration.TabPages[ tabsConfiguration.SelectedIndex ];

			if( page == null || page.Tag != null )
			{
				return;
			}

			InitTabPage( page );
		}
		/// <summary>
		/// Raises when item was selected from list of languages.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboLanguages_SelectedIndexChanged( object sender, EventArgs e )
		{
			SavePreviousLanguage( m_activeIndex );

			IConfigLanguage activeLang = GetActiveLanguage();

			// Some language was selected.
			if( activeLang != null )
			{
				LoadLanguage( activeLang );
			}
			else if( this.Visible ) // Create new configuration.
			{
				CreateNewConfiguration();
			}

			m_activeIndex = comboLanguages.SelectedIndex;

			RestoreLexemControls();

			ToggleControls();
			ToggleFormatControls();
			ToggleLexemControls();
		}
		/// <summary>
		/// Raises whent SaveAs button pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSaveAs_Click( object sender, EventArgs e )
		{
			Save();
		}
		/// <summary>
		/// Saves all changes on language configurations.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnOK_Click( object sender, EventArgs e )
		{
			SaveAll();

			if( m_strLoadedConfigurationFileName != string.Empty && m_strLoadedConfigurationFileName != null )
			{
				m_config.Save( m_strLoadedConfigurationFileName );
			}
		}
		/// <summary>
		/// Removes language from collection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDelete_Click( object sender, System.EventArgs e )
		{
			int currentpos = comboLanguages.SelectedIndex;

			IConfigLanguage lang = GetActiveLanguage();

			if( !( lang == null || lang == m_config.DefaultLanguage ) )
			{
				m_config.Remove( lang );

				m_newLanguages.Remove( lang );
				comboLanguages.Items.Remove( lang.Language );
				comboLanguages.SelectedIndex = currentpos;
			}
		}
		/// <summary>
		/// Add button for extension added.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstExtensions_OnAddClick( object sender, System.EventArgs e )
		{
			StringBuilder builder = new StringBuilder();
			IConfigLanguage lang = null;

			// get  extensions from all languages.
			for( int i = 0, leni = m_newLanguages.Count; i < leni; i++ )
			{
				lang = m_newLanguages[ i ] as IConfigLanguage;

				for( int j = 0, lenj = lang.Extensions.Count; j < lenj; j++ )
				{
					builder.Append( "^" + Regex.Escape( ( string )lang.Extensions[ j ] ) +
						"$|" );
				}
			}

			// get extension from current list.
			for( int i = 0, len = lstExtensions.ItemsList.Length; i < len; i++ )
			{
				builder.Append( "^" + Regex.Escape( lstExtensions.ItemsList[ i ] ) + "$|" );
			}

			builder.Remove( builder.Length - 1, 1 );

			lstExtensions.Validator = new Regex( builder.ToString(),
				Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX );
		}
		/// <summary>
		/// Selected Format in list changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lsbFormats_SelectedIndexChanged( object sender, EventArgs e )
		{
			ToggleFormatControls();

			Format format = GetActiveFormat();

			if( format != null )
			{
				DrawFormatSample();
				LoadFormat( format );
			}
		}
		/// <summary>
		/// Paints sample on panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pnlSample_Paint( object sender, PaintEventArgs e )
		{
			Format format = GetActiveFormat();

			if( format == null ) return;

			float tHeight = format.Font.GetHeight();
			float top = ( ( float )pnlSample.Height - tHeight ) / 2.0f;

			float tWidth = format.MeasureText( e.Graphics, DEF_FORMAT_SAMPLE, true, false, 0 ).Width;
			float left = ( ( float )pnlSample.Width - tWidth ) / 2.0f;

			m_info.DrawRectangle = new Rectangle( ( int )left, ( int )top,
				( int )tWidth, ( int )tHeight );

			using( SolidBrush brush = new SolidBrush( format.BackColor ) )
			{
				e.Graphics.FillRectangle( brush, pnlSample.ClientRectangle );
				BorderInfo bi = new BorderInfo();
				format.DrawText( e.Graphics, ref m_info, ref bi, false, 0 );
			}

		}
		/// <summary>
		/// Opens Font Dialog for selecting font.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnFontFormat_Click( object sender, EventArgs e )
		{
			Format format = GetActiveFormat();

			if( format != null )
			{
				fontDlg.Font = format.Font;

				if( fontDlg.ShowDialog( this ) == DialogResult.OK )
				{
					format.Font = fontDlg.Font;
					DrawFormatSample();
				}
			}
		}
		/// <summary>
		/// Gets color by name.
		/// </summary>
		/// <param name="name">Localized name of the color.</param>
		/// <returns>Color.</returns>
		private Color GetColor( string name )
		{
			Color result = Color.Empty;

			if( name != DEF_EMPTY_COLOR )
				result = Color.FromName( Localizer.GetNativeEnumValueName( typeof( KnownColor ), name ) );

			return result;
		}
		/// <summary>
		/// Color for format in list changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Format_Changed( object sender, EventArgs e )
		{
			if( sender is ComboBox == false ) return;

			Format format = GetActiveFormat();
			if( format == null ) return;

			ComboBox colorList = sender as ComboBox;

			/* Set colors */
			if( colorList == comboFontColor )
			{
				format.FontColor = GetColor( comboFontColor.Text );
			}
			else if( colorList == comboForeColor )
			{
				format.ForeColor = GetColor( comboForeColor.Text );
			}
			else if( colorList == comboBackColor )
			{
				format.BackColor = GetColor( comboBackColor.Text );
			}
			else if( colorList == comboLineColor )
			{
				format.LineColor = GetColor( comboLineColor.Text );
			}

				/* Set style of format. */
			else if( colorList == comboHatchStyle )
			{
				string strText = comboHatchStyle.Text;

				if( DEF_NONE_HATCH == strText )
				{
					format.UseHatchFill = false;
				}
				else
				{
					format.UseHatchFill = true;
					format.HatchStyle = ( HatchStyle )Localizer.GetEnumValue( typeof( HatchStyle ),
						comboHatchStyle.Text );
				}
			}
			else if( colorList == comboUnderlineStyle )
			{
				format.UnderlineStyle = ( UnderlineStyle )Localizer.GetEnumValue( typeof( UnderlineStyle ),
					comboUnderlineStyle.Text );
			}
			else if( colorList == comboUnderlineWeight )
			{
				format.UnderlineWeight = ( UnderlineWeight )Localizer.GetEnumValue( typeof( UnderlineWeight ),
					comboUnderlineWeight.Text );
			}

			DrawFormatSample();
		}
		/// <summary>
		/// Restores changed format.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRestore_Click( object sender, EventArgs e )
		{
			Format format = GetActiveFormat();

			if( format == null ) return;

			if( m_formatState.ContainsKey( format.Name ) )
			{
				FormatState state = ( FormatState )m_formatState[ format.Name ];

				state.RestoreState( format );
				LoadFormat( format );
			}

		}
		/// <summary>
		/// Adds new format.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAddFormat_Click( object sender, EventArgs e )
		{
			// TODO: add new format.

			frmSimpleAdd addDlg = new frmSimpleAdd();
			addDlg.ReverseValidation = true;
			addDlg.Validator = GetFormatNameRegex();

			if( addDlg.ShowDialog( this ) == DialogResult.OK )
			{
				AddFormat( addDlg.Value );
			}
		}
		/// <summary>
		/// Removes format from the list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRemoveFormat_Click( object sender, EventArgs e )
		{
			Format format = GetActiveFormat();

			if( format == null ) return;

			// format is system, don't remove it.
			if( m_formatType.Contains( format.Name ) ) return;

			ConfigLanguage lang = GetActiveLanguage() as ConfigLanguage;

			if( lang == null ) return;

			int index = lsbFormats.Items.IndexOf( format.Name );

			lsbFormats.Items.Remove( format.Name );
			m_formatState.Remove( format.Name );
			lang.Remove( format );

			lsbFormats.SelectedIndex = index - 1;

			comboFormat.Items.Remove( format.Name );
		}
		/// <summary>
		/// Selected lexem has been changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeLexems_AfterSelect( object sender, TreeViewEventArgs e )
		{
			ConfigLexem lexem = GetActiveLexem();

			if( lexem != null )
			{
				LoadLexem( lexem );
			}
			else
			{
				RestoreLexemControls();
			}

			ToggleLexemControls();

			m_activeLexem = treeLexems.SelectedNode.Tag as ConfigLexem;
		}
		/// <summary>
		/// Add sub lexem to collection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAddSubLexem_Click( object sender, EventArgs e )
		{
			ConfigLexem activeLexem = GetActiveLexem();

			if( activeLexem == null ) return;

			using( frmSimpleAdd addDlg = new frmSimpleAdd() )
			{
				if( addDlg.ShowDialog( this ) == DialogResult.OK )
				{
					AddLexem( activeLexem, addDlg.Value, true );
				}
			}
		}
		/// <summary>
		/// Adds new lexem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAddLexem_Click( object sender, EventArgs e )
		{
			ConfigLexem activeLexem = GetActiveLexem();

			if( activeLexem == null )
			{
				ConfigLanguage lang = GetActiveLanguage() as ConfigLanguage;

				if( lang == null || lang.Lexems.Count == 0 ) return;

				activeLexem = lang.Lexems[ 0 ] as ConfigLexem;
			}

			using( frmSimpleAdd addDlg = new frmSimpleAdd() )
			{
				if( addDlg.ShowDialog( this ) == DialogResult.OK )
				{
					AddLexem( activeLexem.ParentConfig, addDlg.Value, false );
				}
			}

		}
		/// <summary>
		/// Removes selected lexem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRemoveLexem_Click( object sender, EventArgs e )
		{
			ConfigLexem lexem = GetActiveLexem();

			if( lexem == null ) return;

			treeLexems.Nodes.Remove( treeLexems.SelectedNode );

			if( lexem.ParentConfig is ConfigLanguage )
			{
				( lexem.ParentConfig as ConfigLanguage ).Lexems.Remove( lexem );
			}
			else if( lexem.ParentConfig is ConfigLexem )
			{
				( lexem.ParentConfig as ConfigLexem ).SubLexems.Remove( lexem );
			}

			treeLexems.SelectedNode = treeLexems.Nodes[ 0 ];
		}
		/// <summary>
		/// Draws combo item for combobox with color rectangles.
		/// </summary>
		/// <param name="combo">ComboBox to draw.</param>
		/// <param name="currentBrush">Current brush to use.</param>
		/// <param name="e">DrawItemEventArgs.</param>
		protected void DrawComboItem( ComboBox combo, Brush currentBrush, DrawItemEventArgs e )
		{
			Graphics g = e.Graphics;
			string item = combo.Items[ e.Index ].ToString();

			e.DrawBackground();

			bool bSelected = ( ( e.State & DrawItemState.Selected ) == DrawItemState.Selected );

			Color txtColor = ( combo.Enabled ) ?
				( ( bSelected ) ? SystemColors.HighlightText : SystemColors.MenuText ) :
				SystemColors.GrayText;

			using( Brush brush = new SolidBrush( txtColor ) )
			{
				Rectangle rc = Rectangle.Inflate( e.Bounds, -3, -3 );

				Rectangle rcBox = new Rectangle( rc.Left + 2, rc.Top,
					PREVIEW_BOX_WIDTH, rc.Height );

				g.FillRectangle( Brushes.Black, rcBox );
				g.FillRectangle( Brushes.White, Rectangle.Inflate( rcBox, -1, -1 ) );
				g.FillRectangle( currentBrush, Rectangle.Inflate( rcBox, -2, -2 ) );

				Size textSize = g.MeasureString( item, e.Font ).ToSize();
				int top = e.Bounds.Top + ( e.Bounds.Height - textSize.Height ) / 2;

				// Clipping rectangle
				Rectangle clipRect = new Rectangle( e.Bounds.Left + 31, top,
					e.Bounds.Width - 31 - ARROW_WIDTH - 4, top + textSize.Height );

				using( StringFormat format = new StringFormat( StringFormatFlags.LineLimit ) )
				{
					format.Trimming = StringTrimming.EllipsisCharacter;
					format.FormatFlags |= StringFormatFlags.NoWrap;

					g.DrawString( item, e.Font, brush, clipRect, format );
				}

				if( bSelected ) e.DrawFocusRectangle();
			}
		}
		/// <summary>
		/// Draws combo item for combobox with font color.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboFontColor_DrawItem( object sender, DrawItemEventArgs e )
		{
			ComboBox combo = sender as ComboBox;
			if( combo == null ) return;
			if( e.Index < 0 ) return;

			string item = combo.Items[ e.Index ].ToString();
			Color clr = GetColor( item );

			using( Brush brush = new SolidBrush( clr ) )
			{
				DrawComboItem( combo, brush, e );
			}
		}
		/// <summary>
		/// Draws combo item for combobox with hatch style.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboHatchStyle_DrawItem( object sender, System.Windows.Forms.DrawItemEventArgs e )
		{
			ComboBox combo = sender as ComboBox;
			if( combo == null ) return;
			if( e.Index < 0 ) return;

			string item = combo.Items[ e.Index ].ToString();

			if( DEF_NONE_HATCH == item ) return;

			HatchStyle style = ( HatchStyle )Localizer.GetEnumValue( typeof( HatchStyle ), item );

			using( Brush brush = new HatchBrush( style, Color.Black, Color.White ) )
			{
				DrawComboItem( combo, brush, e );
			}
		}
		/// <summary>
		/// Sets new font and draws sample.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSetFonts_Click( object sender, System.EventArgs e )
		{
			ConfigLanguage conf = ( ConfigLanguage )GetActiveLanguage();
			Format formatCurr = this.GetActiveFormat();

			for( int i = 0, len = conf.Count; i < len; i++ )
			{
				Format format = conf[ i ] as Format;

				if( format != formatCurr )
				{
					format.Font = formatCurr.Font.Clone() as Font;
				}
			}

			DrawFormatSample();
		}
		/// <summary>
		/// Shows file open dialog where user can load some configfile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnOpen_Click( object sender, System.EventArgs e )
		{
			if( dlgOpen.ShowDialog( this ) == DialogResult.OK )
			{
				m_config.Open( dlgOpen.FileName );

				m_newLanguages.Clear();
				m_newLanguages.AddRange( m_config.KnownLanguages );

				InfillLanguageList();
				comboLanguages_SelectedIndexChanged( this, EventArgs.Empty );

				m_strLoadedConfigurationFileName = dlgOpen.FileName;
			}

		}
		/// <summary>
		/// Shows file save dialog where user can give the name of the file, configuration should be saved to.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSaveAs_Click_1( object sender, System.EventArgs e )
		{
			if( saveDlg.ShowDialog( this ) == DialogResult.OK )
			{
				SavePreviousLexem();
				SaveAll();

				m_config.Save( saveDlg.FileName );

				m_strLoadedConfigurationFileName = saveDlg.FileName;
			}
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Infills list of loaded languages. 
		/// </summary>
		private void InfillLanguageList()
		{
			comboLanguages.BeginUpdate();
			comboLanguages.Items.Clear();

			comboLanguages.Items.AddRange( m_config.KnownLanguageNames.ToArray() );

			comboLanguages.Items.Add( DEF_NEW_LANGUAGE );

			if( comboLanguages.Items.Count > 1 )
			{
				if( m_activeLang != null )
				{
					comboLanguages.SelectedIndex = comboLanguages.Items.IndexOf( m_activeLang.Language );
				}
				else
				{
					comboLanguages.SelectedIndex = 0;
				}
			}

			if( comboLanguages.SelectedIndex < 0 && comboLanguages.Items.Count > 0 )
				comboLanguages.SelectedIndex = 0;

			comboLanguages.EndUpdate();

			tabFileExtensions.Tag = string.Empty;
		}
		/// <summary>
		/// Returns active language.
		/// </summary>
		/// <returns></returns>
		private IConfigLanguage GetActiveLanguage()
		{
			return GetLanguage( comboLanguages.SelectedIndex );
		}
		/// <summary>
		/// Returns language by defined index.
		/// </summary>
		/// <returns></returns>
		private IConfigLanguage GetLanguage( int index )
		{
			if( index < 0 || index >= m_newLanguages.Count )
				return null;

			return m_newLanguages[ index ] as IConfigLanguage;
		}
		/// <summary>
		/// Loads all properties of selected language to controls.
		/// </summary>
		/// <param name="lang">Configuration language to load.</param>
		private void LoadLanguage( IConfigLanguage lang )
		{
			if( lang == null )
				throw new ArgumentNullException( "lang" );

			InfillsSplitsList( lang );

			InfillFormatsList( lang );

			InitLexemPage();

		}
		/// <summary>
		/// Saves defined language.
		/// </summary>
		/// <param name="lang"></param>
		private void SaveLanguage( IConfigLanguage lang )
		{
			if( lang == null )
				throw new ArgumentNullException( "lang" );


			bool defLangActive = ( GetActiveLanguage() == m_config.DefaultLanguage );


			/* Update One char splits list */
			lang.OneCharTokenSplits = txtOneChar.Text;

			/* Update Multi char splits list */
			lang.Splits.Clear();

			foreach( string split in lstMulti.ItemsList )
			{
				lang.Splits.Add( new Split( split, false ) );
			}

			/* Update extentions list */
			lang.Extensions.Clear();
			lang.Extensions.AddRange( lstExtensions.ItemsList );
		}
		/// <summary>
		/// Saves results.
		/// </summary>
		private void Save()
		{
			IConfigLanguage activeLang = GetActiveLanguage();

			if( activeLang != null )
			{
				// change all settings for active language.
				SaveLanguage( activeLang );

				// add all languages to config.
				m_config.Add( activeLang, DuplicatesOptions.SkipDuplicates );
			}
		}
		/// <summary>
		/// Saves all languages to config.
		/// </summary>
		private void SaveAll()
		{
			// save current active language.
			SavePreviousLanguage( comboLanguages.SelectedIndex );

			// add all languages to config.
			m_config.ProcessAndAppend( ( ConfigLanguage[] )m_newLanguages.ToArray( typeof( ConfigLanguage ) ),
				DuplicatesOptions.SkipDuplicates );
		}
		/// <summary>
		/// Enables/disables buttons Save and Delete for languages.
		/// </summary>
		private void ToggleControls()
		{
			bool defLangActive = ( GetActiveLanguage() == m_config.DefaultLanguage );

			btnDelete.Enabled = !defLangActive;
		}
		/// <summary>
		/// Creates new language configuration.
		/// </summary>
		private void CreateNewConfiguration()
		{
			FrmCreateLangDialog createDlg = new FrmCreateLangDialog( m_config );

			if( createDlg.ShowDialog( this ) == DialogResult.OK )
			{
				XmlDocument document = new XmlDocument();

				// inheritance configuration is from resources.
				if( createDlg.FilePath == null )
				{
					document.Load( Config.DefConfigStream );
				}
				else // inheritance configuration is from external file.
				{
					document.Load( createDlg.FilePath );
				}

				XmlNode node = document.SelectSingleNode( FrmCreateLangDialog.DEF_XPATH_LANG +
					"[@name='" + createDlg.InheritanceName + "']" );

				if( node != null && node is XmlElement )
				{
					XmlElement element = node as XmlElement;
					XmlAttribute attr = element.Attributes[ "name" ];

					if( attr != null )
					{
						attr.Value = createDlg.ConfigurationName;

						AddConfigLanguage( CreateConfigFromXml( element ) );
					}
				}
			}
			else
			{
				comboLanguages.SelectedIndex = m_activeIndex;
			}
		}
		/// <summary>
		/// Creates new configuration language from Xml Document.
		/// </summary>
		/// <param name="element">XML element with language configuration..</param>
		/// <returns>ConfigLanguage from document.</returns>
		private IConfigLanguage CreateConfigFromXml( XmlElement element )
		{
			if( element == null )
				throw new ArgumentNullException( "element" );

			ConfigLanguage language = null;
			using( StringReader reader = new StringReader( element.OuterXml ) )
			{

				XmlSerializer serializer = Syncfusion.XmlSerializersCreator.SerializersManager.GetSerializer( typeof( ConfigLanguage ) );
				language = ( ConfigLanguage )serializer.Deserialize( reader );
			}

			return language;
		}
		/// <summary>
		/// Adds language to collection.
		/// </summary>
		/// <param name="language">New language.</param>
		private void AddConfigLanguage( IConfigLanguage language )
		{
			if( language == null )
				throw new ArgumentNullException( "language" );

			m_newLanguages.Add( language );

			// NOTE: clear file extensions for new languages
			// because conflict with parent configuration may occur.
			language.Extensions.Clear();

			int index = comboLanguages.Items.Count - 1;
			comboLanguages.Items.Insert( index, language.Language );
			comboLanguages.SelectedIndex = index;
		}
		/// <summary>
		/// Saves previous active language from the list.
		/// </summary>
		private void SavePreviousLanguage( int index )
		{
			IConfigLanguage lang = GetLanguage( index );

			if( lang != null )
			{
				SaveLanguage( lang );
			}
		}
		/// <summary>
		/// Enables double buffering.
		/// </summary>
		public void EnableDoubleBuffering()
		{
			// Set the value of the double-buffering style bits to true.
			this.SetStyle( ControlStyles.DoubleBuffer |
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint,
				true );

			this.UpdateStyles();
		}
		#endregion

		#region Init Tab Pages Methods
		/// <summary>
		/// Initializes all control on defined Tab Page.
		/// </summary>
		/// <param name="page"></param>
		private void InitTabPage( TabPage page )
		{
			if( page == null )
				throw new ArgumentNullException( "page" );

			if( page.Tag != null ) return;

			if( page == tabFormats ) InitFormatPage();
			else if( page == tabLexems ) InitLexemPage();

			page.Tag = string.Empty;
		}
		/// <summary>
		/// Initializes Formats tab page.
		/// </summary>
		private void InitFormatPage()
		{
			if( tabFormats.Tag != null ) return;

			IConfigLanguage lang = GetActiveLanguage();

			if( lang != null )
			{
				InfillFormatsList( lang );
			}

			/* Infills color lists. */
			object[] colors = m_colors.ToArray();

			comboFontColor.Items.Clear();
			comboForeColor.Items.Clear();
			comboBackColor.Items.Clear();
			comboLineColor.Items.Clear();

			comboFontColor.Items.AddRange( colors );
			comboForeColor.Items.AddRange( colors );
			comboBackColor.Items.AddRange( colors );
			comboLineColor.Items.AddRange( colors );

			/* Infills hatchstyle list. */
			comboHatchStyle.Items.Clear();
			comboHatchStyle.Items.AddRange( m_hatchStyle.ToArray() );

			/* Infills underline style list */
			comboUnderlineStyle.Items.Clear();
			comboUnderlineStyle.Items.AddRange( m_underlineStyle.ToArray() );

			/* Infills underline weight list */
			comboUnderlineWeight.Items.Clear();
			comboUnderlineWeight.Items.AddRange( m_underlineWeight.ToArray() );

			ToggleFormatControls();
		}
		/// <summary>
		/// Initializes Lexems tab page.
		/// </summary>
		private void InitLexemPage()
		{
			ConfigLanguage lang = GetActiveLanguage() as ConfigLanguage;

			if( lang != null )
			{
				InfillLexemsList( lang );
			}

			comboFormat.Items.Clear();
			ISnippetFormat format = null;

			for( int i = 0, len = lang.Count; i < len; i++ )
			{
				format = lang[ i ] as ISnippetFormat;
				comboFormat.Items.Add( format.Name );
			}
            treeLexems.Focus();

			ToggleLexemControls();
		}
		/// <summary>
		/// Infills formats list.
		/// </summary>
		/// <param name="language">Language configuration, witch's data have to be used to fill lists.</param>
		private void InfillFormatsList( IConfigLanguage language )
		{
			if( language == null )
				throw new ArgumentNullException( "language" );

			ConfigLanguage lang = language as ConfigLanguage;

			/* Infills list of format names. */
			lsbFormats.Items.Clear();
			ISnippetFormat format = null;
			FormatState state = FormatState.Empty;

			for( int i = 0, len = lang.Count; i < len; i++ )
			{
				format = lang[ i ] as ISnippetFormat;
				lsbFormats.Items.Add( format.Name );

				state = new FormatState( format.Name );
				state.SaveState( format as Format );
				m_formatState[ format.Name ] = state;
			}
		}
		/// <summary>
		/// Fills in lists on first tab page by data from given language.
		/// </summary>
		/// <param name="lang">Language configuration, witch's data have to be used to fill lists.</param>
		private void InfillsSplitsList( IConfigLanguage lang )
		{
			if( lang == null )
				throw new ArgumentNullException( "lang" );

			/* Load One char splits list */
			txtOneChar.Text = lang.OneCharTokenSplits;

			/* Load Multi char splits list */
			lstMulti.Clear();

			foreach( Split split in lang.Splits )
			{
				lstMulti.Add( split.Text );
			}

			/* Load extentions list */
			lstExtensions.Clear();
			lstExtensions.AddRange( ( string[] )lang.Extensions.ToArray( typeof( string ) ) );
		}
		/// <summary>
		/// Adds nodes to tree.
		/// </summary>
		/// <param name="lang">Language configuration, witch's data have to be used to fill lists.</param>
		private void InfillLexemsList( IConfigLanguage lang )
		{
			if( lang == null )
				throw new ArgumentNullException( "lang" );

			treeLexems.Nodes.Clear();

			TreeNode node = new TreeNode( DEF_LEXEM_ROOT );
			treeLexems.Nodes.Add( node );

			for( int i = 0, len = lang.Lexems.Count; i < len; i++ )
			{
				InfillLexemsSubList( node, lang.Lexems[ i ] as ConfigLexem );
			}
		}
		/// <summary>
		/// Infills sub lexems in lexems tree.
		/// </summary>
		/// <param name="parent">Parent node.</param>
		/// <param name="lexem">Lexem configuration, witch's data have to be used.</param>
		private TreeNode InfillLexemsSubList( TreeNode parent, ConfigLexem lexem )
		{
			if( parent == null )
				throw new ArgumentNullException( "parent" );

			if( lexem == null )
				throw new ArgumentNullException( "lexem" );

			TreeNode node = new TreeNode( lexem.BeginBlock );
			node.Tag = lexem;

			parent.Nodes.Add( node );

			for( int i = 0, len = lexem.SubLexems.Count; i < len; i++ )
			{
				InfillLexemsSubList( node, lexem.SubLexems[ i ] as ConfigLexem );
			}

			return node;
		}
		#endregion

		#region Formats Utility Methods
		/// <summary>
		/// Returns selected format.
		/// </summary>
		/// <returns>Active format or NULL.</returns>
		private Format GetActiveFormat()
		{
			ConfigLanguage lang = GetActiveLanguage() as ConfigLanguage;

			if( lang == null || lsbFormats.SelectedIndex < 0 ||
				lsbFormats.SelectedIndex > lang.Count - 1 ) return null;

			return lang[ lsbFormats.SelectedIndex ] as Format;
		}
		/// <summary>
		/// Draws format sample.
		/// </summary>
		private void DrawFormatSample()
		{
			pnlSample.Invalidate();
		}
		/// <summary>
		/// Gets color name.
		/// Empty colors are returned correctly.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <returns>Color name.</returns>
		protected string GetColorName( Color color )
		{
			if( color.IsEmpty ) return DEF_EMPTY_COLOR;

			KnownColor knowncolor = ( KnownColor )Enum.Parse( typeof( KnownColor ), color.Name );

			return Localizer.GetEnumValueName( typeof( KnownColor ), knowncolor );
		}
		/// <summary>
		/// Loads all settings from active format.
		/// </summary>
		/// <param name="format">Format to be loaded.</param>
		private void LoadFormat( Format format )
		{
			if( format == null )
				throw new ArgumentNullException( "format" );

			int index = comboFontColor.Items.IndexOf( GetColorName( format.FontColor ) );
			index = ( index < 0 ) ? 0 : index;
			comboFontColor.SelectedIndex = index;

			index = comboForeColor.Items.IndexOf( GetColorName( format.ForeColor ) );
			index = ( index < 0 ) ? 0 : index;
			comboForeColor.SelectedIndex = index;

			index = comboBackColor.Items.IndexOf( GetColorName( format.BackColor ) );
			index = ( index < 0 ) ? 0 : index;
			comboBackColor.SelectedIndex = index;

			index = comboLineColor.Items.IndexOf( GetColorName( format.LineColor ) );
			index = ( index < 0 ) ? 0 : index;
			comboLineColor.SelectedIndex = index;

			index = comboHatchStyle.Items.IndexOf( Localizer.GetEnumValueName( typeof( HatchStyle ), format.HatchStyle ) );
			comboHatchStyle.SelectedIndex = index;

			index = comboUnderlineStyle.Items.IndexOf( Localizer.GetEnumValueName( typeof( UnderlineStyle ), format.UnderlineStyle ) );
			comboUnderlineStyle.SelectedIndex = index;

			index = comboUnderlineWeight.Items.IndexOf( Localizer.GetEnumValueName( typeof( UnderlineWeight ), format.UnderlineWeight ) );
			comboUnderlineWeight.SelectedIndex = index;
		}
		/// <summary>
		/// Enables/disables controls on format tab page.
		/// </summary>
		private void ToggleFormatControls()
		{
			Format format = GetActiveFormat();

			bool isEnabled = !( format == null );

			btnRemoveFormat.Enabled = btnFontFormat.Enabled = isEnabled;
			btnSetFonts.Enabled = isEnabled;
			comboFontColor.Enabled = isEnabled;
			comboForeColor.Enabled = isEnabled;
			comboBackColor.Enabled = isEnabled;
			comboLineColor.Enabled = isEnabled;
			comboHatchStyle.Enabled = isEnabled;
			comboUnderlineStyle.Enabled = isEnabled;
			comboUnderlineWeight.Enabled = isEnabled;
			btnRestore.Enabled = isEnabled;
		}
		/// <summary>
		/// Returns Regex for format names.
		/// </summary>
		/// <returns><see cref="Regex"/> with list of all existing format names.</returns>
		private Regex GetFormatNameRegex()
		{
			StringBuilder builder = new StringBuilder();

			for( int i = 0, len = lsbFormats.Items.Count; i < len; i++ )
			{
				builder.Append( "^" + Regex.Escape( ( string )lsbFormats.Items[ i ] ) +
					"$|" );
			}

			builder.Remove( builder.Length - 1, 1 );

			Regex result = new Regex( builder.ToString(), Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX );

			return result;
		}
		/// <summary>
		/// Creates new format and adds to collection.
		/// </summary>
		/// <param name="name">Name of the format.</param>
		private bool AddFormat( string name )
		{
			if( name == null )
				throw new ArgumentNullException( "name" );

			if( name.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_14 );

			Format format = new Format( name );

			ConfigLanguage lang = GetActiveLanguage() as ConfigLanguage;

			if( lang == null ) return false;

			lang.Add( format );
			int index = lsbFormats.Items.Add( format.Name );

			FormatState state = new FormatState( format.Name );
			state.SaveState( format );
			m_formatState[ format.Name ] = state;

			lsbFormats.SelectedIndex = index;

			comboFormat.Items.Add( format.Name );

			return true;
		}
		#endregion

		#region Lexems Utility Methods
		/// <summary>
		/// Returns active lexem.
		/// </summary>
		/// <returns>Lexem configuration or NULL if there 
		/// is no lexem configuration currently selected.</returns>
		private ConfigLexem GetActiveLexem()
		{
			TreeNode node = treeLexems.SelectedNode;

			if( node == null ||
				node.Tag == null ||
				node.Tag is ConfigLexem == false ) return null;

			return node.Tag as ConfigLexem;
		}
		/// <summary>
		/// Enables/disables controls on lexem edit page.
		/// </summary>
		private void ToggleLexemControls()
		{
			ConfigLexem lexem = GetActiveLexem();

			// disable buttons.
			if( lexem == null )
			{
				btnAddSubLexem.Enabled = btnRemoveLexem.Enabled = false;
			}
			else
			{
				btnAddSubLexem.Enabled = btnRemoveLexem.Enabled = true;
			}

			// Check value of begin block of lexem.
			bool enableLexem = ( txtBeginToken.Text.Length > 0 );

			txtContinueToken.Enabled = enableLexem;
			txtEndToken.Enabled = enableLexem;

			chkBeginToken.Enabled = enableLexem;
			chkContinueToken.Enabled = enableLexem;
			chkEndToken.Enabled = enableLexem;

			upDownPriority.Enabled = enableLexem;
			chkOnlyLocals.Enabled = enableLexem;
			chkIsComplex.Enabled = enableLexem;

			comboFormat.Enabled = enableLexem;
		}
		/// <summary>
		/// Loads lexem info to controls. 
		/// </summary>
		/// <param name="lexem">Lexem configuration to be loaded.</param>
		private void LoadLexem( ConfigLexem lexem )
		{
			if( lexem == null )
				throw new ArgumentNullException( "lexem" );

			SavePreviousLexem();
			ClearDataBindings();

			comboFormat.SelectedIndex = comboFormat.Items.IndexOf( lexem.Format.Name );

			txtBeginToken.DataBindings.Add( "Text", lexem, "BeginBlock" );
			txtContinueToken.DataBindings.Add( "Text", lexem, "ContinueBlock" );
			txtEndToken.DataBindings.Add( "Text", lexem, "EndBlock" );
			chkBeginToken.DataBindings.Add( "Checked", lexem, "IsBeginRegex" );
			chkContinueToken.DataBindings.Add( "Checked", lexem, "IsContinueRegex" );
			chkEndToken.DataBindings.Add( "Checked", lexem, "IsEndRegex" );
            if (lexem.Priority < 0)
            {
                lexem.Priority = 0;
            }
            upDownPriority.DataBindings.Add("Value", lexem, "Priority");
			chkOnlyLocals.DataBindings.Add( "Checked", lexem, "OnlyLocalSublexems" );
			chkIsComplex.DataBindings.Add( "Checked", lexem, "IsComplex" );
		}
		/// <summary>
		/// Restores controls to their default values.
		/// </summary>
		private void RestoreLexemControls()
		{
			ClearDataBindings();

			string text = string.Empty;
			txtBeginToken.Text = text;
			txtContinueToken.Text = text;
			txtEndToken.Text = text;

			comboFormat.SelectedIndex = -1;

			chkBeginToken.Checked = false;
			chkContinueToken.Checked = false;
			chkEndToken.Checked = false;

			upDownPriority.Value = upDownPriority.Minimum;

			chkOnlyLocals.Checked = false;
			chkIsComplex.Checked = false;
		}
		/// <summary>
		/// Builds regex for checking name of new lexem.
		/// </summary>
		/// <param name="lexem"></param>
		/// <returns>
		/// <see cref="Regex"/> with list of names of lexems 
		/// that belongs to the parent of the given one.
		/// </returns>
		private Regex GetLexemNameRegex( ConfigLexem lexem )
		{
			if( lexem == null )
				throw new ArgumentNullException( "lexem" );

			IList lexems = lexem.ParentConfig.SubLexems;

			if( lexems == null ) return null;

			StringBuilder builder = new StringBuilder();
			ConfigLexem neighbourLexem = null;

			for( int i = 0, len = lexems.Count; i < len; i++ )
			{
				neighbourLexem = lexems[ i ] as ConfigLexem;
				builder.Append( "^" + Regex.Escape( neighbourLexem.BeginBlock ) + "$|" );
			}

			if( builder.Length > 0 ) builder.Remove( builder.Length - 1, 1 );

			return new Regex( builder.ToString(), Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX );
		}
		/// <summary>
		/// Adds new lexem to collection..
		/// </summary>
		/// <param name="lexemName">Name of the lexem configuration..</param>
		/// <param name="parentLexem">Parent of the lexem configuration.</param>
		/// <param name="subLexem"></param>
		private void AddLexem( IConfigLexem parentLexem, string lexemName, bool subLexem )
		{
			if( parentLexem == null )
				throw new ArgumentNullException( "parentLexem" );

			if( lexemName == null )
				throw new ArgumentNullException( "lexemName" );

			ConfigLexem lexem = new ConfigLexem();
			lexem.BeginBlock = lexemName;
			lexem.ParentConfig = parentLexem;
			parentLexem.SubLexems.Add( lexem );

			TreeNode parentNode = ( subLexem || treeLexems.SelectedNode.Parent == null ) ?
				treeLexems.SelectedNode : treeLexems.SelectedNode.Parent;

			TreeNode activeNode = InfillLexemsSubList( parentNode, lexem );

			treeLexems.SelectedNode = activeNode;
			treeLexems.Focus();
		}
		/// <summary>
		/// Clears all DataBindings.
		/// </summary>
		private void ClearDataBindings()
		{
			txtBeginToken.DataBindings.Clear();
			txtContinueToken.DataBindings.Clear();
			txtEndToken.DataBindings.Clear();
			chkBeginToken.DataBindings.Clear();
			chkContinueToken.DataBindings.Clear();
			chkEndToken.DataBindings.Clear();
			upDownPriority.DataBindings.Clear();
			chkOnlyLocals.DataBindings.Clear();
			chkIsComplex.DataBindings.Clear();

		}
		/// <summary>
		/// Saves previous lexem.
		/// </summary>
		private void SavePreviousLexem()
		{
			ConfigLanguage lang = GetActiveLanguage() as ConfigLanguage;

			if( lang == null ) return;

			if( m_activeLexem != null )
			{
				PropertyManager manager = ( PropertyManager )this.BindingContext[ m_activeLexem ];

				if( manager != null )
				{
					manager.EndCurrentEdit();
				}

				// type of format is standard.
				if( m_formatType.Contains( comboFormat.Text ) )
				{
					m_activeLexem.Type = ( FormatType )Enum.Parse( typeof( FormatType ),
						comboFormat.Text, true );
				}
				else // format has custom type.
				{
					m_activeLexem.Type = FormatType.Custom;
					m_activeLexem.FormatName = comboFormat.Text;
				}
			}
		}
		#endregion
	}
	#endregion

	#region *** frmConfigDialog
	/// <summary>
	/// Summary description for frmConfigDialog.
	/// </summary>
	[Obsolete( "This class has been renamed. Please use ConfigurationDialog instead." )]
	public class frmConfigDialog
		: ConfigurationDialog
	{
		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="configurator"><see cref="Config"/> class instance.</param>
		/// <param name="activeLang">Active configuration language.</param>
		public frmConfigDialog( Config configurator, IConfigLanguage activeLang )
			: base( configurator, activeLang )
		{ }
	}
	#endregion
}