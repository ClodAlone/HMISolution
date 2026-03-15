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
    public class DialogPropertiesBuilder
    {
        public Dialog dialog;

        public DialogPropertiesBuilder(Dialog dialog)
        { this.dialog = new Dialog(dialog.ID, dialog.DialogModel); }
        
        public DialogPropertiesBuilder()
        {
        }
      //Int values
        public DialogPropertiesBuilder MinHeight(int minHeight)
        {
            dialog.DialogModel.MinHeight = minHeight;
            return this;
        }
        public DialogPropertiesBuilder MinWidth(int minWidth)
        {
            dialog.DialogModel.MinWidth = minWidth;
            return this;
        }
        public DialogPropertiesBuilder Width(int width)
        {
            dialog.DialogModel.Width = width;
            return this;
        }
        public DialogPropertiesBuilder ZIndex(int zIndex)
        {
            dialog.DialogModel.ZIndex = zIndex;
            return this;
        }
        //Boolean Values
        public DialogPropertiesBuilder AutoOpen()
        {
            dialog.DialogModel.AutoOpen = true;
            return this;
        }
        public DialogPropertiesBuilder AutoOpen(bool autoOpen)
        {
            dialog.DialogModel.AutoOpen = autoOpen;
            return this;
        }
        public DialogPropertiesBuilder CloseOnEscape()
        {
            dialog.DialogModel.CloseOnEscape = true;
            return this;
        }
        public DialogPropertiesBuilder CloseOnEscape(bool closeOnEscape)
        {
            dialog.DialogModel.CloseOnEscape = closeOnEscape;
            return this;
        }
        public DialogPropertiesBuilder Draggable()
        {
            dialog.DialogModel.Draggable = true;
            return this;
        }
        public DialogPropertiesBuilder Draggable(bool draggable)
        {
            dialog.DialogModel.Draggable = draggable;
            return this;
        }
        public DialogPropertiesBuilder Modal()
        {
            dialog.DialogModel.Modal = true;
            return this;
        }
        public DialogPropertiesBuilder Modal(bool modal)
        {
            dialog.DialogModel.Modal = modal;
            return this;
        }
        public DialogPropertiesBuilder Resizable()
        {
            dialog.DialogModel.Resizable = true;
            return this;
        }
        public DialogPropertiesBuilder Resizable(bool resizable)
        {
            dialog.DialogModel.Resizable = resizable;
            return this;
        }
        public DialogPropertiesBuilder WindowResizing()
        {
            dialog.DialogModel.WindowResizing = true;
            return this;
        }
        public DialogPropertiesBuilder WindowResizing(bool windowResizing)
        {
            dialog.DialogModel.WindowResizing = windowResizing;
            return this;
        }
        public DialogPropertiesBuilder ShowHeader()
        {
            dialog.DialogModel.ShowHeader = true;
            return this;
        }
        public DialogPropertiesBuilder ShowHeader(bool showHeader)
        {
            dialog.DialogModel.ShowHeader = showHeader;
            return this;
        }
        public DialogPropertiesBuilder Rtl()
        {
            dialog.DialogModel.Rtl = true;
            return this;
        }
        public DialogPropertiesBuilder Rtl(bool rtl)
        {
            dialog.DialogModel.Rtl = rtl;
            return this;
        }
        public DialogPropertiesBuilder AllowKeyboardNavigation()
        {
            dialog.DialogModel.AllowKeyboardNavigation = true;
            return this;
        }
        public DialogPropertiesBuilder AllowKeyboardNavigation(bool allowKeyboardNavigation)
        {
            dialog.DialogModel.AllowKeyboardNavigation = allowKeyboardNavigation;
            return this;
        }
        public DialogPropertiesBuilder RoundedCorner()
        {
            dialog.DialogModel.RoundedCorner = true;
            return this;
        }
        public DialogPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            dialog.DialogModel.RoundedCorner = roundedCorner;
            return this;
        }
        public DialogPropertiesBuilder Persist()
        {
            dialog.DialogModel.Persist = true;
            return this;
        }
        public DialogPropertiesBuilder Persist(bool persist)
        {
            dialog.DialogModel.Persist = persist;
            return this;
        }
        public DialogPropertiesBuilder Enabled()
        {
            dialog.DialogModel.Enabled = true;
            return this;
        }
        public DialogPropertiesBuilder Enabled(bool enabled)
        {
            dialog.DialogModel.Enabled = enabled;
            return this;
        }
        //StringValues
        public DialogPropertiesBuilder CloseText(String closeText)
        {
            dialog.DialogModel.CloseText = closeText;
            return this;
        }
        public DialogPropertiesBuilder Height(String height)
        {
            dialog.DialogModel.Height = height;
            return this;
        }
        public DialogPropertiesBuilder MaxHeight(String maxHeight)
        {
            dialog.DialogModel.MaxHeight = maxHeight;
            return this;
        }
        public DialogPropertiesBuilder MaxWidth(String maxWidth)
        {
            dialog.DialogModel.MaxWidth = maxWidth;
            return this;
        }
        public DialogPropertiesBuilder Content(String content)
        {
            dialog.DialogModel.Content = content;
            return this;
        }
        public DialogPropertiesBuilder LoadUrl(String loadUrl)
        {
            dialog.DialogModel.LoadUrl = loadUrl;
            return this;
        }
        public DialogPropertiesBuilder Title(String title)
        {
            dialog.DialogModel.Title = title;
            return this;
        }
        public DialogPropertiesBuilder CssClass(String cssClass)
        {
            dialog.DialogModel.CssClass = cssClass;
            return this;
        }
        public DialogPropertiesBuilder CustomIconCss(String customIconCss)
        {
            dialog.DialogModel.CustomIconCss = customIconCss;
            return this;
        }
        public DialogPropertiesBuilder ContentContainer(String contentContainer)
        {
            dialog.DialogModel.ContentContainer = contentContainer;
            return this;
        }
        //String Array
        public DialogPropertiesBuilder IconAction(List<String> iconAction)
        {
            dialog.DialogModel.IconAction = iconAction;
            return this;
        }

        //Objects
        public DialogPropertiesBuilder Position(Action<PositionBuilder> position)
        {
            var pos = new Position();
            dialog.DialogModel.Position = pos;
            var builder = new PositionBuilder(pos);
            if (position != null)
                position.Invoke(builder);
            return this;
        }
        public DialogPropertiesBuilder AjaxOptions(Action<jQueryAjaxOptionsBuilder> ajaxOptions)
        {
            var ajaxOpt = new jQueryAjaxOptions();
            dialog.DialogModel.AjaxOptions = ajaxOpt;
            var builder = new jQueryAjaxOptionsBuilder(ajaxOpt);
            if (ajaxOptions != null)
               ajaxOptions.Invoke(builder);
            return this;
        }
        //Events
        public DialogPropertiesBuilder ClientSideEvents(Action<DialogClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new DialogClientSideEventsBuilder(this.dialog.DialogModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        public DialogPropertiesBuilder Items(Action<DialogBaseItemBuilder> items)
        {
            DialogBaseItemBuilder dialogAdded = new DialogBaseItemBuilder(this.dialog.DialogModel.Items);
            items.Invoke(dialogAdded);
            return this as DialogPropertiesBuilder;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(dialog.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
        
    }
}
