using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UFInterfaces.Constants;

namespace ScreenManager
{
    [DataContract(Name = "ScreenLayoutData", Namespace = Namespaces.UriProgea)]
    [KnownType(typeof(Transform))]
    [KnownType(typeof(TransformGroup))]
    [KnownType(typeof(ScaleTransform))]
    [KnownType(typeof(RotateTransform))]
    [KnownType(typeof(MatrixTransform))]
    [KnownType(typeof(TranslateTransform))]
    [KnownType(typeof(SkewTransform))]
    public class ScreenLayoutData
    {
        [DataMember]
        public String Name;
        [DataMember]
        public double Top;
        [DataMember]
        public double Left;
        [DataMember]
        public double Height;
        [DataMember]
        public double Width;
        [DataMember]
        public bool IsMaximized;
        [DataMember]
        public bool IsMinimized;
        [DataMember]
        public Transform RenderTransform;
        [DataMember]
        public Point RenderTransformOrigin;
    }
}
