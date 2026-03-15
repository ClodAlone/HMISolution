using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Media.Media3D;

namespace UFInterfaces.Animatable
{
    public interface IAnimatable
    {
        String Name { get; }
        IEnumerable AnimationList { get; set; }
        UIElement Element { get; }
        Model3D Element3D { get; }
    }
}
