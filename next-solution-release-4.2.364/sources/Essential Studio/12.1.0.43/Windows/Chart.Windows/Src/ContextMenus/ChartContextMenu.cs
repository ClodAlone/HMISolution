#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart.Utils;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Contains the all logical of context menu of the <see cref="ChartControl"/>.
    /// </summary>
    ///<internalonly/>
    [DocumentationExclude()]
    [ToolboxItem(false)]
    public class ChartContextMenu : ContextMenu
    {
        #region Members
        private ChartControl chart;
        private TagMenuItem seriesItem;
        private TagMenuItem modeItem;
        private TagMenuItem mode2DItem;
        private TagMenuItem mode3DItem;
        private TagMenuItem realMode3DItem;

        private TagMenuItem m_areaItem;
        private TagMenuItem m_zoomingItem;
        private TagMenuItem m_enableXZoomItem;
        private TagMenuItem m_enableYZoomItem;
        private TagMenuItem m_resetZoomItem;
        private TagMenuItem m_enableAutoHighlightItem;
        private TagMenuItem m_allowSetAlignmentItem;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="ChartContextMenu"/> to initalize the menu items
        /// </summary>
        public ChartContextMenu(ChartControl control) : this()
        {
            chart = control;
            InitializeMenu();
        }
        /// <summary>
        /// Initializes a new instance of <see cref="ChartContextMenu"/>
        /// </summary>
        public ChartContextMenu():base()
        {
           
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initialze the necessary members and items of <see cref="ChartContextMenu"/>.
        /// </summary>
        private void InitializeMenu()
        {
            // Series
            
            seriesItem = new TagMenuItem(this.chart.Localization.Series);

            // Set alignment
            m_allowSetAlignmentItem = new TagMenuItem(this.chart.Localization.AllowAlignment, new EventHandler(AllowSetAligment_Click));
            m_allowSetAlignmentItem.Image = ChartCommandsImages.AllowAlignment;

            // Area
            m_areaItem = new TagMenuItem(this.chart.Localization.Area);
            m_zoomingItem = new TagMenuItem(this.chart.Localization.Zooming);
            m_enableXZoomItem = new TagMenuItem(this.chart.Localization.EnableXZooming, new EventHandler(EnableXZoomItem_Click));
            m_enableYZoomItem = new TagMenuItem(this.chart.Localization.EnableYZooming, new EventHandler(EnableYZoomItem_Click));
            m_resetZoomItem = new TagMenuItem(this.chart.Localization.ResetZoom, new EventHandler(ResetZoomItem_Click));
            m_enableAutoHighlightItem = new TagMenuItem(this.chart.Localization.AutoHighlight, new EventHandler(AutoHighlightItem_Click));

            m_zoomingItem.Image = ChartCommandsImages.Zooming;
            m_enableXZoomItem.Image = ChartCommandsImages.XZooming;
            m_enableYZoomItem.Image = ChartCommandsImages.YZooming;
            m_resetZoomItem.Image = ChartCommandsImages.ResetZooming;
            m_enableAutoHighlightItem.Image = ChartCommandsImages.AutoHighlight;

            m_areaItem.MenuItems.Add(m_zoomingItem);
            m_areaItem.MenuItems.Add(CreateSeparator());
            m_areaItem.MenuItems.Add(m_enableAutoHighlightItem);

            m_zoomingItem.MenuItems.Add(m_enableXZoomItem);
            m_zoomingItem.MenuItems.Add(m_enableYZoomItem);
            m_zoomingItem.MenuItems.Add(m_resetZoomItem);

            // Mode
            modeItem = new TagMenuItem(this.chart.Localization.Mode);
            mode2DItem = new TagMenuItem(this.chart.Localization.TwoD, new EventHandler(Mode2D_Click));
            mode3DItem = new TagMenuItem(this.chart.Localization.ThreeD, new EventHandler(Mode3D_Click));
            realMode3DItem = new TagMenuItem(this.chart.Localization.Real3D, new EventHandler(ModeReal3D_Click));

            mode2DItem.Image = ChartCommandsImages.Series2D;
            mode3DItem.Image = ChartCommandsImages.Series3D;

            modeItem.MenuItems.Add(mode2DItem);
            modeItem.MenuItems.Add(mode3DItem);
            // modeItem.MenuItems.Add(realMode3DItem);

            // Ser items
            this.MenuItems.Add(m_areaItem);
            this.MenuItems.Add(seriesItem);
            this.MenuItems.Add(modeItem);
            this.MenuItems.Add(CreateSeparator());
            this.MenuItems.Add(m_allowSetAlignmentItem);
        }

        /// <summary>
        /// Prepares the items before showing.
        /// </summary>
        private void PrepareChart()
        {
            m_enableXZoomItem.Checked = chart.EnableXZooming;
            m_enableYZoomItem.Checked = chart.EnableYZooming;
            m_enableAutoHighlightItem.Checked = chart.AutoHighlight;
            m_allowSetAlignmentItem.Checked = chart.DockingManager.DockAlignment;

            int sercount = chart.Series.Count;

            seriesItem.MenuItems.Clear();

            foreach (ChartSeries series in chart.Series)
            {
                seriesItem.MenuItems.Add(this.CreateSeriesMenuItem(series));
            }

            TagMenuItem seriesPalettes = new TagMenuItem(this.chart.Localization.Palettes);

            seriesPalettes.Image = ChartCommandsImages.Palettes;

            foreach (ChartColorPalette palette in Enum.GetValues(typeof(ChartColorPalette)))
            {
                TagMenuItem palItem = new TagMenuItem(palette, new EventHandler(Palettes_Click));

                palItem.Checked = (chart.Palette == palette);
                palItem.Image = chart.Model.ColorModel.CreatePaletteIcon(new Size(16, 16), palette, 6);

                seriesPalettes.MenuItems.Add(palItem);
            }

            seriesItem.MenuItems.Add(this.CreateSeparator());
            seriesItem.MenuItems.Add(seriesPalettes);

            this.MenuItems.Clear();
            this.MenuItems.Add(seriesItem);
            this.MenuItems.Add(modeItem);
            this.MenuItems.Add(m_areaItem);
            this.MenuItems.Add(CreateSeparator());
            this.MenuItems.Add(m_allowSetAlignmentItem);

            // chart modes
            if (!chart.Series3D)
            {
                mode2DItem.Checked = true;
                mode3DItem.Checked = false;
                realMode3DItem.Checked = false;
            }
            else
            {
                if (chart.RealMode3D)
                {
                    mode2DItem.Checked = false;
                    mode3DItem.Checked = false;
                    realMode3DItem.Checked = true;
                }
                else
                {
                    mode2DItem.Checked = false;
                    mode3DItem.Checked = true;
                    realMode3DItem.Checked = false;
                }
            }
        }

        #region Create items methods
        /// <summary>
        /// Creates the series menu item.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Returns TagMenuItem.</returns>
        private TagMenuItem CreateSeriesMenuItem(ChartSeries series)
        {
            TagMenuItem seriesItem = new TagMenuItem(series.Name);            
            TagMenuItem editStyleItem = new TagMenuItem(this.chart.Localization.EditStyle, series, new EventHandler(OnEditStyleClick));

            seriesItem.Tag = series;
            editStyleItem.Image = ChartCommandsImages.Style;

            seriesItem.Image = new Bitmap(16, 16);
            seriesItem.MenuItems.Add(this.CreateSeriesTypesItem(series));
            seriesItem.MenuItems.Add(this.CreateSeparator());
            seriesItem.MenuItems.Add(editStyleItem);

            using (Graphics g = Graphics.FromImage(seriesItem.Image))
            {
                BrushPaint.FillRectangle(g, new Rectangle(0, 0, 16, 16), series.GetOfflineStyle().Interior);
                g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
            }

            return seriesItem;
        }

        /// <summary>
        /// Creates the series types item.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Returns TagMenuItem.</returns>
        private TagMenuItem CreateSeriesTypesItem(ChartSeries series)
        {
            TagMenuItem itemTypes = new TagMenuItem(this.chart.Localization.Types);

            itemTypes.Image = ChartCommandsImages.SeriesType;
            itemTypes.Tag = series;

            foreach (ChartSeriesType type in Enum.GetValues(typeof(ChartSeriesType)))
            {
                TagMenuItem typeItem = new TagMenuItem(type, new EventHandler(OnSeriesTypeClick));

                typeItem.Image = ChartSeriesTypeImages.GetImage(type);
                typeItem.Checked = series.Type == type;

                itemTypes.MenuItems.Add(typeItem);
            }

            return itemTypes;
        }
        #endregion

        #region Events handlers
        /// <summary>
        /// Called when the Type item of the Series item is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>    
        private void OnSeriesTypeClick(object sender, System.EventArgs e)
        {
            TagMenuItem typeItem = sender as TagMenuItem;
            ChartSeries series = (typeItem.Parent as TagMenuItem).Tag as ChartSeries;

            if (typeItem != null)
            {
                series.Type = (ChartSeriesType)typeItem.Tag;
            }
        }

        /// <summary>
        /// Called when the Edit style item of the Series item item is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnEditStyleClick(object sender, System.EventArgs e)
        {
            TagMenuItem typeItem = sender as TagMenuItem;
            ChartSeries series = typeItem.Tag as ChartSeries;

            if (series != null)
            {
                chart.DisplayUserEditStylesDialog(chart.Series.IndexOf(series));
            }
        }
        #endregion

        /// <summary>
        ///  Displays the shortcut menu at the specified position.
        /// </summary>
        /// <param name="control">A <see cref="Control"/> that specifies the control with which this shortcut menu is associated</param>
        /// <param name="pos">A <see cref="Point"/> that specifies the coordinates at which to display the menu.</param>
        /// <param name="ch">Instance of the class <see cref="ChartControl"/>.</param>
        public void Show(Control control, Point pos, ChartControl ch)
        {
            chart = ch;
            PrepareChart();
            this.Show(control, pos);            
        }

        /// <summary>
        /// Handles the Click event of the Palettes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Palettes_Click(object sender, System.EventArgs e)
        {
            TagMenuItem typeItem = sender as TagMenuItem;
            if (typeItem != null)
            {
                chart.Palette = (ChartColorPalette)typeItem.Tag;
            }
        }

        /// <summary>
        /// Handles the Click event of the Mode2D control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Mode2D_Click(object sender, System.EventArgs e)
        {
            chart.Series3D = false;
        }

        /// <summary>
        /// Handles the Click event of the Mode3D control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Mode3D_Click(object sender, System.EventArgs e)
        {
            chart.Series3D = true;
            chart.RealMode3D = false;
        }

        /// <summary>
        /// Handles the Click event of the ModeReal3D control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ModeReal3D_Click(object sender, System.EventArgs e)
        {
            chart.Series3D = true;
            chart.RealMode3D = true;
        }

        /// <summary>
        /// Handles the Click event of the EnableXZoomItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EnableXZoomItem_Click(object sender, System.EventArgs e)
        {
            chart.EnableXZooming = !chart.EnableXZooming;
        }

        /// <summary>
        /// Handles the Click event of the EnableYZoomItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EnableYZoomItem_Click(object sender, System.EventArgs e)
        {
            chart.EnableYZooming = !chart.EnableYZooming;
        }

        /// <summary>
        /// Called when the Reset Zoom item is clicked.
        /// </summary>
        /// <param name="sender">A sender of the event.</param>
        /// <param name="e">Argument.</param>
        private void ResetZoomItem_Click(object sender, System.EventArgs e)
        {
            chart.ZoomFactorX = 1f;
            chart.ZoomFactorY = 1f;
            chart.ZoomPositionX = 0;
            chart.ZoomPositionY = 0;
        }

        /// <summary>
        /// Called when the Auto Highlight item is clicked.
        /// </summary>
        /// <param name="sender">A sender of the event.</param>
        /// <param name="e">Argument.</param>
        private void AutoHighlightItem_Click(object sender, System.EventArgs e)
        {
            chart.AutoHighlight = !chart.AutoHighlight;
        }

        /// <summary>
        /// Called when the Allow Aligment item is clicked.
        /// </summary>
        /// <param name="sender">A sender of the event.</param>
        /// <param name="e">Argument.</param>
        private void AllowSetAligment_Click(object sender, System.EventArgs e)
        {
            chart.DockingManager.DockAlignment = !chart.DockingManager.DockAlignment;
        }

        /// <summary>
        /// Returns the separator of the menu items.
        /// </summary>
        /// <returns>The Instance of <see cref="MenuItem"/> class.</returns>
        private MenuItem CreateSeparator()
        {
            return new MenuItem("-");
        }
        #endregion
    }

    /// <summary>
    /// This class inherits the class <see cref="MenuItem"/>.
    /// Contains additional property <see cref="TagMenuItem.Tag"/>.
    /// </summary>
    class TagMenuItem : MenuItem
    {
        #region Members
        private object m_tag;
        private Image m_image = null;
        private ImageList m_imageList;
        private int m_imageIndex;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the object that contains data about the item. 
        /// </summary>
        public new object Tag
        {
            get
            {
                return m_tag;
            }

            set
            {
                m_tag = value;
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public Image Image
        {
            get
            {
                if (m_image == null && m_imageList != null)
                {
                    if (m_imageIndex > -1 && m_imageIndex < m_imageList.Images.Count)
                    {
                        return m_imageList.Images[m_imageIndex];
                    }
                }

                return m_image;
            }

            set 
            {
                m_image = value; 
            }
        }

        /// <summary>
        /// Gets or sets the image list.
        /// </summary>
        /// <value>The image list.</value>
        public ImageList ImageList
        {
            get { return m_imageList; }

            set { m_imageList = value; }
        }

        /// <summary>
        /// Gets or sets the index of the image.
        /// </summary>
        /// <value>The index of the image.</value>
        public int ImageIndex
        {
            get { return m_imageIndex; }

            set { m_imageIndex = value; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TagMenuItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="onclick">The onclick.</param>
        public TagMenuItem(string name, object tag, EventHandler onclick)
            : base(name, onclick)
        {
            m_tag = tag;
            this.OwnerDraw = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagMenuItem"/> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="onclick">The onclick.</param>
        public TagMenuItem(object tag, EventHandler onclick)
            : base(tag.ToString(), onclick)
        {
            m_tag = tag;
            this.OwnerDraw = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagMenuItem"/> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        public TagMenuItem(object tag)
            : base(tag.ToString())
        {
            m_tag = tag;
            this.OwnerDraw = true;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.MenuItem.MeasureItem"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MeasureItemEventArgs"></see> that contains the event data.</param>
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            SizeF textSize = e.Graphics.MeasureString(this.Text, SystemInformation.MenuFont);

            e.ItemWidth = SystemInformation.MenuHeight;
            e.ItemHeight = SystemInformation.MenuHeight;
            e.ItemWidth += (int)Math.Round(textSize.Width);

            base.OnMeasureItem(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.MenuItem.DrawItem"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DrawItemEventArgs"/> that contains the event data.</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (this.Enabled)
            {
                e.DrawBackground();
            }

            Rectangle imgRect = e.Bounds;
            imgRect.Width = SystemInformation.MenuHeight;

            #region Draw background
            e.Graphics.FillRectangle(SystemBrushes.Control, imgRect);
            #endregion

            #region Draw checkbox
            if ((e.State & DrawItemState.Checked) == DrawItemState.Checked)
            {
                Rectangle checkRect = imgRect;// Rectangle.Inflate(imgRect, -1, -1);

                e.Graphics.DrawRectangle(SystemPens.ControlDarkDark,
                    checkRect.X, checkRect.Y, checkRect.Width - 1, checkRect.Height - 1);
            }
            #endregion

            #region Draw icon
            if (this.Image != null)
            {
                e.Graphics.DrawImage(this.Image, Rectangle.Inflate(imgRect, -3, -3));
            }
            #endregion

            #region Draw text
            Rectangle txtRect = e.Bounds;
            StringFormat strFormat = new StringFormat();

            strFormat.LineAlignment = StringAlignment.Center;

            txtRect.X += SystemInformation.MenuHeight;
            txtRect.Width -= SystemInformation.MenuHeight;

            if (this.Enabled)
            {
                using (SolidBrush sb = new SolidBrush(SystemColors.MenuText))
                {
                    e.Graphics.DrawString(this.Text, SystemInformation.MenuFont, sb, txtRect, strFormat);
                }
            }
            else
            {
                using (SolidBrush sb = new SolidBrush(Color.Gray))
                {
                    e.Graphics.DrawString(this.Text, SystemInformation.MenuFont, sb, txtRect, strFormat);
                }
            }
            #endregion

            e.DrawFocusRectangle();
        }
        #endregion
    }

    /// <summary>
    /// The ChartCommandMenuItem class which is inherited from TagMenuItem.
    /// </summary>
    class ChartCommandMenuItem : TagMenuItem
    {
        #region Members
        private ChartCommand m_command;
        private string m_commandParameter;
        private ChartControl m_chart;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public ChartCommand Command
        {
            get { return m_command; }

            set { m_command = value; }
        }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>The command parameter.</value>
        public string CommandParameter
        {
            get { return m_commandParameter; }

            set { m_commandParameter = value; }
        }

        /// <summary>
        /// Gets or sets the chart.
        /// </summary>
        /// <value>The chart.</value>
        public ChartControl Chart
        {
            get { return m_chart; }

            set { m_chart = value; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCommandMenuItem"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public ChartCommandMenuItem(ChartCommand command)
            : base(command)
        {
            this.Image = command.Image;
            this.Checked = command.IsToggled(m_chart, m_commandParameter);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.MenuItem.Click"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            if (m_chart != null)
            {
                m_command.Execute(m_chart, m_commandParameter);
                this.Checked = m_command.IsToggled(m_chart, m_commandParameter);
            }

            base.OnClick(e);
        }
        #endregion
    }
}

