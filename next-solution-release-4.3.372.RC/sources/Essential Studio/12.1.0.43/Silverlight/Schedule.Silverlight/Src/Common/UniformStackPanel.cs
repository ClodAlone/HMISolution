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
using System.Linq;

namespace Syncfusion.Windows.Controls.Schedule
{
	/// <summary>
	/// Represents horizintal stack panel with items of the same width.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class UniformStackPanel :
		Panel
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="UniformStackPanel"/> class.
		/// </summary>
		public UniformStackPanel()
		{
           
		}

       

        	

		#endregion		

		#region Dependency properties

		#region Orientation

		/// <summary>
		/// Gets or sets the dimension by which child elements are stacked.
		/// </summary>
		public Orientation Orientation
		{
			get
			{
				return (Orientation)GetValue(OrientationProperty);
			}

			set
			{
				SetValue(OrientationProperty, value);
			}
		}

		/// <summary>
		/// Event that is raised when <see cref="UniformStackPanel.Orientation"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback OrientationChanged;

		/// <summary>
		/// The identifier for the <see cref="UniformStackPanel.Orientation"/> dependency property. 
		/// </summary>
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
			"Orientation", typeof(Orientation), typeof(UniformStackPanel), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

		/// <summary>
		/// Calls OnOrientationChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UniformStackPanel instance = (UniformStackPanel)d;
			instance.OnOrientationChanged(e);
		}

		/// <summary>
		/// Raises OrientationChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.OrientationChanged != null)
			{
				this.OrientationChanged(this, e);
			}
		}

		#endregion

		#endregion

		#region Overrides

		/// <summary>
		/// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
		/// </summary>
		/// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
		/// <returns>
		/// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
		/// </returns>
		protected override Size MeasureOverride(Size availableSize)
		{
			if (this.Children.Count <= 0)
			{
				return new Size();
			}

#if SILVERLIGHT
            int visibleItemsCount = this.Children.Where(el => el.Visibility != Visibility.Collapsed).Count();
#else
            int visibleItemsCount = this.Children.ToTypedList<UIElement>().Where(el => el.Visibility != Visibility.Collapsed).Count();
#endif
            Size itemSize = availableSize;

			if (this.Orientation == Orientation.Horizontal)
			{
				if (!double.IsInfinity(itemSize.Width))
				{
					itemSize.Width /= visibleItemsCount;
				}

				double maxHeight = 0.0;
				double width = 0.0;
               
				foreach (UIElement item in this.Children)
				{
					if (item.Visibility != Visibility.Collapsed)
					{
						item.Measure(itemSize);
						width += double.IsInfinity(itemSize.Width) ? item.DesiredSize.Width : itemSize.Width;
                        
						maxHeight = Math.Max(maxHeight, item.DesiredSize.Height);
					}
				}


                if (double.IsInfinity(itemSize.Width))
                {
                    itemSize.Width = width / visibleItemsCount;
                    itemSize.Height = maxHeight;

                    foreach (UIElement item in this.Children)
                    {
                        if (item.Visibility != Visibility.Collapsed)
                        {
                            item.Measure(itemSize);
                        }
                    }
                }

				if (double.IsInfinity(availableSize.Width))
				{
					availableSize.Width = width;
				}

				if (double.IsInfinity(availableSize.Height))
				{
					availableSize.Height = maxHeight;
				}
			}
			else
			{
				if (!double.IsInfinity(itemSize.Height))
				{
					itemSize.Height /= visibleItemsCount;
				}

				double maxWidth = 0.0;
				double height = 0.0;

				foreach (UIElement item in this.Children)
				{
					if (item.Visibility != Visibility.Collapsed)
					{
						item.Measure(itemSize);
						height += double.IsInfinity(itemSize.Height) ? item.DesiredSize.Height : itemSize.Height;
						maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
					}
				}

				if (double.IsInfinity(itemSize.Height))
				{
					itemSize.Width = maxWidth;
					itemSize.Height = height / visibleItemsCount;

					foreach (UIElement item in this.Children)
					{
						if (item.Visibility != Visibility.Collapsed)
						{
							item.Measure(itemSize);
						}
					}
				}

				if (double.IsInfinity(availableSize.Height))
				{
					availableSize.Height = height;
				}

				if (double.IsInfinity(availableSize.Width))
				{
					availableSize.Width = maxWidth;
				}
			}

			return availableSize;
		}

		/// <summary>
		/// Provides the behavior for the "arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
		/// </summary>
		/// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
		/// <returns>The actual size used.</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (this.Children.Count <= 0)
			{
				return finalSize;
			}

			int visibleItemsCount = 0;

			foreach (UIElement item in this.Children)
			{
				if (item.Visibility != Visibility.Collapsed)
				{
					++visibleItemsCount;
				}
			}

			if (this.Orientation == Orientation.Horizontal)
			{
				double itemWidth = finalSize.Width / visibleItemsCount;
				Rect itemRect = new Rect(0, 0, itemWidth, finalSize.Height);

				foreach (UIElement item in this.Children)
				{
					if (item.Visibility != Visibility.Collapsed)
					{
						item.Arrange(itemRect);
						Size size = item.DesiredSize;

						itemRect.X += itemWidth;
					}
				}
			}
			else
			{
				double itemHeight = finalSize.Height / visibleItemsCount;
				Rect itemRect = new Rect(0, 0, finalSize.Width, itemHeight);

				foreach (UIElement item in this.Children)
				{
					if (item.Visibility != Visibility.Collapsed)
					{
						item.Arrange(itemRect);

						itemRect.Y += itemHeight;
					}
				}
			}

			return finalSize;
		}

        

		#endregion
	}
}
