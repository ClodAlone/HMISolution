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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Page size control
    /// </summary>
    public partial class PageSizeControl : UserControl
    {
        #region Class Constants
        private const string m_cSameAsPrinter = "SameAsPrinter";
        private const string m_cCustom = "SameAsPrinter";
		private const string m_cSizeToFitDrawing = "SizeToFitDrawing";
        #endregion

        #region Class Members
        private PageSize m_pageSize;
        private PageSettings m_printerSettings;
        private SizeF m_szModel;
        #endregion

        #region Class Event Handlers
        /// <summary>
        /// Occurs when page size is changed.
        /// </summary>
        public event EventHandler PageSizeChanged;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets or sets the printer settings.
        /// </summary>
        /// <value>The printer settings.</value>
        public PageSettings PrinterSettings
        {
            get
            {
                if (m_printerSettings == null)
                    m_printerSettings = new PageSettings();

                return m_printerSettings;
            }
            set
            {
                if (m_printerSettings != value)
                {
                    m_printerSettings = value;
                    UpdatePageSize();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public PageSize PageSize
        {
            get
            {
                if (m_pageSize == null)
                    m_pageSize = new PageSize();

                return m_pageSize;
            }
            set
            {
                if (m_pageSize != value)
                {
                    m_pageSize = value;
                    InitializePageSize();
                    RaisePageSizeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether page landscape is checked.
        /// </summary>
        /// <value><c>true</c> if page landscape is checked; otherwise, <c>false</c>.</value>
        public bool PageLandscape
        {
            get 
            { 
                return rdPageSizeLandscape.Checked; 
            }
            set
            {
                rdPageSizeLandscape.Checked = value;
                rdPageSizePortrait.Checked = !value;
            }
        }

        /// <summary>
        /// Gets or sets the size of content of the model.
        /// </summary>
        /// <value>The content of the model.</value>
        public SizeF ModelContentSize
        {
            get { return m_szModel; }
            set { m_szModel = value; }
        }
        #endregion

        #region Class Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PageSizeControl"/> class.
        /// </summary>
        public PageSizeControl()
        {
            InitializeComponent();

            FillCombos();
        }
        #endregion

        #region Class Events Methods
        private void PageSize_CheckedChanged(object sender, EventArgs e)
        {
            bool bPreDefinedSize = rdPageSizePreDefinedSize.Checked;
            bool bCustomSize = rdPageSizeCustomSize.Checked;
            
            // lock/unlock predefine size combos
            comboPageSizeStandart.Enabled = bPreDefinedSize;
            comboPageSizePaper.Enabled = bPreDefinedSize;

            // lock/unlock custom size fields
            txtPageSizeCustomWidth.Enabled = bCustomSize;
            txtPageSizeCustomHeight.Enabled = bCustomSize;

            rdPageSizePortrait.Enabled = !rdPageSizeToFitDrawingContent.Checked;
            rdPageSizeLandscape.Enabled = !rdPageSizeToFitDrawingContent.Checked;

            // save changes
            UpdatePageSize();
            RaisePageSizeChanged();
        }
        private void ComboPageSizeStandart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboPageSizeStandart.SelectedItem != null)
            {
                PaperStandart standart = (PaperStandart)comboPageSizeStandart.SelectedItem;
                comboPageSizePaper.Items.Clear();

                for (int i = 0, length = standart.PageSizes.Length; i < length; i++)
                {
                    comboPageSizePaper.Items.Add(standart.PageSizes[i]);
                }

                comboPageSizePaper.SelectedIndex = 0;
            }
        }
        private void ComboPageSizePaper_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboPageSizePaper.SelectedItem != null)
            {
                PageSize paperSize = (PageSize)comboPageSizePaper.SelectedItem;

                if (this.PageLandscape)
                {
                    txtPageSizeCustomWidth.SetValue(paperSize.Height, paperSize.HeightMeasureUnit);
                    txtPageSizeCustomHeight.SetValue(paperSize.Width, paperSize.WidthMeasureUnit);
                }
                else
                {
                    txtPageSizeCustomWidth.SetValue(paperSize.Width, paperSize.WidthMeasureUnit);
                    txtPageSizeCustomHeight.SetValue(paperSize.Height, paperSize.HeightMeasureUnit);
                }

                m_pageSize = (PageSize)paperSize.Clone();
            }

            RaisePageSizeChanged();
        }
        private void PageSizeCustomSize_TextChanged(object sender, EventArgs e)
        {
            PageSize pageSize = this.PageSize;

            if (this.PageLandscape)
            {
                pageSize.HeightMeasureUnit = txtPageSizeCustomWidth.MeasureUnits;
                pageSize.Height = txtPageSizeCustomWidth.Value;

                pageSize.WidthMeasureUnit = txtPageSizeCustomHeight.MeasureUnits;
                pageSize.Width = txtPageSizeCustomHeight.Value;
            }
            else
            {
                pageSize.WidthMeasureUnit = txtPageSizeCustomWidth.MeasureUnits;
                pageSize.Width = txtPageSizeCustomWidth.Value;

                pageSize.HeightMeasureUnit = txtPageSizeCustomHeight.MeasureUnits;
                pageSize.Height = txtPageSizeCustomHeight.Value;
            }

            RaisePageSizeChanged();
        }
        private void PageSizeOrientation_CheckedChanged(object sender, EventArgs e)
        {
            PageSize pageSize = this.PageSize;

            float fWidth = this.PageLandscape ? pageSize.Width : pageSize.Height;
            MeasureUnits mWidth = this.PageLandscape ? pageSize.WidthMeasureUnit : pageSize.HeightMeasureUnit;

            txtPageSizeCustomWidth.MeasureUnits = this.PageLandscape ? pageSize.HeightMeasureUnit : pageSize.WidthMeasureUnit;
            txtPageSizeCustomWidth.Value = m_pageSize.Width = this.PageLandscape ? pageSize.Height : pageSize.Width;

            txtPageSizeCustomHeight.MeasureUnits = mWidth;
            txtPageSizeCustomHeight.Value = m_pageSize.Height = fWidth;
			this.PrinterSettings.Landscape = this.PageLandscape;
            RaisePageSizeChanged();
        }
        #endregion

        #region Class Helper Methods
        private void FillCombos()
        {
            FillCombo(comboPageSizeStandart, PaperStandart.GetStandarts());
        }
        private void FillCombo(ComboBox combo, IList array)
        {
            for (int i = 0, length = array.Count; i < length; i++)
            {
                combo.Items.Add(array[i]);
            }

            // select first item
            combo.SelectedIndex = 0;
        }
        private void InitializePageSize()
        {
            PageSize pageSize = (PageSize)this.PageSize.Clone();
			float fWidth = 0f;
            float fHeight = 0f;
            bool bFound = false;

            if (this.PageSize.DisplayName == m_cSizeToFitDrawing)
            {
                rdPageSizeToFitDrawingContent.Checked = true;
            }            
            else if (this.PrinterSettings.PaperSize.Width == pageSize.PixelWidth
                && this.PrinterSettings.PaperSize.Height == pageSize.Height)
            {               
                rdPageSizeSameAsPrinterSize.Checked = true;
               
            }
            else if (this.PrinterSettings.PaperSize.Height == pageSize.PixelWidth
                && this.PrinterSettings.PaperSize.Width == pageSize.Height)
            {
                rdPageSizeSameAsPrinterSize.Checked = true;                
            }
            else
            {
                PaperStandart[] standarts = PaperStandart.GetStandarts();

                for (int i = 0, length = standarts.Length; i < length && !bFound; i++)
                {
                    PageSize[] sizes = standarts[i].PageSizes;

                    for (int j = 0, nLength = sizes.Length; j < nLength && !bFound; j++)
                    {
                        PageSize size = sizes[j];

                        if (size.PixelWidth == pageSize.PixelWidth && size.PixelHeight == pageSize.PixelHeight)
                        {
                            comboPageSizeStandart.SelectedIndex = i;
                            comboPageSizePaper.SelectedIndex = j;
                            rdPageSizePreDefinedSize.Checked = true;
                            bFound = true;
                        }
                    }
                }

                if (!bFound)
                {
                    rdPageSizeCustomSize.Checked = true;
                }
            }

            // set orientation
            rdPageSizeLandscape.Checked = this.PageLandscape;
            fWidth = pageSize.Width;
            fHeight = pageSize.Height;
            MeasureUnits mWidth = pageSize.WidthMeasureUnit;
            MeasureUnits mHeight = pageSize.HeightMeasureUnit;

            // set custom size
            if (this.PageLandscape)
            {
                txtPageSizeCustomHeight.SetValue(fWidth, mWidth);
                txtPageSizeCustomWidth.SetValue(fHeight, mHeight);
            }
            else
            {
                txtPageSizeCustomWidth.SetValue(fWidth, mWidth);
                txtPageSizeCustomHeight.SetValue(fHeight, mHeight);
            }
        }
        private void UpdatePageSize()
        {
            if (rdPageSizeSameAsPrinterSize.Checked)
            {
                PaperSize pageSize = PrinterSettings.PaperSize;
                float fWidth = this.PageLandscape ? pageSize.Height : pageSize.Width;
                float fHeight = this.PageLandscape ? pageSize.Width : pageSize.Height;

                // update custom size
                txtPageSizeCustomWidth.SetValue(fWidth, MeasureUnits.Pixel);
                txtPageSizeCustomHeight.SetValue(fHeight, MeasureUnits.Pixel);

                // update page orientation
                rdPageSizeLandscape.Checked = PrinterSettings.Landscape;
                rdPageSizePortrait.Checked = !PrinterSettings.Landscape;

                m_pageSize.DisplayName = m_cSameAsPrinter;
            }
            else if (rdPageSizeToFitDrawingContent.Checked)
            {				
                m_pageSize.DisplayName = m_cSizeToFitDrawing;
                txtPageSizeCustomWidth.Value = MeasureUnitsConverter.FromPixelX(this.ModelContentSize.Width, txtPageSizeCustomWidth.MeasureUnits);
                txtPageSizeCustomHeight.Value = MeasureUnitsConverter.FromPixelX(this.ModelContentSize.Height, txtPageSizeCustomWidth.MeasureUnits);

                rdPageSizeLandscape.Checked = this.PrinterSettings.Landscape;
                rdPageSizePortrait.Checked = !this.PrinterSettings.Landscape;
            }
            else if (rdPageSizePreDefinedSize.Checked)
            {
                if (comboPageSizePaper.SelectedItem != null)
                {
                    PageSize paperSize = (PageSize)comboPageSizePaper.SelectedItem;

                    if (this.PageLandscape)
                    {
                        txtPageSizeCustomWidth.SetValue(paperSize.Height, paperSize.HeightMeasureUnit);
                        txtPageSizeCustomHeight.SetValue(paperSize.Width, paperSize.WidthMeasureUnit);
                    }
                    else
                    {
                        txtPageSizeCustomWidth.SetValue(paperSize.Width, paperSize.WidthMeasureUnit);
                        txtPageSizeCustomHeight.SetValue(paperSize.Height, paperSize.HeightMeasureUnit);
                    }

                    m_pageSize = (PageSize)paperSize.Clone();
                }
            }
            else
            {
                m_pageSize.DisplayName = m_cCustom;
            }
        }

        private void RaisePageSizeChanged()
        {
            if (PageSizeChanged != null)
                PageSizeChanged(this, EventArgs.Empty);
        }
        #endregion
    }
}
