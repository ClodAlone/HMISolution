#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Basic abstract class for rendering UpDownBase control.
    /// </summary>
    public abstract class UpDownRenderer : IUpDownRenderer
    {
        public UpDownRenderer(UpDownBase upDownControl)
        {
            this.m_upDownControl = upDownControl;
            this.Init();
        }

        #region Members
        private UpDownBase m_upDownControl;
        private Control m_upDownButtons;
        private TextBox m_upDownTextBox;
        private Rectangle m_upButtonRect;
        private Rectangle m_downButtonRect;
        #endregion

        #region Properties
      
        /// <summary>
        /// Gets the control that is rendering.
        /// </summary>
        protected UpDownBase UpDownControl
        {
            get
            {
                return m_upDownControl;
            }
        }
    
        /// <summary>
        /// Gets the spin orientation of rendering control.
        /// </summary>
        protected Orientation UpDownSpinOrientation
        {
            get
            {
                Orientation orientation = Orientation.Vertical;
                if (m_upDownControl is IUpDownButtonsOrientable)
                {
                    orientation = (m_upDownControl as IUpDownButtonsOrientable).SpinOrientation;
                }
                return orientation;
            }
        }
     
        /// <summary>
        /// Gets the up-down buttons of rendering control.
        /// </summary>
        protected Control UpDownButtons
        {
            get
            {
                if (m_upDownButtons != null)
                    return m_upDownButtons;

                foreach (Control control in this.UpDownControl.Controls)
                {
                    if (!(control is TextBox))
                    {
                        m_upDownButtons = control;
                    }
                }
                return m_upDownButtons;
            }
        }
     
        /// <summary>
        /// Gets the text box of rendering control.
        /// </summary>
        protected Control UpDownTextBox
        {
            get
            {
                if (m_upDownTextBox != null)
                    return m_upDownTextBox;

                foreach (Control control in this.UpDownControl.Controls)
                {
                    if (control is TextBox)
                    {
                        m_upDownTextBox = control as TextBox;
                    }
                }
                return m_upDownTextBox;
            }
        }

        /// <summary>
        /// Gets the bounds of up-button.
        /// </summary>
        protected Rectangle UpButtonRectangle
        {
            get
            {
                return m_upButtonRect;
            }
        }
  
        /// <summary>
        /// Gets the bounds of down-button.
        /// </summary>
        protected Rectangle DownButtonRectangle
        {
            get
            {
                return m_downButtonRect;
            }
        }
        #endregion

        #region Methods
        private void UpDownButtonsSizeChanged(object sender, EventArgs e)
        {
            Layout();
        }
        private void Init()
        {
            this.Layout();
            this.UpDownButtons.SizeChanged += new EventHandler(UpDownButtonsSizeChanged);
            IUpDownButtonsOrientable upDown = m_upDownControl as IUpDownButtonsOrientable;
            if (upDown != null)
            {
                upDown.SpinOrientationChanged += new EventHandler(UpDownSpinOrientationChanged);
            }
        }
        private void UpDownSpinOrientationChanged(object sender, EventArgs e)
        {
            this.Layout();
        }
        #endregion

        #region IUpDownRenderer Members
      
        /// <summary>
        /// Recalculates and sets button's rectangles.
        /// </summary>
        public void Layout()
        {
            int w = this.UpDownButtons.Width / 2;
            int h = this.UpDownButtons.Height / 2;
            if (this.UpDownSpinOrientation == Orientation.Vertical)
            {
                m_upButtonRect = new Rectangle(0, 0, this.UpDownButtons.Width, h);
                m_downButtonRect = new Rectangle(0, h, this.UpDownButtons.Width, h);
            }
            else
            {
                m_upButtonRect = new Rectangle(w, 0, w, this.UpDownButtons.Height);
                m_downButtonRect = new Rectangle(0, 0, w, this.UpDownButtons.Height);
            }
        }
   
        /// <summary>
        /// Draws arrow.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle where the arrow will be drawn.</param>
        /// <param name="buttonState">State of button where the arrow will be drawn.</param>
        /// <param name="upper">Indicates whether the arrow is upper.</param>
        public abstract void DrawArrow(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, bool upper);
      
        /// <summary>
        /// Draws button's border.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        public abstract void DrawScrollButtonBorder(Graphics g, Rectangle buttonRectangle, ButtonState buttonState);
     
        /// <summary>
        /// Draws button's background.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        public abstract void DrawScrollButtonBackground(Graphics g, Rectangle buttonRectangle, ButtonState buttonState);
       
        /// <summary>
        /// Draws button's background, border and arrow.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonRectangle">Rectangle of button.</param>
        /// <param name="buttonState">State of button.</param>
        /// <param name="buttonID">Indicates whether the arrow is up-button or down-button.</param>
        public virtual void DrawScrollButton(Graphics g, Rectangle buttonRectangle, ButtonState buttonState, ButtonID buttonID)
        {
            //this.DrawScrollButtonBorder(g, buttonRectangle, buttonState);
            SolidBrush brushcolor = new SolidBrush(Color.FromArgb(50 , Color.Gray));
            int h = buttonRectangle.Height / 2;
            Rectangle topR = new Rectangle(buttonRectangle.X, buttonRectangle.Y, buttonRectangle.Width, h);
            Rectangle botR = new Rectangle(buttonRectangle.X, buttonRectangle.Y + h, buttonRectangle.Width, buttonRectangle.Height - h);
            g.FillRectangle(brushcolor, topR);
            g.FillRectangle(brushcolor, botR);
            this.DrawScrollButtonBackground(g, buttonRectangle, buttonState);
            this.DrawArrow(g, buttonRectangle, buttonState, buttonID == ButtonID.Up);
            brushcolor.Dispose();
        }
     
        /// <summary>
        /// Draws up and down buttons.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="upButtonState">State of up-button.</param>
        /// <param name="downButtonState">State of down-button.</param>
        public virtual void Render(Graphics g, ButtonState upButtonState, ButtonState downButtonState)
        {
            this.DrawScrollButton(g, m_upButtonRect, upButtonState, ButtonID.Up);
            this.DrawScrollButton(g, m_downButtonRect, downButtonState, ButtonID.Down);
        }
        #endregion
    }
}
