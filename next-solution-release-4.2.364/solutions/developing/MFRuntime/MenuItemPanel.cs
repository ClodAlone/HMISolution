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


    // This is the base class of all our windows; it makes every window visible, 
    // sets the window's size to the full size of the LCD, and give the window focus
    internal class PresentationWindow : Window
    {
        protected Program _program;

        public PresentationWindow(Program program)
        {
            _program = program;

            // Make the window visible and the size of the LCD
            this.Visibility = Visibility.Visible;
            this.Width = SystemMetrics.ScreenWidth;
            this.Height = SystemMetrics.ScreenHeight;
            Buttons.Focus(this); // Set focus to this window
        }

        protected override void OnButtonDown(ButtonEventArgs e)
        {
            // Remove this window form the Window Manager
            this.Close();

            // When any button is pressed, go back to the Home page
            _program.GoHome();
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    // This control class is a container for the MenuItem controls
    // It handles which MenuItem is current and moving them back and forth
    internal sealed class MenuItemPanel : Control
    {

        // Private members
        private int _currentChild = 0;
        private int _width;
        private int _height;
        private int _animationStep;

        // This array holds the MenuItems
        public ArrayList MenuItemList;

        public MenuItemPanel(int width, int height)
        {

            // Default background color is black
            Background = new SolidColorBrush(Color.Black);

            // Width and height are passed in and set to our local members
            _width = width;
            _height = height;

            // Create the MenuItem array
            MenuItemList = new ArrayList();
        }

        // This method wraps the ArrayList Add method
        public void AddMenuItem(MenuItem menuItem)
        {
            MenuItemList.Add(menuItem);
        }

        // This property handles getting and setting the current child
        // MenuItem index
        public int CurrentChild
        {

            // Simply return the current child index
            get { return _currentChild; }

            // Setting the current child also kicks off the animition sequence
            set
            {
                if (value > _currentChild)
                    _animationStep = maxStep;        // Moving right
                else if (value < _currentChild)
                    _animationStep = -maxStep;       // Moving left
                else
                    _animationStep = 0;              // Same child, no movement

                if (value >= MenuItemList.Count)    // Handle wraping around right
                    value = 0;

                if (value < 0)                      // Handle wraping around left
                    value = MenuItemList.Count - 1;

                // Set the child and redraw to start the animation
                if (_animationStep != 0)
                {
                    _currentChild = value;
                    Invalidate();
                }
            }
        }

        // Using static and constant members allows us to easily change these
        // in one location and affect the many places that these numbers are needed
        static public int maxStep = 5;      // Number of frames in the animation
        const int xOffsetSeparation = 4;    // Distance between each MenuItem
        const int timerInterval = 20;       // Number of MS between each frame

        // Override the OnRender to do the actual drawing of the menu
        public override void OnRender(DrawingContext dc)
        {

            // Still call the base class in case this control contains other controls
            // Depending on where those controls are placed this may not be optimal
            base.OnRender(dc);

            // Calculate some initial values for positioning and drawing the MenuItems

            // This is the width of each MenuItem
            int largeX = Resources.GetBitmap(Resources.BitmapResources.Canvas_Panel_Icon).Width + xOffsetSeparation;

            // This is the starting x position
            int x = (_width / 2) - ((largeX * 2) + (largeX / 2));

            // This is the starting y position
            int y = 6;

            // This is the scaling of the current MenuItem
            int scale = 0;

            // This is the scaling offset based on the animation step
            int scaleOffset = System.Math.Abs(_animationStep);

            // adjust the x based on the animation step
            x += _animationStep * 5;

            // Iterate through the children limiting them to 2 in front and 2 behind
            // the current child. The places the current MenuItem in the middle of
            // the menu
            for (int i = _currentChild - 2; i < _currentChild + 3; i++)
            {

                // If we are on the current child
                if (i == _currentChild)
                {

                    // Scale the current child based on the current animation step value
                    // the current child is getting smaller so take the largest value
                    // (maxStep) and subtract the current scaling offset
                    scale = maxStep - scaleOffset;

                }
                else
                {

                    // If we are moving left and are drawing the child to the left or we
                    // are moving right and are drawing the child to the right then that
                    // child needs to be growing in size
                    // Else the child is drawn without any scaling
                    if ((_animationStep < 0 && i == _currentChild + 1) || (_animationStep > 0 && i == _currentChild - 1))
                        scale = scaleOffset;
                    else
                        scale = 0;
                }

                // Variable to point to the current MenuItem we want to draw
                MenuItem menuItem = null;

                // Get the correct MenuItem from the array based on the value of i
                // Because we are looking 2 left and 2 right if the current child
                // is near the beginning or end of the array we have to watch for
                // wraping around then ends
                if (i < 0)
                    menuItem = (MenuItem)MenuItemList[MenuItemList.Count + i];
                else if (i > MenuItemList.Count - 1)
                    menuItem = (MenuItem)MenuItemList[i - MenuItemList.Count];
                else
                    menuItem = (MenuItem)MenuItemList[i];

                // Have the MenuItem render itself based on the position and scaling calculated
                menuItem.Render(dc, x, y, scale);

                // Increment the x position by the size of the MenuItems
                x += largeX;
            }

            // Draw the current menuItem's text
            if (_width > 0)
            {
                // Check window size for displaying instructions
                int step = 20;
                int row = 120;
                if (_width < _height)   // Check for portrait display
                    step = 40;

                // Draw the description of the current MenuItem
                string text = ((MenuItem)MenuItemList[_currentChild]).Description;
                dc.DrawText(ref text, Program.NinaBFont, Color.White, 10, row, _width - 20, step, TextAlignment.Center, TextTrimming.None);

                // Draw the basic instructions for the menu
                text = Resources.GetString(Resources.StringResources.MenuScrolling);
                row += (step * 2);
                dc.DrawText(ref text, Program.NinaBFont, Color.White, 10, row, _width - 20, step, TextAlignment.Center, TextTrimming.None);
                text = Resources.GetString(Resources.StringResources.MenuSelection);
                row += step;
                dc.DrawText(ref text, Program.NinaBFont, Color.White, 10, row, _width - 20, step, TextAlignment.Center, TextTrimming.None);
                text = Resources.GetString(Resources.StringResources.ReturnToMenu);
                row += step;
                dc.DrawText(ref text, Program.NinaBFont, Color.White, 10, row, _width - 20, step, TextAlignment.Center, TextTrimming.None);
            }

            // Start the animation timer
            // This gets called every time the menu is rendered and it will
            // handle decrementing the _animationStep member and stopping the timer
            // when _animationStep reaches 0
            StartAnimationTimer();
        }

        // Private timer member and public accessor function
        private DispatcherTimer _animationTimer;
        private void StartAnimationTimer()
        {

            // Only start the timer if _animationStep is not 0
            if (_animationStep != 0)
            {

                // The first time through we will create the timer
                if (_animationTimer == null)
                {
                    _animationTimer = new DispatcherTimer(this.Dispatcher);
                    _animationTimer.Interval = new TimeSpan(0, 0, 0, 0, timerInterval);
                    _animationTimer.Tick += new EventHandler(OnAnimationTimer);
                }

                // Keep track of when we started the timer to deal with missing
                // frames because of a slow processor or being in the emulator
                _lastTick = DateTime.Now.Ticks;

                // Start the timer
                _animationTimer.Start();
            }
        }

        // Private member to keep track of when the timer was started
        // so we can detect missing frames on slow processors and the
        // emulator
        long _lastTick = 0;

        // Timer method to handle the actual timer ticks
        private void OnAnimationTimer(object o, EventArgs e)
        {

            // Stop the timer while we process this frame
            _animationTimer.Stop();

            // Figure out how much time has gone by since the timer was started
            long ms = ((DateTime.Now.Ticks - _lastTick) / 10000);

            // Set the last tick to now
            _lastTick = DateTime.Now.Ticks;

            // Figure out how many frames should have been displayed by now
            int increment = (int)(ms / timerInterval);

            // If the timer is being serviced in less time than the minimum
            // then we are ok to just process the frame
            // Else If we have gone beyond the maxStep then just move the frame
            // to that one
            if (increment < 1)
                increment = 1;
            else if (increment > maxStep)
                increment = maxStep;

            // Increment _animationStep based on which direction we are going
            if (_animationStep < 0)
                _animationStep += increment;
            else if (_animationStep > 0)
                _animationStep -= increment;

            // This will trigger another OnRender and kick the timer off again
            // to take the next step in the animation
            Invalidate();
        }

        // Override MeasureOverride if you want to hard code the size of your control
        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            desiredWidth = _width;
            desiredHeight = _height;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    // This control class is the MenuItem that handles drawing and actual menu item
    internal sealed class MenuItem : Control
    {

        // Private members
        private Bitmap _imageSmall;   // Small image because you can't stretch and image smaller, only larger
        private Bitmap _image;        // Larger version so it looks good instead of stretching the small one
        private string _description;  // Description of this MenuItem
        private int[] _widthSteps;    // Array of widths so we save time by pre-calculating them
        private int[] _heightSteps;   // Array of heights so we save time by pre-calculating them
        private int _largeWidth;      // Width of large image
        private int _largeHeight;     // Height of large image

        public MenuItem()
        {
        }

        // This is the constructor that we want to use so we can get all the pieces in one go
        public MenuItem(Resources.BitmapResources rBitmapSmall, Resources.BitmapResources rBitmap, string description, Resources.BitmapResources rLargeSizeBitmap)
        {

            // Get the images from the resource manager
            _imageSmall = Resources.GetBitmap(rBitmapSmall);
            _image = Resources.GetBitmap(rBitmap);

            // Set the description
            _description = description;

            // Create the step arrays for zooming in and out
            _widthSteps = new int[MenuItemPanel.maxStep];
            _heightSteps = new int[MenuItemPanel.maxStep];

            // Get the difference in size between the large and small images
            int wDiff = _image.Width - _imageSmall.Width;
            int hDiff = _image.Height - _imageSmall.Height;

            // Pre-calculate the width and height values for scaling the image
            for (int i = 1; i < MenuItemPanel.maxStep; i++)
            {
                _widthSteps[i] = (wDiff / MenuItemPanel.maxStep) * i;
                _heightSteps[i] = (hDiff / MenuItemPanel.maxStep) * i;
            }

            // Set the large width and height based on one of the main icons
            Bitmap bmp = Resources.GetBitmap(rLargeSizeBitmap);
            _largeWidth = bmp.Width;
            _largeHeight = bmp.Height;
        }

        // Public accessor method for the description
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        // We are not overriding the OnRender method of the class, but are simply creating
        // a render method that can be called by the menu container
        public void Render(DrawingContext dc, int x, int y, int scale)
        {

            // Make sure we have all of the proper images
            if (_image != null && _imageSmall != null)
            {

                // If the scale is at maxStep then just use the larger image so it looks nice
                // Else use the scale value to scale from the smaller image to something bigger
                if (scale == MenuItemPanel.maxStep)
                {
                    Width = _image.Width;
                    Height = _image.Height;
                    dc.DrawImage(_image, x, y);
                }
                else
                {
                    // If the scale is 0 then just draw the small bitmap
                    // Else calculate the difference between the small and large bitmaps
                    // and stretch the small bitmap
                    if (scale == 0)
                    {
                        Width = _imageSmall.Width;
                        Height = _imageSmall.Height;
                        x += ((_largeWidth - Width) / 2);
                        y += ((_largeHeight - Height) / 2);
                        dc.DrawImage(_imageSmall, x, y);
                    }
                    else
                    {
                        int wDiff = _image.Width - _imageSmall.Width;
                        int hDiff = _image.Height - _imageSmall.Height;

                        Width = _imageSmall.Width + _widthSteps[scale];
                        Height = _imageSmall.Height + _heightSteps[scale];
                        x += ((_largeWidth - Width) / 2);
                        y += ((_largeHeight - Height) / 2);
                        dc.Bitmap.StretchImage(x, y, _imageSmall, Width, Height, 255);
                    }
                }
            }
        }
    }
    //////////////////////////////////////////////////////////////////////////////
}
