#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;


namespace Syncfusion.HtmlConverter.Natives
{
    #region AxWebBrowser Events
    internal class DWebBrowserEvents2_BeforeNavigate2Event
    {
        public bool cancel;
        public object flags;
        public object headers;
        public object pDisp;
        public object postData;
        public object targetFrameName;
        public object uRL;

        public DWebBrowserEvents2_BeforeNavigate2Event(object pDisp, object uRL, object flags, object targetFrameName, object postData, object headers, bool cancel)
        {
            this.pDisp = pDisp;
            this.uRL = uRL;
            this.flags = flags;
            this.targetFrameName = targetFrameName;
            this.postData = postData;
            this.headers = headers;
            this.cancel = cancel;
        }
    }
    internal class AxWebBrowserEventMulticaster : DWebBrowserEvents2
    {
        private AxWebBrowser parent;

        public AxWebBrowserEventMulticaster(AxWebBrowser parent)
        {
            this.parent = parent;
        }

        public virtual void BeforeNavigate2(object pDisp, ref object uRL, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
        {
            DWebBrowserEvents2_BeforeNavigate2Event e = new DWebBrowserEvents2_BeforeNavigate2Event(pDisp, uRL, flags, targetFrameName, postData, headers, cancel);
            this.parent.RaiseOnBeforeNavigate2(this.parent, e);
            uRL = e.uRL;
            flags = e.flags;
            targetFrameName = e.targetFrameName;
            postData = e.postData;
            headers = e.headers;
            cancel = e.cancel;
        }

        public virtual void ClientToHostWindow(ref int cX, ref int cY)
        {
            DWebBrowserEvents2_ClientToHostWindowEvent e = new DWebBrowserEvents2_ClientToHostWindowEvent(cX, cY);
            this.parent.RaiseOnClientToHostWindow(this.parent, e);
            cX = e.cX;
            cY = e.cY;
        }

        public virtual void CommandStateChange(int command, bool enable)
        {
            DWebBrowserEvents2_CommandStateChangeEvent e = new DWebBrowserEvents2_CommandStateChangeEvent(command, enable);
            this.parent.RaiseOnCommandStateChange(this.parent, e);
        }

        public virtual void DocumentComplete(object pDisp, ref object uRL)
        {
            DWebBrowserEvents2_DocumentCompleteEvent e = new DWebBrowserEvents2_DocumentCompleteEvent(pDisp, uRL);
            this.parent.RaiseOnDocumentComplete(this.parent, e);
            uRL = e.uRL;
        }

        public virtual void DownloadBegin()
        {
            EventArgs e = new EventArgs();
            this.parent.RaiseOnDownloadBegin(this.parent, e);
        }

        public virtual void DownloadComplete()
        {
            EventArgs e = new EventArgs();
            this.parent.RaiseOnDownloadComplete(this.parent, e);
        }

        public virtual void FileDownload(ref bool cancel)
        {
            DWebBrowserEvents2_FileDownloadEvent e = new DWebBrowserEvents2_FileDownloadEvent(cancel);
            this.parent.RaiseOnFileDownload(this.parent, e);
            cancel = e.cancel;
        }

        public virtual void NavigateComplete2(object pDisp, ref object uRL)
        {
            DWebBrowserEvents2_NavigateComplete2Event e = new DWebBrowserEvents2_NavigateComplete2Event(pDisp, uRL);
            this.parent.RaiseOnNavigateComplete2(this.parent, e);
            uRL = e.uRL;
        }

        public virtual void NavigateError(object pDisp, ref object uRL, ref object frame, ref object statusCode, ref bool cancel)
        {
            DWebBrowserEvents2_NavigateErrorEvent e = new DWebBrowserEvents2_NavigateErrorEvent(pDisp, uRL, frame, statusCode, cancel);
            this.parent.RaiseOnNavigateError(this.parent, e);
            uRL = e.uRL;
            frame = e.frame;
            statusCode = e.statusCode;
            cancel = e.cancel;
        }

        public virtual void NewWindow2(ref object ppDisp, ref bool cancel)
        {
            DWebBrowserEvents2_NewWindow2Event e = new DWebBrowserEvents2_NewWindow2Event(ppDisp, cancel);
            this.parent.RaiseOnNewWindow2(this.parent, e);
            ppDisp = e.ppDisp;
            cancel = e.cancel;
        }

        public virtual void NewWindow3(ref object ppDisp, ref bool cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            DWebBrowserEvents2_NewWindow3Event e = new DWebBrowserEvents2_NewWindow3Event(ppDisp, cancel, dwFlags, bstrUrlContext, bstrUrl);
            this.parent.RaiseOnNewWindow3(this.parent, e);
            ppDisp = e.ppDisp;
            cancel = e.cancel;
        }

        public virtual void OnFullScreen(bool fullScreen)
        {
            DWebBrowserEvents2_OnFullScreenEvent e = new DWebBrowserEvents2_OnFullScreenEvent(fullScreen);
            this.parent.RaiseOnOnFullScreen(this.parent, e);
        }

        public virtual void OnMenuBar(bool menuBar)
        {
            DWebBrowserEvents2_OnMenuBarEvent e = new DWebBrowserEvents2_OnMenuBarEvent(menuBar);
            this.parent.RaiseOnOnMenuBar(this.parent, e);
        }

        public virtual void OnQuit()
        {
            EventArgs e = new EventArgs();
            this.parent.RaiseOnOnQuit(this.parent, e);
        }

        public virtual void OnStatusBar(bool statusBar)
        {
            DWebBrowserEvents2_OnStatusBarEvent e = new DWebBrowserEvents2_OnStatusBarEvent(statusBar);
            this.parent.RaiseOnOnStatusBar(this.parent, e);
        }

        public virtual void OnTheaterMode(bool theaterMode)
        {
            DWebBrowserEvents2_OnTheaterModeEvent e = new DWebBrowserEvents2_OnTheaterModeEvent(theaterMode);
            this.parent.RaiseOnOnTheaterMode(this.parent, e);
        }

        public virtual void OnToolBar(bool toolBar)
        {
            DWebBrowserEvents2_OnToolBarEvent e = new DWebBrowserEvents2_OnToolBarEvent(toolBar);
            this.parent.RaiseOnOnToolBar(this.parent, e);
        }

        public virtual void OnVisible(bool visible)
        {
            DWebBrowserEvents2_OnVisibleEvent e = new DWebBrowserEvents2_OnVisibleEvent(visible);
            this.parent.RaiseOnOnVisible(this.parent, e);
        }

        public virtual void PrintTemplateInstantiation(object pDisp)
        {
            DWebBrowserEvents2_PrintTemplateInstantiationEvent e = new DWebBrowserEvents2_PrintTemplateInstantiationEvent(pDisp);
            this.parent.RaiseOnPrintTemplateInstantiation(this.parent, e);
        }

        public virtual void PrintTemplateTeardown(object pDisp)
        {
            DWebBrowserEvents2_PrintTemplateTeardownEvent e = new DWebBrowserEvents2_PrintTemplateTeardownEvent(pDisp);
            this.parent.RaiseOnPrintTemplateTeardown(this.parent, e);
        }

        public virtual void PrivacyImpactedStateChange(bool bImpacted)
        {
            DWebBrowserEvents2_PrivacyImpactedStateChangeEvent e = new DWebBrowserEvents2_PrivacyImpactedStateChangeEvent(bImpacted);
            this.parent.RaiseOnPrivacyImpactedStateChange(this.parent, e);
        }

        public virtual void ProgressChange(int progress, int progressMax)
        {
            DWebBrowserEvents2_ProgressChangeEvent e = new DWebBrowserEvents2_ProgressChangeEvent(progress, progressMax);
            this.parent.RaiseOnProgressChange(this.parent, e);
        }

        public virtual void PropertyChange(string szProperty)
        {
            DWebBrowserEvents2_PropertyChangeEvent e = new DWebBrowserEvents2_PropertyChangeEvent(szProperty);
            this.parent.RaiseOnPropertyChange(this.parent, e);
        }

        public virtual void SetSecureLockIcon(int secureLockIcon)
        {
            DWebBrowserEvents2_SetSecureLockIconEvent e = new DWebBrowserEvents2_SetSecureLockIconEvent(secureLockIcon);
            this.parent.RaiseOnSetSecureLockIcon(this.parent, e);
        }

        public virtual void StatusTextChange(string text)
        {
            DWebBrowserEvents2_StatusTextChangeEvent e = new DWebBrowserEvents2_StatusTextChangeEvent(text);
            this.parent.RaiseOnStatusTextChange(this.parent, e);
        }

        public virtual void TitleChange(string text)
        {
            DWebBrowserEvents2_TitleChangeEvent e = new DWebBrowserEvents2_TitleChangeEvent(text);
            this.parent.RaiseOnTitleChange(this.parent, e);
        }

        public virtual void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone)
        {
            DWebBrowserEvents2_UpdatePageStatusEvent e = new DWebBrowserEvents2_UpdatePageStatusEvent(pDisp, nPage, fDone);
            this.parent.RaiseOnUpdatePageStatus(this.parent, e);
            nPage = e.nPage;
            fDone = e.fDone;
        }

