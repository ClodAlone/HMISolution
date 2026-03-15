#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;

#if WPF
using Microsoft.Win32;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
    public class InsertPictureCommand :CommandBase
    {
        public InsertPictureCommand(RichTextBoxAdv rich)
            : base(rich)
        {

        }

        protected override void ExecuteCommand(object parameter)
        {
            OpenFileDialog opendialog = new OpenFileDialog()
            {
                Filter = "Images (*.png,*.jpeg,*.jpg)|*.png;*.jpeg;*.jpg",
                FilterIndex = 1
            };
            if (opendialog.ShowDialog() != null)
            {
                BitmapImage bitmapimage = new BitmapImage();
                ImageContainerAdv imagadv;
                imagadv = new ImageContainerAdv();
                if (opendialog.File != null)
                {
                    Stream stream;
                    FileInfo fileinfo = opendialog.File;
                    if (fileinfo != null)
                    {
                        stream = fileinfo.OpenRead();
                        bitmapimage.SetSource(stream);
                        imagadv = new ImageContainerAdv();
                        imagadv.ImageBytes = stream.GetBytes();
                        imagadv.ImageSource = bitmapimage;
                        imagadv.Height = bitmapimage.PixelHeight;
                        imagadv.Width = bitmapimage.PixelWidth;
                        if (AssociatedRichEditor != null)
                        {
                            AssociatedRichEditor.InsertInlineInParagraph(imagadv);
                            AssociatedRichEditor.Focus();
                        }
                    }
                }
            }
        }

    }
}
