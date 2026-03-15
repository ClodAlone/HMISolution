#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
#if WinRT
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    public class PrintPagePanel : Panel, IDisposable
    {

        #region Fields

        readonly Size InfiniteSize =
     new Size(double.PositiveInfinity, double.PositiveInfinity);
        private const double IndentWidth = 20d;

        #endregion

        internal List<PrintManagerBase.RowInfo> RowsInfoList { get; set; }

        #region Overrides

        protected override Size MeasureOverride(Size availableSize)
        {
            double curY = 0, curLineHeight = 0, maxLineWidth = 0;
#if WinRT
            var childrenEnumarator =(Children as IEnumerable<UIElement>).GetEnumerator();
#else
            var childrenEnumarator = Children.GetEnumerator();
#endif
            foreach (var cellsInfo in RowsInfoList.Select(rowInfo => rowInfo.CellInfos))
            {
                curY += curLineHeight;
                double curX = 0;
                curLineHeight = 0;
                for (var i = 0; i < cellsInfo.Count; i++)
                {
                    if(!childrenEnumarator.MoveNext())
                        break;

                    var child = childrenEnumarator.Current as UIElement;

                    child.Measure(InfiniteSize);

                    curX += child.DesiredSize.Width;

                    if (child.DesiredSize.Height > curLineHeight)
                        curLineHeight = child.DesiredSize.Height;
                }

                if (curX > maxLineWidth)
                    maxLineWidth = curX;

                curY += curLineHeight;
            }

            var size = new Size
            {
                Width = double.IsInfinity(availableSize.Width) ? maxLineWidth : availableSize.Width,
                Height = double.IsInfinity(availableSize.Height) ? curY : availableSize.Height
            };

            return size;

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children == null || this.Children.Count == 0)
            { return finalSize; }


#if WinRT
            var childrenEnumarator =(Children as IEnumerable<UIElement>).GetEnumerator();
#else
            var childrenEnumarator = Children.GetEnumerator();
#endif
            foreach (var cellsInfo in RowsInfoList.Select(rowInfo => rowInfo.CellInfos))
            {
                
                foreach (var cellInfo in cellsInfo)
                {
                    if (!childrenEnumarator.MoveNext())
                        break;

                    var child = childrenEnumarator.Current as UIElement;

                    child.Arrange(cellInfo.CellRect);
                }
            }

           

            return finalSize;
        }

        #endregion

        #region Dispose Member

        public void Dispose()
        {
            if(RowsInfoList != null)
                RowsInfoList.Clear();
        }

        #endregion

    }
}
