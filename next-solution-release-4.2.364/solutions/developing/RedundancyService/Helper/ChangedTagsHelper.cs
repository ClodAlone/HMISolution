using Opc.Ua.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedundancyService.Helper
{
    public class ChangedTagsHelper : ChangedHelper<ChangedTags>
    {
        #region Declarations
        readonly List<ChangedTags> changedTags;
        #endregion

        #region Constructors
        public ChangedTagsHelper(IList<ChangedTags> changedTags)
        {
            this.changedTags = new List<ChangedTags>(changedTags.Count);
            this.changedTags.AddRange(changedTags);
        }

        public ChangedTagsHelper(ChangedTags changedTags) : 
            this(new List<ChangedTags>() { changedTags })
        { }
        #endregion

        #region Overrides
        protected override int GetElementsCount()
        {
            return changedTags.Count;
        }
        protected override int GetValuesCount()
        {
            return changedTags[skipElements].DataValues.Count;
        }

        public override List<ChangedTags> GetChangedElementsBlock()
        {
            var tags = new List<ChangedTags>();

            if (changedTags.Count > 0)
            {
                UpdateCounters();

                var list = changedTags.Skip(skipElements).Take(takeElements).ToList();
                if (list.Count == 1)
                {
                    var values = list[0].DataValues.Skip(skipValues).Take(takeValues).ToList();
                    var changedTag = new ChangedTags()
                    {
                        NodeId = list[0].NodeId,
                        DataValues = new WrappedDataValueCollection(values)
                    };

                    if (list[0].Statistics != null)
                    {
                        changedTag.Statistics = new StatisticsData()
                        {
                            Min = list[0].Statistics.Min,
                            Max = list[0].Statistics.Max,
                            TotAverage = list[0].Statistics.TotAverage,
                            CountUpdates = list[0].Statistics.CountUpdates,
                            TotalTimeOn = list[0].Statistics.TotalTimeOn,
                            LastTotalTimeOn = list[0].Statistics.LastTotalTimeOn
                        };
                    };

                    tags.Add(changedTag);
                }
                else if (list.Count > 1)
                {
                    tags.AddRange(list);
                }
            }

            return tags;
        }
        #endregion
    }
}