        public virtual void WindowClosing(bool isChildWindow, ref bool cancel)
        {
            DWebBrowserEvents2_WindowClosingEvent e = new DWebBrowserEvents2_WindowClosingEvent(isChildWindow, cancel);
            this.parent.RaiseOnWindowClosing(this.parent, e);
            cancel = e.cancel;
        }

        public virtual void WindowSetHeight(int height)
        {
            DWebBrowserEvents2_WindowSetHeightEvent e = new DWebBrowserEvents2_WindowSetHeightEvent(height);
            this.parent.RaiseOnWindowSetHeight(this.parent, e);
        }

        public virtual void WindowSetLeft(int left)
        {
            DWebBrowserEvents2_WindowSetLeftEvent e = new DWebBrowserEvents2_WindowSetLeftEvent(left);
            this.parent.RaiseOnWindowSetLeft(this.parent, e);
        }

        public virtual void WindowSetResizable(bool resizable)
        {
            DWebBrowserEvents2_WindowSetResizableEvent e = new DWebBrowserEvents2_WindowSetResizableEvent(resizable);
            this.parent.RaiseOnWindowSetResizable(this.parent, e);
        }

        public virtual void WindowSetTop(int top)
        {
            DWebBrowserEvents2_WindowSetTopEvent e = new DWebBrowserEvents2_WindowSetTopEvent(top);
            this.parent.RaiseOnWindowSetTop(this.parent, e);
        }

