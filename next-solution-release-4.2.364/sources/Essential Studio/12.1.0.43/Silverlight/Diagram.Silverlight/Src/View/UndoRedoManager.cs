#region Copyright Syncfusion Inc. 2001 - 2014
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

namespace Syncfusion.Windows.Diagram
{
    internal class StackExt<T> : System.Collections.Generic.Stack<object>
    {
        internal bool m_CanPush = true;
        internal bool m_IsGroupedOper = false;
        internal event PushedEventHandler PushedEvent;

        public new void Push(object current)
        {
            if (m_CanPush)
            {
                if (m_IsGroupedOper)
                {
                    object top = this.Peek();
                    if (top is NodeOperation)
                    {
                        if (current is NodeOperation)
                        {
                            if (((NodeOperation)top).m_Node.Equals(((NodeOperation)current).m_Node))
                            {
                                return;
                            }
                        }
                    }
                    else if (top is LineOperation)
                    {
                        if (current is LineOperation)
                        {
                            if (((LineOperation)top).LC.Equals(((LineOperation)current).LC))
                            {
                                return;
                            }
                        }
                    }
                }
                if(current is string)
                {
                    if((current as string).Equals("Stop"))
                    {
                        m_IsGroupedOper = true;
                    }
                    else if ((current as string).Equals("Start"))
                    {
                        m_IsGroupedOper = false;
                        if (this.Peek() is string && (this.Peek() as string).Equals("Stop"))
                        {
                            this.Pop();
                            return;
                        }
                    }
                }
                base.Push(current);
                if (this.PushedEvent != null)
                {
                    PushedHandlerEventArgs evtArgs = new PushedHandlerEventArgs();
                    PushedEvent.Invoke(this, evtArgs);
                }
            }
        }
    }

    internal class PushedHandlerEventArgs : EventArgs
    {

    }


    internal delegate void PushedEventHandler(object sender, PushedHandlerEventArgs args);

    internal enum NodeOperations
    {
        Dragged,
        Resized,
        Rotated,
        Order,
        Added,
        Deleted,
    }

    internal enum LineOperations
    {
        Dragged,
        Resized,
        Rotated,
        Order,
        Added,
        Deleted,
    }

    internal struct NodeOperation 
    {
        internal NodeOperations m_Operations;
        internal Point m_Position;
        internal Size m_Size;
        internal double m_Angle;
        internal int m_ZOrder;
        internal Node m_Node;
        internal CollectionExt m_nodegroup;
        internal bool m_previouslygrouped;

        internal NodeOperation(NodeOperations oper, Node node, Point pos, Size size, double angle, int zorder,CollectionExt nodegroup,bool previousgroup)
        {
            m_Operations = oper;
            m_Position = pos;
            m_Size = size;
            m_Angle = angle;
            m_ZOrder = zorder;
            m_Node = node;
            m_nodegroup = nodegroup;
            m_previouslygrouped = previousgroup;
        }

        internal NodeOperation(NodeOperations oper, Node node)
        {
            m_Operations = oper;
            m_Position = new Point(node.PxOffsetX,node.PxOffsetY);
            m_Size = new System.Windows.Size(node.Width, node.Height);
            double angle = 0;
            if (node.RenderTransform != null && node.RenderTransform is RotateTransform)
            {
                angle = (node.RenderTransform as RotateTransform).Angle;
            }
            m_Angle = angle;
            m_ZOrder = Canvas.GetZIndex(node);
            m_Node = node;
            m_previouslygrouped = node.IsGrouped;
            m_nodegroup = node.Groups;
        }
    }

    internal struct LineOperation 
    {
        internal LineOperations Oper;
        internal Node HeadNode;
        internal Node TailNode;
        internal Point StartPointPosition;
        internal Point EndPointPosition;
        internal LineConnector LC;
        internal double ArcHeight;
        internal SweepDirection ArcDirection;

        internal LineOperation(LineOperations oper, LineConnector lc, Node headNode, Node tailNode, Point headPoint, Point tailPoint, double archeight, SweepDirection arcdirection)
        {
            Oper = oper;
            LC = lc;
            HeadNode = headNode;
            TailNode = tailNode;
            StartPointPosition = headPoint;
            EndPointPosition = tailPoint;
            ArcHeight = archeight;
            ArcDirection = arcdirection;
        }

        internal LineOperation(LineOperations oper, LineConnector lc)
        {
            Oper = oper;
            LC = lc;
            HeadNode = lc.HeadNode as Node;
            TailNode = lc.TailNode as Node;
            StartPointPosition = lc.PxStartPointPosition;
            EndPointPosition = lc.PxEndPointPosition;
            ArcHeight = lc.ArcHeight;
            ArcDirection = lc.ArcDirection;
        }
    }

