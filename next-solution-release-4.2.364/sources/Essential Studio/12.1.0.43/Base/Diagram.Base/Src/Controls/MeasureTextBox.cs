#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Measure TextBox
    /// </summary>
    public class MeasureTextBox : TextBox
    {
        #region Class members
        private float m_fValue;
        private MeasureUnits m_measureUnits;
        private float m_fDefaultValue = 0f;
        private MeasureUnits m_defaultUnit = MeasureUnits.Pixel;
        #endregion

        #region Class event handlers
        /// <summary>
        /// Occurs when value is changing.
        /// </summary>
        public event EventHandler ValueChanging;

        /// <summary>
        /// Occurs when value is changed.
        /// </summary>
        public event EventHandler ValueChanged;
        #endregion

        #region Class initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureTextBox"/> class.
        /// </summary>
        public MeasureTextBox()
            : base()
        {
            UpdateText();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the current text in the <see cref="T:System.Windows.Forms.TextBox"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The text displayed in the control.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Gets or sets the default measure units.
        /// </summary>
        /// <value>The default measure units.</value>
        public MeasureUnits DefaultMeasureUnits
        {
            get { return m_defaultUnit; }
            set { m_defaultUnit = value; }
        }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public float DefaultValue
        {
            get { return m_fDefaultValue; }
            set { m_fDefaultValue = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value
        {
            get 
            { 
                return m_fValue; 
            }
            set
            {
                if (m_fValue != value)
                {
                    OnValueChanging();

                    m_fValue = value;
                    UpdateText();

                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether value is validated.
        /// </summary>
        /// <value><c>true</c> if value is validated; otherwise, <c>false</c>.</value>
        public bool ValidatedValue
        {
            get
            {
                float value;
                MeasureUnits measureUnit;
                return TryParseMeasureValue(this.Text, out value, out measureUnit);
            }
        }

        /// <summary>
        /// Gets or sets the measure units.
        /// </summary>
        /// <value>The measure units.</value>
        public MeasureUnits MeasureUnits
        {
            get 
            { 
                return m_measureUnits; 
            }
            set
            {
                if (m_measureUnits != value)
                {
                    OnValueChanging();

                    m_fValue = MeasureUnitsConverter.Convert(this.Value, m_measureUnits, value);
                    m_measureUnits = value;

                    UpdateText();

                    OnValueChanged();
                }
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Raises the TextChanged event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            ApplyValue(false);
            base.OnTextChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Enter"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnEnter(EventArgs e)
        {
            ApplyValue(true);
            base.OnEnter(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            ApplyValue(true);
            base.OnLostFocus(e);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="fValue">The value.</param>
        /// <param name="unit">The unit.</param>
        public void SetValue(float fValue, MeasureUnits unit)
        {
            if (m_fValue != fValue || m_measureUnits != unit)
            {
                OnValueChanging();

                m_measureUnits = unit;
                m_fValue = fValue;

                // update text
                UpdateText();

                OnValueChanged();
            }
        }
        #endregion

        #region Class helper methods
        private void OnValueChanging()
        {
            if (ValueChanging != null)
            {
                ValueChanging(this, EventArgs.Empty);
            }
        }
        private void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }
        private bool ApplyValue(bool bShowError)
        {
            bool bSuccess = false;
            float value;
            MeasureUnits measureUnit;

            bSuccess = TryParseMeasureValue(this.Text, out value, out measureUnit);

            if (bSuccess)
            {
                if (m_fValue != value || m_measureUnits != measureUnit)
                {
                    OnValueChanging();

                    m_measureUnits = measureUnit;
                    m_fValue = value;

                    if (bShowError)
                    {
                        // update text
                        UpdateText();
                    }

                    OnValueChanged();
                }
            }
            else if (bShowError)
            {
                UpdateText();
                string caption = "Page Setup";
                string text = "Value is not valid";
                MessageBox.Show(this, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }

            return bSuccess;
        }
        private void UpdateText()
        {
            string strText = string.Format("{0} {1}", this.Value, MeasureUnitsConverter.GetAbbreviation(this.MeasureUnits));

            if (strText != this.Text)
            {
                this.Text = strText;
            }
        }
        private bool TryParseMeasureValue(string text, out float value, out MeasureUnits units)
        {
            bool bSuccess = false;
            value = this.DefaultValue;
            units = this.DefaultMeasureUnits;

            char[] separator = { ' ' };
            string[] values = text.Split(separator, 2);

            if (values.Length > 0 && values[0] != string.Empty)
            {
                bSuccess = float.TryParse(values[0], out value);

                if (bSuccess && values.Length > 1 && values[1] != string.Empty)
                {
                    bSuccess = MeasureUnitsConverter.GetMeasureUnit(values[1], out units);
                }
            }

            return bSuccess;
        }
        #endregion
    }
}
