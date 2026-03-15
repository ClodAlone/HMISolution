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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Drawing scale control.
    /// </summary>
    public partial class DrawingScaleControl : UserControl
    {
        #region Class Members
        private PageScale m_pageScale = null;
        private PageSize m_pageSize;
        #endregion

        #region Class Event Handlers
        /// <summary>
        /// Occurs when page scale is changed.
        /// </summary>
        public event EventHandler PageScaleChanged;

        /// <summary>
        /// Occurs when page size is changed.
        /// </summary>
        public event EventHandler PageSizeChanged;

        /// <summary>
        /// Occurs when measure units is changed.
        /// </summary>
        public event EventHandler MeasureUnitsChanged;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets or sets the page scale.
        /// </summary>
        /// <value>The page scale.</value>
        public PageScale PageScale
        {
            get
            {
                if (m_pageScale == null)
                    m_pageScale = new PageScale();

                return m_pageScale;
            }
            set
            {
                if (m_pageScale != value)
                {
                    m_pageScale = value;
                    InitializeDrawingScale();
                    RaisePageScaleChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size in measure current measurement units.
        /// </summary>
        /// <value>The size in current measurement units.</value>
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
                    MeasureUnits units = this.MeasureUnits;
                    float fWidth = MeasureUnitsConverter.ConvertX(value.Width, value.WidthMeasureUnit, units);
                    float fHeight = MeasureUnitsConverter.ConvertY(value.Height, value.HeightMeasureUnit, units);

                    float fFactor = this.PageScale.GetScaleFactor(units);
                    fWidth /= fFactor;
                    fHeight /= fFactor;

                    // update document size in measure units
                    txtDrawingScalePageWidth.SetValue(fWidth, units);
                    txtDrawingScalePageHeight.SetValue(fHeight, units);

                    m_pageSize = value;
                    RaisePageSizeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current setting measure units.
        /// </summary>
        /// <value>The measure units.</value>
        public MeasureUnits MeasureUnits
        {
            get 
            { 
                return (MeasureUnits)comboMeasureUnits.SelectedItem; 
            }
            set
            {
                if (this.MeasureUnits != value)
                {
                    comboMeasureUnits.SelectedItem = value;
                    RaiseMeasureUnitsChanged();
                }
            }
        }
        #endregion

        #region Class Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingScaleControl"/> class.
        /// </summary>
        public DrawingScaleControl()
        {
            InitializeComponent();

            FillSetupCombos();
        }
        #endregion

        #region Class Events
        private void DrawingScale_CheckedChanged(object sender, EventArgs e)
        {
            comboDrawingPreDefinedPaper.Enabled = rdDrawingPreDefinedScale.Checked;
            comboDrawingPreDefinedStandart.Enabled = rdDrawingPreDefinedScale.Checked;

            txtDrawingCustomWidth.Enabled = rdDrawingCustomScale.Checked;
            txtDrawingCustomHeight.Enabled = rdDrawingCustomScale.Checked;

            txtDrawingScalePageWidth.Enabled = !rdDrawingNoScale.Checked;
            txtDrawingScalePageHeight.Enabled = !rdDrawingNoScale.Checked;

            UpdatePageScale();
            RaisePageScaleChanged();
        }
        private void ComboDrawingPreDefinedStandart_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaperScaleStandart scale = comboDrawingPreDefinedStandart.SelectedItem as PaperScaleStandart;

            if (scale != null)
            {
                comboDrawingPreDefinedPaper.Items.Clear();

                for (int i = 0, length = scale.PageScales.Length; i < length; i++)
                {
                    comboDrawingPreDefinedPaper.Items.Add(scale.PageScales[i]);
                }

                comboDrawingPreDefinedPaper.SelectedIndex = 0;
            }
        }
        private void ComboDrawingPreDefinedPaper_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePageScale();
            RaisePageScaleChanged();
        }
        private void DrawingCustom_TextChanged(object sender, EventArgs e)
        {
            UpdatePageScale();

            PageSize pageSize = (PageSize)this.PageSize.Clone();
            MeasureUnits units = this.MeasureUnits;
            float fWidth = MeasureUnitsConverter.ConvertX(pageSize.Width, pageSize.WidthMeasureUnit, units);
            float fHeight = MeasureUnitsConverter.ConvertY(pageSize.Height, pageSize.HeightMeasureUnit, units);

            float fFactor = this.PageScale.GetScaleFactor(units);
            fWidth /= fFactor;
            fHeight /= fFactor;

            // update document size in measure units
            txtDrawingScalePageWidth.SetValue(fWidth, units);
            txtDrawingScalePageHeight.SetValue(fHeight, units);

            RaisePageScaleChanged();
        }
        private void ComboMeasureUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePageScale();
            RaiseMeasureUnitsChanged();
        }
        private void DrawingScalePageSize_ValueChanged(object sender, EventArgs e)
        {
            MeasureUnits mUnits = this.MeasureUnits;
            float fWidth = MeasureUnitsConverter.ConvertX(txtDrawingScalePageWidth.Value, txtDrawingScalePageWidth.MeasureUnits, mUnits);
            float fHeight = MeasureUnitsConverter.ConvertY(txtDrawingScalePageHeight.Value, txtDrawingScalePageHeight.MeasureUnits, mUnits);

            float fFactor = this.PageScale.GetScaleFactor(mUnits);
            fWidth *= fFactor;
            fHeight *= fFactor;
            m_pageSize = new PageSize(fWidth, mUnits, fHeight, mUnits);
            RaisePageSizeChanged();
        }
        #endregion

        #region Class Helper Methods
        private void InitializeDrawingScale()
        {
            PaperScaleStandart[] standarts = PaperScaleStandart.GetStandarts();
            bool bFound = false;
            string strDisplayName = this.PageScale.DisplayName;

            for (int i = 0, length = standarts.Length; i < length && !bFound; i++)
            {
                PageScale[] sizes = standarts[i].PageScales;

                for (int j = 0, nLength = sizes.Length; j < nLength && !bFound; j++)
                {
                    if (sizes[j].DisplayName == strDisplayName)
                    {
                        rdDrawingPreDefinedScale.Checked = true;
                        comboDrawingPreDefinedStandart.SelectedIndex = i;
                        comboDrawingPreDefinedPaper.SelectedIndex = j;
                        bFound = true;
                    }
                }
            }

            float fWidth = this.PageScale.ModelScale;
            float fHeight = this.PageScale.DrawingScale;
            MeasureUnits mWidth = this.PageScale.ModelScaleUnit;
            MeasureUnits mHeight = this.PageScale.DrawingScaleUnit;

            if (!bFound)
            {
                if (this.PageScale.PixelWidth == this.PageScale.PixelHeight && strDisplayName.ToLower() == "noscale")
                {
                    if (!rdDrawingNoScale.Checked)
                        rdDrawingNoScale.Checked = true;
                }
                else if (!rdDrawingCustomScale.Checked)
                {
                    rdDrawingCustomScale.Checked = true;
                }
            }

            txtDrawingCustomWidth.MeasureUnits = mWidth;
            txtDrawingCustomWidth.Value = fWidth;

            txtDrawingCustomHeight.MeasureUnits = mHeight;
            txtDrawingCustomHeight.Value = fHeight;
        }
        private void FillSetupCombos()
        {
            FillCombo(comboMeasureUnits, (MeasureUnits[])Enum.GetValues(typeof(MeasureUnits)));
            FillCombo(comboDrawingPreDefinedStandart, PaperScaleStandart.GetStandarts());
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
        private void UpdatePageScale()
        {
            MeasureUnits units = this.MeasureUnits;

            if (rdDrawingNoScale.Checked)
            {
                m_pageScale = new PageScale("NoScale", 1, units, 1, units);

                txtDrawingCustomWidth.SetValue(1, m_pageScale.ModelScaleUnit);
                txtDrawingCustomHeight.SetValue(1, m_pageScale.DrawingScaleUnit);
            }
            else if (rdDrawingPreDefinedScale.Checked)
            {
                PageScale pageScale = comboDrawingPreDefinedPaper.SelectedItem as PageScale;

                if (pageScale != null)
                {
                    txtDrawingCustomWidth.SetValue(pageScale.ModelScale, this.MeasureUnits);
                    txtDrawingCustomHeight.SetValue(pageScale.DrawingScale, this.MeasureUnits);

                    m_pageScale = new PageScale(pageScale.DisplayName, pageScale.ModelScale, this.MeasureUnits, pageScale.DrawingScale, this.MeasureUnits);
                }
            }
            else
            {
                m_pageScale = new PageScale(
                    "Custom", 
                    txtDrawingCustomWidth.Value, 
                    txtDrawingCustomWidth.MeasureUnits,
                    txtDrawingCustomHeight.Value, 
                    txtDrawingCustomHeight.MeasureUnits);

                // update measure units
                txtDrawingScalePageWidth.MeasureUnits = units;
                txtDrawingScalePageHeight.MeasureUnits = units;
            }
        }

        private void RaisePageScaleChanged()
        {
            if (PageScaleChanged != null)
            {
                PageScaleChanged(this, EventArgs.Empty);
            }
        }
        private void RaisePageSizeChanged()
        {
            if (PageSizeChanged != null)
            {
                PageSizeChanged(this, EventArgs.Empty);
            }
        }
        private void RaiseMeasureUnitsChanged()
        {
            if (MeasureUnitsChanged != null)
            {
                MeasureUnitsChanged(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
