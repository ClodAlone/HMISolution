#region Copyright
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
using System.Collections.ObjectModel;
using System.Linq;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents panel for tabcontrol's header panel and content layout.
    /// </summary>
    public class TabControlPanel : Panel
    {
        #region Private members
        /// <summary>
        /// Tabcontrol parent.
        /// </summary>
        private TabLayoutPanel m_tabLayoutPanel;

        /// <summary>
        /// Tabcontrol parent.
        /// </summary>
        private TabStripPlacement m_tabStripPlacement;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the tabcontrol parent.
        /// </summary>
        internal TabLayoutPanel TabLayoutPanel
        {
            get
            {
                return m_tabLayoutPanel;
            }

            set
            {
                m_tabLayoutPanel = value;
            }
        }

        /// <summary>
        /// Gets or sets the tabs strip placement.
        /// </summary>
        internal TabStripPlacement TabStripPlacement
        {
            get
            {
                return m_tabStripPlacement;
            }

            set
            {
                m_tabStripPlacement = value;
            }
        }

        /// <summary>
        /// Gets the height of all tab rows.
        /// </summary>
        internal double AllRowsHeight
        {
            get
            {
                double height = 0;
                if (TabLayoutPanel != null)
                {
                    height = TabLayoutPanel.RowHeight * TabLayoutPanel.NumRows;
                }

                return height;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsPositiveInfinity(availableSize.Height))
            {
                // availableSize.Height = 0;
                availableSize.Height = this.Children.OfType<UIElement>().Max((e) => e.DesiredSize.Height);
            }

            if (double.IsPositiveInfinity(availableSize.Width))
            {
                //availableSize.Width = 0;
                availableSize.Width = this.Children.OfType<UIElement>().Max((e) => e.DesiredSize.Width);
            }
            foreach (UIElement child in this.Children)
            {
                if (child is Grid && (this.TabStripPlacement == TabStripPlacement.Left ||
                    this.TabStripPlacement == TabStripPlacement.Right))
                {
                    child.Measure(new Size(availableSize.Height, availableSize.Width));

                    if (availableSize.Height < this.AllRowsHeight)
                    {
                        availableSize.Height = this.AllRowsHeight;
                    }
                }
                else
                {
                    child.Measure(availableSize);
                }
            }

           

            return availableSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double allRowsHeight = 0;
            double headerPanelHeight = 0;
            switch (this.TabStripPlacement)
            {
                case TabStripPlacement.Top:
                    foreach (FrameworkElement child in Children)
                    {
                        if (child is Grid)
                        {
                            headerPanelHeight = child.DesiredSize.Height;
                            child.Arrange(new Rect(0, 0, finalSize.Width, headerPanelHeight));

                            RotateTransform rotateTransform = new RotateTransform
                            {
                                Angle = 0
                            };
                            child.RenderTransform = rotateTransform;
                        }
                        else
                        {
                            child.Margin = new Thickness(0, 0, 0, headerPanelHeight);
                            child.Arrange(new Rect(0, headerPanelHeight, finalSize.Width, finalSize.Height));
                        }
                    }

                    break;
                case TabStripPlacement.Left:
                    foreach (FrameworkElement child in Children)
                    {
                        if (child is Grid)
                        {
                            allRowsHeight = this.AllRowsHeight;
                            headerPanelHeight = child.DesiredSize.Height;

                            double width = child.DesiredSize.Width;
                            if (width < allRowsHeight)
                            {
                                width = allRowsHeight;
                            }

                            child.Arrange(new Rect(0, 0, width, finalSize.Height));

                            RotateTransform rotateTransform = new RotateTransform
                            {
                                Angle = -90d,
                                CenterX = child.DesiredSize.Width / 2,
                                CenterY = child.DesiredSize.Height / 2
                            };

                            TranslateTransform translateTransform = new TranslateTransform
                            {
                                X = (-finalSize.Height / 2) + (child.DesiredSize.Height / 2),
                                Y = (-width / 2) + (child.DesiredSize.Width / 2)
                            };

                            TransformGroup transformGroup = new TransformGroup();
                            transformGroup.Children.Add(rotateTransform);
                            transformGroup.Children.Add(translateTransform);
                            child.RenderTransform = transformGroup;
                        }
                        else
                        {
                            child.Margin = new Thickness(0, 0, headerPanelHeight, 0);
                            child.Arrange(new Rect(headerPanelHeight, 0, finalSize.Width, finalSize.Height));
                        }
                    }

                    break;
                case TabStripPlacement.Right:
                    foreach (FrameworkElement child in Children)
                    {
                        if (child is Grid)
                        {
                            allRowsHeight = this.AllRowsHeight;
                            headerPanelHeight = child.DesiredSize.Height;

                            double width = child.DesiredSize.Width;
                            if (width < allRowsHeight)
                            {
                                width = allRowsHeight;
                            }

                            child.Arrange(new Rect(finalSize.Width - headerPanelHeight, 0, width, finalSize.Height));

                            RotateTransform rotateTransform = new RotateTransform
                            {
                                Angle = 90d,
                                CenterX = child.DesiredSize.Width / 2,
                                CenterY = child.DesiredSize.Height / 2
                            };
                            TranslateTransform translateTransform = new TranslateTransform
                            {
                                X = (-finalSize.Height / 2) + (child.DesiredSize.Height / 2),
                                Y = (-width / 2) + (child.DesiredSize.Width / 2)
                            };

                            TransformGroup transformGroup = new TransformGroup();
                            transformGroup.Children.Add(rotateTransform);
                            transformGroup.Children.Add(translateTransform);
                            child.RenderTransform = transformGroup;
                        }
                        else
                        {
                            child.Margin = new Thickness(0, 0, headerPanelHeight, 0);
                            child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                        }
                    }

                    break;
                case TabStripPlacement.Bottom:
                    foreach (FrameworkElement child in Children)
                    {
                        if (child is Grid)
                        {
                            child.Arrange(new Rect(0, finalSize.Height - child.DesiredSize.Height, finalSize.Width, child.DesiredSize.Height));
                            headerPanelHeight = child.DesiredSize.Height;
                            RotateTransform rotateTransform = new RotateTransform
                            {
                                Angle = 180,
                                CenterX = child.DesiredSize.Width / 2,
                                CenterY = child.DesiredSize.Height / 2
                            };
                            child.RenderTransform = rotateTransform;
                        }
                        else
                        {
                            child.Margin = new Thickness(0, 0, 0, headerPanelHeight);
                            child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                        }
                    }

                    break;
            }

            if (this.TabStripPlacement == TabStripPlacement.Left ||
                        this.TabStripPlacement == TabStripPlacement.Right)
            {
                if (finalSize.Height < allRowsHeight)
                {
                    finalSize.Height = allRowsHeight;
                }
            }

            return finalSize;
        }
        #endregion
    }
}
