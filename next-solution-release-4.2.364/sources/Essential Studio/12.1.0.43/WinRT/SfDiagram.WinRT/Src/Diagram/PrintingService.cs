#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Graphics.Printing;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Printing;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Graphics.Display;
using Windows.Graphics.Printing.OptionDetails;
using Windows.Foundation;
using Windows.ApplicationModel.Core;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Windows.UI.Xaml.Shapes;
using System.ComponentModel;
namespace Syncfusion.UI.Xaml.Diagram
{
    public class PrintingService : INotifyPropertyChanged, ISharedData
    {
        #region PrivateVariables
        Dictionary<int, UIElement> _mpreviewPages = new Dictionary<int, UIElement>();
        private SharedData _mSharedData;
        PrintDocument _mdocument = null;
        IPrintDocumentSource _mdocumentSource = null;
        List<UIElement> _mpages = null;
        private event EventHandler pagesCreated;
        UIElement _mpreviewPage { get; set; }
        Dictionary<int, Rect> _mpageBoundsList = new Dictionary<int, Rect>();
        int _mtotalPageCount = 1;
        int _mvcount = 1;
        int _mhcount = 1;
        string title = string.Empty;
        PrintTask _mprinttask = null;
        #endregion

        #region Properties

        private PrintOrientation _mprintOrientation = PrintOrientation.Portrait;
        private PrintMediaSize _mprintMediaSize = PrintMediaSize.NorthAmericaLetter;
        private string _mtitle = "PrintSfDiagram";
        private Stretch _mcurrentstretch = Stretch.None;
        private Thickness _mprintRect = new Thickness(24);

        public PrintOrientation PrintOrientation
        {
            get
            {
                return _mprintOrientation;
            }
            set
            {
                if (_mprintOrientation != value)
                {
                    _mprintOrientation = value;
                    OnPropertyChanged("PrintOrientation");
                }
            }
        }

        public PrintMediaSize PrintMediaSize
        {
            get
            {
                return _mprintMediaSize;
            }
            set
            {
                if (_mprintMediaSize != value)
                {
                    _mprintMediaSize = value;
                    OnPropertyChanged("PrintMediaSize");
                }
            }
        }

