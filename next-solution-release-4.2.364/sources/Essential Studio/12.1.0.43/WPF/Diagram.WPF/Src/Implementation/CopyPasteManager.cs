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

namespace Syncfusion.Windows.Diagram
{
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif

    internal class CopyPasteManager : DispatcherObject
    {
        internal static Guid syncfusionClipboardId;

        internal List<INodeGroup> tempNL;

        internal DiagramView diagramView;

        internal CollectionExt pastedCollection;

        internal bool invalidateSelection = true;

        internal int pasteCount = 0;

        internal static bool resetPasteCount = false;

        internal static bool IsValidClipboardContent
        {
            get
            {
                try
                {
                    string text = Clipboard.GetText();
                    if (text == null || text.Equals(string.Empty))
                    {
                        return false;
                    }
                    else if (text.Contains(syncfusionClipboardId.ToString()))
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

        internal static bool IsValidPasteContent
        {
            get
            {
                string text = Clipboard.GetText();
                if (text.Contains(pasteContent.id.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [DataContract]
        internal class CopyContent
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
            public List<string> lines = new List<string>();
            [DataMember]
            public List<List<int>> Groups = new List<List<int>>();
        }

        internal class PasteContent
        {
            public Guid id;
            public Guid syncfusionClipboardId = CopyPasteManager.syncfusionClipboardId;
            public int minRefNo;
            public List<Node> Nodes = new List<Node>();
            public List<LineConnector> lines = new List<LineConnector>();
            public List<List<int>> Groups = new List<List<int>>();
        }

        internal static PasteContent pasteContent = new PasteContent();

        DispatcherOperation disp;
        internal delegate void dispDelegate();

        public CopyPasteManager(DiagramView view)
        {
            diagramView = view;
            diagramView.SelectionList.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectionList_CollectionChanged);
        }

        void SelectionList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            invalidateSelection = true;
        }

        internal void copy()
        {
            {
                CopyContent content = new CopyContent();
                content.minRefNo = getMinRefNo();
                content.id = Guid.NewGuid();
                tempNL = new List<INodeGroup>();
                foreach (UIElement ui in diagramView.SelectionList)
                {
                    if (ui is Layer)
                    {

                    }
                    else if (ui is Group)
                    {
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
                                        tempNL.Add(gui);
                                        content.Nodes.Add(getXamlWriterText(gui as UIElement));
                                    }
                                    else if (gui is LineConnector)
                                    {
                                        tempNL.Add(gui);
                                        content.lines.Add(getXamlWriterText(gui as UIElement));
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
                            tempNL.Add(ui as Node);
                            content.Nodes.Add(getXamlWriterText(ui));
                        }
                    }
                    else if (ui is LineConnector)
                    {
                        if (!tempNL.Contains(ui as LineConnector))
                        {
                            tempNL.Add(ui as LineConnector);
                            (ui as LineConnector).SetSerializationData();
                            content.lines.Add(getXamlWriterText(ui));
                        }
                    }
                }

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(CopyContent));
                serializer.WriteObject(ms, content);
                ms.Position = 0;
                System.IO.StreamReader sr = new System.IO.StreamReader(ms);
                Clipboard.SetText(sr.ReadToEnd());

                content = null;
                invalidateSelection = false;
                if (disp != null && (disp.Status == DispatcherOperationStatus.Executing || disp.Status == DispatcherOperationStatus.Pending))
                {
                    disp.Abort();
                }

                CopyPasteManager.resetPasteCount = true;

                disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                                        new dispDelegate(CopyPasteManager.startDispatch));
            }
        }

        private static void startDispatch()
        {
            CopyContent content = new CopyContent();
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
                preparePasteContent(content, CopyPasteManager.pasteContent, false);
            }

        }

