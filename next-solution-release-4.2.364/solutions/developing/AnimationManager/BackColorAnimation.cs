using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using Utilities.Animations;
using Utilities.WPF;
using Utilities;
#else
using System.Drawing;
#endif
using System.ComponentModel;
using UFInterfaces;
using StringManager.ComponentService;
using DocumentManager.ComponentService;
#if !WINDOWS_UWP
using System.Windows.Media;
#if !NET_STANDARD
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
#endif
#else
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
#endif

namespace AnimationManager
{
    [DataContract(Name = "BackColorAnimation")]
    public class BackColorAnimation : AnimationManager
    {
#region Members
#if !NET_STANDARD
        bool bSaved;
        Brush oldBrush;
        Dictionary<UIElement, Brush> mapOldBrush;
        Dictionary<ContentControl, String> mapOldContent;
        List<FrameworkElement> listPendingLoaded;
        List<Brush> listClonedBrush;
        Color currentColor;
        Color currentBlinkColor;
        int currentBlinkTime;
        String currentText;
        Object content;
#if !WINDOWS_UWP
        Dictionary<GeometryModel3D, Material> mapActive3dModels;
        Dictionary<GeometryModel3D, Material> mapBackActive3dModels;
        List<WPFUtilities.PropertyChangeNotifier> listPropertyChangeNotifier;
#endif
#endif
        #endregion

        #region Properties

        [DataMember]
        List<ColorData> listColors = new List<ColorData>();
#if !WINDOWS_UWP
[Browsable(false)]
#endif
        public List<ColorData> ListColors
        {
            get 
            {
                if (listColors == null)
                    listColors = new List<ColorData>();
                if (listColors.Count == 0)
                    listColors.Add(new ColorData() { Color =
                        Colors.Red,
                        Value = 0 });
                return listColors; 
            }
            set
            {
                if (value == listColors)
                    return;
                listColors = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("ListColors");
#endif
                Reexecute();
#endif
            }
        }

