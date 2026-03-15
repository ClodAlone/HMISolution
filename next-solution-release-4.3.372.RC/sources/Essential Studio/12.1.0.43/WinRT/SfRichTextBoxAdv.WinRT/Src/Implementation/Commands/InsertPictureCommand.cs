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
using System.Windows.Input;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    public class InsertPictureCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertPictureCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public InsertPictureCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the insert picture command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            InsertPicture();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
        /// <summary>
        /// Inserts the picture.
        /// </summary>
        private async void InsertPicture()
        {
            try
            {
                FileOpenPicker fileOpenPicker = new FileOpenPicker();
                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.FileTypeFilter.Add(".jpeg");
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.FileTypeFilter.Add(".bmp");
                StorageFile stgFile = await fileOpenPicker.PickSingleFileAsync();

                ImageContainerAdv image = new ImageContainerAdv();
                BitmapImage bitmapImage = new BitmapImage();
                Stream imageStream = await stgFile.OpenStreamForReadAsync();
                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                Stream stream = randomAccessStream.AsStreamForWrite();
                imageStream.Position = 0;
                //Copies the data to random access stream.
                imageStream.CopyTo(stream);
                stream.Position = 0;
                bitmapImage.SetSource(randomAccessStream);
                image.ImageSource = bitmapImage;
                image.ImageBytes = imageStream.GetBytes();
                imageStream.Dispose();
                OwnerControl.Selection.InsertImage(image);
            }
            catch (Exception ex)
            { }
        }
    }
}
