// <copyright file="UriConverter.cs" company="$registerdorganization$">
// Copyright (c) 2010 Microsoft. All Right Reserved
// </copyright>
// <author>Claudio</author>
// <email></email>
// <date>2010-07-23</date>
// <summary>A value converter for WPF and Silverlight data binding</summary>

namespace AnimatedObjects.Controls
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using DocumentManager.ComponentService;
    using OPCUAViewModelService.ComponentService;
    using Utilities;
    using System.Windows.Media.Imaging;
    using System.Windows.Markup;
    using System.Collections.Generic;
    using WPFUtilities;

    /// <summary>
    /// A Value converter
    /// </summary>
    public class UriToVisualBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value ==  DependencyProperty.UnsetValue ||  value == null || value.ToString().Length == 0)
                return new SolidColorBrush(Colors.Transparent);

            try
            {
                if (OPCUAViewModelComponent.workspaceServiceAvailable)
                {
                    var document = OPCUAViewModelComponent.workspaceService.ContextDocument;
                    if(document == null)
                        return new SolidColorBrush(Colors.Transparent);

                    Uri uriFound = WPFUtilities.ImageHelper.GetRelativeUriSource(document, value as Uri);
                    if (!File.Exists(uriFound.GetPathString()))
                        return new SolidColorBrush(Colors.Transparent);
                    string sourcefilename = uriFound.GetPathString();
                    var ext = System.IO.Path.GetExtension(sourcefilename);
                    if (DropFileHelper.SupportedImageExtension.Contains(ext.ToLower()))
                    {
                        var image = new Image();
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri(sourcefilename, UriKind.RelativeOrAbsolute);
                        bmp.EndInit();
                        image.Source = bmp;
                        return new VisualBrush(image);
                    }
                    else
                    {
                        try
                        {
                            var md = new MediaElement();
                            md.Source = uriFound;
                            (md as IUriContext).BaseUri = uriFound;
                            Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(md);
                            return new VisualBrush(md);
                        }
                        catch
                        {
                            return new SolidColorBrush(Colors.Transparent);
                        }
                    }
                }
                else
                    return new SolidColorBrush(Colors.Transparent);
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
