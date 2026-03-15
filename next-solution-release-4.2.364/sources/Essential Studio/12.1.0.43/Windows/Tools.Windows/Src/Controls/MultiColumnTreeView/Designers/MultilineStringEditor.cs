#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Microsoft.Win32;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Tools.Controls.MultiColumnTreeView.Designers
{
    /// <summary>
    /// Class is used for TreeNodeAdv property Text
    /// </summary>
    public sealed class MultilineStringEditor : UITypeEditor
    {
        #region members
        // Fields
        private MultilineStringEditorUI _editorUI;
        #endregion

        #region overrides

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService == null)
                {
                    return value;
                }
                if (this._editorUI == null)
                {
                    this._editorUI = new MultilineStringEditorUI();
                }
                this._editorUI.BeginEdit(editorService, value);
                editorService.DropDownControl(this._editorUI);
                object obj2 = this._editorUI.Value;
                if (this._editorUI.EndEdit())
                {
                    value = obj2;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }
        #endregion

        #region helper types
        // Nested Types
        private class MultilineStringEditorUI : RichTextBox
        {
            // Fields
            private const int _caretPadding = 3;
            private const int _workAreaPadding = 0x10;
            private readonly StringFormat _watermarkFormat;
            private bool _contentsResizedRaised;
            private bool _ctrlEnterPressed;
            private IWindowsFormsEditorService _editorService;
            private bool _escapePressed;
            private Hashtable _fallbackFonts;
            private Size _minimumSize = Size.Empty;
            private SolidBrush _watermarkBrush;

            private Size _watermarkSize = Size.Empty;

            internal MultilineStringEditorUI()
            {
                this.InitializeComponent();
                this._watermarkFormat = new StringFormat();
                this._watermarkFormat.Alignment = StringAlignment.Center;
                this._watermarkFormat.LineAlignment = StringAlignment.Center;
                this._fallbackFonts = new Hashtable(2);
            }

            internal void BeginEdit(IWindowsFormsEditorService editorService, object value)
            {
                this._editorService = editorService;
                this._minimumSize = Size.Empty;
                this._watermarkSize = Size.Empty;
                this._escapePressed = false;
                this._ctrlEnterPressed = false;
                this.Text = (string)value;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && (this._watermarkBrush != null))
                {
                    this._watermarkBrush.Dispose();
                    this._watermarkBrush = null;
                }
                base.Dispose(disposing);
            }

            internal bool EndEdit()
            {
                this._editorService = null;
                this._ctrlEnterPressed = false;
                this.Text = null;
                return !this._escapePressed;
            }

            private void InitializeComponent()
            {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                base.RichTextShortcutsEnabled = false;
#endif
                base.WordWrap = false;
                base.BorderStyle = BorderStyle.None;
                this.Multiline = true;
                base.ScrollBars = RichTextBoxScrollBars.Both;
                base.DetectUrls = false;
            }

            protected override bool IsInputKey(Keys keyData)
            {
                return ((((keyData & Keys.KeyCode) == Keys.Return) && this.Multiline) && ((keyData & Keys.Alt) == Keys.None)) || base.IsInputKey(keyData);
            }

            protected override void OnContentsResized(ContentsResizedEventArgs e)
            {
                this._contentsResizedRaised = true;
                this.ResizeToContent();
                base.OnContentsResized(e);
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (this.ShouldShowWatermark)
                {
                    base.Invalidate();
                }
                if ((e.Control && (e.KeyCode == Keys.Return)) && (e.Modifiers == Keys.Control))
                {
                    this._editorService.CloseDropDown();
                    this._ctrlEnterPressed = true;
                }
            }

            protected override void OnTextChanged(EventArgs e)
            {
                if (!this._contentsResizedRaised)
                {
                    this.ResizeToContent();
                }
                this._contentsResizedRaised = false;
                base.OnTextChanged(e);
            }

            protected override void OnVisibleChanged(EventArgs e)
            {
                if (base.Visible)
                {
                    this.ProcessSurrogateFonts(0, this.Text.Length);
                    base.Select(this.Text.Length, 0);
                }
                this.ResizeToContent();
                base.OnVisibleChanged(e);
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                if ((keyData & (Keys.Alt | Keys.Shift)) == Keys.None)
                {
                    Keys keys = keyData & Keys.KeyCode;
                    if ((keys == Keys.Escape) && ((keyData & Keys.Control) == Keys.None))
                    {
                        this._escapePressed = true;
                    }
                }
                return base.ProcessDialogKey(keyData);
            }

            public void ProcessSurrogateFonts(int start, int length)
            {
                string str = this.Text;
                if (str != null)
                {
                    int[] numArray = StringInfo.ParseCombiningCharacters(str);
                    if (numArray.Length != str.Length)
                    {
                        for (int i = 0; i < numArray.Length; i++)
                        {
                            if ((numArray[i] >= start) && (numArray[i] < (start + length)))
                            {
                                string familyName = null;
                                char ch = str[numArray[i]];
                                char ch2 = '\0';
                                if ((numArray[i] + 1) < str.Length)
                                {
                                    ch2 = str[numArray[i] + 1];
                                }
                                if (((ch >= 0xd800) && (ch <= 0xdbff)) && ((ch2 >= 0xdc00) && (ch2 <= 0xdfff)))
                                {
                                    int num2 = ((ch / '@') - 0x360) + 1;
                                    Font font = this._fallbackFonts[num2] as Font;
                                    if (font == null)
                                    {
                                        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\LanguagePack\SurrogateFallback"))
                                        {
                                            if (key != null)
                                            {
                                                familyName = (string)key.GetValue("Plane" + num2);
                                                if (null != familyName && string.Empty != familyName)
                                                {
                                                    font = new Font(familyName, base.Font.Size, base.Font.Style);
                                                }
                                                this._fallbackFonts[num2] = font;
                                            }
                                        }
                                    }
                                    if (font != null)
                                    {
                                        int num3 = (i == (numArray.Length - 1)) ? (str.Length - numArray[i]) : (numArray[i + 1] - numArray[i]);
                                        base.Select(numArray[i], num3);
                                        base.SelectionFont = font;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private void ResizeToContent()
            {
                if (base.Visible)
                {
                    Size contentSize = this.ContentSize;
                    contentSize.Width += SystemInformation.VerticalScrollBarWidth;
#if	!( SyncfusionFramework1_0 || SyncfusionFramework1_1	)
                    contentSize.Width = Math.Max(contentSize.Width, this.MinimumSize.Width);
#else
			contentSize.Width =	contentSize.Width;
#endif
                    Rectangle workingArea = Screen.GetWorkingArea(this);
                    int num = base.PointToScreen(base.Location).X - workingArea.Left;
                    int num2 = Math.Min(contentSize.Width - base.ClientSize.Width, num);
#if	!( SyncfusionFramework1_0 || SyncfusionFramework1_1	)
                    base.ClientSize = new Size(base.ClientSize.Width + num2, this.MinimumSize.Height);
#else
			base.ClientSize	= new Size( base.ClientSize.Width + num2, base.ClientSize.Height );
#endif
                }
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if ((m.Msg == 15) && this.ShouldShowWatermark)
                {
                    using (Graphics graphics = base.CreateGraphics())
                    {
                        graphics.DrawString(SR.GetString("MultilineStringEditorWatermark"), this.Font, this.WatermarkBrush, new RectangleF(0f, 0f, (float)base.ClientSize.Width, (float)base.ClientSize.Height), this._watermarkFormat);
                    }
                }
            }

            private Size ContentSize
            {
                get
                {
                    NativeMethods.RECT lpRect = new NativeMethods.RECT();
                    IntPtr hDC = NativeMethods.GetDC(IntPtr.Zero);
                    IntPtr hObject = this.Font.ToHfont();
                    IntPtr ref4 = NativeMethods.SelectObject(hDC, hObject);
                    try
                    {
                        NativeMethods.DrawText(hDC, this.Text, this.Text.Length, ref lpRect, 0x400);
                    }
                    finally
                    {
                        NativeMethods.DeleteObject(hObject);
                        NativeMethods.SelectObject(hDC, ref4);
                        NativeMethods.ReleaseDC(IntPtr.Zero, hDC);
                    }
                    return new Size((lpRect.right - lpRect.left) + 3, lpRect.bottom - lpRect.top);
                }
            }

            public override Font Font
            {
                get
                {
                    return base.Font;
                }
                set
                {
                }
            }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
  
            public override Size MinimumSize
            {
                get
                {
                    if (this._minimumSize == Size.Empty)
                    {
                        Rectangle workingArea = Screen.GetWorkingArea(this);
                        this._minimumSize = new Size((int)Math.Min(Math.Ceiling(this.WatermarkSize.Width * 1.75), (double)(workingArea.Width / 4)), Math.Min((int)(this.Font.Height * 10), (int)(workingArea.Height / 4)));
                    }
                    return this._minimumSize;
                }
            }
#endif

            private bool ShouldShowWatermark
            {
                get
                {
                    if (this.Text.Length != 0)
                    {
                        return false;
                    }
                    return this.WatermarkSize.Width < base.ClientSize.Width;
                }
            }

            public override string Text
            {
                get
                {
                    if (!base.IsHandleCreated)
                    {
                        return string.Empty;
                    }
                    int capacity = NativeMethods.GetWindowTextLength(base.Handle) + 1;
                    StringBuilder lpString = new StringBuilder(capacity);
                    NativeMethods.GetWindowText(base.Handle, lpString, lpString.Capacity);
                    if (!this._ctrlEnterPressed)
                    {
                        return lpString.ToString();
                    }
                    string text = lpString.ToString();
                    int startIndex = text.LastIndexOf("\r\n");
                    return text.Remove(startIndex, 2);
                }
                set
                {
                    base.Text = value;
                }
            }

            internal object Value
            {
                get
                {
                    return this.Text;
                }
            }

            private Brush WatermarkBrush
            {
                get
                {
                    if (this._watermarkBrush == null)
                    {
                        Color window = SystemColors.Window;
                        Color windowText = SystemColors.WindowText;
                        Color color = Color.FromArgb((short)((windowText.R * 0.3) + (window.R * 0.7)), (short)((windowText.G * 0.3) + (window.G * 0.7)), (short)((windowText.B * 0.3) + (window.B * 0.7)));
                        this._watermarkBrush = new SolidBrush(color);
                    }
                    return this._watermarkBrush;
                }
            }

                    private Size WatermarkSize
            {
                get
                {
                    if (this._watermarkSize == Size.Empty)
                    {
                        SizeF ef = SizeF.Empty;

                        using (Graphics graphics = base.CreateGraphics())
                        {
                            ef = graphics.MeasureString(SR.GetString("MultilineStringEditorWatermark"), this.Font);
                        }

                        this._watermarkSize = new Size((int)Math.Ceiling((double)ef.Width), (int)Math.Ceiling((double)ef.Height));
                    }
                    return this._watermarkSize;
                }
            }
        }
        #endregion
    }
}
