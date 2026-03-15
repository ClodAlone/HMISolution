#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents the Resource of Schedule
    /// </summary>
    public class Resource : DependencyObject
    {
        #region DependencyProperties

        #region ResourceName
        /// <summary>
        /// Gets or sets the name for resource.
        /// </summary>
        public string ResourceName
        {
            get { return (string)GetValue(ResourceNameProperty); }
            set { SetValue(ResourceNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResourceName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResourceNameProperty =
            DependencyProperty.Register("ResourceName", typeof(string), typeof(Resource), new PropertyMetadata(string.Empty));
        #endregion

        #region DisplayName
        /// <summary>
        /// Gets or sets the name to be displayed for resource.
        /// </summary>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(Resource), new PropertyMetadata(string.Empty));
        #endregion

        #region TypeName
        /// <summary>
        /// Gets or sets the type name for resource.
        /// </summary>
        public string TypeName
        {
            get { return (string)GetValue(TypeNameProperty); }
            set { SetValue(TypeNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeNameProperty =
            DependencyProperty.Register("TypeName", typeof(string), typeof(Resource), new PropertyMetadata(string.Empty));
        #endregion

        #endregion
    }
}
