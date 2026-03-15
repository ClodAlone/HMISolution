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
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Specifies how the chart elements will be arranged.
	/// </summary>
	public enum ChartLayoutMode
	{
		/// <summary>
		/// The elements will be stacked.
		/// </summary>
		Stack,
		/// <summary>
		/// The elements will be wrapped.
		/// </summary>
		Wrap
	}

	/// <summary>
	/// ChartDockingManager provides docking feature of chart elements (Legends, Titles, ToolBar...).
	/// </summary>
	public sealed class ChartDockingManager : IDisposable
	{
		#region Internal types
		/// <summary>
		/// Implements the wraping of elements.
		/// </summary>
		class WrapLayouter
		{
			#region Members
			private List<int> m_lines = new List<int>();
			private List<IChartDockControl> m_elements = new List<IChartDockControl>();
			private int m_dimension = 0;
			private int m_spacing = 0;
			private bool m_isVertical = false;
			#endregion

			#region Properties
			/// <summary>
			/// Gets the elements.
			/// </summary>
			/// <value>The elements.</value>
			public List<IChartDockControl> Elements
			{
				get { return m_elements; }
			}
			/// <summary>
			/// Gets or sets the dimension.
			/// </summary>
			/// <value>The dimension.</value>
			public int Dimension
			{
				get { return m_dimension; }
				set { m_dimension = value; }
			}
			#endregion

			#region Constructor
			/// <summary>
			/// Initializes a new instance of the <see cref="WrapLayouter"/> class.
			/// </summary>
			/// <param name="isVertical">if set to <c>true</c> [is vertical].</param>
			/// <param name="spacing">The spacing.</param>
			public WrapLayouter(bool isVertical, int spacing)
			{
				m_isVertical = isVertical;
				m_spacing = spacing;
			}
			#endregion

			#region Implementation
			/// <summary>
			/// Measures elements by the specified size.
			/// </summary>
			/// <param name="measureSize">The maximal size.</param>
			/// <returns></returns>
			public int Measure(Size measureSize)
			{
				int currentSize = 0;
				int currentPosition = 0;
				int length = m_isVertical ? measureSize.Height : measureSize.Width;

				m_dimension = 0;
				m_lines.Clear();

				foreach (IChartDockControl element in m_elements)
				{
					Size size = Size.Round(element.Measure(measureSize));

					if (m_isVertical)
					{
						size = new Size(size.Height, size.Width);
					}

					if (currentPosition + size.Width > length)
					{
						currentPosition = m_spacing + size.Width;
						m_dimension += currentSize + m_spacing;
						m_lines.Add(currentSize);
						currentSize = 0;
					}
					else
					{
						currentPosition += m_spacing + size.Width;
					}

					currentSize = Math.Max(currentSize, size.Height);
				}

				if (currentSize != 0)
				{
					m_lines.Add(currentSize);
					m_dimension += currentSize + m_spacing;
				}

				return m_dimension;
			}
			/// <summary>
			/// Arranges elements by the specified rect.
			/// </summary>
			/// <param name="rect">The rect.</param>
			public void Arrange(Rectangle rect)
			{
				if (m_lines.Count > 0 && rect.Height > 0 && rect.Width > 0)
				{
					int totalLength = m_isVertical ? rect.Height : rect.Width;

					int position = 0;
					int lineOffset = 0;
					int lineIndex = 0;

					for (int i = 0; i < m_elements.Count; i++ )
					{
						IChartDockControl element = m_elements[i];

						int length = m_isVertical ? element.Size.Height : element.Size.Width;

						if (position + length > totalLength)
						{
							lineOffset += m_lines[lineIndex++] + m_spacing;
							position = 0;
						}

						if (m_isVertical)
						{
							element.Location = new Point(rect.X + lineOffset, rect.Y + position);
						}
						else
						{
							element.Location = new Point(rect.X + position, rect.Y + lineOffset);
						}

						position += length + m_spacing;
					}
				}
			}
			#endregion
		}
		#endregion

		#region Members
		private Control m_host = null;
		private List<IChartDockControl> m_elements = new List<IChartDockControl>();

		private bool m_dockAlignment = false;
		private bool m_allIsSet = true;
		private bool m_supressEventFiring = false;
		private bool m_supressEventProcessing = false;

		private int m_spacing = 10;

		private Rectangle m_outsideRect = Rectangle.Empty;
		private Rectangle m_insideRect;
		private Size m_mouseOffset = Size.Empty;

		private IChartDockControl m_activeElement = null;
		private ChartPlacement m_placement = ChartPlacement.Outside;
		private ChartLayoutMode m_layoutMode = ChartLayoutMode.Stack;
		#endregion

		#region Events
		/// <summary>
		/// Event is raised when the size of the docking manager is changed
		/// </summary>
		public event EventHandler SizeChanged;
		#endregion

		#region Proprties
		/// <summary>
		/// Indicates space between chart elements.
		/// </summary>
		[DefaultValue(10)]
		public int Spacing
		{
			get
			{
				return m_spacing;
			}
			set
			{
				m_spacing = value;
			}
		}
		/// <summary>
		/// If it's enable, user can set using mouse alignment for controls.
		/// </summary>
		[DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
		public bool DockAlignment
		{
			get
			{
				return m_dockAlignment;
			}
			set
			{
				if (m_dockAlignment != value)
				{
					m_dockAlignment = value;
				}
			}
		}
		/// <summary>
		/// Determines if the docked element is placed inside or outside the host.
		/// </summary>
		/// <value>The placement.</value>
		[DefaultValue(ChartPlacement.Outside)]
		public ChartPlacement Placement
		{
			get { return m_placement; }
			set
			{
				if (m_placement != value)
				{
					m_placement = value;
					this.RaiseSizeChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets the layout mode.
		/// </summary>
		/// <value>The layout mode.</value>
		[DefaultValue(ChartLayoutMode.Stack)]
		public ChartLayoutMode LayoutMode
		{
			get { return m_layoutMode; }
			set { m_layoutMode = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartDockingManager"/> class.
		/// </summary>
		public ChartDockingManager()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartDockingManager"/> class.
		/// </summary>
		/// <param name="host">The host.</param>
		public ChartDockingManager(Control host)
		{
			m_host = host;
			m_host.MouseMove += new MouseEventHandler(this.OnHostMouseMove);
			m_host.MouseDown += new MouseEventHandler(this.OnHostMouseDown);
			m_host.MouseUp += new MouseEventHandler(this.OnHostMouseUp);
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method prevents manager from any processing and firing of events.
		/// </summary>
		public void Freeze()
		{
			m_supressEventFiring = true;
			m_supressEventProcessing = true;
		}
		/// <summary>
		/// This method restores the original state of manager (before freezing).
		/// </summary>
		public void Melt()
		{
			m_supressEventFiring = false;
			m_supressEventProcessing = false;
		}

		/// <summary>
		/// Adds the specified control to the ChartDockingManager
		/// </summary>
		/// <param name="control">The control.</param>
		public void Add(IChartDockControl control)
		{
			if (control != null)
			{
				m_elements.Add(control);

				//control.LocationChanged += new EventHandler(Control_LocationChanged);
				control.LocationChanging += new LocationEventHandler(Control_LocationChanging);
				control.ChartDockChanged += new EventHandler(Control_ChartDockChanged);
				control.ChartAlignmentChanged += new EventHandler(Control_ChartAlignmentChanged);
				control.SizeChanged += new EventHandler(Control_SizeChanged);
			}

			this.RaiseSizeChanged(EventArgs.Empty);
		}
		/// <summary>
		/// Removes the specified control from the Docking manager.
		/// </summary>
		/// <param name="control">The control.</param>
		public void Remove(IChartDockControl control)
		{
			if (control != null)
			{
				control.LocationChanging -= new LocationEventHandler(Control_LocationChanging);
				control.ChartDockChanged -= new EventHandler(Control_ChartDockChanged);
				control.ChartAlignmentChanged -= new EventHandler(Control_ChartAlignmentChanged);
				//control.LocationChanged -= new EventHandler(Control_LocationChanged);
				control.SizeChanged -= new EventHandler(Control_SizeChanged);
			}

			m_elements.Remove(control);
			this.RaiseSizeChanged(EventArgs.Empty);
		}
		/// <summary>
		/// Returns the size of the specified rectangle.
		/// </summary>
		/// <param name="rect">The bounds of host element.</param>
		/// <returns></returns>
		public Rectangle DoLayout(Rectangle rect)
		{
			m_allIsSet = false;

			Rectangle vRect = rect;
			vRect.Inflate(-m_spacing, -m_spacing);

			m_outsideRect = rect;
			m_insideRect = vRect;

			if (m_layoutMode == ChartLayoutMode.Wrap)
			{
				WrapLayouter topLayouter = new WrapLayouter(false, m_spacing);
				WrapLayouter leftLayouter = new WrapLayouter(true, m_spacing);
				WrapLayouter rightLayouter = new WrapLayouter(true, m_spacing);
				WrapLayouter bottomLayouter = new WrapLayouter(false, m_spacing);

				foreach (IChartDockControl element in m_elements)
				{
					if (element.Visible)
					{
						switch (element.Position)
						{
							case ChartDock.Left:
								leftLayouter.Elements.Add(element);
								break;
							case ChartDock.Right:
								rightLayouter.Elements.Add(element);
								break;
							case ChartDock.Top:
								topLayouter.Elements.Add(element);
								break;
							case ChartDock.Bottom:
								bottomLayouter.Elements.Add(element);
								break;
							case ChartDock.Floating:
								element.Measure(rect.Size);
								break;
						}
					}
				}

				int dimLeft = leftLayouter.Measure(GetMeasureSize(ChartDock.Left, m_insideRect.Size));
				int dimRight = rightLayouter.Measure(GetMeasureSize(ChartDock.Right, m_insideRect.Size));

				m_insideRect.X += dimLeft;
				m_insideRect.Width -= dimLeft + dimRight;

				int dimTop = topLayouter.Measure(GetMeasureSize(ChartDock.Top, m_insideRect.Size));
				int dimBottom = bottomLayouter.Measure(GetMeasureSize(ChartDock.Bottom, m_insideRect.Size));

				m_insideRect.Y += dimTop;
				m_insideRect.Height -= dimTop + dimBottom;

				leftLayouter.Arrange(new Rectangle(vRect.Left, 
					vRect.Top, dimLeft, vRect.Height));
				rightLayouter.Arrange(new Rectangle(vRect.Right - dimRight + m_spacing, 
					vRect.Top, dimRight, vRect.Height));

				topLayouter.Arrange(new Rectangle(vRect.Left + dimLeft,
					vRect.Top, vRect.Width - dimLeft - dimRight, dimTop));
				bottomLayouter.Arrange(new Rectangle(vRect.Left + dimLeft,
					vRect.Bottom - dimBottom + m_spacing, vRect.Width - dimLeft - dimRight, dimBottom));
			}
			else
			{
				#region Calculate size
				foreach (IChartDockControl element in m_elements)
				{
					Size sz = element.Measure(GetMeasureSize(element.Position, m_insideRect.Size)).ToSize();

					if (element.Visible 
						&& (element.Behavior & ChartDockingFlags.Dockable) == ChartDockingFlags.Dockable )
					{
						switch (element.Position)
						{
							case ChartDock.Bottom:
								m_insideRect.Height -= sz.Height + m_spacing;
								break;
							case ChartDock.Left:
								m_insideRect.X += sz.Width + m_spacing;
								m_insideRect.Width -= sz.Width + m_spacing;
								break;
							case ChartDock.Right:
								m_insideRect.Width -= sz.Width + m_spacing;
								break;
							case ChartDock.Top:
								m_insideRect.Y += sz.Height + m_spacing;
								m_insideRect.Height -= sz.Height + m_spacing;
								break;
						}
					}
				}
				#endregion

				#region Set location
				foreach (IChartDockControl element in m_elements)
				{
					Size sz = element.Size;

					if (element.Visible 
						&& (element.Behavior & ChartDockingFlags.Dockable) == ChartDockingFlags.Dockable)
					{
						switch (element.Position)
						{
							case ChartDock.Left:
								SetToCenter(element, new Rectangle(rect.Left + m_spacing, m_insideRect.Top,
									sz.Width, m_insideRect.Height));
								rect.X += sz.Width + m_spacing;
								rect.Width -= sz.Width + m_spacing;
								break;

							case ChartDock.Bottom:
								SetToCenter(element, new Rectangle(vRect.Left, rect.Bottom - sz.Height - m_spacing,
									vRect.Width, sz.Height));
								rect.Height -= sz.Height + m_spacing;
								break;

							case ChartDock.Right:
								SetToCenter(element, new Rectangle(rect.Right - sz.Width - m_spacing, m_insideRect.Top,
									sz.Width, m_insideRect.Height));
								rect.Width -= sz.Width + m_spacing;
								break;

							case ChartDock.Top:
								SetToCenter(element, new Rectangle(vRect.Left, rect.Top + m_spacing,
									vRect.Width, sz.Height));
								rect.Y += sz.Height + m_spacing;
								rect.Height -= sz.Height + m_spacing;
								break;
						}
					}
				}
				#endregion
			}

			m_allIsSet = true;

			return m_placement == ChartPlacement.Outside ? m_insideRect : m_outsideRect;
		}
		/// <summary>
		/// Clears all dock controls.
		/// </summary>
		public void Clear()
		{
			this.Freeze();

			foreach (IChartDockControl ctrl in m_elements)
			{
				this.Remove(ctrl);
			}

			this.Melt();
			this.RaiseSizeChanged(EventArgs.Empty);
		}
		/// <summary>
		/// Returns the controls.
		/// </summary>
		/// <returns></returns>
		public IEnumerable GetAllControls()
		{
			return m_elements;
		}
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (m_host != null)
			{
				m_host.MouseMove -= new MouseEventHandler(this.OnHostMouseMove);
				m_host.MouseDown -= new MouseEventHandler(this.OnHostMouseDown);
				m_host.MouseUp -= new MouseEventHandler(this.OnHostMouseUp);
				m_host = null;
			}

			foreach (IChartDockControl control in m_elements)
			{
				control.LocationChanging -= new LocationEventHandler(Control_LocationChanging);
				control.ChartDockChanged -= new EventHandler(Control_ChartDockChanged);
				control.ChartAlignmentChanged -= new EventHandler(Control_ChartAlignmentChanged);
				control.SizeChanged -= new EventHandler(Control_SizeChanged);
			}

			this.SizeChanged = null;
		}
		#endregion

		#region Helper methods
		/// <summary>
		/// Called when mouse is down.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void OnHostMouseDown(object sender, MouseEventArgs e)
		{
			m_activeElement = null;

			foreach (IChartDockControl element in m_elements)
			{
				if (element.Enabled)
				{
					Rectangle rect = new Rectangle(element.Location, element.Size);

					if (rect.Contains(e.X, e.Y))
					{
						m_activeElement = element;
						m_mouseOffset = new Size(e.X - m_activeElement.Location.X,
							e.Y - m_activeElement.Location.Y);
					}
				}
			}
		}
		/// <summary>
		/// Called when mouse is up.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void OnHostMouseUp(object sender, MouseEventArgs e)
		{
			m_activeElement = null;
		}
		/// <summary>
		/// Called when mouse is move.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void OnHostMouseMove(object sender, MouseEventArgs e)
		{
			if (Control.MouseButtons != MouseButtons.None)
			{
				if (m_activeElement != null && m_activeElement.Enabled 
					&& (m_activeElement.Behavior & ChartDockingFlags.Movable) == ChartDockingFlags.Movable)
				{
					m_activeElement.Location = new Point(m_activeElement.Location.X + m_mouseOffset.Width,
						m_activeElement.Location.Y + m_mouseOffset.Height);
				}
			}
		}
		/// <summary>
		/// Moves the in wrap dock.
		/// </summary>
		/// <param name="dockElement">The dock element.</param>
		/// <param name="pt">The pt.</param>
		private void MoveInWrapDock(IChartDockControl dockElement, Point pt)
		{
			foreach (IChartDockControl element in m_elements)
			{
				if (dockElement != element)
				{
					Rectangle bounds = new Rectangle(element.Location, element.Size);

					if (bounds.Contains(pt))
					{
						this.Move(dockElement, m_elements.IndexOf(element));
						this.RaiseSizeChanged(EventArgs.Empty);
						break;
					}
				}
			}
		}
		/// <summary>
		/// Moves the in dock.
		/// </summary>
		/// <param name="dockElement">The dock element.</param>
		/// <param name="pt">The cursor position.</param>
		private void MoveInDock(IChartDockControl dockElement, Point pt)
		{
			bool isChnaged = false;
			int index = m_elements.IndexOf(dockElement);

			for (int i = 0; i < m_elements.Count; i++)
			{
				IChartDockControl element = m_elements[i];

				if (element != dockElement && element.Position == dockElement.Position)
				{
					int cx = element.Location.X + element.Size.Width / 2;
					int cy = element.Location.Y + element.Size.Height / 2;

					bool isBefore = index > i;
					bool isAfter = index < i;
					bool isLowerX = pt.X < cx;
					bool isLowerY = pt.Y < cy;

					switch (dockElement.Position)
					{
						case ChartDock.Left:
							if ((isLowerX && isBefore) || (!isLowerX && isAfter))
							{
								this.Move(dockElement, i);
								isChnaged = true;
							}
							break;

						case ChartDock.Right:
							if ((isLowerX && isAfter) || (!isLowerX && isBefore))
							{
								this.Move(dockElement, i);
								isChnaged = true;
							}
							break;

						case ChartDock.Top:
							if ((!isLowerY && isAfter) || (isLowerY && isBefore))
							{
								this.Move(dockElement, i);
								isChnaged = true;
							}
							break;

						case ChartDock.Bottom:
							if ((!isLowerY && isBefore) || (isLowerY && isAfter))
							{
								this.Move(dockElement, i);
								isChnaged = true;
							}
							break;
					}
				}
			}

			if (m_dockAlignment)
			{
				dockElement.Alignment = this.GetAlignmentByRect(pt, m_insideRect, dockElement.Orientation);
			}

			if (isChnaged)
			{
				this.DoLayout(m_outsideRect);
			}
		}
		/// <summary>
		/// Docks the specified dock element by mouse point.
		/// </summary>
		/// <param name="dockElement">The dock element.</param>
		/// <param name="pt">The pt.</param>
		private bool Dock(IChartDockControl dockElement, Point pt)
		{
			ChartDock position = dockElement.Position;

			if (pt.X < m_insideRect.Left)
			{
				position = ChartDock.Left;
			}
			else if (pt.X > m_insideRect.Right)
			{
				position = ChartDock.Right;
			}
			else if (pt.Y < m_insideRect.Top)
			{
				position = ChartDock.Top;
			}
			else if (pt.Y > m_insideRect.Bottom)
			{
				position = ChartDock.Bottom;
			}
			else if (m_insideRect.Contains(pt))
			{
				position = ChartDock.Floating;
			}

			if (dockElement.Position != position)
			{
				dockElement.Position = position;
				return true;
			}

			return false;
		}
		/// <summary>
		/// Moves the specified element to the new index.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="to">To.</param>
		private void Move(IChartDockControl element, int to)
		{
			m_elements.Remove(element);
			m_elements.Insert(Math.Min(to, m_elements.Count), element);
		}
		/// <summary>
		/// Sets specified control to the center of specified rectangle.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <param name="rect">The rectangle.</param>
		private static void SetToCenter(IChartDockControl control, Rectangle rect)
		{
			int x = (rect.Width - control.Size.Width) / 2;
			int y = (rect.Height - control.Size.Height) / 2;

			if (control.Orientation == ChartOrientation.Horizontal)
			{
				if (control.Alignment == ChartAlignment.Center)
				{
					control.Location = new Point(rect.Left + x, rect.Top + y);
				}
				else if (control.Alignment == ChartAlignment.Near)
				{
					control.Location = new Point(rect.Left + y, rect.Top + y);
				}
				else if (control.Alignment == ChartAlignment.Far)
				{
					control.Location = new Point(rect.Right - control.Size.Width - y, rect.Top + y);
				}
			}
			else
			{
				if (control.Alignment == ChartAlignment.Center)
				{
					control.Location = new Point(rect.Left + x, rect.Top + y);
				}
				else if (control.Alignment == ChartAlignment.Near)
				{
					control.Location = new Point(rect.Left + x, rect.Top + x);
				}
				else if (control.Alignment == ChartAlignment.Far)
				{
					control.Location = new Point(rect.Left + x, rect.Bottom - control.Size.Height - x);
				}
			}
		}
		/// <summary>
		/// Gets the size of the measure.
		/// </summary>
		/// <param name="position">The ChartDock.</param>
		/// <param name="maxSize">Size of the max.</param>
		/// <returns></returns>
		private static Size GetMeasureSize(ChartDock position, Size maxSize)
		{
			switch (position)
			{
				case ChartDock.Left:
				case ChartDock.Right:
					maxSize.Width = maxSize.Width / 2;
					break;
				case ChartDock.Top:
				case ChartDock.Bottom:
					maxSize.Height = maxSize.Height / 2;
					break;
			}

			return maxSize;
		}

		/// <summary>
		/// Gets the alignment by rect.
		/// </summary>
		/// <param name="pt">The The cursor position.</param>
		/// <param name="rc">The inner bounds.</param>
		/// <param name="or">The orientation.</param>
		/// <returns></returns>
		private ChartAlignment GetAlignmentByRect(Point pt, Rectangle rc, ChartOrientation or)
		{
			ChartAlignment res = ChartAlignment.Center;

			if (or == ChartOrientation.Horizontal)
			{
				int cl = rc.Left + rc.Width / 3;
				int cr = rc.Right - rc.Width / 3;

				if (pt.X < cl)
				{
					res = ChartAlignment.Near;
				}
				else if (pt.X > cr)
				{
					res = ChartAlignment.Far;
				}
			}
			else
			{
				int ct = rc.Top + rc.Height / 3;
				int cb = rc.Bottom - rc.Height / 3;

				if (pt.Y < ct)
				{
					res = ChartAlignment.Near;
				}
				else if (pt.Y > cb)
				{
					res = ChartAlignment.Far;
				}
			}

			return res;
		}
		/// <summary>
		/// Handles the LocationChanging event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.LocationEventArgs"/> instance containing the event data.</param>
		private void Control_LocationChanging(object sender, LocationEventArgs e)
		{
			if (!m_supressEventProcessing && m_allIsSet)
			{
				m_allIsSet = false;

				IChartDockControl dockElement = sender as IChartDockControl;

				if ((dockElement.Behavior & ChartDockingFlags.Dockable) == ChartDockingFlags.Dockable)
				{
					Point mousePt = Point.Empty;

					if (m_host != null)
					{
						mousePt = m_host.PointToClient(Control.MousePosition);
					}
					else if (dockElement is Control)
					{
						mousePt = (dockElement as Control).Parent.PointToClient(Control.MousePosition);
					}

					if (!this.Dock(dockElement, mousePt))
					{
						if (m_layoutMode == ChartLayoutMode.Stack)
						{
							this.MoveInDock(dockElement, mousePt);
						}
						else
						{
							this.MoveInWrapDock(dockElement, mousePt);
						}
					}

					e.Allowed = (dockElement.Position == ChartDock.Floating);
				}

				m_allIsSet = true;
			}
		}
		/// <summary>
		/// Handles the ChartDockChanged event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Control_ChartDockChanged(object sender, EventArgs e)
		{
			if (!m_supressEventProcessing)
			{
				this.RaiseSizeChanged(EventArgs.Empty);
			}
		}
		/// <summary>
		/// Handles the ChartAlignmentChanged event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Control_ChartAlignmentChanged(object sender, EventArgs e)
		{
			if (!m_supressEventProcessing)
			{
				this.DoLayout(m_outsideRect);
			}
		}
		/// <summary>
		/// Raises the SizeChanged event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void RaiseSizeChanged(EventArgs e)
		{
			if (SizeChanged != null && !m_supressEventFiring)
			{
				SizeChanged(this, e);
			}
		}
		/// <summary>
		/// Handles the LocationChanged event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Control_LocationChanged(object sender, EventArgs e)
		{
			if (!m_supressEventProcessing)
			{
				if (m_allIsSet)
				{
					m_allIsSet = false;
					IChartDockControl dockElement = sender as IChartDockControl;

					if ((dockElement.Behavior & ChartDockingFlags.Dockable) == ChartDockingFlags.Dockable)
					{
						this.Dock(dockElement, dockElement.Location);
					}

					m_allIsSet = true;
				}
			}
		}
		/// <summary>
		/// Handles the SizeChanged event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Control_SizeChanged(object sender, EventArgs e)
		{
			IChartDockControl dockElement = sender as IChartDockControl;

			if (dockElement.Position != ChartDock.Floating
					&& (dockElement.Behavior & ChartDockingFlags.Dockable) == ChartDockingFlags.Dockable)
			{
				if (!m_supressEventProcessing)
				{
					this.RaiseSizeChanged(e);
				}
			}
		}
		#endregion
	}
}
