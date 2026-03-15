using DevExpress.UI.Xaml.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using ZXing;

namespace NextAR.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QRReaderView : DXPage
    {
        private MediaCapture _mediaCapture = new MediaCapture();
        private Result _result;
        private bool _continue;

        public QRReaderView()
        {
            this.InitializeComponent();
            Loaded += QRReaderView_Loaded;
            Unloaded += QRReaderView_Unloaded;
        }

        async void QRReaderView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _continue = true;

                var cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                if (cameras.Count < 1)
                {
                    Error.Text = "No camera found, decoding static image";
                    await DecodeStaticResource();
                    return;
                }
                MediaCaptureInitializationSettings settings;
                if (cameras.Count == 1)
                {
                    settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameras[0].Id }; // 0 => front, 1 => back
                }
                else
                {
                    settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameras[1].Id }; // 0 => front, 1 => back
                }

                await _mediaCapture.InitializeAsync(settings);
                VideoCapture.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();

                while (_continue && _result == null)
                {
                    var photoStorageFile = await KnownFolders.PicturesLibrary.CreateFileAsync("scan.jpg", CreationCollisionOption.GenerateUniqueName);
                    await _mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), photoStorageFile);

                    var stream = await photoStorageFile.OpenReadAsync();
                    // initialize with 1,1 to get the current size of the image
                    var writeableBmp = new WriteableBitmap(1, 1);
                    writeableBmp.SetSource(stream);
                    // and create it again because otherwise the WB isn't fully initialized and decoding
                    // results in a IndexOutOfRange
                    writeableBmp = new WriteableBitmap(writeableBmp.PixelWidth, writeableBmp.PixelHeight);
                    stream.Seek(0);
                    writeableBmp.SetSource(stream);

                    _result = ScanBitmap(writeableBmp);

                    await photoStorageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }

                if (_continue)
                {
                    await _mediaCapture.StopPreviewAsync();
                    _mediaCapture = null;
                    VideoCapture.Visibility = Visibility.Collapsed;
                    CaptureImage.Visibility = Visibility.Visible;
                    ScanResult.Content = _result.Text;
                    ScanResult.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Error.Text = ex.Message;
            }

            try
            {
                if (_mediaCapture != null)
                    await _mediaCapture.StopPreviewAsync();
            }
            catch
            {

            }
        }

        private async System.Threading.Tasks.Task DecodeStaticResource()
        {
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\1.jpg");
            var stream = await file.OpenReadAsync();
            // initialize with 1,1 to get the current size of the image
            var writeableBmp = new WriteableBitmap(1, 1);
            writeableBmp.SetSource(stream);
            // and create it again because otherwise the WB isn't fully initialized and decoding
            // results in a IndexOutOfRange
            writeableBmp = new WriteableBitmap(writeableBmp.PixelWidth, writeableBmp.PixelHeight);
            stream.Seek(0);
            writeableBmp.SetSource(stream);
            CaptureImage.Source = writeableBmp;
            VideoCapture.Visibility = Visibility.Collapsed;
            CaptureImage.Visibility = Visibility.Visible;

            _result = ScanBitmap(writeableBmp);
            if (_result != null)
            {
                ScanResult.Content = _result.Text;
                ScanResult.Visibility = Visibility.Visible;
            }
            return;
        }

        private Result ScanBitmap(WriteableBitmap writeableBmp)
        {
            var barcodeReader = new BarcodeReader
            {
                TryHarder = true,
                AutoRotate = true
            };
            var result = barcodeReader.Decode(writeableBmp);

            if (result != null)
            {
                CaptureImage.Source = writeableBmp;
            }

            return result;
        }

        void QRReaderView_Unloaded(object sender, RoutedEventArgs e)
        {
            _continue = false;
        }

        private async void ScanResult_Click(object sender, RoutedEventArgs e)
        {
            var bError = false;
            try
            {
                Uri mapsAppUri = new Uri(String.Format("{0}/Screen.aspx?url={1}", Settings.WebUrl, ScanResult.Content as String));

                await Windows.System.Launcher.LaunchUriAsync(mapsAppUri);
            }
            catch (Exception ex)
            {
                bError = true;
            }

            if (!bError)
                return;

            var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            var msgDialog = new MessageDialog(resourceLoader.GetString("ErrorUri"), resourceLoader.GetString("ErrorUriTitle"));
            await msgDialog.ShowAsync();

            Windows.UI.ApplicationSettings.SettingsPane.Show();
        }
    }
}
