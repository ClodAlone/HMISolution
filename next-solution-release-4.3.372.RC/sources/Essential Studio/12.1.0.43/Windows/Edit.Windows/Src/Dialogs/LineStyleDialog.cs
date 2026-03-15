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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Forms.Popup
{
	/// <summary>
	/// Dialog for line properties selecting.
	/// </summary>
	public class LineStyleDialog : System.Windows.Forms.Form
	{
		#region Constants
		/// <summary>
		/// Length of sample lines.
		/// </summary>
		const int DEF_LINES_LENGTH = 90;
		/// <summary>
		/// Horizontal offset of sample lines.
		/// </summary>
		const int DEF_LINES_HOR_OFFSET = 15;
		/// <summary>
		/// Vertical offset of sample lines.
		/// </summary>
		const int DEF_LINES_VER_OFFSET = 6;
		#endregion

		#region Fields
		/// <summary>
		/// Line weight.
		/// </summary>
		private BorderWeight m_weight = BorderWeight.Thin;
		/// <summary>
		/// Line style.
		/// </summary>
		private FrameBorderStyle m_style = FrameBorderStyle.Solid;
		/// <summary>
		/// Line color.
		/// </summary>
		private Color m_color = Color.Empty;
		#endregion

		#region Controls
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupWeight;
		private System.Windows.Forms.RadioButton radioThick;
		private System.Windows.Forms.RadioButton radioBold;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.Button btnColor;
		private System.Windows.Forms.GroupBox groupStyle;
		private System.Windows.Forms.RadioButton radioSolid;
		private System.Windows.Forms.RadioButton radioDashDot;
		private System.Windows.Forms.RadioButton radioDot;
		private System.Windows.Forms.RadioButton radioDash;
		private System.Windows.Forms.RadioButton radioWave;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.RadioButton radioDouble;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets line weight.
		/// </summary>
		public BorderWeight Weight
		{
			get
			{
				return m_weight;
			}
			set
			{
				m_weight = value;
			}
		}
		/// <summary>
		/// Gets or sets line style.
		/// </summary>
		public FrameBorderStyle Style
		{
			get
			{
				return m_style;
			}
			set
			{
				m_style = value;
			}
		}
		/// <summary>
		/// Gets or sets line color.
		/// </summary>
		public Color Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates new instance of class.
		/// </summary>
		public LineStyleDialog( Color color )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			m_color = color;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupWeight = new System.Windows.Forms.GroupBox();
			this.radioDouble = new System.Windows.Forms.RadioButton();
			this.radioBold = new System.Windows.Forms.RadioButton();
			this.radioThick = new System.Windows.Forms.RadioButton();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.btnColor = new System.Windows.Forms.Button();
			this.groupStyle = new System.Windows.Forms.GroupBox();
			this.radioWave = new System.Windows.Forms.RadioButton();
			this.radioDash = new System.Windows.Forms.RadioButton();
			this.radioDot = new System.Windows.Forms.RadioButton();
			this.radioDashDot = new System.Windows.Forms.RadioButton();
			this.radioSolid = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupWeight.SuspendLayout();
			this.groupStyle.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupWeight
			// 
			this.groupWeight.Controls.Add( this.radioDouble );
			this.groupWeight.Controls.Add( this.radioBold );
			this.groupWeight.Controls.Add( this.radioThick );
			this.groupWeight.Location = new System.Drawing.Point( 8, 16 );
			this.groupWeight.Name = "groupWeight";
			this.groupWeight.Size = new System.Drawing.Size( 144, 96 );
			this.groupWeight.TabIndex = 0;
			this.groupWeight.TabStop = false;
			this.groupWeight.Text = "Weight";
			// 
			// radioDouble
			// 
			this.radioDouble.Location = new System.Drawing.Point( 16, 72 );
			this.radioDouble.Name = "radioDouble";
			this.radioDouble.Size = new System.Drawing.Size( 120, 16 );
			this.radioDouble.TabIndex = 2;
			this.radioDouble.Paint += new System.Windows.Forms.PaintEventHandler( this.radioDouble_Paint );
			this.radioDouble.CheckedChanged += new System.EventHandler( this.weight_CheckedChanged );
			// 
			// radioBold
			// 
			this.radioBold.Location = new System.Drawing.Point( 16, 48 );
			this.radioBold.Name = "radioBold";
			this.radioBold.Size = new System.Drawing.Size( 120, 16 );
			this.radioBold.TabIndex = 1;
			this.radioBold.Paint += new System.Windows.Forms.PaintEventHandler( this.radioBold_Paint );
			this.radioBold.CheckedChanged += new System.EventHandler( this.weight_CheckedChanged );
			// 
			// radioThick
			// 
			this.radioThick.Checked = true;
			this.radioThick.Location = new System.Drawing.Point( 16, 24 );
			this.radioThick.Name = "radioThick";
			this.radioThick.Size = new System.Drawing.Size( 120, 16 );
			this.radioThick.TabIndex = 0;
			this.radioThick.TabStop = true;
			this.radioThick.Paint += new System.Windows.Forms.PaintEventHandler( this.radioThick_Paint );
			this.radioThick.CheckedChanged += new System.EventHandler( this.weight_CheckedChanged );
			// 
			// colorDialog
			// 
			this.colorDialog.Color = System.Drawing.Color.Blue;
			// 
			// btnColor
			// 
			this.btnColor.Location = new System.Drawing.Point( 168, 48 );
			this.btnColor.Name = "btnColor";
			this.btnColor.Size = new System.Drawing.Size( 80, 23 );
			this.btnColor.TabIndex = 1;
			this.btnColor.Text = "Color...";
			this.btnColor.Click += new System.EventHandler( this.btnColor_Click );
			// 
			// groupStyle
			// 
			this.groupStyle.Controls.Add( this.radioWave );
			this.groupStyle.Controls.Add( this.radioDash );
			this.groupStyle.Controls.Add( this.radioDot );
			this.groupStyle.Controls.Add( this.radioDashDot );
			this.groupStyle.Controls.Add( this.radioSolid );
			this.groupStyle.Location = new System.Drawing.Point( 8, 120 );
			this.groupStyle.Name = "groupStyle";
			this.groupStyle.Size = new System.Drawing.Size( 144, 144 );
			this.groupStyle.TabIndex = 2;
			this.groupStyle.TabStop = false;
			this.groupStyle.Text = "Style";
			// 
			// radioWave
			// 
			this.radioWave.Location = new System.Drawing.Point( 16, 120 );
			this.radioWave.Name = "radioWave";
			this.radioWave.Size = new System.Drawing.Size( 120, 16 );
			this.radioWave.TabIndex = 4;
			this.radioWave.Paint += new System.Windows.Forms.PaintEventHandler( this.radioWave_Paint );
			this.radioWave.CheckedChanged += new System.EventHandler( this.style_CheckedChanged );
			// 
			// radioDash
			// 
			this.radioDash.Location = new System.Drawing.Point( 16, 96 );
			this.radioDash.Name = "radioDash";
			this.radioDash.Size = new System.Drawing.Size( 120, 16 );
			this.radioDash.TabIndex = 3;
			this.radioDash.Paint += new System.Windows.Forms.PaintEventHandler( this.radioDash_Paint );
			this.radioDash.CheckedChanged += new System.EventHandler( this.style_CheckedChanged );
			// 
			// radioDot
			// 
			this.radioDot.Location = new System.Drawing.Point( 16, 72 );
			this.radioDot.Name = "radioDot";
			this.radioDot.Size = new System.Drawing.Size( 120, 16 );
			this.radioDot.TabIndex = 2;
			this.radioDot.Paint += new System.Windows.Forms.PaintEventHandler( this.radioDot_Paint );
			this.radioDot.CheckedChanged += new System.EventHandler( this.style_CheckedChanged );
			// 
			// radioDashDot
			// 
			this.radioDashDot.Location = new System.Drawing.Point( 16, 48 );
			this.radioDashDot.Name = "radioDashDot";
			this.radioDashDot.Size = new System.Drawing.Size( 120, 16 );
			this.radioDashDot.TabIndex = 1;
			this.radioDashDot.Paint += new System.Windows.Forms.PaintEventHandler( this.radioDashDot_Paint );
			this.radioDashDot.CheckedChanged += new System.EventHandler( this.style_CheckedChanged );
			// 
			// radioSolid
			// 
			this.radioSolid.Checked = true;
			this.radioSolid.Location = new System.Drawing.Point( 16, 24 );
			this.radioSolid.Name = "radioSolid";
			this.radioSolid.Size = new System.Drawing.Size( 120, 16 );
			this.radioSolid.TabIndex = 0;
			this.radioSolid.TabStop = true;
			this.radioSolid.Paint += new System.Windows.Forms.PaintEventHandler( this.radioThick_Paint );
			this.radioSolid.CheckedChanged += new System.EventHandler( this.style_CheckedChanged );
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point( 168, 160 );
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size( 80, 24 );
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point( 168, 200 );
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size( 80, 24 );
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			// 
			// LineStyleDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size( 258, 269 );
			this.Controls.Add( this.btnCancel );
			this.Controls.Add( this.btnOK );
			this.Controls.Add( this.groupStyle );
			this.Controls.Add( this.btnColor );
			this.Controls.Add( this.groupWeight );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LineStyleDialog";
			this.RightToLeftLayout = true;
			this.ShowInTaskbar = false;
			this.Text = "Choose line properties";
			this.groupWeight.ResumeLayout( false );
			this.groupStyle.ResumeLayout( false );
			this.ResumeLayout( false );
		}
		#endregion

		#endregion

		#region Event Handlers
		/// <summary>
		/// Draws corresponding line.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioThick_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			DrawSampleLine( e.Graphics, 1, DashStyle.Solid );
		}
		/// <summary>
		/// Draws corresponding line,
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioBold_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			DrawSampleLine( e.Graphics, 2, DashStyle.Solid );
		}
		/// <summary>
		/// Draws corresponding line.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioDouble_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			Pen pen = new Pen( m_color );
			e.Graphics.DrawLine( pen, DEF_LINES_HOR_OFFSET, DEF_LINES_VER_OFFSET - 1, DEF_LINES_HOR_OFFSET + DEF_LINES_LENGTH, DEF_LINES_VER_OFFSET - 1 );
			e.Graphics.DrawLine( pen, DEF_LINES_HOR_OFFSET, DEF_LINES_VER_OFFSET + 1, DEF_LINES_HOR_OFFSET + DEF_LINES_LENGTH, DEF_LINES_VER_OFFSET + 1 );
			pen.Dispose();
		}
		/// <summary>
		/// Draws corresponding line.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioDashDot_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			DrawSampleLine( e.Graphics, 1, DashStyle.DashDot );
		}
		/// <summary>
		/// Draws corresponding line.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioDot_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			DrawSampleLine( e.Graphics, 1, DashStyle.Dot );
		}
		/// <summary>
		/// Draws corresponding line.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioDash_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			DrawSampleLine( e.Graphics, 1, DashStyle.Dash );
		}
		/// <summary>
		/// Draws corresponding line.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioWave_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			GraphicsUtils.DrawHorizontalWaveLine( e.Graphics, DEF_LINES_VER_OFFSET, DEF_LINES_HOR_OFFSET,
				DEF_LINES_HOR_OFFSET + DEF_LINES_LENGTH, m_color, false );
		}
		/// <summary>
		/// Invokes color dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnColor_Click( object sender, System.EventArgs e )
		{
			if( DialogResult.OK == colorDialog.ShowDialog() )
			{
				m_color = colorDialog.Color;
				Invalidate( true );
			}
		}
		/// <summary>
		/// Sets new value to weight member.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void weight_CheckedChanged( object sender, System.EventArgs e )
		{
			if( radioThick.Checked )
				m_weight = BorderWeight.Thin;
			else if( radioBold.Checked )
				m_weight = BorderWeight.Bold;
			else if( radioDouble.Checked )
				m_weight = BorderWeight.Double;
		}
		/// <summary>
		/// Sets new value to style member.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void style_CheckedChanged( object sender, System.EventArgs e )
		{
			if( radioDash.Checked )
				m_style = FrameBorderStyle.Dash;
			else if( radioDashDot.Checked )
				m_style = FrameBorderStyle.DashDot;
			else if( radioDot.Checked )
				m_style = FrameBorderStyle.Dot;
			else if( radioSolid.Checked )
				m_style = FrameBorderStyle.Solid;
			else if( radioWave.Checked )
				m_style = FrameBorderStyle.Wave;
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Draws sample line.
		/// </summary>
		/// <param name="g">Graphics object to draw at.</param>
		/// <param name="weight">Weight of line.</param>
		/// <param name="style">Dash style of line.</param>
		private void DrawSampleLine( Graphics g, int weight, DashStyle style )
		{
			Pen pen = new Pen( m_color, weight );
			pen.DashStyle = style;
			g.DrawLine( pen, DEF_LINES_HOR_OFFSET, DEF_LINES_VER_OFFSET,
				DEF_LINES_HOR_OFFSET + DEF_LINES_LENGTH, DEF_LINES_VER_OFFSET );
			pen.Dispose();
		}
		#endregion
	}
}