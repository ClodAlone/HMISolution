#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.JavaScript.Shared.Serializer;
using System.ComponentModel;

namespace Syncfusion.JavaScript
{
    public class PropertiesBase
    {
        public PropertiesBase() { }

        #region Fields

        //Events 
        private String create = null;
        private String destroy = null;

        #endregion Fields

        #region Properties
        //Events 
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion Properties
    }

    public class ButtonPropertiesBase : PropertiesBase
    {

        public ButtonPropertiesBase() { }

        #region Fields
        //Boolean Values
        private bool enabled = true;
        //Enumeration Values
        private Contents contentType = Contents.TextOnly;
        private ImagePositions imagePosition = ImagePositions.ImageLeft;

        //String Values
        private String text = null;
        private String cssClass = "";
        #endregion Fields


        #region Properties

        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("contentType")]
        [DefaultValue(Contents.TextOnly)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Contents ContentType
        {
            get { return this.contentType; }
            set { this.contentType = value; }
        }
        [JsonProperty("imagePosition")]
        [DefaultValue(ImagePositions.ImageLeft)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ImagePositions ImagePosition
        {
            get { return this.imagePosition; }
            set { this.imagePosition = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("text")]
        [DefaultValue(null)]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }


        #endregion Properties
    }

    public class DatePickerPropertiesBase : PropertiesBase
    {
        public DatePickerPropertiesBase() { }
        #region Fields
        private string localize = "en-US";
        private bool enabled = true;
        private string cssClass = "";
        #endregion Fields
        #region Properties

        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("localize")]
        [DefaultValue("en-US")]
        public string Localize
        {
            get { return this.localize; }
            set { this.localize = value; }
        }


        #endregion Properties
    }

    public class TimePickerPropertiesBase : PropertiesBase
    {
        public TimePickerPropertiesBase() { }
        #region Fields
        private bool enabled = true;
        private string cssClass = "";
        #endregion Fields
        #region Properties

        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        #endregion Properties
    }

    public class AutoCompletePropertiesBase : PropertiesBase
    {
        public AutoCompletePropertiesBase() { }

