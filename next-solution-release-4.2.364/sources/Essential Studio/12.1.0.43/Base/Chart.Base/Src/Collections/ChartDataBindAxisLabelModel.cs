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
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Collections;

using Syncfusion.Windows.Forms.Chart;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Binds ChartAxis to a data source, so as to provide labels for the axis.
    /// </summary>
    public class ChartDataBindAxisLabelModel : ChartBaseDataBindList, IChartAxisLabelModel
    {
        #region Constants
        private const int m_emptyIndex = -1;
        #endregion

        #region Members
        private string m_labelName = null;
        private string m_positionName = null;

        private PropertyDescriptor m_labelProperty = null;
        private PropertyDescriptor m_positionProperty = null;
        private ChartAxisLabel[] m_cache = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the index of the label.
        /// </summary>
        /// <value>The index of the label.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public int LabelIndex
        {
            get
            {
                return m_properties == null ? m_emptyIndex : m_properties.IndexOf(m_labelProperty);
            }

            set
            {
                this.LabelProperty = m_properties[value];
            }
        }

        /// <summary>
        /// Gets or sets the name of the label.
        /// </summary>
        /// <value>The name of the label.</value>
        [DefaultValue(null)]
        public string LabelName
        {
            get
            {
                return m_labelName;
            }

            set
            {
                if (m_labelName != value)
                {
                    m_labelName = value;
                    this.UpdateProperties();
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the position.
        /// </summary>
        /// <value>The index of the position.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public int PositionIndex
        {
            get
            {
                return m_properties == null ? m_emptyIndex : m_properties.IndexOf(m_positionProperty);
            }
            set
            {
                this.PositionProperty = m_properties[value];
            }
        }

        /// <summary>
        /// Gets or sets the name of the position.
        /// </summary>
        /// <value>The name of the position.</value>
        [DefaultValue(null)]
        public string PositionName
        {
            get
            {
                return m_positionName;
            }

            set
            {
                if (m_positionName != value)
                {
                    m_positionName = value;
                    this.UpdateProperties();
                }
            }
        }

        /// <summary>
        /// Gets or sets the label property.
        /// </summary>
        /// <value>The label property.</value>
        protected PropertyDescriptor LabelProperty
        {
            get
            {
                return m_labelProperty;
            }
            set
            {
                if (m_labelProperty != value)
                {
                    m_labelProperty = value;
                    m_cache = null;
                }
            }
        }
        /// <summary>
        /// Gets or sets the label property.
        /// </summary>
        /// <value>The label property.</value>
        protected PropertyDescriptor PositionProperty
        {
            get
            {
                return m_positionProperty;
            }
            set
            {
                if (m_positionProperty != value)
                {
                    m_positionProperty = value;
                    m_cache = null;
                }
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDataBindAxisLabelModel"/> class.
        /// </summary>
        public ChartDataBindAxisLabelModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDataBindAxisLabelModel"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public ChartDataBindAxisLabelModel(object dataSource)
            : this(dataSource, "")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDataBindAxisLabelModel"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="dataMember">The data member.</param>
        public ChartDataBindAxisLabelModel(object dataSource, string dataMember)
            : this(dataSource, dataMember, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartDataBindAxisLabelModel"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="dataMember">The data member.</param>
        /// <param name="bindingContext">The binding context.</param>
        public ChartDataBindAxisLabelModel(object dataSource, string dataMember, BindingContext bindingContext)
            : base(dataSource, dataMember, bindingContext)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns the label at the specified index.
        /// </summary>
        /// <param name="index">Index value to look for.</param>
        /// <returns>ChartAxisLabel to be used as label.</returns>
        public virtual ChartAxisLabel GetLabelAt(int index)
        {
            if (m_dataManager != null)
            {
                if (m_cache == null)
                {
                    m_cache = new ChartAxisLabel[m_dataManager.Count];
                }

                if (m_cache[index] == null)
                {
                    ChartAxisLabel label = new ChartAxisLabel();

                    label.Color = Color.Empty;
                    label.Font = null;

                    if (m_labelProperty == null)
                    {
                        label.CustomText = m_dataManager.List[index].ToString();
                    }
                    else
                    {
                        label.CustomText = m_labelProperty.GetValue(m_dataManager.List[index]).ToString();
                    }

                    label.ValueType = ChartValueType.Custom;

                    if (m_positionProperty == null)
                    {
                        label.DoubleValue = index;
                    }
                    else
                    {
                        label.DoubleValue = this.GetAppropriateValue(m_positionProperty.GetValue(m_dataManager.List[index]));
                    }

                    m_cache[index] = label;
                }

                return m_cache[index];
            }

            return null;
        }
        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.UpdateProperties();

            m_cache = null;
        }
        /// <summary>
        /// Gets the appropriate value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        private double GetAppropriateValue(object obj)
        {
            double result = 0;

            try
            {
                if (obj != DBNull.Value && obj != null)
                {
                    if (obj is DateTime)
                    {
                        result = ((DateTime)obj).ToOADate();
                    }
                    else if (obj is IConvertible)
                    {
                        result = Convert.ToDouble(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return result;
        }
        /// <summary>
        /// Updates the properties.
        /// </summary>
        private void UpdateProperties()
        {
            m_labelProperty = null;
            m_positionProperty = null;

            if (m_properties != null)
            {
                if (m_labelName != null && m_labelName != string.Empty)
                {
                    m_labelProperty = m_properties[m_labelName];
                }

                if (m_positionName != null && m_positionName != string.Empty)
                {
                    m_positionProperty = m_properties[m_positionName];
                }
            }
        }
        #endregion
    }
}
