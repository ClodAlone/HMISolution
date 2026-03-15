#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Threading;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This is the base class for all Implementation's classes.
  /// </summary>
//  [DebuggerStepThrough]
  public class CommonObject : IParentApplication, IDisposable
  {
    #region Class members
    /// <summary>
    /// Reference to Application object.
    /// </summary>
    private IApplication  m_appl;
    /// <summary>
    /// Reference to parent Object.
    /// </summary>
    private object        m_parent;
    /// <summary>
    /// Counter which can be used for calculating references.
    /// </summary>
    private   int         m_iReferenceCount;
    /// <summary>
    /// Flag which indicates if the object was disposed or not.
    /// </summary>
    protected bool        m_bIsDisposed;
    #endregion

    #region Class Properties
    /// <summary>
    /// Reference to Application which hosts all objects. Read-only.
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
    /// Reference to Parent object. Read-only.
    /// </summary>
    public object Parent
    {
      [DebuggerStepThrough]
      get
      {
        return m_parent;
      }
    }
    #endregion

    #region Class private properties
    /// <summary>
    /// Reference to Application which hosts all objects. Read-only.
    /// </summary>
    public ApplicationImpl AppImplementation
    {
      get
      {
        return ( ApplicationImpl )m_appl;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. Object cannot be constructed without setting Application
    /// and parent references.
    /// </summary>
    private CommonObject()
    {
    }
    /// <summary>
    /// Main class constructor. Application and Parent properties are set.
    /// </summary>
    /// <param name="application">Reference to Application instance.</param>
    /// <param name="parent">
    /// Reference to the Parent object which will host this object
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// If specified application or parent is null.
    /// </exception>
    public CommonObject( IApplication application, object parent )
      : this()
    {
      if( application == null )
        throw new ArgumentNullException( "application" );

      if( parent == null )
        throw new ArgumentNullException( "parent" );

      m_appl = application;
      m_parent = parent;
    }
    /// <summary>
    /// Destructor. Call dispose method of current object.
    /// </summary>
    ~CommonObject()
    {
      Dispose();
    }
    #endregion

    #region Class Helper methods
    /// <summary>
    /// This method is used to find parent with specific type.
    /// </summary>
    /// <param name="parentType">Parent type to locate.</param>
    /// <returns>The found parent or NULL if parent was not found.</returns>
    /// <exception cref="System.ArgumentException">
    /// When there is cycle in object tree.
    /// </exception>
    //[DebuggerStepThrough]
    public object FindParent( Type parentType )
    {
      return FindParent( m_parent, parentType );
    }
    /// <summary>
    /// This method is used to find parent with specific type.
    /// </summary>
    /// <param name="parentType">Parent type to locate.</param>
    /// <param name="bSubTypes">Indicates whether to search subtypes.</param>
    /// <returns>The found parent or NULL if parent was not found.</returns>
    /// <exception cref="System.ArgumentException">
    /// When there is cycle in object tree.
    /// </exception>
    public object FindParent( Type parentType, bool bSubTypes )
    {
      return FindParent( m_parent, parentType, bSubTypes );
    }
    /// <summary>
    /// This method is used to find parent with specific type.
    /// </summary>
    /// <param name="parentStart">Start object for search operation.</param>
    /// <param name="parentType">Parent type to locate.</param>
    /// <returns>The found parent or NULL if parent was not found.</returns>
    public static object FindParent( object parentStart, Type parentType )
    {
      return FindParent( parentStart, parentType, false );
    }
    /// <summary>
    /// This method is used to find parent with specific type.
    /// </summary>
    /// <param name="parentStart">Start object for search operation.</param>
    /// <param name="parentType">Parent type to locate.</param>
    /// <param name="bSubTypes">Indicates whether to search subtypes.</param>
    /// <returns>The found parent or NULL if parent was not found.</returns>
    public static object FindParent( object parentStart, Type parentType, bool bSubTypes )
    {
      if( parentType == null )
        throw new ArgumentNullException( "parentType" );

      int iCount = 0;
      IParentApplication parentTmp = ( IParentApplication )parentStart;
      bool bNeedInterface =
#if ( WINRT )
          parentType.GetTypeInfo().IsInterface;
#else
          parentType.IsInterface;
#endif

      // Find workbook to which the style must belong to.
      do
      {
        if( iCount > 100 )
          throw new ArgumentException( "links Cycle in object tree detected!" );

        if( parentTmp == null ) break;

        Type type = parentTmp.GetType();

        if( !bNeedInterface )
        {
          if( type.Equals( parentType )
            || bSubTypes && type.IsSubclassOf( parentType ) )
          {
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
    /// Finds parent objects.
    /// </summary>
    /// <param name="arrTypes">Array of parents type.</param>
    /// <returns>Returns array of found parent objects.</returns>
    public object[] FindParents( Type[] arrTypes )
    {
      int iCount = 0;
      IParentApplication parentTmp = ( IParentApplication )m_parent;
      object[] arrResult = new object[ arrTypes.Length ];

      // Find workbook to which the style must belong to.
      do
      {
        if( iCount > 100 )
          throw new ArgumentException( "links Cycle in object tree detected!" );

        if( parentTmp == null ) break;

        int index = Array.IndexOf( arrTypes, parentTmp.GetType() );

        if( index != -1 ) arrResult[ index ] = parentTmp;

        parentTmp = ( IParentApplication )parentTmp.Parent;
        iCount++;
      }
      while( parentTmp != null );

      return arrResult;
    }
    /// <summary>
    /// Find parent of object.
    /// </summary>
    /// <param name="arrTypes">Array of parents type.</param>
    /// <returns>Returns found parent object.</returns>
    public object FindParent( Type[] arrTypes )
    {
      int iCount = 0;
      IParentApplication parentTmp = ( IParentApplication )Parent;

      // Find workbook to which the style must belong to.
      do
      {
        if( iCount > 100 )
          throw new ArgumentException( "links Cycle in object tree detected!" );

        if( parentTmp == null ) break;

        int index = Array.IndexOf( arrTypes, parentTmp.GetType() );

        if( index != -1 ) return parentTmp;

        parentTmp = ( IParentApplication )parentTmp.Parent;
        iCount++;
      }
      while( parentTmp != null );

      return parentTmp;
    }
    /// <summary>
    /// Sets parent of the object.
    /// </summary>
    /// <param name="parent">New parent for this object.</param>
    protected internal void SetParent( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      m_parent = parent;
    }
    /// <summary>
    /// Checks whether object was disposed and throws exception if it was.
    /// </summary>
    protected void CheckDisposed()
    {
      if( m_bIsDisposed )
        throw new ApplicationException( "Object was disposed." );
    }
    #endregion

    #region Class instance references count
    /// <summary>
    /// Increase the quantity of reference. User must use this method when
    /// new wrapper on object is created or reference on object stored.
    /// </summary>
    /// <returns>New state of Reference count value.</returns>
    [DebuggerStepThrough]
    public virtual int AddReference()
    {
      return Interlocked.Increment( ref m_iReferenceCount );
      //lock( this )
      //{
      //  return ++m_iReferenceCount;
      //}
    }
    /// <summary>
    /// Decrease quantity of Reference. User must call this method 
    /// when freeing resources.
    /// </summary>
    /// <returns>New state of Reference count value.</returns>
    [DebuggerStepThrough]
    public virtual int ReleaseReference()
    {
      return Interlocked.Decrement( ref m_iReferenceCount );
      //lock( this )
      //{
      //  return --m_iReferenceCount;
      //}
    }
    /// <summary>
    /// Get quantity of instance references.
    /// </summary>
    public int ReferenceCount
    {
      [DebuggerStepThrough]
      get
      {
        //return Interlocked.Exchange( ref m_iReferenceCount, 
        //  Interlocked.Exchange( ref m_iReferenceCount, 1 ) );
        return m_iReferenceCount;
        //lock( this )
        //{
        //  return m_iReferenceCount;
        //}
      }
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Dispose object and free resources.
    /// </summary>
    public virtual void Dispose()
    {
      if( !m_bIsDisposed )
      {
        OnDispose();
        m_parent = null;
        m_appl = null;
        m_bIsDisposed = true;
        GC.SuppressFinalize( this );
      }
    }
    /// <summary>
    /// Method which can be overridden by users to take any specific actions when
    /// object is disposed.
    /// </summary>
    protected virtual void OnDispose()
    {
    }
    #endregion
  }
  ///<exclude/>
  /// <summary>
  /// Class used as message sender on Property value change. Class provides old and
  /// new values which allow user to create advanced logic.
  /// </summary>
  [DebuggerStepThrough]
  public class ValueChangedEventArgs : EventArgs
  {
    private static ValueChangedEventArgs _empty = new ValueChangedEventArgs();

    #region Class members
    /// <summary>
    /// Storage of Old value.
    /// </summary>
    private object m_old;
    /// <summary>
    /// Storage of new value.
    /// </summary>
    private object m_new;
    /// <summary>
    /// Name of property or unique identifier of the object
    /// whose value changed.
    /// </summary>
    private string m_strName;
    /// <summary>
    /// Value changed event arguments.
    /// </summary>
    private ValueChangedEventArgs m_next;

#if VALUE_CHANGE_DEBUG
    /// <summary>
    /// Number of instances.
    /// </summary>
    internal static int m_iInstancesCount;
    /// <summary>
    /// Number of references to NewValue property.
    /// </summary>
    internal static int m_iNewReferenced;
    /// <summary>
    /// Number of references to OldValue property.
    /// </summary>
    internal static int m_iOldReferenced;
    /// <summary>
    /// Number of name object referenced.
    /// </summary>
    internal static int m_iNameReferenced;
#endif
    #endregion

    #region Class Properties
    /// <summary>
    /// New property value. Event handler property has new value set.
    /// Read-only.
    /// </summary>
    public object newValue
    {
      [DebuggerStepThrough]
      get
      {
#if VALUE_CHANGE_DEBUG
        m_iNewReferenced++;
#endif
        return m_new;
      }
    }

    /// <summary>
    /// Old property value. Event handler property has new value set.
    /// Read-only.
    /// </summary>
    public object oldValue
    {
      [DebuggerStepThrough]
      get
      {
#if VALUE_CHANGE_DEBUG
        m_iOldReferenced++;
#endif
        return m_old;
      }
    }

    /// <summary>
    /// Name of the property. Read-only.
    /// </summary>
    public string Name
    {
      [DebuggerStepThrough]
      get
      {
#if VALUE_CHANGE_DEBUG
        m_iNameReferenced++;
#endif
        return m_strName;
      }
    }
    /// <summary>
    /// If more than one property must be changed on one send message, 
    /// attach it to the ValueChangeEventArgs to create a one way directed list of property changes.
    /// </summary>
    public ValueChangedEventArgs Next
    {
      [DebuggerStepThrough]
      get
      {
        return m_next;
      }
      set
      {
        m_next = null;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    private ValueChangedEventArgs()
    {
    }

    /// <summary>
    /// Main constructor.
    /// </summary>
    /// <param name="old">Old property value.</param>
    /// <param name="newValue">New property value.</param>
    /// <param name="objectName">Unique Identifier of object whose value changed.</param>
    public  ValueChangedEventArgs( object old, object newValue, string objectName )
      : this( old, newValue, objectName, null )
    {
    }
    /// <summary>
    /// Main constructor.
    /// </summary>
    /// <param name="old">Old property value.</param>
    /// <param name="newValue">New property value.</param>
    /// <param name="objectName">Unique Identifier of object whose value changed.</param>
    /// <param name="next">Next property which must be changed.</param>
    public  ValueChangedEventArgs( object old, object newValue, string objectName, ValueChangedEventArgs next )
    {
#if VALUE_CHANGE_DEBUG
      m_iInstancesCount++;
#endif
      m_old = old;
      m_new = newValue;
      m_strName = objectName;
      m_next = next;
    }
    #endregion

    #region Class static properties
    /// <summary>
    /// Returns the class instance with empty values. Read-only.
    /// </summary>
    new public static ValueChangedEventArgs Empty
    {
      [DebuggerStepThrough]
      get
      {
        return _empty;
      }
    }
    #endregion
  }
  ///<exclude/>
  /// <summary>
  /// Delegate which can be used for Property Changed events declarations.
  /// </summary>
  public delegate void ValueChangedEventHandler( object sender, ValueChangedEventArgs e );
}