using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Utilities
{
    public static class LayoutHelper
    {
        public static bool HasTouchInput()
        {
            foreach (TabletDevice tabletDevice in Tablet.TabletDevices)
            {
                //Only detect if it is a touch Screen not how many touches (i.e. Single touch or Multi-touch)
                if (tabletDevice.Type == TabletDeviceType.Touch)
                    return true;
            }

            return false;
        }

        public static DependencyObject GetParentLogical(DependencyObject d)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
            {
                if (CheckIsDesignTimeRoot(d)) return null;
            }
            var retParent = LogicalTreeHelper.GetParent(d);
            if (retParent != null)
                return retParent;
            if (d is Visual)
                return VisualTreeHelper.GetParent(d);
            return retParent;
        }
        public static bool IsChildElementLogical(DependencyObject root, DependencyObject element)
        {
            DependencyObject parent = element;
            while (parent != null)
            {
                if (parent == root)
                    return true;
                parent = LayoutHelper.GetParentLogical(parent);
            }
            return false;
        }

        public static bool IsChildElement(DependencyObject root, DependencyObject element)
        {
            DependencyObject parent = element;
            while (parent != null)
            {
                if (parent == root)
                    return true;
                parent = LayoutHelper.GetParent(parent);
            }
            return false;
        }
        public static DependencyObject FindRoot(DependencyObject d)
        {
            DependencyObject current = d;
            while (GetParent(current) != null)
                current = GetParent(current);
            return current;
        }
        public static FrameworkElement GetTopLevelVisual(DependencyObject node)
        {
            FrameworkElement topElement = (FrameworkElement)node;
            while (node != null)
            {
                node = VisualTreeHelper.GetParent(node);
                if (node is FrameworkElement)
                    topElement = node as FrameworkElement;
            }
            return topElement;
        }
        public static Rect GetRelativeElementRect(UIElement element, UIElement parent)
        {
            GeneralTransform transform = element.TransformToVisual(parent);
            return transform.TransformBounds(new Rect(element.RenderSize));
        }
        public static UIElement GetTopContainerWithAdornerLayer(UIElement element)
        {
            DependencyObject currentObject = element;
            UIElement topContainer = null;
            while (currentObject != null)
            {
                UIElement currentUIElement = currentObject as UIElement;
                if (currentUIElement != null && AdornerLayer.GetAdornerLayer(currentUIElement) != null)
                    topContainer = (UIElement)currentObject;
                currentObject = VisualTreeHelper.GetParent(currentObject);
            }
            return topContainer;
        }
        static bool CheckIsDesignTimeRoot(DependencyObject d)
        {
            FrameworkElement elem = d as FrameworkElement;
            if (elem != null)
            {
                elem = VisualTreeHelper.GetParent(d) as FrameworkElement;
                if (elem != null)
                {
                    elem = elem.TemplatedParent as FrameworkElement;
                    if (elem != null && (elem.GetType().Name.Contains("DesignTimeWindow") || elem.GetType().Name.Contains("WindowInstance"))) return true;
                }
            }
            return false;
        }
        
        public static DependencyObject GetParent(DependencyObject d)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
            {
                if (CheckIsDesignTimeRoot(d)) return null;
            }
            if (d is Visual)
                return VisualTreeHelper.GetParent(d);
            return LogicalTreeHelper.GetParent(d);
        }
        public static T FindParentObject<T>(DependencyObject child) where T : class
        {
            while (child != null)
            {
                if (child is T)
                    return child as T;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
        public static T FindLayoutOrVisusualParentObject<T>(DependencyObject child) where T : class
        {
            while (child != null)
            {
                if (child is T)
                    return child as T;
                child = GetParent(child);
            }
            return null;
        }
        public static Size MeasureElementWithSingleChild(UIElement element, Size constraint)
        {
            UIElement child = (VisualTreeHelper.GetChildrenCount(element) > 0) ? (VisualTreeHelper.GetChild(element, 0) as UIElement) : null;
            if (child != null)
            {
                child.Measure(constraint);
                return child.DesiredSize;
            }
            return new Size();
        }
        public static Size ArrangeElementWithSingleChild(UIElement element, Size arrangeSize, Point position)
        {
            UIElement child = (VisualTreeHelper.GetChildrenCount(element) > 0) ? (VisualTreeHelper.GetChild(element, 0) as UIElement) : null;
            if (child != null)
                child.Arrange(new Rect(position, arrangeSize));
            return arrangeSize;
        }
        public static Size ArrangeElementWithSingleChild(UIElement element, Size arrangeSize)
        {
            return ArrangeElementWithSingleChild(element, arrangeSize, new Point(0, 0));
        }
        public static FrameworkElement GetRoot(FrameworkElement element)
        {
            FrameworkElement current = element;
            while (GetParent(current) != null)
                current = (FrameworkElement)GetParent(current);
            return current;
        }
        public static FrameworkElement FindElement(FrameworkElement treeRoot, Predicate<FrameworkElement> predicate)
        {
            VisualTreeEnumerator en = new VisualTreeEnumerator(treeRoot);
            while (en.MoveNext())
            {
                FrameworkElement element = en.Current as FrameworkElement;
                if (element != null && predicate(element))
                    return element;
            }
            return null;
        }
        public delegate void ElementHandler(FrameworkElement e);
        public static void ForEachElement(FrameworkElement treeRoot, ElementHandler elementHandler)
        {
            VisualTreeEnumerator en = new VisualTreeEnumerator(treeRoot);
            while (en.MoveNext())
            {
                FrameworkElement element = en.Current as FrameworkElement;
                if (element != null)
                    elementHandler(element);
            }
        }
    }
    public static class DockPanelLayoutHelper
    {
        public static Size MeasureDockPanelLayout(FrameworkElement parent, Size constraint)
        {
            double maxWidth = 0.0;
            double MaxHeight = 0.0;
            double totalWidth = 0.0;
            double totalHeight = 0.0;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                UIElement element = VisualTreeHelper.GetChild(parent, i) as UIElement;
                if (element == null) continue;
                Size availableSize = new Size(Math.Max(0, constraint.Width - totalWidth), Math.Max(0, constraint.Height - totalHeight));
                element.Measure(availableSize);
                Size desiredSize = element.DesiredSize;
                switch (DockPanel.GetDock(element))
                {
                    case Dock.Left:
                    case Dock.Right:
                        MaxHeight = Math.Max(MaxHeight, totalHeight + desiredSize.Height);
                        totalWidth += desiredSize.Width;
                        break;
                    case Dock.Top:
                    case Dock.Bottom:
                        maxWidth = Math.Max(maxWidth, totalWidth + desiredSize.Width);
                        totalHeight += desiredSize.Height;
                        break;
                }
            }
            maxWidth = Math.Max(maxWidth, totalWidth);
            return new Size(maxWidth, Math.Max(MaxHeight, totalHeight));
        }
        public static Size ArrangeDockPanelLayout(FrameworkElement parent, Size arrangeSize, bool lastChildFill)
        {
            double x = 0;
            double y = 0;
            double totalWidth = 0;
            double totalHeight = 0;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            int lastDockableChildIndex = count - (lastChildFill ? 1 : 0);
            for (int i = 0; i < count; i++)
            {
                UIElement element = VisualTreeHelper.GetChild(parent, i) as UIElement;
                if (element == null) continue;
                Size desiredSize = element.DesiredSize;
                Rect finalRect = new Rect(x, y, Math.Max(0, arrangeSize.Width - (x + totalWidth)), Math.Max(0, arrangeSize.Height - (y + totalHeight)));
                if (i < lastDockableChildIndex)
                {
                    switch (DockPanel.GetDock(element))
                    {
                        case Dock.Left:
                            x += desiredSize.Width;
                            finalRect.Width = desiredSize.Width;
                            break;
                        case Dock.Top:
                            y += desiredSize.Height;
                            finalRect.Height = desiredSize.Height;
                            break;
                        case Dock.Right:
                            totalWidth += desiredSize.Width;
                            finalRect.X = Math.Max((double)0.0, (double)(arrangeSize.Width - totalWidth));
                            finalRect.Width = desiredSize.Width;
                            break;
                        case Dock.Bottom:
                            totalHeight += desiredSize.Height;
                            finalRect.Y = Math.Max((double)0.0, (double)(arrangeSize.Height - totalHeight));
                            finalRect.Height = desiredSize.Height;
                            break;
                    }
                }
                element.Arrange(finalRect);
            }
            return arrangeSize;
        }
    }
}
