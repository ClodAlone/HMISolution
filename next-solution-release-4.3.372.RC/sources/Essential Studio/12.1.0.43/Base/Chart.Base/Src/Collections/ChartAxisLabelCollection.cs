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
using System.Diagnostics;
using System.Drawing;
using Syncfusion.Documentation;
using System.Drawing.Drawing2D;
using System.ComponentModel;

#endregion

namespace Syncfusion.Windows.Forms.Chart
{

  /// <summary>
  ///    Interface that needs to be implemented to display custom axis labels. 
  /// </summary>
  public interface IChartAxisLabelModel
  {
    /// <summary>
    ///     Returns the label at the specified index.
    /// </summary>
    /// <param name="index" type="int">
    ///     <para>
    ///     Index value to look for.    
    ///     </para>
    /// </param>
    /// <returns>
    ///     ChartAxisLabel to be used as label.
    /// </returns>
    ChartAxisLabel GetLabelAt( int index );

    /// <summary>
    /// Returns the number of labels.    
    /// </summary>
    int Count { get; }
  }
	
	/// <summary>
  /// Collection of custom ChartAxis labels. These labels will be used as axis labels when
  /// the value type of the ChartAxis is set to custom.
  /// <seealso cref="ChartValueType"/>
  /// </summary>
  [TypeConverter(typeof(CollectionConverter))]
  public class ChartAxisLabelCollection 
    : CollectionBase
    , IChartAxisLabelModel
  {
    #region Class properties
#if SyncfusionFramework2_0
    // Bug - OT d7114 / TS #11542
    /// <summary>
    /// Gets or sets the number of elements that the <see cref="T:System.Collections.CollectionBase"></see> can contain.
    /// </summary>
    /// <value></value>
    /// <returns>The number of elements that the <see cref="T:System.Collections.CollectionBase"></see> can contain.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><see cref="P:System.Collections.CollectionBase.Capacity"></see> is set to a value that is less than <see cref="P:System.Collections.CollectionBase.Count"></see>.</exception>
    /// <exception cref="T:System.OutOfMemoryException">There is not enough memory available on the system.</exception>
    [ Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ) ]
    new public int Capacity
    {
      get
      {
        return base.Capacity;
      }
      set
      {
        base.Capacity = value;
      }
    }
#endif
    /// <summary>
    /// Returns the axis label at the specified index value.
    /// </summary>
    public ChartAxisLabel this[ int index ]
    {
      get
      {
        if( index < 0 || index >= List.Count )
        {
          throw new IndexOutOfRangeException(String.Format( "Invalid Index {0} items count {1}", index, List.Count ) );
        }

        return this.List[ index ] as ChartAxisLabel;
      }
    }
    #endregion
    
    #region Class events
    /// <summary>
    /// Occurs when a custom axis label was changed.
    /// </summary>
    public event EventHandler Changed;

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent collection construction without ChartAxis reference.
    /// </summary>
    public ChartAxisLabelCollection()
    {
    }
    #endregion
    
    #region Class Public Methods
    /// <summary>
    ///     Looks up this collection and returns the index value of the specified label.
    /// </summary>
    /// <param name="label" type="ChartAxisLabel">
    ///     <para>
    ///      Label to look for in this collection.   
    ///     </para>
    /// </param>
    /// <returns>
    ///     The index value of the label if the look up is successful; -1 otherwise.
    /// </returns>
    public int IndexOf( ChartAxisLabel label )
    {
      return InnerList.IndexOf( label );
    }

    /// <summary>
    ///     Adds the specified label to this collection.
    /// </summary>
    /// <param name="label" type="ChartAxisLabel">
    ///     <para>
    ///     An instance of the label that is to be to add.    
    ///     </para>
    /// </param>
    public void Add( ChartAxisLabel label )
    {

      if( label == null )
        throw new ArgumentNullException( "label" );

      if( this.List.IndexOf( label ) == -1 )
      {
        this.List.Add( label );
      }
    }

    /// <summary>
    ///     Inserts the specified label at the specified index.
    /// </summary>
    /// <param name="index" type="int">
    ///     <para>
    ///     Index value where the label that is to be inserted.   
    ///     </para>
    /// </param>
    /// <param name="label" type="ChartAxisLabel">
    ///     <para>
    ///     An instance of the label that is to be added.    
    ///     </para>
    /// </param>
    public void Insert( int index, ChartAxisLabel label )
    {

      if( index < 0 )
        throw new ArgumentOutOfRangeException( "index", index, "Value can not be less than 0." );
      
      if( label == null )
        throw new ArgumentNullException( "label" );

      this.List.Insert( index, label );
    }

    /// <summary>
    ///     Removes the specified label from this collection.
    /// </summary>
    /// <param name="label" type="ChartAxisLabel">
    ///     <para>
    ///      Label that is to be removed.   
    ///     </para>
    /// </param>
    public void Remove( ChartAxisLabel label )
    {
      this.List.Remove( label );
    }

    /// <summary>
    ///     Gets the label at the specified index.
    /// </summary>
    /// <param name="index" type="int">
    ///     <para>
    ///      The index value to look for.   
    ///     </para>
    /// </param>
    /// <returns>
    ///     The ChartAxis label at the specified index.
    /// </returns>
    public ChartAxisLabel GetLabelAt( int index )
    {
      return this[ index ];
    }
  
    #endregion

    #region Class overrides

		/// <summary>
		/// Overriden. <see cref="System.Collections.CollectionBase"/>
		/// </summary>
    [ DocumentationExclude() ]
    protected override void OnClearComplete()
    {
      BroadcastChange();
      base.OnClearComplete();
    }
    /// <summary>
    /// Overriden. <see cref="System.Collections.CollectionBase"/>
    /// </summary>
    [ DocumentationExclude() ]
    protected override void OnInsertComplete( int index, object value )
    {
      BroadcastChange();
      base.OnInsertComplete( index, value );
    }
    /// <summary>
    /// Overriden. <see cref="System.Collections.CollectionBase"/>
    /// </summary>
    [ DocumentationExclude() ]
    protected override void OnRemoveComplete( int index, object value )
    {
      BroadcastChange();
      base.OnRemoveComplete( index, value );
    }
    
		/// <summary>
		/// Overriden. <see cref="System.Collections.CollectionBase"/>
		/// </summary>
    [ DocumentationExclude() ]
    protected override void OnSetComplete( int index, object oldValue, object newValue )
    {
      BroadcastChange();
      base.OnSetComplete( index, oldValue, newValue );
    }

		/// <summary>
		/// Overriden. <see cref="System.Collections.CollectionBase"/>
		/// </summary>
    [ DocumentationExclude() ]
    protected override void OnValidate( object value )
    {

      if( value == null )
        throw new ArgumentNullException( "value" );
    }

    #endregion

    #region Class event raisers

		/// <internalonly/>
    [ DocumentationExclude() ]
    private void BroadcastChange()
    {

      if( Changed != null )
      {
        Changed( this, EventArgs.Empty );
      }
    }

    #endregion
  }
}