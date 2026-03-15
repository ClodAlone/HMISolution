using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.IO;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using WPFUtilities;
using Utilities;
using Utilities.WPF;
using System.Windows.Data;
using ScreenSettings;

namespace ScreenManager.Adorners
{
    class GenericElementAdorner : BasicAdorner
    {
        RotateThumb rotateTopLeft;
        RotateThumb rotateBottomLeft;
        RotateThumb rotateTopRight;
        RotateThumb rotateBottomRight;
        ResizeThumb resizeTopLeft;
        ResizeThumb resizeBottomLeft;
        ResizeThumb resizeTopRight;
        ResizeThumb resizeBottomRight;
        ResizeThumb resizeLeft;
        ResizeThumb resizeRight;
        ResizeThumb resizeTop;
        ResizeThumb resizeBottom;

        TextBox glyph;
        ContentControl contentglyph;

        bool isRotationAdorner;
        GenericElementAdorner rotationAdorner;

        TransformOriginThumb TransformOrigin;

        public GenericElementAdorner(UIElement parent, UIElement element, bool canMove, IGridViewInfoService gridViewInfoService, 
            bool bRotationAdorner = false)
            : base(parent, element, canMove, gridViewInfoService)
        {
            isRotationAdorner = bRotationAdorner;

            if (isRotationAdorner)
            {
                rotateTopLeft = new RotateThumb(GridViewInfoService);
                rotateBottomLeft = new RotateThumb(GridViewInfoService);
                rotateTopRight = new RotateThumb(GridViewInfoService);
                rotateBottomRight = new RotateThumb(GridViewInfoService);

                rotateTopLeft.Visibility = Visibility.Collapsed;
                rotateBottomLeft.Visibility = Visibility.Collapsed;
                rotateTopRight.Visibility = Visibility.Collapsed;
                rotateBottomRight.Visibility = Visibility.Collapsed;

                rotateTopLeft.VerticalAlignment = VerticalAlignment.Top;
                rotateTopLeft.HorizontalAlignment = HorizontalAlignment.Left;
                rotateBottomLeft.VerticalAlignment = VerticalAlignment.Bottom;
                rotateBottomLeft.HorizontalAlignment = HorizontalAlignment.Left;
                rotateTopRight.VerticalAlignment = VerticalAlignment.Top;
                rotateTopRight.HorizontalAlignment = HorizontalAlignment.Right;
                rotateBottomRight.VerticalAlignment = VerticalAlignment.Bottom;
                rotateBottomRight.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else 
            {
                ShowHideRotationThumbs();

                resizeTopLeft = new ResizeThumb(parent, GridViewInfoService);
                resizeBottomLeft = new ResizeThumb(parent, GridViewInfoService);
                resizeTopRight = new ResizeThumb(parent, GridViewInfoService);
                resizeBottomRight = new ResizeThumb(parent, GridViewInfoService);
                resizeLeft = new ResizeThumb(parent, GridViewInfoService);
                resizeRight = new ResizeThumb(parent, GridViewInfoService);
                resizeTop = new ResizeThumb(parent, GridViewInfoService);
                resizeBottom = new ResizeThumb(parent, GridViewInfoService);

                resizeTopLeft.Cursor = Cursors.SizeNWSE;
                resizeTopLeft.VerticalAlignment = VerticalAlignment.Top;
                resizeTopLeft.HorizontalAlignment = HorizontalAlignment.Left;
                resizeBottomLeft.Cursor = Cursors.SizeNESW;
                resizeBottomLeft.VerticalAlignment = VerticalAlignment.Bottom;
                resizeBottomLeft.HorizontalAlignment = HorizontalAlignment.Left;
                resizeTopRight.Cursor = Cursors.SizeNESW;
                resizeTopRight.VerticalAlignment = VerticalAlignment.Top;
                resizeTopRight.HorizontalAlignment = HorizontalAlignment.Right;
                resizeBottomRight.Cursor = Cursors.SizeNWSE;
                resizeBottomRight.VerticalAlignment = VerticalAlignment.Bottom;
                resizeBottomRight.HorizontalAlignment = HorizontalAlignment.Right;
                resizeLeft.Cursor = Cursors.SizeWE;
                resizeLeft.HorizontalAlignment = HorizontalAlignment.Left;
                resizeLeft.VerticalAlignment = VerticalAlignment.Center;
                resizeRight.Cursor = Cursors.SizeWE;
                resizeRight.HorizontalAlignment = HorizontalAlignment.Right;
                resizeRight.VerticalAlignment = VerticalAlignment.Center;
                resizeTop.Cursor = Cursors.SizeNS;
                resizeTop.HorizontalAlignment = HorizontalAlignment.Center;
                resizeTop.VerticalAlignment = VerticalAlignment.Top;
                resizeBottom.Cursor = Cursors.SizeNS;
                resizeBottom.HorizontalAlignment = HorizontalAlignment.Center;
                resizeBottom.VerticalAlignment = VerticalAlignment.Bottom;

                contentglyph = new ContentControl();
                contentglyph.HorizontalAlignment = HorizontalAlignment.Stretch;
                contentglyph.VerticalAlignment = VerticalAlignment.Stretch;

                glyph = new TextBox();
                glyph.AcceptsReturn = true;
                glyph.Visibility = System.Windows.Visibility.Collapsed;
                glyph.BorderThickness = new Thickness(0);
                glyph.Margin = new Thickness(4);
                glyph.PreviewKeyDown += (o, e) =>
                {
                    var control = adornedElement as Control;
                    if (control != null && glyph.Visibility == System.Windows.Visibility.Visible &&
                        (e.Key == Key.Escape ||
                        e.Key == Key.Enter))
                    {
                        AcceptTextEditChanges(e.Key != Key.Enter);
                        e.Handled = true;
                    }
                };
                glyph.PreviewLostKeyboardFocus += (o, e) =>
                {
                    var control = adornedElement as Control;
                    if (control != null && glyph.Visibility == System.Windows.Visibility.Visible)
                    {
                        AcceptTextEditChanges(false);
                        e.Handled = true;
                    }
                };

                contentglyph.Content = glyph;

                TransformOrigin = new TransformOriginThumb();
                TransformOrigin.Visibility = Visibility.Collapsed;
            }

            dragControl.DataContext = element;

            if (!isRotationAdorner)
                Activate();
        }

        PropertyChangeNotifier notifier; 
        public override void Activate()
        {
            if (!isRotationAdorner && TransformOrigin != null)
            {
                DependencyPropertyDescriptor propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsManipulationEnabledProperty, typeof(UIElement));
                if (notifier == null)
                {
                    notifier = new PropertyChangeNotifier(adornedElement, propDesc.Name);
                    notifier.ValueChanged += (o, e) =>
                        {
                            if (!adornedElement.IsManipulationEnabled)
                                TransformOrigin.Visibility = Visibility.Visible;
                            else
                            {
                                TransformOrigin.Visibility = Visibility.Collapsed;
                                adornedElement.RenderTransformOrigin = new Point(0.5, 0.5);
                            }
                        };
                }
                if (adornedElement.IsManipulationEnabled)
                {
                    TransformOrigin.Visibility = Visibility.Collapsed;
                    adornedElement.RenderTransformOrigin = new Point(0.5, 0.5);
                }
            }

            if (adornedElement.ReadLocalValue(RenderTransformOriginProperty) == DependencyProperty.UnsetValue)
                adornedElement.RenderTransformOrigin = new Point(0.5, 0.5);

            if (!isRotationAdorner)
                base.Activate();

            if (isRotationAdorner)
            {
                visualChildren.Add(rotateTopLeft);
                visualChildren.Add(rotateBottomLeft);
                visualChildren.Add(rotateTopRight);
                visualChildren.Add(rotateBottomRight);
            }
            else if (bCanMove)
            {
                visualChildren.Add(resizeTopLeft);
                visualChildren.Add(resizeBottomLeft);
                visualChildren.Add(resizeTopRight);
                visualChildren.Add(resizeBottomRight);
                visualChildren.Add(resizeLeft);
                visualChildren.Add(resizeRight);
                visualChildren.Add(resizeTop);
                visualChildren.Add(resizeBottom);
                resizeTopLeft.IsEnabled = bCanMove;
                resizeBottomLeft.IsEnabled = bCanMove;
                resizeTopRight.IsEnabled = bCanMove;
                resizeBottomRight.IsEnabled = bCanMove;
                resizeLeft.IsEnabled = bCanMove;
                resizeRight.IsEnabled = bCanMove;
                resizeTop.IsEnabled = bCanMove;
                resizeBottom.IsEnabled = bCanMove;

                if (TransformOrigin != null)
                    visualChildren.Add(TransformOrigin);
            }

            if (contentglyph != null)
                visualChildren.Add(contentglyph);

            foreach (var c in visualChildren)
            {
                if (c is Thumb)
                {
                    Thumb control = c as Thumb;
                    control.DataContext = adornedElement;
                    if (control is TransformOriginThumb)
                    {
                        control.DragStarted += pointDragStartedTransformOrigin;
                        control.DragDelta += pointDragDeltaTransformOrigin;
                        control.DragCompleted += pointDragCompletedTransformOrigin;
                    }
                    else
                    {
                        control.DragStarted += genericcontrol_DragStarted;
                        control.DragCompleted += control_DragCompleted;
                        control.DragDelta += control_DragDelta;
                        // if (control.Cursor == Cursors.SizeAll) control.RenderTransform = AdornedElement.RenderTransform;
                    }
                }
            }
        }
        protected void genericcontrol_DragStarted(object sender, DragStartedEventArgs e)
        {
            AcceptTextEditChanges(false);
            base.control_DragStarted(sender, e);
        }
        internal override void ShowHideRotationThumbs()
        {
            base.ShowHideRotationThumbs();
            if (!isRotationAdorner)
            {
                if (rotationAdorner == null)
                {
                    rotationAdorner = new GenericElementAdorner(adornedElement, adornedElement,
                                                        false, GridViewInfoService, true);
                    var adorner = AdornerLayer.GetAdornerLayer(adornedElement);
                    if (adorner != null)
                        adorner.Add(rotationAdorner);
                    rotationAdorner.Activate();
                    rotationAdorner.changed += adorner_changed;
                    rotationAdorner.changing += adorner_changing;
                }

                rotationAdorner.ShowHideRotationThumbs();
            }
        }

