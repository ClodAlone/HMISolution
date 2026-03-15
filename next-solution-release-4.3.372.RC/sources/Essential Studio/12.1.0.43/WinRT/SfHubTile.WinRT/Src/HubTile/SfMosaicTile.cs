// <copyright file="MosaicTile.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !(SILVERLIGHT||WPF||WINDOWS_PHONE_7)
using Windows.UI;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Syncfusion.WP.Controls.Notification
#else
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Syncfusion.Tools.Controls.Notification
#else
#if WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
namespace Syncfusion.Windows.Controls.Notification
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
#endif
#endif
{
    /// <summary>
    /// MosaicTile control resembles
    /// the people tile in Windows phone home screen. It displays a collection of images
    /// in tile structure. Each tile will show a flip transition and show image
    /// randomly.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class SfMosaicTile : HubTileBase
    {
        #region Variables

        private DispatcherTimer timer;

        private Dictionary<int, Image> tempList;

        private Grid PART_MosaicContent;

        private Random randomTile;

        private Random randomImage;

        private Random randomOpacity;

        private bool maxImageStarted = false;

        private Random alignmentPicker = new Random();

        private long count = 0;

        private int[] topleft = { 0, 1, 3, 4 };

        private int[] topright = { 1, 2, 4, 5 };

        private int[] bottomleft = { 3, 4, 6, 7 };

        private int[] bottomright = { 4, 5, 7, 8 };

        private List<int> temparray = new List<int>();

        int? previousindex = null;

        private MosaicTileContent previousContent;

        private string previousImage;

        private int arrayposition = -1;

        private bool isreaload = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.MosaicTile"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfMosaicTile()
        {
            DefaultStyleKey = typeof(SfMosaicTile);
            this.Loaded += MosaicTile_Loaded;
            this.Unloaded += MosaicTile_Unloaded;
        }

        void MosaicTile_Unloaded(object sender, RoutedEventArgs e)
        {
            HubTileService.Dequeue(this);
            if (timer != null)
                timer.Stop();

            IsEnabledChanged -= OnIsEnabledChanged;
            if(ImageList!=null)
            ImageList.CollectionChanged -= ImageList_CollectionChanged;
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.ImageList"/> that holds the
        /// collection of images to display with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.MosaicTile"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public ImageList ImageList
        {
            get { return (ImageList)GetValue(ImageListProperty); }
            set { SetValue(ImageListProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageList.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageListProperty =
            DependencyProperty.Register("ImageList", typeof(ImageList), typeof(SfMosaicTile), new PropertyMetadata(null,OnImageListChanged));


        /// <summary>
        /// Gets or sets the brush to show when the same image is repeating
        /// </summary>
        /// <value>
        /// <para>The default value is <see
        /// cref="T:Windows.UI.Colors.Transparent">Colors.Transparent</see></para>
        /// </value>
        public new Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public new static DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfMosaicTile), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the number of milliseconds to wait before initiating a post back.
        /// </summary>
        /// <value>
        /// It accepts the type of <see cref="T:System.TimeSpan"/>, The default values is
        /// 0 <see cref="P:System.TimeSpan.Milliseconds"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public TimeSpan Interval
        {
            get { return (TimeSpan)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(TimeSpan), typeof(SfMosaicTile), new PropertyMetadata(TimeSpan.FromSeconds(0), new PropertyChangedCallback(OnIntervalChanged)));


        #endregion

        #region Helper Methods

        private void MosaicTile_Loaded(object sender, RoutedEventArgs e)
        {
            HubTileService.Enqueue(this);
            if (!isreaload)
            {
                if (timer == null)
                {
                    timer = new DispatcherTimer();
                }

                if (randomTile == null)
                {
                    randomTile = new Random();
                }

                if (randomImage == null)
                {
                    randomImage = new Random();
                }

                if (tempList == null)
                {
                    tempList = new Dictionary<int, Image>();
                }
                if (ImageList == null && PART_MosaicContent!=null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        TransitionContentControl control = PART_MosaicContent.Children[i] as TransitionContentControl;
                        MosaicTileContent tilecontent = new MosaicTileContent();
                        tilecontent.Background = AccentBrush;
                        tilecontent.Opacity = GetRandomOpacity();
                        control.Content = tilecontent;
                    }
                }

                if (PART_MosaicContent != null && ImageList != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        TransitionContentControl control = PART_MosaicContent.Children[i] as TransitionContentControl;
                        MosaicTileContent tilecontent = new MosaicTileContent();
                        string image = ImageList[randomImage.Next(ImageList.Count)];

                        if (!CheckForExistence(image))
                        {
                            tilecontent.Image = image;
                        }
                        else
                        {
                            tilecontent.Image = null;
                            tilecontent.Background = AccentBrush;
                            tilecontent.Opacity = GetRandomOpacity();
                        }
                        control.Content = tilecontent;
                    }
                }

                timer.Interval = TimeSpan.FromSeconds(1.3);
                timer.Tick -= timer_Tick;
                timer.Tick += timer_Tick;
            }
            isreaload = true;
            if (ImageList != null)
            {
                ImageList.CollectionChanged -= ImageList_CollectionChanged;
                ImageList.CollectionChanged += ImageList_CollectionChanged;
            }
            if (timer != null && !IsFrozen)
                timer.Start();
#if SILVERLIGHT
            if(timer!=null && Interval!=TimeSpan.FromSeconds(0))
                timer.Interval = Interval;
#endif
            IsEnabledChanged += OnIsEnabledChanged;
            UpdateVisualState();
        }
        
        void ImageList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                timer.Stop();
                HubTileService.Dequeue(this);
                for (int i = 0; i < 9; i++)
                {
                    MosaicTileContent tilecontent = new MosaicTileContent();
                    TransitionContentControl control = PART_MosaicContent.Children[i] as TransitionContentControl;
                    if (ImageList.Count == 0)
                    {
                        tilecontent.Background = AccentBrush;
                        tilecontent.Opacity = GetRandomOpacity();
                    }
                    else
                    {
                        string image = ImageList[randomImage.Next(ImageList.Count)];

                        if (!CheckForExistence(image))
                        {
                            tilecontent.Image = image;
                        }
                        else
                        {
                            tilecontent.Image = null;
                            tilecontent.Background = AccentBrush;
                            tilecontent.Opacity = GetRandomOpacity();
                        }
                    }
                    control.Content = tilecontent;
                }
                maxImageStarted = false;

                if (ImageList.Count == 0)
                {
                    previousindex = null;
                    previousImage = null;
                    randomTile = new Random();
                    randomImage = new Random();
                    tempList = new Dictionary<int, Image>();
                    temparray = new List<int>();
                    count = 0;
                }
                HubTileService.Enqueue(this);
                if(!IsFrozen)
                timer.Start();
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (timer != null)
            {
                if (!IsEnabled || IsFrozen)
                    timer.Stop();
                else
                    timer.Start();

                if (!IsEnabled)
                    VisualStateManager.GoToState(this, "Disabled", true);
                else
                    VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        private double GetRandomOpacity()
        {
            if (randomOpacity == null)
            {
                randomOpacity = new Random();
            }

            int value = randomOpacity.Next(3);
            double opacity = 0.8;

            if (value == 0)
            {
                opacity = 0;
            }
            else if (value == 1)
            {
                opacity = 0.2;
            }
            else if (value == 2)
            {
                opacity = 0.4;
            }
            else
            {
                opacity = 0.6;
            }
            return opacity;
        }

        /// <summary>
        /// Checks if there exists children for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.MosaicTileContent"/>
        /// </summary>
        /// <param name="image"></param>
        /// <returns>
        /// <c>true</c> if element exists; otherwise, <c>false</c>
        /// </returns>
        public bool CheckForExistence(string image)
        {
            foreach (FrameworkElement element in PART_MosaicContent.Children)
            {
                TransitionContentControl _control = element as TransitionContentControl;
                if (_control != null)
                {
                    MosaicTileContent _tilecontent = _control.Content as MosaicTileContent;
                    if (_tilecontent != null && _tilecontent.Image == image)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void timer_Tick(object sender, object e)
        {
            int index = randomTile.Next(0, 9);

            if (previousindex != null && index == previousindex)
            {
                return;
            }

            MosaicTileContent tilecontent = new MosaicTileContent();
            TransitionContentControl control = PART_MosaicContent.Children[index] as TransitionContentControl;

            if (ImageList == null)
            {
                tilecontent.Background = AccentBrush;
                tilecontent.Opacity = GetRandomOpacity();
            }
            else
            {
                if (!maxImageStarted && ImageList != null && ImageList.Count>0)
                {
                    string image = ImageList[randomImage.Next(ImageList.Count)];

                    if (count % 10 == 0)
                    {
                        if (!CheckForExistence(image))
                        {
                            tilecontent.ImageWidth = ActualWidth - (ActualWidth / 3);
                            tilecontent.ImageHeight = ActualHeight - (ActualHeight / 3);
                            tilecontent.HorizontalImageAlignment = GetHAlignment(index);
                            tilecontent.VerticalImageAlignment = GetVAlignment(index);
                            tilecontent.Image = image;
                            maxImageStarted = true;
                        }
                        else
                        {
                            tilecontent.Image = null;
                            tilecontent.Background = AccentBrush;
                            tilecontent.Opacity = GetRandomOpacity();
                        }
                    }
                    else
                    {
                        if (!CheckForExistence(image))
                        {
                            tilecontent.Image = image;
                        }
                        else
                        {
                            tilecontent.Image = null;
                            tilecontent.Background = AccentBrush;
                            tilecontent.Opacity = GetRandomOpacity();
                        }
                        count++;
                    }
                    previousImage = image;
                }
                else
                {
                    if (previousindex.HasValue)
                    {
                        if (temparray.Count == 0)
                        {
                            count++;
                            arrayposition = GetArrayPosition(previousindex.Value, previousContent);
                            temparray.Add(previousindex.Value);
                        }

                        if (arrayposition < 0)
                        {
                            return;
                        }

                        index = GetRandomMaxImageIndex(arrayposition, temparray);

                        tilecontent.ImageWidth = ActualWidth - (ActualWidth / 3);
                        tilecontent.ImageHeight = ActualHeight - (ActualHeight / 3);
                        if (temparray.Count == 0)
                        {
                            tilecontent.HorizontalImageAlignment = GetHAlignment(index);
                            tilecontent.VerticalImageAlignment = GetVAlignment(index);
                        }
                        else
                        {
                            tilecontent.HorizontalImageAlignment = GetHAlignment(index, arrayposition);
                            tilecontent.VerticalImageAlignment = GetVAlignment(index, arrayposition);
                        }

                        if (!temparray.Contains(index))
                        {
                            control = PART_MosaicContent.Children[index] as TransitionContentControl;
                            temparray.Add(index);
                        }
                        else
                        {
                            return;
                        }

                        tilecontent.Image = previousImage;

                        if (temparray.Count == 4)
                        {
                            maxImageStarted = false;
                            temparray.Clear();
                        }
                    }
                }
            }

            control.Content = tilecontent;
            previousindex = index;
            previousContent = tilecontent;
        }

        private HorizontalAlignment GetHAlignment(int index)
        {
            if (index == 0 || index == 6 || index == 3)
            {
                return HorizontalAlignment.Left;
            }
            else if (index == 2 || index == 8 || index == 5)
            {
                return HorizontalAlignment.Right;
            }
            else
            {
                return alignmentPicker.Next(1) == 0 ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            }
        }

        private HorizontalAlignment GetHAlignment(int index, int arrayposition)
        {
            if (index == 0 || index == 6 || index == 3)
            {
                return HorizontalAlignment.Left;
            }
            else if (index == 2 || index == 8 || index == 5)
            {
                return HorizontalAlignment.Right;
            }
            else if (index == 1)
            {
                if (arrayposition == 0)
                {
                    return HorizontalAlignment.Right;
                }
                else
                {
                    return HorizontalAlignment.Left;
                }
            }
            else if (index == 4)
            {
                if (arrayposition == 0 || arrayposition == 2)
                {
                    return HorizontalAlignment.Right;
                }
                else
                {
                    return HorizontalAlignment.Left;
                }
            }
            else
            {
                if (arrayposition == 2)
                {
                    return HorizontalAlignment.Right;
                }
                else
                {
                    return HorizontalAlignment.Left;
                }
            }
        }

        private VerticalAlignment GetVAlignment(int index)
        {
            if (index == 0 || index == 1 || index == 2)
            {
                return VerticalAlignment.Top;
            }
            else if (index == 6 || index == 7 || index == 8)
            {
                return VerticalAlignment.Bottom;
            }
            else
            {
                return alignmentPicker.Next(1) == 0 ? VerticalAlignment.Top : VerticalAlignment.Bottom;
            }
        }

        private VerticalAlignment GetVAlignment(int index, int arrayposition)
        {
            if (index == 0 || index == 1 || index == 2)
            {
                return VerticalAlignment.Top;
            }
            else if (index == 6 || index == 7 || index == 8)
            {
                return VerticalAlignment.Bottom;
            }
            else if (index == 4)
            {
                if (arrayposition == 0 || arrayposition == 1)
                {
                    return VerticalAlignment.Bottom;
                }
                else
                {
                    return VerticalAlignment.Top;
                }
            }
            else if (index == 3)
            {
                if (arrayposition == 0)
                {
                    return VerticalAlignment.Bottom;
                }
                else
                {
                    return VerticalAlignment.Top;
                }
            }
            else
            {
                if (arrayposition == 1)
                {
                    return VerticalAlignment.Bottom;
                }
                else
                {
                    return VerticalAlignment.Top;
                }
            }
        }

        private int GetArrayPosition(int index, MosaicTileContent tilecontent)
        {
            if (index == 4)
            {
                if (tilecontent.VerticalImageAlignment == VerticalAlignment.Top && tilecontent.HorizontalImageAlignment == HorizontalAlignment.Left)
                {
                    return 3;
                }
                else if (tilecontent.VerticalImageAlignment == VerticalAlignment.Bottom && tilecontent.HorizontalImageAlignment == HorizontalAlignment.Right)
                {
                    return 1;
                }
                else if (tilecontent.HorizontalImageAlignment == HorizontalAlignment.Right && tilecontent.VerticalImageAlignment == VerticalAlignment.Top)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (index == 1)
            {
                if (tilecontent.HorizontalImageAlignment == HorizontalAlignment.Right)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if (index == 3)
            {
                if (tilecontent.VerticalImageAlignment == VerticalAlignment.Bottom)
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else if (index == 5)
            {
                if (tilecontent.VerticalImageAlignment == VerticalAlignment.Bottom)
                {
                    return 1;
                }
                else
                {
                    return 3;
                }
            }
            else if (index == 7)
            {
                if (tilecontent.HorizontalImageAlignment == HorizontalAlignment.Right)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            else if (index == 0)
            {
                return 0;
            }
            else if (index == 2)
            {
                return 1;
            }
            else if (index == 6)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        private int GetRandomMaxImageIndex(int position, List<int> temparray)
        {
            int[] array;

            if (position == 0)
            {
                array = topleft;
            }
            else if (position == 1)
            {
                array = topright;
            }
            else if (position == 2)
            {
                array = bottomleft;
            }
            else
            {
                array = bottomright;
            }

            Random random = new Random();
            int index = random.Next(3);
            return GetNumberOtherThanThis(index, array, temparray);
        }

        private int GetNumberOtherThanThis(int index, int[] array, List<int> temparray)
        {
            if (temparray.Count == 0 || !temparray.Contains(array[index]))
            {
                return array[index];
            }
            else
            {
                index = index - 1;
                if (index == -1)
                {
                    index = 3;
                }
                return GetNumberOtherThanThis(index, array, temparray);
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Invoked when the IsFrozen property is changed
        /// </summary>
        /// <param name="e"></param>
        /// <value> Timer is
        /// <c>Stop</c> if IsFrozen is True; otherwise, <c>Start</c>.
        /// </value>
        protected override void OnIsFrozenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsFrozen)
            {
                if (this.timer != null)
                this.timer.Stop();
            }
            else
            {
                if (this.timer != null)
                this.timer.Start();
            }
        }

        /// <summary>
        /// Initializes all the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.MosaicTile"/>
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_MosaicContent = GetTemplateChild("PART_MosaicContent") as Grid;
            base.OnApplyTemplate();
#if SILVERLIGHT
            isreaload = false;
            this.Loaded+=MosaicTile_Loaded;
#endif
        }

        #endregion

        #region Callback Methods
   
        private static void OnImageListChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfMosaicTile mosaictile = sender as SfMosaicTile;
            if (mosaictile.ImageList != null)
            {
                mosaictile.ImageList.CollectionChanged -= mosaictile.ImageList_CollectionChanged;
                mosaictile.ImageList.CollectionChanged += mosaictile.ImageList_CollectionChanged;
            }
            if (e.OldValue != null)
            {
                HubTileService.Dequeue(mosaictile);
                mosaictile.randomTile = new Random();
                mosaictile.randomImage = new Random();
                mosaictile.tempList = new Dictionary<int, Image>();
                mosaictile.count = 0;
                if (mosaictile.ImageList != null && mosaictile.PART_MosaicContent != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        TransitionContentControl control = mosaictile.PART_MosaicContent.Children[i] as TransitionContentControl;
                        MosaicTileContent tilecontent = new MosaicTileContent();
                        string image = mosaictile.ImageList[mosaictile.randomImage.Next(mosaictile.ImageList.Count)];

                        if (!mosaictile.CheckForExistence(image))
                        {
                            tilecontent.Image = image;
                        }
                        else
                        {
                            tilecontent.Image = null;
                            tilecontent.Background = mosaictile.AccentBrush;
                            tilecontent.Opacity = mosaictile.GetRandomOpacity();
                        }
                        control.Content = tilecontent;
                    }
                    mosaictile.maxImageStarted = false;
                }
                HubTileService.Enqueue(mosaictile);
                mosaictile.timer.Start();                
            }
        }
        private static void OnIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfMosaicTile tile = sender as SfMosaicTile;
            if (tile != null)
            {
                if (e.NewValue != null && ((TimeSpan)e.NewValue).Seconds != 0)
                {
                    if (tile.timer == null)
                    {
                        tile.timer = new DispatcherTimer();
                    }
                    tile.timer.Interval = tile.Interval;
                    tile.timer.Tick -= tile.timer_Tick;
                    tile.timer.Tick += tile.timer_Tick;
                }
                else
                {
                    tile.timer.Tick -= tile.timer_Tick;
                }
            }
        }

        #endregion


    }
}
