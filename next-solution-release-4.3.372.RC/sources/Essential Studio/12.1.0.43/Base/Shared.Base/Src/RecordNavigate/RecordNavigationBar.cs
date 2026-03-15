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

using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms
{

	/// <summary>
	/// Provides event data for the <see cref="ArrowButtonBar.ArrowButtonClicked"/> event of
	/// a <see cref="ArrowButtonBar"/>, <see cref="RecordNavigationBar"/> or <see cref="RecordNavigationControl"/>
	/// object.
	/// </summary>
	public class ArrowButtonEventArgs: SyncfusionCancelEventArgs
	{
		ArrowType arrow;

		/// <summary>
		/// Initializes a new <see cref="ArrowButtonEventArgs"/> object with event data.
		/// </summary>
		/// <param name="arrow">Specifies the arrow button that was clicked.</param>
		public ArrowButtonEventArgs( ArrowType arrow )
		{
			this.arrow = arrow;
		}

		/// <summary>
		/// Gets / sets the arrow button that was clicked.
		/// </summary>
		[TraceProperty( true )]
		public ArrowType Arrow
		{
			get
			{
				return arrow;
			}
			set
			{
				arrow = value;
			}
		}
	}

	/// <summary>
	///Handles the  <see cref="ArrowButtonBar.ArrowButtonClicked"/> event.
	/// </summary>
	public delegate void ArrowButtonEventHandler( object sender, ArrowButtonEventArgs e );

	/// <summary>
	/// Returns the values of a record navigation bar.
	/// </summary>
	public interface IRecordNavigationBarData
	{
		/// <summary>
		/// Returns the minimum record.
		/// </summary>
		int MinRecord
		{
			get;
		}

		/// <summary>
		/// Returns the maximum record.
		/// </summary>
		int MaxRecord
		{
			get;
		}

		/// <summary>
		/// Indicates whether adding new records is enabled.
		/// </summary>
		bool AllowAddNew
		{
			get;
		}
		//
		//		string MaxLabel
		//		{
		//			get;
		//		}
		// current record
	}

	[ToolboxItem( false )]
	class RecordNavigationBarTextBox: RichTextBox
	{
		public RecordNavigationBarTextBox()
		{
			this.SetStyle( ControlStyles.Selectable, false );
			this.CausesValidation = false;
		}

	}

	/// <summary>
	/// The record navigation bar displays arrow buttons and current record fields.
	/// </summary>
	[ToolboxItem( true )]
	[System.Drawing.ToolboxBitmap( typeof( Syncfusion.Windows.Forms.RecordNavigationBar ), "ToolboxIcons.RecordNavigationBar.bmp" )]
	public class RecordNavigationBar: ArrowButtonBar,
		IInternalButtonParent
	{
		// Fields
		int currentRecord = 0;
		int minRecord = 1;
		int maxRecord = -1;
		bool addNewRecord = true;
		bool bFocused = false;
		RichTextBox textBox = null;
		InternalButton labelButton = null;
		InternalButton labelMaxButton = null;
		Rectangle textArea = Rectangle.Empty;
		int mouseDownTick = int.MaxValue;
		int step = 1;
		IRecordNavigationBarData rnbData;
		bool textBoxVisible = false;
		bool allowStepIncrease = true;
		int correctedYCoord = 0;
		protected IContainer components = new Container();

		// Events
		/// <summary>
		/// Occurs when the current record is changing.
		/// </summary>
		[Description( "Occurs when the current record is changing." )]
		public event CurrentRecordChangedEventHandler CurrentRecordChanging;

		/// <summary>
		/// Occurs when the current record is changed.
		/// </summary>
		[Description( "Occurs when the current record is changed." )]
		public event CurrentRecordChangedEventHandler CurrentRecordChanged;

		/// <summary>
		/// Initializes a new <see cref="RecordNavigationBar"/>.
		/// </summary>
		public RecordNavigationBar()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(RecordNavigationBar));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			this.labelButton = new InternalButton( this, SR.GetString( "RecordNavigationBarRecord" ), SR.GetString( "RecordNavigationBarCurrentRecord" ), new Size(), false );
			this.components.Add( this.labelButton );
			this.labelMaxButton  = new InternalButton( this, "", SR.GetString( "RecordNavigationBarRecordCount" ), new Size(), false );
			this.components.Add( this.labelMaxButton );

			this.CausesValidation = false;
			this.SetStyle( ControlStyles.Selectable, false );
			this.Enabled = true;
		}

		/// <override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				components.Dispose();

				if( this.textBox != null )
				{
					this.textBox.Dispose();
					this.textBox = null;
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Initializes the record field textbox.
		/// </summary>
		/// <param name="setFocus">True if focus should be set to the textbox.</param>
		protected internal virtual void ShowTextBox( bool setFocus )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( setFocus, "----BEGIN----", this );
#endif

			RichTextBox textBox = TextBox;

			textBox.Font = this.Font;
			textBox.BackColor = this.BackColor;
			textBox.ForeColor = this.ForeColor;
			textBox.BorderStyle = BorderStyle.None;
			textBox.AllowDrop = false;
			textBox.AcceptsTab = false;
			textBox.AutoSize = false;
			textBox.Multiline = true;
			textBox.ShowSelectionMargin = false;
			textBox.ScrollBars = RichTextBoxScrollBars.None;
			textBox.Text = this.Enabled ? CurrentRecord.ToString() : "";
			textBox.CausesValidation = false;
			textBox.Location = textArea.Location;
			textBox.Bounds = this.textArea;
			//textBox.RightToLeft = this.RightToLeft;
			textBox.Visible = true;
			textBoxVisible = true;

			if( !Controls.Contains( textBox ) )
			{
				SuspendLayout();
				Controls.Add( textBox );
				ResumeLayout( false );
			}

			if( setFocus && this.Enabled )
			{
				SplitterControl splitterControl = Parent as SplitterControl;
				if( splitterControl != null )
				{
#if DEBUG
					if( Switches.Workbook.TraceVerbose )
						TraceUtil.TraceCurrentMethodInfo( setFocus, "Set ActiveControl", splitterControl );
#endif

					splitterControl.ActiveControl = textBox;
					if( splitterControl.ActiveControl != textBox )
						return;
#if DEBUG
					if( Switches.Workbook.TraceVerbose )
						TraceUtil.TraceCurrentMethodInfo( setFocus, "Before Focus", splitterControl );
#endif

					Stack f = new Stack();
					FixCausesValidation( f );
					splitterControl.Focus();
#if DEBUG
					if( Switches.Workbook.TraceVerbose )
						TraceUtil.TraceCurrentMethodInfo( "Focused:", textBox.Focused, splitterControl );
#endif

					while( f.Count > 0 )
						( (Control)f.Pop() ).CausesValidation = true;
					textBox.Show();
					textBox.Focus();
				}
				else
					textBox.Focus();
#if DEBUG
				if( Switches.Workbook.TraceVerbose )
					TraceUtil.TraceCurrentMethodInfo( setFocus, "After Focus", splitterControl );
#endif

			}

			textBox.Modified = false;
			textBox.Refresh();
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( setFocus, "----END----", this );
#endif

		}

		void TextBoxLeave( object sender, EventArgs e )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#endif

			// Hide without interfering with focus ...
			HideTextBox();
		}

		void TextBoxLostFocus( object sender, EventArgs e )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#endif

			OnLostFocus( EventArgs.Empty );
			if( !inSetCurrentRecord )
				HideTextBox();
		}

		void FixCausesValidation( Stack f )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo();
