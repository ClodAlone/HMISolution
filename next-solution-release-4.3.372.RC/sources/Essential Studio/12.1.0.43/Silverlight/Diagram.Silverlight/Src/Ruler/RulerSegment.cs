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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace Syncfusion.Windows.Diagram
{
    internal class RulerSegment : Control
    {

        internal enum LengthMode
        {
            MinorLength,
            MajorLength,
            IntermediateLength
        }

        Binding MStrokeBinding;
        Binding mStrokeBinding;
        Binding MThicknessBinding;
        Binding mThicknessBinding;

        internal Grid minorLineGrid;

        internal Line MajorLine;

        internal TextBlock SegmentValue;

        ResourceDictionary rs;

        internal Ruler ParentRuler
        {
            get;
            set;
        }

        internal double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        internal int NoOfLines
        {
            get;
            set;
        }

        private double SegmentWidth
        {
            get
            {
                return (double)GetValue(SegmentWidthProperty);
            }
            set
            {
                SetValue(SegmentWidthProperty, value);
            }
        }

        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(RulerSegment), new PropertyMetadata(Double.NaN, new PropertyChangedCallback(OnValuePropertyChanged)));

        private static readonly DependencyProperty SegmentWidthProperty = DependencyProperty.Register("SegmentWidth", typeof(double), typeof(RulerSegment), new PropertyMetadata(Double.NaN, new PropertyChangedCallback(OnSegmentWidthPropertyChanged)));
        
        private void UpdateNoOfLines()
        {
            RulerSegment rs = this;
            int current = rs.minorLineGrid.ColumnDefinitions.Count;

            if (current < rs.NoOfLines)
            {
                for (int i = current; i < rs.NoOfLines; i++)
                {
                    Line l = rs.addLine();
                }
            }
            else if (current > rs.NoOfLines)
            {
                int extra = 0;
                for (int i = current - 1; i >= rs.NoOfLines; i--)
                {
                    Line l = minorLineGrid.Children[i + extra] as Line;
                    minorLineGrid.Children.Remove(l);
                    minorLineGrid.ColumnDefinitions.RemoveAt(i);
                }
            }
        }

        internal Line addLine()
        {
            Line line = new Line() { X1 = 0, Y1 = 0, X2 = 0, Y2 = 2000 };
            ColumnDefinition cd = new ColumnDefinition();
            minorLineGrid.ColumnDefinitions.Add(cd);
            minorLineGrid.Children.Add(line);
            double d = (double)Math.Floor(ParentRuler.Scale);
            if (d > 2)
            {
                d = Math.Floor(Math.Log(d, 2));
                d = Math.Pow(2, d);
            }
            else if (d < 1)
            {
                d = Math.Floor(ParentRuler.Scale * 10);
                d = Math.Floor(Math.Log(d, 2));
                d = Math.Pow(2, d);
                d /= 10;
            }
            minorLineGrid.Width = ParentRuler.PxRoundedIntervalValue * ParentRuler.Scale / Math.Max(d, 0.1);
            Grid.SetColumn(line, minorLineGrid.ColumnDefinitions.IndexOf(cd));
            {
                line.SetBinding(Line.StrokeProperty, mStrokeBinding);
                line.SetBinding(Line.StrokeThicknessProperty, mThicknessBinding);
            }            
            return line;
        }
        Random r = new Random();
       
        void line_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ParentRuler.MinorLinesPerInterval += 1;
            this.ParentRuler.MarkerUp = !this.ParentRuler.MarkerUp;
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnSegmentWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public RulerSegment()
        {
            this.DefaultStyleKey = typeof(RulerSegment);
            this.rs = new ResourceDictionary();
            this.rs.Source = new Uri("/Syncfusion.Diagram.Silverlight;component/Themes/RulerSegmentStyle.xaml", UriKind.RelativeOrAbsolute);
            this.Template = ((rs["RulerSegmentStyle"] as Style).Setters[0] as Setter).Value as ControlTemplate;
            this.Loaded += new RoutedEventHandler(RulerSegment_Loaded);
        }

        void RulerSegment_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Ruler parent = GetParentRuler(this);
            if (parent != null && parent is Ruler)
            {
                ParentRuler = parent;
            }

            MStrokeBinding = new Binding("MajorLinesStroke");
            MStrokeBinding.ElementName = ParentRuler.Name;
            MStrokeBinding.BindsDirectlyToSource = true;

            MThicknessBinding = new Binding("MajorLinesThickness");
            MThicknessBinding.ElementName = ParentRuler.Name;
            MThicknessBinding.BindsDirectlyToSource = true;

            mStrokeBinding = new Binding("MinorLinesStroke");
            mStrokeBinding.ElementName = ParentRuler.Name;
            mStrokeBinding.BindsDirectlyToSource = true;

            mThicknessBinding = new Binding("MinorLinesThickness");
            mThicknessBinding.ElementName = ParentRuler.Name;
            mThicknessBinding.BindsDirectlyToSource = true;

            parent.MarkerUpChanged += new EventHandler(parent_MarkerUpChanged);
            parent.PxMinorLineCountPerIntervalChanged += new EventHandler(parent_PxMinorLineCountPerIntervalChanged);
            minorLineGrid = GetTemplateChild("PART_Lines") as Grid;
            SegmentValue = GetTemplateChild("PART_Value") as TextBlock;
            MajorLine = GetTemplateChild("PART_MajorLine") as Line;

            minorLineGrid.UseLayoutRounding = false;

            Binding LengthBinding = new Binding("MinorLength");
            LengthBinding.ElementName = ParentRuler.Name;
            minorLineGrid.SetBinding(Grid.HeightProperty, LengthBinding);

            LengthBinding = new Binding("MajorLength");
            LengthBinding.ElementName = ParentRuler.Name;
            MajorLine.SetBinding(Line.HeightProperty, LengthBinding);

            MajorLine.SetBinding(Line.StrokeProperty, MStrokeBinding);
            MajorLine.SetBinding(Line.StrokeThicknessProperty, MThicknessBinding);


            minorLineGrid.SizeChanged += new SizeChangedEventHandler(minorLineGrid_SizeChanged);

            minorLineGrid.Width = ParentRuler.PxRoundedIntervalValue;
            ParentRuler.PxIntervalValueChanged += new EventHandler(ParentRuler_PxIntervalValueChanged);

            NoOfLines = parent.MinorLinesPerInterval + 1;
            UpdateNoOfLines();
            Binding LabelFontColorBind = new Binding("LabelFontColor");
            LabelFontColorBind.Source = ParentRuler;
            SegmentValue.SetBinding(TextBlock.ForegroundProperty, LabelFontColorBind);

            Binding RulerSegmentValueBind = new Binding("InvalidateRulerSegmentValue");
            RulerSegmentValueBind.Source = ParentRuler;
            object[] param = new object[3];
            param[0] = this;
            param[1] = ParentRuler;
            //param[2] = SegmentValue;
            RulerSegmentValueBind.ConverterParameter = param;
            RulerSegmentValueBind.Converter = new RulerSegmentValueConverter();
            SegmentValue.SetBinding(TextBlock.TextProperty, RulerSegmentValueBind);


            Binding MarkerUPBinding = new Binding("MarkerUp");
            MarkerUPBinding.Converter = new MarkerConverter();
            MarkerUPBinding.ConverterParameter = false;
            MarkerUPBinding.Source = ParentRuler;
            minorLineGrid.SetBinding(Grid.VerticalAlignmentProperty, MarkerUPBinding);
            MajorLine.SetBinding(Line.VerticalAlignmentProperty, MarkerUPBinding);
            Binding iMarkerUPBinding = new Binding("MarkerUp");
            iMarkerUPBinding.Converter = new MarkerConverter();
            iMarkerUPBinding.ConverterParameter = true;
            iMarkerUPBinding.Source = ParentRuler;
            SegmentValue.SetBinding(TextBlock.VerticalAlignmentProperty, iMarkerUPBinding);
        }

        void minorLineGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //SegmentValue.Width = e.NewSize.Width - 3;
        }

        void ParentRuler_PxIntervalValueChanged(object sender, EventArgs e)
        {
            double d = (double)Math.Floor(ParentRuler.Scale);
            if (d > 2)
            {
                d = Math.Floor(Math.Log(d, 2));
                d = Math.Pow(2, d);
            }
            else if (d < 1)
            {
                d = Math.Floor(ParentRuler.Scale * 10);
                d = Math.Floor(Math.Log(d, 2));
                d = Math.Pow(2, d);
                d /= 10;
            }
            minorLineGrid.Width= ParentRuler.PxRoundedIntervalValue * ParentRuler.Scale / Math.Max(d, 0.1); //( 1 + ( ParentRuler.Scale % 1));
            foreach (RulerSegment rule in ParentRuler.NegativeScale.Children)
            {
                rule.Width = ParentRuler.PxRoundedIntervalValue * ParentRuler.Scale / Math.Max(d, 0.1); //( 1 + ( ParentRuler.Scale % 1));
            }
        }

        void parent_PxMinorLineCountPerIntervalChanged(object sender, EventArgs e)
        {
            RulerSegment rs = this;
            rs.NoOfLines = (int)(e as Ruler.ValueChangedEventArgs).NewValue + 1;
            rs.UpdateNoOfLines();
        }

        void parent_MarkerUpChanged(object sender, EventArgs e)
        {
        }
       
        private Ruler GetParentRuler(UIElement rulerSegment)
        {
            Object obj = VisualTreeHelper.GetParent(rulerSegment);
            if (obj != null)
            {
                if (obj is Ruler)
                {
                    return obj as Ruler;
                }
                else
                {
                    return GetParentRuler(obj as UIElement);
                }
            }
            return null;
        }

    }

    class RulerSegmentValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object[] param = parameter as object[];
            RulerSegment Segment = (RulerSegment)param[0];
            Ruler ParentRuler = (Ruler)param[1];
            TextBlock SegmentValue = Segment.SegmentValue;

            Segment.minorLineGrid.Width = ParentRuler.PxRoundedIntervalValue;
            string ret;
            if (Segment.Value < 0)
            {
                SegmentValue.FlowDirection = FlowDirection.LeftToRight;
                //SegmentValue.Text = Value * MURoundedIntervalValue + "";
                ret = Segment.Value * ParentRuler.MURoundedIntervalValue + "";
            }
            else
            {
                //SegmentValue.Text = Value * MURoundedIntervalValue + "";
                double d = (double)Math.Floor(ParentRuler.Scale);
                if (d > 2)
                {
                    d = Math.Floor(Math.Log(d, 2));
                    d = Math.Pow(2, d);
                }
                else if (d < 1)
                {
                    d = Math.Floor(ParentRuler.Scale * 10);
                    d = Math.Floor(Math.Log(d, 2));
                    d = Math.Pow(2, d);
                    d /= 10;
                }
                ret = (Segment.Value * ParentRuler.MURoundedIntervalValue / Math.Max(d, 0.1)).ToString("0.######") + "";
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
