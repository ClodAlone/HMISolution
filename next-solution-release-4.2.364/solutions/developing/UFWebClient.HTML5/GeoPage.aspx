<%@ Page Language="C#" MasterPageFile="~/Droptiles.master" AutoEventWireup="true" CodeBehind="GeoPage.aspx.cs" Inherits="UFWebClient.HTML5.GeoPage" %>

<asp:Content ContentPlaceHolderID="body" runat="server">


    <div id="navbar" class="navbar navbar-fixed-top navbar-inverse">
        <div class="navbar-inner">
            <div class="container">                    
                <div class="nav-collapse collapse">
                    <ul class="nav">
<%--                            <li class="active">                                
                            <a class="brand" href="?"><img src="img/avatar474_2.gif" style="max-height: 20px; margin-top: -2px; margin-right:5px; vertical-align: middle" />WebClient</a>
                        </li>--%>
                        <li><a class="active" href="?"><i class="icon-white icon-th-large"></i>Layout</a></li>
                        <li><a href="Report.aspx"><i class="icon-white icon-file"></i>Reports</a></li>
                        <li><a href="Alarm.aspx"><i class="icon-white icon-tags"></i>Alarms</a></li>
                        <li><a href="DataGrid.aspx"><i class="icon-white icon-book"></i>DataGrids</a></li>
                        <li><a href="DataAnalisys.aspx"><i class="icon-white icon-magnet"></i>DataAnalisys</a></li>
                        <li><a href="DashBoard.aspx"><i class="icon-white icon-tasks"></i>DashBoard</a></li>
                    </ul>
                    <ul class="nav pull-right">
                        <%--<li><a href="javascript:fullscreen()"><i class="icon-facetime-video"></i>Go Fullscreen</a></li>--%>
                        <li><a href="ServerStuff/Logout.ashx"><i class="icon-white icon-refresh"></i>Reset</a></li>

                        <li>
                            <div class="loginDisplay">
                                <i class="icon-white icon-user"></i>
                                <asp:LoginView ID="LoginView1" runat="server" EnableViewState="false">
                                    <AnonymousTemplate>
                                        [ <a href="~/Account/Login.aspx" ID="HeadLoginStatus" runat="server">Log In</a> ]
                                    </AnonymousTemplate>
                                    <LoggedInTemplate>
                                        Welcome <span class="bold"><asp:LoginName ID="HeadLoginName" runat="server" /></span>!
                                        [ <asp:LoginStatus ID="LoginStatus1" runat="server" LogoutAction="Redirect" LogoutText="Log Out" LogoutPageUrl="~/"/> ]
                                    </LoggedInTemplate>
                                </asp:LoginView>
                            </div>
                        </li>

<%--                            <li data-bind="if: user().isAnonymous"><a onclick="ui.login()" href="#login"><i class="icon-white icon-user"></i>Login</a></li>
                        <li data-bind="if: !user().isAnonymous"><a href="ServerStuff/Logout.ashx"><i class="icon-white icon-user"></i>Logout</a></li>--%>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <div id="myMap" class="map" style="position: relative;width:800px; height:600px; top: 45px; left: 0px;"></div>
    
<% if (HttpContext.Current.IsDebuggingEnabled) { %>
    <script type="text/javascript" src="Scripts/jquery-3.1.0.js"></script>
    <script type="text/javascript" src="Scripts/jquery-migrate-3.0.0.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.2.1.js"></script>
<% } else { %>
    <script type="text/javascript" src="Scripts/jquery-3.1.0.min.js"></script>
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
        var infoboxLayer = new Microsoft.Maps.EntityCollection();
        var pinLayer = new Microsoft.Maps.EntityCollection();

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
            var pin = new Microsoft.Maps.Pushpin(latLon);

            pin.Title = coords.Name;//usually title of the infobox
            pin.Description = coords.ImageUrl; //information you want to display in the infobox
            pin.setOptions();

            // Create the infobox for the pushpin
            var pinInfobox = new Microsoft.Maps.Infobox(pin.getLocation(),
                {
                    width: 150,
                    height: 40, 
                    title: coords.Name,
                    // description: 'This pushpin is located at (0,0).',
                    visible: true,
                    offset: new Microsoft.Maps.Point(0, 15)
                });

            pinLayer.push(pin); //add pushpin to pinLayer
            pinLayer.push(pinInfobox); //add pushpin to pinLayer
            Microsoft.Maps.Events.addHandler(pin, 'click', function (e) {

                var url = "Screen.aspx?url=" + coords.Url;
                window.location = url;
            });
            Microsoft.Maps.Events.addHandler(pinInfobox, 'click', function (e) {

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
            infoboxLayer.push(pinInfobox);

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

            bingMap.entities.clear();

            bingMap.entities.push(pinLayer);
            bingMap.entities.push(infoboxLayer);

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
