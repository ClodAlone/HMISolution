using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for _3DTransformSliders.xaml
    /// </summary>
    public partial class _3DTransformSliders : UserControl
    {
        Popup parentPopup;

        public _3DTransformSliders()
        {
            InitializeComponent();
        }

        public void ShowPopup()
        {
            if (parentPopup == null)
            {
                parentPopup = new Popup();
                Popup.CreateRootPopup(parentPopup, this);

                parentPopup.AllowsTransparency = true;
                parentPopup.Placement = PlacementMode.Right;
                parentPopup.PopupAnimation = PopupAnimation.Fade;
                parentPopup.StaysOpen = false;
                parentPopup.Focusable = false;
            }

            Viewport3D TargetElement = DataContext as Viewport3D;
            parentPopup.PlacementTarget = TargetElement;
            parentPopup.IsOpen = true;
        }

        private void slider_ValueChanged(Slider slider, Vector3D vector)
        {
            Viewport3D TargetElement = DataContext as Viewport3D;
            if (TargetElement != null)
            {
                foreach (Visual3D visual3D in TargetElement.Children)
                {
                    if (!(visual3D.Transform is Transform3DGroup))
                        visual3D.Transform = new Transform3DGroup();

                    Transform3DGroup Group = visual3D.Transform as Transform3DGroup;
                    var fxShadow = from fx in Group.Children
                                   where fx is RotateTransform3D
                                   select fx;
                    if (fxShadow.Count() == 0)
                    {
                        AxisAngleRotation3D transformX = new AxisAngleRotation3D(vector, slider.Value);
                        Group.Children.Add(new RotateTransform3D(transformX));
                    }
                    else
                    {
                        bool bFound = false;
                        foreach (RotateTransform3D t3D in fxShadow)
                        {
                            AxisAngleRotation3D a3D = t3D.Rotation as AxisAngleRotation3D;
                            if (a3D.Axis.X == vector.X && a3D.Axis.Y == vector.Y && a3D.Axis.Z == vector.Z)
                            {
                                bFound = true;
                                a3D.Angle = slider.Value;
                            }
                        }

                        if (!bFound)
                        {
                            AxisAngleRotation3D transformX = new AxisAngleRotation3D(vector, slider.Value);
                            Group.Children.Add(new RotateTransform3D(transformX));
                        }
                    }
                }
            }
        }
        private void sliderX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slider_ValueChanged(sliderX, new Vector3D(1, 0, 0));
        }

        private void sliderY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slider_ValueChanged(sliderY, new Vector3D(0, 1, 0));
        }

        private void sliderZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slider_ValueChanged(sliderZ, new Vector3D(0, 0, 1));
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            Viewport3D TargetElement = DataContext as Viewport3D;
            if (TargetElement != null)
            {
                foreach (Visual3D visual3D in TargetElement.Children)
                    visual3D.Transform = null;
            }
        }
    }
}
