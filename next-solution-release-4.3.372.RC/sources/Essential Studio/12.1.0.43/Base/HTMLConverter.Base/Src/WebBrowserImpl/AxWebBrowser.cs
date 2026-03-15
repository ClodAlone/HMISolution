#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Syncfusion.HtmlConverter.Natives
{
    [DesignTimeVisible(true), Clsid("{8856f961-340a-11d0-a96b-00c04fd705a2}"), DefaultProperty("Name")]
    internal class AxWebBrowser : AxHost
    {
        private AxHost.ConnectionPointCookie cookie;
        private AxWebBrowserEventMulticaster eventMulticaster;
        private IWebBrowser2 ocx;

        public event DWebBrowserEvents2_BeforeNavigate2EventHandler BeforeNavigate2;

        public event DWebBrowserEvents2_ClientToHostWindowEventHandler ClientToHostWindow;

        public event DWebBrowserEvents2_CommandStateChangeEventHandler CommandStateChange;

        public event DWebBrowserEvents2_DocumentCompleteEventHandler DocumentComplete;

        public event EventHandler DownloadBegin;

        public event EventHandler DownloadComplete;

        public event DWebBrowserEvents2_FileDownloadEventHandler FileDownload;

        public event DWebBrowserEvents2_NavigateComplete2EventHandler NavigateComplete2;

        public event DWebBrowserEvents2_NavigateErrorEventHandler NavigateError;

        public event DWebBrowserEvents2_NewWindow2EventHandler NewWindow2;

        public event DWebBrowserEvents2_NewWindow3EventHandler NewWindow3;

        public event DWebBrowserEvents2_OnFullScreenEventHandler OnFullScreen;

        public event DWebBrowserEvents2_OnMenuBarEventHandler OnMenuBar;

        public event EventHandler OnQuit;

        public event DWebBrowserEvents2_OnStatusBarEventHandler OnStatusBar;

        public event DWebBrowserEvents2_OnTheaterModeEventHandler OnTheaterMode;

        public event DWebBrowserEvents2_OnToolBarEventHandler OnToolBar;

        public event DWebBrowserEvents2_OnVisibleEventHandler OnVisible;

        public event DWebBrowserEvents2_PrintTemplateInstantiationEventHandler PrintTemplateInstantiation;

        public event DWebBrowserEvents2_PrintTemplateTeardownEventHandler PrintTemplateTeardown;

        public event DWebBrowserEvents2_PrivacyImpactedStateChangeEventHandler PrivacyImpactedStateChange;

        public event DWebBrowserEvents2_ProgressChangeEventHandler ProgressChange;

        public event DWebBrowserEvents2_PropertyChangeEventHandler PropertyChange;

        public event DWebBrowserEvents2_SetSecureLockIconEventHandler SetSecureLockIcon;

        public event DWebBrowserEvents2_StatusTextChangeEventHandler StatusTextChange;

        public event DWebBrowserEvents2_TitleChangeEventHandler TitleChange;

        public event DWebBrowserEvents2_UpdatePageStatusEventHandler UpdatePageStatus;

        public event DWebBrowserEvents2_WindowClosingEventHandler WindowClosing;

        public event DWebBrowserEvents2_WindowSetHeightEventHandler WindowSetHeight;

        public event DWebBrowserEvents2_WindowSetLeftEventHandler WindowSetLeft;

        public event DWebBrowserEvents2_WindowSetResizableEventHandler WindowSetResizable;

        public event DWebBrowserEvents2_WindowSetTopEventHandler WindowSetTop;

        public event DWebBrowserEvents2_WindowSetWidthEventHandler WindowSetWidth;


        public AxWebBrowser()
            : base("8856f961-340a-11d0-a96b-00c04fd705a2")
        {

        }

        
        protected override void AttachInterfaces()
        {
            try
            {
                this.ocx = (IWebBrowser2)base.GetOcx();
            }
            catch (Exception)
            {
            }
        }
        
        public virtual void ClientToWindow(ref int pcx, ref int pcy)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("ClientToWindow", 0);
            }
            this.ocx.ClientToWindow(ref pcx, ref pcy);
        }
        
        protected override void CreateSink()
        {
            try
            {
                this.eventMulticaster = new AxWebBrowserEventMulticaster(this);
                this.cookie = new AxHost.ConnectionPointCookie(this.ocx, this.eventMulticaster, typeof(DWebBrowserEvents2));
            }
            catch (Exception)
            {
            }
        }
        
        public virtual void CtlRefresh()
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("CtlRefresh", 0);
            }
            this.ocx.Refresh();
        }
        
        protected override void DetachSink()
        {
            try
            {
                this.cookie.Disconnect();
            }
            catch (Exception)
            {
            }
        }
        
        public virtual void ExecWB(OLECMDID cmdID, OLECMDEXECOPT cmdexecopt)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("ExecWB", 0);
            }
            object[] parameters = new object[] { cmdID, cmdexecopt, Missing.Value, Missing.Value };
            System.Type type = typeof(IWebBrowser2);
            type.GetMethod("ExecWB").Invoke(this.ocx, parameters);
        }
        
        public virtual void ExecWB(OLECMDID cmdID, OLECMDEXECOPT cmdexecopt, ref object pvaIn, ref object pvaOut)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("ExecWB", 0);
            }
            this.ocx.ExecWB(cmdID, cmdexecopt, ref pvaIn, ref pvaOut);
        }
        
        public virtual object GetProperty(string property)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("GetProperty", 0);
            }
            return this.ocx.GetProperty(property);
        }
        
        public virtual void GoBack()
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("GoBack", 0);
            }
            this.ocx.GoBack();
        }

        public virtual void GoForward()
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("GoForward", 0);
            }
            this.ocx.GoForward();
        }
        
        public virtual void GoHome()
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("GoHome", 0);
            }
            this.ocx.GoHome();
        }
        
        public virtual void GoSearch()
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("GoSearch", 0);
            }
            this.ocx.GoSearch();
        }
        
        public virtual void Navigate(string uRL)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("Navigate", 0);
            }
            object[] parameters = new object[] { uRL, Missing.Value, Missing.Value, Missing.Value, Missing.Value };
            System.Type type = typeof(IWebBrowser2);
            type.GetMethod("Navigate").Invoke(this.ocx, parameters);
        }
        
        public virtual void Navigate(string uRL, ref object flags, ref object targetFrameName, ref object postData, ref object headers)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("Navigate", 0);
            }
            this.ocx.Navigate(uRL, ref flags, ref targetFrameName, ref postData, ref headers);
        }
        
        public virtual void Navigate2(ref object uRL)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("Navigate2", 0);
            }
            object[] parameters = new object[] { uRL, Missing.Value, Missing.Value, Missing.Value, Missing.Value };
            System.Type type = typeof(IWebBrowser2);
            type.GetMethod("Navigate2").Invoke(this.ocx, parameters);
            uRL = parameters[0];
        }
        
        public virtual void Navigate2(ref object uRL, ref object flags, ref object targetFrameName, ref object postData, ref object headers)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("Navigate2", 0);
            }
            this.ocx.Navigate2(ref uRL, ref flags, ref targetFrameName, ref postData, ref headers);
        }
        
        public virtual void PutProperty(string property, object vtValue)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("PutProperty", 0);
            }
            this.ocx.PutProperty(property, vtValue);
        }
        
        public virtual OLECMDF QueryStatusWB(OLECMDID cmdID)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("QueryStatusWB", 0);
            }
            return this.ocx.QueryStatusWB(cmdID);
        }
        
        public virtual void Quit()
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("Quit", 0);
            }
            this.ocx.Quit();
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnBeforeNavigate2(object sender, DWebBrowserEvents2_BeforeNavigate2Event e)
        {
            if (this.BeforeNavigate2 != null)
            {
                this.BeforeNavigate2(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnClientToHostWindow(object sender, DWebBrowserEvents2_ClientToHostWindowEvent e)
        {
            if (this.ClientToHostWindow != null)
            {
                this.ClientToHostWindow(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnCommandStateChange(object sender, DWebBrowserEvents2_CommandStateChangeEvent e)
        {
            if (this.CommandStateChange != null)
            {
                this.CommandStateChange(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnDocumentComplete(object sender, DWebBrowserEvents2_DocumentCompleteEvent e)
        {
            if (this.DocumentComplete != null)
            {
                this.DocumentComplete(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnDownloadBegin(object sender, EventArgs e)
        {
            if (this.DownloadBegin != null)
            {
                this.DownloadBegin(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnDownloadComplete(object sender, EventArgs e)
        {
            if (this.DownloadComplete != null)
            {
                this.DownloadComplete(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnFileDownload(object sender, DWebBrowserEvents2_FileDownloadEvent e)
        {
            if (this.FileDownload != null)
            {
                this.FileDownload(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnNavigateComplete2(object sender, DWebBrowserEvents2_NavigateComplete2Event e)
        {
            if (this.NavigateComplete2 != null)
            {
                this.NavigateComplete2(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnNavigateError(object sender, DWebBrowserEvents2_NavigateErrorEvent e)
        {
            if (this.NavigateError != null)
            {
                this.NavigateError(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnNewWindow2(object sender, DWebBrowserEvents2_NewWindow2Event e)
        {
            if (this.NewWindow2 != null)
            {
                this.NewWindow2(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnNewWindow3(object sender, DWebBrowserEvents2_NewWindow3Event e)
        {
            if (this.NewWindow3 != null)
            {
                this.NewWindow3(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnOnFullScreen(object sender, DWebBrowserEvents2_OnFullScreenEvent e)
        {
            if (this.OnFullScreen != null)
            {
                this.OnFullScreen(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnOnMenuBar(object sender, DWebBrowserEvents2_OnMenuBarEvent e)
        {
            if (this.OnMenuBar != null)
            {
                this.OnMenuBar(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnOnQuit(object sender, EventArgs e)
        {
            if (this.OnQuit != null)
            {
                this.OnQuit(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnOnStatusBar(object sender, DWebBrowserEvents2_OnStatusBarEvent e)
        {
            if (this.OnStatusBar != null)
            {
                this.OnStatusBar(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnOnTheaterMode(object sender, DWebBrowserEvents2_OnTheaterModeEvent e)
        {
            if (this.OnTheaterMode != null)
            {
                this.OnTheaterMode(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnOnToolBar(object sender, DWebBrowserEvents2_OnToolBarEvent e)
        {
            if (this.OnToolBar != null)
            {
                this.OnToolBar(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnOnVisible(object sender, DWebBrowserEvents2_OnVisibleEvent e)
        {
            if (this.OnVisible != null)
            {
                this.OnVisible(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnPrintTemplateInstantiation(object sender, DWebBrowserEvents2_PrintTemplateInstantiationEvent e)
        {
            if (this.PrintTemplateInstantiation != null)
            {
                this.PrintTemplateInstantiation(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnPrintTemplateTeardown(object sender, DWebBrowserEvents2_PrintTemplateTeardownEvent e)
        {
            if (this.PrintTemplateTeardown != null)
            {
                this.PrintTemplateTeardown(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnPrivacyImpactedStateChange(object sender, DWebBrowserEvents2_PrivacyImpactedStateChangeEvent e)
        {
            if (this.PrivacyImpactedStateChange != null)
            {
                this.PrivacyImpactedStateChange(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnProgressChange(object sender, DWebBrowserEvents2_ProgressChangeEvent e)
        {
            if (this.ProgressChange != null)
            {
                this.ProgressChange(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnPropertyChange(object sender, DWebBrowserEvents2_PropertyChangeEvent e)
        {
            if (this.PropertyChange != null)
            {
                this.PropertyChange(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnSetSecureLockIcon(object sender, DWebBrowserEvents2_SetSecureLockIconEvent e)
        {
            if (this.SetSecureLockIcon != null)
            {
                this.SetSecureLockIcon(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnStatusTextChange(object sender, DWebBrowserEvents2_StatusTextChangeEvent e)
        {
            if (this.StatusTextChange != null)
            {
                this.StatusTextChange(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnTitleChange(object sender, DWebBrowserEvents2_TitleChangeEvent e)
        {
            if (this.TitleChange != null)
            {
                this.TitleChange(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnUpdatePageStatus(object sender, DWebBrowserEvents2_UpdatePageStatusEvent e)
        {
            if (this.UpdatePageStatus != null)
            {
                this.UpdatePageStatus(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnWindowClosing(object sender, DWebBrowserEvents2_WindowClosingEvent e)
        {
            if (this.WindowClosing != null)
            {
                this.WindowClosing(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnWindowSetHeight(object sender, DWebBrowserEvents2_WindowSetHeightEvent e)
        {
            if (this.WindowSetHeight != null)
            {
                this.WindowSetHeight(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnWindowSetLeft(object sender, DWebBrowserEvents2_WindowSetLeftEvent e)
        {
            if (this.WindowSetLeft != null)
            {
                this.WindowSetLeft(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnWindowSetResizable(object sender, DWebBrowserEvents2_WindowSetResizableEvent e)
        {
            if (this.WindowSetResizable != null)
            {
                this.WindowSetResizable(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnWindowSetTop(object sender, DWebBrowserEvents2_WindowSetTopEvent e)
        {
            if (this.WindowSetTop != null)
            {
                this.WindowSetTop(sender, e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RaiseOnWindowSetWidth(object sender, DWebBrowserEvents2_WindowSetWidthEvent e)
        {
            if (this.WindowSetWidth != null)
            {
                this.WindowSetWidth(sender, e);
            }
        }
        
        public virtual void Refresh2()
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("Refresh2", 0);
            }
            object[] parameters = new object[] { Missing.Value };
            System.Type type = typeof(IWebBrowser2);
            type.GetMethod("Refresh2").Invoke(this.ocx, parameters);
        }
        
        public virtual void Refresh2(ref object level)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("Refresh2", 0);
            }
            this.ocx.Refresh2(ref level);
        }
        
        public virtual void ShowBrowserBar(ref object pvaClsid)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("ShowBrowserBar", 0);
            }
            object[] parameters = new object[] { pvaClsid, Missing.Value, Missing.Value };
            System.Type type = typeof(IWebBrowser2);
            type.GetMethod("ShowBrowserBar").Invoke(this.ocx, parameters);
            pvaClsid = parameters[0];
        }
        
        public virtual void ShowBrowserBar(ref object pvaClsid, ref object pvarShow, ref object pvarSize)
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("ShowBrowserBar", 0);
            }
            this.ocx.ShowBrowserBar(ref pvaClsid, ref pvarShow, ref pvarSize);
        }
        
        public virtual void Stop()
        {
            if (this.ocx == null)
            {
                throw new AxHost.InvalidActiveXStateException("Stop", 0);
            }
            this.ocx.Stop();
        }

        [DispId(0x22b), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool AddressBar
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("AddressBar", ActiveXInvokeKind.MethodInvoke);
                }
                return this.ocx.AddressBar;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("AddressBar", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.AddressBar = value;
            }
        }

        [DispId(200), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object Application
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Application", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Application;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0xd4)]
        public virtual bool Busy
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Busy", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Busy;
            }
        }

        [DispId(0xca), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object CtlContainer
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlContainer", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Container;
            }
        }

        [DispId(0xd1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int CtlHeight
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlHeight", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Height;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlHeight", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.Height = value;
            }
        }

        [DispId(0xce), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int CtlLeft
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlLeft", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Left;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlLeft", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.Left = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0xc9)]
        public virtual object CtlParent
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlParent", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Parent;
            }
        }

        [DispId(0xcf), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int CtlTop
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlTop", ActiveXInvokeKind.PropertySet);
                }
                return this.ocx.Top;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlTop", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.Top = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0x192)]
        public virtual bool CtlVisible
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlVisible", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Visible;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlVisible", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.Visible = value;
            }
        }

        [DispId(0xd0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int CtlWidth
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlWidth", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Width;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("CtlWidth", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.Width = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0xcb)]
        public virtual object Document
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Document", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Document;
            }
        }

        [DispId(400), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string FullName
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("FullName", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.FullName;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0x197)]
        public virtual bool FullScreen
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("FullScreen", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.FullScreen;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("FullScreen", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.FullScreen = value;
            }
        }

        [ComAliasName("System.Int32"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(-515), Browsable(false)]
        public virtual int HWND
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("HWND", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.HWND;
            }
        }

        [DispId(210), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string LocationName
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("LocationName", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.LocationName;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0xd3)]
        public virtual string LocationURL
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("LocationURL", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.LocationURL;
            }
        }

        [DispId(0x196), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool MenuBar
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("MenuBar", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.MenuBar;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("MenuBar", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.MenuBar = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(true), DispId(0)]
        public virtual string Name
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Name", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Name;
            }
        }

        [DispId(550), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Offline
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Offline", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Offline;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Offline", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.Offline = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0x191)]
        public virtual string Path
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Path", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Path;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(BindableSupport.Yes), DispId(-525)]
        public virtual tagREADYSTATE ReadyState
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("ReadyState", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.ReadyState;
            }
        }

        [DispId(0x228), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool RegisterAsBrowser
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("RegisterAsBrowser", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.RegisterAsBrowser;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("RegisterAsBrowser", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.RegisterAsBrowser = value;
            }
        }

        [DispId(0x229), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool RegisterAsDropTarget
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("RegisterAsDropTarget", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.RegisterAsDropTarget;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("RegisterAsDropTarget", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.RegisterAsDropTarget = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0x22c)]
        public virtual bool Resizable
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Resizable", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Resizable;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Resizable", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.Resizable = value;
            }
        }

        [DispId(0x227), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Silent
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Silent", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Silent;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Silent", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.Silent = value;
            }
        }

        [DispId(0x193), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool StatusBar
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("StatusBar", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.StatusBar;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("StatusBar", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.StatusBar = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0x194)]
        public virtual string StatusText
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("StatusText", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.StatusText;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("StatusText", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.StatusText = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0x22a)]
        public virtual bool TheaterMode
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("TheaterMode", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.TheaterMode;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("TheaterMode", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.TheaterMode = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0x195)]
        public virtual int ToolBar
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("ToolBar", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.ToolBar;
            }
            set
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("ToolBar", ActiveXInvokeKind.PropertySet);
                }
                this.ocx.ToolBar = value;
            }
        }

        [DispId(0xcc), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool TopLevelContainer
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("TopLevelContainer", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.TopLevelContainer;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DispId(0xcd)]
        public virtual string Type
        {
            get
            {
                if (this.ocx == null)
                {
                    throw new AxHost.InvalidActiveXStateException("Type", ActiveXInvokeKind.PropertyGet);
                }
                return this.ocx.Type;
            }
        }
    }
}

