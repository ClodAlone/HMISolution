#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Linq;
#if !WinRT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
#endif
using Syncfusion.Data.Extensions;

namespace Syncfusion.UI.Xaml.Grid
{
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DetailsViewDataGrid : SfDataGrid, IDetailsViewInfo
    {

#if WPF
        static DetailsViewDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DetailsViewDataGrid), new FrameworkPropertyMetadata(typeof(DetailsViewDataGrid)));
        }
#endif
        public DetailsViewDataGrid()
        {
#if !WPF
            this.DefaultStyleKey = typeof (DetailsViewDataGrid);
#endif
            this.AllowDetailsViewPadding = true;
        }

        internal void InitializeDetailsViewDataGrid()
        {
            container = new VisualContainer();
            this.RefreshContainerAndView();
        }

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            var cp = this.GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            cp.Content = container;
        }

        protected override void RefreshContainerAndView()
        {
           base.RefreshContainerAndView();
        }

        protected override void RefreshHeaderLineCount()
        {
            headerLineCount = 1;
            if (StackedHeaderRows.Count > 0)
                headerLineCount += StackedHeaderRows.Count;
            if (AddNewRowPosition == AddNewRowPosition.Top)
                headerLineCount += 1;
            headerLineCount += this.GetTableSummaryCount(TableSummaryRowPosition.Top);
        }

        protected override void WireEvents()
        {
            base.WireEvents();
        }

        protected override void UnWireEvents()
        {
            base.UnWireEvents();
        }

        protected override void DisposeViewOnItemsSourceChanged()
        {
            //base.DisposeViewOnItemsSourceChanged();
        }

        internal override void SetSourceList(object itemsSource)
        {
            this.UnWireEvents();
            CreateCollectionView(itemsSource);
            this.WireEvents();
            if (this.View == null)
            {
                return;
            }
            var needsRefresh = false;
            if (this.View.SortDescriptions.Count != this.SortColumnDescriptions.Count || this.View.GroupDescriptions.Count != this.GroupColumnDescriptions.Count)
            {
                needsRefresh = true;
            }
            else
            {
                foreach (var sortdescription in this.SortColumnDescriptions)
                {
                    var sort = this.View.SortDescriptions.Any(x => !x.PropertyName.Equals(sortdescription.ColumnName) && !x.Direction.Equals(sortdescription.SortDirection));
                    if (sort)
                    {
                        needsRefresh = true;
                        break;
                    }
                }

                foreach (var groupdescription in this.GroupColumnDescriptions)
                {
                    var group = this.View.GroupDescriptions.Any(x => !x.GroupNames.Equals(groupdescription.ColumnName) && !x.GroupNames.Equals(groupdescription.Converter));
                    if (group)
                    {
                        needsRefresh = true;
                        break;
                    }
                }
            }
            if (needsRefresh)
                DeferRefresh();
        }

        #region IDetailsViewScrollInfo

        public void SetClipRect(Rect rect)
        {
            var width = this.VisualContainer.ColumnWidths.TotalExtent + 1;
            var height = rect.Height + VisualContainer.RowHeights.PaddingDistance;
            var clipRect = new Rect(0, 0, width < rect.Width ? width : rect.Width, rect.Height);
            this.Clip = new RectangleGeometry { Rect = clipRect };
        }

        public void SetHorizontalOffset(double offset)
        {
            if (!double.IsNaN(offset))
                this.VisualContainer.SetHorizontalOffset(offset);
        }

        public void SetVerticalOffset(double offset)
        {
            if (!double.IsNaN(offset))
                this.VisualContainer.SetVerticalOffset(offset);
        }

        public double GetExtendedWidth()
        {
            return this.VisualContainer.ColumnWidths.TotalExtent;
        }

        #endregion
    }
}
