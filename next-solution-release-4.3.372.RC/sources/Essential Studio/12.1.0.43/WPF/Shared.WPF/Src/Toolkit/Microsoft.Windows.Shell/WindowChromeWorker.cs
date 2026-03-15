#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

namespace Syncfusion.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Standard;  

    using HANDLE_MESSAGE = System.Collections.Generic.KeyValuePair<Standard.WM, Standard.MessageHandler>;
    using System.Windows.Controls;
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum NonClientFrameEdges
    {
        /// <summary>
        /// 
        /// </summary>
        Bottom = 8,
        /// <summary>
        /// 
        /// </summary>
        Left = 1,
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        Right = 4,
        /// <summary>
        /// 
        /// </summary>
        Top = 2
    }
    /// <summary>
    /// 
    /// </summary>
    public enum ResizeGripDirection
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        TopLeft,
        /// <summary>
        /// 
        /// </summary>
        Top,
        /// <summary>
        /// 
        /// </summary>
        TopRight,
        /// <summary>
        /// 
        /// </summary>
        Right,
        /// <summary>
        /// 
        /// </summary>
        BottomRight,
        /// <summary>
        /// 
        /// </summary>
        Bottom,
        /// <summary>
        /// 
        /// </summary>
        BottomLeft,
        /// <summary>
        /// 
        /// </summary>
        Left
    }

    internal class WindowChromeWorker : DependencyObject
    {
        // Delegate signature used for Dispatcher.BeginInvoke.
        private delegate void _Action();

        #region Fields

        private const SWP _SwpFlags = SWP.FRAMECHANGED | SWP.NOSIZE | SWP.NOMOVE | SWP.NOZORDER | SWP.NOOWNERZORDER | SWP.NOACTIVATE;

        private readonly List<HANDLE_MESSAGE> _messageTable;

        /// <summary>The Window that's chrome is being modified.</summary>
        private Window _window;
        /// <summary>Underlying HWND for the _window.</summary>
        private IntPtr _hwnd;
        private HwndSource _hwndSource = null;
        private bool _isHooked = false;

        // These fields are for tracking workarounds for WPF 3.5SP1 behaviors.
        private bool _isFixedUp = false;
        private bool _isUserResizing = false;
        private bool _hasUserMovedWindow = false;
        private Point _windowPosAtStartOfUserMove = default(Point);

        // Field to track attempts to force off Device Bitmaps on Win7.
        private int _blackGlassFixupAttemptCount;

        /// <summary>Object that describes the current modifications being made to the chrome.</summary>
        private WindowChrome _chromeInfo;

        // Keep track of this so we can detect when we need to apply changes.  Tracking these separately
        // as I've seen using just one cause things to get enough out of sync that occasionally the caption will redraw.
        private WindowState _lastRoundingState;
        private WindowState _lastMenuState;
        private bool _isGlassEnabled;

        #endregion

        public WindowChromeWorker()
        {
            
            _messageTable = new List<HANDLE_MESSAGE>
            {
                new HANDLE_MESSAGE(WM.SETTEXT,               _HandleSetTextOrIcon),
                new HANDLE_MESSAGE(WM.SETICON,               _HandleSetTextOrIcon),
                new HANDLE_MESSAGE(WM.NCACTIVATE,            _HandleNCActivate),
                new HANDLE_MESSAGE(WM.NCCALCSIZE,            _HandleNCCalcSize),
                new HANDLE_MESSAGE(WM.NCHITTEST,             _HandleNCHitTest),
                new HANDLE_MESSAGE(WM.NCRBUTTONUP,           _HandleNCRButtonUp),
                new HANDLE_MESSAGE(WM.SIZE,                  _HandleSize),
                new HANDLE_MESSAGE(WM.WINDOWPOSCHANGED,      _HandleWindowPosChanged),
                new HANDLE_MESSAGE(WM.DWMCOMPOSITIONCHANGED, _HandleDwmCompositionChanged), 
            };

            if (Utility.IsPresentationFrameworkVersionLessThan4)
            {
                _messageTable.AddRange(new[] 
                {
                   new HANDLE_MESSAGE(WM.SETTINGCHANGE,         _HandleSettingChange),
                   new HANDLE_MESSAGE(WM.ENTERSIZEMOVE,         _HandleEnterSizeMove),
                   new HANDLE_MESSAGE(WM.EXITSIZEMOVE,          _HandleExitSizeMove),
                   new HANDLE_MESSAGE(WM.MOVE,                  _HandleMove),
                });
            }
        }

        public void SetWindowChrome(WindowChrome newChrome)
        {
            VerifyAccess();
            Assert.IsNotNull(_window);

            if (newChrome == _chromeInfo)
            {
                // Nothing's changed.
                return;
            }

            if (_chromeInfo != null)
            {
                _chromeInfo.PropertyChangedThatRequiresRepaint -= _OnChromePropertyChangedThatRequiresRepaint;
            }

            _chromeInfo = newChrome;
            if (_chromeInfo != null)
            {
                _chromeInfo.PropertyChangedThatRequiresRepaint += _OnChromePropertyChangedThatRequiresRepaint;
            }

            _ApplyNewCustomChrome();
        }

        private void _OnChromePropertyChangedThatRequiresRepaint(object sender, EventArgs e)
        {
            _UpdateFrameState(true);
        }

        public static readonly DependencyProperty WindowChromeWorkerProperty = DependencyProperty.RegisterAttached(
            "WindowChromeWorker",
            typeof(WindowChromeWorker),
            typeof(WindowChromeWorker),
            new PropertyMetadata(null, _OnChromeWorkerChanged));

        private static void _OnChromeWorkerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var w = (Window)d;
            var cw = (WindowChromeWorker)e.NewValue;

            // The WindowChromeWorker object should only be set on the window once, and never to null.
            Assert.IsNotNull(w);
            Assert.IsNotNull(cw);
            Assert.IsNull(cw._window);

            cw._SetWindow(w);
        }

        private void _SetWindow(Window window)
        {
            Assert.IsNull(_window);
            Assert.IsNotNull(window);

            _window = window;

            // There are potentially a couple funny states here.
            // The window may have been shown and closed, in which case it's no longer usable.
            // We shouldn't add any hooks in that case, just exit early.
            // If the window hasn't yet been shown, then we need to make sure to remove hooks after it's closed.
            _hwnd = new WindowInteropHelper(_window).Handle;

            if (Utility.IsPresentationFrameworkVersionLessThan4)
            {
                // On older versions of the framework the client size of the window is incorrectly calculated.
                // We need to modify the template to fix this on behalf of the user.
                Utility.AddDependencyPropertyChangeListener(this._window, Control.TemplateProperty, new EventHandler(this._OnWindowPropertyChangedThatRequiresTemplateFixup));
                Utility.AddDependencyPropertyChangeListener(this._window, FrameworkElement.FlowDirectionProperty, new EventHandler(this._OnWindowPropertyChangedThatRequiresTemplateFixup));
            }

            _window.Closed += _UnsetWindow;

            // Use whether we can get an HWND to determine if the Window has been loaded.
            if (IntPtr.Zero != _hwnd)
            {
                // We've seen that the HwndSource can't always be retrieved from the HWND, so cache it early.
                // Specifically it seems to sometimes disappear when the OS theme is changing.
                _hwndSource = HwndSource.FromHwnd(_hwnd);
                Assert.IsNotNull(_hwndSource);
                _window.ApplyTemplate();

                if (_chromeInfo != null)
                {
                    _ApplyNewCustomChrome();
                }
            }
            else
            {
                _window.SourceInitialized += (sender, e) =>
                {
                    _hwnd = new WindowInteropHelper(_window).Handle;
                    Assert.IsNotDefault(_hwnd);
                    _hwndSource = HwndSource.FromHwnd(_hwnd);
                    Assert.IsNotNull(_hwndSource);

                    if (_chromeInfo != null)
                    {
                        _ApplyNewCustomChrome();
                    }
                };
            }
        }

        private void _UnsetWindow(object sender, EventArgs e)
        {
            if (Utility.IsPresentationFrameworkVersionLessThan4)
            {
                Utility.RemoveDependencyPropertyChangeListener(this._window, Control.TemplateProperty, new EventHandler(this._OnWindowPropertyChangedThatRequiresTemplateFixup));
                Utility.RemoveDependencyPropertyChangeListener(this._window, FrameworkElement.FlowDirectionProperty, new EventHandler(this._OnWindowPropertyChangedThatRequiresTemplateFixup));
            }

            if (_chromeInfo != null)
            {
                _chromeInfo.PropertyChangedThatRequiresRepaint -= _OnChromePropertyChangedThatRequiresRepaint;
            }

            _RestoreStandardChromeState(true);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static WindowChromeWorker GetWindowChromeWorker(Window window)
        {
            Verify.IsNotNull(window, "window");
            return (WindowChromeWorker)window.GetValue(WindowChromeWorkerProperty);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetWindowChromeWorker(Window window, WindowChromeWorker chrome)
        {
            Verify.IsNotNull(window, "window");
            window.SetValue(WindowChromeWorkerProperty, chrome);
        }

        private void _OnWindowPropertyChangedThatRequiresTemplateFixup(object sender, EventArgs e)
        {
            Assert.IsTrue(Utility.IsPresentationFrameworkVersionLessThan4);

            if (_chromeInfo != null && _hwnd != IntPtr.Zero)
            {
                // Assume that when the template changes it's going to be applied.
                // We don't have a good way to externally hook into the template
                // actually being applied, so we asynchronously post the fixup operation
                // at Loaded priority, so it's expected that the visual tree will be
                // updated before _FixupFrameworkIssues is called.
                _window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new _Action(this._FixupTemplateIssues));
            }
        }

        private void _ApplyNewCustomChrome()
        {
            if (_hwnd == IntPtr.Zero)
            {
                // Not yet hooked.
                return;
            }

            if (_chromeInfo == null)
            {
                _RestoreStandardChromeState(false);
                return;
            }

            if (!_isHooked)
            {
                _hwndSource.AddHook(_WndProc);
                _isHooked = true;
            }

            _FixupTemplateIssues();

            // Force this the first time.
            _UpdateSystemMenu(_window.WindowState);
            _UpdateFrameState(true);

            NativeMethods.SetWindowPos(this._hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP.NOOWNERZORDER | SWP.DRAWFRAME | SWP.NOACTIVATE | SWP.NOZORDER | SWP.NOMOVE | SWP.NOSIZE);
        }

        private void _FixupTemplateIssues()
        {
            if (this._window.Template != null)
            {
                if (VisualTreeHelper.GetChildrenCount(this._window) == 0)
                {
                    this._window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new _Action(this._FixupTemplateIssues));
                }
                else
                {
                    Thickness templateFixupMargin = new Thickness();
                    Transform templateFixupTransform = null;
                    FrameworkElement rootElement = (FrameworkElement)VisualTreeHelper.GetChild(this._window, 0);
                    if (this._chromeInfo.NonClientFrameEdges != NonClientFrameEdges.None)
                    {
                        if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 2))
                        {
                            templateFixupMargin.Top -= SystemParameters2.Current.WindowResizeBorderThickness.Top;
                        }
                        if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 1))
                        {
                            templateFixupMargin.Left -= SystemParameters2.Current.WindowResizeBorderThickness.Left;
                        }
                        if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 8))
                        {
                            templateFixupMargin.Bottom -= SystemParameters2.Current.WindowResizeBorderThickness.Bottom;
                        }
                        if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 4))
                        {
                            templateFixupMargin.Right -= SystemParameters2.Current.WindowResizeBorderThickness.Right;
                        }
                    }
                    if (Utility.IsPresentationFrameworkVersionLessThan4)
                    {
                        RECT rcWindow = Standard.NativeMethods.GetWindowRect(this._hwnd);
                        RECT rcAdjustedClient = this._GetAdjustedWindowRect(rcWindow);
                        Rect rcLogicalWindow = DpiHelper.DeviceRectToLogical(new Rect((double)rcWindow.Left, (double)rcWindow.Top, (double)rcWindow.Width, (double)rcWindow.Height));
                        Rect rcLogicalClient = DpiHelper.DeviceRectToLogical(new Rect((double)rcAdjustedClient.Left, (double)rcAdjustedClient.Top, (double)rcAdjustedClient.Width, (double)rcAdjustedClient.Height));
                        if (!Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 1))
                        {
                            templateFixupMargin.Right -= rcLogicalWindow.Left - rcLogicalClient.Left;
                        }
                        if (!Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 4))
                        {
                            templateFixupMargin.Right += rcLogicalWindow.Right - rcLogicalClient.Right;
                        }
                        if (!Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 2))
                        {
                            templateFixupMargin.Bottom -= rcLogicalWindow.Top - rcLogicalClient.Top;
                        }
                        if (!Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 8))
                        {
                            templateFixupMargin.Bottom += rcLogicalWindow.Bottom - rcLogicalClient.Bottom;
                        }
                        if (this._window.FlowDirection == FlowDirection.RightToLeft)
                        {
                            Thickness nonClientThickness = new Thickness(rcLogicalWindow.Left - rcLogicalClient.Left, rcLogicalWindow.Top - rcLogicalClient.Top, rcLogicalClient.Right - rcLogicalWindow.Right, rcLogicalClient.Bottom - rcLogicalWindow.Bottom);
                            templateFixupTransform = new MatrixTransform(1.0, 0.0, 0.0, 1.0, -(nonClientThickness.Left + nonClientThickness.Right), 0.0);
                        }
                        else
                        {
                            templateFixupTransform = null;
                        }
                        rootElement.RenderTransform = templateFixupTransform;
                    }
                    rootElement.Margin = templateFixupMargin;
                    _chromeInfo.OnMarginChanged(_chromeInfo, new MarginChangedEventArgs(templateFixupMargin));
                    if (Utility.IsPresentationFrameworkVersionLessThan4 && !this._isFixedUp)
                    {
                        this._hasUserMovedWindow = false;
                        this._window.StateChanged += new EventHandler(this._FixupRestoreBounds);
                        this._isFixedUp = true;
                    }
                }
            }
        }

        // There was a regression in DWM in Windows 7 with regard to handling WM_NCCALCSIZE to effect custom chrome.
        // When windows with glass are maximized on a multimonitor setup the glass frame tends to turn black.
        // Also when windows are resized they tend to flicker black, sometimes staying that way until resized again.
        //
        // This appears to be a bug in DWM related to device bitmap optimizations.  At least on RTM Win7 we can
        // evoke a legacy code path that bypasses the bug by calling an esoteric DWM function.  This doesn't affect
        // the system, just the application.
        // WPF also tends to call this function anyways during animations, so we're just forcing the issue
        // consistently and a bit earlier.
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void _FixupWindows7Issues()
        {
            if (_blackGlassFixupAttemptCount > 5)
            {
                // Don't keep trying if there's an endemic problem with this.
                return;
            }

            if (Utility.IsOSWindows7OrNewer && NativeMethods.DwmIsCompositionEnabled())
            {
                ++_blackGlassFixupAttemptCount;

                bool success = false;
                try
                {
                    DWM_TIMING_INFO? dti = NativeMethods.DwmGetCompositionTimingInfo(_hwnd);
                    success = dti != null;
                }
                catch (Exception)
                {
                    // We aren't sure of all the reasons this could fail.
                    // If we find new ones we should consider making the NativeMethod swallow them as well.
                    // Since we have a limited number of retries and this method isn't actually critical, just repost.

                    // Disabling this for the published code to reduce debug noise.  This will get compiled away for retail binaries anyways.
                    //Assert.Fail(e.Message);
                }

                // NativeMethods.DwmGetCompositionTimingInfo swallows E_PENDING.
                // If the call wasn't successful, try again later.
                if (!success)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (_Action)_FixupWindows7Issues);
                }
                else
                {
                    // Reset this.  We will want to force this again if DWM composition changes.
                    _blackGlassFixupAttemptCount = 0;
                }
            }
        }

        private void _FixupRestoreBounds(object sender, EventArgs e)
        {
            Assert.IsTrue(Utility.IsPresentationFrameworkVersionLessThan4);
            if (_window.WindowState == WindowState.Maximized || _window.WindowState == WindowState.Minimized)
            {
                // Old versions of WPF sometimes force their incorrect idea of the Window's location
                // on the Win32 restore bounds.  If we have reason to think this is the case, then
                // try to undo what WPF did after it has done its thing.
                if (_hasUserMovedWindow)
                {
                    _hasUserMovedWindow = false;
                    WINDOWPLACEMENT wp = NativeMethods.GetWindowPlacement(_hwnd);

                    RECT adjustedDeviceRc = _GetAdjustedWindowRect(new RECT { Bottom = 100, Right = 100 });
                    Point adjustedTopLeft = DpiHelper.DevicePixelsToLogical(
                        new Point(
                            wp.rcNormalPosition.Left - adjustedDeviceRc.Left,
                            wp.rcNormalPosition.Top - adjustedDeviceRc.Top));
                    
                    _window.Top = adjustedTopLeft.Y;
                    _window.Left = adjustedTopLeft.X;
                }
            }
        }

        private RECT _GetAdjustedWindowRect(RECT rcWindow)
        {
            // This should only be used to work around issues in the Framework that were fixed in 4.0
            Assert.IsTrue(Utility.IsPresentationFrameworkVersionLessThan4);

            var style = (WS)NativeMethods.GetWindowLongPtr(_hwnd, GWL.STYLE);
            var exstyle = (WS_EX)NativeMethods.GetWindowLongPtr(_hwnd, GWL.EXSTYLE);

            return NativeMethods.AdjustWindowRectEx(rcWindow, style, false, exstyle);
        }

        private HT _GetHTFromResizeGripDirection(ResizeGripDirection direction)
        {
            bool compliment = this._window.FlowDirection == FlowDirection.RightToLeft;
            switch (direction)
            {
                case ResizeGripDirection.TopLeft:
                    return (compliment ? HT.TOPRIGHT : HT.TOPLEFT);

                case ResizeGripDirection.Top:
                    return HT.TOP;

                case ResizeGripDirection.TopRight:
                    return (compliment ? HT.TOPLEFT : HT.TOPRIGHT);

                case ResizeGripDirection.Right:
                    return (compliment ? HT.LEFT : HT.RIGHT);

                case ResizeGripDirection.BottomRight:
                    return (compliment ? HT.BOTTOMLEFT : HT.BOTTOMRIGHT);

                case ResizeGripDirection.Bottom:
                    return HT.BOTTOM;

                case ResizeGripDirection.BottomLeft:
                    return (compliment ? HT.BOTTOMRIGHT : HT.BOTTOMLEFT);

                case ResizeGripDirection.Left:
                    return (compliment ? HT.RIGHT : HT.LEFT);
            }
            return HT.NOWHERE;
        }

        // Windows tries hard to hide this state from applications.
        // Generally you can tell that the window is in a docked position because the restore bounds from GetWindowPlacement
        // don't match the current window location and it's not in a maximized or minimized state.
        // Because this isn't doced or supported, it's also not incredibly consistent.  Sometimes some things get updated in
        // different orders, so this isn't absolutely reliable.
        private bool _IsWindowDocked
        {
            get
            {
                // We're only detecting this state to work around .Net 3.5 issues.
                // This logic won't work correctly when those issues are fixed.
                Assert.IsTrue(Utility.IsPresentationFrameworkVersionLessThan4);

                if (_window.WindowState != WindowState.Normal)
                {
                    return false;
                }

                RECT adjustedOffset = _GetAdjustedWindowRect(new RECT { Bottom = 100, Right = 100 });
                Point windowTopLeft = new Point(_window.Left, _window.Top);
                windowTopLeft -= (Vector)DpiHelper.DevicePixelsToLogical(new Point(adjustedOffset.Left, adjustedOffset.Top));

                return _window.RestoreBounds.Location != windowTopLeft;
            }
        }

        #region WindowProc and Message Handlers

        private IntPtr _WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Only expecting messages for our cached HWND.
            Assert.AreEqual(hwnd, _hwnd);

            var message = (WM)msg;
            foreach (var handlePair in _messageTable)
            {
                if (handlePair.Key == message)
                {
                    return handlePair.Value(message, wParam, lParam, out handled);
                }
            }
            return IntPtr.Zero;
        }

        private IntPtr _HandleSetTextOrIcon(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            bool modified = this._ModifyStyle(WS.TILED | WS.VISIBLE, WS.TILED);

            // Setting the caption text and icon cause Windows to redraw the caption.
            // Letting the default WndProc handle the message without the WS_VISIBLE
            // style applied bypasses the redraw.
            IntPtr lRet = NativeMethods.DefWindowProc(_hwnd, uMsg, wParam, lParam);

            // Put back the style we removed.
            if (modified)
            {
                this._ModifyStyle(WS.TILED, WS.TILED | WS.VISIBLE);
            }
            handled = true;
            return lRet;
        }

        private IntPtr _HandleNCActivate(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // Despite MSDN's documentation of lParam not being used,
            // calling DefWindowProc with lParam set to -1 causes Windows not to draw over the caption.

            // Directly call DefWindowProc with a custom parameter
            // which bypasses any other handling of the message.
            IntPtr lRet = NativeMethods.DefWindowProc(_hwnd, WM.NCACTIVATE, wParam, new IntPtr(-1));
            handled = true;
            return lRet;
        }

        private IntPtr _HandleNCCalcSize(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // lParam is an [in, out] that can be either a RECT* (wParam == FALSE) or an NCCALCSIZE_PARAMS*.
            // Since the first field of NCCALCSIZE_PARAMS is a RECT and is the only field we care about
            // we can unconditionally treat it as a RECT.

            // Since we always want the client size to equal the window size, we can unconditionally handle it
            // without having to modify the parameters.
            if (this._chromeInfo.NonClientFrameEdges != NonClientFrameEdges.None)
            {
                Thickness windowResizeBorderThicknessDevice = DpiHelper.LogicalThicknessToDevice(SystemParameters2.Current.WindowResizeBorderThickness);
                RECT rcClientArea = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 2))
                {
                    rcClientArea.Top += (int)windowResizeBorderThicknessDevice.Top;
                }
                if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 1))
                {
                    rcClientArea.Left += (int)windowResizeBorderThicknessDevice.Left;
                }
                if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 8))
                {
                    rcClientArea.Bottom -= (int)windowResizeBorderThicknessDevice.Bottom;
                }
                if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 4))
                {
                    rcClientArea.Right -= (int)windowResizeBorderThicknessDevice.Right;
                }
                Marshal.StructureToPtr(rcClientArea, lParam, false);
            }
                      
            handled = true;
            return new IntPtr((int)WVR.REDRAW);
        }

        private IntPtr _HandleNCHitTest(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            Point mousePosScreen = new Point((double)Utility.GET_X_LPARAM(lParam), (double)Utility.GET_Y_LPARAM(lParam));
            Rect windowPosition = this._GetWindowRect();
            Point mousePosWindow = mousePosScreen;
            mousePosWindow.Offset(-windowPosition.X, -windowPosition.Y);
            mousePosWindow = DpiHelper.DevicePixelsToLogical(mousePosWindow);
            IInputElement inputElement = this._window.InputHitTest(mousePosWindow);
            if (inputElement != null)
            {
                if (WindowChrome.GetIsHitTestVisibleInChrome(inputElement))
                {
                    handled = true;
                    return new IntPtr(1);
                }
                ResizeGripDirection direction = WindowChrome.GetResizeGripDirection(inputElement);
                if (direction != ResizeGripDirection.None)
                {
                    handled = true;
                    return new IntPtr((int)this._GetHTFromResizeGripDirection(direction));
                }
            }
            if (this._chromeInfo.UseAeroCaptionButtons && ((Utility.IsOSVistaOrNewer && (this._chromeInfo.GlassFrameThickness != new Thickness())) && this._isGlassEnabled))
            {
                IntPtr lRet;
                handled = Standard.NativeMethods.DwmDefWindowProc(this._hwnd, uMsg, wParam, lParam, out lRet);
                if (IntPtr.Zero != lRet)
                {
                    return lRet;
                }
            }
            HT ht = this._HitTestNca(DpiHelper.DeviceRectToLogical(windowPosition), DpiHelper.DevicePixelsToLogical(mousePosScreen));
            handled = true;
            return new IntPtr((int)ht);
        }

        private IntPtr _HandleNCRButtonUp(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // Emulate the system behavior of clicking the right mouse button over the caption area
            // to bring up the system menu.
            if (HT.CAPTION == (HT)wParam.ToInt32())
            {
                SystemCommands.ShowSystemMenuPhysicalCoordinates(_window, new Point(Utility.GET_X_LPARAM(lParam), Utility.GET_Y_LPARAM(lParam)));
            }
            handled = false;
            return IntPtr.Zero;
        }

        private IntPtr _HandleSize(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            const int SIZE_MAXIMIZED = 2;

            // Force when maximized.
            // We can tell what's happening right now, but the Window doesn't yet know it's
            // maximized.  Not forcing this update will eventually cause the
            // default caption to be drawn.
            WindowState? state = null;
            if (wParam.ToInt32() == SIZE_MAXIMIZED)
            {
                state = WindowState.Maximized;
            }
            _UpdateSystemMenu(state);

            // Still let the default WndProc handle this.
            handled = false;
            return IntPtr.Zero;
        }

        private IntPtr _HandleWindowPosChanged(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // http://blogs.msdn.com/oldnewthing/archive/2008/01/15/7113860.aspx
            // The WM_WINDOWPOSCHANGED message is sent at the end of the window
            // state change process. It sort of combines the other state change
            // notifications, WM_MOVE, WM_SIZE, and WM_SHOWWINDOW. But it doesn't
            // suffer from the same limitations as WM_SHOWWINDOW, so you can
            // reliably use it to react to the window being shown or hidden.

            _UpdateSystemMenu(null);

            if (!_isGlassEnabled)
            {
                Assert.IsNotDefault(lParam);
                var wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                _SetRoundingRegion(wp);
            }

            // Still want to pass this to DefWndProc
            handled = false;
            return IntPtr.Zero;
        }

        private IntPtr _HandleDwmCompositionChanged(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            _UpdateFrameState(false);
            
            handled = false;
            return IntPtr.Zero;
        }

        private IntPtr _HandleSettingChange(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // There are several settings that can cause fixups for the template to become invalid when changed.
            // These shouldn't be required on the v4 framework.
            Assert.IsTrue(Utility.IsPresentationFrameworkVersionLessThan4);
            
            _FixupTemplateIssues();

            handled = false;
            return IntPtr.Zero;
        }

        private IntPtr _HandleEnterSizeMove(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // This is only intercepted to deal with bugs in Window in .Net 3.5 and below.
            Assert.IsTrue(Utility.IsPresentationFrameworkVersionLessThan4);

            _isUserResizing = true;

            // On Win7 if the user is dragging the window out of the maximized state then we don't want to use that location
            // as a restore point.
            Assert.Implies(_window.WindowState == WindowState.Maximized, Utility.IsOSWindows7OrNewer);
            if (_window.WindowState != WindowState.Maximized)
            {
                // Check for the docked window case.  The window can still be restored when it's in this position so 
                // try to account for that and not update the start position.
                if (!_IsWindowDocked)
                {
                    _windowPosAtStartOfUserMove = new Point(_window.Left, _window.Top);
                }
                // Realistically we also don't want to update the start position when moving from one docked state to another (or to and from maximized),
                // but it's tricky to detect and this is already a workaround for a bug that's fixed in newer versions of the framework.
                // Not going to try to handle all cases.
            }

            handled = false;
            return IntPtr.Zero;
        }

        private IntPtr _HandleExitSizeMove(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // This is only intercepted to deal with bugs in Window in .Net 3.5 and below.
            Assert.IsTrue(Utility.IsPresentationFrameworkVersionLessThan4);

            _isUserResizing = false;

            // On Win7 the user can change the Window's state by dragging the window to the top of the monitor.
            // If they did that, then we need to try to update the restore bounds or else WPF will put the window at the maximized location (e.g. (-8,-8)).
            if (_window.WindowState == WindowState.Maximized)
            {
                Assert.IsTrue(Utility.IsOSWindows7OrNewer);
                _window.Top = _windowPosAtStartOfUserMove.Y;
                _window.Left = _windowPosAtStartOfUserMove.X;
            }

            handled = false;
            return IntPtr.Zero;
        }

        private IntPtr _HandleMove(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // This is only intercepted to deal with bugs in Window in .Net 3.5 and below.
            Assert.IsTrue(Utility.IsPresentationFrameworkVersionLessThan4);

            if (_isUserResizing)
            {
                _hasUserMovedWindow = true;
            }

            handled = false;
            return IntPtr.Zero;
        }

        #endregion

        /// <summary>Add and remove a native WindowStyle from the HWND.</summary>
        /// <param name="removeStyle">The styles to be removed.  These can be bitwise combined.</param>
        /// <param name="addStyle">The styles to be added.  These can be bitwise combined.</param>
        /// <returns>Whether the styles of the HWND were modified as a result of this call.</returns>
        private bool _ModifyStyle(WS removeStyle, WS addStyle)
        {
            Assert.IsNotDefault(_hwnd);
            var dwStyle = (WS)NativeMethods.GetWindowLongPtr(_hwnd, GWL.STYLE).ToInt32();
            var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;
            if (dwStyle == dwNewStyle)
            {
                return false;
            }

            NativeMethods.SetWindowLongPtr(_hwnd, GWL.STYLE, new IntPtr((int)dwNewStyle));
            return true;
        }

        /// <summary>
        /// Get the WindowState as the native HWND knows it to be.  This isn't necessarily the same as what Window thinks.
        /// </summary>
        private WindowState _GetHwndState()
        {
            var wpl = NativeMethods.GetWindowPlacement(_hwnd);
            switch (wpl.showCmd)
            {
                case SW.SHOWMINIMIZED: return WindowState.Minimized;
                case SW.SHOWMAXIMIZED: return WindowState.Maximized;
            }
            return WindowState.Normal;
        }

        /// <summary>
        /// Get the bounding rectangle for the window in physical coordinates.
        /// </summary>
        /// <returns>The bounding rectangle for the window.</returns>
        private Rect _GetWindowRect()
        {
            // Get the window rectangle.
            RECT windowPosition = NativeMethods.GetWindowRect(_hwnd);
            return new Rect(windowPosition.Left, windowPosition.Top, windowPosition.Width, windowPosition.Height);
        }

        /// <summary>
        /// Update the items in the system menu based on the current, or assumed, WindowState.
        /// </summary>
        /// <param name="assumeState">
        /// The state to assume that the Window is in.  This can be null to query the Window's state.
        /// </param>
        /// <remarks>
        /// We want to update the menu while we have some control over whether the caption will be repainted.
        /// </remarks>
        private void _UpdateSystemMenu(WindowState? assumeState)
        {
            const MF mfEnabled = MF.ENABLED | MF.BYCOMMAND;
            const MF mfDisabled = MF.GRAYED | MF.DISABLED | MF.BYCOMMAND;

            WindowState state = assumeState ?? _GetHwndState();

            if (null != assumeState || _lastMenuState != state)
            {
                _lastMenuState = state;

                bool modified = _ModifyStyle(WS.VISIBLE, 0);
                IntPtr hmenu = NativeMethods.GetSystemMenu(_hwnd, false);
                if (IntPtr.Zero != hmenu)
                {
                    var dwStyle = (WS)NativeMethods.GetWindowLongPtr(_hwnd, GWL.STYLE).ToInt32();

                    bool canMinimize = Utility.IsFlagSet((int)dwStyle, (int)WS.MINIMIZEBOX);
                    bool canMaximize = Utility.IsFlagSet((int)dwStyle, (int)WS.MAXIMIZEBOX);
                    bool canSize = Utility.IsFlagSet((int)dwStyle, (int)WS.THICKFRAME);

                    switch (state)
                    {
                        case WindowState.Maximized:
                            NativeMethods.EnableMenuItem(hmenu, SC.RESTORE, mfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.MOVE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.SIZE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.MINIMIZE, canMinimize ? mfEnabled : mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.MAXIMIZE, mfDisabled);
                            break;
                        case WindowState.Minimized:
                            NativeMethods.EnableMenuItem(hmenu, SC.RESTORE, mfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.MOVE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.SIZE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.MINIMIZE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.MAXIMIZE, canMaximize ? mfEnabled : mfDisabled);
                            break;
                        default:
                            NativeMethods.EnableMenuItem(hmenu, SC.RESTORE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.MOVE, mfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.SIZE, canSize ? mfEnabled : mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.MINIMIZE, canMinimize ? mfEnabled : mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SC.MAXIMIZE, canMaximize ? mfEnabled : mfDisabled);
                            break;
                    }
                }

                if (modified)
                {
                    _ModifyStyle(WS.TILED, WS.TILED | WS.VISIBLE);
                }
            }
        }

        private void _UpdateFrameState(bool force)
        {
            if (IntPtr.Zero == _hwnd)
            {
                return;
            }

            // Don't rely on SystemParameters2 for this, just make the check ourselves.
            bool frameState = NativeMethods.DwmIsCompositionEnabled();

            if (force || frameState != _isGlassEnabled)
            {
                _isGlassEnabled = frameState && _chromeInfo.GlassFrameThickness != default(Thickness);

                if (!_isGlassEnabled)
                {
                    _SetRoundingRegion(null);
                }
                else
                {
                    _ClearRoundingRegion();
                    _ExtendGlassFrame();                   
                }

                NativeMethods.SetWindowPos(this._hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP.NOOWNERZORDER | SWP.DRAWFRAME | SWP.NOACTIVATE | SWP.NOZORDER | SWP.NOMOVE | SWP.NOSIZE);
            }
        }

        private void _ClearRoundingRegion()
        {
            NativeMethods.SetWindowRgn(_hwnd, IntPtr.Zero, NativeMethods.IsWindowVisible(_hwnd));
        }

        private void _SetRoundingRegion(WINDOWPOS? wp)
        {
            IntPtr hrgn;
            if (Standard.NativeMethods.GetWindowPlacement(this._hwnd).showCmd == SW.SHOWMAXIMIZED)
            {
                int left;
                int top;
                if (wp.HasValue)
                {
                    left = wp.Value.x;
                    top = wp.Value.y;
                }
                else
                {
                    Rect r = this._GetWindowRect();
                    left = (int)r.Left;
                    top = (int)r.Top;
                }
                RECT rcMax = Standard.NativeMethods.GetMonitorInfo(Standard.NativeMethods.MonitorFromWindow(this._hwnd, 2)).rcWork;
                rcMax.Offset(-left, -top);
                hrgn = IntPtr.Zero;
                try
                {
                    hrgn = Standard.NativeMethods.CreateRectRgnIndirect(rcMax);
                    Standard.NativeMethods.SetWindowRgn(this._hwnd, hrgn, Standard.NativeMethods.IsWindowVisible(this._hwnd));
                    hrgn = IntPtr.Zero;
                }
                finally
                {
                    Utility.SafeDeleteObject(ref hrgn);
                }
            }
            else
            {
                Size windowSize;
                if (!(!wp.HasValue || Utility.IsFlagSet(wp.Value.flags, 1)))
                {
                    windowSize = new Size((double)wp.Value.cx, (double)wp.Value.cy);
                }
                else
                {
                    if (wp.HasValue && (this._lastRoundingState == this._window.WindowState))
                    {
                        return;
                    }
                    windowSize = this._GetWindowRect().Size;
                }
                this._lastRoundingState = this._window.WindowState;
                hrgn = IntPtr.Zero;
                try
                {
                    double shortestDimension = Math.Min(windowSize.Width, windowSize.Height);
                    double topLeftRadius = Math.Min(DpiHelper.LogicalPixelsToDevice(new Point(this._chromeInfo.CornerRadius.TopLeft, 0.0)).X, shortestDimension / 2.0);
                    if (_IsUniform(this._chromeInfo.CornerRadius))
                    {
                        hrgn = _CreateRoundRectRgn(new Rect(windowSize), topLeftRadius);
                    }
                    else
                    {
                        hrgn = _CreateRoundRectRgn(new Rect(0.0, 0.0, (windowSize.Width / 2.0) + topLeftRadius, (windowSize.Height / 2.0) + topLeftRadius), topLeftRadius);

                        double topRightRadius = Math.Min(DpiHelper.LogicalPixelsToDevice(new Point(this._chromeInfo.CornerRadius.TopRight, 0.0)).X, shortestDimension / 2.0);

                        Rect topRightRegionRect = new Rect(0.0, 0.0, (windowSize.Width / 2.0) + topRightRadius, (windowSize.Height / 2.0) + topRightRadius);

                        topRightRegionRect.Offset((double)((windowSize.Width / 2.0) - topRightRadius), (double)0.0);

                        _CreateAndCombineRoundRectRgn(hrgn, topRightRegionRect, topRightRadius);

                        double bottomLeftRadius = Math.Min(DpiHelper.LogicalPixelsToDevice(new Point(this._chromeInfo.CornerRadius.BottomLeft, 0.0)).X, shortestDimension / 2.0);

                        Rect bottomLeftRegionRect = new Rect(0.0, 0.0, (windowSize.Width / 2.0) + bottomLeftRadius, (windowSize.Height / 2.0) + bottomLeftRadius);

                        bottomLeftRegionRect.Offset((double)0.0, (double)((windowSize.Height / 2.0) - bottomLeftRadius));

                        _CreateAndCombineRoundRectRgn(hrgn, bottomLeftRegionRect, bottomLeftRadius);

                        double bottomRightRadius = Math.Min(DpiHelper.LogicalPixelsToDevice(new Point(this._chromeInfo.CornerRadius.BottomRight, 0.0)).X, shortestDimension / 2.0);

                        Rect bottomRightRegionRect = new Rect(0.0, 0.0, (windowSize.Width / 2.0) + bottomRightRadius, (windowSize.Height / 2.0) + bottomRightRadius);

                        bottomRightRegionRect.Offset((double)((windowSize.Width / 2.0) - bottomRightRadius), (double)((windowSize.Height / 2.0) - bottomRightRadius));

                        _CreateAndCombineRoundRectRgn(hrgn, bottomRightRegionRect, bottomRightRadius);
                    }
                    Standard.NativeMethods.SetWindowRgn(this._hwnd, hrgn, Standard.NativeMethods.IsWindowVisible(this._hwnd));
                    hrgn = IntPtr.Zero;
                }
                finally
                {
                    Utility.SafeDeleteObject(ref hrgn);
                }
            }
        }

        private static IntPtr _CreateRoundRectRgn(Rect region, double radius)
        {
            // Round outwards.

            if (DoubleUtilities.AreClose(0, radius))
            {
                return NativeMethods.CreateRectRgn(
                    (int)Math.Floor(region.Left),
                    (int)Math.Floor(region.Top), 
                    (int)Math.Ceiling(region.Right), 
                    (int)Math.Ceiling(region.Bottom));
            }

            // RoundedRect HRGNs require an additional pixel of padding on the bottom right to look correct.
            return NativeMethods.CreateRoundRectRgn(
                (int)Math.Floor(region.Left),
                (int)Math.Floor(region.Top), 
                (int)Math.Ceiling(region.Right) + 1, 
                (int)Math.Ceiling(region.Bottom) + 1,
                (int)Math.Ceiling(radius), 
                (int)Math.Ceiling(radius));
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HRGNs")]
        private static void _CreateAndCombineRoundRectRgn(IntPtr hrgnSource, Rect region, double radius)
        {
            IntPtr hrgn = IntPtr.Zero;
            try
            {
                hrgn = _CreateRoundRectRgn(region, radius);
                CombineRgnResult result = NativeMethods.CombineRgn(hrgnSource, hrgnSource, hrgn, RGN.OR);
                if (result == CombineRgnResult.ERROR)
                {
                    throw new InvalidOperationException("Unable to combine two HRGNs.");
                }
            }
            catch
            {
                Utility.SafeDeleteObject(ref hrgn);
                throw;
            }
        }

        private static bool _IsUniform(CornerRadius cornerRadius)
        {
            if (!DoubleUtilities.AreClose(cornerRadius.BottomLeft, cornerRadius.BottomRight))
            {
                return false;
            }

            if (!DoubleUtilities.AreClose(cornerRadius.TopLeft, cornerRadius.TopRight))
            {
                return false;
            }

            if (!DoubleUtilities.AreClose(cornerRadius.BottomLeft, cornerRadius.TopRight))
            {
                return false;
            }

            return true;
        }

        private void _ExtendGlassFrame()
        {
            Assert.IsNotNull(_window);

            // Expect that this might be called on OSes other than Vista.
            if (!Utility.IsOSVistaOrNewer)
            {
                // Not an error.  Just not on Vista so we're not going to get glass.
                return;
            }

            if (IntPtr.Zero == _hwnd)
            {
                // Can't do anything with this call until the Window has been shown.
                return;
            }

            // Ensure standard HWND background painting when DWM isn't enabled.
            if (!NativeMethods.DwmIsCompositionEnabled())
            {
                _hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
            }
            else
            {

                this._hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;
                Thickness deviceGlassThickness = DpiHelper.LogicalThicknessToDevice(this._chromeInfo.GlassFrameThickness);
                if (this._chromeInfo.NonClientFrameEdges != NonClientFrameEdges.None)
                {
                    Thickness windowResizeBorderThicknessDevice = DpiHelper.LogicalThicknessToDevice(SystemParameters2.Current.WindowResizeBorderThickness);
                    if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 2))
                    {
                        deviceGlassThickness.Top -= windowResizeBorderThicknessDevice.Top;
                        deviceGlassThickness.Top = Math.Max(0.0, deviceGlassThickness.Top);
                    }
                    if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 1))
                    {
                        deviceGlassThickness.Left -= windowResizeBorderThicknessDevice.Left;
                        deviceGlassThickness.Left = Math.Max(0.0, deviceGlassThickness.Left);
                    }
                    if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 8))
                    {
                        deviceGlassThickness.Bottom -= windowResizeBorderThicknessDevice.Bottom;
                        deviceGlassThickness.Bottom = Math.Max(0.0, deviceGlassThickness.Bottom);
                    }
                    if (Utility.IsFlagSet((int)this._chromeInfo.NonClientFrameEdges, 4))
                    {
                        deviceGlassThickness.Right -= windowResizeBorderThicknessDevice.Right;
                        deviceGlassThickness.Right = Math.Max(0.0, deviceGlassThickness.Right);
                    }
                }
                MARGINS dwmMargin = new MARGINS
                {
                    cxLeftWidth = (int)Math.Ceiling(deviceGlassThickness.Left),
                    cxRightWidth = (int)Math.Ceiling(deviceGlassThickness.Right),
                    cyTopHeight = (int)Math.Ceiling(deviceGlassThickness.Top),
                    cyBottomHeight = (int)Math.Ceiling(deviceGlassThickness.Bottom)
                };
                
                NativeMethods.DwmExtendFrameIntoClientArea(_hwnd, ref dwmMargin);
            }
        }

        /// <summary>
        /// Matrix of the HT values to return when responding to NC window messages.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
        private static readonly HT[,] _HitTestBorders = new[,]
        {
            { HT.TOPLEFT,    HT.TOP,     HT.TOPRIGHT    },
            { HT.LEFT,       HT.CLIENT,  HT.RIGHT       },
            { HT.BOTTOMLEFT, HT.BOTTOM,  HT.BOTTOMRIGHT },
        };

        private HT _HitTestNca(Rect windowPosition, Point mousePosition)
        {
            int uRow = 1;
            int uCol = 1;
            bool onResizeBorder = false;
            if ((mousePosition.Y >= windowPosition.Top) && (mousePosition.Y < ((windowPosition.Top + this._chromeInfo.ResizeBorderThickness.Top) + this._chromeInfo.CaptionHeight)))
            {
                onResizeBorder = mousePosition.Y < (windowPosition.Top + this._chromeInfo.ResizeBorderThickness.Top);
                uRow = 0;
            }
            else if ((mousePosition.Y < windowPosition.Bottom) && (mousePosition.Y >= (windowPosition.Bottom - ((int)this._chromeInfo.ResizeBorderThickness.Bottom))))
            {
                uRow = 2;
            }
            if ((mousePosition.X >= windowPosition.Left) && (mousePosition.X < (windowPosition.Left + ((int)this._chromeInfo.ResizeBorderThickness.Left))))
            {
                uCol = 0;
            }
            else if ((mousePosition.X < windowPosition.Right) && (mousePosition.X >= (windowPosition.Right - this._chromeInfo.ResizeBorderThickness.Right)))
            {
                uCol = 2;
            }
            if (!(((uRow != 0) || (uCol == 1)) || onResizeBorder))
            {
                uRow = 1;
            }
            HT ht = _HitTestBorders[uRow, uCol];
            if (!((ht != HT.TOP) || onResizeBorder))
            {
                ht = HT.CAPTION;
            }
            return ht;
        }

        #region Remove Custom Chrome Methods

        private void _RestoreStandardChromeState(bool isClosing)
        {
            VerifyAccess();
            
            _UnhookCustomChrome();

            if (!isClosing)
            {
                _RestoreFrameworkIssueFixups();
                _RestoreGlassFrame();
                _RestoreHrgn();

                _window.InvalidateMeasure();
            }
        }

        private void _UnhookCustomChrome()
        {
            Assert.IsNotDefault(_hwnd);
            Assert.IsNotNull(_window);

            if (_isHooked)
            {
                _hwndSource.RemoveHook(_WndProc);
                _isHooked = false;
            }
        }

        private void _RestoreFrameworkIssueFixups()
        {
            // This margin is only necessary if the client rect is going to be calculated incorrectly by WPF.
            // This bug was fixed in V4 of the framework.
            FrameworkElement rootElement = (FrameworkElement)VisualTreeHelper.GetChild(this._window, 0);
            rootElement.Margin = new Thickness();
            if (Utility.IsPresentationFrameworkVersionLessThan4)
            {
                this._window.StateChanged -= new EventHandler(this._FixupRestoreBounds);
                this._isFixedUp = false;
            }
        }

        private void _RestoreGlassFrame()
        {
            Assert.IsNull(_chromeInfo);
            Assert.IsNotNull(_window);

            // Expect that this might be called on OSes other than Vista
            // and if the window hasn't yet been shown, then we don't need to undo anything.
            if (!Utility.IsOSVistaOrNewer || _hwnd == IntPtr.Zero)
            {
                return;
            }

            _hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;

            if (NativeMethods.DwmIsCompositionEnabled())
            {
                // If glass is enabled, push it back to the normal bounds.
                var dwmMargin = new MARGINS();
                NativeMethods.DwmExtendFrameIntoClientArea(_hwnd, ref dwmMargin);
            }
        }

        private void _RestoreHrgn()
        {
            _ClearRoundingRegion();
            NativeMethods.SetWindowPos(_hwnd, IntPtr.Zero, 0, 0, 0, 0, _SwpFlags);
        }

        #endregion
    }
}
