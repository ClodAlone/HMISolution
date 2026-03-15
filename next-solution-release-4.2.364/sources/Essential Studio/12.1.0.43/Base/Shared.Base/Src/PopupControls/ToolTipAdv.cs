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
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools;
#endregion

namespace Syncfusion.Windows.Forms
{
  /// <summary>
  /// Summary description for ToolTipAdv.
  /// </summary>
  /// <remarks></remarks>
  /// Tooltip occurs if the TreeViewAdv control's width is shorter than the text length of some of the nodes and when the mouse pointer is hovered on
  /// top of the texts of these nodes, a filled rectangle is displayed.The name of the tooltipAdv depends upon the text of the TreeNodeAdv.
  /// By default tooltips will display ,if the user leaves the mouse pointer stationary over the node(whose text Length is greater than the width 
  /// of the tree)for a short period. 
  /// <example1>This example describes how to display Tooltips for the images's associated with the TreeNodeAdv.
  /// The tooltips for the images ,associated with the nodes can be displayed by using Syncfusion.Windows.Forms.ToolTipAdv along with handling 
  /// treeViewAdv's MouseHoverEvent .The tooltips for the images are displayed in the same way as the tooltips for the nodes.Here PointToClient and
  /// PointToNode methods are used in the treeViewAdv.
  /// <code lang="C#">
  /// private void treeViewAdv1_MouseHover(object sender, System.EventArgs e) 
  /// {  
  /// TreeNodeAdv node=new TreeNodeAdv(); 
  /// Point p=this.treeViewAdv1.PointToClient(Control.MousePosition); 
  /// node=this.treeViewAdv1.PointToNode(p); 
  /// Point mouseLoc=Control.MousePosition; 
  /// mouseLoc.Offset(10,10); 
  /// if(node==this.treeViewAdv1.Nodes[0]) 
  /// { 
  /// this.toolTipAdv1.ShowPopup(mouseLoc); 
  /// } 
  /// if(node==this.treeViewAdv1.Nodes[1]) 
  /// { 
  /// this.toolTipAdv2.ShowPopup(mouseLoc); 
  /// } 
  /// if(node==this.treeViewAdv1.Nodes[2]) 
  /// { 
  /// this.toolTipAdv3.ShowPopup(mouseLoc); 
  /// } 
  /// if(node==this.treeViewAdv1.Nodes[3]) 
  /// { 
  /// this.toolTipAdv4.ShowPopup(mouseLoc); 
  /// } 
  /// } 
  /// </code><code lang="VB">
  /// Private Sub treeViewAdv1_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) 
  /// Dim node As TreeNodeAdv = New TreeNodeAdv() 
  /// Dim p As Point=Me.treeViewAdv1.PointToClient(Control.MousePosition) 
  /// node=Me.treeViewAdv1.PointToNode(p) 
  /// Dim mouseLoc As Point=Control.MousePosition 
  /// mouseLoc.Offset(10,10) 
  /// If node Is Me.treeViewAdv1.Nodes(0) Then 
  /// Me.toolTipAdv1.ShowPopup(mouseLoc) 
  /// End If 
  /// If node Is Me.treeViewAdv1.Nodes(1) Then 
  /// Me.toolTipAdv2.ShowPopup(mouseLoc) 
  /// End If 
  /// If node Is Me.treeViewAdv1.Nodes(2) Then 
  /// Me.toolTipAdv3.ShowPopup(mouseLoc) 
  /// End If 
  /// If node Is Me.treeViewAdv1.Nodes(3) Then 
  /// Me.toolTipAdv4.ShowPopup(mouseLoc) 
  /// End If 
  /// End Sub 
  /// </code></example1>
  /// <example2>This example describes the way for completely disabling the Tooltips in the TreeViewAdv and the way  for disabling of tooltips for
  /// some particular nodes.
  /// The tooltip for some of the nodes which have HelpText can be disabled by handling ToolTipControl_BeforePopup event.Here in the treeViewAdv, for 
  /// some of the nodes e.Cancel property is set to true by getting the node's position in ToolTipControl's BeforePopup event handler in which the
  /// tooltips for the respective nodes are disabled .The tooltips for the same nodes can be enabled by setting the e.Cancel=false in ToolTipControl's 
  /// BeforePopup Event.
  /// <code lang="C#">
  /// private void ToolTipControl_BeforePopup(object sender, CancelEventArgs e)
  /// { 
  /// Point pt=this.treeViewAdv1.PointToClient(new Point(MousePosition.X,MousePosition.Y)); 
  /// TreeNodeAdv node=this.treeViewAdv1.GetNodeAtPoint(pt); 
  /// if(node!=null) 
  /// { 
  /// if(node.Text=="Node1" || node.Text=="Node3"||node.Text=="Node5"||node.Text=="Node7") 
  /// { 
  /// e.Cancel=true; 
  /// } 
  /// } 
  /// } 
  /// </code><code lang="VB">
  /// Private Sub ToolTipControl_BeforePopup(ByVal sender As Object, ByVal e As CancelEventArgs) 
  /// Dim pt As Point=Me.treeViewAdv1.PointToClient(New Point(MousePosition.X,MousePosition.Y)) 
  /// Dim node As TreeNodeAdv=Me.treeViewAdv1.GetNodeAtPoint(pt) 
  /// If Not node Is Nothing Then 
  /// If node.Text="Node1" OrElse node.Text="Node3" OrElse node.Text="Node5" OrElse node.Text="Node7" Then 
  /// e.Cancel=True 
  /// End If 
  /// End If 
  /// End Sub 
  /// </code></example2>
  [ 
  ToolboxItem( false ), 
  DocumentationExclude() 
  ]
  public class ToolTipAdv : PopupControlContainer
  {
    #region Class members
    /// <summary></summary>
    private bool m_bInheritHostCursor = true;
    /// <summary></summary>
    private int restrictWidth = 0;
    #endregion

