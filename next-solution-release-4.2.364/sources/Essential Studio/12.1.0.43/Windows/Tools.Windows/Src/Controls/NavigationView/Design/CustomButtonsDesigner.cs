#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Navigation.Design
{
    internal class CustomButtonsDesigner :
        ControlDesigner
    {
        #region Fields

        private bool _newComponent = false;

        #endregion

        #region Overrides

        /// <summary>
        /// Initializes a newly created component.
        /// </summary>
        /// <param name="defaultValues">A name/value dictionary of default values to apply to properties. May be null if no default values are specified.</param>
        public override void InitializeNewComponent(System.Collections.IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);
            _newComponent = true;
        }

        /// <summary>
        /// Initializes the designer with the specified component.
        /// </summary>
        /// <param name="component">The <see cref="T:System.ComponentModel.IComponent"/> to associate the designer with. This component must always be an instance of, or derive from, <see cref="T:System.Windows.Forms.Control"/>.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            this.Control.ParentChanged += new System.EventHandler(OnControlParentChanged);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Design.ControlDesigner"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.Control.ParentChanged -= new System.EventHandler(OnControlParentChanged);

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the selection rules that indicate the movement capabilities of a component.
        /// </summary>
        /// <value></value>
        /// <returns>A bitwise combination of <see cref="T:System.Windows.Forms.Design.SelectionRules"/> values.</returns>
        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.BottomSizeable | SelectionRules.RightSizeable;
            }
        }

        #endregion

        #region Implementation

     private void OnControlParentChanged(object sender, System.EventArgs e)
        {
            if (_newComponent)
            {
                CustomButton btn = (CustomButton)this.Control;
                NavigationView nv = btn.Parent as NavigationView;

                if (nv != null)
                {
                    switch (nv.VisualStyle)
                    {
                        case VisualStyles.Office2007:
                            {
                                btn.Appearance = ButtonAppearance.Office2007;
                                btn.Office2007ColorScheme = nv.Office2007ColorTheme;

                                break;
                            }

                        case VisualStyles.Vista:
                            {
                                break;
                            }
                    }
                }
            }
        }

        #endregion
    }
}