        bool applyToAllChild = true;
        [DataMember]
        public bool ApplyToAllChild
        {
            get { return applyToAllChild; }
            set
            {
                if (value == applyToAllChild)
                    return;
                applyToAllChild = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("ApplyToAllChild");
#endif
                Reexecute();
#endif
            }
        }

#endregion

#if !NET_STANDARD
#if !WINDOWS_UWP
        static readonly String tagBackground = Properties.Settings.Default.BackgroundTag;
#else
        static readonly String tagBackground = "Background";
#endif
#endif

#region Methods
#if !NET_STANDARD
        void SetControlBrush(Brush brush, bool bStopping = false)
        {
#if !WINDOWS_UWP
            if (Control3D != null)
            {
                if (brush == null)
                {
                    if (mapActive3dModels != null)
                    {
                        foreach (var model in mapActive3dModels.Keys)
                        {
                            model.Material = mapActive3dModels[model];
                        }
                        mapActive3dModels.Clear();
                    }
                    if (mapBackActive3dModels != null)
                    {
                        foreach (var model in mapBackActive3dModels.Keys)
                            model.BackMaterial = mapBackActive3dModels[model];
                        mapBackActive3dModels.Clear();
                    }

                    currentColor = Colors.Transparent;
                }
                else
                {
                    var geometries = new List<GeometryModel3D>();
                    if (Control3D is GeometryModel3D)
                        geometries.Add(Control3D as GeometryModel3D);
                    else if (Control3D is Model3DGroup)
                        geometries = DependencyObjectExtensions.GetAllGeometries(Control3D as Model3DGroup);

                    foreach (var geometry in geometries)
                    {
                        geometry.Material = new DiffuseMaterial()
                        {
                            Brush = brush
                        };
                        geometry.BackMaterial = new DiffuseMaterial()
                        {
                            Brush = brush
                        };
                    }
                }
            }
            else
#endif
            {
                if (Control is Panel)
                    (Control as Panel).Background = brush;
                else if (Control is Control)
                    (Control as Control).Background = brush;
                else if (Control is Shape)
                    (Control as Shape).Fill = brush;
                else if (Control is Border)
                    (Control as Border).Background = brush;
                else if (Control is ContentControl)
                {
                    var contentControl = Control as ContentControl;
                    if (contentControl.Content is Panel)
                        (contentControl.Content as Panel).Background = brush;
                    else if (contentControl.Content is Control)
                        (contentControl.Content as Control).Background = brush;
                    else if (contentControl.Content is Shape)
                        (contentControl.Content as Shape).Fill = brush;
                    else if (Control is Border)
                        (Control as Border).Background = brush;
                }

                if (Control != null)
                {
                    if (mapOldBrush == null)
                        mapOldBrush = new Dictionary<UIElement, Brush>();
                    else if (!bStopping)
                        mapOldBrush.Clear();

                    if (!bStopping)
                    {
                        var childs = Control.GetVisualChildrenOfType<UIElement>();

                        (from c in childs.OfType<Panel>()
                         where (c.Tag as String) == tagBackground
                         select c).ToList().ForEach(child =>
                         {
                             if (!mapOldBrush.ContainsKey(child))
                                 mapOldBrush.Add(child, child.Background);
                         });
                        (from c in childs.OfType<Control>()
                         where (c.Tag as String) == tagBackground || ApplyToAllChild
                         select c).ToList().ForEach(child =>
                         {
                             if (!mapOldBrush.ContainsKey(child))
                                 mapOldBrush.Add(child, child.Background);
                         });
#if !WINDOWS_UWP
                        (from c in childs.OfType<System.Windows.Shapes.Shape>()
#else
                    (from c in childs.OfType<Shape>()
#endif
                         where (c.Tag as String) == tagBackground || ApplyToAllChild
                         select c).ToList().ForEach(child =>
                         {
                             if (!mapOldBrush.ContainsKey(child))
                                 mapOldBrush.Add(child, child.Fill);
                         });
                        (from c in childs.OfType<Border>()
                         where (c.Tag as String) == tagBackground || ApplyToAllChild
                         select c).ToList().ForEach(child =>
                         {
                             if (!mapOldBrush.ContainsKey(child))
                                 mapOldBrush.Add(child, child.Background);
                         });
                    }
                }

                if (mapOldBrush != null)
                {
                    mapOldBrush.Keys.ToList().ForEach(control =>
                    {
                        var newBrush = brush;
                        var ob = mapOldBrush[control];
                        if (ob is GradientBrush)
                        {
#if !WINDOWS_UWP
                            var gradientBrush = (ob as GradientBrush).Clone();
                            if (listClonedBrush == null)
                                listClonedBrush = new List<Brush>();
                            listClonedBrush.Add(gradientBrush);
#else
                            var gradientBrush = (ob as GradientBrush);
#endif
                            if (currentBlinkTime != 0)
                                gradientBrush.GradientBrushColorAnimation(currentBlinkColor,
                                    currentColor, currentBlinkTime, true, true, AnimationEquation);
                            else
                                gradientBrush.GradientBrushColorAnimation(currentColor,
                                    currentColor, AnimationTime, Repeatable, Autoreverse, AnimationEquation);
                            newBrush = gradientBrush;
                        }
                        
                        if (control is Panel)
                            (control as Panel).Background = newBrush;
                        else if (control is Control)
                            (control as Control).Background = newBrush;
                        else if (control is Shape)
                            (control as Shape).Fill = newBrush;
                        else if (control is Border)
                            (control as Border).Background = newBrush;
                    });
                }
            }
        }

        Brush GetControlBrush()
        {
            if (Control is ContentControl)
                content = (Control as ContentControl).Content;
            else if (Control is TextBox)
                content = (Control as TextBox).Text;
            else if (Control is TextBlock)
                content = (Control as TextBlock).Text;

            if (Control != null)
            {
                if (mapOldBrush == null)
                {
                    mapOldBrush = new Dictionary<UIElement, Brush>();

#if !WINDOWS_UWP
                    if (ApplyToAllChild)
                        ScanChildContentControls(Control);
#endif
                    SaveOldChildBrushes(Control);
                }
            }

#if !WINDOWS_UWP
            if (Control3D != null)
            {
                if (mapActive3dModels == null)
                    mapActive3dModels = new Dictionary<GeometryModel3D, Material>();
                if (mapBackActive3dModels == null)
                    mapBackActive3dModels = new Dictionary<GeometryModel3D, Material>();

                var geometries = new List<GeometryModel3D>();
                if (Control3D is GeometryModel3D)
                    geometries.Add(Control3D as GeometryModel3D);
                else if (Control3D is Model3DGroup)
                    geometries = DependencyObjectExtensions.GetAllGeometries(Control3D as Model3DGroup);

                foreach (var geometry in geometries)
                {
                    mapActive3dModels.Add(geometry, geometry.Material);
                    mapBackActive3dModels.Add(geometry, geometry.BackMaterial);
                }
            }
            else
#endif
                if (Control is Panel)
                return (Control as Panel).Background;
            else if (Control is Control)
                return (Control as Control).Background;
            else if (Control is Shape)
                return (Control as Shape).Fill;
            else if (Control is Border)
                return (Control as Border).Background;

            return null;
        }

        void RestoreContent()
        {
            if (content != null)
            {
                if (Control is ContentControl)
                    (Control as ContentControl).Content = content;
                else if (Control is TextBox)
                    (Control as TextBox).Text = content as String;
                else if (Control is TextBlock)
                    (Control as TextBlock).Text = content as String;
                currentText = null;
                // content = null;
            }
        }

        void CreateBrush(double value)
        {
            try
            {
                var list = (from c in ListColors where c.Value <= value orderby c ascending select c).ToList();
                var color = ListColors.Last();
                if (list.Count > 0)
                    color = list.Last();
                if (list.Count == 0 || list.Count == 1 && list[0].Color == Colors.Transparent || color.Color.A == 0)
                {
                    StopAnimate();
                    if (oldBrush != null)
                        SetControlBrush(oldBrush);
                    RestoreOldChildBrushes();
                    currentColor = Colors.Transparent;
                }
                else
                {
                    if (color.Color != currentColor || color.Color.A == 0 || color.BlinkTime != currentBlinkTime ||
                        currentText != color.Text)
                    {
                        var savecurrentText = currentText;
                        var savecontent = content;
                        RestoreContent();
                        currentText = savecurrentText;
                        content  = savecontent;

                        if (!String.IsNullOrEmpty(color.Text))
                        {
                            IDictionary<String, String> map = null;
                            if (Parent != null)
                            {
                                var stringeditorManager = Parent.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                                if (stringeditorManager != null)
                                {
                                    var cultures = stringeditorManager.GetListAvailableCultures(Parent);
                                    if (cultures != null && cultures.Count() > 0)
                                        map = stringeditorManager.GetListStringForCulture(Parent, stringeditorManager.GetActiveCulture(Parent));
                                }
                            }

                            var txt = color.Text;
                            if (map != null && map.ContainsKey(txt))
                                txt = map[txt];
                            currentText = color.Text;
                            if (Control is ContentControl)
                            {
                                var contentControl = Control as ContentControl;
                                if (contentControl.Content is String)
                                    contentControl.Content = txt;
                                else
                                {
                                    var currentList = (from c in contentControl.GetVisualChildrenOfType<ContentControl>()
                                                       where c.Content is String &&
#if !WINDOWS_UWP
                                                       c.Visibility == System.Windows.Visibility.Visible
#else
                                                       c.Visibility == Visibility.Visible
#endif
                                                       select c).ToList();
                                    if (mapOldContent == null)
                                    {
                                        mapOldContent = new Dictionary<ContentControl, String>();
                                        currentList.ForEach(c =>
                                        {
                                            mapOldContent.Add(c, c.Content as String);
                                        });
                                    }
                                    currentList.ForEach(c =>
                                        {
                                            c.Content = txt;
                                        });
                                }
                                contentControl.GetVisualChildrenOfType<Border>().ToList().ForEach(border =>
                                {
                                    border.BorderThickness = contentControl.BorderThickness;
                                });
                            }
                            else if (Control is TextBox)
                                (Control as TextBox).Text = txt;
                            else if (Control is TextBlock)
                                (Control as TextBlock).Text = txt;
                        }

                        currentColor = color.Color;
                        currentBlinkColor = color.BlinkColor;
                        currentBlinkTime = color.BlinkTime;
                        if (oldBrush is GradientBrush)
                        {
#if !WINDOWS_UWP
                            var gradientBrush = (oldBrush as GradientBrush).Clone();
#else
                            var gradientBrush = (oldBrush as GradientBrush);
#endif
                            StopAnimate();
                            CommonControls.CommonProperties.SetIsBrushAnimating((Control as ContentControl)?.Content as UIElement ?? Control, true);
                            SetControlBrush(gradientBrush);
                            if (color.BlinkTime != 0)
                                gradientBrush.GradientBrushColorAnimation(color.BlinkColor, color.Color, color.BlinkTime, true, true, AnimationEquation);
                            else
                                gradientBrush.GradientBrushColorAnimation(currentColor, color.Color, AnimationTime, Repeatable, Autoreverse, AnimationEquation);

                            if (listClonedBrush == null)
                                listClonedBrush = new List<Brush>();
                            listClonedBrush.Add(gradientBrush);
                        }
                        else
                        {
                            var brush = new SolidColorBrush(currentColor);
                            StopAnimate();
                            CommonControls.CommonProperties.SetIsBrushAnimating((Control as ContentControl)?.Content as UIElement ?? Control, true);
                            SetControlBrush(brush);
                            if (color.BlinkTime != 0)
                                brush.SolidBrushColorAnimation(color.BlinkColor, color.Color, color.BlinkTime, true, true, AnimationEquation);
                            else
                                brush.SolidBrushColorAnimation(currentColor, color.Color, AnimationTime, Repeatable, Autoreverse, AnimationEquation);

                            if (listClonedBrush == null)
                                listClonedBrush = new List<Brush>();
                            listClonedBrush.Add(brush);
                        }
                    }
                }
            }
            catch
            {
                StopAnimate();
                SetControlBrush(oldBrush);
                RestoreOldChildBrushes();
            }
        }

#if !WINDOWS_UWP
        void ScanChildContentControls(UIElement control)
        {
            var contentControls = control.GetVisualChildrenOfType<ContentControl>();
            foreach (var contentControl in contentControls)
            {
                if (contentControl is UserControl ||
                    (contentControl.Content is UIElement && (contentControl.Content as UIElement).IsVisible))
                    continue;

                var action = new Action(() =>
                {
                    var content = contentControl.Content as UIElement;
                    if (content != null)
                    {
                        ScanChildContentControls(content);

                        if (content.IsVisible)
                        {
                            SaveOldChildBrushes(content);
                            Reexecute();
                        }
                        else
                        {
                            var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsVisibleProperty, typeof(UIElement));
                            var notifierVisibility = new WPFUtilities.PropertyChangeNotifier(content, propDesc.Name);
                            notifierVisibility.ValueChanged += (o, e) =>
                            {
                                if (content.IsVisible)
                                {
                                    notifierVisibility.Dispose();
                                    listPropertyChangeNotifier.Remove(notifierVisibility);
                                    content.Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                                    {
                                        if (!IsInitialized())
                                            return;

                                        SaveOldChildBrushes(content);
                                        Reexecute();
                                    });
                                }
                            };
                            if (listPropertyChangeNotifier == null)
                                listPropertyChangeNotifier = new List<WPFUtilities.PropertyChangeNotifier>();
                            listPropertyChangeNotifier.Add(notifierVisibility);
                        }
                    }
                });

                if (contentControl.Content != null)
                    action();
                else
                {
                    var propDesc = DependencyPropertyDescriptor.FromProperty(ContentControl.HasContentProperty, typeof(ContentControl));
                    var notifierContent = new WPFUtilities.PropertyChangeNotifier(contentControl, propDesc.Name);
                    notifierContent.ValueChanged += (o, e) =>
                    {
                        if (contentControl.Content != null)
                        {
                            notifierContent.Dispose();
                            listPropertyChangeNotifier.Remove(notifierContent);
                            action();
                        }
                    };
                    if (listPropertyChangeNotifier == null)
                        listPropertyChangeNotifier = new List<WPFUtilities.PropertyChangeNotifier>();
                    listPropertyChangeNotifier.Add(notifierContent);
                }
            }
        }
#endif

