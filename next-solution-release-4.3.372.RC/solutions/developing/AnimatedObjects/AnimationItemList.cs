using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Utilities;
using DocumentManager.ComponentService;

namespace AnimatedObjects
{
    public class AnimationItemList : ObservableCollection<AnimationItem>
    {
        #region Constructors
        public AnimationItemList() 
        { }
        
        public AnimationItemList(List<AnimationItem> instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new AnimationItem(item));
        }

        public AnimationItemList(AnimationItemList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new AnimationItem(item));
        }
        #endregion

        public List<object> ToDictionary()
        {
            List<string> list = new List<string>();
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary(list)));
            return res;
        }
    }

    internal class ConvertAnimationItemList : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender is AnimatedObject)
                (sender as AnimatedObject).MakeRelativeImages(document as IDocument);
            var entity = value as AnimationItemList;
            if (entity == null)
                return (new AnimationItemList()).ToDictionary();

            return (new AnimationItemList(entity.OrderBy(a => a.Value).ToList())).ToDictionary();
        }
        public override Type StorageType
        {
            get
            {
                return typeof(List<object>);
            }
        }
    }
}
