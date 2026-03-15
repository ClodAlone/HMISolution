using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace KinectControls
{
    public class KinectMultiTouchDevice : TouchDevice
    {
        private static Dictionary<int, KinectMultiTouchDevice> _devices = new Dictionary<int, KinectMultiTouchDevice>();

        public Point Position { get; set; }

        private KinectMultiTouchDevice(int deviceId)
            : base(deviceId)
        {
        }

        public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
        {
            return new TouchPointCollection();
        }

        public override TouchPoint GetTouchPoint(IInputElement relativeTo)
        {
            Point pt = Position;
            if (relativeTo != null && ActiveSource.RootVisual.IsDescendantOf((Visual)relativeTo))
            {
                pt = ActiveSource.RootVisual.TransformToDescendant((Visual)relativeTo).Transform(Position);
            }
            Rect rect = new Rect(pt, new Size(10, 10));
            return new TouchPoint(this, pt, rect, TouchAction.Move);
        }

        // Report TouchDown
        public static void MouseDown(Window window, HandPointEventArgs e)
        {
            var device = GetDevice(e);
            if (!device.IsActive)
            {
                device.SetActiveSource(PresentationSource.FromVisual(window));
                device.Activate();
            }
            device.Position = GetPosition(e);
            System.Diagnostics.Debug.WriteLine("ReportDown from " + e.ID + " Z " + e.Position.Z);
            device.ReportDown();
        }

        // Report TouchMove
        public static void MouseMove(HandPointEventArgs e)
        {
            var device = GetDevice(e);
            // if (device.IsActive)
            {
                device.Position = GetPosition(e);
                device.ReportMove();
            }
        }

        // Report TouchUp
        public static void MouseUp(HandPointEventArgs e)
        {
            //var device = _devices.FirstOrDefault().Value;
            var device = GetDevice(e);
            // if (device.IsActive)
            {
                device.Position = GetPosition(e);
                System.Diagnostics.Debug.WriteLine("ReportUp from " + e.ID + " Z " + e.Position.Z);
                device.ReportUp();
                // device.Deactivate();
            }
        }

        public void Disable()
        {
            if (IsActive)
            {
                ReportUp(); 
                Deactivate();
            }
        }

        public static KinectMultiTouchDevice GetDevice(int deviceID)
        {
            KinectMultiTouchDevice device = null;
            _devices.TryGetValue(deviceID, out device);
            return device;
        }

        private static KinectMultiTouchDevice GetDevice(HandPointEventArgs e)
        {
            KinectMultiTouchDevice device = null;
            if (!_devices.TryGetValue(e.ID, out device))
            {
                device = new KinectMultiTouchDevice(e.ID);
                _devices.Add(e.ID, device);
            }
            return device;
        }

        private static Point GetPosition(HandPointEventArgs e)
        {
            return new Point(e.Position.X, e.Position.Y);
        }
    }
}
