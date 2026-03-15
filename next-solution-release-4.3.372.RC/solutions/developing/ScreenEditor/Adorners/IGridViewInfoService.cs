using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ScreenManager.Adorners
{
    interface IGridViewInfoService
    {
        void PrepareControlRectangleForSnapLines(UIElement exludeme);
        bool IsPointOnSnapLineHorizontal(double h);
        bool IsPointOnSnapLineVertical(double v);
        double SnapPointToGridSizeHorizontal(double h);
        double SnapPointToGridSizeVertical(double v);

        void GridServiceObjectResized();

        void DragMultipleSelection(double h, double v, UIElement current);

        Point SnapPointToGridSize(Point pt);
        Point GetMousePosition();

        bool CanDragStart(FrameworkElement item, DragStartedEventArgs e);

        Double GridSnapNumber { get; }

        void ActivateProperty();
    }
}
