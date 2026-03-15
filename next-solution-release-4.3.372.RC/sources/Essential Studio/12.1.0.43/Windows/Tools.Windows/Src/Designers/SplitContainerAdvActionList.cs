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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Text;
using System.Reflection;
using System.Globalization;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Tools.Enums;

namespace Syncfusion.Windows.Forms.Tools
{
	public class SplitContainerAdvActionList : SyncActionListBase<SplitContainerAdv>
	{
		public SplitContainerAdvActionList (IComponent component)
			: base(component)
		{
			
		}

		protected override void InitializeActionList ( )
		{
			this.AddDesignerActionHeaderItem("Essential Tools - SplitContainerAdv");

			
			this.AddDesignerActionPropertyItem("IsSplitterFixed", "Fixed Splitter", "Appearance", "Specifies whether the splitter should be fixed.");
			this.AddDesignerActionPropertyItem("Panel1Collapsed", "Collapse Panel1", "Appearance", "Specifies whether panel1 should be collapsed.");
			this.AddDesignerActionPropertyItem("Panel2Collapsed", "Collapse Panel2", "Appearance", "Specifies whether panel2 should be collapsed.");

			this.AddDesignerActionPropertyItem("BorderStyle", "BorderStyle", "Appearance", "Specifies the border style of panel.");
			this.AddDesignerActionPropertyItem("FixedPanel", "Fixed Panel", "Appearance", "Specifies which panel should be fixed.");
			this.AddDesignerActionPropertyItem("Orientation", "Orientation", "Appearance", "Specifies the orientation of the panels.");
			this.AddDesignerActionPropertyItem("Panel1MinSize", "Panel1 MinSize", "Appearance", "Specifies the minimum size of the panel1.");
			this.AddDesignerActionPropertyItem("Panel2MinSize", "Panel2 MinSize", "Appearance", "Specifies the minimum size of the panel2.");
			this.AddDesignerActionPropertyItem("SplitterIncrement", "Splitter Increment", "Appearance", "Specifies the no. of pixels the splitter moves in increment.");
			this.AddDesignerActionPropertyItem("SplitterWidth", "Splitter Width", "Appearance", "Specifies the thickness of the splitter.");

		}

		public bool IsSplitterFixed
		{
			get
			{
				bool isSplitterFixed = false;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
					isSplitterFixed = control.IsSplitterFixed;
				}
				return isSplitterFixed;
			}
			set
			{
				SetValue("IsSplitterFixed", value);
			}
		}

		public BorderStyle BorderStyle
		{
			get
			{
				BorderStyle borderStyle = BorderStyle.None;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
					borderStyle = control.BorderStyle;
				}
				return borderStyle;
			}
			set
			{
				SetValue("BorderStyle", value);
			}
		}

		public Syncfusion.Windows.Forms.Tools.Enums.FixedPanel FixedPanel
		{
			get
			{
                Syncfusion.Windows.Forms.Tools.Enums.FixedPanel fixedPanel = Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.None;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
					fixedPanel = control.FixedPanel;
				}
				return fixedPanel;
			}
			set
			{
				SetValue("FixedPanel", value);
			}
		}

		public Orientation Orientation
		{
			get
			{
				Orientation orientation = Orientation.Horizontal;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
					orientation = control.Orientation;
				}
				return orientation;
			}
			set
			{
				SetValue("Orientation", value);
			}
		}

		public bool Panel1Collapsed
		{
			get
			{
				bool panel1Collapsed = false;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
                    panel1Collapsed = ( control.CollapsedPanel == CollapsedPanel.Panel1 );
				}
				return panel1Collapsed;
			}
			set
			{
				SetValue("Panel1Collapsed", value);
			}
		}

		public bool Panel2Collapsed
		{
			get
			{
				bool panel2Collapsed = false;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
                    panel2Collapsed = ( control.CollapsedPanel == CollapsedPanel.Panel2 );
				}
				return panel2Collapsed;
			}
			set
			{
				SetValue("Panel2Collapsed", value);
			}
		}

		public int Panel1MinSize
		{
			get
			{
				int panel1MinSize = 25;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
					panel1MinSize = control.Panel1MinSize;
				}
				return panel1MinSize;
			}
			set
			{
				SetValue("Panel1MinSize", value);
			}
		}

		public int Panel2MinSize
		{
			get
			{
				int panel2MinSize = 25;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
					panel2MinSize = control.Panel2MinSize;
				}
				return panel2MinSize;
			}
			set
			{
				SetValue("Panel2MinSize", value);
			}
		}

		public int SplitterIncrement
		{
			get
			{
				int splitterIncrement = 1;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
					splitterIncrement = control.SplitterIncrement;
				}
				return splitterIncrement;
			}
			set
			{
				SetValue("SplitterIncrement", value);
			}
		}

		public int SplitterWidth
		{
			get
			{
				int splitterWidth = 6;
				if (this.Control != null)
				{
					SplitContainerAdv control = this.Control as SplitContainerAdv;
					splitterWidth = control.SplitterWidth;
				}
				return splitterWidth;
			}
			set
			{
				SetValue("SplitterWidth", value);
			}
		}
	}
}
#endif