        #region Fields
        private bool allowSorting = true;
        private string noResults = "No suggestions";
        private bool showNoResults = true;
        private int minCharacter = 1;
        private bool enabled = true;
        private string cssClass = "";
        #endregion Fields
        #region Properties

        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [case sensitive].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [case sensitive]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("caseSensitive")]
        [DefaultValue(false)]
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic fill].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic fill]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("autoFill")]
        [DefaultValue(false)]
        public bool AutoFill { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [multi value].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [multi value]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("multiValue")]
        [DefaultValue(false)]
        public bool MultiValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow sorting].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow sorting]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("allowSorting")]
        [DefaultValue(true)]
        public bool AllowSorting { get { return allowSorting; } set { allowSorting = value; } }

        /// <summary>
        /// Gets or sets the no results.
        /// </summary>
        /// <value>
        /// The no results.
        /// </value>
        [JsonProperty("noResults")]
        [DefaultValue("No suggestions")]
        public string NoResults { get { return noResults; } set { noResults = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show no results].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show no results]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showNoResults")]
        [DefaultValue(true)]
        public bool ShowNoResults { get { return showNoResults; } set { showNoResults = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileAutoCompletePropertiesBuilder"/> is distinct.
        /// </summary>
        /// <value>
        ///   <c>true</c> if distinct; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("distinct")]
        [DefaultValue(false)]
        public bool Distinct { get; set; }

        /// <summary>
        /// Gets or sets the minimum character.
        /// </summary>
        /// <value>
        /// The minimum character.
        /// </value>
        [JsonProperty("minCharacter")]
        [DefaultValue(1)]
        public int MinCharacter { get { return minCharacter; } set { minCharacter = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileAutoCompletePropertiesBuilder"/> is persist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if persist; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist { get; set; }


        #endregion Properties
    }

    public class SliderPropertiesBase : PropertiesBase
    {

        public SliderPropertiesBase() { }

        #region Fields

        //Int Values
        private int startValue = 0;
        private int endValue = 100;
        private int step = 1;
        private int animationSpeed = 500;

        //String Values
        private string cssClass = "";
        private string start = null;
        private string stop = null;
        private string load = null;
        private string slide = null;
        private string chnage = null;

        //Boolean Values
        private bool range = false;
        private bool animate = false;
        private bool readOnly = false;
        private bool persist = false;
        private bool enabled = true;

        //Enumeration Values
        private Orientation orientation = Orientation.Horizontal;

        #endregion Fields


        #region Properties

        /// <summary>
        /// Gets or sets the start value.
        /// </summary>
        /// <value>
        /// The start value.
        /// </value>
        [JsonProperty("startValue")]
        [DefaultValue(0)]
        public int StartValue { get; set; }



        /// <summary>
        /// Gets or sets the end value.
        /// </summary>
        /// <value>
        /// The end value.
        /// </value>
        [JsonProperty("endValue")]
        [DefaultValue(100)]
        public int EndValue { get { return endValue; } set { endValue = value; } }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileSliderProperties"/> is range.
        /// </summary>
        /// <value>
        ///   <c>true</c> if range; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("range")]
        [DefaultValue(false)]
        public bool Range { get; set; }



        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileSliderProperties"/> is animate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if animate; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("animate")]
        [DefaultValue(false)]
        public bool Animate { get; set; }


        /// <summary>
        /// Gets or sets the animation speed.
        /// </summary>
        /// <value>
        /// The animation speed.
        /// </value>
        [JsonProperty("animationSpeed")]
        [DefaultValue(500)]
        public int AnimationSpeed { get { return animationSpeed; } set { animationSpeed = value; } }


        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [read only]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly { get; set; }


        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        /// <value>
        /// The step.
        /// </value>
        [JsonProperty("step")]
        [DefaultValue(1)]
        public int Step { get { return step; } set { step = value; } }


        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass { get { return cssClass; } set { cssClass = value; } }





        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        [JsonProperty("start")]
        public string Start { get; set; }


        /// <summary>
        /// Gets or sets the stop.
        /// </summary>
        /// <value>
        /// The stop.
        /// </value>
        [JsonProperty("stop")]
        public string Stop { get; set; }

        /// <summary>
        /// Gets or sets the slide.
        /// </summary>
        /// <value>
        /// The slide.
        /// </value>
        [JsonProperty("slide")]
        public string Slide { get; set; }


        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        /// <value>
        /// The change.
        /// </value>
        [JsonProperty("change")]
        public string Change { get; set; }

        /// <summary>
        /// Gets or sets the load.
        /// </summary>
        /// <value>
        /// The load.
        /// </value>
        [JsonProperty("load")]
        public string Load { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileSliderProperties"/> is persist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if persist; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("persist")]
        public bool Persist { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        [JsonProperty("orientation")]
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation { get { return orientation; } set { orientation = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileProgressBarProperties"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        #endregion Properties

    }

    public class AccordionPropertiesBase : PropertiesBase
    {
        public AccordionPropertiesBase() { }

        #region Fields

        //Boolean Values
        private bool collapsible;
        private bool enabled;
        private bool persist;
        private bool multipleOpen;

        //String Values
        private string cssClass;

        //Enumeration Values
        private HeightStyle heightStyle = HeightStyle.Content;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AccordionPropertiesBase"/> is collapsible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if collapsible; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("collapsible")]
        [DefaultValue(false)]
        public bool Collapsible { get { return collapsible; } set { collapsible = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AccordionPropertiesBase"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(false)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AccordionPropertiesBase"/> is persist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if persist; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist { get { return persist; } set { persist = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [multiple open].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [multiple open]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("multipleOpen")]
        [DefaultValue(false)]
        public bool MultipleOpen { get { return multipleOpen; } set { multipleOpen = value; } }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass { get { return cssClass; } set { cssClass = value; } }

        /// <summary>
        /// Gets or sets the height style.
        /// </summary>
        /// <value>
        /// The height style.
        /// </value>
        [JsonProperty("heightStyle")]
        [DefaultValue(HeightStyle.Content)]
        [JsonConverter(typeof(StringEnumConverter))]
        public HeightStyle HeightStyle { get { return heightStyle; } set { heightStyle = value; } }
        #endregion
    }
    public class DialogPropertiesBase : PropertiesBase
    {
        public DialogPropertiesBase() { }

        #region Fields
        //Boolean Values
        private bool autoOpen = true;
        private bool modal = false;
        //String Values
        private String title = null;
        private String cssClass = "";
        #endregion Fields

        #region Properties

        [JsonProperty("autoOpen")]       
        public bool AutoOpen
        {
            get { return this.autoOpen; }
            set { this.autoOpen = value; }
        }
        [JsonProperty("modal")]
        [DefaultValue(false)]
        public bool Modal
        {
            get { return this.modal; }
            set { this.modal = value; }
        }
        [JsonProperty("title")]
        [DefaultValue(null)]
        public String Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }

        #endregion Properties
    }
    public class RotatorPropertiesBase : PropertiesBase
    {
        public RotatorPropertiesBase() { }

        #region Fields
        //Boolean Values
        private bool pager = true;
        //Object Values
        private object dataSource = new object();
        //String Values
        private String cssClass = "";
        #endregion Fields

        #region Properties

        [JsonProperty("dataSource")]
        [DefaultValue(null)]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }
        [JsonProperty("pager")]
        [DefaultValue(true)]
        public bool Pager
        {
            get { return this.pager; }
            set { this.pager = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }

        #endregion Properties
    }
    public class TextBoxPropertiesBase : PropertiesBase
    {

        public TextBoxPropertiesBase() { }

        #region Fields

        private string mask = "";
        private string value = "";
        private string waterMarkText = "";
        private bool enabled = true;
        private bool persist = false;
        private bool readOnly = false;
        private string change = "";
        private string cssClass = "";

        #endregion Fields


        #region Properties

        /// <summary>
        /// Gets or sets the start value.
        /// </summary>
        /// <value>
        /// The start value.
        /// </value>
        [JsonProperty("mask")]
        [DefaultValue("")]
        public string Mask { get { return mask; } set { mask = value; } }



        /// <summary>
        /// Gets or sets the end value.
        /// </summary>
        /// <value>
        /// The end value.
        /// </value>
        [JsonProperty("value")]
        [DefaultValue("")]
        public string Value { get { return value; } set { this.value = value; } }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileSliderProperties"/> is range.
        /// </summary>
        /// <value>
        ///   <c>true</c> if range; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("waterMarkText")]
        [DefaultValue("")]
        public string WaterMarkText { get { return waterMarkText; } set { waterMarkText = value; } }



        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileSliderProperties"/> is animate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if animate; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }


        /// <summary>
        /// Gets or sets the animation speed.
        /// </summary>
        /// <value>
        /// The animation speed.
        /// </value>
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist { get { return persist; } set { persist = value; } }


        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [read only]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly { get { return readOnly; } set { readOnly = value; } }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass { get { return cssClass; } set { cssClass = value; } }

        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        /// <value>
        /// The change.
        /// </value>
        [JsonProperty("change")]
        public string Change { get; set; }

        #endregion Properties
    }

    public class RatingPropertiesBase : PropertiesBase
    {

        public RatingPropertiesBase() { }

        #region Fields

        //Int Values
        private int minValue = 0;
        private int maxValue = 5;
        private int currentValue = 1;
        private int shapeWidth = 25;
        private int shapeHeight = 25;

        //Boolean Values
        private bool readOnly = false;
        private bool enabled = true;
        private bool persist = false;

        //Enumeration Values
        private Orientation orientation = Orientation.Horizontal;
        private Precisions precision = Precisions.Full;

        //Events 
        private String click = null;
        private String valueChanged = null;

        #endregion Fields


        #region Properties

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        /// <value>
        /// The current value.
        /// </value>
        [JsonProperty("currentValue")]
        [DefaultValue(0)]
        public int CurrentValue { get; set; }


        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        [JsonProperty("minValue")]
        [DefaultValue(0)]
        public int MinValue { get; set; }



        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        [JsonProperty("maxValue")]
        [DefaultValue(5)]
        public int MaxValue { get { return maxValue; } set { maxValue = value; } }


        /// <summary>
        /// Gets or sets the width of the shape.
        /// </summary>
        /// <value>
        /// The width of the shape.
        /// </value>
        [JsonProperty("shapeWidth")]
        [DefaultValue(25)]
        public int ShapeWidth { get { return shapeWidth; } set { shapeWidth = value; } }



        /// <summary>
        /// Gets or sets the height of the shape.
        /// </summary>
        /// <value>
        /// The height of the shape.
        /// </value>
        [JsonProperty("shapeHeight")]
        [DefaultValue(25)]
        public int ShapeHeight { get { return shapeHeight; } set { shapeHeight = value; } }



        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [read only]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly { get; set; }


        /// <summary>
        /// Gets or sets the click.
        /// </summary>
        /// <value>
        /// The click.
        /// </value>
        [JsonProperty("click")]
        [DefaultValue(null)]
        public string Click { get; set; }



        /// <summary>
        /// Gets or sets the value changed.
        /// </summary>
        /// <value>
        /// The value changed.
        /// </value>
        [JsonProperty("valueChanged")]
        [DefaultValue(null)]
        public string ValueChanged { get; set; }


        /// <summary>
        /// Gets or sets the persist.
        /// </summary>
        /// <value>
        /// The persist.
        /// </value>
        [JsonProperty("persist")]
        public string Persist { get; set; }


        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        [JsonProperty("orientation")]
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation { get { return orientation; } set { orientation = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileProgressBarProperties"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        /// <summary>
        /// Gets or sets the precisions.
        /// </summary>
        /// <value>
        /// The precisions.
        /// </value>
        [JsonProperty("precision")]
        [DefaultValue(Precisions.Full)]
        public Precisions Precision { get { return precision; } set { precision = value; } }

        #endregion Properties

    }

    public class ProgressBarPropertiesBase : PropertiesBase
    {

        public ProgressBarPropertiesBase() { }

        #region Fields

        //Int Values
        private int min = 0;
        private int max = 100;
        private int value = 0;
        private int percentage = 0;

        //Boolean Values
        private bool enabled = true;
        private bool persist = false;

        //String values
        private String text = "";
        private String cssClass = "";

        //Events 
        private String create = null;
        private String complete = null;
        private String change = null;

        #endregion Fields


        #region Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty("value")]
        [DefaultValue(0)]
        public int Value { get; set; }


        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        /// <value>
        /// The percentage.
        /// </value>
        [JsonProperty("percentage")]
        [DefaultValue(0)]
        public int Percentage { get; set; }


        /// <summary>
        /// Gets or sets the step value.
        /// </summary>
        /// <value>
        /// The step value.
        /// </value>
        [JsonProperty("stepValue")]
        [DefaultValue(0)]
        public int StepValue { get; set; }


        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        [JsonProperty("min")]
        [DefaultValue(0)]
        public int Min { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        [JsonProperty("max")]
        [DefaultValue(100)]
        public int Max { get { return max; } set { max = value; } }


        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("text")]
        [DefaultValue("")]
        public string Text { get; set; }


        /// <summary>
        /// Gets or sets the create.
        /// </summary>
        /// <value>
        /// The create.
        /// </value>
        [JsonProperty("create")]
        public string Create { get; set; }

        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        /// <value>
        /// The change.
        /// </value>
        [JsonProperty("change")]
        public string Change { get; set; }

        /// <summary>
        /// Gets or sets the complete.
        /// </summary>
        /// <value>
        /// The complete.
        /// </value>
        [JsonProperty("complete")]
        public string Complete { get; set; }

        /// <summary>
        /// Gets or sets the persist.
        /// </summary>
        /// <value>
        /// The persist.
        /// </value>
        [JsonProperty("persist")]
        public string Persist { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileProgressBarProperties"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        #endregion Properties

    }
	public class EditorPropertiesBase : PropertiesBase
    {

        public EditorPropertiesBase() { }

        #region Fields

        private bool showSpinButton = true;
        private int decimals = 0;
        private bool strictMode = false;
        private int incrementStep = 1;
        public string value = "";
        private string name = "";
        private bool persist = false;
        private bool enabled = true;
        private bool readOnly = false;
        private string waterMarkText = "";
        private double minValue = 10000;
        private double maxValue = 10000000;
        private string change = "";

        #endregion Fields


        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [show spin button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show spin button]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showSpinButton")]
        [DefaultValue(true)]
        public bool ShowSpinButton { get { return showSpinButton; } set { showSpinButton = value; } }

        /// <summary>
        /// Gets or sets the decimals.
        /// </summary>
        /// <value>
        /// The decimals.
        /// </value>
        [JsonProperty("decimals")]
        [DefaultValue(0)]
        public int Decimals { get { return decimals; } set { decimals = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [strict mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [strict mode]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("strictMode")]
        [DefaultValue(false)]
        public bool StrictMode { get { return strictMode; } set { strictMode = value; } }

        /// <summary>
        /// Gets or sets the increment step.
        /// </summary>
        /// <value>
        /// The increment step.
        /// </value>
        [JsonProperty("incrementStep")]
        [DefaultValue(1)]
        public int IncrementStep { get { return incrementStep; } set { incrementStep = value; } }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty("value")]
        [DefaultValue(null)]
        public string Value { get { return value; } set { this.value = value; } }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        [DefaultValue(null)]
        public string Name { get { return name; } set { name = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="EditorPropertiesBase"/> is persist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if persist; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist { get { return persist; } set { persist = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="EditorPropertiesBase"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [read only]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly { get { return readOnly; } set { readOnly = value; } }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        [JsonProperty("minValue")]
        [DefaultValue(10000)]
        public double MinValue { get { return minValue; } set { minValue = value; } }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        [JsonProperty("maxValue")]
        [DefaultValue(10000000)]
        public double MaxValue { get { return maxValue; } set { maxValue = value; } }

        /// <summary>
        /// Gets or sets the water mark text.
        /// </summary>
        /// <value>
        /// The water mark text.
        /// </value>
        [JsonProperty("waterMarkText")]
        [DefaultValue("")]
        public string WaterMarkText { get { return waterMarkText; } set { waterMarkText = value; } }

        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        /// <value>
        /// The change.
        /// </value>
        [JsonProperty("change")]
        public string Change { get { return change; } set { change = value; } }

        #endregion Properties
    }
    public class ToolBarPropertiesBase : PropertiesBase
    {

        public ToolBarPropertiesBase() { }

        #region Fields

        //Boolean Values
        private bool enabled = true;
        private bool hide = false;
        //String Values
        private string cssClass = "";

        #endregion Fields


        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileToolbarProperties"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass { get { return cssClass; } set { cssClass = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileToolbarProperties"/> is hide.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hide; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("hide")]
        [DefaultValue(false)]
        public bool Hide { get { return hide; } set { hide = value; } }

        #endregion Properties
    }
    public class CheckBoxPropertiesBase : PropertiesBase
    {

        public CheckBoxPropertiesBase() { }

        #region Fields

        private bool check = false;
        private bool enabled = true;
        private string text = "";
        private bool preventdefault = false;
        private bool indeterminatestate = false;
        private bool indeterminate = false;

        #endregion Fields


        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MCheckBoxProperties"/> is check.
        /// </summary>
        /// <value>
        ///   <c>true</c> if check; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("check")]
        [DefaultValue(false)]
        public bool Check { get { return check; } set { check = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MCheckBoxProperties"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("text")]
        [DefaultValue("")]
        public string Text { get { return text; } set { text = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [prevent default].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [prevent default]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("preventdefault")]
        [DefaultValue(false)]
        public bool PreventDefault { get { return preventdefault; } set { preventdefault = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [indeterminate state].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [indeterminate state]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("indeterminatestate")]
        [DefaultValue(false)]
        public bool IndeterminateState { get { return indeterminatestate; } set { indeterminatestate = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileCheckBoxProperties"/> is indeterminate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if indeterminate; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("indeterminate")]
        [DefaultValue(false)]
        public bool Indeterminate { get { return indeterminate; } set { indeterminate = value; } }

        #endregion Properties
    }
    public class RadioButtonPropertiesBase : PropertiesBase
    {

        public RadioButtonPropertiesBase() { }

        #region Fields

        private bool check = false;
        private bool enabled = true;
        private string text = "";

        #endregion Fields


        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MRadioButtonProperties"/> is check.
        /// </summary>
        /// <value>
        ///   <c>true</c> if check; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("check")]
        [DefaultValue(false)]
        public bool Check { get { return check; } set { check = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MRadioButtonProperties"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("text")]
        [DefaultValue("")]
        public string Text { get { return text; } set { text = value; } }
        #endregion Properties
    }
 }