        void SaveOldChildBrushes(UIElement control)
        {
            var fecontrol = control as FrameworkElement;
            if (fecontrol != null && !fecontrol.IsLoaded)
            {
                if (listPendingLoaded == null)
                    listPendingLoaded = new List<FrameworkElement>();
                if (!listPendingLoaded.Contains(fecontrol))
                {
                    listPendingLoaded.Add(fecontrol);
                    fecontrol.Loaded += control_Loaded;
                }
            }
            else
            {
                var childs = control.GetVisualChildrenOfType<UIElement>();

                (from c in childs.OfType<Panel>()
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     if (!mapOldBrush.ContainsKey(child))
                         mapOldBrush.Add(child, child.Background);
                 });
                (from c in childs.OfType<Control>()
                 where (c.Tag as String) == tagBackground || ApplyToAllChild
                 select c).ToList().ForEach(child =>
                 {
                     if (!mapOldBrush.ContainsKey(child))
                         mapOldBrush.Add(child, child.Background);
                 });
#if !WINDOWS_UWP
                (from c in childs.OfType<System.Windows.Shapes.Shape>()
#else
                (from c in childs.OfType<Shape>()
#endif
                where (c.Tag as String) == tagBackground || ApplyToAllChild
                select c).ToList().ForEach(child =>
                {
                    if (!mapOldBrush.ContainsKey(child))
                        mapOldBrush.Add(child, child.Fill);
                });
                (from c in childs.OfType<Border>()
                where (c.Tag as String) == tagBackground || ApplyToAllChild
                select c).ToList().ForEach(child =>
                {
                    if (!mapOldBrush.ContainsKey(child))
                        mapOldBrush.Add(child, child.Background);
                });
            }
        }

