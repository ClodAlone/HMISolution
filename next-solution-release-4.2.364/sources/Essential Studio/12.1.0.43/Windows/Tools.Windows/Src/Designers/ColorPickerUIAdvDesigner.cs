#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <exclude/>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	public class ColorPickerDesigner : ParentControlDesigner
	{
	}
#else
	public class ColorPickerDesigner : ControlDesigner
	{
	}
#endif

	public class ColorPickerUIAdvDesigner : 
		ColorPickerDesigner
	{
		ColorPickerUIAdv m_colorPicker = null;

		protected override bool GetHitTest(Point point)
		{
			return true;
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize (component);

			m_colorPicker = this.Control as ColorPickerUIAdv;

			m_colorPicker.MouseDown += new MouseEventHandler(Control_MouseDown);
		}

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
			ISelectionService selectionService = this.GetService(typeof(ISelectionService)) as ISelectionService;
            ArrayList list = new ArrayList();

            Point p = new Point(e.X, e.Y);
            bool hitsItem = false;

            foreach (ColorUIAdvGroup group in m_colorPicker.Groups)
            {
                if (group.Bounds.Contains(p))
                {
                    Point focus = m_colorPicker.GetItemUnderPoint(p);

                    ColorItem item = null;
                    if (focus.X != -1 && focus.Y != -1)
                    {
                        hitsItem = true;

                        item = m_colorPicker.GetItemFromFocus(focus);

                        list.Add(item);
                        selectionService.SetSelectedComponents(list);

                        break;
                    }
                }
            }

            if (!hitsItem && m_colorPicker.ClientRectangle.Contains(p))
            {
                list.Add(m_colorPicker);
                selectionService.SetSelectedComponents(list);
            }
        }
	}
}