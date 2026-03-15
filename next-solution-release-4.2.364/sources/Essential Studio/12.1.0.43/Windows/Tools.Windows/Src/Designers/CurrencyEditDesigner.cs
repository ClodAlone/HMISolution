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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Drawing;
using System.Text;
using System.Reflection;
using Syncfusion.Windows.Forms.Design;
using System.Globalization;
#endif
namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// Extends design-time behavior for the <see cref="ButtonEdit"/> control.
	/// </summary>
	public class CurrencyEditDesigner : ControlDesigner, ICurrencyEditDesigner
    {
        #region Class members
        /// <summary>
        /// To prevent unserializable child control from getting serialized 
        /// </summary>
		private CurrencyEditCollectionSerializationProvider dummySerProvider;
        /// <summary>
        /// </summary>
		private CurrencyEditButtonsCollectionSerializationProvider dummyButtonsSerProvider;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the CurrencyEditDesigner class.
        /// </summary>
        public CurrencyEditDesigner()
        {
        }

        /// <summary>
		/// Prepares the designer to view, edit and design the specified component.
		/// Overrides ComponentDesigner.Initialize
		/// </summary>
		/// <param name="component">The component for this designer.</param>
		public override void Initialize(IComponent component)
		{
			base.Initialize( component );

			// Serialization listeners
			System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager =
				(System.ComponentModel.Design.Serialization.IDesignerSerializationManager)
				this.GetService(typeof(System.ComponentModel.Design.Serialization.IDesignerSerializationManager));

			if(manager != null)
			{
				dummySerProvider = new CurrencyEditCollectionSerializationProvider(this.Control.Controls, this);
				manager.AddSerializationProvider(this.dummySerProvider);
				dummyButtonsSerProvider = new CurrencyEditButtonsCollectionSerializationProvider(((CurrencyEdit)this.Control).Buttons, this);
				manager.AddSerializationProvider(this.dummyButtonsSerProvider);
			}         
		}

        protected override void Dispose( bool bdisposing )
        {
            base.Dispose(bdisposing);
        }
        #endregion

        #region Class overrides

       //SmartTags added for .NET Framework 2.0        
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        DesignerActionListCollection actionLists;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == actionLists)
                {
                    actionLists = new DesignerActionListCollection();
                    actionLists.Add(
                        new CurrencyEditActionList(this.Component));
                }
                return actionLists;
            }
        }
