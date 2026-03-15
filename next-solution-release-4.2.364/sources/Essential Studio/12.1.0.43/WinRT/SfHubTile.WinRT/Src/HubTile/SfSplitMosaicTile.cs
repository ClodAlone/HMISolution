// <copyright file="SplitMosaicTile.cs" company="Syncfusion">
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

#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.WP.Controls.Notification
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Tools.Controls.Notification
#else
#if WPF
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.Notification
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
#endif
#endif
{
    /// <summary>
    /// SplitMosaicTile Control resembles
    /// the Contacts group tile in Windows Phone home screen. It will display a
    /// collection of images in 3 horizontally stretched tiles. Each tile will flip
    /// themselves randomly to bring a new image.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class SfSplitMosaicTile : HubTileBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SplitMosaicTile"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfSplitMosaicTile()
        {
            DefaultStyleKey = typeof(SfSplitMosaicTile);
            this.Loaded += SimpleImageRotator_Loaded;
            this.Unloaded += SplitMosaicTile_Unloaded;
        }

        void SplitMosaicTile_Unloaded(object sender, RoutedEventArgs e)
        {
            HubTileService.Dequeue(this);
            if (timer != null)
                timer.Stop();

            IsEnabledChanged -= OnIsEnabledChanged;
        }

        #endregion

        #region Variables

        private DispatcherTimer timer;

        private Random randomTile;

        private Random randomImage;

        private Random randomOpacity;

        private Grid PART_SimpleImageContent;

        private ContentPresenter PART_Header;

        int? previousindex = null;

        private string previousimage;

        private List<int> temparray = new List<int>();

        private bool maximageStarted = false;

        private MosaicTileContent previousContent;

        int[] randomarray = { 0, 1, 2 };

        private bool isrealoaded = false;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.ImageList"/> that holds the
        /// collection of images to display with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SplitMosaicTile"/>.
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
            DependencyProperty.Register("ImageList", typeof(ImageList), typeof(SfSplitMosaicTile), new PropertyMetadata(null));

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
        public new static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfSplitMosaicTile), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

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
            DependencyProperty.Register("Interval", typeof(TimeSpan), typeof(SfSplitMosaicTile), new PropertyMetadata(TimeSpan.FromSeconds(0), new PropertyChangedCallback(OnIntervalChanged)));

        #endregion

        #region Helper Methods

        void SimpleImageRotator_Loaded(object sender, RoutedEventArgs e)
        {
            HubTileService.Enqueue(this);
            if (!isrealoaded)
            {
                if (timer == null)
                    timer = new DispatcherTimer();

                if (randomImage == null)
                    randomImage = new Random();

                if (randomTile == null)
                    randomTile = new Random();

                if (ImageList == null && PART_SimpleImageContent!=null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        TransitionContentControl control = PART_SimpleImageContent.Children[i] as TransitionContentControl;
                        MosaicTileContent tilecontent = new MosaicTileContent();

                        tilecontent.Background = AccentBrush;
                        tilecontent.Opacity = GetRandomOpacity();
                        control.Content = tilecontent;
                    }
                }

                if (PART_SimpleImageContent != null && ImageList != null)
                {
                    string image = ImageList[randomImage.Next(ImageList.Count)];

                    for (int i = 0; i < 3; i++)
                    {
                        TransitionContentControl control = PART_SimpleImageContent.Children[i] as TransitionContentControl;
                        MosaicTileContent tilecontent = new MosaicTileContent();

                        tilecontent.ImageHeight = ActualHeight;
                        tilecontent.ImageWidth = ActualWidth;
                        tilecontent.Image = image;
                        tilecontent.VerticalImageAlignment = GetVerticalAlignment(i);

                        control.Content = tilecontent;
                    }
                }

                timer.Interval = TimeSpan.FromSeconds(1.3);
                timer.Tick -= timer_Tick;
                timer.Tick += timer_Tick;
            }
            if (timer != null && !IsFrozen)
                timer.Start();
            isrealoaded = true;
#if SILVERLIGHT
            if (timer != null && Interval!=TimeSpan.FromSeconds(0))
                timer.Interval = Interval;
