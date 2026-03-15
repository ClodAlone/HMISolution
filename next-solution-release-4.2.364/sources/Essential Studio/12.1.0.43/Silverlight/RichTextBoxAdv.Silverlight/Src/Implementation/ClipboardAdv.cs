#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Imaging;
using System.Text;

namespace Syncfusion.Windows.Tools.Controls
{
    public static class ClipboardAdv
    {
        private static BlockCollection<BlockAdv> Blocks;

        internal static string Text;

        /// <summary>
        /// Sets text in clipboard
        /// </summary>
        /// <param name="Text"></param>
        internal static void SetText(string text)
        {
            try
            {
                Text = text;
                Clipboard.SetText(text);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Queries the clipboard for the presence of data in unicode format 
        /// </summary>
        /// <returns></returns>
        internal static bool ContainsText()
        {
            bool flag = false;

            try
            {
                flag = Clipboard.ContainsText();
            }
            catch
            {

            }

            return flag;
        }

        /// <summary>
        /// Gets the text from clipboard
        /// </summary>
        /// <returns></returns>
        internal static string GetText()
        {
            string text = string.Empty;

            try
            {
                text = Clipboard.GetText();
            }
            catch
            {

            }

            //return Clipboard.GetText();
            return text;
        }

        /// <summary>
        /// Gets the copied block
        /// </summary>
        /// <param name="blocks"></param>
        internal static void SetBlock(BlockCollection<BlockAdv> blocks)
        {
            Blocks = blocks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static BlockCollection<BlockAdv> GetBlock()
        {
            if (ContainsText())
            {
                if (GetText() == Text && (Blocks != null && Blocks.Count > 0))
                {
                    return Blocks;
                }
                else
                {
                    Blocks = new BlockCollection<BlockAdv>();
                    Text = GetText();
                    TextBox text = new TextBox();
                    text.Text = Text;
                    string[] texts = Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in texts)
                    {
                        ParagraphAdv tempParagraph = new ParagraphAdv();
                        SpanAdv tempSpan = new SpanAdv();
                        tempSpan.Text = str;
                        tempParagraph.Inlines.Add(tempSpan);
                        Blocks.Add(tempParagraph);
                    }

                    return Blocks;
                }
            }
            else
            {
                Blocks = null;
            }

            return null;
        }
    }

    public static class ExtensionMethods
    {
        public static byte[] GetBytes(this Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            byte[] bytearray = new byte[stream.Length];
            stream.Read(bytearray, 0, bytearray.Length);
            return bytearray;
        }

        internal static MemoryStream ToMemoryStream(this Stream stream)
        {
            MemoryStream result = new MemoryStream();
            stream.CopyTo(result);
            result.Seek(0L, SeekOrigin.Begin);
            return result;
        }

        public static void SetSource(this BitmapImage bitmapImage, Stream imageStream)
        {
#if !WPF 
            bitmapImage.SetSource(imageStream);
#else
            bitmapImage.StreamSource = imageStream.ToMemoryStream();
#endif

        }

#if !(SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        public static void CopyTo(this Stream stream, Stream output)
        {
            const int bufferSize = 0x1000;
            int num;
            byte[] buffer = new byte[bufferSize];
            while ((num = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                output.Write(buffer, 0, num);
            }
        }
#endif

#if !(SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        public static void Clear(this StringBuilder builder)
        {
            if (builder.Length > 0)
            {
                builder.Remove(0, builder.Length);
            }
        }
#endif
    }
}
