#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using Syncfusion.Windows.Forms.Tools.Design;
using System.ComponentModel.Design.Serialization;

namespace Syncfusion.Windows.Forms.Tools
{
	#region Enums

		public enum CaptionButtonType
		{
			/// <summary>
			/// Closing control.
			/// </summary>
				Close,
			/// <summary>
			/// Auto-hiding control.
			/// </summary>
				Pin,
			/// <summary>
			/// Showing popup menu.
			/// </summary>
				Menu,
			/// <summary>
			/// Maximizing control. May be used alone or in pair with Restore button.
			/// </summary>
				Maximize,
			/// <summary>
			/// Restoring control's previous size. May be used only in pair with Maximize button.
			/// </summary>
				Restore,
			/// <summary>
			/// User-defined button.
			/// </summary>
				Custom
		}

		/// <summary>
		/// Specifies the Caption Button state of the docking windows.
		/// </summary>
	public enum CaptionButtonState
	{
		/// <summary>
		/// Normal state of Caption Button.
		/// </summary>
		Normal, 
		/// <summary>
		/// Active state of Caption Button.
		/// </summary>
		Active, 
		/// <summary>
		/// Pushed state of Caption Button.
		/// </summary>
		Pushed 
	}

	#endregion

	[DesignerSerializer(typeof(CaptionButtonsCodeDomSerializer), typeof(CodeDomSerializer))]
    public class CaptionButtonsCollection : CollectionBase
    {
		#region Indexer

		public CaptionButton this[int index]
		{
			get
			{
				return this.List[ index ] as CaptionButton;
			}
			set
			{
				if(index < 0 || index > List.Count)
				{
					throw new IndexOutOfRangeException("Incorrect index.");
				}
				if( value == null )
				{
					throw new NullReferenceException("Incorrect value (null).");
				}
				if( List[ index ] != value )
				{
					List[ index ] = value;
				}
			}
		}

		public CaptionButton this[ string name ]
		{
			get
			{
				if( name == null )
				{
					throw new NullReferenceException( "Index." );
				}
				for( int i = 0; i < List.Count; i++ )
				{
					if( name == this[i].Name )
					{
						return this[i];
					}
				}
				return null;
			}
			set
			{
				if( name == null )
				{
					throw new NullReferenceException( "Index." );
				}
				if( value == null )
				{
					throw new NullReferenceException("Incorrect value (null).");
				}
				for( int i = 0; i < List.Count; i++ )
				{
					if( name == this[i].Name )
					{
						if( List[i] != value )
						{
							List[i] = value;
						}
					}
				}
			}
		}

		#endregion

		#region Protected/Private Methods

		protected void OnCollectionChanged()
		{
			if( this.CollectionChanged != null )
				this.CollectionChanged( this, EventArgs.Empty );
		}

