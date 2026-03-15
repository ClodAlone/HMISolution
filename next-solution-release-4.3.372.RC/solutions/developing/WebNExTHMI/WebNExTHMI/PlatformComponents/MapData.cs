using DocumentManager.ComponentService;
using Microsoft.AspNetCore.SignalR;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UFInterfaces;
using ViewModelLib;
using Utilities;
using Opc.Ua;

namespace WebNExTHMI.PlatformComponents
{
    public class MapData : ViewModelBase, IEntityReference
    {
        List<OPCUAEntityReference> listReferences = new List<OPCUAEntityReference>();
        Dictionary<OPCUAEntityReference, TileInfo> mapReferenceTiles =
            new Dictionary<OPCUAEntityReference, TileInfo>();
        List<OPCUAEntityReference> listReferenceIsLat = new List<OPCUAEntityReference>();

        List<PropertyObserver<OPCUAEntityReference>> listObserverReferences = new List<PropertyObserver<OPCUAEntityReference>>();

        Dictionary<OPCUAEntityReference, PropertyObserver<MonitoredItemViewModel>> mapObserverMonitoredModels =
            new Dictionary<OPCUAEntityReference, PropertyObserver<MonitoredItemViewModel>>();

        Dictionary<String, PushedMapData> pendingUpdates = new Dictionary<String, PushedMapData>();

        Object lockObjectMapData = new Object();

        bool bLoaded;

        readonly IClientProxy caller;
        readonly List<TileInfo> listTiles;
        readonly String SessionName;
        readonly String viewId;

        public MapData(String id, List<TileInfo> list, IClientProxy c, String sessionName)
        {
            viewId = id;
            caller = c;
            listTiles = list;
            SessionName = sessionName;

            PromoteIdleExecution(1);
        }

        DateTime lastTimeAlive;
        static readonly TimeSpan aliveTimeout = TimeSpan.FromSeconds(PlatformComponents.ScreenAliveTimeoutSecs);
        public void KeepAlive()
        {
            lastTimeAlive = DateTime.UtcNow;
        }

        public bool IsAlive()
        {
            if (lastTimeAlive == null)
                return false;

            return (lastTimeAlive - DateTime.UtcNow < aliveTimeout);
        }

        protected override void IdleExecution()
        {
            if (bDisposed)
                return;

            base.IdleExecution();

            SubscribeReferences();

            var tempData = new List<PushedMapData>();
            lock (pendingUpdates)
            {
                foreach (var pair in pendingUpdates)
                    tempData.Add(pair.Value);
                pendingUpdates.Clear();
            }

            if (tempData.Count > 0)
                caller.SendAsync("PositionUpdate", viewId, tempData);
        }

        void AddPendingValue(DataValue dataValue, TileInfo tile, bool isLat = false)
        {
            if (dataValue == null)
                return;

            var isGood = Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) || dataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue;
            if (!isGood)
                return;

            try
            {
                double v = Convert.ToDouble(dataValue.Value);
                lock (pendingUpdates)
                {
                    if (!pendingUpdates.ContainsKey(tile.Url))
                        pendingUpdates.Add(tile.Url, new PushedMapData() { screenUri = tile.Url });
                    if (isLat)
                        pendingUpdates[tile.Url].latValue = v;
                    else
                        pendingUpdates[tile.Url].lonValue = v;
                }

            }
            catch
            {
                return;
            }

            PromoteIdleExecution(PlatformComponents.CacheDelay);
        }

        void SubscribeReferences()
        {
            if (bLoaded)
                return;
            bLoaded = true;

            foreach(var tile in listTiles)
            {
                if (bDisposed)
                    return;

                var tagLat = tile.LatTag.FromXml<OPCUAEntityReference>();
                var tagLon = tile.LonTag.FromXml<OPCUAEntityReference>();
                if (tagLat == null || tagLon == null)
                    return;

                listReferences.Add(tagLat);
                mapReferenceTiles.Add(tagLat, tile);
                listReferenceIsLat.Add(tagLat);
                listReferences.Add(tagLon);
                mapReferenceTiles.Add(tagLon, tile);
            }

            lock (lockObjectMapData)
            {
                listReferences.ForEach(reference =>
                {
                    var observer = new PropertyObserver<OPCUAEntityReference>(reference)
                        .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                        {
                            if (n.MonitoredItemViewModel != null)
                            {
                                lock (lockObjectMapData)
                                {
                                    if (mapObserverMonitoredModels.ContainsKey(reference))
                                    {
                                        mapObserverMonitoredModels[reference].Dispose();
                                        mapObserverMonitoredModels.Remove(reference);
                                    }
                                }
                                var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                                lock (lockObjectMapData)
                                {
                                    mapObserverMonitoredModels.Add(reference, observerMonitoredModel);
                                }

                                AddPendingValue(n.MonitoredItemViewModel.DataValue,
                                    mapReferenceTiles[reference], listReferenceIsLat.Contains(reference));

                                observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                                {
                                    AddPendingValue(m.DataValue,
                                        mapReferenceTiles[reference], listReferenceIsLat.Contains(reference));
                                });

                                AddPendingValue(n.MonitoredItemViewModel.DataValue,
                                    mapReferenceTiles[reference], listReferenceIsLat.Contains(reference));
                            }
                        });

                    listObserverReferences.Add(observer);
                    
                    try
                    {
                        reference.Resolve(SessionName, PlatformComponents.GetProjectDocument());
                        reference.SetInUse(this, true);
                    }
                    catch (Exception e) { }
                });
            }
        }

        void UnsubscribeReferences()
        {
            listObserverReferences.ForEach(observer => observer.Dispose());
            listObserverReferences.Clear();
            mapObserverMonitoredModels.Values.ToList().ForEach(observer => observer.Dispose());
            mapObserverMonitoredModels.Clear();
        }

        #region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        public ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Dispose
        bool bDisposed;
        protected override void OnDispose()
        {
            base.OnDispose();

            List<OPCUAEntityReference> list = null;
            lock (lockObjectMapData)
            {
                bDisposed = true;
                if (bLoaded)
                {
                    list = new List<OPCUAEntityReference>(listReferences);
                    listReferences.Clear();
                    mapReferenceTiles.Clear();
                    listReferenceIsLat.Clear();
                }

                UnsubscribeReferences();
            }

            if (list != null)
            {
                list.ForEach(reference =>
                {
                    reference.SetInUse(this, false);
                });
            }
        }
        #endregion
    }
}
