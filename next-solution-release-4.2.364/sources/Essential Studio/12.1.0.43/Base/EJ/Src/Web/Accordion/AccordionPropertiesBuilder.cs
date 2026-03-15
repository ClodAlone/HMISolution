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


namespace Syncfusion.JavaScript
{
   public class AccordionPropertiesBuilder
    {
       public Accordion accordion;

       public AccordionPropertiesBuilder(Accordion accordion)
       { this.accordion = new Accordion(accordion.ID, accordion.AccordionModel); }

            public AccordionPropertiesBuilder()
       {
       }
       //Boolean Values
       public AccordionPropertiesBuilder Collapsible()
       {
           accordion.AccordionModel.Collapsible = true;
           return this;
       }
       public AccordionPropertiesBuilder Collapsible(bool collapsible)
       {
           accordion.AccordionModel.Collapsible = collapsible;
           return this;
       }
       public AccordionPropertiesBuilder Rtl()
       {
           accordion.AccordionModel.Rtl = true;
           return this;
       }
       public AccordionPropertiesBuilder Rtl(bool rtl)
       {
           accordion.AccordionModel.Rtl = rtl;
           return this;
       }
       public AccordionPropertiesBuilder RoundedCorner()
       {
           accordion.AccordionModel.RoundedCorner = true;
           return this;
       }
       public AccordionPropertiesBuilder RoundedCorner(bool roundedCorner)
       {
           accordion.AccordionModel.RoundedCorner = roundedCorner;
           return this;
       }
       public AccordionPropertiesBuilder AllowKeyboardNavigation()
       {
           accordion.AccordionModel.AllowKeyboardNavigation = true;
           return this;
       }
       public AccordionPropertiesBuilder AllowKeyboardNavigation(bool allowKeyboardNavigation)
       {
           accordion.AccordionModel.AllowKeyboardNavigation = allowKeyboardNavigation;
           return this;
       }
       public AccordionPropertiesBuilder MultipleOpen()
       {
           accordion.AccordionModel.MultipleOpen = true;
           return this;
       }
       public AccordionPropertiesBuilder MultipleOpen(bool multipleOpen)
       {
           accordion.AccordionModel.MultipleOpen = multipleOpen;
           return this;
       }
       public AccordionPropertiesBuilder Enabled()
       {
           accordion.AccordionModel.Enabled = true;
           return this;
       }
       public AccordionPropertiesBuilder Enabled(bool enabled)
       {
           accordion.AccordionModel.Enabled = enabled;
           return this;
       }
       public AccordionPropertiesBuilder Persist()
       {
           accordion.AccordionModel.Persist = true;
           return this;
       }
       public AccordionPropertiesBuilder Persist(bool persist)
       {
           accordion.AccordionModel.Persist = persist;
           return this;
       }
       public AccordionPropertiesBuilder CssClass(String cssClass)
       {
           accordion.AccordionModel.CssClass = cssClass;
           return this;
       }
       public AccordionPropertiesBuilder Events(String events)
       {
           accordion.AccordionModel.Events = events;
           return this;
       }
       public AccordionPropertiesBuilder HeightStyles(HeightStyle heightStyle)
       {
           accordion.AccordionModel.HeightStyles = heightStyle;
           return this;
       }

       public AccordionPropertiesBuilder DisabledItems(List<String> disabledItems)
       {
           accordion.AccordionModel.DisabledItems = disabledItems;
           return this;
       }
       public AccordionPropertiesBuilder SelectedItems(List<String> selectedItems)
       {
           accordion.AccordionModel.SelectedItems = selectedItems;
           return this;
       }
       //Integer Values
       public AccordionPropertiesBuilder SelectedItemIndex(int selectedItemIndex)
       {
           accordion.AccordionModel.SelectedItemIndex = selectedItemIndex;
           return this;
       }
       //Objects
       public AccordionPropertiesBuilder AjaxOptions(Action<jQueryAjaxOptionsBuilder> ajaxOptions)
       {
           var ajaxOpt = new jQueryAjaxOptions();
           accordion.AccordionModel.AjaxOptions = ajaxOpt;
           var builder = new jQueryAjaxOptionsBuilder(ajaxOpt);
           if (ajaxOptions != null)
               ajaxOptions.Invoke(builder);
           return this;
       }
       public AccordionPropertiesBuilder IconCSS(Action<IconCSSBuilder> iconCSS)
       {
           var iconcss = new IconCSS();
           accordion.AccordionModel.IconCSS = iconcss;
           var builder = new IconCSSBuilder(iconcss);
           if (iconCSS != null)
               iconCSS.Invoke(builder);
           return this;
       }
       //Events
       public AccordionPropertiesBuilder ClientSideEvents(Action<AccordionClientSideEventsBuilder> clientSideEvents)
       {
           var builder = new AccordionClientSideEventsBuilder(this.accordion.AccordionModel);
           if (clientSideEvents != null)
               clientSideEvents.Invoke(builder);
           return this;
       }
       //
       public AccordionPropertiesBuilder Items(Action<AccordionBaseItemAdder> items)
       {
           this.ItemsCollection = new List<AccordionBaseItem>();
           AccordionBaseItemAdder accordionAdded = new AccordionBaseItemAdder(this.accordion.AccordionModel.Items);
           items.Invoke(accordionAdded);
           return this as AccordionPropertiesBuilder;
       }
       
       //Render
       public HtmlString Render()
       {
           return new HtmlString(accordion.Render().ToString());
       }
       public override String ToString()
       {

           return Render().ToString();
       }


       public List<AccordionBaseItem> ItemsCollection { get; set; }
    }
}