    public partial class DiagramView
    {
        internal StackExt<object> tUndoStack { get; set; }
        internal StackExt<object> tRedoStack { get; set; }
        
        public static void UndoRedoCommand(DiagramView mDiagramView, bool IsUndo)
        {
            StackExt<object> UndoStack = mDiagramView.tUndoStack;
            StackExt<object> RedoStack = mDiagramView.tRedoStack;

            if (!IsUndo)
            {
                StackExt<object> temp = mDiagramView.tRedoStack;
                UndoStack = temp;
                RedoStack = mDiagramView.tUndoStack;
            }
            if (UndoStack.Count == 0)
            {
                return;
            }
            Object obj = UndoStack.Pop();
            bool contin = false;
            if (obj is string && (obj as string).Equals("Start"))
            {
                RedoStack.Push("Stop");
                contin = true;
            }
            do
            {
                if (obj is NodeOperation)
                {
                    NodeOperation nodeOper = (NodeOperation)obj;
                    NodeOperation redoOper;
                    switch (nodeOper.m_Operations)
                    {
                        case NodeOperations.Added:
                            redoOper = new NodeOperation(NodeOperations.Deleted, nodeOper.m_Node);
                            RedoStack.Push(redoOper);
                            PerformNodeDelete(nodeOper);                     
                            mDiagramView.dc.Model.Nodes.Remove(nodeOper.m_Node);
                            break;
                        case NodeOperations.Deleted:
                            redoOper = new NodeOperation(NodeOperations.Added, nodeOper.m_Node);
                            RedoStack.Push(redoOper);
                            PerformNodeAdd(nodeOper);
                            if (!mDiagramView.dc.Model.Nodes.Contains(nodeOper.m_Node))
                                mDiagramView.dc.Model.Nodes.Add(nodeOper.m_Node);
                            break;
                        case NodeOperations.Dragged:
                            redoOper = new NodeOperation(NodeOperations.Dragged, nodeOper.m_Node);
                            RedoStack.Push(redoOper);
                            nodeOper.m_Node.PxOffsetX = nodeOper.m_Position.X;
                            nodeOper.m_Node.PxOffsetY = nodeOper.m_Position.Y;
                            (mDiagramView.Page as DiagramPage).InvalidateMeasure();
                            break;
                        case NodeOperations.Order:
                            redoOper = new NodeOperation(NodeOperations.Order, nodeOper.m_Node);
                            RedoStack.Push(redoOper);
                            Canvas.SetZIndex(nodeOper.m_Node, nodeOper.m_ZOrder);
                            break;
                        case NodeOperations.Resized:
                            redoOper = new NodeOperation(NodeOperations.Resized, nodeOper.m_Node);
                            RedoStack.Push(redoOper);
                            nodeOper.m_Node.Width = nodeOper.m_Size.Width;
                            nodeOper.m_Node.Height = nodeOper.m_Size.Height;
                            nodeOper.m_Node.PxOffsetX = nodeOper.m_Position.X;
                            nodeOper.m_Node.PxOffsetY = nodeOper.m_Position.Y;
                            break;
                        case NodeOperations.Rotated:
                            redoOper = new NodeOperation(NodeOperations.Rotated, nodeOper.m_Node);
                            RedoStack.Push(redoOper);
                            nodeOper.m_Node.RenderTransform = new RotateTransform() { Angle = nodeOper.m_Angle };
                            break;
                    }
                            nodeOper.m_Node.UpdateLayout();
                }
                else if (obj is LineOperation)
                {
                    LineOperation lineOper = (LineOperation)obj;
                    switch (lineOper.Oper)
                    {
                        case LineOperations.Added:
                            RedoStack.Push(new LineOperation(LineOperations.Deleted,lineOper.LC));
                            mDiagramView.dc.Model.Connections.Remove(lineOper.LC);
                            break;
                        case LineOperations.Deleted:
                            RedoStack.Push(new LineOperation(LineOperations.Added, lineOper.LC));
                            if (!mDiagramView.dc.Model.Connections.Contains(lineOper.LC))
                                mDiagramView.dc.Model.Connections.Add(lineOper.LC);
                            break;
                        case LineOperations.Dragged:
                            RedoStack.Push(new LineOperation(LineOperations.Dragged, lineOper.LC));
                            if (lineOper.HeadNode != lineOper.LC.HeadNode)
                            {
                                lineOper.LC.HeadNode = lineOper.HeadNode;
                            }
                            if (lineOper.TailNode != lineOper.LC.TailNode)
                            {
                                lineOper.LC.TailNode = lineOper.TailNode;
                            }
                            if (!lineOper.StartPointPosition.Equals(lineOper.LC.PxStartPointPosition))
                            {
                                lineOper.LC.PxStartPointPosition = lineOper.StartPointPosition;
                            }
                            if (!lineOper.EndPointPosition.Equals(lineOper.LC.PxEndPointPosition))
                            {
                                lineOper.LC.PxEndPointPosition = lineOper.EndPointPosition;
                            }
                            break;
                        case LineOperations.Resized:
                            RedoStack.Push(new LineOperation(LineOperations.Resized, lineOper.LC));
                            lineOper.LC.ArcHeight = lineOper.ArcHeight;                            
                            break;
                        case LineOperations.Rotated:
                            RedoStack.Push(new LineOperation(LineOperations.Rotated, lineOper.LC));                            
                            lineOper.LC.ArcDirection = lineOper.ArcDirection;
                            break;
                        case LineOperations.Order:
                            break;
                    }
                            lineOper.LC.UpdateConnectorPathGeometry();
                }
                if (contin)
                {
                    object tempObj = UndoStack.Peek();
                    if ((tempObj is string))
                    {
                        if ((tempObj as string).Equals("Stop"))
                        {
                            UndoStack.Pop();
                            RedoStack.Push("Start");
                            contin = false;
                        }
                    }
                    else
                    {
                        contin = true;
                    }
                }
                if (contin)
                {
                    obj = UndoStack.Pop();
                }
            } while (contin);
        }
        public static void Undo(DiagramView mDiagramView)
        {
            if (mDiagramView.UndoRedoEnabled)
            {
                mDiagramView.Undone = true;
                UndoRedoCommand(mDiagramView, true);
                mDiagramView.Undone = false;
                (mDiagramView.Page as DiagramPage).InvalidateMeasure();
                (mDiagramView.Page as DiagramPage).InvalidateArrange();
            }
        }

