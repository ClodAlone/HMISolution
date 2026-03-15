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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// The BorderPanel is a control that can be used to wrap other controls inside improving its visual appearance.
	/// </summary>
	[Designer(typeof(ControlDesigner))]
	[ToolboxItem(false)]
	public class BorderPanel : Panel
	{

		#region members

		private System.Windows.Forms.Panel innerPanel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private BorderPanelState outerState = BorderPanelState.Raised;
		private BorderPanelState innerState = BorderPanelState.Sunken;
		private BorderPanelCornerSettings cornerRadius;
		private int outerBorderWidth = 10;
		private int innerBorderWidth = 8;
		private BorderPanelCornerSettings innerCornerRadius;
		private BorderPanelDimensions dimensions;
		private BrushInfo interior;
		#endregion

		#region properties
        
		[Browsable(false)]
		public new Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
		
		/// <summary>
		/// Gets / sets the background information of the border.
		/// </summary>
		[Description("Indicates the background information of the border.")]
		[Category("Appearance")]
		public BrushInfo Background
		{
			get{return interior;}
			set
			{
				if(interior!=value)
				{
					interior = value;
					Invalidate();
				}
			}
		}
		

		/// <summary>
		/// Gets / sets the color of the background of the inner surface.
		/// </summary>
		[Description("Indicates the color of the background of the inner surface.")]
		[Category("Appearance")]
		public Color InnerColor
		{
			get{return innerPanel.BackColor;}
			set
			{
				innerPanel.BackColor = value;
				Invalidate(true);
			}
		}

		protected bool ShouldSerializeInnerColor()
		{
			return InnerColor != SystemColors.Window;
		}

		protected void ResetInnerColor()
		{
			InnerColor = SystemColors.Window;
		}
		
		
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				Invalidate();
			}
		}

		protected bool ShouldSerializeText()
		{
			return Text != "";
		}

		protected new void ResetText()
		{
			Text = "";
		}

		/// <summary>
		/// Gets / sets the sizes of the sides of the border.
		/// </summary>
		[Description("Indicates the sizes of the sides of the border.")]
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BorderPanelDimensions Dimensions
		{
			get{return dimensions;}
			set
			{
				if(dimensions!=value)
				{
					dimensions = value;
					dimensions.SettingsChanged += new EventHandler(Dimensions_Changed);
				}
			}
		}

		protected bool ShouldSerializeDimensions()
		{
			return dimensions!=BorderPanelDimensions.Default;
		}

		protected void ResetDimensions()
		{
			Dimensions = BorderPanelDimensions.Default;
		}
		
		/// <summary>
		/// Gets / sets the control to be displayed inside the panel.
		/// </summary>
		[Description("The control to be displayed inside the panel.")]
		[Category("Behavior")]
		[DefaultValue(null)]
		public Control Control
		{
			get
			{
				if(innerPanel.Controls.Count==0)
				{
					return null;
				}
				else
				{
					return innerPanel.Controls[0];
				}
			}
			set
			{
				if(value == null) return;
				innerPanel.Controls.Clear();
				innerPanel.Controls.Add(value);
				value.Dock = DockStyle.Fill;
			}
		}

		/// <summary>
		/// Gets / sets the round corner radii of the inner surface.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Holds information about the round corner radiuses of the inner surface.")]
		[Category("Appearance")]
		public BorderPanelCornerSettings InnerCornerRadius
		{
			get{return innerCornerRadius;}
			set
			{
				if(innerCornerRadius!=value)
				{
					innerCornerRadius = value;
					innerCornerRadius.SettingsChanged += new EventHandler(this.CornerRadius_Changed);
					StateChanged();
				}
			}
		}

		protected bool ShouldSerializeInnerCornerRadius()
		{
			return innerCornerRadius != BorderPanelCornerSettings.Default;
		}

		protected void ResetInnerCornerRadius()
		{
			InnerCornerRadius = BorderPanelCornerSettings.Default;
		}
		
		
		/// <summary>
		/// Gets / sets the width of the border of the inner surface.
		/// </summary>
		[Description("Indicates the width of the border of the inner surface.")]
		[Category("Appearance")]
		[DefaultValue(8)]
		public int InnerBorderWidth
		{
			get{return innerBorderWidth;}
			set
			{
				if(innerBorderWidth!=value)
				{
					innerBorderWidth = value;
					StateChanged();
				}
			}
		}
		
		/// <summary>
		/// Gets / sets the width of the border.
		/// </summary>
		[Description("Indicates the width of the border.")]
		[Category("Appearance")]
		[DefaultValue(10)]
		public int OuterBorderWidth
		{
			get{return outerBorderWidth;}
			set
			{
				if(outerBorderWidth!=value)
				{
					outerBorderWidth = value;
					StateChanged();
				}
			}
		}
		
		
		/// <summary>
		/// Gets / sets the round corner radii of the outer shape.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Holds information about the round corner radiuses of the outer shape.")]
		[Category("Appearance")]
		public BorderPanelCornerSettings CornerRadius
		{
			get{return cornerRadius;}
			set
			{
				if(cornerRadius!=value)
				{
					cornerRadius = value;
					cornerRadius.SettingsChanged += new EventHandler(CornerRadius_Changed);
				}
			}
		}
		

		protected bool ShouldSerializeCornerRadius()
		{
			return cornerRadius != BorderPanelCornerSettings.Default;
		}

		protected void ResetCornerRadius()
		{
			CornerRadius = BorderPanelCornerSettings.Default;
		}
		
		/// <summary>
		/// Gets / sets the height state of the inner surface.
		/// </summary>
		[Description("Indicates the height state of the inner surface.")]
		[Category("Appearance")]
		[DefaultValue(BorderPanelState.Sunken)]
		public BorderPanelState InnerState
		{
			get{return innerState;}
			set
			{
				if(innerState!=value)
				{
					innerState = value;
					StateChanged();
				}
			}
		}
		
		/// <summary>
		/// Gets / sets the height state of the outer surface.
		/// </summary>
		[Description("Indicates the height state of the outer surface.")]
		[Category("Appearance")]
		[DefaultValue(BorderPanelState.Raised)]
		public BorderPanelState OuterState
		{
			get{return outerState;}
			set
			{
				if(outerState!=value)
				{
					outerState = value;
					StateChanged();
				}
			}
		}
		
