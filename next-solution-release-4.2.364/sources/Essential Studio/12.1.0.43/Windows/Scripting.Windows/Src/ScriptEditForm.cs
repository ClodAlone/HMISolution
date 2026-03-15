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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Syncfusion.Scripting.Design
{
  /// <summary>
  /// Summary description for ScriptEditForm.
  /// </summary>
  public class ScriptEditForm : Form, IScriptEditor
  {
    private ScriptEditControl scriptEditControl1;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private Container components = null;

    public ScriptEditControl ScriptEditControl
    {
      get
      {
        return this.scriptEditControl1;
      }
    }

    public ScriptEditForm()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();
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
          components.Dispose();
        }
      }
    }

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
		this.scriptEditControl1 = new Syncfusion.Scripting.Design.ScriptEditControl();
		this.SuspendLayout();
		// 
		// scriptEditControl1
		// 
		this.scriptEditControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.scriptEditControl1.EnableExternalCompile = true;
		this.scriptEditControl1.EnableExternalRun = true;
		this.scriptEditControl1.Location = new System.Drawing.Point(0, 0);
		this.scriptEditControl1.Name = "scriptEditControl1";
		this.scriptEditControl1.ScriptingManager = null;
		this.scriptEditControl1.Size = new System.Drawing.Size(944, 558);
		this.scriptEditControl1.TabIndex = 0;
		// 
		// ScriptEditForm
		// 
#if SyncfusionFramework2_0 
        this.AutoScaleDimensions = new System.Drawing.SizeF(5,13);
#else
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
#endif
		this.ClientSize = new System.Drawing.Size(944, 558);
		this.Controls.Add(this.scriptEditControl1);
		this.Name = "ScriptEditForm";
		this.Text = "ScriptEditForm";
		this.Closing += new System.ComponentModel.CancelEventHandler(this.ScriptEditControlForm_Closing);
		this.Activated += new System.EventHandler(this.ScriptEditControlForm_Activated);
		this.ResumeLayout(false);

	}
    #endregion

    private void ScriptEditControlForm_Closing( object sender, CancelEventArgs e )
    {
      if( this.scriptEditControl1.PendingSave == true )
      {
        if( MessageBox.Show( "Save changes to Script?", "Essential Suite Scripting", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
        {
          this.UpdateScript( this.scriptEditControl1.Script );
        }
      }
      this.DialogResult = DialogResult.OK;
    }

    private void ScriptEditControlForm_Activated( object sender, EventArgs e )
    {
      // Docking Windows UserControl workaround
      this.scriptEditControl1.Width += 1;
      this.scriptEditControl1.Width -= 1;
    }

    #region IScriptEditor implementation
    public void UpdateEditor( Script script )
    {
      this.scriptEditControl1.InitializeScriptEditor( script );
    }

    public void UpdateScript( Script script )
    {
      this.scriptEditControl1.UpdateScript( script );
    }

    public Form Form
    {
      get
      {
        return this;
      }
    }
    #endregion	// IScriptEditor

    protected internal void SetManager( ScriptingManager manager )
    {
      if( manager == null )
      {
        throw new ArgumentNullException( "manager" );
      }

      this.scriptEditControl1.ScriptingManager = manager;
    }
  }
}