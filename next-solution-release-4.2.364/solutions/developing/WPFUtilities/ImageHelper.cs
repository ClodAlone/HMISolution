using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System;
using DocumentManager.ComponentService;
using System.Windows.Media;
using Utilities;
using System.Windows.Data;
using VFS;
using System.Linq;

namespace WPFUtilities
{
    public static class ImageHelper
    {
        public enum BitmapEncoderFormat
        {
            Png,
            Bmp,
            Jpeg,
            Gif,
            Tiff
        };
        public static Image BitmapImageToImage(BitmapImage bi, BitmapEncoderFormat bf)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc;
                switch (bf)
                {
                    case BitmapEncoderFormat.Png:
                        enc = new PngBitmapEncoder();
                        break;
                    case BitmapEncoderFormat.Bmp:
                        enc = new BmpBitmapEncoder();
                        break;
                    case BitmapEncoderFormat.Jpeg:
                        enc = new JpegBitmapEncoder();
                        break;
                    case BitmapEncoderFormat.Gif:
                        enc = new GifBitmapEncoder();
                        break;
                    case BitmapEncoderFormat.Tiff:
                        enc = new TiffBitmapEncoder();
                        break;
                    default:
                        enc = new PngBitmapEncoder();
                        break;
                }
                enc.Frames.Add(BitmapFrame.Create(bi));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return bitmap as Image;
            }
        }
        public static string BrushToImagePath(IDocument document, object brush, bool bUseParentFolder = true)
        {
            string source = null;
            if (brush is VisualBrush)
            {
                VisualBrush color = (brush as VisualBrush);
                if (color.Visual is System.Windows.Controls.MediaElement)
                    source = GetSource(document, color.Visual as System.Windows.Controls.MediaElement, bUseParentFolder);
                else if (color.Visual is System.Windows.Controls.Image)
                    source = GetSource(document, color.Visual as System.Windows.Controls.Image, bUseParentFolder);
            }
            else if (brush is ImageBrush)
            {
                ImageBrush imageBrush = brush as ImageBrush;
                source = GetSource(document, brush as ImageBrush, bUseParentFolder);
            }
            else if (brush is System.Windows.Controls.Image)
            {
                System.Windows.Controls.Image imageBrush = brush as System.Windows.Controls.Image;
                source = GetSource(document, brush as System.Windows.Controls.Image, bUseParentFolder);
            }
            return source;
        }
        public static Uri BrushToImageUri(IDocument document, object brush)
        {
            Uri source = null;
            if (brush is VisualBrush)
            {
                VisualBrush color = (brush as VisualBrush);
                if (color.Visual is System.Windows.Controls.MediaElement)
                    source = GetUriSource(document, color.Visual as System.Windows.Controls.MediaElement);
                else if (color.Visual is System.Windows.Controls.Image)
                    source = GetUriSource(document, color.Visual as System.Windows.Controls.Image);
            }
            else if (brush is ImageBrush)
            {
                ImageBrush imageBrush = brush as ImageBrush;
                source = GetUriSource(document, brush as ImageBrush);
            }
            else if (brush is System.Windows.Controls.Image)
            {
                System.Windows.Controls.Image imageBrush = brush as System.Windows.Controls.Image;
                source = GetUriSource(document, brush as System.Windows.Controls.Image);
            }
            return source;
        }

        public static Uri GetUriSource(IDocument document, System.Windows.Controls.MediaElement media)
        {
            Uri source = null;
            if (media == null)
                return null;
            if (media.Source != null)
                return media.Source;
            else
            {
                BindingExpression sourceBinding = media.GetBindingExpression(System.Windows.Controls.MediaElement.SourceProperty);
                if (sourceBinding != null && sourceBinding.ParentBinding != null && !string.IsNullOrEmpty(sourceBinding.ParentBinding.ConverterParameter?.ToString()))
                {
                    string _source = sourceBinding.ParentBinding.ConverterParameter.ToString();
                    if (sourceBinding.ParentBinding.Converter is WPFUtilities.Converters.UriToUriAbsoluteImageConverter)
                    {
                        source = GetBindingUriSource((sourceBinding.ParentBinding.Converter as WPFUtilities.Converters.UriToUriAbsoluteImageConverter), document, _source);
                    }
                    else
                        source = GetRelativeUriSource(document, new Uri(_source, UriKind.RelativeOrAbsolute));
                }
            }
            //if (!string.IsNullOrEmpty(source) && source.StartsWith(baseFolder))
            //    source = source.Replace(baseFolder, "..\\..\\");
            return source;
        }
        public static Uri GetUriSource(IDocument document, System.Windows.Controls.Image image)
        {
            Uri source = null;
            if (image == null)
                return null;
            if (image.Source != null)
            {
                if (image.Source is System.Windows.Media.Imaging.BitmapImage)
                    source = GetRelativeUriSource(document, (image.Source as System.Windows.Media.Imaging.BitmapImage)?.UriSource);
                else
                    source = GetRelativeUriSource(document, new Uri(image.Source.ToString()));
            }
            else
            {
                BindingExpression sourceBinding = image.GetBindingExpression(System.Windows.Controls.Image.SourceProperty);
                if (sourceBinding != null && sourceBinding.ParentBinding != null && !string.IsNullOrEmpty(sourceBinding.ParentBinding.ConverterParameter?.ToString()))
                {
                    string _source = sourceBinding.ParentBinding.ConverterParameter.ToString();
                    if (sourceBinding.ParentBinding.Converter is WPFUtilities.Converters.UriToUriAbsoluteImageConverter)
                    {
                        source = GetBindingUriSource((sourceBinding.ParentBinding.Converter as WPFUtilities.Converters.UriToUriAbsoluteImageConverter), document, _source);
                    }
                    else
                        source = GetRelativeUriSource(document, new Uri(_source, UriKind.RelativeOrAbsolute));
                }
            }
            return source;
        }
        public static Uri GetUriSource(IDocument document, ImageBrush imageBrush)
        {
            Uri source = null;
            if (imageBrush.ImageSource != null)
            {
                if (imageBrush.ImageSource is System.Windows.Media.Imaging.BitmapImage)
                    source = GetRelativeUriSource(document, (imageBrush.ImageSource as System.Windows.Media.Imaging.BitmapImage).UriSource);

                return source;
            }
            else
                return source;
        }

        public static Uri GetBindingUriSource(WPFUtilities.Converters.UriToUriAbsoluteImageConverter converter, IDocument document, string _source)
        {
            if (converter == null)
                return new Uri(_source, UriKind.RelativeOrAbsolute);
            Uri source;
            var name = _source;
            if (name == null)
                return null;

            if (document.fileSystemProviderBase != null)
            {
                source = new Uri(name, UriKind.RelativeOrAbsolute);
                var uri = WPFUtilities.Converters.UriToAbsoluteUriConverter.GetFileSystemProviderBaseUri(source, document, SpecialFolders.Images);
                return uri;
            }

            try
            {
                string dest = System.IO.Path.GetDirectoryName(document.FilePath) + "\\";
                Uri AbsolutePath = document.GetSpecialFolder(SpecialFolders.Images);
                Uri AbsolutePath2 = new Uri(dest, UriKind.RelativeOrAbsolute);
                if ((AbsolutePath == null && AbsolutePath2 == null))
                    return null;

                name = new Uri(name, UriKind.RelativeOrAbsolute).GetPathString();
                source = new Uri(AbsolutePath, name);
                if (File.Exists(source.GetPathString()))
                    return source;
                else
                {
                    source = new Uri(AbsolutePath2, name);
                    if (File.Exists(source.GetPathString()))
                        return source;
                }
            }
            catch
            {
                return null;
            }

            return source;
        }

        public static Uri GetRelativeUriSource(IDocument document, Uri uriSource)
        {
            if (uriSource == null)
                return null;
            Uri source = null;

            if (document.fileSystemProviderBase != null)
            {
                var uri = WPFUtilities.Converters.UriToAbsoluteUriConverter.GetFileSystemProviderBaseUri(uriSource, document, SpecialFolders.Images);
                return uri;
            }
            else
            {
                try
                {
                    if (File.Exists(uriSource.GetPathString()))
                        return uriSource;
                }
                catch { }
            }

            try
            {
                Uri AbsolutePath = document.GetSpecialFolder(SpecialFolders.Images);
                var name = uriSource.GetPathString();
                source = new Uri(AbsolutePath, name);
                if (File.Exists(source.GetPathString()))
                    return source;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public static string GetSource(IDocument document, ImageBrush imageBrush, bool bUseParentFolder)
        {
            string source = null;
            if (imageBrush.ImageSource != null)
            {
                if (imageBrush.ImageSource is System.Windows.Media.Imaging.BitmapImage)
                    source = GetRelativeSource(document, (imageBrush.ImageSource as System.Windows.Media.Imaging.BitmapImage).UriSource, bUseParentFolder: bUseParentFolder);

                return source;
            }
            else
                return source;
        }

        public static string GetRelativeSource(IDocument document, Uri uriSource, bool skipCheck = false, bool bUseParentFolder = true)
        {
            if (uriSource == null)
                return null;
            Uri source = null;
            var parent = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(document, true);

            if (!skipCheck)
            {
                if (parent.fileSystemProviderBase != null)
                {
                    FileManagerFile fileManager = new FileManagerFile(parent.fileSystemProviderBase, uriSource.GetPathString());
                    if (parent.fileSystemProviderBase.Exists(fileManager))
                    {
                        source = uriSource;
                    }
                    else
                    {
                        try
                        {
                            if (File.Exists(uriSource.GetPathString()))
                                source = uriSource;
                        }
                        catch { }
                    }
                }
                else
                {
                    try
                    {
                        if (File.Exists(uriSource.GetPathString()))
                            source = uriSource;
                    }
                    catch { }
                }
            }
            else
                source = uriSource;

            if (source == null)
                return null;

            //source = parent.MakeRelativeUri(source);
            var title = System.IO.Path.GetFileNameWithoutExtension(parent.FilePath);
            if (XpoHelpers.XpoHelper.IsDataSource(parent.FilePath))
                title = XpoHelpers.XpoHelper.GetDataSourceTitle(parent.FilePath, onlytitle: true);
            string prefix = bUseParentFolder ? $"{images}\\{title}" : $"{images}";
            if (source.GetPathString().StartsWith(SpecialFolders.Images.ToString()))
                return $"{prefix}\\{System.IO.Path.GetFileName(source.GetPathString())}";
            else
            {
                string imagefolder = document.GetSpecialFolder(SpecialFolders.Images).GetPathString();
                if (source.GetPathString().StartsWith(imagefolder))
                    return $"{prefix}\\{source.GetPathString().Replace(imagefolder, "")}";

                string dest = System.IO.Path.GetDirectoryName(document.FilePath) + "\\";
                var absolutePath = new Uri(dest, UriKind.RelativeOrAbsolute);
                if (source.GetPathString().StartsWith(absolutePath.GetPathString()))
                    return System.IO.Path.GetFileName(uriSource.GetPathString());
                else
                    return source.GetPathString();
            }
        }

        public static string GetSource(IDocument document, System.Windows.Controls.Image image, bool bUseParentFolder)
        {
            string source = null;
            if (image == null)
                return null;
            if (image.Source != null)
            {
                if (image.Source is System.Windows.Media.Imaging.BitmapImage)
                    source = GetRelativeSource(document, (image.Source as System.Windows.Media.Imaging.BitmapImage)?.UriSource, bUseParentFolder: bUseParentFolder);
                else
                    source = GetRelativeSource(document, new Uri(image.Source.ToString()), bUseParentFolder: bUseParentFolder);
            }
            else
            {
                BindingExpression sourceBinding = image.GetBindingExpression(System.Windows.Controls.Image.SourceProperty);
                if (sourceBinding != null && sourceBinding.ParentBinding != null && !string.IsNullOrEmpty(sourceBinding.ParentBinding.ConverterParameter?.ToString()))
                {
                    string _source = sourceBinding.ParentBinding.ConverterParameter.ToString();
                    if (sourceBinding.ParentBinding.Converter is WPFUtilities.Converters.UriToUriAbsoluteImageConverter)
                    {
                        source = GetBindingSource((sourceBinding.ParentBinding.Converter as WPFUtilities.Converters.UriToUriAbsoluteImageConverter), document, _source, bUseParentFolder);
                    }
                    else
                        source = GetRelativeSource(document, new Uri(_source, UriKind.RelativeOrAbsolute), bUseParentFolder: bUseParentFolder);
                }
            }
            return source;
        }

        public static string GetBindingSource(WPFUtilities.Converters.UriToUriAbsoluteImageConverter converter, IDocument document, string _source, bool bUseParentFolder)
        {
            if (converter == null)
                return _source;
            string source = null;
            string dest = System.IO.Path.GetDirectoryName(document.FilePath) + "\\";
            Uri AbsolutePath = document.GetSpecialFolder(SpecialFolders.Images); 
            Uri AbsolutePath2 = new Uri(dest, UriKind.RelativeOrAbsolute);

            var name = _source;
            if (name == null || (AbsolutePath == null && AbsolutePath2 == null))
                return null;
            Uri uri = null;
            name = new Uri(name, UriKind.RelativeOrAbsolute).GetPathString();
            var parent = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(document, true);

            if (parent.fileSystemProviderBase != null)
            {
                var vfsFile = new FileManagerFile(parent.fileSystemProviderBase, name);
                if (!parent.fileSystemProviderBase.Exists(vfsFile))
                    vfsFile = new FileManagerFile(parent.fileSystemProviderBase, new FileManagerFolder(parent.fileSystemProviderBase, AbsolutePath.GetPathString()), name);
                if (!parent.fileSystemProviderBase.Exists(vfsFile))
                    vfsFile = new FileManagerFile(parent.fileSystemProviderBase, new FileManagerFolder(parent.fileSystemProviderBase, AbsolutePath2.GetPathString()), name);
                if (!parent.fileSystemProviderBase.Exists(vfsFile))
                    return null;

                try
                {
                    uri = new Uri(vfsFile.FullName, UriKind.RelativeOrAbsolute);
                    source = GetRelativeSource(document, uri, true, bUseParentFolder);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    uri = new Uri(AbsolutePath, name);
                    if (File.Exists(uri.GetPathString()))
                        source = GetRelativeSource(document, uri, true, bUseParentFolder);
                    else
                    {
                        uri = new Uri(AbsolutePath2, name);
                        if (File.Exists(uri.GetPathString()))
                            source = GetRelativeSource(document, uri, true, bUseParentFolder);
                        else 
                        {
                            uri = new Uri(document.GetSpecialFolder(SpecialFolders.Documents), name);
                            if (File.Exists(uri.GetPathString()))
                                source = GetRelativeSource(document, uri, true, bUseParentFolder);
                            else
                                return GetRelativeSource(document, new Uri(name, UriKind.RelativeOrAbsolute), true, bUseParentFolder); 
                        }
                    }
                }
                catch
                {
                    try
                    {
                        uri = new Uri(AbsolutePath2, name);
                        if (File.Exists(uri.GetPathString()))
                            source = GetRelativeSource(document, uri, true, bUseParentFolder);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return source;
        }

        public static string GetSource(IDocument document, System.Windows.Controls.MediaElement media, bool bUseParentFolder)
        {
            string source = null;
            if (media == null)
                return null;
            if (media.Source != null)
            {
                if(media.Source.IsAbsoluteUri)
                    source = media.Source.GetPathString();
                else
                    source = GetBindingSource(new Converters.UriToUriAbsoluteImageConverter() 
                    { AbsolutePath = document.GetSpecialFolder(SpecialFolders.Images), 
                      AbsolutePath2 = document.GetSpecialFolder(SpecialFolders.Documents)}, document, media.Source.GetPathString(), bUseParentFolder);
            }
            else
            {
                BindingExpression sourceBinding = media.GetBindingExpression(System.Windows.Controls.MediaElement.SourceProperty);
                if (sourceBinding != null && sourceBinding.ParentBinding != null && !string.IsNullOrEmpty(sourceBinding.ParentBinding.ConverterParameter?.ToString()))
                {
                    string _source = sourceBinding.ParentBinding.ConverterParameter.ToString();
                    if (sourceBinding.ParentBinding.Converter is WPFUtilities.Converters.UriToUriAbsoluteImageConverter)
                    {
                        source = GetBindingSource((sourceBinding.ParentBinding.Converter as WPFUtilities.Converters.UriToUriAbsoluteImageConverter), document, _source, bUseParentFolder);
                    }
                    else
                        source = GetRelativeSource(document, new Uri(_source, UriKind.RelativeOrAbsolute), bUseParentFolder: bUseParentFolder);
                }
            }
            //if (!string.IsNullOrEmpty(source) && source.StartsWith(baseFolder))
            //    source = source.Replace(baseFolder, "..\\..\\");
            return source;
        }
        static string images = $"{SpecialFolders.Images.ToString().First<char>().ToString().ToLower()}{SpecialFolders.Images.ToString().Substring(1)}";
    }
}
