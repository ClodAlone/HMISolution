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
    public class EditorPropertiesBuilder
    {
        public Numeric editor;
        public EditorPropertiesBuilder(Numeric edit)
        {
            this.editor = new Numeric(edit.ID, edit.EditorModel);
        }
        public EditorPropertiesBuilder(Percent edit){
            editor = new Numeric { ID = edit.ID, EditorModel = edit.EditorModel,EditorName=edit.EditorName };
        }
        public EditorPropertiesBuilder(Currency edit) {
            editor = new Numeric { ID = edit.ID, EditorModel = edit.EditorModel, EditorName = edit.EditorName };
        }
        public EditorPropertiesBuilder()
        {
        }
         //Boolean values
        public EditorPropertiesBuilder Enabled()
        {
            editor.EditorModel.Enabled = true;
            return this;
        }
        public EditorPropertiesBuilder Enabled(bool enabled)
        {
            editor.EditorModel.Enabled = enabled;
            return this;
        }
        public EditorPropertiesBuilder RoundedCorner()
        {
            editor.EditorModel.RoundedCorner = true;
            return this;
        }
        public EditorPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            editor.EditorModel.RoundedCorner = roundedCorner;
            return this;
        }
        public EditorPropertiesBuilder Rtl()
        {
            editor.EditorModel.Rtl = true;
            return this;
        }
        public EditorPropertiesBuilder Rtl(bool rtl)
        {
            editor.EditorModel.Rtl = rtl;
            return this;
        }
        public EditorPropertiesBuilder Persist()
        {
            editor.EditorModel.Persist = true;
            return this;
        }
        public EditorPropertiesBuilder Persist(bool persist)
        {
            editor.EditorModel.Persist = persist;
            return this;
        }
        public EditorPropertiesBuilder ShowSpinButton()
        {
            editor.EditorModel.ShowSpinButton = true;
            return this;
        }
        public EditorPropertiesBuilder ShowSpinButton(bool showSpinButton)
        {
            editor.EditorModel.ShowSpinButton = showSpinButton;
            return this;
        }
        public EditorPropertiesBuilder StrictMode()
        {
            editor.EditorModel.StrictMode = true;
            return this;
        }
        public EditorPropertiesBuilder StrictMode(bool strictMode)
        {
            editor.EditorModel.StrictMode = strictMode;
            return this;
        }
        public EditorPropertiesBuilder ReadOnly()
        {
            editor.EditorModel.ReadOnly = true;
            return this;
        }
        public EditorPropertiesBuilder ReadOnly(bool readOnly)
        {
            editor.EditorModel.ReadOnly = readOnly;
            return this;
        }
        public EditorPropertiesBuilder MinValue(double minValue)
        {
            editor.EditorModel.MinValue = minValue;
            return this;
        }
        public EditorPropertiesBuilder MaxValue(double maxValue)
        {
            editor.EditorModel.MaxValue = maxValue;
            return this;
        }
        public EditorPropertiesBuilder IncrementStep(int incrementStep)
        {
            editor.EditorModel.IncrementStep = incrementStep;
            return this;
        }
        public EditorPropertiesBuilder Decimals(int decimals)
        {
            editor.EditorModel.Decimals = decimals;
            return this;
        }
        //String Values
        public EditorPropertiesBuilder Width(string width)
        {
            editor.EditorModel.Width = width;
            return this;
        }
        public EditorPropertiesBuilder Height(string height)
        {
            editor.EditorModel.Height = height;
            return this;
        }
        public EditorPropertiesBuilder CssClass(string cssClass)
        {
            editor.EditorModel.CssClass = cssClass;
            return this;
        }
        public EditorPropertiesBuilder WaterMarkText(string waterMarkText)
        {
            editor.EditorModel.WaterMarkText = waterMarkText;
            return this;
        }
        public EditorPropertiesBuilder Localize(string localize)
        {
            editor.EditorModel.Localize = localize;
            return this;
        }
        public EditorPropertiesBuilder Value(string value)
        {
            editor.EditorModel.Value = value;
            return this;
        }
        public EditorPropertiesBuilder Name(string name)
        {
            editor.EditorModel.Name = name;
            return this;
        }
        //Events
        public EditorPropertiesBuilder ClientSideEvents(Action<EditorClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new EditorClientSideEventsBuilder(this.editor.EditorModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(editor.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
