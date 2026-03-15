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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Collections
{
	/// <summary></summary>
	[
	Serializable(),
	Editor( typeof( ControlsCollectionEditor ), typeof( UITypeEditor ) )
	]
	public class ControlsCollection
		: CollectionBase
	{
		#region Class members
		/// <summary>Reference on parent control</summary>
		private Control m_parent;
		#endregion

		#region Class properties
		/// <summary>Reference on parent control/container.</summary>
		protected Control Parent
		{
			get
			{
				return m_parent;
			}
		}

        /// <summary>Typed version of indexer.</summary>
		public Control this[ int index ]
		{
			get
			{
				return this.List[ index ] as Control;
			}
			set
			{
				this.List[ index ] = value;
			}
		}
		#endregion

		#region class events
		/// <summary></summary>
        public event CollectionChangeEventHandler CollectionChanged;
        /// <summary></summary>
        public event ConfigureControlEventHandler ConfigureControl;
		#endregion

		#region class initialize\finalize methods
        /// <summary>
        /// Initializes new instance of ControlsCollectionBase class
        /// </summary>
        /// <param name="parent">Reference on parent control. Can not be NULL.</param>
        public ControlsCollection( Control parent )
		{
          if( null == parent )
            throw new ArgumentNullException( "parent" );
        	  
          m_parent = parent;
		}
        /// <summary>
		/// Initializes new instance of ControlsCollectionBase class
		/// </summary>
        /// <param name="parent">Reference on parent control. Can not be NULL.</param>
        /// <param name="eventHandler">Attach event handler to ConfigureControl event on initialize.</param>
		public ControlsCollection( Control parent, ConfigureControlEventHandler eventHandler ) : 
            this( parent )
		{
			if( null == eventHandler )
				throw new ArgumentNullException( "handler" );

          this.ConfigureControl += eventHandler;
        }
		#endregion

		#region class event risers
		/// <summary>Occurs when collection changed.</summary>
		/// <param name="e"></param>
		protected virtual void OnCollectionChanged( CollectionChangeEventArgs e )
		{
			if( null != CollectionChanged )
			{
				CollectionChanged( this, e );
			}
		}

		/// <summary></summary>
		/// <param name="control"></param>
		protected virtual void ConfigureItem( Control control )
		{
			if( null != ConfigureControl )
			{
				ConfigureControl( this, new ConfigureControlEventArgs( control ) );
			}
		}
		#endregion

		#region class public methods
		/// <summary>
		/// Add Control object to collection.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public int Add( Control control )
		{
            int index = -1;
            if( !List.Contains( control ) )
            {
                index = this.List.Add( control );
            }
            return index;
		}

		/// <summary></summary>
		/// <param name="controls"></param>
		public void AddRange( Control[ ] controls )
		{
            this.Parent.SuspendLayout();

			for( int i = 0, len = controls.Length ; i < len ; i++ )
			{
                if( !List.Contains( controls[ i ] ) )
				{
                    this.List.Add( controls[ i ] );
                }
			}

            this.Parent.ResumeLayout( true );
		}

		/// <summary>
		/// Removes control from collection.
		/// </summary>
		/// <param name="control"></param>
		public void Remove( Control control )
		{
			if( List.Contains( control ) )
			{
				this.List.Remove( control );
			}
		}

		/// <summary>
		/// Insert control in position with index value is index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="control"></param>
		public void Insert( int index, Control control )
		{
            if( !List.Contains( control ) )
			{
                this.List.Insert( index, control );
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool Contains( Control control )
        {
            return this.List.Contains( control );
        }

        /// <summary>
        /// Includes/exclude controls from parent controls collection.
        /// </summary>
        /// <param name="hide">If true - excludes, else includes controls in parent controls collection.</param>
        internal void HideControls( bool hide )
        {
            if( hide )
            {
                int count = List.Count;
                for (int i = 0; i < count; i++)
                {
                    Control control = List[ i ] as Control;
                    if( this.Parent.Contains( control ) )
                    {
                        this.Parent.Controls.Remove( control );
                    }
                }
            }
            else
            {
                for (int i = 0; i < List.Count; i++)
                {
                    this.Parent.Controls.Add( List[i] as Control );
                }
            }
        }
		#endregion

		#region class overrides
		/// <summary></summary>
		protected override void OnClear()
		{
			// cleanup collection
			this.Parent.SuspendLayout();

			int count = this.InnerList.Count;
			for( int i = 0 ; i < count ; i++ )
			{
				this.Parent.Controls.Remove( InnerList[ 0 ] as Control );
			}

			this.Parent.ResumeLayout( true );

			base.OnClear();

            CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Refresh, null );
            OnCollectionChanged( args );
		}

		/// <summary></summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsertComplete( int index, object value )
		{
			Control control = ( Control )value;

			if( !this.Parent.Controls.Contains( control ) )
			{
				this.Parent.Controls.Add( control );
				ConfigureItem( control );
			}

			base.OnInsertComplete( index, value );

            CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Add, control );
            OnCollectionChanged( args );
		}

		/// <summary></summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnRemoveComplete( int index, object value )
		{
			Control control = ( Control )value;

			if( this.Parent.Controls.Contains( control ) )
			{
				this.Parent.Controls.Remove( control );
			}

            if( this.Parent is ScrollBarCustomDraw )
            {
                ( this.Parent as ScrollBarCustomDraw ).Layout();
            }

			base.OnRemoveComplete( index, value );

            CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Remove, control );
            OnCollectionChanged( args );
		}

		/// <summary></summary>
		/// <param name="index"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected override void OnSetComplete( int index, object oldValue, object newValue )
		{
			Control ctrlNew = ( Control )newValue;
			Control ctrlOld = ( Control )oldValue;

			this.Parent.SuspendLayout();
			this.Parent.Controls.Remove( ctrlOld );

			if( !this.Parent.Controls.Contains( ctrlNew ) )
			{
				this.Parent.Controls.Add( ctrlNew );
				ConfigureItem( ctrlNew );
			}

			this.Parent.ResumeLayout();

			base.OnSetComplete( index, oldValue, newValue );

			CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Refresh, ctrlNew );
            OnCollectionChanged( args );
		}

		#endregion
	}

  /// <summary>Message class that contains reference on control that 
  /// required configuration.</summary>
  public class ConfigureControlEventArgs : EventArgs
  {
    #region Class members
    /// <summary>Storage of control reference.</summary>
    private Control m_control;
    #endregion

    #region Class properties
    /// <summary>Get reference on Control.</summary>
    public Control Control
    {
      get
      {
        return m_control;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>Default constructor.</summary>
    /// <param name="control"></param>
    public ConfigureControlEventArgs( Control control )
    {
      m_control = control;
    }
    #endregion
  }

	/// <summary></summary>
	/// <param name="sender"/>
	/// <param name="args"/>
	public delegate void ConfigureControlEventHandler( object sender, ConfigureControlEventArgs args );

	#region ControlsCollection UITypeEditor
	/// <summary>We specify that only buttons can be created in design time in collection.
	/// In runtime developer will have more freedom.</summary>
	public class ControlsCollectionEditor : CollectionEditor
	{
		/// <summary>Default constructor.</summary>
		/// <param name="type"></param>
		public ControlsCollectionEditor( Type type ) : base( type )
		{
		}

		/// <summary>Allow creation only of buttons in designe time.</summary>
		/// <returns>Allowed types for creation.</returns>
		protected override Type[ ] CreateNewItemTypes()
		{
			return new Type[ ]{typeof( ButtonAdv )};
		}
	}
	#endregion
}