        public virtual void WindowSetWidth(int width)
        {
            DWebBrowserEvents2_WindowSetWidthEvent e = new DWebBrowserEvents2_WindowSetWidthEvent(width);
            this.parent.RaiseOnWindowSetWidth(this.parent, e);
        }
    }
    internal delegate void DWebBrowserEvents2_BeforeNavigate2EventHandler(object sender, DWebBrowserEvents2_BeforeNavigate2Event e);
    internal class DWebBrowserEvents2_ClientToHostWindowEvent
    {
        public int cX;
        public int cY;

        public DWebBrowserEvents2_ClientToHostWindowEvent(int cX, int cY)
        {
            this.cX = cX;
            this.cY = cY;
        }
    }
    internal delegate void DWebBrowserEvents2_ClientToHostWindowEventHandler(object sender, DWebBrowserEvents2_ClientToHostWindowEvent e);
    internal class DWebBrowserEvents2_CommandStateChangeEvent
    {
        public int command;
        public bool enable;

        public DWebBrowserEvents2_CommandStateChangeEvent(int command, bool enable)
        {
            this.command = command;
            this.enable = enable;
        }
    }
    internal delegate void DWebBrowserEvents2_CommandStateChangeEventHandler(object sender, DWebBrowserEvents2_CommandStateChangeEvent e);
    internal class DWebBrowserEvents2_DocumentCompleteEvent
    {
        public object pDisp;
        public object uRL;

        public DWebBrowserEvents2_DocumentCompleteEvent(object pDisp, object uRL)
        {
            this.pDisp = pDisp;
            this.uRL = uRL;
        }
    }
    internal delegate void DWebBrowserEvents2_DocumentCompleteEventHandler(object sender, DWebBrowserEvents2_DocumentCompleteEvent e);
    internal class DWebBrowserEvents2_FileDownloadEvent
    {
        public bool cancel;

