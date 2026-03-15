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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// This is the base class for all collections.
  /// </summary>
  //[DebuggerStepThrough]
  public class CollectionBaseEx<T>
    : Syncfusion.XlsIO.Implementation.Collections.CollectionBase<T>
    , IList<T>
    , IParentApplication
    , ICloneParent
  {
    #region Class members
    /// <summary>
    /// An Application object that represents the Excel application.
    /// </summary>
    private IApplication  m_appl;
    /// <summary>
    /// The parent object for the specified object.
    /// </summary>
    private object        m_parent;
    /// <summary>
    /// If True, events will not be raised; if False, events will be raised.
    /// </summary>
    private bool          m_bSkipEvents;

    private static Dictionary<string, int> m_dictCollectionsMaxValues = new Dictionary<string, int>();
    #endregion

    #region Class Properties
    /// <summary>
    /// Read-only. An Application object that represents the Excel application.
    /// </summary>
    public IApplication Application
    {
      [DebuggerStepThrough]
      get
      {
        return m_appl;
      }
    }
    /// <summary>
    /// Read-only. The parent object for the specified object.
    /// </summary>
    public object Parent
    {
      [DebuggerStepThrough]
      get
      {
        return m_parent;
      }
    }
    /// <summary>
    /// Gets / sets whether class can raise events.
    /// </summary>
    public bool QuietMode
    {
      get
      {
        return m_bSkipEvents;
      }
      set
      {
        if( value != m_bSkipEvents )
        {
          m_bSkipEvents = value;
        }
      }
    }
    /// <summary>
    /// Application object. Read-only.
    /// </summary>
    protected ApplicationImpl AppImplementation
    {
      [DebuggerStepThrough]
      get
      {
        return (ApplicationImpl) Application;
      }
    }
    #endregion

    #region Class delegates
    /// <summary>
    /// Delegate for Clear event.
    /// </summary>
    public delegate void CollectionClear();
    /// <summary>
    /// Delegate for Change event.
    /// </summary>
    public delegate void CollectionChange( object sender, CollectionChangeEventArgs<T> args );
    /// <summary>
    /// Delegate for Set event.
    /// </summary>
    public delegate void CollectionSet( int index, object old, object value );
    #endregion

    #region Class events
    /// <summary>
    /// Change in the collection.
    /// </summary>
    public event EventHandler     Changed;
    /// <summary>
    /// Raised by class before real cleaning of collection.
    /// </summary>
    public event CollectionClear  Clearing;
    /// <summary>
    /// Raised by class after collection clean process.
    /// </summary>
    public event CollectionClear  Cleared;
    /// <summary>
    /// Raised by class before item will be added into the collection.
    /// </summary>
    public event CollectionChange Inserting;
    /// <summary>
    /// Raised by class after item is added to the collection.
    /// </summary>
    public event CollectionChange Inserted;
    /// <summary>
    /// Raised by class before real item is removed from the collection.
    /// </summary>
    public event CollectionChange Removing;
    /// <summary>
    /// Raised by class after item is removed from the collection storage
    /// </summary>
    public event CollectionChange Removed;
    /// <summary>
    /// Raised by class before item is replaced in the collection.
    /// </summary>
    public event CollectionSet    Setting;
    /// <summary>
    /// Raised by class after item is replaced in the collection.
    /// </summary>
    public event CollectionSet    Set;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. To prevent class creation by default constructor.
    /// </summary>
    private CollectionBaseEx()
    {
    }
    /// <summary>
    /// Creates collection and sets its Application and Parent values.
    /// </summary>
    /// <param name="application">
    /// Application object that represents the Excel application.
    /// </param>
    /// <param name="parent">Parent object of this collection.</param>
    public CollectionBaseEx( IApplication application, object parent )
      : this()
    {
      m_appl = application;
      m_parent = parent;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// This method raises the Changed event.
    /// </summary>
    private void RaiseChangedEvent()
    {
      if( Changed != null && m_bSkipEvents == false )
      {
        Changed( this, EventArgs.Empty );
      }
    }
    /// <summary>
    /// OnClear is invoked before Clear behavior.
    /// </summary>
    protected override void OnClear()
    {
      // Any attached event handlers?
      if( Clearing != null && m_bSkipEvents == false )
      {
        // Raise event to notify all contents removed.
        Clearing();
      }
      m_dictCollectionsMaxValues.Clear();
      base.OnClear();
    }
    /// <summary>
    /// OnClear is invoked after Clear behavior.
    /// </summary>
    protected override void OnClearComplete()
    {
      // Any attached event handlers?
      if( Cleared != null && m_bSkipEvents == false )
      {
        // Raise event to notify all contents removed.
        Cleared();
      }

      base.OnClearComplete();

      RaiseChangedEvent();
    }
    /// <summary>
    /// Performs additional processes before inserting
    /// a new element into the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at index.</param>
    protected override void OnInsert( int index, T value )
    {
      // Any attached event handlers?
      if( Inserting != null && m_bSkipEvents == false )
      {
        // Raise event to notify new content added.
        Inserting( this, new CollectionChangeEventArgs<T>( index, value ) );
      }

      base.OnInsert( index, value );
    }
    /// <summary>
    /// Performs additional processes after inserting
    /// a new element into the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at index.</param>
    protected override void OnInsertComplete( int index, T value )
    {
      // Check for any attached event handlers.
      if( Inserted != null && m_bSkipEvents == false )
      {
        // Raise event to notify new content added.
        Inserted( this, new CollectionChangeEventArgs<T>( index, value ) );
      }

      base.OnInsertComplete( index, value );

      RaiseChangedEvent();
    }
    /// <summary>
    /// Performs additional processes before removing
    /// an element from the collection.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which the value can be found.
    /// </param>
    /// <param name="value">
    /// The value of the element to remove from index.
    /// </param>
    protected override void OnRemove( int index, T value )
    {
      // Check for attached event handlers.
      if( Removing != null && m_bSkipEvents == false )
      {
        // Raise event to notify content has been removed.
        Removing( this, new CollectionChangeEventArgs<T>( index, value ) );
      }

      base.OnRemove( index, value );
    }
    /// <summary>
    /// Performs additional processes after removing 
    /// an element from the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which the value can be found.</param>
    /// <param name="value">The value of the element to remove from index.</param>
    protected override void OnRemoveComplete( int index, T value )
    {
      // Check for attached event handlers.
      if( Removed != null && m_bSkipEvents == false )
      {
        // Raise event to notify that content has been removed.
        Removed( this, new CollectionChangeEventArgs<T>( index, value ) );
      }

      base.OnRemoveComplete( index, value );

      RaiseChangedEvent();
    }
    /// <summary>
    /// Performs additional processes before setting
    /// a value in the collection.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which oldValue can be found.
    /// </param>
    /// <param name="oldValue">The value to replace with newValue.</param>
    /// <param name="newValue">The new value of the element at index.</param>
    protected override void OnSet( int index, T oldValue, T newValue )
    {
      if( Setting != null && m_bSkipEvents == false )
      {
        Setting( index, oldValue, newValue );
      }

      base.OnSet( index, oldValue, newValue );
    }
    /// <summary>
    /// Performs additional processes after setting a value in the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which oldValue can be found.</param>
    /// <param name="oldValue">The value to replace with newValue.</param>
    /// <param name="newValue">The new value of the element at the index.</param>
    protected override void OnSetComplete( int index, T oldValue, T newValue )
    {
      if( Set != null && m_bSkipEvents == false )
      {
        Set( index, oldValue, newValue );
      }

      base.OnSetComplete( index, oldValue, newValue );

      RaiseChangedEvent();
    }
    #endregion

    #region Class Helper methods
    /// <summary>
    /// Method used to find parent within a specific type.
    /// </summary>
    /// <param name="parentType">Parent type to search.</param>
    /// <returns>Found parent if was parent was found or NULL otherwise.</returns>
    /// <exception cref="System.ArgumentException">
    /// When there is cycle in object tree.
    /// </exception>
    public object FindParent( Type parentType )
    {
      return FindParent( parentType, false );
    }
    /// <summary>
    /// Method used to find parent within a specific type.
    /// </summary>
    /// <param name="parentType">Parent type to search.</param>
    /// <param name="bCheckSubclasses">Indicates whether to look into subclasses.</param>
    /// <returns>Found parent if was parent was found or NULL otherwise.</returns>
    /// <exception cref="System.ArgumentException">
    /// When there is cycle in object tree.
    /// </exception>
    //[DebuggerStepThrough]
    public object FindParent( Type parentType, bool bCheckSubclasses )
    {
      int iCount = 0;
      IParentApplication parentTmp = ( IParentApplication )Parent;
      bool bNeedInterface =
#if ( WINRT )
          parentType.GetTypeInfo().IsInterface;
#else
          parentType.IsInterface;
#endif

      // Find workbook to which style must belong to.
      do
      {
        if( iCount > 100 )
          throw new ArgumentException( "links Cycle in object tree detected!" );

        if( parentTmp == null || parentTmp.Parent == null ) break;
        Type type = parentTmp.GetType();
        
        if( !bNeedInterface )
        {
          if( type.Equals( parentType ) )
          {
            break;
          }
          else if( bCheckSubclasses )
          {
            type.IsSubclassOf( parentType );
            break;
          }
        }
        else
        {
          if( type.GetInterface( parentType.Name, false ) != null )
            break;
        }

        parentTmp = ( IParentApplication )parentTmp.Parent;
        iCount++;
      }
      while( parentTmp != null );

      return parentTmp;
    }
    /// <summary>
    /// Sets parent object for class.
    /// </summary>
    /// <param name="parent">Parent object for sets.</param>
    public void SetParent( object parent )
    {
      m_parent = parent;
    }
    /// <summary>
    /// Creates copy of the collection.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns>A copy of the collection.</returns>
    public virtual object Clone( object parent )
    {
      Type type = GetType();
      ConstructorInfo constructor = type.GetConstructor(
        new Type[]{ typeof( IApplication ), typeof( object ) } );

      if( constructor == null )
        throw new ApplicationException( "Cannot find required constructor." );

      object objResult = constructor.Invoke( new object[] { Application, parent } );
      CollectionBaseEx<T> result = objResult as CollectionBaseEx<T>;

      List<T> list = InnerList;
      //IList<T> destList = result.List;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        T item = list[ i ];

        if( item is ICloneParent )
        {
          ICloneParent toClone = ( ICloneParent )item;
          item = ( T )toClone.Clone( result );
        }
        else if( item is ICloneable )
        {
          ICloneable toClone = ( ICloneable )item;
          item = ( T )toClone.Clone();
        }

        result.Add( item );
      }

      return result;
    }
    /// <summary>
    /// Enlarges internal storage if necessary.
    /// </summary>
    /// <param name="size">Required size.</param>
    public void EnsureCapacity( int size )
    {
      if( InnerList.Capacity < size )
      {
        InnerList.Capacity = size;
      }
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Generates default name.
    /// </summary>
    /// <param name="namesCollection">Names collection.</param>
    /// <param name="strStart">Start string.</param>
    /// <returns>Returns string with new name.</returns>
    public static string GenerateDefaultName( ICollection<T> namesCollection, string strStart )
    {
      int max = 1;
      int iStartLength = strStart.Length;
      
      if (m_dictCollectionsMaxValues.ContainsKey(strStart))
      {
          max = m_dictCollectionsMaxValues[strStart];
          max = max + 1;
          m_dictCollectionsMaxValues[strStart] = max;
      }
      else
      {
          foreach (INamedObject named in namesCollection)
          {
              string strName = named.Name;

              if (strName != null && strName.StartsWith(strStart))
              {
                  string strNumber = strName.Substring(iStartLength,
                    strName.Length - iStartLength);

                  double doubleValue;

                  if (double.TryParse(strNumber, NumberStyles.Integer, null, out doubleValue))
                  {
                      int i = (int)doubleValue + 1;
                      max = Math.Max(i, max);
                  }
              }

          }
              m_dictCollectionsMaxValues.Add(strStart, max);
      }
      return strStart + max.ToString();
    }
    /// <summary>
    /// Generates default name.
    /// </summary>
    /// <param name="namesCollection">Names collection.</param>
    /// <param name="strStart">Start string.</param>
    /// <returns>Returns string with new name.</returns>
    public static string GenerateDefaultName( ICollection namesCollection, string strStart )
    {
      int max = 1;
      int iStartLength = strStart.Length;

      foreach( INamedObject named in namesCollection )
      {
        string strName = named.Name;

        if( strName != null && strName.StartsWith( strStart ) )
        {
          string strNumber = strName.Substring( iStartLength,
            strName.Length - iStartLength );

          double doubleValue;

          if( double.TryParse( strNumber, NumberStyles.Integer, null, out doubleValue ) )
          {
            int i = ( int )doubleValue + 1;
            max = Math.Max( i, max );
          }
        }
      }

      return strStart + max.ToString();
    }
    /// <summary>
    /// Generate default name.
    /// </summary>
    /// <param name="strStart">Start string.</param>
    /// <param name="arrCollections">Collection with names.</param>
    /// <returns>Returns new name.</returns>
    public static string GenerateDefaultName( string strStart, params ICollection[] arrCollections )
    {
      int max = 1;
      int iStartLength = strStart.Length;

      for( int j = 0, len = arrCollections.Length; j < len; j++ )
      {
        ICollection namesCollection = arrCollections[ j ];

        foreach( object obj in namesCollection )
        {
          string strName;

          if( obj is INamedObject )
          {
            strName = ( obj as INamedObject ).Name;
          }
          else
          {
            strName = obj.ToString();
          }

          if( strName.StartsWith( strStart ) )
          {
            string strNumber = strName.Substring( iStartLength,
              strName.Length - iStartLength );

            double doubleValue;

            if( double.TryParse( strNumber, NumberStyles.Integer, null, out doubleValue ) )
            {
              int i = ( int ) doubleValue + 1;//int.Parse( strNumber ) + 1;
              max = Math.Max( i, max );
            }
          }
        }
      }

      return strStart + max.ToString();
    }
    /// <summary>
    /// Changes name.
    /// </summary>
    /// <param name="hashNames">Hash table with names</param>
    /// <param name="e"></param>
    public static void ChangeName( IDictionary hashNames, ValueChangedEventArgs e )
    {
      string oldName = ( string )e.oldValue;
      string newName = ( string )e.newValue;

      if( !hashNames.Contains( oldName ) )
        throw new ArgumentOutOfRangeException
          ( "Collection does not contain object named " + oldName );

      if( hashNames.Contains( newName ) )
        throw new ArgumentOutOfRangeException
          ( "Collection already contains object named " + newName );

      object value = hashNames[ oldName ];
      hashNames.Remove( oldName );
      hashNames.Add( newName, value );
    }
    #endregion
  }
  /// <summary>
  /// Class that represent event args.
  /// </summary>
  public class CollectionChangeEventArgs<T> : EventArgs
  {
    #region Class members
    /// <summary>
    /// Element index.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Element value.
    /// </summary>
    private T m_value;
    #endregion
    
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    private CollectionChangeEventArgs()
    {
    }
    /// <summary>
    /// Creates new instance of event arguments.
    /// </summary>
    /// <param name="index">Changed element index.</param>
    /// <param name="value">Changed element value.</param>
    public CollectionChangeEventArgs( int index, T value )
    {
      m_iIndex = index;
      m_value = value;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Element index. Read-only.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
    }
    /// <summary>
    /// Element value. Read-only.
    /// </summary>
    public T Value
    {
      get
      {
        return m_value;
      }
    }
    #endregion
  }
}
