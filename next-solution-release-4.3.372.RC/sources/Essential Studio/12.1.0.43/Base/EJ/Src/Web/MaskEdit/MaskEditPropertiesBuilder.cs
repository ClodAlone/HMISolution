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
    public class MaskEditPropertiesBuilder
    {
        public MaskEdit maskEdit;

        public MaskEditPropertiesBuilder(MaskEdit maskEdit)
        { this.maskEdit = new MaskEdit(maskEdit.ID, maskEdit.MaskEditModel); }

        public MaskEditPropertiesBuilder()
        {
        }
        //Boolean values
        public MaskEditPropertiesBuilder Enabled()
        {
            maskEdit.MaskEditModel.Enabled = true;
            return this;
        }
        public MaskEditPropertiesBuilder Enabled(bool enabled)
        {
            maskEdit.MaskEditModel.Enabled = enabled;
            return this;
        }
        public MaskEditPropertiesBuilder RoundedCorner()
        {
            maskEdit.MaskEditModel.RoundedCorner = true;
            return this;
        }
        public MaskEditPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            maskEdit.MaskEditModel.RoundedCorner = roundedCorner;
            return this;
        }
        public MaskEditPropertiesBuilder Error()
        {
            maskEdit.MaskEditModel.Error = true;
            return this;
        }
        public MaskEditPropertiesBuilder Error(bool error)
        {
            maskEdit.MaskEditModel.Error = error;
            return this;
        }
        public MaskEditPropertiesBuilder ReadOnly()
        {
            maskEdit.MaskEditModel.ReadOnly = true;
            return this;
        }
        public MaskEditPropertiesBuilder ReadOnly(bool readOnly)
        {
            maskEdit.MaskEditModel.ReadOnly = readOnly;
            return this;
        }
        public MaskEditPropertiesBuilder HidePromptOnLeave()
        {
            maskEdit.MaskEditModel.HidePromptOnLeave = true;
            return this;
        }
        public MaskEditPropertiesBuilder HidePromptOnLeave(bool hidePromptOnLeave)
        {
            maskEdit.MaskEditModel.HidePromptOnLeave = hidePromptOnLeave;
            return this;
        }
        //EnumValues
        public MaskEditPropertiesBuilder TextAlign(TextAlign textAlign)
        {
            maskEdit.MaskEditModel.TextAlign = textAlign;
            return this;
        }
        public MaskEditPropertiesBuilder InputMode(InputMode inputMode)
        {
            maskEdit.MaskEditModel.InputMode = inputMode;
            return this;
        }
        //String Values
        public MaskEditPropertiesBuilder Height(String height)
        {
            maskEdit.MaskEditModel.Height = height;
            return this;
        }
        public MaskEditPropertiesBuilder Width(String width)
        {
            maskEdit.MaskEditModel.Width = width;
            return this;
        }
        public MaskEditPropertiesBuilder CssClass(String cssClass)
        {
            maskEdit.MaskEditModel.CssClass = cssClass;
            return this;
        }
        public MaskEditPropertiesBuilder Mask(String mask)
        {
            maskEdit.MaskEditModel.Mask = mask;
            return this;
        }
        public MaskEditPropertiesBuilder Value(String vaue)
        {
            maskEdit.MaskEditModel.Value = vaue;
            return this;
        }
        public MaskEditPropertiesBuilder WaterMarkText(String waterMarkText)
        {
            maskEdit.MaskEditModel.WaterMarkText = waterMarkText;
            return this;
        }
        public MaskEditPropertiesBuilder CustomChar(String customChar)
        {
            maskEdit.MaskEditModel.CustomChar = customChar;
            return this;
        }
        //Events
        public MaskEditPropertiesBuilder ClientSideEvents(Action<MaskEditClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MaskEditClientSideEventsBuilder(this.maskEdit.MaskEditModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(maskEdit.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
