#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.Navigation.Layouting
{
    using HitTestAreaCollection = List<LayoutInfo>;

    /// <summary>
    /// Hit test areas.
    /// </summary>
    public enum HitTestAreas
    {
        /// <summary>
        /// Represents History Back Button
        /// </summary>
        HistoryBackButton = 0,

        /// <summary>
        /// Represents History Forward button
        /// </summary>
        HistoryForwardButton,

        /// <summary>
        /// Represents History Dropdown button
        /// </summary>
        HistoryDropDownButton,

        /// <summary>
        /// Represents BarImage
        /// </summary>
        BarImage,

        /// <summary>
        /// Represents textbox
        /// </summary>
        TextBox,

        /// <summary>
        /// Represents area between last bar and drop-down button.
        /// </summary>
        Space,

        /// <summary>
        /// Represents Drop down button
        /// </summary>
        DropDownButton,

        /// <summary>
        /// Represents custom buttons
        /// </summary>
        CustomButtons,

        /// <summary>
        /// Represents Bar
        /// </summary>
        Bar,

        /// <summary>
        /// Represents MaxNonBarArea
        /// </summary>
        MaxNonBarArea = Bar,

        /// <summary>
        /// Represents Bar Text
        /// </summary>
        BarText,

        /// <summary>
        /// Represents Bardrop down
        /// </summary>
        BarDropDown,

        /// <summary>
        /// Represents nothing
        /// </summary>
        Nothing
    }

    /// <summary>
    /// Area states.
    /// </summary>
    public enum AreaStates
    {
        /// <summary>
        /// Default state of the area.
        /// </summary>
        Default,

        /// <summary>
        /// Area is hot (mouse is hovered over the area).
        /// </summary>
        Hot,

        /// <summary>
        /// Area is active (mouse is hovered over the child area).
        /// </summary>
        Active,

        /// <summary>
        /// Area is pressed.
        /// </summary>
        Pressed,

        /// <summary>
        /// Area is disabled.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// Common layout information.
    /// </summary>
    public class LayoutInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutInfo"/> class.
        /// </summary>
        /// <param name="area">The <see cref="NavigationView"/>'s hit test area.</param>
        /// <param name="bounds">The bounds.</param>
        public LayoutInfo(HitTestAreas area, Rectangle bounds)
        {
            this.Area = area;
            this.Bounds = bounds;
        }

        /// <summary>
        /// Hit test area.
        /// </summary>
        public readonly HitTestAreas Area;

        /// <summary>
        /// The bounds.
        /// </summary>
        public Rectangle Bounds;
    }

    /// <summary>
    /// <see cref="Bar"/>'s layout information.
    /// </summary>
    public class BarLayoutInfo :
        LayoutInfo
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="BarLayoutInfo"/> class.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="bounds">The bounds of <see cref="Bar"/>.</param>
        /// <param name="textBounds">The text's bounds of <see cref="Bar"/>.</param>
        /// <param name="dropDownBounds">The drop-down bounds of <see cref="Bar"/>.</param>
        /// <param name="showShevron">Shevron visibility</param>
        public BarLayoutInfo(int index, Rectangle bounds, /*Rectangle imageBounds,*/ Rectangle textBounds, Rectangle dropDownBounds, bool showShevron) :
            base(HitTestAreas.Bar, bounds)
        {
            this.Index = index;

            // this.Image = new LayoutInfo( HitTestAreas.BarImage, imageBounds );
            this.Text = new LayoutInfo(HitTestAreas.BarText, textBounds);
            this.DropDown = new LayoutInfo(HitTestAreas.BarDropDown, dropDownBounds);
            this.ShowShevron = showShevron;
        }

        #endregion

        #region Readonly fields

        /// <summary>
        /// Index of bar is the collection of selected <see cref="Bar"/>s.
        /// </summary>
        public readonly int Index;
        /*
                /// <summary>
                /// The image's bounds of <see cref="Bar"/>.
                /// </summary>
                public readonly LayoutInfo Image;
        */

        /// <summary>
        /// The text's bounds of <see cref="Bar"/>.
        /// </summary>
        public readonly LayoutInfo Text;

        /// <summary>
        /// The drop-down bounds of <see cref="Bar"/>.
        /// </summary>
        public readonly LayoutInfo DropDown;

        /// <summary>
        /// Indicates whether chevron is shown instead of drop-down button.
        /// </summary>
        public readonly bool ShowShevron;

        #endregion

        #region Implementation

        /// <summary>
        /// Offsets all layout's bounds on X-axis.
        /// </summary>
        /// <param name="offset">The X-axis offset.</param>
        public void OffsetBoundsX(int offset)
        {
            if (!this.Bounds.IsEmpty)
            {
                this.Bounds.X += offset;
            }
          
            if (!this.Text.Bounds.IsEmpty)
            {
                this.Text.Bounds.X += offset;
            }
            if (!this.DropDown.Bounds.IsEmpty)
            {
                this.DropDown.Bounds.X += offset;
            }
        }

        /// <summary>
        /// Performs hits test within the <see cref="Bar"/>.
        /// </summary>
        /// <param name="pt">Tested hit point.</param>
        /// <returns>Layouting info of <see cref="Bar"/>'s hit test area.</returns>
        public LayoutInfo HitTest(Point pt)
        {
            LayoutInfo hit = this;
            LayoutInfo[] layouts = new LayoutInfo[] { /*Image,*/ Text, DropDown };

            foreach (LayoutInfo li in layouts)
            {
                if (li.Bounds.Contains(pt))
                {
                    hit = li;
                    break;
                }
            }

            return hit;
        }

        #endregion
    }

    /// <summary>
    /// Stores collection of <see cref="LayoutInfo"/> instances.
    /// </summary>
    public class LayoutInfoCollection :
        SortedList<HitTestAreas, LayoutInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutInfoCollection"/> class.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="LayoutInfoCollection"/> can contain.</param>
        public LayoutInfoCollection(int capacity) :
            base(capacity)
        {
        }
    }

    /// <summary>
    /// Stores collection of <see cref="BarLayoutInfo"/> instances.
    /// </summary>
    public class BarLayoutInfoCollection :
        List<BarLayoutInfo>
    {
    }

    /// <summary>
    /// Specifies that this object implements layouting for <see cref="NavigationView"/> control.
    /// </summary>
    public interface ILayouter :
        IDisposable
    {
        /// <summary>
        /// Measures the text used in <see cref="NavigationView"/>.
        /// </summary>
        /// <param name="navView">The <see cref="NavigationView"/> control.</param>
        /// <param name="g">Graphics to measure at.</param>
        /// <param name="text">The text to be measured.</param>
        /// <returns>Size of measured text.</returns>
        Size MeasureText(NavigationView navView, Graphics g, string text);

        /// <summary>
        /// Updates the layout of <see cref="NavigationView"/> control according to its state.
        /// </summary>
        /// <param name="navView">The <see cref="NavigationView"/> control.</param>
        void UpdateLayout(NavigationView navView);

        /// <summary>
        /// Performs hits test.
        /// </summary>
        /// <param name="point">Tested hit point.</param>
        /// <returns>Layouting info of hit test area.</returns>
        LayoutInfo HitTest(Point point);

        /// <summary>
        /// Gets the Layouting information for the specifies area of <see cref="NavigationView"/> control.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns>Layouting information of the area.</returns>
        LayoutInfo GetLayout(HitTestAreas area);

        /// <summary>
        /// Gets the layout information for <see cref="Bar"/>s.
        /// </summary>
        BarLayoutInfoCollection BarLayoutInfos { get; }

        /// <summary>
        /// Gets the string format used in layouting.
        /// </summary>
        TextFormatFlags TextFormat { get; }

        /// <summary>
        /// Gets the border's bounds.
        /// </summary>
        Rectangle BorderBounds { get; }

        /// <summary>
        /// Gets <see cref="Bar"/> of current layout.
        /// </summary>
        Bar RootLayoutBar { get; }
    }

    /// <summary>
    /// Specifies that this object implements cache areas' state information.
    /// </summary>
    public interface IAreasStateCache
    {
        /// <summary>
        /// Gets the state of the hit test area.
        /// </summary>
        /// <param name="info">Area's layout information.</param>
        /// <returns>Area's state</returns>
        AreaStates GetAreaState(LayoutInfo info);

        /// <summary>
        /// Sets the state of the hit test area.
        /// </summary>
        /// <param name="info">Area's layout information.</param>
        /// <param name="areaState">Area's state.</param>
        void SetAreaState(LayoutInfo info, AreaStates areaState);

        /// <summary>
        /// Clears all cached area states.
        /// </summary>
        /// <returns>Collection of cleared layouts.</returns>
        List<LayoutInfo> ClearAreaStates();
    }

    /// <summary>
    /// Base layouter for <see cref="NavigationView"/> control.
    /// </summary>
    public class LayouterBase :
        ILayouter,
        IAreasStateCache,
        IDisposable
    {
        #region Constants

        private const TextFormatFlags c_textFormat = TextFormatFlags.Left | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter
            | TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform;

        #endregion

        #region Fields

        private LayoutInfoCollection _layouts = new LayoutInfoCollection((int)HitTestAreas.MaxNonBarArea);
        private HitTestAreaCollection _hitTestAreas = new HitTestAreaCollection();
        private BarLayoutInfoCollection _barsLayout;
        private AreaStateCollection _areasState = new AreaStateCollection();
        private Rectangle m_border;
        private Bar _rootBar;

        #endregion

        #region Constants

        public const int DropDownButtonWidth = 16;
        public const int BarHorzPadding = 5;
        public const int BarDropDownButtonWidth = 16;
        public const int HistoryButtonHeight = 28;
        public const int HistoryBackButtonWidth = 28;
        public const int HistoryForwardButtonWidth = 26;
        public const int HistoryDropDownButtonWidth = 15;
        public const int TextBoxVerticalPadding = 4;
        public const int TextBoxHorizontalPadding = 4;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="LayouterBase"/> class.
        /// </summary>
        public LayouterBase()
        {
        }

        #endregion

        #region ILayouter implementation

        /// <summary>
        /// Measures the text used in <see cref="NavigationView"/>.
        /// </summary>
        /// <param name="navView">The <see cref="NavigationView"/> control.</param>
        /// <param name="g">Graphics to measure at.</param>
        /// <param name="text">The text to be measured.</param>
        /// <returns>Size of measured text.</returns>
        public virtual Size MeasureText(NavigationView navView, Graphics g, string text)
        {
            TextFormatFlags textFormat = c_textFormat;

            if (navView.RightToLeft != RightToLeft.Yes)
            {
                textFormat &= ~TextFormatFlags.RightToLeft;
            }
            else
            {
                textFormat |= TextFormatFlags.RightToLeft;
            }

            Size size = TextRenderer.MeasureText(g, text, navView.Font, new Size(int.MaxValue, int.MaxValue), textFormat);

            return size;
        }

        /// <summary>
        /// Updates the layout of <see cref="NavigationView"/> control according to its state.
        /// </summary>
        /// <param name="navView">The <see cref="NavigationView"/> control.</param>
        public virtual void UpdateLayout(NavigationView navView)
        {
            using (Graphics g = navView.CreateGraphics())
            {
                Rectangle client = navView.ClientRectangle;

                SetupLayouter(navView);

                LayoutHistoryButtons(navView, ref client);
                LayoutBarImage(navView, ref client);
                LayoutCustomButtons(navView, ref client);
                LayoutDropDownButton(navView, ref client);

                this.BarLayoutInfos.Clear();

                if (navView.AllowEditMode && navView.InEditingMode)
                {
                    LayoutEditArea(navView);
                }
                else
                {
                    LayoutBars(navView, g, client);
                }

                if (navView.ShowBorder)
                {
                    LayoutBorder(navView);
                }

                if (navView.RightToLeft == RightToLeft.Yes)
                {
                    UpdateLayoutForRTL(navView);
                }
            }
        }

        /// <summary>
        /// Gets the height of the text box.
        /// </summary>
        /// <param name="navView">The navigation view.</param>
        /// <returns>Returns TextBox Height</returns>
        protected virtual int GetTextBoxHeight(NavigationView navView)
        {
            return navView.Font.Height;
        }

        /// <summary>
        /// Gets the height of the space area.
        /// </summary>
        /// <param name="navView">The navigation view.</param>
        /// <returns>Returns integer value</returns>
        protected virtual int GetSpaceAreaHeight(NavigationView navView)
        {
            return GetTextBoxHeight(navView) + 2 * TextBoxVerticalPadding;
        }

        private void LayoutEditArea(NavigationView navView)
        {
            LayoutSpaceArea(navView, null);

            LayoutInfo spaceLayout = GetLayout(HitTestAreas.Space);
            Rectangle textBoxBounds = spaceLayout.Bounds;
            Rectangle client = navView.ClientRectangle;

            textBoxBounds.Inflate(-TextBoxHorizontalPadding, 0);
            textBoxBounds.Height = GetTextBoxHeight(navView);
            textBoxBounds.Y = client.Top + (client.Height - textBoxBounds.Height) / 2;

            LayoutInfo textBoxLayout = new LayoutInfo(HitTestAreas.TextBox, textBoxBounds);

            _layouts[HitTestAreas.TextBox] = textBoxLayout;
            _hitTestAreas.Add(textBoxLayout);
        }

        /// <summary>
        /// Performs hits test.
        /// </summary>
        /// <param name="point">Tested hit point.</param>
        /// <returns>Layouting info of hit test area.</returns>
        public virtual LayoutInfo HitTest(Point point)
        {
            LayoutInfo found = _hitTestAreas.Find(
                delegate(LayoutInfo li)
                {
                    return li.Bounds.Contains(point);
                });

            return found;
        }

        /// <summary>
        /// Gets the Layouting information for the specifies area of <see cref="NavigationView"/> control.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns>Layouting information of the area.</returns>
        public LayoutInfo GetLayout(HitTestAreas area)
        {
            LayoutInfo layout = null;

            if (_layouts.ContainsKey(area))
            {
                layout = _layouts[area];
            }

            return layout;
        }

        /// <summary>
        /// Gets the layout information for <see cref="Bar"/>s.
        /// </summary>
        public BarLayoutInfoCollection BarLayoutInfos
        {
            get
            {
                if (_barsLayout == null)
                {
                    _barsLayout = new BarLayoutInfoCollection();
                }

                return _barsLayout;
            }
        }

        /// <summary>
        /// Gets the string format used in layouting.
        /// </summary>
        public TextFormatFlags TextFormat
        {
            get
            {
                return c_textFormat;
            }
        }

        /// <summary>
        /// Gets the border's bounds.
        /// </summary>
        /// <value></value>
        public virtual Rectangle BorderBounds
        {
            get
            {
                return m_border;
            }
        }

        /// <summary>
        /// Gets <see cref="Bar"/> of current layout.
        /// </summary>
        /// <value></value>
        public virtual Bar RootLayoutBar
        {
            get
            {
                return _rootBar;
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Implementation

        /// <summary>
        /// Gets the bounds for the specifies area of <see cref="NavigationView"/> control.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="bounds">The area's bounds.</param>
        protected void SetBounds(HitTestAreas area, Rectangle bounds)
        {
            if ((int)area < (int)HitTestAreas.MaxNonBarArea)
            {
                if (!bounds.IsEmpty)
                {
                    LayoutInfo layout;

                    if (_layouts.ContainsKey(area))
                    {
                        layout = _layouts[area];
                        layout.Bounds = bounds;
                    }
                    else
                    {
                        layout = new LayoutInfo(area, bounds);
                        _layouts[area] = layout;
                    }

                    _hitTestAreas.Add(layout);
                }
                else
                {
                    _layouts.Remove(area);
                }
            }
        }

        private void LayoutHistoryButtons(NavigationView navView, ref Rectangle client)
        {
            if (navView.ShowHistoryButtons)
            {
                Size szBack = new Size(HistoryBackButtonWidth, HistoryButtonHeight);
                Size szFwd = new Size(HistoryForwardButtonWidth, HistoryButtonHeight);
                Size szDD = new Size(HistoryDropDownButtonWidth, HistoryButtonHeight);
                int top = (client.Height - HistoryButtonHeight) / 2;
                Rectangle rcBack = new Rectangle(new Point(client.Left, top), szBack);
                Rectangle rcFwd = new Rectangle(new Point(rcBack.Left + szBack.Width, top), szFwd);
                Rectangle rcDD = new Rectangle(new Point(rcFwd.Left + rcFwd.Width, top), szDD);

                SetBounds(HitTestAreas.HistoryBackButton, rcBack);
                SetBounds(HitTestAreas.HistoryForwardButton, rcFwd);
                SetBounds(HitTestAreas.HistoryDropDownButton, rcDD);

                int width = szBack.Width + szFwd.Width + szDD.Width;
                client.X += width;
                client.Width -= width;
            }
        }

        private void LayoutBarImage(NavigationView navView, ref Rectangle client)
        {
            Bar bar = navView.SelectedBar;

            if (bar != null)
            {
                Image image = Bar.Helper.GetBarImage(bar, navView);

                if (image == null && !bar.Enabled)
                {
                    image = Bar.Helper.GetBarImage(bar, navView, true);
                }

                if (image != null)
                {
                    Size szImage = image.Size;
                    int maxDim = client.Height - BarHorzPadding;

                    if (szImage.Height > szImage.Width)
                    {
                        if (szImage.Height > maxDim)
                        {
                            szImage.Width = szImage.Width * maxDim / szImage.Height;
                            szImage.Height = maxDim;
                        }
                    }
                    else if (szImage.Width > maxDim)
                    {
                        szImage.Height = szImage.Height * maxDim / szImage.Width;
                        szImage.Width = maxDim;
                    }

                    int top = (client.Height - szImage.Height) / 2;
                    int horzPadding = BarHorzPadding / 2;
                    Rectangle rcImage = new Rectangle(client.Left + horzPadding, top, szImage.Width, szImage.Height);

                    SetBounds(HitTestAreas.BarImage, rcImage);

                    client.X += rcImage.Width + BarHorzPadding;
                    client.Width -= rcImage.Width + BarHorzPadding;
                }
            }
        }

        private void LayoutCustomButtons(NavigationView navView, ref Rectangle client)
        {
            CustomButtonCollection buttons = navView.CustomButtons;

            if (buttons.Count > 0)
            {
                int width = 0;

                foreach (CustomButton btn in buttons)
                {
                    if (btn.Visible)
                    {
                        width += btn.Width;
                    }
                }

                Rectangle customButtons = Rectangle.Empty;

                if (width > 0)
                {
                    customButtons = new Rectangle(client.Left + client.Width - width, client.Top, width, client.Height);
                    client.Width -= width;

                    SetBounds(HitTestAreas.CustomButtons, customButtons);

                    int x = customButtons.Location.X;

                    foreach (CustomButton btn in buttons)
                    {
                        if (btn.Visible)
                        {
                            btn.Left = x;
                            btn.Top = (client.Height - btn.Height) / 2;

                            x += btn.Width;
                        }
                    }
                }
            }
        }

        private void LayoutDropDownButton(NavigationView navView, ref Rectangle client)
        {
            int height = GetSpaceAreaHeight(navView);
            Rectangle dropDownButton = new Rectangle(client.Left + client.Width - DropDownButtonWidth, client.Top + (client.Height - height) / 2, DropDownButtonWidth, height);
            client.Width -= DropDownButtonWidth;

            SetBounds(HitTestAreas.DropDownButton, dropDownButton);
        }

        private void LayoutBars(NavigationView navView, Graphics g, Rectangle client)
        {
            List<Bar> bars = navView.SelectedBars;
            BarLayoutInfoCollection layouts = this.BarLayoutInfos;
            int count = bars.Count;

            layouts.Capacity = count;

            for (int last = count - 1, i = last; i >= 0; --i)
            {
                Bar bar = bars[i];

                if (bar.Visible)
                {
                    bool bLast = i == last;
                    BarLayoutInfo layout = LayoutBar(i, bar, navView, g, bLast, ref client);
                    if (layout == null && layouts.Count > 0 && i < last)
                    {
                        int iPrevLayout = layouts.Count - 1;
                        BarLayoutInfo prevLayout = layouts[iPrevLayout];
                        int minWidth = GetBarMinWidth(bar, navView);
                       client.X += minWidth;
                        client.Width += prevLayout.Bounds.Width - minWidth;

                        int iPrevBar = i + 1;
                        Bar prevBar = bars[iPrevBar];
                        prevLayout = LayoutBar(iPrevBar, prevBar, navView, g, (iPrevBar == last), ref client);

                        if (prevLayout != null)
                        {
                            layouts[iPrevLayout] = prevLayout;
                            client.X -= minWidth;
                            client.Width += minWidth;

                            if (!prevLayout.ShowShevron)
                            {
                                layout = LayoutBar(i, bar, navView, g, bLast, ref client);
                            }
                        }
                    }

                    if (layout != null)
                    {
                        layouts.Add(layout);

                        if (layout.ShowShevron)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            layouts.TrimExcess();

            _rootBar = null;

            int offset = -client.Width;

            if (layouts.Count > 0)
            {
                BarLayoutInfo layout = layouts[layouts.Count - 1];

                _rootBar = bars[layout.Index];

                if (!navView.ShowRootBarText && !layout.ShowShevron && layouts.Count > 1)
                {
                    Rectangle bounds = layout.Bounds;
                    int textWidth = layout.Text.Bounds.Width + 2 * BarHorzPadding;

                    bounds.X += textWidth;
                    bounds.Width -= textWidth;

                    layout.Bounds = bounds;
                    layout.Text.Bounds = Rectangle.Empty;

                    offset -= textWidth;
                }
            }

            if (offset < 0)
            {
                foreach (BarLayoutInfo layout in layouts)
                {
                    layout.OffsetBoundsX(offset);
                    _hitTestAreas.Add(layout);
                }
            }

            LayoutSpaceArea(navView, layouts);
        }

        private void LayoutSpaceArea(NavigationView navView, BarLayoutInfoCollection layouts)
        {
            Rectangle spaceBounds = GetSpaceBounds(navView, layouts);
            LayoutInfo space = new LayoutInfo(HitTestAreas.Space, spaceBounds);

            _layouts[HitTestAreas.Space] = space;
            _hitTestAreas.Add(space);
        }

        private Rectangle GetSpaceBounds(NavigationView navView, BarLayoutInfoCollection layouts)
        {
            Rectangle client = navView.ClientRectangle;
            LayoutInfo histDDBtn = GetLayout(HitTestAreas.HistoryDropDownButton);
            Rectangle rcHistDDBtn = (histDDBtn != null) ? histDDBtn.Bounds : Rectangle.Empty;
            LayoutInfo dropDown = GetLayout(HitTestAreas.DropDownButton);
            Rectangle rcDropDown = dropDown.Bounds;
            int left = rcHistDDBtn.IsEmpty ? 0 : rcHistDDBtn.Right + 1;
            int right = rcDropDown.Left;

            if (layouts != null && layouts.Count > 0)
            {
                BarLayoutInfo bl = layouts[0];

                left = bl.Bounds.Right;
            }
            else
            {
                LayoutInfo barImage = GetLayout(HitTestAreas.BarImage);

                if (barImage != null)
                {
                    left += barImage.Bounds.Width + BarHorzPadding / 2;
                }
            }

            int height = GetSpaceAreaHeight(navView);
            int top = client.Top + (client.Height - height) / 2;
            Rectangle spaceBounds = new Rectangle(left, top, right - left, height);

            return spaceBounds;
        }

        private BarLayoutInfo LayoutBar(int index, Bar bar, NavigationView navView, Graphics g, bool barIsLast, ref Rectangle client)
        {
            int barMinWidth = GetBarMinWidth(bar, navView);

            if (client.Width < barMinWidth)
            {
                return null;
            }

            Size textSize = MeasureText(navView, g, bar.Text);
            int barWidth = barMinWidth + BarHorzPadding + textSize.Width + BarHorzPadding;
            bool showChevron = false;

            if (barWidth > client.Width)
            {
                int diff = barWidth - client.Width;

                textSize.Width -= diff;

                if (textSize.Width <= 0)
                {
                    textSize = Size.Empty;
                }

                barWidth -= diff;

                if (!barIsLast)
                {
                    textSize = Size.Empty;
                    barWidth = barMinWidth;
                    showChevron = true;
                }

                if (barWidth > client.Width)
                {
                    return null;
                }
            }

            int barHeight = GetSpaceAreaHeight(navView);

            Rectangle bounds = new Rectangle(client.Right - barWidth, client.Top + (client.Height - barHeight) / 2, barWidth, barHeight);
            Rectangle textRect = textSize.IsEmpty ? Rectangle.Empty :
                new Rectangle(bounds.Left + BarHorzPadding, bounds.Top, textSize.Width, bounds.Height);

            // Rectangle imageRect = ( textRect.IsEmpty && image != null ) ?
            //    new Rectangle( bounds.Left + this.BarHorzPadding, bounds.Top, image.Width, bounds.Height  ) : Rectangle.Empty;
            Rectangle dropRect = (bar.Bars.Count <= 0) ? Rectangle.Empty :
                new Rectangle(bounds.Right - BarDropDownButtonWidth, bounds.Top, BarDropDownButtonWidth, bounds.Height);
            BarLayoutInfo info = new BarLayoutInfo(index, bounds, /*imageRect,*/ textRect, dropRect, showChevron);

            client.Width -= barWidth;

            return info;
        }

        private int GetBarMinWidth(Bar bar, NavigationView navView)
        {
            int minWidth = 0;
            if (bar.Bars.Count > 0)
            {
                minWidth += BarDropDownButtonWidth;
            }

            return minWidth;
        }

        private void SetupLayouter(NavigationView navView)
        {
            _hitTestAreas.Clear();
            _hitTestAreas.Capacity = navView.SelectedBars.Count + (int)HitTestAreas.MaxNonBarArea;

            _layouts.Clear();
        }

        private void UpdateLayoutForRTL(NavigationView navView)
        {
            int clientWidth = navView.ClientRectangle.Width;

            foreach (LayoutInfo info in _layouts.Values)
            {
                LayoutBoundsRTL(clientWidth, info);
            }

            foreach (BarLayoutInfo info in _barsLayout)
            {
                LayoutBoundsRTL(clientWidth, info);
                LayoutBoundsRTL(clientWidth, info.Text);
                LayoutBoundsRTL(clientWidth, info.DropDown);
            }

            CustomButtonCollection buttons = navView.CustomButtons;

            if (buttons.Count > 0)
            {
                foreach (CustomButton btn in buttons)
                {
                    if (btn.Visible)
                    {
                        btn.Left = clientWidth - btn.Right;
                    }
                }
            }
        }

        private static void LayoutBoundsRTL(int width, LayoutInfo info)
        {
            if (info.Bounds != Rectangle.Empty)
            {
                info.Bounds.X = width - info.Bounds.Right;
            }
        }

        private void LayoutBorder(NavigationView navView)
        {
            Rectangle rcClient = navView.ClientRectangle;
            int left = 0, right = rcClient.Right;
            LayoutInfo ddBtn = GetLayout(HitTestAreas.DropDownButton);
            Rectangle ddBtnBounds = ddBtn.Bounds;

            right = ddBtnBounds.Right;

            if (navView.ShowHistoryButtons)
            {
                LayoutInfo hddBtn = GetLayout(HitTestAreas.HistoryDropDownButton);

                left = hddBtn.Bounds.Right + 1;
                m_border = new Rectangle(left, ddBtnBounds.Top - 1, m_border.Width = right - left - 1, ddBtnBounds.Height + 1);
            }
            else
            {
                m_border = new Rectangle(left, rcClient.Top, m_border.Width = right - left - 1, rcClient.Height - 1);
            }

            if (navView.RightToLeft == RightToLeft.Yes)
            {
                m_border.X = rcClient.Width - m_border.Right - 1;
            }
        }

        #endregion

        #region Classes

       public class AreaStateCollection :
            Dictionary<LayoutInfo, AreaStates>
        {
            public AreaStateCollection()
            {
            }
        }

        #endregion

        #region IAreasStateCache implementation

        /// <summary>
        /// Gets the state of the hit test area.
        /// </summary>
        /// <param name="info">Area's layout information.</param>
        /// <returns>Area's state</returns>
        public AreaStates GetAreaState(LayoutInfo info)
        {
            AreaStates state = AreaStates.Default;

            if (_areasState.ContainsKey(info))
            {
                state = _areasState[info];
            }

            return state;
        }

        /// <summary>
        /// Sets the state of the hit test area.
        /// </summary>
        /// <param name="info">Area's layout information.</param>
        /// <param name="areaState">Area's state.</param>
        public void SetAreaState(LayoutInfo info, AreaStates areaState)
        {
            _areasState[info] = areaState;
        }

        /// <summary>
        /// Clears all cached area states.
        /// </summary>
        /// <returns>Collection of cleared layouts.</returns>
        public List<LayoutInfo> ClearAreaStates()
        {
            List<LayoutInfo> areas = new List<LayoutInfo>(_areasState.Count);

            areas.AddRange(_areasState.Keys);
            _areasState.Clear();

            return areas;
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Finalizes an instance of the LayouterBase class.
        /// <see cref="LayouterBase"/> is reclaimed by garbage collection.
        /// </summary>
        ~LayouterBase()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rootBar = null;
            }
        }

        #endregion
    }
}
