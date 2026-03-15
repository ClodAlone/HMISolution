using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ScreenManager.Adorners
{
    /// <summary>
    /// An Adorner which displays animated text message. 
    /// </summary>
    public class MessageAdorner : Adorner
    {
        #region Construtor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adornedElement"></param>
        public MessageAdorner(UIElement adornedElement) : base(adornedElement) { }

        #endregion Construtor


        #region Public Methods

        /// <summary>
        /// Show a text message at the specified location
        /// </summary>
        /// <param name="message"></param>
        /// <param name="location"></param>
        public void ShowMessage(string message, Point location)
        {
            // Format the message text
            _message = new FormattedText(message, CultureInfo.InvariantCulture,
               FlowDirection.LeftToRight, MessageTypeface, 32, Brushes.White);
            Effect = new DropShadowEffect()
            {
                ShadowDepth = 2,
                BlurRadius = 0,
                Color = Colors.DarkGray
            };

            _location = location;

            // Setup an opacity animation
            var storyboard = new Storyboard();
            var opacityPropertyPath = new PropertyPath(UIElement.OpacityProperty);

            var animationOpacity = new DoubleAnimation(1d, 0d, AnimationTimeSpan);
            animationOpacity.SetValue(Storyboard.TargetPropertyProperty, opacityPropertyPath);
            storyboard.Children.Add(animationOpacity);
            BeginStoryboard(storyboard);

            // Invalidate rendering
            InvalidateVisual();
        }

        #endregion Public Methods


        #region Protected Methods

        /// <summary>
        /// The overriden rendering Method
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_message == null)
            {
                // Nothing to render
                return;
            }

            // Draw the text message
            drawingContext.DrawText(_message, _location);
        }

        #endregion Protected Methods

        #region Private Fields

        private FormattedText _message;
        private Point _location;
        private readonly TimeSpan AnimationTimeSpan = new TimeSpan(0, 0, 5);
        private readonly Typeface MessageTypeface = new Typeface(SystemFonts.MessageFontFamily, SystemFonts.MessageFontStyle,
                                                        SystemFonts.MessageFontWeight, FontStretches.Normal);

        #endregion Private Fields
    }
}
