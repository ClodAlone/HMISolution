#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Edit.Dialogs.Options
{
/// <summary>
/// Form for managing context menu EditControl options.
/// </summary>
[ToolboxItem( false )]
public class ControlOptions : System.Windows.Forms.Form
{
#region Class Members
/// <summary>
/// Underlying EditControl.
/// </summary>
private EditControl m_control;
private System.Windows.Forms.Panel panel2;
private Syncfusion.Windows.Forms.Edit.Dialogs.Options.AppearanceControlOptions appearanceControlOptions;
private Syncfusion.Windows.Forms.Edit.Dialogs.Options.AppearanceTextOptions appearanceTextOptions;
private Syncfusion.Windows.Forms.Edit.Dialogs.Options.BehaviourControlOptions behaviourControlOptions;
private Syncfusion.Windows.Forms.Edit.Dialogs.Options.BehaviourTabsOptions behaviourTabsOptions;
private Syncfusion.Windows.Forms.Edit.Dialogs.Options.AppearanceAreasOptions appearanceAreasOptions;
private System.Windows.Forms.Button btnOK;
private System.Windows.Forms.Button btnCancel;
private System.Windows.Forms.TreeView treeOptions;
private System.Windows.Forms.Panel pnlButtons;
private System.Windows.Forms.Panel pnlMegaline;

/// <summary>
/// List of controls with options.
/// </summary>
private IList m_arrOptionsControls = new ArrayList();
#endregion

#region Class Initialization & Finalization
/// <summary>
/// Initializes a new instance of the ControlOptions class.
/// </summary>
/// <param name="control">Underlying EditControl.</param>
public ControlOptions( EditControl control )
{
// Required for Windows Form Designer support
InitializeComponent();

if ( control == null ) throw new ArgumentNullException( "control" );

m_control = control;

m_arrOptionsControls.Add( appearanceAreasOptions );
m_arrOptionsControls.Add( appearanceTextOptions );
m_arrOptionsControls.Add( appearanceControlOptions );
m_arrOptionsControls.Add( behaviourControlOptions );
m_arrOptionsControls.Add( behaviourTabsOptions );
}

private System.ComponentModel.IContainer components;

private System.Windows.Forms.ImageList imgTreeImages;

#region Windows Form Designer generated code

/// <summary>
/// Required method for Designer support - do not modify
/// the contents of this method with the code editor.
/// </summary>
private void InitializeComponent()
{
this.components = new System.ComponentModel.Container();
System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( ControlOptions ) );
this.imgTreeImages = new System.Windows.Forms.ImageList( this.components );
this.treeOptions = new System.Windows.Forms.TreeView();
this.pnlButtons = new System.Windows.Forms.Panel();
this.btnCancel = new System.Windows.Forms.Button();
this.btnOK = new System.Windows.Forms.Button();
this.panel2 = new System.Windows.Forms.Panel();
this.pnlMegaline = new System.Windows.Forms.Panel();
this.appearanceAreasOptions = new Syncfusion.Windows.Forms.Edit.Dialogs.Options.AppearanceAreasOptions();
this.behaviourTabsOptions = new Syncfusion.Windows.Forms.Edit.Dialogs.Options.BehaviourTabsOptions();
this.behaviourControlOptions = new Syncfusion.Windows.Forms.Edit.Dialogs.Options.BehaviourControlOptions();
this.appearanceTextOptions = new Syncfusion.Windows.Forms.Edit.Dialogs.Options.AppearanceTextOptions();
this.appearanceControlOptions = new Syncfusion.Windows.Forms.Edit.Dialogs.Options.AppearanceControlOptions();
this.pnlButtons.SuspendLayout();
this.panel2.SuspendLayout();
this.SuspendLayout();
// 
// imgTreeImages
// 
this.imgTreeImages.ImageSize = ( ( System.Drawing.Size )( resources.GetObject( "imgTreeImages.ImageSize" ) ) );
this.imgTreeImages.ImageStream = ( ( System.Windows.Forms.ImageListStreamer )( resources.GetObject( "imgTreeImages.ImageStream" ) ) );
this.imgTreeImages.TransparentColor = System.Drawing.Color.Transparent;
// 
// treeOptions
// 
this.treeOptions.AccessibleDescription = resources.GetString( "treeOptions.AccessibleDescription" );
this.treeOptions.AccessibleName = resources.GetString( "treeOptions.AccessibleName" );
this.treeOptions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "treeOptions.Anchor" ) ) );
this.treeOptions.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "treeOptions.BackgroundImage" ) ) );
this.treeOptions.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "treeOptions.Dock" ) ) );
this.treeOptions.Enabled = ( ( bool )( resources.GetObject( "treeOptions.Enabled" ) ) );
this.treeOptions.Font = ( ( System.Drawing.Font )( resources.GetObject( "treeOptions.Font" ) ) );
this.treeOptions.ImageIndex = ( ( int )( resources.GetObject( "treeOptions.ImageIndex" ) ) );
this.treeOptions.ImageList = this.imgTreeImages;
this.treeOptions.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "treeOptions.ImeMode" ) ) );
this.treeOptions.Indent = ( ( int )( resources.GetObject( "treeOptions.Indent" ) ) );
this.treeOptions.ItemHeight = ( ( int )( resources.GetObject( "treeOptions.ItemHeight" ) ) );
this.treeOptions.Location = ( ( System.Drawing.Point )( resources.GetObject( "treeOptions.Location" ) ) );
this.treeOptions.Name = "treeOptions";
this.treeOptions.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "treeOptions.RightToLeft" ) ) );
this.treeOptions.RightToLeftLayout = true;
this.treeOptions.SelectedImageIndex = ( ( int )( resources.GetObject( "treeOptions.SelectedImageIndex" ) ) );
this.treeOptions.Size = ( ( System.Drawing.Size )( resources.GetObject( "treeOptions.Size" ) ) );
this.treeOptions.TabIndex = ( ( int )( resources.GetObject( "treeOptions.TabIndex" ) ) );
this.treeOptions.Text = resources.GetString( "treeOptions.Text" );
this.treeOptions.Visible = ( ( bool )( resources.GetObject( "treeOptions.Visible" ) ) );
this.treeOptions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.TreeOptions_AfterSelect );
// 
// pnlButtons
// 
this.pnlButtons.AccessibleDescription = resources.GetString( "pnlButtons.AccessibleDescription" );
this.pnlButtons.AccessibleName = resources.GetString( "pnlButtons.AccessibleName" );
this.pnlButtons.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "pnlButtons.Anchor" ) ) );
this.pnlButtons.AutoScroll = ( ( bool )( resources.GetObject( "pnlButtons.AutoScroll" ) ) );
this.pnlButtons.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "pnlButtons.AutoScrollMargin" ) ) );
this.pnlButtons.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "pnlButtons.AutoScrollMinSize" ) ) );
this.pnlButtons.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "pnlButtons.BackgroundImage" ) ) );
this.pnlButtons.Controls.Add( this.btnCancel );
this.pnlButtons.Controls.Add( this.btnOK );
this.pnlButtons.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "pnlButtons.Dock" ) ) );
this.pnlButtons.Enabled = ( ( bool )( resources.GetObject( "pnlButtons.Enabled" ) ) );
this.pnlButtons.Font = ( ( System.Drawing.Font )( resources.GetObject( "pnlButtons.Font" ) ) );
this.pnlButtons.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "pnlButtons.ImeMode" ) ) );
this.pnlButtons.Location = ( ( System.Drawing.Point )( resources.GetObject( "pnlButtons.Location" ) ) );
this.pnlButtons.Name = "pnlButtons";
this.pnlButtons.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "pnlButtons.RightToLeft" ) ) );
this.pnlButtons.Size = ( ( System.Drawing.Size )( resources.GetObject( "pnlButtons.Size" ) ) );
this.pnlButtons.TabIndex = ( ( int )( resources.GetObject( "pnlButtons.TabIndex" ) ) );
this.pnlButtons.Text = resources.GetString( "pnlButtons.Text" );
this.pnlButtons.Visible = ( ( bool )( resources.GetObject( "pnlButtons.Visible" ) ) );
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
// btnOK
// 
this.btnOK.AccessibleDescription = resources.GetString( "btnOK.AccessibleDescription" );
this.btnOK.AccessibleName = resources.GetString( "btnOK.AccessibleName" );
this.btnOK.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnOK.Anchor" ) ) );
this.btnOK.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnOK.BackgroundImage" ) ) );
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
this.btnOK.Click += new System.EventHandler( this.BtnOK_Click );
// 
// panel2
// 
this.panel2.AccessibleDescription = resources.GetString( "panel2.AccessibleDescription" );
this.panel2.AccessibleName = resources.GetString( "panel2.AccessibleName" );
this.panel2.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "panel2.Anchor" ) ) );
this.panel2.AutoScroll = ( ( bool )( resources.GetObject( "panel2.AutoScroll" ) ) );
this.panel2.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "panel2.AutoScrollMargin" ) ) );
this.panel2.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "panel2.AutoScrollMinSize" ) ) );
this.panel2.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "panel2.BackgroundImage" ) ) );
this.panel2.Controls.Add( this.pnlMegaline );
this.panel2.Controls.Add( this.appearanceAreasOptions );
this.panel2.Controls.Add( this.behaviourTabsOptions );
this.panel2.Controls.Add( this.behaviourControlOptions );
this.panel2.Controls.Add( this.appearanceTextOptions );
this.panel2.Controls.Add( this.appearanceControlOptions );
this.panel2.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "panel2.Dock" ) ) );
this.panel2.Enabled = ( ( bool )( resources.GetObject( "panel2.Enabled" ) ) );
this.panel2.Font = ( ( System.Drawing.Font )( resources.GetObject( "panel2.Font" ) ) );
this.panel2.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "panel2.ImeMode" ) ) );
this.panel2.Location = ( ( System.Drawing.Point )( resources.GetObject( "panel2.Location" ) ) );
this.panel2.Name = "panel2";
this.panel2.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "panel2.RightToLeft" ) ) );
this.panel2.Size = ( ( System.Drawing.Size )( resources.GetObject( "panel2.Size" ) ) );
this.panel2.TabIndex = ( ( int )( resources.GetObject( "panel2.TabIndex" ) ) );
this.panel2.Text = resources.GetString( "panel2.Text" );
this.panel2.Visible = ( ( bool )( resources.GetObject( "panel2.Visible" ) ) );
// 
// pnlMegaline
// 
this.pnlMegaline.AccessibleDescription = resources.GetString( "pnlMegaline.AccessibleDescription" );
this.pnlMegaline.AccessibleName = resources.GetString( "pnlMegaline.AccessibleName" );
this.pnlMegaline.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "pnlMegaline.Anchor" ) ) );
this.pnlMegaline.AutoScroll = ( ( bool )( resources.GetObject( "pnlMegaline.AutoScroll" ) ) );
this.pnlMegaline.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "pnlMegaline.AutoScrollMargin" ) ) );
this.pnlMegaline.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "pnlMegaline.AutoScrollMinSize" ) ) );
this.pnlMegaline.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "pnlMegaline.BackgroundImage" ) ) );
this.pnlMegaline.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "pnlMegaline.Dock" ) ) );
this.pnlMegaline.Enabled = ( ( bool )( resources.GetObject( "pnlMegaline.Enabled" ) ) );
this.pnlMegaline.Font = ( ( System.Drawing.Font )( resources.GetObject( "pnlMegaline.Font" ) ) );
this.pnlMegaline.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "pnlMegaline.ImeMode" ) ) );
this.pnlMegaline.Location = ( ( System.Drawing.Point )( resources.GetObject( "pnlMegaline.Location" ) ) );
this.pnlMegaline.Name = "pnlMegaline";
this.pnlMegaline.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "pnlMegaline.RightToLeft" ) ) );
this.pnlMegaline.Size = ( ( System.Drawing.Size )( resources.GetObject( "pnlMegaline.Size" ) ) );
this.pnlMegaline.TabIndex = ( ( int )( resources.GetObject( "pnlMegaline.TabIndex" ) ) );
this.pnlMegaline.Text = resources.GetString( "pnlMegaline.Text" );
this.pnlMegaline.Visible = ( ( bool )( resources.GetObject( "pnlMegaline.Visible" ) ) );
this.pnlMegaline.Paint += new System.Windows.Forms.PaintEventHandler( this.PnlButtons_Paint );
// 
// appearanceAreasOptions
// 
this.appearanceAreasOptions.AccessibleDescription = resources.GetString( "appearanceAreasOptions.AccessibleDescription" );
this.appearanceAreasOptions.AccessibleName = resources.GetString( "appearanceAreasOptions.AccessibleName" );
this.appearanceAreasOptions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "appearanceAreasOptions.Anchor" ) ) );
this.appearanceAreasOptions.AutoScroll = ( ( bool )( resources.GetObject( "appearanceAreasOptions.AutoScroll" ) ) );
this.appearanceAreasOptions.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "appearanceAreasOptions.AutoScrollMargin" ) ) );
this.appearanceAreasOptions.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "appearanceAreasOptions.AutoScrollMinSize" ) ) );
this.appearanceAreasOptions.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "appearanceAreasOptions.BackgroundImage" ) ) );
this.appearanceAreasOptions.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "appearanceAreasOptions.Dock" ) ) );
this.appearanceAreasOptions.Enabled = ( ( bool )( resources.GetObject( "appearanceAreasOptions.Enabled" ) ) );
this.appearanceAreasOptions.Font = ( ( System.Drawing.Font )( resources.GetObject( "appearanceAreasOptions.Font" ) ) );
this.appearanceAreasOptions.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "appearanceAreasOptions.ImeMode" ) ) );
this.appearanceAreasOptions.Location = ( ( System.Drawing.Point )( resources.GetObject( "appearanceAreasOptions.Location" ) ) );
this.appearanceAreasOptions.Name = "appearanceAreasOptions";
this.appearanceAreasOptions.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "appearanceAreasOptions.RightToLeft" ) ) );
this.appearanceAreasOptions.Size = ( ( System.Drawing.Size )( resources.GetObject( "appearanceAreasOptions.Size" ) ) );
this.appearanceAreasOptions.TabIndex = ( ( int )( resources.GetObject( "appearanceAreasOptions.TabIndex" ) ) );
this.appearanceAreasOptions.Visible = ( ( bool )( resources.GetObject( "appearanceAreasOptions.Visible" ) ) );
// 
// behaviourTabsOptions
// 
this.behaviourTabsOptions.AccessibleDescription = resources.GetString( "behaviourTabsOptions.AccessibleDescription" );
this.behaviourTabsOptions.AccessibleName = resources.GetString( "behaviourTabsOptions.AccessibleName" );
this.behaviourTabsOptions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "behaviourTabsOptions.Anchor" ) ) );
this.behaviourTabsOptions.AutoScroll = ( ( bool )( resources.GetObject( "behaviourTabsOptions.AutoScroll" ) ) );
this.behaviourTabsOptions.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "behaviourTabsOptions.AutoScrollMargin" ) ) );
this.behaviourTabsOptions.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "behaviourTabsOptions.AutoScrollMinSize" ) ) );
this.behaviourTabsOptions.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "behaviourTabsOptions.BackgroundImage" ) ) );
this.behaviourTabsOptions.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "behaviourTabsOptions.Dock" ) ) );
this.behaviourTabsOptions.Enabled = ( ( bool )( resources.GetObject( "behaviourTabsOptions.Enabled" ) ) );
this.behaviourTabsOptions.Font = ( ( System.Drawing.Font )( resources.GetObject( "behaviourTabsOptions.Font" ) ) );
this.behaviourTabsOptions.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "behaviourTabsOptions.ImeMode" ) ) );
this.behaviourTabsOptions.Location = ( ( System.Drawing.Point )( resources.GetObject( "behaviourTabsOptions.Location" ) ) );
this.behaviourTabsOptions.Name = "behaviourTabsOptions";
this.behaviourTabsOptions.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "behaviourTabsOptions.RightToLeft" ) ) );
this.behaviourTabsOptions.Size = ( ( System.Drawing.Size )( resources.GetObject( "behaviourTabsOptions.Size" ) ) );
this.behaviourTabsOptions.TabIndex = ( ( int )( resources.GetObject( "behaviourTabsOptions.TabIndex" ) ) );
this.behaviourTabsOptions.Visible = ( ( bool )( resources.GetObject( "behaviourTabsOptions.Visible" ) ) );
// 
// behaviourControlOptions
// 
this.behaviourControlOptions.AccessibleDescription = resources.GetString( "behaviourControlOptions.AccessibleDescription" );
this.behaviourControlOptions.AccessibleName = resources.GetString( "behaviourControlOptions.AccessibleName" );
this.behaviourControlOptions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "behaviourControlOptions.Anchor" ) ) );
this.behaviourControlOptions.AutoScroll = ( ( bool )( resources.GetObject( "behaviourControlOptions.AutoScroll" ) ) );
this.behaviourControlOptions.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "behaviourControlOptions.AutoScrollMargin" ) ) );
this.behaviourControlOptions.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "behaviourControlOptions.AutoScrollMinSize" ) ) );
this.behaviourControlOptions.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "behaviourControlOptions.BackgroundImage" ) ) );
this.behaviourControlOptions.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "behaviourControlOptions.Dock" ) ) );
this.behaviourControlOptions.Enabled = ( ( bool )( resources.GetObject( "behaviourControlOptions.Enabled" ) ) );
this.behaviourControlOptions.Font = ( ( System.Drawing.Font )( resources.GetObject( "behaviourControlOptions.Font" ) ) );
this.behaviourControlOptions.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "behaviourControlOptions.ImeMode" ) ) );
this.behaviourControlOptions.Location = ( ( System.Drawing.Point )( resources.GetObject( "behaviourControlOptions.Location" ) ) );
this.behaviourControlOptions.Name = "behaviourControlOptions";
this.behaviourControlOptions.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "behaviourControlOptions.RightToLeft" ) ) );
this.behaviourControlOptions.Size = ( ( System.Drawing.Size )( resources.GetObject( "behaviourControlOptions.Size" ) ) );
this.behaviourControlOptions.TabIndex = ( ( int )( resources.GetObject( "behaviourControlOptions.TabIndex" ) ) );
this.behaviourControlOptions.Visible = ( ( bool )( resources.GetObject( "behaviourControlOptions.Visible" ) ) );
// 
// appearanceTextOptions
// 
this.appearanceTextOptions.AccessibleDescription = resources.GetString( "appearanceTextOptions.AccessibleDescription" );
this.appearanceTextOptions.AccessibleName = resources.GetString( "appearanceTextOptions.AccessibleName" );
this.appearanceTextOptions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "appearanceTextOptions.Anchor" ) ) );
this.appearanceTextOptions.AutoScroll = ( ( bool )( resources.GetObject( "appearanceTextOptions.AutoScroll" ) ) );
this.appearanceTextOptions.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "appearanceTextOptions.AutoScrollMargin" ) ) );
this.appearanceTextOptions.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "appearanceTextOptions.AutoScrollMinSize" ) ) );
this.appearanceTextOptions.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "appearanceTextOptions.BackgroundImage" ) ) );
this.appearanceTextOptions.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "appearanceTextOptions.Dock" ) ) );
this.appearanceTextOptions.Enabled = ( ( bool )( resources.GetObject( "appearanceTextOptions.Enabled" ) ) );
this.appearanceTextOptions.Font = ( ( System.Drawing.Font )( resources.GetObject( "appearanceTextOptions.Font" ) ) );
this.appearanceTextOptions.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "appearanceTextOptions.ImeMode" ) ) );
this.appearanceTextOptions.Location = ( ( System.Drawing.Point )( resources.GetObject( "appearanceTextOptions.Location" ) ) );
this.appearanceTextOptions.Name = "appearanceTextOptions";
this.appearanceTextOptions.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "appearanceTextOptions.RightToLeft" ) ) );
this.appearanceTextOptions.Size = ( ( System.Drawing.Size )( resources.GetObject( "appearanceTextOptions.Size" ) ) );
this.appearanceTextOptions.TabIndex = ( ( int )( resources.GetObject( "appearanceTextOptions.TabIndex" ) ) );
this.appearanceTextOptions.Visible = ( ( bool )( resources.GetObject( "appearanceTextOptions.Visible" ) ) );
// 
// appearanceControlOptions
// 
this.appearanceControlOptions.AccessibleDescription = resources.GetString( "appearanceControlOptions.AccessibleDescription" );
this.appearanceControlOptions.AccessibleName = resources.GetString( "appearanceControlOptions.AccessibleName" );
this.appearanceControlOptions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "appearanceControlOptions.Anchor" ) ) );
this.appearanceControlOptions.AutoScroll = ( ( bool )( resources.GetObject( "appearanceControlOptions.AutoScroll" ) ) );
this.appearanceControlOptions.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "appearanceControlOptions.AutoScrollMargin" ) ) );
this.appearanceControlOptions.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "appearanceControlOptions.AutoScrollMinSize" ) ) );
this.appearanceControlOptions.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "appearanceControlOptions.BackgroundImage" ) ) );
this.appearanceControlOptions.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "appearanceControlOptions.Dock" ) ) );
this.appearanceControlOptions.Enabled = ( ( bool )( resources.GetObject( "appearanceControlOptions.Enabled" ) ) );
this.appearanceControlOptions.Font = ( ( System.Drawing.Font )( resources.GetObject( "appearanceControlOptions.Font" ) ) );
this.appearanceControlOptions.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "appearanceControlOptions.ImeMode" ) ) );
this.appearanceControlOptions.Location = ( ( System.Drawing.Point )( resources.GetObject( "appearanceControlOptions.Location" ) ) );
this.appearanceControlOptions.Name = "appearanceControlOptions";
this.appearanceControlOptions.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "appearanceControlOptions.RightToLeft" ) ) );
this.appearanceControlOptions.Size = ( ( System.Drawing.Size )( resources.GetObject( "appearanceControlOptions.Size" ) ) );
this.appearanceControlOptions.TabIndex = ( ( int )( resources.GetObject( "appearanceControlOptions.TabIndex" ) ) );
this.appearanceControlOptions.Visible = ( ( bool )( resources.GetObject( "appearanceControlOptions.Visible" ) ) );
// 
// ControlOptions
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
this.Controls.Add( this.treeOptions );
this.Controls.Add( this.panel2 );
this.Controls.Add( this.pnlButtons );
this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
this.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "$this.Icon" ) ) );
this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
this.MaximizeBox = false;
this.MaximumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MaximumSize" ) ) );
this.MinimizeBox = false;
this.MinimumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MinimumSize" ) ) );
this.Name = "ControlOptions";
this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
this.RightToLeftLayout = true;
this.ShowInTaskbar = false;
this.StartPosition = ( ( System.Windows.Forms.FormStartPosition )( resources.GetObject( "$this.StartPosition" ) ) );
this.Text = resources.GetString( "$this.Text" );
this.VisibleChanged += new System.EventHandler( this.ControlOptions_VisibleChanged );
this.pnlButtons.ResumeLayout( false );
this.panel2.ResumeLayout( false );
this.ResumeLayout( false );

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

