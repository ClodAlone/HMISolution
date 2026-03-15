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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools.Navigation
{
    using Layouting;

    /// <summary>
    /// <see cref="NavigationView"/> visual styles.
    /// </summary>
    public enum VisualStyles
    {
        /// <summary>
        /// Represents office 2007 Style
        /// </summary>
        Office2007,

        /// <summary>
        /// Represents Vista Style
        /// </summary>
        Vista,
         /// <summary>
        /// Represents Metro Style
        /// </summary>
        Metro
    }

    internal class Visualizer :
        Component
    {
        #region Fields

        private NavigationView _navView;
        private VisualStyles _style;
        private ILayouter _layouter;
        private IAreasStateCache _states;
        private Rendering.IRenderer _rendrer;

        #endregion

        #region Construction

        public Visualizer(NavigationView navigationView, VisualStyles style)
        {
            _navView = navigationView;
            _style = style;

            OnStyleChanged();
        }

        #endregion

        #region Properties

        public VisualStyles Style
        {
            get
            {
                return _style;
            }
            set
            {
                if (_style != value)
                {
                    _style = value;

                    OnStyleChanged();
                }
            }
        }

        public ILayouter Layouter
        {
            get
            {
                return _layouter;
            }
        }

        public IAreasStateCache States
        {
            get
            {
                return _states;
            }
        }

        public Rendering.IRenderer Renderer
        {
            get
            {
                return _rendrer;
            }
        }

        #endregion

        #region Methods

        public void RenderControl(Graphics graphics)
        {
            _rendrer.RenderControl(_navView, graphics, _layouter, _states);
        }

        public LayoutInfo HitTest(Point point)
        {
            return this.Layouter.HitTest(point);
        }

        public void UpdateLayout()
        {
            UpdateLayout(true);
        }

        public void UpdateLayout(bool invalidate)
        {
            if (_navView != null && !_navView.Initializing && _navView.IsHandleCreated)
            {
                if (_navView != null)
                {
                    _layouter.UpdateLayout(_navView);
                }

                UpdateHistoryButtonsState(false);

                if (invalidate)
                {
                    _navView.Invalidate();
                }
            }
        }

        public void UpdateHistoryButtonsState(bool invalidate)
        {
            if (_navView.ShowHistoryButtons)
            {
                if (_navView.HistoryEnabled && _navView.HistoryManager != null)
                {
                    bool canUndo = _navView.HistoryManager.CanUndo;
                    bool canRedo = _navView.HistoryManager.CanRedo;
                    bool dropDown = (canUndo || canRedo) && (_navView.HistoryManager is HistoryManager);

                    SetState(HitTestAreas.HistoryBackButton, canUndo ? AreaStates.Default : AreaStates.Disabled);
                    SetState(HitTestAreas.HistoryForwardButton, canRedo ? AreaStates.Default : AreaStates.Disabled);
                    SetState(HitTestAreas.HistoryDropDownButton, dropDown ? AreaStates.Default : AreaStates.Disabled);
                }
                else
                {
                    SetState(HitTestAreas.HistoryBackButton, AreaStates.Disabled);
                    SetState(HitTestAreas.HistoryForwardButton, AreaStates.Disabled);
                    SetState(HitTestAreas.HistoryDropDownButton, AreaStates.Disabled);
                }
            }

            if (invalidate)
            {
                _navView.Invalidate();
            }
        }

        public AreaStates GetState(HitTestAreas area)
        {
            AreaStates state = AreaStates.Default;
            LayoutInfo li = _layouter.GetLayout(area);

            if (li != null)
            {
                state = _states.GetAreaState(li);
            }

            return state;
        }

        public void SetState(HitTestAreas area, AreaStates state)
        {
            LayoutInfo li = _layouter.GetLayout(area);

            if (li != null)
            {
                _states.SetAreaState(li, state);
            }
        }

        #endregion

        #region Implementation

        private void OnStyleChanged()
        {
            DisposeRenderer();

            if (_layouter == null)
            {
                _layouter = new Layouting.LayouterBase();
                _states = (IAreasStateCache)_layouter;
            }

            switch (_style)
            {
                case VisualStyles.Office2007:
                    {
                        _rendrer = new Rendering.Office2007Render();
                        break;
                    }

                case VisualStyles.Vista:
                    {
                        _rendrer = new Rendering.VistaRender();
                        break;
                    }
                case VisualStyles.Metro :
                        {
                            _rendrer = new Rendering.MetroRender();
                            break;
                        }
            }

            UpdateLayout();
        }

        private void DisposeRenderer()
        {
            if (_rendrer != null)
            {
                IDisposable renderer = (IDisposable)_rendrer;

                renderer.Dispose();
                _rendrer = null;
            }
        }

        #endregion

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeRenderer();

                if (_layouter == null)
                {
                    _layouter.Dispose();
                    _layouter = null;
                }

                _navView = null;
                _states = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    namespace Rendering
    {
        /// <summary>
        /// Specifies that this object implements rendering for <see cref="NavigationView"/> control.
        /// </summary>
        public interface IRenderer :
            IDisposable
        {
            /// <summary>
            /// Renders control.
            /// </summary>
            /// <param name="navView"><see cref="NavigationView"/> control to render.</param>
            /// <param name="g"><see cref="NavigationView"/> control's <see cref="Graphics"/>.</param>
            /// <param name="layouter"><see cref="NavigationView"/> control's layouter.</param>
            /// <param name="states"><see cref="NavigationView"/> control's area states.</param>
            void RenderControl(NavigationView navView, Graphics g, ILayouter layouter, IAreasStateCache states);
        }

        /// <summary>
        /// Base renderer for <see cref="NavigationView"/> control.
        /// </summary>
        public abstract class RendrerBase :
            IRenderer,
            IDisposable
        {
            #region Constants

            public readonly int PressedOffset = 1;

            #endregion

            #region Fields

            protected NavigationView m_navView;
            protected Graphics m_g;
            protected ILayouter m_layouter;
            protected IAreasStateCache m_states;

            #endregion

            #region IRenderer implementation

            /// <summary>
            /// Renders control.
            /// </summary>
            /// <param name="navView"><see cref="NavigationView"/> control to render.</param>
            /// <param name="g"><see cref="NavigationView"/> control's <see cref="Graphics"/>.</param>
            /// <param name="layouter"><see cref="NavigationView"/> control's layouter.</param>
            /// <param name="states"><see cref="NavigationView"/> control's area states.</param>
            public virtual void RenderControl(NavigationView navView, Graphics g, ILayouter layouter, IAreasStateCache states)
            {
                InitRenderer(navView, g, layouter, states);

                RenderBackground();

                LayoutInfo imageLayout = layouter.GetLayout(HitTestAreas.BarImage);

                if (imageLayout != null)
                {
                    RenderBarImage(imageLayout);
                }

                RenderHistoryButtonsArea();
                RenderBarsArea();

                LayoutInfo ddBtn = m_layouter.GetLayout(HitTestAreas.DropDownButton);

                if (ddBtn != null)
                {
                    RenderDropDown(ddBtn);
                }

                LayoutInfo customBtns = m_layouter.GetLayout(HitTestAreas.CustomButtons);

                if (customBtns != null)
                {
                    RenderCustomButtonsArea();
                }

                RenderBorders();

                ClearReferences();
            }

            /// <summary>
            /// Initializes the renderer.
            /// </summary>
            /// <param name="navView"><see cref="NavigationView"/> control to render.</param>
            /// <param name="g"><see cref="NavigationView"/> control's <see cref="Graphics"/>.</param>
            /// <param name="layouter"><see cref="NavigationView"/> control's layouter.</param>
            /// <param name="states"><see cref="NavigationView"/> control's area states.</param>
            protected virtual void InitRenderer(NavigationView navView, Graphics g, ILayouter layouter, IAreasStateCache states)
            {
                m_navView = navView;
                m_g = g;
                m_layouter = layouter;
                m_states = states;
            }

            #endregion

            #region Implementation

            /// <summary>
            /// Renders control's background.
            /// </summary>
            protected virtual void RenderBackground()
            {
                using (Brush brush = new SolidBrush(Office2007BlueColors.Default.TabBarSplitterTabEndColor))
                {
                    m_g.FillRectangle(brush, m_navView.ClientRectangle);
                }
            }

            /// <summary>
            /// Renders control's borders.
            /// </summary>
            protected virtual void RenderBorders()
            {
                if (m_navView.ShowBorder)
                {
                    Rectangle bounds = m_layouter.BorderBounds;

                    using (Pen p = new Pen(m_navView.BorderColor))
                    {
                        m_g.DrawRectangle(p, bounds);
                    }
                }
            }

            #region History buttons' area

            /// <summary>
            /// Renders the history buttons' area.
            /// </summary>
            protected virtual void RenderHistoryButtonsArea()
            {
                RenderHistoryButtonsBackground();

                LayoutInfo histBackBtn = m_layouter.GetLayout(HitTestAreas.HistoryBackButton);

                if (histBackBtn != null)
                {
                    RenderHistoryBackButton(histBackBtn);
                }

                LayoutInfo histFwdBtn = m_layouter.GetLayout(HitTestAreas.HistoryForwardButton);

                if (histFwdBtn != null)
                {
                    RenderHistoryForwardButton(histFwdBtn);
                }

                LayoutInfo histDDBtn = m_layouter.GetLayout(HitTestAreas.HistoryDropDownButton);

                if (histDDBtn != null)
                {
                    RenderHistoryDropDownButton(histDDBtn);
                }
            }

            /// <summary>
            /// Renders the history buttons's background.
            /// </summary>
            protected virtual void RenderHistoryButtonsBackground()
            {
            }

            /// <summary>
            /// Renders the selected bar's image.
            /// </summary>
            /// <param name="layout">The layout info.</param>
            protected virtual void RenderBarImage(LayoutInfo layout)
            {
                Bar bar = m_navView.SelectedBar;
                Image image = Bar.Helper.GetBarImage(bar, m_navView);
                AreaStates state = m_states.GetAreaState(layout);

                if (image != null)
                {
                    Rectangle bounds = GetBarImageBounds(state, layout.Bounds);
                    Rectangle imgBounds = new Rectangle(Point.Empty, image.Size);

                    m_g.DrawImage(image, bounds, imgBounds, GraphicsUnit.Pixel);
                }
                else if (!bar.Enabled)
                {
                    image = Bar.Helper.GetBarImage(bar, m_navView, true);

                    if (image != null)
                    {
                        Rectangle bounds = GetBarImageBounds(state, layout.Bounds);
                        Rectangle imgBounds = new Rectangle(Point.Empty, image.Size);

                        DrawingUtils.DrawGrayedImage(m_g, image, bounds, imgBounds, 1f);
                    }
                }
            }

            private Rectangle GetBarImageBounds(AreaStates state, Rectangle bounds)
            {
                if (state == AreaStates.Pressed && !m_navView.InEditingMode)
                {
                    bounds.Offset(1, 1);
                }

                return bounds;
            }

            /// <summary>
            /// Renders the history button's image.
            /// </summary>
            /// <param name="image">The image.</param>
            /// <param name="bounds">The bounds.</param>
            protected virtual void RenderHistoryButtonsImage(string dropdownstate, Rectangle bounds)
            {
                SolidBrush brush = new SolidBrush(Color.LightGray);
                if (this.m_navView.VisualStyle == VisualStyles.Metro)
                    brush = new SolidBrush(Color.White);
                m_g.FillRectangle(brush, bounds);
                brush.Dispose();
                string path = "Syncfusion.Windows.Forms.Tools.Controls.NavigationView.MetroImages." + dropdownstate + ".png";
                using (Bitmap bmp = new Bitmap(typeof(NavigationView).Assembly.GetManifestResourceStream(path)))
                {
                    using (CMirroredDrawer md = new CMirroredDrawer(m_g, bounds, (m_navView.RightToLeft == RightToLeft.Yes)))
                    {
                        if (bmp != null)
                            m_g.DrawImage(bmp, bounds);
                    }
                }
            }
            protected virtual void RenderHistoryButtonsImage(Image image, Rectangle bounds)
            {
                using (CMirroredDrawer md = new CMirroredDrawer(m_g, bounds, (m_navView.RightToLeft == RightToLeft.Yes)))
                {
                    m_g.DrawImage(image, bounds);
                }
            }
            protected virtual void MetroRenderHistoryButtonImage(Rectangle bounds , string historystate)
            {
                string state = null;
                state = historystate;
                SolidBrush brush = new SolidBrush(Color.White);
                m_g.FillRectangle(brush, bounds);
                brush.Dispose();
                string path = "Syncfusion.Windows.Forms.Tools.Controls.NavigationView.MetroImages." + state + ".png";
                Bitmap bmp = new Bitmap(typeof(Clock).Assembly.GetManifestResourceStream(path));
                if (bmp != null)
                m_g.DrawImage(bmp, bounds);
            }

            /// <summary>
            /// Renders the history back button.
            /// </summary>
            /// <param name="layout">The area's layout information.</param>
            protected virtual void RenderHistoryBackButton(LayoutInfo layout)
            {
                Image image = Resources.hbDefault;

                switch (m_states.GetAreaState(layout))
                {
                    case AreaStates.Hot:
                        image = Resources.hbHot;
                        break;

                    case AreaStates.Disabled:
                        image = Resources.hbDisabled;
                        break;

                    case AreaStates.Pressed:
                        image = Resources.hbPressed;
                        break;
                }

                RenderHistoryButtonsImage(image, layout.Bounds);
            }

            /// <summary>
            /// Renders the history forward button.
            /// </summary>
            /// <param name="layout">The area's layout information.</param>
            protected virtual void RenderHistoryForwardButton(LayoutInfo layout)
            {
                Image image = Resources.hfDefault;

                switch (m_states.GetAreaState(layout))
                {
                    case AreaStates.Hot:
                        image = Resources.hfHot;
                        break;

                    case AreaStates.Disabled:
                        image = Resources.hfDisabled;
                        break;

                    case AreaStates.Pressed:
                        image = Resources.hfPressed;
                        break;
                }

                RenderHistoryButtonsImage(image, layout.Bounds);
            }

            /// <summary>
            /// Renders the history drop down button.
            /// </summary>
            /// <param name="layout">The area's layout information.</param>
            protected virtual void RenderHistoryDropDownButton(LayoutInfo layout)
            {
                Image image = Resources.hddDefault;

                switch (m_states.GetAreaState(layout))
                {
                    case AreaStates.Hot:
                        image = Resources.hddHot;
                        break;

                    case AreaStates.Disabled:
                        image = Resources.hddDisabled;
                        break;

                    case AreaStates.Pressed:
                        image = Resources.hddPressed;
                        break;
                }

                RenderHistoryButtonsImage(image, layout.Bounds);
            }

            #endregion

            /// <summary>
            /// Renders the editor area.
            /// </summary>
            protected abstract void RenderEditorArea();

            /// <summary>
            /// Sets the color of the text box background.
            /// </summary>
            /// <param name="clrBack">The background color.</param>
            protected void SetTextBoxBackColor(Color clrBack)
            {
                TextBox textBox = m_navView.TextBox;

                textBox.BackColor = clrBack;
            }

            #region Bars' area

            /// <summary>
            /// Renders the bars' area.
            /// </summary>
            protected virtual void RenderBarsArea()
            {
                if (m_navView.AllowEditMode && m_navView.InEditingMode)
                {
                    RenderEditorArea();
                }
                else
                {
                    RenderBars();
                }
            }

            /// <summary>
            /// Renders the bars.
            /// </summary>
            protected virtual void RenderBars()
            {
                BarLayoutInfoCollection layouts = m_layouter.BarLayoutInfos;
                List<Bar> bars = m_navView.SelectedBars;

                foreach (BarLayoutInfo info in layouts)
                {
                    Bar bar = bars[info.Index];

                    RenderBar(bar, info);
                }
            }

            /// <summary>
            /// Renders the bar.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected virtual void RenderBar(Bar bar, BarLayoutInfo layout)
            {
                RenderBarBackground(bar, layout);

                if (!layout.Text.Bounds.IsEmpty)
                {
                    RenderBarTextArea(bar, layout);
                }

                if (!layout.DropDown.Bounds.IsEmpty)
                {
                    RenderBarDropDown(bar, layout);
                }

                RenderBarBorders(bar, layout);
            }

            /// <summary>
            /// Renders the bar's background.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected abstract void RenderBarBackground(Bar bar, BarLayoutInfo layout);

            /// <summary>
            /// Renders the border within specified bounds with specified state.
            /// </summary>
            /// <param name="state">The state.</param>
            /// <param name="bounds">The bounds.</param>
            protected abstract void RenderBorder(AreaStates state, Rectangle bounds);

            /// <summary>
            /// Renders the bar's text area.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected virtual void RenderBarTextArea(Bar bar, BarLayoutInfo layout)
            {
                AreaStates state = m_states.GetAreaState(layout);

                RenderBarText(bar, state, layout.Text.Bounds);
            }

            /// <summary>
            /// Renders the bar's text.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="state">The state.</param>
            /// <param name="textBounds">The text bounds.</param>
            protected virtual void RenderBarText(Bar bar, AreaStates state, Rectangle textBounds)
            {
                if (!textBounds.IsEmpty)
                {
                    TextFormatFlags textFormat = m_layouter.TextFormat;

                    if (m_navView.RightToLeft != RightToLeft.Yes)
                    {
                        textFormat &= ~TextFormatFlags.RightToLeft;
                    }
                    else
                    {
                        textFormat |= TextFormatFlags.RightToLeft;
                    }

                    if(m_navView.VisualStyle == VisualStyles.Office2007 && m_navView.Office2007ColorTheme == Office2007Theme.Black 
                        && state == AreaStates.Default)
                        TextRenderer.DrawText(m_g, bar.Text, m_navView.Font, textBounds, Color.White, textFormat);
                    else
                        TextRenderer.DrawText(m_g, bar.Text, m_navView.Font, textBounds, m_navView.ForeColor, textFormat);
                }
            }

            /// <summary>
            /// Renders the bar's drop down button.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected virtual void RenderBarDropDown(Bar bar, BarLayoutInfo layout)
            {
                RenderBarDropDownArrow(bar, layout);
            }

            /// <summary>
            /// Renders the bar drop-down button's arrow.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected virtual void RenderBarDropDownArrow(Bar bar, BarLayoutInfo layout)
            {
                AreaStates state = m_states.GetAreaState(layout);
                Image image;

                if (layout.ShowShevron)
                {
                    image = Resources.chevron;
                }
                else
                {
                    image = (state == AreaStates.Pressed) ? Resources.arrowD : Resources.arrowR;
                }

                Rectangle imgBounds = GetBarDropDownArrowBounds(layout, image);

                using (CMirroredDrawer md = new CMirroredDrawer(m_g, imgBounds, (m_navView.RightToLeft == RightToLeft.Yes)))
                {
                    m_g.DrawImage(image, imgBounds);
                }
            }

            /// <summary>
            /// Gets the bounds of bar's drop-down button's arrow.
            /// </summary>
            /// <param name="layout">The bar's layout.</param>
            /// <param name="image">The image.</param>
            /// <returns>The arrow bounds.</returns>
            protected virtual Rectangle GetBarDropDownArrowBounds(BarLayoutInfo layout, Image image)
            {
                Rectangle bounds = layout.DropDown.Bounds;
                Point imgLoc = new Point(bounds.Left + (bounds.Width - image.Width) / 2, bounds.Top + (bounds.Height - image.Height) / 2);
                Rectangle imgBounds = new Rectangle(imgLoc, image.Size);

                return imgBounds;
            }

            /// <summary>
            /// Renders the bar's borders.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected virtual void RenderBarBorders(Bar bar, BarLayoutInfo layout)
            {
                AreaStates barState = m_states.GetAreaState(layout);
                bool bDrawBarBorder = barState != AreaStates.Default;
                if (m_navView.VisualStyle == VisualStyles.Office2007 || m_navView.VisualStyle == VisualStyles.Vista)
                {
                    RenderBarBorder(barState, bDrawBarBorder, layout.Bounds);
                    RenderBarDropDownBorder(barState, bDrawBarBorder, layout);
                }
            }

            /// <summary>
            /// Renders the bar's border.
            /// </summary>
            /// <param name="barState">State of the drop-down button.</param>
            /// <param name="drawBarBorder">if set to <c>true</c>, bar's border is drawn.</param>
            /// <param name="bounds">The bounds.</param>
            protected virtual void RenderBarBorder(AreaStates barState, bool drawBarBorder, Rectangle bounds)
            {
                if (drawBarBorder)
                {
                    RenderBorder(barState, bounds);
                }
            }

            /// <summary>
            /// Renders the border of bar's drop-down button.
            /// </summary>
            /// <param name="barState">State of the drop-down button.</param>
            /// <param name="bDrawBarBorder">if set to <c>true</c>, bar's border is drawn.</param>
            /// <param name="layout">The bar's layout.</param>
            protected virtual void RenderBarDropDownBorder(AreaStates barState, bool bDrawBarBorder, BarLayoutInfo layout)
            {
                LayoutInfo liDD = layout.DropDown;
                Rectangle ddBounds = liDD.Bounds;

                if (ddBounds.Width > 0 && ddBounds.Height > 0)
                {
                    AreaStates barDDState = m_states.GetAreaState(layout);

                    if (bDrawBarBorder || (barDDState != AreaStates.Default))
                    {
                        RenderBorder(barState, ddBounds);
                    }
                }
            }

            /// <summary>
            /// Creates background brush for a <see cref="Bar"/>.
            /// </summary>
            /// <param name="bounds">The bounds.</param>
            /// <param name="cl1">First color.</param>
            /// <param name="cl2">Second color.</param>
            /// <param name="blend">The blend.</param>
            /// <returns>Created background brush.</returns>
            protected static Brush GetBarBackgroundBrush(Rectangle bounds, Color cl1, Color cl2, Blend blend)
            {
                LinearGradientBrush brush = new LinearGradientBrush(bounds, cl1, cl2, 90F);
                brush.Blend = blend;
                brush.WrapMode = WrapMode.TileFlipXY;

                return brush;
            }

            #endregion

            #region Drop-down button's area

            /// <summary>
            /// Renders the drop-down button.
            /// </summary>
            /// <param name="layout">The drop-down button's layout.</param>
            protected abstract void RenderDropDown(LayoutInfo layout);

            #endregion

            /// <summary>
            /// Renders custom buttons' area.
            /// </summary>
            protected virtual void RenderCustomButtonsArea()
            {
            }

            #endregion

            #region IDisposable implementation

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="RendrerBase"/> class.
            /// </summary>
            ~RendrerBase()
            {
                Dispose(false);
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources
            /// </summary>
            /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            protected void Dispose(bool disposing)
            {
                if (disposing)
                {
                    ClearReferences();
                }
            }

            private void ClearReferences()
            {
                m_navView = null;
                m_g = null;
                m_layouter = null;
            }

            #endregion
        }

        internal class Office2007Render :
            RendrerBase
        {
            #region Fields

            protected NavigationViewDropDownButton m_dropDownButton;

            #endregion

            #region Properties

            /// <summary>
            /// Gets current Office2007 color table.
            /// </summary>
            protected Office2007Colors ColorTable
            {
                get
                {
                    return Office2007Colors.GetColorTable(m_navView.Office2007ColorTheme);
                }
            }

            #endregion

            #region Overrides

            /// <summary>
            /// Initializes the renderer.
            /// </summary>
            /// <param name="navView"><see cref="NavigationView"/> control to render.</param>
            /// <param name="g"><see cref="NavigationView"/> control's <see cref="Graphics"/>.</param>
            /// <param name="layouter"><see cref="NavigationView"/> control's layouter.</param>
            /// <param name="states"><see cref="NavigationView"/> control's area states.</param>
            protected override void InitRenderer(NavigationView navView, Graphics g, ILayouter layouter, IAreasStateCache states)
            {
                base.InitRenderer(navView, g, layouter, states);

                if (m_dropDownButton != null && m_dropDownButton.NavView != navView)
                {
                    ((IDisposable)m_dropDownButton).Dispose();
                    m_dropDownButton = null;
                }

                if (m_dropDownButton == null)
                {
                    m_dropDownButton = new NavigationViewDropDownButton(m_navView);
                    m_dropDownButton.Style = VisualStyle.Office2007;
                }
            }

            /// <summary>
            /// Renders the editor area.
            /// </summary>
            protected override void RenderEditorArea()
            {
                LayoutInfo spaceLayout = m_layouter.GetLayout(HitTestAreas.Space);
                Rectangle rcSpace = spaceLayout.Bounds;

                if (rcSpace.Width > 0 && rcSpace.Height > 0)
                {
                    Office2007Colors colors = this.ColorTable;

                    SetTextBoxBackColor(colors.MenuTextBoxBackColor);

                    ++rcSpace.Width;

                    using (Brush b = new SolidBrush(colors.MenuTextBoxBackColor))
                    {
                        m_g.FillRectangle(b, rcSpace);
                    }

                    using (Pen pen = new Pen(colors.MenuTextBoxBorderColor))
                    {
                        --rcSpace.Height;

                        m_g.DrawRectangle(pen, rcSpace);
                    }
                }
            }

            /// <summary>
            /// Renders the bars.
            /// </summary>
            protected override void RenderBars()
            {
                base.RenderBars();

                LayoutInfo space = m_layouter.GetLayout(HitTestAreas.Space);

                if (space != null)
                {
                    using (Brush b = new SolidBrush(this.BackgroundColor))
                    {
                        m_g.FillRectangle(b, space.Bounds);
                    }
                }
            }

            /// <summary>
            /// Renders the bar's background.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected override void RenderBarBackground(Bar bar, BarLayoutInfo layout)
            {
                RenderArea(layout);
            }

            /// <summary>
            /// Renders the border within specified bounds with specified state.
            /// </summary>
            /// <param name="state">The state.</param>
            /// <param name="bounds">The bounds.</param>
            protected override void RenderBorder(AreaStates state, Rectangle bounds)
            {
                if (bounds.Width > 0 && bounds.Height > 0)
                {
                    Office2007Colors colorTable = this.ColorTable;
                    Color clrBorder;

                    switch (state)
                    {
                        case AreaStates.Hot:
                        case AreaStates.Active:
                            clrBorder = colorTable.ButtonSelectedBorderColor;
                            break;

                        case AreaStates.Pressed:
                            clrBorder = colorTable.ButtonPressedBorderColor;
                            break;

                        default:
                            clrBorder = colorTable.ButtonDefaultBorderColor;
                            break;
                    }

                    using (Pen pen = new Pen(clrBorder))
                    {
                        --bounds.Height;
                        --bounds.Width;

                        m_g.DrawRectangle(pen, bounds);
                    }
                }
            }

            /// <summary>
            /// Renders the bar's drop down button.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected override void RenderBarDropDown(Bar bar, BarLayoutInfo layout)
            {
                LayoutInfo liDD = layout.DropDown;
                AreaStates barDDState = m_states.GetAreaState(liDD);
                Rectangle bounds = layout.DropDown.Bounds;

                if (barDDState == AreaStates.Hot || barDDState == AreaStates.Pressed)
                {
                    RenderBackgroundRect(barDDState, bounds);
                }

                if (barDDState != AreaStates.Default)
                {
                    RenderBorder(barDDState, bounds);
                }

                base.RenderBarDropDown(bar, layout);
            }

            /// <summary>
            /// Renders the bar borders.
            /// </summary>
            /// <param name="bar">The bar.</param>
            /// <param name="info">The info.</param>
            protected override void RenderBarBorders(Bar bar, BarLayoutInfo info)
            {
                AreaStates barState = m_states.GetAreaState(info);
                bool bDrawBarBorder = (barState == AreaStates.Active) || (barState == AreaStates.Pressed);

                if (bDrawBarBorder)
                {
                    RenderBorder(barState, info.Bounds);
                }

                LayoutInfo liDD = info.DropDown;

                if (!liDD.Bounds.IsEmpty)
                {
                    AreaStates barDDState = m_states.GetAreaState(liDD);

                    if (bDrawBarBorder || (barDDState == AreaStates.Hot) || (barDDState == AreaStates.Pressed))
                    {
                        RenderBorder(barState, liDD.Bounds);
                    }
                }
            }

            /// <summary>
            /// Renders the drop down.
            /// </summary>
            /// <param name="layout">The drop-down button's layout.</param>
            protected override void RenderDropDown(LayoutInfo layout)
            {
                Rectangle bounds = layout.Bounds;

                m_dropDownButton.SuspendInvalidates = true;
                m_dropDownButton.Office2007ColorTheme = m_navView.Office2007ColorTheme;

                ResetDropDownState();

                m_dropDownButton.IsActive = true;

                switch (m_states.GetAreaState(layout))
                {
                    case AreaStates.Active:
                    case AreaStates.Hot:
                        {
                            m_dropDownButton.Hot = true;
                            break;
                        }
                    case AreaStates.Pressed:
                        {
                            m_dropDownButton.Pushed = true;
                            break;
                        }
                    case AreaStates.Disabled:
                        {
                            m_dropDownButton.Enabled = false;
                            break;
                        }
                }

                m_dropDownButton.Bounds = bounds;
                m_dropDownButton.OnPaint(m_g);

                m_dropDownButton.SuspendInvalidates = false;
            }

            #endregion

            #region Implementation

            private Color BackgroundColor
            {
                get
                {
                    Office2007Colors colors = Office2007Colors.GetColorTable(m_navView.Office2007ColorTheme);

                    return colors.TabBarSplitterTabEndColor;
                }
            }

            private void RenderArea(LayoutInfo layout)
            {
                Rectangle bounds = layout.Bounds;
                AreaStates state = m_states.GetAreaState(layout);

                RenderBackgroundRect(state, bounds);
            }

            private void RenderBackgroundRect(AreaStates state, Rectangle bounds)
            {
                if (bounds.Width > 0 && bounds.Height > 0)
                {
                    Office2007Colors colorTable = this.ColorTable;
                    Brush bgBrush;

                    switch (state)
                    {
                        case AreaStates.Hot:
                        case AreaStates.Active:
                            {
                                bgBrush = GetBarBackgroundBrushHot(bounds);
                                break;
                            }
                        case AreaStates.Pressed:
                            {
                                bgBrush = GetBarBackgroundBrushPressed(bounds);
                                break;
                            }
                        default:
                            {
                                bgBrush = GetBarBackgroundBrushDefault(bounds);
                                break;
                            }
                    }

                    m_g.FillRectangle(bgBrush, bounds);
                    bgBrush.Dispose();
                }
            }

            private Brush GetBarBackgroundBrushDefault(Rectangle bounds)
            {
                Office2007Colors colorTable = this.ColorTable;
                Color cl1 = colorTable.ButtonDefaultTopColor, cl2 = colorTable.ButtonDefaultBottomColor;
                Blend blend = new Blend();

                blend.Positions = new float[] { 0f, .45f, .45f, 1f };
                blend.Factors = new float[] { 0f, .5F, 1.1f, .5f }; 

                return GetBarBackgroundBrush(bounds, cl1, cl2, blend);
            }

            private Brush GetBarBackgroundBrushHot(Rectangle bounds)
            {
                Office2007Colors colorTable = this.ColorTable;
                Color cl1 = colorTable.ButtonSelectedTopColor, cl2 = colorTable.ButtonSelectedBottomColor;
                Blend blend = new Blend();

                blend.Positions = new float[] { 0.0F, 0.45F, 0.5F, 1.0F };
                blend.Factors = new float[] { 0.0F, 0.4F, 0.8F, 0.2F };

                return GetBarBackgroundBrush(bounds, cl1, cl2, blend);
            }

            private Brush GetBarBackgroundBrushPressed(Rectangle bounds)
            {
                Office2007Colors colorTable = this.ColorTable;
                Color cl1 = colorTable.ButtonPressedTopColor, cl2 = colorTable.ButtonSelectedBottomColor;
                Blend blend = new Blend();

                blend.Positions = new float[] { 0.0F, 0.50F, 0.55F, 1.0F };
                blend.Factors = new float[] { 0.0F, 0.6F, 1.0F, 0.4F };

                return GetBarBackgroundBrush(bounds, cl1, cl2, blend);
            }

            private void ResetDropDownState()
            {
                m_dropDownButton.Hot = false;
                m_dropDownButton.IsActive = false;
                m_dropDownButton.Pushed = false;
                m_dropDownButton.Enabled = true;
                m_dropDownButton.IsDroppedDown = false;
            }

            #endregion

            #region Classes

            /// <summary>
            /// Drop-down button implementation for <see cref="NavigationView"/>.
            /// </summary>
            protected class NavigationViewDropDownButton :
                DropDownButton
            {
                #region Construction

                /// <summary>
                /// Initializes a new instance of the <see cref="NavigationViewDropDownButton"/> class.
                /// </summary>
                /// <param name="navView">The <see cref="NavigationView"/> control.</param>
                public NavigationViewDropDownButton(NavigationView navView) :
                    base(navView)
                {
                }

                #endregion

                #region Properties

                /// <summary>
                /// Gets owner <see cref="NavigationView"/>.
                /// </summary>
                public NavigationView NavView
                {
                    get
                    {
                        return this.control as NavigationView;
                    }
                }

                #endregion

                #region Overrides

                /// <summary>
                /// Indicates whether owner control is active.
                /// </summary>
                /// <value>
                /// <c>true</c> if this instance is control active; otherwise, <c>false</c>.
                /// </value>
                protected override bool IsControlActive
                {
                    get
                    {
                        return false;
                    }
                }

                #endregion
            }

            #endregion
        }

        internal class VistaRender :
            RendrerBase
        {
            #region Overrides

            /// <summary>
            /// Renders control's background.
            /// </summary>
            protected override void RenderBackground()
            {
                base.RenderBackground();

                LayoutInfo image = m_layouter.GetLayout(HitTestAreas.BarImage);

                if (image != null)
                {
                    LayoutInfo dropDown = m_layouter.GetLayout(HitTestAreas.DropDownButton);
                    Rectangle rcImage = image.Bounds;
                    Rectangle rcDropDown = dropDown.Bounds;

                    m_g.FillRectangle(Brushes.White, rcImage.Left, rcDropDown.Top, rcImage.Width + 1, rcDropDown.Height);
                }
            }

            /// <summary>
            /// Renders control's borders.
            /// </summary>
            protected override void RenderBorders()
            {
                if (m_navView.ShowBorder)
                {
                    Color outerT = Color.FromArgb(228, 240, 252);
                    Color outerB = Color.FromArgb(231, 241, 252);
                    Color innerT = Color.FromArgb(90, 94, 98);
                    Color innerB = Color.FromArgb(170, 177, 186);
                    Rectangle bounds = m_layouter.BorderBounds;

                    RenderBorder(outerT, outerB, bounds);

                    bounds.Inflate(-1, -1);

                    RenderBorder(innerT, innerB, bounds);

                    RenderCornerRect(bounds.Left, bounds.Top);
                    RenderCornerRect(bounds.Right, bounds.Top);
                    RenderCornerRect(bounds.Right, bounds.Bottom);
                    RenderCornerRect(bounds.Left, bounds.Bottom);
                }
            }

            /// <summary>
            /// Renders the bars' area.
            /// </summary>
            protected override void RenderBarsArea()
            {
                LayoutInfo spaceLayout = m_layouter.GetLayout(HitTestAreas.Space);
                Rectangle rcSpace = spaceLayout.Bounds;

                if (rcSpace.Width > 0 && rcSpace.Height > 0)
                {
                    m_g.FillRectangle(Brushes.White, rcSpace);
                }

                base.RenderBarsArea();
            }

            /// <summary>
            /// Renders the editor area.
            /// </summary>
            protected override void RenderEditorArea()
            {
                LayoutInfo spaceLayout = m_layouter.GetLayout(HitTestAreas.Space);
                Rectangle rcSpace = spaceLayout.Bounds;

                if (rcSpace.Width > 0 && rcSpace.Height > 0)
                {
                    SetTextBoxBackColor(Color.White);

                    m_g.FillRectangle(Brushes.White, rcSpace);              
                }
            }

            /// <summary>
            /// Renders the bar's background.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected override void RenderBarBackground(Bar bar, BarLayoutInfo layout)
            {
                AreaStates state = m_states.GetAreaState(layout);

                RenderAreaBackground(state, layout.Bounds);
            }

            /// <summary>
            /// Renders the border within specified bounds with specified state.
            /// </summary>
            /// <param name="state">The state.</param>
            /// <param name="bounds">The bounds.</param>
            protected override void RenderBorder(AreaStates state, Rectangle bounds)
            {
                if (bounds.Width > 0 && bounds.Height > 0 && state != AreaStates.Default)
                {
                    Color border;

                    switch (state)
                    {
                        case AreaStates.Hot:
                        case AreaStates.Active:
                            {
                                border = Color.FromArgb(57, 125, 181);
                                break;
                            }
                        case AreaStates.Pressed:
                            {
                                border = Color.FromArgb(41, 97, 140);
                                break;
                            }
                        default:
                            {
                                return;
                            }
                    }

                    --bounds.Height;
                    --bounds.Width;

                    using (Pen pen = new Pen(border))
                    {
                        m_g.DrawRectangle(pen, bounds);
                    }
                }
            }

            /// <summary>
            /// Renders the bar.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected override void RenderBar(Bar bar, BarLayoutInfo layout)
            {
                base.RenderBar(bar, layout);

                AreaStates state = m_states.GetAreaState(layout);

                if (state != AreaStates.Default && state != AreaStates.Pressed)
                {
                    Rectangle bounds = layout.Bounds;
                    Rectangle dropDownBounds = layout.DropDown.Bounds;

                    bounds.Width -= dropDownBounds.Width;

                    if (m_navView.RightToLeft == RightToLeft.Yes)
                    {
                        bounds.X = dropDownBounds.Right - 1;
                    }

                    bounds.Inflate(-2, -2);

                    m_g.DrawRectangle(Pens.White, bounds);
                }
            }

            /// <summary>
            /// Renders the bar's text.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="state">The state.</param>
            /// <param name="textBounds">The text bounds.</param>
            protected override void RenderBarText(Bar bar, AreaStates state, Rectangle textBounds)
            {
                if (state == AreaStates.Pressed)
                {
                    textBounds.Offset(PressedOffset, PressedOffset);
                }

                base.RenderBarText(bar, state, textBounds);
            }

            /// <summary>
            /// Renders the bar's border.
            /// </summary>
            /// <param name="barState">State of the drop-down button.</param>
            /// <param name="drawBarBorder">if set to <c>true</c>, bar's border is drawn.</param>
            /// <param name="bounds">The bounds.</param>
            protected override void RenderBarBorder(AreaStates barState, bool drawBarBorder, Rectangle bounds)
            {
                if (barState == AreaStates.Active)
                {
                    --bounds.Height;
                    --bounds.Width;

                    using (Pen pen = new Pen(Color.FromArgb(140, 142, 140)))
                    {
                        m_g.DrawRectangle(pen, bounds);
                   }
                }
                else if (drawBarBorder || barState == AreaStates.Hot)
                {
                    base.RenderBarBorder(barState, true, bounds);
                }
            }

            /// <summary>
            /// Renders the bar's drop down button.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected override void RenderBarDropDown(Bar bar, BarLayoutInfo layout)
            {
                LayoutInfo ddLayout = layout.DropDown;
                AreaStates state = m_states.GetAreaState(layout);

                if (state != AreaStates.Hot && state != AreaStates.Pressed)
                {
                    state = m_states.GetAreaState(ddLayout);
                }

                RenderAreaBackground(state, ddLayout.Bounds);

                base.RenderBarDropDown(bar, layout);
            }

            /// <summary>
            /// Gets the bounds of bar's drop-down button's arrow.
            /// </summary>
            /// <param name="layout">The bar's layout.</param>
            /// <param name="image">The image.</param>
            /// <returns>The arrow bounds.</returns>
            protected override Rectangle GetBarDropDownArrowBounds(BarLayoutInfo layout, Image image)
            {
                Rectangle bounds = base.GetBarDropDownArrowBounds(layout, image);

                bounds.Offset(1, 1);

                return bounds;
            }

            /// <summary>
            /// Renders the border of bar's drop-down button.
            /// </summary>
            /// <param name="barState">State of the drop-down button.</param>
            /// <param name="bDrawBarBorder">if set to <c>true</c>, bar's border is drawn.</param>
            /// <param name="layout">The bar's layout.</param>
            protected override void RenderBarDropDownBorder(AreaStates barState, bool bDrawBarBorder, BarLayoutInfo layout)
            {
                base.RenderBarDropDownBorder(barState, bDrawBarBorder, layout);

                Rectangle ddBounds = layout.DropDown.Bounds;

                if (barState == AreaStates.Pressed)
                {
                    Rectangle bounds = layout.Bounds;

                    bounds.Width -= ddBounds.Width;

                    if (m_navView.RightToLeft == RightToLeft.Yes)
                    {
                        bounds.X = ddBounds.Right - 1;
                    }

                    RenderInnerBorder(bounds);
                    RenderInnerBorder(ddBounds);
                }
                else if (barState != AreaStates.Default)
                {
                    ddBounds.Inflate(-2, -2);
                    m_g.DrawRectangle(Pens.White, ddBounds);
                }
            }

            /// <summary>
            /// Renders the drop-down button.
            /// </summary>
            /// <param name="layout">The drop-down button's layout.</param>
            protected override void RenderDropDown(LayoutInfo layout)
            {
                Rectangle bounds = layout.Bounds;
                AreaStates state = m_states.GetAreaState(layout);

                RenderAreaBackground(state, bounds);
                RenderBorder(state, bounds);

                if (state == AreaStates.Pressed)
                {
                    RenderInnerBorder(bounds);
                }

                Image image = Resources.arrowD;

                bounds = new Rectangle(bounds.Left + (bounds.Width - image.Width) / 2, bounds.Top + (bounds.Height - image.Height) / 2, image.Width, image.Height);
                
                if (state == AreaStates.Pressed)
                {
                    bounds.Offset(1, 1);
                }

                ImageAttributes ia = new ImageAttributes();
                ColorMap map = new ColorMap();
                Rectangle newBounds = bounds;

                map.OldColor = Color.Black;
                map.NewColor = Color.White;
                ia.SetRemapTable(new ColorMap[] { map }, ColorAdjustType.Bitmap);
                newBounds.Inflate(1, 1);

                using (Bitmap bmp = new Bitmap(image))
                {
                    m_g.DrawImage(bmp, newBounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
                }

                m_g.DrawImage(image, bounds);
            }

            #endregion

            #region Implementation

            private void RenderCornerRect(int x, int y)
            {
                Rectangle corner = new Rectangle(x - 1, y - 1, 3, 3);

                using (LinearGradientBrush lgb = new LinearGradientBrush(corner, Color.FromArgb(31, Color.White), Color.FromArgb(63, Color.White), LinearGradientMode.ForwardDiagonal))                   
                {
                    lgb.WrapMode = WrapMode.TileFlipXY;
                    m_g.FillRectangle(lgb, corner);
                }
            }

            private void RenderBorder(Color topColor, Color bottomColor, Rectangle bounds)
            {
                using (Brush side = new LinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical))
                using (Pen t = new Pen(topColor))
                using (Pen b = new Pen(bottomColor))
                using (Pen r = new Pen(side))
                using (Pen l = new Pen(side))
                {
                    m_g.DrawLine(t, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                    m_g.DrawLine(r, bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
                    m_g.DrawLine(l, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                    m_g.DrawLine(b, bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
                }
            }

            private void RenderAreaBackground(AreaStates state, Rectangle bounds)
            {
                Brush brush;

                switch (state)
                {
                    case AreaStates.Hot:
                        brush = GetActiveBackgroundBrush(bounds, Color.White, Color.FromArgb(165, 219, 247));
                        break;

                    case AreaStates.Active:
                        brush = GetActiveBackgroundBrush(bounds, Color.FromArgb(247, 243, 247), Color.FromArgb(206, 207, 206));
                        break;

                    case AreaStates.Pressed:
                        brush = GetPressedBackgroundBrush(bounds, Color.FromArgb(198, 231, 247), Color.FromArgb(148, 207, 239));
                        break;

                    default:
                        brush = (Brush)Brushes.White.Clone();
                        break;
                }

                m_g.FillRectangle(brush, bounds);
                brush.Dispose();
            }

            private static Brush GetActiveBackgroundBrush(Rectangle bounds, Color cl1, Color cl2)
            {
                Blend blend = new Blend();
                blend.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
                blend.Factors = new float[] { 0.0F, 0.5F, 0.7F, 1.0F };

                return GetBarBackgroundBrush(bounds, cl1, cl2, blend);
            }

            private static Brush GetPressedBackgroundBrush(Rectangle bounds, Color cl1, Color cl2)
            {
                Blend blend = new Blend();
                blend.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
                blend.Factors = new float[] { 0.0F, 0.0F, 1.0F, 1.0F };

                return GetBarBackgroundBrush(bounds, cl1, cl2, blend);
            }

            private void RenderInnerBorder(Rectangle bounds)
            {
                Color innerBorder = Color.FromArgb(107, 142, 156);

                bounds.Inflate(-1, -1);
                --bounds.Height;

                using (Pen pen = new Pen(innerBorder))
                {
                    m_g.DrawLine(pen, bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
                    m_g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                }
            }

            #endregion
        }
        internal class MetroRender :
           RendrerBase
        {
            public enum MetroImage
            { 
                MetroBackEnable,
                MetroForEnable , 
                MetroBackDisable,
                MetroForDisable,
                MetroDropdownDisable,
                MetroDropdownEnable,
                NextPressed,
                PreviousPressed,
                DropdownPressed
            }

            #region Overrides

            /// <summary>
            /// Renders control's background.
            /// </summary>
       
            protected override void RenderHistoryDropDownButton(LayoutInfo layout)
            {
                string state = "Default";
                base.RenderHistoryDropDownButton(layout);
                switch (m_states.GetAreaState(layout))
                {
                    case AreaStates.Hot:
                        state = Convert.ToString(MetroImage.MetroDropdownEnable);
                        break;

                    case AreaStates.Disabled:
                        state = Convert.ToString(MetroImage.MetroDropdownDisable);
                        break;

                    case AreaStates.Pressed:
                        state = Convert.ToString(MetroImage.DropdownPressed);
                        break;
                }

                RenderHistoryButtonsImage(state, layout.Bounds);
            }
          
       
            protected override void RenderHistoryForwardButton(LayoutInfo layout)
            {
                string state = "ForDefault";
                  base.RenderHistoryForwardButton(layout);
                  switch (m_states.GetAreaState(layout))
                  {
                      case AreaStates.Hot:
                          state = Convert.ToString(MetroImage.MetroForEnable);
                          break;

                      case AreaStates.Disabled:
                          state = Convert.ToString(MetroImage.MetroForDisable);
                          break;

                      case AreaStates.Pressed:
                          state = Convert.ToString(MetroImage.NextPressed);
                          break;
                  }

                  MetroRenderHistoryButtonImage(layout.Bounds, state);
            }

            protected override void RenderHistoryBackButton(LayoutInfo layout)
            {
                string state = "BackDefault";
                base.RenderHistoryBackButton(layout);
                switch (m_states.GetAreaState(layout))
                {
                    case AreaStates.Hot:
                        state = Convert.ToString(MetroImage.MetroBackEnable);
                        break;

                    case AreaStates.Disabled:
                        state = Convert.ToString(MetroImage.MetroBackDisable);
                        break;

                    case AreaStates.Pressed:
                        state = Convert.ToString(MetroImage.PreviousPressed);
                        break;
                }

                MetroRenderHistoryButtonImage(layout.Bounds, state);
            }
            protected override void RenderBackground()
            {
                base.RenderBackground();
                
                using (Brush brush = new SolidBrush(Color.White))
                {
                    m_g.FillRectangle(brush, m_navView.ClientRectangle);
                }

            }
         
            /// <summary>
            /// Renders control's borders.
            /// </summary>
        

            /// <summary>
            /// Renders the bars' area.
            /// </summary>
           
            protected override void RenderBarsArea()
            {
                LayoutInfo spaceLayout = m_layouter.GetLayout(HitTestAreas.Space);
                Rectangle rcSpace = spaceLayout.Bounds;

                if (rcSpace.Width > 0 && rcSpace.Height > 0)
                {
                  m_g.FillRectangle(Brushes.White , rcSpace);
                }
                Pen pen = new Pen(Color.FromArgb(109,110,113));
                --rcSpace.Height;
                m_g.DrawRectangle(pen, rcSpace);
                pen.Dispose();
                base.RenderBarsArea();
            }

            /// <summary>
            /// Renders the editor area.
            /// </summary>
            protected override void RenderEditorArea()
            {
                LayoutInfo spaceLayout = m_layouter.GetLayout(HitTestAreas.Space);
                Rectangle rcSpace = spaceLayout.Bounds;

                if (rcSpace.Width > 0 && rcSpace.Height > 0)
                {
                    SetTextBoxBackColor(Color.White);
                    m_g.FillRectangle(Brushes.White, rcSpace);
                }

                Pen pen = new Pen(Color.FromArgb(109, 110, 113));
                --rcSpace.Height;
                m_g.DrawRectangle(pen, rcSpace);
                pen.Dispose();
            }

            /// <summary>
            /// Renders the bar's background.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected override void RenderBarBackground(Bar bar, BarLayoutInfo layout)
            {
                AreaStates state = m_states.GetAreaState(layout);

                RenderAreaBackground(state, layout.Bounds);
            }

            /// <summary>
            /// Renders the border within specified bounds with specified state.
            /// </summary>
            /// <param name="state">The state.</param>
            /// <param name="bounds">The bounds.</param>
            protected override void RenderBorder(AreaStates state, Rectangle bounds)
            {
                if (bounds.Width > 0 && bounds.Height > 0 && state != AreaStates.Default)
                {
                    Color border;

                    switch (state)
                    {
                        case AreaStates.Hot:
                        case AreaStates.Active:
                            {
                                border = Color.FromArgb(57, 125, 181);
                                break;
                            }
                        case AreaStates.Pressed:
                            {
                                border = Color.FromArgb(41, 97, 140);
                                break;
                            }
                        default:
                            {
                                return;
                            }
                    }

                    --bounds.Height;
                    --bounds.Width;

                    using (Pen pen = new Pen(border))
                    {
                        m_g.DrawRectangle(pen, bounds);
                    }
                }
            }

            /// <summary>
            /// Renders the bar.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected override void RenderBar(Bar bar, BarLayoutInfo layout)
            {
                base.RenderBar(bar, layout);

                AreaStates state = m_states.GetAreaState(layout);

                if (state != AreaStates.Default && state != AreaStates.Pressed)
                {
                    Rectangle bounds = layout.Bounds;
                    Rectangle dropDownBounds = layout.DropDown.Bounds;

                    bounds.Width -= dropDownBounds.Width;

                    if (m_navView.RightToLeft == RightToLeft.Yes)
                    {
                        bounds.X = dropDownBounds.Right - 1;
                    }

                    bounds.Inflate(-2, -2);
                }
            }

            /// <summary>
            /// Renders the bar's text.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="state">The state.</param>
            /// <param name="textBounds">The text bounds.</param>
            protected override void RenderBarText(Bar bar, AreaStates state, Rectangle textBounds)
            {
                if (state == AreaStates.Pressed)
                {
                    textBounds.Offset(PressedOffset, PressedOffset);
                }

                base.RenderBarText(bar, state, textBounds);
            }

            /// <summary>
            /// Renders the bar's border.
            /// </summary>
            /// <param name="barState">State of the drop-down button.</param>
            /// <param name="drawBarBorder">if set to <c>true</c>, bar's border is drawn.</param>
            /// <param name="bounds">The bounds.</param>
            protected override void RenderBarBorder(AreaStates barState, bool drawBarBorder, Rectangle bounds)
            {
                if (barState == AreaStates.Active)
                {
                    --bounds.Height;
                    --bounds.Width;

                    using (Pen pen = new Pen(Color.FromArgb(140, 142, 140)))
                    {
                        m_g.DrawRectangle(pen, bounds);
                    }
                }
                else if (drawBarBorder || barState == AreaStates.Hot)
                {
                    base.RenderBarBorder(barState, true, bounds);
                }
            }

            /// <summary>
            /// Renders the bar's drop down button.
            /// </summary>
            /// <param name="bar">The bar to be rendered.</param>
            /// <param name="layout">The bar's layout info.</param>
            protected override void RenderBarDropDown(Bar bar, BarLayoutInfo layout)
            {
                LayoutInfo ddLayout = layout.DropDown;
                AreaStates state = m_states.GetAreaState(layout);
              

                if (state != AreaStates.Hot && state != AreaStates.Pressed)
                {
                    state = m_states.GetAreaState(ddLayout);
                }

                RenderAreaBackground(state, ddLayout.Bounds);

                base.RenderBarDropDown(bar, layout);
                
            }

            /// <summary>
            /// Gets the bounds of bar's drop-down button's arrow.
            /// </summary>
            /// <param name="layout">The bar's layout.</param>
            /// <param name="image">The image.</param>
            /// <returns>The arrow bounds.</returns>
            protected override Rectangle GetBarDropDownArrowBounds(BarLayoutInfo layout, Image image)
            {
                Rectangle bounds = base.GetBarDropDownArrowBounds(layout, image);

                bounds.Offset(1, 1);

                return bounds;
            }

            /// <summary>
            /// Renders the border of bar's drop-down button.
            /// </summary>
            /// <param name="barState">State of the drop-down button.</param>
            /// <param name="bDrawBarBorder">if set to <c>true</c>, bar's border is drawn.</param>
            /// <param name="layout">The bar's layout.</param>
            protected override void RenderBarDropDownBorder(AreaStates barState, bool bDrawBarBorder, BarLayoutInfo layout)
            {
                base.RenderBarDropDownBorder(barState, bDrawBarBorder, layout);

                Rectangle ddBounds = layout.DropDown.Bounds;

                if (barState == AreaStates.Pressed)
                {
                    Rectangle bounds = layout.Bounds;

                    bounds.Width -= ddBounds.Width;

                    if (m_navView.RightToLeft == RightToLeft.Yes)
                    {
                        bounds.X = ddBounds.Right - 1;
                    }
                }
                else if (barState != AreaStates.Default)
                {
                    ddBounds.Inflate(-2, -2);
                    m_g.DrawRectangle(Pens.White, ddBounds);
                }
            }

            /// <summary>
            /// Renders the drop-down button.
            /// </summary>
            /// <param name="layout">The drop-down button's layout.</param>
            protected override void RenderDropDown(LayoutInfo layout)
            {
                Rectangle bounds = layout.Bounds;
                Rectangle ddBounds = layout.Bounds;
                AreaStates state = m_states.GetAreaState(layout);
                RenderAreaBackground(state, bounds);
                RenderBorder(state, bounds);
                Image image = Resources.arrowD;
                bounds = new Rectangle(bounds.Left + (bounds.Width - image.Width) / 2, bounds.Top + (bounds.Height - image.Height) / 2, image.Width, image.Height);
                ImageAttributes ia = new ImageAttributes();
                ColorMap map = new ColorMap();
                Rectangle newBounds = bounds;
                map.OldColor = Color.Black;
                map.NewColor = Color.White;
                ia.SetRemapTable(new ColorMap[] { map }, ColorAdjustType.Bitmap);
                newBounds.Inflate(1, 1);
                if(state != AreaStates.Pressed)
                m_g.DrawImage(image, bounds);
                else if(state == AreaStates.Pressed)
                {
                    string State = "PressedArrowD";
                     drawMetroPressedArrowImage(State, bounds);
                    }
                ddBounds.Width = ddBounds.Width - 1;
                ddBounds.Height--;
                m_g.DrawRectangle(Pens.Gray, ddBounds);
               
            }
            private void drawMetroPressedArrowImage(String state , Rectangle bounds)
            {
                string path = "Syncfusion.Windows.Forms.Tools.Controls.NavigationView.MetroImages." + state + ".png";
                Bitmap bmp = new Bitmap(typeof(Clock).Assembly.GetManifestResourceStream(path));
                if (bmp != null)
                    m_g.DrawImage(bmp, bounds);
            }
            #endregion

            #region Implementation
		/// <summary>
		/// Renders the background area.
		/// </summary>
            private void RenderAreaBackground(AreaStates state, Rectangle bounds)
            {
                Brush brush;

                switch (state)
                {
                    case AreaStates.Hot:
                        brush = new SolidBrush(Color.White);
                        break;

                    case AreaStates.Active:
                        brush = new SolidBrush(Color.White);
                        break;

                    case AreaStates.Pressed:
                        brush = new SolidBrush(ColorTranslator.FromHtml("#119EDA"));
                        break;

                    default:
                        brush = (Brush)Brushes.White.Clone();
                        break;
                }
                m_g.FillRectangle(brush, bounds);
                brush.Dispose();
            }

            #endregion
        }
      
    }
}
