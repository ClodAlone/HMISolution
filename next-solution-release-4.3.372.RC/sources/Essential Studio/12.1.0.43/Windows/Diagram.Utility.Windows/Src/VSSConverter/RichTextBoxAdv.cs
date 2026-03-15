#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.InteropServices.WinAPI;

#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// RichTextBoxAdv class.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    [
    ToolboxItem(false)
    ]
    public class RichTextBoxAdv : RichTextBox
    {
        #region Class constants
        /// <summary>
        /// Underline Colors
        /// </summary>
        public enum UnderlineColor
        {
            /// <summary>
            /// Default underline color
            /// </summary>
            Default = 0x00,

            /// <summary>
            /// Blue color
            /// </summary>
            Blue = 0x10,

            /// <summary>
            /// Aqua color
            /// </summary>
            Aqua = 0x20,

            /// <summary>
            /// Lime color.
            /// </summary>
            Lime = 0x30,

            /// <summary>
            /// Fuchsia color.
            /// </summary>
            Fuchsia = 0x40,

            /// <summary>
            /// Red color.
            /// </summary>
            Red = 0x50,

            /// <summary>
            /// Yellow color.
            /// </summary>
            Yellow = 0x60,

            /// <summary>
            /// White color.
            /// </summary>
            White = 0x70,

            /// <summary>
            /// Navy color.
            /// </summary>
            Navy = 0x80,

            /// <summary>
            /// Teal color.
            /// </summary>
            Teal = 0x90,

            /// <summary>
            /// Green color.
            /// </summary>
            Green = 0xa0,

            /// <summary>
            /// Purple color.
            /// </summary>
            Purple = 0xb0,

            /// <summary>
            /// Maroon color.
            /// </summary>
            Maroon = 0xc0,

            /// <summary>
            /// Olive color.
            /// </summary>
            Olive = 0xd0,

            /// <summary>
            /// Gray color.
            /// </summary>
            Gray = 0xe0,

            /// <summary>
            /// Silver color.
            /// </summary>
            Silver = 0xf0
        }

        /// <summary>
        /// Underline styles.
        /// </summary>
        public enum UnderlineStyle
        {
            /// <summary>
            /// No underline.
            /// </summary>
            None = 0,

            /// <summary>
            /// Single underline.
            /// </summary>
            Underline = 1,

            /// <summary>
            /// Double underline.
            /// </summary>
            UnderlineDouble = 3,

            /// <summary>
            /// Dotted underline.
            /// </summary>
            UnderlineDotted = 4,

            /// <summary>
            /// Dash underline.
            /// </summary>
            UnderlineDash = 5,

            /// <summary>
            /// Dash dot underline.
            /// </summary>
            UnderlineDashDot = 6,

            /// <summary>
            /// Dash dot dot underline.
            /// </summary>
            UnderlineDashDotDot = 7,

            /// <summary>
            /// Wave underline.
            /// </summary>
            UnderlineWave = 8,

            /// <summary>
            /// Thick underline.
            /// </summary>
            UnderlineThick = 9,

            /// <summary>
            /// Hairline underline.
            /// </summary>
            UnderlineHairline = 10,

            /// <summary>
            /// Double wave underline.
            /// </summary>
            UnderlineDoubleWave = 11,

            /// <summary>
            /// Heavy wave underline.
            /// </summary>
            UnderlineHeavyWave = 12,

            /// <summary>
            /// Long dash underline.
            /// </summary>
            UnderlineLongDash = 13,

            /// <summary>
            /// Thick dash underline.
            /// </summary>
            UnderlineThickDash = 14,

            /// <summary>
            /// Thich dash dot underline.
            /// </summary>
            UnderlineThickDashDot = 15,

            /// <summary>
            /// Thich dash dot dot underline.
            /// </summary>
            UnderlineThickDashDotDot = 16,

            /// <summary>
            /// Thick dotted underline.
            /// </summary>
            UnderlineThickDotted = 17,

            /// <summary>
            /// Thick long dash underline.
            /// </summary>
            UnderlineThickLongDash = 18,
        }

        /// <summary>
        /// Specifies how text is horizontally aligned.
        /// </summary>
        public enum TextAlign
        {
            /// <summary>
            /// The text is aligned to the left.
            /// </summary>
            Left = 1,

            /// <summary>
            /// The text is aligned to the right.
            /// </summary>
            Right = 2,

            /// <summary>
            /// The text is aligned in the center.
            /// </summary>
            Center = 3,

            /// <summary>
            /// The text is justified.
            /// </summary>
            Justify = 4
        }
        #endregion

        #region Class members
        /// <summary>
        /// Redrawing locking calls counter. Allow to ignore unneeded control (un)locking.
        /// </summary>
        private int m_iLockCounter;

        /// <summary>
        /// Special mask of Richedit control. It stored by BeginUpdate method and required
        /// for proper unlocking. User must not change it.
        /// </summary>
        private int m_iEventMask;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the selections background color
        /// </summary>
        [Browsable(false),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color SelectionBackColor
        {
            get
            {
                RichTextNativeMethods.CHARFORMAT2 cf = this.GetCharFormat();
                return ColorTranslator.FromOle(cf.crBackColor);
            }
            set
            {
                RichTextNativeMethods.CHARFORMAT2 cf = new RichTextNativeMethods.CHARFORMAT2();
                cf.cbSize = Marshal.SizeOf(cf);
                cf.dwMask = (uint)RichTextNativeMethods.RichEditFormat.CFM_BACKCOLOR;
                cf.crBackColor = ColorTranslator.ToWin32(value);

                int result = RichTextNativeMethods.SendMessage(
                  base.Handle,
                  RichTextNativeMethods.EM_SETCHARFORMAT,
                  RichTextNativeMethods.SCF_SELECTION,
                  ref cf);
            }
        }

        /// <summary>
        /// Gets or sets the alignment to apply to the current
        /// selection or insertion point.
        /// </summary>
        /// <remarks>
        /// Replaces the SelectionAlignment from
        /// <see cref="RichTextBox"/>.
        /// </remarks>
        public new TextAlign SelectionAlignment
        {
            get
            {
                RichTextNativeMethods.PARAFORMAT2 fmt = new RichTextNativeMethods.PARAFORMAT2();
                fmt.cbSize = Marshal.SizeOf(fmt);

                // Get the alignment.
                RichTextNativeMethods.SendMessage(
                  this.Handle,
                  RichTextNativeMethods.EM_GETPARAFORMAT,
                  RichTextNativeMethods.SCF_SELECTION, 
                  ref fmt);

                // Default to Left align.
                if ((fmt.dwMask & RichTextNativeMethods.PFM_ALIGNMENT) == 0)
                    return TextAlign.Left;

                return (TextAlign)fmt.wAlignment;
            }

            set
            {
                RichTextNativeMethods.PARAFORMAT2 fmt = new RichTextNativeMethods.PARAFORMAT2();
                fmt.cbSize = Marshal.SizeOf(fmt);
                fmt.dwMask = RichTextNativeMethods.PFM_ALIGNMENT;
                fmt.wAlignment = (short)value;

                // Set the alignment.
                RichTextNativeMethods.SendMessage(
                  this.Handle,
                  RichTextNativeMethods.EM_SETPARAFORMAT,
                  RichTextNativeMethods.SCF_SELECTION, 
                  ref fmt);
            }
        }

        /// <summary>
        /// Gets or sets the Rtf string.
        /// </summary>
        public new string Rtf
        {
            get
            {
                return base.Rtf;
            }
            set
            {
                base.Rtf = value;

                OnRtfChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public override string Text
        {
            get
            {
                RichTextNativeMethods.GETTEXTLENGTHEX getLength = new RichTextNativeMethods.GETTEXTLENGTHEX();
                getLength.flags = RichTextNativeMethods.GTL_CLOSE; // get buffer size
                getLength.codepage = 1200; // Unicode

                int textLength = RichTextNativeMethods.SendMessage(
                    base.Handle, 
                    RichTextNativeMethods.EM_GETTEXTLENGTHEX, 
                    ref getLength, 
                    0);

                RichTextNativeMethods.GETTEXTEX getText = new RichTextNativeMethods.GETTEXTEX();
                getText.cb = textLength + 2; // add space for null terminator
                getText.flags = RichTextNativeMethods.GT_DEFAULT;
                getText.codepage = 1200; // Unicode

                StringBuilder sb = new StringBuilder(getText.cb);

                RichTextNativeMethods.SendMessage(base.Handle, RichTextNativeMethods.EM_GETTEXTEX, ref getText, sb);

                return sb.ToString();
            }
            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// Gets the string length.
        /// </summary>
        public override int TextLength
        {
            get
            {
                RichTextNativeMethods.GETTEXTLENGTHEX getLength = new RichTextNativeMethods.GETTEXTLENGTHEX();
                getLength.flags = RichTextNativeMethods.GTL_DEFAULT; // Returns the number of characters
                getLength.codepage = 1200; // Unicode

                return RichTextNativeMethods.SendMessage(base.Handle, RichTextNativeMethods.EM_GETTEXTLENGTHEX, ref getLength, 0);
            }
        }

        /// <summary>
        /// Gets or sets the underline color to apply to the
        /// current selection or insertion point.
        /// </summary>
        /// <remarks>
        /// Underline colors can be set to any value of the
        /// <see cref="UnderlineColor"/> enumeration.
        /// </remarks>
        public UnderlineColor SelectionUnderlineColor
        {
            get
            {
                RichTextNativeMethods.CHARFORMAT2 fmt = new RichTextNativeMethods.CHARFORMAT2();
                fmt.cbSize = Marshal.SizeOf(fmt);

                // Get the underline color.
                RichTextNativeMethods.SendMessage(
                  this.Handle,
                  RichTextNativeMethods.EM_GETCHARFORMAT,
                  RichTextNativeMethods.SCF_SELECTION, 
                  ref fmt);

                // Default to black.
                if ((fmt.dwMask & (uint)RichTextNativeMethods.RichEditFormat.CFM_UNDERLINETYPE) == 0)
                    return UnderlineColor.Default;

                byte style = (byte)(fmt.bUnderlineType & 0xF0);

                return (UnderlineColor)style;
            }
            set
            {
                // Ensure we don't alter the style.
                UnderlineStyle style = SelectionUnderlineStyle;

                // Ensure we don't show it if it shouldn't be shown.
                if (style == UnderlineStyle.None)
                    value = UnderlineColor.Default;

                RichTextNativeMethods.CHARFORMAT2 fmt = new RichTextNativeMethods.CHARFORMAT2();
                fmt.cbSize = Marshal.SizeOf(fmt);
                fmt.dwMask = (uint)RichTextNativeMethods.RichEditFormat.CFM_UNDERLINETYPE;
                fmt.bUnderlineType = (byte)((byte)style | (byte)value);

                // Set the underline color.
                RichTextNativeMethods.SendMessage(
                  this.Handle,
                  RichTextNativeMethods.EM_SETCHARFORMAT,
                  RichTextNativeMethods.SCF_SELECTION, 
                  ref fmt);
            }
        }

        /// <summary>
        /// Gets or sets the underline style to apply to the
        /// current selection or insertion point.
        /// </summary>
        /// <remarks>
        /// Underline styles can be set to any value of the
        /// <see cref="UnderlineStyle"/> enumeration.
        /// </remarks>
        public UnderlineStyle SelectionUnderlineStyle
        {
            get
            {
                RichTextNativeMethods.CHARFORMAT2 fmt = new RichTextNativeMethods.CHARFORMAT2();
                fmt.cbSize = Marshal.SizeOf(fmt);

                // Get the underline style.
                RichTextNativeMethods.SendMessage(
                  this.Handle,
                  RichTextNativeMethods.EM_GETCHARFORMAT,
                  RichTextNativeMethods.SCF_SELECTION, 
                  ref fmt);

                // Default to no underline.
                if ((fmt.dwMask & (uint)RichTextNativeMethods.RichEditFormat.CFM_UNDERLINETYPE) == 0)
                    return UnderlineStyle.None;

                byte style = (byte)(fmt.bUnderlineType & 0x0F);

                return (UnderlineStyle)style;
            }
            set
            {
                // Ensure we don't alter the color by accident.
                UnderlineColor color = SelectionUnderlineColor;

                // Ensure we don't show it if it shouldn't be shown.
                if (value == UnderlineStyle.None)
                    color = UnderlineColor.Default;

                RichTextNativeMethods.CHARFORMAT2 fmt = new RichTextNativeMethods.CHARFORMAT2();
                fmt.cbSize = Marshal.SizeOf(fmt);
                fmt.dwMask = (uint)RichTextNativeMethods.RichEditFormat.CFM_UNDERLINETYPE;
                fmt.bUnderlineType = (byte)((byte)value | (byte)color);

                // Set the underline type.
                RichTextNativeMethods.SendMessage(
                  this.Handle,
                  RichTextNativeMethods.EM_SETCHARFORMAT,
                  RichTextNativeMethods.SCF_SELECTION, 
                  ref fmt);
            }
        }

        #endregion

        #region non inheritable items
        /// <summary>
        /// Lock control updating till proper call of EndUpdate Method. Each call 
        /// of BeginUpdate must be corresponding EndUpdate, otherwise control redrawing
        /// will be locked
        /// </summary>
        protected void BeginUpdate()
        {
            m_iLockCounter++;

            if (m_iLockCounter == 1)
            {
                // optimize by disabling event messages
                m_iEventMask = RichTextNativeMethods.SendMessage(this.Handle, RichTextNativeMethods.EM_SETEVENTMASK, 0, 0);

                // optimize by disabling redraw
                RichTextNativeMethods.SendMessage(base.Handle, RichTextNativeMethods.WM_SETREDRAW, 0, 0);
            }
        }

        /// <summary>
        /// UnLock control update 
        /// </summary>
        protected void EndUpdate()
        {
            m_iLockCounter--;

            if (m_iLockCounter <= 0)
            {
                // restore event messages
                m_iEventMask = RichTextNativeMethods.SendMessage(this.Handle, RichTextNativeMethods.EM_SETEVENTMASK, 0, m_iEventMask);
                
                // restore redraw
                RichTextNativeMethods.SendMessage(base.Handle, RichTextNativeMethods.WM_SETREDRAW, 1, 0);

                m_iLockCounter = 0;
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Raises the <see cref="E:RtfChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnRtfChanged(EventArgs args)
        {
        }

        /// <summary>
        /// Underlines the selection.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="length">The length.</param>
        /// <param name="style">The style.</param>
        /// <param name="color">The color.</param>
        [CLSCompliant(false)]
        protected void UnderlineSelection(int position, int length, UnderlineStyle style, UnderlineColor color)
        {
            int selectionStart = base.SelectionStart;
            int selectionLength = base.SelectionLength;

            base.Select(position, length);

            this.SelectionUnderlineColor = color;
            this.SelectionUnderlineStyle = style;

            // restore selection
            base.Select(selectionStart, selectionLength);
        }

        /// <summary>
        /// Gets the char format.
        /// </summary>
        /// <returns>CHAR format.</returns>
        [CLSCompliant(false)]
        protected RichTextNativeMethods.CHARFORMAT2 GetCharFormat()
        {
            RichTextNativeMethods.CHARFORMAT2 cf = new RichTextNativeMethods.CHARFORMAT2();
            cf.cbSize = Marshal.SizeOf(cf);
            cf.dwMask = (uint)RichTextNativeMethods.RichEditFormat.CFM_UNDERLINETYPE;
            cf.dwEffects = (uint)RichTextNativeMethods.RichEditEffects.CFE_UNDERLINE;

            int result = RichTextNativeMethods.SendMessage(
              base.Handle,
              RichTextNativeMethods.EM_GETCHARFORMAT,
              RichTextNativeMethods.SCF_SELECTION,
              ref cf);

            return cf;
        }

        /// <summary>
        /// Toggles the selection font style.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ToggleSelectionFontStyle(System.Drawing.FontStyle value)
        {
            System.Drawing.Font oldFont = this.SelectionFont;

            if (oldFont != null)
            {
                System.Drawing.FontStyle style = oldFont.Style;
                style ^= value;

                System.Drawing.Font font = new Font(oldFont.FontFamily, oldFont.Size, style);
                this.SelectionFont = font;
            }
            else
            {
                BeginUpdate();

                // store old selection
                int selectionStart = base.SelectionStart;
                int selectionLength = base.SelectionLength;

                for (int i = selectionStart, len = selectionStart + selectionLength; i < len; i++)
                {
                    this.Select(i, 1);
                    ToggleSelectionFontStyle(value);
                }

                // restore old selection
                base.Select(selectionStart, selectionLength);

                EndUpdate();
            }
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// This member overrides
        /// <see cref="Control"/>.OnHandleCreated.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Enable support for justification.
            RichTextNativeMethods.SendMessage(
              this.Handle,
              RichTextNativeMethods.EM_SETTYPOGRAPHYOPTIONS,
              RichTextNativeMethods.TO_ADVANCEDTYPOGRAPHY,
              RichTextNativeMethods.TO_ADVANCEDTYPOGRAPHY);

            // call this method only when Handle created, On OS which does
            // not support themes this call will not make any effect
            RemoveTheme();
        }

        /// <summary> 
        /// Removes the themed style from the specified window. 
        /// </summary> 
        private void RemoveTheme()
        {
            // Call API function 
            if (this.IsHandleCreated)
            {
                RichTextNativeMethods.SetWindowTheme(this.Handle, " ", " ");
            }
        }
        #endregion
    }
}