#endregion

#region Class Event Handlers
/// <summary>
/// Looks through options controls and applies settings.
/// </summary>
/// <param name="sender">The sender</param>
/// <param name="e">The event argument</param>
private void BtnOK_Click( object sender, System.EventArgs e )
{
Control errorControl = null;
IOptionsControl errorOpts = null;
foreach ( IOptionsControl optionsControl in m_arrOptionsControls )
{
errorControl = optionsControl.Apply( m_control );
if ( errorControl != null )
{
errorOpts = optionsControl;
break;
}
}

if ( errorControl != null )
{
ShowOptions( errorOpts );
errorControl.Focus();
}
else
{
Close();
}
}

/// <summary>
/// Looks through options controls and inits settings.
/// </summary>
/// <param name="sender">The sender</param>
/// <param name="e">The event argument</param>
private void ControlOptions_VisibleChanged( object sender, System.EventArgs e )
{
if ( this.Visible )
{
foreach ( IOptionsControl optionsControl in m_arrOptionsControls )
{
optionsControl.Init( m_control );
}

TreeNode[] arrAppearanceNodes = new TreeNode[ 3 ];
arrAppearanceNodes[ 0 ] = new TreeNode( Localizer.GetString(Localizer.DEF_OPTS_APPEARANCEAREAS), 0, 1 );
arrAppearanceNodes[ 1 ] = new TreeNode( Localizer.GetString(Localizer.DEF_OPTS_APPEARANCETEXT), 0, 1 );
arrAppearanceNodes[ 2 ] = new TreeNode( Localizer.GetString(Localizer.DEF_OPTS_APPEARANCECONTROL), 0, 1 );

TreeNode appearanceNode = new TreeNode( Localizer.GetString(Localizer.DEF_OPTS_APPEARANCE), 0, 1, arrAppearanceNodes );

TreeNode[] arrBehaviourNodes = new TreeNode[ 2 ];
    
arrBehaviourNodes[ 0 ] = new TreeNode("General", 0, 1 );
arrBehaviourNodes[ 1 ] = new TreeNode( "Tabs", 0, 1 );

TreeNode behaviourNode = new TreeNode( "Behaviuour", 0, 1, arrBehaviourNodes );

treeOptions.Nodes.Add( appearanceNode );
treeOptions.Nodes.Add( behaviourNode );

treeOptions.ExpandAll();
}
else
{
treeOptions.Nodes.Clear();
}
}