    #region Container controls
    /// <summary></summary>
    private Label label;
    /// <summary></summary>
    private GradientPanel gradientPanel;
    /// <summary></summary>
    private Control ctrlHost;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private Container components = null;
    #endregion

    #region Class properties
	/// <summary></summary>
	[Browsable( true )]
	public new string Text
	{
		get
		{
			// Fixed defect : 2945 Tool tip does not display & symbol
			return Regex.Replace( base.Text, "&", "&&" );
		}
		set
		{
			if( base.Text != value )
			{
				base.Text = value;
			}
		}
	}

    /// <summary>
    /// The background color, gradient and other styles can be set through 
    /// this property.
    /// </summary>
    /// <remarks>
    /// The ToolTipAdv control provides this property to enable specialized
    /// custom gradient backgrounds.
    /// </remarks>
    [
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    Category( "Appearance" ),
    Description( "Lets you set the background color, gradient, etc." )
    ]
    public BrushInfo BackgroundColor
    {
      get
      {
        return gradientPanel.BackgroundColor;
      }
      set
      {
        gradientPanel.BackgroundColor = value;
      }
    }

    /// <summary>
    /// Gets / sets the 2D border style.
    /// </summary>
    [ Description( "Indicates the 2D border style." ), Category( "Appearance" ), DefaultValue( ButtonBorderStyle.Solid ) ]

    
    public ButtonBorderStyle BorderSingle
    {
      get
      {
        return gradientPanel.BorderSingle;
      }
      set
      {
        gradientPanel.BorderSingle = value;
      }
    }

    /// <summary>
    /// Gets / sets the style of the 3D border.
    /// </summary>
    [ Description( "Indicates the style of the 3D border." ), Category( "Appearance" ) ]
    
    public Border3DStyle Border3DStyle
    {
      get
      {
        return gradientPanel.Border3DStyle;
      }
      set
      {
        gradientPanel.Border3DStyle = value;
      }
    }

