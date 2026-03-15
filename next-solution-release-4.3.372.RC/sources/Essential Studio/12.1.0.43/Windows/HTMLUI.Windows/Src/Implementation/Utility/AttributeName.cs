#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Class that represents the names of attributes in tag elements.
    /// </summary>
    [AttributeHolder(typeof(HTMLAttributesCollection))]
    internal sealed class AttributeName
    {
        #region Class constants
        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("name", ReactType.None)]
        public const string Name = "name";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("size", ReactType.ReCalculatingDocument)]
        public const string Size = "size";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("href", ReactType.None)]
        public const string Href = "href";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("src", ReactType.ReCalculatingDocument)]
        public const string Src = "src";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("width", ReactType.ReFormatMergElmAndReCalcDoc)]
        public const string Width = "width";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("height", ReactType.ReFormatMergElmAndReCalcDoc)]
        public const string Height = "height";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("title", ReactType.None)]
        public const string Title = "title";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("noshade", ReactType.RePaintElement)]
        public const string Noshade = "noshade";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("color", ReactType.ReFormatMergElmAndReCalcDoc)]
        public const string Color = "color";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("type", ReactType.None)]
        public const string Type = "type";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("value", ReactType.None)]
        public const string Value = "value";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("maxlength", ReactType.None)]
        public const string MaxLength = "maxlength";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("checked", ReactType.None)]
        public const string Checked = "checked";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("rows", ReactType.None)]
        public const string Rows = "rows";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("cols", ReactType.None)]
        public const string Cols = "cols";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("multiple", ReactType.None)]
        public const string Multiple = "multiple";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("selected", ReactType.None)]
        public const string Selected = "selected";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("tabindex", ReactType.None)]
        public const string TabIndex = "tabindex";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("colspan", ReactType.ReCalculatingDocument)]
        public const string Colspan = "colspan";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("rowspan", ReactType.ReCalculatingDocument)]
        public const string Rowspan = "rowspan";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("bgcolor", ReactType.ReFormatMergElmAndReCalcDoc)]
        public const string BgColor = "bgcolor";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("border", ReactType.ReCalculatingDocument)]
        public const string Border = "border";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("align", ReactType.ReFormatCrtElmAndReCalcDoc)]
        public const string Align = "align";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("valign", ReactType.ReFormatCrtElmAndReCalcDoc)]
        public const string VAlign = "valign";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("style", ReactType.ReFormatCrtElmAndReCalcDoc)]
        public const string Style = "style";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("class", ReactType.ReFormatCrtElmAndReCalcDoc)]
        public const string Class = "class";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("rel", ReactType.None)]
        public const string Rel = "rel";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("background", ReactType.RePaintDocument)]
        public const string Background = "background";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("language", ReactType.None)]
        public const string Language = "language";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("alt", ReactType.None)]
        public const string Alt = "alt";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("bordercolor", ReactType.ReFormatMergElmAndReCalcDoc)]
        public const string BorderColor = "bordercolor";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("display", ReactType.ReCalculatingDocument)]
        public const string Display = "display";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("leftmargin", ReactType.ReCalculatingDocument)]
        public const string LeftMargin = "leftmargin";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("topmargin", ReactType.ReCalculatingDocument)]
        public const string TopMargin = "topmargin";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("rightmargin", ReactType.ReCalculatingDocument)]
        public const string RightMargin = "rightmargin";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("bottommargin", ReactType.ReCalculatingDocument)]
        public const string BottomMargin = "bottommargin";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("nowrap", ReactType.ReCalculatingDocument)]
        public const string NoWrap = "nowrap";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("face", ReactType.ReFormatMergElmAndChildrenAndReCalcDoc)]
        public const string Face = "face";

        /// <summary>
        /// Name of the tag attribute.
        /// </summary>
        [ReactionType("disabled", ReactType.None)]
        public const string Disabled = "disabled";
        #endregion

        /// <summary>
        /// Prevents a default instance of the AttributeName class from being created
        /// </summary>
        private AttributeName()
        {
            throw new NotImplementedException();
        }
    }
}
