#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    ///
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class ExtendedScrollingAdorner : Adorner
    {
        private Thumb valueThumb;
        private VisualCollection visualCollection;
        private Rect adornedElementRect;
        private FrameworkElement fElement;
        private double prevVerticalChange, prevHorizontalChange;
        private CursorHandler.POINT pnt;
        private bool isThumbMoved;
        private StreamResourceInfo info;
        private Cursor scrollingCursor;

        /// <summary>
        ///
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (fElement is EditorBase)
                    return (fElement as EditorBase).IsReadOnly;
                if (fElement is TimeSpanEdit)
                    return (fElement as TimeSpanEdit).IsReadOnly;
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="adornedElement"></param>
        public ExtendedScrollingAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            valueThumb = new Thumb { Opacity = 0d };
            visualCollection = new VisualCollection(this) {valueThumb};
            if (AdornedElement != null) 
                adornedElementRect = new Rect(AdornedElement.DesiredSize);
            fElement = adornedElement as FrameworkElement;
            ResourceDictionary rd = new ResourceDictionary
                {
                    Source =
                        new Uri("/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Generic.xaml",
                                UriKind.RelativeOrAbsolute)
                };
            info = Application.GetResourceStream(new Uri("/Syncfusion.Shared.WPF;component/Controls/Editors/Cursors/SizeAllCursor.cur", UriKind.RelativeOrAbsolute));
            if (info != null) 
                scrollingCursor = new Cursor(info.Stream);
            Style thumbStyle = rd["ExtendedScrollingAdornerStyle"] as Style;
            valueThumb.Style = thumbStyle;
            if (fElement != null)
            {
                if (fElement.IsLoaded)
                    ArrangeThumb();
                fElement.PreviewMouseMove += fElement_PreviewMouseMove;
                fElement.IsKeyboardFocusedChanged += new DependencyPropertyChangedEventHandler(fElement_IsKeyboardFocusedChanged);
                fElement.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(fElement_IsKeyboardFocusWithinChanged);
            }
            valueThumb.DragDelta += new DragDeltaEventHandler(valueThumb_DragDelta);
            valueThumb.PreviewMouseUp += valueThumb_PreviewMouseUp;
            valueThumb.PreviewMouseDown += new MouseButtonEventHandler(valueThumb_PreviewMouseDown);
            valueThumb.PreviewMouseMove += new MouseEventHandler(valueThumb_PreviewMouseMove);
        }

        private void valueThumb_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (IsReadOnly && valueThumb != null)
            {
               valueThumb.Visibility = Visibility.Collapsed;
            }
        }

        private void fElement_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && valueThumb !=null)
            {
                valueThumb.Visibility = Visibility.Collapsed;
                valueThumb.Cursor = Cursors.IBeam;
            }
        }

        private void fElement_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && valueThumb !=null)
            {
                valueThumb.Visibility = Visibility.Collapsed;
                valueThumb.Cursor = Cursors.IBeam;
            }
        }

        private void valueThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CursorHandler.GetCursorPos(out pnt);
        }

        private void fElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (valueThumb != null)
            {
                if (!IsReadOnly)
                {
                    if (!fElement.IsFocused)
                    {
                        valueThumb.Visibility = Visibility.Visible;
                        valueThumb.Cursor = scrollingCursor;
                    }
                }
                else
                {
                    valueThumb.Visibility = Visibility.Collapsed;
                    valueThumb.Cursor = Cursors.IBeam;
                }
            }
        }

        private void valueThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBox tBox = AdornedElement as TextBox;
            if (tBox != null)
            {
                if (!tBox.IsFocused && !isThumbMoved)
                {
                    tBox.Focus();
                    if (valueThumb != null) 
                        valueThumb.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (valueThumb != null) 
                        valueThumb.Cursor = scrollingCursor;
                    CursorHandler.SetCursorPos(pnt.X, pnt.Y);
                }
            }
            isThumbMoved = false;
        }

        private void valueThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (valueThumb != null) 
                valueThumb.Cursor = Cursors.None;
            if (!IsReadOnly)
            {
                if (e.HorizontalChange > prevHorizontalChange || e.VerticalChange < prevVerticalChange)
                    ValueChange(true);
                else
                    ValueChange(false);
                prevHorizontalChange = e.HorizontalChange;
                prevVerticalChange = e.VerticalChange;
                if (Math.Abs(e.HorizontalChange) > 5d || Math.Abs(e.VerticalChange) > 5d)
                {
                    isThumbMoved = true;
                }
            }
        }

        private void ValueChange(bool increase)
        {
            if (AdornedElement is IntegerTextBox)
            {
                IntegerTextBox iTextBox = AdornedElement as IntegerTextBox;
                if (increase)
                {
                    var newValue = iTextBox.Value + iTextBox.ScrollInterval;
                    if (newValue <= iTextBox.MaxValue && ((iTextBox.MaxLength == 0 || (newValue.ToString().Length <= iTextBox.MaxLength))))
                        iTextBox.Value = newValue;
                }
                else
                {
                    var newValue = iTextBox.Value - iTextBox.ScrollInterval;
                    if (newValue >= iTextBox.MinValue)
                        iTextBox.Value = newValue;
                }
            }

            if (AdornedElement is DoubleTextBox)
            {
                DoubleTextBox dTextBox = AdornedElement as DoubleTextBox;
                if (increase)
                {
                    var newValue = dTextBox.Value + dTextBox.ScrollInterval;
                    if (newValue <= dTextBox.MaxValue && ((dTextBox.MaxLength == 0 || (newValue.ToString().Length <= dTextBox.MaxLength))))
                        dTextBox.Value = newValue;
                }
                else
                {
                    var newValue = dTextBox.Value - dTextBox.ScrollInterval;
                    if (newValue >= dTextBox.MinValue)
                        dTextBox.Value = newValue;
                }
            }

            if (AdornedElement is PercentTextBox)
            {
                PercentTextBox pTextBox = AdornedElement as PercentTextBox;
                if (increase)
                {
                    var newValue = pTextBox.PercentValue + pTextBox.ScrollInterval;
                    if (newValue <= pTextBox.MaxValue && newValue >= pTextBox.MinValue)
                        pTextBox.PercentValue = newValue;
                }
                else
                {
                    var newValue = pTextBox.PercentValue - pTextBox.ScrollInterval;
                    if (newValue >= pTextBox.MinValue)
                        pTextBox.PercentValue = newValue;
                }
            }

            if (AdornedElement is CurrencyTextBox)
            {
                CurrencyTextBox cTextBox = AdornedElement as CurrencyTextBox;
                if (increase)
                {
                    var newValue = cTextBox.Value + (decimal)cTextBox.ScrollInterval;
                    if (newValue <= cTextBox.MaxValue && ((cTextBox.MaxLength == 0 || (newValue.ToString().Length <= cTextBox.MaxLength))))
                        cTextBox.Value = newValue;
                }
                else
                {
                    var newValue = cTextBox.Value - (decimal)cTextBox.ScrollInterval;
                    if (newValue >= cTextBox.MinValue)
                        cTextBox.Value = newValue;
                }
            }
            if (AdornedElement is TimeSpanEdit)
            {
                TimeSpanEdit tSpanEdit = AdornedElement as TimeSpanEdit;
                if (increase)
                {
                    tSpanEdit.UpExecute();
                }
                else
                {
                    tSpanEdit.DownExecute();
                }
            }
        }

        private void ArrangeThumb()
        {
            if (valueThumb != null)
            {
                if (fElement != null)
                    valueThumb.Arrange(new Rect(adornedElementRect.TopLeft.X, adornedElementRect.TopLeft.Y, fElement.RenderSize.Width, fElement.RenderSize.Height));
                valueThumb.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected override int VisualChildrenCount { get { return visualCollection.Count; } }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            if (visualCollection.Count > index) 
                return visualCollection[index];
            return null;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class CursorHandler
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="lpPoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        ///
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            ///
            /// </summary>
            public int X;

            /// <summary>
            ///
            /// </summary>
            public int Y;

            /// <summary>
            ///
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
    }
}