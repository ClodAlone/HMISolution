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
    /// Class used for setting chart elements layout
    /// </summary>
    public class ChartLayoutImpl
        : CommonObject,
          IChartLayout
    {

        #region Class members
        /// <summary>
        /// Parent chart.
        /// </summary>
        protected ChartImpl m_book;
        /// <summary>
        /// Parent object
        /// </summary>
        protected object m_Parent;
        /// <summary>
        /// Chart interface
        /// </summary>
        protected IChart m_chart;
        /// <summary>
        /// ChartShape interface
        /// </summary>
        protected IShape m_chartShape;
        /// <summary>
        /// Manual layout
        /// </summary>
        protected IChartManualLayout m_manualLayout;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Creates chart and sets its Application and Parent
        /// properties to specified values.
        /// </summary>
        /// <param name="application">Application object for the chart.</param>
        /// <param name="parent">Parent object for the chart.</param>
        public ChartLayoutImpl( IApplication application, object parent, object chartObject )
          : this( application, parent, false, false, true )
        {

            if ((chartObject as IChart)!= null)
            {
                m_chart = (IChart)chartObject;

                if (m_chart.Parent  != null && (m_chart.Parent as IShape) != null)
                    m_chartShape = (IShape)m_chart.Parent;
            }
        }
        /// <summary>
        /// Creates chart and sets its Application and Parent
        /// properties to specified values.
        /// </summary>
        /// <param name="application">Application object for the chart.</param>
        /// <param name="parent">Parent object for the chart.</param>
        /// <param name="bSetDefaults">Indicates whether we should set defaults for fill and border properties.</param>
        public ChartLayoutImpl( IApplication application, object parent, bool bSetDefaults )
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
        public ChartLayoutImpl( IApplication application, object parent, bool bAutoSize,
          bool bIsInteriorGrey, bool bSetDefaults )
          : base( application, parent )
        {
          SetParents(parent);

          if (!Workbook.Loading && bSetDefaults)
              SetDefaultValues();
        }
        /// <summary>
        /// Creates and parses current object.
        /// </summary>
        /// <param name="application">Application object for the chart.</param>
        /// <param name="parent">Parent object for the chart.</param>
        /// <param name="data">Records storage.</param>
        /// <param name="iPos">Position in storage.</param>
        public ChartLayoutImpl(IApplication application, object parent, IList<BiffRecordRaw> data, ref int iPos)
          : base( application, parent )
        {
          SetParents(parent);
          Parse( data, ref iPos );
        }
        /// <summary>
        /// Searches for all necessary parent objects.
        /// </summary>
        private void SetParents(object parent)
        {
          m_book = FindParent( typeof( ChartImpl ) ) as ChartImpl;
          m_Parent = parent;

          if( m_book == null )
          {
            throw new ArgumentNullException( "Can't find parent chart" );
          }
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Returns parent workbook. Read-only.
        /// </summary>
        public WorkbookImpl Workbook
        {
            get
            {
                return m_book.InnerWorkbook;
            }
        }
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
        /// <summary>
        /// Gets or sets the manual layout for the layout
        /// </summary>
        public IChartManualLayout ManualLayout
        {
            get
            {
                if (m_manualLayout == null)
                    m_manualLayout = new ChartManualLayoutImpl(Workbook.Application, this);

                return m_manualLayout;
            }
            set
            {
                m_manualLayout = value;
            }
        }
        /// <summary>
        /// Gets or sets the layout target
        /// </summary>
        public LayoutTargets LayoutTarget
        {
            get
            {
                return ManualLayout.LayoutTarget;
            }
            set
            {
                ManualLayout.LayoutTarget = value;
            }
        }
        /// <summary>
        /// Gets or sets the left mode (x) value
        /// </summary>
        public LayoutModes LeftMode
        {
            get
            {
                return ManualLayout.LeftMode;
            }
            set
            {
                ManualLayout.LeftMode = value;
            }
        }
        /// <summary>
        /// Gets or sets the top mode (y) value
        /// </summary>
        public LayoutModes TopMode
        {
            get
            {
                return ManualLayout.TopMode;
            }
            set
            {
                ManualLayout.TopMode = value;
            }
        }
        /// <summary>
        /// Gets or sets the left (x) value
        /// </summary>
        public double Left
        {
            get
            {
                double val = 0;
                if (m_chart != null)
                    val = ManualLayout.Left * ((m_chartShape != null) ? m_chartShape.Width : m_chart.Width);
                else
                    val = ManualLayout.Left;

                return val;
            }
            set
            {
                if (m_chart != null)
                    ManualLayout.Left = value / ((m_chartShape != null) ? m_chartShape.Width : m_chart.Width);
                else
                    ManualLayout.Left = value;
            }
        }
        /// <summary>
        /// Gets or sets the top (y) value
        /// </summary>
        public double Top
        {
            get
            {
                double val = 0;
                if (m_chart != null)
                    val = ManualLayout.Top * ((m_chartShape != null) ? m_chartShape.Height : m_chart.Height);
                else
                    val = ManualLayout.Top;

                return val;
            }
            set
            {
                if (m_chart != null)
                    ManualLayout.Top = value / ((m_chartShape != null) ? m_chartShape.Height : m_chart.Height);
                else
                    ManualLayout.Top = value;
            }
        }
        /// <summary>
        /// Gets or sets the Width mode
        /// </summary>
        public LayoutModes WidthMode
        {
            get
            {
                return ManualLayout.WidthMode;
            }
            set
            {
                ManualLayout.WidthMode = value;
            }
        }
        /// <summary>
        /// Gets or sets the Height mode
        /// </summary>
        public LayoutModes HeightMode
        {
            get
            {
                return ManualLayout.HeightMode;
            }
            set
            {
                ManualLayout.HeightMode = value;
            }
        }
        /// <summary>
        /// Gets or sets the Width
        /// </summary>
        public double Width
        {
            get
            {
                double val = 0;
                if (m_chart != null)
                    val = ManualLayout.Width * ((m_chartShape != null) ? m_chartShape.Width : m_chart.Width);
                else
                    val = ManualLayout.Width;

                return val;
            }
            set
            {
                if (m_chart != null)
                    ManualLayout.Width = value / ((m_chartShape != null) ? m_chartShape.Width : m_chart.Width);
                else
                    ManualLayout.Width = value;
            }
        }
        /// <summary>
        /// Gets or sets the Height
        /// </summary>
        public double Height
        {
            get
            {
                double val = 0;
                if (m_chart != null)
                    val = ManualLayout.Height * ((m_chartShape != null) ? m_chartShape.Height : m_chart.Height);
                else
                    val = ManualLayout.Height;

                return val;
            }
            set
            {
                if (m_chart != null)
                    ManualLayout.Height = value / ((m_chartShape != null) ? m_chartShape.Height : m_chart.Height);
                else
                    ManualLayout.Height = value;
            }
        }
        #endregion

        #region Class parse / serialize methods
        /// <summary>
        /// Parses frame.
        /// </summary>
        /// <param name="data">Array with frame records.</param>
        /// <param name="iPos">Position of the frame records.</param>
        [CLSCompliant(false)]
        public void Parse(IList<BiffRecordRaw> data, ref int iPos)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (iPos < 0 || iPos > data.Count)
                throw new ArgumentOutOfRangeException("iPos", "Value cannot be less than 0 and greater than data.Count");

            BiffRecordRaw record = (BiffRecordRaw)data[iPos];
            record = UnwrapRecord(record);
            record.CheckTypeCode(TBIFFRecord.ChartFrame);
            //m_chartFrame = (ChartFrameRecord)record;

            //iPos++;
            //record = (BiffRecordRaw)data[iPos];

            //int iBeginCounter = 0;


            //if (CheckBegin(record))
            //{
            //    iBeginCounter++;

            //    do
            //    {
            //        iPos++;
            //        record = (BiffRecordRaw)data[iPos];
            //        ParseRecord(record, ref iBeginCounter);
            //    }
            //    while (iBeginCounter != 0);

            //    iPos++;
            //}
        }
        /// <summary>
        /// Checks whether specified record is begin.
        /// </summary>
        /// <param name="record">Record to check.</param>
        /// <returns>True if this is begin; false otherwise.</returns>
        [CLSCompliant(false)]
        protected virtual bool CheckBegin(BiffRecordRaw record)
        {
            if (record == null)
                throw new ArgumentNullException("record");

            return (record.TypeCode == TBIFFRecord.Begin);
        }
        /// <summary>
        /// Parses single record.
        /// </summary>
        /// <param name="record">Record to parse.</param>
        /// <param name="iBeginCounter">Number of not closed begin record.</param>
        [CLSCompliant(false)]
        protected virtual void ParseRecord(BiffRecordRaw record, ref int iBeginCounter)
        {
            if (record == null)
                throw new ArgumentNullException("record");

            switch (record.TypeCode)
            {
                case TBIFFRecord.Begin:
                    iBeginCounter++;
                    break;

                case TBIFFRecord.End:
                    iBeginCounter--;
                    break;

                //case TBIFFRecord.ChartAreaFormat:
                //    m_interior = new ChartInteriorImpl(Application, this, (ChartAreaFormatRecord)record);
                //    break;

                //case TBIFFRecord.ChartLineFormat:
                //    m_border = new ChartBorderImpl(Application, this, (ChartLineFormatRecord)record);
                //    break;

                //case TBIFFRecord.ChartGelFrame:
                //    m_fill = new ChartFillImpl(Application, this, (ChartGelFrameRecord)record);
                //    break;
            }
        }
        /// <summary>
        /// Saves chart frame as biff records.
        /// </summary>
        /// <param name="records">OffsetArrayList that will get biff records.</param>
        [CLSCompliant(false)]
        public void Serialize(IList<IBiffStorage> records)
        {
            if (records == null)
                throw new ArgumentNullException("records");

            //SerializeRecord(records, m_chartFrame);
            //SerializeRecord(records, BiffRecordFactory.GetRecord(TBIFFRecord.Begin));

            //if (m_border != null)
            //    m_border.Serialize(records);

            //if (m_interior != null)
            //    m_interior.Serialize(records);

            //if (m_fill != null)
            //    m_fill.Serialize(records);

            //SerializeRecord(records, BiffRecordFactory.GetRecord(TBIFFRecord.End));
        }
        /// <summary>
        /// Serializes single record.
        /// </summary>
        /// <param name="list">OffsetArrayList that will get biff records.</param>
        /// <param name="record">Record to serialize.</param>
        [CLSCompliant(false)]
        protected virtual void SerializeRecord(IList<IBiffStorage> list, BiffRecordRaw record)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (record == null)
                throw new ArgumentNullException("record");

            list.Add((BiffRecordRaw)record.Clone());
        }
        /// <summary>
        /// Unwraps record.
        /// </summary>
        /// <param name="record">Record to unwrap.</param>
        /// <returns>Unwrapped record.</returns>
        [CLSCompliant(false)]
        protected virtual BiffRecordRaw UnwrapRecord(BiffRecordRaw record)
        {
            return record;
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
