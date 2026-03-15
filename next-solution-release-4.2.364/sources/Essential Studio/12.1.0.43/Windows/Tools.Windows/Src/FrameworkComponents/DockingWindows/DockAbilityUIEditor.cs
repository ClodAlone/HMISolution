#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Editor for DockAbility extended property of controls in DockingManager.
	/// </summary>
	public sealed class DockAbilityEditor : UITypeEditor
	{
		#region Members

		private DockAbilityUI m_DockAbilityUI;

		#endregion

		#region Constructors

		public DockAbilityEditor()
		{
		}


		#endregion

		#region Overrides

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

				if (service == null)
				{
					return value;
				}

				if (this.m_DockAbilityUI == null)
				{
					this.m_DockAbilityUI = new DockAbilityEditor.DockAbilityUI();
				}

				this.m_DockAbilityUI.Start( value);
				service.DropDownControl(this.m_DockAbilityUI);
				value = this.m_DockAbilityUI.Value;
				this.m_DockAbilityUI.End();
			}
			return value;
		}


		#endregion

		[ToolboxItem(false)]
		private class DockAbilityUI : Control
		{
			#region Members

			private InternalArrowControl m_left;
			private InternalArrowControl m_top;
			private InternalArrowControl m_right;
			private InternalArrowControl m_bottom;
			private InternalArrowControl m_tabbed;

			private Pen m_LightPen;
			private Pen m_DarkPen;

			#endregion

			#region Constructors

			public DockAbilityUI()
			{
				m_LightPen = new Pen( DEF_BORDER_LIGHT_COLOR );
				m_DarkPen = new Pen( DEF_BORDER_DARK_COLOR );

				this.Size = new Size( DEF_CONTROL_SIZE, DEF_CONTROL_SIZE );
			}


			#endregion

			#region Methods

			private void InitializeInnerControls()
			{
				m_left.Bounds = new Rectangle( DEF_NEAR, DEF_MIDDLE, DEF_INNER_CONTROL_SIZE, DEF_INNER_CONTROL_SIZE );
				m_top.Bounds = new Rectangle( DEF_MIDDLE, DEF_NEAR, DEF_INNER_CONTROL_SIZE, DEF_INNER_CONTROL_SIZE );
				m_right.Bounds = new Rectangle( DEF_FAR, DEF_MIDDLE, DEF_INNER_CONTROL_SIZE, DEF_INNER_CONTROL_SIZE );
				m_bottom.Bounds = new Rectangle( DEF_MIDDLE, DEF_FAR, DEF_INNER_CONTROL_SIZE, DEF_INNER_CONTROL_SIZE );
				m_tabbed.Bounds = new Rectangle( DEF_MIDDLE, DEF_MIDDLE, DEF_INNER_CONTROL_SIZE, DEF_INNER_CONTROL_SIZE );
			}

			public void End()
			{
				m_left.Dispose();
				m_top.Dispose();
				m_right.Dispose();
				m_bottom.Dispose();
				m_tabbed.Dispose();
			}

			public void Start( object strvalue )
			{
                object value = Enum.Parse(typeof(DockAbility), strvalue as string);
				m_left = new InternalArrowControl( DockingStyle.Left, 
					((DockAbility)value | DockAbility.Left) == (DockAbility)value );
				m_top = new InternalArrowControl( DockingStyle.Top, 
					((DockAbility)value | DockAbility.Top) == (DockAbility)value );
				m_right = new InternalArrowControl( DockingStyle.Right, 
					((DockAbility)value | DockAbility.Right) == (DockAbility)value );
				m_bottom = new InternalArrowControl( DockingStyle.Bottom, 
					((DockAbility)value | DockAbility.Bottom) == (DockAbility)value );
				m_tabbed = new InternalArrowControl( DockingStyle.Tabbed, 
					((DockAbility)value | DockAbility.Tabbed) == (DockAbility)value );

				this.Controls.Add( m_left );
				this.Controls.Add( m_top );
				this.Controls.Add( m_right );
				this.Controls.Add( m_bottom );
				this.Controls.Add( m_tabbed );

				InitializeInnerControls();
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				base.OnPaint (e);

				Rectangle rcBounds = Bounds;
				for( int indent = 1; indent < DEF_FRAME_WIDTH; indent++ )
				{
					rcBounds.Inflate(-1, -1);
					if( indent < DEF_OUTER_FRAME_WIDTH )
					{
						e.Graphics.DrawRectangle( m_DarkPen, rcBounds );
					}
					else
					{
						e.Graphics.DrawRectangle( m_LightPen, rcBounds );
					}
				}
			}


			#endregion

			#region Properties

			public object Value
			{
				get
				{
					DockAbility result = DockAbility.None;

					if( m_left.Checked ) { result |= DockAbility.Left; }
					if( m_top.Checked ) { result |= DockAbility.Top; }
					if( m_right.Checked ) { result |= DockAbility.Right; }
					if( m_bottom.Checked ) { result |= DockAbility.Bottom; }
					if( m_tabbed.Checked ) { result |= DockAbility.Tabbed; }

					return result.ToString();
				}
			}


			#endregion

			#region Constants

			private const int DEF_CONTROL_SIZE = 88;
			private const int DEF_INNER_CONTROL_SIZE = 24;
			private const int DEF_NEAR = 4;
			private const int DEF_MIDDLE = 32;
			private const int DEF_FAR = 60;
			private const int DEF_OUTER_FRAME_WIDTH = 2;
			private const int DEF_FRAME_WIDTH = 4;
			private Color DEF_BORDER_LIGHT_COLOR = Color.FromArgb( 225, 225, 225 );
			private Color DEF_BORDER_DARK_COLOR = Color.FromArgb( 200, 200, 200 );

			#endregion

			private class InternalArrowControl : Control
			{
				#region Members

				private DockingStyle m_style;
				private bool m_Checked;
				private Brush m_ActiveBrush;
				private Brush m_InactiveBrush;

				#endregion

				#region Constructors

				public InternalArrowControl( DockingStyle style, bool bChecked )
				{
					this.m_style = style;
					this.m_Checked = bChecked;
					this.m_ActiveBrush = new SolidBrush( DEF_ACTIVE_COLOR );
					this.m_InactiveBrush = new SolidBrush( DEF_INACTIVE_COLOR );
				}


				#endregion

				#region Properties

				public bool Checked
				{
					get { return m_Checked; }
				}

				public DockingStyle Style
				{
					get
					{
						return m_style;
					}
				}

				public Brush InternalBrush
				{
					get
					{
						if( Checked )
						{
							return m_ActiveBrush;
						}
						else
						{
							return m_InactiveBrush;
						}
					}
				}

				#endregion

				#region Overrides

				protected override void OnClick(EventArgs e)
				{
					base.OnClick (e);

					this.m_Checked = !this.m_Checked;
					Invalidate();
				}

				protected override void OnPaint(PaintEventArgs e)
				{
					base.OnPaint (e);

					if( Style == DockingStyle.Tabbed )
					{
						e.Graphics.FillRectangle( InternalBrush, DEF_NEAR, DEF_NEAR, DEF_RECT_SIZE, DEF_RECT_SIZE );
					}
					else
					{
						Point[] points = new Point[3];
						switch( Style )
						{
							case DockingStyle.Left:
								points[0] = new Point(DEF_NEAR, DEF_MIDDLE);
								points[1] = new Point(DEF_FAR, DEF_NEAR);
								points[2] = new Point(DEF_FAR, DEF_FAR);
								break;
							case DockingStyle.Top:
								points[0] = new Point(DEF_NEAR, DEF_FAR);
								points[1] = new Point(DEF_MIDDLE, DEF_NEAR);
								points[2] = new Point(DEF_FAR, DEF_FAR);
								break;
							case DockingStyle.Right:
								points[0] = new Point(DEF_NEAR, DEF_NEAR);
								points[1] = new Point(DEF_NEAR, DEF_FAR);
								points[2] = new Point(DEF_FAR, DEF_MIDDLE);
								break;
							case DockingStyle.Bottom:
								points[0] = new Point(DEF_NEAR, DEF_NEAR);
								points[1] = new Point(DEF_FAR, DEF_NEAR);
								points[2] = new Point(DEF_MIDDLE, DEF_FAR);
								break;
						}
						e.Graphics.FillPolygon( InternalBrush, points );
					}
				}


				#endregion

				#region Constants

				private const int DEF_NEAR = 4;
				private const int DEF_MIDDLE = 12;
				private const int DEF_FAR = 20;
				private const int DEF_RECT_SIZE = 16;
				private Color DEF_ACTIVE_COLOR = Color.FromArgb( 77, 79, 170 );
				private Color DEF_INACTIVE_COLOR = Color.FromArgb( 177, 179, 170 );

				#endregion

			}
		}

	}
}
