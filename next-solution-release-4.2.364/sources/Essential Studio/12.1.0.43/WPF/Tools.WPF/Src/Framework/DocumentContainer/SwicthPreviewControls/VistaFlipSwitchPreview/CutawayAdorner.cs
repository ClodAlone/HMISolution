// <copyright file="CutawayAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents control for to show header of vista flip items.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CutawayAdorner : TemplatedAdornerBase
    {
        #region Constants
        /// <summary>
        /// Presents FACTOR_OF_HEIGHT
        /// </summary>
        private const double FACTOR_OF_HEIGHT = 0.875;
        
        /// <summary>
        /// Presents TEXT_CUTAWAY_NAME
        /// </summary>
        private const string TEXT_CUTAWAY_NAME = "PART_TextCutAway";
        #endregion

        #region Private members
        /// <summary>
        /// Presents AnimationDuration
        /// </summary>
        private readonly Duration m_AnimationDuration;
        
        /// <summary>
        /// Presents TextBlock
        /// </summary>
        private TextBlock m_textBlock;
        
        /// <summary>
        /// Presents Text
        /// </summary>
        private string m_text = string.Empty;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="CutawayAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="duration">The duration.</param>
        public CutawayAdorner(UIElement adornedElement, Duration duration)
            : base(adornedElement)
        {
            ////TimeSpan timeSpan = new TimeSpan((int) duration.TimeSpan.Ticks/2);
            ////m_AnimationDuration = new Duration( timeSpan );
            m_AnimationDuration = duration;
            Loaded += new RoutedEventHandler(OnCutawayAdornerLoaded);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the factor of height.
        /// </summary>
        /// <value>The height of the factor of.</value>
        private double FactorOfHeight
        {
            get
            {
                return (double)GetValue(FactorOfHeightProperty);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="text">The text m_textBlock.</param>
        public void SetText(string text)
        {
            if (null != m_textBlock)
            {
                m_textBlock.Text = text;
            }

            m_text = text;
        }
        
        /// <summary>
        /// Stars the animation.
        /// </summary>
        public void StarAnimation()
        {
            double factor = GetFactor();
            StartCutAwayAnimation(factor, FACTOR_OF_HEIGHT);
        }
        
        /// <summary>
        /// Exits the animation.
        /// </summary>
        public void ExitAnimation()
        {
            double factor = GetFactor();
            StartCutAwayAnimation(FACTOR_OF_HEIGHT, factor);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Arranges inner control to the full size.
        /// </summary>
        /// <param name="finalSize">The final area that
        /// this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Control internalcontrol = InnerControl;
            Size size = AdornedElement.RenderSize;
            Point startPoint = new Point((size.Width - internalcontrol.DesiredSize.Width) / 2, FactorOfHeight * size.Height);
            internalcontrol.Arrange(new Rect(startPoint, internalcontrol.DesiredSize));
            return internalcontrol.RenderSize;
        }

        /// <summary>
        /// Called when [cutaway adorner loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCutawayAdornerLoaded(object sender, RoutedEventArgs e)
        {
            m_textBlock = (TextBlock)InnerControl.Template.FindName(TEXT_CUTAWAY_NAME, InnerControl);

            if (null == m_textBlock)
            {
                throw new NotImplementedException("Incorrect template.");
            }

            m_textBlock.Text = m_text;
        }
        
        /// <summary>
        /// Gets the factor.
        /// </summary>
        /// <returns> double value type</returns>
        private double GetFactor()
        {
            double height = AdornedElement.RenderSize.Height;
            return 0 != height ? (height - ActualHeight) / height : 0.9;
        }
        
        /// <summary>
        /// Starts the cutaway animation.
        /// </summary>
        /// <param name="from">From StartCutAwayAnimation.</param>
        /// <param name="to">To StartCutAwayAnimation.</param>
        private void StartCutAwayAnimation(double from, double to)
        {
            DoubleAnimation startAnimation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = m_AnimationDuration
            };

            Timeline.SetDesiredFrameRate(startAnimation, null);
            BeginAnimation(FactorOfHeightProperty, startAnimation);
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// presents factor of height for "show" animation.
        /// </summary>
        public static readonly DependencyProperty FactorOfHeightProperty = DependencyProperty.Register("FactorOfHeight", typeof(double), typeof(CutawayAdorner), new FrameworkPropertyMetadata(FACTOR_OF_HEIGHT, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion
    }
}