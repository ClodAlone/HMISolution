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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class SelectedCardConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override StandardValuesCollection GetStandardValues(
			ITypeDescriptorContext context)
		{
			ArrayList values = new ArrayList();
			object instance = context.Instance;
			CardLayout layout = instance as CardLayout;
			if(layout != null)
				values = layout.GetCardNames();
			if(values.Count == 0)
				values.Add(String.Empty);
			return new StandardValuesCollection(values);
		}
		public override bool GetStandardValuesExclusive(	
			ITypeDescriptorContext context	)
		{
			return true;
		}
	}

	/// <summary>
	/// Specifies how the children will be laid out.
	/// </summary>
	/// <remarks>
	/// <para>In Default mode, the CardLayout manager will center the control within the 
	/// layout rectangle based on its preferred size, when there is enough space available. 
	/// When the space available is less than the preferred size, it will simply show the 
	/// child from the top / left of the layout rectangle, shrinking the size to fit the 
	/// layout rectangle, not going below the minimum size.</para>
	/// <para>In Fill mode, the preferred size of the control will be ignored and it will
	/// be made to fill the entire client area of the parent, taking into consideration the
	/// parent's DockPading parent(if the parent is a ScrollableControl).</para>
	/// </remarks>
	public enum CardLayoutMode
	{
		/// <summary>
		/// The child control is laid out based on its preferred size.
		/// </summary>
		Default,
		/// <summary>
		/// The child control is laid out to fill the parent.
		/// </summary>
		Fill
	}

	/// <summary>
	/// Represents the layout manager that lays out the children as "Cards".
	/// </summary>
	/// <remarks>
	/// <para>Each child component is a "Card" with a name attached to it.
	/// The CardLayout will display only one Card at a time, allowing you
	/// to flip through it. Use the <see cref="First"/>, <see cref="Last"/>, <see cref="Next"/>, <see cref="Previous"/>, and <see cref="Show"/> methods to do so.</para>
	/// <para>The <see cref="LayoutMode"/> property lets you specify whether to lay out the children based on their
	/// preferred size or make them fill the parent's client rectangle.</para>
	/// <para>The <see cref="SetCardName"/> method will expect you to pass a string value
	/// indicating the name of the Card as the constraint.</para>
	/// <para>Setting the same Card name for more than one component will result in unforseen conflicts.</para>
	/// <para>During design-time, you can change the order of the child controls being laid out by moving them around
	/// using the "Bring to Front" and "Send to Back" verbs provided by the control designer.</para>
	/// <para>Take a look at the LayoutManager class documentation for more information on
	/// LayoutManagers in general.</para>
	/// </remarks>
	/// <example>
	/// <para>Here is some sample code that tells you how to initialize a CardLayout manager:</para>
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\cs\CardLayoutForm.cs" name="Initializing CardLayout" lang="C#"><code lang="C#">
	///			// Binding a Control to the CardLayout manager programmatically.
	///			this.cardLayout1 = new CardLayout();
	///		
	///			// Set the container control; all the child controls of this container control are
	///			// automatically registered as children with the manager and get default card names.
	///			this.cardLayout1.ContainerControl = this.panel1;
	///			// Set custom card names to replace default card names.
	///			this.cardLayout1.SetCardName(this.label1, "MyCard1");
	///
	///			// To select a card manually, use the SelectedCard property.
	///			this.cardLayout1.SelectedCard = "MyCard1";
	///			
	///			// Or move through the cards like this:
	///			this.cardLayout1.Next();
	///			this.cardLayout1.Previous();</code></coderef>
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\VB\CardLayoutForm.vb" name="Initializing CardLayout" lang="VB"><code lang="VB">
	///            ' Binding a Control to the CardLayout manager programmatically.
	///            Me.cardLayout1 = New CardLayout
	///            ' Set the target control; all the child controls of this target control are
	///            ' automatically registered as children with the manager and get default card names.
	///            Me.cardLayout1.ContainerControl = Me.panel1
	///            ' Set custom card names to replace default card names.
	///            Me.cardLayout1.SetCardName(Me.label1, "MyCard1")
	///            ' To select a card manually, use the SelectedCard property.
	///            Me.cardLayout1.SelectedCard = "MyCard1"
	///            ' Or move through the cards like this:
	///            Me.cardLayout1.Next
	///            Me.cardLayout1.Previous</code></coderef>
	/// <para>Also, take a look at the project in Tools/Samples/Quick Start/LayoutManagers for an example.</para>
	/// </example>
	[
	Designer(
		typeof(Syncfusion.Windows.Forms.Tools.Design.CardLayoutDesigner),
		typeof(System.ComponentModel.Design.IDesigner)),
	ProvideProperty("CardName", typeof(Control)),
	ProvideProperty("MaintainAspectRatio",typeof(Control)),
	// PopupControlContainer has the same namespace as the default assembly namespace
	System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.CardLayout.bmp"),
	ToolboxItemFilter("System.Windows.Forms"),
	Description("Represents the layout manager that lays out the children as Cards.")
	]
	public class CardLayout : LayoutManager
	{
		private Hashtable cardNameVsControls;
		private Hashtable controlsVsCardNames;
		private Hashtable maintainAspectRatios;		
		private CardLayoutMode cardLayoutMode = CardLayoutMode.Default;
		/// <summary>
		/// Indicates the Base Name of the card.
		/// </summary>
		protected string CardNameBase = "Card";
		private Control selectedControl = null;
		private string cachedSelectedCardSetting = String.Empty;

		/// <summary>
		/// Overloaded. Creates a new instance of the CardLayout class and sets its defaults.
		/// </summary>
		public CardLayout()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
				new Syncfusion.Core.Licensing.LicensedComponent(typeof(CardLayout));
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			this.cardNameVsControls = new Hashtable();
			this.controlsVsCardNames = new Hashtable();
			this.maintainAspectRatios = new Hashtable();
		}
		/// <summary>
		/// Creates a new instance of the CardLayout class and adds itself to the specified container.
		/// </summary>
		/// <param name="container">The logical ContainerControl parent into which to add itself.</param>
		/// <remarks><para>This constructor is used by the design-time to add a component to the form's
		/// IContainer field so that it gets Disposed when the form gets Disposed.</para>
		/// <para>Note that this is not the same as the layout manager's container control.</para></remarks>
		public CardLayout(IContainer container)
			: this()
		{
			if(container != null)
				container.Add(this);
		}
		/// <summary>
		/// Creates a new instance of the CardLayout class and sets its ContainerControl.
		/// </summary>
		public CardLayout(Control container)
			:this()
		{
			ContainerControl = container;
		}

		/// <summary>
        /// Ends designer initialization.
		/// </summary>
		public override void EndInit()
		{
			if(this.cachedSelectedCardSetting.Length > 0)
				this.SelectedCard = this.cachedSelectedCardSetting;

			this.cachedSelectedCardSetting = String.Empty;

			this.ValidateHiddenStates();
			base.EndInit();
		}

		#region Properties

		/// <summary>
		/// Gets or sets the layout mode.
		/// </summary>
		/// <value>The current CardLayoutMode. Default is CardLayoutMode.Default.</value>
		[DefaultValue(CardLayoutMode.Default),
		Localizable(true),
		Category("Behavior"),
		Description("Indicates the layout mode.")
		]
		public CardLayoutMode LayoutMode
		{
			get{return this.cardLayoutMode;}
			set
			{
				if(this.cardLayoutMode != value)
				{
					this.cardLayoutMode = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal Control SelectedControl
		{
			get
			{
				return selectedControl;
			}
			set
			{
				if (selectedControl != value)
				{
					if (selectedControl != null)
					{
						selectedControl.Visible = false;
					}
					
					selectedControl = value;

					if (selectedControl != null)
					{
						selectedControl.Visible = true;
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// Returns the Card name of a child component.
		/// </summary>
		/// <param name="control">The child component whose Card name is to be retrieved.</param>
		/// <returns>The Card name as string.</returns>
		[Category("Layout Manager"),
		Localizable(true),
		MergableProperty(false)]
		public string GetCardName(Control control)
		{
			if(controlsVsCardNames[control] == null)
				return String.Empty;	// This should never happen
			else
				return (string)controlsVsCardNames[control];
		}
		/// <summary>
		/// Sets the Card name for a child component.
		/// </summary>
		/// <param name="control">The child component whose Card name is to be set.</param>
		/// <param name="value">The Card name as string.</param>
		/// <remarks>Use this method to change the card name of a component
		/// once set using <see cref="AddLayoutComponent"/>. Setting a NULL or empty string
		/// will also remove the component from the layout list.</remarks>
		[MergableProperty(false)]
		public void SetCardName(Control control, string value)
		{
			// This means the user has copied a child control and pasted it somewhere on the form:
			// as opposed to the designer loading.
			if(this.DesignMode && this.LoadingDocument && this.DesignerInTransaction
				// Sometimes, at the end of load the designer starts a transaction. This check will exclude that case.
				&& this.ContainerControl != null)
			{
				// Then make sure not to honor this (or else this will result in 
				// multiple controls having the same card name.
				// Instead the card name will be set in OnControlAdded.
				if(!this.ContainerControl.Contains(control))
					return;
			}
			if(value == null || value.Length == 0)
				this.RemoveLayoutComponent(control);
			else
				AddLayoutComponent(control, value);
		}

		/// <summary>
		/// Returns the value for maintaining aspect ratio based on the control's PreferredSize.
		/// </summary>
		/// <param name="control">The control whose aspect ratio setting is to be known.</param>
		/// <returns>True if the aspect ratio should be maintained; false otherwise.</returns>
		[Category("Layout Manager"),
		MergableProperty(false),
		DefaultValue(false)]
		public virtual bool GetMaintainAspectRatio(Control control)
		{
			if(maintainAspectRatios[control] == null)
				SetMaintainAspectRatio(control, false);
			
			return (bool)maintainAspectRatios[control];
		}

		/// <summary>
		/// Sets the value for maintaining aspect ratio based on the control's PreferredSize.
		/// </summary>
		/// <param name="control">The control to associate the aspect ratio setting.</param>
		/// <param name="value">True to maintain aspect ratio. False otherwise.</param>
		/// <remarks>Use this method to specify whether or not the control will be drawn
		/// maintaining its aspect ratio based on the control's PreferredSize. Applicable only
		/// when the <see cref="LayoutMode"/> property is set to CardLayoutMode.Default.</remarks>
		public virtual void SetMaintainAspectRatio(Control control, bool value)
		{
			if(maintainAspectRatios[control] == null
				|| !((bool)maintainAspectRatios[control]).Equals(value))
			{
				maintainAspectRatios[control] = value;
				if(this.ContainerControl != null)
					this.ContainerControl.PerformLayout();
			}
		}
		/// <summary>
		/// Overridden. See <see cref="LayoutManager.OnControlAdded"/>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnControlAdded(object sender, ControlEventArgs e)
		{
			Control child = e.Control;
			if(controlsVsCardNames[child] == null)
			{
				string newName = this.GetNewCardName();
				SetCardName(child, newName);
			}
			base.OnControlAdded(sender, e);
		}
		/// <summary>
		/// Generates a new unique name for a card that could be added to this CardLayout.
		/// </summary>
		/// <returns>A card name unique within this layout manager.</returns>
		public virtual string GetNewCardName()
		{
			ArrayList currentCardNames = this.GetCardNames();
			string newName = String.Empty;
			int suggestedIndexSuffix = ((controlsVsCardNames.Count));
			while(newName.Length == 0 || currentCardNames.Contains(newName))
			{
				suggestedIndexSuffix++;
				newName = CardNameBase + suggestedIndexSuffix.ToString();
			}
			return newName;
		}
		/// <summary>
		/// Overridden. See <see cref="LayoutManager.OnContainerControlChanged"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnContainerControlChanged(EventArgs e)
		{
			// Make sure to call base class first.
			base.OnContainerControlChanged(e);

			if(this.ContainerControl != null && !this.LoadingDocument)
			{
				// Make all the child controls participate in layout management by default.
				foreach(Control control in this.ContainerControl.Controls)
				{
					if(controlsVsCardNames[control] == null)
					{
						string name = CardNameBase + ((controlsVsCardNames.Count)+1).ToString();
						SetCardName(control, name);
					}
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="LayoutManager.AddLayoutComponent"/>.
		/// </summary>
		/// <param name="childControl"></param>
		/// <param name="constraints"></param>
		public override void AddLayoutComponent(Control childControl, object constraints)
		{
			if(constraints == null || ((string)constraints).Length == 0)
				throw new ArgumentException("Constraints cannot be NULL or an empty string when calling AddLayoutComponent", "constraints");
			if(constraints is string)
			{
				if(this.ContainerControl != null)
					this.ContainerControl.SuspendLayout();

				bool isSelectedControl = this.selectedControl == childControl;

				string name = (string)constraints;
				// This will happen when initializing (replacing default with custom card names).
				// In that case skip this check.
				if(!this.initializing && cardNameVsControls[name] != null
					&& cardNameVsControls[name] != childControl)
				{
					// A control already exists with this name.
					if(controlsVsCardNames[childControl] != null)
						// Revert back to the old name.
						name = (string)controlsVsCardNames[childControl];
					else
						// Create a new name.
						name = this.GetNewCardName();
				}

				// Check if an entry for this control already exists.
				RemoveLayoutComponent(childControl);

				if(cardNameVsControls.Count > 1)
					childControl.Visible = false;

				//Hashtable 1
				cardNameVsControls[name] = childControl;
				// Hashtable 2
				controlsVsCardNames[childControl] = name;

				base.AddLayoutComponent(childControl, constraints);

				// RemoveLayoutComponent above might have affected selectedControl.
				if (isSelectedControl)
				{
					this.SelectedControl = childControl;
				}

				if(this.ContainerControl != null)
				{
					this.ContainerControl.ResumeLayout(false);
					this.ContainerControl.PerformLayout(childControl, "Bounds");
				}
			}
			return;
		}

		/// <summary>
		/// Overridden. See <see cref="LayoutManager.RemoveLayoutComponent"/>.
		/// </summary>
		/// <param name="childControl"></param>
		public override void RemoveLayoutComponent(Control childControl)
		{
			if( null != childControl && !childControl.IsDisposed )
			{
				// Hashtable 1
				IEnumerator valueEnumerator = cardNameVsControls.Values.GetEnumerator();
				IEnumerator keyEnumerator = cardNameVsControls.Keys.GetEnumerator();
				Control childParsed;
				while(valueEnumerator.MoveNext())
				{
					keyEnumerator.MoveNext();
					childParsed = (Control)valueEnumerator.Current;
					if(childParsed == childControl)
					{
						// Remove existing entry
						cardNameVsControls.Remove(keyEnumerator.Current);
						break;
					}
				}
				// Hashtable 2
				controlsVsCardNames.Remove(childControl);

				childControl.Visible = true;

				if(this.selectedControl == childControl)
				{
					this.Next();

					if(this.selectedControl == childControl)
					{
						// Still cannot change this, so just remove selection.
						this.SelectedCard = String.Empty;
					}
				}

				base.RemoveLayoutComponent(childControl);
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.ResetLayoutInfo"/>.
		/// </summary>
		protected override void ResetLayoutInfo()
		{
			// Unhide the controls when removing layout.
			if(this.ContainerControl != null)
				this.ContainerControl.SuspendLayout();
			foreach(Control control in this.GetControls())
				control.Visible = true;
			if(this.ContainerControl != null)
				this.ContainerControl.ResumeLayout(false);

			base.ResetLayoutInfo();
			// Also remove the child control cache.
			this.cardNameVsControls.Clear();
			this.controlsVsCardNames.Clear();
			this.selectedControl = null;
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.PreferredLayoutSize"/>.
		/// </summary>
		public override Size PreferredLayoutSize()
		{
			Monitor.Enter(this);
			IList controls = this.GetControls();
			int nMembers = controls.Count;
			int w = 0;
			int h = 0;

			for (int i = 0 ; i < nMembers ; i++) 
			{
				Control child = controls[i]  as Control;
				Size s = GetPreferredSize(child);
				if (s.Width > w) 
				{
					w = s.Width;
				}
				if (s.Height > h) 
				{
					h = s.Height;
				}
			}
			Monitor.Exit(this);
			return new Size(this.AdjustWidthForMargins(w), this.AdjustHeightForMargins(h));
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.MinimumLayoutSize"/>.
		/// </summary>
		public override Size MinimumLayoutSize()
		{
			Monitor.Enter(this);
			IList controls = this.GetControls();
			int nMembers = controls.Count;
			int w = 0;
			int h = 0;

			for (int i = 0 ; i < nMembers ; i++) 
			{
				Control child = controls[i] as Control;
				Size s = GetMinimumSize(child);
				if (s.Width > w) 
				{
					w = s.Width;
				}
				if (s.Height > h) 
				{
					h = s.Height;
				}
			}
			Monitor.Exit(this);
			return new Size(w, h);
		}

        /// <summary>
        /// Validates hidden states.
        /// </summary>
		[Documentation.DocumentationExclude()]
		public void ValidateHiddenStates()
		{
			// Show only the selectedCard.

			IList controls = this.GetControls();
			foreach(Control child in controls)
			{
				if(child != this.selectedControl)
					child.Visible = false;
			}
			if(this.selectedControl != null)
				this.selectedControl.Visible = true;
		}
		// Make sure one and only one control is visible.
		//		[Syncfusion.Documentation.DocumentationExclude()]
		//		public void ValidateHiddenStates()
		//		{
		//			Monitor.Enter(this);
		//			if(this.ContainerControl != null)
		//				this.ContainerControl.SuspendLayout();
		//
		//			IList controls = this.GetControls();
		//
		//			// Find out if there is more than one control visible.
		//			int visibleControls = 0;
		//			foreach(Control control in controls)
		//			{
		//				if(this.IsVisible(control))
		//				{
		//					visibleControls++;
		//					if(visibleControls > 1)
		//						break;
		//				}
		//			}
		//			if(visibleControls > 1)
		//			{
		//				bool visibleControlFound = false;
		//				for(int i = 0; i < controls.Count; i++)
		//				{
		//					// If more than 1 control is Visible, Keep the first such control Visible and hide the rest
		//					Control control = controls[i] as Control;
		//					if(this.IsVisible(control))
		//					{
		//						if(!visibleControlFound)
		//							visibleControlFound = true;
		//						else
		//							control.Visible = false;
		//					}
		//				}
		//			}
		//			else if (visibleControls == 0)
		//			{
		//				if(controls.Count > 1)
		//					((Control)controls[0]).Visible = true;
		//			}
		//			if(this.ContainerControl != null)
		//				this.ContainerControl.ResumeLayout(true);
		//			Monitor.Exit(this);
		//		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.LayoutContainer"/>.
		/// </summary>
		public override void LayoutContainer()
		{
			if(!IsInit())
				return;
			Monitor.Enter(this);
			if(this.ContainerControl != null)
				this.ContainerControl.SuspendLayout();

			IList controls = this.GetControls();

			if(this.SelectedCard!=null && this.SelectedCard.Length == 0 && controls.Count > 0)
				this.SelectedCard = (string)this.controlsVsCardNames[controls[0]];
			

			int nMembers = controls.Count;
			for (int i = 0 ; i < nMembers ; i++) 
			{
				Control child = controls[i]  as Control;
				// Not checking for visibility since in CardLayout mode,
				// only one card is visible anyway.
			{
				Rectangle containerBounds = this.GetBounds();
				Rectangle newBounds = new Rectangle(containerBounds.Top, containerBounds.Left, 0, 0);
				Size sizeForChild = new Size(containerBounds.Width, containerBounds.Height);

				if(this.LayoutMode == CardLayoutMode.Default)
				{
					Size prefChildSize = this.GetPreferredSize(child);
					Size minChildSize = this.GetMinimumSize(child);

					float aspectRatio = (float)prefChildSize.Width / (float)prefChildSize.Height;

					if(sizeForChild.Width >= prefChildSize.Width)
					{
						newBounds.X += (sizeForChild.Width - prefChildSize.Width)/2;
						newBounds.Width = prefChildSize.Width;
					}
					else if(sizeForChild.Width < prefChildSize.Width
						&& sizeForChild.Width > minChildSize.Width)
						newBounds.Width = sizeForChild.Width;
					else
						newBounds.Width = minChildSize.Width;

					if(sizeForChild.Height >= prefChildSize.Height)
					{
						newBounds.Y += (sizeForChild.Height - prefChildSize.Height)/2;
						newBounds.Height = prefChildSize.Height;
					}
					else if(sizeForChild.Height < prefChildSize.Height
						&& sizeForChild.Height > minChildSize.Height)
						newBounds.Height = sizeForChild.Height;
					else
						newBounds.Height = minChildSize.Height;

					if(GetMaintainAspectRatio(child))
					{
						float tempWidth = newBounds.Width;
						float tempHeight = newBounds.Height;

						float ratioHeight = tempWidth / aspectRatio;
						float ratioWidth = tempHeight * aspectRatio;

						if(ratioHeight < tempHeight)
						{
							newBounds.Height = (int)ratioHeight;
							newBounds.Y = (sizeForChild.Height - (int)ratioHeight)/2;
						}
						else if(ratioWidth < tempWidth)
						{
							newBounds.Width = (int)ratioWidth;
							newBounds.X = (sizeForChild.Width - (int)ratioWidth)/2;
						}
					}
				}
				else
				{
					newBounds.Width = sizeForChild.Width;
					newBounds.Height = sizeForChild.Height;

					//this.ApplyDockPadding(ref newBounds, child);
				}

				// Raise RestoreChildPosition event
				CancelEventArgs evtArgs = new CancelEventArgs();
				OnRestoreChildPosition( evtArgs );

				if( !evtArgs.Cancel && ( child.Bounds != newBounds ) )
				{
					child.Bounds = newBounds;
					child.PerformLayout();
				}
			}
			}
			if(this.ContainerControl != null)
				this.ContainerControl.ResumeLayout(true);

			Monitor.Exit(this);
		}

		protected virtual void OnRestoreChildPosition( CancelEventArgs e)
		{
			if( this.RestoreChildPosition != null )
				this.RestoreChildPosition( this, e );
		}

		/// <summary>
		/// Returns the current active Card.
		/// </summary>
		/// <returns>The control representing the card.</returns>
		protected Control GetCurrentVisibleChild()
		{
			IList controls = this.GetControls();
			int nMembers = controls.Count;
			for (int i = 0 ; i < nMembers ; i++) 
			{
				Control child = controls[i]  as Control;
				if (this.IsVisible(child)) 
					return child;
			}
			return null;
		}

		/// <summary>
		/// Shows the first Card.
		/// </summary>
		public void First() 
		{
			if(!IsInit())
				return;

			Monitor.Enter(this);
			
			IList controls = GetControls();
			if(controls.Count == 0)
				this.SelectedCard = String.Empty;
			else
			{
				Control firstChild = GetControls()[0] as Control;
				if(this.selectedControl != firstChild)
				{
					this.SelectedCard = (string)this.controlsVsCardNames[firstChild];
				}
			}
			
			Monitor.Exit(this);
		}

		/// <summary>
		/// Returns the index of the next card that will be shown when the <see cref="Next"/> method gets called.
		/// </summary>
		/// <value>A valid index into the list returned by the <see cref="LayoutManager.GetControls"/> method; -1 if 
		/// a next card is not available.</value>
		/// <remarks>This property will return the first card if the currently selected card is the last card.</remarks>
		[Description("Indicated the index of the next card that will be shown when the Next method gets called.")]
		public int NextCardIndex
		{
			get
			{
				if(!IsInit())
					return -1;

				IList controls = this.GetControls();
				int nCurVisibleChildIndex = this.selectedControl == null ?
					-1 : controls.IndexOf(this.selectedControl);
				int nNextIndex = (nCurVisibleChildIndex + 1 < controls.Count) ? nCurVisibleChildIndex + 1 : 0;

				return nNextIndex;
			}
		}
		/// <summary>
		/// Shows the next Card in the list.
		/// </summary>
		public void Next() 
		{
			if(!IsInit())
				return;

			Monitor.Enter(this);

			int nNextIndex = this.NextCardIndex;

			IList controls = this.GetControls();

			if(nNextIndex >= controls.Count)
				this.SelectedCard = String.Empty;
			else
			{
				Control nextChild = controls[nNextIndex] as Control;
			
				if(this.selectedControl != nextChild)
				{
					this.SelectedCard = (string)this.controlsVsCardNames[nextChild];
				}
			}
			Monitor.Exit(this);
		}

		/// <summary>
		/// Returns the index of the previous card that will be shown when the <see cref="Previous"/> method gets called.
		/// </summary>
		/// <value>A valid index into the list returned by the <see cref="LayoutManager.GetControls"/> method; -1 if 
		/// a previous card is not available.</value>
		/// <remarks>This property will return the last card if the currently selected card is the first card.</remarks>
		[Description("Indicates the index of the previous card that will be shown when the Previous method gets called.")]
		public int PreviousCardIndex
		{
			get
			{
				if(!IsInit())
					return -1;

				IList controls = this.GetControls();
				int nCurVisibleChildIndex = this.selectedControl == null ?
					-1 : controls.IndexOf(this.selectedControl);
				int nPrevIndex = (nCurVisibleChildIndex == 0) ? controls.Count - 1 : nCurVisibleChildIndex - 1;

				return nPrevIndex;
			}
		}
		/// <summary>
		/// Shows the previous Card in the list.
		/// </summary>
		public void Previous() 
		{
			if(!IsInit())
				return;

			Monitor.Enter(this);

			int nPrevIndex = this.PreviousCardIndex;

			IList controls = this.GetControls();

			if(nPrevIndex >= controls.Count)
				this.SelectedCard = String.Empty;
			else
			{
				Control prevChild = controls[nPrevIndex] as Control;

				if(this.selectedControl != prevChild)
				{
					this.SelectedCard = (string)this.controlsVsCardNames[prevChild];
				}
			}
			Monitor.Exit(this);
		}

		/// <summary>
		/// Shows the last Card in the list.
		/// </summary>
		public void Last() 
		{
			if(!IsInit())
				return;

			Monitor.Enter(this);
			
			IList controls = this.GetControls();
			if(controls.Count == 0)
				this.SelectedCard = String.Empty;
			else
			{
				Control lastChild = controls[controls.Count - 1] as Control;
				if(this.selectedControl != lastChild)
				{
					this.SelectedCard = (string)this.controlsVsCardNames[lastChild];
				}
			}

			Monitor.Exit(this);
		}

		/// <summary>
		/// Shows a Card by name.
		/// </summary>
		/// <param name="cardName">The Card's name.</param>
		public void Show(String cardName) 
		{
			if(!IsInit())
				return;

			this.SelectedCard = cardName;
		}
		/// <summary>
		/// Gets or sets the current Card's name.
		/// </summary>
		/// <value>The current Card's name.</value>
		[
		Category("Appearance"),
		TypeConverter(typeof(SelectedCardConverter)),
		Localizable(true),
		Description("Specifies the current Card's name."),
		DefaultValue("")
		]
		public string SelectedCard 
		{
			get
			{
				if(this.selectedControl == null)
					return String.Empty;
				else
					return (string)this.controlsVsCardNames[this.selectedControl];
			}
			set 
			{
				if(this.initializing && this.cardNameVsControls[value] == null)
				{
					// Try again in EndInit.
					this.cachedSelectedCardSetting = value;
					return;
				}

				Control newControl = this.cardNameVsControls[value] as Control;
				if(this.selectedControl != newControl && 
					(value.Length == 0 || newControl != null))
				{
					if(this.ContainerControl != null)
						this.ContainerControl.SuspendLayout();
					
					if(this.selectedControl != null)
						this.selectedControl.Visible = false;
					
					this.selectedControl = newControl;

					this.ValidateHiddenStates();

					if(this.ContainerControl != null)
					{
						this.ContainerControl.ResumeLayout(false);
						if(this.selectedControl != null)
							this.ContainerControl.PerformLayout(this.selectedControl, "Visible");

						this.MakeDirty();
					}
				}
			}
		}
		/// <summary>
		/// Returns an array containing the Card names as strings.
		/// </summary>
		/// <returns>An ArrayList of Card names.</returns>
		public ArrayList GetCardNames()
		{
			ArrayList names = new ArrayList();
			//IEnumerator keyEnumerator = cardNameVsControls.Keys.GetEnumerator();
			IList childControls = this.GetControls();
			foreach(Control child in childControls)
			{
				names.Add((string)this.controlsVsCardNames[child]);
			}
			return names;
		}
		/// <summary>
		/// Returns an associated control given a Card name.
		/// </summary>
		/// <param name="cardName">The card name whose control to retrieve.</param>
		/// <returns>A control associated with the card name.</returns>
		public Control GetComponentFromName(string cardName)
		{
			return cardNameVsControls[cardName] as Control;
		}

		/// <summary>
		/// This event is raised in LayoutContainer method before applying new bounds.
		/// </summary>
		[Description("This event is fired in LayoutContainer method.")]
		[Category("CardLayout Events")]
		public event RestoreChildPositionEventHandler RestoreChildPosition;
		/// <summary>
		/// Represents the method that will handle the <see cref="CardLayout.RestoreChildPosition"/> event of
		/// the CardLayout.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
		public delegate void RestoreChildPositionEventHandler( object sender, CancelEventArgs e );
	}
}
