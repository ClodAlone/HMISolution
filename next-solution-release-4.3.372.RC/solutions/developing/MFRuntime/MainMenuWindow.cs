using System;
using System.Collections;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Shapes;

namespace MFRuntime.UI
{
    //////////////////////////////////////////////////////////////////////////////

    // This is the main menu window; it shows the animated, sliding menu icons
    // and instructions to the user. It also handles button presses to move
    // the menu icons and handle selection of the menu items.
    internal sealed class MainMenuWindow : PresentationWindow
    {

        // This member keeps the menu item panel around
        private MenuItemPanel m_MenuItemPanel;

        public MainMenuWindow(Program program)
            : base(program)
        {
            // The Main window contains a veritcal StackPanel
            StackPanel panel = new StackPanel(Orientation.Vertical);
            this.Child = panel;

            // The top child contains a horizontal StackPanel
            m_MenuItemPanel = new MenuItemPanel(this.Width, this.Height);

            // The top child contains the menu items
            // We pass in the small bitmap, large bitmap a description and then a large bitmap to use
            // as a common sized bitmap for calculating the width and height of a MenuItem
            MenuItem menuItem1 = new MenuItem(Resources.BitmapResources.Canvas_Panel_Icon_Small, Resources.BitmapResources.Canvas_Panel_Icon, "SysInfo", Resources.BitmapResources.Canvas_Panel_Icon);
            MenuItem menuItem2 = new MenuItem(Resources.BitmapResources.Canvas_Panel_Icon_Small, Resources.BitmapResources.Canvas_Panel_Icon, "Main", Resources.BitmapResources.Canvas_Panel_Icon);

            // Add each of the menu items to the menu item panel
            m_MenuItemPanel.AddMenuItem(menuItem1);
            m_MenuItemPanel.AddMenuItem(menuItem2);

            // Add the menu item panel to the main window panel
            panel.Children.Add(m_MenuItemPanel);
        }

        protected override void OnButtonDown(ButtonEventArgs e)
        {
            switch (e.Button)
            {
                // If <Enter> button is pressed, go into the selected demo
                case Button.VK_SELECT:
                {
                    switch (m_MenuItemPanel.CurrentChild)
                    {
                        case 0:  
                            new SysInfo(_program);
                            break;
                        case 1:
                            new BouncyViewWindow(_program);
                            break;
                    }
                }
                break;

                // If <Left> button is pressed, change the menu item left one
                case Button.VK_LEFT:
                {
                    if (m_MenuItemPanel != null)
                        m_MenuItemPanel.CurrentChild--;
                }
                break;

                // If <Right> button is pressed, change the menu item right one
                case Button.VK_RIGHT:
                {
                    if (m_MenuItemPanel != null)
                        m_MenuItemPanel.CurrentChild++;
                }
                break;
            }

            // Don't call base implementation (base.OnButtonDown) or we'll go back Home
        }
    }
}
