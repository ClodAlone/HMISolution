#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Panel designed to layout collection of <see cref="RibbonBar"/>.
	/// </summary>
	public class RibbonPanel :
		ScrollPanel
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonPanel"/> class.
		/// </summary>
		public RibbonPanel()
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
			double barsWidth = this.Measure(ref availableSize);

            IEnumerable<UIElement> bars = (from bar in this.Children select bar).Reverse();
           

            double _barsWidth = 0.0;
            foreach (UIElement element in bars)
            {
                RibbonBar bar = element as RibbonBar;
                if (bar != null)
                {
                    bar.Collapsed = false;
                }

                element.Measure(availableSize);

                _barsWidth += element.DesiredSize.Width;
            }

            if (_barsWidth > availableSize.Width)
            {
                foreach (UIElement element in bars)
                {
                    RibbonBar bar = element as RibbonBar;

                    if (bar != null && !bar.Collapsed)
                    {
                        Size prevBarSize = bar.DesiredSize;
                        double prevBarWidth = prevBarSize.Width;

                        _barsWidth -= prevBarWidth;
                      
                        bar.Collapsed = true;
                           
                        bar.Measure(availableSize);

                        Size szBar = bar.DesiredSize;

                        if (prevBarWidth <= szBar.Width)
                        {
                            bar.Collapsed = false;

                            bar.Measure(availableSize);
                            szBar = bar.DesiredSize;
                        }

                        _barsWidth += szBar.Width;

                        if (_barsWidth <= availableSize.Width)
                        {
                            break;
                        }
                    }

                }
            }

			this.Extent = _barsWidth;

			if (double.IsInfinity(availableSize.Width))
			{
				availableSize.Width = _barsWidth;
			}

			if (this.Extent < availableSize.Width + this.ScrollPosition)
			{
				double scrollPos = this.Extent - availableSize.Width;

				this.ScrollPosition = (scrollPos < 0.0) ? 0.0 : scrollPos;
			}

			if (double.IsInfinity(availableSize.Height))
			{
				double barsHeight = 0.0;

				foreach (UIElement item in this.Children)
				{
					barsHeight = Math.Max(barsHeight, item.DesiredSize.Height);
				}

				availableSize.Height = barsHeight;
			}

			return availableSize;
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Measures the specified available size.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        /// <returns></returns>
		private double Measure(ref Size availableSize)
		{
			IEnumerable<UIElement> bars = (from bar in this.Children select bar).Reverse();

			double barsWidth = 0.0;
			foreach (UIElement element in bars)
			{
				RibbonBar bar = element as RibbonBar;
                if (bar != null)
                {
                    bar.RevertGallery();
                    bar.ResetAll();
                    bar.Collapsed = false;
                }
                
				element.Measure(availableSize);

				barsWidth += element.DesiredSize.Width;
			}

			if (barsWidth > availableSize.Width)
			{
				foreach (UIElement element in bars)
				{
					RibbonBar bar = element as RibbonBar;

					if (bar != null && !bar.Collapsed)
					{
						Size prevBarSize = bar.DesiredSize;
						double prevBarWidth = prevBarSize.Width;

						barsWidth -= prevBarWidth;

                        bar.ResizeGallery();

                        if (!bar.ChangeLargeItems())
                        {
                            if (!bar.ChangeNormalItems())
                            {
                                bar.Collapsed = true;
                            }
                        }

                        
						bar.Measure(availableSize);

						Size szBar = bar.DesiredSize;

						if (prevBarWidth <= szBar.Width)
						{
                            bar.Collapsed = false;
							
							bar.Measure(availableSize);
							szBar = bar.DesiredSize;
						}

						barsWidth += szBar.Width;

						if (barsWidth <= availableSize.Width)
						{
                            //bar.RevertGallery();
							break;
						}
					}
                   
				}
			}
           

			return barsWidth;
		}

		#endregion
	}
}
