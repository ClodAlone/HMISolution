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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.ComponentBannerTextProviders;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.ComponentBannerTextProviders
{
    /// <summary>
    /// Can be directly used by user. Just adds new providers form Tools.Windows.
    /// </summary>
    internal class BannerTextProvider2 :
        BannerTextProvider
    {
        static BannerTextProvider2()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(BannerTextProvider2));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            RegisterProvider(new UpDownBannerTextProvider());
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            RegisterProvider(new ToolStripTextBoxBannerTextProvider());
            RegisterProvider(new ToolStripComboBoxBannerTextProvider());
            RegisterProvider(new ToolStripComboBoxExBannerTextProvider());
#endif
            RegisterProvider(new ComboBoxBarItemBannerTextProvider());
            RegisterProvider(new TextBoxBarItemBannerTextProvider());
        }

        internal static new void Extend(BannerTextProvider provider, IExtendableTexBox mainETB, IExtendableTexBox subETB)
        {
            BannerTextProvider.Extend(provider, mainETB, subETB);
        }
    }

    /// <summary>
    /// Banner text provider for UpDownBase-derived classes.
    /// </summary>
    internal class UpDownBannerTextProvider :
        ComponentBannerTextProviderBase
    {
        public UpDownBannerTextProvider()
        {
            m_type = typeof(UpDownBase);
        }

        #region IComponentBannerTextProvider implementation

        public override IExtendableTexBox GetExtendableTexBox(Component extendee, BannerTextProvider provider)
        {
            IExtendableTexBox etb = null;
            UpDownBase upDown = extendee as UpDownBase;

            if (upDown != null)
            {
                foreach (Control ctl in upDown.Controls)
                {
                    TextBox textBox = ctl as TextBox;

                    if (textBox != null)
                    {
                        etb = new ExtendableTextBox(textBox);
                        break;
                    }
                }
            }

            return etb;
        }

        #endregion
    }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    /// <summary>
    /// Banner text provider for ToolStripTextBox-derived classes.
    /// </summary>
    internal class ToolStripTextBoxBannerTextProvider :
        ComponentBannerTextProviderBase
    {
        public ToolStripTextBoxBannerTextProvider()
        {
            m_type = typeof(ToolStripTextBox);
        }

        #region IComponentBannerTextProvider implementation

        public override IExtendableTexBox GetExtendableTexBox(Component extendee, BannerTextProvider provider)
        {
            IExtendableTexBox etb = null;
            ToolStripTextBox tsTextBox = extendee as ToolStripTextBox;

            if (tsTextBox != null)
            {
                etb = new ExtendableTextBox(tsTextBox.TextBox);
            }

            return etb;
        }

        #endregion
    }

    /// <summary>
    /// Banner text provider for ToolStripComboBox-derived classes.
    /// </summary>
    internal class ToolStripComboBoxBannerTextProvider :
        ComponentBannerTextProviderBase
    {
        public ToolStripComboBoxBannerTextProvider()
        {
            m_type = typeof(ToolStripComboBox);
        }

        #region IComponentBannerTextProvider implementation

        public override IExtendableTexBox GetExtendableTexBox(Component extendee, BannerTextProvider provider)
        {
            IExtendableTexBox etb = null;
            ToolStripComboBox tsComboBox = extendee as ToolStripComboBox;

            if (tsComboBox != null)
            {
                etb = new ExtendableComboBoxTextBox(tsComboBox.ComboBox);
            }

            return etb;
        }

        #endregion
    }

    /// <summary>
    /// Banner text provider for ToolStripComboBoxEx-derived classes.
    /// </summary>
    internal class ToolStripComboBoxExBannerTextProvider :
        ComponentBannerTextProviderBase
    {
        public ToolStripComboBoxExBannerTextProvider()
        {
            m_type = typeof(ToolStripComboBoxEx);
        }

        #region IComponentBannerTextProvider implementation

        public override IExtendableTexBox GetExtendableTexBox(Component extendee, BannerTextProvider provider)
        {
            IExtendableTexBox etb = null;
            ToolStripComboBoxEx tsComboBoxEx = extendee as ToolStripComboBoxEx;

            if (tsComboBoxEx != null)
            {
                etb = new ExtendableComboBoxTextBox(tsComboBoxEx.ComboBox);
            }

            return etb;
        }

        #endregion
    }