        void RestoreOldChildBrushes()
        {
            if (mapOldBrush != null)
                mapOldBrush.Keys.ToList().ForEach(control =>
                    {
                        if (control is Panel)
                            (control as Panel).Background = mapOldBrush[control];
                        else if (control is Control)
                            (control as Control).Background = mapOldBrush[control];
                        else if (control is Shape)
                            (control as Shape).Fill = mapOldBrush[control];
                        else if (control is Border)
                            (control as Border).Background = mapOldBrush[control];
                    });

            if (mapOldContent != null)
                mapOldContent.Keys.ToList().ForEach(c => c.Content = mapOldContent[c]);
        }

        void StopAnimate()
        {
            if (Control != null)
                CommonControls.CommonProperties.SetIsBrushAnimating((Control as ContentControl)?.Content as UIElement ?? Control, false);
            if (listClonedBrush != null)
            {
                listClonedBrush.ForEach((brush) =>
                {
                    if (brush is GradientBrush)
                        (brush as GradientBrush).GradientBrushColorAnimation(Colors.Transparent, Colors.Transparent, -1, false, false, null);
                    else if (brush is SolidColorBrush)
                        (brush as SolidColorBrush).SolidBrushColorAnimation(Colors.Transparent, Colors.Transparent, -1, false, false, null);
                });
                listClonedBrush.Clear();
            }
        }

