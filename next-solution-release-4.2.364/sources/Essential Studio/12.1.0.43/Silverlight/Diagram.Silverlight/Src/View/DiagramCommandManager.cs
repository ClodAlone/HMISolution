// <copyright file="DiagramCommandManager.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the Diagram Command manager.
    /// </summary>
    public class DiagramCommandManager
    {
        internal DiagramModel Model;

        internal DiagramView View;

        /// <summary>
        /// Used to store the group count.
        /// </summary>
        private static int i = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramCommandManager"/> class.
        /// </summary>
        public DiagramCommandManager()
        {
        }

        #region Clipboard Commands

        /// <summary>
        /// DeleteCommand
        /// </summary>
        public void OnDeleteCommand(object sender)
        {
            if (View.IsPageEditable)
            {
                if (View.SelectionList.Count > 0)
                {
                    (View as DiagramView).DeleteObjects((View as DiagramView));
                }
            }
        }
        public void OnCutCommand(object sender)
        {
            if (View.IsPageEditable && View.IsCutEnabled)
            {
                if (View.SelectionList.Count > 0)
                {
                    CopyPasteManager cpm = new CopyPasteManager(View as DiagramView);
                    cpm.cut();
                }
            }
        }
        public void OnCopyCommand(object sender)
        {
            if (View.IsPageEditable && View.IsCopyEnabled)
            {
                if (View.SelectionList.Count > 0)
                {
                    CopyPasteManager cpm = new CopyPasteManager(View as DiagramView);
                    cpm.copy();
                }
            }
        }
        public void OnPasteCommand(object sender)
        {
            if (View.IsPageEditable && View.IsPasteEnabled)
            {
                if (CopyPasteManager.IsValidStringContent)
                {
                    CopyPasteManager cpm = new CopyPasteManager(View as DiagramView);
                    cpm.paste();
                }
            }
        }
        #endregion

        #region Undo Redo commands

        public void OnUndoCommand(object sender)
        {
            DiagramView.Undo(View as DiagramView);
        }
        public void ZoomInCommand(object sender)
        {
            DiagramView.ZoomIn(View as DiagramView);


        }

        public void ZoomOutCommand(object sender)
        {
            DiagramView.ZoomOut(View as DiagramView);
        }

        public void OnRedoCommand(object sender)
        {
            DiagramView.Redo(View as DiagramView);
        }
        public void ResetCommand(object sender)
        {
            DiagramView.Reset(View as DiagramView);
        }

        #endregion

        #region SelectAll Commands
        public void OnSelectAllCommand(object sender)
        {
            DiagramView.SelectAll(View as DiagramView);
        }
        #endregion

        #region Nudge commands

        /// <summary>
        /// Invoked when the MoveUp Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        public void OnMoveUpCommand(object sender)
        {
            DiagramView.MoveUp(View as DiagramView);
        }

        /// <summary>
        /// Invoked when the MoveDown Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        public void OnMoveDownCommand(object sender)
        {
            DiagramView.MoveDown(View as DiagramView);
        }

        /// <summary>
        /// Invoked when the MoveLeft Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        public void OnMoveLeftCommand(object sender)
        {
            DiagramView.MoveLeft(View as DiagramView);
        }

        /// <summary>
        /// Invoked when the MoveRight Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        public void OnMoveRightCommand(object sender)
        {
            DiagramView.MoveRight(View as DiagramView);
        }

        #endregion

        #region Group Commands
        /// <summary>
        /// Invoked when the GroupNodes Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void GroupNodes(object sender)
        {
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }

            DiagramControl dc = DiagramPage.GetDiagramControl(view);

            if (view.SelectionList.Count > 1)
            {
                Group g = new Group();
                foreach (INodeGroup shape in view.SelectionList)
                {
                    if (shape is LineConnector)
                    {
                        //if ((shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            GroupEventArgs newEventArgs = new GroupEventArgs(g, null, shape as LineConnector);
                            view.OnGrouping(g, newEventArgs);
                            view.oldselectionlist.Remove(shape as Node);
                            g.AddChild(shape);
                        }

                        continue;
                    }
                    if (shape is Node)
                    {
                        GroupEventArgs newEventArgs1 = new GroupEventArgs(g, shape as Node, null);
                        view.OnGrouping(g, newEventArgs1);
                        view.oldselectionlist.Remove(shape as Node);
                    }
                    g.AddChild(shape);
                }

                if (string.IsNullOrEmpty(g.Name))
                {
                    g.Name = "sync_dgm_group" + i.ToString();
                    i++;
                }
                GroupEventArgs newEvenArgs = new GroupEventArgs(g, null, null);
                view.OnGrouped(g, newEvenArgs);
                if (!this.Model.Nodes.Contains(g))
                    this.Model.Nodes.Add(g);
                dc.View.SelectionList.Clear();
                dc.View.SelectionList.Select(g);

            }
        }

        /// <summary>
        /// Invoked when the UnGroupNodes Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void UnGroupNodes(object sender)
        {
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            DiagramControl dc = DiagramPage.GetDiagramControl(view);
            foreach (ICommon shape in view.SelectionList)
            {
                if (shape is Group)
                {
                    foreach (INodeGroup element in (shape as Group).NodeChildren)
                    {
                        if (element is Node)
                        {
                            UnGroupEventArgs newEventArgs1 = new UnGroupEventArgs(shape as Group, element as Node, null);
                            view.OnUnGrouping(shape as Node, newEventArgs1);
                        }
                        else
                        {
                            UnGroupEventArgs newEventArgs1 = new UnGroupEventArgs(shape as Group, null, element as LineConnector);
                            view.OnUnGrouping(shape as Node, newEventArgs1);
                        }
                        view.oldselectionlist.Remove(shape as Node);
                        element.Groups.Remove(shape);
                        if (element.Groups.Count == 0)
                        {
                            element.IsGrouped = false;
                        }
                    }

                    UnGroupEventArgs newEventArgs = new UnGroupEventArgs(shape as Group, null, null);
                    view.OnunGrouped(shape as Group, newEventArgs);
                    (shape as Group).NodeChildren.Clear();
                    CollectionExt.Cleared = false;
                    dc.Model.Nodes.Remove(shape);
                }
            }

            dc.View.SelectionList.Clear();
        }

        #endregion

        #region Size Commands

        /// <summary>
        /// Invoked when the SameSize Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void SameSizeCommand(object sender)
        {
            bool donotexecute = false;
            double widthrefvalue = 0;
            double heightrefvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view != null)
            {
                view.m_IsCommandInProgress = true;
                view.tUndoStack.Push("Stop");
            }
            if (view.SelectionList.Count > 1)
            {
                if (view.SelectionList[0] is Node)
                {
                    widthrefvalue = (view.SelectionList[0] as Node).Width;
                    heightrefvalue = (view.SelectionList[0] as Node).Height;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    widthrefvalue = Math.Abs((view.SelectionList[0] as LineConnector).PxEndPointPosition.X - (view.SelectionList[0] as LineConnector).PxStartPointPosition.X);
                    heightrefvalue = Math.Abs((view.SelectionList[0] as LineConnector).PxEndPointPosition.Y - (view.SelectionList[0] as LineConnector).PxStartPointPosition.Y);
                }
                else
                {
                    donotexecute = true;
                }
            }
            else
            {
                donotexecute = true;
            }

            if (!donotexecute)
            {
                foreach (ICommon shape in view.SelectionList)
                {
                    if (shape is Group && (view.SelectionList[0] != (shape as Group)))
                    {
                        foreach (INodeGroup child in (shape as Group).NodeChildren)
                        {
                            if (child is Node)
                            {
                                (child as Node).Oldsize = new Size((child as Node).Width, (child as Node).Height);
                                (child as Node).Width = widthrefvalue;
                                (child as Node).Height = heightrefvalue;
                                //Size m_newsize = new Size((child as Node).Width, (child as Node).Height);
                                //foreach (ConnectionPort port in (child as Node).Ports)
                                //{
                                //    if ((child as Node).Oldsize.Width != 0 && (child as Node).Oldsize.Height != 0)
                                //    {
                                //        port.PreviousPortPoint = new Point(port.Left, port.Top);
                                //        port.Left = (m_newsize.Width / (child as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                //        port.Top = (m_newsize.Height / (child as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                //        TranslateTransform tr = new TranslateTransform() { X = port.Left, Y = port.Top };
                                //        port.RenderTransform = tr;
                                //    }
                                //}
                            }
                            else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                            {
                                double linewidth = Math.Abs((child as LineConnector).PxEndPointPosition.X - (child as LineConnector).PxStartPointPosition.X);
                                double extrawidth = widthrefvalue - linewidth;
                                double lineheight = Math.Abs((child as LineConnector).PxEndPointPosition.Y - (child as LineConnector).PxStartPointPosition.Y);
                                double extraheight = heightrefvalue - lineheight;
                                if ((child as LineConnector).PxStartPointPosition.X < (child as LineConnector).PxEndPointPosition.X)
                                {
                                    ////(child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X - extrasize / 2, (child as LineConnector).PxStartPointPosition.Y);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + extrawidth, (child as LineConnector).PxEndPointPosition.Y + extraheight);
                                }
                                else
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + extrawidth, (child as LineConnector).PxStartPointPosition.Y + extraheight);
                                    ////(child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X - extrasize / 2, (child as LineConnector).PxEndPointPosition.Y);
                                }
                                (child as LineConnector).UpdateConnectorPathGeometry();
                            }
                        }
                    }
                    else if (shape is Node)
                    {
                        (shape as Node).Oldsize = new Size((shape as Node).Width, (shape as Node).Height);
                        (shape as Node).Width = widthrefvalue;
                        (shape as Node).Height = heightrefvalue;
                        //Size m_newsize = new Size((shape as Node).Width, (shape as Node).Height);
                        //foreach (ConnectionPort port in (shape as Node).Ports)
                        //{
                        //    if ((shape as Node).Oldsize.Width != 0 && (shape as Node).Oldsize.Height != 0)
                        //    {
                        //        port.PreviousPortPoint = new Point(port.Left, port.Top);
                        //        port.Left = (m_newsize.Width / (shape as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                        //        port.Top = (m_newsize.Height / (shape as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                        //        TranslateTransform tr = new TranslateTransform() { X = port.Left, Y = port.Top };
                        //        port.RenderTransform = tr;
                        //    }
                        //}
                    }
                    else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                    {
                        double linewidth = Math.Abs((shape as LineConnector).PxEndPointPosition.X - (shape as LineConnector).PxStartPointPosition.X);
                        double extrawidth = widthrefvalue - linewidth;
                        double lineheight = Math.Abs((shape as LineConnector).PxEndPointPosition.Y - (shape as LineConnector).PxStartPointPosition.Y);
                        double extraheight = heightrefvalue - lineheight;
                        if ((shape as LineConnector).PxStartPointPosition.X < (shape as LineConnector).PxEndPointPosition.X)
                        {
                            //// (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X - extrasize / 2, (shape as LineConnector).PxStartPointPosition.Y);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X + extrawidth, (shape as LineConnector).PxEndPointPosition.Y + extraheight);
                        }
                        else
                        {
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X + extrawidth, (shape as LineConnector).PxStartPointPosition.Y + extraheight);
                            //// (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X - extrasize / 2, (shape as LineConnector).PxEndPointPosition.Y);
                        }
                        (shape as LineConnector).UpdateConnectorPathGeometry();
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();

            if (view != null)
            {
                view.m_IsCommandInProgress = false;
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the SameWidth Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void SameWidthCommand(object sender)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view != null)
            {
                view.m_IsCommandInProgress = true;
                view.tUndoStack.Push("Stop");
            }
            if (view.SelectionList.Count > 1)
            {
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).Width;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = Math.Abs((view.SelectionList[0] as LineConnector).PxEndPointPosition.X - (view.SelectionList[0] as LineConnector).PxStartPointPosition.X);
                }
                else
                {
                    donotexecute = true;
                }
            }
            else
            {
                donotexecute = true;
            }

            if (!donotexecute)
            {
                foreach (ICommon shape in view.SelectionList)
                {
                    if (shape is Group && (view.SelectionList[0] != (shape as Group)))
                    {
                        foreach (INodeGroup child in (shape as Group).NodeChildren)
                        {
                            if (child is Node)
                            {
                                (child as Node).Oldsize = new Size((child as Node).Width, (child as Node).Height);
                                (child as Node).Width = refvalue;
                                //Size m_newsize = new Size((child as Node).Width, (child as Node).Height);
                                //foreach (ConnectionPort port in (child as Node).Ports)
                                //{
                                //    if ((child as Node).Oldsize.Width != 0 && (child as Node).Oldsize.Height != 0)
                                //    {
                                //        port.PreviousPortPoint = new Point(port.Left, port.Top);
                                //        port.Left = (m_newsize.Width / (child as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                //        port.Top = (m_newsize.Height / (child as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                //        TranslateTransform tr = new TranslateTransform() { X = port.Left, Y = port.Top };
                                //        port.RenderTransform = tr;
                                //    }
                                //}
                            }
                            else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                            {
                                double linewidth = Math.Abs((child as LineConnector).PxEndPointPosition.X - (child as LineConnector).PxStartPointPosition.X);
                                double extrasize = refvalue - linewidth;
                                if ((child as LineConnector).PxStartPointPosition.X < (child as LineConnector).PxEndPointPosition.X)
                                {
                                    ////(child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X - extrasize / 2, (child as LineConnector).PxStartPointPosition.Y);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + extrasize, (child as LineConnector).PxEndPointPosition.Y);
                                }
                                else
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + extrasize, (child as LineConnector).PxStartPointPosition.Y);
                                    ////(child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X - extrasize / 2, (child as LineConnector).PxEndPointPosition.Y);
                                }
                                (child as LineConnector).UpdateConnectorPathGeometry();
                            }
                        }
                    }
                    else if (shape is Node)
                    {
                        (shape as Node).Oldsize = new Size((shape as Node).Width, (shape as Node).Height);
                        (shape as Node).Width = refvalue;
                        //Size m_newsize = new Size((shape as Node).Width, (shape as Node).Height);
                        //foreach (ConnectionPort port in (shape as Node).Ports)
                        //{
                        //    if ((shape as Node).Oldsize.Width != 0 && (shape as Node).Oldsize.Height != 0)
                        //    {
                        //        port.PreviousPortPoint = new Point(port.Left, port.Top);
                        //        port.Left = (m_newsize.Width / (shape as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                        //        port.Top = (m_newsize.Height / (shape as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                        //        TranslateTransform tr = new TranslateTransform() { X = port.Left, Y = port.Top };
                        //        port.RenderTransform = tr;
                        //    }
                        //}
                    }
                    else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                    {
                        double linewidth = Math.Abs((shape as LineConnector).PxEndPointPosition.X - (shape as LineConnector).PxStartPointPosition.X);
                        double extrasize = refvalue - linewidth;
                        if ((shape as LineConnector).PxStartPointPosition.X < (shape as LineConnector).PxEndPointPosition.X)
                        {
                            //// (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X - extrasize / 2, (shape as LineConnector).PxStartPointPosition.Y);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X + extrasize, (shape as LineConnector).PxEndPointPosition.Y);
                        }
                        else
                        {
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X + extrasize, (shape as LineConnector).PxStartPointPosition.Y);
                            //// (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X - extrasize / 2, (shape as LineConnector).PxEndPointPosition.Y);
                        }
                        (shape as LineConnector).UpdateConnectorPathGeometry();
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
            if (view != null)
            {
                view.m_IsCommandInProgress = false;
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the SameHeight Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void SameHeightCommand(object sender)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view != null)
            {
                view.m_IsCommandInProgress = true;
                view.tUndoStack.Push("Stop");
            }
            if (view.SelectionList.Count > 1)
            {
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).Height;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = Math.Abs((view.SelectionList[0] as LineConnector).PxEndPointPosition.Y - (view.SelectionList[0] as LineConnector).PxStartPointPosition.Y);
                }
                else
                {
                    donotexecute = true;
                }
            }
            else
            {
                donotexecute = true;
            }

            if (!donotexecute)
            {
                foreach (ICommon shape in view.SelectionList)
                {
                    if (shape is Group && (view.SelectionList[0] != (shape as Group)))
                    {
                        foreach (INodeGroup child in (shape as Group).NodeChildren)
                        {
                            if (child is Node)
                            {
                                (child as Node).Oldsize = new Size((child as Node).Width, (child as Node).Height);
                                (child as Node).Height = refvalue;
                                //Size m_newsize = new Size((child as Node).Width, (child as Node).Height);
                                //foreach (ConnectionPort port in (child as Node).Ports)
                                //{
                                //    if ((child as Node).Oldsize.Width != 0 && (child as Node).Oldsize.Height != 0)
                                //    {
                                //        port.PreviousPortPoint = new Point(port.Left, port.Top);
                                //        port.Left = (m_newsize.Width / (child as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                //        port.Top = (m_newsize.Height / (child as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                //        TranslateTransform tr = new TranslateTransform() { X = port.Left, Y = port.Top };
                                //        port.RenderTransform = tr;
                                //    }
                                //}
                            }
                            else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                            {
                                double lineheight = Math.Abs((child as LineConnector).PxEndPointPosition.Y - (child as LineConnector).PxStartPointPosition.Y);
                                double extrasize = refvalue - lineheight;
                                if ((child as LineConnector).PxStartPointPosition.Y < (child as LineConnector).PxEndPointPosition.Y)
                                {
                                    //// (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y - extrasize / 2);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + extrasize);
                                }
                                else
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + extrasize);
                                    //// (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y - extrasize / 2);
                                }
                                (child as LineConnector).UpdateConnectorPathGeometry();
                            }
                        }
                    }
                    else if (shape is Node)
                    {
                        (shape as Node).Oldsize = new Size((shape as Node).Width, (shape as Node).Height);
                        (shape as Node).Height = refvalue;
                        //Size m_newsize = new Size((shape as Node).Width, (shape as Node).Height);
                        //foreach (ConnectionPort port in (shape as Node).Ports)
                        //{
                        //    if ((shape as Node).Oldsize.Width != 0 && (shape as Node).Oldsize.Height != 0)
                        //    {
                        //        port.PreviousPortPoint = new Point(port.Left, port.Top);
                        //        port.Left = (m_newsize.Width / (shape as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                        //        port.Top = (m_newsize.Height / (shape as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                        //        TranslateTransform tr = new TranslateTransform() { X = port.Left, Y = port.Top };
                        //        port.RenderTransform = tr;
                        //    }
                        //}
                    }
                    else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                    {
                        double lineheight = Math.Abs((shape as LineConnector).PxEndPointPosition.Y - (shape as LineConnector).PxStartPointPosition.Y);
                        double extrasize = refvalue - lineheight;
                        if ((shape as LineConnector).PxStartPointPosition.X < (shape as LineConnector).PxEndPointPosition.X)
                        {
                            //// (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y - extrasize / 2);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y + extrasize);
                        }
                        else
                        {
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y + extrasize);
                            ////(shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y - extrasize / 2);
                        }
                        (shape as LineConnector).UpdateConnectorPathGeometry();
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
            if (view != null)
            {
                view.m_IsCommandInProgress = false;
                view.tUndoStack.Push("Start");
            }
        }

        #endregion

        #region Space Commands

        /// <summary>
        /// Invoked when the SpaceDown Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnSpaceDownCommand(object sender)
        {
            bool donotexecute = false;
            double topvalue = 0;
            double bottomvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view.SelectionList.Count > 2)
            {

                if (view != null)
                {
                    view.m_IsCommandInProgress = true;
                    view.tUndoStack.Push("Stop");
                }
                if (view.SelectionList[0] is Node)
                {
                    topvalue = (view.SelectionList[0] as Node).PxOffsetY;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    if ((view.SelectionList[0] as LineConnector).PxStartPointPosition.Y < (view.SelectionList[0] as LineConnector).PxEndPointPosition.Y)
                    {
                        topvalue = (view.SelectionList[0] as LineConnector).PxStartPointPosition.Y;
                    }
                    else
                    {
                        topvalue = (view.SelectionList[0] as LineConnector).PxEndPointPosition.Y;
                    }
                }
                else
                {
                    donotexecute = true;
                }

                if (view.SelectionList[view.SelectionList.Count - 1] is Node)
                {
                    bottomvalue = (view.SelectionList[view.SelectionList.Count - 1] as Node).PxOffsetY;
                }
                else if (view.SelectionList[view.SelectionList.Count - 1] is LineConnector && (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).HeadNode == null && (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).TailNode == null)
                {
                    if ((view.SelectionList[view.SelectionList.Count - 1] as LineConnector).PxStartPointPosition.Y < (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).PxEndPointPosition.Y)
                    {
                        bottomvalue = (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).PxStartPointPosition.Y;
                    }
                    else
                    {
                        bottomvalue = (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).PxEndPointPosition.Y;
                    }
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    double count = view.SelectionList.Count - 1;
                    double refvalue = (bottomvalue - topvalue) / count;
                    for (int i = 1; i < view.SelectionList.Count - 1; i++)
                    {
                        if (view.SelectionList[i] is Group)
                        {
                            double oldy = (view.SelectionList[i] as Group).PxOffsetY;
                            (view.SelectionList[i] as Group).PxOffsetY = topvalue + (i * refvalue);
                            double translateoffset = (view.SelectionList[i] as Group).PxOffsetY - oldy;
                            foreach (INodeGroup child in (view.SelectionList[i] as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetY += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + translateoffset);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + translateoffset);
                                }
                            }
                        }
                        else if (view.SelectionList[i] is Node)
                        {
                            (view.SelectionList[i] as Node).PxOffsetY = topvalue + (i * refvalue);
                        }
                        else if (view.SelectionList[i] is LineConnector && (view.SelectionList[i] as LineConnector).HeadNode == null && (view.SelectionList[i] as LineConnector).TailNode == null)
                        {
                            if ((view.SelectionList[i] as LineConnector).PxStartPointPosition.Y < (view.SelectionList[i] as LineConnector).PxEndPointPosition.Y)
                            {
                                double oldy = (view.SelectionList[i] as LineConnector).PxStartPointPosition.Y;
                                (view.SelectionList[i] as LineConnector).PxStartPointPosition = new Point((view.SelectionList[i] as LineConnector).PxStartPointPosition.X, topvalue + (i * refvalue));
                                double offset = (view.SelectionList[i] as LineConnector).PxStartPointPosition.Y - oldy;
                                (view.SelectionList[i] as LineConnector).PxEndPointPosition = new Point((view.SelectionList[i] as LineConnector).PxEndPointPosition.X, (view.SelectionList[i] as LineConnector).PxEndPointPosition.Y + offset);
                            }
                            else
                            {
                                double oldy = (view.SelectionList[i] as LineConnector).PxEndPointPosition.Y;
                                (view.SelectionList[i] as LineConnector).PxEndPointPosition = new Point((view.SelectionList[i] as LineConnector).PxEndPointPosition.X, topvalue + (i * refvalue));
                                double offset = (view.SelectionList[i] as LineConnector).PxEndPointPosition.Y - oldy;
                                (view.SelectionList[i] as LineConnector).PxStartPointPosition = new Point((view.SelectionList[i] as LineConnector).PxStartPointPosition.X, (view.SelectionList[i] as LineConnector).PxStartPointPosition.Y + offset);
                            }
                        }
                    }
                }

                if (view != null)
                {
                    view.m_IsCommandInProgress = false;
                    view.tUndoStack.Push("Start");
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }

        /// <summary>
        /// Invoked when the SpaceAcross Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnSpaceAcrossCommand(object sender)
        {
            bool donotexecute = false;
            double leftvalue = 0;
            double rightvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }

            if (view.SelectionList.Count > 2)
            {
                if (view != null)
                {
                    view.m_IsCommandInProgress = true;
                    view.tUndoStack.Push("Stop");
                }
                if (view.SelectionList[0] is Node)
                {
                    leftvalue = (view.SelectionList[0] as Node).PxOffsetX;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    if ((view.SelectionList[0] as LineConnector).PxStartPointPosition.X < (view.SelectionList[0] as LineConnector).PxEndPointPosition.X)
                    {
                        leftvalue = (view.SelectionList[0] as LineConnector).PxStartPointPosition.X;
                    }
                    else
                    {
                        leftvalue = (view.SelectionList[0] as LineConnector).PxEndPointPosition.X;
                    }
                }
                else
                {
                    donotexecute = true;
                }

                if (view.SelectionList[view.SelectionList.Count - 1] is Node)
                {
                    rightvalue = (view.SelectionList[view.SelectionList.Count - 1] as Node).PxOffsetX;
                }
                else if (view.SelectionList[view.SelectionList.Count - 1] is LineConnector && (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).HeadNode == null && (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).TailNode == null)
                {
                    if ((view.SelectionList[view.SelectionList.Count - 1] as LineConnector).PxStartPointPosition.X < (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).PxEndPointPosition.X)
                    {
                        rightvalue = (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).PxStartPointPosition.X;
                    }
                    else
                    {
                        rightvalue = (view.SelectionList[view.SelectionList.Count - 1] as LineConnector).PxEndPointPosition.X;
                    }
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    double count = view.SelectionList.Count - 1;
                    double refvalue = (rightvalue - leftvalue) / count;
                    for (int i = 1; i < view.SelectionList.Count - 1; i++)
                    {
                        if (view.SelectionList[i] is Group)
                        {
                            double oldx = (view.SelectionList[i] as Group).PxOffsetX;
                            (view.SelectionList[i] as Group).PxOffsetX = leftvalue + (i * refvalue);
                            double translateoffset = (view.SelectionList[i] as Group).PxOffsetX - oldx;
                            foreach (INodeGroup child in (view.SelectionList[i] as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetX += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + translateoffset, (child as LineConnector).PxStartPointPosition.Y);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + translateoffset, (child as LineConnector).PxEndPointPosition.Y);
                                }
                            }
                        }
                        else if (view.SelectionList[i] is Node)
                        {
                            (view.SelectionList[i] as Node).PxOffsetX = leftvalue + (i * refvalue);
                        }
                        else if (view.SelectionList[i] is LineConnector && (view.SelectionList[i] as LineConnector).HeadNode == null && (view.SelectionList[i] as LineConnector).TailNode == null)
                        {
                            if ((view.SelectionList[i] as LineConnector).PxStartPointPosition.X < (view.SelectionList[i] as LineConnector).PxEndPointPosition.X)
                            {
                                double oldx = (view.SelectionList[i] as LineConnector).PxStartPointPosition.X;
                                (view.SelectionList[i] as LineConnector).PxStartPointPosition = new Point(leftvalue + (i * refvalue), (view.SelectionList[i] as LineConnector).PxStartPointPosition.Y);
                                double offset = (view.SelectionList[i] as LineConnector).PxStartPointPosition.X - oldx;
                                (view.SelectionList[i] as LineConnector).PxEndPointPosition = new Point((view.SelectionList[i] as LineConnector).PxEndPointPosition.X + offset, (view.SelectionList[i] as LineConnector).PxEndPointPosition.Y);
                            }
                            else
                            {
                                double oldx = (view.SelectionList[i] as LineConnector).PxEndPointPosition.X;
                                (view.SelectionList[i] as LineConnector).PxEndPointPosition = new Point(leftvalue + (i * refvalue), (view.SelectionList[i] as LineConnector).PxEndPointPosition.Y);
                                double offset = (view.SelectionList[i] as LineConnector).PxEndPointPosition.X - oldx;
                                (view.SelectionList[i] as LineConnector).PxStartPointPosition = new Point((view.SelectionList[i] as LineConnector).PxStartPointPosition.X + offset, (view.SelectionList[i] as LineConnector).PxStartPointPosition.Y);
                            }
                        }
                    }
                }

                if (view != null)
                {
                    view.m_IsCommandInProgress = false;
                    view.tUndoStack.Push("Start");
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
        }
        #endregion

        #region Alignment Commands

        /// <summary>
        /// Invoked when the AlignLeft Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignLeftCommand(object sender)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view != null)
            {
                view.m_IsCommandInProgress = true;
                view.tUndoStack.Push("Stop");
            }
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetX;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = (view.SelectionList[0] as LineConnector).GetBounds().Left;
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldx = (shape as Group).PxOffsetX;
                            (shape as Group).PxOffsetX = refvalue;
                            double translateoffset = (shape as Group).PxOffsetX - oldx;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetX += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + translateoffset, (child as LineConnector).PxEndPointPosition.Y);
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + translateoffset, (child as LineConnector).PxStartPointPosition.Y);
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X + translateoffset, (child as LineConnector).IntermediatePoints[i].Y);
                                        }
                                    }
                                    (child as LineConnector).UpdateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetX = refvalue;
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            double translateval = (shape as LineConnector).GetBounds().Left - refvalue;// MeasureUnitsConverter.ToPixels(refvalue, (view.Page as DiagramPage).MeasurementUnits);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X - translateval, (shape as LineConnector).PxStartPointPosition.Y);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X - translateval, (shape as LineConnector).PxEndPointPosition.Y);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X - translateval, (shape as LineConnector).IntermediatePoints[i].Y);
                                }
                            }
                            (shape as LineConnector).UpdateConnectorPathGeometry();
                        }
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
            if (view != null)
            {
                view.m_IsCommandInProgress = false;
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the AlignCenter Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignCenterCommand(object sender)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view != null)
            {
                view.m_IsCommandInProgress = true;
                view.tUndoStack.Push("Stop");
            }
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetX + ((view.SelectionList[0] as Node).Width / 2);
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = ((view.SelectionList[0] as LineConnector).GetBounds().Left + ((view.SelectionList[0] as LineConnector).GetBounds().Right)) / 2;
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldx = (shape as Group).PxOffsetX;
                            (shape as Group).PxOffsetX = refvalue - ((shape as Group).Width / 2);
                            double translateoffset = (shape as Group).PxOffsetX - oldx;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetX += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + translateoffset, (child as LineConnector).PxEndPointPosition.Y);
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + translateoffset, (child as LineConnector).PxStartPointPosition.Y);
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X + translateoffset, (child as LineConnector).IntermediatePoints[i].Y);
                                        }
                                    }
                                    (child as LineConnector).UpdateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetX = refvalue - ((shape as Node).Width / 2);
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            double translateval = ((shape as LineConnector).GetBounds().Left + (shape as LineConnector).GetBounds().Right) / 2 - refvalue;// MeasureUnitsConverter.ToPixels(refvalue, (view.Page as DiagramPage).MeasurementUnits);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X - translateval, (shape as LineConnector).PxStartPointPosition.Y);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X - translateval, (shape as LineConnector).PxEndPointPosition.Y);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X - translateval, (shape as LineConnector).IntermediatePoints[i].Y);
                                }
                            }
                            (shape as LineConnector).UpdateConnectorPathGeometry();
                        }
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
            if (view != null)
            {
                view.m_IsCommandInProgress = false;
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the AlignRight Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignRightCommand(object sender)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view != null)
            {
                view.m_IsCommandInProgress = true;
                view.tUndoStack.Push("Stop");
            }
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetX + (view.SelectionList[0] as Node).Width;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = (view.SelectionList[0] as LineConnector).GetBounds().Right;
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldx = (shape as Group).PxOffsetX;
                            (shape as Group).PxOffsetX = refvalue - (shape as Node).Width;
                            double translateoffset = (shape as Group).PxOffsetX - oldx;

                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetX += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X + translateoffset, (child as LineConnector).PxEndPointPosition.Y);
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X + translateoffset, (child as LineConnector).PxStartPointPosition.Y);
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X + translateoffset, (child as LineConnector).IntermediatePoints[i].Y);
                                        }
                                    }
                                    (child as LineConnector).UpdateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetX = refvalue - (shape as Node).Width;
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            double translateval = (shape as LineConnector).GetBounds().Right - refvalue;
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X - translateval, (shape as LineConnector).PxStartPointPosition.Y);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X - translateval, (shape as LineConnector).PxEndPointPosition.Y);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X - translateval, (shape as LineConnector).IntermediatePoints[i].Y);
                                }
                            }
                            (shape as LineConnector).UpdateConnectorPathGeometry();
                        }
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
            if (view != null)
            {
                view.m_IsCommandInProgress = false;
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the AlignTop Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignTopCommand(object sender)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view != null)
            {
                view.m_IsCommandInProgress = true;
                view.tUndoStack.Push("Stop");
            }
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetY;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = (view.SelectionList[0] as LineConnector).GetBounds().Top;
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldy = (shape as Group).PxOffsetY;
                            (shape as Group).PxOffsetY = refvalue;
                            double translateoffset = (shape as Group).PxOffsetY - oldy;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetY += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + translateoffset);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + translateoffset);
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X, (child as LineConnector).IntermediatePoints[i].Y + translateoffset);
                                        }
                                    }
                                    (child as LineConnector).UpdateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetY = refvalue;
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            double translateval = (shape as LineConnector).GetBounds().Top - refvalue;// MeasureUnitsConverter.ToPixels(refvalue, (view.Page as DiagramPage).MeasurementUnits);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y - translateval);//MeasureUnitsConverter.ToPixels(refvalue, (view.Page as DiagramPage).MeasurementUnits));
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y - translateval);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X, (shape as LineConnector).IntermediatePoints[i].Y - translateval);
                                }
                            }
                            (shape as LineConnector).UpdateConnectorPathGeometry();
                        }
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();

            if (view != null)
            {
                view.m_IsCommandInProgress = false;
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the AlignMiddle Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignMiddleCommand(object sender)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view != null)
            {
                view.m_IsCommandInProgress = true;
                view.tUndoStack.Push("Stop");
            }
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetY + ((view.SelectionList[0] as Node).Height / 2);
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = ((view.SelectionList[0] as LineConnector).GetBounds().Top + (view.SelectionList[0] as LineConnector).GetBounds().Bottom) / 2;

                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldy = (shape as Group).PxOffsetY;
                            (shape as Group).PxOffsetY = refvalue - ((shape as Node).Height / 2);
                            double translateoffset = (shape as Group).PxOffsetY - oldy;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetY += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + translateoffset);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + translateoffset);
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X, (child as LineConnector).IntermediatePoints[i].Y + translateoffset);
                                        }
                                    }
                                    (child as LineConnector).UpdateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetY = refvalue - (((shape as Node).Height) / 2);
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            double translateval = ((shape as LineConnector).GetBounds().Top + (shape as LineConnector).GetBounds().Bottom) / 2 - refvalue;
                            
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y - translateval);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y - translateval);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X, (shape as LineConnector).IntermediatePoints[i].Y - translateval);
                                }
                            }
                            (shape as LineConnector).UpdateConnectorPathGeometry();
                        }
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();

            if (view != null)
            {
                view.m_IsCommandInProgress = false;
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the AlignBottom Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignBottomCommand(object sender)
        {
            bool donotexecute = false;
            double refvalue = 0;
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view != null)
            {
                view.m_IsCommandInProgress = true;
                view.tUndoStack.Push("Stop");
            }
            if (view.SelectionList.Count > 0)
            {
                var nodes = view.SelectionList.OfType<IShape>();
                view.NodeDragCount = nodes.Count() - 1;
                if (view.SelectionList[0] is Node)
                {
                    refvalue = (view.SelectionList[0] as Node).PxOffsetY + (view.SelectionList[0] as Node).Height;
                }
                else if (view.SelectionList[0] is LineConnector && (view.SelectionList[0] as LineConnector).HeadNode == null && (view.SelectionList[0] as LineConnector).TailNode == null)
                {
                    refvalue = (view.SelectionList[0] as LineConnector).GetBounds().Bottom;
                }
                else
                {
                    donotexecute = true;
                }

                if (!donotexecute)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        if (shape is Group)
                        {
                            double oldy = (shape as Group).PxOffsetY;
                            (shape as Group).PxOffsetY = refvalue - (shape as Node).Height;
                            double translateoffset = (shape as Group).PxOffsetY - oldy;
                            foreach (INodeGroup child in (shape as Group).NodeChildren)
                            {
                                if (child is Node)
                                {
                                    (child as Node).PxOffsetY += translateoffset;
                                }
                                else if (child is LineConnector && (child as LineConnector).HeadNode == null && (child as LineConnector).TailNode == null)
                                {
                                    (child as LineConnector).PxStartPointPosition = new Point((child as LineConnector).PxStartPointPosition.X, (child as LineConnector).PxStartPointPosition.Y + translateoffset);
                                    (child as LineConnector).PxEndPointPosition = new Point((child as LineConnector).PxEndPointPosition.X, (child as LineConnector).PxEndPointPosition.Y + translateoffset);
                                    if ((child as LineConnector).IntermediatePoints != null)
                                    {
                                        for (int i = 0; i < (child as LineConnector).IntermediatePoints.Count; i++)
                                        {
                                            (child as LineConnector).IntermediatePoints[i] = new Point((child as LineConnector).IntermediatePoints[i].X, (child as LineConnector).IntermediatePoints[i].Y + translateoffset);
                                        }
                                    }
                                    (child as LineConnector).UpdateConnectorPathGeometry();
                                }
                            }
                        }
                        else if (shape is Node)
                        {
                            (shape as Node).PxOffsetY = refvalue - (shape as Node).Height;
                        }
                        else if (shape is LineConnector && (shape as LineConnector).HeadNode == null && (shape as LineConnector).TailNode == null)
                        {
                            double translateval = (shape as LineConnector).GetBounds().Bottom - refvalue;
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y - translateval);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y - translateval);
                            if ((shape as LineConnector).IntermediatePoints != null)
                            {
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X, (shape as LineConnector).IntermediatePoints[i].Y - translateval);
                                }
                            }
                            (shape as LineConnector).UpdateConnectorPathGeometry();
                        }
                    }
                }
            }

            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
            if (view != null)
            {
                view.m_IsCommandInProgress = false;
                view.tUndoStack.Push("Start");
            }
        }

        #endregion

        #region Order commands

        /// <summary>
        /// Invoked when the BringToFront Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>      
        public void OnBringToFrontCommand(object sender)
        {
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view.SelectionList.Count > 0)
            {

                view.tUndoStack.Push("Stop");
                List<UIElement> ordered = (from UIElement item in view.Page.Children.OfType<ICommon>()
                                           orderby Canvas.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();

                if (view.SelectionList.Count == 1)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        int oldindex = Canvas.GetZIndex(shape as UIElement);
                        if (shape is Node)
                        {
                            view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, shape as Node));
                        }
                        else if (shape is LineConnector)
                        {
                            view.tUndoStack.Push(new LineOperation(LineOperations.Order, shape as LineConnector));
                        }
                        Canvas.SetZIndex(shape as UIElement, view.Page.Children.Count);
                        int newindex = Canvas.GetZIndex(shape as UIElement);

                        foreach (UIElement element in view.Page.Children)
                        {
                            if (!view.SelectionList.Contains(element) && Canvas.GetZIndex(element) > oldindex)
                            {
                                if (shape is Node)
                                {
                                    view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, shape as Node));
                                }
                                else
                                {
                                    view.tUndoStack.Push(new LineOperation(LineOperations.Order, shape as LineConnector));
                                }
                                Canvas.SetZIndex(element, Canvas.GetZIndex(element) - 1);
                            }
                        }
                    }
                }
                else
                {
                    int childcount = view.Page.Children.OfType<ICommon>().Count<ICommon>();
                    int initialindexvalue = childcount - view.SelectionList.Count;
                    List<UIElement> selectionordered = (from UIElement item in view.SelectionList
                                                        orderby Canvas.GetZIndex(item as UIElement)
                                                        select item as UIElement).ToList();
                    for (int i = 0; i < view.SelectionList.Count; i++)
                    {
                        (selectionordered[i] as ICommon).OldZIndex = Canvas.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                        if (selectionordered[i] is Node)
                        {
                            view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, selectionordered[i] as Node));
                        }
                        else if (selectionordered[i] is LineConnector)
                        {
                            view.tUndoStack.Push(new LineOperation(LineOperations.Order, selectionordered[i] as LineConnector));
                        }
                        Canvas.SetZIndex(selectionordered[i], initialindexvalue++);
                        (selectionordered[i] as ICommon).NewZIndex = Canvas.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                    }

                    for (int i = 0; i < view.SelectionList.Count - 1; i++)
                    {
                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!selectionordered.Contains(element))
                            {
                                int g = Canvas.GetZIndex(element);
                                if (Canvas.GetZIndex(element) > (selectionordered[i] as ICommon).OldZIndex && Canvas.GetZIndex(element) < (selectionordered[i + 1] as ICommon).OldZIndex)
                                {
                                    if (element is Node)
                                    {
                                        view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                                    }
                                    else if (element is LineConnector)
                                    {
                                        view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                                    }
                                    Canvas.SetZIndex(element, Canvas.GetZIndex(element) - (i + 1));
                                }

                                int h = Canvas.GetZIndex(element);
                            }
                        }
                    }

                    foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                    {
                        if (!selectionordered.Contains(element))
                        {
                            if (Canvas.GetZIndex(element) > (selectionordered[view.SelectionList.Count - 1] as ICommon).OldZIndex)
                            {
                                if (element is Node)
                                {
                                    view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                                }
                                else if (element is LineConnector)
                                {
                                    view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                                }
                                Canvas.SetZIndex(element, Canvas.GetZIndex(element) - view.SelectionList.Count);
                            }
                        }
                    }
                }
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the SendToBack Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>      
        public void OnSendToBackCommand(object sender)
        {
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view.SelectionList.Count > 0)
            {

                view.tUndoStack.Push("Stop");
                List<UIElement> ordered = (from UIElement item in view.Page.Children.OfType<ICommon>()
                                           orderby Canvas.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();

                if (view.SelectionList.Count == 1)
                {
                    foreach (ICommon shape in view.SelectionList)
                    {
                        int oldindex = Canvas.GetZIndex(shape as UIElement);
                        if (shape is Node)
                        {
                            view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, shape as Node));
                        }
                        else if (shape is LineConnector)
                        {
                            view.tUndoStack.Push(new LineOperation(LineOperations.Order, shape as LineConnector));
                        }
                        Canvas.SetZIndex(shape as UIElement, 1);
                        int newindex = Canvas.GetZIndex(shape as UIElement);

                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (element is Node)
                            {
                                view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                            }
                            else if (element is LineConnector)
                            {
                                view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                            }
                            if (!view.SelectionList.Contains(element) && Canvas.GetZIndex(element) < oldindex)
                            {
                                Canvas.SetZIndex(element, Canvas.GetZIndex(element) + 1);
                            }
                        }
                    }
                }
                else
                {
                    int childcount = view.Page.Children.OfType<ICommon>().Count<ICommon>();
                    int initialindexvalue = 0;
                    List<UIElement> selectionordered = (from UIElement item in view.SelectionList
                                                        orderby Canvas.GetZIndex(item as UIElement)
                                                        select item as UIElement).ToList();
                    for (int i = 0; i < view.SelectionList.Count; i++)
                    {
                        (selectionordered[i] as ICommon).OldZIndex = Canvas.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                        if (selectionordered[i] is Node)
                        {
                            view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, selectionordered[i] as Node));
                        }
                        else if (selectionordered[i] is LineConnector)
                        {
                            view.tUndoStack.Push(new LineOperation(LineOperations.Order, selectionordered[i] as LineConnector));
                        }
                        Canvas.SetZIndex(selectionordered[i], initialindexvalue++);
                        (selectionordered[i] as ICommon).NewZIndex = Canvas.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                    }

                    for (int i = 0; i < view.SelectionList.Count - 1; i++)
                    {
                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!selectionordered.Contains(element))
                            {
                                int g = Canvas.GetZIndex(element);
                                if (Canvas.GetZIndex(element) < (selectionordered[0] as ICommon).OldZIndex)
                                {
                                    if (element is Node)
                                    {
                                        view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                                    }
                                    else if (element is LineConnector)
                                    {
                                        view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                                    }
                                    Canvas.SetZIndex(element, Canvas.GetZIndex(element) + view.SelectionList.Count);
                                }
                                else
                                    if (Canvas.GetZIndex(element) > (selectionordered[i] as ICommon).OldZIndex && Canvas.GetZIndex(element) < (selectionordered[i + 1] as ICommon).OldZIndex)
                                    {
                                        if (element is Node)
                                        {
                                            view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                                        }
                                        else if (element is LineConnector)
                                        {
                                            view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                                        }
                                        Canvas.SetZIndex(element, Canvas.GetZIndex(element) + (view.SelectionList.Count - 1));
                                    }

                                int h = Canvas.GetZIndex(element);
                            }
                        }
                    }
                }
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the MoveForward Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>      
        public void OnMoveForwardCommand(object sender)
        {
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view.SelectionList.Count > 0)
            {

                List<UIElement> ordered = (from UIElement item in view.Page.Children.OfType<ICommon>()
                                           orderby Canvas.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();

                if (view.SelectionList.Count == 1)
                {
                    view.tUndoStack.Push("Stop");
                    foreach (ICommon shape in view.SelectionList)
                    {
                        int oldindex = Canvas.GetZIndex(shape as UIElement);
                        if (oldindex < view.Page.Children.Count)
                        {
                            if (shape is Node)
                            {
                                view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, shape as Node));
                            }
                            else if (shape is LineConnector)
                            {
                                view.tUndoStack.Push(new LineOperation(LineOperations.Order, shape as LineConnector));
                            }
                            Canvas.SetZIndex(shape as UIElement, oldindex + 1);
                        }

                        //int newindex = Canvas.GetZIndex(shape as UIElement);

                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!view.SelectionList.Contains(element) && Canvas.GetZIndex(element) == oldindex + 1)
                            {
                                if (element is Node)
                                {
                                    view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                                }
                                else if (element is LineConnector)
                                {
                                    view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                                }
                                Canvas.SetZIndex(element, Canvas.GetZIndex(element) - 1);
                            }
                        }
                    }
                }
                else
                {
                    int childcount = view.Page.Children.OfType<ICommon>().Count<ICommon>();
                    int initialindexvalue = childcount - view.SelectionList.Count;
                    List<UIElement> selectionordered = (from UIElement item in view.SelectionList
                                                        orderby Canvas.GetZIndex(item as UIElement)
                                                        select item as UIElement).ToList();
                    for (int i = 0; i < view.SelectionList.Count; i++)
                    {
                        (selectionordered[i] as ICommon).OldZIndex = Canvas.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                        if ((selectionordered[i] as ICommon).OldZIndex != childcount - 1)
                        {
                            if (selectionordered[i] is Node)
                            {
                                view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, selectionordered[i] as Node));
                            }
                            else if (selectionordered[i] is LineConnector)
                            {
                                view.tUndoStack.Push(new LineOperation(LineOperations.Order, selectionordered[i] as LineConnector));
                            }
                            Canvas.SetZIndex(selectionordered[i], (selectionordered[i] as ICommon).OldZIndex + 1);
                        }
                        else
                        {
                            for (int h = view.SelectionList.Count - 1; h >= 0; h--)
                            {
                                foreach (ICommon icom in view.SelectionList)
                                {
                                    if (icom != (selectionordered[h] as ICommon) && (Canvas.GetZIndex(icom as UIElement) == Canvas.GetZIndex(selectionordered[h] as UIElement)))
                                    {
                                        if (icom is Node)
                                        {
                                            view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, icom as Node));
                                        }
                                        else if (icom is LineConnector)
                                        {
                                            view.tUndoStack.Push(new LineOperation(LineOperations.Order, icom as LineConnector));
                                        }
                                        Canvas.SetZIndex(icom as UIElement, icom.NewZIndex - 1);
                                    }
                                }
                            }
                        }

                        (selectionordered[i] as ICommon).NewZIndex = Canvas.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                    }

                    for (int i = 0; i < view.SelectionList.Count; i++)
                    {
                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!selectionordered.Contains(element))
                            {
                                if (Canvas.GetZIndex(element) == (selectionordered[i] as ICommon).NewZIndex)
                                {
                                    if (element is Node)
                                    {
                                        view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                                    }
                                    else if (element is LineConnector)
                                    {
                                        view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                                    }
                                    Canvas.SetZIndex(element, Canvas.GetZIndex(element) - 1);
                                    this.CompareIndex(element, view, selectionordered);
                                }
                            }
                        }
                    }
                }
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Invoked when the SendBackward Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>      
        public void OnSendBackwardCommand(object sender)
        {
            DiagramView view;
            if (sender == null)
            {
                view = View as DiagramView;
            }
            else
            {
                view = sender as DiagramView;
            }
            if (view.SelectionList.Count > 0)
            {

                List<UIElement> ordered = (from UIElement item in view.Page.Children.OfType<ICommon>()
                                           orderby Canvas.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();

                if (view.SelectionList.Count == 1)
                {
                    view.tUndoStack.Push("Stop");
                    foreach (ICommon shape in view.SelectionList)
                    {
                        int oldindex = Canvas.GetZIndex(shape as UIElement);
                        if (oldindex > 1)
                        {
                            if (shape is Node)
                            {
                                view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, shape as Node));
                            }
                            else if (shape is LineConnector)
                            {
                                view.tUndoStack.Push(new LineOperation(LineOperations.Order, shape as LineConnector));
                            }
                            Canvas.SetZIndex(shape as UIElement, oldindex - 1);
                        }

                        //int newindex = Canvas.GetZIndex(shape as UIElement);

                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!view.SelectionList.Contains(element) && Canvas.GetZIndex(element) == oldindex - 1)
                            {
                                if (element is Node)
                                {
                                    view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                                }
                                else if (element is LineConnector)
                                {
                                    view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                                }
                                Canvas.SetZIndex(element, Canvas.GetZIndex(element) + 1);
                            }
                        }
                    }
                }
                else
                {
                    int childcount = view.Page.Children.OfType<ICommon>().Count<ICommon>();
                    int initialindexvalue = childcount - view.SelectionList.Count;
                    List<UIElement> selectionordered = (from UIElement item in view.SelectionList
                                                        orderby Canvas.GetZIndex(item as UIElement)
                                                        select item as UIElement).ToList();
                    for (int i = view.SelectionList.Count - 1; i >= 0; i--)
                    {
                        (selectionordered[i] as ICommon).OldZIndex = Canvas.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                        if ((selectionordered[i] as ICommon).OldZIndex != 0)
                        {
                            if (selectionordered[i] is Node)
                            {
                                view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, selectionordered[i] as Node));
                            }
                            else if (selectionordered[i] is LineConnector)
                            {
                                view.tUndoStack.Push(new LineOperation(LineOperations.Order, selectionordered[i] as LineConnector));
                            }
                            Canvas.SetZIndex(selectionordered[i], (selectionordered[i] as ICommon).OldZIndex - 1);
                        }
                        else
                        {
                            for (int h = 0; h <= view.SelectionList.Count - 1; h++)
                            {
                                foreach (ICommon icom in view.SelectionList)
                                {
                                    if (icom != (selectionordered[h] as ICommon) && (Canvas.GetZIndex(icom as UIElement) == Canvas.GetZIndex(selectionordered[h] as UIElement)))
                                    {
                                        if (icom is Node)
                                        {
                                            view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, icom as Node));
                                        }
                                        else if (icom is LineConnector)
                                        {
                                            view.tUndoStack.Push(new LineOperation(LineOperations.Order, icom as LineConnector));
                                        }
                                        Canvas.SetZIndex(icom as UIElement, icom.NewZIndex + 1);
                                    }
                                }
                            }
                        }

                        (selectionordered[i] as ICommon).NewZIndex = Canvas.GetZIndex((selectionordered[i] as ICommon) as UIElement);
                    }

                    for (int i = 0; i < view.SelectionList.Count - 1; i++)
                    {
                        foreach (UIElement element in view.Page.Children.OfType<ICommon>())
                        {
                            if (!selectionordered.Contains(element))
                            {
                                if (Canvas.GetZIndex(element) == (selectionordered[i] as ICommon).NewZIndex)
                                {
                                    if (element is Node)
                                    {
                                        view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                                    }
                                    else if (element is LineConnector)
                                    {
                                        view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                                    }
                                    Canvas.SetZIndex(element, Canvas.GetZIndex(element) + 1);
                                    this.CompareBackwardIndex(element, view, selectionordered);
                                }
                            }
                        }
                    }
                }
                view.tUndoStack.Push("Start");
            }
        }

        /// <summary>
        /// Compares the index.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="view">The view instance.</param>
        /// <param name="selectionordered">The ordered list.</param>
        private void CompareIndex(UIElement element, DiagramView view, List<UIElement> selectionordered)
        {
            for (int i = 0; i < view.SelectionList.Count - 1; i++)
            {
                if (Canvas.GetZIndex(element) == (selectionordered[i] as ICommon).NewZIndex)
                {
                    if (element is Node)
                    {
                        view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                    }
                    else if (element is LineConnector)
                    {
                        view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                    }
                    Canvas.SetZIndex(element, Canvas.GetZIndex(element) - 1);
                    this.CompareBackwardIndex(element, view, selectionordered);
                }
            }
        }

        /// <summary>
        /// Compares the index of the backward.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="view">The view instance.</param>
        /// <param name="selectionordered">The ordered list.</param>
        private void CompareBackwardIndex(UIElement element, DiagramView view, List<UIElement> selectionordered)
        {
            for (int i = 0; i < view.SelectionList.Count; i++)
            {
                if (Canvas.GetZIndex(element) == (selectionordered[i] as ICommon).NewZIndex)
                {
                    if (element is Node)
                    {
                        view.tUndoStack.Push(new NodeOperation(NodeOperations.Order, element as Node));
                    }
                    else if (element is LineConnector)
                    {
                        view.tUndoStack.Push(new LineOperation(LineOperations.Order, element as LineConnector));
                    }
                    Canvas.SetZIndex(element, Canvas.GetZIndex(element) + 1);
                    this.CompareBackwardIndex(element, view, selectionordered);
                }
            }
        }

        #endregion

    }
}
