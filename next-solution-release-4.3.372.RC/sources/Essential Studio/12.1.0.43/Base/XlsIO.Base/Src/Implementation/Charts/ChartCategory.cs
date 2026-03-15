#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation.Charts
{
    public class ChartCategory: CommonObject
    , IChartCategory
    {
        private ChartSeriesCollection m_series;
        private bool m_isfiltered;
        private IRange m_categoryLabel = null;
        private IRange m_Value = null;
        private WorkbookImpl m_book;
        private ChartImpl m_chart;
        private ChartSeriesCollection series;
        private ChartCategoryCollection m_categoryColl;
        public bool Filter_customize;
        private string m_categoryName;
        public ChartCategory(IApplication application, object parent)
      : base( application, parent )
    {
      SetParents();
      InitializeCollections();
      
    }
   private void InitializeCollections()
    {
      
    }       

        private void SetParents()                    
    {
      object parent = FindParent( typeof( WorkbookImpl ) );

      if( parent == null )
        throw new ArgumentNullException( "Can't find parent workbook." );

      m_book = (WorkbookImpl) parent;

      parent = FindParent( typeof( ChartImpl ) );

      if( parent == null )
        throw new ArgumentNullException( "Can't find parent chart." );

      m_chart = (ChartImpl) parent;

      parent = FindParent( typeof( ChartCategoryCollection ) );

      if( parent == null )
        throw new ArgumentNullException( "Can't find parent series collection." );

      m_categoryColl = (ChartCategoryCollection) parent;
    }
        /// <summary>
        /// Represents the Filterd Category
        /// </summary>
        public bool IsFiltered
        {
            get
            {
                return m_isfiltered;
            }
            set
            {                
                m_isfiltered = value;
                if (!m_book.Loading)
                    Filter_customize = true;
            }
        }  
        /// <summary>
        /// Represents the category labels
        /// </summary>
        public IRange CategoryLabel
        {
            get
            {
                return m_categoryLabel;
            }
            set
            { 
                m_categoryLabel = value;               
            }
        }
        /// <summary>
        /// Represents the category values
        /// </summary>
        public IRange Values
        {
            get
            {
                return m_Value;
            }
            set
            {
                    m_Value = value;
            }
        }
        /// <summary>
        /// Represents the categoryname.
        /// </summary>
        public string Name
        {
            get
            {
                return m_categoryName;
            }
            set
            {
                m_categoryName = value;
            }
            
        }
        

    }

}
