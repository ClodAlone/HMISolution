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
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Controls.Schedule
{
#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  clas that holds schedule rectangular border
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleRectangleBorder : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleRectangleBorder"/> class.
        /// </summary>
        public ScheduleRectangleBorder()
        {
            this.DefaultStyleKey = typeof(ScheduleRectangleBorder);
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LeftBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LeftBrushProperty = DependencyProperty.Register("LeftBrush", typeof(Brush), typeof(ScheduleRectangleBorder), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets brush value for LeftBrush
        /// </summary>
        public Brush LeftBrush
        {
            get
            {
                return (Brush)this.GetValue(ScheduleRectangleBorder.LeftBrushProperty);
            }

            set
            {
                this.SetValue(ScheduleRectangleBorder.LeftBrushProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for RightBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RightBrushProperty = DependencyProperty.Register("RightBrush", typeof(Brush), typeof(ScheduleRectangleBorder), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets brush value for right brush
        /// </summary>
        public Brush RightBrush
        {
            get
            {
                return (Brush)this.GetValue(ScheduleRectangleBorder.RightBrushProperty);
            }

            set
            {
                this.SetValue(ScheduleRectangleBorder.RightBrushProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TopBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TopBrushProperty = DependencyProperty.Register("TopBrush", typeof(Brush), typeof(ScheduleRectangleBorder), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets brush value for TopBrush
        /// </summary>
        public Brush TopBrush
        {
            get
            {
                return (Brush)this.GetValue(ScheduleRectangleBorder.TopBrushProperty);
            }

            set
            {
                this.SetValue(ScheduleRectangleBorder.TopBrushProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for BottomBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BottomBrushProperty = DependencyProperty.Register("BottomBrush", typeof(Brush), typeof(ScheduleRectangleBorder), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets brush value for BottomBrush
        /// </summary>
        public Brush BottomBrush
        {
            get
            {
                return (Brush)this.GetValue(ScheduleRectangleBorder.BottomBrushProperty);
            }

            set
            {
                this.SetValue(ScheduleRectangleBorder.BottomBrushProperty, value);
            }
        }
    }

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  Class that holds rectangular border for schedule
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleRectangleBorderExt : ScheduleRectangleBorder
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleRectangleBorderExt"/>
        /// class.
        /// </summary>
        public ScheduleRectangleBorderExt()
        {
            this.DefaultStyleKey = typeof(ScheduleRectangleBorderExt);           
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsSelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ScheduleRectangleBorder), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var rectangle = dpo as ScheduleRectangleBorderExt;
            rectangle.GotoSelectedState();
        }

        private void GotoSelectedState()
        {
            if (this.IsSelected)
            {
                VisualStateManager.GoToState(this, "Selected", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }


        /// <summary>
        /// Gets / Sets if this rectangle is selected under a particular time slot.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(ScheduleRectangleBorderExt.IsSelectedProperty);
            }

            set
            {
                this.SetValue(ScheduleRectangleBorderExt.IsSelectedProperty, value);
            }
        }

      

        /// <summary>
        /// Gets / Sets the SelectionBackground property.
        /// </summary>
        public Brush SelectionBackground
        {
            get { return (Brush)GetValue(SelectionBackgroundProperty); }
            set { SetValue(SelectionBackgroundProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SelectionBackground.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionBackgroundProperty = DependencyProperty.Register("SelectionBackground",
            typeof(Brush), typeof(ScheduleRectangleBorderExt), new PropertyMetadata(null));

        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsMarked.  This
        /// enables animation, styling, binding, etc...
        /// </summary>

        public static readonly DependencyProperty IsMarkedProperty = DependencyProperty.Register("IsMarked", typeof(bool), typeof(ScheduleRectangleBorderExt), new PropertyMetadata(false, OnIsMarkChanged));

        private static void OnIsMarkChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var rectangle = dpo as ScheduleRectangleBorderExt;
            rectangle.GotoMarkState();
        }

        private void GotoMarkState()
        {
            if (this.IsMarked)
            {
                VisualStateManager.GoToState(this, "Marked", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        /// <summary>
        /// Gets / Sets the mark state if this rectangle is under a particular Schedule Appointment.
        /// </summary>
        public bool IsMarked
        {
            get
            {
                return (bool)this.GetValue(ScheduleRectangleBorderExt.IsMarkedProperty);
            }

            set
            {
                this.SetValue(ScheduleRectangleBorderExt.IsMarkedProperty, value);
            }
        }

    }

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class that holds Rectangle for Schedule View
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleHorizontalRectangleSel : ScheduleRectangleBorder
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalRectangleSel"/>
        /// class.
        /// </summary>
        public ScheduleHorizontalRectangleSel()
        {
            this.DefaultStyleKey = typeof(ScheduleHorizontalRectangleSel);
        }

        /// <summary>
        ///  reference for MousePointerType
        /// </summary>
        public MousePointerType DragStatus;
        /// <summary>
        ///  reference for mouse resize position
        /// </summary>
        public ResizePosition MousePosition;
        private Rectangle TopLeft;
        private Rectangle TopRight;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.TopLeft = this.GetTemplateChild("TopLeft") as Rectangle;
            this.TopRight = this.GetTemplateChild("TopRight") as Rectangle;
            this.SetupEvents();
            base.OnApplyTemplate();
        }

        private void SetupEvents()
        {
            this.TopLeft.MouseEnter += new MouseEventHandler(TopLeft_MouseEnter);
            this.TopLeft.MouseLeave += new MouseEventHandler(TopLeft_MouseLeave);
            this.TopRight.MouseEnter += new MouseEventHandler(TopRight_MouseEnter);
            this.TopRight.MouseLeave += new MouseEventHandler(TopRight_MouseLeave);
        }

        private void TopLeft_MouseLeave(object sender, MouseEventArgs e)
        {
            DragStatus = MousePointerType.None;
            MousePosition = ResizePosition.None;
        }

        private void TopLeft_MouseEnter(object sender, MouseEventArgs e)
        {
            MousePosition = ResizePosition.Left;
            DragStatus = MousePointerType.Resize;
        }

        private void TopRight_MouseLeave(object sender, MouseEventArgs e)
        {
            DragStatus = MousePointerType.None;
            MousePosition = ResizePosition.None;
        }

        private void TopRight_MouseEnter(object sender, MouseEventArgs e)
        {
            MousePosition = ResizePosition.Right;
            DragStatus = MousePointerType.Resize;
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsSelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool),
            typeof(ScheduleHorizontalRectangleSel), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var rectangle = dpo as ScheduleHorizontalRectangleSel;
            rectangle.GotoSelectedState();
        }

        private void GotoSelectedState()
        {
            if (this.IsSelected)
            {
                VisualStateManager.GoToState(this, "AllSelected", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "AllDeSelected", false);
            }
        }

        /// <summary>
        /// Gets / Sets if this rectangle is selected under a particular time slot.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(ScheduleHorizontalRectangleSel.IsSelectedProperty);
            }

            set
            {
                this.SetValue(ScheduleHorizontalRectangleSel.IsSelectedProperty, value);
            }
        }
    }


    /// <summary>
    ///  class that holds cross lines
    /// </summary>
    public class ScheduleCrosslineControl : Control
    {
        private Image ImageViewport;
        private Grid LayoutRoot;
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleCrosslineControl"/> class.
        /// </summary>
        public ScheduleCrosslineControl()
        {
            this.DefaultStyleKey = typeof(ScheduleCrosslineControl);
        }

        #region Interval (DependencyProperty)

        /// <summary>
        /// Gets / Sets the TimeInterval.
        /// </summary>
        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Interval.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(int), typeof(ScheduleCrosslineControl), new PropertyMetadata(5, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var scheduleCrosslineControl = dpo as ScheduleCrosslineControl;
            scheduleCrosslineControl.DrawCrossLine(5);
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ImageViewport = this.GetTemplateChild("ImageViewport") as Image;
            this.LayoutRoot = this.GetTemplateChild("LayoutRoot") as Grid;
            this.SizeChanged += new SizeChangedEventHandler(ScheduleCrosslineControl_SizeChanged);
            DrawCrossLine(5);
        }

        void ScheduleCrosslineControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawCrossLine(5);
        }

        private void DrawCrossLine(int displacement)
        {
#if SILVERLIGHT
            WriteableBitmap writableBitMap = new WriteableBitmap((int)this.LayoutRoot.ActualWidth, (int)this.LayoutRoot.ActualHeight);         
            
            LineDrawWritableBitMap lineDrawWritableBitMap = new LineDrawWritableBitMap();
            //lineDrawWritableBitMap.DrawLine(writableBitMap, 0, 0, 100, 100, Colors.Blue);
            this.ImageViewport.Source = writableBitMap;
            //int posY1 = 0;
            //int posX1 = 0;
            //int posY2 = 0;
            //int posX2 = 0;
            //for (; posX1 < (this.LayoutRoot).ActualWidth && posY2 < (this.LayoutRoot).ActualHeight; )
            //{
            //    if (posY1 >= (int)this.LayoutRoot.ActualHeight)
            //    {
            //        posY1 = (int)this.LayoutRoot.ActualHeight;
            //        posX1 = posX1 + this.Interval;
            //    }
            //    if (posX2 >= (int)this.LayoutRoot.ActualWidth)
            //    {
            //        posX2 = (int)this.LayoutRoot.ActualWidth;
            //        posY2 = posY2 + this.Interval;
            //    }
            //((SolidColorBrush)this.BorderBrush).Color
            // lineDrawWritableBitMap.DrawLine(writableBitMap, posX1, posY1, posX2, posY2, Colors.Blue);
            //  posY1 = posY1 + this.Interval;
            //  posX2 = posX2 + this.Interval;
            //}
            Color crossLineColor = new Color();
            crossLineColor.A = 255;
            crossLineColor.R = 176;
            crossLineColor.G = 182;
            crossLineColor.B = 190;
                    int posY1 = 5;
                    int posX1 = 0;
                    int posY2 = 0;
                    int posX2 = 5;
                    for (; posY2 < (this.LayoutRoot).ActualHeight; )
                    {                        
                        
                        Point p1 = new Point(posX1, posY1);
                        Point p2 = new Point(posX2, posY2);
                        lineDrawWritableBitMap.DrawLine(writableBitMap, posX1, posY1, posX2, posY2, crossLineColor);
                        posY1 = posY1 + this.Interval;
                        posY2 = posY2 + this.Interval;
                    }
            
#else
            if ((int)this.LayoutRoot.ActualWidth > 0 && (int)this.LayoutRoot.ActualHeight  > 0)
            {

                //WriteableBitmap writableBitMap = new WriteableBitmap((int)this.LayoutRoot.ActualWidth, (int)this.LayoutRoot.ActualHeight, 96, 96, PixelFormats.Bgr32, null);

                SolidColorBrush s = new SolidColorBrush();
                s.Color = Color.FromArgb(255,176,182,190);
                Pen p = new Pen(s ,1); 
                DrawingGroup dg = new DrawingGroup();
                using (DrawingContext dc = dg.Open())
                {                    
                    int posY1 = 5;
                    int posX1 = 0;
                    int posY2 = 0;
                    int posX2 = 5;
                    for (; posY2 < (this.LayoutRoot).ActualHeight; )
                    {                        
                        
                        Point p1 = new Point(posX1, posY1);
                        Point p2 = new Point(posX2, posY2);
                        dc.DrawLine(p, p1, p2);
                        posY1 = posY1 + this.Interval;
                        posY2 = posY2 + this.Interval;
                    }
                }                
                DrawingImage drawImage = new DrawingImage(dg);
                this.ImageViewport.Source = drawImage;
            }
#endif
        }
    }
}
