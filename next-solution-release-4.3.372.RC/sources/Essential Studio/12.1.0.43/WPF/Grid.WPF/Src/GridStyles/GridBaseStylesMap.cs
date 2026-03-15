#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
using System.Windows.Interop;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Controls.Grid
{
    internal class GridBaseStyleIdentity : StyleInfoIdentityBase
    {
        GridBaseStylesMap styleInfoMap;

        /// <override/>
        public override void Dispose()
        {
            styleInfoMap = null;
            GC.SuppressFinalize(this);
        }

        public GridBaseStyleIdentity(GridBaseStylesMap styleInfoMap)
        {
            this.styleInfoMap = styleInfoMap;
        }

        public GridBaseStylesMap BaseStylesMap
        {
            get { return styleInfoMap; }
        }

        /// <override/>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (styleInfoMap != null)
            {
                GridStyleInfo gridStyleInfo = (GridStyleInfo)thisStyleInfo;
                string baseStyleName = gridStyleInfo.HasBaseStyle ? gridStyleInfo.BaseStyle : "";
                int level;
                GridStyleInfo[] gridStyles = styleInfoMap.GetBaseStylesMapStyles(baseStyleName, out level);
                IStyleInfo[] levels = new IStyleInfo[level];
                Array.Copy(gridStyles, levels, level);
                return levels;
            }
            return null;
        }

        /// <override/>
        public override string ToString()
        {
            return "";
        }

        /// <summary>
        /// Results of ToString method.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }

        /// <override/>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
        }
    }


    /// <summary>
    /// GridBaseStyle represents base styles in the grid. A base style has a name and holds the <see cref="GridStyleInfo"/> object
    /// with style information. Cells in the grid can reference a base style with <see cref="GridStyleInfo.BaseStyle"/>.
    /// </summary>
    /// <remarks>
    /// The grid has four system-defined base styles:
    /// <list type="table">
    /// <listheader><term>Name</term><description>Descriptions</description></listheader>
    /// <item><term>Standard</term><description>Holds cell information for all cells.</description></item>
    /// <item><term>Header</term><description>Holds cell information for row and column headers.</description></item>
    /// <item><term>Row Header</term><description>Holds cell information specific to row headers.</description></item>
    /// <item><term>Column Header</term><description>Holds cell information specific to column headers.</description></item>
    /// </list>
    /// You can format each cell in the grid individually, but settings that have not been initialized will be inherited from a base style
    /// whereas the standard style is the style that any cell will inherit information from.
    /// <para/>
    /// You can register custom base styles with the <see cref="GridBaseStylesMap"/> map that you can access with <see cref="GridModel.BaseStylesMap"/>.
    /// <para/>
    /// Base styles themselves can be inherited from other base styles. The "Row Header" base style is derived from the "Header" base style for example.
    /// </remarks>
    /// <example>
    /// The following example shows how to modify base styles and how to add a new custom base style:
    /// <code lang="C#">
    ///     GridStyleInfo standard = model.BaseStylesMap["Standard"].StyleInfo;
    ///     GridStyleInfo header = model.BaseStylesMap["Header"].StyleInfo;
    ///     GridStyleInfo rowHeader = model.BaseStylesMap["Row Header"].StyleInfo;
    ///     GridStyleInfo colHeader = model.BaseStylesMap["Column Header"].StyleInfo;
    /// 
    ///     standard.TextColor = Color.FromArgb(0, 21, 84);
    ///     header.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(238, 234, 216), Color.FromArgb(203, 199, 184));
    ///     rowHeader.Interior = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(238, 234, 216), Color.FromArgb(203, 199, 184));
    ///     standard.Font.Facename = "Helvetica";
    ///     standard.Interior = new BrushInfo(Color.FromArgb(237, 240, 247));
    /// 
    /// 
    ///     GridStyleInfo customStyle = model.BaseStylesMap["Custom Style"].StyleInfo;
    ///     standard.Interior = new BrushInfo(Color.Green);
    ///         model[1, 1].BaseStyle = "Custom Style";
    /// </code>
    /// </example>
    /// <seealso cref="GridBaseStylesMap"/>
    /// <seealso cref="GridStyleInfo"/>
    [
    Serializable,
    // FXM
    // TypeConverter(typeof(GridBaseStyleConverter))
    ]
#if ENABLE_PARTIAL_TRUST
    public class GridBaseStyle : NonFinalizeDisposable, ICloneable
