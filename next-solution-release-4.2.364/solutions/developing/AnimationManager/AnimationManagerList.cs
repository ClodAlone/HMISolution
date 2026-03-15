using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AnimationManager
{
    [CollectionDataContract
                (Name = "AnimationManagerList",
                ItemName = "entry")]
    [KnownType("GetKnownTypes")]
    public class AnimationManagerList : List<AnimationManager> 
    {
        static Type[] GetKnownTypes()
        {
            var list = AnimationManager.LoadAnimationTypes();
            var array = new List<Type>();
            foreach (var s in list)
                array.Add(AnimationManager.GetAnimationType(s));
            return array.ToArray();
        }
    }
}
