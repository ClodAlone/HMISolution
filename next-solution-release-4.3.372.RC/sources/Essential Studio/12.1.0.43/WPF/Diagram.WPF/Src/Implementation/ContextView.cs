#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.Specialized;
    using System.Windows.Threading;
    using System.Collections;
    using System.Windows.Media;

    /// <summary>
    /// Manage the context view of the Diagram.
    /// </summary>
    public class ContextViewManager
    {
        Dictionary<Node, Node> NodeClone = new Dictionary<Node, Node>();
        Dictionary<LineConnector, LineConnector> ConnectorClone = new Dictionary<LineConnector, LineConnector>();

        private DiagramControl _SourceControl;
        private DiagramControl _TargetControl;
        private Node _Root;

        public DiagramControl SourceControl
        {
            get { return _SourceControl; }
        }
        public DiagramControl TargetControl
        {
            get { return _TargetControl; }
        }
        /// <summary>
        /// Layout to arrange the context view
        /// </summary>
        public ILayout Layout { get; set; }
        
        private DiagramView View { get { return SourceControl.View; } }
        private DiagramModel Model { get { return TargetControl.Model; } }
        private CollectionExt Nodes { get { return Model.Nodes; } }
        private CollectionExt Connections { get { return Model.Connections; } }
        private RecycleBin _Bin = new RecycleBin();

        /// <summary>
        /// get or set ContextViewMode.
        /// </summary>
        public ContextViewMode ContextViewMode { get; set; }

        public ContextViewManager(DiagramControl source, DiagramControl contextTarget)
        {
            _SourceControl = source;
            _TargetControl = contextTarget;
            if (View != null)
            {
                View.SelectionList.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectionList_CollectionChanged);
            }
            Connections.CollectionChanged += new NotifyCollectionChangedEventHandler(Connections_CollectionChanged);
            _TargetControl.View.UndoRedoEnabled = false;
        }

        void Connections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TargetControl.Dispatcher.Invalidate(UpdateDiagram);
        }

        void SelectionList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                //case NotifyCollectionChangedAction.Add:
                //case NotifyCollectionChangedAction.Move:
                //case NotifyCollectionChangedAction.Remove:
                //case NotifyCollectionChangedAction.Replace:
                //    if (e.NewItems != null)
                //    {
                //        foreach (var item in e.NewItems)
                //        {
                //            if (item is Node)
                //            {
                //                Nodes.Add((item as Node).VirtualClone);
                //                AddDependent(item as Node);
                //            }
                //        }
                //    }
                //    if (e.OldItems != null)
                //    {
                //        foreach (var item in e.OldItems)
                //        {
                //            if (item is Node)
                //            {
                //                Nodes.Remove((item as Node).VirtualClone);
                //                //RemoveDependent(item as Node);
                //            }
                //        }
                //    }
                //    break;
                case NotifyCollectionChangedAction.Reset:
                    TargetControl.Dispatcher.Invalidate(UpdateDiagram);
                    break;
            }
        }

        private void UpdateDiagram()
        {
            _Bin.Enqueue(Connections, true);
            while (Connections.Count > 0)
            {
                Node tail = (Connections[0] as LineConnector).TailNode as Node;
                Node head = (Connections[0] as LineConnector).HeadNode as Node;
                (Connections[0] as LineConnector).HeadNode = null;
                (Connections[0] as LineConnector).TailNode = null;
                RemoveLine(this.Connections[0] as LineConnector);
                Connections.RemoveAt(0);
            }
            _Bin.Enqueue(Nodes, true);
            while (Nodes.Count > 0)
            {
                RemoveNode(Nodes[0] as Node);
                Nodes.RemoveAt(0);
            }

            _Root = null;
            foreach (var item in View.SelectionList)
            {
                if (item is Node)
                {
                    if (!NodeClone.ContainsKey(item as Node))
                    {
                        Node clone = PrepareNode(item as Node);
                        _Root = _Root ?? clone;
                        Nodes.Add(clone);
                        AddDependent(item as Node, clone);
                    }
                }
            }
            RefreshLayout();
        }        

        /// <summary>
        /// Refresh will be called automatically, it can also be force to be updated by calling this funtion.
        /// </summary>
        public virtual void RefreshLayout()
        {
            if (Layout is DirectedTreeLayout)
            {
                List<Node> roots = Nodes.OfType<Node>().Where((n) => n.InNeighbors.Count == 0 && n.OutNeighbors.Count > 0).ToList();

                if (roots.Count == 0)
                {
                    Model.LayoutRoot = _Root;
                }
                else if (roots.Count > 0)
                {
                    Model.LayoutRoot = roots[0];
                }
                //Model.RootNodes.Clear();
                //foreach (var item in roots)
                //{
                //    Model.RootNodes.Add(item);
                //}

                //Model.LayoutRoot = _Root;

                //Model.LayoutRoot = null;
                //Model.RootNodes.Clear();


                DirectedTreeLayout layout = new DirectedTreeLayout(TargetControl.Model, TargetControl.View);
                layout.RefreshLayout();
                //(Layout as DirectedTreeLayout).RefreshLayout();
            }
            else if (Layout is HierarchicalTreeLayout)
            {
                HierarchicalTreeLayout layout = new HierarchicalTreeLayout(TargetControl.Model, TargetControl.View);
                layout.RefreshLayout();
            }
            else if (Layout is TableLayout)
            {
                TableLayout layout = new TableLayout(TargetControl.Model, TargetControl.View);
                layout.RefreshLayout();
            }
            else if (Layout is BowtieLayout)
            {
                BowtieLayout layout = new BowtieLayout(TargetControl.Model, TargetControl.View);
                layout.RefreshLayout();
            }
            else if (Layout is RadialTreeLayout)
            {
                RadialTreeLayout layout = new RadialTreeLayout(TargetControl.Model, TargetControl.View);
                layout.RefreshLayout();
            }
        }

        private Node PrepareNode(Node source)
        {
            Node clone = _Bin.Dequeue<Node>();
            if (clone.Children != null)
            {
                //clone.Children = new CollectionExt();
                clone.Children.Clear();
                clone.Parents.Clear();
                clone.ParentNode = null;
                clone.Model = null;
                
                CollectionExt.Cleared = false;
            }
            VisualBrush brush = source.Brush;
            clone.Background = brush;
            clone.Width = source.Width;
            clone.Height = source.Height;
            AddNode(source, clone);
            return clone;
        }

        private void Connect(Node head, Node tail, LineConnector line)
        {
            LineConnector lc = PrepareLine(line);
            lc.HeadNode = head;
            lc.TailNode = tail;
            this.Connections.Add(lc);
        }

        private LineConnector PrepareLine(LineConnector line)
        {
            LineConnector clone = _Bin.Dequeue<LineConnector>();
            clone.ConnectorType = line.ConnectorType;
            AddLine(line, clone);
            return clone;
        }

        private void AddNode(Node source, Node clone)
        {
            if (NodeClone.ContainsKey(source))
            {
                NodeClone[source] = clone;
            }
            else
            {
                NodeClone.Add(source, clone);
            }
            if (NodeClone.ContainsKey(clone))
            {
                NodeClone[clone] = source;
            }
            else
            {
                NodeClone.Add(clone, source);
            }
        }

        private void AddLine(LineConnector line, LineConnector clone)
        {
            if (ConnectorClone.ContainsKey(line))
            {
                ConnectorClone[line] = clone;
            }
            else
            {
                ConnectorClone.Add(line, clone);
            }
            if (ConnectorClone.ContainsKey(clone))
            {
                ConnectorClone[clone] = line;
            }
            else
            {
                ConnectorClone.Add(clone, line);
            }
        }

        private void RemoveNode(Node node)
        {
            Node source = node;
            Node clone = NodeClone[node];
            NodeClone.Remove(source);
            NodeClone.Remove(clone);
        }

        private void RemoveLine(LineConnector line)
        {
            LineConnector clone = ConnectorClone[line];
            ConnectorClone.Remove(line);
            ConnectorClone.Remove(clone);
        }

        private void AddDependent(Node selectedNode, Node selectedClone)
        {
            switch (ContextViewMode)
            {
                case Diagram.ContextViewMode.Neighborhood:
                    foreach (var item in selectedNode.InEdges)
                    {
                        if (item is LineConnector)
                        {
                            Node node = (item as LineConnector).HeadNode as Node;
                            LineConnector line = item as LineConnector;
                            if (!NodeClone.ContainsKey(node))
                            {
                                Node clone = PrepareNode(node);
                                Nodes.Add(clone);
                                Connect(clone, selectedClone, line);
                            }
                        }
                    }

                    foreach (var item in selectedNode.OutEdges)
                    {
                        if (item is LineConnector)
                        {
                            Node node = (item as LineConnector).TailNode as Node;
                            LineConnector line = item as LineConnector;
                            if (!NodeClone.ContainsKey(node))
                            {
                                Node clone = PrepareNode(node);
                                Nodes.Add(clone);
                                Connect(selectedClone, clone, line);
                            }
                        }
                    }
                    break;

                case Diagram.ContextViewMode.Predecessors:

                    foreach (var item in selectedNode.InEdges)
                    {
                        if (item is LineConnector)
                        {
                            Node node = (item as LineConnector).HeadNode as Node;
                            LineConnector line = item as LineConnector;
                            if (!NodeClone.ContainsKey(node))
                            {
                                Node clone = PrepareNode(node);
                                Nodes.Add(clone);
                                Connect(clone, selectedClone, line);
                                AddDependent(node, clone);
                            }
                        }
                    }
                    break;

                case Diagram.ContextViewMode.Successors:
                    foreach (var item in selectedNode.OutEdges)
                    {
                        if (item is LineConnector)
                        {
                            Node node = (item as LineConnector).TailNode as Node;
                            LineConnector line = item as LineConnector;
                            if (!NodeClone.ContainsKey(node))
                            {
                                Node clone = PrepareNode(node);
                                Nodes.Add(clone);
                                Connect(selectedClone, clone, line);
                                AddDependent(node, clone);
                            }
                        }
                    }
                    break;
            }
        }
        
    }

    /// <summary>
    /// ContextViewMode to choose the type of view
    /// </summary>
    public enum ContextViewMode
    {
        Neighborhood,
        Predecessors,
        Successors
    }

    internal static class DispatcherExtension
    {
        static Dictionary<Action, object> Works = new Dictionary<Action, object>();

        public static void Invalidate(this Dispatcher Dispatcher, Action work)
        {
            if (!Works.ContainsKey(work))
            {
                Works.Add(work, null);
                Dispatcher.BeginInvoke(
                    new Action(
                        () =>
                        {
                            work();
                            Works.Remove(work);
                        }
                    ),
                    DispatcherPriority.Background,
                    null
                );
            }
        }
    }

    internal class RecycleBin
    {
        Dictionary<Type, Queue<object>> Garbage = new Dictionary<Type, Queue<object>>();

        public T Dequeue<T>()
        {
            Queue<object> q;
            Garbage.TryGetValue(typeof(T), out q);
            if (q != null)
            {
                if (q.Count == 0)
                {
                    return (T)Activator.CreateInstance(typeof(T));
                }
                else
                {
                    return (T)q.Dequeue();
                }
            }
            return (T) Activator.CreateInstance(typeof(T));
        }

        public void Enqueue(object obj, bool collection)
        {
            Queue<object> q;
            Garbage.TryGetValue(obj.GetType(), out q);
            if (collection)
            {
                IEnumerable col = obj as IEnumerable;
                if (col != null)
                {
                    bool hasElement = false;
                    foreach (var item in col)
                    {
                        hasElement = true;
                        break;
                    }
                    if (!hasElement)
                    {
                        return;
                    }
                    Garbage.TryGetValue(col.Cast<object>().FirstOrDefault().GetType(), out q);
                    if (q == null)
                    {
                        q = new Queue<object>((obj as IEnumerable).Cast<object>());
                        Garbage.Add(col.Cast<object>().FirstOrDefault().GetType(), q);
                    }
                    else
                    {
                        foreach (var item in col)
                        {
                            q.Enqueue(item);
                        }
                    }
                }
            }
            else
            {
                if (q != null)
                {
                    q.Enqueue(obj);
                }
                else
                {
                    q = new Queue<object>();
                    q.Enqueue(obj);
                    Garbage.Add(obj.GetType(), q);
                }
            }
        }
    }
}
