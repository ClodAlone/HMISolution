#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for drawing semi-circles.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.RectangleToolBase"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.SemiCircle"/>
    /// </remarks>
    public class SemiCircleTool:RectangleToolBase
    {
        #region Class members
        public SemiCircleType m_type = SemiCircleType.Closed;
        private FillStyle m_fillStyle;

        #endregion

		#region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SemiCircleTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public SemiCircleTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("SemiCircleTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
		#endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value to specifies the semi-circle type.
        /// </summary>     
        public SemiCircleType SemiCircleType
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        /// <summary>
        /// Gets the properties used to fill the interior of regions.
        /// </summary>
        /// <value>The fill style.</value>
        public FillStyle FillStyle
        {
            get
            {
                if (m_fillStyle == null)
                    m_fillStyle = new FillStyle();
                return m_fillStyle;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Complete the action.
        /// </summary>
        /// <param name="rectBounding">The rectangle bounding.</param>
        /// <returns>The node.</returns>
        protected override Node CreateNode(RectangleF rectBounding)
        {
            SemiCircle semiCircle = new SemiCircle(rectBounding);
            semiCircle.Type = SemiCircleType;            
            SetFillStyle(semiCircle);
            return semiCircle;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Set the fill style to the new node.
        /// </summary>      
        protected void SetFillStyle(FilledPath node)
        {
            node.FillStyle.Color = this.FillStyle.Color;
            node.FillStyle.ForeColor = this.FillStyle.ForeColor;
            node.FillStyle.ForeColorAlphaFactor = this.FillStyle.ForeColorAlphaFactor;
            node.FillStyle.ColorAlphaFactor = this.FillStyle.ColorAlphaFactor;
            node.FillStyle.Type = this.FillStyle.Type;
            node.FillStyle.TextureWrapMode = this.FillStyle.TextureWrapMode;
            node.FillStyle.Texture = this.FillStyle.Texture;
            node.FillStyle.PathBrushStyle = this.FillStyle.PathBrushStyle;
            node.FillStyle.HatchBrushStyle = this.FillStyle.HatchBrushStyle;
            node.FillStyle.GradientAngle = this.FillStyle.GradientAngle;
            node.FillStyle.GradientCenter = this.FillStyle.GradientCenter;
        }
        #endregion
    }
}
