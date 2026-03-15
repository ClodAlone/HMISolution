using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Collections.Specialized;

using Bling;
using Bling.Util;
using Bling.Core;

namespace Bling.WPF {

  public class AnimatedCanvas : Canvas, ICanStart {
    public static readonly GetProperty<AnimatedCanvas, double> AnimateProperty = "Animate".NewProperty<AnimatedCanvas, double>();
    public DoubleBl Animate {
      get { return AnimateProperty[this]; }
      set { Animate.Bind = value; }
    }
    public void Start() {
      Animate.Stop(false);
      Animate.Now = 0;
      Animate.Animate().Duration(1000).Forever().To = 1.0;
    }
    public void Stop() {
      Animate.Stop(true);
    }
  }


  internal class TimerTask : DependencyObject {
    public double Time; // can be safely read outside of the UI thread.
    public static GetProperty<TimerTask, double> TimeProperty = "Time".NewProperty<TimerTask, double>(0);
    public DoubleBl TimeSignal { get { return TimeProperty[this]; } }
    private Action OnZero;
    public TimerTask(BackgroundTimer owner, double initial) : this(owner, initial, () => { }) { }
    private readonly double Initial;
    public DoubleBl Scaled {
      get {
        return TimeSignal / Initial;
      }
    }

    public TimerTask(BackgroundTimer owner, double initial, Action OnZero) {
      Initial = initial;
      Time = initial;
      TimeSignal.Now = initial;
      owner.Tasks.Add(this);
      this.OnZero = OnZero;
    }
    public bool Update(double step) {
      Time = Math.Max(Time - step, 0);
      TimeSignal.Now = Time;
      if (Time == 0) {
        if (OnZero != null) OnZero();
        return true;
      } else return false;
    }
  }



  public class TimeoutManager<OWNER> : ICanStart where OWNER : FrameworkElement {
    private readonly OWNER Owner;
    private readonly BackgroundTimer Timer;
    public TimeoutManager(OWNER Owner, DoubleBl TimeOut, int Step, Action<OWNER> DoTimeout) { 
      this.Owner = Owner;
      {
        Action StepTime = DoubleBl.Assign(Time, LockTime.Condition(0, Time + Step));
        Func<bool> OverTime = (Time >= TimeOut).AsFunc;
        Action ResetTime = DoubleBl.Assign(Time, 0d);
        Timer = new BackgroundTimer(Owner.Dispatcher, Step, () => { }, () => {
          StepTime();
          if (OverTime()) {
            ResetTime();
            DoTimeout(Owner);
          }
        });
      }
    }
    private static readonly GetProperty<OWNER, double> TimeProperty = "Time".NewProperty<OWNER, double>();
    private DoubleBl Time { get { return TimeProperty[Owner]; } set { Time.Bind = value; } }

    private static readonly GetProperty<OWNER, bool> LockTimeProperty = "LockTime".NewProperty<OWNER, bool>(false);
    public BoolBl LockTime { get { return LockTimeProperty[Owner]; } set { LockTime.Bind = value; } }

    public void Start() { Timer.Start();  }
    public void Stop() { Timer.Stop();  }

  }

  public class BackgroundTimer : ICanStart {
    private readonly Dispatcher Dispatcher;
    private Timer timer;
    private readonly Action BackgroundTask;
    private readonly Action UITask;
    internal readonly List<TimerTask> Tasks = new List<TimerTask>();
    private bool busy = false;
    private readonly int Period;
    private int Step;
    private int Last;
    public BackgroundTimer(Dispatcher Dispatcher, int milliPeriod, Action BackgroundTask, Action UITask) {
      this.Dispatcher = Dispatcher;
      this.BackgroundTask = BackgroundTask;
      this.UITask = UITask;
      this.Period = milliPeriod;
      this.Step = Period;
      this.Last = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
    }
    public void Start() {
      timer = new Timer(new TimerCallback((x) => ((BackgroundTimer)x).BackgroundTask0()), this, Period, Period);
    }
    public void Stop() {
      if (timer != null) {
        timer.Dispose();
        timer = null;
      }
    }


