#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
    /// <summary>
    /// Class used for setting chart elements layout manually
    /// </summary>
    public class ChartManualLayoutImpl
        : CommonObject,
          IChartManualLayout
    {
        #region Class members
        /// <summary>
        /// Parent chart.
        /// </summary>
        protected ChartLayoutImpl m_layout;
        ///         /// <summary>
        /// Parent object
        /// </summary>
        protected object m_Parent;
        private ChartAttachedLabelLayoutRecord m_atachedLabelLayout;
        private ChartPlotAreaLayoutRecord m_plotAreaLayout;
        protected LayoutTargets m_layoutTarget;
        protected LayoutModes m_leftMode;
        protected LayoutModes m_topMode;
        protected double m_left;
        protected double m_top;
        protected double m_dX;
        protected double m_dY;
        protected LayoutModes m_widthMode;
        protected LayoutModes m_heightMode;
        protected double m_width;
        protected double m_height;
        protected int m_xTL;
        protected int m_yTL;
        protected int m_xBR;
        protected int m_yBR;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Creates chart and sets its Application and Parent
        /// properties to specified values.
        /// </summary>
        /// <param name="application">Application object for the chart.</param>
        /// <param name="parent">Parent object for the chart.</param>
        public ChartManualLayoutImpl( IApplication application, object parent )
          : this( application, parent, false, false, true )
        {
        }
        /// <summary>
        /// Creates chart and sets its Application and Parent
        /// properties to specified values.
        /// </summary>
        /// <param name="application">Application object for the chart.</param>
        /// <param name="parent">Parent object for the chart.</param>
        /// <param name="bSetDefaults">Indicates whether we should set defaults for fill and border properties.</param>
        public ChartManualLayoutImpl( IApplication application, object parent, bool bSetDefaults )
          : this( application, parent, false, false, bSetDefaults )
        {
        }
        /// <summary>
        /// Creates chart and sets its Application and Parent
        /// properties to specified values.
        /// </summary>
        /// <param name="application">Application object for the chart.</param>
        /// <param name="parent">Parent object for the chart.</param>
        /// <param name="bAutoSize">Indicates is auto size.</param>
        /// <param name="bIsInteriorGrey">Indicates is interior is gray.</param>
        /// <param name="bSetDefaults">Indicates whether we should set defaults for fill and border properties.</param>
        public ChartManualLayoutImpl( IApplication application, object parent, bool bAutoSize,
          bool bIsInteriorGrey, bool bSetDefaults )
          : base( application, parent )
        {
          SetParents(parent);

          //if (!Workbook.Loading && bSetDefaults)
          //    SetDefaultValues();
        }
        /// <summary>
        /// Creates and parses current object.
        /// </summary>
        /// <param name="application">Application object for the chart.</param>
        /// <param name="parent">Parent object for the chart.</param>
        /// <param name="data">Records storage.</param>
        /// <param name="iPos">Position in storage.</param>
        public ChartManualLayoutImpl(IApplication application, object parent, IList<BiffRecordRaw> data, ref int iPos)
          : base( application, parent )
        {
          SetParents(parent);
        }
        /// <summary>
        /// Searches for all necessary parent objects.
        /// </summary>
        private void SetParents(object parent)
        {
            m_layout = FindParent(typeof(ChartLayoutImpl)) as ChartLayoutImpl;
            m_Parent = parent;

            if( m_layout == null )
            {
              throw new ArgumentNullException( "Can't find parent chart" );
            }
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Return the parent object. Read-only.
        /// </summary>
        public object Parent
        {
            get
            {
                return m_Parent;
            }
        }
        public ChartAttachedLabelLayoutRecord AttachedLabelLayout
        {
            get
            {
                if (m_atachedLabelLayout == null)
                    m_atachedLabelLayout = new ChartAttachedLabelLayoutRecord();
                return m_atachedLabelLayout;
            }
            set
            {
                m_atachedLabelLayout = value;
            }
        }
        public ChartPlotAreaLayoutRecord PlotAreaLayout
        {
            get
            {
                if (m_plotAreaLayout == null)
                    m_plotAreaLayout = new ChartPlotAreaLayoutRecord();
                return m_plotAreaLayout;
            }
            set
            {
                m_plotAreaLayout = value;
            }
        }
        /// <summary>
        /// Gets or sets the layout target
        /// </summary>
        public LayoutTargets LayoutTarget
        {
            get
            {
                return m_layoutTarget;
            }
            set
            {
                m_layoutTarget = value;
            }
        }
        /// <summary>
        /// Gets or sets the left mode (x) value
        /// </summary>
        public LayoutModes LeftMode
        {
            get
            {
                return m_leftMode;
            }
            set
            {
                m_leftMode = value;

                if (Parent is ChartLayoutImpl)
                {
                    if ((Parent as ChartLayoutImpl).Parent is ChartTextAreaImpl || (Parent as ChartLayoutImpl).Parent is ChartLegendImpl)
                    {
                        AttachedLabelLayout.WXMode = value;
                    }
                    else if ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl)
                    {
                        PlotAreaLayout.WXMode = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the top mode (y) value
        /// </summary>
        public LayoutModes TopMode
        {
            get
            {
                return m_topMode;
            }
            set
            {
                m_topMode = value;

                if (Parent is ChartLayoutImpl)
                {
                    if ((Parent as ChartLayoutImpl).Parent is ChartTextAreaImpl || (Parent as ChartLayoutImpl).Parent is ChartLegendImpl)
                    {
                        AttachedLabelLayout.WYMode = value;
                    }
                    else if ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl)
                    {
                        PlotAreaLayout.WYMode = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the left (x) value
        /// </summary>
        public double Left
        {
            get
            {
                return m_left;
            }
            set
            {
                m_left = value;

                if (Parent is ChartLayoutImpl)
                {
                    if ((Parent as ChartLayoutImpl).Parent is ChartTextAreaImpl || (Parent as ChartLayoutImpl).Parent is ChartLegendImpl)
                    {
                        AttachedLabelLayout.X = value;
                    }
                    else if ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl)
                    {
                        PlotAreaLayout.X = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the top (y) value
        /// </summary>
        public double Top
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;

                if (Parent is ChartLayoutImpl)
                {
                    if ((Parent as ChartLayoutImpl).Parent is ChartTextAreaImpl || (Parent as ChartLayoutImpl).Parent is ChartLegendImpl)
                    {
                        AttachedLabelLayout.Y = value;
                    }
                    else if ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl)
                    {
                        PlotAreaLayout.Y = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the dx value
        /// </summary>
        public double dX
        {
            get
            {
                return m_dX;
            }
            set
            {
                m_dX = value;
                if (Parent is ChartLayoutImpl)
                {
                    if ((Parent as ChartLayoutImpl).Parent is ChartTextAreaImpl || (Parent as ChartLayoutImpl).Parent is ChartLegendImpl)
                    {
                        AttachedLabelLayout.Dx = value;
                    }
                    else if ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl)
                    {
                        PlotAreaLayout.Dx = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the dy value
        /// </summary>
        public double dY
        {
            get
            {
                return m_dY;
            }
            set
            {
                m_dY = value;
                if (Parent is ChartLayoutImpl)
                {
                    if ((Parent as ChartLayoutImpl).Parent is ChartTextAreaImpl || (Parent as ChartLayoutImpl).Parent is ChartLegendImpl)
                    {
                        AttachedLabelLayout.Dy = value;
                    }
                    else if ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl)
                    {
                        PlotAreaLayout.Dy = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the Width mode
        /// </summary>
        public LayoutModes WidthMode
        {
            get
            {
                return m_widthMode;
            }
            set
            {
                m_widthMode = value;

                if (Parent is ChartLayoutImpl)
                {
                    if ((Parent as ChartLayoutImpl).Parent is ChartTextAreaImpl || (Parent as ChartLayoutImpl).Parent is ChartLegendImpl)
                    {
                        AttachedLabelLayout.WWidthMode = value;
                    }
                    else if ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl)
                    {
                        PlotAreaLayout.WWidthMode = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the Height mode
        /// </summary>
        public LayoutModes HeightMode
        {
            get
            {
                return m_heightMode;
            }
            set
            {
                m_heightMode = value;

                if (Parent is ChartLayoutImpl)
                {
                    if ((Parent as ChartLayoutImpl).Parent is ChartTextAreaImpl || (Parent as ChartLayoutImpl).Parent is ChartLegendImpl)
                    {
                        AttachedLabelLayout.WHeightMode = value;
                    }
                    else if ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl)
                    {
                        PlotAreaLayout.WHeightMode = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the Width
        /// </summary>
        public double Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }
        /// <summary>
        /// Gets or sets the Height
        /// </summary>
        public double Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }
        /// <summary>
        /// Gets or sets the xTL value
        /// </summary>
        public int xTL
        {
            get
            {
                return m_xTL;
            }
            set
            {
                m_xTL = value;

                if (Parent is ChartLayoutImpl && ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl))
                {
                        PlotAreaLayout.xTL = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the yTL value
        /// </summary>
        public int yTL
        {
            get
            {
                return m_yTL;
            }
            set
            {
                m_yTL = value;

                if (Parent is ChartLayoutImpl && ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl))
                {
                    PlotAreaLayout.yTL = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the xBR value
        /// </summary>
        public int xBR
        {
            get
            {
                return m_xBR;
            }
            set
            {
                m_xBR = value;

                if (Parent is ChartLayoutImpl && ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl))
                {
                    PlotAreaLayout.xBR = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the yBR value
        /// </summary>
        public int yBR
        {
            get
            {
                return m_yBR;
            }
            set
            {
                m_yBR = value;

                if (Parent is ChartLayoutImpl && ((Parent as ChartLayoutImpl).Parent is ChartPlotAreaImpl))
                {
                    PlotAreaLayout.yBR = value;
                }
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Set variable to the default state.
        /// </summary>
        /// <param name="bAutoSize">Indicates whether MS Excel should calculate size of the frame.</param>
        /// <param name="bIsInteriorGray">Indicates is default interior is gray.</param>
        public void SetDefaultValues()
        {
           
        }
        #endregion
    }
}
