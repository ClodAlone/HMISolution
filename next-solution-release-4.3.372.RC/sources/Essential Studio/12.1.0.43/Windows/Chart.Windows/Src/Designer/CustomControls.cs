#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart.Design;
using Syncfusion.ComponentModel;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// 
	/// </summary>
	class PropertiesDialog : Form
	{
		#region	Members
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Panel panelButtonsContainer;
		private System.Windows.Forms.Button buttonCancel;
		#endregion

		#region	Consrtuctor
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertiesDialog"/> class.
		/// </summary>
		public PropertiesDialog()
		{
			this.InitializeComponent();
		}
		#endregion

		#region	Properties
		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <param name="selectedObject">The selected object.</param>
		/// <returns></returns>
		public bool ShowDialog( object selectedObject )
		{
			propertyGrid.SelectedObject = selectedObject;
			propertyGrid.ExpandAllGridItems();

			return this.ShowDialog() == DialogResult.OK;
		}
		#endregion

		#region	Implementation
		/// <summary>
		/// Initializes the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.panelButtonsContainer = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.panelButtonsContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(256, 334);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.Text = "propertyGrid1";
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// panelButtonsContainer
			// 
			this.panelButtonsContainer.Controls.Add(this.buttonCancel);
			this.panelButtonsContainer.Controls.Add(this.buttonOK);
			this.panelButtonsContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelButtonsContainer.Location = new System.Drawing.Point(0, 334);
			this.panelButtonsContainer.Name = "panelButtonsContainer";
			this.panelButtonsContainer.Size = new System.Drawing.Size(256, 40);
			this.panelButtonsContainer.TabIndex = 1;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(176, 8);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(88, 8);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 24);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			// 
			// PropertiesDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(256, 374);
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.panelButtonsContainer);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Name = "PropertiesDialog";
			this.panelButtonsContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
	/// <summary>
  /// 
  /// </summary>
  [ ToolboxItem( false ) ]
  class ImageButton : Button
  {
    #region Members
    private bool m_isPushed = false;
    private bool m_isPushable = false;
    private bool m_isMouseOver = false;
    private bool m_isPressed = false;

    private Color m_borderColor = Color.Transparent;
    private Image m_selectedImage = ChartWizardResources.ButtonSelectedImage;
		private Image m_normalImage = ChartWizardResources.ButtonNormalImage;
    private int m_tagIndex = -1;
    #endregion

    #region Properties
		/// <summary>
		/// Gets or sets border color;
		/// </summary>
		/// <value>The color of the border.</value>
		public Color BorderColor
    {
      get
      {
        return m_borderColor;
      }
      set
      {
        m_borderColor = value;
      }
    }
		/// <summary>
		/// Gets or sets the normal image.
		/// </summary>
		/// <value>The normal image.</value>
    public Image NormalImage
    {
      get
      {
        return m_normalImage;
      }
      set
      {
        if( m_normalImage != value )
        {
          m_normalImage = value; 
          this.Invalidate();
        }
      }
    }
		/// <summary>
		/// Gets or sets the selected image.
		/// </summary>
		/// <value>The selected image.</value>
    public Image SelectedImage
    {
      get
      {
        return m_selectedImage;
      }
      set
      {
        if( m_selectedImage != value )
        {
          m_selectedImage = value; 
          this.Invalidate();
        }
      }
    }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is pushable.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is pushable; otherwise, <c>false</c>.
		/// </value>
		[ DefaultValue( false ) ]
		public bool IsPushable
    {
      get
      {
        return m_isPushable;
      }
      set
      {
        m_isPushable = value;
      }
    }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is pushed.
		/// </summary>
		/// <value><c>true</c> if this instance is pushed; otherwise, <c>false</c>.</value>
		[ DefaultValue( false ) ]
    public bool IsPushed
    {
      get
      {
        return m_isPushed;
      }
      set
      {
        if( m_isPushed != value )
        {
          m_isPushed = value;
          this.Invalidate();
        }
      }
    }
		/// <summary>
		/// Gets or sets the index of the tag.
		/// </summary>
		/// <value>The index of the tag.</value>
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable( false ) ]
    public int TagIndex
    {
      get
      {
        return m_tagIndex; 
      }
      set
      {
        m_tagIndex = value;
      }
    }
    #endregion

    #region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageButton"/> class.
		/// </summary>
    public ImageButton()
    {
      this.SetStyle( ControlStyles.UserPaint 
        | ControlStyles.SupportsTransparentBackColor
        | ControlStyles.ResizeRedraw, true);
    }
    #endregion

    #region Implementation
		/// <summary>
		/// Should the serialize normal image.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeNormalImage()
		{
			return m_normalImage != ChartWizardResources.ButtonNormalImage;
		}
		/// <summary>
		/// Should the serialize selected image.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeSelectedImage()
		{
			return m_selectedImage != ChartWizardResources.ButtonSelectedImage;
		}
		/// <summary>
		/// Should the color of the serialize border.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeBorderColor()
		{
			return m_borderColor != Color.Transparent;
		}
		/// <summary>
		/// Resets the normal image.
		/// </summary>
		protected void ResetNormalImage()
		{
			m_normalImage = ChartWizardResources.ButtonNormalImage;
		}
		/// <summary>
		/// Resets the selected image.
		/// </summary>
		protected void ResetSelectedImage()
		{
			m_selectedImage = ChartWizardResources.ButtonSelectedImage;
		}
		/// <summary>
		/// Resets the color of the border.
		/// </summary>
		protected void ResetBorderColor()
		{
			m_borderColor = Color.Transparent;
		}
		/// <summary>
		/// Overrides <see cref="Control.OnPaint"/> method.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
      StringFormat stringFormat = new StringFormat(StringFormat.GenericDefault);

      switch( this.TextAlign )
      {
        case ContentAlignment.BottomLeft :
          stringFormat.Alignment = StringAlignment.Near;
          stringFormat.LineAlignment = StringAlignment.Far;
          break;
        case ContentAlignment.BottomCenter :
          stringFormat.Alignment = StringAlignment.Center;
          stringFormat.LineAlignment = StringAlignment.Far;
          break;
        case ContentAlignment.BottomRight :
          stringFormat.Alignment = StringAlignment.Far;
          stringFormat.LineAlignment = StringAlignment.Far;
          break;

        case ContentAlignment.TopLeft :
          stringFormat.Alignment = StringAlignment.Near;
          stringFormat.LineAlignment = StringAlignment.Near;
          break;
        case ContentAlignment.TopCenter :
          stringFormat.Alignment = StringAlignment.Center;
          stringFormat.LineAlignment = StringAlignment.Near;
          break;
        case ContentAlignment.TopRight :
          stringFormat.Alignment = StringAlignment.Far;
          stringFormat.LineAlignment = StringAlignment.Near;
          break;

        case ContentAlignment.MiddleLeft :
          stringFormat.Alignment = StringAlignment.Near;
          stringFormat.LineAlignment = StringAlignment.Center;
          break;
        case ContentAlignment.MiddleCenter :
          stringFormat.Alignment = StringAlignment.Center;
          stringFormat.LineAlignment = StringAlignment.Center;
          break;
        case ContentAlignment.MiddleRight :
          stringFormat.Alignment = StringAlignment.Far;
          stringFormat.LineAlignment = StringAlignment.Center;
          break;
      }

      using( SolidBrush backBrush = new SolidBrush(this.BackColor ))
      {
        e.Graphics.FillRectangle(backBrush, 0,0 ,this.Width, this.Height );
      }

      if( m_isPressed || m_isPushed || m_isMouseOver )
      {
        if(m_selectedImage != null )
        {
          e.Graphics.DrawImage(m_selectedImage, 0,0, this.Width, this.Height );
        }
      }
      else if(m_normalImage != null )
      {
        e.Graphics.DrawImage(m_normalImage, 0,0, this.Width, this.Height );
      }

      using( Pen pen = new Pen(m_borderColor))
      {
        e.Graphics.DrawRectangle(pen, 0, 0, this.Width-1, this.Height-1);
      }

      using( Brush textBrush = new SolidBrush( this.ForeColor))
      {
        e.Graphics.DrawString( this.Text, this.Font, textBrush, this.ClientRectangle, stringFormat );
      }
    }
		/// <summary>
		/// Raises the <see cref="E:MouseEnter"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnMouseEnter(EventArgs e)
    {
      m_isMouseOver = true;
      this.Invalidate();
      base.OnMouseEnter (e);
    }
		/// <summary>
		/// Raises the <see cref="E:MouseLeave"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnMouseLeave(EventArgs e)
    {
      m_isMouseOver = false;
      this.Invalidate();
      base.OnMouseLeave (e);
    }
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
      m_isPressed = true;
      this.Invalidate();
      base.OnMouseDown (e);
    }
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
    protected override void OnMouseUp(MouseEventArgs e)
    {
      m_isPressed = false;
      this.Invalidate();
      base.OnMouseUp (e);
    }
    #endregion
  }
  /// <summary>
  /// 
  /// </summary>
  [ ToolboxItem( false ) ]
  class ChartTabControl : TabControl
  {
    #region Members
    private Image m_noneImage = ChartWizardResources.TabHeaderNormalImage;
		private Image m_selectedImage = ChartWizardResources.TabHeaderSelectedImage;
    private Color m_borderColor = Color.FromArgb(132, 170, 217);
    private Color m_innerBackColor = Color.FromArgb(190, 216, 253);
    #endregion

    #region Properties
		/// <summary>
		/// Gets or sets the none image.
		/// </summary>
		/// <value>The none image.</value>
    public Image NoneImage
    {
      get
      {
        return m_noneImage;
      }
      set
      {
        m_noneImage = value;
        Invalidate();
      }
    }
		/// <summary>
		/// Gets or sets the selected image.
		/// </summary>
		/// <value>The selected image.</value>
    public Image SelectedImage
    {
      get
      {
        return m_selectedImage;
      }
      set
      {
        m_selectedImage = value;
        Invalidate();
      }
    }
		/// <summary>
		/// This member is not meaningful for this control.
		/// </summary>
		/// <value></value>
		/// <returns>Always <see cref="P:System.Drawing.SystemColors.Control"></see>.</returns>
    public override Color BackColor
    {
      get
      {
        return Color.Transparent;
      }
    }
		/// <summary>
		/// Gets or sets border color;
		/// </summary>
		/// <value>The color of the border.</value>
    public Color BorderColor
    {
      get
      {
        return m_borderColor;
      }
      set
      {
        m_borderColor = value;
      }
    }
		/// <summary>
		/// Gets or sets the color of the inner back.
		/// </summary>
		/// <value>The color of the inner back.</value>
    public Color InnerBackColor
    {
      get
      {
        return m_innerBackColor;
      }
      set
      {
        m_innerBackColor = value;
      }
    }
    #endregion

    #region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartTabControl"/> class.
		/// </summary>
    public ChartTabControl()
    {
      this.SetStyle( ControlStyles.SupportsTransparentBackColor | 
        ControlStyles.UserPaint, true );
    }
    #endregion

    #region Implementation
		/// <summary>
		/// Should the serialize none image.
		/// </summary>
		/// <returns></returns>
    protected bool ShouldSerializeNoneImage()
    {
			return m_noneImage != ChartWizardResources.TabHeaderNormalImage;
    }
		/// <summary>
		/// Should the serialize selected image.
		/// </summary>
		/// <returns></returns>
    protected bool ShouldSerializeSelectedImage()
    {
			return m_selectedImage != ChartWizardResources.TabHeaderSelectedImage;
    }
		/// <summary>
		/// Should the color of the serialize border.
		/// </summary>
		/// <returns></returns>
    protected bool ShouldSerializeBorderColor()
    {
      return m_borderColor != Color.FromArgb(132, 170, 217);
    }
		/// <summary>
		/// Should the color of the serialize inner back.
		/// </summary>
		/// <returns></returns>
    protected bool ShouldSerializeInnerBackColor()
    {
      return m_innerBackColor != Color.FromArgb(190, 216, 253);
    }
		/// <summary>
		/// Resets the none image.
		/// </summary>
    protected void ResetNoneImage()
    {
			m_noneImage = ChartWizardResources.TabHeaderNormalImage;
    }
		/// <summary>
		/// Resets the selected image.
		/// </summary>
    protected void ResetSelectedImage()
    {
			m_selectedImage = ChartWizardResources.TabHeaderSelectedImage;
    }
		/// <summary>
		/// Resets the color of the border.
		/// </summary>
    protected void ResetBorderColor()
    {
      m_borderColor = Color.FromArgb(132, 170, 217);
    }
		/// <summary>
		/// Resets the color of the inner back.
		/// </summary>
    protected void ResetInnerBackColor()
    {
      m_innerBackColor = Color.FromArgb(190, 216, 253);
    }
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
      StringFormat stringFormat = new StringFormat();
      stringFormat.Alignment = StringAlignment.Center;
      stringFormat.LineAlignment = StringAlignment.Center;

      Rectangle selectedRect = Rectangle.Empty;

      for( int i = 0; i < TabPages.Count; i ++ )
      {
        TabPage page = TabPages[ i ];
        Rectangle rect = this.GetTabRect( i );

        if( SelectedTab == page )
        {
          selectedRect = rect;

          if( m_selectedImage != null )
          {
            e.Graphics.DrawImage( m_selectedImage, rect );
          }
        }
        else
        {
          if( m_noneImage != null )
          {
            e.Graphics.DrawImage( m_noneImage, rect );
          }
        }

        using( SolidBrush textBrush = new SolidBrush( page.ForeColor ))
        {
          e.Graphics.DrawString( page.Text, page.Font, textBrush, rect, stringFormat );
        }
      }

      GraphicsPath borderPath = new GraphicsPath();

      borderPath.AddLine(selectedRect.Right, selectedRect.Bottom, this.Width - 1, selectedRect.Bottom);
      borderPath.AddLine(this.Width - 1, this.Height - 1, 0, this.Height - 1 );
      borderPath.AddLine(0, selectedRect.Bottom, selectedRect.Left, selectedRect.Bottom);

      using( SolidBrush sb = new SolidBrush(m_innerBackColor))
      {
        e.Graphics.FillPath(sb,borderPath);
      }

      using( Pen borderPen = new Pen(m_borderColor))
      {
        e.Graphics.DrawPath(borderPen,borderPath);
      }

      base.OnPaint (e);
    }
    #endregion
  }
  /// <summary>
  /// 
  /// </summary>
  [ToolboxItem(false)]
  class ChartGroupBox : GroupBox
  {
    #region Constants
    private const int DEF_TEXT_SPACING = 8;
    private const int DEF_BORDER_SPACING = 2;
    #endregion

    #region Memebers
		private bool m_isPaintBackground = false;
    private Color m_borderColor = Color.Black;
    #endregion

    #region Properties
		/// <summary>
		/// Gets or sets the color of the border.
		/// </summary>
		/// <value>The color of the border.</value>
    [ DefaultValue(typeof(Color), "Black") ]
    public Color BorderColor
    {
      get
      {
        return m_borderColor;
      }
      set
      {
        m_borderColor = value;
        Invalidate();
      }
    }
		/// <summary>
		/// Gets or sets the background color for the control.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Drawing.Color"></see> that represents the background color of the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor"></see> property.</returns>
		/// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
		[ DefaultValue( typeof( Color), "White" ) ]
		public override Color BackColor
		{
			get
			{
				return m_isPaintBackground ? Color.Transparent : base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
    #endregion

    #region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartGroupBox"/> class.
		/// </summary>
    public ChartGroupBox()
    {
      this.SetStyle( ControlStyles.SupportsTransparentBackColor, true );
			this.BackColor = Color.White;
    }
    #endregion

    #region Implementation
		/// <summary>
		/// Raises the <see cref="E:Paint"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
      if( this.FlatStyle == FlatStyle.Flat )
      {
        Graphics graph = e.Graphics;
        Rectangle rect = Rectangle.Inflate( ClientRectangle, -DEF_TEXT_SPACING, -DEF_BORDER_SPACING );
        rect.Size = Size.Ceiling( graph.MeasureString( Text, Font, rect.Width ));
        rect.Width += DEF_TEXT_SPACING;
        rect.Height += DEF_BORDER_SPACING;

        StringFormat stringFormat = new StringFormat();

        stringFormat.Alignment = StringAlignment.Center;
        stringFormat.LineAlignment = StringAlignment.Center;

        int halfHight = rect.Height/2;
        Point[] points = new Point[]{ new Point( DEF_BORDER_SPACING, halfHight ),
                                      new Point( rect.Left, halfHight ),
                                      new Point( rect.Left, rect.Top ),
                                      new Point( rect.Right, rect.Top ),
                                      new Point( rect.Right, halfHight ),
                                      new Point( this.Width - DEF_BORDER_SPACING, halfHight ),
                                      new Point( this.Width - DEF_BORDER_SPACING, this.Height - DEF_BORDER_SPACING ),
                                      new Point( DEF_BORDER_SPACING, this.Height - DEF_BORDER_SPACING )};

        using( SolidBrush background = new SolidBrush( this.BackColor ))
        {
          graph.FillPolygon( background, points );
        }

        using( Pen border = new Pen( m_borderColor ))
        {
          graph.DrawPolygon( border, points );
        }

        using( SolidBrush textBrush = new SolidBrush( ForeColor ))
        {
          graph.DrawString( this.Text, this.Font, textBrush, rect, stringFormat );
        }
      }
      else
      {
        base.OnPaint (e);
      }
    }
		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="pevent">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains information about the control to paint.</param>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			m_isPaintBackground = true;
			base.OnPaintBackground (pevent);
			m_isPaintBackground = false;
		}
    #endregion
  }
	/// <summary>
	/// 
	/// </summary>
	class ChartComboBox : ComboBox
	{
		#region Members
		private StringFormat m_sFormat = new StringFormat();
		private ImageList m_imageList;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the image list.
		/// </summary>
		/// <value>The image list.</value>
		[ DefaultValue( null ) ]
		public ImageList ImageList
		{
			get { return m_imageList; }
			set 
			{ 
				if( m_imageList != value )
				{
					m_imageList = value;

					if (m_imageList != null)
					{
						this.ItemHeight = Math.Max(this.ItemHeight, m_imageList.ImageSize.Height + 2);
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether your code or the operating system will handle drawing of elements in the list.
		/// </summary>
		/// <value></value>
		/// <returns>One of the <see cref="T:System.Windows.Forms.DrawMode"></see> enumeration values. The default is <see cref="F:System.Windows.Forms.DrawMode.Normal"></see>.</returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value is not a valid <see cref="T:System.Windows.Forms.DrawMode"></see> enumeration value. </exception>
		/// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
		[ DefaultValue(DrawMode.OwnerDrawVariable) ]
		new public DrawMode DrawMode
		{
			get
			{
				return base.DrawMode;
			}
			set
			{
				base.DrawMode = value;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartComboBox"/> class.
		/// </summary>
		public ChartComboBox()
		{
			m_sFormat.LineAlignment = StringAlignment.Center;
			base.DrawMode = DrawMode.OwnerDrawVariable;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.ComboBox.DrawItem"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.DrawItemEventArgs"></see> that contains the event data.</param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.DrawBackground();

			if (m_imageList != null && e.Index >= 0 && e.Index < m_imageList.Images.Count)
			{
				int iy = e.Bounds.Y + (e.Bounds.Height - m_imageList.ImageSize.Height) / 2;
				RectangleF txtRect = e.Bounds;
				txtRect.X += m_imageList.ImageSize.Width;
				txtRect.Width -= m_imageList.ImageSize.Width;

				m_imageList.Draw(e.Graphics, e.Bounds.X, iy, e.Index);

				using (SolidBrush sb = new SolidBrush(this.ForeColor))
				{
					e.Graphics.DrawString(this.Items[e.Index].ToString(), this.Font, sb, txtRect, m_sFormat);
				}
			}
			else
			{
				using (SolidBrush sb = new SolidBrush(this.ForeColor))
				{
					e.Graphics.DrawString(this.Items[e.Index].ToString(), this.Font, sb, e.Bounds, m_sFormat);
				}
			}

			e.DrawFocusRectangle();

			base.OnDrawItem(e);
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	class BrushInfoBox : UserControl
	{
		#region	BrushInfoContainer
		/// <summary>
		/// 
		/// </summary>
		private class BrushInfoContainer
		{
			#region	Members
			private BrushInfo m_brushInfo = null;
			#endregion

			#region	Properties
			/// <summary>
			/// Gets or sets the brush info.
			/// </summary>
			/// <value>The brush info.</value>
			public BrushInfo BrushInfo
			{
				get
				{
					return m_brushInfo;
				}
				set
				{
					m_brushInfo = value;
				}
			}		
			#endregion
		}
		#endregion

		#region	Members
		private Panel m_previewPanel = null;
		private Button m_editButton = null;
		private BrushInfo m_brushInfo = new BrushInfo(Color.White);
		#endregion

		#region	Events
		public event EventHandler BrushInfoChanged;
		#endregion

		#region	Properties
		/// <summary>
		/// Gets or sets the brush info.
		/// </summary>
		/// <value>The brush info.</value>
		[ Editor( typeof(BrushInfoEditor), typeof(UITypeEditor)) ]
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable( false )]
		public BrushInfo BrushInfo
		{
			get
			{
				return m_brushInfo;
			}
			set
			{
				if( m_brushInfo != value )
				{
					m_brushInfo = value;
					m_previewPanel.Invalidate();

					if( BrushInfoChanged != null )
					{
						BrushInfoChanged(this, EventArgs.Empty);
					}
				}
			}
		}		
		#endregion

		#region	Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="BrushInfoBox"/> class.
		/// </summary>
		public BrushInfoBox()
		{
			m_editButton = new Button();
			m_editButton.Dock = DockStyle.Right;
			m_editButton.Width = 24;
			m_editButton.Text = "...";
			m_editButton.FlatStyle = FlatStyle.System;
			m_editButton.Click += new EventHandler(OnEditButtonClick);
			
			m_previewPanel = new Panel();
			m_previewPanel.Dock = DockStyle.Fill;
			m_previewPanel.Paint += new PaintEventHandler(OnPreviewPanelPaint);
			m_previewPanel.BorderStyle = BorderStyle.FixedSingle;

			this.Controls.Add(m_previewPanel);
			this.Controls.Add(m_editButton);
		}
		#endregion

		#region	Implementation
		/// <summary>
		/// Called when edit button is clicked.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnEditButtonClick(object sender, EventArgs e)
		{
			PropertiesDialog dialog = new PropertiesDialog();
			BrushInfoContainer brushInfoContainer = new BrushInfoContainer();

			brushInfoContainer.BrushInfo = m_brushInfo;

			if( dialog.ShowDialog(brushInfoContainer) ) 
			{
				this.BrushInfo = brushInfoContainer.BrushInfo;
			}
		}
		/// <summary>
		/// Called when preview panel is painted.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
		private void OnPreviewPanelPaint(object sender, PaintEventArgs e)
		{
			BrushPaint.FillRectangle(e.Graphics, m_previewPanel.ClientRectangle, m_brushInfo);
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	class ColorBox : UserControl
	{
		#region	Members
		private Panel m_previewPanel = null;
		private ColorPickerButton m_editButton = null;
		#endregion

		#region	Events
		/// <summary>
		/// Occurs when color changed.
		/// </summary>
		public event EventHandler ColorChanged;
		#endregion

		#region	Properties
		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable( false )]
		public Color Color
		{
			get
			{
				return m_editButton.SelectedColor;
			}
			set
			{
				m_editButton.SelectedColor = value;
			}
		}		
		#endregion

		#region	Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorBox"/> class.
		/// </summary>
		public ColorBox()
		{
			m_editButton = new ColorPickerButton();
			m_editButton.Dock = DockStyle.Right;
			m_editButton.Width = 24;
			m_editButton.Text = "...";
			m_editButton.ColorSelected += new EventHandler(OnColorSelected);
			m_editButton.FlatStyle = FlatStyle.System;
			
			m_previewPanel = new Panel();
			m_previewPanel.Dock = DockStyle.Fill;
			m_previewPanel.Paint += new PaintEventHandler(OnPreviewPanelPaint);
			m_previewPanel.BorderStyle = BorderStyle.FixedSingle;

			this.Controls.Add(m_previewPanel);
			this.Controls.Add(m_editButton);
		}
		#endregion

		#region	Implementation
		/// <summary>
		/// Called when preview panel is painted.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
		private void OnPreviewPanelPaint(object sender, PaintEventArgs e)
		{
			BrushPaint.FillRectangle(e.Graphics, m_previewPanel.ClientRectangle, m_editButton.SelectedColor);
		}
		/// <summary>
		/// Called when color is selected.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnColorSelected(object sender, EventArgs e)
		{
			m_previewPanel.Invalidate();

			if( ColorChanged != null )
			{
				ColorChanged(this, e);
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	class ChartTextOrientationBox : UserControl
	{
		#region	Members
		private ImageButton[] m_buttons = null;
		private ChartTextOrientation m_orientation = ChartTextOrientation.Smart;
		#endregion

		#region	Events
		public event EventHandler OrientationChanged;
		#endregion

		#region	Proeprties
		/// <summary>
		/// Gets or sets the orientation.
		/// </summary>
		/// <value>The orientation.</value>
		[ DefaultValue(ChartTextOrientation.Center) ]
		public ChartTextOrientation Orientation
		{
			get
			{
				return m_orientation;
			}
			set
			{
				if( m_orientation != value )
				{
					m_orientation = value;

					for( int i = 0; i < m_buttons.Length; i ++ )
					{
						m_buttons[i].IsPushed = object.Equals( m_buttons[i].Tag, m_orientation);
					}

					if( OrientationChanged != null )
					{
						OrientationChanged( this, EventArgs.Empty);
					}
				}
			}
		}
		#endregion

		#region	Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartTextOrientationBox"/> class.
		/// </summary>
		public ChartTextOrientationBox()
		{
			m_buttons = new ImageButton[9];

			for( int i = 0; i < m_buttons.Length; i ++ )
			{
				m_buttons[i] = new ImageButton();

				m_buttons[i].Click += new EventHandler(OnOrientationButtonClick);

				this.Controls.Add(m_buttons[i]);
			}

			m_buttons[0].Tag = ChartTextOrientation.UpLeft;
			m_buttons[1].Tag = ChartTextOrientation.Up;
			m_buttons[2].Tag = ChartTextOrientation.UpRight;

			m_buttons[3].Tag = ChartTextOrientation.Left;
			m_buttons[4].Tag = ChartTextOrientation.Center;
			m_buttons[5].Tag = ChartTextOrientation.Right;

			m_buttons[6].Tag = ChartTextOrientation.DownLeft;
			m_buttons[7].Tag = ChartTextOrientation.Down;
			m_buttons[8].Tag = ChartTextOrientation.DownRight;

			m_buttons[0].NormalImage = ChartWizardResources.ButtonTLNormalImage;
			m_buttons[1].NormalImage = ChartWizardResources.ButtonTCNormalImage;
			m_buttons[2].NormalImage = ChartWizardResources.ButtonTRNormalImage;

			m_buttons[3].NormalImage = ChartWizardResources.ButtonCLNormalImage;
			m_buttons[4].NormalImage = ChartWizardResources.ButtonCCNormalImage;
			m_buttons[5].NormalImage = ChartWizardResources.ButtonCRNormalImage;

			m_buttons[6].NormalImage = ChartWizardResources.ButtonBLNormalImage;
			m_buttons[7].NormalImage = ChartWizardResources.ButtonBCNormalImage;
			m_buttons[8].NormalImage = ChartWizardResources.ButtonBRNormalImage;

			m_buttons[0].SelectedImage = ChartWizardResources.ButtonTLSelectedImage;
			m_buttons[1].SelectedImage = ChartWizardResources.ButtonTCSelectedImage;
			m_buttons[2].SelectedImage = ChartWizardResources.ButtonTRSelectedImage;

			m_buttons[3].SelectedImage = ChartWizardResources.ButtonCLSelectedImage;
			m_buttons[4].SelectedImage = ChartWizardResources.ButtonCCSelectedImage;
			m_buttons[5].SelectedImage = ChartWizardResources.ButtonCRSelectedImage;

			m_buttons[6].SelectedImage = ChartWizardResources.ButtonBLSelectedImage;
			m_buttons[7].SelectedImage = ChartWizardResources.ButtonBCSelectedImage;
			m_buttons[8].SelectedImage = ChartWizardResources.ButtonBRSelectedImage;

			this.Orientation = ChartTextOrientation.Center;
		}
		#endregion

		#region	Implementation
		/// <summary>
		/// Called when orientation button is clicked.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnOrientationButtonClick(object sender, EventArgs e)
		{
			this.Orientation = (ChartTextOrientation)(sender as ButtonBase).Tag;
		}
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged"></see> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
		protected override void OnSizeChanged(EventArgs e)
		{
			int cnt = (int)Math.Sqrt(m_buttons.Length);
			Size sz = new Size( this.Width / cnt, this.Height / cnt);

			for( int i = 0; i < cnt; i ++ )
			{
				for( int j = 0; j < cnt; j ++ )
				{
          Button bttn = m_buttons[ i * cnt + j ];

					bttn.Location = new Point( j * sz.Width, i * sz.Height);
					bttn.Size = sz;
				}
			}

			base.OnSizeChanged (e);
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	class FontBox : UserControl
	{
		#region	Members
		private Label m_previewLabel = null;
		private Button m_editButton = null;

		private Font m_selectedFont = null;
		#endregion

		#region	Events
		/// <summary>
		/// Occurs when selected font is changed.
		/// </summary>
		public event EventHandler SelectedFontChanged;
		#endregion

		#region	Properties
		/// <summary>
		/// Gets or sets the selected font.
		/// </summary>
		/// <value>The selected font.</value>
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable( false )]
		public Font SelectedFont
		{
			get
			{
				return m_selectedFont;
			}
			set
			{
				if( m_selectedFont != value )
				{
					m_selectedFont = value;
					m_previewLabel.Text = m_selectedFont.Name + ", " + m_selectedFont.Size;
					m_previewLabel.Font = new Font(m_selectedFont.FontFamily, 7.5f );

					if( SelectedFontChanged != null )
					{
						SelectedFontChanged( this, EventArgs.Empty );
					}
				}
			}
		}		
		#endregion

		#region	Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FontBox"/> class.
		/// </summary>
		public FontBox()
		{
			m_editButton = new Button();
			m_editButton.Dock = DockStyle.Right;
			m_editButton.Width = 24;
			m_editButton.Text = "...";
			m_editButton.Click += new EventHandler(OnEditButtonClick);
			m_editButton.FlatStyle = FlatStyle.System;
			
			m_previewLabel = new Label();
			m_previewLabel.Dock = DockStyle.Fill;
			m_previewLabel.BorderStyle = BorderStyle.FixedSingle;
			m_previewLabel.TextAlign = ContentAlignment.MiddleCenter;

			this.Controls.Add(m_previewLabel);
			this.Controls.Add(m_editButton);

			this.SelectedFont = this.Font;
		}
		#endregion

		#region	Implementation
		/// <summary>
		/// Called when edit button is clicked.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnEditButtonClick(object sender, EventArgs e)
		{
			FontDialog fontDialog = new FontDialog();
			fontDialog.Font = this.SelectedFont;

			if( fontDialog.ShowDialog() == DialogResult.OK )
			{
				this.SelectedFont = fontDialog.Font;
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	class StringAlignmentBox : UserControl
	{
		#region	Members
		private StringAlignment m_alignment = StringAlignment.Near;

		private RadioButton m_radioButtonNear = null;
		private RadioButton m_radioButtonFar = null;
		private RadioButton m_radioButtonCenter = null;
		#endregion

		#region	Events
		/// <summary>
		/// Occurs when Alignment is changed.
		/// </summary>
		public event EventHandler StringAlignmentChanged;
		#endregion

		#region	Properties
		/// <summary>
		/// Gets or sets the string alignment.
		/// </summary>
		/// <value>The string alignment.</value>
		[ DefaultValue( StringAlignment.Center) ]
		public StringAlignment StringAlignment
		{
			get
			{
				return m_alignment;
			}
			set
			{
				if( m_alignment != value )
				{
					m_alignment = value;

					m_radioButtonNear.Checked = (value == StringAlignment.Near);
					m_radioButtonCenter.Checked = (value == StringAlignment.Center);
					m_radioButtonFar.Checked = (value == StringAlignment.Far);

					if( StringAlignmentChanged != null )
					{
						StringAlignmentChanged( this, EventArgs.Empty );
					}
				}
			}
		}
		#endregion

		#region	Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="StringAlignmentBox"/> class.
		/// </summary>
		public StringAlignmentBox()
		{
			m_radioButtonNear = new RadioButton();
			m_radioButtonCenter = new RadioButton();
			m_radioButtonFar = new RadioButton();

			m_radioButtonNear.Text = "Near";
			m_radioButtonCenter.Text = "Center";
			m_radioButtonFar.Text = "Far";

			m_radioButtonNear.SetBounds( 0, 0, 50, 24 );
			m_radioButtonCenter.SetBounds( 50, 0, 60, 24 );
			m_radioButtonFar.SetBounds( 110, 0, 40, 24 );

			m_radioButtonNear.FlatStyle = FlatStyle.System;
			m_radioButtonCenter.FlatStyle = FlatStyle.System;
			m_radioButtonFar.FlatStyle = FlatStyle.System;

			m_radioButtonNear.CheckedChanged += new EventHandler(OnRadioButtonCheckedChanged);
			m_radioButtonCenter.CheckedChanged += new EventHandler(OnRadioButtonCheckedChanged);
			m_radioButtonFar.CheckedChanged += new EventHandler(OnRadioButtonCheckedChanged);

			this.Controls.Add(m_radioButtonNear);
			this.Controls.Add(m_radioButtonCenter);
			this.Controls.Add(m_radioButtonFar);

			this.StringAlignment = StringAlignment.Center;
		}
		#endregion

		#region	Implementation
		/// <summary>
		/// Called when radio button checked is changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;

			if( radioButton.Checked )
			{
				if( radioButton == m_radioButtonCenter )
				{
					this.StringAlignment = StringAlignment.Center;
				}
				else if( radioButton == m_radioButtonNear )
				{
					this.StringAlignment = StringAlignment.Near;
				}
				else if( radioButton == m_radioButtonFar )
				{
					this.StringAlignment = StringAlignment.Far;
				}
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	class HighlightImageLabel : Label
	{
		#region Members
		private Image m_highlightImage = null;
		private Image m_normaImage = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the highlight image.
		/// </summary>
		/// <value>The highlight image.</value>
		[DefaultValue( null )] 
		public Image HighlightImage
		{
			get { return m_highlightImage; }
			set { m_highlightImage = value; }
		}
		/// <summary>
		/// Gets or sets the normal image.
		/// </summary>
		/// <value>The normal image.</value>
		[DefaultValue(null)]
		public Image NormalImage
		{
			get { return m_normaImage; }
			set 
			{ 
				m_normaImage = value;
				this.Image = value;
			}
		}
		/// <summary>
		/// Gets or sets the image that is displayed on a <see cref="T:System.Windows.Forms.Label"></see>.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="T:System.Drawing.Image"></see> displayed on the <see cref="T:System.Windows.Forms.Label"></see>. The default is null.</returns>
		/// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new Image Image
		{
			get
			{
				return base.Image;
			}
			set
			{
				base.Image = value;
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Raises the <see cref="E:MouseEnter"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnMouseEnter(EventArgs e)
    {
			base.Image = m_highlightImage;
			base.OnMouseEnter(e);
    }
		/// <summary>
		/// Raises the <see cref="E:MouseLeave"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnMouseLeave(EventArgs e)
    {
			base.Image = m_normaImage;
      base.OnMouseLeave (e);
    }
		#endregion
	}
}
