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

using System.Globalization;
using System.Windows;
using System.Windows.Media;

using System.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Common;

using System.ComponentModel;

namespace Syncfusion.Windows.Reports.Designer.Controls
{

    internal class Ruler : FrameworkElement
    {
        #region Fields
        private double SegmentHeight;
        
        #endregion

        #region Properties

        #region Length

        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register(
                "Length",
                typeof(double),
                typeof(Ruler),
                new FrameworkPropertyMetadata(20D, FrameworkPropertyMetadataOptions.AffectsRender|FrameworkPropertyMetadataOptions.AffectsMeasure));
        #endregion

        #region Chip

        public static readonly DependencyProperty ChipProperty =
            DependencyProperty.Register("Chip", typeof(double), typeof(Ruler),
                new FrameworkPropertyMetadata((double)-1000,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public double Chip
        {
            get { return (double)GetValue(ChipProperty); }
            set { SetValue(ChipProperty, value); }
        }

        #endregion

        #region CountShift

        public static readonly DependencyProperty CountShiftProperty =
            DependencyProperty.Register("CountShift", typeof(int), typeof(Ruler),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.AffectsRender));


        public int CountShift
        {
            get { return (int)GetValue(CountShiftProperty); }
            set { SetValue(CountShiftProperty, value); }
        }

        #endregion

        #region TickColor

        public static readonly DependencyProperty TickColorProperty =
            DependencyProperty.Register("TickColor", typeof(Brush), typeof(Ruler),
                new FrameworkPropertyMetadata(Brushes.Black,
                     FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush TickColor
        {
            get { return (Brush)GetValue(TickColorProperty); }
            set { SetValue(TickColorProperty, value); }
        }

        #endregion

        #region BorderColor

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Brush), typeof(Ruler),
                new FrameworkPropertyMetadata(Brushes.Transparent,
                     FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BorderColor
        {
            get { return (Brush)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        #endregion

        #region IndicatorColor

        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register("IndicatorColor", typeof(Brush), typeof(Ruler),
                new FrameworkPropertyMetadata(Brushes.Red,
                     FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush IndicatorColor
        {
            get { return (Brush)GetValue(IndicatorColorProperty); }
            set { SetValue(IndicatorColorProperty, value); }
        }

        #endregion

        #region TextColor

        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Brush), typeof(Ruler),
                new FrameworkPropertyMetadata(Brushes.Black,
                     FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        #endregion

        #region BackGroundColor

        public static readonly DependencyProperty BackGroundProperty =
            DependencyProperty.Register("BackGround", typeof(Brush), typeof(Ruler),
                new FrameworkPropertyMetadata(Brushes.White,
                     FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BackGround
        {
            get { return (Brush)GetValue(BackGroundProperty); }
            set { SetValue(BackGroundProperty, value); }
        }

        #endregion

        #region Marks

        public static readonly DependencyProperty MarksProperty =
            DependencyProperty.Register("Marks", typeof(MarksLocation), typeof(Ruler),
                new FrameworkPropertyMetadata(MarksLocation.Up,
                     FrameworkPropertyMetadataOptions.AffectsRender));

        public MarksLocation Marks
        {
            get { return (MarksLocation)GetValue(MarksProperty); }
            set { SetValue(MarksProperty, value); }
        }

        #endregion

        #region Unit

        public Unit Unit
        {
            get { return (Unit)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(
                "Unit",
                typeof(Unit),
                typeof(Ruler),
                new FrameworkPropertyMetadata(Unit.Cm, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #endregion

        #region Constructor
        static Ruler()
        {
            HeightProperty.OverrideMetadata(typeof(Ruler), new FrameworkPropertyMetadata(20.0));
        }
        public Ruler()
        {
            SegmentHeight = this.Height - 10;
        }
        #endregion

        #region Methods

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Pen p = new Pen(this.TickColor, 0.7);
            Pen ThinPen = new Pen(this.TickColor, 0.5);            
            Pen BorderPen = new Pen(this.BorderColor, 1);
            Pen RedPen = new Pen(this.IndicatorColor, 1);
            double xDest = Unit == Unit.Cm ? DipHelper.CmToDip(Length) : DipHelper.InchToDip(Length);
            double topVal = 0.0;
            double widthVal = this.ActualWidth;
            double heightVal = Height;
            string rulerParent = (this.Parent as Grid).Name;

            if (rulerParent == "GridBodyRuler" || rulerParent == "GridFooterRuler" || rulerParent == "GridHeaderRuler")
            {
                topVal = 8;
                widthVal -= 3;
            }
            else
            {
                heightVal -= 8;
            }

            drawingContext.DrawRectangle(this.BackGround, BorderPen, new Rect(new Point(0.0, topVal), new Point(widthVal, heightVal)));
            double chip = Unit == Unit.Cm ? DipHelper.CmToDip(Chip) : DipHelper.InchToDip(Chip);
            drawingContext.DrawLine(RedPen, new Point(chip, 0), new Point(chip, Height));

            for (double dUnit = 0; dUnit <= Length; dUnit++)
            {
                double d;
                if (Unit == Unit.Cm)
                {
                    d = DipHelper.CmToDip(dUnit);
                    if (dUnit < Length)
                    {
                        for (int i = 1; i <= 9; i++)
                        {
                            if (i != 5)
                            {
                                double dMm = DipHelper.CmToDip(dUnit + 0.1 * i);
                                if (Marks == MarksLocation.Up)
                                {
                                    drawingContext.DrawLine(ThinPen, new Point(dMm, 0), new Point(dMm, SegmentHeight / 3.0));
                                }
                                else
                                {
                                    drawingContext.DrawLine(ThinPen, new Point(dMm, Height), new Point(dMm, Height - SegmentHeight / 3.0));
                                }
                            }
                        }
                        double dMiddle = DipHelper.CmToDip(dUnit + 0.5);
                        if (Marks == MarksLocation.Up)
                        {
                            drawingContext.DrawLine(p, new Point(dMiddle, 0), new Point(dMiddle, SegmentHeight * 2.0 / 3.0));
                        }
                        else
                        {
                            drawingContext.DrawLine(p, new Point(dMiddle, Height), new Point(dMiddle, Height - SegmentHeight * 2.0 / 3.0));
                        }
                    }
                }
                else
                {
                    d = DipHelper.InchToDip(dUnit);
                    if (dUnit < Length)
                    {
                        if (Marks == MarksLocation.Up)
                        {
                            double dQuarter = DipHelper.InchToDip(dUnit + 0.25);
                            drawingContext.DrawLine(ThinPen, new Point(dQuarter, 0), new Point(dQuarter, SegmentHeight / 3.0));

                            double dMiddle = DipHelper.InchToDip(dUnit + 0.5);
                            drawingContext.DrawLine(p, new Point(dMiddle, 0), new Point(dMiddle, SegmentHeight * 2D / 3D));

                            double d3Quarter = DipHelper.InchToDip(dUnit + 0.75);
                            drawingContext.DrawLine(ThinPen, new Point(d3Quarter, 0), new Point(d3Quarter, SegmentHeight / 3.0));
                        }
                        else
                        {
                            double dQuarter = DipHelper.InchToDip(dUnit + 0.25);
                            drawingContext.DrawLine(ThinPen, new Point(dQuarter, Height), new Point(dQuarter, Height - SegmentHeight / 3.0));

                            double dMiddle = DipHelper.InchToDip(dUnit + 0.5);
                            drawingContext.DrawLine(p, new Point(dMiddle, Height), new Point(dMiddle, Height - SegmentHeight * 2D / 3D));

                            double d3Quarter = DipHelper.InchToDip(dUnit + 0.75);
                            drawingContext.DrawLine(ThinPen, new Point(d3Quarter, Height), new Point(d3Quarter, Height - SegmentHeight / 3.0));
                        }
                    }
                }

                if ((dUnit != 0.0) && (dUnit < Length))
                {
                    FormattedText ft = new FormattedText(
                       (dUnit + CountShift).ToString(CultureInfo.CurrentCulture),
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        DipHelper.PtToDip(8),
                        this.TextColor);

                    ft.SetFontWeight(System.Windows.FontWeights.Regular);
                    ft.TextAlignment = System.Windows.TextAlignment.Center;

                    if (Marks == MarksLocation.Up)
                    {
                        drawingContext.DrawText(ft, new Point(d, 0));
                    }
                    else
                    {
                        drawingContext.DrawText(ft, new Point(d, Height - ft.Height));
                    }
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize;
            if (Unit == Unit.Cm)
            {
                desiredSize = new Size(DipHelper.CmToDip(Length), Height );
            }
            else
            {
                double size =DipHelper.InchToDip(Length) - 2.5;
                size = size > 0 ? size : 0;
                desiredSize = new Size(size, Height);
            }
            return desiredSize ;
        }
        #endregion
    }


    internal enum Unit
    {
        Cm,
        Inch
    };

    internal enum MarksLocation
    {
        Up, Down
    }


    internal static class DipHelper
    {

        public static double MmToDip(double mm)
        {
            return CmToDip(mm / 10.0);
        }


        public static double CmToDip(double cm)
        {
            return (cm * 96.0 / 2.54);
        }


        public static double InchToDip(double inch)
        {
            return (inch * 96.0);
        }

        public static double DipToInch(double dip)
        {
            return dip / 96D;
        }

        public static double PtToDip(double pt)
        {
            return (pt * 96.0 / 72.0);
        }

        public static double DipToCm(double dip)
        {
            return (dip * 2.54 / 96.0);
        }

        public static double DipToMm(double dip)
        {
            return DipToCm(dip) * 10.0;
        }

        private static Point GetSystemDpiFactor()
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            Matrix m = source.CompositionTarget.TransformToDevice;
            return new Point(m.M11, m.M22);
        }

        private const double DpiBase = 96.0;

        public static Point GetSystemDpi()
        {
            Point sysDpiFactor = GetSystemDpiFactor();
            return new Point(
                sysDpiFactor.X * DpiBase,
                sysDpiFactor.Y * DpiBase);
        }

        public static Point GetPhysicalDpi(double diagonalScreenSize)
        {
            Point sysDpiFactor = GetSystemDpiFactor();
            double pixelScreenWidth = SystemParameters.PrimaryScreenWidth * sysDpiFactor.X;
            double pixelScreenHeight = SystemParameters.PrimaryScreenHeight * sysDpiFactor.Y;
            double formatRate = pixelScreenWidth / pixelScreenHeight;

            double inchHeight = diagonalScreenSize / Math.Sqrt(formatRate * formatRate + 1.0);
            double inchWidth = formatRate * inchHeight;

            double xDpi = Math.Round(pixelScreenWidth / inchWidth);
            double yDpi = Math.Round(pixelScreenHeight / inchHeight);

            return new Point(xDpi, yDpi);
        }

        public static Point DpiToScaleFactor(Point dpi)
        {
            Point sysDpi = GetSystemDpi();
            return new Point(
                dpi.X / sysDpi.X,
                dpi.Y / sysDpi.Y);
        }

        public static Point GetScreenIndependentScaleFactor(double diagonalScreenSize)
        {
            return DpiToScaleFactor(GetPhysicalDpi(diagonalScreenSize));
        }
    }
}
