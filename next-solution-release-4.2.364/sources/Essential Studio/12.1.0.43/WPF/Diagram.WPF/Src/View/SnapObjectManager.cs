#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Windows.Diagram
{
    #region SnapSettings Class
    public class SnapSettings : DependencyObject
    {

        #region Variables

        internal DiagramPage dia_page;
        private Point _oldStartPoint = new Point(0.0, 0.0);
        private Adorner _oldadorner;
        internal List<Point> _oldHorizontalDrawlines;
        internal List<Point> _oldVerticalLines;
        internal List<Point> _oldWidthLines;
        internal List<Point> _oldHeightLines;
        internal List<Point> _CommonLines;
        List<Adorner> HorizontalLineAdorner = new List<Adorner>();
        List<Adorner> VerticalLineAdorner = new List<Adorner>();
        List<Adorner> WidhtLineAdorner = new List<Adorner>();
        List<Adorner> HeightLineAdorner = new List<Adorner>();
        internal double AdjustDistance = 15;

        /// <summary>
        /// Gets or sets the DrawingLinePen property.
        /// <value>
        /// Type: <see cref="Pen"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty SnapLinePenProperty = DependencyProperty.Register("SnapLinePen", typeof(Pen), typeof(SnapSettings), new PropertyMetadata(null));

        public Pen SnapLinePen
        {
            get
            {
                return (Pen)GetValue(SnapLinePenProperty);
            }

            set
            {
                SetValue(SnapLinePenProperty, value);
            }
        }


        #endregion

        #region Constructor

        public SnapSettings()
        {
           
        }

        #endregion

        #region DrawingGuideLines

        /// <summary>
        /// Gets or sets the SnapAdjustmentDistance property.
        /// <value>
        /// Type: <see cref="Double"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty SnapAdjustmentDistanceProperty = DependencyProperty.Register("SnapAdjustmentDistance", typeof(double), typeof(SnapSettings), new PropertyMetadata(10d));

        public double SnapAdjustmentDistance
        {
            get
            {
                return (double)GetValue(SnapAdjustmentDistanceProperty);
            }

            set
            {
                SetValue(SnapAdjustmentDistanceProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the SnapPort property.
        /// <value>
        /// Type: <see cref="SnapPort"/>
        /// </value>
        /// </summary>

        internal static readonly DependencyProperty SnapPortProperty = DependencyProperty.Register("SnapPort", typeof(SnapPort), typeof(SnapSettings), new PropertyMetadata(null));

        internal SnapPort SnapPort
        {
            get
            {
                return (SnapPort)GetValue(SnapPortProperty);
            }

            set
            {
                SetValue(SnapPortProperty, value);
            }
        }

        private static void OnSnapPortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SnapSettings snapsetting = d as SnapSettings;
            if (snapsetting.EnableSnapPort)
            {
                snapsetting.SnapPort = new SnapPort(snapsetting.dia_page);
            }
            else
            {
                snapsetting.SnapPort = null;
            }
        }

        public NodeSnapMode NodeSnapMode
        {
            get { return (NodeSnapMode)GetValue(NodeSnapModeProperty); }
            set { SetValue(NodeSnapModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SnapNodeMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NodeSnapModeProperty =
            DependencyProperty.Register("SnapNodeMode", typeof(NodeSnapMode), typeof(SnapSettings), new PropertyMetadata(NodeSnapMode.Node));

        /// <summary>
        /// Gets or sets the EnableSnapNode property.
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty EnableSnapNodeProperty = DependencyProperty.Register("EnableSnapNode", typeof(bool), typeof(SnapSettings), new PropertyMetadata(false));

        public bool EnableSnapNode
        {
            get
            {
                return (bool)GetValue(EnableSnapNodeProperty);
            }

            set
            {
                SetValue(EnableSnapNodeProperty, value);
            }
        }



        /// <summary>
        /// Gets or sets the EnableSnapNode property.
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty EnableSnapPortProperty = DependencyProperty.Register("EnableSnapPort", typeof(bool), typeof(SnapSettings), new PropertyMetadata(false,(OnSnapPortChanged)));

        public bool EnableSnapPort
        {
            get
            {
                return (bool)GetValue(EnableSnapPortProperty);
            }

            set
            {
                SetValue(EnableSnapPortProperty, value);
            }
        }





        /// <summary>
        /// Gets or sets the CenterY property.
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(bool), typeof(SnapSettings), new PropertyMetadata(true));

        public bool CenterY
        {
            get
            {
                return (bool)GetValue(CenterYProperty);
            }

            set
            {
                SetValue(CenterYProperty, value);
            }
        }



        /// <summary>
        /// Gets or sets the CenterX property.
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(bool), typeof(SnapSettings), new PropertyMetadata(true));

        public bool CenterX
        {
            get
            {
                return (bool)GetValue(CenterXProperty);
            }

            set
            {
                SetValue(CenterXProperty, value);
            }
        }





        /// <summary>
        /// Gets or sets the Right property.
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty RightProperty = DependencyProperty.Register("Right", typeof(bool), typeof(SnapSettings), new PropertyMetadata(true));

        public bool Right
        {
            get
            {
                return (bool)GetValue(RightProperty);
            }

            set
            {
                SetValue(RightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Bottom property.
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty BottomProperty = DependencyProperty.Register("Bottom", typeof(bool), typeof(SnapSettings), new PropertyMetadata(true));

        public bool Bottom
        {
            get
            {
                return (bool)GetValue(BottomProperty);
            }

            set
            {
                SetValue(BottomProperty, value);
            }
        }




        /// <summary>
        /// Gets or sets the EnableVerticalDrawingLine property.
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(bool), typeof(SnapSettings), new PropertyMetadata(true));

        public bool Left
        {
            get
            {
                return (bool)GetValue(LeftProperty);
            }

            set
            {
                SetValue(LeftProperty, value);
            }
        }



        /// <summary>
        /// Gets or sets the Top property.
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// </summary>

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(bool), typeof(SnapSettings), new PropertyMetadata(true));

        public bool Top
        {
            get
            {
                return (bool)GetValue(TopProperty);
            }

            set
            {
                SetValue(TopProperty, value);
            }
        }      


        #endregion      

        #region Helping Methods

        // Getting Element details and Current node....
        internal void GetLinePoint(Node node, List<Node> nodelist, string whichline)
        {
            if (whichline == "Vertical")
            {
                if (NodeSnapMode == (NodeSnapMode.Node | NodeSnapMode.Port) || NodeSnapMode == NodeSnapMode.Node)
                {
                    if (node.dc.View.SnapSettings.EnableSnapNode)
                    {
                        List<Point> DrawingLineVerticalCenterLine = new List<Point>(); ;
                        // Vertical Line Drawing........
                        if (CenterY)
                        {
                            DrawingLineVerticalCenterLine = this.GetVerticalCenterLinePoints(node, nodelist);
                            this.DrawLinePoints(node, nodelist, DrawingLineVerticalCenterLine, "Vertical");

                        }
                        if (Left)
                        {
                            if (DrawingLineVerticalCenterLine.Count <= 0)
                            {
                                List<Point> DrawingLines2 = this.GetVerticalLinePoints(node, nodelist);
                                this.DrawLinePoints(node, nodelist, DrawingLines2, "Vertical");
                            }
                        }
                        if (Right)
                        {

                            if (DrawingLineVerticalCenterLine.Count <= 0)
                            {
                                List<Point> DrawingHeightLines = this.GetHeightLinePoints(node, nodelist);
                                if (DrawingHeightLines.Count > 0)
                                {
                                    this.DrawLinePoints(node, nodelist, DrawingHeightLines, "Vertical");
                                }
                            }
                        }
                    }
                    if (NodeSnapMode == (NodeSnapMode.Node | NodeSnapMode.Port))
                    {
                        List<Point> verticalLinePts = this.GetVerticalPortpts(node, nodelist);
                        this.DrawLinePoints(node, nodelist, verticalLinePts, "Vertical");
                    }
                }
                else if(NodeSnapMode==NodeSnapMode.Port)
                {
                    List<Point> verticalLinePts = this.GetVerticalPortpts(node, nodelist);
                    this.DrawLinePoints(node, nodelist, verticalLinePts, "Vertical");
                }
            }
            else
            {
                if (NodeSnapMode == (NodeSnapMode.Node | NodeSnapMode.Port) || NodeSnapMode == NodeSnapMode.Node)
                {
                    if (node.dc.View.SnapSettings.EnableSnapNode)
                    {
                        List<Point> DrawingLineHorizontalCenterLine = new List<Point>();
                        //Horizontal Line Drawing.......
                        if (CenterX)
                        {
                            DrawingLineHorizontalCenterLine = this.GetHorizontalCenterLinePoints(node, nodelist);

                            this.DrawLinePoints(node, nodelist, DrawingLineHorizontalCenterLine, "Horizontal");
                        }

                        if (Top)
                        {
                            if (DrawingLineHorizontalCenterLine.Count <= 0)
                            {
                                List<Point> DrawingLines = this.GetHorizontalLinePoints(node, nodelist);
                                this.DrawLinePoints(node, nodelist, DrawingLines, "Horizontal");
                            }
                        }
                        if (Bottom)
                        {
                            if (DrawingLineHorizontalCenterLine.Count <= 0)
                            {
                                List<Point> DrawingLinesWidth = this.GetWidthLinePoints(node, nodelist);
                                if (DrawingLinesWidth.Count > 0)
                                {
                                    this.DrawLinePoints(node, nodelist, DrawingLinesWidth, "Horizontal");
                                }
                            }
                        }
                    }
                    if (NodeSnapMode == (NodeSnapMode.Node | NodeSnapMode.Port))
                    {
                        List<Point> HorizontalLinePts = this.GetHorizontalPortpts(node, nodelist);
                        this.DrawLinePoints(node, nodelist, HorizontalLinePts, "Horizontal");
                    }
                }
                else if (NodeSnapMode == NodeSnapMode.Port)
                {
                    List<Point> HorizontalLinePts = this.GetHorizontalPortpts(node, nodelist);
                    this.DrawLinePoints(node, nodelist, HorizontalLinePts, "Horizontal");
                }
            }
        }


        // Preaparing LinePoints and Removing Old line points.....
        private void DrawLinePoints(Node node, List<Node> nodelist, List<Point> DrawingLines, string WhichLine)
        {
            AdornerLayer adLayer = AdornerLayer.GetAdornerLayer(this.dia_page);
            Adorner[] asas = adLayer.GetAdorners(this.dia_page);
            GettingCurrentPoints(WhichLine);
            RemovingOldLinePoints(DrawingLines, adLayer, asas, WhichLine);
            if (DrawingLines.Count > 0)
            {
                GetLinePointDetails(WhichLine, DrawingLines);
            }
            else
            {
                if (asas != null && asas.ToList<Adorner>().Count <= 1 && this._CommonLines != null && WhichLine == "Vertical")
                {
                }
                else
                if (asas != null && asas.ToList<Adorner>().Count > 1 && this._CommonLines != null)
                {
                    using (List<Point>.Enumerator enumerator = this._CommonLines.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            Point point = enumerator.Current;
                            if (asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)] is SnapAdorner)
                            adLayer.Remove(asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)]);
                        }
                    }
                }
            }
        }


        Adorner _currentAdorner;
        // Removing old LinePoints.......
        private void RemovingOldLinePoints(List<Point> DrawingLines, AdornerLayer adLayer, Adorner[] asas, string WhichLine)
        {
            if (DrawingLines.Count > 0)
            {
                if (this._CommonLines != null && this._CommonLines.Count > 0)
                {
                    if (!(this._CommonLines[0] == DrawingLines[0]) || !(this._CommonLines[1] == DrawingLines[1]))
                    {
                        asas = adLayer.GetAdorners(this.dia_page);
                        if (asas != null && asas.ToList<Adorner>().Count > 0)
                        {

                            foreach (Adorner ado in asas.ToList<Adorner>())
                            {
                                switch (WhichLine)
                                {
                                    case "Horizontal":
                                        {
                                            if (HorizontalLineAdorner.Contains(ado) && _currentAdorner != ado)
                                            {
                                                adLayer.Remove(ado);
                                            }
                                            break;
                                        }
                                    case "Vertical":
                                        {
                                            if (VerticalLineAdorner.Contains(ado) && _currentAdorner != ado)
                                            {
                                                adLayer.Remove(ado);
                                            }
                                            break;
                                        }
                                    case "Width":
                                        {
                                            if (WidhtLineAdorner.Contains(ado) && _currentAdorner != ado)
                                            {
                                                adLayer.Remove(ado);
                                            }
                                            break;
                                        }
                                    case "Height":
                                        {
                                            if (HeightLineAdorner.Contains(ado) && _currentAdorner != ado)
                                            {
                                                adLayer.Remove(ado);
                                            }
                                            break;
                                        }
                                }
                            }
                            //using (List<Point>.Enumerator enumerator = this._CommonLines.GetEnumerator())
                            //{
                            //    if (enumerator.MoveNext())
                            //    {
                            //        Point point = enumerator.Current;
                            //        Adorner ado = asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)];
                            //        if(_currentAdorner!=ado)
                            //        adLayer.Remove(asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)]);
                            //    }
                            //}
                        }
                    }
                }
                if (DrawingLines.Count > 0)
                {
                    SnapAdorner ad = new SnapAdorner(this.dia_page, DrawingLines, SnapLinePen);
                    _currentAdorner = ad;
                    GetLinePointDetails(WhichLine, DrawingLines);
                    switch (WhichLine)
                    {
                        case "Horizontal":
                            {
                                HorizontalLineAdorner.Add(ad);
                                break;
                            }
                        case "Vertical":
                            {
                                VerticalLineAdorner.Add(ad);
                                break;
                            }
                        case "Width":
                            {
                                WidhtLineAdorner.Add(ad);
                                break;
                            }
                        case "Height":
                            {
                                HeightLineAdorner.Add(ad);
                                break;
                            }
                    }
                    adLayer.Add(ad);
                    this._oldStartPoint = DrawingLines[0];
                    this._oldadorner = ad;
                }
                else
                {
                    if (asas != null && asas.ToList<Adorner>().Count > 0)
                    {
                        using (List<Point>.Enumerator enumerator = this._CommonLines.GetEnumerator())
                        {
                            if (enumerator.MoveNext())
                            {
                                Point point = enumerator.Current;
                                if (asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)] is SnapAdorner)
                                adLayer.Remove(asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)]);
                            }
                        }
                    }
                }
            }
        }

        //Getting Current Orientation Old points.....
        private void GettingCurrentPoints(string Whichline)
        {
            switch (Whichline)
            {
                case "Horizontal":
                    {
                        _CommonLines = _oldHorizontalDrawlines;
                        break;
                    }
                case "Vertical":
                    {
                        _CommonLines = _oldVerticalLines;
                        break;
                    }
                case "Width":
                    {
                        _CommonLines = _oldWidthLines;
                        break;
                    }
                case "Height":
                    {
                        _CommonLines = _oldHeightLines;
                        break;
                    }

            }
        }


        // Preparing Old points.....
        private void GetLinePointDetails(string WhichLine, List<Point> drawLines)
        {
            if (drawLines.Count > 0)
            {
                switch (WhichLine)
                {
                    case "Horizontal":
                        {
                            this._oldHorizontalDrawlines = drawLines;
                            break;
                        }
                    case "Vertical":
                        {
                            this._oldVerticalLines = drawLines;
                            break;
                        }
                    case "Width":
                        {
                            this._oldWidthLines = drawLines;
                            break;
                        }
                    case "Height":
                        {
                            this._oldHeightLines = drawLines;
                            break;
                        }
                }
            }
        }


        private List<Point> GetVerticalPortpts(Node node, List<Node> nodelist)
        {
            List<Point> VerticalPortline = new List<Point>();
            List<double> lines = new List<double>();
            ConnectionPort pairport = null;
            Node pairNode = null;
            ConnectionPort currentport=null;
            List<Node> pluswayNodes = node.Ports[0].PluswayNodes(node);
            foreach (ConnectionPort port in node.Ports)
            {
                foreach (Node _pairnode in pluswayNodes)
                {
                    foreach (ConnectionPort _pairport in _pairnode.Ports)
                    {
                        if (port != _pairport && port.Left + port.Node.OffsetX + port.Width / 2 <= _pairport.Left + _pairport.Node.OffsetX + AdjustDistance + _pairport.Width / 2 && port.Left + port.Node.OffsetX + port.Width / 2 > _pairport.Left + _pairport.Node.OffsetX - AdjustDistance + _pairport.Width / 2)
                        {
                            lines.Add(_pairport.Top + _pairport.Node.OffsetY + _pairport.Height / 2);
                            pairport = _pairport;
                            currentport=port;
                            pairNode = _pairnode;
                        }
                    }
                }
            }
            if (lines.Count > 0)
            {
                double diffx = pairport.Left + pairport.Node.OffsetX + pairport.Width / 2 - (currentport.Left + currentport.Node.OffsetX + currentport.Width / 2);
                node.OffsetX += diffx;
                Point StartPoint = new Point(pairport.Left + pairport.Node.OffsetX, currentport.Top + currentport.Node.OffsetY);
                Point EndPoint = new Point(pairport.Left + pairport.Node.OffsetX, pairport.Top + pairport.Node.OffsetY);
                if (StartPoint.Y <= EndPoint.Y)
                {
                    VerticalPortline.Add(StartPoint);
                    VerticalPortline.Add(EndPoint);
                }
                else
                {
                    VerticalPortline.Add(EndPoint);
                    VerticalPortline.Add(StartPoint);
                }
            }
            return VerticalPortline;
        }
        private List<Point> GetHorizontalPortpts(Node node, List<Node> nodelist)
        {
            List<Point> VerticalPortline = new List<Point>();
            List<double> lines = new List<double>();
            ConnectionPort pairport = null;
            Node pairNode = null;
            ConnectionPort currentport = null;
            List<Node> pluswayNodes = node.Ports[0].PluswayNodes(node);
            foreach (ConnectionPort port in node.Ports)
            {
                foreach (Node _pairnode in pluswayNodes)
                {
                    foreach (ConnectionPort _pairport in _pairnode.Ports)
                    {
                        if (port != _pairport && port.Top + port.Node.OffsetY + port.Height / 2 <= _pairport.Top + _pairport.Node.OffsetY + AdjustDistance + _pairport.Height / 2 && port.Top + port.Node.OffsetY + port.Height / 2 > _pairport.Top + _pairport.Node.OffsetY - AdjustDistance + _pairport.Height / 2)
                        {
                            lines.Add(_pairport.Left + _pairport.Node.OffsetX + _pairport.Width / 2);
                            pairport = _pairport;
                            currentport = port;
                            pairNode = _pairnode;
                        }
                    }
                }
            }
            if (lines.Count > 0)
            {
                double diffx = pairport.Top + pairport.Node.OffsetY + pairport.Height / 2 - (currentport.Top + currentport.Node.OffsetY + currentport.Height / 2);
                node.OffsetY += diffx;

                Point StartPoint = new Point(currentport.Left + currentport.Node.OffsetX, pairport.Top + pairport.Node.OffsetY);
                Point EndPoint = new Point(pairport.Left + pairport.Node.OffsetX, pairport.Top + pairport.Node.OffsetY);
                if (StartPoint.X <= EndPoint.X)
                {
                    VerticalPortline.Add(StartPoint);
                    VerticalPortline.Add(EndPoint);
                }
                else
                {
                    VerticalPortline.Add(EndPoint);
                    VerticalPortline.Add(StartPoint);
                }
            }
            return VerticalPortline;
        }
        // Get points for Horizontal line.....
        private List<Point> GetHorizontalLinePoints(Node node, List<Node> nodelist)
        {
            double orginpoint = node.OffsetY;
            double orginpointX = node.OffsetX;
            List<double> lines = new List<double>();
            Node pairnode = null;
            foreach (Node _node in nodelist)
            {
                if (_node != node && _node.OffsetY >= orginpoint - AdjustDistance && _node.OffsetY <= orginpoint + AdjustDistance)
                {
                    lines.Clear();
                    lines.Add(_node.OffsetX);
                    pairnode = _node;

                }
            }
            List<Point> HorizontalLine = new List<Point>();
            if (lines.Count > 0)
            {
                node.PxOffsetY = pairnode.PxOffsetY; //orginpoint;
                Point StartPoint = new Point(orginpointX, node.PxOffsetY);
                Point EndPoint = new Point(lines.Min(), node.PxOffsetY);
                if (StartPoint.X <= EndPoint.X)
                {
                    EndPoint = new Point(EndPoint.X + pairnode.Width, EndPoint.Y);
                    HorizontalLine.Add(StartPoint);
                    HorizontalLine.Add(EndPoint);
                }
                else
                {
                    StartPoint = new Point(StartPoint.X + node.Width, StartPoint.Y);
                    HorizontalLine.Add(EndPoint);
                    HorizontalLine.Add(StartPoint);
                }
            }
            return HorizontalLine;
        }

        // Get points for Horizontal line.....
        private List<Point> GetWidthLinePoints(Node node, List<Node> nodelist)
        {
            double orginpoint = node.OffsetY + node.Height;
            double orginpointX = node.OffsetX;

            //if (Ydirection)
            //{
            //    orginpoint = node.OffsetY+node.Height;
            //}
            Dictionary<double, Node> NodeDictionary = new Dictionary<double, Node>();
            List<double> lines = new List<double>();
            Node pairnode = null;
            foreach (Node _node in nodelist)
            {
                double widthOffX = _node.OffsetY + _node.Height;
                if (_node != node && widthOffX >= orginpoint - AdjustDistance && widthOffX <= orginpoint + AdjustDistance)
                {
                    lines.Clear();
                    lines.Add(_node.OffsetX);
                    pairnode = _node;

                }
            }
            List<Point> HorizontalLine = new List<Point>();
            if (lines.Count > 0)
            {
                node.PxOffsetY = pairnode.PxOffsetY + pairnode.Height - node.Height; //orginpoint;
                Point StartPoint = new Point(orginpointX, node.PxOffsetY + node.Height);
                Point EndPoint = new Point(lines.Min(), node.PxOffsetY + node.Height);
                if (StartPoint.X <= EndPoint.X)
                {
                    EndPoint = new Point(EndPoint.X + pairnode.Width, EndPoint.Y);
                    HorizontalLine.Add(StartPoint);
                    HorizontalLine.Add(EndPoint);
                }
                else
                {
                    StartPoint = new Point(StartPoint.X + node.Width, StartPoint.Y);
                    HorizontalLine.Add(EndPoint);
                    HorizontalLine.Add(StartPoint);
                }
            }
            return HorizontalLine;
        }


        // Get points for Horizontal Center line.....
        private List<Point> GetHorizontalCenterLinePoints(Node node, List<Node> nodelist)
        {
            double orginpoint = node.OffsetY + node.Height / 2;
            double orginpointX = node.OffsetX;

            //if (Ydirection)
            //{
            //    orginpoint = node.OffsetY+node.Height;
            //}
            Dictionary<double, Node> NodeDictionary = new Dictionary<double, Node>();
            List<double> lines = new List<double>();
            Node pairnode = null;
            double widthOffX = 0;
            foreach (Node _node in nodelist)
            {
                widthOffX = _node.OffsetY + _node.Height / 2;
                if (_node != node && widthOffX >= orginpoint - AdjustDistance && widthOffX <= orginpoint + AdjustDistance)
                {
                    lines.Clear();
                    lines.Add(_node.OffsetX);
                    pairnode = _node;

                }
            }
            List<Point> HorizontalLine = new List<Point>();
            if (lines.Count > 0)
            {
                node.PxOffsetY = pairnode.OffsetY + pairnode.Height / 2 - node.Height / 2;
                Point StartPoint = new Point(orginpointX, node.PxOffsetY + node.Height / 2);
                Point EndPoint = new Point(lines.Last(), node.PxOffsetY + node.Height / 2);
                if (StartPoint.X <= EndPoint.X)
                {
                    EndPoint = new Point(EndPoint.X + pairnode.Width, EndPoint.Y);
                    HorizontalLine.Add(StartPoint);
                    HorizontalLine.Add(EndPoint);
                }
                else
                {
                    StartPoint = new Point(StartPoint.X + node.Width, StartPoint.Y);
                    HorizontalLine.Add(EndPoint);
                    HorizontalLine.Add(StartPoint);
                }

            }
            return HorizontalLine;
        }




        // Get points for Vertical Line...
        private List<Point> GetVerticalLinePoints(Node node, List<Node> nodelist)
        {
            double orginpoint = node.OffsetX;
            double orginpointY = node.OffsetY;
            List<double> lines = new List<double>();
            Node pairnode = null;
            foreach (Node _node in nodelist)
            {
                if (_node != node && _node.OffsetX >= orginpoint - AdjustDistance && _node.OffsetX <= orginpoint + AdjustDistance)
                {
                    lines.Clear();
                    lines.Add(_node.OffsetY);
                    pairnode = _node;
                }
            }
            List<Point> HorizontalLine = new List<Point>();
            if (lines.Count > 0)
            {
                node.PxOffsetX = pairnode.PxOffsetX; //orginpoint;
                Point StartPoint = new Point(node.PxOffsetX, node.PxOffsetY);
                Point EndPoint = new Point(node.PxOffsetX, lines.Min());
                if (StartPoint.Y <= EndPoint.Y)
                {
                    EndPoint = new Point(EndPoint.X, EndPoint.Y + pairnode.Height);
                    HorizontalLine.Add(StartPoint);
                    HorizontalLine.Add(EndPoint);
                }
                else
                {
                    StartPoint = new Point(StartPoint.X, StartPoint.Y + node.Height);
                    HorizontalLine.Add(EndPoint);
                    HorizontalLine.Add(StartPoint);
                }


            }
            return HorizontalLine;
        }


        // Get points for Height line.....
        private List<Point> GetHeightLinePoints(Node node, List<Node> nodelist)
        {
            double orginpoint = node.OffsetX + node.Width;
            double orginpointX = node.OffsetY;

            //if (Ydirection)
            //{
            //    orginpoint = node.OffsetY+node.Height;
            //}
            Dictionary<double, Node> NodeDictionary = new Dictionary<double, Node>();
            List<double> lines = new List<double>();
            Node pairnode = null;
            foreach (Node _node in nodelist)
            {
                double HeightOffX = _node.OffsetX + _node.Width;
                if (_node != node && HeightOffX >= orginpoint - AdjustDistance && HeightOffX <= orginpoint + AdjustDistance)
                {
                    lines.Clear();
                    lines.Add(_node.OffsetY);
                    pairnode = _node;

                }
            }
            List<Point> HorizontalLine = new List<Point>();
            if (lines.Count > 0)
            {
                node.PxOffsetX = pairnode.PxOffsetX + pairnode.Width - node.Width; //orginpoint;
                Point StartPoint = new Point(node.OffsetX + node.Width, orginpointX); //new Point(orginpointX, node.PxOffsetY + node.Height);
                Point EndPoint = new Point(node.OffsetX + node.Width, lines.Last()); //new Point(lines.Max(), node.PxOffsetY + node.Height);
                if (StartPoint.Y <= EndPoint.Y)
                {
                    EndPoint = new Point(EndPoint.X, EndPoint.Y + pairnode.Height);
                    HorizontalLine.Add(StartPoint);
                    HorizontalLine.Add(EndPoint);
                }
                else
                {
                    StartPoint = new Point(StartPoint.X, StartPoint.Y + node.Height);
                    HorizontalLine.Add(EndPoint);
                    HorizontalLine.Add(StartPoint);
                }
            }
            return HorizontalLine;
        }



        // Get points for Vertical Center line.....
        private List<Point> GetVerticalCenterLinePoints(Node node, List<Node> nodelist)
        {
            double orginpoint = node.OffsetX + node.Width / 2;
            double orginpointX = node.OffsetY;

            //if (Ydirection)
            //{
            //    orginpoint = node.OffsetY+node.Height;
            //}
            Dictionary<double, Node> NodeDictionary = new Dictionary<double, Node>();
            List<double> lines = new List<double>();
            Node pairnode = null;
            foreach (Node _node in nodelist)
            {
                double widthOffX = _node.OffsetX + _node.Width / 2;
                if (_node != node && widthOffX >= orginpoint - AdjustDistance && widthOffX <= orginpoint + AdjustDistance)
                {
                    lines.Clear();
                    lines.Add(_node.OffsetY);
                    pairnode = _node;

                }
            }
            List<Point> HorizontalLine = new List<Point>();
            if (lines.Count > 0)
            {
                node.PxOffsetX = pairnode.OffsetX + pairnode.Width / 2 - node.Width / 2;
                Point StartPoint = new Point(node.PxOffsetX + node.Width / 2, orginpointX);
                Point EndPoint = new Point(node.PxOffsetX + node.Width / 2, lines.Min());
                if (StartPoint.Y <= EndPoint.Y)
                {
                    EndPoint = new Point(EndPoint.X, EndPoint.Y + pairnode.Height);
                    HorizontalLine.Add(StartPoint);
                    HorizontalLine.Add(EndPoint);
                }
                else
                {
                    StartPoint = new Point(StartPoint.X, StartPoint.Y + node.Height);
                    HorizontalLine.Add(EndPoint);
                    HorizontalLine.Add(StartPoint);
                }

            }
            return HorizontalLine;
        }



        // Clear all the adorner which added in Page.....
        internal void Clear()
        {
            if (this.dia_page != null)
            {
                AdornerLayer adLayer = AdornerLayer.GetAdornerLayer(this.dia_page);
                Adorner[] asas = adLayer.GetAdorners(this.dia_page);
                if (asas != null && asas.ToList<Adorner>().Count > 0)
                {
                    foreach (Adorner ado in asas.ToList<Adorner>())
                    {
                        if (ado is SnapAdorner)
                            adLayer.Remove(ado);
                    }
                }
                HorizontalLineAdorner.Clear();
                VerticalLineAdorner.Clear();
                WidhtLineAdorner.Clear();
                HeightLineAdorner.Clear();
            }
        }

        #endregion

    }
    #endregion

    #region SnapAdorner Class

    /// <summary>
    ///  MyAdorner class 
    /// </summary>
    /// 
    internal class SnapAdorner : Adorner
    {
        private Point point1;
        private Point point2;
        private Pen MyPen;
        public SnapAdorner(UIElement adornedElement, List<Point> points, Pen mypen)
            : base(adornedElement)
        {
            this.point1 = points[points.Count - 2];
            this.point2 = points[points.Count - 1];
            MyPen = mypen;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen drawpen;
            LinearGradientBrush bru = new LinearGradientBrush(Colors.Orange, Colors.OrangeRed, 45.0)
            {
                MappingMode = BrushMappingMode.RelativeToBoundingBox,
                ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation
            };
            if (MyPen == null)
            {
                drawpen = new Pen(bru, 0.5);
            }
            else
            { drawpen = MyPen; }
            drawingContext.DrawLine(drawpen, this.point1, this.point2);
        }
    }
     #endregion

}
