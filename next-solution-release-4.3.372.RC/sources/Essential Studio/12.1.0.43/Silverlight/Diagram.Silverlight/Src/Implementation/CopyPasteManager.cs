// <copyright file="CopyPasteManager.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.Serialization;
using System.Windows.Threading;
using System.Windows.Markup;

namespace Syncfusion.Windows.Diagram
{
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif

    public class CopyPasteManager
    {
        internal static Guid syncfusionClipboardId;

        internal List<INodeGroup> tempNL;

        internal DiagramView diagramView;

        internal CollectionExt pastedCollection;

        internal static int pasteCount = 0;

        internal static bool resetPasteCount = false;

        //internal static string copyString;

        internal Point PastePosition
        { get; set; }

        internal bool IsPastePosition = false;

        internal static bool IsValidStringContent
        {
            get
            {
                try
                {
                    string text = Clipboard.GetText();
                    if (text == null || text.Equals(string.Empty))
                    //if (copyString == null || copyString.Equals(string.Empty))
                    {
                        return false;
                    }
                    else if (text.Contains(syncfusionClipboardId.ToString()))
                    //else if (copyString.Contains(syncfusionClipboardId.ToString()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        internal static bool IsValidPasteContent
        {
            get
            {
                try
                {
                    string text = Clipboard.GetText();
                    if (text.Contains(pasteContent.id.ToString()))
                    //if (copyString.Contains(pasteContent.id.ToString()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        static CopyPasteManager()
        {
            CopyPasteManager.syncfusionClipboardId = new Guid("ada60200-78fb-4b16-9c1c-ec561793e5a2");
        }

        [DataContract]
        public class CopyContent
        {
            [DataMember]
            public Guid id;
            [DataMember]
            public Guid syncfusionClipboardId = CopyPasteManager.syncfusionClipboardId;
            [DataMember]
            public int minRefNo;
            [DataMember]
            public List<string> Nodes = new List<string>();
            [DataMember]
            public List<string> Lines = new List<string>();
            [DataMember]
            public List<List<int>> Groups = new List<List<int>>();
        }

        internal class PasteContent
        {
            public Guid id;
            public Guid syncfusionClipboardId = CopyPasteManager.syncfusionClipboardId;
            public int minRefNo;
            public List<Node> Nodes = new List<Node>();
            public List<LineConnector> Lines = new List<LineConnector>();
            public List<List<int>> Groups = new List<List<int>>();
        }

        internal static PasteContent pasteContent = new PasteContent();

        public CopyPasteManager(DiagramView view)
        {
            diagramView = view;
        }

        bool isCopy = true;
        internal void copy()
        {
            if (diagramView != null)
            {
                diagramView.m_IsCommandInProgress = true;
                diagramView.tUndoStack.Push("Stop");
            }

            DiagramPageXamlWriter xamlWriter = new DiagramPageXamlWriter();
            CopyContent content = new CopyContent();
            content.minRefNo = getMinRefNo();
            content.id = Guid.NewGuid();
            tempNL = new List<INodeGroup>();
            foreach (UIElement ui in diagramView.SelectionList)
            {
                if (ui is Group)
                {
                    (ui as Group).IsSelected = true;
                    (ui as Group).GroupChildrenRef.Clear();
                    CollectionExt.Cleared = false;
                    foreach (INodeGroup child in (ui as Group).NodeChildren)
                    {
                        (ui as Group).GroupChildrenRef.Add(child.ReferenceNo);
                    }
                    List<int> gContent = new List<int>();
                    foreach (INodeGroup gui in (ui as Group).NodeChildren)
                    {
                        if (!(gui is Group))
                        {
                            gContent.Add(gui.ReferenceNo);
                            if (!tempNL.Contains(gui))
                            {
                                if (gui is Node)
                                {
                                    (gui as Node).PathStyle.Fill = (gui as Node).NodePathFill;
                                    (gui as Node).PathStyle.PathObject = (gui as Node).NodeShape as System.Windows.Shapes.Path;
                                    tempNL.Add(gui);
                                    content.Nodes.Add(xamlWriter.WriteXaml(gui as UIElement));

                                    if ((gui as Node).OutEdges.Count > 0)
                                    {
                                        foreach (INodeGroup l in (gui as Node).OutEdges)
                                        {
                                            tempNL.Add(l);
                                            content.Lines.Add(xamlWriter.WriteXaml(l as UIElement));
                                        }
                                    }
                                }
                                else if (gui is LineConnector)
                                {
                                    tempNL.Add(gui);
                                    content.Lines.Add(xamlWriter.WriteXaml(gui as UIElement));
                                }
                            }
                        }
                    }
                    content.Groups.Add(gContent);
                }

                else if (ui is Node)
                {
                    if (!tempNL.Contains(ui as Node))
                    {
                        (ui as Node).PathStyle.Fill = (ui as Node).NodePathFill;
                        (ui as Node).PathStyle.PathObject = (ui as Node).NodeShape as System.Windows.Shapes.Path;
                        tempNL.Add(ui as Node);
                        content.Nodes.Add(xamlWriter.WriteXaml(ui));
                    }
                }
                else if (ui is LineConnector)
                {
                    if (!tempNL.Contains(ui as LineConnector))
                    {
                        tempNL.Add(ui as LineConnector);
                        (ui as LineConnector).SetSerializationData();
                        content.Lines.Add(xamlWriter.WriteXaml(ui));
                    }
                }
            }

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            DataContractSerializer serializer = new DataContractSerializer(typeof(CopyContent));
            serializer.WriteObject(ms, content);
            ms.Position = 0;
            System.IO.StreamReader sr = new System.IO.StreamReader(ms);
            //copyString = "";
            //copyString = (sr.ReadToEnd()).ToString();
            try
            {
                Clipboard.SetText(sr.ReadToEnd());
                isCopy = true;
            }
            catch { isCopy = false; }

            if (isCopy)
            {
                content = null;
                CopyPasteManager.resetPasteCount = true;
                CopyPasteManager.startDispatch();
            }

            if (diagramView != null)
            {
                diagramView.m_IsCommandInProgress = false;
                diagramView.tUndoStack.Push("Start");
            }
        }

        private static void startDispatch()
        {
            CopyContent content = new CopyContent();
            //System.IO.StringReader sr = new System.IO.StringReader(copyString);
            System.IO.StringReader sr = new System.IO.StringReader(Clipboard.GetText());
            System.Xml.XmlReader xr = System.Xml.XmlReader.Create(sr);
            DataContractSerializer serializer = new DataContractSerializer(typeof(CopyContent));
            content = serializer.ReadObject(xr) as CopyContent;
            if (content != null)
            {
                CopyPasteManager.pasteContent = new PasteContent();
                CopyPasteManager.pasteContent.id = content.id;
                CopyPasteManager.pasteContent.minRefNo = content.minRefNo;
                CopyPasteManager.pasteContent.Groups = content.Groups;
                preparePasteContent(content, CopyPasteManager.pasteContent);
            }
        }

        internal void cut()
        {
            if (diagramView != null)
            {
                diagramView.m_IsCommandInProgress = true;
                diagramView.tUndoStack.Push("Stop");
            }

            copy();

            if (isCopy)
            {
                DiagramControl dc;
                dc = DiagramPage.GetDiagramControl(diagramView) as DiagramControl;
                dc.Delete.Execute(dc.View);
            }

            if (diagramView != null)
            {
                diagramView.m_IsCommandInProgress = false;
                diagramView.tUndoStack.Push("Start");
            }
        }

        internal void paste()
        {
            if (IsValidStringContent)
            {
                if (diagramView != null)
                {
                    diagramView.m_IsCommandInProgress = true;
                    diagramView.tUndoStack.Push("Stop");
                }

                if (!IsValidPasteContent)
                {
                    CopyPasteManager.startDispatch();
                }

                DiagramControl dc;
                dc = DiagramPage.GetDiagramControl(diagramView) as DiagramControl;
                int max = getMaxRefNo();

                if (!CopyPasteManager.resetPasteCount)
                {
                    pasteCount++;
                }
                else
                {
                    CopyPasteManager.resetPasteCount = false;
                    pasteCount = 0;
                }

                pastedCollection = new CollectionExt();
                pasteFromCashContent(CopyPasteManager.pasteContent, max, CopyPasteManager.pasteContent.minRefNo, pastedCollection);
                CopyPasteManager.startDispatch();
                diagramView.SelectionList.Clear();

                foreach (ICommon com in pastedCollection)
                {
                    if (com != null)
                    {
                        diagramView.SelectionList.Add(com);
                    }
                }

                if (diagramView != null)
                {
                    diagramView.m_IsCommandInProgress = false;
                    diagramView.tUndoStack.Push("Start");
                }
            }
        }

        private int getMaxRefNo()
        {
            int max = 0;
            DiagramControl dc;
            dc = DiagramPage.GetDiagramControl(diagramView) as DiagramControl;

            foreach (Node n in dc.Model.Nodes)
            {
                max = Math.Max(n.ReferenceNo, max);
            }

            foreach (LineConnector l in dc.Model.Connections)
            {
                max = Math.Max(l.ReferenceNo, max);
            }
            return max;
        }

        private int getMinRefNo()
        {
            int min = int.MaxValue;
            foreach (UIElement ui in diagramView.SelectionList)
            {
                if (ui is Group)
                {
                    foreach (INodeGroup g in (ui as Group).NodeChildren)
                    {
                        min = Math.Min(g.ReferenceNo, min);
                    }
                }
                else if (ui is Node)
                {
                    min = Math.Min((ui as Node).ReferenceNo, min);
                }
                else if (ui is LineConnector)
                {
                    min = Math.Min((ui as LineConnector).ReferenceNo, min);
                }
            }
            return min;
        }

        private static void preparePasteContent(CopyContent content, PasteContent pasteContent)
        {
            int i = 0;
            ConnectionPort p = null;

            foreach (string str in content.Nodes)
            {
                object o = XamlReader.Load(str) as object;

                if (o is Node)
                {
                    Node n = o as Node;

                    foreach (ConnectionPort port in n.Ports)
                    {
                        if (port.CenterPortReferenceNo == 0)
                        {
                            port.Name = "PART_Sync_CenterPort";
                            i++;
                            if (i >= 1)
                            {
                                p = port;
                            }
                        }
                        port.Node = n;
                    }

                    if (p != null)
                    {
                        n.Ports.Remove(p);
                    }
                    pasteContent.Nodes.Add(n);
                }
            }

            foreach (string str in content.Lines)
            {
                object o = XamlReader.Load(str) as object;

                if (o is LineConnector)
                {
                    LineConnector ln = o as LineConnector;
                    foreach (Node node in pasteContent.Nodes)
                    {
                        if (ln.HeadNodeReferenceNo == node.ReferenceNo)
                        {
                            ln.HeadNode = node as Node;
                            foreach (ConnectionPort cp in node.Ports)
                            {
                                if (cp.Name != "PART_Sync_CenterPort")
                                {
                                    if (ln.HeadPortReferenceNo == cp.PortReferenceNo)
                                    {
                                        ln.ConnectionHeadPort = cp;
                                    }
                                }
                            }
                        }

                        if (ln.TailNodeReferenceNo == node.ReferenceNo)
                        {
                            ln.TailNode = node as Node;
                            foreach (ConnectionPort cp in node.Ports)
                            {
                                if (cp.Name != "PART_Sync_CenterPort")
                                {
                                    if (ln.TailPortReferenceNo == cp.PortReferenceNo)
                                    {
                                        ln.ConnectionTailPort = cp;
                                    }
                                }
                            }
                        }
                    }
                    pasteContent.Lines.Add(ln);
                    ln.UpdateLayout();
                }
            }
        }

        private void pasteFromCashContent(PasteContent content, int max, int min, CollectionExt pastedCollection)
        {
            int incriment = max + 1 - min;

            double leastOffsetX = 0;
            double leastOffsetY = 0;
            double diffOffsetX = 0;
            double diffOffsetY = 0;
            bool isFirstNode = true;

            double leastSPX = 0;
            double leastSPY = 0;
            double diffSPX = 0;
            double diffSPY = 0;
            double diffEPX = 0;
            double diffEPY = 0;
            bool isFirstLine = true;

            double leastX = 0;
            double leastY = 0;

            PastePosition = new Point(300, 300);
            Point PxPastePosition = new Point();
            PxPastePosition = MeasureUnitsConverter.ToPixels(PastePosition, (diagramView.Page as DiagramPage).MeasurementUnits);

            DiagramControl dc;
            dc = DiagramPage.GetDiagramControl(diagramView) as DiagramControl;

            foreach (Node o in content.Nodes)
            {
                if (o is Node)
                {
                    if (isFirstNode)
                    {
                        leastOffsetX = o.PxOffsetX;
                        leastOffsetY = o.PxOffsetY;
                        isFirstNode = false;
                    }
                    else
                    {
                        leastOffsetX = Math.Min(leastOffsetX, o.PxOffsetX);
                        leastOffsetY = Math.Min(leastOffsetY, o.PxOffsetY);
                    }
                }
            }

            foreach (LineConnector l in content.Lines)
            {
                if (l is LineConnector)
                {
                    if (isFirstLine)
                    {
                        leastSPX = l.PxStartPointPosition.X;
                        leastSPY = l.PxStartPointPosition.Y;
                        isFirstLine = false;
                    }
                    else
                    {
                        leastSPX = Math.Min(leastSPX, l.PxStartPointPosition.X);
                        leastSPY = Math.Min(leastSPY, l.PxStartPointPosition.Y);
                    }
                }
            }

            if (!isFirstNode && !isFirstLine)
            {
                leastX = Math.Min(leastOffsetX, leastSPX);
                leastY = Math.Min(leastOffsetY, leastSPY);
            }
            else if (!isFirstNode)
            {
                leastX = leastOffsetX;
                leastY = leastOffsetY;
            }
            else if (!isFirstLine)
            {
                leastX = leastSPX;
                leastY = leastSPY;
            }

            foreach (Node o in content.Nodes)
            {
                if (o is Node)
                {
                    Node n = o as Node;
                    n.MeasurementUnits = (diagramView.Page as DiagramPage).MeasurementUnits;
                    n.Name = "Node" + Guid.NewGuid().ToString("N");
                    n.ReferenceNo += incriment;
                    n.IsGrouped = false;
                    n.Groups.Clear();
                    CollectionExt.Cleared = false;
                    //n.PxWidth = n.Width;
                    //n.PxHeight = n.Height;

                    if (IsPastePosition)
                    {
                        diffOffsetX = n.PxOffsetX - leastX;
                        diffOffsetY = n.PxOffsetY - leastY;

                        //n.PxOffsetX = PxPastePosition.X + diffOffsetX;
                        //n.PxOffsetY = PxPastePosition.Y + diffOffsetY;

                        n.PxOffsetX = (PxPastePosition.X + diffOffsetX) + (25 * (pasteCount + 1));
                        n.PxOffsetY = (PxPastePosition.Y + diffOffsetY) + (25 * (pasteCount + 1));
                    }
                    else
                    {
                        n.PxOffsetX += (25 * (pasteCount + 1));
                        n.PxOffsetY += (25 * (pasteCount + 1));
                    }

                    if (n.PathStyle.PathObject != null)
                    {
                        n.NodeShape = n.PathStyle.PathObject as System.Windows.Shapes.Path;
                        if (n.Shape == Shapes.CustomPath)
                        {
                            Style CPS = new Style(typeof(System.Windows.Shapes.Path));
                            CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.StrokeProperty, (n.PathStyle.PathObject as System.Windows.Shapes.Path).Stroke));
                            CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.FillProperty, (n.PathStyle.PathObject as System.Windows.Shapes.Path).Fill));
                            CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.StrokeThicknessProperty, (n.PathStyle.PathObject as System.Windows.Shapes.Path).StrokeThickness));
                            CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.StretchProperty, (n.PathStyle.PathObject as System.Windows.Shapes.Path).Stretch));
                            n.CustomPathStyle = CPS;
                        }
                    }
                    dc.Model.Nodes.Add(n);
                    pastedCollection.Add(n);
                    n.UpdateLayout();
                }
            }

