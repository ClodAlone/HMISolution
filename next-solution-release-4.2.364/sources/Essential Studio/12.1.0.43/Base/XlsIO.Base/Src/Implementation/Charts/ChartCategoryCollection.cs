#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;

#if SILVERLIGHT
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

namespace Syncfusion.XlsIO.Implementation.Charts
{
    public class ChartCategoryCollection : CollectionBaseEx<IChartCategory>
    , IChartCategories
    , ICloneParent
    , IList<IChartCategory>
       
    {
        ChartImpl m_chart;
        #region Class constructors
    /// <summary>
    /// Creates collection.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
        public ChartCategoryCollection(IApplication application, object parent)
      : base( application, parent )
    {
      m_chart = ( ChartImpl )FindParent( typeof( ChartImpl ) );

      if( m_chart == null )
        throw new Exception( "cannot find parent chart." );
    }
    #endregion
        #region IChartcategory Members
        /// <summary>
        /// Returns a single Name object from a Names collection.
        /// </summary>
        public ChartCategory this[int index]
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        public IChartCategory this[string name]
        {
            get
            {
                for (int i = 0, iLen = Count; i < iLen; i++)
                {
                    if (m_chart.Categories[i].Name == name)
                        return m_chart.Categories[i];
                }

                return null;
            }
           
        }
        #endregion
        #region Class methods
        /// <summary>
        /// Adds series to the collection.
        /// </summary>
        /// <param name="serieToAdd">Series that should be added to the collection.</param>
        /// <returns>Series that was added.</returns>
        public IChartCategory Add(ChartSerieImpl serieToAdd)
        {
            throw new ArgumentException("This method is not supported");
            
        }
        public void Remove(string serieName)
        {
            
        }
        public IChartCategory Add(string name, ExcelChartType serieType)
        {
            
            return null;
        }
        public IChartCategory Add(ExcelChartType serieType)
        {
            throw new ArgumentException("This method is not supported");
        }
        public IChartCategory Add()
        {
            ChartCategory category = new ChartCategory(Application, this);

            base.Add(category);
            return category;
        }
        /// <summary>
        /// Performs additional operations before the Clear method.
        /// </summary>
        protected override void OnClear()
        {
            base.OnClear();
        }       
        /// <summary>
        /// Clone current instance.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <returns>Returns cloned instance.</returns>
        public override object Clone(object parent)
        {
            throw new ArgumentException("This method is not supported");
        }
        /// <summary>
        /// Returns array of entered records.
        /// </summary>
        /// <param name="siIndex">Si record index.</param>
        /// <returns>Returns array of entered records.</returns>
        public List<BiffRecordRaw> GetEnteredRecords(int siIndex)
        {
            return null;
        }
        public IChartCategory Add(string name)
        {
            ChartSerieImpl serie = new ChartSerieImpl(Application, this);
            serie.Name = name;

            if (m_chart.ChartTitle == null)
                m_chart.ChartTitle = name;

            return null;
        }
        /// <summary>
        /// Gets array by si index.
        /// </summary>
        /// <param name="siIndex">Si index.</param>
        /// <returns>Returns array of arrays by si index.</returns>
        private List<List<BiffRecordRaw>> GetArrays(int siIndex)
        {
           
            return null;
        }
        public IChartCategory Add(IRange Categorylabel,IRange Values)
        {
            ChartCategory category = new ChartCategory(Application, this);
            category.CategoryLabel = Categorylabel;
            category.Values = Values;
            base.Add(category);
            return category;
        } 
        #endregion
    }
}