        void adorner_changed(object sender, EventArgs e)
        {
            OnFireChanged(this);
        }

        void adorner_changing(object sender, EventArgs e)
        {
            OnFireChanging(this);
        }

        public override void Deactivate()
        {
            AcceptTextEditChanges();

            if (notifier != null)
            {
                notifier.Dispose();
                notifier = null;
            }

            if (rotationAdorner != null)
            {
                var adorner = AdornerLayer.GetAdornerLayer(adornedElement);
                if (adorner != null)
                    adorner.Remove(rotationAdorner);
                rotationAdorner.changing -= adorner_changing;
                rotationAdorner.changed -= adorner_changed;
                rotationAdorner.Deactivate();
                rotationAdorner = null;
            }

            foreach (var c in visualChildren)
            {
                if (c is Thumb)
                {
                    Thumb control = c as Thumb;
                    control.DataContext = adornedElement;
                    if (control is TransformOriginThumb)
                    {
                        control.DragStarted -= pointDragStartedTransformOrigin;
                        control.DragDelta -= pointDragDeltaTransformOrigin;
                        control.DragCompleted -= pointDragCompletedTransformOrigin;
                    }
                    else
                    {
                        control.DragStarted -= genericcontrol_DragStarted;
                        control.DragCompleted -= control_DragCompleted;
                        control.DragDelta -= control_DragDelta;
                    }
                }
            }

            base.Deactivate();
        }