            foreach (LineConnector o in content.Lines)
            {
                if (o is LineConnector)
                {
                    LineConnector ln = o as LineConnector;
                    ln.MeasurementUnit = (diagramView.Page as DiagramPage).MeasurementUnits;
                    ln.Name = "Line" + Guid.NewGuid().ToString("N");
                    ln.ReferenceNo += incriment;
                    ln.IsGrouped = false;
                    ln.Groups.Clear();
                    CollectionExt.Cleared = false;

                    ln.RetrieveSerializationData();
                    if (ln.IntermediatePoints != null)
                    {
                        for (int i = 0; i < ln.IntermediatePoints.Count; i++)
                        {
                            ln.IntermediatePoints[i] = new Point(ln.IntermediatePoints[i].X + (25 * (pasteCount + 1)), ln.IntermediatePoints[i].Y + (25 * (pasteCount + 1)));
                        }
                    }

                    if (IsPastePosition)
                    {
                        diffSPX = ln.PxStartPointPosition.X - leastX;
                        diffSPY = ln.PxStartPointPosition.Y - leastY;
                        diffEPX = ln.PxEndPointPosition.X - leastX;
                        diffEPY = ln.PxEndPointPosition.Y - leastY;

                        //ln.PxStartPointPosition = new Point(PxPastePosition.X + diffSPX, PastePosition.Y + diffSPY);
                        //ln.PxEndPointPosition = new Point(PxPastePosition.X + diffEPX, PastePosition.Y + diffEPY);

                        ln.PxStartPointPosition = new Point((PxPastePosition.X + diffSPX) + (25 * (pasteCount + 1)), (PxPastePosition.Y + diffSPY) + (25 * (pasteCount + 1)));
                        ln.PxEndPointPosition = new Point((PxPastePosition.X + diffEPX) + (25 * (pasteCount + 1)), (PxPastePosition.Y + diffEPY) + (25 * (pasteCount + 1)));
                    }
                    else
                    {
                        ln.PxStartPointPosition = new Point(ln.PxStartPointPosition.X + (25 * (pasteCount + 1)), ln.PxStartPointPosition.Y + (25 * (pasteCount + 1)));
                        ln.PxEndPointPosition = new Point(ln.PxEndPointPosition.X + (25 * (pasteCount + 1)), ln.PxEndPointPosition.Y + (25 * (pasteCount + 1)));
                    }

                    if (ln.HeadNodeReferenceNo >= 0)
                    {
                        ln.HeadNodeReferenceNo += incriment;
                    }
                    if (ln.TailNodeReferenceNo >= 0)
                    {
                        ln.TailNodeReferenceNo += incriment;
                    }

                    dc.Model.Connections.Add(ln);
                    pastedCollection.Add(ln);
                    ln.UpdateLayout();
                }
            }

            foreach (List<int> ints in content.Groups)
            {
                for (int i = 0; i < ints.Count; i++)
                {
                    ints[i] += incriment;
                }

                Group g = new Group();
                g.Name = "Group" + Guid.NewGuid().ToString("N");
                var y = from FrameworkElement ui in dc.View.Page.Children
                        from int i in ints
                        where (ui is INodeGroup) && (ui as INodeGroup).ReferenceNo == i
                        select ui;
                foreach (INodeGroup ui in y)
                {
                    g.AddChild(ui);
                }
                g.IsSelected = true;
                dc.Model.Nodes.Add(g);
            }
        }
    }

    [DataContract]
    public class ConnectorStatePersistence
    {
        [DataMember]
        public List<Point> IntermediatePoints { get; set; }

        public void SetIntermediatePoints(List<Point> pts)
        {
            IntermediatePoints = new List<Point>();
            foreach (Point pt in pts)
            {
                IntermediatePoints.Add(pt);
            }
        }
    }
}
