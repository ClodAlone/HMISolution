#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using VS = System.Windows.Forms.VisualStyles;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Intializes the AdvancedToolTip for custom control.
    /// </summary>
    [CLSCompliant(true), ToolboxItem(false)]
    public partial class AdvancedToolTip : ToolStripDropDown
    {
        #region " Fields & Properties "

        private Control cont = null;

        /// <summary>
        /// Gets the content control of the pop-up.
        /// </summary>
        public Control Content
        {
            get
            {
                return cont;
            }
            private set
            {
                cont = value;
            }
        }

        private ToolTipAnimations showAnimation = ToolTipAnimations.SystemDefault; 
        /// <summary>
        /// Determines which animation to use while showing the pop-up window.
        /// </summary>
        public ToolTipAnimations ShowingAnimation
        {
            get
            {
                return showAnimation;
            }
            set
            {
                showAnimation = value;
            }
        }

        private ToolTipAnimations hideAnimation = ToolTipAnimations.SystemDefault;
        /// <summary>
        /// Determines which animation to use while hiding the pop-up window.
        /// </summary>
        public ToolTipAnimations HidingAnimation
        {
            get
            {
                return hideAnimation;
            }
            set
            {
                hideAnimation = value;
            }
        }

        private int animationDuration = 1000;
        /// <summary>
        /// Determines the duration of the animation.
        /// </summary>
        public int AnimationDuration 
        { 
            get
            {
                return animationDuration;
            }
            set
            {
                animationDuration = value;
            }
        }

        private bool focusOnOpen = false;
        /// <summary>
        /// Gets or sets a value indicating whether the content should receive the focus after the pop-up has been opened.
        /// </summary>
        /// <value><c>true</c> if the content should be focused after the pop-up has been opened; otherwise, <c>false</c>.</value>
        /// <remarks>If the FocusOnOpen property is set to <c>false</c>, then pop-up cannot use the fade effect.</remarks>
        public bool FocusOnOpen
        {
            get
            {
                return focusOnOpen;
            }
            set
            {
                focusOnOpen = value;
            }
        }

        private bool acceptAlt = false;
        /// <summary>
        /// Gets or sets a value indicating whether pressing the alt key should close the pop-up.
        /// </summary>
        /// <value><c>true</c> if pressing the alt key does not close the pop-up; otherwise, <c>false</c>.</value>
        public bool AcceptAlt 
        { 
            get
            {
                return acceptAlt;
            } 
            set
            {
                acceptAlt = value;
            }
        }

        private ArrowHeadDirection ArrowDir = ArrowHeadDirection.Left;

        /// <summary>
        /// Determines arrowheaddirection while showing the pop-up window.
        /// </summary>
        public ArrowHeadDirection ArrowHeadDirection
        {
            get
            {
                return ArrowDir;
            }
            set
            {
                ArrowDir = value;
                OutControl.Region = new Region(graphicsPath = CreateRoundRectangle(OutControl.Width - 1, OutControl.Height - 1, 6));
            }
        }

        private ToolStripControlHost _host;
        private Control _opener;
        private AdvancedToolTip _ownerPopup;
        private AdvancedToolTip _childPopup;
        private bool _resizableTop;
        private bool _resizableLeft;

        private bool _isChildPopupOpened;
        private bool _resizable;
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PopupControl.Popup" /> is resizable.
        /// </summary>
        /// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
        public bool Resizable
        {
            get { return _resizable && !_isChildPopupOpened; }
            set { _resizable = value; }
        }

        private bool _nonInteractive;
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PopupControl.Popup"></see> acts like a transparent windows (so it cannot be clicked).
        /// </summary>
        /// <value>
        /// <c>true</c> if the popup is noninteractive; otherwise, <c>false</c>.</value>
        public bool NonInteractive
        {
            get { return _nonInteractive; }
            set
            {
                if (value != _nonInteractive)
                {
                    _nonInteractive = value;
                    if (IsHandleCreated) RecreateHandle();
                }
            }
        }

        private Size minimumSize;
        /// <summary>
        /// Gets or sets a minimum size of the pop-up.
        /// </summary>
        /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size" /> representing the width and height of a rectangle.</returns>
        public new Size MinimumSize
        {
            get
            {
                return minimumSize;
            }
            set
            {
                minimumSize =value;
            }
        }

        private Size maximumSize;
        /// <summary>
        /// Gets or sets a maximum size of the pop-up.
        /// </summary>
        /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size" /> representing the width and height of a rectangle.</returns>
        public new Size MaximumSize
        {
            get
            {
                return maximumSize;
            }
            set
            {
                maximumSize = value;
            }
        }

        /// <summary>
        /// Gets parameters of a new window.
        /// </summary>
        /// <returns>An object of type <see cref="T:System.Windows.Forms.CreateParams" /> used when creating a new window.</returns>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
                if (NonInteractive) cp.ExStyle |= NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        #endregion

        private Control dispControl = new Control();

        /// <summary>
        /// Gets or Sets the system control in which the desired usercontrol ToolTip needed to be displayed.
        /// </summary>
        public Control DisplayControl
        {
            get
            {
                return dispControl;
            }
            set
            {
                dispControl = value;
            }
        }
        #region " Constructors "

        private Control InrControl = new Control(), OutControl = new Control();

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupControl.Popup"/> class.
        /// </summary>
        /// <param name="innerControl">The content of the pop-up.</param>
        /// <remarks>
        /// Pop-up will be disposed immediately after disposion of the content control.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="innerControl" /> is <code>null</code>.</exception>
        public AdvancedToolTip(Control innerControl, Control displayControl)
        {
            if (innerControl == null)
            {
                throw new ArgumentNullException("content");
            }
            OutControl.ClientSize = new Size(innerControl.Width + 25, innerControl.Height + 25);
            OutControl.Controls.Add(innerControl);
            OutControl.BackColor = Color.White;
            innerControl.Location = new Point(innerControl.Location.X + 11, innerControl.Location.Y + 11);
            Content = OutControl;
            DisplayControl = displayControl;

            FocusOnOpen = false;
            AcceptAlt = true;
            ShowingAnimation = ToolTipAnimations.SystemDefault;
            HidingAnimation = ToolTipAnimations.None;
            AnimationDuration = 100;
            InitializeComponent();
            AutoSize = false;
            DoubleBuffered = true;
            ResizeRedraw = true;
            _host = new ToolStripControlHost(OutControl);
            Padding = Margin = _host.Padding = _host.Margin = Padding.Empty;
            if (NativeMethods.IsRunningOnMono) OutControl.Margin = Padding.Empty;
            MinimumSize = OutControl.MinimumSize;
            OutControl.MinimumSize = OutControl.Size;
            MaximumSize = OutControl.MaximumSize;
            OutControl.MaximumSize = OutControl.Size;
            Size = OutControl.Size;
            if (NativeMethods.IsRunningOnMono) _host.Size = OutControl.Size;
            TabStop = OutControl.TabStop = true;
            OutControl.Location = Point.Empty;
            Items.Add(_host);
            OutControl.Disposed += new EventHandler(OutControl_Disposed);
            OutControl.RegionChanged += new EventHandler(OutControl_RegionChanged);
            OutControl.Paint += new PaintEventHandler(OutControl_Paint);
            OutControl.Region = new Region(graphicsPath = CreateRoundRectangle(OutControl.Width - 3, OutControl.Height - 1, 6));
            UpdateRegion();

            displayControl.MouseEnter += new EventHandler(displayControl_MouseEnter);
            displayControl.MouseHover += new EventHandler(displayControl_MouseHover);
            displayControl.MouseMove += new MouseEventHandler(displayControl_MouseMove);
            displayControl.MouseLeave += new EventHandler(displayControl_MouseLeave);

            OutControl.MouseMove += new MouseEventHandler(OutControl_MouseMove);
            OutControl.MouseLeave += new EventHandler(OutControl_MouseLeave);
            OutControl.GotFocus += new EventHandler(OutControl_GotFocus);
        }

        private Color borderColor = ColorTranslator.FromHtml("#076CC8");
        /// <summary>
        /// Gets or Sets the bordercolor of the tooltip window.
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }

        void OutControl_Disposed(object sender, EventArgs e)
        {
            OutControl = null;
            Dispose(true);
        }

        void OutControl_RegionChanged(object sender, EventArgs e)
        {
            UpdateRegion();
        }

        private Timer tm = new Timer();
        private int hitCount = 0;
        private bool timeElapsed = true;
        private Point pte = new Point();
        private Point pt = new Point();
                
        /// <summary>
        /// ToolTip displaycontrol mouseLeave method being accessed to hook the timer operation.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">event data.</param>
        void displayControl_MouseLeave(object sender, EventArgs e)
        {
            tm.Tick += new EventHandler(tm_Tick);
            tm.Interval = 1000;
            tm.Enabled = true;
        }

        /// <summary>
        /// ToolTip OuterControl Gotfocus being accessed to hook the timer operation.
        /// </summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">event data.</param>
        void OutControl_GotFocus(object sender, EventArgs e)
        {
            if (tm.Enabled == true)
            {
                tm.Enabled = false;
                tm.Enabled = true;
            }
            tm.Tick += new EventHandler(tm_Tick);
            tm.Interval = 2000;
            tm.Enabled = true;
        }
                
        /// <summary>
        /// ToolTip Outercontrol mouseleave to ensure the display duration of the ToolTip when mouse cursor exits focus.
        /// </summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">event data.</param>
        void OutControl_MouseLeave(object sender, EventArgs e)
        {
            hitCount++;
            if (hitCount == 2)
            {
                timeElapsed = true;
                hitCount = 0;
            }
        }
        
        /// <summary>
        /// Tick event hooked to disable the ToolTip after a certain duration.
        /// </summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">event data.</param>
        void tm_Tick(object sender, EventArgs e)
        {
            if (this != null && timeElapsed)
            {
                this.Hide();
                timeElapsed = true;
            }
        }

        
        /// <summary>
        /// To ensure the ToolTip availablity when the cursor is over the ToolTip.
        /// </summary>
        /// </summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">event data.</param>
        void OutControl_MouseMove(object sender, MouseEventArgs e)
        {
            timeElapsed = false;
        }

        
        /// <summary>
        /// To ensure the ToolTip availablity when the cursor is over the displaycontrol.
        /// </summary>
        /// </summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">event data.</param>
        void displayControl_MouseMove(object sender, MouseEventArgs e)
        {
            tm.Enabled = false;
        }
                
        /// <summary>
        /// To ensure the ToolTip availablity when the cursor is over the displaycontrol.
        /// </summary>
        /// </summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">event data.</param>
        void displayControl_MouseEnter(object sender, EventArgs e)
        {
            tm.Enabled = false;
            pte = new Point(Cursor.Position.X, Cursor.Position.Y);
        }

        
        /// <summary>
        /// To ensure the ToolTip availablity when the cursor is over the displaycontrol.
        /// </summary>
        /// </summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">event data.</param>
        void displayControl_MouseHover(object sender, EventArgs e)
        {
            Point postion = this.DisplayControl.PointToScreen(new Point(this.DisplayControl.Location.X + this.DisplayControl.Width, this.DisplayControl.Location.Y + this.DisplayControl.Height));
            if (this.DisplayControl.Parent != null)
                postion = this.DisplayControl.Parent.PointToScreen(new Point(this.DisplayControl.Location.X + this.DisplayControl.Width, this.DisplayControl.Location.Y + this.DisplayControl.Height / 2));
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            Rectangle BtRtRect = new Rectangle(postion.X, postion.Y, screenBounds.Width - postion.X, screenBounds.Height - postion.Y);
            Rectangle TpRtRect = new Rectangle(postion.X, 0, screenBounds.Width - postion.X, postion.Y);
            Rectangle BtLtRect = new Rectangle(0, postion.Y, postion.X, screenBounds.Height - postion.Y);
            Rectangle TpLtRect = new Rectangle(0, 0, postion.X, postion.Y);

            if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Automatic)
            {
                if (BtRtRect.Width > this.Width && BtRtRect.Height > this.Height / 2 + this.DisplayControl.Height / 2 || ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Left)
                {
                    AutomaticType = "Left";
                    this.Show(new Point(postion.X, postion.Y - this.Height / 2));
                }
                else if (BtLtRect.Width > postion.X - this.DisplayControl.Width && BtLtRect.Height > this.Height || ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Right)
                {
                    AutomaticType = "Right";
                    this.Show(new Point(postion.X - this.DisplayControl.Width - this.Width, postion.Y - this.Height / 2));
                }
                else if (TpLtRect.Width - DisplayControl.Width < this.Width / 2 && TpLtRect.Height < this.Height || ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Top)
                {
                    AutomaticType = "Top";
                    this.Show(new Point(postion.X - this.DisplayControl.Width / 2 - this.Width / 2, postion.Y));
                }
                else if (TpRtRect.Width + DisplayControl.Width / 2 > this.Width / 2 && TpRtRect.Height > this.Height || ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Bottom)
                {
                    AutomaticType = "Bottom";
                    this.Show(new Point(postion.X - this.DisplayControl.Width / 2 - this.Width / 2, postion.Y - this.DisplayControl.Height - this.Height));
                    AutomaticType = "Bottom";
                }
                else
                {
                    this.Show(new Point(postion.X, postion.Y));
                }
            }
            else
            {
                if ( ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Left)
                {
                    AutomaticType = "Left";
                    this.Show(new Point(postion.X, postion.Y - this.Height / 2));
                }
                else if ( ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Right)
                {
                    AutomaticType = "Right";
                    this.Show(new Point(postion.X - this.DisplayControl.Width - this.Width, postion.Y - this.Height / 2));
                }
                else if ( ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Top)
                {
                    AutomaticType = "Top";
                    this.Show(new Point(postion.X - this.DisplayControl.Width / 2 - this.Width / 2, postion.Y+ 13));
                }
                else if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Bottom)
                {
                    AutomaticType = "Bottom";
                    this.Show(new Point(postion.X - this.DisplayControl.Width / 2 - this.Width / 2, postion.Y - this.DisplayControl.Height - this.Height));
                    AutomaticType = "Bottom";
                }
                else
                {
                    this.Show(new Point(postion.X, postion.Y));
                }
            }
        }

        private string automaticType = "Left";

        /// <summary>
        /// This AutomaticType string is used to get / set the ArrowHead for the AutomaticType Enumeration. And to update the exact ToolTip location.
        /// </summary>
        private string AutomaticType
        {
            get
            {
                return automaticType;
            }
            set
            {
                automaticType = value;
                OutControl.Region = new Region(graphicsPath = CreateRoundRectangle(OutControl.Width - 1, OutControl.Height - 1, 6));
            }
        }

        private bool automaticEnabled = false;

        
        /// <summary>
        /// To ensure the ToolTip arrowhead location over the passed usercontrol for different enums.
        /// </summary>
        /// </summary>
        /// <param name="sender">sender of the event.</param>
        /// <param name="e">event data.</param>
        void OutControl_Paint(object sender, PaintEventArgs e)
        {
            // <summary>
            // Paints the sizing grip.
            //if (e == null || e.Graphics == null || !_resizable)
            //{
            //    return;
            //}
            //Size clientSize = Content.ClientSize;
            //using (Bitmap gripImage = new Bitmap(0x10, 0x10))
            //{
            //    using (Graphics g = Graphics.FromImage(gripImage))
            //    {
            //        if (Application.RenderWithVisualStyles)
            //        {
            //            if (_sizeGripRenderer == null)
            //            {
            //                _sizeGripRenderer = new VS.VisualStyleRenderer(VS.VisualStyleElement.Status.Gripper.Normal);
            //            }
            //            _sizeGripRenderer.DrawBackground(g, new Rectangle(0, 0, 0x10, 0x10));
            //        }
            //        else
            //        {
            //            ControlPaint.DrawSizeGrip(g, Content.BackColor, 0, 0, 0x10, 0x10);
            //        }
            //    }
            //    GraphicsState gs = e.Graphics.Save();
            //    e.Graphics.ResetTransform();
            //    if (_resizableTop)
            //    {
            //        if (_resizableLeft)
            //        {
            //            e.Graphics.RotateTransform(180);
            //            e.Graphics.TranslateTransform(-clientSize.Width, -clientSize.Height);
            //        }
            //        else
            //        {
            //            e.Graphics.ScaleTransform(1, -1);
            //            e.Graphics.TranslateTransform(0, -clientSize.Height);
            //        }
            //    }
            //    else if (_resizableLeft)
            //    {
            //        e.Graphics.ScaleTransform(-1, 1);
            //        e.Graphics.TranslateTransform(-clientSize.Width, 0);
            //    }
            //    e.Graphics.DrawImage(gripImage, clientSize.Width - 0x10, clientSize.Height - 0x10 + 1, 0x10, 0x10);
            //    e.Graphics.Restore(gs);
            //}
            // </summary>
            // <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs" /> instance containing the event data.</param>

            int w = OutControl.Width - 1, h = OutControl.Height - 1, r = 6;
            int d = r << 1;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Automatic)
            {
                pt = DisplayControl.PointToScreen(new Point(Cursor.Position.X, Cursor.Position.Y));
                automaticEnabled = true;
            }
            else
            {
                automaticEnabled = false;
            }
            if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Bottom || (automaticEnabled && AutomaticType == "Bottom"))
            {
                int iWd = 2;
                path.AddLine(0 + iWd, 0 + iWd, 0 + iWd, h - 10 - iWd);
                path.AddLine(w / 2 - 10 - iWd, h - 10 - iWd, w / 2 - iWd, h - iWd);
                path.AddLine(w / 2 + 10 - iWd, h - 10 - iWd, w - iWd, h - 10 - iWd);
                path.AddLine(w - iWd, 0 + iWd, 0 + iWd, 0 + iWd);
                e.Graphics.DrawLine(new Pen(BorderColor, 1f), 0 + iWd, h - 10 - iWd, w / 2 - 10, h - 10 - iWd);
                e.Graphics.DrawLine(new Pen(BorderColor, 1f), w / 2 + 9, h - 10 - iWd, w - iWd, h - 10 - iWd);
                e.Graphics.DrawLine(new Pen(BorderColor, 1f), w - iWd, h - 10 - iWd, w - iWd, 0 + iWd);
            }
            else if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Left || (automaticEnabled && AutomaticType == "Left"))
            {
                path.AddLine(10, 0, 10, h / 2 - 10);
                path.AddLine(0, h / 2, 10, h / 2 + 10);
                path.AddLine(10, h, w, h);
                path.AddLine(w, 0, 10, 0);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), 10, h, w, h);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), w, h, w, 0);
            }
            else if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Right || (automaticEnabled && AutomaticType == "Right"))
            {
                path.AddLine(0, 0, 0, h);
                path.AddLine(w - 10, h, w - 10, h / 2 + 10);
                path.AddLine(w, h / 2, w - 10, h / 2 - 10);
                path.AddLine(w - 10, 0, 0, 0);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), 0, h, w - 10, h);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), w - 10, h, w - 10, h / 2 + 10);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), w - 10, h / 2 + 10, w, h / 2);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), w, h / 2, w - 10, h / 2 - 10);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), w - 10, h / 2 - 10, w - 10, 0);
            }
            else if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Top || (automaticEnabled && AutomaticType == "Top"))
            {
                path.AddLine(0, 10, 0, h);
                path.AddLine(w, h, w, 10);
                path.AddLine(w / 2 + 10, 10, w / 2, 0);
                path.AddLine(w / 2 - 10, 10, 0, 10);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), 0, h, w, h);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), w, h, w, 10);
                e.Graphics.DrawLine(new Pen(BorderColor, 3f), w / 2 + 10, 10, w / 2, 0);
            }
            path.CloseFigure();
            e.Graphics.DrawPath(new Pen(BorderColor, 1f), path);
        }
        GraphicsPath graphicsPath = new GraphicsPath();

        //To draw the ToolTip arrowhead over the passed usercontrol.
        private GraphicsPath CreateRoundRectangle(int w, int h, int r)
        {
            int d = r << 1;
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Automatic)
            {
                pt = DisplayControl.PointToScreen(new Point(Cursor.Position.X, Cursor.Position.Y));
                automaticEnabled = true;
            }
            else
            {
                automaticEnabled = false;
            }
            if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Bottom || (automaticEnabled && AutomaticType == "Bottom"))
            {
                path.AddLine(0, 0, 0, h - 10);
                path.AddLine(w / 2 - 10, h - 10, w / 2, h);
                path.AddLine(w / 2 + 10, h - 10, w, h - 10);
                path.AddLine(w, 0, 0, 0);
            }
            else if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Left || (automaticEnabled && AutomaticType == "Left"))
            {
                path.AddLine(10, 0, 10, h / 2 - 10);
                path.AddLine(0, h / 2, 10, h / 2 + 10);
                path.AddLine(10, h, w, h);
                path.AddLine(w, 0, 10, 0);
            }
            else if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Right || (automaticEnabled && AutomaticType == "Right"))
            {
                path.AddLine(0, 0, 0, h);
                path.AddLine(w - 10, h, w - 10, h / 2 + 10);
                path.AddLine(w, h / 2, w - 10, h / 2 - 10);
                path.AddLine(w - 10, 0, 0, 0);
            }
            else if (ArrowHeadDirection == GridHelperClasses.ArrowHeadDirection.Top || (automaticEnabled && AutomaticType == "Top"))
            {
                path.AddLine(0, 10, 0, h);
                path.AddLine(w, h, w, 10);
                path.AddLine(w / 2 + 10, 10, w / 2, 0);
                path.AddLine(w / 2 - 10, 10, 0, 10);
            }
            path.CloseFigure();
            return path;
        }

        #endregion

        #region " Methods "

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripItem.VisibleChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (NativeMethods.IsRunningOnMono) return; // in case of non-Windows
            if ((Visible && ShowingAnimation == ToolTipAnimations.None) || (!Visible && HidingAnimation == ToolTipAnimations.None))
            {
                return;
            }
            NativeMethods.AnimationFlags flags = Visible ? NativeMethods.AnimationFlags.Roll : NativeMethods.AnimationFlags.Hide;
            ToolTipAnimations _flags = Visible ? ShowingAnimation : HidingAnimation;
            if (_flags == ToolTipAnimations.SystemDefault)
            {
                if (SystemInformation.IsMenuAnimationEnabled)
                {
                    if (SystemInformation.IsMenuFadeEnabled)
                    {
                        _flags = ToolTipAnimations.Blend;
                    }
                    else
                    {
                        _flags = ToolTipAnimations.Slide | (Visible ? ToolTipAnimations.TopToBottom : ToolTipAnimations.BottomToTop);
                    }
                }
                else
                {
                    _flags = ToolTipAnimations.None;
                }
            }
            if ((_flags & (ToolTipAnimations.Blend | ToolTipAnimations.Center | ToolTipAnimations.Roll | ToolTipAnimations.Slide)) == ToolTipAnimations.None)
            {
                return;
            }
            if (_resizableTop) // popup is “inverted”, so the animation must be
            {
                if ((_flags & ToolTipAnimations.BottomToTop) != ToolTipAnimations.None)
                {
                    _flags = (_flags & ~ToolTipAnimations.BottomToTop) | ToolTipAnimations.TopToBottom;
                }
                else if ((_flags & ToolTipAnimations.TopToBottom) != ToolTipAnimations.None)
                {
                    _flags = (_flags & ~ToolTipAnimations.TopToBottom) | ToolTipAnimations.BottomToTop;
                }
            }
            if (_resizableLeft) // popup is “inverted”, so the animation must be
            {
                if ((_flags & ToolTipAnimations.RightToLeft) != ToolTipAnimations.None)
                {
                    _flags = (_flags & ~ToolTipAnimations.RightToLeft) | ToolTipAnimations.LeftToRight;
                }
                else if ((_flags & ToolTipAnimations.LeftToRight) != ToolTipAnimations.None)
                {
                    _flags = (_flags & ~ToolTipAnimations.LeftToRight) | ToolTipAnimations.RightToLeft;
                }
            }
            flags = flags | (NativeMethods.AnimationFlags.Mask & (NativeMethods.AnimationFlags)(int)_flags);
            NativeMethods.SetTopMost(this);
            NativeMethods.AnimateWindow(this, AnimationDuration, flags);
        }

        /// <summary>
        /// Processes a dialog box key.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values that represents the key to process.</param>
        /// <returns>
        /// true if the key was processed by the control; otherwise, false.
        /// </returns>
        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (AcceptAlt && ((keyData & Keys.Alt) == Keys.Alt))
            {
                if ((keyData & Keys.F4) != Keys.F4)
                {
                    return false;
                }
                else
                {
                    Close();
                }
            }
            bool processed = base.ProcessDialogKey(keyData);
            if (!processed && (keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)))
            {
                bool backward = (keyData & Keys.Shift) == Keys.Shift;
                Content.SelectNextControl(null, !backward, true, true, true);
            }
            return processed;
        }

        /// <summary>
        /// Updates the pop-up region.
        /// </summary>
        protected void UpdateRegion()
        {
            if (Region != null)
            {
                Region.Dispose();
                Region = null;
            }
            if (Content.Region != null)
            {
                Region = Content.Region.Clone();
            }
        }

        /// <summary>
        /// Shows the pop-up window below the specified control.
        /// </summary>
        /// <param name="control">The control below which the pop-up will be shown.</param>
        /// <remarks>
        /// When there is no space below the specified control, the pop-up control is shown above it.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="control"/> is <code>null</code>.</exception>
        public void Show(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            Show(control, control.ClientRectangle);
        }

        /// <summary>
        /// Shows the pop-up window below the specified area.
        /// </summary>
        /// <param name="area">The area of desktop below which the pop-up will be shown.</param>
        /// <remarks>
        /// When there is no space below specified area, the pop-up control is shown above it.
        /// </remarks>
        public void Show(Rectangle area)
        {
            _resizableTop = _resizableLeft = false;
            Point location = new Point(area.Left, area.Top + area.Height);
            Rectangle screen = Screen.FromControl(this).WorkingArea;
            if (location.X + Size.Width > (screen.Left + screen.Width))
            {
                _resizableLeft = true;
                location.X = (screen.Left + screen.Width) - Size.Width;
            }
            if (location.Y + Size.Height > (screen.Top + screen.Height))
            {
                _resizableTop = true;
                location.Y -= Size.Height + area.Height;
            }
            //location = control.PointToClient(location);
            Show(location, ToolStripDropDownDirection.BelowRight);
        }

        /// <summary>
        /// Shows the pop-up window below the specified area of the specified control.
        /// </summary>
        /// <param name="control">The control used to compute screen location of specified area.</param>
        /// <param name="area">The area of control below which the pop-up will be shown.</param>
        /// <remarks>
        /// When there is no space below specified area, the pop-up control is shown above it.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="control"/> is <code>null</code>.</exception>
        public void Show(Control control, Rectangle area)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            SetOwnerItem(control);

            _resizableTop = _resizableLeft = false;
            Point location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
            Rectangle screen = Screen.FromControl(control).WorkingArea;
            if (location.X + Size.Width > (screen.Left + screen.Width))
            {
                _resizableLeft = true;
                location.X = (screen.Left + screen.Width) - Size.Width;
            }
            if (location.Y + Size.Height > (screen.Top + screen.Height))
            {
                _resizableTop = true;
                location.Y -= Size.Height + area.Height;
            }
            location = control.PointToClient(location);
            Show(control, location, ToolStripDropDownDirection.BelowRight);
        }

        private void SetOwnerItem(Control control)
        {
            if (control == null)
            {
                return;
            }
            if (control is AdvancedToolTip)
            {
                AdvancedToolTip popupControl = control as AdvancedToolTip;
                _ownerPopup = popupControl;
                _ownerPopup._childPopup = this;
                OwnerItem = popupControl.Items[0];
                return;
            }
            else if (_opener == null)
            {
                _opener = control;
            }
            if (control.Parent != null)
            {
                SetOwnerItem(control.Parent);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (Content != null)
            {
                Content.MinimumSize = Size;
                Content.MaximumSize = Size;
                Content.Size = Size;
                Content.Location = Point.Empty;
            }
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Layout" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.LayoutEventArgs" /> that contains the event data.</param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            if (!NativeMethods.IsRunningOnMono)
            {
                base.OnLayout(e);
                return;
            }
            Size suggestedSize = GetPreferredSize(Size.Empty);
            if (AutoSize && suggestedSize != Size)
            {
                Size = suggestedSize;
            }
            SetDisplayedItems();
            OnLayoutCompleted(EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Opening" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
        protected override void OnOpening(CancelEventArgs e)
        {
            if (Content.IsDisposed || Content.Disposing)
            {
                e.Cancel = true;
                return;
            }
            UpdateRegion();
            base.OnOpening(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Opened" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnOpened(EventArgs e)
        {
            if (_ownerPopup != null)
            {
                _ownerPopup._isChildPopupOpened = true;
            }
            if (FocusOnOpen)
            {
                Content.Focus();
            }
            base.OnOpened(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Closed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripDropDownClosedEventArgs"/> that contains the event data.</param>
        protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
        {
            _opener = null;
            if (_ownerPopup != null)
            {
                _ownerPopup._isChildPopupOpened = false;
            }
            base.OnClosed(e);
        }

        #endregion

        #region " Resizing Support "

        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            //if (m.Msg == NativeMethods.WM_PRINT && !Visible)
            //{
            //    Visible = true;
            //}
            if (InternalProcessResizing(ref m, false))
            {
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Processes the resizing messages.
        /// </summary>
        /// <param name="m">The message.</param>
        /// <returns>true, if the WndProc method from the base class shouldn't be invoked.</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool ProcessResizing(ref Message m)
        {
            return InternalProcessResizing(ref m, true);
        }

        /// <summary>
        /// used internally.
        /// </summary>
        /// <param name="m">message.</param>
        /// <param name="contentControl">boolean value</param>
        /// <returns>boolean value</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool InternalProcessResizing(ref Message m, bool contentControl)
        {
            if (m.Msg == NativeMethods.WM_NCACTIVATE && m.WParam != IntPtr.Zero && _childPopup != null && _childPopup.Visible)
            {
                _childPopup.Hide();
            }
            if (!Resizable && !NonInteractive)
            {
                return false;
            }
            if (m.Msg == NativeMethods.WM_NCHITTEST)
            {
                return OnNcHitTest(ref m, contentControl);
            }
            else if (m.Msg == NativeMethods.WM_GETMINMAXINFO)
            {
                return OnGetMinMaxInfo(ref m);
            }
            return false;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="m">message.</param>
        /// <returns>boolean value</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool OnGetMinMaxInfo(ref Message m)
        {
            NativeMethods.MINMAXINFO minmax = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.MINMAXINFO));
            if (!MaximumSize.IsEmpty)
            {
                minmax.maxTrackSize = MaximumSize;
            }
            minmax.minTrackSize = MinimumSize;
            Marshal.StructureToPtr(minmax, m.LParam, false);
            return true;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="m">message.</param>
        /// <param name="contentControl">boolean value.</param>
        /// <returns>boolean value.</returns>
        private bool OnNcHitTest(ref Message m, bool contentControl)
        {
            if (NonInteractive)
            {
                m.Result = (IntPtr)NativeMethods.HTTRANSPARENT;
                return true;
            }

            int x = Cursor.Position.X; // NativeMethods.LOWORD(m.LParam);
            int y = Cursor.Position.Y; // NativeMethods.HIWORD(m.LParam);
            Point clientLocation = PointToClient(new Point(x, y));

            GripBounds gripBouns = new GripBounds(contentControl ? Content.ClientRectangle : ClientRectangle);
            IntPtr transparent = new IntPtr(NativeMethods.HTTRANSPARENT);

            if (_resizableTop)
            {
                if (_resizableLeft && gripBouns.TopLeft.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPLEFT;
                    return true;
                }
                if (!_resizableLeft && gripBouns.TopRight.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPRIGHT;
                    return true;
                }
                if (gripBouns.Top.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOP;
                    return true;
                }
            }
            else
            {
                if (_resizableLeft && gripBouns.BottomLeft.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMLEFT;
                    return true;
                }
                if (!_resizableLeft && gripBouns.BottomRight.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMRIGHT;
                    return true;
                }
                if (gripBouns.Bottom.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOM;
                    return true;
                }
            }
            if (_resizableLeft && gripBouns.Left.Contains(clientLocation))
            {
                m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTLEFT;
                return true;
            }
            if (!_resizableLeft && gripBouns.Right.Contains(clientLocation))
            {
                m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTRIGHT;
                return true;
            }
            return false;
        }

      
        #endregion
    }
}