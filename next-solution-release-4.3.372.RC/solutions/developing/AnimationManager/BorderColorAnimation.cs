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
#if !WINDOWS_UWP
using System.Windows.Media;
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
#endif
using System.ComponentModel;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using StringManager.ComponentService;

namespace AnimationManager
{
#if !NET_STANDARD
    class TwoPairsColor
    {
        public Brush Brush1;
        public Brush Brush2;
    }
#endif

    [DataContract(Name = "BorderColorAnimation")]
    public class BorderColorAnimation : AnimationManager
    {
        #region Members
#if !NET_STANDARD
        bool bSaved;
        TwoPairsColor oldBrush;
        Dictionary<UIElement, TwoPairsColor> mapOldBrush;
        Dictionary<ContentControl, String> mapOldContent;
        List<FrameworkElement> listPendingLoaded;
        List<Brush> listClonedBrush;
        Color currentColor;
        int currentBlinkTime;
        String currentText;
        Object content;
        bool useTextAnimation;
#if !WINDOWS_UWP
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
        void SetControlBrush(TwoPairsColor brush, bool bStopping = false)
        {
            if (brush == null)
                return;
            Control _control = Control as Control;
            if (Control is TextBox)
            {
                (Control as TextBox).BorderBrush = brush.Brush1;
                (Control as TextBox).Foreground = brush.Brush2;
            }
            else if (Control is TextBlock)
            {
                (Control as TextBlock).Foreground = brush.Brush1;
            }
            else if (Control is Shape)
                (Control as Shape).Stroke = brush.Brush1;
            else if (Control is Border)
                (Control as Border).BorderBrush = brush.Brush1;
            else if (_control != null)
            {
                _control.BorderBrush = brush.Brush1;
                _control.Foreground = brush.Brush2;

                if(Control is ContentControl)
                {
                    var contentControl = Control as ContentControl;
                    if (contentControl.Content is Control)
                    {
                        (contentControl.Content as Control).BorderBrush = brush.Brush1;
                        (contentControl.Content as Control).Foreground = brush.Brush2;
                    }
                    else if (contentControl.Content is Shape)
                        (contentControl.Content as Shape).Stroke = brush.Brush1;

                    var currentList = (from c in contentControl.GetVisualChildrenOfType<ContentControl>()
#if !WINDOWS_UWP
                                       where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
#else
                                   where c.Content is String && c.Visibility == Visibility.Visible
#endif
                                       select c).ToList();
                    currentList.ForEach(c =>
                    {
                        if (c.Content is Control)
                        {
                            (c.Content as Control).BorderBrush = brush.Brush1;
                            (c.Content as Control).Foreground = brush.Brush2;
                        }
                        else if (c.Content is Shape)
                            (c.Content as Shape).Stroke = brush.Brush1;
                    });

                }
            }

            if (Control != null)
            {
                if (mapOldBrush == null)
                    mapOldBrush = new Dictionary<UIElement, TwoPairsColor>();
                else if (!bStopping)
                    mapOldBrush.Clear();

                if (!bStopping)
                {
                    (from c in Control.GetVisualChildrenOfType<Control>()
                     where (c.Tag as String) == tagBackground || ApplyToAllChild
                     select c).ToList().ForEach(child =>
                     {
                         if (!mapOldBrush.ContainsKey(child))
                             mapOldBrush.Add(child, new TwoPairsColor() { Brush1 = child.BorderBrush, Brush2 = child.Foreground });
                     });
#if !WINDOWS_UWP
                    (from c in Control.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
#else
                (from c in Control.GetVisualChildrenOfType<Shape>()
#endif
                     where (c.Tag as String) == tagBackground || ApplyToAllChild
                     select c).ToList().ForEach(child =>
                     {
                         if (!mapOldBrush.ContainsKey(child))
                             mapOldBrush.Add(child, new TwoPairsColor() { Brush1 = child.Stroke });
                     });
                }

                if (mapOldBrush != null)
                {
                    mapOldBrush.Keys.ToList().ForEach(control =>
                    {
                        if (control is Control)
                        {
                            (control as Control).BorderBrush = brush.Brush1;
                            (control as Control).Foreground = brush.Brush2;
                        }
                        else if (control is Shape)
                            (control as Shape).Stroke = brush.Brush1;
                        else if (control is Border)
                            (control as Border).BorderBrush = brush.Brush1;
                    });
                }
            }
        }

        TwoPairsColor GetControlBrush()
        {
            if (mapOldBrush == null)
            {
                mapOldBrush = new Dictionary<UIElement, TwoPairsColor>();

                if (Control != null)
                {
#if !WINDOWS_UWP
                    if (ApplyToAllChild)
                        ScanChildContentControls(Control);
#endif
                    SaveOldChildBrushes(Control);
                }

                if (Control is TextBox)
                    content = (Control as TextBox).Text;
                else if (Control is TextBlock)
                    content = (Control as TextBlock).Text;
                else if (Control is ContentControl)
                    content = (Control as ContentControl).Content;

                if (Control is Control)
                    return new TwoPairsColor() { Brush1 = (Control as Control).BorderBrush, Brush2 = (Control as Control).Foreground };
                else if (Control is Shape)
                    return new TwoPairsColor() { Brush1 = (Control as Shape).Stroke };
                else if (Control is Border)
                    return new TwoPairsColor() { Brush1 = (Control as Border).BorderBrush};
            }

            return null;
        }

        void RestoreContent()
        {
            if (!useTextAnimation)
                return;

            if (content != null)
            {
                if (Control is TextBox)
                    (Control as TextBox).Text = content as String;
                else if (Control is TextBlock)
                    (Control as TextBlock).Text = content as String;
                else if (Control is ContentControl)
                    (Control as ContentControl).Content = content;
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
                if (list.Count == 0 || list.Count == 1 && list[0].Color == Colors.Transparent)
                {
                    var savecurrentText = currentText;
                    var savecontent = content;
                    RestoreContent();
                    currentText = savecurrentText;
                    content = savecontent;

                    StopAnimate();
                    if (oldBrush != null)
                        SetControlBrush(oldBrush);
                    RestoreOldChildBrushes();
                    currentColor = Colors.Transparent;
                }
                else
                {
                    if (list.Count > 0)
                        color = list.Last();
                    if (color.Color != currentColor || color.BlinkTime != currentBlinkTime || 
                        currentText != color.Text)
                    {
                        var savecurrentText = currentText;
                        var savecontent = content;
                        RestoreContent();
                        currentText = savecurrentText;
                        content = savecontent;

                        if (useTextAnimation && !String.IsNullOrEmpty(color.Text))
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
                            if (Control is TextBox)
                                (Control as TextBox).Text = txt;
                            else if (Control is TextBlock)
                                (Control as TextBlock).Text = txt;
                            else if (Control is ContentControl)
                            {
                                var contentControl = Control as ContentControl;
                                if (contentControl.Content is String)
                                    contentControl.Content = txt;
                                else
                                {
                                    var currentList = (from c in contentControl.GetVisualChildrenOfType<ContentControl>()
#if !WINDOWS_UWP
                                                       where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
#else
                                                       where c.Content is String && c.Visibility == Visibility.Visible
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
                            else if(Control is Control)
                            {
                                var control = Control as Control;
                                control.GetVisualChildrenOfType<Border>().ToList().ForEach(border =>
                                {
                                    border.BorderThickness = control.BorderThickness;
                                });
                            }
                        }

                        currentColor = color.Color;
                        currentBlinkTime = color.BlinkTime;
                        var brush = new SolidColorBrush(currentColor);
                        StopAnimate();
                        CommonControls.CommonProperties.SetIsBrushAnimating((Control as ContentControl)?.Content as UIElement ?? Control, true);
                        SetControlBrush(new TwoPairsColor() { Brush1 = brush, Brush2 = brush });
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
                var contentControl = control as ContentControl;
                if (contentControl != null)
                    (contentControl.Content as FrameworkElement)?.ApplyTemplate();
                DependencyObjectExtensions.CleanChildrenOfTypeCache(control);

                (from c in control.GetChildrenOfType<Control>()
                where (c.Tag as String) == tagBackground || ApplyToAllChild
                select c).ToList().ForEach(child =>
                {
                    if (!mapOldBrush.ContainsKey(child))
                        mapOldBrush.Add(child, new TwoPairsColor() { Brush1 = child.BorderBrush, Brush2 = child.Foreground });
                });
#if !WINDOWS_UWP
                (from c in control.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
#else
                (from c in control.GetVisualChildrenOfType<Shape>()
#endif
                where (c.Tag as String) == tagBackground || ApplyToAllChild
                select c).ToList().ForEach(child =>
                {
                    if (!mapOldBrush.ContainsKey(child))
                        mapOldBrush.Add(child, new TwoPairsColor() { Brush1 = child.Stroke });
                });
            }
        }

        void RestoreOldChildBrushes()
        {
            if (mapOldBrush != null)
                mapOldBrush.Keys.ToList().ForEach(control =>
                {
                    if (control is Control)
                    {
                        (control as Control).BorderBrush = mapOldBrush[control].Brush1;
                        (control as Control).Foreground = mapOldBrush[control].Brush2;
                    }
                    else if (control is Shape)
                        (control as Shape).Stroke = mapOldBrush[control].Brush1;
                    else if (control is Border)
                        (control as Border).BorderBrush = mapOldBrush[control].Brush1;
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

            useTextAnimation = (from c in ListColors where !String.IsNullOrEmpty(c.Text) select c).ToList().Count != 0;

            oldBrush = GetControlBrush();
            if (Control is TextBox)
                content = (Control as TextBox).Text;
            else if (Control is TextBlock)
                content = (Control as TextBlock).Text;
            else if (Control is ContentControl)
                content = (Control as ContentControl).Content;

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
                if (Control is TextBox)
                    content = (Control as TextBox).Text;
                else if (Control is TextBlock)
                    content = (Control as TextBlock).Text;
                else if (Control is ContentControl)
                    content = (Control as ContentControl).Content;
            }
            CreateBrush(CommonTarget);
            base.Demo();
        }
#endif
        public override void Terminate()
        {
            base.Terminate();
            content = null;
            RestoreContent();
        }

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
            if (bDemoMode)
            {
                bSaved = false;
                mapOldBrush = null;
            }
            base.Stop();
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
                return Properties.Resources.BorderColorName;
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
