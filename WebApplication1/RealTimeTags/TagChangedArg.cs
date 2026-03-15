
using Opc.Ua;
using RealTimeData;

namespace RealTimeTags
{
    internal class TagChangedArg : EventArgs
    {
        Dictionary<String, StatusCode> mapStatus;
        IEnumerable<Tag> ListChanges;
        internal TagChangedArg(IEnumerable<Tag> listChanges)
        {
            ListChanges = listChanges;
        }

        internal IEnumerable<Tag> GetListChanges()
        {
            return ListChanges;
        }

        internal void SetError(String guid, StatusCode statusCode)
        {
            if (mapStatus == null)
                mapStatus = new Dictionary<String, StatusCode>();
            if (mapStatus.ContainsKey(guid))
                mapStatus.Remove(guid);
            mapStatus.Add(guid, statusCode);
        }
        
        internal bool IsGood(Tag tag)
        {
            if (mapStatus == null)
                return true;
            return !mapStatus.ContainsKey(tag.id) || StatusCode.IsGood(mapStatus[tag.id]);
        }
    }
}