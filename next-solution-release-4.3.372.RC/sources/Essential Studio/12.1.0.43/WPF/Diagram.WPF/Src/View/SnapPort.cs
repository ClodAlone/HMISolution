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
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Syncfusion.Windows.Diagram
{
    internal class SnapPort : DependencyObject
    {
        #region Variables
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
        internal double AdjustDistance =5;
        internal DiagramPage diaPage;
        #endregion
        public SnapPort(DiagramPage diapage)
        {
            this.diaPage = diapage;
        }


        internal void GetLinePoint(Node node,ConnectionPort C_port, List<ConnectionPort> nodelist, string whichline)
        {
            Clear();
            List<Point> DrawingLineVerticalCenterLine = new List<Point>(); ;
            // Vertical Line Drawing........
            DrawingLineVerticalCenterLine =this.GetVerticalCenterLinePoints(C_port, nodelist);
            List<Point> DrawingLineHorizontalCenterLine = new List<Point>(); ;
             DrawingLineHorizontalCenterLine= this.GetHorizontaCenterLinePoints(C_port, nodelist);
            this.DrawLinePoints(node, nodelist, DrawingLineVerticalCenterLine, "Vertical");
            this.DrawLinePoints(node, nodelist, DrawingLineHorizontalCenterLine, "Horizontal");
        }

        #region Adorner PART I 
        //// Preaparing LinePoints and Removing Old line points.....
        private void DrawLinePoints(Node node, List<ConnectionPort> nodelist, List<Point> DrawingLines, string WhichLine)
        {
            AdornerLayer adLayer = AdornerLayer.GetAdornerLayer(this.diaPage);
            Adorner[] asas = adLayer.GetAdorners(this.diaPage);
            GettingCurrentPoints(WhichLine);
            RemovingOldLinePoints(DrawingLines, adLayer, asas, WhichLine);
            if (DrawingLines.Count > 0)
            {
                GetLinePointDetails(WhichLine, DrawingLines);
            }
            else
            {
                if (asas != null && asas.ToList<Adorner>().Count <=1 && this._CommonLines != null && WhichLine=="Vertical")
                {
                }else
                     if (asas != null && asas.ToList<Adorner>().Count > 1 && this._CommonLines != null)
                     {
                    using (List<Point>.Enumerator enumerator = this._CommonLines.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            Point point = enumerator.Current;
                            adLayer.Remove(asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)]);
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
                        asas = adLayer.GetAdorners(this.diaPage);
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
                            using (List<Point>.Enumerator enumerator = this._CommonLines.GetEnumerator())
                            {
                                if (enumerator.MoveNext())
                                {
                                    Point point = enumerator.Current;
                                    Adorner ado = asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)];
                                    if (_currentAdorner != ado)
                                        adLayer.Remove(asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)]);
                                }
                            }
                        }
                    }
                }
                if (DrawingLines.Count > 0)
                {
                    SnapAdorner ad = new SnapAdorner(this.diaPage, DrawingLines, null);
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
                                adLayer.Remove(asas.ToList<Adorner>()[this._CommonLines.IndexOf(point)]);
                            }
                        }
                    }
                }
            }
        }

        // Clear all the adorner which added in Page.....
        internal void Clear()
        {
            if (this.diaPage != null)
            {
                AdornerLayer adLayer = AdornerLayer.GetAdornerLayer(this.diaPage);
                Adorner[] asas = adLayer.GetAdorners(this.diaPage);
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

        // Get points for Vertical Center line.....
        #endregion

        private List<Point> GetVerticalCenterLinePoints(ConnectionPort Port_node, List<ConnectionPort> portlist)
        {
            //Point orginPosition = Port_node.TransformToAncestor(Port_node.diagramPage as Panel).Transform(new Point(Port_node.Width / 2, Port_node.Height / 2));
            //double orginpoint = orginPosition.X;
            //double orginpointX = orginPosition.Y;
            List<double> lines = new List<double>();
            ConnectionPort pairport = null;
            foreach (ConnectionPort _port in portlist)
            {
                Point Position = _port.TransformToAncestor(diaPage as Panel).Transform(new Point(_port.Width / 2, _port.Height / 2));
                if (Port_node != _port && Port_node.Left + Port_node.Node.OffsetX + Port_node.Width / 2 <= _port.Left + _port.Node.OffsetX + AdjustDistance + _port.Width / 2 && Port_node.Left + Port_node.Node.OffsetX + Port_node.Width / 2 > _port.Left + _port.Node.OffsetX - AdjustDistance + _port.Width / 2)
                {
                    lines.Add(_port.Top + _port.Node.OffsetY + _port.Height / 2);
                    pairport = _port;
                    pairport.CenterPosition = pairport.TransformToAncestor(pairport.diagramPage as Panel).Transform(new Point(pairport.Width / 2, pairport.Height / 2));
                }
            }
            List<Point> HorizontalLine = new List<Point>();
            if (lines.Count > 0)
            {
            Port_node.Left = (pairport.CenterPosition.X) - Port_node.Node.OffsetX;
            Port_node.CenterPosition = Port_node.TransformToAncestor(Port_node.diagramPage as Panel).Transform(new Point(Port_node.Width / 2, Port_node.Height / 2));
            Point StartPoint = new Point(Port_node.CenterPosition.X,Port_node.CenterPosition.Y);
            Point EndPoint = new Point(pairport.CenterPosition.X,pairport.CenterPosition.Y);
            if (StartPoint.Y <= EndPoint.Y)
            {
                HorizontalLine.Add(StartPoint);
                HorizontalLine.Add(EndPoint);
            }
            else
            {
                HorizontalLine.Add(EndPoint);
                HorizontalLine.Add(StartPoint);
            }
            }
            return HorizontalLine;
        }

        private List<Point> GetHorizontaCenterLinePoints(ConnectionPort Port_node, List<ConnectionPort> portlist)
        {
            //Point orginPosition = Port_node.TransformToAncestor(Port_node.diagramPage as Panel).Transform(new Point(Port_node.Width / 2, Port_node.Height / 2));
            //double orginpoint = orginPosition.Y;//orginPosition.X;
            //double orginpointX = orginPosition.X;//orginPosition.Y;
            List<double> lines = new List<double>();
            ConnectionPort pairport = null;
            foreach (ConnectionPort _port in portlist)
            {
                Point Position = _port.TransformToAncestor(diaPage as Panel).Transform(new Point(_port.Width / 2, _port.Height / 2));
                if (Port_node != _port && Port_node.Top + Port_node.Node.OffsetY + Port_node.Height / 2 <= _port.Top + _port.Node.OffsetY + AdjustDistance + _port.Height / 2 && Port_node.Top + Port_node.Node.OffsetY + Port_node.Height / 2 > _port.Top + _port.Node.OffsetY - AdjustDistance + _port.Height / 2)
                {
                    lines.Add(_port.Left + _port.Node.OffsetX  + _port.Width / 2);
                    pairport = _port;
                    pairport.CenterPosition = pairport.TransformToAncestor(pairport.diagramPage as Panel).Transform(new Point(pairport.Width / 2, pairport.Height / 2));
                }
            }
            List<Point> HorizontalLine = new List<Point>();
            if (lines.Count > 0)
            {
                Port_node.Top = (pairport.CenterPosition.Y) - Port_node.Node.OffsetY;
                Port_node.CenterPosition = Port_node.TransformToAncestor(Port_node.diagramPage as Panel).Transform(new Point(Port_node.Width / 2, Port_node.Height / 2));
                Point StartPoint = new Point(Port_node.CenterPosition.X, Port_node.CenterPosition.Y);
                Point EndPoint = new Point(pairport.CenterPosition.X, pairport.CenterPosition.Y);
                if (StartPoint.Y <= EndPoint.Y)
                {
                    HorizontalLine.Add(StartPoint);
                    HorizontalLine.Add(EndPoint);
                }
                else
                {
                    HorizontalLine.Add(EndPoint);
                    HorizontalLine.Add(StartPoint);
                }
            }
            return HorizontalLine;
        }

    }
}
