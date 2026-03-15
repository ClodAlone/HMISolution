#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Panel with scrolling ability.
	/// </summary>
	public class ScrollPanel :
		Panel
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="ScrollPanel"/> class.
		/// </summary>
		public ScrollPanel()
		{
		}

		#endregion

		#region Dependency property

		#region ScrollPosition

		/// <summary>
		/// Gets or sets the scroll position.
		/// </summary>
		public double ScrollPosition
		{
			get
			{
				return (double)GetValue(ScrollPositionProperty);
			}

			set
			{
				SetValue(ScrollPositionProperty, value);
			}
		}

		/// <summary>
		/// Event that is raised when <see cref="ScrollPanel.ScrollPosition"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback ScrollPositionChanged;

		/// <summary>
		/// The identifier for the <see cref="ScrollPanel.ScrollPosition"/> dependency property. 
		/// </summary>
		public static readonly DependencyProperty ScrollPositionProperty = DependencyProperty.Register(
			"ScrollPosition",
			typeof(double),
			typeof(ScrollPanel),
			new PropertyMetadata(0.0, new PropertyChangedCallback(OnScrollPositionChanged)));

		/// <summary>
		/// Calls OnScrollPositionChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnScrollPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ScrollPanel instance = (ScrollPanel)d;
			instance.OnScrollPositionChanged(e);
		}

		/// <summary>
		/// Raises ScrollPositionChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		protected virtual void OnScrollPositionChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.ScrollPosition < 0.0)
			{
				this.ScrollPosition = 0.0;
			}
			else if (this.ScrollPosition + this.ActualWidth <= this.Extent)
			{
				InvalidateMeasure();

				this.RaiseScrollPositionChangedEvent(e);
			}
			else
			{
				this.ScrollPosition = this.Extent - this.ActualWidth;
			}
		}

		#endregion

		#region Extent

		/// <summary>
		/// Gets or sets the horizontal size of all the content for display in the panel. 
		/// </summary>
		public double Extent
		{
			get
			{
				return (double)GetValue(ExtentProperty);
			}

			set
			{
				SetValue(ExtentProperty, value);
			}
		}

		/// <summary>
		/// Event that is raised when <see cref="ScrollPanel.Extent"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback ExtentChanged;

		/// <summary>
		/// The identifier for the <see cref="ScrollPanel.Extent"/> dependency property. 
		/// </summary>
		public static readonly DependencyProperty ExtentProperty = DependencyProperty.Register(
			"Extent",
			typeof(double),
			typeof(ScrollPanel),
			new PropertyMetadata(0.0, new PropertyChangedCallback(OnExtentChanged)));

		/// <summary>
		/// Calls OnExtentChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnExtentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ScrollPanel instance = (ScrollPanel)d;
			instance.OnExtentChanged(e);
		}

		/// <summary>
		/// Raises ExtentChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private void OnExtentChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.ExtentChanged != null)
			{
				this.ExtentChanged(this, e);
			}
		}

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Raises the <see cref="ScrollPositionChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		protected void RaiseScrollPositionChangedEvent(DependencyPropertyChangedEventArgs e)
		{
			if (this.ScrollPositionChanged != null)
			{
				this.ScrollPositionChanged(this, e);
			}
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
		/// </summary>
		/// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
		/// <returns>The actual size used.</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			UIElementCollection children = this.Children;
			Point p = new Point(-this.ScrollPosition, 0);

			foreach (UIElement item in children)
			{
				if (item.Visibility == Visibility.Visible)
				{
					item.Arrange(new Rect(p, new Size(item.DesiredSize.Width, finalSize.Height)));

					p.X += item.DesiredSize.Width;
				}
			}

			this.Clip = new RectangleGeometry() { Rect = new Rect(new Point(), finalSize) };

			return finalSize;
		}

		/// <summary>
		/// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
		/// </summary>
		/// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
		/// <returns>
		/// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
		/// </returns>
		protected override Size MeasureOverride(Size availableSize)
		{
			double itemsWidth = 0.0;
			double itemsHeight = 0.0;

			foreach (UIElement item in this.Children)
			{
				item.Measure(availableSize);

				itemsWidth += item.DesiredSize.Width;
				itemsHeight = Math.Max(itemsHeight, item.DesiredSize.Height);
			}

			double availWidth = availableSize.Width;

			if (double.IsInfinity(availWidth) || itemsWidth < availWidth)
			{
				availWidth = itemsWidth;
			}

			if (double.IsInfinity(availableSize.Height))
			{
				availableSize.Height = itemsHeight;
			}

			this.Extent = Math.Max(availWidth, itemsWidth);

			if (availWidth >= this.Extent)
			{
				this.ScrollPosition = 0.0;
			}

			availableSize.Width = availWidth;

			return availableSize;
		}

		#endregion
	}
}