#else
    public class GridBaseStyle : NonFinalizeDisposable, ICloneable,ISerializable
#endif
    {
        string name;
        bool isSystem;
        StyleInfoStore styleInfoStore;
        [NonSerialized]
        GridBaseStylesMap styleInfoMap;

        [NonSerialized]
        GridBaseStyleInfo styleInfo = null;

        /// <overload>
        /// Initializes a new <see cref="GridBaseStyle"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridBaseStyle"/> object.
        /// </summary>
        public GridBaseStyle()
        {
            this.isSystem = false;
            this.styleInfoStore = null;
        }

        /// <summary>
        /// Initializes a new <see cref="GridBaseStyle"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridBaseStyle(SerializationInfo info, StreamingContext context)
        {
            this.name = info.GetString("Name");
            this.isSystem = info.GetBoolean("IsSystem");
            object typeObj = info.GetValue("Type", typeof(object));//, typeof(Type));
            Type type;
            if (typeObj is string)
                type = ValueConvert.GetType((string)typeObj);
            else
                type = (Type)typeObj;
            this.styleInfoStore = (StyleInfoStore)info.GetValue("Store", type);
            // TODO: styleInfoMap
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridBaseStyle"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
#if !ENABLE_PARTIAL_TRUST
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", name);
            info.AddValue("IsSystem", isSystem);
            info.AddValue("Type", ValueConvert.GetTypeName(styleInfoStore.GetType()));
            info.AddValue("Store", styleInfoStore);
        }
#endif
        /// <summary>
        /// Initializes a new <see cref="GridBaseStyle"/> with name and style information and associates it with a base styles map.
        /// </summary>
        /// <param name="name">The name of the base style.</param>
        /// <param name="isSystem">True if this is a system base style that cannot be deleted; False otherwise.</param>
        /// <param name="styleInfoStore">A <see cref="GridStyleInfoStore"/> with style settings.</param>
        /// <param name="styleInfoMap">The <see cref="GridBaseStylesMap"/> for this base style.</param>
        public GridBaseStyle(string name, bool isSystem, GridStyleInfoStore styleInfoStore, GridBaseStylesMap styleInfoMap)
        {
            this.name = name;
            this.isSystem = isSystem;
            this.styleInfoStore = styleInfoStore;
            this.styleInfoMap = styleInfoMap;
        }

        /// <summary>
        /// Initializes a new <see cref="GridBaseStyle"/> with name and style information.
        /// </summary>
        /// <param name="name">The name of the base style.</param>
        /// <param name="isSystem">True if this is a system base style that cannot be deleted; False otherwise.</param>
        /// <param name="styleInfoStore">A <see cref="GridStyleInfoStore"/> with style settings.</param>
        public GridBaseStyle(string name, bool isSystem, GridStyleInfoStore styleInfoStore)
            : this(name, isSystem, styleInfoStore, null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridBaseStyle"/> with name and style information.
        /// </summary>
        /// <param name="name">The name of the base style.</param>
        /// <param name="isSystem">True if this is a system base style that cannot be deleted; False otherwise.</param>
        /// <param name="styleInfo">A <see cref="GridStyleInfo"/> with style settings.</param>
        public GridBaseStyle(string name, bool isSystem, GridStyleInfo styleInfo)
            : this(name, isSystem, (GridStyleInfoStore)styleInfo.Store, null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridBaseStyle"/> with a name.
        /// </summary>
        /// <param name="name">The name of the base style.</param>
        /// <param name="isSystem">True if this is a system base style that cannot be deleted; False otherwise.</param>
        public GridBaseStyle(string name, bool isSystem)
            : this(name, isSystem, null, null)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (styleInfoMap != null)
                {
                    if (styleInfoMap.baseStyles.ContainsValue(this))
                        styleInfoMap.baseStyles.Remove(name);
                }

                if (styleInfo != null)
                {
                    styleInfo.Changed -= new StyleChangedEventHandler(OnStyleInfoChanged);
                    this.styleInfo.Dispose();
                }

                if (styleInfoStore != null)
                    styleInfoStore.Dispose();

                styleInfoMap = null;
                styleInfo = null;
                styleInfoStore = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates a new <see cref="GridBaseStyle"/> object and copies all properties from this object.
        /// </summary>
        /// <returns>The new created <see cref="GridBaseStyle"/> object.</returns>
        public object Clone()
        {
            return new GridBaseStyle(name, isSystem, (GridStyleInfoStore)styleInfoStore.Clone(), styleInfoMap);
        }

        /// <override/>
        /// <summary>Returns the name of the base style.</summary>
        /// <returns>Base style name.</returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// The base style name.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!isSystem && name != value)
                {
                    if (styleInfoMap != null)
                    {
                        styleInfoMap.baseStyles.Remove(name);
                        name = value;
                        styleInfoMap[name] = this;
                    }
                    else
                        name = value;
                }
            }
        }

        /// <summary>
        /// Indicates if this a system style that cannot be removed.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSystem
        {
            get { return isSystem; }
        }

        /// <summary>
        /// The <see cref="GridBaseStylesMap"/> this base style belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridBaseStylesMap BaseStylesMap
        {
            get
            {
                return styleInfoMap;
            }
            set
            {
                if (styleInfoMap != value)
                {
                    styleInfoMap = value;
                    StyleInfo = null;
                }
            }
        }


        void OnStyleInfoChanged(object sender, StyleChangedEventArgs e)
        {
            if (styleInfoMap != null)
            {
                styleInfoMap.Add(this);
                styleInfoMap.OnStyleChanged(this);
            }
        }

        /// <summary>
        /// The <see cref="GridStyleInfo"/> with style settings for this base style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo"/> returned by this property is simply a wrapper for
        /// the settings in <see cref="StyleInfoStore"/>.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridStyleInfo StyleInfo
        {
            get
            {
                if (styleInfo == null)
                {
                    styleInfo = new GridBaseStyleInfo(new GridBaseStyleIdentity(this.styleInfoMap), StyleInfoStore);
                    styleInfo.Changed += new StyleChangedEventHandler(OnStyleInfoChanged);
                }

                return styleInfo;
            }
            set
            {
                if (value != styleInfo)
                {
                    if (styleInfo != null)
                        styleInfo.Changed -= new StyleChangedEventHandler(OnStyleInfoChanged);

                    styleInfo = null;
                    if (value != null)
                    {
                        styleInfoStore = value.Store;
                    }
                }
            }
        }

        bool ShouldSerializeStyleInfo()
        {
            return styleInfo != null && styleInfo.Store != null && !styleInfo.IsEmpty;
        }

        /// <summary>
        /// The <see cref="GridStyleInfoStore"/> with style settings for this base style.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        [System.Xml.Serialization.XmlIgnore]
        public GridStyleInfoStore StyleInfoStore
        {
            get
            {
                if (styleInfoStore == null)
                    this.styleInfoStore = new GridStyleInfoStore();
                return (GridStyleInfoStore)styleInfoStore;
            }
            set
            {
                if (value != styleInfoStore)
                {
                    styleInfoStore = value;
                    if (styleInfo != null)
                    {
                        styleInfo.Changed -= new StyleChangedEventHandler(OnStyleInfoChanged);
                        styleInfo.Dispose();
                    }
                    styleInfoStore.Dispose();
                    styleInfo = null;
                }
            }
        }
    }

    /// <summary>
    /// GridBaseStylesMap holds a collection of base styles for a grid and lets you add, remove, and change base styles.
    /// </summary>
    /// <remarks>
    /// A base style has a name and holds the <see cref="GridStyleInfo"/> object
    /// with style information. Cells in the grid can reference a base style with <see cref="GridStyleInfo.BaseStyle"/>.
    /// <para/>
    /// The grid has four system-defined base styles as discussed in <see cref="GridBaseStyle"/> overview. The
    /// <see cref="GridBaseStylesMap.RegisterStandardStyles"/> method adds these system styles to the collection.
    /// <para/>
    /// You can register custom base styles with the <see cref="GridBaseStylesMap"/> map that you can access with <see cref="GridModel.BaseStylesMap"/>.
    /// <para/>
    /// Use <see cref="ShowGridBaseStylesMapDialog"/> to let users customize base styles at run-time.
    /// </remarks>
    /// <example>
    /// The following example shows how to modify base styles and how to add a new custom base style:
    /// <code lang="C#">
    ///     GridStyleInfo standard = model.BaseStylesMap["Standard"].StyleInfo;
    ///     GridStyleInfo header = model.BaseStylesMap["Header"].StyleInfo;
    ///     GridStyleInfo rowHeader = model.BaseStylesMap["Row Header"].StyleInfo;
    ///     GridStyleInfo colHeader = model.BaseStylesMap["Column Header"].StyleInfo;
    /// 
    ///     standard.TextColor = Color.FromArgb(0, 21, 84);
    ///     header.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(238, 234, 216), Color.FromArgb(203, 199, 184));
    ///     rowHeader.Interior = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(238, 234, 216), Color.FromArgb(203, 199, 184));
    ///     standard.Font.Facename = "Helvetica";
    ///     standard.Interior = new BrushInfo(Color.FromArgb(237, 240, 247));
    /// 
    /// 
    ///     GridStyleInfo customStyle = model.BaseStylesMap["Custom Style"].StyleInfo;
    ///     standard.Interior = new BrushInfo(Color.Green);
    ///         model[1, 1].BaseStyle = "Custom Style";
    /// </code>
    /// </example>
    /// <seealso cref="GridBaseStyle"/>
    /// <seealso cref="GridStyleInfo"/>
    [
    Serializable
    ]
