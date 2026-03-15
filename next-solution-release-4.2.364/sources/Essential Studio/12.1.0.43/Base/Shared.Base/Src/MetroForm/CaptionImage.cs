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
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace Syncfusion.Windows.Forms
{

    /// <summary>
    /// Advanced caption image for Metro form
    /// </summary>
    public class CaptionImage
    {
        /// <summary>
        /// location for CaptionImage
        /// </summary>
        private Point location;
        /// <summary>
        /// image for CaptionImage
        /// </summary>
        private Image captionImage = null;
        /// <summary>
        /// size for CaptionImage
        /// </summary>
        private Size size;
        /// <summary>
        /// backcolor for CaptionImage
        /// </summary>
        private Color backColor;
        /// <summary>
        /// Font for CaptionImage
        /// </summary>
        private Font font;
        /// <summary>
        /// forecolor for CaptionImage
        /// </summary>
        private Color foreColor;
        /// <summary>
        /// Constructor for FormCaptionImage
        /// </summary>
        public CaptionImage()
        {
            location = new Point(30, 4);
            size = new Size(24, 24);
            backColor = SystemColors.Control;
            font = SystemFonts.DefaultFont;
            foreColor = Color.Black;
            this.ImageMouseMove += delegate { };
            this.ImageMouseDown += delegate { };
            this.ImageMouseUp += delegate { };
            this.ImageMouseEnter += delegate { };
            this.ImageMouseLeave += delegate { };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>     
        public CaptionImage(Color color)
        {
            ForeColor = color;
        }
        #region Events
        /// <summary>
        /// Mouse move delegate for CaptionImage
        /// </summary>
        public delegate void MouseMove(object sender, ImageMouseMoveEventArgs e);
        /// <summary>
        /// Mouse move event for CaptionImage
        /// </summary>
        public event MouseMove ImageMouseMove;
        /// <summary>
        /// Mouse leave delegate for CaptionImage
        /// </summary>
        public delegate void MouseLeave(object sender, ImageMouseLeaveEventArgs e);
        /// <summary>
        /// Mouse leave event for CaptionImage
        /// </summary>
        public event MouseLeave ImageMouseLeave;
        /// <summary>
        /// Mouse enter delegate for CaptionImage
        /// </summary>
        public delegate void MouseEnter(object sender, ImageMouseEnterEventArgs e);
        /// <summary>
        /// Mouse enter event for CaptionImage
        /// </summary>
        public event MouseEnter ImageMouseEnter;
        /// <summary>
        /// Mouse down delegate for CaptionImage
        /// </summary>
        /// <param name="FormCaptionImage"></param>
        public delegate void MouseDown(object sender, ImageMouseDownEventArgs e);
        /// <summary>
        /// Mouse down delegate for CaptionImage
        /// </summary>
        public event MouseDown ImageMouseDown;
        /// <summary>
        ///  Mouse up delegate for CaptionImage
        /// </summary>
        /// <param name="FormCaptionImage"></param>
        public delegate void MouseUp(object sender, ImageMouseUpEventArgs e);
        /// <summary>
        /// Mouse up delegate for CaptionImage
        /// </summary>
        public event MouseUp ImageMouseUp;
        /// <summary>
        /// Mouse move event for CaptionImage
        /// </summary>
        internal void Mousemove(Point location)
        {
            ImageMouseMove(this, new ImageMouseMoveEventArgs(this,this.Image, this.BackColor, this.location, this.Size, this.ForeColor, MouseButtons.None, 0, location.X, location.Y, 20));
        }
        /// <summary>
        /// Mouse leave event for CaptionImage
        /// </summary>
        internal void Mouseleave(Point location)
        {
            ImageMouseLeave(this, new ImageMouseLeaveEventArgs(this, this.Image, this.BackColor, this.location, this.Size, this.ForeColor, MouseButtons.None, 0, location.X, location.Y, 20));
        }
        /// <summary>
        /// Mouse enter event for CaptionImage
        /// </summary>
        internal void Mouseenter(Point location)
        {
            ImageMouseEnter(this, new ImageMouseEnterEventArgs(this, this.Image, this.BackColor, this.location, this.Size, this.ForeColor, MouseButtons.None, 0, location.X, location.Y, 20));
        }
        /// <summary>
        /// Mouse down event for CaptionImage
        /// </summary>
        internal void Mousedown()
        {
            ImageMouseDown(this, new ImageMouseDownEventArgs(this,this.Image, this.BackColor, this.location, this.Size, this.ForeColor, MouseButtons.Left, 0, location.X, location.Y, 20));
        }
        /// <summary>
        /// Mouse up event for CaptionImage
        /// </summary>
        internal void Mouseup()
        {
            ImageMouseUp(this, new ImageMouseUpEventArgs(this,this.Image, this.BackColor, this.location, this.Size, this.ForeColor, MouseButtons.Left, 0, location.X, location.Y, 20));
        }
        #endregion

        /// <summary>
        /// Gets/sets the value for location
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/sets the value for location.")]
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
        /// Gets/Sets the value for Image
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/Sets the value for Image.")]
        public Image Image
        {
            get
            {
                return captionImage;
            }
            set
            {
                if (value != null)
                    captionImage = value;
            }
        }
        /// <summary>
        /// Gets/Sets the value for CaptionImage size
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/Sets the value for CaptionImage size.")]
        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }
        /// <summary>
        /// Gets/Sets the value for backcolor
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.Description("Gets/Sets the value for backcolor.")]
        public Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
            }
        }
        internal string name = string.Empty;
        /// <summary>
        /// Gets or Sets the instance name
        /// </summary>
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Design"),
        System.ComponentModel.DisplayName("(Name)"),
        System.ComponentModel.Description("Gets or Sets the instance name.")]
        public string Name
        {
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
        /// Gets/Sets the value for forecolor
        /// </summary>
        private Form Owner;
        [System.ComponentModel.Browsable(false)]
        public void SetOwner(Form value) { Owner = value; }
        [System.ComponentModel.Browsable(true),
        System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Description("Gets/Sets the value for forecolor.")]
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
            return (BackColor != SystemColors.Control);
        }
        /// <summary>
        /// Resets the BackColor.
        /// </summary>
        void ResetBackColor()
        {
            this.BackColor = SystemColors.Control;
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
        /// Indicates whether the current value of the Size property is to be serialized.
        /// </summary>
        bool ShouldSerializeSize()
        {
            return (Size != new Size(24, 24));
        }
        /// <summary>
        /// Resets the Size.
        /// </summary>
        void ResetSize()
        {
            this.Size = new Size(24, 24);
        }
        /// <summary>
        /// Indicates whether the current value of the image property is to be serialized.
        /// </summary>
        bool ShouldSerializeImage()
        {
            return (Image != null);
        }
        /// <summary>
        /// Resets the image.
        /// </summary>
        void ResetImage()
        {
            this.Image = null;
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
            this.Location = new Point(30, 4);
        }
        #endregion
    }

    /// <summary>
    ///  Collection of Images used in the Form Caption
    /// </summary>
    public class CaptionImageCollection : CollectionBase
    {
        private Form Owner;
        /// <summary>
        /// Constructor for CaptionImageCollection
        /// </summary>
        public CaptionImageCollection(Form sender) { Owner = sender; }
        /// <summary>
        /// Gets the index of for the CaptionImage
        /// </summary>
        public CaptionImage this[int index] { get { return (CaptionImage)List[index]; } }
        /// <summary>
        /// Returns whether the list contains the CaptionImage type
        /// </summary>
        public bool Contains(CaptionImage itemType) { return List.Contains(itemType); }
        /// <summary>
        /// Adds the CaptionImage type to the list
        /// </summary>
        public int Add(CaptionImage itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            return List.Add(itemType);
        }
        /// <summary>
        ///Removes the CaptionImage type from the list
        /// </summary>
        public void Remove(CaptionImage itemType) { List.Remove(itemType); }
        /// <summary>
        /// Inserts the CaptionImage type into the list
        /// </summary>
        public void Insert(int index, CaptionImage itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            List.Insert(index, itemType);
        }
        /// <summary>
        /// Returns the index of the CaptionImage type
        /// </summary>
        public int IndexOf(CaptionImage itemType) { return List.IndexOf(itemType); }
        /// <summary>
        /// searches the name in the list
        /// </summary>
        public CaptionImage FindByName(string name)
        {
            foreach (CaptionImage captionImage in List)
            {
                if (captionImage.Name == name) return captionImage;
            }
            return null;
        }
        /// <summary>
        /// Overrides the oninsert method
        /// </summary>
        protected override void OnInsert(int index, object value)
        {
            if (string.IsNullOrEmpty(((CaptionImage)value).Name)) ((CaptionImage)value).Name = GetUniqueName();
            base.OnInsert(index, value);
            ((CaptionImage)value).SetOwner(Owner);
        }
        /// <summary>
        /// Gets the unique name
        /// </summary>
        private string GetUniqueName()
        {
            const string Prefix = "CaptionImage";
            int index = 1;
            bool valid;
            while (this.Count != 0)
            {
                valid = true;
                for (int x = 0; x < this.Count; x++)
                {
                    if (this[x].name == (Prefix + index.ToString()))
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
    /// Mouse Move events for caption image
    /// </summary>
    public class ImageMouseLeaveEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for ImageMouseLeaveEventArgs
        /// </summary>
        /// <param name="cImage">Owner for the events</param>
        /// <param name="iImage">Image for caption image</param>
        /// <param name="ibackColor">BackColor for caption image</param>
        /// <param name="iLocation">Location for caption image</param>
        /// <param name="iSize">Size of the caption image</param>
        /// <param name="iForeColor">ForeColor for caption image</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public ImageMouseLeaveEventArgs(CaptionImage cImage, Image iImage, Color ibackColor, Point iLocation, Size iSize, Color iForeColor, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = ibackColor;
            location = iLocation;
            size = iSize;
            foreColor = iForeColor;
            owner = cImage;
            image = iImage;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionImage owner;
        /// <summary>
        /// ForeColor for caption image
        /// </summary>
        private Image image;

        public Image Image
        {
            get { return image; }
            set
            {
                image = value;
                owner.Image = value;
            }
        }
        
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
        /// Size of the caption image
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
        /// BackColor for caption image
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
        /// Location for the caption image
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

    public class ImageMouseEnterEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for ImageMouseEnterEventArgs
        /// </summary>
        /// <param name="cImage">Owner for the events</param>
        /// <param name="iImage">Image for caption image</param>
        /// <param name="ibackColor">BackColor for caption image</param>
        /// <param name="iLocation">Location for caption image</param>
        /// <param name="iSize">Size of the caption image</param>
        /// <param name="iForeColor">ForeColor for caption image</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public ImageMouseEnterEventArgs(CaptionImage cImage, Image iImage, Color ibackColor, Point iLocation, Size iSize, Color iForeColor, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = ibackColor;
            location = iLocation;
            size = iSize;
            foreColor = iForeColor;
            owner = cImage;
            image = iImage;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionImage owner;
        /// <summary>
        /// ForeColor for caption image
        /// </summary>
        private Image image;

        public Image Image
        {
            get { return image; }
            set
            {
                image = value;
                owner.Image = value;
            }
        }

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
        /// Size of the caption image
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
        /// BackColor for caption image
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
        /// Location for the caption image
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

    public class ImageMouseMoveEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for ImageMouseMoveEventArgs
        /// </summary>
        /// <param name="cImage">Owner for the events</param>
        /// <param name="iImage">Image for caption image</param>
        /// <param name="ibackColor">BackColor for caption image</param>
        /// <param name="iLocation">Location for caption image</param>
        /// <param name="iSize">Size of the caption image</param>
        /// <param name="iForeColor">ForeColor for caption image</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public ImageMouseMoveEventArgs(CaptionImage cImage, Image iImage, Color ibackColor, Point iLocation, Size iSize, Color iForeColor, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = ibackColor;
            location = iLocation;
            size = iSize;
            foreColor = iForeColor;
            owner = cImage;
            image = iImage;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionImage owner;
        /// <summary>
        /// ForeColor for caption image
        /// </summary>
        private Image image;

        public Image Image
        {
            get { return image; }
            set
            {
                image = value;
                owner.Image = value;
            }
        }
        
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
        /// Size of the caption image
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
        /// BackColor for caption image
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
        /// Location for the caption image
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
    /// Mouse Up events for caption image
    /// </summary>
    public class ImageMouseUpEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for ImageMouseUpEventArgs
        /// </summary>
        /// <param name="cImage">Owner for the events</param>
        /// <param name="iImage">Image for caption image</param>
        /// <param name="ibackColor">BackColor for caption image</param>
        /// <param name="iFont">Font for caption image</param>
        /// <param name="iLocation">Location for caption image</param>
        /// <param name="iSize">Size of the caption image</param>
        /// <param name="iForeColor">ForeColor for caption image</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public ImageMouseUpEventArgs(CaptionImage cImage, Image iImage, Color ibackColor, Point iLocation, Size iSize, Color iForeColor, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = ibackColor;
            location = iLocation;
            size = iSize;
            foreColor = iForeColor;
            owner = cImage;
            image = iImage;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionImage owner;
        private Image image;

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }
        
        /// <summary>
        /// ForeColor for caption image
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
        /// Size of the caption image
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
        /// BackColor for caption image
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
        /// Location for the caption image
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
    /// Mouse Down events for caption image
    /// </summary>
    public class ImageMouseDownEventArgs : System.Windows.Forms.MouseEventArgs
    {
        /// <summary>
        /// Constructor for ImageMouseDownEventArgs
        /// </summary>
        /// <param name="cImage">Owner for the events</param>
        /// <param name="iImage">Image for caption image</param>
        /// <param name="ibackColor">BackColor for caption image</param>
        /// <param name="iLocation">Location for caption image</param>
        /// <param name="iSize">Size of the caption image</param>
        /// <param name="iForeColor">ForeColor for caption image</param>
        /// <param name="x">Mouse points in X-co ordinate</param>
        /// <param name="y">Mouse points in Y-co ordinate</param>
        public ImageMouseDownEventArgs(CaptionImage cImage,Image iImage, Color ibackColor, Point iLocation, Size iSize, Color iForeColor, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            backColor = ibackColor;
            location = iLocation;
            size = iSize;
            foreColor = iForeColor;
            owner = cImage;
            image = iImage;
        }
        /// <summary>
        /// Owner for the events
        /// </summary>
        private CaptionImage owner;
        /// <summary>
        /// Image for caption image
        /// </summary>
        private Image image;
        /// <summary>
        /// Gets/Sets the value for Image
        /// </summary>
        public Image Image
        {
            get { return image; }
            set
            {
                image = value;
                owner.Image = value;
            }
        }
        /// <summary>
        /// ForeColor for caption image
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
        /// Size of the caption image
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
        /// BackColor for caption image
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
        /// Location for the caption image
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
