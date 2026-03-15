using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Mindscape.WpfElements.WpfPropertyGrid;
using System.Windows.Threading;
using Mindscape.WpfElements;

namespace PropertyControl
{
    public class NodeState : GridState
    {
        public bool IsExpanded { get; set; }
    }

    public class GridState
    {
        private Dictionary<string, NodeState> _childStates = new Dictionary<string, NodeState>();

        public Dictionary<string, NodeState> ChildStates
        {
            get { return _childStates; }
        }
    }

    public class PropertyGridState
    {
        private static Dictionary<Type, GridState> _gridStates = new Dictionary<Type, GridState>();

        public static void RecordGridStatus(PropertyGrid pg)
        {
            if (pg == null || pg.SelectedObject == null)
                return;

            GridState state = new GridState();
            RecordStatus((TreeListView)(pg.Template.FindName("PART_Grid", pg)), state);

            _gridStates[pg.SelectedObject.GetType()] = state;
        }

        private static void RecordStatus(ItemsControl ctrl, GridState state)
        {
            if (ctrl == null)
                return;

            foreach (PropertyGridRow row in ctrl.Items)
            {
                if (!row.IsLeaf)
                {
                    TreeListViewItem tlvi = (TreeListViewItem)(ctrl.ItemContainerGenerator.ContainerFromItem(row));
                    if (tlvi == null)
                        continue;
                    state.ChildStates[row.Node.Name] = new NodeState { IsExpanded = tlvi.IsExpanded };
                    if (tlvi.IsExpanded)
                    {
                        RecordStatus(tlvi, state.ChildStates[row.Node.Name]);
                    }
                }
            }
        }


        public static void RestoreGridStatus(PropertyGrid pg)
        {
            GridState state;
            if (pg.SelectedObject == null || !_gridStates.TryGetValue(pg.SelectedObject.GetType(), out state))
                return;

            RestoreChildStatus((TreeListView)(pg.Template.FindName("PART_Grid", pg)), state);
        }

        private static void RestoreChildStatus(ItemsControl ctrl, GridState state)
        {
            if (ctrl == null)
                return;

            foreach (PropertyGridRow row in ctrl.Items)
            {
                if (!row.IsLeaf)
                {
                    NodeState status;
                    bool gotStatus = state.ChildStates.TryGetValue(row.Node.Name, out status);
                    if (gotStatus && status.IsExpanded)
                    {
                        TreeListViewItem tlvi = (TreeListViewItem)(ctrl.ItemContainerGenerator.ContainerFromItem(row));
                        if (tlvi == null)
                            continue;
                        tlvi.IsExpanded = true;
                        Action action = () => RestoreChildStatus(tlvi, status);
                        ctrl.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
                    }
                }
            }
        }
    }
}