    /// <summary>
    /// Gets / sets the border style of the panel.
    /// </summary>
    [ DefaultValue( BorderStyle.Fixed3D ) ]
    public new BorderStyle BorderStyle
    {
      get
      {
        return this.gradientPanel.BorderStyle;
      }
      set
      {
        this.gradientPanel.BorderStyle = value;
        
        UpdateSize();
      }
    }
    /// <summary>
    /// Gets or sets the maximum width of the tooltip control. It wraps text that flows beyond the 
    /// restricted width.To allow text to flow in a single line, set restricted width to zero.
    /// </summary>
    [Description("Gets or sets the maximum width of the tooltip control. It wraps text that flows beyond the restricted width.To allow text to flow in a single line, set restricted width to zero.")]
    public int RestrictWidth
    {
      get
      {
        return restrictWidth;
      }
      set
      {
        if( restrictWidth != value )
        {
          restrictWidth = value;
          UpdateSize();
        }
      }
    }

    /// <summary>
    /// Indicates if cursor of host control is used.
    /// </summary>
    [
    DefaultValue( true ),
    Description( "Indicates if cursor of host control is used." ),
    Category( "Appearance" )
    ]
    public bool InheritHostCursor
    {
      get
      {
        return m_bInheritHostCursor;
      }
      set
      {
        m_bInheritHostCursor = value;
      }
    }

    /// <summary>
    /// Indicates whether to ignore all keys.
    /// </summary>
    /// <value>True to ignore all keys; False otherwise. Default is False.</value>
    /// <remarks>
    /// When the popup is showing, it will "swallow" all the WM_KEYDOWN and WM_CHAR
    /// messages. To prevent it, set this property to True.
    /// </remarks>
    [ 
    DefaultValue( true ),
    Category( "Popup" ),
    Description( "Specifies whether or not to ignore all keys." )
    ]
    public new bool IgnoreKeys
    {
      get
      {
        return base.IgnoreKeys;
      }
      set
      {
        base.IgnoreKeys = value;
      }
    }

