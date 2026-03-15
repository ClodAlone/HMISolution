using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using Utilities.Animations;
using Utilities.WPF;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;
using Utilities.Animations;
#endif
using System.ComponentModel;
#endif
using Utilities;
using UFInterfaces;
using DocumentManager.ComponentService;
using WPFUtilities;
using System.ComponentModel.DataAnnotations;

namespace AnimationManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum CompareModes
    {
        [Display(Order = 1)]
        Equal,
        [Display(Order = 2)]
        Major,
        [Display(Order = 3)]
        Minor,
        [Display(Order = 4)]
        MajorEqual,
        [Display(Order = 5)]
        MinorEqual,
        [Display(Order = 0)]
        Different
    };

    [DataContract(Name = "VisibilityAnimation")]
    public class VisibilityAnimation : AnimationManager
    {
#region Members
#if !NET_STANDARD
#if !WINDOWS_UWP
        Dictionary<Brush, double> mapBrushOpacity;
        List<GeometryModel3D> geometries;

        PropertyChangeNotifier notifierX;
        PropertyChangeNotifier notifierY;

        Viewport3D viewport3D;
        Model3DGroup father;
        int index;
#endif
        bool bForceAnimation;
        // bool bCompletelyInizialized;
#endif
#endregion

#region Methods
#if !WINDOWS_UWP && !NET_STANDARD
        public void SetOpacity(Model3D model, double opacity)
        {
            if (opacity <= 0)
            {
                if (mapBrushOpacity != null)
                {
                    foreach (var brush in mapBrushOpacity.Keys)
                    {
                        brush.Opacity = mapBrushOpacity[brush];
                    }
                    mapBrushOpacity.Clear();
                }

                if (opacity == 0 && father != null)
                {
                    if (father.Children.Contains(Control3D))
                        father.Children.Remove(Control3D);
                }
                return;
            }


            if (father != null)
            {
                // SetOpacity(Control3D, 1);
                if (!father.Children.Contains(Control3D))
                    father.Children.Insert(index, Control3D);
            }

            if (geometries == null)
            {
                mapBrushOpacity = new Dictionary<Brush, double>();
                geometries = new List<GeometryModel3D>();
                if (Control3D is GeometryModel3D)
                    geometries.Add(Control3D as GeometryModel3D);
                else if (Control3D is Model3DGroup)
                    geometries = DependencyObjectExtensions.GetAllGeometries(Control3D as Model3DGroup);
            }

            geometries.ForEach(geometry => 
                {
                    SetOpacity(geometry.Material, opacity);
                    SetOpacity(geometry.BackMaterial, opacity);
                });
        }

        public void SetOpacity(Material material, double opacity)
        {
            var group = material as MaterialGroup;
            if (group != null)
            {
                foreach (var mat in group.Children)
                {
                    SetOpacity(mat, opacity);
                }
                return;
            }

            Brush brush = null;
            var diffuse = material as DiffuseMaterial;
            var emissive = material as EmissiveMaterial;
            var specular = material as SpecularMaterial;
            if (diffuse != null)
                brush = diffuse.Brush;
            if (emissive != null)
                brush = emissive.Brush;
            if (specular != null)
                brush = specular.Brush;

            if (brush == null)
                return;

            if (!mapBrushOpacity.ContainsKey(brush))
                mapBrushOpacity.Add(brush, brush.Opacity);
            brush.Opacity = opacity;
        }

        ScaleTransform3D GetCameraTransform()
        {
            if (Control == null || viewport3D == null)
                return null;

            var listViewport3Ds = Control.GetChildrenOfType<Viewport3D>().ToList();
            if (listViewport3Ds.Count == 0 && Control is Viewport3D)
                listViewport3Ds.Add(Control as Viewport3D);
            if (listViewport3Ds.Count > 0 && listViewport3Ds[0].Camera.Transform is Transform3DGroup)
            {
                viewport3D = listViewport3Ds[0];

                var tg = viewport3D.Camera.Transform as Transform3DGroup;
                var list = (from c in tg.Children where c is ScaleTransform3D select c as ScaleTransform3D).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            return null;
        }

        void SetVisibilityOnCameraZoom()
        {
            var t = GetCameraTransform();
            if (t == null || father == null)
                return;
            if (t.ScaleX < CompareValue && t.ScaleY < CompareValue)
            {
                // SetOpacity(Control3D, 1);
                if (!father.Children.Contains(Control3D))
                    father.Children.Insert(index, Control3D);
            }
            else
            {
                // SetOpacity(Control3D, 0);
                if (father.Children.Contains(Control3D))
                    father.Children.Remove(Control3D);
            }
        }
#endif
#endregion

#region Properties

        double compareValue = 0;
        [DataMember]
        public double CompareValue
        {
            get { return compareValue; }
            set
            {
                if (compareValue == value)
                    return;
                compareValue = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("CompareValue");
#endif
                Reexecute();
#endif
            }
        }

        CompareModes compareMode;
        [DataMember]
        public CompareModes CompareMode
        {
            get { return compareMode; }
            set
            {
                if (compareMode == value)
                    return;
                compareMode = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("CompareMode");
#endif
                Reexecute();
#endif
            }
        }

        bool visibleOn3DCameraZoom = false;
        [DataMember]
        public bool VisibleOn3DCameraZoom
        {
            get { return visibleOn3DCameraZoom; }
            set
            {
                if (value == visibleOn3DCameraZoom)
                    return;
                visibleOn3DCameraZoom = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("VisibleOn3DCameraZoom");
#endif
                Reexecute();
#endif
            }
        }

        #endregion

        #region Overrides
#if !NET_STANDARD
#if !WINDOWS_UWP
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "AnimationTime" || propertyName == "Repeatable" || 
                    propertyName == "Autoreverse" || propertyName == "AnimationBehavior" || 
                    propertyName == "AnimationEquation" || propertyName == "TagMinValue" ||
                    propertyName == "TagMaxValue")
                {
                    return false;
                }

                return base[propertyName];
            }
        }
        public override void PreInit(IEntityReference entity)
        {
            if (IsInitialized())
                return;

            var uie = entity.ContainedObject as UIElement;
            if (uie != null)
                uie.Visibility = Visibility.Collapsed;

            bForceAnimation = (Control as FrameworkElement)?.Parent is StackPanel || (Control as FrameworkElement)?.Parent is WrapPanel;
        }

        public override void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            if (Control3D != null)
                SetOpacity(Control3D, 0);
            else if (Control != null)
            {
                //Control.Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                //{
                    //if (Control != null)
                    //    Control.Fade(0, 0, AnimationEquation, null);
                //    bCompletelyInizialized = true;
                //});
            }

            base.Init(entity, parent, sessionname);            

            if (Control != null && Control3D != null)
            {
                Control.Dispatcher.BeginInvokeAsynchronouslyInInput(() =>
                    {
                        if (Control != null && Control3D != null)
                        {
	                        var listViewport3Ds = Control.GetChildrenOfType<Viewport3D>().ToList();
	                        if (listViewport3Ds.Count == 0 && Control is Viewport3D)
	                            listViewport3Ds.Add(Control as Viewport3D);
	                        if (listViewport3Ds.Count > 0 && listViewport3Ds[0].Camera.Transform is Transform3DGroup)
	                            viewport3D = listViewport3Ds[0];

                            if (viewport3D != null)
                            {
                                var list = (from c in viewport3D.Children.OfType<ModelVisual3D>() select c);
                                foreach (var modelVisual3D in list)
                                {
                                    var f = Utilities.WPF.DependencyObjectExtensions.FindParentGroup(modelVisual3D, Control3D);
                                    if (f != null)
                                    {
                                        father = f;
                                        index = father.Children.IndexOf(Control3D);
                                        break;
                                    }
                                }

                                var t = GetCameraTransform();
                                if (father != null && t != null && VisibleOn3DCameraZoom)
                                {
                                    SetVisibilityOnCameraZoom();

                                    var propDesc = DependencyPropertyDescriptor.FromProperty(ScaleTransform3D.ScaleXProperty, typeof(ScaleTransform3D));
                                    if (notifierX == null)
                                    {
                                        notifierX = new PropertyChangeNotifier(t, propDesc.Name);
                                        notifierX.ValueChanged += (o, e) =>
                                        {
                                            SetVisibilityOnCameraZoom();
                                        };
                                    }

                                    propDesc = DependencyPropertyDescriptor.FromProperty(ScaleTransform3D.ScaleYProperty, typeof(ScaleTransform3D));
                                    if (notifierY == null)
                                    {
                                        notifierY = new PropertyChangeNotifier(t, propDesc.Name);
                                        notifierY.ValueChanged += (o, e) =>
                                        {
                                            SetVisibilityOnCameraZoom();
                                        };
                                    }
                                }

	                            SetOpacity(Control3D, 0);
	                            Execute();
                            }
                        }

                        SetOpacity(Control3D, 0);
                        Execute();
                    });
            }
        }

        public override void Terminate()
        {
            base.Terminate();
            lastResult = null;
        }

        public override void Demo()
        {
            //if (Control3D != null)
            //    SetOpacity(Control3D, CommonTarget);
            //else
            //    Control.Fade(CommonTarget, AnimationTime, AnimationEquation, null);
            //base.Demo();
        }