        void RemoveAllPendingOperations()
        {
            if (listPendingLoaded != null)
            {
                foreach (var fe in listPendingLoaded)
                    fe.Loaded -= control_Loaded;
                listPendingLoaded.Clear();
            }

#if !WINDOWS_UWP
            if (listPropertyChangeNotifier != null)
            {
                listPropertyChangeNotifier.ForEach(c => c.Dispose());
                listPropertyChangeNotifier.Clear();
                listPropertyChangeNotifier = null;
            }
#endif
        }

        void control_Loaded(object sender, RoutedEventArgs e)
        {
            var fecontrol = sender as FrameworkElement;
            if (listPendingLoaded.Contains(fecontrol))
            {
                fecontrol.Loaded -= control_Loaded;
                listPendingLoaded.Remove(fecontrol);
                fecontrol.ApplyTemplate();
                DependencyObjectExtensions.CleanChildrenOfTypeCache(fecontrol);
                SaveOldChildBrushes(fecontrol);
            }
        }
#endif
#endregion

#region Overrides
#if !NET_STANDARD
        public override void ExecuteChangeLanguage(bool bPreparing = false)
        {
            Enable(!bPreparing);
        }

        protected override void Reexecute()
        {
            if (bSaved)
            {
                StopAnimate();
                SetControlBrush(oldBrush);
                RestoreOldChildBrushes();
                currentColor = Colors.Transparent;
            }

            base.Reexecute();
        }

