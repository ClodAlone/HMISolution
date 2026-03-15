// <copyright file="PreviewBorder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class draws a visual element from <see cref="Syncfusion.Windows.Shared.PreviewBorder"/> as background.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PreviewBorder : Border
    {
        #region Private members
        /// <summary>
        /// stretch type.
        /// </summary>
        private readonly Stretch m_Stretch;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewBorder"/> class.
        /// </summary>
        public PreviewBorder()
            : this(Stretch.Uniform)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewBorder"/> class.
        /// </summary>
        /// <param name="stretch">The stretch.</param>
        public PreviewBorder(Stretch stretch)
        {
            m_Stretch = stretch;
            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when [data context changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Visual dataContext = e.NewValue as Visual;

            if (null != dataContext)
            {
                VisualBrush visualBrush = new VisualBrush
                {
                    Visual = dataContext,
                    Stretch = m_Stretch
                };

                Background = visualBrush;
            }
            else
            {
                Background = Brushes.Transparent;
            }
        }
        #endregion
    }
}
