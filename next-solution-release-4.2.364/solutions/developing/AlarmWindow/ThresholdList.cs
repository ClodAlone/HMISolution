using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Utilities;
using System.Windows.Media;
 
namespace AlarmWindow
{
    public class ThresholdList : ObservableCollection<ThresholdSettings>
    {
        #region Constructors
        public ThresholdList() 
        { }

        public ThresholdList(IEnumerable<ThresholdSettings> thresholdList) :
            base(thresholdList)
        { }

        public ThresholdList(ThresholdList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new ThresholdSettings(item));
        }
        #endregion

        public List<object> ToDictionary()
        {
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary()));
            return res;
        }
		
		public void InitThreshold()
        {
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 239, 105), Colors.Black, 0));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 204, 148), Colors.Black, 10));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 154, 108), Colors.Black, 20));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 107, 58), Colors.Black, 30));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 63, 0), Colors.Black, 40));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 190, 190), Colors.Black, 50));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 145, 145), Colors.Black, 60));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 116, 116), Colors.Black, 70));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 77, 77), Colors.Black, 80));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 53, 53), Colors.Black, 90));
            Add(new ThresholdSettings(Color.FromArgb(255, 255, 29, 29), Colors.Black, 100));
        }
    }


    public class ConvertThresholdList : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            var entity = value as ThresholdList;
            if (entity == null)
                entity = new ThresholdList();

			 if (entity.Count == 0)
				entity.InitThreshold();
				
            return (new ThresholdList(entity.OrderBy(t => t.ThresholdValue))).ToDictionary();
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }
}
