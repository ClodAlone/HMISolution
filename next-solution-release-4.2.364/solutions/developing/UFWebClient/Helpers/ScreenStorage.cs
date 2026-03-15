using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Collections.Generic;
using AnimationManager;

namespace UFWebClient.Helpers
{
    [DataContract(Name = "ScreenObject")]
    public class ScreenObject
    {
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public String Xaml { get; set; }
        [DataMember]
        public String StyleResource { get; set; }
        [DataMember]
        public String BrushResource { get; set; }
        [DataMember]
        public String PenResource { get; set; }
        [DataMember]
        public Rect Rect { get; set; }
    }

    [DataContract(Name = "ScreenEntity")]
    public class ScreenStorage
    {
        [DataMember]
        public Uri uri { get; set; }
        [DataMember]
        public String canvasXaml { get; set; }
        [DataMember]
        public List<ScreenObject> listObjects = new List<ScreenObject>();
        [DataMember]
        public List<String> listResources = new List<String>();
        [DataMember]
        public List<String> listCommandSources = new List<String>();
        [DataMember]
        public List<String> listDynamicSources = new List<String>();
        [DataMember]
        public Dictionary<String, AnimationManagerList> mapAnimations = new Dictionary<String, AnimationManagerList>();
        [DataMember]
        public int hash { get; set; }
    }
}