#if ENABLE_PARTIAL_TRUST
    public class GridBaseStylesMap : NonFinalizeDisposable, ICloneable, ICollection, IDisposable
#else
    public class GridBaseStylesMap : NonFinalizeDisposable, ICloneable,ISerializable, ICollection, IDisposable
#endif
    {
        internal Hashtable baseStyles;
        [NonSerialized]
        private ArrayList m_params = null;
        [NonSerialized]
        int nameCounter = -1;
        [NonSerialized]
        bool modified = false;

        [NonSerialized]
        ArrayList cellTypes = new ArrayList();

        /// <overload>
        /// Initializes a new <see cref="GridBaseStylesMap"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridBaseStylesMap"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridBaseStylesMap(SerializationInfo info, StreamingContext context)
        {
            baseStyles = new Hashtable();

            bool mod = true;
            SerializationInfoEnumerator sie = info.GetEnumerator();
            while (sie.MoveNext())
            {
                if (sie.Name == "#&Modified")
                {
                    mod = Convert.ToBoolean(sie.Value);
                }
                else
                {
                    GridBaseStyle bs = (GridBaseStyle)sie.Value as GridBaseStyle;
                    bs.BaseStylesMap = this;
                    baseStyles.Add(sie.Name, sie.Value as GridBaseStyle);
                }
                this.Modified = mod;
            }
        }

        /// <override/>
        /// <summary>Gets the string equivalent of the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return GetType().Name + "{ Count=" + this.baseStyles.Count + " }";
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridBaseStylesMap"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>

#if !ENABLE_PARTIAL_TRUST
#if !SyncfusionFramework4_0
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
#else
        [SecurityCritical]
#endif
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (GridBaseStyle bs in baseStyles.Values)
            {
                info.AddValue(bs.Name, bs);
            }
            info.AddValue("#&Modified", this.modified);
        }
