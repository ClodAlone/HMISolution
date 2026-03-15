<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeBehind="GeoPage.aspx.cs" Inherits="UFWebClient.HTML5.GeoPage" %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">

    <div id="myMap" class="map" style="position: relative;width:800px; height:600px; top: 0px; left: 0px;"></div>
    
<% if (HttpContext.Current.IsDebuggingEnabled) { %>
    <script type="text/javascript" src="Scripts/jquery-3.1.1.js"></script>
    <script type="text/javascript" src="Scripts/jquery-migrate-3.0.0.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.2.1.js"></script>
<% } else { %>
    <script type="text/javascript" src="Scripts/jquery-3.1.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-migrate-3.0.0.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.2.1.min.js"></script>
<% } %>
    <script type="text/javascript" src="SignalR/Hubs"></script>

    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0"></script>
    <script type="text/javascript">
        // <![CDATA[

        var zoomRestoreId = 'zoomRestore';
        var latRestoreId = 'latRestore';
        var lonRestoreId = 'lonRestore';

        var bingMap = null;
        var initialized = false;

        window.onresize = resize;
        function resize() {
            var w = window.innerWidth;
            var h = window.innerHeight;

            myMap.style.width = w + 'px';
            myMap.style.height = h + 'px';
        }

        var projectHub = $.connection.projectHub;
        projectHub.client.done = function () {
        }

        var deferredStart = $.connection.hub.start();
        deferredStart.done(function () {
            createMap();
            projectHub.server.getListTileInfo().done(onSuccessListTileInfo).fail(onFailureListTileInfo);
        });

        deferredStart.fail(function () {
            // error
            $.connection.hub.stop()
        });

        function onFailureListTileInfo() {
        }

        function onSuccessListTileInfo(listTiles) {

            var sectionArray = [];
            var sectionTiles = [];

            $.each(listTiles, function () {
                // for (var i = 0; i < listTiles.length; i++) {

                // var sectionTiles = []; section are not fully supported
                $.each(this, function () {
                    // for (var j = 0; j < listTiles[i].Value.length; ++j) {

                    addPin(this);
                });

                try {
                    $.connection.hub.stop()
                }
                catch (e) {

                }
            });
        }

        var key = 'AlyRXdH0-7gITM8V4SpqrJg7QxwD9zzbzLIICmxrhn1LS8rtJn7bpcCym6NK0qGb';
        //var infoboxLayer = new Microsoft.Maps.Layer();
        //var pinLayer = new Microsoft.Maps.Layer();

        function addPin(coords) {

            if (!coords.HasGeoCoordinates)
                return;

            /*
            var tileParams =
            {
                uniqueId: coords.Name,
                name: coords.Name,
                appTitle: coords.Name,
                label: coords.Name,
                color: coords.Color,
                iconSrc: coords.ImageUrl,
                appUrl: "Screen.aspx?url=" + coords.Url
            };

            if (this.IsExtraLarge)
                tileParams.size = "tile-triple tile-triple-vertical";
            else if (this.IsLarge)
                tileParams.size = "tile-double tile-double-vertical";
            */


            var latLon = new Microsoft.Maps.Location(coords.Latitude, coords.Longitude);
            // var pushpinOptions = { icon: coords.ImageUrl, width: 30, height: 30 };
            var pin = new Microsoft.Maps.Pushpin(latLon,
                {
                    title: coords.Name,
                    enableHoverStyle: true,
                });

            //pin.Title = coords.Name;//usually title of the infobox
            //pin.Description = coords.ImageUrl; //information you want to display in the infobox
            //pin.setOptions();

            // Create the infobox for the pushpin
            //var pinInfobox = new Microsoft.Maps.Infobox(pin.getLocation(),
            //    {
            //        width: 150,
            //        height: 40, 
            //        title: coords.Name,
            //        // description: 'This pushpin is located at (0,0).',
            //        visible: true,
            //        //offset: new Microsoft.Maps.Point(0, 15)
            //    });

            var pinLayer = new Microsoft.Maps.Layer();
            pinLayer.add(pin);
            //pinLayer.add(pinInfobox);
            bingMap.layers.insert(pinLayer);
            //pinLayer.push(pin); //add pushpin to pinLayer
            //pinLayer.push(pinInfobox); //add pushpin to pinLayer
            Microsoft.Maps.Events.addHandler(pinLayer, 'click', function () {

                var url = "Screen.aspx?url=" + coords.Url;
                window.location = url;
            });
        }

        //function displayInfobox(e) {
        //    pinInfobox.setOptions({ title: e.target.Title, description: e.target.Description, visible: true, offset: new Microsoft.Maps.Point(0, 25) });
        //    pinInfobox.setLocation(e.target.getLocation());
        //}

        //function hideInfobox(e) {
        //    pinInfobox.setOptions({ visible: false });
        //}


        function createMap() {
            if (bingMap)
                bingMap.dispose();

            // Create the info box for the pushpin
            pinInfobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false });
            var infoboxLayer = new Microsoft.Maps.Layer();
            infoboxLayer.add(pinInfobox);

            var zoomRestore = 4;
            try {
                zoomRestore = parseFloat(localStorage[zoomRestoreId]);
            }
            catch (e) {
                zoomRestore = 4;
            }

            var latRestore = 39.106667;
            try {
                latRestore = parseFloat(localStorage[latRestoreId]);
                if (isNaN(latRestore))
                    latRestore = 39.106667;
            }
            catch (e) {
                latRestore = 39.106667;
            }

            var lonRestore = -94.676389;
            try {
                lonRestore = parseFloat(localStorage[lonRestoreId]);
                if (isNaN(lonRestore))
                    lonRestore = -94.676389;
            }
            catch (e) {
                lonRestore = -94.676389;
            }

            var mapOptions = {
                credentials: key,
                center: new Microsoft.Maps.Location(latRestore, lonRestore),
                mapTypeId: Microsoft.Maps.MapTypeId.aerial,
                enableClickableLogo: false,
                enableSearchLogo: false,
                zoom: zoomRestore,
            }

            bingMap = new Microsoft.Maps.Map(myMap, mapOptions);

            bingMap.layers.clear();
            bingMap.layers.insert(infoboxLayer);

            //bingMap.entities.clear();
            //bingMap.entities.push(pinLayer);
            //bingMap.entities.push(infoboxLayer);

            resize();

            Microsoft.Maps.Events.addHandler(bingMap, 'viewchangeend', viewChanged)

            initialized = true;
        }

        function viewChanged(e) {

            if (!initialized)
                return;

            localStorage[zoomRestoreId] = bingMap.getZoom();
            var center = bingMap.getCenter();
            localStorage[latRestoreId] = center.latitude;
            localStorage[lonRestoreId] = center.longitude;
        }
        // ]]>
    </script>

</asp:Content>
