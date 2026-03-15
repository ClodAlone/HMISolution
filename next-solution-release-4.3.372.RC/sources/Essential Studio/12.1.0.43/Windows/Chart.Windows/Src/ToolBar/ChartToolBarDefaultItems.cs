#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart.Utils;

namespace Syncfusion.Windows.Forms.Chart
{
    #region Save Item
    /// <summary>
    /// Represents default toolbar item. This item provides the saving feature.
    /// </summary>
    public sealed class ChartToolBarSaveItem : ChartToolBarItem
    {
        #region Constants
        private const string c_imagesFilter = "Image file(*.bmp,*.jpeg,*.jpg,*.tiff,*.gif,*.emf)|*.bmp;*.jpeg;*.jpg;*.tiff;*.gif;*.emf|All files (*.*)|*.*";
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Checked
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsCheckable
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected override Image DefaultImage
        {
            get
            {
                return ChartCommandsImages.Save;
            }
        }

        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected override string DefaultToolTip
        {
            get
            {
                return "Save chart";
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "SaveItem";
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected override void OnClick()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = c_imagesFilter;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.Chart.SaveImage(dialog.FileName);
            }

            base.OnClick();
        }
        #endregion
    }
    #endregion

    #region Copy Item
    /// <summary>
    /// Represents default toolbar item. This item provides the copying feature.
    /// </summary>
    public sealed class ChartToolBarCopyItem : ChartToolBarItem
    {
        #region Proeprties
        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected override Image DefaultImage
        {
            get
            {
                return ChartCommandsImages.Copy;
            }
        }

        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected override string DefaultToolTip
        {
            get
            {
                return "Copy chart";
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "CopyItem";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Checked
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsCheckable
        {
            get
            {
                return false;
            }

            set
            {
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected override void OnClick()
        {
            Image img = new Bitmap(this.Chart.Width, this.Chart.Height);
            this.Chart.Draw(img);
            Clipboard.SetDataObject(img);

            base.OnClick();
        }
        #endregion
    }
    #endregion

    #region Print Item
    /// <summary>
    /// Represents default toolbar item. This item provides the printing feature.
    /// </summary>
    public sealed class ChartToolBarPrintItem : ChartToolBarItem
    {
        #region Proeprties
        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected override Image DefaultImage
        {
            get
            {
                return ChartCommandsImages.Print;
            }
        }

        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected override string DefaultToolTip
        {
            get
            {
                return "Print chart";
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "PrintItem";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Checked
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsCheckable
        {
            get
            {
                return false;
            }

            set
            {
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected override void OnClick()
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = this.Chart.PrintDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.Chart.PrintDocument.Print();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            base.OnClick();
        }
        #endregion
    }
    #endregion

    #region PrintPreview Item
    /// <summary>
    /// Represents default toolbar item. This item provides the print preview feature.
    /// </summary>
    public sealed class ChartToolBarPrintPreviewItem : ChartToolBarItem
    {
        #region Properties
        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected override Image DefaultImage
        {
            get
            {
                return ChartCommandsImages.PrintPreview;
            }
        }

        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected override string DefaultToolTip
        {
            get
            {
                return "Print preview";
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "PrintPreviewItem";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Checked
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        ///    <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsCheckable
        {
            get
            {
                return false;
            }

            set
            {
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected override void OnClick()
        {
            PrintPreviewDialog printDialog = new PrintPreviewDialog();

            printDialog.Document = this.Chart.PrintDocument;
            printDialog.ShowDialog();

            base.OnClick();
        }
        #endregion
    }
    #endregion

    #region Palette Item
    /// <summary>
    /// Represents default toolbar item. This item provides the palette choosing feature.
    /// </summary>
    public sealed class ChartToolBarPaletteItem : ChartToolBarDropDown
    {
        #region Constants
        private const string c_menuItemEditPaletteText = "Edit palette...";
        private const string c_menuItemSeparatorText = "-";
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected override Image DefaultImage
        {
            get
            {
                return ChartCommandsImages.Palettes;
            }
        }

        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected override string DefaultToolTip
        {
            get
            {
                return "Change palette";
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "PaletteItem";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Checked
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        ///    <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsCheckable
        {
            get
            {
                return false;
            }

            set
            {
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected override void OnClick()
        {
            m_menu = new ContextMenu();
            Array palettes = Enum.GetValues(typeof(ChartColorPalette));

            for (int i = 0, end = palettes.Length; i < end; i++)
            {
                ChartColorPalette palette = (ChartColorPalette)palettes.GetValue(i);
                TagMenuItem tagMenuItem = new TagMenuItem(palette, new EventHandler(OnPaletteMenuClick));
                tagMenuItem.Image = this.Chart.Model.ColorModel.CreatePaletteIcon(new Size(16, 16), palette, 6);
                m_menu.MenuItems.Add(tagMenuItem);

                if (this.Chart.Palette.Equals(palettes.GetValue(i)))
                {
                    m_menu.MenuItems[i].Checked = true;
                }
            }

            m_menu.MenuItems.Add(new MenuItem(c_menuItemSeparatorText));
            m_menu.MenuItems.Add(new MenuItem(this.Chart.Localization.EditPalette, new EventHandler(OnEditPaletteMenuClick)));

            base.OnClick();
        }

        /// <summary>
        /// Method is called when the palettes menu is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPaletteMenuClick(object sender, EventArgs e)
        {
            foreach (ChartSeries series in this.Chart.Series)
            {
                series.Style.ResetInterior();
            }

            this.Chart.Palette = (ChartColorPalette)(sender as TagMenuItem).Tag;
        }

        /// <summary>
        /// Method is called when the Edit palette item is clicked.
        /// </summary>
        /// <param name="sender">A sender of the event.</param>
        /// <param name="e">Argument.</param>
        private void OnEditPaletteMenuClick(object sender, EventArgs e)
        {
            PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(this.Chart)["CustomPalette"];

            if (propDescriptor != null)
            {
                UITypeEditor uiEditor = (UITypeEditor)propDescriptor.GetEditor(typeof(UITypeEditor));

                if (uiEditor != null)
                {
                    TypeDescriptorContext typeDescriptorContext = new TypeDescriptorContext(this.Chart.CustomPalette, propDescriptor);
                    WindowsFormsEditorServiceContainer windowsFormsEditorServiceContainer = new WindowsFormsEditorServiceContainer(typeDescriptorContext.ServiceProvider);

                    Color[] palette = this.Chart.CustomPalette == null ? new Color[0] : this.Chart.CustomPalette;

                    this.Chart.CustomPalette = (Color[])uiEditor.EditValue(typeDescriptorContext, windowsFormsEditorServiceContainer, palette);
                }
            }
        }
        #endregion
    }
    #endregion

    #region Style Item
    /// <summary>
    /// Represents default toolbar item. This item provides the styles editing feature.
    /// </summary>
    public sealed class ChartToolBarStyleItem : ChartToolBarDropDown
    {
        #region Properties
        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected override Image DefaultImage
        {
            get
            {
                return ChartCommandsImages.Style;
            }
        }

        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected override string DefaultToolTip
        {
            get
            {
                return "Edit style";
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "EditStyleItem";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Checked
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsCheckable
        {
            get
            {
                return false;
            }

            set
            {
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected override void OnClick()
        {
            ContextMenu seriesMenu = new ContextMenu();

            foreach (ChartSeries series in this.Chart.Series)
            {
                TagMenuItem tagMenuItem = new TagMenuItem(series.Name, series, new EventHandler(OnMenuClick));
                tagMenuItem.Image = new Bitmap(16, 16);

                using (Graphics g = Graphics.FromImage(tagMenuItem.Image))
                {
                    BrushPaint.FillRectangle(g, new Rectangle(0, 0, 16, 16), series.GetOfflineStyle().Interior);
                    g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
                }

                seriesMenu.MenuItems.Add(tagMenuItem);
            }

            seriesMenu.Show(this.ToolBar, new Point(this.Bounds.Left, this.Bounds.Bottom));

            base.OnClick();
        }

        /// <summary>
        /// Method is called when the series menu is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMenuClick(object sender, EventArgs e)
        {
            this.Chart.DisplayUserEditStylesDialog((sender as MenuItem).Index);
        }
        #endregion
    }
    #endregion

    #region Type Item
    /// <summary>
    /// Represents default toolbar item. This item provides the type choosing feature.
    /// </summary>
    public sealed class ChartToolBarTypeItem : ChartToolBarDropDown
    {
        #region Properties
        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected override Image DefaultImage
        {
            get
            {
                return ChartCommandsImages.SeriesType;
            }
        }

        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected override string DefaultToolTip
        {
            get
            {
                return "Change type";
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "SerieTypeItem";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Checked
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsCheckable
        {
            get
            {
                return false;
            }

            set
            {
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected override void OnClick()
        {
            bool isCombinedType = false;
            Array palettes = Enum.GetValues(typeof(ChartSeriesType));
            ChartSeriesType combinedType = ChartSeriesType.Line;

            if (this.Chart.Series.Count > 0)
            {
                isCombinedType = true;
                combinedType = this.Chart.Series[0].Type;

                for (int i = 1; i < this.Chart.Series.Count; i++)
                {
                    if (this.Chart.Series[i].Type != combinedType)
                    {
                        isCombinedType = false;
                        break;
                    }
                }
            }

            ContextMenu typeMenu = new ContextMenu();

            for (int i = 0, end = palettes.Length; i < end; i++)
            {
                ChartSeriesType seriesType = (ChartSeriesType)palettes.GetValue(i);

                if (seriesType != ChartSeriesType.Custom)
                {
                    TagMenuItem tagMenuItem = new TagMenuItem(seriesType, new EventHandler(OnTypeMenuClick));
                    tagMenuItem.Image = ChartSeriesTypeImages.GetImage((ChartSeriesType)palettes.GetValue(i));
                    typeMenu.MenuItems.Add(tagMenuItem);

                    if (isCombinedType)
                    {
                        if (combinedType.Equals(seriesType))
                        {
                            typeMenu.MenuItems[i].Checked = true;
                        }
                    }
                }
            }

            m_menu = typeMenu;

            base.OnClick();
        }

        /// <summary>
        /// Method is called when the palettes menu is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTypeMenuClick(object sender, EventArgs e)
        {
            for (int i = 0; i < this.Chart.Series.Count; i++)
            {
                this.Chart.Series[i].Type = (ChartSeriesType)(sender as TagMenuItem).Tag;
            }
        }
        #endregion
    }
    #endregion

    #region Series3D Item
    /// <summary>
    /// Represents default toolbar item. This item provides the drawing mode changing feature.
    /// </summary>
    public sealed class ChartToolBarSeries3DItem : ChartToolBarItem
    {
        #region Proeprties
        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected override string DefaultToolTip
        {
            get
            {
                return "Switch 3D mode";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Checked
        {
            get
            {
                return this.Chart == null ? false : this.Chart.Series3D;
            }

            set
            {
                if (this.Chart != null) this.Chart.Series3D = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsCheckable
        {
            get
            {
                return true;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected override Image DefaultImage
        {
            get
            {
                return ChartCommandsImages.Series3D;
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "Series3DItem";
            }
        }
        #endregion
    }
    #endregion

    #region ShowLegend Item
    /// <summary>
    /// Represents default toolbar item. This item provides the legend showing feature.
    /// </summary>
    public sealed class ChartToolBarShowLegendItem : ChartToolBarItem
    {
        #region Properties
        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected override string DefaultToolTip
        {
            get
            {
                return "Show/Hide legend";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Checked
        {
            get
            {
                return this.Chart == null ? false : this.Chart.ShowLegend;
            }

            set
            {
                if (this.Chart != null) this.Chart.ShowLegend = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsCheckable
        {
            get
            {
                return true;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected override Image DefaultImage
        {
            get
            {
                return ChartCommandsImages.ShowLegend;
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "ShowLegendItem";
            }
        }
        #endregion
    }
    #endregion
}
