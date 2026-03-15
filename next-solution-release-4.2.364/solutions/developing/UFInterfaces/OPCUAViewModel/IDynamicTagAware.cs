using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;

namespace UFInterfaces
{
    public interface IDynamicTagAware
    {
        Dictionary<String, String> GetMapDynamics();
        void PreserveTagsFromMap(Dictionary<String, String> map);
        bool MatchTypeDefinition(String relative, String absolute);
        void UpdateMapDynamics(Dictionary<String, String> map);
        void SetConverterLabel(string label);
    }
    public interface IStatisticTagAware
    {
        int GetStatisticType();
    }
}
