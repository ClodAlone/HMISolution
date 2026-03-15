// <copyright file="DocumentContainer_MDI.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DocumentContainer partial class
    /// </summary>

    public partial class DocumentContainer
    {
        #region Events
        /// <summary>
        /// Event that is raised when AdjustStartPosition property is changed.
        /// </summary>
        public event PropertyChangedCallback AdjustStartPositionChanged;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets or sets a value indicating whether [adjust start position].
        /// </summary>
        /// <value><c>true</c> if [adjust start position]; otherwise, <c>false</c>.</value>
        public bool AdjustStartPosition
        {
            get
            {
                return (bool)GetValue(AdjustStartPositionProperty);
            }

            set
            {
                SetValue(AdjustStartPositionProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises AdjustStartPositionChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAdjustStartPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != AdjustStartPositionChanged)
            {
                AdjustStartPositionChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAdjustStartPositionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAdjustStartPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnAdjustStartPositionChanged(e);
        }
        /// <summary>
        /// Gets the IsMDIResizeProperty attached property
        /// </summary>
        /// <param name="obj">UIElement</param>
        /// <returns>bool</returns>
        public static bool GetIsMDIResize(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMDIResizeProperty);
        }
        /// <summary>
        /// Sets the IsMDIResizeProperty attached property
        /// </summary>
        /// <param name="obj">UIElement</param>
        /// <param name="value">value</param>
        public static void SetIsMDIResize(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMDIResizeProperty, value);
        }

        #endregion

        #region Dependency properties
        /// <summary>
        /// Presents property that indicate whether needs to change start position for new MDI window.
        /// </summary>
        public static readonly DependencyProperty AdjustStartPositionProperty = DependencyProperty.Register("AdjustStartPosition", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAdjustStartPositionChanged)));
        /// <summary>
        /// Presented property that indicates whether individual MDIWindow is resizable or not
        /// </summary>
        public static readonly DependencyProperty IsMDIResizeProperty = DependencyProperty.RegisterAttached("IsMDIResize", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true));

        #endregion
    }
}
