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

#region file using directives
using System;
using System.Collections;
using System.Collections.Generic;
using Syncfusion.XlsIO.Parser.Biff_Records;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
	/// <summary>
	/// Wrapper over conditional format collection for range.
	/// </summary>
	public class CondFormatCollectionWrapper
    : CommonWrapper
    , IConditionalFormats
	{
    #region Class members
    /// <summary>
    /// Wrapped range object.
    /// </summary>
    private ICombinedRange m_range;
    /// <summary>
    /// Wrapped conditional formats collection.
    /// </summary>
    private ConditionalFormats m_condFormats;
    /// <summary>
    /// List with all wrapped conditions.
    /// </summary>
    private List<IConditionalFormat> m_arrConditions = new List<IConditionalFormat>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent creation without arguments.
    /// </summary>
    private CondFormatCollectionWrapper()
    {
    }
    /// <summary>
    /// Creates wrapper for the specified range object.
    /// </summary>
    /// <param name="range">Range to wrap conditional formats for.</param>
    public CondFormatCollectionWrapper( ICombinedRange range )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      m_range = range;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public override void BeginUpdate()
    {
      base.BeginUpdate();

      if( BeginCallsCount == 1 )
      {
        // Here we have to remove ranges from all conditional formats collection.
        // Then create new conditional formats collection that will be wrapped and
        // will be used for getting and setting value.
        CreateReadOnlyFormats();
        m_range.ClearConditionalFormats();
        CreateWriteableFormats();
      }
    }

    /// <summary>
    /// This method should be called after several updates to the object.
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();

      if( BeginCallsCount == 0 )
      {
        // Here we have to add wrapped collection to the worksheet's collection
        // with all CondFormatCollections (this can cause new collection creation, or
        // range can be added to the old one.
        m_condFormats = SheetFormats.Add( m_condFormats );
      }
    }

    #endregion

    #region IConditionalFormats Members
    /// <summary>
    /// Returns number of elements in the collection. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        CreateReadOnlyFormats();
        return m_condFormats.Count;
      }
    }

    /// <summary>
    /// Returns single element from the collection. Read-only.
    /// </summary>
    public IConditionalFormat this[ int index ]
    {
      get
      {
        CreateReadOnlyFormats();          
        ((ConditionalFormatWrapper)m_arrConditions[index]).Range = m_range;
        return m_arrConditions[ index ];
      }
    }

    /// <summary>
    /// Adds new condition to the collection.
    /// </summary>
    /// <returns>Newly added condition.</returns>
    public IConditionalFormat AddCondition()
    {
      BeginUpdate();

      try
      {
        IConditionalFormat format =  m_condFormats.AddCondition();
        format = new ConditionalFormatWrapper( this, Count - 1 );
        ((ConditionalFormatWrapper)format).Range = m_range;
        m_arrConditions.Add( format );
        return format;
      }
      finally
      {
        EndUpdate();
      }
    }
    /// <summary>
    /// Removes the Condtional Format at the specified range
    /// </summary>
    public void Remove()
    {
        if (m_range.ConditionalFormats == null)
            throw new ArgumentNullException("Conditional Format");

        ((WorksheetImpl)this.m_range.Worksheet).ConditionalFormats.Remove(m_range.GetRectangles());
    }
    /// <summary>
    /// Removes the Condtional Format at the Specified Index
    /// </summary>
    public void RemoveAt(int index)
    {
        if (m_range.ConditionalFormats == null)
            throw new ArgumentNullException("Conditional Format");

        ((WorksheetImpl)this.m_range.Worksheet).ConditionalFormats.RemoveAt(index);
    } 

    #endregion

    #region IEnumerable Members
    /// <summary>
    /// Returns an enumerator that can iterate through a collection.
    /// </summary>
    /// <returns>An enumerator that can iterate through a collection</returns>
    public IEnumerator GetEnumerator()
    {
      return m_arrConditions.GetEnumerator();
    }

    #endregion

    #region IParentApplication Members
    /// <summary>
    /// Application object for this object.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_range.Application;
      }
    }

    /// <summary>
    /// Parent object for this object.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_range;
      }
    }

    #endregion

    #region Class methods
    /// <summary>
    /// Creates conditional formats for read-only mode.
    /// </summary>
    private void CreateReadOnlyFormats()
    {
      if( m_condFormats == null )
      {
        WorksheetConditionalFormats sheetFormats = SheetFormats;

        // NOTE: here we can optimize a little bit if we will cache rectangles.
        m_condFormats = sheetFormats.Find( m_range.GetRectangles() );

        if( m_condFormats == null )
        {
          m_condFormats = new ConditionalFormats( Application, sheetFormats );
        }

        CreateConditionWrappers();
      }
    }
    /// <summary>
    /// Creates copy of the current conditional formats collection to enable writing.
    /// </summary>
    private void CreateWriteableFormats()
    {
      if( m_condFormats == null )
      {
        CreateReadOnlyFormats();
      }

      // NOTE: here we can add some optimization to prevent creation of two empty collections
      // in case of range without conditional format.
      m_condFormats = new ConditionalFormats( Application, m_range, m_condFormats );
      m_condFormats.ClearCells();
      m_condFormats.AddRange( m_range );
      m_condFormats.EnclosedRange = new TAddr(m_range.Row - 1, m_range.Column - 1, m_range.LastRow - 1, m_range.LastColumn - 1);
    }
    /// <summary>
    /// Creates wrappers for all conditions.
    /// </summary>
    private void CreateConditionWrappers()
    {
      for( int i = 0, len = m_condFormats.Count; i < len; i++ )
      {
        m_arrConditions.Add( new ConditionalFormatWrapper( this, i ) );
      }
    }
    /// <summary>
    /// Returns unwrapped condition.
    /// </summary>
    /// <param name="iCondition">Condition index.</param>
    /// <returns>Unwrapped condition.</returns>
    public ConditionalFormatImpl GetCondition( int iCondition )
    {
      return m_condFormats[ iCondition ] as ConditionalFormatImpl;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns collection of worksheets conditional formats of
    /// the parent worksheet. Read-only.
    /// </summary>
    private WorksheetConditionalFormats SheetFormats
    {
      get
      {
        WorksheetImpl sheet = ( WorksheetImpl )m_range.Worksheet;
        return sheet.ConditionalFormats;
      }
    }
	 /// <summary>
   /// Gets the Conditional Formats.
   /// </summary>
   internal ConditionalFormats ConditionalFormats
   {
       get
       {
           return m_condFormats;
       }
   }
   /// <summary>
   /// Gets the range object.
   /// </summary>
   internal IRange Range
   {
       get
       {
           return m_range;
       }
   }
    #endregion
  }
}
