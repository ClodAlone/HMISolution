#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Forms.Diagram
{
	partial class PageScaleDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( PageScaleDialog ) );
			Syncfusion.Windows.Forms.Diagram.PageScale pageScale1 = new Syncfusion.Windows.Forms.Diagram.PageScale();
			this.drawingScaleControl1 = new Syncfusion.Windows.Forms.Diagram.DrawingScaleControl();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// drawingScaleControl1
			// 
			resources.ApplyResources( this.drawingScaleControl1, "drawingScaleControl1" );
			this.drawingScaleControl1.Name = "drawingScaleControl1";
			pageScale1.DisplayName = "Custom";
			this.drawingScaleControl1.PageScale = pageScale1;
			// 
			// btnCancel
			// 
			resources.ApplyResources( this.btnCancel, "btnCancel" );
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			resources.ApplyResources( this.btnOk, "btnOk" );
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// PageScaleDialog
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources( this, "$this" );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add( this.btnOk );
			this.Controls.Add( this.btnCancel );
			this.Controls.Add( this.drawingScaleControl1 );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PageScaleDialog";
			this.ResumeLayout( false );

		}

		#endregion

		private Syncfusion.Windows.Forms.Diagram.DrawingScaleControl drawingScaleControl1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;

	}
}