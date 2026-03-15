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
    /// <summary>
    /// Class for AutoComplete Property Builder
    /// </summary>
    public class MobileAutoCompletePropertiesBuilder
    {
        #region Fields
        private AutoComplete mAutoComplete;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAutoCompletePropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mAutoComplete">The m automatic complete.</param>
        public MobileAutoCompletePropertiesBuilder(AutoComplete mAutoComplete)
        {
            this.mAutoComplete = new AutoComplete(mAutoComplete.ID, mAutoComplete.MAutoCompleteModel);

        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mAutoComplete.MAutoCompleteModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Theme(Theme theme)
        {
            mAutoComplete.MAutoCompleteModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Cases the sensitive.
        /// </summary>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder CaseSensitive(bool caseSensitive)
        {
            mAutoComplete.MAutoCompleteModel.CaseSensitive = caseSensitive;
            return this;
        }

        /// <summary>
        /// Automatics the fill.
        /// </summary>
        /// <param name="autoFill">if set to <c>true</c> [automatic fill].</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder AutoFill(bool autoFill)
        {
            mAutoComplete.MAutoCompleteModel.AutoFill = autoFill;
            return this;
        }

        /// <summary>
        /// Multis the value.
        /// </summary>
        /// <param name="multiValue">if set to <c>true</c> [multi value].</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder MultiValue(bool multiValue)
        {
            mAutoComplete.MAutoCompleteModel.MultiValue = multiValue;
            return this;
        }

        /// <summary>
        /// Shows the checkbox.
        /// </summary>
        /// <param name="showCheckbox">if set to <c>true</c> [show checkbox].</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder ShowCheckbox(bool showCheckbox)
        {
            mAutoComplete.MAutoCompleteModel.ShowCheckbox = showCheckbox;
            return this;
        }

        /// <summary>
        /// Allows the sorting.
        /// </summary>
        /// <param name="allowSorting">if set to <c>true</c> [allow sorting].</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder AllowSorting(bool allowSorting)
        {
            mAutoComplete.MAutoCompleteModel.AllowSorting = allowSorting;
            return this;
        }

        /// <summary>
        /// Noes the results.
        /// </summary>
        /// <param name="noResults">The no results.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder NoResults(string noResults)
        {
            mAutoComplete.MAutoCompleteModel.NoResults = noResults;
            return this;
        }

        /// <summary>
        /// Shows the no results.
        /// </summary>
        /// <param name="showNoResults">if set to <c>true</c> [show no results].</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder ShowNoResults(bool showNoResults)
        {
            mAutoComplete.MAutoCompleteModel.ShowNoResults = showNoResults;
            return this;
        }

        /// <summary>
        /// Distincts the specified distinct.
        /// </summary>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Distinct(bool distinct)
        {
            mAutoComplete.MAutoCompleteModel.Distinct = distinct;
            return this;
        }

        /// <summary>
        /// Minimums the character.
        /// </summary>
        /// <param name="minCharacter">The minimum character.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder MinCharacter(int minCharacter)
        {
            mAutoComplete.MAutoCompleteModel.MinCharacter = minCharacter;
            return this;
        }

        /// <summary>
        /// Persists the specified persist.
        /// </summary>
        /// <param name="persist">if set to <c>true</c> [persist].</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Persist(bool persist)
        {
            mAutoComplete.MAutoCompleteModel.Persist = persist;
            return this;
        }

        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Enabled(bool enabled)
        {
            mAutoComplete.MAutoCompleteModel.Enabled = enabled;
            return this;
        }

        /// <summary>
        /// Delimiters the specified delimiter.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Delimiter(string delimiter)
        {
            mAutoComplete.MAutoCompleteModel.Delimiter = delimiter;
            return this;
        }

        /// <summary>
        /// Watermarks the text.
        /// </summary>
        /// <param name="watermarkText">The watermark text.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder WatermarkText(string watermarkText)
        {
            mAutoComplete.MAutoCompleteModel.WatermarkText = watermarkText;
            return this;
        }

        /// <summary>
        /// Values the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Value(string value)
        {
            mAutoComplete.MAutoCompleteModel.Value = value;
            return this;
        }

        /// <summary>
        /// Mappers the specified mapper.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Mapper(string mapper)
        {
            mAutoComplete.MAutoCompleteModel.Mapper = mapper;
            return this;
        }

        /// <summary>
        /// Lists the size.
        /// </summary>
        /// <param name="listSize">Size of the list.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder ListSize(double listSize)
        {
            mAutoComplete.MAutoCompleteModel.ListSize = listSize;
            return this;
        }

        /// <summary>
        /// Fields the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Field(string field)
        {
            mAutoComplete.MAutoCompleteModel.Field = field;
            return this;
        }

        /// <summary>
        /// Images the class.
        /// </summary>
        /// <param name="imageClass">The image class.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder ImageClass(string imageClass)
        {
            mAutoComplete.MAutoCompleteModel.ImageClass = imageClass;
            return this;
        }

        /// <summary>
        /// Images the field.
        /// </summary>
        /// <param name="imageField">The image field.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder ImageField(string imageField)
        {
            mAutoComplete.MAutoCompleteModel.ImageField = imageField;
            return this;
        }

        /// <summary>
        /// Datas the source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder DataSource(object dataSource)
        {
            mAutoComplete.MAutoCompleteModel.DataSource = dataSource;
            return this;
        }

        /// <summary>
        /// Filters the type.
        /// </summary>
        /// <param name="filterType">Type of the filter.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder FilterType(MobileFilterType filterType)
        {
            mAutoComplete.MAutoCompleteModel.FilterType = filterType;
            return this;
        }

        /// <summary>
        /// Filters the mode.
        /// </summary>
        /// <param name="filterMode">The filter mode.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder FilterMode(FilterMode filterMode)
        {
            mAutoComplete.MAutoCompleteModel.FilterMode = filterMode;
            return this;
        }

        /// <summary>
        /// Sorts the order.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder SortOrder(MobileSortOrder sortOrder)
        {
            mAutoComplete.MAutoCompleteModel.SortOrder = sortOrder;
            return this;
        }

        /// <summary>
        /// Modes the specified mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Mode(Mode mode)
        {
            mAutoComplete.MAutoCompleteModel.Mode = mode;
            return this;
        }


        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder Windows(Action<MobileAutoCompleteWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileAutoCompleteWindowsPropertiesBuilder(this.mAutoComplete.MAutoCompleteModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }


        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileAutoCompletePropertiesBuilder ClientSideEvents(Action<MobileAutoCompleteClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileAutoCompleteClientSideEventsBuilder(this.mAutoComplete.MAutoCompleteModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
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
            return new HtmlString(mAutoComplete.Render().ToString());
        }
        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override String ToString()
        {
            return Render().ToString();
        }
        #endregion
    }
}
