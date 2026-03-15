#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;


namespace Syncfusion.JavaScript
{
    public class MenuPropertiesBuilder
    {
        public Menu menu;

        public MenuPropertiesBuilder(Menu menu)
        { this.menu = new Menu(menu.ID, menu.MenuModel); }

        public MenuPropertiesBuilder()
        {
        }
         //Boolean values
        public MenuPropertiesBuilder OpenOnClick()
        {
            menu.MenuModel.OpenOnClick = true;
            return this;
        }
        public MenuPropertiesBuilder OpenOnClick(bool openOnClick)
        {
            menu.MenuModel.OpenOnClick = openOnClick;
            return this;
        }
        public MenuPropertiesBuilder CenterAlign()
        {
            menu.MenuModel.CenterAlign = true;
            return this;
        }
        public MenuPropertiesBuilder CenterAlign(bool centerAlign)
        {
            menu.MenuModel.CenterAlign = centerAlign;
            return this;
        }
        public MenuPropertiesBuilder ShowBottomLevelArrows()
        {
            menu.MenuModel.ShowBottomLevelArrows = true;
            return this;
        }
        public MenuPropertiesBuilder ShowBottomLevelArrows(bool showBottomLevelArrows)
        {
            menu.MenuModel.ShowBottomLevelArrows = showBottomLevelArrows;
            return this;
        }
        public MenuPropertiesBuilder ShowTopLevelArrows()
        {
            menu.MenuModel.ShowTopLevelArrows = true;
            return this;
        }
        public MenuPropertiesBuilder ShowTopLevelArrows(bool showTopLevelArrows)
        {
            menu.MenuModel.ShowTopLevelArrows = showTopLevelArrows;
            return this;
        }
        public MenuPropertiesBuilder EnableSeparator()
        {
            menu.MenuModel.EnableSeparator = true;
            return this;
        }
        public MenuPropertiesBuilder EnableSeparator(bool enableSeparator)
        {
            menu.MenuModel.EnableSeparator = enableSeparator;
            return this;
        }
        public MenuPropertiesBuilder Enabled()
        {
            menu.MenuModel.Enabled = true;
            return this;
        }
        public MenuPropertiesBuilder Enabled(bool enabled)
        {
            menu.MenuModel.Enabled = enabled;
            return this;
        }
        public MenuPropertiesBuilder Rtl()
        {
            menu.MenuModel.Rtl = true;
            return this;
        }
        public MenuPropertiesBuilder Rtl(bool rtl)
        {
            menu.MenuModel.Rtl = rtl;
            return this;
        }
        //Enum values
        public MenuPropertiesBuilder Orientation(Orientation orientation)
        {
            menu.MenuModel.Orientation = orientation;
            return this;
        }
        public MenuPropertiesBuilder MenuType(MenuType menuType)
        {
            menu.MenuModel.MenuType = menuType;
            return this;
        }
        public MenuPropertiesBuilder Animation(Animation animation)
        {
            menu.MenuModel.Animation = animation;
            return this;
        }
        public MenuPropertiesBuilder SubMenuDirection(Direction subMenuDirection)
        {
            menu.MenuModel.SubMenuDirection = subMenuDirection;
            return this;
        }
        //object values
        public MenuPropertiesBuilder MenuFields(Action<MenuFieldsBuilder> fields)
        {
            var flds = new MenuFields();
            menu.MenuModel.MenuFields = flds;
            var builder = new MenuFieldsBuilder(flds);
            if (fields != null)
                fields.Invoke(builder);
            return this;
        }
        //String Values
        public MenuPropertiesBuilder CssClass(String cssClass)
        {
            menu.MenuModel.CssClass = cssClass;
            return this;
        }
        public MenuPropertiesBuilder Height(String height)
        {
            menu.MenuModel.Height = height;
            return this;
        }
        public MenuPropertiesBuilder Width(String width)
        {
            menu.MenuModel.Width = width;
            return this;
        }
        public MenuPropertiesBuilder ContextTargetId(String contextTargetId)
        {
            menu.MenuModel.ContextTargetId = contextTargetId;
            return this;
        }
        //
        public MenuPropertiesBuilder Items(Action<MenuBaseItemAdder> items)
        {
            this.ItemsCollection = new List<MenuBaseItem>();
            MenuBaseItemAdder menuAdded = new MenuBaseItemAdder(this.menu.MenuModel.Items);
            items.Invoke(menuAdded);
            return this as MenuPropertiesBuilder;
        }
        //Events
        public MenuPropertiesBuilder ClientSideEvents(Action<MenuClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MenuClientSideEventsBuilder(this.menu.MenuModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(menu.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
        internal List<MenuBaseItem> ItemsCollection { get; set; }
    }
}
