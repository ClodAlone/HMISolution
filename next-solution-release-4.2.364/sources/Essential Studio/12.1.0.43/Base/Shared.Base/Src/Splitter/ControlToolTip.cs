#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.ComponentModel;


namespace Syncfusion.Windows.Forms
{

    [Syncfusion.Documentation.DocumentationExclude()]
	public interface IControlToolTipProvider 
    {
        ControlToolTip GetControlToolTip();
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    public class ControlToolTip 
		: Disposable
    {
		[Syncfusion.Documentation.DocumentationExclude()]
			internal class TTM
        {
            static TTM() 
            {
                ADDTOOL = ADDTOOLW;
                DELTOOL = DELTOOLW;
                ENUMTOOLS = ENUMTOOLSW;
                GETCURRENTTOOL = GETCURRENTTOOLW;
                GETTEXT = GETTEXTW;
                GETTOOLINFO = GETTOOLINFOW;
                HITTEST = HITTESTW;
                NEWTOOLRECT = NEWTOOLRECTW;
                SETTOOLINFO = SETTOOLINFOW;
                UPDATETIPTEXT = UPDATETIPTEXTW;
            }

            public const int ACTIVATE = 1025; // 0x0401 
            public readonly static int ADDTOOL;
            public const int ADDTOOLA = 1028; // 0x0404 
            public const int ADDTOOLW = 1074; // 0x0432 
            public const int ADJUSTRECT = 1055; // 0x041f 
            public readonly static int DELTOOL;
            public const int DELTOOLA = 1029; // 0x0405 
            public const int DELTOOLW = 1075; // 0x0433 
            public readonly static int ENUMTOOLS;
            public const int ENUMTOOLSA = 1038; // 0x040e 
            public const int ENUMTOOLSW = 1082; // 0x043a 
            public readonly static int GETCURRENTTOOL;
            public const int GETCURRENTTOOLA = 1039; // 0x040f 
            public const int GETCURRENTTOOLW = 1083; // 0x043b 
            public const int GETDELAYTIME = 1045; // 0x0415 
            public const int GETMARGIN = 1051; // 0x041b 
            public const int GETMAXTIPWIDTH = 1049; // 0x0419 
            public readonly static int GETTEXT;
            public const int GETTEXTA = 1035; // 0x040b 
            public const int GETTEXTW = 1080; // 0x0438 
            public const int GETTIPBKCOLOR = 1046; // 0x0416 
            public const int GETTIPTEXTCOLOR = 1047; // 0x0417 
            public const int GETTOOLCOUNT = 1037; // 0x040d 
            public readonly static int GETTOOLINFO;
            public const int GETTOOLINFOA = 1032; // 0x0408 
            public const int GETTOOLINFOW = 1077; // 0x0435 
            public readonly static int HITTEST;
            public const int HITTESTA = 1034; // 0x040a 
            public const int HITTESTW = 1079; // 0x0437 
            public readonly static int NEWTOOLRECT;
            public const int NEWTOOLRECTA = 1030; // 0x0406 
            public const int NEWTOOLRECTW = 1076; // 0x0434 
            public const int POP = 1052; // 0x041c 
            public const int RELAYEVENT = 1031; // 0x0407 
            public const int SETDELAYTIME = 1027; // 0x0403 
            public const int SETMARGIN = 1050; // 0x041a 
            public const int SETMAXTIPWIDTH = 1048; // 0x0418 
            public const int SETTIPBKCOLOR = 1043; // 0x0413 
            public const int SETTIPTEXTCOLOR = 1044; // 0x0414 
            public readonly static int SETTOOLINFO;
            public const int SETTOOLINFOA = 1033; // 0x0409 
            public const int SETTOOLINFOW = 1078; // 0x0436 
            public const int TRACKACTIVATE = 1041; // 0x0411 
            public const int TRACKPOSITION = 1042; // 0x0412 
            public const int UPDATE = 1053; // 0x041d 
            public readonly static int UPDATETIPTEXT;
            public const int UPDATETIPTEXTA = 1036; // 0x040c 
            public const int UPDATETIPTEXTW = 1081; // 0x0439 
            public const int WINDOWFROMPOINT = 1040; // 0x0410 
        }

        // Fields
        private Control control = null;
        private NativeWindow tipWindow = null;
    	private int toolTipId = 0;
	    public readonly static string TOOLTIPS_CLASS = "tooltips_class32";
    	public readonly static string TOOLTIPS_CLASSA = "tooltips_class";
    	public readonly static string TOOLTIPS_CLASSW = "tooltips_class32";

        // Constructors
        public ControlToolTip(Control Control)
        {
            this.control = Control;
        }

		protected override void Dispose(bool disposing)
		{
			this.Destroy();
			base.Dispose (disposing);
		}


        public int RequestNextToolTipId()
        {
            return toolTipId++;
        }

        // Methods
        public void AddToolTip(string toolTipString, int toolTipId, Rectangle iconBounds)  
        {
			CreateToolTipHandle();

            if (this.control == null || this.tipWindow == null || !this.control.IsHandleCreated)
                return;

            NativeMethods.TOOLINFO_T toolinfo;

            if (toolTipString == null)
                throw new ArgumentNullException(toolTipString);

            if (iconBounds.IsEmpty)
                throw new ArgumentException(SR.GetString(@"TabBarToolTipEmptyIcon", "iconBounds"));
            
            toolinfo = new NativeMethods.TOOLINFO_T();
            toolinfo.cbSize = Marshal.SizeOf(toolinfo);
            toolinfo.hWnd = this.control.Handle;
            toolinfo.uId = toolTipId;
            toolinfo.lpszText = toolTipString;
            toolinfo.rect = NativeMethods.RECT.FromXYWH(iconBounds.X,iconBounds.Y,iconBounds.Width,iconBounds.Height);
            toolinfo.uFlags = 16/*TTF_SUBCLASS*/;

            NativeMethods.SendMessage(this.tipWindow.Handle, TTM.ADDTOOL, 0, toolinfo);
        }

        public void CreateToolTipHandle()  
        {
            if (this.control == null || !this.control.IsHandleCreated)
                return;

            NativeMethods.INITCOMMONCONTROLSEX cc;
            System.Windows.Forms.CreateParams cp;
            System.Drawing.Size size;

            if (this.tipWindow == null || this.tipWindow.Handle == IntPtr.Zero)
            {
                cc = new NativeMethods.INITCOMMONCONTROLSEX();
                cc.dwICC = 8;
                cc.dwSize = Marshal.SizeOf(cc);
                NativeMethods.InitCommonControlsEx(cc);
                cp = new CreateParams();
                cp.Parent = this.control.Handle;
                cp.ClassName = TOOLTIPS_CLASS;
                cp.Style = 1;
                this.tipWindow = new NativeWindow();
                this.tipWindow.CreateHandle(cp);
                size = SystemInformation.MaxWindowTrackSize;
                NativeMethods.SendMessage(this.tipWindow.Handle,0x418/*TTM_SETMAXTIPWIDTH*/,0,size.Width);
                NativeMethods.SetWindowPos(this.tipWindow.Handle, (IntPtr) NativeMethods.HWND_NOTOPMOST,0,0,0,0,19);
                NativeMethods.SendMessage(this.tipWindow.Handle,0x403/*TTM_SETDELAYTIME*/,50,0);
            }
        }

        public void DeactivateToolTip()  
        {
			if (this.tipWindow != null)
	            NativeMethods.SendMessage(this.tipWindow.Handle,0x401/*TTM_ACTIVATE*/,0,0);
        }

        public void ActivateToolTip()  
        {
			if (this.tipWindow != null)
				NativeMethods.SendMessage(this.tipWindow.Handle,0x401/*TTM_ACTIVATE*/,1,0);
        }

        public void InitToolTip(ref int tooltipID, string toolTipText, Rectangle bounds)
        {
			if (this.tipWindow == null)
				return;
                
            bool reset = (bounds.IsEmpty || toolTipText == null || toolTipText.Length == 0);

            if (tooltipID == -1 && !reset)
            {
                tooltipID = this.RequestNextToolTipId();
                this.AddToolTip(toolTipText, tooltipID, bounds);
            }
            else if (tooltipID != -1 && reset)
            {
                this.RemoveToolTip(tooltipID);
                tooltipID = -1;
            }
        }

        public void Destroy()  
        {
			if (this.tipWindow != null)
				this.tipWindow.DestroyHandle();
            this.tipWindow = null;
            this.toolTipId = 0;
        }

        public void RemoveToolTip(int toolTipId)  
        {
            if (this.control == null || this.tipWindow == null || !this.control.IsHandleCreated)
                return;

            NativeMethods.TOOLINFO_T toolinfo;
            toolinfo = new NativeMethods.TOOLINFO_T();
            toolinfo.cbSize = Marshal.SizeOf(toolinfo);
            toolinfo.hWnd = this.control.Handle;
            toolinfo.uId = toolTipId;
            NativeMethods.SendMessage(this.tipWindow.Handle,TTM.DELTOOL,0, toolinfo);
        }
    }
}
