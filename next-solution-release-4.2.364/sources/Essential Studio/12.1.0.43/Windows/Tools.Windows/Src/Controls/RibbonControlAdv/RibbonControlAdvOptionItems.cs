#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing;
using System.Collections;
using Syncfusion.Windows.Forms.Tools.Design;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// RibbonDropDownContainer Class 
    /// </summary>
    [ToolboxItem(false)]
    public partial class RibbonDropDownContainer : Control
    {
        public RibbonDropDownContainer()
        {
            InitializeComponent();
            RibbonOptionDropDownCollections = new RibbonOptionDropDownCollection(this);
        }
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonOptionDropDownCollection RibbonOptionDropDownItems
        {
            get
            {
                return RibbonOptionDropDownCollections;
            }
        }
        int y;
        /// <summary>
        /// MenuColor for ControlItem
        /// </summary>
        private Color menuColor = ColorTranslator.FromHtml("#116EDA");
        /// <summary>
        /// Gets/Sets the value for MenuColor
        /// </summary>
        public Color MenuColor
        {
            get
            {
                return MenuColor;
            }
            set
            {
                menuColor = value;
                foreach (ControlItem ctrl in RibbonOptionDropDownItems)
                {
                    ctrl.ArrowColor = value;
                }
            }
        }
        /// <summary>
        /// OfficeColorScheme for ControlItem
        /// </summary>
        private Office2013ColorScheme colorScheme = Office2013ColorScheme.White;
        /// <summary>
        /// Gets/Sets the valuse for OfficeColorScheme
        /// </summary>
        public Office2013ColorScheme ColorScheme
        {
            get
            {
                return colorScheme;
            }
            set
            {
                colorScheme = value; 
                foreach (ControlItem ctrl in RibbonOptionDropDownItems)
                {
                    ctrl.ColorScheme = value;
                }
            }
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            foreach (ControlItem ctrl in RibbonOptionDropDownItems)
            {
                this.Controls.Add(ctrl);
                if (ctrl.HeaderText == "Mouse")
                {
                    y = 30;
                }
                ctrl.Location = new Point (0, y);
                y += ctrl.Height;
            }
            foreach (ControlItem ctrl in RibbonOptionDropDownItems)
            {
                ctrl.MouseUp += new MouseEventHandler(ctrl_MouseUp);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle rect = e.ClipRectangle;
            rect.Width-=1;
            rect.Height-=1;
            e.Graphics.DrawRectangle(new Pen(Color.Gray), rect);
        }
        void ctrl_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (ControlItem ctrl in RibbonOptionDropDownItems)
            {
                if ((sender as ControlItem).Name == ctrl.Name)
                {
                    ctrl.StatusCheck = true;
                }
                else
                {
                    ctrl.StatusCheck = false;
                }
            }
            this.Refresh();
        }
        private RibbonOptionDropDownCollection RibbonOptionDropDownCollections;
    }
    public class RibbonOptionDropDownCollection : CollectionBase
    {
        private RibbonDropDownContainer Owner;
        /// <summary>
        /// Constructor for RibbonOptionDropDownCollection
        /// </summary>
        /// <param name="sender"></param>
        public RibbonOptionDropDownCollection(RibbonDropDownContainer sender) { Owner = sender; }
        /// <summary>
        /// Gets the index of for the RibbonOptionDropDownCollection
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ControlItem this[int index] { get { return (ControlItem)List[index]; } }
        /// <summary>
        /// Returns whether the list contains the RibbonOptionDropDownCollection type
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public bool Contains(ControlItem itemType) { return List.Contains(itemType); }
        /// <summary>
        /// Adds the range type to the list
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int Add(ControlItem itemType)
        {
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            return List.Add(itemType);
        }
        /// <summary>
        ///Removes the type from the list
        /// </summary>
        /// <param name="itemType"></param>
        public void Remove(ControlItem itemType) { List.Remove(itemType); }
        /// <summary>
        /// Inserts teh ControlItem into the list
        /// </summary>
        /// <param name="index"></param>
        /// <param name="itemType"></param>
        public void Insert(int index, ControlItem itemType)
        {
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            List.Insert(index, itemType);
        }
        /// <summary>
        /// Returns the index of the ControlItem
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int IndexOf(ControlItem itemType) { return List.IndexOf(itemType); }
        /// <summary>
        /// searches the name in the list
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ControlItem FindByName(string name)
        {
            foreach (ControlItem ControlItemRange in List)
            {
                if (ControlItemRange.Name == name) return ControlItemRange;
            }
            return null;
        }
        /// <summary>
        /// Overrides the oninsert method
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnInsert(int index, object value)
        {
            if (string.IsNullOrEmpty(((ControlItem)value).Name)) ((ControlItem)value).Name = GetUniqueName();
            base.OnInsert(index, value);
        }
        /// <summary>
        /// Gets the unique name
        /// </summary>
        /// <returns></returns>
        private string GetUniqueName()
        {
            const string Prefix = "ControlItem";
            int index = 1;
            bool valid;
            while (this.Count != 0)
            {
                valid = true;
                for (int x = 0; x < this.Count; x++)
                {
                    if (this[x].Name == (Prefix + index.ToString()))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid) break;
                index++;
            };
            return Prefix + index.ToString();
        }
    }
    public class ControlItemDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.None;
            }
        }
    }
    [ToolboxItem(false),
Designer(typeof(ControlItemDesigner), typeof(IDesigner))]
    public class ControlItem : Control
    {
        public ControlItem()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.Size = new System.Drawing.Size(200, 60);
            this.BackColor = Color.White;
            this.MouseEnter += new EventHandler(UserControl1_MouseEnter);
            this.MouseLeave += new EventHandler(UserControl1_MouseLeave);
            headerText = "Show Tabs and Comments";
            subText = " Show Ribbon tabs and Commands all the time";
        }
        void UserControl1_MouseLeave(object sender, EventArgs e)
        {
            backColor = Color.White;
            this.Refresh();
        }

        void UserControl1_MouseEnter(object sender, EventArgs e)
        {
            backColor = ColorTranslator.FromHtml("#cde6f7");
            this.Refresh();
        }

        /// <summary>
        /// Text for Header
        /// </summary>
        private string headerText;
        /// <summary>
        /// Gets/Sets the value for HeaderText
        /// </summary>
        public string HeaderText
        {
            get
            {
                return headerText;
            }
            set
            {
                if (headerText != value)
                    headerText = value;
            }
        }
        /// <summary>
        /// Font for HeaderTextFont
        /// </summary>
        private Font headerTextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        /// <summary>
        /// Gets/Sets the value for HeaderTextFont
        /// </summary>
        public Font HeaderTextFont
        {
            get
            {
                return headerTextFont;
            }
            set
            {
                headerTextFont = value;
            }
        }
        /// <summary>
        /// SubTextFont
        /// </summary>
        private Font subTextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        /// <summary>
        /// Gets/Sets the value for SubText
        /// </summary>
        public Font SubTextFont
        {
            get
            {
                return subTextFont;
            }
            set
            {
                subTextFont = value;
            }
        }
        /// <summary>
        /// SubText
        /// </summary>
        private string subText;
        /// <summary>
        /// Gets/Sets the value for SubText
        /// </summary>
        public string SubText
        {
            get
            {
                return subText;
            }
            set
            {
                if (subText != value)
                    subText = value;
            }
        }
        /// <summary>
        /// itemImage
        /// </summary>
        private Image itemImage = null;
        /// <summary>
        /// Gets/Sets the value for itemImage
        /// </summary>
        public Image ItemImage
        {
            get
            {
                return itemImage;
            }
            set
            {
                itemImage = value;
            }
        }
        /// <summary>
        /// ArrowColor
        /// </summary>
        private Color arrowColor = ColorTranslator.FromHtml("#116EDA");
        /// <summary>
        /// Gets/Sets the value for ArrowColor
        /// </summary>
        public Color ArrowColor
        {
            get
            {
                return arrowColor;
            }
            set
            {
                arrowColor = Color.FromArgb(200, value);
                this.Refresh();
            }
        }
        /// <summary>
        /// ColorScheme
        /// </summary>
        private Office2013ColorScheme colorScheme = Office2013ColorScheme.White;
        /// <summary>
        /// Gets/Sets the value for ColorScheme
        /// </summary>
        public Office2013ColorScheme ColorScheme
        {
            get
            {
                return colorScheme;
            }
            set
            {
                colorScheme = value;
            }
        }
        /// <summary>
        /// StatusCheck
        /// </summary>
        private bool statusCheck = false;
        /// <summary>
        /// Gets/Sets the value for StatusCheck
        /// </summary>
        public bool StatusCheck
        {
            get
            {
                return statusCheck;
            }
            set
            {
                statusCheck = value;
            }
        }
        int SubTextYBound = 20;
        Color backColor = Color.White;
        Double hFontSize = 8.10;
        Double sFontSize = 8.10;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle HeaderRectangle = new Rectangle(55, 5, this.Width - 55, this.Height - 20);
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }
            if (StatusCheck)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(8, 8, 42, this.Height - 20));
                e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#e6f2fa")), new Rectangle(8, 8, 42, this.Height - 20)); // #cde6f7
            }
            e.Graphics.DrawImage(itemImage, new Rectangle(10, 10, 40, this.Height - 18));
            hFontSize = this.headerTextFont.Size;
            sFontSize = this.SubTextFont.Size;
            if (e.Graphics.DpiX > 96)
            {
                hFontSize = this.SubTextFont.Size / (e.Graphics.DpiX / 96);
                sFontSize = this.SubTextFont.Size / (float)(e.Graphics.DpiX / 96);
            }
            Font hFont = new Font(this.HeaderTextFont.OriginalFontName, (float)hFontSize, this.HeaderTextFont.Style);
            Font sFont = new Font(this.SubTextFont.OriginalFontName, (float)sFontSize, this.SubTextFont.Style);
            SubTextYBound = TextRenderer.MeasureText(HeaderText, hFont).Height + 10;
            Rectangle rect = new Rectangle(55, SubTextYBound, this.Width - 55, this.Height - (SubTextYBound));
            if (StatusCheck)
                e.Graphics.DrawRectangle(new Pen(Color.SkyBlue), new Rectangle(8, 8, 40, this.Height - 20));
            if (HeaderText != string.Empty)
                e.Graphics.DrawString(HeaderText, hFont, new SolidBrush(Color.Gray), HeaderRectangle ,format);
            if (SubText != String.Empty)
                e.Graphics.DrawString(SubText, sFont, new SolidBrush(Color.Gray), rect, format);
        }
    }
}