#endif
            IsEnabledChanged += OnIsEnabledChanged;
            UpdateVisualState();
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

        void timer_Tick(object sender, object e)
        {
            int index = randomTile.Next(0, 3);

            if (previousindex != null && index == previousindex)
                return;

            MosaicTileContent tilecontent = new MosaicTileContent();
            TransitionContentControl control = PART_SimpleImageContent.Children[index] as TransitionContentControl;

            if (ImageList == null)
            {
                tilecontent.Background = AccentBrush;
                tilecontent.Opacity = GetRandomOpacity();
            }

            if (ImageList != null && ImageList.Count>0)
            {
                string image = ImageList[randomImage.Next(ImageList.Count)];

                if (!maximageStarted)
                {
                    if (!CheckForExistence(image))
                    {
                        tilecontent.ImageHeight = ActualHeight;
                        tilecontent.ImageWidth = ActualWidth;
                        tilecontent.Image = image;
                        tilecontent.VerticalImageAlignment = GetVerticalAlignment(index);
                        maximageStarted = true;
                    }
                    else
                    {
                        tilecontent.Image = null;
                        tilecontent.Background = AccentBrush;
                        tilecontent.Opacity = GetRandomOpacity();
                    }

                    previousimage = image;
                }
                else
                {

                    if (previousindex.HasValue)
                    {
                        if (temparray.Count == 0)
                            temparray.Add(previousindex.Value);

                        index = GetTilePosition(previousindex.Value, previousContent);

                        tilecontent.ImageWidth = ActualWidth;
                        tilecontent.ImageHeight = ActualHeight;


                        if (!temparray.Contains(index))
                        {
                            control = PART_SimpleImageContent.Children[index] as TransitionContentControl;
                            temparray.Add(index);
                        }
                        else
                        {
                            foreach (var value in randomarray)
                            {
                                if (value != previousindex && !temparray.Contains(value))
                                {
                                    index = value;
                                }
                            }
                            control = PART_SimpleImageContent.Children[index] as TransitionContentControl;
                            temparray.Add(index);
                        }

                        tilecontent.VerticalImageAlignment = GetVerticalAlignment(index);
                        tilecontent.Image = previousimage;

                        if (temparray.Count == 3)
                        {
                            maximageStarted = false;
                            temparray.Clear();
                        }
                    }
                }
            }
            control.Content = tilecontent;
            previousContent = tilecontent;
            previousindex = index;
        }

        private int CheckAgainForExistence(List<int> temparray, int? previousvalue, int index)
        {
            int arrindex = temparray.IndexOf(index);
            int retindex = arrindex++;
            if (temparray.ElementAt(retindex) != previousvalue)
            {
                return temparray.ElementAt(arrindex++);
            }
            else
            {
                return CheckAgainForExistence(temparray, previousvalue, temparray.ElementAt(retindex));
            }

        }

        private int GetTilePosition(int index, MosaicTileContent tilecontent)
        {
            Random random = new Random();

            if (tilecontent.VerticalImageAlignment == VerticalAlignment.Top)
            {
                return random.Next(1, 2);
            }
            else if (tilecontent.VerticalImageAlignment == VerticalAlignment.Center)
            {
                int rndmval = random.Next(0, 1);
                if (rndmval == 0)
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                return random.Next(0, 1);
            }
        }

        private bool CheckForExistence(string image)
        {
            foreach (FrameworkElement element in PART_SimpleImageContent.Children)
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

        private VerticalAlignment GetVerticalAlignment(int index)
        {
            if (index == 0)
            {
                return VerticalAlignment.Top;
            }
            else if (index == 1)
            {
                return VerticalAlignment.Center;
            }
            else
            {
                return VerticalAlignment.Bottom;
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
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SplitMosaicTile"/>
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_SimpleImageContent = GetTemplateChild("PART_SimpleImageContent") as Grid;
            PART_Header = GetTemplateChild("PART_Header") as ContentPresenter;

            if (PART_Header != null)
            {
                if (this.HeaderTemplate != null)
                    PART_Header.Height = this.ActualHeight / 2;
            }

            base.OnApplyTemplate();
#if SILVERLIGHT
            isrealoaded = false;
            this.Loaded+=SimpleImageRotator_Loaded;
#endif
        }

        #endregion

        #region Callback Methods

        private static void OnIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfSplitMosaicTile tile = sender as SfSplitMosaicTile;
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
