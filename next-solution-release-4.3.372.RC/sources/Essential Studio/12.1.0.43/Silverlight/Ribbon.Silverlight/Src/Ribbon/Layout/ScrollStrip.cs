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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Abstract items control with support of content scrolling.
	/// </summary>
	[TemplateVisualState(GroupName = "LeftScrollButtonState", Name = "LeftNormal")]
	[TemplateVisualState(GroupName = "LeftScrollButtonState", Name = "LeftVisible")]
	[TemplateVisualState(GroupName = "RightScrollButtonState", Name = "RightNormal")]
	[TemplateVisualState(GroupName = "RightScrollButtonState", Name = "RightVisible")]
	public abstract class ScrollStrip :
		ItemsControl
	{
		#region Constants

		private const double ScrollTick = 20.0;

		#endregion

		#region Fields

		internal ScrollPanel panel;
		private ButtonBase leftScroll;
		private ButtonBase rightScroll;
		private Storyboard sbScrollLeft;
		private Storyboard sbScrollRight;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ScrollStrip"/> class.
		/// </summary>
		public ScrollStrip()
		{
			this.DefaultStyleKey = typeof(ScrollStrip);            
		}

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
            availableSize = base.MeasureOverride(availableSize);

			this.UpdateScrollButtons();

			return availableSize;
		}

		/// <summary>
		/// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
		/// </summary>
		/// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
		/// <returns>The actual size used.</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			finalSize = base.ArrangeOverride(finalSize);

			this.UpdateScrollButtons();

			return finalSize;
		}

		/// <summary>
		/// Prepares the specified element to display the specified item.
		/// </summary>
		/// <param name="element">The element used to display the specified item.</param>
		/// <param name="item">The item to display.</param>
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			if (this.panel == null || this.panel != VisualTreeHelper.GetParent(element))
			{
				this.SetupScrolling(element as FrameworkElement);
				this.UpdateScrollButtons();
			}
		}

		/// <summary>
		/// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
		/// </summary>
		/// <param name="element">The container element.</param>
		/// <param name="item">The target item.</param>
		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			base.ClearContainerForItemOverride(element, item);

			this.panel = null;
		}

		#endregion

		#region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is animation active.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is animation active; otherwise, <c>false</c>.
        /// </value>
		private bool IsAnimationActive
		{
			get
			{
				return this.sbScrollLeft.GetCurrentState() == ClockState.Active || this.sbScrollRight.GetCurrentState() == ClockState.Active;
			}
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Called when [left scroll click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void OnLeftScrollClick(object sender, RoutedEventArgs e)
		{
			if (!this.IsAnimationActive)
			{
				this.sbScrollLeft.Begin();
			}
		}

        /// <summary>
        /// Called when [right scroll click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void OnRightScrollClick(object sender, RoutedEventArgs e)
		{
			if (!this.IsAnimationActive)
			{
				this.sbScrollRight.Begin();
			}
		}

        /// <summary>
        /// Setups the scrolling.
        /// </summary>
        /// <param name="element">The element.</param>
		private void SetupScrolling(FrameworkElement element)
		{
			this.panel = VisualTreeHelper.GetParent(element) as ScrollPanel;

			if (this.panel != null)
			{
				this.panel.ScrollPositionChanged += new PropertyChangedCallback(this.OnScrollPositionChanged);

				this.leftScroll = GetTemplateChild("LeftScroll") as ButtonBase;
				this.rightScroll = GetTemplateChild("RightScroll") as ButtonBase;

				if (this.leftScroll != null)
				{
					this.leftScroll.Click += new RoutedEventHandler(this.OnLeftScrollClick);
				}

				if (this.rightScroll != null)
				{
					this.rightScroll.Click += new RoutedEventHandler(this.OnRightScrollClick);
				}

				FrameworkElement root = GetTemplateChild("RootElement") as FrameworkElement;

				if (root != null)
				{
					this.CreateStoryboards();					
				}
			}
		}

        /// <summary>
        /// Creates the storyboards.
        /// </summary>
		private void CreateStoryboards()
		{
			this.sbScrollLeft = ScrollStrip.CreateStoryboard();
			this.sbScrollRight = ScrollStrip.CreateStoryboard();

			this.SetStoryboardTarget(this.sbScrollLeft, -ScrollTick);
			this.SetStoryboardTarget(this.sbScrollRight, ScrollTick);
		}

        /// <summary>
        /// Creates the storyboard.
        /// </summary>
        /// <returns></returns>
		private static Storyboard CreateStoryboard()
		{
			Storyboard sb = new Storyboard() { SpeedRatio = 4 };
			DoubleAnimation da = new DoubleAnimation();

			Storyboard.SetTargetProperty(da, new PropertyPath("ScrollPosition", new object[] { }));

			sb.Children.Add(da);

			return sb;
		}

        /// <summary>
        /// Called when [scroll position changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private void OnScrollPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			InvalidateMeasure();
		}

        /// <summary>
        /// Sets the storyboard target.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="tick">The tick.</param>
		private void SetStoryboardTarget(Storyboard s, double tick)
		{
			if (s != null && s.Children.Count > 0)
			{
				DoubleAnimation da = s.Children[0] as DoubleAnimation;

				if (da != null)
				{
					Storyboard.SetTarget(da, this.panel);
					da.By = tick;
				}
			}
		}

        /// <summary>
        /// Updates the scroll buttons.
        /// </summary>
		private void UpdateScrollButtons()
		{
			if (this.panel != null)
			{
				if (this.panel.ScrollPosition > 0.0)
				{
					VisualStateManager.GoToState(this, "LeftVisible", true);
				}
				else
				{
					VisualStateManager.GoToState(this, "LeftNormal", true);

					if (this.leftScroll != null)
					{
						VisualStateManager.GoToState(this.leftScroll, "Normal", false);
					}
				}

				if (this.panel.ScrollPosition + this.panel.ActualWidth < this.panel.Extent)
				{
					VisualStateManager.GoToState(this, "RightVisible", true);
				}
				else
				{
					VisualStateManager.GoToState(this, "RightNormal", true);

					if (this.rightScroll != null)
					{
						VisualStateManager.GoToState(this.rightScroll, "Normal", false);
					}
				}
			}
		}

		#endregion
	}
}