#endif
			foreach( Control c in Parent.Controls )
			{
				if( c.CausesValidation )
				{
					f.Push( c );
					c.CausesValidation = false;
				}
			}
		}

		/// <override/>
		protected override void OnMouseDown( MouseEventArgs mevent )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this );
#endif

			this.mouseDownTick = Environment.TickCount + 1500;
			this.step = 1;

			RichTextBox textBox = TextBox;
			if( !DesignMode && this.textArea.Contains( new Point( mevent.X, mevent.Y ) ) )
			{
				// Click in text.
				ShowTextBox( true );
			}
			else
			{
				// Click elsewhere.
				this.bFocused = this.textBox.Focused;
#if DEBUG

				if( Switches.SplitterControlEvents.TraceVerbose )

					TraceUtil.TraceCurrentMethodInfo( Name, mevent.X, mevent.Y, mevent.Button, mevent.Clicks );
#endif

				base.OnMouseDown( mevent );
			}

		}

		internal void HideTextBox()
		{
			textBoxVisible = false;
			if( textBox.Focused )
				textBox.Location = new Point( -1000, 1000 );
			else
				textBox.Visible = false;
			this.Invalidate( this.textArea );
		}

		/// <override/>
		protected override void OnMouseUp( MouseEventArgs mevent )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, mevent.X, mevent.Y, mevent.Button, mevent.Clicks );