        public static void Redo(DiagramView mDiagramView)
        {
            if (mDiagramView.UndoRedoEnabled)
            {
                mDiagramView.Redone = true;
                UndoRedoCommand(mDiagramView, false);
                mDiagramView.Redone = false;
                (mDiagramView.Page as DiagramPage).InvalidateMeasure();
                (mDiagramView.Page as DiagramPage).InvalidateArrange();
            }
        }

        private static void PerformNodeDelete(NodeOperation nodeOper)
        {
            if (nodeOper.m_Node.IsGrouped)
            {
                foreach (Group g in nodeOper.m_Node.Groups)
                {
                    g.NodeChildren.Remove(nodeOper.m_Node);
                }
            }

            if (nodeOper.m_Node is Group)
            {
                foreach (INodeGroup element in (nodeOper.m_Node as Group).NodeChildren)
                {
                    element.Groups.Remove(nodeOper.m_Node);
                    if (element.Groups.Count == 0)
                    {
                        element.IsGrouped = false;
                    }
                }
            }
        }

        private static void PerformNodeAdd(NodeOperation nodeOper)
        {
            Canvas.SetZIndex(nodeOper.m_Node, nodeOper.m_ZOrder);
            if (nodeOper.m_Node is Group)
            {
                foreach (INodeGroup element in (nodeOper.m_Node as Group).NodeChildren)
                {
                    if (!element.Groups.Contains(nodeOper.m_Node))
                    {
                        element.Groups.Add(nodeOper.m_Node);
                    }

                    element.IsGrouped = true;
                }
            }

            if (nodeOper.m_Node.IsGrouped)
            {
                foreach (Group g in nodeOper.m_Node.Groups)
                {
                    if (!(g.NodeChildren.Contains(nodeOper.m_Node)))
                    {
                        g.NodeChildren.Add(nodeOper.m_Node);
                    }
                   
                    foreach (ICommon shape in g.NodeChildren)
                    {
                        (shape as INodeGroup).IsGrouped = true;
                    }
                }
            }
            //if (nodeOper.m_previouslygrouped)
            //{
            //    foreach (Group g in nodeOper.m_nodegroup)
            //    {
            //        if (!(g.NodeChildren.Contains(nodeOper.m_Node)))
            //        {
            //            g.NodeChildren.Add(nodeOper.m_Node);
            //        }

            //        foreach (ICommon shape in g.NodeChildren)
            //        {
            //            if((shape as INodeGroup).IsGrouped==false)
            //            (shape as INodeGroup).IsGrouped = true;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Clears the undo redo stack.
        /// </summary>
        public void ClearUndoRedoStack()
        {
            this.tUndoStack.Clear();
            this.tRedoStack.Clear();
        }


        void tUndoStack_PushedEvent(object sender, PushedHandlerEventArgs args)
        {
            if (!this.Undone && !this.Redone)
            {
                this.tRedoStack.Clear();
            }
        }
    }
}