#endif

        /// <summary>
        /// Returns an <see cref="ArrayList"/> will cell type names as strings.
        /// </summary>
        public ArrayList CellTypes
        {
            get { return cellTypes; }
        }

        /// <summary>
        /// Initializes a new <see cref="GridBaseStylesMap"/>.
        /// </summary>
        public GridBaseStylesMap()
        {
            baseStyles = new Hashtable();

            string[] types = new string[] 
				{
						"Header", "Static", "TextBox", "CheckBox",
					"PushButton", "NumericUpDown", "DropDownGrid", 
					"GridListControl", "ComboBox", "ColorEdit", "MonthCalendar", "FormulaCell",
					"MaskEdit", "Currency", "Image",
					"RichText", "Control", "OriginalTextBox", "ProgressBar", "RadioButton"

				};

            foreach (string cell in types)
                cellTypes.Add(cell);
        }

        /// <summary>
        /// Initializes a new <see cref="GridBaseStylesMap"/> and copies an array of base styles.
        /// </summary>
        /// <param name="baseStyles">An array with <see cref="GridBaseStyle"/> styles.</param>
        public GridBaseStylesMap(GridBaseStyle[] baseStyles)
        {
            this.baseStyles = new Hashtable();
            foreach (GridBaseStyle baseStyle in baseStyles)
                this[baseStyle.Name] = baseStyle;
        }

        /// <summary>
        /// Creates a new instance <see cref="GridBaseStylesMap"/> and initializes it with all base styles from this collection.
        /// </summary>
        /// <returns>A duplicate of the current object.</returns>
        public object Clone()
        {
            GridBaseStylesMap clone = new GridBaseStylesMap();
            foreach (GridBaseStyle baseStyle in this)
                clone[baseStyle.Name] = baseStyle;
            clone.Modified = Modified;
            return clone;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (GridBaseStyle baseStyle in this)
                    baseStyle.BaseStylesMap = null;
                baseStyles.Clear();
                if (this.cellTypes != null)
                {
                    this.cellTypes.Clear();
                    this.cellTypes = null;
                }  
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Copies a base style and all base styles it depends on into an array.
        /// </summary>
        /// <param name="name">The name of the base style.</param>
        /// <param name="level">The maximum number of levels to look at when walking referenced
        /// base styles.</param>
        /// <returns>An array of <see cref="GridBaseStyle"/> objects with the first base style and all base styles it depends on.</returns>
        public GridStyleInfo[] GetBaseStylesMapStyles(string name, out int level)
        {
            GridStyleInfo[] styleLevels = new GridStyleInfo[16];

            bool exitLoop = false;
            level = 0;
            while (level < 16)
            {
                if (this.baseStyles.ContainsKey(name))
                {
                    GridBaseStyle baseStyle = this[name];
                    GridStyleInfo style = baseStyle.StyleInfo;
                    styleLevels[level++] = style;
                    if (name == "Standard")
                        return styleLevels;
                    else if (style.HasBaseStyle)
                    {
                        name = style.BaseStyle;
                        continue;
                    }
                }
                if (exitLoop)
                    break;
                name = "Standard";
                exitLoop = true;
            }

            return styleLevels;
        }


        [NonSerialized]
        int updateCount = 0;
        [NonSerialized]
        bool changePending = false;

        internal void BeginUpdate()
        {
            updateCount++;
        }

        internal void EndUpdate(bool notifyPending)
        {
            if (--updateCount == 0 && changePending)
            {
                if (notifyPending)
                    OnStyleChanged(null);
                changePending = false;
            }
        }

        [Browsable(false)]
        internal bool Updating
        {
            get
            {
                return updateCount > 0;
            }
        }

        internal void OnStyleAdded(GridBaseStyle baseStyle)
        {
            Modified = true;
            if (Updating)
                changePending = true;
        }

        internal void OnStyleChanged(GridBaseStyle baseStyle)
        {
            Modified = true;
            if (Updating)
                changePending = true;

            // FXM
            //foreach (GridBaseStyle gbs in this)
            //{
            //    if (gbs.StyleInfo.HasFont)
            //        gbs.StyleInfo.Font.ResetGdipFont();
            //}
        }

        internal void OnStyleRemoved(string name)
        {
            Modified = true;
            if (Updating)
                changePending = true;
        }

        /// <summary>
        /// Adds "Standard", "Header", "Row Header", and "Column Header" base styles.
        /// </summary>
        public void RegisterStandardStyles()
        {
            BeginUpdate();

            this.baseStyles.Clear();

            // Standard
            GridBaseStyle standardStyle = new GridBaseStyle("Standard", true);
            standardStyle.BaseStylesMap = this;
            Add(standardStyle);

            // Header
            GridBaseStyle headerStyle = new GridBaseStyle("Header", true);
            headerStyle.StyleInfo.CellType = "Header";
            headerStyle.BaseStylesMap = this;
            Add(headerStyle);

            // Column Header
            GridBaseStyle colHeaderStyle = new GridBaseStyle("Column Header", true);
            colHeaderStyle.BaseStylesMap = this;
            Add(colHeaderStyle);

            // Row Header
            GridBaseStyle rowHeaderStyle = new GridBaseStyle("Row Header", true);
            rowHeaderStyle.BaseStylesMap = this;
            Add(rowHeaderStyle);

            EndUpdate(true);
            Modified = false;
        }

        /// <summary>
        /// Property Modified (bool)
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool Modified
        {
            get
            {
                return this.modified;
            }
            set
            {
                this.modified = value;
            }
        }

        internal bool inCollectionEditorEditValue = false;

        /// <internalonly/>
        /// <summary>For internal use.</summary>
        public bool InCollectionEditor
        {
            get
            {
                return inCollectionEditorEditValue;
            }
        }


        /// <summary>
        /// Gets a suggestion for a new base style name, e.g. when the user adds a new
        /// base style in the <see cref="GridBaseStyleCollectionEditor"/>.
        /// </summary>
        /// <returns>A string for a new base style name.</returns>
        public string GetNewBaseStyleName()
        {
            if (nameCounter == -1)
            {
                foreach (GridBaseStyle gbs in this)
                {
                    if (gbs.Name.StartsWith("BaseStyle"))
                    {
                        try
                        {
                            nameCounter = Math.Max(nameCounter, int.Parse(gbs.Name.Substring("BaseStyle".Length)));
                        }
                        catch (Exception ex)
                        {
                            if (!BrowserInteropHelper.IsBrowserHosted)
                            {
                                TraceUtil.TraceExceptionCatched(ex);
                            }
                            if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                throw;
                        }
                    }
                }
            }

            nameCounter++;
            return "BaseStyle" + nameCounter.ToString();
        }

        /// <summary>
        /// The <see cref="GridBaseStyle"/> for the specified name.
        /// </summary>
        [Browsable(false)]
        public GridBaseStyle this[string name]
        {
            get
            {
                if (baseStyles.Contains(name))
                {
                    GridBaseStyle gbs = (GridBaseStyle)baseStyles[name];
                    gbs.BaseStylesMap = this;
                    return gbs;
                }
                else
                    // GC will collect this if no changes are made. If changes
                    // are made, GridBaseStyle.OnStyleChanged will add style to stylesmap
                    return new GridBaseStyle(name, false, null, this);
            }
            set
            {
                GridBaseStyle currentStyle = null;
                if (value.BaseStylesMap != this)
                {
                    // Store a copy of style object if it is not owned by the stylesmap.
                    baseStyles[name] = currentStyle = new GridBaseStyle(value.Name, value.IsSystem,
                        (GridStyleInfoStore)value.StyleInfoStore.Clone(), this);
                    OnStyleAdded(currentStyle);
                }
                else
                {
                    if (baseStyles.Contains(name))
                        currentStyle = this[name];
                    if (currentStyle != value)
                    {
                        baseStyles[name] = currentStyle = value;
                        if (currentStyle == null)
                            OnStyleAdded(currentStyle);
                    }
                }
            }
        }

        /// <summary>
        /// Gets / sets a <see cref="GridBaseStyle"/> at the specified index.
        /// </summary>
        [Browsable(false)]
        public GridBaseStyle this[int index]
        {
            get
            {
                return (GridBaseStyle)baseStyles[index];
            }
            set
            {
                Add(value);
            }
        }

        /// <summary>
        /// Adds a base style to the <see cref="GridBaseStylesMap"/>.
        /// </summary>
        /// <param name="baseStyle">The base style to be added.</param>
        public void Add(GridBaseStyle baseStyle)
        {
            GridBaseStyle oldStyle = this[baseStyle.Name];
            if (oldStyle == baseStyle)
                return;
            else if (oldStyle != null)
                oldStyle.BaseStylesMap = null;
            this[baseStyle.Name] = baseStyle;
            OnStyleAdded(baseStyle);
        }

        /// <summary>
        /// Removes the base style with the specified name.
        /// </summary>
        /// <param name="name">The name of the base style to be removed.</param>
        void Remove(string name)
        {
            if (baseStyles.ContainsKey(name))
            {
                baseStyles.Remove(name);
                OnStyleRemoved(name);
            }
        }


        /// <summary>
        /// Adds a range of base styles to the <see cref="GridBaseStylesMap"/>.
        /// </summary>
        /// <param name="value"></param>
        public void AddRange(GridBaseStyle[] value)
        {
            BeginUpdate();
            foreach (object o in value)
            {
                GridBaseStyle style = o as GridBaseStyle;
                if (style == null)
                {
                    continue;
                }

                this[style.Name] = style;
            }
            EndUpdate(true);
        }

        /// <summary>
        /// The number of base styles in the <see cref="GridBaseStylesMap"/>.
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get
            {
                return baseStyles.Count;
            }
        }


        /// <summary>
        /// Gets an object that can be used to synchronize access to base styles table.
        /// </summary>
        [Browsable(false)]
        public object SyncRoot
        {
            get
            {
                return baseStyles.SyncRoot;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the base styles collection is Read-only.
        /// </summary>
        [Browsable(false)]
        public bool IsReadOnly
        {
            get
            {
                return baseStyles.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the base styles collection is synchronized.
        /// </summary>
        [Browsable(false)]
        public bool IsSynchronized
        {
            get
            {
                return baseStyles.IsSynchronized;
            }
        }

        /// <overload>
        /// Copies the base style collection elements to a one-dimensional Array instance at the specified index.
        /// </overload>
        /// <summary>
        /// Copies the base style collection elements to a one-dimensional Array instance at the specified index.
        /// </summary>
        /// <param name="array">An array to which the base style collection has to be copied to.</param>
        /// <param name="index">The index at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            baseStyles.CopyTo(array, index);
        }

        /// <summary>
        /// Copies all base styles into an array of <see cref="GridBaseStyle"/> starting at specified index.
        /// </summary>
        /// <param name="values">The array of <see cref="GridBaseStyle"/> where the values should be copied to.</param>
        /// <param name="index">The starting index in the destination array.</param>
        public void CopyTo(GridBaseStyle[] values, int index)
        {
            baseStyles.Values.CopyTo(values, index);
        }


        /// <summary>
        /// Returns an enumerator that can enumerate through the base styles object in this collection.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return baseStyles.Values.GetEnumerator();
        }

        [Browsable(false)]
        ArrayList Params
        {
            get
            {
                if (m_params == null)
                    m_params = new ArrayList();
                return m_params;
            }
        }

        void IDisposable.Dispose()
        {
            
        }
    }

    /// <summary>
    /// Provides information about the base styles.
    /// </summary>
    [TypeConverter(typeof(StyleInfoBaseConverter))]
    class GridBaseStyleInfo : GridStyleInfo
    {
        /// <summary>
        /// Initializes a new <see cref="GridBaseStyleInfo"/>.
        /// </summary>
        /// <param name="identity">Style info identity.</param>
        /// <param name="store">Style info store.</param>
        public GridBaseStyleInfo(StyleInfoIdentityBase identity, GridStyleInfoStore store)
            : base(identity, store)
        {
        }

    }


    #region "'GridBaseStyleCollection' strongly typed collection class"


    /// <summary>
    /// A collection that stores 'GridBaseStyle' objects.
    /// </summary>
    public class GridBaseStyleCollection : System.Collections.CollectionBase
    {

        /// <summary>
        ///     Initializes a new instance of 'GridBaseStyleCollection'.
        /// </summary>
        public GridBaseStyleCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of 'GridBaseStyleCollection' based on an already existing instance.
        /// </summary>
        /// <param name='griValue'>
        ///     A 'GridBaseStyleCollection' from which the contents is copied
        /// </param>
        public GridBaseStyleCollection(GridBaseStyleCollection griValue)
        {
            this.AddRange(griValue);
        }

        /// <summary>
        ///     Initializes a new instance of 'GridBaseStyleCollection' with an array of 'GridBaseStyle' objects.
        /// </summary>
        /// <param name='griValue'>
        ///     An array of 'GridBaseStyle' objects with which to initialize the collection
        /// </param>
        public GridBaseStyleCollection(GridBaseStyle[] griValue)
        {
            this.AddRange(griValue);
        }

        /// <summary>
        ///     Represents the 'GridBaseStyle' item at the specified index position.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index of the entry to locate in the collection.
        /// </param>
        /// <value>
        ///     The entry at the specified index of the collection.
        /// </value>
        public GridBaseStyle this[int intIndex]
        {
            get
            {
                return ((GridBaseStyle)(List[intIndex]));
            }
            set
            {
                List[intIndex] = value;
            }
        }

        /// <summary>
        ///     Adds a 'GridBaseStyle' item with the specified value to the 'GridBaseStyleCollection'
        /// </summary>
        /// <param name='griValue'>
        ///     The 'GridBaseStyle' to add.
        /// </param>
        /// <returns>
        ///     The index at which the new element was inserted.
        /// </returns>
        public int Add(GridBaseStyle griValue)
        {
            return List.Add(griValue);
        }

        /// <summary>
        ///     Copies the elements of an array at the end of this instance of 'GridBaseStyleCollection'.
        /// </summary>
        /// <param name='griValue'>
        ///     An array of 'GridBaseStyle' objects to add to the collection.
        /// </param>
        public void AddRange(GridBaseStyle[] griValue)
        {
            for (int intCounter = 0; (intCounter < griValue.Length); intCounter = (intCounter + 1))
            {
                this.Add(griValue[intCounter]);
            }
        }

        /// <summary>
        ///     Adds the contents of another 'GridBaseStyleCollection' at the end of this instance.
        /// </summary>
        /// <param name='griValue'>
        ///     A 'GridBaseStyleCollection' containing the objects to add to the collection.
        /// </param>
        public void AddRange(GridBaseStyleCollection griValue)
        {
            for (int intCounter = 0; (intCounter < griValue.Count); intCounter = (intCounter + 1))
            {
                this.Add(griValue[intCounter]);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the 'GridBaseStyleCollection' contains the specified value.
        /// </summary>
        /// <param name='griValue'>
        ///     The item to locate.
        /// </param>
        /// <returns>
        ///     True if the item exists in the collection; false otherwise.
        /// </returns>
        public bool Contains(GridBaseStyle griValue)
        {
            return List.Contains(griValue);
        }

        /// <summary>
        ///     Copies the 'GridBaseStyleCollection' values to a one-dimensional System.Array
        ///     instance starting at the specified array index.
        /// </summary>
        /// <param name='griArray'>
        ///     The one-dimensional System.Array that represents the copy destination.
        /// </param>
        /// <param name='intIndex'>
        ///     The index in the array where copying begins.
        /// </param>
        public void CopyTo(GridBaseStyle[] griArray, int intIndex)
        {
            List.CopyTo(griArray, intIndex);
        }

        /// <summary>
        ///     Returns the index of a 'GridBaseStyle' object in the collection.
        /// </summary>
        /// <param name='griValue'>
        ///     The 'GridBaseStyle' object whose index will be retrieved.
        /// </param>
        /// <returns>
        ///     If found, the index of the value; otherwise, -1.
        /// </returns>
        public int IndexOf(GridBaseStyle griValue)
        {
            return List.IndexOf(griValue);
        }

        /// <summary>
        ///     Inserts an existing 'GridBaseStyle' into the collection at the specified index.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index where the new item should be inserted.
        /// </param>
        /// <param name='griValue'>
        ///     The item to insert.
        /// </param>
        public void Insert(int intIndex, GridBaseStyle griValue)
        {
            List.Insert(intIndex, griValue);
        }

        /// <summary>
        ///     Returns an enumerator that can be used to iterate through
        ///     the 'GridBaseStyleCollection'.
        /// </summary>
        public new GridBaseStyleEnumerator GetEnumerator()
        {
            return new GridBaseStyleEnumerator(this);
        }

        /// <summary>
        ///     Removes a specific item from the 'GridBaseStyleCollection'.
        /// </summary>
        /// <param name='griValue'>
        ///     The item to remove from the 'GridBaseStyleCollection'.
        /// </param>
        public void Remove(GridBaseStyle griValue)
        {
            List.Remove(griValue);
        }

        /// <summary>
        ///     A strongly typed enumerator for 'GridBaseStyleCollection'
        /// </summary>
        public class GridBaseStyleEnumerator : object, System.Collections.IEnumerator
        {

            private System.Collections.IEnumerator iEnBase;

            private System.Collections.IEnumerable iEnLocal;

            /// <summary>
            ///     Enumerator constructor
            /// </summary>
            /// <param name="griMappings">Base styles.</param>
            public GridBaseStyleEnumerator(GridBaseStyleCollection griMappings)
            {
                this.iEnLocal = ((System.Collections.IEnumerable)(griMappings));
                this.iEnBase = iEnLocal.GetEnumerator();
            }

            /// <summary>
            ///     Gets the current element from the collection (strongly typed)
            /// </summary>
            public GridBaseStyle Current
            {
                get
                {
                    return ((GridBaseStyle)(iEnBase.Current));
                }
            }

            /// <summary>
            ///     Gets the current element from the collection
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return iEnBase.Current;
                }
            }

            /// <summary>
            ///     Advances the enumerator to the next element of the collection
            /// </summary>
            public bool MoveNext()
            {
                return iEnBase.MoveNext();
            }

            /// <summary>
            ///     Advances the enumerator to the next element of the collection
            /// </summary>
            bool System.Collections.IEnumerator.MoveNext()
            {
                return iEnBase.MoveNext();
            }

            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            public void Reset()
            {
                iEnBase.Reset();
            }

            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            void System.Collections.IEnumerator.Reset()
            {
                iEnBase.Reset();
            }
        }
    }

    #endregion //('GridBaseStyleCollection' strongly typed collection class)

}

