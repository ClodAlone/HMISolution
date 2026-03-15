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
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// Extends design-time behavior for the <see cref="ButtonEdit"/> control.
	/// </summary>
    public class ButtonEditDesigner : ParentControlDesigner
    {
        #region Class members
        /// <summary>
        /// Designer verb for adding a button.
        /// </summary>
		protected DesignerVerb dvAddButton = null;
		/// <summary>
		/// Designer verb for removing a button.
		/// </summary>
		protected DesignerVerb dvRemoveButton = null;
		/// <summary>
		/// The collection of designer verbs.
		/// </summary>
		protected DesignerVerbCollection dvcVerbs = null;
        /// <summary>
        /// Instance of ButtonEdit
        /// </summary>
        private ButtonEdit m_btn = null;
        /// <summary>
        /// Instance of TextBoxExt
        /// </summary>
        private TextBoxExt m_txtBox = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
		/// Initializes a new instance of the ButtonEditDesigner class.
		/// </summary>
		public ButtonEditDesigner()
		{
			this.dvAddButton = new DesignerVerb( "Add Button", new EventHandler( this.OnAddButton ) );
			this.dvRemoveButton = new DesignerVerb( "Remove Button", new EventHandler( this.OnRemoveButton ) );
			this.dvRemoveButton.Enabled = false;
			DesignerVerb[] dvarray = new DesignerVerb[] { this.dvAddButton, this.dvRemoveButton }; 
			this.dvcVerbs = new DesignerVerbCollection( dvarray );
        }

        /// <summary>
        /// Prepares the designer to view, edit and design the specified component.
        /// Overrides ComponentDesigner.Initialize.
        /// </summary>
        /// <param name="component">The component for this designer.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize( component );

            IComponentChangeService iccs = this.GetService( typeof( IComponentChangeService ) ) as IComponentChangeService;
            iccs.ComponentChanged += new ComponentChangedEventHandler( this.HandleComponentChanged );

            m_btn = this.Control as ButtonEdit;
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        public override void InitializeNewComponent( IDictionary defaultValues )
        { 
            CreateTextBox();
            m_btn.TextBox = m_txtBox;

            base.InitializeNewComponent( defaultValues );
        }
#else
		public override void OnSetComponentDefaults()
		{
			CreateTextBox();
			m_btn.TextBox = m_txtBox;

			base.OnSetComponentDefaults ();
		}
#endif

        protected override void Dispose( bool bdisposing )
        {
            IComponentChangeService iccs = this.GetService( typeof( IComponentChangeService ) ) as IComponentChangeService;
            if ( iccs != null )
            {
                iccs.ComponentChanged -= new ComponentChangedEventHandler( this.HandleComponentChanged );
            }

            base.Dispose( bdisposing );
        }  

        #endregion

        #region Class overrides

        /// <override/>
        protected override void OnDragDrop( DragEventArgs de )
        {
            TextBoxExt dragItem = GetSelectedControl() as TextBoxExt;
            if( dragItem != null )
            {
                m_btn.TextBox = dragItem;
                m_btn.Layout();
                base.OnDragDrop( de );
            }            
        }

        /// <summary>
		/// Returns the designer verbs collection.
		/// </summary>
		public override DesignerVerbCollection Verbs 
		{
			get
			{
				if(((ButtonEdit)(this.Control)).Buttons.Count > 0)
					this.dvRemoveButton.Enabled = true;
				else
					this.dvRemoveButton.Enabled = false;
				return this.dvcVerbs;	
			}
		}        


#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        System.ComponentModel.Design.DesignerActionListCollection actionLists;

        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if( null == actionLists )
                {
                    actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    actionLists.Add( new ButtonEditActionList( this.Component ) );
                }
                return actionLists;
            }
        }
