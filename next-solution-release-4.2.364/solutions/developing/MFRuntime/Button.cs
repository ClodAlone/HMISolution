using System;
using System.Collections;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Shapes;

namespace MFRuntime.UI.Library
{
    // Button Class
    public class Button : UIElement
    {

        // Autosizing constructor
        public Button(string caption, Font font)
        {
            _caption = caption;
            _font = font;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Bottom;

            int textWidth;
            int textHeight;
            _font.ComputeExtent(_caption, out textWidth, out textHeight);

            _width = textWidth + _textMarginX * 2;
            _height = textHeight + _textMarginY * 2;
        }

        // Manual sizing constructor
        public Button(string caption, Font font, int width, int height)
        {
            _width = width;
            _height = height;
            _caption = caption;
            _font = font;
        }

        // Override OnRender to do our own drawing
        public override void OnRender(DrawingContext dc)
        {
            int x;
            int y;

            SolidColorBrush brush;
            Pen pen;
            Color color;
            Pen shade1;
            Pen shade2;

            // Check the pressed state and draw accordingly
            if (_pressed)
            {
                brush = _pressedBackgroundBrush;
                pen = _pressedBorderPen;
                color = _pressedForeColor;
                shade1 = _darkShade;
                shade2 = _lightShade;
            }
            else
            {
                brush = _normalBackgroundBrush;
                pen = _borderPen;
                color = _foreColor;
                shade1 = _lightShade;
                shade2 = _darkShade;
            }

            GetLayoutOffset(out x, out y);

            // Draw the base rectangle of the button
            dc.DrawRectangle(brush, pen, 1, 1, _width - 1, _height - 1);

            // Draw the caption
            string caption = _caption;
            dc.DrawText(ref caption, _font, color, 0, _textMarginY, _width, _height, _alignment, _trimming);

            // Shade the outline of the rectangle for classic button look
            dc.DrawLine(shade1, 1, 1, _width - 1, 1);
            dc.DrawLine(shade1, 1, 1, 1, _height - 1);
            dc.DrawLine(shade2, _width - 1, 1, _width - 1, _height - 1);
            dc.DrawLine(shade2, 1, _height - 1, _width - 1, _height - 1);
        }

        public event EventHandler Click;

        // Handle the stylus down event
        protected override void OnStylusDown(StylusEventArgs e)
        {

            // Flag for drawing state
            _pressed = true;

            // Capture the stylus
            Stylus.Capture(this);

            // Trigger redraw
            Invalidate();
        }

        // Handle the stylus up event
        protected override void OnStylusUp(StylusEventArgs e)
        {

            // Flag for drawing state
            _pressed = false;

            // Release the stylus
            Stylus.Capture(this, CaptureMode.None);

            // Trigger redraw
            Invalidate();

            // Fire a click event
            EventArgs args = new EventArgs();
            OnClick(args);
        }

        // Handle the stylus move....don't do anything special
        protected override void OnStylusMove(StylusEventArgs e)
        {

        }

        // Our click event
        protected virtual void OnClick(EventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }

        protected override void ArrangeOverride(int arrangeWidth, int arrangeHeight)
        {

        }

        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            desiredWidth = (availableWidth > _width) ? _width : availableWidth;
            desiredHeight = (availableHeight > _height) ? _height : availableHeight;
        }

        private SolidColorBrush _normalBackgroundBrush = new SolidColorBrush(ColorUtility.ColorFromRGB(192, 192, 192));
        private SolidColorBrush _pressedBackgroundBrush = new SolidColorBrush(ColorUtility.ColorFromRGB(128, 128, 128));

        private Pen _borderPen = new Pen(ColorUtility.ColorFromRGB(128, 128, 128));

        private Pen _pressedBorderPen = new Pen(ColorUtility.ColorFromRGB(128, 128, 128));

        private Pen _lightShade = new Pen(ColorUtility.ColorFromRGB(216, 216, 216));
        private Pen _darkShade = new Pen(ColorUtility.ColorFromRGB(64, 64, 64));

        int _width;
        int _height;
        string _caption = "";
        Font _font = null;
        Color _foreColor = ColorUtility.ColorFromRGB(0, 0, 0);
        Color _pressedForeColor = ColorUtility.ColorFromRGB(255, 255, 255);
        private TextTrimming _trimming = TextTrimming.WordEllipsis;
        private TextAlignment _alignment = TextAlignment.Center;
        protected int _textMarginX = 16;
        protected int _textMarginY = 8;
        protected bool _pressed = false;
    }
}
