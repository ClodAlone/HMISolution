using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using WPFUtilities;

namespace ScreenManager.Adorners
{
    class SelectionAdorner : Adorner
    {
        #region Class variables

        /// <summary>
        /// Used to store the start point.
        /// </summary>
        private Point? startposition;

        /// <summary>
        /// Used to store the start point.
        /// </summary>
        private Point? endposition;

        /// <summary>
        /// Used to store the pen.
        /// </summary>
        private Pen selectionPen;

        /// <summary>
        /// Used to store the Page instance..
        /// </summary>
        private ScreenEditorView Editor;

        #endregion

        public SelectionAdorner(ScreenEditorView editor, Point? dragStartPoint)
            : base(editor.MainSurface)
        {
            Editor = editor;
            startposition = dragStartPoint;
            selectionPen = new Pen(Brushes.Black, 1);
            selectionPen.DashStyle = DashStyles.Dot;
        }


        #region Overrides

        /// <summary>
        /// Provides class handling for the MouseMove routed event that occurs when the mouse 
        /// pointer  is over this control.
        /// </summary>
        /// <param name="e">The MouseEventArgs</param>
        DelayedSingleActionInvoker sizeChangedInvoker;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ////Get the current position as end position.
                endposition = e.GetPosition(Editor.MainSurface);

                if (!IsMouseCaptured)
                {
                    CaptureMouse();
                }

                if (sizeChangedInvoker == null)
                    sizeChangedInvoker = new DelayedSingleActionInvoker(
                        new Action(() =>
                        {
                            Editor.InvalidateSelection(startposition, endposition);
                            Editor.UpdateCurrentSelectionContext();

                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                        }), TimeSpan.FromMilliseconds(100));
                sizeChangedInvoker.BeginInvoke();
                InvalidateVisual();
            }
            else
            {
                if (IsMouseCaptured)
                {
                    ReleaseMouseCapture();
                }
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(Editor.MainSurface);
            if (adornerLayer != null)
                adornerLayer.Remove(this);

            endposition = e.GetPosition(Editor.MainSurface);

            Editor.UpdateCurrentSelectionContext(startposition, endposition);

            e.Handled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
            if (startposition.HasValue && endposition.HasValue)
            {
                Rect rect = new Rect(startposition.Value, endposition.Value);
                dc.DrawRectangle(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22999999")), selectionPen, rect);
            }
        }

        #endregion
    }
}