#endif

        /// <summary>
		/// Returns the collection of components associated with the designer.
		/// </summary>
        public override ICollection AssociatedComponents 
		{ 
            get
            {
                ButtonEdit buttonEdit = this.Control as Syncfusion.Windows.Forms.Tools.ButtonEdit;
                if ( buttonEdit == null )
					return base.AssociatedComponents;
				else
                    return ( ( ButtonEdit ) ( this.Control ) ).Buttons;
            }
        }

		public override SelectionRules SelectionRules
		{
			get
			{
				return base.SelectionRules & ~(SelectionRules.BottomSizeable | SelectionRules.TopSizeable);
			}
		}
        #endregion

        #region Class event handlers

        /// <summary>
		/// Handles the ComponentChanged event of the IComponentChangeService.
		/// </summary>
		/// <param name="sender">The IComponentChange Service.</param>
		/// <param name="e">The event data.</param>
		public void HandleComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			if( this.Control != null )
			{
				ButtonEdit buttonEdit = this.Control as ButtonEdit;
				if( this.dvcVerbs != null && this.dvcVerbs.Count > 0 )
				{
					if( buttonEdit.Buttons.Count > 0 )
						this.dvRemoveButton.Enabled = true;
					else
						this.dvRemoveButton.Enabled = false;
				}

                if( buttonEdit.TextBox.Parent != buttonEdit && buttonEdit.TextBox != null )
                {
                    buttonEdit.DetachTextBox();
                    buttonEdit.SetTextBoxNull();
                }
			}
		}

		/// <summary>
		/// Handles the AddButton verb. 
		/// </summary>
		/// <param name="sender">The designer.</param>
		/// <param name="e">The event data.</param>
		public void OnAddButton(object sender, EventArgs e)
		{
			if( this.Control != null )
			{
				ButtonEdit buttonEdit = this.Control as ButtonEdit;
				IDesignerHost idh = this.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				ButtonEdit.ButtonEditChildButtonCollection btnlistold = buttonEdit.Buttons;			
				ButtonEditChildButton btn = ( ButtonEditChildButton ) idh.CreateComponent( typeof( ButtonEditChildButton ) );
                btn.Text = btn.Name;
				btn.ButtonType = ButtonTypes.Normal;
				buttonEdit.Buttons.Add( btn );
			
				this.RaiseComponentChanged( TypeDescriptor.GetProperties( buttonEdit )[ "Buttons" ], btnlistold, buttonEdit.Buttons );			
			}
		}

		/// <summary>
		/// Handles the RemoveButton verb.
		/// </summary>
		/// <param name="sender">The designer.</param>
		/// <param name="e">The event data.</param>
		public void OnRemoveButton(object sender, EventArgs e)
		{
			if( this.Control != null )
			{
				ButtonEdit buttonEdit = this.Control as ButtonEdit;
				IDesignerHost idh = this.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				ButtonEdit.ButtonEditChildButtonCollection btnlistold = buttonEdit.Buttons;			
				if( btnlistold.Count > 0 )
				{
					ButtonEditChildButton btn = btnlistold[ btnlistold.Count - 1 ];
					buttonEdit.Buttons.Remove( btn );
					idh.DestroyComponent( btn );

					this.RaiseComponentChanged( TypeDescriptor.GetProperties( buttonEdit )[ "Buttons" ], btnlistold, buttonEdit.Buttons );			
				}
			}
        }

        #endregion

        #region Class utility methods

        private void CreateTextBox()
		{
            TextBoxExt txtBox = null;
            IDesignerHost host = GetService( typeof( IDesignerHost ) ) as IDesignerHost;

            if ( host != null )
            {
                txtBox = host.CreateComponent( typeof( TextBoxExt ) ) as TextBoxExt;
                txtBox.BorderStyle = BorderStyle.None;
            }

            m_txtBox = txtBox;   
		}

        private Control GetSelectedControl()
        {
            ISelectionService service = base.GetService( typeof( ISelectionService ) ) as ISelectionService;
            if( service != null )
            {
                return service.PrimarySelection as Control;
            }

            return null;
        }

        #endregion
    }
}