#endif
        bool? lastResult;
        internal override bool ExecuteWithValue(double dValue)
        {
            bool bResult = false;
            switch(CompareMode)
            {
                case CompareModes.Different: bResult = dValue != CompareValue; break;
                case CompareModes.Equal: bResult = dValue == CompareValue; break;
                case CompareModes.Major: bResult = dValue > CompareValue; break;
                case CompareModes.MajorEqual: bResult = dValue >= CompareValue; break;
                case CompareModes.Minor: bResult = dValue < CompareValue; break;
                case CompareModes.MinorEqual: bResult = dValue <= CompareValue; break;
            }

            if (bDoNotCheckCompare || !lastResult.HasValue || bResult != lastResult || bForceAnimation)
            {
                bForceAnimation = false;
                lastResult = bResult;

                if (VisibleOn3DCameraZoom)
                    return true;

#if !WINDOWS_UWP
                if (Control3D != null)
                    SetOpacity(Control3D, bResult ? 1 : 0);
                else
#endif
                {
                    if (Control == null)
                        return true;

                    //if (bCompletelyInizialized)
                        Control.Fade(bResult ? 1 : 0, AnimationTime, AnimationEquation, null);
                    //else
                    //{
                    //    Control.Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                    //    {
                    //        if (Control != null)
                    //            Control.Fade(bResult ? 1 : 0, AnimationTime, AnimationEquation, null);
                    //    });
                    //}

                    if (bResult)
                    {
                        var fe = Control as ContentControl;
                        if (fe != null)
                        {
                            var content = fe.Content as FrameworkElement;
                            if (content != null)
                                content.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            return true;
        }

        bool bDoNotCheckCompare;
        public override void Execute()
        {
            bDoNotCheckCompare = true;
            try
            {
                InternalExecute(LastData, false);
            }
            finally
            {
                bDoNotCheckCompare = false;
            }
        }

        public override void Enable(bool bEnable)
        {
            base.Enable(bEnable);

            if (bEnable)
                Execute();
        }

        public override void Stop()
        {
#if !WINDOWS_UWP
            if (notifierX != null)
            {
                notifierX.Dispose();
                notifierX = null;
            }
            if (notifierY != null)
            {
                notifierY.Dispose();
                notifierY = null;
            }

            if (Control3D != null)
                SetOpacity(Control3D, -1);
            else
#endif
            {
                if (Control != null)
                    Control.Fade(0, -1, AnimationEquation, null);
            }
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.VisibilityName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return CompareValue;
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
#endif
        public override bool IsFreezable
        {
            get
            {
                return false;
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