#endif

			base.OnMouseUp( mevent );

			if( this.IsHandleCreated && textBox.Focused )
				ShowTextBox( true );
		}

		/// <override/>
		protected override void OnPaint( PaintEventArgs pe )
		{
			lock( this )
			{
				Region clip = pe.Graphics.Clip;

				if( !textBoxVisible &&
					!this.textArea.IsEmpty &&
					this.textArea.IntersectsWith( pe.ClipRectangle ) )
				{
					// Draw static text when text box is hidden.
					pe.Graphics.IntersectClip( this.textArea );
					Brush textBrush = new SolidBrush( this.ForeColor );
					Brush fillBrush = new SolidBrush( this.BackColor );
					try
					{
						pe.Graphics.FillRectangle( fillBrush, this.textArea );
						StringFormat format = new StringFormat();
						Rectangle r = textArea;
						string text = CurrentRecord.ToString();
						r.Y = GetCorrectedYCoord( pe.Graphics, text );

						//r.Offset(-1, 0);
						if( this.RightToLeft == RightToLeft.Yes )
						{
							format.Alignment = StringAlignment.Far;
						}
						if( this.Enabled && CurrentRecord >= MinRecord && ( CurrentRecord <= MaxRecord + ( this.AllowAddNew?1:0 ) ) )
							pe.Graphics.DrawString( CurrentRecord.ToString(), this.Font, textBrush,
								r, format );
                        format.Dispose();
						// little offset adjusts RichtText control text offset
					}
					finally
					{
						textBrush.Dispose();
						fillBrush.Dispose();
					}

					pe.Graphics.Clip = clip;
				}

				pe.Graphics.ExcludeClip( this.textArea );

				// Buttons
#if DEBUG
				if( Switches.RecordNavigationBarEvents.TraceVerbose )
					TraceUtil.TraceCurrentMethodInfo( pe.ClipRectangle );
#endif

				base.OnPaint( pe );

				pe.Graphics.Clip = clip;
			}
		}

		/// <override/>
		protected override void OnEnabledChanged( EventArgs e )
		{
			base.OnEnabledChanged( e );
			if( textBox != null )
			{
				if( !Enabled )
				{
					textBox.Text = "";
					textBox.Visible = false;
				}
			}
			SyncArrowButtons();
		}

		/// <summary>
		/// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
		/// </summary>
		public override void Refresh()
		{
			if( !Updating )
			{
				RichTextBox textBox = TextBox;
				if( textBox.IsHandleCreated && textBox.Visible )
					textBox.Refresh();
				SyncArrowButtons();
			}

			base.Refresh();
		}


		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		static public Rectangle CenterInRect( Rectangle rect, Size size )
		{
			int dx = 0;
			if( size.Width < rect.Width )
				dx = rect.Width-size.Width;

			int dy = 0;
			if( size.Height < rect.Height )
				dy = rect.Height-size.Height;

			return new Rectangle( rect.Left+dx/2, rect.Top+dy/2,
				Math.Min( size.Width, rect.Width ), Math.Min( size.Height, rect.Height ) );
		}

		/// <override/>
		protected override void OnLayout( LayoutEventArgs levent )
		{
			Graphics g = this.CreateGraphics();
			Font font = this.Font;
			//Size textSize = g.MeasureString("01234567", font).ToSize();
			Size textSize = g.MeasureString( this.currentRecord.ToString(), font ).ToSize();
			textSize.Width += 1;
			g.Dispose();

			this.labelButton.HideButton = false;
			this.labelMaxButton.HideButton = false;
#if DEBUG
			if( Switches.RecordNavigationBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( levent.AffectedProperty );
#endif

			base.OnLayout( levent );

			InternalButtonBar bar = this.ButtonBarChild;

			if( !textSize.IsEmpty &&
				!this.labelButton.Size.IsEmpty &&
				bar.placeHolderBounds.Width < textSize.Width/2 )
			{
				this.labelButton.HideButton = true;

				if( bar.placeHolderBounds.Width == 0 )
				{
					bar.placeHolderBounds = this.labelButton.Bounds;
				}
				else
				{
					int x = this.labelButton.Size.Width;
					bar.placeHolderBounds.X -= x;
					bar.placeHolderBounds.Width += x;
					this.labelButton.Bounds = bar.placeHolderBounds;
				}
			}

			Rectangle rect = bar.placeHolderBounds;
			if( !rect.IsEmpty )
			{
				this.textArea = new Rectangle( rect.Location, textSize );// CenterInRect(rect, textSize);
				//this.textArea = CenterInRect(rect, textSize);
				this.textArea.Height = Math.Max( this.textArea.Height,
					bar.Bounds.Height );

				//this.textArea.Inflate(-3, 0);

				if( this.RightToLeft == RightToLeft.Yes )
					this.textArea.X = bar.Bounds.Right - textArea.Right + bar.Bounds.Left;
				if( textBoxVisible )
					TextBox.Bounds = this.textArea;

				this.labelMaxButton.Bounds = new Rectangle( this.RightToLeft == RightToLeft.Yes ? this.textArea.Left : this.textArea.Right, this.textArea.Top, this.labelMaxButton.Bounds.Width, this.labelMaxButton.Bounds.Height );

			}
		}


		/// <summary>
		/// Indicates whether the step for increasing the record position should be increased
		/// when the user holds down the mouse on a record navigation button for a longer period.
		/// </summary>
		[DefaultValue( true )]
		[Description( "Indicates whether the step for increasing the record position should be increased when the user holds down the mouse on a record navigation button for a longer period." )]
		public bool AllowStepIncrease
		{
			get
			{
				return allowStepIncrease;
			}
			set
			{
				allowStepIncrease = value;
			}
		}

        /// <summary>
        /// Sets the current record position to first record. 
        /// </summary>
         public void MoveFirst()
        {
            this.CurrentRecord = this.MinRecord;
        }

        /// <summary>
        /// Sets the current record position to the next record.
        /// </summary>
        public void MoveNext()
        {
            this.CurrentRecord = Math.Min(CurrentRecord + this.step, this.MaxRecord);
        }

        /// <summary>
        /// Sets the current record position to the previous record.
        /// </summary>
        public void MovePrevious()
        {
            this.CurrentRecord = Math.Max(CurrentRecord - this.step, 1);
        }

        /// <summary>
        /// Sets the current record position to the last record.
        /// </summary>
        public void MoveLast()
        {
            this.CurrentRecord = this.MaxRecord;
        }

		/// <summary>
		/// Occurs when the specified button was clicked or the mouse is pressed down on the button.
		/// </summary>
		/// <param name="button">The source of the event.</param>
		public override void /*IInternalButtonParent.*/ OnClickedButton( InternalButton button )
		{
			InternalArrowButton arrow = button as InternalArrowButton;
			if( arrow != null )
			{
				ArrowButtonEventArgs e = new ArrowButtonEventArgs( arrow.Type );
				OnArrowButtonClicked( e );
				if( e.Cancel )
					return;

				if( Environment.TickCount > this.mouseDownTick )
				{
					this.mouseDownTick = Environment.TickCount + 1000;
					//Commented out so that step does not get increased, should make this an option later...
					if( allowStepIncrease )
						this.step = Math.Min( 9, this.step+1 );
				}

				switch( arrow.Type )
				{
					case ArrowType.First:
					CurrentRecord = this.MinRecord;
					break;

					case ArrowType.Last:
					CurrentRecord = this.MaxRecord;
					break;

					case ArrowType.Next:
					CurrentRecord = Math.Min( CurrentRecord + this.step, this.MaxRecord );
					break;

					case ArrowType.Previous:
					CurrentRecord = Math.Max( CurrentRecord - this.step, 1 );
					break;

					case ArrowType.AddNew:
					CurrentRecord = this.MaxRecord+1;
					break;
				}

				this.PerformLayout();

				if( !TextBox.Focused )
				{
					HideTextBox();
				}
			}
			else
			{
				switch( button.SpinButton )
				{
					case SpinButtonType.Up:
					CurrentRecord++;
					break;

					case SpinButtonType.Down:
					CurrentRecord--;
					break;
				}
			}
		}

		/// <override/>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
		protected override bool ProcessKeyPreview( ref Message m )
		{
			if( this.textBox.IsHandleCreated &&
				(int)m.WParam == 0x0D/*VK_RETURN*/)
			{
				if( m.Msg == 0x0100/*WM_KEYDOWN*/)
				{
					try
					{
						CurrentRecord = Int32.Parse( this.textBox.Text );
					}
					catch( Exception ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
						if( !ExceptionManager.RaiseExceptionCatched( null, ex ) )
							throw;
					}
					this.textBox.Modified = false;
				}
				return true;
			}

			return base.ProcessKeyPreview( ref m );
		}

		void SyncArrowButtons()
		{
			ArrowType flags = ArrowType.None;

			if( Enabled )
			{
				if( CurrentRecord != this.MinRecord )
					flags |= ArrowType.First;

				if( CurrentRecord > this.MinRecord )
					flags |= ArrowType.Previous;

				if( this.MaxRecord == -1 || CurrentRecord < this.MaxRecord )
					flags |=  ArrowType.Next;

				if( this.MaxRecord != -1 && CurrentRecord != this.MaxRecord && this.MaxRecord + ( this.AllowAddNew ? 1:0 ) > this.MinRecord )
					flags |=  ArrowType.Last;

				if( AllowAddNew && CurrentRecord != this.MaxRecord+1 )
					flags |= ArrowType.AddNew;
			}

			EnableButtonFlags = flags;
		}

		bool IsValidRecord( int record )
		{
            if (record == 0)
                return true;
			else if( record < this.MinRecord )
				return false;
			else if( this.MaxRecord == -1 )
				return true;
			else if( AllowAddNew && record <= this.MaxRecord + 1 )
				return true;
			else
				return ( record <= this.MaxRecord );
		}

		// Properties

		/// <override/>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		protected override InternalButton[] NoArrowButtons
		{
			get
			{
				return new InternalButton[] {
												this.labelButton,
												null
											};
			}
		}

		/// <override/>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		protected override InternalButton[] SingleArrowButtons
		{
			get
			{
				InternalButton[] buttons;

				if( !this.AllowAddNew )
				{
					buttons = new InternalButton[] {
													new InternalArrowButton(this, ArrowType.Previous, SR.GetString("RecordNavigationBarPreviousRecord")),
													this.labelButton,
													null,
													new InternalArrowButton(this, ArrowType.Next, SR.GetString("RecordNavigationBarNextRecord")),
													new InternalArrowButton(this, ArrowType.AddNew, SR.GetString("RecordNavigationBarAllowAddNew"))
												};
				}
				else
				{
					buttons = new InternalButton[] {
													new InternalArrowButton(this, ArrowType.Previous, SR.GetString("RecordNavigationBarPreviousRecord")),
													this.labelButton,
													null,
													new InternalArrowButton(this, ArrowType.Next, SR.GetString("RecordNavigationBarNextRecord")),
					};
				}

				UpdateComponents( buttons );

				return buttons;
			}
		}

		private void UpdateComponents( InternalButton[] buttons )
		{
			foreach( InternalButton btn in buttons )
			{
				if( btn != null && btn != this.labelButton )
				{
					this.components.Add( btn );
				}
			}
		}

		/// <override/>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		protected override InternalButton[] AllArrowButtons
		{
			get
			{
				InternalButton[] buttons;

				if( !this.AllowAddNew )
				{
					buttons = new InternalButton[] {
													new InternalArrowButton(this, ArrowType.First, SR.GetString("RecordNavigationBarFirstRecord")),
													new InternalArrowButton(this, ArrowType.Previous, SR.GetString("RecordNavigationBarPreviousRecord")),
													this.labelButton,
													null,
													this.labelMaxButton,
													new InternalArrowButton(this, ArrowType.Next, SR.GetString("RecordNavigationBarNextRecord")),
													new InternalArrowButton(this, ArrowType.Last, SR.GetString("RecordNavigationBarLastRecord")),
					};
				}
				else
				{
					buttons = new InternalButton[] {
													new InternalArrowButton(this, ArrowType.First, SR.GetString("RecordNavigationBarFirstRecord")),
													new InternalArrowButton(this, ArrowType.Previous, SR.GetString("RecordNavigationBarPreviousRecord")),
													this.labelButton,
													null,
													this.labelMaxButton,
													new InternalArrowButton(this, ArrowType.Next, SR.GetString("RecordNavigationBarNextRecord")),
													new InternalArrowButton(this, ArrowType.Last, SR.GetString("RecordNavigationBarLastRecord")),
													new InternalArrowButton(this, ArrowType.AddNew, SR.GetString("RecordNavigationBarAllowAddNew")),
					};
				}

				UpdateComponents( buttons );

				return buttons;
			}
		}


		/// <overload>
		/// Sets the current record position.
		/// </overload>
		/// <summary>
		/// Sets the current record position.
		/// </summary>
		/// <param name="value">The new record index.</param>
		/// <param name="force">Indicates whether record should be applied to text box even if <see cref="CurrentRecord"/>
		/// is not changed.</param>
		public void SetCurrentRecord( int value, bool force )
		{
			SetCurrentRecord( value, force, true );
		}

		/// <summary>
		/// Sets the current record position and lets you specify if <see cref="CurrentRecordChanging"/>
		/// and <see cref="CurrentRecordChanged"/> events should be raised.
		/// </summary>
		/// <param name="value">The new record index.</param>
		/// <param name="force">Indicates whether record should be applied to text box even if <see cref="CurrentRecord"/>
		/// is not changed.</param>
		/// <param name="raiseChanged">Specifies if <see cref="CurrentRecordChanging"/>
		/// and <see cref="CurrentRecordChanged"/> events should be raised.</param>
		public void SetCurrentRecord( int value, bool force, bool raiseChanged )
		{
			this.inSetCurrentRecord = true;
			if( ( force || this.currentRecord != value ) && IsValidRecord( value )
				&& ( !raiseChanged || OnCurrentRecordChanging( ref value ) ) )
			{
				if( force || this.currentRecord != value )
				{
					this.currentRecord = value;
					SyncArrowButtons();
					try
					{
						RichTextBox textBox = TextBox;
						textBox.Visible = false;
						textBox.Text = this.Enabled ? CurrentRecord.ToString() : "";
                        if (!Updating && !textBox.Visible)
                        {
                            Invalidate(this.textArea);
                            Update();
                            if(this.SizeToFit)
                            PerformLayout();
						}
					}
					catch( Exception ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
						if( !ExceptionManager.RaiseExceptionCatched( null, ex ) )
							throw;
					}
				}

				this.inSetCurrentRecord = false;
				if( raiseChanged )
					OnCurrentRecordChanged( value );
			}
			this.inSetCurrentRecord = false;
		}

		bool inSetCurrentRecord = false;

		/// <summary>
		/// Gets or sets the current record position.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public int CurrentRecord
		{
			get
			{
				return this.currentRecord;
			}
			set
			{
				SetCurrentRecord( value, false );
			}
		}
        private bool sizetofit = false;
        public bool SizeToFit
        {
            get
            {
                return sizetofit;
            }
            set
            {
                sizetofit = value;
            }
        }



		/// <summary>
		/// Thread safe method that applies all settings at once.
		/// </summary>
		/// <param name="min">The minimum record position.</param>
		/// <param name="max">The maximum record position.</param>
		/// <param name="label">The label to be displayed before the record field text box.</param>
		/// <param name="allowAdd">Indicates whether adding new records is enabled.</param>
		/// <param name="current">The current record position.</param>
		public void SetValues( int min, int max, string label, bool allowAdd, int current )
		{
			// Make call thread safe
			if( this.InvokeRequired )
			{
				Invoke( new MethodInvoker( Update ), new object[] { min, max, label, allowAdd, current } );
				return;
			}

			MaxRecord = max;
			MinRecord = min;
			this.AllowAddNew = allowAdd;
			MaxLabel = label;
			CurrentRecord = current;
		}

		/// <summary>
		/// Gets or sets the minimum record position.
		/// </summary>
		[
			//Category("RecordNavigation"),
		DefaultValue( 1 ),
		Description( "Gets or sets the minimum record position." )
		]
		public int MinRecord
		{
			get
			{
				return rnbData != null ? rnbData.MinRecord : this.minRecord;
			}
			set
			{
				if( this.minRecord != value )
				{
					this.minRecord = value;
					SyncArrowButtons();
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum record position.
		/// </summary>
		[
			//Category("RecordNavigation"),
		DefaultValue( -1 ),
		Description( "Gets or sets the maximum record position." )
		]
		public int MaxRecord
		{
			get
			{
				return rnbData != null ? rnbData.MaxRecord : this.maxRecord;
			}
			set
			{
				if( this.maxRecord != value )
				{
					this.maxRecord = value;
					this.Enabled = this.MaxRecord >= this.MinRecord || this.AllowAddNew;
					SyncArrowButtons();
				}
			}
		}

		/// <summary>
		/// Indicates whether adding new records is enabled.
		/// </summary>
		[
			//Category("RecordNavigation"),
		DefaultValue( true ),
		Description( "Indicates whether adding new records is enabled." )
		]
		public bool AllowAddNew
		{
			get
			{
				return rnbData != null ? rnbData.AllowAddNew : this.addNewRecord;
			}
			set
			{
				if( this.addNewRecord != value )
				{
					this.addNewRecord = value;
					this.Enabled = this.MaxRecord >= this.MinRecord || this.AllowAddNew;
					SyncArrowButtons();
				}
			}
		}

		/// <summary>
		/// Gets or sets <see cref="IRecordNavigationBarData"/>.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public IRecordNavigationBarData RnbData
		{
			get
			{
				return rnbData;
			}
			set
			{
				rnbData = value;
			}
		}

		/// <summary>
		/// Gets or sets the label to be displayed before the record field textbox.
		/// </summary>
		[
			//Category("RecordNavigation"),
		DefaultValue( "Record" ),
		Description( "Gets or sets the label to be displayed before the record field textbox." )
		]
		public string Label
		{
			get
			{
				return this.labelButton.Cookie.ToString();
			}
			set
			{
				if( (string)this.labelButton.Cookie != value )
				{
					this.labelButton.Cookie = value;
					PerformLayout();
				}
			}
		}

		/// <summary>
		/// Gets or sets an optional maximum label (e.g. "of 1000").
		/// </summary>
		[
			//Category("RecordNavigation"),
		DefaultValue( "" ),
		Description( "Gets or sets an optional maximum label (e.g. of 1000)." )
		]
		public string MaxLabel
		{
			get
			{
				return this.labelMaxButton.Cookie.ToString();
			}
			set
			{
				if( (string)this.labelMaxButton.Cookie != value )
				{
					this.labelMaxButton.Cookie = value;
					PerformLayout();
				}
			}
		}

		/// <summary>
		/// Gets or sets the textbox where users can enter record indexes manually.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public RichTextBox TextBox
		{
			get
			{
				if( this.textBox == null )
					this.textBox = OnCreateTextBox();

				return this.textBox;
			}
		}

		private RichTextBox OnCreateTextBox()
		{
			RichTextBox textBox = new RecordNavigationBarTextBox();
			textBox.Visible = false;
			textBox.TabStop = false;
			textBox.CausesValidation = false;
			textBox.KeyPress += new KeyPressEventHandler( TextBoxKeyPress );
			textBox.Leave += new EventHandler( TextBoxLeave );
			//			textBox.Enter += new EventHandler(TextBoxEnter);
			textBox.LostFocus += new EventHandler( TextBoxLostFocus );

			return textBox;
		}

		void TextBoxKeyPress( object sender, KeyPressEventArgs e )
		{
			if( !Char.IsDigit( e.KeyChar ) )
				e.Handled = true;
		}


		// CurrentRecordChangingEvent



		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RaiseCurrentRecordChangingEvent( CurrentRecordEventArgs e )
		{
			if( CurrentRecordChanging != null )
				CurrentRecordChanging( this, e );
		}

		/// <summary>
		/// Raises the <see cref="CurrentRecordChanging"/> event.
		/// </summary>
		/// <param name="record">The new record index.</param>
		protected virtual bool OnCurrentRecordChanging( ref int record )
		{
			CurrentRecordEventArgs eventArgs = new CurrentRecordEventArgs( record );
#if DEBUG
			if( Switches.RecordNavigationBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, eventArgs );
#endif

			RaiseCurrentRecordChangingEvent( eventArgs );
			record = eventArgs.Record;

			return eventArgs.Cancel == false;
		}

		// CurrentRecordChangedEvent

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RaiseCurrentRecordChangedEvent( CurrentRecordEventArgs e )
		{
			if( CurrentRecordChanged != null )
				CurrentRecordChanged( this, e );
		}

		/// <summary>
		/// Raises the <see cref="CurrentRecordChanged"/> event.
		/// </summary>
		/// <param name="record">The new record index.</param>
		protected virtual void OnCurrentRecordChanged( int record )
		{
			CurrentRecordEventArgs eventArgs = new CurrentRecordEventArgs( record );
#if DEBUG
			if( Switches.RecordNavigationBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, eventArgs );
#endif

			RaiseCurrentRecordChangedEvent( eventArgs );
		}
		/// <summary>
		/// Calculates corrected Y location of numbers.
		/// </summary>
		/// <param name="g">Grapchics object.</param>
		/// <param name="text">Output text.</param>
		/// <returns>Corrected Y coordinate.</returns>
		private int GetCorrectedYCoord( Graphics g, string text )
		{
			if( g == null )
				throw new ArgumentNullException( "g" );

			if( text == null )
				throw new ArgumentNullException( "text" );

			Size textSize = g.MeasureString( text, this.Font ).ToSize();
			Rectangle rect = this.textArea;
			rect = CenterInRect( rect, textSize );

			correctedYCoord = rect.Y;
			return correctedYCoord;
		}
	}


	/// <summary>
	/// Provides data about a <see cref="RecordNavigationBar.CurrentRecordChanging"/> and <see cref="RecordNavigationBar.CurrentRecordChanged"/> events of a <see cref="RecordNavigationBar"/>.
	/// </summary>
	public class CurrentRecordEventArgs: SyncfusionCancelEventArgs
	{
		/// <summary>
		/// Initializes a new <see cref="CurrentRecordEventArgs"/>.
		/// </summary>
		/// <param name="record">The record index.</param>
		public CurrentRecordEventArgs( int record )
		{
			this.record = record;
		}

		/// <summary>
		/// Gets / sets the record index.
		/// </summary>
		[TraceProperty( true )]
		public int Record
		{
			get
			{
				return record;
			}
			set
			{
				record = value;
			}
		}

		private int record;
	}


	/// <summary>
	/// Handles a <see cref="RecordNavigationBar.CurrentRecordChanged"/> event of a <see cref="RecordNavigationBar"/>.
	/// </summary>
	public delegate void CurrentRecordChangedEventHandler( object sender, CurrentRecordEventArgs e );

	/// <summary>
	/// Handles a <see cref="RecordNavigationBar.CurrentRecordChanging"/> event of a <see cref="RecordNavigationBar"/>.
	/// </summary>
	public delegate void CurrentRecordChangingEventHandler( object sender, CurrentRecordEventArgs e );
}
