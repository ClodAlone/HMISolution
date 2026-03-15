#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// DateTimePickerAdv Menu class.
    /// </summary>
    [ToolboxItem(false)]
    public class DateTimePickerAdvMenu : ContextMenu, IDateTimePickerAdvMenu
    {
        public event EventHandler Cut;
        public event EventHandler Copy;
        public event EventHandler Paste;
        public event EventHandler NoDateTime;
        public CultureInfo currentCulture;
        private MenuItem cutItem;
        private MenuItem copyItem;
        private MenuItem pasteItem;
        private MenuItem noDateTimeItem;
        private DateTimePickerAdv m_DateTimePicker=null ;
        private bool m_bInitialized = false;

        public bool NoDateTimeChecked
        {
            get { return noDateTimeItem.Checked; }
            set { noDateTimeItem.Checked = value; }
        }

        public bool ShowNoDateTime
        {
            get { return noDateTimeItem.Visible; }
            set { noDateTimeItem.Visible = value; }
        }
        public bool CopyEnabled
        {
            get { return copyItem.Enabled; }
            set { copyItem.Enabled = value; }
        }
        public bool CutEnabled
        {
            get { return cutItem.Enabled; }
            set { cutItem.Enabled = value; }
        }
        public bool PasteEnabled
        {
            get { return pasteItem.Enabled; }
            set { pasteItem.Enabled = value; }
        }
        public bool NoDateTimeEnabled
        {
            get { return noDateTimeItem.Enabled; }
            set { noDateTimeItem.Enabled = value; }
        }

        public DateTimePickerAdvMenu()
        {
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (cutItem != null)
                {
                    cutItem.Dispose();
                    cutItem = null;
                }
                if (copyItem != null)
                {
                    copyItem.Dispose();
                    copyItem = null;
                }
                if (pasteItem != null)
                {
                    pasteItem.Dispose();
                    pasteItem = null;
                }
                if (noDateTimeItem != null)
                {
                    noDateTimeItem.Dispose();
                    noDateTimeItem = null;
                }
            }
            base.Dispose(disposing);
        }
        protected override void OnPopup(EventArgs e)
        {
            if (!m_bInitialized)
                this.InitMenuItems();

            base.OnPopup(e);
        }

        private void InitMenuItems()
        {
            cutItem = new MenuItem(SR.GetString(currentCulture, SR.Cut, m_DateTimePicker), new EventHandler(CutItem_Click), Shortcut.CtrlX);
            copyItem = new MenuItem(SR.GetString(currentCulture, SR.Copy, m_DateTimePicker), new EventHandler(CopyItem_Click), Shortcut.CtrlC);
            pasteItem = new MenuItem(SR.GetString(currentCulture, SR.Paste, m_DateTimePicker), new EventHandler(PasteItem_Click), Shortcut.CtrlV);
            noDateTimeItem = new MenuItem(SR.GetString(currentCulture, SR.DateTimePickerNoDate, m_DateTimePicker), new EventHandler(NoDateTimeItem_Click), Shortcut.Del);

            cutItem.ShowShortcut = false;
            copyItem.ShowShortcut = false;
            pasteItem.ShowShortcut = false;
            noDateTimeItem.ShowShortcut = false;

            this.MenuItems.Add(cutItem);
            this.MenuItems.Add(copyItem);
            this.MenuItems.Add(pasteItem);
            this.MenuItems.Add(new MenuItem("-"));
            this.MenuItems.Add(noDateTimeItem);

            m_bInitialized = true;
        }

        private void CutItem_Click(object sender, EventArgs e)
        {
            if (Cut != null) Cut(this, e);
        }
        private void CopyItem_Click(object sender, EventArgs e)
        {
            if (Copy != null) Copy(this, e);
        }
        private void PasteItem_Click(object sender, EventArgs e)
        {
            if (Paste != null) Paste(this, e);
        }
        private void NoDateTimeItem_Click(object sender, EventArgs e)
        {
            if (NoDateTime != null) NoDateTime(this, e);
        }

        public void ShowPopup(Control control, System.Drawing.Point pt)
        {
            if (control is DateTimePickerAdv)
			{
				m_DateTimePicker = control as DateTimePickerAdv;
                currentCulture = (control as DateTimePickerAdv).Culture;
			}
            this.Show(control, pt);
        }
    }
}
