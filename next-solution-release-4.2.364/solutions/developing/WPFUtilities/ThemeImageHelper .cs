using DocumentManager.ComponentService;
using DocumentManager.ComponentService.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml;
using Utilities;
using Utilities.WPF;

namespace Utilities
{
    public class ThemeImageHelper : IDisposable
    {
        public static BitmapImage TransformToImage(BitmapImage sourceimage, IDocument document, bool bignoretheme = false, string themename = null)
        {
            var themeName = themename ?? GetTheme(document);
            if (string.IsNullOrEmpty(themeName) || bignoretheme)
                return sourceimage;

            switch (themeName)
            {
                case "None": return InvertColorImage(sourceimage); break;
                case "Default": return InvertColorImage(sourceimage); break;
                //case "Blend": return sourceimage; break;
                case "VS2010": return InvertColorImage(sourceimage); break;
                //case "Office2007Black": return sourceimage; break;
                //case "Office2007Silver": return sourceimage; break;
                //case "Office2007Blue": return sourceimage; break;
                //case "Office2010Black": return sourceimage; break;
                //case "Office2010Silver": return sourceimage; break;
                //case "Office2010Blue": return sourceimage; break;
                //case "Office2013": return sourceimage; break;
                //case "TouchlineDark": return sourceimage; break;
                default: return sourceimage; break;
            }
        }
        public static string GetTheme(IDocument document)
        {
            if (document == null)
                return null;
            var parent = DocumentHelper.GetRootParent(document, traverse: true);
            if (parent != null)
            {
                IScreenController controller = parent as IScreenController;
                if (controller != null)
                    return controller.GetTheme().ToString();
                else
                    return null;
            }
            else
                return null;
        }

        #region Project Theme
        static public bool NeedsDarkBitmapResources(IDocument document)
        {
            String currentStyle = GetTheme(document);
            return NeedsDarkBitmapResources(currentStyle);
        }

        static public bool NeedsDarkBitmapResources(string currentStyle)
        {
            switch (currentStyle)
            {
                case "Default": return true;
                case "Blend": return false;
                case "VS2010": return true;
                case "Office2007Black": return false;
                case "Office2007Silver": return true;
                case "Office2007Blue": return true;
                case "Office2010Black": return false;
                case "Office2010Silver": return true;
                case "Office2010Blue": return true;
                case "Office2013": return true;
                case "TouchlineDark": return false;
                case "VS2017Light": return true;
                case "VS2017Dark": return false;
                case "VS2017Dark2": return false;
                default: return false;
            }
        }

        static public void LoadBitmapImageResourceDictionary(ResourceDictionary resources, IDocument document)
        {
            LoadBitmapImageResourceDictionary(resources, document, false);
        }
        static public void LoadBitmapImageResourceDictionary(ResourceDictionary resources, FrameworkElement fe, bool bReplaceEditing)
        {
            string currentStyle = null;
            var wnd = fe.FindParent<Window>();
            try
            {
                if (wnd != null)
                    currentStyle = WPFUtilities.ThemeHelper.GetTheme(wnd);
            }
            catch (Exception)
            {
            }
            if (string.IsNullOrEmpty(currentStyle))
                return;
            bool needDarkBitmapResources = NeedsDarkBitmapResources(currentStyle);
            LoadResourceDictionary(resources, needDarkBitmapResources, bReplaceEditing);
        }

        static public void LoadBitmapImageResourceDictionary(ResourceDictionary resources, IDocument document, bool bReplaceEditing)
        {
            if (document == null)
                return;
            bool needDarkBitmapResources = NeedsDarkBitmapResources(document);
            LoadResourceDictionary(resources, needDarkBitmapResources, bReplaceEditing);
        }

        static void LoadResourceDictionary(ResourceDictionary resources, bool needDarkBitmapResources, bool bReplaceEditing)
        {
            if (needDarkBitmapResources || bReplaceEditing)
            {
                string resourceToReplace = WPFUtilities.Properties.Settings.Default.RuntimeLightSharedResourcesNamespace;
                if (bReplaceEditing)
                    resourceToReplace = WPFUtilities.Properties.Settings.Default.SharedResourcesNamespace;
                string newresource = WPFUtilities.Properties.Settings.Default.RuntimeDarkSharedResourcesNamespace;
                if (!needDarkBitmapResources && bReplaceEditing)
                    newresource = WPFUtilities.Properties.Settings.Default.RuntimeLightSharedResourcesNamespace;
                var toRemoveList = (from resource in resources.MergedDictionaries
                                    where resource.Source.OriginalString.Contains(resourceToReplace)
                                    select resource).ToList();
                toRemoveList.ForEach(r =>
                {
                    r.Source = new Uri(r.Source.OriginalString.Replace(resourceToReplace, newresource), UriKind.RelativeOrAbsolute);
                    //resources.MergedDictionaries.Remove(r);
                    //resources.MergedDictionaries.Add(new ResourceDictionary() 
                    //{ 
                    //    Source = new Uri(r.Source.OriginalString.Replace(WPFUtilities.Properties.Settings.Default.RuntimeLightSharedResourcesNamespace, 
                    //                     WPFUtilities.Properties.Settings.Default.RuntimeDarkSharedResourcesNamespace)) 
                    //});
                });
            }
        }
        #endregion

        static BitmapImage InvertColorImage(BitmapImage sourceimage)
        {
            try
            {
                BitmapImage retval;

                using (MemoryStream outStream = new MemoryStream())
                {
                    PngBitmapEncoder enc = new PngBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(sourceimage));
                    enc.Save(outStream);
                    using (Bitmap source = new Bitmap(outStream))
                    {
                        using (Bitmap newBitmap = new Bitmap(source.Width, source.Height))
                        {
                            //get a graphics object from the new image
                            Graphics g = Graphics.FromImage(newBitmap);

                            //// create some image attributes
                            ImageAttributes attributes = new ImageAttributes();

                            ColorMatrix colorMatrix = new ColorMatrix(
                                   new float[][]
                                   {
                                      new float[] {-1, 0, 0, 0, 0},
                                      new float[] {0, -1, 0, 0, 0},
                                      new float[] {0, 0, -1, 0, 0},
                                      new float[] {0, 0, 0, 1, 0},
                                      new float[] {1, 1, 1, 0, 1}
                                   });

                            attributes.SetColorMatrix(colorMatrix);

                            g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                                        0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);

                            using (MemoryStream memory = new MemoryStream())
                            {
                                newBitmap.Save(memory, ImageFormat.Png);
                                memory.Position = 0;
                                retval = new BitmapImage();
                                retval.BeginInit();
                                retval.StreamSource = memory;
                                retval.CacheOption = BitmapCacheOption.OnLoad;
                                retval.EndInit();
                            }
                            //dispose the Graphics object
                            g.Dispose();
                            attributes.Dispose();
                        }
                    }
                }
                return retval;
            }
            catch
            {
                return sourceimage;
            }
        }

        bool bDisposed;
        public void Dispose()
        {
            bDisposed = true;
        }
    }
}
