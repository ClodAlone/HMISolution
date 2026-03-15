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

#region File Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Win32;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

using Syncfusion.Runtime.Serialization;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.XPMenus;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// The CommandBarController acts as a central point of control for the <see cref="CommandBar"/>s hosted on a form.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The Essential Tools CommandBars framework implements a hosting environment that can be used for
	/// creating toolbars, statusbars and rebars similar to those that are present
	/// in the Microsoft Visual Studio.NET IDE and the Microsoft Office XP product suite.
	/// A CommandBar, similar to Win32/MFC control bars, is purely a container control that
	/// is responsible only for it's layout state and it is the client window, such as a ToolBar,
	/// StatusBar or any other Windows Forms control, that provides the CommandBar with it's
	/// functional identity.
	/// </p>
	/// <p>CommandBars have two basic states - a docked state and a floating state.
	/// In the docked state, the CommandBars are usually aligned along one of the borders
	/// (left, right, top or bottom) of the host form. Depending upon the preferences set,
	/// the bar can either take up an entire row as in the case of a menu or share it's
	/// space with other bars within a particular row as with toolbars and rebars. By default,
	/// a docked CommandBar has a gripper and a drop-down button with a customizable click event.
	/// The gripper can be used to reposition the bar within a row, move it between rows and to drag
	/// it out of the frame and float as a separate window. In the docked mode, when the row
	/// width is adequate, the CommandBars are usually sized to their maximum length. However,
	/// reducing the frame width/height will, based on the user-setting, force the CommandBar to
	/// reduce in size while retaining the same height or wrap thereby increasing the bar height.
	/// The wrapping mode is particularly useful for toolbars and is fully customizable.
	/// In the floating state, the CommandBar is parented by a floating window
	/// that can dragged around the desktop. A floating CommandBar can be redocked to it's host
	/// by either double-clicking it or by dragging it over one of the edges of the host form.
	/// </p>
	/// <p>
	/// The CommandBarController class, as the name implies, serves as a form-scope controller for all the CommandBars.
	/// Attributes that are common across all CommandBars within a host form, such as the
	/// <see cref="CommandBarController.EnabledDockBorders"/> and <see cref="CommandBarController.PersistState"/> are
	/// usually set on the CommandBarController instance. The CommandBarController implements
	/// the API and the requisite design-time support for creating and working with CommandBars.
	/// </p>
	/// </remarks>
	/// <seealso cref="CommandBar"></seealso>
	/// <example>
	/// The sample code shows how to create and initialize a CommandBarController, create a CommandBar
	/// control that is initially docked to the top border of the form and hosts a Panel control and finally add
	/// the CommandBar to the CommandBarController's collection.
	///
	/// <coderef file="Tools\Samples\CommandBars Package\CommandBars\CS\DockedBarsForm.cs" name="CommandBars" lang="C#"><code lang="C#">
	///		private void InitializeCommandBars()
	///		{
	///			// Create the CommandBarController
	///			this.commandBarController1 = new Syncfusion.Windows.Forms.Tools.CommandBarController();
	///			((System.ComponentModel.ISupportInitialize)(this.commandBarController1)).BeginInit();
	///
	///			// Set the CommandBarController's host form
	///			this.commandBarController1.HostForm = this;
	///			this.commandBarController1.PersistState = true;
	///
	///			// Create the CommandBar control
	///			this.commandBarAddress = new Syncfusion.Windows.Forms.Tools.CommandBar();
	///
	///			// Set the CommandBar Layout/Behavior/Appearance attributes
	///			this.commandBarAddress.DockBorder = Syncfusion.Windows.Forms.Tools.DockBorder.Top;
	///			this.commandBarAddress.HideDropDownButton = true;
	///			this.commandBarAddress.MaxLength = 400;
	///			this.commandBarAddress.MinHeight = 26;
	///			this.commandBarAddress.MinLength = 50;
	///			this.commandBarAddress.Name = "commandBarAddress";
	///			this.commandBarAddress.RowIndex = 1;
	///			this.commandBarAddress.RowOffset = 1;
	///			this.commandBarAddress.Text = "Address";
	///
	///			// Create the ComboBox control and add it to the CommandBars Controls collection
	///			this.comboBox1 = new System.Windows.Forms.ComboBox();
	///			this.commandBarAddress.Controls.AddRange(new System.Windows.Forms.Control[] {this.comboBox1});
	///
	///			// Add the CommandBar to the CommandBarController.CommandBars collection
	///			this.commandBarController1.CommandBars.Add(this.commandBarAddress);
	///
	///			((System.ComponentModel.ISupportInitialize)(this.commandBarController1)).EndInit();
	///		}</code></coderef>
	///
	/// <coderef file="Tools\Samples\CommandBars Package\CommandBars\VB\DockedBarsForm.vb" name="CommandBars" lang="VB"><code lang="VB">
	///        Private Sub InitializeCommandBars()
	///
	///            ' Create the CommandBarController
	///            Me.commandBarController1 = New Syncfusion.Windows.Forms.Tools.CommandBarController(Me.components)
	///            CType(Me.commandBarController1, System.ComponentModel.ISupportInitialize).BeginInit()
	///
	///            ' Set the CommandBarController's host form
	///            Me.commandBarController1.HostForm = Me
	///            Me.commandBarController1.PersistState = True
	///
	///            ' Create the CommandBar control
	///            Me.commandBarAddress = New Syncfusion.Windows.Forms.Tools.CommandBar()
	///
	///            ' Set the CommandBar Layout/Behavior/Appearance attributes
	///            Me.commandBarAddress.DockState = Syncfusion.Windows.Forms.Tools.CommandBarDockState.Top
	///            Me.commandBarAddress.MaxLength = 400
	///            Me.commandBarAddress.MinHeight = 26
	///            Me.commandBarAddress.MinLength = 50
	///            Me.commandBarAddress.Name = "commandBarAddress"
	///            Me.commandBarAddress.RowIndex = 1
	///            Me.commandBarAddress.RowOffset = 1
	///            Me.commandBarAddress.Text = "Address"
	///
	///            ' Create the ComboBox control and add it to the CommandBars Controls collection
	///            Me.comboBox1 = New System.Windows.Forms.ComboBox()
	///            Me.commandBarAddress.Controls.AddRange(New System.Windows.Forms.Control() {Me.comboBox1})
	///
	///            ' Add the CommandBar to the CommandBarControllers CommandBars       // collection
	///            Me.commandBarController1.CommandBars.Add(Me.commandBarAddress)
	///
	///            CType(Me.commandBarController1, System.ComponentModel.ISupportInitialize).EndInit()
	///
	///        End Sub</code></coderef>
	///
	/// </example>
	[
	ToolboxBitmap(typeof(CommandBarController), "ToolboxIcons.commandbarcontroller.bmp"),
	Designer( typeof(Syncfusion.Windows.Forms.Tools.Design.CBControllerDesigner),
		typeof(System.ComponentModel.Design.IDesigner) ),
	ToolboxItemFilter("System.Windows.Forms"),
	Description("Component which hosts command Bars in a form.")
	]
	public class CommandBarController : Component, ISupportInitialize, ICBControllerDesignerInvoke,IVisualStyle 
	{
		static CommandBarController()
		{
#if SINGLE_DLL_BUILD
			AppStateSerializer.SetBindingInfo("Syncfusion.Tools.Windows", typeof(CommandBarController).Assembly);
			AppStateSerializer.SetBindingInfo("Syncfusion.Tools", typeof(CommandBarController).Assembly);
#else
			AppStateSerializer.SetBindingInfo("Syncfusion.Tools.Frameworks", typeof(CommandBarController).Assembly);
			// TO support backward compatibility
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(CBarSerializationWrapper).FullName, typeof(CBarSerializationWrapper).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(CBCtrlrSerializationWrapper).FullName, typeof(CBCtrlrSerializationWrapper).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(CommandBarDockBorder).FullName, typeof(CommandBarDockBorder).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(CommandBarDockState).FullName, typeof(CommandBarDockState).Assembly);
#endif
		}

		protected CommandBar focusedCBar = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandBar FocusedCommandBar
		{
			get { return this.focusedCBar; }
			set	{ this.focusedCBar = value; }
		}

		#region Nested classes
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal class AppMainFormWnd : NativeWindowSubclass
		{
			CommandBarController cbController = null;

			internal AppMainFormWnd(CommandBarController cbc)
			{
				this.cbController = cbc;
			}

			// To avoid design time issue with BarManager:
			// Avoiding Releasing the Handle (due to framework bug).
			// Instead deactivating this subclass.
			// Releasing causes subsequent addition of controls (like PopupControlContainer)
			// to the designer to fail.
			public void Deactivate()
			{
				this.cbController = null;
			}

            void ToggleCommandBarsFloatState( bool bActivate )
            {
                if( null != this.cbController )
                {
                    this.cbController.ToggleCommandBarFloatState(bActivate);
                }
            }

			protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);

                int nwparam = m.WParam.ToInt32();

				switch(m.Msg)
				{
                    case NativeMethods.WM_SIZE:
					{
						ToggleCommandBarsFloatState( 1 != nwparam );    // SIZE_MINIMIZED != nwparam
						break;
					}

					case NativeMethods.WM_ACTIVATEAPP:
					{
						ToggleCommandBarsFloatState( 1 == nwparam );
						break;
					}
					
					case NativeMethods.WM_MOUSEACTIVATE:
					{
						if( (this.cbController != null) && (this.cbController.FocusedCommandBar != null) )
						{
							Point pt = Cursor.Position;
							IntPtr hwndunderpt = NativeMethods.WindowFromPoint( pt.X, pt.Y );

							if( hwndunderpt != IntPtr.Zero )
							{
								if( !NativeMethods.IsChild( this.cbController.FocusedCommandBar.Handle, hwndunderpt ) )
								{
									this.ResetFocus();
								}
							}
						}
						break;
					}

					case NativeMethods.WM_LBUTTONDOWN:
					{
						if( (this.cbController != null) && (this.cbController.FocusedCommandBar != null) )
						{
							this.ResetFocus();
						}
						break;
					}
					
					case NativeMethods.WM_NCLBUTTONDOWN:
					{
						if( (this.cbController != null) && (this.cbController.FocusedCommandBar != null) )
						{
							this.ResetFocus();
						}
						break;
					}
				}
			}

			protected void ResetFocus()
			{
				// Reset focus to the HostForm's child that has the lowest TabIndex value.
				Control firstchild = null;
				foreach(Control ctrl in this.cbController.HostForm.Controls)
				{
					if( (ctrl == this.cbController.CommandDockBarL) || (ctrl == this.cbController.CommandDockBarT)
						|| (ctrl == this.cbController.CommandDockBarR) || (ctrl == this.cbController.CommandDockBarB) )
					{
						continue;
					}
					if(firstchild == null)
						firstchild = ctrl;
					if(ctrl.TabIndex < firstchild.TabIndex)
						firstchild = ctrl;
				}
				if(firstchild != null)
					firstchild.Focus();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal class DockBarStateEventArgs : EventArgs
		{
			CommandDockBar dockBar;
			bool created;
			public DockBarStateEventArgs(CommandDockBar dockBar, bool created)
			{
				this.dockBar = dockBar;
				this.created = created;
			}
			public CommandDockBar CommandDockBar
			{
				get{return this.dockBar;}
			}
			public bool Created
			{
				get{return this.created;}
			}
		}
		/// <summary>
		/// Represents a collection of <see cref="CommandBar"/> objects.
		/// </summary>
		/// <seealso cref="CommandBarController.CommandBars"></seealso>
		[
		Category("Syncfusion CommandBars"),
		Description("The collection of CommandBars in the CommandBarController."),
		DefaultProperty("Item"),
		Editor(typeof(Syncfusion.Windows.Forms.Tools.Design.CommandBarsCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))
		]
		public sealed class CommandBarsCollection : CollectionBase
		{
			private CommandBarController cbcOwner;
			internal event System.EventHandler NewBarAdded;


			/// <summary>
			/// Returns the <see cref="CommandBar"/> at the specified index.
			/// </summary>
			/// <value>The zero-based index of the CommandBar to get.</value>
			public CommandBar this[int nindex]
			{
				get	{ return (CommandBar)(this.List[nindex]); }
			}

			/// <summary>
			/// Creates a new instance of the CommandBarsCollection class.
			/// </summary>
			/// <param name="cbctrlr"> The <see cref="CommandBarController"/> that this collection belongs to.</param>
			public CommandBarsCollection(CommandBarController cbctrlr)
			{
				this.cbcOwner = cbctrlr;
			}

			/// <summary>
			/// Adds a <see cref="CommandBar"/> to the collection.
			/// </summary>
			/// <param name="cbar"> The CommandBar to be added.</param>
			/// <returns> The total number of CommandBars present in the collection. </returns>
			public int Add(CommandBar cbar)
			{
				if(cbar.Controller == null)
					cbar.Controller = this.cbcOwner;
				if(this.cbcOwner.FreezeLayout == true)
				{
					if(this.cbcOwner.alFreezeRemoveBars.Contains(cbar))
						this.cbcOwner.alFreezeRemoveBars.Remove(cbar);
					return this.cbcOwner.alFreezeAddBars.Add(cbar);
				}
				else
				{
					this.OnNewBarAdded(EventArgs.Empty);
					return this.List.Add(cbar);
				}
			}

			/// <summary>
			/// Removes the <see cref="CommandBar"/> from the collection.
			/// </summary>
			/// <param name="cbar">The CommandBar to be removed.</param>
			public void Remove(CommandBar cbar)
			{
				if(this.cbcOwner.FreezeLayout == true)
				{
					if(this.cbcOwner.alFreezeAddBars.Contains(cbar))
					{
						this.cbcOwner.alFreezeAddBars.Remove(cbar);
						cbar.Dispose();
					}
					else this.cbcOwner.alFreezeRemoveBars.Add(cbar);
				}
				else
				{
                    if (this.List.Contains(cbar) == true)
                         this.List.Remove(cbar);
 				}
			}

			/// <summary>
			/// Indicates whether the <see cref="CommandBar"/> is present.
			/// </summary>
			/// <param name="cbar"> The CommandBar to locate in the collection.</param>
			/// <returns>TRUE if the CommandBar is present; FALSE otherwise.</returns>
			public bool Contains(CommandBar cbar)
			{
				return this.List.Contains(cbar);
			}

			private void OnNewBarAdded(EventArgs e)
			{
				if(this.NewBarAdded != null)
					this.NewBarAdded(this, e);
			}

			[Syncfusion.Documentation.DocumentationExclude()]
			protected override void OnInsertComplete(int index,	object value)
			{
				base.OnInsertComplete(index, value);

				CommandBar cbar = value as CommandBar;
				if(cbar.Controller == null)
					cbar.Controller = this.cbcOwner;
				if(this.cbcOwner.bInitializationComplete == true)
				{
					if(cbar.Parent == null)	// If the CommandBar is yet to initialized
					{
						this.cbcOwner.SetCommandBarInitialPosition(cbar);
						this.cbcOwner.RecalcLayout(CommandBarDockBorder.Left|CommandBarDockBorder.Top|CommandBarDockBorder.Right|CommandBarDockBorder.Bottom);
						this.NewBarAdded += new System.EventHandler(cbar.OnNewBarAddedEH);
					}
				}
			}

			[Syncfusion.Documentation.DocumentationExclude()]
			protected override void OnRemoveComplete(int index, object value)
			{
				base.OnRemoveComplete(index, value);

				CommandBar cbar = value as CommandBar;
				if(cbar.Parent != null)	// Visible docked or floating CommandBars
				{
					if(cbar.Floating == true)
					{
						CommandBarForm cbarform = cbar.Parent as CommandBarForm;
						cbarform.Visible = false;
                        cbarform.Controls.Remove(cbar);
						cbarform.Close();
					}
					else
					{
						if(cbar.CommandBarBaseVisible == true)
						{
							cbar.CommandBarBaseVisible = false;
                            this.NewBarAdded -= new System.EventHandler(cbar.OnNewBarAddedEH);
							cbar.cdbParent.RemoveCommandBar(cbar);
						}
						else
						{
							Debug.Assert(cbar.cdbParent != null);
                            this.NewBarAdded -= new System.EventHandler(cbar.OnNewBarAddedEH);
							cbar.cdbParent.Controls.Remove(cbar);
						}
					}
				}
				else
				{
                    this.NewBarAdded -= new System.EventHandler(cbar.OnNewBarAddedEH);
					cbar.Dispose();
				}
			}

			[Syncfusion.Documentation.DocumentationExclude()]
			protected override void OnClear()
			{
				foreach(CommandBar cbar in this.List)
					cbar.Controller = null;
				base.OnClear();
			}
		}
		#endregion

		#region Fields
		[Syncfusion.Documentation.DocumentationExclude()]
		protected AppMainFormWnd wndMainFrm = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Form frmMain = null;
		protected CommandBarsCollection alCommandBars = null;
		protected CommandBarDockBorder cbDockBorder = CommandBarDockBorder.Left|CommandBarDockBorder.Top|CommandBarDockBorder.Right|CommandBarDockBorder.Bottom;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandDockBar CommandDockBarL = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandDockBar CommandDockBarT = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandDockBar CommandDockBarR = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandDockBar CommandDockBarB = null;
		
        protected internal ArrayList alDelayLoadBars = new ArrayList();
		protected ArrayList alFreezeAddBars = new ArrayList();
		protected ArrayList alFreezeRemoveBars = new ArrayList();
		protected ArrayList alFreezeRelayoutBars = new ArrayList();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nFreezeLayoutInternal = 0;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nFreezeLayout = 0;

		// Serialization Implementation
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CBCtrlrSerializationWrapper wpprCBController = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Type typeCBSerializer = typeof(CBarSerializationWrapper);

		[Syncfusion.Documentation.DocumentationExclude()]
		protected const String strPersistKey = "CommandBarInfo";
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bPersistState = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected MemoryStream strmDefault = new MemoryStream();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bDisableButtons = false;
		/// <summary>
		/// Colors for Office2007 visual style.
		/// </summary>
		private Office2007Colors m_office2007ColorTable = null;
        /// <summary>
        /// Colors for Office2010 visual style.
        /// </summary>
        private Office2010Colors m_office2010ColorTable = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal delegate void DockBarStateChangedHandler(object sender, DockBarStateEventArgs args);
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal event DockBarStateChangedHandler DockBarStateChanged;

		/// <summary>
		/// Lets you specify a unique ID used to distinguish the persistence information
		/// of different instances of your Form type.
		/// </summary>
		/// <remarks>
		/// The default persistence logic assumes that there will be only a single CommandBarController
		/// in an application. But that might not be the case if you have more than 1 MDI parent.
		/// In such cases, the persisted state of one MDI parent will get overridden by the other
		/// since the default logic doesn't distinguish these 2 different instances.
		/// </remarks>
		[Description( "Lets you specify a unique ID used to distinguish the persistence information of different instances of your Form type." )]
		public event ProvidePersistenceIDEventHandler ProvidePersisteceID;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bLoadVisibility = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bInitializationComplete = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bThemesEnabled = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected VisualStyle tbStyle = VisualStyle.OfficeXP;

		protected Color clrBackColor = Control.DefaultBackColor;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bBackColorSet = false;

		/// <summary>
		/// Indicates whether brought the CommandBar to the front of the z-order.
		/// </summary>
		private bool m_bInternalDocking = false;

		/// <summary>
		/// Colorschemes for Office2007 visual style.
		/// </summary>
		protected Office2007Theme m_office2007Theme = Office2007Theme.Blue;
        /// <summary>
        /// Colorschemes for Office2010 visual style.
        /// </summary>
        protected Office2010Theme m_office2010Theme = Office2010Theme.Blue;
        static bool s_isDevEnv = (Application.ExecutablePath.IndexOf("devenv.exe") < 0);
		#endregion

		#region Events
		/// <summary>
		/// Occurs when the <see cref="CommandBarController.Style"/> property is changed.
		/// </summary>
		[Description( "Occurs when Style property is changed." )]
		public event EventHandler StyleChanged;
		/// <summary>
		/// Occurs when the <see cref="CommandBarController.ThemesEnabled"/> property is changed.
		/// </summary>
		[Description( "Occurs when ThemesEnabled property is changed." )]
		public event EventHandler ThemesEnabledChanged;
		/// <summary>
		/// Occurs when CommandBars' layout is internally suspended.
		/// </summary>
		[Description( "Occurs when CommandBars' layout is internally suspended." )]
		public event EventHandler LayoutSuspended;
		/// <summary>
		/// Occurs when CommandBars' layout is internally resumed.
		/// </summary>
		[Description( "Occurs when CommandBars' layout is internally resumed." )]
		public event EventHandler LayoutResumed;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether brought the CommandBar to the front of the z-order.
		/// </summary>
		[
		DefaultValue( false ),
		Description( "Indicates whether bring the CommandBar to the front of the z-order." ),
		Category( "Behavior" )
		]
		public bool InternalDocking
		{
			get
			{
				return m_bInternalDocking;
			}
			set 
			{
				if (value != m_bInternalDocking)
				{
					m_bInternalDocking = value;
					ResetDockBarZOrder();
				}
			}
		}


		/// <summary>
		/// Gets or sets the host form.
		/// </summary>
		/// <value>The Form that will host the <see cref="CommandBar"/>s.</value>
		[
		Description("The form window hosting the CommandBars."),
		Category("Syncfusion CommandBars")
		]
		public Form HostForm
		{
			get { return this.frmMain; }
			set
			{
				if(this.frmMain == null)
				{
                    this.frmMain = value;
					if (this.DesignMode == false)
					{
						this.frmMain.Load += new EventHandler(this.HostForm_Load);
						this.frmMain.Closing += new CancelEventHandler(this.HostForm_Closing);
						this.frmMain.SystemColorsChanged += new EventHandler(this.HostForm_SystemColorsChanged);
					}
					if(this.bBackColorSet == false)
						this.clrBackColor = this.frmMain.BackColor;
					this.frmMain.BackColorChanged += new EventHandler(this.HostForm_BackColorChanged);
				}
			}
		}

		/// <summary>
		/// Gets or sets the edges of the form along which <see cref="CommandBar"/>s are allowed to dock.
		/// </summary>
		/// <value>A <see cref="CommandBarDockBorder"/> value specifying the dockable edges.</value>
		[
		DefaultValue(CommandBarDockBorder.Left|CommandBarDockBorder.Top|CommandBarDockBorder.Right|CommandBarDockBorder.Bottom),
		Category("Syncfusion CommandBars"),
		Description("The host form borders capable of accomdating the CommandBars."),
		Localizable(true)
		]
		public CommandBarDockBorder EnabledDockBorders
		{
			get { return this.cbDockBorder; }
			set
			{
				if(this.cbDockBorder != value)
				{
					this.cbDockBorder = value;
					if(this.frmMain != null)
						this.InitializeDockBars();
				}
			}
		}

		/// <summary>
		/// Indicates whether the application's CommandBars state should be persisted.
		/// </summary>
		/// <value>When TRUE the application's CommandBars state will be persisted. The default is FALSE.</value>
		[
		Description("Saves the CommandBar state between application instances."),
		DefaultValue(false),
		Category("Syncfusion CommandBars")
		]
		public bool PersistState
		{
			get { return this.bPersistState; }
			set { this.bPersistState = value; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool FreezeLayoutInternal
		{
			get { return (this.nFreezeLayoutInternal > 0); }
			set
			{
				if(value == true)
					this.nFreezeLayoutInternal++;
				else if(this.nFreezeLayoutInternal > 0)
					this.nFreezeLayoutInternal--;
			}
		}

        /// <summary>
        /// Gets or sets FreezeLayout
        /// </summary>
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool FreezeLayout
		{
			get { return (this.nFreezeLayout > 0); }
			set
			{
				if(value == true)
					this.nFreezeLayout++;
				else if(this.nFreezeLayout > 0)
				{
					this.nFreezeLayout--;
					if(this.nFreezeLayout == 0)
					{
						// If the freezelists contain any CommandBars, then Add/Remove those.
						foreach(CommandBar cbar in this.alFreezeRemoveBars)
						{
							if(this.alFreezeAddBars.Contains(cbar))
								this.alFreezeAddBars.Remove(cbar);
							if(this.alFreezeRelayoutBars.Contains(cbar))
								this.alFreezeRelayoutBars.Remove(cbar);
						}
						if(this.alFreezeRemoveBars.Count > 0)
						{
							CommandBar[] cbarray = (CommandBar[])this.alFreezeRemoveBars.ToArray(typeof(CommandBar));
							this.alFreezeRemoveBars.Clear();
							foreach(CommandBar cbar in cbarray)
								this.CommandBars.Remove(cbar);
						}
						if(this.alFreezeAddBars.Count > 0)
						{
							CommandBar[] cbarray = (CommandBar[])this.alFreezeAddBars.ToArray(typeof(CommandBar));
							this.alFreezeAddBars.Clear();
							foreach(CommandBar cbar in cbarray)
								this.CommandBars.Add(cbar);
						}
						if(this.alFreezeRelayoutBars.Count > 0)
						{
							this.ApplyDeserializedState(this.alFreezeRelayoutBars);
							this.alFreezeRelayoutBars.Clear();
						}
						this.RecalcLayout(CommandBarDockBorder.Left|CommandBarDockBorder.Top|CommandBarDockBorder.Right|CommandBarDockBorder.Bottom);
					}
				}
			}
		}

		/// <summary>
		/// Returns a reference to the <see cref="CommandBar"/>s that belong to this CommandBarController.
		/// </summary>
		/// <value>A <see cref="CommandBarController.CommandBarsCollection"/> that contains the CommandBars in this
		/// CommandBarController.</value>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(true),
		Description("The CommandBar collection."),
		Category("Syncfusion CommandBars")
		]
		public CommandBarsCollection CommandBars
		{
			get { return this.alCommandBars; }
		}

        /// <summary>
        /// Gets or sets the CommandBarSerializer
        /// </summary>
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		[Syncfusion.Documentation.DocumentationExclude()]
		public Type CommandBarSerializer
		{
			get { return this.typeCBSerializer; }
			set
			{
				if(!(value.IsSerializable && typeof(ICommandBarSerializer).IsAssignableFrom(value)))
				{
					Debug.Assert(false, "Error: Serializers should implement both the ISerializable and ICommandBarSerializer interfaces.");
					return;
				}
				this.typeCBSerializer = value;
			}
		}

		/// <summary>
		/// Indicates whether XP Themes (visual styles) should be used for CommandBars.
		/// </summary>
		/// <value>True to turn on themes; false otherwise.</value>
		[
		DefaultValue(false),
		Category(@"Appearance"),
		Description("Specifies whether XP Themes (visual styles) should be used for CommandBars.")
		]
		public bool ThemesEnabled
		{
			get{return this.bThemesEnabled;}
			set
			{
				if(this.bThemesEnabled != value)
				{
					this.bThemesEnabled = value;
					this.SetBarsTransparency();
					OnThemesEnabledChanged();
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="CommandBarController.ThemesEnabledChanged"/> event.
		/// </summary>
		protected virtual void OnThemesEnabledChanged()
		{
			if( this.ThemesEnabledChanged != null )
			{
				this.ThemesEnabledChanged( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Gets a value indicating whether the control's elements are aligned RightToLeft.
		/// </summary>
		[Browsable(false)]
		public virtual RightToLeft RightToLeft
		{
			get
			{
				return (null != this.frmMain) ? this.frmMain.RightToLeft : RightToLeft.No;
			}
		}
		/// <summary>
		///Metrocolor
		/// </summary>
        private Color metroColor = Color.FromArgb(22, 165, 220);
		/// <summary>
		///Gets or Sets the MetroColor
		/// </summary>
        public Color MetroColor
        {
            get
            {
                return metroColor;
            }
            set
            {
                if (metroColor != value)
                    metroColor = value;
                
            }
        }
		/// <summary>
		/// Gets or sets the visual style of the CommandBars.
		/// </summary>
		/// <value>A <see cref="VisualStyle"/> value. Default is VisualStyle.OfficeXP.</value>
		/// <remarks>Note that this setting will be ignored when <see cref="ThemesEnabled"/> is turned on and themes are available in the OS.</remarks>
		[DefaultValue(VisualStyle.OfficeXP),
		Category("Appearance"),
		Description("Specifies the visual style of the CommandBars.")
		]
		public VisualStyle Style
		{
			get { return this.tbStyle; }

			set
			{
				if(this.tbStyle != value)
				{
					this.tbStyle = value;
					this.SetBarsTransparency();
					OnStyleChanged();
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="CommandBarController.StyleChanged"/> event.
		/// </summary>
		protected virtual void OnStyleChanged()
		{
			if( this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2007Outlook )
			{
				m_office2007ColorTable = Office2007Colors.GetColorTable( this.Office2007Theme );
				XPMenus.Office2007BarItemPainter.ColorTable = m_office2007ColorTable;
			}
            else if (this.Style == VisualStyle.Office2010 )
            {
                m_office2010ColorTable = Office2010Colors.GetColorTable(this.Office2010Theme);
                XPMenus.Office2010BarItemPainter.ColorTable = m_office2010ColorTable;
            }
			if( this.StyleChanged != null )
			{
				this.StyleChanged( this, EventArgs.Empty );
			}
		}

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;

                if (value == "Office2007Blue")
                {
                    Style = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    Style = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    Style = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Black;
                }
                if (value == "Office2010Blue")
                {
                    Style = VisualStyle.Office2010;
                    Office2010Theme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    Style = VisualStyle.Office2010;
                    Office2010Theme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    Style = VisualStyle.Office2010;
                    Office2010Theme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    if (this.Style == VisualStyle.Office2010)
                        Office2010Theme = Office2010Theme.Managed;
                    else
                        Office2007Theme = Office2007Theme.Managed;
                }
                else if (value == "Default")
                    Style = VisualStyle.Default;
                else if (value == "Office2003")
                    Style = VisualStyle.Office2003;
                else if (value == "OfficeXP")
                    Style = VisualStyle.OfficeXP;
                else if (value == "Office2007Outlook")
                    Style = VisualStyle.Office2007Outlook;
                else if (value == "VS2005")
                    Style = VisualStyle.VS2005;
                else if (value == "VS2010")
                    Style = VisualStyle.VS2010;
            }
        }
		/// <summary>
		/// Gets or sets colorschemes for Office2010 visual style.
		/// </summary>
		[
		Description( "Colorschemes for Office2010 visual style." ),
		Category( "Appearance" ),
		DefaultValue( Office2010Theme.Blue )
		]
		public Office2010Theme Office2010Theme
		{
			get { return m_office2010Theme; }
			set
			{
                if (m_office2010Theme != value)
				{
                    m_office2010Theme = value;
					Office2010ThemeChanged();
				}
			}
		}

		/// <summary>
		/// Gets colors for Office2007 visual style.
		/// </summary>
		protected internal Office2010Colors Office2010ColorTable
		{
			get
			{
				return m_office2010ColorTable;
			}
		}
		#endregion
        /// <summary>
        /// Gets or sets colorschemes for Office2007 visual style.
        /// </summary>
        [
        Description("Colorschemes for Office2007 visual style."),
        Category("Appearance"),
        DefaultValue(Office2007Theme.Blue)
        ]
        public Office2007Theme Office2007Theme
        {
            get { return m_office2007Theme; }
            set
            {
                if (m_office2007Theme != value)
                {
                    m_office2007Theme = value;
                    Office2007ThemeChanged();
                }
            }
        }

        /// <summary>
        /// Gets colors for Office2007 visual style.
        /// </summary>
        protected internal Office2007Colors Office2007ColorTable
        {
            get
            {
                return m_office2007ColorTable;
            }
        }
		private void Office2007ThemeChanged()
		{
			if( this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2007Outlook )
			{
				m_office2007ColorTable = Office2007Colors.GetColorTable( this.Office2007Theme );
				XPMenus.Office2007BarItemPainter.ColorTable = m_office2007ColorTable;
			}

			this.SetBarsTransparency();
		}
        private void Office2010ThemeChanged()
        {
            if (this.Style == VisualStyle.Office2010)
            {
                m_office2010ColorTable = Office2010Colors.GetColorTable(this.Office2010Theme);
                XPMenus.Office2010BarItemPainter.ColorTable = m_office2010ColorTable;
            }

            this.SetBarsTransparency();
        }
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetBarsTransparency()
		{
			if(this.CommandDockBarL != null)
				this.CommandDockBarL.Invalidate(false);
			if(this.CommandDockBarT != null)
				this.CommandDockBarT.Invalidate(false);
			if(this.CommandDockBarR != null)
				this.CommandDockBarR.Invalidate(false);
			if(this.CommandDockBarB != null)
				this.CommandDockBarB.Invalidate(false);
			foreach(CommandBar cbar in this.alCommandBars)
			{
				if(cbar.bBackColorSet == false)
				{
					if( this.bThemesEnabled == true 
						|| this.tbStyle == VisualStyle.Office2003
						|| this.tbStyle == VisualStyle.VS2005 
						|| this.tbStyle == VisualStyle.Office2007 
						|| this.tbStyle == VisualStyle.Office2007Outlook
                        || this.tbStyle == VisualStyle.Office2010)
					{
                        if (cbar.Floating)
                        {
                            cbar.BackColor = cbar.Parent.BackColor;
                        }
                        else
                        {
                            cbar.BackColor = Color.Transparent;
                        }

						cbar.bBackColorSet = false;
					}
					else if(cbar.BackColor == System.Drawing.Color.Transparent)
					{
						cbar.BackColor = this.frmMain.BackColor;
						cbar.bBackColorSet = false;
					}
				}
				if(cbar.Floating == true)
					cbar.Parent.Invalidate(true);
				else
					cbar.Invalidate(false);

				// Also update the PopupMenu's Style
				if( null != cbar.PopupMenu )
				{
					cbar.PopupMenu.ParentBarItem.Style = this.Style;
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color used to draw the host form's dockable regions.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> that represents the background color.</value>
		/// <remarks>
		/// The dockable region is the area along the <see cref="CommandBarController.HostForm"/>'s border
		/// on which the <see cref="CommandBar"/>s are docked. The <see cref="CommandBarController"/> uses the
		/// specified color for drawing the Form's dockable regions.
		/// </remarks>
		[
		Category("Appearance"),
		Description("The background color used to draw the host form's dockable regions."),
		Localizable(true)
		]
		public Color BackColor
		{
			get	{ return this.clrBackColor; }
			set
			{
				if(this.clrBackColor != value)
				{
					this.clrBackColor = value;
					if(this.CommandDockBarL != null)
						this.CommandDockBarL.BackColor = this.clrBackColor;
					if(this.CommandDockBarT != null)
						this.CommandDockBarT.BackColor = this.clrBackColor;
					if(this.CommandDockBarR != null)
						this.CommandDockBarR.BackColor = this.clrBackColor;
					if(this.CommandDockBarB != null)
						this.CommandDockBarB.BackColor = this.clrBackColor;
					this.bBackColorSet = true;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeBackColor()
		{
			return this.bBackColorSet;
		}

        /// <summary>
        /// Resumes the back color to default value.
        /// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void ResetBackColor()
		{
			this.clrBackColor = this.frmMain.BackColor;
			if(this.CommandDockBarL != null)
				this.CommandDockBarL.ResetBackColor();
			if(this.CommandDockBarT != null)
				this.CommandDockBarT.ResetBackColor();
			if(this.CommandDockBarR != null)
				this.CommandDockBarR.ResetBackColor();
			if(this.CommandDockBarB != null)
				this.CommandDockBarB.ResetBackColor();
			this.bBackColorSet = false;
		}

		internal bool DesignTime
		{
			get { return this.DesignMode; }
		}

		internal object GetDesignerService(Type service)
		{
			return this.GetService(service);
		}

		/// <summary>
		/// Overloaded. Creates a new instance of the <see cref="CommandBarController"/>.
		/// </summary>
		public CommandBarController()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( CommandBarController ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
			this.alCommandBars = new CommandBarsCollection( this );
		}

		/// <summary>
		/// Creates a new instance of the <see cref="CommandBarController"/> and initializes it with the container.
		/// </summary>
		/// <param name="container">An object implementing the <see cref="System.ComponentModel.IContainer"/> 
        /// interface to associate with this instance of the CommandBarController.</param>
		public CommandBarController(IContainer container)
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( CommandBarController ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
			container.Add( this );
			this.alCommandBars = new CommandBarsCollection( this );
		}

		/// <summary>
		/// Overloaded. Persists the current state of the <see cref="CommandBar"/> objects to IsolatedStorage.
		/// </summary>
		/// <seealso cref="CommandBarController.LoadCommandBarState()"/>
		public void SaveCommandBarState()
		{
			this.SaveCommandBarState(AppStateSerializer.GetSingleton());
		}

		/// <summary>
		/// Persists the current state of the <see cref="CommandBar"/> objects using the specified storage medium and location.
		/// </summary>
		/// <remarks>
		/// Writes the CommandBar state information onto the persistence medium specified by the
		/// <paramref name="mode"/> parameter and at the path specified by the <paramref name="persistpath"/> object.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is highly recommended that
        /// you use the <see cref="CommandBarController.SaveCommandBarState(AppStateSerializer)"/> and
        /// <see cref="CommandBarController.LoadCommandBarState(AppStateSerializer)"/> methods.
		/// <para>
		/// This method will be removed in a future version. Please use the more flexible SaveCommandBarState(AppStateSerializer) variant, instead.
		/// </para>
		/// </remarks>
		/// <param name="mode">A <see cref="SerializeMode"/> value describing the persistence medium.</param>
		/// <param name="persistpath">The name of the IsolatedStorage/INI/XML file or registry key in which the
		/// state information is to be persisted. </param>
		[Obsolete("This method will be removed in a future version. Please use the more flexible SaveCommandBarState(AppStateSerializer) variant, instead.", false)]
		public void SaveCommandBarState(SerializeMode mode, Object persistpath)
		{
			try
			{
				CBCtrlrSerializationWrapper cbcwppr = this.GetPersistanceData();
				if((mode == SerializeMode.IsolatedStorage) && (persistpath == null)) // Default serialization to Isolated Storage
				{
					AppStateSerializer serializer = AppStateSerializer.GetInstance();
					serializer.SerializeObject(this.PersistenceID + CommandBarController.strPersistKey, cbcwppr);
				}
				else	// User-invoked
				{
					AppStateSerializer.SerializeIsolatedObject(mode, persistpath, this.PersistenceID + CommandBarController.strPersistKey, cbcwppr);
				}
			}
			catch(Exception e)
			{
				Debug.Assert(false, "SaveCommandBarState Failed.", e.Message);
			}
		}

		/// <summary>
		/// Persists the current state of the <see cref="CommandBar"/> objects using the specified <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/>.
		/// </summary>
		/// <remarks>
		/// Writes the CommandBar state information onto the persistence medium.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is highly recommended that
        /// you use the <see cref="CommandBarController.SaveCommandBarState(AppStateSerializer)"/> and
        /// <see cref="CommandBarController.LoadCommandBarState(AppStateSerializer)"/> methods.
		/// </remarks>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		public void SaveCommandBarState(AppStateSerializer serializer)
		{
			if(serializer != null)
			{
				try
				{
					CBCtrlrSerializationWrapper cbcwppr = this.GetPersistanceData();
					serializer.SerializeObject(this.PersistenceID + CommandBarController.strPersistKey, cbcwppr);
				}
				catch(Exception e)
				{
					Debug.Assert(false, "SaveCommandBarState Failed.", e.Message);
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual CBCtrlrSerializationWrapper GetPersistanceData()
		{
			return new CBCtrlrSerializationWrapper(this);
		}

		// Used by external designer
		internal void SaveToStream(Stream file)
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				CBCtrlrSerializationWrapper cbcwppr = new CBCtrlrSerializationWrapper( this );
				BinaryFormatter fmtr = new BinaryFormatter();
				fmtr.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
				fmtr.Binder = AppStateSerializer.CustomBinder;
				fmtr.Serialize( file, cbcwppr );
			}
			catch( Exception e )
			{
				Debug.Assert( false, "SaveCommandBarState Failed.", e.Message );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
		}

		/// <summary>
		/// Overloaded. Retrieves the persisted state of the <see cref="CommandBar"/> objects from Isolated Storage.
		/// </summary>
		/// <returns>TRUE if the load is successful; FALSE otherwise.</returns>
        /// <seealso cref="CommandBarController.SaveCommandBarState(AppStateSerializer)"/>
		public bool LoadCommandBarState()
		{
			return this.LoadCommandBarState(AppStateSerializer.GetSingleton());
		}

		// Provides a way to lose the persisted state, if already loaded.
		internal void LoseCachedCommandBarState()
		{
			this.wpprCBController = null;
		}

		/// <summary>
		/// Retrieves the persisted state of the <see cref="CommandBar"/> objects using the specified storage and location.
		/// </summary>
		/// <param name="mode">A <see cref="SerializeMode"/> value describing the persistence medium.</param>
		/// <param name="persistpath">The name of the IsolatedStorage/INI/XML file or registry key containing the persisted information.</param>
		/// <returns>TRUE if the load is successful.</returns>
		/// <remarks>
		/// <para>Reads the CommandBar state information from the specified persistent store and
		/// applies the new state. This method has been provided only to allow a higher degree
		/// of control over the serialization process. For normal state storage and retrieval
        /// it is highly recommended that you use the <see cref="CommandBarController.SaveCommandBarState(AppStateSerializer)"/>
        /// and <see cref="CommandBarController.LoadCommandBarState(AppStateSerializer)"/> methods.</para>
		/// <para>
		/// This method will be removed in a future version. Please use the more flexible LoadCommandBarState(AppStateSerializer) variant, instead.
		/// </para>
		/// </remarks>
		[Obsolete("This method will be removed in a future version. Please use the more flexible LoadCommandBarState(AppStateSerializer) variant, instead.", false)]
		public bool LoadCommandBarState(SerializeMode mode, Object persistpath)
		{
			try
			{
				if((mode == SerializeMode.IsolatedStorage) && (persistpath == null))
				{
					AppStateSerializer serializer = AppStateSerializer.GetInstance();
					object deserialized = serializer.DeserializeObject(this.PersistenceID + CommandBarController.strPersistKey);
					this.wpprCBController =  deserialized as CBCtrlrSerializationWrapper;
				}
				else	// User-invoked
				{
					this.wpprCBController = AppStateSerializer.DeserializeIsolatedObject(mode,
						persistpath, this.PersistenceID + CommandBarController.strPersistKey) as CBCtrlrSerializationWrapper;
				}
				if(this.wpprCBController == null)
					return false;
				this.EnabledDockBorders = this.wpprCBController.cbBorder;
				this.ApplyDeserializedState(this.alCommandBars);
			}
			catch(Exception e)
			{
				Debug.Assert(false, "LoadCommandBarState Failed.", e.Message);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Retrieves the persisted state of the <see cref="CommandBar"/> objects using the specified <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/>.
		/// </summary>
		/// <param name="serializer">An instance of AppStateSerializer, from which to load.</param>
		/// <returns>TRUE if the load is successful.</returns>
		/// <remarks>
		/// Reads the CommandBar state information from the specified persistent store and
		/// applies the new state. This method has been provided only to allow a higher degree
		/// of  control over the serialization process. For normal state storage and retrieval
        /// it is highly recommended that you use the <see cref="CommandBarController.SaveCommandBarState(AppStateSerializer)"/>
		/// and <see cref="CommandBarController.LoadCommandBarState(AppStateSerializer)"/> methods.
		/// </remarks>
		public bool LoadCommandBarState(AppStateSerializer serializer)
		{
			if(serializer != null)
			{
				try
				{
					object deserialized = serializer.DeserializeObject(this.PersistenceID + CommandBarController.strPersistKey);
					this.wpprCBController = deserialized as CBCtrlrSerializationWrapper;

					if(this.wpprCBController == null)
						return false;
					this.EnabledDockBorders = this.wpprCBController.cbBorder;

                    this.FreezeLayout = true;
					this.ApplyDeserializedState(this.alCommandBars);
					// Ensures the postions of bars after loading the commanad bar state
					this.wpprCBController.CorrectCommandBarsOffset(this);
					this.FreezeLayout = false;
				}
				catch(Exception e)
				{
					Debug.Assert(false, "LoadCommandBarState Failed.", e.Message);
					return false;
				}
				return true;
			}
			return false;
		}

		internal void UpdateCachedCommandBarState()
		{
			this.wpprCBController = new CBCtrlrSerializationWrapper(this);
		}

		/// <summary>
		/// Restores the default state of the <see cref="CommandBar"/> objects.
		/// </summary>
		/// <remarks>
		/// Loads the state set within the designer.
		/// </remarks>
		/// <returns>TRUE if the load is successful.</returns>
		public bool LoadDesignerCommandBarState()
		{
			Debug.Assert(this.strmDefault.Length > 0);
			return this.LoadFromStream( this.strmDefault );
		}

		// Added for external designer
        internal bool LoadFromStream( Stream file )
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				BinaryFormatter fmtr = new BinaryFormatter();
				fmtr.AssemblyFormat = FormatterAssemblyStyle.Simple;
				fmtr.Binder = AppStateSerializer.CustomBinder;
				this.wpprCBController = fmtr.Deserialize( file ) as CBCtrlrSerializationWrapper;

				if( this.wpprCBController != null )
				{
					this.EnabledDockBorders = this.wpprCBController.cbBorder;
					ApplyDeserializedState(this.alCommandBars);
					return true;
				}
				return false;
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
		}

        /// <summary>
        /// Sets location for CommandBar from ChildFrameBarManager;
        /// </summary>
        internal void SetChildStartLocation()
        {
            foreach( CommandBar cBar in this.CommandBars )
            {
                this.wpprCBController.AddCommandBarData( cBar );
            }
        }

        /// <summary>
        /// Need this to avoid recursive mdichild and owned forms activating.
        /// Especially issue occurs when activating mdichild form, which is in minimized state, and
        /// has some floating forms related to it's ChildBarManager.
        /// </summary>
        protected int nFreezeActivation = 0;
        protected internal bool FreezeActivation
        {
            get { return this.nFreezeActivation > 0; }
            set
            {
                if(value)
					this.nFreezeActivation++;
                else if (this.nFreezeActivation > 0)
                {
                    this.nFreezeActivation--;
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        [Obsolete("This method will be removed in a future version. Please use the more flexible ApplyDeserializedState(IList) variant, instead.")]
        protected void ApplyDeserializedState(IEnumerator ebarlist)
        {
            this.FreezeActivation = true;

            CommandBarDockBorder dockborder = CommandBarDockBorder.Top;
            for (int i = 0; i < 4; i++, dockborder = (CommandBarDockBorder)((int)dockborder * 2))
            {
                CommandDockBar cdbparent = this.GetDockBar(dockborder);
                cdbparent.ResetBarState();
            }

            // Undock all docked CommandBars before reading the serialized data. This undocking/redocking
            // ensures that the CommandBars are docked correctly at their deserialized positions
            ebarlist.Reset();
            while (ebarlist.MoveNext())
            {
                CommandBar cb = ebarlist.Current as CommandBar;
                if (this.alCommandBars.Contains(cb))
                {
                    cb.CommandBarBaseVisible = false;
                    if (cb.DockState != CommandBarDockState.Float)
                        cb.cdbParent.RemoveCommandBar(cb);
                    else
                        cb.Parent.Visible = false;
                    cb.bRedockNeeded = false;
                    cb.bRecalcNeeded = false;
                }
            }

            ebarlist.Reset();
            while (ebarlist.MoveNext())
                ReadDeserializedData(ebarlist.Current as CommandBar);

            // Run through the CommandBar list and add each CommandBar to it's dockbarparent
            // For each dockborder, add CommandBars in sequence starting from row 0 to preserve the row order.
            ArrayList[][] cbarsTBLR = new ArrayList[4][];
            ArrayList cbarsF = new ArrayList();
            // First calculate the number of rows in each dockborder
            int ntop = 0, nbottom = 0, nleft = 0, nright = 0;
            ebarlist.Reset();
            while (ebarlist.MoveNext())
            {
                CommandBar cbar = ebarlist.Current as CommandBar;
                int nrcindex = cbar.nRCIndex + 1;
                switch (cbar.DockState)
                {
                    case CommandBarDockState.Top:
                        ntop = (nrcindex > ntop) ? nrcindex : ntop;
                        break;
                    case CommandBarDockState.Bottom:
                        nbottom = (nrcindex > nbottom) ? nrcindex : nbottom;
                        break;
                    case CommandBarDockState.Left:
                        nleft = (nrcindex > nleft) ? nrcindex : nleft;
                        break;
                    case CommandBarDockState.Right:
                        nright = (nrcindex > nright) ? nrcindex : nright;
                        break;
                }
            }
            cbarsTBLR[0] = new ArrayList[ntop];
            cbarsTBLR[1] = new ArrayList[nbottom];
            cbarsTBLR[2] = new ArrayList[nleft];
            cbarsTBLR[3] = new ArrayList[nright];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < cbarsTBLR[i].Length; j++)
                    cbarsTBLR[i][j] = new ArrayList();
            }
            ebarlist.Reset();
            while (ebarlist.MoveNext())
            {
                CommandBar cbar = ebarlist.Current as CommandBar;

                // cbar.nRCIndex may be -1, for example: in case when I have
                // some form with mainframebarManager and some Toolbars on that form,
                // and I create another form,- form1, INHERITED from form,
                // and add there are also some toolBars,
                // cbar.nRCIndex has actually value -1
                // and next switch-case block crashes.
                if (cbar.nRCIndex < 0)
                {
                    cbarsF.Add(cbar);
                    continue;
                }

                switch (cbar.DockState)
                {
                    case CommandBarDockState.Top:
                        cbarsTBLR[0][cbar.nRCIndex].Add(cbar);
                        break;
                    case CommandBarDockState.Bottom:
                        cbarsTBLR[1][cbar.nRCIndex].Add(cbar);
                        break;
                    case CommandBarDockState.Left:
                        cbarsTBLR[2][cbar.nRCIndex].Add(cbar);
                        break;
                    case CommandBarDockState.Right:
                        cbarsTBLR[3][cbar.nRCIndex].Add(cbar);
                        break;
                    default:
                        cbarsF.Add(cbar);
                        break;
                }
            }

            // Apply the deserialized position information for all docked CommandBars
            dockborder = CommandBarDockBorder.Top;
            for (int i = 0; i < 4; i++)
            {
                CommandDockBar cdbparent = this.GetDockBar(dockborder);

                foreach (ArrayList cbarlist in cbarsTBLR[i])
                {
                    foreach (CommandBar cbar in cbarlist)
                    {
                        Debug.Assert((cdbparent != null), "Error - Invalid Dock Border specified.");
                        CommandBarDockState border = cdbparent.GetDockBorder();
                        cbar.cbarDockState = CommandBarDockState.None;	// Temporarily set to Null DockState
                        cbar.FireCommandBarStateChanging(new CommandBarStateChangingEventArgs(border));
                        cbar.cbarDockState = border;
                        CommandBarForm frmfloating = null;
                        if (cbar.Parent != null)
                            frmfloating = cbar.Parent as CommandBarForm;
                        cbar.cdbParent = cdbparent;
                        cbar.DockState = border;
                        if (cbar.Visible)
                        {
                            cdbparent.AddCommandBar(cbar, false);
                            cbar.CommandBarBaseVisible = true;
                        }
                        if (frmfloating != null)
                        {
                            if (frmfloating.Controls.Contains(cbar))
                                frmfloating.Controls.Remove(cbar);
                            frmfloating.Close();
                        }
                        cbar.FireCommandBarStateChanged(EventArgs.Empty);
                    }
                }
                dockborder = (CommandBarDockBorder)((int)dockborder * 2);
            }

            // Apply deserialized info for the currently orphaned floating CommandBars
            foreach (CommandBar cbar in cbarsF)
            {
                cbar.cbarDockState = CommandBarDockState.None;
                cbar.FireCommandBarStateChanging(new CommandBarStateChangingEventArgs(CommandBarDockState.Float));
                cbar.cbarDockState = CommandBarDockState.Float;
                if (cbar.Parent == null || cbar.Parent.Location != cbar.FloatBounds.Location)
                    cbar.EnterFloatMode(cbar.FloatBounds.Location);

                if (cbar.Visible)
                {
                    if (cbar.Parent.Visible == false)
                        cbar.Parent.Visible = true;
                    cbar.CommandBarBaseVisible = true;
                }
                else
                    cbar.Parent.Visible = false;

                cbar.FireCommandBarStateChanged(EventArgs.Empty);

                if (!this.wpprCBController.IsInitialized(cbar))
                {
                    cbar.nRCIndex = -1;
                    cbar.nRowOffsetDir = 0;
                    cbar.nRowOffsetInDir = 0;
                }
            }
            this.RecalcLayout(CommandBarDockBorder.Left | CommandBarDockBorder.Top | CommandBarDockBorder.Right | CommandBarDockBorder.Bottom);

            this.FreezeActivation = false;
        }

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void ApplyDeserializedState(IList ebarlist)
		{
            this.FreezeActivation = true;

            CommandBarDockBorder dockborder = CommandBarDockBorder.Top;
            for (int i = 0; i < 4; i++, dockborder = (CommandBarDockBorder)((int)dockborder * 2))
            {
                CommandDockBar cdbparent = this.GetDockBar(dockborder);
                cdbparent.ResetBarState();
            }
            
            // Undock all docked CommandBars before reading the serialized data. This undocking/redocking
            // ensures that the CommandBars are docked correctly at their deserialized positions
            for (int i = 0; i < ebarlist.Count && ebarlist.Count > 0; i++)
            {
                CommandBar cb = ebarlist[i] as CommandBar;
                if (this.alCommandBars.Contains(cb))
                {
                    cb.CommandBarBaseVisible = false;
                    if (cb.DockState != CommandBarDockState.Float)
                        cb.cdbParent.RemoveCommandBar(cb);
                    else
                        cb.Parent.Visible = false;
                    cb.bRedockNeeded = false;
                    cb.bRecalcNeeded = false;
                }
            }

            for (int i = 0; i < ebarlist.Count && ebarlist.Count > 0; i++)
            {
                ReadDeserializedData(ebarlist[i] as CommandBar);
            }

			// Run through the CommandBar list and add each CommandBar to it's dockbarparent
			// For each dockborder, add CommandBars in sequence starting from row 0 to preserve the row order.
			ArrayList[][] cbarsTBLR = new ArrayList[4][];
			ArrayList cbarsF = new ArrayList();
			// First calculate the number of rows in each dockborder
			int ntop = 0, nbottom = 0, nleft = 0, nright = 0;
            for (int i = 0; i < ebarlist.Count && ebarlist.Count > 0; i++)
            {
				CommandBar cbar = ebarlist[i] as CommandBar;
				int nrcindex = cbar.nRCIndex + 1;
				switch(cbar.DockState)
				{
					case CommandBarDockState.Top:
						ntop = (nrcindex > ntop) ? nrcindex : ntop;
						break;
					case CommandBarDockState.Bottom:
						nbottom = (nrcindex > nbottom) ? nrcindex : nbottom;
						break;
					case CommandBarDockState.Left:
						nleft = (nrcindex > nleft) ? nrcindex : nleft;
						break;
					case CommandBarDockState.Right:
						nright = (nrcindex > nright) ? nrcindex : nright;
						break;
				}
			}
			cbarsTBLR[0] = new ArrayList[ntop];
			cbarsTBLR[1] = new ArrayList[nbottom];
			cbarsTBLR[2] = new ArrayList[nleft];
			cbarsTBLR[3] = new ArrayList[nright];
			for(int i=0; i<4; i++)
			{
				for(int j=0; j<cbarsTBLR[i].Length; j++)
					cbarsTBLR[i][j] = new ArrayList();
			}

            for (int i = 0; i < ebarlist.Count && ebarlist.Count > 0; i++)
			{
				CommandBar cbar = ebarlist[i] as CommandBar;

				// cbar.nRCIndex may be -1, for example: in case when I have
				// some form with mainframebarManager and some Toolbars on that form,
				// and I create another form,- form1, INHERITED from form,
				// and add there are also some toolBars,
				// cbar.nRCIndex has actually value -1
				// and next switch-case block crashes.
				if( cbar.nRCIndex < 0 )
				{
					cbarsF.Add(cbar);
					continue;
				}

				switch(cbar.DockState)
				{
					case CommandBarDockState.Top:
						cbarsTBLR[0][cbar.nRCIndex].Add(cbar);
						break;
					case CommandBarDockState.Bottom:
						cbarsTBLR[1][cbar.nRCIndex].Add(cbar);
						break;
					case CommandBarDockState.Left:
						cbarsTBLR[2][cbar.nRCIndex].Add(cbar);
						break;
					case CommandBarDockState.Right:
						cbarsTBLR[3][cbar.nRCIndex].Add(cbar);
						break;
					default:
						cbarsF.Add(cbar);
						break;
				}
			}

			// Apply the deserialized position information for all docked CommandBars
			dockborder = CommandBarDockBorder.Top;
			for(int i = 0; i < 4; i++)
			{
				CommandDockBar cdbparent = this.GetDockBar(dockborder);

				foreach(ArrayList cbarlist in cbarsTBLR[i])
				{
					foreach(CommandBar cbar in cbarlist)
					{
						Debug.Assert((cdbparent != null), "Error - Invalid Dock Border specified.");
						CommandBarDockState border = cdbparent.GetDockBorder();
						cbar.cbarDockState = CommandBarDockState.None;	// Temporarily set to Null DockState
						cbar.FireCommandBarStateChanging(new CommandBarStateChangingEventArgs(border));
						cbar.cbarDockState = border;
						CommandBarForm frmfloating = null;
						if(cbar.Parent != null)
							frmfloating = cbar.Parent as CommandBarForm;
						cbar.cdbParent = cdbparent;
						cbar.DockState = border;
						if(cbar.Visible)
						{
							cdbparent.AddCommandBar(cbar, false);
							cbar.CommandBarBaseVisible = true;
						}
						if(frmfloating != null)
						{
							if(frmfloating.Controls.Contains(cbar))
								frmfloating.Controls.Remove(cbar);
							frmfloating.Close();
						}
						cbar.FireCommandBarStateChanged(EventArgs.Empty);
					}
				}
				dockborder = (CommandBarDockBorder)((int)dockborder * 2);
			}

			// Apply deserialized info for the currently orphaned floating CommandBars
			foreach(CommandBar cbar in cbarsF)
			{
				cbar.cbarDockState = CommandBarDockState.None;
				cbar.FireCommandBarStateChanging(new CommandBarStateChangingEventArgs(CommandBarDockState.Float));
				cbar.cbarDockState = CommandBarDockState.Float;
				if(cbar.Parent == null || cbar.Parent.Location != cbar.FloatBounds.Location)
					cbar.EnterFloatMode(cbar.FloatBounds.Location);

				if(cbar.Visible)
				{
					if(cbar.Parent.Visible == false)
						cbar.Parent.Visible = true;
					cbar.CommandBarBaseVisible = true;
				}
				else
					cbar.Parent.Visible = false;

				cbar.FireCommandBarStateChanged(EventArgs.Empty);

                if (!this.wpprCBController.IsInitialized(cbar))
                {
                    cbar.nRCIndex = -1;
                    cbar.nRowOffsetDir = 0;
                    cbar.nRowOffsetInDir = 0;
                }
			}
			this.RecalcLayout(CommandBarDockBorder.Left|CommandBarDockBorder.Top|CommandBarDockBorder.Right|CommandBarDockBorder.Bottom);

            this.FreezeActivation = false;
		}

        internal bool ShouldHostFormGetFocus()
        {
            bool setFocus = true;

            if( NativeMethods.GetActiveWindow() != this.HostForm.Handle )
            {
                MdiClient client = null;
                foreach( Control c in this.HostForm.Controls )
                {
                    if( c is MdiClient )
                    {
                        client = c as MdiClient;
                        break;
                    }
                }

                if( client != null )
                {
                    bool bMax = false;
                    IntPtr child = NativeMethods.SendMessage( client.Handle, NativeMethods.WM_MDIGETACTIVE, IntPtr.Zero, out bMax );
                    Form childForm = Control.FromHandle( child ) as Form;

                    if( ( bMax && childForm != null && childForm.WindowState == FormWindowState.Minimized ) || this.FreezeActivation )
                    {
                        setFocus = false;
                    }
                }
            }

            return setFocus;
        }

        /// <summary>
        /// Reads deserialized data to CommandBar.
        /// </summary>
        protected virtual void ReadDeserializedData( CommandBar commandBar )
        {
            if( commandBar != null )
            {
                this.wpprCBController.ReadDeserializedData( commandBar );
            }
        }

        protected virtual void RemoveCommandBarData(CommandBar commandBar)
        {
            if(commandBar != null)
            {
                this.wpprCBController.RemoveCommandBarData(commandBar);
            }
        }

        /// <summary>
        /// Loads the command bar state from cache
        /// </summary>
        /// <param name="cbar"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void LoadCommandBarStateFromCache(CommandBar cbar)
		{
			if(this.wpprCBController == null)
				return;

			if(this.alCommandBars.Contains(cbar))
			{
				if(this.FreezeLayout == false)
				{
					cbar.CommandBarBaseVisible = false;
					if(cbar.DockState != CommandBarDockState.Float)
						cbar.cdbParent.RemoveCommandBar(cbar);
					else
						cbar.Parent.Visible = false;
					cbar.bRedockNeeded = false;
					cbar.bRecalcNeeded = false;

                    ReadDeserializedData( cbar );

					// Apply the deserialized position information
					if(cbar.DockState != CommandBarDockState.Float)
					{
						CommandDockBar cdbparent = this.GetDockBar(cbar.DockState);
						Debug.Assert((cdbparent != null), "Error - Invalid Dock Border specified.");
						CommandBarDockState border = cdbparent.GetDockBorder();
						cbar.cbarDockState = CommandBarDockState.None;
						cbar.FireCommandBarStateChanging(new CommandBarStateChangingEventArgs(border));
						cbar.cbarDockState = border;
						CommandBarForm frmfloating = null;
						if(cbar.Parent != null)
							frmfloating = cbar.Parent as CommandBarForm;
						cbar.DockState = border;
						cbar.cdbParent = cdbparent;
						//cbar.cdbParent.ValidateBarIndex(cbar, true);	// Removed for StatusBar issue
						if(cbar.Visible == true)
						{
							cdbparent.AddCommandBar(cbar, false);
							cbar.CommandBarBaseVisible = true;
						}
						if(frmfloating != null)
						{
							if(frmfloating.Controls.Contains(cbar))
								frmfloating.Controls.Remove(cbar);
							frmfloating.Close();
						}
						cbar.FireCommandBarStateChanged(EventArgs.Empty);
					}
					else
					{
						cbar.cbarDockState = CommandBarDockState.None;
						cbar.FireCommandBarStateChanging(new CommandBarStateChangingEventArgs(CommandBarDockState.Float));
						cbar.cbarDockState = CommandBarDockState.Float;
						if(cbar.Parent == null)
							cbar.EnterFloatMode(cbar.FloatBounds.Location);
						if(cbar.Visible)
						{
							if (!cbar.Parent.Visible)
								cbar.Parent.Visible = true;
							cbar.CommandBarBaseVisible = true;
						}
						else
							cbar.Parent.Visible = false;
						cbar.FireCommandBarStateChanged(EventArgs.Empty);

                        if (!this.wpprCBController.IsInitialized(cbar))
                        {
                            cbar.nRCIndex = -1;
                            cbar.nRowOffsetDir = 0;
                            cbar.nRowOffsetInDir = 0;
                        }
					}
					this.RecalcLayout(cbar);
				}
				else	// FreezeLayout == true
				{
					if (!this.alFreezeRelayoutBars.Contains(cbar))
						this.alFreezeRelayoutBars.Add(cbar);
				}
			}
			else
			{
                ReadDeserializedData( cbar );
			}
		}

		/// <summary>
		/// Overloaded. Forces a layout recalculation.
		/// </summary>
		/// <remarks>
		/// Forces a recalculation of all <see cref="CommandBar"/>s occupied by the
		/// state specified by the <see cref="CommandBarDockBorder"/> value.
		/// </remarks>
		/// <param name="style">A <see cref="CommandBarDockBorder"/> value representing the CommandBar position.</param>
		public void RecalcLayout(CommandBarDockBorder style)
		{
			if(style == (CommandBarDockBorder.Left|CommandBarDockBorder.Top|CommandBarDockBorder.Right|CommandBarDockBorder.Bottom))
			{
				style = CommandBarDockBorder.Top;
				for(int i=0; i<4; i++)
				{
					CommandDockBar cdb = this.GetDockBar(style);
					if(cdb != null)
					{
						cdb.CalcDockbarSize();
						cdb.LayoutDockBar();
					}
					style = (CommandBarDockBorder)((int)style * 2);
				}
				// Issue a recalc layout on all floating CommandBars
				foreach(CommandBar cbar in this.alCommandBars)
				{
					if(cbar.DockState == CommandBarDockState.Float)
						this.RecalcLayout(cbar);
				}
			}
			else
			{
				CommandDockBar cdb = GetDockBar(style);
				if(cdb != null)
				{
					cdb.CalcDockbarSize();
					cdb.LayoutDockBar();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="border"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void RecalcBorderLayout(CommandBarDockState border)
		{
			CommandBarDockBorder style = (CommandBarDockBorder)border;
			this.RecalcLayout(style);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dockBarToLayout"></param>
		/// <param name="dockBarToIgnore"></param>
		private void LayoutDockBar( CommandDockBar dockBarToLayout, CommandDockBar dockBarToIgnore )
		{
			if( dockBarToLayout != null && dockBarToLayout != dockBarToIgnore )
			{
				dockBarToLayout.LayoutDockBar();
			}
		}
		/// <summary>
		/// Layout the bars which are docked.
		/// </summary>
		/// <param name="dockBarToIgnore">The dock bar which should be ignored while layouting.</param>
		public void LayoutDockBarsExceptBar( CommandDockBar dockBarToIgnore )
		{
			LayoutDockBar( this.CommandDockBarT, dockBarToIgnore );
			LayoutDockBar( this.CommandDockBarR, dockBarToIgnore );
			LayoutDockBar( this.CommandDockBarB, dockBarToIgnore );
			LayoutDockBar( this.CommandDockBarL, dockBarToIgnore );
		}
		/// <summary>
		/// Forces a layout recalculation on the specified <see cref="CommandBar"/>.
		/// </summary>
		/// <param name="cbar">The CommandBar for which the layout is to be recalculated.</param>
		public void RecalcLayout(CommandBar cbar)
		{
			CommandBarDockState border = cbar.DockState;
			if(border != CommandBarDockState.Float)
			{
				this.RecalcBorderLayout(border);
			}
			else	// Force a recalc of the floating frame
			{
				CommandBarForm cbform = cbar.Parent as CommandBarForm;

				if( null != cbform )
				{
                    cbform.Size = cbar.CalculateFloatingSize();
					cbar.rcFloat = cbform.Bounds;
				}

				cbar.Invalidate(cbar.CaptionRect, false);
			}
		}
		/// <summary>
		/// Brings the CommandBar to the front of the z-order.
		/// </summary>
		public void BringToFront()
		{
			if( this.CommandDockBarB != null )
				this.CommandDockBarB.BringToFront();

			if (this.CommandDockBarL != null)
				this.CommandDockBarL.BringToFront();

			if (this.CommandDockBarR != null)
				this.CommandDockBarR.BringToFront();

			if (this.CommandDockBarT != null)
				this.CommandDockBarT.BringToFront();
		}
		/// <summary>
		/// Sends the CommandBar to the back of the z-order.
		/// </summary>
		public void SendToBack()
		{
			if( this.CommandDockBarB != null )
				this.CommandDockBarB.SendToBack();

			if( this.CommandDockBarL != null )
				this.CommandDockBarL.SendToBack();

			if( this.CommandDockBarR != null )
				this.CommandDockBarR.SendToBack();
            
			if( this.CommandDockBarT != null )
				this.CommandDockBarT.SendToBack();
		}
		/// <summary>
		/// 
		/// </summary>
		internal void SuspendLayout()
		{
			if (this.LayoutSuspended != null)
			{
				this.LayoutSuspended(this, EventArgs.Empty);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal void ResumeLayout()
		{
			if (this.LayoutResumed != null)
			{
				this.LayoutResumed(this, EventArgs.Empty);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="border"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandDockBar GetDockBar(CommandBarDockState border)
		{
			CommandDockBar cdb = null;
			switch(border)
			{
				case CommandBarDockState.Left:
					cdb = this.CommandDockBarL;
					break;
				case CommandBarDockState.Top:
					cdb = this.CommandDockBarT;
					break;
				case CommandBarDockState.Right:
					cdb = this.CommandDockBarR;
					break;
				case CommandBarDockState.Bottom:
					cdb = this.CommandDockBarB;
					break;
			}
			return cdb;
		}

        /// <summary>
        /// Gets the Dock bar
        /// </summary>
        /// <param name="dockstyle"></param>
        /// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public CommandDockBar GetDockBar(CommandBarDockBorder dockstyle)
		{
			CommandDockBar cdb = null;
			switch(dockstyle)
			{
				case CommandBarDockBorder.Left:
					cdb = this.CommandDockBarL;
					break;
				case CommandBarDockBorder.Top:
					cdb = this.CommandDockBarT;
					break;
				case CommandBarDockBorder.Right:
					cdb = this.CommandDockBarR;
					break;
				case CommandBarDockBorder.Bottom:
					cdb = this.CommandDockBarB;
					break;
			}
			return cdb;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal CommandDockBar GetDockBar(Point ptscreen)
		{
			Point ptclient = this.frmMain.PointToClient(ptscreen);
			if((this.CommandDockBarL != null) &&
				(Rectangle.Inflate(this.CommandDockBarL.Bounds, CommandBar.nDefaultUnitHt, 0).Contains(ptclient)))
				return this.CommandDockBarL;
			else if((this.CommandDockBarT != null) &&
				(Rectangle.Inflate(this.CommandDockBarT.Bounds, 0, CommandBar.nDefaultUnitHt).Contains(ptclient)))
				return this.CommandDockBarT;
			else if((this.CommandDockBarR != null) &&
				(Rectangle.Inflate(this.CommandDockBarR.Bounds, CommandBar.nDefaultUnitHt, 0).Contains(ptclient)))
				return this.CommandDockBarR;
			else if((this.CommandDockBarB != null) &&
				(Rectangle.Inflate(this.CommandDockBarB.Bounds, 0, CommandBar.nDefaultUnitHt).Contains(ptclient)))
				return this.CommandDockBarB;

			return null;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void ToggleCommandBarFloatState(bool bactivate)
		{
			if(bactivate == true)
			{
				// Unhide all floating CommandBars
				foreach(CommandBar cbar in this.alCommandBars)
				{
					Control ctrlparent = cbar.Parent;
					if((ctrlparent is CommandBarForm) && (ctrlparent.Visible == false)
						&& (cbar.Visible == true))
					{
						Syncfusion.Runtime.InteropServices.NativeMethods.ShowWindow(ctrlparent.Handle, 4/*SW_SHOWNOACTIVATE*/);
						ctrlparent.Visible = true;
					}
				}
			}
			else
			{
				// Hide all floating CommandBars
				foreach(CommandBar cbar in this.alCommandBars)
				{
					Control ctrlparent = cbar.Parent;
					if((ctrlparent is CommandBarForm) && (ctrlparent.Visible == true))
						ctrlparent.Visible = false;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void InitializeCBController()
		{
			Debug.Assert(this.frmMain != null);

			InitializeDockBars();

			if((this.DesignMode == false) && (this.wndMainFrm == null))
			{
				// Subclass the main form hosting the CommandBars
				this.wndMainFrm = new AppMainFormWnd(this);
				this.wndMainFrm.AssignHandleCustom(this.frmMain);
			}

			if(this.alCommandBars.Count > 0)
			{
				ArrayList alinherited = new ArrayList();
				foreach(CommandBar cbar in this.alCommandBars)
				{
					if(cbar.cdbParent != null)	// An inherited CommandBar
						alinherited.Add(cbar);
				}
				Hashtable htwrapper = new Hashtable();
				foreach(CommandBar cbar in alinherited)
				{
					CBarSerializationWrapper wrapper = new CBarSerializationWrapper();
					wrapper.cbarDockState = (cbar.cbDockStateT == CommandBarDockState.None) ? cbar.cbarDockState : cbar.cbDockStateT;
					wrapper.nRowOffsetDir = (cbar.nRowOffsetDirT == -1) ? cbar.nRowOffsetDir : cbar.nRowOffsetDirT;
					wrapper.nRCIndex = (cbar.nRCIndexT == -1) ? cbar.nRCIndex : cbar.nRCIndexT;
					htwrapper.Add(cbar, wrapper);

					cbar.bRedockNeeded = false;
					cbar.bRecalcNeeded = false;
					cbar.ResetTempDefaults();
				}
				foreach(CommandBar cbar in alinherited)
				{
					if(cbar.cbarDockState != CommandBarDockState.Float)
						cbar.cdbParent.RemoveCommandBar(cbar);
					else
					{
						CommandBarForm cbarform = cbar.Parent as CommandBarForm;
						Debug.Assert(cbarform != null);
						cbarform.Controls.Remove(cbar);
						cbarform.Close();
					}
				}
				foreach(CommandBar cbar in alinherited)
				{
					CBarSerializationWrapper wrapper = htwrapper[cbar] as CBarSerializationWrapper;
					cbar.cbarDockState = wrapper.cbarDockState;
					cbar.nRowOffsetDir = wrapper.nRowOffsetDir;
					cbar.nRowOffsetInDir = cbar.nRowOffsetDir;
					cbar.nRCIndex = wrapper.nRCIndex;
					cbar.cdbParent = null;
				}

				// If CommandBars have their default index values, then update em, so that
				// they get added as separate rows in the parent dockbar
				foreach(CommandBar cbar in this.alCommandBars)
				{
					if(cbar.nRCIndex == -1)
					{
						if(cbar.cdbParent == null)
						{
							if(this.CommandDockBarT != null)
								cbar.cdbParent = this.CommandDockBarT;	// The default dockbar
							else if(this.CommandDockBarB != null)
								cbar.cdbParent = this.CommandDockBarB;
							else if(this.CommandDockBarL != null)
								cbar.cdbParent = this.CommandDockBarL;
							else if(this.CommandDockBarR != null)
								cbar.cdbParent = this.CommandDockBarR;
							else if(cbar.DisableFloating == false)
								cbar.DockState = CommandBarDockState.Float;
							else
								throw new ApplicationException("The CommandBar DockState value is invalid.");
						}
						if(cbar.cdbParent != null)
							cbar.nRCIndex = cbar.cdbParent.nRCCount;
					}
				}

				// Run through the CommandBar list and add each CommandBar to it's dockbarparent
				// For each dockborder, add CommandBars in sequence starting from row 0 to preserve the row order.
				ArrayList[][] cbarsTBLR = new ArrayList[4][];
				ArrayList cbarsF = new ArrayList();
				// First calculate the number of rows in each dockborder
				int ntop = 0, nbottom = 0, nleft = 0, nright = 0;
				IEnumerator ebarlist = this.alCommandBars.GetEnumerator();
				ebarlist.Reset();
				while(ebarlist.MoveNext())
				{
					CommandBar cbar = ebarlist.Current as CommandBar;
					int nrcindex = cbar.nRCIndex+1;
					switch(cbar.DockState)
					{
						case CommandBarDockState.Top:
							ntop = (nrcindex > ntop) ? nrcindex : ntop;
							break;
						case CommandBarDockState.Bottom:
							nbottom = (nrcindex > nbottom) ? nrcindex : nbottom;
							break;
						case CommandBarDockState.Left:
							nleft = (nrcindex > nleft) ? nrcindex : nleft;
							break;
						case CommandBarDockState.Right:
							nright = (nrcindex > nright) ? nrcindex : nright;
							break;
					}
				}
				cbarsTBLR[0] = new ArrayList[ntop];
				cbarsTBLR[1] = new ArrayList[nbottom];
				cbarsTBLR[2] = new ArrayList[nleft];
				cbarsTBLR[3] = new ArrayList[nright];
				for(int i=0; i<4; i++)
				{
					for(int j=0; j<cbarsTBLR[i].Length; j++)
						cbarsTBLR[i][j] = new ArrayList();
				}
				ebarlist.Reset();
				while(ebarlist.MoveNext())
				{
					CommandBar cbar = ebarlist.Current as CommandBar;
					switch(cbar.DockState)
					{
						case CommandBarDockState.Top:
							cbarsTBLR[0][cbar.nRCIndex].Add(cbar);
							break;
						case CommandBarDockState.Bottom:
							cbarsTBLR[1][cbar.nRCIndex].Add(cbar);
							break;
						case CommandBarDockState.Left:
							cbarsTBLR[2][cbar.nRCIndex].Add(cbar);
							break;
						case CommandBarDockState.Right:
							cbarsTBLR[3][cbar.nRCIndex].Add(cbar);
							break;
						default:
							cbarsF.Add(cbar);
							break;
					}
				}

				CommandBarDockState dockborder = CommandBarDockState.Top;
				for(int i = 0; i < 4; i++)
				{
					CommandDockBar cdbparent = this.GetDockBar(dockborder);
					foreach(ArrayList cbarlist in cbarsTBLR[i])
					{
						foreach(CommandBar cbar in cbarlist)
							SetCommandBarInitialPosition(cbar);
					}
					dockborder = (CommandBarDockState)((int)dockborder * 2);
				}
				foreach(CommandBar cbarfloat in cbarsF)
					SetCommandBarInitialPosition(cbarfloat);
				this.RecalcLayout(CommandBarDockBorder.Left|CommandBarDockBorder.Top|CommandBarDockBorder.Right|CommandBarDockBorder.Bottom);
			}

			this.frmMain.ControlAdded += new ControlEventHandler(this.HostForm_ControlAdded);
			this.frmMain.ControlRemoved += new ControlEventHandler(this.HostForm_ControlRemoved);
			this.frmMain.RightToLeftChanged += new EventHandler(this.HostForm_RightToLeftChanged);

			foreach(Control ctrl in this.frmMain.Controls)
			{
				if(!(ctrl is CommandDockBar))
				{
					ctrl.DockChanged += new EventHandler(this.ChildControl_DockChanged);
				}
			}

			bDisableButtons = DesignMode;
			this.bInitializationComplete = true;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void InitializeDockBars()
		{
			Debug.Assert(this.frmMain != null, "Error - Controller is yet to be initialized.");

			// Clear any existing dockbars that are not required by the current dockborder setting
			if( (this.CommandDockBarL != null) &&
				((this.cbDockBorder & ~(CommandBarDockBorder.Top|CommandBarDockBorder.Bottom|CommandBarDockBorder.Right)) != CommandBarDockBorder.Left) )
			{
				this.frmMain.Controls.Remove(this.CommandDockBarL);
				this.CommandDockBarL.Visible = false;
				this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarL, false));
				this.CommandDockBarL.Dispose();
				this.CommandDockBarL = null;
			}
			if( (this.CommandDockBarR != null) &&
				((this.cbDockBorder & ~(CommandBarDockBorder.Top|CommandBarDockBorder.Bottom|CommandBarDockBorder.Left)) != CommandBarDockBorder.Right) )
			{
				this.frmMain.Controls.Remove(this.CommandDockBarR);
				this.CommandDockBarR.Visible = false;
				this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarR, false));
				this.CommandDockBarR.Dispose();
				this.CommandDockBarR = null;
			}
			if( (this.CommandDockBarT != null) &&
				((this.cbDockBorder & ~(CommandBarDockBorder.Bottom|CommandBarDockBorder.Left|CommandBarDockBorder.Right)) != CommandBarDockBorder.Top) )
			{
				this.frmMain.Controls.Remove(this.CommandDockBarT);
				this.CommandDockBarT.Visible = false;
				this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarT, false));
				this.CommandDockBarT.Dispose();
				this.CommandDockBarT = null;
			}
			if( (this.CommandDockBarB != null) &&
				((this.cbDockBorder & ~(CommandBarDockBorder.Top|CommandBarDockBorder.Left|CommandBarDockBorder.Right)) != CommandBarDockBorder.Bottom) )
			{
				this.frmMain.Controls.Remove(this.CommandDockBarB);
				this.CommandDockBarB.Visible = false;
				this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarB, false));
				this.CommandDockBarB.Dispose();
				this.CommandDockBarB = null;
			}

			// Create the commanddockbars specified in the current border attribute
			String strID = this.PersistenceID;
			if((this.CommandDockBarL == null) &&
				((this.cbDockBorder & ~(CommandBarDockBorder.Top|CommandBarDockBorder.Bottom|CommandBarDockBorder.Right)) == CommandBarDockBorder.Left))
			{
				this.CommandDockBarL = this.CreateCommandDockBar();
				this.CommandDockBarL.Name = String.Concat("CommandDockBarLeft", strID);
				this.frmMain.Controls.Add(this.CommandDockBarL);
				this.CommandDockBarL.Dock = System.Windows.Forms.DockStyle.Left;
				this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarL, true));
			}
			if((this.CommandDockBarR == null) &&
				((this.cbDockBorder & ~(CommandBarDockBorder.Top|CommandBarDockBorder.Bottom|CommandBarDockBorder.Left)) == CommandBarDockBorder.Right))
			{
				this.CommandDockBarR = this.CreateCommandDockBar();
				this.CommandDockBarR.Name = String.Concat("CommandDockBarRight", strID);
				this.frmMain.Controls.Add(this.CommandDockBarR);
				this.CommandDockBarR.Dock = System.Windows.Forms.DockStyle.Right;
				this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarR, true));
			}
			if((this.CommandDockBarT == null) &&
				((this.cbDockBorder & ~(CommandBarDockBorder.Bottom|CommandBarDockBorder.Left|CommandBarDockBorder.Right)) == CommandBarDockBorder.Top))
			{
				this.CommandDockBarT = this.CreateCommandDockBar();
				this.CommandDockBarT.Name = String.Concat("CommandDockBarTop", strID);
				this.frmMain.Controls.Add(this.CommandDockBarT);
				this.CommandDockBarT.Dock = System.Windows.Forms.DockStyle.Top;
				this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarT, true));
			}
			if( (this.CommandDockBarB == null) &&
				((this.cbDockBorder & ~(CommandBarDockBorder.Top|CommandBarDockBorder.Left|CommandBarDockBorder.Right)) == CommandBarDockBorder.Bottom))
			{
				this.CommandDockBarB = this.CreateCommandDockBar();
				this.CommandDockBarB.Name = String.Concat("CommandDockBarBottom", strID);
				this.frmMain.Controls.Add(this.CommandDockBarB);
				this.CommandDockBarB.Dock = System.Windows.Forms.DockStyle.Bottom;
				this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarB, true));
			}

			ResetDockBarZOrder();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual CommandDockBar CreateCommandDockBar()
		{
			return new CommandDockBar(this);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnDockBarStateChanged(DockBarStateEventArgs args)
		{
			if(this.DockBarStateChanged != null)
			{
				this.DockBarStateChanged(this, args);
			}
		}

		/// <summary>
		/// Raises the ProvidePersisteceID event.
		/// </summary>
		/// <param name="e">
		/// An ProvidePersistenceIDEventArgs object containing data pertaining to this event.
		/// </param>
		/// <remarks>
		/// The OnProvidePresistenceID method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// <para>Notes to Inheritors:  When overriding OnProvidePresistenceID in a derived
		/// class, be sure to call the base class's OnProvidePresistenceID method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnProvidePresistenceID(ProvidePersistenceIDEventArgs e)
		{
			if(this.ProvidePersisteceID != null)
				this.ProvidePersisteceID(this, e);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal virtual string PersistenceID
		{
			get
			{
				ProvidePersistenceIDEventArgs e = new ProvidePersistenceIDEventArgs(String.Empty);
				this.OnProvidePresistenceID(e);
				if(e.PersistenceID != String.Empty)
					return String.Concat(e.PersistenceID, ":");
				else
					return String.Empty;
			}
		}

        /// <summary>
        /// Resumes the dock bar z-order to default value.
        /// </summary>
		[
		Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void ResetDockBarZOrder()
		{
			if( m_bInternalDocking )
			{
				BringToFront();
			}
			else
			{
				SendToBack();
			}

            if( this.frmMain != null && this.frmMain.Controls != null )
            {
                foreach( Control control in this.frmMain.Controls )
                {
                    if( control is System.Windows.Forms.StatusBar 
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						|| control is StatusStrip 
#endif
						|| control is IStatusBarAdv )
                    {
                        control.SendToBack();
                        break;
                    }
                }
            }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetCommandBarInitialPosition(CommandBar cbar)
		{
			if(cbar.DockState != CommandBarDockState.Float)
			{
				// If the CommandBar is yet to be assigned a parent dockbar, then do so.
				if(cbar.cdbParent == null)
					cbar.cdbParent = this.GetDockBar(cbar.DockState);
				if(cbar.cdbParent != null)
					this.SetInitialDockedPosition(cbar);
				else	// Default to a floating state
					this.SetInitialFloatingPosition(cbar);
			}
			else
			{
				this.SetInitialFloatingPosition(cbar);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetInitialDockedPosition(CommandBar cbar)
		{
			Debug.Assert(cbar.cdbParent != null, "Error - The CommandBarDockState is yet to be initialized.");

			if(cbar.nRCIndex == -1)
			{
				cbar.cdbParent.InitializeIndexRowOffsets(cbar);
			}

			if(cbar.nRCIndex > 0)
			{
				CommandBar[] cbprevarray = cbar.cdbParent.GetRowArray(cbar.nRCIndex-1);
				if(cbprevarray.Length == 0)
				{
					// If cbprevarray is 0, then there is a row-index mismatch caused by the
					// loading order. Hold off initializing this CommandBar, till the bars in
					// the previous rows have been added.
					if(this.alDelayLoadBars.Contains(cbar) == false)
						this.alDelayLoadBars.Add(cbar);
					return;
				}
			}

			if(cbar.nRowOffsetDir == -1)
			{
				cbar.nRowOffsetDir = 0;
				cbar.cdbParent.InitializeRowOffsets(cbar);
			}

			if (cbar.Visible)
			{
				CommandBarDockState border = cbar.cdbParent.GetDockBorder();
				cbar.cbarDockState = CommandBarDockState.None;
				cbar.FireCommandBarStateChanging(new CommandBarStateChangingEventArgs(border));
				cbar.cbarDockState = border;

                cbar.cdbParent.IsInitializing = true;
				cbar.cdbParent.AddCommandBar(cbar, false);
                cbar.cdbParent.IsInitializing = false;

				// A new bar has been added. If there are any bars in the delay list for
				// the parent CommandDockBar, then add them.
				ArrayList delaylist = new ArrayList();
				foreach(CommandBar bar in this.alDelayLoadBars)
				{
					if(bar.cdbParent == cbar.cdbParent)
						delaylist.Add(bar);
				}

				if(delaylist.Count > 0)
				{
					foreach(CommandBar cbardelayed in delaylist)
					{
						if(cbardelayed.nRCIndex > cbar.nRCIndex)
						{
							this.alDelayLoadBars.Remove(cbardelayed);
                            this.SetCommandBarInitialPosition(cbardelayed);
                        }
					}
					delaylist.Clear();
				}

				cbar.FireCommandBarStateChanged(EventArgs.Empty);
			}
			else
			{
				// The CommandBar is not shown now. Reset nRCIndex and RowOffset values to pre-initialized state.
				// The CommandBar will be reinitialized later on when it's Visible property is set to TRUE.

                if (null != wpprCBController && !this.wpprCBController.IsInitialized(cbar))
                {
                    cbar.nRCIndex = -1;
                    cbar.nRowOffsetDir = -1;
                    cbar.nRowOffsetInDir = -1;
                }    
				
                cbar.CommandBarBaseVisible = false;
				cbar.cdbParent.Controls.Add(cbar);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetInitialFloatingPosition(CommandBar cbar)
		{
			cbar.cbarDockState = CommandBarDockState.None;
			cbar.FireCommandBarStateChanging(new CommandBarStateChangingEventArgs(CommandBarDockState.Float));
			cbar.cbarDockState = CommandBarDockState.Float;
			if((this.frmMain != null) && (cbar.FloatBounds.IsEmpty == true))
				cbar.EnterFloatMode(this.frmMain.Location);
			else
				cbar.EnterFloatMode(cbar.FloatBounds.Location);
			if((cbar.Visible == false) && (cbar.Parent != null))
				cbar.Parent.Visible = false;
			cbar.FireCommandBarStateChanged(EventArgs.Empty);

            if( null != wpprCBController && !this.wpprCBController.IsInitialized(cbar) )
            {
                cbar.nRCIndex = -1;
                cbar.nRowOffsetDir = 0;
                cbar.nRowOffsetInDir = 0;
            }
		}

		// ISupportInitialize implementation
		/// <summary>
		/// Begins the initialization of the <see cref="CommandBarController"/> component.
		/// </summary>
		[ EditorBrowsable(EditorBrowsableState.Never) ]
		public virtual void BeginInit()
		{
			// If BeginInit/EndInit has already been called once as in a derived Form, then unsubscribe
			// from the previous handler.
			if(this.frmMain != null)
				this.frmMain.Layout -= new System.Windows.Forms.LayoutEventHandler(this.HostForm_Layout);

			this.bInitializationComplete = false;
			if(this.DesignMode == false)
				this.bLoadVisibility = false;
		}

		/// <summary>
		/// Ends the initialization of the <see cref="CommandBarController"/> component.
		/// </summary>
		[ EditorBrowsable(EditorBrowsableState.Never) ]
		public virtual void EndInit()
		{
			if(this.frmMain != null)
			{
				if(this.DesignMode == false)
				{
					this.frmMain.Layout += new LayoutEventHandler(this.HostForm_Layout);
					this.frmMain.ResumeLayout( true );
					this.frmMain.PerformLayout();
				}
				else
				{
					this.InitializeCBController();
					this.ResetDockBarZOrder();
				}
			}
			else
			{
				this.bInitializationComplete = true;
				this.bLoadVisibility = true;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostForm_Load(object sender, EventArgs e)
		{
			this.bLoadVisibility = true;
			foreach(CommandBar cbar in this.alCommandBars)
			{
				if((cbar.Floating == true) && (cbar.Visible == true))
					cbar.Parent.Visible = true;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostForm_Closing(object sender, CancelEventArgs e)
		{
			if((this.DesignMode == false) && (this.bPersistState == true))
			{
				this.SaveCommandBarState();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostForm_Layout(object sender, LayoutEventArgs levent)
		{
			this.frmMain.Layout -= new LayoutEventHandler(this.HostForm_Layout);

			Syncfusion.Runtime.InteropServices.NativeMethodsHelper.SuspendRedrawWindow(this.frmMain.Handle);

			this.InitializeCBController();

			if(s_isDevEnv)  // Loading a base form within a designer
			{
				// Load the state set in the initialization code and save this to the memstream.
				// This stream is used for loading the default designer state.
				this.SaveToStream(this.strmDefault);
				this.strmDefault.Seek(0, SeekOrigin.Begin);

				if(this.bPersistState == true)
					this.LoadCommandBarState();
			}

			this.ResetDockBarZOrder();

			MethodInvoker mi = new MethodInvoker( ResumeRedrawMainFormAsync );

			this.frmMain.BeginInvoke( mi );
		}

		private void ResumeRedrawMainFormAsync()
		{
			Syncfusion.Runtime.InteropServices.NativeMethodsHelper.ResumeRedrawWindow( this.frmMain.Handle, false );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostForm_SystemColorsChanged(object sender, EventArgs e)
		{
			CommandBar.InitializeFloatCaptionFont();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostForm_BackColorChanged(object sender, EventArgs e)
		{
			if(this.bBackColorSet == false)
				this.clrBackColor = this.frmMain.BackColor;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostForm_ControlAdded(object sender, ControlEventArgs e)
		{
			if((e.Control != null) && ((e.Control is CommandDockBar)==false))
			{
				if(e.Control.Dock != DockStyle.None)
					this.ResetDockBarZOrder();
				e.Control.DockChanged += new EventHandler(this.ChildControl_DockChanged);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostForm_ControlRemoved(object sender, ControlEventArgs e)
		{
			if((e.Control != null) && ((e.Control is CommandDockBar)==false))
				e.Control.DockChanged -= new EventHandler(this.ChildControl_DockChanged);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void ChildControl_DockChanged(object sender, EventArgs e)
		{
			Control ctrl = sender as Control;
			if((ctrl != null) && (ctrl.Dock != DockStyle.None))
				this.ResetDockBarZOrder();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostForm_RightToLeftChanged( object sender, EventArgs e )
		{
			RightToLeft eRTL = this.RightToLeft;

			this.CommandDockBarL.RightToLeft = eRTL;
			this.CommandDockBarT.RightToLeft = eRTL;
			this.CommandDockBarR.RightToLeft = eRTL;
			this.CommandDockBarB.RightToLeft = eRTL;

			foreach( CommandBar cmdBar in this.alCommandBars )
			{
				cmdBar.RightToLeft = eRTL;
				cmdBar.Invalidate(true);
				//Removed this support in ParentBarItem
				//cmdBar.PopupMenu.ParentBarItem.RightToLeft = eRTL;
				RecalcLayout(cmdBar);
			}
		}

		/// <summary>
		/// Gets the list of commandBars in the commandBars	Collection.
		/// </summary>
		/// <returns></returns>
		public virtual CommandBarController.CommandBarsCollection GetCommandBarsList()
		{
			return this.alCommandBars;
		}

		// Private implementation of the ICBControllerDesignerInvoke interface
		CommandDockBar ICBControllerDesignerInvoke.GetCommandDockBarL()
		{
			return this.CommandDockBarL;
		}

		CommandDockBar ICBControllerDesignerInvoke.GetCommandDockBarT()
		{
			return this.CommandDockBarT;
		}

		CommandDockBar ICBControllerDesignerInvoke.GetCommandDockBarR()
		{
			return this.CommandDockBarR;
		}

		CommandDockBar ICBControllerDesignerInvoke.GetCommandDockBarB()
		{
			return this.CommandDockBarB;
		}

		void ICBControllerDesignerInvoke.InitializeCBController()
		{
			this.InitializeCBController();
		}

		/// <summary>
		/// Overridden. See <see cref="System.ComponentModel.Component.Dispose(bool)"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				this.alFreezeAddBars.Clear();
				this.alFreezeRemoveBars.Clear();
				this.alFreezeRelayoutBars.Clear();
				this.alDelayLoadBars.Clear();
				// Dispose off all the CommandBars
				foreach(CommandBar cbar in this.alCommandBars)
				{
					if(cbar.Parent != null)	// Docked or floating visible CommandBars
					{
						if(cbar.Floating == true)
						{
							CommandBarForm cbarform = cbar.Parent as CommandBarForm;
							cbarform.Visible = false;
							cbarform.Controls.Remove(cbar);
							cbarform.Close();
						}
						else
						{
							if(cbar.CommandBarBaseVisible == true)
							{
								cbar.CommandBarBaseVisible = false;
								cbar.cdbParent.RemoveCommandBar(cbar);
							}
							else
							{
								cbar.cdbParent.Controls.Remove(cbar);
							}
						}
					}
					cbar.Dispose();
				}
				this.alCommandBars.Clear();

				// Dispose DockBars
				if(this.CommandDockBarL != null)
				{
					this.frmMain.Controls.Remove(this.CommandDockBarL);
					this.CommandDockBarL.Visible = false;
					this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarL, false));
					this.CommandDockBarL.Dispose();
					this.CommandDockBarL = null;
				}
				if(this.CommandDockBarB != null)
				{
					this.frmMain.Controls.Remove(this.CommandDockBarB);
					this.CommandDockBarB.Visible = false;
					this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarB, false));
					this.CommandDockBarB.Dispose();
					this.CommandDockBarB = null;
				}
				if(this.CommandDockBarR != null)
				{
					this.frmMain.Controls.Remove(this.CommandDockBarR);
					this.CommandDockBarR.Visible = false;
					this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarR, false));
					this.CommandDockBarR.Dispose();
					this.CommandDockBarR = null;
				}
				if(this.CommandDockBarT != null)
				{
					this.frmMain.Controls.Remove(this.CommandDockBarT);
					this.CommandDockBarT.Visible = false;
					this.OnDockBarStateChanged(new DockBarStateEventArgs(this.CommandDockBarT, false));
					this.CommandDockBarT.Dispose();
					this.CommandDockBarT = null;
				}

				if(this.frmMain != null)
				{
					foreach(Control ctrl in this.frmMain.Controls)
					{
						if((ctrl is CommandDockBar) == false)
						{
							ctrl.DockChanged -= new EventHandler(this.ChildControl_DockChanged);
						}
					}
					this.frmMain.ControlAdded -= new ControlEventHandler(this.HostForm_ControlAdded);
					this.frmMain.ControlRemoved -= new ControlEventHandler(this.HostForm_ControlRemoved);
					this.frmMain.RightToLeftChanged -= new EventHandler(this.HostForm_RightToLeftChanged);

					if(this.DesignMode == false)
					{
						this.frmMain.Load -= new EventHandler(this.HostForm_Load);
						this.frmMain.Closing -= new CancelEventHandler(this.HostForm_Closing);
						this.frmMain.SystemColorsChanged -= new EventHandler(this.HostForm_SystemColorsChanged);
					}
					this.frmMain.BackColorChanged -= new EventHandler(this.HostForm_BackColorChanged);
					this.frmMain = null;
				}

				this.strmDefault.Close();
			}

			if(this.wndMainFrm != null)
			{
				// Not releasing, instead Deactivating. Look at Deactivate comments.
				//this.wndMainFrm.ReleaseHandleCustom();
				this.wndMainFrm.Deactivate();
                this.wndMainFrm = null;
			}
            
			base.Dispose(disposing);
		}
	}


	/// The CommandBar designer use this interface for communicating mousemove messages.
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface ICommandBarDesignerMouseHook
	{
		void HandleMouseDown(MouseButtons button, Point ptscreen);
		void HandleMouseMove(MouseButtons button, Point ptscreen);
		void HandleMouseUp(MouseButtons button, Point ptscreen);
		void HandleDoubleClick(Point ptscreen);
		void HandleMouseLeave();
	}
}
