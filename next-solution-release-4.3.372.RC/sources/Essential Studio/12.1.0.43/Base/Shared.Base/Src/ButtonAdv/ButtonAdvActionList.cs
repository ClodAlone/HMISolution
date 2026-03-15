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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Design;
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Text;
using System.Reflection;
using System.Globalization;
#endif

namespace Syncfusion.Windows.Forms
{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	/// <summary></summary>
  public class ButtonAdvDesigner : ControlDesigner
	{
    #region Class members
    /// <summary></summary>
    private System.ComponentModel.Design.DesignerActionListCollection actionLists;
    #endregion

    #region Class properties
    /// <summary></summary>
    public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
    {
      get
      {
        if( null == actionLists )
        {
          actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
          actionLists.Add( new ButtonAdvActionList( this.Component ) );
        }

        return actionLists;
      }
    }
    #endregion

    #region Class Initialize/finilize methods
    /// <summary></summary>
    public ButtonAdvDesigner()
      : base()
    {
    }
    /// <summary></summary>
    /// <param name="component"></param>
    public override void Initialize( IComponent component )
    {
      base.Initialize( component );
    }
    #endregion

	}

    /// <summary></summary>
    public class ButtonAdvActionList : SyncActionListBase<ButtonAdv>
	{
		public ButtonAdvActionList( IComponent component )
			: base( component )
		{
		}

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ButtonAdv");

            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");
            this.AddDesignerActionPropertyItem("Text", "Text", "Design", "The text associated with the control.");

            // Specifying category.
            this.AddDesignerActionHeaderItem("Appearance");
            // Name, DisplayName, Category, ToolTip.
            this.AddDesignerActionPropertyItem("KeepFocusRectangle", "Keep Focus Rectangle", "Appearance", "Indicates whether ButtonAdv will show focus rectangle receiving focus.");
            this.AddDesignerActionPropertyItem("UseVisualStyle", "Use Visual Style", "Appearance", "Indicates whether Visual Styles must be enabled for the button.");
            this.AddDesignerActionPropertyItem("Appearance", "Style", "Appearance", " Gets or sets the look and feel of the ButtonAdv.");
            this.AddDesignerActionPropertyItem("ButtonType", "Button Type", "Appearance", "  Gets or sets the type of button to be used.");
            this.AddDesignerActionPropertyItem("Image", "Image ", "Appearance", " Gets or sets the  image that is displayed in the control.");
            this.AddDesignerActionPropertyItem("ImageAlign", "Image Align", "Appearance", " Gets or sets the alignment of an image that is displayed in the control.");
            this.AddDesignerActionPropertyItem("TextAlign", "Text Alignment", "Appearance", "Indicates the alignment of Text in the Gradient Label.");
        }

		public string Name
		{
			get
			{
				string name = string.Empty;
				if( this.Control != null )
				{
					ButtonAdv control = this.Control as ButtonAdv;
					name = control.Name;
				}
				return name;
			}
			set
			{
				SetValue( "Name", value );
			}
		}

		public bool UseVisualStyle
		{
			get
			{
                bool UseVisualStyle = false;
				
                if( this.Control != null )
				{
					ButtonAdv control = this.Control as ButtonAdv;
					UseVisualStyle = control.UseVisualStyle;
				}				
                return UseVisualStyle;
			}

			set
			{
				SetValue( "UseVisualStyle", value );
			}
		}

		public bool KeepFocusRectangle
		{
			get
			{
				bool m_bKeepFocusRectangle = false;
				if( this.Control != null )
				{
					ButtonAdv control = this.Control as ButtonAdv;
					m_bKeepFocusRectangle = control.KeepFocusRectangle;
				}
				return m_bKeepFocusRectangle;
			}
			set
			{
				SetValue( "KeepFocusRectangle", value );
			}
		}

      public ButtonAppearance Appearance
		{
			get
			{
                ButtonAppearance appearance = ButtonAppearance.Classic;
				if( this.Control != null )
				{
					ButtonAdv control = this.Control as ButtonAdv;
                    appearance = control.Appearance;
				}
                return appearance;
			}
			set
			{
                SetValue( "Appearance", value );
			}
		}

		public ButtonTypes ButtonType
		{
			get
			{
				ButtonTypes ButtonType = ButtonTypes.Normal;
				if( this.Control != null )
				{
					ButtonAdv control = this.Control as ButtonAdv;
					ButtonType = control.ButtonType;
				}
				return ButtonType;
			}
			set
			{
				SetValue( "ButtonType", value );
			}
		}

		public Image Image
		{
			get
			{
				Image bImage = null;
				if( this.Control != null )
				{
					ButtonAdv control = this.Control as ButtonAdv;
					bImage = control.Image;
				}
				return bImage;
			}
			set
			{
				SetValue( "Image", value );
			}
		}

		public ContentAlignment ImageAlign
		{
			get
			{
				ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
				if( this.Control != null )
				{
					ButtonAdv control = this.Control as ButtonAdv;
					imageAlign = control.ImageAlign;
				}
				return imageAlign;
			}
			set
			{
				SetValue( "ImageAlign", value );
			}
		}

		public string Text
		{
			get
			{
				string text = string.Empty;
				if( this.Control != null )
				{
					ButtonAdv control = this.Control as ButtonAdv;
					text = control.Text;
				}
				return text;
			}
			set
			{
				SetValue( "Text", value );
			}
		}

		public ContentAlignment TextAlign
		{
			get
			{
				ContentAlignment textAlign = ContentAlignment.MiddleLeft;
				if( this.Control != null )
				{
					ButtonAdv control = this.Control as ButtonAdv;
					textAlign = control.TextAlign;
				}
				return textAlign;
			}
			set
			{
				SetValue( "TextAlign", value );
			}
		}

	}
#endif
}