		protected void OnCollectionItemChanged()
		{
			if( this.CollectionItemChanged != null )
			{
				this.CollectionItemChanged( this, EventArgs.Empty );
			}
		}
		protected override void  OnRemove(int index, object value)
		{
			CaptionButton button = value as CaptionButton;
			if (button != null)
			{
				button.Changed -= new EventHandler(button_Changed);
			}
			OnCollectionChanged();
			base.OnRemove(index, value);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			OnCollectionChanged();
			base.OnRemoveComplete(index, value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			CaptionButton button = value as CaptionButton;
			if (button != null && button.Name == null)
			{
				button.Name = this.GetCaptionButtonDefaultName();
				button.Changed += new EventHandler(button_Changed);
				button.TypeChanging += new CancelEventHandler(button_TypeChanging);
			}
			OnCollectionChanged();
			base.OnInsertComplete(index, value);
		}

		protected override void OnClear()
		{
			for (int i = 0; i < Count; i++)
			{
				CaptionButton button = this[i] as CaptionButton;
				if (button != null)
				{
					button.Changed -= new EventHandler(button_Changed);
				}
			}
			OnCollectionChanged();
			base.OnClear();
		}

		protected override void OnClearComplete()
		{
			OnCollectionChanged();
			base.OnClearComplete();
		}

		private void button_Changed(object sender, EventArgs e)
		{
			OnCollectionItemChanged();
		}

		protected virtual void button_TypeChanging(object sender, CancelEventArgs e)
		{
			CaptionButton curButton = sender as CaptionButton;
			if (curButton != null && 
				(curButton.Type == CaptionButtonType.Maximize || curButton.Type == CaptionButtonType.Restore))
			{
				foreach (CaptionButton button in List)
				{
					if (button.Type == curButton.Type && button != curButton)
					{
						e.Cancel = true;
						break;
					}
				}
			}
		}

		protected internal bool ContainsName(string name)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].Name == name)
					return true;
			}
			return false;
		}

		protected string GetCaptionButtonDefaultName()
		{
			int num = 1;
			while (this.ContainsName("CaptionButton" + num.ToString()))
			{
				num++;
			}
			return "CaptionButton" + num.ToString();
		}

		protected internal bool ContainsButtonType( CaptionButtonType type )
	{
		foreach (CaptionButton button in List)
		{
			if (button.Type == type)
			{
				return true;
			}
		}
		return false;
	}

		#endregion

		#region SuperToolTip Helper Methods

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private bool EqualToolTipInfo(ToolTipInfo tti1, ToolTipInfo tti2)
		{
			if (tti1.BackColor == tti2.BackColor && tti1.BorderColor == tti2.BorderColor
				&& tti1.ForeColor == tti2.ForeColor && tti1.Separator == tti2.Separator
				&& EqualToolTipItem( tti1.Footer, tti2.Footer )
				&& EqualToolTipItem(tti1.Header, tti2.Header)
				&& EqualToolTipItem(tti1.Body, tti2.Body) )
				return true;
			else
				return false;
		}

	private bool EqualToolTipItem(ToolTipInfo.ToolTipItem tti1, ToolTipInfo.ToolTipItem tti2)
	{
		if (tti1.Bounds == tti2.Bounds && tti1.Font == tti2.Font
				&& tti1.ForeColor == tti2.ForeColor && tti1.Hidden == tti2.Hidden
				&& tti1.Image == tti2.Image && tti1.ImageAlign == tti2.ImageAlign
				&& tti1.ImageBounds == tti2.ImageBounds && tti1.ImageScalingSize == tti2.ImageScalingSize
				&& tti1.ImageTransparentColor == tti2.ImageTransparentColor && tti1.Text == tti2.Text
				&& tti1.TextAlign == tti2.TextAlign && tti1.TextBounds == tti2.TextBounds
				&& tti1.TextImageRelation == tti2.TextImageRelation && tti1.TextMargin == tti2.TextMargin)
			return true;
		else
			return false;
	}
