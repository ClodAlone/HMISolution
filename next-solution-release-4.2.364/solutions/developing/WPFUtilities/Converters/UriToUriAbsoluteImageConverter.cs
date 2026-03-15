// <copyright file="UriConverter.cs" company="$registerdorganization$">
// Copyright (c) 2010 Microsoft. All Right Reserved
// </copyright>
// <author>Claudio</author>
// <email></email>
// <date>2010-07-23</date>
// <summary>A value converter for WPF and Silverlight data binding</summary>

namespace WPFUtilities.Converters
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Xml;
    using UFInterfaces.Converters;
    using Utilities;
    using VFS;

    /// <summary>
    /// A Value converter
    /// </summary>
    public class UriToUriAbsoluteImageConverter : IUriToUriAbsoluteImageConverter
    {
        #region Converters.IAbsoluteImageConverter
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileSystemProviderBase FileSystemProviderBase { get; set; }
        public Uri AbsolutePath { get; set; }
        public Uri AbsolutePath2 { get; set; }
        #endregion

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
            var name = parameter as String;
            if (name == null || (AbsolutePath == null && AbsolutePath2 == null))
                return null;
            Uri uri = null;
            name = new Uri(name, UriKind.RelativeOrAbsolute).GetPathString();
            try
            {
                uri = new Uri(AbsolutePath, name);
            }
            catch
            { }

            if (targetType == typeof(Uri))
            {
                return uri;
            }
            else if (targetType == typeof(ImageSource))
            {
                if (uri == null && FileSystemProviderBase != null)
                {
                    var vfsFile = new FileManagerFile(FileSystemProviderBase, new FileManagerFolder(FileSystemProviderBase, AbsolutePath.GetPathString()), name);
                    if (!FileSystemProviderBase.Exists(vfsFile))
                        vfsFile = new FileManagerFile(FileSystemProviderBase, new FileManagerFolder(FileSystemProviderBase, AbsolutePath2.GetPathString()), name);
                    if (!FileSystemProviderBase.Exists(vfsFile))
                        return null;

                    try
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.StreamSource = new MemoryStream(FileSystemProviderBase.ReadFile(vfsFile));
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                        return img;
                    }
                    catch
                    {
                        return null;
                    }
                }

                try
                {
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = uri;
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    return img;
                }
                catch
                {
                    try
                    {
                        uri = new Uri(AbsolutePath2, name);
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.UriSource = uri;
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                        return img;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            else
                throw new InvalidOperationException("Target Type must be a Uri or a ImageSource");
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


    public class BindingExpressionConverter : ExpressionConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension))
                return true;
            else return false;
        }
        public override object ConvertTo(ITypeDescriptorContext context,
                                         System.Globalization.CultureInfo culture,
                                         object value, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension))
            {
                BindingExpression bindingExpression = value as BindingExpression;
                if (bindingExpression == null)
                    throw new Exception();
                return bindingExpression.ParentBinding;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public static class BindingExpressionHelper
    {
        public static void Register<T, TC>()
        {
            Attribute[] attr = new Attribute[1];
            TypeConverterAttribute vConv = new TypeConverterAttribute(typeof(TC));
            attr[0] = vConv;
            TypeDescriptor.AddAttributes(typeof(T), attr);
            var at = TypeDescriptor.GetAttributes(typeof(T));
        }
        public static string Save(this Brush obj)
        {
            try
            {
                if (obj is VisualBrush && ((obj as VisualBrush).Visual is System.Windows.Controls.Image))
                {
                    Register<BindingExpression, BindingExpressionConverter>();
                    StringBuilder outstr = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = true;
                    XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings));
                    dsm.XamlWriterMode = XamlWriterMode.Expression;

                    XamlWriter.Save(obj, dsm);
                    return outstr.ToString();
                }
                else
                    return XamlWriter.Save(obj);
            }
            catch
            {
                return XamlWriter.Save(obj);
            }
        }
    }
}