        public DWebBrowserEvents2_FileDownloadEvent(bool cancel)
        {
            this.cancel = cancel;
        }
    }
    internal delegate void DWebBrowserEvents2_FileDownloadEventHandler(object sender, DWebBrowserEvents2_FileDownloadEvent e);
    internal class DWebBrowserEvents2_NavigateComplete2Event
    {
        public object pDisp;
        public object uRL;

        public DWebBrowserEvents2_NavigateComplete2Event(object pDisp, object uRL)
        {
            this.pDisp = pDisp;
            this.uRL = uRL;
        }
    }
    internal delegate void DWebBrowserEvents2_NavigateComplete2EventHandler(object sender, DWebBrowserEvents2_NavigateComplete2Event e);
    internal class DWebBrowserEvents2_NavigateErrorEvent
    {
        public bool cancel;
        public object frame;
        public object pDisp;
        public object statusCode;
        public object uRL;

        public DWebBrowserEvents2_NavigateErrorEvent(object pDisp, object uRL, object frame, object statusCode, bool cancel)
        {
            this.pDisp = pDisp;
            this.uRL = uRL;
            this.frame = frame;
            this.statusCode = statusCode;
            this.cancel = cancel;
        }
    }
    internal delegate void DWebBrowserEvents2_NavigateErrorEventHandler(object sender, DWebBrowserEvents2_NavigateErrorEvent e);
    internal class DWebBrowserEvents2_NewWindow2Event
    {
        public bool cancel;
        public object ppDisp;

        public DWebBrowserEvents2_NewWindow2Event(object ppDisp, bool cancel)
        {
            this.ppDisp = ppDisp;
            this.cancel = cancel;
        }
    }
    internal delegate void DWebBrowserEvents2_NewWindow2EventHandler(object sender, DWebBrowserEvents2_NewWindow2Event e);
    internal class DWebBrowserEvents2_NewWindow3Event
    {
        public string bstrUrl;
        public string bstrUrlContext;
        public bool cancel;
        public uint dwFlags;
        public object ppDisp;

        public DWebBrowserEvents2_NewWindow3Event(object ppDisp, bool cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            this.ppDisp = ppDisp;
            this.cancel = cancel;
            this.dwFlags = dwFlags;
            this.bstrUrlContext = bstrUrlContext;
            this.bstrUrl = bstrUrl;
        }
    }
    internal delegate void DWebBrowserEvents2_NewWindow3EventHandler(object sender, DWebBrowserEvents2_NewWindow3Event e);
    internal class DWebBrowserEvents2_OnFullScreenEvent
    {
        public bool fullScreen;

        public DWebBrowserEvents2_OnFullScreenEvent(bool fullScreen)
        {
            this.fullScreen = fullScreen;
        }
    }
    internal delegate void DWebBrowserEvents2_OnFullScreenEventHandler(object sender, DWebBrowserEvents2_OnFullScreenEvent e);
    internal class DWebBrowserEvents2_OnMenuBarEvent
    {
        public bool menuBar;

        public DWebBrowserEvents2_OnMenuBarEvent(bool menuBar)
        {
            this.menuBar = menuBar;
        }
    }
    internal delegate void DWebBrowserEvents2_OnMenuBarEventHandler(object sender, DWebBrowserEvents2_OnMenuBarEvent e);
    internal class DWebBrowserEvents2_OnStatusBarEvent
    {
        public bool statusBar;

        public DWebBrowserEvents2_OnStatusBarEvent(bool statusBar)
        {
            this.statusBar = statusBar;
        }
    }
    internal delegate void DWebBrowserEvents2_OnStatusBarEventHandler(object sender, DWebBrowserEvents2_OnStatusBarEvent e);
    internal class DWebBrowserEvents2_OnTheaterModeEvent
    {
        public bool theaterMode;

        public DWebBrowserEvents2_OnTheaterModeEvent(bool theaterMode)
        {
            this.theaterMode = theaterMode;
        }
    }
    internal delegate void DWebBrowserEvents2_OnTheaterModeEventHandler(object sender, DWebBrowserEvents2_OnTheaterModeEvent e);
    internal class DWebBrowserEvents2_OnToolBarEvent
    {
        public bool toolBar;

