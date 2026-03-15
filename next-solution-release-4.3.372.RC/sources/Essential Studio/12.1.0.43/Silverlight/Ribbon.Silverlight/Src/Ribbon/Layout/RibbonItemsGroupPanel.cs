#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls.Theming;
using System;


namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the RibbonItemsGroupPanel Class.
    /// </summary>
	public class RibbonItemsGroupPanel : Panel
	{
		#region Constructor

		/// <summary>
		/// Initialize a new instance of <see cref="RibbonItemsGroupPanel"/>
		/// </summary>
		public RibbonItemsGroupPanel()
		{
		}


        /// <summary>
        /// 
        /// </summary>
        public Ribbon parentRibbon 
        {
            get
            {
                return VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
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
			Rect rect = new Rect(new Point(0, 0), finalSize);
			double width = 0;

			if (!this.separatorsInitialized)
			{
                if (this.parentRibbon!=null && !SkinManager.GetVisualStyle(parentRibbon).ToString().Contains("Office2010"))
                    this.InitializeSeparators(rect.Height);
			}

			foreach (UIElement ui in this.Children)
			{
				rect.X += width;
				rect.Width = width = ui.DesiredSize.Width;

				ui.Arrange(rect);
			}

			return base.ArrangeOverride(finalSize);
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
			this.MeasureChildren(availableSize);

			return new Size(this.GetSummaryWidthOfChildren(Children), this.GetMaxHeightOfChildren(Children));
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Initializes the separators.
        /// </summary>
        /// <param name="height">The height.</param>
		private void InitializeSeparators(double height)
		{
			int childCount = Children.Count;
            ResourceDictionary dictionary = new ResourceDictionary();
            if (SkinManager.GetVisualStyle(this.parentRibbon).ToString() == "Blend")
            {
                dictionary.Source = new Uri(@"/Syncfusion.Theming.blend;component/Ribbon.xaml", UriKind.RelativeOrAbsolute);
                this.separatorBrush = dictionary["ButtonPanelSeperatorBrush"] as Brush;
            }
            //if (childCount > 1)
            //{
            //    int sepCount = childCount - 1;
            //    while (this.separatorArray.Count < sepCount)
            //    {
            //        Rectangle rect = new Rectangle();
            //        rect.Fill = this.separatorBrush;
            //        rect.Width = 1;
            //        rect.Height = height;
            //        Children.Insert(curPos, rect);
            //        //this.separatorArray.Add(rect);
            //        curPos += 2;
            //    }
            //}

			this.separatorsInitialized = true;
		}

        /// <summary>
        /// Measures the children.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
		private void MeasureChildren(Size availableSize)
		{
			foreach (UIElement ui in this.Children)
			{
				ui.Measure(availableSize);
			}
		}

        /// <summary>
        /// Gets the summary width of children.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <returns></returns>
		private double GetSummaryWidthOfChildren(UIElementCollection children)
		{
			double width = 0;

			foreach (UIElement ui in children)
			{
				width += ui.DesiredSize.Width;
			}

			return width;
		}

        /// <summary>
        /// Gets the max height of children.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <returns></returns>
		private double GetMaxHeightOfChildren(UIElementCollection children)
		{
			double maxHeight = 0, height = 0;

			foreach (UIElement ui in children)
			{
				height = ui.DesiredSize.Height;

				if (maxHeight < height)
				{
					maxHeight = height;
				}
			}

			return maxHeight;
		}
		#endregion

		#region Fields

		private bool separatorsInitialized = false;

		private List<Rectangle> separatorArray = new List<Rectangle>();

		private Brush separatorBrush = new SolidColorBrush(Color.FromArgb(255, 180, 204, 232));

		#endregion
	}
}
