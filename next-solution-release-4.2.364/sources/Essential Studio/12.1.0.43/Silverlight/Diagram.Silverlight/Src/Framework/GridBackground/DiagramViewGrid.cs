#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the diagram grid.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class DiagramViewGrid : Control
    {
        #region Initialization

        private Grid m_HorGrid;
        private Grid m_VerGrid;
        internal DiagramView m_View;
        private double m_HorizontalGridLineCount;
        private double m_VerticalGridLineCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramViewGrid"/> class.
        /// </summary>
        public DiagramViewGrid()
        {
            DefaultStyleKey = typeof(DiagramViewGrid);
            this.IsHitTestVisible = false;
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.m_HorGrid = GetTemplateChild("HorGrid") as Grid;
            this.m_VerGrid = GetTemplateChild("VerGrid") as Grid;
            m_View = GetParentDV(this);
            if (m_View != null)
            {
                m_View.dviewGrid = this;
                Binding visBinding = new Binding("ShowHorizontalGridLine");
                visBinding.Source = m_View;
                visBinding.Converter= new BooleanToVisibilityConverter();
                this.m_HorGrid.SetBinding(DiagramViewGrid.VisibilityProperty, visBinding);

                visBinding = new Binding("ShowVerticalGridLine");
                visBinding.Source = m_View;
                visBinding.Converter = new BooleanToVisibilityConverter();
                this.m_VerGrid.SetBinding(DiagramViewGrid.VisibilityProperty, visBinding);
            }
            this.SizeChanged += new SizeChangedEventHandler(DiagramViewGrid_SizeChanged);
        }

        void DiagramViewGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
            if(m_View.ShowVerticalGridLine==true || m_View.ShowVerticalGridLine==true)
            UpdateGrid();
        }

        internal void RearrrangeHorGridLines()
        {
            if (m_HorGrid != null)
            {
                foreach (RowDefinition row in m_HorGrid.RowDefinitions)
                {
                    row.Height = new GridLength((m_View.Page as DiagramPage).PxGridHorizontalOffset);
                    
                }
                UpdateGrid();
            }
        }

        internal void RearrrangeVerGridLines()
        {
            if (m_VerGrid != null)
            {
                foreach (ColumnDefinition col in m_VerGrid.ColumnDefinitions)
                {
                    col.Width = new GridLength((m_View.Page as DiagramPage).PxGridVerticalOffset);
                }
                UpdateGrid();
            }
        }
        internal void UpdateGrid()
        {
                if (m_View == null)
                {
                    m_View = GetParentDV(this);
                }
                if (m_View != null)
                {

                    if (m_View.ShowVerticalGridLine == true)
                    
                    {
                        //m_HorizontalGridLineCount = Math.Round(Math.Max(m_View.Scrollviewer.ActualHeight, m_View.Scrollviewer.ExtentHeight) + (m_View.Page as DiagramPage).Top / (m_View.Page as DiagramPage).PxGridHorizontalOffset) + 1;
                        m_VerticalGridLineCount = Math.Round(Math.Max(m_View.Scrollviewer.ActualWidth, m_View.Scrollviewer.ExtentWidth) + (m_View.Page as DiagramPage).Left / (m_View.Page as DiagramPage).PxGridVerticalOffset) + 1;

                        if (m_VerGrid.Children.Count != m_VerticalGridLineCount)
                        {
                            if (m_VerGrid.Children.Count > m_VerticalGridLineCount)
                            {
                                for (int i = m_VerGrid.Children.Count; i > m_VerticalGridLineCount; i--)
                                {
                                    m_VerGrid.Children.RemoveAt(i - 1);
                                    m_VerGrid.ColumnDefinitions.RemoveAt(m_VerGrid.ColumnDefinitions.Count - 1);
                                }
                            }
                            else if (m_VerGrid.Children.Count < m_VerticalGridLineCount)
                            {
                                for (int i = m_VerGrid.Children.Count; i < m_VerticalGridLineCount; i++)
                                {
                                    m_VerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength((m_View.Page as DiagramPage).PxGridVerticalOffset) });
                                    Line newLine = new Line();
                                    //newLine.StrokeThickness = 2;
                                    //newLine.Stroke = new SolidColorBrush(Colors.Gray);
                                    newLine.Stretch = Stretch.Fill;
                                    newLine.Y2 = 1;
                                    m_View.VerticalGridLineStyle.SetGridLineStyle(newLine);
                                    newLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    m_VerGrid.Children.Add(newLine);
                                    Grid.SetColumn(newLine, i);
                                }
                            }
                        }
                    }
                    if (m_View.ShowHorizontalGridLine == true)
                    {
                        m_HorizontalGridLineCount = Math.Round(Math.Max(m_View.Scrollviewer.ActualHeight, m_View.Scrollviewer.ExtentHeight) + (m_View.Page as DiagramPage).Top / (m_View.Page as DiagramPage).PxGridHorizontalOffset) + 1;
                        if (m_HorGrid.Children.Count != m_HorizontalGridLineCount)
                        {
                            if (m_HorGrid.Children.Count > m_HorizontalGridLineCount)
                            {
                                for (int i = m_HorGrid.Children.Count; i > m_HorizontalGridLineCount; i--)
                                {
                                    m_HorGrid.Children.RemoveAt(i - 1);
                                    m_HorGrid.RowDefinitions.RemoveAt(m_HorGrid.RowDefinitions.Count - 1);
                                }
                            }
                            else if (m_HorGrid.Children.Count < m_HorizontalGridLineCount)
                            {
                                for (int i = m_HorGrid.Children.Count; i < m_HorizontalGridLineCount; i++)
                                {
                                    m_HorGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength((m_View.Page as DiagramPage).PxGridHorizontalOffset) });
                                    Line newLine = new Line();
                                    //newLine.StrokeThickness = 2;
                                    //newLine.Stroke = new SolidColorBrush(Colors.Gray);
                                    newLine.Stretch = Stretch.Fill;
                                    newLine.X2 = 1;
                                    m_View.HorizontalGridLineStyle.SetGridLineStyle(newLine);
                                    newLine.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                    m_HorGrid.Children.Add(newLine);
                                    Grid.SetRow(newLine, i);
                                }
                            }
                        }
                    }
                    //m_HorGrid.Width = m_Page.DesiredSize.Width;
                    //m_HorGrid.Height = m_Page.DesiredSize.Height;
                }
           
        }

        private DiagramView GetParentDV(UIElement dv)
        {
            Object obj = VisualTreeHelper.GetParent(dv);
            if (obj != null)
            {
                if (obj is DiagramView)
                {
                    return obj as DiagramView;
                }
                else
                {
                    return GetParentDV(obj as UIElement);
                }
            }
            return null;
        }

    }


    public class GridLineStyle : DependencyObject
    {
        public GridLineStyle()
        {
        }

        internal void SetGridLineStyle(Line ln)
        {
            SetBinding("Brush", ln, Line.StrokeProperty);
            SetBinding("StrokeThickness", ln, Line.StrokeThicknessProperty);
        }

        private void SetBinding(string property, Line ln, DependencyProperty dp)
        {
            Binding bind = new Binding(property);
            bind.Source = this;
            ln.SetBinding(dp, bind);
        }

       

        public Brush Brush
        {
            get
            {
                return (Brush)GetValue(BrushProperty);
            }

            set
            {
                SetValue(BrushProperty, value);
            }
        }

        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof(Brush), typeof(GridLineStyle), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray), new PropertyChangedCallback(OnBrushPropertyChanged)));

        private static void OnBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        
        public double StrokeThickness
        {
            get
            {
                return (double)GetValue(StrokeThicknessProperty);
            }

            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(GridLineStyle), new PropertyMetadata(0.3d, new PropertyChangedCallback(OnStrokeThicknessPropertyChanged)));

        private static void OnStrokeThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }        internal DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)GetValue(StrokeDashArrayProperty);
            }

            set
            {
                SetValue(StrokeDashArrayProperty, value);
            }
        }

        internal static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(GridLineStyle), new PropertyMetadata(null, new PropertyChangedCallback(OnStrokeDashArrayPropertyChanged)));

        internal static void OnStrokeDashArrayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal PenLineCap StrokeEndLineCap
        {
            get
            {
                return (PenLineCap)GetValue(StrokeEndLineCapProperty);
            }

            set
            {
                SetValue(StrokeEndLineCapProperty, value);
            }
        }

        internal static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(GridLineStyle), new PropertyMetadata(PenLineCap.Flat, new PropertyChangedCallback(OnStrokeEndLineCapPropertyChanged)));

        internal static void OnStrokeEndLineCapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal double StrokeDashOffset
        {
            get
            {
                return (double)GetValue(StrokeDashOffsetProperty);
            }

            set
            {
                SetValue(StrokeDashOffsetProperty, value);
            }
        }

        internal static readonly DependencyProperty StrokeDashOffsetProperty = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(GridLineStyle), new PropertyMetadata(0d, new PropertyChangedCallback(OndoublePropertyChanged)));

        internal static void OndoublePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal PenLineCap StrokeDashCap
        {
            get
            {
                return (PenLineCap)GetValue(StrokeDashCapProperty);
            }

            set
            {
                SetValue(StrokeDashCapProperty, value);
            }
        }

        internal static readonly DependencyProperty StrokeDashCapProperty = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(GridLineStyle), new PropertyMetadata(PenLineCap.Flat, new PropertyChangedCallback(OnStrokeDashCapChanged)));

        internal static void OnStrokeDashCapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal PenLineJoin StrokeLineJoin
        {
            get
            {
                return (PenLineJoin)GetValue(StrokeLineJoinProperty);
            }

            set
            {
                SetValue(StrokeLineJoinProperty, value);
            }
        }

        internal static readonly DependencyProperty StrokeLineJoinProperty = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(GridLineStyle), new PropertyMetadata(PenLineJoin.Bevel, new PropertyChangedCallback(OnStrokeLineJoinPropertyChanged)));

        internal static void OnStrokeLineJoinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal double StrokeMiterLimit
        {
            get
            {
                return (double)GetValue(StrokeMiterLimitProperty);
            }

            set
            {
                SetValue(StrokeMiterLimitProperty, value);
            }
        }

        internal static readonly DependencyProperty StrokeMiterLimitProperty = DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(GridLineStyle), new PropertyMetadata(0d, new PropertyChangedCallback(OnStrokeMiterLimitPropertyChanged)));

        internal static void OnStrokeMiterLimitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal PenLineCap StrokeStartLineCap
        {
            get
            {
                return (PenLineCap)GetValue(StrokeStartLineCapProperty);
            }

            set
            {
                SetValue(StrokeStartLineCapProperty, value);
            }
        }

        internal static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(GridLineStyle), new PropertyMetadata(PenLineCap.Flat, new PropertyChangedCallback(OnStrokeStartLineCapPropertyChanged)));

        internal static void OnStrokeStartLineCapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

    }


}
