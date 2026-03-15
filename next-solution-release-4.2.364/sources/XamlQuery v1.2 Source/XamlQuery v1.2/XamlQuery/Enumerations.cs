//Enumerations.cs

/*
 * XamlQuery Silverlight Library v1.2
 * http://XamlQuery.codeplex.com/
 * 
 * Author: cincoutprabu (cincoutprabu@gmail.com)
 * Copyright 2010 cincoutprabu. All rights reserved.
 * 
 * Released under Microsoft Public License.
 * http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 *
 * Date: 2010-Sep-10 06:00 IST
 * Revision: 1
 */

using System;
using System.Linq;

namespace XamlQuery
{
    /// <summary>
    /// Provides a list of standard speeds for animation.
    /// </summary>
    public enum Speed
    {
        /// <summary>
        /// The animation finishes within 3000 milliseconds.
        /// </summary>
        VerySlow = 3000,

        /// <summary>
        /// The animation finishes within 2000 milliseconds.
        /// </summary>
        Slow = 2000,

        /// <summary>
        /// The animation finishes within 1000 milliseconds.
        /// </summary>
        Normal = 1000,

        /// <summary>
        /// The animation finishes within 500 milliseconds.
        /// </summary>
        Fast = 500,

        /// <summary>
        /// The animation finishes within 200 milliseconds.
        /// </summary>
        VeryFast = 200
    }

    /// <summary>
    /// Provides a list of high-level event names, useful for binding and triggering events easily.
    /// </summary>
    public enum EventType
    {
        #region System.Windows.UIElement events

        /// <summary>
        /// Indicates GotFocus event of System.Windows.UIElement.
        /// </summary>
        GotFocus = 1,

        /// <summary>
        /// Indicates LostFocus event of System.Windows.UIElement.
        /// </summary>
        LostFocus = 2,

        /// <summary>
        /// Indicates KeyDown event of System.Windows.UIElement.
        /// </summary>
        KeyDown = 3,

        /// <summary>
        /// Indicates KeyUp event of System.Windows.UIElement.
        /// </summary>
        KeyUp = 4,

        /// <summary>
        /// Indicates MouseEnter event of System.Windows.UIElement.
        /// </summary>
        MouseEnter = 5,

        /// <summary>
        /// Indicates MouseLeave event of System.Windows.UIElement.
        /// </summary>
        MouseLeave = 6,

        /// <summary>
        /// Indicates MouseMove event of System.Windows.UIElement.
        /// </summary>
        MouseMove = 7,

        /// <summary>
        /// Indicates LostMouseCapture event of System.Windows.UIElement.
        /// </summary>
        LostMouseCapture = 8,

        /// <summary>
        /// Indicates MouseLeftButtonDown event of System.Windows.UIElement.
        /// </summary>
        MouseLeftButtonDown = 9,

        /// <summary>
        /// Indicates MouseLeftButtonUp event of System.Windows.UIElement.
        /// </summary>
        MouseLeftButtonUp = 10,

        /// <summary>
        /// Indicates MouseWheel event of System.Windows.UIElement.
        /// </summary>
        MouseWheel = 11,

        #endregion

        #region System.Windows.FrameworkElement events

        /// <summary>
        /// Indicates BindingValidationError event of System.Windows.FrameworkElement.
        /// </summary>
        BindingValidationError = 12,

        /// <summary>
        /// Indicates Loaded event of System.Windows.FrameworkElement.
        /// </summary>
        Loaded = 13,

        /// <summary>
        /// Indicates LayoutUpdated event of System.Windows.FrameworkElement.
        /// </summary>
        LayoutUpdated = 14,

        /// <summary>
        /// Indicates SizeChanged event of System.Windows.FrameworkElement.
        /// </summary>
        SizeChanged = 15,

        #endregion

        #region System.Windows.Controls.Control events

        /// <summary>
        /// Indicates IsEnabledChanged event of System.Windows.Controls.Control.
        /// </summary>
        IsEnabledChanged = 16,

        #endregion

        #region System.Windows.Controls.TextBox events

        /// <summary>
        /// Indicates TextChanged event of System.Windows.Controls.TextBox.
        /// </summary>
        TextChanged = 17,

        /// <summary>
        /// Indicates TextSelectionChanged event of System.Windows.Controls.TextBox.
        /// </summary>
        TextSelectionChanged = 18,

        #endregion

        #region System.Windows.Controls.Primitives.Selector events

        /// <summary>
        /// Indicates SelectionChanged event of System.Windows.Controls.Primitives.Selector.
        /// </summary>
        SelectionChanged = 19

        #endregion
    }

    /// <summary>
    /// Represents the type of matching a specified property's value (while choosing a set of controls).
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Controls whose property value is equal to specified value are chosen. The Equals() method of corresponding property is used for checking the equality.
        /// </summary>
        Equals = 1,

        /// <summary>
        /// Controls whose property value is not equal to specified value are chosen. The Equals() method of corresponding property is used for checking the equality.
        /// </summary>
        NotEquals = 2,

        /// <summary>
        /// Controls whose property value starts with a specified value are chosen. The ToString() method of corresponding property is used for matching.
        /// </summary>
        StartsWith = 3,

        /// <summary>
        /// Controls whose property value ends with a specified value are chosen. The ToString() method of corresponding property is used for matching.
        /// </summary>
        EndsWith = 4,

        /// <summary>
        /// Controls whose property value contains a specified value are chosen. The ToString() method of corresponding property is used for matching.
        /// </summary>
        Contains = 5
    }
}
