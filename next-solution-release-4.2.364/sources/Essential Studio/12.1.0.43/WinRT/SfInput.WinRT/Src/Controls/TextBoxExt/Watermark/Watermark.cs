// <copyright file="Watermark.cs" company="Syncfusion">
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

#if WPF
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;
using Syncfusion.WP.Primitives;

namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Tools.Controls.Input
#else
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    //[ClassReference(IsReviewed = true)]
    public partial class SfTextBoxExt : TextBox
    {
        #region Variables

        private FrameworkElement PART_Watermark;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the content displayed as a watermark in the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt"/> when it is empty.
        /// </summary>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.WatermarkTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.WatermarkTemplateSelector"/>
        [ClassReference(IsReviewed = false)]
#if (WINDOWS_PHONE_7||WINRT||WPF|| WINDOWS_PHONE)
        public object Watermark
#else 
        public new object Watermark
#endif
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        /// </summary>
#if (WINDOWS_PHONE_7||WINRT||WPF|| WINDOWS_PHONE)
        public static readonly DependencyProperty WatermarkProperty=
#else
        public static new readonly DependencyProperty WatermarkProperty =
#endif
            DependencyProperty.Register("Watermark", typeof(object), typeof(SfTextBoxExt), new PropertyMetadata(null));


        /// <summary>
        /// <para>Gets or sets the <see cref="N:Windows.UI.Xaml.DataTemplate"/>
        /// used to display the <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Watermark"/> of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt"/>.</para>
        /// </summary>
        /// <value>
        /// The Default value is null. 
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Watermark"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.WatermarkTemplateSelector"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WatermarkTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(SfTextBoxExt), new PropertyMetadata(null));

       #if WPF || WINRT
        /// <summary>
        /// Provides a way to choose a <see
        /// cref="N:Windows.UI.Xaml.Controls.DataTemplateSelector"/> based on
        /// the data object and the data-bound <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.Watermark"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt.WatermarkTemplate"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplateSelector WatermarkTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(WatermarkTemplateSelectorProperty); }
            set { SetValue(WatermarkTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WatermarkTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateSelectorProperty =
            DependencyProperty.Register("WatermarkTemplateSelector", typeof(DataTemplateSelector), typeof(SfTextBoxExt), new PropertyMetadata(null));
#endif

        #endregion

        #region Helper Methods

        private void UpdateWatermark()
        {
            if (PART_Watermark != null)
            {
                if ((PART_Watermark as ContentControl).Content == null)
                    PART_Watermark.DataContext = null;
                if (isFocussed)
                {
                    PART_Watermark.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (String.IsNullOrEmpty(Text))
                    {
                        PART_Watermark.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        PART_Watermark.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        #endregion

    }
}
