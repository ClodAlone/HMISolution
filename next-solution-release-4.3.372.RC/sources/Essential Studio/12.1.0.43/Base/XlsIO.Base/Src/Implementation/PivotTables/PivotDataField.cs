#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
  public class PivotDataField :
    IPivotDataField,
    ICloneParent
  {
    #region Members
    /// <summary>
    /// Name of the data field.
    /// </summary>
    private string m_strName;
    /// <summary>
    /// Subtotal function used for data field.
    /// </summary>
    private PivotSubtotalTypes m_subtotal;
    /// <summary>
    /// Base field.
    /// </summary>
    private PivotFieldImpl m_field;
        /// <summary>
        /// 
        /// </summary>
        private PivotFieldDataFormat m_showDataAs = PivotFieldDataFormat.Normal;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<PivotFieldDataFormat, string> m_showDataCollections;
        private List<PivotFieldDataFormat> m_excel2010Data;
        /// <summary>
        /// 
        /// </summary>
        private int m_baseItem;
        /// <summary>
        /// 
        /// </summary>
        private int m_baseField = 0;
    #endregion

    #region Properties
    /// <summary>
    /// Gets / sets name of the data field.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        if( value == null || value.Length == 0 )
          throw new ArgumentOutOfRangeException( "value" );

        m_strName = value;
      }
    }
    /// <summary>
    /// Gets/ sets subtotal function used for data field.
    /// </summary>
    public PivotSubtotalTypes Subtotal
    {
      get
      {
        return m_subtotal;
      }
      set
      {
        m_subtotal = value;
      }
    }
    /// <summary>
    /// Gets parent field. Read-only.
    /// </summary>
    public PivotFieldImpl Field
    {
      get
      {
        return m_field;
      }
    }
      /// <summary>
        /// Gets or sets the show data as.
        /// </summary>
        /// <value>The show data as.</value>
        public PivotFieldDataFormat ShowDataAs
        {
            get
            {
                return m_showDataAs;
            }
            set
            {
                m_showDataAs = value;
            }
        }
        /// <summary>
        /// Gets or sets the base item.
        /// </summary>
        /// <value>The base item.</value>
        public int BaseItem
        {
            get
            {
                return m_baseItem;
            }
            set
            {
                m_baseItem = value;
           }
        }
        /// <summary>
        /// Gets or sets the base field.
        /// </summary>
        /// <value>The base field.</value>
        public int BaseField
        {
            get
            {
                return m_baseField;
            }
           set
            {
                m_baseField = value;
            }
       }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the data field.
    /// </summary>
    /// <param name="name">Name of the data field.</param>
    /// <param name="subtotal">Subtotal function.</param>
    /// <param name="parentField">Parent field.</param>
    public PivotDataField( string name, PivotSubtotalTypes subtotal, PivotFieldImpl parentField )
    {
      if( name == null || name.Length == 0 )
        throw new ArgumentOutOfRangeException( "name" );

      if( parentField == null )
        throw new ArgumentNullException( "parentField" );

      m_strName = name;
      m_subtotal = subtotal;
      m_field = parentField;

      parentField.IsDataField = true;

      if (parentField.Axis == PivotAxisTypes.None)
          parentField.Axis = PivotAxisTypes.Data;

      InitializeShowData();
      InitializeExcel2010Data();
    }

    /// <summary>
    /// Initializes the show data.
    /// </summary>
    private void InitializeShowData()
    {
        if (m_showDataCollections != null)
            return;

        m_showDataCollections = new Dictionary<PivotFieldDataFormat, string>();
        m_showDataCollections.Add(PivotFieldDataFormat.Difference, "difference");
        m_showDataCollections.Add(PivotFieldDataFormat.Index, "index");
        m_showDataCollections.Add(PivotFieldDataFormat.Normal, "normal");
        m_showDataCollections.Add(PivotFieldDataFormat.Percent, "percent");
        m_showDataCollections.Add(PivotFieldDataFormat.PercentageOfDifference, "percentDiff");
        m_showDataCollections.Add(PivotFieldDataFormat.PercentageOfColumn, "percentOfCol");
        m_showDataCollections.Add(PivotFieldDataFormat.PercentageOfRow, "percentOfRow");
        m_showDataCollections.Add(PivotFieldDataFormat.PercentageOfTotal, "percentOfTotal");
        m_showDataCollections.Add(PivotFieldDataFormat.RunTotal, "runTotal");
        m_showDataCollections.Add(PivotFieldDataFormat.PercentageOfParentColumn, "percentOfParentCol");
        m_showDataCollections.Add(PivotFieldDataFormat.PercentageOfParentRow, "percentOfParentRow");
        m_showDataCollections.Add(PivotFieldDataFormat.RankAscending, "rankAscending");
        m_showDataCollections.Add(PivotFieldDataFormat.RankDecending, "rankDescending");
        m_showDataCollections.Add(PivotFieldDataFormat.PercentageOfRunningTotal, "percentOfRunningTotal");
    }
    #endregion
      
      #region Helper methods
    /// <summary>
    /// Initializes the excel2010 data.
    /// </summary>
        private void InitializeExcel2010Data()
        {
            if (m_excel2010Data != null)
                return;

            m_excel2010Data = new List<PivotFieldDataFormat>();

            m_excel2010Data.Add(PivotFieldDataFormat.PercentageOfParentColumn);
            m_excel2010Data.Add(PivotFieldDataFormat.PercentageOfParentRow);
            m_excel2010Data.Add(PivotFieldDataFormat.RankAscending);
            m_excel2010Data.Add(PivotFieldDataFormat.RankDecending);
            m_excel2010Data.Add(PivotFieldDataFormat.PercentageOfRunningTotal);
        }

        /// <summary>
        /// Sets the show data.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal PivotFieldDataFormat SetShowData(string value)
        {
            foreach (KeyValuePair<PivotFieldDataFormat, string> data in m_showDataCollections)
            {
                if (data.Value == value)
                    return data.Key;
            }
            return PivotFieldDataFormat.Normal;
        }
        /// <summary>
        /// Gets the show data.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        internal string GetShowData(PivotFieldDataFormat value)
       {
            return m_showDataCollections[value];
       }
        /// <summary>
        /// Determines whether [is excel2010 data].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is excel2010 data]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsExcel2010Data()
        {
            return (Array.IndexOf(m_excel2010Data.ToArray(), ShowDataAs) >= 0);
        }
        #endregion
    

    #region ICloneParent Members

    public object Clone( object parent )
    {
      PivotDataField result = ( PivotDataField )MemberwiseClone();
      PivotDataFields fields = ( PivotDataFields )CommonObject.FindParent( parent, typeof( PivotDataFields ) );
      PivotTableImpl table = ( PivotTableImpl )CommonObject.FindParent( parent, typeof( PivotTableImpl ) );
      result.m_field = ( PivotFieldImpl )table.Fields[ m_field.Name ];
      return result;
    }

    #endregion
  }
}
