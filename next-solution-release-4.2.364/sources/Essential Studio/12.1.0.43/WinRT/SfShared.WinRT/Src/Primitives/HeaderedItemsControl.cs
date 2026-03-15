// <copyright file="HeaderedItemsControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Syncfusion.WP.Primitives
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
namespace Syncfusion.Tools.Primitives
#else
#if WPF
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
namespace Syncfusion.Windows.Primitives
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Primitives
#endif
#endif
#endif
{
    /// <summary>
    ///  Represents a control that contains multiple items and has a header.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl"/>
    [ClassReference(IsReviewed = false)]
    public class HeaderedItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl"/> class.
        /// </summary>
        public HeaderedItemsControl()
        {
            DefaultStyleKey = typeof(HeaderedItemsControl);
        }

        #endregion



        #region Dependency Properties

        /// <summary>
        /// Gets or sets  the style that appearance of the <see
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.Header"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.HeaderTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.HeaderTemplateSelector"/>
        [ClassReference(IsReviewed = false)]
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(HeaderedItemsControl), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the data used for the header of each control.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.HeaderStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.HeaderTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.HeaderTemplateSelector"/>
        [ClassReference(IsReviewed = false)]
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(HeaderedItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="N:Windows.UI.Xaml.DataTemplate">DataTemplate</see> used
        /// to display the content of the control&apos;s <see
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.Header"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.HeaderStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.HeaderTemplateSelector"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a <see
        /// cref="N:Windows.UI.Xaml.Controls.DataTemplateSelector">DataTemplateSelector</see> that provides
        /// custom logic for choosing the template used to display the <see
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.Header"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.HeaderStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl.HeaderTemplate"/>
        [ClassReference(IsReviewed = false)]
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT)
        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
            set { SetValue(HeaderTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateSelectorProperty =
            DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(HeaderedItemsControl), new PropertyMetadata(null));

#endif
        #endregion

        /// <summary>
        /// Arranges the container for overrided items
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parentItemsControl"></param>
        public void PrepareHeaderedItemsControlContainer(object item, ItemsControl parentItemsControl)
        {
            var parentDataTemplate = parentItemsControl.ItemTemplate;

            if (parentDataTemplate != null)
            {
                ItemTemplate = parentDataTemplate;
            }

            var template = parentDataTemplate as HierarchicalDataTemplate;

            if (template != null)
            {
                HeaderTemplate= template.Template;

                if (template.ItemTemplate != null)
                {
                    ItemTemplate = template.ItemTemplate;
                }
                else if (template.Template != null)
                {
                    ItemTemplate = parentItemsControl.ItemTemplate;
                }

                if (template.ItemsSource != null)
                {
                    var binding = new Binding()
                                      {
                                          Source = Header,
                                          Path = template.ItemsSource.Path,
                                          Mode = template.ItemsSource.Mode,
                                          Converter = template.ItemsSource.Converter,
                                          ConverterParameter = template.ItemsSource.ConverterParameter,
#if !WINRT
                                          ConverterCulture =template.ItemsSource.ConverterCulture,
#else
                                           ConverterLanguage = template.ItemsSource.ConverterLanguage,
#endif

                                      };
                    this.SetBinding(ItemsSourceProperty, binding);
                }
                
                if (template.ItemContainerStyle != null)
                {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT)
                    this.ItemContainerStyle = template.ItemContainerStyle;
#endif
                }
               
            }
        }
    }
}
