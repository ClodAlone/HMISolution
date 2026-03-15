#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class for drawing guides
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(GuidesConverter))]
    public class Guides : PropertyContainer
    {
        #region Members
        private bool m_bEnable = true;
        private GuideTypes m_type = GuideTypes.All;
        private float m_fMargin = 40f;
        private LineStyle m_styleLine;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Guides"/> class.
        /// </summary>
        public Guides()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Guides"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public Guides(Guides src)
            : base(src)
        {
            m_bEnable = src.m_bEnable;
            m_fMargin = src.m_fMargin;
            m_type = src.m_type;
            m_styleLine = src.m_styleLine;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Guides"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected Guides(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "GuideTypes":
                        m_type = (GuideTypes)entry.Value;
                        break;
                    case "Margin":
                        m_fMargin = float.Parse(entry.Value.ToString());
                        break;
                    case "Enable":
                        m_bEnable = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "LineStyle":
                        m_styleLine = (LineStyle)entry.Value;
                        break;
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether to show guides or not.
        /// </summary>
        [Browsable(true)]
        [Description("Enables the guides.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        public bool Enable
        {
            get { return m_bEnable; }
            set
            {
                if (value != m_bEnable)
                    m_bEnable = value;
            }
        }

        /// <summary>
        /// Gets or sets the margin between nodes.
        /// </summary>
        [Browsable(true)]
        [Description("Margin distance between two nodes")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(40f)]
        public float Margin
        {
            get { return m_fMargin; }
            set
            {
                if (value != m_fMargin)
                    m_fMargin = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of guide to be shown for nodes.
        /// </summary>
        [Browsable(true)]
        [Description("Type of guide to be shown.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(GuideTypes.All)]
        public GuideTypes Type
        {
            get { return m_type; }
            set
            {
                if (m_type != value)
                    m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets the linestyle for guides
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Line style for Guides.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LineStyle LineStyle
        {
            get
            {
                if (m_styleLine == null)
                {
                    m_styleLine = new LineStyle();
                    m_styleLine.LineColor = Color.FromArgb(74, 255, 255);
                    m_styleLine.UpdateServiceReferences(this);
                }

                return m_styleLine;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>Property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return DPN.Guides;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new Guides(this);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("GuideTypes", m_type);
            info.AddValue("Margin", m_fMargin);
            info.AddValue("Enable", m_bEnable);
            info.AddValue("LineStyle", m_styleLine);
        }
        #endregion
    }
}
