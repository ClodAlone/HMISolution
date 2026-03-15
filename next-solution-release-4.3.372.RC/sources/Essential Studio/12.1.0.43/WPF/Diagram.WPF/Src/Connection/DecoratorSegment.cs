#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Shared;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Collections;

namespace Syncfusion.Windows.Diagram
{
    public abstract partial class ConnectorBase
    {        
        internal ObservableCollection<DecoratorProperty> DecoratorSegments
        {
            get { return (ObservableCollection<DecoratorProperty>)GetValue(DecoratorSegmentsProperty); }
            set { SetValue(DecoratorSegmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecoratorSegments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DecoratorSegmentsProperty =
            DependencyProperty.Register("DecoratorSegments", typeof(ObservableCollection<DecoratorProperty>), typeof(ConnectorBase), new UIPropertyMetadata(null));
        
        public SegmentDecoratorSettings SegmentDecoratorSettings
        {
            get { return (SegmentDecoratorSettings)GetValue(SegmentDecoratorSettingsProperty); }
            set { SetValue(SegmentDecoratorSettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SegmentDecoratorSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SegmentDecoratorSettingsProperty =
            DependencyProperty.Register("SegmentDecoratorSettings", typeof(SegmentDecoratorSettings), typeof(ConnectorBase), new UIPropertyMetadata(null, OnSegmentDecoratorSettingsChanged));

        private static void OnSegmentDecoratorSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConnectorBase cb = d as ConnectorBase;
            SegmentDecoratorSettings setting = e.NewValue as SegmentDecoratorSettings;
            if (setting != null)
            {
                if (setting.SegmentDecorator is INotifyCollectionChanged)
                {
                    (setting.SegmentDecorator as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(cb.ConnectorBase_CollectionChanged);
                }
                cb.UpdateDecoratorSegment();
            }
        }

        internal void UpdateDecoratorSegment()
        {
            if (ConnectorPathGeometry == null || ConnectorPathGeometry.Bounds.IsEmpty)
            {
                return;
            }
            //List<DecoratorProperty> pts = new List<DecoratorProperty>();
            if (DecoratorSegments == null)
            {
                DecoratorSegments = new ObservableCollection<DecoratorProperty>();
            }

            if (SegmentDecoratorSettings != null)
            {
                switch (SegmentDecoratorSettings.Context)
                {
                    case LineContext.Line:
                        UpdateDecoratorSegment_Line(DecoratorSegments);
                        break;
                    case LineContext.Segment:
                        UpdateDecoratorSegment_Segment(DecoratorSegments);
                        break;
                }
            }
            //PopulateDecoratorShapes(pts);
        }

        //private void PopulateDecoratorShapes(List<DecoratorProperty> pts)
        //{
        //    while(pts.Count != DecoratorSegments.Count)
        //    {
        //        if (pts.Count < DecoratorSegments.Count)
        //        {
        //            DecoratorSegments.RemoveAt(0);
        //        }
        //        else if(pts.Count > DecoratorSegments.Count)
        //        {
        //            DecoratorSegments.Add(new Path());
        //        }
        //    }

        //    foreach (Path path in DecoratorSegments)
        //    {
                
        //    }
        //}

        private void UpdateDecoratorSegment_Line(ObservableCollection<DecoratorProperty> pts)
        {
            switch (SegmentDecoratorSettings.Unit)
            {
                case LineUnit.AbsoluteFraction:
                    break;
                case LineUnit.RelativeFraction:
                    break;
                //case LineUnit.AbsoluteValue:
                //    break;
                //case LineUnit.RelativeValue:
                //    break;
            }
        }

        private void UpdateDecoratorSegment_Segment(ObservableCollection<DecoratorProperty> pts)
        {
            List<SegmentDecorator> decorators = new List<SegmentDecorator>(); ;
            if (SegmentDecoratorSettings.SegmentDecorator != null)
                decorators = SegmentDecoratorSettings.SegmentDecorator.OfType<SegmentDecorator>().ToList();

            int i = 0;
            PathSegment preSeg = null;
            foreach (PathSegment seg in ConnectorPathGeometry.Figures[0].Segments)
            {
                if (decorators.Count > 0)
                {
                    double runDecoratorOffset = 0d;
                    do
                    {
                        foreach (SegmentDecorator dec in decorators)
                        {
                            Point st = new Point(0, 0);
                            if (preSeg == null)
                            {
                                st = ConnectorPathGeometry.Figures[0].StartPoint;
                            }
                            else if (preSeg is LineSegment)
                            {
                                st = (preSeg as LineSegment).Point;
                            }
                            else if (preSeg is PolyLineSegment)
                            {
                                st = (preSeg as PolyLineSegment).Points.Last();
                            }
                            else if (preSeg is BezierSegment)
                            {
                                st = (preSeg as BezierSegment).Point3;
                            }
                            else if (preSeg is PolyBezierSegment)
                            {
                                st = (preSeg as PolyBezierSegment).Points.Last();
                            }
                            else if (preSeg is QuadraticBezierSegment)
                            {
                                st = (preSeg as QuadraticBezierSegment).Point2;
                            }
                            else if (preSeg is PolyQuadraticBezierSegment)
                            {
                                st = (preSeg as PolyQuadraticBezierSegment).Points.Last();
                            }

                            switch (SegmentDecoratorSettings.Unit)
                            {
                                case LineUnit.AbsoluteFraction:
                                    runDecoratorOffset = dec.DecoratorOffset;
                                    break;
                                case LineUnit.RelativeFraction:
                                    runDecoratorOffset += dec.DecoratorOffset;
                                    break;
                                case LineUnit.AbsoluteValue:
                                    double fract = ValueToFraction(dec.DecoratorOffset, st, seg, dec.DecoratorShape);
                                    runDecoratorOffset = fract;
                                    break;
                                case LineUnit.RelativeValue:
                                    fract = ValueToFraction(dec.DecoratorOffset + runDecoratorOffset, st, seg, dec.DecoratorShape);
                                    runDecoratorOffset += fract;
                                    break;
                            }

                            DecoratorProperty decProp = GetDecoratorPropertyAtFractionLength(runDecoratorOffset, st, seg, dec.DecoratorShape);

                            if (runDecoratorOffset > 1 || decProp == null)
                            {
                                continue;
                            }

                            if (dec.CustomDecoratorStyle == null)
                            {
                                if (SegmentDecoratorSettings.CustomDecoratorStyle == null)
                                {
                                    decProp.DecoratorStyle = this.CustomTailDecoratorStyle;
                                }
                                else
                                {
                                    decProp.DecoratorStyle = SegmentDecoratorSettings.CustomDecoratorStyle;
                                }
                            }
                            else
                            {
                                decProp.DecoratorStyle = dec.CustomDecoratorStyle;
                            }
                            //this.Dispatcher.BeginInvoke(new Test(m), null);
                            if (pts.Count > i)
                            {
                                pts[i].Angle = decProp.Angle;
                                pts[i].DecoratorShape = decProp.DecoratorShape;
                                pts[i].DecoratorStyle = decProp.DecoratorStyle;
                                pts[i].Position = decProp.Position;
                            }
                            else
                            {
                                pts.Add(decProp);
                            }
                            i++;
                        }
                    }
                    while ((SegmentDecoratorSettings.Unit == LineUnit.RelativeFraction && runDecoratorOffset <= 1)
                            || (SegmentDecoratorSettings.Unit == LineUnit.RelativeValue && runDecoratorOffset <= 1));
                    preSeg = seg;
                }
            }
            while (pts.Count > i)
            {
                pts.Remove(pts.Last());
            }
        }
                
        private DecoratorProperty GetDecoratorPropertyAtFractionLength(double offset, Point start, PathSegment seg, DecoratorShape shape)
        { 
            DecoratorProperty prop = new DecoratorProperty();
            PathGeometry geo = new PathGeometry();
            PathFigure fig = new PathFigure() { StartPoint = start };
            geo.Figures.Add(fig);
            fig.Segments.Add(seg.Clone());
            Point tangent = new Point();
            Point angle;
            if (geo.Bounds == Rect.Empty || (geo.Bounds.Width==0 && geo.Bounds.Height==0))
            {
                return null;
            }
            geo.GetPointAtFractionLength(offset, out angle, out tangent);
            prop.Position = angle;
            prop.Angle = Math.Atan2(tangent.Y, tangent.X) * (180 / Math.PI);
            prop.DecoratorShape = shape;
            return prop;
        }

        private double ValueToFraction(double offset, Point start, PathSegment seg, DecoratorShape shape)
        {
            double length = 0;
            if(seg is LineSegment)
            {
                length = (start - (seg as LineSegment).Point).Length;
            }
            double frac = offset / length;
            if (double.IsNaN(frac) || double.IsInfinity(frac) || frac == 0)
            {
                return -1;
            }
            else
            {
                return frac;
            }
        }

        internal class DecoratorProperty : INotifyPropertyChanged
        {
            private Point m_Position;
            public Point Position
            {
                get { return m_Position; }
                set
                {
                    if (m_Position != value)
                    {
                        m_Position = value;
                        OnPropertyChanged("Position");
                    }
                }
            }

            private double m_Angle;
            public double Angle
            {
                get { return m_Angle; }
                set
                {
                    if (m_Angle != value)
                    {
                        m_Angle = value;
                        OnPropertyChanged("Angle");
                    }
                }
            }

            private Style m_DecoratorStyle;
            public Style DecoratorStyle
            {
                get { return m_DecoratorStyle; }
                set
                {
                    if (m_DecoratorStyle != value)
                    {
                        m_DecoratorStyle = value;
                        OnPropertyChanged("DecoratorStyle");
                    }
                }
            }

            private DecoratorShape m_DecoratorShape;
            public DecoratorShape DecoratorShape
            {
                get { return m_DecoratorShape; }
                set
                {
                    if (m_DecoratorShape != value)
                    {
                        m_DecoratorShape = value;
                        OnPropertyChanged("DecoratorShape");
                    }
                }
            }

            protected void OnPropertyChanged(string propName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void ConnectorBase_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

    }
    
    public class SegmentDecoratorSettings : DependencyObject
    {
        /// <summary>
        /// Gets or sets Context in which decorators should be placed.
        /// </summary>
        /// <remarks>The Context of placing decorator can be complete line or segments</remarks>
        internal LineContext Context
        {
            get { return (LineContext)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        /// <summary>
        /// Identifies the Context dependency property.
        /// </summary>
        internal static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register("Context", typeof(LineContext), typeof(SegmentDecoratorSettings), new UIPropertyMetadata(LineContext.Segment));

        /// <summary>
        /// Gets or sets Unit type for the offset.
        /// </summary>
        /// <remarks>Offset mentioned can be in ratio or absolute value</remarks>
        public LineUnit Unit
        {
            get { return (LineUnit)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        /// <summary>
        /// Identifies the Unit dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(LineUnit), typeof(SegmentDecoratorSettings), new UIPropertyMetadata(LineUnit.RelativeFraction));

        /// <summary>
        /// Gets or sets SegmentDecorator.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IEnumerable SegmentDecorator
        {
            get { return (IEnumerable)GetValue(SegmentDecoratorProperty); }
            set { SetValue(SegmentDecoratorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SegmentDecorator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SegmentDecoratorProperty =
            DependencyProperty.Register("SegmentDecorator", typeof(IEnumerable), typeof(SegmentDecoratorSettings), new UIPropertyMetadata(null));

        private static void OnSegmentDecoratorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Gets or sets CustomDecoratorStyle.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Style CustomDecoratorStyle
        {
            get { return (Style)GetValue(CustomDecoratorStyleProperty); }
            set { SetValue(CustomDecoratorStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomDecoratorStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomDecoratorStyleProperty =
            DependencyProperty.Register("CustomDecoratorStyle", typeof(Style), typeof(SegmentDecoratorSettings), new UIPropertyMetadata(null));

    }

    public class SegmentDecorator
    {
        /// <summary>
        /// Gets or sets DecoratorOffset.
        /// </summary>
        /// <remarks></remarks>
        public double DecoratorOffset { get; set; }

        /// <summary>
        /// Gets or sets CustomDecoratorStyle.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Style CustomDecoratorStyle { get; set; }

        /// <summary>
        /// Gets or sets DecoratorShape.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public DecoratorShape DecoratorShape { get; set; }

    }

}
