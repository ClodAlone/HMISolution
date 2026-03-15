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
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileEditorPropertiesBuilder
    {
        #region Fields
        private MobileEditor mEditor;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MEditorPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mEditor">The m Editor.</param>
        public MobileEditorPropertiesBuilder(MobileEditor mEditor)
        {
            this.mEditor = new MobileEditor(mEditor.ID, mEditor.MobileEditorModel);
        }

        #endregion

        #region Builder

        public MobileEditorPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mEditor.MobileEditorModel.RenderMode = renderMode;
            return this;
        }

        public MobileEditorPropertiesBuilder Theme(Theme theme)
        {
            mEditor.MobileEditorModel.Theme = theme;
            return this;
        }

        public MobileEditorPropertiesBuilder ShowBorder(bool showborder)
        {
            mEditor.MobileEditorModel.ShowBorder = showborder;
            return this;
        }

        public MobileEditorPropertiesBuilder StrictMode(bool strictmode)
        {
            mEditor.MobileEditorModel.StrictMode = strictmode;
            return this;
        }

        public MobileEditorPropertiesBuilder incrementStep(int value)
        {
            mEditor.MobileEditorModel.IncrementStep = value;
            return this;
        }

        public MobileEditorPropertiesBuilder ShowDecimals(int showdecimals)
        {
            mEditor.MobileEditorModel.Decimals = showdecimals;
            return this;
        }

        public MobileEditorPropertiesBuilder ShowSpinButton(bool showspinbutton)
        {
            mEditor.MobileEditorModel.ShowSpinButton = showspinbutton;
            return this;
        }

        public MobileEditorPropertiesBuilder Value(string value)
        {
            mEditor.MobileEditorModel.Value = value;
            return this;
        }

        public MobileEditorPropertiesBuilder Name(string value)
        {
            mEditor.MobileEditorModel.Name = value;
            return this;
        }

        public MobileEditorPropertiesBuilder WaterMarkText(string watermarktext)
        {
            mEditor.MobileEditorModel.WaterMarkText = watermarktext;
            return this;
        }

        public MobileEditorPropertiesBuilder Enabled(bool enabled)
        {
            mEditor.MobileEditorModel.Enabled = enabled;
            return this;
        }

        public MobileEditorPropertiesBuilder Persist(bool persist)
        {
            mEditor.MobileEditorModel.Persist = persist;
            return this;
        }

        public MobileEditorPropertiesBuilder ReadOnly(bool value)
        {
            mEditor.MobileEditorModel.ReadOnly = value;
            return this;
        }
        public MobileEditorPropertiesBuilder MinValue(double minValue)
        {
            mEditor.MobileEditorModel.MinValue = minValue;
            return this;
        }
        public MobileEditorPropertiesBuilder MaxValue(double maxValue)
        {
            mEditor.MobileEditorModel.MaxValue = maxValue;
            return this;
        }

        public MobileEditorPropertiesBuilder ClientSideEvents(Action<MobileEditorClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileEditorClientSideEventsBuilder(this.mEditor.MobileEditorModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        public MobileEditorPropertiesBuilder Windows(Action<MobileEditorWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileEditorWindowsPropertiesBuilder(this.mEditor.MobileEditorModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }

        #endregion

        #region Render
        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns></returns>
        public HtmlString Render()
        {
            return new HtmlString(mEditor.Render().ToString());
        }
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            return Render().ToString();
        }
        #endregion
    }
}
