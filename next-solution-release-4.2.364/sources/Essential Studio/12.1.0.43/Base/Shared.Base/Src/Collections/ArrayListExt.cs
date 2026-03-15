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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

using Syncfusion.ComponentModel;
using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Collections
{
  /// <summary>
  /// Extends ArrayList by throwing events when Collection changes, an
  /// item's property changes, etc.
  /// </summary>
  /// <remarks>
  /// This class lets you know through the <see cref="CollectionChanged"/> event
  /// when an item gets added or deleted from the ArrayList or
  /// when an exisiting item is replaced by a new item at a position.
  /// It will also listen for property change notifications from the
  /// items in the list, provided the items implement the <see cref="Syncfusion.ComponentModel.IChangeNotifyingItem"/>
  /// interface and forward them using the <see cref="ItemPropertyChanged"/> event.
  /// </remarks>
  [ Serializable ]
  public class ArrayListExt : ArrayList
  {
    #region Class members
    /// <summary></summary>
    private bool suspendCollectionChangedEvent = false;
    /// <summary></summary>
    private bool forceReadOnly = false;
    /// <summary></summary>
    private bool forceFixedSize = false;
    #endregion

    #region Class properties
    /// <summary>
    /// Overridden. See <see cref="P:System.Collections.ArrayList.Item"/>.
    /// </summary>
    public override object this[ int index ]
    {
      get
      {
        return base[ index ];
      }
      set
      {
        if( this.IsReadOnly )
        {
          throw new NotSupportedException( "Trying to edit a Read-only ArrayListExt." );
        }

        if( base[ index ] != value )
        {
          OnCollectionChanging();

          object old = base[ index ];
          base[ index ] = value;

          if( old != null )
          {
            ReleaseHandler( old );
          }

          AddHandlers( value );
        }
      }
    }

    /// <summary>
    /// Indicates whether the collection is Read-only.
    /// </summary>
    /// <value>True to make the collection Read-only; False otherwise. Default is False.</value>
    /// <remarks>When set to True, the <see cref="IsReadOnly"/> property will return True
    /// and exisiting items in the list cannot be replaced.</remarks>
    protected bool ForceReadOnly
    {
      get
      {
        return this.forceReadOnly;
      }
      set
      {
        this.forceReadOnly = value;
      }
    }
    /// <summary>
    /// Indicates whether the collection should be made fixed size.
    /// </summary>
    /// <value>True to make the collection fixed size; False otherwise. Default is False.</value>
    /// <remarks>When set to True, the <see cref="IsFixedSize"/> property will return True
    /// and no new elements can be added to the list.</remarks>
    protected bool ForceFixedSize
    {
      get
      {
        return this.forceFixedSize;
      }
      set
      {
        this.forceFixedSize = value;
      }
    }
    /// <override/>
    /// <summary></summary>
    public override bool IsFixedSize
    {
      get
      {
        return this.forceFixedSize;
      }
    }
    /// <override/>
    /// <summary></summary>
    public override bool IsReadOnly
    {
      get
      {
        return this.forceReadOnly;
      }
    }
    /// <summary>
    /// Indicates whether firing <see cref="CollectionChanged"/> event is suspended.
    /// </summary>
    /// <value>True indicates firing the event is suspended; False otherwise.</value>
    public bool IsCollectionChangedEventSuspended
    {
      get
      {
        return this.suspendCollectionChangedEvent;
      }
    }
    #endregion

    #region Class events
    /// <summary>
    /// Will be thrown when the Collection has changed due to the addition or removal of one
    /// or more items.
    /// </summary>
    [ Description( "Will be called when the Collection has changed due to the addition or removal of one or more items." ) ]
    public event CollectionChangeEventHandler CollectionChanged;
    /// <summary>
    /// Will be thrown if the items in the Collection implement <see cref="T:Syncfusion.ComponentModel.IChangeNotifyingItem"/> interface 
    /// and when their property changes.
    /// </summary>
    [ Description( "Will be thrown if the items in the Collection implement IChangeNotifyingItem interface and when their property changes." ) ]
    public event SyncfusionPropertyChangedEventHandler ItemPropertyChanged;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded. Creates a new instance of the ArrayListExt class.
    /// </summary>
    public ArrayListExt()
    {
    }

    /// <summary>
    /// Creates a new instance of the ArrayListExt class and inserts
    /// the items specified in an array into the ArrayList.
    /// </summary>
    /// <param name="items">An array of objects.</param>
    public ArrayListExt( object[ ] items )
      : base( items )
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event.
    /// </summary>
    /// <param name="args">
    /// A <see cref="CollectionChangeEventArgs"/> object containing data 
    /// pertaining to this event.
    /// </param>
    /// <remarks><para>The <see cref="OnCollectionChanged"/> method also allows derived classes to handle the event 
    /// without attaching a delegate. This is the preferred technique for 
    /// handling the event in a derived class. </para><para>Note to Inheritors:  When overriding OnCollectionChanged in a derived 
    /// class, be sure to call the base class's OnCollectionChanged method so that 
    /// registered delegates receive the event.</para></remarks>
    protected virtual void OnCollectionChanged( CollectionChangeEventArgs args )
    {
      if( !IsCollectionChangedEventSuspended && CollectionChanged != null )
      {
        this.CollectionChanged( this, args );
      }
    }

    /// <summary>
    /// Called when an item is being added, removed, moved or when an exisiting item is
    /// replaced by a new item.
    /// </summary>
    /// <remarks><para>This method does not fire a corresponding event.</para></remarks>
    protected virtual void OnCollectionChanging()
    {
    }

    /// <summary>
    /// Raises the <see cref="ItemPropertyChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="SyncfusionPropertyChangedEventArgs"/> object that contains
    /// data pertaining to this event.</param>
    /// <remarks><para>The <see cref="OnItemPropertyChanged"/> method also allows derived classes to handle the event 
    /// without attaching a delegate. This is the preferred technique for 
    /// handling the event in a derived class. </para><para>Note to Inheritors: When overriding <see cref="OnItemPropertyChanged"/> in a derived 
    /// class, be sure to call the base class's <see cref="OnItemPropertyChanged"/> method so that 
    /// registered delegates receive the event.</para></remarks>
    protected virtual void OnItemPropertyChanged( object sender, SyncfusionPropertyChangedEventArgs e )
    {
      if( !this.IsCollectionChangedEventSuspended && ItemPropertyChanged != null )
      {
        ItemPropertyChanged( sender, e );
      }
    }

    /// <summary>
    /// Called when an item gets removed from the list.
    /// </summary>
    /// <param name="item">The object that got removed from the list.</param>
    /// <remarks><para>This provides you a convenient place where you can
    /// perform operations pertaining to the removal of an object
    /// from the list. Make sure to call the base class when you override
    /// this function.</para><para>The base class will call the <see cref="OnCollectionChanged"/> event with appropriate parameters.</para></remarks>
    protected virtual void ReleaseHandler( object item )
    {
      if( item != null )
      {
        IChangeNotifyingItem notifyItem = item as IChangeNotifyingItem;

        if( notifyItem != null )
        {
          notifyItem.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( this.ItemPropertyChanged_Handler );
        }
      }

      OnCollectionChanged( new CollectionChangeEventArgs( CollectionChangeAction.Remove, item ) );
    }

    /// <summary>
    /// Called when an object gets added to the list.
    /// </summary>
    /// <param name="item">The object that got added to the list.</param>
    /// <remarks><para>This provides you a convenient place where you can
    /// perform operations pertaining to the addition of an object
    /// to the list. Make sure to call the base class when you override
    /// this function.</para><para>The method will call the <see cref="OnCollectionChanged"/> method with appropriate parameters.</para></remarks>
    protected virtual void AddHandlers( object item )
    {
      if( item != null )
      {
        IChangeNotifyingItem notifyItem = item as IChangeNotifyingItem;

        if( notifyItem != null )
        {
          notifyItem.PropertyChanged += new SyncfusionPropertyChangedEventHandler( ItemPropertyChanged_Handler );
        }
      }

      OnCollectionChanged( new CollectionChangeEventArgs( CollectionChangeAction.Add, item ) );
    }
    #endregion

    #region Class utility methods
    /// <summary></summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ItemPropertyChanged_Handler( object sender, SyncfusionPropertyChangedEventArgs e )
    {
      OnItemPropertyChanged( sender, e );
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Call this method to force a <see cref="CollectionChanged"/> event.
    /// </summary>
    /// <param name="args">The args for the above mentioned event.</param>
    /// <remarks>This method is useful when after suspending and resuming events in this list,
    /// you might want to fire the CollectionChanged event for some specific changes.</remarks>
    public void RaiseCollectionChanged( CollectionChangeEventArgs args )
    {
      OnCollectionChanged( args );
    }

    /// <summary>
    /// Overridden. See <see cref="M:System.Collections.ArrayList.Add"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override int Add( object value )
    {
      if( this.ForceFixedSize )
      {
        throw new NotSupportedException( "Trying to add / remove elements in a fixed size." + this.GetType().FullName + "." );
      }

      OnCollectionChanging();

      int result = base.Add( value );

      AddHandlers( value );

      return result;
    }

    /// <summary>
    /// Overridden. See <see cref="M:System.Collections.ArrayList.Insert"/>.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public override void Insert( int index, object value )
    {
      if( this.ForceFixedSize )
      {
        throw new NotSupportedException( "Trying to add / remove elements in a fixed size ArrayListExt." );
      }

      OnCollectionChanging();
      base.Insert( index, value );
      AddHandlers( value );
    }

    /// <summary>
    /// Overridden. See <see cref="M:System.Collections.ArrayList.InsertRange"/>.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="c"></param>
    public override /*ArrayList*/ void InsertRange( int index, ICollection c )
    {
      if( this.ForceFixedSize )
      {
        throw new NotSupportedException( "Trying to add / remove elements in a fixed size ArrayListExt." );
      }

      OnCollectionChanging();
      base.InsertRange( index, c );

      // Sometimes the CollectionChanged listeners could remove the items from the collection passed in (c), 
      // in which case enumeration in for each will fail. 
      // To work around that, we copy the collection over to a new arraylist and then use for each to enumerate.
      object[ ] items = new object[c.Count];
      c.CopyTo( items, 0 );

      foreach( object item in items )
      {
        AddHandlers( item );
      }
    }

    /// <summary>
    /// Overridden. See <see cref="M:System.Collections.ArrayList.Clear"/>.
    /// </summary>
    public override /*IList*/ void Clear()
    {
      if( this.ForceFixedSize )
      {
        throw new NotSupportedException( "Trying to add / remove elements in a fixed size ArrayListExt." );
      }

      OnCollectionChanging();

      // Store reference for calling ReleaseHandler later
      object[ ] oldItems = new object[this.Count];
      this.CopyTo( oldItems, 0 );

      base.Clear();

      foreach( object item in oldItems )
      {
        this.ReleaseHandler( item );
      }
    }

    /// <summary>
    /// Overridden. See <see cref="M:System.Collections.ArrayList.RemoveAt"/></summary>
    /// <param name="index"/>
    public override /*IList*/ void RemoveAt( int index )
    {
      if( this.ForceFixedSize )
      {
        throw new NotSupportedException( "Trying to add / remove elements in a fixed size ArrayListExt." );
      }

      OnCollectionChanging();
      object oldValue = this[ index ];

      base.RemoveAt( index );

      this.ReleaseHandler( oldValue );
    }

    /// <summary>
    /// Overridden. See <see cref="M:System.Collections.ArrayList.RemoveRange"/>.
    /// </summary>
    /// <param name="index"/>
    /// <param name="count"/>
    public override void RemoveRange( int index, int count )
    {
      if( this.ForceFixedSize )
      {
        throw new NotSupportedException( "Trying to add / remove elements in a fixed size ArrayListExt." );
      }

      OnCollectionChanging();
      object[ ] oldItems = new object[count];
      this.CopyTo(index, oldItems, 0, count);
      
      base.RemoveRange( index, count );

      foreach( object item in oldItems )
      {
        this.ReleaseHandler( item );
      }
    }

    /// <summary>
    /// Sorts the elements in the entire System.Collections.ArrayList 
    /// using the System.IComparable implementation of each element.
    /// </summary>
    /// <param name="index"/>
    /// <param name="count"/>
    /// <param name="comparer"/>
    public override void Sort( int index, int count, IComparer comparer )
    {
      OnCollectionChanging();

      base.Sort( index, count, comparer );

      OnCollectionChanged( new CollectionChangeEventArgs( CollectionChangeAction.Refresh, null ) );
    }

    /// <summary>
    /// Reverse range of items in collection from position specified by index 
    /// parameter and length specified by count parameter.
    /// </summary>
    /// <param name="index">range start position.</param>
    /// <param name="count">range length.</param>
    public override void Reverse( int index, int count )
    {
      OnCollectionChanging();

      base.Reverse( index, count );

      OnCollectionChanged( new CollectionChangeEventArgs( CollectionChangeAction.Refresh, null ) );
    }

    /// <summary>
    /// Allows you to move one or more items in the collection from 
    /// one position to another.
    /// </summary>
    /// <param name="from">The beginning index of the range of items to move.</param>
    /// <param name="to">The destination index where the items will be moved to.</param>
    /// <param name="count">The number of elements in the range to be moved.</param>
    /// <remarks><para>
    /// If the above indices are not within the list's count, this
    /// method will return without performing any operation.
    /// </para><para>For example, say a list contains the following elements:
    /// A, B, C, D, E, F. Then the following call:</para><code>list.Move(2, 4, 2); </code><para>will result in the following array: A, B, E, F, C, D.</para></remarks>
    public virtual void Move( int from, int to, int count )
    {
      OnCollectionChanging();

      if( from == to || from < 0 || to < 0 ||
        to + count > this.Count || from + count > this.Count )
      {
        return;
      }

      bool locallySuspended = false;

      if( !this.IsCollectionChangedEventSuspended )
      {
        suspendCollectionChangedEvent = true;
        locallySuspended = true;
      }

      object[ ] items = new object[count];

      // Cache the items before moving them.
      for( int i = 0 ; i < count ; i++ )
      {
        items[ i ] = this[ i + from ];
      }

      base.RemoveRange( from, count );
      base.InsertRange( to, items );

      if( locallySuspended )
      {
        this.suspendCollectionChangedEvent = false;
      }

      OnCollectionChanged( new CollectionChangeEventArgs( CollectionChangeAction.Refresh, null ) );
    }

    /// <summary>
    /// Will suspend the <see cref="CollectionChanged"/> and  <see cref="ItemPropertyChanged"/> events 
    /// temporarily.
    /// </summary>
    /// <remarks><para>Call <see cref="ResumeEvents"/> when you are ready to receive events again.
    /// The <see cref="SuspendEvents"/>/<see cref="ResumeEvents"/> calls are useful when you
    /// are performing a series of operations that will result in 
    /// multiple changes in the collection, throwing multiple events.
    /// In such cases, you could use these methods to have a single event thrown
    /// at the end of the series of operations.</para><para>Note that there is no one-to-one correspondence between
    /// <see cref="SuspendEvents"/> and <see cref="ResumeEvents"/>. <see cref="SuspendEvents"/> could be called
    /// more than once but a single subsequent <see cref="ResumeEvents"/> call will
    /// resume throwing events.</para></remarks>
    public void SuspendEvents()
    {
      suspendCollectionChangedEvent = true;
    }

    /// <summary>
    /// Starts throwing the <see cref="CollectionChanged"/> and <see cref="ItemPropertyChanged"/>
    /// events.
    /// </summary>
    /// <param name="throwEvent">True will throw a <see cref="CollectionChanged"/> event; False will not.</param>
    /// <remarks><para>Call this method after calling a <see cref="SuspendEvents"/> method to resume
    /// throwing the <see cref="CollectionChanged"/> and <see cref="ItemPropertyChanged"/> events.</para><para>Note that there is no one-to-one correspondence between
    /// SuspendEvents and ResumeEvents. SuspendEvents could be called
    /// more than once but a single subsequent ResumeEvents call will
    /// resume throwing events.</para></remarks>
    public void ResumeEvents( bool throwEvent )
    {
      suspendCollectionChangedEvent = false;

      if( throwEvent )
      {
        OnCollectionChanged( new CollectionChangeEventArgs( CollectionChangeAction.Refresh, null ) );
      }
    }
    #endregion
  }

  /// <summary>
  /// A list deriving from <see cref="VisuallyInheritableList"/> with a strongly-typed indexer of type int.
  /// </summary>
  public class VisuallyInheritableIntList : VisuallyInheritableList
  {
    /// <summary>
    /// Overloaded. Creates a new instance of the <see cref="VisuallyInheritableList"/> class.
    /// </summary>
    public VisuallyInheritableIntList()
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="VisuallyInheritableList"/> class and inserts
    /// the items specified in an array into the <see cref="VisuallyInheritableList"/>, without
    /// support for visual inheritance.
    /// </summary>
    /// <param name="items">An array of integers.</param>
    public VisuallyInheritableIntList( int[ ] items )
    {
      foreach( int item in items )
      {
        this.Add( item );
      }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="VisuallyInheritableList"/> with support for
    /// visual inheritance provided based on the specified parent's design state.
    /// </summary>
    /// <param name="parent">A reference to the <see cref="T:Syncfusion.ComponentModel.IDesignable"/> interface
    /// that typically contains this collection.</param>
    public VisuallyInheritableIntList( IDesignable parent )
      : base( parent )
    {
    }

    /// <summary>
    /// Custom indexer of type int. See <see cref="M:System.Collections.ArrayList.Item"/> for more information.
    /// </summary>
    public new int this[ int index ]
    {
      get
      {
        return ( int )base[ index ];
      }
      set
      {
        base[ index ] = value;
      }
    }

    /// <summary>
    /// Lets you add an array of integers into the list.
    /// </summary>
    /// <param name="value">An integer array.</param>
    /// <remarks><para>Available to enable serialization using AddRange in designer.</para></remarks>
    public void AddRange( int[ ] value )
    {
      foreach( int intVal in value )
      {
        Add( intVal );
      }
    }

    /// <summary>
    /// Overridden. See <see cref="M:Syncfusion.Collections.VisuallyInheritableList:IsIdentiacalObjects"/>.
    /// </summary>
    /// <param name="item1">Object 1.</param>
    /// <param name="item2">Object 2.</param>
    /// <returns>True if identical; False otherwise.</returns>
    protected override bool IsIdenticalObjects( object item1, object item2 )
    {
      if( ( int )item1 == ( int )item2 )
      {
        return true;
      }
      else
      {
        return false;
      }
    }
  }

  /// <summary>
  /// A list deriving from <see cref="ArrayListExt"/> with a strongly-typed indexer of type int.
  /// </summary>
  public class IntList : ArrayListExt
  {
    /// <summary>
    /// Overloaded. Creates a new instance of the IntList class.
    /// </summary>
    public IntList()
    {
    }

    /// <summary>
    /// Creates a new instance of the IntList class and inserts
    /// the items specified in array into the IntList.
    /// </summary>
    /// <param name="items">An array of integers.</param>
    public IntList( int[ ] items )
    {
      foreach( int item in items )
      {
        this.Add( item );
      }
    }

    /// <summary>
    /// Custom indexer of type int. See <see cref="M:System.Collections.ArrayList.Item"/> for more information.
    /// </summary>
    public new int this[ int index ]
    {
      get
      {
        return ( int )base[ index ];
      }
      set
      {
        base[ index ] = value;
      }
    }

    /// <summary>
    /// Lets you add an array of integers into the list.
    /// </summary>
    /// <param name="value">An integer array.</param>
    /// Available to enable serialization via AddRange in designer.
    public void AddRange( int[ ] value )
    {
      foreach( int intVal in value )
      {
        Add( intVal );
      }
    }
  }

  /// <summary></summary>
  [ DocumentationExclude(),
  TypeConverter( typeof( IntListConverter ) ) ]
  public class IntListDesignTime : IntList
  {
    /// <summary></summary>
    public IntListDesignTime()
    {
    }

    /// <summary></summary>
    /// <param name="items"/>
    public IntListDesignTime( int[ ] items )
    {
      foreach( int item in items )
      {
        this.Add( item );
      }
    }
  }

  /// <summary>
  /// An <see cref="ArrayListExt"/> derived class that supports creating "visually inheritable lists".
  /// </summary>
  /// <remarks><para>
  /// A "visually inheritable list" is one that keeps track of and persists in code, 
  /// the incremental changes applied on the list in a derived class's designer.
  /// </para><para>
  /// You should typically not use the methods and properties exposed by this class in 
  /// your code. The properties are meant to be used during design-time to store the
  /// incremental changes done at design-time.
  /// </para></remarks>
  public class VisuallyInheritableList : ArrayListExt
  {
    // Keep track of new items added by the derived class, so that they can be removed from the base class copy
    // as they get removed.
    /// <summary></summary>
    private ArrayList newItemsInDerivedClass = null;
    /// <summary></summary>
    private IDesignable parent = null;
    /// <summary></summary>
    private ArrayList designTimeBaseClassCopy = null;
    /// <summary></summary>
    private bool bApplyingDesignTimeChanges = false;

    /// <summary>
    /// Overloaded. Creates a new instance of the <see cref="VisuallyInheritableList"/> without any support
    /// for visual inheritance.
    /// </summary>
    public VisuallyInheritableList()
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="VisuallyInheritableList"/> class and inserts
    /// the items specified in the array into the VisuallyInheritableList without any
    /// support for visual inheritance.
    /// </summary>
    /// <param name="items">An array of objects.</param>
    public VisuallyInheritableList( object[ ] items )
      : base( items )
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="VisuallyInheritableList"/> with support for
    /// visual inheritance provided based on the specified parent's design state.
    /// </summary>
    /// <param name="parent">A reference to the <see cref="T:Syncfusion.ComponentModel.IDesignable"/> interface
    /// that typically contains this collection.</param>
    /// <remarks><para>
    /// This list keeps track of the incremental changes happening in a designer by
    /// following the parent's <see cref="IDesignable.DesignMode"/> property.
    /// </para></remarks>
    public VisuallyInheritableList( IDesignable parent )
    {
      this.parent = parent;
    }

    /// <summary></summary>
    private ArrayList BaseClassCopy
    {
      get
      {
        return this.designTimeBaseClassCopy;
      }
    }

    /// <summary></summary>
    [ DocumentationExclude() ]
    public void ReinitBaseClassCopy()
    {
      this.designTimeBaseClassCopy = null;
      this.newItemsInDerivedClass = null;
      this.InitBaseClassCopy();
    }

    /// <summary></summary>
    private void InitBaseClassCopy()
    {
      // Do this only when opening the parent in DesignMode.
      if( this.parent != null && this.parent.DesignMode )
      {
        if( this.designTimeBaseClassCopy == null )
        {
          this.designTimeBaseClassCopy = new ArrayList( this );
        }
        if( this.newItemsInDerivedClass == null )
        {
          this.newItemsInDerivedClass = new ArrayList();
        }
      }
    }

    /// <summary>
    /// Gets / sets the associated parent.
    /// </summary>
    public IDesignable Parent
    {
      get
      {
        return this.parent;
      }
      set
      {
        this.parent = value;
      }
    }
    /// <summary>
    /// Gets  / sets the incremental changes done at design-time.
    /// </summary>
    public IntListDesignTime DesignTimeChanges
    {
      get
      {
        if( this.BaseClassCopy == null )
        {
          return null;
        }
        else
        {
          return this.GetDesignTimeChanges();
        }
      }
      set
      {
        this.designTimeBaseClassCopy = null;

        if( value != null )
        {
          this.InitBaseClassCopy();
          this.SetDesignTimeChanges( value );
        }
        else
        {
          this.SetDesignTimeChanges( this.BaseClassCopy );
        }
      }
    }

    /// <summary>
    /// Indicates whether to continue applying incremental-changes done in the 
    /// previous design-time invocation.
    /// </summary>
    /// <returns>True to continue; False otherwise.</returns>
    /// <remarks><para>
    /// This method is called if it seems like items were removed in the base class
    /// since the last invoke of this design-time.
    /// </para></remarks>
    protected virtual bool ShouldContinueIfItemsCountChanged()
    {
      return true;
    }

    /// <summary>
    /// Called to apply the changes made in the previous design-time invoke.
    /// </summary>
    /// <param name="newPositionList">A list specifying the new positions based on the positions of the exisiting items 
    /// set in the base class.</param>
    protected virtual void SetDesignTimeChanges( ArrayList newPositionList )
    {
      if( newPositionList == null )
      {
        return;
      }

      // Don't apply changes if the counts are not the same. This could mean
      // the base class was modified.
      if( newPositionList.Count != this.Count
        && !this.ShouldContinueIfItemsCountChanged() )
      {
        return;
      }

      if( newPositionList.Count == 0 )
      {
        this.Clear();
      }
      else
      {
        int i = 0;
        ArrayList newList = new ArrayList();
        // Init with dummy values.
        for( int count = 0 ; count < newPositionList.Count ; count++ )
        {
          newList.Add( -1 );
        }

        foreach( int index in newPositionList )
        {
          // Make sure the indices are valid.
          if( index < this.Count )
          {
            newList[ i ] = this[ index ];
            i++;
          }
        }

        if( i < newList.Count )
        {
          // Remove extra entries in the temp list.
          newList.RemoveRange( i, newList.Count - i );
        }

        // Suspend events here, otherwise while we rearrange items,
        // the OnCollectionChanged method will be called and we will
        // end up removing items.
        this.bApplyingDesignTimeChanges = true;
        i = 0;
        foreach( object item in newList )
        {
          this[ i ] = item;
          i++;
        }
        if( i < this.Count )
        {
          // Remove extra entries in this list.
          this.RemoveRange( i, this.Count - i );
        }
        this.ResumeEvents( false );
        this.bApplyingDesignTimeChanges = false;
      }
    }

    /// <summary>
    /// Returns a list representing the incremental changes made at design-time.
    /// </summary>
    /// <returns>A list representing the new position of the items based on the position of the items in the base class.</returns>
    /// <remarks><para>This list will be provided in a call to <see cref="SetDesignTimeChanges"/>
    /// to reapply the changes done during this design-time.</para></remarks>
    protected virtual IntListDesignTime GetDesignTimeChanges()
    {
      if( this.BaseClassCopy == null || this.BaseClassCopy.Count == 0 )
      {
        return null;
      }

      IntListDesignTime indicesList = new IntListDesignTime();

      //For each item in the current list, look up its position in the base class copy.
      for( int i = 0 ; i < this.Count ; i++ )
      {
        object item = this[ i ];
        int j = 0;
        for( ; j < this.designTimeBaseClassCopy.Count ; j++ )
        {
          object baseItem = this.designTimeBaseClassCopy[ j ];
          if( this.IsIdenticalObjects( item, baseItem ) )
          {
            indicesList.Add( j );
            break;
          }
        }
        if( j == this.designTimeBaseClassCopy.Count )
        {
          // Did not find the base class copy! Insert this item into the base-class-copy
          this.designTimeBaseClassCopy.Add( item );
          indicesList.Add( j );
        }
      }

      return indicesList;
    }

    /// <summary>
    /// Compares two objects for equality.
    /// </summary>
    /// <param name="item1">Object 1.</param>
    /// <param name="item2">Object 2.</param>
    /// <returns>True if identical; False otherwise.</returns>
    /// <remarks><para>The base class version uses the "==" operator to perform the 
    /// comparison. This could be overridden to support boxed types (int, for example),
    /// where 2 integers should be compared based on their value rather than the boxed object instance.
    /// </para></remarks>
    protected virtual bool IsIdenticalObjects( object item1, object item2 )
    {
      return item1 == item2;
    }

    /// <summary>
    /// Overridden. See <see cref="M:Syncfusion.Collections.ArrayListExt.OnCollectionChanged"/>.
    /// </summary>
    protected override void OnCollectionChanging()
    {
      this.InitBaseClassCopy();

      base.OnCollectionChanging();
    }

    /// <summary></summary>
    /// <param name="args"/>
    protected override void OnCollectionChanged( CollectionChangeEventArgs args )
    {
      if( !this.bApplyingDesignTimeChanges )
      {
        // Keep track of elementes added in the derived class so that you can remove
        // them from the base class copy as they get removed.
        if( args.Action == CollectionChangeAction.Add && this.newItemsInDerivedClass != null
          && ( this.designTimeBaseClassCopy == null || !this.designTimeBaseClassCopy.Contains( args.Element ) ) )
        {
          this.newItemsInDerivedClass.Add( args.Element );
        }

        if( args.Action == CollectionChangeAction.Remove && this.designTimeBaseClassCopy != null
          && this.newItemsInDerivedClass != null )
        {
          if( this.newItemsInDerivedClass.Contains( args.Element ) )
          {
            this.newItemsInDerivedClass.Remove( args.Element );
            this.designTimeBaseClassCopy.Remove( args.Element );
          }
        }
      }
      base.OnCollectionChanged( args );
    }
  }

  /// <summary></summary>
  [ DocumentationExclude() ]
  public class IntListConverter : TypeConverter
  {
    /// <summary></summary>
    /// <returns></returns>
    /// <param name="context"/>
    /// <param name="culture"/>
    /// <param name="value"/>
    /// <param name="destinationType"/>
    public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
    {
      if( destinationType == typeof( InstanceDescriptor )
        && value is IntListDesignTime )
      {
        IntListDesignTime intList = ( IntListDesignTime )value;
        Type[ ] args;
        args = new Type[1];

        args[ 0 ] = typeof( int[ ] );

        ConstructorInfo constructorInfo;
        constructorInfo = typeof( IntListDesignTime ).GetConstructor( args );
        if( constructorInfo != null )
        {
          object[ ] argValues;
          argValues = ( object[ ] )new Object[1];
          int[ ] intArray = new int[intList.Count];
          for( int i = 0 ; i < intList.Count ; i++ )
          {
            intArray[ i ] = intList[ i ];
          }
          argValues[ 0 ] = intArray;

          return ( object )new InstanceDescriptor( constructorInfo, argValues );
        }
      }
      return base.ConvertTo( context, culture, value, destinationType );
    }

    /// <summary></summary>
    /// <returns></returns>
    /// <param name="context"/>
    /// <param name="destinationType"/>
    public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
    {
      if( destinationType == typeof( InstanceDescriptor ) )
      {
        return true;
      }

      return base.CanConvertTo( context, destinationType );
    }
  }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
  /// <summary>
  /// An ArrayListExt instance that will enforce the type of objects that can be added to the array during runtime.
  /// </summary>
  /// <typeparam name="T">Specifies the type of objects that can be added to the array.</typeparam>
  public class TypedArrayListExt<T> : ArrayListExt
  {
    protected override void AddHandlers(object item)
    {
      if (!(item is T)) 
        throw new ArgumentException("The item added to the ArrayList is not of appropriate type. It should be of type " + typeof(T).Name + ".");
      
      base.AddHandlers(item);
    }

    /// <summary>
    /// An indexer of the specified template type.
    /// </summary>
    /// <param name="index">The index of the object in the list that you want to retrieve.</param>
    /// <returns>The object at the specified index.</returns>
    public new T this[ int index ]
    {
      get 
      { 
        return ( T )base[ index ];
      }
      set 
      { 
        base[index] = value; 
      }
    }
  }
#endif
}