        public DWebBrowserEvents2_OnToolBarEvent(bool toolBar)
        {
            this.toolBar = toolBar;
        }
    }
    internal delegate void DWebBrowserEvents2_OnToolBarEventHandler(object sender, DWebBrowserEvents2_OnToolBarEvent e);
    internal class DWebBrowserEvents2_OnVisibleEvent
    {
        public bool visible;

        public DWebBrowserEvents2_OnVisibleEvent(bool visible)
        {
            this.visible = visible;
        }
    }
    internal delegate void DWebBrowserEvents2_OnVisibleEventHandler(object sender, DWebBrowserEvents2_OnVisibleEvent e);
    internal class DWebBrowserEvents2_PrintTemplateInstantiationEvent
    {
        public object pDisp;

        public DWebBrowserEvents2_PrintTemplateInstantiationEvent(object pDisp)
        {
            this.pDisp = pDisp;
        }
    }
    internal delegate void DWebBrowserEvents2_PrintTemplateInstantiationEventHandler(object sender, DWebBrowserEvents2_PrintTemplateInstantiationEvent e);
    internal class DWebBrowserEvents2_PrintTemplateTeardownEvent
    {
        public object pDisp;

        public DWebBrowserEvents2_PrintTemplateTeardownEvent(object pDisp)
        {
            this.pDisp = pDisp;
        }
    }
    internal delegate void DWebBrowserEvents2_PrintTemplateTeardownEventHandler(object sender, DWebBrowserEvents2_PrintTemplateTeardownEvent e);
    internal class DWebBrowserEvents2_PrivacyImpactedStateChangeEvent
    {
        public bool bImpacted;

        public DWebBrowserEvents2_PrivacyImpactedStateChangeEvent(bool bImpacted)
        {
            this.bImpacted = bImpacted;
        }
    }
    internal delegate void DWebBrowserEvents2_PrivacyImpactedStateChangeEventHandler(object sender, DWebBrowserEvents2_PrivacyImpactedStateChangeEvent e);
    internal class DWebBrowserEvents2_ProgressChangeEvent
    {
        public int progress;
        public int progressMax;

        public DWebBrowserEvents2_ProgressChangeEvent(int progress, int progressMax)
        {
            this.progress = progress;
            this.progressMax = progressMax;
        }
    }
    internal delegate void DWebBrowserEvents2_ProgressChangeEventHandler(object sender, DWebBrowserEvents2_ProgressChangeEvent e);
    internal class DWebBrowserEvents2_PropertyChangeEvent
    {
        public string szProperty;

        public DWebBrowserEvents2_PropertyChangeEvent(string szProperty)
        {
            this.szProperty = szProperty;
        }
    }
    internal delegate void DWebBrowserEvents2_PropertyChangeEventHandler(object sender, DWebBrowserEvents2_PropertyChangeEvent e);
    internal class DWebBrowserEvents2_SetSecureLockIconEvent
    {
        public int secureLockIcon;

        public DWebBrowserEvents2_SetSecureLockIconEvent(int secureLockIcon)
        {
            this.secureLockIcon = secureLockIcon;
        }
    }
    internal delegate void DWebBrowserEvents2_SetSecureLockIconEventHandler(object sender, DWebBrowserEvents2_SetSecureLockIconEvent e);
    internal class DWebBrowserEvents2_StatusTextChangeEvent
    {
        public string text;

        public DWebBrowserEvents2_StatusTextChangeEvent(string text)
        {
            this.text = text;
        }
    }
    internal delegate void DWebBrowserEvents2_StatusTextChangeEventHandler(object sender, DWebBrowserEvents2_StatusTextChangeEvent e);
    internal class DWebBrowserEvents2_TitleChangeEvent
    {
        public string text;

        public DWebBrowserEvents2_TitleChangeEvent(string text)
        {
            this.text = text;
        }
    }
    internal delegate void DWebBrowserEvents2_TitleChangeEventHandler(object sender, DWebBrowserEvents2_TitleChangeEvent e);
    internal class DWebBrowserEvents2_UpdatePageStatusEvent
    {
        public object fDone;
        public object nPage;
        public object pDisp;