    /// <summary></summary>
    [ 
    DefaultValue( false ),
    Category( "Appearance" ),
    Description( "Specifies whether the ToolTipAdv control will be visible." )
    ]
    public new bool Visible
    {
      get
      {
        return base.Visible;
      }
      set
      {
        base.Visible = value;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary></summary>
    /// <param name="host"/>
    public ToolTipAdv( Control host )
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();
      
      base.BorderStyle = BorderStyle.None;
      this.Controls.Add( gradientPanel );
      gradientPanel.Controls.Add( label );
      
      this.ctrlHost = host;
      this.IgnoreKeys = true;
      
      this.label.MouseDown += new MouseEventHandler( this.Label_MouseDown );
      this.label.MouseUp += new MouseEventHandler( this.Label_MouseUp );
      this.label.MouseMove += new MouseEventHandler( this.Label_MouseMove );
      this.label.MouseLeave += new EventHandler( this.Label_MouseLeave );
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing"/>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if( components != null )
        {
          components.Dispose();
        }

        if (gradientPanel != null)
        {
            gradientPanel.Dispose();
            gradientPanel = null;
      	}
        if (this.ctrlHost != null)
        {
            this.ctrlHost = null;
        }
        if (label != null)
        {
            this.label.MouseDown -= new MouseEventHandler(this.Label_MouseDown);
            this.label.MouseUp -= new MouseEventHandler(this.Label_MouseUp);
            this.label.MouseMove -= new MouseEventHandler(this.Label_MouseMove);
            this.label.MouseLeave -= new EventHandler(this.Label_MouseLeave);
            label.Dispose();
            label = null;
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
		this.label = new System.Windows.Forms.Label();
		this.gradientPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
		// 
		// label
		// 
		this.label.BackColor = System.Drawing.Color.Transparent;
		this.label.Dock = System.Windows.Forms.DockStyle.Fill;
		this.label.Location = new System.Drawing.Point( 17, 17 );
		this.label.Name = "label";
		this.label.TabIndex = 0;
		this.label.AutoSize = true;
		this.label.Text = "0";
		this.label.TextChanged += new System.EventHandler( this.label_TextChanged );

#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
		this.label.UseCompatibleTextRendering = false;
		this.label.Margin = Padding.Empty;
#endif
		// 
		// GradientPanel
		// 
		this.gradientPanel.BorderColor = System.Drawing.Color.Black;
		this.gradientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gradientPanel.Location = new System.Drawing.Point( 31, 42 );
		this.gradientPanel.Name = "gradientPanel";
		this.gradientPanel.Size = new System.Drawing.Size( 224, 80 );
		this.gradientPanel.TabIndex = 0;
		// 
		// ToolTipAdv
		// 
		this.Size = new System.Drawing.Size( 392, 112 );
		this.ForeColorChanged += new System.EventHandler( this.ToolTipAdv_ForeColorChanged );
		this.TextChanged += new System.EventHandler( this.ToolTipAdv_TextChanged );
		this.FontChanged += new System.EventHandler( this.ToolTipAdv_FontChanged );
		this.BackgroundImageChanged += new System.EventHandler( this.ToolTipAdv_BackgroundImageChanged );
		this.BackColorChanged += new System.EventHandler( this.ToolTipAdv_BackColorChanged );
	}
    #endregion

    #region Class codedom serialization
    /// <summary></summary>
    protected virtual void ResetBackgroundColor()
    {
      this.BackgroundColor = BrushInfo.Empty;
    }

    /// <summary></summary>
    /// <returns></returns>
    protected virtual bool ShouldSerializeBackgroundColor()
    {
      return ( gradientPanel.BackgroundColor != BrushInfo.Empty );
    }

    /// <summary></summary>
    protected virtual void ResetBorder3DStyle()
    {
      this.Border3DStyle = Border3DStyle.Sunken;
    }

    /// <summary></summary>
    /// <returns></returns>
    protected virtual bool ShouldSerializeBorder3DStyle()
    {
      return ( this.gradientPanel.Border3DStyle != Border3DStyle.Sunken );
    }

    /// <summary></summary>
    protected virtual void ResetRestrictWidth()
    {
      this.RestrictWidth = 0;
    }

    /// <summary></summary>
    /// <returns></returns>
    protected virtual bool ShouldSerializeRestrictWidth()
    {
      return ( this.restrictWidth > 0 );
    }

    #endregion

    #region Class overrides
    /// <summary>
    /// Raises the before popup event, when popup is about to be shown
    /// </summary>
    /// <param name="args"></param>
    protected override void OnBeforePopup( CancelEventArgs args )
    {
      if( this.ctrlHost != null )
      {
		  this.ParentControl = FindFormHelper.FindForm(this.ctrlHost);
		  if (this.ParentControl == null)
		  {
			  this.ParentControl = this.ctrlHost;
		  }

        if( this.InheritHostCursor )
        {
          this.Cursor = this.ctrlHost.Cursor;
        }
      }

      base.OnBeforePopup( args );
    }

	/// <summary>
	/// Raised when the size of the label's text is changed
	/// </summary>
	protected virtual void UpdateSize()
	{
		int border = 0;

		if( BorderStyle == BorderStyle.FixedSingle )
		{
			border = 1;
		}
		else if( BorderStyle == BorderStyle.Fixed3D )
		{
			border = 2;
		}

		Size sz = Size.Empty;

		if (this.RestrictWidth > 0)
		{
			sz = TextRenderer.MeasureText(this.label.Text, this.label.Font, new Size(this.RestrictWidth, int.MaxValue), TextFormatFlags.WordBreak);
		}
		else
		{
			this.label.AutoSize = false;
			this.label.AutoSize = true;
			sz = this.label.Size;
			this.label.AutoSize = false;
		}

#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
		sz.Height += 2 * border + label.Margin.Vertical;
		sz.Width += 2 * border + label.Margin.Horizontal;
#else
		sz.Height += 2 + 2 * border;
		sz.Width += 2 + 2 * border;
#endif
		this.Size = sz;
	}

    #endregion

    #region Class event handlers
    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void ToolTipAdv_TextChanged( object sender, EventArgs e )
    {
      this.label.Text = this.Text;
    }

    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void ToolTipAdv_FontChanged( object sender, EventArgs e )
    {
      this.label.Font = this.Font;
      UpdateSize();
    }

    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void ToolTipAdv_ForeColorChanged( object sender, EventArgs e )
    {
      label.ForeColor = this.ForeColor;
    }

    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void label_TextChanged( object sender, EventArgs e )
    {
      UpdateSize();
    }

    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void ToolTipAdv_BackgroundImageChanged( object sender, EventArgs e )
    {
      this.gradientPanel.BackgroundImage = this.BackgroundImage;
    }

    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void ToolTipAdv_BackColorChanged( object sender, EventArgs e )
    {
      this.gradientPanel.BackColor = this.BackColor;
    }

    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void Label_MouseDown( object sender, MouseEventArgs e )
    {
      Point ptclient = this.ctrlHost.PointToClient( this.label.PointToScreen( new Point( e.X, e.Y ) ) );

      if( e.Button == MouseButtons.Left )
      {
        if( e.Clicks == 2 )
        {
          NativeMethods.SendMessage( this.ctrlHost.Handle, NativeMethods.WM_LBUTTONDBLCLK,
            ( IntPtr )0x0001 /*MK_LBUTTON*/, 
            ( IntPtr )NativeMethods.MAKELPARAM( ptclient.X, ptclient.Y ) );
        }
        else
        {
          NativeMethods.SendMessage( this.ctrlHost.Handle, NativeMethods.WM_LBUTTONDOWN,
            ( IntPtr )0x0001 /*MK_LBUTTON*/, 
            ( IntPtr )NativeMethods.MAKELPARAM( ptclient.X, ptclient.Y ) );
        }
      }
      else if( e.Button == MouseButtons.Right )
      {
        HidePopup();
        
        NativeMethods.SendMessage( this.ctrlHost.Handle, NativeMethods.WM_RBUTTONDOWN,
          ( IntPtr )0x0002 /*MK_RBUTTON*/, 
          ( IntPtr )NativeMethods.MAKELPARAM( ptclient.X, ptclient.Y ) );
      }
      else
      {
        HidePopup();
      }
    }

    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void Label_MouseUp( object sender, MouseEventArgs e )
    {
      Point ptclient = this.ctrlHost.PointToClient( this.label.PointToScreen( new Point( e.X, e.Y ) ) );
      
      if( e.Button == MouseButtons.Left )
      {
        NativeMethods.SendMessage( this.ctrlHost.Handle, NativeMethods.WM_LBUTTONUP,
          ( IntPtr )0x0001 /*MK_LBUTTON*/, 
          ( IntPtr )NativeMethods.MAKELPARAM( ptclient.X, ptclient.Y ) );
      }
      else if( e.Button == MouseButtons.Right )
      {
        NativeMethods.SendMessage( this.ctrlHost.Handle, NativeMethods.WM_RBUTTONUP,
          ( IntPtr )0x0002 /*MK_RBUTTON*/, 
          ( IntPtr )NativeMethods.MAKELPARAM( ptclient.X, ptclient.Y ) );
      }
    }

    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void Label_MouseMove( object sender, MouseEventArgs e )
    {
      Point ptscreen = this.label.PointToScreen( new Point( e.X, e.Y ) );
      
      if( !this.ctrlHost.RectangleToScreen( this.ctrlHost.ClientRectangle ).Contains( ptscreen ) )
      {
        HidePopup( PopupCloseType.Deactivated );
      }
    }

    /// <summary></summary>
    /// <param name="sender"/>
    /// <param name="e"/>
    private void Label_MouseLeave( object sender, EventArgs e )
    {
      if( !this.label.RectangleToScreen( this.label.ClientRectangle ).Contains( Cursor.Position ) )
      {
        HidePopup( PopupCloseType.Deactivated );
      }
    }
    #endregion
  }
}