#endif

		internal bool NeedSerializeToolTipInfo()
		{
			bool res = false;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			ToolTipInfo ttiDefault = new ToolTipInfo();
			for (int i = 0; i < Count; i++)
			{
				if (EqualToolTipInfo(ttiDefault, this[i].SuperToolTipInfo) == false)
				{
					res = true;
					break;
				}
			}
#endif
			return res;
		}

		#endregion

		#region Public Methods

        /// <summary>
        /// Adds the caption button to the control.
        /// </summary>
        /// <param name="button"></param>
		public virtual void Add( CaptionButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException("Incorrect argument (null).");
			}

            if( (button.Type != CaptionButtonType.Custom) && ContainsButtonType( button.Type ) )
			{
				return; // Exit if collection already contains button of this type.
			}

			List.Add( button );
			button.Changed += new EventHandler(button_Changed);
			button.TypeChanging += new CancelEventHandler(button_TypeChanging);
			OnCollectionChanged();
		}

		public bool Contains( CaptionButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException( "Incorrect argument (null)." );
			}

			return List.Contains( button );
		}

		public void Remove( CaptionButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException("Incorrect argument (null).");
			}
			if( this.Contains(button) == false )
			{
				throw new ArgumentException( "Not in list." );
			}

			List.Remove( button );
			button.Changed -= new EventHandler(button_Changed);
			button.TypeChanging -= new CancelEventHandler(button_TypeChanging);
			OnCollectionChanged();
		}

		public int IndexOf( CaptionButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException("Incorrect argument (null).");
			}

				return List.IndexOf(button);
		}

		public virtual void Insert( int index, CaptionButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException("Incorrect argument (null).");
			}
			if( index < 0 || index > List.Count )
			{
				throw new ArgumentException("Incorrect index");
			}
			if ((button.Type == CaptionButtonType.Maximize || button.Type == CaptionButtonType.Restore) && ContainsButtonType(button.Type))
			{
				return; // Exit if collection already contains button of this type.
			}

			List.Insert( index, button );
			button.Changed += new EventHandler(button_Changed);
			button.TypeChanging += new CancelEventHandler(button_TypeChanging);
			OnCollectionChanged();
		}

		public void Dispose()
		{
			foreach( CaptionButton button in List )
			{
				button.Changed -= new EventHandler(button_Changed);
				button.TypeChanging -= new CancelEventHandler(button_TypeChanging);
			}
			List.Clear();
		}

		public CaptionButtonsCollection Clone()
		{
			CaptionButtonsCollection collection = new CaptionButtonsCollection();
			for (int i = 0; i < Count; i++)
			{
				collection.Add(this[i].Clone());
			}
			return collection;
		}

        /// <summary>
        /// Serves to merge with caption buttons collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="bCloneButtons"></param>
		public void MergeWith(CaptionButtonsCollection collection, bool bCloneButtons)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				if (ContainsName(collection[i].Name))
				{
					if (bCloneButtons)
						this[collection[i].Name] = collection[i].Clone();
					else
						this[collection[i].Name] = collection[i];
				}
				else
				{
					if( bCloneButtons )
						Add(collection[i].Clone());
					else
						Add(collection[i]);
				}
			}
		}

		public void ExcludeCommonButtonsWith(CaptionButtonsCollection collection)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				if (ContainsName(collection[i].Name))
				{
					Remove(this[collection[i].Name]);
				}
			}
		}

		#endregion

		#region Events

		public event EventHandler CollectionChanged;
		public event EventHandler CollectionItemChanged;

		#endregion
	}

	public class CaptionButtonOptionsCollection : CollectionBase
	{
		#region Indexer

		public CaptionButtonOptions this[int index]
		{
			get
			{
				return this.List[ index ] as CaptionButtonOptions;
			}
			set
			{
				if(index < 0 || index > List.Count)
				{
					throw new IndexOutOfRangeException("Incorrect index.");
				}
				if( value == null )
				{
					throw new NullReferenceException("Incorrect value (null).");
				}
				if( List[ index ] != value )
				{
					List[ index ] = value;
				}
			}
		}

		#endregion

		#region Protected/Private Methods

		protected void OnCollectionChanged()
		{
			if( this.CollectionChanged != null )
				this.CollectionChanged( this, EventArgs.Empty );
		}

		#endregion

		#region Public Methods

		public void Add( CaptionButtonOptions options )
		{
			if( options == null )
			{
				throw new NullReferenceException("Incorrect argument (null).");
			}

			List.Add( options );
			OnCollectionChanged();
		}

		public bool Contains( CaptionButtonOptions options )
		{
			if( options == null )
			{
				throw new NullReferenceException( "Incorrect argument (null)." );
			}

			return List.Contains( options );
		}

		public void Remove( CaptionButtonOptions options )
		{
			if( options == null )
			{
				throw new NullReferenceException("Incorrect argument (null).");
			}
			if( this.Contains(options) == false )
			{
				throw new ArgumentException( "Not in list." );
			}

			List.Remove( options );
			OnCollectionChanged();
		}

		public int IndexOf( CaptionButtonOptions options )
		{
			if( options == null )
			{
				throw new NullReferenceException("Incorrect argument (null).");
			}

			return List.IndexOf(options);
		}

		public void Insert( int index, CaptionButtonOptions options )
		{
			if( options == null )
			{
				throw new NullReferenceException("Incorrect argument (null).");
			}
			if( index < 0 || index > List.Count )
			{
				throw new ArgumentException("Incorrect index");
			}

			List.Insert( index, options );
			OnCollectionChanged();
		}
		#endregion

		#region Events

		public event EventHandler CollectionChanged;

		#endregion
	}

	public class CaptionButtonOptionsTable : IDisposable
	{
		#region Members

		protected CaptionButtonsCollection m_Buttons = new CaptionButtonsCollection();
		protected CaptionButtonOptionsCollection m_Options = new CaptionButtonOptionsCollection();
        protected bool m_isDisposed = false;

		#endregion

		#region Public Methods

		public void Add( CaptionButton button, CaptionButtonOptions options )
		{
			if( button == null )
			{
				throw new ArgumentNullException("Caption button.");
			}

			if( options == null )
			{
				throw new ArgumentNullException("Caption button options.");
			}

			m_Buttons.Add( button );
			m_Options.Add( options );
		}

		public void RemoveAt( int index )
		{
			if( index < 0 || index >= m_Buttons.Count || index >= m_Options.Count  )
			{
				throw new ArgumentException("Invalid index.");
			}
			m_Buttons.RemoveAt( index );
			m_Options.RemoveAt( index );
		}

		public void Clear()
		{
			m_Buttons.Clear();
			m_Options.Clear();
		}

        public void Dispose()
        {
            if (!this.m_isDisposed)
            {
                m_Buttons.Dispose();
                m_Options.Clear();

                m_Buttons = null;
                m_Options = null;
                this.m_isDisposed = true;
            }
        }
		#endregion

		#region Properties

        public bool IsDisposed
        {
            get
            {
                return this.m_isDisposed;
            }
        }

		public CaptionButtonsCollection Buttons
		{
			get
			{
				return m_Buttons;
			}
		}

		public CaptionButtonOptionsCollection Options
		{
			get
			{
				return m_Options;
			}
		}

		#endregion
	}

    public class CaptionButton
    {
		#region Members

		protected string m_name;
		protected string m_ToolTip = "";
		protected bool m_bModified = false;
		protected Color m_clTransparentImage = Color.Transparent;
		protected int m_ImageIndex = -1;
		protected CaptionButtonType m_cbtType;

	#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		protected ToolTipInfo m_toolTipInfo = new ToolTipInfo();
	#endif

		#endregion

		#region Constructors

		public CaptionButton() : this( CaptionButtonType.Custom ){}

		public CaptionButton( CaptionButtonType type )
		{
			this.m_cbtType = type;
			switch (type)
			{
				case CaptionButtonType.Close:
					m_name = "CloseButton";
                    m_ToolTip = SR.GetString( SR.ToolTipCaptionButtonClose,this ); 
					break;
				case CaptionButtonType.Pin:
					m_name = "PinButton";
                    m_ToolTip = SR.GetString( SR.ToolTipCaptionButtonPin, this); 
					break;
				case CaptionButtonType.Menu:
					m_name = "MenuButton";
                    m_ToolTip = SR.GetString( SR.ToolTipCaptionButtonMenu, this); 
					break;
				case CaptionButtonType.Maximize:
					m_name = "MaximizeButton";
                    m_ToolTip = SR.GetString( SR.ToolTipCaptionButtonMaximize, this); 
					break;
				case CaptionButtonType.Restore:
					m_name = "RestoreButton";
                    m_ToolTip = SR.GetString( SR.ToolTipCaptionButtonRestore, this);
					break;
			}
		}

		public CaptionButton( CaptionButtonType type, string name )
		{
			this.m_cbtType = type;
			this.m_name = name;
			switch (type)
			{
				case CaptionButtonType.Close:
                    m_ToolTip = SR.GetString(SR.ToolTipCaptionButtonClose, this);
					break;
				case CaptionButtonType.Pin:
                    m_ToolTip = SR.GetString(SR.ToolTipCaptionButtonPin, this);
					break;
				case CaptionButtonType.Menu:
                    m_ToolTip = SR.GetString(SR.ToolTipCaptionButtonMenu, this);
					break;
				case CaptionButtonType.Maximize:
                    m_ToolTip = SR.GetString(SR.ToolTipCaptionButtonMaximize, this);
					break;
				case CaptionButtonType.Restore:
                    m_ToolTip = SR.GetString(SR.ToolTipCaptionButtonRestore, this);
					break;
			}
		}

		public CaptionButton( CaptionButtonType type, string name, int imageIndex, Color clr, string tooltip ) : this( type, name )
		{
			this.m_clTransparentImage = clr;
			this.m_ImageIndex = imageIndex;
			this.m_ToolTip = tooltip;
		}

		#endregion

		#region Properties
		#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			/// <summary>
			/// Gets / sets customizable SuperToolTip drawing info used by the caption button.
			/// </summary>
			/// <value>A <see cref="Syncfusion.Windows.Forms.Tools.ToolTipInfo"/> used to draw supertooltip for current button.</value>
			[
			DefaultValue(null),
			Description("SuperToolTip drawing info associated with button.")
			]
		public ToolTipInfo SuperToolTipInfo
		{
			get
			{
				return m_toolTipInfo;
			}
			set
			{
				if( m_toolTipInfo != value )
				{
					m_toolTipInfo = value;
				}
			}
		}
		#endif
		/// <summary>
		/// Gets/Sets the name of this button.
		/// </summary>
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				if( m_name != value )
				{
					m_name = value;
				}
			}
		}

		/// <summary>
		/// Gets/sets text that will be displayed in ToolTip when cursor is over this button.
		/// </summary>
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				return m_ToolTip;
			}
			set
			{
				if( m_ToolTip != value )
				{
					m_ToolTip = value;
				}
			}
		}

		/// <summary>
		/// Indicates what color in button's image should be transparent.
		/// </summary>
		[DefaultValue(typeof(Color), "Transparent")]
		public Color TransparentImageColor
		{
			get
			{
				return m_clTransparentImage;
			}
			set
			{
				if( m_clTransparentImage != value )
				{
					m_clTransparentImage = value;
					OnRepaint();
				}
			}
		}

		/// <summary>
		/// Indicates index of image to use when displaying this button.
		/// External ImageList should be used.
		/// </summary>
		[DefaultValue(-1)]
		public int ImageIndex
		{
			get
			{
				return m_ImageIndex;
			}
			set
			{
				if( m_ImageIndex != value )
				{
					m_ImageIndex = value;
					OnRepaint();
				}
			}
		}

		/// <summary>
		/// Indicates type of this button.
		/// </summary>
		[DefaultValue(CaptionButtonType.Custom)]
		public CaptionButtonType Type
		{
			get
			{
				return m_cbtType;
			}
			set
			{
				if( m_cbtType != value )
				{
					CancelEventArgs args = new CancelEventArgs(false);
					CaptionButtonType oldType = m_cbtType;
					m_cbtType = value;
					OnTypeChanging( args );
					if (args.Cancel)
					{
						m_cbtType = oldType;
						throw new DockingManagerException("Adding to collection more than one button of type Maximize or Restore is not allowed.");
					}
					else
					{
						OnRepaint();
					}
				}
			}
		}

		protected internal bool Modified
		{
			get { return m_bModified; }
			set 
			{
				if (m_bModified != value)
				{
					m_bModified = value;
				}
			}
		}

		#endregion
		
		#region Methods

			protected void OnClick( CancelEventArgs args )
			{
				if( this.Click != null )
					this.Click( this, args );
			}

			public void FireClickEvent( CancelEventArgs e )
			{
				this.OnClick(e);
			}

			protected void OnTypeChanging(CancelEventArgs args)
			{
				if (this.TypeChanging != null)
				{
					this.TypeChanging(this, args);
				}
			}

			protected void OnRepaint()
			{
				if( this.Changed != null )
				{
					this.Changed( this, EventArgs.Empty );
				}
			}

			public CaptionButton Clone()
			{
				return new CaptionButton(Type, Name, ImageIndex, TransparentImageColor, ToolTip);
			}

			#endregion

		#region Events

			public event CancelEventHandler Click;
			public event EventHandler Changed;
			protected internal event CancelEventHandler TypeChanging;

			#endregion

		#region Static methods

			public static bool HasDefaultValue( CaptionButton button )
			{
				bool res = false;
				if (button.ImageIndex < 0 && button.TransparentImageColor == Color.Transparent)
				{
					if (button.Type == CaptionButtonType.Close && button.ToolTip == "Close")
						res = true;
					else if (button.Type == CaptionButtonType.Pin && button.ToolTip == "Auto Hide")
						res = true;
					else if (button.Type == CaptionButtonType.Menu && button.ToolTip == "Window Position")
						res = true;
					else if (button.Type == CaptionButtonType.Maximize && button.ToolTip == "Maximize")
						res = true;
					else if (button.Type == CaptionButtonType.Restore && button.ToolTip == "Restore")
						res = true;
					else if (button.Type == CaptionButtonType.Custom && button.ToolTip == "")
						res = true;
				}
				return res;
			}

			#endregion
    }

	public class CaptionButtonOptions
	{
		#region Members

		protected bool m_bModifiedView = false;

		#endregion

		#region Properties

		public bool ModifiedView
		{
			get
			{
				return m_bModifiedView;
			}
			set
			{
				if( m_bModifiedView != value )
				{
					m_bModifiedView = value;
				}
			}
		}

		#endregion

		#region Consructors

		public CaptionButtonOptions()
		{}

		public CaptionButtonOptions( bool bmodified )
		{
			this.m_bModifiedView = bmodified;
		}

		#endregion
	}
}