    public DoubleBl Task(double millis) {
      var task = new TimerTask(this, millis);
      return task.Scaled;
    }
    public DoubleBl Task(double millis, Action onZero) {
      var task = new TimerTask(this, millis, onZero);
      return task.Scaled;
    }
    private void BackgroundTask0() {
      if (busy) return;
      busy = true;
      var now = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
      this.Step = now - Last;
      Last = now;

      BackgroundTask();
      Dispatcher.BeginInvoke(new Action<BackgroundTimer>(timer => {
        if (timer.Tasks.Count > 0) { //  back in the UI thread!
          int i = 0;
          while (i < timer.Tasks.Count) {
            if (timer.Tasks[i].Update(timer.Step)) {
              timer.Tasks.RemoveAt(i);
            } else i++;
          }
        }
        timer.UITask();
        busy = false;
      }), this);
    }
  }
  public class SwapRenderer : DependencyObject {
    private RenderTargetBitmap bufB;
    public readonly int Width;
    public readonly int Height;
    public ImageSource Output() {
      return image.Source;
      //return image.Source.Map<ImageSource>(d => d);
    }
    public readonly Image image = new Image();
    public SwapRenderer(int width, int height, ShaderEffect effect) {
      this.Width = width;
      this.Height = height;
      var bufA = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
      bufB = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
      image.Source = bufA;
      image.Measure(new Size(Width, Height));
      image.Arrange(new Rect(0, 0, Width, Height));
      image.Effect = effect;
    }
    public void Swap(Visual visual) {
      var bufA = (RenderTargetBitmap)image.Source;
      image.Source = bufB;
      if (visual != null) bufB.Render(visual);

      image.Measure(new Size(Width, Height));
      image.Arrange(new Rect(0, 0, Width, Height));
      bufA.Render(image);
      bufB = bufA;
    }
    private DispatcherTimer timer;
    public void Go(long millis, Visual visual) {
      timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(millis) };
      timer.Tick += (x, y) => Swap(visual);
      timer.Start();
    }
  }
  public static class Utils {
    public static Point TransformToScreen(Point point, Visual relativeTo) {
      HwndSource hwndSource = PresentationSource.FromVisual(relativeTo) as HwndSource;
      Visual root = hwndSource.RootVisual;
      // Translate the point from the visual to the root.
      GeneralTransform transformToRoot = relativeTo.TransformToAncestor(root);
      Point pointRoot = transformToRoot.Transform(point);
      // Transform the point from the root to client coordinates.
      Matrix m = Matrix.Identity;
      Transform transform = VisualTreeHelper.GetTransform(root);
      if (transform != null) {
        m = Matrix.Multiply(m, transform.Value);
      }
      Vector offset = VisualTreeHelper.GetOffset(root);
      m.Translate(offset.X, offset.Y);
      Point pointClient = m.Transform(pointRoot);
      // Convert from “device-independent pixels” into pixels.
      pointClient = hwndSource.CompositionTarget.TransformToDevice.Transform(pointClient);
      POINT pointClientPixels = new POINT();
      pointClientPixels.x = (0 < pointClient.X) ? (int)(pointClient.X + 0.5) : (int)(pointClient.X - 0.5);
      pointClientPixels.y = (0 < pointClient.Y) ? (int)(pointClient.Y + 0.5) : (int)(pointClient.Y - 0.5);
      // Transform the point into screen coordinates.
      POINT pointScreenPixels = pointClientPixels;
      ClientToScreen(hwndSource.Handle, pointScreenPixels);
      return new Point(pointScreenPixels.x, pointScreenPixels.y);
    }
    [StructLayout(LayoutKind.Sequential)]
    public class POINT {
      public int x = 0;
      public int y = 0;
    }
    [DllImport("User32", EntryPoint = "ClientToScreen", SetLastError = true,
    ExactSpelling = true, CharSet = CharSet.Auto)]
    private static extern int ClientToScreen(IntPtr hWnd, [In, Out] POINT pt);
  }


}