/// <summary>
/// Makes corresponding options control visible.
/// </summary>
/// <param name="sender">The sender</param>
/// <param name="e">The event argument</param>
private void TreeOptions_AfterSelect( object sender, System.Windows.Forms.TreeViewEventArgs e )
{
string strNode = e.Node.Text;

if ( strNode == Localizer.GetString(Localizer.DEF_OPTS_APPEARANCE) || strNode == Localizer.GetString(Localizer.DEF_OPTS_APPEARANCEAREAS) )
{
ShowOptions( appearanceAreasOptions );
}
else if ( strNode == Localizer.GetString(Localizer.DEF_OPTS_APPEARANCETEXT) )
{
ShowOptions( appearanceTextOptions );
}
else if ( strNode == Localizer.GetString(Localizer.DEF_OPTS_APPEARANCECONTROL) )
{
ShowOptions( appearanceControlOptions );
}
else if ( strNode == "General"|| strNode == Localizer.GetString(Localizer.DEF_OPTS_BEHAVIOUR) )
{
ShowOptions( behaviourControlOptions );
}
else if ( strNode == "Tabs" )
{
ShowOptions( behaviourTabsOptions );
}
}

/// <summary>
/// /// Control
/// </summary>
/// <param name="control">The control</param>
private void ShowOptions( IOptionsControl control )
{
foreach ( Control optionsControl in m_arrOptionsControls )
{
optionsControl.Visible = false;
}

( ( Control )control ).Visible = true;
}

/// <summary>
/// Draws 3d line above panel.
/// </summary>
/// <param name="sender">The sender</param>
/// <param name="e">The event argument</param>
private void PnlButtons_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
{
GraphicsUtils.Draw3DBorder( e.Graphics, new Rectangle( 5, 0, pnlMegaline.Width - 11, 1 ) );
}
#endregion
}
}