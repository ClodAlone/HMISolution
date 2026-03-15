using DocumentManager.ComponentService;
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
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml;
using Utilities;
using DocumentManager.ComponentService.Helpers;

namespace ImageHelper
{
    public class ImageHelper : IDisposable
    {
        public static BitmapSource Transform(BitmapImage sourceimage, IDocument document)
        {
            if (document == null)
                return sourceimage;
            var parent = DocumentHelper.GetRootParent(document, traverse: true);
            if (parent != null)
            {
                IScreenController controller = parent as IScreenController;
                if(controller != null)
                {
                    var theme = controller.GetTheme();
                    var themeName = theme.ToString();
                    switch (themeName)
                    {
                        case "Default": return InvertColor(sourceimage, new ColorMap[] { new ColorMap() { OldColor = Color.FromName("White"), NewColor = Color.FromName("Black") } }); break;
                        //case "Blend": return sourceimage; break;
                        //case "VS2010": return sourceimage; break;
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
                else return sourceimage;
            }
            else return sourceimage;
        }

        static BitmapSource InvertColor(BitmapImage sourceimage, ColorMap[] remapTable)
        {
            try
            {
                Bitmap source;
                BitmapSource retval;

                using (MemoryStream outStream = new MemoryStream())
                {
                    PngBitmapEncoder enc = new PngBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(sourceimage));
                    enc.Save(outStream);
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                    source = new Bitmap(bitmap);
                }
                //create a blank bitmap the same size as original
                Bitmap newBitmap = new Bitmap(source.Width, source.Height);

                //get a graphics object from the new image
                Graphics g = Graphics.FromImage(newBitmap);

                //// create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                attributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                            0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);

                //dispose the Graphics object
                g.Dispose();

                IntPtr hBitmap = newBitmap.GetHbitmap();

                retval = Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());

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
