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
using System.Windows.Forms;
using System.ComponentModel;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
	#region Hepler classes
	/// <summary>
	/// Represents the data of <see cref="LocationEventHandler"/>.
	/// </summary>
	public class LocationEventArgs : EventArgs
	{
		#region Members
		private Point m_location;
		private bool m_allowed;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the location.
		/// </summary>
		/// <value>The location.</value>
		public Point Location
		{
			get
			{
				return m_location;
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LocationEventArgs"/> is allowed.
		/// </summary>
		/// <value><c>true</c> if allowed; otherwise, <c>false</c>.</value>
		public bool Allowed
		{
			get
			{
				return m_allowed;
			}
			set
			{
				m_allowed = value;
			}
		}
		#endregion

		#region Constrcutor
		/// <summary>
		/// Initializes a new instance of the <see cref="LocationEventArgs"/> class.
		/// </summary>
		/// <param name="location">The location.</param>
		public LocationEventArgs(Point location)
		{
			m_location = location;
			m_allowed = true;
		}
		#endregion
	}
	#endregion

	/// <summary>
	/// Represents the the method that handles the <see cref="ChartDockControl.LocationChanging"/> event.
	/// </summary>
	public delegate void LocationEventHandler(object sender, LocationEventArgs e);

	/// <summary>
	/// Specifies behavior of <see cref="IChartDockControl"/>
	/// </summary>
	public enum ChartDockingFlags
	{
		/// <summary>
		/// Element doesn't suppport docking or moving.
		/// </summary>
		None = 0x00,
		/// <summary>
		/// Element suppports moving.
		/// </summary>
		Movable = 0x01,
		/// <summary>
		/// Element suppports docking.
		/// </summary>
		Dockable = 0x02,
		/// <summary>
		/// Element supports both behaviours.
		/// </summary>
		All = Movable | Dockable
	}
	
	/// <summary>
	/// That class that implements the basic functionality of <see cref="IChartDockControl"/> interface.
	/// </summary>
	/// <remarks>
	///	This class can be used as the host for other controls.
	/// </remarks>
	/// <example>
	/// Button button1 = new Button();<p/>
	/// button1.Text = "Button";<p/>
	/// chartControl1.DockingManager.Add( new ChartDockControl( button1 ));
	/// </example>
	/// <seealso cref="ChartDockingManager"/>
	[ToolboxItem(false)]
	public class ChartDockControl : Control, IChartDockControl
	{
		#region Members
		/// <summary>
		/// The dock position of element.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartDock m_position = ChartDock.Top;
		/// <summary>
		/// The alignment of element.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartAlignment m_alignment = ChartAlignment.Center;
		/// <summary>
		/// The orientation of element.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ChartOrientation m_orientation = ChartOrientation.Horizontal;
		/// <summary>
		/// Indicates whether element should be docked.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool m_dockingFree = false;

		private Control m_control = null;

		private SizeF m_mouseOffset = SizeF.Empty;
		private bool m_mouseIsDown = false;

		private Cursor m_tempCursor = Cursors.Default;
		private ChartDockingFlags m_behaviour = ChartDockingFlags.All;
		#endregion

		#region Events
		/// <summary>
		/// Occurs when Location is changing.
		/// </summary>
		public event LocationEventHandler LocationChanging;
		/// <summary>
		/// Occurs when Position is changed.
		/// </summary>
		public event EventHandler ChartDockChanged;
		/// <summary>
		/// Occurs when Alignment is changed.
		/// </summary>
		public event EventHandler ChartAlignmentChanged;
		#endregion

		#region Properties
		/// <summary>
		/// Specifies the docking position of the control
		/// </summary>
		[DefaultValue(ChartDock.Top)]
		[Description("Indicates the docking position of the control")]
		public virtual ChartDock Position
		{
			get
			{
				return m_position;
			}
			set
			{
				if (m_position != value)
				{
					m_position = value;
					SetOrientationByPosition();
					RaiseChartDockChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Controls the alignment of the Docked Control inside the ChartArea
		/// </summary>
		[DefaultValue(ChartAlignment.Center)]
		[Description("Indicates the alignment of control inside the Chart")]
		public virtual ChartAlignment Alignment
		{
			get
			{
				return m_alignment;
			}
			set
			{
				if (m_alignment != value)
				{
					m_alignment = value;
					RaiseChartAlignmentChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Specifies the orientation of the docked control  inside ChartArea
		/// </summary>
		[DefaultValue(ChartOrientation.Horizontal)]
		[Description("Indicates the orientation of control")]
		public virtual ChartOrientation Orientation
		{
			get
			{
				return m_orientation;
			}
			set
			{
				if ((m_orientation != value) 
					&& (m_position == ChartDock.Floating || m_dockingFree))
				{
					m_orientation = value;
				}
			}
		}
		/// <summary>
		/// Indicates if the control is docking free
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Indicates if the control should be docked inside the Chart")]
		public bool DockingFree
		{
			get
			{
				return (m_behaviour & ChartDockingFlags.Dockable) != ChartDockingFlags.Dockable;
			}
			set
			{
				bool current = (m_behaviour & ChartDockingFlags.Dockable) != ChartDockingFlags.Dockable;

				if (value != current)
				{
					if (value)
					{
						this.Behavior ^= ChartDockingFlags.Dockable;
					}
					else
					{
						this.Behavior |= ChartDockingFlags.Dockable;
					}
				}
			}
		}
		/// <summary>
		/// Get and set the docking behaviour.
		/// </summary>
		/// <value></value>
		[DefaultValue(ChartDockingFlags.All)]
		[Description("Indicates behaviour of the dock control")]
		public ChartDockingFlags Behavior
		{
			get
			{
				return m_behaviour;
			}
			set
			{
				if (m_behaviour != value)
				{
					m_behaviour = value;

					if ((m_behaviour & ChartDockingFlags.Dockable) == ChartDockingFlags.None)
					{
						this.SetOrientationByPosition();
					}

					this.RaiseChartDockChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets the name of the control.
		/// </summary>
		[Description("Gets or sets the name of the control.")
	 , DefaultValue("")]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartDockControl"/> class.
		/// </summary>
		public ChartDockControl()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartDockControl"/> class.
		/// </summary>
		/// <param name="control">The control.</param>
		public ChartDockControl(Control control)
		{
			m_control = control;

			Size = m_control.Size;
			Location = m_control.Location;

			m_control.MouseDown += new MouseEventHandler(Control_MouseDown);
			m_control.MouseUp += new MouseEventHandler(Control_MouseUp);
			m_control.MouseMove += new MouseEventHandler(Control_MouseMove);
			m_control.SizeChanged += new EventHandler(Control_SizeChanged);
			m_control.LocationChanged += new EventHandler(Control_LocationChanged);

			Controls.Add(m_control);
		}
		#endregion

		#region Helper methods
		/// <summary>
		/// Measure size of control.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public virtual SizeF Measure(SizeF size)
		{
			return this.Size;
		}
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.BringToFront();

			if ((e.Button | MouseButtons.Left) == MouseButtons.Left)
			{
				m_mouseOffset = new SizeF((float)e.X / this.Width, (float)e.Y / this.Height);
				m_mouseIsDown = true;
			}

			base.OnMouseDown(e);
		}
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (Cursor == Cursors.SizeAll && m_tempCursor != Cursors.SizeAll)
			{
				this.Cursor = m_tempCursor;
			}

			m_mouseOffset = SizeF.Empty;
			m_mouseIsDown = false;

			base.OnMouseUp(e);
		}
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if ((m_behaviour & ChartDockingFlags.Movable) == ChartDockingFlags.Movable)
			{
				if (m_mouseIsDown && Cursor != Cursors.SizeAll)
				{
					m_tempCursor = Cursor;
					this.Cursor = Cursors.SizeAll;
				}

				this.MouseMoveTo(e.X, e.Y);
			}

			base.OnMouseMove(e);
		}
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.VisibleChanged"></see> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			this.RaiseChartDockChanged(EventArgs.Empty);
			base.OnVisibleChanged(e);
		}
		/// <summary>
		/// Handles the MouseDown event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			this.OnMouseDown(e);
		}
		/// <summary>
		/// Handles the MouseUp event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			this.OnMouseUp(e);
		}
		/// <summary>
		/// Handles the MouseMove event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void Control_MouseMove(object sender, MouseEventArgs e)
		{
			this.OnMouseMove(e);
		}
		/// <summary>
		/// Handles the SizeChanged event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Control_SizeChanged(object sender, EventArgs e)
		{
			this.Size = m_control.Size;
		}
		/// <summary>
		/// Event is raised when the location is changing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Control_LocationChanged(object sender, EventArgs e)
		{
			//m_control.Location = new Point( 0, 0 );
		}
		/// <summary>
		/// This method is called during the MouseMove event
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		protected void MouseMoveTo(int x, int y)
		{
			if (m_mouseIsDown)
			{
				int oldX = (int)(this.Width * m_mouseOffset.Width);
				int oldY = (int)(this.Height * m_mouseOffset.Height);

				Point newLoc = new Point(Location.X + (x - oldX), Location.Y + (y - oldY));
				LocationEventArgs lea = new LocationEventArgs(new Point(Location.X + x, Location.Y + y));

				if (LocationChanging != null)
				{
					LocationChanging(this, lea);
				}

				if (lea.Allowed)
				{
					this.Location = this.CheckLocation(newLoc);
				}
			}
		}
		/// <summary>
		/// Specifies the orientation of the dock control based on dock position.
		/// </summary>
		protected void SetOrientationByPosition()
		{
			switch (Position)
			{
				case ChartDock.Left:
				case ChartDock.Right:
					m_orientation = ChartOrientation.Vertical;
					break;
				case ChartDock.Top:
				case ChartDock.Bottom:
					m_orientation = ChartOrientation.Horizontal;
					break;
				case ChartDock.Floating:
				default:
					break;
			}
		}
		/// <summary>
		/// Event is raised when the docking position is changed
		/// </summary>
		/// <param name="e"></param>
		private void RaiseChartDockChanged(EventArgs e)
		{
			if (ChartDockChanged != null)
			{
				ChartDockChanged(this, e);
			}
		}
		/// <summary>
		/// Event is raised when alignment is changed
		/// </summary>
		/// <param name="e"></param>
		private void RaiseChartAlignmentChanged(EventArgs e)
		{
			if (ChartAlignmentChanged != null)
			{
				ChartAlignmentChanged(this, e);
			}
		}
		/// <summary>
		/// Checks the location.
		/// </summary>
		/// <param name="pt">The pt.</param>
		/// <returns></returns>
		protected Point CheckLocation(Point pt)
		{
			if (Parent != null)
			{
				if (pt.X > Parent.Width - Width)
				{
					pt = new Point(Parent.Width - Width, pt.Y);
				}

				if (pt.Y > Parent.Height - Height)
				{
					pt = new Point(pt.X, Parent.Height - Height);
				}

				if (pt.X < 0)
				{
					pt = new Point(0, pt.Y);
				}

				if (pt.Y < 0)
				{
					pt = new Point(pt.X, 0);
				}
			}

			return pt;
		}
		#endregion
	}
}
