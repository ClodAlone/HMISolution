using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Utilities;
using OPCUAViewModel;

namespace AnimatedObjects
{
    public class AnimationItem : INotifyPropertyChanged
    {
        #region Private Members;
        private double threshold;
        private String text;
        private AnimationType textanimation;
        private double animationtime;
        private Brush background;
        private Brush foreground;
        private Uri backimage;
        private BackImageItemList backimagelist;
        private Stretch imagestretch;
        private String tagreferenceXml;
        private string tagname;
        private String nodeid;
        #endregion

        #region Constructors
        public AnimationItem()
        { }

        public AnimationItem(AnimationItem instance)
        {
            if (instance == null)
                return;

            Value = instance.Value;
            Text = instance.Text;
            Animation = instance.Animation;
            AnimationTime = instance.AnimationTime;
            Background = instance.Background;
            Foreground = instance.Foreground;
            BackImage = instance.BackImage;
            BackImageList = new BackImageItemList(instance.BackImageList);
            ImageStretch = instance.ImageStretch;
            TagName = instance.TagName;
            TagReferenceXml = instance.TagReferenceXml;
            NodeId = instance.NodeId;
        }
        #endregion

        #region Public Properties
        public String NodeId
        {
            get
            {
                if (string.IsNullOrEmpty(nodeid))
                    nodeid = Guid.NewGuid().ToString();
                return nodeid;
            }
            set
            {
                if (nodeid == value)
                    return;
                nodeid = value;
                OnPropertyChanged("NodeId");
            }
        }
        public double Value
        {
            get { return threshold; }
            set
            {
                if (threshold == value)
                    return;
                threshold = value;
                OnPropertyChanged("Value");
            }
        }
        public String Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;
                text = value;
                OnPropertyChanged("Text");
            }
        }
        public AnimationType Animation
        {
            get { return textanimation; }
            set
            {
                if (textanimation == value)
                    return;
                textanimation = value;
                OnPropertyChanged("Animation");
            }
        }
        public double AnimationTime
        {
            get { return animationtime; }
            set
            {
                if (animationtime == value)
                    return;
                animationtime = value;
                OnPropertyChanged("AnimationTime");
            }
        }
        public Brush Background
        {
            get { return background; }
            set
            {
                if (background == value)
                    return;
                background = value;
                OnPropertyChanged("Background");
            }
        }
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (foreground == value)
                    return;
                foreground = value;
                OnPropertyChanged("Foreground");
            }
        }
        public Uri BackImage
        {
            get { return backimage; }
            set
            {
                if (backimage == value)
                    return;
                backimage = value;
                OnPropertyChanged("BackImage");
            }
        }
        public BackImageItemList BackImageList
        {
            get
            {
                if (backimagelist == null)
                    backimagelist = new BackImageItemList();
                return backimagelist;
            }
            set
            {
                if (backimagelist == value)
                    return;
                backimagelist = value;
                OnPropertyChanged("BackImageList");
            }
        }
        public Stretch ImageStretch
        {
            get { return imagestretch; }
            set
            {
                if (imagestretch == value)
                    return;
                imagestretch = value;
                OnPropertyChanged("ImageStretch");
            }
        }
        public String TagName
        {
            get { return tagname; }
            set
            {
                if (tagname == value)
                    return;
                tagname = value;
                OnPropertyChanged("TagName");
            }
        }
        public String TagReferenceXml
        {
            get { return tagreferenceXml; }
            set
            {
                if (tagreferenceXml == value)
                    return;
                tagreferenceXml = value;
                OnPropertyChanged("TagReferenceXml");
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference TagReference
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(tagreferenceXml))
                        return tagreferenceXml.FromXml<OPCUAEntityReference>();
                }
                catch
                { }

                return null;
            }
            set
            {
                if (value != null)
                {
                    TagReferenceXml = value.ToXml();
                    OnPropertyChanged("TagReference");
                }
                else
                {
                    TagReferenceXml = string.Empty;
                    OnPropertyChanged("TagReference");
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion

        public Dictionary<string, object> ToDictionary(List<string> usedIDs)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(tagreferenceXml))
                res.Add("SVGReferenceId", CreateUniqueName(usedIDs));
            else
                res.Add("SVGReferenceId", null);
            res.Add("Value", Value);
            res.Add("Text", Text);
            res.Add("Animation", Animation);
            res.Add("AnimationTime", AnimationTime);
            res.Add("Background", Background);
            res.Add("Foreground", Foreground);
            res.Add("BackImage", BackImage?.GetPathString());
            res.Add("BackImageList", BackImageList?.ToDictionary());
            res.Add("ImageStretch", ImageStretch);
            return res;
        }

        internal String CreateUniqueName(List<String> list)
        {
            var name = $"{Properties.Resources.PropertyInspectorPrefix} {Properties.Resources.Threshold}";

            if (!list.Contains(String.Format("{0} ({1})", name, 0)))
            {
                list.Add(String.Format("{0} ({1})", name, 0));
                return String.Format("{0} ({1})", name, 0);
            }
            var newname = String.Format("{0} ({1})", name, 0);
            int i = 0;
            while (list.Contains(newname))
                newname = String.Format("{0} ({1})", name, ++i);

            list.Add(newname);
            return newname;
        }
    }

}