        protected override void SetThumbVisibility(bool bVisible)
        {
            if (isRotationAdorner)
            {
                rotateTopLeft.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                rotateBottomLeft.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                rotateTopRight.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                rotateBottomRight.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                resizeTopLeft.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                resizeBottomLeft.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                resizeTopRight.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                resizeBottomRight.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                resizeLeft.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                resizeRight.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                resizeTop.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
                resizeBottom.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void pointDragStartedTransformOrigin(object sender, DragStartedEventArgs e)
        {
            // adornedElement.RenderTransform = new TransformGroup(); // reset the center
            bChanged = false;
        }

        void pointDragDeltaTransformOrigin(object sender, DragDeltaEventArgs e)
        {
            OnFireChanging(this);
            bEnableFireChanging = false;
            bChanged = true;

            Point currentPoint = Mouse.GetPosition(AdornedElement);
            Point ptOrigin = new Point();

            double dWidth = ElementWidth;
            double dHeight = ElementHeight;
            currentPoint.X -= ElementLeft;
            currentPoint.Y -= ElementTop;

            if (dHeight == 0.0)
                ptOrigin.Y = 0;
            else
                ptOrigin.Y = currentPoint.Y / dHeight;

            if (dWidth == 0.0)
                ptOrigin.X = 0;
            else
                ptOrigin.X = currentPoint.X / dWidth;

            adornedElement.RenderTransformOrigin = ptOrigin;

            InvalidateArrange();
        }

        void pointDragCompletedTransformOrigin(object sender, DragCompletedEventArgs e)
        {
            if (bChanged)
                OnFireChanged(this);
            bEnableFireChanging = true;
        }

        UIElement GetInnerElement()
        {
            UIElement screenElement = adornedElement;
            if (adornedElement != null && Utilities.WPF.XmlHelper.IsProblematicXamlWriter(adornedElement) && (!(adornedElement is ContentControl) || !((adornedElement as ContentControl).Content is String) || adornedElement.Visibility != Visibility.Visible))
                screenElement = (from c in adornedElement.GetVisualChildrenOfType<ContentControl>()
                                 where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
                                 select c).FirstOrDefault();
            return screenElement;
        }

        void AcceptTextEditChanges(bool bCancel = false)
        {
            var control = adornedElement as Control;
            if (control == null || glyph == null || glyph.Visibility != System.Windows.Visibility.Visible)
                return;

            glyph.Visibility = System.Windows.Visibility.Collapsed;
            //editControl.Visibility = System.Windows.Visibility.Visible;

            if (!bCancel)
                lastTextEdit = glyph.Text;

            lastTextEdit = lastTextEdit.Replace("\\n", System.Environment.NewLine);

            Type t = control.GetType();
            var p = t.GetProperty("Text");
            if (p != null)
            {
                p.SetValue(control, lastTextEdit);
            }
            else if (control is ContentControl && (control as ContentControl).Content is String)
            {
                (control as ContentControl).Content = lastTextEdit;
            }
            else if (!(control is HeaderedContentControl) &&
                    control is ContentControl && !((control as ContentControl).Content is String))
            {
                var currentList = (from c in control.GetVisualChildrenOfType<ContentControl>()
                                   where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
                                   select c).ToList();
                if (currentList.Count == 0)
                    return;
                currentList[0].Content = lastTextEdit;
            }
            else if (control is HeaderedContentControl &&
                ((control as HeaderedContentControl).Header is String ||
                 (control as HeaderedContentControl).Header == null))
            {
                (control as HeaderedContentControl).Header = lastTextEdit;
            }

            if (!bCancel)
                OnFireChanged(this, new AdornerOperationEventArgs(GetInnerElement(), AdornerOperationEventArgs.AdornerOperation.TextChanged));
        }

        String lastTextEdit;
        void DisplayTextEditControl()
        {
            // Data bind the glyph's vertical and horizontal alignment 
            // to the target control's alignment properties.
            var control = adornedElement as Control;
            if (glyph.Visibility == System.Windows.Visibility.Visible || 
                control == null || control is ComboBox || control is ListBox ||
                HasPopupOpened())
                return;

            var document = ScreenDocument.GetScreenDocument(adornedElement)?.ActiveView as ScreenEditorView;
            if (document != null)
                document.RestoreUntranslated(GetInnerElement() as FrameworkElement);

            glyph.Background = control.Background;
            glyph.Foreground = control.Foreground;
            glyph.FontFamily = control.FontFamily;
            glyph.FontSize = control.FontSize;
            glyph.FontStretch = control.FontStretch;
            glyph.FontStyle = control.FontStyle;
            glyph.FontWeight = control.FontWeight;

            Type t = control.GetType();
            var p = t.GetProperty("Text");
            if (p != null)
            {
                var cv = p.GetValue(control) as String;
                lastTextEdit = cv != null ? cv : String.Empty;
                glyph.Text = lastTextEdit;
                OnFireChanging(this);
                p.SetValue(control, "");
            }
            else if (control is ContentControl && (control as ContentControl).Content is String)
            {
                lastTextEdit = (control as ContentControl).Content as String;
                glyph.Text = lastTextEdit;
                OnFireChanging(this);
                (control as ContentControl).Content = String.Empty;
            }
            else if (!(control is HeaderedContentControl) &&
                    control is ContentControl && !((control as ContentControl).Content is String))
            {
                var currentList = (from c in control.GetVisualChildrenOfType<ContentControl>()
                                   where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
                                   select c).ToList();
                if (currentList.Count != 1)
                    return;
                lastTextEdit = currentList[0].Content as String;
                glyph.Text = lastTextEdit;
                glyph.Foreground = currentList[0].Foreground;
                glyph.FontFamily = currentList[0].FontFamily;
                glyph.FontSize = currentList[0].FontSize;
                glyph.FontStretch = currentList[0].FontStretch;
                glyph.FontStyle = currentList[0].FontStyle;
                glyph.FontWeight = currentList[0].FontWeight;
                OnFireChanging(this);
                currentList[0].Content = String.Empty;
            }
            else if (control is HeaderedContentControl &&
                ((control as HeaderedContentControl).Header is String ||
                 (control as HeaderedContentControl).Header == null))
            {
                lastTextEdit = (control as HeaderedContentControl).Header as String;
                glyph.Text = lastTextEdit != null ? lastTextEdit : String.Empty;
                OnFireChanging(this);
                (control as HeaderedContentControl).Header = String.Empty;
            }
            else
                return;

            glyph.HorizontalAlignment = control.HorizontalAlignment;
            if (glyph.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
                glyph.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            glyph.VerticalAlignment = control.VerticalAlignment;
            if (glyph.VerticalAlignment == System.Windows.VerticalAlignment.Stretch)
                glyph.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            if (control.ActualWidth > 10)
                glyph.MaxWidth = control.ActualWidth - 10;
            else
                glyph.MaxWidth = control.ActualWidth;
            if (control.ActualHeight > 10)
                glyph.MaxHeight = control.ActualHeight - 10;
            else
                glyph.MaxHeight = control.ActualHeight;

            glyph.Visibility = System.Windows.Visibility.Visible;
            adornedElement.Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                glyph.Focusable = true;
                glyph.Focus();
                glyph.SelectAll();
                glyph.ScrollToEnd();
                if (editControl != null)
                    editControl.Visibility = System.Windows.Visibility.Collapsed;
            });

            // OnFireChanging(this);
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftAlt) && 
                !Keyboard.IsKeyDown(Key.RightAlt) &&
                !Keyboard.IsKeyDown(Key.LeftCtrl) &&
                !Keyboard.IsKeyDown(Key.RightCtrl) &&
                !Keyboard.IsKeyDown(Key.LeftShift) &&
                !Keyboard.IsKeyDown(Key.RightShift))
                DisplayTextEditControl();
            base.OnPreviewKeyDown(e);
        }

