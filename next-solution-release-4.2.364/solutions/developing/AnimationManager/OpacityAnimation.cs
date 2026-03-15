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

namespace AnimationManager
{
    [DataContract(Name = "OpacityAnimation")]
    public class OpacityAnimation : AnimationManager
    {
#region Members
#if !WINDOWS_UWP && !NET_STANDARD
        Dictionary<Brush, double> mapBrushOpacity;
        List<GeometryModel3D> geometries;

        /*
        PropertyChangeNotifier notifierX;
        PropertyChangeNotifier notifierY;

        Viewport3D viewport3D;
        Model3DGroup father;
        int index;
        */
#endif
#endregion

#region Methods
#if !WINDOWS_UWP && !NET_STANDARD
        public void SetOpacity(Model3D model, double opacity)
        {
            if (opacity < 0)
            {
                if (mapBrushOpacity != null)
                {
                    foreach (var brush in mapBrushOpacity.Keys)
                    {
                        brush.Opacity = mapBrushOpacity[brush];
                    }
                    mapBrushOpacity.Clear();
                }

                /*
                if (opacity == 0 && father != null)
                {
                    if (father.Children.Contains(Control3D))
                        father.Children.Remove(Control3D);
                }
                */
                return;
            }

            /*
            if (father != null)
            {
                // SetOpacity(Control3D, 1);
                if (!father.Children.Contains(Control3D))
                    father.Children.Insert(index, Control3D);
            }
            */
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

        /*
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
            if (t.ScaleX < Opacity && t.ScaleY < Opacity)
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
        */
#endif
#endregion

#region Properties

        double opacity = 0.5;
        [DataMember]
        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (value == opacity)
                    return;
                opacity = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("Opacity");
#endif
                Reexecute();
#endif
            }
        }

        /*
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
#if !WINDOWS_UWP
                OnPropertyChanged("VisibleOn3DCameraZoom");
#endif
                Reexecute();
            }
        }
        */
        #endregion

        #region Overrides
#if !NET_STANDARD
#if !WINDOWS_UWP
        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "AnimationBehavior")
            {
                if (AnimationBehavior == AnimationBehavior.Trigger)
                    return Properties.Resources.InvalidAnimationBehaviour;
            }
            if (propertyName == "Opacity")
            {
                if (Opacity < 0 || Opacity > 1)
                    return Properties.Resources.OpacityOutOfRange;
            }
            

            return base.PerformValidation(propertyName);
        }
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "AnimationEquation" ||
                    propertyName == "Autoreverse" || propertyName == "Repeatable")
                {
                    return false;
                }
                if (propertyName == "Opacity" ||
                    propertyName == "TagMinValue" || propertyName == "TagMaxValue")
                {
                    return AnimationBehavior == AnimationBehavior.Proportional;
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
            {
                if (!oldOpacity.HasValue)
                    oldOpacity = uie.Opacity;
                uie.Fade(0, 0, null, null, bSetVisibility: false);
            }
        }

        public override void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            if (!oldOpacity.HasValue && Control != null)
                oldOpacity = Control.Opacity;

            if (Control3D != null)
                SetOpacity(Control3D, 1);
            else if (Control != null)
                Control.Fade(0, 0, AnimationEquation, null, bSetVisibility: false);

            base.Init(entity, parent, sessionname);

            /*
            if (Control != null && Control3D != null)
            {
                Control.Dispatcher.BeginInvokeAsynchronouslyInInput(() =>
                    {
                        var listViewport3Ds = Control.GetChildrenOfType<Viewport3D>().ToList();
                        if (listViewport3Ds.Count == 0 && Control is Viewport3D)
                            listViewport3Ds.Add(Control as Viewport3D);
                        if (listViewport3Ds.Count > 0 && listViewport3Ds[0].Camera.Transform is Transform3DGroup)
                            viewport3D = listViewport3Ds[0];

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
                        if (father != null && t != null && viewport3D != null && VisibleOn3DCameraZoom)
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
                    });
            }
            */
        }

        double? oldOpacity;
        public override void Demo()
        {
            if (!oldOpacity.HasValue && Control != null)
                oldOpacity = Control.Opacity;
            if (Control3D != null)
                SetOpacity(Control3D, CommonTarget);
            else if (Control != null)
                Control.Fade(CommonTarget, AnimationTime, AnimationEquation, null, false);
            base.Demo();
        }
        


#endif
        public override void Execute()
        {
            /*
            if (VisibleOn3DCameraZoom)
                return;
            */
#if !WINDOWS_UWP
            if (Control3D != null)
                SetOpacity(Control3D, dTargetValue);
            else
#endif
            {
                if (Control == null)
                    return;
                Control.Fade(dTargetValue, AnimationTime, AnimationEquation, null, false);
            }
        }

        public override void Stop()
        {
#if !WINDOWS_UWP
            /*
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
            */
            if (Control3D != null)
                SetOpacity(Control3D, -1);
            else
#endif
            {
                if (Control != null && oldOpacity.HasValue)
                    Control.Fade(oldOpacity.Value, AnimationTime, AnimationEquation, null, bSetVisibility: false);
                    // Control.Fade(0, -1, AnimationEquation, null, false);
            }
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.OpacityName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return Opacity;
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
