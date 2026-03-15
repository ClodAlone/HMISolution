#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.Navigation
{
    using Design;

    /// <summary>
    /// Specifies that this object can contain a collection of <see cref="Bars"/>.
    /// </summary>
    public interface IContainBars
    {
        BarCollection Bars { get; }
    }

    /// <summary>
    /// <see cref="NavigationView"/>'s bar.
    /// </summary>
    [Serializable]
    [DefaultProperty("Text")]
    [DefaultEvent("PropertyChanged")]
    [TypeConverter(typeof(BarConvernter))]
    public class Bar :
        INotifyPropertyChanged,
        IDisposable,
        IContainBars
    {
        #region Enums

        /// <summary>
        /// <see cref="Bar"/>'s properties that support property changed notification.
        /// </summary>
        [Flags]
        public enum Properties
        {
            /// <summary>
            /// Represents Text
            /// </summary>
            Text,

            /// <summary>
            /// Represents Image
            /// </summary>
            Image,

            /// <summary>
            /// Represents Disabled Image
            /// </summary>
            DisabledImage,

            /// <summary>
            /// Represents Image Index
            /// </summary>
            ImageIndex,

            /// <summary>
            /// Represents Disabled Image
            /// </summary>
            DisabledImageIndex,

            /// <summary>
            /// Represents visible
            /// </summary>
            Visible,

            /// <summary>
            /// Represents Enabled
            /// </summary>
            Enabled,

            /// <summary>
            /// Represents AutoExpand
            /// </summary>
            AutoExpand
        }

        #endregion

        #region Fields

        private BarCollection _bars;

        private string _text;
        private Image _image;
        private Image _disabledImage;
        private int _imageIndex = -1;
        private int _disabledImageIndex = -1;
        private bool _visible = true;
        private bool _enabled = true;
        private bool _autoExpand = true;

        #endregion

        #region Contruction

        /// <summary>
        /// Initializes a new instance of the <see cref="Bar"/> class.
        /// </summary>
        public Bar()
        {
            _text = String.Empty;
            _imageIndex = -1;
            _disabledImageIndex = -1;
            _visible = true;
            _enabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bar"/> class and sets its text.
        /// </summary>
        /// <param name="text">The <see cref="Bar"/>'s text.</param>
        public Bar(string text) :
            this()
        {
            _text = text;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets Collection of child bars.
        /// </summary>
        [Description("Collection of child bars.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Behavior")]
        [Browsable(false)]
        public BarCollection Bars
        {
            get
            {
                if (_bars == null)
                {
                    _bars = new BarCollection();
                }

                return _bars;
            }
        }

        /// <summary>
        /// Gets the collection of visible <see cref="Bar"/>s.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BarCollection VisibleBars
        {
            get
            {
                BarCollection visibleBars = new BarCollection();

                foreach (Bar bar in _bars)
                {
                    if (bar.Visible)
                    {
                        visibleBars.Add(bar);
                    }
                }

                return visibleBars;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="Bar"/>'s text.
        /// </summary>
        [Description("Bar's text.")]
        [DefaultValue("")]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;

                    FirePropertyChangedEvent(Properties.Text);
                }
            }
        }

        /// <summary>
        /// Gets or sets <see cref="Bar"/>'s image.
        /// </summary>
        /// <remarks><see cref="Bar.ImageIndex"/> is ignored if <see cref="Bar.Image"/> has non-null value.</remarks>
        [Description("Bar's image.")]
        [DefaultValue(null)]
        [Category("Appearance")]
        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (_image != value)
                {
                    _image = value;

                    FirePropertyChangedEvent(Properties.Image);
                }
            }
        }

        /// <summary>
        /// Gets or sets disabled <see cref="Bar"/>'s image.
        /// </summary>
        /// <remarks><see cref="Bar.DisabledImageIndex"/> is ignored if <see cref="Bar.DisabledImage"/> has non-null value.</remarks>
        [Description("Disabled Bar's image.")]
        [DefaultValue(null)]
        [Category("Appearance")]
        public Image DisabledImage
        {
            get
            {
                return _disabledImage;
            }
            set
            {
                if (_disabledImage != value)
                {
                    _disabledImage = value;

                    FirePropertyChangedEvent(Properties.DisabledImage);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Bar"/>'s index of the image in <see cref="NavigationView.ImageList"/>.
        /// </summary>
        /// <remarks><see cref="Bar.ImageIndex"/> is ignored if <see cref="Bar.Image"/> has non-null value.</remarks>
        [Description("Bar's image index.")]
        [DefaultValue(-1)]
        [Category("Appearance")]
        public int ImageIndex
        {
            get
            {
                return _imageIndex;
            }
            set
            {
                if (_imageIndex != value)
                {
                    _imageIndex = value;

                    FirePropertyChangedEvent(Properties.ImageIndex);
                }
            }
        }

        /// <summary>
        /// Gets or sets the disabled <see cref="Bar"/>'s index of the image in <see cref="NavigationView.ImageList"/>.
        /// </summary>
        /// <remarks><see cref="Bar.ImageList"/> is ignored if <see cref="Bar.Image"/> has non-null value.</remarks>
        [Description("Disabled Bar's image index.")]
        [DefaultValue(-1)]
        [Category("Appearance")]
        public int DisabledImageIndex
        {
            get
            {
                return _disabledImageIndex;
            }
            set
            {
                if (_disabledImageIndex != value)
                {
                    _disabledImageIndex = value;

                    FirePropertyChangedEvent(Properties.DisabledImageIndex);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Bar"/> is visible.
        /// </summary>
        [Description("Bar's visibility.")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (_visible != value)
                {
                    _visible = value;

                    FirePropertyChangedEvent(Properties.Visible);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Bar"/> is enabled.
        /// </summary>
        [Description("Indicates whether Bar is enabled.")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;

                    FirePropertyChangedEvent(Properties.Enabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Bar"/>'s menu auto expands when mouse is over the bar.
        /// </summary>
        [Description("Indicates whether bar's menu auto expands when mouse is over the bar.")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool AutoExpand
        {
            get
            {
                return _autoExpand;
            }
            set
            {
                if (_autoExpand != value)
                {
                    _autoExpand = value;

                    FirePropertyChangedEvent(Properties.AutoExpand);
                }
            }
        }

        private object _tag;

        /// <summary>
        /// Gets or sets the object that contains data about the <see cref="Bar"/>.
        /// </summary>
        /// <value>An <see cref="System.Object"/> that contains data about the <see cref="Bar"/>. The default is <c>null</c>.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether this instance of <see cref="Bar"/> is a selected bar of the specified <see cref="NavigationView"/>.
        /// </summary>
        /// <param name="owner">The instance of <see cref="NavigationView"/> owner control.</param>
        /// <seealso cref="NavigationView.SelectedBar"/>
        /// <returns>Returns true if selected</returns>
        public bool IsSelected(NavigationView owner)
        {
            return (owner != null) ? (this == owner.SelectedBar) : false;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Fires <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="property">Chaned property</param>
        protected void FirePropertyChangedEvent(Properties property)
        {
            if (this.PropertyChanged != null)
            {
                string propName = Enum.GetName(typeof(Properties), property);
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(propName);

                this.PropertyChanged(this, e);
            }
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_bars != null)
                {
                    _bars.Clear();
                }
            }
        }

        ~Bar()
        {
            Dispose(false);
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return String.IsNullOrEmpty(this.Text) ? base.ToString() : this.Text;
        }

        #endregion

        #region Classes

        internal static class Helper
        {
            public static Image GetBarImage(Bar bar, NavigationView navView, bool bBarEnabled)
            {
                Image image = null;

                if (bBarEnabled)
                {
                    if (bar.Image != null)
                    {
                        image = bar.Image;
                    }
                    else
                    {
                        image = ExtractImage(navView.ImageList, bar.ImageIndex);
                    }
                }
                else
                {
                    if (bar.DisabledImage != null)
                    {
                        image = bar.DisabledImage;
                    }
                    else
                    {
                        image = ExtractImage(navView.DisabledImageList, bar.DisabledImageIndex);
                    }
                }

                return image;
            }

            public static Image GetBarImage(Bar bar, NavigationView navView)
            {
                return GetBarImage(bar, navView, bar.Enabled);
            }

            public static Image ExtractImage(ImageList imgList, int imageIdx)
            {
                Image image = null;

                if (imgList != null && imageIdx >= 0 && imageIdx < imgList.Images.Count)
                {
                    image = imgList.Images[imageIdx];
                }

                return image;
            }

            public static Properties ParsePropertyEnumString(string propertyName)
            {
                Properties prop = (Properties)Enum.Parse(typeof(Properties), propertyName);

                return prop;
            }
        }

        #endregion
    }
}