        Point lastMouseDown;
        protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource == dragControl)
            {
                var point = Mouse.GetPosition(FindParentCanvas(adornedElement));
                if (lastMouseDown == point)
                    DisplayTextEditControl();
            }

            base.OnPreviewMouseLeftButtonUp(e);
        }

        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            lastMouseDown = Mouse.GetPosition(FindParentCanvas(adornedElement));
            base.OnPreviewMouseLeftButtonDown(e);
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            Matrix m = new Matrix();
            m.OffsetX = ((MatrixTransform)transform).Matrix.OffsetX;
            m.OffsetY = ((MatrixTransform)transform).Matrix.OffsetY;

            return transform;//new MatrixTransform(m); //this code neded for right manipulators zooming
        }

        Pen pen = new Pen(Brushes.Blue, 0.2d);
        Pen penSnapped = new Pen(Brushes.Red, 1d);
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (currentThumb == null || currentThumb is RotateThumb)
                return;

            double dWidth = ElementWidth;
            double dHeight = ElementHeight;

            double top = double.IsNaN(Canvas.GetTop(adornedElement)) ? 0 : Canvas.GetTop(adornedElement);
            double left = double.IsNaN(Canvas.GetLeft(adornedElement)) ? 0 : Canvas.GetLeft(adornedElement);

            switch (currentThumb.VerticalAlignment)
            {
                // case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                case VerticalAlignment.Top:
                    dc.DrawLine(GridViewInfoService.IsPointOnSnapLineVertical(top) ? penSnapped : pen, 
                        new Point(-100000, top), new Point(100000, top));
                    FormattedText ftextTop = new FormattedText(
                        top.ToString("F"),
                        CultureInfo.CurrentUICulture,
                        System.Windows.FlowDirection.LeftToRight,
                        new Typeface("TimesNewRoman"),
                        8, Brushes.Blue);
                    dc.DrawRectangle(Brushes.White, pen, 
                        new Rect(new Point(left + 3, top -(ftextTop.Height) - 5), new Size(ftextTop.Width + 2, ftextTop.Height + 2)));
                    dc.DrawText(ftextTop, new Point(left + 4, top -(ftextTop.Height + 4)));
                    break;
                case VerticalAlignment.Bottom:
                    dc.DrawLine(GridViewInfoService.IsPointOnSnapLineVertical(top + dHeight) ? penSnapped : pen, 
                        new Point(-100000, top + dHeight), new Point(10000, top + dHeight));
                    FormattedText ftextHeight = new FormattedText(
                        dHeight.ToString("F"),
                        CultureInfo.CurrentUICulture,
                        System.Windows.FlowDirection.LeftToRight,
                        new Typeface("TimesNewRoman"),
                        8, Brushes.Blue);
                    dc.DrawRectangle(Brushes.White, pen,
                        new Rect(new Point(left + dWidth + 3, top + ((dHeight - ftextHeight.Height) / 2) - 1), new Size(ftextHeight.Width + 2, ftextHeight.Height + 2)));
                    dc.DrawText(ftextHeight, new Point(left + dWidth + 4, top + ((dHeight - ftextHeight.Height) / 2)));
                    break;
            }
            switch (currentThumb.HorizontalAlignment)
            {
                // case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                case HorizontalAlignment.Left:
                    dc.DrawLine(GridViewInfoService.IsPointOnSnapLineHorizontal(left) ? penSnapped : pen, 
                        new Point(left, -100000), new Point(left, 100000));
                    FormattedText ftextLeft = new FormattedText(
                        left.ToString("F"),
                        CultureInfo.CurrentUICulture,
                        System.Windows.FlowDirection.LeftToRight,
                        new Typeface("TimesNewRoman"),
                        8, Brushes.Blue);
                    dc.DrawRectangle(Brushes.White, pen,
                        new Rect(new Point(left -(ftextLeft.Width + 0) - 4, top + 2), new Size(ftextLeft.Width + 2, ftextLeft.Height + 2)));
                    dc.DrawText(ftextLeft, new Point(left -(ftextLeft.Width + 4), top + 4));
                    break;
                case HorizontalAlignment.Right:
                    dc.DrawLine(GridViewInfoService.IsPointOnSnapLineHorizontal(left + dWidth) ? penSnapped : pen, 
                        new Point(left + dWidth, -100000), new Point(left + dWidth, 100000));
                    FormattedText ftextWidth = new FormattedText(
                        dWidth.ToString("F"),
                        CultureInfo.CurrentUICulture,
                        System.Windows.FlowDirection.LeftToRight,
                        new Typeface("TimesNewRoman"),
                        8, Brushes.Blue);
                    dc.DrawRectangle(Brushes.White, pen,
                        new Rect(new Point(left + ((dWidth - ftextWidth.Width) / 2) - 2, top + dHeight + 2), new Size(ftextWidth.Width + 2, ftextWidth.Height + 2)));
                    dc.DrawText(ftextWidth, new Point(left + ((dWidth - ftextWidth.Width) / 2), top + dHeight + 4));
                    break;
            }

            //RotateTransform rt = new RotateTransform(270);
            //rt.CenterX = ftextHeight.Width / 2;
            //rt.CenterY = ftextHeight.Height / 2;
            //dc.PushTransform(rt);
            //dc.DrawText(ftextHeight, new Point(-(ftextHeight.Width * 2), dWidth - (ftextHeight.Height / 2)));
            //dc.Pop();
        }
    }

    public class AdornerOperationEventArgs : EventArgs
    {
        public enum AdornerOperation
        {
            None,
            TextChanged
        }
        public AdornerOperation Operation;
        public UIElement uiElement;
        public AdornerOperationEventArgs(UIElement el, AdornerOperation o = AdornerOperation.None)
        {
            Operation = o;
            uiElement = el;
        }
    }
}