#endif

        protected override /*ParentControlDesigner*/ void OnPaintAdornments(PaintEventArgs pe)
		{
			if(this.Control != null)
			{
				System.Windows.Forms.ContainerControl targetControl;
				targetControl = (System.Windows.Forms.ContainerControl)this.Control;
				DrawingUtils.DrawDesignTimeBorder(pe.Graphics, targetControl);
			}
			base.OnPaintAdornments(pe);
		}

		/// <summary>
		/// Overrides PreFilterProperties and removes the properties 
		/// visible in the designer for the SplashControl.
		/// </summary>
		/// <param name="properties"></param>
		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);
		
			String[] strcolln = new String[3];
			strcolln[0] = "DataBindings";
			strcolln[1] = "Buttons";
			strcolln[2] = "ShowTextBox";
			CurrencyEditDesigner.RemovePropertyBrowsable(this.Control, strcolln, properties);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Helper function for removing a list of properties.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="strcolln"></param>
        /// <param name="properties"></param>
        static private void RemovePropertyBrowsable(Control control, String[] strcolln, IDictionary properties)
        {
            foreach (String property in strcolln)
            {
                PropertyDescriptor prop = (PropertyDescriptor)properties[property];
                if ((prop != null) && (prop.IsBrowsable == true))
                {
                    AttributeCollection mac = prop.Attributes;
                    bool bnondef = false;
                    foreach (Attribute mematt in mac)
                    {
                        // Is Browsable a default attribute? If so, break.
                        if (mematt as BrowsableAttribute != null)
                        {
                            bnondef = true;
                            break;
                        }
                    }
                    int ncount = (bnondef == true) ? mac.Count : mac.Count + 1;
                    Attribute[] arrmematt = new Attribute[ncount];
                    mac.CopyTo(arrmematt, 0);
                    if (bnondef == true)
                        arrmematt[Array.IndexOf(arrmematt, BrowsableAttribute.Yes)] = BrowsableAttribute.No;
                    else
                        arrmematt[ncount - 1] = BrowsableAttribute.No;
                    properties[property] = TypeDescriptor.CreateProperty(control.GetType(), prop, arrmematt);
                }
            }
        }

        public void RemoveUnserializableChildControls()
        {
            ButtonEdit buttonEdit = this.Control as ButtonEdit;
            if (buttonEdit != null)
            {
                ArrayList childControls = new ArrayList();
                for (int i = buttonEdit.Controls.Count - 1; i >= 0; i--)
                {
                    Control childControl = buttonEdit.Controls[i];
                    if (childControl != null && childControl is ButtonEditChildButton)
                    {
                        childControls.Add(childControl);
                        buttonEdit.Controls.Remove(childControl);
                    }
                }
                if (childControls.Count > 0)
                {
                    buttonEdit.ChildControlsRemovedByDesigner(childControls);
                }
            }
        }

        public void RemoveUnserializableButtons()
        {
            CurrencyEdit currencyEdit = this.Control as CurrencyEdit;
            if (currencyEdit != null)
            {
                ArrayList childButtons = new ArrayList();
                for (int i = currencyEdit.Buttons.Count - 1; i >= 0; i--)
                {
                    Control childButton = currencyEdit.Buttons[i];
                    if (childButton != null && (childButton is ButtonEditChildButton))
                    {
                        childButtons.Add(childButton);
                        currencyEdit.Buttons.Remove((ButtonEditChildButton)childButton);
                    }
                }
                if (childButtons.Count > 0)
                    currencyEdit.ChildButtonsRemovedByDesigner(childButtons);
            }
        }

        #endregion
    }


    #region Currency Edit ActionList
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

    public class CurrencyEditActionList : SyncActionListBase<CurrencyEdit>
    {

        public CurrencyEditActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - Currency Edit");

            //Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("TextAlign", "Text Alignment", "Appearance", "Horizontal text alignment for the control.");
            this.AddDesignerActionPropertyItem("CalculatorLayoutType", "Layout Type", "Appearance", "Specifies the Calculator Layout types.");
            this.AddDesignerActionPropertyItem("PopupCalculatorAlignment", "Popup Calculator Alignment", "Appearance", "Gets or sets the alignment of the Popup Calculator with respect to the ButtonEdit control.");
            if(this.UseVisualStyle)
                this.AddDesignerActionPropertyItem("ButtonStyle", "Button Styles", "Appearance", "Specifies the different styles of buttons");
            this.AddDesignerActionPropertyItem("UseVisualStyle", "Enable Visual styles", "Appearance", "Enables Visual Styles for buttons");

            //Appearance category.
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("TransferFromCalculator", "Transfer From Calculator", "Behavior", "Indicates whether to transfer the calculated value to the edit control.");
            this.AddDesignerActionPropertyItem("TransferToCalculator", "Transfer To Calculator", "Behavior", "Indicates whether to transfer the values from the edit control to the calculator.");
            this.AddDesignerActionPropertyItem("CloseAction", "Close Action", "Behavior", "Enables visual styles for the buttons");
            this.AddDesignerActionPropertyItem("DecimalValue", "DecimalValue", "Behavior", "Gets or sets the decimal value of the control. This will be formatted and displayed.");


        }
       
        public HorizontalAlignment TextAlign
        {
            get
            {
                HorizontalAlignment textAlign = HorizontalAlignment.Left;
                if (this.Control != null)
                {
                    CurrencyEdit control = this.Control as CurrencyEdit;
                    textAlign = control.TextAlign;
                }
                return textAlign;
            }
            set
            {
                SetValue("TextAlign", value);
            }

        }
        public CalculatorLayoutTypes CalculatorLayoutType
        {

            get
            {
                CalculatorLayoutTypes layoutType = CalculatorLayoutTypes.WindowsStandard;
                if (this.Control != null)
                {
                    CurrencyEdit control = this.Control as CurrencyEdit;
                    layoutType = control.CalculatorLayoutType;
                }
                return layoutType;
            }
            set
            {
                SetValue("CalculatorLayoutType", value);
            }

        }
        public CalculatorPopupAlignment PopupCalculatorAlignment
        {
            get
            {
                CalculatorPopupAlignment popupCalculatorAlignment = CalculatorPopupAlignment.Right;
                if (this.Control != null)
                {
                    CurrencyEdit control = this.Control as CurrencyEdit;
                    popupCalculatorAlignment = control.PopupCalculatorAlignment;
                }
                return popupCalculatorAlignment;
            }
            set
            {
                SetValue("PopupCalculatorAlignment", value);
            }
        }
        public ButtonAppearance ButtonStyle
        {

            get
            {
                ButtonAppearance appearance = ButtonAppearance.Classic;
                if (this.Control != null)
                {
                    CurrencyEdit control = this.Control as CurrencyEdit;
                    appearance = control.ButtonStyle;
                }
                return appearance;
            }
            set
            {
                SetValue( "ButtonStyle", value );
            }

        }
        public bool UseVisualStyle
        {

            get
            {
                bool visualStyle = false;
                if ( this.Control != null )
                {
                    CurrencyEdit control = this.Control as CurrencyEdit;
                    visualStyle = control.UseVisualStyle;
                }
                return visualStyle;
            }
            set
            {
                SetValue("UseVisualStyle", value);
            }

        }
        public CalcActions CloseAction
        {

            get
            {
                CalcActions closeAction = CalcActions.CalcOperatorEquals;
                if (this.Control != null)
                {
                    CurrencyEdit control = this.Control as CurrencyEdit;
                    closeAction = control.CloseAction;
                }
                return closeAction;
            }
            set
            {
                SetValue("CloseAction", value);
            }
        }
        public bool TransferFromCalculator
        {

            get
            {
                bool transferFromCalculator = true;
                if (this.Control != null)
                {
                    CurrencyEdit control = this.Control as CurrencyEdit;
                    transferFromCalculator = control.TransferFromCalculator;
                }
                return transferFromCalculator;
            }
            set
            {
                SetValue("TransferFromCalculator", value);
            }

        }
        public bool TransferToCalculator
        {

            get
            {
                bool transferToCalculator = true;
                if (this.Control != null)
                {
                    CurrencyEdit control = this.Control as CurrencyEdit;
                    transferToCalculator = control.TransferToCalculator;
                }
                return transferToCalculator;
            }
            set
            {
                SetValue("TransferToCalculator", value);
            }

        }
        public decimal DecimalValue
        {
            get
            {
                decimal decimalValue = 1.0M;
                if (this.Control != null)
                {
                    CurrencyEdit control = this.Control as CurrencyEdit;
                    decimalValue = control.DecimalValue;
                }
                return decimalValue;
            }
            set
            {
                SetValue("DecimalValue", value);
            }
        }
    }

#endif
    #endregion
}

