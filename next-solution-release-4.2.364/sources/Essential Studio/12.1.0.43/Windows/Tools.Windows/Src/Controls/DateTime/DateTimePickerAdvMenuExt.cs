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

using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// DateTimePickerAdv MenuExt class.
    /// </summary>
    [ToolboxItem(false)]
    internal class DateTimePickerAdvMenuExt : Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu, IDateTimePickerAdvMenu
    {
        public event EventHandler Cut;
        public event EventHandler Copy;
        public event EventHandler Paste;
        public event EventHandler NoDateTime;
        public new event EventHandler Popup;
        private ParentBarItem parentBarItem;
        private BarItem cutBarItem;
        private BarItem copyBarItem;
        private BarItem pasteBarItem;
        private BarItem noDateTimeBarItem;

        public bool NoDateTimeChecked
        {
            get { return noDateTimeBarItem.Checked; }
            set { noDateTimeBarItem.Checked = value; }
        }
        public bool ShowNoDateTime
        {
            get { return noDateTimeBarItem.Visible; }
            set { noDateTimeBarItem.Visible = value; }
        }
        public bool CopyEnabled
        {
            get { return copyBarItem.Enabled; }
            set { copyBarItem.Enabled = value; }
        }
        public bool CutEnabled
        {
            get { return cutBarItem.Enabled; }
            set { cutBarItem.Enabled = value; }
        }
        public bool PasteEnabled
        {
            get { return pasteBarItem.Enabled; }
            set { pasteBarItem.Enabled = value; }
        }
        public bool NoDateTimeEnabled
        {
            get { return noDateTimeBarItem.Enabled; }
            set { noDateTimeBarItem.Enabled = value; }
        }
        public DateTimePickerAdvMenuExt()
            : base()
        {
            parentBarItem = new ParentBarItem();
            this.ParentBarItem = parentBarItem;

            cutBarItem = new BarItem("Cut", new EventHandler(CutBarItem_Click));
            copyBarItem = new BarItem("Copy", new EventHandler(CopyBarItem_Click));
            pasteBarItem = new BarItem("Paste", new EventHandler(PasteBarItem_Click));
            noDateTimeBarItem = new BarItem("No Date/Time", new EventHandler(NoDateTimeBarItem_Click));

            parentBarItem.Items.Add(cutBarItem);
            parentBarItem.Items.Add(copyBarItem);
            parentBarItem.Items.Add(pasteBarItem);
            parentBarItem.Items.Add(noDateTimeBarItem);

            parentBarItem.Popup += new EventHandler(ParetnBarItem_Popup);
        }

        private void ParetnBarItem_Popup(object sender, EventArgs e)
        {
            if (Popup != null)
            {
                Popup(this, e);
            }
        }

        private void CutBarItem_Click(object sender, EventArgs e)
        {
            if (Cut != null) Cut(this, e);
        }
        private void CopyBarItem_Click(object sender, EventArgs e)
        {
            if (Copy != null) Copy(this, e);
        }
        private void PasteBarItem_Click(object sender, EventArgs e)
        {
            if (Paste != null) Paste(this, e);
        }
        private void NoDateTimeBarItem_Click(object sender, EventArgs e)
        {
            if (NoDateTime != null) NoDateTime(this, e);
        }

        public void ShowPopup(Control control, System.Drawing.Point pt)
        {
            this.Show(control, pt);
        }
    }
}