#endregion

		#region implementation
		
		public BorderPanel()
		{
			// This call is required by the Windows.Forms Form Designer.
			this.cornerRadius = new BorderPanelCornerSettings(20);
			cornerRadius.SettingsChanged += new EventHandler(this.CornerRadius_Changed);
		
			this.innerCornerRadius = new BorderPanelCornerSettings(10);
			innerCornerRadius.SettingsChanged += new EventHandler(this.CornerRadius_Changed);
		
			dimensions = new BorderPanelDimensions();
			dimensions.SettingsChanged += new EventHandler(Dimensions_Changed);
		
			interior = new BrushInfo(BackColor);
		
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | WhidbeyCompatibleControlStyles.DoubleBuffer,true);
		
			InitializeComponent();
		
			// TODO: Add any initialization after the InitForm call
		
		}
		
		/// <summary> 
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		
				#region Component Designer generated code
		/// <summary> 
		/// Required method for designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.innerPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// innerPanel
			// 
			this.innerPanel.BackColor = System.Drawing.SystemColors.Window;
			this.innerPanel.Location = new System.Drawing.Point(24, 24);
			this.innerPanel.Name = "innerPanel";
			this.innerPanel.Size = new System.Drawing.Size(272, 208);
			this.innerPanel.TabIndex = 0;
			// 
			// BorderPanel
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.innerPanel});
			this.Size = new System.Drawing.Size(320, 256);
			this.ResumeLayout(false);

		}
				#endregion
		
		private void StateChanged()
		{
			Invalidate(true);
		}
		
		private void CornerRadius_Changed(object sender,EventArgs e)
		{
			CalculateRegion();
		}
		
		private void Dimensions_Changed(object sender,EventArgs e)
		{
			UpdateSize();
		}
		
		private void UpdateSize()
		{
			innerPanel.Location = new Point(dimensions.Left,dimensions.Top);
			innerPanel.Size = new Size(Width-dimensions.Left-dimensions.Right,Height-dimensions.Top-dimensions.Bottom);
			Invalidate(true);
		}
		
		private Region GetRegion(Rectangle rect,BorderPanelCornerSettings cornerRadius)
		{
			int Width = rect.Width;
			int Height = rect.Height;
			GraphicsPath gp = new GraphicsPath();
			gp.AddLine(cornerRadius.TopLeft,0,Width-cornerRadius.TopRight,0);
			if(cornerRadius.TopRight != 0)
			{
				gp.AddArc(Width-cornerRadius.TopRight*2,0,cornerRadius.TopRight*2,cornerRadius.TopRight*2,-90,90);
			}
			gp.AddLine(Width,cornerRadius.TopRight,Width,Height-cornerRadius.BottomRight);
			if(cornerRadius.BottomRight!=0)
			{
				gp.AddArc(Width-cornerRadius.BottomRight*2,Height-cornerRadius.BottomRight*2,cornerRadius.BottomRight*2,cornerRadius.BottomRight*2,0,90);
			}
			gp.AddLine(Width-cornerRadius.BottomRight,Height,cornerRadius.BottomLeft,Height);
			if(cornerRadius.BottomLeft!=0)
			{
				gp.AddArc(0,Height-cornerRadius.BottomLeft*2,cornerRadius.BottomLeft*2,cornerRadius.BottomLeft*2,90,90);
			}
			gp.AddLine(0,Height-cornerRadius.BottomLeft,0,cornerRadius.TopLeft);
			if(cornerRadius.TopLeft!=0)
			{
				gp.AddArc(0,0,cornerRadius.TopLeft*2,cornerRadius.TopLeft*2,180,90);
			}
		
			gp.CloseAllFigures();
			Region rgn = new Region(gp);
			gp.Dispose();
			return rgn;
		}
		private void CalculateRegion()
		{
			this.Region = GetRegion(ClientRectangle,cornerRadius);
			Invalidate(true);
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			BrushPaint.FillRectangle(e.Graphics,ClientRectangle,interior);
			Color BackColor = interior.BackColor;
			//			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			//Draw the outer border
			if(outerState != BorderPanelState.Level && this.outerBorderWidth >0)
			{
					
				int ow = this.outerBorderWidth;
				Color topLeftColor = Color.FromArgb(175,255,255,255);//ControlPaint.LightLight(BackColor);
				Color bottomRightColor = Color.FromArgb(125,0,0,0);//ControlPaint.Dark(BackColor);
				if(outerState == BorderPanelState.Sunken)
				{
					topLeftColor = Color.FromArgb(125,0,0,0);//ControlPaint.Dark(BackColor);
					bottomRightColor = Color.FromArgb(175,255,255,255);//ControlPaint.LightLight(BackColor);
				}
		
				//Draw the right side.
				LinearGradientBrush rightBrush = new LinearGradientBrush(new Point(Width-ow,0),new Point(Width,0),Color.Transparent,bottomRightColor);
				e.Graphics.FillRectangle(rightBrush,Width-ow,0,ow,Height);
		
				//Draw the bottom side.
				LinearGradientBrush bottomBrush = new LinearGradientBrush(new Point(0,Height-ow),new Point(0,Height),Color.Transparent,bottomRightColor);
				e.Graphics.FillRectangle(bottomBrush,0,Height-ow,Width,ow);
		
		
				//Draw the left side.
				LinearGradientBrush leftBrush = new LinearGradientBrush(new Point(0,0),new Point(ow,0),topLeftColor,Color.Transparent);
				e.Graphics.FillRectangle(leftBrush,0,0,ow,Height);
		
				//Draw the top side.
				LinearGradientBrush topBrush = new LinearGradientBrush(new Point(0,0),new Point(0,ow),topLeftColor,Color.Transparent);
				e.Graphics.FillRectangle(topBrush,0,0,Width,ow);
		
				leftBrush.Dispose();
				rightBrush.Dispose();
				topBrush.Dispose();
				bottomBrush.Dispose();
		
			}
		
			//Draw the inner border.
			if(innerState != BorderPanelState.Level && this.innerBorderWidth >0)
			{
				int iw = this.innerBorderWidth;
				Rectangle rc = innerPanel.Bounds;
				rc.Inflate(iw*2,iw*2);
				Region rgn = GetRegion(rc,innerCornerRadius);
				rgn.Translate(rc.X,rc.Y);
				using (Brush brush = new SolidBrush(innerPanel.BackColor))
					e.Graphics.FillRegion(brush, rgn);
		
				e.Graphics.SetClip(rgn,CombineMode.Replace);
				e.Graphics.TranslateTransform(rc.X,rc.Y);
				int Width = rc.Width;
				int Height = rc.Height;
		
				Color topLeftColor = Color.FromArgb(175,255,255,255);//ControlPaint.LightLight(innerPanel.BackColor);
				Color bottomRightColor = Color.FromArgb(100,0,0,0);;//ControlPaint.Dark(innerPanel.BackColor);
				if(InnerState == BorderPanelState.Sunken)
				{
					topLeftColor = Color.FromArgb(100,0,0,0);//ControlPaint.Dark(innerPanel.BackColor);
					bottomRightColor = Color.FromArgb(175,255,255,255);//ControlPaint.LightLight(innerPanel.BackColor);
				}
		
				//Draw the left side.
				LinearGradientBrush leftBrush = new LinearGradientBrush(new Point(0,0),new Point(iw,0),topLeftColor,Color.Transparent);
				e.Graphics.FillRectangle(leftBrush,0,0,iw,Height);
		
				//Draw the top side.
				LinearGradientBrush topBrush = new LinearGradientBrush(new Point(0,0),new Point(0,iw),topLeftColor,Color.Transparent);
				e.Graphics.FillRectangle(topBrush,0,0,Width,iw);
		
				//Draw the right side.
				LinearGradientBrush rightBrush = new LinearGradientBrush(new Point(Width-iw,0),new Point(Width,0),Color.Transparent,bottomRightColor);
				e.Graphics.FillRectangle(rightBrush,Width-iw,0,iw,Height);
		
				//Draw the bottom side.
				LinearGradientBrush bottomBrush = new LinearGradientBrush(new Point(0,Height-iw),new Point(0,Height),Color.Transparent,bottomRightColor);
				e.Graphics.FillRectangle(bottomBrush,0,Height-iw,Width,iw);
		
				leftBrush.Dispose();
				topBrush.Dispose();
				rightBrush.Dispose();
				bottomBrush.Dispose();
		
				e.Graphics.ResetTransform();
				e.Graphics.ResetClip();
			}
		
			Size sz = e.Graphics.MeasureString(Text,Font).ToSize();
			Brush br = new SolidBrush(ForeColor);
			e.Graphics.DrawString(Text,Font,br,(this.Width-sz.Width)/2,(dimensions.Top-innerBorderWidth-sz.Height)/2);
			br.Dispose();
		}
		
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			CalculateRegion();
			UpdateSize();
		}
		#endregion
		
	}

	/// <summary>
	/// Indicates the states of the two borders of the BorderPanel.
	/// </summary>
	public enum BorderPanelState
	{
		Level,
		Raised,
		Sunken
	}

	/// <summary>
	/// Holds information about the rounded corners of the BorderPanel.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BorderPanelCornerSettings
	{
		private int topLeft;
		private int topRight;
		private int bottomLeft;
		private int bottomRight;
		
		public event EventHandler SettingsChanged;

		protected virtual void OnSettingsChanged(EventArgs e)
		{
			if(SettingsChanged!=null)
			{
				SettingsChanged(this,e);
			}
		}

		/// <summary>
		/// The default settings of the panel.
		/// </summary>
		public static BorderPanelCornerSettings Default
		{
			get{return new BorderPanelCornerSettings();}
		}

		public static bool operator ==(BorderPanelCornerSettings c1,BorderPanelCornerSettings c2)
		{
			return c1.TopLeft == c2.TopLeft && c1.TopRight == c2.TopRight && c1.BottomLeft == c2.BottomLeft && c1.BottomRight == c2.BottomRight;
		}
		public static bool operator !=(BorderPanelCornerSettings c1,BorderPanelCornerSettings c2)
		{
			return !(c1==c2);
		}

		public override bool Equals(object o)
		{
			if(o is BorderPanelCornerSettings)
			{
				return this == (BorderPanelCornerSettings)o;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Gets / sets the BottomRight round corner radius.
		/// </summary>
		[Description("Indicates the BottomRight round corner radius.")]
		public int BottomRight
		{
			get{return bottomRight;}
			set
			{
				if(bottomRight!=value)
				{
					bottomRight = value;
					StateChanged();
				}
			}
		}
		
		
		/// <summary>
		/// Gets / sets the BottomLeft round corner radius.
		/// </summary>
		[Description("Indicates the BottomLeft round corner radius.")]
		public int BottomLeft
		{
			get{return bottomLeft;}
			set
			{
				if(bottomLeft!=value)
				{
					bottomLeft = value;
					StateChanged();
				}
			}
		}
		
		
		/// <summary>
		/// Gets / sets the TopRight round corner radius.
		/// </summary>
		[Description("Indicates the TopRight round corner radius.")]
		public int TopRight
		{
			get{return topRight;}
			set
			{
				if(topRight!=value)
				{
					topRight = value;
					StateChanged();
				}
			}
		}
		
		
		/// <summary>
		/// Gets / sets the TopLeft round corner radius.
		/// </summary>
		[Description("Indicates the TopLeft round corner radius.")]
		public int TopLeft
		{
			get{return topLeft;}
			set
			{
				if(topLeft!=value)
				{
					topLeft = value;
					StateChanged();
				}
			}
		}

		/// <summary>
		/// Gets / sets all the other values to the specified value.
		/// </summary>
		[Description("Sets all the other values to the specified value.")]
		public int All
		{
			get{return topLeft;}
			set
			{
				topLeft = value;
				topRight = value;
				bottomLeft = value;
				bottomRight = value;
				StateChanged();
			}
		}

		private void StateChanged()
		{
			OnSettingsChanged(EventArgs.Empty);
		}
		
		public BorderPanelCornerSettings(int topLeft,int topRight,int bottomLeft,int bottomRight)
		{
			this.topLeft = topLeft;
			this.topRight = topRight;
			this.bottomLeft = bottomLeft;
			this.bottomRight = bottomRight;
		}

		public BorderPanelCornerSettings():this(20,20,20,20)
		{

		}

		public BorderPanelCornerSettings(int all):this(all,all,all,all)
		{
		}
	}

	/// <summary>
	/// Holds information about the sides of the borders of the BorderPanel.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BorderPanelDimensions
	{
		private int top;
		private int left;
		private int right;
		private int bottom;
		
		public event EventHandler SettingsChanged;

		protected virtual void OnSettingsChanged(EventArgs e)
		{
			if(SettingsChanged!=null)
			{
				SettingsChanged(this,e);
			}
		}

		/// <summary>
		/// The default dimension settings.
		/// </summary>
		public static BorderPanelDimensions Default
		{
			get{return new BorderPanelDimensions();}
		}

		public static bool operator ==(BorderPanelDimensions dim1,BorderPanelDimensions dim2)
		{
			return dim1.Top == dim2.Top && dim1.Left == dim2.Left && dim1.Right == dim2.Right && dim1.Bottom == dim2.Bottom;
		}
		public static bool operator !=(BorderPanelDimensions dim1,BorderPanelDimensions dim2)
		{
			return !(dim1==dim2);
		}

		public override bool Equals(object o)
		{
			if(o is BorderPanelDimensions)
			{
				return this == (BorderPanelDimensions)o;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Gets / sets all the other members to the specified value.
		/// </summary>
		[Description("Sets all the other members to the specified value.")]
		public int All
		{
			get
			{
				return left;
			}
			set
			{
				top = left = right = bottom = value;
				OnSettingsChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets / sets the bottom border side's width.
		/// </summary>
		[Description("Indicates the bottom border side's width.")]
		public int Bottom
		{
			get{return bottom;}
			set
			{
				if(bottom!=value)
				{
					bottom = value;
					OnSettingsChanged(EventArgs.Empty);
				}
			}
		}
		
		
		/// <summary>
		/// Gets / sets the right border side's width.
		/// </summary>
		[Description("Indicates the right border side's width.")]
		public int Right
		{
			get{return right;}
			set
			{
				if(right!=value)
				{
					right = value;
					OnSettingsChanged(EventArgs.Empty);
				}
			}
		}
		
		
		/// <summary>
		/// Gets / sets the left border side's width.
		/// </summary>
		[Description("Indicates the left border side's width.")]
		public int Left
		{
			get{return left;}
			set
			{
				if(left!=value)
				{
					left = value;
					OnSettingsChanged(EventArgs.Empty);
				}
			}
		}
		
		/// <summary>
		/// Gets / sets the top border side's width.
		/// </summary>
		[Description("Indicates the top border side's width.")]
		public int Top
		{
			get{return top;}
			set
			{
				if(top!=value)
				{
					top = value;
					OnSettingsChanged(EventArgs.Empty);
				}
			}
		}
		
		public BorderPanelDimensions(int all):this(all,all,all,all)
		{
		}
		public BorderPanelDimensions():this(30,40,30,30)
		{
		}
		public BorderPanelDimensions(int left,int top, int right,int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}
	}
}