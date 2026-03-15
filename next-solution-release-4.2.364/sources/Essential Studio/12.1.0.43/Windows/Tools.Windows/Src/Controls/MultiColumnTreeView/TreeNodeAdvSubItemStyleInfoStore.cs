#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;

using Syncfusion.Drawing;
using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{ 
    [TypeConverter(typeof(TreeNodeAdvSubItemStyleInfoStoreConverter))]
    public class TreeNodeAdvSubItemStyleInfoStore : StyleInfoStore
    {
        #region Class static members

        private static readonly StaticData s_sd = new StaticData(typeof(TreeNodeAdvSubItemStyleInfoStore), typeof(TreeNodeAdvSubItemStyleInfo), false);
         
        #endregion
        #region Class properties
        /// <summary>
        /// Static data must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        [XmlIgnore]
        protected override StaticData StaticDataStore
        {
            get
            {
                return s_sd;
            }
        }
        internal static StaticData Store
        {
            get
            {
                return s_sd;
            }
        }
        #endregion

        #region Storage Properties List
        /// <summary>Declaration of storage propterty: BaseStyle.</summary>
        internal static readonly StyleInfoProperty BaseStyleProperty = Store.CreateStyleInfoProperty(typeof(string), "BaseStyle");

        /// <summary>Declaration of storage propterty: Visible.</summary>
        internal static readonly StyleInfoProperty VisibleProperty = Store.CreateStyleInfoProperty(typeof(bool), "Visible");

        /// <summary>Declaration of storage propterty: Font.</summary>
        internal static readonly StyleInfoProperty FontProperty = Store.CreateStyleInfoProperty(typeof(Font), "Font");

        /// <summary>Declaration of storage propterty: LineAlignment.</summary>
        internal static readonly StyleInfoProperty LineAlignmentProperty = Store.CreateStyleInfoProperty(typeof(StringAlignment), "LineAlignment");

        /// <summary>Declaration of storage propterty: Alignment.</summary>
        internal static readonly StyleInfoProperty AlignmentProperty = Store.CreateStyleInfoProperty(typeof(StringAlignment), "Alignment");

        /// <summary>Declaration of storage propterty: Text.</summary>
        internal static readonly StyleInfoProperty TextProperty = Store.CreateStyleInfoProperty(typeof(string), "Text");

        /// <summary>Declaration of storage propterty: TextColor.</summary>
        internal static readonly StyleInfoProperty TextColorProperty = Store.CreateStyleInfoProperty(typeof(Color), "TextColor");

        /// <summary>Declaration of storage propterty: HelpText.</summary>
        internal static readonly StyleInfoProperty HelpTextProperty = Store.CreateStyleInfoProperty(typeof(string), "HelpText");

        /// <summary>Declaration of storage propterty: Tag.</summary>
        internal static readonly StyleInfoProperty TagProperty = Store.CreateStyleInfoProperty(typeof(object), "Tag");

        /// <summary>Declaration of storage propterty: LeftImage.</summary>
        internal static readonly StyleInfoProperty LeftImageProperty = Store.CreateStyleInfoProperty(typeof(Image), "LeftImage");

        /// <summary>Declaration of storage propterty: LeftImageIndices.</summary>
        internal static readonly StyleInfoProperty LeftImageIndicesProperty = Store.CreateStyleInfoProperty(typeof(int[]), "LeftImageIndices");

        /// <summary>Declaration of storage propterty: LeftImagePadding.</summary>
        internal static readonly StyleInfoProperty LeftImagePaddingProperty = Store.CreateStyleInfoProperty(typeof(int), "LeftImagePadding");

        /// <summary>Declaration of storage propterty: RightImage.</summary>
        internal static readonly StyleInfoProperty RightImageProperty = Store.CreateStyleInfoProperty(typeof(Image), "RightImage");

        /// <summary>Declaration of storage propterty: RightImageIndices.</summary>
        internal static readonly StyleInfoProperty RightImageIndicesProperty = Store.CreateStyleInfoProperty(typeof(int[]), "RightImageIndices");

        /// <summary>Declaration of storage propterty: RightImagePadding.</summary>
        internal static readonly StyleInfoProperty RightImagePaddingProperty = Store.CreateStyleInfoProperty(typeof(int), "RightImagePadding");

        /// <summary>Declaration of storage propterty: Background.</summary>
        internal static readonly StyleInfoProperty BackgroundProperty = Store.CreateStyleInfoProperty(typeof(BrushInfo), "Background");

        /// <summary>Declaration of storage propterty: BorderSides.</summary>
        internal static readonly StyleInfoProperty BorderSidesProperty = Store.CreateStyleInfoProperty(typeof(Border3DSide), "BorderSides");

        /// <summary>Declaration of storage propterty: BorderStyle.</summary>
        internal static readonly StyleInfoProperty BorderStyleProperty = Store.CreateStyleInfoProperty(typeof(BorderStyle), "BorderStyle");

        /// <summary>Declaration of storage propterty: Border3DStyle.</summary>
        internal static readonly StyleInfoProperty Border3DStyleProperty = Store.CreateStyleInfoProperty(typeof(Border3DStyle), "Border3DStyle");

        /// <summary>Declaration of storage propterty: BorderColor.</summary>
        internal static readonly StyleInfoProperty BorderColorProperty = Store.CreateStyleInfoProperty(typeof(Color), "BorderColor");

        /// <summary>Declaration of storage propterty: BorderSingle.</summary>
        internal static readonly StyleInfoProperty BorderSingleProperty = Store.CreateStyleInfoProperty(typeof(ButtonBorderStyle), "BorderSingle");
        #endregion

        #region Class Initialize/Finalize methods

        public TreeNodeAdvSubItemStyleInfoStore()
            : base()
        {
        }

        public TreeNodeAdvSubItemStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (s_sd.IsEmpty)
            {
                new TreeNodeAdvSubItemStyleInfo();
            }
        }
        #endregion

        #region Class overrides
   
        public override object Clone()
        {
            StyleInfoStore target = new TreeNodeAdvSubItemStyleInfoStore();
            CopyTo(target);
            return target;
        }
        #endregion
    }
}