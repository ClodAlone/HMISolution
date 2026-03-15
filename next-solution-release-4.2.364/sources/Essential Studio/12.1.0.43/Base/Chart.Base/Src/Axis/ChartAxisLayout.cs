#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Web.UI.WebControls;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ChartAxisLayout
	{
		#region Helper classes
		/// <summary>
		/// 
		/// </summary>
		class AxesCollection : Collection<ChartAxis>
		{
			#region Members
			private ChartAxisLayout m_owner = null;
			#endregion

			#region Constructor
			/// <summary>
			/// Initializes a new instance of the <see cref="AxesCollection"/> class.
			/// </summary>
			/// <param name="owner">The owner.</param>
			public AxesCollection(ChartAxisLayout owner)
			{
				m_owner = owner;
			}
			#endregion

			#region Implementation
			/// <summary>
			/// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
			/// <param name="item">The object to insert. The value can be null for reference types.</param>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
			protected override void InsertItem(int index, ChartAxis item)
			{
				item.Layout = m_owner;
				base.InsertItem(index, item);
				m_owner.Invalidate();
			}

			/// <summary>
			/// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
			/// </summary>
			protected override void ClearItems()
			{
				foreach (ChartAxis axis in this)
				{
					axis.Layout = null;
				}

				base.ClearItems();

				m_owner.Invalidate();
			}

			/// <summary>
			/// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
			/// </summary>
			/// <param name="index">The zero-based index of the element to remove.</param>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
			protected override void RemoveItem(int index)
			{
				base.RemoveItem(index);
				m_owner.Invalidate();
			}

			/// <summary>
			/// Replaces the element at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index of the element to replace.</param>
			/// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
			protected override void SetItem(int index, ChartAxis item)
			{
				this[index].Layout = null;
				item.Layout = m_owner;

				base.SetItem(index, item);
				m_owner.Invalidate();
			}
			#endregion
		}
		/// <summary>
		/// 
		/// </summary>
		class AxisLayoutException : Exception
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="AxisLayoutException"/> class.
			/// </summary>
			/// <param name="message">The message.</param>
			public AxisLayoutException(string message)
				: base(message)
			{ 
			}
		}
		#endregion

		#region Members
		private AxesCollection m_axes = null;
		private ChartAxesLayoutMode m_layoutMode = ChartAxesLayoutMode.Stacking;
		private ChartAxisLayoutCollection m_owner = null;
		private SizeF m_dimention = SizeF.Empty;
		private float m_spacing = 0;
		private Unit m_height = Unit.Empty;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the axes.
		/// </summary>
		/// <value>The axes.</value>
		public IList<ChartAxis> Axes
		{
			get
			{
				return m_axes;
			}
		}
		/// <summary>
		/// Gets or sets the layout mode.
		/// </summary>
		/// <value>The layout mode.</value>
		public ChartAxesLayoutMode LayoutMode
		{
			get { return m_layoutMode; }
			set { m_layoutMode = value; }
		}
        /// <summary>
        /// Gets or sets the layout height in percentage or pixels of chart area.
        /// </summary>
        /// <value>The layout height in percentage.</value>
        /// 
        public Unit Height 
        {
            get {return m_height; }
            set { m_height = value; }
        }
		/// <summary>
		/// Gets or sets the spacing.
		/// </summary>
		/// <value>The spacing.</value>
		public float Spacing
		{
			get { return m_spacing; }
			set 
			{
				if (m_spacing != value)
				{
					m_spacing = value;
					this.Invalidate();
				}
			}
		}
		/// <summary>
		/// Gets or sets the chart area.
		/// </summary>
		/// <value>The chart area.</value>
		internal ChartAxisLayoutCollection Owner
		{
			get
			{
				return m_owner;
			}
			set
			{
				if (value == null)
				{
					m_owner = value;
				}
				else
				{
					if (m_owner != null)
						throw new ArgumentException("ChartAxisLayout is already added");

					m_owner = value;
				}
			}
		}
		/// <summary>
		/// Gets the orientation.
		/// </summary>
		/// <value>The orientation.</value>
		private ChartOrientation Orientation
		{
			get 
			{
				if (m_axes.Count > 0)
				{
					return m_axes[0].Orientation;
				}

				return ChartOrientation.Horizontal;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartAxisLayout"/> class.
		/// </summary>
		public ChartAxisLayout()
		{
			m_axes = new AxesCollection(this);
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		private void Invalidate()
		{
			if (m_owner != null && m_owner.ChartArea != null)
			{
				m_owner.ChartArea.Redraw(true);
			}
		}
		/// <summary>
		/// Validates this instance.
		/// </summary>
		internal void Validate(ChartOrientation orientation)
		{
			if (m_axes.Count > 0)
			{
				for (int i = 0; i < m_axes.Count; i++)
				{
					ChartAxis axis = m_axes[i];

					if (axis.Orientation != orientation)
					{
						throw new AxisLayoutException( "Axes should have the same orientation" );
					}
				}
			}
		}
		/// <summary>
		/// Arranges the specified g.
		/// </summary>
		/// <param name="bounds">The bounds.</param>
		/// <param name="orientation">The orientation.</param>
		internal void Arrange( RectangleF bounds,
			ChartOrientation orientation)
		{
			if (orientation == ChartOrientation.Horizontal)
			{
				SizeF spacing = m_owner.ChartArea.AxisSpacing;

				if (m_layoutMode == ChartAxesLayoutMode.SideBySide)
				{
					float position = 0;
					float width = (bounds.Width - (m_axes.Count - 1) * m_spacing) / m_axes.Count;

					foreach (ChartAxis axis in m_axes)
					{
						axis.DockToRectangle(new RectangleF(bounds.X + position, bounds.Y, width, bounds.Height));
						position += width + m_spacing;
					}
				}
				else
				{
					RectangleF rc = bounds;

					foreach (ChartAxis axis in m_axes)
					{
						rc = axis.DockToRectangle(rc);

						if (axis.OpposedPosition)
						{
							rc.Y -= m_spacing;
						}

						rc.Height += m_spacing;
					}
				}
			}
			else
			{
				SizeF spacing = m_owner.ChartArea.AxisSpacing;

				if (m_layoutMode == ChartAxesLayoutMode.SideBySide)
				{
					float position = 0;
					float height = (bounds.Height - (m_axes.Count - 1) * m_spacing) / m_axes.Count;

					foreach (ChartAxis axis in m_axes)
					{
						axis.DockToRectangle(new RectangleF(bounds.X, bounds.Y + position, bounds.Width, height));
						position += height + m_spacing;
					}
				}
				else
				{
					RectangleF rc = bounds;

					foreach (ChartAxis axis in m_axes)
					{
						rc = axis.DockToRectangle(rc);

						if (!axis.OpposedPosition)
						{
							rc.X -= m_spacing;
						}

						rc.Width += m_spacing;
					}
				}
			}
		}
		/// <summary>
		/// Measures by specified <see cref="Graphics"/>.
		/// </summary>
		/// <param name="g">The <see cref="Graphics"/>.</param>
		/// <param name="bounds">The bounds.</param>
		/// <param name="orientation">The orientation.</param>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <param name="scrolls">The scrolls.</param>
		internal void Measure(Graphics g, RectangleF bounds, 
			ChartOrientation orientation,
			out float left, 
			out float right, 
			out float scrolls)
		{
			left = 0;
			right = 0;
			scrolls = 0;

			if (orientation == ChartOrientation.Vertical)
			{
				foreach (ChartAxis axis in m_axes)
				{
					float sz = axis.GetDimension(g, m_owner.ChartArea, bounds);

					if (m_layoutMode == ChartAxesLayoutMode.Stacking)
					{
						if (axis.OpposedPosition)
						{
							right += sz;
						}
						else
						{
							left += sz;
						}

						if ((axis.ScrollBar != null)&& (axis.ScrollBar.Visible))
						{
							scrolls += axis.ScrollBar.Dimension;
						}
					}
					else
					{
						if (axis.OpposedPosition)
						{
							right = Math.Max(sz, right);
						}
						else
						{
							left = Math.Max(sz, left);
						}

                        if ((axis.ScrollBar != null) && (axis.ScrollBar.Visible))
						{
							scrolls = axis.ScrollBar.Dimension;
						}
					}
				}
			}
			else
			{
				foreach (ChartAxis axis in m_axes)
				{
					float sz = axis.GetDimension(g, m_owner.ChartArea, bounds);

					if (m_layoutMode == ChartAxesLayoutMode.Stacking)
					{
						if (axis.OpposedPosition)
						{
							right += sz;                            
						}
						else
						{
							left += sz;                           
						}

                        if ((axis.ScrollBar != null) && (axis.ScrollBar.Visible))
						{
							scrolls += axis.ScrollBar.Dimension;
						}
					}
					else
					{
						if (axis.OpposedPosition)
						{
							right = Math.Max(sz, right);                         
						}
						else
						{
							left = Math.Max(sz, left);                          
						}

                        if ((axis.ScrollBar != null) && (axis.ScrollBar.Visible))
						{
							scrolls = axis.ScrollBar.Dimension;
						}
					}
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class ChartAxisLayoutCollection : Collection<ChartAxisLayout>
	{
		#region Members
		private ChartArea m_owner = null;
		private float m_spacing = 0;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the chart area.
		/// </summary>
		/// <value>The chart area.</value>
		internal ChartArea ChartArea
		{
			get
			{
				return m_owner;
			}
		}
		/// <summary>
		/// Gets or sets the spacing.
		/// </summary>
		/// <value>The spacing.</value>
		public float Spacing
		{
			get { return m_spacing; }
			set { m_spacing = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartAxisLayoutCollection"/> class.
		/// </summary>
		/// <param name="owner">The owner.</param>
		internal ChartAxisLayoutCollection(ChartArea owner)
		{
			m_owner = owner;
		}
		#endregion

		#region Implementgation
		/// <summary>
		/// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
		/// </summary>
		protected override void ClearItems()
		{
			foreach (ChartAxisLayout layout in this)
			{
				layout.Owner = null;
			}

			base.ClearItems();
			this.Invalidate();
		}
		/// <summary>
		/// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
		/// <param name="item">The object to insert. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
		protected override void InsertItem(int index, ChartAxisLayout item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			item.Owner = this;
			base.InsertItem(index, item);
			this.Invalidate();
		}
		/// <summary>
		/// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
		protected override void RemoveItem(int index)
		{
			this[index].Owner = null;
			base.RemoveItem(index);
			this.Invalidate();
		}
		/// <summary>
		/// Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to replace.</param>
		/// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
		protected override void SetItem(int index, ChartAxisLayout item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			this[index].Owner = null;
			item.Owner = this;

			base.SetItem(index, item);
		}
		/// <summary>
		/// Invalidates this instance.
		/// </summary>
		internal void Invalidate()
		{
			if (m_owner != null)
			{
				m_owner.Redraw(false);
			}
		}
		/// <summary>
		/// Validates this instance.
		/// </summary>
		internal void Validate(ChartOrientation orientation)
		{
			if (this.Count > 0)
			{
				foreach (ChartAxis axis in m_owner.Axes)
				{
					if (axis.Orientation == orientation 
						&& (axis.Layout == null || axis.Layout.Owner != this))
					{
						throw new Exception( string.Format("Layouts collection should contains all {0} axes.", orientation));
					}
				}

				foreach (ChartAxisLayout layout in this)
				{
					layout.Validate(orientation);
				}
			}
		}
		/// <summary>
		/// Arranges the specified g.
		/// </summary>
		/// <param name="bounds">The bounds.</param>
		/// <param name="orientation">The orientation.</param>
		internal void Arrange(RectangleF bounds,
			ChartOrientation orientation)
		{
			float position = 0;

			if (orientation == ChartOrientation.Horizontal)
			{
				float width = (bounds.Width - (this.Count - 1) * m_spacing) / this.Count;

				foreach (ChartAxisLayout layout in this)
				{
					layout.Arrange(new RectangleF(bounds.X + position, bounds.Y, width, bounds.Height), orientation);
					position += width + m_spacing;
				}
			}
			else
			{
				float height = (bounds.Height - (this.Count - 1) * m_spacing) / this.Count;

                float pixelHeight = 0;
                float percentageHeight = 0;
                float remainingHeight = 0;
                int notSet = this.Count;
                foreach (ChartAxisLayout layout in this)
                {
                    if (layout.Height.Value > 0)
                    {
                        if (layout.Height.Type == UnitType.Pixel)
                            pixelHeight += (float)layout.Height.Value;
                        else if (layout.Height.Type == UnitType.Percentage)
                        {
                            percentageHeight += (float)layout.Height.Value;
                        }
                        notSet -= 1;
                    }
                    else
                    {
                        layout.Height = new Unit(100, UnitType.Percentage);
                        percentageHeight += (float)layout.Height.Value;
                    }
                }
				if(bounds.Height> pixelHeight)
                    remainingHeight = bounds.Height - pixelHeight;					
                foreach (ChartAxisLayout layout in this)
                {
                    switch (layout.Height.Type)
                    {
                        case UnitType.Percentage:
                            if (layout.Height.Value > 0)
                            {
                                float percentage = ((float)layout.Height.Value * 100) / percentageHeight;
                                height = (percentage * remainingHeight) / 100;
                            }
                            break;

                        case UnitType.Pixel:
                            if (layout.Height.Value > 0)
                            {
                                height = (float)layout.Height.Value;
                            }

                            break;
                    }
                    layout.Arrange(new RectangleF(bounds.X, bounds.Y + position, bounds.Width, height), orientation);
                    position += height + m_spacing;
                }
			}
		}
		/// <summary>
		/// Measures by the specified <see cref="Graphics"/>.
		/// </summary>
		/// <param name="g">The <see cref="Graphics"/>.</param>
		/// <param name="bounds">The bounds.</param>
		/// <param name="orientation">The orientation.</param>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <param name="scrolls">The scrolls.</param>
		internal void Measure(Graphics g, RectangleF bounds, 
			ChartOrientation orientation,
			out float left, 
			out float right, 
			out float scrolls)
		{
			left = 0;
			right = 0;
			scrolls = 0;

			float tLeft, tRight, tScrolls;

			foreach (ChartAxisLayout layout in this)
			{
				layout.Measure(g, bounds, orientation, out tLeft, out tRight, out tScrolls);

				left = Math.Max(left, tLeft);
				right = Math.Max(right, tRight);
				scrolls = Math.Max(scrolls, tScrolls);
			}
		}
		#endregion
	}
}
