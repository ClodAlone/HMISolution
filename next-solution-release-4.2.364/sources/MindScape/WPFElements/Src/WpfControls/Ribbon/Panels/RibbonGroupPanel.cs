using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A panel that provides layout logic for items within a <see cref="RibbonGroup"/>.
  /// </summary>
  public class RibbonGroupPanel : Panel
  {
    private IList<RibbonGroupPanelGroup> _groups = new List<RibbonGroupPanelGroup>();

    private bool _measureLock;

    /// <summary>
    /// Measures the elements within this <see cref="RibbonGroupPanel"/> and returns the desired size.
    /// </summary>
    /// <param name="availableSize">The available size of this <see cref="RibbonGroupPanel"/>.</param>
    /// <returns>The desired size of this <see cref="RibbonGroupPanel"/>.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      if (_measureLock)
      {
        return base.MeasureOverride(availableSize);
      }
      _measureLock = true;
      double availableWidth = availableSize.Width;
      if (Double.IsInfinity(availableSize.Width) || Double.IsNaN(availableSize.Width))
      {
        availableWidth = 100000000;
        availableSize.Width = availableWidth;
      }

      double width = 0;
      double height = 0;
      if (!Double.IsInfinity(availableSize.Height) && !Double.IsNaN(availableSize.Height))
      {
        height = availableSize.Height;
      }

      List<RibbonGroupPanelGroup> priorityGroups = new List<RibbonGroupPanelGroup>();

      // Initial group population:
      _groups = new List<RibbonGroupPanelGroup>();
      int index = 0; // TODO: this may be temporary to help with the priority group sorting.
      RibbonGroupPanelGroup currentGroup = new RibbonGroupPanelGroup();
      double collapsedGroupWidth = 0;
      currentGroup.Index = index;
      _groups.Add(currentGroup);
      priorityGroups.Add(currentGroup);
      for (int i = Children.Count - 1; i >= 0; i--)
      {
        index++;
        UIElement element = Children[i];

        bool added = currentGroup.TryAddElement(element);
        if (!added)
        {
          width += collapsedGroupWidth;
          currentGroup = new RibbonGroupPanelGroup();
          collapsedGroupWidth = 0;
          currentGroup.Index = index;
          _groups.Insert(0, currentGroup);
          priorityGroups.Add(currentGroup);
          currentGroup.TryAddElement(element);
        }

        element.Measure(availableSize);
        height = Math.Max(height, element.DesiredSize.Height);
        if (Ribbon.GetEditorSize(element) == RibbonEditorSize.Large)
        {
          width += element.DesiredSize.Width;
          if (width == availableWidth)
          {
            element.Measure(new Size(availableWidth + 1, availableSize.Height));
            if (element.DesiredSize.Width == availableWidth + 1)
            {
              width++;
            }
          }
        }
        else
        {
          collapsedGroupWidth = Math.Max(collapsedGroupWidth, element.DesiredSize.Width);
          currentGroup.CollapsedWidth = collapsedGroupWidth;
          if (collapsedGroupWidth == availableSize.Width)
          {
            element.Measure(new Size(availableWidth + 1, availableSize.Height));
            if (element.DesiredSize.Width == availableWidth + 1)
            {
              width++;
            }
          }
        }
      }
      width += collapsedGroupWidth;

      ItemsControl itemsHost = ItemsControl.GetItemsOwner(this);
      RibbonGroup ribbonGroup = VisualTreeUtils.FindAncestor<RibbonGroup>(itemsHost);

      // If width is larger than available width, shrink each group one by one until they all fit:
      while (width > availableWidth)
      {
        priorityGroups.Sort((g1, g2) => { return g1.Priority == g2.Priority ? g1.Index - g2.Index : g2.Priority - g1.Priority; });
        while (priorityGroups.Count > 0)
        {
          if (priorityGroups[0].TryShrink())
          {
            break;
          }
          else
          {
            priorityGroups.RemoveAt(0);
          }
        }
        if (priorityGroups.Count == 0)
        {
          if (ribbonGroup != null)
          {
            //ribbonGroup.SetIsExpanded(false);
            ribbonGroup.IsReadyToCollapse = true;
          }
          break;
        }
        
        width = 0;
        double collapsedWidth = 0;
        foreach (RibbonGroupPanelGroup group in _groups)
        {
          foreach (UIElement element in group.Elements)
          {
            if (Ribbon.GetEditorSize(element) == RibbonEditorSize.Large)
            {
              width += collapsedWidth;
              element.Measure(availableSize);
              width += element.DesiredSize.Width;
              collapsedWidth = 0;
            }
            else
            {
              // NOTE: This first measure is to activate template triggers. The second measure is to get the final size reading.
              element.Measure(new Size(availableWidth + 1, availableSize.Height / 3.0)); // TODO: measure with 1/3 available height.
              element.InvalidateMeasure();
              element.Measure(new Size(availableWidth, availableSize.Height / 3.0));
              collapsedWidth = Math.Max(collapsedWidth, element.DesiredSize.Width);
              group.CollapsedWidth = collapsedWidth;
              if (collapsedWidth == availableWidth)
              {
                element.Measure(new Size(availableWidth + 1, availableSize.Height / 3.0));
                if (element.DesiredSize.Width == availableWidth + 1)
                {
                  collapsedWidth++;
                }
              }
            }
          }
          width += collapsedWidth;
          collapsedWidth = 0;
        }
      }

      if (ribbonGroup != null)
      {
        bool canShrink = false;
        foreach (RibbonGroupPanelGroup group in priorityGroups)
        {
          if (group.CanShrink())
          {
            canShrink = true;
            break;
          }
        }
        ribbonGroup.CanShrink = canShrink;
      }

      _measureLock = false;
      return new Size(width, height);
    }

    /// <summary>
    /// Arranges the elements within this <see cref="RibbonGroupPanel"/> and returns the final size.
    /// </summary>
    /// <param name="finalSize">The available size for arranging the elements.</param>
    /// <returns>The final size of this <see cref="RibbonGroupPanel"/>.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      double x = 0;

      double collapsedWidth = 0;
      double y = 0;
      double availableCollapsedHeight = finalSize.Height / 3.0;
      foreach (RibbonGroupPanelGroup group in _groups)
      {
        foreach (UIElement element in group.Elements)
        {
          if (Ribbon.GetEditorSize(element) == RibbonEditorSize.Large)
          {
            x += collapsedWidth;
            element.Arrange(new Rect(x, 0, element.DesiredSize.Width, finalSize.Height));
            x += element.DesiredSize.Width;
            collapsedWidth = 0;
            y = 0;
          }
          else
          {
            double arrangeWidth = element.DesiredSize.Width;
            if (Ribbon.GetEditorHeader(element) != null && !Double.IsNaN(Ribbon.GetEditorWidth(element)))
            {
              arrangeWidth = group.CollapsedWidth;
            }
            element.Arrange(new Rect(x, y, arrangeWidth, availableCollapsedHeight));
            collapsedWidth = Math.Max(collapsedWidth, element.DesiredSize.Width);
            y += availableCollapsedHeight;
          }
        }
        x += collapsedWidth;
        collapsedWidth = 0;
        y = 0;
      }

      return finalSize;
    }
  }
}
