// <copyright file="VistaFlipSwitchPreviewControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class presents actions for like Vista Flip element switch.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class VistaFlipSwitchPreviewControl : SwicthPreviewControlBase
    {
        #region Constans
        /// <summary>
        /// This is needs for create custom name.
        /// </summary>
        private const string CUTOM_NAME_PREFIX = "CustomName";

        /// <summary>
        /// Default value for x axis.
        /// </summary>
        private const double RESET_VALUE_OF_X = -0.2;

        /// <summary>
        /// Default value for y axis.
        /// </summary>
        private const double RESET_VALUE_OF_Y = -0.45;

        /// <summary>
        /// Default value for z axis.
        /// </summary>
        private const double RESET_VALUE_OF_Z = 0;

        /// <summary>
        /// Count of view visible items.
        /// </summary>
        private const int COUNT_OF_VIEW = 5;

        /// <summary>
        /// Count of view away items.
        /// </summary>
        private const double AWAY_FACTOR = 1.3;

        /// <summary>
        /// Presents start value of FieldOfView.
        /// </summary>
        private const double START_FIELDOFVIEW = 9.5;

        /// <summary>
        /// Presents end value of FieldOfView.
        /// </summary>
        private const double END_FIELDOFVIEW = 13;

        /// <summary>
        /// Presents desired frame rate of animation.
        /// </summary>
        private readonly int? DESIRED_FRAME_RATE = null;
        #endregion

        #region Private members
        /// <summary>
        /// Original point. 
        /// </summary>
        private static readonly Point ORIGIN_POINT = new Point(0, 0);

        /// <summary>
        /// This member presents Viewport3D for 3D scene.
        /// </summary>
        private readonly Viewport3D m_Viewport;

        /// <summary>
        /// This member presents Dictionary3D for 3D scene.
        /// </summary>
        private readonly Dictionary3D m_VisualTo3DModelMap = new Dictionary3D();

        /// <summary>
        /// This is collections items.
        /// </summary>
        private readonly List<UIElement> m_Collections = new List<UIElement>();

        /// <summary>
        /// Presents values that indicates or start animation is completed. 
        /// </summary>
        private bool m_isAnimationCompleted = true;

        /// <summary>
        /// This member indicates whether items move.
        /// </summary>
        private bool m_isMovingItems;

        /// <summary>
        /// It is change value for create custom name.
        /// </summary>
        private int m_nameVariable = 0;

        /// <summary>
        /// This member presents Panel3DAdorner for 3D scene.
        /// </summary>
        private Panel3DAdorner m_flipAdorner = null;

        /// <summary>
        /// Presents member for cut away of selected item.
        /// </summary>
        private CutawayAdorner m_textAddorner = null;

        /// <summary>
        /// This member presents adorner layer of control.
        /// </summary>
        private AdornerLayer m_adornerLayer = null;

        /// <summary>
        /// Presents animation for switch items.
        /// </summary>
        private DoubleAnimation m_animation = null;

        /// <summary>
        /// Presents finished animation.
        /// </summary>
        private DoubleAnimation m_ainishedAnimation = null;

        /// <summary>
        /// Presents owner.
        /// </summary>
        private IVistaFlipOwner m_owner = null;

        /// <summary>
        /// Value for x axis.
        /// </summary>
        private double m_offsetX = RESET_VALUE_OF_X;

        /// <summary>
        /// Value for y axis.
        /// </summary>
        private double m_offsetY = RESET_VALUE_OF_Y;

        /// <summary>
        /// Value for z axis.
        /// </summary>
        private double m_offsetZ = RESET_VALUE_OF_Z;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="VistaFlipSwitchPreviewControl"/> class.
        /// </summary>
        static VistaFlipSwitchPreviewControl()
        {
            EnvironmentTest.ValidateLicense(typeof(VistaFlipSwitchPreviewControl));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaFlipSwitchPreviewControl), new FrameworkPropertyMetadata(typeof(VistaFlipSwitchPreviewControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VistaFlipSwitchPreviewControl"/> class.
        /// </summary>
        public VistaFlipSwitchPreviewControl()
        {
            m_Viewport = new Viewport3D
            {
                Camera = new PerspectiveCamera
                {
                    Position = new Point3D(0, 0, 7),
                    LookDirection = new Vector3D(0, 0, -1),
                }
            };
            NameScope.SetNameScope(m_Viewport, new NameScope());

            ModelVisual3D visual3D = new ModelVisual3D
            {
                Content = new AmbientLight(Colors.Transparent)
            };
            m_Viewport.Children.Add(visual3D);
            Unloaded += new RoutedEventHandler(VistaFlipSwitchPreviewControl_Unloaded);
        }

        void VistaFlipSwitchPreviewControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_animation != null)
                m_animation.Completed -= new EventHandler(OnAnimationCompleted);
            if (m_ainishedAnimation != null) 
                m_ainishedAnimation.Completed -= new EventHandler(OnFinishedAnimationCompleted);
            
        }
        #endregion

        #region Preoperties
        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public override object SelectedItem
        {
            get
            {
                if (m_Viewport.Children.Count == 1)
                {
                    return null;
                }

                FrameworkElement visual = (FrameworkElement)((Viewport2DVisual3D)m_Viewport.Children[1]).Visual;
                return GetItemFromSelectedWrapper(visual);
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public override IEnumerable Items
        {
            get
            {
                return m_Collections;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [used main collection].
        /// </summary>
        /// <value><c>true</c> if [used main collection]; otherwise, <c>false</c>.</value>
        public override bool UsedMainCollection
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        protected virtual IVistaFlipOwner Owner
        {
            get
            {
                if (null == m_owner)
                {
                    m_owner = (IVistaFlipOwner)TemplatedParent;
                }

                return m_owner;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item ContentControl.</param>
        public override void AddItem(ContentControl item)
        {
            m_Collections.Add(item);
            Viewport2DVisual3D model = BuildInteractive3DModel(item);
            m_VisualTo3DModelMap.Add(item, model);
            m_Viewport.Children.Add(model);
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        public override void ClearItems()
        {
            BeginFinishedAnimation();
            StartFieldOfViewAnimation(END_FIELDOFVIEW / Owner.FactoryOfViewVistaFlip, START_FIELDOFVIEW, true);
        }

        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="isForward">if set to <c>true</c> [is forward].</param>
        /// <param name="switchDirection">The switch direction.</param>
        public override void MoveItem(bool isForward, SwitchDirection switchDirection)
        {
            if (m_isAnimationCompleted)
            {
                if (m_isMovingItems || m_Viewport.Children.Count < 3)
                {
                    return;
                }

                List<Viewport2DVisual3D> viewport2Ds = new List<Viewport2DVisual3D>();
                List<TranslateTransform3D> transforms = new List<TranslateTransform3D>();
                PrepareListOfTransformation(viewport2Ds, transforms);

                double aratioc;
                double dratioc;
                double aratio;
                double dratio;

                if (isForward)
                {
                    ReplaceItems(viewport2Ds, 0, viewport2Ds.Count - 1, 1, m_Viewport.Children.Count - 1);
                    aratioc = 0;
                    dratioc = 1;
                    aratio = 0.7;
                    dratio = 0.3;
                }
                else
                {
                    ReplaceItems(viewport2Ds, viewport2Ds.Count - 1, 0, m_Viewport.Children.Count - 1, 1);
                    aratioc = 1;
                    dratioc = 0;
                    aratio = 0.3;
                    dratio = 0.7;
                }

                Storyboard story = new Storyboard();
                Duration duration = Owner.VistaFlipAnimationDuration;

                for (int i = 0; i < transforms.Count; ++i)
                {
                    double targetX = transforms[i].OffsetX;
                    double targetY = transforms[i].OffsetY;
                    double targetZ = transforms[i].OffsetZ;

                    TranslateTransform3D trans = (TranslateTransform3D)((Transform3DGroup)viewport2Ds[i].Transform).Children[0];
                    string name = GetNextName();
                    m_Viewport.RegisterName(name, trans);

                    DoubleAnimation anim = CreateAnimation(targetX, duration, aratioc, dratioc, "OffsetX", name, story);
                    CreateAnimation(targetY, duration, aratio, dratio, "OffsetY", name, story);
                    CreateAnimation(targetZ, duration, dratio, aratio, "OffsetZ", name, story);
                    FrameworkElement contentPres = (FrameworkElement)viewport2Ds[i].Visual;
                    CreateAnimation(GetOpacityTarget(i), duration, 0.2, 0.8, "Opacity", contentPres.Name, story);

                    if (i == 0)
                    {
                        if (null != m_animation)
                        {
                            anim.Completed -= new EventHandler(OnAnimationCompleted);
                        }

                        m_animation = anim;
                        m_animation.Completed += new EventHandler(OnAnimationCompleted);
                    }
                }

                m_isMovingItems = true;
                Timeline.SetDesiredFrameRate(story, DESIRED_FRAME_RATE);
                story.Begin(m_Viewport);
                m_textAddorner.SetText(DocumentContainer.GetHeader((DependencyObject)SelectedItem).ToString());
            }
        }

        /// <summary>
        /// Shows the selected item.
        /// </summary>
        public override void ShowSelectedItem()
        {
            m_flipAdorner.PerentSize = Owner.RenderSize;
            m_adornerLayer.Add(m_flipAdorner);
            m_adornerLayer.Add(m_textAddorner);
            m_offsetX = RESET_VALUE_OF_X;
            m_offsetY = RESET_VALUE_OF_Y;
            m_offsetZ = RESET_VALUE_OF_Z;

            m_textAddorner.StarAnimation();
            StartFieldOfViewAnimation(START_FIELDOFVIEW, END_FIELDOFVIEW / Owner.FactoryOfViewVistaFlip, false);
            Focus();
            m_textAddorner.SetText(DocumentContainer.GetHeader((DependencyObject)SelectedItem).ToString());
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Framework Element</returns>
        protected virtual FrameworkElement GetWindow(FrameworkElement element)
        {
            MDIWindow window = element.DataContext as MDIWindow;

            if (null == window)
            {
                FrameworkElement content = (FrameworkElement)((TabItemExt)element.DataContext).Content;
                content.Arrange(new Rect(new Point(0, 0), Owner.RenderSize));
                return content;
            }

            return window;
        }

        /// <summary>
        /// Gets the item from selected wrapper.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <returns> object window Content</returns>
        protected virtual object GetItemFromSelectedWrapper(FrameworkElement visual)
        {
            MDIWindow window = visual.DataContext as MDIWindow;

            if (null == window)
            {
                if ((visual.DataContext as FrameworkElement).Parent is TabItemExt)
                {
                    DocumentContainer.SetHeader(visual.DataContext as DependencyObject, ((visual.DataContext as FrameworkElement).Parent as TabItemExt).Header);
                }
                if (visual.DataContext is ContentPresenter)
                    return (visual.DataContext as ContentPresenter).Content;
                else
                    return visual.DataContext;
            }

            if ((window as MDIWindow).DocumentHeader.Header != null)
            {
                DocumentContainer.SetHeader(window.Content as DependencyObject, (window as MDIWindow).DocumentHeader.Header);
            }
            return window.Content;
        }

        /// <summary>
        /// Gets the size of the item.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Rect GetMDIBounds</returns>
        protected virtual Rect GetItemSize(FrameworkElement element)
        {
            return DocumentContainer.GetMDIBounds(((MDIWindow)element.DataContext).Content);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            m_adornerLayer = AdornerLayer.GetAdornerLayer(this);
            m_flipAdorner = new Panel3DAdorner(this, m_Viewport);
            m_textAddorner = new CutawayAdorner(this, Owner.VistaFlipAnimationDuration);
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_Viewport.Arrange(new Rect(ORIGIN_POINT, Owner.RenderSize));
            return Owner.RenderSize;
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            m_Viewport.Measure(Owner.RenderSize);
            return m_Viewport.DesiredSize;
        }

        /// <summary>
        /// Called when [finished animation completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnFinishedAnimationCompleted(object sender, EventArgs e)
        {
            m_ainishedAnimation.Completed -= new EventHandler(OnFinishedAnimationCompleted);
            m_Collections.Clear();

            foreach (Viewport2DVisual3D model in m_VisualTo3DModelMap.Values)
            {
                m_Viewport.Children.Remove(model);
            }

            m_VisualTo3DModelMap.Clear();
            m_adornerLayer.Remove(m_flipAdorner);
            m_adornerLayer.Remove(m_textAddorner);
            Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Begins the finished animation.
        /// </summary>
        private void BeginFinishedAnimation()
        {
            List<Viewport2DVisual3D> viewport2Ds = new List<Viewport2DVisual3D>();
            List<TranslateTransform3D> transforms = new List<TranslateTransform3D>();
            PrepareListOfTransformation(viewport2Ds, transforms);
            Storyboard story = new Storyboard();
            Duration duration = Owner.VistaFlipAnimationDuration;
            int cnt = Math.Min(transforms.Count, COUNT_OF_VIEW);

            for (int i = 0; i < cnt; ++i)
            {
                TranslateTransform3D trans = (TranslateTransform3D)((Transform3DGroup)viewport2Ds[i].Transform).Children[0];
                string name = GetNextName();
                m_Viewport.RegisterName(name, trans);

                CreateAnimation(0, duration, 0.2, 0.8, "OffsetX", name, story);
                CreateAnimation(0, duration, 0.2, 0.8, "OffsetY", name, story);
                CreateAnimation(0, duration, 0.2, 0.8, "OffsetZ", name, story);
                FrameworkElement contentPres = (FrameworkElement)viewport2Ds[i].Visual;
                CreateAnimation(1, duration, 0.2, 0.8, "Opacity", contentPres.Name, story);
            }

            Timeline.SetDesiredFrameRate(story, DESIRED_FRAME_RATE);
            story.Begin(m_Viewport);
            m_textAddorner.ExitAnimation();
        }

        /// <summary>
        /// Starts the field of view animation.
        /// </summary>
        /// <param name="from">From StartFieldOfViewAnimation.</param>
        /// <param name="to">To StartFieldOfViewAnimation.</param>
        /// <param name="isFinished">if set to <c>true</c> [is finished].</param>
        private void StartFieldOfViewAnimation(double from, double to, bool isFinished)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = Owner.VistaFlipAnimationDuration,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8
            };

            if (isFinished)
            {
                m_ainishedAnimation = animation;
                m_ainishedAnimation.Completed += new EventHandler(OnFinishedAnimationCompleted);
            }

            Timeline.SetDesiredFrameRate(animation, DESIRED_FRAME_RATE);
            m_Viewport.Camera.BeginAnimation(PerspectiveCamera.FieldOfViewProperty, animation);
        }

        /// <summary>
        /// Gets the name of the next.
        /// </summary>
        /// <returns>string value type</returns>
        private string GetNextName()
        {
            return CUTOM_NAME_PREFIX + m_nameVariable++;
        }

        /// <summary>
        /// Builds the interactive3D model.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Viewport2D Visual3D</returns>
        private Viewport2DVisual3D BuildInteractive3DModel(FrameworkElement element)
        {
            FrameworkElement window = GetWindow(element);
            PreviewBorder previewBorder = CreatePreview(window);

            Size winSize = GetWindowSize(window);
            double winHeight = winSize.Height;
            double winWidth = winSize.Width;
            double factorHeight;
            double factorWidth;

            if (Owner.IsItemsInFullScreen)
            {
                double factor = Math.Max(winHeight, winWidth);
                factorHeight = winHeight / factor;
                factorWidth = winWidth / factor;
            }
            else
            {
                Size ownerSize = Owner.RenderSize;
                double ownerHeight = ownerSize.Height;
                double ownerWidth = ownerSize.Width;
                factorHeight = winSize.Height < ownerHeight ? winHeight / ownerHeight : 1;
                factorWidth = winSize.Width < ownerWidth ? winWidth / ownerWidth : 1;
            }

            Viewport2DVisual3D model = CreateModel(previewBorder, element, factorHeight, factorWidth);

            return model;
        }

        /// <summary>
        /// Replaces the items.
        /// </summary>
        /// <param name="viewport2Ds">The viewport2 ds.</param>
        /// <param name="index2D3D">The index2 d3 D.</param>
        /// <param name="addIndex2D3D">The add index2 d3 D.</param>
        /// <param name="index3D">The index3 D.</param>
        /// <param name="addIndex3D">The add index3 D.</param>
        private void ReplaceItems(IList<Viewport2DVisual3D> viewport2Ds, int index2D3D, int addIndex2D3D, int index3D, int addIndex3D)
        {
            Viewport2DVisual3D firstGeo = viewport2Ds[index2D3D];
            viewport2Ds.RemoveAt(index2D3D);
            viewport2Ds.Insert(addIndex2D3D, firstGeo);

            Visual3D firstChild = m_Viewport.Children[index3D];
            m_Viewport.Children.RemoveAt(index3D);
            m_Viewport.Children.Insert(addIndex3D, firstChild);
        }

        /// <summary>
        /// Gets the opacity target.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>double value type </returns>
        private double GetOpacityTarget(int index)
        {
            return Owner.FirstFlipItemOpacity / (index * Owner.OpacityFactorOfVistaFlip + 1);
        }

        /// <summary>
        /// Called when [animation completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAnimationCompleted(object sender, EventArgs e)
        {
            m_isMovingItems = false;
            m_animation.Completed -= new EventHandler(OnAnimationCompleted);
        }

        /// <summary>
        /// Creates the animation.
        /// </summary>
        /// <param name="target">The target CreateAnimation.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="acRatio">The ac ratio.</param>
        /// <param name="decRatio">The dec ratio.</param>
        /// <param name="path">The path CreateAnimation. </param>
        /// <param name="name">The name CreateAnimation.</param>
        /// <param name="story">The story CreateAnimation.</param>
        /// <returns>Double Animation</returns>
        private static DoubleAnimation CreateAnimation(double target, Duration duration, double acRatio, double decRatio, string path, string name, TimelineGroup story)
        {
            DoubleAnimation anim = new DoubleAnimation
            {
                To = target,
                Duration = duration,
                AccelerationRatio = acRatio,
                DecelerationRatio = decRatio
            };
            Storyboard.SetTargetProperty(anim, new PropertyPath(path));
            Storyboard.SetTargetName(anim, name);
            story.Children.Add(anim);

            return anim;
        }
        #endregion

        #region CreateElements
        /// <summary>
        /// Creates the preview.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Preview Border</returns>
        private PreviewBorder CreatePreview(FrameworkElement element)
        {
            PreviewBorder previewBorder = new PreviewBorder(Stretch.Uniform)
            {
                DataContext = element,
                Width = element.ActualWidth,
                Height = element.ActualHeight,
                Name = GetNextName(),
            };

            m_Viewport.RegisterName(previewBorder.Name, previewBorder);

            Storyboard story = new Storyboard();
            double opacityTarget = GetOpacityTarget(m_Viewport.Children.Count - 1);
            CreateAnimation(opacityTarget, Owner.VistaFlipAnimationDuration, 0.2, 0.8, "Opacity", previewBorder.Name, story);
            Timeline.SetDesiredFrameRate(story, DESIRED_FRAME_RATE);
            story.Begin(m_Viewport);

            return previewBorder;
        }

        /// <summary>
        /// Creates the model.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="element">The element.</param>
        /// <param name="factorH">The factor height.</param>
        /// <param name="factorW">The factor width.</param>
        /// <returns>Viewport2D Visual3D</returns>
        private Viewport2DVisual3D CreateModel(Visual visual, FrameworkElement element, double factorH, double factorW)
        {
            Viewport2DVisual3D model = new Viewport2DVisual3D
            {
                Geometry = new MeshGeometry3D
                {
                    TriangleIndices = new Int32Collection(
                        new int[] { 0, 1, 2, 2, 3, 0 }),
                    TextureCoordinates = new PointCollection(
                        new Point[] 
                            { 
                                new Point(0, 1), 
                                new Point(1, 1), 
                                new Point(1, 0), 
                                new Point(0, 0) 
                            }),
                    Positions = new Point3DCollection(
                        new Point3D[] 
                            { 
                                new Point3D(0, 0, 0), 
                                new Point3D(factorW, 0, 0), 
                                new Point3D(factorW, factorH, 0), 
                                new Point3D(0, factorH, 0) 
                            })
                },
                Material = new DiffuseMaterial
                {
                    Brush = Brushes.Transparent
                },

                Transform = GetTransform3DGroup(element, factorH, factorW),
                Visual = visual,
            };

            Viewport2DVisual3D.SetIsVisualHostMaterial(model.Material, true);

            return model;
        }

        /// <summary>
        /// Gets the size of the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>Size RenderSize</returns>
        private Size GetWindowSize(UIElement window)
        {
            return Owner.IsItemsInFullScreen
                ? Owner.RenderSize : window.RenderSize;
        }

        /// <summary>
        /// Gets the transform3D group.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="factorH">The factor Height.</param>
        /// <param name="factorW">The factor Width.</param>
        /// <returns>Transform 3DGroup</returns>
        private Transform3DGroup GetTransform3DGroup(FrameworkElement element, double factorH, double factorW)
        {
            double count = m_Viewport.Children.Count - 1;

            if (0 != count)
            {
                if (COUNT_OF_VIEW >= count)
                {
                    m_offsetY += 0.5 / (count * count + count);
                    m_offsetZ -= 0.5 / count;
                }
                else if (COUNT_OF_VIEW * AWAY_FACTOR >= count)
                {
                    m_offsetX -= 0.01;
                    m_offsetY -= 0.01;
                    m_offsetZ -= 0.01;
                }
                else if (Owner.KeepLimitedVistaItemsStack)
                {
                    m_offsetX = -10;
                }
            }

            Transform3DGroup group3D = new Transform3DGroup();
            SetTransform3D(group3D, element, factorH, factorW, (int)count);
            SetAxisAngle(group3D);

            return group3D;
        }

        /// <summary>
        /// Sets the transform3D.
        /// </summary>
        /// <param name="group3D">The group3D.</param>
        /// <param name="element">The element.</param>
        /// <param name="factorH">The factor Height.</param>
        /// <param name="factorW">The factor Width.</param>
        /// <param name="count">The count.</param>
        private void SetTransform3D(Transform3DGroup group3D, FrameworkElement element, double factorH, double factorW, int count)
        {
            TranslateTransform3D translate3D;

            if (COUNT_OF_VIEW * AWAY_FACTOR >= count)
            {
                double offsetX;
                double offsetY;

                if (Owner.IsItemsInFullScreen)
                {
                    offsetX = -factorW / 2;
                    offsetY = -factorH / 2;
                }
                else
                {
                    Rect itemRect = GetItemSize(element);
                    offsetX = GetOffsetX(itemRect.X);
                    offsetY = GetOffsetY(itemRect.Y, factorH);
                }

                translate3D = new TranslateTransform3D
                {
                    OffsetX = offsetX,
                    OffsetY = offsetY,
                    OffsetZ = 0
                };

                bool isSubscribeCompleted = 0 == count;
                CreateAniamtionForOffset(translate3D, m_offsetX, TranslateTransform3D.OffsetXProperty, false);
                CreateAniamtionForOffset(translate3D, m_offsetY, TranslateTransform3D.OffsetYProperty, false);
                CreateAniamtionForOffset(translate3D, m_offsetZ, TranslateTransform3D.OffsetZProperty, isSubscribeCompleted);
            }
            else
            {
                translate3D = new TranslateTransform3D
                {
                    OffsetX = m_offsetX,
                    OffsetY = m_offsetY,
                    OffsetZ = m_offsetZ
                };
            }

            group3D.Children.Add(translate3D);
        }

        /// <summary>
        /// Gets the offset X.
        /// </summary>
        /// <param name="x">The x RenderSize.</param>
        /// <returns>double RenderSize</returns>
        private double GetOffsetX(double x)
        {
            double midleX = Owner.RenderSize.Width / 2;
            return (x - midleX) / midleX * 0.5;
        }

        /// <summary>
        /// Gets the offset Y.
        /// </summary>
        /// <param name="y">The y RenderSize.</param>
        /// <param name="factorH">The factor height</param>
        /// <remarks>Before optimization: -( y - midleY ) / midleY * 0.5 - ( 0.5 - factorH / 2 );</remarks>
        /// <returns>double GetOffsetY</returns>
        private double GetOffsetY(double y, double factorH)
        {
            double midleY = Owner.RenderSize.Height / 2;
            return (factorH - y / midleY) / 2;
        }

        /// <summary>
        /// Creates the animation for offset.
        /// </summary>
        /// <param name="translate3D">The translate3D.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="depProperty">The dependency property.</param>
        /// <param name="isSubscribeCompleted">if set to <c>true</c> [is subscribe completed].</param>
        private void CreateAniamtionForOffset(IAnimatable translate3D, double offset, DependencyProperty depProperty, bool isSubscribeCompleted)
        {
            m_isAnimationCompleted = false;

            DoubleAnimation animOffset = new DoubleAnimation
            {
                To = offset,
                Duration = Owner.VistaFlipAnimationDuration,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8
            };

            if (isSubscribeCompleted)
            {
                animOffset.Completed += new EventHandler(OnAnimOffsetCompleted);
            }

            translate3D.BeginAnimation(depProperty, animOffset);
        }

        /// <summary>
        /// Called when [anim offset completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAnimOffsetCompleted(object sender, EventArgs e)
        {
            m_isAnimationCompleted = true;
            m_isMovingItems = false;
        }

        /// <summary>
        /// Prepares the list of transformation.
        /// </summary>
        /// <param name="viewport2Ds">The viewport2Ds.</param>
        /// <param name="transforms">The transforms.</param>
        private void PrepareListOfTransformation(ICollection<Viewport2DVisual3D> viewport2Ds, ICollection<TranslateTransform3D> transforms)
        {
            foreach (Visual3D model in m_Viewport.Children)
            {
                Viewport2DVisual3D viewport2D = model as Viewport2DVisual3D;

                if (null != viewport2D)
                {
                    viewport2Ds.Add(viewport2D);
                    Transform3DGroup group = (Transform3DGroup)viewport2D.Transform;
                    transforms.Add((TranslateTransform3D)group.Children[0]);
                }
            }
        }

        /// <summary>
        /// Sets the axis angle.
        /// </summary>
        /// <param name="group3D">The group3 D.</param>
        private void SetAxisAngle(Transform3DGroup group3D)
        {
            AxisAngleRotation3D rotation3D = new AxisAngleRotation3D
            {
                Axis = new Vector3D(0, 3, 0),
                Angle = 15
            };
            RotateTransform3D rotate3D = new RotateTransform3D
            {
                Rotation = rotation3D
            };
            group3D.Children.Add(rotate3D);

            DoubleAnimation anim = new DoubleAnimation
            {
                To = 30,
                Duration = Owner.VistaFlipAnimationDuration,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8
            };
            rotation3D.BeginAnimation(AxisAngleRotation3D.AngleProperty, anim);
        }
        #endregion
    }
}