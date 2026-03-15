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

namespace MFRuntime.UI
{
    //////////////////////////////////////////////////////////////////////////////

    // This class is a helper class for the scrolling text example below; it derives
    // from the TextFlow class and adds functionality that makes it easier to simply
    // pass in a string of text that contains CRLF pairs. This class breaks up the
    // lines into proper TextRuns for the TextFlow class it is based on
    internal sealed class ScrollerText : TextFlow
    {
        public ScrollerText(string text, Font font, Color color)
            : base()
        {

            int pos = 0;

            // Break the text up if it contains CR/LF pairs
            while ((pos = text.IndexOf("\r\n")) > -1)
            {
                TextRuns.Add(new TextRun(text.Substring(0, pos), font, color));
                TextRuns.Add(TextRun.EndOfLine);
                pos += 2;
                try
                {
                    text = text.Substring(pos, text.Length - pos);
                }
                catch (Exception ex)
                {
                    return;
                }
            }

            TextRuns.Add(new TextRun(text, font, color));
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    // This is the TextScrollView class; it derives from a Panel and wraps a
    // ScrollViewer object and a ScrollerText object (defined above)
    // With those 2 objects it demostrates how to create a scrollable text object
    internal sealed class TextScrollViewer : Panel
    {

        // These private values and public member give easy access to controlling
        // how large the horizontal scroll bar is
        private int _hScrollHeight = 10;
        private int _hScrollWidth = 0;
        private double _hScrollRatio = 1;
        public int HScrollHeight
        {
            get { return _hScrollHeight; }
            set
            {
                _hScrollHeight = value;
                _hScrollWidth = Width - _vScrollWidth;
                _hScrollRatio = _hScrollWidth - _vScrollWidth - _vScrollWidth;
                _hScrollRatio /= Width;
            }
        }

        // These private values and public member give easy access to controlling
        // how large the vertical scroll bar is
        private int _vScrollWidth = 10;
        private int _vScrollHeight = 0;
        private double _vScrollRatio = 1;
        public int VScrollWidth
        {
            get { return _vScrollWidth; }
            set
            {
                _vScrollWidth = value;
                _vScrollHeight = Height - _hScrollHeight;
                _vScrollRatio = _vScrollHeight - _hScrollHeight - _hScrollHeight;
                _vScrollRatio /= Height;
            }
        }

        // This private member is a standard Micro Framework Presentation object
        // The important member functions are the ones that control the scrolling
        // The TextScrollViewer class provides easy access to those scrolling
        // functions with the 4 following member functions
        private ScrollViewer _viewer;

        // Scroll one line up
        public void LineUp()
        {
            _viewer.LineUp();
        }

        // Scroll one line down
        public void LineDown()
        {
            _viewer.LineDown();
        }

        // Scroll to the left
        public void LineLeft()
        {
            _viewer.LineLeft();
        }

        // Scroll to the right
        public void LineRight()
        {
            _viewer.LineRight();
        }

        // This public member is the text object that is based on the TextFlow class
        public ScrollerText ScrollText;

        public TextScrollViewer(string text, Font font, Color color)
        {

            // Create the ScrollViewer object
            _viewer = new ScrollViewer();

            // Create the ScrollText object using the parameters passed into
            // the constructor and then set other important member values
            ScrollText = new ScrollerText(text, font, color);
            ScrollText.HorizontalAlignment = HorizontalAlignment.Left;
            ScrollText.VerticalAlignment = VerticalAlignment.Top;

            // Set the child of the viewer to be the ScrollText object
            this._viewer.Child = ScrollText;

            // Hard code a line with and height based on the character 'A'
            _viewer.LineWidth = font.CharWidth('A');
            _viewer.LineHeight = font.Height;

            // Set the child of our class to be the ScrollViewer object
            this.Children.Add(_viewer);
        }

        // Normally the presentation framework would arrange our child objects based on their visible
        // size. Since we are tyring to show scrolling beyond that size we override the ArrangeOverride
        // method to set things up to be larger than the screen
        protected override void ArrangeOverride(int arrangeWidth, int arrangeHeight)
        {
            base.ArrangeOverride(arrangeWidth, arrangeHeight);

            // Set the ScrollViewer's width and height to allow room for
            // the scroll bars
            _viewer.Width = arrangeWidth - _vScrollWidth;
            _viewer.Height = arrangeHeight - _hScrollHeight;
            _viewer.Arrange(0, 0, _viewer.Width, _viewer.Height);

            // Set the ScrollText's width and height to be twice as big as the allow area
            //ScrollText.Width = arrangeWidth * 2;
            //ScrollText.Height = arrangeHeight * 2;
            //ScrollText.UpdateLayout();
        }

        // Override the OnRender so we can manually draw the scroll bars
        //public override void OnRender(DrawingContext dc)
        //{
        //    base.OnRender(dc);

        //    // Make a brush and pen for drawing scroll bars
        //    SolidColorBrush brush = new SolidColorBrush(ColorUtility.ColorFromRGB(224, 224, 224));
        //    SolidColorBrush sliderBrush = new SolidColorBrush(ColorUtility.ColorFromRGB(64, 64, 64));
        //    Pen pen = new Pen(ColorUtility.ColorFromRGB(64, 64, 64));

        //    // Draw the horizontal scroll bar
        //    int hOffset = (int)(_viewer.HorizontalOffset * _hScrollRatio);
        //    dc.DrawRectangle(brush, pen, 0, Height - _hScrollHeight, _hScrollWidth, _hScrollHeight);
        //    dc.DrawRectangle(sliderBrush, pen, (int)(_viewer.HorizontalOffset * _hScrollRatio), Height - _hScrollHeight, _vScrollWidth, _hScrollHeight);

        //    // Draw the vertical scroll bar
        //    int vOffset = (int)(_viewer.VerticalOffset * _vScrollRatio);
        //    dc.DrawRectangle(brush, pen, Width - _vScrollWidth, 0, _vScrollWidth, _vScrollHeight);
        //    dc.DrawRectangle(sliderBrush, pen, Width - _vScrollWidth, vOffset, _vScrollWidth, _hScrollHeight);
        //}

        int _thumbHeight, _thumbWidth;

        // Override the OnRender so we can manually draw the scroll bars
        public override void OnRender(DrawingContext dc) 
        {
            base.OnRender(dc);

            // Make a brush and pen for drawing scroll bars
            SolidColorBrush brush = new 
            SolidColorBrush(ColorUtility.ColorFromRGB(224, 224, 224));
            SolidColorBrush sliderBrush = new 
            SolidColorBrush(ColorUtility.ColorFromRGB(64, 64, 64));
            Pen pen = new Pen(ColorUtility.ColorFromRGB(64, 64, 64));

            if (_viewer.ExtentHeight > _viewer.ActualHeight)
                _thumbHeight = (int)(_viewer.ActualHeight / 
                (((float)_viewer.ExtentHeight - _viewer.ActualHeight) / _viewer.LineHeight));
            else
                _thumbHeight = _viewer.ActualHeight;

            if (_viewer.ExtentWidth > _viewer.ActualWidth)
                _thumbWidth = (int)(_viewer.ActualWidth / 
                (((float)_viewer.ExtentWidth - _viewer.ActualWidth) / _viewer.LineWidth));
            else
                _thumbWidth = _viewer.ActualWidth;


            // Draw the horizontal scroll bar
            int hOffset = (int)(_viewer.HorizontalOffset * _hScrollRatio);
            dc.DrawRectangle(brush, pen, 0, Height - _hScrollHeight, Width - 
                            _hScrollWidth, _hScrollHeight);
            dc.DrawRectangle(sliderBrush, pen, System.Math.Min( hOffset / 
                            _viewer.LineWidth * _thumbWidth, Width - _hScrollWidth - _thumbWidth), 
                            Height - _hScrollHeight, _thumbWidth, _hScrollHeight);

            // Draw the vertical scroll bar
            int vOffset = (int)(_viewer.VerticalOffset * _vScrollRatio);
            dc.DrawRectangle(brush, pen, Width - _vScrollWidth, 0, _vScrollWidth, 
                            Height - _vScrollHeight);
            dc.DrawRectangle(sliderBrush, pen, Width - _vScrollWidth, 
                    System.Math.Min(vOffset / _viewer.LineHeight * _thumbHeight, Height - 
                    _hScrollHeight - _thumbHeight), _vScrollWidth,  _thumbHeight);
        }
    }
}
