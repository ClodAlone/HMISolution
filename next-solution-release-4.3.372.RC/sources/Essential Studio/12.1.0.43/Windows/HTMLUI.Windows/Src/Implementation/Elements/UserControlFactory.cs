#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Windows.Forms;
using System.Xml;

using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Creates an instance of the class which represents Winform control.
    /// </summary>
    public sealed class UserControlFactory
    {
        #region Class constants
        /// <summary>
        /// Name of the user control.
        /// </summary>
        private const string BUTTON = "button";

        /// <summary>
        /// Name of the user control.
        /// </summary>
        private const string SUBMIT = "submit";

        /// <summary>
        /// Name of the user control.
        /// </summary>
        private const string RESET = "reset";

        /// <summary>
        /// Name of the user control.
        /// </summary>
        private const string RADIO = "radio";

        /// <summary>
        /// Name of the user control.
        /// </summary>
        private const string CHECKBOX = "checkbox";

        /// <summary>
        /// Name of the user control.
        /// </summary>
        private const string TEXT = "text";

        /// <summary>
        /// Name of the user control.
        /// </summary>
        private const string PASSWORD = "password";

        /// <summary>
        /// Name of the user control.
        /// </summary>
        private const string HIDDEN = "hidden";
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the UserControlFactory class from being created
        /// </summary>
        private UserControlFactory()
        {
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Creates the corresponding Winforms control from the tag element.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <returns>User control element.</returns>
        public static IControlImpl CreateUserControl(IUserControlHolder parent)
        {
            if (parent == null)
                throw new ArgumentNullException("storage");

            IHTMLElement tagParent = parent as IHTMLElement;
            HTMLAttributeImpl attr = null;
            UserControlImpl result = null;

            if (tagParent != null)
            {
                attr = (HTMLAttributeImpl)tagParent.Attributes[AttributeName.Type];
            }

            if (attr == null)
            {
                if (parent is INPUTElementImpl)
                {
                    result = new TEXTControlImpl(parent, false);
                }
                else if (parent is TEXTAREAElementImpl)
                {
                    result = new TEXTAREAControlImpl(parent);
                }
                else if (parent is SELECTElementImpl)
                {
                    result = new SELECTControlImpl(parent);
                }
                else
                {
                    result = new TEXTControlImpl(parent, false);
                }
            }
            else
            {
                switch (attr.Value.ToLower())
                {
                    case RESET:
                    case SUBMIT:
                    case BUTTON: 
                    result = new BUTTONControlImpl(parent); 
                    break;
                    case RADIO: 
                        result = new RADIOControlImpl(parent);
                        break;
                    case CHECKBOX:
                        result = new CHECKBOXControlImpl(parent); 
                        break;
                    case PASSWORD:
                        result = new TEXTControlImpl(parent, true); 
                        break;
                    case HIDDEN: 
                        result = new TEXTControlImpl(parent, false, false);
                        break;
                    case TEXT:
                    default:
                    result = new TEXTControlImpl(parent, false); 
                    break;
                }
            }

            return result as IControlImpl;
        }
        #endregion
    }
}