        public DWebBrowserEvents2_UpdatePageStatusEvent(object pDisp, object nPage, object fDone)
        {
            this.pDisp = pDisp;
            this.nPage = nPage;
            this.fDone = fDone;
        }
    }
    internal delegate void DWebBrowserEvents2_UpdatePageStatusEventHandler(object sender, DWebBrowserEvents2_UpdatePageStatusEvent e);
    internal class DWebBrowserEvents2_WindowClosingEvent
    {
        public bool cancel;
        public bool isChildWindow;

        public DWebBrowserEvents2_WindowClosingEvent(bool isChildWindow, bool cancel)
        {
            this.isChildWindow = isChildWindow;
            this.cancel = cancel;
        }
    }
    internal delegate void DWebBrowserEvents2_WindowClosingEventHandler(object sender, DWebBrowserEvents2_WindowClosingEvent e);
    internal class DWebBrowserEvents2_WindowSetHeightEvent
    {
        public int height;

        public DWebBrowserEvents2_WindowSetHeightEvent(int height)
        {
            this.height = height;
        }
    }
    internal delegate void DWebBrowserEvents2_WindowSetHeightEventHandler(object sender, DWebBrowserEvents2_WindowSetHeightEvent e);
    internal class DWebBrowserEvents2_WindowSetLeftEvent
    {
        public int left;

        public DWebBrowserEvents2_WindowSetLeftEvent(int left)
        {
            this.left = left;
        }
    }
    internal delegate void DWebBrowserEvents2_WindowSetLeftEventHandler(object sender, DWebBrowserEvents2_WindowSetLeftEvent e);
    internal class DWebBrowserEvents2_WindowSetResizableEvent
    {
        public bool resizable;

        public DWebBrowserEvents2_WindowSetResizableEvent(bool resizable)
        {
            this.resizable = resizable;
        }
    }
    internal delegate void DWebBrowserEvents2_WindowSetResizableEventHandler(object sender, DWebBrowserEvents2_WindowSetResizableEvent e);
    internal class DWebBrowserEvents2_WindowSetTopEvent
    {
        public int top;

        public DWebBrowserEvents2_WindowSetTopEvent(int top)
        {
            this.top = top;
        }
    }
    internal delegate void DWebBrowserEvents2_WindowSetTopEventHandler(object sender, DWebBrowserEvents2_WindowSetTopEvent e);
    internal class DWebBrowserEvents2_WindowSetWidthEvent
    {
        public int width;

        public DWebBrowserEvents2_WindowSetWidthEvent(int width)
        {
            this.width = width;
        }
    }
    internal delegate void DWebBrowserEvents2_WindowSetWidthEventHandler(object sender, DWebBrowserEvents2_WindowSetWidthEvent e);
    #endregion

    #region Shdocw Events

    #endregion

    #region ProcessUrlActionEventArgs
    internal delegate void ProcessUrlActionEventHandler(object sender, ProcessUrlActionEventArgs e);
    internal class ProcessUrlActionEventArgs : System.ComponentModel.CancelEventArgs
    {
        public bool handled;
        public bool hasContext;
        public string url;
        public URLACTION urlAction;
        public URLPOLICY urlPolicy;
        public Guid context;
        public ProcessUrlActionFlags flags;

        public ProcessUrlActionEventArgs() { }

        public void SetParameters(string surl, URLACTION action, URLPOLICY policy, Guid gcontext, ProcessUrlActionFlags puaf, bool bhascontext)
        {
            this.Cancel = false;
            this.handled = false;

            this.url = surl;
            this.urlAction = action;
            this.urlPolicy = policy;
            this.context = gcontext;
            this.flags = puaf;
            this.hasContext = bhascontext;
        }

        public void ResetParameters()
        {
            this.Cancel = false;
            this.handled = false;
            this.url = string.Empty;
            this.urlAction = URLACTION.MIN;
            this.urlPolicy = URLPOLICY.ALLOW;
            this.context = Guid.Empty;
            this.flags = ProcessUrlActionFlags.PUAF_DEFAULT;
            this.hasContext = false;
        }
    }
    #endregion

}
