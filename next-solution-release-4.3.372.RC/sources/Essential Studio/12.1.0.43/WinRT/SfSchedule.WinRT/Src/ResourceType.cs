#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#else
using System.Windows;
using System.Windows.Markup;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents the Resource Type of Schedule.
    /// </summary>
#if WINRT
    [ContentProperty(Name = "ResourceCollection")]
#else
    [ContentProperty("ResourceCollection")]
#endif
    public class ResourceType : DependencyObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.ResourceType">ResourceType</see> class. 
        /// </summary>
        public ResourceType()
        {
            ResourceCollection = new ObservableCollection<Resource>();
        }

        #endregion

        #region DependencyProperties

        #region ResourceCollection
        /// <summary>
        /// Gets or sets the collection of resources.
        /// </summary>
        public ObservableCollection<Resource> ResourceCollection
        {
            get { return (ObservableCollection<Resource>)GetValue(ResourceCollectionProperty); }
            set { SetValue(ResourceCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResourceCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResourceCollectionProperty =
            DependencyProperty.Register("ResourceCollection", typeof(ObservableCollection<Resource>), typeof(ResourceType), new PropertyMetadata(null));
        #endregion

        #region TypeName
        /// <summary>
        /// Gets or sets the type name for resource type.
        /// </summary>
        public string TypeName
        {
            get { return (string)GetValue(TypeNameProperty); }
            set { SetValue(TypeNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TypeName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TypeNameProperty =
            DependencyProperty.Register("TypeName", typeof(string), typeof(ResourceType), new PropertyMetadata(string.Empty));
        #endregion

        #region SubResourceType
        /// <summary>
        /// Gets or sets the sub resource type.
        /// </summary>
        public ResourceType SubResourceType
        {
            get { return (ResourceType)GetValue(SubResourceTypeProperty); }
            set { SetValue(SubResourceTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SubResourceType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SubResourceTypeProperty =
            DependencyProperty.Register("SubResourceType", typeof(ResourceType), typeof(ResourceType), new PropertyMetadata(null));
        #endregion

        #endregion
    }
}
