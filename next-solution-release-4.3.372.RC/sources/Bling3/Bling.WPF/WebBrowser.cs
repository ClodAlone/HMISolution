using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using forms = System.Windows.Forms;
using drawing = System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
namespace Bling.Browser {
  public class Page : IDisposable, Bling.Util.ICanStart {
    private readonly Dispatcher ParentDispatcher;
    private Dispatcher Child;
    private forms.WebBrowser Browser;
    public readonly WriteableBitmap Bitmap;
    private readonly drawing.Graphics BitmapGraphics;
    public void Start() {
      if (Timer != null && Timer.IsEnabled) Timer.Stop();
    }
    public void Stop() {
      if (Timer != null && !Timer.IsEnabled) Timer.Start();
    }
    private readonly double updateMillis;
    public Page(Visual window, Point BrowserSize, double updateMillis) {
      Width = (int)BrowserSize.X;
      Height = (int)BrowserSize.Y;
      this.updateMillis = updateMillis;

      this.ParentDispatcher = window.Dispatcher;
      System.Windows.Media.Matrix m = PresentationSource.FromVisual(window).CompositionTarget.TransformToDevice;
      double dpi_x = m.M11 * 96.0;
      double dpi_y = m.M22 * 96.0;
      Bitmap = new WriteableBitmap(Width, Height, dpi_x, dpi_y, PixelFormats.Pbgra32, null);
      drawing.Bitmap b = new drawing.Bitmap(Width, Height, Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, Bitmap.BackBuffer);
      BitmapGraphics = System.Drawing.Graphics.FromImage(b);
      BitmapGraphics.InterpolationMode = drawing.Drawing2D.InterpolationMode.NearestNeighbor;
      BitmapGraphics.SmoothingMode = drawing.Drawing2D.SmoothingMode.None;

      Thread thread = new Thread(() => {
        this.Child = Dispatcher.CurrentDispatcher;
        this.Browser = new forms.WebBrowser() {
          Width = Width, Height = Height,
          ScrollBarsEnabled = false, ScriptErrorsSuppressed = true,
        };
        Browser.NewWindow += (x, y) => y.Cancel = true;
        Browser.DocumentCompleted += OnDocumentCompleted;
        Browser.Navigated += (x, y) => {
          Browser.Document.Window.Error += (_x, _y) => { _y.Handled = true; };
          var Url = Browser.Url;
          ParentDispatcher.BeginInvoke(DownloadStart, this, Url);
        };
        System.Windows.Threading.Dispatcher.Run();
      }) {
        Name = "WebPageThread" + (GetHashCode() % 100),
      };

      thread.IsBackground = true;
      thread.SetApartmentState(ApartmentState.STA);
      thread.Start();
      // wait 100 milliseconds, give thread a chance to start!
      while (Child == null || Browser == null)
        thread.Join(TimeSpan.FromMilliseconds(100));
    }
    public void Navigate(Uri uri) {
      //Parent.CheckAccess().Assert();
      Child.BeginInvoke(new Action<Page, Uri>((page, url) => {
        if (Browser.IsBusy) Browser.Stop();
        Browser.Width = Width;
        Browser.Height = Height;
        Browser.Url = url;
      }), this, uri);
    }
    public void Dispose() {
      Browser.Dispose();
      Child.BeginInvokeShutdown(DispatcherPriority.Normal);
    }

    private bool Busy = false;
    private bool Initialized = false;
    private void Capture() {
      Busy = true;
      var Current = Dispatcher.CurrentDispatcher;
      if (Current == ParentDispatcher) throw new Exception();
      if (Current != Child) throw new Exception();
      ParentDispatcher.Invoke(new Action<Page>(page => {
        page.Bitmap.Lock();
      }), this);
      {
        BitmapGraphics.Clear(System.Drawing.Color.Black);
        var device = new NativeMethods.tagDVTARGETDEVICE();
        var bounds = new NativeMethods.COMRECT(0, 0, Width, Height);
        var view = Browser.ActiveXInstance as NativeMethods.IViewObject;
        view.Draw(1, -1, (IntPtr)0, null, (IntPtr)0, (IntPtr)BitmapGraphics.GetHdc(), bounds, bounds, (IntPtr)0, 0);
        BitmapGraphics.ReleaseHdc();
      }
      var NoticedWidth = Browser.Width;
      ParentDispatcher.Invoke(new Action<Page>(page => {
        page.Bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
        page.Bitmap.Unlock();
        DownloadCompleted(page, NoticedWidth);
      }), this);
      {
        Busy = false;
        if (!Initialized) {
          Initialized = true;
          if (updateMillis > 0) {
            Timer = new DispatcherTimer() {
              Interval = TimeSpan.FromMilliseconds(updateMillis), 
            };
            Timer.Tick += (x1, y1) => {
              if (!Busy) Capture();
            };
            Timer.Start();
          }
        }
      }
      if (Busy) throw new Exception();
    }
    private DispatcherTimer Timer = null;

    private void OnDocumentCompleted(object sender, forms.WebBrowserDocumentCompletedEventArgs e) {
      if (Busy || Browser.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
        return;
      Capture();
    }

    public event Action<Page, double> DownloadCompleted = (page, width) => { };
    public event Action<Page, Uri> DownloadStart = (page, uri) => { };

    public void DoClick(Point p) {
      var Current = Dispatcher.CurrentDispatcher;
      if (Current != ParentDispatcher) throw new Exception();
      if (Current == Child) throw new Exception();

      if (Initialized)
        Child.BeginInvoke(new Action<Page, Point>((page, p0) =>
           NativeMethods.SimulateClick.DoClick(page.Browser, (int)p0.X, (int)p0.Y)), this, p);
    }
    public void DoKeyDown(Key k) {
      var Current = Dispatcher.CurrentDispatcher;
      if (Current != ParentDispatcher) throw new Exception();
      if (Current == Child) throw new Exception();
      /*
      if (Initialized)
        Child.BeginInvoke(new Action<Page, Point>((page, p0) =>
           NativeMethods.SimulateClick.DoClick(page.Browser, (int)p0.X, (int)p0.Y)), this, p);
       * */
    }

    private void DoMove(Point p) {
      if (Initialized)
        Child.BeginInvoke(new Action<Page, Point>((page, p0) =>
           NativeMethods.SimulateMove.DoMove(page.Browser, (int)p0.X, (int)p0.Y)), this, p);
    }

    public void GoBack() {
      if (Initialized)
        Child.BeginInvoke(new Action<Page>((page) => page.Browser.GoBack()), this);
    }
    public void GoForward() {
      if (Initialized)
        Child.BeginInvoke(new Action<Page>((page) => page.Browser.GoForward()), this);
    }
    private readonly int Width /* = 1000 */;
    private readonly int Height /* = 3000 */;
  }
}