#endif

    /// <summary>
    /// Abstract extendable text box wrapper for <see cref="BarItem"/>.
    /// </summary>
    internal abstract class ExtendableBarItemTextBox :
        ExtendableTextBoxBase
    {
        #region Data

        /// <summary>
        /// Extended bar item.
        /// </summary>
        protected BarItem m_barItem;

        /// <summary>
        /// Text box to extended text box info map.
        /// </summary>
        protected IDictionary m_textBox2ETB = new Hashtable();

        /// <summary>
        /// Extendable tex box wrapper.
        /// </summary>
        protected IExtendableTexBox m_etb;

        /// <summary>
        /// Instance of owning <see cref="BannerTextProvider"/>.
        /// </summary>
        protected BannerTextProvider m_provider;

        #endregion

        #region Construction

        public ExtendableBarItemTextBox(BarItem barItem, BannerTextProvider provider)
        {
            Debug.Assert(barItem != null, "The BarItem should not be null.");
            Debug.Assert(provider != null, "BannerTextProvider Value should no be null.");

            m_barItem = barItem;
            m_provider = provider;
        }

        #endregion

        #region Implementation

        protected void AdwiseTextBoxEvents(TextBoxBase textBoxBase)
        {
            textBoxBase.Disposed += new EventHandler(TextBoxDisposed);
        }

        protected void UnadwiseTextBoxEvents(TextBoxBase textBoxBase)
        {
            textBoxBase.Disposed -= new EventHandler(TextBoxDisposed);
        }

        private void TextBoxDisposed(object sender, EventArgs e)
        {
            TextBoxBase textBoxBase = (TextBoxBase)sender;
            IExtendableTexBox etb = (IExtendableTexBox)m_textBox2ETB[textBoxBase];

            m_textBox2ETB.Remove(textBoxBase);

            UnadwiseTextBoxEvents(textBoxBase);
            UndwiseExtendableTextBoxEvents(etb);
        }

        protected void AdwiseExtendableTextBoxEvents(IExtendableTexBox etb)
        {
            if (etb.Handle != IntPtr.Zero)
            {
                ExtandbaleTextBoxHandleCreated(this, EventArgs.Empty);
            }

            etb.TextBoxTextChanged += new ValueChangedEventHandler(ExtendableTextBoxTextChanged);
            etb.HandleCreated += new EventHandler(ExtandbaleTextBoxHandleCreated);
        }

        protected void UndwiseExtendableTextBoxEvents(IExtendableTexBox etb)
        {
            etb.TextBoxTextChanged -= new ValueChangedEventHandler(ExtendableTextBoxTextChanged);
            etb.HandleCreated -= new EventHandler(ExtandbaleTextBoxHandleCreated);
        }

        protected virtual void ExtandbaleTextBoxHandleCreated(object sender, EventArgs e)
        {
            OnHandleCreated(this, e);
        }

        protected virtual void ExtendableTextBoxTextChanged(object sender, ValueChangedEventArgs e)
        {
            OnTextBoxTextChanged(this, e);
        }

        protected virtual void TextBoxBarItemAfterPopupItemPaint(object sender, PopupItemPaintEventArgs drawItemInfo)
        {
            if (drawItemInfo.Element == DrawElement.TextBox || drawItemInfo.Element == DrawElement.ComboBox)
            {
                BannerTextInfo info = m_provider.GetBannerText(m_barItem);
                GridStyleInfo style = drawItemInfo.Style;

                if (info != null && BannerTextProvider.TextBoxExtender.IsBannerTextVisible(info, (m_etb != null ? m_etb.Focused : false), (style.Text.Length == 0)))
                {
                    Rectangle textRect = drawItemInfo.Bounds;
                    GridMargins textMargins = style.TextMargins.ToMargins();

                    textRect.X += textMargins.Width + textMargins.Left;
                    textRect.Width -= 2 * textMargins.Width;

                    BannerTextProvider.TextBoxExtender.DrawBannerText(drawItemInfo.Graphics, info, style.BackColor, style.Font.GdipFont, style.RightToLeft, textRect);

                    drawItemInfo.Handled = true;
                }
            }
        }

        protected void OnTextBoxBound(TextBoxBase textBoxBase)
        {
            if (!m_textBox2ETB.Contains(textBoxBase))
            {
                IExtendableTexBox etb = new ExtendableTextBox(textBoxBase);

                m_textBox2ETB.Add(textBoxBase, etb);

                if (m_etb == null)
                {
                    m_etb = etb;
                    AdwiseExtendableTextBoxEvents(m_etb);
                }
                else
                {
                    BannerTextProvider2.Extend(m_provider, this, etb);
                }
            }

            ExtendableTextBoxTextChanged(textBoxBase, new ValueChangedEventArgs(null, textBoxBase.Text));
        }
        #endregion

        #region IExtendableTextBox implementation

        public override IntPtr Handle
        {
            get
            {
                return m_etb != null ? m_etb.Handle : IntPtr.Zero;
            }
        }

        public override bool Focused
        {
            get
            {
                return m_etb != null ? m_etb.Focused : false;
            }
        }

        public override Color BackColor
        {
            get
            {
                return m_etb != null ? m_etb.BackColor : SystemColors.Window;
            }
        }

        public override Rectangle ClientRectangle
        {
            get
            {
                return m_etb != null ? m_etb.ClientRectangle : Rectangle.Empty;
            }
        }

        public override Font Font
        {
            get
            {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                return m_etb != null ? m_etb.Font : SystemFonts.DefaultFont;
#else
				return m_etb != null ? m_etb.Font : (new TextBox()).Font;
#endif
            }
        }

        public override RightToLeft RightToLeft
        {
            get
            {
                RightToLeft rtl = RightToLeft.Inherit;

                if (m_etb != null)
                {
                    rtl = m_etb.RightToLeft;
                }
                else if (m_barItem.Manager != null)
                {
                    rtl = m_barItem.Manager.RightToLeft;
                }

                return rtl;
            }
        }

        public override void Invalidate()
        {
            if (m_etb != null)
            {
                m_etb.Invalidate();
            }
        }

        #endregion
    }

    /// <summary>
    /// Extendable text box wrapper for <see cref="ComboBoxBarItem"/>.
    /// </summary>
    internal class ExtendableComboBoxBarItemTextBox :
        ExtendableBarItemTextBox
    {
        public ExtendableComboBoxBarItemTextBox(ComboBoxBarItem cbBarItem, BannerTextProvider provider) :
            base(cbBarItem, provider)
        {
            cbBarItem.TextBoxBound += new TextBoxBoundEventHandler(ComboBoxBarItemTextBoxBound);
            cbBarItem.AfterPopupItemPaint += new PopupItemPaintEventHandler(TextBoxBarItemAfterPopupItemPaint);
        }

        private void ComboBoxBarItemTextBoxBound(object sender, TextBoxBoundEventArgs args)
        {
            OnTextBoxBound(args.TextBox);
        }

        protected override void ExtandbaleTextBoxHandleCreated(object sender, EventArgs e)
        {
            base.ExtandbaleTextBoxHandleCreated(sender, e);

            ComboBoxBarItem cbbi = (ComboBoxBarItem)m_barItem;

            ExtendableTextBoxTextChanged(sender, new ValueChangedEventArgs(null, cbbi.TextBoxValue));
        }
    }

    /// <summary>
    /// Banner text provider ComboBoxBarItem-derived classes.
    /// </summary>
    internal class ComboBoxBarItemBannerTextProvider :
        ComponentBannerTextProviderBase
    {
        public ComboBoxBarItemBannerTextProvider()
        {
            m_type = typeof(ComboBoxBarItem);
        }

        #region IComponentBannerTextProvider implementation

        public override bool CanExtend(Component extendee)
        {
            bool bCanExtend = false;
            ComboBoxBarItem combo = extendee as ComboBoxBarItem;

            if (combo != null && base.CanExtend(extendee))
            {
                // Text portion of the constorl is editable with Simple and DropDown style set.
                bCanExtend = combo.Editable;
            }

            return bCanExtend;
        }

        public override IExtendableTexBox GetExtendableTexBox(Component extendee, BannerTextProvider provider)
        {
            IExtendableTexBox etb = null;
            ComboBoxBarItem combo = extendee as ComboBoxBarItem;

            if (combo != null)
            {
                etb = new ExtendableComboBoxBarItemTextBox(combo, provider);
            }

            return etb;
        }

        #endregion
    }

    /// <summary>
    /// Extendable text box wrapper for <see cref="TextBoxBarItem"/>.
    /// </summary>
    internal class ExtendableTextBoxBarItemTextBox :
        ExtendableBarItemTextBox
    {
        public ExtendableTextBoxBarItemTextBox(TextBoxBarItem tbBarItem, BannerTextProvider provider) :
            base(tbBarItem, provider)
        {
            tbBarItem.TextBoxItemBound += new TextBoxItemBoundEventHandler(TextBoxBarItemTextBoxBound);
            tbBarItem.AfterPopupItemPaint += new PopupItemPaintEventHandler(TextBoxBarItemAfterPopupItemPaint);
        }

        protected void TextBoxBarItemTextBoxBound(object sender, TextBoxItemBoundEventArgs args)
        {
            OnTextBoxBound(args.TextBox);
        }

        protected override void ExtandbaleTextBoxHandleCreated(object sender, EventArgs e)
        {
            base.ExtandbaleTextBoxHandleCreated(sender, e);

            TextBoxBarItem tbbi = (TextBoxBarItem)m_barItem;

            ExtendableTextBoxTextChanged(sender, new ValueChangedEventArgs(null, tbbi.TextBoxValue));
        }
    }

    /// <summary>
    /// Banner text provider TextBoxBarItem-derived classes.
    /// </summary>
    internal class TextBoxBarItemBannerTextProvider :
        ComponentBannerTextProviderBase
    {
        public TextBoxBarItemBannerTextProvider()
        {
            m_type = typeof(TextBoxBarItem);
        }

        #region IComponentBannerTextProvider implementation

        public override IExtendableTexBox GetExtendableTexBox(Component extendee, BannerTextProvider provider)
        {
            IExtendableTexBox etb = null;
            TextBoxBarItem tbBarItem = extendee as TextBoxBarItem;

            if (tbBarItem != null)
            {
                etb = new ExtendableTextBoxBarItemTextBox(tbBarItem, provider);
            }

            return etb;
        }

        #endregion
    }
}
