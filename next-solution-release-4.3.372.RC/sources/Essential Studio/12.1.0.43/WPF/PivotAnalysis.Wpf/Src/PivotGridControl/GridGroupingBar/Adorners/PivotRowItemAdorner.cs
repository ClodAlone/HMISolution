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
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// This class represents the Adorner which contains the PivotRow ItemFields
    /// </summary>
    internal class PivotRowItemAdorner : Adorner
    {
        #region [ Public Properties ]

        public VisualCollection VisualChildren { get; set; }

        public PivotGridControl GridControl { get; set; }

        public PivotGroupingItemsControl PivotRowItemPanel { get; set; }

        #endregion

        #region [ Initialize/Finalize ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotRowItemAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The element to bind the adorner to.</param>
        /// <param name="rowItemsPanel">The panel holding pivot row items.</param>
        /// <exception cref="T:System.ArgumentNullException">adornedElement is null.</exception>
        public PivotRowItemAdorner(UIElement adornedElement, PivotGroupingItemsControl rowItemsPanel)
            : base(adornedElement)
        {
            VisualChildren = new VisualCollection(this);
            GridControl = adornedElement as PivotGridControl;
            PivotRowItemPanel = new PivotGroupingItemsControl();
            PivotRowItemPanel.Name = "PART_RowList";
            PivotRowItemPanel.DataContext = this.GridControl.PivotRows;
            PivotRowItemPanel.AllowDrop = true;
            this.PivotRowItemPanel = rowItemsPanel;

            //ItemsPanelTemplate PanelTemplate = XamlReader.Parse("<ItemsPanelTemplate " +
            //" xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'" +
            //" xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'> " +
            //" <StackPanel Orientation='Horizontal' VerticalAlignment='Bottom'/> </ItemsPanelTemplate>") as ItemsPanelTemplate;

            //PivotRowItemPanel.ItemsPanel = PanelTemplate;

            //PivotRowItemPanel.ItemTemplate = XamlReader.Parse("<DataTemplate " +
            //" xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'" +
            //" xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'> " +
            //" <Button Content='{Binding FieldHeader}' VerticalAlignment='Bottom'/> </DataTemplate>") as DataTemplate;

            ResourceDictionary resource = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            PivotRowItemPanel.ItemsPanel = resource["PivotRowItemPanelTemplate"] as ItemsPanelTemplate;

            PivotRowItemPanel.ItemTemplate = resource["PivotRowItemTemplate"] as DataTemplate;

            PivotRowItemPanel.ItemContainerStyle = resource["lstItemContainerStyle"] as Style;

            PivotRowItemPanel.ItemsSource = this.GridControl.PivotRows;

            //PivotRowItemPanel.ItemContainerGenerator.ContainerFromItem(
            VisualChildren.Add(PivotRowItemPanel);
        }

        #endregion

        #region [ Overrides ]

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return VisualChildren.Count;
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return VisualChildren[index];
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = GetAdornerWidth();
            double height = GetAdornerHeight();
            this.GridControl.GroupingBar.CalculateComputationInfoWidth();
            PivotRowItemPanel.Arrange(new Rect(0, 70, width, height));
            return finalSize;
        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Gets the height of the adorner.
        /// </summary>
        /// <returns></returns>
        private double GetAdornerHeight()
        {
            if (this.GridControl != null)
            {
                PivotGridControlBase gridControlBase = this.GridControl.InternalGrid;
                if (gridControlBase != null)
                {
                    double size = 0;
                    int headerCount = this.GridControl.PivotColumns.Count;

                    if (this.GridControl.PivotCalculations.Count > 1)
                    {
                        if (this.GridControl.PivotEngine.ShowCalculationsAsColumns)
                            headerCount++;
                    }

                    for (int i = 0; i < headerCount; i++)
                    {
                        size += gridControlBase.Model.RowHeights[i];
                    }
                    return size;
                }
            }
            return 20;
        }

        /// <summary>
        /// Gets the width of the adorner.
        /// </summary>
        /// <returns></returns>
        private double GetAdornerWidth()
        {
            if (this.GridControl != null)
            {
                PivotGridControlBase gridControlBase = this.GridControl.InternalGrid;
                if (gridControlBase != null)
                {
                    double size = 0;
                    int headerCount = this.GridControl.PivotRows.Count;

                    if (this.GridControl.PivotCalculations.Count > 1)
                    {
                        if (!this.GridControl.PivotEngine.ShowCalculationsAsColumns)
                            headerCount++;
                    }

                    for (int i = 0; i < headerCount; i++)
                    {
                        size += gridControlBase.Model.ColumnWidths[i];
                    }
                    return size;
                }
            }
            return 20;
        }

        #endregion
    }  
}