        public override void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            base.Init(entity, parent, sessionname);
            bSaved = true;

            oldBrush = GetControlBrush();
            if (Control is ContentControl)
                content = (Control as ContentControl).Content;
            else if (Control is TextBox)
                content = (Control as TextBox).Text;
            else if (Control is TextBlock)
                content = (Control as TextBlock).Text;

            if (Control != null && !Control.IsVisible)
            {
                var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsVisibleProperty, typeof(UIElement));
                var notifierVisibility = new WPFUtilities.PropertyChangeNotifier(Control, propDesc.Name);
                notifierVisibility.ValueChanged += (o, e) =>
                {
                    if (Control.IsVisible)
                    {
                        notifierVisibility.Dispose();
                        listPropertyChangeNotifier.Remove(notifierVisibility);
                        
                        if (Control is FrameworkElement)
                            (Control as FrameworkElement).ApplyTemplate();
                        Control.UpdateLayout();

                        DependencyObjectExtensions.CleanChildrenOfTypeCache(Control);
                        SaveOldChildBrushes(Control);
                        Reexecute();
                    }
                };
                if (listPropertyChangeNotifier == null)
                    listPropertyChangeNotifier = new List<WPFUtilities.PropertyChangeNotifier>();
                listPropertyChangeNotifier.Add(notifierVisibility);
            }
        }

        public override void Enable(bool bEnable)
        {
            if (bEnable)
                currentColor = Colors.Transparent;
            base.Enable(bEnable);
        }

#if !WINDOWS_UWP
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "AnimationTime" || propertyName == "Repeatable" || 
                    propertyName == "Autoreverse" || propertyName == "AnimationBehavior" ||
                    propertyName == "TagMinValue" || propertyName == "TagMaxValue")
                {
                    return false;
                }

                return base[propertyName];
            }
        }
        public override void Demo()
        {
            // if (oldBrush == null)
            if (!bSaved)
            {
                bSaved = true;
                oldBrush = GetControlBrush();
                if (Control is ContentControl)
                    content = (Control as ContentControl).Content;
                else if (Control is TextBox)
                    content = (Control as TextBox).Text;
                else if (Control is TextBlock)
                    content = (Control as TextBlock).Text;
            }
            CreateBrush(CommonTarget);
            base.Demo();
        }
#endif
        public override void Execute()
        {
            CreateBrush(dTargetValue);
        }

        public override void Stop()
        {
            if (bSaved)
            {
                RemoveAllPendingOperations();
                StopAnimate();
                RestoreContent();
                SetControlBrush(oldBrush, true);
                RestoreOldChildBrushes();
                content = null;
                currentColor = Colors.Transparent;
            }
            base.Stop();
        }

        public override void Terminate()
        {
            base.Terminate();
            content = null;
            RestoreContent();
        }

#if !WINDOWS_UWP
        [Browsable(false)]
        public override UserControl Editor
        {
            get
            {
                return new UserControls.ColorAnimationPropertyEditor();
            }
        }
#endif

#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.BackColorName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return 1;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override bool Is3D
        {
            get
            {
                return true;
            }
        }
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override bool Is2D
        {
            get
            {
                return true;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
        public override String AnimationSummary
        {
            get
            {
                return base.AnimationSummary;
            }
        }
#endif
        
        #endregion
    }
}