        private bool isNodeinSelectedGroup(Node node)
        {
            foreach (UIElement ui in diagramView.SelectionList)
            {
                if (ui is Group && !(ui is Layer))
                {
                    if ((ui as Group).GroupChildrenRef.Contains(node.ReferenceNo))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal void cut()
        {
            copy();
            ApplicationCommands.Delete.Execute(diagramView.Page, diagramView);
            diagramView.Focus();
        }

        internal void paste()
        {
            if (IsValidClipboardContent)
            {
                if (!IsValidPasteContent)
                {
                    disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                                        new dispDelegate(CopyPasteManager.startDispatch));
                }
                DateTime dt = DateTime.Now;

                DiagramControl dc;
                dc = DiagramPage.GetDiagramControl(diagramView) as DiagramControl;
                int max = getMaxRefNo();
                if (disp == null)
                {
                    disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                                           new dispDelegate(CopyPasteManager.startDispatch));
                }

                if (disp != null)
                {
                    if ((disp.Status == DispatcherOperationStatus.Executing || disp.Status == DispatcherOperationStatus.Pending))
                    {
                        disp.Priority = DispatcherPriority.Send;
                        disp.Wait();
                        disp.Priority = DispatcherPriority.SystemIdle;
                    }
                }

                if (!CopyPasteManager.resetPasteCount)
                {
                    pasteCount++;
                }
                else
                {
                    pasteCount = 0;
                    CopyPasteManager.resetPasteCount = false;
                }

                int AddCount = 0;
                pastedCollection = new CollectionExt();
                pasteFromCashContent(CopyPasteManager.pasteContent, false, max, CopyPasteManager.pasteContent.minRefNo, ref AddCount, pastedCollection);
                var x = (from LineConnector l in dc.Model.Connections
                         from Node n in dc.Model.Nodes
                         where ((n.ReferenceNo == l.TailNodeReferenceNo) || (n.ReferenceNo == l.HeadNodeReferenceNo))
                         where l.ReferenceNo > max
                         select new { l, n });

                foreach (var items in x)
                {
                    if (items.l.HeadNodeReferenceNo == items.n.ReferenceNo )
                    {
                        items.l.HeadNode = items.n;
                        if (diagramView.UndoStack.Count > 3)
                        {
                            diagramView.UndoStack.Pop();
                            diagramView.UndoStack.Pop();
                            diagramView.UndoStack.Pop();
                            diagramView.UndoStack.Pop();
                        }
                    }
                    if (items.l.TailNodeReferenceNo == items.n.ReferenceNo)
                    {
                        items.l.TailNode = items.n;
                        if(diagramView.UndoStack.Count > 3)
                        {
                        diagramView.UndoStack.Pop();
                        diagramView.UndoStack.Pop();
                        diagramView.UndoStack.Pop();
                        diagramView.UndoStack.Pop();
                        }
                    }
                }
                diagramView.UndoStack.Push(AddCount);
                diagramView.UndoStack.Push("Added");
                if (disp != null)
                {
                    disp.Abort();
                }
                disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                                        new dispDelegate(CopyPasteManager.startDispatch));
                diagramView.SelectionList.Clear();
                foreach (ICommon com in pastedCollection)
                {
                    if (com != null)
                    {
                        diagramView.SelectionList.Add(com);
                    }
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

        private static void preparePasteContent(CopyContent content, PasteContent pasteContent, bool isGroup)
        {
            List<Node> nodes = new List<Node>();
            List<LineConnector> lines = new List<LineConnector>();
            int i = 0;
            ConnectionPort p = null;
            Group g = new Group();
            string str1 = string.Empty;

            foreach (string str in content.Nodes)
            {
                str1 = str;
#if  SyncfusionFramework3_5

                str1 = str1.Replace("http://schemas.syncfusion.com/wpf", "clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Wpf");

#endif
                System.IO.StringReader sr1 = new System.IO.StringReader(str1);              
                System.Xml.XmlReader xr1 = System.Xml.XmlReader.Create(sr1);
                object o = System.Windows.Markup.XamlReader.Load(xr1);
                if (o is Node)
                {
                    Node n = o as Node;
                    foreach (ConnectionPort port in n.Ports)
                    {
                        if (port.CenterPortReferenceNo == 0)
                        {
                            port.Name = "PART_Sync_CenterPort";
                            i++;
                            if (i > 1)
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

            foreach (string str in content.lines)
            {
                System.IO.StringReader sr1 = new System.IO.StringReader(str);
                System.Xml.XmlReader xr1 = System.Xml.XmlReader.Create(sr1);
                object o = System.Windows.Markup.XamlReader.Load(xr1);
                bool tailref = false;
                bool headref = false;
                if (o is LineConnector)
                {
                    LineConnector ln = o as LineConnector;
                    foreach (Node node in pasteContent.Nodes)
                    {
                        if (ln.HeadNodeReferenceNo == node.ReferenceNo)
                        {
                            foreach (ConnectionPort cp in node.Ports)
                            {
                                if (ln.HeadPortReferenceNo == cp.PortReferenceNo)
                                {
                                    ln.ConnectionHeadPort = cp;
                                }
                            }
                        }
                        if (ln.TailNodeReferenceNo == node.ReferenceNo)
                        {
                            foreach (ConnectionPort cp in node.Ports)
                            {
                                if (ln.TailPortReferenceNo == cp.PortReferenceNo)
                                {
                                    ln.ConnectionTailPort = cp;
                                }
                            }
                        }
                        if (!headref)
                        {
                            if (node.ReferenceNo == ln.HeadNodeReferenceNo)
                            {
                                headref = true;
                            }
                            else
                            {
                                headref = false;
                            }
                        }
                        if (!tailref)
                        {
                            if (node.ReferenceNo == ln.TailNodeReferenceNo)
                            {
                                tailref = true;
                            }
                            else
                            {
                                tailref = false;
                            }
                        }
                    }
                    if (!tailref)
                    {
                        ln.TailNodeReferenceNo = -1;
                    }
                    if (!headref)
                    {
                        ln.HeadNodeReferenceNo = -1;
                    }
                    pasteContent.lines.Add(ln);
                }
            }
        }

        private string getXamlWriterText(UIElement ui)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                if (ui != null && ms != null)
                {
                    ms.Flush();
                    System.Windows.Markup.XamlWriter.Save(ui, ms);
                    ms.Position = 0;
                    System.IO.StreamReader sr = new System.IO.StreamReader(ms);
                    return sr.ReadToEnd();
                }
                else
                {
                    return null;
                }
            }
        }

        private void pasteFromCashContent(PasteContent content, bool isGroup, int max, int min, ref int AddCount, CollectionExt pastedCollection)
        {
            int incriment = max + 1 - min;

            DiagramControl dc;
            dc = DiagramPage.GetDiagramControl(diagramView) as DiagramControl;
            List<Node> nodes = new List<Node>();
            List<LineConnector> lines = new List<LineConnector>();

            foreach (Node o in content.Nodes)
            {
                if (o is Node)
                {
                    Node n = o as Node;
                    n.MeasurementUnits = (diagramView.Page as DiagramPage).MeasurementUnits;
                    n.Name = "Node" + Guid.NewGuid().ToString("N");
                    nodes.Add(n);
                    n.ReferenceNo += incriment;
                    n.IsGrouped = false;
                    n.Groups.Clear();
                    CollectionExt.Cleared = false;
                    n.PxWidth = n.Width;
                    n.PxHeight = n.Height;
                    n.PxOffsetX += (25 * (pasteCount + 1));
                    n.PxOffsetY += (25 * (pasteCount + 1));
                    dc.Model.Nodes.Add(n);
                    pastedCollection.Add(n);
                    AddCount++;
                }
            }

            foreach (LineConnector o in content.lines)
            {
                if (o is LineConnector)
                {
                    LineConnector ln = o as LineConnector;
                    ln.MeasurementUnit = (diagramView.Page as DiagramPage).MeasurementUnits;
                    ln.Name = "Line" + Guid.NewGuid().ToString("N");
                    ln.IsGrouped = false;
                    ln.Groups.Clear();
                    CollectionExt.Cleared = false;
                    ln.RetrieveSerializationData();
                    //if (ln.ConnectorType == ConnectorType.Straight || ln.ConnectorType == ConnectorType.Orthogonal)
                    //{
                    //    DiagramControl.LoadIntermediatePoints(ln);
                    //}
                    if (ln.IntermediatePoints != null)
                    {
                        for (int i = 0; i < ln.IntermediatePoints.Count; i++)
                        {
                            ln.IntermediatePoints[i] = new Point(ln.IntermediatePoints[i].X + (25 * (pasteCount + 1)), ln.IntermediatePoints[i].Y + (25 * (pasteCount + 1)));
                        }
                    }
                    ln.PxStartPointPosition = new Point(ln.PxStartPointPosition.X + (25 * (pasteCount + 1)), ln.PxStartPointPosition.Y + (25 * (pasteCount + 1)));
                    ln.PxEndPointPosition = new Point(ln.PxEndPointPosition.X + (25 * (pasteCount + 1)), ln.PxEndPointPosition.Y + (25 * (pasteCount + 1)));
                    ln.ReferenceNo += incriment;
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
                    AddCount++;
                }
            }

            foreach (List<int> ints in content.Groups)
            {
                for (int i = 0; i < ints.Count; i++)
                {
                    ints[i] += incriment;
                }
                AddCount++;
            }

            diagramView.LayoutUpdated += new EventHandler(diagramView_LayoutUpdated);
        }

        void diagramView_LayoutUpdated(object sender, EventArgs e)
        {
            DiagramControl dc;
            dc = DiagramPage.GetDiagramControl(diagramView) as DiagramControl;

            diagramView.UndoStack.Pop();
            int cnt = (int)diagramView.UndoStack.Pop();


            diagramView.LayoutUpdated -= new EventHandler(diagramView_LayoutUpdated);
            foreach (List<int> ints in CopyPasteManager.pasteContent.Groups)
            {
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
                dc.Model.Nodes.Add(g);
            }

            diagramView.UndoStack.Push(cnt);
            diagramView.UndoStack.Push("Added");
        }
    }

    [DataContract]
    internal class ConnectorStatePersistence
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