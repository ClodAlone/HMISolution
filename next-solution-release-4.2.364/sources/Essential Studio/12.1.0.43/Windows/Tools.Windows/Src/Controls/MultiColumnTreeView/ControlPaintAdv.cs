#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    internal sealed class ControlPaintHelper
    {
        #region Class constants
        private const string DrawFlatCheckBoxMethodName = "DrawFlatCheckBox";
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        private const string DrawFrameControlMethodName = "DrawFrameControl";
#endif
        #endregion

        #region Class static members
        private static readonly MethodInfo DrawFlatCheckBoxMethodInfo;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        private static readonly MethodInfo DrawFrameControlMethodInfo;
#endif
        #endregion

        #region Class Initialize/Finalize methods
        static ControlPaintHelper()
        {
            MethodInfo[] mis = typeof(ControlPaint).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);

            foreach (MethodInfo mi in mis)
            {
                // if required methods found methods
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                if (DrawFlatCheckBoxMethodInfo != null && DrawFrameControlMethodInfo != null)
                    break;
#else
        if( DrawFlatCheckBoxMethodInfo != null ) break;
#endif
                switch (mi.Name)
                {
                    case DrawFlatCheckBoxMethodName:
                        if (mi.GetParameters().Length > 3)
                        {
                            DrawFlatCheckBoxMethodInfo = mi;
                        }
                        break;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    case DrawFrameControlMethodName:
                        if (mi.GetParameters().Length > 7)
                        {
                            DrawFrameControlMethodInfo = mi;
                        }
                        break;
#endif
                }
            }
        }

        #endregion

        #region Class Public Methods
    
        public static void DrawCheckBox(Graphics g, Rectangle r, ButtonState bs, TreeNodeAdv tna)
        {
            Brush brush1 = ((bs & ButtonState.Inactive) == ButtonState.Inactive) ?
              tna.IntermediateCheckBoxBackGround : tna.CheckBoxBackGround;

            Color check = ((bs & ButtonState.Inactive) == ButtonState.Inactive) ?
              tna.IntermediateCheckColor : tna.CheckColor;

            DrawFlatCheckBox(g, r, check, brush1, bs);
        }

        public static void DrawRadioButton(Graphics g, Rectangle r, ButtonState bs, TreeNodeAdv tna)
        {
            int state = 4 | (int)bs;

            DrawFrameControl(g, r.Left, r.Top, r.Width, r.Height, 4, state, tna.SelectedOptionButtonColor, tna.OptionButtonColor);       
        }
        #endregion

        #region Class utility methods
      
        private static void DrawFlatCheckBox(Graphics g, Rectangle r, Color check, Brush brush, ButtonState bs)
        {
            if (DrawFlatCheckBoxMethodInfo != null)
            {
                DrawFlatCheckBoxMethodInfo.Invoke(null, new object[] { g, r, check, brush, bs });
            }
        }

        private static void DrawFrameControl(Graphics g, int left, int top, int width, int height, int kind, int state, Color fore, Color back)
        {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            if (DrawFrameControlMethodInfo != null)
            {
                DrawFrameControlMethodInfo.Invoke(null, new object[] { g, left, top, width, height, kind, state, fore, back });
            }
#endif
        }
        #endregion
    }
}