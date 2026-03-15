#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace Syncfusion.Windows.Forms
{	
    /// <summary>
    /// Advanced caption label for Metro form
    /// </summary>
    public class CaptionLabel
    {
        /// <summary>
        /// value for label text
        /// </summary>
        private string labelText;
        /// <summary>
        /// value for label location
        /// </summary>
        private Point location;
        /// <summary>
        /// value for label size
        /// </summary>
        private Size labelSize;
        /// <summary>
        /// value for label backcolor
        /// </summary>
        private Color labelBackColor;
        /// <summary>
        /// value for label font
        /// </summary>
        private Font labelFont;
        /// <summary>
        /// value for label forecolor
        /// </summary>
        private Color foreColor;
        /// <summary>
        /// constructor for CaptionLabel
        /// </summary>
        public CaptionLabel()
        {
            labelText = "CaptionLabel";
            location = new Point(30, 4);
            labelSize = new Size(100, 24);
            labelBackColor = Color.Transparent;
            labelFont = SystemFonts.DefaultFont;
            foreColor = Color.Black;
            this.LabelMouseMove += delegate { };
            this.LabelMouseDown += delegate { };
            this.LabelMouseUp += delegate { };
            this.LabelMouseEnter += delegate { };
            this.LabelMouseLeave += delegate { };
        }
        /// <summary>
        /// constructor for CaptionLabel
        /// </summary>label
        /// <param name="color"></param>      
        public CaptionLabel(Color color)
        {
            ForeColor = color;
        }
        /// <summary>
        /// Gets/Sets the value for Label text
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/Sets the value for Label text.")]
        public string Text
        {
            get
            {
                return labelText;
            }
            set  
            {
                labelText = value;
            }
        }
        #region events
        /// <summary>
        /// Mouse move delegate for CaptionLabel
        /// </summary>
        public delegate void MouseMove(object sender, LabelMouseMoveEventArgs e);
        /// <summary>
        /// Mouse move event for CaptionLabel
        /// </summary>
        public event MouseMove LabelMouseMove;
        /// <summary>
        /// Mouse leave delegate for CaptionLabel
        /// </summary>
        public delegate void MouseLeave(object sender, LabelMouseLeaveEventArgs e);
        /// <summary>
        /// Mouse leave event for CaptionLabel
        /// </summary>
        public event MouseLeave LabelMouseLeave;
        /// <summary>
        /// Mouse enter delegate for CaptionLabel
        /// </summary>
        public delegate void MouseEnter(object sender, LabelMouseEnterEventArgs e);
        /// <summary>
        /// Mouse enter event for CaptionLabel
        /// </summary>
        public event MouseEnter LabelMouseEnter;
        /// <summary>
        /// Mouse down delegate for CaptionLabel
        /// </summary>
        public delegate void MouseDown(object sender, LabelMouseDownEventArgs e);
        /// <summary>
        /// Mouse down event for CaptionLabel
        /// </summary>
        public event MouseDown LabelMouseDown;
        /// <summary>
        /// Mouse up delegate for CaptionLabel
        /// </summary>
        public delegate void MouseUp(object sender, LabelMouseUpEventArgs e);
        /// <summary>
        /// Mouse up event for CaptionLabel
        /// </summary>
        public event MouseUp LabelMouseUp;
        /// <summary>
        ///Mouse move event invoking
        /// </summary>
        internal void Mousemove(Point location)
        {
            LabelMouseMove(this, new LabelMouseMoveEventArgs(this, this.BackColor, this.Font, this.Text, this.location, this.Size, this.ForeColor, MouseButtons.None, 0, location.X, location.Y, 20));
        }
        /// <summary>
        /// Mouse enter event invoking
        /// </summary>
        internal void Mouseenter(Point location)
        {
            LabelMouseEnter(this, new LabelMouseEnterEventArgs(this, this.BackColor, this.Font, this.Text, this.location, this.Size, this.ForeColor, MouseButtons.None, 0, location.X, location.Y, 20));
        }
        /// <summary>
        /// Mouse leave event invoking
        /// </summary>
        internal void Mouseleave(Point location)
        {
            LabelMouseLeave(this, new LabelMouseLeaveEventArgs(this, this.BackColor, this.Font, this.Text, this.location, this.Size, this.ForeColor, MouseButtons.None, 0, location.X, location.Y, 20));
        }
        /// <summary>
        ///Mouse down event invoking
        /// </summary>
        internal void Mousedown(Point location)
        {
            LabelMouseDown(this, new LabelMouseDownEventArgs( this ,this.BackColor, this.Font, this.Text, this.location, this.Size, this.ForeColor, MouseButtons.Left, 0, location.X, location.Y, 20));
        }
        /// <summary>
        ///Mouse up event invoking
        /// </summary>
        internal void Mouseup(Point location)
        {
            LabelMouseUp(this,new LabelMouseUpEventArgs(this, this.BackColor, this.Font, this.Text, this.location, this.Size, this.ForeColor, MouseButtons.Left, 0, location.X, location.Y, 20));
        }

        #endregion

        /// <summary>
        /// Gets/Sets the value for label location
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/Sets the value for label location.")]
        public Point Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        /// <summary>
        /// Gets/Sets the value for label size
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/Sets the value for label size.")]
        public Size Size
        {
            get
            {
                return labelSize;
            }
            set
            {
                labelSize = value;
            }
        }
        /// <summary>
        /// Gets/Sets the value for label backcolor
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/Sets the value for label backcolor.")]
        public Color BackColor
        {
            get
            {
                return labelBackColor;
            }
            set
            {
                labelBackColor = value;
            }
        }
        /// <summary>
        /// Gets/Sets the value for label font.
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/Sets the value for label font.")]
        public Font Font
        {
            get
            {
                return labelFont;
            }
            set
            {
                labelFont = value;
            }
        }
        internal string name = string.Empty;
        /// <summary>
        /// Gets or Sets the instance label name
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.DisplayName("(Name)"),
        System.ComponentModel.Description("Gets or Sets the instance label name.")]
        public string Name {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private Form Owner;
        /// <summary>
        /// Specifies the owner of the label
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public void SetOwner(Form value) { Owner = value; }
        /// <summary>
        /// Gets or Sets the Forecolor of the CaptionLabel.
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("Gets or Sets the Forecolor of the CaptionLabel.")]
        public Color ForeColor
        {
            get
            {
                return foreColor;
            }
            set
            {
                foreColor = value;
            }
        }

        #region ShouldSerialize & Reset Methods
        /// <summary>
        /// Indicates whether the current value of the backcolor property is to be serialized.
        /// </summary>
        bool ShouldSerializeBackColor()
        {
            return (BackColor != Color.Transparent);
        }
        /// <summary>
        /// Resets the BackColor.
        /// </summary>
        void ResetBackColor()
        {
            this.BackColor = Color.Transparent;
        }
        /// <summary>
        /// Indicates whether the current value of the forecolor property is to be serialized.
        /// </summary>
        bool ShouldSerializeForeColor()
        {
            return (ForeColor != Color.Black);
        }
        /// <summary>
        /// Resets the BackColor.
        /// </summary>
        void ResetForeColor()
        {
            this.BackColor = Color.Black;
        }
        /// <summary>
        /// Indicates whether the current value of the font property is to be serialized.
        /// </summary>
        bool ShouldSerializeFont()
        {
            return (Font != new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
        }
        /// <summary>
        /// Resets the BackColor.
        /// </summary>
        void ResetFont()
        {
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }
        /// <summary>
        /// Indicates whether the current value of the Size property is to be serialized.
        /// </summary>
        bool ShouldSerializeSize()
        {
            return (Size != new Size(100, 24));
        }
        /// <summary>
        /// Resets the Size.
        /// </summary>
        void ResetSize()
        {
            this.Size = new Size(100, 24);
        }
        /// <summary>
        /// Indicates whether the current value of the location property is to be serialized.
        /// </summary>
        bool ShouldSerializeLocation()
        {
            return (Location != new Point(30, 4));
        }
        /// <summary>
        /// Resets the location.
        /// </summary>
        void ResetLocation()
        {
            this.Location = new Point(30, 4) ;
        }
        /// <summary>
        /// Indicates whether the current value of the text property is to be serialized.
        /// </summary>
        bool ShouldSerializeText()
        {
            return (Text !="CaptionLabel");
        }
        /// <summary>
        /// Resets the Text.
        /// </summary>
        void ResetText()
        {
            this.Text = "CaptionLabel" ;
        }
        #endregion
    }

    /// <summary>
    /// Collection of Labels used in the Form Caption
    /// </summary>
    public class CaptionLabelCollection : CollectionBase
    {
        private Form Owner;
        /// <summary>
        /// Constructor for CaptionLabelCollection
        /// </summary>
        public CaptionLabelCollection(Form sender) { Owner = sender; }
        /// <summary>
        /// Gets the index of for the label
        /// </summary>
        public CaptionLabel this[int index] { get { return (CaptionLabel)List[index]; } }
        /// <summary>
        /// Returns whether the list contains the label type
        /// </summary>
        public bool Contains(CaptionLabel itemType) { return List.Contains(itemType); }
        /// <summary>
        /// Adds the label type to the list
        /// </summary>
        public int Add(CaptionLabel itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            return List.Add(itemType);
        }
        /// <summary>
        ///Removes the label type from the list
        /// </summary>
        public void Remove(CaptionLabel itemType) { List.Remove(itemType); }
        /// <summary>
        /// Inserts the label type into the list
        /// </summary>
        public void Insert(int index, CaptionLabel itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            List.Insert(index, itemType);
        }
        /// <summary>
        /// Returns the index of the label type
        /// </summary>
        public int IndexOf(CaptionLabel itemType) { return List.IndexOf(itemType); }
        /// <summary>
        /// searches the name in the list
        /// </summary>
        public CaptionLabel FindByName(string name)
        {
            foreach (CaptionLabel label in List)
            {
                if (label.Name == name) return label;
            }
            return null;
        }
        /// <summary>
        /// Overrides the oninsert method
        /// </summary>
        protected override void OnInsert(int index, object value)
        {
            if (string.IsNullOrEmpty(((CaptionLabel)value).Name)) ((CaptionLabel)value).Name = GetUniqueName();
            base.OnInsert(index, value);
            ((CaptionLabel)value).SetOwner(Owner);
        }
        /// <summary>
        /// Gets the unique name
        /// </summary>
        private string GetUniqueName()
        {
            const string Prefix = "CaptionLabel";
            int index = 1;
            bool valid;
            while (this.Count != 0)
            {
                valid = true;
                for (int x = 0; x < this.Count; x++)
                {
                    if (this[x].Name == (Prefix + index.ToString()))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid) break;
                index++;
            };
            return Prefix + index.ToString();
        }
    }

    #region Events

    #region Mouse Move Event
    /// <summary>
    /// Mouse Move events for caption label
    /// </summary>
    public class LabelMouseMoveEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for LabelMouseMoveEventArgs
        /// </summary>
        /// <param name="label">Owner for the events</param>
        /// <param name="lbackColor">BackColor for caption label</param>
        /// <param name="lFont">Font for caption label</param>
        /// <param name="lText">Text for caption label</param>
        /// <param name="lLocation">Location for caption label</param>
        /// <param name="lSize">Size of the caption label</param>
        /// <param name="lForeColoe">ForeColor for caption label</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public LabelMouseMoveEventArgs(CaptionLabel label, Color lBackColor, Font lFont, string lText,Point lLocation,Size lSize,Color lForeColoe, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = lBackColor;
            font = lFont;
            text = lText;
            location = lLocation;
            size = lSize;
            foreColor = lForeColoe;
            owner = label;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionLabel owner;
        /// <summary>
        /// ForeColor for caption label
        /// </summary>
        private Color foreColor;
        /// <summary>
        /// Gets/Sets the value for forecolor
        /// </summary>
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColor = value;
                owner.ForeColor = value;
            }
        }
        /// <summary>
        /// Size of the caption label
        /// </summary>
        private Size size;
        /// <summary>
        /// Gets/Sets the value for size
        /// </summary>
        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                owner.Size = value;
            }
        }
        /// <summary>
        /// BackColor for caption label
        /// </summary>
        private Color backColor;
        /// <summary>
        /// Gets/Sets the value for backcolor
        /// </summary>
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                owner.BackColor = value;
            }
        }
        /// <summary>
        /// Font for caption label
        /// </summary>
        private Font font;
        /// <summary>
        /// Gets/Sets the value for Font
        /// </summary>
        public Font Font
        {
            get { return font; }
            set
            {
                font = value;
                owner.Font = value;
            }
        }
        /// <summary>
        /// Text for caption label
        /// </summary>
        private string text;
        /// <summary>
        /// Gets/Sets the value for Text
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                owner.Text = value;
            }
        }
        /// <summary>
        /// Location for the caption label
        /// </summary>
        private Point location;
        /// <summary>
        /// Gets/Sets the value for Location
        /// </summary>
        new public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                owner.Location = value;
            }
        }
    }
    public class LabelMouseLeaveEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for LabelMouseLeaveEventArgs
        /// </summary>
        /// <param name="label">Owner for the events</param>
        /// <param name="lbackColor">BackColor for caption label</param>
        /// <param name="lFont">Font for caption label</param>
        /// <param name="lText">Text for caption label</param>
        /// <param name="lLocation">Location for caption label</param>
        /// <param name="lSize">Size of the caption label</param>
        /// <param name="lForeColoe">ForeColor for caption label</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public LabelMouseLeaveEventArgs(CaptionLabel label, Color lBackColor, Font lFont, string lText, Point lLocation, Size lSize, Color lForeColoe, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = lBackColor;
            font = lFont;
            text = lText;
            location = lLocation;
            size = lSize;
            foreColor = lForeColoe;
            owner = label;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionLabel owner;
        /// <summary>
        /// ForeColor for caption label
        /// </summary>
        private Color foreColor;
        /// <summary>
        /// Gets/Sets the value for forecolor
        /// </summary>
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColor = value;
                owner.ForeColor = value;
            }
        }
        /// <summary>
        /// Size of the caption label
        /// </summary>
        private Size size;
        /// <summary>
        /// Gets/Sets the value for size
        /// </summary>
        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                owner.Size = value;
            }
        }
        /// <summary>
        /// BackColor for caption label
        /// </summary>
        private Color backColor;
        /// <summary>
        /// Gets/Sets the value for backcolor
        /// </summary>
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                owner.BackColor = value;
            }
        }
        /// <summary>
        /// Font for caption label
        /// </summary>
        private Font font;
        /// <summary>
        /// Gets/Sets the value for Font
        /// </summary>
        public Font Font
        {
            get { return font; }
            set
            {
                font = value;
                owner.Font = value;
            }
        }
        /// <summary>
        /// Text for caption label
        /// </summary>
        private string text;
        /// <summary>
        /// Gets/Sets the value for Text
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                owner.Text = value;
            }
        }
        /// <summary>
        /// Location for the caption label
        /// </summary>
        private Point location;
        /// <summary>
        /// Gets/Sets the value for Location
        /// </summary>
        new public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                owner.Location = value;
            }
        }
    }
    public class LabelMouseEnterEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for LabelMouseEnterEventArgs
        /// </summary>
        /// <param name="label">Owner for the events</param>
        /// <param name="lbackColor">BackColor for caption label</param>
        /// <param name="lFont">Font for caption label</param>
        /// <param name="lText">Text for caption label</param>
        /// <param name="lLocation">Location for caption label</param>
        /// <param name="lSize">Size of the caption label</param>
        /// <param name="lForeColoe">ForeColor for caption label</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public LabelMouseEnterEventArgs(CaptionLabel label, Color lBackColor, Font lFont, string lText, Point lLocation, Size lSize, Color lForeColoe, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = lBackColor;
            font = lFont;
            text = lText;
            location = lLocation;
            size = lSize;
            foreColor = lForeColoe;
            owner = label;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionLabel owner;
        /// <summary>
        /// ForeColor for caption label
        /// </summary>
        private Color foreColor;
        /// <summary>
        /// Gets/Sets the value for forecolor
        /// </summary>
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColor = value;
                owner.ForeColor = value;
            }
        }
        /// <summary>
        /// Size of the caption label
        /// </summary>
        private Size size;
        /// <summary>
        /// Gets/Sets the value for size
        /// </summary>
        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                owner.Size = value;
            }
        }
        /// <summary>
        /// BackColor for caption label
        /// </summary>
        private Color backColor;
        /// <summary>
        /// Gets/Sets the value for backcolor
        /// </summary>
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                owner.BackColor = value;
            }
        }
        /// <summary>
        /// Font for caption label
        /// </summary>
        private Font font;
        /// <summary>
        /// Gets/Sets the value for Font
        /// </summary>
        public Font Font
        {
            get { return font; }
            set
            {
                font = value;
                owner.Font = value;
            }
        }
        /// <summary>
        /// Text for caption label
        /// </summary>
        private string text;
        /// <summary>
        /// Gets/Sets the value for Text
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                owner.Text = value;
            }
        }
        /// <summary>
        /// Location for the caption label
        /// </summary>
        private Point location;
        /// <summary>
        /// Gets/Sets the value for Location
        /// </summary>
        new public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                owner.Location = value;
            }
        }
    }
    #endregion

    #region Mouse Up event
    /// <summary>
    /// Mouse Up events for caption label
    /// </summary>
    public class LabelMouseUpEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for LabelMouseUpEventArgs
        /// </summary>
        /// <param name="label">Owner for the events</param>
        /// <param name="lbackColor">BackColor for caption label</param>
        /// <param name="lFont">Font for caption label</param>
        /// <param name="lText">Text for caption label</param>
        /// <param name="lLocation">Location for caption label</param>
        /// <param name="lSize">Size of the caption label</param>
        /// <param name="lForeColoe">ForeColor for caption label</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public LabelMouseUpEventArgs(CaptionLabel label, Color lBackColor, Font lFont, string lText, Point lLocation, Size lSize, Color lForeColor, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = lBackColor;
            font = lFont;
            text = lText;
            location = lLocation;
            size = lSize;
            foreColor = lForeColor;
            owner = label;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionLabel owner;
        /// <summary>
        /// ForeColor for caption label
        /// </summary>
        private Color foreColor;
        /// <summary>
        /// Gets/Sets the value for forecolor
        /// </summary>
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColor = value;
                owner.ForeColor = value;
            }
        }
        /// <summary>
        /// Size of the caption label
        /// </summary>
        private Size size;
        /// <summary>
        /// Gets/Sets the value for size
        /// </summary>
        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                owner.Size = value;
            }
        }
        /// <summary>
        /// BackColor for caption label
        /// </summary>
        private Color backColor;
        /// <summary>
        /// Gets/Sets the value for backcolor
        /// </summary>
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                owner.BackColor = value;
            }
        }
        /// <summary>
        /// Font for caption label
        /// </summary>
        private Font font;
        /// <summary>
        /// Gets/Sets the value for Font
        /// </summary>
        public Font Font
        {
            get { return font; }
            set
            {
                font = value;
                owner.Font = value;
            }
        }
        /// <summary>
        /// Text for caption label
        /// </summary>
        private string text;
        /// <summary>
        /// Gets/Sets the value for Text
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                owner.Text = value;
            }
        }
        /// <summary>
        /// Location for the caption label
        /// </summary>
        private Point location;
        /// <summary>
        /// Gets/Sets the value for Location
        /// </summary>
        new public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                owner.Location = value;
            }
        }
    }
    #endregion

    #region Mouse Down Event
    /// <summary>
    /// Mouse Down events for caption label
    /// </summary>
    public class LabelMouseDownEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for LabelMouseDownEventArgs
        /// </summary>
        /// <param name="label">Owner for the events</param>
        /// <param name="lbackColor">BackColor for caption label</param>
        /// <param name="lFont">Font for caption label</param>
        /// <param name="lText">Text for caption label</param>
        /// <param name="lLocation">Location for caption label</param>
        /// <param name="lSize">Size of the caption label</param>
        /// <param name="lForeColoe">ForeColor for caption label</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public LabelMouseDownEventArgs(CaptionLabel label, Color lbackColor, Font lFont, string lText, Point lLocation, Size lSize, Color lForeColoe, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = lbackColor;
            font = lFont;
            text = lText;
            location = lLocation;
            size = lSize;
            foreColor = lForeColoe;
            owner = label;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionLabel owner;
        /// <summary>
        /// ForeColor for caption label
        /// </summary>
        private Color foreColor;
        /// <summary>
        /// Gets/Sets the value for forecolor
        /// </summary>
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColor = value;
                owner.ForeColor = value;
            }
        }
        /// <summary>
        /// Size of the caption label
        /// </summary>
        private Size size;
        /// <summary>
        /// Gets/Sets the value for size
        /// </summary>
        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                owner.Size = value;
            }
        }
        /// <summary>
        /// BackColor for caption label
        /// </summary>
        private Color backColor;
        /// <summary>
        /// Gets/Sets the value for backcolor
        /// </summary>
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                owner.BackColor = value;
            }
        }
        /// <summary>
        /// Font for caption label
        /// </summary>
        private Font font;
        /// <summary>
        /// Gets/Sets the value for Font
        /// </summary>
        public Font Font
        {
            get { return font; }
            set
            {
                font = value;
                owner.Font = value;
            }
        }
        /// <summary>
        /// Text for caption label
        /// </summary>
        private string text;
        /// <summary>
        /// Gets/Sets the value for Text
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                owner.Text = value;
            }
        }
        /// <summary>
        /// Location for the caption label
        /// </summary>
        private Point location;
        /// <summary>
        /// Gets/Sets the value for Location
        /// </summary>
        new public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                owner.Location = value;
            }
        }
    }
    #endregion

    #endregion
}
