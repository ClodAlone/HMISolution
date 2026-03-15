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

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Panel designed to layout collection of <see cref="TabButton"/>.
	/// </summary>
	public class RibbonTabPanel : ScrollPanel
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonTabPanel"/> class.
		/// </summary>
		public RibbonTabPanel()
		{
		}

		#endregion

		#region Override methods

		/// <summary>
		/// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
		/// </summary>
		/// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
		/// <returns>
		/// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
		/// </returns>
		protected override Size MeasureOverride(Size availableSize)
		{
			double tabsFullWidth;
			double tabsWidth = this.MeasureTabs(availableSize, out tabsFullWidth);

			if (double.IsInfinity(availableSize.Width))
			{
				availableSize.Width = tabsWidth;
			}

			this.Extent = tabsWidth;

			this.UpdateSeparators(tabsFullWidth, tabsWidth);

			if (this.Extent < availableSize.Width + this.ScrollPosition)
			{
				double scrollPos = this.Extent - availableSize.Width;

				this.ScrollPosition = (scrollPos < 0.0) ? 0.0 : scrollPos;
			}

			if (double.IsInfinity(availableSize.Height))
			{
				double tabsHeight = 0.0;

				foreach (UIElement item in this.Children)
				{
					tabsHeight = Math.Max(tabsHeight, item.DesiredSize.Height);
				}

				availableSize.Height = tabsHeight;
			}

			return availableSize;
		}

		#endregion

		#region Implementation

		private double MeasureTabs(Size availableSize, out double tabsFullWidth)
		{
			tabsFullWidth = 0.0;

			UIElementCollection tabs = this.Children;
			double tabsWidth = 0.0;

			if (tabs.Count > 0)
			{
				foreach (TabButton tab in tabs)
				{
					tab.Measure(availableSize);
					tabsWidth += tab.DesiredSize.Width;
				}

				tabsFullWidth = tabsWidth;

				double availWidth = availableSize.Width;

				if (tabsWidth > availWidth && availWidth > 3 * tabs.Count)
				{
					double d = Math.Ceiling((tabsWidth - availWidth) / tabs.Count);
					tabsWidth = 0.0;

					foreach (TabButton tab in tabs)
					{
						double width = tab.DesiredSize.Width - d;

						if (width > tab.MaxWidth)
						{
							width = tab.MaxWidth;
						}
						else if (width < tab.MinWidth)
						{
							width = tab.MinWidth;
						}

						Size size = new Size(width, tab.DesiredSize.Height);

						tab.Measure(size);

						tabsWidth += tab.DesiredSize.Width;
					}
				}
			}

			return tabsWidth;
		}

        /// <summary>
        /// Updates the separators.
        /// </summary>
        /// <param name="tabsFullWidth">Full width of the tabs.</param>
        /// <param name="tabsWidth">Width of the tabs.</param>
		private void UpdateSeparators(double tabsFullWidth, double tabsWidth)
		{
			double separatorOpacity = 0.0;

			if (tabsWidth > 0.0 && tabsWidth < tabsFullWidth)
			{
				if (tabsWidth < tabsFullWidth * 0.75)
				{
					separatorOpacity = 1.0;
				}
				else
				{
					separatorOpacity = 1.0 - (2.0 * ((tabsWidth / tabsFullWidth) - 0.5));
				}
			}

			foreach (TabButton tab in this.Children)
			{
				tab.SeparatorOpacity = separatorOpacity;
			}

			int iTab = this.Children.Count - 1;

			while (iTab >= 0 && this.Children[iTab].Visibility == Visibility.Collapsed)
			{
				--iTab;
			}

			if (iTab >= 0)
			{
				TabButton tab = this.Children[iTab] as TabButton;

				tab.SeparatorOpacity = 0.0;
			}
		}

		#endregion
	}
}
