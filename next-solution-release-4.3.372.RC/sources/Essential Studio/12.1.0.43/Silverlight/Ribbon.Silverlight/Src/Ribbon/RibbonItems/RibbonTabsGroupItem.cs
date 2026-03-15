#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the RibbonTabsGroupItem Class.
    /// </summary>
	internal class RibbonTabsGroupItem : Grid
	{
		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonTabsGroupItem"/> class.
        /// </summary>
		internal RibbonTabsGroupItem()
		{
			this.textPresenter = new TextBlock();

			this.textPresenter.IsHitTestVisible = false;
			this.textPresenter.Margin = new Thickness(6, 1, 6, 1);

			this.textPresenter.VerticalAlignment = VerticalAlignment.Top;
			this.textPresenter.HorizontalAlignment = HorizontalAlignment.Left;

            this.Children.Add(this.textPresenter);

            #region Border styles
            Rectangle leftBar = new Rectangle()
                {
                    Width = 1,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    IsHitTestVisible = false,
                    Stretch = Stretch.Fill,
                };
            var brush = new LinearGradientBrush() { EndPoint = new Point(0, 1) };

            brush.GradientStops.Add(new GradientStop() { Offset = 0.0, Color = Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF) });
            brush.GradientStops.Add(new GradientStop() { Offset = 0.7, Color = Color.FromArgb(0x22, 0x00, 0x00, 0x00) });
            leftBar.Fill = brush;

            this.Children.Add(leftBar);

            Rectangle rightBar = new Rectangle()
            {
                Width = 1,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                IsHitTestVisible = false,
                Stretch = Stretch.Fill,
            };

            rightBar.Fill = brush;

            this.Children.Add(rightBar);

            Rectangle leftBottomBar = new Rectangle()
            {
                Width = 1,
                Height = 20,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, -20),
                IsHitTestVisible = false,
                Stretch = Stretch.Fill,
            };
            brush = new LinearGradientBrush() { EndPoint = new Point(0, 1) };

            brush.GradientStops.Add(new GradientStop() { Offset = 0.4, Color = Color.FromArgb(0x20, 0x00, 0x00, 0x00) });
            brush.GradientStops.Add(new GradientStop() { Offset = 1.0, Color = Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF) });
            leftBottomBar.Fill = brush;

            this.Children.Add(leftBottomBar);

            Rectangle rightBottomBar = new Rectangle()
            {
                Width = 1,
                Height = 20,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, -20),
                IsHitTestVisible = false,
                Stretch = Stretch.Fill,
            };

            rightBottomBar.Fill = brush;

            this.Children.Add(rightBottomBar); 
            #endregion
		}

		#endregion

		#region Properties

        /// <summary>
        /// Gets or sets the tabs group.
        /// </summary>
        /// <value>The tabs group.</value>
		internal RibbonTabsGroup TabsGroup
		{
			get
			{
				return this.tabsGroup;
			}

			set
			{
				if (this.tabsGroup != value)
				{
					this.tabsGroup = value;

					this.UpdateBackground();
					this.UpdateCaption();
				}
			}
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Updates the background.
        /// </summary>
		internal void UpdateBackground()
		{
			LinearGradientBrush brush = null;

			if (this.tabsGroup != null)
			{
				Color clr = this.tabsGroup.Color;

				if (clr.A > 0)
				{
					brush = new LinearGradientBrush() { EndPoint = new Point(0, 1), ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation };

                    brush.GradientStops.Add(new GradientStop() { Offset = 0.0, Color = Color.FromArgb(0, 0, 0, 0) });
                    brush.GradientStops.Add(new GradientStop() { Offset = 2.0, Color = clr });
				}
			}

			this.Background = brush;
		}

        /// <summary>
        /// Updates the caption.
        /// </summary>
		internal void UpdateCaption()
		{
			string text = null;

			if (this.tabsGroup != null)
			{
				text = this.tabsGroup.Caption;
			}

			this.textPresenter.Text = text;
		}

		#endregion

		#region Fields

		private TextBlock textPresenter;
		private RibbonTabsGroup tabsGroup;

		#endregion
	}
}