        public string Title
        {
            get
            {
                return _mtitle;
            }
            set
            {
                if (_mtitle != value)
                {
                    _mtitle = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        public Thickness PrintMargin
        {
            get
            {
                return _mprintRect;
            }
            set
            {
                if (_mprintRect != value)
                {
                    _mprintRect = value;
                    OnPropertyChanged("PrintMargin");
                }
            }
        }

        public Stretch PrintPreviewStretch
        {
            get
            {
                return _mcurrentstretch;
            }
            set
            {
                if (_mcurrentstretch != value)
                {
                    _mcurrentstretch = value;
                    OnPropertyChanged("PrintPreviewStretch");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region Register/UnRegister

        public PrintingService()
        {
            _mpages = new List<UIElement>();
        }

        /// <summary>
        /// This function unregisters the app for printing with Windows.
        /// </summary>
        public void UnregisterForPrinting()
        {
            if (_mdocument == null)
                return;

            _mdocument.Paginate -= CreatePrintPreviewPages;
            _mdocument.GetPreviewPage -= GetPrintPreviewPage;
            _mdocument.AddPages -= AddPrintPages;

            // Remove the handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;
            _mSharedData.PrintandExportController.PrintContainer.Children.Clear();
        }

        /// <summary>
        /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        public void RegisterForPrinting()
        {
            _mdocument = GetPrintDocument();
            _mdocumentSource = _mdocument.DocumentSource;
            _mdocument.Paginate += CreatePrintPreviewPages;
            _mdocument.GetPreviewPage += GetPrintPreviewPage;
            _mdocument.AddPages += AddPrintPages;
            PrintManager manager = PrintManager.GetForCurrentView();
            _mtitle = this.Title;
            manager.PrintTaskRequested += PrintTaskRequested;
        }
        #endregion

        #region Virtual methods

        protected virtual IPrintDocumentSource GetPrintDocumentSource()
        {
            return _mdocumentSource;
        }

        protected virtual PrintDocument GetPrintDocument()
        {
            _mdocument = new PrintDocument();
            return _mdocument;
        }

        protected virtual PrintTask GetPrintTask()
        {
            return _mprinttask;
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs </param>
        protected async virtual void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            PrintTask task = GetPrintTask();
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            if (task == null)
            {
                task = args.Request.CreatePrintTask(_mtitle, sourceRequested =>
                {
                    GetNewOptions(task);
                    sourceRequested.SetSource(_mdocumentSource);
                });
                _mprinttask = task;
            }
            else
            {
                _mprinttask = args.Request.CreatePrintTask(_mtitle, sourceRequested =>
                {
                    GetNewOptions(_mprinttask);
                    sourceRequested.SetSource(_mdocumentSource);
                });
            }
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                 () =>
                                 {
                                     _mprinttask.Options.MediaSize = this.PrintMediaSize;
                                     _mprinttask.Options.Orientation = this.PrintOrientation;

                                 });
        }

        protected async virtual Task<BitmapEncoder> GetBitmapEncoder(InMemoryRandomAccessStream ms)
        {
            BitmapEncoder _mencoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms);
            return _mencoder;
        }

        #endregion

        #region Private functions
        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            _mpages.Clear();
            _mpageBoundsList.Clear();
            _mSharedData.PrintandExportController.PrintContainer.Children.Clear();
            _mpreviewPages.Clear();
            PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);
            PrintPageDescription PageDescription = printingOptions.GetPageDescription(0);
            Size msize = new Size(PageDescription.PageSize.Width, PageDescription.PageSize.Height);
            _mhcount = 1;
            _mvcount = 1;
            if (_mcurrentstretch == Stretch.None)
            {
                _mSharedData.PageSettingsController.UpdateRowandColoumn(out _mvcount, out _mhcount, msize.Width, msize.Height);
                if (_mvcount > 0)
                {
                    if (_mhcount > 0)
                    {
                        int i = 0;
                        for (int row = 0; row < _mvcount; row++)
                        {
                            for (int column = 0; column < _mhcount; column++)
                            {
                                if (column == 0)
                                {
                                    if (row == 0)
                                    {
                                        _mpageBoundsList.Add(i, new Rect()
                                        {
                                            X = (column * msize.Width),
                                            Y = (row * msize.Height),
                                            Width = msize.Width,
                                            Height = msize.Height
                                        });
                                    }
                                    else
                                    {
                                        _mpageBoundsList.Add(i, new Rect()
                                        {
                                            X = 0,
                                            Y = ((row * msize.Height) - (PrintMargin.Top * 2)),
                                            Width = msize.Width,
                                            Height = msize.Height
                                        });
                                    }
                                }
                                else
                                {
                                    if (row == 0)
                                    {
                                        _mpageBoundsList.Add(i, new Rect()
                                        {
                                            X = ((column * msize.Width) - (PrintMargin.Left * 2)),
                                            Y = 0,
                                            Width = msize.Width,
                                            Height = msize.Height
                                        });
                                    }
                                    else
                                    {
                                        _mpageBoundsList.Add(i, new Rect()
                                        {
                                            X = ((column * msize.Width) - (PrintMargin.Left * 2)),
                                            Y = ((row * msize.Height) - (PrintMargin.Top * 2)),
                                            Width = msize.Width,
                                            Height = msize.Height
                                        });
                                    }
                                }
                                i += 1;
                            }

                        }
                    }
                    else
                    {
                        for (int row = 0; row < _mvcount; row++)
                        {
                            if (row == 0)
                            {
                                _mpageBoundsList.Add(row, new Rect()
                                {
                                    X = 0,
                                    Y = (row * msize.Height),
                                    Width = msize.Width,
                                    Height = msize.Height
                                });
                            }
                            else
                            {
                                _mpageBoundsList.Add(row, new Rect()
                                {
                                    X = 0,
                                    Y = ((row * msize.Height) - (PrintMargin.Top * 2)),
                                    Width = msize.Width,
                                    Height = msize.Height
                                });
                            }
                        }
                    }
                }
                else if (_mhcount > 0)
                {
                    for (int column = 0; column < _mhcount; column++)
                    {
                        if (column == 0)
                        {
                            _mpageBoundsList.Add(column, new Rect()
                            {
                                X = (column * msize.Width),
                                Y = 0,
                                Width = msize.Width,
                                Height = msize.Height
                            });
                        }
                        else
                        {
                            _mpageBoundsList.Add(column, new Rect()
                            {
                                X = ((column * msize.Width) - (PrintMargin.Left * 2)),
                                Y = 0,
                                Width = msize.Width,
                                Height = msize.Height
                            });
                        }
                    }
                }
                _mtotalPageCount = _mhcount * _mvcount;
            }
            else
            {
                _mpageBoundsList.Add(0, new Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = PageDescription.PageSize.Width,
                    Height = PageDescription.PageSize.Height
                });
                _mtotalPageCount = 1;
            }

            if (pagesCreated != null)
            {
                pagesCreated.Invoke(_mpages, null);
            }

            PrintDocument printDoc = (PrintDocument)sender;
            printDoc.SetPreviewPageCount(_mtotalPageCount, PreviewPageCountType.Intermediate);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        ///     
        async void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            PrintDocument printDoc1 = (PrintDocument)sender;
            try
            {
                await AddOnePrintPreviewPage(e.PageNumber);
                printDoc1.SetPreviewPage(e.PageNumber, _mpreviewPage);
                if (!_mpreviewPages.ContainsKey(e.PageNumber))
                {
                    _mpreviewPages.Add(e.PageNumber, _mpreviewPage);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        async void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            PrintDocument printDoc = (PrintDocument)sender;
            printDoc.InvalidatePreview();
            if (_mpages.Count != _mtotalPageCount)
            {
                if (_mpreviewPages.Count == _mtotalPageCount)
                {
                    UpdatePage(_mpreviewPages);
                }
                else
                {
                    _mpreviewPages = new Dictionary<int, UIElement>();
                    for (int i = 1; i <= _mtotalPageCount; i++)
                    {
                        await AddOnePrintPreviewPage(i);
                        if (!_mpreviewPages.ContainsKey(i))
                        {
                            _mpreviewPages.Add(i, _mpreviewPage);
                        }
                    }
                    if (_mpreviewPages.Count > 0)
                    {
                        UpdatePage(_mpreviewPages);
                    }
                }

            }
            for (int i = 0; i < _mpages.Count; i++)
            {
                printDoc.AddPage(_mpages[i]);
            }
            printDoc.AddPagesComplete();
        }

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in printPreviewPages.
        /// </summary>
        /// <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
        /// <param name="printPageDescription">Printer's page description</param>
        async Task AddOnePrintPreviewPage(int p, bool first = false)
        {
            FrameworkElement page = null;
            PrintPreviewControl p1 = new PrintPreviewControl();
            PrintTask task = GetPrintTask();
            PrintPageDescription PageDescription = task.Options.GetPageDescription((uint)p);
            Rect Bounds = _mpageBoundsList[p - 1];
            Grid g = await _mSharedData.PrintandExportController.RenderDiagramasImage(Bounds, _mvcount, _mhcount, _mcurrentstretch, true);
            var ms = new InMemoryRandomAccessStream();
            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(g);
            var PixelBuffer = await bitmap.GetPixelsAsync();
            var coder = await GetBitmapEncoder(ms);
            coder.BitmapTransform.Bounds = new BitmapBounds() { X = 0, Y = 0, Width = (uint)Bounds.Width, Height = (uint)Bounds.Height };
            coder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth,
                (uint)bitmap.PixelHeight, DisplayInformation.GetForCurrentView().LogicalDpi,
                DisplayInformation.GetForCurrentView().LogicalDpi, PixelBuffer.ToArray());
            await coder.FlushAsync();
            BitmapImage bi = new BitmapImage();
            bi.SetSource(ms);
            (p1 as PrintPreviewControl).PrintSource = bi;
            p1.PageCount = _mtotalPageCount;
            p1.CurrentPageNo = p;
            page = p1;
            page.Width = PageDescription.PageSize.Width;
            page.Height = PageDescription.PageSize.Height;
            _mSharedData.PrintandExportController.PrintContainer.Children.Clear();
            _mSharedData.PrintandExportController.PrintContainer.Children.Add(page);
            _mSharedData.PrintandExportController.PrintContainer.InvalidateMeasure();
            _mSharedData.PrintandExportController.PrintContainer.UpdateLayout();
            _mSharedData.PrintandExportController.PrintContainer.InvalidateMeasure();
            _mpreviewPage = page;
        }

        //Adding the new options using PrintTask
        protected virtual void GetNewOptions(PrintTask task)
        {
            PrintTaskOptionDetails taskoptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(task.Options);
            taskoptions.DisplayedOptions.Clear();
            taskoptions.DisplayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Orientation);
            PrintCustomItemListOptionDetails imagestretch = taskoptions.CreateItemListOption("Image", "Stretch");
            imagestretch.AddItem("None", "None");
            imagestretch.AddItem("Fill", "Fill");
            imagestretch.AddItem("Uniform", "Uniform");
            imagestretch.AddItem("UniformFill", "UniformToFill");
            taskoptions.DisplayedOptions.Add("Image");
            if (_mcurrentstretch != Stretch.None)
            {
                imagestretch.TrySetValue(_mcurrentstretch.ToString());
            }
            taskoptions.OptionChanged += taskoptions_OptionChanged;
        }

        //Option Changed event
        async void taskoptions_OptionChanged(PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs args)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            string optionId = args.OptionId as string;
            if (string.IsNullOrEmpty(optionId))
            {
                if (optionId == null)
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                      () =>
                                      {
                                          UpdateImage(_mcurrentstretch);
                                      });
                }
                return;
            }
            if (optionId.ToString() == "Image")
            {
                string option = (sender.Options["Image"].Value as string);
                switch (option)
                {
                    case "None":
                        UpdateImage(Stretch.None);
                        break;
                    case "Fill":
                        UpdateImage(Stretch.Fill);
                        break;
                    case "Uniform":
                        UpdateImage(Stretch.Uniform);
                        break;
                    case "UniformFill":
                        UpdateImage(Stretch.UniformToFill);
                        break;
                }
            }
            else if (optionId.ToString() == "PageOrientation")
            {
                PrintOrientation option = (PrintOrientation)(sender.Options["PageOrientation"].Value);
                _mprintOrientation = option;
            }
            else
            {
                Windows.Graphics.Printing.PrintMediaSize p = (Windows.Graphics.Printing.PrintMediaSize)(sender.Options["PageMediaSize"].Value);

            }
        }

        //Updating Stretch Option
        private async void UpdateImage(Stretch stretch, bool Other = false)
        {
            _mcurrentstretch = stretch;
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                       () =>
                                       {
                                           _mdocument.InvalidatePreview();
                                       });
        }

        //Adding the PreviewPages into Pages
        private void UpdatePage(Dictionary<int, UIElement> PreviewPages)
        {
            var dic = from key in PreviewPages.Keys orderby key ascending select key;
            foreach (int ui in dic)
            {
                _mpages.Add(PreviewPages[ui]);
            }
        }

        void ISharedData.Init(SharedData shared)
        {
            _mSharedData = shared;
        }

        void IDisposable.Dispose()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
