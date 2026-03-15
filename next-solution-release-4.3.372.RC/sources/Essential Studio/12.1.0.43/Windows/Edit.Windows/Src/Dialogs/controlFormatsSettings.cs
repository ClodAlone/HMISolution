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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;

using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Control for EditControl's formats settings.
	/// </summary>
	[ToolboxItem( false )]
	public class ControlFormatsSettings : System.Windows.Forms.UserControl
	{
		#region Classes
		/// <summary>
		/// String representation of format data.
		/// </summary>
		protected struct FormatStringData
		{
			/// <summary>
			/// Font size.
			/// </summary>
			public float FontSize;
			/// <summary>
			/// Font name.
			/// </summary>
			public string FontName;
			/// <summary>
			/// Font style, string representation of SimpleFontStyle enum value.
			/// </summary>
			public string FontSyle;
			/// <summary>
			/// Font color, string representation.
			/// </summary>
			public string FontColor;
			/// <summary>
			/// Background color, string representation.
			/// </summary>
			public string BackgroundColor;
			/// <summary>
			/// Background style, string representation of HatchStyle enum value or Solid.
			/// </summary>
			public string BackgroundStyle;
			/// <summary>
			/// Border color, string representation.
			/// </summary>
			public string BorderColor;
			/// <summary>
			/// Underline weight, string representation of UnderlineWeight enum value.
			/// </summary>
			public string UnderlineWeight;
			/// <summary>
			/// Underline color, string representation.
			/// </summary>
			public string UnderlineColor;
			/// <summary>
			/// Underline style, string representation of UnderlineStyle enum value.
			/// </summary>
			public string UnderlineStyle;
			/// <summary>
			/// Text strike out color, string representation.
			/// </summary>
			public string StrikeOutColor;
		}
		/// <summary>
		/// Font styles used in combo.
		/// </summary>
		protected enum SimpleFontStyle
		{
			/// <summary>
			/// Regular style.
			/// </summary>
			Regular,
			/// <summary>
			/// Bold style.
			/// </summary>
			Bold,
			/// <summary>
			/// Italic style.
			/// </summary>
			Italic,
			/// <summary>
			/// Bold italic style.
			/// </summary>
			BoldItalic
		}
		#endregion

		#region Constants
		/// <summary>
		/// Width of the preview box.
		/// </summary>
		private const int PREVIEW_BOX_WIDTH = 20;
		/// <summary>
		/// Width of the arrow.
		/// </summary>
		public const int ARROW_WIDTH = 12;
		/// <summary>
		/// Empty color name.
		/// </summary>
		private string DEF_COLOR_EMPTY = Localizer.DEF_COLOR_EMPTY;
		/// <summary>
		/// Name of the item in combo that should be selected in case of solid fill.
		/// </summary>
		private string DEF_SOLID_FILL = Localizer.DEF_SOLID_FILL;
		#endregion

		#region Fields
		/// <summary>
		/// List of the formats to edit.
		/// </summary>
		private FormatsCollection m_formatsList;
		/// <summary>
		/// Specifies whether handling of events is disabled.
		/// </summary>
		private int m_iLockEventHandling;
		/// <summary>
		/// Formats selector control.
		/// </summary>
		private ControlFormatsList m_formatSelector;
		/// <summary>
		/// Font index in comboFonts.
		/// </summary>
		private int m_previousFontPositon;
		#endregion

		#region Events
		/// <summary>
		/// Event that is raised when user changes some settings.
		/// </summary>
		public event EventHandler Changed;
		#endregion

		#region Properties
		/// <summary>
		/// Gets value indication whether event handling is locked.
		/// </summary>
		protected bool isLocked
		{
			get
			{
				return ( m_iLockEventHandling != 0 );
			}
		}
		/// <summary>
		/// Gets or sets list of the formats to be edited.
		/// If FormatsSelector is set, this property will return selection of the FormatsSelector.
		/// </summary>
		public IList Formats
		{
			get
			{
				if( FormatsSelector != null )
				{
					ISnippetFormat[] formats = FormatsSelector.SelectedFormats;

					if( formats == null )
						return null;

					return new ArrayList( FormatsSelector.SelectedFormats );
				}

				return m_formatsList;
			}
			set
			{
				m_formatsList.Clear();

				if( value != null )
				{
					LockEventProcessing();
					m_formatsList.AddRange( value );
					UnlockEventProcessing();

					UpdateSettingsFromFormats();
				}
			}
		}
		/// <summary>
		/// Gets or sets control that is used for selecting formats.
		/// </summary>
		[Browsable( true )]
		[TypeConverter( typeof( ComponentConverter ) )]
		[Category( "Data" )]
		public ControlFormatsList FormatsSelector
		{
			get
			{
				return m_formatSelector;
			}
			set
			{
				if( m_formatSelector != value )
				{
					if( m_formatSelector != null )
						m_formatSelector.SelectedFormatChanged -= new EventHandler( SelectorFormatsSelectionChanged );

					m_formatSelector = value;

					UpdateSettingsFromFormats();

					if( m_formatSelector != null )
						m_formatSelector.SelectedFormatChanged += new EventHandler( SelectorFormatsSelectionChanged );
				}
			}
		}
		#endregion

		#region Controls
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboFontColors;
		private System.Windows.Forms.ComboBox comboFonts;
		private System.Windows.Forms.ComboBox comboFontStyle;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboBackgroundColor;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboBorderColor;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox comboBackgroundFillStyle;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboUnderlineWeight;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboUnderlineColor;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox comboUnderlineStyle;
		private Syncfusion.Windows.Forms.Tools.DoubleTextBox numFontSize;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ComboBox comboStrikeOutColor;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Initialization/Finalization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		public ControlFormatsSettings()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			m_formatsList = new FormatsCollection();
			m_formatsList.OnChanged += new EventHandler( m_formatsList_OnChanged );

			InitializeCombos();
		}
		/// <summary>
		/// Initializes comboboxes.
		/// </summary>
		protected void InitializeCombos()
		{
			foreach( FontFamily family in FontFamily.Families )
			{
				comboFonts.Items.Add( family.Name );
			}

			string[] namesKnownColors = Localizer.GetEnumNames( typeof( KnownColor ) );
			string[] namesSimpleFontStyle = Localizer.GetEnumNames( typeof( SimpleFontStyle ) );
			string[] namesHatchStyle = Localizer.GetEnumNames( typeof( HatchStyle ) );
			string[] namesUnderlineStyle = Localizer.GetEnumNames( typeof( UnderlineStyle ) );
			string[] namesUnderlineWeight = Localizer.GetEnumNames( typeof( UnderlineWeight ) );

			comboFontColors.Items.Add( DEF_COLOR_EMPTY );
			comboFontColors.Items.AddRange( namesKnownColors );

			comboFontStyle.Items.AddRange( namesSimpleFontStyle );

			comboBackgroundColor.Items.Add( DEF_COLOR_EMPTY );
			comboBackgroundColor.Items.AddRange( namesKnownColors );

			comboBackgroundFillStyle.Items.Add( DEF_SOLID_FILL );
			comboBackgroundFillStyle.Items.AddRange( namesHatchStyle );

			comboBorderColor.Items.Add( DEF_COLOR_EMPTY );
			comboBorderColor.Items.AddRange( namesKnownColors );

			comboStrikeOutColor.Items.Add( DEF_COLOR_EMPTY );
			comboStrikeOutColor.Items.AddRange( namesKnownColors );

			comboUnderlineColor.Items.Add( DEF_COLOR_EMPTY );
			comboUnderlineColor.Items.AddRange( namesKnownColors );
			comboUnderlineStyle.Items.AddRange( namesUnderlineStyle );
			comboUnderlineWeight.Items.AddRange( namesUnderlineWeight );

			DisableControls();
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
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ControlFormatsSettings ) );
			this.comboFonts = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboFontStyle = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboFontColors = new System.Windows.Forms.ComboBox();
			this.comboBackgroundColor = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.comboBorderColor = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.comboBackgroundFillStyle = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboStrikeOutColor = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.numFontSize = new Syncfusion.Windows.Forms.Tools.DoubleTextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboUnderlineStyle = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.comboUnderlineColor = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboUnderlineWeight = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize )( this.numFontSize ) ).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboFonts
			// 
			this.comboFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboFonts, "comboFonts" );
			this.comboFonts.Name = "comboFonts";
			this.comboFonts.SelectedIndexChanged += new System.EventHandler( this.comboFonts_SelectedIndexChanged );
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label1, "label1" );
			this.label1.Name = "label1";
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label2, "label2" );
			this.label2.Name = "label2";
			// 
			// label3
			// 
			this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label3, "label3" );
			this.label3.Name = "label3";
			// 
			// comboFontStyle
			// 
			this.comboFontStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboFontStyle, "comboFontStyle" );
			this.comboFontStyle.Name = "comboFontStyle";
			this.comboFontStyle.SelectedIndexChanged += new System.EventHandler( this.comboFontStyle_SelectedIndexChanged );
			// 
			// label4
			// 
			this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label4, "label4" );
			this.label4.Name = "label4";
			// 
			// comboFontColors
			// 
			this.comboFontColors.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.comboFontColors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboFontColors, "comboFontColors" );
			this.comboFontColors.Name = "comboFontColors";
			this.comboFontColors.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboBorderColor_DrawItem );
			this.comboFontColors.SelectedIndexChanged += new System.EventHandler( this.comboFontColors_SelectedIndexChanged );
			// 
			// comboBackgroundColor
			// 
			this.comboBackgroundColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.comboBackgroundColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboBackgroundColor, "comboBackgroundColor" );
			this.comboBackgroundColor.Name = "comboBackgroundColor";
			this.comboBackgroundColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboBorderColor_DrawItem );
			this.comboBackgroundColor.SelectedIndexChanged += new System.EventHandler( this.comboBackgroundColor_SelectedIndexChanged );
			// 
			// label7
			// 
			this.label7.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label7, "label7" );
			this.label7.Name = "label7";
			// 
			// label8
			// 
			this.label8.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label8, "label8" );
			this.label8.Name = "label8";
			// 
			// comboBorderColor
			// 
			this.comboBorderColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.comboBorderColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboBorderColor, "comboBorderColor" );
			this.comboBorderColor.Name = "comboBorderColor";
			this.comboBorderColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboBorderColor_DrawItem );
			this.comboBorderColor.SelectedIndexChanged += new System.EventHandler( this.comboBorderColor_SelectedIndexChanged );
			// 
			// label9
			// 
			this.label9.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label9, "label9" );
			this.label9.Name = "label9";
			// 
			// comboBackgroundFillStyle
			// 
			this.comboBackgroundFillStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboBackgroundFillStyle, "comboBackgroundFillStyle" );
			this.comboBackgroundFillStyle.Name = "comboBackgroundFillStyle";
			this.comboBackgroundFillStyle.SelectedIndexChanged += new System.EventHandler( this.comboBackgroundFillStyle_SelectedIndexChanged );
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add( this.comboStrikeOutColor );
			this.groupBox1.Controls.Add( this.label11 );
			this.groupBox1.Controls.Add( this.comboFonts );
			this.groupBox1.Controls.Add( this.label1 );
			this.groupBox1.Controls.Add( this.label2 );
			this.groupBox1.Controls.Add( this.label3 );
			this.groupBox1.Controls.Add( this.comboFontStyle );
			this.groupBox1.Controls.Add( this.label4 );
			this.groupBox1.Controls.Add( this.comboFontColors );
			this.groupBox1.Controls.Add( this.numFontSize );
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.groupBox1, "groupBox1" );
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// comboStrikeOutColor
			// 
			this.comboStrikeOutColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.comboStrikeOutColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboStrikeOutColor, "comboStrikeOutColor" );
			this.comboStrikeOutColor.Name = "comboStrikeOutColor";
			this.comboStrikeOutColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboBorderColor_DrawItem );
			this.comboStrikeOutColor.SelectedIndexChanged += new System.EventHandler( this.comboStrikeOutColor_SelectedIndexChanged );
			// 
			// label11
			// 
			resources.ApplyResources( this.label11, "label11" );
			this.label11.Name = "label11";
			// 
			// numFontSize
			// 
			this.numFontSize.DoubleValue = 1;
			resources.ApplyResources( this.numFontSize, "numFontSize" );
			this.numFontSize.MaxValue = 40;
			this.numFontSize.MinValue = 0;
			this.numFontSize.Name = "numFontSize";
			this.numFontSize.NegativeInputPendingOnSelectAll = false;
			this.numFontSize.NullString = "0.0";
			this.numFontSize.OverflowIndicatorToolTipText = null;
			this.numFontSize.ReadOnlyBackColor = System.Drawing.SystemColors.Control;
			this.numFontSize.DoubleValueChanged += new System.EventHandler( this.numFontSize_ValueChanged );
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add( this.label8 );
			this.groupBox2.Controls.Add( this.label7 );
			this.groupBox2.Controls.Add( this.comboBackgroundFillStyle );
			this.groupBox2.Controls.Add( this.label9 );
			this.groupBox2.Controls.Add( this.comboBorderColor );
			this.groupBox2.Controls.Add( this.comboBackgroundColor );
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.groupBox2, "groupBox2" );
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add( this.comboUnderlineStyle );
			this.groupBox3.Controls.Add( this.label10 );
			this.groupBox3.Controls.Add( this.comboUnderlineColor );
			this.groupBox3.Controls.Add( this.label6 );
			this.groupBox3.Controls.Add( this.comboUnderlineWeight );
			this.groupBox3.Controls.Add( this.label5 );
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.groupBox3, "groupBox3" );
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// comboUnderlineStyle
			// 
			this.comboUnderlineStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboUnderlineStyle, "comboUnderlineStyle" );
			this.comboUnderlineStyle.Name = "comboUnderlineStyle";
			this.comboUnderlineStyle.SelectedIndexChanged += new System.EventHandler( this.comboUnderlineStyle_SelectedIndexChanged );
			// 
			// label10
			// 
			this.label10.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label10, "label10" );
			this.label10.Name = "label10";
			// 
			// comboUnderlineColor
			// 
			this.comboUnderlineColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.comboUnderlineColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboUnderlineColor, "comboUnderlineColor" );
			this.comboUnderlineColor.Name = "comboUnderlineColor";
			this.comboUnderlineColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.comboBorderColor_DrawItem );
			this.comboUnderlineColor.SelectedIndexChanged += new System.EventHandler( this.comboUnderlineColor_SelectedIndexChanged );
			// 
			// label6
			// 
			this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label6, "label6" );
			this.label6.Name = "label6";
			// 
			// comboUnderlineWeight
			// 
			this.comboUnderlineWeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources( this.comboUnderlineWeight, "comboUnderlineWeight" );
			this.comboUnderlineWeight.Name = "comboUnderlineWeight";
			this.comboUnderlineWeight.SelectedIndexChanged += new System.EventHandler( this.comboUnderlineWeight_SelectedIndexChanged );
			// 
			// label5
			// 
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources( this.label5, "label5" );
			this.label5.Name = "label5";
			// 
			// ControlFormatsSettings
			// 
			this.Controls.Add( this.groupBox3 );
			this.Controls.Add( this.groupBox2 );
			this.Controls.Add( this.groupBox1 );
			this.Name = "ControlFormatsSettings";
			resources.ApplyResources( this, "$this" );
			this.groupBox1.ResumeLayout( false );
			this.groupBox1.PerformLayout();
			( ( System.ComponentModel.ISupportInitialize )( this.numFontSize ) ).EndInit();
			this.groupBox2.ResumeLayout( false );
			this.groupBox3.ResumeLayout( false );
			this.ResumeLayout( false );

		}
		#endregion

		#region Helper Methods
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
		/// Disables controls and sets empty values.
		/// </summary>
		protected void DisableControls()
		{
			comboFonts.SelectedIndex = -1;
			comboFontColors.SelectedIndex = -1;
			comboFontStyle.SelectedIndex = -1;
			comboBackgroundColor.SelectedIndex = -1;
			comboBackgroundFillStyle.SelectedIndex = -1;
			comboBorderColor.SelectedIndex = -1;
			comboUnderlineColor.SelectedIndex = -1;
			comboStrikeOutColor.SelectedIndex = -1;
			comboUnderlineStyle.SelectedIndex = -1;
			comboUnderlineWeight.SelectedIndex = -1;
			numFontSize.DoubleValue = 1;

			comboFonts.Enabled = false;
			comboFontColors.Enabled = false;
			comboFontStyle.Enabled = false;
			comboBackgroundColor.Enabled = false;
			comboBackgroundFillStyle.Enabled = false;
			comboBorderColor.Enabled = false;
			comboUnderlineColor.Enabled = false;
			comboStrikeOutColor.Enabled = false;
			comboUnderlineStyle.Enabled = false;
			comboUnderlineWeight.Enabled = false;
			numFontSize.Enabled = false;
		}
		/// <summary>
		/// Enables all controls.
		/// </summary>
		protected void EnableControls()
		{
			numFontSize.Enabled = true;
			comboFonts.Enabled = true;
			comboFontColors.Enabled = true;
			comboFontStyle.Enabled = true;
			comboBackgroundColor.Enabled = true;
			comboBackgroundFillStyle.Enabled = true;
			comboBorderColor.Enabled = true;
			comboUnderlineColor.Enabled = true;
			comboStrikeOutColor.Enabled = true;
			comboUnderlineStyle.Enabled = true;
			comboUnderlineWeight.Enabled = true;
		}
		/// <summary>
		/// Converts bold and italic values combination to SimpleFontStyle.
		/// </summary>
		/// <param name="bold">Font style bold value.</param>
		/// <param name="italic">Font style italic value.</param>
		/// <returns>SimpleFontStyle value.</returns>
		protected SimpleFontStyle GetSimpleFontStyle( bool bold, bool italic )
		{
			if( bold && !italic )
				return SimpleFontStyle.Bold;

			if( !bold && italic )
				return SimpleFontStyle.Italic;

			if( bold && italic )
				return SimpleFontStyle.BoldItalic;

			return SimpleFontStyle.Regular;
		}
		/// <summary>
		/// Gets font style from string representation of SimpleFontStyle.
		/// </summary>
		/// <param name="styleString">SimpleFontStyle string representation.</param>
		/// <returns>FontStyle.</returns>
		protected FontStyle GetFontStyle( string styleString )
		{
			SimpleFontStyle style = ( SimpleFontStyle )Localizer.GetEnumValue( typeof( SimpleFontStyle ), styleString );

			switch( style )
			{
				case SimpleFontStyle.Bold:
					return FontStyle.Bold;

				case SimpleFontStyle.Italic:
					return FontStyle.Italic;

				case SimpleFontStyle.BoldItalic:
					return FontStyle.Bold | FontStyle.Italic;

				default:
					return FontStyle.Regular;
			}
		}
		/// <summary>
		/// Gets color name. Empty colors are returned correctly.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <returns>Color name.</returns>
		protected string GetColorName( Color color )
		{
			if( color.IsEmpty ) return DEF_COLOR_EMPTY;

			KnownColor knowncolor = ( KnownColor )Enum.Parse( typeof( KnownColor ), color.Name );

			return Localizer.GetEnumValueName( typeof( KnownColor ), knowncolor );
		}
		/// <summary>
		/// Get color by name.
		/// </summary>
		/// <param name="color">Color name or Empty.</param>
		/// <returns>Color value.</returns>
		protected Color GetColor( string color )
		{
			if( color == DEF_COLOR_EMPTY || color == string.Empty )
				return Color.Empty;

			string nativeColorName = Localizer.GetNativeEnumValueName( typeof( KnownColor ), color );
			Color col = Color.FromName( nativeColorName );

			return col;
		}
		/// <summary>
		/// Updates all formats with nonempty data from "data" argument.
		/// </summary>
		/// <param name="data">Data values.</param>
		protected void UpdateFormats( FormatStringData data )
		{
			LockEventProcessing();

			try
			{
				foreach( Format format in Formats )
				{
					if( data.BackgroundColor != string.Empty && data.BackgroundColor != null )
						format.BackColor = GetColor( data.BackgroundColor );

					if( data.BackgroundStyle != string.Empty && data.BackgroundStyle != null )
					{
						format.UseHatchFill = ( data.BackgroundStyle != DEF_SOLID_FILL );

						if( format.UseHatchFill )
							format.HatchStyle = ( HatchStyle )Localizer.GetEnumValue( typeof( HatchStyle ), data.BackgroundStyle );
					}

					if( data.BorderColor != string.Empty && data.BorderColor != null )
						format.ForeColor = GetColor( data.BorderColor );

					if( data.FontColor != string.Empty && data.FontColor != null )
						format.FontColor = GetColor( data.FontColor );

					if( ( data.FontName != string.Empty && data.FontName != null )
						|| data.FontSize != 0f
						|| ( data.FontSyle != string.Empty && data.FontSyle != null ) )
					{
						format.Font = new Font(
							( ( data.FontName != string.Empty && data.FontName != null )
								? data.FontName : format.Font.FontFamily.Name ),
							( ( data.FontSize != 0f ) ? data.FontSize : format.Font.SizeInPoints ),
							( ( data.FontSyle != string.Empty && data.FontSyle != null )
								? GetFontStyle( data.FontSyle ) : format.Font.Style ) );
					}

					if( data.UnderlineColor != string.Empty && data.UnderlineColor != null )
						format.LineColor = GetColor( data.UnderlineColor );

					if( data.UnderlineStyle != string.Empty && data.UnderlineStyle != null )
						format.UnderlineStyle = ( UnderlineStyle )Localizer.GetEnumValue( typeof( UnderlineStyle ), data.UnderlineStyle );

					if( data.UnderlineWeight != string.Empty && data.UnderlineWeight != null )
						format.UnderlineWeight = ( UnderlineWeight )Localizer.GetEnumValue( typeof( UnderlineWeight ), data.UnderlineWeight );

					if( data.StrikeOutColor != string.Empty && data.StrikeOutColor != null )
						format.StrikeOutColor = GetColor( data.StrikeOutColor );
				}
			}
			catch( Exception exp )
			{
				comboFonts.SelectedIndex = m_previousFontPositon;

				MessageBox.Show( exp.Message, Localizer.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				UnlockEventProcessing();
				OnChanged();
			}
		}
		/// <summary>
		/// Gets string representation of formats data.
		/// </summary>
		/// <param name="format">Format. Can not be null.</param>
		/// <returns>FormatStringData structure.</returns>
		protected FormatStringData GetFormatStrings( Format format )
		{
			if( format == null ) throw new ArgumentNullException( "format" );

			FormatStringData data = new FormatStringData();
			data.BackgroundColor = GetColorName( format.BackColor );
			data.BackgroundStyle = ( format.UseHatchFill ) ? ( Localizer.GetEnumValueName( typeof( HatchStyle ), format.HatchStyle ) ) : ( DEF_SOLID_FILL );
			data.BorderColor = GetColorName( format.ForeColor );
			data.FontColor = GetColorName( format.FontColor );
			data.FontName = format.Font.FontFamily.Name;
			data.FontSize = format.Font.SizeInPoints;
			data.FontSyle = Localizer.GetEnumValueName( typeof( SimpleFontStyle ), GetSimpleFontStyle( format.Font.Bold, format.Font.Italic ) );
			data.UnderlineColor = GetColorName( format.LineColor );
			data.UnderlineStyle = Localizer.GetEnumValueName( typeof( UnderlineStyle ), format.UnderlineStyle );
			data.UnderlineWeight = Localizer.GetEnumValueName( typeof( UnderlineWeight ), format.UnderlineWeight );
			data.StrikeOutColor = GetColorName( format.StrikeOutColor );

			return data;
		}
		/// <summary>
		/// Merges 2 strings. If both string have the same value, resulting string will have this value, otherwise empty string will be returned.
		/// </summary>
		/// <param name="str1">String 1.</param>
		/// <param name="str2">String 2.</param>
		/// <returns>If both string have the same value, method returns this value, otherwise empty string will be returned.</returns>
		protected string MergeString( string str1, string str2 )
		{
			if( str1 == str2 ) return str1;

			return string.Empty;
		}
		/// <summary>
		/// Merges formats string representations.
		/// </summary>
		/// <param name="arg1">First format representation.</param>
		/// <param name="arg2">Second format representation.</param>
		/// <returns>Merged formats string representation.</returns>
		protected FormatStringData MergeFormatData( FormatStringData arg1, FormatStringData arg2 )
		{
			FormatStringData result = new FormatStringData();

			result.BackgroundColor = MergeString( arg1.BackgroundColor, arg2.BackgroundColor );
			result.BackgroundStyle = MergeString( arg1.BackgroundStyle, arg2.BackgroundStyle );
			result.BorderColor = MergeString( arg1.BorderColor, arg2.BorderColor );
			result.FontColor = MergeString( arg1.FontColor, arg2.FontColor );
			result.FontName = MergeString( arg1.FontName, arg2.FontName );
			result.FontSyle = MergeString( arg1.FontSyle, arg2.FontSyle );
			result.UnderlineColor = MergeString( arg1.UnderlineColor, arg2.UnderlineColor );
			result.UnderlineStyle = MergeString( arg1.UnderlineStyle, arg2.UnderlineStyle );
			result.UnderlineWeight = MergeString( arg1.UnderlineWeight, arg2.UnderlineWeight );
			result.StrikeOutColor = MergeString( arg1.StrikeOutColor, arg2.StrikeOutColor );

			if( arg1.FontSize == arg2.FontSize )
				result.FontSize = arg1.FontSize;
			else
				result.FontSize = 0;

			return result;
		}
		/// <summary>
		/// Gets merged string representation of formats.
		/// </summary>
		/// <returns>String representation of formats list.</returns>
		protected FormatStringData GetMergedFormatData()
		{
			FormatStringData data = new FormatStringData();

			if( Formats.Count == 0 ) return data;

			Format firstFormat = ( Format )Formats[ 0 ];
			data = GetFormatStrings( firstFormat );

			foreach( Format format in Formats )
			{
				data = MergeFormatData( GetFormatStrings( format ), data );
			}

			return data;
		}
		/// <summary>
		/// Updates all settings.
		/// </summary>
		protected void UpdateSettingsFromFormats()
		{
			IList formats = Formats;

			if( formats == null || formats.Count == 0 )
			{
				DisableControls();
				return;
			}
			else
			{
				EnableControls();
			}

			FormatStringData data = GetMergedFormatData();

			LockEventProcessing();
			SelectItemByValue( comboBackgroundColor, data.BackgroundColor );
			SelectItemByValue( comboBackgroundFillStyle, data.BackgroundStyle );
			SelectItemByValue( comboBorderColor, data.BorderColor );
			SelectItemByValue( comboFontColors, data.FontColor );
			SelectItemByValue( comboFonts, data.FontName );
			SelectItemByValue( comboFontStyle, data.FontSyle );
			SelectItemByValue( comboUnderlineColor, data.UnderlineColor );
			SelectItemByValue( comboUnderlineStyle, data.UnderlineStyle );
			SelectItemByValue( comboUnderlineWeight, data.UnderlineWeight );
			SelectItemByValue( comboStrikeOutColor, data.StrikeOutColor );

			numFontSize.DoubleValue = data.FontSize;
			UnlockEventProcessing();
		}
		/// <summary>
		/// Selects combobox item by value.
		/// </summary>
		/// <param name="combo">Combobox</param>
		/// <param name="value">value to be selected.</param>
		protected void SelectItemByValue( ComboBox combo, object value )
		{
			if( combo == null )
				throw new ArgumentNullException( "combo" );

			combo.SelectedIndex = combo.Items.IndexOf( value );
		}
		/// <summary>
		/// Locks processing of events.
		/// </summary>
		protected void LockEventProcessing()
		{
			m_iLockEventHandling++;
		}
		/// <summary>
		/// Unlocks processing of events.
		/// </summary>
		protected void UnlockEventProcessing()
		{
			m_iLockEventHandling = Math.Max( 0, --m_iLockEventHandling );
		}
		/// <summary>
		/// Gets selected value of the combobox.
		/// </summary>
		/// <param name="combo">Combobox.</param>
		/// <returns>Selected value or empty string if no value is selected.</returns>
		protected string GetComboBoxValue( ComboBox combo )
		{
			if( combo == null )
				throw new ArgumentNullException( "combo" );

			if( combo.SelectedIndex >= 0 )
				return ( string )combo.SelectedItem;

			return string.Empty;
		}
		/// <summary>
		/// Raises Changed event.
		/// </summary>
		protected void OnChanged()
		{
			if( Changed != null )
			{
				Changed( this, EventArgs.Empty );
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Updates controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_formatsList_OnChanged( object sender, EventArgs e )
		{
			if( isLocked ) return;

			UpdateSettingsFromFormats();
		}
		/// <summary>
		/// Updates data.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectorFormatsSelectionChanged( object sender, EventArgs e )
		{
			if( isLocked ) return;

			UpdateSettingsFromFormats();
		}
		/// <summary>
		/// Draws color combobox item. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBorderColor_DrawItem( object sender, System.Windows.Forms.DrawItemEventArgs e )
		{
			ComboBox combo = sender as ComboBox;
			if( combo == null ) return;
			if( e.Index < 0 ) return;

			string item = combo.Items[ e.Index ].ToString();
			Color clr = Color.Empty;

			if( item != DEF_COLOR_EMPTY )
				clr = Color.FromName( Localizer.GetNativeEnumValueName( typeof( KnownColor ), item.ToString() ) );

			using( Brush brush = new SolidBrush( clr ) )
			{
				DrawComboItem( combo, brush, e );
			}
		}
		/// <summary>
		/// Changes font name.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboFonts_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( !isLocked )
			{
				if( Formats != null && ( ( ComboBox )sender ).SelectedIndex != -1 )
				{
					FormatStringData data = new FormatStringData();
					data.FontName = comboFonts.SelectedItem.ToString();

					UpdateFormats( data );
				}
			}

			m_previousFontPositon = comboFonts.SelectedIndex;
		}
		/// <summary>
		/// Changes font size.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void numFontSize_ValueChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || numFontSize.DoubleValue == 0 )
				return;

			FormatStringData data = new FormatStringData();
			data.FontSize = ( float )numFontSize.DoubleValue;

			UpdateFormats( data );
		}
		/// <summary>
		/// Changes font style.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboFontStyle_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || ( ( ComboBox )sender ).SelectedIndex == -1 )
				return;

			FormatStringData data = new FormatStringData();
			data.FontSyle = comboFontStyle.SelectedItem.ToString();

			UpdateFormats( data );
		}
		/// <summary>
		/// Changes font color.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboFontColors_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || ( ( ComboBox )sender ).SelectedIndex == -1 )
				return;

			FormatStringData data = new FormatStringData();
			data.FontColor = comboFontColors.SelectedItem.ToString();

			UpdateFormats( data );
		}
		/// <summary>
		/// Changes background color.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBackgroundColor_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || ( ( ComboBox )sender ).SelectedIndex == -1 )
				return;

			FormatStringData data = new FormatStringData();
			data.BackgroundColor = comboBackgroundColor.SelectedItem.ToString();

			UpdateFormats( data );
		}
		/// <summary>
		/// Changes borders color.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBorderColor_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || ( ( ComboBox )sender ).SelectedIndex == -1 )
				return;

			FormatStringData data = new FormatStringData();
			data.BorderColor = comboBorderColor.SelectedItem.ToString();

			UpdateFormats( data );
		}
		/// <summary>
		/// Changes background fill color.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBackgroundFillStyle_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || ( ( ComboBox )sender ).SelectedIndex == -1 )
				return;

			FormatStringData data = new FormatStringData();
			data.BackgroundStyle = comboBackgroundFillStyle.SelectedItem.ToString();

			UpdateFormats( data );
		}
		/// <summary>
		/// Changes underline weight.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboUnderlineWeight_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || ( ( ComboBox )sender ).SelectedIndex == -1 )
				return;

			FormatStringData data = new FormatStringData();
			data.UnderlineWeight = comboUnderlineWeight.SelectedItem.ToString();

			UpdateFormats( data );
		}
		/// <summary>
		/// Changes underline color.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboUnderlineColor_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || ( ( ComboBox )sender ).SelectedIndex == -1 )
				return;

			FormatStringData data = new FormatStringData();
			data.UnderlineColor = comboUnderlineColor.SelectedItem.ToString();

			UpdateFormats( data );
		}
		/// <summary>
		/// Changes underline style.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboUnderlineStyle_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || ( ( ComboBox )sender ).SelectedIndex == -1 )
				return;

			FormatStringData data = new FormatStringData();
			data.UnderlineStyle = comboUnderlineStyle.SelectedItem.ToString();

			UpdateFormats( data );
		}
		/// <summary>
		/// Updates strikeout color.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboStrikeOutColor_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( isLocked ) return;

			if( Formats == null || ( ( ComboBox )sender ).SelectedIndex == -1 )
				return;

			FormatStringData data = new FormatStringData();
			data.StrikeOutColor = comboStrikeOutColor.SelectedItem.ToString();

			UpdateFormats( data );
		}
		#endregion
	}
}